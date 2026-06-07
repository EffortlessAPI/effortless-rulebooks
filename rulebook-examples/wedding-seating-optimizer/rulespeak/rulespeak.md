# 📘 Wedding Seating Optimizer — RuleSpeak

_Seating plan as a DAG — per-table happiness, capacity flags, per-guest satisfaction recompute on every move._

> Declarative business rules rendered from the rulebook. Every statement
> below expresses truth in the business domain — it is neither a procedure
> nor an imperative. The rulebook's formulas are the single source of truth;
> this document is their plain-language reading.

## 1 Business Vocabulary

| Term | Meaning (business sense) |
|------|--------------------------|
| **User** | Login identities for the demo (stub auth). |
| **Table** | Physical reception tables. Guests are assigned to one Table each. |
| Head Count | The number of guests related to the table. |
| Open Seats | Computed as `Seats - HeadCount`. |
| Over Capacity | True when the head count is greater than the seats. |
| Raw Happiness | Sum of EffectiveScore for relationships whose seated table matches this one. |
| Violation Count | Number of must-not pairs seated at this table. |
| Happiness | Net happiness: raw affinity minus capacity penalty minus side-skew penalty. |
| Grade | Computed: `If(ViolationCount>0, "Conflict", If(OverCapacity, "Over Capacity", If(Happiness>=15, "Great", If(Happiness>=5, "OK", "Cold"))))`. |
| Bride Side Count | Number of bride-side guests at this table. |
| Groom Side Count | Number of groom-side guests at this table. |
| Side Skew | Absolute difference between bride and groom side counts. |
| **Guest** | Wedding guests. Each is assigned to one Table. |
| Table Label | The label of the guest's assigned table. |
| Table Seats | The seats of the guest's assigned table. |
| Table Head Count | The head count of the guest's assigned table. |
| Relationships As a | The number of relationships related to the guest. |
| Relationships As B | The number of relationships related to the guest. |
| Satisfaction a | Sum of EffectiveScore on relationships where this guest is GuestA and the pair is co-seated. |
| Satisfaction B | The total effective score across the guest's relationships. |
| Satisfaction | Per-guest happiness rollup. |
| Mood | Computed: `If(Satisfaction>=10, "Happy", If(Satisfaction>=0, "Neutral", "Unhappy"))`. |
| Bride Flag | 1 if bride-side guest; else 0. Feeds Tables.BrideSideCount. |
| Groom Flag | 1 if groom-side guest; else 0. Feeds Tables.GroomSideCount. |
| **Relationship** | Directed-but-symmetric guest-pair affinities. Kind is loves \| prefers \| avoid \| must-not, with a Weight (positive = affinity, negative = friction). |
| Guest a Name | The full name of the relationship's guest a. |
| Guest B Name | The full name of the relationship's guest b. |
| Guest a Table | The assigned table of the relationship's guest a. |
| Guest B Table | The assigned table of the relationship's guest b. |
| Same Table | True iff both ends sit at the same non-empty table. |
| Seated Table | If both guests share a table, that table's id; else blank. Drives the per-table aggregations. |
| Effective Score | Weight when co-seated; else 0. Must-not pairs co-seated produce a large penalty regardless of Weight sign. |
| Is Must Not Violation | True when all of the following hold: the kind is the literal “must-not” and the same table flag is set. |
| Is Satisfied | Positive-affinity pair sitting together, or negative-affinity pair sitting apart. |
| Violation Table | TableId if this is a must-not violation; else blank. |
| Violation Flag | 1 if must-not pair co-seated; else 0. Feeds the per-table ViolationCount aggregation. |

## 2 Fact Types

- a **guest** may reference one **table**
- a **relationship** may reference one **guest**

## 3 Definitional Rules

_All statements express truth in the business domain; they are neither
procedures nor imperatives. "iff" is avoided in favor of "only if" so a
one-directional necessity is not mistaken for an equivalence._

| ID | Declarative rule |
|----|------------------|
| **DR-1 Head Count** | A table's head count is the number of guests related to the table. |
| **DR-2 Open Seats** | A table's open seats is computed as `Seats - HeadCount`. |
| **DR-3 Over Capacity** | A table is flagged over capacity if the head count is greater than the seats. |
| **DR-4 Raw Happiness** | A table's raw happiness is the total effective score across the relationships related to the table. |
| **DR-5 Violation Count** | A table's violation count is the total violation flag across the relationships related to the table. |
| **DR-6 Happiness** | A table's happiness is computed as `RawHappiness - (If(OverCapacity, 25, 0) + SideSkew)`. |
| **DR-7 Grade** | The table's grade is determined by the following priority:<br>1. the literal “Conflict”, if the violation count is greater than 0;<br>2. the literal “Over Capacity”, if the over capacity flag is set;<br>3. the literal “Great”, if the happiness is at least 15;<br>4. the literal “OK”, if the happiness is at least 5;<br>5. otherwise the literal “Cold”. |
| **DR-8 Bride Side Count** | A table's bride side count is the total bride flag across the guests related to the table. |
| **DR-9 Groom Side Count** | A table's groom side count is the total groom flag across the guests related to the table. |
| **DR-10 Side Skew** | The table's side skew is determined by the following priority:<br>1. `BrideSideCount-GroomSideCount`, if the bride side count is greater than the groom side count;<br>2. otherwise `GroomSideCount-BrideSideCount`. |
| **DR-11 Table Label** | A guest's table label is the label of the guest's assigned table. |
| **DR-12 Table Seats** | A guest's table seats is the seats of the guest's assigned table. |
| **DR-13 Table Head Count** | A guest's table head count is the head count of the guest's assigned table. |
| **DR-14 Relationships As a** | A guest's relationships as a is the number of relationships related to the guest. |
| **DR-15 Relationships As B** | A guest's relationships as b is the number of relationships related to the guest. |
| **DR-16 Satisfaction a** | A guest's satisfaction a is the total effective score across the relationships related to the guest. |
| **DR-17 Satisfaction B** | A guest's satisfaction b is the total effective score across the relationships related to the guest. |
| **DR-18 Satisfaction** | A guest's satisfaction is computed as `SatisfactionA + SatisfactionB`. |
| **DR-19 Mood** | The guest's mood is determined by the following priority:<br>1. the literal “Happy”, if the satisfaction is at least 10;<br>2. the literal “Neutral”, if the satisfaction is at least 0;<br>3. otherwise the literal “Unhappy”. |
| **DR-20 Bride Flag** | The guest's bride flag is determined by the following priority:<br>1. 1, if the side is the literal “bride”;<br>2. otherwise 0. |
| **DR-21 Groom Flag** | The guest's groom flag is determined by the following priority:<br>1. 1, if the side is the literal “groom”;<br>2. otherwise 0. |
| **DR-22 Guest a Name** | A relationship's guest a name is the full name of the relationship's guest a. |
| **DR-23 Guest B Name** | A relationship's guest b name is the full name of the relationship's guest b. |
| **DR-24 Guest a Table** | A relationship's guest a table is the assigned table of the relationship's guest a. |
| **DR-25 Guest B Table** | A relationship's guest b table is the assigned table of the relationship's guest b. |
| **DR-26 Same Table** | A relationship is flagged same table if all of the following hold: the guest a table is not an empty string and the guest a table is the guest b table. |
| **DR-27 Seated Table** | The relationship's seated table is determined by the following priority:<br>1. the guest a table, if the same table flag is set;<br>2. otherwise an empty string. |
| **DR-28 Effective Score** | The relationship's effective score is determined by the following priority:<br>1. `If(Kind="must-not", -50, Weight)`, if the same table flag is set;<br>2. otherwise 0. |
| **DR-29 Is Must Not Violation** | A relationship is considered a must not violation if all of the following hold: the kind is the literal “must-not” and the same table flag is set. |
| **DR-30 Is Satisfied** | A relationship is considered satisfied if at least one of the following holds: all of the following hold: the weight is greater than 0 and the same table flag is set or all of the following hold: the weight is less than 0 and it is not the case that the same table flag is set. |
| **DR-31 Violation Table** | The relationship's violation table is determined by the following priority:<br>1. the guest a table, if the is must not violation flag is set;<br>2. otherwise an empty string. |
| **DR-32 Violation Flag** | The relationship's violation flag is determined by the following priority:<br>1. 1, if the is must not violation flag is set;<br>2. otherwise 0. |

## 4 Traceability to Schema

_The expression column is the rule's definition in RuleSpeak notation —
the same logic the rulebook stores, written for a business reader._

| Schema element | Kind | Expression |
|----------------|------|------------|
| **Tables.HeadCount** | rollup | `Count(Guests via AssignedTable)` |
| **Tables.OpenSeats** | formula | `Seats - HeadCount` |
| **Tables.OverCapacity** | formula | `HeadCount > Seats` |
| **Tables.RawHappiness** | rollup | `Sum(Relationships.EffectiveScore via SeatedTable)` |
| **Tables.ViolationCount** | rollup | `Sum(Relationships.ViolationFlag via SeatedTable)` |
| **Tables.Happiness** | formula | `RawHappiness - (If(OverCapacity, 25, 0) + SideSkew)` |
| **Tables.Grade** | formula | `If(ViolationCount>0, "Conflict", If(OverCapacity, "Over Capacity", If(Happiness>=15, "Great", If(Happiness>=5, "OK", "Cold"))))` |
| **Tables.BrideSideCount** | rollup | `Sum(Guests.BrideFlag via AssignedTable)` |
| **Tables.GroomSideCount** | rollup | `Sum(Guests.GroomFlag via AssignedTable)` |
| **Tables.SideSkew** | formula | `If(BrideSideCount>GroomSideCount, BrideSideCount-GroomSideCount, GroomSideCount-BrideSideCount)` |
| **Guests.TableLabel** | lookup | `Lookup(Tables.Label via AssignedTable)` |
| **Guests.TableSeats** | lookup | `Lookup(Tables.Seats via AssignedTable)` |
| **Guests.TableHeadCount** | lookup | `Lookup(Tables.HeadCount via AssignedTable)` |
| **Guests.RelationshipsAsA** | rollup | `Count(Relationships via GuestA)` |
| **Guests.RelationshipsAsB** | rollup | `Count(Relationships via GuestB)` |
| **Guests.SatisfactionA** | rollup | `Sum(Relationships.EffectiveScore via GuestA)` |
| **Guests.SatisfactionB** | rollup | `Sum(Relationships.EffectiveScore via GuestB)` |
| **Guests.Satisfaction** | formula | `SatisfactionA + SatisfactionB` |
| **Guests.Mood** | formula | `If(Satisfaction>=10, "Happy", If(Satisfaction>=0, "Neutral", "Unhappy"))` |
| **Guests.BrideFlag** | formula | `If(Side="bride", 1, 0)` |
| **Guests.GroomFlag** | formula | `If(Side="groom", 1, 0)` |
| **Relationships.GuestAName** | lookup | `Lookup(Guests.FullName via GuestA)` |
| **Relationships.GuestBName** | lookup | `Lookup(Guests.FullName via GuestB)` |
| **Relationships.GuestATable** | lookup | `Lookup(Guests.AssignedTable via GuestA)` |
| **Relationships.GuestBTable** | lookup | `Lookup(Guests.AssignedTable via GuestB)` |
| **Relationships.SameTable** | formula | `And(GuestATable<>"", GuestATable=GuestBTable)` |
| **Relationships.SeatedTable** | formula | `If(SameTable, GuestATable, "")` |
| **Relationships.EffectiveScore** | formula | `If(SameTable, If(Kind="must-not", -50, Weight), 0)` |
| **Relationships.IsMustNotViolation** | formula | `And(Kind="must-not", SameTable)` |
| **Relationships.IsSatisfied** | formula | `Or(And(Weight>0, SameTable), And(Weight<0, Not(SameTable)))` |
| **Relationships.ViolationTable** | formula | `If(IsMustNotViolation, GuestATable, "")` |
| **Relationships.ViolationFlag** | formula | `If(IsMustNotViolation, 1, 0)` |
