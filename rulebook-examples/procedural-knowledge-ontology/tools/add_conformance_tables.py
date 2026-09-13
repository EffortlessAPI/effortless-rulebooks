#!/usr/bin/env python3
"""Add the conformance layer: how well every installed substrate agrees with the rulebook.

This project is compiled by eight tools. One of them, compile-rulebook, bakes
every calculated/lookup/aggregation value back into the rulebook's own rows;
those stored values are the answer keys. The other seven re-derive the same
values natively (Postgres views, Python, Go, TypeScript, Entity Framework,
Excel formulas, SHACL-AF rules) and the repo's conformance harness grades each
of them cell by cell.

These tables record that grading as data, so "does every substrate agree?" is
a view column rather than a log file:

    ConformanceSubstrates <- SubstrateRunScores -> ConformanceRuns
            ^                                         (history: one row per run x substrate)
            +-- TableConformance  -> RulebookTables   (latest run: one row per substrate x table)
            +-- FieldDisagreements -> RulebookFields  (latest run: one row per substrate x field
                      ^                                that got at least one cell wrong)
                      +-- CellDisagreements           (latest run: sampled failing cells with
                                                       expected and actual values)

Rows are written by tools/record_conformance.py, never by hand.

Targeted insertion; only adds its own keys. Re-running after the tables exist
is a no-op, so a lost write is simply re-appliable.
"""
import json
import sys

RB = "effortless-rulebook/procedural-knowledge-ontology-rulebook.json"
IRI = "urn:effortless:pko-extension#"
QUESTION = "q-auditor-substrate-agreement"


def f(name, dt, typ, desc, formula=None, related=None, nullable=True):
    d = {"name": name, "datatype": dt, "type": typ, "nullable": nullable, "Description": desc}
    if formula:
        d["formula"] = formula
    if related:
        d["RelatedTo"] = related
    return d


def iri():
    return f("SemanticTypeIri", "string", "raw", "Semantic type IRI.")


def score(tested, passed):
    return f"=IF({{{{{tested}}}}} = 0, 0, ROUND(100 * {{{{{passed}}}}} / {{{{{tested}}}}}, 2))"


T = {}

T["ConformanceSubstrates"] = {
    "Description": "Every tool that compiles this rulebook into something that computes. One is the answer-key author (compile-rulebook bakes derived values into the rulebook's own rows); the rest are graded substrates that re-derive those values natively and are scored cell by cell against them.",
    "important": True,
    "schema": [
        f("ConformanceSubstrateId", "string", "raw", "Substrate name as the conformance harness knows it, e.g. 'effortless-python'.", nullable=False),
        f("Name", "string", "calculated", "Human-readable calculated display alias.", "={{Label}}"),
        f("Label", "string", "raw", "Short human label, e.g. 'Python'."),
        f("Transpiler", "string", "raw", "The effortless.json tool that produces this substrate, e.g. 'rulebook-to-python'."),
        f("OutputFolder", "string", "raw", "Project folder the tool writes into, e.g. '/effortless-python'."),
        f("Engine", "string", "raw", "What actually evaluates the formulas: a database, a language runtime, a spreadsheet engine or a reasoner."),
        f("HowItComputes", "string", "raw", "One paragraph a curious reader can follow: what the tool emits and how the harness makes it produce answers."),
        f("Role", "string", "raw", "'answer-key' for the tool whose stored values every other substrate is graded against; 'graded' for everything else."),
        f("SortOrder", "number", "raw", "Display order."),
        f("IsGraded", "boolean", "calculated", "True for substrates the harness scores.", '={{Role}} = "graded"'),
        f("RunCount", "number", "aggregation", "How many recorded conformance runs graded this substrate.", "=COUNTIFS(SubstrateRunScores!{{Substrate}}, {{ConformanceSubstrateId}})"),
        f("LatestCellsTested", "number", "aggregation", "Cells graded in the latest run: every (record x derived field) pair in the answer keys.", "=SUMIFS(SubstrateRunScores!{{LatestCellsTested}}, SubstrateRunScores!{{Substrate}}, {{ConformanceSubstrateId}})"),
        f("LatestCellsPassed", "number", "aggregation", "Cells this substrate computed identically to the answer key in the latest run.", "=SUMIFS(SubstrateRunScores!{{LatestCellsPassed}}, SubstrateRunScores!{{Substrate}}, {{ConformanceSubstrateId}})"),
        f("LatestHarnessErrors", "number", "aggregation", "1 when the latest run could not execute this substrate at all.", "=SUMIFS(SubstrateRunScores!{{LatestErrorFlag}}, SubstrateRunScores!{{Substrate}}, {{ConformanceSubstrateId}})"),
        f("LatestCellsFailed", "number", "calculated", "Cells this substrate got wrong, or did not produce, in the latest run.", "={{LatestCellsTested}} - {{LatestCellsPassed}}"),
        f("LatestScore", "number", "calculated", "Percent of cells agreeing with the answer key in the latest run.", score("LatestCellsTested", "LatestCellsPassed")),
        f("DisagreeingFieldCount", "number", "aggregation", "Derived fields on which this substrate got at least one cell wrong in the latest run.", "=COUNTIFS(FieldDisagreements!{{Substrate}}, {{ConformanceSubstrateId}})"),
        f("DisagreeingTableCount", "number", "aggregation", "Tables on which this substrate got at least one cell wrong in the latest run.", "=COUNTIFS(TableConformance!{{ImperfectSubstrateKey}}, {{ConformanceSubstrateId}})"),
        f("IsFullyConformant", "boolean", "calculated", "True when the latest run graded this substrate, it ran, and every cell agreed. A substrate never graded is not conformant.", "=AND({{LatestCellsTested}} > 0, {{LatestCellsFailed}} = 0, {{LatestHarnessErrors}} = 0)"),
        iri(),
    ],
    "data": [],
}

T["ConformanceRuns"] = {
    "Description": "One execution of the conformance harness over every graded substrate. Kept as history; exactly one run is marked IsLatest, and the per-table, per-field and per-cell detail tables describe that run.",
    "important": True,
    "schema": [
        f("ConformanceRunId", "string", "raw", "Stored logical identifier, e.g. 'run-20260913-171500'.", nullable=False),
        f("Name", "string", "calculated", "Human-readable calculated display alias.", "={{ConformanceRunId}}"),
        f("RanOn", "datetime", "raw", "When the harness graded the substrates."),
        f("RulebookCommit", "string", "raw", "git HEAD of the repository when the run was recorded; the rulebook may also carry uncommitted edits, noted in Notes."),
        f("AnswerKeyAuthor", "string", "relationship", "The substrate whose stored values were the answer keys for this run.", related="ConformanceSubstrates"),
        f("IsLatest", "boolean", "raw", "True for the most recent recorded run only. The recorder moves it."),
        f("Notes", "string", "raw", "Anything a reader needs to interpret the run."),
        f("SubstrateCount", "number", "aggregation", "Substrates graded in this run.", "=COUNTIFS(SubstrateRunScores!{{Run}}, {{ConformanceRunId}})"),
        f("PerfectSubstrateCount", "number", "aggregation", "Substrates that ran and agreed on every cell.", "=COUNTIFS(SubstrateRunScores!{{PerfectRunKey}}, {{ConformanceRunId}})"),
        f("CellsTested", "number", "aggregation", "Cells graded across all substrates.", "=SUMIFS(SubstrateRunScores!{{CellsTested}}, SubstrateRunScores!{{Run}}, {{ConformanceRunId}})"),
        f("CellsPassed", "number", "aggregation", "Cells that agreed across all substrates.", "=SUMIFS(SubstrateRunScores!{{CellsPassed}}, SubstrateRunScores!{{Run}}, {{ConformanceRunId}})"),
        f("CellsFailed", "number", "calculated", "Cells that disagreed across all substrates.", "={{CellsTested}} - {{CellsPassed}}"),
        f("OverallScore", "number", "calculated", "Percent of all graded cells, across all substrates, that agreed.", score("CellsTested", "CellsPassed")),
        f("ImperfectSubstrateCount", "number", "calculated", "Substrates with at least one disagreeing cell or a harness error.", "={{SubstrateCount}} - {{PerfectSubstrateCount}}"),
        f("IsFullyConformant", "boolean", "calculated", "True when every substrate graded in this run agreed on every cell: the 100% bar.", "=AND({{SubstrateCount}} > 0, {{ImperfectSubstrateCount}} = 0)"),
        iri(),
    ],
    "data": [],
}

T["SubstrateRunScores"] = {
    "Description": "One substrate's grade in one conformance run, split by field class so a substrate that does scalar math natively but not cross-table joins reads as exactly that.",
    "important": True,
    "schema": [
        f("SubstrateRunScoreId", "string", "raw", "Stored logical identifier: '<run>|<substrate>'.", nullable=False),
        f("Name", "string", "calculated", "Human-readable calculated display alias.", '=CONCAT({{Run}}, " / ", {{Substrate}})'),
        f("Run", "string", "relationship", "The run that produced this grade.", related="ConformanceRuns"),
        f("Substrate", "string", "relationship", "The substrate graded.", related="ConformanceSubstrates"),
        f("HarnessError", "string", "raw", "Why the substrate could not be run, verbatim from the harness. Blank when it ran."),
        f("DurationSeconds", "number", "raw", "Wall-clock seconds to produce answers."),
        f("CellsTested", "number", "raw", "Every (record x derived field) pair in the answer keys."),
        f("CellsPassed", "number", "raw", "Cells matching the answer key."),
        f("CalculatedTested", "number", "raw", "Cells of calculated (same-row formula) fields."),
        f("CalculatedPassed", "number", "raw", "Calculated cells that matched."),
        f("LookupTested", "number", "raw", "Cells of lookup (cross-table INDEX/MATCH) fields."),
        f("LookupPassed", "number", "raw", "Lookup cells that matched."),
        f("AggregationTested", "number", "raw", "Cells of aggregation (COUNTIFS/SUMIFS rollup) fields."),
        f("AggregationPassed", "number", "raw", "Aggregation cells that matched."),
        f("CellsFailed", "number", "calculated", "Cells that did not match.", "={{CellsTested}} - {{CellsPassed}}"),
        f("Score", "number", "calculated", "Percent of cells matching.", score("CellsTested", "CellsPassed")),
        f("CalculatedScore", "number", "calculated", "Percent of calculated cells matching.", score("CalculatedTested", "CalculatedPassed")),
        f("LookupScore", "number", "calculated", "Percent of lookup cells matching.", score("LookupTested", "LookupPassed")),
        f("AggregationScore", "number", "calculated", "Percent of aggregation cells matching.", score("AggregationTested", "AggregationPassed")),
        f("IsPerfect", "boolean", "calculated", "True when the substrate ran and matched every cell.", '=AND({{HarnessError}} = "", {{CellsTested}} > 0, {{CellsFailed}} = 0)'),
        f("PerfectRunKey", "string", "calculated", "The run id when this grade is perfect, else blank; lets the run count its perfect substrates with a single-criterion COUNTIFS.", '=IF({{IsPerfect}}, {{Run}}, "")'),
        f("IsInLatestRun", "boolean", "lookup", "Whether this grade belongs to the latest run.", "=INDEX(ConformanceRuns!{{IsLatest}}, MATCH({{Run}}, ConformanceRuns!{{ConformanceRunId}}, 0))"),
        f("LatestCellsTested", "number", "calculated", "CellsTested when in the latest run, else 0.", "=IF({{IsInLatestRun}}, {{CellsTested}}, 0)"),
        f("LatestCellsPassed", "number", "calculated", "CellsPassed when in the latest run, else 0.", "=IF({{IsInLatestRun}}, {{CellsPassed}}, 0)"),
        f("LatestErrorFlag", "number", "calculated", "1 when this is the latest run and the harness could not run the substrate.", '=IF(AND({{IsInLatestRun}}, {{HarnessError}} <> ""), 1, 0)'),
        f("SubstrateLabel", "string", "lookup", "The substrate's short label.", "=INDEX(ConformanceSubstrates!{{Label}}, MATCH({{Substrate}}, ConformanceSubstrates!{{ConformanceSubstrateId}}, 0))"),
        iri(),
    ],
    "data": [],
}

T["TableConformance"] = {
    "Description": "Latest run only: how one substrate did on one table. The grid of these rows is the substrate-by-table heatmap.",
    "important": True,
    "schema": [
        f("TableConformanceId", "string", "raw", "Stored logical identifier: '<substrate>|<Table>'.", nullable=False),
        f("Name", "string", "calculated", "Human-readable calculated display alias.", '=CONCAT({{Substrate}}, " / ", {{RulebookTable}})'),
        f("Run", "string", "relationship", "The run this row describes.", related="ConformanceRuns"),
        f("Substrate", "string", "relationship", "The substrate graded.", related="ConformanceSubstrates"),
        f("RulebookTable", "string", "relationship", "The table graded.", related="RulebookTables"),
        f("RecordCount", "number", "raw", "Rows in the answer key for this table."),
        f("DerivedFieldCount", "number", "raw", "Derived fields graded on this table."),
        f("CellsTested", "number", "raw", "RecordCount x DerivedFieldCount."),
        f("CellsPassed", "number", "raw", "Cells matching the answer key."),
        f("IsMissingAnswerFile", "boolean", "raw", "True when the substrate produced no answers for this table at all; every cell then counts as failed."),
        f("CellsFailed", "number", "calculated", "Cells that did not match.", "={{CellsTested}} - {{CellsPassed}}"),
        f("Score", "number", "calculated", "Percent of cells matching.", score("CellsTested", "CellsPassed")),
        f("IsPerfect", "boolean", "calculated", "True when every cell matched.", "={{CellsFailed}} = 0"),
        f("ImperfectSubstrateKey", "string", "calculated", "The substrate id when this table is imperfect, else blank.", '=IF({{IsPerfect}}, "", {{Substrate}})'),
        f("ImperfectTableKey", "string", "calculated", "The table id when this table is imperfect, else blank.", '=IF({{IsPerfect}}, "", {{RulebookTable}})'),
        f("DisagreeingFieldCount", "number", "aggregation", "Fields on this table the substrate got at least one cell wrong.", "=COUNTIFS(FieldDisagreements!{{TableConformance}}, {{TableConformanceId}})"),
        f("SubstrateLabel", "string", "lookup", "The substrate's short label.", "=INDEX(ConformanceSubstrates!{{Label}}, MATCH({{Substrate}}, ConformanceSubstrates!{{ConformanceSubstrateId}}, 0))"),
        f("SubjectArea", "string", "lookup", "The table's subject area.", "=INDEX(RulebookTables!{{SubjectArea}}, MATCH({{RulebookTable}}, RulebookTables!{{RulebookTableId}}, 0))"),
        iri(),
    ],
    "data": [],
}

T["FieldDisagreements"] = {
    "Description": "Latest run only: a derived field on which one substrate got at least one cell wrong. A field with no row here agreed everywhere. CellsFailed is the true count; CellDisagreements holds a sample of the cells.",
    "important": True,
    "schema": [
        f("FieldDisagreementId", "string", "raw", "Stored logical identifier: '<substrate>|<Table>.<Field>'.", nullable=False),
        f("Name", "string", "calculated", "Human-readable calculated display alias.", '=CONCAT({{Substrate}}, " / ", {{RulebookField}})'),
        f("Substrate", "string", "relationship", "The substrate that disagreed.", related="ConformanceSubstrates"),
        f("RulebookField", "string", "relationship", "The field it disagreed on.", related="RulebookFields"),
        f("TableConformance", "string", "relationship", "The substrate-by-table row this field belongs to.", related="TableConformance"),
        f("FieldClass", "string", "raw", "calculated, lookup or aggregation."),
        f("CellsFailed", "number", "raw", "Every cell of this field the substrate got wrong, not just the sampled ones."),
        f("DominantReason", "string", "raw", "The harness's most frequent reason: 'wrong/null value', 'missing record' or 'missing entity file'."),
        f("SampledCellCount", "number", "aggregation", "Failing cells recorded as CellDisagreements rows.", "=COUNTIFS(CellDisagreements!{{FieldDisagreement}}, {{FieldDisagreementId}})"),
        f("IsFullySampled", "boolean", "calculated", "True when every failing cell is recorded, not just a sample.", "={{SampledCellCount}} = {{CellsFailed}}"),
        f("Formula", "string", "lookup", "The formula every substrate was asked to compute, shown as evidence.", "=INDEX(RulebookFields!{{Formula}}, MATCH({{RulebookField}}, RulebookFields!{{RulebookFieldId}}, 0))"),
        f("SubstrateLabel", "string", "lookup", "The substrate's short label.", "=INDEX(ConformanceSubstrates!{{Label}}, MATCH({{Substrate}}, ConformanceSubstrates!{{ConformanceSubstrateId}}, 0))"),
        iri(),
    ],
    "data": [],
}

T["CellDisagreements"] = {
    "Description": "Latest run only: one cell a substrate computed differently from the answer key, with both values verbatim. Sampled per field; see FieldDisagreements.IsFullySampled.",
    "important": True,
    "schema": [
        f("CellDisagreementId", "string", "raw", "Stored logical identifier: '<substrate>|<Table>.<Field>|<record id>'.", nullable=False),
        f("Name", "string", "calculated", "Human-readable calculated display alias.", '=CONCAT({{FieldDisagreement}}, " @ ", {{RecordId}})'),
        f("FieldDisagreement", "string", "relationship", "The substrate-by-field disagreement this cell belongs to.", related="FieldDisagreements"),
        f("RecordId", "string", "raw", "Primary key of the row whose cell disagreed."),
        f("ExpectedValue", "string", "raw", "The answer key's value, JSON-encoded so null, '', false and 0 stay distinguishable."),
        f("ActualValue", "string", "raw", "The substrate's value, JSON-encoded the same way."),
        f("Reason", "string", "raw", "The harness's reason: 'wrong/null value', 'missing record' or 'missing entity file'."),
        f("Substrate", "string", "lookup", "The substrate that produced ActualValue.", "=INDEX(FieldDisagreements!{{Substrate}}, MATCH({{FieldDisagreement}}, FieldDisagreements!{{FieldDisagreementId}}, 0))"),
        f("RulebookField", "string", "lookup", "The field.", "=INDEX(FieldDisagreements!{{RulebookField}}, MATCH({{FieldDisagreement}}, FieldDisagreements!{{FieldDisagreementId}}, 0))"),
        iri(),
    ],
    "data": [],
}

# Reverse aggregations on the existing field and table catalogs, so the
# explorer can show cross-substrate agreement beside every field and table.
CATALOG_FIELDS = {
    "RulebookFields": [
        f("DisagreeingSubstrateCount", "number", "aggregation", "How many substrates computed this field differently from the answer key in the latest conformance run. 0 means every substrate agreed.", "=COUNTIFS(FieldDisagreements!{{RulebookField}}, {{RulebookFieldId}})"),
        f("IsSubstrateContested", "boolean", "calculated", "True when at least one substrate disagrees about this field's values.", "={{DisagreeingSubstrateCount}} > 0"),
    ],
    "RulebookTables": [
        f("DisagreeingSubstrateCount", "number", "aggregation", "How many substrates got at least one cell of this table wrong in the latest conformance run.", "=COUNTIFS(TableConformance!{{ImperfectTableKey}}, {{RulebookTableId}})"),
    ],
}

# The witness layer: every new derived field traces to a role's question.
ROLE_QUESTION = {
    "RoleQuestionId": QUESTION,
    "AskingRole": "knowledge-authority",
    "WitnessLoop": None,
    "QuestionText": "This rulebook compiles into Postgres, Python, Go, TypeScript, C#, a spreadsheet and an OWL reasoner. When I approve a rule, do all of them compute the same answer, and if not, which one is wrong about which field?",
    "WhyItMatters": "A procedure is only as authoritative as the systems that execute it. If the spreadsheet a controller opens disagrees with the database the close automation reads, the rule has two meanings, and nobody finds out until an audit does. Agreement has to be witnessed per cell, not assumed from a green build.",
    "AnswerableBefore": "No. The harness printed scores to a markdown file per substrate; the rulebook had no record of which substrates exist, how they scored, or which fields they disagree on.",
    "WitnessedAnswer": None,
    "SemanticTypeIri": IRI + "RoleQuestion",
}

TABLE_ROWS = [
    ("ConformanceSubstrates", "Substrates the conformance harness grades, and the answer-key author."),
    ("ConformanceRuns", "Conformance harness executions."),
    ("SubstrateRunScores", "Per-run, per-substrate grades."),
    ("TableConformance", "Latest-run grades per substrate and table."),
    ("FieldDisagreements", "Latest-run fields a substrate got wrong."),
    ("CellDisagreements", "Latest-run sampled failing cells."),
]


def snake(name):
    import re
    s1 = re.sub(r"([^_])([A-Z][a-z]+)", r"\1_\2", name)
    return re.sub(r"([a-z0-9])([A-Z])", r"\1_\2", s1).lower()


def main():
    rb = json.load(open(RB, encoding="utf-8"))
    changed = False

    for k, v in T.items():
        if k not in rb:
            rb[k] = v
            changed = True

    for table, fields in CATALOG_FIELDS.items():
        names = {x["name"] for x in rb[table]["schema"]}
        for field in fields:
            if field["name"] not in names:
                schema = rb[table]["schema"]
                # Keep SemanticTypeIri last, matching every other table.
                at = next((i for i, x in enumerate(schema) if x["name"] == "SemanticTypeIri"), len(schema))
                schema.insert(at, field)
                changed = True

    if not any(r["RoleQuestionId"] == QUESTION for r in rb["RoleQuestions"]["data"]):
        rb["RoleQuestions"]["data"].append(dict(ROLE_QUESTION))
        changed = True

    # Pre-register the new derived fields in the catalog with their question,
    # so reconcile_field_catalog.py (which preserves InventedForQuestion by
    # RulebookFieldId) keeps the provenance when it rebuilds the census.
    have_fields = {r["RulebookFieldId"] for r in rb["RulebookFields"]["data"]}
    derived = [(t, x) for t, v in T.items() for x in v["schema"]] + \
              [(t, x) for t, xs in CATALOG_FIELDS.items() for x in xs]
    for table, field in derived:
        fid = f"{table}.{field['name']}"
        if field["type"] in ("calculated", "lookup", "aggregation") and field["name"] != "Name" \
                and fid not in have_fields:
            rb["RulebookFields"]["data"].append({
                "RulebookFieldId": fid,
                "TargetTable": table,
                "FieldName": field["name"],
                "FieldType": field["type"],
                "Datatype": field["datatype"],
                "Formula": field.get("formula"),
                "InventedForQuestion": QUESTION,
                "SemanticTypeIri": IRI + "RulebookField",
            })
            changed = True

    have_tables = {r["RulebookTableId"] for r in rb["RulebookTables"]["data"]}
    for table, _ in TABLE_ROWS:
        if table not in have_tables:
            rb["RulebookTables"]["data"].append({
                "RulebookTableId": table,
                "TableName": table,
                "PhysicalTable": snake(table),
                "PhysicalView": "vw_" + snake(table),
                "SubjectArea": "meta",
                "IsExtension": True,
                "SemanticTypeIri": IRI + "RulebookTable",
            })
            changed = True

    have_maps = {r["SemanticMappingId"] for r in rb["SemanticMappings"]["data"]}
    for table, note in TABLE_ROWS:
        mid = "map-" + snake(table).replace("_", "-")
        if mid not in have_maps:
            rb["SemanticMappings"]["data"].append({
                "SemanticMappingId": mid,
                "SourcePath": table,
                "MappingKind": "class",
                "TargetIri": IRI + table[:-1] if table.endswith("s") else IRI + table,
                "MappingRelation": "extension",
                "OntologyProfile": "erb-pko-extension-1.0.0",
                "Notes": note + " Not part of PKO 2.0.0.",
            })
            changed = True

    if not changed:
        print("conformance layer already present; nothing to do")
        return 0

    with open(RB, "w", encoding="utf-8") as fh:
        json.dump(rb, fh, indent=1, ensure_ascii=False)
        fh.write("\n")
    print("added conformance layer")
    return 0


if __name__ == "__main__":
    sys.exit(main())
