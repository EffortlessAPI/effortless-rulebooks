#!/usr/bin/env python3
"""Seed the access-control layer.

Derives RulebookTables from the live rulebook + Postgres, then seeds one
principal per domain Role, a schema each, and policies/grants that express
real, defensible entitlements for this domain.

Targeted: only writes the 8 access-control table `data` arrays (plus
RulebookTables schema-derived rows). Never touches another table.
"""
import json, os, re, subprocess, sys

RB = "effortless-rulebook/procedural-knowledge-ontology-rulebook.json"
DB = os.environ.get("DATABASE_URL",
     "postgresql://postgres@localhost:5432/erb_procedural_knowledge_ontology")
IRI = "urn:effortless:pko-extension#"

def snake(n):
    s = re.sub(r'(?<!^)(?=[A-Z])', '_', n).lower()
    return re.sub(r'_+', '_', s)

def psql(q):
    r = subprocess.run(["psql", DB, "-tAF\x1f", "-c", q],
                       capture_output=True, text=True)
    if r.returncode != 0:
        raise SystemExit(f"FATAL psql: {r.stderr.strip()}")
    return [l.split("\x1f") for l in r.stdout.strip().split("\n") if l]

rb = json.load(open(RB))
TABLES = [k for k in rb if isinstance(rb[k], dict) and "schema" in rb[k]]

# ---- ground truth from Postgres: which physical tables/views actually exist
real_tables = {r[0] for r in psql(
    "select table_name from information_schema.tables where table_schema='public'")}
real_views  = {r[0] for r in psql(
    "select table_name from information_schema.views where table_schema='public'")}

SUBJECT = [
 (("Procedure","Step","Action","Function","Tool","Transition"), "specification"),
 (("Execution","Observed","Error","Issue","Satisfaction","Outcome"), "execution"),
 (("Role","Agent","Organization","Assignment","Mentor","Communit"), "org"),
 (("Requirement","Verification","Exception","Rationale","Authority"), "governance"),
 (("Knowledge","Elicitation","FAQ","Explanation","Gap","Fragment"), "knowledge"),
 (("Communication","Message","Recipient","Send","Template","Delivered"), "communication"),
 (("Access","Field","Schema","Jwt","Denial","Policy"), "access-control"),
 (("App","Nav","Route"), "application"),
 (("Test","Witness","RoleQuestion","Rulebook","Semantic","Evaluation","ERB","__meta__"), "meta"),
]
def subject(t):
    for keys, name in SUBJECT:
        if any(k in t for k in keys): return name
    return "domain"

EXT_PREFIXES = ("Access","FieldGrants","RoleSchema","JwtClaim","Rulebook","Witness",
                "RoleQuestion","Knowledge","Elicitation","Stewardship","Operational",
                "App","Test","ERB","__meta__","Evaluation","Semantic")

# ---------------------------------------------------------- RulebookTables rows
rt_rows = []
for t in sorted(TABLES):
    phys, view = snake(t), "vw_" + snake(t)
    rt_rows.append({
        "TableName": t,
        "PhysicalTable": phys if phys in real_tables else None,
        "PhysicalView":  view if view in real_views else None,
        "SubjectArea": subject(t),
        "IsExtension": t.startswith(EXT_PREFIXES),
        "SemanticTypeIri": IRI + "RulebookTable",
    })

# --------------------------------------------------------- principals + schemas
roles = {r["RoleId"]: r for r in rb["Roles"]["data"]}
ADMIN_ROLES = {"process-steward", "knowledge-authority"}

principals, schemas = [], []
for rid, r in roles.items():
    pid  = f"principal-{rid}"
    pg   = "pko_" + rid.replace("-", "_")
    principals.append({
        "AccessPrincipalId": pid,
        "Label": r.get("Label") or rid,
        "DomainRole": rid,
        "PgRoleName": pg,
        "SchemaName": pg,
        "IsAdministrator": rid in ADMIN_ROLES,
        "SemanticTypeIri": IRI + "AccessPrincipal",
    })
    schemas.append({
        "RoleSchemaId": f"schema-{rid}",
        "Principal": pid,
        "SchemaName": pg,
        "IsSealed": True,
        "SemanticTypeIri": IRI + "RoleSchema",
    })

# ----------------------------------------------------------------- JWT claims
jwt_rows = [
 {"JwtClaimMappingId":"claim-email","ClaimName":"email","SqlAccessor":"app.jwt_email()",
  "IsReservedClaim":True,"MapsToPrincipal":True,
  "Description2":"Verified email address. Magic-links asserts only that the bearer controls this mailbox; everything else about the caller is resolved from it inside this database.",
  "SemanticTypeIri":IRI+"JwtClaimMapping"},
 {"JwtClaimMappingId":"claim-tenant","ClaimName":"tenant_id","SqlAccessor":"app.jwt_tenant_id()",
  "IsReservedClaim":True,"MapsToPrincipal":False,
  "Description2":"Magic-links tenant the token was minted for. Pinned so a token from another tenant cannot authenticate here.",
  "SemanticTypeIri":IRI+"JwtClaimMapping"},
 {"JwtClaimMappingId":"claim-principal","ClaimName":"principal","SqlAccessor":"app.jwt_principal()",
  "IsReservedClaim":False,"MapsToPrincipal":True,
  "Description2":"AccessPrincipals row the caller acts as, resolved from the verified email via vw_agents. Not trusted from the token itself.",
  "SemanticTypeIri":IRI+"JwtClaimMapping"},
 {"JwtClaimMappingId":"claim-org","ClaimName":"organization","SqlAccessor":"app.jwt_organization()",
  "IsReservedClaim":False,"MapsToPrincipal":False,
  "Description2":"Organization the caller belongs to, derived from their principal's domain role. The default tenancy boundary for row predicates.",
  "SemanticTypeIri":IRI+"JwtClaimMapping"},
]

# -------------------------------------------------------------------- policies
# Tables every principal may read (reference/spec data, no tenancy dimension).
COMMON_READ = ["Procedures","ProcedureVersions","Steps","StepTransitions","Actions",
               "Functions","Tools","StepActions","StepFunctions","StepTools",
               "Requirements","StepRequirements","Rationales","Exceptions",
               "ProcedureTypes","Resources","ProcedureResources","Roles",
               "EvaluationContexts","StepVerifications","VerificationOutcomes"]

# Tenancy-scoped: only rows belonging to the caller's organization.
ORG_SCOPED = {
  "Agents": "organization = app.jwt_organization()",
  "Recipients": "organization = app.jwt_organization()",
  "CommunitiesOfPractice": "organization = app.jwt_organization()",
}

# Inference-backed: the predicate calls a derived column / SECURITY DEFINER fn.
INFERENCE_SCOPED = {
  "ChangeRequests":   ("public.calc_change_requests_is_open(change_request_id)",
                       "Open change requests only. is_open is derived from decision state several hops down the DAG, so the policy stays one line while the semantics stay deep."),
  "KnowledgeGaps":    ("public.calc_knowledge_gaps_is_open(knowledge_gap_id)",
                       "Open knowledge gaps only; resolved gaps are governance history and belong to the stewards."),
  "RoleAssignments":  ("public.calc_role_assignments_is_current(role_assignment_id)",
                       "Only assignments in force at the modelled evaluation instant. Expired and future-dated assignments are invisible."),
  "ProcedureVersions":("public.calc_procedure_versions_is_current(procedure_version_id)",
                       "Only the current version of each procedure; superseded versions are for stewards."),
}

# Role-scoped: rows naming the caller's own domain role.
OWN_ROLE = {
  "KnowledgeFragments": "owner_role = app.jwt_role()",
  "StewardshipAssignments": "steward_role = app.jwt_role()",
}

policies, grants, views, denials = [], [], [], []

def add_policy(pid, principal, table, cmd, pred, rationale, infer=False, check=None):
    policies.append({
        "AccessPolicyId": pid, "Principal": principal, "TargetTable": table,
        "Command": cmd, "RowPredicate": pred, "CheckPredicate": check or "",
        "Rationale": rationale, "ReferencesInference": infer,
        "SemanticTypeIri": IRI + "AccessPolicy",
    })

admin_tables = [r["TableName"] for r in rt_rows if r["PhysicalView"]]

for p in principals:
    pid, rid = p["AccessPrincipalId"], p["DomainRole"]
    short = rid.replace("-", "")
    if p["IsAdministrator"]:
        for t in admin_tables:
            add_policy(f"pol-{short}-{snake(t)}-select", pid, t, "SELECT", "",
                       f"{p['Label']} is a declared administrator: full read of every table is the point of the role.")
        continue
    for t in COMMON_READ:
        if t in rb:
            add_policy(f"pol-{short}-{snake(t)}-select", pid, t, "SELECT", "",
                       "Procedure specification is shared reference material; every principal reads it.")
    for t, pred in ORG_SCOPED.items():
        if t in rb:
            add_policy(f"pol-{short}-{snake(t)}-select", pid, t, "SELECT", pred,
                       "Tenancy boundary: a principal sees only rows belonging to its own organization.")
    for t, (pred, why) in INFERENCE_SCOPED.items():
        if t in rb:
            add_policy(f"pol-{short}-{snake(t)}-select", pid, t, "SELECT", pred, why, infer=True)
    for t, pred in OWN_ROLE.items():
        if t in rb:
            add_policy(f"pol-{short}-{snake(t)}-select", pid, t, "SELECT", pred,
                       "Ownership: a principal reads the rows its own domain role owns.")

# Write grants: narrow and deliberate.
WRITES = [
 ("principal-finance-analyst","StepExecutions","INSERT","", "The analyst records what actually happened when executing a step."),
 ("principal-finance-analyst","RequirementSatisfactions","INSERT","", "The analyst evidences requirement satisfaction as part of executing."),
 ("principal-controller","ChangeRequests","UPDATE",
  "public.calc_change_requests_is_open(change_request_id)",
  "The controller decides open change requests; a decided request is history and must not be rewritten."),
 ("principal-cfo","ChangeRequests","UPDATE",
  "public.calc_change_requests_is_open(change_request_id)",
  "The CFO decides open change requests escalated above the controller."),
 ("principal-communications-manager","SendIntents","INSERT","", "The communications manager raises intents to send outbound messages."),
 ("principal-hr-policy-owner","KnowledgeFragments","UPDATE","owner_role = app.jwt_role()",
  "A policy owner may revise the knowledge fragments its own role owns, and no others."),
]
for principal, table, cmd, pred, why in WRITES:
    if table in rb:
        short = principal.replace("principal-","").replace("-","")
        add_policy(f"pol-{short}-{snake(table)}-{cmd.lower()}", principal, table, cmd,
                   pred, why, infer=bool(pred and "calc_" in pred), check=pred)

# ------------------------------------------------------------------- field grants
# Fields no non-administrator may read, regardless of table access.
SENSITIVE = {"Agents": {"ContactAddress"}}

fields_by_table = {}
for f in rb["RulebookFields"]["data"]:
    fields_by_table.setdefault(f["TargetTable"], []).append(f)

granted_pairs = set()
for pol in policies:
    if pol["Command"] != "SELECT": continue
    granted_pairs.add((pol["Principal"], pol["TargetTable"]))

for principal, table in sorted(granted_pairs):
    p = next(x for x in principals if x["AccessPrincipalId"] == principal)
    for f in fields_by_table.get(table, []):
        fname = f["FieldName"]
        if not p["IsAdministrator"] and fname in SENSITIVE.get(table, set()):
            continue  # absent, not filtered
        grants.append({
            "FieldGrantId": f"fg-{principal.replace('principal-','')}-{table}.{fname}",
            "Principal": principal,
            "TargetField": f["RulebookFieldId"],
            "CanRead": True, "CanWrite": False,
            "MaskStrategy": "plain",
            "SemanticTypeIri": IRI + "FieldGrant",
        })

# ------------------------------------------------------------- role schema views
for principal, table in sorted(granted_pairs):
    p = next(x for x in principals if x["AccessPrincipalId"] == principal)
    rid = p["DomainRole"]
    views.append({
        "RoleSchemaViewId": f"rsv-{rid}-{snake(table)}",
        "RoleSchema": f"schema-{rid}",
        "Principal": principal,
        "TargetTable": table,
        "ViewName": snake(table),
        "SemanticTypeIri": IRI + "RoleSchemaView",
    })

# ------------------------------------------------------------------ denial tests
DENIALS = [
 ("deny-analyst-agent-other-org","principal-finance-analyst","Agents",
  "pol-financeanalyst-agents-select","legal-counsel-01",
  "A finance-analyst must not see an agent belonging to acme-legal."),
 ("deny-analyst-contact-address","principal-finance-analyst","Agents",
  "pol-financeanalyst-agents-select","maria-chen",
  "ContactAddress is withheld from every non-administrator; the column must be absent from the analyst's view, not merely filtered."),
 ("deny-analyst-decided-change-request","principal-finance-analyst","ChangeRequests",
  "pol-financeanalyst-changerequests-select","cr-close-window",
  "A decided change request is governance history; is_open is false, so the inference-backed predicate must exclude it."),
 ("deny-counsel-finance-fragment","principal-employment-counsel","KnowledgeFragments",
  "pol-employmentcounsel-knowledgefragments-select","kf-variance-threshold",
  "Employment counsel must not read a knowledge fragment owned by the finance side."),
 ("deny-notifier-resolved-gap","principal-notification-publisher","KnowledgeGaps",
  "pol-notificationpublisher-knowledgegaps-select","kg-close-automation",
  "A resolved knowledge gap must not appear to the notification publisher."),
]
for tid, principal, table, pol, row, why in DENIALS:
    denials.append({
        "AccessDenialTestId": tid, "TargetPolicy": pol, "Principal": principal,
        "TargetTable": table, "ForbiddenRowId": row,
        "ExpectedVisible": False, "ObservedVisible": None, "LastRunAt": None,
        "SemanticTypeIri": IRI + "AccessDenialTest",
    })

# ------------------------------------------------------------------------ write
payload = {
 "RulebookTables": rt_rows, "AccessPrincipals": principals, "RoleSchemas": schemas,
 "AccessPolicies": policies, "FieldGrants": grants, "RoleSchemaViews": views,
 "JwtClaimMappings": jwt_rows, "AccessDenialTests": denials,
}

rb2 = json.load(open(RB))          # re-read: another agent may have written
for t, rows in payload.items():
    if t not in rb2: raise SystemExit(f"FATAL: {t} missing from rulebook")
    if rb2[t]["data"]: raise SystemExit(f"FATAL: {t} already has {len(rb2[t]['data'])} rows; refusing")
    rb2[t]["data"] = rows

tmp = RB + ".tmp"
with open(tmp, "w") as fh:
    json.dump(rb2, fh, indent=1, ensure_ascii=False)
os.replace(tmp, RB)

for t, rows in payload.items():
    print(f"  {t:22s} {len(rows):5d} rows")
