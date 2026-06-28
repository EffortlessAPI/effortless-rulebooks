// Thin runtime executor over the declarative schema.
//
// All derivation rules live in ../schema/scheduler.schema.mjs. This module
// just walks the computed-field DAG in topological order and runs each
// node's `compute` function with the right context.
//
// The previous version of this file inlined 353 LOC of derivation logic and
// hand-built parallel explain blocks. Now the rule of record is the schema
// file, and the explain payload here is *also* assembled from that same
// metadata — so it cannot drift from the formula it claims to document.

import { computed, computedOrder, computedByTable } from '../schema/scheduler.schema.mjs';

const ENTITY_KEY = {
  shifts: 'shift',
  events: 'event',
  volunteer_event: 'volunteer_event',
};

// Run every computed field for a single target table on one row.
// `ctx` carries the lookup pool: { events, shifts, assignments, volunteers, volunteer_skills, computedShifts }.
function runForTable(targetTable, row, ctx) {
  const derived = {};
  for (const key of computedOrder) {
    const [t, field] = key.split('.');
    if (t !== targetTable) continue;
    const node = computed[key];
    derived[field] = node.compute({ row, ctx, derived, computedShifts: ctx.computedShifts || [] });
  }
  return derived;
}

// Build the per-cell explain payload from schema metadata. The "inputs"
// list is derived directly from `depends`, so adding a new dependency in
// the schema automatically shows up here. References resolve to either a
// raw row.column value or an upstream derived value.
function buildExplain(targetTable, row, derived, ctx) {
  const out = {};
  const keys = computedByTable[targetTable] || [];
  for (const key of keys) {
    const node = computed[key];
    const field = key.split('.').slice(1).join('.');
    const inputs = (node.depends || []).map((dep) => {
      const [depT, depF] = dep.split('.');

      // Self-table dependency on another computed field → reuse derived.
      if (depT === targetTable && derived[depF] !== undefined) {
        return {
          label: dep,
          value: derived[depF],
          ref: `${ENTITY_KEY[targetTable] || targetTable}:${row.id}:${depF}`,
        };
      }
      // Self-table dependency on a raw column → row value.
      if (depT === targetTable && depF in row) {
        return { label: dep, value: row[depF] };
      }
      // Cross-table dep: surface as a structural pointer + count.
      const pool = ctx[depT];
      if (Array.isArray(pool)) {
        return { label: dep, value: `${pool.length} row(s)`, ref: depT };
      }
      return { label: dep, value: '—' };
    });

    out[field] = {
      kind: node.kind,
      formula: node.formula,
      excelFormula: node.excelFormula || null,
      sqlFormula: node.sqlFormula || null,
      depends: node.depends || [],
      result: derived[field],
      inputs,
    };
  }
  return out;
}

// -----------------------------------------------------------------------
// Public entry points used by server/index.js.
// -----------------------------------------------------------------------

/**
 * Compute every derived field on a list of shifts. Returns a parallel
 * array of `{ ...shift, computed: { ...derived, explain } }`.
 */
export function computeShifts(shifts, ctx) {
  return shifts.map((shift) => {
    const derived = runForTable('shifts', shift, ctx);
    const explain = buildExplain('shifts', shift, derived, ctx);
    // Spread derived at the top level so cross-table compute functions can
    // read `s.assigned_count` directly. Also keep them under `computed`
    // (with the explain bundle) for the API/UI to consume.
    return { ...shift, ...derived, computed: { ...derived, explain } };
  });
}

/** Compute every derived field on a list of events. Requires computedShifts in ctx. */
export function computeEvents(events, ctx) {
  return events.map((event) => {
    const derived = runForTable('events', event, ctx);
    const explain = buildExplain('events', event, derived, ctx);
    return {
      ...event,
      computed: { ...derived, explain },
    };
  });
}

/**
 * Compute per-volunteer-per-event load. Each input row is
 * { volunteer_id, event_id, id?: 'v{vid}-e{eid}' } so the explain refs are stable.
 */
export function computeVolunteerLoads(rows, ctx) {
  return rows.map((row) => {
    const synthRow = { id: `v${row.volunteer_id}-e${row.event_id}`, ...row };
    const derived = runForTable('volunteer_event', synthRow, ctx);
    const explain = buildExplain('volunteer_event', synthRow, derived, ctx);
    return {
      volunteer_id: row.volunteer_id,
      event_id: row.event_id,
      ...derived,
      explain,
    };
  });
}
