# PKO-Native Procedural Knowledge Rulebook

This directory is a concrete proof of the architecture:

```text
source representation
    -> one canonical PKO-native Effortless Rulebook
        -> natural-language procedure documentation
        -> financial SOPs
        -> email/SMS policies
        -> governance handbook
        -> any existing Effortless execution substrate
```

The canonical asset is [`effortless-rulebook/procedural-knowledge-ontology-rulebook.json`](effortless-rulebook/procedural-knowledge-ontology-rulebook.json). It is an Effortless Rulebook whose structural model is aligned to the **Procedural Knowledge Ontology (PKO) 2.0.0** and its **industry module 2.0.0**.

- PKO core version IRI: `https://w3id.org/pko/2.0.0`
- PKO industry version IRI: `https://w3id.org/pko/industry/2.0.0`
- ERB-PKO profile: `schemas/pko-erb-profile-1.0.0.schema.json`
- Canonical rulebook release: `1.0.0`

## What is represented

The single rulebook includes:

- procedure specifications and separately modeled executions;
- procedure versions, lifecycle status changes, steps, transitions, alternatives, and fallbacks;
- human actions, software functions, tools, requirements, and verifications;
- agents, stable roles, and time-bounded role assignments;
- resources, provenance, operational records, and live data bindings;
- errors, issue occurrences, questions, feedback, FAQs, and explanations;
- tacit, implicit, explicit, and situated knowledge;
- practitioner elicitation, rationales, exceptions, and known knowledge gaps;
- stewardship, authority, change requests, periodic reviews, and semantic versioning;
- communities of practice, mentorship, retrospectives, and learning evidence;
- financial-control semantics and email/SMS communication policy semantics.

The mock data contains two complete procedure families:

1. **Quarter-End Financial Close**
2. **Workforce Policy Change and Employee Notification**

## Native PKO versus the ERB-PKO extension

The project does not relabel every enterprise concept as PKO.

Exact PKO, P-Plan, PROV-O, DCAT, DCMI, OWL-Time, PRO, Metadata4Ing, and ODRL mappings are recorded in the `SemanticMappings` table. Concepts that PKO 2.0.0 does not define directly—such as `KnowledgeFragment`, `ElicitationSession`, `KnowledgeGap`, `StewardshipAssignment`, and `OperationalBinding`—are explicitly identified as `urn:effortless:pko-extension:*`.

See [`PKO-ALIGNMENT.md`](PKO-ALIGNMENT.md).

## Run it

```bash
./start.sh            # validate + regenerate every projection + run the tests
./start.sh validate   # validate only
./start.sh test       # tests only
./start.sh open       # full loop, then open the generated documentation
```

`./start.sh` is the one command for this domain. It has no server — the deliverables are documents — so start.sh validates the rulebook, regenerates all projections, exercises the BPM process-export adapter end-to-end, and runs the conformance tests.

Separately, the standard ERB build produces the plain-English RuleSpeak rendering via the published transpiler:

```bash
effortless build      # -> rulespeak/rulespeak.html + rulespeak.md
```

### Running the steps individually

No third-party Python packages are required.

```bash
# Validate the canonical rulebook
./bin/pko-rulebook-validate   -i effortless-rulebook/procedural-knowledge-ontology-rulebook.json

# Generate full human-readable procedure documentation
./bin/pko-rulebook-to-natural-language-docs   -i effortless-rulebook/procedural-knowledge-ontology-rulebook.json   -o generated/natural-language-docs.md

# Generate a finance-specific SOP
./bin/pko-rulebook-to-financial-sops   -i effortless-rulebook/procedural-knowledge-ontology-rulebook.json   -o generated/financial-sops.md

# Generate channel-specific employee email/SMS policies
./bin/pko-rulebook-to-text-email-policies   -i effortless-rulebook/procedural-knowledge-ontology-rulebook.json   -o generated/text-email-policies.md

# Generate governance and knowledge-health documentation
./bin/pko-rulebook-to-governance   -i effortless-rulebook/procedural-knowledge-ontology-rulebook.json   -o generated/governance.md
```

The generated Markdown files are disposable projections. Edit the rulebook, not the projection.

## One adapter unlocks every projection

[`tools/bpm_process_export_to_pko.py`](tools/bpm_process_export_to_pko.py) is a deliberately simple example of a source adapter.

```bash
./bin/bpm-process-export-to-pko   -i examples/bpm-vendor-payment.json   -o generated/bpm-imported-pko-rulebook.json

./bin/pko-rulebook-to-financial-sops   -i generated/bpm-imported-pko-rulebook.json   -o generated/bpm-financial-sops.md
```

The adapter maps a BPM process into the same canonical PKO rulebook tables:

- process -> `Procedures` + `ProcedureVersions`
- activity -> `Steps`
- edge -> `StepTransitions`
- role and assignee -> `Roles`, `Agents`, and `RoleAssignments`
- control -> `Requirements` + `StepRequirements`
- rationale -> `Rationales`
- exception -> `Exceptions`

Once that mapping exists, the imported process immediately gains every PKO projection above, plus the broader Effortless substrate matrix.

## Validation contract

The validator checks:

- ERB table and field conventions;
- unique stored identifiers and referential integrity;
- acyclic table relationships;
- calculated `Name` aliases;
- exact PKO 2.0.0 profile references;
- specification/execution separation;
- required semantic mappings;
- explicit coverage of the procedural-knowledge goals listed above.

The current canonical rulebook passes the validation contract. See [`generated/validation-report.md`](generated/validation-report.md).

## Tests

```bash
./start.sh test
# or: python3 -m unittest discover -s tests -v
```

## Attribution and status

PKO was created by Valentina Anita Carriero, Mario Scrocca, Ilaria Baroni, Antonia Azzini, and Irene Celino and is published under CC BY 4.0. This experiment references and aligns to PKO; it is not an official PKO distribution and does not imply endorsement by PKO's authors, Jessica Talisman, or any other third party.

See [`NOTICE.md`](NOTICE.md).

---

## Local transpiler bus (`localhost:4242`)

> **All 13 local transpilers live on `localhost:4242`.** Once you run
> `./start.sh` from the repo root, the ssotme-proxy exposes every repo-local
> transpiler — `rulebook-to-postgres`, `rulebook-to-python`, `rulebook-to-golang`,
> `rulebook-to-cobol`, `rulebook-to-owl`, and more — as first-class `ssotme://`
> routes any `effortless build` can call.
