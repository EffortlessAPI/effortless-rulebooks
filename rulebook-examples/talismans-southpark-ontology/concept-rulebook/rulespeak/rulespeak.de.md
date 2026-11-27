# 📘 Talisman's South Park ontology explainer: the concept form — Dokumentation

_This ontology in the concept form: a table of individuals, a table per class, a table or a column per property, and ERBTables and ERBFields for what the structure cannot say. orchestration/ontology_form.py wrote it; injecting it over the ontology brings the ontology's things, properties, axioms and facts in line with these tables._

> Deklarative Geschäftsregeln, aus dem Regelbuch gerendert. Jede Aussage
> unten drückt eine Wahrheit der Geschäftsdomäne aus — sie ist weder eine
> Prozedur noch ein Imperativ. Die Formeln des Regelbuchs sind die einzige
> Quelle der Wahrheit; dieses Dokument ist ihre klarsprachliche Lesart.

## 1 Geschäftsvokabular

| Begriff | Beschreibung | Erläuternder Kommentar |
|------|-------------|-------------------|
| **Thing** | Every individual of the ontology (owl:Thing). A class's table lists the individuals that belong to it. | — |
| Label | Ein definiertes Attribut. | _The name the page shows._ |
| Sort Order | Ein definiertes Attribut. | _Position in the cast._ |
| Portrait | Ein definiertes Attribut. | _The portrait drawn for this thing; set when its name matches a portrait alias._ |

<details open>
<summary>Beispieldaten</summary>

| Label | Sort Order |
|---|---|
| Marvin | 1 |
| Randy | 2 |
| Sharon | 3 |

</details>

| Begriff | Beschreibung | Erläuternder Kommentar |
|------|-------------|-------------------|
| **Person** | The class Person: one row per individual that belongs to it. Its label and place are its ERBTables row. | — |
| Thing | Ein definiertes Attribut. | _The individual's row in Things._ |
| Is Asserted | Wahr, wenn eine leere Zeichenkette. | _True when a fact states it is a Person. False when only a property's domain or range implies it: such a row follows from the facts and is not added by hand._ |
| has Grade Level | Ein definiertes Attribut. | _has grade level: the one current value. A property of the ontology; its other attributes are its ERBFields row._ |
| has Role | Ein definiertes Attribut. | _has role: the one current value. A property of the ontology; its other attributes are its ERBFields row._ |

<details open>
<summary>Beispieldaten</summary>

| Is Asserted | has Grade Level | has Role |
|---|---|---|
| true | 4 | Student |

</details>

| Begriff | Beschreibung | Erläuternder Kommentar |
|------|-------------|-------------------|
| **School** | The class School: one row per individual that belongs to it. Its label and place are its ERBTables row. | — |
| Thing | Ein definiertes Attribut. | _The individual's row in Things._ |
| Is Asserted | Wahr, wenn eine leere Zeichenkette. | _True when a fact states it is a School. False when only a property's domain or range implies it: such a row follows from the facts and is not added by hand._ |

<details open>
<summary>Beispieldaten</summary>

| Is Asserted |
|---|
| false |

</details>

| Begriff | Beschreibung | Erläuternder Kommentar |
|------|-------------|-------------------|
| **is Parent of** | is parent of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | Ein definiertes Attribut. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | Ein definiertes Attribut. | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| Begriff | Beschreibung | Erläuternder Kommentar |
|------|-------------|-------------------|
| **is Child of** | is child of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | Ein definiertes Attribut. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | Ein definiertes Attribut. | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| Begriff | Beschreibung | Erläuternder Kommentar |
|------|-------------|-------------------|
| **is Spouse of** | is spouse of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | Ein definiertes Attribut. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | Ein definiertes Attribut. | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| Begriff | Beschreibung | Erläuternder Kommentar |
|------|-------------|-------------------|
| **is Friend of** | is friend of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | Ein definiertes Attribut. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | Ein definiertes Attribut. | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| Begriff | Beschreibung | Erläuternder Kommentar |
|------|-------------|-------------------|
| **attend** | attends: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | Ein definiertes Attribut. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | Ein definiertes Attribut. | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| Begriff | Beschreibung | Erläuternder Kommentar |
|------|-------------|-------------------|
| **has Age** | has age: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | Ein definiertes Attribut. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Value | Ein definiertes Attribut. | _The value._ |

<details open>
<summary>Beispieldaten</summary>

| Value |
|---|
| 10 |

</details>

| Begriff | Beschreibung | Erläuternder Kommentar |
|------|-------------|-------------------|
| **is Grandparent of** | is grandparent of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | Ein definiertes Attribut. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | Ein definiertes Attribut. | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

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
| **ERB Table** | One row per table of this rulebook (the platform's Schema module), and on it what the table's structure does not say. A class: its label and its place in the cast. A property that is a table: its label, its wording on the page, whether it is symmetric, and its inverse, chain and disjoint axioms. What a table IS is read from its fields: Thing and IsAsserted make a class; Subject with Object or Value make a property, whose domain and range are the tables they link to. | — |
| Table Name | Ein definiertes Attribut. | _The table's name again, as a plain column._ |
| Description | Ein definiertes Attribut. | _What the table holds. Follows the table._ |
| Is Meta Table | Wahr, wenn eine leere Zeichenkette. | _True for a table that serves the rulebook itself (ERBTables, ERBFields) and not the ontology._ |
| Is Tenant Scoped | Wahr, wenn eine leere Zeichenkette. | _The Schema module's: rows belong to one tenant. Not used by the ontology._ |
| Tenant Parent Table | Ein definiertes Attribut. | _The Schema module's: the table tenancy is reached through. Not used by the ontology._ |
| Tenant Parent Field | Ein definiertes Attribut. | _The Schema module's: the field that points at TenantParentTable. Not used by the ontology._ |
| Sort Order | Ein definiertes Attribut. | _Display order. For a class: Position in the cast. For a property: Position in the Add fact form's property list._ |
| Label | Ein definiertes Attribut. | _For a class: The name the page shows. For a property: The property's wording on the page._ |
| Portrait | Ein definiertes Attribut. | _For a class: The portrait drawn for this thing; set when its name matches a portrait alias._ |
| Is User Addable | Wahr, wenn eine leere Zeichenkette. | _For a property: Offered in the page's Add fact form._ |
| Is Symmetric | Wahr, wenn eine leere Zeichenkette. | _For a property: The symmetric rule makes this link true in both directions._ |
| Value Node Label | Ein definiertes Attribut. | _For a property: The small label under a value node for this property: 'role value' or 'number value'._ |
| Current Value Wording | Ein definiertes Attribut. | _For a property: How the Add fact form names a replaced value, as in 'Stan now has grade level 5'._ |
| Input Name | Ein definiertes Attribut. | _For a property: How the integer check names this property's value, as in 'for grade level'._ |
| Record Kind | Ein definiertes Attribut. | _For a property: The word the current-record summary uses for this property's values: role, grade or school._ |
| Record Order | Ein definiertes Attribut. | _For a property: Position of this property in the current-record summary._ |
| Inverse of | Ein definiertes Attribut. | _The property that reads this one backwards (OWL inverseOf): when A has this link to B, the inverse rule concludes that B has that one to A. Names a property: a property's table, or a column that is a property._ |
| Chain First | Ein definiertes Attribut. | _With ChainSecond, the property chain that concludes this property (OWL propertyChainAxiom): a ChainFirst link followed by a ChainSecond link._ |
| Chain Second | Ein definiertes Attribut. | _The chain's second link. Set together with ChainFirst._ |
| Disjoint With | Ein definiertes Attribut. | _Properties that never both hold between the same two things (OWL propertyDisjointWith). Several are separated by ', '._ |

<details open>
<summary>Beispieldaten</summary>

| Table Name | Description | Is Meta Table | Is Tenant Scoped | Tenant Parent Field | Sort Order | Label | Is User Addable | Is Symmetric | Value Node Label | Current Value Wording | Input Name | Record Kind | Record Order | Inverse of | Chain First | Chain Second | Disjoint With |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| Things | Every individual of the ontology (owl:Thing). A class's table lists the individuals that belong to it. | false | — | — | — | — | — | — | — | — | — | — | — | — | — | — | — |
| Person | The class Person: one row per individual that belongs to it. Its label and place are its ERBTables row. | false | — | — | 8 | Person | — | — | — | — | — | — | — | — | — | — | — |
| School | The class School: one row per individual that belongs to it. Its label and place are its ERBTables row. | false | — | — | 9 | School | — | — | — | — | — | — | — | — | — | — | — |

</details>

| Begriff | Beschreibung | Erläuternder Kommentar |
|------|-------------|-------------------|
| **ERB Field** | One row per field of every table (the platform's Schema module), and on it the same attributes for a property that is a column: a property with one current value, on its domain's class table (on Things when it has no domain class). | — |
| ERB Table | Ein definiertes Attribut. | _The table the field is on._ |
| Field Name | Ein definiertes Attribut. | _The field's name. For a column that is a property: the property's id._ |
| Field Type | Ein definiertes Attribut. | _raw or relationship. Follows the field._ |
| Datatype | Ein definiertes Attribut. | _The rulebook datatype. Follows the field._ |
| Is Primary Key | Wahr, wenn eine leere Zeichenkette. | _True for the field that identifies a row of its table._ |
| Description | Ein definiertes Attribut. | _What the field means. Follows the field._ |
| Label | Ein definiertes Attribut. | _For a property: The property's wording on the page._ |
| Is User Addable | Wahr, wenn eine leere Zeichenkette. | _For a property: Offered in the page's Add fact form._ |
| Is Symmetric | Wahr, wenn eine leere Zeichenkette. | _For a property: The symmetric rule makes this link true in both directions._ |
| Sort Order | Ein definiertes Attribut. | _For a property: Position in the Add fact form's property list._ |
| Value Node Label | Ein definiertes Attribut. | _For a property: The small label under a value node for this property: 'role value' or 'number value'._ |
| Current Value Wording | Ein definiertes Attribut. | _For a property: How the Add fact form names a replaced value, as in 'Stan now has grade level 5'._ |
| Input Name | Ein definiertes Attribut. | _For a property: How the integer check names this property's value, as in 'for grade level'._ |
| Record Kind | Ein definiertes Attribut. | _For a property: The word the current-record summary uses for this property's values: role, grade or school._ |
| Record Order | Ein definiertes Attribut. | _For a property: Position of this property in the current-record summary._ |
| Inverse of | Ein definiertes Attribut. | _The property that reads this one backwards (OWL inverseOf): when A has this link to B, the inverse rule concludes that B has that one to A. Names a property: a property's table, or a column that is a property._ |
| Chain First | Ein definiertes Attribut. | _With ChainSecond, the property chain that concludes this property (OWL propertyChainAxiom): a ChainFirst link followed by a ChainSecond link._ |
| Chain Second | Ein definiertes Attribut. | _The chain's second link. Set together with ChainFirst._ |
| Disjoint With | Ein definiertes Attribut. | _Properties that never both hold between the same two things (OWL propertyDisjointWith). Several are separated by ', '._ |

<details open>
<summary>Beispieldaten</summary>

| Field Name | Field Type | Datatype | Is Primary Key | Description | Label | Is User Addable | Is Symmetric | Sort Order | Value Node Label | Current Value Wording | Input Name | Record Kind | Record Order | Inverse of | Chain First | Chain Second | Disjoint With |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| ThingId | raw | string | true | Key of the thing; also its local name in the model namespace. | — | — | — | — | — | — | — | — | — | — | — | — | — |
| Label | raw | string | false | The name the page shows. | — | — | — | — | — | — | — | — | — | — | — | — | — |
| SortOrder | raw | integer | false | Position in the cast. | — | — | — | — | — | — | — | — | — | — | — | — | — |

</details>

## 2 Faktentypen

- ein **Thing** kann auf ein **Portrait** verweisen
- ein **Person** verweist auf genau ein **Thing**
- ein **School** verweist auf genau ein **Thing**
- ein **is Parent of** verweist auf genau ein **Thing**
- ein **is Child of** verweist auf genau ein **Thing**
- ein **is Spouse of** verweist auf genau ein **Thing**
- ein **is Friend of** verweist auf genau ein **Thing**
- ein **attend** verweist auf genau ein **Person**
- ein **attend** verweist auf genau ein **School**
- eine **has Age** verweist auf genau ein **Thing**
- ein **is Grandparent of** verweist auf genau ein **Thing**
- eine **ERB Table** kann auf ein **ERB Table** verweisen
- eine **ERB Table** kann auf ein **Portrait** verweisen
- ein **ERB Field** kann auf ein **ERB Table** verweisen

## 3 Operative Regeln

_Operative Regeln besagen, was das Geschäft **verpflichtet**, **verbietet** oder
empfiehlt (**sollte**). Strukturelle Regeln stammen aus Pflichtfeldern und Fremdschlüsseln;
semantische Regeln stammen aus der Constraints-Tabelle, jede gestützt auf einen booleschen Wert,
den das Regelbuch bereits berechnet (querverwiesen als DR-N in den definitorischen Regeln unten)._

### Strukturelle Einschränkungen (aus dem Schema)

- Ein Thing **muss** ein Label und ein Sort Order haben.
- Ein Person **muss** auf genau einen Thing verweisen.
- Ein Person **muss** erfassen, ob es ist asserted.
- Ein School **muss** auf genau einen Thing verweisen.
- Ein School **muss** erfassen, ob es ist asserted.
- Ein is Parent of **muss** auf genau einen Thing als seinen Subject verweisen.
- Ein is Parent of **muss** auf genau einen Thing als seinen Object verweisen.
- Ein is Child of **muss** auf genau einen Thing als seinen Subject verweisen.
- Ein is Child of **muss** auf genau einen Thing als seinen Object verweisen.
- Ein is Spouse of **muss** auf genau einen Thing als seinen Subject verweisen.
- Ein is Spouse of **muss** auf genau einen Thing als seinen Object verweisen.
- Ein is Friend of **muss** auf genau einen Thing als seinen Subject verweisen.
- Ein is Friend of **muss** auf genau einen Thing als seinen Object verweisen.
- Ein attend **muss** auf genau einen Person als seinen Subject verweisen.
- Ein attend **muss** auf genau einen School als seinen Object verweisen.
- Eine has Age **muss** auf genau einen Thing als seinen Subject verweisen.
- Eine has Age **muss** eine Value haben.
- Ein is Grandparent of **muss** auf genau einen Thing als seinen Subject verweisen.
- Ein is Grandparent of **muss** auf genau einen Thing als seinen Object verweisen.
- Ein Portrait **muss** eine Display Name; eine Page; eine Image; ein Crop; ein Width; ein Height; und ein Sort Order haben.

## 4 Definitorische Regeln

_Alle Aussagen drücken Wahrheiten der Geschäftsdomäne aus; sie sind weder Prozeduren
noch Imperative. Statt „genau dann, wenn“ wird „nur dann, wenn“ verwendet, damit eine
einseitige Notwendigkeit nicht mit einer Äquivalenz verwechselt wird. Ein
**⚠︎ mechanisch**-Hinweis markiert eine Regel, deren deterministische Formulierung treu,
aber hölzern ist — ein Signal für eine optionale Umformulierung, kein Fehler._

| ID | Deklarative Regel |
|----|------------------|
| **DR-1 Name** | Ein Portrait: Name entspricht Display Name. |

## 5 Rückverfolgbarkeit zum Schema

_Die Ausdrucksspalte ist die Definition der Regel in Dokumentation-Notation —
dieselbe Logik, die das Regelbuch speichert, geschrieben für Fachleser._

| Schema-Element | Art | Ausdruck |
|----------------|------|------------|
| **Portraits.Name** | Formel | `DisplayName` |

---

> **RuleSpeak®** — die von **Ronald G. Ross** geschaffene deklarative Geschäftsregel-Notation nach den Konventionen von **SBVR** — kommt bald. Bis dahin zeigt diese Seite die reguläre Dokumentation. [www.RonRoss.info](https://www.RonRoss.info)
