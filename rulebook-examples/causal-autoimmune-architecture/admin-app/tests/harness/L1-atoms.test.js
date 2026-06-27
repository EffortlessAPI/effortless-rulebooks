// L1 — ATOMS. The per-row derivations the L2 aggregations count over:
//   - evidence atoms     (ZStat -> IsQualifiedEvidence)
//   - replication atoms  (sign/p/ancestry -> IsCrossAncestryConcordant)   [transport]
//   - control atoms      (permutation vs null -> IsSurvived)              [anti-spurious]
//   - calibration atoms  (coverage/absErr -> IsWellCalibratedBin)         [calibration]
//
// Each endpoint returns the rows for a mechanism/prediction; the harness derives
// the deciding aggregate the app must back. RED today (501); Loop 3 wires them.

import { test, before, after, describe } from 'node:test';
import {
  EVIDENCE_ATOMS,
  REPLICATION_ATOMS,
  CONTROL_ATOMS,
  CALIBRATION_ATOMS,
  PATIENTS,
} from '../oracle/dag-oracle.js';
import { CONTRACT } from './contract.js';
import { startServer, stopServer, getJson } from './server-fixture.js';

const pBy = (mech) => PATIENTS.find((x) => x.mechanism === mech);
const pByPred = (pred) => PATIENTS.find((x) => x.prediction === pred);

// count rows in the served list where a boolean-ish field is true
function countTrue(rows, field) {
  if (!Array.isArray(rows)) return null;
  return rows.filter((r) => r[field] === true).length;
}

async function assertListBacks(path, computeActual, expected, witness) {
  const { status, body } = await getJson(path);
  if (status !== 200) {
    throw new Error(
      `App does not yet surface these atoms (HTTP ${status}). Wire the endpoint to read the vw_* child rows.\n   GET ${path}\n   witness: ${witness}`,
    );
  }
  const rows = Array.isArray(body) ? body : body.rows;
  const actual = computeActual(rows);
  if (actual !== expected) {
    throw new Error(
      `Expected ${expected} but app's served rows imply ${actual}.\n   GET ${path}\n   witness: ${witness}`,
    );
  }
}

describe('L1 · ATOMS · evidence / replication / control / calibration leaves', () => {
  before(startServer);
  after(stopServer);

  // --- evidence atoms: count of IsQualifiedEvidence rows ---
  for (const [mech, o] of Object.entries(EVIDENCE_ATOMS)) {
    const p = pBy(mech);
    test(`evidence · ${p.key} · ${mech} · ${o.count_qualified} qualified`, async () => {
      await assertListBacks(
        CONTRACT.evidenceAtoms.endpoint(p),
        (rows) => countTrue(rows, 'is_qualified_evidence'),
        o.count_qualified,
        o.witness,
      );
    });
  }

  // --- replication atoms: count of IsCrossAncestryConcordant rows (transport) ---
  for (const [mech, o] of Object.entries(REPLICATION_ATOMS)) {
    const p = pBy(mech);
    test(`replication · ${p.key} · ${mech} · ${o.count_cross_ancestry_concordant} cross-ancestry concordant`, async () => {
      await assertListBacks(
        CONTRACT.replicationAtoms.endpoint(p),
        (rows) => countTrue(rows, 'is_cross_ancestry_concordant'),
        o.count_cross_ancestry_concordant,
        o.witness,
      );
    });
  }

  // --- control atoms: count of IsSurvived rows (anti-spurious) ---
  for (const [mech, o] of Object.entries(CONTROL_ATOMS)) {
    const p = pBy(mech);
    test(`control · ${p.key} · ${mech} · ${o.count_survived} survived`, async () => {
      await assertListBacks(
        CONTRACT.controlAtoms.endpoint(p),
        (rows) => countTrue(rows, 'is_survived'),
        o.count_survived,
        o.witness,
      );
    });
  }

  // --- calibration atoms: count of IsWellCalibratedBin rows ---
  for (const [pred, o] of Object.entries(CALIBRATION_ATOMS)) {
    const p = pByPred(pred);
    test(`calibration · ${p.key} · ${pred} · ${o.count_well_calibrated_bins} well-calibrated bins`, async () => {
      await assertListBacks(
        CONTRACT.calibrationAtoms.endpoint(p),
        (rows) => countTrue(rows, 'is_well_calibrated_bin'),
        o.count_well_calibrated_bins,
        o.witness,
      );
    });
  }
});
