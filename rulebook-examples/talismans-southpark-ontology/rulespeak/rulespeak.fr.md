# 📘 Talisman's South Park ontology explainer — Documentation

_Jessica Talisman's South Park Ontology Explainer, as a rulebook: the page's ten facts, five rule switches, property wiring, provenance wording and current-value policy are rows, and every conclusion is a formula. The rules apply in three fixed strata (inverse and symmetric, then the property chain, then domain and range); property wiring that feeds an earlier stratum would need repeated rounds. Every fact that could hold over the cast is a candidate row; app/lib/candidates.mjs writes the candidates when a thing, value or property is added._

> Règles métier déclaratives générées à partir du livre de règles. Chaque énoncé
> exprime une vérité du domaine métier — ce n'est ni une procédure ni un impératif.
> Les formules du livre de règles sont l'unique source de vérité ; ce document en
> est la lecture en langage clair.

## 1 Vocabulaire métier

| Terme | Description | Commentaire narratif |
|------|-------------|-------------------|
| **Graph** | The explainer page as one graph: its wording variables and its totals. | — |
| Label | Un attribut défini. | _Display name of the graph._ |
| Literal Quote | Un attribut défini. | _The quote mark written around a literal value in a fact's wording._ |
| Literal Type Marker | Un attribut défini. | _The marker written between a literal value and its datatype, as in Turtle._ |
| Built in Source Text | Un attribut défini. | _Source line shown for a fact the page states as a teaching example._ |
| User Source Text | Un attribut défini. | _Source line shown for a fact the reader stated._ |
| No Conclusions Text | Un attribut défini. | _Message shown when the selected facts and rules add no conclusion._ |
| Name | Identique à label. | _Order 1. Display name of the graph._ |
| Count of Stated Facts | Le total de in effect unit sur les statements liés à le graph. | _Order 5. Facts that are switched on and in effect._ |
| Count of Conclusions | Le total de conclusion unit sur les statements liés à le graph. | _Order 18. Facts that hold only because an enabled rule concluded them._ |
| Count of Violations | Le total de violation unit sur les disjoint checks liés à le graph. | _Order 18. Disjointness checks that found both links between the same pair._ |
| Is Consistent | Vrai lorsque le count of violations est 0. | _Order 19. True when no enabled disjointness rule is violated._ |
| Conclusions Message | Déterminé par priorité : le no conclusions text si le count of conclusions est 0 ; dans tous les autres cas, une chaîne vide. | _Order 19. The empty-state message for the conclusions panel, blank when there are conclusions._ |
| Count of Facts to Add | Le total de listed off unit sur les statements liés à le graph. | _Order 5. Listed facts that are not in effect; the Add next fact button is enabled while this is above zero._ |
| Has Facts to Add | Vrai lorsque le count of facts to add est supérieur à 0. | _Order 6. True while some listed fact can still be added._ |
| Next Fact Sort Order | La plus petite valeur de sort order sur les statements du graph qui sont listed off unit. | _Order 5. List position of the first listed fact that is not in effect._ |
| Max Activation Seq | La plus grande valeur de activation seq sur les statements liés à le graph. | _Order 1. Highest activation number used so far._ |
| Next Activation Seq | Calculé comme le max activation seq plus 1. | _Order 2. The activation number the next switched-on fact takes._ |
| Max Listed Sort Order | La plus grande valeur de sort order sur les statements du graph qui sont listed unit. | _Order 2. Highest list position among listed facts._ |
| Next Listed Sort Order | Calculé comme le max listed sort order plus 1. | _Order 3. The list position a newly added fact takes._ |
| Max Sort Order | La plus grande valeur de sort order sur les statements liés à le graph. | _Order 1. Highest sort order of any fact, listed or candidate._ |
| Next Candidate Sort Order | Calculé comme le max sort order plus 1. | _Order 2. The sort order a new candidate fact, or a fact removed from the list, takes._ |

<details open>
<summary>Données d'exemple</summary>

| Label | Literal Quote | Literal Type Marker | Built in Source Text | User Source Text | No Conclusions Text | Name ƒ | Count of Stated Facts ƒ | Count of Conclusions ƒ | Count of Violations ƒ | Is Consistent ƒ | Conclusions Message ƒ | Count of Facts to Add ƒ | Has Facts to Add ƒ | Next Fact Sort Order ƒ | Max Activation Seq ƒ | Next Activation Seq ƒ | Max Listed Sort Order ƒ | Next Listed Sort Order ƒ | Max Sort Order ƒ | Next Candidate Sort Order ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| South Park | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | These facts and selected rules add no further conclusions. Try another fact or rule. | South Park | 10 | 0 | 0 | true | These facts and selected rules add no further conclusions. Try another fact or rule. | 0 | false | 0 | 10 | 11 | 10 | 11 | 100269 | 100270 |

_ƒ signale une colonne calculée._

</details>

| Terme | Description | Commentaire narratif |
|------|-------------|-------------------|
| **Rule** | The five rule switches on the page. | — |
| Label | Un attribut défini. | _The rule's name on the page._ |
| Explanation | Un attribut défini. | _The rule's one-line explanation on the page._ |
| Is Enabled | Vrai lorsque une chaîne vide. | _The rule's checkbox._ |
| Sort Order | Un attribut défini. | _Position in the rule list._ |
| Name | Identique à label. | _Order 1. The rule's name._ |
| Count of Firings | Le total de fired unit sur les firings liés à le rule. | _Order 15. Rule applications that fired, counting every derivation._ |

<details open>
<summary>Données d'exemple</summary>

| Label | Explanation | Is Enabled | Sort Order | Name ƒ | Count of Firings ƒ |
|---|---|---|---|---|---|
| Inverse | A parent link also gives a child link between the same two people. | false | 1 | Inverse | 0 |
| Symmetric | A friend link or a spouse link is true for both people. | false | 2 | Symmetric | 0 |
| Property chain | A parent of a parent is a grandparent. | false | 3 | Property chain | 0 |

_ƒ signale une colonne calculée._

</details>

| Terme | Description | Commentaire narratif |
|------|-------------|-------------------|
| **Firing Kind** | The ways a rule can apply. Domain and range belong to one rule but are worded apart; 'disjoint' names the rule the disjointness checks answer to. | — |
| Rule | Un attribut défini. | _The rule switch this kind belongs to._ |
| Wording | Un attribut défini. | _How a conclusion card names the rule, as in 'Worked out by the inverse rule'._ |
| Sort Order | Un attribut défini. | _Order in which the page's reasoner tries the kinds for one fact._ |
| Name | Identique à wording. | _Order 1. The kind's wording._ |
| Rule Enabled | Vrai lorsque le rule lié est enabled. | _Order 1. Whether the owning rule is switched on._ |

<details open>
<summary>Données d'exemple</summary>

| Wording | Sort Order | Name ƒ | Rule Enabled ƒ |
|---|---|---|---|
| inverse rule | 1 | inverse rule | false |
| symmetric rule | 2 | symmetric rule | false |
| property chain rule | 3 | property chain rule | false |

_ƒ signale une colonne calculée._

</details>

| Terme | Description | Commentaire narratif |
|------|-------------|-------------------|
| **Thing** | Every individual and class the page can draw. | — |
| Label | Un attribut défini. | _The name the page shows._ |
| Kind | Un attribut défini. | _'individual' or 'class'._ |
| Sort Order | Un attribut défini. | _Position in the cast._ |
| Portrait | Un attribut défini. | _The portrait drawn for this thing; set when its name matches a portrait alias._ |
| Portrait Name | Le display name du portrait lié du thing. | _Order 1. The name on the portrait's Fandom page._ |
| Portrait Page | Repris du portrait lié. | _Order 1. The Fandom page credited for the portrait._ |
| Portrait Image | Repris du portrait lié. | _Order 1. The original Fandom image._ |
| Portrait Crop | Repris du portrait lié. | _Order 1. The portrait's crop as an SVG viewBox._ |
| Portrait Width | Repris du portrait lié. | _Order 1. The original image's width in pixels._ |
| Portrait Height | Repris du portrait lié. | _Order 1. The original image's height in pixels._ |
| Name | Identique à label. | _Order 1. The thing's name._ |
| Node Key | Identique à thing ID. | _Order 1. The key the graph draws this thing under._ |
| Count of Children | Le total de parent unit sur les statements liés à le thing. | _Order 17. People this thing is a parent of, stated or concluded._ |
| Count of Grandchildren | Le total de grandparent unit sur les statements liés à le thing. | _Order 17. People this thing is a grandparent of._ |
| Count As Place | Le total de place unit sur les statements liés à le thing. | _Order 17. Holding attends facts whose object is this thing._ |
| Count As Subject | Le total de holds unit sur les statements liés à le thing. | _Order 17. Holding facts about this thing._ |
| Count As Object | Le total de holds unit sur les statements liés à le thing. | _Order 17. Holding facts that point at this thing._ |
| Is in Graph | Vrai lorsque au moins une des conditions suivantes est vraie : le count as subject est supérieur à 0 ou le count as object est supérieur à 0. | _Order 18. True when the graph draws this thing._ |
| Node Kind | Déterminé par priorité : une chaîne vide si le kind est vide; « class » si le kind est « class »; « place » si le count as place est supérieur à 0 ; dans tous les autres cas, « person ». | _Order 18. How the graph draws the thing: a class, a place someone attends, or a person._ |

<details open>
<summary>Données d'exemple</summary>

| Label | Kind | Sort Order | Portrait Name ƒ | Portrait Page ƒ | Portrait Image ƒ | Portrait Crop ƒ | Portrait Width ƒ | Portrait Height ƒ | Name ƒ | Node Key ƒ | Count of Children ƒ | Count of Grandchildren ƒ | Count As Place ƒ | Count As Subject ƒ | Count As Object ƒ | Is in Graph ƒ | Node Kind ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| Marvin | individual | 1 | Marvin Marsh | https://southpark.fandom.com/wiki/Marvin_Marsh | https://static.wikia.nocookie.net/southpark/images/d/db/Marsh-family-grandpa-marsh-w-purple-blanket.png/revision/latest | 48 -5 280 280 | 375 | 536 | Marvin | Marvin | 1 | 0 | 0 | 1 | 0 | true | person |
| Randy | individual | 2 | Randy Marsh | https://southpark.fandom.com/wiki/Randy_Marsh | https://static.wikia.nocookie.net/southparkfanon/images/d/d6/Randy_Marsh.png/revision/latest | 0 -10 428 428 | 428 | 1057 | Randy | Randy | 2 | 0 | 0 | 3 | 1 | true | person |
| Sharon | individual | 3 | Sharon Marsh | https://southpark.fandom.com/wiki/Sharon_Marsh | https://static.wikia.nocookie.net/southpark/images/9/99/Sharon_Marsh.png/revision/latest | 20 -10 390 390 | 439 | 1049 | Sharon | Sharon | 0 | 0 | 0 | 0 | 1 | true | person |

_ƒ signale une colonne calculée._

</details>

| Terme | Description | Commentaire narratif |
|------|-------------|-------------------|
| **Portrait** | The page's Fandom portraits, including those for characters a reader may add. | — |
| Display Name | Un attribut défini. | _The name on the portrait's Fandom page._ |
| Page | Un attribut défini. | _The Fandom page credited for the portrait._ |
| Image | Un attribut défini. | _The original Fandom image._ |
| Crop | Un attribut défini. | _The crop as an SVG viewBox: x y width height._ |
| Width | Un attribut défini. | _The original image's width in pixels._ |
| Height | Un attribut défini. | _The original image's height in pixels._ |
| Sort Order | Un attribut défini. | _Position in the credits._ |
| Name | Identique à display name. | _Order 1. The portrait's name._ |

<details open>
<summary>Données d'exemple</summary>

| Display Name | Page | Image | Crop | Width | Height | Sort Order | Name ƒ |
|---|---|---|---|---|---|---|---|
| Marvin Marsh | https://southpark.fandom.com/wiki/Marvin_Marsh | https://static.wikia.nocookie.net/southpark/images/d/db/Marsh-family-grandpa-marsh-w-purple-blanket.png/revision/latest | 48 -5 280 280 | 375 | 536 | 1 | Marvin Marsh |
| Randy Marsh | https://southpark.fandom.com/wiki/Randy_Marsh | https://static.wikia.nocookie.net/southparkfanon/images/d/d6/Randy_Marsh.png/revision/latest | 0 -10 428 428 | 428 | 1057 | 2 | Randy Marsh |
| Sharon Marsh | https://southpark.fandom.com/wiki/Sharon_Marsh | https://static.wikia.nocookie.net/southpark/images/9/99/Sharon_Marsh.png/revision/latest | 20 -10 390 390 | 439 | 1049 | 3 | Sharon Marsh |

_ƒ signale une colonne calculée._

</details>

| Terme | Description | Commentaire narratif |
|------|-------------|-------------------|
| **Portrait Aliase** | The names a portrait answers to. A new thing whose name matches an alias gets that portrait. | — |
| Portrait | Un attribut défini. | _The portrait this name selects._ |
| Name | Identique à portrait alias ID. | _Order 1. The alias._ |

<details open>
<summary>Données d'exemple</summary>

| Name ƒ |
|---|
| marvin |
| marvin marsh |
| grandpa marsh |

_ƒ signale une colonne calculée._

</details>

| Terme | Description | Commentaire narratif |
|------|-------------|-------------------|
| **Property** | The ten properties, with what the symmetric, domain and range rules read. Axioms that relate one property to another (inverse, chain, disjoint) are rows of PropertyAxioms. | — |
| Label | Un attribut défini. | _The property's wording on the page._ |
| Property Kind | Un attribut défini. | _'object' links two things, 'datatype' links a thing to a value, 'type' states class membership._ |
| Datatype | Un attribut défini. | _The value's XML Schema datatype for a datatype property; blank otherwise._ |
| Is User Addable | Vrai lorsque une chaîne vide. | _Offered in the page's Add fact form._ |
| Has One Current Value | Vrai lorsque une chaîne vide. | _The page keeps one current value per person: switching a value on replaces the earlier one._ |
| Is Symmetric | Vrai lorsque une chaîne vide. | _The symmetric rule makes this link true in both directions._ |
| Domain Class | Un attribut défini. | _The class the domain rule gives this property's subject._ |
| Range Class | Un attribut défini. | _The class the range rule gives this property's object._ |
| Sort Order | Un attribut défini. | _Position in the Add fact form's property list._ |
| Value Node Label | Un attribut défini. | _The small label under a value node for this property: 'role value' or 'number value'._ |
| Current Value Wording | Un attribut défini. | _How the Add fact form names a replaced value, as in 'Stan now has grade level 5'._ |
| Input Name | Un attribut défini. | _How the integer check names this property's value, as in 'for grade level'._ |
| Record Kind | Un attribut défini. | _The word the current-record summary uses for this property's values: role, grade or school._ |
| Record Order | Un attribut défini. | _Position of this property in the current-record summary._ |
| Name | Identique à label. | _Order 1. The property's wording._ |

<details open>
<summary>Données d'exemple</summary>

| Label | Property Kind | Datatype | Is User Addable | Has One Current Value | Is Symmetric | Sort Order | Value Node Label | Current Value Wording | Input Name | Record Kind | Record Order | Name ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|
| is parent of | object | — | true | false | false | 1 | — | — | — | — | — | is parent of |
| is child of | object | — | true | false | false | 2 | — | — | — | — | — | is child of |
| is spouse of | object | — | true | false | true | 3 | — | — | — | — | — | is spouse of |

_ƒ signale une colonne calculée._

</details>

| Terme | Description | Commentaire narratif |
|------|-------------|-------------------|
| **Property Axiom** | Axioms that relate one property to others, as OWL states them: an inverse, a two-link chain, or a disjoint pair. The candidate generator reads these rows. | — |
| Kind | Un attribut défini. | _inverse, chain or disjoint._ |
| Property | Un attribut défini. | _The property the axiom is about: the one inverted, the one a chain derives, or the first of a disjoint pair._ |
| First | Un attribut défini. | _The inverse property, the chain's first link, or the other property of a disjoint pair._ |
| Second | Un attribut défini. | _The chain's second link._ |
| Name | Identique à property axiom ID. | _Order 1. The axiom's key._ |

<details open>
<summary>Données d'exemple</summary>

| Name ƒ |
|---|
| inverse:isParentOf |
| disjoint:isParentOf~isSpouseOf |
| inverse:isChildOf |

_ƒ signale une colonne calculée._

</details>

| Terme | Description | Commentaire narratif |
|------|-------------|-------------------|
| **Statement** | Every fact the page can state or conclude about the cast. A listed fact appears in the page's fact list; the rest are candidates that only a rule can make hold. | — |
| Graph | Un attribut défini. | _The graph the fact belongs to._ |
| Subject | Un attribut défini. | _What the fact describes._ |
| Property | Un attribut défini. | _How the subject connects to the object._ |
| Object Thing | Un attribut défini. | _The related thing, for an object or type property._ |
| Object Value | Un attribut défini. | _The literal value, for a datatype property._ |
| Is Listed | Vrai lorsque une chaîne vide. | _Shown in the page's fact list._ |
| Is Built in | Vrai lorsque une chaîne vide. | _One of the page's teaching facts; the reader cannot remove it._ |
| Is Detail | Vrai lorsque une chaîne vide. | _One of Stan's detail facts that Show Stan's details switches on._ |
| Is Stated | Vrai lorsque une chaîne vide. | _The fact's checkbox._ |
| Activation Seq | Un attribut défini. | _When the fact was last switched on, as a running number; decides which value is current._ |
| Sort Order | Un attribut défini. | _Position in the fact list; candidates follow the listed facts._ |
| Subject Label | Repris du subject lié. | _Order 1. The subject's name._ |
| Property Label | Repris du property lié. | _Order 1. The property's wording._ |
| Object Label | Repris du object thing lié. | _Order 1. The object's name, for an object or type property._ |
| Property Datatype | Repris du property lié. | _Order 1. The value's datatype, for a datatype property._ |
| Property Has One Current Value | Vrai lorsque le property lié a un one current value. | _Order 1. Whether the property keeps one current value per person._ |
| Property Domain Class | Repris du property lié. | _Order 1. The property's domain class._ |
| Property Range Class | Repris du property lié. | _Order 1. The property's range class._ |
| Literal Quote | Repris du graph lié. | _Order 1. The graph's literal quote mark._ |
| Literal Type Marker | Repris du graph lié. | _Order 1. The graph's datatype marker._ |
| Built in Source Text | Repris du graph lié. | _Order 1. The graph's source line for a teaching fact._ |
| User Source Text | Repris du graph lié. | _Order 1. The graph's source line for a reader's fact._ |
| Next Fact Sort Order | Repris du graph lié. | _Order 6. List position of the next fact to add._ |
| Is Literal | Vrai lorsque l'object thing est vide. | _Order 1. True when the object is a value rather than a thing._ |
| Object Text | Déterminé par priorité : l'object value si l'object thing est vide ; dans tous les autres cas, l'object label. | _Order 2. The object's name or the value._ |
| Name | Calculé comme le subject label, suivi de une espace, suivi de le property label, suivi de une espace, suivi de l'object text. | _Order 3. The fact in words._ |
| Words | Calculé comme le subject label, suivi de une espace, suivi de le property label, suivi de une espace, suivi de le literal quote, suivi de l'object value, suivi de le literal quote, suivi de le literal type marker, suivi de le property datatype si l'object thing est vide, dans tous les autres cas l'object label. ⚠︎ mécanique <!-- rulespeak:reword --> | _Order 2. The fact as a conclusion card quotes it, with a literal written in Turtle form._ |
| Source | Déterminé par priorité : le built in source text si l'indicateur « built in » est activé ; dans tous les autres cas, l'user source text. | _Order 2. Where a listed fact came from._ |
| Subject Property Key | Calculé comme le subject, suivi de « | », suivi de le property. | _Order 1. Groups the values one subject has for one property._ |
| Latest Sibling Activation Seq | La plus grande valeur de activation seq sur les statements liés à le statement. | _Order 2. The most recent activation among this subject's values for this property._ |
| Is in Effect | Vrai lorsque tout ce qui suit est vrai : l'indicateur « stated » est activé et au moins une des conditions suivantes est vraie : l'indicateur « property has one current value » n'est pas activé ou l'activation seq est le latest sibling activation seq. | _Order 3. Switched on and, for a one-value property, the most recently switched-on value. Drawn as a solid line._ |
| Listed Unit | Déterminé par priorité : 1 si l'indicateur « listed » est activé ; dans tous les autres cas, 0. | _Order 1. 1 when the fact is in the fact list._ |
| In Effect Unit | Déterminé par priorité : 1 si l'indicateur « in effect » est activé ; dans tous les autres cas, 0. | _Order 4. 1 when the fact is in effect._ |
| Listed Off Unit | Déterminé par priorité : 1 si tout ce qui suit est vrai : l'indicateur « listed » est activé et l'indicateur « in effect » n'est pas activé ; dans tous les autres cas, 0. | _Order 4. 1 when the fact is listed but not in effect._ |
| Is Next to Add | Vrai lorsque tout ce qui suit est vrai : le listed off unit est 1 et le sort order est le next fact sort order. | _Order 7. The fact the Add next fact button switches on._ |
| Count of Inverse Firings | Le total de inverse fired unit sur les firings liés à le statement. | _Order 6. Inverse rule applications that conclude this fact._ |
| Count of Symmetric Firings | Le total de symmetric fired unit sur les firings liés à le statement. | _Order 6. Symmetric rule applications that conclude this fact._ |
| Holds Before Chain | Vrai lorsque le IF de le OR de l'is in effect; le count of inverse firings est supérieur à 0; et le count of symmetric firings est supérieur à 0 et vrai. | _Order 7. TRUE when the fact is in effect or the inverse or symmetric rule concludes it. Blank when nothing states or concludes it: not known, never FALSE (open world: removing a statement does not say that its opposite is true)._ |
| Count of Chain Firings | Le total de chain fired unit sur les firings liés à le statement. | _Order 10. Property chain applications that conclude this fact._ |
| Holds Before Typing | Vrai lorsque le IF de le OR de l'holds before chain et le count of chain firings est supérieur à 0 et vrai. | _Order 11. TRUE when the fact holds before the domain and range rules apply. Blank when nothing states or concludes it: not known, never FALSE (open world: removing a statement does not say that its opposite is true)._ |
| Count of Type Firings | Le total de type fired unit sur les firings liés à le statement. | _Order 14. Domain and range applications that conclude this fact._ |
| Holds | Vrai lorsque le IF de le OR de l'holds before typing et le count of type firings est supérieur à 0 et vrai. | _Order 15. TRUE when in effect or concluded by an enabled rule; the graph draws it. Blank when nothing states or concludes it: not known, never FALSE (open world: removing a statement does not say that its opposite is true)._ |
| Is Conclusion | Vrai lorsque le IF de le AND de l'holds et l'indicateur « in effect » n'est pas activé et vrai. | _Order 16. TRUE when the fact holds only because a rule concluded it; drawn dashed. Blank otherwise: a stated fact is not a conclusion, and a fact nothing concludes is not known (never FALSE)._ |
| Holds Unit | Déterminé par priorité : 1 si l'indicateur « holds » est activé ; dans tous les autres cas, 0. | _Order 16. 1 when the fact holds._ |
| Conclusion Unit | Déterminé par priorité : 1 si l'indicateur « conclusion » est activé ; dans tous les autres cas, 0. | _Order 17. 1 when the fact is a conclusion._ |
| Parent Unit | Déterminé par priorité : 1 si tout ce qui suit est vrai : l'indicateur « holds » est activé et le property est « isParentOf » ; dans tous les autres cas, 0. | _Order 16. 1 for a holding parent link._ |
| Grandparent Unit | Déterminé par priorité : 1 si tout ce qui suit est vrai : l'indicateur « holds » est activé et le property est « isGrandparentOf » ; dans tous les autres cas, 0. | _Order 16. 1 for a holding grandparent link._ |
| Place Unit | Déterminé par priorité : 1 si tout ce qui suit est vrai : l'indicateur « holds » est activé et le property est « attends » ; dans tous les autres cas, 0. | _Order 16. 1 for a holding attends link._ |
| First Pre Chain Order Key | La plus petite valeur de pre chain order key sur les firings du statement qui sont pre chain fired unit. | _Order 7. Order key of the first inverse or symmetric application that concludes this fact._ |
| Snap Position | Déterminé par priorité : le sort order si l'indicateur « in effect » est activé; 100000 plus le first pre chain order key si l'indicateur « holds before chain » est activé ; dans tous les autres cas, 0. | _Order 8. Where the page's reasoner meets this fact as a premise: a stated fact at its list position, a fact concluded by the inverse or symmetric rule after every stated fact, in the order it was concluded; 0 for a fact that does not hold yet, so a link to no fact reads the same on every engine._ |
| First Fired Order Key | La plus petite valeur de order key sur les firings du statement qui sont fired unit. | _Order 15. Order key of the first rule application that concludes this fact, in the page reasoner's order._ |
| Count of Chosen Firings | Le total de chosen unit sur les firings liés à le statement. | _Order 20. Rule applications shown as this fact's conclusion card: one for a conclusion, zero otherwise._ |
| Subject Node Key | Identique à subject. | _Order 1. The graph node this fact starts from._ |
| Object Node Key | Déterminé par priorité : « literal| », suivi de l'object value, suivi de « | », suivi de le property datatype si l'object thing est vide ; dans tous les autres cas, l'object thing. | _Order 2. The graph node this fact points at: the thing, or one node per value and datatype._ |
| Subject Node Kind | Repris du subject lié. | _Order 19. How the graph draws the subject._ |
| Object Thing Node Kind | Repris du object thing lié. | _Order 19. How the graph draws the object thing._ |
| Object Node Kind | Déterminé par priorité : « value » si l'object thing est vide ; dans tous les autres cas, l'object thing node kind. | _Order 20. How the graph draws the object: a value, a class, a place or a person._ |
| Object Value Node Label | Repris du property lié. | _Order 1. The small label under a value node._ |
| Count of Violations As First | Le total de violation unit sur les disjoint checks liés à le statement. | _Order 18. Violated disjointness checks naming this fact first._ |
| Count of Violations As Second | Le total de violation unit sur les disjoint checks liés à le statement. | _Order 18. Violated disjointness checks naming this fact second._ |
| Is in Contradiction | Vrai lorsque au moins une des conditions suivantes est vraie : le count of violations as first est supérieur à 0 ou le count of violations as second est supérieur à 0. | _Order 19. Part of a contradiction the disjoint rule found; drawn in the alarm colour._ |

<details open>
<summary>Données d'exemple</summary>

| Object Value | Is Listed | Is Built in | Is Detail | Is Stated | Activation Seq | Sort Order | Subject Label ƒ | Property Label ƒ | Object Label ƒ | Property Datatype ƒ | Property Has One Current Value ƒ | Property Domain Class ƒ | Property Range Class ƒ | Literal Quote ƒ | Literal Type Marker ƒ | Built in Source Text ƒ | User Source Text ƒ | Next Fact Sort Order ƒ | Is Literal ƒ | Object Text ƒ | Name ƒ | Words ƒ | Source ƒ | Subject Property Key ƒ | Latest Sibling Activation Seq ƒ | Is in Effect ƒ | Listed Unit ƒ | In Effect Unit ƒ | Listed Off Unit ƒ | Is Next to Add ƒ | Count of Inverse Firings ƒ | Count of Symmetric Firings ƒ | Holds Before Chain ƒ | Count of Chain Firings ƒ | Holds Before Typing ƒ | Count of Type Firings ƒ | Holds ƒ | Is Conclusion ƒ | Holds Unit ƒ | Conclusion Unit ƒ | Parent Unit ƒ | Grandparent Unit ƒ | Place Unit ƒ | First Pre Chain Order Key ƒ | Snap Position ƒ | First Fired Order Key ƒ | Count of Chosen Firings ƒ | Subject Node Key ƒ | Object Node Key ƒ | Subject Node Kind ƒ | Object Thing Node Kind ƒ | Object Node Kind ƒ | Object Value Node Label ƒ | Count of Violations As First ƒ | Count of Violations As Second ƒ | Is in Contradiction ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| — | true | true | false | true | 1 | 1 | Marvin | is parent of | Randy | — | false | — | — | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | 0 | false | Randy | Marvin is parent of Randy | Marvin is parent of Randy | Stated as a teaching example on this page. | Marvin\|isParentOf | 1 | true | 1 | 1 | 0 | false | 0 | 0 | true | 0 | true | 0 | true | — | 1 | 0 | 1 | 0 | 0 | 0.0 | 1.0 | 0.0 | 0 | Marvin | Randy | person | person | person | — | 0 | 0 | false |
| — | false | false | false | false | 0 | 100001 | Marvin | is parent of | Sharon | — | false | — | — | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | 0 | false | Sharon | Marvin is parent of Sharon | Marvin is parent of Sharon | Stated by you on this page. | Marvin\|isParentOf | 1 | false | 0 | 0 | 0 | false | 0 | 0 | — | 0 | — | 0 | — | — | 0 | 0 | 0 | 0 | 0 | 0.0 | 0.0 | 0.0 | 0 | Marvin | Sharon | person | person | person | — | 0 | 0 | false |
| — | false | false | false | false | 0 | 100002 | Marvin | is parent of | Stan | — | false | — | — | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | 0 | false | Stan | Marvin is parent of Stan | Marvin is parent of Stan | Stated by you on this page. | Marvin\|isParentOf | 1 | false | 0 | 0 | 0 | false | 0 | 0 | — | 0 | — | 0 | — | — | 0 | 0 | 0 | 0 | 0 | 0.0 | 0.0 | 0.0 | 0 | Marvin | Stan | person | person | person | — | 0 | 0 | false |

_ƒ signale une colonne calculée._

</details>

| Terme | Description | Commentaire narratif |
|------|-------------|-------------------|
| **Firing** | Every way an enabled rule could conclude a candidate fact from the facts that hold. | — |
| Kind | Un attribut défini. | _How the rule applies._ |
| Rule | Un attribut défini. | _The rule switch._ |
| Target | Un attribut défini. | _The fact this application concludes._ |
| First | Un attribut défini. | _The premise._ |
| Second | Un attribut défini. | _The second premise, for the property chain._ |
| Rule Enabled | Vrai lorsque le rule lié est enabled. | _Order 1. Whether the rule is switched on._ |
| Kind Wording | Repris du kind lié. | _Order 1. How the card names the rule._ |
| Kind Sort Order | Repris du kind lié. | _Order 1. The kind's place in the reasoner's order._ |
| Target Name | Repris du target lié. | _Order 4. The concluded fact in words._ |
| Target Subject Label | Repris du target lié. | _Order 2. The concluded fact's subject._ |
| Target Property Label | Repris du target lié. | _Order 2. The concluded fact's property._ |
| Target Object Text | Repris du target lié. | _Order 3. The concluded fact's object._ |
| Target is Conclusion | Vrai lorsque le target lié est une conclusion. | _Order 17. Whether the concluded fact is new._ |
| Target Subject Node Key | Repris du target lié. | _Order 2. The concluded fact's subject node._ |
| Target Object Node Key | Repris du target lié. | _Order 3. The concluded fact's object node._ |
| Target First Fired Order Key | Repris du target lié. | _Order 16. The order key of the concluded fact's first application._ |
| First Words | Repris du first lié. | _Order 3. The premise as the card quotes it._ |
| Second Words | Repris du second lié. | _Order 3. The second premise as the card quotes it._ |
| First is in Effect | Vrai lorsque le first lié est in effect. | _Order 4. Whether the premise is in effect._ |
| First Holds Before Chain | Vrai lorsque le first lié est holds before chain. | _Order 8. Whether the premise holds before the chain._ |
| Second Holds Before Chain | Vrai lorsque le second lié est holds before chain. | _Order 8. Whether the second premise holds before the chain._ |
| First Holds Before Typing | Vrai lorsque le first lié est holds before typing. | _Order 12. Whether the premise holds before typing._ |
| First Property | Repris du first lié. | _Order 1. The premise's property key, as the card's axiom names it._ |
| First Property Domain Class | Repris du first lié. | _Order 2. The premise property's domain class._ |
| First Property Range Class | Repris du first lié. | _Order 2. The premise property's range class._ |
| First Sort Order | Repris du first lié. | _Order 1. The premise's list position._ |
| Second is in Effect | Vrai lorsque le second lié est in effect. | _Order 4. Whether the second premise is in effect._ |
| First Snap Position | Repris du first lié. | _Order 9. Where the reasoner meets the premise._ |
| Second Snap Position | Repris du second lié. | _Order 9. Where the reasoner meets the second premise._ |
| Inverse Fired Unit | Déterminé par priorité : 1 si tout ce qui suit est vrai : l'indicateur « rule enabled » est activé; le kind est « inverse »; et l'indicateur « first is in effect » est activé ; dans tous les autres cas, 0. | _Order 5. 1 when the inverse rule fires here._ |
| Symmetric Fired Unit | Déterminé par priorité : 1 si tout ce qui suit est vrai : l'indicateur « rule enabled » est activé; le kind est « symmetric »; et l'indicateur « first is in effect » est activé ; dans tous les autres cas, 0. | _Order 5. 1 when the symmetric rule fires here._ |
| Chain Fired Unit | Déterminé par priorité : 1 si tout ce qui suit est vrai : l'indicateur « rule enabled » est activé; le kind est « chain »; l'indicateur « first holds before chain » est activé; et l'indicateur « second holds before chain » est activé ; dans tous les autres cas, 0. | _Order 9. 1 when the property chain fires here._ |
| Type Fired Unit | Déterminé par priorité : 1 si tout ce qui suit est vrai : l'indicateur « rule enabled » est activé; au moins une des conditions suivantes est vraie : le kind est « domain » ou le kind est « range »; et l'indicateur « first holds before typing » est activé ; dans tous les autres cas, 0. | _Order 13. 1 when the domain or range rule fires here._ |
| Fired Unit | Calculé comme l'inverse fired unit plus le symmetric fired unit plus le chain fired unit plus le type fired unit. | _Order 14. 1 when this application fires._ |
| Pre Chain Fired Unit | Calculé comme l'inverse fired unit plus le symmetric fired unit. | _Order 6. 1 when this is an inverse or symmetric application that fires._ |
| Pre Chain Order Key | Calculé comme le first sort order multiplié par 10 plus le kind sort order. | _Order 2. For an inverse or symmetric application: its place in the reasoner's first round._ |
| Round | Déterminé par priorité : 2 si tout ce qui suit est vrai : le kind est « chain » et il n'est pas vrai que tout ce qui suit est vrai : l'indicateur « first is in effect » est activé et l'indicateur « second is in effect » est activé ; dans tous les autres cas, 1. | _Order 5. The reasoner's round: 1 when every premise is a stated fact, 2 when the property chain reads a concluded fact._ |
| Order Key | Calculé comme le round multiplié par 1000000000000000 plus la first snap position multiplié par 100000000 plus le kind sort order multiplié par 10000000 plus 0 si le second est vide, dans tous les autres cas la second snap position. | _Order 10. The page reasoner's order: by round, then premise, then rule kind, then second premise._ |
| Is Chosen | Vrai lorsque tout ce qui suit est vrai : le fired unit est 1; l'indicateur « target is conclusion » est activé; et l'order key est le target first fired order key. | _Order 18. The application the conclusions panel shows for its fact: the first one in the reasoner's order._ |
| Chosen Unit | Déterminé par priorité : 1 si l'indicateur « chosen » est activé ; dans tous les autres cas, 0. | _Order 19. 1 when this application is shown._ |
| Card Text | Déterminé par priorité : « Worked out by the », suivi de le kind wording, suivi de « from “ », suivi de le first words, suivi de « ” », suivi de « and “ », suivi de le second words, suivi de « ” » si le kind est « chain », dans tous les autres cas « and “ », suivi de le first property, suivi de « has domain », suivi de le first property domain class, suivi de « ” » si le kind est « domain », dans tous les autres cas « and “ », suivi de le first property, suivi de « has range », suivi de le first property range class, suivi de « ” », suivi de un point si l'indicateur « chosen » est activé ; dans tous les autres cas, une chaîne vide. ⚠︎ mécanique <!-- rulespeak:reword --> | _Order 19. The conclusion card's provenance line, in the page's wording._ |
| Name | Identique à firing ID. | _Order 1. The application's key._ |

<details open>
<summary>Données d'exemple</summary>

| Rule Enabled ƒ | Kind Wording ƒ | Kind Sort Order ƒ | Target Name ƒ | Target Subject Label ƒ | Target Property Label ƒ | Target Object Text ƒ | Target is Conclusion ƒ | Target Subject Node Key ƒ | Target Object Node Key ƒ | Target First Fired Order Key ƒ | First Words ƒ | Second Words ƒ | First is in Effect ƒ | First Holds Before Chain ƒ | Second Holds Before Chain ƒ | First Holds Before Typing ƒ | First Property ƒ | First Property Domain Class ƒ | First Property Range Class ƒ | First Sort Order ƒ | Second is in Effect ƒ | First Snap Position ƒ | Second Snap Position ƒ | Inverse Fired Unit ƒ | Symmetric Fired Unit ƒ | Chain Fired Unit ƒ | Type Fired Unit ƒ | Fired Unit ƒ | Pre Chain Fired Unit ƒ | Pre Chain Order Key ƒ | Round ƒ | Order Key ƒ | Is Chosen ƒ | Chosen Unit ƒ | Card Text ƒ | Name ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| false | inverse rule | 1 | Randy is child of Marvin | Randy | is child of | Marvin | — | Randy | Marvin | 0.0 | Marvin is parent of Randy |    | true | true | — | true | isParentOf | — | — | 1 | false | 1.0 | 0.0 | 0 | 0 | 0 | 0 | 0 | 0 | 11.0 | 1 | 1000000110000000.0 | false | 0 | — | inverse:Marvin\|isParentOf\|Randy>Randy\|isChildOf\|Marvin |
| false | inverse rule | 1 | Sharon is child of Marvin | Sharon | is child of | Marvin | — | Sharon | Marvin | 0.0 | Marvin is parent of Sharon |    | false | — | — | — | isParentOf | — | — | 100001 | false | 0.0 | 0.0 | 0 | 0 | 0 | 0 | 0 | 0 | 1000011.0 | 1 | 1000000010000000.0 | false | 0 | — | inverse:Marvin\|isParentOf\|Sharon>Sharon\|isChildOf\|Marvin |
| false | inverse rule | 1 | Stan is child of Marvin | Stan | is child of | Marvin | — | Stan | Marvin | 0.0 | Marvin is parent of Stan |    | false | — | — | — | isParentOf | — | — | 100002 | false | 0.0 | 0.0 | 0 | 0 | 0 | 0 | 0 | 0 | 1000021.0 | 1 | 1000000010000000.0 | false | 0 | — | inverse:Marvin\|isParentOf\|Stan>Stan\|isChildOf\|Marvin |

_ƒ signale une colonne calculée._

</details>

| Terme | Description | Commentaire narratif |
|------|-------------|-------------------|
| **Disjoint Check** | Every pair the disjointness rule watches: the same two people linked by two disjoint properties. | — |
| Graph | Un attribut défini. | _The graph._ |
| Rule | Un attribut défini. | _The rule switch._ |
| First | Un attribut défini. | _The first link._ |
| Second | Un attribut défini. | _The disjoint link between the same pair, in the same direction._ |
| Rule Enabled | Vrai lorsque le rule lié est enabled. | _Order 1. Whether the disjointness rule is on._ |
| First Holds | Vrai lorsque le first lié est holds. | _Order 16. Whether the first link holds._ |
| Second Holds | Vrai lorsque le second lié est holds. | _Order 16. Whether the second link holds._ |
| Subject Label | Repris du first lié. | _Order 2. The person recorded with both links._ |
| Object Label | Repris du first lié. | _Order 2. The other person._ |
| Violation Unit | Déterminé par priorité : 1 si tout ce qui suit est vrai : l'indicateur « rule enabled » est activé; l'indicateur « first holds » est activé; et l'indicateur « second holds » est activé ; dans tous les autres cas, 0. | _Order 17. 1 when both links hold._ |
| Alarm Text | Déterminé par priorité : « Contradiction found. », suivi de le subject label, suivi de « is recorded as both parent and spouse of », suivi de l'object label, suivi de «. Check those facts and the selected disjoint rule. » si le violation unit est 1 ; dans tous les autres cas, une chaîne vide. | _Order 18. The page's contradiction alert, blank when there is none._ |
| Name | Identique à disjoint check ID. | _Order 1. The check's key._ |

<details open>
<summary>Données d'exemple</summary>

| Rule Enabled ƒ | First Holds ƒ | Second Holds ƒ | Subject Label ƒ | Object Label ƒ | Violation Unit ƒ | Alarm Text ƒ | Name ƒ |
|---|---|---|---|---|---|---|---|
| false | true | — | Marvin | Randy | 0 | — | Marvin\|isParentOf\|Randy~isSpouseOf |
| false | — | — | Marvin | Sharon | 0 | — | Marvin\|isParentOf\|Sharon~isSpouseOf |
| false | — | — | Marvin | Stan | 0 | — | Marvin\|isParentOf\|Stan~isSpouseOf |

_ƒ signale une colonne calculée._

</details>

| Terme | Description | Commentaire narratif |
|------|-------------|-------------------|
| **Scenario** | The page's buttons. Each one is a fact baseline, explicit fact changes and rule settings, applied in that order. | — |
| Graph | Un attribut défini. | _The graph._ |
| Label | Un attribut défini. | _The button's text._ |
| Note | Un attribut défini. | _The note shown under the buttons after this one is pressed; blank clears it._ |
| Sort Order | Un attribut défini. | _Position among the buttons._ |
| Button Group | Un attribut défini. | _'stan' for the two imagined changes, 'facts' for the buttons under the fact list._ |
| Is Primary | Vrai lorsque une chaîne vide. | _Drawn as the filled button._ |
| Fact Baseline | Un attribut défini. | _What happens to every listed fact first: keep, built-in (teaching facts on, others off), built-in-detail, all-off, activate-all (each switched on in list order), or next (the next fact on)._ |
| Layout | Un attribut défini. | _The node layout the graph returns to; blank keeps the current positions._ |
| Clears Selection | Vrai lorsque une chaîne vide. | _Clears the selected node or line._ |
| Clears Form Message | Vrai lorsque une chaîne vide. | _Clears the Add fact form's message._ |
| Name | Identique à label. | _Order 1. The button's text._ |
| Graph Has Facts to Add | Vrai lorsque le graph lié a un facts to add. | _Order 7. Whether a listed fact can still be added._ |
| Is Available | Vrai lorsque au moins une des conditions suivantes est vraie : le fact baseline n'est pas « next » ou l'indicateur « graph has facts to add » est activé. | _Order 8. False only for Add next fact once every listed fact is in effect._ |

<details open>
<summary>Données d'exemple</summary>

| Label | Note | Sort Order | Button Group | Is Primary | Fact Baseline | Layout | Clears Selection | Clears Form Message | Name ƒ | Graph Has Facts to Add ƒ | Is Available ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|
| Move Stan to grade 5 | This imagined change updates Stan’s grade to 5. His identifier, Person statement and family links stay the same. His other selected details are unchanged. | 1 | stan | false | keep | — | true | true | Move Stan to grade 5 | false | true |
| Stan leaves school | This imagined change clears Stan’s current school details. His identifier, Person statement and family links remain. Removing a current fact does not say that its opposite is true; school history needs dates. | 2 | stan | false | keep | — | true | true | Stan leaves school | false | true |
| Add next fact | — | 3 | facts | true | next | — | false | false | Add next fact | false | false |

_ƒ signale une colonne calculée._

</details>

| Terme | Description | Commentaire narratif |
|------|-------------|-------------------|
| **Scenario Fact Change** | Fact changes a button makes after its baseline. A blank object key matches every listed value of the property. | — |
| Scenario | Un attribut défini. | _The button._ |
| Subject | Un attribut défini. | _The subject whose facts change._ |
| Property | Un attribut défini. | _The property whose facts change._ |
| Object Key | Un attribut défini. | _The object thing's key or the literal value; blank for every listed value._ |
| Action | Un attribut défini. | _'on' switches the fact on as the newest value; 'off' switches it off._ |
| Adds to List | Vrai lorsque une chaîne vide. | _Adds the fact to the fact list first when it is not listed._ |
| Sort Order | Un attribut défini. | _Order of the change within the button._ |
| Name | Identique à scenario fact change ID. | _Order 1. The change's key._ |

<details open>
<summary>Données d'exemple</summary>

| Object Key | Action | Adds to List | Sort Order | Name ƒ |
|---|---|---|---|---|
| Person | on | true | 1 | stan-grade-five:Stan\|isA\|Person |
| 5 | on | true | 2 | stan-grade-five:Stan\|hasGradeLevel\|5 |
| Person | on | true | 3 | stan-leaves-school:Stan\|isA\|Person |

_ƒ signale une colonne calculée._

</details>

| Terme | Description | Commentaire narratif |
|------|-------------|-------------------|
| **Scenario Rule Setting** | Rule switches a button sets. | — |
| Scenario | Un attribut défini. | _The button._ |
| Rule | Un attribut défini. | _The rule switch._ |
| Is Enabled | Vrai lorsque une chaîne vide. | _The state the button puts the switch in._ |
| Name | Identique à scenario rule setting ID. | _Order 1. The setting's key._ |

<details open>
<summary>Données d'exemple</summary>

| Is Enabled | Name ƒ |
|---|---|
| false | reset-original:inverse |
| false | reset-original:symmetric |
| false | reset-original:chain |

_ƒ signale une colonne calculée._

</details>

| Terme | Description | Commentaire narratif |
|------|-------------|-------------------|
| **Node Layout** | Node positions in the graph's 380 by 480 drawing, one set per layout. | — |
| Layout | Un attribut défini. | _The layout: 'default' or 'stan-details'._ |
| Node Key | Un attribut défini. | _The node: a thing's key, or literal\|value\|datatype._ |
| X | Un attribut défini. | _Horizontal position._ |
| Y | Un attribut défini. | _Vertical position._ |
| Name | Identique à node layout ID. | _Order 1. The position's key._ |

<details open>
<summary>Données d'exemple</summary>

| Layout | Node Key | X | Y | Name ƒ |
|---|---|---|---|---|
| default | Marvin | 70 | 45 | default:Marvin |
| default | Randy | 195 | 125 | default:Randy |
| default | Sharon | 318 | 55 | default:Sharon |

_ƒ signale une colonne calculée._

</details>

| Terme | Description | Commentaire narratif |
|------|-------------|-------------------|
| **Layout Slot** | Places for nodes no layout positions, used in turn. | — |
| Sort Order | Un attribut défini. | _Order in which slots are used._ |
| X | Un attribut défini. | _Horizontal position._ |
| Y | Un attribut défini. | _Vertical position._ |
| Name | Identique à layout slot ID. | _Order 1. The slot's key._ |

<details open>
<summary>Données d'exemple</summary>

| Sort Order | X | Y | Name ƒ |
|---|---|---|---|
| 1 | 330 | 135 | slot-1 |
| 2 | 45 | 120 | slot-2 |
| 3 | 330 | 400 | slot-3 |

_ƒ signale une colonne calculée._

</details>

| Terme | Description | Commentaire narratif |
|------|-------------|-------------------|
| **Page Text** | The page's wording, one row per string. {name}-style placeholders are filled by the page. | — |
| Text | Un attribut défini. | _The string._ |
| Name | Identique à page text ID. | _Order 1. The string's key._ |

<details open>
<summary>Données d'exemple</summary>

| Text | Name ƒ |
|---|---|
| South Park Ontology Explainer, on one rulebook | page-title |
| A rulebook rebuild of the explainer by | byline-prefix |
| Jessica Talisman | byline-author |

_ƒ signale une colonne calculée._

</details>

| Terme | Description | Commentaire narratif |
|------|-------------|-------------------|
| **Engine** | The execution engines the page can be computed by. Each is a build output of this rulebook. | — |
| Label | Un attribut défini. | _The engine's name in the switcher._ |
| Substrate | Un attribut défini. | _The conformance harness's name for this substrate._ |
| Is Default | Vrai lorsque une chaîne vide. | _The engine a new visitor starts on._ |
| Description | Un attribut défini. | _What computes the page on this engine._ |
| Sort Order | Un attribut défini. | _Position in the switcher._ |
| Name | Identique à label. | _Order 1. The engine's name._ |

<details open>
<summary>Données d'exemple</summary>

| Label | Substrate | Is Default | Description | Sort Order | Name ƒ |
|---|---|---|---|---|---|
| Postgres views | effortless-postgres | true | Every value is a column of a generated view; the database computes it when the page reads it. | 1 | Postgres views |
| TypeScript | effortless-typescript | false | The generated TypeScript computes every field from the same rows, with no database. | 2 | TypeScript |
| OWL and SHACL rules | effortless-owl | false | The generated ontology and SHACL rules, run over the same rows by a SPARQL engine. | 3 | OWL and SHACL rules |

_ƒ signale une colonne calculée._

</details>

| Terme | Description | Commentaire narratif |
|------|-------------|-------------------|
| **Rulebook Tool** | What each rulebook tool adds to the original explainer; the tools panel lists them. | — |
| Label | Un attribut défini. | _The tool's name._ |
| Adds | Un attribut défini. | _What it adds to the original explainer._ |
| Href | Un attribut défini. | _Where its link goes; 'editor' means the running rulebook editor._ |
| Link Text | Un attribut défini. | _The link's text._ |
| Sort Order | Un attribut défini. | _Position in the panel._ |
| Name | Identique à label. | _Order 1. The tool's name._ |

<details open>
<summary>Données d'exemple</summary>

| Label | Adds | Href | Link Text | Sort Order | Name ƒ |
|---|---|---|---|---|---|
| Interchangeable engines | The same page, computed by Postgres views, generated TypeScript or the generated OWL and SHACL rules. Switch with the control above; nothing else changes. | — | — | 1 | Interchangeable engines |
| Conformance | Every engine's answers for the current facts, compared value by value with the conformance harness's own rule for when two values are equal. | — | — | 2 | Conformance |
| Explainer | Hover a dotted value for its rule in plain words and its witness: the values it came from, down to the stated facts. | /rulebook-explainer-dag/pages/index.html | Open the explainer | 3 | Explainer |

_ƒ signale une colonne calculée._

</details>

## 2 Types de faits

- un **firing kind** référence exactement un **rule**
- un **thing** peut référencer un **portrait**
- un **portrait aliase** référence exactement un **portrait**
- un **property** peut référencer un **thing**
- un **property axiom** référence exactement un **firing kind**
- un **property axiom** référence exactement un **property**
- un **statement** référence exactement un **graph**
- un **statement** référence exactement un **thing**
- un **statement** référence exactement un **property**
- un **firing** référence exactement un **firing kind**
- un **firing** référence exactement un **rule**
- un **firing** référence exactement un **statement**
- un **disjoint check** référence exactement un **graph**
- un **disjoint check** référence exactement un **rule**
- un **disjoint check** référence exactement un **statement**
- un **scenario** référence exactement un **graph**
- un **scenario fact change** référence exactement un **scenario**
- un **scenario fact change** référence exactement un **thing**
- un **scenario fact change** référence exactement un **property**
- un **scenario rule setting** référence exactement un **scenario**
- un **scenario rule setting** référence exactement un **rule**

## 3 Règles opératives

_Les règles opératives énoncent ce que le métier **oblige**, **interdit** ou
conseille (**devrait**). Les règles structurelles proviennent des champs obligatoires et
des clés étrangères ; les règles sémantiques proviennent de la table Constraints, chacune
s'appuyant sur un booléen déjà calculé (référencé DR-N dans les règles définitionnelles)._

### Contraintes structurelles (issues du schéma)

- Un graph **doit** avoir un label; un literal quote; un literal type marker; un built in source text; un user source text; et un no conclusions text.
- Un rule **doit** avoir un label; une explanation; et un sort order, et consigner si il est enabled.
- Un firing kind **doit** référencer exactement un rule.
- Un firing kind **doit** avoir un wording et un sort order.
- Un thing **doit** avoir un label; un kind; et un sort order.
- Un portrait **doit** avoir un display name; un page; un image; un crop; un width; un height; et un sort order.
- Un portrait aliase **doit** référencer exactement un portrait.
- Un property **doit** avoir un label; un property kind; et un sort order, et consigner si il est user addable; si il a un one current value; et si il est symmetric.
- Un property axiom **doit** référencer exactement un firing kind comme son kind.
- Un property axiom **doit** référencer exactement un property.
- Un property axiom **doit** référencer exactement un property comme son first.
- Un statement **doit** référencer exactement un graph.
- Un statement **doit** référencer exactement un thing comme son subject.
- Un statement **doit** référencer exactement un property.
- Un statement **doit** avoir un activation seq et un sort order, et consigner si il est listed; si il est un built in; si il est un detail; et si il est stated.
- Un firing **doit** référencer exactement un firing kind comme son kind.
- Un firing **doit** référencer exactement un rule.
- Un firing **doit** référencer exactement un statement comme son target.
- Un firing **doit** référencer exactement un statement comme son first.
- Un disjoint check **doit** référencer exactement un graph.
- Un disjoint check **doit** référencer exactement un rule.
- Un disjoint check **doit** référencer exactement un statement comme son first.
- Un disjoint check **doit** référencer exactement un statement comme son second.
- Un scenario **doit** référencer exactement un graph.
- Un scenario **doit** avoir un label; un sort order; un button group; et un fact baseline, et consigner si il est primary; si il est clears selection; et si il est clears form message.
- Un scenario fact change **doit** référencer exactement un scenario.
- Un scenario fact change **doit** référencer exactement un thing comme son subject.
- Un scenario fact change **doit** référencer exactement un property.
- Un scenario fact change **doit** avoir une action et un sort order, et consigner si il est adds to list.
- Un scenario rule setting **doit** référencer exactement un scenario.
- Un scenario rule setting **doit** référencer exactement un rule.
- Un scenario rule setting **doit** consigner si il est enabled.
- Un node layout **doit** avoir un layout; un node key; un x; et un y.
- Un layout slot **doit** avoir un sort order; un x; et un y.
- Un page text **doit** avoir un text.
- Un engine **doit** avoir un label; un substrate; une description; et un sort order, et consigner si il est un default.
- Un rulebook tool **doit** avoir un label; un adds; et un sort order.

## 4 Règles définitionnelles

_Tous les énoncés expriment des vérités du domaine métier ; ce ne sont ni des procédures
ni des impératifs. « si et seulement si » est évité au profit de « seulement si », afin
qu'une nécessité unidirectionnelle ne soit pas confondue avec une équivalence. Une puce
**⚠︎ mécanique** signale une règle dont la formulation déterministe est fidèle mais
lourde — un signal pour une reformulation optionnelle, pas un défaut._

| ID | Règle déclarative |
|----|------------------|
| **DR-1 Name** | Un graph : name est identique à label. |
| **DR-2 Count of Stated Facts** | Un graph : count of stated facts est le total de in effect unit sur les statements liés à le graph. |
| **DR-3 Count of Conclusions** | Un graph : count of conclusions est le total de conclusion unit sur les statements liés à le graph. |
| **DR-4 Count of Violations** | Un graph : count of violations est le total de violation unit sur les disjoint checks liés à le graph. |
| **DR-5 Is Consistent** | Un graph est considéré comme consistent si le count of violations est 0. |
| **DR-6 Conclusions Message** | Le champ « conclusions message » du graph est déterminé selon la priorité suivante :<br>1. le no conclusions text, si le count of conclusions est 0 ;<br>2. dans tous les autres cas, une chaîne vide. |
| **DR-7 Count of Facts to Add** | Un graph : count of facts to add est le total de listed off unit sur les statements liés à le graph. |
| **DR-8 Has Facts to Add** | Un graph est considéré comme disposant de un facts to add si le count of facts to add est supérieur à 0. |
| **DR-9 Next Fact Sort Order** | Un graph : next fact sort order est la plus petite valeur de sort order sur les statements du graph qui sont listed off unit. |
| **DR-10 Max Activation Seq** | Un graph : max activation seq est la plus grande valeur de activation seq sur les statements liés à le graph. |
| **DR-11 Next Activation Seq** | Un graph : next activation seq est calculé comme le max activation seq plus 1. |
| **DR-12 Max Listed Sort Order** | Un graph : max listed sort order est la plus grande valeur de sort order sur les statements du graph qui sont listed unit. |
| **DR-13 Next Listed Sort Order** | Un graph : next listed sort order est calculé comme le max listed sort order plus 1. |
| **DR-14 Max Sort Order** | Un graph : max sort order est la plus grande valeur de sort order sur les statements liés à le graph. |
| **DR-15 Next Candidate Sort Order** | Un graph : next candidate sort order est calculé comme le max sort order plus 1. |
| **DR-16 Name** | Un rule : name est identique à label. |
| **DR-17 Count of Firings** | Un rule : count of firings est le total de fired unit sur les firings liés à le rule. |
| **DR-18 Name** | Un firing kind : name est identique à wording. |
| **DR-19 Rule Enabled** | Un firing kind : rule enabled est vrai lorsque le rule lié est enabled. |
| **DR-20 Portrait Name** | Un thing : portrait name est le display name du portrait lié. |
| **DR-21 Portrait Page** | Un thing : portrait page — repris du portrait lié. |
| **DR-22 Portrait Image** | Un thing : portrait image — repris du portrait lié. |
| **DR-23 Portrait Crop** | Un thing : portrait crop — repris du portrait lié. |
| **DR-24 Portrait Width** | Un thing : portrait width — repris du portrait lié. |
| **DR-25 Portrait Height** | Un thing : portrait height — repris du portrait lié. |
| **DR-26 Name** | Un thing : name est identique à label. |
| **DR-27 Node Key** | Un thing : node key est identique à thing ID. |
| **DR-28 Count of Children** | Un thing : count of children est le total de parent unit sur les statements liés à le thing. |
| **DR-29 Count of Grandchildren** | Un thing : count of grandchildren est le total de grandparent unit sur les statements liés à le thing. |
| **DR-30 Count As Place** | Un thing : count as place est le total de place unit sur les statements liés à le thing. |
| **DR-31 Count As Subject** | Un thing : count as subject est le total de holds unit sur les statements liés à le thing. |
| **DR-32 Count As Object** | Un thing : count as object est le total de holds unit sur les statements liés à le thing. |
| **DR-33 Is in Graph** | Un thing est considéré comme in graph si au moins une des conditions suivantes est vraie : le count as subject est supérieur à 0 ou le count as object est supérieur à 0. |
| **DR-34 Node Kind** | Le champ « node kind » du thing est déterminé selon la priorité suivante :<br>1. une chaîne vide, si le kind est vide ;<br>2. « class », si le kind est « class » ;<br>3. « place », si le count as place est supérieur à 0 ;<br>4. dans tous les autres cas, « person ». |
| **DR-35 Name** | Un portrait : name est identique à display name. |
| **DR-36 Name** | Un portrait aliase : name est identique à portrait alias ID. |
| **DR-37 Name** | Un property : name est identique à label. |
| **DR-38 Name** | Un property axiom : name est identique à property axiom ID. |
| **DR-39 Subject Label** | Un statement : subject label — repris du subject lié. |
| **DR-40 Property Label** | Un statement : property label — repris du property lié. |
| **DR-41 Object Label** | Un statement : object label — repris du object thing lié. |
| **DR-42 Property Datatype** | Un statement : property datatype — repris du property lié. |
| **DR-43 Property Has One Current Value** | Un statement : property has one current value est vrai lorsque le property lié a un one current value. |
| **DR-44 Property Domain Class** | Un statement : property domain class — repris du property lié. |
| **DR-45 Property Range Class** | Un statement : property range class — repris du property lié. |
| **DR-46 Literal Quote** | Un statement : literal quote — repris du graph lié. |
| **DR-47 Literal Type Marker** | Un statement : literal type marker — repris du graph lié. |
| **DR-48 Built in Source Text** | Un statement : built in source text — repris du graph lié. |
| **DR-49 User Source Text** | Un statement : user source text — repris du graph lié. |
| **DR-50 Next Fact Sort Order** | Un statement : next fact sort order — repris du graph lié. |
| **DR-51 Is Literal** | Un statement est considéré comme literal si l'object thing est vide. |
| **DR-52 Object Text** | Le champ « object text » du statement est déterminé selon la priorité suivante :<br>1. l'object value, si l'object thing est vide ;<br>2. dans tous les autres cas, l'object label. |
| **DR-53 Name** | Un statement : name est calculé comme le subject label, suivi de une espace, suivi de le property label, suivi de une espace, suivi de l'object text. |
| **DR-54 Words** | Un statement : words est calculé comme le subject label, suivi de une espace, suivi de le property label, suivi de une espace, suivi de le literal quote, suivi de l'object value, suivi de le literal quote, suivi de le literal type marker, suivi de le property datatype si l'object thing est vide, dans tous les autres cas l'object label. ⚠︎ mécanique <!-- rulespeak:reword --> |
| **DR-55 Source** | Le champ « source » du statement est déterminé selon la priorité suivante :<br>1. le built in source text, si l'indicateur « built in » est activé ;<br>2. dans tous les autres cas, l'user source text. |
| **DR-56 Subject Property Key** | Un statement : subject property key est calculé comme le subject, suivi de « | », suivi de le property. |
| **DR-57 Latest Sibling Activation Seq** | Un statement : latest sibling activation seq est la plus grande valeur de activation seq sur les statements liés à le statement. |
| **DR-58 Is in Effect** | Un statement est considéré comme in effect si tout ce qui suit est vrai : l'indicateur « stated » est activé et au moins une des conditions suivantes est vraie : l'indicateur « property has one current value » n'est pas activé ou l'activation seq est le latest sibling activation seq. |
| **DR-59 Listed Unit** | Le champ « listed unit » du statement est déterminé selon la priorité suivante :<br>1. 1, si l'indicateur « listed » est activé ;<br>2. dans tous les autres cas, 0. |
| **DR-60 In Effect Unit** | Le champ « in effect unit » du statement est déterminé selon la priorité suivante :<br>1. 1, si l'indicateur « in effect » est activé ;<br>2. dans tous les autres cas, 0. |
| **DR-61 Listed Off Unit** | Le champ « listed off unit » du statement est déterminé selon la priorité suivante :<br>1. 1, si tout ce qui suit est vrai : l'indicateur « listed » est activé et l'indicateur « in effect » n'est pas activé ;<br>2. dans tous les autres cas, 0. |
| **DR-62 Is Next to Add** | Un statement est considéré comme un next to add si tout ce qui suit est vrai : le listed off unit est 1 et le sort order est le next fact sort order. |
| **DR-63 Count of Inverse Firings** | Un statement : count of inverse firings est le total de inverse fired unit sur les firings liés à le statement. |
| **DR-64 Count of Symmetric Firings** | Un statement : count of symmetric firings est le total de symmetric fired unit sur les firings liés à le statement. |
| **DR-65 Holds Before Chain** | Un statement est considéré comme satisfaisant : before chain si le IF de le OR de l'is in effect; le count of inverse firings est supérieur à 0; et le count of symmetric firings est supérieur à 0 et vrai. |
| **DR-66 Count of Chain Firings** | Un statement : count of chain firings est le total de chain fired unit sur les firings liés à le statement. |
| **DR-67 Holds Before Typing** | Un statement est considéré comme satisfaisant : before typing si le IF de le OR de l'holds before chain et le count of chain firings est supérieur à 0 et vrai. |
| **DR-68 Count of Type Firings** | Un statement : count of type firings est le total de type fired unit sur les firings liés à le statement. |
| **DR-69 Holds** | Un statement est considéré comme satisfaisant : si le IF de le OR de l'holds before typing et le count of type firings est supérieur à 0 et vrai. |
| **DR-70 Is Conclusion** | Un statement est considéré comme une conclusion si le IF de le AND de l'holds et l'indicateur « in effect » n'est pas activé et vrai. |
| **DR-71 Holds Unit** | Le champ « holds unit » du statement est déterminé selon la priorité suivante :<br>1. 1, si l'indicateur « holds » est activé ;<br>2. dans tous les autres cas, 0. |
| **DR-72 Conclusion Unit** | Le champ « conclusion unit » du statement est déterminé selon la priorité suivante :<br>1. 1, si l'indicateur « conclusion » est activé ;<br>2. dans tous les autres cas, 0. |
| **DR-73 Parent Unit** | Le champ « parent unit » du statement est déterminé selon la priorité suivante :<br>1. 1, si tout ce qui suit est vrai : l'indicateur « holds » est activé et le property est « isParentOf » ;<br>2. dans tous les autres cas, 0. |
| **DR-74 Grandparent Unit** | Le champ « grandparent unit » du statement est déterminé selon la priorité suivante :<br>1. 1, si tout ce qui suit est vrai : l'indicateur « holds » est activé et le property est « isGrandparentOf » ;<br>2. dans tous les autres cas, 0. |
| **DR-75 Place Unit** | Le champ « place unit » du statement est déterminé selon la priorité suivante :<br>1. 1, si tout ce qui suit est vrai : l'indicateur « holds » est activé et le property est « attends » ;<br>2. dans tous les autres cas, 0. |
| **DR-76 First Pre Chain Order Key** | Un statement : first pre chain order key est la plus petite valeur de pre chain order key sur les firings du statement qui sont pre chain fired unit. |
| **DR-77 Snap Position** | Le champ « snap position » du statement est déterminé selon la priorité suivante :<br>1. le sort order, si l'indicateur « in effect » est activé ;<br>2. 100000 plus le first pre chain order key, si l'indicateur « holds before chain » est activé ;<br>3. dans tous les autres cas, 0. |
| **DR-78 First Fired Order Key** | Un statement : first fired order key est la plus petite valeur de order key sur les firings du statement qui sont fired unit. |
| **DR-79 Count of Chosen Firings** | Un statement : count of chosen firings est le total de chosen unit sur les firings liés à le statement. |
| **DR-80 Subject Node Key** | Un statement : subject node key est identique à subject. |
| **DR-81 Object Node Key** | Le champ « object node key » du statement est déterminé selon la priorité suivante :<br>1. « literal| », suivi de l'object value, suivi de « | », suivi de le property datatype, si l'object thing est vide ;<br>2. dans tous les autres cas, l'object thing. |
| **DR-82 Subject Node Kind** | Un statement : subject node kind — repris du subject lié. |
| **DR-83 Object Thing Node Kind** | Un statement : object thing node kind — repris du object thing lié. |
| **DR-84 Object Node Kind** | Le champ « object node kind » du statement est déterminé selon la priorité suivante :<br>1. « value », si l'object thing est vide ;<br>2. dans tous les autres cas, l'object thing node kind. |
| **DR-85 Object Value Node Label** | Un statement : object value node label — repris du property lié. |
| **DR-86 Count of Violations As First** | Un statement : count of violations as first est le total de violation unit sur les disjoint checks liés à le statement. |
| **DR-87 Count of Violations As Second** | Un statement : count of violations as second est le total de violation unit sur les disjoint checks liés à le statement. |
| **DR-88 Is in Contradiction** | Un statement est considéré comme in contradiction si au moins une des conditions suivantes est vraie : le count of violations as first est supérieur à 0 ou le count of violations as second est supérieur à 0. |
| **DR-89 Rule Enabled** | Un firing : rule enabled est vrai lorsque le rule lié est enabled. |
| **DR-90 Kind Wording** | Un firing : kind wording — repris du kind lié. |
| **DR-91 Kind Sort Order** | Un firing : kind sort order — repris du kind lié. |
| **DR-92 Target Name** | Un firing : target name — repris du target lié. |
| **DR-93 Target Subject Label** | Un firing : target subject label — repris du target lié. |
| **DR-94 Target Property Label** | Un firing : target property label — repris du target lié. |
| **DR-95 Target Object Text** | Un firing : target object text — repris du target lié. |
| **DR-96 Target is Conclusion** | Un firing : target is conclusion est vrai lorsque le target lié est une conclusion. |
| **DR-97 Target Subject Node Key** | Un firing : target subject node key — repris du target lié. |
| **DR-98 Target Object Node Key** | Un firing : target object node key — repris du target lié. |
| **DR-99 Target First Fired Order Key** | Un firing : target first fired order key — repris du target lié. |
| **DR-100 First Words** | Un firing : first words — repris du first lié. |
| **DR-101 Second Words** | Un firing : second words — repris du second lié. |
| **DR-102 First is in Effect** | Un firing : first is in effect est vrai lorsque le first lié est in effect. |
| **DR-103 First Holds Before Chain** | Un firing : first holds before chain est vrai lorsque le first lié est holds before chain. |
| **DR-104 Second Holds Before Chain** | Un firing : second holds before chain est vrai lorsque le second lié est holds before chain. |
| **DR-105 First Holds Before Typing** | Un firing : first holds before typing est vrai lorsque le first lié est holds before typing. |
| **DR-106 First Property** | Un firing : first property — repris du first lié. |
| **DR-107 First Property Domain Class** | Un firing : first property domain class — repris du first lié. |
| **DR-108 First Property Range Class** | Un firing : first property range class — repris du first lié. |
| **DR-109 First Sort Order** | Un firing : first sort order — repris du first lié. |
| **DR-110 Second is in Effect** | Un firing : second is in effect est vrai lorsque le second lié est in effect. |
| **DR-111 First Snap Position** | Un firing : first snap position — repris du first lié. |
| **DR-112 Second Snap Position** | Un firing : second snap position — repris du second lié. |
| **DR-113 Inverse Fired Unit** | Le champ « inverse fired unit » du firing est déterminé selon la priorité suivante :<br>1. 1, si tout ce qui suit est vrai : l'indicateur « rule enabled » est activé; le kind est « inverse »; et l'indicateur « first is in effect » est activé ;<br>2. dans tous les autres cas, 0. |
| **DR-114 Symmetric Fired Unit** | Le champ « symmetric fired unit » du firing est déterminé selon la priorité suivante :<br>1. 1, si tout ce qui suit est vrai : l'indicateur « rule enabled » est activé; le kind est « symmetric »; et l'indicateur « first is in effect » est activé ;<br>2. dans tous les autres cas, 0. |
| **DR-115 Chain Fired Unit** | Le champ « chain fired unit » du firing est déterminé selon la priorité suivante :<br>1. 1, si tout ce qui suit est vrai : l'indicateur « rule enabled » est activé; le kind est « chain »; l'indicateur « first holds before chain » est activé; et l'indicateur « second holds before chain » est activé ;<br>2. dans tous les autres cas, 0. |
| **DR-116 Type Fired Unit** | Le champ « type fired unit » du firing est déterminé selon la priorité suivante :<br>1. 1, si tout ce qui suit est vrai : l'indicateur « rule enabled » est activé; au moins une des conditions suivantes est vraie : le kind est « domain » ou le kind est « range »; et l'indicateur « first holds before typing » est activé ;<br>2. dans tous les autres cas, 0. |
| **DR-117 Fired Unit** | Un firing : fired unit est calculé comme l'inverse fired unit plus le symmetric fired unit plus le chain fired unit plus le type fired unit. |
| **DR-118 Pre Chain Fired Unit** | Un firing : pre chain fired unit est calculé comme l'inverse fired unit plus le symmetric fired unit. |
| **DR-119 Pre Chain Order Key** | Un firing : pre chain order key est calculé comme le first sort order multiplié par 10 plus le kind sort order. |
| **DR-120 Round** | Le champ « round » du firing est déterminé selon la priorité suivante :<br>1. 2, si tout ce qui suit est vrai : le kind est « chain » et il n'est pas vrai que tout ce qui suit est vrai : l'indicateur « first is in effect » est activé et l'indicateur « second is in effect » est activé ;<br>2. dans tous les autres cas, 1. |
| **DR-121 Order Key** | Un firing : order key est calculé comme le round multiplié par 1000000000000000 plus la first snap position multiplié par 100000000 plus le kind sort order multiplié par 10000000 plus 0 si le second est vide, dans tous les autres cas la second snap position. |
| **DR-122 Is Chosen** | Un firing est considéré comme un chosen si tout ce qui suit est vrai : le fired unit est 1; l'indicateur « target is conclusion » est activé; et l'order key est le target first fired order key. |
| **DR-123 Chosen Unit** | Le champ « chosen unit » du firing est déterminé selon la priorité suivante :<br>1. 1, si l'indicateur « chosen » est activé ;<br>2. dans tous les autres cas, 0. |
| **DR-124 Card Text** | Le champ « card text » du firing est déterminé selon la priorité suivante :<br>1. « Worked out by the », suivi de le kind wording, suivi de « from “ », suivi de le first words, suivi de « ” », suivi de « and “ », suivi de le second words, suivi de « ” » si le kind est « chain », dans tous les autres cas « and “ », suivi de le first property, suivi de « has domain », suivi de le first property domain class, suivi de « ” » si le kind est « domain », dans tous les autres cas « and “ », suivi de le first property, suivi de « has range », suivi de le first property range class, suivi de « ” », suivi de un point, si l'indicateur « chosen » est activé ;<br>2. dans tous les autres cas, une chaîne vide. ⚠︎ mécanique <!-- rulespeak:reword --> |
| **DR-125 Name** | Un firing : name est identique à firing ID. |
| **DR-126 Rule Enabled** | Un disjoint check : rule enabled est vrai lorsque le rule lié est enabled. |
| **DR-127 First Holds** | Un disjoint check : first holds est vrai lorsque le first lié est holds. |
| **DR-128 Second Holds** | Un disjoint check : second holds est vrai lorsque le second lié est holds. |
| **DR-129 Subject Label** | Un disjoint check : subject label — repris du first lié. |
| **DR-130 Object Label** | Un disjoint check : object label — repris du first lié. |
| **DR-131 Violation Unit** | Le champ « violation unit » du disjoint check est déterminé selon la priorité suivante :<br>1. 1, si tout ce qui suit est vrai : l'indicateur « rule enabled » est activé; l'indicateur « first holds » est activé; et l'indicateur « second holds » est activé ;<br>2. dans tous les autres cas, 0. |
| **DR-132 Alarm Text** | Le champ « alarm text » du disjoint check est déterminé selon la priorité suivante :<br>1. « Contradiction found. », suivi de le subject label, suivi de « is recorded as both parent and spouse of », suivi de l'object label, suivi de «. Check those facts and the selected disjoint rule. », si le violation unit est 1 ;<br>2. dans tous les autres cas, une chaîne vide. |
| **DR-133 Name** | Un disjoint check : name est identique à disjoint check ID. |
| **DR-134 Name** | Un scenario : name est identique à label. |
| **DR-135 Graph Has Facts to Add** | Un scenario : graph has facts to add est vrai lorsque le graph lié a un facts to add. |
| **DR-136 Is Available** | Un scenario est considéré comme available si au moins une des conditions suivantes est vraie : le fact baseline n'est pas « next » ou l'indicateur « graph has facts to add » est activé. |
| **DR-137 Name** | Un scenario fact change : name est identique à scenario fact change ID. |
| **DR-138 Name** | Un scenario rule setting : name est identique à scenario rule setting ID. |
| **DR-139 Name** | Un node layout : name est identique à node layout ID. |
| **DR-140 Name** | Un layout slot : name est identique à layout slot ID. |
| **DR-141 Name** | Un page text : name est identique à page text ID. |
| **DR-142 Name** | Un engine : name est identique à label. |
| **DR-143 Name** | Un rulebook tool : name est identique à label. |

## 5 Traçabilité vers le schéma

_La colonne expression est la définition de la règle en notation Documentation —
la même logique que stocke le livre de règles, écrite pour un lecteur métier._

| Élément du schéma | Type | Expression |
|----------------|------|------------|
| **Graphs.Name** | formule | `Label` |
| **Graphs.CountOfStatedFacts** | agrégat | `Sum(Statements.InEffectUnit via Graph)` |
| **Graphs.CountOfConclusions** | agrégat | `Sum(Statements.ConclusionUnit via Graph)` |
| **Graphs.CountOfViolations** | agrégat | `Sum(DisjointChecks.ViolationUnit via Graph)` |
| **Graphs.IsConsistent** | formule | `CountOfViolations = 0` |
| **Graphs.ConclusionsMessage** | formule | `If(CountOfConclusions = 0, NoConclusionsText, "")` |
| **Graphs.CountOfFactsToAdd** | agrégat | `Sum(Statements.ListedOffUnit via Graph)` |
| **Graphs.HasFactsToAdd** | formule | `CountOfFactsToAdd > 0` |
| **Graphs.NextFactSortOrder** | agrégat | `Min(Statements.SortOrder via Graph)` |
| **Graphs.MaxActivationSeq** | agrégat | `Max(Statements.ActivationSeq via Graph)` |
| **Graphs.NextActivationSeq** | formule | `MaxActivationSeq + 1` |
| **Graphs.MaxListedSortOrder** | agrégat | `Max(Statements.SortOrder via Graph)` |
| **Graphs.NextListedSortOrder** | formule | `MaxListedSortOrder + 1` |
| **Graphs.MaxSortOrder** | agrégat | `Max(Statements.SortOrder via Graph)` |
| **Graphs.NextCandidateSortOrder** | formule | `MaxSortOrder + 1` |
| **Rules.Name** | formule | `Label` |
| **Rules.CountOfFirings** | agrégat | `Sum(Firings.FiredUnit via Rule)` |
| **FiringKinds.Name** | formule | `Wording` |
| **FiringKinds.RuleEnabled** | recherche | `Lookup(Rules.IsEnabled via Rule)` |
| **Things.PortraitName** | recherche | `Lookup(Portraits.DisplayName via Portrait)` |
| **Things.PortraitPage** | recherche | `Lookup(Portraits.Page via Portrait)` |
| **Things.PortraitImage** | recherche | `Lookup(Portraits.Image via Portrait)` |
| **Things.PortraitCrop** | recherche | `Lookup(Portraits.Crop via Portrait)` |
| **Things.PortraitWidth** | recherche | `Lookup(Portraits.Width via Portrait)` |
| **Things.PortraitHeight** | recherche | `Lookup(Portraits.Height via Portrait)` |
| **Things.Name** | formule | `Label` |
| **Things.NodeKey** | formule | `ThingId` |
| **Things.CountOfChildren** | agrégat | `Sum(Statements.ParentUnit via Subject)` |
| **Things.CountOfGrandchildren** | agrégat | `Sum(Statements.GrandparentUnit via Subject)` |
| **Things.CountAsPlace** | agrégat | `Sum(Statements.PlaceUnit via ObjectThing)` |
| **Things.CountAsSubject** | agrégat | `Sum(Statements.HoldsUnit via Subject)` |
| **Things.CountAsObject** | agrégat | `Sum(Statements.HoldsUnit via ObjectThing)` |
| **Things.IsInGraph** | formule | `Or(CountAsSubject > 0, CountAsObject > 0)` |
| **Things.NodeKind** | formule | `If(Kind = "", "", If(Kind = "class", "class", If(CountAsPlace > 0, "place", "person")))` |
| **Portraits.Name** | formule | `DisplayName` |
| **PortraitAliases.Name** | formule | `PortraitAliasId` |
| **Properties.Name** | formule | `Label` |
| **PropertyAxioms.Name** | formule | `PropertyAxiomId` |
| **Statements.SubjectLabel** | recherche | `Lookup(Things.Label via Subject)` |
| **Statements.PropertyLabel** | recherche | `Lookup(Properties.Label via Property)` |
| **Statements.ObjectLabel** | recherche | `Lookup(Things.Label via ObjectThing)` |
| **Statements.PropertyDatatype** | recherche | `Lookup(Properties.Datatype via Property)` |
| **Statements.PropertyHasOneCurrentValue** | recherche | `Lookup(Properties.HasOneCurrentValue via Property)` |
| **Statements.PropertyDomainClass** | recherche | `Lookup(Properties.DomainClass via Property)` |
| **Statements.PropertyRangeClass** | recherche | `Lookup(Properties.RangeClass via Property)` |
| **Statements.LiteralQuote** | recherche | `Lookup(Graphs.LiteralQuote via Graph)` |
| **Statements.LiteralTypeMarker** | recherche | `Lookup(Graphs.LiteralTypeMarker via Graph)` |
| **Statements.BuiltInSourceText** | recherche | `Lookup(Graphs.BuiltInSourceText via Graph)` |
| **Statements.UserSourceText** | recherche | `Lookup(Graphs.UserSourceText via Graph)` |
| **Statements.NextFactSortOrder** | recherche | `Lookup(Graphs.NextFactSortOrder via Graph)` |
| **Statements.IsLiteral** | formule | `ObjectThing = ""` |
| **Statements.ObjectText** | formule | `If(ObjectThing = "", ObjectValue, ObjectLabel)` |
| **Statements.Name** | formule | `Concat(SubjectLabel, " ", PropertyLabel, " ", ObjectText)` |
| **Statements.Words** | formule | `Concat(SubjectLabel, " ", PropertyLabel, " ", If(ObjectThing = "", Concat(LiteralQuote, ObjectValue, LiteralQuote, LiteralTypeMarker, PropertyDatatype), ObjectLabel))` |
| **Statements.Source** | formule | `If(IsBuiltIn, BuiltInSourceText, UserSourceText)` |
| **Statements.SubjectPropertyKey** | formule | `Concat(Subject, "\|", Property)` |
| **Statements.LatestSiblingActivationSeq** | agrégat | `Max(Statements.ActivationSeq via SubjectPropertyKey)` |
| **Statements.IsInEffect** | formule | `And(IsStated, Or(Not(PropertyHasOneCurrentValue), ActivationSeq = LatestSiblingActivationSeq))` |
| **Statements.ListedUnit** | formule | `If(IsListed, 1, 0)` |
| **Statements.InEffectUnit** | formule | `If(IsInEffect, 1, 0)` |
| **Statements.ListedOffUnit** | formule | `If(And(IsListed, Not(IsInEffect)), 1, 0)` |
| **Statements.IsNextToAdd** | formule | `And(ListedOffUnit = 1, SortOrder = NextFactSortOrder)` |
| **Statements.CountOfInverseFirings** | agrégat | `Sum(Firings.InverseFiredUnit via Target)` |
| **Statements.CountOfSymmetricFirings** | agrégat | `Sum(Firings.SymmetricFiredUnit via Target)` |
| **Statements.HoldsBeforeChain** | formule | `If(Or(IsInEffect, CountOfInverseFirings > 0, CountOfSymmetricFirings > 0), True())` |
| **Statements.CountOfChainFirings** | agrégat | `Sum(Firings.ChainFiredUnit via Target)` |
| **Statements.HoldsBeforeTyping** | formule | `If(Or(HoldsBeforeChain, CountOfChainFirings > 0), True())` |
| **Statements.CountOfTypeFirings** | agrégat | `Sum(Firings.TypeFiredUnit via Target)` |
| **Statements.Holds** | formule | `If(Or(HoldsBeforeTyping, CountOfTypeFirings > 0), True())` |
| **Statements.IsConclusion** | formule | `If(And(Holds, Not(IsInEffect)), True())` |
| **Statements.HoldsUnit** | formule | `If(Holds, 1, 0)` |
| **Statements.ConclusionUnit** | formule | `If(IsConclusion, 1, 0)` |
| **Statements.ParentUnit** | formule | `If(And(Holds, Property = "isParentOf"), 1, 0)` |
| **Statements.GrandparentUnit** | formule | `If(And(Holds, Property = "isGrandparentOf"), 1, 0)` |
| **Statements.PlaceUnit** | formule | `If(And(Holds, Property = "attends"), 1, 0)` |
| **Statements.FirstPreChainOrderKey** | agrégat | `Min(Firings.PreChainOrderKey via Target)` |
| **Statements.SnapPosition** | formule | `If(IsInEffect, SortOrder, If(HoldsBeforeChain, 100000 + FirstPreChainOrderKey, 0))` |
| **Statements.FirstFiredOrderKey** | agrégat | `Min(Firings.OrderKey via Target)` |
| **Statements.CountOfChosenFirings** | agrégat | `Sum(Firings.ChosenUnit via Target)` |
| **Statements.SubjectNodeKey** | formule | `Subject` |
| **Statements.ObjectNodeKey** | formule | `If(ObjectThing = "", Concat("literal\|", ObjectValue, "\|", PropertyDatatype), ObjectThing)` |
| **Statements.SubjectNodeKind** | recherche | `Lookup(Things.NodeKind via Subject)` |
| **Statements.ObjectThingNodeKind** | recherche | `Lookup(Things.NodeKind via ObjectThing)` |
| **Statements.ObjectNodeKind** | formule | `If(ObjectThing = "", "value", ObjectThingNodeKind)` |
| **Statements.ObjectValueNodeLabel** | recherche | `Lookup(Properties.ValueNodeLabel via Property)` |
| **Statements.CountOfViolationsAsFirst** | agrégat | `Sum(DisjointChecks.ViolationUnit via First)` |
| **Statements.CountOfViolationsAsSecond** | agrégat | `Sum(DisjointChecks.ViolationUnit via Second)` |
| **Statements.IsInContradiction** | formule | `Or(CountOfViolationsAsFirst > 0, CountOfViolationsAsSecond > 0)` |
| **Firings.RuleEnabled** | recherche | `Lookup(Rules.IsEnabled via Rule)` |
| **Firings.KindWording** | recherche | `Lookup(FiringKinds.Wording via Kind)` |
| **Firings.KindSortOrder** | recherche | `Lookup(FiringKinds.SortOrder via Kind)` |
| **Firings.TargetName** | recherche | `Lookup(Statements.Name via Target)` |
| **Firings.TargetSubjectLabel** | recherche | `Lookup(Statements.SubjectLabel via Target)` |
| **Firings.TargetPropertyLabel** | recherche | `Lookup(Statements.PropertyLabel via Target)` |
| **Firings.TargetObjectText** | recherche | `Lookup(Statements.ObjectText via Target)` |
| **Firings.TargetIsConclusion** | recherche | `Lookup(Statements.IsConclusion via Target)` |
| **Firings.TargetSubjectNodeKey** | recherche | `Lookup(Statements.SubjectNodeKey via Target)` |
| **Firings.TargetObjectNodeKey** | recherche | `Lookup(Statements.ObjectNodeKey via Target)` |
| **Firings.TargetFirstFiredOrderKey** | recherche | `Lookup(Statements.FirstFiredOrderKey via Target)` |
| **Firings.FirstWords** | recherche | `Lookup(Statements.Words via First)` |
| **Firings.SecondWords** | recherche | `Lookup(Statements.Words via Second)` |
| **Firings.FirstIsInEffect** | recherche | `Lookup(Statements.IsInEffect via First)` |
| **Firings.FirstHoldsBeforeChain** | recherche | `Lookup(Statements.HoldsBeforeChain via First)` |
| **Firings.SecondHoldsBeforeChain** | recherche | `Lookup(Statements.HoldsBeforeChain via Second)` |
| **Firings.FirstHoldsBeforeTyping** | recherche | `Lookup(Statements.HoldsBeforeTyping via First)` |
| **Firings.FirstProperty** | recherche | `Lookup(Statements.Property via First)` |
| **Firings.FirstPropertyDomainClass** | recherche | `Lookup(Statements.PropertyDomainClass via First)` |
| **Firings.FirstPropertyRangeClass** | recherche | `Lookup(Statements.PropertyRangeClass via First)` |
| **Firings.FirstSortOrder** | recherche | `Lookup(Statements.SortOrder via First)` |
| **Firings.SecondIsInEffect** | recherche | `Lookup(Statements.IsInEffect via Second)` |
| **Firings.FirstSnapPosition** | recherche | `Lookup(Statements.SnapPosition via First)` |
| **Firings.SecondSnapPosition** | recherche | `Lookup(Statements.SnapPosition via Second)` |
| **Firings.InverseFiredUnit** | formule | `If(And(RuleEnabled, Kind = "inverse", FirstIsInEffect), 1, 0)` |
| **Firings.SymmetricFiredUnit** | formule | `If(And(RuleEnabled, Kind = "symmetric", FirstIsInEffect), 1, 0)` |
| **Firings.ChainFiredUnit** | formule | `If(And(RuleEnabled, Kind = "chain", FirstHoldsBeforeChain, SecondHoldsBeforeChain), 1, 0)` |
| **Firings.TypeFiredUnit** | formule | `If(And(RuleEnabled, Or(Kind = "domain", Kind = "range"), FirstHoldsBeforeTyping), 1, 0)` |
| **Firings.FiredUnit** | formule | `InverseFiredUnit + SymmetricFiredUnit + ChainFiredUnit + TypeFiredUnit` |
| **Firings.PreChainFiredUnit** | formule | `InverseFiredUnit + SymmetricFiredUnit` |
| **Firings.PreChainOrderKey** | formule | `FirstSortOrder * 10 + KindSortOrder` |
| **Firings.Round** | formule | `If(And(Kind = "chain", Not(And(FirstIsInEffect, SecondIsInEffect))), 2, 1)` |
| **Firings.OrderKey** | formule | `Round * 1000000000000000 + FirstSnapPosition * 100000000 + KindSortOrder * 10000000 + If(Second = "", 0, SecondSnapPosition)` |
| **Firings.IsChosen** | formule | `And(FiredUnit = 1, TargetIsConclusion, OrderKey = TargetFirstFiredOrderKey)` |
| **Firings.ChosenUnit** | formule | `If(IsChosen, 1, 0)` |
| **Firings.CardText** | formule | `If(IsChosen, Concat("Worked out by the ", KindWording, " from “", FirstWords, "”", If(Kind = "chain", Concat(" and “", SecondWords, "”"), If(Kind = "domain", Concat(" and “", FirstProperty, " has domain ", FirstPropertyDomainClass, "”"), If(Kind = "range", Concat(" and “", FirstProperty, " has range ", FirstPropertyRangeClass, "”"), ""))), "."), "")` |
| **Firings.Name** | formule | `FiringId` |
| **DisjointChecks.RuleEnabled** | recherche | `Lookup(Rules.IsEnabled via Rule)` |
| **DisjointChecks.FirstHolds** | recherche | `Lookup(Statements.Holds via First)` |
| **DisjointChecks.SecondHolds** | recherche | `Lookup(Statements.Holds via Second)` |
| **DisjointChecks.SubjectLabel** | recherche | `Lookup(Statements.SubjectLabel via First)` |
| **DisjointChecks.ObjectLabel** | recherche | `Lookup(Statements.ObjectLabel via First)` |
| **DisjointChecks.ViolationUnit** | formule | `If(And(RuleEnabled, FirstHolds, SecondHolds), 1, 0)` |
| **DisjointChecks.AlarmText** | formule | `If(ViolationUnit = 1, Concat("Contradiction found. ", SubjectLabel, " is recorded as both parent and spouse of ", ObjectLabel, ". Check those facts and the selected disjoint rule."), "")` |
| **DisjointChecks.Name** | formule | `DisjointCheckId` |
| **Scenarios.Name** | formule | `Label` |
| **Scenarios.GraphHasFactsToAdd** | recherche | `Lookup(Graphs.HasFactsToAdd via Graph)` |
| **Scenarios.IsAvailable** | formule | `Or(FactBaseline <> "next", GraphHasFactsToAdd)` |
| **ScenarioFactChanges.Name** | formule | `ScenarioFactChangeId` |
| **ScenarioRuleSettings.Name** | formule | `ScenarioRuleSettingId` |
| **NodeLayouts.Name** | formule | `NodeLayoutId` |
| **LayoutSlots.Name** | formule | `LayoutSlotId` |
| **PageTexts.Name** | formule | `PageTextId` |
| **Engines.Name** | formule | `Label` |
| **RulebookTools.Name** | formule | `Label` |

---

> **RuleSpeak®** — la notation déclarative de règles métier créée par **Ronald G. Ross**, selon les conventions de **SBVR** — arrive bientôt. D’ici là, cette page présente la documentation habituelle. [www.RonRoss.info](https://www.RonRoss.info)
