// L13 — THE EMERGENT SEROLOGY-SIGNATURE CLUSTER (Loop 11). The doctor's deepest
// point made falsifiable at corpus scale: a single chart never discovers a
// mechanism; a pattern across many patients does. The rising-anti-dsDNA /
// falling-complement serology trajectory that precedes nephritis surfaces as a
// DERIVED corpus-level cluster on the cohort board — emergent from the
// population's raw serology series, not a label anyone assigned.
//
// Served on /api/cohort-individuals as is_in_pre_nephritic_signature_cluster +
// signature_strength (rolled up from SerologyObservations.IsPreNephriticSignaturePanel).
//
// The test asserts per-patient membership AND the SEPARATION INVARIANT that is the
// actual finding: every cluster member is active/progressing, every non-member is
// quiescent presymptomatic. If a future data edit broke that separation, this test
// goes red — which is the point.

import { test, before, after, describe } from 'node:test';
import assert from 'node:assert/strict';
import { PROGRESSION_INDIVIDUALS, SIGNATURE_CLUSTER, DISEASE_STATE } from '../oracle/dag-oracle.js';
import { CONTRACT } from './contract.js';
import { startServer, stopServer, getJson } from './server-fixture.js';

const QUIESCENT_STATE = 'PresymptomaticAutoimmunity';
const ACTIVE_STATES = new Set(['SerologicActive', 'EarlyNephritis', 'RenalFlareRisk', 'BiopsyIndicated']);

describe('L13 · Emergent serology-signature cluster · derived corpus-level discovery', () => {
  before(startServer);
  after(stopServer);

  // Per-patient: the app surfaces the derived membership + strength matching the oracle.
  for (const e of PROGRESSION_INDIVIDUALS) {
    const o = SIGNATURE_CLUSTER[e.individual];
    if (!o) continue;
    test(`${e.key} · ${e.name} · in_cluster=${o.is_in_pre_nephritic_signature_cluster} · strength=${o.signature_strength}`, async () => {
      const { status, body } = await getJson(CONTRACT.signatureCluster.endpoint());
      assert.equal(status, 200, `cohort-individuals not surfaced (HTTP ${status})`);
      const row = body.find((r) => r.individual_id === e.individual);
      assert.ok(row, `cohort board is missing ${e.individual}`);
      assert.equal(
        row.is_in_pre_nephritic_signature_cluster, o.is_in_pre_nephritic_signature_cluster,
        `${e.name}: expected cluster membership ${o.is_in_pre_nephritic_signature_cluster}, got ${row.is_in_pre_nephritic_signature_cluster}`);
      assert.equal(
        Number(row.signature_strength), o.signature_strength,
        `${e.name}: expected signature_strength ${o.signature_strength}, got ${row.signature_strength}`);
    });
  }

  // THE FINDING — the separation invariant, asserted as a set property over the whole cohort.
  test('separation invariant · every cluster member is active/progressing; every non-member is quiescent', async () => {
    const { body } = await getJson(CONTRACT.signatureCluster.endpoint());
    const violations = [];
    for (const r of body) {
      const inCluster = r.is_in_pre_nephritic_signature_cluster;
      const state = r.nephritis_progression_state_key;
      if (inCluster && !ACTIVE_STATES.has(state)) violations.push(`${r.family_name} in cluster but state=${state} (not active)`);
      if (!inCluster && state !== QUIESCENT_STATE) violations.push(`${r.family_name} NOT in cluster but state=${state} (not quiescent)`);
    }
    assert.equal(violations.length, 0,
      `the emergent signature must perfectly separate active/progressing from quiescent. Violations:\n  ${violations.join('\n  ')}`);
  });

  // And the cluster is the EXPECTED size (not empty, not everyone) — a real discovery.
  test('cluster size matches the oracle and is a strict subset of the cohort', async () => {
    const { body } = await getJson(CONTRACT.signatureCluster.endpoint());
    const actualMembers = body.filter((r) => r.is_in_pre_nephritic_signature_cluster).map((r) => r.individual_id).sort();
    const expectedMembers = Object.entries(SIGNATURE_CLUSTER)
      .filter(([, v]) => v.is_in_pre_nephritic_signature_cluster).map(([k]) => k).sort();
    assert.deepEqual(actualMembers, expectedMembers,
      `cluster membership must be ${JSON.stringify(expectedMembers)}, got ${JSON.stringify(actualMembers)}`);
    assert.ok(actualMembers.length > 0 && actualMembers.length < Object.keys(DISEASE_STATE).length,
      'a real discovery: the cluster is non-empty and not the whole cohort');
  });
});
