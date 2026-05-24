"""
CLI command handlers for the Job Search RAG Assistant.

Each public function corresponds to a CLI subcommand and encapsulates
the wiring, orchestration, and output for that command.
"""

from __future__ import annotations

import argparse
import asyncio
import csv as csv_mod
import shutil
import sys
import webbrowser
from pathlib import Path

from jobsearch_rag.adapters import AdapterRegistry
from jobsearch_rag.adapters.base import JobListing
from jobsearch_rag.adapters.session import SessionConfig, SessionManager
from jobsearch_rag.config import Settings, load_settings
from jobsearch_rag.export import CSVExporter, JDFileExporter, MarkdownExporter
from jobsearch_rag.logging import configure_file_logging
from jobsearch_rag.pipeline.eval import (
    EvalHistory,
    EvalReport,
    EvalRunner,
    ModelComparisonResult,
)
from jobsearch_rag.pipeline.ranker import RankedListing, Ranker
from jobsearch_rag.pipeline.rescorer import Rescorer
from jobsearch_rag.pipeline.review import ReviewSession
from jobsearch_rag.pipeline.runner import PipelineRunner
from jobsearch_rag.rag.decisions import DecisionRecorder
from jobsearch_rag.rag.embedder import Embedder
from jobsearch_rag.rag.indexer import Indexer
from jobsearch_rag.rag.scorer import Scorer, ScoreResult
from jobsearch_rag.rag.store import VectorStore
from jobsearch_rag.text import slugify


def _read_jd_text(
    rank: int,
    title: str,
    company: str,
    *,
    jd_dir: Path | str = "output/jds",
) -> str:
    """
    Read the full JD body from the corresponding JD markdown file.

    Returns the text after the ``## Job Description`` marker, or an
    empty string if the file is missing or the marker is absent.
    """
    jd_dir = Path(jd_dir)
    filename = f"{rank:03d}_{slugify(company)}_{slugify(title)}.md"
    jd_path = jd_dir / filename
    if not jd_path.exists():
        return ""
    content = jd_path.read_text()
    marker = "## Job Description\n"
    idx = content.find(marker)
    if idx == -1:
        return ""
    return content[idx + len(marker) :].strip()


# Board login URLs for interactive authentication
_LOGIN_URLS: dict[str, str] = {
    "ziprecruiter": "https://www.ziprecruiter.com/authn/login",
    "linkedin": "https://www.linkedin.com/login",
    "indeed": "https://secure.indeed.com/auth",
}


def handle_login(args: argparse.Namespace) -> None:
    """
    Open an interactive browser session for manual login.

    Launches a headed (visible) browser, navigates to the board's
    login page, and waits for the operator to complete authentication
    (including any CAPTCHA / Cloudflare challenges).  Once the
    operator presses Enter, the session cookies are saved to
    ``data/{board}_session.json`` for reuse by subsequent searches.

    Use ``--browser msedge`` to launch Microsoft Edge instead of
    Chromium — Edge bypasses Cloudflare where Chromium cannot.
    """
    board = args.board
    browser = getattr(args, "browser", None)
    login_url = _LOGIN_URLS.get(board, f"https://www.{board}.com")

    config = SessionConfig(
        board_name=board,
        headless=False,  # Always headed for interactive login
        browser_channel=browser,
    )

    async def _run() -> None:
        async with SessionManager(config) as session:
            page = await session.new_page()
            await page.goto(login_url, wait_until="domcontentloaded")

            print(f"\n{'=' * 60}")
            print(f"  Interactive Login — {board}")
            print(f"{'=' * 60}")
            print(f"  Browser opened to: {login_url}")
            print("  Complete login / solve any CAPTCHA in the browser.")
            print(f"{'=' * 60}\n")

            # Block until operator signals completion
            input("Press Enter when you have finished logging in...")

            path = await session.save_storage_state()
            print(f"\nSession saved to {path}")
            print("You can now run 'search' — cookies will be loaded automatically.")

    asyncio.run(_run())


def handle_boards() -> None:
    """List all registered adapter board names."""
    boards = AdapterRegistry.list_registered()
    if not boards:
        print("No adapters registered.")
        return
    print("Registered adapters:")
    for name in sorted(boards):
        print(f"  - {name}")


def handle_index(args: argparse.Namespace) -> None:
    """Index resume and/or archetypes into ChromaDB."""
    settings = load_settings()
    embedder = Embedder(
        base_url=settings.ollama.base_url,
        embed_model=settings.ollama.embed_model,
        llm_model=settings.ollama.llm_model,
    )
    store = VectorStore(persist_dir=settings.chroma.persist_dir)
    indexer = Indexer(store=store, embedder=embedder)

    archetypes_only = getattr(args, "archetypes_only", False)

    async def _run() -> None:
        await embedder.health_check()

        if archetypes_only:
            # Rebuild archetypes, negative signals, and positive signals only
            n_archetypes = await indexer.index_archetypes(settings.archetypes_path)
            print(f"Indexed {n_archetypes} archetypes")
            n_neg = await indexer.index_negative_signals(
                settings.global_rubric_path, settings.archetypes_path
            )
            print(f"Indexed {n_neg} negative signals")
            n_pos = await indexer.index_global_positive_signals(settings.global_rubric_path)
            print(f"Indexed {n_pos} global positive signals")
            return

        if not args.resume_only:
            n_archetypes = await indexer.index_archetypes(settings.archetypes_path)
            print(f"Indexed {n_archetypes} archetypes")
            n_neg = await indexer.index_negative_signals(
                settings.global_rubric_path, settings.archetypes_path
            )
            print(f"Indexed {n_neg} negative signals")
            n_pos = await indexer.index_global_positive_signals(settings.global_rubric_path)
            print(f"Indexed {n_pos} global positive signals")

        n_resume = await indexer.index_resume(settings.resume_path)
        print(f"Indexed {n_resume} resume chunks")

    asyncio.run(_run())


def handle_search(args: argparse.Namespace) -> None:
    """Run search across enabled boards."""
    settings = load_settings()

    log_dir = Path(settings.chroma.persist_dir).parent / "logs"
    configure_file_logging(str(log_dir))

    runner = PipelineRunner(settings)

    boards = [args.board] if args.board else None

    async def _run() -> None:
        result = await runner.run(
            boards=boards,
            overnight=args.overnight,
            force_rescore=args.force_rescore,
        )

        # Print summary
        print(f"\n{'=' * 60}")
        print(" Search Results Summary")
        print(f"{'=' * 60}")
        print(f" Boards searched: {', '.join(result.boards_searched)}")
        print(f" Total found:     {result.summary.total_found}")
        print(f" Scored:          {result.summary.total_scored}")
        print(f" Deduplicated:    {result.summary.total_deduplicated}")
        print(f" Excluded:        {result.summary.total_excluded}")
        print(f" Prior decisions: {result.skipped_decisions}")
        print(f" Failed:          {result.failed_listings}")
        print(f" Final results:   {len(result.ranked_listings)}")
        print(f"{'=' * 60}\n")

        # Print surfaced errors if any
        if result.errors:
            print(" Surfaced Errors")
            print(f"{'-' * 60}")
            for surfaced_error in result.errors:
                print(f" Error:      {surfaced_error.error}")
                print(f" Service:    {surfaced_error.service or 'unknown'}")
                print(f" Suggestion: {surfaced_error.suggestion or 'No suggestion available'}")
                print()

        # Print ranked listings
        for i, ranked in enumerate(result.ranked_listings, 1):
            listing = ranked.listing
            print(f"{i}. [{ranked.final_score:.2f}] {listing.title}")
            print(f"   {listing.company} | {listing.board} | {listing.url}")
            print(f"   {ranked.score_explanation()}")
            if ranked.duplicate_boards:
                print(f"   Also on: {', '.join(ranked.duplicate_boards)}")
            print()

        # Auto-export results
        if result.ranked_listings:
            out_dir = Path(settings.output.output_dir)
            out_dir.mkdir(parents=True, exist_ok=True)

            md_path = str(out_dir / "results.md")
            MarkdownExporter().export(result.ranked_listings, md_path, summary=result.summary)
            print(f"Exported Markdown → {md_path}")

            csv_path = str(out_dir / "results.csv")
            CSVExporter().export(result.ranked_listings, csv_path, summary=result.summary)
            print(f"Exported CSV      → {csv_path}")

            jd_dir = str(out_dir / "jds")
            jd_paths = JDFileExporter().export(
                result.ranked_listings, jd_dir, summary=result.summary
            )
            print(f"Exported JDs      → {jd_dir}/ ({len(jd_paths)} files)")

        # Open top N in browser if requested
        open_n = args.open_top if args.open_top is not None else settings.output.open_top_n
        if open_n and open_n > 0 and result.ranked_listings:
            to_open = result.ranked_listings[:open_n]
            print(f"Opening top {len(to_open)} results in browser...")
            for ranked in to_open:
                try:
                    webbrowser.open(ranked.listing.url)
                except Exception as exc:
                    print(f"  Failed to open {ranked.listing.url}: {exc}")

    asyncio.run(_run())


def handle_decide(args: argparse.Namespace) -> None:
    """Record a verdict on a job listing."""
    settings = load_settings()
    embedder = Embedder(
        base_url=settings.ollama.base_url,
        embed_model=settings.ollama.embed_model,
        llm_model=settings.ollama.llm_model,
    )
    store = VectorStore(persist_dir=settings.chroma.persist_dir)
    recorder = DecisionRecorder(store=store, embedder=embedder)

    # Look up the job in the decisions or latest results
    # For now, we need the JD text from somewhere — check if it exists in any collection
    existing = recorder.get_decision(args.job_id)
    if existing:
        # Re-recording with a new verdict — retrieve the stored document
        results = store.get_documents(
            collection_name="decisions",
            ids=[f"decision-{args.job_id}"],
        )
        documents = results.get("documents", [])
        if documents and documents[0]:
            jd_text = documents[0]
        else:
            print(f"Error: Could not retrieve JD text for job '{args.job_id}'")
            sys.exit(1)

        board = existing.get("board", "unknown")
        title = existing.get("title", "")
        company = existing.get("company", "")
    else:
        print(f"Error: No job found with ID '{args.job_id}'")
        print("The job must have been previously scored or decided upon.")
        sys.exit(1)

    async def _run() -> None:
        await recorder.record(
            job_id=args.job_id,
            verdict=args.verdict,
            jd_text=jd_text,
            board=board,
            title=title,
            company=company,
            reason=args.reason,
        )
        print(f"Recorded '{args.verdict}' for {args.job_id}")
        if args.reason:
            print(f"  Reason: {args.reason}")
        print(f"  History size: {recorder.history_count()} decisions")

    asyncio.run(_run())


def handle_decisions(args: argparse.Namespace) -> None:
    """Dispatch decisions subcommands: show, remove, audit."""
    settings = load_settings()
    embedder = Embedder(
        base_url=settings.ollama.base_url,
        embed_model=settings.ollama.embed_model,
        llm_model=settings.ollama.llm_model,
    )
    store = VectorStore(persist_dir=settings.chroma.persist_dir)
    recorder = DecisionRecorder(store=store, embedder=embedder)

    sub = args.decisions_command
    if sub == "show":
        _handle_decisions_show(recorder, args.job_id)
    elif sub == "remove":
        _handle_decisions_remove(recorder, args.job_id)
    elif sub == "audit":
        _handle_decisions_audit(recorder)
    else:
        print("Usage: jobsearch-rag decisions {show,remove,audit}")
        sys.exit(1)


def _handle_decisions_show(recorder: DecisionRecorder, job_id: str) -> None:
    """Print metadata for a specific decision."""
    decision = recorder.get_decision(job_id)
    if decision is None:
        print(f"No decision found for job_id '{job_id}'")
        return

    print(f"Decision for {job_id}:")
    for key, value in sorted(decision.items()):
        print(f"  {key}: {value}")


def _handle_decisions_remove(recorder: DecisionRecorder, job_id: str) -> None:
    """Remove a decision from ChromaDB (JSONL audit log is preserved)."""
    removed = recorder.remove_decision(job_id)
    if removed:
        print(f"Removed decision for '{job_id}' from ChromaDB")
        print("  Note: 'removed' entry appended to JSONL audit log")
    else:
        print(f"No decision found for job_id '{job_id}'")


def _handle_decisions_audit(recorder: DecisionRecorder) -> None:
    """List all decisions that have a non-empty reason field."""
    results = recorder.audit_decisions()
    if not results:
        print("No decisions with reasons to audit.")
        return

    print(f"Decisions with reasons ({len(results)}):\n")
    for entry in results:
        print(f"  {entry['job_id']}  [{entry['verdict']}]  {entry['reason']}")


def handle_review(args: argparse.Namespace) -> None:
    """
    Interactively review undecided listings from the latest search.

    Walks through each undecided listing in ranked order, displaying
    scores and compensation.  The operator enters y/n/m to record a
    verdict, s to skip, o to open the JD, or q to quit.
    """
    settings = load_settings()
    embedder = Embedder(
        base_url=settings.ollama.base_url,
        embed_model=settings.ollama.embed_model,
        llm_model=settings.ollama.llm_model,
    )
    store = VectorStore(persist_dir=settings.chroma.persist_dir)
    recorder = DecisionRecorder(store=store, embedder=embedder)

    # Load latest results from CSV
    out_dir = Path(settings.output.output_dir)
    csv_path = out_dir / "results.csv"
    if not csv_path.exists():
        print("No results found. Run 'search' first.")
        return

    ranked_listings: list[RankedListing] = []
    jd_dir = out_dir / "jds"

    with open(csv_path) as f:
        reader = csv_mod.DictReader(f)
        for _i, row in enumerate(reader, 1):
            title = row.get("title", "")
            company = row.get("company", "")
            full_text = _read_jd_text(_i, title, company, jd_dir=jd_dir)
            listing = JobListing(
                board=row.get("board", "unknown"),
                external_id=row.get("url", "").rstrip("/").rsplit("/", 1)[-1],
                title=title,
                company=company,
                location=row.get("location", ""),
                url=row.get("url", ""),
                full_text=full_text,
                comp_min=float(row["comp_min"]) if row.get("comp_min") else None,
                comp_max=float(row["comp_max"]) if row.get("comp_max") else None,
            )
            scores = ScoreResult(
                fit_score=float(row.get("fit_score", 0)),
                archetype_score=float(row.get("archetype_score", 0)),
                history_score=float(row.get("history_score", 0)),
                disqualified=row.get("disqualified", "").lower() == "true",
                disqualifier_reason=row.get("disqualifier_reason") or None,
                comp_score=float(row.get("comp_score", 0)),
                culture_score=float(row.get("culture_score", 0)),
            )
            ranked = RankedListing(
                listing=listing,
                scores=scores,
                final_score=float(row.get("final_score", 0)),
            )
            ranked_listings.append(ranked)

    session = ReviewSession(
        ranked_listings=ranked_listings,
        recorder=recorder,
        jd_dir=str(jd_dir),
    )

    undecided = session.undecided_listings()
    if not undecided:
        print("All listings have been decided — nothing to review.")
        return

    print(f"\n{len(undecided)} undecided listing(s) to review.\n")
    print("Commands: y=yes  n=no  m=maybe  s=skip  o=open  q=quit")
    print("After entering a verdict, you can add an optional reason.\n")

    async def _run() -> None:
        for idx, ranked in enumerate(undecided, 1):
            print(session.format_listing(ranked, rank=idx, total=len(undecided)))

            while True:
                try:
                    key = input("  > ").strip().lower()
                except (EOFError, KeyboardInterrupt):
                    key = "q"

                if key == "q":
                    print(f"\nReview stopped. {idx - 1} listing(s) reviewed.")
                    return
                elif key == "o":
                    session.open_listing(ranked, rank=idx)
                    continue  # re-prompt after opening
                elif key == "s":
                    break  # skip — advance to next listing
                elif session.should_record(key):
                    try:
                        reason = input("  Reason (Enter to skip): ").strip()
                    except (EOFError, KeyboardInterrupt):
                        reason = ""
                    await session.record_verdict(ranked, key, reason=reason)
                    msg = f"  Recorded: {key}"
                    if reason:
                        msg += f" — {reason}"
                    print(msg)
                    break
                else:
                    print("  Invalid input. Use y/n/m/s/o/q")

        print(f"\nReview complete. All {len(undecided)} listing(s) reviewed.")

    asyncio.run(_run())


def handle_rescore(args: argparse.Namespace) -> None:
    """
    Re-score existing JDs through updated collections without browser automation.

    Loads previously exported JD files from ``output/jds/``, re-scores each
    through the current RAG collections (archetypes, negative signals, resume,
    decisions), re-ranks, and re-exports all results.
    """
    settings = load_settings()
    embedder = Embedder(
        base_url=settings.ollama.base_url,
        embed_model=settings.ollama.embed_model,
        llm_model=settings.ollama.llm_model,
    )
    store = VectorStore(persist_dir=settings.chroma.persist_dir)
    scorer = Scorer(
        store=store,
        embedder=embedder,
        disqualify_on_llm_flag=settings.scoring.disqualify_on_llm_flag,
    )
    ranker = Ranker(
        archetype_weight=settings.scoring.archetype_weight,
        fit_weight=settings.scoring.fit_weight,
        history_weight=settings.scoring.history_weight,
        comp_weight=settings.scoring.comp_weight,
        negative_weight=settings.scoring.negative_weight,
        culture_weight=settings.scoring.culture_weight,
        min_score_threshold=settings.scoring.min_score_threshold,
    )
    rescorer = Rescorer(
        scorer=scorer,
        ranker=ranker,
        base_salary=settings.scoring.base_salary,
    )

    jd_dir = Path(settings.output.output_dir) / "jds"

    async def _run() -> None:
        await embedder.health_check()

        result = await rescorer.rescore(str(jd_dir))

        print(f"\n{'=' * 60}")
        print(" Rescore Results Summary")
        print(f"{'=' * 60}")
        print(f" JDs loaded:     {result.total_loaded}")
        print(f" Scored:         {result.summary.total_scored}")
        print(f" Deduplicated:   {result.summary.total_deduplicated}")
        print(f" Excluded:       {result.summary.total_excluded}")
        print(f" Failed:         {result.failed_listings}")
        print(f" Final results:  {len(result.ranked_listings)}")
        print(f"{'=' * 60}\n")

        for i, ranked in enumerate(result.ranked_listings, 1):
            listing = ranked.listing
            print(f"{i}. [{ranked.final_score:.2f}] {listing.title}")
            print(f"   {listing.company} | {listing.board} | {listing.url}")
            print(f"   {ranked.score_explanation()}")
            print()

        # Re-export results
        if result.ranked_listings:
            out_dir = Path(settings.output.output_dir)
            out_dir.mkdir(parents=True, exist_ok=True)

            md_path = str(out_dir / "results.md")
            MarkdownExporter().export(result.ranked_listings, md_path, summary=result.summary)
            print(f"Exported Markdown → {md_path}")

            csv_path = str(out_dir / "results.csv")
            CSVExporter().export(result.ranked_listings, csv_path, summary=result.summary)
            print(f"Exported CSV      → {csv_path}")

            jd_out_dir = str(out_dir / "jds")
            jd_paths = JDFileExporter().export(
                result.ranked_listings, jd_out_dir, summary=result.summary
            )
            print(f"Exported JDs      → {jd_out_dir}/ ({len(jd_paths)} files)")

    asyncio.run(_run())


def handle_eval(args: argparse.Namespace) -> None:
    """
    Evaluate the scoring pipeline against stored human decisions.

    Loads all decisions from ChromaDB, re-scores each JD through the
    current scorer/ranker configuration, and prints agreement rate,
    precision, recall, and Spearman correlation.

    With ``--compare-models MODEL_A MODEL_B``, runs the evaluation twice
    (once per model) and prints a side-by-side comparison table.
    """
    settings = load_settings()
    compare_models: list[str] | None = getattr(args, "compare_models", None)

    if compare_models:
        _handle_eval_compare(settings, compare_models)
    else:
        _handle_eval_single(settings)


def _build_eval_stack(
    settings: Settings, *, llm_model: str | None = None
) -> tuple[Embedder, EvalRunner]:
    """Construct an Embedder + EvalRunner from settings."""
    embedder = Embedder(
        base_url=settings.ollama.base_url,
        embed_model=settings.ollama.embed_model,
        llm_model=llm_model or settings.ollama.llm_model,
    )
    store = VectorStore(persist_dir=settings.chroma.persist_dir)
    scorer = Scorer(
        store=store,
        embedder=embedder,
        disqualify_on_llm_flag=settings.scoring.disqualify_on_llm_flag,
    )
    ranker = Ranker(
        archetype_weight=settings.scoring.archetype_weight,
        fit_weight=settings.scoring.fit_weight,
        history_weight=settings.scoring.history_weight,
        comp_weight=settings.scoring.comp_weight,
        negative_weight=settings.scoring.negative_weight,
        culture_weight=settings.scoring.culture_weight,
        min_score_threshold=settings.scoring.min_score_threshold,
    )
    runner = EvalRunner(scorer=scorer, ranker=ranker, store=store)
    return embedder, runner


def _handle_eval_single(settings: Settings) -> None:
    """Run a single-model evaluation."""
    embedder, runner = _build_eval_stack(settings)

    async def _run() -> None:
        await embedder.health_check()
        result = await runner.evaluate()

        print(f"\n{'=' * 60}")
        print(" Evaluation Results")
        print(f"{'=' * 60}")
        print(f" Decisions evaluated: {result.decisions_evaluated}")
        print(f" Agreement rate:      {result.agreement_rate:.1%}")
        print(f" Precision:           {result.precision:.1%}")
        print(f" Recall:              {result.recall:.1%}")
        print(f" Spearman correlation: {result.spearman:.2f}")
        print(f"{'=' * 60}\n")

        if result.decisions_evaluated == 0:
            print("No decisions found. Record some decisions first with 'decide'.")
            return

        report_path = EvalReport.write(result, settings.output.output_dir)
        print(f"Report written to {report_path}")

        EvalHistory.append(result, "data/eval_history.jsonl")
        print("History appended to data/eval_history.jsonl")

    asyncio.run(_run())


def _handle_eval_compare(settings: Settings, models: list[str]) -> None:
    """Run dual-model evaluation and print comparison table."""
    model_a, model_b = models
    embedder_a, runner_a = _build_eval_stack(settings, llm_model=model_a)
    embedder_b, runner_b = _build_eval_stack(settings, llm_model=model_b)

    async def _run() -> None:
        await embedder_a.health_check()
        result_a = await runner_a.evaluate()

        await embedder_b.health_check()
        result_b = await runner_b.evaluate()

        comparison = ModelComparisonResult(
            model_a=model_a,
            model_b=model_b,
            result_a=result_a,
            result_b=result_b,
        )

        def _sign(v: float) -> str:
            return f"+{v:.2f}" if v >= 0 else f"{v:.2f}"

        print(f"\n{'=' * 60}")
        print(" Model Comparison")
        print(f"{'=' * 60}")
        print(f" {'Metric':<22} {model_a:<14} {model_b:<14} {'delta':>8}")
        print(f" {'-' * 58}")
        print(
            f" {'Agreement rate':<22} "
            f"{result_a.agreement_rate:<14.1%} "
            f"{result_b.agreement_rate:<14.1%} "
            f"{_sign(comparison.agreement_delta):>8}"
        )
        print(
            f" {'Precision':<22} "
            f"{result_a.precision:<14.1%} "
            f"{result_b.precision:<14.1%} "
            f"{_sign(comparison.precision_delta):>8}"
        )
        print(
            f" {'Recall':<22} "
            f"{result_a.recall:<14.1%} "
            f"{result_b.recall:<14.1%} "
            f"{_sign(comparison.recall_delta):>8}"
        )
        print(
            f" {'Spearman correlation':<22} "
            f"{result_a.spearman:<14.2f} "
            f"{result_b.spearman:<14.2f} "
            f"{_sign(comparison.spearman_delta):>8}"
        )
        print(f"{'=' * 60}\n")

    asyncio.run(_run())


def handle_export(args: argparse.Namespace) -> None:
    """
    Re-export last results in a specific format.

    Search results are auto-exported during ``search``.  This command
    can re-export from the saved output files in a different format.
    """
    settings = load_settings()
    out_dir = Path(settings.output.output_dir)
    md_path = out_dir / "results.md"
    csv_path = out_dir / "results.csv"

    if not md_path.exists() and not csv_path.exists():
        print("No previous results found. Run 'search' first.")
        sys.exit(1)

    fmt = args.format
    if fmt == "markdown" and md_path.exists():
        print(md_path.read_text())
    elif fmt == "csv" and csv_path.exists():
        print(csv_path.read_text())
    else:
        print(f"No {fmt} export found in {out_dir}. Run 'search' to generate results.")


# Known ChromaDB collections used by the pipeline
_COLLECTIONS = ["resume", "role_archetypes", "negative_signals", "decisions"]


def handle_reset(args: argparse.Namespace) -> None:
    """Reset ChromaDB collections and optionally clear output files."""
    settings = load_settings()
    store = VectorStore(persist_dir=settings.chroma.persist_dir)

    collections = [args.collection] if args.collection else list(_COLLECTIONS)

    for name in collections:
        store.reset_collection(name)
        print(f"  Reset collection: {name}")

    # Optionally clear output files
    if args.clear_output:
        out_dir = Path(settings.output.output_dir)
        if out_dir.exists():
            shutil.rmtree(out_dir)
            out_dir.mkdir(parents=True, exist_ok=True)
            print(f"  Cleared output directory: {out_dir}")

    print(f"\nReset complete. {len(collections)} collection(s) cleared.")
    if not args.collection:
        print("Run 'index' to re-index resume and archetypes before searching.")


def build_parser() -> argparse.ArgumentParser:
    """Build the CLI argument parser with all subcommands."""
    parser = argparse.ArgumentParser(
        prog="jobsearch-rag",
        description="Local LLM + RAG pipeline for intelligent job search filtering",
    )
    sub = parser.add_subparsers(dest="command", required=True)

    # -- index ---------------------------------------------------------------
    index_p = sub.add_parser("index", help="Index resume and/or archetypes into ChromaDB")
    index_p.add_argument(
        "--resume-only",
        action="store_true",
        help="Re-index resume only (skip archetypes)",
    )
    index_p.add_argument(
        "--archetypes-only",
        action="store_true",
        help="Re-index archetypes and negative signals only (skip resume)",
    )

    # -- search --------------------------------------------------------------
    search_p = sub.add_parser("search", help="Run search across enabled boards")
    search_p.add_argument("--board", type=str, default=None, help="Search a specific board only")
    search_p.add_argument(
        "--overnight",
        action="store_true",
        help="Enable overnight mode (slow, headed, capped)",
    )
    search_p.add_argument(
        "--open-top",
        type=int,
        default=None,
        metavar="N",
        help="Open top N results in browser tabs",
    )
    search_p.add_argument(
        "--force-rescore",
        action="store_true",
        help="Re-score all listings even if they have prior decisions",
    )

    # -- export --------------------------------------------------------------
    export_p = sub.add_parser("export", help="Export last results")
    export_p.add_argument(
        "--format",
        choices=["markdown", "csv", "json"],
        default="markdown",
        help="Output format (default: markdown)",
    )

    # -- rescore -------------------------------------------------------------
    sub.add_parser(
        "rescore",
        help="Re-score existing JDs through updated collections (no browser)",
    )

    # -- decide --------------------------------------------------------------
    decide_p = sub.add_parser("decide", help="Record your verdict on a role")
    decide_p.add_argument("job_id", type=str, help="Job ID from the latest search output")
    decide_p.add_argument(
        "--verdict",
        choices=["yes", "no", "maybe"],
        required=True,
        help="Your verdict on this role",
    )
    decide_p.add_argument(
        "--reason",
        type=str,
        default="",
        help="Optional reason explaining your verdict",
    )

    # -- review --------------------------------------------------------------
    sub.add_parser(
        "review",
        help="Interactively review and decide on undecided listings",
    )

    # -- decisions -----------------------------------------------------------
    decisions_p = sub.add_parser(
        "decisions",
        help="Inspect, audit, and manage the decisions collection",
    )
    decisions_sub = decisions_p.add_subparsers(
        dest="decisions_command",
        required=True,
    )

    decisions_show_p = decisions_sub.add_parser(
        "show",
        help="Print metadata for a specific decision",
    )
    decisions_show_p.add_argument(
        "job_id",
        type=str,
        help="Job ID to look up in the decisions collection",
    )

    decisions_remove_p = decisions_sub.add_parser(
        "remove",
        help="Remove a decision from ChromaDB (JSONL audit log is preserved)",
    )
    decisions_remove_p.add_argument(
        "job_id",
        type=str,
        help="Job ID to remove from the decisions collection",
    )

    decisions_sub.add_parser(
        "audit",
        help="List all decisions that have a non-empty reason field",
    )

    # -- boards --------------------------------------------------------------
    sub.add_parser("boards", help="List registered adapters")

    # -- reset ---------------------------------------------------------------
    reset_p = sub.add_parser(
        "reset",
        help="Reset ChromaDB collections (clears all indexed data)",
    )
    reset_p.add_argument(
        "--collection",
        type=str,
        default=None,
        choices=["resume", "role_archetypes", "negative_signals", "decisions"],
        help="Reset a specific collection only (default: all)",
    )
    reset_p.add_argument(
        "--clear-output",
        action="store_true",
        help="Also delete output files (results.md, results.csv, jds/)",
    )

    # -- login ---------------------------------------------------------------
    login_p = sub.add_parser(
        "login",
        help="Open interactive browser for manual login (saves session cookies)",
    )

    # -- eval ----------------------------------------------------------------
    eval_p = sub.add_parser(
        "eval",
        help="Evaluate scoring pipeline against stored human decisions",
    )
    eval_p.add_argument(
        "--compare-models",
        nargs=2,
        metavar=("MODEL_A", "MODEL_B"),
        default=None,
        help="Compare two LLM models side-by-side (e.g. mistral:7b llama3:8b)",
    )
    login_p.add_argument(
        "--board",
        type=str,
        required=True,
        help="Board to authenticate with (e.g. ziprecruiter)",
    )
    login_p.add_argument(
        "--browser",
        type=str,
        default=None,
        metavar="CHANNEL",
        help="Browser channel: msedge, chrome, chromium (default: board setting or chromium)",
    )

    return parser
