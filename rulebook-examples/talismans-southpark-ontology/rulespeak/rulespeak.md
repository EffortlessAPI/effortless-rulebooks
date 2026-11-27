# 📘 Talisman's South Park ontology explainer — Docs

_Jessica Talisman's South Park Ontology Explainer, as a rulebook: the page's ten facts, five rule switches, property wiring, provenance wording and current-value policy are rows, and every conclusion is a formula. The rules apply in three fixed strata (inverse and symmetric, then the property chain, then domain and range); property wiring that feeds an earlier stratum would need repeated rounds. Every fact that could hold over the cast is a candidate row; app/lib/candidates.mjs writes the candidates when a thing, value or property is added._

> Declarative business rules rendered from the rulebook. Every statement
> below expresses truth in the business domain — it is neither a procedure
> nor an imperative. The rulebook's formulas are the single source of truth;
> this document is their plain-language reading.

## 1 Business Vocabulary

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Graph** | The explainer page as one graph: its wording variables and its totals. | — |
| Label | A defined attribute. | _Display name of the graph._ |
| Literal Quote | A defined attribute. | _The quote mark written around a literal value in a fact's wording._ |
| Literal Type Marker | A defined attribute. | _The marker written between a literal value and its datatype, as in Turtle._ |
| Built in Source Text | A defined attribute. | _Source line shown for a fact the page states as a teaching example._ |
| User Source Text | A defined attribute. | _Source line shown for a fact the reader stated._ |
| No Conclusions Text | A defined attribute. | _Message shown when the selected facts and rules add no conclusion._ |
| Name | The same as its label. | _Order 1. Display name of the graph._ |
| Count of Stated Facts | The total in effect unit across the statements related to the graph. | _Order 5. Facts that are switched on and in effect._ |
| Count of Conclusions | The total conclusion unit across the statements related to the graph. | _Order 18. Facts that hold only because an enabled rule concluded them._ |
| Count of Violations | The total violation unit across the disjoint checks related to the graph. | _Order 18. Disjointness checks that found both links between the same pair._ |
| Is Consistent | True when the count of violations is 0. | _Order 19. True when no enabled disjointness rule is violated._ |
| Conclusions Message | Determined by priority: the no conclusions text if the count of conclusions is 0; in all other cases, an empty string. | _Order 19. The empty-state message for the conclusions panel, blank when there are conclusions._ |
| Count of Facts to Add | The total listed off unit across the statements related to the graph. | _Order 5. Listed facts that are not in effect; the Add next fact button is enabled while this is above zero._ |
| Has Facts to Add | True when the count of facts to add is greater than 0. | _Order 6. True while some listed fact can still be added._ |
| Next Fact Sort Order | The smallest sort order across the graph's statements that are listed off unit. | _Order 5. List position of the first listed fact that is not in effect._ |
| Max Activation Seq | The largest activation seq across the statements related to the graph. | _Order 1. Highest activation number used so far._ |
| Next Activation Seq | Computed as the max activation seq plus 1. | _Order 2. The activation number the next switched-on fact takes._ |
| Max Listed Sort Order | The largest sort order across the graph's statements that are listed unit. | _Order 2. Highest list position among listed facts._ |
| Next Listed Sort Order | Computed as the max listed sort order plus 1. | _Order 3. The list position a newly added fact takes._ |
| Max Sort Order | The largest sort order across the statements related to the graph. | _Order 1. Highest sort order of any fact, listed or candidate._ |
| Next Candidate Sort Order | Computed as the max sort order plus 1. | _Order 2. The sort order a new candidate fact, or a fact removed from the list, takes._ |

<details open>
<summary>Example data</summary>

| Label | Literal Quote | Literal Type Marker | Built in Source Text | User Source Text | No Conclusions Text | Name ƒ | Count of Stated Facts ƒ | Count of Conclusions ƒ | Count of Violations ƒ | Is Consistent ƒ | Conclusions Message ƒ | Count of Facts to Add ƒ | Has Facts to Add ƒ | Next Fact Sort Order ƒ | Max Activation Seq ƒ | Next Activation Seq ƒ | Max Listed Sort Order ƒ | Next Listed Sort Order ƒ | Max Sort Order ƒ | Next Candidate Sort Order ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| South Park | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | These facts and selected rules add no further conclusions. Try another fact or rule. | South Park | 10 | 0 | 0 | true | These facts and selected rules add no further conclusions. Try another fact or rule. | 0 | false | 0 | 10 | 11 | 10 | 11 | 100269 | 100270 |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Rule** | The five rule switches on the page. | — |
| Label | A defined attribute. | _The rule's name on the page._ |
| Explanation | A defined attribute. | _The rule's one-line explanation on the page._ |
| Is Enabled | True when an empty string. | _The rule's checkbox._ |
| Sort Order | A defined attribute. | _Position in the rule list._ |
| Name | The same as its label. | _Order 1. The rule's name._ |
| Count of Firings | The total fired unit across the firings related to the rule. | _Order 15. Rule applications that fired, counting every derivation._ |

<details open>
<summary>Example data</summary>

| Label | Explanation | Is Enabled | Sort Order | Name ƒ | Count of Firings ƒ |
|---|---|---|---|---|---|
| Inverse | A parent link also gives a child link between the same two people. | false | 1 | Inverse | 0 |
| Symmetric | A friend link or a spouse link is true for both people. | false | 2 | Symmetric | 0 |
| Property chain | A parent of a parent is a grandparent. | false | 3 | Property chain | 0 |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Firing Kind** | The ways a rule can apply. Domain and range belong to one rule but are worded apart; 'disjoint' names the rule the disjointness checks answer to. | — |
| Rule | A defined attribute. | _The rule switch this kind belongs to._ |
| Wording | A defined attribute. | _How a conclusion card names the rule, as in 'Worked out by the inverse rule'._ |
| Sort Order | A defined attribute. | _Order in which the page's reasoner tries the kinds for one fact._ |
| Name | The same as its wording. | _Order 1. The kind's wording._ |
| Rule Enabled | True when the linked rule is enabled. | _Order 1. Whether the owning rule is switched on._ |

<details open>
<summary>Example data</summary>

| Wording | Sort Order | Name ƒ | Rule Enabled ƒ |
|---|---|---|---|
| inverse rule | 1 | inverse rule | false |
| symmetric rule | 2 | symmetric rule | false |
| property chain rule | 3 | property chain rule | false |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Thing** | Every individual and class the page can draw. | — |
| Label | A defined attribute. | _The name the page shows._ |
| Kind | A defined attribute. | _'individual' or 'class'._ |
| Sort Order | A defined attribute. | _Position in the cast._ |
| Portrait | A defined attribute. | _The portrait drawn for this thing; set when its name matches a portrait alias._ |
| Portrait Name | The display name of the thing's portrait. | _Order 1. The name on the portrait's Fandom page._ |
| Portrait Page | Taken from the linked portrait. | _Order 1. The Fandom page credited for the portrait._ |
| Portrait Image | Taken from the linked portrait. | _Order 1. The original Fandom image._ |
| Portrait Crop | Taken from the linked portrait. | _Order 1. The portrait's crop as an SVG viewBox._ |
| Portrait Width | Taken from the linked portrait. | _Order 1. The original image's width in pixels._ |
| Portrait Height | Taken from the linked portrait. | _Order 1. The original image's height in pixels._ |
| Name | The same as its label. | _Order 1. The thing's name._ |
| Node Key | The same as its thing ID. | _Order 1. The key the graph draws this thing under._ |
| Count of Children | The total parent unit across the statements related to the thing. | _Order 17. People this thing is a parent of, stated or concluded._ |
| Count of Grandchildren | The total grandparent unit across the statements related to the thing. | _Order 17. People this thing is a grandparent of._ |
| Count As Place | The total place unit across the statements related to the thing. | _Order 17. Holding attends facts whose object is this thing._ |
| Count As Subject | The total holds unit across the statements related to the thing. | _Order 17. Holding facts about this thing._ |
| Count As Object | The total holds unit across the statements related to the thing. | _Order 17. Holding facts that point at this thing._ |
| Is in Graph | True when at least one of the following holds: the count as subject is greater than 0 or the count as object is greater than 0. | _Order 18. True when the graph draws this thing._ |
| Node Kind | Determined by priority: an empty string if the kind is blank; “class” if the kind is “class”; “place” if the count as place is greater than 0; in all other cases, “person”. | _Order 18. How the graph draws the thing: a class, a place someone attends, or a person._ |

<details open>
<summary>Example data</summary>

| Label | Kind | Sort Order | Portrait Name ƒ | Portrait Page ƒ | Portrait Image ƒ | Portrait Crop ƒ | Portrait Width ƒ | Portrait Height ƒ | Name ƒ | Node Key ƒ | Count of Children ƒ | Count of Grandchildren ƒ | Count As Place ƒ | Count As Subject ƒ | Count As Object ƒ | Is in Graph ƒ | Node Kind ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| Marvin | individual | 1 | Marvin Marsh | https://southpark.fandom.com/wiki/Marvin_Marsh | https://static.wikia.nocookie.net/southpark/images/d/db/Marsh-family-grandpa-marsh-w-purple-blanket.png/revision/latest | 48 -5 280 280 | 375 | 536 | Marvin | Marvin | 1 | 0 | 0 | 1 | 0 | true | person |
| Randy | individual | 2 | Randy Marsh | https://southpark.fandom.com/wiki/Randy_Marsh | https://static.wikia.nocookie.net/southparkfanon/images/d/d6/Randy_Marsh.png/revision/latest | 0 -10 428 428 | 428 | 1057 | Randy | Randy | 2 | 0 | 0 | 3 | 1 | true | person |
| Sharon | individual | 3 | Sharon Marsh | https://southpark.fandom.com/wiki/Sharon_Marsh | https://static.wikia.nocookie.net/southpark/images/9/99/Sharon_Marsh.png/revision/latest | 20 -10 390 390 | 439 | 1049 | Sharon | Sharon | 0 | 0 | 0 | 0 | 1 | true | person |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Portrait** | The page's Fandom portraits, including those for characters a reader may add. | — |
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
| **Portrait Aliase** | The names a portrait answers to. A new thing whose name matches an alias gets that portrait. | — |
| Portrait | A defined attribute. | _The portrait this name selects._ |
| Name | The same as its portrait alias ID. | _Order 1. The alias._ |

<details open>
<summary>Example data</summary>

| Name ƒ |
|---|
| marvin |
| marvin marsh |
| grandpa marsh |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Property** | The ten properties, with what the symmetric, domain and range rules read. Axioms that relate one property to another (inverse, chain, disjoint) are rows of PropertyAxioms. | — |
| Label | A defined attribute. | _The property's wording on the page._ |
| Property Kind | A defined attribute. | _'object' links two things, 'datatype' links a thing to a value, 'type' states class membership._ |
| Datatype | A defined attribute. | _The value's XML Schema datatype for a datatype property; blank otherwise._ |
| Is User Addable | True when an empty string. | _Offered in the page's Add fact form._ |
| Has One Current Value | True when an empty string. | _The page keeps one current value per person: switching a value on replaces the earlier one._ |
| Is Symmetric | True when an empty string. | _The symmetric rule makes this link true in both directions._ |
| Domain Class | A defined attribute. | _The class the domain rule gives this property's subject._ |
| Range Class | A defined attribute. | _The class the range rule gives this property's object._ |
| Sort Order | A defined attribute. | _Position in the Add fact form's property list._ |
| Value Node Label | A defined attribute. | _The small label under a value node for this property: 'role value' or 'number value'._ |
| Current Value Wording | A defined attribute. | _How the Add fact form names a replaced value, as in 'Stan now has grade level 5'._ |
| Input Name | A defined attribute. | _How the integer check names this property's value, as in 'for grade level'._ |
| Record Kind | A defined attribute. | _The word the current-record summary uses for this property's values: role, grade or school._ |
| Record Order | A defined attribute. | _Position of this property in the current-record summary._ |
| Name | The same as its label. | _Order 1. The property's wording._ |

<details open>
<summary>Example data</summary>

| Label | Property Kind | Datatype | Is User Addable | Has One Current Value | Is Symmetric | Sort Order | Value Node Label | Current Value Wording | Input Name | Record Kind | Record Order | Name ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|
| is parent of | object | — | true | false | false | 1 | — | — | — | — | — | is parent of |
| is child of | object | — | true | false | false | 2 | — | — | — | — | — | is child of |
| is spouse of | object | — | true | false | true | 3 | — | — | — | — | — | is spouse of |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Property Axiom** | Axioms that relate one property to others, as OWL states them: an inverse, a two-link chain, or a disjoint pair. The candidate generator reads these rows. | — |
| Kind | A defined attribute. | _inverse, chain or disjoint._ |
| Property | A defined attribute. | _The property the axiom is about: the one inverted, the one a chain derives, or the first of a disjoint pair._ |
| First | A defined attribute. | _The inverse property, the chain's first link, or the other property of a disjoint pair._ |
| Second | A defined attribute. | _The chain's second link._ |
| Name | The same as its property axiom ID. | _Order 1. The axiom's key._ |

<details open>
<summary>Example data</summary>

| Name ƒ |
|---|
| inverse:isParentOf |
| disjoint:isParentOf~isSpouseOf |
| inverse:isChildOf |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Statement** | Every fact the page can state or conclude about the cast. A listed fact appears in the page's fact list; the rest are candidates that only a rule can make hold. | — |
| Graph | A defined attribute. | _The graph the fact belongs to._ |
| Subject | A defined attribute. | _What the fact describes._ |
| Property | A defined attribute. | _How the subject connects to the object._ |
| Object Thing | A defined attribute. | _The related thing, for an object or type property._ |
| Object Value | A defined attribute. | _The literal value, for a datatype property._ |
| Is Listed | True when an empty string. | _Shown in the page's fact list._ |
| Is Built in | True when an empty string. | _One of the page's teaching facts; the reader cannot remove it._ |
| Is Detail | True when an empty string. | _One of Stan's detail facts that Show Stan's details switches on._ |
| Is Stated | True when an empty string. | _The fact's checkbox._ |
| Activation Seq | A defined attribute. | _When the fact was last switched on, as a running number; decides which value is current._ |
| Sort Order | A defined attribute. | _Position in the fact list; candidates follow the listed facts._ |
| Subject Label | Taken from the linked subject. | _Order 1. The subject's name._ |
| Property Label | Taken from the linked property. | _Order 1. The property's wording._ |
| Object Label | Taken from the linked object thing. | _Order 1. The object's name, for an object or type property._ |
| Property Datatype | Taken from the linked property. | _Order 1. The value's datatype, for a datatype property._ |
| Property Has One Current Value | True when the linked property has a one current value. | _Order 1. Whether the property keeps one current value per person._ |
| Property Domain Class | Taken from the linked property. | _Order 1. The property's domain class._ |
| Property Range Class | Taken from the linked property. | _Order 1. The property's range class._ |
| Literal Quote | Taken from the linked graph. | _Order 1. The graph's literal quote mark._ |
| Literal Type Marker | Taken from the linked graph. | _Order 1. The graph's datatype marker._ |
| Built in Source Text | Taken from the linked graph. | _Order 1. The graph's source line for a teaching fact._ |
| User Source Text | Taken from the linked graph. | _Order 1. The graph's source line for a reader's fact._ |
| Next Fact Sort Order | Taken from the linked graph. | _Order 6. List position of the next fact to add._ |
| Is Literal | True when the object thing is blank. | _Order 1. True when the object is a value rather than a thing._ |
| Object Text | Determined by priority: the object value if the object thing is blank; in all other cases, the object label. | _Order 2. The object's name or the value._ |
| Name | Computed as the subject label, followed by a space, followed by the property label, followed by a space, followed by the object text. | _Order 3. The fact in words._ |
| Words | Computed as the subject label, followed by a space, followed by the property label, followed by a space, followed by the literal quote, followed by the object value, followed by the literal quote, followed by the literal type marker, followed by the property datatype if the object thing is blank, in all other cases the object label. ⚠︎ mechanical <!-- rulespeak:reword --> | _Order 2. The fact as a conclusion card quotes it, with a literal written in Turtle form._ |
| Source | Determined by priority: the built in source text if the built in flag is set; in all other cases, the user source text. | _Order 2. Where a listed fact came from._ |
| Subject Property Key | Computed as the subject, followed by “|”, followed by the property. | _Order 1. Groups the values one subject has for one property._ |
| Latest Sibling Activation Seq | The largest activation seq across the statements related to the statement. | _Order 2. The most recent activation among this subject's values for this property._ |
| Is in Effect | True when all of the following hold: the stated flag is set and at least one of the following holds: the property has one current value flag is not set or the activation seq is the latest sibling activation seq. | _Order 3. Switched on and, for a one-value property, the most recently switched-on value. Drawn as a solid line._ |
| Listed Unit | Determined by priority: 1 if the listed flag is set; in all other cases, 0. | _Order 1. 1 when the fact is in the fact list._ |
| In Effect Unit | Determined by priority: 1 if the in effect flag is set; in all other cases, 0. | _Order 4. 1 when the fact is in effect._ |
| Listed Off Unit | Determined by priority: 1 if all of the following hold: the listed flag is set and the in effect flag is not set; in all other cases, 0. | _Order 4. 1 when the fact is listed but not in effect._ |
| Is Next to Add | True when all of the following hold: the listed off unit is 1 and the sort order is the next fact sort order. | _Order 7. The fact the Add next fact button switches on._ |
| Count of Inverse Firings | The total inverse fired unit across the firings related to the statement. | _Order 6. Inverse rule applications that conclude this fact._ |
| Count of Symmetric Firings | The total symmetric fired unit across the firings related to the statement. | _Order 6. Symmetric rule applications that conclude this fact._ |
| Holds Before Chain | True when the IF of the OR of the is in effect; the count of inverse firings is greater than 0; and the count of symmetric firings is greater than 0 and true. | _Order 7. TRUE when the fact is in effect or the inverse or symmetric rule concludes it. Blank when nothing states or concludes it: not known, never FALSE (open world: removing a statement does not say that its opposite is true)._ |
| Count of Chain Firings | The total chain fired unit across the firings related to the statement. | _Order 10. Property chain applications that conclude this fact._ |
| Holds Before Typing | True when the IF of the OR of the holds before chain and the count of chain firings is greater than 0 and true. | _Order 11. TRUE when the fact holds before the domain and range rules apply. Blank when nothing states or concludes it: not known, never FALSE (open world: removing a statement does not say that its opposite is true)._ |
| Count of Type Firings | The total type fired unit across the firings related to the statement. | _Order 14. Domain and range applications that conclude this fact._ |
| Holds | True when the IF of the OR of the holds before typing and the count of type firings is greater than 0 and true. | _Order 15. TRUE when in effect or concluded by an enabled rule; the graph draws it. Blank when nothing states or concludes it: not known, never FALSE (open world: removing a statement does not say that its opposite is true)._ |
| Is Conclusion | True when the IF of the AND of the holds and the in effect flag is not set and true. | _Order 16. TRUE when the fact holds only because a rule concluded it; drawn dashed. Blank otherwise: a stated fact is not a conclusion, and a fact nothing concludes is not known (never FALSE)._ |
| Holds Unit | Determined by priority: 1 if the holds flag is set; in all other cases, 0. | _Order 16. 1 when the fact holds._ |
| Conclusion Unit | Determined by priority: 1 if the conclusion flag is set; in all other cases, 0. | _Order 17. 1 when the fact is a conclusion._ |
| Parent Unit | Determined by priority: 1 if all of the following hold: the holds flag is set and the property is “isParentOf”; in all other cases, 0. | _Order 16. 1 for a holding parent link._ |
| Grandparent Unit | Determined by priority: 1 if all of the following hold: the holds flag is set and the property is “isGrandparentOf”; in all other cases, 0. | _Order 16. 1 for a holding grandparent link._ |
| Place Unit | Determined by priority: 1 if all of the following hold: the holds flag is set and the property is “attends”; in all other cases, 0. | _Order 16. 1 for a holding attends link._ |
| First Pre Chain Order Key | The smallest pre chain order key across the statement's firings that are pre chain fired unit. | _Order 7. Order key of the first inverse or symmetric application that concludes this fact._ |
| Snap Position | Determined by priority: the sort order if the in effect flag is set; 100000 plus the first pre chain order key if the holds before chain flag is set; in all other cases, 0. | _Order 8. Where the page's reasoner meets this fact as a premise: a stated fact at its list position, a fact concluded by the inverse or symmetric rule after every stated fact, in the order it was concluded; 0 for a fact that does not hold yet, so a link to no fact reads the same on every engine._ |
| First Fired Order Key | The smallest order key across the statement's firings that are fired unit. | _Order 15. Order key of the first rule application that concludes this fact, in the page reasoner's order._ |
| Count of Chosen Firings | The total chosen unit across the firings related to the statement. | _Order 20. Rule applications shown as this fact's conclusion card: one for a conclusion, zero otherwise._ |
| Subject Node Key | The same as its subject. | _Order 1. The graph node this fact starts from._ |
| Object Node Key | Determined by priority: “literal|”, followed by the object value, followed by “|”, followed by the property datatype if the object thing is blank; in all other cases, the object thing. | _Order 2. The graph node this fact points at: the thing, or one node per value and datatype._ |
| Subject Node Kind | Taken from the linked subject. | _Order 19. How the graph draws the subject._ |
| Object Thing Node Kind | Taken from the linked object thing. | _Order 19. How the graph draws the object thing._ |
| Object Node Kind | Determined by priority: “value” if the object thing is blank; in all other cases, the object thing node kind. | _Order 20. How the graph draws the object: a value, a class, a place or a person._ |
| Object Value Node Label | Taken from the linked property. | _Order 1. The small label under a value node._ |
| Count of Violations As First | The total violation unit across the disjoint checks related to the statement. | _Order 18. Violated disjointness checks naming this fact first._ |
| Count of Violations As Second | The total violation unit across the disjoint checks related to the statement. | _Order 18. Violated disjointness checks naming this fact second._ |
| Is in Contradiction | True when at least one of the following holds: the count of violations as first is greater than 0 or the count of violations as second is greater than 0. | _Order 19. Part of a contradiction the disjoint rule found; drawn in the alarm colour._ |

<details open>
<summary>Example data</summary>

| Object Value | Is Listed | Is Built in | Is Detail | Is Stated | Activation Seq | Sort Order | Subject Label ƒ | Property Label ƒ | Object Label ƒ | Property Datatype ƒ | Property Has One Current Value ƒ | Property Domain Class ƒ | Property Range Class ƒ | Literal Quote ƒ | Literal Type Marker ƒ | Built in Source Text ƒ | User Source Text ƒ | Next Fact Sort Order ƒ | Is Literal ƒ | Object Text ƒ | Name ƒ | Words ƒ | Source ƒ | Subject Property Key ƒ | Latest Sibling Activation Seq ƒ | Is in Effect ƒ | Listed Unit ƒ | In Effect Unit ƒ | Listed Off Unit ƒ | Is Next to Add ƒ | Count of Inverse Firings ƒ | Count of Symmetric Firings ƒ | Holds Before Chain ƒ | Count of Chain Firings ƒ | Holds Before Typing ƒ | Count of Type Firings ƒ | Holds ƒ | Is Conclusion ƒ | Holds Unit ƒ | Conclusion Unit ƒ | Parent Unit ƒ | Grandparent Unit ƒ | Place Unit ƒ | First Pre Chain Order Key ƒ | Snap Position ƒ | First Fired Order Key ƒ | Count of Chosen Firings ƒ | Subject Node Key ƒ | Object Node Key ƒ | Subject Node Kind ƒ | Object Thing Node Kind ƒ | Object Node Kind ƒ | Object Value Node Label ƒ | Count of Violations As First ƒ | Count of Violations As Second ƒ | Is in Contradiction ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| — | true | true | false | true | 1 | 1 | Marvin | is parent of | Randy | — | false | — | — | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | 0 | false | Randy | Marvin is parent of Randy | Marvin is parent of Randy | Stated as a teaching example on this page. | Marvin\|isParentOf | 1 | true | 1 | 1 | 0 | false | 0 | 0 | true | 0 | true | 0 | true | — | 1 | 0 | 1 | 0 | 0 | 0.0 | 1.0 | 0.0 | 0 | Marvin | Randy | person | person | person | — | 0 | 0 | false |
| — | false | false | false | false | 0 | 100001 | Marvin | is parent of | Sharon | — | false | — | — | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | 0 | false | Sharon | Marvin is parent of Sharon | Marvin is parent of Sharon | Stated by you on this page. | Marvin\|isParentOf | 1 | false | 0 | 0 | 0 | false | 0 | 0 | — | 0 | — | 0 | — | — | 0 | 0 | 0 | 0 | 0 | 0.0 | 0.0 | 0.0 | 0 | Marvin | Sharon | person | person | person | — | 0 | 0 | false |
| — | false | false | false | false | 0 | 100002 | Marvin | is parent of | Stan | — | false | — | — | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | 0 | false | Stan | Marvin is parent of Stan | Marvin is parent of Stan | Stated by you on this page. | Marvin\|isParentOf | 1 | false | 0 | 0 | 0 | false | 0 | 0 | — | 0 | — | 0 | — | — | 0 | 0 | 0 | 0 | 0 | 0.0 | 0.0 | 0.0 | 0 | Marvin | Stan | person | person | person | — | 0 | 0 | false |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Firing** | Every way an enabled rule could conclude a candidate fact from the facts that hold. | — |
| Kind | A defined attribute. | _How the rule applies._ |
| Rule | A defined attribute. | _The rule switch._ |
| Target | A defined attribute. | _The fact this application concludes._ |
| First | A defined attribute. | _The premise._ |
| Second | A defined attribute. | _The second premise, for the property chain._ |
| Rule Enabled | True when the linked rule is enabled. | _Order 1. Whether the rule is switched on._ |
| Kind Wording | Taken from the linked kind. | _Order 1. How the card names the rule._ |
| Kind Sort Order | Taken from the linked kind. | _Order 1. The kind's place in the reasoner's order._ |
| Target Name | Taken from the linked target. | _Order 4. The concluded fact in words._ |
| Target Subject Label | Taken from the linked target. | _Order 2. The concluded fact's subject._ |
| Target Property Label | Taken from the linked target. | _Order 2. The concluded fact's property._ |
| Target Object Text | Taken from the linked target. | _Order 3. The concluded fact's object._ |
| Target is Conclusion | True when the linked target is a conclusion. | _Order 17. Whether the concluded fact is new._ |
| Target Subject Node Key | Taken from the linked target. | _Order 2. The concluded fact's subject node._ |
| Target Object Node Key | Taken from the linked target. | _Order 3. The concluded fact's object node._ |
| Target First Fired Order Key | Taken from the linked target. | _Order 16. The order key of the concluded fact's first application._ |
| First Words | Taken from the linked first. | _Order 3. The premise as the card quotes it._ |
| Second Words | Taken from the linked second. | _Order 3. The second premise as the card quotes it._ |
| First is in Effect | True when the linked first is in effect. | _Order 4. Whether the premise is in effect._ |
| First Holds Before Chain | True when the linked first is holds before chain. | _Order 8. Whether the premise holds before the chain._ |
| Second Holds Before Chain | True when the linked second is holds before chain. | _Order 8. Whether the second premise holds before the chain._ |
| First Holds Before Typing | True when the linked first is holds before typing. | _Order 12. Whether the premise holds before typing._ |
| First Property | Taken from the linked first. | _Order 1. The premise's property key, as the card's axiom names it._ |
| First Property Domain Class | Taken from the linked first. | _Order 2. The premise property's domain class._ |
| First Property Range Class | Taken from the linked first. | _Order 2. The premise property's range class._ |
| First Sort Order | Taken from the linked first. | _Order 1. The premise's list position._ |
| Second is in Effect | True when the linked second is in effect. | _Order 4. Whether the second premise is in effect._ |
| First Snap Position | Taken from the linked first. | _Order 9. Where the reasoner meets the premise._ |
| Second Snap Position | Taken from the linked second. | _Order 9. Where the reasoner meets the second premise._ |
| Inverse Fired Unit | Determined by priority: 1 if all of the following hold: the rule enabled flag is set; the kind is “inverse”; and the first is in effect flag is set; in all other cases, 0. | _Order 5. 1 when the inverse rule fires here._ |
| Symmetric Fired Unit | Determined by priority: 1 if all of the following hold: the rule enabled flag is set; the kind is “symmetric”; and the first is in effect flag is set; in all other cases, 0. | _Order 5. 1 when the symmetric rule fires here._ |
| Chain Fired Unit | Determined by priority: 1 if all of the following hold: the rule enabled flag is set; the kind is “chain”; the first holds before chain flag is set; and the second holds before chain flag is set; in all other cases, 0. | _Order 9. 1 when the property chain fires here._ |
| Type Fired Unit | Determined by priority: 1 if all of the following hold: the rule enabled flag is set; at least one of the following holds: the kind is “domain” or the kind is “range”; and the first holds before typing flag is set; in all other cases, 0. | _Order 13. 1 when the domain or range rule fires here._ |
| Fired Unit | Computed as the inverse fired unit plus the symmetric fired unit plus the chain fired unit plus the type fired unit. | _Order 14. 1 when this application fires._ |
| Pre Chain Fired Unit | Computed as the inverse fired unit plus the symmetric fired unit. | _Order 6. 1 when this is an inverse or symmetric application that fires._ |
| Pre Chain Order Key | Computed as the first sort order times 10 plus the kind sort order. | _Order 2. For an inverse or symmetric application: its place in the reasoner's first round._ |
| Round | Determined by priority: 2 if all of the following hold: the kind is “chain” and it is not the case that all of the following hold: the first is in effect flag is set and the second is in effect flag is set; in all other cases, 1. | _Order 5. The reasoner's round: 1 when every premise is a stated fact, 2 when the property chain reads a concluded fact._ |
| Order Key | Computed as the round times 1000000000000000 plus the first snap position times 100000000 plus the kind sort order times 10000000 plus 0 if the second is blank, in all other cases the second snap position. | _Order 10. The page reasoner's order: by round, then premise, then rule kind, then second premise._ |
| Is Chosen | True when all of the following hold: the fired unit is 1; the target is conclusion flag is set; and the order key is the target first fired order key. | _Order 18. The application the conclusions panel shows for its fact: the first one in the reasoner's order._ |
| Chosen Unit | Determined by priority: 1 if the chosen flag is set; in all other cases, 0. | _Order 19. 1 when this application is shown._ |
| Card Text | Determined by priority: “Worked out by the ”, followed by the kind wording, followed by “ from “”, followed by the first words, followed by “””, followed by “ and “”, followed by the second words, followed by “”” if the kind is “chain”, in all other cases “ and “”, followed by the first property, followed by “ has domain ”, followed by the first property domain class, followed by “”” if the kind is “domain”, in all other cases “ and “”, followed by the first property, followed by “ has range ”, followed by the first property range class, followed by “””, followed by a period if the chosen flag is set; in all other cases, an empty string. ⚠︎ mechanical <!-- rulespeak:reword --> | _Order 19. The conclusion card's provenance line, in the page's wording._ |
| Name | The same as its firing ID. | _Order 1. The application's key._ |

<details open>
<summary>Example data</summary>

| Rule Enabled ƒ | Kind Wording ƒ | Kind Sort Order ƒ | Target Name ƒ | Target Subject Label ƒ | Target Property Label ƒ | Target Object Text ƒ | Target is Conclusion ƒ | Target Subject Node Key ƒ | Target Object Node Key ƒ | Target First Fired Order Key ƒ | First Words ƒ | Second Words ƒ | First is in Effect ƒ | First Holds Before Chain ƒ | Second Holds Before Chain ƒ | First Holds Before Typing ƒ | First Property ƒ | First Property Domain Class ƒ | First Property Range Class ƒ | First Sort Order ƒ | Second is in Effect ƒ | First Snap Position ƒ | Second Snap Position ƒ | Inverse Fired Unit ƒ | Symmetric Fired Unit ƒ | Chain Fired Unit ƒ | Type Fired Unit ƒ | Fired Unit ƒ | Pre Chain Fired Unit ƒ | Pre Chain Order Key ƒ | Round ƒ | Order Key ƒ | Is Chosen ƒ | Chosen Unit ƒ | Card Text ƒ | Name ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| false | inverse rule | 1 | Randy is child of Marvin | Randy | is child of | Marvin | — | Randy | Marvin | 0.0 | Marvin is parent of Randy |    | true | true | — | true | isParentOf | — | — | 1 | false | 1.0 | 0.0 | 0 | 0 | 0 | 0 | 0 | 0 | 11.0 | 1 | 1000000110000000.0 | false | 0 | — | inverse:Marvin\|isParentOf\|Randy>Randy\|isChildOf\|Marvin |
| false | inverse rule | 1 | Sharon is child of Marvin | Sharon | is child of | Marvin | — | Sharon | Marvin | 0.0 | Marvin is parent of Sharon |    | false | — | — | — | isParentOf | — | — | 100001 | false | 0.0 | 0.0 | 0 | 0 | 0 | 0 | 0 | 0 | 1000011.0 | 1 | 1000000010000000.0 | false | 0 | — | inverse:Marvin\|isParentOf\|Sharon>Sharon\|isChildOf\|Marvin |
| false | inverse rule | 1 | Stan is child of Marvin | Stan | is child of | Marvin | — | Stan | Marvin | 0.0 | Marvin is parent of Stan |    | false | — | — | — | isParentOf | — | — | 100002 | false | 0.0 | 0.0 | 0 | 0 | 0 | 0 | 0 | 0 | 1000021.0 | 1 | 1000000010000000.0 | false | 0 | — | inverse:Marvin\|isParentOf\|Stan>Stan\|isChildOf\|Marvin |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Disjoint Check** | Every pair the disjointness rule watches: the same two people linked by two disjoint properties. | — |
| Graph | A defined attribute. | _The graph._ |
| Rule | A defined attribute. | _The rule switch._ |
| First | A defined attribute. | _The first link._ |
| Second | A defined attribute. | _The disjoint link between the same pair, in the same direction._ |
| Rule Enabled | True when the linked rule is enabled. | _Order 1. Whether the disjointness rule is on._ |
| First Holds | True when the linked first is holds. | _Order 16. Whether the first link holds._ |
| Second Holds | True when the linked second is holds. | _Order 16. Whether the second link holds._ |
| Subject Label | Taken from the linked first. | _Order 2. The person recorded with both links._ |
| Object Label | Taken from the linked first. | _Order 2. The other person._ |
| Violation Unit | Determined by priority: 1 if all of the following hold: the rule enabled flag is set; the first holds flag is set; and the second holds flag is set; in all other cases, 0. | _Order 17. 1 when both links hold._ |
| Alarm Text | Determined by priority: “Contradiction found. ”, followed by the subject label, followed by “ is recorded as both parent and spouse of ”, followed by the object label, followed by “. Check those facts and the selected disjoint rule.” if the violation unit is 1; in all other cases, an empty string. | _Order 18. The page's contradiction alert, blank when there is none._ |
| Name | The same as its disjoint check ID. | _Order 1. The check's key._ |

<details open>
<summary>Example data</summary>

| Rule Enabled ƒ | First Holds ƒ | Second Holds ƒ | Subject Label ƒ | Object Label ƒ | Violation Unit ƒ | Alarm Text ƒ | Name ƒ |
|---|---|---|---|---|---|---|---|
| false | true | — | Marvin | Randy | 0 | — | Marvin\|isParentOf\|Randy~isSpouseOf |
| false | — | — | Marvin | Sharon | 0 | — | Marvin\|isParentOf\|Sharon~isSpouseOf |
| false | — | — | Marvin | Stan | 0 | — | Marvin\|isParentOf\|Stan~isSpouseOf |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Scenario** | The page's buttons. Each one is a fact baseline, explicit fact changes and rule settings, applied in that order. | — |
| Graph | A defined attribute. | _The graph._ |
| Label | A defined attribute. | _The button's text._ |
| Note | A defined attribute. | _The note shown under the buttons after this one is pressed; blank clears it._ |
| Sort Order | A defined attribute. | _Position among the buttons._ |
| Button Group | A defined attribute. | _'stan' for the two imagined changes, 'facts' for the buttons under the fact list._ |
| Is Primary | True when an empty string. | _Drawn as the filled button._ |
| Fact Baseline | A defined attribute. | _What happens to every listed fact first: keep, built-in (teaching facts on, others off), built-in-detail, all-off, activate-all (each switched on in list order), or next (the next fact on)._ |
| Layout | A defined attribute. | _The node layout the graph returns to; blank keeps the current positions._ |
| Clears Selection | True when an empty string. | _Clears the selected node or line._ |
| Clears Form Message | True when an empty string. | _Clears the Add fact form's message._ |
| Name | The same as its label. | _Order 1. The button's text._ |
| Graph Has Facts to Add | True when the linked graph has a facts to add. | _Order 7. Whether a listed fact can still be added._ |
| Is Available | True when at least one of the following holds: the fact baseline is not “next” or the graph has facts to add flag is set. | _Order 8. False only for Add next fact once every listed fact is in effect._ |

<details open>
<summary>Example data</summary>

| Label | Note | Sort Order | Button Group | Is Primary | Fact Baseline | Layout | Clears Selection | Clears Form Message | Name ƒ | Graph Has Facts to Add ƒ | Is Available ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|
| Move Stan to grade 5 | This imagined change updates Stan’s grade to 5. His identifier, Person statement and family links stay the same. His other selected details are unchanged. | 1 | stan | false | keep | — | true | true | Move Stan to grade 5 | false | true |
| Stan leaves school | This imagined change clears Stan’s current school details. His identifier, Person statement and family links remain. Removing a current fact does not say that its opposite is true; school history needs dates. | 2 | stan | false | keep | — | true | true | Stan leaves school | false | true |
| Add next fact | — | 3 | facts | true | next | — | false | false | Add next fact | false | false |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Scenario Fact Change** | Fact changes a button makes after its baseline. A blank object key matches every listed value of the property. | — |
| Scenario | A defined attribute. | _The button._ |
| Subject | A defined attribute. | _The subject whose facts change._ |
| Property | A defined attribute. | _The property whose facts change._ |
| Object Key | A defined attribute. | _The object thing's key or the literal value; blank for every listed value._ |
| Action | A defined attribute. | _'on' switches the fact on as the newest value; 'off' switches it off._ |
| Adds to List | True when an empty string. | _Adds the fact to the fact list first when it is not listed._ |
| Sort Order | A defined attribute. | _Order of the change within the button._ |
| Name | The same as its scenario fact change ID. | _Order 1. The change's key._ |

<details open>
<summary>Example data</summary>

| Object Key | Action | Adds to List | Sort Order | Name ƒ |
|---|---|---|---|---|
| Person | on | true | 1 | stan-grade-five:Stan\|isA\|Person |
| 5 | on | true | 2 | stan-grade-five:Stan\|hasGradeLevel\|5 |
| Person | on | true | 3 | stan-leaves-school:Stan\|isA\|Person |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Scenario Rule Setting** | Rule switches a button sets. | — |
| Scenario | A defined attribute. | _The button._ |
| Rule | A defined attribute. | _The rule switch._ |
| Is Enabled | True when an empty string. | _The state the button puts the switch in._ |
| Name | The same as its scenario rule setting ID. | _Order 1. The setting's key._ |

<details open>
<summary>Example data</summary>

| Is Enabled | Name ƒ |
|---|---|
| false | reset-original:inverse |
| false | reset-original:symmetric |
| false | reset-original:chain |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Node Layout** | Node positions in the graph's 380 by 480 drawing, one set per layout. | — |
| Layout | A defined attribute. | _The layout: 'default' or 'stan-details'._ |
| Node Key | A defined attribute. | _The node: a thing's key, or literal\|value\|datatype._ |
| X | A defined attribute. | _Horizontal position._ |
| Y | A defined attribute. | _Vertical position._ |
| Name | The same as its node layout ID. | _Order 1. The position's key._ |

<details open>
<summary>Example data</summary>

| Layout | Node Key | X | Y | Name ƒ |
|---|---|---|---|---|
| default | Marvin | 70 | 45 | default:Marvin |
| default | Randy | 195 | 125 | default:Randy |
| default | Sharon | 318 | 55 | default:Sharon |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Layout Slot** | Places for nodes no layout positions, used in turn. | — |
| Sort Order | A defined attribute. | _Order in which slots are used._ |
| X | A defined attribute. | _Horizontal position._ |
| Y | A defined attribute. | _Vertical position._ |
| Name | The same as its layout slot ID. | _Order 1. The slot's key._ |

<details open>
<summary>Example data</summary>

| Sort Order | X | Y | Name ƒ |
|---|---|---|---|
| 1 | 330 | 135 | slot-1 |
| 2 | 45 | 120 | slot-2 |
| 3 | 330 | 400 | slot-3 |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Page Text** | The page's wording, one row per string. {name}-style placeholders are filled by the page. | — |
| Text | A defined attribute. | _The string._ |
| Name | The same as its page text ID. | _Order 1. The string's key._ |

<details open>
<summary>Example data</summary>

| Text | Name ƒ |
|---|---|
| South Park Ontology Explainer, on one rulebook | page-title |
| A rulebook rebuild of the explainer by | byline-prefix |
| Jessica Talisman | byline-author |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Engine** | The execution engines the page can be computed by. Each is a build output of this rulebook. | — |
| Label | A defined attribute. | _The engine's name in the switcher._ |
| Substrate | A defined attribute. | _The conformance harness's name for this substrate._ |
| Is Default | True when an empty string. | _The engine a new visitor starts on._ |
| Description | A defined attribute. | _What computes the page on this engine._ |
| Sort Order | A defined attribute. | _Position in the switcher._ |
| Name | The same as its label. | _Order 1. The engine's name._ |

<details open>
<summary>Example data</summary>

| Label | Substrate | Is Default | Description | Sort Order | Name ƒ |
|---|---|---|---|---|---|
| Postgres views | effortless-postgres | true | Every value is a column of a generated view; the database computes it when the page reads it. | 1 | Postgres views |
| TypeScript | effortless-typescript | false | The generated TypeScript computes every field from the same rows, with no database. | 2 | TypeScript |
| OWL and SHACL rules | effortless-owl | false | The generated ontology and SHACL rules, run over the same rows by a SPARQL engine. | 3 | OWL and SHACL rules |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Rulebook Tool** | What each rulebook tool adds to the original explainer; the tools panel lists them. | — |
| Label | A defined attribute. | _The tool's name._ |
| Adds | A defined attribute. | _What it adds to the original explainer._ |
| Href | A defined attribute. | _Where its link goes; 'editor' means the running rulebook editor._ |
| Link Text | A defined attribute. | _The link's text._ |
| Sort Order | A defined attribute. | _Position in the panel._ |
| Name | The same as its label. | _Order 1. The tool's name._ |

<details open>
<summary>Example data</summary>

| Label | Adds | Href | Link Text | Sort Order | Name ƒ |
|---|---|---|---|---|---|
| Interchangeable engines | The same page, computed by Postgres views, generated TypeScript or the generated OWL and SHACL rules. Switch with the control above; nothing else changes. | — | — | 1 | Interchangeable engines |
| Conformance | Every engine's answers for the current facts, compared value by value with the conformance harness's own rule for when two values are equal. | — | — | 2 | Conformance |
| Explainer | Hover a dotted value for its rule in plain words and its witness: the values it came from, down to the stated facts. | /rulebook-explainer-dag/pages/index.html | Open the explainer | 3 | Explainer |

_ƒ marks a computed column._

</details>

## 2 Fact Types

- a **firing kind** references exactly one **rule**
- a **thing** may reference one **portrait**
- a **portrait aliase** references exactly one **portrait**
- a **property** may reference one **thing**
- a **property axiom** references exactly one **firing kind**
- a **property axiom** references exactly one **property**
- a **statement** references exactly one **graph**
- a **statement** references exactly one **thing**
- a **statement** references exactly one **property**
- a **firing** references exactly one **firing kind**
- a **firing** references exactly one **rule**
- a **firing** references exactly one **statement**
- a **disjoint check** references exactly one **graph**
- a **disjoint check** references exactly one **rule**
- a **disjoint check** references exactly one **statement**
- a **scenario** references exactly one **graph**
- a **scenario fact change** references exactly one **scenario**
- a **scenario fact change** references exactly one **thing**
- a **scenario fact change** references exactly one **property**
- a **scenario rule setting** references exactly one **scenario**
- a **scenario rule setting** references exactly one **rule**

## 3 Operative Rules

_Operative rules state what the business **obliges**, **prohibits**, or
advises (**should**). Structural rules come from required fields and foreign keys;
semantic rules come from the Constraints table, each keyed on a boolean the rulebook
already computes (cross-referenced as DR-N in the Definitional Rules below)._

### Structural Constraints (from the schema)

- A graph **must** have a label; a literal quote; a literal type marker; a built in source text; a user source text; and no conclusions text.
- A rule **must** have a label; an explanation; and a sort order, and record whether it is enabled.
- A firing kind **must** reference exactly one rule.
- A firing kind **must** have a wording and a sort order.
- A thing **must** have a label; a kind; and a sort order.
- A portrait **must** have a display name; a page; an image; a crop; a width; a height; and a sort order.
- A portrait aliase **must** reference exactly one portrait.
- A property **must** have a label; a property kind; and a sort order, and record whether it is user addable; whether it has a one current value; and whether it is symmetric.
- A property axiom **must** reference exactly one firing kind as its kind.
- A property axiom **must** reference exactly one property.
- A property axiom **must** reference exactly one property as its first.
- A statement **must** reference exactly one graph.
- A statement **must** reference exactly one thing as its subject.
- A statement **must** reference exactly one property.
- A statement **must** have an activation seq and a sort order, and record whether it is listed; whether it is a built in; whether it is a detail; and whether it is stated.
- A firing **must** reference exactly one firing kind as its kind.
- A firing **must** reference exactly one rule.
- A firing **must** reference exactly one statement as its target.
- A firing **must** reference exactly one statement as its first.
- A disjoint check **must** reference exactly one graph.
- A disjoint check **must** reference exactly one rule.
- A disjoint check **must** reference exactly one statement as its first.
- A disjoint check **must** reference exactly one statement as its second.
- A scenario **must** reference exactly one graph.
- A scenario **must** have a label; a sort order; a button group; and a fact baseline, and record whether it is primary; whether it is clears selection; and whether it is clears form message.
- A scenario fact change **must** reference exactly one scenario.
- A scenario fact change **must** reference exactly one thing as its subject.
- A scenario fact change **must** reference exactly one property.
- A scenario fact change **must** have an action and a sort order, and record whether it is adds to list.
- A scenario rule setting **must** reference exactly one scenario.
- A scenario rule setting **must** reference exactly one rule.
- A scenario rule setting **must** record whether it is enabled.
- A node layout **must** have a layout; a node key; a x; and a y.
- A layout slot **must** have a sort order; a x; and a y.
- A page text **must** have a text.
- An engine **must** have a label; a substrate; a description; and a sort order, and record whether it is a default.
- A rulebook tool **must** have a label; an adds; and a sort order.

## 4 Definitional Rules

_All statements express truth in the business domain; they are neither
procedures nor imperatives. "iff" is avoided in favor of "only if" so a
one-directional necessity is not mistaken for an equivalence. A
**⚠︎ mechanical** chip marks a rule whose deterministic wording is faithful
but clunky — a flag for an optional downstream reword pass, not a defect._

| ID | Declarative rule |
|----|------------------|
| **DR-1 Name** | A graph's name is the same as its label. |
| **DR-2 Count of Stated Facts** | A graph's count of stated facts is the total in effect unit across the statements related to the graph. |
| **DR-3 Count of Conclusions** | A graph's count of conclusions is the total conclusion unit across the statements related to the graph. |
| **DR-4 Count of Violations** | A graph's count of violations is the total violation unit across the disjoint checks related to the graph. |
| **DR-5 Is Consistent** | A graph is considered consistent if the count of violations is 0. |
| **DR-6 Conclusions Message** | The graph's conclusions message is determined by the following priority:<br>1. the no conclusions text, if the count of conclusions is 0;<br>2. in all other cases, an empty string. |
| **DR-7 Count of Facts to Add** | A graph's count of facts to add is the total listed off unit across the statements related to the graph. |
| **DR-8 Has Facts to Add** | A graph is considered to have a facts to add if the count of facts to add is greater than 0. |
| **DR-9 Next Fact Sort Order** | A graph's next fact sort order is the smallest sort order across the graph's statements that are listed off unit. |
| **DR-10 Max Activation Seq** | A graph's max activation seq is the largest activation seq across the statements related to the graph. |
| **DR-11 Next Activation Seq** | A graph's next activation seq is computed as the max activation seq plus 1. |
| **DR-12 Max Listed Sort Order** | A graph's max listed sort order is the largest sort order across the graph's statements that are listed unit. |
| **DR-13 Next Listed Sort Order** | A graph's next listed sort order is computed as the max listed sort order plus 1. |
| **DR-14 Max Sort Order** | A graph's max sort order is the largest sort order across the statements related to the graph. |
| **DR-15 Next Candidate Sort Order** | A graph's next candidate sort order is computed as the max sort order plus 1. |
| **DR-16 Name** | A rule's name is the same as its label. |
| **DR-17 Count of Firings** | A rule's count of firings is the total fired unit across the firings related to the rule. |
| **DR-18 Name** | A firing kind's name is the same as its wording. |
| **DR-19 Rule Enabled** | A firing kind's rule enabled when the linked rule is enabled. |
| **DR-20 Portrait Name** | A thing's portrait name is the display name of the thing's portrait. |
| **DR-21 Portrait Page** | A thing's portrait page — taken from the linked portrait. |
| **DR-22 Portrait Image** | A thing's portrait image — taken from the linked portrait. |
| **DR-23 Portrait Crop** | A thing's portrait crop — taken from the linked portrait. |
| **DR-24 Portrait Width** | A thing's portrait width — taken from the linked portrait. |
| **DR-25 Portrait Height** | A thing's portrait height — taken from the linked portrait. |
| **DR-26 Name** | A thing's name is the same as its label. |
| **DR-27 Node Key** | A thing's node key is the same as its thing ID. |
| **DR-28 Count of Children** | A thing's count of children is the total parent unit across the statements related to the thing. |
| **DR-29 Count of Grandchildren** | A thing's count of grandchildren is the total grandparent unit across the statements related to the thing. |
| **DR-30 Count As Place** | A thing's count as place is the total place unit across the statements related to the thing. |
| **DR-31 Count As Subject** | A thing's count as subject is the total holds unit across the statements related to the thing. |
| **DR-32 Count As Object** | A thing's count as object is the total holds unit across the statements related to the thing. |
| **DR-33 Is in Graph** | A thing is considered in-graph if at least one of the following holds: the count as subject is greater than 0 or the count as object is greater than 0. |
| **DR-34 Node Kind** | The thing's node kind is determined by the following priority:<br>1. an empty string, if the kind is blank;<br>2. “class”, if the kind is “class”;<br>3. “place”, if the count as place is greater than 0;<br>4. in all other cases, “person”. |
| **DR-35 Name** | A portrait's name is the same as its display name. |
| **DR-36 Name** | A portrait aliase's name is the same as its portrait alias ID. |
| **DR-37 Name** | A property's name is the same as its label. |
| **DR-38 Name** | A property axiom's name is the same as its property axiom ID. |
| **DR-39 Subject Label** | A statement's subject label — taken from the linked subject. |
| **DR-40 Property Label** | A statement's property label — taken from the linked property. |
| **DR-41 Object Label** | A statement's object label — taken from the linked object thing. |
| **DR-42 Property Datatype** | A statement's property datatype — taken from the linked property. |
| **DR-43 Property Has One Current Value** | A statement's property has one current value when the linked property has a one current value. |
| **DR-44 Property Domain Class** | A statement's property domain class — taken from the linked property. |
| **DR-45 Property Range Class** | A statement's property range class — taken from the linked property. |
| **DR-46 Literal Quote** | A statement's literal quote — taken from the linked graph. |
| **DR-47 Literal Type Marker** | A statement's literal type marker — taken from the linked graph. |
| **DR-48 Built in Source Text** | A statement's built in source text — taken from the linked graph. |
| **DR-49 User Source Text** | A statement's user source text — taken from the linked graph. |
| **DR-50 Next Fact Sort Order** | A statement's next fact sort order — taken from the linked graph. |
| **DR-51 Is Literal** | A statement is considered literal if the object thing is blank. |
| **DR-52 Object Text** | The statement's object text is determined by the following priority:<br>1. the object value, if the object thing is blank;<br>2. in all other cases, the object label. |
| **DR-53 Name** | A statement's name is computed as the subject label, followed by a space, followed by the property label, followed by a space, followed by the object text. |
| **DR-54 Words** | A statement's words is computed as the subject label, followed by a space, followed by the property label, followed by a space, followed by the literal quote, followed by the object value, followed by the literal quote, followed by the literal type marker, followed by the property datatype if the object thing is blank, in all other cases the object label. ⚠︎ mechanical <!-- rulespeak:reword --> |
| **DR-55 Source** | The statement's source is determined by the following priority:<br>1. the built in source text, if the built in flag is set;<br>2. in all other cases, the user source text. |
| **DR-56 Subject Property Key** | A statement's subject property key is computed as the subject, followed by “|”, followed by the property. |
| **DR-57 Latest Sibling Activation Seq** | A statement's latest sibling activation seq is the largest activation seq across the statements related to the statement. |
| **DR-58 Is in Effect** | A statement is considered in-effect if all of the following hold: the stated flag is set and at least one of the following holds: the property has one current value flag is not set or the activation seq is the latest sibling activation seq. |
| **DR-59 Listed Unit** | The statement's listed unit is determined by the following priority:<br>1. 1, if the listed flag is set;<br>2. in all other cases, 0. |
| **DR-60 In Effect Unit** | The statement's in effect unit is determined by the following priority:<br>1. 1, if the in effect flag is set;<br>2. in all other cases, 0. |
| **DR-61 Listed Off Unit** | The statement's listed off unit is determined by the following priority:<br>1. 1, if all of the following hold: the listed flag is set and the in effect flag is not set;<br>2. in all other cases, 0. |
| **DR-62 Is Next to Add** | A statement is considered a next to add if all of the following hold: the listed off unit is 1 and the sort order is the next fact sort order. |
| **DR-63 Count of Inverse Firings** | A statement's count of inverse firings is the total inverse fired unit across the firings related to the statement. |
| **DR-64 Count of Symmetric Firings** | A statement's count of symmetric firings is the total symmetric fired unit across the firings related to the statement. |
| **DR-65 Holds Before Chain** | A statement is considered to hold a before chain if the IF of the OR of the is in effect; the count of inverse firings is greater than 0; and the count of symmetric firings is greater than 0 and true. |
| **DR-66 Count of Chain Firings** | A statement's count of chain firings is the total chain fired unit across the firings related to the statement. |
| **DR-67 Holds Before Typing** | A statement is considered to hold before typing if the IF of the OR of the holds before chain and the count of chain firings is greater than 0 and true. |
| **DR-68 Count of Type Firings** | A statement's count of type firings is the total type fired unit across the firings related to the statement. |
| **DR-69 Holds** | A statement is considered to hold if the IF of the OR of the holds before typing and the count of type firings is greater than 0 and true. |
| **DR-70 Is Conclusion** | A statement is considered a conclusion if the IF of the AND of the holds and the in effect flag is not set and true. |
| **DR-71 Holds Unit** | The statement's holds unit is determined by the following priority:<br>1. 1, if the holds flag is set;<br>2. in all other cases, 0. |
| **DR-72 Conclusion Unit** | The statement's conclusion unit is determined by the following priority:<br>1. 1, if the conclusion flag is set;<br>2. in all other cases, 0. |
| **DR-73 Parent Unit** | The statement's parent unit is determined by the following priority:<br>1. 1, if all of the following hold: the holds flag is set and the property is “isParentOf”;<br>2. in all other cases, 0. |
| **DR-74 Grandparent Unit** | The statement's grandparent unit is determined by the following priority:<br>1. 1, if all of the following hold: the holds flag is set and the property is “isGrandparentOf”;<br>2. in all other cases, 0. |
| **DR-75 Place Unit** | The statement's place unit is determined by the following priority:<br>1. 1, if all of the following hold: the holds flag is set and the property is “attends”;<br>2. in all other cases, 0. |
| **DR-76 First Pre Chain Order Key** | A statement's first pre chain order key is the smallest pre chain order key across the statement's firings that are pre chain fired unit. |
| **DR-77 Snap Position** | The statement's snap position is determined by the following priority:<br>1. the sort order, if the in effect flag is set;<br>2. 100000 plus the first pre chain order key, if the holds before chain flag is set;<br>3. in all other cases, 0. |
| **DR-78 First Fired Order Key** | A statement's first fired order key is the smallest order key across the statement's firings that are fired unit. |
| **DR-79 Count of Chosen Firings** | A statement's count of chosen firings is the total chosen unit across the firings related to the statement. |
| **DR-80 Subject Node Key** | A statement's subject node key is the same as its subject. |
| **DR-81 Object Node Key** | The statement's object node key is determined by the following priority:<br>1. “literal|”, followed by the object value, followed by “|”, followed by the property datatype, if the object thing is blank;<br>2. in all other cases, the object thing. |
| **DR-82 Subject Node Kind** | A statement's subject node kind — taken from the linked subject. |
| **DR-83 Object Thing Node Kind** | A statement's object thing node kind — taken from the linked object thing. |
| **DR-84 Object Node Kind** | The statement's object node kind is determined by the following priority:<br>1. “value”, if the object thing is blank;<br>2. in all other cases, the object thing node kind. |
| **DR-85 Object Value Node Label** | A statement's object value node label — taken from the linked property. |
| **DR-86 Count of Violations As First** | A statement's count of violations as first is the total violation unit across the disjoint checks related to the statement. |
| **DR-87 Count of Violations As Second** | A statement's count of violations as second is the total violation unit across the disjoint checks related to the statement. |
| **DR-88 Is in Contradiction** | A statement is considered in-contradiction if at least one of the following holds: the count of violations as first is greater than 0 or the count of violations as second is greater than 0. |
| **DR-89 Rule Enabled** | A firing's rule enabled when the linked rule is enabled. |
| **DR-90 Kind Wording** | A firing's kind wording — taken from the linked kind. |
| **DR-91 Kind Sort Order** | A firing's kind sort order — taken from the linked kind. |
| **DR-92 Target Name** | A firing's target name — taken from the linked target. |
| **DR-93 Target Subject Label** | A firing's target subject label — taken from the linked target. |
| **DR-94 Target Property Label** | A firing's target property label — taken from the linked target. |
| **DR-95 Target Object Text** | A firing's target object text — taken from the linked target. |
| **DR-96 Target is Conclusion** | A firing's target is conclusion when the linked target is a conclusion. |
| **DR-97 Target Subject Node Key** | A firing's target subject node key — taken from the linked target. |
| **DR-98 Target Object Node Key** | A firing's target object node key — taken from the linked target. |
| **DR-99 Target First Fired Order Key** | A firing's target first fired order key — taken from the linked target. |
| **DR-100 First Words** | A firing's first words — taken from the linked first. |
| **DR-101 Second Words** | A firing's second words — taken from the linked second. |
| **DR-102 First is in Effect** | A firing's first is in effect when the linked first is in effect. |
| **DR-103 First Holds Before Chain** | A firing's first holds before chain when the linked first is holds before chain. |
| **DR-104 Second Holds Before Chain** | A firing's second holds before chain when the linked second is holds before chain. |
| **DR-105 First Holds Before Typing** | A firing's first holds before typing when the linked first is holds before typing. |
| **DR-106 First Property** | A firing's first property — taken from the linked first. |
| **DR-107 First Property Domain Class** | A firing's first property domain class — taken from the linked first. |
| **DR-108 First Property Range Class** | A firing's first property range class — taken from the linked first. |
| **DR-109 First Sort Order** | A firing's first sort order — taken from the linked first. |
| **DR-110 Second is in Effect** | A firing's second is in effect when the linked second is in effect. |
| **DR-111 First Snap Position** | A firing's first snap position — taken from the linked first. |
| **DR-112 Second Snap Position** | A firing's second snap position — taken from the linked second. |
| **DR-113 Inverse Fired Unit** | The firing's inverse fired unit is determined by the following priority:<br>1. 1, if all of the following hold: the rule enabled flag is set; the kind is “inverse”; and the first is in effect flag is set;<br>2. in all other cases, 0. |
| **DR-114 Symmetric Fired Unit** | The firing's symmetric fired unit is determined by the following priority:<br>1. 1, if all of the following hold: the rule enabled flag is set; the kind is “symmetric”; and the first is in effect flag is set;<br>2. in all other cases, 0. |
| **DR-115 Chain Fired Unit** | The firing's chain fired unit is determined by the following priority:<br>1. 1, if all of the following hold: the rule enabled flag is set; the kind is “chain”; the first holds before chain flag is set; and the second holds before chain flag is set;<br>2. in all other cases, 0. |
| **DR-116 Type Fired Unit** | The firing's type fired unit is determined by the following priority:<br>1. 1, if all of the following hold: the rule enabled flag is set; at least one of the following holds: the kind is “domain” or the kind is “range”; and the first holds before typing flag is set;<br>2. in all other cases, 0. |
| **DR-117 Fired Unit** | A firing's fired unit is computed as the inverse fired unit plus the symmetric fired unit plus the chain fired unit plus the type fired unit. |
| **DR-118 Pre Chain Fired Unit** | A firing's pre chain fired unit is computed as the inverse fired unit plus the symmetric fired unit. |
| **DR-119 Pre Chain Order Key** | A firing's pre chain order key is computed as the first sort order times 10 plus the kind sort order. |
| **DR-120 Round** | The firing's round is determined by the following priority:<br>1. 2, if all of the following hold: the kind is “chain” and it is not the case that all of the following hold: the first is in effect flag is set and the second is in effect flag is set;<br>2. in all other cases, 1. |
| **DR-121 Order Key** | A firing's order key is computed as the round times 1000000000000000 plus the first snap position times 100000000 plus the kind sort order times 10000000 plus 0 if the second is blank, in all other cases the second snap position. |
| **DR-122 Is Chosen** | A firing is considered a chosen if all of the following hold: the fired unit is 1; the target is conclusion flag is set; and the order key is the target first fired order key. |
| **DR-123 Chosen Unit** | The firing's chosen unit is determined by the following priority:<br>1. 1, if the chosen flag is set;<br>2. in all other cases, 0. |
| **DR-124 Card Text** | The firing's card text is determined by the following priority:<br>1. “Worked out by the ”, followed by the kind wording, followed by “ from “”, followed by the first words, followed by “””, followed by “ and “”, followed by the second words, followed by “”” if the kind is “chain”, in all other cases “ and “”, followed by the first property, followed by “ has domain ”, followed by the first property domain class, followed by “”” if the kind is “domain”, in all other cases “ and “”, followed by the first property, followed by “ has range ”, followed by the first property range class, followed by “””, followed by a period, if the chosen flag is set;<br>2. in all other cases, an empty string. ⚠︎ mechanical <!-- rulespeak:reword --> |
| **DR-125 Name** | A firing's name is the same as its firing ID. |
| **DR-126 Rule Enabled** | A disjoint check's rule enabled when the linked rule is enabled. |
| **DR-127 First Holds** | A disjoint check's first holds when the linked first is holds. |
| **DR-128 Second Holds** | A disjoint check's second holds when the linked second is holds. |
| **DR-129 Subject Label** | A disjoint check's subject label — taken from the linked first. |
| **DR-130 Object Label** | A disjoint check's object label — taken from the linked first. |
| **DR-131 Violation Unit** | The disjoint check's violation unit is determined by the following priority:<br>1. 1, if all of the following hold: the rule enabled flag is set; the first holds flag is set; and the second holds flag is set;<br>2. in all other cases, 0. |
| **DR-132 Alarm Text** | The disjoint check's alarm text is determined by the following priority:<br>1. “Contradiction found. ”, followed by the subject label, followed by “ is recorded as both parent and spouse of ”, followed by the object label, followed by “. Check those facts and the selected disjoint rule.”, if the violation unit is 1;<br>2. in all other cases, an empty string. |
| **DR-133 Name** | A disjoint check's name is the same as its disjoint check ID. |
| **DR-134 Name** | A scenario's name is the same as its label. |
| **DR-135 Graph Has Facts to Add** | A scenario's graph has facts to add when the linked graph has a facts to add. |
| **DR-136 Is Available** | A scenario is considered available if at least one of the following holds: the fact baseline is not “next” or the graph has facts to add flag is set. |
| **DR-137 Name** | A scenario fact change's name is the same as its scenario fact change ID. |
| **DR-138 Name** | A scenario rule setting's name is the same as its scenario rule setting ID. |
| **DR-139 Name** | A node layout's name is the same as its node layout ID. |
| **DR-140 Name** | A layout slot's name is the same as its layout slot ID. |
| **DR-141 Name** | A page text's name is the same as its page text ID. |
| **DR-142 Name** | An engine's name is the same as its label. |
| **DR-143 Name** | A rulebook tool's name is the same as its label. |

## 5 Traceability to Schema

_The expression column is the rule's definition in Docs notation —
the same logic the rulebook stores, written for a business reader._

| Schema element | Kind | Expression |
|----------------|------|------------|
| **Graphs.Name** | formula | `Label` |
| **Graphs.CountOfStatedFacts** | rollup | `Sum(Statements.InEffectUnit via Graph)` |
| **Graphs.CountOfConclusions** | rollup | `Sum(Statements.ConclusionUnit via Graph)` |
| **Graphs.CountOfViolations** | rollup | `Sum(DisjointChecks.ViolationUnit via Graph)` |
| **Graphs.IsConsistent** | formula | `CountOfViolations = 0` |
| **Graphs.ConclusionsMessage** | formula | `If(CountOfConclusions = 0, NoConclusionsText, "")` |
| **Graphs.CountOfFactsToAdd** | rollup | `Sum(Statements.ListedOffUnit via Graph)` |
| **Graphs.HasFactsToAdd** | formula | `CountOfFactsToAdd > 0` |
| **Graphs.NextFactSortOrder** | rollup | `Min(Statements.SortOrder via Graph)` |
| **Graphs.MaxActivationSeq** | rollup | `Max(Statements.ActivationSeq via Graph)` |
| **Graphs.NextActivationSeq** | formula | `MaxActivationSeq + 1` |
| **Graphs.MaxListedSortOrder** | rollup | `Max(Statements.SortOrder via Graph)` |
| **Graphs.NextListedSortOrder** | formula | `MaxListedSortOrder + 1` |
| **Graphs.MaxSortOrder** | rollup | `Max(Statements.SortOrder via Graph)` |
| **Graphs.NextCandidateSortOrder** | formula | `MaxSortOrder + 1` |
| **Rules.Name** | formula | `Label` |
| **Rules.CountOfFirings** | rollup | `Sum(Firings.FiredUnit via Rule)` |
| **FiringKinds.Name** | formula | `Wording` |
| **FiringKinds.RuleEnabled** | lookup | `Lookup(Rules.IsEnabled via Rule)` |
| **Things.PortraitName** | lookup | `Lookup(Portraits.DisplayName via Portrait)` |
| **Things.PortraitPage** | lookup | `Lookup(Portraits.Page via Portrait)` |
| **Things.PortraitImage** | lookup | `Lookup(Portraits.Image via Portrait)` |
| **Things.PortraitCrop** | lookup | `Lookup(Portraits.Crop via Portrait)` |
| **Things.PortraitWidth** | lookup | `Lookup(Portraits.Width via Portrait)` |
| **Things.PortraitHeight** | lookup | `Lookup(Portraits.Height via Portrait)` |
| **Things.Name** | formula | `Label` |
| **Things.NodeKey** | formula | `ThingId` |
| **Things.CountOfChildren** | rollup | `Sum(Statements.ParentUnit via Subject)` |
| **Things.CountOfGrandchildren** | rollup | `Sum(Statements.GrandparentUnit via Subject)` |
| **Things.CountAsPlace** | rollup | `Sum(Statements.PlaceUnit via ObjectThing)` |
| **Things.CountAsSubject** | rollup | `Sum(Statements.HoldsUnit via Subject)` |
| **Things.CountAsObject** | rollup | `Sum(Statements.HoldsUnit via ObjectThing)` |
| **Things.IsInGraph** | formula | `Or(CountAsSubject > 0, CountAsObject > 0)` |
| **Things.NodeKind** | formula | `If(Kind = "", "", If(Kind = "class", "class", If(CountAsPlace > 0, "place", "person")))` |
| **Portraits.Name** | formula | `DisplayName` |
| **PortraitAliases.Name** | formula | `PortraitAliasId` |
| **Properties.Name** | formula | `Label` |
| **PropertyAxioms.Name** | formula | `PropertyAxiomId` |
| **Statements.SubjectLabel** | lookup | `Lookup(Things.Label via Subject)` |
| **Statements.PropertyLabel** | lookup | `Lookup(Properties.Label via Property)` |
| **Statements.ObjectLabel** | lookup | `Lookup(Things.Label via ObjectThing)` |
| **Statements.PropertyDatatype** | lookup | `Lookup(Properties.Datatype via Property)` |
| **Statements.PropertyHasOneCurrentValue** | lookup | `Lookup(Properties.HasOneCurrentValue via Property)` |
| **Statements.PropertyDomainClass** | lookup | `Lookup(Properties.DomainClass via Property)` |
| **Statements.PropertyRangeClass** | lookup | `Lookup(Properties.RangeClass via Property)` |
| **Statements.LiteralQuote** | lookup | `Lookup(Graphs.LiteralQuote via Graph)` |
| **Statements.LiteralTypeMarker** | lookup | `Lookup(Graphs.LiteralTypeMarker via Graph)` |
| **Statements.BuiltInSourceText** | lookup | `Lookup(Graphs.BuiltInSourceText via Graph)` |
| **Statements.UserSourceText** | lookup | `Lookup(Graphs.UserSourceText via Graph)` |
| **Statements.NextFactSortOrder** | lookup | `Lookup(Graphs.NextFactSortOrder via Graph)` |
| **Statements.IsLiteral** | formula | `ObjectThing = ""` |
| **Statements.ObjectText** | formula | `If(ObjectThing = "", ObjectValue, ObjectLabel)` |
| **Statements.Name** | formula | `Concat(SubjectLabel, " ", PropertyLabel, " ", ObjectText)` |
| **Statements.Words** | formula | `Concat(SubjectLabel, " ", PropertyLabel, " ", If(ObjectThing = "", Concat(LiteralQuote, ObjectValue, LiteralQuote, LiteralTypeMarker, PropertyDatatype), ObjectLabel))` |
| **Statements.Source** | formula | `If(IsBuiltIn, BuiltInSourceText, UserSourceText)` |
| **Statements.SubjectPropertyKey** | formula | `Concat(Subject, "\|", Property)` |
| **Statements.LatestSiblingActivationSeq** | rollup | `Max(Statements.ActivationSeq via SubjectPropertyKey)` |
| **Statements.IsInEffect** | formula | `And(IsStated, Or(Not(PropertyHasOneCurrentValue), ActivationSeq = LatestSiblingActivationSeq))` |
| **Statements.ListedUnit** | formula | `If(IsListed, 1, 0)` |
| **Statements.InEffectUnit** | formula | `If(IsInEffect, 1, 0)` |
| **Statements.ListedOffUnit** | formula | `If(And(IsListed, Not(IsInEffect)), 1, 0)` |
| **Statements.IsNextToAdd** | formula | `And(ListedOffUnit = 1, SortOrder = NextFactSortOrder)` |
| **Statements.CountOfInverseFirings** | rollup | `Sum(Firings.InverseFiredUnit via Target)` |
| **Statements.CountOfSymmetricFirings** | rollup | `Sum(Firings.SymmetricFiredUnit via Target)` |
| **Statements.HoldsBeforeChain** | formula | `If(Or(IsInEffect, CountOfInverseFirings > 0, CountOfSymmetricFirings > 0), True())` |
| **Statements.CountOfChainFirings** | rollup | `Sum(Firings.ChainFiredUnit via Target)` |
| **Statements.HoldsBeforeTyping** | formula | `If(Or(HoldsBeforeChain, CountOfChainFirings > 0), True())` |
| **Statements.CountOfTypeFirings** | rollup | `Sum(Firings.TypeFiredUnit via Target)` |
| **Statements.Holds** | formula | `If(Or(HoldsBeforeTyping, CountOfTypeFirings > 0), True())` |
| **Statements.IsConclusion** | formula | `If(And(Holds, Not(IsInEffect)), True())` |
| **Statements.HoldsUnit** | formula | `If(Holds, 1, 0)` |
| **Statements.ConclusionUnit** | formula | `If(IsConclusion, 1, 0)` |
| **Statements.ParentUnit** | formula | `If(And(Holds, Property = "isParentOf"), 1, 0)` |
| **Statements.GrandparentUnit** | formula | `If(And(Holds, Property = "isGrandparentOf"), 1, 0)` |
| **Statements.PlaceUnit** | formula | `If(And(Holds, Property = "attends"), 1, 0)` |
| **Statements.FirstPreChainOrderKey** | rollup | `Min(Firings.PreChainOrderKey via Target)` |
| **Statements.SnapPosition** | formula | `If(IsInEffect, SortOrder, If(HoldsBeforeChain, 100000 + FirstPreChainOrderKey, 0))` |
| **Statements.FirstFiredOrderKey** | rollup | `Min(Firings.OrderKey via Target)` |
| **Statements.CountOfChosenFirings** | rollup | `Sum(Firings.ChosenUnit via Target)` |
| **Statements.SubjectNodeKey** | formula | `Subject` |
| **Statements.ObjectNodeKey** | formula | `If(ObjectThing = "", Concat("literal\|", ObjectValue, "\|", PropertyDatatype), ObjectThing)` |
| **Statements.SubjectNodeKind** | lookup | `Lookup(Things.NodeKind via Subject)` |
| **Statements.ObjectThingNodeKind** | lookup | `Lookup(Things.NodeKind via ObjectThing)` |
| **Statements.ObjectNodeKind** | formula | `If(ObjectThing = "", "value", ObjectThingNodeKind)` |
| **Statements.ObjectValueNodeLabel** | lookup | `Lookup(Properties.ValueNodeLabel via Property)` |
| **Statements.CountOfViolationsAsFirst** | rollup | `Sum(DisjointChecks.ViolationUnit via First)` |
| **Statements.CountOfViolationsAsSecond** | rollup | `Sum(DisjointChecks.ViolationUnit via Second)` |
| **Statements.IsInContradiction** | formula | `Or(CountOfViolationsAsFirst > 0, CountOfViolationsAsSecond > 0)` |
| **Firings.RuleEnabled** | lookup | `Lookup(Rules.IsEnabled via Rule)` |
| **Firings.KindWording** | lookup | `Lookup(FiringKinds.Wording via Kind)` |
| **Firings.KindSortOrder** | lookup | `Lookup(FiringKinds.SortOrder via Kind)` |
| **Firings.TargetName** | lookup | `Lookup(Statements.Name via Target)` |
| **Firings.TargetSubjectLabel** | lookup | `Lookup(Statements.SubjectLabel via Target)` |
| **Firings.TargetPropertyLabel** | lookup | `Lookup(Statements.PropertyLabel via Target)` |
| **Firings.TargetObjectText** | lookup | `Lookup(Statements.ObjectText via Target)` |
| **Firings.TargetIsConclusion** | lookup | `Lookup(Statements.IsConclusion via Target)` |
| **Firings.TargetSubjectNodeKey** | lookup | `Lookup(Statements.SubjectNodeKey via Target)` |
| **Firings.TargetObjectNodeKey** | lookup | `Lookup(Statements.ObjectNodeKey via Target)` |
| **Firings.TargetFirstFiredOrderKey** | lookup | `Lookup(Statements.FirstFiredOrderKey via Target)` |
| **Firings.FirstWords** | lookup | `Lookup(Statements.Words via First)` |
| **Firings.SecondWords** | lookup | `Lookup(Statements.Words via Second)` |
| **Firings.FirstIsInEffect** | lookup | `Lookup(Statements.IsInEffect via First)` |
| **Firings.FirstHoldsBeforeChain** | lookup | `Lookup(Statements.HoldsBeforeChain via First)` |
| **Firings.SecondHoldsBeforeChain** | lookup | `Lookup(Statements.HoldsBeforeChain via Second)` |
| **Firings.FirstHoldsBeforeTyping** | lookup | `Lookup(Statements.HoldsBeforeTyping via First)` |
| **Firings.FirstProperty** | lookup | `Lookup(Statements.Property via First)` |
| **Firings.FirstPropertyDomainClass** | lookup | `Lookup(Statements.PropertyDomainClass via First)` |
| **Firings.FirstPropertyRangeClass** | lookup | `Lookup(Statements.PropertyRangeClass via First)` |
| **Firings.FirstSortOrder** | lookup | `Lookup(Statements.SortOrder via First)` |
| **Firings.SecondIsInEffect** | lookup | `Lookup(Statements.IsInEffect via Second)` |
| **Firings.FirstSnapPosition** | lookup | `Lookup(Statements.SnapPosition via First)` |
| **Firings.SecondSnapPosition** | lookup | `Lookup(Statements.SnapPosition via Second)` |
| **Firings.InverseFiredUnit** | formula | `If(And(RuleEnabled, Kind = "inverse", FirstIsInEffect), 1, 0)` |
| **Firings.SymmetricFiredUnit** | formula | `If(And(RuleEnabled, Kind = "symmetric", FirstIsInEffect), 1, 0)` |
| **Firings.ChainFiredUnit** | formula | `If(And(RuleEnabled, Kind = "chain", FirstHoldsBeforeChain, SecondHoldsBeforeChain), 1, 0)` |
| **Firings.TypeFiredUnit** | formula | `If(And(RuleEnabled, Or(Kind = "domain", Kind = "range"), FirstHoldsBeforeTyping), 1, 0)` |
| **Firings.FiredUnit** | formula | `InverseFiredUnit + SymmetricFiredUnit + ChainFiredUnit + TypeFiredUnit` |
| **Firings.PreChainFiredUnit** | formula | `InverseFiredUnit + SymmetricFiredUnit` |
| **Firings.PreChainOrderKey** | formula | `FirstSortOrder * 10 + KindSortOrder` |
| **Firings.Round** | formula | `If(And(Kind = "chain", Not(And(FirstIsInEffect, SecondIsInEffect))), 2, 1)` |
| **Firings.OrderKey** | formula | `Round * 1000000000000000 + FirstSnapPosition * 100000000 + KindSortOrder * 10000000 + If(Second = "", 0, SecondSnapPosition)` |
| **Firings.IsChosen** | formula | `And(FiredUnit = 1, TargetIsConclusion, OrderKey = TargetFirstFiredOrderKey)` |
| **Firings.ChosenUnit** | formula | `If(IsChosen, 1, 0)` |
| **Firings.CardText** | formula | `If(IsChosen, Concat("Worked out by the ", KindWording, " from “", FirstWords, "”", If(Kind = "chain", Concat(" and “", SecondWords, "”"), If(Kind = "domain", Concat(" and “", FirstProperty, " has domain ", FirstPropertyDomainClass, "”"), If(Kind = "range", Concat(" and “", FirstProperty, " has range ", FirstPropertyRangeClass, "”"), ""))), "."), "")` |
| **Firings.Name** | formula | `FiringId` |
| **DisjointChecks.RuleEnabled** | lookup | `Lookup(Rules.IsEnabled via Rule)` |
| **DisjointChecks.FirstHolds** | lookup | `Lookup(Statements.Holds via First)` |
| **DisjointChecks.SecondHolds** | lookup | `Lookup(Statements.Holds via Second)` |
| **DisjointChecks.SubjectLabel** | lookup | `Lookup(Statements.SubjectLabel via First)` |
| **DisjointChecks.ObjectLabel** | lookup | `Lookup(Statements.ObjectLabel via First)` |
| **DisjointChecks.ViolationUnit** | formula | `If(And(RuleEnabled, FirstHolds, SecondHolds), 1, 0)` |
| **DisjointChecks.AlarmText** | formula | `If(ViolationUnit = 1, Concat("Contradiction found. ", SubjectLabel, " is recorded as both parent and spouse of ", ObjectLabel, ". Check those facts and the selected disjoint rule."), "")` |
| **DisjointChecks.Name** | formula | `DisjointCheckId` |
| **Scenarios.Name** | formula | `Label` |
| **Scenarios.GraphHasFactsToAdd** | lookup | `Lookup(Graphs.HasFactsToAdd via Graph)` |
| **Scenarios.IsAvailable** | formula | `Or(FactBaseline <> "next", GraphHasFactsToAdd)` |
| **ScenarioFactChanges.Name** | formula | `ScenarioFactChangeId` |
| **ScenarioRuleSettings.Name** | formula | `ScenarioRuleSettingId` |
| **NodeLayouts.Name** | formula | `NodeLayoutId` |
| **LayoutSlots.Name** | formula | `LayoutSlotId` |
| **PageTexts.Name** | formula | `PageTextId` |
| **Engines.Name** | formula | `Label` |
| **RulebookTools.Name** | formula | `Label` |

---

> **RuleSpeak®** — the declarative business-rule notation created by **Ronald G. Ross**, following the conventions of **SBVR** — is coming soon. Until then this page shows the regular documentation. [www.RonRoss.info](https://www.RonRoss.info)
