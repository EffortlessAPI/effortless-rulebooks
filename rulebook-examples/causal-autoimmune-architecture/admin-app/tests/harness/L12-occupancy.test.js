// L12 — THE PROGRESSION MACHINE'S ADMIN WITNESS (Loop 10). The disease-state
// simulator (L11) is now BOUND into the generic state-machine admin: each state
// of the lupus-nephritis-progression machine reports its live cohort occupancy —
// how many subjects are CURRENTLY in that state — served by the same generic
// /api/state-machines/:id endpoint that drives the admin viewer.
//
// The occupancy is DERIVED (count of IsCurrent rows on the bitemporal occupancy
// chain), never entered. We don't hand-pin a separate constant: the EXPECTED
// per-state distribution is COMPUTED from the frozen DISEASE_STATE oracle (the
// same nephritis_progression_state_key every L11 test asserts), so this test can
// never drift from the keystone/verdict contract. If a patient's derived state
// changes, both L11 and this occupancy roll-up move together.

import { test, before, after, describe } from 'node:test';
import assert from 'node:assert/strict';
import { DISEASE_STATE } from '../oracle/dag-oracle.js';
import { CONTRACT } from './contract.js';
import { startServer, stopServer, getJson } from './server-fixture.js';

const MACHINE = 'lupus-nephritis-progression';

// Expected CURRENT occupancy per state = how many oracle individuals derive to
// that nephritis_progression_state_key. This is the corpus-level roll-up of L11.
const EXPECTED_OCCUPANCY = Object.values(DISEASE_STATE).reduce((acc, row) => {
  const k = row.nephritis_progression_state_key;
  acc[k] = (acc[k] || 0) + 1;
  return acc;
}, {});

describe('L12 · Progression machine admin · per-state cohort occupancy (derived IsCurrent counts)', () => {
  before(startServer);
  after(stopServer);

  test('the state-machine detail endpoint serves the progression machine with states', async () => {
    const { status, body } = await getJson(CONTRACT.stateMachineOccupancy.endpoint(MACHINE));
    assert.equal(status, 200, `state-machine admin endpoint not surfaced (HTTP ${status}) for ${MACHINE}`);
    assert.ok(Array.isArray(body.states) && body.states.length > 0, 'expected a non-empty states array');
  });

  test('every state carries an integer current_occupancy and ever_occupancy with ever ≥ current', async () => {
    const { body } = await getJson(CONTRACT.stateMachineOccupancy.endpoint(MACHINE));
    for (const s of body.states) {
      assert.ok(Number.isInteger(s.current_occupancy), `state ${s.state_key} missing integer current_occupancy`);
      assert.ok(Number.isInteger(s.ever_occupancy), `state ${s.state_key} missing integer ever_occupancy`);
      assert.ok(s.current_occupancy >= 0, `state ${s.state_key} has negative current_occupancy`);
      assert.ok(s.ever_occupancy >= s.current_occupancy, `state ${s.state_key}: ever (${s.ever_occupancy}) < current (${s.current_occupancy})`);
    }
  });

  // The load-bearing assertion: each state's DERIVED current occupancy equals the
  // count of oracle individuals whose derived disease state is that state.
  for (const [stateKey, expected] of Object.entries(EXPECTED_OCCUPANCY)) {
    test(`${stateKey} · current cohort occupancy = ${expected} (= # oracle individuals deriving to this state)`, async () => {
      const { body } = await getJson(CONTRACT.stateMachineOccupancy.endpoint(MACHINE));
      const state = body.states.find((s) => s.state_key === stateKey);
      assert.ok(state, `progression machine is missing state ${stateKey}`);
      assert.equal(
        state.current_occupancy, expected,
        `expected ${expected} subjects currently in ${stateKey} (from the frozen DISEASE_STATE oracle) but the admin served ${state.current_occupancy}`,
      );
    });
  }

  test('total current occupancy across states = the whole derived cohort', async () => {
    const { body } = await getJson(CONTRACT.stateMachineOccupancy.endpoint(MACHINE));
    const total = body.states.reduce((sum, s) => sum + s.current_occupancy, 0);
    assert.equal(
      total, Object.keys(DISEASE_STATE).length,
      `sum of per-state current occupancy (${total}) must equal the cohort size (${Object.keys(DISEASE_STATE).length}) — every subject is in exactly one current state`,
    );
  });
});
