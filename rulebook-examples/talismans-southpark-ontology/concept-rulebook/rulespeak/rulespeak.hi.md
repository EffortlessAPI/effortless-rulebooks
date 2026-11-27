# 📘 Talisman's South Park ontology explainer: the concept form — दस्तावेज़ीकरण

_This ontology in the concept form: a table of individuals, a table per class, a table or a column per property, and ERBTables and ERBFields for what the structure cannot say. orchestration/ontology_form.py wrote it; injecting it over the ontology brings the ontology's things, properties, axioms and facts in line with these tables._

> नियम-पुस्तिका से प्रस्तुत घोषणात्मक व्यावसायिक नियम। नीचे दिया गया हर कथन
> व्यावसायिक क्षेत्र का सत्य व्यक्त करता है — यह न कोई प्रक्रिया है, न कोई आदेश।
> नियम-पुस्तिका के सूत्र ही सत्य के एकमात्र स्रोत हैं; यह दस्तावेज़ उनका
> सरल भाषा में पठन है।

## 1 व्यावसायिक शब्दावली

| पद | विवरण | व्याख्यात्मक टिप्पणी |
|------|-------------|-------------------|
| **Thing** | Every individual of the ontology (owl:Thing). A class's table lists the individuals that belong to it. | — |
| Label | एक परिभाषित विशेषता। | _The name the page shows._ |
| Sort Order | एक परिभाषित विशेषता। | _Position in the cast._ |
| Portrait | एक परिभाषित विशेषता। | _The portrait drawn for this thing; set when its name matches a portrait alias._ |

<details open>
<summary>उदाहरण डेटा</summary>

| Label | Sort Order |
|---|---|
| Marvin | 1 |
| Randy | 2 |
| Sharon | 3 |

</details>

| पद | विवरण | व्याख्यात्मक टिप्पणी |
|------|-------------|-------------------|
| **Person** | The class Person: one row per individual that belongs to it. Its label and place are its ERBTables row. | — |
| Thing | एक परिभाषित विशेषता। | _The individual's row in Things._ |
| Is Asserted | तब सत्य, जब एक खाली स्ट्रिंग। | _True when a fact states it is a Person. False when only a property's domain or range implies it: such a row follows from the facts and is not added by hand._ |
| has Grade Level | एक परिभाषित विशेषता। | _has grade level: the one current value. A property of the ontology; its other attributes are its ERBFields row._ |
| has Role | एक परिभाषित विशेषता। | _has role: the one current value. A property of the ontology; its other attributes are its ERBFields row._ |

<details open>
<summary>उदाहरण डेटा</summary>

| Is Asserted | has Grade Level | has Role |
|---|---|---|
| true | 4 | Student |

</details>

| पद | विवरण | व्याख्यात्मक टिप्पणी |
|------|-------------|-------------------|
| **School** | The class School: one row per individual that belongs to it. Its label and place are its ERBTables row. | — |
| Thing | एक परिभाषित विशेषता। | _The individual's row in Things._ |
| Is Asserted | तब सत्य, जब एक खाली स्ट्रिंग। | _True when a fact states it is a School. False when only a property's domain or range implies it: such a row follows from the facts and is not added by hand._ |

<details open>
<summary>उदाहरण डेटा</summary>

| Is Asserted |
|---|
| false |

</details>

| पद | विवरण | व्याख्यात्मक टिप्पणी |
|------|-------------|-------------------|
| **is Parent of** | is parent of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | एक परिभाषित विशेषता। | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | एक परिभाषित विशेषता। | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| पद | विवरण | व्याख्यात्मक टिप्पणी |
|------|-------------|-------------------|
| **is Child of** | is child of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | एक परिभाषित विशेषता। | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | एक परिभाषित विशेषता। | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| पद | विवरण | व्याख्यात्मक टिप्पणी |
|------|-------------|-------------------|
| **is Spouse of** | is spouse of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | एक परिभाषित विशेषता। | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | एक परिभाषित विशेषता। | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| पद | विवरण | व्याख्यात्मक टिप्पणी |
|------|-------------|-------------------|
| **is Friend of** | is friend of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | एक परिभाषित विशेषता। | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | एक परिभाषित विशेषता। | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| पद | विवरण | व्याख्यात्मक टिप्पणी |
|------|-------------|-------------------|
| **attend** | attends: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | एक परिभाषित विशेषता। | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | एक परिभाषित विशेषता। | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| पद | विवरण | व्याख्यात्मक टिप्पणी |
|------|-------------|-------------------|
| **has Age** | has age: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | एक परिभाषित विशेषता। | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Value | एक परिभाषित विशेषता। | _The value._ |

<details open>
<summary>उदाहरण डेटा</summary>

| Value |
|---|
| 10 |

</details>

| पद | विवरण | व्याख्यात्मक टिप्पणी |
|------|-------------|-------------------|
| **is Grandparent of** | is grandparent of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | एक परिभाषित विशेषता। | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | एक परिभाषित विशेषता। | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| पद | विवरण | व्याख्यात्मक टिप्पणी |
|------|-------------|-------------------|
| **Portrait** | The page's Fandom portraits, including those for characters a reader may add. | — |
| Display Name | एक परिभाषित विशेषता। | _The name on the portrait's Fandom page._ |
| Page | एक परिभाषित विशेषता। | _The Fandom page credited for the portrait._ |
| Image | एक परिभाषित विशेषता। | _The original Fandom image._ |
| Crop | एक परिभाषित विशेषता। | _The crop as an SVG viewBox: x y width height._ |
| Width | एक परिभाषित विशेषता। | _The original image's width in pixels._ |
| Height | एक परिभाषित विशेषता। | _The original image's height in pixels._ |
| Sort Order | एक परिभाषित विशेषता। | _Position in the credits._ |
| Name | उसके “display name” के समान। | _Order 1. The portrait's name._ |

<details open>
<summary>उदाहरण डेटा</summary>

| Display Name | Page | Image | Crop | Width | Height | Sort Order | Name ƒ |
|---|---|---|---|---|---|---|---|
| Marvin Marsh | https://southpark.fandom.com/wiki/Marvin_Marsh | https://static.wikia.nocookie.net/southpark/images/d/db/Marsh-family-grandpa-marsh-w-purple-blanket.png/revision/latest | 48 -5 280 280 | 375 | 536 | 1 | Marvin Marsh |
| Randy Marsh | https://southpark.fandom.com/wiki/Randy_Marsh | https://static.wikia.nocookie.net/southparkfanon/images/d/d6/Randy_Marsh.png/revision/latest | 0 -10 428 428 | 428 | 1057 | 2 | Randy Marsh |
| Sharon Marsh | https://southpark.fandom.com/wiki/Sharon_Marsh | https://static.wikia.nocookie.net/southpark/images/9/99/Sharon_Marsh.png/revision/latest | 20 -10 390 390 | 439 | 1049 | 3 | Sharon Marsh |

_ƒ परिकलित स्तंभ को चिह्नित करता है।_

</details>

| पद | विवरण | व्याख्यात्मक टिप्पणी |
|------|-------------|-------------------|
| **ERB Table** | One row per table of this rulebook (the platform's Schema module), and on it what the table's structure does not say. A class: its label and its place in the cast. A property that is a table: its label, its wording on the page, whether it is symmetric, and its inverse, chain and disjoint axioms. What a table IS is read from its fields: Thing and IsAsserted make a class; Subject with Object or Value make a property, whose domain and range are the tables they link to. | — |
| Table Name | एक परिभाषित विशेषता। | _The table's name again, as a plain column._ |
| Description | एक परिभाषित विशेषता। | _What the table holds. Follows the table._ |
| Is Meta Table | तब सत्य, जब एक खाली स्ट्रिंग। | _True for a table that serves the rulebook itself (ERBTables, ERBFields) and not the ontology._ |
| Is Tenant Scoped | तब सत्य, जब एक खाली स्ट्रिंग। | _The Schema module's: rows belong to one tenant. Not used by the ontology._ |
| Tenant Parent Table | एक परिभाषित विशेषता। | _The Schema module's: the table tenancy is reached through. Not used by the ontology._ |
| Tenant Parent Field | एक परिभाषित विशेषता। | _The Schema module's: the field that points at TenantParentTable. Not used by the ontology._ |
| Sort Order | एक परिभाषित विशेषता। | _Display order. For a class: Position in the cast. For a property: Position in the Add fact form's property list._ |
| Label | एक परिभाषित विशेषता। | _For a class: The name the page shows. For a property: The property's wording on the page._ |
| Portrait | एक परिभाषित विशेषता। | _For a class: The portrait drawn for this thing; set when its name matches a portrait alias._ |
| Is User Addable | तब सत्य, जब एक खाली स्ट्रिंग। | _For a property: Offered in the page's Add fact form._ |
| Is Symmetric | तब सत्य, जब एक खाली स्ट्रिंग। | _For a property: The symmetric rule makes this link true in both directions._ |
| Value Node Label | एक परिभाषित विशेषता। | _For a property: The small label under a value node for this property: 'role value' or 'number value'._ |
| Current Value Wording | एक परिभाषित विशेषता। | _For a property: How the Add fact form names a replaced value, as in 'Stan now has grade level 5'._ |
| Input Name | एक परिभाषित विशेषता। | _For a property: How the integer check names this property's value, as in 'for grade level'._ |
| Record Kind | एक परिभाषित विशेषता। | _For a property: The word the current-record summary uses for this property's values: role, grade or school._ |
| Record Order | एक परिभाषित विशेषता। | _For a property: Position of this property in the current-record summary._ |
| Inverse of | एक परिभाषित विशेषता। | _The property that reads this one backwards (OWL inverseOf): when A has this link to B, the inverse rule concludes that B has that one to A. Names a property: a property's table, or a column that is a property._ |
| Chain First | एक परिभाषित विशेषता। | _With ChainSecond, the property chain that concludes this property (OWL propertyChainAxiom): a ChainFirst link followed by a ChainSecond link._ |
| Chain Second | एक परिभाषित विशेषता। | _The chain's second link. Set together with ChainFirst._ |
| Disjoint With | एक परिभाषित विशेषता। | _Properties that never both hold between the same two things (OWL propertyDisjointWith). Several are separated by ', '._ |

<details open>
<summary>उदाहरण डेटा</summary>

| Table Name | Description | Is Meta Table | Is Tenant Scoped | Tenant Parent Field | Sort Order | Label | Is User Addable | Is Symmetric | Value Node Label | Current Value Wording | Input Name | Record Kind | Record Order | Inverse of | Chain First | Chain Second | Disjoint With |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| Things | Every individual of the ontology (owl:Thing). A class's table lists the individuals that belong to it. | false | — | — | — | — | — | — | — | — | — | — | — | — | — | — | — |
| Person | The class Person: one row per individual that belongs to it. Its label and place are its ERBTables row. | false | — | — | 8 | Person | — | — | — | — | — | — | — | — | — | — | — |
| School | The class School: one row per individual that belongs to it. Its label and place are its ERBTables row. | false | — | — | 9 | School | — | — | — | — | — | — | — | — | — | — | — |

</details>

| पद | विवरण | व्याख्यात्मक टिप्पणी |
|------|-------------|-------------------|
| **ERB Field** | One row per field of every table (the platform's Schema module), and on it the same attributes for a property that is a column: a property with one current value, on its domain's class table (on Things when it has no domain class). | — |
| ERB Table | एक परिभाषित विशेषता। | _The table the field is on._ |
| Field Name | एक परिभाषित विशेषता। | _The field's name. For a column that is a property: the property's id._ |
| Field Type | एक परिभाषित विशेषता। | _raw or relationship. Follows the field._ |
| Datatype | एक परिभाषित विशेषता। | _The rulebook datatype. Follows the field._ |
| Is Primary Key | तब सत्य, जब एक खाली स्ट्रिंग। | _True for the field that identifies a row of its table._ |
| Description | एक परिभाषित विशेषता। | _What the field means. Follows the field._ |
| Label | एक परिभाषित विशेषता। | _For a property: The property's wording on the page._ |
| Is User Addable | तब सत्य, जब एक खाली स्ट्रिंग। | _For a property: Offered in the page's Add fact form._ |
| Is Symmetric | तब सत्य, जब एक खाली स्ट्रिंग। | _For a property: The symmetric rule makes this link true in both directions._ |
| Sort Order | एक परिभाषित विशेषता। | _For a property: Position in the Add fact form's property list._ |
| Value Node Label | एक परिभाषित विशेषता। | _For a property: The small label under a value node for this property: 'role value' or 'number value'._ |
| Current Value Wording | एक परिभाषित विशेषता। | _For a property: How the Add fact form names a replaced value, as in 'Stan now has grade level 5'._ |
| Input Name | एक परिभाषित विशेषता। | _For a property: How the integer check names this property's value, as in 'for grade level'._ |
| Record Kind | एक परिभाषित विशेषता। | _For a property: The word the current-record summary uses for this property's values: role, grade or school._ |
| Record Order | एक परिभाषित विशेषता। | _For a property: Position of this property in the current-record summary._ |
| Inverse of | एक परिभाषित विशेषता। | _The property that reads this one backwards (OWL inverseOf): when A has this link to B, the inverse rule concludes that B has that one to A. Names a property: a property's table, or a column that is a property._ |
| Chain First | एक परिभाषित विशेषता। | _With ChainSecond, the property chain that concludes this property (OWL propertyChainAxiom): a ChainFirst link followed by a ChainSecond link._ |
| Chain Second | एक परिभाषित विशेषता। | _The chain's second link. Set together with ChainFirst._ |
| Disjoint With | एक परिभाषित विशेषता। | _Properties that never both hold between the same two things (OWL propertyDisjointWith). Several are separated by ', '._ |

<details open>
<summary>उदाहरण डेटा</summary>

| Field Name | Field Type | Datatype | Is Primary Key | Description | Label | Is User Addable | Is Symmetric | Sort Order | Value Node Label | Current Value Wording | Input Name | Record Kind | Record Order | Inverse of | Chain First | Chain Second | Disjoint With |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| ThingId | raw | string | true | Key of the thing; also its local name in the model namespace. | — | — | — | — | — | — | — | — | — | — | — | — | — |
| Label | raw | string | false | The name the page shows. | — | — | — | — | — | — | — | — | — | — | — | — | — |
| SortOrder | raw | integer | false | Position in the cast. | — | — | — | — | — | — | — | — | — | — | — | — | — |

</details>

## 2 तथ्य-प्रकार

- **thing** एक **portrait** का संदर्भ दे सकता है
- **person** ठीक एक **thing** का संदर्भ देता है
- **school** ठीक एक **thing** का संदर्भ देता है
- **is parent of** ठीक एक **thing** का संदर्भ देता है
- **is child of** ठीक एक **thing** का संदर्भ देता है
- **is spouse of** ठीक एक **thing** का संदर्भ देता है
- **is friend of** ठीक एक **thing** का संदर्भ देता है
- **attend** ठीक एक **person** का संदर्भ देता है
- **attend** ठीक एक **school** का संदर्भ देता है
- **has age** ठीक एक **thing** का संदर्भ देता है
- **is grandparent of** ठीक एक **thing** का संदर्भ देता है
- **ERB table** एक **ERB table** का संदर्भ दे सकता है
- **ERB table** एक **portrait** का संदर्भ दे सकता है
- **ERB field** एक **ERB table** का संदर्भ दे सकता है

## 3 प्रवर्तनकारी नियम

_प्रवर्तनकारी नियम बताते हैं कि व्यवसाय क्या **अनिवार्य** करता है, क्या **निषिद्ध** करता है,
या किसकी सलाह देता है (**चाहिए**)। संरचनात्मक नियम अनिवार्य फ़ील्डों और विदेशी कुंजियों से आते हैं;
अर्थगत नियम Constraints तालिका से आते हैं, जिनमें से हर एक किसी ऐसे बूलियन पर आधारित है जिसे
नियम-पुस्तिका पहले से गणना करती है (नीचे परिभाषात्मक नियमों में DR-N के रूप में संदर्भित)।_

### संरचनात्मक प्रतिबंध (स्कीमा से)

- thing को label और sort order रखना **अनिवार्य** है।
- person को ठीक एक “thing” का संदर्भ देना **अनिवार्य** है।
- person को यह कि वह “asserted” है या नहीं दर्ज करना **अनिवार्य** है।
- school को ठीक एक “thing” का संदर्भ देना **अनिवार्य** है।
- school को यह कि वह “asserted” है या नहीं दर्ज करना **अनिवार्य** है।
- is parent of को अपने “subject” के रूप में ठीक एक “thing” का संदर्भ देना **अनिवार्य** है।
- is parent of को अपने “object” के रूप में ठीक एक “thing” का संदर्भ देना **अनिवार्य** है।
- is child of को अपने “subject” के रूप में ठीक एक “thing” का संदर्भ देना **अनिवार्य** है।
- is child of को अपने “object” के रूप में ठीक एक “thing” का संदर्भ देना **अनिवार्य** है।
- is spouse of को अपने “subject” के रूप में ठीक एक “thing” का संदर्भ देना **अनिवार्य** है।
- is spouse of को अपने “object” के रूप में ठीक एक “thing” का संदर्भ देना **अनिवार्य** है।
- is friend of को अपने “subject” के रूप में ठीक एक “thing” का संदर्भ देना **अनिवार्य** है।
- is friend of को अपने “object” के रूप में ठीक एक “thing” का संदर्भ देना **अनिवार्य** है।
- attend को अपने “subject” के रूप में ठीक एक “person” का संदर्भ देना **अनिवार्य** है।
- attend को अपने “object” के रूप में ठीक एक “school” का संदर्भ देना **अनिवार्य** है।
- has age को अपने “subject” के रूप में ठीक एक “thing” का संदर्भ देना **अनिवार्य** है।
- has age को value रखना **अनिवार्य** है।
- is grandparent of को अपने “subject” के रूप में ठीक एक “thing” का संदर्भ देना **अनिवार्य** है।
- is grandparent of को अपने “object” के रूप में ठीक एक “thing” का संदर्भ देना **अनिवार्य** है।
- portrait को display name, page, image, crop, width, height और sort order रखना **अनिवार्य** है।

## 4 परिभाषात्मक नियम

_सभी कथन व्यावसायिक क्षेत्र का सत्य व्यक्त करते हैं; ये न प्रक्रियाएँ हैं, न आदेश।
"यदि और केवल यदि" के स्थान पर "केवल तभी जब" का प्रयोग किया गया है, ताकि एकतरफ़ा
अनिवार्यता को समतुल्यता न समझ लिया जाए। **⚠︎ यांत्रिक** चिह्न ऐसे नियम को दर्शाता है
जिसका निर्धारित शब्द-विन्यास सही तो है पर बोझिल — यह वैकल्पिक पुनर्लेखन का संकेत है,
कोई दोष नहीं।_

| क्रमांक | घोषणात्मक नियम |
|----|------------------|
| **DR-1 Name** | portrait का “name” उसके “display name” के समान है। |

## 5 स्कीमा तक अनुरेखणीयता

_व्यंजक स्तंभ दस्तावेज़ीकरण संकेतन में नियम की परिभाषा है —
वही तर्क जो नियम-पुस्तिका रखती है, व्यावसायिक पाठक के लिए लिखा हुआ।_

| स्कीमा तत्व | प्रकार | व्यंजक |
|----------------|------|------------|
| **Portraits.Name** | सूत्र | `DisplayName` |

---

> **RuleSpeak®** — **Ronald G. Ross** द्वारा रचित, **SBVR** के नियमों का पालन करने वाली घोषणात्मक व्यावसायिक-नियम संकेत-पद्धति — जल्द आ रही है। तब तक यह पृष्ठ सामान्य दस्तावेज़ीकरण दिखाता है। [www.RonRoss.info](https://www.RonRoss.info)
