// L5 — PREDICTION-LEVEL derived scalars. These are the lookups + PredictedValue
// + calibration aggregates that feed the gates. One test per field per patient.
//
// RED today: /api/predictions/:id (501). Loop 1 turns these green.

import { test, before, after, describe } from 'node:test';
import { PATIENTS, PREDICTION_LEVEL, PREDICTION_WITNESS } from '../oracle/dag-oracle.js';
import { CONTRACT } from './contract.js';
import { startServer, stopServer, assertAppSurfaces } from './server-fixture.js';

const FIELDS = [
  'individual_causal_mass',
  'individual_confirmed_node_count',
  'individual_cross_ancestry_node_count',
  'individual_has_cryptic_relatedness',
  'predicted_value',
  'count_bins',
  'count_well_calibrated_bins',
  'well_calibrated_fraction',
  'calibrated_uncertainty',
  'rests_on_confirmed_mechanism',
  'has_spurious_correlation_flag',
  'is_transportable_to_absent_ancestry',
  'patient_stratification_tier',
];

describe('L5 · PREDICTION scalars · causal mass, predicted value, calibration, flags', () => {
  before(startServer);
  after(stopServer);

  for (const p of PATIENTS) {
    const row = PREDICTION_LEVEL[p.prediction];
    for (const field of FIELDS) {
      test(`${p.key} · ${p.name} · ${field} = ${row[field]}`, async () => {
        await assertAppSurfaces(
          CONTRACT.predictionLevel.endpoint(p),
          field,
          row[field],
          PREDICTION_WITNESS[field] || `derived scalar ${field} from vw_individual_predictions`,
        );
      });
    }
  }
});
