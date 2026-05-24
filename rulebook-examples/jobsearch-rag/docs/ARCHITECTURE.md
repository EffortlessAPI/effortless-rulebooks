# Architecture

> System design guide for contributors. If you want to add a board adapter,
> fix a bug, or understand how the pieces connect, start here.

---

## Design Principles

1. **Local-first** — All processing (LLM inference, embeddings, vector storage)
   runs on your machine. Nothing leaves your network.
2. **Board-agnostic pipeline** — Only the adapter layer knows about specific
   job boards. Everything downstream (scoring, ranking, export) works against
   the `JobListing` data contract.
3. **Tests are the spec** — There is no separate requirements document. The
   test suite is the living specification. Each test class documents WHO needs
   it, WHAT it proves, and WHY. If there isn't **100% test coverage**, then the 
   implementation is underspecified.
4. **Actionable errors** — Every error carries enough context for the operator
   (or an AI assistant) to resolve it without searching logs or source code.

---

## System Overview

```
CLI ──▶ Adapter Layer ──▶ RAG Pipeline ──▶ Ranker ──▶ Export
          │                    │
          │                    ├── Resume embeddings (fit_score)
          │                    ├── Role archetypes (archetype_score)
          │                    └── Decision history (history_score)
          │
          ├── ZipRecruiter adapter
          ├── Indeed adapter
          ├── WeWorkRemotely adapter
          └── LinkedIn adapter (overnight mode)
```

Each run:
1. The CLI loads enabled boards from `config/settings.toml`
2. The adapter registry resolves board names to adapter classes (IoC)
3. Each adapter uses Playwright to navigate search results and extract listings
4. The RAG pipeline scores each listing against three ChromaDB collections
5. The ranker fuses scores, deduplicates across boards, and applies LLM disqualification
6. Results export as Markdown, CSV, or browser tabs

---

## Adapter Layer

### Data Contract: `JobListing`

Every adapter produces `JobListing` instances — the single abstraction that
makes the rest of the pipeline board-agnostic:

```python
@dataclass
class JobListing:
    board: str              # "ziprecruiter", "linkedin", etc.
    external_id: str        # Board's own job ID, for deduplication
    title: str
    company: str
    location: str
    url: str
    full_text: str          # Full JD text, extracted by adapter
    posted_at: datetime | None = None
    raw_html: str | None = None
    metadata: dict = field(default_factory=dict)
```

### Abstract Base Class: `JobBoardAdapter`

All adapters implement this interface:

| Method / Property | Purpose |
|---|---|
| `board_name` | Unique string identifier (matches `settings.toml` key) |
| `authenticate(page)` | Establish session; detect CAPTCHAs and expired cookies |
| `search(page, query, max_pages)` | Paginate search results → `list[JobListing]` |
| `extract_detail(page, listing)` | Navigate to listing URL → populate `full_text` |
| `rate_limit_seconds` | `(min, max)` tuple for random throttle jitter |

### Adapter Registry (IoC)

Adapters self-register via a decorator. The pipeline runner never imports
concrete adapter classes:

```python
@AdapterRegistry.register
class ZipRecruiterAdapter(JobBoardAdapter):
    ...
```

The runner resolves adapters from config:

```python
enabled = settings["boards"]["enabled"]     # ["ziprecruiter", ...]
adapters = [AdapterRegistry.get(name) for name in enabled]
```

This means adding a new board adapter requires **zero changes** to the
pipeline, ranker, or export code.

### ZipRecruiter: JSON Extraction Strategy

ZipRecruiter is a React SPA. The rendered DOM contains empty shell divs —
CSS selectors against the visible page structure won't find job data.
Instead, all data is embedded in:

```html
<script id="js_variables" type="application/json">{ ... }</script>
```

The adapter extracts this JSON blob via regex and parses:
- **Search results:** `hydrateJobCardsResponse.jobCards[]` — each card
  contains title, company, location, URL, snippet, salary, and metadata.
- **Job detail:** `getJobDetailsResponse.jobDetails.htmlFullDescription` —
  the full JD as HTML, which is stripped to plain text.

This approach is resilient to UI redesigns since the data contract is
separate from the rendering layer.

---

## Session Management

`SessionManager` handles Playwright browser context lifecycle:

- **Storage state persistence** — Cookies are saved per board to avoid
  re-authentication on every run.
- **Throttling** — `throttle()` applies random jitter within the adapter's
  `rate_limit_seconds` range between every page navigation.

Board-specific detection logic (e.g., LinkedIn's authwall redirects and
challenge interstitials) lives in the adapter module, not in `SessionManager`.
The session manager stays board-agnostic.

---

## Error Hierarchy

All errors extend `ActionableError`, which carries structured remediation:

```python
class ActionableError(Exception):
    error_type: ErrorType       # AUTHENTICATION, PARSE, CONNECTION, RATE_LIMIT
    suggestion: str             # Human-readable fix suggestion
    ai_guidance: AIGuidance     # Structured context for AI assistants
```

Factory classmethods enforce consistent construction:

```python
raise ActionableError.authentication(
    board="ziprecruiter",
    message="Session expired — cookies older than 24h",
    suggestion="Re-run with --login to refresh session",
)
```

This pattern ensures every error is immediately actionable without
requiring operators (or AI tools) to search source code.

---

## RAG Pipeline

### Three-Collection Scoring Model

Each job description is embedded and scored against three ChromaDB collections:

| Collection | Source | Score |
|---|---|---|
| `resume` | Your resume, chunked by section | `fit_score` — how well your background matches |
| `role_archetypes` | Natural language role descriptions from TOML | `archetype_score` — does this match what you're targeting? |
| `decisions` | Past yes/no/maybe verdicts you've recorded | `history_score` — resembles roles you've approved |

### Score Fusion

```
final_score = (
    archetype_weight × archetype_score +
    fit_weight       × fit_score       +
    history_weight   × history_score
) × (0.0 if disqualified else 1.0)
```

Weights are configurable in `settings.toml`. The LLM disqualifier catches
structural problems (IC role disguised as architect title, vendor chain
postings, SRE primary ownership) that keyword matching misses.

### Deduplication

The same job often appears on multiple boards. The ranker collapses
near-duplicates (cosine similarity > 0.95 on `full_text`) before final
ranking, keeping the highest-scored instance and noting which boards
carried it.

---

## Project Structure

```
jobsearch-rag/
├── config/
│   ├── settings.toml           # Board config, scoring weights, Ollama settings
│   └── role_archetypes.toml    # Your target role descriptions
├── data/
│   ├── resume.md               # Your resume in plain Markdown
│   ├── chroma_db/              # Vector store persistence (git-ignored)
│   └── decisions/              # Past verdicts (git-ignored)
├── src/jobsearch_rag/
│   ├── __main__.py             # CLI entry point
│   ├── errors.py               # ActionableError hierarchy
│   ├── adapters/
│   │   ├── base.py             # JobListing + JobBoardAdapter ABC
│   │   ├── registry.py         # AdapterRegistry (IoC)
│   │   ├── session.py          # SessionManager, throttle
│   │   ├── ziprecruiter.py     # ZipRecruiter JSON extraction
│   │   ├── indeed.py           # Indeed adapter
│   │   ├── weworkremotely.py   # WeWorkRemotely adapter
│   │   └── linkedin.py         # LinkedIn overnight adapter
│   ├── rag/                    # Ollama embeddings, ChromaDB, scoring
│   ├── pipeline/               # Runner orchestration, ranking
│   └── export/                 # Markdown, CSV, browser tab output
└── tests/
    ├── fixtures/               # HTML fixtures, sample JD JSON
    └── test_*.py               # 163+ tests — the living spec
```

---

## Testing Philosophy

Tests are organized by **behavioral requirement**, not by code structure.
Each test class documents:

- **WHO** needs this behavior
- **WHAT** the behavior is (including failure modes)
- **WHY** it matters (what breaks if the contract is violated)

```python
class TestAuthenticationFailures:
    """
    REQUIREMENT: Authentication failures are detected early and reported clearly.

    WHO: The operator running the tool; the pipeline runner
    WHAT: Expired sessions are detected before search begins; CAPTCHA
          encounters halt the run gracefully
    WHY: An unauthenticated scrape returns login-page HTML silently,
         producing zero valid listings with no error
    """
```

Key principles:

- **Mock I/O boundaries, not implementation** — Tests use HTML fixtures, not
  mocked method calls. When the adapter internals were rewritten from CSS
  selectors to JSON extraction, zero existing tests changed.
- **Failure specs are as important as happy-path specs** — An unspecified
  failure is an unhandled failure.
- **Missing spec = missing requirement** — When a bug is found, step one is
  always "add the missing spec."

---

## Adding a New Board Adapter

1. Create `src/jobsearch_rag/adapters/yourboard.py`
2. Subclass `JobBoardAdapter` and implement all abstract methods
3. Decorate with `@AdapterRegistry.register`
4. Add a `[boards.yourboard]` section to `config/settings.toml`
5. Add the board name to `boards.enabled`
6. Create test fixtures in `tests/fixtures/`
7. Write tests organized by behavioral requirement

The adapter must produce `JobListing` instances with all required fields
populated. The rest of the pipeline will pick it up automatically.

---

## Stack

| Layer | Choice | Why |
|---|---|---|
| Language | Python 3.13 | First-class support for local AI stack |
| Browser | Playwright | Async-native, clean cookie persistence, better waiting than Selenium |
| LLM | Ollama + mistral:7b | Zero-cost, privacy-respecting, no data egress |
| Embeddings | nomic-embed-text via Ollama | Fast local embeddings, consistent with local-first philosophy |
| Vector store | ChromaDB | Embedded, persistent, no server required |
| Config | TOML | Human-readable, easy to extend per board |
| Lint/format | ruff | Fast, replaces flake8 + isort + black |
| Type checking | pyright | Catches contract violations at dev time |
| Testing | pytest + pytest-asyncio | Async adapter tests, auto mode |
