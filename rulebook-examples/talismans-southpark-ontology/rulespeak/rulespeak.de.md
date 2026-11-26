# 📘 Talisman's South Park ontology explainer — Dokumentation

_Jessica Talisman's South Park Ontology Explainer, as a rulebook: the page's ten facts, five rule switches, property wiring, provenance wording and current-value policy are rows, and every conclusion is a formula. The rules apply in three fixed strata (inverse and symmetric, then the property chain, then domain and range); property wiring that feeds an earlier stratum would need repeated rounds. Every fact that could hold over the cast is a candidate row; app/lib/candidates.mjs writes the candidates when a thing, value or property is added._

> Deklarative Geschäftsregeln, aus dem Regelbuch gerendert. Jede Aussage
> unten drückt eine Wahrheit der Geschäftsdomäne aus — sie ist weder eine
> Prozedur noch ein Imperativ. Die Formeln des Regelbuchs sind die einzige
> Quelle der Wahrheit; dieses Dokument ist ihre klarsprachliche Lesart.

## 1 Geschäftsvokabular

| Begriff | Beschreibung | Erläuternder Kommentar |
|------|-------------|-------------------|
| **Graph** | The explainer page as one graph: its wording variables and its totals. | — |
| Label | Ein definiertes Attribut. | _Display name of the graph._ |
| Literal Quote | Ein definiertes Attribut. | _The quote mark written around a literal value in a fact's wording._ |
| Literal Type Marker | Ein definiertes Attribut. | _The marker written between a literal value and its datatype, as in Turtle._ |
| Built in Source Text | Ein definiertes Attribut. | _Source line shown for a fact the page states as a teaching example._ |
| User Source Text | Ein definiertes Attribut. | _Source line shown for a fact the reader stated._ |
| No Conclusions Text | Ein definiertes Attribut. | _Message shown when the selected facts and rules add no conclusion._ |
| Name | Entspricht Label. | _Order 1. Display name of the graph._ |
| Count of Stated Facts | Die Gesamtsumme von In Effect Unit über die mit dem Graph verbundenen Statements. | _Order 5. Facts that are switched on and in effect._ |
| Count of Conclusions | Die Gesamtsumme von Conclusion Unit über die mit dem Graph verbundenen Statements. | _Order 18. Facts that hold only because an enabled rule concluded them._ |
| Count of Violations | Die Gesamtsumme von Violation Unit über die mit dem Graph verbundenen Disjoint Checks. | _Order 18. Disjointness checks that found both links between the same pair._ |
| Is Consistent | Wahr, wenn der Count of Violations 0 ist. | _Order 19. True when no enabled disjointness rule is violated._ |
| Conclusions Message | Nach Priorität bestimmt: der No Conclusions Text, wenn der Count of Conclusions 0 ist; andernfalls eine leere Zeichenkette. | _Order 19. The empty-state message for the conclusions panel, blank when there are conclusions._ |
| Count of Facts to Add | Die Gesamtsumme von Listed Off Unit über die mit dem Graph verbundenen Statements. | _Order 5. Listed facts that are not in effect; the Add next fact button is enabled while this is above zero._ |
| Has Facts to Add | Wahr, wenn der Count of Facts to Add größer ist als 0. | _Order 6. True while some listed fact can still be added._ |
| Next Fact Sort Order | Der kleinste Wert von Sort Order über die Statements des Graph, die Listed Off Unit sind. | _Order 5. List position of the first listed fact that is not in effect._ |
| Max Activation Seq | Der größte Wert von Activation Seq über die mit dem Graph verbundenen Statements. | _Order 1. Highest activation number used so far._ |
| Next Activation Seq | Berechnet als der Max Activation Seq plus 1. | _Order 2. The activation number the next switched-on fact takes._ |
| Max Listed Sort Order | Der größte Wert von Sort Order über die Statements des Graph, die Listed Unit sind. | _Order 2. Highest list position among listed facts._ |
| Next Listed Sort Order | Berechnet als der Max Listed Sort Order plus 1. | _Order 3. The list position a newly added fact takes._ |
| Max Sort Order | Der größte Wert von Sort Order über die mit dem Graph verbundenen Statements. | _Order 1. Highest sort order of any fact, listed or candidate._ |
| Next Candidate Sort Order | Berechnet als der Max Sort Order plus 1. | _Order 2. The sort order a new candidate fact, or a fact removed from the list, takes._ |

<details open>
<summary>Beispieldaten</summary>

| Label | Literal Quote | Literal Type Marker | Built in Source Text | User Source Text | No Conclusions Text | Name ƒ | Count of Stated Facts ƒ | Count of Conclusions ƒ | Count of Violations ƒ | Is Consistent ƒ | Conclusions Message ƒ | Count of Facts to Add ƒ | Has Facts to Add ƒ | Next Fact Sort Order ƒ | Max Activation Seq ƒ | Next Activation Seq ƒ | Max Listed Sort Order ƒ | Next Listed Sort Order ƒ | Max Sort Order ƒ | Next Candidate Sort Order ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| South Park | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | These facts and selected rules add no further conclusions. Try another fact or rule. | South Park | 10 | 0 | 0 | true | These facts and selected rules add no further conclusions. Try another fact or rule. | 0 | false | — | 10 | 11 | 10 | 11 | 100269 | 100270 |

_ƒ kennzeichnet eine berechnete Spalte._

</details>

| Begriff | Beschreibung | Erläuternder Kommentar |
|------|-------------|-------------------|
| **Rule** | The five rule switches on the page. | — |
| Label | Ein definiertes Attribut. | _The rule's name on the page._ |
| Explanation | Ein definiertes Attribut. | _The rule's one-line explanation on the page._ |
| Is Enabled | Wahr, wenn eine leere Zeichenkette. | _The rule's checkbox._ |
| Sort Order | Ein definiertes Attribut. | _Position in the rule list._ |
| Name | Entspricht Label. | _Order 1. The rule's name._ |
| Count of Firings | Die Gesamtsumme von Fired Unit über die mit der Rule verbundenen Firings. | _Order 15. Rule applications that fired, counting every derivation._ |

<details open>
<summary>Beispieldaten</summary>

| Label | Explanation | Is Enabled | Sort Order | Name ƒ | Count of Firings ƒ |
|---|---|---|---|---|---|
| Inverse | A parent link also gives a child link between the same two people. | false | 1 | Inverse | 0 |
| Symmetric | A friend link or a spouse link is true for both people. | false | 2 | Symmetric | 0 |
| Property chain | A parent of a parent is a grandparent. | false | 3 | Property chain | 0 |

_ƒ kennzeichnet eine berechnete Spalte._

</details>

| Begriff | Beschreibung | Erläuternder Kommentar |
|------|-------------|-------------------|
| **Firing Kind** | The ways a rule can apply. Domain and range belong to one rule but are worded apart; 'disjoint' names the rule the disjointness checks answer to. | — |
| Rule | Ein definiertes Attribut. | _The rule switch this kind belongs to._ |
| Wording | Ein definiertes Attribut. | _How a conclusion card names the rule, as in 'Worked out by the inverse rule'._ |
| Sort Order | Ein definiertes Attribut. | _Order in which the page's reasoner tries the kinds for one fact._ |
| Name | Entspricht Wording. | _Order 1. The kind's wording._ |
| Rule Enabled | Wahr, wenn der zugehörige Rule des Firing Kind ist enabled. | _Order 1. Whether the owning rule is switched on._ |

<details open>
<summary>Beispieldaten</summary>

| Wording | Sort Order | Name ƒ | Rule Enabled ƒ |
|---|---|---|---|
| inverse rule | 1 | inverse rule | false |
| symmetric rule | 2 | symmetric rule | false |
| property chain rule | 3 | property chain rule | false |

_ƒ kennzeichnet eine berechnete Spalte._

</details>

| Begriff | Beschreibung | Erläuternder Kommentar |
|------|-------------|-------------------|
| **Thing** | Every individual and class the page can draw. | — |
| Label | Ein definiertes Attribut. | _The name the page shows._ |
| Kind | Ein definiertes Attribut. | _'individual' or 'class'._ |
| Sort Order | Ein definiertes Attribut. | _Position in the cast._ |
| Portrait | Ein definiertes Attribut. | _The portrait drawn for this thing; set when its name matches a portrait alias._ |
| Portrait Name | Der Display Name des zugehörigen Portrait des Thing. | _Order 1. The name on the portrait's Fandom page._ |
| Portrait Page | Aus dem zugehörigen Portrait übernommen. | _Order 1. The Fandom page credited for the portrait._ |
| Portrait Image | Aus dem zugehörigen Portrait übernommen. | _Order 1. The original Fandom image._ |
| Portrait Crop | Aus dem zugehörigen Portrait übernommen. | _Order 1. The portrait's crop as an SVG viewBox._ |
| Portrait Width | Aus dem zugehörigen Portrait übernommen. | _Order 1. The original image's width in pixels._ |
| Portrait Height | Aus dem zugehörigen Portrait übernommen. | _Order 1. The original image's height in pixels._ |
| Name | Entspricht Label. | _Order 1. The thing's name._ |
| Node Key | Entspricht Thing ID. | _Order 1. The key the graph draws this thing under._ |
| Count of Children | Die Gesamtsumme von Parent Unit über die mit dem Thing verbundenen Statements. | _Order 17. People this thing is a parent of, stated or concluded._ |
| Count of Grandchildren | Die Gesamtsumme von Grandparent Unit über die mit dem Thing verbundenen Statements. | _Order 17. People this thing is a grandparent of._ |
| Count As Place | Die Gesamtsumme von Place Unit über die mit dem Thing verbundenen Statements. | _Order 17. Holding attends facts whose object is this thing._ |
| Count As Subject | Die Gesamtsumme von Holds Unit über die mit dem Thing verbundenen Statements. | _Order 17. Holding facts about this thing._ |
| Count As Object | Die Gesamtsumme von Holds Unit über die mit dem Thing verbundenen Statements. | _Order 17. Holding facts that point at this thing._ |
| Is in Graph | Wahr, wenn mindestens eines des Folgenden gilt: der Count As Subject größer ist als 0 oder der Count As Object größer ist als 0. | _Order 18. True when the graph draws this thing._ |
| Node Kind | Nach Priorität bestimmt: eine leere Zeichenkette, wenn der Kind leer ist; „class“, wenn der Kind „class“ ist; „place“, wenn die Count As Place größer ist als 0; andernfalls „person“. | _Order 18. How the graph draws the thing: a class, a place someone attends, or a person._ |

<details open>
<summary>Beispieldaten</summary>

| Label | Kind | Sort Order | Portrait Name ƒ | Portrait Page ƒ | Portrait Image ƒ | Portrait Crop ƒ | Portrait Width ƒ | Portrait Height ƒ | Name ƒ | Node Key ƒ | Count of Children ƒ | Count of Grandchildren ƒ | Count As Place ƒ | Count As Subject ƒ | Count As Object ƒ | Is in Graph ƒ | Node Kind ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| Marvin | individual | 1 | Marvin Marsh | https://southpark.fandom.com/wiki/Marvin_Marsh | https://static.wikia.nocookie.net/southpark/images/d/db/Marsh-family-grandpa-marsh-w-purple-blanket.png/revision/latest | 48 -5 280 280 | 375 | 536 | Marvin | Marvin | 1 | 0 | 0 | 1 | 0 | true | person |
| Randy | individual | 2 | Randy Marsh | https://southpark.fandom.com/wiki/Randy_Marsh | https://static.wikia.nocookie.net/southparkfanon/images/d/d6/Randy_Marsh.png/revision/latest | 0 -10 428 428 | 428 | 1057 | Randy | Randy | 2 | 0 | 0 | 3 | 1 | true | person |
| Sharon | individual | 3 | Sharon Marsh | https://southpark.fandom.com/wiki/Sharon_Marsh | https://static.wikia.nocookie.net/southpark/images/9/99/Sharon_Marsh.png/revision/latest | 20 -10 390 390 | 439 | 1049 | Sharon | Sharon | 0 | 0 | 0 | 0 | 1 | true | person |

_ƒ kennzeichnet eine berechnete Spalte._

</details>

| Begriff | Beschreibung | Erläuternder Kommentar |
|------|-------------|-------------------|
| **Portrait** | The page's Fandom portraits, including those for characters a reader may add. | — |
| Display Name | Ein definiertes Attribut. | _The name on the portrait's Fandom page._ |
| Page | Ein definiertes Attribut. | _The Fandom page credited for the portrait._ |
| Image | Ein definiertes Attribut. | _The original Fandom image._ |
| Crop | Ein definiertes Attribut. | _The crop as an SVG viewBox: x y width height._ |
| Width | Ein definiertes Attribut. | _The original image's width in pixels._ |
| Height | Ein definiertes Attribut. | _The original image's height in pixels._ |
| Sort Order | Ein definiertes Attribut. | _Position in the credits._ |
| Name | Entspricht Display Name. | _Order 1. The portrait's name._ |

<details open>
<summary>Beispieldaten</summary>

| Display Name | Page | Image | Crop | Width | Height | Sort Order | Name ƒ |
|---|---|---|---|---|---|---|---|
| Marvin Marsh | https://southpark.fandom.com/wiki/Marvin_Marsh | https://static.wikia.nocookie.net/southpark/images/d/db/Marsh-family-grandpa-marsh-w-purple-blanket.png/revision/latest | 48 -5 280 280 | 375 | 536 | 1 | Marvin Marsh |
| Randy Marsh | https://southpark.fandom.com/wiki/Randy_Marsh | https://static.wikia.nocookie.net/southparkfanon/images/d/d6/Randy_Marsh.png/revision/latest | 0 -10 428 428 | 428 | 1057 | 2 | Randy Marsh |
| Sharon Marsh | https://southpark.fandom.com/wiki/Sharon_Marsh | https://static.wikia.nocookie.net/southpark/images/9/99/Sharon_Marsh.png/revision/latest | 20 -10 390 390 | 439 | 1049 | 3 | Sharon Marsh |

_ƒ kennzeichnet eine berechnete Spalte._

</details>

| Begriff | Beschreibung | Erläuternder Kommentar |
|------|-------------|-------------------|
| **Portrait Aliase** | The names a portrait answers to. A new thing whose name matches an alias gets that portrait. | — |
| Portrait | Ein definiertes Attribut. | _The portrait this name selects._ |
| Name | Entspricht Portrait Alias ID. | _Order 1. The alias._ |

<details open>
<summary>Beispieldaten</summary>

| Name ƒ |
|---|
| marvin |
| marvin marsh |
| grandpa marsh |

_ƒ kennzeichnet eine berechnete Spalte._

</details>

| Begriff | Beschreibung | Erläuternder Kommentar |
|------|-------------|-------------------|
| **Property** | The ten properties, with what the symmetric, domain and range rules read. Axioms that relate one property to another (inverse, chain, disjoint) are rows of PropertyAxioms. | — |
| Label | Ein definiertes Attribut. | _The property's wording on the page._ |
| Property Kind | Ein definiertes Attribut. | _'object' links two things, 'datatype' links a thing to a value, 'type' states class membership._ |
| Datatype | Ein definiertes Attribut. | _The value's XML Schema datatype for a datatype property; blank otherwise._ |
| Is User Addable | Wahr, wenn eine leere Zeichenkette. | _Offered in the page's Add fact form._ |
| Has One Current Value | Wahr, wenn eine leere Zeichenkette. | _The page keeps one current value per person: switching a value on replaces the earlier one._ |
| Is Symmetric | Wahr, wenn eine leere Zeichenkette. | _The symmetric rule makes this link true in both directions._ |
| Domain Class | Ein definiertes Attribut. | _The class the domain rule gives this property's subject._ |
| Range Class | Ein definiertes Attribut. | _The class the range rule gives this property's object._ |
| Sort Order | Ein definiertes Attribut. | _Position in the Add fact form's property list._ |
| Value Node Label | Ein definiertes Attribut. | _The small label under a value node for this property: 'role value' or 'number value'._ |
| Current Value Wording | Ein definiertes Attribut. | _How the Add fact form names a replaced value, as in 'Stan now has grade level 5'._ |
| Input Name | Ein definiertes Attribut. | _How the integer check names this property's value, as in 'for grade level'._ |
| Record Kind | Ein definiertes Attribut. | _The word the current-record summary uses for this property's values: role, grade or school._ |
| Record Order | Ein definiertes Attribut. | _Position of this property in the current-record summary._ |
| Name | Entspricht Label. | _Order 1. The property's wording._ |

<details open>
<summary>Beispieldaten</summary>

| Label | Property Kind | Datatype | Is User Addable | Has One Current Value | Is Symmetric | Sort Order | Value Node Label | Current Value Wording | Input Name | Record Kind | Record Order | Name ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|
| is parent of | object | — | true | false | false | 1 | — | — | — | — | — | is parent of |
| is child of | object | — | true | false | false | 2 | — | — | — | — | — | is child of |
| is spouse of | object | — | true | false | true | 3 | — | — | — | — | — | is spouse of |

_ƒ kennzeichnet eine berechnete Spalte._

</details>

| Begriff | Beschreibung | Erläuternder Kommentar |
|------|-------------|-------------------|
| **Property Axiom** | Axioms that relate one property to others, as OWL states them: an inverse, a two-link chain, or a disjoint pair. The candidate generator reads these rows. | — |
| Kind | Ein definiertes Attribut. | _inverse, chain or disjoint._ |
| Property | Ein definiertes Attribut. | _The property the axiom is about: the one inverted, the one a chain derives, or the first of a disjoint pair._ |
| First | Ein definiertes Attribut. | _The inverse property, the chain's first link, or the other property of a disjoint pair._ |
| Second | Ein definiertes Attribut. | _The chain's second link._ |
| Name | Entspricht Property Axiom ID. | _Order 1. The axiom's key._ |

<details open>
<summary>Beispieldaten</summary>

| Name ƒ |
|---|
| inverse:isParentOf |
| disjoint:isParentOf~isSpouseOf |
| inverse:isChildOf |

_ƒ kennzeichnet eine berechnete Spalte._

</details>

| Begriff | Beschreibung | Erläuternder Kommentar |
|------|-------------|-------------------|
| **Statement** | Every fact the page can state or conclude about the cast. A listed fact appears in the page's fact list; the rest are candidates that only a rule can make hold. | — |
| Graph | Ein definiertes Attribut. | _The graph the fact belongs to._ |
| Subject | Ein definiertes Attribut. | _What the fact describes._ |
| Property | Ein definiertes Attribut. | _How the subject connects to the object._ |
| Object Thing | Ein definiertes Attribut. | _The related thing, for an object or type property._ |
| Object Value | Ein definiertes Attribut. | _The literal value, for a datatype property._ |
| Is Listed | Wahr, wenn eine leere Zeichenkette. | _Shown in the page's fact list._ |
| Is Built in | Wahr, wenn eine leere Zeichenkette. | _One of the page's teaching facts; the reader cannot remove it._ |
| Is Detail | Wahr, wenn eine leere Zeichenkette. | _One of Stan's detail facts that Show Stan's details switches on._ |
| Is Stated | Wahr, wenn eine leere Zeichenkette. | _The fact's checkbox._ |
| Activation Seq | Ein definiertes Attribut. | _When the fact was last switched on, as a running number; decides which value is current._ |
| Sort Order | Ein definiertes Attribut. | _Position in the fact list; candidates follow the listed facts._ |
| Subject Label | Aus dem zugehörigen Subject übernommen. | _Order 1. The subject's name._ |
| Property Label | Aus dem zugehörigen Property übernommen. | _Order 1. The property's wording._ |
| Object Label | Aus dem zugehörigen Object Thing übernommen. | _Order 1. The object's name, for an object or type property._ |
| Property Datatype | Aus dem zugehörigen Property übernommen. | _Order 1. The value's datatype, for a datatype property._ |
| Property Has One Current Value | Wahr, wenn der zugehörige Property des Statement hat eine One Current Value. | _Order 1. Whether the property keeps one current value per person._ |
| Property Domain Class | Aus dem zugehörigen Property übernommen. | _Order 1. The property's domain class._ |
| Property Range Class | Aus dem zugehörigen Property übernommen. | _Order 1. The property's range class._ |
| Literal Quote | Aus dem zugehörigen Graph übernommen. | _Order 1. The graph's literal quote mark._ |
| Literal Type Marker | Aus dem zugehörigen Graph übernommen. | _Order 1. The graph's datatype marker._ |
| Built in Source Text | Aus dem zugehörigen Graph übernommen. | _Order 1. The graph's source line for a teaching fact._ |
| User Source Text | Aus dem zugehörigen Graph übernommen. | _Order 1. The graph's source line for a reader's fact._ |
| Next Fact Sort Order | Aus dem zugehörigen Graph übernommen. | _Order 6. List position of the next fact to add._ |
| Is Literal | Wahr, wenn der Object Thing leer ist. | _Order 1. True when the object is a value rather than a thing._ |
| Object Text | Nach Priorität bestimmt: die Object Value, wenn der Object Thing leer ist; andernfalls der Object Label. | _Order 2. The object's name or the value._ |
| Name | Berechnet als der Subject Label, gefolgt von ein Leerzeichen, gefolgt von der Property Label, gefolgt von ein Leerzeichen, gefolgt von der Object Text. | _Order 3. The fact in words._ |
| Words | Berechnet als der Subject Label, gefolgt von ein Leerzeichen, gefolgt von der Property Label, gefolgt von ein Leerzeichen, gefolgt von die Literal Quote, gefolgt von die Object Value, gefolgt von die Literal Quote, gefolgt von der Literal Type Marker, gefolgt von die Property Datatype, wenn der Object Thing leer ist, andernfalls der Object Label. ⚠︎ mechanisch <!-- rulespeak:reword --> | _Order 2. The fact as a conclusion card quotes it, with a literal written in Turtle form._ |
| Source | Nach Priorität bestimmt: der Built in Source Text, wenn das Kennzeichen „Is Built in“ gesetzt ist; andernfalls der User Source Text. | _Order 2. Where a listed fact came from._ |
| Subject Property Key | Berechnet als der Subject, gefolgt von „|“, gefolgt von der Property. | _Order 1. Groups the values one subject has for one property._ |
| Latest Sibling Activation Seq | Der größte Wert von Activation Seq über die mit dem Statement verbundenen Statements. | _Order 2. The most recent activation among this subject's values for this property._ |
| Is in Effect | Wahr, wenn alles Folgende gilt: das Kennzeichen „Is Stated“ gesetzt ist und mindestens eines des Folgenden gilt: das Kennzeichen „Property Has One Current Value“ nicht gesetzt ist oder der Activation Seq der Latest Sibling Activation Seq ist. | _Order 3. Switched on and, for a one-value property, the most recently switched-on value. Drawn as a solid line._ |
| Listed Unit | Nach Priorität bestimmt: 1, wenn das Kennzeichen „Is Listed“ gesetzt ist; andernfalls 0. | _Order 1. 1 when the fact is in the fact list._ |
| In Effect Unit | Nach Priorität bestimmt: 1, wenn das Kennzeichen „Is in Effect“ gesetzt ist; andernfalls 0. | _Order 4. 1 when the fact is in effect._ |
| Listed Off Unit | Nach Priorität bestimmt: 1, wenn alles Folgende gilt: das Kennzeichen „Is Listed“ gesetzt ist und das Kennzeichen „Is in Effect“ nicht gesetzt ist; andernfalls 0. | _Order 4. 1 when the fact is listed but not in effect._ |
| Is Next to Add | Wahr, wenn alles Folgende gilt: der Listed Off Unit 1 ist und der Sort Order der Next Fact Sort Order ist. | _Order 7. The fact the Add next fact button switches on._ |
| Count of Inverse Firings | Die Gesamtsumme von Inverse Fired Unit über die mit dem Statement verbundenen Firings. | _Order 6. Inverse rule applications that conclude this fact._ |
| Count of Symmetric Firings | Die Gesamtsumme von Symmetric Fired Unit über die mit dem Statement verbundenen Firings. | _Order 6. Symmetric rule applications that conclude this fact._ |
| Holds Before Chain | Wahr, wenn mindestens eines des Folgenden gilt: das Kennzeichen „Is in Effect“ gesetzt ist; der Count of Inverse Firings größer ist als 0; oder der Count of Symmetric Firings größer ist als 0. | _Order 7. Holds from facts in effect plus the inverse and symmetric rules._ |
| Count of Chain Firings | Die Gesamtsumme von Chain Fired Unit über die mit dem Statement verbundenen Firings. | _Order 10. Property chain applications that conclude this fact._ |
| Holds Before Typing | Wahr, wenn mindestens eines des Folgenden gilt: das Kennzeichen „Holds Before Chain“ gesetzt ist oder der Count of Chain Firings größer ist als 0. | _Order 11. Holds before the domain and range rules apply._ |
| Count of Type Firings | Die Gesamtsumme von Type Fired Unit über die mit dem Statement verbundenen Firings. | _Order 14. Domain and range applications that conclude this fact._ |
| Holds | Wahr, wenn mindestens eines des Folgenden gilt: das Kennzeichen „Holds Before Typing“ gesetzt ist oder der Count of Type Firings größer ist als 0. | _Order 15. True when in effect or concluded by an enabled rule; the graph draws it._ |
| Is Conclusion | Wahr, wenn alles Folgende gilt: das Kennzeichen „Holds“ gesetzt ist und das Kennzeichen „Is in Effect“ nicht gesetzt ist. | _Order 16. Holds only because a rule concluded it. Drawn dashed._ |
| Holds Unit | Nach Priorität bestimmt: 1, wenn das Kennzeichen „Holds“ gesetzt ist; andernfalls 0. | _Order 16. 1 when the fact holds._ |
| Conclusion Unit | Nach Priorität bestimmt: 1, wenn das Kennzeichen „Is Conclusion“ gesetzt ist; andernfalls 0. | _Order 17. 1 when the fact is a conclusion._ |
| Parent Unit | Nach Priorität bestimmt: 1, wenn alles Folgende gilt: das Kennzeichen „Holds“ gesetzt ist und der Property „isParentOf“ ist; andernfalls 0. | _Order 16. 1 for a holding parent link._ |
| Grandparent Unit | Nach Priorität bestimmt: 1, wenn alles Folgende gilt: das Kennzeichen „Holds“ gesetzt ist und der Property „isGrandparentOf“ ist; andernfalls 0. | _Order 16. 1 for a holding grandparent link._ |
| Place Unit | Nach Priorität bestimmt: 1, wenn alles Folgende gilt: das Kennzeichen „Holds“ gesetzt ist und der Property „attends“ ist; andernfalls 0. | _Order 16. 1 for a holding attends link._ |
| First Pre Chain Order Key | Der kleinste Wert von Pre Chain Order Key über die Firings des Statement, die Pre Chain Fired Unit sind. | _Order 7. Order key of the first inverse or symmetric application that concludes this fact._ |
| Snap Position | Nach Priorität bestimmt: der Sort Order, wenn das Kennzeichen „Is in Effect“ gesetzt ist; 100000 plus der First Pre Chain Order Key, wenn das Kennzeichen „Holds Before Chain“ gesetzt ist; andernfalls 0. | _Order 8. Where the page's reasoner meets this fact as a premise: a stated fact at its list position, a fact concluded by the inverse or symmetric rule after every stated fact, in the order it was concluded; 0 for a fact that does not hold yet, so a link to no fact reads the same on every engine._ |
| First Fired Order Key | Der kleinste Wert von Order Key über die Firings des Statement, die Fired Unit sind. | _Order 15. Order key of the first rule application that concludes this fact, in the page reasoner's order._ |
| Count of Chosen Firings | Die Gesamtsumme von Chosen Unit über die mit dem Statement verbundenen Firings. | _Order 20. Rule applications shown as this fact's conclusion card: one for a conclusion, zero otherwise._ |
| Subject Node Key | Entspricht Subject. | _Order 1. The graph node this fact starts from._ |
| Object Node Key | Nach Priorität bestimmt: „literal|“, gefolgt von die Object Value, gefolgt von „|“, gefolgt von die Property Datatype, wenn der Object Thing leer ist; andernfalls der Object Thing. | _Order 2. The graph node this fact points at: the thing, or one node per value and datatype._ |
| Subject Node Kind | Aus dem zugehörigen Subject übernommen. | _Order 19. How the graph draws the subject._ |
| Object Thing Node Kind | Aus dem zugehörigen Object Thing übernommen. | _Order 19. How the graph draws the object thing._ |
| Object Node Kind | Nach Priorität bestimmt: „value“, wenn der Object Thing leer ist; andernfalls der Object Thing Node Kind. | _Order 20. How the graph draws the object: a value, a class, a place or a person._ |
| Object Value Node Label | Aus dem zugehörigen Property übernommen. | _Order 1. The small label under a value node._ |
| Count of Violations As First | Die Gesamtsumme von Violation Unit über die mit dem Statement verbundenen Disjoint Checks. | _Order 18. Violated disjointness checks naming this fact first._ |
| Count of Violations As Second | Die Gesamtsumme von Violation Unit über die mit dem Statement verbundenen Disjoint Checks. | _Order 18. Violated disjointness checks naming this fact second._ |
| Is in Contradiction | Wahr, wenn mindestens eines des Folgenden gilt: der Count of Violations As First größer ist als 0 oder der Count of Violations As Second größer ist als 0. | _Order 19. Part of a contradiction the disjoint rule found; drawn in the alarm colour._ |

<details open>
<summary>Beispieldaten</summary>

| Object Value | Is Listed | Is Built in | Is Detail | Is Stated | Activation Seq | Sort Order | Subject Label ƒ | Property Label ƒ | Object Label ƒ | Property Datatype ƒ | Property Has One Current Value ƒ | Property Domain Class ƒ | Property Range Class ƒ | Literal Quote ƒ | Literal Type Marker ƒ | Built in Source Text ƒ | User Source Text ƒ | Next Fact Sort Order ƒ | Is Literal ƒ | Object Text ƒ | Name ƒ | Words ƒ | Source ƒ | Subject Property Key ƒ | Latest Sibling Activation Seq ƒ | Is in Effect ƒ | Listed Unit ƒ | In Effect Unit ƒ | Listed Off Unit ƒ | Is Next to Add ƒ | Count of Inverse Firings ƒ | Count of Symmetric Firings ƒ | Holds Before Chain ƒ | Count of Chain Firings ƒ | Holds Before Typing ƒ | Count of Type Firings ƒ | Holds ƒ | Is Conclusion ƒ | Holds Unit ƒ | Conclusion Unit ƒ | Parent Unit ƒ | Grandparent Unit ƒ | Place Unit ƒ | First Pre Chain Order Key ƒ | Snap Position ƒ | First Fired Order Key ƒ | Count of Chosen Firings ƒ | Subject Node Key ƒ | Object Node Key ƒ | Subject Node Kind ƒ | Object Thing Node Kind ƒ | Object Node Kind ƒ | Object Value Node Label ƒ | Count of Violations As First ƒ | Count of Violations As Second ƒ | Is in Contradiction ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| — | true | true | false | true | 1 | 1 | Marvin | is parent of | Randy | — | false | — | — | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | — | false | Randy | Marvin is parent of Randy | Marvin is parent of Randy | Stated as a teaching example on this page. | Marvin\|isParentOf | 1 | true | 1 | 1 | 0 | false | 0 | 0 | true | 0 | true | 0 | true | false | 1 | 0 | 1 | 0 | 0 | — | 1.0 | — | 0 | Marvin | Randy | person | person | person | — | 0 | 0 | false |
| — | false | false | false | false | 0 | 100001 | Marvin | is parent of | Sharon | — | false | — | — | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | — | false | Sharon | Marvin is parent of Sharon | Marvin is parent of Sharon | Stated by you on this page. | Marvin\|isParentOf | 1 | false | 0 | 0 | 0 | false | 0 | 0 | false | 0 | false | 0 | false | false | 0 | 0 | 0 | 0 | 0 | — | 0.0 | — | 0 | Marvin | Sharon | person | person | person | — | 0 | 0 | false |
| — | false | false | false | false | 0 | 100002 | Marvin | is parent of | Stan | — | false | — | — | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | — | false | Stan | Marvin is parent of Stan | Marvin is parent of Stan | Stated by you on this page. | Marvin\|isParentOf | 1 | false | 0 | 0 | 0 | false | 0 | 0 | false | 0 | false | 0 | false | false | 0 | 0 | 0 | 0 | 0 | — | 0.0 | — | 0 | Marvin | Stan | person | person | person | — | 0 | 0 | false |

_ƒ kennzeichnet eine berechnete Spalte._

</details>

| Begriff | Beschreibung | Erläuternder Kommentar |
|------|-------------|-------------------|
| **Firing** | Every way an enabled rule could conclude a candidate fact from the facts that hold. | — |
| Kind | Ein definiertes Attribut. | _How the rule applies._ |
| Rule | Ein definiertes Attribut. | _The rule switch._ |
| Target | Ein definiertes Attribut. | _The fact this application concludes._ |
| First | Ein definiertes Attribut. | _The premise._ |
| Second | Ein definiertes Attribut. | _The second premise, for the property chain._ |
| Rule Enabled | Wahr, wenn der zugehörige Rule des Firing ist enabled. | _Order 1. Whether the rule is switched on._ |
| Kind Wording | Aus dem zugehörigen Kind übernommen. | _Order 1. How the card names the rule._ |
| Kind Sort Order | Aus dem zugehörigen Kind übernommen. | _Order 1. The kind's place in the reasoner's order._ |
| Target Name | Aus dem zugehörigen Target übernommen. | _Order 4. The concluded fact in words._ |
| Target Subject Label | Aus dem zugehörigen Target übernommen. | _Order 2. The concluded fact's subject._ |
| Target Property Label | Aus dem zugehörigen Target übernommen. | _Order 2. The concluded fact's property._ |
| Target Object Text | Aus dem zugehörigen Target übernommen. | _Order 3. The concluded fact's object._ |
| Target is Conclusion | Wahr, wenn der zugehörige Target des Firing ist eine Conclusion. | _Order 17. Whether the concluded fact is new._ |
| Target Subject Node Key | Aus dem zugehörigen Target übernommen. | _Order 2. The concluded fact's subject node._ |
| Target Object Node Key | Aus dem zugehörigen Target übernommen. | _Order 3. The concluded fact's object node._ |
| Target First Fired Order Key | Aus dem zugehörigen Target übernommen. | _Order 16. The order key of the concluded fact's first application._ |
| First Words | Aus dem zugehörigen First übernommen. | _Order 3. The premise as the card quotes it._ |
| Second Words | Aus dem zugehörigen Second übernommen. | _Order 3. The second premise as the card quotes it._ |
| First is in Effect | Wahr, wenn der zugehörige First des Firing ist in effect. | _Order 4. Whether the premise is in effect._ |
| First Holds Before Chain | Wahr, wenn der zugehörige First des Firing ist Holds Before Chain. | _Order 8. Whether the premise holds before the chain._ |
| Second Holds Before Chain | Wahr, wenn der zugehörige Second des Firing ist Holds Before Chain. | _Order 8. Whether the second premise holds before the chain._ |
| First Holds Before Typing | Wahr, wenn der zugehörige First des Firing ist Holds Before Typing. | _Order 12. Whether the premise holds before typing._ |
| First Property | Aus dem zugehörigen First übernommen. | _Order 1. The premise's property key, as the card's axiom names it._ |
| First Property Domain Class | Aus dem zugehörigen First übernommen. | _Order 2. The premise property's domain class._ |
| First Property Range Class | Aus dem zugehörigen First übernommen. | _Order 2. The premise property's range class._ |
| First Sort Order | Aus dem zugehörigen First übernommen. | _Order 1. The premise's list position._ |
| Second is in Effect | Wahr, wenn der zugehörige Second des Firing ist in effect. | _Order 4. Whether the second premise is in effect._ |
| First Snap Position | Aus dem zugehörigen First übernommen. | _Order 9. Where the reasoner meets the premise._ |
| Second Snap Position | Aus dem zugehörigen Second übernommen. | _Order 9. Where the reasoner meets the second premise._ |
| Inverse Fired Unit | Nach Priorität bestimmt: 1, wenn alles Folgende gilt: das Kennzeichen „Rule Enabled“ gesetzt ist; der Kind „inverse“ ist; und das Kennzeichen „First is in Effect“ gesetzt ist; andernfalls 0. | _Order 5. 1 when the inverse rule fires here._ |
| Symmetric Fired Unit | Nach Priorität bestimmt: 1, wenn alles Folgende gilt: das Kennzeichen „Rule Enabled“ gesetzt ist; der Kind „symmetric“ ist; und das Kennzeichen „First is in Effect“ gesetzt ist; andernfalls 0. | _Order 5. 1 when the symmetric rule fires here._ |
| Chain Fired Unit | Nach Priorität bestimmt: 1, wenn alles Folgende gilt: das Kennzeichen „Rule Enabled“ gesetzt ist; der Kind „chain“ ist; das Kennzeichen „First Holds Before Chain“ gesetzt ist; und das Kennzeichen „Second Holds Before Chain“ gesetzt ist; andernfalls 0. | _Order 9. 1 when the property chain fires here._ |
| Type Fired Unit | Nach Priorität bestimmt: 1, wenn alles Folgende gilt: das Kennzeichen „Rule Enabled“ gesetzt ist; mindestens eines des Folgenden gilt: der Kind „domain“ ist oder der Kind „range“ ist; und das Kennzeichen „First Holds Before Typing“ gesetzt ist; andernfalls 0. | _Order 13. 1 when the domain or range rule fires here._ |
| Fired Unit | Berechnet als der Inverse Fired Unit plus der Symmetric Fired Unit plus der Chain Fired Unit plus der Type Fired Unit. | _Order 14. 1 when this application fires._ |
| Pre Chain Fired Unit | Berechnet als der Inverse Fired Unit plus der Symmetric Fired Unit. | _Order 6. 1 when this is an inverse or symmetric application that fires._ |
| Pre Chain Order Key | Berechnet als der First Sort Order mal 10 plus der Kind Sort Order. | _Order 2. For an inverse or symmetric application: its place in the reasoner's first round._ |
| Round | Nach Priorität bestimmt: 2, wenn alles Folgende gilt: der Kind „chain“ ist und es nicht der Fall ist, dass alles Folgende gilt: das Kennzeichen „First is in Effect“ gesetzt ist und das Kennzeichen „Second is in Effect“ gesetzt ist; andernfalls 1. | _Order 5. The reasoner's round: 1 when every premise is a stated fact, 2 when the property chain reads a concluded fact._ |
| Order Key | Berechnet als der Round mal 1000000000000000 plus die First Snap Position mal 100000000 plus der Kind Sort Order mal 10000000 plus 0, wenn der Second leer ist, andernfalls die Second Snap Position. | _Order 10. The page reasoner's order: by round, then premise, then rule kind, then second premise._ |
| Is Chosen | Wahr, wenn alles Folgende gilt: der Fired Unit 1 ist; das Kennzeichen „Target is Conclusion“ gesetzt ist; und der Order Key der Target First Fired Order Key ist. | _Order 18. The application the conclusions panel shows for its fact: the first one in the reasoner's order._ |
| Chosen Unit | Nach Priorität bestimmt: 1, wenn das Kennzeichen „Is Chosen“ gesetzt ist; andernfalls 0. | _Order 19. 1 when this application is shown._ |
| Card Text | Nach Priorität bestimmt: „Worked out by the “, gefolgt von der Kind Wording, gefolgt von „ from ““, gefolgt von der First Words, gefolgt von „”“, gefolgt von „ and ““, gefolgt von der Second Words, gefolgt von „”“, wenn der Kind „chain“ ist, andernfalls „ and ““, gefolgt von der First Property, gefolgt von „ has domain “, gefolgt von der First Property Domain Class, gefolgt von „”“, wenn der Kind „domain“ ist, andernfalls „ and ““, gefolgt von der First Property, gefolgt von „ has range “, gefolgt von der First Property Range Class, gefolgt von „”“, gefolgt von ein Punkt, wenn das Kennzeichen „Is Chosen“ gesetzt ist; andernfalls eine leere Zeichenkette. ⚠︎ mechanisch <!-- rulespeak:reword --> | _Order 19. The conclusion card's provenance line, in the page's wording._ |
| Name | Entspricht Firing ID. | _Order 1. The application's key._ |

<details open>
<summary>Beispieldaten</summary>

| Rule Enabled ƒ | Kind Wording ƒ | Kind Sort Order ƒ | Target Name ƒ | Target Subject Label ƒ | Target Property Label ƒ | Target Object Text ƒ | Target is Conclusion ƒ | Target Subject Node Key ƒ | Target Object Node Key ƒ | Target First Fired Order Key ƒ | First Words ƒ | Second Words ƒ | First is in Effect ƒ | First Holds Before Chain ƒ | Second Holds Before Chain ƒ | First Holds Before Typing ƒ | First Property ƒ | First Property Domain Class ƒ | First Property Range Class ƒ | First Sort Order ƒ | Second is in Effect ƒ | First Snap Position ƒ | Second Snap Position ƒ | Inverse Fired Unit ƒ | Symmetric Fired Unit ƒ | Chain Fired Unit ƒ | Type Fired Unit ƒ | Fired Unit ƒ | Pre Chain Fired Unit ƒ | Pre Chain Order Key ƒ | Round ƒ | Order Key ƒ | Is Chosen ƒ | Chosen Unit ƒ | Card Text ƒ | Name ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| false | inverse rule | 1 | Randy is child of Marvin | Randy | is child of | Marvin | false | Randy | Marvin | — | Marvin is parent of Randy |    | true | true | false | true | isParentOf | — | — | 1 | false | 1.0 | 0.0 | 0 | 0 | 0 | 0 | 0 | 0 | 11.0 | 1 | 1000000110000000.0 | false | 0 | — | inverse:Marvin\|isParentOf\|Randy>Randy\|isChildOf\|Marvin |
| false | inverse rule | 1 | Sharon is child of Marvin | Sharon | is child of | Marvin | false | Sharon | Marvin | — | Marvin is parent of Sharon |    | false | false | false | false | isParentOf | — | — | 100001 | false | 0.0 | 0.0 | 0 | 0 | 0 | 0 | 0 | 0 | 1000011.0 | 1 | 1000000010000000.0 | false | 0 | — | inverse:Marvin\|isParentOf\|Sharon>Sharon\|isChildOf\|Marvin |
| false | inverse rule | 1 | Stan is child of Marvin | Stan | is child of | Marvin | false | Stan | Marvin | — | Marvin is parent of Stan |    | false | false | false | false | isParentOf | — | — | 100002 | false | 0.0 | 0.0 | 0 | 0 | 0 | 0 | 0 | 0 | 1000021.0 | 1 | 1000000010000000.0 | false | 0 | — | inverse:Marvin\|isParentOf\|Stan>Stan\|isChildOf\|Marvin |

_ƒ kennzeichnet eine berechnete Spalte._

</details>

| Begriff | Beschreibung | Erläuternder Kommentar |
|------|-------------|-------------------|
| **Disjoint Check** | Every pair the disjointness rule watches: the same two people linked by two disjoint properties. | — |
| Graph | Ein definiertes Attribut. | _The graph._ |
| Rule | Ein definiertes Attribut. | _The rule switch._ |
| First | Ein definiertes Attribut. | _The first link._ |
| Second | Ein definiertes Attribut. | _The disjoint link between the same pair, in the same direction._ |
| Rule Enabled | Wahr, wenn der zugehörige Rule des Disjoint Check ist enabled. | _Order 1. Whether the disjointness rule is on._ |
| First Holds | Wahr, wenn der zugehörige First des Disjoint Check ist Holds. | _Order 16. Whether the first link holds._ |
| Second Holds | Wahr, wenn der zugehörige Second des Disjoint Check ist Holds. | _Order 16. Whether the second link holds._ |
| Subject Label | Aus dem zugehörigen First übernommen. | _Order 2. The person recorded with both links._ |
| Object Label | Aus dem zugehörigen First übernommen. | _Order 2. The other person._ |
| Violation Unit | Nach Priorität bestimmt: 1, wenn alles Folgende gilt: das Kennzeichen „Rule Enabled“ gesetzt ist; das Kennzeichen „First Holds“ gesetzt ist; und das Kennzeichen „Second Holds“ gesetzt ist; andernfalls 0. | _Order 17. 1 when both links hold._ |
| Alarm Text | Nach Priorität bestimmt: „Contradiction found. “, gefolgt von der Subject Label, gefolgt von „ is recorded as both parent and spouse of “, gefolgt von der Object Label, gefolgt von „. Check those facts and the selected disjoint rule.“, wenn der Violation Unit 1 ist; andernfalls eine leere Zeichenkette. | _Order 18. The page's contradiction alert, blank when there is none._ |
| Name | Entspricht Disjoint Check ID. | _Order 1. The check's key._ |

<details open>
<summary>Beispieldaten</summary>

| Rule Enabled ƒ | First Holds ƒ | Second Holds ƒ | Subject Label ƒ | Object Label ƒ | Violation Unit ƒ | Alarm Text ƒ | Name ƒ |
|---|---|---|---|---|---|---|---|
| false | true | false | Marvin | Randy | 0 | — | Marvin\|isParentOf\|Randy~isSpouseOf |
| false | false | false | Marvin | Sharon | 0 | — | Marvin\|isParentOf\|Sharon~isSpouseOf |
| false | false | false | Marvin | Stan | 0 | — | Marvin\|isParentOf\|Stan~isSpouseOf |

_ƒ kennzeichnet eine berechnete Spalte._

</details>

| Begriff | Beschreibung | Erläuternder Kommentar |
|------|-------------|-------------------|
| **Scenario** | The page's buttons. Each one is a fact baseline, explicit fact changes and rule settings, applied in that order. | — |
| Graph | Ein definiertes Attribut. | _The graph._ |
| Label | Ein definiertes Attribut. | _The button's text._ |
| Note | Ein definiertes Attribut. | _The note shown under the buttons after this one is pressed; blank clears it._ |
| Sort Order | Ein definiertes Attribut. | _Position among the buttons._ |
| Button Group | Ein definiertes Attribut. | _'stan' for the two imagined changes, 'facts' for the buttons under the fact list._ |
| Is Primary | Wahr, wenn eine leere Zeichenkette. | _Drawn as the filled button._ |
| Fact Baseline | Ein definiertes Attribut. | _What happens to every listed fact first: keep, built-in (teaching facts on, others off), built-in-detail, all-off, activate-all (each switched on in list order), or next (the next fact on)._ |
| Layout | Ein definiertes Attribut. | _The node layout the graph returns to; blank keeps the current positions._ |
| Clears Selection | Wahr, wenn eine leere Zeichenkette. | _Clears the selected node or line._ |
| Clears Form Message | Wahr, wenn eine leere Zeichenkette. | _Clears the Add fact form's message._ |
| Name | Entspricht Label. | _Order 1. The button's text._ |
| Graph Has Facts to Add | Wahr, wenn der zugehörige Graph des Scenario hat einen Facts to Add. | _Order 7. Whether a listed fact can still be added._ |
| Is Available | Wahr, wenn mindestens eines des Folgenden gilt: die Fact Baseline nicht „next“ ist oder das Kennzeichen „Graph Has Facts to Add“ gesetzt ist. | _Order 8. False only for Add next fact once every listed fact is in effect._ |

<details open>
<summary>Beispieldaten</summary>

| Label | Note | Sort Order | Button Group | Is Primary | Fact Baseline | Layout | Clears Selection | Clears Form Message | Name ƒ | Graph Has Facts to Add ƒ | Is Available ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|
| Move Stan to grade 5 | This imagined change updates Stan’s grade to 5. His identifier, Person statement and family links stay the same. His other selected details are unchanged. | 1 | stan | false | keep | — | true | true | Move Stan to grade 5 | false | true |
| Stan leaves school | This imagined change clears Stan’s current school details. His identifier, Person statement and family links remain. Removing a current fact does not say that its opposite is true; school history needs dates. | 2 | stan | false | keep | — | true | true | Stan leaves school | false | true |
| Add next fact | — | 3 | facts | true | next | — | false | false | Add next fact | false | false |

_ƒ kennzeichnet eine berechnete Spalte._

</details>

| Begriff | Beschreibung | Erläuternder Kommentar |
|------|-------------|-------------------|
| **Scenario Fact Change** | Fact changes a button makes after its baseline. A blank object key matches every listed value of the property. | — |
| Scenario | Ein definiertes Attribut. | _The button._ |
| Subject | Ein definiertes Attribut. | _The subject whose facts change._ |
| Property | Ein definiertes Attribut. | _The property whose facts change._ |
| Object Key | Ein definiertes Attribut. | _The object thing's key or the literal value; blank for every listed value._ |
| Action | Ein definiertes Attribut. | _'on' switches the fact on as the newest value; 'off' switches it off._ |
| Adds to List | Wahr, wenn eine leere Zeichenkette. | _Adds the fact to the fact list first when it is not listed._ |
| Sort Order | Ein definiertes Attribut. | _Order of the change within the button._ |
| Name | Entspricht Scenario Fact Change ID. | _Order 1. The change's key._ |

<details open>
<summary>Beispieldaten</summary>

| Object Key | Action | Adds to List | Sort Order | Name ƒ |
|---|---|---|---|---|
| Person | on | true | 1 | stan-grade-five:Stan\|isA\|Person |
| 5 | on | true | 2 | stan-grade-five:Stan\|hasGradeLevel\|5 |
| Person | on | true | 3 | stan-leaves-school:Stan\|isA\|Person |

_ƒ kennzeichnet eine berechnete Spalte._

</details>

| Begriff | Beschreibung | Erläuternder Kommentar |
|------|-------------|-------------------|
| **Scenario Rule Setting** | Rule switches a button sets. | — |
| Scenario | Ein definiertes Attribut. | _The button._ |
| Rule | Ein definiertes Attribut. | _The rule switch._ |
| Is Enabled | Wahr, wenn eine leere Zeichenkette. | _The state the button puts the switch in._ |
| Name | Entspricht Scenario Rule Setting ID. | _Order 1. The setting's key._ |

<details open>
<summary>Beispieldaten</summary>

| Is Enabled | Name ƒ |
|---|---|
| false | reset-original:inverse |
| false | reset-original:symmetric |
| false | reset-original:chain |

_ƒ kennzeichnet eine berechnete Spalte._

</details>

| Begriff | Beschreibung | Erläuternder Kommentar |
|------|-------------|-------------------|
| **Node Layout** | Node positions in the graph's 380 by 480 drawing, one set per layout. | — |
| Layout | Ein definiertes Attribut. | _The layout: 'default' or 'stan-details'._ |
| Node Key | Ein definiertes Attribut. | _The node: a thing's key, or literal\|value\|datatype._ |
| X | Ein definiertes Attribut. | _Horizontal position._ |
| Y | Ein definiertes Attribut. | _Vertical position._ |
| Name | Entspricht Node Layout ID. | _Order 1. The position's key._ |

<details open>
<summary>Beispieldaten</summary>

| Layout | Node Key | X | Y | Name ƒ |
|---|---|---|---|---|
| default | Marvin | 70 | 45 | default:Marvin |
| default | Randy | 195 | 125 | default:Randy |
| default | Sharon | 318 | 55 | default:Sharon |

_ƒ kennzeichnet eine berechnete Spalte._

</details>

| Begriff | Beschreibung | Erläuternder Kommentar |
|------|-------------|-------------------|
| **Layout Slot** | Places for nodes no layout positions, used in turn. | — |
| Sort Order | Ein definiertes Attribut. | _Order in which slots are used._ |
| X | Ein definiertes Attribut. | _Horizontal position._ |
| Y | Ein definiertes Attribut. | _Vertical position._ |
| Name | Entspricht Layout Slot ID. | _Order 1. The slot's key._ |

<details open>
<summary>Beispieldaten</summary>

| Sort Order | X | Y | Name ƒ |
|---|---|---|---|
| 1 | 330 | 135 | slot-1 |
| 2 | 45 | 120 | slot-2 |
| 3 | 330 | 400 | slot-3 |

_ƒ kennzeichnet eine berechnete Spalte._

</details>

| Begriff | Beschreibung | Erläuternder Kommentar |
|------|-------------|-------------------|
| **Page Text** | The page's wording, one row per string. {name}-style placeholders are filled by the page. | — |
| Text | Ein definiertes Attribut. | _The string._ |
| Name | Entspricht Page Text ID. | _Order 1. The string's key._ |

<details open>
<summary>Beispieldaten</summary>

| Text | Name ƒ |
|---|---|
| South Park Ontology Explainer, on one rulebook | page-title |
| A rulebook rebuild of the explainer by | byline-prefix |
| Jessica Talisman | byline-author |

_ƒ kennzeichnet eine berechnete Spalte._

</details>

| Begriff | Beschreibung | Erläuternder Kommentar |
|------|-------------|-------------------|
| **Engine** | The execution engines the page can be computed by. Each is a build output of this rulebook. | — |
| Label | Ein definiertes Attribut. | _The engine's name in the switcher._ |
| Substrate | Ein definiertes Attribut. | _The conformance harness's name for this substrate._ |
| Is Default | Wahr, wenn eine leere Zeichenkette. | _The engine a new visitor starts on._ |
| Description | Ein definiertes Attribut. | _What computes the page on this engine._ |
| Sort Order | Ein definiertes Attribut. | _Position in the switcher._ |
| Name | Entspricht Label. | _Order 1. The engine's name._ |

<details open>
<summary>Beispieldaten</summary>

| Label | Substrate | Is Default | Description | Sort Order | Name ƒ |
|---|---|---|---|---|---|
| Postgres views | effortless-postgres | true | Every value is a column of a generated view; the database computes it when the page reads it. | 1 | Postgres views |
| TypeScript | effortless-typescript | false | The generated TypeScript computes every field from the same rows, with no database. | 2 | TypeScript |
| OWL and SHACL rules | effortless-owl | false | The generated ontology and SHACL rules, run over the same rows by a SPARQL engine. | 3 | OWL and SHACL rules |

_ƒ kennzeichnet eine berechnete Spalte._

</details>

| Begriff | Beschreibung | Erläuternder Kommentar |
|------|-------------|-------------------|
| **Rulebook Tool** | What each rulebook tool adds to the original explainer; the tools panel lists them. | — |
| Label | Ein definiertes Attribut. | _The tool's name._ |
| Adds | Ein definiertes Attribut. | _What it adds to the original explainer._ |
| Href | Ein definiertes Attribut. | _Where its link goes; 'editor' means the running rulebook editor._ |
| Link Text | Ein definiertes Attribut. | _The link's text._ |
| Sort Order | Ein definiertes Attribut. | _Position in the panel._ |
| Name | Entspricht Label. | _Order 1. The tool's name._ |

<details open>
<summary>Beispieldaten</summary>

| Label | Adds | Href | Link Text | Sort Order | Name ƒ |
|---|---|---|---|---|---|
| Interchangeable engines | The same page, computed by Postgres views, generated TypeScript or the generated OWL and SHACL rules. Switch with the control above; nothing else changes. | — | — | 1 | Interchangeable engines |
| Conformance | Every engine's answers for the current facts, compared value by value with the conformance harness's own rule for when two values are equal. | — | — | 2 | Conformance |
| Explainer | Hover a dotted value for its rule in plain words and its witness: the values it came from, down to the stated facts. | /rulebook-explainer-dag/pages/index.html | Open the explainer | 3 | Explainer |

_ƒ kennzeichnet eine berechnete Spalte._

</details>

## 2 Faktentypen

- ein **Firing Kind** verweist auf genau ein **Rule**
- ein **Thing** kann auf ein **Portrait** verweisen
- eine **Portrait Aliase** verweist auf genau ein **Portrait**
- ein **Property** kann auf ein **Thing** verweisen
- ein **Property Axiom** verweist auf genau ein **Firing Kind**
- ein **Property Axiom** verweist auf genau ein **Property**
- ein **Statement** verweist auf genau ein **Graph**
- ein **Statement** verweist auf genau ein **Thing**
- ein **Statement** verweist auf genau ein **Property**
- ein **Firing** verweist auf genau ein **Firing Kind**
- ein **Firing** verweist auf genau ein **Rule**
- ein **Firing** verweist auf genau ein **Statement**
- ein **Disjoint Check** verweist auf genau ein **Graph**
- ein **Disjoint Check** verweist auf genau ein **Rule**
- ein **Disjoint Check** verweist auf genau ein **Statement**
- ein **Scenario** verweist auf genau ein **Graph**
- eine **Scenario Fact Change** verweist auf genau ein **Scenario**
- eine **Scenario Fact Change** verweist auf genau ein **Thing**
- eine **Scenario Fact Change** verweist auf genau ein **Property**
- ein **Scenario Rule Setting** verweist auf genau ein **Scenario**
- ein **Scenario Rule Setting** verweist auf genau ein **Rule**

## 3 Operative Regeln

_Operative Regeln besagen, was das Geschäft **verpflichtet**, **verbietet** oder
empfiehlt (**sollte**). Strukturelle Regeln stammen aus Pflichtfeldern und Fremdschlüsseln;
semantische Regeln stammen aus der Constraints-Tabelle, jede gestützt auf einen booleschen Wert,
den das Regelbuch bereits berechnet (querverwiesen als DR-N in den definitorischen Regeln unten)._

### Strukturelle Einschränkungen (aus dem Schema)

- Ein Graph **muss** ein Label; eine Literal Quote; ein Literal Type Marker; ein Built in Source Text; ein User Source Text; und ein No Conclusions Text haben.
- Eine Rule **muss** ein Label; eine Explanation; und ein Sort Order haben und erfassen, ob es ist enabled.
- Ein Firing Kind **muss** auf genau einen Rule verweisen.
- Ein Firing Kind **muss** ein Wording und ein Sort Order haben.
- Ein Thing **muss** ein Label; ein Kind; und ein Sort Order haben.
- Ein Portrait **muss** eine Display Name; eine Page; eine Image; ein Crop; ein Width; ein Height; und ein Sort Order haben.
- Eine Portrait Aliase **muss** auf genau einen Portrait verweisen.
- Ein Property **muss** ein Label; ein Property Kind; und ein Sort Order haben und erfassen, ob es ist eine User Addable; ob es hat eine One Current Value; und ob es ist ein Symmetric.
- Ein Property Axiom **muss** auf genau einen Firing Kind als seinen Kind verweisen.
- Ein Property Axiom **muss** auf genau einen Property verweisen.
- Ein Property Axiom **muss** auf genau einen Property als seinen First verweisen.
- Ein Statement **muss** auf genau einen Graph verweisen.
- Ein Statement **muss** auf genau einen Thing als seinen Subject verweisen.
- Ein Statement **muss** auf genau einen Property verweisen.
- Ein Statement **muss** ein Activation Seq und ein Sort Order haben und erfassen, ob es ist listed; ob es ist ein Built in; ob es ist ein Detail; und ob es ist stated.
- Ein Firing **muss** auf genau einen Firing Kind als seinen Kind verweisen.
- Ein Firing **muss** auf genau einen Rule verweisen.
- Ein Firing **muss** auf genau einen Statement als seinen Target verweisen.
- Ein Firing **muss** auf genau einen Statement als seinen First verweisen.
- Ein Disjoint Check **muss** auf genau einen Graph verweisen.
- Ein Disjoint Check **muss** auf genau einen Rule verweisen.
- Ein Disjoint Check **muss** auf genau einen Statement als seinen First verweisen.
- Ein Disjoint Check **muss** auf genau einen Statement als seinen Second verweisen.
- Ein Scenario **muss** auf genau einen Graph verweisen.
- Ein Scenario **muss** ein Label; ein Sort Order; ein Button Group; und eine Fact Baseline haben und erfassen, ob es ist ein Primary; ob es ist Clears Selection; und ob es ist Clears Form Message.
- Eine Scenario Fact Change **muss** auf genau einen Scenario verweisen.
- Eine Scenario Fact Change **muss** auf genau einen Thing als seinen Subject verweisen.
- Eine Scenario Fact Change **muss** auf genau einen Property verweisen.
- Eine Scenario Fact Change **muss** eine Action und ein Sort Order haben und erfassen, ob es ist Adds to List.
- Ein Scenario Rule Setting **muss** auf genau einen Scenario verweisen.
- Ein Scenario Rule Setting **muss** auf genau einen Rule verweisen.
- Ein Scenario Rule Setting **muss** erfassen, ob es ist enabled.
- Ein Node Layout **muss** ein Layout; ein Node Key; ein X; und ein Y haben.
- Ein Layout Slot **muss** ein Sort Order; ein X; und ein Y haben.
- Ein Page Text **muss** ein Text haben.
- Eine Engine **muss** ein Label; eine Substrate; eine Description; und ein Sort Order haben und erfassen, ob es ist ein Default.
- Ein Rulebook Tool **muss** ein Label; ein Adds; und ein Sort Order haben.

## 4 Definitorische Regeln

_Alle Aussagen drücken Wahrheiten der Geschäftsdomäne aus; sie sind weder Prozeduren
noch Imperative. Statt „genau dann, wenn“ wird „nur dann, wenn“ verwendet, damit eine
einseitige Notwendigkeit nicht mit einer Äquivalenz verwechselt wird. Ein
**⚠︎ mechanisch**-Hinweis markiert eine Regel, deren deterministische Formulierung treu,
aber hölzern ist — ein Signal für eine optionale Umformulierung, kein Fehler._

| ID | Deklarative Regel |
|----|------------------|
| **DR-1 Name** | Ein Graph: Name entspricht Label. |
| **DR-2 Count of Stated Facts** | Ein Graph: Count of Stated Facts ist die Gesamtsumme von In Effect Unit über die mit dem Graph verbundenen Statements. |
| **DR-3 Count of Conclusions** | Ein Graph: Count of Conclusions ist die Gesamtsumme von Conclusion Unit über die mit dem Graph verbundenen Statements. |
| **DR-4 Count of Violations** | Ein Graph: Count of Violations ist die Gesamtsumme von Violation Unit über die mit dem Graph verbundenen Disjoint Checks. |
| **DR-5 Is Consistent** | Ein Graph gilt als ein Consistent, wenn der Count of Violations 0 ist. |
| **DR-6 Conclusions Message** | Das Feld „Conclusions Message“ des Graph wird nach folgender Priorität bestimmt:<br>1. der No Conclusions Text, wenn der Count of Conclusions 0 ist;<br>2. andernfalls eine leere Zeichenkette. |
| **DR-7 Count of Facts to Add** | Ein Graph: Count of Facts to Add ist die Gesamtsumme von Listed Off Unit über die mit dem Graph verbundenen Statements. |
| **DR-8 Has Facts to Add** | Ein Graph gilt als über einen Facts to Add verfügend, wenn der Count of Facts to Add größer ist als 0. |
| **DR-9 Next Fact Sort Order** | Ein Graph: Next Fact Sort Order ist der kleinste Wert von Sort Order über die Statements des Graph, die Listed Off Unit sind. |
| **DR-10 Max Activation Seq** | Ein Graph: Max Activation Seq ist der größte Wert von Activation Seq über die mit dem Graph verbundenen Statements. |
| **DR-11 Next Activation Seq** | Ein Graph: Next Activation Seq wird berechnet als der Max Activation Seq plus 1. |
| **DR-12 Max Listed Sort Order** | Ein Graph: Max Listed Sort Order ist der größte Wert von Sort Order über die Statements des Graph, die Listed Unit sind. |
| **DR-13 Next Listed Sort Order** | Ein Graph: Next Listed Sort Order wird berechnet als der Max Listed Sort Order plus 1. |
| **DR-14 Max Sort Order** | Ein Graph: Max Sort Order ist der größte Wert von Sort Order über die mit dem Graph verbundenen Statements. |
| **DR-15 Next Candidate Sort Order** | Ein Graph: Next Candidate Sort Order wird berechnet als der Max Sort Order plus 1. |
| **DR-16 Name** | Eine Rule: Name entspricht Label. |
| **DR-17 Count of Firings** | Eine Rule: Count of Firings ist die Gesamtsumme von Fired Unit über die mit der Rule verbundenen Firings. |
| **DR-18 Name** | Ein Firing Kind: Name entspricht Wording. |
| **DR-19 Rule Enabled** | Ein Firing Kind: Rule Enabled ist wahr, wenn der zugehörige Rule des Firing Kind ist enabled. |
| **DR-20 Portrait Name** | Ein Thing: Portrait Name ist der Display Name des zugehörigen Portrait des Thing. |
| **DR-21 Portrait Page** | Ein Thing: Portrait Page — aus dem zugehörigen Portrait übernommen. |
| **DR-22 Portrait Image** | Ein Thing: Portrait Image — aus dem zugehörigen Portrait übernommen. |
| **DR-23 Portrait Crop** | Ein Thing: Portrait Crop — aus dem zugehörigen Portrait übernommen. |
| **DR-24 Portrait Width** | Ein Thing: Portrait Width — aus dem zugehörigen Portrait übernommen. |
| **DR-25 Portrait Height** | Ein Thing: Portrait Height — aus dem zugehörigen Portrait übernommen. |
| **DR-26 Name** | Ein Thing: Name entspricht Label. |
| **DR-27 Node Key** | Ein Thing: Node Key entspricht Thing ID. |
| **DR-28 Count of Children** | Ein Thing: Count of Children ist die Gesamtsumme von Parent Unit über die mit dem Thing verbundenen Statements. |
| **DR-29 Count of Grandchildren** | Ein Thing: Count of Grandchildren ist die Gesamtsumme von Grandparent Unit über die mit dem Thing verbundenen Statements. |
| **DR-30 Count As Place** | Ein Thing: Count As Place ist die Gesamtsumme von Place Unit über die mit dem Thing verbundenen Statements. |
| **DR-31 Count As Subject** | Ein Thing: Count As Subject ist die Gesamtsumme von Holds Unit über die mit dem Thing verbundenen Statements. |
| **DR-32 Count As Object** | Ein Thing: Count As Object ist die Gesamtsumme von Holds Unit über die mit dem Thing verbundenen Statements. |
| **DR-33 Is in Graph** | Ein Thing gilt als in graph, wenn mindestens eines des Folgenden gilt: der Count As Subject größer ist als 0 oder der Count As Object größer ist als 0. |
| **DR-34 Node Kind** | Das Feld „Node Kind“ des Thing wird nach folgender Priorität bestimmt:<br>1. eine leere Zeichenkette, wenn der Kind leer ist;<br>2. „class“, wenn der Kind „class“ ist;<br>3. „place“, wenn die Count As Place größer ist als 0;<br>4. andernfalls „person“. |
| **DR-35 Name** | Ein Portrait: Name entspricht Display Name. |
| **DR-36 Name** | Eine Portrait Aliase: Name entspricht Portrait Alias ID. |
| **DR-37 Name** | Ein Property: Name entspricht Label. |
| **DR-38 Name** | Ein Property Axiom: Name entspricht Property Axiom ID. |
| **DR-39 Subject Label** | Ein Statement: Subject Label — aus dem zugehörigen Subject übernommen. |
| **DR-40 Property Label** | Ein Statement: Property Label — aus dem zugehörigen Property übernommen. |
| **DR-41 Object Label** | Ein Statement: Object Label — aus dem zugehörigen Object Thing übernommen. |
| **DR-42 Property Datatype** | Ein Statement: Property Datatype — aus dem zugehörigen Property übernommen. |
| **DR-43 Property Has One Current Value** | Ein Statement: Property Has One Current Value ist wahr, wenn der zugehörige Property des Statement hat eine One Current Value. |
| **DR-44 Property Domain Class** | Ein Statement: Property Domain Class — aus dem zugehörigen Property übernommen. |
| **DR-45 Property Range Class** | Ein Statement: Property Range Class — aus dem zugehörigen Property übernommen. |
| **DR-46 Literal Quote** | Ein Statement: Literal Quote — aus dem zugehörigen Graph übernommen. |
| **DR-47 Literal Type Marker** | Ein Statement: Literal Type Marker — aus dem zugehörigen Graph übernommen. |
| **DR-48 Built in Source Text** | Ein Statement: Built in Source Text — aus dem zugehörigen Graph übernommen. |
| **DR-49 User Source Text** | Ein Statement: User Source Text — aus dem zugehörigen Graph übernommen. |
| **DR-50 Next Fact Sort Order** | Ein Statement: Next Fact Sort Order — aus dem zugehörigen Graph übernommen. |
| **DR-51 Is Literal** | Ein Statement gilt als ein Literal, wenn der Object Thing leer ist. |
| **DR-52 Object Text** | Das Feld „Object Text“ des Statement wird nach folgender Priorität bestimmt:<br>1. die Object Value, wenn der Object Thing leer ist;<br>2. andernfalls der Object Label. |
| **DR-53 Name** | Ein Statement: Name wird berechnet als der Subject Label, gefolgt von ein Leerzeichen, gefolgt von der Property Label, gefolgt von ein Leerzeichen, gefolgt von der Object Text. |
| **DR-54 Words** | Ein Statement: Words wird berechnet als der Subject Label, gefolgt von ein Leerzeichen, gefolgt von der Property Label, gefolgt von ein Leerzeichen, gefolgt von die Literal Quote, gefolgt von die Object Value, gefolgt von die Literal Quote, gefolgt von der Literal Type Marker, gefolgt von die Property Datatype, wenn der Object Thing leer ist, andernfalls der Object Label. ⚠︎ mechanisch <!-- rulespeak:reword --> |
| **DR-55 Source** | Das Feld „Source“ des Statement wird nach folgender Priorität bestimmt:<br>1. der Built in Source Text, wenn das Kennzeichen „Is Built in“ gesetzt ist;<br>2. andernfalls der User Source Text. |
| **DR-56 Subject Property Key** | Ein Statement: Subject Property Key wird berechnet als der Subject, gefolgt von „|“, gefolgt von der Property. |
| **DR-57 Latest Sibling Activation Seq** | Ein Statement: Latest Sibling Activation Seq ist der größte Wert von Activation Seq über die mit dem Statement verbundenen Statements. |
| **DR-58 Is in Effect** | Ein Statement gilt als in effect, wenn alles Folgende gilt: das Kennzeichen „Is Stated“ gesetzt ist und mindestens eines des Folgenden gilt: das Kennzeichen „Property Has One Current Value“ nicht gesetzt ist oder der Activation Seq der Latest Sibling Activation Seq ist. |
| **DR-59 Listed Unit** | Das Feld „Listed Unit“ des Statement wird nach folgender Priorität bestimmt:<br>1. 1, wenn das Kennzeichen „Is Listed“ gesetzt ist;<br>2. andernfalls 0. |
| **DR-60 In Effect Unit** | Das Feld „In Effect Unit“ des Statement wird nach folgender Priorität bestimmt:<br>1. 1, wenn das Kennzeichen „Is in Effect“ gesetzt ist;<br>2. andernfalls 0. |
| **DR-61 Listed Off Unit** | Das Feld „Listed Off Unit“ des Statement wird nach folgender Priorität bestimmt:<br>1. 1, wenn alles Folgende gilt: das Kennzeichen „Is Listed“ gesetzt ist und das Kennzeichen „Is in Effect“ nicht gesetzt ist;<br>2. andernfalls 0. |
| **DR-62 Is Next to Add** | Ein Statement gilt als ein Next to Add, wenn alles Folgende gilt: der Listed Off Unit 1 ist und der Sort Order der Next Fact Sort Order ist. |
| **DR-63 Count of Inverse Firings** | Ein Statement: Count of Inverse Firings ist die Gesamtsumme von Inverse Fired Unit über die mit dem Statement verbundenen Firings. |
| **DR-64 Count of Symmetric Firings** | Ein Statement: Count of Symmetric Firings ist die Gesamtsumme von Symmetric Fired Unit über die mit dem Statement verbundenen Firings. |
| **DR-65 Holds Before Chain** | Ein Statement gilt als Before Chain haltend, wenn mindestens eines des Folgenden gilt: das Kennzeichen „Is in Effect“ gesetzt ist; der Count of Inverse Firings größer ist als 0; oder der Count of Symmetric Firings größer ist als 0. |
| **DR-66 Count of Chain Firings** | Ein Statement: Count of Chain Firings ist die Gesamtsumme von Chain Fired Unit über die mit dem Statement verbundenen Firings. |
| **DR-67 Holds Before Typing** | Ein Statement gilt als Before Typing haltend, wenn mindestens eines des Folgenden gilt: das Kennzeichen „Holds Before Chain“ gesetzt ist oder der Count of Chain Firings größer ist als 0. |
| **DR-68 Count of Type Firings** | Ein Statement: Count of Type Firings ist die Gesamtsumme von Type Fired Unit über die mit dem Statement verbundenen Firings. |
| **DR-69 Holds** | Ein Statement gilt als haltend, wenn mindestens eines des Folgenden gilt: das Kennzeichen „Holds Before Typing“ gesetzt ist oder der Count of Type Firings größer ist als 0. |
| **DR-70 Is Conclusion** | Ein Statement gilt als eine Conclusion, wenn alles Folgende gilt: das Kennzeichen „Holds“ gesetzt ist und das Kennzeichen „Is in Effect“ nicht gesetzt ist. |
| **DR-71 Holds Unit** | Das Feld „Holds Unit“ des Statement wird nach folgender Priorität bestimmt:<br>1. 1, wenn das Kennzeichen „Holds“ gesetzt ist;<br>2. andernfalls 0. |
| **DR-72 Conclusion Unit** | Das Feld „Conclusion Unit“ des Statement wird nach folgender Priorität bestimmt:<br>1. 1, wenn das Kennzeichen „Is Conclusion“ gesetzt ist;<br>2. andernfalls 0. |
| **DR-73 Parent Unit** | Das Feld „Parent Unit“ des Statement wird nach folgender Priorität bestimmt:<br>1. 1, wenn alles Folgende gilt: das Kennzeichen „Holds“ gesetzt ist und der Property „isParentOf“ ist;<br>2. andernfalls 0. |
| **DR-74 Grandparent Unit** | Das Feld „Grandparent Unit“ des Statement wird nach folgender Priorität bestimmt:<br>1. 1, wenn alles Folgende gilt: das Kennzeichen „Holds“ gesetzt ist und der Property „isGrandparentOf“ ist;<br>2. andernfalls 0. |
| **DR-75 Place Unit** | Das Feld „Place Unit“ des Statement wird nach folgender Priorität bestimmt:<br>1. 1, wenn alles Folgende gilt: das Kennzeichen „Holds“ gesetzt ist und der Property „attends“ ist;<br>2. andernfalls 0. |
| **DR-76 First Pre Chain Order Key** | Ein Statement: First Pre Chain Order Key ist der kleinste Wert von Pre Chain Order Key über die Firings des Statement, die Pre Chain Fired Unit sind. |
| **DR-77 Snap Position** | Das Feld „Snap Position“ des Statement wird nach folgender Priorität bestimmt:<br>1. der Sort Order, wenn das Kennzeichen „Is in Effect“ gesetzt ist;<br>2. 100000 plus der First Pre Chain Order Key, wenn das Kennzeichen „Holds Before Chain“ gesetzt ist;<br>3. andernfalls 0. |
| **DR-78 First Fired Order Key** | Ein Statement: First Fired Order Key ist der kleinste Wert von Order Key über die Firings des Statement, die Fired Unit sind. |
| **DR-79 Count of Chosen Firings** | Ein Statement: Count of Chosen Firings ist die Gesamtsumme von Chosen Unit über die mit dem Statement verbundenen Firings. |
| **DR-80 Subject Node Key** | Ein Statement: Subject Node Key entspricht Subject. |
| **DR-81 Object Node Key** | Das Feld „Object Node Key“ des Statement wird nach folgender Priorität bestimmt:<br>1. „literal|“, gefolgt von die Object Value, gefolgt von „|“, gefolgt von die Property Datatype, wenn der Object Thing leer ist;<br>2. andernfalls der Object Thing. |
| **DR-82 Subject Node Kind** | Ein Statement: Subject Node Kind — aus dem zugehörigen Subject übernommen. |
| **DR-83 Object Thing Node Kind** | Ein Statement: Object Thing Node Kind — aus dem zugehörigen Object Thing übernommen. |
| **DR-84 Object Node Kind** | Das Feld „Object Node Kind“ des Statement wird nach folgender Priorität bestimmt:<br>1. „value“, wenn der Object Thing leer ist;<br>2. andernfalls der Object Thing Node Kind. |
| **DR-85 Object Value Node Label** | Ein Statement: Object Value Node Label — aus dem zugehörigen Property übernommen. |
| **DR-86 Count of Violations As First** | Ein Statement: Count of Violations As First ist die Gesamtsumme von Violation Unit über die mit dem Statement verbundenen Disjoint Checks. |
| **DR-87 Count of Violations As Second** | Ein Statement: Count of Violations As Second ist die Gesamtsumme von Violation Unit über die mit dem Statement verbundenen Disjoint Checks. |
| **DR-88 Is in Contradiction** | Ein Statement gilt als in contradiction, wenn mindestens eines des Folgenden gilt: der Count of Violations As First größer ist als 0 oder der Count of Violations As Second größer ist als 0. |
| **DR-89 Rule Enabled** | Ein Firing: Rule Enabled ist wahr, wenn der zugehörige Rule des Firing ist enabled. |
| **DR-90 Kind Wording** | Ein Firing: Kind Wording — aus dem zugehörigen Kind übernommen. |
| **DR-91 Kind Sort Order** | Ein Firing: Kind Sort Order — aus dem zugehörigen Kind übernommen. |
| **DR-92 Target Name** | Ein Firing: Target Name — aus dem zugehörigen Target übernommen. |
| **DR-93 Target Subject Label** | Ein Firing: Target Subject Label — aus dem zugehörigen Target übernommen. |
| **DR-94 Target Property Label** | Ein Firing: Target Property Label — aus dem zugehörigen Target übernommen. |
| **DR-95 Target Object Text** | Ein Firing: Target Object Text — aus dem zugehörigen Target übernommen. |
| **DR-96 Target is Conclusion** | Ein Firing: Target is Conclusion ist wahr, wenn der zugehörige Target des Firing ist eine Conclusion. |
| **DR-97 Target Subject Node Key** | Ein Firing: Target Subject Node Key — aus dem zugehörigen Target übernommen. |
| **DR-98 Target Object Node Key** | Ein Firing: Target Object Node Key — aus dem zugehörigen Target übernommen. |
| **DR-99 Target First Fired Order Key** | Ein Firing: Target First Fired Order Key — aus dem zugehörigen Target übernommen. |
| **DR-100 First Words** | Ein Firing: First Words — aus dem zugehörigen First übernommen. |
| **DR-101 Second Words** | Ein Firing: Second Words — aus dem zugehörigen Second übernommen. |
| **DR-102 First is in Effect** | Ein Firing: First is in Effect ist wahr, wenn der zugehörige First des Firing ist in effect. |
| **DR-103 First Holds Before Chain** | Ein Firing: First Holds Before Chain ist wahr, wenn der zugehörige First des Firing ist Holds Before Chain. |
| **DR-104 Second Holds Before Chain** | Ein Firing: Second Holds Before Chain ist wahr, wenn der zugehörige Second des Firing ist Holds Before Chain. |
| **DR-105 First Holds Before Typing** | Ein Firing: First Holds Before Typing ist wahr, wenn der zugehörige First des Firing ist Holds Before Typing. |
| **DR-106 First Property** | Ein Firing: First Property — aus dem zugehörigen First übernommen. |
| **DR-107 First Property Domain Class** | Ein Firing: First Property Domain Class — aus dem zugehörigen First übernommen. |
| **DR-108 First Property Range Class** | Ein Firing: First Property Range Class — aus dem zugehörigen First übernommen. |
| **DR-109 First Sort Order** | Ein Firing: First Sort Order — aus dem zugehörigen First übernommen. |
| **DR-110 Second is in Effect** | Ein Firing: Second is in Effect ist wahr, wenn der zugehörige Second des Firing ist in effect. |
| **DR-111 First Snap Position** | Ein Firing: First Snap Position — aus dem zugehörigen First übernommen. |
| **DR-112 Second Snap Position** | Ein Firing: Second Snap Position — aus dem zugehörigen Second übernommen. |
| **DR-113 Inverse Fired Unit** | Das Feld „Inverse Fired Unit“ des Firing wird nach folgender Priorität bestimmt:<br>1. 1, wenn alles Folgende gilt: das Kennzeichen „Rule Enabled“ gesetzt ist; der Kind „inverse“ ist; und das Kennzeichen „First is in Effect“ gesetzt ist;<br>2. andernfalls 0. |
| **DR-114 Symmetric Fired Unit** | Das Feld „Symmetric Fired Unit“ des Firing wird nach folgender Priorität bestimmt:<br>1. 1, wenn alles Folgende gilt: das Kennzeichen „Rule Enabled“ gesetzt ist; der Kind „symmetric“ ist; und das Kennzeichen „First is in Effect“ gesetzt ist;<br>2. andernfalls 0. |
| **DR-115 Chain Fired Unit** | Das Feld „Chain Fired Unit“ des Firing wird nach folgender Priorität bestimmt:<br>1. 1, wenn alles Folgende gilt: das Kennzeichen „Rule Enabled“ gesetzt ist; der Kind „chain“ ist; das Kennzeichen „First Holds Before Chain“ gesetzt ist; und das Kennzeichen „Second Holds Before Chain“ gesetzt ist;<br>2. andernfalls 0. |
| **DR-116 Type Fired Unit** | Das Feld „Type Fired Unit“ des Firing wird nach folgender Priorität bestimmt:<br>1. 1, wenn alles Folgende gilt: das Kennzeichen „Rule Enabled“ gesetzt ist; mindestens eines des Folgenden gilt: der Kind „domain“ ist oder der Kind „range“ ist; und das Kennzeichen „First Holds Before Typing“ gesetzt ist;<br>2. andernfalls 0. |
| **DR-117 Fired Unit** | Ein Firing: Fired Unit wird berechnet als der Inverse Fired Unit plus der Symmetric Fired Unit plus der Chain Fired Unit plus der Type Fired Unit. |
| **DR-118 Pre Chain Fired Unit** | Ein Firing: Pre Chain Fired Unit wird berechnet als der Inverse Fired Unit plus der Symmetric Fired Unit. |
| **DR-119 Pre Chain Order Key** | Ein Firing: Pre Chain Order Key wird berechnet als der First Sort Order mal 10 plus der Kind Sort Order. |
| **DR-120 Round** | Das Feld „Round“ des Firing wird nach folgender Priorität bestimmt:<br>1. 2, wenn alles Folgende gilt: der Kind „chain“ ist und es nicht der Fall ist, dass alles Folgende gilt: das Kennzeichen „First is in Effect“ gesetzt ist und das Kennzeichen „Second is in Effect“ gesetzt ist;<br>2. andernfalls 1. |
| **DR-121 Order Key** | Ein Firing: Order Key wird berechnet als der Round mal 1000000000000000 plus die First Snap Position mal 100000000 plus der Kind Sort Order mal 10000000 plus 0, wenn der Second leer ist, andernfalls die Second Snap Position. |
| **DR-122 Is Chosen** | Ein Firing gilt als ein Chosen, wenn alles Folgende gilt: der Fired Unit 1 ist; das Kennzeichen „Target is Conclusion“ gesetzt ist; und der Order Key der Target First Fired Order Key ist. |
| **DR-123 Chosen Unit** | Das Feld „Chosen Unit“ des Firing wird nach folgender Priorität bestimmt:<br>1. 1, wenn das Kennzeichen „Is Chosen“ gesetzt ist;<br>2. andernfalls 0. |
| **DR-124 Card Text** | Das Feld „Card Text“ des Firing wird nach folgender Priorität bestimmt:<br>1. „Worked out by the “, gefolgt von der Kind Wording, gefolgt von „ from ““, gefolgt von der First Words, gefolgt von „”“, gefolgt von „ and ““, gefolgt von der Second Words, gefolgt von „”“, wenn der Kind „chain“ ist, andernfalls „ and ““, gefolgt von der First Property, gefolgt von „ has domain “, gefolgt von der First Property Domain Class, gefolgt von „”“, wenn der Kind „domain“ ist, andernfalls „ and ““, gefolgt von der First Property, gefolgt von „ has range “, gefolgt von der First Property Range Class, gefolgt von „”“, gefolgt von ein Punkt, wenn das Kennzeichen „Is Chosen“ gesetzt ist;<br>2. andernfalls eine leere Zeichenkette. ⚠︎ mechanisch <!-- rulespeak:reword --> |
| **DR-125 Name** | Ein Firing: Name entspricht Firing ID. |
| **DR-126 Rule Enabled** | Ein Disjoint Check: Rule Enabled ist wahr, wenn der zugehörige Rule des Disjoint Check ist enabled. |
| **DR-127 First Holds** | Ein Disjoint Check: First Holds ist wahr, wenn der zugehörige First des Disjoint Check ist Holds. |
| **DR-128 Second Holds** | Ein Disjoint Check: Second Holds ist wahr, wenn der zugehörige Second des Disjoint Check ist Holds. |
| **DR-129 Subject Label** | Ein Disjoint Check: Subject Label — aus dem zugehörigen First übernommen. |
| **DR-130 Object Label** | Ein Disjoint Check: Object Label — aus dem zugehörigen First übernommen. |
| **DR-131 Violation Unit** | Das Feld „Violation Unit“ des Disjoint Check wird nach folgender Priorität bestimmt:<br>1. 1, wenn alles Folgende gilt: das Kennzeichen „Rule Enabled“ gesetzt ist; das Kennzeichen „First Holds“ gesetzt ist; und das Kennzeichen „Second Holds“ gesetzt ist;<br>2. andernfalls 0. |
| **DR-132 Alarm Text** | Das Feld „Alarm Text“ des Disjoint Check wird nach folgender Priorität bestimmt:<br>1. „Contradiction found. “, gefolgt von der Subject Label, gefolgt von „ is recorded as both parent and spouse of “, gefolgt von der Object Label, gefolgt von „. Check those facts and the selected disjoint rule.“, wenn der Violation Unit 1 ist;<br>2. andernfalls eine leere Zeichenkette. |
| **DR-133 Name** | Ein Disjoint Check: Name entspricht Disjoint Check ID. |
| **DR-134 Name** | Ein Scenario: Name entspricht Label. |
| **DR-135 Graph Has Facts to Add** | Ein Scenario: Graph Has Facts to Add ist wahr, wenn der zugehörige Graph des Scenario hat einen Facts to Add. |
| **DR-136 Is Available** | Ein Scenario gilt als eine Available, wenn mindestens eines des Folgenden gilt: die Fact Baseline nicht „next“ ist oder das Kennzeichen „Graph Has Facts to Add“ gesetzt ist. |
| **DR-137 Name** | Eine Scenario Fact Change: Name entspricht Scenario Fact Change ID. |
| **DR-138 Name** | Ein Scenario Rule Setting: Name entspricht Scenario Rule Setting ID. |
| **DR-139 Name** | Ein Node Layout: Name entspricht Node Layout ID. |
| **DR-140 Name** | Ein Layout Slot: Name entspricht Layout Slot ID. |
| **DR-141 Name** | Ein Page Text: Name entspricht Page Text ID. |
| **DR-142 Name** | Eine Engine: Name entspricht Label. |
| **DR-143 Name** | Ein Rulebook Tool: Name entspricht Label. |

## 5 Rückverfolgbarkeit zum Schema

_Die Ausdrucksspalte ist die Definition der Regel in Dokumentation-Notation —
dieselbe Logik, die das Regelbuch speichert, geschrieben für Fachleser._

| Schema-Element | Art | Ausdruck |
|----------------|------|------------|
| **Graphs.Name** | Formel | `Label` |
| **Graphs.CountOfStatedFacts** | Rollup | `Sum(Statements.InEffectUnit via Graph)` |
| **Graphs.CountOfConclusions** | Rollup | `Sum(Statements.ConclusionUnit via Graph)` |
| **Graphs.CountOfViolations** | Rollup | `Sum(DisjointChecks.ViolationUnit via Graph)` |
| **Graphs.IsConsistent** | Formel | `CountOfViolations = 0` |
| **Graphs.ConclusionsMessage** | Formel | `If(CountOfConclusions = 0, NoConclusionsText, "")` |
| **Graphs.CountOfFactsToAdd** | Rollup | `Sum(Statements.ListedOffUnit via Graph)` |
| **Graphs.HasFactsToAdd** | Formel | `CountOfFactsToAdd > 0` |
| **Graphs.NextFactSortOrder** | Rollup | `Min(Statements.SortOrder via Graph)` |
| **Graphs.MaxActivationSeq** | Rollup | `Max(Statements.ActivationSeq via Graph)` |
| **Graphs.NextActivationSeq** | Formel | `MaxActivationSeq + 1` |
| **Graphs.MaxListedSortOrder** | Rollup | `Max(Statements.SortOrder via Graph)` |
| **Graphs.NextListedSortOrder** | Formel | `MaxListedSortOrder + 1` |
| **Graphs.MaxSortOrder** | Rollup | `Max(Statements.SortOrder via Graph)` |
| **Graphs.NextCandidateSortOrder** | Formel | `MaxSortOrder + 1` |
| **Rules.Name** | Formel | `Label` |
| **Rules.CountOfFirings** | Rollup | `Sum(Firings.FiredUnit via Rule)` |
| **FiringKinds.Name** | Formel | `Wording` |
| **FiringKinds.RuleEnabled** | Nachschlagewert | `Lookup(Rules.IsEnabled via Rule)` |
| **Things.PortraitName** | Nachschlagewert | `Lookup(Portraits.DisplayName via Portrait)` |
| **Things.PortraitPage** | Nachschlagewert | `Lookup(Portraits.Page via Portrait)` |
| **Things.PortraitImage** | Nachschlagewert | `Lookup(Portraits.Image via Portrait)` |
| **Things.PortraitCrop** | Nachschlagewert | `Lookup(Portraits.Crop via Portrait)` |
| **Things.PortraitWidth** | Nachschlagewert | `Lookup(Portraits.Width via Portrait)` |
| **Things.PortraitHeight** | Nachschlagewert | `Lookup(Portraits.Height via Portrait)` |
| **Things.Name** | Formel | `Label` |
| **Things.NodeKey** | Formel | `ThingId` |
| **Things.CountOfChildren** | Rollup | `Sum(Statements.ParentUnit via Subject)` |
| **Things.CountOfGrandchildren** | Rollup | `Sum(Statements.GrandparentUnit via Subject)` |
| **Things.CountAsPlace** | Rollup | `Sum(Statements.PlaceUnit via ObjectThing)` |
| **Things.CountAsSubject** | Rollup | `Sum(Statements.HoldsUnit via Subject)` |
| **Things.CountAsObject** | Rollup | `Sum(Statements.HoldsUnit via ObjectThing)` |
| **Things.IsInGraph** | Formel | `Or(CountAsSubject > 0, CountAsObject > 0)` |
| **Things.NodeKind** | Formel | `If(Kind = "", "", If(Kind = "class", "class", If(CountAsPlace > 0, "place", "person")))` |
| **Portraits.Name** | Formel | `DisplayName` |
| **PortraitAliases.Name** | Formel | `PortraitAliasId` |
| **Properties.Name** | Formel | `Label` |
| **PropertyAxioms.Name** | Formel | `PropertyAxiomId` |
| **Statements.SubjectLabel** | Nachschlagewert | `Lookup(Things.Label via Subject)` |
| **Statements.PropertyLabel** | Nachschlagewert | `Lookup(Properties.Label via Property)` |
| **Statements.ObjectLabel** | Nachschlagewert | `Lookup(Things.Label via ObjectThing)` |
| **Statements.PropertyDatatype** | Nachschlagewert | `Lookup(Properties.Datatype via Property)` |
| **Statements.PropertyHasOneCurrentValue** | Nachschlagewert | `Lookup(Properties.HasOneCurrentValue via Property)` |
| **Statements.PropertyDomainClass** | Nachschlagewert | `Lookup(Properties.DomainClass via Property)` |
| **Statements.PropertyRangeClass** | Nachschlagewert | `Lookup(Properties.RangeClass via Property)` |
| **Statements.LiteralQuote** | Nachschlagewert | `Lookup(Graphs.LiteralQuote via Graph)` |
| **Statements.LiteralTypeMarker** | Nachschlagewert | `Lookup(Graphs.LiteralTypeMarker via Graph)` |
| **Statements.BuiltInSourceText** | Nachschlagewert | `Lookup(Graphs.BuiltInSourceText via Graph)` |
| **Statements.UserSourceText** | Nachschlagewert | `Lookup(Graphs.UserSourceText via Graph)` |
| **Statements.NextFactSortOrder** | Nachschlagewert | `Lookup(Graphs.NextFactSortOrder via Graph)` |
| **Statements.IsLiteral** | Formel | `ObjectThing = ""` |
| **Statements.ObjectText** | Formel | `If(ObjectThing = "", ObjectValue, ObjectLabel)` |
| **Statements.Name** | Formel | `Concat(SubjectLabel, " ", PropertyLabel, " ", ObjectText)` |
| **Statements.Words** | Formel | `Concat(SubjectLabel, " ", PropertyLabel, " ", If(ObjectThing = "", Concat(LiteralQuote, ObjectValue, LiteralQuote, LiteralTypeMarker, PropertyDatatype), ObjectLabel))` |
| **Statements.Source** | Formel | `If(IsBuiltIn, BuiltInSourceText, UserSourceText)` |
| **Statements.SubjectPropertyKey** | Formel | `Concat(Subject, "\|", Property)` |
| **Statements.LatestSiblingActivationSeq** | Rollup | `Max(Statements.ActivationSeq via SubjectPropertyKey)` |
| **Statements.IsInEffect** | Formel | `And(IsStated, Or(Not(PropertyHasOneCurrentValue), ActivationSeq = LatestSiblingActivationSeq))` |
| **Statements.ListedUnit** | Formel | `If(IsListed, 1, 0)` |
| **Statements.InEffectUnit** | Formel | `If(IsInEffect, 1, 0)` |
| **Statements.ListedOffUnit** | Formel | `If(And(IsListed, Not(IsInEffect)), 1, 0)` |
| **Statements.IsNextToAdd** | Formel | `And(ListedOffUnit = 1, SortOrder = NextFactSortOrder)` |
| **Statements.CountOfInverseFirings** | Rollup | `Sum(Firings.InverseFiredUnit via Target)` |
| **Statements.CountOfSymmetricFirings** | Rollup | `Sum(Firings.SymmetricFiredUnit via Target)` |
| **Statements.HoldsBeforeChain** | Formel | `Or(IsInEffect, CountOfInverseFirings > 0, CountOfSymmetricFirings > 0)` |
| **Statements.CountOfChainFirings** | Rollup | `Sum(Firings.ChainFiredUnit via Target)` |
| **Statements.HoldsBeforeTyping** | Formel | `Or(HoldsBeforeChain, CountOfChainFirings > 0)` |
| **Statements.CountOfTypeFirings** | Rollup | `Sum(Firings.TypeFiredUnit via Target)` |
| **Statements.Holds** | Formel | `Or(HoldsBeforeTyping, CountOfTypeFirings > 0)` |
| **Statements.IsConclusion** | Formel | `And(Holds, Not(IsInEffect))` |
| **Statements.HoldsUnit** | Formel | `If(Holds, 1, 0)` |
| **Statements.ConclusionUnit** | Formel | `If(IsConclusion, 1, 0)` |
| **Statements.ParentUnit** | Formel | `If(And(Holds, Property = "isParentOf"), 1, 0)` |
| **Statements.GrandparentUnit** | Formel | `If(And(Holds, Property = "isGrandparentOf"), 1, 0)` |
| **Statements.PlaceUnit** | Formel | `If(And(Holds, Property = "attends"), 1, 0)` |
| **Statements.FirstPreChainOrderKey** | Rollup | `Min(Firings.PreChainOrderKey via Target)` |
| **Statements.SnapPosition** | Formel | `If(IsInEffect, SortOrder, If(HoldsBeforeChain, 100000 + FirstPreChainOrderKey, 0))` |
| **Statements.FirstFiredOrderKey** | Rollup | `Min(Firings.OrderKey via Target)` |
| **Statements.CountOfChosenFirings** | Rollup | `Sum(Firings.ChosenUnit via Target)` |
| **Statements.SubjectNodeKey** | Formel | `Subject` |
| **Statements.ObjectNodeKey** | Formel | `If(ObjectThing = "", Concat("literal\|", ObjectValue, "\|", PropertyDatatype), ObjectThing)` |
| **Statements.SubjectNodeKind** | Nachschlagewert | `Lookup(Things.NodeKind via Subject)` |
| **Statements.ObjectThingNodeKind** | Nachschlagewert | `Lookup(Things.NodeKind via ObjectThing)` |
| **Statements.ObjectNodeKind** | Formel | `If(ObjectThing = "", "value", ObjectThingNodeKind)` |
| **Statements.ObjectValueNodeLabel** | Nachschlagewert | `Lookup(Properties.ValueNodeLabel via Property)` |
| **Statements.CountOfViolationsAsFirst** | Rollup | `Sum(DisjointChecks.ViolationUnit via First)` |
| **Statements.CountOfViolationsAsSecond** | Rollup | `Sum(DisjointChecks.ViolationUnit via Second)` |
| **Statements.IsInContradiction** | Formel | `Or(CountOfViolationsAsFirst > 0, CountOfViolationsAsSecond > 0)` |
| **Firings.RuleEnabled** | Nachschlagewert | `Lookup(Rules.IsEnabled via Rule)` |
| **Firings.KindWording** | Nachschlagewert | `Lookup(FiringKinds.Wording via Kind)` |
| **Firings.KindSortOrder** | Nachschlagewert | `Lookup(FiringKinds.SortOrder via Kind)` |
| **Firings.TargetName** | Nachschlagewert | `Lookup(Statements.Name via Target)` |
| **Firings.TargetSubjectLabel** | Nachschlagewert | `Lookup(Statements.SubjectLabel via Target)` |
| **Firings.TargetPropertyLabel** | Nachschlagewert | `Lookup(Statements.PropertyLabel via Target)` |
| **Firings.TargetObjectText** | Nachschlagewert | `Lookup(Statements.ObjectText via Target)` |
| **Firings.TargetIsConclusion** | Nachschlagewert | `Lookup(Statements.IsConclusion via Target)` |
| **Firings.TargetSubjectNodeKey** | Nachschlagewert | `Lookup(Statements.SubjectNodeKey via Target)` |
| **Firings.TargetObjectNodeKey** | Nachschlagewert | `Lookup(Statements.ObjectNodeKey via Target)` |
| **Firings.TargetFirstFiredOrderKey** | Nachschlagewert | `Lookup(Statements.FirstFiredOrderKey via Target)` |
| **Firings.FirstWords** | Nachschlagewert | `Lookup(Statements.Words via First)` |
| **Firings.SecondWords** | Nachschlagewert | `Lookup(Statements.Words via Second)` |
| **Firings.FirstIsInEffect** | Nachschlagewert | `Lookup(Statements.IsInEffect via First)` |
| **Firings.FirstHoldsBeforeChain** | Nachschlagewert | `Lookup(Statements.HoldsBeforeChain via First)` |
| **Firings.SecondHoldsBeforeChain** | Nachschlagewert | `Lookup(Statements.HoldsBeforeChain via Second)` |
| **Firings.FirstHoldsBeforeTyping** | Nachschlagewert | `Lookup(Statements.HoldsBeforeTyping via First)` |
| **Firings.FirstProperty** | Nachschlagewert | `Lookup(Statements.Property via First)` |
| **Firings.FirstPropertyDomainClass** | Nachschlagewert | `Lookup(Statements.PropertyDomainClass via First)` |
| **Firings.FirstPropertyRangeClass** | Nachschlagewert | `Lookup(Statements.PropertyRangeClass via First)` |
| **Firings.FirstSortOrder** | Nachschlagewert | `Lookup(Statements.SortOrder via First)` |
| **Firings.SecondIsInEffect** | Nachschlagewert | `Lookup(Statements.IsInEffect via Second)` |
| **Firings.FirstSnapPosition** | Nachschlagewert | `Lookup(Statements.SnapPosition via First)` |
| **Firings.SecondSnapPosition** | Nachschlagewert | `Lookup(Statements.SnapPosition via Second)` |
| **Firings.InverseFiredUnit** | Formel | `If(And(RuleEnabled, Kind = "inverse", FirstIsInEffect), 1, 0)` |
| **Firings.SymmetricFiredUnit** | Formel | `If(And(RuleEnabled, Kind = "symmetric", FirstIsInEffect), 1, 0)` |
| **Firings.ChainFiredUnit** | Formel | `If(And(RuleEnabled, Kind = "chain", FirstHoldsBeforeChain, SecondHoldsBeforeChain), 1, 0)` |
| **Firings.TypeFiredUnit** | Formel | `If(And(RuleEnabled, Or(Kind = "domain", Kind = "range"), FirstHoldsBeforeTyping), 1, 0)` |
| **Firings.FiredUnit** | Formel | `InverseFiredUnit + SymmetricFiredUnit + ChainFiredUnit + TypeFiredUnit` |
| **Firings.PreChainFiredUnit** | Formel | `InverseFiredUnit + SymmetricFiredUnit` |
| **Firings.PreChainOrderKey** | Formel | `FirstSortOrder * 10 + KindSortOrder` |
| **Firings.Round** | Formel | `If(And(Kind = "chain", Not(And(FirstIsInEffect, SecondIsInEffect))), 2, 1)` |
| **Firings.OrderKey** | Formel | `Round * 1000000000000000 + FirstSnapPosition * 100000000 + KindSortOrder * 10000000 + If(Second = "", 0, SecondSnapPosition)` |
| **Firings.IsChosen** | Formel | `And(FiredUnit = 1, TargetIsConclusion, OrderKey = TargetFirstFiredOrderKey)` |
| **Firings.ChosenUnit** | Formel | `If(IsChosen, 1, 0)` |
| **Firings.CardText** | Formel | `If(IsChosen, Concat("Worked out by the ", KindWording, " from “", FirstWords, "”", If(Kind = "chain", Concat(" and “", SecondWords, "”"), If(Kind = "domain", Concat(" and “", FirstProperty, " has domain ", FirstPropertyDomainClass, "”"), If(Kind = "range", Concat(" and “", FirstProperty, " has range ", FirstPropertyRangeClass, "”"), ""))), "."), "")` |
| **Firings.Name** | Formel | `FiringId` |
| **DisjointChecks.RuleEnabled** | Nachschlagewert | `Lookup(Rules.IsEnabled via Rule)` |
| **DisjointChecks.FirstHolds** | Nachschlagewert | `Lookup(Statements.Holds via First)` |
| **DisjointChecks.SecondHolds** | Nachschlagewert | `Lookup(Statements.Holds via Second)` |
| **DisjointChecks.SubjectLabel** | Nachschlagewert | `Lookup(Statements.SubjectLabel via First)` |
| **DisjointChecks.ObjectLabel** | Nachschlagewert | `Lookup(Statements.ObjectLabel via First)` |
| **DisjointChecks.ViolationUnit** | Formel | `If(And(RuleEnabled, FirstHolds, SecondHolds), 1, 0)` |
| **DisjointChecks.AlarmText** | Formel | `If(ViolationUnit = 1, Concat("Contradiction found. ", SubjectLabel, " is recorded as both parent and spouse of ", ObjectLabel, ". Check those facts and the selected disjoint rule."), "")` |
| **DisjointChecks.Name** | Formel | `DisjointCheckId` |
| **Scenarios.Name** | Formel | `Label` |
| **Scenarios.GraphHasFactsToAdd** | Nachschlagewert | `Lookup(Graphs.HasFactsToAdd via Graph)` |
| **Scenarios.IsAvailable** | Formel | `Or(FactBaseline <> "next", GraphHasFactsToAdd)` |
| **ScenarioFactChanges.Name** | Formel | `ScenarioFactChangeId` |
| **ScenarioRuleSettings.Name** | Formel | `ScenarioRuleSettingId` |
| **NodeLayouts.Name** | Formel | `NodeLayoutId` |
| **LayoutSlots.Name** | Formel | `LayoutSlotId` |
| **PageTexts.Name** | Formel | `PageTextId` |
| **Engines.Name** | Formel | `Label` |
| **RulebookTools.Name** | Formel | `Label` |

---

> **RuleSpeak®** — die von **Ronald G. Ross** geschaffene deklarative Geschäftsregel-Notation nach den Konventionen von **SBVR** — kommt bald. Bis dahin zeigt diese Seite die reguläre Dokumentation. [www.RonRoss.info](https://www.RonRoss.info)
