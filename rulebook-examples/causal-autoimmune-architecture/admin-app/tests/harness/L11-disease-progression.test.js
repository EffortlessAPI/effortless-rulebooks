// L11 — THE DISEASE-STATE SIMULATOR (v2). The layer the v1 audit said didn't exist:
// not "is the evidence actionable?" (the gates) but "is the DISEASE progressing?".
//
// Disease is modeled as a witnessed state machine (lupus-nephritis-progression) whose
// CURRENT STATE is derived purely from raw serology leaves (anti-dsDNA trend, complement
// trend, proteinuria, urinary sediment) — never hand-set. Two assertions per individual:
//   L11   — the derived state + activity scalars  (served on /api/individuals/:id)
//   L11b  — the full progression WALK             (served on /api/individuals/:id/progression)
//
// The marquee case is Diego (D): the simulator places him at BiopsyIndicated and
// is_disease_progressing = true, EVEN THOUGH his prediction is NOT clinically actionable
// (it fails the cryptic-relatedness gate). The two layers are independent — see
// L5d-treatment-line for the explicit disagreement assertion.

import { test, before, after, describe } from 'node:test';
import {
  PROGRESSION_INDIVIDUALS, DISEASE_STATE, DISEASE_STATE_WITNESS, PROGRESSION_PATHS,
} from '../oracle/dag-oracle.js';
import { CONTRACT } from './contract.js';
import { startServer, stopServer, assertAppSurfaces, getJson } from './server-fixture.js';
import assert from 'node:assert/strict';

const DISEASE_FIELDS = ['nephritis_progression_state_key', 'latest_sledai_score', 'activity_tier', 'is_disease_progressing'];

describe('L11 · Disease-state simulator · disease state derived from raw serology (the layer the v1 audit asked for)', () => {
  before(startServer);
  after(stopServer);

  for (const e of PROGRESSION_INDIVIDUALS) {
    const row = DISEASE_STATE[e.individual];
    if (!row) continue;
    for (const field of DISEASE_FIELDS) {
      test(`${e.key} · ${e.name} · ${field} = ${row[field]}`, async () => {
        await assertAppSurfaces(CONTRACT.diseaseState.endpoint(e), field, row[field], DISEASE_STATE_WITNESS[field]);
      });
    }
  }

  // L11b — the full progression WALK (same suite, so the shared pool stays open).
  for (const e of PROGRESSION_INDIVIDUALS) {
    const o = PROGRESSION_PATHS[e.individual];
    if (!o) continue;
    test(`${e.key} · ${e.name} · ${o.states.join(' → ')} (current ${o.current})`, async () => {
      const { status, body } = await getJson(CONTRACT.progression.endpoint(e));
      assert.equal(status, 200, `progression endpoint not surfaced (HTTP ${status}) for ${e.individual}`);
      assert.deepEqual(body.states, o.states, `expected progression path ${JSON.stringify(o.states)}, got ${JSON.stringify(body.states)}`);
      assert.equal(body.current, o.current, `expected current state ${o.current}, got ${body.current}`);
    });
  }
});
