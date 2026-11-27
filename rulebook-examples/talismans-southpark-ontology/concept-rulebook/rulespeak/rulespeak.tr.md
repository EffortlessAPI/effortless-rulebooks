# 📘 Talisman's South Park ontology explainer: the concept form — Dokümantasyon

_This ontology in the concept form: a table of individuals, a table per class, a table or a column per property, and ERBTables and ERBFields for what the structure cannot say. orchestration/ontology_form.py wrote it; injecting it over the ontology brings the ontology's things, properties, axioms and facts in line with these tables._

> Kural kitabından üretilmiş bildirimsel iş kuralları. Aşağıdaki her ifade
> iş bağlamındaki bir doğruyu dile getirir; ne bir prosedürdür ne de bir buyruk.
> Kural kitabındaki formüller tek doğruluk kaynağıdır;
> bu belge onların yalın dildeki okunuşudur.

## 1 İş Terimleri Sözlüğü

| Terim | Açıklama | Açıklayıcı Not |
|------|-------------|-------------------|
| **Thing** | Every individual of the ontology (owl:Thing). A class's table lists the individuals that belong to it. | — |
| Label | Tanımlı bir öznitelik. | _The name the page shows._ |
| Sort Order | Tanımlı bir öznitelik. | _Position in the cast._ |
| Portrait | Tanımlı bir öznitelik. | _The portrait drawn for this thing; set when its name matches a portrait alias._ |

<details open>
<summary>Örnek veri</summary>

| Label | Sort Order |
|---|---|
| Marvin | 1 |
| Randy | 2 |
| Sharon | 3 |

</details>

| Terim | Açıklama | Açıklayıcı Not |
|------|-------------|-------------------|
| **Person** | The class Person: one row per individual that belongs to it. Its label and place are its ERBTables row. | — |
| Thing | Tanımlı bir öznitelik. | _The individual's row in Things._ |
| Is Asserted | Doğru olma koşulu: boş metin. | _True when a fact states it is a Person. False when only a property's domain or range implies it: such a row follows from the facts and is not added by hand._ |
| has Grade Level | Tanımlı bir öznitelik. | _has grade level: the one current value. A property of the ontology; its other attributes are its ERBFields row._ |
| has Role | Tanımlı bir öznitelik. | _has role: the one current value. A property of the ontology; its other attributes are its ERBFields row._ |

<details open>
<summary>Örnek veri</summary>

| Is Asserted | has Grade Level | has Role |
|---|---|---|
| true | 4 | Student |

</details>

| Terim | Açıklama | Açıklayıcı Not |
|------|-------------|-------------------|
| **School** | The class School: one row per individual that belongs to it. Its label and place are its ERBTables row. | — |
| Thing | Tanımlı bir öznitelik. | _The individual's row in Things._ |
| Is Asserted | Doğru olma koşulu: boş metin. | _True when a fact states it is a School. False when only a property's domain or range implies it: such a row follows from the facts and is not added by hand._ |

<details open>
<summary>Örnek veri</summary>

| Is Asserted |
|---|
| false |

</details>

| Terim | Açıklama | Açıklayıcı Not |
|------|-------------|-------------------|
| **is Parent of** | is parent of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | Tanımlı bir öznitelik. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | Tanımlı bir öznitelik. | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| Terim | Açıklama | Açıklayıcı Not |
|------|-------------|-------------------|
| **is Child of** | is child of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | Tanımlı bir öznitelik. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | Tanımlı bir öznitelik. | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| Terim | Açıklama | Açıklayıcı Not |
|------|-------------|-------------------|
| **is Spouse of** | is spouse of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | Tanımlı bir öznitelik. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | Tanımlı bir öznitelik. | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| Terim | Açıklama | Açıklayıcı Not |
|------|-------------|-------------------|
| **is Friend of** | is friend of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | Tanımlı bir öznitelik. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | Tanımlı bir öznitelik. | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| Terim | Açıklama | Açıklayıcı Not |
|------|-------------|-------------------|
| **attend** | attends: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | Tanımlı bir öznitelik. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | Tanımlı bir öznitelik. | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| Terim | Açıklama | Açıklayıcı Not |
|------|-------------|-------------------|
| **has Age** | has age: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | Tanımlı bir öznitelik. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Value | Tanımlı bir öznitelik. | _The value._ |

<details open>
<summary>Örnek veri</summary>

| Value |
|---|
| 10 |

</details>

| Terim | Açıklama | Açıklayıcı Not |
|------|-------------|-------------------|
| **is Grandparent of** | is grandparent of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | Tanımlı bir öznitelik. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | Tanımlı bir öznitelik. | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| Terim | Açıklama | Açıklayıcı Not |
|------|-------------|-------------------|
| **Portrait** | The page's Fandom portraits, including those for characters a reader may add. | — |
| Display Name | Tanımlı bir öznitelik. | _The name on the portrait's Fandom page._ |
| Page | Tanımlı bir öznitelik. | _The Fandom page credited for the portrait._ |
| Image | Tanımlı bir öznitelik. | _The original Fandom image._ |
| Crop | Tanımlı bir öznitelik. | _The crop as an SVG viewBox: x y width height._ |
| Width | Tanımlı bir öznitelik. | _The original image's width in pixels._ |
| Height | Tanımlı bir öznitelik. | _The original image's height in pixels._ |
| Sort Order | Tanımlı bir öznitelik. | _Position in the credits._ |
| Name | “display name” alanı ile aynı değeri taşır. | _Order 1. The portrait's name._ |

<details open>
<summary>Örnek veri</summary>

| Display Name | Page | Image | Crop | Width | Height | Sort Order | Name ƒ |
|---|---|---|---|---|---|---|---|
| Marvin Marsh | https://southpark.fandom.com/wiki/Marvin_Marsh | https://static.wikia.nocookie.net/southpark/images/d/db/Marsh-family-grandpa-marsh-w-purple-blanket.png/revision/latest | 48 -5 280 280 | 375 | 536 | 1 | Marvin Marsh |
| Randy Marsh | https://southpark.fandom.com/wiki/Randy_Marsh | https://static.wikia.nocookie.net/southparkfanon/images/d/d6/Randy_Marsh.png/revision/latest | 0 -10 428 428 | 428 | 1057 | 2 | Randy Marsh |
| Sharon Marsh | https://southpark.fandom.com/wiki/Sharon_Marsh | https://static.wikia.nocookie.net/southpark/images/9/99/Sharon_Marsh.png/revision/latest | 20 -10 390 390 | 439 | 1049 | 3 | Sharon Marsh |

_ƒ, hesaplanan bir sütunu gösterir._

</details>

| Terim | Açıklama | Açıklayıcı Not |
|------|-------------|-------------------|
| **ERB Table** | One row per table of this rulebook (the platform's Schema module), and on it what the table's structure does not say. A class: its label and its place in the cast. A property that is a table: its label, its wording on the page, whether it is symmetric, and its inverse, chain and disjoint axioms. What a table IS is read from its fields: Thing and IsAsserted make a class; Subject with Object or Value make a property, whose domain and range are the tables they link to. | — |
| Table Name | Tanımlı bir öznitelik. | _The table's name again, as a plain column._ |
| Description | Tanımlı bir öznitelik. | _What the table holds. Follows the table._ |
| Is Meta Table | Doğru olma koşulu: boş metin. | _True for a table that serves the rulebook itself (ERBTables, ERBFields) and not the ontology._ |
| Is Tenant Scoped | Doğru olma koşulu: boş metin. | _The Schema module's: rows belong to one tenant. Not used by the ontology._ |
| Tenant Parent Table | Tanımlı bir öznitelik. | _The Schema module's: the table tenancy is reached through. Not used by the ontology._ |
| Tenant Parent Field | Tanımlı bir öznitelik. | _The Schema module's: the field that points at TenantParentTable. Not used by the ontology._ |
| Sort Order | Tanımlı bir öznitelik. | _Display order. For a class: Position in the cast. For a property: Position in the Add fact form's property list._ |
| Label | Tanımlı bir öznitelik. | _For a class: The name the page shows. For a property: The property's wording on the page._ |
| Portrait | Tanımlı bir öznitelik. | _For a class: The portrait drawn for this thing; set when its name matches a portrait alias._ |
| Is User Addable | Doğru olma koşulu: boş metin. | _For a property: Offered in the page's Add fact form._ |
| Is Symmetric | Doğru olma koşulu: boş metin. | _For a property: The symmetric rule makes this link true in both directions._ |
| Value Node Label | Tanımlı bir öznitelik. | _For a property: The small label under a value node for this property: 'role value' or 'number value'._ |
| Current Value Wording | Tanımlı bir öznitelik. | _For a property: How the Add fact form names a replaced value, as in 'Stan now has grade level 5'._ |
| Input Name | Tanımlı bir öznitelik. | _For a property: How the integer check names this property's value, as in 'for grade level'._ |
| Record Kind | Tanımlı bir öznitelik. | _For a property: The word the current-record summary uses for this property's values: role, grade or school._ |
| Record Order | Tanımlı bir öznitelik. | _For a property: Position of this property in the current-record summary._ |
| Inverse of | Tanımlı bir öznitelik. | _The property that reads this one backwards (OWL inverseOf): when A has this link to B, the inverse rule concludes that B has that one to A. Names a property: a property's table, or a column that is a property._ |
| Chain First | Tanımlı bir öznitelik. | _With ChainSecond, the property chain that concludes this property (OWL propertyChainAxiom): a ChainFirst link followed by a ChainSecond link._ |
| Chain Second | Tanımlı bir öznitelik. | _The chain's second link. Set together with ChainFirst._ |
| Disjoint With | Tanımlı bir öznitelik. | _Properties that never both hold between the same two things (OWL propertyDisjointWith). Several are separated by ', '._ |

<details open>
<summary>Örnek veri</summary>

| Table Name | Description | Is Meta Table | Is Tenant Scoped | Tenant Parent Field | Sort Order | Label | Is User Addable | Is Symmetric | Value Node Label | Current Value Wording | Input Name | Record Kind | Record Order | Inverse of | Chain First | Chain Second | Disjoint With |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| Things | Every individual of the ontology (owl:Thing). A class's table lists the individuals that belong to it. | false | — | — | — | — | — | — | — | — | — | — | — | — | — | — | — |
| Person | The class Person: one row per individual that belongs to it. Its label and place are its ERBTables row. | false | — | — | 8 | Person | — | — | — | — | — | — | — | — | — | — | — |
| School | The class School: one row per individual that belongs to it. Its label and place are its ERBTables row. | false | — | — | 9 | School | — | — | — | — | — | — | — | — | — | — | — |

</details>

| Terim | Açıklama | Açıklayıcı Not |
|------|-------------|-------------------|
| **ERB Field** | One row per field of every table (the platform's Schema module), and on it the same attributes for a property that is a column: a property with one current value, on its domain's class table (on Things when it has no domain class). | — |
| ERB Table | Tanımlı bir öznitelik. | _The table the field is on._ |
| Field Name | Tanımlı bir öznitelik. | _The field's name. For a column that is a property: the property's id._ |
| Field Type | Tanımlı bir öznitelik. | _raw or relationship. Follows the field._ |
| Datatype | Tanımlı bir öznitelik. | _The rulebook datatype. Follows the field._ |
| Is Primary Key | Doğru olma koşulu: boş metin. | _True for the field that identifies a row of its table._ |
| Description | Tanımlı bir öznitelik. | _What the field means. Follows the field._ |
| Label | Tanımlı bir öznitelik. | _For a property: The property's wording on the page._ |
| Is User Addable | Doğru olma koşulu: boş metin. | _For a property: Offered in the page's Add fact form._ |
| Is Symmetric | Doğru olma koşulu: boş metin. | _For a property: The symmetric rule makes this link true in both directions._ |
| Sort Order | Tanımlı bir öznitelik. | _For a property: Position in the Add fact form's property list._ |
| Value Node Label | Tanımlı bir öznitelik. | _For a property: The small label under a value node for this property: 'role value' or 'number value'._ |
| Current Value Wording | Tanımlı bir öznitelik. | _For a property: How the Add fact form names a replaced value, as in 'Stan now has grade level 5'._ |
| Input Name | Tanımlı bir öznitelik. | _For a property: How the integer check names this property's value, as in 'for grade level'._ |
| Record Kind | Tanımlı bir öznitelik. | _For a property: The word the current-record summary uses for this property's values: role, grade or school._ |
| Record Order | Tanımlı bir öznitelik. | _For a property: Position of this property in the current-record summary._ |
| Inverse of | Tanımlı bir öznitelik. | _The property that reads this one backwards (OWL inverseOf): when A has this link to B, the inverse rule concludes that B has that one to A. Names a property: a property's table, or a column that is a property._ |
| Chain First | Tanımlı bir öznitelik. | _With ChainSecond, the property chain that concludes this property (OWL propertyChainAxiom): a ChainFirst link followed by a ChainSecond link._ |
| Chain Second | Tanımlı bir öznitelik. | _The chain's second link. Set together with ChainFirst._ |
| Disjoint With | Tanımlı bir öznitelik. | _Properties that never both hold between the same two things (OWL propertyDisjointWith). Several are separated by ', '._ |

<details open>
<summary>Örnek veri</summary>

| Field Name | Field Type | Datatype | Is Primary Key | Description | Label | Is User Addable | Is Symmetric | Sort Order | Value Node Label | Current Value Wording | Input Name | Record Kind | Record Order | Inverse of | Chain First | Chain Second | Disjoint With |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| ThingId | raw | string | true | Key of the thing; also its local name in the model namespace. | — | — | — | — | — | — | — | — | — | — | — | — | — |
| Label | raw | string | false | The name the page shows. | — | — | — | — | — | — | — | — | — | — | — | — | — |
| SortOrder | raw | integer | false | Position in the cast. | — | — | — | — | — | — | — | — | — | — | — | — | — |

</details>

## 2 Olgu Türleri

- Her **thing** kaydı bir **portrait** kaydına referans verebilir
- Her **person** kaydı tam olarak bir **thing** kaydına referans verir
- Her **school** kaydı tam olarak bir **thing** kaydına referans verir
- Her **is parent of** kaydı tam olarak bir **thing** kaydına referans verir
- Her **is child of** kaydı tam olarak bir **thing** kaydına referans verir
- Her **is spouse of** kaydı tam olarak bir **thing** kaydına referans verir
- Her **is friend of** kaydı tam olarak bir **thing** kaydına referans verir
- Her **attend** kaydı tam olarak bir **person** kaydına referans verir
- Her **attend** kaydı tam olarak bir **school** kaydına referans verir
- Her **has age** kaydı tam olarak bir **thing** kaydına referans verir
- Her **is grandparent of** kaydı tam olarak bir **thing** kaydına referans verir
- Her **ERB table** kaydı bir **ERB table** kaydına referans verebilir
- Her **ERB table** kaydı bir **portrait** kaydına referans verebilir
- Her **ERB field** kaydı bir **ERB table** kaydına referans verebilir

## 3 İşletimsel Kurallar

_İşletimsel kurallar, işletmenin neyi **zorunlu kıldığını**, neyi **yasakladığını** ve neyi
tavsiye ettiğini (**önerilir**) belirtir. Yapısal kurallar zorunlu alanlardan ve yabancı anahtarlardan türetilir;
anlamsal kurallar Constraints tablosundan gelir ve her biri kural kitabının zaten hesapladığı
bir mantıksal değere dayanır (aşağıdaki Tanımsal Kurallar bölümünde DR-N olarak çapraz başvurulur)._

### Yapısal Kısıtlar (şemadan)

- “thing” kaydı, “label” alanı ve “sort order” alanı dolu olmak **zorundadır**.
- “person” kaydı, tam olarak bir “thing” kaydına referans vermek **zorundadır**.
- “person” kaydı, “asserted” olup olmadığı bilgisini kaydetmek **zorundadır**.
- “school” kaydı, tam olarak bir “thing” kaydına referans vermek **zorundadır**.
- “school” kaydı, “asserted” olup olmadığı bilgisini kaydetmek **zorundadır**.
- “is parent of” kaydı, “subject” alanı aracılığıyla tam olarak bir “thing” kaydına referans vermek **zorundadır**.
- “is parent of” kaydı, “object” alanı aracılığıyla tam olarak bir “thing” kaydına referans vermek **zorundadır**.
- “is child of” kaydı, “subject” alanı aracılığıyla tam olarak bir “thing” kaydına referans vermek **zorundadır**.
- “is child of” kaydı, “object” alanı aracılığıyla tam olarak bir “thing” kaydına referans vermek **zorundadır**.
- “is spouse of” kaydı, “subject” alanı aracılığıyla tam olarak bir “thing” kaydına referans vermek **zorundadır**.
- “is spouse of” kaydı, “object” alanı aracılığıyla tam olarak bir “thing” kaydına referans vermek **zorundadır**.
- “is friend of” kaydı, “subject” alanı aracılığıyla tam olarak bir “thing” kaydına referans vermek **zorundadır**.
- “is friend of” kaydı, “object” alanı aracılığıyla tam olarak bir “thing” kaydına referans vermek **zorundadır**.
- “attend” kaydı, “subject” alanı aracılığıyla tam olarak bir “person” kaydına referans vermek **zorundadır**.
- “attend” kaydı, “object” alanı aracılığıyla tam olarak bir “school” kaydına referans vermek **zorundadır**.
- “has age” kaydı, “subject” alanı aracılığıyla tam olarak bir “thing” kaydına referans vermek **zorundadır**.
- “has age” kaydı, “value” alanı dolu olmak **zorundadır**.
- “is grandparent of” kaydı, “subject” alanı aracılığıyla tam olarak bir “thing” kaydına referans vermek **zorundadır**.
- “is grandparent of” kaydı, “object” alanı aracılığıyla tam olarak bir “thing” kaydına referans vermek **zorundadır**.
- “portrait” kaydı, “display name” alanı, “page” alanı, “image” alanı, “crop” alanı, “width” alanı, “height” alanı ve “sort order” alanı dolu olmak **zorundadır**.

## 4 Tanımsal Kurallar

_Tüm ifadeler iş bağlamındaki bir doğruyu dile getirir; ne prosedürdür ne de buyruk.
Tek yönlü bir gereklilik denklik sanılmasın diye “ancak ve ancak” yerine
“yalnızca … ise” kullanılır. **⚠︎ mekanik** etiketi, belirlenimci ifadesi aslına sadık
ama hantal olan bir kuralı gösterir — isteğe bağlı bir yeniden yazım turu için
bir işarettir, kusur değildir._

| Kural No | Bildirimsel kural |
|----|------------------|
| **DR-1 Name** | “portrait” kaydının “name” alanı, “display name” alanı ile aynı değeri taşır. |

## 5 Şemaya İzlenebilirlik

_İfade sütunu, kuralın Dokümantasyon gösterimindeki tanımıdır —
kural kitabının sakladığı mantığın aynısı, iş okuru için yazılmış biçimiyle._

| Şema öğesi | Tür | İfade |
|----------------|------|------------|
| **Portraits.Name** | formül | `DisplayName` |

---

> **RuleSpeak®** — **Ronald G. Ross** tarafından geliştirilen, **SBVR** kurallarına uyan bildirimsel iş kuralı gösterimi — yakında geliyor. O zamana kadar bu sayfa olağan dokümantasyonu gösterir. [www.RonRoss.info](https://www.RonRoss.info)
