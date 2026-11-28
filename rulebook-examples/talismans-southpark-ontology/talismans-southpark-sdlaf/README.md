# The South Park ontology, in SDLAF

The same world as the project around this folder — Jessica Talisman's South Park ontology,
her ten built-in facts and her five rules — written as an ordinary rulebook: Schema, Data,
Lookups, Aggregations and Formulas, and nothing else. There is no subject–property–object
store and no rule engine. A fact is a row or a column of the table it is about, and each rule
is a field.

`effortless-rulebook/talismans-southpark-sdlaf-rulebook.json` is the rulebook, and it is HEAD:
edit it directly. Its build (`effortless build` in this folder) writes Postgres into
`postgres-bootstrap/` and loads it into `erb_talismans_southpark_sdlaf`, then writes the
documentation (`rulespeak/`) and the explainer (`explainer/`).

## Where each fact lives

| Her fact | Here |
|---|---|
| `Stan isA Person` | `Individuals.IsAssertedPerson` |
| `Stan hasAge 10` | `Individuals.Age` |
| `Stan attends South Park Elementary` | `Individuals.Attends`, a link to the school's row |
| `Stan hasGradeLevel 4`, `Stan hasRole Student` | a row of `GradeLevelRecords` / `RoleRecords`; `Individuals.GradeLevel` and `Role` are the one in effect |
| `Marvin isParentOf Randy` (and two more) | a row of `Parenthoods`: `Parent`, `Child` |
| `Randy isSpouseOf Sharon` | a row of `Marriages` |
| `Stan isFriendOf Kyle` | a row of `Friendships` |

What one individual has alone is a column of its row. A parenthood, a marriage and a
friendship join two individuals, and a person has many of each, so each is an entity of its
own with two links to `Individuals` (the rulebook never holds a many-to-many link).

## Where each rule lives

All five switches are columns of the one `Reasoning` row; every table links to it, so a
switch is one lookup from the field it gates. Every conclusion is open-world: TRUE, or not
known (`IF(<condition>, TRUE())`).

| Her rule | Here |
|---|---|
| **Inverse** (`isChildOf`) | Nothing stored: a parenthood read from the child's end. `Parenthoods.ChildOfHolds`; `Individuals.ParentCount` is there whatever the rule says, `ChildOfFactCount` only when it is on. |
| **Symmetric** (spouse, friend) | A marriage or a friendship has no direction: `MirrorHolds` on the row, and `Individuals.SpouseCount` / `FriendCount` count it from both ends. |
| **Property chain** (`isGrandparentOf`) | `Grandparenthoods`: every ordered pair of parenthoods is a candidate row (a formula cannot add a row), and `Holds` when the first one's child is the second one's parent. `Individuals.GrandchildCount` / `GrandparentCount` roll it up. |
| **Domain and range** | Class membership is a formula, because it is concluded: `Individuals.IsPerson` (asserted, or attends something, or has a grade level or a role) and `IsSchool` (asserted, or someone attends it). |
| **Disjoint** | `Parenthoods.IsContradiction`: the parent is also the child's spouse, stated or mirrored. `Reasoning.IsConsistent` rolls it up. |
| One current value (grade, role) | Time, not a flag: the latest record switched on (`MAXIFS` over `StatedSeq`) is the one in effect. |

Beyond her page: `Parenthoods.AncestryClosure` is the transitive closure of parenthood
(`vw_parenthoods_closure`), and `Individuals.DescendantCount` / `AncestorCount` count it.

`Reasoning.Summary` says the whole world in one line: with every rule on, *10 stated facts,
8 conclusions, 0 contradictions*. Her page starts with every rule off; here they start on, so
every conclusion shows.

## Run it

```bash
effortless build    # in this folder: Postgres (loaded into erb_talismans_southpark_sdlaf),
                    # TypeScript, Python, OWL, Excel, the editor, the docs and the explainer
./start.sh          # the generated rulebook editor: http://localhost:43632 (API :43631)
```

The explainer opens from the file system: `explainer/rulebook-explainer-dag/pages/index.html`.

## Tests

```bash
python3 testing/test_sdlaf.py
```

- Her facts, each conclusion, each rule switch, and her changes (a later grade, Stan leaving
  school, a parent who is also the spouse), each inside a transaction that is rolled back.
- Every engine agrees: Postgres (the views), TypeScript (the SDK's `evaluate`) and Python
  compute every cell alike in three scenarios, compared by the harness's own `compare_values`
  through `orchestration/live_substrates.py`.
- OWL agrees on every cell when it runs on the rows its own build wrote. On rows handed to it
  live, `live_substrates.py` writes no edges for a closure (the OWL build's `manifest.json`
  does not say which property holds them), so `DescendantCount` and `AncestorCount` read 0:
  an expected failure in the tests until `rulebook-to-owl` names that property.
