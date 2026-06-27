// ============================================================================
//  contract.js — the APP-SURFACE contract the harness asserts against.
// ============================================================================
//  The harness is load-bearing on the APP, not on Postgres: it asks the running
//  server for each DAG node and checks the served value against the oracle.
//  This file maps each DAG level to the endpoint that must serve it. Every
//  endpoint here returns 501 today (see server/index.js) — that is why the
//  whole suite is RED until the loops wire these endpoints to read vw_*.
//
//  endpoint(p)  -> the API path for a patient/oracle row
//  surfaces     -> the level this endpoint is responsible for (doc only)
// ============================================================================

export const CONTRACT = {
  keystone: {
    surfaces: 'L7 IsClinicallyActionable',
    endpoint: (p) => `/api/predictions/${p.prediction}`,
    plannedLoop: 'Loop 1',
  },
  gates: {
    surfaces: 'L6 the four gates',
    endpoint: (p) => `/api/predictions/${p.prediction}`,
    plannedLoop: 'Loop 1',
  },
  predictionLevel: {
    surfaces: 'L5 prediction scalars (causal mass, predicted value, calibrated uncertainty, flags)',
    endpoint: (p) => `/api/predictions/${p.prediction}`,
    plannedLoop: 'Loop 1',
  },
  severityLevel: {
    surfaces: 'L5b severity prediction (max severity score, tier, severity-actionable gate chained to the onset mechanism gates)',
    endpoint: (p) => `/api/predictions/${p.prediction}`,
    plannedLoop: 'Loop 4',
  },
  treatmentLevel: {
    surfaces: 'L5c treatment-response prediction (effective therapy on a confirmed mechanism — the mechanism match)',
    endpoint: (p) => `/api/predictions/${p.prediction}`,
    plannedLoop: 'Loop 5',
  },
  individualLevel: {
    surfaces: 'L4 individual rollups',
    endpoint: (p) => `/api/individuals/${p.individual}`,
    plannedLoop: 'Loop 3',
  },
  mechanismLevel: {
    surfaces: 'L2/L3 mechanism aggregations + verdicts',
    endpoint: (p) => `/api/mechanisms/${p.mechanism}`,
    plannedLoop: 'Loop 3',
  },
  evidenceAtoms: {
    surfaces: 'L1 evidence atoms',
    endpoint: (p) => `/api/mechanisms/${p.mechanism}/evidence`,
    plannedLoop: 'Loop 3',
  },
  replicationAtoms: {
    surfaces: 'L1 replication / transport atoms',
    endpoint: (p) => `/api/mechanisms/${p.mechanism}/replications`,
    plannedLoop: 'Loop 3',
  },
  controlAtoms: {
    surfaces: 'L1 negative-control atoms',
    endpoint: (p) => `/api/mechanisms/${p.mechanism}/controls`,
    plannedLoop: 'Loop 3',
  },
  calibrationAtoms: {
    surfaces: 'L1 calibration atoms',
    endpoint: (p) => `/api/predictions/${p.prediction}/calibration`,
    plannedLoop: 'Loop 3',
  },
  variantLevel: {
    surfaces: 'L0/L1 variant candidacy',
    endpoint: (p) => `/api/variants/${p.variant}`,
    plannedLoop: 'Loop 3',
  },
  cohort: {
    surfaces: 'L7 keystone list (cohort screen)',
    endpoint: () => `/api/cohort`,
    plannedLoop: 'Loop 1',
  },
  // ---- NEW: case lifecycle + routing surfaces ----
  lifecycle: {
    surfaces: 'L8 the case walk through the diagnosis state machine',
    endpoint: (p) => `/api/predictions/${p.prediction}/lifecycle`,
    plannedLoop: 'Loop 2',
  },
  routingEntity: {
    surfaces: 'L9 per-entity RelativePath',
    endpoint: (e) => e.endpoint,
    plannedLoop: 'Loop 2',
  },
  routingTree: {
    surfaces: 'L9 role-based nav tree',
    endpoint: (role) => `/api/routing/tree?role=${role}`,
    plannedLoop: 'Loop 2',
  },
  // ---- v2: the DISEASE-STATE SIMULATOR (the layer the audit asked for) ----
  diseaseState: {
    surfaces: 'L11 derived disease state + activity (served on /api/individuals/:id)',
    endpoint: (e) => `/api/individuals/${e.individual}`,
    plannedLoop: 'v2',
  },
  progression: {
    surfaces: 'L11b the disease-progression walk (lupus-nephritis machine)',
    endpoint: (e) => `/api/individuals/${e.individual}/progression`,
    plannedLoop: 'v2',
  },
  treatmentLine: {
    surfaces: 'L5d treatment-line selection + the disagreement counter-example (served on /api/predictions/:id)',
    endpoint: (p) => `/api/predictions/${p.prediction}`,
    plannedLoop: 'v2',
  },
  // ---- Loop 10: the disease-state simulator's ADMIN WITNESS ----
  // The progression machine bound into the state-machine admin: each state shows
  // its live cohort occupancy (current_occupancy), DERIVED from the IsCurrent
  // occupancy chain — the simulator and its admin view read the same model.
  stateMachineOccupancy: {
    surfaces: 'L12 per-state cohort occupancy on the state-machine admin (derived IsCurrent counts)',
    endpoint: (machineId) => `/api/state-machines/${machineId}`,
    plannedLoop: 'Loop 10',
  },
  // ---- Loop 11: the emergent SEROLOGY-SIGNATURE CLUSTER (corpus-level discovery) ----
  // is_in_pre_nephritic_signature_cluster / signature_strength are DERIVED per
  // individual from the raw serology series — the rising-dsDNA/falling-complement
  // signature, rolled up. Surfaced on the cohort board (/api/cohort-individuals).
  signatureCluster: {
    surfaces: 'L13 emergent pre-nephritic serology-signature cluster (derived corpus-level membership)',
    endpoint: () => `/api/cohort-individuals`,
    plannedLoop: 'Loop 11',
  },
};
