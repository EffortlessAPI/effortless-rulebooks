# 📘 South Park ontology explainer — Dokumentasi

_The barebones South Park ontology from Jessica Talisman's explainer: ten stated facts and five rules that can each be switched on. Conclusions are formulas._

> Aturan bisnis deklaratif yang disusun dari buku aturan. Setiap pernyataan
> di bawah ini mengungkapkan kebenaran dalam domain bisnis — bukan prosedur
> dan bukan pula perintah. Rumus dalam buku aturan merupakan satu-satunya sumber kebenaran;
> dokumen ini adalah pembacaannya dalam bahasa sehari-hari.

## 1 Kosakata Bisnis

| Istilah | Deskripsi | Catatan Naratif |
|------|-------------|-------------------|
| **Graph** | The whole example. | — |
| Name | Sama dengan graph ID. | — |
| Count of Stated Facts | Nilai total stated unit pada seluruh statements yang terkait dengan graph tersebut. | — |
| Count of Conclusions | Nilai total conclusion unit pada seluruh statements yang terkait dengan graph tersebut. | _Facts that hold but were never stated._ |
| Count of Violations | Nilai total violation unit pada seluruh disjoint checks yang terkait dengan graph tersebut. | — |
| Is Consistent | Bernilai benar jika count of violations adalah 0. | — |

<details open>
<summary>Contoh data</summary>

| Name ƒ | Count of Stated Facts ƒ | Count of Conclusions ƒ | Count of Violations ƒ | Is Consistent ƒ |
|---|---|---|---|---|
| south-park | 10 | 8 | 0 | true |

_ƒ menandai kolom terhitung._

</details>

| Istilah | Deskripsi | Catatan Naratif |
|------|-------------|-------------------|
| **Rule** | The five rules; IsEnabled is the checkbox. | — |
| Label | Atribut terdefinisi. | — |
| Explanation | Atribut terdefinisi. | — |
| Is Enabled | Bernilai benar jika teks kosong. | — |
| Name | Sama dengan label. | — |
| Count of Firings | Nilai total fired unit pada seluruh firings yang terkait dengan rule tersebut. | _How many times the rule produced a fact._ |

<details open>
<summary>Contoh data</summary>

| Label | Explanation | Is Enabled | Name ƒ | Count of Firings ƒ |
|---|---|---|---|---|
| Inverse | A parent link also gives a child link between the same two people. | true | Inverse | 3 |
| Symmetric | A friend link or a spouse link is true for both people. | true | Symmetric | 2 |
| Property chain | A parent of a parent is a grandparent. | true | Property chain | 2 |

_ƒ menandai kolom terhitung._

</details>

| Istilah | Deskripsi | Catatan Naratif |
|------|-------------|-------------------|
| **Thing** | A node in the picture: a person, a place, a class or a role value. | — |
| Label | Atribut terdefinisi. | — |
| Kind | Atribut terdefinisi. | — |
| Name | Sama dengan label. | — |
| Count of Children | Nilai total parent unit pada seluruh statements yang terkait dengan thing tersebut. | _Added: a count, which a reasoner alone does not give._ |
| Count of Grandchildren | Nilai total grandparent unit pada seluruh statements yang terkait dengan thing tersebut. | _Added: counts concluded grandchildren._ |

<details open>
<summary>Contoh data</summary>

| Label | Kind | Name ƒ | Count of Children ƒ | Count of Grandchildren ƒ |
|---|---|---|---|---|
| Marvin | individual | Marvin | 1 | 2 |
| Randy | individual | Randy | 2 | 0 |
| Sharon | individual | Sharon | 0 | 0 |

_ƒ menandai kolom terhitung._

</details>

| Istilah | Deskripsi | Catatan Naratif |
|------|-------------|-------------------|
| **Property** | A kind of link, with the characteristics the rules use. | — |
| Label | Atribut terdefinisi. | — |
| Inverse of | Atribut terdefinisi. | — |
| Is Symmetric | Bernilai benar jika teks kosong. | — |
| Chain First | Atribut terdefinisi. | — |
| Chain Second | Atribut terdefinisi. | — |
| Domain Class | Atribut terdefinisi. | — |
| Range Class | Atribut terdefinisi. | — |
| Disjoint With | Atribut terdefinisi. | — |
| Name | Sama dengan label. | — |

<details open>
<summary>Contoh data</summary>

| Label | Is Symmetric | Name ƒ |
|---|---|---|
| is parent of | false | is parent of |
| is child of | false | is child of |
| is spouse of | true | is spouse of |

_ƒ menandai kolom terhitung._

</details>

| Istilah | Deskripsi | Catatan Naratif |
|------|-------------|-------------------|
| **Statement** | A fact that is stated, or could be concluded: Subject, Property, Object. | — |
| Graph | Atribut terdefinisi. | — |
| Subject | Atribut terdefinisi. | — |
| Property | Atribut terdefinisi. | — |
| Object Thing | Atribut terdefinisi. | — |
| Number Value | Atribut terdefinisi. | — |
| Is Stated | Bernilai benar jika teks kosong. | _Drawn as a solid line in the explainer._ |
| Subject Label | Diambil dari subject yang tertaut. | — |
| Property Label | Diambil dari property yang tertaut. | — |
| Object Label | Diambil dari object thing yang tertaut. | — |
| Name | Dihitung sebagai subject label, diikuti spasi, diikuti property label, diikuti spasi, diikuti number value jika object thing kosong, dan dalam semua kasus lainnya object label. | — |
| Count of Inverse Firings | Nilai total inverse fired unit pada seluruh firings yang terkait dengan statement tersebut. | — |
| Count of Symmetric Firings | Nilai total symmetric fired unit pada seluruh firings yang terkait dengan statement tersebut. | — |
| Holds Before Chain | Bernilai benar jika paling sedikit satu dari kondisi berikut terpenuhi: penanda “stated” aktif, count of inverse firings lebih dari 0, atau count of symmetric firings lebih dari 0. | — |
| Count of Chain Firings | Nilai total chain fired unit pada seluruh firings yang terkait dengan statement tersebut. | — |
| Holds Before Typing | Bernilai benar jika paling sedikit satu dari kondisi berikut terpenuhi: penanda “holds before chain” aktif atau count of chain firings lebih dari 0. | — |
| Count of Type Firings | Nilai total type fired unit pada seluruh firings yang terkait dengan statement tersebut. | — |
| Holds | Bernilai benar jika paling sedikit satu dari kondisi berikut terpenuhi: penanda “holds before typing” aktif atau count of type firings lebih dari 0. | _True when stated or concluded by an enabled rule._ |
| Is Conclusion | Bernilai benar jika semua kondisi berikut terpenuhi: penanda “holds” aktif dan penanda “stated” tidak aktif. | _Drawn as a dashed line in the explainer._ |
| Worked Out by | Ditentukan menurut prioritas: teks kosong jika penanda “conclusion” tidak aktif; “inverse” jika count of inverse firings lebih dari 0; “symmetric” jika count of symmetric firings lebih dari 0; “property chain” jika count of chain firings lebih dari 0; dalam semua kasus lainnya, “domain and range”. | — |
| Stated Unit | Ditentukan menurut prioritas: 1 jika penanda “stated” aktif; dalam semua kasus lainnya, 0. | — |
| Conclusion Unit | Ditentukan menurut prioritas: 1 jika penanda “conclusion” aktif; dalam semua kasus lainnya, 0. | — |
| Parent Unit | Ditentukan menurut prioritas: 1 jika semua kondisi berikut terpenuhi: penanda “holds” aktif dan property adalah “isParentOf”; dalam semua kasus lainnya, 0. | — |
| Grandparent Unit | Ditentukan menurut prioritas: 1 jika semua kondisi berikut terpenuhi: penanda “holds” aktif dan property adalah “isGrandparentOf”; dalam semua kasus lainnya, 0. | — |

<details open>
<summary>Contoh data</summary>

| Number Value | Is Stated | Subject Label ƒ | Property Label ƒ | Object Label ƒ | Name ƒ | Count of Inverse Firings ƒ | Count of Symmetric Firings ƒ | Holds Before Chain ƒ | Count of Chain Firings ƒ | Holds Before Typing ƒ | Count of Type Firings ƒ | Holds ƒ | Is Conclusion ƒ | Worked Out by ƒ | Stated Unit ƒ | Conclusion Unit ƒ | Parent Unit ƒ | Grandparent Unit ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| — | true | Marvin | is parent of | Randy | Marvin is parent of Randy | 0 | 0 | true | 0 | true | 0 | true | false | — | 1 | 0 | 1 | 0 |
| — | false | Marvin | is parent of | Sharon | Marvin is parent of Sharon | 0 | 0 | false | 0 | false | 0 | false | false | — | 0 | 0 | 0 | 0 |
| — | false | Marvin | is parent of | Shelly | Marvin is parent of Shelly | 0 | 0 | false | 0 | false | 0 | false | false | — | 0 | 0 | 0 | 0 |

_ƒ menandai kolom terhitung._

</details>

| Istilah | Deskripsi | Catatan Naratif |
|------|-------------|-------------------|
| **Firing** | One way one rule could produce one fact from one or two others. | — |
| Rule | Atribut terdefinisi. | — |
| Kind | Atribut terdefinisi. | — |
| Target | Atribut terdefinisi. | — |
| First | Atribut terdefinisi. | — |
| Second | Atribut terdefinisi. | _Same as First when the rule needs one premise._ |
| Rule Enabled | Bernilai benar jika rule yang tertaut pada firing tersebut tergolong “enabled”. | — |
| Rule Label | Diambil dari rule yang tertaut. | — |
| Target Name | Diambil dari target yang tertaut. | — |
| First Name | Diambil dari first yang tertaut. | — |
| Second Name | Diambil dari second yang tertaut. | — |
| First is Stated | Bernilai benar jika first yang tertaut pada firing tersebut tergolong “stated”. | — |
| First Holds Before Chain | Bernilai benar jika first yang tertaut pada firing tersebut memenuhi kondisi “holds before chain”. | — |
| Second Holds Before Chain | Bernilai benar jika second yang tertaut pada firing tersebut memenuhi kondisi “holds before chain”. | — |
| First Holds Before Typing | Bernilai benar jika first yang tertaut pada firing tersebut memenuhi kondisi “holds before typing”. | — |
| Inverse Fired Unit | Ditentukan menurut prioritas: 1 jika semua kondisi berikut terpenuhi: penanda “rule enabled” aktif, kind adalah “inverse”, dan penanda “first is stated” aktif; dalam semua kasus lainnya, 0. | — |
| Symmetric Fired Unit | Ditentukan menurut prioritas: 1 jika semua kondisi berikut terpenuhi: penanda “rule enabled” aktif, kind adalah “symmetric”, dan penanda “first is stated” aktif; dalam semua kasus lainnya, 0. | — |
| Chain Fired Unit | Ditentukan menurut prioritas: 1 jika semua kondisi berikut terpenuhi: penanda “rule enabled” aktif, kind adalah “chain”, penanda “first holds before chain” aktif, dan penanda “second holds before chain” aktif; dalam semua kasus lainnya, 0. | — |
| Type Fired Unit | Ditentukan menurut prioritas: 1 jika semua kondisi berikut terpenuhi: penanda “rule enabled” aktif; paling sedikit satu dari kondisi berikut terpenuhi: kind adalah “domain” atau kind adalah “range”; dan penanda “first holds before typing” aktif; dalam semua kasus lainnya, 0. | — |
| Fired Unit | Dihitung sebagai inverse fired unit ditambah symmetric fired unit ditambah chain fired unit ditambah type fired unit. | — |
| Name | Ditentukan menurut prioritas: target name, diikuti “: worked out by the ”, diikuti rule label, diikuti “ rule from '”, diikuti first name, diikuti “'”, diikuti “ and '”, diikuti second name, diikuti “'” jika fired unit lebih dari 0; dalam semua kasus lainnya, firing ID. | _The explainer's conclusion card._ |

<details open>
<summary>Contoh data</summary>

| Kind | Rule Enabled ƒ | Rule Label ƒ | Target Name ƒ | First Name ƒ | Second Name ƒ | First is Stated ƒ | First Holds Before Chain ƒ | Second Holds Before Chain ƒ | First Holds Before Typing ƒ | Inverse Fired Unit ƒ | Symmetric Fired Unit ƒ | Chain Fired Unit ƒ | Type Fired Unit ƒ | Fired Unit ƒ | Name ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| inverse | true | Inverse | Randy is child of Marvin | Marvin is parent of Randy | Marvin is parent of Randy | true | true | true | true | 1 | 0 | 0 | 0 | 1 | Randy is child of Marvin: worked out by the Inverse rule from 'Marvin is parent of Randy' |
| inverse | true | Inverse | Sharon is child of Marvin | Marvin is parent of Sharon | Marvin is parent of Sharon | false | false | false | false | 0 | 0 | 0 | 0 | 0 | inverse:Marvin\|isParentOf\|Sharon>Sharon\|isChildOf\|Marvin |
| inverse | true | Inverse | Shelly is child of Marvin | Marvin is parent of Shelly | Marvin is parent of Shelly | false | false | false | false | 0 | 0 | 0 | 0 | 0 | inverse:Marvin\|isParentOf\|Shelly>Shelly\|isChildOf\|Marvin |

_ƒ menandai kolom terhitung._

</details>

| Istilah | Deskripsi | Catatan Naratif |
|------|-------------|-------------------|
| **Disjoint Check** | Two statements that must not both hold. | — |
| Graph | Atribut terdefinisi. | — |
| Rule | Atribut terdefinisi. | — |
| A | Atribut terdefinisi. | — |
| B | Atribut terdefinisi. | — |
| Name | Sama dengan disjoint check ID. | — |
| Rule Enabled | Bernilai benar jika rule yang tertaut pada disjoint check tersebut tergolong “enabled”. | — |
| A Holds | Bernilai benar jika a yang tertaut pada disjoint check tersebut memenuhi kondisi “holds”. | — |
| B Holds | Bernilai benar jika b yang tertaut pada disjoint check tersebut memenuhi kondisi “holds”. | — |
| Violation Unit | Ditentukan menurut prioritas: 1 jika semua kondisi berikut terpenuhi: penanda “rule enabled” aktif, penanda “a holds” aktif, dan penanda “b holds” aktif; dalam semua kasus lainnya, 0. | — |

<details open>
<summary>Contoh data</summary>

| Name ƒ | Rule Enabled ƒ | A Holds ƒ | B Holds ƒ | Violation Unit ƒ |
|---|---|---|---|---|
| Marvin\|isParentOf\|Randy~isSpouseOf | true | true | false | 0 |
| Marvin\|isParentOf\|Sharon~isSpouseOf | true | false | false | 0 |
| Marvin\|isParentOf\|Shelly~isSpouseOf | true | false | false | 0 |

_ƒ menandai kolom terhitung._

</details>

## 2 Jenis Fakta

- Setiap **property** dapat merujuk pada satu **property**
- Setiap **property** dapat merujuk pada satu **thing**
- Setiap **statement** merujuk pada tepat satu **graph**
- Setiap **statement** merujuk pada tepat satu **thing**
- Setiap **statement** merujuk pada tepat satu **property**
- Setiap **firing** merujuk pada tepat satu **rule**
- Setiap **firing** merujuk pada tepat satu **statement**
- Setiap **disjoint check** merujuk pada tepat satu **graph**
- Setiap **disjoint check** merujuk pada tepat satu **rule**
- Setiap **disjoint check** merujuk pada tepat satu **statement**

## 3 Aturan Operatif

_Aturan operatif menyatakan apa yang **diwajibkan**, **dilarang**, atau
dianjurkan (**sebaiknya**) oleh bisnis. Aturan struktural berasal dari kolom wajib dan kunci asing;
aturan semantik berasal dari tabel Constraints, masing-masing didasarkan pada nilai boolean yang sudah
dihitung oleh buku aturan (dirujuk silang sebagai DR-N dalam Aturan Definisional di bawah)._

### Batasan Struktural (dari skema)

- Setiap rule **harus** memiliki label dan explanation, serta menyatakan apakah tergolong “enabled”.
- Setiap thing **harus** memiliki label dan kind.
- Setiap property **harus** memiliki label, serta menyatakan apakah tergolong “symmetric”.
- Setiap statement **harus** merujuk pada tepat satu graph.
- Setiap statement **harus** merujuk pada tepat satu thing sebagai subject.
- Setiap statement **harus** merujuk pada tepat satu property.
- Setiap statement **harus** menyatakan apakah tergolong “stated”.
- Setiap firing **harus** merujuk pada tepat satu rule.
- Setiap firing **harus** merujuk pada tepat satu statement sebagai target.
- Setiap firing **harus** merujuk pada tepat satu statement sebagai first.
- Setiap firing **harus** merujuk pada tepat satu statement sebagai second.
- Setiap firing **harus** memiliki kind.
- Setiap disjoint check **harus** merujuk pada tepat satu graph.
- Setiap disjoint check **harus** merujuk pada tepat satu rule.
- Setiap disjoint check **harus** merujuk pada tepat satu statement sebagai a.
- Setiap disjoint check **harus** merujuk pada tepat satu statement sebagai b.

## 4 Aturan Definisional

_Semua pernyataan mengungkapkan kebenaran dalam domain bisnis; bukan prosedur
dan bukan pula perintah. “Jika dan hanya jika” dihindari dan diganti dengan “hanya jika” agar
syarat perlu yang searah tidak disalahartikan sebagai kesetaraan. Tanda
**⚠︎ mekanis** menandai aturan yang rumusan deterministiknya setia pada rumus
tetapi kaku — isyarat untuk penyuntingan ulang yang opsional, bukan suatu cacat._

| ID | Aturan deklaratif |
|----|------------------|
| **DR-1 Name** | Untuk setiap graph, name sama dengan graph ID. |
| **DR-2 Count of Stated Facts** | Untuk setiap graph, count of stated facts adalah nilai total stated unit pada seluruh statements yang terkait dengan graph tersebut. |
| **DR-3 Count of Conclusions** | Untuk setiap graph, count of conclusions adalah nilai total conclusion unit pada seluruh statements yang terkait dengan graph tersebut. |
| **DR-4 Count of Violations** | Untuk setiap graph, count of violations adalah nilai total violation unit pada seluruh disjoint checks yang terkait dengan graph tersebut. |
| **DR-5 Is Consistent** | Setiap graph dianggap “consistent” jika count of violations adalah 0. |
| **DR-6 Name** | Untuk setiap rule, name sama dengan label. |
| **DR-7 Count of Firings** | Untuk setiap rule, count of firings adalah nilai total fired unit pada seluruh firings yang terkait dengan rule tersebut. |
| **DR-8 Name** | Untuk setiap thing, name sama dengan label. |
| **DR-9 Count of Children** | Untuk setiap thing, count of children adalah nilai total parent unit pada seluruh statements yang terkait dengan thing tersebut. |
| **DR-10 Count of Grandchildren** | Untuk setiap thing, count of grandchildren adalah nilai total grandparent unit pada seluruh statements yang terkait dengan thing tersebut. |
| **DR-11 Name** | Untuk setiap property, name sama dengan label. |
| **DR-12 Subject Label** | Untuk setiap statement, subject label diambil dari subject yang tertaut. |
| **DR-13 Property Label** | Untuk setiap statement, property label diambil dari property yang tertaut. |
| **DR-14 Object Label** | Untuk setiap statement, object label diambil dari object thing yang tertaut. |
| **DR-15 Name** | Untuk setiap statement, name dihitung sebagai subject label, diikuti spasi, diikuti property label, diikuti spasi, diikuti number value jika object thing kosong, dan dalam semua kasus lainnya object label. |
| **DR-16 Count of Inverse Firings** | Untuk setiap statement, count of inverse firings adalah nilai total inverse fired unit pada seluruh firings yang terkait dengan statement tersebut. |
| **DR-17 Count of Symmetric Firings** | Untuk setiap statement, count of symmetric firings adalah nilai total symmetric fired unit pada seluruh firings yang terkait dengan statement tersebut. |
| **DR-18 Holds Before Chain** | Setiap statement dianggap memenuhi kondisi “holds before chain” jika paling sedikit satu dari kondisi berikut terpenuhi: penanda “stated” aktif, count of inverse firings lebih dari 0, atau count of symmetric firings lebih dari 0. |
| **DR-19 Count of Chain Firings** | Untuk setiap statement, count of chain firings adalah nilai total chain fired unit pada seluruh firings yang terkait dengan statement tersebut. |
| **DR-20 Holds Before Typing** | Setiap statement dianggap memenuhi kondisi “holds before typing” jika paling sedikit satu dari kondisi berikut terpenuhi: penanda “holds before chain” aktif atau count of chain firings lebih dari 0. |
| **DR-21 Count of Type Firings** | Untuk setiap statement, count of type firings adalah nilai total type fired unit pada seluruh firings yang terkait dengan statement tersebut. |
| **DR-22 Holds** | Setiap statement dianggap memenuhi kondisi “holds” jika paling sedikit satu dari kondisi berikut terpenuhi: penanda “holds before typing” aktif atau count of type firings lebih dari 0. |
| **DR-23 Is Conclusion** | Setiap statement dianggap “conclusion” jika semua kondisi berikut terpenuhi: penanda “holds” aktif dan penanda “stated” tidak aktif. |
| **DR-24 Worked Out by** | Untuk setiap statement, worked out by ditentukan menurut urutan prioritas berikut:<br>1. teks kosong, jika penanda “conclusion” tidak aktif;<br>2. “inverse”, jika count of inverse firings lebih dari 0;<br>3. “symmetric”, jika count of symmetric firings lebih dari 0;<br>4. “property chain”, jika count of chain firings lebih dari 0;<br>5. dalam semua kasus lainnya, “domain and range”. |
| **DR-25 Stated Unit** | Untuk setiap statement, stated unit ditentukan menurut urutan prioritas berikut:<br>1. 1, jika penanda “stated” aktif;<br>2. dalam semua kasus lainnya, 0. |
| **DR-26 Conclusion Unit** | Untuk setiap statement, conclusion unit ditentukan menurut urutan prioritas berikut:<br>1. 1, jika penanda “conclusion” aktif;<br>2. dalam semua kasus lainnya, 0. |
| **DR-27 Parent Unit** | Untuk setiap statement, parent unit ditentukan menurut urutan prioritas berikut:<br>1. 1, jika semua kondisi berikut terpenuhi: penanda “holds” aktif dan property adalah “isParentOf”;<br>2. dalam semua kasus lainnya, 0. |
| **DR-28 Grandparent Unit** | Untuk setiap statement, grandparent unit ditentukan menurut urutan prioritas berikut:<br>1. 1, jika semua kondisi berikut terpenuhi: penanda “holds” aktif dan property adalah “isGrandparentOf”;<br>2. dalam semua kasus lainnya, 0. |
| **DR-29 Rule Enabled** | Untuk setiap firing, rule enabled bernilai benar jika rule yang tertaut pada firing tersebut tergolong “enabled”. |
| **DR-30 Rule Label** | Untuk setiap firing, rule label diambil dari rule yang tertaut. |
| **DR-31 Target Name** | Untuk setiap firing, target name diambil dari target yang tertaut. |
| **DR-32 First Name** | Untuk setiap firing, first name diambil dari first yang tertaut. |
| **DR-33 Second Name** | Untuk setiap firing, second name diambil dari second yang tertaut. |
| **DR-34 First is Stated** | Untuk setiap firing, first is stated bernilai benar jika first yang tertaut pada firing tersebut tergolong “stated”. |
| **DR-35 First Holds Before Chain** | Untuk setiap firing, first holds before chain bernilai benar jika first yang tertaut pada firing tersebut memenuhi kondisi “holds before chain”. |
| **DR-36 Second Holds Before Chain** | Untuk setiap firing, second holds before chain bernilai benar jika second yang tertaut pada firing tersebut memenuhi kondisi “holds before chain”. |
| **DR-37 First Holds Before Typing** | Untuk setiap firing, first holds before typing bernilai benar jika first yang tertaut pada firing tersebut memenuhi kondisi “holds before typing”. |
| **DR-38 Inverse Fired Unit** | Untuk setiap firing, inverse fired unit ditentukan menurut urutan prioritas berikut:<br>1. 1, jika semua kondisi berikut terpenuhi: penanda “rule enabled” aktif, kind adalah “inverse”, dan penanda “first is stated” aktif;<br>2. dalam semua kasus lainnya, 0. |
| **DR-39 Symmetric Fired Unit** | Untuk setiap firing, symmetric fired unit ditentukan menurut urutan prioritas berikut:<br>1. 1, jika semua kondisi berikut terpenuhi: penanda “rule enabled” aktif, kind adalah “symmetric”, dan penanda “first is stated” aktif;<br>2. dalam semua kasus lainnya, 0. |
| **DR-40 Chain Fired Unit** | Untuk setiap firing, chain fired unit ditentukan menurut urutan prioritas berikut:<br>1. 1, jika semua kondisi berikut terpenuhi: penanda “rule enabled” aktif, kind adalah “chain”, penanda “first holds before chain” aktif, dan penanda “second holds before chain” aktif;<br>2. dalam semua kasus lainnya, 0. |
| **DR-41 Type Fired Unit** | Untuk setiap firing, type fired unit ditentukan menurut urutan prioritas berikut:<br>1. 1, jika semua kondisi berikut terpenuhi: penanda “rule enabled” aktif; paling sedikit satu dari kondisi berikut terpenuhi: kind adalah “domain” atau kind adalah “range”; dan penanda “first holds before typing” aktif;<br>2. dalam semua kasus lainnya, 0. |
| **DR-42 Fired Unit** | Untuk setiap firing, fired unit dihitung sebagai inverse fired unit ditambah symmetric fired unit ditambah chain fired unit ditambah type fired unit. |
| **DR-43 Name** | Untuk setiap firing, name ditentukan menurut urutan prioritas berikut:<br>1. target name, diikuti “: worked out by the ”, diikuti rule label, diikuti “ rule from '”, diikuti first name, diikuti “'”, diikuti “ and '”, diikuti second name, diikuti “'”, jika fired unit lebih dari 0;<br>2. dalam semua kasus lainnya, firing ID. |
| **DR-44 Name** | Untuk setiap disjoint check, name sama dengan disjoint check ID. |
| **DR-45 Rule Enabled** | Untuk setiap disjoint check, rule enabled bernilai benar jika rule yang tertaut pada disjoint check tersebut tergolong “enabled”. |
| **DR-46 A Holds** | Untuk setiap disjoint check, a holds bernilai benar jika a yang tertaut pada disjoint check tersebut memenuhi kondisi “holds”. |
| **DR-47 B Holds** | Untuk setiap disjoint check, b holds bernilai benar jika b yang tertaut pada disjoint check tersebut memenuhi kondisi “holds”. |
| **DR-48 Violation Unit** | Untuk setiap disjoint check, violation unit ditentukan menurut urutan prioritas berikut:<br>1. 1, jika semua kondisi berikut terpenuhi: penanda “rule enabled” aktif, penanda “a holds” aktif, dan penanda “b holds” aktif;<br>2. dalam semua kasus lainnya, 0. |

## 5 Ketertelusuran ke Skema

_Kolom ekspresi memuat definisi aturan dalam notasi Dokumentasi —
logika yang sama dengan yang disimpan dalam buku aturan, ditulis untuk pembaca bisnis._

| Elemen skema | Jenis | Ekspresi |
|----------------|------|------------|
| **Graphs.Name** | rumus | `GraphId` |
| **Graphs.CountOfStatedFacts** | agregasi | `Sum(Statements.StatedUnit via Graph)` |
| **Graphs.CountOfConclusions** | agregasi | `Sum(Statements.ConclusionUnit via Graph)` |
| **Graphs.CountOfViolations** | agregasi | `Sum(DisjointChecks.ViolationUnit via Graph)` |
| **Graphs.IsConsistent** | rumus | `CountOfViolations = 0` |
| **Rules.Name** | rumus | `Label` |
| **Rules.CountOfFirings** | agregasi | `Sum(Firings.FiredUnit via Rule)` |
| **Things.Name** | rumus | `Label` |
| **Things.CountOfChildren** | agregasi | `Sum(Statements.ParentUnit via Subject)` |
| **Things.CountOfGrandchildren** | agregasi | `Sum(Statements.GrandparentUnit via Subject)` |
| **Properties.Name** | rumus | `Label` |
| **Statements.SubjectLabel** | pencarian | `Lookup(Things.Label via Subject)` |
| **Statements.PropertyLabel** | pencarian | `Lookup(Properties.Label via Property)` |
| **Statements.ObjectLabel** | pencarian | `Lookup(Things.Label via ObjectThing)` |
| **Statements.Name** | rumus | `SubjectLabel & " " & PropertyLabel & " " & If(ObjectThing = Blank(), NumberValue, ObjectLabel)` |
| **Statements.CountOfInverseFirings** | agregasi | `Sum(Firings.InverseFiredUnit via Target)` |
| **Statements.CountOfSymmetricFirings** | agregasi | `Sum(Firings.SymmetricFiredUnit via Target)` |
| **Statements.HoldsBeforeChain** | rumus | `Or(IsStated, CountOfInverseFirings > 0, CountOfSymmetricFirings > 0)` |
| **Statements.CountOfChainFirings** | agregasi | `Sum(Firings.ChainFiredUnit via Target)` |
| **Statements.HoldsBeforeTyping** | rumus | `Or(HoldsBeforeChain, CountOfChainFirings > 0)` |
| **Statements.CountOfTypeFirings** | agregasi | `Sum(Firings.TypeFiredUnit via Target)` |
| **Statements.Holds** | rumus | `Or(HoldsBeforeTyping, CountOfTypeFirings > 0)` |
| **Statements.IsConclusion** | rumus | `And(Holds, Not(IsStated))` |
| **Statements.WorkedOutBy** | rumus | `If(Not(IsConclusion), "", If(CountOfInverseFirings > 0, "inverse", If(CountOfSymmetricFirings > 0, "symmetric", If(CountOfChainFirings > 0, "property chain", "domain and range"))))` |
| **Statements.StatedUnit** | rumus | `If(IsStated, 1, 0)` |
| **Statements.ConclusionUnit** | rumus | `If(IsConclusion, 1, 0)` |
| **Statements.ParentUnit** | rumus | `If(And(Holds, Property = "isParentOf"), 1, 0)` |
| **Statements.GrandparentUnit** | rumus | `If(And(Holds, Property = "isGrandparentOf"), 1, 0)` |
| **Firings.RuleEnabled** | pencarian | `Lookup(Rules.IsEnabled via Rule)` |
| **Firings.RuleLabel** | pencarian | `Lookup(Rules.Label via Rule)` |
| **Firings.TargetName** | pencarian | `Lookup(Statements.Name via Target)` |
| **Firings.FirstName** | pencarian | `Lookup(Statements.Name via First)` |
| **Firings.SecondName** | pencarian | `Lookup(Statements.Name via Second)` |
| **Firings.FirstIsStated** | pencarian | `Lookup(Statements.IsStated via First)` |
| **Firings.FirstHoldsBeforeChain** | pencarian | `Lookup(Statements.HoldsBeforeChain via First)` |
| **Firings.SecondHoldsBeforeChain** | pencarian | `Lookup(Statements.HoldsBeforeChain via Second)` |
| **Firings.FirstHoldsBeforeTyping** | pencarian | `Lookup(Statements.HoldsBeforeTyping via First)` |
| **Firings.InverseFiredUnit** | rumus | `If(And(RuleEnabled, Kind = "inverse", FirstIsStated), 1, 0)` |
| **Firings.SymmetricFiredUnit** | rumus | `If(And(RuleEnabled, Kind = "symmetric", FirstIsStated), 1, 0)` |
| **Firings.ChainFiredUnit** | rumus | `If(And(RuleEnabled, Kind = "chain", FirstHoldsBeforeChain, SecondHoldsBeforeChain), 1, 0)` |
| **Firings.TypeFiredUnit** | rumus | `If(And(RuleEnabled, Or(Kind = "domain", Kind = "range"), FirstHoldsBeforeTyping), 1, 0)` |
| **Firings.FiredUnit** | rumus | `InverseFiredUnit + SymmetricFiredUnit + ChainFiredUnit + TypeFiredUnit` |
| **Firings.Name** | rumus | `If(FiredUnit > 0, TargetName & ": worked out by the " & RuleLabel & " rule from '" & FirstName & "'" & If(Kind = "chain", " and '" & SecondName & "'", ""), FiringId)` |
| **DisjointChecks.Name** | rumus | `DisjointCheckId` |
| **DisjointChecks.RuleEnabled** | pencarian | `Lookup(Rules.IsEnabled via Rule)` |
| **DisjointChecks.AHolds** | pencarian | `Lookup(Statements.Holds via A)` |
| **DisjointChecks.BHolds** | pencarian | `Lookup(Statements.Holds via B)` |
| **DisjointChecks.ViolationUnit** | rumus | `If(And(RuleEnabled, AHolds, BHolds), 1, 0)` |
