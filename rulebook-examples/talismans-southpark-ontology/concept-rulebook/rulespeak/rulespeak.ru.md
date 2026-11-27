# 📘 Talisman's South Park ontology explainer: the concept form — Документация

_This ontology in the concept form: a table of individuals, a table per class, a table or a column per property, and ERBTables and ERBFields for what the structure cannot say. orchestration/ontology_form.py wrote it; injecting it over the ontology brings the ontology's things, properties, axioms and facts in line with these tables._

> Декларативные бизнес-правила, построенные из книги правил. Каждое утверждение
> выражает истину предметной области — это не процедура и не императив.
> Формулы книги правил — единственный источник истины; этот документ — их
> прочтение на простом языке.

## 1 Бизнес-словарь

| Термин | Описание | Пояснение |
|------|-------------|-------------------|
| **Thing** | Every individual of the ontology (owl:Thing). A class's table lists the individuals that belong to it. | — |
| Label | Определённый атрибут. | _The name the page shows._ |
| Sort Order | Определённый атрибут. | _Position in the cast._ |
| Portrait | Определённый атрибут. | _The portrait drawn for this thing; set when its name matches a portrait alias._ |

<details open>
<summary>Примеры данных</summary>

| Label | Sort Order |
|---|---|
| Marvin | 1 |
| Randy | 2 |
| Sharon | 3 |

</details>

| Термин | Описание | Пояснение |
|------|-------------|-------------------|
| **Person** | The class Person: one row per individual that belongs to it. Its label and place are its ERBTables row. | — |
| Thing | Определённый атрибут. | _The individual's row in Things._ |
| Is Asserted | Истинно, когда пустая строка. | _True when a fact states it is a Person. False when only a property's domain or range implies it: such a row follows from the facts and is not added by hand._ |
| has Grade Level | Определённый атрибут. | _has grade level: the one current value. A property of the ontology; its other attributes are its ERBFields row._ |
| has Role | Определённый атрибут. | _has role: the one current value. A property of the ontology; its other attributes are its ERBFields row._ |

<details open>
<summary>Примеры данных</summary>

| Is Asserted | has Grade Level | has Role |
|---|---|---|
| true | 4 | Student |

</details>

| Термин | Описание | Пояснение |
|------|-------------|-------------------|
| **School** | The class School: one row per individual that belongs to it. Its label and place are its ERBTables row. | — |
| Thing | Определённый атрибут. | _The individual's row in Things._ |
| Is Asserted | Истинно, когда пустая строка. | _True when a fact states it is a School. False when only a property's domain or range implies it: such a row follows from the facts and is not added by hand._ |

<details open>
<summary>Примеры данных</summary>

| Is Asserted |
|---|
| false |

</details>

| Термин | Описание | Пояснение |
|------|-------------|-------------------|
| **is Parent of** | is parent of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | Определённый атрибут. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | Определённый атрибут. | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| Термин | Описание | Пояснение |
|------|-------------|-------------------|
| **is Child of** | is child of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | Определённый атрибут. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | Определённый атрибут. | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| Термин | Описание | Пояснение |
|------|-------------|-------------------|
| **is Spouse of** | is spouse of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | Определённый атрибут. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | Определённый атрибут. | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| Термин | Описание | Пояснение |
|------|-------------|-------------------|
| **is Friend of** | is friend of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | Определённый атрибут. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | Определённый атрибут. | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| Термин | Описание | Пояснение |
|------|-------------|-------------------|
| **attend** | attends: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | Определённый атрибут. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | Определённый атрибут. | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| Термин | Описание | Пояснение |
|------|-------------|-------------------|
| **has Age** | has age: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | Определённый атрибут. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Value | Определённый атрибут. | _The value._ |

<details open>
<summary>Примеры данных</summary>

| Value |
|---|
| 10 |

</details>

| Термин | Описание | Пояснение |
|------|-------------|-------------------|
| **is Grandparent of** | is grandparent of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | Определённый атрибут. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | Определённый атрибут. | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| Термин | Описание | Пояснение |
|------|-------------|-------------------|
| **Portrait** | The page's Fandom portraits, including those for characters a reader may add. | — |
| Display Name | Определённый атрибут. | _The name on the portrait's Fandom page._ |
| Page | Определённый атрибут. | _The Fandom page credited for the portrait._ |
| Image | Определённый атрибут. | _The original Fandom image._ |
| Crop | Определённый атрибут. | _The crop as an SVG viewBox: x y width height._ |
| Width | Определённый атрибут. | _The original image's width in pixels._ |
| Height | Определённый атрибут. | _The original image's height in pixels._ |
| Sort Order | Определённый атрибут. | _Position in the credits._ |
| Name | Совпадает с display name. | _Order 1. The portrait's name._ |

<details open>
<summary>Примеры данных</summary>

| Display Name | Page | Image | Crop | Width | Height | Sort Order | Name ƒ |
|---|---|---|---|---|---|---|---|
| Marvin Marsh | https://southpark.fandom.com/wiki/Marvin_Marsh | https://static.wikia.nocookie.net/southpark/images/d/db/Marsh-family-grandpa-marsh-w-purple-blanket.png/revision/latest | 48 -5 280 280 | 375 | 536 | 1 | Marvin Marsh |
| Randy Marsh | https://southpark.fandom.com/wiki/Randy_Marsh | https://static.wikia.nocookie.net/southparkfanon/images/d/d6/Randy_Marsh.png/revision/latest | 0 -10 428 428 | 428 | 1057 | 2 | Randy Marsh |
| Sharon Marsh | https://southpark.fandom.com/wiki/Sharon_Marsh | https://static.wikia.nocookie.net/southpark/images/9/99/Sharon_Marsh.png/revision/latest | 20 -10 390 390 | 439 | 1049 | 3 | Sharon Marsh |

_ƒ обозначает вычисляемый столбец._

</details>

| Термин | Описание | Пояснение |
|------|-------------|-------------------|
| **ERB Table** | One row per table of this rulebook (the platform's Schema module), and on it what the table's structure does not say. A class: its label and its place in the cast. A property that is a table: its label, its wording on the page, whether it is symmetric, and its inverse, chain and disjoint axioms. What a table IS is read from its fields: Thing and IsAsserted make a class; Subject with Object or Value make a property, whose domain and range are the tables they link to. | — |
| Table Name | Определённый атрибут. | _The table's name again, as a plain column._ |
| Description | Определённый атрибут. | _What the table holds. Follows the table._ |
| Is Meta Table | Истинно, когда пустая строка. | _True for a table that serves the rulebook itself (ERBTables, ERBFields) and not the ontology._ |
| Is Tenant Scoped | Истинно, когда пустая строка. | _The Schema module's: rows belong to one tenant. Not used by the ontology._ |
| Tenant Parent Table | Определённый атрибут. | _The Schema module's: the table tenancy is reached through. Not used by the ontology._ |
| Tenant Parent Field | Определённый атрибут. | _The Schema module's: the field that points at TenantParentTable. Not used by the ontology._ |
| Sort Order | Определённый атрибут. | _Display order. For a class: Position in the cast. For a property: Position in the Add fact form's property list._ |
| Label | Определённый атрибут. | _For a class: The name the page shows. For a property: The property's wording on the page._ |
| Portrait | Определённый атрибут. | _For a class: The portrait drawn for this thing; set when its name matches a portrait alias._ |
| Is User Addable | Истинно, когда пустая строка. | _For a property: Offered in the page's Add fact form._ |
| Is Symmetric | Истинно, когда пустая строка. | _For a property: The symmetric rule makes this link true in both directions._ |
| Value Node Label | Определённый атрибут. | _For a property: The small label under a value node for this property: 'role value' or 'number value'._ |
| Current Value Wording | Определённый атрибут. | _For a property: How the Add fact form names a replaced value, as in 'Stan now has grade level 5'._ |
| Input Name | Определённый атрибут. | _For a property: How the integer check names this property's value, as in 'for grade level'._ |
| Record Kind | Определённый атрибут. | _For a property: The word the current-record summary uses for this property's values: role, grade or school._ |
| Record Order | Определённый атрибут. | _For a property: Position of this property in the current-record summary._ |
| Inverse of | Определённый атрибут. | _The property that reads this one backwards (OWL inverseOf): when A has this link to B, the inverse rule concludes that B has that one to A. Names a property: a property's table, or a column that is a property._ |
| Chain First | Определённый атрибут. | _With ChainSecond, the property chain that concludes this property (OWL propertyChainAxiom): a ChainFirst link followed by a ChainSecond link._ |
| Chain Second | Определённый атрибут. | _The chain's second link. Set together with ChainFirst._ |
| Disjoint With | Определённый атрибут. | _Properties that never both hold between the same two things (OWL propertyDisjointWith). Several are separated by ', '._ |

<details open>
<summary>Примеры данных</summary>

| Table Name | Description | Is Meta Table | Is Tenant Scoped | Tenant Parent Field | Sort Order | Label | Is User Addable | Is Symmetric | Value Node Label | Current Value Wording | Input Name | Record Kind | Record Order | Inverse of | Chain First | Chain Second | Disjoint With |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| Things | Every individual of the ontology (owl:Thing). A class's table lists the individuals that belong to it. | false | — | — | — | — | — | — | — | — | — | — | — | — | — | — | — |
| Person | The class Person: one row per individual that belongs to it. Its label and place are its ERBTables row. | false | — | — | 8 | Person | — | — | — | — | — | — | — | — | — | — | — |
| School | The class School: one row per individual that belongs to it. Its label and place are its ERBTables row. | false | — | — | 9 | School | — | — | — | — | — | — | — | — | — | — | — |

</details>

| Термин | Описание | Пояснение |
|------|-------------|-------------------|
| **ERB Field** | One row per field of every table (the platform's Schema module), and on it the same attributes for a property that is a column: a property with one current value, on its domain's class table (on Things when it has no domain class). | — |
| ERB Table | Определённый атрибут. | _The table the field is on._ |
| Field Name | Определённый атрибут. | _The field's name. For a column that is a property: the property's id._ |
| Field Type | Определённый атрибут. | _raw or relationship. Follows the field._ |
| Datatype | Определённый атрибут. | _The rulebook datatype. Follows the field._ |
| Is Primary Key | Истинно, когда пустая строка. | _True for the field that identifies a row of its table._ |
| Description | Определённый атрибут. | _What the field means. Follows the field._ |
| Label | Определённый атрибут. | _For a property: The property's wording on the page._ |
| Is User Addable | Истинно, когда пустая строка. | _For a property: Offered in the page's Add fact form._ |
| Is Symmetric | Истинно, когда пустая строка. | _For a property: The symmetric rule makes this link true in both directions._ |
| Sort Order | Определённый атрибут. | _For a property: Position in the Add fact form's property list._ |
| Value Node Label | Определённый атрибут. | _For a property: The small label under a value node for this property: 'role value' or 'number value'._ |
| Current Value Wording | Определённый атрибут. | _For a property: How the Add fact form names a replaced value, as in 'Stan now has grade level 5'._ |
| Input Name | Определённый атрибут. | _For a property: How the integer check names this property's value, as in 'for grade level'._ |
| Record Kind | Определённый атрибут. | _For a property: The word the current-record summary uses for this property's values: role, grade or school._ |
| Record Order | Определённый атрибут. | _For a property: Position of this property in the current-record summary._ |
| Inverse of | Определённый атрибут. | _The property that reads this one backwards (OWL inverseOf): when A has this link to B, the inverse rule concludes that B has that one to A. Names a property: a property's table, or a column that is a property._ |
| Chain First | Определённый атрибут. | _With ChainSecond, the property chain that concludes this property (OWL propertyChainAxiom): a ChainFirst link followed by a ChainSecond link._ |
| Chain Second | Определённый атрибут. | _The chain's second link. Set together with ChainFirst._ |
| Disjoint With | Определённый атрибут. | _Properties that never both hold between the same two things (OWL propertyDisjointWith). Several are separated by ', '._ |

<details open>
<summary>Примеры данных</summary>

| Field Name | Field Type | Datatype | Is Primary Key | Description | Label | Is User Addable | Is Symmetric | Sort Order | Value Node Label | Current Value Wording | Input Name | Record Kind | Record Order | Inverse of | Chain First | Chain Second | Disjoint With |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| ThingId | raw | string | true | Key of the thing; also its local name in the model namespace. | — | — | — | — | — | — | — | — | — | — | — | — | — |
| Label | raw | string | false | The name the page shows. | — | — | — | — | — | — | — | — | — | — | — | — | — |
| SortOrder | raw | integer | false | Position in the cast. | — | — | — | — | — | — | — | — | — | — | — | — | — |

</details>

## 2 Типы фактов

- **thing** может ссылаться на один **portrait**
- **person** ссылается ровно на один **thing**
- **school** ссылается ровно на один **thing**
- **is parent of** ссылается ровно на один **thing**
- **is child of** ссылается ровно на один **thing**
- **is spouse of** ссылается ровно на один **thing**
- **is friend of** ссылается ровно на один **thing**
- **attend** ссылается ровно на один **person**
- **attend** ссылается ровно на один **school**
- **has age** ссылается ровно на один **thing**
- **is grandparent of** ссылается ровно на один **thing**
- **ERB table** может ссылаться на один **ERB table**
- **ERB table** может ссылаться на один **portrait**
- **ERB field** может ссылаться на один **ERB table**

## 3 Оперативные правила

_Оперативные правила формулируют, что бизнес **обязывает**, **запрещает** или
рекомендует (**следует**). Структурные правила берутся из обязательных полей и внешних
ключей; семантические — из таблицы Constraints, каждое опирается на булево значение,
которое книга правил уже вычисляет (со ссылкой DR-N в определяющих правилах ниже)._

### Структурные ограничения (из схемы)

- Thing **обязан** иметь label и sort order.
- Person **обязан** ссылаться ровно на один «thing».
- Person **обязан** фиксировать то, он является «asserted» ли.
- School **обязан** ссылаться ровно на один «thing».
- School **обязан** фиксировать то, он является «asserted» ли.
- Is parent of **обязан** ссылаться ровно на один «thing» в роли «subject».
- Is parent of **обязан** ссылаться ровно на один «thing» в роли «object».
- Is child of **обязан** ссылаться ровно на один «thing» в роли «subject».
- Is child of **обязан** ссылаться ровно на один «thing» в роли «object».
- Is spouse of **обязан** ссылаться ровно на один «thing» в роли «subject».
- Is spouse of **обязан** ссылаться ровно на один «thing» в роли «object».
- Is friend of **обязан** ссылаться ровно на один «thing» в роли «subject».
- Is friend of **обязан** ссылаться ровно на один «thing» в роли «object».
- Attend **обязан** ссылаться ровно на один «person» в роли «subject».
- Attend **обязан** ссылаться ровно на один «school» в роли «object».
- Has age **обязан** ссылаться ровно на один «thing» в роли «subject».
- Has age **обязан** иметь value.
- Is grandparent of **обязан** ссылаться ровно на один «thing» в роли «subject».
- Is grandparent of **обязан** ссылаться ровно на один «thing» в роли «object».
- Portrait **обязан** иметь display name; page; image; crop; width; height; и sort order.

## 4 Определяющие правила

_Все утверждения выражают истины предметной области; это не процедуры и не
императивы. Вместо «тогда и только тогда» используется «только если», чтобы
одностороннюю необходимость не приняли за эквивалентность. Пометка
**⚠︎ механически** указывает правило, чья детерминированная формулировка верна,
но тяжеловесна — сигнал для необязательной переформулировки, а не дефект._

| ID | Декларативное правило |
|----|------------------|
| **DR-1 Name** | Portrait: «name» совпадает с display name. |

## 5 Прослеживаемость к схеме

_Колонка выражения — определение правила в нотации Документация:
та же логика, что хранит книга правил, записанная для бизнес-читателя._

| Элемент схемы | Вид | Выражение |
|----------------|------|------------|
| **Portraits.Name** | формула | `DisplayName` |

---

> **RuleSpeak®** — декларативная нотация бизнес-правил, созданная **Рональдом Г. Россом** по соглашениям **SBVR**, — скоро появится. А пока эта страница показывает обычную документацию. [www.RonRoss.info](https://www.RonRoss.info)
