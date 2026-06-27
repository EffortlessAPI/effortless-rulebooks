// routes/owl.js — the OWL substrate admin surface.
//
// Exposes the cross-substrate conformance receipt (inference kind #7) to the UI:
//   GET /api/owl/closure      -> the reachability closure from Postgres
//                                (vw_state_transition_rules_closure): every
//                                (from,to) pair, hop distance, asserted vs inferred.
//   GET /api/owl/conformance  -> runs owl/reason.py (the local OWL-RL reasoner) and
//                                returns whether the OWL closure == the Postgres
//                                closure, with the pair counts. PASS/FAIL receipt.
//
// This is the admin window onto the OWL work: the SAME reachability is computed by
// a recursive SQL view AND an independent OWL-RL deductive closure over a
// TransitiveProperty; agreement is the receipt.
import express from 'express';
import { exec } from 'node:child_process';
import { fileURLToPath } from 'node:url';
import path from 'node:path';
import { query } from '../db.js';

export const router = express.Router();

// Project root = three levels up (server/routes -> server -> admin-app -> project).
const PROJECT_ROOT = path.resolve(path.dirname(fileURLToPath(import.meta.url)), '..', '..', '..');

// GET /api/owl/closure — the Postgres reachability closure, machine + readable labels.
router.get('/closure', async (_req, res) => {
  try {
    const rows = await query(
      `SELECT from_id, to_id, hop_distance, is_inferred
         FROM vw_state_transition_rules_closure
        ORDER BY from_id, hop_distance, to_id`,
    );
    // Derive a compact display label (strip the machine prefix) and the machine name.
    const shaped = rows.map((r) => {
      const machine = String(r.from_id).split('--')[0];
      const fromState = String(r.from_id).split('--').slice(1).join('--');
      const toState = String(r.to_id).split('--').slice(1).join('--');
      return { ...r, machine, from_state: fromState, to_state: toState };
    });
    const asserted = shaped.filter((r) => !r.is_inferred).length;
    const inferred = shaped.filter((r) => r.is_inferred).length;
    res.json({
      pairs: shaped,
      counts: { total: shaped.length, asserted, inferred, maxHop: Math.max(0, ...shaped.map((r) => r.hop_distance)) },
    });
  } catch (err) { res.status(500).json({ error: String(err) }); }
});

// GET /api/owl/conformance — run the OWL-RL reasoner + referee, parse its output.
// reason.py exits 0 on agreement and prints the counts + a RECEIPT/MISMATCH line.
router.get('/conformance', async (_req, res) => {
  const script = path.join(PROJECT_ROOT, 'owl', 'reason.py');
  exec(`python3 ${JSON.stringify(script)}`, { cwd: PROJECT_ROOT, timeout: 60000 }, (err, stdout, stderr) => {
    const out = String(stdout || '');
    // Parse the counts the script prints, e.g.:
    //   asserted edges      OWL= 14   PG= 14
    //   full reachability   OWL= 32   PG= 32
    //   inferred (closed-asserted)  OWL= 18   PG= 18
    const num = (re) => { const m = out.match(re); return m ? Number(m[1]) : null; };
    const owlTotal = num(/full reachability\s+OWL=\s*(\d+)/);
    const pgTotal = num(/full reachability\s+OWL=\s*\d+\s+PG=\s*(\d+)/);
    const owlAsserted = num(/asserted edges\s+OWL=\s*(\d+)/);
    const owlInferred = num(/inferred[^O]*OWL=\s*(\d+)/);
    const pass = /RECEIPT: the two substrates agree EXACTLY/.test(out) && !err;
    res.json({
      pass,
      exitCode: err ? (err.code ?? 1) : 0,
      counts: { owlTotal, pgTotal, owlAsserted, owlInferred },
      headlinePairPresent: /intake -> actionable[\s\S]*in OWL closure: True\s+in PG closure: True/.test(out),
      raw: out.trim(),
      stderr: String(stderr || '').trim() || null,
    });
  });
});

export default router;
