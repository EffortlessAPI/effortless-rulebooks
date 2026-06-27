// L8 — CASE LIFECYCLE. Each of the 7 cases walks its OWN branch through the
// diagnosis-lifecycle state machine, diverging at its single deciding gate and
// ending at Actionable / NotActionable. The expected path is DERIVED from the
// frozen decidingGate in the oracle, so it can never drift from the keystone.
//
// The app must surface the ordered StateKeys via
//   GET /api/predictions/:id/lifecycle  ->  { states:[...], terminal, is_clinically_actionable }
// and the IsCurrent occupancy's StateKey must equal the derived LifecycleStateKey.

import { test, before, after, describe } from 'node:test';
import { PATIENTS, LIFECYCLE_PATHS } from '../oracle/dag-oracle.js';
import { CONTRACT } from './contract.js';
import { startServer, stopServer, assertAppSurfaces } from './server-fixture.js';

describe('L8 · CASE LIFECYCLE · each case walks its branch to the diagnosis', () => {
  before(startServer);
  after(stopServer);

  for (const p of PATIENTS) {
    const o = LIFECYCLE_PATHS[p.prediction];
    test(`${p.key} · ${p.name} · ${o.states.join(' → ')} [${p.decidingGate}]`, async () => {
      const ep = CONTRACT.lifecycle.endpoint(p);
      // ordered path of visited states
      await assertAppSurfaces(ep, 'states', o.states, o.witness);
      // terminal state
      await assertAppSurfaces(ep, 'terminal', o.terminal, o.witness);
      // and the keystone the walk lands on
      await assertAppSurfaces(ep, 'is_clinically_actionable', o.is_clinically_actionable, o.witness);
    });
  }
});
