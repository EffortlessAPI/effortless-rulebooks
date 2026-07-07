# Article Series: Full Production Plan

**Series title:** Trust the Artifact, Not the Autocomplete
**Source project:** `rulebook-examples/simpsons-paradox/`
**Goal:** 10 Medium articles targeting distinct audiences, each self-contained but cross-linked.
**LinkedIn:** One scheduled post per article, one week apart, linking to Medium.

---

## Phase 1: Article Writing (10 conversations)

Each article is written in its own conversation, one at a time. The web-based agent will not have
local repo access — everything it needs is in the outline file for that article.

**Process per article:**
1. Open a new conversation with the web agent
2. Paste the full content of the article's outline `.md` file
3. Say: "Write the full Medium article from this outline. Use all source material quoted in the file. Match the target length and tone."
4. Review, revise, finalize
5. Publish to Medium, copy the URL
6. Add the URL to the tracking table below

---

## Tracking Table

| # | File | Status | Medium URL | LinkedIn post date |
|---|------|--------|------------|--------------------|
| 1 | 01-general-public.md | outline complete | — | — |
| 2 | 02-executive.md | outline complete | — | — |
| 3 | 03-developer.md | outline complete | — | — |
| 4 | 04-data-scientist.md | outline complete | — | — |
| 5 | 05-knowledge-governance.md | outline complete | — | — |
| 6 | 06-legal-compliance.md | outline complete | — | — |
| 7 | 07-product-ops.md | outline complete | — | — |
| 8 | 08-high-school.md | outline complete | — | — |
| 9 | 09-young-child.md | outline complete | — | — |
| 10 | 10-phd-ontologist.md | outline complete | — | — |

---

## Phase 2: LinkedIn Scheduling

One post per week, Mondays 8am EST (suggested). Posts should:
- Open with the hook from the article (not a summary of the summary)
- Include one specific number or finding from the source material
- End with the Medium link
- Tag 2-3 relevant hashtags from the list below

**Hashtag bank:**
`#ArtificialIntelligence` `#AIGovernance` `#DataScience` `#Statistics` `#SimpsonParadox`
`#KnowledgeManagement` `#EnterpriseAI` `#DataEngineering` `#SoftwareEngineering`
`#OntologyEngineering` `#SemanticWeb` `#CriticalThinking` `#DataLiteracy`

---

## Series cross-linking strategy

Each article should:
- Open with a one-sentence series context: "This is part N of a 10-part series exploring what it looks like to build trustworthy AI systems with verifiable outputs."
- End with a "next in the series" or "also in this series" pointer to 1-2 adjacent articles by topic
- Include a link to the `00-series-overview.md` content (or a Medium "series" collection if Medium supports it)

**Audience bridge suggestions:**
- Article 1 (General public) → links to Article 8 (High school) for younger readers and Article 2 (Executive) for organizational context
- Article 3 (Developer) → links to Article 4 (Data scientist) for statistical depth and Article 10 (Ontologist) for formal theory
- Article 6 (Legal) → links to Article 2 (Executive) for boardroom framing and Article 5 (Knowledge governance) for vocabulary governance
- Article 9 (Young child) → links to Article 8 (High school) as the natural "next step" reader upgrade

---

## Key source material reference

All article outlines are self-contained with quoted source material. For additional depth,
the source files are:

- **Loops table data**: `effortless-rulebook/simpsons-paradox-rulebook.json` → `"Loops"` → `"data"`
- **InvariantChecks data**: same file → `"InvariantChecks"` → `"data"`
- **Conclusions data**: same file → `"Conclusions"` → `"data"`
- **ModelSummary data**: same file → `"ModelSummary"` → `"data"`
- **TreatmentRankings schema**: same file → `"TreatmentRankings"` → `"schema"` (for formula strings)
- **owl/reason.py**: `owl/reason.py` (the docstring alone is highly quotable)
- **IngestionProtocol**: `"IngestionProtocol"` table in the rulebook
- **StudyImportTemplate**: `"StudyImportTemplate"` table in the rulebook

---

## What NOT to add to articles

Per the CLAUDE.md doctrine for this repo:
- Do not invent statistics not found in the rulebook or `owl/reason.py`
- Do not describe Postgres as "the only complete substrate" — Python, Go, Excel, OWL are also fully expressive for their conformance sets
- Do not claim the LLM "wrote" the formulas — it proposed candidates; the formulas were formalized and tested
- Do not describe the paradox as "detected by AI" — it is computed as a derived fact from the DAG, not flagged by a model
- Do not mix the project rulebook with a demo rulebook — this is a standalone ERB project under `rulebook-examples/simpsons-paradox/`

---

## Estimated timeline (writing only)

Assuming one article per conversation, ~30-60 minutes each:
- 10 articles × 45 min avg = ~7.5 hours total writing time
- Plus review/edit: ~3-5 hours
- **Total:** ~10-12 hours across multiple sessions

LinkedIn scheduling takes ~10 minutes per post once the Medium URL exists.
