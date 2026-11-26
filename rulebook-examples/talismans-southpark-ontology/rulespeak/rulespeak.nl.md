# 📘 Talisman's South Park ontology explainer — Documentatie

_Jessica Talisman's South Park Ontology Explainer, as a rulebook: the page's ten facts, five rule switches, property wiring, provenance wording and current-value policy are rows, and every conclusion is a formula. The rules apply in three fixed strata (inverse and symmetric, then the property chain, then domain and range); property wiring that feeds an earlier stratum would need repeated rounds._

> Declaratieve bedrijfsregels, gegenereerd uit het regelboek. Elke uitspraak
> drukt een waarheid van het bedrijfsdomein uit — het is geen procedure en geen
> imperatief. De formules van het regelboek zijn de enige bron van waarheid;
> dit document is hun lezing in klare taal.

## 1 Bedrijfsvocabulaire

| Term | Beschrijving | Toelichting |
|------|-------------|-------------------|
| **Graph** | The explainer page as one graph: its wording variables and its totals. | — |
| Label | Een gedefinieerd attribuut. | _Display name of the graph._ |
| Literal Quote | Een gedefinieerd attribuut. | _The quote mark written around a literal value in a fact's wording._ |
| Literal Type Marker | Een gedefinieerd attribuut. | _The marker written between a literal value and its datatype, as in Turtle._ |
| Built in Source Text | Een gedefinieerd attribuut. | _Source line shown for a fact the page states as a teaching example._ |
| User Source Text | Een gedefinieerd attribuut. | _Source line shown for a fact the reader stated._ |
| No Conclusions Text | Een gedefinieerd attribuut. | _Message shown when the selected facts and rules add no conclusion._ |
| Name | Gelijk aan label. | _Order 1. Display name of the graph._ |
| Count of Stated Facts | Het totaal van in effect unit over de statements gekoppeld aan de graph. | _Order 5. Facts that are switched on and in effect._ |
| Count of Conclusions | Het totaal van conclusion unit over de statements gekoppeld aan de graph. | _Order 18. Facts that hold only because an enabled rule concluded them._ |
| Count of Violations | Het totaal van violation unit over de disjoint checks gekoppeld aan de graph. | _Order 18. Disjointness checks that found both links between the same pair._ |
| Is Consistent | Waar wanneer de count of violations 0 is. | _Order 19. True when no enabled disjointness rule is violated._ |
| Conclusions Message | Bepaald op prioriteit: de no conclusions text als de count of conclusions 0 is; in alle andere gevallen een lege tekenreeks. | _Order 19. The empty-state message for the conclusions panel, blank when there are conclusions._ |
| Count of Facts to Add | Het totaal van listed off unit over de statements gekoppeld aan de graph. | _Order 5. Listed facts that are not in effect; the Add next fact button is enabled while this is above zero._ |
| Has Facts to Add | Waar wanneer de count of facts to add groter is dan 0. | _Order 6. True while some listed fact can still be added._ |
| Next Fact Sort Order | De laagste waarde van sort order over de statements van de graph die listed off unit zijn. | _Order 5. List position of the first listed fact that is not in effect._ |
| Max Activation Seq | De hoogste waarde van activation seq over de statements gekoppeld aan de graph. | _Order 1. Highest activation number used so far._ |
| Next Activation Seq | Berekend als de max activation seq plus 1. | _Order 2. The activation number the next switched-on fact takes._ |
| Max Listed Sort Order | De hoogste waarde van sort order over de statements van de graph die listed unit zijn. | _Order 2. Highest list position among listed facts._ |
| Next Listed Sort Order | Berekend als de max listed sort order plus 1. | _Order 3. The list position a newly added fact takes._ |

<details open>
<summary>Voorbeeldgegevens</summary>

| Label | Literal Quote | Literal Type Marker | Built in Source Text | User Source Text | No Conclusions Text | Name ƒ | Count of Stated Facts ƒ | Count of Conclusions ƒ | Count of Violations ƒ | Is Consistent ƒ | Conclusions Message ƒ | Count of Facts to Add ƒ | Has Facts to Add ƒ | Next Fact Sort Order ƒ | Max Activation Seq ƒ | Next Activation Seq ƒ | Max Listed Sort Order ƒ | Next Listed Sort Order ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| South Park | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | These facts and selected rules add no further conclusions. Try another fact or rule. | South Park | 10 | 0 | 0 | true | These facts and selected rules add no further conclusions. Try another fact or rule. | 0 | false | — | 10 | 11 | 10 | 11 |

_ƒ markeert een berekende kolom._

</details>

| Term | Beschrijving | Toelichting |
|------|-------------|-------------------|
| **Rule** | The five rule switches on the page. | — |
| Label | Een gedefinieerd attribuut. | _The rule's name on the page._ |
| Explanation | Een gedefinieerd attribuut. | _The rule's one-line explanation on the page._ |
| Is Enabled | Waar wanneer een lege tekenreeks. | _The rule's checkbox._ |
| Sort Order | Een gedefinieerd attribuut. | _Position in the rule list._ |
| Name | Gelijk aan label. | _Order 1. The rule's name._ |
| Count of Firings | Het totaal van fired unit over de firings gekoppeld aan de rule. | _Order 15. Rule applications that fired, counting every derivation._ |

<details open>
<summary>Voorbeeldgegevens</summary>

| Label | Explanation | Is Enabled | Sort Order | Name ƒ | Count of Firings ƒ |
|---|---|---|---|---|---|
| Inverse | A parent link also gives a child link between the same two people. | false | 1 | Inverse | 0 |
| Symmetric | A friend link or a spouse link is true for both people. | false | 2 | Symmetric | 0 |
| Property chain | A parent of a parent is a grandparent. | false | 3 | Property chain | 0 |

_ƒ markeert een berekende kolom._

</details>

| Term | Beschrijving | Toelichting |
|------|-------------|-------------------|
| **Firing Kind** | The ways a rule can apply. Domain and range belong to one rule but are worded apart. | — |
| Rule | Een gedefinieerd attribuut. | _The rule switch this kind belongs to._ |
| Wording | Een gedefinieerd attribuut. | _How a conclusion card names the rule, as in 'Worked out by the inverse rule'._ |
| Sort Order | Een gedefinieerd attribuut. | _Order in which the page's reasoner tries the kinds for one fact._ |
| Name | Gelijk aan wording. | _Order 1. The kind's wording._ |
| Rule Enabled | Waar wanneer de gekoppelde rule is enabled. | _Order 1. Whether the owning rule is switched on._ |

<details open>
<summary>Voorbeeldgegevens</summary>

| Wording | Sort Order | Name ƒ | Rule Enabled ƒ |
|---|---|---|---|
| inverse rule | 1 | inverse rule | false |
| symmetric rule | 2 | symmetric rule | false |
| property chain rule | 3 | property chain rule | false |

_ƒ markeert een berekende kolom._

</details>

| Term | Beschrijving | Toelichting |
|------|-------------|-------------------|
| **Thing** | Every individual and class the page can draw. | — |
| Label | Een gedefinieerd attribuut. | _The name the page shows._ |
| Kind | Een gedefinieerd attribuut. | _'individual' or 'class'._ |
| Sort Order | Een gedefinieerd attribuut. | _Position in the cast._ |
| Portrait Name | Een gedefinieerd attribuut. | _The name on the portrait's Fandom page._ |
| Portrait Page | Een gedefinieerd attribuut. | _The Fandom page credited for the portrait._ |
| Portrait Image | Een gedefinieerd attribuut. | _The original Fandom image._ |
| Portrait Crop | Een gedefinieerd attribuut. | _The portrait's crop as an SVG viewBox: x y width height._ |
| Portrait Width | Een gedefinieerd attribuut. | _The original image's width in pixels._ |
| Portrait Height | Een gedefinieerd attribuut. | _The original image's height in pixels._ |
| Name | Gelijk aan label. | _Order 1. The thing's name._ |
| Node Key | Gelijk aan thing ID. | _Order 1. The key the graph draws this thing under._ |
| Count of Children | Het totaal van parent unit over de statements gekoppeld aan de thing. | _Order 17. People this thing is a parent of, stated or concluded._ |
| Count of Grandchildren | Het totaal van grandparent unit over de statements gekoppeld aan de thing. | _Order 17. People this thing is a grandparent of._ |
| Count As Place | Het totaal van place unit over de statements gekoppeld aan de thing. | _Order 17. Holding attends facts whose object is this thing._ |
| Count As Subject | Het totaal van holds unit over de statements gekoppeld aan de thing. | _Order 17. Holding facts about this thing._ |
| Count As Object | Het totaal van holds unit over de statements gekoppeld aan de thing. | _Order 17. Holding facts that point at this thing._ |
| Is in Graph | Waar wanneer ten minste één van het volgende geldt: de count as subject groter is dan 0 of de count as object groter is dan 0. | _Order 18. True when the graph draws this thing._ |
| Node Kind | Bepaald op prioriteit: ‘class’ als de kind ‘class’ is; ‘place’ als de count as place groter is dan 0; in alle andere gevallen ‘person’. | _Order 18. How the graph draws the thing: a class, a place someone attends, or a person._ |

<details open>
<summary>Voorbeeldgegevens</summary>

| Label | Kind | Sort Order | Portrait Name | Portrait Page | Portrait Image | Portrait Crop | Portrait Width | Portrait Height | Name ƒ | Node Key ƒ | Count of Children ƒ | Count of Grandchildren ƒ | Count As Place ƒ | Count As Subject ƒ | Count As Object ƒ | Is in Graph ƒ | Node Kind ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| Marvin | individual | 1 | Marvin Marsh | https://southpark.fandom.com/wiki/Marvin_Marsh | https://static.wikia.nocookie.net/southpark/images/d/db/Marsh-family-grandpa-marsh-w-purple-blanket.png/revision/latest | 48 -5 280 280 | 375 | 536 | Marvin | Marvin | 1 | 0 | 0 | 1 | 0 | true | person |
| Randy | individual | 2 | Randy Marsh | https://southpark.fandom.com/wiki/Randy_Marsh | https://static.wikia.nocookie.net/southparkfanon/images/d/d6/Randy_Marsh.png/revision/latest | 0 -10 428 428 | 428 | 1057 | Randy | Randy | 2 | 0 | 0 | 3 | 1 | true | person |
| Sharon | individual | 3 | Sharon Marsh | https://southpark.fandom.com/wiki/Sharon_Marsh | https://static.wikia.nocookie.net/southpark/images/9/99/Sharon_Marsh.png/revision/latest | 20 -10 390 390 | 439 | 1049 | Sharon | Sharon | 0 | 0 | 0 | 0 | 1 | true | person |

_ƒ markeert een berekende kolom._

</details>

| Term | Beschrijving | Toelichting |
|------|-------------|-------------------|
| **Property** | The ten properties and the wiring every rule reads. | — |
| Label | Een gedefinieerd attribuut. | _The property's wording on the page._ |
| Property Kind | Een gedefinieerd attribuut. | _'object' links two things, 'datatype' links a thing to a value, 'type' states class membership._ |
| Datatype | Een gedefinieerd attribuut. | _The value's XML Schema datatype for a datatype property; blank otherwise._ |
| Is User Addable | Waar wanneer een lege tekenreeks. | _Offered in the page's Add fact form._ |
| Has One Current Value | Waar wanneer een lege tekenreeks. | _The page keeps one current value per person: switching a value on replaces the earlier one._ |
| Inverse of | Een gedefinieerd attribuut. | _The property the inverse rule derives from this one._ |
| Is Symmetric | Waar wanneer een lege tekenreeks. | _The symmetric rule makes this link true in both directions._ |
| Chain First | Een gedefinieerd attribuut. | _First link of the property chain that derives this property._ |
| Chain Second | Een gedefinieerd attribuut. | _Second link of the property chain that derives this property._ |
| Domain Class | Een gedefinieerd attribuut. | _The class the domain rule gives this property's subject._ |
| Range Class | Een gedefinieerd attribuut. | _The class the range rule gives this property's object._ |
| Disjoint With | Een gedefinieerd attribuut. | _A property that may not link the same pair, in the same direction, as this one._ |
| Sort Order | Een gedefinieerd attribuut. | _Position in the Add fact form's property list._ |
| Value Node Label | Een gedefinieerd attribuut. | _The small label under a value node for this property: 'role value' or 'number value'._ |
| Current Value Wording | Een gedefinieerd attribuut. | _How the Add fact form names a replaced value, as in 'Stan now has grade level 5'._ |
| Input Name | Een gedefinieerd attribuut. | _How the integer check names this property's value, as in 'for grade level'._ |
| Record Kind | Een gedefinieerd attribuut. | _The word the current-record summary uses for this property's values: role, grade or school._ |
| Record Order | Een gedefinieerd attribuut. | _Position of this property in the current-record summary._ |
| Name | Gelijk aan label. | _Order 1. The property's wording._ |

<details open>
<summary>Voorbeeldgegevens</summary>

| Label | Property Kind | Datatype | Is User Addable | Has One Current Value | Is Symmetric | Sort Order | Value Node Label | Current Value Wording | Input Name | Record Kind | Record Order | Name ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|
| is parent of | object | — | true | false | false | 1 | — | — | — | — | — | is parent of |
| is child of | object | — | true | false | false | 2 | — | — | — | — | — | is child of |
| is spouse of | object | — | true | false | true | 3 | — | — | — | — | — | is spouse of |

_ƒ markeert een berekende kolom._

</details>

| Term | Beschrijving | Toelichting |
|------|-------------|-------------------|
| **Statement** | Every fact the page can state or conclude about the cast. A listed fact appears in the page's fact list; the rest are candidates that only a rule can make hold. | — |
| Graph | Een gedefinieerd attribuut. | _The graph the fact belongs to._ |
| Subject | Een gedefinieerd attribuut. | _What the fact describes._ |
| Property | Een gedefinieerd attribuut. | _How the subject connects to the object._ |
| Object Thing | Een gedefinieerd attribuut. | _The related thing, for an object or type property._ |
| Object Value | Een gedefinieerd attribuut. | _The literal value, for a datatype property._ |
| Is Listed | Waar wanneer een lege tekenreeks. | _Shown in the page's fact list._ |
| Is Built in | Waar wanneer een lege tekenreeks. | _One of the page's teaching facts; the reader cannot remove it._ |
| Is Detail | Waar wanneer een lege tekenreeks. | _One of Stan's detail facts that Show Stan's details switches on._ |
| Is Stated | Waar wanneer een lege tekenreeks. | _The fact's checkbox._ |
| Activation Seq | Een gedefinieerd attribuut. | _When the fact was last switched on, as a running number; decides which value is current._ |
| Sort Order | Een gedefinieerd attribuut. | _Position in the fact list; candidates follow the listed facts._ |
| Subject Label | Overgenomen van de gekoppelde subject. | _Order 1. The subject's name._ |
| Property Label | Overgenomen van de gekoppelde property. | _Order 1. The property's wording._ |
| Object Label | Overgenomen van de gekoppelde object thing. | _Order 1. The object's name, for an object or type property._ |
| Property Datatype | Overgenomen van de gekoppelde property. | _Order 1. The value's datatype, for a datatype property._ |
| Property Has One Current Value | Waar wanneer de gekoppelde property heeft een one current value. | _Order 1. Whether the property keeps one current value per person._ |
| Property Domain Class | Overgenomen van de gekoppelde property. | _Order 1. The property's domain class._ |
| Property Range Class | Overgenomen van de gekoppelde property. | _Order 1. The property's range class._ |
| Literal Quote | Overgenomen van de gekoppelde graph. | _Order 1. The graph's literal quote mark._ |
| Literal Type Marker | Overgenomen van de gekoppelde graph. | _Order 1. The graph's datatype marker._ |
| Built in Source Text | Overgenomen van de gekoppelde graph. | _Order 1. The graph's source line for a teaching fact._ |
| User Source Text | Overgenomen van de gekoppelde graph. | _Order 1. The graph's source line for a reader's fact._ |
| Next Fact Sort Order | Overgenomen van de gekoppelde graph. | _Order 6. List position of the next fact to add._ |
| Is Literal | Waar wanneer de object thing leeg is. | _Order 1. True when the object is a value rather than a thing._ |
| Object Text | Bepaald op prioriteit: de object value als de object thing leeg is; in alle andere gevallen de object label. | _Order 2. The object's name or the value._ |
| Name | Berekend als de subject label, gevolgd door een spatie, gevolgd door de property label, gevolgd door een spatie, gevolgd door de object text. | _Order 3. The fact in words._ |
| Words | Berekend als de subject label, gevolgd door een spatie, gevolgd door de property label, gevolgd door een spatie, gevolgd door de literal quote, gevolgd door de object value, gevolgd door de literal quote, gevolgd door de literal type marker, gevolgd door de property datatype als de object thing leeg is, in alle andere gevallen de object label. ⚠︎ mechanisch <!-- rulespeak:reword --> | _Order 2. The fact as a conclusion card quotes it, with a literal written in Turtle form._ |
| Source | Bepaald op prioriteit: de built in source text als de vlag ‘built in’ is gezet; in alle andere gevallen de user source text. | _Order 2. Where a listed fact came from._ |
| Subject Property Key | Berekend als de subject, gevolgd door ‘|’, gevolgd door de property. | _Order 1. Groups the values one subject has for one property._ |
| Latest Sibling Activation Seq | De hoogste waarde van activation seq over de statements gekoppeld aan het statement. | _Order 2. The most recent activation among this subject's values for this property._ |
| Is in Effect | Waar wanneer al het volgende geldt: de vlag ‘stated’ is gezet en ten minste één van het volgende geldt: de vlag ‘property has one current value’ niet is gezet of de activation seq de latest sibling activation seq is. | _Order 3. Switched on and, for a one-value property, the most recently switched-on value. Drawn as a solid line._ |
| Listed Unit | Bepaald op prioriteit: 1 als de vlag ‘listed’ is gezet; in alle andere gevallen 0. | _Order 1. 1 when the fact is in the fact list._ |
| In Effect Unit | Bepaald op prioriteit: 1 als de vlag ‘in effect’ is gezet; in alle andere gevallen 0. | _Order 4. 1 when the fact is in effect._ |
| Listed Off Unit | Bepaald op prioriteit: 1 als al het volgende geldt: de vlag ‘listed’ is gezet en de vlag ‘in effect’ niet is gezet; in alle andere gevallen 0. | _Order 4. 1 when the fact is listed but not in effect._ |
| Is Next to Add | Waar wanneer al het volgende geldt: de listed off unit 1 is en de sort order de next fact sort order is. | _Order 7. The fact the Add next fact button switches on._ |
| Count of Inverse Firings | Het totaal van inverse fired unit over de firings gekoppeld aan het statement. | _Order 6. Inverse rule applications that conclude this fact._ |
| Count of Symmetric Firings | Het totaal van symmetric fired unit over de firings gekoppeld aan het statement. | _Order 6. Symmetric rule applications that conclude this fact._ |
| Holds Before Chain | Waar wanneer ten minste één van het volgende geldt: de vlag ‘in effect’ is gezet; de count of inverse firings groter is dan 0; of de count of symmetric firings groter is dan 0. | _Order 7. Holds from facts in effect plus the inverse and symmetric rules._ |
| Count of Chain Firings | Het totaal van chain fired unit over de firings gekoppeld aan het statement. | _Order 10. Property chain applications that conclude this fact._ |
| Holds Before Typing | Waar wanneer ten minste één van het volgende geldt: de vlag ‘holds before chain’ is gezet of de count of chain firings groter is dan 0. | _Order 11. Holds before the domain and range rules apply._ |
| Count of Type Firings | Het totaal van type fired unit over de firings gekoppeld aan het statement. | _Order 14. Domain and range applications that conclude this fact._ |
| Holds | Waar wanneer ten minste één van het volgende geldt: de vlag ‘holds before typing’ is gezet of de count of type firings groter is dan 0. | _Order 15. True when in effect or concluded by an enabled rule; the graph draws it._ |
| Is Conclusion | Waar wanneer al het volgende geldt: de vlag ‘holds’ is gezet en de vlag ‘in effect’ niet is gezet. | _Order 16. Holds only because a rule concluded it. Drawn dashed._ |
| Holds Unit | Bepaald op prioriteit: 1 als de vlag ‘holds’ is gezet; in alle andere gevallen 0. | _Order 16. 1 when the fact holds._ |
| Conclusion Unit | Bepaald op prioriteit: 1 als de vlag ‘conclusion’ is gezet; in alle andere gevallen 0. | _Order 17. 1 when the fact is a conclusion._ |
| Parent Unit | Bepaald op prioriteit: 1 als al het volgende geldt: de vlag ‘holds’ is gezet en de property ‘isParentOf’ is; in alle andere gevallen 0. | _Order 16. 1 for a holding parent link._ |
| Grandparent Unit | Bepaald op prioriteit: 1 als al het volgende geldt: de vlag ‘holds’ is gezet en de property ‘isGrandparentOf’ is; in alle andere gevallen 0. | _Order 16. 1 for a holding grandparent link._ |
| Place Unit | Bepaald op prioriteit: 1 als al het volgende geldt: de vlag ‘holds’ is gezet en de property ‘attends’ is; in alle andere gevallen 0. | _Order 16. 1 for a holding attends link._ |
| First Fired Order Key | De laagste waarde van order key over de firings van het statement die fired unit zijn. | _Order 15. Order key of the first rule application that concludes this fact, in the page reasoner's order._ |
| Count of Chosen Firings | Het totaal van chosen unit over de firings gekoppeld aan het statement. | _Order 20. Rule applications shown as this fact's conclusion card: one for a conclusion, zero otherwise._ |
| Subject Node Key | Gelijk aan subject. | _Order 1. The graph node this fact starts from._ |
| Object Node Key | Bepaald op prioriteit: ‘literal|’, gevolgd door de object value, gevolgd door ‘|’, gevolgd door de property datatype als de object thing leeg is; in alle andere gevallen de object thing. | _Order 2. The graph node this fact points at: the thing, or one node per value and datatype._ |
| Subject Node Kind | Overgenomen van de gekoppelde subject. | _Order 19. How the graph draws the subject._ |
| Object Thing Node Kind | Overgenomen van de gekoppelde object thing. | _Order 19. How the graph draws the object thing._ |
| Object Node Kind | Bepaald op prioriteit: ‘value’ als de object thing leeg is; in alle andere gevallen de object thing node kind. | _Order 20. How the graph draws the object: a value, a class, a place or a person._ |
| Object Value Node Label | Overgenomen van de gekoppelde property. | _Order 1. The small label under a value node._ |
| Count of Violations As First | Het totaal van violation unit over de disjoint checks gekoppeld aan het statement. | _Order 18. Violated disjointness checks naming this fact first._ |
| Count of Violations As Second | Het totaal van violation unit over de disjoint checks gekoppeld aan het statement. | _Order 18. Violated disjointness checks naming this fact second._ |
| Is in Contradiction | Waar wanneer ten minste één van het volgende geldt: de count of violations as first groter is dan 0 of de count of violations as second groter is dan 0. | _Order 19. Part of a contradiction the disjoint rule found; drawn in the alarm colour._ |

<details open>
<summary>Voorbeeldgegevens</summary>

| Object Value | Is Listed | Is Built in | Is Detail | Is Stated | Activation Seq | Sort Order | Subject Label ƒ | Property Label ƒ | Object Label ƒ | Property Datatype ƒ | Property Has One Current Value ƒ | Property Domain Class ƒ | Property Range Class ƒ | Literal Quote ƒ | Literal Type Marker ƒ | Built in Source Text ƒ | User Source Text ƒ | Next Fact Sort Order ƒ | Is Literal ƒ | Object Text ƒ | Name ƒ | Words ƒ | Source ƒ | Subject Property Key ƒ | Latest Sibling Activation Seq ƒ | Is in Effect ƒ | Listed Unit ƒ | In Effect Unit ƒ | Listed Off Unit ƒ | Is Next to Add ƒ | Count of Inverse Firings ƒ | Count of Symmetric Firings ƒ | Holds Before Chain ƒ | Count of Chain Firings ƒ | Holds Before Typing ƒ | Count of Type Firings ƒ | Holds ƒ | Is Conclusion ƒ | Holds Unit ƒ | Conclusion Unit ƒ | Parent Unit ƒ | Grandparent Unit ƒ | Place Unit ƒ | First Fired Order Key ƒ | Count of Chosen Firings ƒ | Subject Node Key ƒ | Object Node Key ƒ | Subject Node Kind ƒ | Object Thing Node Kind ƒ | Object Node Kind ƒ | Object Value Node Label ƒ | Count of Violations As First ƒ | Count of Violations As Second ƒ | Is in Contradiction ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| — | true | true | false | true | 1 | 1 | Marvin | is parent of | Randy | — | false | — | — | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | — | false | Randy | Marvin is parent of Randy | Marvin is parent of Randy | Stated as a teaching example on this page. | Marvin\|isParentOf | 1 | true | 1 | 1 | 0 | false | 0 | 0 | true | 0 | true | 0 | true | false | 1 | 0 | 1 | 0 | 0 | — | 0 | Marvin | Randy | person | person | person | — | 0 | 0 | false |
| — | false | false | false | false | 0 | 101 | Marvin | is parent of | Sharon | — | false | — | — | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | — | false | Sharon | Marvin is parent of Sharon | Marvin is parent of Sharon | Stated by you on this page. | Marvin\|isParentOf | 1 | false | 0 | 0 | 0 | false | 0 | 0 | false | 0 | false | 0 | false | false | 0 | 0 | 0 | 0 | 0 | — | 0 | Marvin | Sharon | person | person | person | — | 0 | 0 | false |
| — | false | false | false | false | 0 | 102 | Marvin | is parent of | Stan | — | false | — | — | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | — | false | Stan | Marvin is parent of Stan | Marvin is parent of Stan | Stated by you on this page. | Marvin\|isParentOf | 1 | false | 0 | 0 | 0 | false | 0 | 0 | false | 0 | false | 0 | false | false | 0 | 0 | 0 | 0 | 0 | — | 0 | Marvin | Stan | person | person | person | — | 0 | 0 | false |

_ƒ markeert een berekende kolom._

</details>

| Term | Beschrijving | Toelichting |
|------|-------------|-------------------|
| **Firing** | Every way an enabled rule could conclude a candidate fact from the facts that hold. | — |
| Kind | Een gedefinieerd attribuut. | _How the rule applies._ |
| Rule | Een gedefinieerd attribuut. | _The rule switch._ |
| Target | Een gedefinieerd attribuut. | _The fact this application concludes._ |
| First | Een gedefinieerd attribuut. | _The premise._ |
| Second | Een gedefinieerd attribuut. | _The second premise, for the property chain._ |
| Rule Enabled | Waar wanneer de gekoppelde rule is enabled. | _Order 1. Whether the rule is switched on._ |
| Kind Wording | Overgenomen van de gekoppelde kind. | _Order 1. How the card names the rule._ |
| Kind Sort Order | Overgenomen van de gekoppelde kind. | _Order 1. The kind's place in the reasoner's order._ |
| Target Name | Overgenomen van de gekoppelde target. | _Order 4. The concluded fact in words._ |
| Target Subject Label | Overgenomen van de gekoppelde target. | _Order 2. The concluded fact's subject._ |
| Target Property Label | Overgenomen van de gekoppelde target. | _Order 2. The concluded fact's property._ |
| Target Object Text | Overgenomen van de gekoppelde target. | _Order 3. The concluded fact's object._ |
| Target is Conclusion | Waar wanneer de gekoppelde target is een conclusion. | _Order 17. Whether the concluded fact is new._ |
| Target Subject Node Key | Overgenomen van de gekoppelde target. | _Order 2. The concluded fact's subject node._ |
| Target Object Node Key | Overgenomen van de gekoppelde target. | _Order 3. The concluded fact's object node._ |
| Target First Fired Order Key | Overgenomen van de gekoppelde target. | _Order 16. The order key of the concluded fact's first application._ |
| First Words | Overgenomen van de gekoppelde first. | _Order 3. The premise as the card quotes it._ |
| Second Words | Overgenomen van de gekoppelde second. | _Order 3. The second premise as the card quotes it._ |
| First is in Effect | Waar wanneer de gekoppelde first is in effect. | _Order 4. Whether the premise is in effect._ |
| First Holds Before Chain | Waar wanneer de gekoppelde first is holds before chain. | _Order 8. Whether the premise holds before the chain._ |
| Second Holds Before Chain | Waar wanneer de gekoppelde second is holds before chain. | _Order 8. Whether the second premise holds before the chain._ |
| First Holds Before Typing | Waar wanneer de gekoppelde first is holds before typing. | _Order 12. Whether the premise holds before typing._ |
| First Property | Overgenomen van de gekoppelde first. | _Order 1. The premise's property key, as the card's axiom names it._ |
| First Property Domain Class | Overgenomen van de gekoppelde first. | _Order 2. The premise property's domain class._ |
| First Property Range Class | Overgenomen van de gekoppelde first. | _Order 2. The premise property's range class._ |
| First Sort Order | Overgenomen van de gekoppelde first. | _Order 1. The premise's list position._ |
| Second Sort Order | Overgenomen van de gekoppelde second. | _Order 1. The second premise's list position._ |
| Inverse Fired Unit | Bepaald op prioriteit: 1 als al het volgende geldt: de vlag ‘rule enabled’ is gezet; de kind ‘inverse’ is; en de vlag ‘first is in effect’ is gezet; in alle andere gevallen 0. | _Order 5. 1 when the inverse rule fires here._ |
| Symmetric Fired Unit | Bepaald op prioriteit: 1 als al het volgende geldt: de vlag ‘rule enabled’ is gezet; de kind ‘symmetric’ is; en de vlag ‘first is in effect’ is gezet; in alle andere gevallen 0. | _Order 5. 1 when the symmetric rule fires here._ |
| Chain Fired Unit | Bepaald op prioriteit: 1 als al het volgende geldt: de vlag ‘rule enabled’ is gezet; de kind ‘chain’ is; de vlag ‘first holds before chain’ is gezet; en de vlag ‘second holds before chain’ is gezet; in alle andere gevallen 0. | _Order 9. 1 when the property chain fires here._ |
| Type Fired Unit | Bepaald op prioriteit: 1 als al het volgende geldt: de vlag ‘rule enabled’ is gezet; ten minste één van het volgende geldt: de kind ‘domain’ is of de kind ‘range’ is; en de vlag ‘first holds before typing’ is gezet; in alle andere gevallen 0. | _Order 13. 1 when the domain or range rule fires here._ |
| Fired Unit | Berekend als de inverse fired unit plus de symmetric fired unit plus de chain fired unit plus de type fired unit. | _Order 14. 1 when this application fires._ |
| Order Key | Berekend als de first sort order maal 100000 plus de kind sort order maal 1000 plus 0 als de second leeg is, in alle andere gevallen de second sort order. | _Order 2. The page reasoner's order: by premise, then rule kind, then second premise._ |
| Is Chosen | Waar wanneer al het volgende geldt: de fired unit 1 is; de vlag ‘target is conclusion’ is gezet; en de order key de target first fired order key is. | _Order 18. The application the conclusions panel shows for its fact: the first one in the reasoner's order._ |
| Chosen Unit | Bepaald op prioriteit: 1 als de vlag ‘chosen’ is gezet; in alle andere gevallen 0. | _Order 19. 1 when this application is shown._ |
| Card Text | Bepaald op prioriteit: ‘Worked out by the ’, gevolgd door de kind wording, gevolgd door ‘ from “’, gevolgd door de first words, gevolgd door ‘”’, gevolgd door ‘ and “’, gevolgd door de second words, gevolgd door ‘”’ als de kind ‘chain’ is, in alle andere gevallen ‘ and “’, gevolgd door de first property, gevolgd door ‘ has domain ’, gevolgd door de first property domain class, gevolgd door ‘”’ als de kind ‘domain’ is, in alle andere gevallen ‘ and “’, gevolgd door de first property, gevolgd door ‘ has range ’, gevolgd door de first property range class, gevolgd door ‘”’, gevolgd door een punt als de vlag ‘chosen’ is gezet; in alle andere gevallen een lege tekenreeks. ⚠︎ mechanisch <!-- rulespeak:reword --> | _Order 19. The conclusion card's provenance line, in the page's wording._ |
| Name | Gelijk aan firing ID. | _Order 1. The application's key._ |

<details open>
<summary>Voorbeeldgegevens</summary>

| Rule Enabled ƒ | Kind Wording ƒ | Kind Sort Order ƒ | Target Name ƒ | Target Subject Label ƒ | Target Property Label ƒ | Target Object Text ƒ | Target is Conclusion ƒ | Target Subject Node Key ƒ | Target Object Node Key ƒ | Target First Fired Order Key ƒ | First Words ƒ | Second Words ƒ | First is in Effect ƒ | First Holds Before Chain ƒ | Second Holds Before Chain ƒ | First Holds Before Typing ƒ | First Property ƒ | First Property Domain Class ƒ | First Property Range Class ƒ | First Sort Order ƒ | Second Sort Order ƒ | Inverse Fired Unit ƒ | Symmetric Fired Unit ƒ | Chain Fired Unit ƒ | Type Fired Unit ƒ | Fired Unit ƒ | Order Key ƒ | Is Chosen ƒ | Chosen Unit ƒ | Card Text ƒ | Name ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| false | inverse rule | 1 | Randy is child of Marvin | Randy | is child of | Marvin | false | Randy | Marvin | — | Marvin is parent of Randy |    | true | true | false | true | isParentOf | — | — | 1 | — | 0 | 0 | 0 | 0 | 0 | 101000 | false | 0 | — | inverse:Marvin\|isParentOf\|Randy>Randy\|isChildOf\|Marvin |
| false | inverse rule | 1 | Sharon is child of Marvin | Sharon | is child of | Marvin | false | Sharon | Marvin | — | Marvin is parent of Sharon |    | false | false | false | false | isParentOf | — | — | 101 | — | 0 | 0 | 0 | 0 | 0 | 10101000 | false | 0 | — | inverse:Marvin\|isParentOf\|Sharon>Sharon\|isChildOf\|Marvin |
| false | inverse rule | 1 | Stan is child of Marvin | Stan | is child of | Marvin | false | Stan | Marvin | — | Marvin is parent of Stan |    | false | false | false | false | isParentOf | — | — | 102 | — | 0 | 0 | 0 | 0 | 0 | 10201000 | false | 0 | — | inverse:Marvin\|isParentOf\|Stan>Stan\|isChildOf\|Marvin |

_ƒ markeert een berekende kolom._

</details>

| Term | Beschrijving | Toelichting |
|------|-------------|-------------------|
| **Disjoint Check** | Every pair the disjointness rule watches: the same two people linked by two disjoint properties. | — |
| Graph | Een gedefinieerd attribuut. | _The graph._ |
| Rule | Een gedefinieerd attribuut. | _The rule switch._ |
| First | Een gedefinieerd attribuut. | _The first link._ |
| Second | Een gedefinieerd attribuut. | _The disjoint link between the same pair, in the same direction._ |
| Rule Enabled | Waar wanneer de gekoppelde rule is enabled. | _Order 1. Whether the disjointness rule is on._ |
| First Holds | Waar wanneer de gekoppelde first is holds. | _Order 16. Whether the first link holds._ |
| Second Holds | Waar wanneer de gekoppelde second is holds. | _Order 16. Whether the second link holds._ |
| Subject Label | Overgenomen van de gekoppelde first. | _Order 2. The person recorded with both links._ |
| Object Label | Overgenomen van de gekoppelde first. | _Order 2. The other person._ |
| Violation Unit | Bepaald op prioriteit: 1 als al het volgende geldt: de vlag ‘rule enabled’ is gezet; de vlag ‘first holds’ is gezet; en de vlag ‘second holds’ is gezet; in alle andere gevallen 0. | _Order 17. 1 when both links hold._ |
| Alarm Text | Bepaald op prioriteit: ‘Contradiction found. ’, gevolgd door de subject label, gevolgd door ‘ is recorded as both parent and spouse of ’, gevolgd door de object label, gevolgd door ‘. Check those facts and the selected disjoint rule.’ als de violation unit 1 is; in alle andere gevallen een lege tekenreeks. | _Order 18. The page's contradiction alert, blank when there is none._ |
| Name | Gelijk aan disjoint check ID. | _Order 1. The check's key._ |

<details open>
<summary>Voorbeeldgegevens</summary>

| Rule Enabled ƒ | First Holds ƒ | Second Holds ƒ | Subject Label ƒ | Object Label ƒ | Violation Unit ƒ | Alarm Text ƒ | Name ƒ |
|---|---|---|---|---|---|---|---|
| false | true | false | Marvin | Randy | 0 | — | Marvin\|isParentOf\|Randy~isSpouseOf |
| false | false | false | Marvin | Sharon | 0 | — | Marvin\|isParentOf\|Sharon~isSpouseOf |
| false | false | false | Marvin | Stan | 0 | — | Marvin\|isParentOf\|Stan~isSpouseOf |

_ƒ markeert een berekende kolom._

</details>

| Term | Beschrijving | Toelichting |
|------|-------------|-------------------|
| **Scenario** | The page's buttons. Each one is a fact baseline, explicit fact changes and rule settings, applied in that order. | — |
| Graph | Een gedefinieerd attribuut. | _The graph._ |
| Label | Een gedefinieerd attribuut. | _The button's text._ |
| Note | Een gedefinieerd attribuut. | _The note shown under the buttons after this one is pressed; blank clears it._ |
| Sort Order | Een gedefinieerd attribuut. | _Position among the buttons._ |
| Button Group | Een gedefinieerd attribuut. | _'stan' for the two imagined changes, 'facts' for the buttons under the fact list._ |
| Is Primary | Waar wanneer een lege tekenreeks. | _Drawn as the filled button._ |
| Fact Baseline | Een gedefinieerd attribuut. | _What happens to every listed fact first: keep, built-in (teaching facts on, others off), built-in-detail, all-off, activate-all (each switched on in list order), or next (the next fact on)._ |
| Layout | Een gedefinieerd attribuut. | _The node layout the graph returns to; blank keeps the current positions._ |
| Clears Selection | Waar wanneer een lege tekenreeks. | _Clears the selected node or line._ |
| Clears Form Message | Waar wanneer een lege tekenreeks. | _Clears the Add fact form's message._ |
| Name | Gelijk aan label. | _Order 1. The button's text._ |
| Graph Has Facts to Add | Waar wanneer de gekoppelde graph heeft een facts to add. | _Order 7. Whether a listed fact can still be added._ |
| Is Available | Waar wanneer ten minste één van het volgende geldt: de fact baseline niet ‘next’ is of de vlag ‘graph has facts to add’ is gezet. | _Order 8. False only for Add next fact once every listed fact is in effect._ |

<details open>
<summary>Voorbeeldgegevens</summary>

| Label | Note | Sort Order | Button Group | Is Primary | Fact Baseline | Layout | Clears Selection | Clears Form Message | Name ƒ | Graph Has Facts to Add ƒ | Is Available ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|
| Move Stan to grade 5 | This imagined change updates Stan’s grade to 5. His identifier, Person statement and family links stay the same. His other selected details are unchanged. | 1 | stan | false | keep | — | true | true | Move Stan to grade 5 | false | true |
| Stan leaves school | This imagined change clears Stan’s current school details. His identifier, Person statement and family links remain. Removing a current fact does not say that its opposite is true; school history needs dates. | 2 | stan | false | keep | — | true | true | Stan leaves school | false | true |
| Add next fact | — | 3 | facts | true | next | — | false | false | Add next fact | false | false |

_ƒ markeert een berekende kolom._

</details>

| Term | Beschrijving | Toelichting |
|------|-------------|-------------------|
| **Scenario Fact Change** | Fact changes a button makes after its baseline. A blank object key matches every listed value of the property. | — |
| Scenario | Een gedefinieerd attribuut. | _The button._ |
| Subject | Een gedefinieerd attribuut. | _The subject whose facts change._ |
| Property | Een gedefinieerd attribuut. | _The property whose facts change._ |
| Object Key | Een gedefinieerd attribuut. | _The object thing's key or the literal value; blank for every listed value._ |
| Action | Een gedefinieerd attribuut. | _'on' switches the fact on as the newest value; 'off' switches it off._ |
| Adds to List | Waar wanneer een lege tekenreeks. | _Adds the fact to the fact list first when it is not listed._ |
| Sort Order | Een gedefinieerd attribuut. | _Order of the change within the button._ |
| Name | Gelijk aan scenario fact change ID. | _Order 1. The change's key._ |

<details open>
<summary>Voorbeeldgegevens</summary>

| Object Key | Action | Adds to List | Sort Order | Name ƒ |
|---|---|---|---|---|
| Person | on | true | 1 | stan-grade-five:Stan\|isA\|Person |
| 5 | on | true | 2 | stan-grade-five:Stan\|hasGradeLevel\|5 |
| Person | on | true | 3 | stan-leaves-school:Stan\|isA\|Person |

_ƒ markeert een berekende kolom._

</details>

| Term | Beschrijving | Toelichting |
|------|-------------|-------------------|
| **Scenario Rule Setting** | Rule switches a button sets. | — |
| Scenario | Een gedefinieerd attribuut. | _The button._ |
| Rule | Een gedefinieerd attribuut. | _The rule switch._ |
| Is Enabled | Waar wanneer een lege tekenreeks. | _The state the button puts the switch in._ |
| Name | Gelijk aan scenario rule setting ID. | _Order 1. The setting's key._ |

<details open>
<summary>Voorbeeldgegevens</summary>

| Is Enabled | Name ƒ |
|---|---|
| false | reset-original:inverse |
| false | reset-original:symmetric |
| false | reset-original:chain |

_ƒ markeert een berekende kolom._

</details>

| Term | Beschrijving | Toelichting |
|------|-------------|-------------------|
| **Node Layout** | Node positions in the graph's 380 by 480 drawing, one set per layout. | — |
| Layout | Een gedefinieerd attribuut. | _The layout: 'default' or 'stan-details'._ |
| Node Key | Een gedefinieerd attribuut. | _The node: a thing's key, or literal\|value\|datatype._ |
| X | Een gedefinieerd attribuut. | _Horizontal position._ |
| Y | Een gedefinieerd attribuut. | _Vertical position._ |
| Name | Gelijk aan node layout ID. | _Order 1. The position's key._ |

<details open>
<summary>Voorbeeldgegevens</summary>

| Layout | Node Key | X | Y | Name ƒ |
|---|---|---|---|---|
| default | Marvin | 70 | 45 | default:Marvin |
| default | Randy | 195 | 125 | default:Randy |
| default | Sharon | 318 | 55 | default:Sharon |

_ƒ markeert een berekende kolom._

</details>

| Term | Beschrijving | Toelichting |
|------|-------------|-------------------|
| **Layout Slot** | Places for nodes no layout positions, used in turn. | — |
| Sort Order | Een gedefinieerd attribuut. | _Order in which slots are used._ |
| X | Een gedefinieerd attribuut. | _Horizontal position._ |
| Y | Een gedefinieerd attribuut. | _Vertical position._ |
| Name | Gelijk aan layout slot ID. | _Order 1. The slot's key._ |

<details open>
<summary>Voorbeeldgegevens</summary>

| Sort Order | X | Y | Name ƒ |
|---|---|---|---|
| 1 | 330 | 135 | slot-1 |
| 2 | 45 | 120 | slot-2 |
| 3 | 330 | 400 | slot-3 |

_ƒ markeert een berekende kolom._

</details>

| Term | Beschrijving | Toelichting |
|------|-------------|-------------------|
| **Page Text** | The page's wording, one row per string. {name}-style placeholders are filled by the page. | — |
| Text | Een gedefinieerd attribuut. | _The string._ |
| Name | Gelijk aan page text ID. | _Order 1. The string's key._ |

<details open>
<summary>Voorbeeldgegevens</summary>

| Text | Name ƒ |
|---|---|
| South Park Ontology Explainer, on one rulebook | page-title |
| A rulebook rebuild of the explainer by | byline-prefix |
| Jessica Talisman | byline-author |

_ƒ markeert een berekende kolom._

</details>

## 2 Feittypen

- een **firing kind** verwijst naar precies één **rule**
- een **property** kan naar één **property** verwijzen
- een **property** kan naar één **thing** verwijzen
- een **statement** verwijst naar precies één **graph**
- een **statement** verwijst naar precies één **thing**
- een **statement** verwijst naar precies één **property**
- een **firing** verwijst naar precies één **firing kind**
- een **firing** verwijst naar precies één **rule**
- een **firing** verwijst naar precies één **statement**
- een **disjoint check** verwijst naar precies één **graph**
- een **disjoint check** verwijst naar precies één **rule**
- een **disjoint check** verwijst naar precies één **statement**
- een **scenario** verwijst naar precies één **graph**
- een **scenario fact change** verwijst naar precies één **scenario**
- een **scenario fact change** verwijst naar precies één **thing**
- een **scenario fact change** verwijst naar precies één **property**
- een **scenario rule setting** verwijst naar precies één **scenario**
- een **scenario rule setting** verwijst naar precies één **rule**

## 3 Operationele regels

_Operationele regels benoemen wat het bedrijf **verplicht**, **verbiedt** of
aanraadt (**zou moeten**). Structurele regels komen uit verplichte velden en
vreemde sleutels; semantische regels uit de Constraints-tabel, elk steunend op een
boolean die het regelboek al berekent (verwezen als DR-N in de definitieregels)._

### Structurele beperkingen (uit het schema)

- Een graph **moet** een label; een literal quote; een literal type marker; een built in source text; een user source text; en een no conclusions text hebben.
- Een rule **moet** een label; een explanation; en een sort order hebben, en vastleggen of het is enabled.
- Een firing kind **moet** naar precies één rule verwijzen.
- Een firing kind **moet** een wording en een sort order hebben.
- Een thing **moet** een label; een kind; en een sort order hebben.
- Een property **moet** een label; een property kind; en een sort order hebben, en vastleggen of het is user addable; of het heeft een one current value; en of het is symmetric.
- Een statement **moet** naar precies één graph verwijzen.
- Een statement **moet** naar precies één thing verwijzen als zijn subject.
- Een statement **moet** naar precies één property verwijzen.
- Een statement **moet** een activation seq en een sort order hebben, en vastleggen of het is listed; of het is een built in; of het is een detail; en of het is stated.
- Een firing **moet** naar precies één firing kind verwijzen als zijn kind.
- Een firing **moet** naar precies één rule verwijzen.
- Een firing **moet** naar precies één statement verwijzen als zijn target.
- Een firing **moet** naar precies één statement verwijzen als zijn first.
- Een disjoint check **moet** naar precies één graph verwijzen.
- Een disjoint check **moet** naar precies één rule verwijzen.
- Een disjoint check **moet** naar precies één statement verwijzen als zijn first.
- Een disjoint check **moet** naar precies één statement verwijzen als zijn second.
- Een scenario **moet** naar precies één graph verwijzen.
- Een scenario **moet** een label; een sort order; een button group; en een fact baseline hebben, en vastleggen of het is primary; of het is clears selection; en of het is clears form message.
- Een scenario fact change **moet** naar precies één scenario verwijzen.
- Een scenario fact change **moet** naar precies één thing verwijzen als zijn subject.
- Een scenario fact change **moet** naar precies één property verwijzen.
- Een scenario fact change **moet** een action en een sort order hebben, en vastleggen of het is adds to list.
- Een scenario rule setting **moet** naar precies één scenario verwijzen.
- Een scenario rule setting **moet** naar precies één rule verwijzen.
- Een scenario rule setting **moet** vastleggen of het is enabled.
- Een node layout **moet** een layout; een node key; een x; en een y hebben.
- Een layout slot **moet** een sort order; een x; en een y hebben.
- Een page text **moet** een text hebben.

## 4 Definitieregels

_Alle uitspraken drukken waarheden van het bedrijfsdomein uit; het zijn geen
procedures en geen imperatieven. ‘dan en slechts dan’ wordt vermeden ten gunste van
‘alleen als’, zodat een eenzijdige noodzaak niet wordt aangezien voor een equivalentie.
Een **⚠︎ mechanisch**-markering duidt een regel aan waarvan de deterministische
formulering trouw maar houterig is — een signaal voor optionele herformulering, geen fout._

| ID | Declaratieve regel |
|----|------------------|
| **DR-1 Name** | Een graph: name is gelijk aan label. |
| **DR-2 Count of Stated Facts** | Een graph: count of stated facts is het totaal van in effect unit over de statements gekoppeld aan de graph. |
| **DR-3 Count of Conclusions** | Een graph: count of conclusions is het totaal van conclusion unit over de statements gekoppeld aan de graph. |
| **DR-4 Count of Violations** | Een graph: count of violations is het totaal van violation unit over de disjoint checks gekoppeld aan de graph. |
| **DR-5 Is Consistent** | Een graph geldt als consistent als de count of violations 0 is. |
| **DR-6 Conclusions Message** | Het veld ‘conclusions message’ van de graph wordt bepaald volgens de volgende prioriteit:<br>1. de no conclusions text, als de count of conclusions 0 is;<br>2. in alle andere gevallen een lege tekenreeks. |
| **DR-7 Count of Facts to Add** | Een graph: count of facts to add is het totaal van listed off unit over de statements gekoppeld aan de graph. |
| **DR-8 Has Facts to Add** | Een graph geldt als beschikkend over een facts to add als de count of facts to add groter is dan 0. |
| **DR-9 Next Fact Sort Order** | Een graph: next fact sort order is de laagste waarde van sort order over de statements van de graph die listed off unit zijn. |
| **DR-10 Max Activation Seq** | Een graph: max activation seq is de hoogste waarde van activation seq over de statements gekoppeld aan de graph. |
| **DR-11 Next Activation Seq** | Een graph: next activation seq wordt berekend als de max activation seq plus 1. |
| **DR-12 Max Listed Sort Order** | Een graph: max listed sort order is de hoogste waarde van sort order over de statements van de graph die listed unit zijn. |
| **DR-13 Next Listed Sort Order** | Een graph: next listed sort order wordt berekend als de max listed sort order plus 1. |
| **DR-14 Name** | Een rule: name is gelijk aan label. |
| **DR-15 Count of Firings** | Een rule: count of firings is het totaal van fired unit over de firings gekoppeld aan de rule. |
| **DR-16 Name** | Een firing kind: name is gelijk aan wording. |
| **DR-17 Rule Enabled** | Een firing kind: rule enabled geldt wanneer de gekoppelde rule is enabled. |
| **DR-18 Name** | Een thing: name is gelijk aan label. |
| **DR-19 Node Key** | Een thing: node key is gelijk aan thing ID. |
| **DR-20 Count of Children** | Een thing: count of children is het totaal van parent unit over de statements gekoppeld aan de thing. |
| **DR-21 Count of Grandchildren** | Een thing: count of grandchildren is het totaal van grandparent unit over de statements gekoppeld aan de thing. |
| **DR-22 Count As Place** | Een thing: count as place is het totaal van place unit over de statements gekoppeld aan de thing. |
| **DR-23 Count As Subject** | Een thing: count as subject is het totaal van holds unit over de statements gekoppeld aan de thing. |
| **DR-24 Count As Object** | Een thing: count as object is het totaal van holds unit over de statements gekoppeld aan de thing. |
| **DR-25 Is in Graph** | Een thing geldt als in graph als ten minste één van het volgende geldt: de count as subject groter is dan 0 of de count as object groter is dan 0. |
| **DR-26 Node Kind** | Het veld ‘node kind’ van de thing wordt bepaald volgens de volgende prioriteit:<br>1. ‘class’, als de kind ‘class’ is;<br>2. ‘place’, als de count as place groter is dan 0;<br>3. in alle andere gevallen ‘person’. |
| **DR-27 Name** | Een property: name is gelijk aan label. |
| **DR-28 Subject Label** | Een statement: subject label — overgenomen van de gekoppelde subject. |
| **DR-29 Property Label** | Een statement: property label — overgenomen van de gekoppelde property. |
| **DR-30 Object Label** | Een statement: object label — overgenomen van de gekoppelde object thing. |
| **DR-31 Property Datatype** | Een statement: property datatype — overgenomen van de gekoppelde property. |
| **DR-32 Property Has One Current Value** | Een statement: property has one current value is waar wanneer de gekoppelde property heeft een one current value. |
| **DR-33 Property Domain Class** | Een statement: property domain class — overgenomen van de gekoppelde property. |
| **DR-34 Property Range Class** | Een statement: property range class — overgenomen van de gekoppelde property. |
| **DR-35 Literal Quote** | Een statement: literal quote — overgenomen van de gekoppelde graph. |
| **DR-36 Literal Type Marker** | Een statement: literal type marker — overgenomen van de gekoppelde graph. |
| **DR-37 Built in Source Text** | Een statement: built in source text — overgenomen van de gekoppelde graph. |
| **DR-38 User Source Text** | Een statement: user source text — overgenomen van de gekoppelde graph. |
| **DR-39 Next Fact Sort Order** | Een statement: next fact sort order — overgenomen van de gekoppelde graph. |
| **DR-40 Is Literal** | Een statement geldt als literal als de object thing leeg is. |
| **DR-41 Object Text** | Het veld ‘object text’ van het statement wordt bepaald volgens de volgende prioriteit:<br>1. de object value, als de object thing leeg is;<br>2. in alle andere gevallen de object label. |
| **DR-42 Name** | Een statement: name wordt berekend als de subject label, gevolgd door een spatie, gevolgd door de property label, gevolgd door een spatie, gevolgd door de object text. |
| **DR-43 Words** | Een statement: words wordt berekend als de subject label, gevolgd door een spatie, gevolgd door de property label, gevolgd door een spatie, gevolgd door de literal quote, gevolgd door de object value, gevolgd door de literal quote, gevolgd door de literal type marker, gevolgd door de property datatype als de object thing leeg is, in alle andere gevallen de object label. ⚠︎ mechanisch <!-- rulespeak:reword --> |
| **DR-44 Source** | Het veld ‘source’ van het statement wordt bepaald volgens de volgende prioriteit:<br>1. de built in source text, als de vlag ‘built in’ is gezet;<br>2. in alle andere gevallen de user source text. |
| **DR-45 Subject Property Key** | Een statement: subject property key wordt berekend als de subject, gevolgd door ‘|’, gevolgd door de property. |
| **DR-46 Latest Sibling Activation Seq** | Een statement: latest sibling activation seq is de hoogste waarde van activation seq over de statements gekoppeld aan het statement. |
| **DR-47 Is in Effect** | Een statement geldt als in effect als al het volgende geldt: de vlag ‘stated’ is gezet en ten minste één van het volgende geldt: de vlag ‘property has one current value’ niet is gezet of de activation seq de latest sibling activation seq is. |
| **DR-48 Listed Unit** | Het veld ‘listed unit’ van het statement wordt bepaald volgens de volgende prioriteit:<br>1. 1, als de vlag ‘listed’ is gezet;<br>2. in alle andere gevallen 0. |
| **DR-49 In Effect Unit** | Het veld ‘in effect unit’ van het statement wordt bepaald volgens de volgende prioriteit:<br>1. 1, als de vlag ‘in effect’ is gezet;<br>2. in alle andere gevallen 0. |
| **DR-50 Listed Off Unit** | Het veld ‘listed off unit’ van het statement wordt bepaald volgens de volgende prioriteit:<br>1. 1, als al het volgende geldt: de vlag ‘listed’ is gezet en de vlag ‘in effect’ niet is gezet;<br>2. in alle andere gevallen 0. |
| **DR-51 Is Next to Add** | Een statement geldt als een next to add als al het volgende geldt: de listed off unit 1 is en de sort order de next fact sort order is. |
| **DR-52 Count of Inverse Firings** | Een statement: count of inverse firings is het totaal van inverse fired unit over de firings gekoppeld aan het statement. |
| **DR-53 Count of Symmetric Firings** | Een statement: count of symmetric firings is het totaal van symmetric fired unit over de firings gekoppeld aan het statement. |
| **DR-54 Holds Before Chain** | Een statement geldt als voldoend aan: before chain als ten minste één van het volgende geldt: de vlag ‘in effect’ is gezet; de count of inverse firings groter is dan 0; of de count of symmetric firings groter is dan 0. |
| **DR-55 Count of Chain Firings** | Een statement: count of chain firings is het totaal van chain fired unit over de firings gekoppeld aan het statement. |
| **DR-56 Holds Before Typing** | Een statement geldt als voldoend aan: before typing als ten minste één van het volgende geldt: de vlag ‘holds before chain’ is gezet of de count of chain firings groter is dan 0. |
| **DR-57 Count of Type Firings** | Een statement: count of type firings is het totaal van type fired unit over de firings gekoppeld aan het statement. |
| **DR-58 Holds** | Een statement geldt als voldoend aan: als ten minste één van het volgende geldt: de vlag ‘holds before typing’ is gezet of de count of type firings groter is dan 0. |
| **DR-59 Is Conclusion** | Een statement geldt als een conclusion als al het volgende geldt: de vlag ‘holds’ is gezet en de vlag ‘in effect’ niet is gezet. |
| **DR-60 Holds Unit** | Het veld ‘holds unit’ van het statement wordt bepaald volgens de volgende prioriteit:<br>1. 1, als de vlag ‘holds’ is gezet;<br>2. in alle andere gevallen 0. |
| **DR-61 Conclusion Unit** | Het veld ‘conclusion unit’ van het statement wordt bepaald volgens de volgende prioriteit:<br>1. 1, als de vlag ‘conclusion’ is gezet;<br>2. in alle andere gevallen 0. |
| **DR-62 Parent Unit** | Het veld ‘parent unit’ van het statement wordt bepaald volgens de volgende prioriteit:<br>1. 1, als al het volgende geldt: de vlag ‘holds’ is gezet en de property ‘isParentOf’ is;<br>2. in alle andere gevallen 0. |
| **DR-63 Grandparent Unit** | Het veld ‘grandparent unit’ van het statement wordt bepaald volgens de volgende prioriteit:<br>1. 1, als al het volgende geldt: de vlag ‘holds’ is gezet en de property ‘isGrandparentOf’ is;<br>2. in alle andere gevallen 0. |
| **DR-64 Place Unit** | Het veld ‘place unit’ van het statement wordt bepaald volgens de volgende prioriteit:<br>1. 1, als al het volgende geldt: de vlag ‘holds’ is gezet en de property ‘attends’ is;<br>2. in alle andere gevallen 0. |
| **DR-65 First Fired Order Key** | Een statement: first fired order key is de laagste waarde van order key over de firings van het statement die fired unit zijn. |
| **DR-66 Count of Chosen Firings** | Een statement: count of chosen firings is het totaal van chosen unit over de firings gekoppeld aan het statement. |
| **DR-67 Subject Node Key** | Een statement: subject node key is gelijk aan subject. |
| **DR-68 Object Node Key** | Het veld ‘object node key’ van het statement wordt bepaald volgens de volgende prioriteit:<br>1. ‘literal|’, gevolgd door de object value, gevolgd door ‘|’, gevolgd door de property datatype, als de object thing leeg is;<br>2. in alle andere gevallen de object thing. |
| **DR-69 Subject Node Kind** | Een statement: subject node kind — overgenomen van de gekoppelde subject. |
| **DR-70 Object Thing Node Kind** | Een statement: object thing node kind — overgenomen van de gekoppelde object thing. |
| **DR-71 Object Node Kind** | Het veld ‘object node kind’ van het statement wordt bepaald volgens de volgende prioriteit:<br>1. ‘value’, als de object thing leeg is;<br>2. in alle andere gevallen de object thing node kind. |
| **DR-72 Object Value Node Label** | Een statement: object value node label — overgenomen van de gekoppelde property. |
| **DR-73 Count of Violations As First** | Een statement: count of violations as first is het totaal van violation unit over de disjoint checks gekoppeld aan het statement. |
| **DR-74 Count of Violations As Second** | Een statement: count of violations as second is het totaal van violation unit over de disjoint checks gekoppeld aan het statement. |
| **DR-75 Is in Contradiction** | Een statement geldt als in contradiction als ten minste één van het volgende geldt: de count of violations as first groter is dan 0 of de count of violations as second groter is dan 0. |
| **DR-76 Rule Enabled** | Een firing: rule enabled geldt wanneer de gekoppelde rule is enabled. |
| **DR-77 Kind Wording** | Een firing: kind wording — overgenomen van de gekoppelde kind. |
| **DR-78 Kind Sort Order** | Een firing: kind sort order — overgenomen van de gekoppelde kind. |
| **DR-79 Target Name** | Een firing: target name — overgenomen van de gekoppelde target. |
| **DR-80 Target Subject Label** | Een firing: target subject label — overgenomen van de gekoppelde target. |
| **DR-81 Target Property Label** | Een firing: target property label — overgenomen van de gekoppelde target. |
| **DR-82 Target Object Text** | Een firing: target object text — overgenomen van de gekoppelde target. |
| **DR-83 Target is Conclusion** | Een firing: target is conclusion is waar wanneer de gekoppelde target is een conclusion. |
| **DR-84 Target Subject Node Key** | Een firing: target subject node key — overgenomen van de gekoppelde target. |
| **DR-85 Target Object Node Key** | Een firing: target object node key — overgenomen van de gekoppelde target. |
| **DR-86 Target First Fired Order Key** | Een firing: target first fired order key — overgenomen van de gekoppelde target. |
| **DR-87 First Words** | Een firing: first words — overgenomen van de gekoppelde first. |
| **DR-88 Second Words** | Een firing: second words — overgenomen van de gekoppelde second. |
| **DR-89 First is in Effect** | Een firing: first is in effect geldt wanneer de gekoppelde first is in effect. |
| **DR-90 First Holds Before Chain** | Een firing: first holds before chain geldt wanneer de gekoppelde first is holds before chain. |
| **DR-91 Second Holds Before Chain** | Een firing: second holds before chain geldt wanneer de gekoppelde second is holds before chain. |
| **DR-92 First Holds Before Typing** | Een firing: first holds before typing geldt wanneer de gekoppelde first is holds before typing. |
| **DR-93 First Property** | Een firing: first property — overgenomen van de gekoppelde first. |
| **DR-94 First Property Domain Class** | Een firing: first property domain class — overgenomen van de gekoppelde first. |
| **DR-95 First Property Range Class** | Een firing: first property range class — overgenomen van de gekoppelde first. |
| **DR-96 First Sort Order** | Een firing: first sort order — overgenomen van de gekoppelde first. |
| **DR-97 Second Sort Order** | Een firing: second sort order — overgenomen van de gekoppelde second. |
| **DR-98 Inverse Fired Unit** | Het veld ‘inverse fired unit’ van de firing wordt bepaald volgens de volgende prioriteit:<br>1. 1, als al het volgende geldt: de vlag ‘rule enabled’ is gezet; de kind ‘inverse’ is; en de vlag ‘first is in effect’ is gezet;<br>2. in alle andere gevallen 0. |
| **DR-99 Symmetric Fired Unit** | Het veld ‘symmetric fired unit’ van de firing wordt bepaald volgens de volgende prioriteit:<br>1. 1, als al het volgende geldt: de vlag ‘rule enabled’ is gezet; de kind ‘symmetric’ is; en de vlag ‘first is in effect’ is gezet;<br>2. in alle andere gevallen 0. |
| **DR-100 Chain Fired Unit** | Het veld ‘chain fired unit’ van de firing wordt bepaald volgens de volgende prioriteit:<br>1. 1, als al het volgende geldt: de vlag ‘rule enabled’ is gezet; de kind ‘chain’ is; de vlag ‘first holds before chain’ is gezet; en de vlag ‘second holds before chain’ is gezet;<br>2. in alle andere gevallen 0. |
| **DR-101 Type Fired Unit** | Het veld ‘type fired unit’ van de firing wordt bepaald volgens de volgende prioriteit:<br>1. 1, als al het volgende geldt: de vlag ‘rule enabled’ is gezet; ten minste één van het volgende geldt: de kind ‘domain’ is of de kind ‘range’ is; en de vlag ‘first holds before typing’ is gezet;<br>2. in alle andere gevallen 0. |
| **DR-102 Fired Unit** | Een firing: fired unit wordt berekend als de inverse fired unit plus de symmetric fired unit plus de chain fired unit plus de type fired unit. |
| **DR-103 Order Key** | Een firing: order key wordt berekend als de first sort order maal 100000 plus de kind sort order maal 1000 plus 0 als de second leeg is, in alle andere gevallen de second sort order. |
| **DR-104 Is Chosen** | Een firing geldt als een chosen als al het volgende geldt: de fired unit 1 is; de vlag ‘target is conclusion’ is gezet; en de order key de target first fired order key is. |
| **DR-105 Chosen Unit** | Het veld ‘chosen unit’ van de firing wordt bepaald volgens de volgende prioriteit:<br>1. 1, als de vlag ‘chosen’ is gezet;<br>2. in alle andere gevallen 0. |
| **DR-106 Card Text** | Het veld ‘card text’ van de firing wordt bepaald volgens de volgende prioriteit:<br>1. ‘Worked out by the ’, gevolgd door de kind wording, gevolgd door ‘ from “’, gevolgd door de first words, gevolgd door ‘”’, gevolgd door ‘ and “’, gevolgd door de second words, gevolgd door ‘”’ als de kind ‘chain’ is, in alle andere gevallen ‘ and “’, gevolgd door de first property, gevolgd door ‘ has domain ’, gevolgd door de first property domain class, gevolgd door ‘”’ als de kind ‘domain’ is, in alle andere gevallen ‘ and “’, gevolgd door de first property, gevolgd door ‘ has range ’, gevolgd door de first property range class, gevolgd door ‘”’, gevolgd door een punt, als de vlag ‘chosen’ is gezet;<br>2. in alle andere gevallen een lege tekenreeks. ⚠︎ mechanisch <!-- rulespeak:reword --> |
| **DR-107 Name** | Een firing: name is gelijk aan firing ID. |
| **DR-108 Rule Enabled** | Een disjoint check: rule enabled geldt wanneer de gekoppelde rule is enabled. |
| **DR-109 First Holds** | Een disjoint check: first holds geldt wanneer de gekoppelde first is holds. |
| **DR-110 Second Holds** | Een disjoint check: second holds geldt wanneer de gekoppelde second is holds. |
| **DR-111 Subject Label** | Een disjoint check: subject label — overgenomen van de gekoppelde first. |
| **DR-112 Object Label** | Een disjoint check: object label — overgenomen van de gekoppelde first. |
| **DR-113 Violation Unit** | Het veld ‘violation unit’ van de disjoint check wordt bepaald volgens de volgende prioriteit:<br>1. 1, als al het volgende geldt: de vlag ‘rule enabled’ is gezet; de vlag ‘first holds’ is gezet; en de vlag ‘second holds’ is gezet;<br>2. in alle andere gevallen 0. |
| **DR-114 Alarm Text** | Het veld ‘alarm text’ van de disjoint check wordt bepaald volgens de volgende prioriteit:<br>1. ‘Contradiction found. ’, gevolgd door de subject label, gevolgd door ‘ is recorded as both parent and spouse of ’, gevolgd door de object label, gevolgd door ‘. Check those facts and the selected disjoint rule.’, als de violation unit 1 is;<br>2. in alle andere gevallen een lege tekenreeks. |
| **DR-115 Name** | Een disjoint check: name is gelijk aan disjoint check ID. |
| **DR-116 Name** | Een scenario: name is gelijk aan label. |
| **DR-117 Graph Has Facts to Add** | Een scenario: graph has facts to add is waar wanneer de gekoppelde graph heeft een facts to add. |
| **DR-118 Is Available** | Een scenario geldt als available als ten minste één van het volgende geldt: de fact baseline niet ‘next’ is of de vlag ‘graph has facts to add’ is gezet. |
| **DR-119 Name** | Een scenario fact change: name is gelijk aan scenario fact change ID. |
| **DR-120 Name** | Een scenario rule setting: name is gelijk aan scenario rule setting ID. |
| **DR-121 Name** | Een node layout: name is gelijk aan node layout ID. |
| **DR-122 Name** | Een layout slot: name is gelijk aan layout slot ID. |
| **DR-123 Name** | Een page text: name is gelijk aan page text ID. |

## 5 Herleidbaarheid naar het schema

_De expressiekolom is de definitie van de regel in Documentatie-notatie —
dezelfde logica die het regelboek opslaat, geschreven voor een zakelijke lezer._

| Schema-element | Soort | Expressie |
|----------------|------|------------|
| **Graphs.Name** | formule | `Label` |
| **Graphs.CountOfStatedFacts** | rollup | `Sum(Statements.InEffectUnit via Graph)` |
| **Graphs.CountOfConclusions** | rollup | `Sum(Statements.ConclusionUnit via Graph)` |
| **Graphs.CountOfViolations** | rollup | `Sum(DisjointChecks.ViolationUnit via Graph)` |
| **Graphs.IsConsistent** | formule | `CountOfViolations = 0` |
| **Graphs.ConclusionsMessage** | formule | `If(CountOfConclusions = 0, NoConclusionsText, "")` |
| **Graphs.CountOfFactsToAdd** | rollup | `Sum(Statements.ListedOffUnit via Graph)` |
| **Graphs.HasFactsToAdd** | formule | `CountOfFactsToAdd > 0` |
| **Graphs.NextFactSortOrder** | rollup | `Min(Statements.SortOrder via Graph)` |
| **Graphs.MaxActivationSeq** | rollup | `Max(Statements.ActivationSeq via Graph)` |
| **Graphs.NextActivationSeq** | formule | `MaxActivationSeq + 1` |
| **Graphs.MaxListedSortOrder** | rollup | `Max(Statements.SortOrder via Graph)` |
| **Graphs.NextListedSortOrder** | formule | `MaxListedSortOrder + 1` |
| **Rules.Name** | formule | `Label` |
| **Rules.CountOfFirings** | rollup | `Sum(Firings.FiredUnit via Rule)` |
| **FiringKinds.Name** | formule | `Wording` |
| **FiringKinds.RuleEnabled** | opzoekwaarde | `Lookup(Rules.IsEnabled via Rule)` |
| **Things.Name** | formule | `Label` |
| **Things.NodeKey** | formule | `ThingId` |
| **Things.CountOfChildren** | rollup | `Sum(Statements.ParentUnit via Subject)` |
| **Things.CountOfGrandchildren** | rollup | `Sum(Statements.GrandparentUnit via Subject)` |
| **Things.CountAsPlace** | rollup | `Sum(Statements.PlaceUnit via ObjectThing)` |
| **Things.CountAsSubject** | rollup | `Sum(Statements.HoldsUnit via Subject)` |
| **Things.CountAsObject** | rollup | `Sum(Statements.HoldsUnit via ObjectThing)` |
| **Things.IsInGraph** | formule | `Or(CountAsSubject > 0, CountAsObject > 0)` |
| **Things.NodeKind** | formule | `If(Kind = "class", "class", If(CountAsPlace > 0, "place", "person"))` |
| **Properties.Name** | formule | `Label` |
| **Statements.SubjectLabel** | opzoekwaarde | `Lookup(Things.Label via Subject)` |
| **Statements.PropertyLabel** | opzoekwaarde | `Lookup(Properties.Label via Property)` |
| **Statements.ObjectLabel** | opzoekwaarde | `Lookup(Things.Label via ObjectThing)` |
| **Statements.PropertyDatatype** | opzoekwaarde | `Lookup(Properties.Datatype via Property)` |
| **Statements.PropertyHasOneCurrentValue** | opzoekwaarde | `Lookup(Properties.HasOneCurrentValue via Property)` |
| **Statements.PropertyDomainClass** | opzoekwaarde | `Lookup(Properties.DomainClass via Property)` |
| **Statements.PropertyRangeClass** | opzoekwaarde | `Lookup(Properties.RangeClass via Property)` |
| **Statements.LiteralQuote** | opzoekwaarde | `Lookup(Graphs.LiteralQuote via Graph)` |
| **Statements.LiteralTypeMarker** | opzoekwaarde | `Lookup(Graphs.LiteralTypeMarker via Graph)` |
| **Statements.BuiltInSourceText** | opzoekwaarde | `Lookup(Graphs.BuiltInSourceText via Graph)` |
| **Statements.UserSourceText** | opzoekwaarde | `Lookup(Graphs.UserSourceText via Graph)` |
| **Statements.NextFactSortOrder** | opzoekwaarde | `Lookup(Graphs.NextFactSortOrder via Graph)` |
| **Statements.IsLiteral** | formule | `ObjectThing = ""` |
| **Statements.ObjectText** | formule | `If(ObjectThing = "", ObjectValue, ObjectLabel)` |
| **Statements.Name** | formule | `Concat(SubjectLabel, " ", PropertyLabel, " ", ObjectText)` |
| **Statements.Words** | formule | `Concat(SubjectLabel, " ", PropertyLabel, " ", If(ObjectThing = "", Concat(LiteralQuote, ObjectValue, LiteralQuote, LiteralTypeMarker, PropertyDatatype), ObjectLabel))` |
| **Statements.Source** | formule | `If(IsBuiltIn, BuiltInSourceText, UserSourceText)` |
| **Statements.SubjectPropertyKey** | formule | `Concat(Subject, "\|", Property)` |
| **Statements.LatestSiblingActivationSeq** | rollup | `Max(Statements.ActivationSeq via SubjectPropertyKey)` |
| **Statements.IsInEffect** | formule | `And(IsStated, Or(Not(PropertyHasOneCurrentValue), ActivationSeq = LatestSiblingActivationSeq))` |
| **Statements.ListedUnit** | formule | `If(IsListed, 1, 0)` |
| **Statements.InEffectUnit** | formule | `If(IsInEffect, 1, 0)` |
| **Statements.ListedOffUnit** | formule | `If(And(IsListed, Not(IsInEffect)), 1, 0)` |
| **Statements.IsNextToAdd** | formule | `And(ListedOffUnit = 1, SortOrder = NextFactSortOrder)` |
| **Statements.CountOfInverseFirings** | rollup | `Sum(Firings.InverseFiredUnit via Target)` |
| **Statements.CountOfSymmetricFirings** | rollup | `Sum(Firings.SymmetricFiredUnit via Target)` |
| **Statements.HoldsBeforeChain** | formule | `Or(IsInEffect, CountOfInverseFirings > 0, CountOfSymmetricFirings > 0)` |
| **Statements.CountOfChainFirings** | rollup | `Sum(Firings.ChainFiredUnit via Target)` |
| **Statements.HoldsBeforeTyping** | formule | `Or(HoldsBeforeChain, CountOfChainFirings > 0)` |
| **Statements.CountOfTypeFirings** | rollup | `Sum(Firings.TypeFiredUnit via Target)` |
| **Statements.Holds** | formule | `Or(HoldsBeforeTyping, CountOfTypeFirings > 0)` |
| **Statements.IsConclusion** | formule | `And(Holds, Not(IsInEffect))` |
| **Statements.HoldsUnit** | formule | `If(Holds, 1, 0)` |
| **Statements.ConclusionUnit** | formule | `If(IsConclusion, 1, 0)` |
| **Statements.ParentUnit** | formule | `If(And(Holds, Property = "isParentOf"), 1, 0)` |
| **Statements.GrandparentUnit** | formule | `If(And(Holds, Property = "isGrandparentOf"), 1, 0)` |
| **Statements.PlaceUnit** | formule | `If(And(Holds, Property = "attends"), 1, 0)` |
| **Statements.FirstFiredOrderKey** | rollup | `Min(Firings.OrderKey via Target)` |
| **Statements.CountOfChosenFirings** | rollup | `Sum(Firings.ChosenUnit via Target)` |
| **Statements.SubjectNodeKey** | formule | `Subject` |
| **Statements.ObjectNodeKey** | formule | `If(ObjectThing = "", Concat("literal\|", ObjectValue, "\|", PropertyDatatype), ObjectThing)` |
| **Statements.SubjectNodeKind** | opzoekwaarde | `Lookup(Things.NodeKind via Subject)` |
| **Statements.ObjectThingNodeKind** | opzoekwaarde | `Lookup(Things.NodeKind via ObjectThing)` |
| **Statements.ObjectNodeKind** | formule | `If(ObjectThing = "", "value", ObjectThingNodeKind)` |
| **Statements.ObjectValueNodeLabel** | opzoekwaarde | `Lookup(Properties.ValueNodeLabel via Property)` |
| **Statements.CountOfViolationsAsFirst** | rollup | `Sum(DisjointChecks.ViolationUnit via First)` |
| **Statements.CountOfViolationsAsSecond** | rollup | `Sum(DisjointChecks.ViolationUnit via Second)` |
| **Statements.IsInContradiction** | formule | `Or(CountOfViolationsAsFirst > 0, CountOfViolationsAsSecond > 0)` |
| **Firings.RuleEnabled** | opzoekwaarde | `Lookup(Rules.IsEnabled via Rule)` |
| **Firings.KindWording** | opzoekwaarde | `Lookup(FiringKinds.Wording via Kind)` |
| **Firings.KindSortOrder** | opzoekwaarde | `Lookup(FiringKinds.SortOrder via Kind)` |
| **Firings.TargetName** | opzoekwaarde | `Lookup(Statements.Name via Target)` |
| **Firings.TargetSubjectLabel** | opzoekwaarde | `Lookup(Statements.SubjectLabel via Target)` |
| **Firings.TargetPropertyLabel** | opzoekwaarde | `Lookup(Statements.PropertyLabel via Target)` |
| **Firings.TargetObjectText** | opzoekwaarde | `Lookup(Statements.ObjectText via Target)` |
| **Firings.TargetIsConclusion** | opzoekwaarde | `Lookup(Statements.IsConclusion via Target)` |
| **Firings.TargetSubjectNodeKey** | opzoekwaarde | `Lookup(Statements.SubjectNodeKey via Target)` |
| **Firings.TargetObjectNodeKey** | opzoekwaarde | `Lookup(Statements.ObjectNodeKey via Target)` |
| **Firings.TargetFirstFiredOrderKey** | opzoekwaarde | `Lookup(Statements.FirstFiredOrderKey via Target)` |
| **Firings.FirstWords** | opzoekwaarde | `Lookup(Statements.Words via First)` |
| **Firings.SecondWords** | opzoekwaarde | `Lookup(Statements.Words via Second)` |
| **Firings.FirstIsInEffect** | opzoekwaarde | `Lookup(Statements.IsInEffect via First)` |
| **Firings.FirstHoldsBeforeChain** | opzoekwaarde | `Lookup(Statements.HoldsBeforeChain via First)` |
| **Firings.SecondHoldsBeforeChain** | opzoekwaarde | `Lookup(Statements.HoldsBeforeChain via Second)` |
| **Firings.FirstHoldsBeforeTyping** | opzoekwaarde | `Lookup(Statements.HoldsBeforeTyping via First)` |
| **Firings.FirstProperty** | opzoekwaarde | `Lookup(Statements.Property via First)` |
| **Firings.FirstPropertyDomainClass** | opzoekwaarde | `Lookup(Statements.PropertyDomainClass via First)` |
| **Firings.FirstPropertyRangeClass** | opzoekwaarde | `Lookup(Statements.PropertyRangeClass via First)` |
| **Firings.FirstSortOrder** | opzoekwaarde | `Lookup(Statements.SortOrder via First)` |
| **Firings.SecondSortOrder** | opzoekwaarde | `Lookup(Statements.SortOrder via Second)` |
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
| **DisjointChecks.RuleEnabled** | opzoekwaarde | `Lookup(Rules.IsEnabled via Rule)` |
| **DisjointChecks.FirstHolds** | opzoekwaarde | `Lookup(Statements.Holds via First)` |
| **DisjointChecks.SecondHolds** | opzoekwaarde | `Lookup(Statements.Holds via Second)` |
| **DisjointChecks.SubjectLabel** | opzoekwaarde | `Lookup(Statements.SubjectLabel via First)` |
| **DisjointChecks.ObjectLabel** | opzoekwaarde | `Lookup(Statements.ObjectLabel via First)` |
| **DisjointChecks.ViolationUnit** | formule | `If(And(RuleEnabled, FirstHolds, SecondHolds), 1, 0)` |
| **DisjointChecks.AlarmText** | formule | `If(ViolationUnit = 1, Concat("Contradiction found. ", SubjectLabel, " is recorded as both parent and spouse of ", ObjectLabel, ". Check those facts and the selected disjoint rule."), "")` |
| **DisjointChecks.Name** | formule | `DisjointCheckId` |
| **Scenarios.Name** | formule | `Label` |
| **Scenarios.GraphHasFactsToAdd** | opzoekwaarde | `Lookup(Graphs.HasFactsToAdd via Graph)` |
| **Scenarios.IsAvailable** | formule | `Or(FactBaseline <> "next", GraphHasFactsToAdd)` |
| **ScenarioFactChanges.Name** | formule | `ScenarioFactChangeId` |
| **ScenarioRuleSettings.Name** | formule | `ScenarioRuleSettingId` |
| **NodeLayouts.Name** | formule | `NodeLayoutId` |
| **LayoutSlots.Name** | formule | `LayoutSlotId` |
| **PageTexts.Name** | formule | `PageTextId` |

---

> **RuleSpeak®** — de declaratieve notatie voor bedrijfsregels van **Ronald G. Ross**, volgens de conventies van **SBVR** — komt binnenkort. Tot die tijd toont deze pagina de gewone documentatie. [www.RonRoss.info](https://www.RonRoss.info)
