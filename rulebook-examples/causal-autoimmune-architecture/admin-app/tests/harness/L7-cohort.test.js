// L7 (list view) — COHORT. The intake app's primary screen lists every patient
// with the keystone verdict. This proves the app surfaces the oracle as a SET:
// exactly A and G actionable, B..F not — the full coverage matrix in one screen.
//
// RED today: /api/cohort (501). Loop 1 turns this green.

import { test, before, after, describe } from 'node:test';
import { PATIENTS } from '../oracle/dag-oracle.js';
import { CONTRACT } from './contract.js';
import { startServer, stopServer, getJson } from './server-fixture.js';

describe('L7 · COHORT · the keystone list (intake home screen)', () => {
  before(startServer);
  after(stopServer);

  test('cohort lists all patients with exactly the oracle-actionable set actionable', async () => {
    const { status, body } = await getJson(CONTRACT.cohort.endpoint());
    if (status !== 200) {
      throw new Error(
        `App does not yet surface the cohort screen (HTTP ${status}). Wire /api/cohort to read vw_individual_predictions.`,
      );
    }
    const allRows = Array.isArray(body) ? body : body.rows;
    if (!Array.isArray(allRows)) throw new Error('Expected /api/cohort to return a list of predictions.');

    // The intake-write-path suite (L10) creates and then deletes transient
    // `*-harness` patients; because node:test runs test FILES concurrently, one of
    // those rows can briefly appear in this cohort query. The oracle is about the
    // real 12-patient cohort, so filter those transient rows out before asserting.
    const rows = allRows.filter((r) => !String(r.individual_prediction_id || r.individual || '').endsWith('-harness'));

    if (rows.length !== PATIENTS.length) {
      throw new Error(`Expected ${PATIENTS.length} patients in the cohort, app served ${rows.length}.`);
    }

    const actionableIds = rows
      .filter((r) => r.is_clinically_actionable === true)
      .map((r) => r.individual_prediction_id)
      .sort();
    const expectedIds = PATIENTS.filter((p) => p.actionable).map((p) => p.prediction).sort();

    if (JSON.stringify(actionableIds) !== JSON.stringify(expectedIds)) {
      throw new Error(
        `Cohort actionable set must be ${JSON.stringify(expectedIds)} but app served ${JSON.stringify(actionableIds)}. ` +
          'Every gate is exercised by both a passing and a failing patient — the whole oracle must hold as a set.',
      );
    }
  });
});
