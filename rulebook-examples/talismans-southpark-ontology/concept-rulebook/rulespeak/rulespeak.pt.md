# 📘 Talisman's South Park ontology explainer: the concept form — Documentação

_This ontology in the concept form: a table of individuals, a table per class, a table or a column per property, and ERBTables and ERBFields for what the structure cannot say. orchestration/ontology_form.py wrote it; injecting it over the ontology brings the ontology's things, properties, axioms and facts in line with these tables._

> Regras de negócio declarativas geradas a partir do livro de regras. Cada
> enunciado expressa uma verdade do domínio de negócio — não é um procedimento
> nem um imperativo. As fórmulas do livro de regras são a única fonte da verdade;
> este documento é a sua leitura em linguagem clara.

## 1 Vocabulário de negócio

| Termo | Descrição | Comentário narrativo |
|------|-------------|-------------------|
| **Thing** | Every individual of the ontology (owl:Thing). A class's table lists the individuals that belong to it. | — |
| Label | Um atributo definido. | _The name the page shows._ |
| Sort Order | Um atributo definido. | _Position in the cast._ |
| Portrait | Um atributo definido. | _The portrait drawn for this thing; set when its name matches a portrait alias._ |

<details open>
<summary>Dados de exemplo</summary>

| Label | Sort Order |
|---|---|
| Marvin | 1 |
| Randy | 2 |
| Sharon | 3 |

</details>

| Termo | Descrição | Comentário narrativo |
|------|-------------|-------------------|
| **Person** | The class Person: one row per individual that belongs to it. Its label and place are its ERBTables row. | — |
| Thing | Um atributo definido. | _The individual's row in Things._ |
| Is Asserted | Verdadeiro quando uma cadeia vazia. | _True when a fact states it is a Person. False when only a property's domain or range implies it: such a row follows from the facts and is not added by hand._ |
| has Grade Level | Um atributo definido. | _has grade level: the one current value. A property of the ontology; its other attributes are its ERBFields row._ |
| has Role | Um atributo definido. | _has role: the one current value. A property of the ontology; its other attributes are its ERBFields row._ |

<details open>
<summary>Dados de exemplo</summary>

| Is Asserted | has Grade Level | has Role |
|---|---|---|
| true | 4 | Student |

</details>

| Termo | Descrição | Comentário narrativo |
|------|-------------|-------------------|
| **School** | The class School: one row per individual that belongs to it. Its label and place are its ERBTables row. | — |
| Thing | Um atributo definido. | _The individual's row in Things._ |
| Is Asserted | Verdadeiro quando uma cadeia vazia. | _True when a fact states it is a School. False when only a property's domain or range implies it: such a row follows from the facts and is not added by hand._ |

<details open>
<summary>Dados de exemplo</summary>

| Is Asserted |
|---|
| false |

</details>

| Termo | Descrição | Comentário narrativo |
|------|-------------|-------------------|
| **is Parent of** | is parent of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | Um atributo definido. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | Um atributo definido. | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| Termo | Descrição | Comentário narrativo |
|------|-------------|-------------------|
| **is Child of** | is child of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | Um atributo definido. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | Um atributo definido. | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| Termo | Descrição | Comentário narrativo |
|------|-------------|-------------------|
| **is Spouse of** | is spouse of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | Um atributo definido. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | Um atributo definido. | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| Termo | Descrição | Comentário narrativo |
|------|-------------|-------------------|
| **is Friend of** | is friend of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | Um atributo definido. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | Um atributo definido. | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| Termo | Descrição | Comentário narrativo |
|------|-------------|-------------------|
| **attend** | attends: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | Um atributo definido. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | Um atributo definido. | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| Termo | Descrição | Comentário narrativo |
|------|-------------|-------------------|
| **has Age** | has age: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | Um atributo definido. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Value | Um atributo definido. | _The value._ |

<details open>
<summary>Dados de exemplo</summary>

| Value |
|---|
| 10 |

</details>

| Termo | Descrição | Comentário narrativo |
|------|-------------|-------------------|
| **is Grandparent of** | is grandparent of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | Um atributo definido. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | Um atributo definido. | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| Termo | Descrição | Comentário narrativo |
|------|-------------|-------------------|
| **Portrait** | The page's Fandom portraits, including those for characters a reader may add. | — |
| Display Name | Um atributo definido. | _The name on the portrait's Fandom page._ |
| Page | Um atributo definido. | _The Fandom page credited for the portrait._ |
| Image | Um atributo definido. | _The original Fandom image._ |
| Crop | Um atributo definido. | _The crop as an SVG viewBox: x y width height._ |
| Width | Um atributo definido. | _The original image's width in pixels._ |
| Height | Um atributo definido. | _The original image's height in pixels._ |
| Sort Order | Um atributo definido. | _Position in the credits._ |
| Name | Igual a display name. | _Order 1. The portrait's name._ |

<details open>
<summary>Dados de exemplo</summary>

| Display Name | Page | Image | Crop | Width | Height | Sort Order | Name ƒ |
|---|---|---|---|---|---|---|---|
| Marvin Marsh | https://southpark.fandom.com/wiki/Marvin_Marsh | https://static.wikia.nocookie.net/southpark/images/d/db/Marsh-family-grandpa-marsh-w-purple-blanket.png/revision/latest | 48 -5 280 280 | 375 | 536 | 1 | Marvin Marsh |
| Randy Marsh | https://southpark.fandom.com/wiki/Randy_Marsh | https://static.wikia.nocookie.net/southparkfanon/images/d/d6/Randy_Marsh.png/revision/latest | 0 -10 428 428 | 428 | 1057 | 2 | Randy Marsh |
| Sharon Marsh | https://southpark.fandom.com/wiki/Sharon_Marsh | https://static.wikia.nocookie.net/southpark/images/9/99/Sharon_Marsh.png/revision/latest | 20 -10 390 390 | 439 | 1049 | 3 | Sharon Marsh |

_ƒ marca uma coluna calculada._

</details>

| Termo | Descrição | Comentário narrativo |
|------|-------------|-------------------|
| **ERB Table** | One row per table of this rulebook (the platform's Schema module), and on it what the table's structure does not say. A class: its label and its place in the cast. A property that is a table: its label, its wording on the page, whether it is symmetric, and its inverse, chain and disjoint axioms. What a table IS is read from its fields: Thing and IsAsserted make a class; Subject with Object or Value make a property, whose domain and range are the tables they link to. | — |
| Table Name | Um atributo definido. | _The table's name again, as a plain column._ |
| Description | Um atributo definido. | _What the table holds. Follows the table._ |
| Is Meta Table | Verdadeiro quando uma cadeia vazia. | _True for a table that serves the rulebook itself (ERBTables, ERBFields) and not the ontology._ |
| Is Tenant Scoped | Verdadeiro quando uma cadeia vazia. | _The Schema module's: rows belong to one tenant. Not used by the ontology._ |
| Tenant Parent Table | Um atributo definido. | _The Schema module's: the table tenancy is reached through. Not used by the ontology._ |
| Tenant Parent Field | Um atributo definido. | _The Schema module's: the field that points at TenantParentTable. Not used by the ontology._ |
| Sort Order | Um atributo definido. | _Display order. For a class: Position in the cast. For a property: Position in the Add fact form's property list._ |
| Label | Um atributo definido. | _For a class: The name the page shows. For a property: The property's wording on the page._ |
| Portrait | Um atributo definido. | _For a class: The portrait drawn for this thing; set when its name matches a portrait alias._ |
| Is User Addable | Verdadeiro quando uma cadeia vazia. | _For a property: Offered in the page's Add fact form._ |
| Is Symmetric | Verdadeiro quando uma cadeia vazia. | _For a property: The symmetric rule makes this link true in both directions._ |
| Value Node Label | Um atributo definido. | _For a property: The small label under a value node for this property: 'role value' or 'number value'._ |
| Current Value Wording | Um atributo definido. | _For a property: How the Add fact form names a replaced value, as in 'Stan now has grade level 5'._ |
| Input Name | Um atributo definido. | _For a property: How the integer check names this property's value, as in 'for grade level'._ |
| Record Kind | Um atributo definido. | _For a property: The word the current-record summary uses for this property's values: role, grade or school._ |
| Record Order | Um atributo definido. | _For a property: Position of this property in the current-record summary._ |
| Inverse of | Um atributo definido. | _The property that reads this one backwards (OWL inverseOf): when A has this link to B, the inverse rule concludes that B has that one to A. Names a property: a property's table, or a column that is a property._ |
| Chain First | Um atributo definido. | _With ChainSecond, the property chain that concludes this property (OWL propertyChainAxiom): a ChainFirst link followed by a ChainSecond link._ |
| Chain Second | Um atributo definido. | _The chain's second link. Set together with ChainFirst._ |
| Disjoint With | Um atributo definido. | _Properties that never both hold between the same two things (OWL propertyDisjointWith). Several are separated by ', '._ |

<details open>
<summary>Dados de exemplo</summary>

| Table Name | Description | Is Meta Table | Is Tenant Scoped | Tenant Parent Field | Sort Order | Label | Is User Addable | Is Symmetric | Value Node Label | Current Value Wording | Input Name | Record Kind | Record Order | Inverse of | Chain First | Chain Second | Disjoint With |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| Things | Every individual of the ontology (owl:Thing). A class's table lists the individuals that belong to it. | false | — | — | — | — | — | — | — | — | — | — | — | — | — | — | — |
| Person | The class Person: one row per individual that belongs to it. Its label and place are its ERBTables row. | false | — | — | 8 | Person | — | — | — | — | — | — | — | — | — | — | — |
| School | The class School: one row per individual that belongs to it. Its label and place are its ERBTables row. | false | — | — | 9 | School | — | — | — | — | — | — | — | — | — | — | — |

</details>

| Termo | Descrição | Comentário narrativo |
|------|-------------|-------------------|
| **ERB Field** | One row per field of every table (the platform's Schema module), and on it the same attributes for a property that is a column: a property with one current value, on its domain's class table (on Things when it has no domain class). | — |
| ERB Table | Um atributo definido. | _The table the field is on._ |
| Field Name | Um atributo definido. | _The field's name. For a column that is a property: the property's id._ |
| Field Type | Um atributo definido. | _raw or relationship. Follows the field._ |
| Datatype | Um atributo definido. | _The rulebook datatype. Follows the field._ |
| Is Primary Key | Verdadeiro quando uma cadeia vazia. | _True for the field that identifies a row of its table._ |
| Description | Um atributo definido. | _What the field means. Follows the field._ |
| Label | Um atributo definido. | _For a property: The property's wording on the page._ |
| Is User Addable | Verdadeiro quando uma cadeia vazia. | _For a property: Offered in the page's Add fact form._ |
| Is Symmetric | Verdadeiro quando uma cadeia vazia. | _For a property: The symmetric rule makes this link true in both directions._ |
| Sort Order | Um atributo definido. | _For a property: Position in the Add fact form's property list._ |
| Value Node Label | Um atributo definido. | _For a property: The small label under a value node for this property: 'role value' or 'number value'._ |
| Current Value Wording | Um atributo definido. | _For a property: How the Add fact form names a replaced value, as in 'Stan now has grade level 5'._ |
| Input Name | Um atributo definido. | _For a property: How the integer check names this property's value, as in 'for grade level'._ |
| Record Kind | Um atributo definido. | _For a property: The word the current-record summary uses for this property's values: role, grade or school._ |
| Record Order | Um atributo definido. | _For a property: Position of this property in the current-record summary._ |
| Inverse of | Um atributo definido. | _The property that reads this one backwards (OWL inverseOf): when A has this link to B, the inverse rule concludes that B has that one to A. Names a property: a property's table, or a column that is a property._ |
| Chain First | Um atributo definido. | _With ChainSecond, the property chain that concludes this property (OWL propertyChainAxiom): a ChainFirst link followed by a ChainSecond link._ |
| Chain Second | Um atributo definido. | _The chain's second link. Set together with ChainFirst._ |
| Disjoint With | Um atributo definido. | _Properties that never both hold between the same two things (OWL propertyDisjointWith). Several are separated by ', '._ |

<details open>
<summary>Dados de exemplo</summary>

| Field Name | Field Type | Datatype | Is Primary Key | Description | Label | Is User Addable | Is Symmetric | Sort Order | Value Node Label | Current Value Wording | Input Name | Record Kind | Record Order | Inverse of | Chain First | Chain Second | Disjoint With |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| ThingId | raw | string | true | Key of the thing; also its local name in the model namespace. | — | — | — | — | — | — | — | — | — | — | — | — | — |
| Label | raw | string | false | The name the page shows. | — | — | — | — | — | — | — | — | — | — | — | — | — |
| SortOrder | raw | integer | false | Position in the cast. | — | — | — | — | — | — | — | — | — | — | — | — | — |

</details>

## 2 Tipos de fatos

- um **thing** pode referenciar um **portrait**
- um **person** referencia exatamente um **thing**
- um **school** referencia exatamente um **thing**
- um **is parent of** referencia exatamente um **thing**
- um **is child of** referencia exatamente um **thing**
- um **is spouse of** referencia exatamente um **thing**
- um **is friend of** referencia exatamente um **thing**
- um **attend** referencia exatamente um **person**
- um **attend** referencia exatamente um **school**
- um **has age** referencia exatamente um **thing**
- um **is grandparent of** referencia exatamente um **thing**
- um **ERB table** pode referenciar um **ERB table**
- um **ERB table** pode referenciar um **portrait**
- um **ERB field** pode referenciar um **ERB table**

## 3 Regras operativas

_As regras operativas enunciam o que o negócio **obriga**, **proíbe** ou
aconselha (**deveria**). As regras estruturais vêm de campos obrigatórios e chaves
estrangeiras; as semânticas vêm da tabela Constraints, cada uma baseada num booleano
que o livro de regras já calcula (referenciado como DR-N nas regras definidoras)._

### Restrições estruturais (do esquema)

- Um thing **deve** ter um label e um sort order.
- Um person **deve** referenciar exatamente um thing.
- Um person **deve** registrar se é asserted.
- Um school **deve** referenciar exatamente um thing.
- Um school **deve** registrar se é asserted.
- Um is parent of **deve** referenciar exatamente um thing como seu subject.
- Um is parent of **deve** referenciar exatamente um thing como seu object.
- Um is child of **deve** referenciar exatamente um thing como seu subject.
- Um is child of **deve** referenciar exatamente um thing como seu object.
- Um is spouse of **deve** referenciar exatamente um thing como seu subject.
- Um is spouse of **deve** referenciar exatamente um thing como seu object.
- Um is friend of **deve** referenciar exatamente um thing como seu subject.
- Um is friend of **deve** referenciar exatamente um thing como seu object.
- Um attend **deve** referenciar exatamente um person como seu subject.
- Um attend **deve** referenciar exatamente um school como seu object.
- Um has age **deve** referenciar exatamente um thing como seu subject.
- Um has age **deve** ter um value.
- Um is grandparent of **deve** referenciar exatamente um thing como seu subject.
- Um is grandparent of **deve** referenciar exatamente um thing como seu object.
- Um portrait **deve** ter um display name; um page; um image; um crop; um width; um height; e um sort order.

## 4 Regras definidoras

_Todos os enunciados expressam verdades do domínio de negócio; não são procedimentos
nem imperativos. Evita-se «se e somente se» em favor de «somente se», para que uma
necessidade unidirecional não seja confundida com uma equivalência. Uma marca
**⚠︎ mecânica** sinaliza uma regra cuja redação determinista é fiel mas
dura — um sinal para uma reescrita opcional, não um defeito._

| ID | Regra declarativa |
|----|------------------|
| **DR-1 Name** | Um portrait: name é igual a display name. |

## 5 Rastreabilidade ao esquema

_A coluna de expressão é a definição da regra em notação Documentação —
a mesma lógica que o livro de regras armazena, escrita para um leitor de negócio._

| Elemento do esquema | Tipo | Expressão |
|----------------|------|------------|
| **Portraits.Name** | fórmula | `DisplayName` |

---

> **RuleSpeak®** — a notação declarativa de regras de negócio criada por **Ronald G. Ross**, seguindo as convenções do **SBVR** — chegará em breve. Até lá, esta página mostra a documentação comum. [www.RonRoss.info](https://www.RonRoss.info)
