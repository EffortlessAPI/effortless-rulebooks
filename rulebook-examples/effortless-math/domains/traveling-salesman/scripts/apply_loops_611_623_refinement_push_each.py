#!/usr/bin/env python3
"""Execute loops 611-623 and push every semantic commit immediately.

The assembled refinement module owns the mathematical changes. This wrapper
preserves the existing validation workflow outside semantic commits, makes each
validated plan/loop commit durable, and records that convergence means rising
coherence rather than monotonically shrinking physical representation.
"""
from __future__ import annotations

from typing import Any

import apply_loops_611_623_refinement as refinement

_original_commit = refinement.commit
_original_write_validation_files = refinement.write_validation_files
_original_loop_611 = refinement.LOOP_FUNCS[611]
_original_append_readme_section = refinement.append_readme_section

# Refine the working prediction before the PLANNED rows are written.
refinement.LOOPS[611]["before"] = (
    "Loop 610 records eight canonical predicates, but relational atoms and reusable "
    "operators are not distinguished. Convergence could also be misread as requiring "
    "monotone table or predicate shrinkage."
)
refinement.LOOPS[611]["criterion"] = (
    "Classify the eight historical predicates as candidate atoms or operator-derived "
    "concepts, preserve their loop-610 provenance, and record that local expansion is "
    "permitted when an opaque dependency is replaced by explicit reusable machinery. "
    "Measure convergence by coherence, reuse, trust-boundary reduction, and residual "
    "ambiguity rather than by monotone physical size."
)


def semantic_validation_files_only() -> None:
    workflow_text = (
        refinement.VALIDATION_WORKFLOW.read_text()
        if refinement.VALIDATION_WORKFLOW.is_file()
        else None
    )
    _original_write_validation_files()
    if workflow_text is None:
        refinement.VALIDATION_WORKFLOW.unlink(missing_ok=True)
    else:
        refinement.VALIDATION_WORKFLOW.write_text(workflow_text)


def loop_611_with_coherence_guardrail(
    rb: dict[str, Any], contract: dict[str, Any]
) -> tuple[str, str]:
    after, witness = _original_loop_611(rb, contract)
    refinement.set_meta(
        rb,
        "convergence_target",
        "string",
        string="COHERENCE_NOT_MONOTONE_PHYSICAL_SHRINKAGE",
    )
    refinement.set_meta(
        rb,
        "convergence_allows_local_expansion",
        "boolean",
        boolean=True,
    )
    refinement.set_meta(
        rb,
        "black_box_internalization_may_expand_explicit_dag",
        "boolean",
        boolean=True,
    )
    refinement.set_meta(
        rb,
        "monotone_size_reduction_claimed",
        "boolean",
        boolean=False,
    )
    claims = contract.setdefault("Claims", {})
    claims.update(
        {
            "CoherenceConvergencePredictionRecorded": True,
            "LocalExpansionDuringInternalizationAllowed": True,
            "MonotoneSchemaShrinkageClaimed": False,
        }
    )
    measurement = refinement.table_index(rb, "TSPConvergenceMeasurements")[
        "convergence-loop-611"
    ]
    measurement["Notes"] += (
        " Local representation may grow when an opaque node is decomposed; support is "
        "measured by increasing semantic reuse, explicit warrants, smaller opaque trust "
        "boundaries, and reduced residual ambiguity."
    )
    return (
        after
        + " Recorded that local representational growth is compatible with global "
        "semantic convergence when it replaces opacity with reusable structure.",
        witness
        + " The convergence objective is coherence rather than monotone row, table, or "
        "predicate-count reduction.",
    )


def append_readme_section_with_coherence_guardrail() -> None:
    _original_append_readme_section()
    text = refinement.README.read_text()
    marker = "**Coherence guardrail.**"
    if marker not in text:
        text += (
            "\n\n**Boundary Fiber.** The asymmetric stress fixture groups equal-port, "
            "equal-coverage paths into boundary fibers before minimum valuation selects "
            "the surviving representative.\n\n"
            "**Coherence guardrail.** Convergence is not defined as monotonically "
            "decreasing tables, rows, or named predicates. Internalizing one opaque black "
            "box may temporarily expand the explicit DAG. The represented trend is toward "
            "greater reuse, clearer warrants, smaller opaque trust boundaries, and less "
            "residual ambiguity—not toward superficial brevity.\n"
        )
        refinement.README.write_text(text)


def durable_commit(message: str, paths: list[refinement.Path]) -> None:
    _original_commit(message, paths)
    refinement.run(
        ["git", "push", "origin", f"HEAD:{refinement.TARGET_BRANCH}"],
        cwd=refinement.REPO,
    )


refinement.write_validation_files = semantic_validation_files_only
refinement.LOOP_FUNCS[611] = loop_611_with_coherence_guardrail
refinement.append_readme_section = append_readme_section_with_coherence_guardrail
refinement.commit = durable_commit


if __name__ == "__main__":
    refinement.main()
