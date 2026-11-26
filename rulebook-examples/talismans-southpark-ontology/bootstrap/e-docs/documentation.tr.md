# 📘 South Park ontology explainer — Dokümantasyon

_The barebones South Park ontology from Jessica Talisman's explainer: ten stated facts and five rules that can each be switched on. Conclusions are formulas._

> Kural kitabından üretilmiş bildirimsel iş kuralları. Aşağıdaki her ifade
> iş bağlamındaki bir doğruyu dile getirir; ne bir prosedürdür ne de bir buyruk.
> Kural kitabındaki formüller tek doğruluk kaynağıdır;
> bu belge onların yalın dildeki okunuşudur.

## 1 İş Terimleri Sözlüğü

| Terim | Açıklama | Açıklayıcı Not |
|------|-------------|-------------------|
| **Graph** | The whole example. | — |
| Name | “graph ID” alanı ile aynı değeri taşır. | — |
| Count of Stated Facts | “graph” kaydıyla ilişkili “statements” kayıtları üzerinden hesaplanan toplam “stated unit” değeri. | — |
| Count of Conclusions | “graph” kaydıyla ilişkili “statements” kayıtları üzerinden hesaplanan toplam “conclusion unit” değeri. | _Facts that hold but were never stated._ |
| Count of Violations | “graph” kaydıyla ilişkili “disjoint checks” kayıtları üzerinden hesaplanan toplam “violation unit” değeri. | — |
| Is Consistent | Doğru olma koşulu: “count of violations” alanı 0 değerine eşit. | — |

<details open>
<summary>Örnek veri</summary>

| Name ƒ | Count of Stated Facts ƒ | Count of Conclusions ƒ | Count of Violations ƒ | Is Consistent ƒ |
|---|---|---|---|---|
| south-park | 11 | 9 | 0 | true |

_ƒ, hesaplanan bir sütunu gösterir._

</details>

| Terim | Açıklama | Açıklayıcı Not |
|------|-------------|-------------------|
| **Rule** | The five rules; IsEnabled is the checkbox. | — |
| Label | Tanımlı bir öznitelik. | — |
| Explanation | Tanımlı bir öznitelik. | — |
| Is Enabled | Doğru olma koşulu: boş metin. | — |
| Name | “label” alanı ile aynı değeri taşır. | — |
| Count of Firings | “rule” kaydıyla ilişkili “firings” kayıtları üzerinden hesaplanan toplam “fired unit” değeri. | _How many times the rule produced a fact._ |

<details open>
<summary>Örnek veri</summary>

| Label | Explanation | Is Enabled | Name ƒ | Count of Firings ƒ |
|---|---|---|---|---|
| Inverse | A parent link also gives a child link between the same two people. | true | Inverse | 4 |
| Symmetric | A friend link or a spouse link is true for both people. | true | Symmetric | 2 |
| Property chain | A parent of a parent is a grandparent. | true | Property chain | 2 |

_ƒ, hesaplanan bir sütunu gösterir._

</details>

| Terim | Açıklama | Açıklayıcı Not |
|------|-------------|-------------------|
| **Thing** | A node in the picture: a person, a place, a class or a role value. | — |
| Label | Tanımlı bir öznitelik. | — |
| Kind | Tanımlı bir öznitelik. | — |
| Name | “label” alanı ile aynı değeri taşır. | — |
| Count of Children | “thing” kaydıyla ilişkili “statements” kayıtları üzerinden hesaplanan toplam “parent unit” değeri. | _Added: a count, which a reasoner alone does not give._ |
| Count of Grandchildren | “thing” kaydıyla ilişkili “statements” kayıtları üzerinden hesaplanan toplam “grandparent unit” değeri. | _Added: counts concluded grandchildren._ |

<details open>
<summary>Örnek veri</summary>

| Label | Kind | Name ƒ | Count of Children ƒ | Count of Grandchildren ƒ |
|---|---|---|---|---|
| Marvin | individual | Marvin | 1 | 2 |
| Randy | individual | Randy | 2 | 0 |
| Sharon | individual | Sharon | 1 | 0 |

_ƒ, hesaplanan bir sütunu gösterir._

</details>

| Terim | Açıklama | Açıklayıcı Not |
|------|-------------|-------------------|
| **Property** | A kind of link, with the characteristics the rules use. | — |
| Label | Tanımlı bir öznitelik. | — |
| Inverse of | Tanımlı bir öznitelik. | — |
| Is Symmetric | Doğru olma koşulu: boş metin. | — |
| Chain First | Tanımlı bir öznitelik. | — |
| Chain Second | Tanımlı bir öznitelik. | — |
| Domain Class | Tanımlı bir öznitelik. | — |
| Range Class | Tanımlı bir öznitelik. | — |
| Disjoint With | Tanımlı bir öznitelik. | — |
| Name | “label” alanı ile aynı değeri taşır. | — |

<details open>
<summary>Örnek veri</summary>

| Label | Is Symmetric | Name ƒ |
|---|---|---|
| is parent of | false | is parent of |
| is child of | false | is child of |
| is spouse of | true | is spouse of |

_ƒ, hesaplanan bir sütunu gösterir._

</details>

| Terim | Açıklama | Açıklayıcı Not |
|------|-------------|-------------------|
| **Statement** | A fact that is stated, or could be concluded: Subject, Property, Object. | — |
| Graph | Tanımlı bir öznitelik. | — |
| Subject | Tanımlı bir öznitelik. | — |
| Property | Tanımlı bir öznitelik. | — |
| Object Thing | Tanımlı bir öznitelik. | — |
| Number Value | Tanımlı bir öznitelik. | — |
| Is Stated | Doğru olma koşulu: boş metin. | _Drawn as a solid line in the explainer._ |
| Subject Label | “subject” alanı üzerinden bağlı kayıttan alınır. | — |
| Property Label | “property” alanı üzerinden bağlı kayıttan alınır. | — |
| Object Label | “object thing” alanı üzerinden bağlı kayıttan alınır. | — |
| Name | Hesaplanan değer: “subject label” alanı, ardından bir boşluk, ardından “property label” alanı, ardından bir boşluk, ardından “object thing” alanı boş ise “number value” alanı, diğer tüm durumlarda “object label” alanı. | — |
| Count of Inverse Firings | “statement” kaydıyla ilişkili “firings” kayıtları üzerinden hesaplanan toplam “inverse fired unit” değeri. | — |
| Count of Symmetric Firings | “statement” kaydıyla ilişkili “firings” kayıtları üzerinden hesaplanan toplam “symmetric fired unit” değeri. | — |
| Holds Before Chain | Doğru olma koşulu: “stated” bayrağı etkin, “count of inverse firings” alanı 0 değerinden büyük veya “count of symmetric firings” alanı 0 değerinden büyük. | — |
| Count of Chain Firings | “statement” kaydıyla ilişkili “firings” kayıtları üzerinden hesaplanan toplam “chain fired unit” değeri. | — |
| Holds Before Typing | Doğru olma koşulu: “holds before chain” bayrağı etkin veya “count of chain firings” alanı 0 değerinden büyük. | — |
| Count of Type Firings | “statement” kaydıyla ilişkili “firings” kayıtları üzerinden hesaplanan toplam “type fired unit” değeri. | — |
| Holds | Doğru olma koşulu: “holds before typing” bayrağı etkin veya “count of type firings” alanı 0 değerinden büyük. | _True when stated or concluded by an enabled rule._ |
| Is Conclusion | Doğru olma koşulu: “holds” bayrağı etkin ve “stated” bayrağı etkin değil. | _Drawn as a dashed line in the explainer._ |
| Worked Out by | Öncelik sırasına göre belirlenir: “conclusion” bayrağı etkin değil ise boş metin; “count of inverse firings” alanı 0 değerinden büyük ise “inverse”; “count of symmetric firings” alanı 0 değerinden büyük ise “symmetric”; “count of chain firings” alanı 0 değerinden büyük ise “property chain”; diğer tüm durumlarda “domain and range”. | — |
| Stated Unit | Öncelik sırasına göre belirlenir: “stated” bayrağı etkin ise 1; diğer tüm durumlarda 0. | — |
| Conclusion Unit | Öncelik sırasına göre belirlenir: “conclusion” bayrağı etkin ise 1; diğer tüm durumlarda 0. | — |
| Parent Unit | Öncelik sırasına göre belirlenir: “holds” bayrağı etkin ve “property” alanı “isParentOf” değerine eşit ise 1; diğer tüm durumlarda 0. | — |
| Grandparent Unit | Öncelik sırasına göre belirlenir: “holds” bayrağı etkin ve “property” alanı “isGrandparentOf” değerine eşit ise 1; diğer tüm durumlarda 0. | — |

<details open>
<summary>Örnek veri</summary>

| Number Value | Is Stated | Subject Label ƒ | Property Label ƒ | Object Label ƒ | Name ƒ | Count of Inverse Firings ƒ | Count of Symmetric Firings ƒ | Holds Before Chain ƒ | Count of Chain Firings ƒ | Holds Before Typing ƒ | Count of Type Firings ƒ | Holds ƒ | Is Conclusion ƒ | Worked Out by ƒ | Stated Unit ƒ | Conclusion Unit ƒ | Parent Unit ƒ | Grandparent Unit ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| — | true | Marvin | is parent of | Randy | Marvin is parent of Randy | 0 | 0 | true | 0 | true | 0 | true | false | — | 1 | 0 | 1 | 0 |
| — | false | Marvin | is parent of | Sharon | Marvin is parent of Sharon | 0 | 0 | false | 0 | false | 0 | false | false | — | 0 | 0 | 0 | 0 |
| — | false | Marvin | is parent of | Shelly | Marvin is parent of Shelly | 0 | 0 | false | 0 | false | 0 | false | false | — | 0 | 0 | 0 | 0 |

_ƒ, hesaplanan bir sütunu gösterir._

</details>

| Terim | Açıklama | Açıklayıcı Not |
|------|-------------|-------------------|
| **Firing** | One way one rule could produce one fact from one or two others. | — |
| Rule | Tanımlı bir öznitelik. | — |
| Kind | Tanımlı bir öznitelik. | — |
| Target | Tanımlı bir öznitelik. | — |
| First | Tanımlı bir öznitelik. | — |
| Second | Tanımlı bir öznitelik. | _Same as First when the rule needs one premise._ |
| Rule Enabled | “rule” alanı üzerinden bağlı kayıt “enabled” ise doğru. | — |
| Rule Label | “rule” alanı üzerinden bağlı kayıttan alınır. | — |
| Target Name | “target” alanı üzerinden bağlı kayıttan alınır. | — |
| First Name | “first” alanı üzerinden bağlı kayıttan alınır. | — |
| Second Name | “second” alanı üzerinden bağlı kayıttan alınır. | — |
| First is Stated | “first” alanı üzerinden bağlı kayıt “stated” ise doğru. | — |
| First Holds Before Chain | “first” alanı üzerinden bağlı kayıt “holds before chain” olarak işaretli ise doğru. | — |
| Second Holds Before Chain | “second” alanı üzerinden bağlı kayıt “holds before chain” olarak işaretli ise doğru. | — |
| First Holds Before Typing | “first” alanı üzerinden bağlı kayıt “holds before typing” olarak işaretli ise doğru. | — |
| Inverse Fired Unit | Öncelik sırasına göre belirlenir: “rule enabled” bayrağı etkin, “kind” alanı “inverse” değerine eşit ve “first is stated” bayrağı etkin ise 1; diğer tüm durumlarda 0. | — |
| Symmetric Fired Unit | Öncelik sırasına göre belirlenir: “rule enabled” bayrağı etkin, “kind” alanı “symmetric” değerine eşit ve “first is stated” bayrağı etkin ise 1; diğer tüm durumlarda 0. | — |
| Chain Fired Unit | Öncelik sırasına göre belirlenir: “rule enabled” bayrağı etkin, “kind” alanı “chain” değerine eşit, “first holds before chain” bayrağı etkin ve “second holds before chain” bayrağı etkin ise 1; diğer tüm durumlarda 0. | — |
| Type Fired Unit | Öncelik sırasına göre belirlenir: “rule enabled” bayrağı etkin, (“kind” alanı “domain” değerine eşit veya “kind” alanı “range” değerine eşit) ve “first holds before typing” bayrağı etkin ise 1; diğer tüm durumlarda 0. | — |
| Fired Unit | Hesaplanan değer: “inverse fired unit” alanı artı “symmetric fired unit” alanı artı “chain fired unit” alanı artı “type fired unit” alanı. | — |
| Name | Öncelik sırasına göre belirlenir: “fired unit” alanı 0 değerinden büyük ise “target name” alanı, ardından “: concluded by the ”, ardından “rule label” alanı, ardından “ rule from '”, ardından “first name” alanı, ardından “'”, ardından “ and '”, ardından “second name” alanı, ardından “'”; diğer tüm durumlarda “firing ID”. | _The explainer's conclusion card._ |

<details open>
<summary>Örnek veri</summary>

| Kind | Rule Enabled ƒ | Rule Label ƒ | Target Name ƒ | First Name ƒ | Second Name ƒ | First is Stated ƒ | First Holds Before Chain ƒ | Second Holds Before Chain ƒ | First Holds Before Typing ƒ | Inverse Fired Unit ƒ | Symmetric Fired Unit ƒ | Chain Fired Unit ƒ | Type Fired Unit ƒ | Fired Unit ƒ | Name ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| inverse | true | Inverse | Randy is child of Marvin | Marvin is parent of Randy | Marvin is parent of Randy | true | true | true | true | 1 | 0 | 0 | 0 | 1 | Randy is child of Marvin: concluded by the Inverse rule from 'Marvin is parent of Randy' |
| inverse | true | Inverse | Sharon is child of Marvin | Marvin is parent of Sharon | Marvin is parent of Sharon | false | false | false | false | 0 | 0 | 0 | 0 | 0 | inverse:Marvin\|isParentOf\|Sharon>Sharon\|isChildOf\|Marvin |
| inverse | true | Inverse | Shelly is child of Marvin | Marvin is parent of Shelly | Marvin is parent of Shelly | false | false | false | false | 0 | 0 | 0 | 0 | 0 | inverse:Marvin\|isParentOf\|Shelly>Shelly\|isChildOf\|Marvin |

_ƒ, hesaplanan bir sütunu gösterir._

</details>

| Terim | Açıklama | Açıklayıcı Not |
|------|-------------|-------------------|
| **Disjoint Check** | Two statements that must not both hold. | — |
| Graph | Tanımlı bir öznitelik. | — |
| Rule | Tanımlı bir öznitelik. | — |
| A | Tanımlı bir öznitelik. | — |
| B | Tanımlı bir öznitelik. | — |
| Name | “disjoint check ID” alanı ile aynı değeri taşır. | — |
| Rule Enabled | “rule” alanı üzerinden bağlı kayıt “enabled” ise doğru. | — |
| A Holds | “a” alanı üzerinden bağlı kayıt “holds” olarak işaretli ise doğru. | — |
| B Holds | “b” alanı üzerinden bağlı kayıt “holds” olarak işaretli ise doğru. | — |
| Violation Unit | Öncelik sırasına göre belirlenir: “rule enabled” bayrağı etkin, “a holds” bayrağı etkin ve “b holds” bayrağı etkin ise 1; diğer tüm durumlarda 0. | — |

<details open>
<summary>Örnek veri</summary>

| Name ƒ | Rule Enabled ƒ | A Holds ƒ | B Holds ƒ | Violation Unit ƒ |
|---|---|---|---|---|
| Marvin\|isParentOf\|Randy~isSpouseOf | true | true | false | 0 |
| Marvin\|isParentOf\|Sharon~isSpouseOf | true | false | false | 0 |
| Marvin\|isParentOf\|Shelly~isSpouseOf | true | false | false | 0 |

_ƒ, hesaplanan bir sütunu gösterir._

</details>

## 2 Olgu Türleri

- Her **property** kaydı bir **property** kaydına referans verebilir
- Her **property** kaydı bir **thing** kaydına referans verebilir
- Her **statement** kaydı tam olarak bir **graph** kaydına referans verir
- Her **statement** kaydı tam olarak bir **thing** kaydına referans verir
- Her **statement** kaydı tam olarak bir **property** kaydına referans verir
- Her **firing** kaydı tam olarak bir **rule** kaydına referans verir
- Her **firing** kaydı tam olarak bir **statement** kaydına referans verir
- Her **disjoint check** kaydı tam olarak bir **graph** kaydına referans verir
- Her **disjoint check** kaydı tam olarak bir **rule** kaydına referans verir
- Her **disjoint check** kaydı tam olarak bir **statement** kaydına referans verir

## 3 İşletimsel Kurallar

_İşletimsel kurallar, işletmenin neyi **zorunlu kıldığını**, neyi **yasakladığını** ve neyi
tavsiye ettiğini (**önerilir**) belirtir. Yapısal kurallar zorunlu alanlardan ve yabancı anahtarlardan türetilir;
anlamsal kurallar Constraints tablosundan gelir ve her biri kural kitabının zaten hesapladığı
bir mantıksal değere dayanır (aşağıdaki Tanımsal Kurallar bölümünde DR-N olarak çapraz başvurulur)._

### Yapısal Kısıtlar (şemadan)

- “rule” kaydı, “label” alanı ve “explanation” alanı dolu olmak, ayrıca “enabled” olup olmadığı bilgisini kaydetmek **zorundadır**.
- “thing” kaydı, “label” alanı ve “kind” alanı dolu olmak **zorundadır**.
- “property” kaydı, “label” alanı dolu olmak, ayrıca “symmetric” olup olmadığı bilgisini kaydetmek **zorundadır**.
- “statement” kaydı, tam olarak bir “graph” kaydına referans vermek **zorundadır**.
- “statement” kaydı, “subject” alanı aracılığıyla tam olarak bir “thing” kaydına referans vermek **zorundadır**.
- “statement” kaydı, tam olarak bir “property” kaydına referans vermek **zorundadır**.
- “statement” kaydı, “stated” olup olmadığı bilgisini kaydetmek **zorundadır**.
- “firing” kaydı, tam olarak bir “rule” kaydına referans vermek **zorundadır**.
- “firing” kaydı, “target” alanı aracılığıyla tam olarak bir “statement” kaydına referans vermek **zorundadır**.
- “firing” kaydı, “first” alanı aracılığıyla tam olarak bir “statement” kaydına referans vermek **zorundadır**.
- “firing” kaydı, “second” alanı aracılığıyla tam olarak bir “statement” kaydına referans vermek **zorundadır**.
- “firing” kaydı, “kind” alanı dolu olmak **zorundadır**.
- “disjoint check” kaydı, tam olarak bir “graph” kaydına referans vermek **zorundadır**.
- “disjoint check” kaydı, tam olarak bir “rule” kaydına referans vermek **zorundadır**.
- “disjoint check” kaydı, “a” alanı aracılığıyla tam olarak bir “statement” kaydına referans vermek **zorundadır**.
- “disjoint check” kaydı, “b” alanı aracılığıyla tam olarak bir “statement” kaydına referans vermek **zorundadır**.

## 4 Tanımsal Kurallar

_Tüm ifadeler iş bağlamındaki bir doğruyu dile getirir; ne prosedürdür ne de buyruk.
Tek yönlü bir gereklilik denklik sanılmasın diye “ancak ve ancak” yerine
“yalnızca … ise” kullanılır. **⚠︎ mekanik** etiketi, belirlenimci ifadesi aslına sadık
ama hantal olan bir kuralı gösterir — isteğe bağlı bir yeniden yazım turu için
bir işarettir, kusur değildir._

| Kural No | Bildirimsel kural |
|----|------------------|
| **DR-1 Name** | “graph” kaydının “name” alanı, “graph ID” alanı ile aynı değeri taşır. |
| **DR-2 Count of Stated Facts** | “graph” kaydının “count of stated facts” alanı, “graph” kaydıyla ilişkili “statements” kayıtları üzerinden hesaplanan toplam “stated unit” değeri olarak belirlenir. |
| **DR-3 Count of Conclusions** | “graph” kaydının “count of conclusions” alanı, “graph” kaydıyla ilişkili “statements” kayıtları üzerinden hesaplanan toplam “conclusion unit” değeri olarak belirlenir. |
| **DR-4 Count of Violations** | “graph” kaydının “count of violations” alanı, “graph” kaydıyla ilişkili “disjoint checks” kayıtları üzerinden hesaplanan toplam “violation unit” değeri olarak belirlenir. |
| **DR-5 Is Consistent** | “graph” kaydı, “count of violations” alanı 0 değerine eşit ise “consistent” sayılır. |
| **DR-6 Name** | “rule” kaydının “name” alanı, “label” alanı ile aynı değeri taşır. |
| **DR-7 Count of Firings** | “rule” kaydının “count of firings” alanı, “rule” kaydıyla ilişkili “firings” kayıtları üzerinden hesaplanan toplam “fired unit” değeri olarak belirlenir. |
| **DR-8 Name** | “thing” kaydının “name” alanı, “label” alanı ile aynı değeri taşır. |
| **DR-9 Count of Children** | “thing” kaydının “count of children” alanı, “thing” kaydıyla ilişkili “statements” kayıtları üzerinden hesaplanan toplam “parent unit” değeri olarak belirlenir. |
| **DR-10 Count of Grandchildren** | “thing” kaydının “count of grandchildren” alanı, “thing” kaydıyla ilişkili “statements” kayıtları üzerinden hesaplanan toplam “grandparent unit” değeri olarak belirlenir. |
| **DR-11 Name** | “property” kaydının “name” alanı, “label” alanı ile aynı değeri taşır. |
| **DR-12 Subject Label** | “statement” kaydının “subject label” alanı, “subject” alanı üzerinden bağlı kayıttan alınır. |
| **DR-13 Property Label** | “statement” kaydının “property label” alanı, “property” alanı üzerinden bağlı kayıttan alınır. |
| **DR-14 Object Label** | “statement” kaydının “object label” alanı, “object thing” alanı üzerinden bağlı kayıttan alınır. |
| **DR-15 Name** | “statement” kaydının “name” alanı, “subject label” alanı, ardından bir boşluk, ardından “property label” alanı, ardından bir boşluk, ardından “object thing” alanı boş ise “number value” alanı, diğer tüm durumlarda “object label” alanı olarak hesaplanır. |
| **DR-16 Count of Inverse Firings** | “statement” kaydının “count of inverse firings” alanı, “statement” kaydıyla ilişkili “firings” kayıtları üzerinden hesaplanan toplam “inverse fired unit” değeri olarak belirlenir. |
| **DR-17 Count of Symmetric Firings** | “statement” kaydının “count of symmetric firings” alanı, “statement” kaydıyla ilişkili “firings” kayıtları üzerinden hesaplanan toplam “symmetric fired unit” değeri olarak belirlenir. |
| **DR-18 Holds Before Chain** | “statement” kaydı, “stated” bayrağı etkin, “count of inverse firings” alanı 0 değerinden büyük veya “count of symmetric firings” alanı 0 değerinden büyük ise “holds before chain” koşulunu sağlamış sayılır. |
| **DR-19 Count of Chain Firings** | “statement” kaydının “count of chain firings” alanı, “statement” kaydıyla ilişkili “firings” kayıtları üzerinden hesaplanan toplam “chain fired unit” değeri olarak belirlenir. |
| **DR-20 Holds Before Typing** | “statement” kaydı, “holds before chain” bayrağı etkin veya “count of chain firings” alanı 0 değerinden büyük ise “holds before typing” koşulunu sağlamış sayılır. |
| **DR-21 Count of Type Firings** | “statement” kaydının “count of type firings” alanı, “statement” kaydıyla ilişkili “firings” kayıtları üzerinden hesaplanan toplam “type fired unit” değeri olarak belirlenir. |
| **DR-22 Holds** | “statement” kaydı, “holds before typing” bayrağı etkin veya “count of type firings” alanı 0 değerinden büyük ise “holds” koşulunu sağlamış sayılır. |
| **DR-23 Is Conclusion** | “statement” kaydı, “holds” bayrağı etkin ve “stated” bayrağı etkin değil ise “conclusion” sayılır. |
| **DR-24 Worked Out by** | “statement” kaydının “worked out by” alanı, aşağıdaki öncelik sırasına göre belirlenir:<br>1. “conclusion” bayrağı etkin değil ise boş metin;<br>2. “count of inverse firings” alanı 0 değerinden büyük ise “inverse”;<br>3. “count of symmetric firings” alanı 0 değerinden büyük ise “symmetric”;<br>4. “count of chain firings” alanı 0 değerinden büyük ise “property chain”;<br>5. diğer tüm durumlarda “domain and range”. |
| **DR-25 Stated Unit** | “statement” kaydının “stated unit” alanı, aşağıdaki öncelik sırasına göre belirlenir:<br>1. “stated” bayrağı etkin ise 1;<br>2. diğer tüm durumlarda 0. |
| **DR-26 Conclusion Unit** | “statement” kaydının “conclusion unit” alanı, aşağıdaki öncelik sırasına göre belirlenir:<br>1. “conclusion” bayrağı etkin ise 1;<br>2. diğer tüm durumlarda 0. |
| **DR-27 Parent Unit** | “statement” kaydının “parent unit” alanı, aşağıdaki öncelik sırasına göre belirlenir:<br>1. “holds” bayrağı etkin ve “property” alanı “isParentOf” değerine eşit ise 1;<br>2. diğer tüm durumlarda 0. |
| **DR-28 Grandparent Unit** | “statement” kaydının “grandparent unit” alanı, aşağıdaki öncelik sırasına göre belirlenir:<br>1. “holds” bayrağı etkin ve “property” alanı “isGrandparentOf” değerine eşit ise 1;<br>2. diğer tüm durumlarda 0. |
| **DR-29 Rule Enabled** | “firing” kaydının “rule enabled” alanı, “rule” alanı üzerinden bağlı kayıt “enabled” ise doğrudur. |
| **DR-30 Rule Label** | “firing” kaydının “rule label” alanı, “rule” alanı üzerinden bağlı kayıttan alınır. |
| **DR-31 Target Name** | “firing” kaydının “target name” alanı, “target” alanı üzerinden bağlı kayıttan alınır. |
| **DR-32 First Name** | “firing” kaydının “first name” alanı, “first” alanı üzerinden bağlı kayıttan alınır. |
| **DR-33 Second Name** | “firing” kaydının “second name” alanı, “second” alanı üzerinden bağlı kayıttan alınır. |
| **DR-34 First is Stated** | “firing” kaydının “first is stated” alanı, “first” alanı üzerinden bağlı kayıt “stated” ise doğrudur. |
| **DR-35 First Holds Before Chain** | “firing” kaydının “first holds before chain” alanı, “first” alanı üzerinden bağlı kayıt “holds before chain” olarak işaretli ise doğrudur. |
| **DR-36 Second Holds Before Chain** | “firing” kaydının “second holds before chain” alanı, “second” alanı üzerinden bağlı kayıt “holds before chain” olarak işaretli ise doğrudur. |
| **DR-37 First Holds Before Typing** | “firing” kaydının “first holds before typing” alanı, “first” alanı üzerinden bağlı kayıt “holds before typing” olarak işaretli ise doğrudur. |
| **DR-38 Inverse Fired Unit** | “firing” kaydının “inverse fired unit” alanı, aşağıdaki öncelik sırasına göre belirlenir:<br>1. “rule enabled” bayrağı etkin, “kind” alanı “inverse” değerine eşit ve “first is stated” bayrağı etkin ise 1;<br>2. diğer tüm durumlarda 0. |
| **DR-39 Symmetric Fired Unit** | “firing” kaydının “symmetric fired unit” alanı, aşağıdaki öncelik sırasına göre belirlenir:<br>1. “rule enabled” bayrağı etkin, “kind” alanı “symmetric” değerine eşit ve “first is stated” bayrağı etkin ise 1;<br>2. diğer tüm durumlarda 0. |
| **DR-40 Chain Fired Unit** | “firing” kaydının “chain fired unit” alanı, aşağıdaki öncelik sırasına göre belirlenir:<br>1. “rule enabled” bayrağı etkin, “kind” alanı “chain” değerine eşit, “first holds before chain” bayrağı etkin ve “second holds before chain” bayrağı etkin ise 1;<br>2. diğer tüm durumlarda 0. |
| **DR-41 Type Fired Unit** | “firing” kaydının “type fired unit” alanı, aşağıdaki öncelik sırasına göre belirlenir:<br>1. “rule enabled” bayrağı etkin, (“kind” alanı “domain” değerine eşit veya “kind” alanı “range” değerine eşit) ve “first holds before typing” bayrağı etkin ise 1;<br>2. diğer tüm durumlarda 0. |
| **DR-42 Fired Unit** | “firing” kaydının “fired unit” alanı, “inverse fired unit” alanı artı “symmetric fired unit” alanı artı “chain fired unit” alanı artı “type fired unit” alanı olarak hesaplanır. |
| **DR-43 Name** | “firing” kaydının “name” alanı, aşağıdaki öncelik sırasına göre belirlenir:<br>1. “fired unit” alanı 0 değerinden büyük ise “target name” alanı, ardından “: concluded by the ”, ardından “rule label” alanı, ardından “ rule from '”, ardından “first name” alanı, ardından “'”, ardından “ and '”, ardından “second name” alanı, ardından “'”;<br>2. diğer tüm durumlarda “firing ID”. |
| **DR-44 Name** | “disjoint check” kaydının “name” alanı, “disjoint check ID” alanı ile aynı değeri taşır. |
| **DR-45 Rule Enabled** | “disjoint check” kaydının “rule enabled” alanı, “rule” alanı üzerinden bağlı kayıt “enabled” ise doğrudur. |
| **DR-46 A Holds** | “disjoint check” kaydının “a holds” alanı, “a” alanı üzerinden bağlı kayıt “holds” olarak işaretli ise doğrudur. |
| **DR-47 B Holds** | “disjoint check” kaydının “b holds” alanı, “b” alanı üzerinden bağlı kayıt “holds” olarak işaretli ise doğrudur. |
| **DR-48 Violation Unit** | “disjoint check” kaydının “violation unit” alanı, aşağıdaki öncelik sırasına göre belirlenir:<br>1. “rule enabled” bayrağı etkin, “a holds” bayrağı etkin ve “b holds” bayrağı etkin ise 1;<br>2. diğer tüm durumlarda 0. |

## 5 Şemaya İzlenebilirlik

_İfade sütunu, kuralın Dokümantasyon gösterimindeki tanımıdır —
kural kitabının sakladığı mantığın aynısı, iş okuru için yazılmış biçimiyle._

| Şema öğesi | Tür | İfade |
|----------------|------|------------|
| **Graphs.Name** | formül | `GraphId` |
| **Graphs.CountOfStatedFacts** | özet | `Sum(Statements.StatedUnit via Graph)` |
| **Graphs.CountOfConclusions** | özet | `Sum(Statements.ConclusionUnit via Graph)` |
| **Graphs.CountOfViolations** | özet | `Sum(DisjointChecks.ViolationUnit via Graph)` |
| **Graphs.IsConsistent** | formül | `CountOfViolations = 0` |
| **Rules.Name** | formül | `Label` |
| **Rules.CountOfFirings** | özet | `Sum(Firings.FiredUnit via Rule)` |
| **Things.Name** | formül | `Label` |
| **Things.CountOfChildren** | özet | `Sum(Statements.ParentUnit via Subject)` |
| **Things.CountOfGrandchildren** | özet | `Sum(Statements.GrandparentUnit via Subject)` |
| **Properties.Name** | formül | `Label` |
| **Statements.SubjectLabel** | arama | `Lookup(Things.Label via Subject)` |
| **Statements.PropertyLabel** | arama | `Lookup(Properties.Label via Property)` |
| **Statements.ObjectLabel** | arama | `Lookup(Things.Label via ObjectThing)` |
| **Statements.Name** | formül | `SubjectLabel & " " & PropertyLabel & " " & If(ObjectThing = Blank(), NumberValue, ObjectLabel)` |
| **Statements.CountOfInverseFirings** | özet | `Sum(Firings.InverseFiredUnit via Target)` |
| **Statements.CountOfSymmetricFirings** | özet | `Sum(Firings.SymmetricFiredUnit via Target)` |
| **Statements.HoldsBeforeChain** | formül | `Or(IsStated, CountOfInverseFirings > 0, CountOfSymmetricFirings > 0)` |
| **Statements.CountOfChainFirings** | özet | `Sum(Firings.ChainFiredUnit via Target)` |
| **Statements.HoldsBeforeTyping** | formül | `Or(HoldsBeforeChain, CountOfChainFirings > 0)` |
| **Statements.CountOfTypeFirings** | özet | `Sum(Firings.TypeFiredUnit via Target)` |
| **Statements.Holds** | formül | `Or(HoldsBeforeTyping, CountOfTypeFirings > 0)` |
| **Statements.IsConclusion** | formül | `And(Holds, Not(IsStated))` |
| **Statements.WorkedOutBy** | formül | `If(Not(IsConclusion), "", If(CountOfInverseFirings > 0, "inverse", If(CountOfSymmetricFirings > 0, "symmetric", If(CountOfChainFirings > 0, "property chain", "domain and range"))))` |
| **Statements.StatedUnit** | formül | `If(IsStated, 1, 0)` |
| **Statements.ConclusionUnit** | formül | `If(IsConclusion, 1, 0)` |
| **Statements.ParentUnit** | formül | `If(And(Holds, Property = "isParentOf"), 1, 0)` |
| **Statements.GrandparentUnit** | formül | `If(And(Holds, Property = "isGrandparentOf"), 1, 0)` |
| **Firings.RuleEnabled** | arama | `Lookup(Rules.IsEnabled via Rule)` |
| **Firings.RuleLabel** | arama | `Lookup(Rules.Label via Rule)` |
| **Firings.TargetName** | arama | `Lookup(Statements.Name via Target)` |
| **Firings.FirstName** | arama | `Lookup(Statements.Name via First)` |
| **Firings.SecondName** | arama | `Lookup(Statements.Name via Second)` |
| **Firings.FirstIsStated** | arama | `Lookup(Statements.IsStated via First)` |
| **Firings.FirstHoldsBeforeChain** | arama | `Lookup(Statements.HoldsBeforeChain via First)` |
| **Firings.SecondHoldsBeforeChain** | arama | `Lookup(Statements.HoldsBeforeChain via Second)` |
| **Firings.FirstHoldsBeforeTyping** | arama | `Lookup(Statements.HoldsBeforeTyping via First)` |
| **Firings.InverseFiredUnit** | formül | `If(And(RuleEnabled, Kind = "inverse", FirstIsStated), 1, 0)` |
| **Firings.SymmetricFiredUnit** | formül | `If(And(RuleEnabled, Kind = "symmetric", FirstIsStated), 1, 0)` |
| **Firings.ChainFiredUnit** | formül | `If(And(RuleEnabled, Kind = "chain", FirstHoldsBeforeChain, SecondHoldsBeforeChain), 1, 0)` |
| **Firings.TypeFiredUnit** | formül | `If(And(RuleEnabled, Or(Kind = "domain", Kind = "range"), FirstHoldsBeforeTyping), 1, 0)` |
| **Firings.FiredUnit** | formül | `InverseFiredUnit + SymmetricFiredUnit + ChainFiredUnit + TypeFiredUnit` |
| **Firings.Name** | formül | `If(FiredUnit > 0, TargetName & ": concluded by the " & RuleLabel & " rule from '" & FirstName & "'" & If(Kind = "chain", " and '" & SecondName & "'", ""), FiringId)` |
| **DisjointChecks.Name** | formül | `DisjointCheckId` |
| **DisjointChecks.RuleEnabled** | arama | `Lookup(Rules.IsEnabled via Rule)` |
| **DisjointChecks.AHolds** | arama | `Lookup(Statements.Holds via A)` |
| **DisjointChecks.BHolds** | arama | `Lookup(Statements.Holds via B)` |
| **DisjointChecks.ViolationUnit** | formül | `If(And(RuleEnabled, AHolds, BHolds), 1, 0)` |
