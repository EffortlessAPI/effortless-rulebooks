# 📘 Talisman's South Park ontology explainer — التوثيق

_Jessica Talisman's South Park Ontology Explainer, as a rulebook: the page's ten facts, five rule switches, property wiring, provenance wording and current-value policy are rows, and every conclusion is a formula. The rules apply in three fixed strata (inverse and symmetric, then the property chain, then domain and range); property wiring that feeds an earlier stratum would need repeated rounds. Every fact that could hold over the cast is a candidate row; app/lib/candidates.mjs writes the candidates when a thing, value or property is added._

> قواعد عمل تصريحية مستخرجة من دفتر القواعد. كل عبارة واردة أدناه تعبّر عن حقيقة
> في مجال العمل؛ فهي ليست إجراءً ولا أمرًا. صيغ دفتر القواعد هي المصدر الوحيد للحقيقة،
> وهذه الوثيقة قراءتها بلغة مبسّطة.

## 1 مفردات العمل

| المصطلح | الوصف | تعليق توضيحي |
|------|-------------|-------------------|
| **Graph** | The explainer page as one graph: its wording variables and its totals. | — |
| Label | سمة معرّفة. | _Display name of the graph._ |
| Literal Quote | سمة معرّفة. | _The quote mark written around a literal value in a fact's wording._ |
| Literal Type Marker | سمة معرّفة. | _The marker written between a literal value and its datatype, as in Turtle._ |
| Built in Source Text | سمة معرّفة. | _Source line shown for a fact the page states as a teaching example._ |
| User Source Text | سمة معرّفة. | _Source line shown for a fact the reader stated._ |
| No Conclusions Text | سمة معرّفة. | _Message shown when the selected facts and rules add no conclusion._ |
| Name | يطابق حقل «label» في السجل نفسه. | _Order 1. Display name of the graph._ |
| Count of Stated Facts | مجموع قيم حقل «in effect unit» عبر سجلات «statements» المرتبطة بسجل «graph». | _Order 5. Facts that are switched on and in effect._ |
| Count of Conclusions | مجموع قيم حقل «conclusion unit» عبر سجلات «statements» المرتبطة بسجل «graph». | _Order 18. Facts that hold only because an enabled rule concluded them._ |
| Count of Violations | مجموع قيم حقل «violation unit» عبر سجلات «disjoint checks» المرتبطة بسجل «graph». | _Order 18. Disjointness checks that found both links between the same pair._ |
| Is Consistent | صحيح إذا تحقق أنّ حقل «count of violations» يساوي 0. | _Order 19. True when no enabled disjointness rule is violated._ |
| Conclusions Message | يتحدد وفق الأولوية: حقل «no conclusions text» إذا تحقق أنّ حقل «count of conclusions» يساوي 0؛ وفي سائر الحالات نص فارغ. | _Order 19. The empty-state message for the conclusions panel, blank when there are conclusions._ |
| Count of Facts to Add | مجموع قيم حقل «listed off unit» عبر سجلات «statements» المرتبطة بسجل «graph». | _Order 5. Listed facts that are not in effect; the Add next fact button is enabled while this is above zero._ |
| Has Facts to Add | صحيح إذا تحقق أنّ حقل «count of facts to add» أكبر من 0. | _Order 6. True while some listed fact can still be added._ |
| Next Fact Sort Order | أصغر قيم حقل «sort order» عبر سجلات «statements» المرتبطة بسجل «graph» التي تحمل علامة «listed off unit». | _Order 5. List position of the first listed fact that is not in effect._ |
| Max Activation Seq | أكبر قيم حقل «activation seq» عبر سجلات «statements» المرتبطة بسجل «graph». | _Order 1. Highest activation number used so far._ |
| Next Activation Seq | يُحسب على النحو الآتي: حقل «max activation seq» زائد 1. | _Order 2. The activation number the next switched-on fact takes._ |
| Max Listed Sort Order | أكبر قيم حقل «sort order» عبر سجلات «statements» المرتبطة بسجل «graph» التي تحمل علامة «listed unit». | _Order 2. Highest list position among listed facts._ |
| Next Listed Sort Order | يُحسب على النحو الآتي: حقل «max listed sort order» زائد 1. | _Order 3. The list position a newly added fact takes._ |
| Max Sort Order | أكبر قيم حقل «sort order» عبر سجلات «statements» المرتبطة بسجل «graph». | _Order 1. Highest sort order of any fact, listed or candidate._ |
| Next Candidate Sort Order | يُحسب على النحو الآتي: حقل «max sort order» زائد 1. | _Order 2. The sort order a new candidate fact, or a fact removed from the list, takes._ |

<details open>
<summary>بيانات توضيحية</summary>

| Label | Literal Quote | Literal Type Marker | Built in Source Text | User Source Text | No Conclusions Text | Name ƒ | Count of Stated Facts ƒ | Count of Conclusions ƒ | Count of Violations ƒ | Is Consistent ƒ | Conclusions Message ƒ | Count of Facts to Add ƒ | Has Facts to Add ƒ | Next Fact Sort Order ƒ | Max Activation Seq ƒ | Next Activation Seq ƒ | Max Listed Sort Order ƒ | Next Listed Sort Order ƒ | Max Sort Order ƒ | Next Candidate Sort Order ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| South Park | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | These facts and selected rules add no further conclusions. Try another fact or rule. | South Park | 10 | 0 | 0 | true | These facts and selected rules add no further conclusions. Try another fact or rule. | 0 | false | — | 10 | 11 | 10 | 11 | 100269 | 100270 |

_ƒ تدل على عمود محسوب._

</details>

| المصطلح | الوصف | تعليق توضيحي |
|------|-------------|-------------------|
| **Rule** | The five rule switches on the page. | — |
| Label | سمة معرّفة. | _The rule's name on the page._ |
| Explanation | سمة معرّفة. | _The rule's one-line explanation on the page._ |
| Is Enabled | صحيح إذا تحقق أنّ نص فارغ. | _The rule's checkbox._ |
| Sort Order | سمة معرّفة. | _Position in the rule list._ |
| Name | يطابق حقل «label» في السجل نفسه. | _Order 1. The rule's name._ |
| Count of Firings | مجموع قيم حقل «fired unit» عبر سجلات «firings» المرتبطة بسجل «rule». | _Order 15. Rule applications that fired, counting every derivation._ |

<details open>
<summary>بيانات توضيحية</summary>

| Label | Explanation | Is Enabled | Sort Order | Name ƒ | Count of Firings ƒ |
|---|---|---|---|---|---|
| Inverse | A parent link also gives a child link between the same two people. | false | 1 | Inverse | 0 |
| Symmetric | A friend link or a spouse link is true for both people. | false | 2 | Symmetric | 0 |
| Property chain | A parent of a parent is a grandparent. | false | 3 | Property chain | 0 |

_ƒ تدل على عمود محسوب._

</details>

| المصطلح | الوصف | تعليق توضيحي |
|------|-------------|-------------------|
| **Firing Kind** | The ways a rule can apply. Domain and range belong to one rule but are worded apart; 'disjoint' names the rule the disjointness checks answer to. | — |
| Rule | سمة معرّفة. | _The rule switch this kind belongs to._ |
| Wording | سمة معرّفة. | _How a conclusion card names the rule, as in 'Worked out by the inverse rule'._ |
| Sort Order | سمة معرّفة. | _Order in which the page's reasoner tries the kinds for one fact._ |
| Name | يطابق حقل «wording» في السجل نفسه. | _Order 1. The kind's wording._ |
| Rule Enabled | صحيح عندما يكون السجل المرتبط عبر حقل «rule» مصنّفًا على أنه «enabled». | _Order 1. Whether the owning rule is switched on._ |

<details open>
<summary>بيانات توضيحية</summary>

| Wording | Sort Order | Name ƒ | Rule Enabled ƒ |
|---|---|---|---|
| inverse rule | 1 | inverse rule | false |
| symmetric rule | 2 | symmetric rule | false |
| property chain rule | 3 | property chain rule | false |

_ƒ تدل على عمود محسوب._

</details>

| المصطلح | الوصف | تعليق توضيحي |
|------|-------------|-------------------|
| **Thing** | Every individual and class the page can draw. | — |
| Label | سمة معرّفة. | _The name the page shows._ |
| Kind | سمة معرّفة. | _'individual' or 'class'._ |
| Sort Order | سمة معرّفة. | _Position in the cast._ |
| Portrait | سمة معرّفة. | _The portrait drawn for this thing; set when its name matches a portrait alias._ |
| Portrait Name | حقل «display name» من السجل المرتبط عبر حقل «portrait». | _Order 1. The name on the portrait's Fandom page._ |
| Portrait Page | مأخوذ من السجل المرتبط عبر حقل «portrait». | _Order 1. The Fandom page credited for the portrait._ |
| Portrait Image | مأخوذ من السجل المرتبط عبر حقل «portrait». | _Order 1. The original Fandom image._ |
| Portrait Crop | مأخوذ من السجل المرتبط عبر حقل «portrait». | _Order 1. The portrait's crop as an SVG viewBox._ |
| Portrait Width | مأخوذ من السجل المرتبط عبر حقل «portrait». | _Order 1. The original image's width in pixels._ |
| Portrait Height | مأخوذ من السجل المرتبط عبر حقل «portrait». | _Order 1. The original image's height in pixels._ |
| Name | يطابق حقل «label» في السجل نفسه. | _Order 1. The thing's name._ |
| Node Key | يطابق حقل «thing ID» في السجل نفسه. | _Order 1. The key the graph draws this thing under._ |
| Count of Children | مجموع قيم حقل «parent unit» عبر سجلات «statements» المرتبطة بسجل «thing». | _Order 17. People this thing is a parent of, stated or concluded._ |
| Count of Grandchildren | مجموع قيم حقل «grandparent unit» عبر سجلات «statements» المرتبطة بسجل «thing». | _Order 17. People this thing is a grandparent of._ |
| Count As Place | مجموع قيم حقل «place unit» عبر سجلات «statements» المرتبطة بسجل «thing». | _Order 17. Holding attends facts whose object is this thing._ |
| Count As Subject | مجموع قيم حقل «holds unit» عبر سجلات «statements» المرتبطة بسجل «thing». | _Order 17. Holding facts about this thing._ |
| Count As Object | مجموع قيم حقل «holds unit» عبر سجلات «statements» المرتبطة بسجل «thing». | _Order 17. Holding facts that point at this thing._ |
| Is in Graph | صحيح إذا تحقق أنّ واحد على الأقل من الشروط الآتية متحقق: حقل «count as subject» أكبر من 0 أو حقل «count as object» أكبر من 0. | _Order 18. True when the graph draws this thing._ |
| Node Kind | يتحدد وفق الأولوية: نص فارغ إذا تحقق أنّ حقل «kind» فارغ; «class» إذا تحقق أنّ حقل «kind» يساوي «class»; «place» إذا تحقق أنّ حقل «count as place» أكبر من 0؛ وفي سائر الحالات «person». | _Order 18. How the graph draws the thing: a class, a place someone attends, or a person._ |

<details open>
<summary>بيانات توضيحية</summary>

| Label | Kind | Sort Order | Portrait Name ƒ | Portrait Page ƒ | Portrait Image ƒ | Portrait Crop ƒ | Portrait Width ƒ | Portrait Height ƒ | Name ƒ | Node Key ƒ | Count of Children ƒ | Count of Grandchildren ƒ | Count As Place ƒ | Count As Subject ƒ | Count As Object ƒ | Is in Graph ƒ | Node Kind ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| Marvin | individual | 1 | Marvin Marsh | https://southpark.fandom.com/wiki/Marvin_Marsh | https://static.wikia.nocookie.net/southpark/images/d/db/Marsh-family-grandpa-marsh-w-purple-blanket.png/revision/latest | 48 -5 280 280 | 375 | 536 | Marvin | Marvin | 1 | 0 | 0 | 1 | 0 | true | person |
| Randy | individual | 2 | Randy Marsh | https://southpark.fandom.com/wiki/Randy_Marsh | https://static.wikia.nocookie.net/southparkfanon/images/d/d6/Randy_Marsh.png/revision/latest | 0 -10 428 428 | 428 | 1057 | Randy | Randy | 2 | 0 | 0 | 3 | 1 | true | person |
| Sharon | individual | 3 | Sharon Marsh | https://southpark.fandom.com/wiki/Sharon_Marsh | https://static.wikia.nocookie.net/southpark/images/9/99/Sharon_Marsh.png/revision/latest | 20 -10 390 390 | 439 | 1049 | Sharon | Sharon | 0 | 0 | 0 | 0 | 1 | true | person |

_ƒ تدل على عمود محسوب._

</details>

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
| **Portrait Aliase** | The names a portrait answers to. A new thing whose name matches an alias gets that portrait. | — |
| Portrait | سمة معرّفة. | _The portrait this name selects._ |
| Name | يطابق حقل «portrait alias ID» في السجل نفسه. | _Order 1. The alias._ |

<details open>
<summary>بيانات توضيحية</summary>

| Name ƒ |
|---|
| marvin |
| marvin marsh |
| grandpa marsh |

_ƒ تدل على عمود محسوب._

</details>

| المصطلح | الوصف | تعليق توضيحي |
|------|-------------|-------------------|
| **Property** | The ten properties, with what the symmetric, domain and range rules read. Axioms that relate one property to another (inverse, chain, disjoint) are rows of PropertyAxioms. | — |
| Label | سمة معرّفة. | _The property's wording on the page._ |
| Property Kind | سمة معرّفة. | _'object' links two things, 'datatype' links a thing to a value, 'type' states class membership._ |
| Datatype | سمة معرّفة. | _The value's XML Schema datatype for a datatype property; blank otherwise._ |
| Is User Addable | صحيح إذا تحقق أنّ نص فارغ. | _Offered in the page's Add fact form._ |
| Has One Current Value | صحيح إذا تحقق أنّ نص فارغ. | _The page keeps one current value per person: switching a value on replaces the earlier one._ |
| Is Symmetric | صحيح إذا تحقق أنّ نص فارغ. | _The symmetric rule makes this link true in both directions._ |
| Domain Class | سمة معرّفة. | _The class the domain rule gives this property's subject._ |
| Range Class | سمة معرّفة. | _The class the range rule gives this property's object._ |
| Sort Order | سمة معرّفة. | _Position in the Add fact form's property list._ |
| Value Node Label | سمة معرّفة. | _The small label under a value node for this property: 'role value' or 'number value'._ |
| Current Value Wording | سمة معرّفة. | _How the Add fact form names a replaced value, as in 'Stan now has grade level 5'._ |
| Input Name | سمة معرّفة. | _How the integer check names this property's value, as in 'for grade level'._ |
| Record Kind | سمة معرّفة. | _The word the current-record summary uses for this property's values: role, grade or school._ |
| Record Order | سمة معرّفة. | _Position of this property in the current-record summary._ |
| Name | يطابق حقل «label» في السجل نفسه. | _Order 1. The property's wording._ |

<details open>
<summary>بيانات توضيحية</summary>

| Label | Property Kind | Datatype | Is User Addable | Has One Current Value | Is Symmetric | Sort Order | Value Node Label | Current Value Wording | Input Name | Record Kind | Record Order | Name ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|
| is parent of | object | — | true | false | false | 1 | — | — | — | — | — | is parent of |
| is child of | object | — | true | false | false | 2 | — | — | — | — | — | is child of |
| is spouse of | object | — | true | false | true | 3 | — | — | — | — | — | is spouse of |

_ƒ تدل على عمود محسوب._

</details>

| المصطلح | الوصف | تعليق توضيحي |
|------|-------------|-------------------|
| **Property Axiom** | Axioms that relate one property to others, as OWL states them: an inverse, a two-link chain, or a disjoint pair. The candidate generator reads these rows. | — |
| Kind | سمة معرّفة. | _inverse, chain or disjoint._ |
| Property | سمة معرّفة. | _The property the axiom is about: the one inverted, the one a chain derives, or the first of a disjoint pair._ |
| First | سمة معرّفة. | _The inverse property, the chain's first link, or the other property of a disjoint pair._ |
| Second | سمة معرّفة. | _The chain's second link._ |
| Name | يطابق حقل «property axiom ID» في السجل نفسه. | _Order 1. The axiom's key._ |

<details open>
<summary>بيانات توضيحية</summary>

| Name ƒ |
|---|
| inverse:isParentOf |
| disjoint:isParentOf~isSpouseOf |
| inverse:isChildOf |

_ƒ تدل على عمود محسوب._

</details>

| المصطلح | الوصف | تعليق توضيحي |
|------|-------------|-------------------|
| **Statement** | Every fact the page can state or conclude about the cast. A listed fact appears in the page's fact list; the rest are candidates that only a rule can make hold. | — |
| Graph | سمة معرّفة. | _The graph the fact belongs to._ |
| Subject | سمة معرّفة. | _What the fact describes._ |
| Property | سمة معرّفة. | _How the subject connects to the object._ |
| Object Thing | سمة معرّفة. | _The related thing, for an object or type property._ |
| Object Value | سمة معرّفة. | _The literal value, for a datatype property._ |
| Is Listed | صحيح إذا تحقق أنّ نص فارغ. | _Shown in the page's fact list._ |
| Is Built in | صحيح إذا تحقق أنّ نص فارغ. | _One of the page's teaching facts; the reader cannot remove it._ |
| Is Detail | صحيح إذا تحقق أنّ نص فارغ. | _One of Stan's detail facts that Show Stan's details switches on._ |
| Is Stated | صحيح إذا تحقق أنّ نص فارغ. | _The fact's checkbox._ |
| Activation Seq | سمة معرّفة. | _When the fact was last switched on, as a running number; decides which value is current._ |
| Sort Order | سمة معرّفة. | _Position in the fact list; candidates follow the listed facts._ |
| Subject Label | مأخوذ من السجل المرتبط عبر حقل «subject». | _Order 1. The subject's name._ |
| Property Label | مأخوذ من السجل المرتبط عبر حقل «property». | _Order 1. The property's wording._ |
| Object Label | مأخوذ من السجل المرتبط عبر حقل «object thing». | _Order 1. The object's name, for an object or type property._ |
| Property Datatype | مأخوذ من السجل المرتبط عبر حقل «property». | _Order 1. The value's datatype, for a datatype property._ |
| Property Has One Current Value | صحيح عندما يكون السجل المرتبط عبر حقل «property» حائزًا على «one current value». | _Order 1. Whether the property keeps one current value per person._ |
| Property Domain Class | مأخوذ من السجل المرتبط عبر حقل «property». | _Order 1. The property's domain class._ |
| Property Range Class | مأخوذ من السجل المرتبط عبر حقل «property». | _Order 1. The property's range class._ |
| Literal Quote | مأخوذ من السجل المرتبط عبر حقل «graph». | _Order 1. The graph's literal quote mark._ |
| Literal Type Marker | مأخوذ من السجل المرتبط عبر حقل «graph». | _Order 1. The graph's datatype marker._ |
| Built in Source Text | مأخوذ من السجل المرتبط عبر حقل «graph». | _Order 1. The graph's source line for a teaching fact._ |
| User Source Text | مأخوذ من السجل المرتبط عبر حقل «graph». | _Order 1. The graph's source line for a reader's fact._ |
| Next Fact Sort Order | مأخوذ من السجل المرتبط عبر حقل «graph». | _Order 6. List position of the next fact to add._ |
| Is Literal | صحيح إذا تحقق أنّ حقل «object thing» فارغ. | _Order 1. True when the object is a value rather than a thing._ |
| Object Text | يتحدد وفق الأولوية: حقل «object value» إذا تحقق أنّ حقل «object thing» فارغ؛ وفي سائر الحالات حقل «object label». | _Order 2. The object's name or the value._ |
| Name | يُحسب على النحو الآتي: حقل «subject label»، ثم مسافة، ثم حقل «property label»، ثم مسافة، ثم حقل «object text». | _Order 3. The fact in words._ |
| Words | يُحسب على النحو الآتي: حقل «subject label»، ثم مسافة، ثم حقل «property label»، ثم مسافة، ثم حقل «literal quote»، ثم حقل «object value»، ثم حقل «literal quote»، ثم حقل «literal type marker»، ثم حقل «property datatype» إذا تحقق أنّ حقل «object thing» فارغ، وفي سائر الحالات حقل «object label». ⚠︎ آلي <!-- rulespeak:reword --> | _Order 2. The fact as a conclusion card quotes it, with a literal written in Turtle form._ |
| Source | يتحدد وفق الأولوية: حقل «built in source text» إذا تحقق أنّ علامة «built in» مفعّلة؛ وفي سائر الحالات حقل «user source text». | _Order 2. Where a listed fact came from._ |
| Subject Property Key | يُحسب على النحو الآتي: حقل «subject»، ثم «|»، ثم حقل «property». | _Order 1. Groups the values one subject has for one property._ |
| Latest Sibling Activation Seq | أكبر قيم حقل «activation seq» عبر سجلات «statements» المرتبطة بسجل «statement». | _Order 2. The most recent activation among this subject's values for this property._ |
| Is in Effect | صحيح إذا تحقق أنّ جميع الشروط الآتية متحققة: علامة «stated» مفعّلة؛ وواحد على الأقل من الشروط الآتية متحقق: علامة «property has one current value» غير مفعّلة أو حقل «activation seq» يساوي حقل «latest sibling activation seq». | _Order 3. Switched on and, for a one-value property, the most recently switched-on value. Drawn as a solid line._ |
| Listed Unit | يتحدد وفق الأولوية: 1 إذا تحقق أنّ علامة «listed» مفعّلة؛ وفي سائر الحالات 0. | _Order 1. 1 when the fact is in the fact list._ |
| In Effect Unit | يتحدد وفق الأولوية: 1 إذا تحقق أنّ علامة «in effect» مفعّلة؛ وفي سائر الحالات 0. | _Order 4. 1 when the fact is in effect._ |
| Listed Off Unit | يتحدد وفق الأولوية: 1 إذا تحقق أنّ جميع الشروط الآتية متحققة: علامة «listed» مفعّلة وعلامة «in effect» غير مفعّلة؛ وفي سائر الحالات 0. | _Order 4. 1 when the fact is listed but not in effect._ |
| Is Next to Add | صحيح إذا تحقق أنّ جميع الشروط الآتية متحققة: حقل «listed off unit» يساوي 1 وحقل «sort order» يساوي حقل «next fact sort order». | _Order 7. The fact the Add next fact button switches on._ |
| Count of Inverse Firings | مجموع قيم حقل «inverse fired unit» عبر سجلات «firings» المرتبطة بسجل «statement». | _Order 6. Inverse rule applications that conclude this fact._ |
| Count of Symmetric Firings | مجموع قيم حقل «symmetric fired unit» عبر سجلات «firings» المرتبطة بسجل «statement». | _Order 6. Symmetric rule applications that conclude this fact._ |
| Holds Before Chain | صحيح إذا تحقق أنّ ناتج دالة «IF» مطبّقةً على ناتج دالة «OR» مطبّقةً على حقل «is in effect»، وحقل «count of inverse firings» أكبر من 0، وحقل «count of symmetric firings» أكبر من 0؛ وصحيح. | _Order 7. TRUE when the fact is in effect or the inverse or symmetric rule concludes it. Blank when nothing states or concludes it: not known, never FALSE (open world: removing a statement does not say that its opposite is true)._ |
| Count of Chain Firings | مجموع قيم حقل «chain fired unit» عبر سجلات «firings» المرتبطة بسجل «statement». | _Order 10. Property chain applications that conclude this fact._ |
| Holds Before Typing | صحيح إذا تحقق أنّ ناتج دالة «IF» مطبّقةً على ناتج دالة «OR» مطبّقةً على حقل «holds before chain» وحقل «count of chain firings» أكبر من 0 وصحيح. | _Order 11. TRUE when the fact holds before the domain and range rules apply. Blank when nothing states or concludes it: not known, never FALSE (open world: removing a statement does not say that its opposite is true)._ |
| Count of Type Firings | مجموع قيم حقل «type fired unit» عبر سجلات «firings» المرتبطة بسجل «statement». | _Order 14. Domain and range applications that conclude this fact._ |
| Holds | صحيح إذا تحقق أنّ ناتج دالة «IF» مطبّقةً على ناتج دالة «OR» مطبّقةً على حقل «holds before typing» وحقل «count of type firings» أكبر من 0 وصحيح. | _Order 15. TRUE when in effect or concluded by an enabled rule; the graph draws it. Blank when nothing states or concludes it: not known, never FALSE (open world: removing a statement does not say that its opposite is true)._ |
| Is Conclusion | صحيح إذا تحقق أنّ ناتج دالة «IF» مطبّقةً على ناتج دالة «AND» مطبّقةً على حقل «holds» وعلامة «in effect» غير مفعّلة وصحيح. | _Order 16. TRUE when the fact holds only because a rule concluded it; drawn dashed. Blank otherwise: a stated fact is not a conclusion, and a fact nothing concludes is not known (never FALSE)._ |
| Holds Unit | يتحدد وفق الأولوية: 1 إذا تحقق أنّ علامة «holds» مفعّلة؛ وفي سائر الحالات 0. | _Order 16. 1 when the fact holds._ |
| Conclusion Unit | يتحدد وفق الأولوية: 1 إذا تحقق أنّ علامة «conclusion» مفعّلة؛ وفي سائر الحالات 0. | _Order 17. 1 when the fact is a conclusion._ |
| Parent Unit | يتحدد وفق الأولوية: 1 إذا تحقق أنّ جميع الشروط الآتية متحققة: علامة «holds» مفعّلة وحقل «property» يساوي «isParentOf»؛ وفي سائر الحالات 0. | _Order 16. 1 for a holding parent link._ |
| Grandparent Unit | يتحدد وفق الأولوية: 1 إذا تحقق أنّ جميع الشروط الآتية متحققة: علامة «holds» مفعّلة وحقل «property» يساوي «isGrandparentOf»؛ وفي سائر الحالات 0. | _Order 16. 1 for a holding grandparent link._ |
| Place Unit | يتحدد وفق الأولوية: 1 إذا تحقق أنّ جميع الشروط الآتية متحققة: علامة «holds» مفعّلة وحقل «property» يساوي «attends»؛ وفي سائر الحالات 0. | _Order 16. 1 for a holding attends link._ |
| First Pre Chain Order Key | أصغر قيم حقل «pre chain order key» عبر سجلات «firings» المرتبطة بسجل «statement» التي تحمل علامة «pre chain fired unit». | _Order 7. Order key of the first inverse or symmetric application that concludes this fact._ |
| Snap Position | يتحدد وفق الأولوية: حقل «sort order» إذا تحقق أنّ علامة «in effect» مفعّلة; 100000 زائد حقل «first pre chain order key» إذا تحقق أنّ علامة «holds before chain» مفعّلة؛ وفي سائر الحالات 0. | _Order 8. Where the page's reasoner meets this fact as a premise: a stated fact at its list position, a fact concluded by the inverse or symmetric rule after every stated fact, in the order it was concluded; 0 for a fact that does not hold yet, so a link to no fact reads the same on every engine._ |
| First Fired Order Key | أصغر قيم حقل «order key» عبر سجلات «firings» المرتبطة بسجل «statement» التي تحمل علامة «fired unit». | _Order 15. Order key of the first rule application that concludes this fact, in the page reasoner's order._ |
| Count of Chosen Firings | مجموع قيم حقل «chosen unit» عبر سجلات «firings» المرتبطة بسجل «statement». | _Order 20. Rule applications shown as this fact's conclusion card: one for a conclusion, zero otherwise._ |
| Subject Node Key | يطابق حقل «subject» في السجل نفسه. | _Order 1. The graph node this fact starts from._ |
| Object Node Key | يتحدد وفق الأولوية: «literal|»، ثم حقل «object value»، ثم «|»، ثم حقل «property datatype» إذا تحقق أنّ حقل «object thing» فارغ؛ وفي سائر الحالات حقل «object thing». | _Order 2. The graph node this fact points at: the thing, or one node per value and datatype._ |
| Subject Node Kind | مأخوذ من السجل المرتبط عبر حقل «subject». | _Order 19. How the graph draws the subject._ |
| Object Thing Node Kind | مأخوذ من السجل المرتبط عبر حقل «object thing». | _Order 19. How the graph draws the object thing._ |
| Object Node Kind | يتحدد وفق الأولوية: «value» إذا تحقق أنّ حقل «object thing» فارغ؛ وفي سائر الحالات حقل «object thing node kind». | _Order 20. How the graph draws the object: a value, a class, a place or a person._ |
| Object Value Node Label | مأخوذ من السجل المرتبط عبر حقل «property». | _Order 1. The small label under a value node._ |
| Count of Violations As First | مجموع قيم حقل «violation unit» عبر سجلات «disjoint checks» المرتبطة بسجل «statement». | _Order 18. Violated disjointness checks naming this fact first._ |
| Count of Violations As Second | مجموع قيم حقل «violation unit» عبر سجلات «disjoint checks» المرتبطة بسجل «statement». | _Order 18. Violated disjointness checks naming this fact second._ |
| Is in Contradiction | صحيح إذا تحقق أنّ واحد على الأقل من الشروط الآتية متحقق: حقل «count of violations as first» أكبر من 0 أو حقل «count of violations as second» أكبر من 0. | _Order 19. Part of a contradiction the disjoint rule found; drawn in the alarm colour._ |

<details open>
<summary>بيانات توضيحية</summary>

| Object Value | Is Listed | Is Built in | Is Detail | Is Stated | Activation Seq | Sort Order | Subject Label ƒ | Property Label ƒ | Object Label ƒ | Property Datatype ƒ | Property Has One Current Value ƒ | Property Domain Class ƒ | Property Range Class ƒ | Literal Quote ƒ | Literal Type Marker ƒ | Built in Source Text ƒ | User Source Text ƒ | Next Fact Sort Order ƒ | Is Literal ƒ | Object Text ƒ | Name ƒ | Words ƒ | Source ƒ | Subject Property Key ƒ | Latest Sibling Activation Seq ƒ | Is in Effect ƒ | Listed Unit ƒ | In Effect Unit ƒ | Listed Off Unit ƒ | Is Next to Add ƒ | Count of Inverse Firings ƒ | Count of Symmetric Firings ƒ | Holds Before Chain ƒ | Count of Chain Firings ƒ | Holds Before Typing ƒ | Count of Type Firings ƒ | Holds ƒ | Is Conclusion ƒ | Holds Unit ƒ | Conclusion Unit ƒ | Parent Unit ƒ | Grandparent Unit ƒ | Place Unit ƒ | First Pre Chain Order Key ƒ | Snap Position ƒ | First Fired Order Key ƒ | Count of Chosen Firings ƒ | Subject Node Key ƒ | Object Node Key ƒ | Subject Node Kind ƒ | Object Thing Node Kind ƒ | Object Node Kind ƒ | Object Value Node Label ƒ | Count of Violations As First ƒ | Count of Violations As Second ƒ | Is in Contradiction ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| — | true | true | false | true | 1 | 1 | Marvin | is parent of | Randy | — | false | — | — | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | — | false | Randy | Marvin is parent of Randy | Marvin is parent of Randy | Stated as a teaching example on this page. | Marvin\|isParentOf | 1 | true | 1 | 1 | 0 | false | 0 | 0 | true | 0 | true | 0 | true | — | 1 | 0 | 1 | 0 | 0 | — | 1.0 | — | 0 | Marvin | Randy | person | person | person | — | 0 | 0 | false |
| — | false | false | false | false | 0 | 100001 | Marvin | is parent of | Sharon | — | false | — | — | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | — | false | Sharon | Marvin is parent of Sharon | Marvin is parent of Sharon | Stated by you on this page. | Marvin\|isParentOf | 1 | false | 0 | 0 | 0 | false | 0 | 0 | — | 0 | — | 0 | — | — | 0 | 0 | 0 | 0 | 0 | — | 0.0 | — | 0 | Marvin | Sharon | person | person | person | — | 0 | 0 | false |
| — | false | false | false | false | 0 | 100002 | Marvin | is parent of | Stan | — | false | — | — | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | — | false | Stan | Marvin is parent of Stan | Marvin is parent of Stan | Stated by you on this page. | Marvin\|isParentOf | 1 | false | 0 | 0 | 0 | false | 0 | 0 | — | 0 | — | 0 | — | — | 0 | 0 | 0 | 0 | 0 | — | 0.0 | — | 0 | Marvin | Stan | person | person | person | — | 0 | 0 | false |

_ƒ تدل على عمود محسوب._

</details>

| المصطلح | الوصف | تعليق توضيحي |
|------|-------------|-------------------|
| **Firing** | Every way an enabled rule could conclude a candidate fact from the facts that hold. | — |
| Kind | سمة معرّفة. | _How the rule applies._ |
| Rule | سمة معرّفة. | _The rule switch._ |
| Target | سمة معرّفة. | _The fact this application concludes._ |
| First | سمة معرّفة. | _The premise._ |
| Second | سمة معرّفة. | _The second premise, for the property chain._ |
| Rule Enabled | صحيح عندما يكون السجل المرتبط عبر حقل «rule» مصنّفًا على أنه «enabled». | _Order 1. Whether the rule is switched on._ |
| Kind Wording | مأخوذ من السجل المرتبط عبر حقل «kind». | _Order 1. How the card names the rule._ |
| Kind Sort Order | مأخوذ من السجل المرتبط عبر حقل «kind». | _Order 1. The kind's place in the reasoner's order._ |
| Target Name | مأخوذ من السجل المرتبط عبر حقل «target». | _Order 4. The concluded fact in words._ |
| Target Subject Label | مأخوذ من السجل المرتبط عبر حقل «target». | _Order 2. The concluded fact's subject._ |
| Target Property Label | مأخوذ من السجل المرتبط عبر حقل «target». | _Order 2. The concluded fact's property._ |
| Target Object Text | مأخوذ من السجل المرتبط عبر حقل «target». | _Order 3. The concluded fact's object._ |
| Target is Conclusion | صحيح عندما يكون السجل المرتبط عبر حقل «target» مصنّفًا على أنه «conclusion». | _Order 17. Whether the concluded fact is new._ |
| Target Subject Node Key | مأخوذ من السجل المرتبط عبر حقل «target». | _Order 2. The concluded fact's subject node._ |
| Target Object Node Key | مأخوذ من السجل المرتبط عبر حقل «target». | _Order 3. The concluded fact's object node._ |
| Target First Fired Order Key | مأخوذ من السجل المرتبط عبر حقل «target». | _Order 16. The order key of the concluded fact's first application._ |
| First Words | مأخوذ من السجل المرتبط عبر حقل «first». | _Order 3. The premise as the card quotes it._ |
| Second Words | مأخوذ من السجل المرتبط عبر حقل «second». | _Order 3. The second premise as the card quotes it._ |
| First is in Effect | صحيح عندما يكون السجل المرتبط عبر حقل «first» مصنّفًا على أنه «in effect». | _Order 4. Whether the premise is in effect._ |
| First Holds Before Chain | صحيح عندما يكون السجل المرتبط عبر حقل «first» موسومًا بعلامة «holds before chain». | _Order 8. Whether the premise holds before the chain._ |
| Second Holds Before Chain | صحيح عندما يكون السجل المرتبط عبر حقل «second» موسومًا بعلامة «holds before chain». | _Order 8. Whether the second premise holds before the chain._ |
| First Holds Before Typing | صحيح عندما يكون السجل المرتبط عبر حقل «first» موسومًا بعلامة «holds before typing». | _Order 12. Whether the premise holds before typing._ |
| First Property | مأخوذ من السجل المرتبط عبر حقل «first». | _Order 1. The premise's property key, as the card's axiom names it._ |
| First Property Domain Class | مأخوذ من السجل المرتبط عبر حقل «first». | _Order 2. The premise property's domain class._ |
| First Property Range Class | مأخوذ من السجل المرتبط عبر حقل «first». | _Order 2. The premise property's range class._ |
| First Sort Order | مأخوذ من السجل المرتبط عبر حقل «first». | _Order 1. The premise's list position._ |
| Second is in Effect | صحيح عندما يكون السجل المرتبط عبر حقل «second» مصنّفًا على أنه «in effect». | _Order 4. Whether the second premise is in effect._ |
| First Snap Position | مأخوذ من السجل المرتبط عبر حقل «first». | _Order 9. Where the reasoner meets the premise._ |
| Second Snap Position | مأخوذ من السجل المرتبط عبر حقل «second». | _Order 9. Where the reasoner meets the second premise._ |
| Inverse Fired Unit | يتحدد وفق الأولوية: 1 إذا تحقق أنّ جميع الشروط الآتية متحققة: علامة «rule enabled» مفعّلة، وحقل «kind» يساوي «inverse»، وعلامة «first is in effect» مفعّلة؛ وفي سائر الحالات 0. | _Order 5. 1 when the inverse rule fires here._ |
| Symmetric Fired Unit | يتحدد وفق الأولوية: 1 إذا تحقق أنّ جميع الشروط الآتية متحققة: علامة «rule enabled» مفعّلة، وحقل «kind» يساوي «symmetric»، وعلامة «first is in effect» مفعّلة؛ وفي سائر الحالات 0. | _Order 5. 1 when the symmetric rule fires here._ |
| Chain Fired Unit | يتحدد وفق الأولوية: 1 إذا تحقق أنّ جميع الشروط الآتية متحققة: علامة «rule enabled» مفعّلة، وحقل «kind» يساوي «chain»، وعلامة «first holds before chain» مفعّلة، وعلامة «second holds before chain» مفعّلة؛ وفي سائر الحالات 0. | _Order 9. 1 when the property chain fires here._ |
| Type Fired Unit | يتحدد وفق الأولوية: 1 إذا تحقق أنّ جميع الشروط الآتية متحققة: علامة «rule enabled» مفعّلة؛ وواحد على الأقل من الشروط الآتية متحقق: حقل «kind» يساوي «domain» أو حقل «kind» يساوي «range»؛ وعلامة «first holds before typing» مفعّلة؛ وفي سائر الحالات 0. | _Order 13. 1 when the domain or range rule fires here._ |
| Fired Unit | يُحسب على النحو الآتي: حقل «inverse fired unit» زائد حقل «symmetric fired unit» زائد حقل «chain fired unit» زائد حقل «type fired unit». | _Order 14. 1 when this application fires._ |
| Pre Chain Fired Unit | يُحسب على النحو الآتي: حقل «inverse fired unit» زائد حقل «symmetric fired unit». | _Order 6. 1 when this is an inverse or symmetric application that fires._ |
| Pre Chain Order Key | يُحسب على النحو الآتي: حقل «first sort order» مضروبًا في 10 زائد حقل «kind sort order». | _Order 2. For an inverse or symmetric application: its place in the reasoner's first round._ |
| Round | يتحدد وفق الأولوية: 2 إذا تحقق أنّ جميع الشروط الآتية متحققة: حقل «kind» يساوي «chain»؛ والشرط الآتي غير متحقق: جميع الشروط الآتية متحققة: علامة «first is in effect» مفعّلة وعلامة «second is in effect» مفعّلة؛ وفي سائر الحالات 1. | _Order 5. The reasoner's round: 1 when every premise is a stated fact, 2 when the property chain reads a concluded fact._ |
| Order Key | يُحسب على النحو الآتي: حقل «round» مضروبًا في 1000000000000000 زائد حقل «first snap position» مضروبًا في 100000000 زائد حقل «kind sort order» مضروبًا في 10000000 زائد 0 إذا تحقق أنّ حقل «second» فارغ، وفي سائر الحالات حقل «second snap position». | _Order 10. The page reasoner's order: by round, then premise, then rule kind, then second premise._ |
| Is Chosen | صحيح إذا تحقق أنّ جميع الشروط الآتية متحققة: حقل «fired unit» يساوي 1، وعلامة «target is conclusion» مفعّلة، وحقل «order key» يساوي حقل «target first fired order key». | _Order 18. The application the conclusions panel shows for its fact: the first one in the reasoner's order._ |
| Chosen Unit | يتحدد وفق الأولوية: 1 إذا تحقق أنّ علامة «chosen» مفعّلة؛ وفي سائر الحالات 0. | _Order 19. 1 when this application is shown._ |
| Card Text | يتحدد وفق الأولوية: «Worked out by the »، ثم حقل «kind wording»، ثم « from “»، ثم حقل «first words»، ثم «”»، ثم « and “»، ثم حقل «second words»، ثم «”» إذا تحقق أنّ حقل «kind» يساوي «chain»، وفي سائر الحالات « and “»، ثم حقل «first property»، ثم « has domain »، ثم حقل «first property domain class»، ثم «”» إذا تحقق أنّ حقل «kind» يساوي «domain»، وفي سائر الحالات « and “»، ثم حقل «first property»، ثم « has range »، ثم حقل «first property range class»، ثم «”»، ثم نقطة إذا تحقق أنّ علامة «chosen» مفعّلة؛ وفي سائر الحالات نص فارغ. ⚠︎ آلي <!-- rulespeak:reword --> | _Order 19. The conclusion card's provenance line, in the page's wording._ |
| Name | يطابق حقل «firing ID» في السجل نفسه. | _Order 1. The application's key._ |

<details open>
<summary>بيانات توضيحية</summary>

| Rule Enabled ƒ | Kind Wording ƒ | Kind Sort Order ƒ | Target Name ƒ | Target Subject Label ƒ | Target Property Label ƒ | Target Object Text ƒ | Target is Conclusion ƒ | Target Subject Node Key ƒ | Target Object Node Key ƒ | Target First Fired Order Key ƒ | First Words ƒ | Second Words ƒ | First is in Effect ƒ | First Holds Before Chain ƒ | Second Holds Before Chain ƒ | First Holds Before Typing ƒ | First Property ƒ | First Property Domain Class ƒ | First Property Range Class ƒ | First Sort Order ƒ | Second is in Effect ƒ | First Snap Position ƒ | Second Snap Position ƒ | Inverse Fired Unit ƒ | Symmetric Fired Unit ƒ | Chain Fired Unit ƒ | Type Fired Unit ƒ | Fired Unit ƒ | Pre Chain Fired Unit ƒ | Pre Chain Order Key ƒ | Round ƒ | Order Key ƒ | Is Chosen ƒ | Chosen Unit ƒ | Card Text ƒ | Name ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| false | inverse rule | 1 | Randy is child of Marvin | Randy | is child of | Marvin | — | Randy | Marvin | — | Marvin is parent of Randy |    | true | true | — | true | isParentOf | — | — | 1 | false | 1.0 | 0.0 | 0 | 0 | 0 | 0 | 0 | 0 | 11.0 | 1 | 1000000110000000.0 | false | 0 | — | inverse:Marvin\|isParentOf\|Randy>Randy\|isChildOf\|Marvin |
| false | inverse rule | 1 | Sharon is child of Marvin | Sharon | is child of | Marvin | — | Sharon | Marvin | — | Marvin is parent of Sharon |    | false | — | — | — | isParentOf | — | — | 100001 | false | 0.0 | 0.0 | 0 | 0 | 0 | 0 | 0 | 0 | 1000011.0 | 1 | 1000000010000000.0 | false | 0 | — | inverse:Marvin\|isParentOf\|Sharon>Sharon\|isChildOf\|Marvin |
| false | inverse rule | 1 | Stan is child of Marvin | Stan | is child of | Marvin | — | Stan | Marvin | — | Marvin is parent of Stan |    | false | — | — | — | isParentOf | — | — | 100002 | false | 0.0 | 0.0 | 0 | 0 | 0 | 0 | 0 | 0 | 1000021.0 | 1 | 1000000010000000.0 | false | 0 | — | inverse:Marvin\|isParentOf\|Stan>Stan\|isChildOf\|Marvin |

_ƒ تدل على عمود محسوب._

</details>

| المصطلح | الوصف | تعليق توضيحي |
|------|-------------|-------------------|
| **Disjoint Check** | Every pair the disjointness rule watches: the same two people linked by two disjoint properties. | — |
| Graph | سمة معرّفة. | _The graph._ |
| Rule | سمة معرّفة. | _The rule switch._ |
| First | سمة معرّفة. | _The first link._ |
| Second | سمة معرّفة. | _The disjoint link between the same pair, in the same direction._ |
| Rule Enabled | صحيح عندما يكون السجل المرتبط عبر حقل «rule» مصنّفًا على أنه «enabled». | _Order 1. Whether the disjointness rule is on._ |
| First Holds | صحيح عندما يكون السجل المرتبط عبر حقل «first» موسومًا بعلامة «holds». | _Order 16. Whether the first link holds._ |
| Second Holds | صحيح عندما يكون السجل المرتبط عبر حقل «second» موسومًا بعلامة «holds». | _Order 16. Whether the second link holds._ |
| Subject Label | مأخوذ من السجل المرتبط عبر حقل «first». | _Order 2. The person recorded with both links._ |
| Object Label | مأخوذ من السجل المرتبط عبر حقل «first». | _Order 2. The other person._ |
| Violation Unit | يتحدد وفق الأولوية: 1 إذا تحقق أنّ جميع الشروط الآتية متحققة: علامة «rule enabled» مفعّلة، وعلامة «first holds» مفعّلة، وعلامة «second holds» مفعّلة؛ وفي سائر الحالات 0. | _Order 17. 1 when both links hold._ |
| Alarm Text | يتحدد وفق الأولوية: «Contradiction found. »، ثم حقل «subject label»، ثم « is recorded as both parent and spouse of »، ثم حقل «object label»، ثم «. Check those facts and the selected disjoint rule.» إذا تحقق أنّ حقل «violation unit» يساوي 1؛ وفي سائر الحالات نص فارغ. | _Order 18. The page's contradiction alert, blank when there is none._ |
| Name | يطابق حقل «disjoint check ID» في السجل نفسه. | _Order 1. The check's key._ |

<details open>
<summary>بيانات توضيحية</summary>

| Rule Enabled ƒ | First Holds ƒ | Second Holds ƒ | Subject Label ƒ | Object Label ƒ | Violation Unit ƒ | Alarm Text ƒ | Name ƒ |
|---|---|---|---|---|---|---|---|
| false | true | — | Marvin | Randy | 0 | — | Marvin\|isParentOf\|Randy~isSpouseOf |
| false | — | — | Marvin | Sharon | 0 | — | Marvin\|isParentOf\|Sharon~isSpouseOf |
| false | — | — | Marvin | Stan | 0 | — | Marvin\|isParentOf\|Stan~isSpouseOf |

_ƒ تدل على عمود محسوب._

</details>

| المصطلح | الوصف | تعليق توضيحي |
|------|-------------|-------------------|
| **Scenario** | The page's buttons. Each one is a fact baseline, explicit fact changes and rule settings, applied in that order. | — |
| Graph | سمة معرّفة. | _The graph._ |
| Label | سمة معرّفة. | _The button's text._ |
| Note | سمة معرّفة. | _The note shown under the buttons after this one is pressed; blank clears it._ |
| Sort Order | سمة معرّفة. | _Position among the buttons._ |
| Button Group | سمة معرّفة. | _'stan' for the two imagined changes, 'facts' for the buttons under the fact list._ |
| Is Primary | صحيح إذا تحقق أنّ نص فارغ. | _Drawn as the filled button._ |
| Fact Baseline | سمة معرّفة. | _What happens to every listed fact first: keep, built-in (teaching facts on, others off), built-in-detail, all-off, activate-all (each switched on in list order), or next (the next fact on)._ |
| Layout | سمة معرّفة. | _The node layout the graph returns to; blank keeps the current positions._ |
| Clears Selection | صحيح إذا تحقق أنّ نص فارغ. | _Clears the selected node or line._ |
| Clears Form Message | صحيح إذا تحقق أنّ نص فارغ. | _Clears the Add fact form's message._ |
| Name | يطابق حقل «label» في السجل نفسه. | _Order 1. The button's text._ |
| Graph Has Facts to Add | صحيح عندما يكون السجل المرتبط عبر حقل «graph» حائزًا على «facts to add». | _Order 7. Whether a listed fact can still be added._ |
| Is Available | صحيح إذا تحقق أنّ واحد على الأقل من الشروط الآتية متحقق: حقل «fact baseline» لا يساوي «next» أو علامة «graph has facts to add» مفعّلة. | _Order 8. False only for Add next fact once every listed fact is in effect._ |

<details open>
<summary>بيانات توضيحية</summary>

| Label | Note | Sort Order | Button Group | Is Primary | Fact Baseline | Layout | Clears Selection | Clears Form Message | Name ƒ | Graph Has Facts to Add ƒ | Is Available ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|
| Move Stan to grade 5 | This imagined change updates Stan’s grade to 5. His identifier, Person statement and family links stay the same. His other selected details are unchanged. | 1 | stan | false | keep | — | true | true | Move Stan to grade 5 | false | true |
| Stan leaves school | This imagined change clears Stan’s current school details. His identifier, Person statement and family links remain. Removing a current fact does not say that its opposite is true; school history needs dates. | 2 | stan | false | keep | — | true | true | Stan leaves school | false | true |
| Add next fact | — | 3 | facts | true | next | — | false | false | Add next fact | false | false |

_ƒ تدل على عمود محسوب._

</details>

| المصطلح | الوصف | تعليق توضيحي |
|------|-------------|-------------------|
| **Scenario Fact Change** | Fact changes a button makes after its baseline. A blank object key matches every listed value of the property. | — |
| Scenario | سمة معرّفة. | _The button._ |
| Subject | سمة معرّفة. | _The subject whose facts change._ |
| Property | سمة معرّفة. | _The property whose facts change._ |
| Object Key | سمة معرّفة. | _The object thing's key or the literal value; blank for every listed value._ |
| Action | سمة معرّفة. | _'on' switches the fact on as the newest value; 'off' switches it off._ |
| Adds to List | صحيح إذا تحقق أنّ نص فارغ. | _Adds the fact to the fact list first when it is not listed._ |
| Sort Order | سمة معرّفة. | _Order of the change within the button._ |
| Name | يطابق حقل «scenario fact change ID» في السجل نفسه. | _Order 1. The change's key._ |

<details open>
<summary>بيانات توضيحية</summary>

| Object Key | Action | Adds to List | Sort Order | Name ƒ |
|---|---|---|---|---|
| Person | on | true | 1 | stan-grade-five:Stan\|isA\|Person |
| 5 | on | true | 2 | stan-grade-five:Stan\|hasGradeLevel\|5 |
| Person | on | true | 3 | stan-leaves-school:Stan\|isA\|Person |

_ƒ تدل على عمود محسوب._

</details>

| المصطلح | الوصف | تعليق توضيحي |
|------|-------------|-------------------|
| **Scenario Rule Setting** | Rule switches a button sets. | — |
| Scenario | سمة معرّفة. | _The button._ |
| Rule | سمة معرّفة. | _The rule switch._ |
| Is Enabled | صحيح إذا تحقق أنّ نص فارغ. | _The state the button puts the switch in._ |
| Name | يطابق حقل «scenario rule setting ID» في السجل نفسه. | _Order 1. The setting's key._ |

<details open>
<summary>بيانات توضيحية</summary>

| Is Enabled | Name ƒ |
|---|---|
| false | reset-original:inverse |
| false | reset-original:symmetric |
| false | reset-original:chain |

_ƒ تدل على عمود محسوب._

</details>

| المصطلح | الوصف | تعليق توضيحي |
|------|-------------|-------------------|
| **Node Layout** | Node positions in the graph's 380 by 480 drawing, one set per layout. | — |
| Layout | سمة معرّفة. | _The layout: 'default' or 'stan-details'._ |
| Node Key | سمة معرّفة. | _The node: a thing's key, or literal\|value\|datatype._ |
| X | سمة معرّفة. | _Horizontal position._ |
| Y | سمة معرّفة. | _Vertical position._ |
| Name | يطابق حقل «node layout ID» في السجل نفسه. | _Order 1. The position's key._ |

<details open>
<summary>بيانات توضيحية</summary>

| Layout | Node Key | X | Y | Name ƒ |
|---|---|---|---|---|
| default | Marvin | 70 | 45 | default:Marvin |
| default | Randy | 195 | 125 | default:Randy |
| default | Sharon | 318 | 55 | default:Sharon |

_ƒ تدل على عمود محسوب._

</details>

| المصطلح | الوصف | تعليق توضيحي |
|------|-------------|-------------------|
| **Layout Slot** | Places for nodes no layout positions, used in turn. | — |
| Sort Order | سمة معرّفة. | _Order in which slots are used._ |
| X | سمة معرّفة. | _Horizontal position._ |
| Y | سمة معرّفة. | _Vertical position._ |
| Name | يطابق حقل «layout slot ID» في السجل نفسه. | _Order 1. The slot's key._ |

<details open>
<summary>بيانات توضيحية</summary>

| Sort Order | X | Y | Name ƒ |
|---|---|---|---|
| 1 | 330 | 135 | slot-1 |
| 2 | 45 | 120 | slot-2 |
| 3 | 330 | 400 | slot-3 |

_ƒ تدل على عمود محسوب._

</details>

| المصطلح | الوصف | تعليق توضيحي |
|------|-------------|-------------------|
| **Page Text** | The page's wording, one row per string. {name}-style placeholders are filled by the page. | — |
| Text | سمة معرّفة. | _The string._ |
| Name | يطابق حقل «page text ID» في السجل نفسه. | _Order 1. The string's key._ |

<details open>
<summary>بيانات توضيحية</summary>

| Text | Name ƒ |
|---|---|
| South Park Ontology Explainer, on one rulebook | page-title |
| A rulebook rebuild of the explainer by | byline-prefix |
| Jessica Talisman | byline-author |

_ƒ تدل على عمود محسوب._

</details>

| المصطلح | الوصف | تعليق توضيحي |
|------|-------------|-------------------|
| **Engine** | The execution engines the page can be computed by. Each is a build output of this rulebook. | — |
| Label | سمة معرّفة. | _The engine's name in the switcher._ |
| Substrate | سمة معرّفة. | _The conformance harness's name for this substrate._ |
| Is Default | صحيح إذا تحقق أنّ نص فارغ. | _The engine a new visitor starts on._ |
| Description | سمة معرّفة. | _What computes the page on this engine._ |
| Sort Order | سمة معرّفة. | _Position in the switcher._ |
| Name | يطابق حقل «label» في السجل نفسه. | _Order 1. The engine's name._ |

<details open>
<summary>بيانات توضيحية</summary>

| Label | Substrate | Is Default | Description | Sort Order | Name ƒ |
|---|---|---|---|---|---|
| Postgres views | effortless-postgres | true | Every value is a column of a generated view; the database computes it when the page reads it. | 1 | Postgres views |
| TypeScript | effortless-typescript | false | The generated TypeScript computes every field from the same rows, with no database. | 2 | TypeScript |
| OWL and SHACL rules | effortless-owl | false | The generated ontology and SHACL rules, run over the same rows by a SPARQL engine. | 3 | OWL and SHACL rules |

_ƒ تدل على عمود محسوب._

</details>

| المصطلح | الوصف | تعليق توضيحي |
|------|-------------|-------------------|
| **Rulebook Tool** | What each rulebook tool adds, for each form of the rulebook. The explainer page and the ontology form's editor list the ontology form's tools; the concept form's editor lists the concept form's. The two forms are the same facts, so they list the same tools. | — |
| Label | سمة معرّفة. | _The tool's name._ |
| Adds | سمة معرّفة. | _What it adds, for this form of the rulebook._ |
| Href | سمة معرّفة. | _Where its link goes; 'editor' means this form's running rulebook editor._ |
| Link Text | سمة معرّفة. | _The link's text._ |
| Sort Order | سمة معرّفة. | _Position in the panel._ |
| Form | سمة معرّفة. | _Which form of the rulebook the tool is about: ontology (things, properties, statements) or concept (tables)._ |
| Name | يطابق حقل «label» في السجل نفسه. | _Order 1. The tool's name._ |

<details open>
<summary>بيانات توضيحية</summary>

| Label | Adds | Href | Link Text | Sort Order | Form | Name ƒ |
|---|---|---|---|---|---|---|
| Interchangeable engines | The same page, computed by Postgres views, generated TypeScript or the generated OWL and SHACL rules. Switch with the control above; nothing else changes. | — | — | 1 | ontology | Interchangeable engines |
| Conformance | Every engine's answers for the current facts, compared value by value with the conformance harness's own rule for when two values are equal. | — | — | 2 | ontology | Conformance |
| Explainer | Hover a dotted value for its rule in plain words and its witness: the values it came from, down to the stated facts. | /rulebook-explainer-dag/pages/index.html | Open the explainer | 3 | ontology | Explainer |

_ƒ تدل على عمود محسوب._

</details>

## 2 أنواع الحقائق

- كل سجل «**firing kind**» يشير إلى سجل «**rule**» واحد فقط
- كل سجل «**thing**» يجوز أن يشير إلى سجل «**portrait**» واحد
- كل سجل «**portrait aliase**» يشير إلى سجل «**portrait**» واحد فقط
- كل سجل «**property**» يجوز أن يشير إلى سجل «**thing**» واحد
- كل سجل «**property axiom**» يشير إلى سجل «**firing kind**» واحد فقط
- كل سجل «**property axiom**» يشير إلى سجل «**property**» واحد فقط
- كل سجل «**statement**» يشير إلى سجل «**graph**» واحد فقط
- كل سجل «**statement**» يشير إلى سجل «**thing**» واحد فقط
- كل سجل «**statement**» يشير إلى سجل «**property**» واحد فقط
- كل سجل «**firing**» يشير إلى سجل «**firing kind**» واحد فقط
- كل سجل «**firing**» يشير إلى سجل «**rule**» واحد فقط
- كل سجل «**firing**» يشير إلى سجل «**statement**» واحد فقط
- كل سجل «**disjoint check**» يشير إلى سجل «**graph**» واحد فقط
- كل سجل «**disjoint check**» يشير إلى سجل «**rule**» واحد فقط
- كل سجل «**disjoint check**» يشير إلى سجل «**statement**» واحد فقط
- كل سجل «**scenario**» يشير إلى سجل «**graph**» واحد فقط
- كل سجل «**scenario fact change**» يشير إلى سجل «**scenario**» واحد فقط
- كل سجل «**scenario fact change**» يشير إلى سجل «**thing**» واحد فقط
- كل سجل «**scenario fact change**» يشير إلى سجل «**property**» واحد فقط
- كل سجل «**scenario rule setting**» يشير إلى سجل «**scenario**» واحد فقط
- كل سجل «**scenario rule setting**» يشير إلى سجل «**rule**» واحد فقط

## 3 القواعد التشغيلية

_القواعد التشغيلية تبيّن ما **يوجبه** العمل وما **يحظره** وما يوصي به (**ينبغي أن**).
القواعد البنيوية مصدرها الحقول الإلزامية والمفاتيح الأجنبية؛ أما القواعد الدلالية فمصدرها
جدول Constraints، وتستند كل قاعدة منها إلى قيمة منطقية يحسبها دفتر القواعد أصلًا
(ويُشار إليها بالرمز DR-N في القواعد التعريفية أدناه)._

### القيود البنيوية (من المخطط)

- في كل سجل «graph»، **يجب أن** تُحدّد قيمة حقل «label»، وحقل «literal quote»، وحقل «literal type marker»، وحقل «built in source text»، وحقل «user source text»، وحقل «no conclusions text».
- في كل سجل «rule»، **يجب أن** تُحدّد قيمة حقل «label»، وحقل «explanation»، وحقل «sort order»، وأن يُسجّل ما إذا كان مصنّفًا على أنه «enabled».
- **يجب أن** يشير كل سجل «firing kind» إلى سجل «rule» واحد فقط.
- في كل سجل «firing kind»، **يجب أن** تُحدّد قيمة حقل «wording» وحقل «sort order».
- في كل سجل «thing»، **يجب أن** تُحدّد قيمة حقل «label»، وحقل «kind»، وحقل «sort order».
- في كل سجل «portrait»، **يجب أن** تُحدّد قيمة حقل «display name»، وحقل «page»، وحقل «image»، وحقل «crop»، وحقل «width»، وحقل «height»، وحقل «sort order».
- **يجب أن** يشير كل سجل «portrait aliase» إلى سجل «portrait» واحد فقط.
- في كل سجل «property»، **يجب أن** تُحدّد قيمة حقل «label»، وحقل «property kind»، وحقل «sort order»، وأن يُسجّل ما إذا كان مصنّفًا على أنه «user addable»، وما إذا كان حائزًا على «one current value»، وما إذا كان مصنّفًا على أنه «symmetric».
- **يجب أن** يشير كل سجل «property axiom» إلى سجل «firing kind» واحد فقط عبر حقل «kind».
- **يجب أن** يشير كل سجل «property axiom» إلى سجل «property» واحد فقط.
- **يجب أن** يشير كل سجل «property axiom» إلى سجل «property» واحد فقط عبر حقل «first».
- **يجب أن** يشير كل سجل «statement» إلى سجل «graph» واحد فقط.
- **يجب أن** يشير كل سجل «statement» إلى سجل «thing» واحد فقط عبر حقل «subject».
- **يجب أن** يشير كل سجل «statement» إلى سجل «property» واحد فقط.
- في كل سجل «statement»، **يجب أن** تُحدّد قيمة حقل «activation seq» وحقل «sort order»، وأن يُسجّل ما إذا كان مصنّفًا على أنه «listed»، وما إذا كان مصنّفًا على أنه «built in»، وما إذا كان مصنّفًا على أنه «detail»، وما إذا كان مصنّفًا على أنه «stated».
- **يجب أن** يشير كل سجل «firing» إلى سجل «firing kind» واحد فقط عبر حقل «kind».
- **يجب أن** يشير كل سجل «firing» إلى سجل «rule» واحد فقط.
- **يجب أن** يشير كل سجل «firing» إلى سجل «statement» واحد فقط عبر حقل «target».
- **يجب أن** يشير كل سجل «firing» إلى سجل «statement» واحد فقط عبر حقل «first».
- **يجب أن** يشير كل سجل «disjoint check» إلى سجل «graph» واحد فقط.
- **يجب أن** يشير كل سجل «disjoint check» إلى سجل «rule» واحد فقط.
- **يجب أن** يشير كل سجل «disjoint check» إلى سجل «statement» واحد فقط عبر حقل «first».
- **يجب أن** يشير كل سجل «disjoint check» إلى سجل «statement» واحد فقط عبر حقل «second».
- **يجب أن** يشير كل سجل «scenario» إلى سجل «graph» واحد فقط.
- في كل سجل «scenario»، **يجب أن** تُحدّد قيمة حقل «label»، وحقل «sort order»، وحقل «button group»، وحقل «fact baseline»، وأن يُسجّل ما إذا كان مصنّفًا على أنه «primary»، وما إذا كان موسومًا بعلامة «clears selection»، وما إذا كان موسومًا بعلامة «clears form message».
- **يجب أن** يشير كل سجل «scenario fact change» إلى سجل «scenario» واحد فقط.
- **يجب أن** يشير كل سجل «scenario fact change» إلى سجل «thing» واحد فقط عبر حقل «subject».
- **يجب أن** يشير كل سجل «scenario fact change» إلى سجل «property» واحد فقط.
- في كل سجل «scenario fact change»، **يجب أن** تُحدّد قيمة حقل «action» وحقل «sort order»، وأن يُسجّل ما إذا كان موسومًا بعلامة «adds to list».
- **يجب أن** يشير كل سجل «scenario rule setting» إلى سجل «scenario» واحد فقط.
- **يجب أن** يشير كل سجل «scenario rule setting» إلى سجل «rule» واحد فقط.
- في كل سجل «scenario rule setting»، **يجب أن** يُسجّل ما إذا كان مصنّفًا على أنه «enabled».
- في كل سجل «node layout»، **يجب أن** تُحدّد قيمة حقل «layout»، وحقل «node key»، وحقل «x»، وحقل «y».
- في كل سجل «layout slot»، **يجب أن** تُحدّد قيمة حقل «sort order»، وحقل «x»، وحقل «y».
- في كل سجل «page text»، **يجب أن** تُحدّد قيمة حقل «text».
- في كل سجل «engine»، **يجب أن** تُحدّد قيمة حقل «label»، وحقل «substrate»، وحقل «description»، وحقل «sort order»، وأن يُسجّل ما إذا كان مصنّفًا على أنه «default».
- في كل سجل «rulebook tool»، **يجب أن** تُحدّد قيمة حقل «label»، وحقل «adds»، وحقل «sort order»، وحقل «form».

## 4 القواعد التعريفية

_جميع العبارات تعبّر عن حقيقة في مجال العمل؛ فهي ليست إجراءات ولا أوامر.
يُتجنّب التعبير «إذا وفقط إذا» ويُستعمل «فقط إذا»، حتى لا تُحسب الضرورة في اتجاه واحد تكافؤًا.
وتدل شارة **⚠︎ آلي** على قاعدة صياغتها الحتمية أمينة لكنها متكلّفة —
فهي إشارة إلى إعادة صياغة اختيارية لاحقة، لا إلى عيب._

| المعرّف | القاعدة التصريحية |
|----|------------------|
| **DR-1 Name** | حقل «name» في سجل «graph» يطابق حقل «label» في السجل نفسه. |
| **DR-2 Count of Stated Facts** | حقل «count of stated facts» في سجل «graph» هو مجموع قيم حقل «in effect unit» عبر سجلات «statements» المرتبطة بسجل «graph». |
| **DR-3 Count of Conclusions** | حقل «count of conclusions» في سجل «graph» هو مجموع قيم حقل «conclusion unit» عبر سجلات «statements» المرتبطة بسجل «graph». |
| **DR-4 Count of Violations** | حقل «count of violations» في سجل «graph» هو مجموع قيم حقل «violation unit» عبر سجلات «disjoint checks» المرتبطة بسجل «graph». |
| **DR-5 Is Consistent** | يُصنّف سجل «graph» على أنه «consistent» إذا تحقق أنّ حقل «count of violations» يساوي 0. |
| **DR-6 Conclusions Message** | حقل «conclusions message» في سجل «graph» يتحدد وفق ترتيب الأولوية الآتي:<br>1. حقل «no conclusions text»، إذا تحقق أنّ حقل «count of conclusions» يساوي 0؛<br>2. نص فارغ، في سائر الحالات. |
| **DR-7 Count of Facts to Add** | حقل «count of facts to add» في سجل «graph» هو مجموع قيم حقل «listed off unit» عبر سجلات «statements» المرتبطة بسجل «graph». |
| **DR-8 Has Facts to Add** | يُعدّ سجل «graph» حائزًا على «facts to add» إذا تحقق أنّ حقل «count of facts to add» أكبر من 0. |
| **DR-9 Next Fact Sort Order** | حقل «next fact sort order» في سجل «graph» هو أصغر قيم حقل «sort order» عبر سجلات «statements» المرتبطة بسجل «graph» التي تحمل علامة «listed off unit». |
| **DR-10 Max Activation Seq** | حقل «max activation seq» في سجل «graph» هو أكبر قيم حقل «activation seq» عبر سجلات «statements» المرتبطة بسجل «graph». |
| **DR-11 Next Activation Seq** | حقل «next activation seq» في سجل «graph» يُحسب على النحو الآتي: حقل «max activation seq» زائد 1. |
| **DR-12 Max Listed Sort Order** | حقل «max listed sort order» في سجل «graph» هو أكبر قيم حقل «sort order» عبر سجلات «statements» المرتبطة بسجل «graph» التي تحمل علامة «listed unit». |
| **DR-13 Next Listed Sort Order** | حقل «next listed sort order» في سجل «graph» يُحسب على النحو الآتي: حقل «max listed sort order» زائد 1. |
| **DR-14 Max Sort Order** | حقل «max sort order» في سجل «graph» هو أكبر قيم حقل «sort order» عبر سجلات «statements» المرتبطة بسجل «graph». |
| **DR-15 Next Candidate Sort Order** | حقل «next candidate sort order» في سجل «graph» يُحسب على النحو الآتي: حقل «max sort order» زائد 1. |
| **DR-16 Name** | حقل «name» في سجل «rule» يطابق حقل «label» في السجل نفسه. |
| **DR-17 Count of Firings** | حقل «count of firings» في سجل «rule» هو مجموع قيم حقل «fired unit» عبر سجلات «firings» المرتبطة بسجل «rule». |
| **DR-18 Name** | حقل «name» في سجل «firing kind» يطابق حقل «wording» في السجل نفسه. |
| **DR-19 Rule Enabled** | حقل «rule enabled» في سجل «firing kind» يكون صحيحًا عندما يكون السجل المرتبط به عبر حقل «rule» مصنّفًا على أنه «enabled». |
| **DR-20 Portrait Name** | حقل «portrait name» في سجل «thing» يأخذ قيمة حقل «display name» من السجل المرتبط به عبر حقل «portrait». |
| **DR-21 Portrait Page** | حقل «portrait page» في سجل «thing» مأخوذ من السجل المرتبط به عبر حقل «portrait». |
| **DR-22 Portrait Image** | حقل «portrait image» في سجل «thing» مأخوذ من السجل المرتبط به عبر حقل «portrait». |
| **DR-23 Portrait Crop** | حقل «portrait crop» في سجل «thing» مأخوذ من السجل المرتبط به عبر حقل «portrait». |
| **DR-24 Portrait Width** | حقل «portrait width» في سجل «thing» مأخوذ من السجل المرتبط به عبر حقل «portrait». |
| **DR-25 Portrait Height** | حقل «portrait height» في سجل «thing» مأخوذ من السجل المرتبط به عبر حقل «portrait». |
| **DR-26 Name** | حقل «name» في سجل «thing» يطابق حقل «label» في السجل نفسه. |
| **DR-27 Node Key** | حقل «node key» في سجل «thing» يطابق حقل «thing ID» في السجل نفسه. |
| **DR-28 Count of Children** | حقل «count of children» في سجل «thing» هو مجموع قيم حقل «parent unit» عبر سجلات «statements» المرتبطة بسجل «thing». |
| **DR-29 Count of Grandchildren** | حقل «count of grandchildren» في سجل «thing» هو مجموع قيم حقل «grandparent unit» عبر سجلات «statements» المرتبطة بسجل «thing». |
| **DR-30 Count As Place** | حقل «count as place» في سجل «thing» هو مجموع قيم حقل «place unit» عبر سجلات «statements» المرتبطة بسجل «thing». |
| **DR-31 Count As Subject** | حقل «count as subject» في سجل «thing» هو مجموع قيم حقل «holds unit» عبر سجلات «statements» المرتبطة بسجل «thing». |
| **DR-32 Count As Object** | حقل «count as object» في سجل «thing» هو مجموع قيم حقل «holds unit» عبر سجلات «statements» المرتبطة بسجل «thing». |
| **DR-33 Is in Graph** | يُصنّف سجل «thing» على أنه «in graph» إذا تحقق أنّ واحد على الأقل من الشروط الآتية متحقق: حقل «count as subject» أكبر من 0 أو حقل «count as object» أكبر من 0. |
| **DR-34 Node Kind** | حقل «node kind» في سجل «thing» يتحدد وفق ترتيب الأولوية الآتي:<br>1. نص فارغ، إذا تحقق أنّ حقل «kind» فارغ؛<br>2. «class»، إذا تحقق أنّ حقل «kind» يساوي «class»؛<br>3. «place»، إذا تحقق أنّ حقل «count as place» أكبر من 0؛<br>4. «person»، في سائر الحالات. |
| **DR-35 Name** | حقل «name» في سجل «portrait» يطابق حقل «display name» في السجل نفسه. |
| **DR-36 Name** | حقل «name» في سجل «portrait aliase» يطابق حقل «portrait alias ID» في السجل نفسه. |
| **DR-37 Name** | حقل «name» في سجل «property» يطابق حقل «label» في السجل نفسه. |
| **DR-38 Name** | حقل «name» في سجل «property axiom» يطابق حقل «property axiom ID» في السجل نفسه. |
| **DR-39 Subject Label** | حقل «subject label» في سجل «statement» مأخوذ من السجل المرتبط به عبر حقل «subject». |
| **DR-40 Property Label** | حقل «property label» في سجل «statement» مأخوذ من السجل المرتبط به عبر حقل «property». |
| **DR-41 Object Label** | حقل «object label» في سجل «statement» مأخوذ من السجل المرتبط به عبر حقل «object thing». |
| **DR-42 Property Datatype** | حقل «property datatype» في سجل «statement» مأخوذ من السجل المرتبط به عبر حقل «property». |
| **DR-43 Property Has One Current Value** | حقل «property has one current value» في سجل «statement» يكون صحيحًا عندما يكون السجل المرتبط به عبر حقل «property» حائزًا على «one current value». |
| **DR-44 Property Domain Class** | حقل «property domain class» في سجل «statement» مأخوذ من السجل المرتبط به عبر حقل «property». |
| **DR-45 Property Range Class** | حقل «property range class» في سجل «statement» مأخوذ من السجل المرتبط به عبر حقل «property». |
| **DR-46 Literal Quote** | حقل «literal quote» في سجل «statement» مأخوذ من السجل المرتبط به عبر حقل «graph». |
| **DR-47 Literal Type Marker** | حقل «literal type marker» في سجل «statement» مأخوذ من السجل المرتبط به عبر حقل «graph». |
| **DR-48 Built in Source Text** | حقل «built in source text» في سجل «statement» مأخوذ من السجل المرتبط به عبر حقل «graph». |
| **DR-49 User Source Text** | حقل «user source text» في سجل «statement» مأخوذ من السجل المرتبط به عبر حقل «graph». |
| **DR-50 Next Fact Sort Order** | حقل «next fact sort order» في سجل «statement» مأخوذ من السجل المرتبط به عبر حقل «graph». |
| **DR-51 Is Literal** | يُصنّف سجل «statement» على أنه «literal» إذا تحقق أنّ حقل «object thing» فارغ. |
| **DR-52 Object Text** | حقل «object text» في سجل «statement» يتحدد وفق ترتيب الأولوية الآتي:<br>1. حقل «object value»، إذا تحقق أنّ حقل «object thing» فارغ؛<br>2. حقل «object label»، في سائر الحالات. |
| **DR-53 Name** | حقل «name» في سجل «statement» يُحسب على النحو الآتي: حقل «subject label»، ثم مسافة، ثم حقل «property label»، ثم مسافة، ثم حقل «object text». |
| **DR-54 Words** | حقل «words» في سجل «statement» يُحسب على النحو الآتي: حقل «subject label»، ثم مسافة، ثم حقل «property label»، ثم مسافة، ثم حقل «literal quote»، ثم حقل «object value»، ثم حقل «literal quote»، ثم حقل «literal type marker»، ثم حقل «property datatype» إذا تحقق أنّ حقل «object thing» فارغ، وفي سائر الحالات حقل «object label». ⚠︎ آلي <!-- rulespeak:reword --> |
| **DR-55 Source** | حقل «source» في سجل «statement» يتحدد وفق ترتيب الأولوية الآتي:<br>1. حقل «built in source text»، إذا تحقق أنّ علامة «built in» مفعّلة؛<br>2. حقل «user source text»، في سائر الحالات. |
| **DR-56 Subject Property Key** | حقل «subject property key» في سجل «statement» يُحسب على النحو الآتي: حقل «subject»، ثم «|»، ثم حقل «property». |
| **DR-57 Latest Sibling Activation Seq** | حقل «latest sibling activation seq» في سجل «statement» هو أكبر قيم حقل «activation seq» عبر سجلات «statements» المرتبطة بسجل «statement». |
| **DR-58 Is in Effect** | يُصنّف سجل «statement» على أنه «in effect» إذا تحقق أنّ جميع الشروط الآتية متحققة: علامة «stated» مفعّلة؛ وواحد على الأقل من الشروط الآتية متحقق: علامة «property has one current value» غير مفعّلة أو حقل «activation seq» يساوي حقل «latest sibling activation seq». |
| **DR-59 Listed Unit** | حقل «listed unit» في سجل «statement» يتحدد وفق ترتيب الأولوية الآتي:<br>1. 1، إذا تحقق أنّ علامة «listed» مفعّلة؛<br>2. 0، في سائر الحالات. |
| **DR-60 In Effect Unit** | حقل «in effect unit» في سجل «statement» يتحدد وفق ترتيب الأولوية الآتي:<br>1. 1، إذا تحقق أنّ علامة «in effect» مفعّلة؛<br>2. 0، في سائر الحالات. |
| **DR-61 Listed Off Unit** | حقل «listed off unit» في سجل «statement» يتحدد وفق ترتيب الأولوية الآتي:<br>1. 1، إذا تحقق أنّ جميع الشروط الآتية متحققة: علامة «listed» مفعّلة وعلامة «in effect» غير مفعّلة؛<br>2. 0، في سائر الحالات. |
| **DR-62 Is Next to Add** | يُصنّف سجل «statement» على أنه «next to add» إذا تحقق أنّ جميع الشروط الآتية متحققة: حقل «listed off unit» يساوي 1 وحقل «sort order» يساوي حقل «next fact sort order». |
| **DR-63 Count of Inverse Firings** | حقل «count of inverse firings» في سجل «statement» هو مجموع قيم حقل «inverse fired unit» عبر سجلات «firings» المرتبطة بسجل «statement». |
| **DR-64 Count of Symmetric Firings** | حقل «count of symmetric firings» في سجل «statement» هو مجموع قيم حقل «symmetric fired unit» عبر سجلات «firings» المرتبطة بسجل «statement». |
| **DR-65 Holds Before Chain** | يُعدّ سجل «statement» محققًا لشرط «holds before chain» إذا تحقق أنّ ناتج دالة «IF» مطبّقةً على ناتج دالة «OR» مطبّقةً على حقل «is in effect»، وحقل «count of inverse firings» أكبر من 0، وحقل «count of symmetric firings» أكبر من 0؛ وصحيح. |
| **DR-66 Count of Chain Firings** | حقل «count of chain firings» في سجل «statement» هو مجموع قيم حقل «chain fired unit» عبر سجلات «firings» المرتبطة بسجل «statement». |
| **DR-67 Holds Before Typing** | يُعدّ سجل «statement» محققًا لشرط «holds before typing» إذا تحقق أنّ ناتج دالة «IF» مطبّقةً على ناتج دالة «OR» مطبّقةً على حقل «holds before chain» وحقل «count of chain firings» أكبر من 0 وصحيح. |
| **DR-68 Count of Type Firings** | حقل «count of type firings» في سجل «statement» هو مجموع قيم حقل «type fired unit» عبر سجلات «firings» المرتبطة بسجل «statement». |
| **DR-69 Holds** | يُعدّ سجل «statement» محققًا لشرط «holds» إذا تحقق أنّ ناتج دالة «IF» مطبّقةً على ناتج دالة «OR» مطبّقةً على حقل «holds before typing» وحقل «count of type firings» أكبر من 0 وصحيح. |
| **DR-70 Is Conclusion** | يُصنّف سجل «statement» على أنه «conclusion» إذا تحقق أنّ ناتج دالة «IF» مطبّقةً على ناتج دالة «AND» مطبّقةً على حقل «holds» وعلامة «in effect» غير مفعّلة وصحيح. |
| **DR-71 Holds Unit** | حقل «holds unit» في سجل «statement» يتحدد وفق ترتيب الأولوية الآتي:<br>1. 1، إذا تحقق أنّ علامة «holds» مفعّلة؛<br>2. 0، في سائر الحالات. |
| **DR-72 Conclusion Unit** | حقل «conclusion unit» في سجل «statement» يتحدد وفق ترتيب الأولوية الآتي:<br>1. 1، إذا تحقق أنّ علامة «conclusion» مفعّلة؛<br>2. 0، في سائر الحالات. |
| **DR-73 Parent Unit** | حقل «parent unit» في سجل «statement» يتحدد وفق ترتيب الأولوية الآتي:<br>1. 1، إذا تحقق أنّ جميع الشروط الآتية متحققة: علامة «holds» مفعّلة وحقل «property» يساوي «isParentOf»؛<br>2. 0، في سائر الحالات. |
| **DR-74 Grandparent Unit** | حقل «grandparent unit» في سجل «statement» يتحدد وفق ترتيب الأولوية الآتي:<br>1. 1، إذا تحقق أنّ جميع الشروط الآتية متحققة: علامة «holds» مفعّلة وحقل «property» يساوي «isGrandparentOf»؛<br>2. 0، في سائر الحالات. |
| **DR-75 Place Unit** | حقل «place unit» في سجل «statement» يتحدد وفق ترتيب الأولوية الآتي:<br>1. 1، إذا تحقق أنّ جميع الشروط الآتية متحققة: علامة «holds» مفعّلة وحقل «property» يساوي «attends»؛<br>2. 0، في سائر الحالات. |
| **DR-76 First Pre Chain Order Key** | حقل «first pre chain order key» في سجل «statement» هو أصغر قيم حقل «pre chain order key» عبر سجلات «firings» المرتبطة بسجل «statement» التي تحمل علامة «pre chain fired unit». |
| **DR-77 Snap Position** | حقل «snap position» في سجل «statement» يتحدد وفق ترتيب الأولوية الآتي:<br>1. حقل «sort order»، إذا تحقق أنّ علامة «in effect» مفعّلة؛<br>2. 100000 زائد حقل «first pre chain order key»، إذا تحقق أنّ علامة «holds before chain» مفعّلة؛<br>3. 0، في سائر الحالات. |
| **DR-78 First Fired Order Key** | حقل «first fired order key» في سجل «statement» هو أصغر قيم حقل «order key» عبر سجلات «firings» المرتبطة بسجل «statement» التي تحمل علامة «fired unit». |
| **DR-79 Count of Chosen Firings** | حقل «count of chosen firings» في سجل «statement» هو مجموع قيم حقل «chosen unit» عبر سجلات «firings» المرتبطة بسجل «statement». |
| **DR-80 Subject Node Key** | حقل «subject node key» في سجل «statement» يطابق حقل «subject» في السجل نفسه. |
| **DR-81 Object Node Key** | حقل «object node key» في سجل «statement» يتحدد وفق ترتيب الأولوية الآتي:<br>1. «literal|»، ثم حقل «object value»، ثم «|»، ثم حقل «property datatype»، إذا تحقق أنّ حقل «object thing» فارغ؛<br>2. حقل «object thing»، في سائر الحالات. |
| **DR-82 Subject Node Kind** | حقل «subject node kind» في سجل «statement» مأخوذ من السجل المرتبط به عبر حقل «subject». |
| **DR-83 Object Thing Node Kind** | حقل «object thing node kind» في سجل «statement» مأخوذ من السجل المرتبط به عبر حقل «object thing». |
| **DR-84 Object Node Kind** | حقل «object node kind» في سجل «statement» يتحدد وفق ترتيب الأولوية الآتي:<br>1. «value»، إذا تحقق أنّ حقل «object thing» فارغ؛<br>2. حقل «object thing node kind»، في سائر الحالات. |
| **DR-85 Object Value Node Label** | حقل «object value node label» في سجل «statement» مأخوذ من السجل المرتبط به عبر حقل «property». |
| **DR-86 Count of Violations As First** | حقل «count of violations as first» في سجل «statement» هو مجموع قيم حقل «violation unit» عبر سجلات «disjoint checks» المرتبطة بسجل «statement». |
| **DR-87 Count of Violations As Second** | حقل «count of violations as second» في سجل «statement» هو مجموع قيم حقل «violation unit» عبر سجلات «disjoint checks» المرتبطة بسجل «statement». |
| **DR-88 Is in Contradiction** | يُصنّف سجل «statement» على أنه «in contradiction» إذا تحقق أنّ واحد على الأقل من الشروط الآتية متحقق: حقل «count of violations as first» أكبر من 0 أو حقل «count of violations as second» أكبر من 0. |
| **DR-89 Rule Enabled** | حقل «rule enabled» في سجل «firing» يكون صحيحًا عندما يكون السجل المرتبط به عبر حقل «rule» مصنّفًا على أنه «enabled». |
| **DR-90 Kind Wording** | حقل «kind wording» في سجل «firing» مأخوذ من السجل المرتبط به عبر حقل «kind». |
| **DR-91 Kind Sort Order** | حقل «kind sort order» في سجل «firing» مأخوذ من السجل المرتبط به عبر حقل «kind». |
| **DR-92 Target Name** | حقل «target name» في سجل «firing» مأخوذ من السجل المرتبط به عبر حقل «target». |
| **DR-93 Target Subject Label** | حقل «target subject label» في سجل «firing» مأخوذ من السجل المرتبط به عبر حقل «target». |
| **DR-94 Target Property Label** | حقل «target property label» في سجل «firing» مأخوذ من السجل المرتبط به عبر حقل «target». |
| **DR-95 Target Object Text** | حقل «target object text» في سجل «firing» مأخوذ من السجل المرتبط به عبر حقل «target». |
| **DR-96 Target is Conclusion** | حقل «target is conclusion» في سجل «firing» يكون صحيحًا عندما يكون السجل المرتبط به عبر حقل «target» مصنّفًا على أنه «conclusion». |
| **DR-97 Target Subject Node Key** | حقل «target subject node key» في سجل «firing» مأخوذ من السجل المرتبط به عبر حقل «target». |
| **DR-98 Target Object Node Key** | حقل «target object node key» في سجل «firing» مأخوذ من السجل المرتبط به عبر حقل «target». |
| **DR-99 Target First Fired Order Key** | حقل «target first fired order key» في سجل «firing» مأخوذ من السجل المرتبط به عبر حقل «target». |
| **DR-100 First Words** | حقل «first words» في سجل «firing» مأخوذ من السجل المرتبط به عبر حقل «first». |
| **DR-101 Second Words** | حقل «second words» في سجل «firing» مأخوذ من السجل المرتبط به عبر حقل «second». |
| **DR-102 First is in Effect** | حقل «first is in effect» في سجل «firing» يكون صحيحًا عندما يكون السجل المرتبط به عبر حقل «first» مصنّفًا على أنه «in effect». |
| **DR-103 First Holds Before Chain** | حقل «first holds before chain» في سجل «firing» يكون صحيحًا عندما يكون السجل المرتبط به عبر حقل «first» موسومًا بعلامة «holds before chain». |
| **DR-104 Second Holds Before Chain** | حقل «second holds before chain» في سجل «firing» يكون صحيحًا عندما يكون السجل المرتبط به عبر حقل «second» موسومًا بعلامة «holds before chain». |
| **DR-105 First Holds Before Typing** | حقل «first holds before typing» في سجل «firing» يكون صحيحًا عندما يكون السجل المرتبط به عبر حقل «first» موسومًا بعلامة «holds before typing». |
| **DR-106 First Property** | حقل «first property» في سجل «firing» مأخوذ من السجل المرتبط به عبر حقل «first». |
| **DR-107 First Property Domain Class** | حقل «first property domain class» في سجل «firing» مأخوذ من السجل المرتبط به عبر حقل «first». |
| **DR-108 First Property Range Class** | حقل «first property range class» في سجل «firing» مأخوذ من السجل المرتبط به عبر حقل «first». |
| **DR-109 First Sort Order** | حقل «first sort order» في سجل «firing» مأخوذ من السجل المرتبط به عبر حقل «first». |
| **DR-110 Second is in Effect** | حقل «second is in effect» في سجل «firing» يكون صحيحًا عندما يكون السجل المرتبط به عبر حقل «second» مصنّفًا على أنه «in effect». |
| **DR-111 First Snap Position** | حقل «first snap position» في سجل «firing» مأخوذ من السجل المرتبط به عبر حقل «first». |
| **DR-112 Second Snap Position** | حقل «second snap position» في سجل «firing» مأخوذ من السجل المرتبط به عبر حقل «second». |
| **DR-113 Inverse Fired Unit** | حقل «inverse fired unit» في سجل «firing» يتحدد وفق ترتيب الأولوية الآتي:<br>1. 1، إذا تحقق أنّ جميع الشروط الآتية متحققة: علامة «rule enabled» مفعّلة، وحقل «kind» يساوي «inverse»، وعلامة «first is in effect» مفعّلة؛<br>2. 0، في سائر الحالات. |
| **DR-114 Symmetric Fired Unit** | حقل «symmetric fired unit» في سجل «firing» يتحدد وفق ترتيب الأولوية الآتي:<br>1. 1، إذا تحقق أنّ جميع الشروط الآتية متحققة: علامة «rule enabled» مفعّلة، وحقل «kind» يساوي «symmetric»، وعلامة «first is in effect» مفعّلة؛<br>2. 0، في سائر الحالات. |
| **DR-115 Chain Fired Unit** | حقل «chain fired unit» في سجل «firing» يتحدد وفق ترتيب الأولوية الآتي:<br>1. 1، إذا تحقق أنّ جميع الشروط الآتية متحققة: علامة «rule enabled» مفعّلة، وحقل «kind» يساوي «chain»، وعلامة «first holds before chain» مفعّلة، وعلامة «second holds before chain» مفعّلة؛<br>2. 0، في سائر الحالات. |
| **DR-116 Type Fired Unit** | حقل «type fired unit» في سجل «firing» يتحدد وفق ترتيب الأولوية الآتي:<br>1. 1، إذا تحقق أنّ جميع الشروط الآتية متحققة: علامة «rule enabled» مفعّلة؛ وواحد على الأقل من الشروط الآتية متحقق: حقل «kind» يساوي «domain» أو حقل «kind» يساوي «range»؛ وعلامة «first holds before typing» مفعّلة؛<br>2. 0، في سائر الحالات. |
| **DR-117 Fired Unit** | حقل «fired unit» في سجل «firing» يُحسب على النحو الآتي: حقل «inverse fired unit» زائد حقل «symmetric fired unit» زائد حقل «chain fired unit» زائد حقل «type fired unit». |
| **DR-118 Pre Chain Fired Unit** | حقل «pre chain fired unit» في سجل «firing» يُحسب على النحو الآتي: حقل «inverse fired unit» زائد حقل «symmetric fired unit». |
| **DR-119 Pre Chain Order Key** | حقل «pre chain order key» في سجل «firing» يُحسب على النحو الآتي: حقل «first sort order» مضروبًا في 10 زائد حقل «kind sort order». |
| **DR-120 Round** | حقل «round» في سجل «firing» يتحدد وفق ترتيب الأولوية الآتي:<br>1. 2، إذا تحقق أنّ جميع الشروط الآتية متحققة: حقل «kind» يساوي «chain»؛ والشرط الآتي غير متحقق: جميع الشروط الآتية متحققة: علامة «first is in effect» مفعّلة وعلامة «second is in effect» مفعّلة؛<br>2. 1، في سائر الحالات. |
| **DR-121 Order Key** | حقل «order key» في سجل «firing» يُحسب على النحو الآتي: حقل «round» مضروبًا في 1000000000000000 زائد حقل «first snap position» مضروبًا في 100000000 زائد حقل «kind sort order» مضروبًا في 10000000 زائد 0 إذا تحقق أنّ حقل «second» فارغ، وفي سائر الحالات حقل «second snap position». |
| **DR-122 Is Chosen** | يُصنّف سجل «firing» على أنه «chosen» إذا تحقق أنّ جميع الشروط الآتية متحققة: حقل «fired unit» يساوي 1، وعلامة «target is conclusion» مفعّلة، وحقل «order key» يساوي حقل «target first fired order key». |
| **DR-123 Chosen Unit** | حقل «chosen unit» في سجل «firing» يتحدد وفق ترتيب الأولوية الآتي:<br>1. 1، إذا تحقق أنّ علامة «chosen» مفعّلة؛<br>2. 0، في سائر الحالات. |
| **DR-124 Card Text** | حقل «card text» في سجل «firing» يتحدد وفق ترتيب الأولوية الآتي:<br>1. «Worked out by the »، ثم حقل «kind wording»، ثم « from “»، ثم حقل «first words»، ثم «”»، ثم « and “»، ثم حقل «second words»، ثم «”» إذا تحقق أنّ حقل «kind» يساوي «chain»، وفي سائر الحالات « and “»، ثم حقل «first property»، ثم « has domain »، ثم حقل «first property domain class»، ثم «”» إذا تحقق أنّ حقل «kind» يساوي «domain»، وفي سائر الحالات « and “»، ثم حقل «first property»، ثم « has range »، ثم حقل «first property range class»، ثم «”»، ثم نقطة، إذا تحقق أنّ علامة «chosen» مفعّلة؛<br>2. نص فارغ، في سائر الحالات. ⚠︎ آلي <!-- rulespeak:reword --> |
| **DR-125 Name** | حقل «name» في سجل «firing» يطابق حقل «firing ID» في السجل نفسه. |
| **DR-126 Rule Enabled** | حقل «rule enabled» في سجل «disjoint check» يكون صحيحًا عندما يكون السجل المرتبط به عبر حقل «rule» مصنّفًا على أنه «enabled». |
| **DR-127 First Holds** | حقل «first holds» في سجل «disjoint check» يكون صحيحًا عندما يكون السجل المرتبط به عبر حقل «first» موسومًا بعلامة «holds». |
| **DR-128 Second Holds** | حقل «second holds» في سجل «disjoint check» يكون صحيحًا عندما يكون السجل المرتبط به عبر حقل «second» موسومًا بعلامة «holds». |
| **DR-129 Subject Label** | حقل «subject label» في سجل «disjoint check» مأخوذ من السجل المرتبط به عبر حقل «first». |
| **DR-130 Object Label** | حقل «object label» في سجل «disjoint check» مأخوذ من السجل المرتبط به عبر حقل «first». |
| **DR-131 Violation Unit** | حقل «violation unit» في سجل «disjoint check» يتحدد وفق ترتيب الأولوية الآتي:<br>1. 1، إذا تحقق أنّ جميع الشروط الآتية متحققة: علامة «rule enabled» مفعّلة، وعلامة «first holds» مفعّلة، وعلامة «second holds» مفعّلة؛<br>2. 0، في سائر الحالات. |
| **DR-132 Alarm Text** | حقل «alarm text» في سجل «disjoint check» يتحدد وفق ترتيب الأولوية الآتي:<br>1. «Contradiction found. »، ثم حقل «subject label»، ثم « is recorded as both parent and spouse of »، ثم حقل «object label»، ثم «. Check those facts and the selected disjoint rule.»، إذا تحقق أنّ حقل «violation unit» يساوي 1؛<br>2. نص فارغ، في سائر الحالات. |
| **DR-133 Name** | حقل «name» في سجل «disjoint check» يطابق حقل «disjoint check ID» في السجل نفسه. |
| **DR-134 Name** | حقل «name» في سجل «scenario» يطابق حقل «label» في السجل نفسه. |
| **DR-135 Graph Has Facts to Add** | حقل «graph has facts to add» في سجل «scenario» يكون صحيحًا عندما يكون السجل المرتبط به عبر حقل «graph» حائزًا على «facts to add». |
| **DR-136 Is Available** | يُصنّف سجل «scenario» على أنه «available» إذا تحقق أنّ واحد على الأقل من الشروط الآتية متحقق: حقل «fact baseline» لا يساوي «next» أو علامة «graph has facts to add» مفعّلة. |
| **DR-137 Name** | حقل «name» في سجل «scenario fact change» يطابق حقل «scenario fact change ID» في السجل نفسه. |
| **DR-138 Name** | حقل «name» في سجل «scenario rule setting» يطابق حقل «scenario rule setting ID» في السجل نفسه. |
| **DR-139 Name** | حقل «name» في سجل «node layout» يطابق حقل «node layout ID» في السجل نفسه. |
| **DR-140 Name** | حقل «name» في سجل «layout slot» يطابق حقل «layout slot ID» في السجل نفسه. |
| **DR-141 Name** | حقل «name» في سجل «page text» يطابق حقل «page text ID» في السجل نفسه. |
| **DR-142 Name** | حقل «name» في سجل «engine» يطابق حقل «label» في السجل نفسه. |
| **DR-143 Name** | حقل «name» في سجل «rulebook tool» يطابق حقل «label» في السجل نفسه. |

## 5 التتبّع إلى المخطط

_عمود التعبير هو تعريف القاعدة بترميز التوثيق —
المنطق نفسه الذي يخزّنه دفتر القواعد، مكتوبًا لقارئ من أهل العمل._

| عنصر المخطط | النوع | التعبير |
|----------------|------|------------|
| **Graphs.Name** | صيغة | `Label` |
| **Graphs.CountOfStatedFacts** | تجميع | `Sum(Statements.InEffectUnit via Graph)` |
| **Graphs.CountOfConclusions** | تجميع | `Sum(Statements.ConclusionUnit via Graph)` |
| **Graphs.CountOfViolations** | تجميع | `Sum(DisjointChecks.ViolationUnit via Graph)` |
| **Graphs.IsConsistent** | صيغة | `CountOfViolations = 0` |
| **Graphs.ConclusionsMessage** | صيغة | `If(CountOfConclusions = 0, NoConclusionsText, "")` |
| **Graphs.CountOfFactsToAdd** | تجميع | `Sum(Statements.ListedOffUnit via Graph)` |
| **Graphs.HasFactsToAdd** | صيغة | `CountOfFactsToAdd > 0` |
| **Graphs.NextFactSortOrder** | تجميع | `Min(Statements.SortOrder via Graph)` |
| **Graphs.MaxActivationSeq** | تجميع | `Max(Statements.ActivationSeq via Graph)` |
| **Graphs.NextActivationSeq** | صيغة | `MaxActivationSeq + 1` |
| **Graphs.MaxListedSortOrder** | تجميع | `Max(Statements.SortOrder via Graph)` |
| **Graphs.NextListedSortOrder** | صيغة | `MaxListedSortOrder + 1` |
| **Graphs.MaxSortOrder** | تجميع | `Max(Statements.SortOrder via Graph)` |
| **Graphs.NextCandidateSortOrder** | صيغة | `MaxSortOrder + 1` |
| **Rules.Name** | صيغة | `Label` |
| **Rules.CountOfFirings** | تجميع | `Sum(Firings.FiredUnit via Rule)` |
| **FiringKinds.Name** | صيغة | `Wording` |
| **FiringKinds.RuleEnabled** | بحث | `Lookup(Rules.IsEnabled via Rule)` |
| **Things.PortraitName** | بحث | `Lookup(Portraits.DisplayName via Portrait)` |
| **Things.PortraitPage** | بحث | `Lookup(Portraits.Page via Portrait)` |
| **Things.PortraitImage** | بحث | `Lookup(Portraits.Image via Portrait)` |
| **Things.PortraitCrop** | بحث | `Lookup(Portraits.Crop via Portrait)` |
| **Things.PortraitWidth** | بحث | `Lookup(Portraits.Width via Portrait)` |
| **Things.PortraitHeight** | بحث | `Lookup(Portraits.Height via Portrait)` |
| **Things.Name** | صيغة | `Label` |
| **Things.NodeKey** | صيغة | `ThingId` |
| **Things.CountOfChildren** | تجميع | `Sum(Statements.ParentUnit via Subject)` |
| **Things.CountOfGrandchildren** | تجميع | `Sum(Statements.GrandparentUnit via Subject)` |
| **Things.CountAsPlace** | تجميع | `Sum(Statements.PlaceUnit via ObjectThing)` |
| **Things.CountAsSubject** | تجميع | `Sum(Statements.HoldsUnit via Subject)` |
| **Things.CountAsObject** | تجميع | `Sum(Statements.HoldsUnit via ObjectThing)` |
| **Things.IsInGraph** | صيغة | `Or(CountAsSubject > 0, CountAsObject > 0)` |
| **Things.NodeKind** | صيغة | `If(Kind = "", "", If(Kind = "class", "class", If(CountAsPlace > 0, "place", "person")))` |
| **Portraits.Name** | صيغة | `DisplayName` |
| **PortraitAliases.Name** | صيغة | `PortraitAliasId` |
| **Properties.Name** | صيغة | `Label` |
| **PropertyAxioms.Name** | صيغة | `PropertyAxiomId` |
| **Statements.SubjectLabel** | بحث | `Lookup(Things.Label via Subject)` |
| **Statements.PropertyLabel** | بحث | `Lookup(Properties.Label via Property)` |
| **Statements.ObjectLabel** | بحث | `Lookup(Things.Label via ObjectThing)` |
| **Statements.PropertyDatatype** | بحث | `Lookup(Properties.Datatype via Property)` |
| **Statements.PropertyHasOneCurrentValue** | بحث | `Lookup(Properties.HasOneCurrentValue via Property)` |
| **Statements.PropertyDomainClass** | بحث | `Lookup(Properties.DomainClass via Property)` |
| **Statements.PropertyRangeClass** | بحث | `Lookup(Properties.RangeClass via Property)` |
| **Statements.LiteralQuote** | بحث | `Lookup(Graphs.LiteralQuote via Graph)` |
| **Statements.LiteralTypeMarker** | بحث | `Lookup(Graphs.LiteralTypeMarker via Graph)` |
| **Statements.BuiltInSourceText** | بحث | `Lookup(Graphs.BuiltInSourceText via Graph)` |
| **Statements.UserSourceText** | بحث | `Lookup(Graphs.UserSourceText via Graph)` |
| **Statements.NextFactSortOrder** | بحث | `Lookup(Graphs.NextFactSortOrder via Graph)` |
| **Statements.IsLiteral** | صيغة | `ObjectThing = ""` |
| **Statements.ObjectText** | صيغة | `If(ObjectThing = "", ObjectValue, ObjectLabel)` |
| **Statements.Name** | صيغة | `Concat(SubjectLabel, " ", PropertyLabel, " ", ObjectText)` |
| **Statements.Words** | صيغة | `Concat(SubjectLabel, " ", PropertyLabel, " ", If(ObjectThing = "", Concat(LiteralQuote, ObjectValue, LiteralQuote, LiteralTypeMarker, PropertyDatatype), ObjectLabel))` |
| **Statements.Source** | صيغة | `If(IsBuiltIn, BuiltInSourceText, UserSourceText)` |
| **Statements.SubjectPropertyKey** | صيغة | `Concat(Subject, "\|", Property)` |
| **Statements.LatestSiblingActivationSeq** | تجميع | `Max(Statements.ActivationSeq via SubjectPropertyKey)` |
| **Statements.IsInEffect** | صيغة | `And(IsStated, Or(Not(PropertyHasOneCurrentValue), ActivationSeq = LatestSiblingActivationSeq))` |
| **Statements.ListedUnit** | صيغة | `If(IsListed, 1, 0)` |
| **Statements.InEffectUnit** | صيغة | `If(IsInEffect, 1, 0)` |
| **Statements.ListedOffUnit** | صيغة | `If(And(IsListed, Not(IsInEffect)), 1, 0)` |
| **Statements.IsNextToAdd** | صيغة | `And(ListedOffUnit = 1, SortOrder = NextFactSortOrder)` |
| **Statements.CountOfInverseFirings** | تجميع | `Sum(Firings.InverseFiredUnit via Target)` |
| **Statements.CountOfSymmetricFirings** | تجميع | `Sum(Firings.SymmetricFiredUnit via Target)` |
| **Statements.HoldsBeforeChain** | صيغة | `If(Or(IsInEffect, CountOfInverseFirings > 0, CountOfSymmetricFirings > 0), True())` |
| **Statements.CountOfChainFirings** | تجميع | `Sum(Firings.ChainFiredUnit via Target)` |
| **Statements.HoldsBeforeTyping** | صيغة | `If(Or(HoldsBeforeChain, CountOfChainFirings > 0), True())` |
| **Statements.CountOfTypeFirings** | تجميع | `Sum(Firings.TypeFiredUnit via Target)` |
| **Statements.Holds** | صيغة | `If(Or(HoldsBeforeTyping, CountOfTypeFirings > 0), True())` |
| **Statements.IsConclusion** | صيغة | `If(And(Holds, Not(IsInEffect)), True())` |
| **Statements.HoldsUnit** | صيغة | `If(Holds, 1, 0)` |
| **Statements.ConclusionUnit** | صيغة | `If(IsConclusion, 1, 0)` |
| **Statements.ParentUnit** | صيغة | `If(And(Holds, Property = "isParentOf"), 1, 0)` |
| **Statements.GrandparentUnit** | صيغة | `If(And(Holds, Property = "isGrandparentOf"), 1, 0)` |
| **Statements.PlaceUnit** | صيغة | `If(And(Holds, Property = "attends"), 1, 0)` |
| **Statements.FirstPreChainOrderKey** | تجميع | `Min(Firings.PreChainOrderKey via Target)` |
| **Statements.SnapPosition** | صيغة | `If(IsInEffect, SortOrder, If(HoldsBeforeChain, 100000 + FirstPreChainOrderKey, 0))` |
| **Statements.FirstFiredOrderKey** | تجميع | `Min(Firings.OrderKey via Target)` |
| **Statements.CountOfChosenFirings** | تجميع | `Sum(Firings.ChosenUnit via Target)` |
| **Statements.SubjectNodeKey** | صيغة | `Subject` |
| **Statements.ObjectNodeKey** | صيغة | `If(ObjectThing = "", Concat("literal\|", ObjectValue, "\|", PropertyDatatype), ObjectThing)` |
| **Statements.SubjectNodeKind** | بحث | `Lookup(Things.NodeKind via Subject)` |
| **Statements.ObjectThingNodeKind** | بحث | `Lookup(Things.NodeKind via ObjectThing)` |
| **Statements.ObjectNodeKind** | صيغة | `If(ObjectThing = "", "value", ObjectThingNodeKind)` |
| **Statements.ObjectValueNodeLabel** | بحث | `Lookup(Properties.ValueNodeLabel via Property)` |
| **Statements.CountOfViolationsAsFirst** | تجميع | `Sum(DisjointChecks.ViolationUnit via First)` |
| **Statements.CountOfViolationsAsSecond** | تجميع | `Sum(DisjointChecks.ViolationUnit via Second)` |
| **Statements.IsInContradiction** | صيغة | `Or(CountOfViolationsAsFirst > 0, CountOfViolationsAsSecond > 0)` |
| **Firings.RuleEnabled** | بحث | `Lookup(Rules.IsEnabled via Rule)` |
| **Firings.KindWording** | بحث | `Lookup(FiringKinds.Wording via Kind)` |
| **Firings.KindSortOrder** | بحث | `Lookup(FiringKinds.SortOrder via Kind)` |
| **Firings.TargetName** | بحث | `Lookup(Statements.Name via Target)` |
| **Firings.TargetSubjectLabel** | بحث | `Lookup(Statements.SubjectLabel via Target)` |
| **Firings.TargetPropertyLabel** | بحث | `Lookup(Statements.PropertyLabel via Target)` |
| **Firings.TargetObjectText** | بحث | `Lookup(Statements.ObjectText via Target)` |
| **Firings.TargetIsConclusion** | بحث | `Lookup(Statements.IsConclusion via Target)` |
| **Firings.TargetSubjectNodeKey** | بحث | `Lookup(Statements.SubjectNodeKey via Target)` |
| **Firings.TargetObjectNodeKey** | بحث | `Lookup(Statements.ObjectNodeKey via Target)` |
| **Firings.TargetFirstFiredOrderKey** | بحث | `Lookup(Statements.FirstFiredOrderKey via Target)` |
| **Firings.FirstWords** | بحث | `Lookup(Statements.Words via First)` |
| **Firings.SecondWords** | بحث | `Lookup(Statements.Words via Second)` |
| **Firings.FirstIsInEffect** | بحث | `Lookup(Statements.IsInEffect via First)` |
| **Firings.FirstHoldsBeforeChain** | بحث | `Lookup(Statements.HoldsBeforeChain via First)` |
| **Firings.SecondHoldsBeforeChain** | بحث | `Lookup(Statements.HoldsBeforeChain via Second)` |
| **Firings.FirstHoldsBeforeTyping** | بحث | `Lookup(Statements.HoldsBeforeTyping via First)` |
| **Firings.FirstProperty** | بحث | `Lookup(Statements.Property via First)` |
| **Firings.FirstPropertyDomainClass** | بحث | `Lookup(Statements.PropertyDomainClass via First)` |
| **Firings.FirstPropertyRangeClass** | بحث | `Lookup(Statements.PropertyRangeClass via First)` |
| **Firings.FirstSortOrder** | بحث | `Lookup(Statements.SortOrder via First)` |
| **Firings.SecondIsInEffect** | بحث | `Lookup(Statements.IsInEffect via Second)` |
| **Firings.FirstSnapPosition** | بحث | `Lookup(Statements.SnapPosition via First)` |
| **Firings.SecondSnapPosition** | بحث | `Lookup(Statements.SnapPosition via Second)` |
| **Firings.InverseFiredUnit** | صيغة | `If(And(RuleEnabled, Kind = "inverse", FirstIsInEffect), 1, 0)` |
| **Firings.SymmetricFiredUnit** | صيغة | `If(And(RuleEnabled, Kind = "symmetric", FirstIsInEffect), 1, 0)` |
| **Firings.ChainFiredUnit** | صيغة | `If(And(RuleEnabled, Kind = "chain", FirstHoldsBeforeChain, SecondHoldsBeforeChain), 1, 0)` |
| **Firings.TypeFiredUnit** | صيغة | `If(And(RuleEnabled, Or(Kind = "domain", Kind = "range"), FirstHoldsBeforeTyping), 1, 0)` |
| **Firings.FiredUnit** | صيغة | `InverseFiredUnit + SymmetricFiredUnit + ChainFiredUnit + TypeFiredUnit` |
| **Firings.PreChainFiredUnit** | صيغة | `InverseFiredUnit + SymmetricFiredUnit` |
| **Firings.PreChainOrderKey** | صيغة | `FirstSortOrder * 10 + KindSortOrder` |
| **Firings.Round** | صيغة | `If(And(Kind = "chain", Not(And(FirstIsInEffect, SecondIsInEffect))), 2, 1)` |
| **Firings.OrderKey** | صيغة | `Round * 1000000000000000 + FirstSnapPosition * 100000000 + KindSortOrder * 10000000 + If(Second = "", 0, SecondSnapPosition)` |
| **Firings.IsChosen** | صيغة | `And(FiredUnit = 1, TargetIsConclusion, OrderKey = TargetFirstFiredOrderKey)` |
| **Firings.ChosenUnit** | صيغة | `If(IsChosen, 1, 0)` |
| **Firings.CardText** | صيغة | `If(IsChosen, Concat("Worked out by the ", KindWording, " from “", FirstWords, "”", If(Kind = "chain", Concat(" and “", SecondWords, "”"), If(Kind = "domain", Concat(" and “", FirstProperty, " has domain ", FirstPropertyDomainClass, "”"), If(Kind = "range", Concat(" and “", FirstProperty, " has range ", FirstPropertyRangeClass, "”"), ""))), "."), "")` |
| **Firings.Name** | صيغة | `FiringId` |
| **DisjointChecks.RuleEnabled** | بحث | `Lookup(Rules.IsEnabled via Rule)` |
| **DisjointChecks.FirstHolds** | بحث | `Lookup(Statements.Holds via First)` |
| **DisjointChecks.SecondHolds** | بحث | `Lookup(Statements.Holds via Second)` |
| **DisjointChecks.SubjectLabel** | بحث | `Lookup(Statements.SubjectLabel via First)` |
| **DisjointChecks.ObjectLabel** | بحث | `Lookup(Statements.ObjectLabel via First)` |
| **DisjointChecks.ViolationUnit** | صيغة | `If(And(RuleEnabled, FirstHolds, SecondHolds), 1, 0)` |
| **DisjointChecks.AlarmText** | صيغة | `If(ViolationUnit = 1, Concat("Contradiction found. ", SubjectLabel, " is recorded as both parent and spouse of ", ObjectLabel, ". Check those facts and the selected disjoint rule."), "")` |
| **DisjointChecks.Name** | صيغة | `DisjointCheckId` |
| **Scenarios.Name** | صيغة | `Label` |
| **Scenarios.GraphHasFactsToAdd** | بحث | `Lookup(Graphs.HasFactsToAdd via Graph)` |
| **Scenarios.IsAvailable** | صيغة | `Or(FactBaseline <> "next", GraphHasFactsToAdd)` |
| **ScenarioFactChanges.Name** | صيغة | `ScenarioFactChangeId` |
| **ScenarioRuleSettings.Name** | صيغة | `ScenarioRuleSettingId` |
| **NodeLayouts.Name** | صيغة | `NodeLayoutId` |
| **LayoutSlots.Name** | صيغة | `LayoutSlotId` |
| **PageTexts.Name** | صيغة | `PageTextId` |
| **Engines.Name** | صيغة | `Label` |
| **RulebookTools.Name** | صيغة | `Label` |

---

> **RuleSpeak®** — صيغة قواعد العمل التصريحية التي ابتكرها **Ronald G. Ross** وفق اصطلاحات **SBVR** — قادمة قريبًا. إلى ذلك الحين تعرض هذه الصفحة التوثيق المعتاد. [www.RonRoss.info](https://www.RonRoss.info)
