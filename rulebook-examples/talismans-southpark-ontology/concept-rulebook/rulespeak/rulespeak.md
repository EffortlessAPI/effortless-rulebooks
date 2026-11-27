# 📘 Talisman's South Park ontology explainer: the concept form — Docs

_This ontology in the concept form: a table of individuals, a table per class, a table or a column per property, and ERBTables and ERBFields for what the structure cannot say. orchestration/ontology_form.py wrote it; injecting it over the ontology brings the ontology's things, properties, axioms and facts in line with these tables._

> Declarative business rules rendered from the rulebook. Every statement
> below expresses truth in the business domain — it is neither a procedure
> nor an imperative. The rulebook's formulas are the single source of truth;
> this document is their plain-language reading.

## 1 Business Vocabulary

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Thing** | Every individual of the ontology (owl:Thing). A class's table lists the individuals that belong to it. | — |
| Label | A defined attribute. | _The name the page shows._ |
| Sort Order | A defined attribute. | _Position in the cast._ |
| Portrait | A defined attribute. | _The portrait drawn for this thing; set when its name matches a portrait alias._ |

<details open>
<summary>Example data</summary>

| Label | Sort Order |
|---|---|
| Marvin | 1 |
| Randy | 2 |
| Sharon | 3 |

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Person** | The class Person: one row per individual that belongs to it. Its label and place are its ERBTables row. | — |
| Thing | A defined attribute. | _The individual's row in Things._ |
| Is Asserted | True when an empty string. | _True when a fact states it is a Person. False when only a property's domain or range implies it: such a row follows from the facts and is not added by hand._ |
| has Grade Level | A defined attribute. | _has grade level: the one current value. A property of the ontology; its other attributes are its ERBFields row._ |
| has Role | A defined attribute. | _has role: the one current value. A property of the ontology; its other attributes are its ERBFields row._ |

<details open>
<summary>Example data</summary>

| Is Asserted | has Grade Level | has Role |
|---|---|---|
| true | 4 | Student |

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **School** | The class School: one row per individual that belongs to it. Its label and place are its ERBTables row. | — |
| Thing | A defined attribute. | _The individual's row in Things._ |
| Is Asserted | True when an empty string. | _True when a fact states it is a School. False when only a property's domain or range implies it: such a row follows from the facts and is not added by hand._ |

<details open>
<summary>Example data</summary>

| Is Asserted |
|---|
| false |

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **is Parent of** | is parent of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | A defined attribute. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | A defined attribute. | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **is Child of** | is child of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | A defined attribute. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | A defined attribute. | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **is Spouse of** | is spouse of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | A defined attribute. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | A defined attribute. | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **is Friend of** | is friend of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | A defined attribute. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | A defined attribute. | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **attend** | attends: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | A defined attribute. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | A defined attribute. | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **has Age** | has age: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | A defined attribute. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Value | A defined attribute. | _The value._ |

<details open>
<summary>Example data</summary>

| Value |
|---|
| 10 |

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **is Grandparent of** | is grandparent of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | A defined attribute. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | A defined attribute. | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

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
| **ERB Table** | One row per table of this rulebook (the platform's Schema module), and on it what the table's structure does not say. A class: its label and its place in the cast. A property that is a table: its label, its wording on the page, whether it is symmetric, and its inverse, chain and disjoint axioms. What a table IS is read from its fields: Thing and IsAsserted make a class; Subject with Object or Value make a property, whose domain and range are the tables they link to. | — |
| Table Name | A defined attribute. | _The table's name again, as a plain column._ |
| Description | A defined attribute. | _What the table holds. Follows the table._ |
| Is Meta Table | True when an empty string. | _True for a table that serves the rulebook itself (ERBTables, ERBFields) and not the ontology._ |
| Is Tenant Scoped | True when an empty string. | _The Schema module's: rows belong to one tenant. Not used by the ontology._ |
| Tenant Parent Table | A defined attribute. | _The Schema module's: the table tenancy is reached through. Not used by the ontology._ |
| Tenant Parent Field | A defined attribute. | _The Schema module's: the field that points at TenantParentTable. Not used by the ontology._ |
| Sort Order | A defined attribute. | _Display order. For a class: Position in the cast. For a property: Position in the Add fact form's property list._ |
| Label | A defined attribute. | _For a class: The name the page shows. For a property: The property's wording on the page._ |
| Portrait | A defined attribute. | _For a class: The portrait drawn for this thing; set when its name matches a portrait alias._ |
| Is User Addable | True when an empty string. | _For a property: Offered in the page's Add fact form._ |
| Is Symmetric | True when an empty string. | _For a property: The symmetric rule makes this link true in both directions._ |
| Value Node Label | A defined attribute. | _For a property: The small label under a value node for this property: 'role value' or 'number value'._ |
| Current Value Wording | A defined attribute. | _For a property: How the Add fact form names a replaced value, as in 'Stan now has grade level 5'._ |
| Input Name | A defined attribute. | _For a property: How the integer check names this property's value, as in 'for grade level'._ |
| Record Kind | A defined attribute. | _For a property: The word the current-record summary uses for this property's values: role, grade or school._ |
| Record Order | A defined attribute. | _For a property: Position of this property in the current-record summary._ |
| Inverse of | A defined attribute. | _The property that reads this one backwards (OWL inverseOf): when A has this link to B, the inverse rule concludes that B has that one to A. Names a property: a property's table, or a column that is a property._ |
| Chain First | A defined attribute. | _With ChainSecond, the property chain that concludes this property (OWL propertyChainAxiom): a ChainFirst link followed by a ChainSecond link._ |
| Chain Second | A defined attribute. | _The chain's second link. Set together with ChainFirst._ |
| Disjoint With | A defined attribute. | _Properties that never both hold between the same two things (OWL propertyDisjointWith). Several are separated by ', '._ |

<details open>
<summary>Example data</summary>

| Table Name | Description | Is Meta Table | Is Tenant Scoped | Tenant Parent Field | Sort Order | Label | Is User Addable | Is Symmetric | Value Node Label | Current Value Wording | Input Name | Record Kind | Record Order | Inverse of | Chain First | Chain Second | Disjoint With |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| Things | Every individual of the ontology (owl:Thing). A class's table lists the individuals that belong to it. | false | — | — | — | — | — | — | — | — | — | — | — | — | — | — | — |
| Person | The class Person: one row per individual that belongs to it. Its label and place are its ERBTables row. | false | — | — | 8 | Person | — | — | — | — | — | — | — | — | — | — | — |
| School | The class School: one row per individual that belongs to it. Its label and place are its ERBTables row. | false | — | — | 9 | School | — | — | — | — | — | — | — | — | — | — | — |

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **ERB Field** | One row per field of every table (the platform's Schema module), and on it the same attributes for a property that is a column: a property with one current value, on its domain's class table (on Things when it has no domain class). | — |
| ERB Table | A defined attribute. | _The table the field is on._ |
| Field Name | A defined attribute. | _The field's name. For a column that is a property: the property's id._ |
| Field Type | A defined attribute. | _raw or relationship. Follows the field._ |
| Datatype | A defined attribute. | _The rulebook datatype. Follows the field._ |
| Is Primary Key | True when an empty string. | _True for the field that identifies a row of its table._ |
| Description | A defined attribute. | _What the field means. Follows the field._ |
| Label | A defined attribute. | _For a property: The property's wording on the page._ |
| Is User Addable | True when an empty string. | _For a property: Offered in the page's Add fact form._ |
| Is Symmetric | True when an empty string. | _For a property: The symmetric rule makes this link true in both directions._ |
| Sort Order | A defined attribute. | _For a property: Position in the Add fact form's property list._ |
| Value Node Label | A defined attribute. | _For a property: The small label under a value node for this property: 'role value' or 'number value'._ |
| Current Value Wording | A defined attribute. | _For a property: How the Add fact form names a replaced value, as in 'Stan now has grade level 5'._ |
| Input Name | A defined attribute. | _For a property: How the integer check names this property's value, as in 'for grade level'._ |
| Record Kind | A defined attribute. | _For a property: The word the current-record summary uses for this property's values: role, grade or school._ |
| Record Order | A defined attribute. | _For a property: Position of this property in the current-record summary._ |
| Inverse of | A defined attribute. | _The property that reads this one backwards (OWL inverseOf): when A has this link to B, the inverse rule concludes that B has that one to A. Names a property: a property's table, or a column that is a property._ |
| Chain First | A defined attribute. | _With ChainSecond, the property chain that concludes this property (OWL propertyChainAxiom): a ChainFirst link followed by a ChainSecond link._ |
| Chain Second | A defined attribute. | _The chain's second link. Set together with ChainFirst._ |
| Disjoint With | A defined attribute. | _Properties that never both hold between the same two things (OWL propertyDisjointWith). Several are separated by ', '._ |

<details open>
<summary>Example data</summary>

| Field Name | Field Type | Datatype | Is Primary Key | Description | Label | Is User Addable | Is Symmetric | Sort Order | Value Node Label | Current Value Wording | Input Name | Record Kind | Record Order | Inverse of | Chain First | Chain Second | Disjoint With |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| ThingId | raw | string | true | Key of the thing; also its local name in the model namespace. | — | — | — | — | — | — | — | — | — | — | — | — | — |
| Label | raw | string | false | The name the page shows. | — | — | — | — | — | — | — | — | — | — | — | — | — |
| SortOrder | raw | integer | false | Position in the cast. | — | — | — | — | — | — | — | — | — | — | — | — | — |

</details>

## 2 Fact Types

- a **thing** may reference one **portrait**
- a **person** references exactly one **thing**
- a **school** references exactly one **thing**
- an **is parent of** references exactly one **thing**
- an **is child of** references exactly one **thing**
- an **is spouse of** references exactly one **thing**
- an **is friend of** references exactly one **thing**
- an **attend** references exactly one **person**
- an **attend** references exactly one **school**
- a **has age** references exactly one **thing**
- an **is grandparent of** references exactly one **thing**
- an **ERB table** may reference one **ERB table**
- an **ERB table** may reference one **portrait**
- an **ERB field** may reference one **ERB table**

## 3 Operative Rules

_Operative rules state what the business **obliges**, **prohibits**, or
advises (**should**). Structural rules come from required fields and foreign keys;
semantic rules come from the Constraints table, each keyed on a boolean the rulebook
already computes (cross-referenced as DR-N in the Definitional Rules below)._

### Structural Constraints (from the schema)

- A thing **must** have a label and a sort order.
- A person **must** reference exactly one thing.
- A person **must** record whether it is asserted.
- A school **must** reference exactly one thing.
- A school **must** record whether it is asserted.
- An is parent of **must** reference exactly one thing as its subject.
- An is parent of **must** reference exactly one thing as its object.
- An is child of **must** reference exactly one thing as its subject.
- An is child of **must** reference exactly one thing as its object.
- An is spouse of **must** reference exactly one thing as its subject.
- An is spouse of **must** reference exactly one thing as its object.
- An is friend of **must** reference exactly one thing as its subject.
- An is friend of **must** reference exactly one thing as its object.
- An attend **must** reference exactly one person as its subject.
- An attend **must** reference exactly one school as its object.
- A has age **must** reference exactly one thing as its subject.
- A has age **must** have a value.
- An is grandparent of **must** reference exactly one thing as its subject.
- An is grandparent of **must** reference exactly one thing as its object.
- A portrait **must** have a display name; a page; an image; a crop; a width; a height; and a sort order.

## 4 Definitional Rules

_All statements express truth in the business domain; they are neither
procedures nor imperatives. "iff" is avoided in favor of "only if" so a
one-directional necessity is not mistaken for an equivalence. A
**⚠︎ mechanical** chip marks a rule whose deterministic wording is faithful
but clunky — a flag for an optional downstream reword pass, not a defect._

| ID | Declarative rule |
|----|------------------|
| **DR-1 Name** | A portrait's name is the same as its display name. |

## 5 Traceability to Schema

_The expression column is the rule's definition in Docs notation —
the same logic the rulebook stores, written for a business reader._

| Schema element | Kind | Expression |
|----------------|------|------------|
| **Portraits.Name** | formula | `DisplayName` |

---

> **RuleSpeak®** — the declarative business-rule notation created by **Ronald G. Ross**, following the conventions of **SBVR** — is coming soon. Until then this page shows the regular documentation. [www.RonRoss.info](https://www.RonRoss.info)
