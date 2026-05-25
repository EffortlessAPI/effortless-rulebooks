# Contributing

Thanks for your interest in contributing to JobSearch RAG. This document
covers the development setup, coding standards, testing philosophy, and
PR process.

---

## Development Setup

```bash
# Clone
git clone https://github.com/grimlor/jobsearch-rag.git
cd jobsearch-rag

# Install with dev dependencies (creates .venv automatically)
uv sync --extra dev

# Optional: auto-activate venv
direnv allow

# Install Playwright browser
uv run playwright install chromium
```

## Running Checks

All checks must pass before submitting a PR:

```bash
task check          # runs lint → type → test
```

Or individually:

```bash
task lint           # ruff check src/ tests/
task format         # ruff format src/ tests/
task type           # pyright type checking
task test           # pytest -v
```

## Code Style

- **Python 3.13** — use modern syntax (`X | Y` unions, `match` statements
  where appropriate).
- **`from __future__ import annotations`** at the top of every module.
- **ruff** handles formatting and import sorting. Don't fight it.
- **pyright** — all functions need type annotations. No `Any` unless
  you have a good reason and document it.
- **Line length:** 99 characters (configured in `pyproject.toml`).

## Testing Standards

Tests are the living specification. Every test class documents a behavioral
requirement, not a code structure.

### Test Class Structure

```python
class TestYourFeature:
    """
    REQUIREMENT: One-sentence summary of the behavioral contract.

    WHO: Who depends on this behavior (operator, pipeline runner, etc.)
    WHAT: What the behavior is, including failure modes
    WHY: What breaks if this contract is violated
    """

    def test_descriptive_name_of_scenario(self) -> None:
        """What this scenario proves in plain English."""
        ...
```

### Key Principles

1. **Mock I/O boundaries, not implementation.** Use HTML fixtures and data
   fixtures. Don't mock internal method calls — that couples tests to
   implementation details and makes refactoring painful.

2. **Failure specs matter.** For every happy path, ask: what goes wrong?
   Write specs for those failure modes. An unspecified failure is an
   unhandled failure.

3. **Missing spec = missing requirement.** If you find a bug, the first
   step is always adding the test that should have caught it, then fixing
   the code to pass that test.

4. **Docstrings on every test method.** One sentence explaining what the
   test proves. This makes test output readable as a specification.

5. **Diagnostic assertion messages.** Every `assert` must include a
   message explaining what went wrong:
   ```python
   # ✅ Good
   assert result is not None, "Expected non-None result from query"
   assert score > 0.5, f"Expected score > 0.5 for qualified role, got {score}"

   # ❌ Bad
   assert result is not None
   assert score > 0.5
   ```

### Fixtures

HTML fixtures live in `tests/fixtures/`. When adding a new board adapter:

- Capture real page HTML from the board (sanitize any personal data)
- Create both search results and detail page fixtures
- Validate fixture structure matches what the live site actually serves

## Architecture

See [ARCHITECTURE.md](docs/ARCHITECTURE.md) for system design, the adapter
pattern, and how the pieces connect.

### Adding a Board Adapter

1. Create `src/jobsearch_rag/adapters/yourboard.py`
2. Subclass `JobBoardAdapter` and implement all abstract methods
3. Decorate with `@AdapterRegistry.register`
4. Add config in `settings.toml`
5. Create fixtures and write tests organized by behavioral requirement

The adapter must produce `JobListing` instances. Everything downstream
picks it up automatically via the registry.

## Commit Messages

Follow [Conventional Commits](https://www.conventionalcommits.org/).

```
feat(scraper): add WeWorkRemotely adapter with fixture-based tests

- Implement search pagination and JD extraction
- Add HTML fixtures for search results and detail page
- 24 tests covering extraction, auth failures, and edge cases
```

## AI Coding Agent Setup

This project uses [universal-dev-skills](https://github.com/grimlor/universal-dev-skills)
for AI coding agent configuration (skills, instructions, and agents). Clone
that repo and follow its README to configure your editor. No per-repo setup
is required — the skills apply automatically across all workspaces.

## Pull Requests

1. **Branch from `main`.**
2. **All checks must pass** — `task check` (lint + type + test).
3. **Include tests** for any new behavior or bug fix.
4. **One concern per PR** — don't mix a new feature with unrelated refactoring.
5. **Describe what and why** in the PR description. If it changes adapter
   behavior, note whether existing tests needed modification (ideally not).

### Security-Sensitive Changes

Changes to the following paths have security implications and require
explicit justification in the PR description:

- `src/jobsearch_rag/adapters/` — adapter code runs authenticated Playwright
  sessions; changes here can affect session cookie handling and browser behavior
- `src/jobsearch_rag/rag/scorer.py` — constructs the disqualifier LLM prompt;
  changes here affect prompt injection attack surface
- `src/jobsearch_rag/adapters/base.py` — `JobListing` dataclass is the trust
  boundary; changes to validation affect all downstream processing
- `src/jobsearch_rag/output/jd_files.py` — constructs file paths from
  user-influenced data; changes here affect path traversal risk

CI flags PRs touching these paths with a security warning. When your PR
modifies security-sensitive paths, include a brief note in the description
explaining why the change is safe. See [SECURITY.md](SECURITY.md) for the
full threat model.

## Reporting Issues

When filing an issue:

- **Bug:** Include the error message, what you expected, and steps to
  reproduce. If it's an adapter issue, note which board and whether it
  works against the fixture HTML.
- **Feature request:** Describe the problem you're trying to solve, not
  just the solution you have in mind.
- **New board adapter:** Open an issue first to discuss the board's
  structure before starting implementation. Some boards (e.g., LinkedIn)
  have detection mechanisms that require careful design.
