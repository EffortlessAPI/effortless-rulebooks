# 📘 Talisman's South Park ontology explainer: the concept form — Documentación

_This ontology in the concept form: a table of individuals, a table per class, a table or a column per property, and ERBTables and ERBFields for what the structure cannot say. orchestration/ontology_form.py wrote it; injecting it over the ontology brings the ontology's things, properties, axioms and facts in line with these tables._

> Reglas de negocio declarativas generadas a partir del libro de reglas. Cada
> enunciado expresa una verdad del dominio de negocio — no es un procedimiento
> ni un imperativo. Las fórmulas del libro de reglas son la única fuente de la
> verdad; este documento es su lectura en lenguaje claro.

## 1 Vocabulario de negocio

| Término | Descripción | Comentario narrativo |
|------|-------------|-------------------|
| **Thing** | Every individual of the ontology (owl:Thing). A class's table lists the individuals that belong to it. | — |
| Label | Un atributo definido. | _The name the page shows._ |
| Sort Order | Un atributo definido. | _Position in the cast._ |
| Portrait | Un atributo definido. | _The portrait drawn for this thing; set when its name matches a portrait alias._ |

<details open>
<summary>Datos de ejemplo</summary>

| Label | Sort Order |
|---|---|
| Marvin | 1 |
| Randy | 2 |
| Sharon | 3 |

</details>

| Término | Descripción | Comentario narrativo |
|------|-------------|-------------------|
| **Person** | The class Person: one row per individual that belongs to it. Its label and place are its ERBTables row. | — |
| Thing | Un atributo definido. | _The individual's row in Things._ |
| Is Asserted | Verdadero cuando una cadena vacía. | _True when a fact states it is a Person. False when only a property's domain or range implies it: such a row follows from the facts and is not added by hand._ |
| has Grade Level | Un atributo definido. | _has grade level: the one current value. A property of the ontology; its other attributes are its ERBFields row._ |
| has Role | Un atributo definido. | _has role: the one current value. A property of the ontology; its other attributes are its ERBFields row._ |

<details open>
<summary>Datos de ejemplo</summary>

| Is Asserted | has Grade Level | has Role |
|---|---|---|
| true | 4 | Student |

</details>

| Término | Descripción | Comentario narrativo |
|------|-------------|-------------------|
| **School** | The class School: one row per individual that belongs to it. Its label and place are its ERBTables row. | — |
| Thing | Un atributo definido. | _The individual's row in Things._ |
| Is Asserted | Verdadero cuando una cadena vacía. | _True when a fact states it is a School. False when only a property's domain or range implies it: such a row follows from the facts and is not added by hand._ |

<details open>
<summary>Datos de ejemplo</summary>

| Is Asserted |
|---|
| false |

</details>

| Término | Descripción | Comentario narrativo |
|------|-------------|-------------------|
| **is Parent of** | is parent of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | Un atributo definido. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | Un atributo definido. | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| Término | Descripción | Comentario narrativo |
|------|-------------|-------------------|
| **is Child of** | is child of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | Un atributo definido. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | Un atributo definido. | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| Término | Descripción | Comentario narrativo |
|------|-------------|-------------------|
| **is Spouse of** | is spouse of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | Un atributo definido. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | Un atributo definido. | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| Término | Descripción | Comentario narrativo |
|------|-------------|-------------------|
| **is Friend of** | is friend of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | Un atributo definido. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | Un atributo definido. | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| Término | Descripción | Comentario narrativo |
|------|-------------|-------------------|
| **attend** | attends: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | Un atributo definido. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | Un atributo definido. | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| Término | Descripción | Comentario narrativo |
|------|-------------|-------------------|
| **has Age** | has age: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | Un atributo definido. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Value | Un atributo definido. | _The value._ |

<details open>
<summary>Datos de ejemplo</summary>

| Value |
|---|
| 10 |

</details>

| Término | Descripción | Comentario narrativo |
|------|-------------|-------------------|
| **is Grandparent of** | is grandparent of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | Un atributo definido. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | Un atributo definido. | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| Término | Descripción | Comentario narrativo |
|------|-------------|-------------------|
| **Portrait** | The page's Fandom portraits, including those for characters a reader may add. | — |
| Display Name | Un atributo definido. | _The name on the portrait's Fandom page._ |
| Page | Un atributo definido. | _The Fandom page credited for the portrait._ |
| Image | Un atributo definido. | _The original Fandom image._ |
| Crop | Un atributo definido. | _The crop as an SVG viewBox: x y width height._ |
| Width | Un atributo definido. | _The original image's width in pixels._ |
| Height | Un atributo definido. | _The original image's height in pixels._ |
| Sort Order | Un atributo definido. | _Position in the credits._ |
| Name | Igual a display name. | _Order 1. The portrait's name._ |

<details open>
<summary>Datos de ejemplo</summary>

| Display Name | Page | Image | Crop | Width | Height | Sort Order | Name ƒ |
|---|---|---|---|---|---|---|---|
| Marvin Marsh | https://southpark.fandom.com/wiki/Marvin_Marsh | https://static.wikia.nocookie.net/southpark/images/d/db/Marsh-family-grandpa-marsh-w-purple-blanket.png/revision/latest | 48 -5 280 280 | 375 | 536 | 1 | Marvin Marsh |
| Randy Marsh | https://southpark.fandom.com/wiki/Randy_Marsh | https://static.wikia.nocookie.net/southparkfanon/images/d/d6/Randy_Marsh.png/revision/latest | 0 -10 428 428 | 428 | 1057 | 2 | Randy Marsh |
| Sharon Marsh | https://southpark.fandom.com/wiki/Sharon_Marsh | https://static.wikia.nocookie.net/southpark/images/9/99/Sharon_Marsh.png/revision/latest | 20 -10 390 390 | 439 | 1049 | 3 | Sharon Marsh |

_ƒ indica una columna calculada._

</details>

| Término | Descripción | Comentario narrativo |
|------|-------------|-------------------|
| **ERB Table** | One row per table of this rulebook (the platform's Schema module), and on it what the table's structure does not say. A class: its label and its place in the cast. A property that is a table: its label, its wording on the page, whether it is symmetric, and its inverse, chain and disjoint axioms. What a table IS is read from its fields: Thing and IsAsserted make a class; Subject with Object or Value make a property, whose domain and range are the tables they link to. | — |
| Table Name | Un atributo definido. | _The table's name again, as a plain column._ |
| Description | Un atributo definido. | _What the table holds. Follows the table._ |
| Is Meta Table | Verdadero cuando una cadena vacía. | _True for a table that serves the rulebook itself (ERBTables, ERBFields) and not the ontology._ |
| Is Tenant Scoped | Verdadero cuando una cadena vacía. | _The Schema module's: rows belong to one tenant. Not used by the ontology._ |
| Tenant Parent Table | Un atributo definido. | _The Schema module's: the table tenancy is reached through. Not used by the ontology._ |
| Tenant Parent Field | Un atributo definido. | _The Schema module's: the field that points at TenantParentTable. Not used by the ontology._ |
| Sort Order | Un atributo definido. | _Display order. For a class: Position in the cast. For a property: Position in the Add fact form's property list._ |
| Label | Un atributo definido. | _For a class: The name the page shows. For a property: The property's wording on the page._ |
| Portrait | Un atributo definido. | _For a class: The portrait drawn for this thing; set when its name matches a portrait alias._ |
| Is User Addable | Verdadero cuando una cadena vacía. | _For a property: Offered in the page's Add fact form._ |
| Is Symmetric | Verdadero cuando una cadena vacía. | _For a property: The symmetric rule makes this link true in both directions._ |
| Value Node Label | Un atributo definido. | _For a property: The small label under a value node for this property: 'role value' or 'number value'._ |
| Current Value Wording | Un atributo definido. | _For a property: How the Add fact form names a replaced value, as in 'Stan now has grade level 5'._ |
| Input Name | Un atributo definido. | _For a property: How the integer check names this property's value, as in 'for grade level'._ |
| Record Kind | Un atributo definido. | _For a property: The word the current-record summary uses for this property's values: role, grade or school._ |
| Record Order | Un atributo definido. | _For a property: Position of this property in the current-record summary._ |
| Inverse of | Un atributo definido. | _The property that reads this one backwards (OWL inverseOf): when A has this link to B, the inverse rule concludes that B has that one to A. Names a property: a property's table, or a column that is a property._ |
| Chain First | Un atributo definido. | _With ChainSecond, the property chain that concludes this property (OWL propertyChainAxiom): a ChainFirst link followed by a ChainSecond link._ |
| Chain Second | Un atributo definido. | _The chain's second link. Set together with ChainFirst._ |
| Disjoint With | Un atributo definido. | _Properties that never both hold between the same two things (OWL propertyDisjointWith). Several are separated by ', '._ |

<details open>
<summary>Datos de ejemplo</summary>

| Table Name | Description | Is Meta Table | Is Tenant Scoped | Tenant Parent Field | Sort Order | Label | Is User Addable | Is Symmetric | Value Node Label | Current Value Wording | Input Name | Record Kind | Record Order | Inverse of | Chain First | Chain Second | Disjoint With |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| Things | Every individual of the ontology (owl:Thing). A class's table lists the individuals that belong to it. | false | — | — | — | — | — | — | — | — | — | — | — | — | — | — | — |
| Person | The class Person: one row per individual that belongs to it. Its label and place are its ERBTables row. | false | — | — | 8 | Person | — | — | — | — | — | — | — | — | — | — | — |
| School | The class School: one row per individual that belongs to it. Its label and place are its ERBTables row. | false | — | — | 9 | School | — | — | — | — | — | — | — | — | — | — | — |

</details>

| Término | Descripción | Comentario narrativo |
|------|-------------|-------------------|
| **ERB Field** | One row per field of every table (the platform's Schema module), and on it the same attributes for a property that is a column: a property with one current value, on its domain's class table (on Things when it has no domain class). | — |
| ERB Table | Un atributo definido. | _The table the field is on._ |
| Field Name | Un atributo definido. | _The field's name. For a column that is a property: the property's id._ |
| Field Type | Un atributo definido. | _raw or relationship. Follows the field._ |
| Datatype | Un atributo definido. | _The rulebook datatype. Follows the field._ |
| Is Primary Key | Verdadero cuando una cadena vacía. | _True for the field that identifies a row of its table._ |
| Description | Un atributo definido. | _What the field means. Follows the field._ |
| Label | Un atributo definido. | _For a property: The property's wording on the page._ |
| Is User Addable | Verdadero cuando una cadena vacía. | _For a property: Offered in the page's Add fact form._ |
| Is Symmetric | Verdadero cuando una cadena vacía. | _For a property: The symmetric rule makes this link true in both directions._ |
| Sort Order | Un atributo definido. | _For a property: Position in the Add fact form's property list._ |
| Value Node Label | Un atributo definido. | _For a property: The small label under a value node for this property: 'role value' or 'number value'._ |
| Current Value Wording | Un atributo definido. | _For a property: How the Add fact form names a replaced value, as in 'Stan now has grade level 5'._ |
| Input Name | Un atributo definido. | _For a property: How the integer check names this property's value, as in 'for grade level'._ |
| Record Kind | Un atributo definido. | _For a property: The word the current-record summary uses for this property's values: role, grade or school._ |
| Record Order | Un atributo definido. | _For a property: Position of this property in the current-record summary._ |
| Inverse of | Un atributo definido. | _The property that reads this one backwards (OWL inverseOf): when A has this link to B, the inverse rule concludes that B has that one to A. Names a property: a property's table, or a column that is a property._ |
| Chain First | Un atributo definido. | _With ChainSecond, the property chain that concludes this property (OWL propertyChainAxiom): a ChainFirst link followed by a ChainSecond link._ |
| Chain Second | Un atributo definido. | _The chain's second link. Set together with ChainFirst._ |
| Disjoint With | Un atributo definido. | _Properties that never both hold between the same two things (OWL propertyDisjointWith). Several are separated by ', '._ |

<details open>
<summary>Datos de ejemplo</summary>

| Field Name | Field Type | Datatype | Is Primary Key | Description | Label | Is User Addable | Is Symmetric | Sort Order | Value Node Label | Current Value Wording | Input Name | Record Kind | Record Order | Inverse of | Chain First | Chain Second | Disjoint With |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| ThingId | raw | string | true | Key of the thing; also its local name in the model namespace. | — | — | — | — | — | — | — | — | — | — | — | — | — |
| Label | raw | string | false | The name the page shows. | — | — | — | — | — | — | — | — | — | — | — | — | — |
| SortOrder | raw | integer | false | Position in the cast. | — | — | — | — | — | — | — | — | — | — | — | — | — |

</details>

## 2 Tipos de hechos

- un **thing** puede referenciar un **portrait**
- un **person** referencia exactamente un **thing**
- un **school** referencia exactamente un **thing**
- un **is parent of** referencia exactamente un **thing**
- un **is child of** referencia exactamente un **thing**
- un **is spouse of** referencia exactamente un **thing**
- un **is friend of** referencia exactamente un **thing**
- un **attend** referencia exactamente un **person**
- un **attend** referencia exactamente un **school**
- un **has age** referencia exactamente un **thing**
- un **is grandparent of** referencia exactamente un **thing**
- un **ERB table** puede referenciar un **ERB table**
- un **ERB table** puede referenciar un **portrait**
- un **ERB field** puede referenciar un **ERB table**

## 3 Reglas operativas

_Las reglas operativas enuncian lo que el negocio **obliga**, **prohíbe** o
aconseja (**debería**). Las reglas estructurales provienen de campos obligatorios y claves
foráneas; las semánticas provienen de la tabla Constraints, cada una apoyada en un booleano
que el libro de reglas ya calcula (referenciado como DR-N en las reglas definitorias)._

### Restricciones estructurales (del esquema)

- Un thing **debe** tener un label y un sort order.
- Un person **debe** referenciar exactamente un thing.
- Un person **debe** registrar si es asserted.
- Un school **debe** referenciar exactamente un thing.
- Un school **debe** registrar si es asserted.
- Un is parent of **debe** referenciar exactamente un thing como su subject.
- Un is parent of **debe** referenciar exactamente un thing como su object.
- Un is child of **debe** referenciar exactamente un thing como su subject.
- Un is child of **debe** referenciar exactamente un thing como su object.
- Un is spouse of **debe** referenciar exactamente un thing como su subject.
- Un is spouse of **debe** referenciar exactamente un thing como su object.
- Un is friend of **debe** referenciar exactamente un thing como su subject.
- Un is friend of **debe** referenciar exactamente un thing como su object.
- Un attend **debe** referenciar exactamente un person como su subject.
- Un attend **debe** referenciar exactamente un school como su object.
- Un has age **debe** referenciar exactamente un thing como su subject.
- Un has age **debe** tener un value.
- Un is grandparent of **debe** referenciar exactamente un thing como su subject.
- Un is grandparent of **debe** referenciar exactamente un thing como su object.
- Un portrait **debe** tener un display name; un page; un image; un crop; un width; un height; y un sort order.

## 4 Reglas definitorias

_Todos los enunciados expresan verdades del dominio de negocio; no son procedimientos
ni imperativos. Se evita «si y solo si» en favor de «solo si» para que una necesidad
unidireccional no se confunda con una equivalencia. Una marca
**⚠︎ mecánica** señala una regla cuya redacción determinista es fiel pero
tosca — una señal para una reescritura opcional, no un defecto._

| ID | Regla declarativa |
|----|------------------|
| **DR-1 Name** | Un portrait: name es igual a display name. |

## 5 Trazabilidad al esquema

_La columna de expresión es la definición de la regla en notación Documentación —
la misma lógica que almacena el libro de reglas, escrita para un lector de negocio._

| Elemento del esquema | Tipo | Expresión |
|----------------|------|------------|
| **Portraits.Name** | fórmula | `DisplayName` |

---

> **RuleSpeak®** — la notación declarativa de reglas de negocio creada por **Ronald G. Ross**, conforme a las convenciones de **SBVR** — llegará pronto. Mientras tanto, esta página muestra la documentación habitual. [www.RonRoss.info](https://www.RonRoss.info)
