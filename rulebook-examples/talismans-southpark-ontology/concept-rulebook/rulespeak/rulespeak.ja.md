# 📘 Talisman's South Park ontology explainer: the concept form — ドキュメント

_This ontology in the concept form: a table of individuals, a table per class, a table or a column per property, and ERBTables and ERBFields for what the structure cannot say. orchestration/ontology_form.py wrote it; injecting it over the ontology brings the ontology's things, properties, axioms and facts in line with these tables._

> ルールブックから生成された宣言的ビジネスルールです。以下の各文はビジネス
> ドメインにおける真実を表し、手続きでも命令でもありません。ルールブックの
> 数式が唯一の真実の源であり、本書はその平易な読み下しです。

## 1 ビジネス語彙

| 用語 | 説明 | 補足コメント |
|------|-------------|-------------------|
| **Thing** | Every individual of the ontology (owl:Thing). A class's table lists the individuals that belong to it. | — |
| Label | 定義済みの属性。 | _The name the page shows._ |
| Sort Order | 定義済みの属性。 | _Position in the cast._ |
| Portrait | 定義済みの属性。 | _The portrait drawn for this thing; set when its name matches a portrait alias._ |

<details open>
<summary>サンプルデータ</summary>

| Label | Sort Order |
|---|---|
| Marvin | 1 |
| Randy | 2 |
| Sharon | 3 |

</details>

| 用語 | 説明 | 補足コメント |
|------|-------------|-------------------|
| **Person** | The class Person: one row per individual that belongs to it. Its label and place are its ERBTables row. | — |
| Thing | 定義済みの属性。 | _The individual's row in Things._ |
| Is Asserted | 空文字列とき真。 | _True when a fact states it is a Person. False when only a property's domain or range implies it: such a row follows from the facts and is not added by hand._ |
| has Grade Level | 定義済みの属性。 | _has grade level: the one current value. A property of the ontology; its other attributes are its ERBFields row._ |
| has Role | 定義済みの属性。 | _has role: the one current value. A property of the ontology; its other attributes are its ERBFields row._ |

<details open>
<summary>サンプルデータ</summary>

| Is Asserted | has Grade Level | has Role |
|---|---|---|
| true | 4 | Student |

</details>

| 用語 | 説明 | 補足コメント |
|------|-------------|-------------------|
| **School** | The class School: one row per individual that belongs to it. Its label and place are its ERBTables row. | — |
| Thing | 定義済みの属性。 | _The individual's row in Things._ |
| Is Asserted | 空文字列とき真。 | _True when a fact states it is a School. False when only a property's domain or range implies it: such a row follows from the facts and is not added by hand._ |

<details open>
<summary>サンプルデータ</summary>

| Is Asserted |
|---|
| false |

</details>

| 用語 | 説明 | 補足コメント |
|------|-------------|-------------------|
| **is Parent of** | is parent of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | 定義済みの属性。 | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | 定義済みの属性。 | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| 用語 | 説明 | 補足コメント |
|------|-------------|-------------------|
| **is Child of** | is child of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | 定義済みの属性。 | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | 定義済みの属性。 | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| 用語 | 説明 | 補足コメント |
|------|-------------|-------------------|
| **is Spouse of** | is spouse of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | 定義済みの属性。 | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | 定義済みの属性。 | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| 用語 | 説明 | 補足コメント |
|------|-------------|-------------------|
| **is Friend of** | is friend of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | 定義済みの属性。 | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | 定義済みの属性。 | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| 用語 | 説明 | 補足コメント |
|------|-------------|-------------------|
| **attend** | attends: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | 定義済みの属性。 | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | 定義済みの属性。 | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| 用語 | 説明 | 補足コメント |
|------|-------------|-------------------|
| **has Age** | has age: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | 定義済みの属性。 | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Value | 定義済みの属性。 | _The value._ |

<details open>
<summary>サンプルデータ</summary>

| Value |
|---|
| 10 |

</details>

| 用語 | 説明 | 補足コメント |
|------|-------------|-------------------|
| **is Grandparent of** | is grandparent of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | 定義済みの属性。 | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | 定義済みの属性。 | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| 用語 | 説明 | 補足コメント |
|------|-------------|-------------------|
| **Portrait** | The page's Fandom portraits, including those for characters a reader may add. | — |
| Display Name | 定義済みの属性。 | _The name on the portrait's Fandom page._ |
| Page | 定義済みの属性。 | _The Fandom page credited for the portrait._ |
| Image | 定義済みの属性。 | _The original Fandom image._ |
| Crop | 定義済みの属性。 | _The crop as an SVG viewBox: x y width height._ |
| Width | 定義済みの属性。 | _The original image's width in pixels._ |
| Height | 定義済みの属性。 | _The original image's height in pixels._ |
| Sort Order | 定義済みの属性。 | _Position in the credits._ |
| Name | display name と同じ。 | _Order 1. The portrait's name._ |

<details open>
<summary>サンプルデータ</summary>

| Display Name | Page | Image | Crop | Width | Height | Sort Order | Name ƒ |
|---|---|---|---|---|---|---|---|
| Marvin Marsh | https://southpark.fandom.com/wiki/Marvin_Marsh | https://static.wikia.nocookie.net/southpark/images/d/db/Marsh-family-grandpa-marsh-w-purple-blanket.png/revision/latest | 48 -5 280 280 | 375 | 536 | 1 | Marvin Marsh |
| Randy Marsh | https://southpark.fandom.com/wiki/Randy_Marsh | https://static.wikia.nocookie.net/southparkfanon/images/d/d6/Randy_Marsh.png/revision/latest | 0 -10 428 428 | 428 | 1057 | 2 | Randy Marsh |
| Sharon Marsh | https://southpark.fandom.com/wiki/Sharon_Marsh | https://static.wikia.nocookie.net/southpark/images/9/99/Sharon_Marsh.png/revision/latest | 20 -10 390 390 | 439 | 1049 | 3 | Sharon Marsh |

_ƒ は計算列を示します。_

</details>

| 用語 | 説明 | 補足コメント |
|------|-------------|-------------------|
| **ERB Table** | One row per table of this rulebook (the platform's Schema module), and on it what the table's structure does not say. A class: its label and its place in the cast. A property that is a table: its label, its wording on the page, whether it is symmetric, and its inverse, chain and disjoint axioms. What a table IS is read from its fields: Thing and IsAsserted make a class; Subject with Object or Value make a property, whose domain and range are the tables they link to. | — |
| Table Name | 定義済みの属性。 | _The table's name again, as a plain column._ |
| Description | 定義済みの属性。 | _What the table holds. Follows the table._ |
| Is Meta Table | 空文字列とき真。 | _True for a table that serves the rulebook itself (ERBTables, ERBFields) and not the ontology._ |
| Is Tenant Scoped | 空文字列とき真。 | _The Schema module's: rows belong to one tenant. Not used by the ontology._ |
| Tenant Parent Table | 定義済みの属性。 | _The Schema module's: the table tenancy is reached through. Not used by the ontology._ |
| Tenant Parent Field | 定義済みの属性。 | _The Schema module's: the field that points at TenantParentTable. Not used by the ontology._ |
| Sort Order | 定義済みの属性。 | _Display order. For a class: Position in the cast. For a property: Position in the Add fact form's property list._ |
| Label | 定義済みの属性。 | _For a class: The name the page shows. For a property: The property's wording on the page._ |
| Portrait | 定義済みの属性。 | _For a class: The portrait drawn for this thing; set when its name matches a portrait alias._ |
| Is User Addable | 空文字列とき真。 | _For a property: Offered in the page's Add fact form._ |
| Is Symmetric | 空文字列とき真。 | _For a property: The symmetric rule makes this link true in both directions._ |
| Value Node Label | 定義済みの属性。 | _For a property: The small label under a value node for this property: 'role value' or 'number value'._ |
| Current Value Wording | 定義済みの属性。 | _For a property: How the Add fact form names a replaced value, as in 'Stan now has grade level 5'._ |
| Input Name | 定義済みの属性。 | _For a property: How the integer check names this property's value, as in 'for grade level'._ |
| Record Kind | 定義済みの属性。 | _For a property: The word the current-record summary uses for this property's values: role, grade or school._ |
| Record Order | 定義済みの属性。 | _For a property: Position of this property in the current-record summary._ |
| Inverse of | 定義済みの属性。 | _The property that reads this one backwards (OWL inverseOf): when A has this link to B, the inverse rule concludes that B has that one to A. Names a property: a property's table, or a column that is a property._ |
| Chain First | 定義済みの属性。 | _With ChainSecond, the property chain that concludes this property (OWL propertyChainAxiom): a ChainFirst link followed by a ChainSecond link._ |
| Chain Second | 定義済みの属性。 | _The chain's second link. Set together with ChainFirst._ |
| Disjoint With | 定義済みの属性。 | _Properties that never both hold between the same two things (OWL propertyDisjointWith). Several are separated by ', '._ |

<details open>
<summary>サンプルデータ</summary>

| Table Name | Description | Is Meta Table | Is Tenant Scoped | Tenant Parent Field | Sort Order | Label | Is User Addable | Is Symmetric | Value Node Label | Current Value Wording | Input Name | Record Kind | Record Order | Inverse of | Chain First | Chain Second | Disjoint With |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| Things | Every individual of the ontology (owl:Thing). A class's table lists the individuals that belong to it. | false | — | — | — | — | — | — | — | — | — | — | — | — | — | — | — |
| Person | The class Person: one row per individual that belongs to it. Its label and place are its ERBTables row. | false | — | — | 8 | Person | — | — | — | — | — | — | — | — | — | — | — |
| School | The class School: one row per individual that belongs to it. Its label and place are its ERBTables row. | false | — | — | 9 | School | — | — | — | — | — | — | — | — | — | — | — |

</details>

| 用語 | 説明 | 補足コメント |
|------|-------------|-------------------|
| **ERB Field** | One row per field of every table (the platform's Schema module), and on it the same attributes for a property that is a column: a property with one current value, on its domain's class table (on Things when it has no domain class). | — |
| ERB Table | 定義済みの属性。 | _The table the field is on._ |
| Field Name | 定義済みの属性。 | _The field's name. For a column that is a property: the property's id._ |
| Field Type | 定義済みの属性。 | _raw or relationship. Follows the field._ |
| Datatype | 定義済みの属性。 | _The rulebook datatype. Follows the field._ |
| Is Primary Key | 空文字列とき真。 | _True for the field that identifies a row of its table._ |
| Description | 定義済みの属性。 | _What the field means. Follows the field._ |
| Label | 定義済みの属性。 | _For a property: The property's wording on the page._ |
| Is User Addable | 空文字列とき真。 | _For a property: Offered in the page's Add fact form._ |
| Is Symmetric | 空文字列とき真。 | _For a property: The symmetric rule makes this link true in both directions._ |
| Sort Order | 定義済みの属性。 | _For a property: Position in the Add fact form's property list._ |
| Value Node Label | 定義済みの属性。 | _For a property: The small label under a value node for this property: 'role value' or 'number value'._ |
| Current Value Wording | 定義済みの属性。 | _For a property: How the Add fact form names a replaced value, as in 'Stan now has grade level 5'._ |
| Input Name | 定義済みの属性。 | _For a property: How the integer check names this property's value, as in 'for grade level'._ |
| Record Kind | 定義済みの属性。 | _For a property: The word the current-record summary uses for this property's values: role, grade or school._ |
| Record Order | 定義済みの属性。 | _For a property: Position of this property in the current-record summary._ |
| Inverse of | 定義済みの属性。 | _The property that reads this one backwards (OWL inverseOf): when A has this link to B, the inverse rule concludes that B has that one to A. Names a property: a property's table, or a column that is a property._ |
| Chain First | 定義済みの属性。 | _With ChainSecond, the property chain that concludes this property (OWL propertyChainAxiom): a ChainFirst link followed by a ChainSecond link._ |
| Chain Second | 定義済みの属性。 | _The chain's second link. Set together with ChainFirst._ |
| Disjoint With | 定義済みの属性。 | _Properties that never both hold between the same two things (OWL propertyDisjointWith). Several are separated by ', '._ |

<details open>
<summary>サンプルデータ</summary>

| Field Name | Field Type | Datatype | Is Primary Key | Description | Label | Is User Addable | Is Symmetric | Sort Order | Value Node Label | Current Value Wording | Input Name | Record Kind | Record Order | Inverse of | Chain First | Chain Second | Disjoint With |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| ThingId | raw | string | true | Key of the thing; also its local name in the model namespace. | — | — | — | — | — | — | — | — | — | — | — | — | — |
| Label | raw | string | false | The name the page shows. | — | — | — | — | — | — | — | — | — | — | — | — | — |
| SortOrder | raw | integer | false | Position in the cast. | — | — | — | — | — | — | — | — | — | — | — | — | — |

</details>

## 2 ファクトタイプ

- **thing**は**portrait**を 1 件参照できます
- **person**は**thing**をちょうど 1 件参照します
- **school**は**thing**をちょうど 1 件参照します
- **is parent of**は**thing**をちょうど 1 件参照します
- **is child of**は**thing**をちょうど 1 件参照します
- **is spouse of**は**thing**をちょうど 1 件参照します
- **is friend of**は**thing**をちょうど 1 件参照します
- **attend**は**person**をちょうど 1 件参照します
- **attend**は**school**をちょうど 1 件参照します
- **has age**は**thing**をちょうど 1 件参照します
- **is grandparent of**は**thing**をちょうど 1 件参照します
- **ERB table**は**ERB table**を 1 件参照できます
- **ERB table**は**portrait**を 1 件参照できます
- **ERB field**は**ERB table**を 1 件参照できます

## 3 運用ルール

_運用ルールは、ビジネスが**義務付ける**こと、**禁止する**こと、
**推奨する**ことを述べます。構造ルールは必須フィールドと外部キーから、
意味ルールは Constraints テーブルから得られ、いずれもルールブックが既に計算する
ブール値に基づきます(下の定義ルールで DR-N として相互参照)。_

### 構造的制約(スキーマ由来)

- thingは labelおよびsort order を持た**なければなりません**。
- personは「thing」をちょうど 1 件参照し**なければなりません**。
- personは 「Asserted」であるかどうかを記録し**なければなりません**。
- schoolは「thing」をちょうど 1 件参照し**なければなりません**。
- schoolは 「Asserted」であるかどうかを記録し**なければなりません**。
- is parent ofは、subjectとして「thing」をちょうど 1 件参照し**なければなりません**。
- is parent ofは、objectとして「thing」をちょうど 1 件参照し**なければなりません**。
- is child ofは、subjectとして「thing」をちょうど 1 件参照し**なければなりません**。
- is child ofは、objectとして「thing」をちょうど 1 件参照し**なければなりません**。
- is spouse ofは、subjectとして「thing」をちょうど 1 件参照し**なければなりません**。
- is spouse ofは、objectとして「thing」をちょうど 1 件参照し**なければなりません**。
- is friend ofは、subjectとして「thing」をちょうど 1 件参照し**なければなりません**。
- is friend ofは、objectとして「thing」をちょうど 1 件参照し**なければなりません**。
- attendは、subjectとして「person」をちょうど 1 件参照し**なければなりません**。
- attendは、objectとして「school」をちょうど 1 件参照し**なければなりません**。
- has ageは、subjectとして「thing」をちょうど 1 件参照し**なければなりません**。
- has ageは value を持た**なければなりません**。
- is grandparent ofは、subjectとして「thing」をちょうど 1 件参照し**なければなりません**。
- is grandparent ofは、objectとして「thing」をちょうど 1 件参照し**なければなりません**。
- portraitは display name、page、image、crop、width、height、およびsort order を持た**なければなりません**。

## 4 定義ルール

_すべての文はビジネスドメインの真実を表し、手続きでも命令でもありません。
一方向の必要条件を同値と取り違えないよう、「〜の場合に限り」を用います。
**⚠︎ 機械的** マークは、決定的な文言が忠実ながらぎこちないルールを示します —
任意の書き直しの合図であり、欠陥ではありません。_

| ID | 宣言的ルール |
|----|------------------|
| **DR-1 Name** | portraitの「name」は display name と同じです。 |

## 5 スキーマへのトレーサビリティ

_式の列は ドキュメント 記法によるルールの定義です —
ルールブックが保持するのと同じロジックを、ビジネス読者向けに記したものです。_

| スキーマ要素 | 種別 | 式 |
|----------------|------|------------|
| **Portraits.Name** | 数式 | `DisplayName` |

---

> **RuleSpeak®** — **Ronald G. Ross** が考案し、**SBVR** の慣例に従う宣言的なビジネスルール表記 — は近日対応予定です。それまでは、このページに通常のドキュメントを表示します。 [www.RonRoss.info](https://www.RonRoss.info)
