# 📘 Talisman's South Park ontology explainer: the concept form — Documentatie

_This ontology in the concept form: a table of individuals, a table per class, a table or a column per property, and ERBTables and ERBFields for what the structure cannot say. orchestration/ontology_form.py wrote it; injecting it over the ontology brings the ontology's things, properties, axioms and facts in line with these tables._

> Declaratieve bedrijfsregels, gegenereerd uit het regelboek. Elke uitspraak
> drukt een waarheid van het bedrijfsdomein uit — het is geen procedure en geen
> imperatief. De formules van het regelboek zijn de enige bron van waarheid;
> dit document is hun lezing in klare taal.

## 1 Bedrijfsvocabulaire

| Term | Beschrijving | Toelichting |
|------|-------------|-------------------|
| **Thing** | Every individual of the ontology (owl:Thing). A class's table lists the individuals that belong to it. | — |
| Label | Een gedefinieerd attribuut. | _The name the page shows._ |
| Sort Order | Een gedefinieerd attribuut. | _Position in the cast._ |
| Portrait | Een gedefinieerd attribuut. | _The portrait drawn for this thing; set when its name matches a portrait alias._ |

<details open>
<summary>Voorbeeldgegevens</summary>

| Label | Sort Order |
|---|---|
| Marvin | 1 |
| Randy | 2 |
| Sharon | 3 |

</details>

| Term | Beschrijving | Toelichting |
|------|-------------|-------------------|
| **Person** | The class Person: one row per individual that belongs to it. Its label and place are its ERBTables row. | — |
| Thing | Een gedefinieerd attribuut. | _The individual's row in Things._ |
| Is Asserted | Waar wanneer een lege tekenreeks. | _True when a fact states it is a Person. False when only a property's domain or range implies it: such a row follows from the facts and is not added by hand._ |
| has Grade Level | Een gedefinieerd attribuut. | _has grade level: the one current value. A property of the ontology; its other attributes are its ERBFields row._ |
| has Role | Een gedefinieerd attribuut. | _has role: the one current value. A property of the ontology; its other attributes are its ERBFields row._ |

<details open>
<summary>Voorbeeldgegevens</summary>

| Is Asserted | has Grade Level | has Role |
|---|---|---|
| true | 4 | Student |

</details>

| Term | Beschrijving | Toelichting |
|------|-------------|-------------------|
| **School** | The class School: one row per individual that belongs to it. Its label and place are its ERBTables row. | — |
| Thing | Een gedefinieerd attribuut. | _The individual's row in Things._ |
| Is Asserted | Waar wanneer een lege tekenreeks. | _True when a fact states it is a School. False when only a property's domain or range implies it: such a row follows from the facts and is not added by hand._ |

<details open>
<summary>Voorbeeldgegevens</summary>

| Is Asserted |
|---|
| false |

</details>

| Term | Beschrijving | Toelichting |
|------|-------------|-------------------|
| **is Parent of** | is parent of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | Een gedefinieerd attribuut. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | Een gedefinieerd attribuut. | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| Term | Beschrijving | Toelichting |
|------|-------------|-------------------|
| **is Child of** | is child of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | Een gedefinieerd attribuut. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | Een gedefinieerd attribuut. | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| Term | Beschrijving | Toelichting |
|------|-------------|-------------------|
| **is Spouse of** | is spouse of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | Een gedefinieerd attribuut. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | Een gedefinieerd attribuut. | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| Term | Beschrijving | Toelichting |
|------|-------------|-------------------|
| **is Friend of** | is friend of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | Een gedefinieerd attribuut. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | Een gedefinieerd attribuut. | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| Term | Beschrijving | Toelichting |
|------|-------------|-------------------|
| **attend** | attends: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | Een gedefinieerd attribuut. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | Een gedefinieerd attribuut. | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| Term | Beschrijving | Toelichting |
|------|-------------|-------------------|
| **has Age** | has age: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | Een gedefinieerd attribuut. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Value | Een gedefinieerd attribuut. | _The value._ |

<details open>
<summary>Voorbeeldgegevens</summary>

| Value |
|---|
| 10 |

</details>

| Term | Beschrijving | Toelichting |
|------|-------------|-------------------|
| **is Grandparent of** | is grandparent of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | Een gedefinieerd attribuut. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | Een gedefinieerd attribuut. | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| Term | Beschrijving | Toelichting |
|------|-------------|-------------------|
| **Portrait** | The page's Fandom portraits, including those for characters a reader may add. | — |
| Display Name | Een gedefinieerd attribuut. | _The name on the portrait's Fandom page._ |
| Page | Een gedefinieerd attribuut. | _The Fandom page credited for the portrait._ |
| Image | Een gedefinieerd attribuut. | _The original Fandom image._ |
| Crop | Een gedefinieerd attribuut. | _The crop as an SVG viewBox: x y width height._ |
| Width | Een gedefinieerd attribuut. | _The original image's width in pixels._ |
| Height | Een gedefinieerd attribuut. | _The original image's height in pixels._ |
| Sort Order | Een gedefinieerd attribuut. | _Position in the credits._ |
| Name | Gelijk aan display name. | _Order 1. The portrait's name._ |

<details open>
<summary>Voorbeeldgegevens</summary>

| Display Name | Page | Image | Crop | Width | Height | Sort Order | Name ƒ |
|---|---|---|---|---|---|---|---|
| Marvin Marsh | https://southpark.fandom.com/wiki/Marvin_Marsh | https://static.wikia.nocookie.net/southpark/images/d/db/Marsh-family-grandpa-marsh-w-purple-blanket.png/revision/latest | 48 -5 280 280 | 375 | 536 | 1 | Marvin Marsh |
| Randy Marsh | https://southpark.fandom.com/wiki/Randy_Marsh | https://static.wikia.nocookie.net/southparkfanon/images/d/d6/Randy_Marsh.png/revision/latest | 0 -10 428 428 | 428 | 1057 | 2 | Randy Marsh |
| Sharon Marsh | https://southpark.fandom.com/wiki/Sharon_Marsh | https://static.wikia.nocookie.net/southpark/images/9/99/Sharon_Marsh.png/revision/latest | 20 -10 390 390 | 439 | 1049 | 3 | Sharon Marsh |

_ƒ markeert een berekende kolom._

</details>

| Term | Beschrijving | Toelichting |
|------|-------------|-------------------|
| **ERB Table** | One row per table of this rulebook (the platform's Schema module), and on it what the table's structure does not say. A class: its label and its place in the cast. A property that is a table: its label, its wording on the page, whether it is symmetric, and its inverse, chain and disjoint axioms. What a table IS is read from its fields: Thing and IsAsserted make a class; Subject with Object or Value make a property, whose domain and range are the tables they link to. | — |
| Table Name | Een gedefinieerd attribuut. | _The table's name again, as a plain column._ |
| Description | Een gedefinieerd attribuut. | _What the table holds. Follows the table._ |
| Is Meta Table | Waar wanneer een lege tekenreeks. | _True for a table that serves the rulebook itself (ERBTables, ERBFields) and not the ontology._ |
| Is Tenant Scoped | Waar wanneer een lege tekenreeks. | _The Schema module's: rows belong to one tenant. Not used by the ontology._ |
| Tenant Parent Table | Een gedefinieerd attribuut. | _The Schema module's: the table tenancy is reached through. Not used by the ontology._ |
| Tenant Parent Field | Een gedefinieerd attribuut. | _The Schema module's: the field that points at TenantParentTable. Not used by the ontology._ |
| Sort Order | Een gedefinieerd attribuut. | _Display order. For a class: Position in the cast. For a property: Position in the Add fact form's property list._ |
| Label | Een gedefinieerd attribuut. | _For a class: The name the page shows. For a property: The property's wording on the page._ |
| Portrait | Een gedefinieerd attribuut. | _For a class: The portrait drawn for this thing; set when its name matches a portrait alias._ |
| Is User Addable | Waar wanneer een lege tekenreeks. | _For a property: Offered in the page's Add fact form._ |
| Is Symmetric | Waar wanneer een lege tekenreeks. | _For a property: The symmetric rule makes this link true in both directions._ |
| Value Node Label | Een gedefinieerd attribuut. | _For a property: The small label under a value node for this property: 'role value' or 'number value'._ |
| Current Value Wording | Een gedefinieerd attribuut. | _For a property: How the Add fact form names a replaced value, as in 'Stan now has grade level 5'._ |
| Input Name | Een gedefinieerd attribuut. | _For a property: How the integer check names this property's value, as in 'for grade level'._ |
| Record Kind | Een gedefinieerd attribuut. | _For a property: The word the current-record summary uses for this property's values: role, grade or school._ |
| Record Order | Een gedefinieerd attribuut. | _For a property: Position of this property in the current-record summary._ |
| Inverse of | Een gedefinieerd attribuut. | _The property that reads this one backwards (OWL inverseOf): when A has this link to B, the inverse rule concludes that B has that one to A. Names a property: a property's table, or a column that is a property._ |
| Chain First | Een gedefinieerd attribuut. | _With ChainSecond, the property chain that concludes this property (OWL propertyChainAxiom): a ChainFirst link followed by a ChainSecond link._ |
| Chain Second | Een gedefinieerd attribuut. | _The chain's second link. Set together with ChainFirst._ |
| Disjoint With | Een gedefinieerd attribuut. | _Properties that never both hold between the same two things (OWL propertyDisjointWith). Several are separated by ', '._ |

<details open>
<summary>Voorbeeldgegevens</summary>

| Table Name | Description | Is Meta Table | Is Tenant Scoped | Tenant Parent Field | Sort Order | Label | Is User Addable | Is Symmetric | Value Node Label | Current Value Wording | Input Name | Record Kind | Record Order | Inverse of | Chain First | Chain Second | Disjoint With |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| Things | Every individual of the ontology (owl:Thing). A class's table lists the individuals that belong to it. | false | — | — | — | — | — | — | — | — | — | — | — | — | — | — | — |
| Person | The class Person: one row per individual that belongs to it. Its label and place are its ERBTables row. | false | — | — | 8 | Person | — | — | — | — | — | — | — | — | — | — | — |
| School | The class School: one row per individual that belongs to it. Its label and place are its ERBTables row. | false | — | — | 9 | School | — | — | — | — | — | — | — | — | — | — | — |

</details>

| Term | Beschrijving | Toelichting |
|------|-------------|-------------------|
| **ERB Field** | One row per field of every table (the platform's Schema module), and on it the same attributes for a property that is a column: a property with one current value, on its domain's class table (on Things when it has no domain class). | — |
| ERB Table | Een gedefinieerd attribuut. | _The table the field is on._ |
| Field Name | Een gedefinieerd attribuut. | _The field's name. For a column that is a property: the property's id._ |
| Field Type | Een gedefinieerd attribuut. | _raw or relationship. Follows the field._ |
| Datatype | Een gedefinieerd attribuut. | _The rulebook datatype. Follows the field._ |
| Is Primary Key | Waar wanneer een lege tekenreeks. | _True for the field that identifies a row of its table._ |
| Description | Een gedefinieerd attribuut. | _What the field means. Follows the field._ |
| Label | Een gedefinieerd attribuut. | _For a property: The property's wording on the page._ |
| Is User Addable | Waar wanneer een lege tekenreeks. | _For a property: Offered in the page's Add fact form._ |
| Is Symmetric | Waar wanneer een lege tekenreeks. | _For a property: The symmetric rule makes this link true in both directions._ |
| Sort Order | Een gedefinieerd attribuut. | _For a property: Position in the Add fact form's property list._ |
| Value Node Label | Een gedefinieerd attribuut. | _For a property: The small label under a value node for this property: 'role value' or 'number value'._ |
| Current Value Wording | Een gedefinieerd attribuut. | _For a property: How the Add fact form names a replaced value, as in 'Stan now has grade level 5'._ |
| Input Name | Een gedefinieerd attribuut. | _For a property: How the integer check names this property's value, as in 'for grade level'._ |
| Record Kind | Een gedefinieerd attribuut. | _For a property: The word the current-record summary uses for this property's values: role, grade or school._ |
| Record Order | Een gedefinieerd attribuut. | _For a property: Position of this property in the current-record summary._ |
| Inverse of | Een gedefinieerd attribuut. | _The property that reads this one backwards (OWL inverseOf): when A has this link to B, the inverse rule concludes that B has that one to A. Names a property: a property's table, or a column that is a property._ |
| Chain First | Een gedefinieerd attribuut. | _With ChainSecond, the property chain that concludes this property (OWL propertyChainAxiom): a ChainFirst link followed by a ChainSecond link._ |
| Chain Second | Een gedefinieerd attribuut. | _The chain's second link. Set together with ChainFirst._ |
| Disjoint With | Een gedefinieerd attribuut. | _Properties that never both hold between the same two things (OWL propertyDisjointWith). Several are separated by ', '._ |

<details open>
<summary>Voorbeeldgegevens</summary>

| Field Name | Field Type | Datatype | Is Primary Key | Description | Label | Is User Addable | Is Symmetric | Sort Order | Value Node Label | Current Value Wording | Input Name | Record Kind | Record Order | Inverse of | Chain First | Chain Second | Disjoint With |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| ThingId | raw | string | true | Key of the thing; also its local name in the model namespace. | — | — | — | — | — | — | — | — | — | — | — | — | — |
| Label | raw | string | false | The name the page shows. | — | — | — | — | — | — | — | — | — | — | — | — | — |
| SortOrder | raw | integer | false | Position in the cast. | — | — | — | — | — | — | — | — | — | — | — | — | — |

</details>

## 2 Feittypen

- een **thing** kan naar één **portrait** verwijzen
- een **person** verwijst naar precies één **thing**
- een **school** verwijst naar precies één **thing**
- een **is parent of** verwijst naar precies één **thing**
- een **is child of** verwijst naar precies één **thing**
- een **is spouse of** verwijst naar precies één **thing**
- een **is friend of** verwijst naar precies één **thing**
- een **attend** verwijst naar precies één **person**
- een **attend** verwijst naar precies één **school**
- een **has age** verwijst naar precies één **thing**
- een **is grandparent of** verwijst naar precies één **thing**
- een **ERB table** kan naar één **ERB table** verwijzen
- een **ERB table** kan naar één **portrait** verwijzen
- een **ERB field** kan naar één **ERB table** verwijzen

## 3 Operationele regels

_Operationele regels benoemen wat het bedrijf **verplicht**, **verbiedt** of
aanraadt (**zou moeten**). Structurele regels komen uit verplichte velden en
vreemde sleutels; semantische regels uit de Constraints-tabel, elk steunend op een
boolean die het regelboek al berekent (verwezen als DR-N in de definitieregels)._

### Structurele beperkingen (uit het schema)

- Een thing **moet** een label en een sort order hebben.
- Een person **moet** naar precies één thing verwijzen.
- Een person **moet** vastleggen of het is asserted.
- Een school **moet** naar precies één thing verwijzen.
- Een school **moet** vastleggen of het is asserted.
- Een is parent of **moet** naar precies één thing verwijzen als zijn subject.
- Een is parent of **moet** naar precies één thing verwijzen als zijn object.
- Een is child of **moet** naar precies één thing verwijzen als zijn subject.
- Een is child of **moet** naar precies één thing verwijzen als zijn object.
- Een is spouse of **moet** naar precies één thing verwijzen als zijn subject.
- Een is spouse of **moet** naar precies één thing verwijzen als zijn object.
- Een is friend of **moet** naar precies één thing verwijzen als zijn subject.
- Een is friend of **moet** naar precies één thing verwijzen als zijn object.
- Een attend **moet** naar precies één person verwijzen als zijn subject.
- Een attend **moet** naar precies één school verwijzen als zijn object.
- Een has age **moet** naar precies één thing verwijzen als zijn subject.
- Een has age **moet** een value hebben.
- Een is grandparent of **moet** naar precies één thing verwijzen als zijn subject.
- Een is grandparent of **moet** naar precies één thing verwijzen als zijn object.
- Een portrait **moet** een display name; een page; een image; een crop; een width; een height; en een sort order hebben.

## 4 Definitieregels

_Alle uitspraken drukken waarheden van het bedrijfsdomein uit; het zijn geen
procedures en geen imperatieven. ‘dan en slechts dan’ wordt vermeden ten gunste van
‘alleen als’, zodat een eenzijdige noodzaak niet wordt aangezien voor een equivalentie.
Een **⚠︎ mechanisch**-markering duidt een regel aan waarvan de deterministische
formulering trouw maar houterig is — een signaal voor optionele herformulering, geen fout._

| ID | Declaratieve regel |
|----|------------------|
| **DR-1 Name** | Een portrait: name is gelijk aan display name. |

## 5 Herleidbaarheid naar het schema

_De expressiekolom is de definitie van de regel in Documentatie-notatie —
dezelfde logica die het regelboek opslaat, geschreven voor een zakelijke lezer._

| Schema-element | Soort | Expressie |
|----------------|------|------------|
| **Portraits.Name** | formule | `DisplayName` |

---

> **RuleSpeak®** — de declaratieve notatie voor bedrijfsregels van **Ronald G. Ross**, volgens de conventies van **SBVR** — komt binnenkort. Tot die tijd toont deze pagina de gewone documentatie. [www.RonRoss.info](https://www.RonRoss.info)
