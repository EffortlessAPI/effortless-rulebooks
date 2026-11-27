# 📘 Talisman's South Park ontology explainer: the concept form — التوثيق

_This ontology in the concept form: a table of individuals, a table per class, a table or a column per property, and ERBTables and ERBFields for what the structure cannot say. orchestration/ontology_form.py wrote it; injecting it over the ontology brings the ontology's things, properties, axioms and facts in line with these tables._

> قواعد عمل تصريحية مستخرجة من دفتر القواعد. كل عبارة واردة أدناه تعبّر عن حقيقة
> في مجال العمل؛ فهي ليست إجراءً ولا أمرًا. صيغ دفتر القواعد هي المصدر الوحيد للحقيقة،
> وهذه الوثيقة قراءتها بلغة مبسّطة.

## 1 مفردات العمل

| المصطلح | الوصف | تعليق توضيحي |
|------|-------------|-------------------|
| **Thing** | Every individual of the ontology (owl:Thing). A class's table lists the individuals that belong to it. | — |
| Label | سمة معرّفة. | _The name the page shows._ |
| Sort Order | سمة معرّفة. | _Position in the cast._ |
| Portrait | سمة معرّفة. | _The portrait drawn for this thing; set when its name matches a portrait alias._ |

<details open>
<summary>بيانات توضيحية</summary>

| Label | Sort Order |
|---|---|
| Marvin | 1 |
| Randy | 2 |
| Sharon | 3 |

</details>

| المصطلح | الوصف | تعليق توضيحي |
|------|-------------|-------------------|
| **Person** | The class Person: one row per individual that belongs to it. Its label and place are its ERBTables row. | — |
| Thing | سمة معرّفة. | _The individual's row in Things._ |
| Is Asserted | صحيح إذا تحقق أنّ نص فارغ. | _True when a fact states it is a Person. False when only a property's domain or range implies it: such a row follows from the facts and is not added by hand._ |
| has Grade Level | سمة معرّفة. | _has grade level: the one current value. A property of the ontology; its other attributes are its ERBFields row._ |
| has Role | سمة معرّفة. | _has role: the one current value. A property of the ontology; its other attributes are its ERBFields row._ |

<details open>
<summary>بيانات توضيحية</summary>

| Is Asserted | has Grade Level | has Role |
|---|---|---|
| true | 4 | Student |

</details>

| المصطلح | الوصف | تعليق توضيحي |
|------|-------------|-------------------|
| **School** | The class School: one row per individual that belongs to it. Its label and place are its ERBTables row. | — |
| Thing | سمة معرّفة. | _The individual's row in Things._ |
| Is Asserted | صحيح إذا تحقق أنّ نص فارغ. | _True when a fact states it is a School. False when only a property's domain or range implies it: such a row follows from the facts and is not added by hand._ |

<details open>
<summary>بيانات توضيحية</summary>

| Is Asserted |
|---|
| false |

</details>

| المصطلح | الوصف | تعليق توضيحي |
|------|-------------|-------------------|
| **is Parent of** | is parent of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | سمة معرّفة. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | سمة معرّفة. | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| المصطلح | الوصف | تعليق توضيحي |
|------|-------------|-------------------|
| **is Child of** | is child of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | سمة معرّفة. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | سمة معرّفة. | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| المصطلح | الوصف | تعليق توضيحي |
|------|-------------|-------------------|
| **is Spouse of** | is spouse of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | سمة معرّفة. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | سمة معرّفة. | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| المصطلح | الوصف | تعليق توضيحي |
|------|-------------|-------------------|
| **is Friend of** | is friend of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | سمة معرّفة. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | سمة معرّفة. | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| المصطلح | الوصف | تعليق توضيحي |
|------|-------------|-------------------|
| **attend** | attends: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | سمة معرّفة. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | سمة معرّفة. | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| المصطلح | الوصف | تعليق توضيحي |
|------|-------------|-------------------|
| **has Age** | has age: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | سمة معرّفة. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Value | سمة معرّفة. | _The value._ |

<details open>
<summary>بيانات توضيحية</summary>

| Value |
|---|
| 10 |

</details>

| المصطلح | الوصف | تعليق توضيحي |
|------|-------------|-------------------|
| **is Grandparent of** | is grandparent of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | سمة معرّفة. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | سمة معرّفة. | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| المصطلح | الوصف | تعليق توضيحي |
|------|-------------|-------------------|
| **Portrait** | The page's Fandom portraits, including those for characters a reader may add. | — |
| Display Name | سمة معرّفة. | _The name on the portrait's Fandom page._ |
| Page | سمة معرّفة. | _The Fandom page credited for the portrait._ |
| Image | سمة معرّفة. | _The original Fandom image._ |
| Crop | سمة معرّفة. | _The crop as an SVG viewBox: x y width height._ |
| Width | سمة معرّفة. | _The original image's width in pixels._ |
| Height | سمة معرّفة. | _The original image's height in pixels._ |
| Sort Order | سمة معرّفة. | _Position in the credits._ |
| Name | يطابق حقل «display name» في السجل نفسه. | _Order 1. The portrait's name._ |

<details open>
<summary>بيانات توضيحية</summary>

| Display Name | Page | Image | Crop | Width | Height | Sort Order | Name ƒ |
|---|---|---|---|---|---|---|---|
| Marvin Marsh | https://southpark.fandom.com/wiki/Marvin_Marsh | https://static.wikia.nocookie.net/southpark/images/d/db/Marsh-family-grandpa-marsh-w-purple-blanket.png/revision/latest | 48 -5 280 280 | 375 | 536 | 1 | Marvin Marsh |
| Randy Marsh | https://southpark.fandom.com/wiki/Randy_Marsh | https://static.wikia.nocookie.net/southparkfanon/images/d/d6/Randy_Marsh.png/revision/latest | 0 -10 428 428 | 428 | 1057 | 2 | Randy Marsh |
| Sharon Marsh | https://southpark.fandom.com/wiki/Sharon_Marsh | https://static.wikia.nocookie.net/southpark/images/9/99/Sharon_Marsh.png/revision/latest | 20 -10 390 390 | 439 | 1049 | 3 | Sharon Marsh |

_ƒ تدل على عمود محسوب._

</details>

| المصطلح | الوصف | تعليق توضيحي |
|------|-------------|-------------------|
| **ERB Table** | One row per table of this rulebook (the platform's Schema module), and on it what the table's structure does not say. A class: its label and its place in the cast. A property that is a table: its label, its wording on the page, whether it is symmetric, and its inverse, chain and disjoint axioms. What a table IS is read from its fields: Thing and IsAsserted make a class; Subject with Object or Value make a property, whose domain and range are the tables they link to. | — |
| Table Name | سمة معرّفة. | _The table's name again, as a plain column._ |
| Description | سمة معرّفة. | _What the table holds. Follows the table._ |
| Is Meta Table | صحيح إذا تحقق أنّ نص فارغ. | _True for a table that serves the rulebook itself (ERBTables, ERBFields) and not the ontology._ |
| Is Tenant Scoped | صحيح إذا تحقق أنّ نص فارغ. | _The Schema module's: rows belong to one tenant. Not used by the ontology._ |
| Tenant Parent Table | سمة معرّفة. | _The Schema module's: the table tenancy is reached through. Not used by the ontology._ |
| Tenant Parent Field | سمة معرّفة. | _The Schema module's: the field that points at TenantParentTable. Not used by the ontology._ |
| Sort Order | سمة معرّفة. | _Display order. For a class: Position in the cast. For a property: Position in the Add fact form's property list._ |
| Label | سمة معرّفة. | _For a class: The name the page shows. For a property: The property's wording on the page._ |
| Portrait | سمة معرّفة. | _For a class: The portrait drawn for this thing; set when its name matches a portrait alias._ |
| Is User Addable | صحيح إذا تحقق أنّ نص فارغ. | _For a property: Offered in the page's Add fact form._ |
| Is Symmetric | صحيح إذا تحقق أنّ نص فارغ. | _For a property: The symmetric rule makes this link true in both directions._ |
| Value Node Label | سمة معرّفة. | _For a property: The small label under a value node for this property: 'role value' or 'number value'._ |
| Current Value Wording | سمة معرّفة. | _For a property: How the Add fact form names a replaced value, as in 'Stan now has grade level 5'._ |
| Input Name | سمة معرّفة. | _For a property: How the integer check names this property's value, as in 'for grade level'._ |
| Record Kind | سمة معرّفة. | _For a property: The word the current-record summary uses for this property's values: role, grade or school._ |
| Record Order | سمة معرّفة. | _For a property: Position of this property in the current-record summary._ |
| Inverse of | سمة معرّفة. | _The property that reads this one backwards (OWL inverseOf): when A has this link to B, the inverse rule concludes that B has that one to A. Names a property: a property's table, or a column that is a property._ |
| Chain First | سمة معرّفة. | _With ChainSecond, the property chain that concludes this property (OWL propertyChainAxiom): a ChainFirst link followed by a ChainSecond link._ |
| Chain Second | سمة معرّفة. | _The chain's second link. Set together with ChainFirst._ |
| Disjoint With | سمة معرّفة. | _Properties that never both hold between the same two things (OWL propertyDisjointWith). Several are separated by ', '._ |

<details open>
<summary>بيانات توضيحية</summary>

| Table Name | Description | Is Meta Table | Is Tenant Scoped | Tenant Parent Field | Sort Order | Label | Is User Addable | Is Symmetric | Value Node Label | Current Value Wording | Input Name | Record Kind | Record Order | Inverse of | Chain First | Chain Second | Disjoint With |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| Things | Every individual of the ontology (owl:Thing). A class's table lists the individuals that belong to it. | false | — | — | — | — | — | — | — | — | — | — | — | — | — | — | — |
| Person | The class Person: one row per individual that belongs to it. Its label and place are its ERBTables row. | false | — | — | 8 | Person | — | — | — | — | — | — | — | — | — | — | — |
| School | The class School: one row per individual that belongs to it. Its label and place are its ERBTables row. | false | — | — | 9 | School | — | — | — | — | — | — | — | — | — | — | — |

</details>

| المصطلح | الوصف | تعليق توضيحي |
|------|-------------|-------------------|
| **ERB Field** | One row per field of every table (the platform's Schema module), and on it the same attributes for a property that is a column: a property with one current value, on its domain's class table (on Things when it has no domain class). | — |
| ERB Table | سمة معرّفة. | _The table the field is on._ |
| Field Name | سمة معرّفة. | _The field's name. For a column that is a property: the property's id._ |
| Field Type | سمة معرّفة. | _raw or relationship. Follows the field._ |
| Datatype | سمة معرّفة. | _The rulebook datatype. Follows the field._ |
| Is Primary Key | صحيح إذا تحقق أنّ نص فارغ. | _True for the field that identifies a row of its table._ |
| Description | سمة معرّفة. | _What the field means. Follows the field._ |
| Label | سمة معرّفة. | _For a property: The property's wording on the page._ |
| Is User Addable | صحيح إذا تحقق أنّ نص فارغ. | _For a property: Offered in the page's Add fact form._ |
| Is Symmetric | صحيح إذا تحقق أنّ نص فارغ. | _For a property: The symmetric rule makes this link true in both directions._ |
| Sort Order | سمة معرّفة. | _For a property: Position in the Add fact form's property list._ |
| Value Node Label | سمة معرّفة. | _For a property: The small label under a value node for this property: 'role value' or 'number value'._ |
| Current Value Wording | سمة معرّفة. | _For a property: How the Add fact form names a replaced value, as in 'Stan now has grade level 5'._ |
| Input Name | سمة معرّفة. | _For a property: How the integer check names this property's value, as in 'for grade level'._ |
| Record Kind | سمة معرّفة. | _For a property: The word the current-record summary uses for this property's values: role, grade or school._ |
| Record Order | سمة معرّفة. | _For a property: Position of this property in the current-record summary._ |
| Inverse of | سمة معرّفة. | _The property that reads this one backwards (OWL inverseOf): when A has this link to B, the inverse rule concludes that B has that one to A. Names a property: a property's table, or a column that is a property._ |
| Chain First | سمة معرّفة. | _With ChainSecond, the property chain that concludes this property (OWL propertyChainAxiom): a ChainFirst link followed by a ChainSecond link._ |
| Chain Second | سمة معرّفة. | _The chain's second link. Set together with ChainFirst._ |
| Disjoint With | سمة معرّفة. | _Properties that never both hold between the same two things (OWL propertyDisjointWith). Several are separated by ', '._ |

<details open>
<summary>بيانات توضيحية</summary>

| Field Name | Field Type | Datatype | Is Primary Key | Description | Label | Is User Addable | Is Symmetric | Sort Order | Value Node Label | Current Value Wording | Input Name | Record Kind | Record Order | Inverse of | Chain First | Chain Second | Disjoint With |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| ThingId | raw | string | true | Key of the thing; also its local name in the model namespace. | — | — | — | — | — | — | — | — | — | — | — | — | — |
| Label | raw | string | false | The name the page shows. | — | — | — | — | — | — | — | — | — | — | — | — | — |
| SortOrder | raw | integer | false | Position in the cast. | — | — | — | — | — | — | — | — | — | — | — | — | — |

</details>

## 2 أنواع الحقائق

- كل سجل «**thing**» يجوز أن يشير إلى سجل «**portrait**» واحد
- كل سجل «**person**» يشير إلى سجل «**thing**» واحد فقط
- كل سجل «**school**» يشير إلى سجل «**thing**» واحد فقط
- كل سجل «**is parent of**» يشير إلى سجل «**thing**» واحد فقط
- كل سجل «**is child of**» يشير إلى سجل «**thing**» واحد فقط
- كل سجل «**is spouse of**» يشير إلى سجل «**thing**» واحد فقط
- كل سجل «**is friend of**» يشير إلى سجل «**thing**» واحد فقط
- كل سجل «**attend**» يشير إلى سجل «**person**» واحد فقط
- كل سجل «**attend**» يشير إلى سجل «**school**» واحد فقط
- كل سجل «**has age**» يشير إلى سجل «**thing**» واحد فقط
- كل سجل «**is grandparent of**» يشير إلى سجل «**thing**» واحد فقط
- كل سجل «**ERB table**» يجوز أن يشير إلى سجل «**ERB table**» واحد
- كل سجل «**ERB table**» يجوز أن يشير إلى سجل «**portrait**» واحد
- كل سجل «**ERB field**» يجوز أن يشير إلى سجل «**ERB table**» واحد

## 3 القواعد التشغيلية

_القواعد التشغيلية تبيّن ما **يوجبه** العمل وما **يحظره** وما يوصي به (**ينبغي أن**).
القواعد البنيوية مصدرها الحقول الإلزامية والمفاتيح الأجنبية؛ أما القواعد الدلالية فمصدرها
جدول Constraints، وتستند كل قاعدة منها إلى قيمة منطقية يحسبها دفتر القواعد أصلًا
(ويُشار إليها بالرمز DR-N في القواعد التعريفية أدناه)._

### القيود البنيوية (من المخطط)

- في كل سجل «thing»، **يجب أن** تُحدّد قيمة حقل «label» وحقل «sort order».
- **يجب أن** يشير كل سجل «person» إلى سجل «thing» واحد فقط.
- في كل سجل «person»، **يجب أن** يُسجّل ما إذا كان مصنّفًا على أنه «asserted».
- **يجب أن** يشير كل سجل «school» إلى سجل «thing» واحد فقط.
- في كل سجل «school»، **يجب أن** يُسجّل ما إذا كان مصنّفًا على أنه «asserted».
- **يجب أن** يشير كل سجل «is parent of» إلى سجل «thing» واحد فقط عبر حقل «subject».
- **يجب أن** يشير كل سجل «is parent of» إلى سجل «thing» واحد فقط عبر حقل «object».
- **يجب أن** يشير كل سجل «is child of» إلى سجل «thing» واحد فقط عبر حقل «subject».
- **يجب أن** يشير كل سجل «is child of» إلى سجل «thing» واحد فقط عبر حقل «object».
- **يجب أن** يشير كل سجل «is spouse of» إلى سجل «thing» واحد فقط عبر حقل «subject».
- **يجب أن** يشير كل سجل «is spouse of» إلى سجل «thing» واحد فقط عبر حقل «object».
- **يجب أن** يشير كل سجل «is friend of» إلى سجل «thing» واحد فقط عبر حقل «subject».
- **يجب أن** يشير كل سجل «is friend of» إلى سجل «thing» واحد فقط عبر حقل «object».
- **يجب أن** يشير كل سجل «attend» إلى سجل «person» واحد فقط عبر حقل «subject».
- **يجب أن** يشير كل سجل «attend» إلى سجل «school» واحد فقط عبر حقل «object».
- **يجب أن** يشير كل سجل «has age» إلى سجل «thing» واحد فقط عبر حقل «subject».
- في كل سجل «has age»، **يجب أن** تُحدّد قيمة حقل «value».
- **يجب أن** يشير كل سجل «is grandparent of» إلى سجل «thing» واحد فقط عبر حقل «subject».
- **يجب أن** يشير كل سجل «is grandparent of» إلى سجل «thing» واحد فقط عبر حقل «object».
- في كل سجل «portrait»، **يجب أن** تُحدّد قيمة حقل «display name»، وحقل «page»، وحقل «image»، وحقل «crop»، وحقل «width»، وحقل «height»، وحقل «sort order».

## 4 القواعد التعريفية

_جميع العبارات تعبّر عن حقيقة في مجال العمل؛ فهي ليست إجراءات ولا أوامر.
يُتجنّب التعبير «إذا وفقط إذا» ويُستعمل «فقط إذا»، حتى لا تُحسب الضرورة في اتجاه واحد تكافؤًا.
وتدل شارة **⚠︎ آلي** على قاعدة صياغتها الحتمية أمينة لكنها متكلّفة —
فهي إشارة إلى إعادة صياغة اختيارية لاحقة، لا إلى عيب._

| المعرّف | القاعدة التصريحية |
|----|------------------|
| **DR-1 Name** | حقل «name» في سجل «portrait» يطابق حقل «display name» في السجل نفسه. |

## 5 التتبّع إلى المخطط

_عمود التعبير هو تعريف القاعدة بترميز التوثيق —
المنطق نفسه الذي يخزّنه دفتر القواعد، مكتوبًا لقارئ من أهل العمل._

| عنصر المخطط | النوع | التعبير |
|----------------|------|------------|
| **Portraits.Name** | صيغة | `DisplayName` |

---

> **RuleSpeak®** — صيغة قواعد العمل التصريحية التي ابتكرها **Ronald G. Ross** وفق اصطلاحات **SBVR** — قادمة قريبًا. إلى ذلك الحين تعرض هذه الصفحة التوثيق المعتاد. [www.RonRoss.info](https://www.RonRoss.info)
