// problemWatch.js -- EVERY ERROR A PERSON SEES IN THE RULEBOOK EDITOR REACHES EFFORTLESS TECH SUPPORT.
//
// Owner, 2026-11-15: "fix ALL of the places errors occur. We may not choose to act on all of them, but we should be
// able to see them, and deal with them." A port of effortlessapi-www's site watcher
// (www-rulebook/src/lib/problemWatch.ts, same rules): it watches what is on screen, not each place that draws an
// error. An element whose rendered text or background is red, with a sentence in it, is reported once to the www
// API's POST /api/support/problems, which files a deduped operator ticket (app 'rulebook-editor'). An error the page's
// own code throws (window 'error', an unhandled rejection) is reported as a script error. The editor's own API answers
// a failed request with a message the page shows in red, so its server-side errors are covered here too.
//
// What is NOT reported: a message that already says "We've been told (ticket tkt-...)" (MUTES), and anything inside
// an element carrying data-problem-mute (its reason is written beside it). The person's own input mistakes (CUSTOMER)
// ARE reported, as customer problems (customer: true): quiet on our side, filed under "Customer errors".
//
// OFF unless the run that started the editor named the www API: a project run's pw-boot.sh sets
// VITE_EFFORTLESS_SUPPORT_URL (and VITE_EFFORTLESS_PROJECT_ID). The editor launched on a laptop by edit-rulebook.sh
// has no such address, so nothing is watched and nothing is sent; it never guesses a host. No key either: the
// route is public (CORS *, throttled per person/IP), and the editor runs in a customer's container.
//
// The same file, byte for byte, is effortless-rulebook-editor's problem-watch.js (its boot page, /__boot/); that
// tool's scripts/check.sh fails when the two differ.

// ---- colour ---------------------------------------------------------------------------------------------------

/** 'rgb(180, 35, 24)', 'rgba(255,122,110,0.13)' and the modern 'rgb(180 35 24 / 13%)'; anything else is not a colour. */
export const parseColor = (css) => {
  if (!css) return null;
  const m = /^rgba?\(\s*([\d.]+)[\s,]+([\d.]+)[\s,]+([\d.]+)(?:\s*[,/]\s*([\d.]+)(%?))?\s*\)$/i.exec(String(css).trim());
  if (!m) return null;
  const a = m[4] === undefined ? 1 : m[5] === '%' ? Number(m[4]) / 100 : Number(m[4]);
  return { r: Number(m[1]), g: Number(m[2]), b: Number(m[3]), a };
};

/** Red as a person sees it: the hue is near 0, it is clearly coloured, and it is not so faint it is only a tint of the page. */
export const isRed = (c) => {
  if (!c || c.a < 0.05) return false;
  const r = c.r / 255, g = c.g / 255, b = c.b / 255;
  const max = Math.max(r, g, b), min = Math.min(r, g, b), d = max - min;
  if (d === 0 || max !== r) return false; // grey, or a hue that is not red-ish (green/blue dominates)
  const l = (max + min) / 2;
  const s = d / (1 - Math.abs(2 * l - 1));
  let h = (60 * ((g - b) / d)) % 360;
  if (h < 0) h += 360;
  const nearRed = h <= 18 || h >= 340;
  return nearRed && s >= 0.45 && l >= 0.2 && l <= 0.97;
};

// ---- what a message is ----------------------------------------------------------------------------------------

export const MIN_CHARS = 15; // 'Failed', 'Error', a badge: a state label. A message is a sentence.
export const MAX_CHARS = 800; // past this it is a page section that happens to be red, not a message

/** Said to us already: an API's own error handler opened a ticket and the message says so. */
export const ALREADY_TICKETED = /We've been told \(ticket tkt-[0-9a-z]+\)/i;

/** Muted on purpose: not reported at all. Every entry says why; a rule with no reason fails the test. */
export const MUTES = [
  { name: 'already ticketed', text: ALREADY_TICKETED, reason: 'the API opened a ticket for this failure and told the person its id; a second one would be a duplicate' },
];

/** The person's own mistake: REPORTED, as a customer problem (quiet, filed under 'Customer errors'). Each says why. */
export const CUSTOMER = [
  { name: 'form entry', text: /^(please )?(enter|choose|select|type|provide|pick) (a|an|the|your)\b|\bis required\b|\bmust be (a|an|at least|at most|between|unique)\b|\b(do not|don't|does not|doesn't) match\b|^invalid (email|code|password|name|address)\b/i, reason: 'the person’s own input is incomplete; the field says how to fix it and nothing on our side is broken' },
];

const matches = (rules, text, route) => rules.find((m) => (m.text && m.text.test(text)) || (m.route && m.route.test(route))) || null;

const ID_SEGMENT = /^(?:[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}|[0-9a-f]{12,}|\d+|(?:tkt|sup|pj|studio|rec|cart|c|prj)-[0-9a-z]{4,}|[A-Za-z0-9_-]{24,})$/i;
/** The page's pattern, never its ids: /run/1dcc98fe-.../editor -> /run/:id/editor. */
export const routePattern = (pathname) => String(pathname || '').split('?')[0].split('/').map((s) => (ID_SEGMENT.test(s) ? ':id' : s)).join('/') || '/';
/** The editor is one page whose screen is ?view=: the route keeps the view, never the table, row or field it shows. */
export const editorRoute = (pathname, search) => {
  const v = new URLSearchParams(search || '').get('view');
  return routePattern(pathname) + (v ? '?view=' + encodeURIComponent(v) : '');
};

/** The same message with its numbers, times and hashes made alike, so 'in 3 minutes' and 'in 4 minutes' are one. */
export const normalize = (text) =>
  String(text).replace(/\s+/g, ' ').trim().replace(/\b[0-9a-f]{7,}\b/gi, '#').replace(/\d+/g, '#').slice(0, MAX_CHARS);

// ---- finding them ---------------------------------------------------------------------------------------------
//
// A node, as the finder sees it (the real DOM through view() below, a plain object in the tests):
//   { tag, text (innerText), children, color, background (computed), hidden, muted (data-problem-mute) }

const NOT_MESSAGES = new Set(['BUTTON', 'A', 'INPUT', 'SELECT', 'TEXTAREA', 'OPTION', 'LABEL', 'SVG', 'SCRIPT', 'STYLE', 'CODE', 'PRE']);
/** A red button or link is an action ('Delete') unless it SAYS something failed. */
const SAYS_FAILED = /\b(failed|failure|error|errors|not answering|could not|couldn't|can't|cannot|unable|problem|refused|stuck|went wrong|not saved)\b/i;
/** Headings, buttons and links: red inside them is an accent, not news. */
const QUIET = new Set(['H1', 'H2', 'H3', 'H4', 'H5', 'H6', 'BUTTON', 'A']);
/** ALL CAPS is a label ('FAILED', a state pill), never a sentence. */
const isLabel = (text) => /[A-Z]/.test(text) && text === text.toUpperCase();
const isMessage = (n, text, quiet) => {
  const tag = n.tag.toUpperCase();
  if (NOT_MESSAGES.has(tag) && tag !== 'BUTTON' && tag !== 'A') return false;
  if (isLabel(text)) return false;
  if (quiet || QUIET.has(tag)) return SAYS_FAILED.test(text);
  return true;
};

/** The topmost red region with a sentence in it, per branch. A red box with a red word inside it is ONE message. */
export const findRedMessages = (root, route) => {
  const out = [];
  const walk = (n, quiet) => {
    if (n.hidden || n.muted) return;
    const red = isRed(parseColor(n.color)) || isRed(parseColor(n.background));
    const text = n.text.replace(/\s+/g, ' ').trim();
    if (red && isMessage(n, text, quiet)) {
      if (text.length >= MIN_CHARS && text.length <= MAX_CHARS && !matches(MUTES, text, route)) {
        out.push(Object.assign({ text, key: route + '|' + normalize(text) }, matches(CUSTOMER, text, route) ? { customer: true } : {}));
      }
      if (text.length >= MIN_CHARS) return; // the box is the message: its red words are not separate ones
    }
    const inner = quiet || QUIET.has(n.tag.toUpperCase());
    n.children.forEach((c) => walk(c, inner));
  };
  walk(root, false);
  return out;
};

/** What a thrown thing says, as one line: 'TypeError: x is undefined'. */
export const scriptErrorText = (e, fallback) => {
  if (e instanceof Error) return (e.name + ': ' + e.message).slice(0, 1000);
  if (typeof e === 'string' && e) return e.slice(0, 1000);
  if (e && typeof e === 'object' && 'message' in e) return String(e.message).slice(0, 1000);
  return (fallback || 'An unknown error was thrown').slice(0, 1000);
};

// ---- where they go --------------------------------------------------------------------------------------------

/**
 * The www API this page reports to, or null: then the watch is OFF. `supportUrl` is the www API's base as the
 * BROWSER reaches it (pw-boot.sh's VITE_EFFORTLESS_SUPPORT_URL); `route` fixes the reported route (the boot page).
 */
export const supportConfig = (opts) => {
  const o = opts || {};
  const base = String(o.supportUrl || '').trim().replace(/\/+$/, '');
  if (!/^https?:\/\/[^/]/i.test(base)) return null;
  const projectId = String(o.projectId || '').trim();
  return { endpoint: base + '/api/support/problems', projectId: projectId || null, route: o.route || null };
};

/** One report; never throws and never blocks the page. No cookies, no key: the route is public. */
export const postProblem = async (cfg, body) => {
  try {
    await fetch(cfg.endpoint, {
      method: 'POST',
      credentials: 'omit',
      headers: { 'content-type': 'application/json' },
      body: JSON.stringify(Object.assign({ app: 'rulebook-editor' }, body, cfg.projectId ? { projectId: cfg.projectId } : {})),
    });
  } catch { /* the page does not tell a person that its own report failed */ }
};

export const sendProblem = (cfg, found, route) =>
  postProblem(cfg, Object.assign({ text: found.text.slice(0, 1000), route }, found.customer ? { customer: true } : {}));

const SCRIPT_MAX_PER_PAGE_LOAD = 20; // a page throwing in a loop is one problem with a count on our side, not a flood
const SCRIPT_ASKED = new Set();
export const sendScriptError = (cfg, text, stack, route) => {
  const key = route + '|' + normalize(text);
  if (SCRIPT_ASKED.has(key) || SCRIPT_ASKED.size >= SCRIPT_MAX_PER_PAGE_LOAD) return Promise.resolve();
  SCRIPT_ASKED.add(key);
  return postProblem(cfg, { kind: 'script', text, stack: stack ? String(stack).slice(0, 1500) : undefined, route });
};

// ---- the browser ----------------------------------------------------------------------------------------------

const ASKED = new Set();
const SETTLE_MS = 1500; // a message must still be there after this: a flash while a page loads is not a problem
const SEEN_ONCE = new Map();

let budget = 0; // elements looked at per scan: a huge page is looked at in part, never allowed to slow the page
const view = (el) => {
  budget -= 1;
  const cs = getComputedStyle(el);
  const rect = el.getBoundingClientRect ? el.getBoundingClientRect() : null;
  const hidden = cs.display === 'none' || cs.visibility === 'hidden' || el.getAttribute('aria-hidden') === 'true' || !rect || (rect.width === 0 && rect.height === 0);
  return {
    tag: el.tagName,
    text: el.innerText != null ? el.innerText : (el.textContent || ''),
    children: hidden || budget <= 0 ? [] : Array.from(el.children).map(view),
    color: cs.color,
    background: cs.backgroundColor,
    hidden,
    muted: el.hasAttribute('data-problem-mute'),
  };
};

const scan = (cfg, root, route) => {
  const now = Date.now();
  const stillThere = new Set();
  budget = 6000;
  for (const f of findRedMessages(view(root), route)) {
    stillThere.add(f.key);
    if (ASKED.has(f.key)) continue;
    const first = SEEN_ONCE.get(f.key);
    if (first === undefined) { SEEN_ONCE.set(f.key, now); continue; }
    if (now - first < SETTLE_MS) continue;
    ASKED.add(f.key);
    void sendProblem(cfg, f, route);
  }
  for (const k of Array.from(SEEN_ONCE.keys())) if (!stillThere.has(k)) SEEN_ONCE.delete(k);
};

let installed = false;
/**
 * Once, at the page's root: { supportUrl, projectId, route? }. Without a www address it does nothing at all.
 * Returns the stop function.
 */
export const installProblemWatch = (opts) => {
  const cfg = supportConfig(opts);
  if (!cfg || installed || typeof document === 'undefined' || typeof MutationObserver === 'undefined') return () => {};
  installed = true;
  const routeNow = () => cfg.route || editorRoute(location.pathname, location.search);
  let timer = null;
  const later = () => {
    if (timer) return;
    timer = setTimeout(() => {
      timer = null;
      try { scan(cfg, document.body, routeNow()); } catch { /* watching never breaks the page */ }
      // anything seen once is looked at again after it has had time to settle
      if (SEEN_ONCE.size) later();
    }, SETTLE_MS);
  };
  const mo = new MutationObserver(later);
  mo.observe(document.body, { childList: true, subtree: true, characterData: true });
  later();
  const onError = (ev) => {
    if (!ev.error && !ev.message) return; // a resource that failed to load (an <img>): not a script error
    void sendScriptError(cfg, scriptErrorText(ev.error, ev.message), ev.error instanceof Error ? ev.error.stack : undefined, routeNow());
  };
  const onRejection = (ev) => {
    void sendScriptError(cfg, scriptErrorText(ev.reason, 'A promise was rejected with nothing'), ev.reason instanceof Error ? ev.reason.stack : undefined, routeNow());
  };
  window.addEventListener('error', onError);
  window.addEventListener('unhandledrejection', onRejection);
  return () => {
    mo.disconnect(); if (timer) clearTimeout(timer); installed = false;
    window.removeEventListener('error', onError);
    window.removeEventListener('unhandledrejection', onRejection);
  };
};
