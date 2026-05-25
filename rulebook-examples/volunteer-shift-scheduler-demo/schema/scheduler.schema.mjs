// Declarative source of truth for the Volunteer Shift Scheduler.
//
// Everything else is generated or driven from this file:
//   - db/schema.sql        (DDL)        via schema/generate.mjs
//   - generated/types.d.ts (TS types)   via schema/generate.mjs
//   - generated/dag.json   (DAG meta)   via schema/generate.mjs
//   - server/compute.js    (runtime)    imports `computed` directly
//   - /api/schema          (exposed)    server passes `schemaForApi` to the UI
//   - Excel export         (workbook)   reads `computed` for formulas + rules
//
// Rule change = edit one entry here, run `./start.sh build`, done.

const TYPES = {
  SERIAL: 'serial',
  INTEGER: 'integer',
  TEXT: 'text',
  DATE: 'date',
  TIMESTAMP: 'timestamp',
  NUMERIC: (p, s) => `numeric(${p},${s})`,
};

/** @typedef {{ type: string, primaryKey?: boolean, notNull?: boolean, default?: any, check?: string, references?: { table: string, column: string, onDelete?: 'CASCADE'|'SET NULL' }, unique?: boolean }} ColumnSpec */
/** @typedef {{ description: string, columns: Record<string, ColumnSpec>, compoundPrimaryKey?: string[], compoundUnique?: string[][], indexes?: { name: string, columns: string[] }[] }} TableSpec */

export const tables = /** @type {Record<string, TableSpec>} */ ({
  events: {
    description: 'A volunteer event — festival, food bank shift block, conference, etc.',
    columns: {
      id:                         { type: TYPES.SERIAL, primaryKey: true },
      name:                       { type: TYPES.TEXT, notNull: true },
      location:                   { type: TYPES.TEXT, notNull: true, default: "''" },
      event_date:                 { type: TYPES.DATE, notNull: true },
      target_hours_per_volunteer: { type: TYPES.NUMERIC(5, 2), notNull: true, default: 4 },
    },
  },
  skills: {
    description: 'A discrete capability a volunteer can hold (e.g. First Aid).',
    columns: {
      id:   { type: TYPES.SERIAL, primaryKey: true },
      name: { type: TYPES.TEXT, notNull: true, unique: true },
    },
  },
  volunteers: {
    description: 'A person who can be assigned to shifts.',
    columns: {
      id:                { type: TYPES.SERIAL, primaryKey: true },
      name:              { type: TYPES.TEXT, notNull: true },
      email:             { type: TYPES.TEXT, notNull: true, unique: true },
      reliability_score: { type: TYPES.NUMERIC(3, 2), notNull: true, default: 0.85, check: 'reliability_score BETWEEN 0 AND 1' },
      max_hours:         { type: TYPES.NUMERIC(5, 2), notNull: true, default: 8 },
    },
  },
  volunteer_skills: {
    description: 'Many-to-many: which volunteers hold which skills.',
    columns: {
      volunteer_id: { type: TYPES.INTEGER, notNull: true, references: { table: 'volunteers', column: 'id', onDelete: 'CASCADE' } },
      skill_id:     { type: TYPES.INTEGER, notNull: true, references: { table: 'skills',     column: 'id', onDelete: 'CASCADE' } },
    },
    compoundPrimaryKey: ['volunteer_id', 'skill_id'],
  },
  shifts: {
    description: 'A time block within an event that needs `required_count` volunteers.',
    columns: {
      id:                { type: TYPES.SERIAL, primaryKey: true },
      event_id:          { type: TYPES.INTEGER, notNull: true, references: { table: 'events', column: 'id', onDelete: 'CASCADE' } },
      name:              { type: TYPES.TEXT, notNull: true },
      starts_at:         { type: TYPES.TIMESTAMP, notNull: true },
      ends_at:           { type: TYPES.TIMESTAMP, notNull: true },
      required_count:    { type: TYPES.INTEGER, notNull: true, default: 1, check: 'required_count >= 0' },
      required_skill_id: { type: TYPES.INTEGER, references: { table: 'skills', column: 'id', onDelete: 'SET NULL' } },
    },
    indexes: [{ name: 'ix_shifts_event', columns: ['event_id'] }],
  },
  assignments: {
    description: 'Pins a volunteer to a shift. (shift_id, volunteer_id) is unique.',
    columns: {
      id:           { type: TYPES.SERIAL, primaryKey: true },
      shift_id:     { type: TYPES.INTEGER, notNull: true, references: { table: 'shifts',     column: 'id', onDelete: 'CASCADE' } },
      volunteer_id: { type: TYPES.INTEGER, notNull: true, references: { table: 'volunteers', column: 'id', onDelete: 'CASCADE' } },
      created_at:   { type: TYPES.TIMESTAMP, notNull: true, default: 'NOW()' },
    },
    compoundUnique: [['shift_id', 'volunteer_id']],
    indexes: [
      { name: 'ix_assignments_shift',     columns: ['shift_id'] },
      { name: 'ix_assignments_volunteer', columns: ['volunteer_id'] },
    ],
  },
});

// ---------------------------------------------------------------------------
// Tunable constants. These are *the* knobs. Edit here, regenerate, done.
// ---------------------------------------------------------------------------
export const constants = {
  GRADE_WEIGHTS: {
    coverage_pct:    0.6,
    avg_reliability: 0.3,
    skill_match_avg: 0.1,
  },
  GRADE_BANDS: [
    { min: 0.93, grade: 'A' },
    { min: 0.85, grade: 'B' },
    { min: 0.75, grade: 'C' },
    { min: 0.65, grade: 'D' },
    { min: 0,    grade: 'F' },
  ],
  LOAD_BANDS: {
    under_factor: 0.75,
    over_factor:  1.25,
  },
};

const round = (n, d = 3) => {
  if (n === null || n === undefined || Number.isNaN(n)) return n;
  const m = 10 ** d;
  return Math.round(Number(n) * m) / m;
};
const hoursBetween = (a, b) => (new Date(b).getTime() - new Date(a).getTime()) / 3_600_000;

/**
 * A computed field. The key is `<table>.<field>`.
 *
 * Each entry has:
 *   - kind          'lookup' | 'aggregation' | 'calc'
 *   - returns       sql-ish type (informational)
 *   - description   one-liner shown in the DAG card + Excel "Rules" sheet
 *   - formula       human/Excel-style formula string (THE rule, in English)
 *   - excelFormula  (optional) actual Excel formula template, may use [@col] syntax
 *   - sqlFormula    (optional) SQL snippet that can compute this from base tables
 *   - depends       array of refs this field depends on, where each ref is
 *                   either `<table>.<column>` (raw) or `<table>.<computed>`.
 *   - compute({ row, ctx, derived })  the *implementation*. derived holds this
 *                   row's other computed values (computed in dependency order).
 *
 * The formula string is the rule of record. The compute function is the
 * executable interpretation. Putting them side-by-side makes drift visible.
 */
export const computed = {
  // ------------------------ SHIFTS ------------------------
  'shifts.assigned_count': {
    kind: 'aggregation',
    returns: 'integer',
    description: 'Number of volunteers assigned to this shift.',
    formula: 'COUNT(assignments WHERE shift_id = this.id)',
    excelFormula: '=COUNTIF(assignments[shift_id], [@id])',
    sqlFormula: '(SELECT COUNT(*)::int FROM assignments a WHERE a.shift_id = s.id)',
    depends: ['assignments.shift_id'],
    compute: ({ row, ctx }) =>
      ctx.assignments.filter((a) => a.shift_id === row.id).length,
  },

  'shifts.duration_hours': {
    kind: 'calc',
    returns: 'numeric',
    description: 'Length of the shift in hours.',
    formula: '(ends_at − starts_at) / 1 hour',
    excelFormula: '=(([@ends_at]-[@starts_at])*24)',
    sqlFormula: 'EXTRACT(EPOCH FROM (s.ends_at - s.starts_at))/3600.0',
    depends: ['shifts.starts_at', 'shifts.ends_at'],
    compute: ({ row }) => round(hoursBetween(row.starts_at, row.ends_at), 2),
  },

  'shifts.coverage_delta': {
    kind: 'calc',
    returns: 'integer',
    description: 'Signed gap between assigned and required volunteers.',
    formula: 'assigned_count − required_count',
    excelFormula: '=[@assigned_count]-[@required_count]',
    depends: ['shifts.assigned_count', 'shifts.required_count'],
    compute: ({ row, derived }) => derived.assigned_count - row.required_count,
  },

  'shifts.coverage_status': {
    kind: 'calc',
    returns: 'text',
    description: 'under / full / over relative to required_count.',
    formula: "IF(assigned < required, 'under', IF(assigned > required, 'over', 'full'))",
    excelFormula: '=IF([@assigned_count]<[@required_count],"under",IF([@assigned_count]>[@required_count],"over","full"))',
    depends: ['shifts.assigned_count', 'shifts.required_count'],
    compute: ({ row, derived }) => {
      if (derived.assigned_count < row.required_count) return 'under';
      if (derived.assigned_count > row.required_count) return 'over';
      return 'full';
    },
  },

  'shifts.weighted_reliability': {
    kind: 'aggregation',
    returns: 'numeric',
    description: 'Mean reliability_score of the volunteers assigned to this shift (0 if none).',
    formula: 'AVG(assigned_volunteers.reliability_score)',
    excelFormula: '=IFERROR(AVERAGEIFS(volunteers[reliability_score], volunteers[id], assignments[volunteer_id], assignments[shift_id], [@id]), 0)',
    depends: ['assignments.shift_id', 'assignments.volunteer_id', 'volunteers.reliability_score'],
    compute: ({ row, ctx }) => {
      const vols = ctx.assignments
        .filter((a) => a.shift_id === row.id)
        .map((a) => ctx.volunteers.find((v) => v.id === a.volunteer_id))
        .filter(Boolean);
      if (vols.length === 0) return 0;
      const sum = vols.reduce((s, v) => s + Number(v.reliability_score), 0);
      return round(sum / vols.length, 3);
    },
  },

  'shifts.skill_match_pct': {
    kind: 'aggregation',
    returns: 'numeric | null',
    description: 'Fraction of assigned volunteers who hold the required skill. Null if no required skill.',
    formula: 'IF(required_skill IS NULL, NULL, COUNT(assigned WHERE has(required_skill)) / COUNT(assigned))',
    excelFormula: '=IF([@required_skill_id]="","",COUNTIFS(volunteer_skills[skill_id], [@required_skill_id], volunteer_skills[volunteer_id], assignments[volunteer_id], assignments[shift_id], [@id]) / [@assigned_count])',
    depends: ['shifts.required_skill_id', 'assignments.shift_id', 'volunteer_skills.skill_id'],
    compute: ({ row, ctx, derived }) => {
      if (row.required_skill_id == null) return null;
      if (derived.assigned_count === 0) return null;
      const assigned = ctx.assignments.filter((a) => a.shift_id === row.id);
      const matched = assigned.filter((a) =>
        ctx.volunteer_skills.some(
          (vs) => vs.volunteer_id === a.volunteer_id && vs.skill_id === row.required_skill_id,
        ),
      ).length;
      return round(matched / assigned.length, 3);
    },
  },

  // ------------------------ EVENTS ------------------------
  'events.shift_count': {
    kind: 'aggregation',
    returns: 'integer',
    description: 'Number of shifts in this event.',
    formula: 'COUNT(shifts WHERE event_id = this.id)',
    excelFormula: '=COUNTIF(shifts[event_id], [@id])',
    sqlFormula: '(SELECT COUNT(*)::int FROM shifts s WHERE s.event_id = e.id)',
    depends: ['shifts.event_id'],
    compute: ({ row, ctx }) => ctx.shifts.filter((s) => s.event_id === row.id).length,
  },

  'events.total_required': {
    kind: 'aggregation',
    returns: 'integer',
    description: 'Sum of required_count across the event\'s shifts.',
    formula: 'SUM(shifts.required_count WHERE event_id = this.id)',
    excelFormula: '=SUMIF(shifts[event_id], [@id], shifts[required_count])',
    sqlFormula: '(SELECT COALESCE(SUM(s.required_count),0)::int FROM shifts s WHERE s.event_id = e.id)',
    depends: ['shifts.event_id', 'shifts.required_count'],
    compute: ({ row, ctx }) =>
      ctx.shifts.filter((s) => s.event_id === row.id).reduce((sum, s) => sum + s.required_count, 0),
  },

  'events.total_assigned': {
    kind: 'aggregation',
    returns: 'integer',
    description: 'Sum of assigned_count across the event\'s shifts.',
    formula: 'SUM(shifts.assigned_count WHERE event_id = this.id)',
    depends: ['shifts.event_id', 'shifts.assigned_count'],
    compute: ({ row, ctx, computedShifts }) =>
      computedShifts.filter((s) => s.event_id === row.id).reduce((sum, s) => sum + s.assigned_count, 0),
  },

  'events.coverage_pct': {
    kind: 'calc',
    returns: 'numeric',
    description: 'total_assigned / total_required (1.0 when nothing is required).',
    formula: 'IF(total_required = 0, 1, total_assigned / total_required)',
    excelFormula: '=IF([@total_required]=0,1,[@total_assigned]/[@total_required])',
    depends: ['events.total_required', 'events.total_assigned'],
    compute: ({ derived }) =>
      derived.total_required === 0 ? 1 : round(derived.total_assigned / derived.total_required, 3),
  },

  'events.understaffed_shift_count': {
    kind: 'aggregation',
    returns: 'integer',
    description: 'Number of shifts in this event with coverage_status = under.',
    formula: "COUNT(shifts WHERE event_id = this.id AND coverage_status = 'under')",
    depends: ['shifts.event_id', 'shifts.coverage_status'],
    compute: ({ row, computedShifts }) =>
      computedShifts.filter((s) => s.event_id === row.id && s.coverage_status === 'under').length,
  },

  'events.overstaffed_shift_count': {
    kind: 'aggregation',
    returns: 'integer',
    description: 'Number of shifts in this event with coverage_status = over.',
    formula: "COUNT(shifts WHERE event_id = this.id AND coverage_status = 'over')",
    depends: ['shifts.event_id', 'shifts.coverage_status'],
    compute: ({ row, computedShifts }) =>
      computedShifts.filter((s) => s.event_id === row.id && s.coverage_status === 'over').length,
  },

  'events.fully_staffed_shift_count': {
    kind: 'aggregation',
    returns: 'integer',
    description: 'Number of shifts in this event with coverage_status = full.',
    formula: "COUNT(shifts WHERE event_id = this.id AND coverage_status = 'full')",
    depends: ['shifts.event_id', 'shifts.coverage_status'],
    compute: ({ row, computedShifts }) =>
      computedShifts.filter((s) => s.event_id === row.id && s.coverage_status === 'full').length,
  },

  'events.volunteer_count': {
    kind: 'aggregation',
    returns: 'integer',
    description: 'Number of distinct volunteers assigned to any shift in this event.',
    formula: 'COUNT(DISTINCT assignments.volunteer_id WHERE shift.event_id = this.id)',
    depends: ['shifts.event_id', 'assignments.shift_id', 'assignments.volunteer_id'],
    compute: ({ row, ctx }) => {
      const shiftIds = new Set(ctx.shifts.filter((s) => s.event_id === row.id).map((s) => s.id));
      const vIds = new Set(
        ctx.assignments.filter((a) => shiftIds.has(a.shift_id)).map((a) => a.volunteer_id),
      );
      return vIds.size;
    },
  },

  'events.avg_reliability': {
    kind: 'aggregation',
    returns: 'numeric',
    description: 'Mean reliability_score over distinct volunteers assigned to any shift in this event.',
    formula: 'AVG(reliability_score) over distinct assigned volunteers',
    depends: ['shifts.event_id', 'assignments.shift_id', 'assignments.volunteer_id', 'volunteers.reliability_score'],
    compute: ({ row, ctx }) => {
      const shiftIds = new Set(ctx.shifts.filter((s) => s.event_id === row.id).map((s) => s.id));
      const vIds = [...new Set(ctx.assignments.filter((a) => shiftIds.has(a.shift_id)).map((a) => a.volunteer_id))];
      if (vIds.length === 0) return 0;
      const sum = vIds.reduce((s, vid) => {
        const v = ctx.volunteers.find((x) => x.id === vid);
        return s + (v ? Number(v.reliability_score) : 0);
      }, 0);
      return round(sum / vIds.length, 3);
    },
  },

  'events.skill_match_avg': {
    kind: 'aggregation',
    returns: 'numeric',
    description: 'Mean of shift.skill_match_pct over shifts that declare a required_skill_id (1.0 when no skills are required anywhere).',
    formula: 'AVG(shifts.skill_match_pct WHERE skill_match_pct IS NOT NULL)',
    depends: ['shifts.event_id', 'shifts.skill_match_pct'],
    compute: ({ row, computedShifts }) => {
      const vals = computedShifts
        .filter((s) => s.event_id === row.id && s.skill_match_pct !== null && s.skill_match_pct !== undefined)
        .map((s) => s.skill_match_pct);
      if (vals.length === 0) return 1;
      return round(vals.reduce((a, b) => a + b, 0) / vals.length, 3);
    },
  },

  // The composite. This used to be a 30-line block buried in compute.js.
  // It's now declarative: weights live in `constants.GRADE_WEIGHTS`,
  // bands live in `constants.GRADE_BANDS`. Edit those to change the formula.
  'events.score': {
    kind: 'calc',
    returns: 'numeric',
    description: 'Weighted composite score in [0..1] — the input to the letter grade.',
    formula:
      'GRADE_WEIGHTS.coverage_pct · min(1, coverage_pct) + GRADE_WEIGHTS.avg_reliability · avg_reliability + GRADE_WEIGHTS.skill_match_avg · skill_match_avg',
    excelFormula: '=0.6*MIN(1,[@coverage_pct])+0.3*[@avg_reliability]+0.1*[@skill_match_avg]',
    depends: ['events.coverage_pct', 'events.avg_reliability', 'events.skill_match_avg'],
    constants: ['GRADE_WEIGHTS'],
    compute: ({ derived }) => {
      const w = constants.GRADE_WEIGHTS;
      const score =
        w.coverage_pct    * Math.min(1, derived.coverage_pct) +
        w.avg_reliability * derived.avg_reliability +
        w.skill_match_avg * derived.skill_match_avg;
      return round(score, 3);
    },
  },

  'events.grade': {
    kind: 'calc',
    returns: 'text',
    description: 'A–F letter grade derived from score via GRADE_BANDS.',
    formula: 'first(band ∈ GRADE_BANDS WHERE score ≥ band.min).grade',
    excelFormula: '=IF([@score]>=0.93,"A",IF([@score]>=0.85,"B",IF([@score]>=0.75,"C",IF([@score]>=0.65,"D","F"))))',
    depends: ['events.score'],
    constants: ['GRADE_BANDS'],
    compute: ({ derived }) => {
      const band = constants.GRADE_BANDS.find((b) => derived.score >= b.min);
      return band ? band.grade : 'F';
    },
  },

  // -------------------- VOLUNTEER-LOAD (per event) --------------------
  'volunteer_event.assigned_hours': {
    kind: 'aggregation',
    returns: 'numeric',
    description: 'Sum of duration_hours over shifts in this event that this volunteer is assigned to.',
    formula: 'SUM(shift.duration_hours WHERE shift.event_id = event.id AND assigned(volunteer, shift))',
    depends: ['shifts.event_id', 'shifts.duration_hours', 'assignments.shift_id', 'assignments.volunteer_id'],
    compute: ({ row, ctx, computedShifts }) => {
      const { volunteer_id, event_id } = row;
      const shiftIds = new Set(
        ctx.assignments
          .filter((a) => a.volunteer_id === volunteer_id)
          .map((a) => a.shift_id),
      );
      return round(
        computedShifts
          .filter((s) => s.event_id === event_id && shiftIds.has(s.id))
          .reduce((sum, s) => sum + s.duration_hours, 0),
        2,
      );
    },
  },

  'volunteer_event.shift_count': {
    kind: 'aggregation',
    returns: 'integer',
    description: 'Number of shifts in this event the volunteer is on.',
    formula: 'COUNT(shifts WHERE event_id = event.id AND assigned(volunteer, shift))',
    depends: ['shifts.event_id', 'assignments.shift_id', 'assignments.volunteer_id'],
    compute: ({ row, ctx }) => {
      const shiftIds = new Set(ctx.shifts.filter((s) => s.event_id === row.event_id).map((s) => s.id));
      return ctx.assignments.filter(
        (a) => a.volunteer_id === row.volunteer_id && shiftIds.has(a.shift_id),
      ).length;
    },
  },

  'volunteer_event.load_status': {
    kind: 'calc',
    returns: 'text',
    description: 'unassigned / under / ok / over, derived from assigned_hours vs event.target and volunteer.max_hours.',
    formula:
      "IF(assigned_hours = 0, 'unassigned', " +
      "IF(assigned_hours > volunteer.max_hours OR assigned_hours > LOAD_BANDS.over_factor × event.target_hours_per_volunteer, 'over', " +
      "IF(assigned_hours < LOAD_BANDS.under_factor × event.target_hours_per_volunteer, 'under', 'ok')))",
    depends: [
      'volunteer_event.assigned_hours',
      'events.target_hours_per_volunteer',
      'volunteers.max_hours',
    ],
    constants: ['LOAD_BANDS'],
    compute: ({ row, ctx, derived }) => {
      const event = ctx.events.find((e) => e.id === row.event_id);
      const vol = ctx.volunteers.find((v) => v.id === row.volunteer_id);
      if (!event || !vol) return 'unassigned';
      const hours = derived.assigned_hours;
      const target = Number(event.target_hours_per_volunteer);
      const max = Number(vol.max_hours);
      const { under_factor, over_factor } = constants.LOAD_BANDS;
      if (hours === 0) return 'unassigned';
      if (hours > max || hours > over_factor * target) return 'over';
      if (hours < under_factor * target) return 'under';
      return 'ok';
    },
  },
};

// Computed fields, grouped by their target table.
export const computedByTable = (() => {
  /** @type {Record<string, string[]>} */
  const out = {};
  for (const key of Object.keys(computed)) {
    const [table] = key.split('.');
    (out[table] ||= []).push(key);
  }
  return out;
})();

// Topological order of computed fields (respecting `depends`). Computed once.
export const computedOrder = (() => {
  const visited = new Set();
  const order = [];
  const visit = (key) => {
    if (visited.has(key)) return;
    visited.add(key);
    const node = computed[key];
    if (!node) return; // raw column reference
    for (const dep of node.depends || []) {
      if (computed[dep]) visit(dep);
    }
    order.push(key);
  };
  for (const k of Object.keys(computed)) visit(k);
  return order;
})();

// Trimmed view of the schema that's safe to ship to the browser.
// (Strips JS compute functions; the UI only needs metadata.)
export const schemaForApi = {
  tables: Object.fromEntries(
    Object.entries(tables).map(([name, t]) => [
      name,
      { description: t.description, columns: t.columns },
    ]),
  ),
  computed: Object.fromEntries(
    Object.entries(computed).map(([key, f]) => [
      key,
      {
        kind: f.kind,
        returns: f.returns,
        description: f.description,
        formula: f.formula,
        excelFormula: f.excelFormula || null,
        sqlFormula: f.sqlFormula || null,
        depends: f.depends || [],
        constants: f.constants || [],
      },
    ]),
  ),
  constants,
};
