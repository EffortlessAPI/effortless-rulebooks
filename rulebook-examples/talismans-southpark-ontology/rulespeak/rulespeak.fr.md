# 📘 Talisman's South Park ontology explainer — Documentation

_Jessica Talisman's South Park Ontology Explainer, as a rulebook: the page's ten facts, five rule switches, property wiring, provenance wording and current-value policy are rows, and every conclusion is a formula. The rules apply in three fixed strata (inverse and symmetric, then the property chain, then domain and range); property wiring that feeds an earlier stratum would need repeated rounds._

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

<details open>
<summary>Données d'exemple</summary>

| Label | Literal Quote | Literal Type Marker | Built in Source Text | User Source Text | No Conclusions Text | Name ƒ | Count of Stated Facts ƒ | Count of Conclusions ƒ | Count of Violations ƒ | Is Consistent ƒ | Conclusions Message ƒ | Count of Facts to Add ƒ | Has Facts to Add ƒ | Next Fact Sort Order ƒ | Max Activation Seq ƒ | Next Activation Seq ƒ | Max Listed Sort Order ƒ | Next Listed Sort Order ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| South Park | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | These facts and selected rules add no further conclusions. Try another fact or rule. | South Park | 10 | 0 | 0 | true | These facts and selected rules add no further conclusions. Try another fact or rule. | 0 | false | — | 10 | 11 | 10 | 11 |

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
| **Firing Kind** | The ways a rule can apply. Domain and range belong to one rule but are worded apart. | — |
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
| Portrait Name | Un attribut défini. | _The name on the portrait's Fandom page._ |
| Portrait Page | Un attribut défini. | _The Fandom page credited for the portrait._ |
| Portrait Image | Un attribut défini. | _The original Fandom image._ |
| Portrait Crop | Un attribut défini. | _The portrait's crop as an SVG viewBox: x y width height._ |
| Name | Identique à label. | _Order 1. The thing's name._ |
| Count of Children | Le total de parent unit sur les statements liés à le thing. | _Order 17. People this thing is a parent of, stated or concluded._ |
| Count of Grandchildren | Le total de grandparent unit sur les statements liés à le thing. | _Order 17. People this thing is a grandparent of._ |
| Count As Place | Le total de place unit sur les statements liés à le thing. | _Order 17. Holding attends facts whose object is this thing._ |
| Count As Subject | Le total de holds unit sur les statements liés à le thing. | _Order 17. Holding facts about this thing._ |
| Count As Object | Le total de holds unit sur les statements liés à le thing. | _Order 17. Holding facts that point at this thing._ |
| Is in Graph | Vrai lorsque au moins une des conditions suivantes est vraie : le count as subject est supérieur à 0 ou le count as object est supérieur à 0. | _Order 18. True when the graph draws this thing._ |
| Node Kind | Déterminé par priorité : « class » si le kind est « class »; « place » si le count as place est supérieur à 0 ; dans tous les autres cas, « person ». | _Order 18. How the graph draws the thing: a class, a place someone attends, or a person._ |

<details open>
<summary>Données d'exemple</summary>

| Label | Kind | Sort Order | Portrait Name | Portrait Page | Portrait Image | Portrait Crop | Name ƒ | Count of Children ƒ | Count of Grandchildren ƒ | Count As Place ƒ | Count As Subject ƒ | Count As Object ƒ | Is in Graph ƒ | Node Kind ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| Marvin | individual | 1 | Marvin Marsh | https://southpark.fandom.com/wiki/Marvin_Marsh | https://static.wikia.nocookie.net/southpark/images/d/db/Marsh-family-grandpa-marsh-w-purple-blanket.png/revision/latest | 48 -5 280 280 | Marvin | 1 | 0 | 0 | 1 | 0 | true | person |
| Randy | individual | 2 | Randy Marsh | https://southpark.fandom.com/wiki/Randy_Marsh | https://static.wikia.nocookie.net/southparkfanon/images/d/d6/Randy_Marsh.png/revision/latest | 0 -10 428 428 | Randy | 2 | 0 | 0 | 3 | 1 | true | person |
| Sharon | individual | 3 | Sharon Marsh | https://southpark.fandom.com/wiki/Sharon_Marsh | https://static.wikia.nocookie.net/southpark/images/9/99/Sharon_Marsh.png/revision/latest | 20 -10 390 390 | Sharon | 0 | 0 | 0 | 0 | 1 | true | person |

_ƒ signale une colonne calculée._

</details>

| Terme | Description | Commentaire narratif |
|------|-------------|-------------------|
| **Property** | The ten properties and the wiring every rule reads. | — |
| Label | Un attribut défini. | _The property's wording on the page._ |
| Property Kind | Un attribut défini. | _'object' links two things, 'datatype' links a thing to a value, 'type' states class membership._ |
| Datatype | Un attribut défini. | _The value's XML Schema datatype for a datatype property; blank otherwise._ |
| Is User Addable | Vrai lorsque une chaîne vide. | _Offered in the page's Add fact form._ |
| Has One Current Value | Vrai lorsque une chaîne vide. | _The page keeps one current value per person: switching a value on replaces the earlier one._ |
| Inverse of | Un attribut défini. | _The property the inverse rule derives from this one._ |
| Is Symmetric | Vrai lorsque une chaîne vide. | _The symmetric rule makes this link true in both directions._ |
| Chain First | Un attribut défini. | _First link of the property chain that derives this property._ |
| Chain Second | Un attribut défini. | _Second link of the property chain that derives this property._ |
| Domain Class | Un attribut défini. | _The class the domain rule gives this property's subject._ |
| Range Class | Un attribut défini. | _The class the range rule gives this property's object._ |
| Disjoint With | Un attribut défini. | _A property that may not link the same pair, in the same direction, as this one._ |
| Sort Order | Un attribut défini. | _Position in the Add fact form's property list._ |
| Name | Identique à label. | _Order 1. The property's wording._ |

<details open>
<summary>Données d'exemple</summary>

| Label | Property Kind | Datatype | Is User Addable | Has One Current Value | Is Symmetric | Sort Order | Name ƒ |
|---|---|---|---|---|---|---|---|
| is parent of | object | — | true | false | false | 1 | is parent of |
| is child of | object | — | true | false | false | 2 | is child of |
| is spouse of | object | — | true | false | true | 3 | is spouse of |

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
| Holds Before Chain | Vrai lorsque au moins une des conditions suivantes est vraie : l'indicateur « in effect » est activé; le count of inverse firings est supérieur à 0; ou le count of symmetric firings est supérieur à 0. | _Order 7. Holds from facts in effect plus the inverse and symmetric rules._ |
| Count of Chain Firings | Le total de chain fired unit sur les firings liés à le statement. | _Order 10. Property chain applications that conclude this fact._ |
| Holds Before Typing | Vrai lorsque au moins une des conditions suivantes est vraie : l'indicateur « holds before chain » est activé ou le count of chain firings est supérieur à 0. | _Order 11. Holds before the domain and range rules apply._ |
| Count of Type Firings | Le total de type fired unit sur les firings liés à le statement. | _Order 14. Domain and range applications that conclude this fact._ |
| Holds | Vrai lorsque au moins une des conditions suivantes est vraie : l'indicateur « holds before typing » est activé ou le count of type firings est supérieur à 0. | _Order 15. True when in effect or concluded by an enabled rule; the graph draws it._ |
| Is Conclusion | Vrai lorsque tout ce qui suit est vrai : l'indicateur « holds » est activé et l'indicateur « in effect » n'est pas activé. | _Order 16. Holds only because a rule concluded it. Drawn dashed._ |
| Holds Unit | Déterminé par priorité : 1 si l'indicateur « holds » est activé ; dans tous les autres cas, 0. | _Order 16. 1 when the fact holds._ |
| Conclusion Unit | Déterminé par priorité : 1 si l'indicateur « conclusion » est activé ; dans tous les autres cas, 0. | _Order 17. 1 when the fact is a conclusion._ |
| Parent Unit | Déterminé par priorité : 1 si tout ce qui suit est vrai : l'indicateur « holds » est activé et le property est « isParentOf » ; dans tous les autres cas, 0. | _Order 16. 1 for a holding parent link._ |
| Grandparent Unit | Déterminé par priorité : 1 si tout ce qui suit est vrai : l'indicateur « holds » est activé et le property est « isGrandparentOf » ; dans tous les autres cas, 0. | _Order 16. 1 for a holding grandparent link._ |
| Place Unit | Déterminé par priorité : 1 si tout ce qui suit est vrai : l'indicateur « holds » est activé et le property est « attends » ; dans tous les autres cas, 0. | _Order 16. 1 for a holding attends link._ |
| First Fired Order Key | La plus petite valeur de order key sur les firings du statement qui sont fired unit. | _Order 15. Order key of the first rule application that concludes this fact, in the page reasoner's order._ |
| Count of Chosen Firings | Le total de chosen unit sur les firings liés à le statement. | _Order 20. Rule applications shown as this fact's conclusion card: one for a conclusion, zero otherwise._ |

<details open>
<summary>Données d'exemple</summary>

| Object Value | Is Listed | Is Built in | Is Detail | Is Stated | Activation Seq | Sort Order | Subject Label ƒ | Property Label ƒ | Object Label ƒ | Property Datatype ƒ | Property Has One Current Value ƒ | Property Domain Class ƒ | Property Range Class ƒ | Literal Quote ƒ | Literal Type Marker ƒ | Built in Source Text ƒ | User Source Text ƒ | Next Fact Sort Order ƒ | Is Literal ƒ | Object Text ƒ | Name ƒ | Words ƒ | Source ƒ | Subject Property Key ƒ | Latest Sibling Activation Seq ƒ | Is in Effect ƒ | Listed Unit ƒ | In Effect Unit ƒ | Listed Off Unit ƒ | Is Next to Add ƒ | Count of Inverse Firings ƒ | Count of Symmetric Firings ƒ | Holds Before Chain ƒ | Count of Chain Firings ƒ | Holds Before Typing ƒ | Count of Type Firings ƒ | Holds ƒ | Is Conclusion ƒ | Holds Unit ƒ | Conclusion Unit ƒ | Parent Unit ƒ | Grandparent Unit ƒ | Place Unit ƒ | First Fired Order Key ƒ | Count of Chosen Firings ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| — | true | true | false | true | 1 | 1 | Marvin | is parent of | Randy | — | false | — | — | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | — | false | Randy | Marvin is parent of Randy | Marvin is parent of Randy | Stated as a teaching example on this page. | Marvin\|isParentOf | 1 | true | 1 | 1 | 0 | false | 0 | 0 | true | 0 | true | 0 | true | false | 1 | 0 | 1 | 0 | 0 | — | 0 |
| — | false | false | false | false | 0 | 101 | Marvin | is parent of | Sharon | — | false | — | — | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | — | false | Sharon | Marvin is parent of Sharon | Marvin is parent of Sharon | Stated by you on this page. | Marvin\|isParentOf | 1 | false | 0 | 0 | 0 | false | 0 | 0 | false | 0 | false | 0 | false | false | 0 | 0 | 0 | 0 | 0 | — | 0 |
| — | false | false | false | false | 0 | 102 | Marvin | is parent of | Stan | — | false | — | — | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | — | false | Stan | Marvin is parent of Stan | Marvin is parent of Stan | Stated by you on this page. | Marvin\|isParentOf | 1 | false | 0 | 0 | 0 | false | 0 | 0 | false | 0 | false | 0 | false | false | 0 | 0 | 0 | 0 | 0 | — | 0 |

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
| Second Sort Order | Repris du second lié. | _Order 1. The second premise's list position._ |
| Inverse Fired Unit | Déterminé par priorité : 1 si tout ce qui suit est vrai : l'indicateur « rule enabled » est activé; le kind est « inverse »; et l'indicateur « first is in effect » est activé ; dans tous les autres cas, 0. | _Order 5. 1 when the inverse rule fires here._ |
| Symmetric Fired Unit | Déterminé par priorité : 1 si tout ce qui suit est vrai : l'indicateur « rule enabled » est activé; le kind est « symmetric »; et l'indicateur « first is in effect » est activé ; dans tous les autres cas, 0. | _Order 5. 1 when the symmetric rule fires here._ |
| Chain Fired Unit | Déterminé par priorité : 1 si tout ce qui suit est vrai : l'indicateur « rule enabled » est activé; le kind est « chain »; l'indicateur « first holds before chain » est activé; et l'indicateur « second holds before chain » est activé ; dans tous les autres cas, 0. | _Order 9. 1 when the property chain fires here._ |
| Type Fired Unit | Déterminé par priorité : 1 si tout ce qui suit est vrai : l'indicateur « rule enabled » est activé; au moins une des conditions suivantes est vraie : le kind est « domain » ou le kind est « range »; et l'indicateur « first holds before typing » est activé ; dans tous les autres cas, 0. | _Order 13. 1 when the domain or range rule fires here._ |
| Fired Unit | Calculé comme l'inverse fired unit plus le symmetric fired unit plus le chain fired unit plus le type fired unit. | _Order 14. 1 when this application fires._ |
| Order Key | Calculé comme le first sort order multiplié par 100000 plus le kind sort order multiplié par 1000 plus 0 si le second est vide, dans tous les autres cas le second sort order. | _Order 2. The page reasoner's order: by premise, then rule kind, then second premise._ |
| Is Chosen | Vrai lorsque tout ce qui suit est vrai : le fired unit est 1; l'indicateur « target is conclusion » est activé; et l'order key est le target first fired order key. | _Order 18. The application the conclusions panel shows for its fact: the first one in the reasoner's order._ |
| Chosen Unit | Déterminé par priorité : 1 si l'indicateur « chosen » est activé ; dans tous les autres cas, 0. | _Order 19. 1 when this application is shown._ |
| Card Text | Déterminé par priorité : « Worked out by the », suivi de le kind wording, suivi de « from “ », suivi de le first words, suivi de « ” », suivi de « and “ », suivi de le second words, suivi de « ” » si le kind est « chain », dans tous les autres cas « and “ », suivi de le first property, suivi de « has domain », suivi de le first property domain class, suivi de « ” » si le kind est « domain », dans tous les autres cas « and “ », suivi de le first property, suivi de « has range », suivi de le first property range class, suivi de « ” », suivi de un point si l'indicateur « chosen » est activé ; dans tous les autres cas, une chaîne vide. ⚠︎ mécanique <!-- rulespeak:reword --> | _Order 19. The conclusion card's provenance line, in the page's wording._ |
| Name | Identique à firing ID. | _Order 1. The application's key._ |

<details open>
<summary>Données d'exemple</summary>

| Rule Enabled ƒ | Kind Wording ƒ | Kind Sort Order ƒ | Target Name ƒ | Target Subject Label ƒ | Target Property Label ƒ | Target Object Text ƒ | Target is Conclusion ƒ | Target First Fired Order Key ƒ | First Words ƒ | Second Words ƒ | First is in Effect ƒ | First Holds Before Chain ƒ | Second Holds Before Chain ƒ | First Holds Before Typing ƒ | First Property ƒ | First Property Domain Class ƒ | First Property Range Class ƒ | First Sort Order ƒ | Second Sort Order ƒ | Inverse Fired Unit ƒ | Symmetric Fired Unit ƒ | Chain Fired Unit ƒ | Type Fired Unit ƒ | Fired Unit ƒ | Order Key ƒ | Is Chosen ƒ | Chosen Unit ƒ | Card Text ƒ | Name ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| false | inverse rule | 1 | Randy is child of Marvin | Randy | is child of | Marvin | false | — | Marvin is parent of Randy |    | true | true | false | true | isParentOf | — | — | 1 | — | 0 | 0 | 0 | 0 | 0 | 101000 | false | 0 | — | inverse:Marvin\|isParentOf\|Randy>Randy\|isChildOf\|Marvin |
| false | inverse rule | 1 | Sharon is child of Marvin | Sharon | is child of | Marvin | false | — | Marvin is parent of Sharon |    | false | false | false | false | isParentOf | — | — | 101 | — | 0 | 0 | 0 | 0 | 0 | 10101000 | false | 0 | — | inverse:Marvin\|isParentOf\|Sharon>Sharon\|isChildOf\|Marvin |
| false | inverse rule | 1 | Stan is child of Marvin | Stan | is child of | Marvin | false | — | Marvin is parent of Stan |    | false | false | false | false | isParentOf | — | — | 102 | — | 0 | 0 | 0 | 0 | 0 | 10201000 | false | 0 | — | inverse:Marvin\|isParentOf\|Stan>Stan\|isChildOf\|Marvin |

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
| false | true | false | Marvin | Randy | 0 | — | Marvin\|isParentOf\|Randy~isSpouseOf |
| false | false | false | Marvin | Sharon | 0 | — | Marvin\|isParentOf\|Sharon~isSpouseOf |
| false | false | false | Marvin | Stan | 0 | — | Marvin\|isParentOf\|Stan~isSpouseOf |

_ƒ signale une colonne calculée._

</details>

## 2 Types de faits

- un **firing kind** référence exactement un **rule**
- un **property** peut référencer un **property**
- un **property** peut référencer un **thing**
- un **statement** référence exactement un **graph**
- un **statement** référence exactement un **thing**
- un **statement** référence exactement un **property**
- un **firing** référence exactement un **firing kind**
- un **firing** référence exactement un **rule**
- un **firing** référence exactement un **statement**
- un **disjoint check** référence exactement un **graph**
- un **disjoint check** référence exactement un **rule**
- un **disjoint check** référence exactement un **statement**

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
- Un property **doit** avoir un label; un property kind; et un sort order, et consigner si il est user addable; si il a un one current value; et si il est symmetric.
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
| **DR-14 Name** | Un rule : name est identique à label. |
| **DR-15 Count of Firings** | Un rule : count of firings est le total de fired unit sur les firings liés à le rule. |
| **DR-16 Name** | Un firing kind : name est identique à wording. |
| **DR-17 Rule Enabled** | Un firing kind : rule enabled est vrai lorsque le rule lié est enabled. |
| **DR-18 Name** | Un thing : name est identique à label. |
| **DR-19 Count of Children** | Un thing : count of children est le total de parent unit sur les statements liés à le thing. |
| **DR-20 Count of Grandchildren** | Un thing : count of grandchildren est le total de grandparent unit sur les statements liés à le thing. |
| **DR-21 Count As Place** | Un thing : count as place est le total de place unit sur les statements liés à le thing. |
| **DR-22 Count As Subject** | Un thing : count as subject est le total de holds unit sur les statements liés à le thing. |
| **DR-23 Count As Object** | Un thing : count as object est le total de holds unit sur les statements liés à le thing. |
| **DR-24 Is in Graph** | Un thing est considéré comme in graph si au moins une des conditions suivantes est vraie : le count as subject est supérieur à 0 ou le count as object est supérieur à 0. |
| **DR-25 Node Kind** | Le champ « node kind » du thing est déterminé selon la priorité suivante :<br>1. « class », si le kind est « class » ;<br>2. « place », si le count as place est supérieur à 0 ;<br>3. dans tous les autres cas, « person ». |
| **DR-26 Name** | Un property : name est identique à label. |
| **DR-27 Subject Label** | Un statement : subject label — repris du subject lié. |
| **DR-28 Property Label** | Un statement : property label — repris du property lié. |
| **DR-29 Object Label** | Un statement : object label — repris du object thing lié. |
| **DR-30 Property Datatype** | Un statement : property datatype — repris du property lié. |
| **DR-31 Property Has One Current Value** | Un statement : property has one current value est vrai lorsque le property lié a un one current value. |
| **DR-32 Property Domain Class** | Un statement : property domain class — repris du property lié. |
| **DR-33 Property Range Class** | Un statement : property range class — repris du property lié. |
| **DR-34 Literal Quote** | Un statement : literal quote — repris du graph lié. |
| **DR-35 Literal Type Marker** | Un statement : literal type marker — repris du graph lié. |
| **DR-36 Built in Source Text** | Un statement : built in source text — repris du graph lié. |
| **DR-37 User Source Text** | Un statement : user source text — repris du graph lié. |
| **DR-38 Next Fact Sort Order** | Un statement : next fact sort order — repris du graph lié. |
| **DR-39 Is Literal** | Un statement est considéré comme literal si l'object thing est vide. |
| **DR-40 Object Text** | Le champ « object text » du statement est déterminé selon la priorité suivante :<br>1. l'object value, si l'object thing est vide ;<br>2. dans tous les autres cas, l'object label. |
| **DR-41 Name** | Un statement : name est calculé comme le subject label, suivi de une espace, suivi de le property label, suivi de une espace, suivi de l'object text. |
| **DR-42 Words** | Un statement : words est calculé comme le subject label, suivi de une espace, suivi de le property label, suivi de une espace, suivi de le literal quote, suivi de l'object value, suivi de le literal quote, suivi de le literal type marker, suivi de le property datatype si l'object thing est vide, dans tous les autres cas l'object label. ⚠︎ mécanique <!-- rulespeak:reword --> |
| **DR-43 Source** | Le champ « source » du statement est déterminé selon la priorité suivante :<br>1. le built in source text, si l'indicateur « built in » est activé ;<br>2. dans tous les autres cas, l'user source text. |
| **DR-44 Subject Property Key** | Un statement : subject property key est calculé comme le subject, suivi de « | », suivi de le property. |
| **DR-45 Latest Sibling Activation Seq** | Un statement : latest sibling activation seq est la plus grande valeur de activation seq sur les statements liés à le statement. |
| **DR-46 Is in Effect** | Un statement est considéré comme in effect si tout ce qui suit est vrai : l'indicateur « stated » est activé et au moins une des conditions suivantes est vraie : l'indicateur « property has one current value » n'est pas activé ou l'activation seq est le latest sibling activation seq. |
| **DR-47 Listed Unit** | Le champ « listed unit » du statement est déterminé selon la priorité suivante :<br>1. 1, si l'indicateur « listed » est activé ;<br>2. dans tous les autres cas, 0. |
| **DR-48 In Effect Unit** | Le champ « in effect unit » du statement est déterminé selon la priorité suivante :<br>1. 1, si l'indicateur « in effect » est activé ;<br>2. dans tous les autres cas, 0. |
| **DR-49 Listed Off Unit** | Le champ « listed off unit » du statement est déterminé selon la priorité suivante :<br>1. 1, si tout ce qui suit est vrai : l'indicateur « listed » est activé et l'indicateur « in effect » n'est pas activé ;<br>2. dans tous les autres cas, 0. |
| **DR-50 Is Next to Add** | Un statement est considéré comme un next to add si tout ce qui suit est vrai : le listed off unit est 1 et le sort order est le next fact sort order. |
| **DR-51 Count of Inverse Firings** | Un statement : count of inverse firings est le total de inverse fired unit sur les firings liés à le statement. |
| **DR-52 Count of Symmetric Firings** | Un statement : count of symmetric firings est le total de symmetric fired unit sur les firings liés à le statement. |
| **DR-53 Holds Before Chain** | Un statement est considéré comme satisfaisant : before chain si au moins une des conditions suivantes est vraie : l'indicateur « in effect » est activé; le count of inverse firings est supérieur à 0; ou le count of symmetric firings est supérieur à 0. |
| **DR-54 Count of Chain Firings** | Un statement : count of chain firings est le total de chain fired unit sur les firings liés à le statement. |
| **DR-55 Holds Before Typing** | Un statement est considéré comme satisfaisant : before typing si au moins une des conditions suivantes est vraie : l'indicateur « holds before chain » est activé ou le count of chain firings est supérieur à 0. |
| **DR-56 Count of Type Firings** | Un statement : count of type firings est le total de type fired unit sur les firings liés à le statement. |
| **DR-57 Holds** | Un statement est considéré comme satisfaisant : si au moins une des conditions suivantes est vraie : l'indicateur « holds before typing » est activé ou le count of type firings est supérieur à 0. |
| **DR-58 Is Conclusion** | Un statement est considéré comme une conclusion si tout ce qui suit est vrai : l'indicateur « holds » est activé et l'indicateur « in effect » n'est pas activé. |
| **DR-59 Holds Unit** | Le champ « holds unit » du statement est déterminé selon la priorité suivante :<br>1. 1, si l'indicateur « holds » est activé ;<br>2. dans tous les autres cas, 0. |
| **DR-60 Conclusion Unit** | Le champ « conclusion unit » du statement est déterminé selon la priorité suivante :<br>1. 1, si l'indicateur « conclusion » est activé ;<br>2. dans tous les autres cas, 0. |
| **DR-61 Parent Unit** | Le champ « parent unit » du statement est déterminé selon la priorité suivante :<br>1. 1, si tout ce qui suit est vrai : l'indicateur « holds » est activé et le property est « isParentOf » ;<br>2. dans tous les autres cas, 0. |
| **DR-62 Grandparent Unit** | Le champ « grandparent unit » du statement est déterminé selon la priorité suivante :<br>1. 1, si tout ce qui suit est vrai : l'indicateur « holds » est activé et le property est « isGrandparentOf » ;<br>2. dans tous les autres cas, 0. |
| **DR-63 Place Unit** | Le champ « place unit » du statement est déterminé selon la priorité suivante :<br>1. 1, si tout ce qui suit est vrai : l'indicateur « holds » est activé et le property est « attends » ;<br>2. dans tous les autres cas, 0. |
| **DR-64 First Fired Order Key** | Un statement : first fired order key est la plus petite valeur de order key sur les firings du statement qui sont fired unit. |
| **DR-65 Count of Chosen Firings** | Un statement : count of chosen firings est le total de chosen unit sur les firings liés à le statement. |
| **DR-66 Rule Enabled** | Un firing : rule enabled est vrai lorsque le rule lié est enabled. |
| **DR-67 Kind Wording** | Un firing : kind wording — repris du kind lié. |
| **DR-68 Kind Sort Order** | Un firing : kind sort order — repris du kind lié. |
| **DR-69 Target Name** | Un firing : target name — repris du target lié. |
| **DR-70 Target Subject Label** | Un firing : target subject label — repris du target lié. |
| **DR-71 Target Property Label** | Un firing : target property label — repris du target lié. |
| **DR-72 Target Object Text** | Un firing : target object text — repris du target lié. |
| **DR-73 Target is Conclusion** | Un firing : target is conclusion est vrai lorsque le target lié est une conclusion. |
| **DR-74 Target First Fired Order Key** | Un firing : target first fired order key — repris du target lié. |
| **DR-75 First Words** | Un firing : first words — repris du first lié. |
| **DR-76 Second Words** | Un firing : second words — repris du second lié. |
| **DR-77 First is in Effect** | Un firing : first is in effect est vrai lorsque le first lié est in effect. |
| **DR-78 First Holds Before Chain** | Un firing : first holds before chain est vrai lorsque le first lié est holds before chain. |
| **DR-79 Second Holds Before Chain** | Un firing : second holds before chain est vrai lorsque le second lié est holds before chain. |
| **DR-80 First Holds Before Typing** | Un firing : first holds before typing est vrai lorsque le first lié est holds before typing. |
| **DR-81 First Property** | Un firing : first property — repris du first lié. |
| **DR-82 First Property Domain Class** | Un firing : first property domain class — repris du first lié. |
| **DR-83 First Property Range Class** | Un firing : first property range class — repris du first lié. |
| **DR-84 First Sort Order** | Un firing : first sort order — repris du first lié. |
| **DR-85 Second Sort Order** | Un firing : second sort order — repris du second lié. |
| **DR-86 Inverse Fired Unit** | Le champ « inverse fired unit » du firing est déterminé selon la priorité suivante :<br>1. 1, si tout ce qui suit est vrai : l'indicateur « rule enabled » est activé; le kind est « inverse »; et l'indicateur « first is in effect » est activé ;<br>2. dans tous les autres cas, 0. |
| **DR-87 Symmetric Fired Unit** | Le champ « symmetric fired unit » du firing est déterminé selon la priorité suivante :<br>1. 1, si tout ce qui suit est vrai : l'indicateur « rule enabled » est activé; le kind est « symmetric »; et l'indicateur « first is in effect » est activé ;<br>2. dans tous les autres cas, 0. |
| **DR-88 Chain Fired Unit** | Le champ « chain fired unit » du firing est déterminé selon la priorité suivante :<br>1. 1, si tout ce qui suit est vrai : l'indicateur « rule enabled » est activé; le kind est « chain »; l'indicateur « first holds before chain » est activé; et l'indicateur « second holds before chain » est activé ;<br>2. dans tous les autres cas, 0. |
| **DR-89 Type Fired Unit** | Le champ « type fired unit » du firing est déterminé selon la priorité suivante :<br>1. 1, si tout ce qui suit est vrai : l'indicateur « rule enabled » est activé; au moins une des conditions suivantes est vraie : le kind est « domain » ou le kind est « range »; et l'indicateur « first holds before typing » est activé ;<br>2. dans tous les autres cas, 0. |
| **DR-90 Fired Unit** | Un firing : fired unit est calculé comme l'inverse fired unit plus le symmetric fired unit plus le chain fired unit plus le type fired unit. |
| **DR-91 Order Key** | Un firing : order key est calculé comme le first sort order multiplié par 100000 plus le kind sort order multiplié par 1000 plus 0 si le second est vide, dans tous les autres cas le second sort order. |
| **DR-92 Is Chosen** | Un firing est considéré comme un chosen si tout ce qui suit est vrai : le fired unit est 1; l'indicateur « target is conclusion » est activé; et l'order key est le target first fired order key. |
| **DR-93 Chosen Unit** | Le champ « chosen unit » du firing est déterminé selon la priorité suivante :<br>1. 1, si l'indicateur « chosen » est activé ;<br>2. dans tous les autres cas, 0. |
| **DR-94 Card Text** | Le champ « card text » du firing est déterminé selon la priorité suivante :<br>1. « Worked out by the », suivi de le kind wording, suivi de « from “ », suivi de le first words, suivi de « ” », suivi de « and “ », suivi de le second words, suivi de « ” » si le kind est « chain », dans tous les autres cas « and “ », suivi de le first property, suivi de « has domain », suivi de le first property domain class, suivi de « ” » si le kind est « domain », dans tous les autres cas « and “ », suivi de le first property, suivi de « has range », suivi de le first property range class, suivi de « ” », suivi de un point, si l'indicateur « chosen » est activé ;<br>2. dans tous les autres cas, une chaîne vide. ⚠︎ mécanique <!-- rulespeak:reword --> |
| **DR-95 Name** | Un firing : name est identique à firing ID. |
| **DR-96 Rule Enabled** | Un disjoint check : rule enabled est vrai lorsque le rule lié est enabled. |
| **DR-97 First Holds** | Un disjoint check : first holds est vrai lorsque le first lié est holds. |
| **DR-98 Second Holds** | Un disjoint check : second holds est vrai lorsque le second lié est holds. |
| **DR-99 Subject Label** | Un disjoint check : subject label — repris du first lié. |
| **DR-100 Object Label** | Un disjoint check : object label — repris du first lié. |
| **DR-101 Violation Unit** | Le champ « violation unit » du disjoint check est déterminé selon la priorité suivante :<br>1. 1, si tout ce qui suit est vrai : l'indicateur « rule enabled » est activé; l'indicateur « first holds » est activé; et l'indicateur « second holds » est activé ;<br>2. dans tous les autres cas, 0. |
| **DR-102 Alarm Text** | Le champ « alarm text » du disjoint check est déterminé selon la priorité suivante :<br>1. « Contradiction found. », suivi de le subject label, suivi de « is recorded as both parent and spouse of », suivi de l'object label, suivi de «. Check those facts and the selected disjoint rule. », si le violation unit est 1 ;<br>2. dans tous les autres cas, une chaîne vide. |
| **DR-103 Name** | Un disjoint check : name est identique à disjoint check ID. |

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
| **Rules.Name** | formule | `Label` |
| **Rules.CountOfFirings** | agrégat | `Sum(Firings.FiredUnit via Rule)` |
| **FiringKinds.Name** | formule | `Wording` |
| **FiringKinds.RuleEnabled** | recherche | `Lookup(Rules.IsEnabled via Rule)` |
| **Things.Name** | formule | `Label` |
| **Things.CountOfChildren** | agrégat | `Sum(Statements.ParentUnit via Subject)` |
| **Things.CountOfGrandchildren** | agrégat | `Sum(Statements.GrandparentUnit via Subject)` |
| **Things.CountAsPlace** | agrégat | `Sum(Statements.PlaceUnit via ObjectThing)` |
| **Things.CountAsSubject** | agrégat | `Sum(Statements.HoldsUnit via Subject)` |
| **Things.CountAsObject** | agrégat | `Sum(Statements.HoldsUnit via ObjectThing)` |
| **Things.IsInGraph** | formule | `Or(CountAsSubject > 0, CountAsObject > 0)` |
| **Things.NodeKind** | formule | `If(Kind = "class", "class", If(CountAsPlace > 0, "place", "person"))` |
| **Properties.Name** | formule | `Label` |
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
| **Statements.HoldsBeforeChain** | formule | `Or(IsInEffect, CountOfInverseFirings > 0, CountOfSymmetricFirings > 0)` |
| **Statements.CountOfChainFirings** | agrégat | `Sum(Firings.ChainFiredUnit via Target)` |
| **Statements.HoldsBeforeTyping** | formule | `Or(HoldsBeforeChain, CountOfChainFirings > 0)` |
| **Statements.CountOfTypeFirings** | agrégat | `Sum(Firings.TypeFiredUnit via Target)` |
| **Statements.Holds** | formule | `Or(HoldsBeforeTyping, CountOfTypeFirings > 0)` |
| **Statements.IsConclusion** | formule | `And(Holds, Not(IsInEffect))` |
| **Statements.HoldsUnit** | formule | `If(Holds, 1, 0)` |
| **Statements.ConclusionUnit** | formule | `If(IsConclusion, 1, 0)` |
| **Statements.ParentUnit** | formule | `If(And(Holds, Property = "isParentOf"), 1, 0)` |
| **Statements.GrandparentUnit** | formule | `If(And(Holds, Property = "isGrandparentOf"), 1, 0)` |
| **Statements.PlaceUnit** | formule | `If(And(Holds, Property = "attends"), 1, 0)` |
| **Statements.FirstFiredOrderKey** | agrégat | `Min(Firings.OrderKey via Target)` |
| **Statements.CountOfChosenFirings** | agrégat | `Sum(Firings.ChosenUnit via Target)` |
| **Firings.RuleEnabled** | recherche | `Lookup(Rules.IsEnabled via Rule)` |
| **Firings.KindWording** | recherche | `Lookup(FiringKinds.Wording via Kind)` |
| **Firings.KindSortOrder** | recherche | `Lookup(FiringKinds.SortOrder via Kind)` |
| **Firings.TargetName** | recherche | `Lookup(Statements.Name via Target)` |
| **Firings.TargetSubjectLabel** | recherche | `Lookup(Statements.SubjectLabel via Target)` |
| **Firings.TargetPropertyLabel** | recherche | `Lookup(Statements.PropertyLabel via Target)` |
| **Firings.TargetObjectText** | recherche | `Lookup(Statements.ObjectText via Target)` |
| **Firings.TargetIsConclusion** | recherche | `Lookup(Statements.IsConclusion via Target)` |
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
| **Firings.SecondSortOrder** | recherche | `Lookup(Statements.SortOrder via Second)` |
| **Firings.InverseFiredUnit** | formule | `If(And(RuleEnabled, Kind = "inverse", FirstIsInEffect), 1, 0)` |
| **Firings.SymmetricFiredUnit** | formule | `If(And(RuleEnabled, Kind = "symmetric", FirstIsInEffect), 1, 0)` |
| **Firings.ChainFiredUnit** | formule | `If(And(RuleEnabled, Kind = "chain", FirstHoldsBeforeChain, SecondHoldsBeforeChain), 1, 0)` |
| **Firings.TypeFiredUnit** | formule | `If(And(RuleEnabled, Or(Kind = "domain", Kind = "range"), FirstHoldsBeforeTyping), 1, 0)` |
| **Firings.FiredUnit** | formule | `InverseFiredUnit + SymmetricFiredUnit + ChainFiredUnit + TypeFiredUnit` |
| **Firings.OrderKey** | formule | `FirstSortOrder * 100000 + KindSortOrder * 1000 + If(Second = "", 0, SecondSortOrder)` |
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

---

> **RuleSpeak®** — la notation déclarative de règles métier créée par **Ronald G. Ross**, selon les conventions de **SBVR** — arrive bientôt. D’ici là, cette page présente la documentation habituelle. [www.RonRoss.info](https://www.RonRoss.info)
