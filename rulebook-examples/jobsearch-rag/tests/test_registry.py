"""
Adapter registration and IoC contract tests.

Spec classes:
    TestAdapterRegistration — self-registration and discovery by board name
    TestAdapterContract — interface conformance for polymorphic adapter use
    TestJobListingDataContract — canonical data contract across all boards
    TestStubAdapterContract — behavioral specs for planned adapters (xfail until delivered)
"""

from __future__ import annotations

import asyncio
import typing
from typing import TYPE_CHECKING
from unittest.mock import MagicMock

if TYPE_CHECKING:
    from collections.abc import Iterator

import pytest

from jobsearch_rag.adapters.base import JobBoardAdapter, JobListing
from jobsearch_rag.adapters.indeed import IndeedAdapter
from jobsearch_rag.adapters.linkedin import LinkedInAdapter
from jobsearch_rag.adapters.registry import AdapterRegistry
from jobsearch_rag.adapters.weworkremotely import WeWorkRemotelyAdapter

# ---------------------------------------------------------------------------
# Helpers
# ---------------------------------------------------------------------------


def _make_adapter_class(name: str) -> type[JobBoardAdapter]:
    """Dynamically create a concrete adapter class for testing."""

    class _TestAdapter(JobBoardAdapter):
        @property
        def board_name(self) -> str:
            return name

        async def authenticate(self, page: object) -> None:
            pass

        async def search(
            self,
            page: object,
            query: str,
            max_pages: int = 3,
        ) -> list[JobListing]:
            return []

        async def extract_detail(
            self,
            page: object,
            listing: JobListing,
        ) -> JobListing:
            listing.full_text = "Extracted detail text for testing."
            return listing

    # Give each dynamic class a unique name for clarity
    _TestAdapter.__name__ = f"TestAdapter_{name}"
    _TestAdapter.__qualname__ = f"TestAdapter_{name}"
    return _TestAdapter


def _make_listing(
    board: str = "test-board",
    external_id: str = "test-001",
    full_text: str = "",
) -> JobListing:
    """Create a minimal JobListing for testing."""
    return JobListing(
        board=board,
        external_id=external_id,
        title="Staff Platform Architect",
        company="Acme Corp",
        location="Remote (USA)",
        url="https://example.org/job/test-001",
        full_text=full_text,
    )


@pytest.fixture(autouse=True)
def _clean_registry() -> Iterator[None]:  # pyright: ignore[reportUnusedFunction]
    """Reset the registry before each test, restoring original state after."""
    saved = dict(AdapterRegistry._registry)  # pyright: ignore[reportPrivateUsage] # Tests verify internal state (_registry)
    AdapterRegistry._registry.clear()  # pyright: ignore[reportPrivateUsage] # Tests verify internal state (_registry)
    yield
    AdapterRegistry._registry.clear()  # pyright: ignore[reportPrivateUsage] # Tests verify internal state (_registry)
    AdapterRegistry._registry.update(saved)  # pyright: ignore[reportPrivateUsage] # Tests verify internal state (_registry)


# ---------------------------------------------------------------------------
# TestAdapterRegistration
# ---------------------------------------------------------------------------


class TestAdapterRegistration:
    """
    REQUIREMENT: Adapters self-register and are discoverable by board name.

    WHO: The pipeline runner loading adapters from settings.toml
    WHAT: (1) The registry returns a ready-to-use adapter instance when the system looks up a registered board name.
          (2) The registry raises a ValueError that identifies the missing board name when the system requests an unregistered board name.
          (3) The registry returns all registered board names when the system lists registered adapters.
          (4) The registry replaces the previously registered adapter class when the system registers a new class under the same board name.
          (5) The registry returns the original class unchanged when the system passes an adapter class through register().
    WHY: The runner must not know concrete adapter classes — IoC requires
         that board name is the only coupling between config and implementation

    MOCK BOUNDARY:
        Mock: nothing — pure registration logic
        Real: AdapterRegistry.register / get / list_registered
        Never: Patch registry internals or _registry dict
    """

    def test_registered_adapter_is_retrievable_by_board_name(self) -> None:
        """
        GIVEN a registered adapter class
        WHEN looked up by its board name string
        THEN a ready-to-use adapter instance is returned.
        """
        # Given: register an adapter
        adapter_cls = _make_adapter_class("ziprecruiter")
        AdapterRegistry.register(adapter_cls)

        # When: look up by board name
        adapter = AdapterRegistry.get("ziprecruiter")

        # Then: returns a working adapter instance
        assert isinstance(adapter, JobBoardAdapter), "Should return a JobBoardAdapter instance"
        assert adapter.board_name == "ziprecruiter", "Board name should match"

    def test_retrieving_unregistered_board_name_raises_value_error_with_name(self) -> None:
        """
        GIVEN no adapter registered for a board name
        WHEN get() is called for that name
        THEN ValueError is raised identifying the missing name.
        """
        # When/Then: unregistered name raises ValueError
        with pytest.raises(ValueError, match="no-such-board"):
            AdapterRegistry.get("no-such-board")

    def test_registry_lists_all_registered_board_names(self) -> None:
        """
        GIVEN multiple registered adapters
        WHEN list_registered() is called
        THEN all board names are returned.
        """
        # Given: register four adapters
        for name in ("ziprecruiter", "indeed", "weworkremotely", "linkedin"):
            AdapterRegistry.register(_make_adapter_class(name))

        # When: list registered names
        registered = AdapterRegistry.list_registered()

        # Then: all four names present
        assert set(registered) == {
            "ziprecruiter",
            "indeed",
            "weworkremotely",
            "linkedin",
        }, "Should list all registered board names"

    def test_duplicate_registration_overwrites_previous(self) -> None:
        """
        GIVEN an adapter already registered for a board name
        WHEN a second class is registered for the same name
        THEN the new class replaces the old one.
        """
        # Given: two distinct classes for the same board
        cls_v1 = _make_adapter_class("ziprecruiter")
        cls_v2 = _make_adapter_class("ziprecruiter")
        assert cls_v1 is not cls_v2, "Precondition: cls_v1 and cls_v2 must be distinct classes"

        # When: register both
        AdapterRegistry.register(cls_v1)
        AdapterRegistry.register(cls_v2)

        # Then: v2 wins
        adapter = AdapterRegistry.get("ziprecruiter")
        assert type(adapter) is cls_v2, "Should instantiate the latest registered class"
        assert type(adapter) is not cls_v1, "Old class should be replaced"

    def test_adapter_decorator_does_not_alter_class_interface(self) -> None:
        """
        GIVEN an adapter class
        WHEN passed through register()
        THEN the original class is returned unchanged.
        """
        # Given: a concrete adapter class
        cls = _make_adapter_class("ziprecruiter")

        # When: register it
        returned = AdapterRegistry.register(cls)

        # Then: same object returned
        assert returned is cls, "register() should return the original class"


# ---------------------------------------------------------------------------
# TestAdapterContract
# ---------------------------------------------------------------------------


class TestAdapterContract:
    """
    REQUIREMENT: All adapters conform to the JobBoardAdapter interface.

    WHO: The pipeline runner invoking adapters polymorphically
    WHAT: (1) The adapter returns a non-empty string for board_name.
          (2) The adapter returns a rate_limit_seconds tuple containing exactly two floats.
          (3) The adapter returns rate_limit_seconds with a lower bound that is strictly less than the upper bound.
          (4) The adapter returns a list from search(), including when no listings are found.
          (5) The adapter populates listing.full_text with content during extract_detail().
          (6) The adapter returns the same listing object from extract_detail() after enriching it in place.
    WHY: The runner calls adapters without knowing their type;
         any deviation from the contract breaks the pipeline silently

    MOCK BOUNDARY:
        Mock: Playwright page (MagicMock) — browser I/O boundary
        Real: JobBoardAdapter implementations, all interface methods
        Never: Patch adapter internals
    """

    def test_board_name_returns_non_empty_string(self) -> None:
        """
        GIVEN a concrete adapter instance
        WHEN board_name is accessed
        THEN a non-empty string is returned.
        """
        # Given: an adapter instance
        cls = _make_adapter_class("ziprecruiter")
        adapter = cls()

        # Then: board_name is a non-empty string
        assert isinstance(adapter.board_name, str), "board_name should be a string"
        assert len(adapter.board_name) > 0, "board_name should not be empty"

    def test_rate_limit_seconds_returns_tuple_of_two_floats(self) -> None:
        """
        GIVEN a concrete adapter instance
        WHEN rate_limit_seconds is accessed
        THEN a (min, max) tuple of two floats is returned.
        """
        # Given: an adapter instance
        cls = _make_adapter_class("test-board")
        adapter = cls()

        # When: access rate limit
        rate_limit = adapter.rate_limit_seconds

        # Then: tuple of two floats
        assert isinstance(rate_limit, tuple), "Should return a tuple"
        assert len(rate_limit) == 2, "Tuple should have exactly 2 elements"
        assert all(isinstance(v, float) for v in rate_limit), "Both elements should be floats"

    def test_rate_limit_min_is_less_than_max(self) -> None:
        """
        GIVEN a concrete adapter instance
        WHEN rate_limit_seconds is accessed
        THEN the lower bound is strictly less than the upper bound.
        """
        # Given: an adapter instance
        cls = _make_adapter_class("test-board")
        adapter = cls()

        # When: unpack rate limits
        lo, hi = adapter.rate_limit_seconds

        # Then: min < max
        assert lo < hi, f"Lower bound {lo} should be less than upper bound {hi}"

    def test_search_returns_list_of_job_listings(self) -> None:
        """
        GIVEN a concrete adapter and a mock Playwright page
        WHEN search() is called
        THEN a list is returned (possibly empty).
        """
        # Given: adapter + mock page
        cls = _make_adapter_class("test-board")
        adapter = cls()
        page = MagicMock()

        # When: run search
        result = asyncio.run(adapter.search(page, "staff architect"))

        # Then: result is a list
        assert isinstance(result, list), "search() should return a list"

    def test_extract_detail_populates_full_text_on_listing(self) -> None:
        """
        GIVEN an adapter and a listing with empty full_text
        WHEN extract_detail() is called
        THEN full_text is populated with content.
        """
        # Given: adapter + listing without full_text
        cls = _make_adapter_class("test-board")
        adapter = cls()
        listing = _make_listing(board="test-board")
        page = MagicMock()

        # When: extract detail
        result = asyncio.run(adapter.extract_detail(page, listing))

        # Then: full_text is populated
        assert result.full_text != "", "full_text should be populated after extraction"

    def test_extract_detail_returns_same_listing_object_enriched(self) -> None:
        """
        GIVEN an adapter and a listing object
        WHEN extract_detail() is called
        THEN the same object is returned (mutated in place).
        """
        # Given: adapter + listing
        cls = _make_adapter_class("test-board")
        adapter = cls()
        listing = _make_listing(board="test-board")
        page = MagicMock()

        # When: extract detail
        result = asyncio.run(adapter.extract_detail(page, listing))

        # Then: same object returned
        assert result is listing, "Should return the same listing object, not a copy"


# ---------------------------------------------------------------------------
# TestJobListingDataContract
# ---------------------------------------------------------------------------


class TestJobListingDataContract:
    """
    REQUIREMENT: JobListing is the canonical data contract across all boards.

    WHO: The RAG scorer, ranker, and exporter consuming listings
    WHAT: (1) The system preserves all required listing fields as non-empty strings after extraction.
          (2) The system produces full_text as a non-empty string ready for embedding after detail extraction.
          (3) The system sets the listing board field to match the adapter's board_name property.
          (4) The system assigns distinct external_id values to different listings within the same board.
          (5) The system defaults posted_at to None when it is missing without raising an error.
          (6) The system defaults metadata to an empty dictionary rather than None when it is missing.
    WHY: Downstream components must not branch on board type —
         the listing is the abstraction that makes them board-agnostic

    MOCK BOUNDARY:
        Mock: Playwright page (MagicMock) — browser I/O boundary
        Real: JobListing construction, field defaults, extract_detail
        Never: Patch dataclass fields
    """

    def test_required_fields_are_present_after_extraction(self) -> None:
        """
        GIVEN a listing constructed with all required fields
        WHEN fields are accessed
        THEN all are non-empty strings.
        """
        # Given: a fully-populated listing
        listing = _make_listing(board="ziprecruiter", full_text="Some JD text")

        # Then: all required fields present
        assert listing.board == "ziprecruiter", "board should match"
        assert listing.external_id == "test-001", "external_id should match"
        assert listing.title != "", "title should be non-empty"
        assert listing.company != "", "company should be non-empty"
        assert listing.location != "", "location should be non-empty"
        assert listing.url != "", "url should be non-empty"
        assert listing.full_text != "", "full_text should be non-empty"

    def test_full_text_is_non_empty_string_after_detail_extraction(self) -> None:
        """
        GIVEN an adapter and a listing
        WHEN extract_detail() completes
        THEN full_text is a non-empty string ready for embedding.
        """
        # Given: adapter + listing
        cls = _make_adapter_class("ziprecruiter")
        adapter = cls()
        listing = _make_listing(board="ziprecruiter")
        page = MagicMock()

        # When: extract detail
        result = asyncio.run(adapter.extract_detail(page, listing))

        # Then: full_text is a non-empty string
        assert isinstance(result.full_text, str), "full_text should be a string"
        assert len(result.full_text) > 0, "full_text should not be empty"

    def test_board_field_matches_adapter_board_name(self) -> None:
        """
        GIVEN a listing created with an adapter's board name
        WHEN the board field is accessed
        THEN it matches the adapter's board_name property.
        """
        # Given: adapter + listing using its board name
        cls = _make_adapter_class("ziprecruiter")
        adapter = cls()
        listing = _make_listing(board=adapter.board_name)

        # Then: board field matches
        assert listing.board == adapter.board_name, "Listing board should match adapter board_name"

    def test_external_id_is_unique_within_a_board(self) -> None:
        """
        GIVEN two listings on the same board with different external IDs
        WHEN external_id is compared
        THEN they are distinct.
        """
        # Given: two listings with different IDs
        listing_a = _make_listing(board="ziprecruiter", external_id="zr-001")
        listing_b = _make_listing(board="ziprecruiter", external_id="zr-002")

        # Then: IDs are distinct
        assert listing_a.external_id != listing_b.external_id, "External IDs should differ"

    def test_missing_posted_at_does_not_raise(self) -> None:
        """
        GIVEN a listing constructed without posted_at
        WHEN posted_at is accessed
        THEN it defaults to None without raising.
        """
        # Given: listing without posted_at
        listing = _make_listing()

        # Then: defaults to None
        assert listing.posted_at is None, "posted_at should default to None"

    def test_metadata_defaults_to_empty_dict_not_none(self) -> None:
        """
        GIVEN a listing constructed without metadata
        WHEN metadata is accessed
        THEN it defaults to an empty dict, not None.
        """
        # Given: listing without metadata
        listing = _make_listing()

        # Then: defaults to empty dict
        assert listing.metadata is not None, "metadata should not be None"
        assert isinstance(listing.metadata, dict), "metadata should be a dict"
        assert listing.metadata == {}, "metadata should be empty"


# ---------------------------------------------------------------------------
# TestStubAdapterContract
# ---------------------------------------------------------------------------


class TestStubAdapterContract:
    """
    REQUIREMENT: Planned adapters conform to the full adapter behavioral contract.

    WHO: The pipeline runner invoking adapters polymorphically
    WHAT: (1) The adapter reports the expected board identifier through its `board_name` property.
          (2) The LinkedIn adapter exposes a wider `rate_limit_seconds` range than the default to avoid detection.
          (3) The adapter verifies a valid browser session during `authenticate()` without raising an error.
          (4) The adapter returns a list of `JobListing` objects from `search()` for a query.
          (5) The adapter populates a listing's `full_text` when `extract_detail()` is called on a shallow listing.
    WHY: These specs define the delivery contract — each adapter is
         expected to fail (xfail) until implementation is delivered,
         then the marks are removed and the specs become regression
         gates

    MOCK BOUNDARY:
        Mock: Playwright page (MagicMock) — browser I/O boundary
        Real: Adapter classes, board_name properties, method contracts
        Never: Patch adapter internals
    """

    _STUB_BOARDS: typing.ClassVar[list[tuple[str, type[JobBoardAdapter]]]] = [
        ("linkedin", LinkedInAdapter),
        ("indeed", IndeedAdapter),
        ("weworkremotely", WeWorkRemotelyAdapter),
    ]

    @pytest.mark.parametrize(
        ("expected_name", "adapter_cls"),
        _STUB_BOARDS,
        ids=["linkedin", "indeed", "weworkremotely"],
    )
    def test_stub_adapter_reports_correct_board_name(
        self, expected_name: str, adapter_cls: type[JobBoardAdapter]
    ) -> None:
        """
        GIVEN a stub adapter instance
        When board_name is accessed
        Then it returns the expected board identifier.
        """
        # Given: a stub adapter instance
        adapter = adapter_cls()

        # Then: board_name matches
        assert adapter.board_name == expected_name, (
            f"Expected board_name '{expected_name}', got '{adapter.board_name}'"
        )

    def test_linkedin_adapter_overrides_rate_limit_for_aggressive_detection(
        self,
    ) -> None:
        """
        GIVEN a LinkedIn adapter instance
        When rate_limit_seconds is accessed
        Then the bounds are wider than the default to avoid detection.
        """
        # Given: LinkedIn adapter
        adapter = LinkedInAdapter()

        # When: access rate limit
        lo, hi = adapter.rate_limit_seconds

        # Then: wider than the base default (1.5, 3.5)
        assert lo >= 5.0, (
            f"LinkedIn min throttle should be >= 5s for detection avoidance, got {lo}"
        )
        assert hi > lo, f"Upper bound {hi} should exceed lower bound {lo}"

    @pytest.mark.parametrize(
        ("_name", "adapter_cls"),
        _STUB_BOARDS,
        ids=["linkedin", "indeed", "weworkremotely"],
    )
    @pytest.mark.xfail(reason="Adapter not yet implemented")
    def test_stub_authenticate_completes_on_valid_session(
        self, _name: str, adapter_cls: type[JobBoardAdapter]
    ) -> None:
        """
        GIVEN a valid browser session
        When authenticate() is called
        Then the session is verified without error.
        """
        # Given: stub adapter + mock page with valid session
        adapter = adapter_cls()
        page = MagicMock()

        # When/Then: authenticate completes without raising
        asyncio.run(adapter.authenticate(page))

    @pytest.mark.parametrize(
        ("_name", "adapter_cls"),
        _STUB_BOARDS,
        ids=["linkedin", "indeed", "weworkremotely"],
    )
    @pytest.mark.xfail(reason="Adapter not yet implemented")
    def test_stub_search_returns_list_of_job_listings(
        self, _name: str, adapter_cls: type[JobBoardAdapter]
    ) -> None:
        """
        GIVEN a valid browser session
        When search() is called with a query
        Then a list of JobListings is returned.
        """
        # Given: stub adapter + mock page
        adapter = adapter_cls()
        page = MagicMock()

        # When: search is called
        result = asyncio.run(adapter.search(page, "staff architect"))

        # Then: returns a list of JobListings
        assert isinstance(result, list), f"Expected list, got {type(result)}"
        for item in result:
            assert isinstance(item, JobListing), f"Expected JobListing, got {type(item)}"

    @pytest.mark.parametrize(
        ("_name", "adapter_cls"),
        _STUB_BOARDS,
        ids=["linkedin", "indeed", "weworkremotely"],
    )
    @pytest.mark.xfail(reason="Adapter not yet implemented")
    def test_stub_extract_detail_populates_full_text(
        self, _name: str, adapter_cls: type[JobBoardAdapter]
    ) -> None:
        """
        GIVEN a shallow listing without full_text
        When extract_detail() is called
        Then the listing's full_text is populated.
        """
        # Given: stub adapter + mock page + shallow listing
        adapter = adapter_cls()
        page = MagicMock()
        listing = _make_listing(board=adapter.board_name)

        # When: extract_detail is called
        result = asyncio.run(adapter.extract_detail(page, listing))

        # Then: full_text is populated
        assert result.full_text != "", (
            f"Expected full_text to be populated, got: {result.full_text!r}"
        )
