# 📘 Talisman's South Park ontology explainer — Dokumentasi

_Jessica Talisman's South Park Ontology Explainer, as a rulebook: the page's ten facts, five rule switches, property wiring, provenance wording and current-value policy are rows, and every conclusion is a formula. The rules apply in three fixed strata (inverse and symmetric, then the property chain, then domain and range); property wiring that feeds an earlier stratum would need repeated rounds. Every fact that could hold over the cast is a candidate row; app/lib/candidates.mjs writes the candidates when a thing, value or property is added._

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
| Max Sort Order | Nilai maksimum sort order pada seluruh statements yang terkait dengan graph tersebut. | _Order 1. Highest sort order of any fact, listed or candidate._ |
| Next Candidate Sort Order | Dihitung sebagai max sort order ditambah 1. | _Order 2. The sort order a new candidate fact, or a fact removed from the list, takes._ |

<details open>
<summary>Contoh data</summary>

| Label | Literal Quote | Literal Type Marker | Built in Source Text | User Source Text | No Conclusions Text | Name ƒ | Count of Stated Facts ƒ | Count of Conclusions ƒ | Count of Violations ƒ | Is Consistent ƒ | Conclusions Message ƒ | Count of Facts to Add ƒ | Has Facts to Add ƒ | Next Fact Sort Order ƒ | Max Activation Seq ƒ | Next Activation Seq ƒ | Max Listed Sort Order ƒ | Next Listed Sort Order ƒ | Max Sort Order ƒ | Next Candidate Sort Order ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| South Park | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | These facts and selected rules add no further conclusions. Try another fact or rule. | South Park | 10 | 0 | 0 | true | These facts and selected rules add no further conclusions. Try another fact or rule. | 0 | false | — | 10 | 11 | 10 | 11 | 100269 | 100270 |

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
| **Firing Kind** | The ways a rule can apply. Domain and range belong to one rule but are worded apart; 'disjoint' names the rule the disjointness checks answer to. | — |
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
| Portrait | Atribut terdefinisi. | _The portrait drawn for this thing; set when its name matches a portrait alias._ |
| Portrait Name | Nilai display name dari portrait yang tertaut pada thing tersebut. | _Order 1. The name on the portrait's Fandom page._ |
| Portrait Page | Diambil dari portrait yang tertaut. | _Order 1. The Fandom page credited for the portrait._ |
| Portrait Image | Diambil dari portrait yang tertaut. | _Order 1. The original Fandom image._ |
| Portrait Crop | Diambil dari portrait yang tertaut. | _Order 1. The portrait's crop as an SVG viewBox._ |
| Portrait Width | Diambil dari portrait yang tertaut. | _Order 1. The original image's width in pixels._ |
| Portrait Height | Diambil dari portrait yang tertaut. | _Order 1. The original image's height in pixels._ |
| Name | Sama dengan label. | _Order 1. The thing's name._ |
| Node Key | Sama dengan thing ID. | _Order 1. The key the graph draws this thing under._ |
| Count of Children | Nilai total parent unit pada seluruh statements yang terkait dengan thing tersebut. | _Order 17. People this thing is a parent of, stated or concluded._ |
| Count of Grandchildren | Nilai total grandparent unit pada seluruh statements yang terkait dengan thing tersebut. | _Order 17. People this thing is a grandparent of._ |
| Count As Place | Nilai total place unit pada seluruh statements yang terkait dengan thing tersebut. | _Order 17. Holding attends facts whose object is this thing._ |
| Count As Subject | Nilai total holds unit pada seluruh statements yang terkait dengan thing tersebut. | _Order 17. Holding facts about this thing._ |
| Count As Object | Nilai total holds unit pada seluruh statements yang terkait dengan thing tersebut. | _Order 17. Holding facts that point at this thing._ |
| Is in Graph | Bernilai benar jika paling sedikit satu dari kondisi berikut terpenuhi: count as subject lebih dari 0 atau count as object lebih dari 0. | _Order 18. True when the graph draws this thing._ |
| Node Kind | Ditentukan menurut prioritas: teks kosong jika kind kosong; “class” jika kind adalah “class”; “place” jika count as place lebih dari 0; dalam semua kasus lainnya, “person”. | _Order 18. How the graph draws the thing: a class, a place someone attends, or a person._ |

<details open>
<summary>Contoh data</summary>

| Label | Kind | Sort Order | Portrait Name ƒ | Portrait Page ƒ | Portrait Image ƒ | Portrait Crop ƒ | Portrait Width ƒ | Portrait Height ƒ | Name ƒ | Node Key ƒ | Count of Children ƒ | Count of Grandchildren ƒ | Count As Place ƒ | Count As Subject ƒ | Count As Object ƒ | Is in Graph ƒ | Node Kind ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| Marvin | individual | 1 | Marvin Marsh | https://southpark.fandom.com/wiki/Marvin_Marsh | https://static.wikia.nocookie.net/southpark/images/d/db/Marsh-family-grandpa-marsh-w-purple-blanket.png/revision/latest | 48 -5 280 280 | 375 | 536 | Marvin | Marvin | 1 | 0 | 0 | 1 | 0 | true | person |
| Randy | individual | 2 | Randy Marsh | https://southpark.fandom.com/wiki/Randy_Marsh | https://static.wikia.nocookie.net/southparkfanon/images/d/d6/Randy_Marsh.png/revision/latest | 0 -10 428 428 | 428 | 1057 | Randy | Randy | 2 | 0 | 0 | 3 | 1 | true | person |
| Sharon | individual | 3 | Sharon Marsh | https://southpark.fandom.com/wiki/Sharon_Marsh | https://static.wikia.nocookie.net/southpark/images/9/99/Sharon_Marsh.png/revision/latest | 20 -10 390 390 | 439 | 1049 | Sharon | Sharon | 0 | 0 | 0 | 0 | 1 | true | person |

_ƒ menandai kolom terhitung._

</details>

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
| **Portrait Aliase** | The names a portrait answers to. A new thing whose name matches an alias gets that portrait. | — |
| Portrait | Atribut terdefinisi. | _The portrait this name selects._ |
| Name | Sama dengan portrait alias ID. | _Order 1. The alias._ |

<details open>
<summary>Contoh data</summary>

| Name ƒ |
|---|
| marvin |
| marvin marsh |
| grandpa marsh |

_ƒ menandai kolom terhitung._

</details>

| Istilah | Deskripsi | Catatan Naratif |
|------|-------------|-------------------|
| **Property** | The ten properties, with what the symmetric, domain and range rules read. Axioms that relate one property to another (inverse, chain, disjoint) are rows of PropertyAxioms. | — |
| Label | Atribut terdefinisi. | _The property's wording on the page._ |
| Property Kind | Atribut terdefinisi. | _'object' links two things, 'datatype' links a thing to a value, 'type' states class membership._ |
| Datatype | Atribut terdefinisi. | _The value's XML Schema datatype for a datatype property; blank otherwise._ |
| Is User Addable | Bernilai benar jika teks kosong. | _Offered in the page's Add fact form._ |
| Has One Current Value | Bernilai benar jika teks kosong. | _The page keeps one current value per person: switching a value on replaces the earlier one._ |
| Is Symmetric | Bernilai benar jika teks kosong. | _The symmetric rule makes this link true in both directions._ |
| Domain Class | Atribut terdefinisi. | _The class the domain rule gives this property's subject._ |
| Range Class | Atribut terdefinisi. | _The class the range rule gives this property's object._ |
| Sort Order | Atribut terdefinisi. | _Position in the Add fact form's property list._ |
| Value Node Label | Atribut terdefinisi. | _The small label under a value node for this property: 'role value' or 'number value'._ |
| Current Value Wording | Atribut terdefinisi. | _How the Add fact form names a replaced value, as in 'Stan now has grade level 5'._ |
| Input Name | Atribut terdefinisi. | _How the integer check names this property's value, as in 'for grade level'._ |
| Record Kind | Atribut terdefinisi. | _The word the current-record summary uses for this property's values: role, grade or school._ |
| Record Order | Atribut terdefinisi. | _Position of this property in the current-record summary._ |
| Name | Sama dengan label. | _Order 1. The property's wording._ |

<details open>
<summary>Contoh data</summary>

| Label | Property Kind | Datatype | Is User Addable | Has One Current Value | Is Symmetric | Sort Order | Value Node Label | Current Value Wording | Input Name | Record Kind | Record Order | Name ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|
| is parent of | object | — | true | false | false | 1 | — | — | — | — | — | is parent of |
| is child of | object | — | true | false | false | 2 | — | — | — | — | — | is child of |
| is spouse of | object | — | true | false | true | 3 | — | — | — | — | — | is spouse of |

_ƒ menandai kolom terhitung._

</details>

| Istilah | Deskripsi | Catatan Naratif |
|------|-------------|-------------------|
| **Property Axiom** | Axioms that relate one property to others, as OWL states them: an inverse, a two-link chain, or a disjoint pair. The candidate generator reads these rows. | — |
| Kind | Atribut terdefinisi. | _inverse, chain or disjoint._ |
| Property | Atribut terdefinisi. | _The property the axiom is about: the one inverted, the one a chain derives, or the first of a disjoint pair._ |
| First | Atribut terdefinisi. | _The inverse property, the chain's first link, or the other property of a disjoint pair._ |
| Second | Atribut terdefinisi. | _The chain's second link._ |
| Name | Sama dengan property axiom ID. | _Order 1. The axiom's key._ |

<details open>
<summary>Contoh data</summary>

| Name ƒ |
|---|
| inverse:isParentOf |
| disjoint:isParentOf~isSpouseOf |
| inverse:isChildOf |

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
| Holds Before Chain | Bernilai benar jika hasil fungsi IF atas hasil fungsi OR atas is in effect, count of inverse firings lebih dari 0, dan count of symmetric firings lebih dari 0 dan benar. | _Order 7. TRUE when the fact is in effect or the inverse or symmetric rule concludes it. Blank when nothing states or concludes it: not known, never FALSE (open world: removing a statement does not say that its opposite is true)._ |
| Count of Chain Firings | Nilai total chain fired unit pada seluruh firings yang terkait dengan statement tersebut. | _Order 10. Property chain applications that conclude this fact._ |
| Holds Before Typing | Bernilai benar jika hasil fungsi IF atas hasil fungsi OR atas holds before chain dan count of chain firings lebih dari 0 dan benar. | _Order 11. TRUE when the fact holds before the domain and range rules apply. Blank when nothing states or concludes it: not known, never FALSE (open world: removing a statement does not say that its opposite is true)._ |
| Count of Type Firings | Nilai total type fired unit pada seluruh firings yang terkait dengan statement tersebut. | _Order 14. Domain and range applications that conclude this fact._ |
| Holds | Bernilai benar jika hasil fungsi IF atas hasil fungsi OR atas holds before typing dan count of type firings lebih dari 0 dan benar. | _Order 15. TRUE when in effect or concluded by an enabled rule; the graph draws it. Blank when nothing states or concludes it: not known, never FALSE (open world: removing a statement does not say that its opposite is true)._ |
| Is Conclusion | Bernilai benar jika hasil fungsi IF atas hasil fungsi AND atas holds dan penanda “in effect” tidak aktif dan benar. | _Order 16. TRUE when the fact holds only because a rule concluded it; drawn dashed. Blank otherwise: a stated fact is not a conclusion, and a fact nothing concludes is not known (never FALSE)._ |
| Holds Unit | Ditentukan menurut prioritas: 1 jika penanda “holds” aktif; dalam semua kasus lainnya, 0. | _Order 16. 1 when the fact holds._ |
| Conclusion Unit | Ditentukan menurut prioritas: 1 jika penanda “conclusion” aktif; dalam semua kasus lainnya, 0. | _Order 17. 1 when the fact is a conclusion._ |
| Parent Unit | Ditentukan menurut prioritas: 1 jika semua kondisi berikut terpenuhi: penanda “holds” aktif dan property adalah “isParentOf”; dalam semua kasus lainnya, 0. | _Order 16. 1 for a holding parent link._ |
| Grandparent Unit | Ditentukan menurut prioritas: 1 jika semua kondisi berikut terpenuhi: penanda “holds” aktif dan property adalah “isGrandparentOf”; dalam semua kasus lainnya, 0. | _Order 16. 1 for a holding grandparent link._ |
| Place Unit | Ditentukan menurut prioritas: 1 jika semua kondisi berikut terpenuhi: penanda “holds” aktif dan property adalah “attends”; dalam semua kasus lainnya, 0. | _Order 16. 1 for a holding attends link._ |
| First Pre Chain Order Key | Nilai minimum pre chain order key pada seluruh firings dari statement tersebut yang tergolong “pre chain fired unit”. | _Order 7. Order key of the first inverse or symmetric application that concludes this fact._ |
| Snap Position | Ditentukan menurut prioritas: sort order jika penanda “in effect” aktif; 100000 ditambah first pre chain order key jika penanda “holds before chain” aktif; dalam semua kasus lainnya, 0. | _Order 8. Where the page's reasoner meets this fact as a premise: a stated fact at its list position, a fact concluded by the inverse or symmetric rule after every stated fact, in the order it was concluded; 0 for a fact that does not hold yet, so a link to no fact reads the same on every engine._ |
| First Fired Order Key | Nilai minimum order key pada seluruh firings dari statement tersebut yang tergolong “fired unit”. | _Order 15. Order key of the first rule application that concludes this fact, in the page reasoner's order._ |
| Count of Chosen Firings | Nilai total chosen unit pada seluruh firings yang terkait dengan statement tersebut. | _Order 20. Rule applications shown as this fact's conclusion card: one for a conclusion, zero otherwise._ |
| Subject Node Key | Sama dengan subject. | _Order 1. The graph node this fact starts from._ |
| Object Node Key | Ditentukan menurut prioritas: “literal|”, diikuti object value, diikuti “|”, diikuti property datatype jika object thing kosong; dalam semua kasus lainnya, object thing. | _Order 2. The graph node this fact points at: the thing, or one node per value and datatype._ |
| Subject Node Kind | Diambil dari subject yang tertaut. | _Order 19. How the graph draws the subject._ |
| Object Thing Node Kind | Diambil dari object thing yang tertaut. | _Order 19. How the graph draws the object thing._ |
| Object Node Kind | Ditentukan menurut prioritas: “value” jika object thing kosong; dalam semua kasus lainnya, object thing node kind. | _Order 20. How the graph draws the object: a value, a class, a place or a person._ |
| Object Value Node Label | Diambil dari property yang tertaut. | _Order 1. The small label under a value node._ |
| Count of Violations As First | Nilai total violation unit pada seluruh disjoint checks yang terkait dengan statement tersebut. | _Order 18. Violated disjointness checks naming this fact first._ |
| Count of Violations As Second | Nilai total violation unit pada seluruh disjoint checks yang terkait dengan statement tersebut. | _Order 18. Violated disjointness checks naming this fact second._ |
| Is in Contradiction | Bernilai benar jika paling sedikit satu dari kondisi berikut terpenuhi: count of violations as first lebih dari 0 atau count of violations as second lebih dari 0. | _Order 19. Part of a contradiction the disjoint rule found; drawn in the alarm colour._ |

<details open>
<summary>Contoh data</summary>

| Object Value | Is Listed | Is Built in | Is Detail | Is Stated | Activation Seq | Sort Order | Subject Label ƒ | Property Label ƒ | Object Label ƒ | Property Datatype ƒ | Property Has One Current Value ƒ | Property Domain Class ƒ | Property Range Class ƒ | Literal Quote ƒ | Literal Type Marker ƒ | Built in Source Text ƒ | User Source Text ƒ | Next Fact Sort Order ƒ | Is Literal ƒ | Object Text ƒ | Name ƒ | Words ƒ | Source ƒ | Subject Property Key ƒ | Latest Sibling Activation Seq ƒ | Is in Effect ƒ | Listed Unit ƒ | In Effect Unit ƒ | Listed Off Unit ƒ | Is Next to Add ƒ | Count of Inverse Firings ƒ | Count of Symmetric Firings ƒ | Holds Before Chain ƒ | Count of Chain Firings ƒ | Holds Before Typing ƒ | Count of Type Firings ƒ | Holds ƒ | Is Conclusion ƒ | Holds Unit ƒ | Conclusion Unit ƒ | Parent Unit ƒ | Grandparent Unit ƒ | Place Unit ƒ | First Pre Chain Order Key ƒ | Snap Position ƒ | First Fired Order Key ƒ | Count of Chosen Firings ƒ | Subject Node Key ƒ | Object Node Key ƒ | Subject Node Kind ƒ | Object Thing Node Kind ƒ | Object Node Kind ƒ | Object Value Node Label ƒ | Count of Violations As First ƒ | Count of Violations As Second ƒ | Is in Contradiction ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| — | true | true | false | true | 1 | 1 | Marvin | is parent of | Randy | — | false | — | — | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | — | false | Randy | Marvin is parent of Randy | Marvin is parent of Randy | Stated as a teaching example on this page. | Marvin\|isParentOf | 1 | true | 1 | 1 | 0 | false | 0 | 0 | true | 0 | true | 0 | true | — | 1 | 0 | 1 | 0 | 0 | — | 1.0 | — | 0 | Marvin | Randy | person | person | person | — | 0 | 0 | false |
| — | false | false | false | false | 0 | 100001 | Marvin | is parent of | Sharon | — | false | — | — | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | — | false | Sharon | Marvin is parent of Sharon | Marvin is parent of Sharon | Stated by you on this page. | Marvin\|isParentOf | 1 | false | 0 | 0 | 0 | false | 0 | 0 | — | 0 | — | 0 | — | — | 0 | 0 | 0 | 0 | 0 | — | 0.0 | — | 0 | Marvin | Sharon | person | person | person | — | 0 | 0 | false |
| — | false | false | false | false | 0 | 100002 | Marvin | is parent of | Stan | — | false | — | — | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | — | false | Stan | Marvin is parent of Stan | Marvin is parent of Stan | Stated by you on this page. | Marvin\|isParentOf | 1 | false | 0 | 0 | 0 | false | 0 | 0 | — | 0 | — | 0 | — | — | 0 | 0 | 0 | 0 | 0 | — | 0.0 | — | 0 | Marvin | Stan | person | person | person | — | 0 | 0 | false |

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
| Target Subject Node Key | Diambil dari target yang tertaut. | _Order 2. The concluded fact's subject node._ |
| Target Object Node Key | Diambil dari target yang tertaut. | _Order 3. The concluded fact's object node._ |
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
| Second is in Effect | Bernilai benar jika second yang tertaut pada firing tersebut tergolong “in effect”. | _Order 4. Whether the second premise is in effect._ |
| First Snap Position | Diambil dari first yang tertaut. | _Order 9. Where the reasoner meets the premise._ |
| Second Snap Position | Diambil dari second yang tertaut. | _Order 9. Where the reasoner meets the second premise._ |
| Inverse Fired Unit | Ditentukan menurut prioritas: 1 jika semua kondisi berikut terpenuhi: penanda “rule enabled” aktif, kind adalah “inverse”, dan penanda “first is in effect” aktif; dalam semua kasus lainnya, 0. | _Order 5. 1 when the inverse rule fires here._ |
| Symmetric Fired Unit | Ditentukan menurut prioritas: 1 jika semua kondisi berikut terpenuhi: penanda “rule enabled” aktif, kind adalah “symmetric”, dan penanda “first is in effect” aktif; dalam semua kasus lainnya, 0. | _Order 5. 1 when the symmetric rule fires here._ |
| Chain Fired Unit | Ditentukan menurut prioritas: 1 jika semua kondisi berikut terpenuhi: penanda “rule enabled” aktif, kind adalah “chain”, penanda “first holds before chain” aktif, dan penanda “second holds before chain” aktif; dalam semua kasus lainnya, 0. | _Order 9. 1 when the property chain fires here._ |
| Type Fired Unit | Ditentukan menurut prioritas: 1 jika semua kondisi berikut terpenuhi: penanda “rule enabled” aktif; paling sedikit satu dari kondisi berikut terpenuhi: kind adalah “domain” atau kind adalah “range”; dan penanda “first holds before typing” aktif; dalam semua kasus lainnya, 0. | _Order 13. 1 when the domain or range rule fires here._ |
| Fired Unit | Dihitung sebagai inverse fired unit ditambah symmetric fired unit ditambah chain fired unit ditambah type fired unit. | _Order 14. 1 when this application fires._ |
| Pre Chain Fired Unit | Dihitung sebagai inverse fired unit ditambah symmetric fired unit. | _Order 6. 1 when this is an inverse or symmetric application that fires._ |
| Pre Chain Order Key | Dihitung sebagai first sort order dikali 10 ditambah kind sort order. | _Order 2. For an inverse or symmetric application: its place in the reasoner's first round._ |
| Round | Ditentukan menurut prioritas: 2 jika semua kondisi berikut terpenuhi: kind adalah “chain” dan tidak benar bahwa semua kondisi berikut terpenuhi: penanda “first is in effect” aktif dan penanda “second is in effect” aktif; dalam semua kasus lainnya, 1. | _Order 5. The reasoner's round: 1 when every premise is a stated fact, 2 when the property chain reads a concluded fact._ |
| Order Key | Dihitung sebagai round dikali 1000000000000000 ditambah first snap position dikali 100000000 ditambah kind sort order dikali 10000000 ditambah 0 jika second kosong, dan dalam semua kasus lainnya second snap position. | _Order 10. The page reasoner's order: by round, then premise, then rule kind, then second premise._ |
| Is Chosen | Bernilai benar jika semua kondisi berikut terpenuhi: fired unit adalah 1, penanda “target is conclusion” aktif, dan order key adalah target first fired order key. | _Order 18. The application the conclusions panel shows for its fact: the first one in the reasoner's order._ |
| Chosen Unit | Ditentukan menurut prioritas: 1 jika penanda “chosen” aktif; dalam semua kasus lainnya, 0. | _Order 19. 1 when this application is shown._ |
| Card Text | Ditentukan menurut prioritas: “Worked out by the ”, diikuti kind wording, diikuti “ from “”, diikuti first words, diikuti “””, diikuti “ and “”, diikuti second words, diikuti “”” jika kind adalah “chain”, dan dalam semua kasus lainnya “ and “”, diikuti first property, diikuti “ has domain ”, diikuti first property domain class, diikuti “”” jika kind adalah “domain”, dan dalam semua kasus lainnya “ and “”, diikuti first property, diikuti “ has range ”, diikuti first property range class, diikuti “””, diikuti tanda titik jika penanda “chosen” aktif; dalam semua kasus lainnya, teks kosong. ⚠︎ mekanis <!-- rulespeak:reword --> | _Order 19. The conclusion card's provenance line, in the page's wording._ |
| Name | Sama dengan firing ID. | _Order 1. The application's key._ |

<details open>
<summary>Contoh data</summary>

| Rule Enabled ƒ | Kind Wording ƒ | Kind Sort Order ƒ | Target Name ƒ | Target Subject Label ƒ | Target Property Label ƒ | Target Object Text ƒ | Target is Conclusion ƒ | Target Subject Node Key ƒ | Target Object Node Key ƒ | Target First Fired Order Key ƒ | First Words ƒ | Second Words ƒ | First is in Effect ƒ | First Holds Before Chain ƒ | Second Holds Before Chain ƒ | First Holds Before Typing ƒ | First Property ƒ | First Property Domain Class ƒ | First Property Range Class ƒ | First Sort Order ƒ | Second is in Effect ƒ | First Snap Position ƒ | Second Snap Position ƒ | Inverse Fired Unit ƒ | Symmetric Fired Unit ƒ | Chain Fired Unit ƒ | Type Fired Unit ƒ | Fired Unit ƒ | Pre Chain Fired Unit ƒ | Pre Chain Order Key ƒ | Round ƒ | Order Key ƒ | Is Chosen ƒ | Chosen Unit ƒ | Card Text ƒ | Name ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| false | inverse rule | 1 | Randy is child of Marvin | Randy | is child of | Marvin | — | Randy | Marvin | — | Marvin is parent of Randy |    | true | true | — | true | isParentOf | — | — | 1 | false | 1.0 | 0.0 | 0 | 0 | 0 | 0 | 0 | 0 | 11.0 | 1 | 1000000110000000.0 | false | 0 | — | inverse:Marvin\|isParentOf\|Randy>Randy\|isChildOf\|Marvin |
| false | inverse rule | 1 | Sharon is child of Marvin | Sharon | is child of | Marvin | — | Sharon | Marvin | — | Marvin is parent of Sharon |    | false | — | — | — | isParentOf | — | — | 100001 | false | 0.0 | 0.0 | 0 | 0 | 0 | 0 | 0 | 0 | 1000011.0 | 1 | 1000000010000000.0 | false | 0 | — | inverse:Marvin\|isParentOf\|Sharon>Sharon\|isChildOf\|Marvin |
| false | inverse rule | 1 | Stan is child of Marvin | Stan | is child of | Marvin | — | Stan | Marvin | — | Marvin is parent of Stan |    | false | — | — | — | isParentOf | — | — | 100002 | false | 0.0 | 0.0 | 0 | 0 | 0 | 0 | 0 | 0 | 1000021.0 | 1 | 1000000010000000.0 | false | 0 | — | inverse:Marvin\|isParentOf\|Stan>Stan\|isChildOf\|Marvin |

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
| false | true | — | Marvin | Randy | 0 | — | Marvin\|isParentOf\|Randy~isSpouseOf |
| false | — | — | Marvin | Sharon | 0 | — | Marvin\|isParentOf\|Sharon~isSpouseOf |
| false | — | — | Marvin | Stan | 0 | — | Marvin\|isParentOf\|Stan~isSpouseOf |

_ƒ menandai kolom terhitung._

</details>

| Istilah | Deskripsi | Catatan Naratif |
|------|-------------|-------------------|
| **Scenario** | The page's buttons. Each one is a fact baseline, explicit fact changes and rule settings, applied in that order. | — |
| Graph | Atribut terdefinisi. | _The graph._ |
| Label | Atribut terdefinisi. | _The button's text._ |
| Note | Atribut terdefinisi. | _The note shown under the buttons after this one is pressed; blank clears it._ |
| Sort Order | Atribut terdefinisi. | _Position among the buttons._ |
| Button Group | Atribut terdefinisi. | _'stan' for the two imagined changes, 'facts' for the buttons under the fact list._ |
| Is Primary | Bernilai benar jika teks kosong. | _Drawn as the filled button._ |
| Fact Baseline | Atribut terdefinisi. | _What happens to every listed fact first: keep, built-in (teaching facts on, others off), built-in-detail, all-off, activate-all (each switched on in list order), or next (the next fact on)._ |
| Layout | Atribut terdefinisi. | _The node layout the graph returns to; blank keeps the current positions._ |
| Clears Selection | Bernilai benar jika teks kosong. | _Clears the selected node or line._ |
| Clears Form Message | Bernilai benar jika teks kosong. | _Clears the Add fact form's message._ |
| Name | Sama dengan label. | _Order 1. The button's text._ |
| Graph Has Facts to Add | Bernilai benar jika graph yang tertaut pada scenario tersebut memiliki “facts to add”. | _Order 7. Whether a listed fact can still be added._ |
| Is Available | Bernilai benar jika paling sedikit satu dari kondisi berikut terpenuhi: fact baseline bukan “next” atau penanda “graph has facts to add” aktif. | _Order 8. False only for Add next fact once every listed fact is in effect._ |

<details open>
<summary>Contoh data</summary>

| Label | Note | Sort Order | Button Group | Is Primary | Fact Baseline | Layout | Clears Selection | Clears Form Message | Name ƒ | Graph Has Facts to Add ƒ | Is Available ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|
| Move Stan to grade 5 | This imagined change updates Stan’s grade to 5. His identifier, Person statement and family links stay the same. His other selected details are unchanged. | 1 | stan | false | keep | — | true | true | Move Stan to grade 5 | false | true |
| Stan leaves school | This imagined change clears Stan’s current school details. His identifier, Person statement and family links remain. Removing a current fact does not say that its opposite is true; school history needs dates. | 2 | stan | false | keep | — | true | true | Stan leaves school | false | true |
| Add next fact | — | 3 | facts | true | next | — | false | false | Add next fact | false | false |

_ƒ menandai kolom terhitung._

</details>

| Istilah | Deskripsi | Catatan Naratif |
|------|-------------|-------------------|
| **Scenario Fact Change** | Fact changes a button makes after its baseline. A blank object key matches every listed value of the property. | — |
| Scenario | Atribut terdefinisi. | _The button._ |
| Subject | Atribut terdefinisi. | _The subject whose facts change._ |
| Property | Atribut terdefinisi. | _The property whose facts change._ |
| Object Key | Atribut terdefinisi. | _The object thing's key or the literal value; blank for every listed value._ |
| Action | Atribut terdefinisi. | _'on' switches the fact on as the newest value; 'off' switches it off._ |
| Adds to List | Bernilai benar jika teks kosong. | _Adds the fact to the fact list first when it is not listed._ |
| Sort Order | Atribut terdefinisi. | _Order of the change within the button._ |
| Name | Sama dengan scenario fact change ID. | _Order 1. The change's key._ |

<details open>
<summary>Contoh data</summary>

| Object Key | Action | Adds to List | Sort Order | Name ƒ |
|---|---|---|---|---|
| Person | on | true | 1 | stan-grade-five:Stan\|isA\|Person |
| 5 | on | true | 2 | stan-grade-five:Stan\|hasGradeLevel\|5 |
| Person | on | true | 3 | stan-leaves-school:Stan\|isA\|Person |

_ƒ menandai kolom terhitung._

</details>

| Istilah | Deskripsi | Catatan Naratif |
|------|-------------|-------------------|
| **Scenario Rule Setting** | Rule switches a button sets. | — |
| Scenario | Atribut terdefinisi. | _The button._ |
| Rule | Atribut terdefinisi. | _The rule switch._ |
| Is Enabled | Bernilai benar jika teks kosong. | _The state the button puts the switch in._ |
| Name | Sama dengan scenario rule setting ID. | _Order 1. The setting's key._ |

<details open>
<summary>Contoh data</summary>

| Is Enabled | Name ƒ |
|---|---|
| false | reset-original:inverse |
| false | reset-original:symmetric |
| false | reset-original:chain |

_ƒ menandai kolom terhitung._

</details>

| Istilah | Deskripsi | Catatan Naratif |
|------|-------------|-------------------|
| **Node Layout** | Node positions in the graph's 380 by 480 drawing, one set per layout. | — |
| Layout | Atribut terdefinisi. | _The layout: 'default' or 'stan-details'._ |
| Node Key | Atribut terdefinisi. | _The node: a thing's key, or literal\|value\|datatype._ |
| X | Atribut terdefinisi. | _Horizontal position._ |
| Y | Atribut terdefinisi. | _Vertical position._ |
| Name | Sama dengan node layout ID. | _Order 1. The position's key._ |

<details open>
<summary>Contoh data</summary>

| Layout | Node Key | X | Y | Name ƒ |
|---|---|---|---|---|
| default | Marvin | 70 | 45 | default:Marvin |
| default | Randy | 195 | 125 | default:Randy |
| default | Sharon | 318 | 55 | default:Sharon |

_ƒ menandai kolom terhitung._

</details>

| Istilah | Deskripsi | Catatan Naratif |
|------|-------------|-------------------|
| **Layout Slot** | Places for nodes no layout positions, used in turn. | — |
| Sort Order | Atribut terdefinisi. | _Order in which slots are used._ |
| X | Atribut terdefinisi. | _Horizontal position._ |
| Y | Atribut terdefinisi. | _Vertical position._ |
| Name | Sama dengan layout slot ID. | _Order 1. The slot's key._ |

<details open>
<summary>Contoh data</summary>

| Sort Order | X | Y | Name ƒ |
|---|---|---|---|
| 1 | 330 | 135 | slot-1 |
| 2 | 45 | 120 | slot-2 |
| 3 | 330 | 400 | slot-3 |

_ƒ menandai kolom terhitung._

</details>

| Istilah | Deskripsi | Catatan Naratif |
|------|-------------|-------------------|
| **Page Text** | The page's wording, one row per string. {name}-style placeholders are filled by the page. | — |
| Text | Atribut terdefinisi. | _The string._ |
| Name | Sama dengan page text ID. | _Order 1. The string's key._ |

<details open>
<summary>Contoh data</summary>

| Text | Name ƒ |
|---|---|
| South Park Ontology Explainer, on one rulebook | page-title |
| A rulebook rebuild of the explainer by | byline-prefix |
| Jessica Talisman | byline-author |

_ƒ menandai kolom terhitung._

</details>

| Istilah | Deskripsi | Catatan Naratif |
|------|-------------|-------------------|
| **Engine** | The execution engines the page can be computed by. Each is a build output of this rulebook. | — |
| Label | Atribut terdefinisi. | _The engine's name in the switcher._ |
| Substrate | Atribut terdefinisi. | _The conformance harness's name for this substrate._ |
| Is Default | Bernilai benar jika teks kosong. | _The engine a new visitor starts on._ |
| Description | Atribut terdefinisi. | _What computes the page on this engine._ |
| Sort Order | Atribut terdefinisi. | _Position in the switcher._ |
| Name | Sama dengan label. | _Order 1. The engine's name._ |

<details open>
<summary>Contoh data</summary>

| Label | Substrate | Is Default | Description | Sort Order | Name ƒ |
|---|---|---|---|---|---|
| Postgres views | effortless-postgres | true | Every value is a column of a generated view; the database computes it when the page reads it. | 1 | Postgres views |
| TypeScript | effortless-typescript | false | The generated TypeScript computes every field from the same rows, with no database. | 2 | TypeScript |
| OWL and SHACL rules | effortless-owl | false | The generated ontology and SHACL rules, run over the same rows by a SPARQL engine. | 3 | OWL and SHACL rules |

_ƒ menandai kolom terhitung._

</details>

| Istilah | Deskripsi | Catatan Naratif |
|------|-------------|-------------------|
| **Rulebook Tool** | What each rulebook tool adds, for each form of the rulebook. The explainer page and the ontology form's editor list the ontology form's tools; the concept form's editor lists the concept form's. The two forms are the same facts, so they list the same tools. | — |
| Label | Atribut terdefinisi. | _The tool's name._ |
| Adds | Atribut terdefinisi. | _What it adds, for this form of the rulebook._ |
| Href | Atribut terdefinisi. | _Where its link goes; 'editor' means this form's running rulebook editor._ |
| Link Text | Atribut terdefinisi. | _The link's text._ |
| Sort Order | Atribut terdefinisi. | _Position in the panel._ |
| Form | Atribut terdefinisi. | _Which form of the rulebook the tool is about: ontology (things, properties, statements) or concept (tables)._ |
| Name | Sama dengan label. | _Order 1. The tool's name._ |

<details open>
<summary>Contoh data</summary>

| Label | Adds | Href | Link Text | Sort Order | Form | Name ƒ |
|---|---|---|---|---|---|---|
| Interchangeable engines | The same page, computed by Postgres views, generated TypeScript or the generated OWL and SHACL rules. Switch with the control above; nothing else changes. | — | — | 1 | ontology | Interchangeable engines |
| Conformance | Every engine's answers for the current facts, compared value by value with the conformance harness's own rule for when two values are equal. | — | — | 2 | ontology | Conformance |
| Explainer | Hover a dotted value for its rule in plain words and its witness: the values it came from, down to the stated facts. | /rulebook-explainer-dag/pages/index.html | Open the explainer | 3 | ontology | Explainer |

_ƒ menandai kolom terhitung._

</details>

## 2 Jenis Fakta

- Setiap **firing kind** merujuk pada tepat satu **rule**
- Setiap **thing** dapat merujuk pada satu **portrait**
- Setiap **portrait aliase** merujuk pada tepat satu **portrait**
- Setiap **property** dapat merujuk pada satu **thing**
- Setiap **property axiom** merujuk pada tepat satu **firing kind**
- Setiap **property axiom** merujuk pada tepat satu **property**
- Setiap **statement** merujuk pada tepat satu **graph**
- Setiap **statement** merujuk pada tepat satu **thing**
- Setiap **statement** merujuk pada tepat satu **property**
- Setiap **firing** merujuk pada tepat satu **firing kind**
- Setiap **firing** merujuk pada tepat satu **rule**
- Setiap **firing** merujuk pada tepat satu **statement**
- Setiap **disjoint check** merujuk pada tepat satu **graph**
- Setiap **disjoint check** merujuk pada tepat satu **rule**
- Setiap **disjoint check** merujuk pada tepat satu **statement**
- Setiap **scenario** merujuk pada tepat satu **graph**
- Setiap **scenario fact change** merujuk pada tepat satu **scenario**
- Setiap **scenario fact change** merujuk pada tepat satu **thing**
- Setiap **scenario fact change** merujuk pada tepat satu **property**
- Setiap **scenario rule setting** merujuk pada tepat satu **scenario**
- Setiap **scenario rule setting** merujuk pada tepat satu **rule**

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
- Setiap portrait **harus** memiliki display name, page, image, crop, width, height, dan sort order.
- Setiap portrait aliase **harus** merujuk pada tepat satu portrait.
- Setiap property **harus** memiliki label, property kind, dan sort order, serta menyatakan apakah tergolong “user addable”, apakah memiliki “one current value”, dan apakah tergolong “symmetric”.
- Setiap property axiom **harus** merujuk pada tepat satu firing kind sebagai kind.
- Setiap property axiom **harus** merujuk pada tepat satu property.
- Setiap property axiom **harus** merujuk pada tepat satu property sebagai first.
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
- Setiap scenario **harus** merujuk pada tepat satu graph.
- Setiap scenario **harus** memiliki label, sort order, button group, dan fact baseline, serta menyatakan apakah tergolong “primary”, apakah memenuhi kondisi “clears selection”, dan apakah memenuhi kondisi “clears form message”.
- Setiap scenario fact change **harus** merujuk pada tepat satu scenario.
- Setiap scenario fact change **harus** merujuk pada tepat satu thing sebagai subject.
- Setiap scenario fact change **harus** merujuk pada tepat satu property.
- Setiap scenario fact change **harus** memiliki action dan sort order, serta menyatakan apakah memenuhi kondisi “adds to list”.
- Setiap scenario rule setting **harus** merujuk pada tepat satu scenario.
- Setiap scenario rule setting **harus** merujuk pada tepat satu rule.
- Setiap scenario rule setting **harus** menyatakan apakah tergolong “enabled”.
- Setiap node layout **harus** memiliki layout, node key, x, dan y.
- Setiap layout slot **harus** memiliki sort order, x, dan y.
- Setiap page text **harus** memiliki text.
- Setiap engine **harus** memiliki label, substrate, description, dan sort order, serta menyatakan apakah tergolong “default”.
- Setiap rulebook tool **harus** memiliki label, adds, sort order, dan form.

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
| **DR-14 Max Sort Order** | Untuk setiap graph, max sort order adalah nilai maksimum sort order pada seluruh statements yang terkait dengan graph tersebut. |
| **DR-15 Next Candidate Sort Order** | Untuk setiap graph, next candidate sort order dihitung sebagai max sort order ditambah 1. |
| **DR-16 Name** | Untuk setiap rule, name sama dengan label. |
| **DR-17 Count of Firings** | Untuk setiap rule, count of firings adalah nilai total fired unit pada seluruh firings yang terkait dengan rule tersebut. |
| **DR-18 Name** | Untuk setiap firing kind, name sama dengan wording. |
| **DR-19 Rule Enabled** | Untuk setiap firing kind, rule enabled bernilai benar jika rule yang tertaut pada firing kind tersebut tergolong “enabled”. |
| **DR-20 Portrait Name** | Untuk setiap thing, portrait name adalah display name dari portrait yang tertaut pada thing tersebut. |
| **DR-21 Portrait Page** | Untuk setiap thing, portrait page diambil dari portrait yang tertaut. |
| **DR-22 Portrait Image** | Untuk setiap thing, portrait image diambil dari portrait yang tertaut. |
| **DR-23 Portrait Crop** | Untuk setiap thing, portrait crop diambil dari portrait yang tertaut. |
| **DR-24 Portrait Width** | Untuk setiap thing, portrait width diambil dari portrait yang tertaut. |
| **DR-25 Portrait Height** | Untuk setiap thing, portrait height diambil dari portrait yang tertaut. |
| **DR-26 Name** | Untuk setiap thing, name sama dengan label. |
| **DR-27 Node Key** | Untuk setiap thing, node key sama dengan thing ID. |
| **DR-28 Count of Children** | Untuk setiap thing, count of children adalah nilai total parent unit pada seluruh statements yang terkait dengan thing tersebut. |
| **DR-29 Count of Grandchildren** | Untuk setiap thing, count of grandchildren adalah nilai total grandparent unit pada seluruh statements yang terkait dengan thing tersebut. |
| **DR-30 Count As Place** | Untuk setiap thing, count as place adalah nilai total place unit pada seluruh statements yang terkait dengan thing tersebut. |
| **DR-31 Count As Subject** | Untuk setiap thing, count as subject adalah nilai total holds unit pada seluruh statements yang terkait dengan thing tersebut. |
| **DR-32 Count As Object** | Untuk setiap thing, count as object adalah nilai total holds unit pada seluruh statements yang terkait dengan thing tersebut. |
| **DR-33 Is in Graph** | Setiap thing dianggap “in graph” jika paling sedikit satu dari kondisi berikut terpenuhi: count as subject lebih dari 0 atau count as object lebih dari 0. |
| **DR-34 Node Kind** | Untuk setiap thing, node kind ditentukan menurut urutan prioritas berikut:<br>1. teks kosong, jika kind kosong;<br>2. “class”, jika kind adalah “class”;<br>3. “place”, jika count as place lebih dari 0;<br>4. dalam semua kasus lainnya, “person”. |
| **DR-35 Name** | Untuk setiap portrait, name sama dengan display name. |
| **DR-36 Name** | Untuk setiap portrait aliase, name sama dengan portrait alias ID. |
| **DR-37 Name** | Untuk setiap property, name sama dengan label. |
| **DR-38 Name** | Untuk setiap property axiom, name sama dengan property axiom ID. |
| **DR-39 Subject Label** | Untuk setiap statement, subject label diambil dari subject yang tertaut. |
| **DR-40 Property Label** | Untuk setiap statement, property label diambil dari property yang tertaut. |
| **DR-41 Object Label** | Untuk setiap statement, object label diambil dari object thing yang tertaut. |
| **DR-42 Property Datatype** | Untuk setiap statement, property datatype diambil dari property yang tertaut. |
| **DR-43 Property Has One Current Value** | Untuk setiap statement, property has one current value bernilai benar jika property yang tertaut pada statement tersebut memiliki “one current value”. |
| **DR-44 Property Domain Class** | Untuk setiap statement, property domain class diambil dari property yang tertaut. |
| **DR-45 Property Range Class** | Untuk setiap statement, property range class diambil dari property yang tertaut. |
| **DR-46 Literal Quote** | Untuk setiap statement, literal quote diambil dari graph yang tertaut. |
| **DR-47 Literal Type Marker** | Untuk setiap statement, literal type marker diambil dari graph yang tertaut. |
| **DR-48 Built in Source Text** | Untuk setiap statement, built in source text diambil dari graph yang tertaut. |
| **DR-49 User Source Text** | Untuk setiap statement, user source text diambil dari graph yang tertaut. |
| **DR-50 Next Fact Sort Order** | Untuk setiap statement, next fact sort order diambil dari graph yang tertaut. |
| **DR-51 Is Literal** | Setiap statement dianggap “literal” jika object thing kosong. |
| **DR-52 Object Text** | Untuk setiap statement, object text ditentukan menurut urutan prioritas berikut:<br>1. object value, jika object thing kosong;<br>2. dalam semua kasus lainnya, object label. |
| **DR-53 Name** | Untuk setiap statement, name dihitung sebagai subject label, diikuti spasi, diikuti property label, diikuti spasi, diikuti object text. |
| **DR-54 Words** | Untuk setiap statement, words dihitung sebagai subject label, diikuti spasi, diikuti property label, diikuti spasi, diikuti literal quote, diikuti object value, diikuti literal quote, diikuti literal type marker, diikuti property datatype jika object thing kosong, dan dalam semua kasus lainnya object label. ⚠︎ mekanis <!-- rulespeak:reword --> |
| **DR-55 Source** | Untuk setiap statement, source ditentukan menurut urutan prioritas berikut:<br>1. built in source text, jika penanda “built in” aktif;<br>2. dalam semua kasus lainnya, user source text. |
| **DR-56 Subject Property Key** | Untuk setiap statement, subject property key dihitung sebagai subject, diikuti “|”, diikuti property. |
| **DR-57 Latest Sibling Activation Seq** | Untuk setiap statement, latest sibling activation seq adalah nilai maksimum activation seq pada seluruh statements yang terkait dengan statement tersebut. |
| **DR-58 Is in Effect** | Setiap statement dianggap “in effect” jika semua kondisi berikut terpenuhi: penanda “stated” aktif dan paling sedikit satu dari kondisi berikut terpenuhi: penanda “property has one current value” tidak aktif atau activation seq adalah latest sibling activation seq. |
| **DR-59 Listed Unit** | Untuk setiap statement, listed unit ditentukan menurut urutan prioritas berikut:<br>1. 1, jika penanda “listed” aktif;<br>2. dalam semua kasus lainnya, 0. |
| **DR-60 In Effect Unit** | Untuk setiap statement, in effect unit ditentukan menurut urutan prioritas berikut:<br>1. 1, jika penanda “in effect” aktif;<br>2. dalam semua kasus lainnya, 0. |
| **DR-61 Listed Off Unit** | Untuk setiap statement, listed off unit ditentukan menurut urutan prioritas berikut:<br>1. 1, jika semua kondisi berikut terpenuhi: penanda “listed” aktif dan penanda “in effect” tidak aktif;<br>2. dalam semua kasus lainnya, 0. |
| **DR-62 Is Next to Add** | Setiap statement dianggap “next to add” jika semua kondisi berikut terpenuhi: listed off unit adalah 1 dan sort order adalah next fact sort order. |
| **DR-63 Count of Inverse Firings** | Untuk setiap statement, count of inverse firings adalah nilai total inverse fired unit pada seluruh firings yang terkait dengan statement tersebut. |
| **DR-64 Count of Symmetric Firings** | Untuk setiap statement, count of symmetric firings adalah nilai total symmetric fired unit pada seluruh firings yang terkait dengan statement tersebut. |
| **DR-65 Holds Before Chain** | Setiap statement dianggap memenuhi kondisi “holds before chain” jika hasil fungsi IF atas hasil fungsi OR atas is in effect, count of inverse firings lebih dari 0, dan count of symmetric firings lebih dari 0 dan benar. |
| **DR-66 Count of Chain Firings** | Untuk setiap statement, count of chain firings adalah nilai total chain fired unit pada seluruh firings yang terkait dengan statement tersebut. |
| **DR-67 Holds Before Typing** | Setiap statement dianggap memenuhi kondisi “holds before typing” jika hasil fungsi IF atas hasil fungsi OR atas holds before chain dan count of chain firings lebih dari 0 dan benar. |
| **DR-68 Count of Type Firings** | Untuk setiap statement, count of type firings adalah nilai total type fired unit pada seluruh firings yang terkait dengan statement tersebut. |
| **DR-69 Holds** | Setiap statement dianggap memenuhi kondisi “holds” jika hasil fungsi IF atas hasil fungsi OR atas holds before typing dan count of type firings lebih dari 0 dan benar. |
| **DR-70 Is Conclusion** | Setiap statement dianggap “conclusion” jika hasil fungsi IF atas hasil fungsi AND atas holds dan penanda “in effect” tidak aktif dan benar. |
| **DR-71 Holds Unit** | Untuk setiap statement, holds unit ditentukan menurut urutan prioritas berikut:<br>1. 1, jika penanda “holds” aktif;<br>2. dalam semua kasus lainnya, 0. |
| **DR-72 Conclusion Unit** | Untuk setiap statement, conclusion unit ditentukan menurut urutan prioritas berikut:<br>1. 1, jika penanda “conclusion” aktif;<br>2. dalam semua kasus lainnya, 0. |
| **DR-73 Parent Unit** | Untuk setiap statement, parent unit ditentukan menurut urutan prioritas berikut:<br>1. 1, jika semua kondisi berikut terpenuhi: penanda “holds” aktif dan property adalah “isParentOf”;<br>2. dalam semua kasus lainnya, 0. |
| **DR-74 Grandparent Unit** | Untuk setiap statement, grandparent unit ditentukan menurut urutan prioritas berikut:<br>1. 1, jika semua kondisi berikut terpenuhi: penanda “holds” aktif dan property adalah “isGrandparentOf”;<br>2. dalam semua kasus lainnya, 0. |
| **DR-75 Place Unit** | Untuk setiap statement, place unit ditentukan menurut urutan prioritas berikut:<br>1. 1, jika semua kondisi berikut terpenuhi: penanda “holds” aktif dan property adalah “attends”;<br>2. dalam semua kasus lainnya, 0. |
| **DR-76 First Pre Chain Order Key** | Untuk setiap statement, first pre chain order key adalah nilai minimum pre chain order key pada seluruh firings dari statement tersebut yang tergolong “pre chain fired unit”. |
| **DR-77 Snap Position** | Untuk setiap statement, snap position ditentukan menurut urutan prioritas berikut:<br>1. sort order, jika penanda “in effect” aktif;<br>2. 100000 ditambah first pre chain order key, jika penanda “holds before chain” aktif;<br>3. dalam semua kasus lainnya, 0. |
| **DR-78 First Fired Order Key** | Untuk setiap statement, first fired order key adalah nilai minimum order key pada seluruh firings dari statement tersebut yang tergolong “fired unit”. |
| **DR-79 Count of Chosen Firings** | Untuk setiap statement, count of chosen firings adalah nilai total chosen unit pada seluruh firings yang terkait dengan statement tersebut. |
| **DR-80 Subject Node Key** | Untuk setiap statement, subject node key sama dengan subject. |
| **DR-81 Object Node Key** | Untuk setiap statement, object node key ditentukan menurut urutan prioritas berikut:<br>1. “literal|”, diikuti object value, diikuti “|”, diikuti property datatype, jika object thing kosong;<br>2. dalam semua kasus lainnya, object thing. |
| **DR-82 Subject Node Kind** | Untuk setiap statement, subject node kind diambil dari subject yang tertaut. |
| **DR-83 Object Thing Node Kind** | Untuk setiap statement, object thing node kind diambil dari object thing yang tertaut. |
| **DR-84 Object Node Kind** | Untuk setiap statement, object node kind ditentukan menurut urutan prioritas berikut:<br>1. “value”, jika object thing kosong;<br>2. dalam semua kasus lainnya, object thing node kind. |
| **DR-85 Object Value Node Label** | Untuk setiap statement, object value node label diambil dari property yang tertaut. |
| **DR-86 Count of Violations As First** | Untuk setiap statement, count of violations as first adalah nilai total violation unit pada seluruh disjoint checks yang terkait dengan statement tersebut. |
| **DR-87 Count of Violations As Second** | Untuk setiap statement, count of violations as second adalah nilai total violation unit pada seluruh disjoint checks yang terkait dengan statement tersebut. |
| **DR-88 Is in Contradiction** | Setiap statement dianggap “in contradiction” jika paling sedikit satu dari kondisi berikut terpenuhi: count of violations as first lebih dari 0 atau count of violations as second lebih dari 0. |
| **DR-89 Rule Enabled** | Untuk setiap firing, rule enabled bernilai benar jika rule yang tertaut pada firing tersebut tergolong “enabled”. |
| **DR-90 Kind Wording** | Untuk setiap firing, kind wording diambil dari kind yang tertaut. |
| **DR-91 Kind Sort Order** | Untuk setiap firing, kind sort order diambil dari kind yang tertaut. |
| **DR-92 Target Name** | Untuk setiap firing, target name diambil dari target yang tertaut. |
| **DR-93 Target Subject Label** | Untuk setiap firing, target subject label diambil dari target yang tertaut. |
| **DR-94 Target Property Label** | Untuk setiap firing, target property label diambil dari target yang tertaut. |
| **DR-95 Target Object Text** | Untuk setiap firing, target object text diambil dari target yang tertaut. |
| **DR-96 Target is Conclusion** | Untuk setiap firing, target is conclusion bernilai benar jika target yang tertaut pada firing tersebut tergolong “conclusion”. |
| **DR-97 Target Subject Node Key** | Untuk setiap firing, target subject node key diambil dari target yang tertaut. |
| **DR-98 Target Object Node Key** | Untuk setiap firing, target object node key diambil dari target yang tertaut. |
| **DR-99 Target First Fired Order Key** | Untuk setiap firing, target first fired order key diambil dari target yang tertaut. |
| **DR-100 First Words** | Untuk setiap firing, first words diambil dari first yang tertaut. |
| **DR-101 Second Words** | Untuk setiap firing, second words diambil dari second yang tertaut. |
| **DR-102 First is in Effect** | Untuk setiap firing, first is in effect bernilai benar jika first yang tertaut pada firing tersebut tergolong “in effect”. |
| **DR-103 First Holds Before Chain** | Untuk setiap firing, first holds before chain bernilai benar jika first yang tertaut pada firing tersebut memenuhi kondisi “holds before chain”. |
| **DR-104 Second Holds Before Chain** | Untuk setiap firing, second holds before chain bernilai benar jika second yang tertaut pada firing tersebut memenuhi kondisi “holds before chain”. |
| **DR-105 First Holds Before Typing** | Untuk setiap firing, first holds before typing bernilai benar jika first yang tertaut pada firing tersebut memenuhi kondisi “holds before typing”. |
| **DR-106 First Property** | Untuk setiap firing, first property diambil dari first yang tertaut. |
| **DR-107 First Property Domain Class** | Untuk setiap firing, first property domain class diambil dari first yang tertaut. |
| **DR-108 First Property Range Class** | Untuk setiap firing, first property range class diambil dari first yang tertaut. |
| **DR-109 First Sort Order** | Untuk setiap firing, first sort order diambil dari first yang tertaut. |
| **DR-110 Second is in Effect** | Untuk setiap firing, second is in effect bernilai benar jika second yang tertaut pada firing tersebut tergolong “in effect”. |
| **DR-111 First Snap Position** | Untuk setiap firing, first snap position diambil dari first yang tertaut. |
| **DR-112 Second Snap Position** | Untuk setiap firing, second snap position diambil dari second yang tertaut. |
| **DR-113 Inverse Fired Unit** | Untuk setiap firing, inverse fired unit ditentukan menurut urutan prioritas berikut:<br>1. 1, jika semua kondisi berikut terpenuhi: penanda “rule enabled” aktif, kind adalah “inverse”, dan penanda “first is in effect” aktif;<br>2. dalam semua kasus lainnya, 0. |
| **DR-114 Symmetric Fired Unit** | Untuk setiap firing, symmetric fired unit ditentukan menurut urutan prioritas berikut:<br>1. 1, jika semua kondisi berikut terpenuhi: penanda “rule enabled” aktif, kind adalah “symmetric”, dan penanda “first is in effect” aktif;<br>2. dalam semua kasus lainnya, 0. |
| **DR-115 Chain Fired Unit** | Untuk setiap firing, chain fired unit ditentukan menurut urutan prioritas berikut:<br>1. 1, jika semua kondisi berikut terpenuhi: penanda “rule enabled” aktif, kind adalah “chain”, penanda “first holds before chain” aktif, dan penanda “second holds before chain” aktif;<br>2. dalam semua kasus lainnya, 0. |
| **DR-116 Type Fired Unit** | Untuk setiap firing, type fired unit ditentukan menurut urutan prioritas berikut:<br>1. 1, jika semua kondisi berikut terpenuhi: penanda “rule enabled” aktif; paling sedikit satu dari kondisi berikut terpenuhi: kind adalah “domain” atau kind adalah “range”; dan penanda “first holds before typing” aktif;<br>2. dalam semua kasus lainnya, 0. |
| **DR-117 Fired Unit** | Untuk setiap firing, fired unit dihitung sebagai inverse fired unit ditambah symmetric fired unit ditambah chain fired unit ditambah type fired unit. |
| **DR-118 Pre Chain Fired Unit** | Untuk setiap firing, pre chain fired unit dihitung sebagai inverse fired unit ditambah symmetric fired unit. |
| **DR-119 Pre Chain Order Key** | Untuk setiap firing, pre chain order key dihitung sebagai first sort order dikali 10 ditambah kind sort order. |
| **DR-120 Round** | Untuk setiap firing, round ditentukan menurut urutan prioritas berikut:<br>1. 2, jika semua kondisi berikut terpenuhi: kind adalah “chain” dan tidak benar bahwa semua kondisi berikut terpenuhi: penanda “first is in effect” aktif dan penanda “second is in effect” aktif;<br>2. dalam semua kasus lainnya, 1. |
| **DR-121 Order Key** | Untuk setiap firing, order key dihitung sebagai round dikali 1000000000000000 ditambah first snap position dikali 100000000 ditambah kind sort order dikali 10000000 ditambah 0 jika second kosong, dan dalam semua kasus lainnya second snap position. |
| **DR-122 Is Chosen** | Setiap firing dianggap “chosen” jika semua kondisi berikut terpenuhi: fired unit adalah 1, penanda “target is conclusion” aktif, dan order key adalah target first fired order key. |
| **DR-123 Chosen Unit** | Untuk setiap firing, chosen unit ditentukan menurut urutan prioritas berikut:<br>1. 1, jika penanda “chosen” aktif;<br>2. dalam semua kasus lainnya, 0. |
| **DR-124 Card Text** | Untuk setiap firing, card text ditentukan menurut urutan prioritas berikut:<br>1. “Worked out by the ”, diikuti kind wording, diikuti “ from “”, diikuti first words, diikuti “””, diikuti “ and “”, diikuti second words, diikuti “”” jika kind adalah “chain”, dan dalam semua kasus lainnya “ and “”, diikuti first property, diikuti “ has domain ”, diikuti first property domain class, diikuti “”” jika kind adalah “domain”, dan dalam semua kasus lainnya “ and “”, diikuti first property, diikuti “ has range ”, diikuti first property range class, diikuti “””, diikuti tanda titik, jika penanda “chosen” aktif;<br>2. dalam semua kasus lainnya, teks kosong. ⚠︎ mekanis <!-- rulespeak:reword --> |
| **DR-125 Name** | Untuk setiap firing, name sama dengan firing ID. |
| **DR-126 Rule Enabled** | Untuk setiap disjoint check, rule enabled bernilai benar jika rule yang tertaut pada disjoint check tersebut tergolong “enabled”. |
| **DR-127 First Holds** | Untuk setiap disjoint check, first holds bernilai benar jika first yang tertaut pada disjoint check tersebut memenuhi kondisi “holds”. |
| **DR-128 Second Holds** | Untuk setiap disjoint check, second holds bernilai benar jika second yang tertaut pada disjoint check tersebut memenuhi kondisi “holds”. |
| **DR-129 Subject Label** | Untuk setiap disjoint check, subject label diambil dari first yang tertaut. |
| **DR-130 Object Label** | Untuk setiap disjoint check, object label diambil dari first yang tertaut. |
| **DR-131 Violation Unit** | Untuk setiap disjoint check, violation unit ditentukan menurut urutan prioritas berikut:<br>1. 1, jika semua kondisi berikut terpenuhi: penanda “rule enabled” aktif, penanda “first holds” aktif, dan penanda “second holds” aktif;<br>2. dalam semua kasus lainnya, 0. |
| **DR-132 Alarm Text** | Untuk setiap disjoint check, alarm text ditentukan menurut urutan prioritas berikut:<br>1. “Contradiction found. ”, diikuti subject label, diikuti “ is recorded as both parent and spouse of ”, diikuti object label, diikuti “. Check those facts and the selected disjoint rule.”, jika violation unit adalah 1;<br>2. dalam semua kasus lainnya, teks kosong. |
| **DR-133 Name** | Untuk setiap disjoint check, name sama dengan disjoint check ID. |
| **DR-134 Name** | Untuk setiap scenario, name sama dengan label. |
| **DR-135 Graph Has Facts to Add** | Untuk setiap scenario, graph has facts to add bernilai benar jika graph yang tertaut pada scenario tersebut memiliki “facts to add”. |
| **DR-136 Is Available** | Setiap scenario dianggap “available” jika paling sedikit satu dari kondisi berikut terpenuhi: fact baseline bukan “next” atau penanda “graph has facts to add” aktif. |
| **DR-137 Name** | Untuk setiap scenario fact change, name sama dengan scenario fact change ID. |
| **DR-138 Name** | Untuk setiap scenario rule setting, name sama dengan scenario rule setting ID. |
| **DR-139 Name** | Untuk setiap node layout, name sama dengan node layout ID. |
| **DR-140 Name** | Untuk setiap layout slot, name sama dengan layout slot ID. |
| **DR-141 Name** | Untuk setiap page text, name sama dengan page text ID. |
| **DR-142 Name** | Untuk setiap engine, name sama dengan label. |
| **DR-143 Name** | Untuk setiap rulebook tool, name sama dengan label. |

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
| **Graphs.MaxSortOrder** | agregasi | `Max(Statements.SortOrder via Graph)` |
| **Graphs.NextCandidateSortOrder** | rumus | `MaxSortOrder + 1` |
| **Rules.Name** | rumus | `Label` |
| **Rules.CountOfFirings** | agregasi | `Sum(Firings.FiredUnit via Rule)` |
| **FiringKinds.Name** | rumus | `Wording` |
| **FiringKinds.RuleEnabled** | pencarian | `Lookup(Rules.IsEnabled via Rule)` |
| **Things.PortraitName** | pencarian | `Lookup(Portraits.DisplayName via Portrait)` |
| **Things.PortraitPage** | pencarian | `Lookup(Portraits.Page via Portrait)` |
| **Things.PortraitImage** | pencarian | `Lookup(Portraits.Image via Portrait)` |
| **Things.PortraitCrop** | pencarian | `Lookup(Portraits.Crop via Portrait)` |
| **Things.PortraitWidth** | pencarian | `Lookup(Portraits.Width via Portrait)` |
| **Things.PortraitHeight** | pencarian | `Lookup(Portraits.Height via Portrait)` |
| **Things.Name** | rumus | `Label` |
| **Things.NodeKey** | rumus | `ThingId` |
| **Things.CountOfChildren** | agregasi | `Sum(Statements.ParentUnit via Subject)` |
| **Things.CountOfGrandchildren** | agregasi | `Sum(Statements.GrandparentUnit via Subject)` |
| **Things.CountAsPlace** | agregasi | `Sum(Statements.PlaceUnit via ObjectThing)` |
| **Things.CountAsSubject** | agregasi | `Sum(Statements.HoldsUnit via Subject)` |
| **Things.CountAsObject** | agregasi | `Sum(Statements.HoldsUnit via ObjectThing)` |
| **Things.IsInGraph** | rumus | `Or(CountAsSubject > 0, CountAsObject > 0)` |
| **Things.NodeKind** | rumus | `If(Kind = "", "", If(Kind = "class", "class", If(CountAsPlace > 0, "place", "person")))` |
| **Portraits.Name** | rumus | `DisplayName` |
| **PortraitAliases.Name** | rumus | `PortraitAliasId` |
| **Properties.Name** | rumus | `Label` |
| **PropertyAxioms.Name** | rumus | `PropertyAxiomId` |
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
| **Statements.HoldsBeforeChain** | rumus | `If(Or(IsInEffect, CountOfInverseFirings > 0, CountOfSymmetricFirings > 0), True())` |
| **Statements.CountOfChainFirings** | agregasi | `Sum(Firings.ChainFiredUnit via Target)` |
| **Statements.HoldsBeforeTyping** | rumus | `If(Or(HoldsBeforeChain, CountOfChainFirings > 0), True())` |
| **Statements.CountOfTypeFirings** | agregasi | `Sum(Firings.TypeFiredUnit via Target)` |
| **Statements.Holds** | rumus | `If(Or(HoldsBeforeTyping, CountOfTypeFirings > 0), True())` |
| **Statements.IsConclusion** | rumus | `If(And(Holds, Not(IsInEffect)), True())` |
| **Statements.HoldsUnit** | rumus | `If(Holds, 1, 0)` |
| **Statements.ConclusionUnit** | rumus | `If(IsConclusion, 1, 0)` |
| **Statements.ParentUnit** | rumus | `If(And(Holds, Property = "isParentOf"), 1, 0)` |
| **Statements.GrandparentUnit** | rumus | `If(And(Holds, Property = "isGrandparentOf"), 1, 0)` |
| **Statements.PlaceUnit** | rumus | `If(And(Holds, Property = "attends"), 1, 0)` |
| **Statements.FirstPreChainOrderKey** | agregasi | `Min(Firings.PreChainOrderKey via Target)` |
| **Statements.SnapPosition** | rumus | `If(IsInEffect, SortOrder, If(HoldsBeforeChain, 100000 + FirstPreChainOrderKey, 0))` |
| **Statements.FirstFiredOrderKey** | agregasi | `Min(Firings.OrderKey via Target)` |
| **Statements.CountOfChosenFirings** | agregasi | `Sum(Firings.ChosenUnit via Target)` |
| **Statements.SubjectNodeKey** | rumus | `Subject` |
| **Statements.ObjectNodeKey** | rumus | `If(ObjectThing = "", Concat("literal\|", ObjectValue, "\|", PropertyDatatype), ObjectThing)` |
| **Statements.SubjectNodeKind** | pencarian | `Lookup(Things.NodeKind via Subject)` |
| **Statements.ObjectThingNodeKind** | pencarian | `Lookup(Things.NodeKind via ObjectThing)` |
| **Statements.ObjectNodeKind** | rumus | `If(ObjectThing = "", "value", ObjectThingNodeKind)` |
| **Statements.ObjectValueNodeLabel** | pencarian | `Lookup(Properties.ValueNodeLabel via Property)` |
| **Statements.CountOfViolationsAsFirst** | agregasi | `Sum(DisjointChecks.ViolationUnit via First)` |
| **Statements.CountOfViolationsAsSecond** | agregasi | `Sum(DisjointChecks.ViolationUnit via Second)` |
| **Statements.IsInContradiction** | rumus | `Or(CountOfViolationsAsFirst > 0, CountOfViolationsAsSecond > 0)` |
| **Firings.RuleEnabled** | pencarian | `Lookup(Rules.IsEnabled via Rule)` |
| **Firings.KindWording** | pencarian | `Lookup(FiringKinds.Wording via Kind)` |
| **Firings.KindSortOrder** | pencarian | `Lookup(FiringKinds.SortOrder via Kind)` |
| **Firings.TargetName** | pencarian | `Lookup(Statements.Name via Target)` |
| **Firings.TargetSubjectLabel** | pencarian | `Lookup(Statements.SubjectLabel via Target)` |
| **Firings.TargetPropertyLabel** | pencarian | `Lookup(Statements.PropertyLabel via Target)` |
| **Firings.TargetObjectText** | pencarian | `Lookup(Statements.ObjectText via Target)` |
| **Firings.TargetIsConclusion** | pencarian | `Lookup(Statements.IsConclusion via Target)` |
| **Firings.TargetSubjectNodeKey** | pencarian | `Lookup(Statements.SubjectNodeKey via Target)` |
| **Firings.TargetObjectNodeKey** | pencarian | `Lookup(Statements.ObjectNodeKey via Target)` |
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
| **Firings.SecondIsInEffect** | pencarian | `Lookup(Statements.IsInEffect via Second)` |
| **Firings.FirstSnapPosition** | pencarian | `Lookup(Statements.SnapPosition via First)` |
| **Firings.SecondSnapPosition** | pencarian | `Lookup(Statements.SnapPosition via Second)` |
| **Firings.InverseFiredUnit** | rumus | `If(And(RuleEnabled, Kind = "inverse", FirstIsInEffect), 1, 0)` |
| **Firings.SymmetricFiredUnit** | rumus | `If(And(RuleEnabled, Kind = "symmetric", FirstIsInEffect), 1, 0)` |
| **Firings.ChainFiredUnit** | rumus | `If(And(RuleEnabled, Kind = "chain", FirstHoldsBeforeChain, SecondHoldsBeforeChain), 1, 0)` |
| **Firings.TypeFiredUnit** | rumus | `If(And(RuleEnabled, Or(Kind = "domain", Kind = "range"), FirstHoldsBeforeTyping), 1, 0)` |
| **Firings.FiredUnit** | rumus | `InverseFiredUnit + SymmetricFiredUnit + ChainFiredUnit + TypeFiredUnit` |
| **Firings.PreChainFiredUnit** | rumus | `InverseFiredUnit + SymmetricFiredUnit` |
| **Firings.PreChainOrderKey** | rumus | `FirstSortOrder * 10 + KindSortOrder` |
| **Firings.Round** | rumus | `If(And(Kind = "chain", Not(And(FirstIsInEffect, SecondIsInEffect))), 2, 1)` |
| **Firings.OrderKey** | rumus | `Round * 1000000000000000 + FirstSnapPosition * 100000000 + KindSortOrder * 10000000 + If(Second = "", 0, SecondSnapPosition)` |
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
| **Scenarios.Name** | rumus | `Label` |
| **Scenarios.GraphHasFactsToAdd** | pencarian | `Lookup(Graphs.HasFactsToAdd via Graph)` |
| **Scenarios.IsAvailable** | rumus | `Or(FactBaseline <> "next", GraphHasFactsToAdd)` |
| **ScenarioFactChanges.Name** | rumus | `ScenarioFactChangeId` |
| **ScenarioRuleSettings.Name** | rumus | `ScenarioRuleSettingId` |
| **NodeLayouts.Name** | rumus | `NodeLayoutId` |
| **LayoutSlots.Name** | rumus | `LayoutSlotId` |
| **PageTexts.Name** | rumus | `PageTextId` |
| **Engines.Name** | rumus | `Label` |
| **RulebookTools.Name** | rumus | `Label` |

---

> **RuleSpeak®** — notasi aturan bisnis deklaratif ciptaan **Ronald G. Ross** yang mengikuti konvensi **SBVR** — segera hadir. Sampai saat itu, halaman ini menampilkan dokumentasi biasa. [www.RonRoss.info](https://www.RonRoss.info)
