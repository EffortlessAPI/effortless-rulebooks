# Security Policy

## Threat Model

This is a local, single-user system with no network-exposed API surface and
no cloud egress. The threat model is therefore different from a cloud service
— but not empty. The primary risks are prompt injection via adversarial JD
content, path traversal via malformed job data, and context memory poisoning
via the decisions collection.

### Applicable Threats (SAFE-MCP Framework)

| SAFE-MCP TTP | Attack Vector | Severity | Mitigation Status |
|---|---|---|---|
| SAFE-T1102 Prompt Injection | Adversarial JD text containing LLM instructions injected into the disqualifier prompt | Medium — attacker controls JD content on the board | **Implemented** — defense-in-depth (see below) |
| SAFE-T2106 Context Memory Poisoning | A poisoned JD, once marked as a decision, enters ChromaDB and influences all future scoring via the `decisions` collection | Medium — persistent across sessions | **Implemented** — decision audit + surgical removal (Phase 6d) |
| SAFE-T1104 Over-Privileged Tool Abuse | Playwright session has access to authenticated board session; a compromised adapter could exfiltrate cookies | Low — adapter code is local and auditable | Adapter code review policy (see CONTRIBUTING.md) |
| SAFE-T1105 Path Traversal | JD title or company name containing `../` sequences used to construct export filenames | Low — file writes are scoped to `output/` | Phase 6c — input validation at adapter boundary |
| SAFE-T1503 Env-Var Scraping | N/A — no API keys stored; Ollama runs unauthenticated on localhost | Not applicable | — |
| SAFE-T1801 Automated Data Harvesting | The system itself is the harvester; ToS compliance is the mitigation | By design | Rate limiting, headless mode caps, overnight mode |

### Not-Applicable Threats

The following common AI system threats do not apply to this architecture:

- **Cloud data exfiltration** — no external API calls; all LLM and embedding
  inference runs on localhost via Ollama.
- **API key leakage** — no API keys exist; Ollama is unauthenticated on
  `localhost:11434`.
- **Model supply chain** — Ollama pulls models from its own registry with
  checksum verification. No custom model training or fine-tuning.

## Privacy Posture

- **No data leaves the machine.** All LLM and embedding calls go to
  `localhost:11434` (Ollama). No telemetry, no analytics, no external API
  keys required.
- **Resume, archetypes, and decision history** are stored only in local
  ChromaDB (`data/chroma_db/`) and JSONL files (`data/decisions/`).
- **Automated verification:** `TestPrivacyGuarantee` in `test_privacy.py`
  asserts that no outbound network calls are made during a scoring pipeline
  run. This is executable proof of the privacy claim, not just a README
  assertion.

## Decision Collection Integrity

The `decisions` collection is the system's long-term memory. A poisoned
entry (e.g., a malicious JD marked `yes`) persists and influences all
future scoring. Mitigation is auditability and surgical removal:

- **JSONL audit log is append-only.** Files in `data/decisions/` are the
  forensic record of every decision. They are never modified or deleted by
  any system operation. `decisions remove` appends a `verdict: "removed"`
  entry so the full history is replayable.
- **ChromaDB is rebuildable.** The `decisions` collection in ChromaDB can
  be reconstructed from the JSONL audit log via `rescore`. Removing an
  entry from ChromaDB does not destroy evidence — it only stops the entry
  from influencing future scoring.
- **Surgical removal:** `python -m jobsearch_rag decisions remove <job_id>`
  deletes a single entry from ChromaDB and logs the removal to JSONL.
- **Audit:** `python -m jobsearch_rag decisions audit` lists all decisions
  that include an operator-provided reason, for human review.
- **Inspection:** `python -m jobsearch_rag decisions show <job_id>` prints
  the stored metadata for a specific decision.

## Sensitive Files

| File / Directory | Contains | Git-ignored? |
|---|---|---|
| `data/*_session.json` | Playwright authentication cookies per board | Yes |
| `data/resume.md` | Professional background (PII risk) | Tracked — use `git update-index --skip-worktree` for local edits |
| `data/decisions/` | Job preferences and verdict history | Yes |
| `data/chroma_db/` | All embedded data (resume, archetypes, scores, decisions) | Yes |
| `data/logs/` | Structured session logs with job IDs and scores | Yes |

To prevent Git from tracking your local changes to `data/resume.md`:

```bash
git update-index --skip-worktree data/resume.md
# Undo: git update-index --no-skip-worktree data/resume.md
```

## Security-Sensitive Code Paths

Changes to these paths have security implications and require careful review:

| Path | Risk | Reason |
|---|---|---|
| `src/jobsearch_rag/adapters/` | Cookie exfiltration, session hijacking | Adapter code runs authenticated Playwright sessions |
| `src/jobsearch_rag/adapters/session.py` | Session persistence, browser launch | Controls cookie storage and browser process spawning |
| `src/jobsearch_rag/rag/scorer.py` | Prompt injection | Constructs the disqualifier LLM prompt from JD text |
| `src/jobsearch_rag/adapters/base.py` | Input validation bypass | `JobListing` dataclass — the system's trust boundary |
| `src/jobsearch_rag/rag/decisions.py` | Context memory poisoning | Records and removes decisions that influence future scoring |
| `src/jobsearch_rag/output/jd_files.py` | Path traversal | Constructs file paths from user-influenced data |

CI enforces this via `security-paths-check` — any PR touching these paths
is flagged with a warning in the workflow output.

## Prompt Injection Defense-in-Depth

No single technique defeats prompt injection. The disqualifier pipeline uses
four layers — an attacker must bypass all of them simultaneously:

| Layer | Mechanism | Catches | Fails When |
|---|---|---|---|
| LLM screening | Separate classify call detects AI-directed instructions in JD text | Novel semantic injection patterns | Recursive injection evades the screener |
| Output validation | Hardened JSON parser defaults to *not disqualified* on any parse failure | Malformed JSON from successful injection | Attacker produces schema-valid JSON |
| Regex pre-filter | Strips known injection signatures (`ignore previous instructions`, embedded `{"disqualified":...}` blobs) | Known low-effort patterns | Novel phrasing, encoding tricks |
| Human review | Operator reviews qualifying JDs during `review` mode; verdict overrides pipeline | Everything the pipeline misses | Operator doesn't review (by design, they always do) |

The screening layer sees the **original** JD text (unsanitized) so it can
detect injection language. If suspicious, the disqualifier is **skipped** —
denying the injection its target prompt. If the screening layer itself fails
(malformed JSON, exception), it defaults to **not suspicious** and the
disqualifier proceeds normally.

## Reporting a Vulnerability

If you discover a security vulnerability in this project, please report it
responsibly by emailing the maintainer directly rather than opening a public
issue.

## Future Considerations

The longer-term intent is to evolve this into a multi-tenant service. That
transition escalates several threats (prompt injection → High, context memory
poisoning → High) and introduces new ones (tenant isolation, AuthN/AuthZ,
transport security). See the project plan Phase 11 for the full
productionization roadmap.
