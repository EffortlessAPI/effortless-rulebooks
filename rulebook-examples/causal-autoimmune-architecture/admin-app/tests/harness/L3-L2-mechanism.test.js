// L3/L2 — MECHANISM aggregations (L2) + verdicts (L3). The central node of the
// DAG: IsCausalArchitectureNode and IsAncestryTransportable, plus every count
// and verdict they rest on. One test per field per mechanism (7 mechanisms).
//
// RED today: /api/mechanisms/:id (501). Loop 3 turns these green.

import { test, before, after, describe } from 'node:test';
import { PATIENTS, MECHANISM_LEVEL, MECHANISM_WITNESS } from '../oracle/dag-oracle.js';
import { CONTRACT } from './contract.js';
import { startServer, stopServer, assertAppSurfaces } from './server-fixture.js';

const FIELDS = [
  'count_qualified_evidence',
  'count_modalities_supporting',
  'count_intervention_targets',
  'is_experimentally_falsifiable',
  'count_replications',
  'count_concordant_replications',
  'count_cross_ancestry_concordant',
  'replicates_across_cohorts',
  'count_neg_control_tests',
  'count_neg_control_survived',
  'survives_negative_controls',
  'is_spurious_derived',
  'causal_confidence',
  'variant_is_causal_candidate',
  'is_causal_architecture_node',
  'is_ancestry_transportable',
];

describe('L3/L2 · MECHANISM · aggregations + IsCausalArchitectureNode + IsAncestryTransportable', () => {
  before(startServer);
  after(stopServer);

  for (const p of PATIENTS) {
    const row = MECHANISM_LEVEL[p.mechanism];
    for (const field of FIELDS) {
      test(`${p.key} · ${p.mechanism} · ${field} = ${row[field]}`, async () => {
        await assertAppSurfaces(
          CONTRACT.mechanismLevel.endpoint(p),
          field,
          row[field],
          MECHANISM_WITNESS[field] || `aggregation/verdict ${field} from vw_causal_mechanisms`,
        );
      });
    }
  }
});
