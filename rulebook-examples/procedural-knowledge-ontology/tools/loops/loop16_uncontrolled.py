"""Loop 16 — who can read the whole register.

Premise: loop 15 finished filling in the access-control layer, and in doing so it removed
the only thing the previous witness for "access to process knowledge must be controlled"
had left to fire on. `RulebookTables.IsUnsecuredGovernanceRecord` asked whether a
governance table had no policy at all; once every governance table had one, the column
read false everywhere and stated nothing.

"Secured" and "controlled" are not the same claim. A table can carry a policy for a
non-administrator whose row predicate is empty — the policy exists, passes every
generator check, and lets that principal read every row in the table. That is the shape
an access review actually looks for, and it is present in this model: 189 of the emitted
policies grant a non-administrator every row of the table they name, across 88 tables.

This loop asks the question in the internal compliance auditor's voice and measures the
answer per table, so the claim is witnessed by a condition that is real in the live model
rather than by one the last round of remediation happened to erase.
"""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parents[1]))
from rulebook_edit import agg, calc  # noqa: E402

LOOP = {
    "WitnessLoopId": "loop-16",
    "LoopNumber": 16,
    "Title": "Controlled is not the same as secured: who reads the register in full",
    "Premise": (
        "Loop 15's access-control seeding gave every governance table a policy, which silently "
        "retired the witness for 'access to process knowledge must be controlled': a column that "
        "asked whether a table had no policy at all now reads false everywhere. The question that "
        "only became askable once every table was policed is the sharper one -- of the policies "
        "that do exist, which ones cut no rows at all for a principal who is not an administrator. "
        "That condition is present in this model and an access review is exactly the exercise that "
        "looks for it."
    ),
}

FIELDS = {
    "RulebookTables": [
        agg("UnrestrictedNonAdminPolicyCount", "integer",
            "How many of this table's row policies name a principal who is not an administrator and "
            "carry no row predicate, so that principal reads every row of the table.",
            "=COUNTIFS(AccessPolicies!{{TargetTable}}, {{RulebookTableId}}, "
            "AccessPolicies!{{IsUnrestrictedNonAdminGrant}}, TRUE)"),
        agg("RestrictedNonAdminPolicyCount", "integer",
            "How many of this table's row policies name a principal who is not an administrator and "
            "do cut the rows that principal sees.",
            "=COUNTIFS(AccessPolicies!{{TargetTable}}, {{RulebookTableId}}, "
            "AccessPolicies!{{IsUnrestrictedNonAdminGrant}}, FALSE, "
            "AccessPolicies!{{PrincipalIsAdmin}}, FALSE)"),
        calc("IsReadableInFullByNonAdmin", "boolean",
             "Some principal who is not an administrator can read every row of this table: a policy "
             "exists, so the table is secured, but it controls nothing. This is the violation an "
             "access review looks for, and it is the reason 'has a policy' is not the test.",
             "={{UnrestrictedNonAdminPolicyCount}} > 0"),
        calc("IsControlledForEveryNonAdmin", "boolean",
             "Every non-administrator who can reach this table reaches it through a policy that cuts "
             "rows. The table is not merely policed; access to it is controlled.",
             "=AND({{RestrictedNonAdminPolicyCount}} > 0, "
             "{{UnrestrictedNonAdminPolicyCount}} = 0)"),
    ],
}

QUESTIONS = [
    ("q16-internal-compliance-auditor-uncontrolled-read", "internal-compliance-auditor",
     "For each table of process knowledge in this register: can anyone who is not an administrator "
     "read the whole of it? Not 'is there a policy on it' -- is there a policy that actually cuts "
     "the rows they see?",
     "Every table in this register has a policy, and a policy that grants a non-administrator every "
     "row passes every check the build performs. Counting policies tells me nothing; it is the "
     "policies with an empty row predicate that I am sent here to find. I want the two counts kept "
     "apart on every table so that a table with wide-open read access cannot hide inside a coverage "
     "figure.",
     ["RulebookTables.UnrestrictedNonAdminPolicyCount",
      "RulebookTables.RestrictedNonAdminPolicyCount",
      "RulebookTables.IsReadableInFullByNonAdmin",
      "RulebookTables.IsControlledForEveryNonAdmin"]),
]
