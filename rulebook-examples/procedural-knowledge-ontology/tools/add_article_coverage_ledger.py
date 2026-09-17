#!/usr/bin/env python3
"""Loop 5: coverage of the source articles as rulebook data.

Adds SourceArticles, ArticleClaims, ClaimEvidence and KnowledgeMethods; adds the
measurement columns that tools/measure_catalog.py fills from Postgres; seeds every
claim from tools/article_claims.py and every evidence link from
tools/article_evidence.py. Idempotent: re-running updates authored rows in place.

"Covered" is decided by ClaimEvidence.IsValid, a formula. This script never marks
anything covered. See ARTICLE-COVERAGE.md.
"""
from __future__ import annotations

import importlib.util
import os
import sys
from collections import OrderedDict
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
from rulebook_edit import (EXT, agg, calc, dump, ensure_fields, ensure_table, idx, load,  # noqa: E402
                           lookup, raw, rel, upsert_rows, mapping)
import article_claims  # noqa: E402
import article_evidence  # noqa: E402

EVIDENCE_TARGET = {  # EvidenceKind -> the ClaimEvidence column that carries the target
    "Field": "RulebookField", "Table": "RulebookTable", "RoleQuestion": "RoleQuestion",
    "OntologyProfile": "OntologyProfile", "KnowledgeMethod": "KnowledgeMethod",
    "Procedure": "Procedure",
}


LEDGER_COLUMNS = {
    "KnowledgeMethods": {"KnowledgeMethodId", "Name", "Label", "MethodFamily", "Summary", "OriginReference",
                         "ElicitationUseCount", "ApplicationCount", "UsageCount", "IsApplied", "SemanticTypeIri"},
    "MethodApplications": {"MethodApplicationId", "Name", "KnowledgeMethod", "AppliedTo", "AppliedAt", "AppliedByAgent",
                           "SemanticTypeIri"},
}


def schema_changes(rb):
    # ---- census repair: identity tables that predate RulebookTables rows ------
    for t in ("AppUsers", "PrincipalAssignments", "IssuedTokens"):
        upsert_rows(rb, "RulebookTables", [OrderedDict([
            ("RulebookTableId", t), ("TableName", t), ("PhysicalTable", None), ("PhysicalView", None),
            ("SubjectArea", "access-control"), ("IsExtension", True),
            ("SemanticTypeIri", f"{EXT}RulebookTable")])])

    # ---- measurement columns, written back from Postgres ----------------------
    # Ledger-owned columns on the catalog tables are rewritten each run so a changed formula lands.
    ledger_cols = {"RulebookFields": {"MeasuredNonBlankCount", "MeasuredSubstantiveCount", "MeasuredDistinctValueCount",
                                      "HasMeasuredData", "IsDiscriminating"},
                   "RulebookTables": {"MeasuredRowCount", "HasMeasuredRows"}}
    for tbl, cols in ledger_cols.items():
        rb[tbl]["schema"] = [f for f in rb[tbl]["schema"] if f["name"] not in cols]
    ensure_fields(rb, "RulebookFields", [
        raw("MeasuredSubstantiveCount", "integer",
            "Rows of this field's vw_* column holding a substantive value (not blank, false or zero), measured in Postgres by tools/measure_catalog.py."),
        raw("MeasuredDistinctValueCount", "integer",
            "Distinct values of this field's vw_* column, with blank counted as one value, measured in Postgres."),
        calc("HasMeasuredData", "boolean",
             "TRUE when Postgres holds at least one substantive value (not blank, false or zero) for this field.",
             "={{MeasuredSubstantiveCount}} > 0"),
        calc("IsDiscriminating", "boolean",
             "TRUE when the field takes at least two distinct values over the seed data, so it can tell rows apart. An all-true or all-false witness states nothing.",
             "={{MeasuredDistinctValueCount}} >= 2"),
    ], after="IsSubstrateContested")
    ensure_fields(rb, "RulebookTables", [
        raw("MeasuredRowCount", "integer",
            "Rows in this table's vw_* view, measured in Postgres by tools/measure_catalog.py."),
        calc("HasMeasuredRows", "boolean", "TRUE when Postgres holds at least one row of this table.",
             "={{MeasuredRowCount}} > 0"),
    ], after="DisagreeingSubstrateCount")

    # ---- counts the evidence rules read ---------------------------------------
    ensure_fields(rb, "OntologyProfiles", [
        agg("MappingCount", "integer", "Semantic mappings that use this ontology profile.",
            "=COUNTIFS(SemanticMappings!{{OntologyProfile}}, {{OntologyProfileId}})"),
    ])
    ensure_fields(rb, "ProcedureVersions", [
        agg("ExecutionCount", "integer", "Recorded executions of this procedure version.",
            "=COUNTIFS(ProcedureExecutions!{{ProcedureVersion}}, {{ProcedureVersionId}})"),
    ])
    ensure_fields(rb, "Procedures", [
        agg("ExecutionCount", "integer", "Recorded executions across every version of this procedure.",
            "=SUMIFS(ProcedureVersions!{{ExecutionCount}}, ProcedureVersions!{{Procedure}}, {{ProcedureId}})"),
    ])

    # ---- KnowledgeMethods -----------------------------------------------------
    # The ledger owns these tables outright: their schema is rewritten on every run so a
    # changed formula lands (rows are kept).
    # Columns that loop specs add to these tables (for example a relationship column on
    # MethodApplications) are kept and re-appended after the ledger's own columns.
    extra_cols = {}
    for owned in ("KnowledgeMethods", "MethodApplications", "SourceArticles", "ArticleClaims", "ClaimEvidence"):
        if owned in rb:
            extra_cols[owned] = [f for f in rb[owned]["schema"]
                                 if owned in LEDGER_COLUMNS and f.get("name") not in LEDGER_COLUMNS[owned]]
            rb[owned]["schema"] = []
    ensure_table(rb, "MethodApplications",
        "One recorded application of a knowledge method to a concrete artifact of this model. Themes add a "
        "relationship column here for the artifact table they apply a method to.",
        [
            raw("MethodApplicationId", "string", "Stored identifier.", nullable=False),
            calc("Name", "string", "Display name.", '={{KnowledgeMethod}} & " applied: " & LEFT({{AppliedTo}}, 50)'),
            rel("KnowledgeMethod", "KnowledgeMethods", "Method applied."),
            raw("AppliedTo", "string", "The artifact the method was applied to, in words."),
            raw("AppliedAt", "datetime", "When."),
            rel("AppliedByAgent", "Agents", "Who applied it."),
            raw("SemanticTypeIri", "string", "Semantic type IRI."),
        ], "knowledge")
    ensure_table(rb, "KnowledgeMethods",
        "Named methods, techniques and methodologies for eliciting, analysing, modeling and governing "
        "process knowledge. A method is only evidence that the model uses it when a seeded row applies it.",
        [
            raw("KnowledgeMethodId", "string", "Stored identifier; PascalCase method key.", nullable=False),
            calc("Name", "string", "Display name.", "={{Label}}"),
            raw("Label", "string", "Human-readable method name."),
            raw("MethodFamily", "string", "Elicitation | Analysis | Modeling | Encoding | Governance."),
            raw("Summary", "string", "What the method does, paraphrased."),
            raw("OriginReference", "string", "Where the method comes from."),
            agg("ElicitationUseCount", "integer", "Elicitation sessions run with this method.",
                "=COUNTIFS(ElicitationSessions!{{Method}}, {{KnowledgeMethodId}})"),
            agg("ApplicationCount", "integer", "Recorded applications of this method to model artifacts.",
                "=COUNTIFS(MethodApplications!{{KnowledgeMethod}}, {{KnowledgeMethodId}})"),
            calc("UsageCount", "integer", "Every recorded application of this method across the model.",
                 "={{ElicitationUseCount}} + {{ApplicationCount}}"),
            calc("IsApplied", "boolean", "TRUE when at least one seeded row applies this method.",
                 "={{UsageCount}} > 0"),
            raw("SemanticTypeIri", "string", "Semantic type IRI."),
        ], "knowledge")

    # ElicitationSessions.Method becomes a reference; the stored values already are method keys.
    for f in rb["ElicitationSessions"]["schema"]:
        if f["name"] == "Method" and f["type"] == "raw":
            f["type"] = "relationship"
            f["RelatedTo"] = "KnowledgeMethods"
            f["Description"] = "The knowledge method this session used."

    # ---- SourceArticles -------------------------------------------------------
    ensure_table(rb, "SourceArticles",
        "The articles this rulebook was bootstrapped from. Coverage of each is computed from its claims; "
        "the article text itself is never stored.",
        [
            raw("SourceArticleId", "string", "Stored identifier.", nullable=False),
            calc("Name", "string", "Display name.", "={{Title}}"),
            raw("Title", "string", "Article title."),
            raw("Author", "string", "Article author."),
            raw("Series", "string", "Publication series."),
            raw("PublishedOn", "date", "Publication date."),
            raw("LocalFileName", "string", "File name under bootstrap/git-ignored-articles/ (never committed)."),
            raw("Thesis", "string", "The article's thesis, paraphrased."),
            agg("ClaimCount", "integer", "Claims inventoried from this article.",
                "=COUNTIFS(ArticleClaims!{{SourceArticle}}, {{SourceArticleId}})"),
            agg("CoveredClaimCount", "integer", "Claims with at least one valid piece of evidence.",
                "=COUNTIFS(ArticleClaims!{{SourceArticle}}, {{SourceArticleId}}, ArticleClaims!{{IsCovered}}, TRUE)"),
            agg("AgreedClaimCount", "integer", "Covered claims whose evidence every graded substrate agrees on.",
                "=COUNTIFS(ArticleClaims!{{SourceArticle}}, {{SourceArticleId}}, ArticleClaims!{{IsAgreed}}, TRUE)"),
            calc("UncoveredClaimCount", "integer", "Claims with no valid evidence.",
                 "={{ClaimCount}} - {{CoveredClaimCount}}"),
            calc("CoveragePercent", "number", "Share of this article's claims that are covered.",
                 "=IF({{ClaimCount}} = 0, 0, ROUND(100 * {{CoveredClaimCount}} / {{ClaimCount}}, 1))"),
            calc("AgreedCoveragePercent", "number", "Share of claims covered by evidence no graded substrate disputes.",
                 "=IF({{ClaimCount}} = 0, 0, ROUND(100 * {{AgreedClaimCount}} / {{ClaimCount}}, 1))"),
            calc("IsFullyCovered", "boolean", "TRUE when every claim in this article is covered.",
                 "=AND({{ClaimCount}} > 0, {{UncoveredClaimCount}} = 0)"),
            raw("SemanticTypeIri", "string", "Semantic type IRI."),
        ], "coverage")

    # ---- ArticleClaims --------------------------------------------------------
    ensure_table(rb, "ArticleClaims",
        "One row per thing a source article says a process-knowledge system must represent, do, answer "
        "or use. Paraphrased with a section reference; never quoted.",
        [
            raw("ArticleClaimId", "string", "Stored identifier.", nullable=False),
            calc("Name", "string", "Display name.", '={{ArticleClaimId}} & ": " & LEFT({{ClaimText}}, 60)'),
            rel("SourceArticle", "SourceArticles", "Article the claim comes from."),
            raw("ClaimKind", "string", "Concept | Prescription | CompetencyQuestion | Standard | Scenario | Illustration."),
            raw("SectionRef", "string", "Section of the article where the claim is made."),
            raw("ClaimText", "string", "The claim, paraphrased."),
            calc("RequiredEvidence", "string", "The evidence this kind of claim demands, stated in words.",
                 '=IF({{ClaimKind}} = "Concept", "a table with rows or a field with data", '
                 'IF({{ClaimKind}} = "CompetencyQuestion", "an answered role question with a computed answer", '
                 'IF({{ClaimKind}} = "Standard", "a mapped ontology profile or an applied knowledge method", '
                 'IF({{ClaimKind}} = "Scenario", "a procedure with recorded executions", '
                 '"a discriminating witness invented for a role question"))))'),
            agg("EvidenceCount", "integer", "Evidence rows offered for this claim.",
                "=COUNTIFS(ClaimEvidence!{{ArticleClaim}}, {{ArticleClaimId}})"),
            agg("ValidEvidenceCount", "integer", "Evidence rows that meet the bar for this claim's kind.",
                "=COUNTIFS(ClaimEvidence!{{ArticleClaim}}, {{ArticleClaimId}}, ClaimEvidence!{{IsValid}}, TRUE)"),
            agg("AgreedEvidenceCount", "integer", "Valid evidence rows no graded substrate disputes.",
                "=COUNTIFS(ClaimEvidence!{{ArticleClaim}}, {{ArticleClaimId}}, ClaimEvidence!{{IsAgreedEvidence}}, TRUE)"),
            calc("IsCovered", "boolean", "TRUE when at least one evidence row meets the bar.",
                 "={{ValidEvidenceCount}} > 0"),
            calc("IsAgreed", "boolean", "TRUE when at least one valid evidence row is undisputed by every graded substrate.",
                 "={{AgreedEvidenceCount}} > 0"),
            calc("HasRejectedEvidence", "boolean", "TRUE when evidence was offered and none of it meets the bar.",
                 "=AND({{EvidenceCount}} > 0, {{ValidEvidenceCount}} = 0)"),
            raw("SemanticTypeIri", "string", "Semantic type IRI."),
        ], "coverage")

    # ---- ClaimEvidence --------------------------------------------------------
    ensure_table(rb, "ClaimEvidence",
        "A proposed piece of evidence that the model covers a claim, with the justification a skeptic "
        "can audit. Whether it counts is computed in IsValid from measured facts, never asserted.",
        [
            raw("ClaimEvidenceId", "string", "Stored identifier.", nullable=False),
            calc("Name", "string", "Display name.", '={{ArticleClaim}} & " <- " & {{EvidenceKind}}'),
            rel("ArticleClaim", "ArticleClaims", "The claim this evidence supports."),
            raw("EvidenceKind", "string", "Field | Table | RoleQuestion | OntologyProfile | KnowledgeMethod | Procedure."),
            rel("RulebookField", "RulebookFields", "Evidence field, when EvidenceKind is Field."),
            rel("RulebookTable", "RulebookTables", "Evidence table, when EvidenceKind is Table."),
            rel("RoleQuestion", "RoleQuestions", "Evidence question, when EvidenceKind is RoleQuestion."),
            rel("OntologyProfile", "OntologyProfiles", "Evidence profile, when EvidenceKind is OntologyProfile."),
            rel("KnowledgeMethod", "KnowledgeMethods", "Evidence method, when EvidenceKind is KnowledgeMethod."),
            rel("Procedure", "Procedures", "Evidence procedure, when EvidenceKind is Procedure."),
            raw("Justification", "string", "How this evidence answers the claim, in words a skeptic can check."),
            lookup("ClaimKind", "string", "Kind of the claim this evidence supports.",
                   idx("ArticleClaims", "ClaimKind", "ArticleClaim")),
            lookup("FieldCatalogName", "string", "The evidence field's name as the catalog records it; blank when the field does not exist.",
                   idx("RulebookFields", "FieldName", "RulebookField")),
            lookup("FieldIsWitness", "boolean", "Whether the evidence field was invented for a role question.",
                   idx("RulebookFields", "IsWitness", "RulebookField")),
            lookup("FieldHasData", "boolean", "Whether Postgres holds data for the evidence field.",
                   idx("RulebookFields", "HasMeasuredData", "RulebookField")),
            lookup("FieldIsDiscriminating", "boolean", "Whether the evidence field tells rows apart.",
                   idx("RulebookFields", "IsDiscriminating", "RulebookField")),
            lookup("FieldIsContested", "boolean", "Whether a graded substrate disagrees on the evidence field.",
                   idx("RulebookFields", "IsSubstrateContested", "RulebookField")),
            lookup("TableHasRows", "boolean", "Whether Postgres holds rows of the evidence table.",
                   idx("RulebookTables", "HasMeasuredRows", "RulebookTable")),
            lookup("QuestionIsAnswered", "boolean", "Whether the evidence question has predicates.",
                   idx("RoleQuestions", "IsAnswered", "RoleQuestion")),
            lookup("QuestionWitnessedAnswer", "string", "The substrate-computed answer of the evidence question.",
                   idx("RoleQuestions", "WitnessedAnswer", "RoleQuestion")),
            lookup("ProfileMappingCount", "integer", "Semantic mappings using the evidence profile.",
                   idx("OntologyProfiles", "MappingCount", "OntologyProfile")),
            lookup("MethodIsApplied", "boolean", "Whether a seeded row applies the evidence method.",
                   idx("KnowledgeMethods", "IsApplied", "KnowledgeMethod")),
            lookup("ProcedureExecutionCount", "integer", "Recorded executions of the evidence procedure.",
                   idx("Procedures", "ExecutionCount", "Procedure")),
            calc("HasJustification", "boolean", "TRUE when the justification is written.",
                 '={{Justification}} <> ""'),
            calc("IsWitnessProof", "boolean", "Evidence is an existing field invented for a role question that discriminates.",
                 '=AND({{EvidenceKind}} = "Field", {{FieldCatalogName}} <> "", {{FieldIsWitness}}, {{FieldIsDiscriminating}})'),
            calc("IsStructuralProof", "boolean", "Evidence is a table with rows or a field with data.",
                 '=OR(AND({{EvidenceKind}} = "Table", {{TableHasRows}}), AND({{EvidenceKind}} = "Field", {{FieldCatalogName}} <> "", {{FieldHasData}}))'),
            calc("IsQuestionProof", "boolean", "Evidence is an answered role question with a computed answer.",
                 '=AND({{EvidenceKind}} = "RoleQuestion", {{QuestionIsAnswered}}, {{QuestionWitnessedAnswer}} <> "")'),
            calc("IsStandardProof", "boolean", "Evidence is a mapped ontology profile or an applied knowledge method.",
                 '=OR(AND({{EvidenceKind}} = "OntologyProfile", {{ProfileMappingCount}} > 0), AND({{EvidenceKind}} = "KnowledgeMethod", {{MethodIsApplied}}))'),
            calc("IsScenarioProof", "boolean", "Evidence is a procedure with recorded executions.",
                 '=AND({{EvidenceKind}} = "Procedure", {{ProcedureExecutionCount}} > 0)'),
            calc("IsValid", "boolean",
                 "TRUE when the evidence is justified and is the kind of proof this claim's kind demands.",
                 '=AND({{HasJustification}}, OR('
                 'AND(OR({{ClaimKind}} = "Prescription", {{ClaimKind}} = "Illustration"), {{IsWitnessProof}}), '
                 'AND({{ClaimKind}} = "Concept", {{IsStructuralProof}}), '
                 'AND({{ClaimKind}} = "CompetencyQuestion", {{IsQuestionProof}}), '
                 'AND({{ClaimKind}} = "Standard", {{IsStandardProof}}), '
                 'AND({{ClaimKind}} = "Scenario", {{IsScenarioProof}})))'),
            calc("IsContested", "boolean", "TRUE when the evidence field is disputed by a graded substrate.",
                 '=AND({{EvidenceKind}} = "Field", {{FieldIsContested}})'),
            calc("IsAgreedEvidence", "boolean", "TRUE when the evidence is valid and no graded substrate disputes it.",
                 "=AND({{IsValid}}, {{IsContested}} = FALSE)"),
            raw("SemanticTypeIri", "string", "Semantic type IRI."),
        ], "coverage")
    return extra_cols


def restore_extra_columns(rb, extra_cols):
    for owned, cols in extra_cols.items():
        have = {f["name"] for f in rb[owned]["schema"]}
        tail = [f for f in rb[owned]["schema"] if f["name"] == "SemanticTypeIri"]
        body = [f for f in rb[owned]["schema"] if f["name"] != "SemanticTypeIri"]
        body += [f for f in cols if f["name"] not in have]
        rb[owned]["schema"] = body + tail
    return rb


def seed(rb):
    upsert_rows(rb, "SourceArticles", [OrderedDict([
        ("SourceArticleId", a[0]), ("Title", a[1]), ("Author", "Jessica Talisman"), ("Series", a[2]),
        ("PublishedOn", a[3]), ("LocalFileName", a[4]), ("Thesis", a[5]),
        ("SemanticTypeIri", f"{EXT}SourceArticle")]) for a in article_claims.ARTICLES])

    # Claims are authored in article_claims.py; the table mirrors it exactly, so a claim
    # removed or renumbered after an audit cannot linger as a stale row.
    rb["ArticleClaims"]["data"] = [OrderedDict([
        ("ArticleClaimId", c[0]), ("SourceArticle", c[1]), ("ClaimKind", c[2]), ("SectionRef", c[3]),
        ("ClaimText", c[4]), ("SemanticTypeIri", f"{EXT}ArticleClaim")]) for c in article_claims.CLAIMS]

    upsert_rows(rb, "KnowledgeMethods", [OrderedDict([
        ("KnowledgeMethodId", m[0]), ("Label", m[1]), ("MethodFamily", m[2]), ("Summary", m[3]),
        ("OriginReference", m[4]), ("SemanticTypeIri", f"{EXT}KnowledgeMethod")])
        for m in article_evidence.METHODS])

    claim_ids = {c[0] for c in article_claims.CLAIMS}
    evidence = dict(article_evidence.EVIDENCE)
    loops_dir = Path(__file__).resolve().parent / "loops"
    only = {t for t in os.environ.get("PKO_EVIDENCE_THEMES", "").split(",") if t}
    for path in sorted(loops_dir.glob("evidence_*.py")):
        if only and path.stem.removeprefix("evidence_") not in only:
            continue
        spec = importlib.util.spec_from_file_location(path.stem, path)
        mod = importlib.util.module_from_spec(spec)
        spec.loader.exec_module(mod)
        for cid, items in mod.EVIDENCE.items():
            if cid in evidence:
                raise SystemExit(f"{cid} has evidence in two modules ({path.name} and an earlier one)")
            evidence[cid] = items
    targets = {
        "Field": {f"{t}.{f['name']}" for t, v in rb.items() if isinstance(v, dict) and "schema" in v
                  for f in v["schema"]},
        "Table": {t for t, v in rb.items() if isinstance(v, dict) and "schema" in v},
        "RoleQuestion": {r["RoleQuestionId"] for r in rb["RoleQuestions"]["data"]},
        "OntologyProfile": {r["OntologyProfileId"] for r in rb["OntologyProfiles"]["data"]},
        "KnowledgeMethod": {m[0] for m in article_evidence.METHODS} | {r["KnowledgeMethodId"] for r in rb["KnowledgeMethods"]["data"]},
        "Procedure": {r["ProcedureId"] for r in rb["Procedures"]["data"]},
    }
    rows = []
    for claim_id, items in evidence.items():
        if claim_id not in claim_ids:
            raise SystemExit(f"evidence names unknown claim {claim_id}")
        for n, (kind, target, justification) in enumerate(items, 1):
            if kind not in EVIDENCE_TARGET:
                raise SystemExit(f"{claim_id}: unknown evidence kind {kind}")
            if target not in targets[kind]:
                raise SystemExit(f"{claim_id}: {kind} {target!r} does not exist in the rulebook")
            row = OrderedDict([("ClaimEvidenceId", f"{claim_id}-e{n}"), ("ArticleClaim", claim_id),
                               ("EvidenceKind", kind)])
            for col in EVIDENCE_TARGET.values():
                row[col] = target if EVIDENCE_TARGET[kind] == col else None
            row["Justification"] = justification
            row["SemanticTypeIri"] = f"{EXT}ClaimEvidence"
            rows.append(row)
    # Evidence is authored in article_evidence.py; the table mirrors it exactly.
    rb["ClaimEvidence"]["data"] = rows

    for sm_id, path in (("map-source-articles", "SourceArticles"), ("map-article-claims", "ArticleClaims"),
                        ("map-claim-evidence", "ClaimEvidence"), ("map-knowledge-methods", "KnowledgeMethods")):
        mapping(rb, sm_id, path, "class", f"{EXT}{path[:-1] if path.endswith('s') else path}",
                "extension", "erb-pko-extension-1.0.0",
                "Article-coverage ledger; not part of PKO 2.0.0.")
    return len(rows)


def main() -> int:
    rb = load()
    extra = schema_changes(rb)
    restore_extra_columns(rb, extra)
    n = seed(rb)
    dump(rb)
    print(f"article coverage ledger: {len(article_claims.CLAIMS)} claims, {n} evidence rows, "
          f"{len(article_evidence.METHODS)} knowledge methods")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
