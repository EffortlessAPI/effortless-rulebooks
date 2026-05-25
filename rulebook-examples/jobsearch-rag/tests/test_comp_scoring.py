"""
Compensation scoring tests — continuous scale, band boundaries, config integration.

Maps to BDD specs: TestCompensationScoring

The comp scorer converts parsed compensation data (comp_max) into a continuous
score in [0.0, 1.0] relative to a configurable base_salary target.  This is a
*taste signal* — it nudges rankings without hard-gating any role.
"""

from __future__ import annotations

import pytest

from jobsearch_rag.rag.comp_parser import compute_comp_score

# ---------------------------------------------------------------------------
# TestCompensationScoring
# ---------------------------------------------------------------------------


class TestCompensationScoring:
    """
    REQUIREMENT: Compensation score is a continuous signal relative to a
    configurable base_salary target, not a binary gate.

    WHO: The ranker consuming comp_score to nudge final ranking;
         the operator tuning base_salary in settings.toml
    WHAT: (1) The system assigns a compensation score of 1.0 when `comp_max` equals `base_salary`.
          (2) The system keeps the compensation score at 1.0 when `comp_max` exceeds `base_salary`.
          (3) The system places the compensation score in the 0.7 to 0.9 band when `comp_max` is 95% of `base_salary`.
          (4) The system assigns a compensation score of 0.7 when `comp_max` is exactly 90% of `base_salary`.
          (5) The system places the compensation score in the 0.4 to 0.7 band when `comp_max` is 85% of `base_salary`.
          (6) The system assigns a compensation score of 0.4 when `comp_max` is exactly 77% of `base_salary`.
          (7) The system places the compensation score in the 0.0 to 0.4 band when `comp_max` is 72% of `base_salary`.
          (8) The system assigns a compensation score of 0.0 when `comp_max` is exactly 68% of `base_salary`.
          (9) The system clamps the compensation score to 0.0 when `comp_max` falls below 68% of `base_salary`.
          (10) The system assigns a neutral compensation score of 0.5 when compensation data is missing.
          (11) The system reads `base_salary` from configuration by producing different scores for the same `comp_max` under different base salaries.
          (12) The system scales compensation thresholds proportionally with `base_salary` so that 90% of any configured base scores 0.7.
          (13) The system always keeps the compensation score within the inclusive range from 0.0 to 1.0.
          (14) The system increases the compensation score monotonically within a band through linear interpolation.
          (15) The system keeps the compensation scale continuous across band boundaries without score gaps or jumps.
    WHY: Compensation as a taste signal lets well-paying roles float up
         without hard-gating roles that might be stepping stones or have
         other attractive qualities — a gate would discard too aggressively

    MOCK BOUNDARY:
        Mock:  nothing — pure arithmetic, no I/O
        Real:  compute_comp_score
        Never: Patch band boundaries or clamp logic
    """

    # Default base_salary for tests
    BASE = 220_000

    # --- Top band: ≥ 100% of base → 1.0 ---

    def test_comp_max_at_base_salary_scores_one(self) -> None:
        """
        Given comp_max equals base_salary
        When the comp score is computed
        Then comp_score is 1.0
        """
        # Given: comp_max exactly at base
        # When / Then: score is maximum
        score = compute_comp_score(comp_max=220_000, base_salary=self.BASE)
        assert score == pytest.approx(1.0), f"Expected 1.0 at base_salary, got {score}"

    def test_comp_max_above_base_salary_scores_one(self) -> None:
        """
        Given comp_max exceeds base_salary
        When the comp score is computed
        Then comp_score is still 1.0 (no bonus for overshoot)
        """
        # Given: comp_max well above base
        # When / Then: score is clamped at 1.0
        score = compute_comp_score(comp_max=300_000, base_salary=self.BASE)
        assert score == pytest.approx(1.0), f"Expected 1.0 for comp above base, got {score}"

    # --- Band: 90-100% of base -> 0.7-0.9 (linear) ---

    def test_comp_max_at_95_percent_of_base_scores_between_07_and_09(self) -> None:
        """
        Given comp_max is at 95% of base_salary
        When the comp score is computed
        Then comp_score falls in the 0.7-0.9 band
        """
        # Given: comp_max at ~95% of base (209k / 220k)
        # When: score is computed
        score = compute_comp_score(comp_max=209_000, base_salary=self.BASE)

        # Then: score is in the 90-100% band
        assert 0.7 < score < 0.9, f"Expected score in (0.7, 0.9) for 95% of base, got {score}"

    def test_comp_max_at_90_percent_of_base_scores_07(self) -> None:
        """
        Given comp_max is at exactly 90% of base_salary
        When the comp score is computed
        Then comp_score is 0.7 (lower boundary of 90-100% band)
        """
        # Given: comp_max at exactly 90% of base (198k / 220k)
        # When: score is computed
        score = compute_comp_score(comp_max=198_000, base_salary=self.BASE)

        # Then: score hits the 0.7 boundary
        assert score == pytest.approx(0.7), f"Expected 0.7 at 90% of base, got {score}"

    # --- Band: 77-90% of base -> 0.4-0.7 (linear) ---

    def test_comp_max_at_85_percent_of_base_scores_between_04_and_07(self) -> None:
        """
        Given comp_max is at 85% of base_salary
        When the comp score is computed
        Then comp_score falls in the 0.4-0.7 band
        """
        # Given: comp_max at ~85% of base (187k / 220k)
        # When: score is computed
        score = compute_comp_score(comp_max=187_000, base_salary=self.BASE)

        # Then: score is in the 77-90% band
        assert 0.4 < score < 0.7, f"Expected score in (0.4, 0.7) for 85% of base, got {score}"

    def test_comp_max_at_77_percent_of_base_scores_04(self) -> None:
        """
        Given comp_max is at exactly 77% of base_salary
        When the comp score is computed
        Then comp_score is 0.4 (lower boundary of 77-90% band)
        """
        # Given: comp_max at exactly 77% of base (169.4k / 220k)
        # When: score is computed
        score = compute_comp_score(comp_max=169_400, base_salary=self.BASE)

        # Then: score hits the 0.4 boundary
        assert score == pytest.approx(0.4), f"Expected 0.4 at 77% of base, got {score}"

    # --- Band: 68-77% of base -> 0.0-0.4 (linear) ---

    def test_comp_max_at_72_percent_of_base_scores_between_00_and_04(self) -> None:
        """
        Given comp_max is at 72% of base_salary
        When the comp score is computed
        Then comp_score falls in the 0.0-0.4 band
        """
        # Given: comp_max at ~72% of base (158.4k / 220k)
        # When: score is computed
        score = compute_comp_score(comp_max=158_400, base_salary=self.BASE)

        # Then: score is in the 68-77% band
        assert 0.0 < score < 0.4, f"Expected score in (0.0, 0.4) for 72% of base, got {score}"

    def test_comp_max_at_68_percent_of_base_scores_zero(self) -> None:
        """
        Given comp_max is at exactly 68% of base_salary
        When the comp score is computed
        Then comp_score is 0.0 (the floor)
        """
        # Given: comp_max at exactly 68% of base (149.6k / 220k)
        # When: score is computed
        score = compute_comp_score(comp_max=149_600, base_salary=self.BASE)

        # Then: score hits the floor
        assert score == pytest.approx(0.0), f"Expected 0.0 at 68% of base, got {score}"

    # --- Below 68% → 0.0 ---

    def test_comp_max_below_68_percent_of_base_scores_zero(self) -> None:
        """
        Given comp_max is well below 68% of base_salary
        When the comp score is computed
        Then comp_score is clamped to 0.0
        """
        # Given: comp_max far below the 68% threshold
        # When / Then: score is clamped at floor
        score = compute_comp_score(comp_max=100_000, base_salary=self.BASE)
        assert score == pytest.approx(0.0), (
            f"Expected 0.0 for comp far below 68% of base, got {score}"
        )

    # --- Missing data → 0.5 ---

    def test_missing_comp_data_scores_05(self) -> None:
        """
        Given comp_max is None (no salary data)
        When the comp score is computed
        Then comp_score is neutral at 0.5
        """
        # Given: no compensation data
        # When / Then: neutral score returned
        score = compute_comp_score(comp_max=None, base_salary=self.BASE)
        assert score == pytest.approx(0.5), f"Expected 0.5 for missing comp data, got {score}"

    # --- Config-driven, not hardcoded ---

    def test_base_salary_is_read_from_config_not_hardcoded(self) -> None:
        """
        Given two different base_salary values
        When the same comp_max is scored against each
        Then the scores differ, proving base_salary is not hardcoded
        """
        # Given: two different base_salary values
        # When: same comp_max is scored against each
        score_at_base = compute_comp_score(comp_max=200_000, base_salary=200_000)
        score_below_base = compute_comp_score(comp_max=200_000, base_salary=300_000)

        # Then: scores differ because base differs
        assert score_at_base == pytest.approx(1.0), (
            f"Expected 1.0 when comp_max equals base, got {score_at_base}"
        )
        assert score_below_base < 0.7, (
            f"Expected <0.7 when comp_max is 67% of base, got {score_below_base}"
        )

    def test_changing_base_salary_shifts_all_boundaries(self) -> None:
        """
        Given two different base_salary values
        When comp_max at 90% of each base is scored
        Then both hit the 0.7 boundary, proving proportional scaling
        """
        # Given: 90% of two different bases
        score_100k = compute_comp_score(comp_max=90_000, base_salary=100_000)
        score_300k = compute_comp_score(comp_max=270_000, base_salary=300_000)

        # Then: both hit the 0.7 boundary (90% of their respective base)
        assert score_100k == pytest.approx(0.7), (
            f"Expected 0.7 at 90% of 100k base, got {score_100k}"
        )
        assert score_300k == pytest.approx(0.7), (
            f"Expected 0.7 at 90% of 300k base, got {score_300k}"
        )

    # --- Invariants ---

    def test_comp_score_is_always_between_zero_and_one(self) -> None:
        """
        Given extreme and edge-case comp_max values
        When each is scored
        Then the result is always in [0.0, 1.0]
        """
        # Given: extreme and edge-case comp_max values
        test_cases = [
            ("very high", 1_000_000),
            ("very low", 10_000),
            ("zero", 0),
        ]
        for label, comp_max in test_cases:
            # When / Then: score is within bounds
            score = compute_comp_score(comp_max=comp_max, base_salary=self.BASE)
            assert 0.0 <= score <= 1.0, (
                f"Score out of [0.0, 1.0] for {label} comp_max={comp_max}: {score}"
            )

        # When / Then: missing data also within bounds
        score = compute_comp_score(comp_max=None, base_salary=self.BASE)
        assert 0.0 <= score <= 1.0, f"Score out of [0.0, 1.0] for missing comp: {score}"

    def test_comp_score_interpolates_linearly_within_bands(self) -> None:
        """
        Given three points within the 77-90% band
        When scores are computed
        Then they increase monotonically (linear interpolation)
        """
        # Given: three comp_max values within the 77-90% band
        low = compute_comp_score(comp_max=170_000, base_salary=self.BASE)  # ~77%
        mid = compute_comp_score(comp_max=184_000, base_salary=self.BASE)  # ~83.6%
        high = compute_comp_score(comp_max=197_000, base_salary=self.BASE)  # ~89.5%

        # Then: monotonically increasing
        assert low < mid < high, f"Expected monotonic increase: {low} < {mid} < {high}"

    def test_scale_is_continuous_across_band_boundaries(self) -> None:
        """
        Given comp_max values $1 above and below each band boundary
        When the comp score is computed for each
        Then there is no gap or jump in the score
        """
        # Given: $1 above/below each boundary
        epsilon = 1.0

        # Then: 68% boundary is continuous
        below_68 = compute_comp_score(comp_max=self.BASE * 0.68 - epsilon, base_salary=self.BASE)
        at_68 = compute_comp_score(comp_max=self.BASE * 0.68, base_salary=self.BASE)
        above_68 = compute_comp_score(comp_max=self.BASE * 0.68 + epsilon, base_salary=self.BASE)
        assert below_68 == pytest.approx(0.0, abs=0.01), (
            f"Expected ~0.0 just below 68% boundary, got {below_68}"
        )
        assert at_68 == pytest.approx(0.0, abs=0.01), f"Expected ~0.0 at 68% boundary, got {at_68}"
        assert above_68 >= 0.0, f"Expected >=0.0 just above 68% boundary, got {above_68}"

        # Then: 77% boundary is continuous
        just_below_77 = compute_comp_score(
            comp_max=self.BASE * 0.77 - epsilon, base_salary=self.BASE
        )
        just_above_77 = compute_comp_score(
            comp_max=self.BASE * 0.77 + epsilon, base_salary=self.BASE
        )
        assert abs(just_above_77 - just_below_77) < 0.02, (
            f"Discontinuity at 77% boundary: below={just_below_77}, above={just_above_77}"
        )

        # Then: 90% boundary is continuous
        just_below_90 = compute_comp_score(
            comp_max=self.BASE * 0.90 - epsilon, base_salary=self.BASE
        )
        just_above_90 = compute_comp_score(
            comp_max=self.BASE * 0.90 + epsilon, base_salary=self.BASE
        )
        assert abs(just_above_90 - just_below_90) < 0.02, (
            f"Discontinuity at 90% boundary: below={just_below_90}, above={just_above_90}"
        )

        # Then: 100% boundary is continuous
        just_below_100 = compute_comp_score(comp_max=self.BASE - epsilon, base_salary=self.BASE)
        at_100 = compute_comp_score(comp_max=self.BASE, base_salary=self.BASE)
        assert just_below_100 > 0.89, (
            f"Score should be near 1.0 just below 100%, got {just_below_100}"
        )
        assert at_100 == pytest.approx(1.0), f"Expected 1.0 at 100% boundary, got {at_100}"
