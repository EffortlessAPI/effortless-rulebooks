// L0/L1 — VARIANT candidacy. The lowest derived bit (IsCausalCandidate) sitting
// directly on raw observations (allele frequency, ASE). The witness makes clear
// candidacy alone does NOT confirm a node — it is one input to the mechanism.
//
// RED today: /api/variants/:id (501). Loop 3 turns these green.

import { test, before, after, describe } from 'node:test';
import { PATIENTS, VARIANT_LEVEL, VARIANT_WITNESS } from '../oracle/dag-oracle.js';
import { CONTRACT } from './contract.js';
import { startServer, stopServer, assertAppSurfaces } from './server-fixture.js';

const FIELDS = ['allele_frequency', 'is_rare_variant', 'has_allele_specific_expression', 'is_causal_candidate'];

describe('L0/L1 · VARIANT · IsCausalCandidate on raw allele-frequency + ASE', () => {
  before(startServer);
  after(stopServer);

  for (const p of PATIENTS) {
    const row = VARIANT_LEVEL[p.variant];
    if (!row) continue; // representative subset of variants is asserted
    for (const field of FIELDS) {
      test(`${p.key} · ${p.variant} · ${field} = ${row[field]}`, async () => {
        await assertAppSurfaces(CONTRACT.variantLevel.endpoint(p), field, row[field], VARIANT_WITNESS);
      });
    }
  }
});
