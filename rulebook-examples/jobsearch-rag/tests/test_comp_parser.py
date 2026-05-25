"""
Compensation parsing tests — extraction, normalization, and source detection.

Maps to BDD spec: TestCompensationParsing

The comp parser extracts salary ranges from JD text via regex, normalizes
hourly rates to annual (x2080), and records the source (employer-stated vs.
board-estimated).  All extraction happens on raw text — no LLM involvement.
"""

from __future__ import annotations

import pytest

from jobsearch_rag.rag.comp_parser import parse_compensation

# ---------------------------------------------------------------------------
# TestCompensationParsing
# ---------------------------------------------------------------------------


class TestCompensationParsing:
    """
    REQUIREMENT: Compensation ranges are extracted from JD text and normalized.

    WHO: The pipeline runner enriching listings after JD extraction;
         the scorer computing comp_score
    WHAT: (1) The parser extracts an annual compensation range of 180000 to 220000 from comma-formatted salary text.
          (2) The parser applies the k suffix as a 1000 multiplier to both endpoints of an annual salary range.
          (3) The parser converts a 95 hourly rate into an annual compensation value of 197600 using 2080 hours.
          (4) The parser converts both endpoints of an hourly pay range into annual compensation using 2080 hours.
          (5) The parser sets both comp_min and comp_max to 200000 when salary text contains a single annual value.
          (6) The parser returns None when the text contains no salary information.
          (7) The parser preserves the original matched salary snippet in comp_text for auditability.
          (8) The parser does not treat an employee count as salary compensation.
          (9) The parser does not treat a revenue figure as salary compensation.
          (10) The parser extracts a compensation range of 150000 to 200000 from a dollar-signed hyphenated range.
          (11) The parser correctly extracts a salary range when the values are joined by the word to.
          (12) The parser recognizes salary text as annual when it uses supported annual suffix variants.
          (13) The parser sets comp_source to employer when no source override is provided.
          (14) The parser sets comp_source to estimated when the caller provides that source override.
          (15) The system prefers the JD-body salary result because it carries the employer comp_source.
          (16) The parser returns a result object with comp_min, comp_max, comp_source, and comp_text fields.
          (17) The parser recognizes the k suffix regardless of whether it appears in uppercase or lowercase.
          (18) The parser correctly extracts a salary range when no spaces surround the hyphen.
          (19) The parser correctly annualizes a decimal hourly rate.
          (20) The parser skips a dollar range in false-positive context and returns None.
          (21) The parser skips a salary range when both values fall below realistic salary bounds and returns None.
          (22) The parser skips a single dollar value when it falls below realistic salary bounds and returns None.
    WHY: Salary data is noisy and inconsistently formatted across boards;
         incorrect parsing would silently distort comp_score rankings —
         a wrong number is worse than a missing one

    MOCK BOUNDARY:
        Mock:  nothing — pure text parsing, no I/O
        Real:  parse_compensation, CompResult
        Never: Patch regex internals or CompResult constructor
    """

    def test_annual_range_with_commas_is_parsed(self) -> None:
        """
        Given JD text with a comma-formatted annual range
        When the text is parsed
        Then comp_min=180000 and comp_max=220000
        """
        # Given: JD text with a comma-formatted annual range
        text = "Compensation: $180,000 - $220,000 per year"

        # When: the text is parsed
        result = parse_compensation(text)

        # Then: both endpoints are extracted correctly
        assert result is not None, f"Expected a CompResult, got None for: {text!r}"
        assert result.comp_min == pytest.approx(180_000), (
            f"Expected comp_min=180000, got {result.comp_min}"
        )
        assert result.comp_max == pytest.approx(220_000), (
            f"Expected comp_max=220000, got {result.comp_max}"
        )

    def test_annual_range_with_k_suffix_is_parsed(self) -> None:
        """
        Given JD text with a k-suffix salary range
        When the text is parsed
        Then the k=1000 multiplier is applied to both endpoints
        """
        # Given: JD text with k-suffix range
        text = "Salary range: $180k-$220k"

        # When: the text is parsed
        result = parse_compensation(text)

        # Then: k multiplier produces 180000 and 220000
        assert result is not None, f"Expected a CompResult, got None for: {text!r}"
        assert result.comp_min == pytest.approx(180_000), (
            f"Expected comp_min=180000, got {result.comp_min}"
        )
        assert result.comp_max == pytest.approx(220_000), (
            f"Expected comp_max=220000, got {result.comp_max}"
        )

    def test_hourly_rate_is_converted_to_annual_via_2080(self) -> None:
        """
        Given JD text with an hourly rate
        When the text is parsed
        Then the rate is annualized as 95 * 2080 = 197600
        """
        # Given: JD text with an hourly rate
        text = "This position pays $95/hr"

        # When: the text is parsed
        result = parse_compensation(text)

        # Then: hourly rate is annualized via x2080
        expected = 95 * 2080
        assert result is not None, f"Expected a CompResult, got None for: {text!r}"
        assert result.comp_min == pytest.approx(expected), (
            f"Expected comp_min={expected}, got {result.comp_min}"
        )
        assert result.comp_max == pytest.approx(expected), (
            f"Expected comp_max={expected}, got {result.comp_max}"
        )

    def test_hourly_range_is_converted_to_annual_via_2080(self) -> None:
        """
        Given JD text with an hourly range
        When the text is parsed
        Then both endpoints are annualized via x2080
        """
        # Given: JD text with an hourly range
        text = "Rate: $85 - $105/hour"

        # When: the text is parsed
        result = parse_compensation(text)

        # Then: both hourly endpoints are annualized
        assert result is not None, f"Expected a CompResult, got None for: {text!r}"
        assert result.comp_min == pytest.approx(85 * 2080), (
            f"Expected comp_min={85 * 2080}, got {result.comp_min}"
        )
        assert result.comp_max == pytest.approx(105 * 2080), (
            f"Expected comp_max={105 * 2080}, got {result.comp_max}"
        )

    def test_single_annual_value_sets_both_min_and_max(self) -> None:
        """
        Given JD text with a single salary value
        When the text is parsed
        Then comp_min and comp_max are both 200000
        """
        # Given: JD text with a single salary value
        text = "Base salary: $200,000"

        # When: the text is parsed
        result = parse_compensation(text)

        # Then: both min and max equal the single value
        assert result is not None, f"Expected a CompResult, got None for: {text!r}"
        assert result.comp_min == pytest.approx(200_000), (
            f"Expected comp_min=200000, got {result.comp_min}"
        )
        assert result.comp_max == pytest.approx(200_000), (
            f"Expected comp_max=200000, got {result.comp_max}"
        )

    def test_no_salary_info_returns_none(self) -> None:
        """
        Given JD text with no salary information
        When the text is parsed
        Then parse_compensation returns None
        """
        # Given: JD text with no salary data
        text = "We are looking for a Staff Architect to lead platform design."

        # When: the text is parsed
        result = parse_compensation(text)

        # Then: None is returned (no comp data extracted)
        assert result is None, f"Expected None for text with no salary info, got {result}"

    def test_comp_text_preserves_original_matched_snippet(self) -> None:
        """
        Given JD text with an explicit salary range
        When the text is parsed
        Then comp_text preserves the original matched text for audit
        """
        # Given: JD text with an explicit salary range
        text = "The salary for this role is $180,000 - $220,000 per year."

        # When: the text is parsed
        result = parse_compensation(text)

        # Then: comp_text contains the original dollar amounts
        assert result is not None, f"Expected a CompResult, got None for: {text!r}"
        assert "$180,000" in result.comp_text, (
            f"Expected '$180,000' in comp_text, got: {result.comp_text!r}"
        )
        assert "$220,000" in result.comp_text, (
            f"Expected '$220,000' in comp_text, got: {result.comp_text!r}"
        )

    def test_employee_count_is_not_mistaken_for_salary(self) -> None:
        """
        Given JD text mentioning employee counts, not salary
        When the text is parsed
        Then the number is not mistaken for salary
        """
        # Given: JD text with employee counts, not salary
        text = "We have 5,000 employees across 10 offices worldwide."

        # When: the text is parsed
        result = parse_compensation(text)

        # Then: no false positive
        assert result is None, f"Employee count '5,000' was falsely parsed as salary: {result}"

    def test_revenue_figures_are_not_mistaken_for_salary(self) -> None:
        """
        Given JD text with revenue figures like '$2.5 billion'
        When the text is parsed
        Then revenue figures are not mistaken for salary
        """
        # Given: JD text with revenue figures
        text = "The company has $2.5 billion in revenue and $500M ARR."

        # When: the text is parsed
        result = parse_compensation(text)

        # Then: no false positive
        assert result is None, f"Revenue figure was falsely parsed as salary: {result}"

    def test_range_with_dollar_sign_and_hyphen_is_parsed(self) -> None:
        """
        Given JD text with a standard dollar-and-hyphen range
        When the text is parsed
        Then comp_min=150000 and comp_max=200000
        """
        # Given: standard dollar-sign-and-hyphen range
        text = "Pay: $150,000 - $200,000"

        # When: the text is parsed
        result = parse_compensation(text)

        # Then: both endpoints extracted
        assert result is not None, f"Expected a CompResult, got None for: {text!r}"
        assert result.comp_min == pytest.approx(150_000), (
            f"Expected comp_min=150000, got {result.comp_min}"
        )
        assert result.comp_max == pytest.approx(200_000), (
            f"Expected comp_max=200000, got {result.comp_max}"
        )

    def test_range_with_to_keyword_is_parsed(self) -> None:
        """
        Given JD text using 'to' instead of '-' between salary values
        When the text is parsed
        Then the range is parsed correctly
        """
        # Given: range using 'to' keyword
        text = "Salary: $150,000 to $200,000 annually"

        # When: the text is parsed
        result = parse_compensation(text)

        # Then: both endpoints extracted
        assert result is not None, f"Expected a CompResult, got None for: {text!r}"
        assert result.comp_min == pytest.approx(150_000), (
            f"Expected comp_min=150000, got {result.comp_min}"
        )
        assert result.comp_max == pytest.approx(200_000), (
            f"Expected comp_max=200000, got {result.comp_max}"
        )

    def test_per_year_and_per_annum_suffixes_are_recognized(self) -> None:
        """
        Given salary text with various annual suffixes
        When the text is parsed with each suffix variant
        Then the salary is recognized as annual
        """
        # Given: the same salary with various annual suffixes
        for suffix in ["/yr", "/year", "per annum", "annually", "per year", "a year"]:
            text = f"Compensation: $200,000 {suffix}"

            # When: the text is parsed
            result = parse_compensation(text)

            # Then: salary is recognized with the given suffix
            assert result is not None, f"Failed to parse salary with suffix '{suffix}'"
            assert result.comp_min == pytest.approx(200_000), (
                f"Expected comp_min=200000 for suffix '{suffix}', got {result.comp_min}"
            )

    def test_comp_source_defaults_to_employer(self) -> None:
        """
        Given JD text with salary and no source parameter
        When the text is parsed with default source
        Then comp_source defaults to 'employer'
        """
        # Given: JD text with salary, no source parameter
        text = "Base salary for this role: $180,000 - $220,000"

        # When: the text is parsed (default source)
        result = parse_compensation(text)

        # Then: comp_source defaults to 'employer'
        assert result is not None, f"Expected a CompResult, got None for: {text!r}"
        assert result.comp_source == "employer", (
            f"Expected comp_source='employer', got {result.comp_source!r}"
        )

    def test_comp_source_is_estimated_when_flagged(self) -> None:
        """
        Given JD text with salary
        When the text is parsed with source='estimated'
        Then comp_source reflects the override
        """
        # Given: JD text parsed with source='estimated'
        text = "Estimated: $180,000 - $220,000"

        # When: the text is parsed with source override
        result = parse_compensation(text, source="estimated")

        # Then: comp_source is 'estimated'
        assert result is not None, f"Expected a CompResult, got None for: {text!r}"
        assert result.comp_source == "estimated", (
            f"Expected comp_source='estimated', got {result.comp_source!r}"
        )

    def test_employer_stated_salary_is_preferred_over_board_estimate(self) -> None:
        """
        Given two parse results from JD body and board estimate
        When both results are compared
        Then the JD-body result has comp_source='employer' for preference
        """
        # Given: two parse results — one employer, one estimated
        jd_result = parse_compensation(
            "Base salary: $200,000 - $250,000 per year", source="employer"
        )
        board_result = parse_compensation(
            "Estimated salary: $170,000 - $200,000", source="estimated"
        )

        # When / Then: source tags distinguish them for pipeline preference
        assert jd_result is not None, "JD-body salary should parse"
        assert board_result is not None, "Board estimate should parse"
        assert jd_result.comp_source == "employer", (
            f"Expected JD result source='employer', got {jd_result.comp_source!r}"
        )
        assert board_result.comp_source == "estimated", (
            f"Expected board result source='estimated', got {board_result.comp_source!r}"
        )

    def test_result_dataclass_has_expected_fields(self) -> None:
        """
        Given JD text that produces a valid CompResult
        When the result fields are inspected
        Then it has comp_min, comp_max, comp_source, and comp_text fields
        """
        # Given: JD text that produces a valid result
        text = "Salary: $200,000"

        # When: the text is parsed
        result = parse_compensation(text)

        # Then: all expected fields are present
        assert result is not None, f"Expected a CompResult, got None for: {text!r}"
        for field in ("comp_min", "comp_max", "comp_source", "comp_text"):
            assert hasattr(result, field), (
                f"CompResult missing expected field '{field}'. "
                f"Available: {[a for a in dir(result) if not a.startswith('_')]}"
            )

    def test_k_suffix_case_insensitive(self) -> None:
        """
        Given salary values with uppercase and lowercase k suffix
        When the text is parsed
        Then the k suffix is recognized regardless of case
        """
        # Given: salary values with uppercase and lowercase k
        for variant in ["$200K", "$200k"]:
            # When: the text is parsed
            result = parse_compensation(f"Salary: {variant}")

            # Then: k multiplier is applied
            assert result is not None, f"Expected a CompResult for '{variant}', got None"
            assert result.comp_min == pytest.approx(200_000), (
                f"Expected comp_min=200000 for '{variant}', got {result.comp_min}"
            )

    def test_range_with_no_space_around_hyphen(self) -> None:
        """
        Given JD text with no whitespace around the range hyphen
        When the text is parsed
        Then the range is parsed correctly
        """
        # Given: range with no whitespace around the hyphen
        text = "Salary: $180,000-$220,000"

        # When: the text is parsed
        result = parse_compensation(text)

        # Then: both endpoints extracted
        assert result is not None, f"Expected a CompResult, got None for: {text!r}"
        assert result.comp_min == pytest.approx(180_000), (
            f"Expected comp_min=180000, got {result.comp_min}"
        )
        assert result.comp_max == pytest.approx(220_000), (
            f"Expected comp_max=220000, got {result.comp_max}"
        )

    def test_decimal_hourly_rate(self) -> None:
        """
        Given JD text with a decimal hourly rate
        When the text is parsed
        Then the decimal rate is annualized correctly
        """
        # Given: hourly rate with decimal
        text = "Rate: $95.50/hr"

        # When: the text is parsed
        result = parse_compensation(text)

        # Then: decimal hourly rate is annualized
        expected = 95.50 * 2080
        assert result is not None, f"Expected a CompResult, got None for: {text!r}"
        assert result.comp_min == pytest.approx(expected), (
            f"Expected comp_min={expected}, got {result.comp_min}"
        )

    def test_range_in_false_positive_context_is_skipped(self) -> None:
        """
        Given a dollar range surrounded by false-positive context words
        When parse_compensation is called
        Then the range match is skipped and None is returned
        """
        # Given: a dollar range in a funding context (not salary)
        text = "Our company raised $100,000 - $200,000 in funding last quarter."

        # When: the text is parsed
        result = parse_compensation(text)

        # Then: the false-positive context is recognized and skipped
        assert result is None, f"Funding context was falsely parsed as salary: {result}"

    def test_range_outside_salary_bounds_is_skipped(self) -> None:
        """
        Given a dollar range where both values are below realistic salary thresholds
        When parse_compensation is called
        Then the match is skipped and None is returned
        """
        # Given: a dollar range too low to be a salary
        text = "The fee is $3 - $5 per widget."

        # When: the text is parsed
        result = parse_compensation(text)

        # Then: below-threshold values are rejected
        assert result is None, (
            f"Below-threshold range '$3-$5' was falsely parsed as salary: {result}"
        )

    def test_single_value_outside_salary_bounds_is_skipped(self) -> None:
        """
        Given a single dollar value below realistic salary thresholds
        When parse_compensation is called
        Then the match is skipped and None is returned
        """
        # Given: a single dollar value too low to be a salary
        text = "The price is $5 per unit."

        # When: the text is parsed
        result = parse_compensation(text)

        # Then: below-threshold value is rejected
        assert result is None, f"Below-threshold value '$5' was falsely parsed as salary: {result}"
