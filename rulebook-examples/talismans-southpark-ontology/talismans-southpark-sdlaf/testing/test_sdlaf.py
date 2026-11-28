#!/usr/bin/env python3
"""The South Park ontology in SDLAF: what its views must answer.

Her page's ten built-in facts with every rule switched on give ten stated facts and eight
conclusions (three child links, a mirrored marriage and friendship, two grandparents, and
South Park Elementary as a School), and no contradiction. Each rule is then switched, and her
two kinds of change are made (a new current value, a fact that contradicts the disjoint rule),
each inside a transaction that is rolled back, so the database is left as the build loaded it.

Run after `effortless build` in this folder (it loads erb_talismans_southpark_sdlaf):

    python3 testing/test_sdlaf.py        (or pytest)
"""
import copy
import json
import os
import subprocess
import sys
import unittest
from pathlib import Path

HERE = Path(__file__).resolve().parents[1]
REPO = HERE.parents[2]
RULEBOOK = HERE / "effortless-rulebook" / "talismans-southpark-sdlaf-rulebook.json"
sys.path.insert(0, str(REPO / "orchestration"))
import live_substrates as live  # noqa: E402  the platform's one way to run a substrate on rows and compare
from shared import to_snake_case  # noqa: E402  the harness's own naming rule
DB = os.environ.get("DATABASE_URL") or "postgresql://postgres@localhost:5432/erb_talismans_southpark_sdlaf"


def sql(script: str) -> list[list[str]]:
    """Runs the script in one transaction that is rolled back; the rows of its last statement."""
    run = subprocess.run(["psql", DB, "-v", "ON_ERROR_STOP=1", "-AtqF", "\t", "-c", f"begin; {script}; rollback;"],
                         capture_output=True, text=True)
    if run.returncode != 0:
        raise AssertionError(f"psql failed against {DB} (run `effortless build` in {HERE.name}/ first):\n{run.stderr}")
    return [line.split("\t") for line in run.stdout.splitlines() if line]


ALL_ON = "update reasoning set inverse_is_on = true, symmetric_is_on = true, chain_is_on = true, domain_range_is_on = true, disjoint_is_on = true"
SUMMARY = "select summary, is_consistent from vw_reasoning"


class AsHerPageStates(unittest.TestCase):
    def test_every_view_answers(self):
        views = [r[0] for r in sql("select table_name from information_schema.views where table_schema = 'public' and table_name like 'vw_%'")]
        self.assertEqual(len(views), 10, views)
        for v in views:
            sql(f"select * from {v}")

    def test_ten_stated_facts_and_eight_conclusions(self):
        self.assertEqual(sql(f"{ALL_ON}; {SUMMARY}"), [["10 stated facts, 8 conclusions, 0 contradictions", "t"]])

    def test_each_conclusion(self):
        self.assertEqual(sql(f"{ALL_ON}; select child_of_sentence from vw_parenthoods where child_of_holds order by 1"),
                         [["Randy is child of Marvin"], ["Shelly is child of Randy"], ["Stan is child of Randy"]])
        self.assertEqual(sql(f"{ALL_ON}; select mirror_sentence from vw_marriages where mirror_holds union all "
                             f"select mirror_sentence from vw_friendships where mirror_holds"),
                         [["Sharon is spouse of Randy"], ["Kyle is friend of Stan"]])
        self.assertEqual(sql(f"{ALL_ON}; select name from vw_grandparenthoods where holds order by 1"),
                         [["Marvin is grandparent of Shelly"], ["Marvin is grandparent of Stan"]])
        self.assertEqual(sql(f"{ALL_ON}; select individual_id, coalesce(is_person::text, ''), coalesce(is_school::text, ''), "
                             f"coalesce(is_school_concluded::text, '') from vw_individuals where is_person or is_school order by 1"),
                         [["SouthParkElementary", "", "true", "true"], ["Stan", "true", "", ""]])

    def test_stans_columns(self):
        self.assertEqual(sql("select age, attends, grade_level, role from vw_individuals where individual_id = 'Stan'"),
                         [["10", "SouthParkElementary", "4", "Student"]])

    def test_the_family_rolls_up(self):
        rows = sql("select individual_id, child_count, parent_count, grandchild_count, grandparent_count, "
                   "descendant_count, ancestor_count, spouse_count, friend_count from vw_individuals order by sort_order")
        self.assertEqual(rows, [
            ["Marvin", "1", "0", "2", "0", "3", "0", "0", "0"],
            ["Randy", "2", "1", "0", "0", "2", "1", "1", "0"],
            ["Sharon", "0", "0", "0", "0", "0", "0", "1", "0"],
            ["Stan", "0", "1", "0", "1", "0", "2", "0", "1"],
            ["Shelly", "0", "1", "0", "1", "0", "2", "0", "0"],
            ["Kyle", "0", "0", "0", "0", "0", "0", "0", "1"],
            ["SouthParkElementary", "0", "0", "0", "0", "0", "0", "0", "0"],
        ])


class TheRuleSwitches(unittest.TestCase):
    def switched(self, **on):
        sets = ", ".join(f"{k} = {'true' if v else 'false'}" for k, v in on.items())
        return sql(f"{ALL_ON}; update reasoning set {sets}; {SUMMARY}")[0][0]

    def test_all_off_concludes_nothing(self):
        self.assertEqual(self.switched(inverse_is_on=False, symmetric_is_on=False, chain_is_on=False,
                                       domain_range_is_on=False, disjoint_is_on=False),
                         "10 stated facts, 0 conclusions, 0 contradictions")

    def test_each_rule_takes_its_own_conclusions_with_it(self):
        self.assertEqual(self.switched(inverse_is_on=False), "10 stated facts, 5 conclusions, 0 contradictions")
        self.assertEqual(self.switched(symmetric_is_on=False), "10 stated facts, 6 conclusions, 0 contradictions")
        self.assertEqual(self.switched(chain_is_on=False), "10 stated facts, 6 conclusions, 0 contradictions")
        self.assertEqual(self.switched(domain_range_is_on=False), "10 stated facts, 7 conclusions, 0 contradictions")

    def test_the_inverse_link_is_there_whatever_the_rule_says(self):
        self.assertEqual(sql(f"{ALL_ON}; update reasoning set inverse_is_on = false; "
                             f"select parent_count, child_of_fact_count from vw_individuals where individual_id = 'Stan'"),
                         [["1", "0"]])


class HerChanges(unittest.TestCase):
    def test_a_later_grade_level_is_the_current_one(self):
        add = ("insert into grade_level_records (grade_level_record_id, reasoning, individual, grade_level, stated_seq, is_on) "
               "values ('Stan|grade|11', 'south-park', 'Stan', 5, 11, true)")
        self.assertEqual(sql(f"{add}; select grade_level from vw_individuals where individual_id = 'Stan'"), [["5"]])
        self.assertEqual(sql(f"{add}; select name, is_current from vw_grade_level_records order by stated_seq"),
                         [["Stan has grade level 4", "f"], ["Stan has grade level 5", "t"]])
        self.assertEqual(sql(f"{ALL_ON}; {add}; {SUMMARY}")[0][0], "10 stated facts, 8 conclusions, 0 contradictions")

    def test_stan_leaves_school(self):
        leave = ("update individuals set attends = null where individual_id = 'Stan'; "
                 "update grade_level_records set is_on = false; update role_records set is_on = false")
        self.assertEqual(sql(f"{ALL_ON}; {leave}; select grade_level, role, is_person from vw_individuals where individual_id = 'Stan'"),
                         [["", "", "t"]])
        self.assertEqual(sql(f"{ALL_ON}; {leave}; select count_of_schools from vw_reasoning"), [["0"]])

    def test_a_parent_who_is_also_the_spouse_is_a_contradiction(self):
        wed = ("insert into marriages (marriage_id, reasoning, partner_a, partner_b) "
               "values ('Stan|isSpouseOf|Randy', 'south-park', 'Stan', 'Randy')")
        self.assertEqual(sql(f"{ALL_ON}; {wed}; {SUMMARY}"), [["11 stated facts, 9 conclusions, 1 contradictions", "f"]])
        self.assertEqual(sql(f"{ALL_ON}; {wed}; select name from vw_parenthoods where is_contradiction"),
                         [["Randy is parent of Stan"]])
        # Stated Stan -> Randy: Randy is Stan's spouse only when the symmetric rule mirrors it.
        self.assertEqual(sql(f"{ALL_ON}; {wed}; update reasoning set symmetric_is_on = false; {SUMMARY}")[0][1], "t")


ALL_OFF = ALL_ON.replace("true", "false")
GRADE_FIVE_AND_A_WEDDING = (
    "insert into grade_level_records (grade_level_record_id, reasoning, individual, grade_level, stated_seq, is_on) "
    "values ('Stan|grade|11', 'south-park', 'Stan', 5, 11, true); "
    "insert into marriages (marriage_id, reasoning, partner_a, partner_b) "
    "values ('Stan|isSpouseOf|Randy', 'south-park', 'Stan', 'Randy')")


def postgres_answers(change: str) -> dict:
    """Every view after `change`, under the rulebook's field names, read inside the rolled-back transaction."""
    rulebook = live.load_rulebook(HERE)
    tables = {n: t for n, t in live.tables_of(rulebook).items() if n != "__meta__"}
    parts = ", ".join(f"'{n}', (select coalesce(jsonb_agg(x), '[]') from vw_{to_snake_case(n)} x)" for n in tables)
    (text,), = sql(f"{change}; select jsonb_build_object({parts})")
    got = json.loads(text)
    return {n: [{f["name"]: r.get(to_snake_case(f["name"])) for f in t["schema"]} for r in got[n]] for n, t in tables.items()}


def stored_rows() -> dict:
    return {n: [{f: r.get(f) for f in live.stored_fields(t)} for r in t["data"]]
            for n, t in live.tables_of(live.load_rulebook(HERE)).items()}


def switched_off(rows: dict) -> dict:
    rows = copy.deepcopy(rows)
    for r in rows["Reasoning"]:
        for rule in ("InverseIsOn", "SymmetricIsOn", "ChainIsOn", "DomainRangeIsOn", "DisjointIsOn"):
            r[rule] = False
    return rows


def grade_five_and_a_wedding(rows: dict) -> dict:
    rows = copy.deepcopy(rows)
    rows["GradeLevelRecords"].append({"GradeLevelRecordId": "Stan|grade|11", "Reasoning": "south-park", "Individual": "Stan",
                                      "GradeLevel": 5, "StatedSeq": 11, "IsOn": True})
    rows["Marriages"].append({"MarriageId": "Stan|isSpouseOf|Randy", "Reasoning": "south-park", "PartnerA": "Stan", "PartnerB": "Randy"})
    return rows


SCENARIOS = {  # name: (the change in Postgres, the same change to the stored rows)
    "as stored": ("set local search_path = public", lambda rows: rows),
    "every rule off": (ALL_OFF, switched_off),
    "grade five and a parent who is a spouse": (GRADE_FIVE_AND_A_WEDDING, grade_five_and_a_wedding),
}


class EveryEngineAgrees(unittest.TestCase):
    """Postgres (the views), TypeScript (the SDK's evaluate) and Python compute every cell alike,
    compared by the harness's own compare_values, in each scenario."""

    def compare(self, substrates):
        for name, (change, rows_after) in SCENARIOS.items():
            with self.subTest(scenario=name):
                rows = rows_after(stored_rows())
                answers = {"effortless-postgres": postgres_answers(change)}
                answers.update({s: live.evaluate(HERE, s, rows)["tables"] for s in substrates})
                result = live.compare(HERE, answers)
                self.assertGreater(result["cells"], 400)
                self.assertEqual(result["missingRows"], [])
                self.assertEqual(result["differing"], [], json.dumps(result["differing"][:6], indent=1))

    def test_postgres_typescript_and_python(self):
        self.compare(("effortless-typescript", "effortless-python"))

    # OWL computes every cell alike from the rows its own build wrote (individuals.ttl), but on rows
    # handed to it live, live_substrates writes no edges for a closure (manifest.json does not say
    # which property holds them), so DescendantCount and AncestorCount read 0.
    @unittest.expectedFailure
    def test_owl_on_live_rows(self):
        self.compare(("effortless-owl",))


class TheFile(unittest.TestCase):
    def test_is_in_the_layout(self):
        run = subprocess.run([sys.executable, str(REPO / "scripts" / "rulebook_layout.py"), "--check", str(RULEBOOK)],
                             capture_output=True, text=True)
        self.assertEqual(run.returncode, 0, run.stdout + run.stderr)

    def test_every_grandparenthood_candidate_is_there(self):
        # The candidates are every ordered pair of parenthoods: nothing about the family decides them.
        self.assertEqual(sql("select (select count(*) from vw_parenthoods) ^ 2 = (select count(*) from vw_grandparenthoods) "
                             "and not exists (select 1 from vw_parenthoods u cross join vw_parenthoods l where not exists "
                             "(select 1 from vw_grandparenthoods g where g.upper_link = u.parenthood_id and g.lower_link = l.parenthood_id))"),
                         [["t"]])


if __name__ == "__main__":
    unittest.main()
