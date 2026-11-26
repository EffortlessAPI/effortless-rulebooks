# 📘 South Park ontology explainer — Docs

_The barebones South Park ontology from Jessica Talisman's explainer: ten stated facts and five rules that can each be switched on. Conclusions are formulas._

> Declarative business rules rendered from the rulebook. Every statement
> below expresses truth in the business domain — it is neither a procedure
> nor an imperative. The rulebook's formulas are the single source of truth;
> this document is their plain-language reading.

## 1 Business Vocabulary

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Graph** | The whole example. | — |
| Name | The same as its graph ID. | — |
| Count of Stated Facts | The total stated unit across the statements related to the graph. | — |
| Count of Conclusions | The total conclusion unit across the statements related to the graph. | _Facts that hold but were never stated._ |
| Count of Violations | The total violation unit across the disjoint checks related to the graph. | — |
| Is Consistent | True when the count of violations is 0. | — |

<details open>
<summary>Example data</summary>

| Name ƒ | Count of Stated Facts ƒ | Count of Conclusions ƒ | Count of Violations ƒ | Is Consistent ƒ |
|---|---|---|---|---|
| south-park | 11 | 9 | 0 | true |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Rule** | The five rules; IsEnabled is the checkbox. | — |
| Label | A defined attribute. | — |
| Explanation | A defined attribute. | — |
| Is Enabled | True when an empty string. | — |
| Name | The same as its label. | — |
| Count of Firings | The total fired unit across the firings related to the rule. | _How many times the rule produced a fact._ |

<details open>
<summary>Example data</summary>

| Label | Explanation | Is Enabled | Name ƒ | Count of Firings ƒ |
|---|---|---|---|---|
| Inverse | A parent link also gives a child link between the same two people. | true | Inverse | 4 |
| Symmetric | A friend link or a spouse link is true for both people. | true | Symmetric | 2 |
| Property chain | A parent of a parent is a grandparent. | true | Property chain | 2 |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Thing** | A node in the picture: a person, a place, a class or a role value. | — |
| Label | A defined attribute. | — |
| Kind | A defined attribute. | — |
| Name | The same as its label. | — |
| Count of Children | The total parent unit across the statements related to the thing. | _Added: a count, which a reasoner alone does not give._ |
| Count of Grandchildren | The total grandparent unit across the statements related to the thing. | _Added: counts concluded grandchildren._ |

<details open>
<summary>Example data</summary>

| Label | Kind | Name ƒ | Count of Children ƒ | Count of Grandchildren ƒ |
|---|---|---|---|---|
| Marvin | individual | Marvin | 1 | 2 |
| Randy | individual | Randy | 2 | 0 |
| Sharon | individual | Sharon | 1 | 0 |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Property** | A kind of link, with the characteristics the rules use. | — |
| Label | A defined attribute. | — |
| Inverse of | A defined attribute. | — |
| Is Symmetric | True when an empty string. | — |
| Chain First | A defined attribute. | — |
| Chain Second | A defined attribute. | — |
| Domain Class | A defined attribute. | — |
| Range Class | A defined attribute. | — |
| Disjoint With | A defined attribute. | — |
| Name | The same as its label. | — |

<details open>
<summary>Example data</summary>

| Label | Is Symmetric | Name ƒ |
|---|---|---|
| is parent of | false | is parent of |
| is child of | false | is child of |
| is spouse of | true | is spouse of |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Statement** | A fact that is stated, or could be concluded: Subject, Property, Object. | — |
| Graph | A defined attribute. | — |
| Subject | A defined attribute. | — |
| Property | A defined attribute. | — |
| Object Thing | A defined attribute. | — |
| Number Value | A defined attribute. | — |
| Is Stated | True when an empty string. | _Drawn as a solid line in the explainer._ |
| Subject Label | Taken from the linked subject. | — |
| Property Label | Taken from the linked property. | — |
| Object Label | Taken from the linked object thing. | — |
| Name | Computed as the subject label, followed by a space, followed by the property label, followed by a space, followed by the number value if the object thing is blank, in all other cases the object label. | — |
| Count of Inverse Firings | The total inverse fired unit across the firings related to the statement. | — |
| Count of Symmetric Firings | The total symmetric fired unit across the firings related to the statement. | — |
| Holds Before Chain | True when at least one of the following holds: the stated flag is set; the count of inverse firings is greater than 0; or the count of symmetric firings is greater than 0. | — |
| Count of Chain Firings | The total chain fired unit across the firings related to the statement. | — |
| Holds Before Typing | True when at least one of the following holds: the holds before chain flag is set or the count of chain firings is greater than 0. | — |
| Count of Type Firings | The total type fired unit across the firings related to the statement. | — |
| Holds | True when at least one of the following holds: the holds before typing flag is set or the count of type firings is greater than 0. | _True when stated or concluded by an enabled rule._ |
| Is Conclusion | True when all of the following hold: the holds flag is set and the stated flag is not set. | _Drawn as a dashed line in the explainer._ |
| Worked Out by | Determined by priority: an empty string if the conclusion flag is not set; “inverse” if the count of inverse firings is greater than 0; “symmetric” if the count of symmetric firings is greater than 0; “property chain” if the count of chain firings is greater than 0; in all other cases, “domain and range”. | — |
| Stated Unit | Determined by priority: 1 if the stated flag is set; in all other cases, 0. | — |
| Conclusion Unit | Determined by priority: 1 if the conclusion flag is set; in all other cases, 0. | — |
| Parent Unit | Determined by priority: 1 if all of the following hold: the holds flag is set and the property is “isParentOf”; in all other cases, 0. | — |
| Grandparent Unit | Determined by priority: 1 if all of the following hold: the holds flag is set and the property is “isGrandparentOf”; in all other cases, 0. | — |

<details open>
<summary>Example data</summary>

| Number Value | Is Stated | Subject Label ƒ | Property Label ƒ | Object Label ƒ | Name ƒ | Count of Inverse Firings ƒ | Count of Symmetric Firings ƒ | Holds Before Chain ƒ | Count of Chain Firings ƒ | Holds Before Typing ƒ | Count of Type Firings ƒ | Holds ƒ | Is Conclusion ƒ | Worked Out by ƒ | Stated Unit ƒ | Conclusion Unit ƒ | Parent Unit ƒ | Grandparent Unit ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| — | true | Marvin | is parent of | Randy | Marvin is parent of Randy | 0 | 0 | true | 0 | true | 0 | true | false | — | 1 | 0 | 1 | 0 |
| — | false | Marvin | is parent of | Sharon | Marvin is parent of Sharon | 0 | 0 | false | 0 | false | 0 | false | false | — | 0 | 0 | 0 | 0 |
| — | false | Marvin | is parent of | Shelly | Marvin is parent of Shelly | 0 | 0 | false | 0 | false | 0 | false | false | — | 0 | 0 | 0 | 0 |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Firing** | One way one rule could produce one fact from one or two others. | — |
| Rule | A defined attribute. | — |
| Kind | A defined attribute. | — |
| Target | A defined attribute. | — |
| First | A defined attribute. | — |
| Second | A defined attribute. | _Same as First when the rule needs one premise._ |
| Rule Enabled | True when the linked rule is enabled. | — |
| Rule Label | Taken from the linked rule. | — |
| Target Name | Taken from the linked target. | — |
| First Name | Taken from the linked first. | — |
| Second Name | Taken from the linked second. | — |
| First is Stated | True when the linked first is stated. | — |
| First Holds Before Chain | True when the linked first is holds before chain. | — |
| Second Holds Before Chain | True when the linked second is holds before chain. | — |
| First Holds Before Typing | True when the linked first is holds before typing. | — |
| Inverse Fired Unit | Determined by priority: 1 if all of the following hold: the rule enabled flag is set; the kind is “inverse”; and the first is stated flag is set; in all other cases, 0. | — |
| Symmetric Fired Unit | Determined by priority: 1 if all of the following hold: the rule enabled flag is set; the kind is “symmetric”; and the first is stated flag is set; in all other cases, 0. | — |
| Chain Fired Unit | Determined by priority: 1 if all of the following hold: the rule enabled flag is set; the kind is “chain”; the first holds before chain flag is set; and the second holds before chain flag is set; in all other cases, 0. | — |
| Type Fired Unit | Determined by priority: 1 if all of the following hold: the rule enabled flag is set; at least one of the following holds: the kind is “domain” or the kind is “range”; and the first holds before typing flag is set; in all other cases, 0. | — |
| Fired Unit | Computed as the inverse fired unit plus the symmetric fired unit plus the chain fired unit plus the type fired unit. | — |
| Name | Determined by priority: the target name, followed by “: concluded by the ”, followed by the rule label, followed by “ rule from '”, followed by the first name, followed by “'”, followed by “ and '”, followed by the second name, followed by “'” if the fired unit is greater than 0; in all other cases, the firing ID. | _The explainer's conclusion card._ |

<details open>
<summary>Example data</summary>

| Kind | Rule Enabled ƒ | Rule Label ƒ | Target Name ƒ | First Name ƒ | Second Name ƒ | First is Stated ƒ | First Holds Before Chain ƒ | Second Holds Before Chain ƒ | First Holds Before Typing ƒ | Inverse Fired Unit ƒ | Symmetric Fired Unit ƒ | Chain Fired Unit ƒ | Type Fired Unit ƒ | Fired Unit ƒ | Name ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| inverse | true | Inverse | Randy is child of Marvin | Marvin is parent of Randy | Marvin is parent of Randy | true | true | true | true | 1 | 0 | 0 | 0 | 1 | Randy is child of Marvin: concluded by the Inverse rule from 'Marvin is parent of Randy' |
| inverse | true | Inverse | Sharon is child of Marvin | Marvin is parent of Sharon | Marvin is parent of Sharon | false | false | false | false | 0 | 0 | 0 | 0 | 0 | inverse:Marvin\|isParentOf\|Sharon>Sharon\|isChildOf\|Marvin |
| inverse | true | Inverse | Shelly is child of Marvin | Marvin is parent of Shelly | Marvin is parent of Shelly | false | false | false | false | 0 | 0 | 0 | 0 | 0 | inverse:Marvin\|isParentOf\|Shelly>Shelly\|isChildOf\|Marvin |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Disjoint Check** | Two statements that must not both hold. | — |
| Graph | A defined attribute. | — |
| Rule | A defined attribute. | — |
| A | A defined attribute. | — |
| B | A defined attribute. | — |
| Name | The same as its disjoint check ID. | — |
| Rule Enabled | True when the linked rule is enabled. | — |
| A Holds | True when the linked a is holds. | — |
| B Holds | True when the linked b is holds. | — |
| Violation Unit | Determined by priority: 1 if all of the following hold: the rule enabled flag is set; the a holds flag is set; and the b holds flag is set; in all other cases, 0. | — |

<details open>
<summary>Example data</summary>

| Name ƒ | Rule Enabled ƒ | A Holds ƒ | B Holds ƒ | Violation Unit ƒ |
|---|---|---|---|---|
| Marvin\|isParentOf\|Randy~isSpouseOf | true | true | false | 0 |
| Marvin\|isParentOf\|Sharon~isSpouseOf | true | false | false | 0 |
| Marvin\|isParentOf\|Shelly~isSpouseOf | true | false | false | 0 |

_ƒ marks a computed column._

</details>

## 2 Fact Types

- a **property** may reference one **property**
- a **property** may reference one **thing**
- a **statement** references exactly one **graph**
- a **statement** references exactly one **thing**
- a **statement** references exactly one **property**
- a **firing** references exactly one **rule**
- a **firing** references exactly one **statement**
- a **disjoint check** references exactly one **graph**
- a **disjoint check** references exactly one **rule**
- a **disjoint check** references exactly one **statement**

## 3 Operative Rules

_Operative rules state what the business **obliges**, **prohibits**, or
advises (**should**). Structural rules come from required fields and foreign keys;
semantic rules come from the Constraints table, each keyed on a boolean the rulebook
already computes (cross-referenced as DR-N in the Definitional Rules below)._

### Structural Constraints (from the schema)

- A rule **must** have a label and an explanation, and record whether it is enabled.
- A thing **must** have a label and a kind.
- A property **must** have a label, and record whether it is symmetric.
- A statement **must** reference exactly one graph.
- A statement **must** reference exactly one thing as its subject.
- A statement **must** reference exactly one property.
- A statement **must** record whether it is stated.
- A firing **must** reference exactly one rule.
- A firing **must** reference exactly one statement as its target.
- A firing **must** reference exactly one statement as its first.
- A firing **must** reference exactly one statement as its second.
- A firing **must** have a kind.
- A disjoint check **must** reference exactly one graph.
- A disjoint check **must** reference exactly one rule.
- A disjoint check **must** reference exactly one statement as its a.
- A disjoint check **must** reference exactly one statement as its b.

## 4 Definitional Rules

_All statements express truth in the business domain; they are neither
procedures nor imperatives. "iff" is avoided in favor of "only if" so a
one-directional necessity is not mistaken for an equivalence. A
**⚠︎ mechanical** chip marks a rule whose deterministic wording is faithful
but clunky — a flag for an optional downstream reword pass, not a defect._

| ID | Declarative rule |
|----|------------------|
| **DR-1 Name** | A graph's name is the same as its graph ID. |
| **DR-2 Count of Stated Facts** | A graph's count of stated facts is the total stated unit across the statements related to the graph. |
| **DR-3 Count of Conclusions** | A graph's count of conclusions is the total conclusion unit across the statements related to the graph. |
| **DR-4 Count of Violations** | A graph's count of violations is the total violation unit across the disjoint checks related to the graph. |
| **DR-5 Is Consistent** | A graph is considered consistent if the count of violations is 0. |
| **DR-6 Name** | A rule's name is the same as its label. |
| **DR-7 Count of Firings** | A rule's count of firings is the total fired unit across the firings related to the rule. |
| **DR-8 Name** | A thing's name is the same as its label. |
| **DR-9 Count of Children** | A thing's count of children is the total parent unit across the statements related to the thing. |
| **DR-10 Count of Grandchildren** | A thing's count of grandchildren is the total grandparent unit across the statements related to the thing. |
| **DR-11 Name** | A property's name is the same as its label. |
| **DR-12 Subject Label** | A statement's subject label — taken from the linked subject. |
| **DR-13 Property Label** | A statement's property label — taken from the linked property. |
| **DR-14 Object Label** | A statement's object label — taken from the linked object thing. |
| **DR-15 Name** | A statement's name is computed as the subject label, followed by a space, followed by the property label, followed by a space, followed by the number value if the object thing is blank, in all other cases the object label. |
| **DR-16 Count of Inverse Firings** | A statement's count of inverse firings is the total inverse fired unit across the firings related to the statement. |
| **DR-17 Count of Symmetric Firings** | A statement's count of symmetric firings is the total symmetric fired unit across the firings related to the statement. |
| **DR-18 Holds Before Chain** | A statement is considered to hold a before chain if at least one of the following holds: the stated flag is set; the count of inverse firings is greater than 0; or the count of symmetric firings is greater than 0. |
| **DR-19 Count of Chain Firings** | A statement's count of chain firings is the total chain fired unit across the firings related to the statement. |
| **DR-20 Holds Before Typing** | A statement is considered to hold before typing if at least one of the following holds: the holds before chain flag is set or the count of chain firings is greater than 0. |
| **DR-21 Count of Type Firings** | A statement's count of type firings is the total type fired unit across the firings related to the statement. |
| **DR-22 Holds** | A statement is considered to hold if at least one of the following holds: the holds before typing flag is set or the count of type firings is greater than 0. |
| **DR-23 Is Conclusion** | A statement is considered a conclusion if all of the following hold: the holds flag is set and the stated flag is not set. |
| **DR-24 Worked Out by** | The statement's worked out by is determined by the following priority:<br>1. an empty string, if the conclusion flag is not set;<br>2. “inverse”, if the count of inverse firings is greater than 0;<br>3. “symmetric”, if the count of symmetric firings is greater than 0;<br>4. “property chain”, if the count of chain firings is greater than 0;<br>5. in all other cases, “domain and range”. |
| **DR-25 Stated Unit** | The statement's stated unit is determined by the following priority:<br>1. 1, if the stated flag is set;<br>2. in all other cases, 0. |
| **DR-26 Conclusion Unit** | The statement's conclusion unit is determined by the following priority:<br>1. 1, if the conclusion flag is set;<br>2. in all other cases, 0. |
| **DR-27 Parent Unit** | The statement's parent unit is determined by the following priority:<br>1. 1, if all of the following hold: the holds flag is set and the property is “isParentOf”;<br>2. in all other cases, 0. |
| **DR-28 Grandparent Unit** | The statement's grandparent unit is determined by the following priority:<br>1. 1, if all of the following hold: the holds flag is set and the property is “isGrandparentOf”;<br>2. in all other cases, 0. |
| **DR-29 Rule Enabled** | A firing's rule enabled when the linked rule is enabled. |
| **DR-30 Rule Label** | A firing's rule label — taken from the linked rule. |
| **DR-31 Target Name** | A firing's target name — taken from the linked target. |
| **DR-32 First Name** | A firing's first name — taken from the linked first. |
| **DR-33 Second Name** | A firing's second name — taken from the linked second. |
| **DR-34 First is Stated** | A firing's first is stated when the linked first is stated. |
| **DR-35 First Holds Before Chain** | A firing's first holds before chain when the linked first is holds before chain. |
| **DR-36 Second Holds Before Chain** | A firing's second holds before chain when the linked second is holds before chain. |
| **DR-37 First Holds Before Typing** | A firing's first holds before typing when the linked first is holds before typing. |
| **DR-38 Inverse Fired Unit** | The firing's inverse fired unit is determined by the following priority:<br>1. 1, if all of the following hold: the rule enabled flag is set; the kind is “inverse”; and the first is stated flag is set;<br>2. in all other cases, 0. |
| **DR-39 Symmetric Fired Unit** | The firing's symmetric fired unit is determined by the following priority:<br>1. 1, if all of the following hold: the rule enabled flag is set; the kind is “symmetric”; and the first is stated flag is set;<br>2. in all other cases, 0. |
| **DR-40 Chain Fired Unit** | The firing's chain fired unit is determined by the following priority:<br>1. 1, if all of the following hold: the rule enabled flag is set; the kind is “chain”; the first holds before chain flag is set; and the second holds before chain flag is set;<br>2. in all other cases, 0. |
| **DR-41 Type Fired Unit** | The firing's type fired unit is determined by the following priority:<br>1. 1, if all of the following hold: the rule enabled flag is set; at least one of the following holds: the kind is “domain” or the kind is “range”; and the first holds before typing flag is set;<br>2. in all other cases, 0. |
| **DR-42 Fired Unit** | A firing's fired unit is computed as the inverse fired unit plus the symmetric fired unit plus the chain fired unit plus the type fired unit. |
| **DR-43 Name** | The firing's name is determined by the following priority:<br>1. the target name, followed by “: concluded by the ”, followed by the rule label, followed by “ rule from '”, followed by the first name, followed by “'”, followed by “ and '”, followed by the second name, followed by “'”, if the fired unit is greater than 0;<br>2. in all other cases, the firing ID. |
| **DR-44 Name** | A disjoint check's name is the same as its disjoint check ID. |
| **DR-45 Rule Enabled** | A disjoint check's rule enabled when the linked rule is enabled. |
| **DR-46 A Holds** | A disjoint check's a holds when the linked a is holds. |
| **DR-47 B Holds** | A disjoint check's b holds when the linked b is holds. |
| **DR-48 Violation Unit** | The disjoint check's violation unit is determined by the following priority:<br>1. 1, if all of the following hold: the rule enabled flag is set; the a holds flag is set; and the b holds flag is set;<br>2. in all other cases, 0. |

## 5 Traceability to Schema

_The expression column is the rule's definition in Docs notation —
the same logic the rulebook stores, written for a business reader._

| Schema element | Kind | Expression |
|----------------|------|------------|
| **Graphs.Name** | formula | `GraphId` |
| **Graphs.CountOfStatedFacts** | rollup | `Sum(Statements.StatedUnit via Graph)` |
| **Graphs.CountOfConclusions** | rollup | `Sum(Statements.ConclusionUnit via Graph)` |
| **Graphs.CountOfViolations** | rollup | `Sum(DisjointChecks.ViolationUnit via Graph)` |
| **Graphs.IsConsistent** | formula | `CountOfViolations = 0` |
| **Rules.Name** | formula | `Label` |
| **Rules.CountOfFirings** | rollup | `Sum(Firings.FiredUnit via Rule)` |
| **Things.Name** | formula | `Label` |
| **Things.CountOfChildren** | rollup | `Sum(Statements.ParentUnit via Subject)` |
| **Things.CountOfGrandchildren** | rollup | `Sum(Statements.GrandparentUnit via Subject)` |
| **Properties.Name** | formula | `Label` |
| **Statements.SubjectLabel** | lookup | `Lookup(Things.Label via Subject)` |
| **Statements.PropertyLabel** | lookup | `Lookup(Properties.Label via Property)` |
| **Statements.ObjectLabel** | lookup | `Lookup(Things.Label via ObjectThing)` |
| **Statements.Name** | formula | `SubjectLabel & " " & PropertyLabel & " " & If(ObjectThing = Blank(), NumberValue, ObjectLabel)` |
| **Statements.CountOfInverseFirings** | rollup | `Sum(Firings.InverseFiredUnit via Target)` |
| **Statements.CountOfSymmetricFirings** | rollup | `Sum(Firings.SymmetricFiredUnit via Target)` |
| **Statements.HoldsBeforeChain** | formula | `Or(IsStated, CountOfInverseFirings > 0, CountOfSymmetricFirings > 0)` |
| **Statements.CountOfChainFirings** | rollup | `Sum(Firings.ChainFiredUnit via Target)` |
| **Statements.HoldsBeforeTyping** | formula | `Or(HoldsBeforeChain, CountOfChainFirings > 0)` |
| **Statements.CountOfTypeFirings** | rollup | `Sum(Firings.TypeFiredUnit via Target)` |
| **Statements.Holds** | formula | `Or(HoldsBeforeTyping, CountOfTypeFirings > 0)` |
| **Statements.IsConclusion** | formula | `And(Holds, Not(IsStated))` |
| **Statements.WorkedOutBy** | formula | `If(Not(IsConclusion), "", If(CountOfInverseFirings > 0, "inverse", If(CountOfSymmetricFirings > 0, "symmetric", If(CountOfChainFirings > 0, "property chain", "domain and range"))))` |
| **Statements.StatedUnit** | formula | `If(IsStated, 1, 0)` |
| **Statements.ConclusionUnit** | formula | `If(IsConclusion, 1, 0)` |
| **Statements.ParentUnit** | formula | `If(And(Holds, Property = "isParentOf"), 1, 0)` |
| **Statements.GrandparentUnit** | formula | `If(And(Holds, Property = "isGrandparentOf"), 1, 0)` |
| **Firings.RuleEnabled** | lookup | `Lookup(Rules.IsEnabled via Rule)` |
| **Firings.RuleLabel** | lookup | `Lookup(Rules.Label via Rule)` |
| **Firings.TargetName** | lookup | `Lookup(Statements.Name via Target)` |
| **Firings.FirstName** | lookup | `Lookup(Statements.Name via First)` |
| **Firings.SecondName** | lookup | `Lookup(Statements.Name via Second)` |
| **Firings.FirstIsStated** | lookup | `Lookup(Statements.IsStated via First)` |
| **Firings.FirstHoldsBeforeChain** | lookup | `Lookup(Statements.HoldsBeforeChain via First)` |
| **Firings.SecondHoldsBeforeChain** | lookup | `Lookup(Statements.HoldsBeforeChain via Second)` |
| **Firings.FirstHoldsBeforeTyping** | lookup | `Lookup(Statements.HoldsBeforeTyping via First)` |
| **Firings.InverseFiredUnit** | formula | `If(And(RuleEnabled, Kind = "inverse", FirstIsStated), 1, 0)` |
| **Firings.SymmetricFiredUnit** | formula | `If(And(RuleEnabled, Kind = "symmetric", FirstIsStated), 1, 0)` |
| **Firings.ChainFiredUnit** | formula | `If(And(RuleEnabled, Kind = "chain", FirstHoldsBeforeChain, SecondHoldsBeforeChain), 1, 0)` |
| **Firings.TypeFiredUnit** | formula | `If(And(RuleEnabled, Or(Kind = "domain", Kind = "range"), FirstHoldsBeforeTyping), 1, 0)` |
| **Firings.FiredUnit** | formula | `InverseFiredUnit + SymmetricFiredUnit + ChainFiredUnit + TypeFiredUnit` |
| **Firings.Name** | formula | `If(FiredUnit > 0, TargetName & ": concluded by the " & RuleLabel & " rule from '" & FirstName & "'" & If(Kind = "chain", " and '" & SecondName & "'", ""), FiringId)` |
| **DisjointChecks.Name** | formula | `DisjointCheckId` |
| **DisjointChecks.RuleEnabled** | lookup | `Lookup(Rules.IsEnabled via Rule)` |
| **DisjointChecks.AHolds** | lookup | `Lookup(Statements.Holds via A)` |
| **DisjointChecks.BHolds** | lookup | `Lookup(Statements.Holds via B)` |
| **DisjointChecks.ViolationUnit** | formula | `If(And(RuleEnabled, AHolds, BHolds), 1, 0)` |
