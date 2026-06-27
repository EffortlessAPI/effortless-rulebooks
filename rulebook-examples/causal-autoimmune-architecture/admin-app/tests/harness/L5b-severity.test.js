// L5b — SEVERITY prediction (Loop 4). The SECOND derived prediction: a severity
// tier + an IsSeverityActionable gate grounded in ClinicalPhenotypes.SeverityScore
// and CHAINED to the onset mechanism gates. One test per field per patient.
//
// Served by the same /api/predictions/:id panel (SELECT * already exposes the
// new columns), so no new endpoint — only new assertions. Red until the severity
// chain exists in the rulebook + DB (Loop 4 turns these green).
//
// The marquee assertion is pred-b: onset is NOT actionable (calibration gate),
// yet severity IS actionable — proving the two predictions are independent and
// that severity rides the confirmed mechanism, not the onset verdict.

import { test, before, after, describe } from 'node:test';
import { PATIENTS, SEVERITY_LEVEL, SEVERITY_WITNESS } from '../oracle/dag-oracle.js';
import { CONTRACT } from './contract.js';
import { startServer, stopServer, assertAppSurfaces } from './server-fixture.js';

const FIELDS = [
  'individual_max_severity_score',
  'severity_tier',
  'is_severity_actionable',
  'severity_deciding_factor',
];

describe('L5b · SEVERITY · second prediction grounded in ClinicalPhenotypes, chained to the onset gates', () => {
  before(startServer);
  after(stopServer);

  for (const p of PATIENTS) {
    const row = SEVERITY_LEVEL[p.prediction];
    for (const field of FIELDS) {
      test(`${p.key} · ${p.name} · ${field} = ${row[field]}`, async () => {
        await assertAppSurfaces(
          CONTRACT.severityLevel.endpoint(p),
          field,
          row[field],
          SEVERITY_WITNESS[field] || `derived severity scalar ${field} from vw_individual_predictions`,
        );
      });
    }
  }
});
