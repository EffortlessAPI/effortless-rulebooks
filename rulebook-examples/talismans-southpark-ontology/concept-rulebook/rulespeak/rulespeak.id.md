# 📘 Talisman's South Park ontology explainer: the concept form — Dokumentasi

_This ontology in the concept form: a table of individuals, a table per class, a table or a column per property, and ERBTables and ERBFields for what the structure cannot say. orchestration/ontology_form.py wrote it; injecting it over the ontology brings the ontology's things, properties, axioms and facts in line with these tables._

> Aturan bisnis deklaratif yang disusun dari buku aturan. Setiap pernyataan
> di bawah ini mengungkapkan kebenaran dalam domain bisnis — bukan prosedur
> dan bukan pula perintah. Rumus dalam buku aturan merupakan satu-satunya sumber kebenaran;
> dokumen ini adalah pembacaannya dalam bahasa sehari-hari.

## 1 Kosakata Bisnis

| Istilah | Deskripsi | Catatan Naratif |
|------|-------------|-------------------|
| **Thing** | Every individual of the ontology (owl:Thing). A class's table lists the individuals that belong to it. | — |
| Label | Atribut terdefinisi. | _The name the page shows._ |
| Sort Order | Atribut terdefinisi. | _Position in the cast._ |
| Portrait | Atribut terdefinisi. | _The portrait drawn for this thing; set when its name matches a portrait alias._ |

<details open>
<summary>Contoh data</summary>

| Label | Sort Order |
|---|---|
| Marvin | 1 |
| Randy | 2 |
| Sharon | 3 |

</details>

| Istilah | Deskripsi | Catatan Naratif |
|------|-------------|-------------------|
| **Person** | The class Person: one row per individual that belongs to it. Its label and place are its ERBTables row. | — |
| Thing | Atribut terdefinisi. | _The individual's row in Things._ |
| Is Asserted | Bernilai benar jika teks kosong. | _True when a fact states it is a Person. False when only a property's domain or range implies it: such a row follows from the facts and is not added by hand._ |
| has Grade Level | Atribut terdefinisi. | _has grade level: the one current value. A property of the ontology; its other attributes are its ERBFields row._ |
| has Role | Atribut terdefinisi. | _has role: the one current value. A property of the ontology; its other attributes are its ERBFields row._ |

<details open>
<summary>Contoh data</summary>

| Is Asserted | has Grade Level | has Role |
|---|---|---|
| true | 4 | Student |

</details>

| Istilah | Deskripsi | Catatan Naratif |
|------|-------------|-------------------|
| **School** | The class School: one row per individual that belongs to it. Its label and place are its ERBTables row. | — |
| Thing | Atribut terdefinisi. | _The individual's row in Things._ |
| Is Asserted | Bernilai benar jika teks kosong. | _True when a fact states it is a School. False when only a property's domain or range implies it: such a row follows from the facts and is not added by hand._ |

<details open>
<summary>Contoh data</summary>

| Is Asserted |
|---|
| false |

</details>

| Istilah | Deskripsi | Catatan Naratif |
|------|-------------|-------------------|
| **is Parent of** | is parent of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | Atribut terdefinisi. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | Atribut terdefinisi. | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| Istilah | Deskripsi | Catatan Naratif |
|------|-------------|-------------------|
| **is Child of** | is child of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | Atribut terdefinisi. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | Atribut terdefinisi. | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| Istilah | Deskripsi | Catatan Naratif |
|------|-------------|-------------------|
| **is Spouse of** | is spouse of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | Atribut terdefinisi. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | Atribut terdefinisi. | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| Istilah | Deskripsi | Catatan Naratif |
|------|-------------|-------------------|
| **is Friend of** | is friend of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | Atribut terdefinisi. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | Atribut terdefinisi. | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| Istilah | Deskripsi | Catatan Naratif |
|------|-------------|-------------------|
| **attend** | attends: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | Atribut terdefinisi. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | Atribut terdefinisi. | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| Istilah | Deskripsi | Catatan Naratif |
|------|-------------|-------------------|
| **has Age** | has age: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | Atribut terdefinisi. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Value | Atribut terdefinisi. | _The value._ |

<details open>
<summary>Contoh data</summary>

| Value |
|---|
| 10 |

</details>

| Istilah | Deskripsi | Catatan Naratif |
|------|-------------|-------------------|
| **is Grandparent of** | is grandparent of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | Atribut terdefinisi. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | Atribut terdefinisi. | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| Istilah | Deskripsi | Catatan Naratif |
|------|-------------|-------------------|
| **Portrait** | The page's Fandom portraits, including those for characters a reader may add. | — |
| Display Name | Atribut terdefinisi. | _The name on the portrait's Fandom page._ |
| Page | Atribut terdefinisi. | _The Fandom page credited for the portrait._ |
| Image | Atribut terdefinisi. | _The original Fandom image._ |
| Crop | Atribut terdefinisi. | _The crop as an SVG viewBox: x y width height._ |
| Width | Atribut terdefinisi. | _The original image's width in pixels._ |
| Height | Atribut terdefinisi. | _The original image's height in pixels._ |
| Sort Order | Atribut terdefinisi. | _Position in the credits._ |
| Name | Sama dengan display name. | _Order 1. The portrait's name._ |

<details open>
<summary>Contoh data</summary>

| Display Name | Page | Image | Crop | Width | Height | Sort Order | Name ƒ |
|---|---|---|---|---|---|---|---|
| Marvin Marsh | https://southpark.fandom.com/wiki/Marvin_Marsh | https://static.wikia.nocookie.net/southpark/images/d/db/Marsh-family-grandpa-marsh-w-purple-blanket.png/revision/latest | 48 -5 280 280 | 375 | 536 | 1 | Marvin Marsh |
| Randy Marsh | https://southpark.fandom.com/wiki/Randy_Marsh | https://static.wikia.nocookie.net/southparkfanon/images/d/d6/Randy_Marsh.png/revision/latest | 0 -10 428 428 | 428 | 1057 | 2 | Randy Marsh |
| Sharon Marsh | https://southpark.fandom.com/wiki/Sharon_Marsh | https://static.wikia.nocookie.net/southpark/images/9/99/Sharon_Marsh.png/revision/latest | 20 -10 390 390 | 439 | 1049 | 3 | Sharon Marsh |

_ƒ menandai kolom terhitung._

</details>

| Istilah | Deskripsi | Catatan Naratif |
|------|-------------|-------------------|
| **ERB Table** | One row per table of this rulebook (the platform's Schema module), and on it what the table's structure does not say. A class: its label and its place in the cast. A property that is a table: its label, its wording on the page, whether it is symmetric, and its inverse, chain and disjoint axioms. What a table IS is read from its fields: Thing and IsAsserted make a class; Subject with Object or Value make a property, whose domain and range are the tables they link to. | — |
| Table Name | Atribut terdefinisi. | _The table's name again, as a plain column._ |
| Description | Atribut terdefinisi. | _What the table holds. Follows the table._ |
| Is Meta Table | Bernilai benar jika teks kosong. | _True for a table that serves the rulebook itself (ERBTables, ERBFields) and not the ontology._ |
| Is Tenant Scoped | Bernilai benar jika teks kosong. | _The Schema module's: rows belong to one tenant. Not used by the ontology._ |
| Tenant Parent Table | Atribut terdefinisi. | _The Schema module's: the table tenancy is reached through. Not used by the ontology._ |
| Tenant Parent Field | Atribut terdefinisi. | _The Schema module's: the field that points at TenantParentTable. Not used by the ontology._ |
| Sort Order | Atribut terdefinisi. | _Display order. For a class: Position in the cast. For a property: Position in the Add fact form's property list._ |
| Label | Atribut terdefinisi. | _For a class: The name the page shows. For a property: The property's wording on the page._ |
| Portrait | Atribut terdefinisi. | _For a class: The portrait drawn for this thing; set when its name matches a portrait alias._ |
| Is User Addable | Bernilai benar jika teks kosong. | _For a property: Offered in the page's Add fact form._ |
| Is Symmetric | Bernilai benar jika teks kosong. | _For a property: The symmetric rule makes this link true in both directions._ |
| Value Node Label | Atribut terdefinisi. | _For a property: The small label under a value node for this property: 'role value' or 'number value'._ |
| Current Value Wording | Atribut terdefinisi. | _For a property: How the Add fact form names a replaced value, as in 'Stan now has grade level 5'._ |
| Input Name | Atribut terdefinisi. | _For a property: How the integer check names this property's value, as in 'for grade level'._ |
| Record Kind | Atribut terdefinisi. | _For a property: The word the current-record summary uses for this property's values: role, grade or school._ |
| Record Order | Atribut terdefinisi. | _For a property: Position of this property in the current-record summary._ |
| Inverse of | Atribut terdefinisi. | _The property that reads this one backwards (OWL inverseOf): when A has this link to B, the inverse rule concludes that B has that one to A. Names a property: a property's table, or a column that is a property._ |
| Chain First | Atribut terdefinisi. | _With ChainSecond, the property chain that concludes this property (OWL propertyChainAxiom): a ChainFirst link followed by a ChainSecond link._ |
| Chain Second | Atribut terdefinisi. | _The chain's second link. Set together with ChainFirst._ |
| Disjoint With | Atribut terdefinisi. | _Properties that never both hold between the same two things (OWL propertyDisjointWith). Several are separated by ', '._ |

<details open>
<summary>Contoh data</summary>

| Table Name | Description | Is Meta Table | Is Tenant Scoped | Tenant Parent Field | Sort Order | Label | Is User Addable | Is Symmetric | Value Node Label | Current Value Wording | Input Name | Record Kind | Record Order | Inverse of | Chain First | Chain Second | Disjoint With |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| Things | Every individual of the ontology (owl:Thing). A class's table lists the individuals that belong to it. | false | — | — | — | — | — | — | — | — | — | — | — | — | — | — | — |
| Person | The class Person: one row per individual that belongs to it. Its label and place are its ERBTables row. | false | — | — | 8 | Person | — | — | — | — | — | — | — | — | — | — | — |
| School | The class School: one row per individual that belongs to it. Its label and place are its ERBTables row. | false | — | — | 9 | School | — | — | — | — | — | — | — | — | — | — | — |

</details>

| Istilah | Deskripsi | Catatan Naratif |
|------|-------------|-------------------|
| **ERB Field** | One row per field of every table (the platform's Schema module), and on it the same attributes for a property that is a column: a property with one current value, on its domain's class table (on Things when it has no domain class). | — |
| ERB Table | Atribut terdefinisi. | _The table the field is on._ |
| Field Name | Atribut terdefinisi. | _The field's name. For a column that is a property: the property's id._ |
| Field Type | Atribut terdefinisi. | _raw or relationship. Follows the field._ |
| Datatype | Atribut terdefinisi. | _The rulebook datatype. Follows the field._ |
| Is Primary Key | Bernilai benar jika teks kosong. | _True for the field that identifies a row of its table._ |
| Description | Atribut terdefinisi. | _What the field means. Follows the field._ |
| Label | Atribut terdefinisi. | _For a property: The property's wording on the page._ |
| Is User Addable | Bernilai benar jika teks kosong. | _For a property: Offered in the page's Add fact form._ |
| Is Symmetric | Bernilai benar jika teks kosong. | _For a property: The symmetric rule makes this link true in both directions._ |
| Sort Order | Atribut terdefinisi. | _For a property: Position in the Add fact form's property list._ |
| Value Node Label | Atribut terdefinisi. | _For a property: The small label under a value node for this property: 'role value' or 'number value'._ |
| Current Value Wording | Atribut terdefinisi. | _For a property: How the Add fact form names a replaced value, as in 'Stan now has grade level 5'._ |
| Input Name | Atribut terdefinisi. | _For a property: How the integer check names this property's value, as in 'for grade level'._ |
| Record Kind | Atribut terdefinisi. | _For a property: The word the current-record summary uses for this property's values: role, grade or school._ |
| Record Order | Atribut terdefinisi. | _For a property: Position of this property in the current-record summary._ |
| Inverse of | Atribut terdefinisi. | _The property that reads this one backwards (OWL inverseOf): when A has this link to B, the inverse rule concludes that B has that one to A. Names a property: a property's table, or a column that is a property._ |
| Chain First | Atribut terdefinisi. | _With ChainSecond, the property chain that concludes this property (OWL propertyChainAxiom): a ChainFirst link followed by a ChainSecond link._ |
| Chain Second | Atribut terdefinisi. | _The chain's second link. Set together with ChainFirst._ |
| Disjoint With | Atribut terdefinisi. | _Properties that never both hold between the same two things (OWL propertyDisjointWith). Several are separated by ', '._ |

<details open>
<summary>Contoh data</summary>

| Field Name | Field Type | Datatype | Is Primary Key | Description | Label | Is User Addable | Is Symmetric | Sort Order | Value Node Label | Current Value Wording | Input Name | Record Kind | Record Order | Inverse of | Chain First | Chain Second | Disjoint With |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| ThingId | raw | string | true | Key of the thing; also its local name in the model namespace. | — | — | — | — | — | — | — | — | — | — | — | — | — |
| Label | raw | string | false | The name the page shows. | — | — | — | — | — | — | — | — | — | — | — | — | — |
| SortOrder | raw | integer | false | Position in the cast. | — | — | — | — | — | — | — | — | — | — | — | — | — |

</details>

## 2 Jenis Fakta

- Setiap **thing** dapat merujuk pada satu **portrait**
- Setiap **person** merujuk pada tepat satu **thing**
- Setiap **school** merujuk pada tepat satu **thing**
- Setiap **is parent of** merujuk pada tepat satu **thing**
- Setiap **is child of** merujuk pada tepat satu **thing**
- Setiap **is spouse of** merujuk pada tepat satu **thing**
- Setiap **is friend of** merujuk pada tepat satu **thing**
- Setiap **attend** merujuk pada tepat satu **person**
- Setiap **attend** merujuk pada tepat satu **school**
- Setiap **has age** merujuk pada tepat satu **thing**
- Setiap **is grandparent of** merujuk pada tepat satu **thing**
- Setiap **ERB table** dapat merujuk pada satu **ERB table**
- Setiap **ERB table** dapat merujuk pada satu **portrait**
- Setiap **ERB field** dapat merujuk pada satu **ERB table**

## 3 Aturan Operatif

_Aturan operatif menyatakan apa yang **diwajibkan**, **dilarang**, atau
dianjurkan (**sebaiknya**) oleh bisnis. Aturan struktural berasal dari kolom wajib dan kunci asing;
aturan semantik berasal dari tabel Constraints, masing-masing didasarkan pada nilai boolean yang sudah
dihitung oleh buku aturan (dirujuk silang sebagai DR-N dalam Aturan Definisional di bawah)._

### Batasan Struktural (dari skema)

- Setiap thing **harus** memiliki label dan sort order.
- Setiap person **harus** merujuk pada tepat satu thing.
- Setiap person **harus** menyatakan apakah tergolong “asserted”.
- Setiap school **harus** merujuk pada tepat satu thing.
- Setiap school **harus** menyatakan apakah tergolong “asserted”.
- Setiap is parent of **harus** merujuk pada tepat satu thing sebagai subject.
- Setiap is parent of **harus** merujuk pada tepat satu thing sebagai object.
- Setiap is child of **harus** merujuk pada tepat satu thing sebagai subject.
- Setiap is child of **harus** merujuk pada tepat satu thing sebagai object.
- Setiap is spouse of **harus** merujuk pada tepat satu thing sebagai subject.
- Setiap is spouse of **harus** merujuk pada tepat satu thing sebagai object.
- Setiap is friend of **harus** merujuk pada tepat satu thing sebagai subject.
- Setiap is friend of **harus** merujuk pada tepat satu thing sebagai object.
- Setiap attend **harus** merujuk pada tepat satu person sebagai subject.
- Setiap attend **harus** merujuk pada tepat satu school sebagai object.
- Setiap has age **harus** merujuk pada tepat satu thing sebagai subject.
- Setiap has age **harus** memiliki value.
- Setiap is grandparent of **harus** merujuk pada tepat satu thing sebagai subject.
- Setiap is grandparent of **harus** merujuk pada tepat satu thing sebagai object.
- Setiap portrait **harus** memiliki display name, page, image, crop, width, height, dan sort order.

## 4 Aturan Definisional

_Semua pernyataan mengungkapkan kebenaran dalam domain bisnis; bukan prosedur
dan bukan pula perintah. “Jika dan hanya jika” dihindari dan diganti dengan “hanya jika” agar
syarat perlu yang searah tidak disalahartikan sebagai kesetaraan. Tanda
**⚠︎ mekanis** menandai aturan yang rumusan deterministiknya setia pada rumus
tetapi kaku — isyarat untuk penyuntingan ulang yang opsional, bukan suatu cacat._

| ID | Aturan deklaratif |
|----|------------------|
| **DR-1 Name** | Untuk setiap portrait, name sama dengan display name. |

## 5 Ketertelusuran ke Skema

_Kolom ekspresi memuat definisi aturan dalam notasi Dokumentasi —
logika yang sama dengan yang disimpan dalam buku aturan, ditulis untuk pembaca bisnis._

| Elemen skema | Jenis | Ekspresi |
|----------------|------|------------|
| **Portraits.Name** | rumus | `DisplayName` |

---

> **RuleSpeak®** — notasi aturan bisnis deklaratif ciptaan **Ronald G. Ross** yang mengikuti konvensi **SBVR** — segera hadir. Sampai saat itu, halaman ini menampilkan dokumentasi biasa. [www.RonRoss.info](https://www.RonRoss.info)
