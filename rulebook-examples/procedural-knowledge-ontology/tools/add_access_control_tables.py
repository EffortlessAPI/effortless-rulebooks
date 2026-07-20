#!/usr/bin/env python3
"""Add the access-control layer to the PKO rulebook.

Targeted insertion ONLY: reads the rulebook, adds exactly the 7 access-control
table keys, writes it back. Never rewrites or reorders pre-existing keys, so a
concurrent agent editing other tables is not clobbered.

Refuses to run if any target key already exists (no silent overwrite).
"""
import json, sys, os, shutil

RB = "effortless-rulebook/procedural-knowledge-ontology-rulebook.json"
IRI = "urn:effortless:pko-extension#"

def f(name, dt, typ, nullable=True, desc="", formula=None, related=None):
    d = {"name": name, "datatype": dt, "type": typ, "nullable": nullable, "Description": desc}
    if formula: d["formula"] = formula
    if related: d["RelatedTo"] = related
    return d

TABLES = {}

# ---------------------------------------------------------------- RulebookTables
TABLES["RulebookTables"] = {
    "Description": "Census of every table in this rulebook. The table-level counterpart to RulebookFields, and the anchor every access policy points at. Derived by tools/reconcile_field_catalog.py -- never hand-maintained.",
    "important": True,
    "schema": [
        f("TableName","string","raw",False,"Stored logical identifier: the rulebook table name, e.g. 'Procedures'."),
        f("Name","string","calculated",True,"Human-readable calculated display alias.","={{TableName}}"),
        f("PhysicalTable","string","raw",True,"snake_case Postgres base table name emitted by rulebook-to-postgres."),
        f("PhysicalView","string","raw",True,"snake_case Postgres view name (vw_*) carrying the computed columns."),
        f("SubjectArea","string","raw",True,"Coarse grouping used to organise role schemas, e.g. 'execution', 'governance'."),
        f("IsExtension","boolean","raw",True,"True when this table is an ERB extension rather than a native/aligned PKO term."),
        f("FieldCount","number","aggregation",True,"Number of catalogued fields on this table.","=COUNTIFS(RulebookFields!{{TargetTable}}, {{TableName}})"),
        f("PolicyCount","number","aggregation",True,"Number of access policies targeting this table.","=COUNTIFS(AccessPolicies!{{TargetTable}}, {{TableName}})"),
        f("IsUnsecured","boolean","calculated",True,"True when RLS is enabled but no policy targets the table, so every principal sees zero rows. A fail-closed table nobody has granted access to.","={{PolicyCount}} = 0"),
        f("SemanticTypeIri","string","raw",True,"Semantic type IRI.")
    ],
    "data": []
}

# ------------------------------------------------------------- AccessPrincipals
TABLES["AccessPrincipals"] = {
    "Description": "Security principals -- the identities policies attach to. A principal is the console persona a person logs in as; it maps many-to-one onto a domain Role, so 'who may see this row' is expressed once against the domain vocabulary while the UI keeps its own persona names. Each principal owns exactly one Postgres role and one Postgres schema.",
    "important": True,
    "schema": [
        f("AccessPrincipalId","string","raw",False,"Stored logical identifier, e.g. 'principal-controller'."),
        f("Name","string","calculated",True,"Human-readable calculated display alias.","={{Label}}"),
        f("Label","string","raw",True,"Display label shown in the console role picker."),
        f("DomainRole","string","relationship",True,"Domain role whose authority this principal exercises.","Roles"),
        f("PgRoleName","string","raw",True,"Postgres role name this principal authenticates as, e.g. 'pko_controller'."),
        f("SchemaName","string","raw",True,"Postgres schema that is this principal's entire visible world, e.g. 'pko_controller'."),
        f("IsAdministrator","boolean","raw",True,"True when this principal may read every table and edit access policy."),
        f("OrganizationScope","string","lookup",True,"Organization inherited from the domain role; the default tenancy boundary for row predicates.","=INDEX(Roles!{{Organization}}, MATCH({{DomainRole}}, Roles!{{RoleId}}, 0))"),
        f("RoleLabel","string","lookup",True,"Label of the domain role, for display.","=INDEX(Roles!{{Label}}, MATCH({{DomainRole}}, Roles!{{RoleId}}, 0))"),
        f("PolicyCount","number","aggregation",True,"Number of row policies granted to this principal.","=COUNTIFS(AccessPolicies!{{Principal}}, {{AccessPrincipalId}})"),
        f("GrantCount","number","aggregation",True,"Number of field grants held by this principal.","=COUNTIFS(FieldGrants!{{Principal}}, {{AccessPrincipalId}})"),
        f("VisibleTableCount","number","aggregation",True,"Number of tables exposed in this principal's schema.","=COUNTIFS(RoleSchemaViews!{{Principal}}, {{AccessPrincipalId}})"),
        f("HasNoAccess","boolean","calculated",True,"True when the principal holds no policies at all, so its schema is empty and it can read nothing. Fail-closed by construction.","={{PolicyCount}} = 0"),
        f("IsOverPrivileged","boolean","calculated",True,"True when a non-administrator principal can reach every table in the rulebook -- an admin-equivalent principal that was never declared as one.","=AND(NOT({{IsAdministrator}}), {{VisibleTableCount}} >= 74)"),
        f("SemanticTypeIri","string","raw",True,"Semantic type IRI.")
    ],
    "data": []
}

# ---------------------------------------------------------------- AccessPolicies
TABLES["AccessPolicies"] = {
    "Description": "Row-level security policies: the VERTICAL cut. One row per principal x table x command, carrying the predicate that decides which rows are visible. RowPredicate is emitted verbatim into a Postgres USING clause, so it may call any SECURITY DEFINER calc_* function and therefore reference inference fields many hops down the DAG.",
    "important": True,
    "schema": [
        f("AccessPolicyId","string","raw",False,"Stored logical identifier, e.g. 'pol-controller-procedures-select'."),
        f("Name","string","calculated",True,"Human-readable calculated display alias.","={{Principal}} & \" \" & {{Command}} & \" \" & {{TargetTable}}"),
        f("Principal","string","relationship",True,"Principal this policy grants to.","AccessPrincipals"),
        f("TargetTable","string","relationship",True,"Rulebook table this policy guards.","RulebookTables"),
        f("Command","string","raw",True,"SQL command the policy governs: SELECT, INSERT, UPDATE, DELETE or ALL."),
        f("RowPredicate","string","raw",True,"SQL boolean expression emitted into USING(...). Empty means all rows of the table. Must not sub-select the guarded table -- Postgres raises infinite recursion; route such predicates through a SECURITY DEFINER function instead."),
        f("CheckPredicate","string","raw",True,"SQL boolean expression emitted into WITH CHECK(...) for write commands. Empty reuses RowPredicate."),
        f("Rationale","string","raw",True,"Why this principal is entitled to these rows, in the granting authority's words."),
        f("ReferencesInference","boolean","raw",True,"True when RowPredicate calls a calc_* function, i.e. the cut depends on a derived field rather than a stored column."),
        f("IsWriteCommand","boolean","calculated",True,"True when this policy governs a mutating command.","=OR({{Command}} = \"INSERT\", {{Command}} = \"UPDATE\", {{Command}} = \"DELETE\", {{Command}} = \"ALL\")"),
        f("IsUnrestricted","boolean","calculated",True,"True when the policy carries no predicate, exposing every row of the target table to the principal.","={{RowPredicate}} = \"\""),
        f("PrincipalIsAdmin","boolean","lookup",True,"Whether the granted principal is an administrator.","=INDEX(AccessPrincipals!{{IsAdministrator}}, MATCH({{Principal}}, AccessPrincipals!{{AccessPrincipalId}}, 0))"),
        f("IsUnrestrictedNonAdminGrant","boolean","calculated",True,"True when a non-administrator principal is granted an unrestricted policy -- a whole-table exposure that no row predicate narrows. The single highest-signal privilege-escalation witness in the model.","=AND({{IsUnrestricted}}, NOT({{PrincipalIsAdmin}}))"),
        f("IsUnwitnessedWrite","boolean","calculated",True,"True when a write policy has no denial test proving it refuses out-of-scope rows. An untested write grant is an assertion, not evidence.","=AND({{IsWriteCommand}}, {{DenialTestCount}} = 0)"),
        f("DenialTestCount","number","aggregation",True,"Number of denial tests seeded against this policy.","=COUNTIFS(AccessDenialTests!{{TargetPolicy}}, {{AccessPolicyId}})"),
        f("SemanticTypeIri","string","raw",True,"Semantic type IRI.")
    ],
    "data": []
}

# ------------------------------------------------------------------- FieldGrants
TABLES["FieldGrants"] = {
    "Description": "Field-level grants: the HORIZONTAL cut. One row per principal x field. A field with no grant row is not filtered from the principal's view -- it is absent from it, so the column does not exist as far as that principal's SQL is concerned.",
    "important": True,
    "schema": [
        f("FieldGrantId","string","raw",False,"Stored logical identifier, e.g. 'fg-controller-Procedures.Title'."),
        f("Name","string","calculated",True,"Human-readable calculated display alias.","={{Principal}} & \" -> \" & {{TargetField}}"),
        f("Principal","string","relationship",True,"Principal receiving the grant.","AccessPrincipals"),
        f("TargetField","string","relationship",True,"Catalogued field being exposed.","RulebookFields"),
        f("CanRead","boolean","raw",True,"True when the principal may read this field."),
        f("CanWrite","boolean","raw",True,"True when the principal may write this field."),
        f("MaskStrategy","string","raw",True,"How the value is presented when read: 'plain', 'redacted' or 'hashed'."),
        f("FieldTable","string","lookup",True,"Table the granted field belongs to.","=INDEX(RulebookFields!{{TargetTable}}, MATCH({{TargetField}}, RulebookFields!{{RulebookFieldId}}, 0))"),
        f("FieldName","string","lookup",True,"Name of the granted field.","=INDEX(RulebookFields!{{FieldName}}, MATCH({{TargetField}}, RulebookFields!{{RulebookFieldId}}, 0))"),
        f("FieldIsDerived","boolean","lookup",True,"Whether the granted field is a derived (calculated/lookup/aggregation) field.","=INDEX(RulebookFields!{{IsDerived}}, MATCH({{TargetField}}, RulebookFields!{{RulebookFieldId}}, 0))"),
        f("IsWritableDerivedField","boolean","calculated",True,"True when a derived field has been granted write access. Derived fields are computed by the substrate and cannot be written -- such a grant is incoherent and must be corrected.","=AND({{CanWrite}}, {{FieldIsDerived}})"),
        f("IsMasked","boolean","calculated",True,"True when the value is transformed rather than shown verbatim.","=AND({{MaskStrategy}} <> \"plain\", {{MaskStrategy}} <> \"\")"),
        f("GrantKeyWhenReadable","string","calculated",True,"Composite echo of principal and table, blank unless readable. Enables single-criterion COUNTIFS rollups of readable columns per principal per table, per the documented multi-criteria COUNTIFS defect.","=IF({{CanRead}}, {{Principal}} & \"|\" & {{FieldTable}}, \"\")"),
        f("SemanticTypeIri","string","raw",True,"Semantic type IRI.")
    ],
    "data": []
}

# ------------------------------------------------------------------ RoleSchemas
TABLES["RoleSchemas"] = {
    "Description": "One Postgres schema per principal -- the principal's entire visible world. The schema is the only entry on that principal's search_path, so a table absent from it cannot be named at all.",
    "important": True,
    "schema": [
        f("RoleSchemaId","string","raw",False,"Stored logical identifier, e.g. 'schema-controller'."),
        f("Name","string","calculated",True,"Human-readable calculated display alias.","={{SchemaName}}"),
        f("Principal","string","relationship",True,"Principal that owns this schema.","AccessPrincipals"),
        f("SchemaName","string","raw",True,"Postgres schema name, e.g. 'pko_controller'."),
        f("SearchPath","string","calculated",True,"search_path set for this principal's sessions. The principal's own schema only -- public is deliberately excluded so base tables cannot be named.","={{SchemaName}}"),
        f("IsSealed","boolean","raw",True,"True when the principal may not create objects in its own schema."),
        f("ViewCount","number","aggregation",True,"Number of views exposed in this schema.","=COUNTIFS(RoleSchemaViews!{{RoleSchema}}, {{RoleSchemaId}})"),
        f("IsEmptySchema","boolean","calculated",True,"True when the schema exposes no views, so the principal can read nothing at all.","={{ViewCount}} = 0"),
        f("SemanticTypeIri","string","raw",True,"Semantic type IRI.")
    ],
    "data": []
}

# -------------------------------------------------------------- RoleSchemaViews
TABLES["RoleSchemaViews"] = {
    "Description": "The emitted views: one per principal x table. ColumnList is DERIVED from FieldGrants, so toggling a single grant changes the emitted DDL with no second edit anywhere. This is what makes an admin's save reshape the database without touching UI code.",
    "important": True,
    "schema": [
        f("RoleSchemaViewId","string","raw",False,"Stored logical identifier, e.g. 'rsv-controller-procedures'."),
        f("Name","string","calculated",True,"Human-readable calculated display alias.","={{SchemaName}} & \".\" & {{ViewName}}"),
        f("RoleSchema","string","relationship",True,"Schema this view is emitted into.","RoleSchemas"),
        f("Principal","string","relationship",True,"Principal that will read this view.","AccessPrincipals"),
        f("TargetTable","string","relationship",True,"Rulebook table this view exposes.","RulebookTables"),
        f("ViewName","string","raw",True,"Unqualified view name inside the principal's schema, e.g. 'procedures'."),
        f("SchemaName","string","lookup",True,"Schema name, from the owning RoleSchemas row.","=INDEX(RoleSchemas!{{SchemaName}}, MATCH({{RoleSchema}}, RoleSchemas!{{RoleSchemaId}}, 0))"),
        f("SourceView","string","lookup",True,"Underlying computed view this narrows, e.g. 'vw_procedures'.","=INDEX(RulebookTables!{{PhysicalView}}, MATCH({{TargetTable}}, RulebookTables!{{TableName}}, 0))"),
        f("GrantKey","string","calculated",True,"Composite key matching FieldGrants.GrantKeyWhenReadable, used to roll up this view's readable column count.","={{Principal}} & \"|\" & {{TargetTable}}"),
        f("ColumnCount","number","aggregation",True,"Number of columns exposed, derived live from the principal's readable field grants. Change one grant and this view's shape changes.","=COUNTIFS(FieldGrants!{{GrantKeyWhenReadable}}, {{GrantKey}})"),
        f("TableFieldCount","number","lookup",True,"Total catalogued fields on the target table.","=INDEX(RulebookTables!{{FieldCount}}, MATCH({{TargetTable}}, RulebookTables!{{TableName}}, 0))"),
        f("IsFullWidth","boolean","calculated",True,"True when every field on the table is exposed, so the horizontal cut removes nothing.","=AND({{ColumnCount}} > 0, {{ColumnCount}} >= {{TableFieldCount}})"),
        f("IsDegenerateView","boolean","calculated",True,"True when the view exposes zero columns -- an emitted view that cannot be selected from. A generator that emits this has produced invalid DDL.","={{ColumnCount}} = 0"),
        f("SemanticTypeIri","string","raw",True,"Semantic type IRI.")
    ],
    "data": []
}

# ------------------------------------------------------------ JwtClaimMappings
TABLES["JwtClaimMappings"] = {
    "Description": "Maps verified JWT claims onto the SQL accessors row predicates call. Magic-links is the notary: it asserts only that the bearer controls an email address. This table records how that verified email, and any additional claims, become values a policy can test.",
    "important": True,
    "schema": [
        f("JwtClaimMappingId","string","raw",False,"Stored logical identifier, e.g. 'claim-email'."),
        f("Name","string","calculated",True,"Human-readable calculated display alias.","={{ClaimName}} & \" -> \" & {{SqlAccessor}}"),
        f("ClaimName","string","raw",True,"Claim key as it appears in the verified JWT payload, e.g. 'email'."),
        f("SqlAccessor","string","raw",True,"SQL function a policy calls to read the claim, e.g. 'app.jwt_email()'."),
        f("IsReservedClaim","boolean","raw",True,"True for claims magic-links controls and an app cannot override: email, iss, iat, nbf, exp, sub, tenant_id."),
        f("MapsToPrincipal","boolean","raw",True,"True when this claim is what resolves the caller to an AccessPrincipals row."),
        f("Description2","string","raw",True,"What the claim asserts and who vouches for it."),
        f("UsageCount","number","aggregation",True,"Number of policies whose predicate calls this accessor.","=COUNTIFS(AccessPolicies!{{RowPredicate}}, {{SqlAccessor}})"),
        f("SemanticTypeIri","string","raw",True,"Semantic type IRI.")
    ],
    "data": []
}

# ------------------------------------------------------------ AccessDenialTests
TABLES["AccessDenialTests"] = {
    "Description": "Denial witnesses. A policy with no failing case seeded against it is an assertion, not evidence -- the same acceptance bar the rest of this rulebook holds. Each row names a principal, a query, and the row that MUST NOT come back, so a policy that silently stops enforcing is caught by a red test rather than by an incident.",
    "important": True,
    "schema": [
        f("AccessDenialTestId","string","raw",False,"Stored logical identifier, e.g. 'deny-analyst-other-org-procedures'."),
        f("Name","string","calculated",True,"Human-readable calculated display alias.","={{Principal}} & \" must not see \" & {{ForbiddenRowId}}"),
        f("TargetPolicy","string","relationship",True,"Policy this test exercises.","AccessPolicies"),
        f("Principal","string","relationship",True,"Principal the query runs as.","AccessPrincipals"),
        f("TargetTable","string","relationship",True,"Table queried.","RulebookTables"),
        f("ForbiddenRowId","string","raw",True,"Primary key of the row that must be invisible to this principal."),
        f("ExpectedVisible","boolean","raw",True,"False for a denial test: the row must not appear. True asserts a row the principal is entitled to does appear."),
        f("ObservedVisible","boolean","raw",True,"What the substrate actually returned on the last run. Written back by the verifier, never hand-set."),
        f("LastRunAt","datetime","raw",True,"When this test was last executed against Postgres."),
        f("HasRun","boolean","calculated",True,"True once the test has been executed at least once.","={{LastRunAt}} <> \"\""),
        f("IsPassing","boolean","calculated",True,"True when observed visibility matches expectation.","={{ObservedVisible}} = {{ExpectedVisible}}"),
        f("IsLeak","boolean","calculated",True,"True when a row that must be invisible was returned. A confirmed access-control breach.","=AND(NOT({{ExpectedVisible}}), {{ObservedVisible}})"),
        f("IsUnproven","boolean","calculated",True,"True when the test has never run, so it proves nothing regardless of how it is written.","=NOT({{HasRun}})"),
        f("SemanticTypeIri","string","raw",True,"Semantic type IRI.")
    ],
    "data": []
}

for t in TABLES.values():
    for fld in t["schema"]:
        if fld["name"] == "SemanticTypeIri":
            pass

def main():
    if not os.path.exists(RB):
        sys.exit(f"FATAL: rulebook not found at {RB}")
    with open(RB) as fh:
        rb = json.load(fh)

    existing = [k for k in TABLES if k in rb]
    if existing:
        sys.exit(f"FATAL: these tables already exist, refusing to overwrite: {existing}")

    before = set(rb.keys())
    for name, tbl in TABLES.items():
        tbl_iri = IRI + name.rstrip("s") if False else None
        rb[name] = tbl

    added = set(rb.keys()) - before
    if added != set(TABLES):
        sys.exit(f"FATAL: key delta mismatch: {added}")

    shutil.copy(RB, RB + ".bak-access")
    tmp = RB + ".tmp"
    with open(tmp, "w") as fh:
        json.dump(rb, fh, indent=1, ensure_ascii=False)
    os.replace(tmp, RB)
    print(f"OK: added {len(TABLES)} tables -> {sorted(TABLES)}")
    print(f"total tables now: {len([k for k in rb if isinstance(rb[k],dict) and 'schema' in rb[k]])}")

if __name__ == "__main__":
    main()
