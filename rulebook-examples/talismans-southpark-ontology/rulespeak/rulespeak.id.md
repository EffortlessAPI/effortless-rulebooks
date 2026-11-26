# 📘 Talisman's South Park ontology explainer — Dokumentasi

_Jessica Talisman's South Park Ontology Explainer, as a rulebook: the page's ten facts, five rule switches, property wiring, provenance wording and current-value policy are rows, and every conclusion is a formula. The rules apply in three fixed strata (inverse and symmetric, then the property chain, then domain and range); property wiring that feeds an earlier stratum would need repeated rounds._

> Aturan bisnis deklaratif yang disusun dari buku aturan. Setiap pernyataan
> di bawah ini mengungkapkan kebenaran dalam domain bisnis — bukan prosedur
> dan bukan pula perintah. Rumus dalam buku aturan merupakan satu-satunya sumber kebenaran;
> dokumen ini adalah pembacaannya dalam bahasa sehari-hari.

## 1 Kosakata Bisnis

| Istilah | Deskripsi | Catatan Naratif |
|------|-------------|-------------------|
| **Graph** | The explainer page as one graph: its wording variables and its totals. | — |
| Label | Atribut terdefinisi. | _Display name of the graph._ |
| Literal Quote | Atribut terdefinisi. | _The quote mark written around a literal value in a fact's wording._ |
| Literal Type Marker | Atribut terdefinisi. | _The marker written between a literal value and its datatype, as in Turtle._ |
| Built in Source Text | Atribut terdefinisi. | _Source line shown for a fact the page states as a teaching example._ |
| User Source Text | Atribut terdefinisi. | _Source line shown for a fact the reader stated._ |
| No Conclusions Text | Atribut terdefinisi. | _Message shown when the selected facts and rules add no conclusion._ |
| Name | Sama dengan label. | _Order 1. Display name of the graph._ |
| Count of Stated Facts | Nilai total in effect unit pada seluruh statements yang terkait dengan graph tersebut. | _Order 5. Facts that are switched on and in effect._ |
| Count of Conclusions | Nilai total conclusion unit pada seluruh statements yang terkait dengan graph tersebut. | _Order 18. Facts that hold only because an enabled rule concluded them._ |
| Count of Violations | Nilai total violation unit pada seluruh disjoint checks yang terkait dengan graph tersebut. | _Order 18. Disjointness checks that found both links between the same pair._ |
| Is Consistent | Bernilai benar jika count of violations adalah 0. | _Order 19. True when no enabled disjointness rule is violated._ |
| Conclusions Message | Ditentukan menurut prioritas: no conclusions text jika count of conclusions adalah 0; dalam semua kasus lainnya, teks kosong. | _Order 19. The empty-state message for the conclusions panel, blank when there are conclusions._ |
| Count of Facts to Add | Nilai total listed off unit pada seluruh statements yang terkait dengan graph tersebut. | _Order 5. Listed facts that are not in effect; the Add next fact button is enabled while this is above zero._ |
| Has Facts to Add | Bernilai benar jika count of facts to add lebih dari 0. | _Order 6. True while some listed fact can still be added._ |
| Next Fact Sort Order | Nilai minimum sort order pada seluruh statements dari graph tersebut yang tergolong “listed off unit”. | _Order 5. List position of the first listed fact that is not in effect._ |
| Max Activation Seq | Nilai maksimum activation seq pada seluruh statements yang terkait dengan graph tersebut. | _Order 1. Highest activation number used so far._ |
| Next Activation Seq | Dihitung sebagai max activation seq ditambah 1. | _Order 2. The activation number the next switched-on fact takes._ |
| Max Listed Sort Order | Nilai maksimum sort order pada seluruh statements dari graph tersebut yang tergolong “listed unit”. | _Order 2. Highest list position among listed facts._ |
| Next Listed Sort Order | Dihitung sebagai max listed sort order ditambah 1. | _Order 3. The list position a newly added fact takes._ |

<details open>
<summary>Contoh data</summary>

| Label | Literal Quote | Literal Type Marker | Built in Source Text | User Source Text | No Conclusions Text | Name ƒ | Count of Stated Facts ƒ | Count of Conclusions ƒ | Count of Violations ƒ | Is Consistent ƒ | Conclusions Message ƒ | Count of Facts to Add ƒ | Has Facts to Add ƒ | Next Fact Sort Order ƒ | Max Activation Seq ƒ | Next Activation Seq ƒ | Max Listed Sort Order ƒ | Next Listed Sort Order ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| South Park | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | These facts and selected rules add no further conclusions. Try another fact or rule. | South Park | 10 | 0 | 0 | true | These facts and selected rules add no further conclusions. Try another fact or rule. | 0 | false | — | 10 | 11 | 10 | 11 |

_ƒ menandai kolom terhitung._

</details>

| Istilah | Deskripsi | Catatan Naratif |
|------|-------------|-------------------|
| **Rule** | The five rule switches on the page. | — |
| Label | Atribut terdefinisi. | _The rule's name on the page._ |
| Explanation | Atribut terdefinisi. | _The rule's one-line explanation on the page._ |
| Is Enabled | Bernilai benar jika teks kosong. | _The rule's checkbox._ |
| Sort Order | Atribut terdefinisi. | _Position in the rule list._ |
| Name | Sama dengan label. | _Order 1. The rule's name._ |
| Count of Firings | Nilai total fired unit pada seluruh firings yang terkait dengan rule tersebut. | _Order 15. Rule applications that fired, counting every derivation._ |

<details open>
<summary>Contoh data</summary>

| Label | Explanation | Is Enabled | Sort Order | Name ƒ | Count of Firings ƒ |
|---|---|---|---|---|---|
| Inverse | A parent link also gives a child link between the same two people. | false | 1 | Inverse | 0 |
| Symmetric | A friend link or a spouse link is true for both people. | false | 2 | Symmetric | 0 |
| Property chain | A parent of a parent is a grandparent. | false | 3 | Property chain | 0 |

_ƒ menandai kolom terhitung._

</details>

| Istilah | Deskripsi | Catatan Naratif |
|------|-------------|-------------------|
| **Firing Kind** | The ways a rule can apply. Domain and range belong to one rule but are worded apart. | — |
| Rule | Atribut terdefinisi. | _The rule switch this kind belongs to._ |
| Wording | Atribut terdefinisi. | _How a conclusion card names the rule, as in 'Worked out by the inverse rule'._ |
| Sort Order | Atribut terdefinisi. | _Order in which the page's reasoner tries the kinds for one fact._ |
| Name | Sama dengan wording. | _Order 1. The kind's wording._ |
| Rule Enabled | Bernilai benar jika rule yang tertaut pada firing kind tersebut tergolong “enabled”. | _Order 1. Whether the owning rule is switched on._ |

<details open>
<summary>Contoh data</summary>

| Wording | Sort Order | Name ƒ | Rule Enabled ƒ |
|---|---|---|---|
| inverse rule | 1 | inverse rule | false |
| symmetric rule | 2 | symmetric rule | false |
| property chain rule | 3 | property chain rule | false |

_ƒ menandai kolom terhitung._

</details>

| Istilah | Deskripsi | Catatan Naratif |
|------|-------------|-------------------|
| **Thing** | Every individual and class the page can draw. | — |
| Label | Atribut terdefinisi. | _The name the page shows._ |
| Kind | Atribut terdefinisi. | _'individual' or 'class'._ |
| Sort Order | Atribut terdefinisi. | _Position in the cast._ |
| Portrait Name | Atribut terdefinisi. | _The name on the portrait's Fandom page._ |
| Portrait Page | Atribut terdefinisi. | _The Fandom page credited for the portrait._ |
| Portrait Image | Atribut terdefinisi. | _The original Fandom image._ |
| Portrait Crop | Atribut terdefinisi. | _The portrait's crop as an SVG viewBox: x y width height._ |
| Name | Sama dengan label. | _Order 1. The thing's name._ |
| Count of Children | Nilai total parent unit pada seluruh statements yang terkait dengan thing tersebut. | _Order 17. People this thing is a parent of, stated or concluded._ |
| Count of Grandchildren | Nilai total grandparent unit pada seluruh statements yang terkait dengan thing tersebut. | _Order 17. People this thing is a grandparent of._ |
| Count As Place | Nilai total place unit pada seluruh statements yang terkait dengan thing tersebut. | _Order 17. Holding attends facts whose object is this thing._ |
| Count As Subject | Nilai total holds unit pada seluruh statements yang terkait dengan thing tersebut. | _Order 17. Holding facts about this thing._ |
| Count As Object | Nilai total holds unit pada seluruh statements yang terkait dengan thing tersebut. | _Order 17. Holding facts that point at this thing._ |
| Is in Graph | Bernilai benar jika paling sedikit satu dari kondisi berikut terpenuhi: count as subject lebih dari 0 atau count as object lebih dari 0. | _Order 18. True when the graph draws this thing._ |
| Node Kind | Ditentukan menurut prioritas: “class” jika kind adalah “class”; “place” jika count as place lebih dari 0; dalam semua kasus lainnya, “person”. | _Order 18. How the graph draws the thing: a class, a place someone attends, or a person._ |

<details open>
<summary>Contoh data</summary>

| Label | Kind | Sort Order | Portrait Name | Portrait Page | Portrait Image | Portrait Crop | Name ƒ | Count of Children ƒ | Count of Grandchildren ƒ | Count As Place ƒ | Count As Subject ƒ | Count As Object ƒ | Is in Graph ƒ | Node Kind ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| Marvin | individual | 1 | Marvin Marsh | https://southpark.fandom.com/wiki/Marvin_Marsh | https://static.wikia.nocookie.net/southpark/images/d/db/Marsh-family-grandpa-marsh-w-purple-blanket.png/revision/latest | 48 -5 280 280 | Marvin | 1 | 0 | 0 | 1 | 0 | true | person |
| Randy | individual | 2 | Randy Marsh | https://southpark.fandom.com/wiki/Randy_Marsh | https://static.wikia.nocookie.net/southparkfanon/images/d/d6/Randy_Marsh.png/revision/latest | 0 -10 428 428 | Randy | 2 | 0 | 0 | 3 | 1 | true | person |
| Sharon | individual | 3 | Sharon Marsh | https://southpark.fandom.com/wiki/Sharon_Marsh | https://static.wikia.nocookie.net/southpark/images/9/99/Sharon_Marsh.png/revision/latest | 20 -10 390 390 | Sharon | 0 | 0 | 0 | 0 | 1 | true | person |

_ƒ menandai kolom terhitung._

</details>

| Istilah | Deskripsi | Catatan Naratif |
|------|-------------|-------------------|
| **Property** | The ten properties and the wiring every rule reads. | — |
| Label | Atribut terdefinisi. | _The property's wording on the page._ |
| Property Kind | Atribut terdefinisi. | _'object' links two things, 'datatype' links a thing to a value, 'type' states class membership._ |
| Datatype | Atribut terdefinisi. | _The value's XML Schema datatype for a datatype property; blank otherwise._ |
| Is User Addable | Bernilai benar jika teks kosong. | _Offered in the page's Add fact form._ |
| Has One Current Value | Bernilai benar jika teks kosong. | _The page keeps one current value per person: switching a value on replaces the earlier one._ |
| Inverse of | Atribut terdefinisi. | _The property the inverse rule derives from this one._ |
| Is Symmetric | Bernilai benar jika teks kosong. | _The symmetric rule makes this link true in both directions._ |
| Chain First | Atribut terdefinisi. | _First link of the property chain that derives this property._ |
| Chain Second | Atribut terdefinisi. | _Second link of the property chain that derives this property._ |
| Domain Class | Atribut terdefinisi. | _The class the domain rule gives this property's subject._ |
| Range Class | Atribut terdefinisi. | _The class the range rule gives this property's object._ |
| Disjoint With | Atribut terdefinisi. | _A property that may not link the same pair, in the same direction, as this one._ |
| Sort Order | Atribut terdefinisi. | _Position in the Add fact form's property list._ |
| Name | Sama dengan label. | _Order 1. The property's wording._ |

<details open>
<summary>Contoh data</summary>

| Label | Property Kind | Datatype | Is User Addable | Has One Current Value | Is Symmetric | Sort Order | Name ƒ |
|---|---|---|---|---|---|---|---|
| is parent of | object | — | true | false | false | 1 | is parent of |
| is child of | object | — | true | false | false | 2 | is child of |
| is spouse of | object | — | true | false | true | 3 | is spouse of |

_ƒ menandai kolom terhitung._

</details>

| Istilah | Deskripsi | Catatan Naratif |
|------|-------------|-------------------|
| **Statement** | Every fact the page can state or conclude about the cast. A listed fact appears in the page's fact list; the rest are candidates that only a rule can make hold. | — |
| Graph | Atribut terdefinisi. | _The graph the fact belongs to._ |
| Subject | Atribut terdefinisi. | _What the fact describes._ |
| Property | Atribut terdefinisi. | _How the subject connects to the object._ |
| Object Thing | Atribut terdefinisi. | _The related thing, for an object or type property._ |
| Object Value | Atribut terdefinisi. | _The literal value, for a datatype property._ |
| Is Listed | Bernilai benar jika teks kosong. | _Shown in the page's fact list._ |
| Is Built in | Bernilai benar jika teks kosong. | _One of the page's teaching facts; the reader cannot remove it._ |
| Is Detail | Bernilai benar jika teks kosong. | _One of Stan's detail facts that Show Stan's details switches on._ |
| Is Stated | Bernilai benar jika teks kosong. | _The fact's checkbox._ |
| Activation Seq | Atribut terdefinisi. | _When the fact was last switched on, as a running number; decides which value is current._ |
| Sort Order | Atribut terdefinisi. | _Position in the fact list; candidates follow the listed facts._ |
| Subject Label | Diambil dari subject yang tertaut. | _Order 1. The subject's name._ |
| Property Label | Diambil dari property yang tertaut. | _Order 1. The property's wording._ |
| Object Label | Diambil dari object thing yang tertaut. | _Order 1. The object's name, for an object or type property._ |
| Property Datatype | Diambil dari property yang tertaut. | _Order 1. The value's datatype, for a datatype property._ |
| Property Has One Current Value | Bernilai benar jika property yang tertaut pada statement tersebut memiliki “one current value”. | _Order 1. Whether the property keeps one current value per person._ |
| Property Domain Class | Diambil dari property yang tertaut. | _Order 1. The property's domain class._ |
| Property Range Class | Diambil dari property yang tertaut. | _Order 1. The property's range class._ |
| Literal Quote | Diambil dari graph yang tertaut. | _Order 1. The graph's literal quote mark._ |
| Literal Type Marker | Diambil dari graph yang tertaut. | _Order 1. The graph's datatype marker._ |
| Built in Source Text | Diambil dari graph yang tertaut. | _Order 1. The graph's source line for a teaching fact._ |
| User Source Text | Diambil dari graph yang tertaut. | _Order 1. The graph's source line for a reader's fact._ |
| Next Fact Sort Order | Diambil dari graph yang tertaut. | _Order 6. List position of the next fact to add._ |
| Is Literal | Bernilai benar jika object thing kosong. | _Order 1. True when the object is a value rather than a thing._ |
| Object Text | Ditentukan menurut prioritas: object value jika object thing kosong; dalam semua kasus lainnya, object label. | _Order 2. The object's name or the value._ |
| Name | Dihitung sebagai subject label, diikuti spasi, diikuti property label, diikuti spasi, diikuti object text. | _Order 3. The fact in words._ |
| Words | Dihitung sebagai subject label, diikuti spasi, diikuti property label, diikuti spasi, diikuti literal quote, diikuti object value, diikuti literal quote, diikuti literal type marker, diikuti property datatype jika object thing kosong, dan dalam semua kasus lainnya object label. ⚠︎ mekanis <!-- rulespeak:reword --> | _Order 2. The fact as a conclusion card quotes it, with a literal written in Turtle form._ |
| Source | Ditentukan menurut prioritas: built in source text jika penanda “built in” aktif; dalam semua kasus lainnya, user source text. | _Order 2. Where a listed fact came from._ |
| Subject Property Key | Dihitung sebagai subject, diikuti “|”, diikuti property. | _Order 1. Groups the values one subject has for one property._ |
| Latest Sibling Activation Seq | Nilai maksimum activation seq pada seluruh statements yang terkait dengan statement tersebut. | _Order 2. The most recent activation among this subject's values for this property._ |
| Is in Effect | Bernilai benar jika semua kondisi berikut terpenuhi: penanda “stated” aktif dan paling sedikit satu dari kondisi berikut terpenuhi: penanda “property has one current value” tidak aktif atau activation seq adalah latest sibling activation seq. | _Order 3. Switched on and, for a one-value property, the most recently switched-on value. Drawn as a solid line._ |
| Listed Unit | Ditentukan menurut prioritas: 1 jika penanda “listed” aktif; dalam semua kasus lainnya, 0. | _Order 1. 1 when the fact is in the fact list._ |
| In Effect Unit | Ditentukan menurut prioritas: 1 jika penanda “in effect” aktif; dalam semua kasus lainnya, 0. | _Order 4. 1 when the fact is in effect._ |
| Listed Off Unit | Ditentukan menurut prioritas: 1 jika semua kondisi berikut terpenuhi: penanda “listed” aktif dan penanda “in effect” tidak aktif; dalam semua kasus lainnya, 0. | _Order 4. 1 when the fact is listed but not in effect._ |
| Is Next to Add | Bernilai benar jika semua kondisi berikut terpenuhi: listed off unit adalah 1 dan sort order adalah next fact sort order. | _Order 7. The fact the Add next fact button switches on._ |
| Count of Inverse Firings | Nilai total inverse fired unit pada seluruh firings yang terkait dengan statement tersebut. | _Order 6. Inverse rule applications that conclude this fact._ |
| Count of Symmetric Firings | Nilai total symmetric fired unit pada seluruh firings yang terkait dengan statement tersebut. | _Order 6. Symmetric rule applications that conclude this fact._ |
| Holds Before Chain | Bernilai benar jika paling sedikit satu dari kondisi berikut terpenuhi: penanda “in effect” aktif, count of inverse firings lebih dari 0, atau count of symmetric firings lebih dari 0. | _Order 7. Holds from facts in effect plus the inverse and symmetric rules._ |
| Count of Chain Firings | Nilai total chain fired unit pada seluruh firings yang terkait dengan statement tersebut. | _Order 10. Property chain applications that conclude this fact._ |
| Holds Before Typing | Bernilai benar jika paling sedikit satu dari kondisi berikut terpenuhi: penanda “holds before chain” aktif atau count of chain firings lebih dari 0. | _Order 11. Holds before the domain and range rules apply._ |
| Count of Type Firings | Nilai total type fired unit pada seluruh firings yang terkait dengan statement tersebut. | _Order 14. Domain and range applications that conclude this fact._ |
| Holds | Bernilai benar jika paling sedikit satu dari kondisi berikut terpenuhi: penanda “holds before typing” aktif atau count of type firings lebih dari 0. | _Order 15. True when in effect or concluded by an enabled rule; the graph draws it._ |
| Is Conclusion | Bernilai benar jika semua kondisi berikut terpenuhi: penanda “holds” aktif dan penanda “in effect” tidak aktif. | _Order 16. Holds only because a rule concluded it. Drawn dashed._ |
| Holds Unit | Ditentukan menurut prioritas: 1 jika penanda “holds” aktif; dalam semua kasus lainnya, 0. | _Order 16. 1 when the fact holds._ |
| Conclusion Unit | Ditentukan menurut prioritas: 1 jika penanda “conclusion” aktif; dalam semua kasus lainnya, 0. | _Order 17. 1 when the fact is a conclusion._ |
| Parent Unit | Ditentukan menurut prioritas: 1 jika semua kondisi berikut terpenuhi: penanda “holds” aktif dan property adalah “isParentOf”; dalam semua kasus lainnya, 0. | _Order 16. 1 for a holding parent link._ |
| Grandparent Unit | Ditentukan menurut prioritas: 1 jika semua kondisi berikut terpenuhi: penanda “holds” aktif dan property adalah “isGrandparentOf”; dalam semua kasus lainnya, 0. | _Order 16. 1 for a holding grandparent link._ |
| Place Unit | Ditentukan menurut prioritas: 1 jika semua kondisi berikut terpenuhi: penanda “holds” aktif dan property adalah “attends”; dalam semua kasus lainnya, 0. | _Order 16. 1 for a holding attends link._ |
| First Fired Order Key | Nilai minimum order key pada seluruh firings dari statement tersebut yang tergolong “fired unit”. | _Order 15. Order key of the first rule application that concludes this fact, in the page reasoner's order._ |
| Count of Chosen Firings | Nilai total chosen unit pada seluruh firings yang terkait dengan statement tersebut. | _Order 20. Rule applications shown as this fact's conclusion card: one for a conclusion, zero otherwise._ |

<details open>
<summary>Contoh data</summary>

| Object Value | Is Listed | Is Built in | Is Detail | Is Stated | Activation Seq | Sort Order | Subject Label ƒ | Property Label ƒ | Object Label ƒ | Property Datatype ƒ | Property Has One Current Value ƒ | Property Domain Class ƒ | Property Range Class ƒ | Literal Quote ƒ | Literal Type Marker ƒ | Built in Source Text ƒ | User Source Text ƒ | Next Fact Sort Order ƒ | Is Literal ƒ | Object Text ƒ | Name ƒ | Words ƒ | Source ƒ | Subject Property Key ƒ | Latest Sibling Activation Seq ƒ | Is in Effect ƒ | Listed Unit ƒ | In Effect Unit ƒ | Listed Off Unit ƒ | Is Next to Add ƒ | Count of Inverse Firings ƒ | Count of Symmetric Firings ƒ | Holds Before Chain ƒ | Count of Chain Firings ƒ | Holds Before Typing ƒ | Count of Type Firings ƒ | Holds ƒ | Is Conclusion ƒ | Holds Unit ƒ | Conclusion Unit ƒ | Parent Unit ƒ | Grandparent Unit ƒ | Place Unit ƒ | First Fired Order Key ƒ | Count of Chosen Firings ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| — | true | true | false | true | 1 | 1 | Marvin | is parent of | Randy | — | false | — | — | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | — | false | Randy | Marvin is parent of Randy | Marvin is parent of Randy | Stated as a teaching example on this page. | Marvin\|isParentOf | 1 | true | 1 | 1 | 0 | false | 0 | 0 | true | 0 | true | 0 | true | false | 1 | 0 | 1 | 0 | 0 | — | 0 |
| — | false | false | false | false | 0 | 101 | Marvin | is parent of | Sharon | — | false | — | — | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | — | false | Sharon | Marvin is parent of Sharon | Marvin is parent of Sharon | Stated by you on this page. | Marvin\|isParentOf | 1 | false | 0 | 0 | 0 | false | 0 | 0 | false | 0 | false | 0 | false | false | 0 | 0 | 0 | 0 | 0 | — | 0 |
| — | false | false | false | false | 0 | 102 | Marvin | is parent of | Stan | — | false | — | — | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | — | false | Stan | Marvin is parent of Stan | Marvin is parent of Stan | Stated by you on this page. | Marvin\|isParentOf | 1 | false | 0 | 0 | 0 | false | 0 | 0 | false | 0 | false | 0 | false | false | 0 | 0 | 0 | 0 | 0 | — | 0 |

_ƒ menandai kolom terhitung._

</details>

| Istilah | Deskripsi | Catatan Naratif |
|------|-------------|-------------------|
| **Firing** | Every way an enabled rule could conclude a candidate fact from the facts that hold. | — |
| Kind | Atribut terdefinisi. | _How the rule applies._ |
| Rule | Atribut terdefinisi. | _The rule switch._ |
| Target | Atribut terdefinisi. | _The fact this application concludes._ |
| First | Atribut terdefinisi. | _The premise._ |
| Second | Atribut terdefinisi. | _The second premise, for the property chain._ |
| Rule Enabled | Bernilai benar jika rule yang tertaut pada firing tersebut tergolong “enabled”. | _Order 1. Whether the rule is switched on._ |
| Kind Wording | Diambil dari kind yang tertaut. | _Order 1. How the card names the rule._ |
| Kind Sort Order | Diambil dari kind yang tertaut. | _Order 1. The kind's place in the reasoner's order._ |
| Target Name | Diambil dari target yang tertaut. | _Order 4. The concluded fact in words._ |
| Target Subject Label | Diambil dari target yang tertaut. | _Order 2. The concluded fact's subject._ |
| Target Property Label | Diambil dari target yang tertaut. | _Order 2. The concluded fact's property._ |
| Target Object Text | Diambil dari target yang tertaut. | _Order 3. The concluded fact's object._ |
| Target is Conclusion | Bernilai benar jika target yang tertaut pada firing tersebut tergolong “conclusion”. | _Order 17. Whether the concluded fact is new._ |
| Target First Fired Order Key | Diambil dari target yang tertaut. | _Order 16. The order key of the concluded fact's first application._ |
| First Words | Diambil dari first yang tertaut. | _Order 3. The premise as the card quotes it._ |
| Second Words | Diambil dari second yang tertaut. | _Order 3. The second premise as the card quotes it._ |
| First is in Effect | Bernilai benar jika first yang tertaut pada firing tersebut tergolong “in effect”. | _Order 4. Whether the premise is in effect._ |
| First Holds Before Chain | Bernilai benar jika first yang tertaut pada firing tersebut memenuhi kondisi “holds before chain”. | _Order 8. Whether the premise holds before the chain._ |
| Second Holds Before Chain | Bernilai benar jika second yang tertaut pada firing tersebut memenuhi kondisi “holds before chain”. | _Order 8. Whether the second premise holds before the chain._ |
| First Holds Before Typing | Bernilai benar jika first yang tertaut pada firing tersebut memenuhi kondisi “holds before typing”. | _Order 12. Whether the premise holds before typing._ |
| First Property | Diambil dari first yang tertaut. | _Order 1. The premise's property key, as the card's axiom names it._ |
| First Property Domain Class | Diambil dari first yang tertaut. | _Order 2. The premise property's domain class._ |
| First Property Range Class | Diambil dari first yang tertaut. | _Order 2. The premise property's range class._ |
| First Sort Order | Diambil dari first yang tertaut. | _Order 1. The premise's list position._ |
| Second Sort Order | Diambil dari second yang tertaut. | _Order 1. The second premise's list position._ |
| Inverse Fired Unit | Ditentukan menurut prioritas: 1 jika semua kondisi berikut terpenuhi: penanda “rule enabled” aktif, kind adalah “inverse”, dan penanda “first is in effect” aktif; dalam semua kasus lainnya, 0. | _Order 5. 1 when the inverse rule fires here._ |
| Symmetric Fired Unit | Ditentukan menurut prioritas: 1 jika semua kondisi berikut terpenuhi: penanda “rule enabled” aktif, kind adalah “symmetric”, dan penanda “first is in effect” aktif; dalam semua kasus lainnya, 0. | _Order 5. 1 when the symmetric rule fires here._ |
| Chain Fired Unit | Ditentukan menurut prioritas: 1 jika semua kondisi berikut terpenuhi: penanda “rule enabled” aktif, kind adalah “chain”, penanda “first holds before chain” aktif, dan penanda “second holds before chain” aktif; dalam semua kasus lainnya, 0. | _Order 9. 1 when the property chain fires here._ |
| Type Fired Unit | Ditentukan menurut prioritas: 1 jika semua kondisi berikut terpenuhi: penanda “rule enabled” aktif; paling sedikit satu dari kondisi berikut terpenuhi: kind adalah “domain” atau kind adalah “range”; dan penanda “first holds before typing” aktif; dalam semua kasus lainnya, 0. | _Order 13. 1 when the domain or range rule fires here._ |
| Fired Unit | Dihitung sebagai inverse fired unit ditambah symmetric fired unit ditambah chain fired unit ditambah type fired unit. | _Order 14. 1 when this application fires._ |
| Order Key | Dihitung sebagai first sort order dikali 100000 ditambah kind sort order dikali 1000 ditambah 0 jika second kosong, dan dalam semua kasus lainnya second sort order. | _Order 2. The page reasoner's order: by premise, then rule kind, then second premise._ |
| Is Chosen | Bernilai benar jika semua kondisi berikut terpenuhi: fired unit adalah 1, penanda “target is conclusion” aktif, dan order key adalah target first fired order key. | _Order 18. The application the conclusions panel shows for its fact: the first one in the reasoner's order._ |
| Chosen Unit | Ditentukan menurut prioritas: 1 jika penanda “chosen” aktif; dalam semua kasus lainnya, 0. | _Order 19. 1 when this application is shown._ |
| Card Text | Ditentukan menurut prioritas: “Worked out by the ”, diikuti kind wording, diikuti “ from “”, diikuti first words, diikuti “””, diikuti “ and “”, diikuti second words, diikuti “”” jika kind adalah “chain”, dan dalam semua kasus lainnya “ and “”, diikuti first property, diikuti “ has domain ”, diikuti first property domain class, diikuti “”” jika kind adalah “domain”, dan dalam semua kasus lainnya “ and “”, diikuti first property, diikuti “ has range ”, diikuti first property range class, diikuti “””, diikuti tanda titik jika penanda “chosen” aktif; dalam semua kasus lainnya, teks kosong. ⚠︎ mekanis <!-- rulespeak:reword --> | _Order 19. The conclusion card's provenance line, in the page's wording._ |
| Name | Sama dengan firing ID. | _Order 1. The application's key._ |

<details open>
<summary>Contoh data</summary>

| Rule Enabled ƒ | Kind Wording ƒ | Kind Sort Order ƒ | Target Name ƒ | Target Subject Label ƒ | Target Property Label ƒ | Target Object Text ƒ | Target is Conclusion ƒ | Target First Fired Order Key ƒ | First Words ƒ | Second Words ƒ | First is in Effect ƒ | First Holds Before Chain ƒ | Second Holds Before Chain ƒ | First Holds Before Typing ƒ | First Property ƒ | First Property Domain Class ƒ | First Property Range Class ƒ | First Sort Order ƒ | Second Sort Order ƒ | Inverse Fired Unit ƒ | Symmetric Fired Unit ƒ | Chain Fired Unit ƒ | Type Fired Unit ƒ | Fired Unit ƒ | Order Key ƒ | Is Chosen ƒ | Chosen Unit ƒ | Card Text ƒ | Name ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| false | inverse rule | 1 | Randy is child of Marvin | Randy | is child of | Marvin | false | — | Marvin is parent of Randy |    | true | true | false | true | isParentOf | — | — | 1 | — | 0 | 0 | 0 | 0 | 0 | 101000 | false | 0 | — | inverse:Marvin\|isParentOf\|Randy>Randy\|isChildOf\|Marvin |
| false | inverse rule | 1 | Sharon is child of Marvin | Sharon | is child of | Marvin | false | — | Marvin is parent of Sharon |    | false | false | false | false | isParentOf | — | — | 101 | — | 0 | 0 | 0 | 0 | 0 | 10101000 | false | 0 | — | inverse:Marvin\|isParentOf\|Sharon>Sharon\|isChildOf\|Marvin |
| false | inverse rule | 1 | Stan is child of Marvin | Stan | is child of | Marvin | false | — | Marvin is parent of Stan |    | false | false | false | false | isParentOf | — | — | 102 | — | 0 | 0 | 0 | 0 | 0 | 10201000 | false | 0 | — | inverse:Marvin\|isParentOf\|Stan>Stan\|isChildOf\|Marvin |

_ƒ menandai kolom terhitung._

</details>

| Istilah | Deskripsi | Catatan Naratif |
|------|-------------|-------------------|
| **Disjoint Check** | Every pair the disjointness rule watches: the same two people linked by two disjoint properties. | — |
| Graph | Atribut terdefinisi. | _The graph._ |
| Rule | Atribut terdefinisi. | _The rule switch._ |
| First | Atribut terdefinisi. | _The first link._ |
| Second | Atribut terdefinisi. | _The disjoint link between the same pair, in the same direction._ |
| Rule Enabled | Bernilai benar jika rule yang tertaut pada disjoint check tersebut tergolong “enabled”. | _Order 1. Whether the disjointness rule is on._ |
| First Holds | Bernilai benar jika first yang tertaut pada disjoint check tersebut memenuhi kondisi “holds”. | _Order 16. Whether the first link holds._ |
| Second Holds | Bernilai benar jika second yang tertaut pada disjoint check tersebut memenuhi kondisi “holds”. | _Order 16. Whether the second link holds._ |
| Subject Label | Diambil dari first yang tertaut. | _Order 2. The person recorded with both links._ |
| Object Label | Diambil dari first yang tertaut. | _Order 2. The other person._ |
| Violation Unit | Ditentukan menurut prioritas: 1 jika semua kondisi berikut terpenuhi: penanda “rule enabled” aktif, penanda “first holds” aktif, dan penanda “second holds” aktif; dalam semua kasus lainnya, 0. | _Order 17. 1 when both links hold._ |
| Alarm Text | Ditentukan menurut prioritas: “Contradiction found. ”, diikuti subject label, diikuti “ is recorded as both parent and spouse of ”, diikuti object label, diikuti “. Check those facts and the selected disjoint rule.” jika violation unit adalah 1; dalam semua kasus lainnya, teks kosong. | _Order 18. The page's contradiction alert, blank when there is none._ |
| Name | Sama dengan disjoint check ID. | _Order 1. The check's key._ |

<details open>
<summary>Contoh data</summary>

| Rule Enabled ƒ | First Holds ƒ | Second Holds ƒ | Subject Label ƒ | Object Label ƒ | Violation Unit ƒ | Alarm Text ƒ | Name ƒ |
|---|---|---|---|---|---|---|---|
| false | true | false | Marvin | Randy | 0 | — | Marvin\|isParentOf\|Randy~isSpouseOf |
| false | false | false | Marvin | Sharon | 0 | — | Marvin\|isParentOf\|Sharon~isSpouseOf |
| false | false | false | Marvin | Stan | 0 | — | Marvin\|isParentOf\|Stan~isSpouseOf |

_ƒ menandai kolom terhitung._

</details>

## 2 Jenis Fakta

- Setiap **firing kind** merujuk pada tepat satu **rule**
- Setiap **property** dapat merujuk pada satu **property**
- Setiap **property** dapat merujuk pada satu **thing**
- Setiap **statement** merujuk pada tepat satu **graph**
- Setiap **statement** merujuk pada tepat satu **thing**
- Setiap **statement** merujuk pada tepat satu **property**
- Setiap **firing** merujuk pada tepat satu **firing kind**
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

- Setiap graph **harus** memiliki label, literal quote, literal type marker, built in source text, user source text, dan no conclusions text.
- Setiap rule **harus** memiliki label, explanation, dan sort order, serta menyatakan apakah tergolong “enabled”.
- Setiap firing kind **harus** merujuk pada tepat satu rule.
- Setiap firing kind **harus** memiliki wording dan sort order.
- Setiap thing **harus** memiliki label, kind, dan sort order.
- Setiap property **harus** memiliki label, property kind, dan sort order, serta menyatakan apakah tergolong “user addable”, apakah memiliki “one current value”, dan apakah tergolong “symmetric”.
- Setiap statement **harus** merujuk pada tepat satu graph.
- Setiap statement **harus** merujuk pada tepat satu thing sebagai subject.
- Setiap statement **harus** merujuk pada tepat satu property.
- Setiap statement **harus** memiliki activation seq dan sort order, serta menyatakan apakah tergolong “listed”, apakah tergolong “built in”, apakah tergolong “detail”, dan apakah tergolong “stated”.
- Setiap firing **harus** merujuk pada tepat satu firing kind sebagai kind.
- Setiap firing **harus** merujuk pada tepat satu rule.
- Setiap firing **harus** merujuk pada tepat satu statement sebagai target.
- Setiap firing **harus** merujuk pada tepat satu statement sebagai first.
- Setiap disjoint check **harus** merujuk pada tepat satu graph.
- Setiap disjoint check **harus** merujuk pada tepat satu rule.
- Setiap disjoint check **harus** merujuk pada tepat satu statement sebagai first.
- Setiap disjoint check **harus** merujuk pada tepat satu statement sebagai second.

## 4 Aturan Definisional

_Semua pernyataan mengungkapkan kebenaran dalam domain bisnis; bukan prosedur
dan bukan pula perintah. “Jika dan hanya jika” dihindari dan diganti dengan “hanya jika” agar
syarat perlu yang searah tidak disalahartikan sebagai kesetaraan. Tanda
**⚠︎ mekanis** menandai aturan yang rumusan deterministiknya setia pada rumus
tetapi kaku — isyarat untuk penyuntingan ulang yang opsional, bukan suatu cacat._

| ID | Aturan deklaratif |
|----|------------------|
| **DR-1 Name** | Untuk setiap graph, name sama dengan label. |
| **DR-2 Count of Stated Facts** | Untuk setiap graph, count of stated facts adalah nilai total in effect unit pada seluruh statements yang terkait dengan graph tersebut. |
| **DR-3 Count of Conclusions** | Untuk setiap graph, count of conclusions adalah nilai total conclusion unit pada seluruh statements yang terkait dengan graph tersebut. |
| **DR-4 Count of Violations** | Untuk setiap graph, count of violations adalah nilai total violation unit pada seluruh disjoint checks yang terkait dengan graph tersebut. |
| **DR-5 Is Consistent** | Setiap graph dianggap “consistent” jika count of violations adalah 0. |
| **DR-6 Conclusions Message** | Untuk setiap graph, conclusions message ditentukan menurut urutan prioritas berikut:<br>1. no conclusions text, jika count of conclusions adalah 0;<br>2. dalam semua kasus lainnya, teks kosong. |
| **DR-7 Count of Facts to Add** | Untuk setiap graph, count of facts to add adalah nilai total listed off unit pada seluruh statements yang terkait dengan graph tersebut. |
| **DR-8 Has Facts to Add** | Setiap graph dianggap memiliki “facts to add” jika count of facts to add lebih dari 0. |
| **DR-9 Next Fact Sort Order** | Untuk setiap graph, next fact sort order adalah nilai minimum sort order pada seluruh statements dari graph tersebut yang tergolong “listed off unit”. |
| **DR-10 Max Activation Seq** | Untuk setiap graph, max activation seq adalah nilai maksimum activation seq pada seluruh statements yang terkait dengan graph tersebut. |
| **DR-11 Next Activation Seq** | Untuk setiap graph, next activation seq dihitung sebagai max activation seq ditambah 1. |
| **DR-12 Max Listed Sort Order** | Untuk setiap graph, max listed sort order adalah nilai maksimum sort order pada seluruh statements dari graph tersebut yang tergolong “listed unit”. |
| **DR-13 Next Listed Sort Order** | Untuk setiap graph, next listed sort order dihitung sebagai max listed sort order ditambah 1. |
| **DR-14 Name** | Untuk setiap rule, name sama dengan label. |
| **DR-15 Count of Firings** | Untuk setiap rule, count of firings adalah nilai total fired unit pada seluruh firings yang terkait dengan rule tersebut. |
| **DR-16 Name** | Untuk setiap firing kind, name sama dengan wording. |
| **DR-17 Rule Enabled** | Untuk setiap firing kind, rule enabled bernilai benar jika rule yang tertaut pada firing kind tersebut tergolong “enabled”. |
| **DR-18 Name** | Untuk setiap thing, name sama dengan label. |
| **DR-19 Count of Children** | Untuk setiap thing, count of children adalah nilai total parent unit pada seluruh statements yang terkait dengan thing tersebut. |
| **DR-20 Count of Grandchildren** | Untuk setiap thing, count of grandchildren adalah nilai total grandparent unit pada seluruh statements yang terkait dengan thing tersebut. |
| **DR-21 Count As Place** | Untuk setiap thing, count as place adalah nilai total place unit pada seluruh statements yang terkait dengan thing tersebut. |
| **DR-22 Count As Subject** | Untuk setiap thing, count as subject adalah nilai total holds unit pada seluruh statements yang terkait dengan thing tersebut. |
| **DR-23 Count As Object** | Untuk setiap thing, count as object adalah nilai total holds unit pada seluruh statements yang terkait dengan thing tersebut. |
| **DR-24 Is in Graph** | Setiap thing dianggap “in graph” jika paling sedikit satu dari kondisi berikut terpenuhi: count as subject lebih dari 0 atau count as object lebih dari 0. |
| **DR-25 Node Kind** | Untuk setiap thing, node kind ditentukan menurut urutan prioritas berikut:<br>1. “class”, jika kind adalah “class”;<br>2. “place”, jika count as place lebih dari 0;<br>3. dalam semua kasus lainnya, “person”. |
| **DR-26 Name** | Untuk setiap property, name sama dengan label. |
| **DR-27 Subject Label** | Untuk setiap statement, subject label diambil dari subject yang tertaut. |
| **DR-28 Property Label** | Untuk setiap statement, property label diambil dari property yang tertaut. |
| **DR-29 Object Label** | Untuk setiap statement, object label diambil dari object thing yang tertaut. |
| **DR-30 Property Datatype** | Untuk setiap statement, property datatype diambil dari property yang tertaut. |
| **DR-31 Property Has One Current Value** | Untuk setiap statement, property has one current value bernilai benar jika property yang tertaut pada statement tersebut memiliki “one current value”. |
| **DR-32 Property Domain Class** | Untuk setiap statement, property domain class diambil dari property yang tertaut. |
| **DR-33 Property Range Class** | Untuk setiap statement, property range class diambil dari property yang tertaut. |
| **DR-34 Literal Quote** | Untuk setiap statement, literal quote diambil dari graph yang tertaut. |
| **DR-35 Literal Type Marker** | Untuk setiap statement, literal type marker diambil dari graph yang tertaut. |
| **DR-36 Built in Source Text** | Untuk setiap statement, built in source text diambil dari graph yang tertaut. |
| **DR-37 User Source Text** | Untuk setiap statement, user source text diambil dari graph yang tertaut. |
| **DR-38 Next Fact Sort Order** | Untuk setiap statement, next fact sort order diambil dari graph yang tertaut. |
| **DR-39 Is Literal** | Setiap statement dianggap “literal” jika object thing kosong. |
| **DR-40 Object Text** | Untuk setiap statement, object text ditentukan menurut urutan prioritas berikut:<br>1. object value, jika object thing kosong;<br>2. dalam semua kasus lainnya, object label. |
| **DR-41 Name** | Untuk setiap statement, name dihitung sebagai subject label, diikuti spasi, diikuti property label, diikuti spasi, diikuti object text. |
| **DR-42 Words** | Untuk setiap statement, words dihitung sebagai subject label, diikuti spasi, diikuti property label, diikuti spasi, diikuti literal quote, diikuti object value, diikuti literal quote, diikuti literal type marker, diikuti property datatype jika object thing kosong, dan dalam semua kasus lainnya object label. ⚠︎ mekanis <!-- rulespeak:reword --> |
| **DR-43 Source** | Untuk setiap statement, source ditentukan menurut urutan prioritas berikut:<br>1. built in source text, jika penanda “built in” aktif;<br>2. dalam semua kasus lainnya, user source text. |
| **DR-44 Subject Property Key** | Untuk setiap statement, subject property key dihitung sebagai subject, diikuti “|”, diikuti property. |
| **DR-45 Latest Sibling Activation Seq** | Untuk setiap statement, latest sibling activation seq adalah nilai maksimum activation seq pada seluruh statements yang terkait dengan statement tersebut. |
| **DR-46 Is in Effect** | Setiap statement dianggap “in effect” jika semua kondisi berikut terpenuhi: penanda “stated” aktif dan paling sedikit satu dari kondisi berikut terpenuhi: penanda “property has one current value” tidak aktif atau activation seq adalah latest sibling activation seq. |
| **DR-47 Listed Unit** | Untuk setiap statement, listed unit ditentukan menurut urutan prioritas berikut:<br>1. 1, jika penanda “listed” aktif;<br>2. dalam semua kasus lainnya, 0. |
| **DR-48 In Effect Unit** | Untuk setiap statement, in effect unit ditentukan menurut urutan prioritas berikut:<br>1. 1, jika penanda “in effect” aktif;<br>2. dalam semua kasus lainnya, 0. |
| **DR-49 Listed Off Unit** | Untuk setiap statement, listed off unit ditentukan menurut urutan prioritas berikut:<br>1. 1, jika semua kondisi berikut terpenuhi: penanda “listed” aktif dan penanda “in effect” tidak aktif;<br>2. dalam semua kasus lainnya, 0. |
| **DR-50 Is Next to Add** | Setiap statement dianggap “next to add” jika semua kondisi berikut terpenuhi: listed off unit adalah 1 dan sort order adalah next fact sort order. |
| **DR-51 Count of Inverse Firings** | Untuk setiap statement, count of inverse firings adalah nilai total inverse fired unit pada seluruh firings yang terkait dengan statement tersebut. |
| **DR-52 Count of Symmetric Firings** | Untuk setiap statement, count of symmetric firings adalah nilai total symmetric fired unit pada seluruh firings yang terkait dengan statement tersebut. |
| **DR-53 Holds Before Chain** | Setiap statement dianggap memenuhi kondisi “holds before chain” jika paling sedikit satu dari kondisi berikut terpenuhi: penanda “in effect” aktif, count of inverse firings lebih dari 0, atau count of symmetric firings lebih dari 0. |
| **DR-54 Count of Chain Firings** | Untuk setiap statement, count of chain firings adalah nilai total chain fired unit pada seluruh firings yang terkait dengan statement tersebut. |
| **DR-55 Holds Before Typing** | Setiap statement dianggap memenuhi kondisi “holds before typing” jika paling sedikit satu dari kondisi berikut terpenuhi: penanda “holds before chain” aktif atau count of chain firings lebih dari 0. |
| **DR-56 Count of Type Firings** | Untuk setiap statement, count of type firings adalah nilai total type fired unit pada seluruh firings yang terkait dengan statement tersebut. |
| **DR-57 Holds** | Setiap statement dianggap memenuhi kondisi “holds” jika paling sedikit satu dari kondisi berikut terpenuhi: penanda “holds before typing” aktif atau count of type firings lebih dari 0. |
| **DR-58 Is Conclusion** | Setiap statement dianggap “conclusion” jika semua kondisi berikut terpenuhi: penanda “holds” aktif dan penanda “in effect” tidak aktif. |
| **DR-59 Holds Unit** | Untuk setiap statement, holds unit ditentukan menurut urutan prioritas berikut:<br>1. 1, jika penanda “holds” aktif;<br>2. dalam semua kasus lainnya, 0. |
| **DR-60 Conclusion Unit** | Untuk setiap statement, conclusion unit ditentukan menurut urutan prioritas berikut:<br>1. 1, jika penanda “conclusion” aktif;<br>2. dalam semua kasus lainnya, 0. |
| **DR-61 Parent Unit** | Untuk setiap statement, parent unit ditentukan menurut urutan prioritas berikut:<br>1. 1, jika semua kondisi berikut terpenuhi: penanda “holds” aktif dan property adalah “isParentOf”;<br>2. dalam semua kasus lainnya, 0. |
| **DR-62 Grandparent Unit** | Untuk setiap statement, grandparent unit ditentukan menurut urutan prioritas berikut:<br>1. 1, jika semua kondisi berikut terpenuhi: penanda “holds” aktif dan property adalah “isGrandparentOf”;<br>2. dalam semua kasus lainnya, 0. |
| **DR-63 Place Unit** | Untuk setiap statement, place unit ditentukan menurut urutan prioritas berikut:<br>1. 1, jika semua kondisi berikut terpenuhi: penanda “holds” aktif dan property adalah “attends”;<br>2. dalam semua kasus lainnya, 0. |
| **DR-64 First Fired Order Key** | Untuk setiap statement, first fired order key adalah nilai minimum order key pada seluruh firings dari statement tersebut yang tergolong “fired unit”. |
| **DR-65 Count of Chosen Firings** | Untuk setiap statement, count of chosen firings adalah nilai total chosen unit pada seluruh firings yang terkait dengan statement tersebut. |
| **DR-66 Rule Enabled** | Untuk setiap firing, rule enabled bernilai benar jika rule yang tertaut pada firing tersebut tergolong “enabled”. |
| **DR-67 Kind Wording** | Untuk setiap firing, kind wording diambil dari kind yang tertaut. |
| **DR-68 Kind Sort Order** | Untuk setiap firing, kind sort order diambil dari kind yang tertaut. |
| **DR-69 Target Name** | Untuk setiap firing, target name diambil dari target yang tertaut. |
| **DR-70 Target Subject Label** | Untuk setiap firing, target subject label diambil dari target yang tertaut. |
| **DR-71 Target Property Label** | Untuk setiap firing, target property label diambil dari target yang tertaut. |
| **DR-72 Target Object Text** | Untuk setiap firing, target object text diambil dari target yang tertaut. |
| **DR-73 Target is Conclusion** | Untuk setiap firing, target is conclusion bernilai benar jika target yang tertaut pada firing tersebut tergolong “conclusion”. |
| **DR-74 Target First Fired Order Key** | Untuk setiap firing, target first fired order key diambil dari target yang tertaut. |
| **DR-75 First Words** | Untuk setiap firing, first words diambil dari first yang tertaut. |
| **DR-76 Second Words** | Untuk setiap firing, second words diambil dari second yang tertaut. |
| **DR-77 First is in Effect** | Untuk setiap firing, first is in effect bernilai benar jika first yang tertaut pada firing tersebut tergolong “in effect”. |
| **DR-78 First Holds Before Chain** | Untuk setiap firing, first holds before chain bernilai benar jika first yang tertaut pada firing tersebut memenuhi kondisi “holds before chain”. |
| **DR-79 Second Holds Before Chain** | Untuk setiap firing, second holds before chain bernilai benar jika second yang tertaut pada firing tersebut memenuhi kondisi “holds before chain”. |
| **DR-80 First Holds Before Typing** | Untuk setiap firing, first holds before typing bernilai benar jika first yang tertaut pada firing tersebut memenuhi kondisi “holds before typing”. |
| **DR-81 First Property** | Untuk setiap firing, first property diambil dari first yang tertaut. |
| **DR-82 First Property Domain Class** | Untuk setiap firing, first property domain class diambil dari first yang tertaut. |
| **DR-83 First Property Range Class** | Untuk setiap firing, first property range class diambil dari first yang tertaut. |
| **DR-84 First Sort Order** | Untuk setiap firing, first sort order diambil dari first yang tertaut. |
| **DR-85 Second Sort Order** | Untuk setiap firing, second sort order diambil dari second yang tertaut. |
| **DR-86 Inverse Fired Unit** | Untuk setiap firing, inverse fired unit ditentukan menurut urutan prioritas berikut:<br>1. 1, jika semua kondisi berikut terpenuhi: penanda “rule enabled” aktif, kind adalah “inverse”, dan penanda “first is in effect” aktif;<br>2. dalam semua kasus lainnya, 0. |
| **DR-87 Symmetric Fired Unit** | Untuk setiap firing, symmetric fired unit ditentukan menurut urutan prioritas berikut:<br>1. 1, jika semua kondisi berikut terpenuhi: penanda “rule enabled” aktif, kind adalah “symmetric”, dan penanda “first is in effect” aktif;<br>2. dalam semua kasus lainnya, 0. |
| **DR-88 Chain Fired Unit** | Untuk setiap firing, chain fired unit ditentukan menurut urutan prioritas berikut:<br>1. 1, jika semua kondisi berikut terpenuhi: penanda “rule enabled” aktif, kind adalah “chain”, penanda “first holds before chain” aktif, dan penanda “second holds before chain” aktif;<br>2. dalam semua kasus lainnya, 0. |
| **DR-89 Type Fired Unit** | Untuk setiap firing, type fired unit ditentukan menurut urutan prioritas berikut:<br>1. 1, jika semua kondisi berikut terpenuhi: penanda “rule enabled” aktif; paling sedikit satu dari kondisi berikut terpenuhi: kind adalah “domain” atau kind adalah “range”; dan penanda “first holds before typing” aktif;<br>2. dalam semua kasus lainnya, 0. |
| **DR-90 Fired Unit** | Untuk setiap firing, fired unit dihitung sebagai inverse fired unit ditambah symmetric fired unit ditambah chain fired unit ditambah type fired unit. |
| **DR-91 Order Key** | Untuk setiap firing, order key dihitung sebagai first sort order dikali 100000 ditambah kind sort order dikali 1000 ditambah 0 jika second kosong, dan dalam semua kasus lainnya second sort order. |
| **DR-92 Is Chosen** | Setiap firing dianggap “chosen” jika semua kondisi berikut terpenuhi: fired unit adalah 1, penanda “target is conclusion” aktif, dan order key adalah target first fired order key. |
| **DR-93 Chosen Unit** | Untuk setiap firing, chosen unit ditentukan menurut urutan prioritas berikut:<br>1. 1, jika penanda “chosen” aktif;<br>2. dalam semua kasus lainnya, 0. |
| **DR-94 Card Text** | Untuk setiap firing, card text ditentukan menurut urutan prioritas berikut:<br>1. “Worked out by the ”, diikuti kind wording, diikuti “ from “”, diikuti first words, diikuti “””, diikuti “ and “”, diikuti second words, diikuti “”” jika kind adalah “chain”, dan dalam semua kasus lainnya “ and “”, diikuti first property, diikuti “ has domain ”, diikuti first property domain class, diikuti “”” jika kind adalah “domain”, dan dalam semua kasus lainnya “ and “”, diikuti first property, diikuti “ has range ”, diikuti first property range class, diikuti “””, diikuti tanda titik, jika penanda “chosen” aktif;<br>2. dalam semua kasus lainnya, teks kosong. ⚠︎ mekanis <!-- rulespeak:reword --> |
| **DR-95 Name** | Untuk setiap firing, name sama dengan firing ID. |
| **DR-96 Rule Enabled** | Untuk setiap disjoint check, rule enabled bernilai benar jika rule yang tertaut pada disjoint check tersebut tergolong “enabled”. |
| **DR-97 First Holds** | Untuk setiap disjoint check, first holds bernilai benar jika first yang tertaut pada disjoint check tersebut memenuhi kondisi “holds”. |
| **DR-98 Second Holds** | Untuk setiap disjoint check, second holds bernilai benar jika second yang tertaut pada disjoint check tersebut memenuhi kondisi “holds”. |
| **DR-99 Subject Label** | Untuk setiap disjoint check, subject label diambil dari first yang tertaut. |
| **DR-100 Object Label** | Untuk setiap disjoint check, object label diambil dari first yang tertaut. |
| **DR-101 Violation Unit** | Untuk setiap disjoint check, violation unit ditentukan menurut urutan prioritas berikut:<br>1. 1, jika semua kondisi berikut terpenuhi: penanda “rule enabled” aktif, penanda “first holds” aktif, dan penanda “second holds” aktif;<br>2. dalam semua kasus lainnya, 0. |
| **DR-102 Alarm Text** | Untuk setiap disjoint check, alarm text ditentukan menurut urutan prioritas berikut:<br>1. “Contradiction found. ”, diikuti subject label, diikuti “ is recorded as both parent and spouse of ”, diikuti object label, diikuti “. Check those facts and the selected disjoint rule.”, jika violation unit adalah 1;<br>2. dalam semua kasus lainnya, teks kosong. |
| **DR-103 Name** | Untuk setiap disjoint check, name sama dengan disjoint check ID. |

## 5 Ketertelusuran ke Skema

_Kolom ekspresi memuat definisi aturan dalam notasi Dokumentasi —
logika yang sama dengan yang disimpan dalam buku aturan, ditulis untuk pembaca bisnis._

| Elemen skema | Jenis | Ekspresi |
|----------------|------|------------|
| **Graphs.Name** | rumus | `Label` |
| **Graphs.CountOfStatedFacts** | agregasi | `Sum(Statements.InEffectUnit via Graph)` |
| **Graphs.CountOfConclusions** | agregasi | `Sum(Statements.ConclusionUnit via Graph)` |
| **Graphs.CountOfViolations** | agregasi | `Sum(DisjointChecks.ViolationUnit via Graph)` |
| **Graphs.IsConsistent** | rumus | `CountOfViolations = 0` |
| **Graphs.ConclusionsMessage** | rumus | `If(CountOfConclusions = 0, NoConclusionsText, "")` |
| **Graphs.CountOfFactsToAdd** | agregasi | `Sum(Statements.ListedOffUnit via Graph)` |
| **Graphs.HasFactsToAdd** | rumus | `CountOfFactsToAdd > 0` |
| **Graphs.NextFactSortOrder** | agregasi | `Min(Statements.SortOrder via Graph)` |
| **Graphs.MaxActivationSeq** | agregasi | `Max(Statements.ActivationSeq via Graph)` |
| **Graphs.NextActivationSeq** | rumus | `MaxActivationSeq + 1` |
| **Graphs.MaxListedSortOrder** | agregasi | `Max(Statements.SortOrder via Graph)` |
| **Graphs.NextListedSortOrder** | rumus | `MaxListedSortOrder + 1` |
| **Rules.Name** | rumus | `Label` |
| **Rules.CountOfFirings** | agregasi | `Sum(Firings.FiredUnit via Rule)` |
| **FiringKinds.Name** | rumus | `Wording` |
| **FiringKinds.RuleEnabled** | pencarian | `Lookup(Rules.IsEnabled via Rule)` |
| **Things.Name** | rumus | `Label` |
| **Things.CountOfChildren** | agregasi | `Sum(Statements.ParentUnit via Subject)` |
| **Things.CountOfGrandchildren** | agregasi | `Sum(Statements.GrandparentUnit via Subject)` |
| **Things.CountAsPlace** | agregasi | `Sum(Statements.PlaceUnit via ObjectThing)` |
| **Things.CountAsSubject** | agregasi | `Sum(Statements.HoldsUnit via Subject)` |
| **Things.CountAsObject** | agregasi | `Sum(Statements.HoldsUnit via ObjectThing)` |
| **Things.IsInGraph** | rumus | `Or(CountAsSubject > 0, CountAsObject > 0)` |
| **Things.NodeKind** | rumus | `If(Kind = "class", "class", If(CountAsPlace > 0, "place", "person"))` |
| **Properties.Name** | rumus | `Label` |
| **Statements.SubjectLabel** | pencarian | `Lookup(Things.Label via Subject)` |
| **Statements.PropertyLabel** | pencarian | `Lookup(Properties.Label via Property)` |
| **Statements.ObjectLabel** | pencarian | `Lookup(Things.Label via ObjectThing)` |
| **Statements.PropertyDatatype** | pencarian | `Lookup(Properties.Datatype via Property)` |
| **Statements.PropertyHasOneCurrentValue** | pencarian | `Lookup(Properties.HasOneCurrentValue via Property)` |
| **Statements.PropertyDomainClass** | pencarian | `Lookup(Properties.DomainClass via Property)` |
| **Statements.PropertyRangeClass** | pencarian | `Lookup(Properties.RangeClass via Property)` |
| **Statements.LiteralQuote** | pencarian | `Lookup(Graphs.LiteralQuote via Graph)` |
| **Statements.LiteralTypeMarker** | pencarian | `Lookup(Graphs.LiteralTypeMarker via Graph)` |
| **Statements.BuiltInSourceText** | pencarian | `Lookup(Graphs.BuiltInSourceText via Graph)` |
| **Statements.UserSourceText** | pencarian | `Lookup(Graphs.UserSourceText via Graph)` |
| **Statements.NextFactSortOrder** | pencarian | `Lookup(Graphs.NextFactSortOrder via Graph)` |
| **Statements.IsLiteral** | rumus | `ObjectThing = ""` |
| **Statements.ObjectText** | rumus | `If(ObjectThing = "", ObjectValue, ObjectLabel)` |
| **Statements.Name** | rumus | `Concat(SubjectLabel, " ", PropertyLabel, " ", ObjectText)` |
| **Statements.Words** | rumus | `Concat(SubjectLabel, " ", PropertyLabel, " ", If(ObjectThing = "", Concat(LiteralQuote, ObjectValue, LiteralQuote, LiteralTypeMarker, PropertyDatatype), ObjectLabel))` |
| **Statements.Source** | rumus | `If(IsBuiltIn, BuiltInSourceText, UserSourceText)` |
| **Statements.SubjectPropertyKey** | rumus | `Concat(Subject, "\|", Property)` |
| **Statements.LatestSiblingActivationSeq** | agregasi | `Max(Statements.ActivationSeq via SubjectPropertyKey)` |
| **Statements.IsInEffect** | rumus | `And(IsStated, Or(Not(PropertyHasOneCurrentValue), ActivationSeq = LatestSiblingActivationSeq))` |
| **Statements.ListedUnit** | rumus | `If(IsListed, 1, 0)` |
| **Statements.InEffectUnit** | rumus | `If(IsInEffect, 1, 0)` |
| **Statements.ListedOffUnit** | rumus | `If(And(IsListed, Not(IsInEffect)), 1, 0)` |
| **Statements.IsNextToAdd** | rumus | `And(ListedOffUnit = 1, SortOrder = NextFactSortOrder)` |
| **Statements.CountOfInverseFirings** | agregasi | `Sum(Firings.InverseFiredUnit via Target)` |
| **Statements.CountOfSymmetricFirings** | agregasi | `Sum(Firings.SymmetricFiredUnit via Target)` |
| **Statements.HoldsBeforeChain** | rumus | `Or(IsInEffect, CountOfInverseFirings > 0, CountOfSymmetricFirings > 0)` |
| **Statements.CountOfChainFirings** | agregasi | `Sum(Firings.ChainFiredUnit via Target)` |
| **Statements.HoldsBeforeTyping** | rumus | `Or(HoldsBeforeChain, CountOfChainFirings > 0)` |
| **Statements.CountOfTypeFirings** | agregasi | `Sum(Firings.TypeFiredUnit via Target)` |
| **Statements.Holds** | rumus | `Or(HoldsBeforeTyping, CountOfTypeFirings > 0)` |
| **Statements.IsConclusion** | rumus | `And(Holds, Not(IsInEffect))` |
| **Statements.HoldsUnit** | rumus | `If(Holds, 1, 0)` |
| **Statements.ConclusionUnit** | rumus | `If(IsConclusion, 1, 0)` |
| **Statements.ParentUnit** | rumus | `If(And(Holds, Property = "isParentOf"), 1, 0)` |
| **Statements.GrandparentUnit** | rumus | `If(And(Holds, Property = "isGrandparentOf"), 1, 0)` |
| **Statements.PlaceUnit** | rumus | `If(And(Holds, Property = "attends"), 1, 0)` |
| **Statements.FirstFiredOrderKey** | agregasi | `Min(Firings.OrderKey via Target)` |
| **Statements.CountOfChosenFirings** | agregasi | `Sum(Firings.ChosenUnit via Target)` |
| **Firings.RuleEnabled** | pencarian | `Lookup(Rules.IsEnabled via Rule)` |
| **Firings.KindWording** | pencarian | `Lookup(FiringKinds.Wording via Kind)` |
| **Firings.KindSortOrder** | pencarian | `Lookup(FiringKinds.SortOrder via Kind)` |
| **Firings.TargetName** | pencarian | `Lookup(Statements.Name via Target)` |
| **Firings.TargetSubjectLabel** | pencarian | `Lookup(Statements.SubjectLabel via Target)` |
| **Firings.TargetPropertyLabel** | pencarian | `Lookup(Statements.PropertyLabel via Target)` |
| **Firings.TargetObjectText** | pencarian | `Lookup(Statements.ObjectText via Target)` |
| **Firings.TargetIsConclusion** | pencarian | `Lookup(Statements.IsConclusion via Target)` |
| **Firings.TargetFirstFiredOrderKey** | pencarian | `Lookup(Statements.FirstFiredOrderKey via Target)` |
| **Firings.FirstWords** | pencarian | `Lookup(Statements.Words via First)` |
| **Firings.SecondWords** | pencarian | `Lookup(Statements.Words via Second)` |
| **Firings.FirstIsInEffect** | pencarian | `Lookup(Statements.IsInEffect via First)` |
| **Firings.FirstHoldsBeforeChain** | pencarian | `Lookup(Statements.HoldsBeforeChain via First)` |
| **Firings.SecondHoldsBeforeChain** | pencarian | `Lookup(Statements.HoldsBeforeChain via Second)` |
| **Firings.FirstHoldsBeforeTyping** | pencarian | `Lookup(Statements.HoldsBeforeTyping via First)` |
| **Firings.FirstProperty** | pencarian | `Lookup(Statements.Property via First)` |
| **Firings.FirstPropertyDomainClass** | pencarian | `Lookup(Statements.PropertyDomainClass via First)` |
| **Firings.FirstPropertyRangeClass** | pencarian | `Lookup(Statements.PropertyRangeClass via First)` |
| **Firings.FirstSortOrder** | pencarian | `Lookup(Statements.SortOrder via First)` |
| **Firings.SecondSortOrder** | pencarian | `Lookup(Statements.SortOrder via Second)` |
| **Firings.InverseFiredUnit** | rumus | `If(And(RuleEnabled, Kind = "inverse", FirstIsInEffect), 1, 0)` |
| **Firings.SymmetricFiredUnit** | rumus | `If(And(RuleEnabled, Kind = "symmetric", FirstIsInEffect), 1, 0)` |
| **Firings.ChainFiredUnit** | rumus | `If(And(RuleEnabled, Kind = "chain", FirstHoldsBeforeChain, SecondHoldsBeforeChain), 1, 0)` |
| **Firings.TypeFiredUnit** | rumus | `If(And(RuleEnabled, Or(Kind = "domain", Kind = "range"), FirstHoldsBeforeTyping), 1, 0)` |
| **Firings.FiredUnit** | rumus | `InverseFiredUnit + SymmetricFiredUnit + ChainFiredUnit + TypeFiredUnit` |
| **Firings.OrderKey** | rumus | `FirstSortOrder * 100000 + KindSortOrder * 1000 + If(Second = "", 0, SecondSortOrder)` |
| **Firings.IsChosen** | rumus | `And(FiredUnit = 1, TargetIsConclusion, OrderKey = TargetFirstFiredOrderKey)` |
| **Firings.ChosenUnit** | rumus | `If(IsChosen, 1, 0)` |
| **Firings.CardText** | rumus | `If(IsChosen, Concat("Worked out by the ", KindWording, " from “", FirstWords, "”", If(Kind = "chain", Concat(" and “", SecondWords, "”"), If(Kind = "domain", Concat(" and “", FirstProperty, " has domain ", FirstPropertyDomainClass, "”"), If(Kind = "range", Concat(" and “", FirstProperty, " has range ", FirstPropertyRangeClass, "”"), ""))), "."), "")` |
| **Firings.Name** | rumus | `FiringId` |
| **DisjointChecks.RuleEnabled** | pencarian | `Lookup(Rules.IsEnabled via Rule)` |
| **DisjointChecks.FirstHolds** | pencarian | `Lookup(Statements.Holds via First)` |
| **DisjointChecks.SecondHolds** | pencarian | `Lookup(Statements.Holds via Second)` |
| **DisjointChecks.SubjectLabel** | pencarian | `Lookup(Statements.SubjectLabel via First)` |
| **DisjointChecks.ObjectLabel** | pencarian | `Lookup(Statements.ObjectLabel via First)` |
| **DisjointChecks.ViolationUnit** | rumus | `If(And(RuleEnabled, FirstHolds, SecondHolds), 1, 0)` |
| **DisjointChecks.AlarmText** | rumus | `If(ViolationUnit = 1, Concat("Contradiction found. ", SubjectLabel, " is recorded as both parent and spouse of ", ObjectLabel, ". Check those facts and the selected disjoint rule."), "")` |
| **DisjointChecks.Name** | rumus | `DisjointCheckId` |

---

> **RuleSpeak®** — notasi aturan bisnis deklaratif ciptaan **Ronald G. Ross** yang mengikuti konvensi **SBVR** — segera hadir. Sampai saat itu, halaman ini menampilkan dokumentasi biasa. [www.RonRoss.info](https://www.RonRoss.info)
