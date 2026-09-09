#!/usr/bin/env python3
"""Register the corpus test harness as first-class rulebook data (2026-09-09).

Before this migration the repo could run conformance for ONE project at a time
(scripts/run-conformance.py, the explorer's /conformance page) and build every
project in a separate, ungraded place (orchestration/build-status/summary.json).
Neither knew about the other, and "is this project even testable?" was inferred
from whether `testing/answer-keys/` happened to be non-empty.

This adds the three tables the corpus runner needs:

* `TestSuites`     — one DECLARED row per registered suite: the root formula-parser
                     pytest suite plus one conformance suite per governed project.
                     `IsRegistered` and `ExpectedSubstrateCount` are declarations;
                     `AnswerKeyCount` / `HasEffortlessJson` / `HasPostgresBootstrap`
                     are witnessed by scripts/scan-test-suites.py, exactly the way
                     ProjectSlotWitnesses witnesses the filesystem.
* `CorpusRuns`      — one row per fan-out across the corpus.
* `CorpusDomainRuns`— one row per (corpus run x project), carrying the per-phase
                     status (build / db / conformance) so a red project is red for
                     a NAMED reason instead of a grepped log line.

Green-ness is a formula in every direction: per-domain-run (`IsGreen`,
`IsFullyGreen`), per-corpus-run (`GreenPercent`, `OverallStatus`) and per-project
(`IsGreenInLatestCorpusRun`). Nothing recomputes it in app code.

Idempotent. Usage:
    python3 scripts/migrate-phase6-corpus-testing.py effortless-rulebook/effortless-rulebook.json
"""

from __future__ import annotations

import json
import sys
from collections import OrderedDict
from pathlib import Path


def raw(name, desc, datatype="string", nullable=True):
    return {"name": name, "datatype": datatype, "type": "raw", "nullable": nullable, "Description": desc}


def calc(name, desc, formula, datatype="string"):
    return {"name": name, "datatype": datatype, "type": "calculated", "nullable": True,
            "Description": desc, "formula": formula}


def agg(name, desc, formula, datatype="number"):
    return {"name": name, "datatype": datatype, "type": "aggregation", "nullable": True,
            "Description": desc, "formula": formula}


def lookup(name, desc, related, lookup_field, via, formula):
    return {"name": name, "datatype": "string", "type": "lookup", "nullable": True,
            "Description": desc, "RelatedTo": related, "isReversed": False,
            "prefersSingleRecordLink": True, "LookupField": lookup_field, "ViaField": via,
            "formula": formula}


def fk(name, desc, related, inverse, nullable=False):
    return {"name": name, "datatype": "string", "type": "relationship", "nullable": nullable,
            "Description": desc, "RelatedTo": related, "isReversed": False,
            "prefersSingleRecordLink": True, "InverseField": inverse}


def reverse(name, desc, related, inverse):
    return {"name": name, "datatype": "string", "type": "relationship", "nullable": True,
            "Description": desc, "RelatedTo": related, "isReversed": True, "InverseField": inverse}


def upsert(schema, spec, after=None):
    """Replace a field of the same name in place, else append (or insert after `after`)."""
    for i, f in enumerate(schema):
        if f["name"] == spec["name"]:
            schema[i] = spec
            return
    if after is not None:
        idx = next((i for i, f in enumerate(schema) if f["name"] == after), len(schema) - 1) + 1
        schema.insert(idx, spec)
    else:
        schema.append(spec)


# ---------------------------------------------------------------------------
# TestSuites
# ---------------------------------------------------------------------------
TEST_SUITES_SCHEMA = [
    raw("TestSuiteId", "PK: suite-<slug> for a project's conformance suite, suite-root-<name> for a root suite.", nullable=False),
    calc("Name", "Order 1. Display alias (calculated).", "={{TestSuiteId}}"),
    fk("Domain", "FK to the governed project this suite tests. Blank for repo-root suites that test the platform itself.",
       "RulebookDomains", "TestSuites", nullable=True),
    raw("SuiteKind", "DECLARED. conformance (cross-substrate harness over a project) | pytest (a python test module at the repo root).", nullable=False),
    raw("Runner", "DECLARED. The command that runs this suite, relative to the repo root.", nullable=False),
    raw("IsRegistered", "DECLARED. This suite is part of the corpus the runner is expected to execute. An unregistered suite is skipped BY DECLARATION, never by accident.", datatype="boolean", nullable=False),
    raw("ExpectedSubstrateCount", "DECLARED. How many execution substrates this suite should grade. 0 for pytest suites.", datatype="number", nullable=False),
    raw("Notes", "Why this suite is (or is not yet) registered.", nullable=True),
    raw("AnswerKeyCount", "WITNESSED by scan-test-suites.py: files in <project>/testing/answer-keys/.", datatype="number", nullable=True),
    raw("HasEffortlessJson", "WITNESSED: the project has an effortless.json and can therefore be built.", datatype="boolean", nullable=True),
    raw("HasPostgresBootstrap", "WITNESSED: postgres-bootstrap/reset-rulebook-db.sh exists, so the per-domain DB can be reset before grading.", datatype="boolean", nullable=True),
    raw("TestFileCount", "WITNESSED: test_*.py files in a pytest suite's directory. Zero means the suite is registered but has nothing to run — which is how a platform test suite silently disappearing becomes visible in the registry instead of only as a red run.", datatype="number", nullable=True),
    raw("LastScannedOn", "WITNESSED: ISO timestamp of the last scan-test-suites.py run that refreshed the witness fields.", nullable=True),
    lookup("DomainName", "Order 2. Display name of the tested project.", "RulebookDomains", "DomainName", "Domain",
           "=INDEX(RulebookDomains!{{DomainName}}, MATCH({{Domain}}, RulebookDomains!{{DomainId}}, 0))"),
    lookup("DomainKind", "Order 2. Declared kind (root | toy | example) of the tested project.", "RulebookDomains", "Kind", "Domain",
           "=INDEX(RulebookDomains!{{Kind}}, MATCH({{Domain}}, RulebookDomains!{{DomainId}}, 0))"),
    calc("HasAnswerKeys", "Order 3. The project has at least one generated answer key.", "={{AnswerKeyCount}} > 0", datatype="boolean"),
    calc("IsRunnable", "Order 4. Registered AND buildable. This is the real gate on whether the runner attempts a suite; "
         "postgres-bootstrap/ and testing/answer-keys/ are GENERATED by the run itself, so their absence never blocks one.",
         '=AND({{IsRegistered}}, OR(AND({{SuiteKind}} = "pytest", {{TestFileCount}} > 0), '
         'AND({{SuiteKind}} <> "pytest", {{HasEffortlessJson}})))', datatype="boolean"),
    calc("RunnableFlag", "Order 5. 1 when IsRunnable, for SUMIFS rollups.",
         '=IF(AND({{IsRegistered}}, OR(AND({{SuiteKind}} = "pytest", {{TestFileCount}} > 0), '
         'AND({{SuiteKind}} <> "pytest", {{HasEffortlessJson}}))), 1, 0)', datatype="number"),
    calc("HasBeenExercised", "Order 4. Evidence on disk that this suite has been run at least once: generated answer keys "
         "or a generated postgres bootstrap. FALSE means the next run is this suite's first, not that it is broken.",
         "=OR({{AnswerKeyCount}} > 0, {{HasPostgresBootstrap}})", datatype="boolean"),
    calc("IsGradable", "Order 4. Runnable AND already carries answer keys, so this run can produce a conformance SCORE "
         "rather than generating its first key set.",
         '=AND({{IsRegistered}}, OR({{SuiteKind}} = "pytest", AND({{HasEffortlessJson}}, {{AnswerKeyCount}} > 0)))', datatype="boolean"),
    calc("GradableFlag", "Order 5. 1 when IsGradable, for SUMIFS rollups.",
         '=IF(AND({{IsRegistered}}, OR({{SuiteKind}} = "pytest", AND({{HasEffortlessJson}}, {{AnswerKeyCount}} > 0))), 1, 0)', datatype="number"),
    calc("RegistrationState", "Order 6. Why this suite will or will not run: unregistered | no-effortless-json | "
         "never-exercised (runnable, but the first run generates its keys) | no-test-files | ready.",
         '=IF(NOT({{IsRegistered}}), "unregistered", '
         'IF({{SuiteKind}} = "pytest", IF({{TestFileCount}} = 0, "no-test-files", "ready"), '
         'IF(NOT({{HasEffortlessJson}}), "no-effortless-json", '
         'IF({{AnswerKeyCount}} = 0, "never-exercised", "ready"))))'),
    reverse("CorpusDomainRuns", "Reverse relationship: every corpus-run row that executed this suite.",
            "CorpusDomainRuns", "Suite"),
    agg("CorpusDomainRunCount", "Order 7. How many corpus runs have executed this suite.",
        "=COUNTIFS(CorpusDomainRuns!{{Suite}}, TestSuites!{{TestSuiteId}})", datatype="number"),
]

# ---------------------------------------------------------------------------
# CorpusRuns
# ---------------------------------------------------------------------------
CORPUS_RUNS_SCHEMA = [
    raw("CorpusRunId", "PK: corpus-<yyyymmdd>-<hhmmss>, matching the run directory under orchestration/corpus-runs/.", nullable=False),
    calc("Name", "Order 1. Display alias (calculated).", "={{CorpusRunId}}"),
    raw("Mode", "build-only (build + db reset, conformance skipped) | full (build + db reset + conformance).", nullable=False),
    raw("StartedOn", "ISO timestamp the fan-out began.", nullable=False),
    raw("FinishedOn", "ISO timestamp the fan-out finished. Blank while the run is still in flight.", nullable=True),
    raw("IsLatest", "TRUE on exactly one row: the most recently STARTED corpus run. The runner clears it from every other row.", datatype="boolean", nullable=False),
    raw("TargetCount", "How many suites the runner selected for this fan-out.", datatype="number", nullable=True),
    raw("StatusPath", "Repo-relative path to this run's status.json, the live artifact the explorer tails.", nullable=True),
    raw("Notes", "Free-text note about why this fan-out was run (baseline, post-fix re-run, ...).", nullable=True),
    reverse("DomainRuns", "Reverse relationship: one row per project attempted in this fan-out.", "CorpusDomainRuns", "CorpusRun"),
    agg("DomainRunCount", "Order 2. Projects attempted in this fan-out.",
        "=COUNTIFS(CorpusDomainRuns!{{CorpusRun}}, CorpusRuns!{{CorpusRunId}})", datatype="number"),
    agg("GreenDomainCount", "Order 3. Projects where no phase failed (a skipped phase does not fail a run).",
        "=SUMIFS(CorpusDomainRuns!{{GreenFlag}}, CorpusDomainRuns!{{CorpusRun}}, CorpusRuns!{{CorpusRunId}})", datatype="number"),
    agg("FullyGreenDomainCount", "Order 3. Projects that BUILT and were CONFORMANCE-GRADED green — the number that matters for a full run.",
        "=SUMIFS(CorpusDomainRuns!{{FullyGreenFlag}}, CorpusDomainRuns!{{CorpusRun}}, CorpusRuns!{{CorpusRunId}})", datatype="number"),
    agg("BuildFailureCount", "Order 4. Projects whose build phase failed.",
        "=SUMIFS(CorpusDomainRuns!{{BuildFailedFlag}}, CorpusDomainRuns!{{CorpusRun}}, CorpusRuns!{{CorpusRunId}})", datatype="number"),
    agg("DbFailureCount", "Order 4. Projects whose database-reset phase failed.",
        "=SUMIFS(CorpusDomainRuns!{{DbFailedFlag}}, CorpusDomainRuns!{{CorpusRun}}, CorpusRuns!{{CorpusRunId}})", datatype="number"),
    agg("ConformanceFailureCount", "Order 4. Projects whose conformance phase failed.",
        "=SUMIFS(CorpusDomainRuns!{{ConformanceFailedFlag}}, CorpusDomainRuns!{{CorpusRun}}, CorpusRuns!{{CorpusRunId}})", datatype="number"),
    agg("TotalDurationSeconds", "Order 5. Summed wall-clock of every project attempted.",
        "=SUMIFS(CorpusDomainRuns!{{DurationSeconds}}, CorpusDomainRuns!{{CorpusRun}}, CorpusRuns!{{CorpusRunId}})", datatype="number"),
    calc("RedDomainCount", "Order 6. Projects where at least one phase failed.",
         "={{DomainRunCount}} - {{GreenDomainCount}}", datatype="number"),
    calc("GreenPercent", "Order 7. Percentage of attempted projects with no failing phase.",
         "=IF({{DomainRunCount}} = 0, 0, ROUND(100 * {{GreenDomainCount}} / {{DomainRunCount}}, 1))", datatype="number"),
    calc("FullyGreenPercent", "Order 7. Percentage of attempted projects that built AND graded green.",
         "=IF({{DomainRunCount}} = 0, 0, ROUND(100 * {{FullyGreenDomainCount}} / {{DomainRunCount}}, 1))", datatype="number"),
    calc("IsComplete", "Order 8. The fan-out has finished.", '={{FinishedOn}} <> ""', datatype="boolean"),
    calc("IsCorpusGreen", "Order 8. Every attempted project came back green.",
         "=AND({{DomainRunCount}} > 0, {{RedDomainCount}} = 0)", datatype="boolean"),
    calc("OverallStatus", "Order 9. running | no-targets | green | red.",
         '=IF({{FinishedOn}} = "", "running", '
         'IF({{DomainRunCount}} = 0, "no-targets", '
         'IF({{RedDomainCount}} = 0, "green", "red")))'),
]

# ---------------------------------------------------------------------------
# CorpusDomainRuns
# ---------------------------------------------------------------------------
CORPUS_DOMAIN_RUNS_SCHEMA = [
    raw("CorpusDomainRunId", "PK: <CorpusRunId>:<project-slug>.", nullable=False),
    calc("Name", "Order 1. Display alias (calculated).", "={{CorpusDomainRunId}}"),
    fk("CorpusRun", "FK to the fan-out this attempt belongs to.", "CorpusRuns", "DomainRuns"),
    fk("Domain", "FK to the governed project attempted.", "RulebookDomains", "CorpusDomainRuns"),
    fk("Suite", "FK to the registered test suite that was executed.", "TestSuites", "CorpusDomainRuns", nullable=True),
    raw("BuildStatus", "pass | fail | skipped — outcome of `effortless build` in the project directory.", nullable=False),
    raw("DbStatus", "pass | fail | skipped — outcome of createdb + postgres-bootstrap/reset-rulebook-db.sh.", nullable=False),
    raw("ConformanceStatus", "pass | fail | skipped — outcome of the cross-substrate harness.", nullable=False),
    raw("ConformanceOutcome", "Why the conformance phase landed where it did: all-substrates-passed | substrate-mismatch (the harness ran fine, substrates disagreed with the answer keys) | harness-error (the harness itself failed) | tests-passed / tests-failed (pytest suites) | skipped. ConformanceStatus stays pass/fail/skipped so the flag formulas remain simple; this column carries the distinction.", nullable=True),
    raw("SubstratesTested", "Substrates the harness graded in this attempt.", datatype="number", nullable=True),
    raw("SubstratesPassed", "Substrates that scored 100 in this attempt.", datatype="number", nullable=True),
    raw("DurationSeconds", "Wall-clock seconds for this project's whole attempt.", datatype="number", nullable=True),
    raw("FirstError", "First error line captured from the failing phase. Blank when nothing failed.", nullable=True),
    raw("LogPath", "Repo-relative path to this attempt's full log.", nullable=True),
    fk("ConformanceRun", "FK to the ConformanceRuns row this attempt recorded, when the conformance phase ran.",
       "ConformanceRuns", "CorpusDomainRuns", nullable=True),
    lookup("DomainName", "Order 2. Display name of the attempted project.", "RulebookDomains", "DomainName", "Domain",
           "=INDEX(RulebookDomains!{{DomainName}}, MATCH({{Domain}}, RulebookDomains!{{DomainId}}, 0))"),
    lookup("DomainKind", "Order 2. Declared kind (root | toy | example) of the attempted project.", "RulebookDomains", "Kind", "Domain",
           "=INDEX(RulebookDomains!{{Kind}}, MATCH({{Domain}}, RulebookDomains!{{DomainId}}, 0))"),
    lookup("CorpusRunMode", "Order 2. Mode of the parent fan-out (build-only | full).", "CorpusRuns", "Mode", "CorpusRun",
           "=INDEX(CorpusRuns!{{Mode}}, MATCH({{CorpusRun}}, CorpusRuns!{{CorpusRunId}}, 0))"),
    lookup("CorpusRunIsLatest", "Order 2. Flattened one hop so per-project 'green right now' can be a SUMIFS on this row.",
           "CorpusRuns", "IsLatest", "CorpusRun",
           "=INDEX(CorpusRuns!{{IsLatest}}, MATCH({{CorpusRun}}, CorpusRuns!{{CorpusRunId}}, 0))"),
    calc("IsGreen", "Order 3. No phase failed. A skipped phase does not fail an attempt.",
         '=AND({{BuildStatus}} <> "fail", {{DbStatus}} <> "fail", {{ConformanceStatus}} <> "fail")', datatype="boolean"),
    calc("GreenFlag", "Order 4. 1 when IsGreen, for SUMIFS rollups.",
         '=IF(AND({{BuildStatus}} <> "fail", {{DbStatus}} <> "fail", {{ConformanceStatus}} <> "fail"), 1, 0)', datatype="number"),
    calc("IsFullyGreen", "Order 3. Nothing failed AND conformance actually ran green — not merely 'nothing failed "
         "because nothing ran'. A pytest suite has no build phase, so this asks that the build did not FAIL rather "
         "than that it passed; a build-only attempt is never fully green because nothing was graded.",
         '=AND({{BuildStatus}} <> "fail", {{DbStatus}} <> "fail", {{ConformanceStatus}} = "pass")', datatype="boolean"),
    calc("FullyGreenFlag", "Order 4. 1 when IsFullyGreen, for SUMIFS rollups.",
         '=IF(AND({{BuildStatus}} <> "fail", {{DbStatus}} <> "fail", {{ConformanceStatus}} = "pass"), 1, 0)', datatype="number"),
    calc("BuildFailedFlag", "Order 4. 1 when the build phase failed.", '=IF({{BuildStatus}} = "fail", 1, 0)', datatype="number"),
    calc("DbFailedFlag", "Order 4. 1 when the database-reset phase failed.", '=IF({{DbStatus}} = "fail", 1, 0)', datatype="number"),
    calc("ConformanceFailedFlag", "Order 4. 1 when the conformance phase failed.", '=IF({{ConformanceStatus}} = "fail", 1, 0)', datatype="number"),
    calc("FailingPhase", "Order 5. The first phase that failed, or blank when the attempt was green.",
         '=IF({{BuildStatus}} = "fail", "build", '
         'IF({{DbStatus}} = "fail", "db", '
         'IF({{ConformanceStatus}} = "fail", "conformance", "")))'),
    calc("LatestGreenFlag", "Order 6. 1 when this attempt is green AND belongs to the latest fan-out. Flattens the two-hop 'is this project green right now' into one column.",
         '=IF(AND({{CorpusRunIsLatest}}, {{BuildStatus}} <> "fail", {{DbStatus}} <> "fail", {{ConformanceStatus}} <> "fail"), 1, 0)', datatype="number"),
    calc("LatestFullyGreenFlag", "Order 6. 1 when this attempt graded green with nothing failing AND belongs to the latest fan-out.",
         '=IF(AND({{CorpusRunIsLatest}}, {{BuildStatus}} <> "fail", {{DbStatus}} <> "fail", {{ConformanceStatus}} = "pass"), 1, 0)', datatype="number"),
    calc("LatestAttemptFlag", "Order 6. 1 when this attempt belongs to the latest fan-out, green or not.",
         "=IF({{CorpusRunIsLatest}}, 1, 0)", datatype="number"),
]

# The reverse side of CorpusDomainRuns.ConformanceRun, which has to live on the
# existing ConformanceRuns table.
CONFORMANCE_RUN_ADDITIONS = [
    reverse("CorpusDomainRuns", "Reverse relationship: the corpus-run attempt that produced this conformance run, "
            "when it came from a fan-out rather than a single-project run.", "CorpusDomainRuns", "ConformanceRun"),
]

# Fields grafted onto RulebookDomains so a project carries its own corpus history.
DOMAIN_ADDITIONS = [
    reverse("TestSuites", "Reverse relationship: registered test suites that target this project.", "TestSuites", "Domain"),
    reverse("CorpusDomainRuns", "Reverse relationship: every corpus-run attempt against this project.", "CorpusDomainRuns", "Domain"),
    agg("TestSuiteCount", "Order 8. Registered test suites targeting this project.",
        "=COUNTIFS(TestSuites!{{Domain}}, RulebookDomains!{{DomainId}})", datatype="number"),
    agg("RunnableSuiteCount", "Order 8. Suites that are registered AND have their prerequisites on disk.",
        "=SUMIFS(TestSuites!{{RunnableFlag}}, TestSuites!{{Domain}}, RulebookDomains!{{DomainId}})", datatype="number"),
    agg("GradableSuiteCount", "Order 8. Suites that can produce a meaningful conformance score today.",
        "=SUMIFS(TestSuites!{{GradableFlag}}, TestSuites!{{Domain}}, RulebookDomains!{{DomainId}})", datatype="number"),
    agg("CorpusAttemptCount", "Order 9. Corpus-run attempts recorded against this project, all time.",
        "=COUNTIFS(CorpusDomainRuns!{{Domain}}, RulebookDomains!{{DomainId}})", datatype="number"),
    agg("LatestAttemptCount", "Order 9. Attempts against this project in the latest fan-out (0 when it was not selected).",
        "=SUMIFS(CorpusDomainRuns!{{LatestAttemptFlag}}, CorpusDomainRuns!{{Domain}}, RulebookDomains!{{DomainId}})", datatype="number"),
    agg("LatestGreenCount", "Order 9. Green attempts against this project in the latest fan-out.",
        "=SUMIFS(CorpusDomainRuns!{{LatestGreenFlag}}, CorpusDomainRuns!{{Domain}}, RulebookDomains!{{DomainId}})", datatype="number"),
    agg("LatestFullyGreenCount", "Order 9. Built-and-graded-green attempts against this project in the latest fan-out.",
        "=SUMIFS(CorpusDomainRuns!{{LatestFullyGreenFlag}}, CorpusDomainRuns!{{Domain}}, RulebookDomains!{{DomainId}})", datatype="number"),
    calc("IsGreenInLatestCorpusRun", "Order 10. This project came back green in the most recent fan-out.",
         "={{LatestGreenCount}} > 0", datatype="boolean"),
    calc("CorpusTestState", "Order 10. not-run | green | fully-green | red — this project's standing in the latest fan-out.",
         '=IF({{LatestAttemptCount}} = 0, "not-run", '
         'IF({{LatestFullyGreenCount}} > 0, "fully-green", '
         'IF({{LatestGreenCount}} > 0, "green", "red")))'),
]


def main() -> None:
    if len(sys.argv) != 2:
        raise SystemExit(__doc__)
    path = Path(sys.argv[1])
    rb = json.loads(path.read_text(encoding="utf-8"), object_pairs_hook=OrderedDict)

    if "RulebookDomains" not in rb:
        raise SystemExit(f"{path} has no RulebookDomains table — wrong rulebook?")
    if "ConformanceResults" not in rb:
        raise SystemExit(f"{path} has no ConformanceResults table — run the conformance migration first.")

    new_tables = OrderedDict([
        ("TestSuites", {
            "Description": "Registered test suites. One DECLARED row per suite the corpus runner is expected to execute: "
                           "the repo-root formula-parser pytest suite plus one cross-substrate conformance suite per governed "
                           "project. IsRegistered is a declaration, never inferred from the filesystem; the AnswerKeyCount / "
                           "HasEffortlessJson / HasPostgresBootstrap columns are witnessed by scripts/scan-test-suites.py so "
                           "RegistrationState can say WHY a registered suite is not yet runnable.",
            "schema": TEST_SUITES_SCHEMA,
            "data": [],
        }),
        ("CorpusRuns", {
            "Description": "One row per corpus-wide fan-out (scripts/run-corpus.py). Append-only history: how much of the "
                           "corpus was green on a given day is read straight off these rows.",
            "schema": CORPUS_RUNS_SCHEMA,
            "data": [],
        }),
        ("CorpusDomainRuns", {
            "Description": "One row per (corpus run x project). Carries the per-phase outcome — build, database reset, "
                           "conformance — so a red project is red for a NAMED phase instead of a grepped log line, and links "
                           "to the ConformanceRuns row it produced.",
            "schema": CORPUS_DOMAIN_RUNS_SCHEMA,
            "data": [],
        }),
    ])

    # Rebuild the top-level key order so the three tables sit immediately after
    # ConformanceResults, keeping the conformance/testing block contiguous.
    rebuilt = OrderedDict()
    for key, value in rb.items():
        if key in new_tables:
            continue  # re-emitted below, in the canonical position
        rebuilt[key] = value
        if key == "ConformanceResults":
            for name, spec in new_tables.items():
                if name in rb:
                    # Preserve witnessed/recorded rows across a re-run; refresh schema only.
                    existing = rb[name]
                    existing["Description"] = spec["Description"]
                    for f in spec["schema"]:
                        upsert(existing["schema"], f)
                    rebuilt[name] = existing
                else:
                    rebuilt[name] = spec

    for f in DOMAIN_ADDITIONS:
        upsert(rebuilt["RulebookDomains"]["schema"], f)
    for f in CONFORMANCE_RUN_ADDITIONS:
        upsert(rebuilt["ConformanceRuns"]["schema"], f)

    path.write_text(json.dumps(rebuilt, indent=2, ensure_ascii=False) + "\n", encoding="utf-8")
    print(f"TestSuites:       {len(rebuilt['TestSuites']['schema'])} fields, {len(rebuilt['TestSuites']['data'])} rows")
    print(f"CorpusRuns:       {len(rebuilt['CorpusRuns']['schema'])} fields, {len(rebuilt['CorpusRuns']['data'])} rows")
    print(f"CorpusDomainRuns: {len(rebuilt['CorpusDomainRuns']['schema'])} fields, {len(rebuilt['CorpusDomainRuns']['data'])} rows")
    print(f"RulebookDomains:  +{len(DOMAIN_ADDITIONS)} corpus fields ({len(rebuilt['RulebookDomains']['schema'])} total)")


if __name__ == "__main__":
    main()
