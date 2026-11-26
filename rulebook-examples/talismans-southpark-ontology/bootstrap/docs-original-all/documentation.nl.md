# 📘 South Park ontology explainer — Documentatie

_The barebones South Park ontology from Jessica Talisman's explainer: ten stated facts and five rules that can each be switched on. Conclusions are formulas._

> Declaratieve bedrijfsregels, gegenereerd uit het regelboek. Elke uitspraak
> drukt een waarheid van het bedrijfsdomein uit — het is geen procedure en geen
> imperatief. De formules van het regelboek zijn de enige bron van waarheid;
> dit document is hun lezing in klare taal.

## 1 Bedrijfsvocabulaire

| Term | Beschrijving | Toelichting |
|------|-------------|-------------------|
| **Graph** | The whole example. | — |
| Name | Gelijk aan graph ID. | — |
| Count of Stated Facts | Het totaal van stated unit over de statements gekoppeld aan de graph. | — |
| Count of Conclusions | Het totaal van conclusion unit over de statements gekoppeld aan de graph. | _Facts that hold but were never stated._ |
| Count of Violations | Het totaal van violation unit over de disjoint checks gekoppeld aan de graph. | — |
| Is Consistent | Waar wanneer de count of violations 0 is. | — |

<details open>
<summary>Voorbeeldgegevens</summary>

| Name ƒ | Count of Stated Facts ƒ | Count of Conclusions ƒ | Count of Violations ƒ | Is Consistent ƒ |
|---|---|---|---|---|
| south-park | 10 | 8 | 0 | true |

_ƒ markeert een berekende kolom._

</details>

| Term | Beschrijving | Toelichting |
|------|-------------|-------------------|
| **Rule** | The five rules; IsEnabled is the checkbox. | — |
| Label | Een gedefinieerd attribuut. | — |
| Explanation | Een gedefinieerd attribuut. | — |
| Is Enabled | Waar wanneer een lege tekenreeks. | — |
| Name | Gelijk aan label. | — |
| Count of Firings | Het totaal van fired unit over de firings gekoppeld aan de rule. | _How many times the rule produced a fact._ |

<details open>
<summary>Voorbeeldgegevens</summary>

| Label | Explanation | Is Enabled | Name ƒ | Count of Firings ƒ |
|---|---|---|---|---|
| Inverse | A parent link also gives a child link between the same two people. | true | Inverse | 3 |
| Symmetric | A friend link or a spouse link is true for both people. | true | Symmetric | 2 |
| Property chain | A parent of a parent is a grandparent. | true | Property chain | 2 |

_ƒ markeert een berekende kolom._

</details>

| Term | Beschrijving | Toelichting |
|------|-------------|-------------------|
| **Thing** | A node in the picture: a person, a place, a class or a role value. | — |
| Label | Een gedefinieerd attribuut. | — |
| Kind | Een gedefinieerd attribuut. | — |
| Name | Gelijk aan label. | — |
| Count of Children | Het totaal van parent unit over de statements gekoppeld aan de thing. | _Added: a count, which a reasoner alone does not give._ |
| Count of Grandchildren | Het totaal van grandparent unit over de statements gekoppeld aan de thing. | _Added: counts concluded grandchildren._ |

<details open>
<summary>Voorbeeldgegevens</summary>

| Label | Kind | Name ƒ | Count of Children ƒ | Count of Grandchildren ƒ |
|---|---|---|---|---|
| Marvin | individual | Marvin | 1 | 2 |
| Randy | individual | Randy | 2 | 0 |
| Sharon | individual | Sharon | 0 | 0 |

_ƒ markeert een berekende kolom._

</details>

| Term | Beschrijving | Toelichting |
|------|-------------|-------------------|
| **Property** | A kind of link, with the characteristics the rules use. | — |
| Label | Een gedefinieerd attribuut. | — |
| Inverse of | Een gedefinieerd attribuut. | — |
| Is Symmetric | Waar wanneer een lege tekenreeks. | — |
| Chain First | Een gedefinieerd attribuut. | — |
| Chain Second | Een gedefinieerd attribuut. | — |
| Domain Class | Een gedefinieerd attribuut. | — |
| Range Class | Een gedefinieerd attribuut. | — |
| Disjoint With | Een gedefinieerd attribuut. | — |
| Name | Gelijk aan label. | — |

<details open>
<summary>Voorbeeldgegevens</summary>

| Label | Is Symmetric | Name ƒ |
|---|---|---|
| is parent of | false | is parent of |
| is child of | false | is child of |
| is spouse of | true | is spouse of |

_ƒ markeert een berekende kolom._

</details>

| Term | Beschrijving | Toelichting |
|------|-------------|-------------------|
| **Statement** | A fact that is stated, or could be concluded: Subject, Property, Object. | — |
| Graph | Een gedefinieerd attribuut. | — |
| Subject | Een gedefinieerd attribuut. | — |
| Property | Een gedefinieerd attribuut. | — |
| Object Thing | Een gedefinieerd attribuut. | — |
| Number Value | Een gedefinieerd attribuut. | — |
| Is Stated | Waar wanneer een lege tekenreeks. | _Drawn as a solid line in the explainer._ |
| Subject Label | Overgenomen van de gekoppelde subject. | — |
| Property Label | Overgenomen van de gekoppelde property. | — |
| Object Label | Overgenomen van de gekoppelde object thing. | — |
| Name | Berekend als de subject label, gevolgd door een spatie, gevolgd door de property label, gevolgd door een spatie, gevolgd door de number value als de object thing leeg is, in alle andere gevallen de object label. | — |
| Count of Inverse Firings | Het totaal van inverse fired unit over de firings gekoppeld aan het statement. | — |
| Count of Symmetric Firings | Het totaal van symmetric fired unit over de firings gekoppeld aan het statement. | — |
| Holds Before Chain | Waar wanneer ten minste één van het volgende geldt: de vlag ‘stated’ is gezet; de count of inverse firings groter is dan 0; of de count of symmetric firings groter is dan 0. | — |
| Count of Chain Firings | Het totaal van chain fired unit over de firings gekoppeld aan het statement. | — |
| Holds Before Typing | Waar wanneer ten minste één van het volgende geldt: de vlag ‘holds before chain’ is gezet of de count of chain firings groter is dan 0. | — |
| Count of Type Firings | Het totaal van type fired unit over de firings gekoppeld aan het statement. | — |
| Holds | Waar wanneer ten minste één van het volgende geldt: de vlag ‘holds before typing’ is gezet of de count of type firings groter is dan 0. | _True when stated or concluded by an enabled rule._ |
| Is Conclusion | Waar wanneer al het volgende geldt: de vlag ‘holds’ is gezet en de vlag ‘stated’ niet is gezet. | _Drawn as a dashed line in the explainer._ |
| Worked Out by | Bepaald op prioriteit: een lege tekenreeks als de vlag ‘conclusion’ niet is gezet; ‘inverse’ als de count of inverse firings groter is dan 0; ‘symmetric’ als de count of symmetric firings groter is dan 0; ‘property chain’ als de count of chain firings groter is dan 0; in alle andere gevallen ‘domain and range’. | — |
| Stated Unit | Bepaald op prioriteit: 1 als de vlag ‘stated’ is gezet; in alle andere gevallen 0. | — |
| Conclusion Unit | Bepaald op prioriteit: 1 als de vlag ‘conclusion’ is gezet; in alle andere gevallen 0. | — |
| Parent Unit | Bepaald op prioriteit: 1 als al het volgende geldt: de vlag ‘holds’ is gezet en de property ‘isParentOf’ is; in alle andere gevallen 0. | — |
| Grandparent Unit | Bepaald op prioriteit: 1 als al het volgende geldt: de vlag ‘holds’ is gezet en de property ‘isGrandparentOf’ is; in alle andere gevallen 0. | — |

<details open>
<summary>Voorbeeldgegevens</summary>

| Number Value | Is Stated | Subject Label ƒ | Property Label ƒ | Object Label ƒ | Name ƒ | Count of Inverse Firings ƒ | Count of Symmetric Firings ƒ | Holds Before Chain ƒ | Count of Chain Firings ƒ | Holds Before Typing ƒ | Count of Type Firings ƒ | Holds ƒ | Is Conclusion ƒ | Worked Out by ƒ | Stated Unit ƒ | Conclusion Unit ƒ | Parent Unit ƒ | Grandparent Unit ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| — | true | Marvin | is parent of | Randy | Marvin is parent of Randy | 0 | 0 | true | 0 | true | 0 | true | false | — | 1 | 0 | 1 | 0 |
| — | false | Marvin | is parent of | Sharon | Marvin is parent of Sharon | 0 | 0 | false | 0 | false | 0 | false | false | — | 0 | 0 | 0 | 0 |
| — | false | Marvin | is parent of | Shelly | Marvin is parent of Shelly | 0 | 0 | false | 0 | false | 0 | false | false | — | 0 | 0 | 0 | 0 |

_ƒ markeert een berekende kolom._

</details>

| Term | Beschrijving | Toelichting |
|------|-------------|-------------------|
| **Firing** | One way one rule could produce one fact from one or two others. | — |
| Rule | Een gedefinieerd attribuut. | — |
| Kind | Een gedefinieerd attribuut. | — |
| Target | Een gedefinieerd attribuut. | — |
| First | Een gedefinieerd attribuut. | — |
| Second | Een gedefinieerd attribuut. | _Same as First when the rule needs one premise._ |
| Rule Enabled | Waar wanneer de gekoppelde rule is enabled. | — |
| Rule Label | Overgenomen van de gekoppelde rule. | — |
| Target Name | Overgenomen van de gekoppelde target. | — |
| First Name | Overgenomen van de gekoppelde first. | — |
| Second Name | Overgenomen van de gekoppelde second. | — |
| First is Stated | Waar wanneer de gekoppelde first is stated. | — |
| First Holds Before Chain | Waar wanneer de gekoppelde first is holds before chain. | — |
| Second Holds Before Chain | Waar wanneer de gekoppelde second is holds before chain. | — |
| First Holds Before Typing | Waar wanneer de gekoppelde first is holds before typing. | — |
| Inverse Fired Unit | Bepaald op prioriteit: 1 als al het volgende geldt: de vlag ‘rule enabled’ is gezet; de kind ‘inverse’ is; en de vlag ‘first is stated’ is gezet; in alle andere gevallen 0. | — |
| Symmetric Fired Unit | Bepaald op prioriteit: 1 als al het volgende geldt: de vlag ‘rule enabled’ is gezet; de kind ‘symmetric’ is; en de vlag ‘first is stated’ is gezet; in alle andere gevallen 0. | — |
| Chain Fired Unit | Bepaald op prioriteit: 1 als al het volgende geldt: de vlag ‘rule enabled’ is gezet; de kind ‘chain’ is; de vlag ‘first holds before chain’ is gezet; en de vlag ‘second holds before chain’ is gezet; in alle andere gevallen 0. | — |
| Type Fired Unit | Bepaald op prioriteit: 1 als al het volgende geldt: de vlag ‘rule enabled’ is gezet; ten minste één van het volgende geldt: de kind ‘domain’ is of de kind ‘range’ is; en de vlag ‘first holds before typing’ is gezet; in alle andere gevallen 0. | — |
| Fired Unit | Berekend als de inverse fired unit plus de symmetric fired unit plus de chain fired unit plus de type fired unit. | — |
| Name | Bepaald op prioriteit: de target name, gevolgd door ‘: worked out by the ’, gevolgd door de rule label, gevolgd door ‘ rule from '’, gevolgd door de first name, gevolgd door ‘'’, gevolgd door ‘ and '’, gevolgd door de second name, gevolgd door ‘'’ als de fired unit groter is dan 0; in alle andere gevallen de firing ID. | _The explainer's conclusion card._ |

<details open>
<summary>Voorbeeldgegevens</summary>

| Kind | Rule Enabled ƒ | Rule Label ƒ | Target Name ƒ | First Name ƒ | Second Name ƒ | First is Stated ƒ | First Holds Before Chain ƒ | Second Holds Before Chain ƒ | First Holds Before Typing ƒ | Inverse Fired Unit ƒ | Symmetric Fired Unit ƒ | Chain Fired Unit ƒ | Type Fired Unit ƒ | Fired Unit ƒ | Name ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| inverse | true | Inverse | Randy is child of Marvin | Marvin is parent of Randy | Marvin is parent of Randy | true | true | true | true | 1 | 0 | 0 | 0 | 1 | Randy is child of Marvin: worked out by the Inverse rule from 'Marvin is parent of Randy' |
| inverse | true | Inverse | Sharon is child of Marvin | Marvin is parent of Sharon | Marvin is parent of Sharon | false | false | false | false | 0 | 0 | 0 | 0 | 0 | inverse:Marvin\|isParentOf\|Sharon>Sharon\|isChildOf\|Marvin |
| inverse | true | Inverse | Shelly is child of Marvin | Marvin is parent of Shelly | Marvin is parent of Shelly | false | false | false | false | 0 | 0 | 0 | 0 | 0 | inverse:Marvin\|isParentOf\|Shelly>Shelly\|isChildOf\|Marvin |

_ƒ markeert een berekende kolom._

</details>

| Term | Beschrijving | Toelichting |
|------|-------------|-------------------|
| **Disjoint Check** | Two statements that must not both hold. | — |
| Graph | Een gedefinieerd attribuut. | — |
| Rule | Een gedefinieerd attribuut. | — |
| A | Een gedefinieerd attribuut. | — |
| B | Een gedefinieerd attribuut. | — |
| Name | Gelijk aan disjoint check ID. | — |
| Rule Enabled | Waar wanneer de gekoppelde rule is enabled. | — |
| A Holds | Waar wanneer de gekoppelde a is holds. | — |
| B Holds | Waar wanneer de gekoppelde b is holds. | — |
| Violation Unit | Bepaald op prioriteit: 1 als al het volgende geldt: de vlag ‘rule enabled’ is gezet; de vlag ‘a holds’ is gezet; en de vlag ‘b holds’ is gezet; in alle andere gevallen 0. | — |

<details open>
<summary>Voorbeeldgegevens</summary>

| Name ƒ | Rule Enabled ƒ | A Holds ƒ | B Holds ƒ | Violation Unit ƒ |
|---|---|---|---|---|
| Marvin\|isParentOf\|Randy~isSpouseOf | true | true | false | 0 |
| Marvin\|isParentOf\|Sharon~isSpouseOf | true | false | false | 0 |
| Marvin\|isParentOf\|Shelly~isSpouseOf | true | false | false | 0 |

_ƒ markeert een berekende kolom._

</details>

## 2 Feittypen

- een **property** kan naar één **property** verwijzen
- een **property** kan naar één **thing** verwijzen
- een **statement** verwijst naar precies één **graph**
- een **statement** verwijst naar precies één **thing**
- een **statement** verwijst naar precies één **property**
- een **firing** verwijst naar precies één **rule**
- een **firing** verwijst naar precies één **statement**
- een **disjoint check** verwijst naar precies één **graph**
- een **disjoint check** verwijst naar precies één **rule**
- een **disjoint check** verwijst naar precies één **statement**

## 3 Operationele regels

_Operationele regels benoemen wat het bedrijf **verplicht**, **verbiedt** of
aanraadt (**zou moeten**). Structurele regels komen uit verplichte velden en
vreemde sleutels; semantische regels uit de Constraints-tabel, elk steunend op een
boolean die het regelboek al berekent (verwezen als DR-N in de definitieregels)._

### Structurele beperkingen (uit het schema)

- Een rule **moet** een label en een explanation hebben, en vastleggen of het is enabled.
- Een thing **moet** een label en een kind hebben.
- Een property **moet** een label hebben, en vastleggen of het is symmetric.
- Een statement **moet** naar precies één graph verwijzen.
- Een statement **moet** naar precies één thing verwijzen als zijn subject.
- Een statement **moet** naar precies één property verwijzen.
- Een statement **moet** vastleggen of het is stated.
- Een firing **moet** naar precies één rule verwijzen.
- Een firing **moet** naar precies één statement verwijzen als zijn target.
- Een firing **moet** naar precies één statement verwijzen als zijn first.
- Een firing **moet** naar precies één statement verwijzen als zijn second.
- Een firing **moet** een kind hebben.
- Een disjoint check **moet** naar precies één graph verwijzen.
- Een disjoint check **moet** naar precies één rule verwijzen.
- Een disjoint check **moet** naar precies één statement verwijzen als zijn a.
- Een disjoint check **moet** naar precies één statement verwijzen als zijn b.

## 4 Definitieregels

_Alle uitspraken drukken waarheden van het bedrijfsdomein uit; het zijn geen
procedures en geen imperatieven. ‘dan en slechts dan’ wordt vermeden ten gunste van
‘alleen als’, zodat een eenzijdige noodzaak niet wordt aangezien voor een equivalentie.
Een **⚠︎ mechanisch**-markering duidt een regel aan waarvan de deterministische
formulering trouw maar houterig is — een signaal voor optionele herformulering, geen fout._

| ID | Declaratieve regel |
|----|------------------|
| **DR-1 Name** | Een graph: name is gelijk aan graph ID. |
| **DR-2 Count of Stated Facts** | Een graph: count of stated facts is het totaal van stated unit over de statements gekoppeld aan de graph. |
| **DR-3 Count of Conclusions** | Een graph: count of conclusions is het totaal van conclusion unit over de statements gekoppeld aan de graph. |
| **DR-4 Count of Violations** | Een graph: count of violations is het totaal van violation unit over de disjoint checks gekoppeld aan de graph. |
| **DR-5 Is Consistent** | Een graph geldt als consistent als de count of violations 0 is. |
| **DR-6 Name** | Een rule: name is gelijk aan label. |
| **DR-7 Count of Firings** | Een rule: count of firings is het totaal van fired unit over de firings gekoppeld aan de rule. |
| **DR-8 Name** | Een thing: name is gelijk aan label. |
| **DR-9 Count of Children** | Een thing: count of children is het totaal van parent unit over de statements gekoppeld aan de thing. |
| **DR-10 Count of Grandchildren** | Een thing: count of grandchildren is het totaal van grandparent unit over de statements gekoppeld aan de thing. |
| **DR-11 Name** | Een property: name is gelijk aan label. |
| **DR-12 Subject Label** | Een statement: subject label — overgenomen van de gekoppelde subject. |
| **DR-13 Property Label** | Een statement: property label — overgenomen van de gekoppelde property. |
| **DR-14 Object Label** | Een statement: object label — overgenomen van de gekoppelde object thing. |
| **DR-15 Name** | Een statement: name wordt berekend als de subject label, gevolgd door een spatie, gevolgd door de property label, gevolgd door een spatie, gevolgd door de number value als de object thing leeg is, in alle andere gevallen de object label. |
| **DR-16 Count of Inverse Firings** | Een statement: count of inverse firings is het totaal van inverse fired unit over de firings gekoppeld aan het statement. |
| **DR-17 Count of Symmetric Firings** | Een statement: count of symmetric firings is het totaal van symmetric fired unit over de firings gekoppeld aan het statement. |
| **DR-18 Holds Before Chain** | Een statement geldt als voldoend aan: before chain als ten minste één van het volgende geldt: de vlag ‘stated’ is gezet; de count of inverse firings groter is dan 0; of de count of symmetric firings groter is dan 0. |
| **DR-19 Count of Chain Firings** | Een statement: count of chain firings is het totaal van chain fired unit over de firings gekoppeld aan het statement. |
| **DR-20 Holds Before Typing** | Een statement geldt als voldoend aan: before typing als ten minste één van het volgende geldt: de vlag ‘holds before chain’ is gezet of de count of chain firings groter is dan 0. |
| **DR-21 Count of Type Firings** | Een statement: count of type firings is het totaal van type fired unit over de firings gekoppeld aan het statement. |
| **DR-22 Holds** | Een statement geldt als voldoend aan: als ten minste één van het volgende geldt: de vlag ‘holds before typing’ is gezet of de count of type firings groter is dan 0. |
| **DR-23 Is Conclusion** | Een statement geldt als een conclusion als al het volgende geldt: de vlag ‘holds’ is gezet en de vlag ‘stated’ niet is gezet. |
| **DR-24 Worked Out by** | Het veld ‘worked out by’ van het statement wordt bepaald volgens de volgende prioriteit:<br>1. een lege tekenreeks, als de vlag ‘conclusion’ niet is gezet;<br>2. ‘inverse’, als de count of inverse firings groter is dan 0;<br>3. ‘symmetric’, als de count of symmetric firings groter is dan 0;<br>4. ‘property chain’, als de count of chain firings groter is dan 0;<br>5. in alle andere gevallen ‘domain and range’. |
| **DR-25 Stated Unit** | Het veld ‘stated unit’ van het statement wordt bepaald volgens de volgende prioriteit:<br>1. 1, als de vlag ‘stated’ is gezet;<br>2. in alle andere gevallen 0. |
| **DR-26 Conclusion Unit** | Het veld ‘conclusion unit’ van het statement wordt bepaald volgens de volgende prioriteit:<br>1. 1, als de vlag ‘conclusion’ is gezet;<br>2. in alle andere gevallen 0. |
| **DR-27 Parent Unit** | Het veld ‘parent unit’ van het statement wordt bepaald volgens de volgende prioriteit:<br>1. 1, als al het volgende geldt: de vlag ‘holds’ is gezet en de property ‘isParentOf’ is;<br>2. in alle andere gevallen 0. |
| **DR-28 Grandparent Unit** | Het veld ‘grandparent unit’ van het statement wordt bepaald volgens de volgende prioriteit:<br>1. 1, als al het volgende geldt: de vlag ‘holds’ is gezet en de property ‘isGrandparentOf’ is;<br>2. in alle andere gevallen 0. |
| **DR-29 Rule Enabled** | Een firing: rule enabled geldt wanneer de gekoppelde rule is enabled. |
| **DR-30 Rule Label** | Een firing: rule label — overgenomen van de gekoppelde rule. |
| **DR-31 Target Name** | Een firing: target name — overgenomen van de gekoppelde target. |
| **DR-32 First Name** | Een firing: first name — overgenomen van de gekoppelde first. |
| **DR-33 Second Name** | Een firing: second name — overgenomen van de gekoppelde second. |
| **DR-34 First is Stated** | Een firing: first is stated geldt wanneer de gekoppelde first is stated. |
| **DR-35 First Holds Before Chain** | Een firing: first holds before chain geldt wanneer de gekoppelde first is holds before chain. |
| **DR-36 Second Holds Before Chain** | Een firing: second holds before chain geldt wanneer de gekoppelde second is holds before chain. |
| **DR-37 First Holds Before Typing** | Een firing: first holds before typing geldt wanneer de gekoppelde first is holds before typing. |
| **DR-38 Inverse Fired Unit** | Het veld ‘inverse fired unit’ van de firing wordt bepaald volgens de volgende prioriteit:<br>1. 1, als al het volgende geldt: de vlag ‘rule enabled’ is gezet; de kind ‘inverse’ is; en de vlag ‘first is stated’ is gezet;<br>2. in alle andere gevallen 0. |
| **DR-39 Symmetric Fired Unit** | Het veld ‘symmetric fired unit’ van de firing wordt bepaald volgens de volgende prioriteit:<br>1. 1, als al het volgende geldt: de vlag ‘rule enabled’ is gezet; de kind ‘symmetric’ is; en de vlag ‘first is stated’ is gezet;<br>2. in alle andere gevallen 0. |
| **DR-40 Chain Fired Unit** | Het veld ‘chain fired unit’ van de firing wordt bepaald volgens de volgende prioriteit:<br>1. 1, als al het volgende geldt: de vlag ‘rule enabled’ is gezet; de kind ‘chain’ is; de vlag ‘first holds before chain’ is gezet; en de vlag ‘second holds before chain’ is gezet;<br>2. in alle andere gevallen 0. |
| **DR-41 Type Fired Unit** | Het veld ‘type fired unit’ van de firing wordt bepaald volgens de volgende prioriteit:<br>1. 1, als al het volgende geldt: de vlag ‘rule enabled’ is gezet; ten minste één van het volgende geldt: de kind ‘domain’ is of de kind ‘range’ is; en de vlag ‘first holds before typing’ is gezet;<br>2. in alle andere gevallen 0. |
| **DR-42 Fired Unit** | Een firing: fired unit wordt berekend als de inverse fired unit plus de symmetric fired unit plus de chain fired unit plus de type fired unit. |
| **DR-43 Name** | Het veld ‘name’ van de firing wordt bepaald volgens de volgende prioriteit:<br>1. de target name, gevolgd door ‘: worked out by the ’, gevolgd door de rule label, gevolgd door ‘ rule from '’, gevolgd door de first name, gevolgd door ‘'’, gevolgd door ‘ and '’, gevolgd door de second name, gevolgd door ‘'’, als de fired unit groter is dan 0;<br>2. in alle andere gevallen de firing ID. |
| **DR-44 Name** | Een disjoint check: name is gelijk aan disjoint check ID. |
| **DR-45 Rule Enabled** | Een disjoint check: rule enabled geldt wanneer de gekoppelde rule is enabled. |
| **DR-46 A Holds** | Een disjoint check: a holds geldt wanneer de gekoppelde a is holds. |
| **DR-47 B Holds** | Een disjoint check: b holds geldt wanneer de gekoppelde b is holds. |
| **DR-48 Violation Unit** | Het veld ‘violation unit’ van de disjoint check wordt bepaald volgens de volgende prioriteit:<br>1. 1, als al het volgende geldt: de vlag ‘rule enabled’ is gezet; de vlag ‘a holds’ is gezet; en de vlag ‘b holds’ is gezet;<br>2. in alle andere gevallen 0. |

## 5 Herleidbaarheid naar het schema

_De expressiekolom is de definitie van de regel in Documentatie-notatie —
dezelfde logica die het regelboek opslaat, geschreven voor een zakelijke lezer._

| Schema-element | Soort | Expressie |
|----------------|------|------------|
| **Graphs.Name** | formule | `GraphId` |
| **Graphs.CountOfStatedFacts** | rollup | `Sum(Statements.StatedUnit via Graph)` |
| **Graphs.CountOfConclusions** | rollup | `Sum(Statements.ConclusionUnit via Graph)` |
| **Graphs.CountOfViolations** | rollup | `Sum(DisjointChecks.ViolationUnit via Graph)` |
| **Graphs.IsConsistent** | formule | `CountOfViolations = 0` |
| **Rules.Name** | formule | `Label` |
| **Rules.CountOfFirings** | rollup | `Sum(Firings.FiredUnit via Rule)` |
| **Things.Name** | formule | `Label` |
| **Things.CountOfChildren** | rollup | `Sum(Statements.ParentUnit via Subject)` |
| **Things.CountOfGrandchildren** | rollup | `Sum(Statements.GrandparentUnit via Subject)` |
| **Properties.Name** | formule | `Label` |
| **Statements.SubjectLabel** | opzoekwaarde | `Lookup(Things.Label via Subject)` |
| **Statements.PropertyLabel** | opzoekwaarde | `Lookup(Properties.Label via Property)` |
| **Statements.ObjectLabel** | opzoekwaarde | `Lookup(Things.Label via ObjectThing)` |
| **Statements.Name** | formule | `SubjectLabel & " " & PropertyLabel & " " & If(ObjectThing = Blank(), NumberValue, ObjectLabel)` |
| **Statements.CountOfInverseFirings** | rollup | `Sum(Firings.InverseFiredUnit via Target)` |
| **Statements.CountOfSymmetricFirings** | rollup | `Sum(Firings.SymmetricFiredUnit via Target)` |
| **Statements.HoldsBeforeChain** | formule | `Or(IsStated, CountOfInverseFirings > 0, CountOfSymmetricFirings > 0)` |
| **Statements.CountOfChainFirings** | rollup | `Sum(Firings.ChainFiredUnit via Target)` |
| **Statements.HoldsBeforeTyping** | formule | `Or(HoldsBeforeChain, CountOfChainFirings > 0)` |
| **Statements.CountOfTypeFirings** | rollup | `Sum(Firings.TypeFiredUnit via Target)` |
| **Statements.Holds** | formule | `Or(HoldsBeforeTyping, CountOfTypeFirings > 0)` |
| **Statements.IsConclusion** | formule | `And(Holds, Not(IsStated))` |
| **Statements.WorkedOutBy** | formule | `If(Not(IsConclusion), "", If(CountOfInverseFirings > 0, "inverse", If(CountOfSymmetricFirings > 0, "symmetric", If(CountOfChainFirings > 0, "property chain", "domain and range"))))` |
| **Statements.StatedUnit** | formule | `If(IsStated, 1, 0)` |
| **Statements.ConclusionUnit** | formule | `If(IsConclusion, 1, 0)` |
| **Statements.ParentUnit** | formule | `If(And(Holds, Property = "isParentOf"), 1, 0)` |
| **Statements.GrandparentUnit** | formule | `If(And(Holds, Property = "isGrandparentOf"), 1, 0)` |
| **Firings.RuleEnabled** | opzoekwaarde | `Lookup(Rules.IsEnabled via Rule)` |
| **Firings.RuleLabel** | opzoekwaarde | `Lookup(Rules.Label via Rule)` |
| **Firings.TargetName** | opzoekwaarde | `Lookup(Statements.Name via Target)` |
| **Firings.FirstName** | opzoekwaarde | `Lookup(Statements.Name via First)` |
| **Firings.SecondName** | opzoekwaarde | `Lookup(Statements.Name via Second)` |
| **Firings.FirstIsStated** | opzoekwaarde | `Lookup(Statements.IsStated via First)` |
| **Firings.FirstHoldsBeforeChain** | opzoekwaarde | `Lookup(Statements.HoldsBeforeChain via First)` |
| **Firings.SecondHoldsBeforeChain** | opzoekwaarde | `Lookup(Statements.HoldsBeforeChain via Second)` |
| **Firings.FirstHoldsBeforeTyping** | opzoekwaarde | `Lookup(Statements.HoldsBeforeTyping via First)` |
| **Firings.InverseFiredUnit** | formule | `If(And(RuleEnabled, Kind = "inverse", FirstIsStated), 1, 0)` |
| **Firings.SymmetricFiredUnit** | formule | `If(And(RuleEnabled, Kind = "symmetric", FirstIsStated), 1, 0)` |
| **Firings.ChainFiredUnit** | formule | `If(And(RuleEnabled, Kind = "chain", FirstHoldsBeforeChain, SecondHoldsBeforeChain), 1, 0)` |
| **Firings.TypeFiredUnit** | formule | `If(And(RuleEnabled, Or(Kind = "domain", Kind = "range"), FirstHoldsBeforeTyping), 1, 0)` |
| **Firings.FiredUnit** | formule | `InverseFiredUnit + SymmetricFiredUnit + ChainFiredUnit + TypeFiredUnit` |
| **Firings.Name** | formule | `If(FiredUnit > 0, TargetName & ": worked out by the " & RuleLabel & " rule from '" & FirstName & "'" & If(Kind = "chain", " and '" & SecondName & "'", ""), FiringId)` |
| **DisjointChecks.Name** | formule | `DisjointCheckId` |
| **DisjointChecks.RuleEnabled** | opzoekwaarde | `Lookup(Rules.IsEnabled via Rule)` |
| **DisjointChecks.AHolds** | opzoekwaarde | `Lookup(Statements.Holds via A)` |
| **DisjointChecks.BHolds** | opzoekwaarde | `Lookup(Statements.Holds via B)` |
| **DisjointChecks.ViolationUnit** | formule | `If(And(RuleEnabled, AHolds, BHolds), 1, 0)` |
