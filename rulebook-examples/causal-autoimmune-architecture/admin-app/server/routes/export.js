// routes/export.js — Excel export of the model.
//
// GET /api/export.xlsx streams a workbook (one sheet per table) built by the
// rulebook-to-xlsx transpiler. We export the rulebook hub (schema + seed data);
// every calculated / lookup / aggregation column is included by the transpiler.
//
// (A future enhancement is the full live-data path: snapshot vw_* into a
// rulebook-export.json first, then transpile that — see effortless-excel-export.)
import express from 'express';
import { spawn } from 'node:child_process';
import { fileURLToPath } from 'node:url';
import { createReadStream, existsSync } from 'node:fs';
import { unlink } from 'node:fs/promises';
import path from 'node:path';

export const router = express.Router();

// project root = server/routes -> server -> admin-app -> project
const PROJECT_ROOT = path.resolve(path.dirname(fileURLToPath(import.meta.url)), '..', '..', '..');
const OUT_NAME = 'causal-autoimmune-export.xlsx';

let inFlight = false;

router.get('/export.xlsx', (_req, res) => {
  if (inFlight) return res.status(409).json({ error: 'an export is already running' });
  inFlight = true;
  const outPath = path.join(PROJECT_ROOT, OUT_NAME);
  // Transpiler wants a relative -o; run from project root.
  const child = spawn('effortless', [
    'rulebook-to-xlsx',
    '-i', 'effortless-rulebook/effortless-rulebook.json',
    '-o', OUT_NAME,
  ], { cwd: PROJECT_ROOT });

  let stderr = '';
  child.stderr.on('data', (d) => { stderr += d.toString(); });
  child.on('error', (err) => { inFlight = false; res.status(500).json({ error: String(err) }); });
  child.on('close', async (code) => {
    inFlight = false;
    if (code !== 0 || !existsSync(outPath)) {
      return res.status(500).json({ error: 'export_failed', code, stderr: stderr.split('\n').slice(-8).join('\n') });
    }
    res.setHeader('Content-Type', 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet');
    res.setHeader('Content-Disposition', `attachment; filename="${OUT_NAME}"`);
    const stream = createReadStream(outPath);
    stream.pipe(res);
    stream.on('close', () => { unlink(outPath).catch(() => {}); });
  });
});
