# 📘 South Park ontology, in SDLAF — Docs

_Jessica Talisman's South Park ontology as an ordinary rulebook: her ten stated facts are rows and columns of the tables they are about (people, parenthoods, marriages, friendships, grade and role records), and each of her five rules is a field: the inverse is the parenthood read from the child's end, the symmetric rule a link with no direction counted from both ends, the property chain a lookup through two parenthoods, domain and range a formula and a count on each individual, disjointness a check on each parenthood. Every rule is switched by the one Reasoning row. Nothing is a generic subject-property-object store: Schema, Data, Lookups, Aggregations and Formulas only._

> Declarative business rules rendered from the rulebook. Every statement
> below expresses truth in the business domain — it is neither a procedure
> nor an imperative. The rulebook's formulas are the single source of truth;
> this document is their plain-language reading.

## 1 Business Vocabulary

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Reasoning** | The one world the rules reason over, and the switch of each of her five rules. Every table links here, so each rule's switch is one lookup away from the field it gates, and every count of the world rolls up here. | — |
| Name | The same as its label. | _Order 1. The world's name._ |
| Label | A defined attribute. | _The world's name._ |
| Inverse is on | True when an empty string. | _Inverse rule: a parent link also gives a child link between the same two people (isChildOf, the inverse of isParentOf)._ |
| Symmetric is on | True when an empty string. | _Symmetric rule: a spouse link or a friend link holds for both people._ |
| Chain is on | True when an empty string. | _Property chain: a parent of a parent is a grandparent (isParentOf then isParentOf gives isGrandparentOf)._ |
| Domain Range is on | True when an empty string. | _Domain and range: whoever attends something, or has a grade level or a role, is a Person; whatever is attended is a School._ |
| Disjoint is on | True when an empty string. | _Disjoint: no one is both the parent and the spouse of the same person (isParentOf is disjoint with isSpouseOf)._ |
| Count of Individuals | The number of individuals related to the reasoning. | _Order 1. Every individual of the world._ |
| Count of People | The number of the reasoning's individuals that are people. | _Order 3. Individuals that are a Person, asserted or concluded._ |
| Count of Schools | The number of the reasoning's individuals that are schools. | _Order 3. Individuals that are a School, asserted or concluded._ |
| Count of Stated Parenthoods | The number of parenthoods related to the reasoning. | _Order 1. Stated isParentOf facts: one per Parenthoods row._ |
| Count of Stated Marriages | The number of marriages related to the reasoning. | _Order 1. Stated isSpouseOf facts: one per Marriages row._ |
| Count of Stated Friendships | The number of friendships related to the reasoning. | _Order 1. Stated isFriendOf facts: one per Friendships row._ |
| Count of Stated Individual Facts | The total stated fact count across the individuals related to the reasoning. | _Order 3. Stated facts held as columns of an individual: attends, has age and is a._ |
| Count of Current Grade Levels | The number of the reasoning's grade level records that are current. | _Order 3. Grade level facts in effect: each person's current grade._ |
| Count of Current Roles | The number of the reasoning's role records that are current. | _Order 3. Role facts in effect: each person's current role._ |
| Count of Stated Facts | Computed as the count of stated parenthoods plus the count of stated marriages plus the count of stated friendships plus the count of stated individual facts plus the count of current grade levels plus the count of current roles. | _Order 4. Every stated fact in effect, wherever it is held._ |
| Count of Child of Conclusions | The number of the reasoning's parenthoods that are child of holds. | _Order 2. isChildOf facts the inverse rule concludes: one per parenthood._ |
| Count of Mirrored Marriages | The number of the reasoning's marriages that are mirror holds. | _Order 2. isSpouseOf facts the symmetric rule concludes: one per marriage, read from its second partner._ |
| Count of Mirrored Friendships | The number of the reasoning's friendships that are mirror holds. | _Order 2. isFriendOf facts the symmetric rule concludes: one per friendship, read from its second friend._ |
| Count of Grandparent Conclusions | The number of the reasoning's grandparenthoods that hold. | _Order 4. isGrandparentOf facts the property chain concludes._ |
| Count of Person Conclusions | The number of the reasoning's individuals that are person-concluded. | _Order 3. Individuals the domain rule makes a Person that no one asserted was one._ |
| Count of School Conclusions | The number of the reasoning's individuals that are school-concluded. | _Order 3. Individuals the range rule makes a School that no one asserted was one._ |
| Count of Conclusions | Computed as the count of child of conclusions plus the count of mirrored marriages plus the count of mirrored friendships plus the count of grandparent conclusions plus the count of person conclusions plus the count of school conclusions. | _Order 5. Every fact the rules conclude that no one stated._ |
| Count of Contradictions | The number of the reasoning's parenthoods that are contradictions. | _Order 4. Parenthoods the disjoint rule flags: the parent is also the child's spouse._ |
| Is Consistent | True when the count of contradictions is 0. | _Order 5. No contradiction the disjoint rule can see._ |
| Summary | Computed as the count of stated facts, followed by “ stated facts, ”, followed by the count of conclusions, followed by “ conclusions, ”, followed by the count of contradictions, followed by “ contradictions”. | _Order 6. The world in one line._ |

<details open>
<summary>Example data</summary>

| Name ƒ | Label | Inverse is on | Symmetric is on | Chain is on | Domain Range is on | Disjoint is on | Count of Individuals ƒ | Count of People ƒ | Count of Schools ƒ | Count of Stated Parenthoods ƒ | Count of Stated Marriages ƒ | Count of Stated Friendships ƒ | Count of Stated Individual Facts ƒ | Count of Current Grade Levels ƒ | Count of Current Roles ƒ | Count of Stated Facts ƒ | Count of Child of Conclusions ƒ | Count of Mirrored Marriages ƒ | Count of Mirrored Friendships ƒ | Count of Grandparent Conclusions ƒ | Count of Person Conclusions ƒ | Count of School Conclusions ƒ | Count of Conclusions ƒ | Count of Contradictions ƒ | Is Consistent ƒ | Summary ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| South Park | South Park | true | true | true | true | true | 7 | 1 | 1 | 3 | 1 | 1 | 3 | 1 | 1 | 10 | 3 | 1 | 1 | 2 | 0 | 1 | 8 | 0 | true | 10 stated facts, 8 conclusions, 0 contradictions |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Portrait** | The picture of each individual, linked from its Fandom page (never downloaded). | — |
| Display Name | A defined attribute. | _The name on the portrait's Fandom page._ |
| Page | A defined attribute. | _The Fandom page credited for the portrait._ |
| Image | A defined attribute. | _The original Fandom image._ |
| Crop | A defined attribute. | _The crop as an SVG viewBox: x y width height._ |
| Width | A defined attribute. | _The original image's width in pixels._ |
| Height | A defined attribute. | _The original image's height in pixels._ |
| Sort Order | A defined attribute. | _Position in the credits._ |
| Name | The same as its display name. | _Order 1. The portrait's name._ |

<details open>
<summary>Example data</summary>

| Display Name | Page | Image | Crop | Width | Height | Sort Order | Name ƒ |
|---|---|---|---|---|---|---|---|
| Marvin Marsh | https://southpark.fandom.com/wiki/Marvin_Marsh | https://static.wikia.nocookie.net/southpark/images/d/db/Marsh-family-grandpa-marsh-w-purple-blanket.png/revision/latest | 48 -5 280 280 | 375 | 536 | 1 | Marvin Marsh |
| Randy Marsh | https://southpark.fandom.com/wiki/Randy_Marsh | https://static.wikia.nocookie.net/southparkfanon/images/d/d6/Randy_Marsh.png/revision/latest | 0 -10 428 428 | 428 | 1057 | 2 | Randy Marsh |
| Sharon Marsh | https://southpark.fandom.com/wiki/Sharon_Marsh | https://static.wikia.nocookie.net/southpark/images/9/99/Sharon_Marsh.png/revision/latest | 20 -10 390 390 | 439 | 1049 | 3 | Sharon Marsh |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Individual** | Every individual of the world (owl:Thing). What is said about one individual alone is a column of its row: its age, the place it attends, whether it is asserted to be a Person or a School. Its current grade level and role come from their records. Class membership is a formula, not a table, because it is concluded: no one stated that South Park Elementary is a School. | — |
| Name | The same as its label. | _Order 1. The individual's name._ |
| Label | A defined attribute. | _The individual's name._ |
| Sort Order | A defined attribute. | _Position in the cast._ |
| Reasoning | A defined attribute. | _The world this individual belongs to._ |
| Portrait | A defined attribute. | _The individual's picture._ |
| Portrait Image | Taken from the linked portrait. | _Order 1. The picture's image._ |
| Symmetric is on | True when the linked reasoning is on. | _Order 1. The Symmetric rule's switch, read from the reasoning this row belongs to._ |
| Domain Range is on | True when the linked reasoning is on. | _Order 1. The DomainRange rule's switch, read from the reasoning this row belongs to._ |
| Is Asserted Person | True when an empty string. | _Stated: this individual is a Person (isA Person)._ |
| Is Asserted School | True when an empty string. | _Stated: this individual is a School (isA School)._ |
| Age | A defined attribute. | _Stated: hasAge._ |
| Attends | A defined attribute. | _Stated: the place this individual attends (attends). Its range is School._ |
| Attends Label | Taken from the linked attends. | _Order 1. The name of the place attended._ |
| Has Age | True when the age has a value. | _Order 1. An age is stated._ |
| Has Attends | True when the attends has a value. | _Order 1. A place attended is stated._ |
| Stated Fact Count | Computed as the count of the following that hold: the asserted person flag is set; the asserted school flag is set; the age flag is set; and the attends flag is set. | _Order 2. Stated facts held on this row: is a Person, is a School, has age, attends._ |
| Count of Grade Levels on | The number of the individual's grade level records that are on. | _Order 1. Grade level records switched on for this individual._ |
| Latest Grade Level Seq | The largest stated seq across the individual's grade level records that are on. | _Order 1. When the latest grade level switched on was stated: the one in effect._ |
| Has Grade Level | True when the count of grade levels on is greater than 0. | _Order 2. A grade level is in effect._ |
| Current Grade Level Record | Determined by priority: the individual ID, followed by “|grade|”, followed by the latest grade level seq if the grade level flag is set; in all other cases, an empty string. | _Order 3. Key of the grade level record in effect._ |
| Grade Level | Taken from the linked current grade level record. | _Order 4. The grade level in effect: the latest one switched on (hasGradeLevel keeps one current value)._ |
| Count of Roles on | The number of the individual's role records that are on. | _Order 1. Role records switched on for this individual._ |
| Latest Role Seq | The largest stated seq across the individual's role records that are on. | _Order 1. When the latest role switched on was stated: the one in effect._ |
| Has Role | True when the count of roles on is greater than 0. | _Order 2. A role is in effect._ |
| Current Role Record | Determined by priority: the individual ID, followed by “|role|”, followed by the latest role seq if the role flag is set; in all other cases, an empty string. | _Order 3. Key of the role record in effect._ |
| Role | Taken from the linked current role record. | _Order 4. The role in effect: the latest one switched on (hasRole keeps one current value)._ |
| Has Person Evidence | True when at least one of the following holds: the attends flag is set; the grade level flag is set; or the role flag is set. | _Order 3. Something is stated that only a Person can have: a place attended, a grade level or a role (the domain of attends, hasGradeLevel and hasRole)._ |
| Is Person by Domain | True when all of the following hold: the domain range is on flag is set and the person evidence flag is set. | _Order 4. The domain rule makes this individual a Person._ |
| Is Person | True when the IF of the OR of the is asserted person and the is person by domain and true. | _Order 5. A Person, asserted or concluded. Open world: true, or not known._ |
| Is Person Concluded | True when the IF of the AND of the is person by domain and the asserted person flag is not set and true. | _Order 5. A Person only because the domain rule says so._ |
| Attended by Count | The number of individuals related to the individual. | _Order 1. Individuals that attend this one._ |
| Is School by Range | True when all of the following hold: the domain range is on flag is set and the attended by count is greater than 0. | _Order 2. The range rule makes this individual a School: someone attends it._ |
| Is School | True when the IF of the OR of the is asserted school and the is school by range and true. | _Order 3. A School, asserted or concluded. Open world: true, or not known._ |
| Is School Concluded | True when the IF of the AND of the is school by range and the asserted school flag is not set and true. | _Order 3. A School only because the range rule says so._ |
| Child Count | The number of parenthoods related to the individual. | _Order 1. Children: parenthoods where this individual is the parent (isParentOf)._ |
| Parent Count | The number of parenthoods related to the individual. | _Order 1. Parents: the same parenthoods read from the child's end. The link is there whatever the inverse rule says._ |
| Child of Fact Count | The number of the individual's parenthoods that are child of holds. | _Order 2. isChildOf facts about this individual: its parenthoods read from its end, when the inverse rule is on._ |
| Grandchild Count | The number of the individual's grandparenthoods that hold. | _Order 5. Grandchildren the property chain concludes (isGrandparentOf)._ |
| Grandparent Count | The number of the individual's grandparenthoods that hold. | _Order 5. Grandparents the property chain concludes, read from the grandchild's end._ |
| Descendant Count | The number of vw parenthoods closure related to the individual. | _Order 1. Everyone below this individual through one or more parenthoods: the transitive closure, which her page does not draw._ |
| Ancestor Count | The number of vw parenthoods closure related to the individual. | _Order 1. Everyone above this individual through one or more parenthoods._ |
| Stated Spouse Count | The number of marriages related to the individual. | _Order 2. Marriages that name this individual first: the stated isSpouseOf._ |
| Mirrored Spouse Count | The number of the individual's marriages that are mirror holds. | _Order 3. Marriages that name this individual second, when the symmetric rule reads them both ways._ |
| Spouse Count | Computed as the stated spouse count plus the mirrored spouse count. | _Order 4. Spouses: isSpouseOf facts about this individual, stated or mirrored._ |
| Stated Friend Count | The number of friendships related to the individual. | _Order 2. Friendships that name this individual first: the stated isFriendOf._ |
| Mirrored Friend Count | The number of the individual's friendships that are mirror holds. | _Order 3. Friendships that name this individual second, when the symmetric rule reads them both ways._ |
| Friend Count | Computed as the stated friend count plus the mirrored friend count. | _Order 4. Friends: isFriendOf facts about this individual, stated or mirrored._ |

<details open>
<summary>Example data</summary>

| Name ƒ | Label | Sort Order | Portrait Image ƒ | Symmetric is on ƒ | Domain Range is on ƒ | Is Asserted Person | Is Asserted School | Age | Attends Label ƒ | Has Age ƒ | Has Attends ƒ | Stated Fact Count ƒ | Count of Grade Levels on ƒ | Latest Grade Level Seq ƒ | Has Grade Level ƒ | Current Grade Level Record ƒ | Grade Level ƒ | Count of Roles on ƒ | Latest Role Seq ƒ | Has Role ƒ | Current Role Record ƒ | Role ƒ | Has Person Evidence ƒ | Is Person by Domain ƒ | Is Person ƒ | Is Person Concluded ƒ | Attended by Count ƒ | Is School by Range ƒ | Is School ƒ | Is School Concluded ƒ | Child Count ƒ | Parent Count ƒ | Child of Fact Count ƒ | Grandchild Count ƒ | Grandparent Count ƒ | Descendant Count ƒ | Ancestor Count ƒ | Stated Spouse Count ƒ | Mirrored Spouse Count ƒ | Spouse Count ƒ | Stated Friend Count ƒ | Mirrored Friend Count ƒ | Friend Count ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| Marvin | Marvin | 1 | https://static.wikia.nocookie.net/southpark/images/d/db/Marsh-family-grandpa-marsh-w-purple-blanket.png/revision/latest | true | true | false | false | — | — | false | false | 0 | 0 | — | false | — | — | 0 | — | false | — | — | false | false | — | — | 0 | false | — | — | 1 | 0 | 0 | 2 | 0 | 3 | 0 | 0 | 0 | 0 | 0 | 0 | 0 |
| Randy | Randy | 2 | https://static.wikia.nocookie.net/southparkfanon/images/d/d6/Randy_Marsh.png/revision/latest | true | true | false | false | — | — | false | false | 0 | 0 | — | false | — | — | 0 | — | false | — | — | false | false | — | — | 0 | false | — | — | 2 | 1 | 1 | 0 | 0 | 2 | 1 | 1 | 0 | 1 | 0 | 0 | 0 |
| Sharon | Sharon | 3 | https://static.wikia.nocookie.net/southpark/images/9/99/Sharon_Marsh.png/revision/latest | true | true | false | false | — | — | false | false | 0 | 0 | — | false | — | — | 0 | — | false | — | — | false | false | — | — | 0 | false | — | — | 0 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | 1 | 1 | 0 | 0 | 0 |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Parenthood** | One row per stated isParentOf fact. A person has many children and a child more than one parent, so the link is its own entity with two links to Individuals. The inverse rule's isChildOf is not stored: it is this row read from the child's end. | — |
| Name | Computed as the parent label, followed by “ is parent of ”, followed by the child label. | _Order 2. The stated fact in words._ |
| Reasoning | A defined attribute. | _The world this fact belongs to._ |
| Parent | A defined attribute. | _The parent (the subject of isParentOf)._ |
| Child | A defined attribute. | _The child (the object of isParentOf)._ |
| Parent Label | Taken from the linked parent. | _Order 1. The parent's name._ |
| Child Label | Taken from the linked child. | _Order 1. The child's name._ |
| Inverse is on | True when the linked reasoning is on. | _Order 1. The Inverse rule's switch, read from the reasoning this row belongs to._ |
| Symmetric is on | True when the linked reasoning is on. | _Order 1. The Symmetric rule's switch, read from the reasoning this row belongs to._ |
| Disjoint is on | True when the linked reasoning is on. | _Order 1. The Disjoint rule's switch, read from the reasoning this row belongs to._ |
| Child of Holds | True when the IF of the inverse is on and true. | _Order 2. The inverse rule concludes the child is child of the parent. Open world: true, or not known._ |
| Child of Sentence | Computed as the child label, followed by “ is child of ”, followed by the parent label. | _Order 2. The inverse fact in words._ |
| Spouse As Stated Count | The number of marriages related to the parenthood. | _Order 3. Marriages stated with the parent first and the child second._ |
| Spouse As Mirrored Count | The number of marriages related to the parenthood. | _Order 3. Marriages stated with the child first, which make the parent the child's spouse when the symmetric rule mirrors them._ |
| Is Contradiction | True when the IF of the AND of the disjoint is on and the spouse as stated count plus the spouse as mirrored count is greater than 0 and true. | _Order 4. The disjoint rule: the parent is also the child's spouse. Open world: true, or not known._ |
| Ancestry Closure | A defined attribute. | _Transitive closure of isParentOf (an owl:TransitiveProperty reading of it, which her page does not draw): every (ancestor, descendant) pair through one or more parenthoods, materialized as vw_parenthoods_closure._ |

<details open>
<summary>Example data</summary>

| Name ƒ | Parent Label ƒ | Child Label ƒ | Inverse is on ƒ | Symmetric is on ƒ | Disjoint is on ƒ | Child of Holds ƒ | Child of Sentence ƒ | Spouse As Stated Count ƒ | Spouse As Mirrored Count ƒ | Is Contradiction ƒ |
|---|---|---|---|---|---|---|---|---|---|---|
| Marvin is parent of Randy | Marvin | Randy | true | true | true | true | Randy is child of Marvin | 0 | 0 | — |
| Randy is parent of Stan | Randy | Stan | true | true | true | true | Stan is child of Randy | 0 | 0 | — |
| Randy is parent of Shelly | Randy | Shelly | true | true | true | true | Shelly is child of Randy | 0 | 0 | — |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Marriage** | One row per stated isSpouseOf fact. A marriage has no direction: the symmetric rule is the same row counted from its second partner's end. | — |
| Name | Computed as the partner a label, followed by “ is spouse of ”, followed by the partner b label. | _Order 2. The stated fact in words._ |
| Reasoning | A defined attribute. | _The world this fact belongs to._ |
| Partner a | A defined attribute. | _The partner the stated fact names first._ |
| Partner B | A defined attribute. | _The partner the stated fact names second._ |
| Partner a Label | Taken from the linked partner a. | _Order 1. The partnera's name._ |
| Partner B Label | Taken from the linked partner b. | _Order 1. The partnerb's name._ |
| Symmetric is on | True when the linked reasoning is on. | _Order 1. The Symmetric rule's switch, read from the reasoning this row belongs to._ |
| Mirror Holds | True when the IF of the symmetric is on and true. | _Order 2. The symmetric rule concludes the second partner is spouse of the first. Open world: true, or not known._ |
| Mirror Sentence | Computed as the partner b label, followed by “ is spouse of ”, followed by the partner a label. | _Order 2. The mirrored fact in words._ |

<details open>
<summary>Example data</summary>

| Name ƒ | Partner a Label ƒ | Partner B Label ƒ | Symmetric is on ƒ | Mirror Holds ƒ | Mirror Sentence ƒ |
|---|---|---|---|---|---|
| Randy is spouse of Sharon | Randy | Sharon | true | true | Sharon is spouse of Randy |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Friendship** | One row per stated isFriendOf fact, read both ways by the symmetric rule, like a marriage. | — |
| Name | Computed as the friend a label, followed by “ is friend of ”, followed by the friend b label. | _Order 2. The stated fact in words._ |
| Reasoning | A defined attribute. | _The world this fact belongs to._ |
| Friend a | A defined attribute. | _The friend the stated fact names first._ |
| Friend B | A defined attribute. | _The friend the stated fact names second._ |
| Friend a Label | Taken from the linked friend a. | _Order 1. The frienda's name._ |
| Friend B Label | Taken from the linked friend b. | _Order 1. The friendb's name._ |
| Symmetric is on | True when the linked reasoning is on. | _Order 1. The Symmetric rule's switch, read from the reasoning this row belongs to._ |
| Mirror Holds | True when the IF of the symmetric is on and true. | _Order 2. The symmetric rule concludes the second friend is friend of the first. Open world: true, or not known._ |
| Mirror Sentence | Computed as the friend b label, followed by “ is friend of ”, followed by the friend a label. | _Order 2. The mirrored fact in words._ |

<details open>
<summary>Example data</summary>

| Name ƒ | Friend a Label ƒ | Friend B Label ƒ | Symmetric is on ƒ | Mirror Holds ƒ | Mirror Sentence ƒ |
|---|---|---|---|---|---|
| Stan is friend of Kyle | Stan | Kyle | true | true | Kyle is friend of Stan |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Grandparenthood** | The property chain isParentOf then isParentOf, as rows: every ordered pair of parenthoods is a candidate, and it holds when the first one's child is the second one's parent. A formula cannot add a row, so the candidates are data (every pair, decided by nothing about the family) and whether one holds is a formula. | — |
| Name | Determined by priority: the grandparent label, followed by “ is grandparent of ”, followed by the grandchild label if the links flag is set; in all other cases, “(”, followed by the upper link, followed by “ does not lead to ”, followed by the lower link, followed by “)”. | _Order 4. The concluded fact in words, or why this pair concludes nothing._ |
| Reasoning | A defined attribute. | _The world this candidate belongs to._ |
| Upper Link | A defined attribute. | _The first link: grandparent to the middle generation._ |
| Lower Link | A defined attribute. | _The second link: the middle generation to the grandchild._ |
| Grandparent | Taken from the linked upper link. | _Order 1. The upper link's parent._ |
| Upper Child | Taken from the linked upper link. | _Order 1. The upper link's child._ |
| Lower Parent | Taken from the linked lower link. | _Order 1. The lower link's parent._ |
| Grandchild | Taken from the linked lower link. | _Order 1. The lower link's child._ |
| Grandparent Label | Taken from the linked upper link. | _Order 2. The grandparent's name._ |
| Grandchild Label | Taken from the linked lower link. | _Order 2. The grandchild's name._ |
| Links | True when the upper child is the lower parent. | _Order 2. The upper link's child is the lower link's parent: the two links make a chain._ |
| Chain is on | True when the linked reasoning is on. | _Order 1. The Chain rule's switch, read from the reasoning this row belongs to._ |
| Holds | True when the IF of the AND of the links and the chain is on and true. | _Order 3. The property chain concludes isGrandparentOf. Open world: true, or not known._ |

<details open>
<summary>Example data</summary>

| Name ƒ | Grandparent ƒ | Upper Child ƒ | Lower Parent ƒ | Grandchild ƒ | Grandparent Label ƒ | Grandchild Label ƒ | Links ƒ | Chain is on ƒ | Holds ƒ |
|---|---|---|---|---|---|---|---|---|---|
| (Marvin\|isParentOf\|Randy does not lead to Marvin\|isParentOf\|Randy) | Marvin | Randy | Marvin | Randy | Marvin | Randy | false | true | — |
| Marvin is grandparent of Stan | Marvin | Randy | Randy | Stan | Marvin | Stan | true | true | true |
| Marvin is grandparent of Shelly | Marvin | Randy | Randy | Shelly | Marvin | Shelly | true | true | true |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Grade Level Record** | Every grade level stated for an individual, kept when a later one replaces it: her page keeps one current grade, the latest switched on. The current value is time (which was stated last), not a flag. | — |
| Name | Computed as the individual label, followed by “ has grade level ”, followed by the grade level. | _Order 2. The fact in words._ |
| Reasoning | A defined attribute. | _The world this record belongs to._ |
| Individual | A defined attribute. | _Whose value it is._ |
| Individual Label | Taken from the linked individual. | _Order 1. Whose value it is, by name._ |
| Grade Level | A defined attribute. | _The value stated._ |
| Stated Seq | A defined attribute. | _When the value was switched on, in the order facts are stated (transaction time). The latest one switched on is in effect._ |
| Is on | True when an empty string. | _The value is switched on. A value switched off stays in the record._ |
| Individual Latest Seq | The latest grade level seq of the grade level record's individual. | _Order 2. When the individual's latest value switched on was stated._ |
| Is Current | True when all of the following hold: the on flag is set and the stated seq is the individual latest seq. | _Order 3. This value is the one in effect: switched on, and the latest switched on._ |
| Key is Canonical | True when the grade level record ID is the individual, followed by “|grade|”, followed by the stated seq. | _Order 1. The key is individual\|grade\|StatedSeq, the key the individual's current value is looked up by._ |

<details open>
<summary>Example data</summary>

| Name ƒ | Individual Label ƒ | Grade Level | Stated Seq | Is on | Individual Latest Seq ƒ | Is Current ƒ | Key is Canonical ƒ |
|---|---|---|---|---|---|---|---|
| Stan has grade level 4 | Stan | 4 | 8 | true | 8 | true | true |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Role Record** | Every role stated for an individual; the latest one switched on is in effect, like a grade level. | — |
| Name | Computed as the individual label, followed by “ has role ”, followed by the role. | _Order 2. The fact in words._ |
| Reasoning | A defined attribute. | _The world this record belongs to._ |
| Individual | A defined attribute. | _Whose value it is._ |
| Individual Label | Taken from the linked individual. | _Order 1. Whose value it is, by name._ |
| Role | A defined attribute. | _The value stated._ |
| Stated Seq | A defined attribute. | _When the value was switched on, in the order facts are stated (transaction time). The latest one switched on is in effect._ |
| Is on | True when an empty string. | _The value is switched on. A value switched off stays in the record._ |
| Individual Latest Seq | The latest role seq of the role record's individual. | _Order 2. When the individual's latest value switched on was stated._ |
| Is Current | True when all of the following hold: the on flag is set and the stated seq is the individual latest seq. | _Order 3. This value is the one in effect: switched on, and the latest switched on._ |
| Key is Canonical | True when the role record ID is the individual, followed by “|role|”, followed by the stated seq. | _Order 1. The key is individual\|role\|StatedSeq, the key the individual's current value is looked up by._ |

<details open>
<summary>Example data</summary>

| Name ƒ | Individual Label ƒ | Role | Stated Seq | Is on | Individual Latest Seq ƒ | Is Current ƒ | Key is Canonical ƒ |
|---|---|---|---|---|---|---|---|
| Stan has role Student | Stan | Student | 9 | true | 9 | true | true |

_ƒ marks a computed column._

</details>

## 2 Fact Types

- an **individual** may reference one **portrait**
- an **individual** may reference one **individual**
- a **parenthood** references exactly one **individual**
- a **marriage** references exactly one **individual**
- a **friendship** references exactly one **individual**
- a **grandparenthood** references exactly one **parenthood**
- a **grade level record** references exactly one **individual**
- a **role record** references exactly one **individual**

## 2b Reachability Rules

_A reachability rule is a transitive closure: relationships that hold not only
directly but through any chain of the same relationship. The asserted edges are
the single source of truth; the inferred edges are necessary consequences of them._

- **Ancestry Closure** — one parenthood is reachable from another by the **ancestry** relationship
  when the second can be reached from the first by following one or more **ancestry** edges
  (from its parent to its child), whether directly asserted or reached transitively.
  - An edge is **asserted** when it exists directly in the parenthoods; it is **inferred**
    when no direct edge states it but it follows from a chain of asserted edges.
  - The **hop distance** of a reachable pair is the length of the shortest such chain
    (1 for a directly-asserted edge).
  - _Transitive closure of isParentOf (an owl:TransitiveProperty reading of it, which her page does not draw): every (ancestor, descendant) pair through one or more parenthoods, materialized as vw_parenthoods_closure._

## 3 Operative Rules

_Operative rules state what the business **obliges**, **prohibits**, or
advises (**should**). Structural rules come from required fields and foreign keys;
semantic rules come from the Constraints table, each keyed on a boolean the rulebook
already computes (cross-referenced as DR-N in the Definitional Rules below)._

### Structural Constraints (from the schema)

- A reasoning **must** have a label, and record whether it is on; whether it is on; whether it is on; whether it is on; and whether it is on.
- A portrait **must** have a display name; a page; an image; a crop; a width; a height; and a sort order.
- An individual **must** reference exactly one reasoning.
- An individual **must** have a label and a sort order, and record whether it is an asserted person and whether it is an asserted school.
- A parenthood **must** reference exactly one reasoning.
- A parenthood **must** reference exactly one individual as its parent.
- A parenthood **must** reference exactly one individual as its child.
- A marriage **must** reference exactly one reasoning.
- A marriage **must** reference exactly one individual as its partner a.
- A marriage **must** reference exactly one individual as its partner b.
- A friendship **must** reference exactly one reasoning.
- A friendship **must** reference exactly one individual as its friend a.
- A friendship **must** reference exactly one individual as its friend b.
- A grandparenthood **must** reference exactly one reasoning.
- A grandparenthood **must** reference exactly one parenthood as its upper link.
- A grandparenthood **must** reference exactly one parenthood as its lower link.
- A grade level record **must** reference exactly one reasoning.
- A grade level record **must** reference exactly one individual.
- A grade level record **must** have a grade level and a stated seq, and record whether it is on.
- A role record **must** reference exactly one reasoning.
- A role record **must** reference exactly one individual.
- A role record **must** have a role and a stated seq, and record whether it is on.

## 4 Definitional Rules

_All statements express truth in the business domain; they are neither
procedures nor imperatives. "iff" is avoided in favor of "only if" so a
one-directional necessity is not mistaken for an equivalence. A
**⚠︎ mechanical** chip marks a rule whose deterministic wording is faithful
but clunky — a flag for an optional downstream reword pass, not a defect._

| ID | Declarative rule |
|----|------------------|
| **DR-1 Name** | A reasoning's name is the same as its label. |
| **DR-2 Count of Individuals** | A reasoning's count of individuals is the number of individuals related to the reasoning. |
| **DR-3 Count of People** | A reasoning's count of people is the number of the reasoning's individuals that are people. |
| **DR-4 Count of Schools** | A reasoning's count of schools is the number of the reasoning's individuals that are schools. |
| **DR-5 Count of Stated Parenthoods** | A reasoning's count of stated parenthoods is the number of parenthoods related to the reasoning. |
| **DR-6 Count of Stated Marriages** | A reasoning's count of stated marriages is the number of marriages related to the reasoning. |
| **DR-7 Count of Stated Friendships** | A reasoning's count of stated friendships is the number of friendships related to the reasoning. |
| **DR-8 Count of Stated Individual Facts** | A reasoning's count of stated individual facts is the total stated fact count across the individuals related to the reasoning. |
| **DR-9 Count of Current Grade Levels** | A reasoning's count of current grade levels is the number of the reasoning's grade level records that are current. |
| **DR-10 Count of Current Roles** | A reasoning's count of current roles is the number of the reasoning's role records that are current. |
| **DR-11 Count of Stated Facts** | A reasoning's count of stated facts is computed as the count of stated parenthoods plus the count of stated marriages plus the count of stated friendships plus the count of stated individual facts plus the count of current grade levels plus the count of current roles. |
| **DR-12 Count of Child of Conclusions** | A reasoning's count of child of conclusions is the number of the reasoning's parenthoods that are child of holds. |
| **DR-13 Count of Mirrored Marriages** | A reasoning's count of mirrored marriages is the number of the reasoning's marriages that are mirror holds. |
| **DR-14 Count of Mirrored Friendships** | A reasoning's count of mirrored friendships is the number of the reasoning's friendships that are mirror holds. |
| **DR-15 Count of Grandparent Conclusions** | A reasoning's count of grandparent conclusions is the number of the reasoning's grandparenthoods that hold. |
| **DR-16 Count of Person Conclusions** | A reasoning's count of person conclusions is the number of the reasoning's individuals that are person-concluded. |
| **DR-17 Count of School Conclusions** | A reasoning's count of school conclusions is the number of the reasoning's individuals that are school-concluded. |
| **DR-18 Count of Conclusions** | A reasoning's count of conclusions is computed as the count of child of conclusions plus the count of mirrored marriages plus the count of mirrored friendships plus the count of grandparent conclusions plus the count of person conclusions plus the count of school conclusions. |
| **DR-19 Count of Contradictions** | A reasoning's count of contradictions is the number of the reasoning's parenthoods that are contradictions. |
| **DR-20 Is Consistent** | A reasoning is considered consistent if the count of contradictions is 0. |
| **DR-21 Summary** | A reasoning's summary is computed as the count of stated facts, followed by “ stated facts, ”, followed by the count of conclusions, followed by “ conclusions, ”, followed by the count of contradictions, followed by “ contradictions”. |
| **DR-22 Name** | A portrait's name is the same as its display name. |
| **DR-23 Name** | An individual's name is the same as its label. |
| **DR-24 Portrait Image** | An individual's portrait image — taken from the linked portrait. |
| **DR-25 Symmetric is on** | An individual's symmetric is on when the linked reasoning is on. |
| **DR-26 Domain Range is on** | An individual's domain range is on when the linked reasoning is on. |
| **DR-27 Attends Label** | An individual's attends label — taken from the linked attends. |
| **DR-28 Has Age** | An individual is considered to have an age if the age has a value. |
| **DR-29 Has Attends** | An individual is considered to have an attends if the attends has a value. |
| **DR-30 Stated Fact Count** | An individual's stated fact count is computed as the count of the following that hold: the asserted person flag is set; the asserted school flag is set; the age flag is set; and the attends flag is set. |
| **DR-31 Count of Grade Levels on** | An individual's count of grade levels on is the number of the individual's grade level records that are on. |
| **DR-32 Latest Grade Level Seq** | An individual's latest grade level seq is the largest stated seq across the individual's grade level records that are on. |
| **DR-33 Has Grade Level** | An individual is considered to have a grade level if the count of grade levels on is greater than 0. |
| **DR-34 Current Grade Level Record** | The individual's current grade level record is determined by the following priority:<br>1. the individual ID, followed by “|grade|”, followed by the latest grade level seq, if the grade level flag is set;<br>2. in all other cases, an empty string. |
| **DR-35 Grade Level** | An individual's grade level — taken from the linked current grade level record. |
| **DR-36 Count of Roles on** | An individual's count of roles on is the number of the individual's role records that are on. |
| **DR-37 Latest Role Seq** | An individual's latest role seq is the largest stated seq across the individual's role records that are on. |
| **DR-38 Has Role** | An individual is considered to have a role if the count of roles on is greater than 0. |
| **DR-39 Current Role Record** | The individual's current role record is determined by the following priority:<br>1. the individual ID, followed by “|role|”, followed by the latest role seq, if the role flag is set;<br>2. in all other cases, an empty string. |
| **DR-40 Role** | An individual's role — taken from the linked current role record. |
| **DR-41 Has Person Evidence** | An individual is considered to have a person evidence if at least one of the following holds: the attends flag is set; the grade level flag is set; or the role flag is set. |
| **DR-42 Is Person by Domain** | An individual is considered a person by domain if all of the following hold: the domain range is on flag is set and the person evidence flag is set. |
| **DR-43 Is Person** | An individual is considered a person if the IF of the OR of the is asserted person and the is person by domain and true. |
| **DR-44 Is Person Concluded** | An individual is considered person-concluded if the IF of the AND of the is person by domain and the asserted person flag is not set and true. |
| **DR-45 Attended by Count** | An individual's attended by count is the number of individuals related to the individual. |
| **DR-46 Is School by Range** | An individual is considered a school by range if all of the following hold: the domain range is on flag is set and the attended by count is greater than 0. |
| **DR-47 Is School** | An individual is considered a school if the IF of the OR of the is asserted school and the is school by range and true. |
| **DR-48 Is School Concluded** | An individual is considered school-concluded if the IF of the AND of the is school by range and the asserted school flag is not set and true. |
| **DR-49 Child Count** | An individual's child count is the number of parenthoods related to the individual. |
| **DR-50 Parent Count** | An individual's parent count is the number of parenthoods related to the individual. |
| **DR-51 Child of Fact Count** | An individual's child of fact count is the number of the individual's parenthoods that are child of holds. |
| **DR-52 Grandchild Count** | An individual's grandchild count is the number of the individual's grandparenthoods that hold. |
| **DR-53 Grandparent Count** | An individual's grandparent count is the number of the individual's grandparenthoods that hold. |
| **DR-54 Descendant Count** | An individual's descendant count is the number of vw parenthoods closure related to the individual. |
| **DR-55 Ancestor Count** | An individual's ancestor count is the number of vw parenthoods closure related to the individual. |
| **DR-56 Stated Spouse Count** | An individual's stated spouse count is the number of marriages related to the individual. |
| **DR-57 Mirrored Spouse Count** | An individual's mirrored spouse count is the number of the individual's marriages that are mirror holds. |
| **DR-58 Spouse Count** | An individual's spouse count is computed as the stated spouse count plus the mirrored spouse count. |
| **DR-59 Stated Friend Count** | An individual's stated friend count is the number of friendships related to the individual. |
| **DR-60 Mirrored Friend Count** | An individual's mirrored friend count is the number of the individual's friendships that are mirror holds. |
| **DR-61 Friend Count** | An individual's friend count is computed as the stated friend count plus the mirrored friend count. |
| **DR-62 Name** | A parenthood's name is computed as the parent label, followed by “ is parent of ”, followed by the child label. |
| **DR-63 Parent Label** | A parenthood's parent label — taken from the linked parent. |
| **DR-64 Child Label** | A parenthood's child label — taken from the linked child. |
| **DR-65 Inverse is on** | A parenthood's inverse is on when the linked reasoning is on. |
| **DR-66 Symmetric is on** | A parenthood's symmetric is on when the linked reasoning is on. |
| **DR-67 Disjoint is on** | A parenthood's disjoint is on when the linked reasoning is on. |
| **DR-68 Child of Holds** | A parenthood is flagged child of holds if the IF of the inverse is on and true. |
| **DR-69 Child of Sentence** | A parenthood's child of sentence is computed as the child label, followed by “ is child of ”, followed by the parent label. |
| **DR-70 Spouse As Stated Count** | A parenthood's spouse as stated count is the number of marriages related to the parenthood. |
| **DR-71 Spouse As Mirrored Count** | A parenthood's spouse as mirrored count is the number of marriages related to the parenthood. |
| **DR-72 Is Contradiction** | A parenthood is considered a contradiction if the IF of the AND of the disjoint is on and the spouse as stated count plus the spouse as mirrored count is greater than 0 and true. |
| **DR-73 Name** | A marriage's name is computed as the partner a label, followed by “ is spouse of ”, followed by the partner b label. |
| **DR-74 Partner a Label** | A marriage's partner a label — taken from the linked partner a. |
| **DR-75 Partner B Label** | A marriage's partner b label — taken from the linked partner b. |
| **DR-76 Symmetric is on** | A marriage's symmetric is on when the linked reasoning is on. |
| **DR-77 Mirror Holds** | A marriage is flagged mirror holds if the IF of the symmetric is on and true. |
| **DR-78 Mirror Sentence** | A marriage's mirror sentence is computed as the partner b label, followed by “ is spouse of ”, followed by the partner a label. |
| **DR-79 Name** | A friendship's name is computed as the friend a label, followed by “ is friend of ”, followed by the friend b label. |
| **DR-80 Friend a Label** | A friendship's friend a label — taken from the linked friend a. |
| **DR-81 Friend B Label** | A friendship's friend b label — taken from the linked friend b. |
| **DR-82 Symmetric is on** | A friendship's symmetric is on when the linked reasoning is on. |
| **DR-83 Mirror Holds** | A friendship is flagged mirror holds if the IF of the symmetric is on and true. |
| **DR-84 Mirror Sentence** | A friendship's mirror sentence is computed as the friend b label, followed by “ is friend of ”, followed by the friend a label. |
| **DR-85 Name** | The grandparenthood's name is determined by the following priority:<br>1. the grandparent label, followed by “ is grandparent of ”, followed by the grandchild label, if the links flag is set;<br>2. in all other cases, “(”, followed by the upper link, followed by “ does not lead to ”, followed by the lower link, followed by “)”. |
| **DR-86 Grandparent** | A grandparenthood's grandparent — taken from the linked upper link. |
| **DR-87 Upper Child** | A grandparenthood's upper child — taken from the linked upper link. |
| **DR-88 Lower Parent** | A grandparenthood's lower parent — taken from the linked lower link. |
| **DR-89 Grandchild** | A grandparenthood's grandchild — taken from the linked lower link. |
| **DR-90 Grandparent Label** | A grandparenthood's grandparent label — taken from the linked upper link. |
| **DR-91 Grandchild Label** | A grandparenthood's grandchild label — taken from the linked lower link. |
| **DR-92 Links** | A grandparenthood is considered to link if the upper child is the lower parent. |
| **DR-93 Chain is on** | A grandparenthood's chain is on when the linked reasoning is on. |
| **DR-94 Holds** | A grandparenthood is considered to hold if the IF of the AND of the links and the chain is on and true. |
| **DR-95 Name** | A grade level record's name is computed as the individual label, followed by “ has grade level ”, followed by the grade level. |
| **DR-96 Individual Label** | A grade level record's individual label — taken from the linked individual. |
| **DR-97 Individual Latest Seq** | A grade level record's individual latest seq is the latest grade level seq of the grade level record's individual. |
| **DR-98 Is Current** | A grade level record is considered current if all of the following hold: the on flag is set and the stated seq is the individual latest seq. |
| **DR-99 Key is Canonical** | A grade level record is flagged key is canonical if the grade level record ID is the individual, followed by “|grade|”, followed by the stated seq. |
| **DR-100 Name** | A role record's name is computed as the individual label, followed by “ has role ”, followed by the role. |
| **DR-101 Individual Label** | A role record's individual label — taken from the linked individual. |
| **DR-102 Individual Latest Seq** | A role record's individual latest seq is the latest role seq of the role record's individual. |
| **DR-103 Is Current** | A role record is considered current if all of the following hold: the on flag is set and the stated seq is the individual latest seq. |
| **DR-104 Key is Canonical** | A role record is flagged key is canonical if the role record ID is the individual, followed by “|role|”, followed by the stated seq. |

## 5 Traceability to Schema

_The expression column is the rule's definition in Docs notation —
the same logic the rulebook stores, written for a business reader._

| Schema element | Kind | Expression |
|----------------|------|------------|
| **Reasoning.Name** | formula | `Label` |
| **Reasoning.CountOfIndividuals** | rollup | `Count(Individuals via Reasoning)` |
| **Reasoning.CountOfPeople** | rollup | `Count(Individuals via Reasoning)` |
| **Reasoning.CountOfSchools** | rollup | `Count(Individuals via Reasoning)` |
| **Reasoning.CountOfStatedParenthoods** | rollup | `Count(Parenthoods via Reasoning)` |
| **Reasoning.CountOfStatedMarriages** | rollup | `Count(Marriages via Reasoning)` |
| **Reasoning.CountOfStatedFriendships** | rollup | `Count(Friendships via Reasoning)` |
| **Reasoning.CountOfStatedIndividualFacts** | rollup | `Sum(Individuals.StatedFactCount via Reasoning)` |
| **Reasoning.CountOfCurrentGradeLevels** | rollup | `Count(GradeLevelRecords via Reasoning)` |
| **Reasoning.CountOfCurrentRoles** | rollup | `Count(RoleRecords via Reasoning)` |
| **Reasoning.CountOfStatedFacts** | formula | `CountOfStatedParenthoods + CountOfStatedMarriages + CountOfStatedFriendships + CountOfStatedIndividualFacts + CountOfCurrentGradeLevels + CountOfCurrentRoles` |
| **Reasoning.CountOfChildOfConclusions** | rollup | `Count(Parenthoods via Reasoning)` |
| **Reasoning.CountOfMirroredMarriages** | rollup | `Count(Marriages via Reasoning)` |
| **Reasoning.CountOfMirroredFriendships** | rollup | `Count(Friendships via Reasoning)` |
| **Reasoning.CountOfGrandparentConclusions** | rollup | `Count(Grandparenthoods via Reasoning)` |
| **Reasoning.CountOfPersonConclusions** | rollup | `Count(Individuals via Reasoning)` |
| **Reasoning.CountOfSchoolConclusions** | rollup | `Count(Individuals via Reasoning)` |
| **Reasoning.CountOfConclusions** | formula | `CountOfChildOfConclusions + CountOfMirroredMarriages + CountOfMirroredFriendships + CountOfGrandparentConclusions + CountOfPersonConclusions + CountOfSchoolConclusions` |
| **Reasoning.CountOfContradictions** | rollup | `Count(Parenthoods via Reasoning)` |
| **Reasoning.IsConsistent** | formula | `CountOfContradictions = 0` |
| **Reasoning.Summary** | formula | `Concat(CountOfStatedFacts, " stated facts, ", CountOfConclusions, " conclusions, ", CountOfContradictions, " contradictions")` |
| **Portraits.Name** | formula | `DisplayName` |
| **Individuals.Name** | formula | `Label` |
| **Individuals.PortraitImage** | lookup | `Lookup(Portraits.Image via Portrait)` |
| **Individuals.SymmetricIsOn** | lookup | `Lookup(Reasoning.SymmetricIsOn via Reasoning)` |
| **Individuals.DomainRangeIsOn** | lookup | `Lookup(Reasoning.DomainRangeIsOn via Reasoning)` |
| **Individuals.AttendsLabel** | lookup | `Lookup(Individuals.Label via Attends)` |
| **Individuals.HasAge** | formula | `Age <> ""` |
| **Individuals.HasAttends** | formula | `Attends <> ""` |
| **Individuals.StatedFactCount** | formula | `If(IsAssertedPerson, 1, 0) + If(IsAssertedSchool, 1, 0) + If(HasAge, 1, 0) + If(HasAttends, 1, 0)` |
| **Individuals.CountOfGradeLevelsOn** | rollup | `Count(GradeLevelRecords via Individual)` |
| **Individuals.LatestGradeLevelSeq** | rollup | `Max(GradeLevelRecords.StatedSeq via Individual)` |
| **Individuals.HasGradeLevel** | formula | `CountOfGradeLevelsOn > 0` |
| **Individuals.CurrentGradeLevelRecord** | formula | `If(HasGradeLevel, Concat(IndividualId, "\|grade\|", LatestGradeLevelSeq), "")` |
| **Individuals.GradeLevel** | lookup | `Lookup(GradeLevelRecords.GradeLevel via CurrentGradeLevelRecord)` |
| **Individuals.CountOfRolesOn** | rollup | `Count(RoleRecords via Individual)` |
| **Individuals.LatestRoleSeq** | rollup | `Max(RoleRecords.StatedSeq via Individual)` |
| **Individuals.HasRole** | formula | `CountOfRolesOn > 0` |
| **Individuals.CurrentRoleRecord** | formula | `If(HasRole, Concat(IndividualId, "\|role\|", LatestRoleSeq), "")` |
| **Individuals.Role** | lookup | `Lookup(RoleRecords.Role via CurrentRoleRecord)` |
| **Individuals.HasPersonEvidence** | formula | `Or(HasAttends, HasGradeLevel, HasRole)` |
| **Individuals.IsPersonByDomain** | formula | `And(DomainRangeIsOn, HasPersonEvidence)` |
| **Individuals.IsPerson** | formula | `If(Or(IsAssertedPerson, IsPersonByDomain), True())` |
| **Individuals.IsPersonConcluded** | formula | `If(And(IsPersonByDomain, Not(IsAssertedPerson)), True())` |
| **Individuals.AttendedByCount** | rollup | `Count(Individuals via Attends)` |
| **Individuals.IsSchoolByRange** | formula | `And(DomainRangeIsOn, AttendedByCount > 0)` |
| **Individuals.IsSchool** | formula | `If(Or(IsAssertedSchool, IsSchoolByRange), True())` |
| **Individuals.IsSchoolConcluded** | formula | `If(And(IsSchoolByRange, Not(IsAssertedSchool)), True())` |
| **Individuals.ChildCount** | rollup | `Count(Parenthoods via Parent)` |
| **Individuals.ParentCount** | rollup | `Count(Parenthoods via Child)` |
| **Individuals.ChildOfFactCount** | rollup | `Count(Parenthoods via Child)` |
| **Individuals.GrandchildCount** | rollup | `Count(Grandparenthoods via Grandparent)` |
| **Individuals.GrandparentCount** | rollup | `Count(Grandparenthoods via Grandchild)` |
| **Individuals.DescendantCount** | rollup | `Count(vw_parenthoods_closure via FromId)` |
| **Individuals.AncestorCount** | rollup | `Count(vw_parenthoods_closure via ToId)` |
| **Individuals.StatedSpouseCount** | rollup | `Count(Marriages via PartnerA)` |
| **Individuals.MirroredSpouseCount** | rollup | `Count(Marriages via PartnerB)` |
| **Individuals.SpouseCount** | formula | `StatedSpouseCount + MirroredSpouseCount` |
| **Individuals.StatedFriendCount** | rollup | `Count(Friendships via FriendA)` |
| **Individuals.MirroredFriendCount** | rollup | `Count(Friendships via FriendB)` |
| **Individuals.FriendCount** | formula | `StatedFriendCount + MirroredFriendCount` |
| **Parenthoods.Name** | formula | `Concat(ParentLabel, " is parent of ", ChildLabel)` |
| **Parenthoods.ParentLabel** | lookup | `Lookup(Individuals.Label via Parent)` |
| **Parenthoods.ChildLabel** | lookup | `Lookup(Individuals.Label via Child)` |
| **Parenthoods.InverseIsOn** | lookup | `Lookup(Reasoning.InverseIsOn via Reasoning)` |
| **Parenthoods.SymmetricIsOn** | lookup | `Lookup(Reasoning.SymmetricIsOn via Reasoning)` |
| **Parenthoods.DisjointIsOn** | lookup | `Lookup(Reasoning.DisjointIsOn via Reasoning)` |
| **Parenthoods.ChildOfHolds** | formula | `If(InverseIsOn, True())` |
| **Parenthoods.ChildOfSentence** | formula | `Concat(ChildLabel, " is child of ", ParentLabel)` |
| **Parenthoods.SpouseAsStatedCount** | rollup | `Count(Marriages via PartnerA)` |
| **Parenthoods.SpouseAsMirroredCount** | rollup | `Count(Marriages via PartnerA)` |
| **Parenthoods.IsContradiction** | formula | `If(And(DisjointIsOn, SpouseAsStatedCount + SpouseAsMirroredCount > 0), True())` |
| **Marriages.Name** | formula | `Concat(PartnerALabel, " is spouse of ", PartnerBLabel)` |
| **Marriages.PartnerALabel** | lookup | `Lookup(Individuals.Label via PartnerA)` |
| **Marriages.PartnerBLabel** | lookup | `Lookup(Individuals.Label via PartnerB)` |
| **Marriages.SymmetricIsOn** | lookup | `Lookup(Reasoning.SymmetricIsOn via Reasoning)` |
| **Marriages.MirrorHolds** | formula | `If(SymmetricIsOn, True())` |
| **Marriages.MirrorSentence** | formula | `Concat(PartnerBLabel, " is spouse of ", PartnerALabel)` |
| **Friendships.Name** | formula | `Concat(FriendALabel, " is friend of ", FriendBLabel)` |
| **Friendships.FriendALabel** | lookup | `Lookup(Individuals.Label via FriendA)` |
| **Friendships.FriendBLabel** | lookup | `Lookup(Individuals.Label via FriendB)` |
| **Friendships.SymmetricIsOn** | lookup | `Lookup(Reasoning.SymmetricIsOn via Reasoning)` |
| **Friendships.MirrorHolds** | formula | `If(SymmetricIsOn, True())` |
| **Friendships.MirrorSentence** | formula | `Concat(FriendBLabel, " is friend of ", FriendALabel)` |
| **Grandparenthoods.Name** | formula | `If(Links, Concat(GrandparentLabel, " is grandparent of ", GrandchildLabel), Concat("(", UpperLink, " does not lead to ", LowerLink, ")"))` |
| **Grandparenthoods.Grandparent** | lookup | `Lookup(Parenthoods.Parent via UpperLink)` |
| **Grandparenthoods.UpperChild** | lookup | `Lookup(Parenthoods.Child via UpperLink)` |
| **Grandparenthoods.LowerParent** | lookup | `Lookup(Parenthoods.Parent via LowerLink)` |
| **Grandparenthoods.Grandchild** | lookup | `Lookup(Parenthoods.Child via LowerLink)` |
| **Grandparenthoods.GrandparentLabel** | lookup | `Lookup(Parenthoods.ParentLabel via UpperLink)` |
| **Grandparenthoods.GrandchildLabel** | lookup | `Lookup(Parenthoods.ChildLabel via LowerLink)` |
| **Grandparenthoods.Links** | formula | `UpperChild = LowerParent` |
| **Grandparenthoods.ChainIsOn** | lookup | `Lookup(Reasoning.ChainIsOn via Reasoning)` |
| **Grandparenthoods.Holds** | formula | `If(And(Links, ChainIsOn), True())` |
| **GradeLevelRecords.Name** | formula | `Concat(IndividualLabel, " has grade level ", GradeLevel)` |
| **GradeLevelRecords.IndividualLabel** | lookup | `Lookup(Individuals.Label via Individual)` |
| **GradeLevelRecords.IndividualLatestSeq** | lookup | `Lookup(Individuals.LatestGradeLevelSeq via Individual)` |
| **GradeLevelRecords.IsCurrent** | formula | `And(IsOn, StatedSeq = IndividualLatestSeq)` |
| **GradeLevelRecords.KeyIsCanonical** | formula | `GradeLevelRecordId = Concat(Individual, "\|grade\|", StatedSeq)` |
| **RoleRecords.Name** | formula | `Concat(IndividualLabel, " has role ", Role)` |
| **RoleRecords.IndividualLabel** | lookup | `Lookup(Individuals.Label via Individual)` |
| **RoleRecords.IndividualLatestSeq** | lookup | `Lookup(Individuals.LatestRoleSeq via Individual)` |
| **RoleRecords.IsCurrent** | formula | `And(IsOn, StatedSeq = IndividualLatestSeq)` |
| **RoleRecords.KeyIsCanonical** | formula | `RoleRecordId = Concat(Individual, "\|role\|", StatedSeq)` |

---

> **RuleSpeak®** — the declarative business-rule notation created by **Ronald G. Ross**, following the conventions of **SBVR** — is coming soon. Until then this page shows the regular documentation. [www.RonRoss.info](https://www.RonRoss.info)
