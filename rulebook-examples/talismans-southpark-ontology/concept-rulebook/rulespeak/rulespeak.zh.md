# 📘 Talisman's South Park ontology explainer: the concept form — 文档

_This ontology in the concept form: a table of individuals, a table per class, a table or a column per property, and ERBTables and ERBFields for what the structure cannot say. orchestration/ontology_form.py wrote it; injecting it over the ontology brings the ontology's things, properties, axioms and facts in line with these tables._

> 由规则手册生成的声明式业务规则。以下每个语句表达业务领域中的一个真理——
> 它既不是过程,也不是命令。规则手册中的公式是唯一的真实来源;
> 本文档是它们的通俗读法。

## 1 业务词汇

| 术语 | 描述 | 补充说明 |
|------|-------------|-------------------|
| **Thing** | Every individual of the ontology (owl:Thing). A class's table lists the individuals that belong to it. | — |
| Label | 一个已定义的属性。 | _The name the page shows._ |
| Sort Order | 一个已定义的属性。 | _Position in the cast._ |
| Portrait | 一个已定义的属性。 | _The portrait drawn for this thing; set when its name matches a portrait alias._ |

<details open>
<summary>示例数据</summary>

| Label | Sort Order |
|---|---|
| Marvin | 1 |
| Randy | 2 |
| Sharon | 3 |

</details>

| 术语 | 描述 | 补充说明 |
|------|-------------|-------------------|
| **Person** | The class Person: one row per individual that belongs to it. Its label and place are its ERBTables row. | — |
| Thing | 一个已定义的属性。 | _The individual's row in Things._ |
| Is Asserted | 当空字符串时为真。 | _True when a fact states it is a Person. False when only a property's domain or range implies it: such a row follows from the facts and is not added by hand._ |
| has Grade Level | 一个已定义的属性。 | _has grade level: the one current value. A property of the ontology; its other attributes are its ERBFields row._ |
| has Role | 一个已定义的属性。 | _has role: the one current value. A property of the ontology; its other attributes are its ERBFields row._ |

<details open>
<summary>示例数据</summary>

| Is Asserted | has Grade Level | has Role |
|---|---|---|
| true | 4 | Student |

</details>

| 术语 | 描述 | 补充说明 |
|------|-------------|-------------------|
| **School** | The class School: one row per individual that belongs to it. Its label and place are its ERBTables row. | — |
| Thing | 一个已定义的属性。 | _The individual's row in Things._ |
| Is Asserted | 当空字符串时为真。 | _True when a fact states it is a School. False when only a property's domain or range implies it: such a row follows from the facts and is not added by hand._ |

<details open>
<summary>示例数据</summary>

| Is Asserted |
|---|
| false |

</details>

| 术语 | 描述 | 补充说明 |
|------|-------------|-------------------|
| **is Parent of** | is parent of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | 一个已定义的属性。 | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | 一个已定义的属性。 | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| 术语 | 描述 | 补充说明 |
|------|-------------|-------------------|
| **is Child of** | is child of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | 一个已定义的属性。 | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | 一个已定义的属性。 | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| 术语 | 描述 | 补充说明 |
|------|-------------|-------------------|
| **is Spouse of** | is spouse of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | 一个已定义的属性。 | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | 一个已定义的属性。 | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| 术语 | 描述 | 补充说明 |
|------|-------------|-------------------|
| **is Friend of** | is friend of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | 一个已定义的属性。 | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | 一个已定义的属性。 | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| 术语 | 描述 | 补充说明 |
|------|-------------|-------------------|
| **attend** | attends: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | 一个已定义的属性。 | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | 一个已定义的属性。 | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| 术语 | 描述 | 补充说明 |
|------|-------------|-------------------|
| **has Age** | has age: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | 一个已定义的属性。 | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Value | 一个已定义的属性。 | _The value._ |

<details open>
<summary>示例数据</summary>

| Value |
|---|
| 10 |

</details>

| 术语 | 描述 | 补充说明 |
|------|-------------|-------------------|
| **is Grandparent of** | is grandparent of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | 一个已定义的属性。 | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | 一个已定义的属性。 | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| 术语 | 描述 | 补充说明 |
|------|-------------|-------------------|
| **Portrait** | The page's Fandom portraits, including those for characters a reader may add. | — |
| Display Name | 一个已定义的属性。 | _The name on the portrait's Fandom page._ |
| Page | 一个已定义的属性。 | _The Fandom page credited for the portrait._ |
| Image | 一个已定义的属性。 | _The original Fandom image._ |
| Crop | 一个已定义的属性。 | _The crop as an SVG viewBox: x y width height._ |
| Width | 一个已定义的属性。 | _The original image's width in pixels._ |
| Height | 一个已定义的属性。 | _The original image's height in pixels._ |
| Sort Order | 一个已定义的属性。 | _Position in the credits._ |
| Name | 与 display name 相同。 | _Order 1. The portrait's name._ |

<details open>
<summary>示例数据</summary>

| Display Name | Page | Image | Crop | Width | Height | Sort Order | Name ƒ |
|---|---|---|---|---|---|---|---|
| Marvin Marsh | https://southpark.fandom.com/wiki/Marvin_Marsh | https://static.wikia.nocookie.net/southpark/images/d/db/Marsh-family-grandpa-marsh-w-purple-blanket.png/revision/latest | 48 -5 280 280 | 375 | 536 | 1 | Marvin Marsh |
| Randy Marsh | https://southpark.fandom.com/wiki/Randy_Marsh | https://static.wikia.nocookie.net/southparkfanon/images/d/d6/Randy_Marsh.png/revision/latest | 0 -10 428 428 | 428 | 1057 | 2 | Randy Marsh |
| Sharon Marsh | https://southpark.fandom.com/wiki/Sharon_Marsh | https://static.wikia.nocookie.net/southpark/images/9/99/Sharon_Marsh.png/revision/latest | 20 -10 390 390 | 439 | 1049 | 3 | Sharon Marsh |

_ƒ 表示计算列。_

</details>

| 术语 | 描述 | 补充说明 |
|------|-------------|-------------------|
| **ERB Table** | One row per table of this rulebook (the platform's Schema module), and on it what the table's structure does not say. A class: its label and its place in the cast. A property that is a table: its label, its wording on the page, whether it is symmetric, and its inverse, chain and disjoint axioms. What a table IS is read from its fields: Thing and IsAsserted make a class; Subject with Object or Value make a property, whose domain and range are the tables they link to. | — |
| Table Name | 一个已定义的属性。 | _The table's name again, as a plain column._ |
| Description | 一个已定义的属性。 | _What the table holds. Follows the table._ |
| Is Meta Table | 当空字符串时为真。 | _True for a table that serves the rulebook itself (ERBTables, ERBFields) and not the ontology._ |
| Is Tenant Scoped | 当空字符串时为真。 | _The Schema module's: rows belong to one tenant. Not used by the ontology._ |
| Tenant Parent Table | 一个已定义的属性。 | _The Schema module's: the table tenancy is reached through. Not used by the ontology._ |
| Tenant Parent Field | 一个已定义的属性。 | _The Schema module's: the field that points at TenantParentTable. Not used by the ontology._ |
| Sort Order | 一个已定义的属性。 | _Display order. For a class: Position in the cast. For a property: Position in the Add fact form's property list._ |
| Label | 一个已定义的属性。 | _For a class: The name the page shows. For a property: The property's wording on the page._ |
| Portrait | 一个已定义的属性。 | _For a class: The portrait drawn for this thing; set when its name matches a portrait alias._ |
| Is User Addable | 当空字符串时为真。 | _For a property: Offered in the page's Add fact form._ |
| Is Symmetric | 当空字符串时为真。 | _For a property: The symmetric rule makes this link true in both directions._ |
| Value Node Label | 一个已定义的属性。 | _For a property: The small label under a value node for this property: 'role value' or 'number value'._ |
| Current Value Wording | 一个已定义的属性。 | _For a property: How the Add fact form names a replaced value, as in 'Stan now has grade level 5'._ |
| Input Name | 一个已定义的属性。 | _For a property: How the integer check names this property's value, as in 'for grade level'._ |
| Record Kind | 一个已定义的属性。 | _For a property: The word the current-record summary uses for this property's values: role, grade or school._ |
| Record Order | 一个已定义的属性。 | _For a property: Position of this property in the current-record summary._ |
| Inverse of | 一个已定义的属性。 | _The property that reads this one backwards (OWL inverseOf): when A has this link to B, the inverse rule concludes that B has that one to A. Names a property: a property's table, or a column that is a property._ |
| Chain First | 一个已定义的属性。 | _With ChainSecond, the property chain that concludes this property (OWL propertyChainAxiom): a ChainFirst link followed by a ChainSecond link._ |
| Chain Second | 一个已定义的属性。 | _The chain's second link. Set together with ChainFirst._ |
| Disjoint With | 一个已定义的属性。 | _Properties that never both hold between the same two things (OWL propertyDisjointWith). Several are separated by ', '._ |

<details open>
<summary>示例数据</summary>

| Table Name | Description | Is Meta Table | Is Tenant Scoped | Tenant Parent Field | Sort Order | Label | Is User Addable | Is Symmetric | Value Node Label | Current Value Wording | Input Name | Record Kind | Record Order | Inverse of | Chain First | Chain Second | Disjoint With |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| Things | Every individual of the ontology (owl:Thing). A class's table lists the individuals that belong to it. | false | — | — | — | — | — | — | — | — | — | — | — | — | — | — | — |
| Person | The class Person: one row per individual that belongs to it. Its label and place are its ERBTables row. | false | — | — | 8 | Person | — | — | — | — | — | — | — | — | — | — | — |
| School | The class School: one row per individual that belongs to it. Its label and place are its ERBTables row. | false | — | — | 9 | School | — | — | — | — | — | — | — | — | — | — | — |

</details>

| 术语 | 描述 | 补充说明 |
|------|-------------|-------------------|
| **ERB Field** | One row per field of every table (the platform's Schema module), and on it the same attributes for a property that is a column: a property with one current value, on its domain's class table (on Things when it has no domain class). | — |
| ERB Table | 一个已定义的属性。 | _The table the field is on._ |
| Field Name | 一个已定义的属性。 | _The field's name. For a column that is a property: the property's id._ |
| Field Type | 一个已定义的属性。 | _raw or relationship. Follows the field._ |
| Datatype | 一个已定义的属性。 | _The rulebook datatype. Follows the field._ |
| Is Primary Key | 当空字符串时为真。 | _True for the field that identifies a row of its table._ |
| Description | 一个已定义的属性。 | _What the field means. Follows the field._ |
| Label | 一个已定义的属性。 | _For a property: The property's wording on the page._ |
| Is User Addable | 当空字符串时为真。 | _For a property: Offered in the page's Add fact form._ |
| Is Symmetric | 当空字符串时为真。 | _For a property: The symmetric rule makes this link true in both directions._ |
| Sort Order | 一个已定义的属性。 | _For a property: Position in the Add fact form's property list._ |
| Value Node Label | 一个已定义的属性。 | _For a property: The small label under a value node for this property: 'role value' or 'number value'._ |
| Current Value Wording | 一个已定义的属性。 | _For a property: How the Add fact form names a replaced value, as in 'Stan now has grade level 5'._ |
| Input Name | 一个已定义的属性。 | _For a property: How the integer check names this property's value, as in 'for grade level'._ |
| Record Kind | 一个已定义的属性。 | _For a property: The word the current-record summary uses for this property's values: role, grade or school._ |
| Record Order | 一个已定义的属性。 | _For a property: Position of this property in the current-record summary._ |
| Inverse of | 一个已定义的属性。 | _The property that reads this one backwards (OWL inverseOf): when A has this link to B, the inverse rule concludes that B has that one to A. Names a property: a property's table, or a column that is a property._ |
| Chain First | 一个已定义的属性。 | _With ChainSecond, the property chain that concludes this property (OWL propertyChainAxiom): a ChainFirst link followed by a ChainSecond link._ |
| Chain Second | 一个已定义的属性。 | _The chain's second link. Set together with ChainFirst._ |
| Disjoint With | 一个已定义的属性。 | _Properties that never both hold between the same two things (OWL propertyDisjointWith). Several are separated by ', '._ |

<details open>
<summary>示例数据</summary>

| Field Name | Field Type | Datatype | Is Primary Key | Description | Label | Is User Addable | Is Symmetric | Sort Order | Value Node Label | Current Value Wording | Input Name | Record Kind | Record Order | Inverse of | Chain First | Chain Second | Disjoint With |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| ThingId | raw | string | true | Key of the thing; also its local name in the model namespace. | — | — | — | — | — | — | — | — | — | — | — | — | — |
| Label | raw | string | false | The name the page shows. | — | — | — | — | — | — | — | — | — | — | — | — | — |
| SortOrder | raw | integer | false | Position in the cast. | — | — | — | — | — | — | — | — | — | — | — | — | — |

</details>

## 2 事实类型

- **thing**可引用一个**portrait**
- **person**恰好引用一个**thing**
- **school**恰好引用一个**thing**
- **is parent of**恰好引用一个**thing**
- **is child of**恰好引用一个**thing**
- **is spouse of**恰好引用一个**thing**
- **is friend of**恰好引用一个**thing**
- **attend**恰好引用一个**person**
- **attend**恰好引用一个**school**
- **has age**恰好引用一个**thing**
- **is grandparent of**恰好引用一个**thing**
- **ERB table**可引用一个**ERB table**
- **ERB table**可引用一个**portrait**
- **ERB field**可引用一个**ERB table**

## 3 操作性规则

_操作性规则陈述业务**强制**、**禁止**或**建议**的内容。结构性规则来自必填字段和
外键;语义规则来自 Constraints 表,每条都依据规则手册已经计算的布尔字段
(在下方定义性规则中以 DR-N 交叉引用)。_

### 结构性约束(来自模式)

- thing**必须**具有label且sort order。
- person**必须**恰好引用一个“thing”。
- person**必须**记录其是“Asserted”与否。
- school**必须**恰好引用一个“thing”。
- school**必须**记录其是“Asserted”与否。
- is parent of**必须**恰好引用一个“thing”作为其subject。
- is parent of**必须**恰好引用一个“thing”作为其object。
- is child of**必须**恰好引用一个“thing”作为其subject。
- is child of**必须**恰好引用一个“thing”作为其object。
- is spouse of**必须**恰好引用一个“thing”作为其subject。
- is spouse of**必须**恰好引用一个“thing”作为其object。
- is friend of**必须**恰好引用一个“thing”作为其subject。
- is friend of**必须**恰好引用一个“thing”作为其object。
- attend**必须**恰好引用一个“person”作为其subject。
- attend**必须**恰好引用一个“school”作为其object。
- has age**必须**恰好引用一个“thing”作为其subject。
- has age**必须**具有value。
- is grandparent of**必须**恰好引用一个“thing”作为其subject。
- is grandparent of**必须**恰好引用一个“thing”作为其object。
- portrait**必须**具有display name、page、image、crop、width、height,且sort order。

## 4 定义性规则

_所有语句都表达业务领域中的真理;它们既不是过程,也不是命令。避免使用
"当且仅当",改用"仅当",以免把单向必要条件误当作等价关系。
**⚠︎ 机械** 标记表示某条规则的确定性措辞忠实但生硬——
这是可选改写的信号,不是缺陷。_

| ID | 声明式规则 |
|----|------------------|
| **DR-1 Name** | portrait的“name”与 display name 相同。 |

## 5 到模式的可追溯性

_表达式列是规则在 文档 记法中的定义——
与规则手册存储的逻辑相同,只是为业务读者书写。_

| 模式元素 | 类别 | 表达式 |
|----------------|------|------------|
| **Portraits.Name** | 公式 | `DisplayName` |

---

> **RuleSpeak®**——由 **Ronald G. Ross** 创立、遵循 **SBVR** 约定的声明式业务规则表示法——即将推出。在此之前，本页显示常规文档。 [www.RonRoss.info](https://www.RonRoss.info)
