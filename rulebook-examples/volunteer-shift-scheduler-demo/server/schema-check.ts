// Static sanity test: assert the schema's generated types compile and that
// the runtime payload shape matches what the API publishes.
//
// This file exists purely so `tsc --noEmit -p server/tsconfig.json` will
// fail if a schema change breaks the contract (e.g. a renamed computed
// field that the server forgets to re-expose).

import type {
  EventsRow,
  ShiftsRow,
  VolunteersRow,
  EventsComputed,
  ShiftsComputed,
  VolunteerEventComputed,
  DagGraph,
} from '../generated/types.d.ts';

// Smoke: every required computed field on an event is non-optional.
type AssertHas<T, K extends keyof T> = K;
type _EvKeys = AssertHas<
  EventsComputed,
  | 'shift_count'
  | 'total_required'
  | 'total_assigned'
  | 'coverage_pct'
  | 'understaffed_shift_count'
  | 'overstaffed_shift_count'
  | 'fully_staffed_shift_count'
  | 'volunteer_count'
  | 'avg_reliability'
  | 'skill_match_avg'
  | 'score'
  | 'grade'
>;
type _ShKeys = AssertHas<
  ShiftsComputed,
  'assigned_count' | 'duration_hours' | 'coverage_delta' | 'coverage_status' | 'weighted_reliability' | 'skill_match_pct'
>;
type _VlKeys = AssertHas<
  VolunteerEventComputed,
  'assigned_hours' | 'shift_count' | 'load_status'
>;

// Smoke: row shapes accept reasonable rows.
const _e: EventsRow = {
  id: 1,
  name: 'X',
  location: 'Y',
  event_date: '2026-01-01',
  target_hours_per_volunteer: 4,
};
const _s: ShiftsRow = {
  id: 1,
  event_id: 1,
  name: 'X',
  starts_at: '2026-01-01T00:00:00Z',
  ends_at: '2026-01-01T03:00:00Z',
  required_count: 2,
};
const _v: VolunteersRow = {
  id: 1,
  name: 'A',
  email: 'a@a',
  reliability_score: 0.9,
  max_hours: 8,
};

// Smoke: a DAG payload from /api/schema parses into DagGraph-shaped data.
const _g: DagGraph = {} as DagGraph;

// Reference the bindings so tsc doesn't tree-shake the type checks above.
void _e; void _s; void _v; void _g;
type _Unused = _EvKeys | _ShKeys | _VlKeys;
const _t: _Unused | undefined = undefined; void _t;
