# 📘 Talisman's South Park ontology explainer: the concept form — Documentazione

_This ontology in the concept form: a table of individuals, a table per class, a table or a column per property, and ERBTables and ERBFields for what the structure cannot say. orchestration/ontology_form.py wrote it; injecting it over the ontology brings the ontology's things, properties, axioms and facts in line with these tables._

> Regole di business dichiarative generate dal libro delle regole. Ogni enunciato
> esprime una verità del dominio di business — non è una procedura né un imperativo.
> Le formule del libro delle regole sono l'unica fonte di verità; questo documento
> ne è la lettura in linguaggio chiaro.

## 1 Vocabolario di business

| Termine | Descrizione | Commento narrativo |
|------|-------------|-------------------|
| **Thing** | Every individual of the ontology (owl:Thing). A class's table lists the individuals that belong to it. | — |
| Label | Un attributo definito. | _The name the page shows._ |
| Sort Order | Un attributo definito. | _Position in the cast._ |
| Portrait | Un attributo definito. | _The portrait drawn for this thing; set when its name matches a portrait alias._ |

<details open>
<summary>Dati di esempio</summary>

| Label | Sort Order |
|---|---|
| Marvin | 1 |
| Randy | 2 |
| Sharon | 3 |

</details>

| Termine | Descrizione | Commento narrativo |
|------|-------------|-------------------|
| **Person** | The class Person: one row per individual that belongs to it. Its label and place are its ERBTables row. | — |
| Thing | Un attributo definito. | _The individual's row in Things._ |
| Is Asserted | Vero quando una stringa vuota. | _True when a fact states it is a Person. False when only a property's domain or range implies it: such a row follows from the facts and is not added by hand._ |
| has Grade Level | Un attributo definito. | _has grade level: the one current value. A property of the ontology; its other attributes are its ERBFields row._ |
| has Role | Un attributo definito. | _has role: the one current value. A property of the ontology; its other attributes are its ERBFields row._ |

<details open>
<summary>Dati di esempio</summary>

| Is Asserted | has Grade Level | has Role |
|---|---|---|
| true | 4 | Student |

</details>

| Termine | Descrizione | Commento narrativo |
|------|-------------|-------------------|
| **School** | The class School: one row per individual that belongs to it. Its label and place are its ERBTables row. | — |
| Thing | Un attributo definito. | _The individual's row in Things._ |
| Is Asserted | Vero quando una stringa vuota. | _True when a fact states it is a School. False when only a property's domain or range implies it: such a row follows from the facts and is not added by hand._ |

<details open>
<summary>Dati di esempio</summary>

| Is Asserted |
|---|
| false |

</details>

| Termine | Descrizione | Commento narrativo |
|------|-------------|-------------------|
| **is Parent of** | is parent of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | Un attributo definito. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | Un attributo definito. | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| Termine | Descrizione | Commento narrativo |
|------|-------------|-------------------|
| **is Child of** | is child of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | Un attributo definito. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | Un attributo definito. | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| Termine | Descrizione | Commento narrativo |
|------|-------------|-------------------|
| **is Spouse of** | is spouse of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | Un attributo definito. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | Un attributo definito. | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| Termine | Descrizione | Commento narrativo |
|------|-------------|-------------------|
| **is Friend of** | is friend of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | Un attributo definito. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | Un attributo definito. | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| Termine | Descrizione | Commento narrativo |
|------|-------------|-------------------|
| **attend** | attends: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | Un attributo definito. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | Un attributo definito. | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| Termine | Descrizione | Commento narrativo |
|------|-------------|-------------------|
| **has Age** | has age: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | Un attributo definito. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Value | Un attributo definito. | _The value._ |

<details open>
<summary>Dati di esempio</summary>

| Value |
|---|
| 10 |

</details>

| Termine | Descrizione | Commento narrativo |
|------|-------------|-------------------|
| **is Grandparent of** | is grandparent of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | Un attributo definito. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | Un attributo definito. | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| Termine | Descrizione | Commento narrativo |
|------|-------------|-------------------|
| **Portrait** | The page's Fandom portraits, including those for characters a reader may add. | — |
| Display Name | Un attributo definito. | _The name on the portrait's Fandom page._ |
| Page | Un attributo definito. | _The Fandom page credited for the portrait._ |
| Image | Un attributo definito. | _The original Fandom image._ |
| Crop | Un attributo definito. | _The crop as an SVG viewBox: x y width height._ |
| Width | Un attributo definito. | _The original image's width in pixels._ |
| Height | Un attributo definito. | _The original image's height in pixels._ |
| Sort Order | Un attributo definito. | _Position in the credits._ |
| Name | Uguale a display name. | _Order 1. The portrait's name._ |

<details open>
<summary>Dati di esempio</summary>

| Display Name | Page | Image | Crop | Width | Height | Sort Order | Name ƒ |
|---|---|---|---|---|---|---|---|
| Marvin Marsh | https://southpark.fandom.com/wiki/Marvin_Marsh | https://static.wikia.nocookie.net/southpark/images/d/db/Marsh-family-grandpa-marsh-w-purple-blanket.png/revision/latest | 48 -5 280 280 | 375 | 536 | 1 | Marvin Marsh |
| Randy Marsh | https://southpark.fandom.com/wiki/Randy_Marsh | https://static.wikia.nocookie.net/southparkfanon/images/d/d6/Randy_Marsh.png/revision/latest | 0 -10 428 428 | 428 | 1057 | 2 | Randy Marsh |
| Sharon Marsh | https://southpark.fandom.com/wiki/Sharon_Marsh | https://static.wikia.nocookie.net/southpark/images/9/99/Sharon_Marsh.png/revision/latest | 20 -10 390 390 | 439 | 1049 | 3 | Sharon Marsh |

_ƒ indica una colonna calcolata._

</details>

| Termine | Descrizione | Commento narrativo |
|------|-------------|-------------------|
| **ERB Table** | One row per table of this rulebook (the platform's Schema module), and on it what the table's structure does not say. A class: its label and its place in the cast. A property that is a table: its label, its wording on the page, whether it is symmetric, and its inverse, chain and disjoint axioms. What a table IS is read from its fields: Thing and IsAsserted make a class; Subject with Object or Value make a property, whose domain and range are the tables they link to. | — |
| Table Name | Un attributo definito. | _The table's name again, as a plain column._ |
| Description | Un attributo definito. | _What the table holds. Follows the table._ |
| Is Meta Table | Vero quando una stringa vuota. | _True for a table that serves the rulebook itself (ERBTables, ERBFields) and not the ontology._ |
| Is Tenant Scoped | Vero quando una stringa vuota. | _The Schema module's: rows belong to one tenant. Not used by the ontology._ |
| Tenant Parent Table | Un attributo definito. | _The Schema module's: the table tenancy is reached through. Not used by the ontology._ |
| Tenant Parent Field | Un attributo definito. | _The Schema module's: the field that points at TenantParentTable. Not used by the ontology._ |
| Sort Order | Un attributo definito. | _Display order. For a class: Position in the cast. For a property: Position in the Add fact form's property list._ |
| Label | Un attributo definito. | _For a class: The name the page shows. For a property: The property's wording on the page._ |
| Portrait | Un attributo definito. | _For a class: The portrait drawn for this thing; set when its name matches a portrait alias._ |
| Is User Addable | Vero quando una stringa vuota. | _For a property: Offered in the page's Add fact form._ |
| Is Symmetric | Vero quando una stringa vuota. | _For a property: The symmetric rule makes this link true in both directions._ |
| Value Node Label | Un attributo definito. | _For a property: The small label under a value node for this property: 'role value' or 'number value'._ |
| Current Value Wording | Un attributo definito. | _For a property: How the Add fact form names a replaced value, as in 'Stan now has grade level 5'._ |
| Input Name | Un attributo definito. | _For a property: How the integer check names this property's value, as in 'for grade level'._ |
| Record Kind | Un attributo definito. | _For a property: The word the current-record summary uses for this property's values: role, grade or school._ |
| Record Order | Un attributo definito. | _For a property: Position of this property in the current-record summary._ |
| Inverse of | Un attributo definito. | _The property that reads this one backwards (OWL inverseOf): when A has this link to B, the inverse rule concludes that B has that one to A. Names a property: a property's table, or a column that is a property._ |
| Chain First | Un attributo definito. | _With ChainSecond, the property chain that concludes this property (OWL propertyChainAxiom): a ChainFirst link followed by a ChainSecond link._ |
| Chain Second | Un attributo definito. | _The chain's second link. Set together with ChainFirst._ |
| Disjoint With | Un attributo definito. | _Properties that never both hold between the same two things (OWL propertyDisjointWith). Several are separated by ', '._ |

<details open>
<summary>Dati di esempio</summary>

| Table Name | Description | Is Meta Table | Is Tenant Scoped | Tenant Parent Field | Sort Order | Label | Is User Addable | Is Symmetric | Value Node Label | Current Value Wording | Input Name | Record Kind | Record Order | Inverse of | Chain First | Chain Second | Disjoint With |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| Things | Every individual of the ontology (owl:Thing). A class's table lists the individuals that belong to it. | false | — | — | — | — | — | — | — | — | — | — | — | — | — | — | — |
| Person | The class Person: one row per individual that belongs to it. Its label and place are its ERBTables row. | false | — | — | 8 | Person | — | — | — | — | — | — | — | — | — | — | — |
| School | The class School: one row per individual that belongs to it. Its label and place are its ERBTables row. | false | — | — | 9 | School | — | — | — | — | — | — | — | — | — | — | — |

</details>

| Termine | Descrizione | Commento narrativo |
|------|-------------|-------------------|
| **ERB Field** | One row per field of every table (the platform's Schema module), and on it the same attributes for a property that is a column: a property with one current value, on its domain's class table (on Things when it has no domain class). | — |
| ERB Table | Un attributo definito. | _The table the field is on._ |
| Field Name | Un attributo definito. | _The field's name. For a column that is a property: the property's id._ |
| Field Type | Un attributo definito. | _raw or relationship. Follows the field._ |
| Datatype | Un attributo definito. | _The rulebook datatype. Follows the field._ |
| Is Primary Key | Vero quando una stringa vuota. | _True for the field that identifies a row of its table._ |
| Description | Un attributo definito. | _What the field means. Follows the field._ |
| Label | Un attributo definito. | _For a property: The property's wording on the page._ |
| Is User Addable | Vero quando una stringa vuota. | _For a property: Offered in the page's Add fact form._ |
| Is Symmetric | Vero quando una stringa vuota. | _For a property: The symmetric rule makes this link true in both directions._ |
| Sort Order | Un attributo definito. | _For a property: Position in the Add fact form's property list._ |
| Value Node Label | Un attributo definito. | _For a property: The small label under a value node for this property: 'role value' or 'number value'._ |
| Current Value Wording | Un attributo definito. | _For a property: How the Add fact form names a replaced value, as in 'Stan now has grade level 5'._ |
| Input Name | Un attributo definito. | _For a property: How the integer check names this property's value, as in 'for grade level'._ |
| Record Kind | Un attributo definito. | _For a property: The word the current-record summary uses for this property's values: role, grade or school._ |
| Record Order | Un attributo definito. | _For a property: Position of this property in the current-record summary._ |
| Inverse of | Un attributo definito. | _The property that reads this one backwards (OWL inverseOf): when A has this link to B, the inverse rule concludes that B has that one to A. Names a property: a property's table, or a column that is a property._ |
| Chain First | Un attributo definito. | _With ChainSecond, the property chain that concludes this property (OWL propertyChainAxiom): a ChainFirst link followed by a ChainSecond link._ |
| Chain Second | Un attributo definito. | _The chain's second link. Set together with ChainFirst._ |
| Disjoint With | Un attributo definito. | _Properties that never both hold between the same two things (OWL propertyDisjointWith). Several are separated by ', '._ |

<details open>
<summary>Dati di esempio</summary>

| Field Name | Field Type | Datatype | Is Primary Key | Description | Label | Is User Addable | Is Symmetric | Sort Order | Value Node Label | Current Value Wording | Input Name | Record Kind | Record Order | Inverse of | Chain First | Chain Second | Disjoint With |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| ThingId | raw | string | true | Key of the thing; also its local name in the model namespace. | — | — | — | — | — | — | — | — | — | — | — | — | — |
| Label | raw | string | false | The name the page shows. | — | — | — | — | — | — | — | — | — | — | — | — | — |
| SortOrder | raw | integer | false | Position in the cast. | — | — | — | — | — | — | — | — | — | — | — | — | — |

</details>

## 2 Tipi di fatto

- un **thing** può referenziare un **portrait**
- un **person** referenzia esattamente un **thing**
- un **school** referenzia esattamente un **thing**
- un **is parent of** referenzia esattamente un **thing**
- un **is child of** referenzia esattamente un **thing**
- un **is spouse of** referenzia esattamente un **thing**
- un **is friend of** referenzia esattamente un **thing**
- un **attend** referenzia esattamente un **person**
- un **attend** referenzia esattamente un **school**
- un **has age** referenzia esattamente un **thing**
- un **is grandparent of** referenzia esattamente un **thing**
- un **ERB table** può referenziare un **ERB table**
- un **ERB table** può referenziare un **portrait**
- un **ERB field** può referenziare un **ERB table**

## 3 Regole operative

_Le regole operative enunciano ciò che il business **obbliga**, **vieta** o
consiglia (**dovrebbe**). Le regole strutturali derivano da campi obbligatori e chiavi
esterne; quelle semantiche dalla tabella Constraints, ognuna basata su un booleano che
il libro delle regole già calcola (referenziato come DR-N nelle regole definitorie)._

### Vincoli strutturali (dallo schema)

- Un thing **deve** avere un label e un sort order.
- Un person **deve** referenziare esattamente un thing.
- Un person **deve** registrare se è asserted.
- Un school **deve** referenziare esattamente un thing.
- Un school **deve** registrare se è asserted.
- Un is parent of **deve** referenziare esattamente un thing come suo subject.
- Un is parent of **deve** referenziare esattamente un thing come suo object.
- Un is child of **deve** referenziare esattamente un thing come suo subject.
- Un is child of **deve** referenziare esattamente un thing come suo object.
- Un is spouse of **deve** referenziare esattamente un thing come suo subject.
- Un is spouse of **deve** referenziare esattamente un thing come suo object.
- Un is friend of **deve** referenziare esattamente un thing come suo subject.
- Un is friend of **deve** referenziare esattamente un thing come suo object.
- Un attend **deve** referenziare esattamente un person come suo subject.
- Un attend **deve** referenziare esattamente un school come suo object.
- Un has age **deve** referenziare esattamente un thing come suo subject.
- Un has age **deve** avere un value.
- Un is grandparent of **deve** referenziare esattamente un thing come suo subject.
- Un is grandparent of **deve** referenziare esattamente un thing come suo object.
- Un portrait **deve** avere un display name; un page; un image; un crop; un width; un height; e un sort order.

## 4 Regole definitorie

_Tutti gli enunciati esprimono verità del dominio di business; non sono procedure
né imperativi. Si evita «se e solo se» a favore di «solo se», affinché una necessità
unidirezionale non venga scambiata per un'equivalenza. Un contrassegno
**⚠︎ meccanica** segnala una regola la cui formulazione deterministica è fedele ma
legnosa — un segnale per una riscrittura opzionale, non un difetto._

| ID | Regola dichiarativa |
|----|------------------|
| **DR-1 Name** | Un portrait: name è uguale a display name. |

## 5 Tracciabilità allo schema

_La colonna espressione è la definizione della regola in notazione Documentazione —
la stessa logica che il libro delle regole memorizza, scritta per un lettore di business._

| Elemento dello schema | Tipo | Espressione |
|----------------|------|------------|
| **Portraits.Name** | formula | `DisplayName` |

---

> **RuleSpeak®** — la notazione dichiarativa delle regole di business creata da **Ronald G. Ross**, secondo le convenzioni di **SBVR** — arriverà presto. Fino ad allora questa pagina mostra la documentazione normale. [www.RonRoss.info](https://www.RonRoss.info)
