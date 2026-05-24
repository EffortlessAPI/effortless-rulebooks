"""
Evaluation harness — measures pipeline-vs-human agreement.

Re-scores every stored decision's JD through the current scorer/ranker
configuration and computes agreement rate, precision, recall, and Spearman
rank correlation against the human verdicts.
"""

from __future__ import annotations

import json
from dataclasses import dataclass, field
from datetime import UTC, datetime
from pathlib import Path
from typing import TYPE_CHECKING

from jobsearch_rag.logging import logger

if TYPE_CHECKING:
    from collections.abc import Sequence

    from jobsearch_rag.pipeline.ranker import Ranker
    from jobsearch_rag.rag.scorer import Scorer
    from jobsearch_rag.rag.store import VectorStore


_VERDICT_ORDINAL: dict[str, float] = {"no": 0.0, "maybe": 1.0, "yes": 2.0}


@dataclass
class EvalDecision:
    """Per-decision evaluation result."""

    job_id: str
    verdict: str
    final_score: float
    above_threshold: bool
    agreed: bool


@dataclass
class EvalResult:
    """Aggregate evaluation metrics."""

    decisions_evaluated: int
    agreement_rate: float
    precision: float
    recall: float
    spearman: float = 0.0
    per_decision: list[EvalDecision] = field(default_factory=lambda: list[EvalDecision]())


def spearman_rank_correlation(x: Sequence[float], y: Sequence[float]) -> float:
    """
    Compute Spearman rank correlation between two sequences.

    Uses average ranks for ties. Returns 0.0 for fewer than 2 items or
    when either sequence has zero variance (all identical values).
    No external dependencies — stdlib only.
    """
    n = len(x)
    if n != len(y) or n < 2:
        return 0.0

    def _rank(values: Sequence[float]) -> list[float]:
        indexed = sorted(enumerate(values), key=lambda pair: pair[1])
        ranks = [0.0] * n
        i = 0
        while i < n:
            j = i
            while j < n - 1 and indexed[j + 1][1] == indexed[i][1]:
                j += 1
            avg_rank = (i + j) / 2.0 + 1.0  # 1-based average rank
            for k in range(i, j + 1):
                ranks[indexed[k][0]] = avg_rank
            i = j + 1
        return ranks

    rx = _rank(x)
    ry = _rank(y)

    mean_x = sum(rx) / n
    mean_y = sum(ry) / n

    cov = sum((a - mean_x) * (b - mean_y) for a, b in zip(rx, ry, strict=True))
    std_x = sum((a - mean_x) ** 2 for a in rx) ** 0.5
    std_y = sum((b - mean_y) ** 2 for b in ry) ** 0.5

    if std_x == 0.0 or std_y == 0.0:
        return 0.0

    return cov / (std_x * std_y)


class EvalReport:
    """Writes a Markdown evaluation report to disk."""

    @staticmethod
    def write(result: EvalResult, output_dir: str) -> Path:
        """Write an eval report to ``{output_dir}/eval_YYYY-MM-DD.md``."""
        out = Path(output_dir)
        out.mkdir(parents=True, exist_ok=True)

        today = datetime.now(UTC).strftime("%Y-%m-%d")
        path = out / f"eval_{today}.md"

        lines: list[str] = [
            f"# Evaluation Report — {today}",
            "",
            f"**Decisions evaluated:** {result.decisions_evaluated}",
            f"**Agreement rate:** {result.agreement_rate:.2f}",
            f"**Precision:** {result.precision:.2f}",
            f"**Recall:** {result.recall:.2f}",
            f"**Spearman correlation:** {result.spearman:.2f}",
            "",
            "## Disagreements",
            "",
        ]

        has_disagreement = False
        for d in result.per_decision:
            if not d.agreed:
                has_disagreement = True
                direction = (
                    "pipeline high / human said no"
                    if d.above_threshold
                    else "pipeline low / human said yes"
                )
                lines.append(
                    f"- **{d.job_id}**: verdict={d.verdict}, "
                    f"score={d.final_score:.2f} ({direction})"
                )
        if not has_disagreement:
            lines.append("No disagreements.")

        lines.append("")  # trailing newline
        path.write_text("\n".join(lines), encoding="utf-8")
        return path


class EvalHistory:
    """Appends eval run summaries to a JSONL history file."""

    @staticmethod
    def append(result: EvalResult, history_path: str) -> None:
        """Append one JSON line to the eval history file."""
        hp = Path(history_path)
        hp.parent.mkdir(parents=True, exist_ok=True)

        record = {
            "timestamp": datetime.now(UTC).isoformat(),
            "decisions_evaluated": result.decisions_evaluated,
            "agreement_rate": result.agreement_rate,
            "precision": result.precision,
            "recall": result.recall,
            "spearman": result.spearman,
        }

        with hp.open("a", encoding="utf-8") as f:
            f.write(json.dumps(record) + "\n")


@dataclass
class ModelComparisonResult:
    """Side-by-side results from evaluating two LLM models."""

    model_a: str
    model_b: str
    result_a: EvalResult
    result_b: EvalResult

    @property
    def agreement_delta(self) -> float:
        """result_b.agreement_rate - result_a.agreement_rate."""
        return self.result_b.agreement_rate - self.result_a.agreement_rate

    @property
    def precision_delta(self) -> float:
        """result_b.precision - result_a.precision."""
        return self.result_b.precision - self.result_a.precision

    @property
    def recall_delta(self) -> float:
        """result_b.recall - result_a.recall."""
        return self.result_b.recall - self.result_a.recall

    @property
    def spearman_delta(self) -> float:
        """result_b.spearman - result_a.spearman."""
        return self.result_b.spearman - self.result_a.spearman


class EvalRunner:
    """Re-scores stored decisions and computes agreement metrics."""

    def __init__(
        self,
        scorer: Scorer,
        ranker: Ranker,
        store: VectorStore,
    ) -> None:
        """Initialize with a scorer, ranker, and vector store."""
        self._scorer = scorer
        self._ranker = ranker
        self._store = store

    async def evaluate(self) -> EvalResult:
        """
        Re-score every decision and compute agreement metrics.

        Verdict mapping:
            Human positive: yes, maybe
            Human negative: no
            Pipeline positive: final_score >= ranker.min_score_threshold

        Metrics:
            agreement_rate = agreed / total
            precision = true_positive / pipeline_positive  (0.0 if none)
            recall = true_positive / human_yes_count  (0.0 if none)
        """
        decisions = self._load_decisions()
        if not decisions:
            logger.info("No decisions found — nothing to evaluate.")
            return EvalResult(
                decisions_evaluated=0,
                agreement_rate=0.0,
                precision=0.0,
                recall=0.0,
            )

        per_decision: list[EvalDecision] = []
        agreed_count = 0
        true_positive = 0
        pipeline_positive_count = 0
        human_yes_count = 0
        human_yes_above = 0

        for job_id, verdict, jd_text in decisions:
            score_result = await self._scorer.score(jd_text)
            final_score = self._ranker.compute_final_score(score_result)
            above_threshold = final_score >= self._ranker.min_score_threshold

            human_positive = verdict in ("yes", "maybe")
            pipeline_positive = above_threshold

            agreed = human_positive == pipeline_positive

            if agreed:
                agreed_count += 1
            if pipeline_positive:
                pipeline_positive_count += 1
                if human_positive:
                    true_positive += 1
            if verdict == "yes":
                human_yes_count += 1
                if pipeline_positive:
                    human_yes_above += 1

            per_decision.append(
                EvalDecision(
                    job_id=job_id,
                    verdict=verdict,
                    final_score=final_score,
                    above_threshold=above_threshold,
                    agreed=agreed,
                )
            )

        total = len(decisions)
        agreement_rate = agreed_count / total
        precision = true_positive / pipeline_positive_count if pipeline_positive_count > 0 else 0.0
        recall = human_yes_above / human_yes_count if human_yes_count > 0 else 0.0

        scores = [d.final_score for d in per_decision]
        ordinals = [_VERDICT_ORDINAL.get(d.verdict, 0.0) for d in per_decision]
        spearman = spearman_rank_correlation(scores, ordinals)

        return EvalResult(
            decisions_evaluated=total,
            agreement_rate=agreement_rate,
            precision=precision,
            recall=recall,
            spearman=spearman,
            per_decision=per_decision,
        )

    def _load_decisions(self) -> list[tuple[str, str, str]]:
        """Load all decisions from the store as (job_id, verdict, jd_text) tuples."""
        try:
            collection = self._store.get_or_create_collection("decisions")
        except Exception:
            logger.debug("Decisions collection not accessible.")
            return []

        result = collection.get(include=["documents", "metadatas"])

        ids = result["ids"]
        documents = result["documents"] or []
        metadatas = result["metadatas"] or []

        decisions: list[tuple[str, str, str]] = []
        for i, doc_id in enumerate(ids):
            meta = metadatas[i] if metadatas and i < len(metadatas) else None
            doc = documents[i] if documents and i < len(documents) else None
            if meta is None or doc is None:
                logger.warning("Skipping decision %s — missing metadata or document.", doc_id)
                continue
            job_id = str(meta.get("job_id", doc_id))
            verdict = str(meta.get("verdict", ""))
            if not verdict:
                logger.warning("Skipping decision %s — no verdict.", doc_id)
                continue
            decisions.append((job_id, verdict, doc))

        return decisions
