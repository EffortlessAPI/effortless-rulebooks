// routes/snapshot.js — "Save to rulebook" (reverse-sync DB -> rulebook).
//
// POST /api/snapshot-to-rulebook        upsert (conservative; never deletes)
// POST /api/snapshot-to-rulebook?mode=replace   full overwrite per entity
//
// This is the API half of the input spoke wired in postgres/init-db.sh: it
// snapshots every vw_* view's RAW + COMPUTED values back into
// effortless-rulebook.json, so an admin who has been nudging knobs in the UI
// (which writes the Postgres BASE tables) can make those edits — and every value
// the model derived from them — durable in the rulebook hub, surviving the next
// `effortless build`.
//
//   upsert  (default): rows present -> updated in place; rows only in the DB ->
//           added; rows only in the rulebook -> left alone. Nothing removed.
//   replace (the "full PUSH/overwrite" the UI offers): each entity that has a
//           view has its data[] rebuilt from the view, so rows deleted in the DB
//           disappear from the rulebook too. Entities with no view are still left
//           untouched, so even replace can't blank out viewless data.
//
// The heavy lifting lives in postgres/snapshot-to-rulebook.mjs (also used by
// init-db.sh) — this route just spawns it and relays its JSON summary.
import express from 'express';
import { spawn } from 'node:child_process';
import { fileURLToPath } from 'node:url';
import path from 'node:path';

export const router = express.Router();

// server/routes -> server -> admin-app -> project
const PROJECT_ROOT = path.resolve(path.dirname(fileURLToPath(import.meta.url)), '..', '..', '..');
const SCRIPT = path.join(PROJECT_ROOT, 'postgres', 'snapshot-to-rulebook.mjs');

let inFlight = false;

router.post('/snapshot-to-rulebook', (req, res) => {
  if (inFlight) return res.status(409).json({ error: 'a snapshot is already running' });
  const mode = req.query.mode === 'replace' ? 'replace' : 'upsert';
  inFlight = true;

  const args = [SCRIPT];
  if (mode === 'replace') args.push('--replace');

  // Inherit DATABASE_URL from the server env so we snapshot the same DB the API
  // reads. The script uses git/.bak as the safety net; we keep .bak on (default).
  const child = spawn('node', args, {
    cwd: PROJECT_ROOT,
    env: { ...process.env },
  });

  let stdout = '';
  let stderr = '';
  child.stdout.on('data', (d) => { stdout += d.toString(); });
  child.stderr.on('data', (d) => { stderr += d.toString(); });
  child.on('error', (err) => { inFlight = false; res.status(500).json({ error: String(err) }); });
  child.on('close', (code) => {
    inFlight = false;
    if (code !== 0) {
      return res.status(500).json({
        error: 'snapshot_failed',
        code,
        mode,
        stderr: stderr.split('\n').slice(-12).join('\n'),
      });
    }
    let summary = null;
    try { summary = JSON.parse(stdout); } catch { /* relay raw if not JSON */ }
    res.json({ ok: true, mode, summary, log: stderr.split('\n').slice(-12).join('\n') });
  });
});
