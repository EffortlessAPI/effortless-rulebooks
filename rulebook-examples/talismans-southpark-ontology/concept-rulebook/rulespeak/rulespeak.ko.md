# 📘 Talisman's South Park ontology explainer: the concept form — 문서

_This ontology in the concept form: a table of individuals, a table per class, a table or a column per property, and ERBTables and ERBFields for what the structure cannot say. orchestration/ontology_form.py wrote it; injecting it over the ontology brings the ontology's things, properties, axioms and facts in line with these tables._

> 룰북에서 생성한 선언적 비즈니스 규칙입니다. 아래의 모든 문장은 비즈니스 영역에서
> 성립하는 사실을 서술하며, 절차나 명령이 아닙니다. 룰북의 수식이 단일 진실 공급원이고,
> 이 문서는 그 수식을 평이한 문장으로 풀어 쓴 것입니다.

## 1 비즈니스 용어

| 용어 | 설명 | 부연 설명 |
|------|-------------|-------------------|
| **Thing** | Every individual of the ontology (owl:Thing). A class's table lists the individuals that belong to it. | — |
| Label | 정의된 속성입니다. | _The name the page shows._ |
| Sort Order | 정의된 속성입니다. | _Position in the cast._ |
| Portrait | 정의된 속성입니다. | _The portrait drawn for this thing; set when its name matches a portrait alias._ |

<details open>
<summary>예시 데이터</summary>

| Label | Sort Order |
|---|---|
| Marvin | 1 |
| Randy | 2 |
| Sharon | 3 |

</details>

| 용어 | 설명 | 부연 설명 |
|------|-------------|-------------------|
| **Person** | The class Person: one row per individual that belongs to it. Its label and place are its ERBTables row. | — |
| Thing | 정의된 속성입니다. | _The individual's row in Things._ |
| Is Asserted | 빈 문자열에 참입니다. | _True when a fact states it is a Person. False when only a property's domain or range implies it: such a row follows from the facts and is not added by hand._ |
| has Grade Level | 정의된 속성입니다. | _has grade level: the one current value. A property of the ontology; its other attributes are its ERBFields row._ |
| has Role | 정의된 속성입니다. | _has role: the one current value. A property of the ontology; its other attributes are its ERBFields row._ |

<details open>
<summary>예시 데이터</summary>

| Is Asserted | has Grade Level | has Role |
|---|---|---|
| true | 4 | Student |

</details>

| 용어 | 설명 | 부연 설명 |
|------|-------------|-------------------|
| **School** | The class School: one row per individual that belongs to it. Its label and place are its ERBTables row. | — |
| Thing | 정의된 속성입니다. | _The individual's row in Things._ |
| Is Asserted | 빈 문자열에 참입니다. | _True when a fact states it is a School. False when only a property's domain or range implies it: such a row follows from the facts and is not added by hand._ |

<details open>
<summary>예시 데이터</summary>

| Is Asserted |
|---|
| false |

</details>

| 용어 | 설명 | 부연 설명 |
|------|-------------|-------------------|
| **is Parent of** | is parent of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | 정의된 속성입니다. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | 정의된 속성입니다. | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| 용어 | 설명 | 부연 설명 |
|------|-------------|-------------------|
| **is Child of** | is child of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | 정의된 속성입니다. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | 정의된 속성입니다. | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| 용어 | 설명 | 부연 설명 |
|------|-------------|-------------------|
| **is Spouse of** | is spouse of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | 정의된 속성입니다. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | 정의된 속성입니다. | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| 용어 | 설명 | 부연 설명 |
|------|-------------|-------------------|
| **is Friend of** | is friend of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | 정의된 속성입니다. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | 정의된 속성입니다. | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| 용어 | 설명 | 부연 설명 |
|------|-------------|-------------------|
| **attend** | attends: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | 정의된 속성입니다. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | 정의된 속성입니다. | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| 용어 | 설명 | 부연 설명 |
|------|-------------|-------------------|
| **has Age** | has age: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | 정의된 속성입니다. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Value | 정의된 속성입니다. | _The value._ |

<details open>
<summary>예시 데이터</summary>

| Value |
|---|
| 10 |

</details>

| 용어 | 설명 | 부연 설명 |
|------|-------------|-------------------|
| **is Grandparent of** | is grandparent of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | 정의된 속성입니다. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | 정의된 속성입니다. | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| 용어 | 설명 | 부연 설명 |
|------|-------------|-------------------|
| **Portrait** | The page's Fandom portraits, including those for characters a reader may add. | — |
| Display Name | 정의된 속성입니다. | _The name on the portrait's Fandom page._ |
| Page | 정의된 속성입니다. | _The Fandom page credited for the portrait._ |
| Image | 정의된 속성입니다. | _The original Fandom image._ |
| Crop | 정의된 속성입니다. | _The crop as an SVG viewBox: x y width height._ |
| Width | 정의된 속성입니다. | _The original image's width in pixels._ |
| Height | 정의된 속성입니다. | _The original image's height in pixels._ |
| Sort Order | 정의된 속성입니다. | _Position in the credits._ |
| Name | 같은 레코드의 ‘display name’ 값과 같습니다. | _Order 1. The portrait's name._ |

<details open>
<summary>예시 데이터</summary>

| Display Name | Page | Image | Crop | Width | Height | Sort Order | Name ƒ |
|---|---|---|---|---|---|---|---|
| Marvin Marsh | https://southpark.fandom.com/wiki/Marvin_Marsh | https://static.wikia.nocookie.net/southpark/images/d/db/Marsh-family-grandpa-marsh-w-purple-blanket.png/revision/latest | 48 -5 280 280 | 375 | 536 | 1 | Marvin Marsh |
| Randy Marsh | https://southpark.fandom.com/wiki/Randy_Marsh | https://static.wikia.nocookie.net/southparkfanon/images/d/d6/Randy_Marsh.png/revision/latest | 0 -10 428 428 | 428 | 1057 | 2 | Randy Marsh |
| Sharon Marsh | https://southpark.fandom.com/wiki/Sharon_Marsh | https://static.wikia.nocookie.net/southpark/images/9/99/Sharon_Marsh.png/revision/latest | 20 -10 390 390 | 439 | 1049 | 3 | Sharon Marsh |

_ƒ 표시는 계산된 열을 나타냅니다._

</details>

| 용어 | 설명 | 부연 설명 |
|------|-------------|-------------------|
| **ERB Table** | One row per table of this rulebook (the platform's Schema module), and on it what the table's structure does not say. A class: its label and its place in the cast. A property that is a table: its label, its wording on the page, whether it is symmetric, and its inverse, chain and disjoint axioms. What a table IS is read from its fields: Thing and IsAsserted make a class; Subject with Object or Value make a property, whose domain and range are the tables they link to. | — |
| Table Name | 정의된 속성입니다. | _The table's name again, as a plain column._ |
| Description | 정의된 속성입니다. | _What the table holds. Follows the table._ |
| Is Meta Table | 빈 문자열에 참입니다. | _True for a table that serves the rulebook itself (ERBTables, ERBFields) and not the ontology._ |
| Is Tenant Scoped | 빈 문자열에 참입니다. | _The Schema module's: rows belong to one tenant. Not used by the ontology._ |
| Tenant Parent Table | 정의된 속성입니다. | _The Schema module's: the table tenancy is reached through. Not used by the ontology._ |
| Tenant Parent Field | 정의된 속성입니다. | _The Schema module's: the field that points at TenantParentTable. Not used by the ontology._ |
| Sort Order | 정의된 속성입니다. | _Display order. For a class: Position in the cast. For a property: Position in the Add fact form's property list._ |
| Label | 정의된 속성입니다. | _For a class: The name the page shows. For a property: The property's wording on the page._ |
| Portrait | 정의된 속성입니다. | _For a class: The portrait drawn for this thing; set when its name matches a portrait alias._ |
| Is User Addable | 빈 문자열에 참입니다. | _For a property: Offered in the page's Add fact form._ |
| Is Symmetric | 빈 문자열에 참입니다. | _For a property: The symmetric rule makes this link true in both directions._ |
| Value Node Label | 정의된 속성입니다. | _For a property: The small label under a value node for this property: 'role value' or 'number value'._ |
| Current Value Wording | 정의된 속성입니다. | _For a property: How the Add fact form names a replaced value, as in 'Stan now has grade level 5'._ |
| Input Name | 정의된 속성입니다. | _For a property: How the integer check names this property's value, as in 'for grade level'._ |
| Record Kind | 정의된 속성입니다. | _For a property: The word the current-record summary uses for this property's values: role, grade or school._ |
| Record Order | 정의된 속성입니다. | _For a property: Position of this property in the current-record summary._ |
| Inverse of | 정의된 속성입니다. | _The property that reads this one backwards (OWL inverseOf): when A has this link to B, the inverse rule concludes that B has that one to A. Names a property: a property's table, or a column that is a property._ |
| Chain First | 정의된 속성입니다. | _With ChainSecond, the property chain that concludes this property (OWL propertyChainAxiom): a ChainFirst link followed by a ChainSecond link._ |
| Chain Second | 정의된 속성입니다. | _The chain's second link. Set together with ChainFirst._ |
| Disjoint With | 정의된 속성입니다. | _Properties that never both hold between the same two things (OWL propertyDisjointWith). Several are separated by ', '._ |

<details open>
<summary>예시 데이터</summary>

| Table Name | Description | Is Meta Table | Is Tenant Scoped | Tenant Parent Field | Sort Order | Label | Is User Addable | Is Symmetric | Value Node Label | Current Value Wording | Input Name | Record Kind | Record Order | Inverse of | Chain First | Chain Second | Disjoint With |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| Things | Every individual of the ontology (owl:Thing). A class's table lists the individuals that belong to it. | false | — | — | — | — | — | — | — | — | — | — | — | — | — | — | — |
| Person | The class Person: one row per individual that belongs to it. Its label and place are its ERBTables row. | false | — | — | 8 | Person | — | — | — | — | — | — | — | — | — | — | — |
| School | The class School: one row per individual that belongs to it. Its label and place are its ERBTables row. | false | — | — | 9 | School | — | — | — | — | — | — | — | — | — | — | — |

</details>

| 용어 | 설명 | 부연 설명 |
|------|-------------|-------------------|
| **ERB Field** | One row per field of every table (the platform's Schema module), and on it the same attributes for a property that is a column: a property with one current value, on its domain's class table (on Things when it has no domain class). | — |
| ERB Table | 정의된 속성입니다. | _The table the field is on._ |
| Field Name | 정의된 속성입니다. | _The field's name. For a column that is a property: the property's id._ |
| Field Type | 정의된 속성입니다. | _raw or relationship. Follows the field._ |
| Datatype | 정의된 속성입니다. | _The rulebook datatype. Follows the field._ |
| Is Primary Key | 빈 문자열에 참입니다. | _True for the field that identifies a row of its table._ |
| Description | 정의된 속성입니다. | _What the field means. Follows the field._ |
| Label | 정의된 속성입니다. | _For a property: The property's wording on the page._ |
| Is User Addable | 빈 문자열에 참입니다. | _For a property: Offered in the page's Add fact form._ |
| Is Symmetric | 빈 문자열에 참입니다. | _For a property: The symmetric rule makes this link true in both directions._ |
| Sort Order | 정의된 속성입니다. | _For a property: Position in the Add fact form's property list._ |
| Value Node Label | 정의된 속성입니다. | _For a property: The small label under a value node for this property: 'role value' or 'number value'._ |
| Current Value Wording | 정의된 속성입니다. | _For a property: How the Add fact form names a replaced value, as in 'Stan now has grade level 5'._ |
| Input Name | 정의된 속성입니다. | _For a property: How the integer check names this property's value, as in 'for grade level'._ |
| Record Kind | 정의된 속성입니다. | _For a property: The word the current-record summary uses for this property's values: role, grade or school._ |
| Record Order | 정의된 속성입니다. | _For a property: Position of this property in the current-record summary._ |
| Inverse of | 정의된 속성입니다. | _The property that reads this one backwards (OWL inverseOf): when A has this link to B, the inverse rule concludes that B has that one to A. Names a property: a property's table, or a column that is a property._ |
| Chain First | 정의된 속성입니다. | _With ChainSecond, the property chain that concludes this property (OWL propertyChainAxiom): a ChainFirst link followed by a ChainSecond link._ |
| Chain Second | 정의된 속성입니다. | _The chain's second link. Set together with ChainFirst._ |
| Disjoint With | 정의된 속성입니다. | _Properties that never both hold between the same two things (OWL propertyDisjointWith). Several are separated by ', '._ |

<details open>
<summary>예시 데이터</summary>

| Field Name | Field Type | Datatype | Is Primary Key | Description | Label | Is User Addable | Is Symmetric | Sort Order | Value Node Label | Current Value Wording | Input Name | Record Kind | Record Order | Inverse of | Chain First | Chain Second | Disjoint With |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| ThingId | raw | string | true | Key of the thing; also its local name in the model namespace. | — | — | — | — | — | — | — | — | — | — | — | — | — |
| Label | raw | string | false | The name the page shows. | — | — | — | — | — | — | — | — | — | — | — | — | — |
| SortOrder | raw | integer | false | Position in the cast. | — | — | — | — | — | — | — | — | — | — | — | — | — |

</details>

## 2 사실 유형

- **thing** 레코드는 **portrait** 레코드를 하나 참조할 수 있습니다
- **person** 레코드는 **thing** 레코드를 정확히 하나 참조합니다
- **school** 레코드는 **thing** 레코드를 정확히 하나 참조합니다
- **is parent of** 레코드는 **thing** 레코드를 정확히 하나 참조합니다
- **is child of** 레코드는 **thing** 레코드를 정확히 하나 참조합니다
- **is spouse of** 레코드는 **thing** 레코드를 정확히 하나 참조합니다
- **is friend of** 레코드는 **thing** 레코드를 정확히 하나 참조합니다
- **attend** 레코드는 **person** 레코드를 정확히 하나 참조합니다
- **attend** 레코드는 **school** 레코드를 정확히 하나 참조합니다
- **has age** 레코드는 **thing** 레코드를 정확히 하나 참조합니다
- **is grandparent of** 레코드는 **thing** 레코드를 정확히 하나 참조합니다
- **ERB table** 레코드는 **ERB table** 레코드를 하나 참조할 수 있습니다
- **ERB table** 레코드는 **portrait** 레코드를 하나 참조할 수 있습니다
- **ERB field** 레코드는 **ERB table** 레코드를 하나 참조할 수 있습니다

## 3 운영 규칙

_운영 규칙은 비즈니스가 **의무화**하거나 **금지**하거나 권고하는(**하는 것이 좋습니다**)
사항을 서술합니다. 구조적 규칙은 필수 필드와 외래 키에서 도출되고, 의미적 규칙은
Constraints 테이블에서 도출되며, 각 의미적 규칙은 룰북이 이미 계산하는 불리언 값을
판단 기준으로 삼습니다(아래 정의 규칙에서 DR-N으로 상호 참조)._

### 구조적 제약(스키마에서 도출)

- ‘thing’ 레코드는 ‘label’ 및 ‘sort order’ 값을 포함**해야 합니다**.
- ‘person’ 레코드는 ‘thing’ 레코드를 정확히 하나 참조**해야 합니다**.
- ‘person’ 레코드는 해당 레코드가 ‘asserted’에 해당하는지 여부를 기록**해야 합니다**.
- ‘school’ 레코드는 ‘thing’ 레코드를 정확히 하나 참조**해야 합니다**.
- ‘school’ 레코드는 해당 레코드가 ‘asserted’에 해당하는지 여부를 기록**해야 합니다**.
- ‘is parent of’ 레코드는 ‘subject’ 필드를 통해 ‘thing’ 레코드를 정확히 하나 참조**해야 합니다**.
- ‘is parent of’ 레코드는 ‘object’ 필드를 통해 ‘thing’ 레코드를 정확히 하나 참조**해야 합니다**.
- ‘is child of’ 레코드는 ‘subject’ 필드를 통해 ‘thing’ 레코드를 정확히 하나 참조**해야 합니다**.
- ‘is child of’ 레코드는 ‘object’ 필드를 통해 ‘thing’ 레코드를 정확히 하나 참조**해야 합니다**.
- ‘is spouse of’ 레코드는 ‘subject’ 필드를 통해 ‘thing’ 레코드를 정확히 하나 참조**해야 합니다**.
- ‘is spouse of’ 레코드는 ‘object’ 필드를 통해 ‘thing’ 레코드를 정확히 하나 참조**해야 합니다**.
- ‘is friend of’ 레코드는 ‘subject’ 필드를 통해 ‘thing’ 레코드를 정확히 하나 참조**해야 합니다**.
- ‘is friend of’ 레코드는 ‘object’ 필드를 통해 ‘thing’ 레코드를 정확히 하나 참조**해야 합니다**.
- ‘attend’ 레코드는 ‘subject’ 필드를 통해 ‘person’ 레코드를 정확히 하나 참조**해야 합니다**.
- ‘attend’ 레코드는 ‘object’ 필드를 통해 ‘school’ 레코드를 정확히 하나 참조**해야 합니다**.
- ‘has age’ 레코드는 ‘subject’ 필드를 통해 ‘thing’ 레코드를 정확히 하나 참조**해야 합니다**.
- ‘has age’ 레코드는 ‘value’ 값을 포함**해야 합니다**.
- ‘is grandparent of’ 레코드는 ‘subject’ 필드를 통해 ‘thing’ 레코드를 정확히 하나 참조**해야 합니다**.
- ‘is grandparent of’ 레코드는 ‘object’ 필드를 통해 ‘thing’ 레코드를 정확히 하나 참조**해야 합니다**.
- ‘portrait’ 레코드는 ‘display name’, ‘page’, ‘image’, ‘crop’, ‘width’, ‘height’ 및 ‘sort order’ 값을 포함**해야 합니다**.

## 4 정의 규칙

_모든 문장은 비즈니스 영역에서 성립하는 사실을 서술하며, 절차나 명령이 아닙니다.
한 방향의 필요조건이 동치로 오해되지 않도록 “iff”(필요충분조건) 대신 “경우에만”이라는
표현을 사용합니다. **⚠︎ 기계적 표현** 표시는 규칙에서 그대로 생성한 문장이 정확하기는 하지만
어색한 규칙에 붙습니다. 원하면 문장을 다듬어도 좋다는 표시일 뿐, 결함이 아닙니다._

| ID | 선언적 규칙 |
|----|------------------|
| **DR-1 Name** | ‘portrait’ 레코드의 ‘name’ 값은 같은 레코드의 ‘display name’ 값과 같습니다. |

## 5 스키마 추적성

_표현식 열은 문서 표기법으로 적은 규칙의 정의입니다.
룰북에 저장된 것과 같은 논리를 비즈니스 독자를 위해 풀어 쓴 것입니다._

| 스키마 요소 | 종류 | 표현식 |
|----------------|------|------------|
| **Portraits.Name** | 수식 | `DisplayName` |

---

> **RuleSpeak®** — **Ronald G. Ross**가 만든, **SBVR** 관례를 따르는 선언형 비즈니스 규칙 표기법 — 은(는) 곧 제공됩니다. 그때까지 이 페이지는 일반 문서를 보여 줍니다. [www.RonRoss.info](https://www.RonRoss.info)
