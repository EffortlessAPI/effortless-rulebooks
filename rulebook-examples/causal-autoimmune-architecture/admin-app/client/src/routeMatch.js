// routeMatch.js — map concrete URLs (/diagnosis/case/pred-a) to route templates
// (/diagnosis/case/:predictionId) and back. Ported verbatim from the Effortless
// portal's routeMatch.ts (TypeScript types stripped). Zero dependencies.

export function segs(p) {
  return p.split('/').filter(Boolean);
}

export function isParamSeg(s) {
  return s.startsWith(':');
}

// Does `template` (e.g. /cases/:id/qualify) match the concrete `path`
// (/cases/abc/qualify)? Returns captured params or null on no match.
export function matchTemplate(template, path) {
  const t = segs(template);
  const p = segs(path);
  if (t.length !== p.length) return null;
  const params = {};
  for (let i = 0; i < t.length; i++) {
    if (isParamSeg(t[i])) params[t[i].slice(1)] = decodeURIComponent(p[i]);
    else if (t[i] !== p[i]) return null;
  }
  return params;
}

// The nav row whose route template best matches the concrete path
// (prefers the match with the most literal, non-param segments).
export function matchNavRoute(rows, path) {
  let best = null;
  let bestLiterals = -1;
  for (const r of rows) {
    if (!r.route) continue;
    if (!matchTemplate(r.route, path)) continue;
    const literals = segs(r.route).filter((s) => !isParamSeg(s)).length;
    if (literals > bestLiterals) {
      best = r;
      bestLiterals = literals;
    }
  }
  return best;
}

// Substitute concrete params into a template: fillTemplate('/cases/:id', {id:'x'}) -> '/cases/x'.
export function fillTemplate(template, params) {
  return '/' + segs(template).map((s) => (isParamSeg(s) ? params[s.slice(1)] ?? s : s)).join('/');
}
