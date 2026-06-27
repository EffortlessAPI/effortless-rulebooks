-- All study data (synthetic and real) now lives in the rulebook JSON SSoT.
-- It is generated into 05-insert-data.sql by the transpiler on every build.

-- ----------------------------------------------------------------------------
-- Evaluate the invariant checks defined in the InvariantChecks rulebook table.
-- run_invariant_checks() reads SqlFilter and SqlAssertion from invariant_checks
-- and writes pass_count / fail_count back. The entire test harness is driven
-- from the rulebook — no assertion SQL lives outside the InvariantChecks table.
-- ----------------------------------------------------------------------------
SELECT * FROM run_invariant_checks();
