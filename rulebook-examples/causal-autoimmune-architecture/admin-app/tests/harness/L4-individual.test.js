// L4 — INDIVIDUAL rollups. Confirmed-node counts + cross-ancestry-confirmed
// counts + the raw leakage/holdout observations, per person.
//
// RED today: /api/individuals/:id (501). Loop 3 turns these green.

import { test, before, after, describe } from 'node:test';
import { PATIENTS, INDIVIDUAL_LEVEL } from '../oracle/dag-oracle.js';
import { CONTRACT } from './contract.js';
import { startServer, stopServer, assertAppSurfaces } from './server-fixture.js';

const FIELDS = [
  'count_confirmed_causal_nodes',
  'count_cross_ancestry_confirmed_nodes',
  'has_cryptic_relatedness_flag',
  'is_ancestry_absent_from_training',
];

const WITNESS = {
  count_confirmed_causal_nodes:
    'COUNT of this individual\'s CausalMechanisms where IsCausalArchitectureNode — the falsifiable, non-spurious, confident validated mechanisms.',
  count_cross_ancestry_confirmed_nodes:
    'COUNT of confirmed nodes that ALSO replicated in >=1 different ancestry (IsAncestryTransportable) — the transport basis.',
  has_cryptic_relatedness_flag: 'OBSERVATION: cryptic-relatedness leakage flag.',
  is_ancestry_absent_from_training: 'OBSERVATION: this ancestry is held out of training.',
};

describe('L4 · INDIVIDUAL rollups · confirmed nodes, cross-ancestry nodes, leakage/holdout', () => {
  before(startServer);
  after(stopServer);

  for (const p of PATIENTS) {
    const row = INDIVIDUAL_LEVEL[p.individual];
    for (const field of FIELDS) {
      test(`${p.key} · ${p.name} · ${field} = ${row[field]}`, async () => {
        await assertAppSurfaces(
          CONTRACT.individualLevel.endpoint(p),
          field,
          row[field],
          WITNESS[field],
        );
      });
    }
  }
});
