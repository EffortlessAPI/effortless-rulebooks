// L5d — TREATMENT-LINE SELECTION + THE DISAGREEMENT COUNTER-EXAMPLE (v2).
//
// The v1 audit's SECOND worked example: a patient "would likely respond differently to
// mycophenolate vs belimumab vs anifrolumab." v2 DERIVES a recommended line + a single
// deciding reason from the confirmed-mechanism TargetPathway + the disease state — served
// on the same /api/predictions/:id panel (SELECT * exposes the new columns).
//
// The MARQUEE assertion is progression_vs_actionability_disagree on pred-d (Diego):
//   the disease-state simulator says he IS progressing (BiopsyIndicated) and the line is
//   MMF induction, YET the keystone is NOT clinically actionable (fails cryptic-relatedness).
//   That single TRUE flag is the proof that the disease-state simulator and the actionability
//   gate are INDEPENDENT layers — i.e. v2 is not v1 relabeled.

import { test, before, after, describe } from 'node:test';
import { PATIENTS, TREATMENT_LINE, TREATMENT_LINE_WITNESS } from '../oracle/dag-oracle.js';
import { CONTRACT } from './contract.js';
import { startServer, stopServer, assertAppSurfaces } from './server-fixture.js';

const FIELDS = ['recommended_treatment_line', 'treatment_line_deciding_factor', 'progression_vs_actionability_disagree'];

describe('L5d · Treatment-line selection · MMF/belimumab/anifrolumab derived from mechanism + disease state', () => {
  before(startServer);
  after(stopServer);

  for (const p of PATIENTS) {
    const row = TREATMENT_LINE[p.prediction];
    if (!row) continue;
    for (const field of FIELDS) {
      test(`${p.key} · ${p.name} · ${field} = ${row[field]}`, async () => {
        await assertAppSurfaces(CONTRACT.treatmentLine.endpoint(p), field, row[field], TREATMENT_LINE_WITNESS[field]);
      });
    }
  }

  // The explicit counter-example, called out as its own assertion so a reader sees it.
  test('COUNTER-EXAMPLE · Diego (D) · disease progressing (BiopsyIndicated) BUT prediction NOT actionable ⇒ disagree = TRUE', async () => {
    await assertAppSurfaces(
      CONTRACT.treatmentLine.endpoint(PATIENTS.find((p) => p.key === 'D')),
      'progression_vs_actionability_disagree',
      true,
      'The disease-state simulator and the actionability gate disagree: the disease IS progressing while the prediction is NOT actionable. Proves the two layers are independent and both real.',
    );
  });
});
