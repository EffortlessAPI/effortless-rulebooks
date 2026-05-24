// Playwright recorder for the Community Event Planner walkthrough.
// Drives a real browser through every step in walkthrough/README.md and
// writes screenshots/ + walkthrough-silent.webm.
import { chromium } from 'playwright';
import fs from 'node:fs';
import path from 'node:path';
import url from 'node:url';

const __dirname = path.dirname(url.fileURLToPath(import.meta.url));
const OUT_DIR = __dirname;
const SHOTS_DIR = path.join(OUT_DIR, 'screenshots');
const VIDEO_DIR = path.join(OUT_DIR, '_video');

const BASE_URL = process.env.BASE_URL || 'http://localhost:5188';

// Steps with target durations (seconds). Each step's `run` is awaited then the
// remainder of `seconds` is padded with waitForTimeout so the video timeline
// matches the narration script.
const STEPS = [
  { id: '01-login',           seconds: 6 },
  { id: '02-pick-admin',      seconds: 6 },
  { id: '03-dashboard-stats', seconds: 10 },
  { id: '04-dashboard-list',  seconds: 10 },
  { id: '05-provenance-on',   seconds: 8 },
  { id: '06-dag-eventstatus', seconds: 12 },
  { id: '07-dag-available',   seconds: 10 },
  { id: '08-open-event',      seconds: 8 },
  { id: '09-event-status',    seconds: 12 },
  { id: '10-registration',    seconds: 10 },
  { id: '11-open-edit-modal', seconds: 12 },
  { id: '12-change-deadline', seconds: 12 },
  { id: '13-save-edit',       seconds: 6 },
  { id: '14-speakers-tab',    seconds: 8 },
  { id: '15-speakers-detail', seconds: 10 },
  { id: '16-wrap-dag-recap',  seconds: 10 },
];

if (!fs.existsSync(SHOTS_DIR)) fs.mkdirSync(SHOTS_DIR, { recursive: true });
if (!fs.existsSync(VIDEO_DIR)) fs.mkdirSync(VIDEO_DIR, { recursive: true });
for (const f of fs.readdirSync(SHOTS_DIR)) fs.unlinkSync(path.join(SHOTS_DIR, f));
for (const f of fs.readdirSync(VIDEO_DIR)) fs.unlinkSync(path.join(VIDEO_DIR, f));

// Visible cursor + click ripple, painted onto the page itself so the recording
// shows where the user "is" — Playwright doesn't draw the real mouse. We wait
// for DOMContentLoaded before attaching to body so it survives React re-mounts.
const CURSOR_SCRIPT = `
  (() => {
    const install = () => {
      if (window.__cursorInstalled || !document.body) return;
      window.__cursorInstalled = true;
      const dot = document.createElement('div');
      dot.id = '__pw_cursor';
      Object.assign(dot.style, {
        position: 'fixed', top: '-100px', left: '-100px',
        width: '24px', height: '24px',
        borderRadius: '50%', background: 'rgba(239,68,68,0.95)',
        border: '3px solid white', boxShadow: '0 0 10px rgba(0,0,0,0.6)',
        pointerEvents: 'none', zIndex: '2147483647',
        transform: 'translate(-50%,-50%)',
        transition: 'left 60ms linear, top 60ms linear'
      });
      document.body.appendChild(dot);
      const move = (x, y) => {
        dot.style.left = x + 'px';
        dot.style.top  = y + 'px';
      };
      document.addEventListener('mousemove', (e) => move(e.clientX, e.clientY), true);
      document.addEventListener('mouseover', (e) => move(e.clientX, e.clientY), true);
      const ripple = (x, y, color) => {
        const r = document.createElement('div');
        Object.assign(r.style, {
          position: 'fixed', left: x + 'px', top: y + 'px',
          width: '12px', height: '12px', borderRadius: '50%',
          border: '3px solid ' + color, pointerEvents: 'none',
          transform: 'translate(-50%,-50%)', zIndex: '2147483647',
          transition: 'all 600ms ease-out', opacity: '1'
        });
        document.body.appendChild(r);
        requestAnimationFrame(() => {
          r.style.width  = '70px';
          r.style.height = '70px';
          r.style.opacity = '0';
        });
        setTimeout(() => r.remove(), 700);
      };
      document.addEventListener('click', (e) => ripple(e.clientX, e.clientY, '#ef4444'), true);
    };
    if (document.body) install();
    else document.addEventListener('DOMContentLoaded', install);
    // React rerenders may remove our element from body; re-install if it's gone.
    setInterval(() => {
      if (!document.getElementById('__pw_cursor') && document.body) {
        window.__cursorInstalled = false;
        install();
      }
    }, 500);
  })();
`;

async function glide(page, x, y, steps = 18) {
  await page.mouse.move(x, y, { steps });
  await page.waitForTimeout(120);
}

async function centerOf(loc) {
  await loc.scrollIntoViewIfNeeded().catch(() => {});
  const box = await loc.boundingBox();
  if (!box) throw new Error('no bounding box');
  return { x: box.x + box.width / 2, y: box.y + box.height / 2 };
}

async function hoverHighlight(page, loc) {
  const { x, y } = await centerOf(loc);
  await glide(page, x, y);
}

async function clickWithCursor(page, loc) {
  await hoverHighlight(page, loc);
  await loc.click();
  await page.waitForTimeout(400);
}

async function dblclickWithCursor(page, loc) {
  await hoverHighlight(page, loc);
  await loc.dblclick();
  await page.waitForTimeout(400);
}

async function shot(page, id) {
  await page.screenshot({ path: path.join(SHOTS_DIR, `${id}.png`), fullPage: false });
}

async function runStep(page, step, fn) {
  const startedAt = Date.now();
  console.log(`▶ ${step.id}  (target ${step.seconds}s)`);
  await fn();
  await shot(page, step.id);
  const used = (Date.now() - startedAt) / 1000;
  const remain = Math.max(0, step.seconds - used);
  if (remain > 0) await page.waitForTimeout(remain * 1000);
  console.log(`  done in ${((Date.now() - startedAt) / 1000).toFixed(1)}s`);
}

(async () => {
  const browser = await chromium.launch({ headless: true });
  const context = await browser.newContext({
    viewport: { width: 1280, height: 800 },
    recordVideo: { dir: VIDEO_DIR, size: { width: 1280, height: 800 } },
  });
  await context.addInitScript(CURSOR_SCRIPT);

  const page = await context.newPage();

  try {
    // 1 — Login screen, hover roles
    await runStep(page, STEPS[0], async () => {
      await page.goto(BASE_URL + '/', { waitUntil: 'networkidle' });
      await page.evaluate(() => localStorage.removeItem('user'));
      await page.reload({ waitUntil: 'networkidle' });
      await page.waitForSelector('button:has-text("Admin")');
      for (const role of ['Admin', 'Event Organizer', 'Speaker', 'Attendee']) {
        const btn = page.locator('button', { hasText: role }).first();
        if (await btn.count()) {
          await hoverHighlight(page, btn);
          await page.waitForTimeout(450);
        }
      }
    });

    // 2 — Pick Admin role
    await runStep(page, STEPS[1], async () => {
      const adminBtn = page.locator('button', { hasText: 'Admin' }).first();
      await clickWithCursor(page, adminBtn);
      await page.waitForLoadState('networkidle');
      await page.waitForSelector('h2:has-text("Dashboard")');
    });

    // 3 — Dashboard stat boxes
    await runStep(page, STEPS[2], async () => {
      for (const label of ['Total Events', 'Ready to Publish', 'Total Attendees', 'With Issues']) {
        const box = page.locator('.stat-box', { hasText: label }).first();
        if (await box.count()) {
          await hoverHighlight(page, box);
          await page.waitForTimeout(700);
        }
      }
    });

    // 4 — Event list rows
    await runStep(page, STEPS[3], async () => {
      const items = page.locator('.event-item');
      const n = await items.count();
      for (let i = 0; i < Math.min(n, 4); i++) {
        await hoverHighlight(page, items.nth(i));
        await page.waitForTimeout(800);
      }
    });

    // 5 — Toggle provenance ON
    await runStep(page, STEPS[4], async () => {
      const toggle = page.locator('button.dag-toggle');
      await clickWithCursor(page, toggle);
      await page.waitForSelector('button.dag-toggle.is-on', { timeout: 5000 });
      // Hover a couple of cells to show the glyphs appear
      const cell1 = page.locator('.event-item').first().locator('.dag-cell').first();
      if (await cell1.count()) await hoverHighlight(page, cell1);
      await page.waitForTimeout(800);
    });

    // 6 — DAG for Events.EventStatus via the "Ready to Publish" stat box's ƒ glyph
    //     (stat boxes have no parent onClick — safe target).
    await runStep(page, STEPS[5], async () => {
      const statBox = page.locator('.stat-box', { hasText: 'Ready to Publish' }).first();
      const link = statBox.locator('a[href*="/dag/Events/EventStatus"]').first();
      await clickWithCursor(page, link);
      await page.waitForURL(/\/dag\/Events\/EventStatus/, { timeout: 8000 });
      await page.waitForLoadState('networkidle');
      await page.waitForTimeout(1500);
    });

    // 7 — Back, open DAG for Events.BookedCapacity via the "Total Attendees" stat box
    await runStep(page, STEPS[6], async () => {
      await page.goBack();
      await page.waitForSelector('h2:has-text("Dashboard")');
      const statBox = page.locator('.stat-box', { hasText: 'Total Attendees' }).first();
      const link = statBox.locator('a[href*="/dag/Events/BookedCapacity"]').first();
      await clickWithCursor(page, link);
      await page.waitForURL(/\/dag\/Events\/BookedCapacity/, { timeout: 8000 });
      await page.waitForLoadState('networkidle');
      await page.waitForTimeout(1500);
    });

    // 8 — Back to dashboard, open tech-talk-1 event detail
    await runStep(page, STEPS[7], async () => {
      await page.goBack();
      await page.waitForSelector('h2:has-text("Dashboard")');
      // Provenance ON adds glyph links on the row, which would steal a click. Turn it off first.
      const toggle = page.locator('button.dag-toggle');
      await clickWithCursor(page, toggle);
      await page.waitForSelector('button.dag-toggle.is-off');
      const techTalk = page.locator('.event-item', { hasText: 'tech-talk-1' }).first();
      await clickWithCursor(page, techTalk);
      await page.waitForURL(/\/events\//, { timeout: 8000 });
      await page.waitForLoadState('networkidle');
    });

    // 9 — Event status checklist
    await runStep(page, STEPS[8], async () => {
      const statusH = page.locator('h3:has-text("Event Status")');
      await hoverHighlight(page, statusH);
      await page.waitForTimeout(600);
      const allLis = await page.locator('ul li').all();
      for (const li of allLis.slice(0, 3)) {
        await hoverHighlight(page, li);
        await page.waitForTimeout(800);
      }
    });

    // 10 — Registration block
    await runStep(page, STEPS[9], async () => {
      const regH = page.locator('h3:has-text("Registration")');
      await regH.scrollIntoViewIfNeeded();
      await hoverHighlight(page, regH);
      await page.waitForTimeout(900);
      // Hover the Deadline + Status lines (siblings of the h3)
      const regBlock = regH.locator('xpath=following-sibling::div[1]');
      const lines = regBlock.locator('div');
      const n = await lines.count();
      for (let i = 0; i < Math.min(n, 2); i++) {
        await hoverHighlight(page, lines.nth(i));
        await page.waitForTimeout(800);
      }
    });

    // 11 — Open edit modal via pencil button
    await runStep(page, STEPS[10], async () => {
      const edit = page.locator('button[title="Edit event"]');
      await edit.scrollIntoViewIfNeeded();
      await clickWithCursor(page, edit);
      await page.waitForSelector('h2:has-text("Edit Event")');
      await page.waitForTimeout(600);
      for (const lbl of ['Event Name', 'Venue', 'Event Date & Time', 'Duration', 'Registration Closes']) {
        const field = page.locator('label', { hasText: lbl }).first();
        if (await field.count()) {
          await hoverHighlight(page, field);
          await page.waitForTimeout(400);
        }
      }
    });

    // 12 — Change the Registration Closes dropdown
    await runStep(page, STEPS[11], async () => {
      const select = page.locator('select').filter({ has: page.locator('option:has-text("before event")') }).first();
      await hoverHighlight(page, select);
      await select.selectOption({ label: '7 days before event' });
      await page.waitForTimeout(1100);
      await select.selectOption({ label: '30 days before event' });
      await page.waitForTimeout(1100);
    });

    // 13 — Save
    await runStep(page, STEPS[12], async () => {
      const save = page.locator('button:has-text("Save")').first();
      await clickWithCursor(page, save);
      await page.waitForSelector('h2:has-text("Edit Event")', { state: 'detached', timeout: 8000 });
      await page.waitForLoadState('networkidle');
      const regH = page.locator('h3:has-text("Registration")');
      await regH.scrollIntoViewIfNeeded();
      await hoverHighlight(page, regH);
    });

    // 14 — Speakers tab
    await runStep(page, STEPS[13], async () => {
      const speakersTab = page.locator('button.nav-button', { hasText: 'Speakers' });
      await clickWithCursor(page, speakersTab);
      await page.waitForURL(/\/speakers/);
      await page.waitForSelector('h2:has-text("Speaker Schedule")');
    });

    // 15 — Pan over the speaker rows
    await runStep(page, STEPS[14], async () => {
      const items = page.locator('.card ul li');
      const n = await items.count();
      for (let i = 0; i < Math.min(n, 5); i++) {
        await hoverHighlight(page, items.nth(i));
        await page.waitForTimeout(550);
      }
    });

    // 16 — Wrap up: turn provenance on and open an aggregation DAG
    await runStep(page, STEPS[15], async () => {
      const toggle = page.locator('button.dag-toggle');
      await clickWithCursor(page, toggle);
      await page.waitForSelector('button.dag-toggle.is-on');
      const aggLink = page.locator('a[href*="/dag/Speakers/AssignmentCount"]').first();
      if (await aggLink.count()) {
        await clickWithCursor(page, aggLink);
        await page.waitForURL(/\/dag\/Speakers\/AssignmentCount/, { timeout: 8000 });
        await page.waitForLoadState('networkidle');
        await page.waitForTimeout(1800);
      }
    });

    console.log('\n✅ Recording complete');
  } catch (err) {
    console.error('❌ Recording failed:', err);
    await shot(page, 'ERROR').catch(() => {});
  } finally {
    await context.close();
    await browser.close();
  }

  const recordedFiles = fs.readdirSync(VIDEO_DIR).filter((f) => f.endsWith('.webm'));
  if (recordedFiles.length) {
    const src = path.join(VIDEO_DIR, recordedFiles[0]);
    const dst = path.join(OUT_DIR, 'walkthrough-silent.webm');
    fs.copyFileSync(src, dst);
    const stat = fs.statSync(dst);
    console.log(`📹 Video: ${dst}  (${(stat.size / 1024 / 1024).toFixed(2)} MB)`);
  } else {
    console.error('⚠️  No video file produced');
  }
})();
