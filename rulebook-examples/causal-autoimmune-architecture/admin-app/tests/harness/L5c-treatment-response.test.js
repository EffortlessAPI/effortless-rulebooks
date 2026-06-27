// L5c — TREATMENT-RESPONSE prediction (Loop 5). The THIRD derived prediction: a
// treatment-response verdict grounded in a MECHANISM MATCH — a Treatment targets a
// CausalMechanism (FK), and the prediction fires only when that mechanism is a
// CONFIRMED causal node AND the treatment is effective (Complete/Partial, not adverse).
//
// Served by the same /api/predictions/:id panel (SELECT * exposes the new columns),
// so no new endpoint — only new assertions. One test per field per patient.
//
// The marquee assertion is again pred-b: onset is NOT actionable (calibration gate),
// yet treatment-response IS actionable — the targeted mechanism is confirmed and the
// drug responds, so the third prediction is independent of the onset verdict.

import { test, before, after, describe } from 'node:test';
import { PATIENTS, TREATMENT_LEVEL, TREATMENT_WITNESS } from '../oracle/dag-oracle.js';
import { CONTRACT } from './contract.js';
import { startServer, stopServer, assertAppSurfaces } from './server-fixture.js';

const FIELDS = [
  'is_treatment_response_actionable',
  'treatment_response_deciding_factor',
];

describe('L5c · TREATMENT-RESPONSE · third prediction grounded in a mechanism match (Treatments → confirmed CausalMechanism)', () => {
  before(startServer);
  after(stopServer);

  for (const p of PATIENTS) {
    const row = TREATMENT_LEVEL[p.prediction];
    for (const field of FIELDS) {
      test(`${p.key} · ${p.name} · ${field} = ${row[field]}`, async () => {
        await assertAppSurfaces(
          CONTRACT.treatmentLevel.endpoint(p),
          field,
          row[field],
          TREATMENT_WITNESS[field] || `derived treatment-response scalar ${field} from vw_individual_predictions`,
        );
      });
    }
  }
});
