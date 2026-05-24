"""
Tests for :mod:`jobsearch_rag.text` — shared text-processing utilities.

Covers :class:`TestSlugify`.
"""

from __future__ import annotations

from jobsearch_rag.text import MAX_SLUG_LEN, slugify


class TestSlugify:
    """
    REQUIREMENT: Text is converted to filesystem-safe slugs consistently
    across CLI, export, and pipeline layers.

    WHO: Any module that builds file paths from human-readable text
         (JD exporter, review session, CLI read_jd_text)
    WHAT: (1) The system converts a mixed-case title into a lowercase slug with spaces replaced by hyphens.
          (2) The system removes non-alphanumeric characters other than hyphens when slugifying text with punctuation.
          (3) The system replaces underscores with hyphens during slugification.
          (4) The system collapses consecutive whitespace into a single hyphen when slugifying text.
          (5) The system strips leading and trailing hyphens from a slugified result.
          (6) The system truncates a slugified result to the default maximum length when it exceeds that limit.
          (7) The system truncates a slugified result to a provided custom maximum length instead of the default limit.
          (8) The system returns an empty string when slugifying an empty string.
          (9) The system preserves an already valid slug apart from lowercasing it.
          (10) The system produces a filename that matches the `NNN_company_title.md` job-description naming convention when slugified company and title values are assembled into that pattern.
    WHY: Inconsistent slugification across modules would cause file-not-found
         errors when one module writes a JD file and another tries to open it

    MOCK BOUNDARY:
        Mock:  nothing — this class tests pure computation
        Real:  slugify function, MAX_SLUG_LEN constant
        Never: Patch slugify internals — call the function directly
    """

    def test_basic_title_is_lowercased_and_hyphenated(self) -> None:
        """
        Given a mixed-case title with spaces
        When the title is slugified
        Then the result is lowercase with hyphens replacing spaces
        """
        # Given: a mixed-case title with spaces

        # When: the title is slugified
        result = slugify("Senior Staff Engineer")

        # Then: the result is lowercase with hyphens
        assert result == "senior-staff-engineer", (
            f"Expected lowercase hyphenated slug, got {result!r}"
        )

    def test_special_characters_are_stripped(self) -> None:
        """
        Given text containing parentheses, em-dashes, and other punctuation
        When the text is slugified
        Then non-alphanumeric characters (except hyphens) are removed
        """
        # Given: text containing parentheses, em-dashes, and punctuation

        # When: the text is slugified
        result = slugify("Senior Staff Engineer — Platform (Remote)")

        # Then: special characters are stripped
        assert result == "senior-staff-engineer-platform-remote", (
            f"Expected special chars stripped, got {result!r}"
        )

    def test_underscores_collapse_to_single_hyphen(self) -> None:
        """
        Given text containing underscores
        When the text is slugified
        Then they are replaced with hyphens like whitespace
        """
        # Given: text containing underscores

        # When: the text is slugified
        result = slugify("some_company_name")

        # Then: underscores become hyphens
        assert result == "some-company-name", f"Expected underscores→hyphens, got {result!r}"

    def test_consecutive_whitespace_collapses_to_single_hyphen(self) -> None:
        """
        Given text with multiple consecutive spaces
        When the text is slugified
        Then the run collapses to a single hyphen
        """
        # Given: text with multiple consecutive spaces

        # When: the text is slugified
        result = slugify("too   many   spaces")

        # Then: consecutive whitespace collapses to a single hyphen
        assert result == "too-many-spaces", f"Expected collapsed whitespace, got {result!r}"

    def test_leading_and_trailing_hyphens_are_stripped(self) -> None:
        """
        Given text that would produce leading or trailing hyphens
        When the text is slugified
        Then those hyphens are removed
        """
        # Given: text that produces leading and trailing hyphens

        # When: the text is slugified
        result = slugify("--leading and trailing--")

        # Then: edge hyphens are stripped
        assert result == "leading-and-trailing", f"Expected stripped edges, got {result!r}"

    def test_truncation_at_default_max_length(self) -> None:
        """
        Given text that exceeds MAX_SLUG_LEN after slugification
        When the text is slugified
        Then the result is truncated to MAX_SLUG_LEN
        """
        # Given: text that exceeds MAX_SLUG_LEN after slugification
        long_text = "a " * (MAX_SLUG_LEN + 10)

        # When: the text is slugified
        result = slugify(long_text)

        # Then: the result is truncated to MAX_SLUG_LEN
        assert len(result) <= MAX_SLUG_LEN, f"Expected len ≤ {MAX_SLUG_LEN}, got {len(result)}"

    def test_custom_max_len_overrides_default(self) -> None:
        """
        Given a long slug and a custom max_len of 10
        When slugified with the custom limit
        Then the result is truncated to that length instead of the default
        """
        # Given: a long slug and a custom max_len of 10

        # When: slugified with the custom limit
        result = slugify("this-is-a-long-slug-that-should-be-truncated", max_len=10)

        # Then: the result respects the custom limit
        assert len(result) <= 10, f"Expected len ≤ 10, got {len(result)}"
        assert result == "this-is-a-", f"Expected first 10 chars, got {result!r}"

    def test_empty_string_returns_empty(self) -> None:
        """
        Given an empty string
        When the string is slugified
        Then the result is an empty string
        """
        # Given: an empty string

        # When: slugified
        result = slugify("")

        # Then: the result is empty
        assert result == "", f"Expected empty string, got {result!r}"

    def test_already_clean_slug_is_unchanged(self) -> None:
        """
        Given text that is already a valid slug
        When the text is slugified
        Then the result is identical to the input (lowercased)
        """
        # Given: text that is already a valid slug

        # When: slugified
        result = slugify("already-clean")

        # Then: the result is unchanged
        assert result == "already-clean", f"Expected no change, got {result!r}"

    def test_company_and_title_round_trip_matches_jd_filename_convention(self) -> None:
        """
        Given realistic company and title values from a job listing
        When company and title are slugified and assembled into the JD filename pattern
        Then the result matches the NNN_company_title.md convention
        """
        # Given: realistic values from a job listing
        company = "Acme Corp."
        title = "Staff Engineer — Platform"
        rank = 7

        # When: company and title are slugified into the JD filename pattern
        filename = f"{rank:03d}_{slugify(company)}_{slugify(title)}.md"

        # Then: the result matches the NNN_company_title.md convention
        assert filename == "007_acme-corp_staff-engineer-platform.md", (
            f"Expected JD filename convention, got {filename!r}"
        )
