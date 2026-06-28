// L7 — KEYSTONE. The single terminal boolean, for all 7 oracle patients.
// Each test names the raw facts and the witnessed line of reasoning, then
// requires the APP to surface IsClinicallyActionable === expected.
//
// RED today: /api/predictions/:id returns 501. Loop 1 turns these green.

import { test, before, after, describe } from 'node:test';
import { PATIENTS, KEYSTONE } from '../oracle/dag-oracle.js';
import { CONTRACT } from './contract.js';
import { startServer, stopServer, assertAppSurfaces } from './server-fixture.js';

describe('L7 · KEYSTONE · IsClinicallyActionable (the whole chain resolves)', () => {
  before(startServer);
  after(stopServer);

  for (const p of PATIENTS) {
    const o = KEYSTONE[p.prediction];
    const verdict = o.expected.is_clinically_actionable ? 'TRUE' : 'FALSE';
    test(`${p.key} · ${p.name} (${p.ancestry}${p.holdout ? ', holdout' : ''}) ⇒ actionable=${verdict} [${p.decidingGate}]`, async () => {
      // raw facts: ${o.rawFacts}
      // witness:   ${o.witness}
      await assertAppSurfaces(
        CONTRACT.keystone.endpoint(p),
        'is_clinically_actionable',
        o.expected.is_clinically_actionable,
        o.witness,
      );
    });
  }
});
