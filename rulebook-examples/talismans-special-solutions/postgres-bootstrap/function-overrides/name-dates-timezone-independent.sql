-- ============================================================================
-- Timezone-independent date rendering in calculated `name` labels.
-- ============================================================================
-- WHY: the rulebook-to-postgres transpiler casts `date` columns to
-- `::timestamptz` when it concatenates them into a string (e.g. the
-- RoleAssignments / ChangeLog `name` labels). A `date` has no time and no
-- zone, so that cast invents one — it renders the value as
-- `2026-01-10 00:00:00-06`, where the `-06`/`-05` offset is the SERVER's
-- local timezone (and flips across the DST boundary). That makes the value
-- NON-PORTABLE: the conformance answer key is frozen with one machine's
-- offset, so the Postgres engine would FAIL the sweep for anyone running in a
-- different timezone — a failure caused purely by physical location, not by a
-- real computation difference. It also makes the label impossible for the
-- OWL/SHACL substrate to reproduce (no DST-aware timestamptz in SPARQL), so
-- the field showed up as a conformance gap.
--
-- FIX: render the bare `date` (ISO `YYYY-MM-DD`). It is identical on every
-- machine and matches what every other substrate (OWL, the Python formula
-- engine) produces, so the field becomes fully conformant and portable.
--
-- This lives in function-overrides/ (applied last by init-db.sh, never touched
-- by the transpiler) so it persists across rebuilds. CREATE OR REPLACE means
-- the vw_* views pick it up at query time with no view rebuild.
-- ============================================================================

CREATE OR REPLACE FUNCTION calc_role_assignments_name(p_role_assignment_id TEXT)
RETURNS TEXT
LANGUAGE sql
STABLE
AS $function$
  SELECT (CONCAT(
    (SELECT NULLIF(role, '') FROM role_assignments WHERE role_assignment_id = p_role_assignment_id),
    ' [',
    (SELECT valid_from FROM role_assignments WHERE role_assignment_id = p_role_assignment_id),
    ' -> ',
    CASE
      WHEN ((SELECT valid_to FROM role_assignments WHERE role_assignment_id = p_role_assignment_id) IS NULL
            OR (SELECT valid_to FROM role_assignments WHERE role_assignment_id = p_role_assignment_id)::text = '')
      THEN 'open'
      ELSE (SELECT valid_to FROM role_assignments WHERE role_assignment_id = p_role_assignment_id)::text
    END,
    ']'))::text;
$function$;

CREATE OR REPLACE FUNCTION calc_change_log_name(p_change_log_id TEXT)
RETURNS TEXT
LANGUAGE sql
STABLE
AS $function$
  SELECT (CONCAT(
    (SELECT NULLIF(version, '') FROM change_log WHERE change_log_id = p_change_log_id),
    ' (',
    (SELECT change_date FROM change_log WHERE change_log_id = p_change_log_id),
    ')'))::text;
$function$;
