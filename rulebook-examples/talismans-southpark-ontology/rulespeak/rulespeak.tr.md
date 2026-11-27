# 📘 Talisman's South Park ontology explainer — Dokümantasyon

_Jessica Talisman's South Park Ontology Explainer, as a rulebook: the page's ten facts, five rule switches, property wiring, provenance wording and current-value policy are rows, and every conclusion is a formula. The rules apply in three fixed strata (inverse and symmetric, then the property chain, then domain and range); property wiring that feeds an earlier stratum would need repeated rounds. Every fact that could hold over the cast is a candidate row; app/lib/candidates.mjs writes the candidates when a thing, value or property is added._

> Kural kitabından üretilmiş bildirimsel iş kuralları. Aşağıdaki her ifade
> iş bağlamındaki bir doğruyu dile getirir; ne bir prosedürdür ne de bir buyruk.
> Kural kitabındaki formüller tek doğruluk kaynağıdır;
> bu belge onların yalın dildeki okunuşudur.

## 1 İş Terimleri Sözlüğü

| Terim | Açıklama | Açıklayıcı Not |
|------|-------------|-------------------|
| **Graph** | The explainer page as one graph: its wording variables and its totals. | — |
| Label | Tanımlı bir öznitelik. | _Display name of the graph._ |
| Literal Quote | Tanımlı bir öznitelik. | _The quote mark written around a literal value in a fact's wording._ |
| Literal Type Marker | Tanımlı bir öznitelik. | _The marker written between a literal value and its datatype, as in Turtle._ |
| Built in Source Text | Tanımlı bir öznitelik. | _Source line shown for a fact the page states as a teaching example._ |
| User Source Text | Tanımlı bir öznitelik. | _Source line shown for a fact the reader stated._ |
| No Conclusions Text | Tanımlı bir öznitelik. | _Message shown when the selected facts and rules add no conclusion._ |
| Name | “label” alanı ile aynı değeri taşır. | _Order 1. Display name of the graph._ |
| Count of Stated Facts | “graph” kaydıyla ilişkili “statements” kayıtları üzerinden hesaplanan toplam “in effect unit” değeri. | _Order 5. Facts that are switched on and in effect._ |
| Count of Conclusions | “graph” kaydıyla ilişkili “statements” kayıtları üzerinden hesaplanan toplam “conclusion unit” değeri. | _Order 18. Facts that hold only because an enabled rule concluded them._ |
| Count of Violations | “graph” kaydıyla ilişkili “disjoint checks” kayıtları üzerinden hesaplanan toplam “violation unit” değeri. | _Order 18. Disjointness checks that found both links between the same pair._ |
| Is Consistent | Doğru olma koşulu: “count of violations” alanı 0 değerine eşit. | _Order 19. True when no enabled disjointness rule is violated._ |
| Conclusions Message | Öncelik sırasına göre belirlenir: “count of conclusions” alanı 0 değerine eşit ise “no conclusions text” alanı; diğer tüm durumlarda boş metin. | _Order 19. The empty-state message for the conclusions panel, blank when there are conclusions._ |
| Count of Facts to Add | “graph” kaydıyla ilişkili “statements” kayıtları üzerinden hesaplanan toplam “listed off unit” değeri. | _Order 5. Listed facts that are not in effect; the Add next fact button is enabled while this is above zero._ |
| Has Facts to Add | Doğru olma koşulu: “count of facts to add” alanı 0 değerinden büyük. | _Order 6. True while some listed fact can still be added._ |
| Next Fact Sort Order | “graph” kaydıyla ilişkili, “listed off unit” olarak işaretli olan “statements” kayıtları üzerinden hesaplanan en küçük “sort order” değeri. | _Order 5. List position of the first listed fact that is not in effect._ |
| Max Activation Seq | “graph” kaydıyla ilişkili “statements” kayıtları üzerinden hesaplanan en büyük “activation seq” değeri. | _Order 1. Highest activation number used so far._ |
| Next Activation Seq | Hesaplanan değer: “max activation seq” alanı artı 1. | _Order 2. The activation number the next switched-on fact takes._ |
| Max Listed Sort Order | “graph” kaydıyla ilişkili, “listed unit” olarak işaretli olan “statements” kayıtları üzerinden hesaplanan en büyük “sort order” değeri. | _Order 2. Highest list position among listed facts._ |
| Next Listed Sort Order | Hesaplanan değer: “max listed sort order” alanı artı 1. | _Order 3. The list position a newly added fact takes._ |
| Max Sort Order | “graph” kaydıyla ilişkili “statements” kayıtları üzerinden hesaplanan en büyük “sort order” değeri. | _Order 1. Highest sort order of any fact, listed or candidate._ |
| Next Candidate Sort Order | Hesaplanan değer: “max sort order” alanı artı 1. | _Order 2. The sort order a new candidate fact, or a fact removed from the list, takes._ |

<details open>
<summary>Örnek veri</summary>

| Label | Literal Quote | Literal Type Marker | Built in Source Text | User Source Text | No Conclusions Text | Name ƒ | Count of Stated Facts ƒ | Count of Conclusions ƒ | Count of Violations ƒ | Is Consistent ƒ | Conclusions Message ƒ | Count of Facts to Add ƒ | Has Facts to Add ƒ | Next Fact Sort Order ƒ | Max Activation Seq ƒ | Next Activation Seq ƒ | Max Listed Sort Order ƒ | Next Listed Sort Order ƒ | Max Sort Order ƒ | Next Candidate Sort Order ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| South Park | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | These facts and selected rules add no further conclusions. Try another fact or rule. | South Park | 10 | 0 | 0 | true | These facts and selected rules add no further conclusions. Try another fact or rule. | 0 | false | 0 | 10 | 11 | 10 | 11 | 100269 | 100270 |

_ƒ, hesaplanan bir sütunu gösterir._

</details>

| Terim | Açıklama | Açıklayıcı Not |
|------|-------------|-------------------|
| **Rule** | The five rule switches on the page. | — |
| Label | Tanımlı bir öznitelik. | _The rule's name on the page._ |
| Explanation | Tanımlı bir öznitelik. | _The rule's one-line explanation on the page._ |
| Is Enabled | Doğru olma koşulu: boş metin. | _The rule's checkbox._ |
| Sort Order | Tanımlı bir öznitelik. | _Position in the rule list._ |
| Name | “label” alanı ile aynı değeri taşır. | _Order 1. The rule's name._ |
| Count of Firings | “rule” kaydıyla ilişkili “firings” kayıtları üzerinden hesaplanan toplam “fired unit” değeri. | _Order 15. Rule applications that fired, counting every derivation._ |

<details open>
<summary>Örnek veri</summary>

| Label | Explanation | Is Enabled | Sort Order | Name ƒ | Count of Firings ƒ |
|---|---|---|---|---|---|
| Inverse | A parent link also gives a child link between the same two people. | false | 1 | Inverse | 0 |
| Symmetric | A friend link or a spouse link is true for both people. | false | 2 | Symmetric | 0 |
| Property chain | A parent of a parent is a grandparent. | false | 3 | Property chain | 0 |

_ƒ, hesaplanan bir sütunu gösterir._

</details>

| Terim | Açıklama | Açıklayıcı Not |
|------|-------------|-------------------|
| **Firing Kind** | The ways a rule can apply. Domain and range belong to one rule but are worded apart; 'disjoint' names the rule the disjointness checks answer to. | — |
| Rule | Tanımlı bir öznitelik. | _The rule switch this kind belongs to._ |
| Wording | Tanımlı bir öznitelik. | _How a conclusion card names the rule, as in 'Worked out by the inverse rule'._ |
| Sort Order | Tanımlı bir öznitelik. | _Order in which the page's reasoner tries the kinds for one fact._ |
| Name | “wording” alanı ile aynı değeri taşır. | _Order 1. The kind's wording._ |
| Rule Enabled | “rule” alanı üzerinden bağlı kayıt “enabled” ise doğru. | _Order 1. Whether the owning rule is switched on._ |

<details open>
<summary>Örnek veri</summary>

| Wording | Sort Order | Name ƒ | Rule Enabled ƒ |
|---|---|---|---|
| inverse rule | 1 | inverse rule | false |
| symmetric rule | 2 | symmetric rule | false |
| property chain rule | 3 | property chain rule | false |

_ƒ, hesaplanan bir sütunu gösterir._

</details>

| Terim | Açıklama | Açıklayıcı Not |
|------|-------------|-------------------|
| **Thing** | Every individual and class the page can draw. | — |
| Label | Tanımlı bir öznitelik. | _The name the page shows._ |
| Kind | Tanımlı bir öznitelik. | _'individual' or 'class'._ |
| Sort Order | Tanımlı bir öznitelik. | _Position in the cast._ |
| Portrait | Tanımlı bir öznitelik. | _The portrait drawn for this thing; set when its name matches a portrait alias._ |
| Portrait Name | “portrait” alanı üzerinden bağlı kaydın “display name” alanı. | _Order 1. The name on the portrait's Fandom page._ |
| Portrait Page | “portrait” alanı üzerinden bağlı kayıttan alınır. | _Order 1. The Fandom page credited for the portrait._ |
| Portrait Image | “portrait” alanı üzerinden bağlı kayıttan alınır. | _Order 1. The original Fandom image._ |
| Portrait Crop | “portrait” alanı üzerinden bağlı kayıttan alınır. | _Order 1. The portrait's crop as an SVG viewBox._ |
| Portrait Width | “portrait” alanı üzerinden bağlı kayıttan alınır. | _Order 1. The original image's width in pixels._ |
| Portrait Height | “portrait” alanı üzerinden bağlı kayıttan alınır. | _Order 1. The original image's height in pixels._ |
| Name | “label” alanı ile aynı değeri taşır. | _Order 1. The thing's name._ |
| Node Key | “thing ID” alanı ile aynı değeri taşır. | _Order 1. The key the graph draws this thing under._ |
| Count of Children | “thing” kaydıyla ilişkili “statements” kayıtları üzerinden hesaplanan toplam “parent unit” değeri. | _Order 17. People this thing is a parent of, stated or concluded._ |
| Count of Grandchildren | “thing” kaydıyla ilişkili “statements” kayıtları üzerinden hesaplanan toplam “grandparent unit” değeri. | _Order 17. People this thing is a grandparent of._ |
| Count As Place | “thing” kaydıyla ilişkili “statements” kayıtları üzerinden hesaplanan toplam “place unit” değeri. | _Order 17. Holding attends facts whose object is this thing._ |
| Count As Subject | “thing” kaydıyla ilişkili “statements” kayıtları üzerinden hesaplanan toplam “holds unit” değeri. | _Order 17. Holding facts about this thing._ |
| Count As Object | “thing” kaydıyla ilişkili “statements” kayıtları üzerinden hesaplanan toplam “holds unit” değeri. | _Order 17. Holding facts that point at this thing._ |
| Is in Graph | Doğru olma koşulu: “count as subject” alanı 0 değerinden büyük veya “count as object” alanı 0 değerinden büyük. | _Order 18. True when the graph draws this thing._ |
| Node Kind | Öncelik sırasına göre belirlenir: “kind” alanı boş ise boş metin; “kind” alanı “class” değerine eşit ise “class”; “count as place” alanı 0 değerinden büyük ise “place”; diğer tüm durumlarda “person”. | _Order 18. How the graph draws the thing: a class, a place someone attends, or a person._ |

<details open>
<summary>Örnek veri</summary>

| Label | Kind | Sort Order | Portrait Name ƒ | Portrait Page ƒ | Portrait Image ƒ | Portrait Crop ƒ | Portrait Width ƒ | Portrait Height ƒ | Name ƒ | Node Key ƒ | Count of Children ƒ | Count of Grandchildren ƒ | Count As Place ƒ | Count As Subject ƒ | Count As Object ƒ | Is in Graph ƒ | Node Kind ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| Marvin | individual | 1 | Marvin Marsh | https://southpark.fandom.com/wiki/Marvin_Marsh | https://static.wikia.nocookie.net/southpark/images/d/db/Marsh-family-grandpa-marsh-w-purple-blanket.png/revision/latest | 48 -5 280 280 | 375 | 536 | Marvin | Marvin | 1 | 0 | 0 | 1 | 0 | true | person |
| Randy | individual | 2 | Randy Marsh | https://southpark.fandom.com/wiki/Randy_Marsh | https://static.wikia.nocookie.net/southparkfanon/images/d/d6/Randy_Marsh.png/revision/latest | 0 -10 428 428 | 428 | 1057 | Randy | Randy | 2 | 0 | 0 | 3 | 1 | true | person |
| Sharon | individual | 3 | Sharon Marsh | https://southpark.fandom.com/wiki/Sharon_Marsh | https://static.wikia.nocookie.net/southpark/images/9/99/Sharon_Marsh.png/revision/latest | 20 -10 390 390 | 439 | 1049 | Sharon | Sharon | 0 | 0 | 0 | 0 | 1 | true | person |

_ƒ, hesaplanan bir sütunu gösterir._

</details>

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
| **Portrait Aliase** | The names a portrait answers to. A new thing whose name matches an alias gets that portrait. | — |
| Portrait | Tanımlı bir öznitelik. | _The portrait this name selects._ |
| Name | “portrait alias ID” alanı ile aynı değeri taşır. | _Order 1. The alias._ |

<details open>
<summary>Örnek veri</summary>

| Name ƒ |
|---|
| marvin |
| marvin marsh |
| grandpa marsh |

_ƒ, hesaplanan bir sütunu gösterir._

</details>

| Terim | Açıklama | Açıklayıcı Not |
|------|-------------|-------------------|
| **Property** | The ten properties, with what the symmetric, domain and range rules read. Axioms that relate one property to another (inverse, chain, disjoint) are rows of PropertyAxioms. | — |
| Label | Tanımlı bir öznitelik. | _The property's wording on the page._ |
| Property Kind | Tanımlı bir öznitelik. | _'object' links two things, 'datatype' links a thing to a value, 'type' states class membership._ |
| Datatype | Tanımlı bir öznitelik. | _The value's XML Schema datatype for a datatype property; blank otherwise._ |
| Is User Addable | Doğru olma koşulu: boş metin. | _Offered in the page's Add fact form._ |
| Has One Current Value | Doğru olma koşulu: boş metin. | _The page keeps one current value per person: switching a value on replaces the earlier one._ |
| Is Symmetric | Doğru olma koşulu: boş metin. | _The symmetric rule makes this link true in both directions._ |
| Domain Class | Tanımlı bir öznitelik. | _The class the domain rule gives this property's subject._ |
| Range Class | Tanımlı bir öznitelik. | _The class the range rule gives this property's object._ |
| Sort Order | Tanımlı bir öznitelik. | _Position in the Add fact form's property list._ |
| Value Node Label | Tanımlı bir öznitelik. | _The small label under a value node for this property: 'role value' or 'number value'._ |
| Current Value Wording | Tanımlı bir öznitelik. | _How the Add fact form names a replaced value, as in 'Stan now has grade level 5'._ |
| Input Name | Tanımlı bir öznitelik. | _How the integer check names this property's value, as in 'for grade level'._ |
| Record Kind | Tanımlı bir öznitelik. | _The word the current-record summary uses for this property's values: role, grade or school._ |
| Record Order | Tanımlı bir öznitelik. | _Position of this property in the current-record summary._ |
| Name | “label” alanı ile aynı değeri taşır. | _Order 1. The property's wording._ |

<details open>
<summary>Örnek veri</summary>

| Label | Property Kind | Datatype | Is User Addable | Has One Current Value | Is Symmetric | Sort Order | Value Node Label | Current Value Wording | Input Name | Record Kind | Record Order | Name ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|
| is parent of | object | — | true | false | false | 1 | — | — | — | — | — | is parent of |
| is child of | object | — | true | false | false | 2 | — | — | — | — | — | is child of |
| is spouse of | object | — | true | false | true | 3 | — | — | — | — | — | is spouse of |

_ƒ, hesaplanan bir sütunu gösterir._

</details>

| Terim | Açıklama | Açıklayıcı Not |
|------|-------------|-------------------|
| **Property Axiom** | Axioms that relate one property to others, as OWL states them: an inverse, a two-link chain, or a disjoint pair. The candidate generator reads these rows. | — |
| Kind | Tanımlı bir öznitelik. | _inverse, chain or disjoint._ |
| Property | Tanımlı bir öznitelik. | _The property the axiom is about: the one inverted, the one a chain derives, or the first of a disjoint pair._ |
| First | Tanımlı bir öznitelik. | _The inverse property, the chain's first link, or the other property of a disjoint pair._ |
| Second | Tanımlı bir öznitelik. | _The chain's second link._ |
| Name | “property axiom ID” alanı ile aynı değeri taşır. | _Order 1. The axiom's key._ |

<details open>
<summary>Örnek veri</summary>

| Name ƒ |
|---|
| inverse:isParentOf |
| disjoint:isParentOf~isSpouseOf |
| inverse:isChildOf |

_ƒ, hesaplanan bir sütunu gösterir._

</details>

| Terim | Açıklama | Açıklayıcı Not |
|------|-------------|-------------------|
| **Statement** | Every fact the page can state or conclude about the cast. A listed fact appears in the page's fact list; the rest are candidates that only a rule can make hold. | — |
| Graph | Tanımlı bir öznitelik. | _The graph the fact belongs to._ |
| Subject | Tanımlı bir öznitelik. | _What the fact describes._ |
| Property | Tanımlı bir öznitelik. | _How the subject connects to the object._ |
| Object Thing | Tanımlı bir öznitelik. | _The related thing, for an object or type property._ |
| Object Value | Tanımlı bir öznitelik. | _The literal value, for a datatype property._ |
| Is Listed | Doğru olma koşulu: boş metin. | _Shown in the page's fact list._ |
| Is Built in | Doğru olma koşulu: boş metin. | _One of the page's teaching facts; the reader cannot remove it._ |
| Is Detail | Doğru olma koşulu: boş metin. | _One of Stan's detail facts that Show Stan's details switches on._ |
| Is Stated | Doğru olma koşulu: boş metin. | _The fact's checkbox._ |
| Activation Seq | Tanımlı bir öznitelik. | _When the fact was last switched on, as a running number; decides which value is current._ |
| Sort Order | Tanımlı bir öznitelik. | _Position in the fact list; candidates follow the listed facts._ |
| Subject Label | “subject” alanı üzerinden bağlı kayıttan alınır. | _Order 1. The subject's name._ |
| Property Label | “property” alanı üzerinden bağlı kayıttan alınır. | _Order 1. The property's wording._ |
| Object Label | “object thing” alanı üzerinden bağlı kayıttan alınır. | _Order 1. The object's name, for an object or type property._ |
| Property Datatype | “property” alanı üzerinden bağlı kayıttan alınır. | _Order 1. The value's datatype, for a datatype property._ |
| Property Has One Current Value | “property” alanı üzerinden bağlı kayıt “one current value” sahibi ise doğru. | _Order 1. Whether the property keeps one current value per person._ |
| Property Domain Class | “property” alanı üzerinden bağlı kayıttan alınır. | _Order 1. The property's domain class._ |
| Property Range Class | “property” alanı üzerinden bağlı kayıttan alınır. | _Order 1. The property's range class._ |
| Literal Quote | “graph” alanı üzerinden bağlı kayıttan alınır. | _Order 1. The graph's literal quote mark._ |
| Literal Type Marker | “graph” alanı üzerinden bağlı kayıttan alınır. | _Order 1. The graph's datatype marker._ |
| Built in Source Text | “graph” alanı üzerinden bağlı kayıttan alınır. | _Order 1. The graph's source line for a teaching fact._ |
| User Source Text | “graph” alanı üzerinden bağlı kayıttan alınır. | _Order 1. The graph's source line for a reader's fact._ |
| Next Fact Sort Order | “graph” alanı üzerinden bağlı kayıttan alınır. | _Order 6. List position of the next fact to add._ |
| Is Literal | Doğru olma koşulu: “object thing” alanı boş. | _Order 1. True when the object is a value rather than a thing._ |
| Object Text | Öncelik sırasına göre belirlenir: “object thing” alanı boş ise “object value” alanı; diğer tüm durumlarda “object label” alanı. | _Order 2. The object's name or the value._ |
| Name | Hesaplanan değer: “subject label” alanı, ardından bir boşluk, ardından “property label” alanı, ardından bir boşluk, ardından “object text” alanı. | _Order 3. The fact in words._ |
| Words | Hesaplanan değer: “subject label” alanı, ardından bir boşluk, ardından “property label” alanı, ardından bir boşluk, ardından “object thing” alanı boş ise “literal quote” alanı, ardından “object value” alanı, ardından “literal quote” alanı, ardından “literal type marker” alanı, ardından “property datatype” alanı, diğer tüm durumlarda “object label” alanı. ⚠︎ mekanik <!-- rulespeak:reword --> | _Order 2. The fact as a conclusion card quotes it, with a literal written in Turtle form._ |
| Source | Öncelik sırasına göre belirlenir: “built in” bayrağı etkin ise “built in source text” alanı; diğer tüm durumlarda “user source text” alanı. | _Order 2. Where a listed fact came from._ |
| Subject Property Key | Hesaplanan değer: “subject” alanı, ardından “|”, ardından “property” alanı. | _Order 1. Groups the values one subject has for one property._ |
| Latest Sibling Activation Seq | “statement” kaydıyla ilişkili “statements” kayıtları üzerinden hesaplanan en büyük “activation seq” değeri. | _Order 2. The most recent activation among this subject's values for this property._ |
| Is in Effect | Doğru olma koşulu: “stated” bayrağı etkin ve (“property has one current value” bayrağı etkin değil veya “activation seq” alanı “latest sibling activation seq” alanı değerine eşit). | _Order 3. Switched on and, for a one-value property, the most recently switched-on value. Drawn as a solid line._ |
| Listed Unit | Öncelik sırasına göre belirlenir: “listed” bayrağı etkin ise 1; diğer tüm durumlarda 0. | _Order 1. 1 when the fact is in the fact list._ |
| In Effect Unit | Öncelik sırasına göre belirlenir: “in effect” bayrağı etkin ise 1; diğer tüm durumlarda 0. | _Order 4. 1 when the fact is in effect._ |
| Listed Off Unit | Öncelik sırasına göre belirlenir: “listed” bayrağı etkin ve “in effect” bayrağı etkin değil ise 1; diğer tüm durumlarda 0. | _Order 4. 1 when the fact is listed but not in effect._ |
| Is Next to Add | Doğru olma koşulu: “listed off unit” alanı 1 değerine eşit ve “sort order” alanı “next fact sort order” alanı değerine eşit. | _Order 7. The fact the Add next fact button switches on._ |
| Count of Inverse Firings | “statement” kaydıyla ilişkili “firings” kayıtları üzerinden hesaplanan toplam “inverse fired unit” değeri. | _Order 6. Inverse rule applications that conclude this fact._ |
| Count of Symmetric Firings | “statement” kaydıyla ilişkili “firings” kayıtları üzerinden hesaplanan toplam “symmetric fired unit” değeri. | _Order 6. Symmetric rule applications that conclude this fact._ |
| Holds Before Chain | Doğru olma koşulu: “IF” işlevinin “OR” işlevinin “is in effect” alanı, “count of inverse firings” alanı 0 değerinden büyük ve “count of symmetric firings” alanı 0 değerinden büyük için verdiği sonuç ve doğru için verdiği sonuç. | _Order 7. TRUE when the fact is in effect or the inverse or symmetric rule concludes it. Blank when nothing states or concludes it: not known, never FALSE (open world: removing a statement does not say that its opposite is true)._ |
| Count of Chain Firings | “statement” kaydıyla ilişkili “firings” kayıtları üzerinden hesaplanan toplam “chain fired unit” değeri. | _Order 10. Property chain applications that conclude this fact._ |
| Holds Before Typing | Doğru olma koşulu: “IF” işlevinin “OR” işlevinin “holds before chain” alanı ve “count of chain firings” alanı 0 değerinden büyük için verdiği sonuç ve doğru için verdiği sonuç. | _Order 11. TRUE when the fact holds before the domain and range rules apply. Blank when nothing states or concludes it: not known, never FALSE (open world: removing a statement does not say that its opposite is true)._ |
| Count of Type Firings | “statement” kaydıyla ilişkili “firings” kayıtları üzerinden hesaplanan toplam “type fired unit” değeri. | _Order 14. Domain and range applications that conclude this fact._ |
| Holds | Doğru olma koşulu: “IF” işlevinin “OR” işlevinin “holds before typing” alanı ve “count of type firings” alanı 0 değerinden büyük için verdiği sonuç ve doğru için verdiği sonuç. | _Order 15. TRUE when in effect or concluded by an enabled rule; the graph draws it. Blank when nothing states or concludes it: not known, never FALSE (open world: removing a statement does not say that its opposite is true)._ |
| Is Conclusion | Doğru olma koşulu: “IF” işlevinin “AND” işlevinin “holds” alanı ve “in effect” bayrağı etkin değil için verdiği sonuç ve doğru için verdiği sonuç. | _Order 16. TRUE when the fact holds only because a rule concluded it; drawn dashed. Blank otherwise: a stated fact is not a conclusion, and a fact nothing concludes is not known (never FALSE)._ |
| Holds Unit | Öncelik sırasına göre belirlenir: “holds” bayrağı etkin ise 1; diğer tüm durumlarda 0. | _Order 16. 1 when the fact holds._ |
| Conclusion Unit | Öncelik sırasına göre belirlenir: “conclusion” bayrağı etkin ise 1; diğer tüm durumlarda 0. | _Order 17. 1 when the fact is a conclusion._ |
| Parent Unit | Öncelik sırasına göre belirlenir: “holds” bayrağı etkin ve “property” alanı “isParentOf” değerine eşit ise 1; diğer tüm durumlarda 0. | _Order 16. 1 for a holding parent link._ |
| Grandparent Unit | Öncelik sırasına göre belirlenir: “holds” bayrağı etkin ve “property” alanı “isGrandparentOf” değerine eşit ise 1; diğer tüm durumlarda 0. | _Order 16. 1 for a holding grandparent link._ |
| Place Unit | Öncelik sırasına göre belirlenir: “holds” bayrağı etkin ve “property” alanı “attends” değerine eşit ise 1; diğer tüm durumlarda 0. | _Order 16. 1 for a holding attends link._ |
| First Pre Chain Order Key | “statement” kaydıyla ilişkili, “pre chain fired unit” olarak işaretli olan “firings” kayıtları üzerinden hesaplanan en küçük “pre chain order key” değeri. | _Order 7. Order key of the first inverse or symmetric application that concludes this fact._ |
| Snap Position | Öncelik sırasına göre belirlenir: “in effect” bayrağı etkin ise “sort order” alanı; “holds before chain” bayrağı etkin ise 100000 artı “first pre chain order key” alanı; diğer tüm durumlarda 0. | _Order 8. Where the page's reasoner meets this fact as a premise: a stated fact at its list position, a fact concluded by the inverse or symmetric rule after every stated fact, in the order it was concluded; 0 for a fact that does not hold yet, so a link to no fact reads the same on every engine._ |
| First Fired Order Key | “statement” kaydıyla ilişkili, “fired unit” olarak işaretli olan “firings” kayıtları üzerinden hesaplanan en küçük “order key” değeri. | _Order 15. Order key of the first rule application that concludes this fact, in the page reasoner's order._ |
| Count of Chosen Firings | “statement” kaydıyla ilişkili “firings” kayıtları üzerinden hesaplanan toplam “chosen unit” değeri. | _Order 20. Rule applications shown as this fact's conclusion card: one for a conclusion, zero otherwise._ |
| Subject Node Key | “subject” alanı ile aynı değeri taşır. | _Order 1. The graph node this fact starts from._ |
| Object Node Key | Öncelik sırasına göre belirlenir: “object thing” alanı boş ise “literal|”, ardından “object value” alanı, ardından “|”, ardından “property datatype” alanı; diğer tüm durumlarda “object thing” alanı. | _Order 2. The graph node this fact points at: the thing, or one node per value and datatype._ |
| Subject Node Kind | “subject” alanı üzerinden bağlı kayıttan alınır. | _Order 19. How the graph draws the subject._ |
| Object Thing Node Kind | “object thing” alanı üzerinden bağlı kayıttan alınır. | _Order 19. How the graph draws the object thing._ |
| Object Node Kind | Öncelik sırasına göre belirlenir: “object thing” alanı boş ise “value”; diğer tüm durumlarda “object thing node kind” alanı. | _Order 20. How the graph draws the object: a value, a class, a place or a person._ |
| Object Value Node Label | “property” alanı üzerinden bağlı kayıttan alınır. | _Order 1. The small label under a value node._ |
| Count of Violations As First | “statement” kaydıyla ilişkili “disjoint checks” kayıtları üzerinden hesaplanan toplam “violation unit” değeri. | _Order 18. Violated disjointness checks naming this fact first._ |
| Count of Violations As Second | “statement” kaydıyla ilişkili “disjoint checks” kayıtları üzerinden hesaplanan toplam “violation unit” değeri. | _Order 18. Violated disjointness checks naming this fact second._ |
| Is in Contradiction | Doğru olma koşulu: “count of violations as first” alanı 0 değerinden büyük veya “count of violations as second” alanı 0 değerinden büyük. | _Order 19. Part of a contradiction the disjoint rule found; drawn in the alarm colour._ |

<details open>
<summary>Örnek veri</summary>

| Object Value | Is Listed | Is Built in | Is Detail | Is Stated | Activation Seq | Sort Order | Subject Label ƒ | Property Label ƒ | Object Label ƒ | Property Datatype ƒ | Property Has One Current Value ƒ | Property Domain Class ƒ | Property Range Class ƒ | Literal Quote ƒ | Literal Type Marker ƒ | Built in Source Text ƒ | User Source Text ƒ | Next Fact Sort Order ƒ | Is Literal ƒ | Object Text ƒ | Name ƒ | Words ƒ | Source ƒ | Subject Property Key ƒ | Latest Sibling Activation Seq ƒ | Is in Effect ƒ | Listed Unit ƒ | In Effect Unit ƒ | Listed Off Unit ƒ | Is Next to Add ƒ | Count of Inverse Firings ƒ | Count of Symmetric Firings ƒ | Holds Before Chain ƒ | Count of Chain Firings ƒ | Holds Before Typing ƒ | Count of Type Firings ƒ | Holds ƒ | Is Conclusion ƒ | Holds Unit ƒ | Conclusion Unit ƒ | Parent Unit ƒ | Grandparent Unit ƒ | Place Unit ƒ | First Pre Chain Order Key ƒ | Snap Position ƒ | First Fired Order Key ƒ | Count of Chosen Firings ƒ | Subject Node Key ƒ | Object Node Key ƒ | Subject Node Kind ƒ | Object Thing Node Kind ƒ | Object Node Kind ƒ | Object Value Node Label ƒ | Count of Violations As First ƒ | Count of Violations As Second ƒ | Is in Contradiction ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| — | true | true | false | true | 1 | 1 | Marvin | is parent of | Randy | — | false | — | — | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | 0 | false | Randy | Marvin is parent of Randy | Marvin is parent of Randy | Stated as a teaching example on this page. | Marvin\|isParentOf | 1 | true | 1 | 1 | 0 | false | 0 | 0 | true | 0 | true | 0 | true | — | 1 | 0 | 1 | 0 | 0 | 0.0 | 1.0 | 0.0 | 0 | Marvin | Randy | person | person | person | — | 0 | 0 | false |
| — | false | false | false | false | 0 | 100001 | Marvin | is parent of | Sharon | — | false | — | — | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | 0 | false | Sharon | Marvin is parent of Sharon | Marvin is parent of Sharon | Stated by you on this page. | Marvin\|isParentOf | 1 | false | 0 | 0 | 0 | false | 0 | 0 | — | 0 | — | 0 | — | — | 0 | 0 | 0 | 0 | 0 | 0.0 | 0.0 | 0.0 | 0 | Marvin | Sharon | person | person | person | — | 0 | 0 | false |
| — | false | false | false | false | 0 | 100002 | Marvin | is parent of | Stan | — | false | — | — | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | 0 | false | Stan | Marvin is parent of Stan | Marvin is parent of Stan | Stated by you on this page. | Marvin\|isParentOf | 1 | false | 0 | 0 | 0 | false | 0 | 0 | — | 0 | — | 0 | — | — | 0 | 0 | 0 | 0 | 0 | 0.0 | 0.0 | 0.0 | 0 | Marvin | Stan | person | person | person | — | 0 | 0 | false |

_ƒ, hesaplanan bir sütunu gösterir._

</details>

| Terim | Açıklama | Açıklayıcı Not |
|------|-------------|-------------------|
| **Firing** | Every way an enabled rule could conclude a candidate fact from the facts that hold. | — |
| Kind | Tanımlı bir öznitelik. | _How the rule applies._ |
| Rule | Tanımlı bir öznitelik. | _The rule switch._ |
| Target | Tanımlı bir öznitelik. | _The fact this application concludes._ |
| First | Tanımlı bir öznitelik. | _The premise._ |
| Second | Tanımlı bir öznitelik. | _The second premise, for the property chain._ |
| Rule Enabled | “rule” alanı üzerinden bağlı kayıt “enabled” ise doğru. | _Order 1. Whether the rule is switched on._ |
| Kind Wording | “kind” alanı üzerinden bağlı kayıttan alınır. | _Order 1. How the card names the rule._ |
| Kind Sort Order | “kind” alanı üzerinden bağlı kayıttan alınır. | _Order 1. The kind's place in the reasoner's order._ |
| Target Name | “target” alanı üzerinden bağlı kayıttan alınır. | _Order 4. The concluded fact in words._ |
| Target Subject Label | “target” alanı üzerinden bağlı kayıttan alınır. | _Order 2. The concluded fact's subject._ |
| Target Property Label | “target” alanı üzerinden bağlı kayıttan alınır. | _Order 2. The concluded fact's property._ |
| Target Object Text | “target” alanı üzerinden bağlı kayıttan alınır. | _Order 3. The concluded fact's object._ |
| Target is Conclusion | “target” alanı üzerinden bağlı kayıt “conclusion” ise doğru. | _Order 17. Whether the concluded fact is new._ |
| Target Subject Node Key | “target” alanı üzerinden bağlı kayıttan alınır. | _Order 2. The concluded fact's subject node._ |
| Target Object Node Key | “target” alanı üzerinden bağlı kayıttan alınır. | _Order 3. The concluded fact's object node._ |
| Target First Fired Order Key | “target” alanı üzerinden bağlı kayıttan alınır. | _Order 16. The order key of the concluded fact's first application._ |
| First Words | “first” alanı üzerinden bağlı kayıttan alınır. | _Order 3. The premise as the card quotes it._ |
| Second Words | “second” alanı üzerinden bağlı kayıttan alınır. | _Order 3. The second premise as the card quotes it._ |
| First is in Effect | “first” alanı üzerinden bağlı kayıt “in effect” ise doğru. | _Order 4. Whether the premise is in effect._ |
| First Holds Before Chain | “first” alanı üzerinden bağlı kayıt “holds before chain” olarak işaretli ise doğru. | _Order 8. Whether the premise holds before the chain._ |
| Second Holds Before Chain | “second” alanı üzerinden bağlı kayıt “holds before chain” olarak işaretli ise doğru. | _Order 8. Whether the second premise holds before the chain._ |
| First Holds Before Typing | “first” alanı üzerinden bağlı kayıt “holds before typing” olarak işaretli ise doğru. | _Order 12. Whether the premise holds before typing._ |
| First Property | “first” alanı üzerinden bağlı kayıttan alınır. | _Order 1. The premise's property key, as the card's axiom names it._ |
| First Property Domain Class | “first” alanı üzerinden bağlı kayıttan alınır. | _Order 2. The premise property's domain class._ |
| First Property Range Class | “first” alanı üzerinden bağlı kayıttan alınır. | _Order 2. The premise property's range class._ |
| First Sort Order | “first” alanı üzerinden bağlı kayıttan alınır. | _Order 1. The premise's list position._ |
| Second is in Effect | “second” alanı üzerinden bağlı kayıt “in effect” ise doğru. | _Order 4. Whether the second premise is in effect._ |
| First Snap Position | “first” alanı üzerinden bağlı kayıttan alınır. | _Order 9. Where the reasoner meets the premise._ |
| Second Snap Position | “second” alanı üzerinden bağlı kayıttan alınır. | _Order 9. Where the reasoner meets the second premise._ |
| Inverse Fired Unit | Öncelik sırasına göre belirlenir: “rule enabled” bayrağı etkin, “kind” alanı “inverse” değerine eşit ve “first is in effect” bayrağı etkin ise 1; diğer tüm durumlarda 0. | _Order 5. 1 when the inverse rule fires here._ |
| Symmetric Fired Unit | Öncelik sırasına göre belirlenir: “rule enabled” bayrağı etkin, “kind” alanı “symmetric” değerine eşit ve “first is in effect” bayrağı etkin ise 1; diğer tüm durumlarda 0. | _Order 5. 1 when the symmetric rule fires here._ |
| Chain Fired Unit | Öncelik sırasına göre belirlenir: “rule enabled” bayrağı etkin, “kind” alanı “chain” değerine eşit, “first holds before chain” bayrağı etkin ve “second holds before chain” bayrağı etkin ise 1; diğer tüm durumlarda 0. | _Order 9. 1 when the property chain fires here._ |
| Type Fired Unit | Öncelik sırasına göre belirlenir: “rule enabled” bayrağı etkin, (“kind” alanı “domain” değerine eşit veya “kind” alanı “range” değerine eşit) ve “first holds before typing” bayrağı etkin ise 1; diğer tüm durumlarda 0. | _Order 13. 1 when the domain or range rule fires here._ |
| Fired Unit | Hesaplanan değer: “inverse fired unit” alanı artı “symmetric fired unit” alanı artı “chain fired unit” alanı artı “type fired unit” alanı. | _Order 14. 1 when this application fires._ |
| Pre Chain Fired Unit | Hesaplanan değer: “inverse fired unit” alanı artı “symmetric fired unit” alanı. | _Order 6. 1 when this is an inverse or symmetric application that fires._ |
| Pre Chain Order Key | Hesaplanan değer: “first sort order” alanı çarpı 10 artı “kind sort order” alanı. | _Order 2. For an inverse or symmetric application: its place in the reasoner's first round._ |
| Round | Öncelik sırasına göre belirlenir: “kind” alanı “chain” değerine eşit ve (“first is in effect” bayrağı etkin ve “second is in effect” bayrağı etkin) koşulu sağlanmıyor ise 2; diğer tüm durumlarda 1. | _Order 5. The reasoner's round: 1 when every premise is a stated fact, 2 when the property chain reads a concluded fact._ |
| Order Key | Hesaplanan değer: “round” alanı çarpı 1000000000000000 artı “first snap position” alanı çarpı 100000000 artı “kind sort order” alanı çarpı 10000000 artı “second” alanı boş ise 0, diğer tüm durumlarda “second snap position” alanı. | _Order 10. The page reasoner's order: by round, then premise, then rule kind, then second premise._ |
| Is Chosen | Doğru olma koşulu: “fired unit” alanı 1 değerine eşit, “target is conclusion” bayrağı etkin ve “order key” alanı “target first fired order key” alanı değerine eşit. | _Order 18. The application the conclusions panel shows for its fact: the first one in the reasoner's order._ |
| Chosen Unit | Öncelik sırasına göre belirlenir: “chosen” bayrağı etkin ise 1; diğer tüm durumlarda 0. | _Order 19. 1 when this application is shown._ |
| Card Text | Öncelik sırasına göre belirlenir: “chosen” bayrağı etkin ise “Worked out by the ”, ardından “kind wording” alanı, ardından “ from “”, ardından “first words” alanı, ardından “””, ardından “kind” alanı “chain” değerine eşit ise “ and “”, ardından “second words” alanı, ardından “””, diğer tüm durumlarda “kind” alanı “domain” değerine eşit ise “ and “”, ardından “first property” alanı, ardından “ has domain ”, ardından “first property domain class” alanı, ardından “””, diğer tüm durumlarda “ and “”, ardından “first property” alanı, ardından “ has range ”, ardından “first property range class” alanı, ardından “””, ardından bir nokta; diğer tüm durumlarda boş metin. ⚠︎ mekanik <!-- rulespeak:reword --> | _Order 19. The conclusion card's provenance line, in the page's wording._ |
| Name | “firing ID” alanı ile aynı değeri taşır. | _Order 1. The application's key._ |

<details open>
<summary>Örnek veri</summary>

| Rule Enabled ƒ | Kind Wording ƒ | Kind Sort Order ƒ | Target Name ƒ | Target Subject Label ƒ | Target Property Label ƒ | Target Object Text ƒ | Target is Conclusion ƒ | Target Subject Node Key ƒ | Target Object Node Key ƒ | Target First Fired Order Key ƒ | First Words ƒ | Second Words ƒ | First is in Effect ƒ | First Holds Before Chain ƒ | Second Holds Before Chain ƒ | First Holds Before Typing ƒ | First Property ƒ | First Property Domain Class ƒ | First Property Range Class ƒ | First Sort Order ƒ | Second is in Effect ƒ | First Snap Position ƒ | Second Snap Position ƒ | Inverse Fired Unit ƒ | Symmetric Fired Unit ƒ | Chain Fired Unit ƒ | Type Fired Unit ƒ | Fired Unit ƒ | Pre Chain Fired Unit ƒ | Pre Chain Order Key ƒ | Round ƒ | Order Key ƒ | Is Chosen ƒ | Chosen Unit ƒ | Card Text ƒ | Name ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| false | inverse rule | 1 | Randy is child of Marvin | Randy | is child of | Marvin | — | Randy | Marvin | 0.0 | Marvin is parent of Randy |    | true | true | — | true | isParentOf | — | — | 1 | false | 1.0 | 0.0 | 0 | 0 | 0 | 0 | 0 | 0 | 11.0 | 1 | 1000000110000000.0 | false | 0 | — | inverse:Marvin\|isParentOf\|Randy>Randy\|isChildOf\|Marvin |
| false | inverse rule | 1 | Sharon is child of Marvin | Sharon | is child of | Marvin | — | Sharon | Marvin | 0.0 | Marvin is parent of Sharon |    | false | — | — | — | isParentOf | — | — | 100001 | false | 0.0 | 0.0 | 0 | 0 | 0 | 0 | 0 | 0 | 1000011.0 | 1 | 1000000010000000.0 | false | 0 | — | inverse:Marvin\|isParentOf\|Sharon>Sharon\|isChildOf\|Marvin |
| false | inverse rule | 1 | Stan is child of Marvin | Stan | is child of | Marvin | — | Stan | Marvin | 0.0 | Marvin is parent of Stan |    | false | — | — | — | isParentOf | — | — | 100002 | false | 0.0 | 0.0 | 0 | 0 | 0 | 0 | 0 | 0 | 1000021.0 | 1 | 1000000010000000.0 | false | 0 | — | inverse:Marvin\|isParentOf\|Stan>Stan\|isChildOf\|Marvin |

_ƒ, hesaplanan bir sütunu gösterir._

</details>

| Terim | Açıklama | Açıklayıcı Not |
|------|-------------|-------------------|
| **Disjoint Check** | Every pair the disjointness rule watches: the same two people linked by two disjoint properties. | — |
| Graph | Tanımlı bir öznitelik. | _The graph._ |
| Rule | Tanımlı bir öznitelik. | _The rule switch._ |
| First | Tanımlı bir öznitelik. | _The first link._ |
| Second | Tanımlı bir öznitelik. | _The disjoint link between the same pair, in the same direction._ |
| Rule Enabled | “rule” alanı üzerinden bağlı kayıt “enabled” ise doğru. | _Order 1. Whether the disjointness rule is on._ |
| First Holds | “first” alanı üzerinden bağlı kayıt “holds” olarak işaretli ise doğru. | _Order 16. Whether the first link holds._ |
| Second Holds | “second” alanı üzerinden bağlı kayıt “holds” olarak işaretli ise doğru. | _Order 16. Whether the second link holds._ |
| Subject Label | “first” alanı üzerinden bağlı kayıttan alınır. | _Order 2. The person recorded with both links._ |
| Object Label | “first” alanı üzerinden bağlı kayıttan alınır. | _Order 2. The other person._ |
| Violation Unit | Öncelik sırasına göre belirlenir: “rule enabled” bayrağı etkin, “first holds” bayrağı etkin ve “second holds” bayrağı etkin ise 1; diğer tüm durumlarda 0. | _Order 17. 1 when both links hold._ |
| Alarm Text | Öncelik sırasına göre belirlenir: “violation unit” alanı 1 değerine eşit ise “Contradiction found. ”, ardından “subject label” alanı, ardından “ is recorded as both parent and spouse of ”, ardından “object label” alanı, ardından “. Check those facts and the selected disjoint rule.”; diğer tüm durumlarda boş metin. | _Order 18. The page's contradiction alert, blank when there is none._ |
| Name | “disjoint check ID” alanı ile aynı değeri taşır. | _Order 1. The check's key._ |

<details open>
<summary>Örnek veri</summary>

| Rule Enabled ƒ | First Holds ƒ | Second Holds ƒ | Subject Label ƒ | Object Label ƒ | Violation Unit ƒ | Alarm Text ƒ | Name ƒ |
|---|---|---|---|---|---|---|---|
| false | true | — | Marvin | Randy | 0 | — | Marvin\|isParentOf\|Randy~isSpouseOf |
| false | — | — | Marvin | Sharon | 0 | — | Marvin\|isParentOf\|Sharon~isSpouseOf |
| false | — | — | Marvin | Stan | 0 | — | Marvin\|isParentOf\|Stan~isSpouseOf |

_ƒ, hesaplanan bir sütunu gösterir._

</details>

| Terim | Açıklama | Açıklayıcı Not |
|------|-------------|-------------------|
| **Scenario** | The page's buttons. Each one is a fact baseline, explicit fact changes and rule settings, applied in that order. | — |
| Graph | Tanımlı bir öznitelik. | _The graph._ |
| Label | Tanımlı bir öznitelik. | _The button's text._ |
| Note | Tanımlı bir öznitelik. | _The note shown under the buttons after this one is pressed; blank clears it._ |
| Sort Order | Tanımlı bir öznitelik. | _Position among the buttons._ |
| Button Group | Tanımlı bir öznitelik. | _'stan' for the two imagined changes, 'facts' for the buttons under the fact list._ |
| Is Primary | Doğru olma koşulu: boş metin. | _Drawn as the filled button._ |
| Fact Baseline | Tanımlı bir öznitelik. | _What happens to every listed fact first: keep, built-in (teaching facts on, others off), built-in-detail, all-off, activate-all (each switched on in list order), or next (the next fact on)._ |
| Layout | Tanımlı bir öznitelik. | _The node layout the graph returns to; blank keeps the current positions._ |
| Clears Selection | Doğru olma koşulu: boş metin. | _Clears the selected node or line._ |
| Clears Form Message | Doğru olma koşulu: boş metin. | _Clears the Add fact form's message._ |
| Name | “label” alanı ile aynı değeri taşır. | _Order 1. The button's text._ |
| Graph Has Facts to Add | “graph” alanı üzerinden bağlı kayıt “facts to add” sahibi ise doğru. | _Order 7. Whether a listed fact can still be added._ |
| Is Available | Doğru olma koşulu: “fact baseline” alanı “next” değerine eşit değil veya “graph has facts to add” bayrağı etkin. | _Order 8. False only for Add next fact once every listed fact is in effect._ |

<details open>
<summary>Örnek veri</summary>

| Label | Note | Sort Order | Button Group | Is Primary | Fact Baseline | Layout | Clears Selection | Clears Form Message | Name ƒ | Graph Has Facts to Add ƒ | Is Available ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|
| Move Stan to grade 5 | This imagined change updates Stan’s grade to 5. His identifier, Person statement and family links stay the same. His other selected details are unchanged. | 1 | stan | false | keep | — | true | true | Move Stan to grade 5 | false | true |
| Stan leaves school | This imagined change clears Stan’s current school details. His identifier, Person statement and family links remain. Removing a current fact does not say that its opposite is true; school history needs dates. | 2 | stan | false | keep | — | true | true | Stan leaves school | false | true |
| Add next fact | — | 3 | facts | true | next | — | false | false | Add next fact | false | false |

_ƒ, hesaplanan bir sütunu gösterir._

</details>

| Terim | Açıklama | Açıklayıcı Not |
|------|-------------|-------------------|
| **Scenario Fact Change** | Fact changes a button makes after its baseline. A blank object key matches every listed value of the property. | — |
| Scenario | Tanımlı bir öznitelik. | _The button._ |
| Subject | Tanımlı bir öznitelik. | _The subject whose facts change._ |
| Property | Tanımlı bir öznitelik. | _The property whose facts change._ |
| Object Key | Tanımlı bir öznitelik. | _The object thing's key or the literal value; blank for every listed value._ |
| Action | Tanımlı bir öznitelik. | _'on' switches the fact on as the newest value; 'off' switches it off._ |
| Adds to List | Doğru olma koşulu: boş metin. | _Adds the fact to the fact list first when it is not listed._ |
| Sort Order | Tanımlı bir öznitelik. | _Order of the change within the button._ |
| Name | “scenario fact change ID” alanı ile aynı değeri taşır. | _Order 1. The change's key._ |

<details open>
<summary>Örnek veri</summary>

| Object Key | Action | Adds to List | Sort Order | Name ƒ |
|---|---|---|---|---|
| Person | on | true | 1 | stan-grade-five:Stan\|isA\|Person |
| 5 | on | true | 2 | stan-grade-five:Stan\|hasGradeLevel\|5 |
| Person | on | true | 3 | stan-leaves-school:Stan\|isA\|Person |

_ƒ, hesaplanan bir sütunu gösterir._

</details>

| Terim | Açıklama | Açıklayıcı Not |
|------|-------------|-------------------|
| **Scenario Rule Setting** | Rule switches a button sets. | — |
| Scenario | Tanımlı bir öznitelik. | _The button._ |
| Rule | Tanımlı bir öznitelik. | _The rule switch._ |
| Is Enabled | Doğru olma koşulu: boş metin. | _The state the button puts the switch in._ |
| Name | “scenario rule setting ID” alanı ile aynı değeri taşır. | _Order 1. The setting's key._ |

<details open>
<summary>Örnek veri</summary>

| Is Enabled | Name ƒ |
|---|---|
| false | reset-original:inverse |
| false | reset-original:symmetric |
| false | reset-original:chain |

_ƒ, hesaplanan bir sütunu gösterir._

</details>

| Terim | Açıklama | Açıklayıcı Not |
|------|-------------|-------------------|
| **Node Layout** | Node positions in the graph's 380 by 480 drawing, one set per layout. | — |
| Layout | Tanımlı bir öznitelik. | _The layout: 'default' or 'stan-details'._ |
| Node Key | Tanımlı bir öznitelik. | _The node: a thing's key, or literal\|value\|datatype._ |
| X | Tanımlı bir öznitelik. | _Horizontal position._ |
| Y | Tanımlı bir öznitelik. | _Vertical position._ |
| Name | “node layout ID” alanı ile aynı değeri taşır. | _Order 1. The position's key._ |

<details open>
<summary>Örnek veri</summary>

| Layout | Node Key | X | Y | Name ƒ |
|---|---|---|---|---|
| default | Marvin | 70 | 45 | default:Marvin |
| default | Randy | 195 | 125 | default:Randy |
| default | Sharon | 318 | 55 | default:Sharon |

_ƒ, hesaplanan bir sütunu gösterir._

</details>

| Terim | Açıklama | Açıklayıcı Not |
|------|-------------|-------------------|
| **Layout Slot** | Places for nodes no layout positions, used in turn. | — |
| Sort Order | Tanımlı bir öznitelik. | _Order in which slots are used._ |
| X | Tanımlı bir öznitelik. | _Horizontal position._ |
| Y | Tanımlı bir öznitelik. | _Vertical position._ |
| Name | “layout slot ID” alanı ile aynı değeri taşır. | _Order 1. The slot's key._ |

<details open>
<summary>Örnek veri</summary>

| Sort Order | X | Y | Name ƒ |
|---|---|---|---|
| 1 | 330 | 135 | slot-1 |
| 2 | 45 | 120 | slot-2 |
| 3 | 330 | 400 | slot-3 |

_ƒ, hesaplanan bir sütunu gösterir._

</details>

| Terim | Açıklama | Açıklayıcı Not |
|------|-------------|-------------------|
| **Page Text** | The page's wording, one row per string. {name}-style placeholders are filled by the page. | — |
| Text | Tanımlı bir öznitelik. | _The string._ |
| Name | “page text ID” alanı ile aynı değeri taşır. | _Order 1. The string's key._ |

<details open>
<summary>Örnek veri</summary>

| Text | Name ƒ |
|---|---|
| South Park Ontology Explainer, on one rulebook | page-title |
| A rulebook rebuild of the explainer by | byline-prefix |
| Jessica Talisman | byline-author |

_ƒ, hesaplanan bir sütunu gösterir._

</details>

| Terim | Açıklama | Açıklayıcı Not |
|------|-------------|-------------------|
| **Engine** | The execution engines the page can be computed by. Each is a build output of this rulebook. | — |
| Label | Tanımlı bir öznitelik. | _The engine's name in the switcher._ |
| Substrate | Tanımlı bir öznitelik. | _The conformance harness's name for this substrate._ |
| Is Default | Doğru olma koşulu: boş metin. | _The engine a new visitor starts on._ |
| Description | Tanımlı bir öznitelik. | _What computes the page on this engine._ |
| Sort Order | Tanımlı bir öznitelik. | _Position in the switcher._ |
| Name | “label” alanı ile aynı değeri taşır. | _Order 1. The engine's name._ |

<details open>
<summary>Örnek veri</summary>

| Label | Substrate | Is Default | Description | Sort Order | Name ƒ |
|---|---|---|---|---|---|
| Postgres views | effortless-postgres | true | Every value is a column of a generated view; the database computes it when the page reads it. | 1 | Postgres views |
| TypeScript | effortless-typescript | false | The generated TypeScript computes every field from the same rows, with no database. | 2 | TypeScript |
| OWL and SHACL rules | effortless-owl | false | The generated ontology and SHACL rules, run over the same rows by a SPARQL engine. | 3 | OWL and SHACL rules |

_ƒ, hesaplanan bir sütunu gösterir._

</details>

| Terim | Açıklama | Açıklayıcı Not |
|------|-------------|-------------------|
| **Rulebook Tool** | What each rulebook tool adds to the original explainer; the tools panel lists them. | — |
| Label | Tanımlı bir öznitelik. | _The tool's name._ |
| Adds | Tanımlı bir öznitelik. | _What it adds to the original explainer._ |
| Href | Tanımlı bir öznitelik. | _Where its link goes; 'editor' means the running rulebook editor._ |
| Link Text | Tanımlı bir öznitelik. | _The link's text._ |
| Sort Order | Tanımlı bir öznitelik. | _Position in the panel._ |
| Name | “label” alanı ile aynı değeri taşır. | _Order 1. The tool's name._ |

<details open>
<summary>Örnek veri</summary>

| Label | Adds | Href | Link Text | Sort Order | Name ƒ |
|---|---|---|---|---|---|
| Interchangeable engines | The same page, computed by Postgres views, generated TypeScript or the generated OWL and SHACL rules. Switch with the control above; nothing else changes. | — | — | 1 | Interchangeable engines |
| Conformance | Every engine's answers for the current facts, compared value by value with the conformance harness's own rule for when two values are equal. | — | — | 2 | Conformance |
| Explainer | Hover a dotted value for its rule in plain words and its witness: the values it came from, down to the stated facts. | /rulebook-explainer-dag/pages/index.html | Open the explainer | 3 | Explainer |

_ƒ, hesaplanan bir sütunu gösterir._

</details>

## 2 Olgu Türleri

- Her **firing kind** kaydı tam olarak bir **rule** kaydına referans verir
- Her **thing** kaydı bir **portrait** kaydına referans verebilir
- Her **portrait aliase** kaydı tam olarak bir **portrait** kaydına referans verir
- Her **property** kaydı bir **thing** kaydına referans verebilir
- Her **property axiom** kaydı tam olarak bir **firing kind** kaydına referans verir
- Her **property axiom** kaydı tam olarak bir **property** kaydına referans verir
- Her **statement** kaydı tam olarak bir **graph** kaydına referans verir
- Her **statement** kaydı tam olarak bir **thing** kaydına referans verir
- Her **statement** kaydı tam olarak bir **property** kaydına referans verir
- Her **firing** kaydı tam olarak bir **firing kind** kaydına referans verir
- Her **firing** kaydı tam olarak bir **rule** kaydına referans verir
- Her **firing** kaydı tam olarak bir **statement** kaydına referans verir
- Her **disjoint check** kaydı tam olarak bir **graph** kaydına referans verir
- Her **disjoint check** kaydı tam olarak bir **rule** kaydına referans verir
- Her **disjoint check** kaydı tam olarak bir **statement** kaydına referans verir
- Her **scenario** kaydı tam olarak bir **graph** kaydına referans verir
- Her **scenario fact change** kaydı tam olarak bir **scenario** kaydına referans verir
- Her **scenario fact change** kaydı tam olarak bir **thing** kaydına referans verir
- Her **scenario fact change** kaydı tam olarak bir **property** kaydına referans verir
- Her **scenario rule setting** kaydı tam olarak bir **scenario** kaydına referans verir
- Her **scenario rule setting** kaydı tam olarak bir **rule** kaydına referans verir

## 3 İşletimsel Kurallar

_İşletimsel kurallar, işletmenin neyi **zorunlu kıldığını**, neyi **yasakladığını** ve neyi
tavsiye ettiğini (**önerilir**) belirtir. Yapısal kurallar zorunlu alanlardan ve yabancı anahtarlardan türetilir;
anlamsal kurallar Constraints tablosundan gelir ve her biri kural kitabının zaten hesapladığı
bir mantıksal değere dayanır (aşağıdaki Tanımsal Kurallar bölümünde DR-N olarak çapraz başvurulur)._

### Yapısal Kısıtlar (şemadan)

- “graph” kaydı, “label” alanı, “literal quote” alanı, “literal type marker” alanı, “built in source text” alanı, “user source text” alanı ve “no conclusions text” alanı dolu olmak **zorundadır**.
- “rule” kaydı, “label” alanı, “explanation” alanı ve “sort order” alanı dolu olmak, ayrıca “enabled” olup olmadığı bilgisini kaydetmek **zorundadır**.
- “firing kind” kaydı, tam olarak bir “rule” kaydına referans vermek **zorundadır**.
- “firing kind” kaydı, “wording” alanı ve “sort order” alanı dolu olmak **zorundadır**.
- “thing” kaydı, “label” alanı, “kind” alanı ve “sort order” alanı dolu olmak **zorundadır**.
- “portrait” kaydı, “display name” alanı, “page” alanı, “image” alanı, “crop” alanı, “width” alanı, “height” alanı ve “sort order” alanı dolu olmak **zorundadır**.
- “portrait aliase” kaydı, tam olarak bir “portrait” kaydına referans vermek **zorundadır**.
- “property” kaydı, “label” alanı, “property kind” alanı ve “sort order” alanı dolu olmak, ayrıca “user addable” olup olmadığı bilgisini, “one current value” sahibi olup olmadığı bilgisini ve “symmetric” olup olmadığı bilgisini kaydetmek **zorundadır**.
- “property axiom” kaydı, “kind” alanı aracılığıyla tam olarak bir “firing kind” kaydına referans vermek **zorundadır**.
- “property axiom” kaydı, tam olarak bir “property” kaydına referans vermek **zorundadır**.
- “property axiom” kaydı, “first” alanı aracılığıyla tam olarak bir “property” kaydına referans vermek **zorundadır**.
- “statement” kaydı, tam olarak bir “graph” kaydına referans vermek **zorundadır**.
- “statement” kaydı, “subject” alanı aracılığıyla tam olarak bir “thing” kaydına referans vermek **zorundadır**.
- “statement” kaydı, tam olarak bir “property” kaydına referans vermek **zorundadır**.
- “statement” kaydı, “activation seq” alanı ve “sort order” alanı dolu olmak, ayrıca “listed” olup olmadığı bilgisini, “built in” olup olmadığı bilgisini, “detail” olup olmadığı bilgisini ve “stated” olup olmadığı bilgisini kaydetmek **zorundadır**.
- “firing” kaydı, “kind” alanı aracılığıyla tam olarak bir “firing kind” kaydına referans vermek **zorundadır**.
- “firing” kaydı, tam olarak bir “rule” kaydına referans vermek **zorundadır**.
- “firing” kaydı, “target” alanı aracılığıyla tam olarak bir “statement” kaydına referans vermek **zorundadır**.
- “firing” kaydı, “first” alanı aracılığıyla tam olarak bir “statement” kaydına referans vermek **zorundadır**.
- “disjoint check” kaydı, tam olarak bir “graph” kaydına referans vermek **zorundadır**.
- “disjoint check” kaydı, tam olarak bir “rule” kaydına referans vermek **zorundadır**.
- “disjoint check” kaydı, “first” alanı aracılığıyla tam olarak bir “statement” kaydına referans vermek **zorundadır**.
- “disjoint check” kaydı, “second” alanı aracılığıyla tam olarak bir “statement” kaydına referans vermek **zorundadır**.
- “scenario” kaydı, tam olarak bir “graph” kaydına referans vermek **zorundadır**.
- “scenario” kaydı, “label” alanı, “sort order” alanı, “button group” alanı ve “fact baseline” alanı dolu olmak, ayrıca “primary” olup olmadığı bilgisini, “clears selection” olarak işaretli olup olmadığı bilgisini ve “clears form message” olarak işaretli olup olmadığı bilgisini kaydetmek **zorundadır**.
- “scenario fact change” kaydı, tam olarak bir “scenario” kaydına referans vermek **zorundadır**.
- “scenario fact change” kaydı, “subject” alanı aracılığıyla tam olarak bir “thing” kaydına referans vermek **zorundadır**.
- “scenario fact change” kaydı, tam olarak bir “property” kaydına referans vermek **zorundadır**.
- “scenario fact change” kaydı, “action” alanı ve “sort order” alanı dolu olmak, ayrıca “adds to list” olarak işaretli olup olmadığı bilgisini kaydetmek **zorundadır**.
- “scenario rule setting” kaydı, tam olarak bir “scenario” kaydına referans vermek **zorundadır**.
- “scenario rule setting” kaydı, tam olarak bir “rule” kaydına referans vermek **zorundadır**.
- “scenario rule setting” kaydı, “enabled” olup olmadığı bilgisini kaydetmek **zorundadır**.
- “node layout” kaydı, “layout” alanı, “node key” alanı, “x” alanı ve “y” alanı dolu olmak **zorundadır**.
- “layout slot” kaydı, “sort order” alanı, “x” alanı ve “y” alanı dolu olmak **zorundadır**.
- “page text” kaydı, “text” alanı dolu olmak **zorundadır**.
- “engine” kaydı, “label” alanı, “substrate” alanı, “description” alanı ve “sort order” alanı dolu olmak, ayrıca “default” olup olmadığı bilgisini kaydetmek **zorundadır**.
- “rulebook tool” kaydı, “label” alanı, “adds” alanı ve “sort order” alanı dolu olmak **zorundadır**.

## 4 Tanımsal Kurallar

_Tüm ifadeler iş bağlamındaki bir doğruyu dile getirir; ne prosedürdür ne de buyruk.
Tek yönlü bir gereklilik denklik sanılmasın diye “ancak ve ancak” yerine
“yalnızca … ise” kullanılır. **⚠︎ mekanik** etiketi, belirlenimci ifadesi aslına sadık
ama hantal olan bir kuralı gösterir — isteğe bağlı bir yeniden yazım turu için
bir işarettir, kusur değildir._

| Kural No | Bildirimsel kural |
|----|------------------|
| **DR-1 Name** | “graph” kaydının “name” alanı, “label” alanı ile aynı değeri taşır. |
| **DR-2 Count of Stated Facts** | “graph” kaydının “count of stated facts” alanı, “graph” kaydıyla ilişkili “statements” kayıtları üzerinden hesaplanan toplam “in effect unit” değeri olarak belirlenir. |
| **DR-3 Count of Conclusions** | “graph” kaydının “count of conclusions” alanı, “graph” kaydıyla ilişkili “statements” kayıtları üzerinden hesaplanan toplam “conclusion unit” değeri olarak belirlenir. |
| **DR-4 Count of Violations** | “graph” kaydının “count of violations” alanı, “graph” kaydıyla ilişkili “disjoint checks” kayıtları üzerinden hesaplanan toplam “violation unit” değeri olarak belirlenir. |
| **DR-5 Is Consistent** | “graph” kaydı, “count of violations” alanı 0 değerine eşit ise “consistent” sayılır. |
| **DR-6 Conclusions Message** | “graph” kaydının “conclusions message” alanı, aşağıdaki öncelik sırasına göre belirlenir:<br>1. “count of conclusions” alanı 0 değerine eşit ise “no conclusions text” alanı;<br>2. diğer tüm durumlarda boş metin. |
| **DR-7 Count of Facts to Add** | “graph” kaydının “count of facts to add” alanı, “graph” kaydıyla ilişkili “statements” kayıtları üzerinden hesaplanan toplam “listed off unit” değeri olarak belirlenir. |
| **DR-8 Has Facts to Add** | “graph” kaydı, “count of facts to add” alanı 0 değerinden büyük ise “facts to add” sahibi sayılır. |
| **DR-9 Next Fact Sort Order** | “graph” kaydının “next fact sort order” alanı, “graph” kaydıyla ilişkili, “listed off unit” olarak işaretli olan “statements” kayıtları üzerinden hesaplanan en küçük “sort order” değeri olarak belirlenir. |
| **DR-10 Max Activation Seq** | “graph” kaydının “max activation seq” alanı, “graph” kaydıyla ilişkili “statements” kayıtları üzerinden hesaplanan en büyük “activation seq” değeri olarak belirlenir. |
| **DR-11 Next Activation Seq** | “graph” kaydının “next activation seq” alanı, “max activation seq” alanı artı 1 olarak hesaplanır. |
| **DR-12 Max Listed Sort Order** | “graph” kaydının “max listed sort order” alanı, “graph” kaydıyla ilişkili, “listed unit” olarak işaretli olan “statements” kayıtları üzerinden hesaplanan en büyük “sort order” değeri olarak belirlenir. |
| **DR-13 Next Listed Sort Order** | “graph” kaydının “next listed sort order” alanı, “max listed sort order” alanı artı 1 olarak hesaplanır. |
| **DR-14 Max Sort Order** | “graph” kaydının “max sort order” alanı, “graph” kaydıyla ilişkili “statements” kayıtları üzerinden hesaplanan en büyük “sort order” değeri olarak belirlenir. |
| **DR-15 Next Candidate Sort Order** | “graph” kaydının “next candidate sort order” alanı, “max sort order” alanı artı 1 olarak hesaplanır. |
| **DR-16 Name** | “rule” kaydının “name” alanı, “label” alanı ile aynı değeri taşır. |
| **DR-17 Count of Firings** | “rule” kaydının “count of firings” alanı, “rule” kaydıyla ilişkili “firings” kayıtları üzerinden hesaplanan toplam “fired unit” değeri olarak belirlenir. |
| **DR-18 Name** | “firing kind” kaydının “name” alanı, “wording” alanı ile aynı değeri taşır. |
| **DR-19 Rule Enabled** | “firing kind” kaydının “rule enabled” alanı, “rule” alanı üzerinden bağlı kayıt “enabled” ise doğrudur. |
| **DR-20 Portrait Name** | “thing” kaydının “portrait name” alanı, “portrait” alanı üzerinden bağlı kaydın “display name” alanından alınır. |
| **DR-21 Portrait Page** | “thing” kaydının “portrait page” alanı, “portrait” alanı üzerinden bağlı kayıttan alınır. |
| **DR-22 Portrait Image** | “thing” kaydının “portrait image” alanı, “portrait” alanı üzerinden bağlı kayıttan alınır. |
| **DR-23 Portrait Crop** | “thing” kaydının “portrait crop” alanı, “portrait” alanı üzerinden bağlı kayıttan alınır. |
| **DR-24 Portrait Width** | “thing” kaydının “portrait width” alanı, “portrait” alanı üzerinden bağlı kayıttan alınır. |
| **DR-25 Portrait Height** | “thing” kaydının “portrait height” alanı, “portrait” alanı üzerinden bağlı kayıttan alınır. |
| **DR-26 Name** | “thing” kaydının “name” alanı, “label” alanı ile aynı değeri taşır. |
| **DR-27 Node Key** | “thing” kaydının “node key” alanı, “thing ID” alanı ile aynı değeri taşır. |
| **DR-28 Count of Children** | “thing” kaydının “count of children” alanı, “thing” kaydıyla ilişkili “statements” kayıtları üzerinden hesaplanan toplam “parent unit” değeri olarak belirlenir. |
| **DR-29 Count of Grandchildren** | “thing” kaydının “count of grandchildren” alanı, “thing” kaydıyla ilişkili “statements” kayıtları üzerinden hesaplanan toplam “grandparent unit” değeri olarak belirlenir. |
| **DR-30 Count As Place** | “thing” kaydının “count as place” alanı, “thing” kaydıyla ilişkili “statements” kayıtları üzerinden hesaplanan toplam “place unit” değeri olarak belirlenir. |
| **DR-31 Count As Subject** | “thing” kaydının “count as subject” alanı, “thing” kaydıyla ilişkili “statements” kayıtları üzerinden hesaplanan toplam “holds unit” değeri olarak belirlenir. |
| **DR-32 Count As Object** | “thing” kaydının “count as object” alanı, “thing” kaydıyla ilişkili “statements” kayıtları üzerinden hesaplanan toplam “holds unit” değeri olarak belirlenir. |
| **DR-33 Is in Graph** | “thing” kaydı, “count as subject” alanı 0 değerinden büyük veya “count as object” alanı 0 değerinden büyük ise “in graph” sayılır. |
| **DR-34 Node Kind** | “thing” kaydının “node kind” alanı, aşağıdaki öncelik sırasına göre belirlenir:<br>1. “kind” alanı boş ise boş metin;<br>2. “kind” alanı “class” değerine eşit ise “class”;<br>3. “count as place” alanı 0 değerinden büyük ise “place”;<br>4. diğer tüm durumlarda “person”. |
| **DR-35 Name** | “portrait” kaydının “name” alanı, “display name” alanı ile aynı değeri taşır. |
| **DR-36 Name** | “portrait aliase” kaydının “name” alanı, “portrait alias ID” alanı ile aynı değeri taşır. |
| **DR-37 Name** | “property” kaydının “name” alanı, “label” alanı ile aynı değeri taşır. |
| **DR-38 Name** | “property axiom” kaydının “name” alanı, “property axiom ID” alanı ile aynı değeri taşır. |
| **DR-39 Subject Label** | “statement” kaydının “subject label” alanı, “subject” alanı üzerinden bağlı kayıttan alınır. |
| **DR-40 Property Label** | “statement” kaydının “property label” alanı, “property” alanı üzerinden bağlı kayıttan alınır. |
| **DR-41 Object Label** | “statement” kaydının “object label” alanı, “object thing” alanı üzerinden bağlı kayıttan alınır. |
| **DR-42 Property Datatype** | “statement” kaydının “property datatype” alanı, “property” alanı üzerinden bağlı kayıttan alınır. |
| **DR-43 Property Has One Current Value** | “statement” kaydının “property has one current value” alanı, “property” alanı üzerinden bağlı kayıt “one current value” sahibi ise doğrudur. |
| **DR-44 Property Domain Class** | “statement” kaydının “property domain class” alanı, “property” alanı üzerinden bağlı kayıttan alınır. |
| **DR-45 Property Range Class** | “statement” kaydının “property range class” alanı, “property” alanı üzerinden bağlı kayıttan alınır. |
| **DR-46 Literal Quote** | “statement” kaydının “literal quote” alanı, “graph” alanı üzerinden bağlı kayıttan alınır. |
| **DR-47 Literal Type Marker** | “statement” kaydının “literal type marker” alanı, “graph” alanı üzerinden bağlı kayıttan alınır. |
| **DR-48 Built in Source Text** | “statement” kaydının “built in source text” alanı, “graph” alanı üzerinden bağlı kayıttan alınır. |
| **DR-49 User Source Text** | “statement” kaydının “user source text” alanı, “graph” alanı üzerinden bağlı kayıttan alınır. |
| **DR-50 Next Fact Sort Order** | “statement” kaydının “next fact sort order” alanı, “graph” alanı üzerinden bağlı kayıttan alınır. |
| **DR-51 Is Literal** | “statement” kaydı, “object thing” alanı boş ise “literal” sayılır. |
| **DR-52 Object Text** | “statement” kaydının “object text” alanı, aşağıdaki öncelik sırasına göre belirlenir:<br>1. “object thing” alanı boş ise “object value” alanı;<br>2. diğer tüm durumlarda “object label” alanı. |
| **DR-53 Name** | “statement” kaydının “name” alanı, “subject label” alanı, ardından bir boşluk, ardından “property label” alanı, ardından bir boşluk, ardından “object text” alanı olarak hesaplanır. |
| **DR-54 Words** | “statement” kaydının “words” alanı, “subject label” alanı, ardından bir boşluk, ardından “property label” alanı, ardından bir boşluk, ardından “object thing” alanı boş ise “literal quote” alanı, ardından “object value” alanı, ardından “literal quote” alanı, ardından “literal type marker” alanı, ardından “property datatype” alanı, diğer tüm durumlarda “object label” alanı olarak hesaplanır. ⚠︎ mekanik <!-- rulespeak:reword --> |
| **DR-55 Source** | “statement” kaydının “source” alanı, aşağıdaki öncelik sırasına göre belirlenir:<br>1. “built in” bayrağı etkin ise “built in source text” alanı;<br>2. diğer tüm durumlarda “user source text” alanı. |
| **DR-56 Subject Property Key** | “statement” kaydının “subject property key” alanı, “subject” alanı, ardından “|”, ardından “property” alanı olarak hesaplanır. |
| **DR-57 Latest Sibling Activation Seq** | “statement” kaydının “latest sibling activation seq” alanı, “statement” kaydıyla ilişkili “statements” kayıtları üzerinden hesaplanan en büyük “activation seq” değeri olarak belirlenir. |
| **DR-58 Is in Effect** | “statement” kaydı, “stated” bayrağı etkin ve (“property has one current value” bayrağı etkin değil veya “activation seq” alanı “latest sibling activation seq” alanı değerine eşit) ise “in effect” sayılır. |
| **DR-59 Listed Unit** | “statement” kaydının “listed unit” alanı, aşağıdaki öncelik sırasına göre belirlenir:<br>1. “listed” bayrağı etkin ise 1;<br>2. diğer tüm durumlarda 0. |
| **DR-60 In Effect Unit** | “statement” kaydının “in effect unit” alanı, aşağıdaki öncelik sırasına göre belirlenir:<br>1. “in effect” bayrağı etkin ise 1;<br>2. diğer tüm durumlarda 0. |
| **DR-61 Listed Off Unit** | “statement” kaydının “listed off unit” alanı, aşağıdaki öncelik sırasına göre belirlenir:<br>1. “listed” bayrağı etkin ve “in effect” bayrağı etkin değil ise 1;<br>2. diğer tüm durumlarda 0. |
| **DR-62 Is Next to Add** | “statement” kaydı, “listed off unit” alanı 1 değerine eşit ve “sort order” alanı “next fact sort order” alanı değerine eşit ise “next to add” sayılır. |
| **DR-63 Count of Inverse Firings** | “statement” kaydının “count of inverse firings” alanı, “statement” kaydıyla ilişkili “firings” kayıtları üzerinden hesaplanan toplam “inverse fired unit” değeri olarak belirlenir. |
| **DR-64 Count of Symmetric Firings** | “statement” kaydının “count of symmetric firings” alanı, “statement” kaydıyla ilişkili “firings” kayıtları üzerinden hesaplanan toplam “symmetric fired unit” değeri olarak belirlenir. |
| **DR-65 Holds Before Chain** | “statement” kaydı, “IF” işlevinin “OR” işlevinin “is in effect” alanı, “count of inverse firings” alanı 0 değerinden büyük ve “count of symmetric firings” alanı 0 değerinden büyük için verdiği sonuç ve doğru için verdiği sonuç ise “holds before chain” koşulunu sağlamış sayılır. |
| **DR-66 Count of Chain Firings** | “statement” kaydının “count of chain firings” alanı, “statement” kaydıyla ilişkili “firings” kayıtları üzerinden hesaplanan toplam “chain fired unit” değeri olarak belirlenir. |
| **DR-67 Holds Before Typing** | “statement” kaydı, “IF” işlevinin “OR” işlevinin “holds before chain” alanı ve “count of chain firings” alanı 0 değerinden büyük için verdiği sonuç ve doğru için verdiği sonuç ise “holds before typing” koşulunu sağlamış sayılır. |
| **DR-68 Count of Type Firings** | “statement” kaydının “count of type firings” alanı, “statement” kaydıyla ilişkili “firings” kayıtları üzerinden hesaplanan toplam “type fired unit” değeri olarak belirlenir. |
| **DR-69 Holds** | “statement” kaydı, “IF” işlevinin “OR” işlevinin “holds before typing” alanı ve “count of type firings” alanı 0 değerinden büyük için verdiği sonuç ve doğru için verdiği sonuç ise “holds” koşulunu sağlamış sayılır. |
| **DR-70 Is Conclusion** | “statement” kaydı, “IF” işlevinin “AND” işlevinin “holds” alanı ve “in effect” bayrağı etkin değil için verdiği sonuç ve doğru için verdiği sonuç ise “conclusion” sayılır. |
| **DR-71 Holds Unit** | “statement” kaydının “holds unit” alanı, aşağıdaki öncelik sırasına göre belirlenir:<br>1. “holds” bayrağı etkin ise 1;<br>2. diğer tüm durumlarda 0. |
| **DR-72 Conclusion Unit** | “statement” kaydının “conclusion unit” alanı, aşağıdaki öncelik sırasına göre belirlenir:<br>1. “conclusion” bayrağı etkin ise 1;<br>2. diğer tüm durumlarda 0. |
| **DR-73 Parent Unit** | “statement” kaydının “parent unit” alanı, aşağıdaki öncelik sırasına göre belirlenir:<br>1. “holds” bayrağı etkin ve “property” alanı “isParentOf” değerine eşit ise 1;<br>2. diğer tüm durumlarda 0. |
| **DR-74 Grandparent Unit** | “statement” kaydının “grandparent unit” alanı, aşağıdaki öncelik sırasına göre belirlenir:<br>1. “holds” bayrağı etkin ve “property” alanı “isGrandparentOf” değerine eşit ise 1;<br>2. diğer tüm durumlarda 0. |
| **DR-75 Place Unit** | “statement” kaydının “place unit” alanı, aşağıdaki öncelik sırasına göre belirlenir:<br>1. “holds” bayrağı etkin ve “property” alanı “attends” değerine eşit ise 1;<br>2. diğer tüm durumlarda 0. |
| **DR-76 First Pre Chain Order Key** | “statement” kaydının “first pre chain order key” alanı, “statement” kaydıyla ilişkili, “pre chain fired unit” olarak işaretli olan “firings” kayıtları üzerinden hesaplanan en küçük “pre chain order key” değeri olarak belirlenir. |
| **DR-77 Snap Position** | “statement” kaydının “snap position” alanı, aşağıdaki öncelik sırasına göre belirlenir:<br>1. “in effect” bayrağı etkin ise “sort order” alanı;<br>2. “holds before chain” bayrağı etkin ise 100000 artı “first pre chain order key” alanı;<br>3. diğer tüm durumlarda 0. |
| **DR-78 First Fired Order Key** | “statement” kaydının “first fired order key” alanı, “statement” kaydıyla ilişkili, “fired unit” olarak işaretli olan “firings” kayıtları üzerinden hesaplanan en küçük “order key” değeri olarak belirlenir. |
| **DR-79 Count of Chosen Firings** | “statement” kaydının “count of chosen firings” alanı, “statement” kaydıyla ilişkili “firings” kayıtları üzerinden hesaplanan toplam “chosen unit” değeri olarak belirlenir. |
| **DR-80 Subject Node Key** | “statement” kaydının “subject node key” alanı, “subject” alanı ile aynı değeri taşır. |
| **DR-81 Object Node Key** | “statement” kaydının “object node key” alanı, aşağıdaki öncelik sırasına göre belirlenir:<br>1. “object thing” alanı boş ise “literal|”, ardından “object value” alanı, ardından “|”, ardından “property datatype” alanı;<br>2. diğer tüm durumlarda “object thing” alanı. |
| **DR-82 Subject Node Kind** | “statement” kaydının “subject node kind” alanı, “subject” alanı üzerinden bağlı kayıttan alınır. |
| **DR-83 Object Thing Node Kind** | “statement” kaydının “object thing node kind” alanı, “object thing” alanı üzerinden bağlı kayıttan alınır. |
| **DR-84 Object Node Kind** | “statement” kaydının “object node kind” alanı, aşağıdaki öncelik sırasına göre belirlenir:<br>1. “object thing” alanı boş ise “value”;<br>2. diğer tüm durumlarda “object thing node kind” alanı. |
| **DR-85 Object Value Node Label** | “statement” kaydının “object value node label” alanı, “property” alanı üzerinden bağlı kayıttan alınır. |
| **DR-86 Count of Violations As First** | “statement” kaydının “count of violations as first” alanı, “statement” kaydıyla ilişkili “disjoint checks” kayıtları üzerinden hesaplanan toplam “violation unit” değeri olarak belirlenir. |
| **DR-87 Count of Violations As Second** | “statement” kaydının “count of violations as second” alanı, “statement” kaydıyla ilişkili “disjoint checks” kayıtları üzerinden hesaplanan toplam “violation unit” değeri olarak belirlenir. |
| **DR-88 Is in Contradiction** | “statement” kaydı, “count of violations as first” alanı 0 değerinden büyük veya “count of violations as second” alanı 0 değerinden büyük ise “in contradiction” sayılır. |
| **DR-89 Rule Enabled** | “firing” kaydının “rule enabled” alanı, “rule” alanı üzerinden bağlı kayıt “enabled” ise doğrudur. |
| **DR-90 Kind Wording** | “firing” kaydının “kind wording” alanı, “kind” alanı üzerinden bağlı kayıttan alınır. |
| **DR-91 Kind Sort Order** | “firing” kaydının “kind sort order” alanı, “kind” alanı üzerinden bağlı kayıttan alınır. |
| **DR-92 Target Name** | “firing” kaydının “target name” alanı, “target” alanı üzerinden bağlı kayıttan alınır. |
| **DR-93 Target Subject Label** | “firing” kaydının “target subject label” alanı, “target” alanı üzerinden bağlı kayıttan alınır. |
| **DR-94 Target Property Label** | “firing” kaydının “target property label” alanı, “target” alanı üzerinden bağlı kayıttan alınır. |
| **DR-95 Target Object Text** | “firing” kaydının “target object text” alanı, “target” alanı üzerinden bağlı kayıttan alınır. |
| **DR-96 Target is Conclusion** | “firing” kaydının “target is conclusion” alanı, “target” alanı üzerinden bağlı kayıt “conclusion” ise doğrudur. |
| **DR-97 Target Subject Node Key** | “firing” kaydının “target subject node key” alanı, “target” alanı üzerinden bağlı kayıttan alınır. |
| **DR-98 Target Object Node Key** | “firing” kaydının “target object node key” alanı, “target” alanı üzerinden bağlı kayıttan alınır. |
| **DR-99 Target First Fired Order Key** | “firing” kaydının “target first fired order key” alanı, “target” alanı üzerinden bağlı kayıttan alınır. |
| **DR-100 First Words** | “firing” kaydının “first words” alanı, “first” alanı üzerinden bağlı kayıttan alınır. |
| **DR-101 Second Words** | “firing” kaydının “second words” alanı, “second” alanı üzerinden bağlı kayıttan alınır. |
| **DR-102 First is in Effect** | “firing” kaydının “first is in effect” alanı, “first” alanı üzerinden bağlı kayıt “in effect” ise doğrudur. |
| **DR-103 First Holds Before Chain** | “firing” kaydının “first holds before chain” alanı, “first” alanı üzerinden bağlı kayıt “holds before chain” olarak işaretli ise doğrudur. |
| **DR-104 Second Holds Before Chain** | “firing” kaydının “second holds before chain” alanı, “second” alanı üzerinden bağlı kayıt “holds before chain” olarak işaretli ise doğrudur. |
| **DR-105 First Holds Before Typing** | “firing” kaydının “first holds before typing” alanı, “first” alanı üzerinden bağlı kayıt “holds before typing” olarak işaretli ise doğrudur. |
| **DR-106 First Property** | “firing” kaydının “first property” alanı, “first” alanı üzerinden bağlı kayıttan alınır. |
| **DR-107 First Property Domain Class** | “firing” kaydının “first property domain class” alanı, “first” alanı üzerinden bağlı kayıttan alınır. |
| **DR-108 First Property Range Class** | “firing” kaydının “first property range class” alanı, “first” alanı üzerinden bağlı kayıttan alınır. |
| **DR-109 First Sort Order** | “firing” kaydının “first sort order” alanı, “first” alanı üzerinden bağlı kayıttan alınır. |
| **DR-110 Second is in Effect** | “firing” kaydının “second is in effect” alanı, “second” alanı üzerinden bağlı kayıt “in effect” ise doğrudur. |
| **DR-111 First Snap Position** | “firing” kaydının “first snap position” alanı, “first” alanı üzerinden bağlı kayıttan alınır. |
| **DR-112 Second Snap Position** | “firing” kaydının “second snap position” alanı, “second” alanı üzerinden bağlı kayıttan alınır. |
| **DR-113 Inverse Fired Unit** | “firing” kaydının “inverse fired unit” alanı, aşağıdaki öncelik sırasına göre belirlenir:<br>1. “rule enabled” bayrağı etkin, “kind” alanı “inverse” değerine eşit ve “first is in effect” bayrağı etkin ise 1;<br>2. diğer tüm durumlarda 0. |
| **DR-114 Symmetric Fired Unit** | “firing” kaydının “symmetric fired unit” alanı, aşağıdaki öncelik sırasına göre belirlenir:<br>1. “rule enabled” bayrağı etkin, “kind” alanı “symmetric” değerine eşit ve “first is in effect” bayrağı etkin ise 1;<br>2. diğer tüm durumlarda 0. |
| **DR-115 Chain Fired Unit** | “firing” kaydının “chain fired unit” alanı, aşağıdaki öncelik sırasına göre belirlenir:<br>1. “rule enabled” bayrağı etkin, “kind” alanı “chain” değerine eşit, “first holds before chain” bayrağı etkin ve “second holds before chain” bayrağı etkin ise 1;<br>2. diğer tüm durumlarda 0. |
| **DR-116 Type Fired Unit** | “firing” kaydının “type fired unit” alanı, aşağıdaki öncelik sırasına göre belirlenir:<br>1. “rule enabled” bayrağı etkin, (“kind” alanı “domain” değerine eşit veya “kind” alanı “range” değerine eşit) ve “first holds before typing” bayrağı etkin ise 1;<br>2. diğer tüm durumlarda 0. |
| **DR-117 Fired Unit** | “firing” kaydının “fired unit” alanı, “inverse fired unit” alanı artı “symmetric fired unit” alanı artı “chain fired unit” alanı artı “type fired unit” alanı olarak hesaplanır. |
| **DR-118 Pre Chain Fired Unit** | “firing” kaydının “pre chain fired unit” alanı, “inverse fired unit” alanı artı “symmetric fired unit” alanı olarak hesaplanır. |
| **DR-119 Pre Chain Order Key** | “firing” kaydının “pre chain order key” alanı, “first sort order” alanı çarpı 10 artı “kind sort order” alanı olarak hesaplanır. |
| **DR-120 Round** | “firing” kaydının “round” alanı, aşağıdaki öncelik sırasına göre belirlenir:<br>1. “kind” alanı “chain” değerine eşit ve (“first is in effect” bayrağı etkin ve “second is in effect” bayrağı etkin) koşulu sağlanmıyor ise 2;<br>2. diğer tüm durumlarda 1. |
| **DR-121 Order Key** | “firing” kaydının “order key” alanı, “round” alanı çarpı 1000000000000000 artı “first snap position” alanı çarpı 100000000 artı “kind sort order” alanı çarpı 10000000 artı “second” alanı boş ise 0, diğer tüm durumlarda “second snap position” alanı olarak hesaplanır. |
| **DR-122 Is Chosen** | “firing” kaydı, “fired unit” alanı 1 değerine eşit, “target is conclusion” bayrağı etkin ve “order key” alanı “target first fired order key” alanı değerine eşit ise “chosen” sayılır. |
| **DR-123 Chosen Unit** | “firing” kaydının “chosen unit” alanı, aşağıdaki öncelik sırasına göre belirlenir:<br>1. “chosen” bayrağı etkin ise 1;<br>2. diğer tüm durumlarda 0. |
| **DR-124 Card Text** | “firing” kaydının “card text” alanı, aşağıdaki öncelik sırasına göre belirlenir:<br>1. “chosen” bayrağı etkin ise “Worked out by the ”, ardından “kind wording” alanı, ardından “ from “”, ardından “first words” alanı, ardından “””, ardından “kind” alanı “chain” değerine eşit ise “ and “”, ardından “second words” alanı, ardından “””, diğer tüm durumlarda “kind” alanı “domain” değerine eşit ise “ and “”, ardından “first property” alanı, ardından “ has domain ”, ardından “first property domain class” alanı, ardından “””, diğer tüm durumlarda “ and “”, ardından “first property” alanı, ardından “ has range ”, ardından “first property range class” alanı, ardından “””, ardından bir nokta;<br>2. diğer tüm durumlarda boş metin. ⚠︎ mekanik <!-- rulespeak:reword --> |
| **DR-125 Name** | “firing” kaydının “name” alanı, “firing ID” alanı ile aynı değeri taşır. |
| **DR-126 Rule Enabled** | “disjoint check” kaydının “rule enabled” alanı, “rule” alanı üzerinden bağlı kayıt “enabled” ise doğrudur. |
| **DR-127 First Holds** | “disjoint check” kaydının “first holds” alanı, “first” alanı üzerinden bağlı kayıt “holds” olarak işaretli ise doğrudur. |
| **DR-128 Second Holds** | “disjoint check” kaydının “second holds” alanı, “second” alanı üzerinden bağlı kayıt “holds” olarak işaretli ise doğrudur. |
| **DR-129 Subject Label** | “disjoint check” kaydının “subject label” alanı, “first” alanı üzerinden bağlı kayıttan alınır. |
| **DR-130 Object Label** | “disjoint check” kaydının “object label” alanı, “first” alanı üzerinden bağlı kayıttan alınır. |
| **DR-131 Violation Unit** | “disjoint check” kaydının “violation unit” alanı, aşağıdaki öncelik sırasına göre belirlenir:<br>1. “rule enabled” bayrağı etkin, “first holds” bayrağı etkin ve “second holds” bayrağı etkin ise 1;<br>2. diğer tüm durumlarda 0. |
| **DR-132 Alarm Text** | “disjoint check” kaydının “alarm text” alanı, aşağıdaki öncelik sırasına göre belirlenir:<br>1. “violation unit” alanı 1 değerine eşit ise “Contradiction found. ”, ardından “subject label” alanı, ardından “ is recorded as both parent and spouse of ”, ardından “object label” alanı, ardından “. Check those facts and the selected disjoint rule.”;<br>2. diğer tüm durumlarda boş metin. |
| **DR-133 Name** | “disjoint check” kaydının “name” alanı, “disjoint check ID” alanı ile aynı değeri taşır. |
| **DR-134 Name** | “scenario” kaydının “name” alanı, “label” alanı ile aynı değeri taşır. |
| **DR-135 Graph Has Facts to Add** | “scenario” kaydının “graph has facts to add” alanı, “graph” alanı üzerinden bağlı kayıt “facts to add” sahibi ise doğrudur. |
| **DR-136 Is Available** | “scenario” kaydı, “fact baseline” alanı “next” değerine eşit değil veya “graph has facts to add” bayrağı etkin ise “available” sayılır. |
| **DR-137 Name** | “scenario fact change” kaydının “name” alanı, “scenario fact change ID” alanı ile aynı değeri taşır. |
| **DR-138 Name** | “scenario rule setting” kaydının “name” alanı, “scenario rule setting ID” alanı ile aynı değeri taşır. |
| **DR-139 Name** | “node layout” kaydının “name” alanı, “node layout ID” alanı ile aynı değeri taşır. |
| **DR-140 Name** | “layout slot” kaydının “name” alanı, “layout slot ID” alanı ile aynı değeri taşır. |
| **DR-141 Name** | “page text” kaydının “name” alanı, “page text ID” alanı ile aynı değeri taşır. |
| **DR-142 Name** | “engine” kaydının “name” alanı, “label” alanı ile aynı değeri taşır. |
| **DR-143 Name** | “rulebook tool” kaydının “name” alanı, “label” alanı ile aynı değeri taşır. |

## 5 Şemaya İzlenebilirlik

_İfade sütunu, kuralın Dokümantasyon gösterimindeki tanımıdır —
kural kitabının sakladığı mantığın aynısı, iş okuru için yazılmış biçimiyle._

| Şema öğesi | Tür | İfade |
|----------------|------|------------|
| **Graphs.Name** | formül | `Label` |
| **Graphs.CountOfStatedFacts** | özet | `Sum(Statements.InEffectUnit via Graph)` |
| **Graphs.CountOfConclusions** | özet | `Sum(Statements.ConclusionUnit via Graph)` |
| **Graphs.CountOfViolations** | özet | `Sum(DisjointChecks.ViolationUnit via Graph)` |
| **Graphs.IsConsistent** | formül | `CountOfViolations = 0` |
| **Graphs.ConclusionsMessage** | formül | `If(CountOfConclusions = 0, NoConclusionsText, "")` |
| **Graphs.CountOfFactsToAdd** | özet | `Sum(Statements.ListedOffUnit via Graph)` |
| **Graphs.HasFactsToAdd** | formül | `CountOfFactsToAdd > 0` |
| **Graphs.NextFactSortOrder** | özet | `Min(Statements.SortOrder via Graph)` |
| **Graphs.MaxActivationSeq** | özet | `Max(Statements.ActivationSeq via Graph)` |
| **Graphs.NextActivationSeq** | formül | `MaxActivationSeq + 1` |
| **Graphs.MaxListedSortOrder** | özet | `Max(Statements.SortOrder via Graph)` |
| **Graphs.NextListedSortOrder** | formül | `MaxListedSortOrder + 1` |
| **Graphs.MaxSortOrder** | özet | `Max(Statements.SortOrder via Graph)` |
| **Graphs.NextCandidateSortOrder** | formül | `MaxSortOrder + 1` |
| **Rules.Name** | formül | `Label` |
| **Rules.CountOfFirings** | özet | `Sum(Firings.FiredUnit via Rule)` |
| **FiringKinds.Name** | formül | `Wording` |
| **FiringKinds.RuleEnabled** | arama | `Lookup(Rules.IsEnabled via Rule)` |
| **Things.PortraitName** | arama | `Lookup(Portraits.DisplayName via Portrait)` |
| **Things.PortraitPage** | arama | `Lookup(Portraits.Page via Portrait)` |
| **Things.PortraitImage** | arama | `Lookup(Portraits.Image via Portrait)` |
| **Things.PortraitCrop** | arama | `Lookup(Portraits.Crop via Portrait)` |
| **Things.PortraitWidth** | arama | `Lookup(Portraits.Width via Portrait)` |
| **Things.PortraitHeight** | arama | `Lookup(Portraits.Height via Portrait)` |
| **Things.Name** | formül | `Label` |
| **Things.NodeKey** | formül | `ThingId` |
| **Things.CountOfChildren** | özet | `Sum(Statements.ParentUnit via Subject)` |
| **Things.CountOfGrandchildren** | özet | `Sum(Statements.GrandparentUnit via Subject)` |
| **Things.CountAsPlace** | özet | `Sum(Statements.PlaceUnit via ObjectThing)` |
| **Things.CountAsSubject** | özet | `Sum(Statements.HoldsUnit via Subject)` |
| **Things.CountAsObject** | özet | `Sum(Statements.HoldsUnit via ObjectThing)` |
| **Things.IsInGraph** | formül | `Or(CountAsSubject > 0, CountAsObject > 0)` |
| **Things.NodeKind** | formül | `If(Kind = "", "", If(Kind = "class", "class", If(CountAsPlace > 0, "place", "person")))` |
| **Portraits.Name** | formül | `DisplayName` |
| **PortraitAliases.Name** | formül | `PortraitAliasId` |
| **Properties.Name** | formül | `Label` |
| **PropertyAxioms.Name** | formül | `PropertyAxiomId` |
| **Statements.SubjectLabel** | arama | `Lookup(Things.Label via Subject)` |
| **Statements.PropertyLabel** | arama | `Lookup(Properties.Label via Property)` |
| **Statements.ObjectLabel** | arama | `Lookup(Things.Label via ObjectThing)` |
| **Statements.PropertyDatatype** | arama | `Lookup(Properties.Datatype via Property)` |
| **Statements.PropertyHasOneCurrentValue** | arama | `Lookup(Properties.HasOneCurrentValue via Property)` |
| **Statements.PropertyDomainClass** | arama | `Lookup(Properties.DomainClass via Property)` |
| **Statements.PropertyRangeClass** | arama | `Lookup(Properties.RangeClass via Property)` |
| **Statements.LiteralQuote** | arama | `Lookup(Graphs.LiteralQuote via Graph)` |
| **Statements.LiteralTypeMarker** | arama | `Lookup(Graphs.LiteralTypeMarker via Graph)` |
| **Statements.BuiltInSourceText** | arama | `Lookup(Graphs.BuiltInSourceText via Graph)` |
| **Statements.UserSourceText** | arama | `Lookup(Graphs.UserSourceText via Graph)` |
| **Statements.NextFactSortOrder** | arama | `Lookup(Graphs.NextFactSortOrder via Graph)` |
| **Statements.IsLiteral** | formül | `ObjectThing = ""` |
| **Statements.ObjectText** | formül | `If(ObjectThing = "", ObjectValue, ObjectLabel)` |
| **Statements.Name** | formül | `Concat(SubjectLabel, " ", PropertyLabel, " ", ObjectText)` |
| **Statements.Words** | formül | `Concat(SubjectLabel, " ", PropertyLabel, " ", If(ObjectThing = "", Concat(LiteralQuote, ObjectValue, LiteralQuote, LiteralTypeMarker, PropertyDatatype), ObjectLabel))` |
| **Statements.Source** | formül | `If(IsBuiltIn, BuiltInSourceText, UserSourceText)` |
| **Statements.SubjectPropertyKey** | formül | `Concat(Subject, "\|", Property)` |
| **Statements.LatestSiblingActivationSeq** | özet | `Max(Statements.ActivationSeq via SubjectPropertyKey)` |
| **Statements.IsInEffect** | formül | `And(IsStated, Or(Not(PropertyHasOneCurrentValue), ActivationSeq = LatestSiblingActivationSeq))` |
| **Statements.ListedUnit** | formül | `If(IsListed, 1, 0)` |
| **Statements.InEffectUnit** | formül | `If(IsInEffect, 1, 0)` |
| **Statements.ListedOffUnit** | formül | `If(And(IsListed, Not(IsInEffect)), 1, 0)` |
| **Statements.IsNextToAdd** | formül | `And(ListedOffUnit = 1, SortOrder = NextFactSortOrder)` |
| **Statements.CountOfInverseFirings** | özet | `Sum(Firings.InverseFiredUnit via Target)` |
| **Statements.CountOfSymmetricFirings** | özet | `Sum(Firings.SymmetricFiredUnit via Target)` |
| **Statements.HoldsBeforeChain** | formül | `If(Or(IsInEffect, CountOfInverseFirings > 0, CountOfSymmetricFirings > 0), True())` |
| **Statements.CountOfChainFirings** | özet | `Sum(Firings.ChainFiredUnit via Target)` |
| **Statements.HoldsBeforeTyping** | formül | `If(Or(HoldsBeforeChain, CountOfChainFirings > 0), True())` |
| **Statements.CountOfTypeFirings** | özet | `Sum(Firings.TypeFiredUnit via Target)` |
| **Statements.Holds** | formül | `If(Or(HoldsBeforeTyping, CountOfTypeFirings > 0), True())` |
| **Statements.IsConclusion** | formül | `If(And(Holds, Not(IsInEffect)), True())` |
| **Statements.HoldsUnit** | formül | `If(Holds, 1, 0)` |
| **Statements.ConclusionUnit** | formül | `If(IsConclusion, 1, 0)` |
| **Statements.ParentUnit** | formül | `If(And(Holds, Property = "isParentOf"), 1, 0)` |
| **Statements.GrandparentUnit** | formül | `If(And(Holds, Property = "isGrandparentOf"), 1, 0)` |
| **Statements.PlaceUnit** | formül | `If(And(Holds, Property = "attends"), 1, 0)` |
| **Statements.FirstPreChainOrderKey** | özet | `Min(Firings.PreChainOrderKey via Target)` |
| **Statements.SnapPosition** | formül | `If(IsInEffect, SortOrder, If(HoldsBeforeChain, 100000 + FirstPreChainOrderKey, 0))` |
| **Statements.FirstFiredOrderKey** | özet | `Min(Firings.OrderKey via Target)` |
| **Statements.CountOfChosenFirings** | özet | `Sum(Firings.ChosenUnit via Target)` |
| **Statements.SubjectNodeKey** | formül | `Subject` |
| **Statements.ObjectNodeKey** | formül | `If(ObjectThing = "", Concat("literal\|", ObjectValue, "\|", PropertyDatatype), ObjectThing)` |
| **Statements.SubjectNodeKind** | arama | `Lookup(Things.NodeKind via Subject)` |
| **Statements.ObjectThingNodeKind** | arama | `Lookup(Things.NodeKind via ObjectThing)` |
| **Statements.ObjectNodeKind** | formül | `If(ObjectThing = "", "value", ObjectThingNodeKind)` |
| **Statements.ObjectValueNodeLabel** | arama | `Lookup(Properties.ValueNodeLabel via Property)` |
| **Statements.CountOfViolationsAsFirst** | özet | `Sum(DisjointChecks.ViolationUnit via First)` |
| **Statements.CountOfViolationsAsSecond** | özet | `Sum(DisjointChecks.ViolationUnit via Second)` |
| **Statements.IsInContradiction** | formül | `Or(CountOfViolationsAsFirst > 0, CountOfViolationsAsSecond > 0)` |
| **Firings.RuleEnabled** | arama | `Lookup(Rules.IsEnabled via Rule)` |
| **Firings.KindWording** | arama | `Lookup(FiringKinds.Wording via Kind)` |
| **Firings.KindSortOrder** | arama | `Lookup(FiringKinds.SortOrder via Kind)` |
| **Firings.TargetName** | arama | `Lookup(Statements.Name via Target)` |
| **Firings.TargetSubjectLabel** | arama | `Lookup(Statements.SubjectLabel via Target)` |
| **Firings.TargetPropertyLabel** | arama | `Lookup(Statements.PropertyLabel via Target)` |
| **Firings.TargetObjectText** | arama | `Lookup(Statements.ObjectText via Target)` |
| **Firings.TargetIsConclusion** | arama | `Lookup(Statements.IsConclusion via Target)` |
| **Firings.TargetSubjectNodeKey** | arama | `Lookup(Statements.SubjectNodeKey via Target)` |
| **Firings.TargetObjectNodeKey** | arama | `Lookup(Statements.ObjectNodeKey via Target)` |
| **Firings.TargetFirstFiredOrderKey** | arama | `Lookup(Statements.FirstFiredOrderKey via Target)` |
| **Firings.FirstWords** | arama | `Lookup(Statements.Words via First)` |
| **Firings.SecondWords** | arama | `Lookup(Statements.Words via Second)` |
| **Firings.FirstIsInEffect** | arama | `Lookup(Statements.IsInEffect via First)` |
| **Firings.FirstHoldsBeforeChain** | arama | `Lookup(Statements.HoldsBeforeChain via First)` |
| **Firings.SecondHoldsBeforeChain** | arama | `Lookup(Statements.HoldsBeforeChain via Second)` |
| **Firings.FirstHoldsBeforeTyping** | arama | `Lookup(Statements.HoldsBeforeTyping via First)` |
| **Firings.FirstProperty** | arama | `Lookup(Statements.Property via First)` |
| **Firings.FirstPropertyDomainClass** | arama | `Lookup(Statements.PropertyDomainClass via First)` |
| **Firings.FirstPropertyRangeClass** | arama | `Lookup(Statements.PropertyRangeClass via First)` |
| **Firings.FirstSortOrder** | arama | `Lookup(Statements.SortOrder via First)` |
| **Firings.SecondIsInEffect** | arama | `Lookup(Statements.IsInEffect via Second)` |
| **Firings.FirstSnapPosition** | arama | `Lookup(Statements.SnapPosition via First)` |
| **Firings.SecondSnapPosition** | arama | `Lookup(Statements.SnapPosition via Second)` |
| **Firings.InverseFiredUnit** | formül | `If(And(RuleEnabled, Kind = "inverse", FirstIsInEffect), 1, 0)` |
| **Firings.SymmetricFiredUnit** | formül | `If(And(RuleEnabled, Kind = "symmetric", FirstIsInEffect), 1, 0)` |
| **Firings.ChainFiredUnit** | formül | `If(And(RuleEnabled, Kind = "chain", FirstHoldsBeforeChain, SecondHoldsBeforeChain), 1, 0)` |
| **Firings.TypeFiredUnit** | formül | `If(And(RuleEnabled, Or(Kind = "domain", Kind = "range"), FirstHoldsBeforeTyping), 1, 0)` |
| **Firings.FiredUnit** | formül | `InverseFiredUnit + SymmetricFiredUnit + ChainFiredUnit + TypeFiredUnit` |
| **Firings.PreChainFiredUnit** | formül | `InverseFiredUnit + SymmetricFiredUnit` |
| **Firings.PreChainOrderKey** | formül | `FirstSortOrder * 10 + KindSortOrder` |
| **Firings.Round** | formül | `If(And(Kind = "chain", Not(And(FirstIsInEffect, SecondIsInEffect))), 2, 1)` |
| **Firings.OrderKey** | formül | `Round * 1000000000000000 + FirstSnapPosition * 100000000 + KindSortOrder * 10000000 + If(Second = "", 0, SecondSnapPosition)` |
| **Firings.IsChosen** | formül | `And(FiredUnit = 1, TargetIsConclusion, OrderKey = TargetFirstFiredOrderKey)` |
| **Firings.ChosenUnit** | formül | `If(IsChosen, 1, 0)` |
| **Firings.CardText** | formül | `If(IsChosen, Concat("Worked out by the ", KindWording, " from “", FirstWords, "”", If(Kind = "chain", Concat(" and “", SecondWords, "”"), If(Kind = "domain", Concat(" and “", FirstProperty, " has domain ", FirstPropertyDomainClass, "”"), If(Kind = "range", Concat(" and “", FirstProperty, " has range ", FirstPropertyRangeClass, "”"), ""))), "."), "")` |
| **Firings.Name** | formül | `FiringId` |
| **DisjointChecks.RuleEnabled** | arama | `Lookup(Rules.IsEnabled via Rule)` |
| **DisjointChecks.FirstHolds** | arama | `Lookup(Statements.Holds via First)` |
| **DisjointChecks.SecondHolds** | arama | `Lookup(Statements.Holds via Second)` |
| **DisjointChecks.SubjectLabel** | arama | `Lookup(Statements.SubjectLabel via First)` |
| **DisjointChecks.ObjectLabel** | arama | `Lookup(Statements.ObjectLabel via First)` |
| **DisjointChecks.ViolationUnit** | formül | `If(And(RuleEnabled, FirstHolds, SecondHolds), 1, 0)` |
| **DisjointChecks.AlarmText** | formül | `If(ViolationUnit = 1, Concat("Contradiction found. ", SubjectLabel, " is recorded as both parent and spouse of ", ObjectLabel, ". Check those facts and the selected disjoint rule."), "")` |
| **DisjointChecks.Name** | formül | `DisjointCheckId` |
| **Scenarios.Name** | formül | `Label` |
| **Scenarios.GraphHasFactsToAdd** | arama | `Lookup(Graphs.HasFactsToAdd via Graph)` |
| **Scenarios.IsAvailable** | formül | `Or(FactBaseline <> "next", GraphHasFactsToAdd)` |
| **ScenarioFactChanges.Name** | formül | `ScenarioFactChangeId` |
| **ScenarioRuleSettings.Name** | formül | `ScenarioRuleSettingId` |
| **NodeLayouts.Name** | formül | `NodeLayoutId` |
| **LayoutSlots.Name** | formül | `LayoutSlotId` |
| **PageTexts.Name** | formül | `PageTextId` |
| **Engines.Name** | formül | `Label` |
| **RulebookTools.Name** | formül | `Label` |

---

> **RuleSpeak®** — **Ronald G. Ross** tarafından geliştirilen, **SBVR** kurallarına uyan bildirimsel iş kuralı gösterimi — yakında geliyor. O zamana kadar bu sayfa olağan dokümantasyonu gösterir. [www.RonRoss.info](https://www.RonRoss.info)
