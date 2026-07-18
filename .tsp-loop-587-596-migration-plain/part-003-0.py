sureCriterion": spec["criterion"], "AfterState": None, "Result": None, "CompletionDisposition": None})
    contract.setdefault("Acceptance", {})["LastPlannedLoop"] = 596
    contract["Acceptance"]["HighestCompletedLoop"] = 586
    contract["Claims"]["RouteReconstructedWithoutSuppliedAntecedent"] = False
    contract["Claims"]["GridvilleRouteDiscoveryBranchCount"] = 12
    contract["Claims"]["NeighborhoodContractionFixtureCertified"] = False

    VALIDATOR_WRAPPER.write_text("#!/usr/bin/env python3\nfrom validate_rulebook_v3 import main\n\nif __name__ == '__main__':\n    main()\n")
    save(rb, contract)
    validate()
    commit("TSP loops 587-596: register planned frontier", [VALIDATOR_WRAPPER])


def set_frontier(rb: dict[str, Any], fid: str, status: str, loop: int, disposition: str) -> None:
    row = by_id(rb, "TSPFrontierObligations")[fid]
    row["Status"] = status
    row["ClosedByLoop"] = f"tsp-loop-{loop}" if status == "CLOSED" else None
    row["TrustDisposition"] = disposition


def loop_587() -> None:
    # Attempt before modifying the canonical state. Reset partial generated output on failure.
    has_effortless = shutil.which("effortless") is not None
    has_psql = shutil.which("psql") is not None
    status = "BLOCKED"; failure = None; output = ""
    build_ok = db_ok = conform_ok = False
    if has_effortless and has_psql:
        proc = run(["bash", "start.sh", "all"], cwd=DOMAIN, check=False, capture=True, env={**os.environ, "PGHOST": "localhost", "PGUSER": "postgres", "PGPASSWORD": os.environ.get("PGPASSWORD", "postgres")})
        output = (proc.stdout + "\n" + proc.stderr)[-12000:]
        if proc.returncode == 0:
            status = "SUCCEEDED"; build_ok = db_ok = conform_ok = True
        else:
            failure = f"start.sh all exited {proc.returncode}"
            run(["git", "reset", "--hard", "HEAD"])
            run(["git", "clean", "-fd", "--", str((DOMAIN / "effortless-postgres").relative_to(REPO)), str((DOMAIN / "rulespeak").relative_to(REPO))], check=False)
    else:
        missing = [name for name, present in [("effortless", has_effortless), ("psql", has_psql)] if not present]
        failure = "missing required executable(s): " + ", ".join(missing)

    rb = load_json(RULEBOOK); contract = load_json(CONTRACT)
    run_row = {"TSPExecutionRunId": "postgres-commissioning-loop-587", "TSPLoop": "tsp-loop-587", "Substrate": "POSTGRES_GENERATED_VIEWS", "AttemptedAt": os.environ.get("GITHUB_RUN_ID", "local-run"), "BuildCommand": "bash start.sh all", "BuildSucceeded": build_ok, "DatabaseInitialized": db_ok, "ConformanceSucceeded": conform_ok, "Status": status, "FailureReason": failure}
    upsert_rows(rb["TSPExecutionRuns"], "TSPExecutionRunId", [run_row])
    checks = []
    for kind, ok in [("GENERATED_BUILD", build_ok), ("DATABASE_INITIALIZATION", db_ok), ("PYTHON_POSTGRES_CONFORMANCE", conform_ok)]:
        checks.append({"TSPConformanceCheckId": f"loop-587-{kind.lower()}", "ExecutionRun": run_row["TSPExecutionRunId"], "CheckKind": kind, "SubjectId": "tsp-domain", "ExpectedValue": "PASS", "ActualValue": "PASS" if ok else status, "Status": "PASS" if ok else status})
    upsert_rows(rb["TSPConformanceChecks"], "TSPConformanceCheckId", checks)
    artifacts = []
    for kind, path in [("CANONICAL_RULEBOOK", RULEBOOK), ("GENERATED_POSTGRES_DIRECTORY", DOMAIN / "effortless-postgres")]:
        present = path.exists()
        digest = sha256(path) if path.is_file() else None
        artifacts.append({"TSPArtifactId": f"loop-587-{kind.lower()}", "ExecutionRun": run_row["TSPExecutionRunId"], "ArtifactKind": kind, "RelativePath": str(path.relative_to(REPO)), "SHA256": digest, "IsPresent": present})
    upsert_rows(rb["TSPArtifacts"], "TSPArtifactId", artifacts)
    if status == "SUCCEEDED":
        set_frontier(rb, "frontier-postgres-commissioning", "CLOSED", 587, "CLOSED_BY_LIVE_SUBSTRATE_CONFORMANCE")
        after = "Generated Postgres, database initialization, and Python/Postgres conformance all passed."
        disposition = "CLOSED"
    else:
        set_frontier(rb, "frontier-postgres-commissioning", "BLOCKED", 587, "BLOCKED_BY_EXECUTION_ENVIRONMENT_WITH_EXACT_FAILURE_RECORDED")
        after = f"Commissioning attempted and recorded as BLOCKED: {failure}. No green substrate claim was made."
        disposition = "BLOCKED"
