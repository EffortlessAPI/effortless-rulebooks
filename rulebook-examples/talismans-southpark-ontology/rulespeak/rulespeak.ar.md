# 📘 Talisman's South Park ontology explainer — التوثيق

_Jessica Talisman's South Park Ontology Explainer, as a rulebook: the page's ten facts, five rule switches, property wiring, provenance wording and current-value policy are rows, and every conclusion is a formula. The rules apply in three fixed strata (inverse and symmetric, then the property chain, then domain and range); property wiring that feeds an earlier stratum would need repeated rounds._

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

<details open>
<summary>بيانات توضيحية</summary>

| Label | Literal Quote | Literal Type Marker | Built in Source Text | User Source Text | No Conclusions Text | Name ƒ | Count of Stated Facts ƒ | Count of Conclusions ƒ | Count of Violations ƒ | Is Consistent ƒ | Conclusions Message ƒ | Count of Facts to Add ƒ | Has Facts to Add ƒ | Next Fact Sort Order ƒ | Max Activation Seq ƒ | Next Activation Seq ƒ | Max Listed Sort Order ƒ | Next Listed Sort Order ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| South Park | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | These facts and selected rules add no further conclusions. Try another fact or rule. | South Park | 10 | 0 | 0 | true | These facts and selected rules add no further conclusions. Try another fact or rule. | 0 | false | — | 10 | 11 | 10 | 11 |

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
| **Firing Kind** | The ways a rule can apply. Domain and range belong to one rule but are worded apart. | — |
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
| Portrait Name | سمة معرّفة. | _The name on the portrait's Fandom page._ |
| Portrait Page | سمة معرّفة. | _The Fandom page credited for the portrait._ |
| Portrait Image | سمة معرّفة. | _The original Fandom image._ |
| Portrait Crop | سمة معرّفة. | _The portrait's crop as an SVG viewBox: x y width height._ |
| Name | يطابق حقل «label» في السجل نفسه. | _Order 1. The thing's name._ |
| Count of Children | مجموع قيم حقل «parent unit» عبر سجلات «statements» المرتبطة بسجل «thing». | _Order 17. People this thing is a parent of, stated or concluded._ |
| Count of Grandchildren | مجموع قيم حقل «grandparent unit» عبر سجلات «statements» المرتبطة بسجل «thing». | _Order 17. People this thing is a grandparent of._ |
| Count As Place | مجموع قيم حقل «place unit» عبر سجلات «statements» المرتبطة بسجل «thing». | _Order 17. Holding attends facts whose object is this thing._ |
| Count As Subject | مجموع قيم حقل «holds unit» عبر سجلات «statements» المرتبطة بسجل «thing». | _Order 17. Holding facts about this thing._ |
| Count As Object | مجموع قيم حقل «holds unit» عبر سجلات «statements» المرتبطة بسجل «thing». | _Order 17. Holding facts that point at this thing._ |
| Is in Graph | صحيح إذا تحقق أنّ واحد على الأقل من الشروط الآتية متحقق: حقل «count as subject» أكبر من 0 أو حقل «count as object» أكبر من 0. | _Order 18. True when the graph draws this thing._ |
| Node Kind | يتحدد وفق الأولوية: «class» إذا تحقق أنّ حقل «kind» يساوي «class»; «place» إذا تحقق أنّ حقل «count as place» أكبر من 0؛ وفي سائر الحالات «person». | _Order 18. How the graph draws the thing: a class, a place someone attends, or a person._ |

<details open>
<summary>بيانات توضيحية</summary>

| Label | Kind | Sort Order | Portrait Name | Portrait Page | Portrait Image | Portrait Crop | Name ƒ | Count of Children ƒ | Count of Grandchildren ƒ | Count As Place ƒ | Count As Subject ƒ | Count As Object ƒ | Is in Graph ƒ | Node Kind ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| Marvin | individual | 1 | Marvin Marsh | https://southpark.fandom.com/wiki/Marvin_Marsh | https://static.wikia.nocookie.net/southpark/images/d/db/Marsh-family-grandpa-marsh-w-purple-blanket.png/revision/latest | 48 -5 280 280 | Marvin | 1 | 0 | 0 | 1 | 0 | true | person |
| Randy | individual | 2 | Randy Marsh | https://southpark.fandom.com/wiki/Randy_Marsh | https://static.wikia.nocookie.net/southparkfanon/images/d/d6/Randy_Marsh.png/revision/latest | 0 -10 428 428 | Randy | 2 | 0 | 0 | 3 | 1 | true | person |
| Sharon | individual | 3 | Sharon Marsh | https://southpark.fandom.com/wiki/Sharon_Marsh | https://static.wikia.nocookie.net/southpark/images/9/99/Sharon_Marsh.png/revision/latest | 20 -10 390 390 | Sharon | 0 | 0 | 0 | 0 | 1 | true | person |

_ƒ تدل على عمود محسوب._

</details>

| المصطلح | الوصف | تعليق توضيحي |
|------|-------------|-------------------|
| **Property** | The ten properties and the wiring every rule reads. | — |
| Label | سمة معرّفة. | _The property's wording on the page._ |
| Property Kind | سمة معرّفة. | _'object' links two things, 'datatype' links a thing to a value, 'type' states class membership._ |
| Datatype | سمة معرّفة. | _The value's XML Schema datatype for a datatype property; blank otherwise._ |
| Is User Addable | صحيح إذا تحقق أنّ نص فارغ. | _Offered in the page's Add fact form._ |
| Has One Current Value | صحيح إذا تحقق أنّ نص فارغ. | _The page keeps one current value per person: switching a value on replaces the earlier one._ |
| Inverse of | سمة معرّفة. | _The property the inverse rule derives from this one._ |
| Is Symmetric | صحيح إذا تحقق أنّ نص فارغ. | _The symmetric rule makes this link true in both directions._ |
| Chain First | سمة معرّفة. | _First link of the property chain that derives this property._ |
| Chain Second | سمة معرّفة. | _Second link of the property chain that derives this property._ |
| Domain Class | سمة معرّفة. | _The class the domain rule gives this property's subject._ |
| Range Class | سمة معرّفة. | _The class the range rule gives this property's object._ |
| Disjoint With | سمة معرّفة. | _A property that may not link the same pair, in the same direction, as this one._ |
| Sort Order | سمة معرّفة. | _Position in the Add fact form's property list._ |
| Name | يطابق حقل «label» في السجل نفسه. | _Order 1. The property's wording._ |

<details open>
<summary>بيانات توضيحية</summary>

| Label | Property Kind | Datatype | Is User Addable | Has One Current Value | Is Symmetric | Sort Order | Name ƒ |
|---|---|---|---|---|---|---|---|
| is parent of | object | — | true | false | false | 1 | is parent of |
| is child of | object | — | true | false | false | 2 | is child of |
| is spouse of | object | — | true | false | true | 3 | is spouse of |

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
| Holds Before Chain | صحيح إذا تحقق أنّ واحد على الأقل من الشروط الآتية متحقق: علامة «in effect» مفعّلة، أو حقل «count of inverse firings» أكبر من 0، أو حقل «count of symmetric firings» أكبر من 0. | _Order 7. Holds from facts in effect plus the inverse and symmetric rules._ |
| Count of Chain Firings | مجموع قيم حقل «chain fired unit» عبر سجلات «firings» المرتبطة بسجل «statement». | _Order 10. Property chain applications that conclude this fact._ |
| Holds Before Typing | صحيح إذا تحقق أنّ واحد على الأقل من الشروط الآتية متحقق: علامة «holds before chain» مفعّلة أو حقل «count of chain firings» أكبر من 0. | _Order 11. Holds before the domain and range rules apply._ |
| Count of Type Firings | مجموع قيم حقل «type fired unit» عبر سجلات «firings» المرتبطة بسجل «statement». | _Order 14. Domain and range applications that conclude this fact._ |
| Holds | صحيح إذا تحقق أنّ واحد على الأقل من الشروط الآتية متحقق: علامة «holds before typing» مفعّلة أو حقل «count of type firings» أكبر من 0. | _Order 15. True when in effect or concluded by an enabled rule; the graph draws it._ |
| Is Conclusion | صحيح إذا تحقق أنّ جميع الشروط الآتية متحققة: علامة «holds» مفعّلة وعلامة «in effect» غير مفعّلة. | _Order 16. Holds only because a rule concluded it. Drawn dashed._ |
| Holds Unit | يتحدد وفق الأولوية: 1 إذا تحقق أنّ علامة «holds» مفعّلة؛ وفي سائر الحالات 0. | _Order 16. 1 when the fact holds._ |
| Conclusion Unit | يتحدد وفق الأولوية: 1 إذا تحقق أنّ علامة «conclusion» مفعّلة؛ وفي سائر الحالات 0. | _Order 17. 1 when the fact is a conclusion._ |
| Parent Unit | يتحدد وفق الأولوية: 1 إذا تحقق أنّ جميع الشروط الآتية متحققة: علامة «holds» مفعّلة وحقل «property» يساوي «isParentOf»؛ وفي سائر الحالات 0. | _Order 16. 1 for a holding parent link._ |
| Grandparent Unit | يتحدد وفق الأولوية: 1 إذا تحقق أنّ جميع الشروط الآتية متحققة: علامة «holds» مفعّلة وحقل «property» يساوي «isGrandparentOf»؛ وفي سائر الحالات 0. | _Order 16. 1 for a holding grandparent link._ |
| Place Unit | يتحدد وفق الأولوية: 1 إذا تحقق أنّ جميع الشروط الآتية متحققة: علامة «holds» مفعّلة وحقل «property» يساوي «attends»؛ وفي سائر الحالات 0. | _Order 16. 1 for a holding attends link._ |
| First Fired Order Key | أصغر قيم حقل «order key» عبر سجلات «firings» المرتبطة بسجل «statement» التي تحمل علامة «fired unit». | _Order 15. Order key of the first rule application that concludes this fact, in the page reasoner's order._ |
| Count of Chosen Firings | مجموع قيم حقل «chosen unit» عبر سجلات «firings» المرتبطة بسجل «statement». | _Order 20. Rule applications shown as this fact's conclusion card: one for a conclusion, zero otherwise._ |

<details open>
<summary>بيانات توضيحية</summary>

| Object Value | Is Listed | Is Built in | Is Detail | Is Stated | Activation Seq | Sort Order | Subject Label ƒ | Property Label ƒ | Object Label ƒ | Property Datatype ƒ | Property Has One Current Value ƒ | Property Domain Class ƒ | Property Range Class ƒ | Literal Quote ƒ | Literal Type Marker ƒ | Built in Source Text ƒ | User Source Text ƒ | Next Fact Sort Order ƒ | Is Literal ƒ | Object Text ƒ | Name ƒ | Words ƒ | Source ƒ | Subject Property Key ƒ | Latest Sibling Activation Seq ƒ | Is in Effect ƒ | Listed Unit ƒ | In Effect Unit ƒ | Listed Off Unit ƒ | Is Next to Add ƒ | Count of Inverse Firings ƒ | Count of Symmetric Firings ƒ | Holds Before Chain ƒ | Count of Chain Firings ƒ | Holds Before Typing ƒ | Count of Type Firings ƒ | Holds ƒ | Is Conclusion ƒ | Holds Unit ƒ | Conclusion Unit ƒ | Parent Unit ƒ | Grandparent Unit ƒ | Place Unit ƒ | First Fired Order Key ƒ | Count of Chosen Firings ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| — | true | true | false | true | 1 | 1 | Marvin | is parent of | Randy | — | false | — | — | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | — | false | Randy | Marvin is parent of Randy | Marvin is parent of Randy | Stated as a teaching example on this page. | Marvin\|isParentOf | 1 | true | 1 | 1 | 0 | false | 0 | 0 | true | 0 | true | 0 | true | false | 1 | 0 | 1 | 0 | 0 | — | 0 |
| — | false | false | false | false | 0 | 101 | Marvin | is parent of | Sharon | — | false | — | — | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | — | false | Sharon | Marvin is parent of Sharon | Marvin is parent of Sharon | Stated by you on this page. | Marvin\|isParentOf | 1 | false | 0 | 0 | 0 | false | 0 | 0 | false | 0 | false | 0 | false | false | 0 | 0 | 0 | 0 | 0 | — | 0 |
| — | false | false | false | false | 0 | 102 | Marvin | is parent of | Stan | — | false | — | — | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | — | false | Stan | Marvin is parent of Stan | Marvin is parent of Stan | Stated by you on this page. | Marvin\|isParentOf | 1 | false | 0 | 0 | 0 | false | 0 | 0 | false | 0 | false | 0 | false | false | 0 | 0 | 0 | 0 | 0 | — | 0 |

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
| Second Sort Order | مأخوذ من السجل المرتبط عبر حقل «second». | _Order 1. The second premise's list position._ |
| Inverse Fired Unit | يتحدد وفق الأولوية: 1 إذا تحقق أنّ جميع الشروط الآتية متحققة: علامة «rule enabled» مفعّلة، وحقل «kind» يساوي «inverse»، وعلامة «first is in effect» مفعّلة؛ وفي سائر الحالات 0. | _Order 5. 1 when the inverse rule fires here._ |
| Symmetric Fired Unit | يتحدد وفق الأولوية: 1 إذا تحقق أنّ جميع الشروط الآتية متحققة: علامة «rule enabled» مفعّلة، وحقل «kind» يساوي «symmetric»، وعلامة «first is in effect» مفعّلة؛ وفي سائر الحالات 0. | _Order 5. 1 when the symmetric rule fires here._ |
| Chain Fired Unit | يتحدد وفق الأولوية: 1 إذا تحقق أنّ جميع الشروط الآتية متحققة: علامة «rule enabled» مفعّلة، وحقل «kind» يساوي «chain»، وعلامة «first holds before chain» مفعّلة، وعلامة «second holds before chain» مفعّلة؛ وفي سائر الحالات 0. | _Order 9. 1 when the property chain fires here._ |
| Type Fired Unit | يتحدد وفق الأولوية: 1 إذا تحقق أنّ جميع الشروط الآتية متحققة: علامة «rule enabled» مفعّلة؛ وواحد على الأقل من الشروط الآتية متحقق: حقل «kind» يساوي «domain» أو حقل «kind» يساوي «range»؛ وعلامة «first holds before typing» مفعّلة؛ وفي سائر الحالات 0. | _Order 13. 1 when the domain or range rule fires here._ |
| Fired Unit | يُحسب على النحو الآتي: حقل «inverse fired unit» زائد حقل «symmetric fired unit» زائد حقل «chain fired unit» زائد حقل «type fired unit». | _Order 14. 1 when this application fires._ |
| Order Key | يُحسب على النحو الآتي: حقل «first sort order» مضروبًا في 100000 زائد حقل «kind sort order» مضروبًا في 1000 زائد 0 إذا تحقق أنّ حقل «second» فارغ، وفي سائر الحالات حقل «second sort order». | _Order 2. The page reasoner's order: by premise, then rule kind, then second premise._ |
| Is Chosen | صحيح إذا تحقق أنّ جميع الشروط الآتية متحققة: حقل «fired unit» يساوي 1، وعلامة «target is conclusion» مفعّلة، وحقل «order key» يساوي حقل «target first fired order key». | _Order 18. The application the conclusions panel shows for its fact: the first one in the reasoner's order._ |
| Chosen Unit | يتحدد وفق الأولوية: 1 إذا تحقق أنّ علامة «chosen» مفعّلة؛ وفي سائر الحالات 0. | _Order 19. 1 when this application is shown._ |
| Card Text | يتحدد وفق الأولوية: «Worked out by the »، ثم حقل «kind wording»، ثم « from “»، ثم حقل «first words»، ثم «”»، ثم « and “»، ثم حقل «second words»، ثم «”» إذا تحقق أنّ حقل «kind» يساوي «chain»، وفي سائر الحالات « and “»، ثم حقل «first property»، ثم « has domain »، ثم حقل «first property domain class»، ثم «”» إذا تحقق أنّ حقل «kind» يساوي «domain»، وفي سائر الحالات « and “»، ثم حقل «first property»، ثم « has range »، ثم حقل «first property range class»، ثم «”»، ثم نقطة إذا تحقق أنّ علامة «chosen» مفعّلة؛ وفي سائر الحالات نص فارغ. ⚠︎ آلي <!-- rulespeak:reword --> | _Order 19. The conclusion card's provenance line, in the page's wording._ |
| Name | يطابق حقل «firing ID» في السجل نفسه. | _Order 1. The application's key._ |

<details open>
<summary>بيانات توضيحية</summary>

| Rule Enabled ƒ | Kind Wording ƒ | Kind Sort Order ƒ | Target Name ƒ | Target Subject Label ƒ | Target Property Label ƒ | Target Object Text ƒ | Target is Conclusion ƒ | Target First Fired Order Key ƒ | First Words ƒ | Second Words ƒ | First is in Effect ƒ | First Holds Before Chain ƒ | Second Holds Before Chain ƒ | First Holds Before Typing ƒ | First Property ƒ | First Property Domain Class ƒ | First Property Range Class ƒ | First Sort Order ƒ | Second Sort Order ƒ | Inverse Fired Unit ƒ | Symmetric Fired Unit ƒ | Chain Fired Unit ƒ | Type Fired Unit ƒ | Fired Unit ƒ | Order Key ƒ | Is Chosen ƒ | Chosen Unit ƒ | Card Text ƒ | Name ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| false | inverse rule | 1 | Randy is child of Marvin | Randy | is child of | Marvin | false | — | Marvin is parent of Randy |    | true | true | false | true | isParentOf | — | — | 1 | — | 0 | 0 | 0 | 0 | 0 | 101000 | false | 0 | — | inverse:Marvin\|isParentOf\|Randy>Randy\|isChildOf\|Marvin |
| false | inverse rule | 1 | Sharon is child of Marvin | Sharon | is child of | Marvin | false | — | Marvin is parent of Sharon |    | false | false | false | false | isParentOf | — | — | 101 | — | 0 | 0 | 0 | 0 | 0 | 10101000 | false | 0 | — | inverse:Marvin\|isParentOf\|Sharon>Sharon\|isChildOf\|Marvin |
| false | inverse rule | 1 | Stan is child of Marvin | Stan | is child of | Marvin | false | — | Marvin is parent of Stan |    | false | false | false | false | isParentOf | — | — | 102 | — | 0 | 0 | 0 | 0 | 0 | 10201000 | false | 0 | — | inverse:Marvin\|isParentOf\|Stan>Stan\|isChildOf\|Marvin |

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
| false | true | false | Marvin | Randy | 0 | — | Marvin\|isParentOf\|Randy~isSpouseOf |
| false | false | false | Marvin | Sharon | 0 | — | Marvin\|isParentOf\|Sharon~isSpouseOf |
| false | false | false | Marvin | Stan | 0 | — | Marvin\|isParentOf\|Stan~isSpouseOf |

_ƒ تدل على عمود محسوب._

</details>

## 2 أنواع الحقائق

- كل سجل «**firing kind**» يشير إلى سجل «**rule**» واحد فقط
- كل سجل «**property**» يجوز أن يشير إلى سجل «**property**» واحد
- كل سجل «**property**» يجوز أن يشير إلى سجل «**thing**» واحد
- كل سجل «**statement**» يشير إلى سجل «**graph**» واحد فقط
- كل سجل «**statement**» يشير إلى سجل «**thing**» واحد فقط
- كل سجل «**statement**» يشير إلى سجل «**property**» واحد فقط
- كل سجل «**firing**» يشير إلى سجل «**firing kind**» واحد فقط
- كل سجل «**firing**» يشير إلى سجل «**rule**» واحد فقط
- كل سجل «**firing**» يشير إلى سجل «**statement**» واحد فقط
- كل سجل «**disjoint check**» يشير إلى سجل «**graph**» واحد فقط
- كل سجل «**disjoint check**» يشير إلى سجل «**rule**» واحد فقط
- كل سجل «**disjoint check**» يشير إلى سجل «**statement**» واحد فقط

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
- في كل سجل «property»، **يجب أن** تُحدّد قيمة حقل «label»، وحقل «property kind»، وحقل «sort order»، وأن يُسجّل ما إذا كان مصنّفًا على أنه «user addable»، وما إذا كان حائزًا على «one current value»، وما إذا كان مصنّفًا على أنه «symmetric».
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
| **DR-14 Name** | حقل «name» في سجل «rule» يطابق حقل «label» في السجل نفسه. |
| **DR-15 Count of Firings** | حقل «count of firings» في سجل «rule» هو مجموع قيم حقل «fired unit» عبر سجلات «firings» المرتبطة بسجل «rule». |
| **DR-16 Name** | حقل «name» في سجل «firing kind» يطابق حقل «wording» في السجل نفسه. |
| **DR-17 Rule Enabled** | حقل «rule enabled» في سجل «firing kind» يكون صحيحًا عندما يكون السجل المرتبط به عبر حقل «rule» مصنّفًا على أنه «enabled». |
| **DR-18 Name** | حقل «name» في سجل «thing» يطابق حقل «label» في السجل نفسه. |
| **DR-19 Count of Children** | حقل «count of children» في سجل «thing» هو مجموع قيم حقل «parent unit» عبر سجلات «statements» المرتبطة بسجل «thing». |
| **DR-20 Count of Grandchildren** | حقل «count of grandchildren» في سجل «thing» هو مجموع قيم حقل «grandparent unit» عبر سجلات «statements» المرتبطة بسجل «thing». |
| **DR-21 Count As Place** | حقل «count as place» في سجل «thing» هو مجموع قيم حقل «place unit» عبر سجلات «statements» المرتبطة بسجل «thing». |
| **DR-22 Count As Subject** | حقل «count as subject» في سجل «thing» هو مجموع قيم حقل «holds unit» عبر سجلات «statements» المرتبطة بسجل «thing». |
| **DR-23 Count As Object** | حقل «count as object» في سجل «thing» هو مجموع قيم حقل «holds unit» عبر سجلات «statements» المرتبطة بسجل «thing». |
| **DR-24 Is in Graph** | يُصنّف سجل «thing» على أنه «in graph» إذا تحقق أنّ واحد على الأقل من الشروط الآتية متحقق: حقل «count as subject» أكبر من 0 أو حقل «count as object» أكبر من 0. |
| **DR-25 Node Kind** | حقل «node kind» في سجل «thing» يتحدد وفق ترتيب الأولوية الآتي:<br>1. «class»، إذا تحقق أنّ حقل «kind» يساوي «class»؛<br>2. «place»، إذا تحقق أنّ حقل «count as place» أكبر من 0؛<br>3. «person»، في سائر الحالات. |
| **DR-26 Name** | حقل «name» في سجل «property» يطابق حقل «label» في السجل نفسه. |
| **DR-27 Subject Label** | حقل «subject label» في سجل «statement» مأخوذ من السجل المرتبط به عبر حقل «subject». |
| **DR-28 Property Label** | حقل «property label» في سجل «statement» مأخوذ من السجل المرتبط به عبر حقل «property». |
| **DR-29 Object Label** | حقل «object label» في سجل «statement» مأخوذ من السجل المرتبط به عبر حقل «object thing». |
| **DR-30 Property Datatype** | حقل «property datatype» في سجل «statement» مأخوذ من السجل المرتبط به عبر حقل «property». |
| **DR-31 Property Has One Current Value** | حقل «property has one current value» في سجل «statement» يكون صحيحًا عندما يكون السجل المرتبط به عبر حقل «property» حائزًا على «one current value». |
| **DR-32 Property Domain Class** | حقل «property domain class» في سجل «statement» مأخوذ من السجل المرتبط به عبر حقل «property». |
| **DR-33 Property Range Class** | حقل «property range class» في سجل «statement» مأخوذ من السجل المرتبط به عبر حقل «property». |
| **DR-34 Literal Quote** | حقل «literal quote» في سجل «statement» مأخوذ من السجل المرتبط به عبر حقل «graph». |
| **DR-35 Literal Type Marker** | حقل «literal type marker» في سجل «statement» مأخوذ من السجل المرتبط به عبر حقل «graph». |
| **DR-36 Built in Source Text** | حقل «built in source text» في سجل «statement» مأخوذ من السجل المرتبط به عبر حقل «graph». |
| **DR-37 User Source Text** | حقل «user source text» في سجل «statement» مأخوذ من السجل المرتبط به عبر حقل «graph». |
| **DR-38 Next Fact Sort Order** | حقل «next fact sort order» في سجل «statement» مأخوذ من السجل المرتبط به عبر حقل «graph». |
| **DR-39 Is Literal** | يُصنّف سجل «statement» على أنه «literal» إذا تحقق أنّ حقل «object thing» فارغ. |
| **DR-40 Object Text** | حقل «object text» في سجل «statement» يتحدد وفق ترتيب الأولوية الآتي:<br>1. حقل «object value»، إذا تحقق أنّ حقل «object thing» فارغ؛<br>2. حقل «object label»، في سائر الحالات. |
| **DR-41 Name** | حقل «name» في سجل «statement» يُحسب على النحو الآتي: حقل «subject label»، ثم مسافة، ثم حقل «property label»، ثم مسافة، ثم حقل «object text». |
| **DR-42 Words** | حقل «words» في سجل «statement» يُحسب على النحو الآتي: حقل «subject label»، ثم مسافة، ثم حقل «property label»، ثم مسافة، ثم حقل «literal quote»، ثم حقل «object value»، ثم حقل «literal quote»، ثم حقل «literal type marker»، ثم حقل «property datatype» إذا تحقق أنّ حقل «object thing» فارغ، وفي سائر الحالات حقل «object label». ⚠︎ آلي <!-- rulespeak:reword --> |
| **DR-43 Source** | حقل «source» في سجل «statement» يتحدد وفق ترتيب الأولوية الآتي:<br>1. حقل «built in source text»، إذا تحقق أنّ علامة «built in» مفعّلة؛<br>2. حقل «user source text»، في سائر الحالات. |
| **DR-44 Subject Property Key** | حقل «subject property key» في سجل «statement» يُحسب على النحو الآتي: حقل «subject»، ثم «|»، ثم حقل «property». |
| **DR-45 Latest Sibling Activation Seq** | حقل «latest sibling activation seq» في سجل «statement» هو أكبر قيم حقل «activation seq» عبر سجلات «statements» المرتبطة بسجل «statement». |
| **DR-46 Is in Effect** | يُصنّف سجل «statement» على أنه «in effect» إذا تحقق أنّ جميع الشروط الآتية متحققة: علامة «stated» مفعّلة؛ وواحد على الأقل من الشروط الآتية متحقق: علامة «property has one current value» غير مفعّلة أو حقل «activation seq» يساوي حقل «latest sibling activation seq». |
| **DR-47 Listed Unit** | حقل «listed unit» في سجل «statement» يتحدد وفق ترتيب الأولوية الآتي:<br>1. 1، إذا تحقق أنّ علامة «listed» مفعّلة؛<br>2. 0، في سائر الحالات. |
| **DR-48 In Effect Unit** | حقل «in effect unit» في سجل «statement» يتحدد وفق ترتيب الأولوية الآتي:<br>1. 1، إذا تحقق أنّ علامة «in effect» مفعّلة؛<br>2. 0، في سائر الحالات. |
| **DR-49 Listed Off Unit** | حقل «listed off unit» في سجل «statement» يتحدد وفق ترتيب الأولوية الآتي:<br>1. 1، إذا تحقق أنّ جميع الشروط الآتية متحققة: علامة «listed» مفعّلة وعلامة «in effect» غير مفعّلة؛<br>2. 0، في سائر الحالات. |
| **DR-50 Is Next to Add** | يُصنّف سجل «statement» على أنه «next to add» إذا تحقق أنّ جميع الشروط الآتية متحققة: حقل «listed off unit» يساوي 1 وحقل «sort order» يساوي حقل «next fact sort order». |
| **DR-51 Count of Inverse Firings** | حقل «count of inverse firings» في سجل «statement» هو مجموع قيم حقل «inverse fired unit» عبر سجلات «firings» المرتبطة بسجل «statement». |
| **DR-52 Count of Symmetric Firings** | حقل «count of symmetric firings» في سجل «statement» هو مجموع قيم حقل «symmetric fired unit» عبر سجلات «firings» المرتبطة بسجل «statement». |
| **DR-53 Holds Before Chain** | يُعدّ سجل «statement» محققًا لشرط «holds before chain» إذا تحقق أنّ واحد على الأقل من الشروط الآتية متحقق: علامة «in effect» مفعّلة، أو حقل «count of inverse firings» أكبر من 0، أو حقل «count of symmetric firings» أكبر من 0. |
| **DR-54 Count of Chain Firings** | حقل «count of chain firings» في سجل «statement» هو مجموع قيم حقل «chain fired unit» عبر سجلات «firings» المرتبطة بسجل «statement». |
| **DR-55 Holds Before Typing** | يُعدّ سجل «statement» محققًا لشرط «holds before typing» إذا تحقق أنّ واحد على الأقل من الشروط الآتية متحقق: علامة «holds before chain» مفعّلة أو حقل «count of chain firings» أكبر من 0. |
| **DR-56 Count of Type Firings** | حقل «count of type firings» في سجل «statement» هو مجموع قيم حقل «type fired unit» عبر سجلات «firings» المرتبطة بسجل «statement». |
| **DR-57 Holds** | يُعدّ سجل «statement» محققًا لشرط «holds» إذا تحقق أنّ واحد على الأقل من الشروط الآتية متحقق: علامة «holds before typing» مفعّلة أو حقل «count of type firings» أكبر من 0. |
| **DR-58 Is Conclusion** | يُصنّف سجل «statement» على أنه «conclusion» إذا تحقق أنّ جميع الشروط الآتية متحققة: علامة «holds» مفعّلة وعلامة «in effect» غير مفعّلة. |
| **DR-59 Holds Unit** | حقل «holds unit» في سجل «statement» يتحدد وفق ترتيب الأولوية الآتي:<br>1. 1، إذا تحقق أنّ علامة «holds» مفعّلة؛<br>2. 0، في سائر الحالات. |
| **DR-60 Conclusion Unit** | حقل «conclusion unit» في سجل «statement» يتحدد وفق ترتيب الأولوية الآتي:<br>1. 1، إذا تحقق أنّ علامة «conclusion» مفعّلة؛<br>2. 0، في سائر الحالات. |
| **DR-61 Parent Unit** | حقل «parent unit» في سجل «statement» يتحدد وفق ترتيب الأولوية الآتي:<br>1. 1، إذا تحقق أنّ جميع الشروط الآتية متحققة: علامة «holds» مفعّلة وحقل «property» يساوي «isParentOf»؛<br>2. 0، في سائر الحالات. |
| **DR-62 Grandparent Unit** | حقل «grandparent unit» في سجل «statement» يتحدد وفق ترتيب الأولوية الآتي:<br>1. 1، إذا تحقق أنّ جميع الشروط الآتية متحققة: علامة «holds» مفعّلة وحقل «property» يساوي «isGrandparentOf»؛<br>2. 0، في سائر الحالات. |
| **DR-63 Place Unit** | حقل «place unit» في سجل «statement» يتحدد وفق ترتيب الأولوية الآتي:<br>1. 1، إذا تحقق أنّ جميع الشروط الآتية متحققة: علامة «holds» مفعّلة وحقل «property» يساوي «attends»؛<br>2. 0، في سائر الحالات. |
| **DR-64 First Fired Order Key** | حقل «first fired order key» في سجل «statement» هو أصغر قيم حقل «order key» عبر سجلات «firings» المرتبطة بسجل «statement» التي تحمل علامة «fired unit». |
| **DR-65 Count of Chosen Firings** | حقل «count of chosen firings» في سجل «statement» هو مجموع قيم حقل «chosen unit» عبر سجلات «firings» المرتبطة بسجل «statement». |
| **DR-66 Rule Enabled** | حقل «rule enabled» في سجل «firing» يكون صحيحًا عندما يكون السجل المرتبط به عبر حقل «rule» مصنّفًا على أنه «enabled». |
| **DR-67 Kind Wording** | حقل «kind wording» في سجل «firing» مأخوذ من السجل المرتبط به عبر حقل «kind». |
| **DR-68 Kind Sort Order** | حقل «kind sort order» في سجل «firing» مأخوذ من السجل المرتبط به عبر حقل «kind». |
| **DR-69 Target Name** | حقل «target name» في سجل «firing» مأخوذ من السجل المرتبط به عبر حقل «target». |
| **DR-70 Target Subject Label** | حقل «target subject label» في سجل «firing» مأخوذ من السجل المرتبط به عبر حقل «target». |
| **DR-71 Target Property Label** | حقل «target property label» في سجل «firing» مأخوذ من السجل المرتبط به عبر حقل «target». |
| **DR-72 Target Object Text** | حقل «target object text» في سجل «firing» مأخوذ من السجل المرتبط به عبر حقل «target». |
| **DR-73 Target is Conclusion** | حقل «target is conclusion» في سجل «firing» يكون صحيحًا عندما يكون السجل المرتبط به عبر حقل «target» مصنّفًا على أنه «conclusion». |
| **DR-74 Target First Fired Order Key** | حقل «target first fired order key» في سجل «firing» مأخوذ من السجل المرتبط به عبر حقل «target». |
| **DR-75 First Words** | حقل «first words» في سجل «firing» مأخوذ من السجل المرتبط به عبر حقل «first». |
| **DR-76 Second Words** | حقل «second words» في سجل «firing» مأخوذ من السجل المرتبط به عبر حقل «second». |
| **DR-77 First is in Effect** | حقل «first is in effect» في سجل «firing» يكون صحيحًا عندما يكون السجل المرتبط به عبر حقل «first» مصنّفًا على أنه «in effect». |
| **DR-78 First Holds Before Chain** | حقل «first holds before chain» في سجل «firing» يكون صحيحًا عندما يكون السجل المرتبط به عبر حقل «first» موسومًا بعلامة «holds before chain». |
| **DR-79 Second Holds Before Chain** | حقل «second holds before chain» في سجل «firing» يكون صحيحًا عندما يكون السجل المرتبط به عبر حقل «second» موسومًا بعلامة «holds before chain». |
| **DR-80 First Holds Before Typing** | حقل «first holds before typing» في سجل «firing» يكون صحيحًا عندما يكون السجل المرتبط به عبر حقل «first» موسومًا بعلامة «holds before typing». |
| **DR-81 First Property** | حقل «first property» في سجل «firing» مأخوذ من السجل المرتبط به عبر حقل «first». |
| **DR-82 First Property Domain Class** | حقل «first property domain class» في سجل «firing» مأخوذ من السجل المرتبط به عبر حقل «first». |
| **DR-83 First Property Range Class** | حقل «first property range class» في سجل «firing» مأخوذ من السجل المرتبط به عبر حقل «first». |
| **DR-84 First Sort Order** | حقل «first sort order» في سجل «firing» مأخوذ من السجل المرتبط به عبر حقل «first». |
| **DR-85 Second Sort Order** | حقل «second sort order» في سجل «firing» مأخوذ من السجل المرتبط به عبر حقل «second». |
| **DR-86 Inverse Fired Unit** | حقل «inverse fired unit» في سجل «firing» يتحدد وفق ترتيب الأولوية الآتي:<br>1. 1، إذا تحقق أنّ جميع الشروط الآتية متحققة: علامة «rule enabled» مفعّلة، وحقل «kind» يساوي «inverse»، وعلامة «first is in effect» مفعّلة؛<br>2. 0، في سائر الحالات. |
| **DR-87 Symmetric Fired Unit** | حقل «symmetric fired unit» في سجل «firing» يتحدد وفق ترتيب الأولوية الآتي:<br>1. 1، إذا تحقق أنّ جميع الشروط الآتية متحققة: علامة «rule enabled» مفعّلة، وحقل «kind» يساوي «symmetric»، وعلامة «first is in effect» مفعّلة؛<br>2. 0، في سائر الحالات. |
| **DR-88 Chain Fired Unit** | حقل «chain fired unit» في سجل «firing» يتحدد وفق ترتيب الأولوية الآتي:<br>1. 1، إذا تحقق أنّ جميع الشروط الآتية متحققة: علامة «rule enabled» مفعّلة، وحقل «kind» يساوي «chain»، وعلامة «first holds before chain» مفعّلة، وعلامة «second holds before chain» مفعّلة؛<br>2. 0، في سائر الحالات. |
| **DR-89 Type Fired Unit** | حقل «type fired unit» في سجل «firing» يتحدد وفق ترتيب الأولوية الآتي:<br>1. 1، إذا تحقق أنّ جميع الشروط الآتية متحققة: علامة «rule enabled» مفعّلة؛ وواحد على الأقل من الشروط الآتية متحقق: حقل «kind» يساوي «domain» أو حقل «kind» يساوي «range»؛ وعلامة «first holds before typing» مفعّلة؛<br>2. 0، في سائر الحالات. |
| **DR-90 Fired Unit** | حقل «fired unit» في سجل «firing» يُحسب على النحو الآتي: حقل «inverse fired unit» زائد حقل «symmetric fired unit» زائد حقل «chain fired unit» زائد حقل «type fired unit». |
| **DR-91 Order Key** | حقل «order key» في سجل «firing» يُحسب على النحو الآتي: حقل «first sort order» مضروبًا في 100000 زائد حقل «kind sort order» مضروبًا في 1000 زائد 0 إذا تحقق أنّ حقل «second» فارغ، وفي سائر الحالات حقل «second sort order». |
| **DR-92 Is Chosen** | يُصنّف سجل «firing» على أنه «chosen» إذا تحقق أنّ جميع الشروط الآتية متحققة: حقل «fired unit» يساوي 1، وعلامة «target is conclusion» مفعّلة، وحقل «order key» يساوي حقل «target first fired order key». |
| **DR-93 Chosen Unit** | حقل «chosen unit» في سجل «firing» يتحدد وفق ترتيب الأولوية الآتي:<br>1. 1، إذا تحقق أنّ علامة «chosen» مفعّلة؛<br>2. 0، في سائر الحالات. |
| **DR-94 Card Text** | حقل «card text» في سجل «firing» يتحدد وفق ترتيب الأولوية الآتي:<br>1. «Worked out by the »، ثم حقل «kind wording»، ثم « from “»، ثم حقل «first words»، ثم «”»، ثم « and “»، ثم حقل «second words»، ثم «”» إذا تحقق أنّ حقل «kind» يساوي «chain»، وفي سائر الحالات « and “»، ثم حقل «first property»، ثم « has domain »، ثم حقل «first property domain class»، ثم «”» إذا تحقق أنّ حقل «kind» يساوي «domain»، وفي سائر الحالات « and “»، ثم حقل «first property»، ثم « has range »، ثم حقل «first property range class»، ثم «”»، ثم نقطة، إذا تحقق أنّ علامة «chosen» مفعّلة؛<br>2. نص فارغ، في سائر الحالات. ⚠︎ آلي <!-- rulespeak:reword --> |
| **DR-95 Name** | حقل «name» في سجل «firing» يطابق حقل «firing ID» في السجل نفسه. |
| **DR-96 Rule Enabled** | حقل «rule enabled» في سجل «disjoint check» يكون صحيحًا عندما يكون السجل المرتبط به عبر حقل «rule» مصنّفًا على أنه «enabled». |
| **DR-97 First Holds** | حقل «first holds» في سجل «disjoint check» يكون صحيحًا عندما يكون السجل المرتبط به عبر حقل «first» موسومًا بعلامة «holds». |
| **DR-98 Second Holds** | حقل «second holds» في سجل «disjoint check» يكون صحيحًا عندما يكون السجل المرتبط به عبر حقل «second» موسومًا بعلامة «holds». |
| **DR-99 Subject Label** | حقل «subject label» في سجل «disjoint check» مأخوذ من السجل المرتبط به عبر حقل «first». |
| **DR-100 Object Label** | حقل «object label» في سجل «disjoint check» مأخوذ من السجل المرتبط به عبر حقل «first». |
| **DR-101 Violation Unit** | حقل «violation unit» في سجل «disjoint check» يتحدد وفق ترتيب الأولوية الآتي:<br>1. 1، إذا تحقق أنّ جميع الشروط الآتية متحققة: علامة «rule enabled» مفعّلة، وعلامة «first holds» مفعّلة، وعلامة «second holds» مفعّلة؛<br>2. 0، في سائر الحالات. |
| **DR-102 Alarm Text** | حقل «alarm text» في سجل «disjoint check» يتحدد وفق ترتيب الأولوية الآتي:<br>1. «Contradiction found. »، ثم حقل «subject label»، ثم « is recorded as both parent and spouse of »، ثم حقل «object label»، ثم «. Check those facts and the selected disjoint rule.»، إذا تحقق أنّ حقل «violation unit» يساوي 1؛<br>2. نص فارغ، في سائر الحالات. |
| **DR-103 Name** | حقل «name» في سجل «disjoint check» يطابق حقل «disjoint check ID» في السجل نفسه. |

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
| **Rules.Name** | صيغة | `Label` |
| **Rules.CountOfFirings** | تجميع | `Sum(Firings.FiredUnit via Rule)` |
| **FiringKinds.Name** | صيغة | `Wording` |
| **FiringKinds.RuleEnabled** | بحث | `Lookup(Rules.IsEnabled via Rule)` |
| **Things.Name** | صيغة | `Label` |
| **Things.CountOfChildren** | تجميع | `Sum(Statements.ParentUnit via Subject)` |
| **Things.CountOfGrandchildren** | تجميع | `Sum(Statements.GrandparentUnit via Subject)` |
| **Things.CountAsPlace** | تجميع | `Sum(Statements.PlaceUnit via ObjectThing)` |
| **Things.CountAsSubject** | تجميع | `Sum(Statements.HoldsUnit via Subject)` |
| **Things.CountAsObject** | تجميع | `Sum(Statements.HoldsUnit via ObjectThing)` |
| **Things.IsInGraph** | صيغة | `Or(CountAsSubject > 0, CountAsObject > 0)` |
| **Things.NodeKind** | صيغة | `If(Kind = "class", "class", If(CountAsPlace > 0, "place", "person"))` |
| **Properties.Name** | صيغة | `Label` |
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
| **Statements.HoldsBeforeChain** | صيغة | `Or(IsInEffect, CountOfInverseFirings > 0, CountOfSymmetricFirings > 0)` |
| **Statements.CountOfChainFirings** | تجميع | `Sum(Firings.ChainFiredUnit via Target)` |
| **Statements.HoldsBeforeTyping** | صيغة | `Or(HoldsBeforeChain, CountOfChainFirings > 0)` |
| **Statements.CountOfTypeFirings** | تجميع | `Sum(Firings.TypeFiredUnit via Target)` |
| **Statements.Holds** | صيغة | `Or(HoldsBeforeTyping, CountOfTypeFirings > 0)` |
| **Statements.IsConclusion** | صيغة | `And(Holds, Not(IsInEffect))` |
| **Statements.HoldsUnit** | صيغة | `If(Holds, 1, 0)` |
| **Statements.ConclusionUnit** | صيغة | `If(IsConclusion, 1, 0)` |
| **Statements.ParentUnit** | صيغة | `If(And(Holds, Property = "isParentOf"), 1, 0)` |
| **Statements.GrandparentUnit** | صيغة | `If(And(Holds, Property = "isGrandparentOf"), 1, 0)` |
| **Statements.PlaceUnit** | صيغة | `If(And(Holds, Property = "attends"), 1, 0)` |
| **Statements.FirstFiredOrderKey** | تجميع | `Min(Firings.OrderKey via Target)` |
| **Statements.CountOfChosenFirings** | تجميع | `Sum(Firings.ChosenUnit via Target)` |
| **Firings.RuleEnabled** | بحث | `Lookup(Rules.IsEnabled via Rule)` |
| **Firings.KindWording** | بحث | `Lookup(FiringKinds.Wording via Kind)` |
| **Firings.KindSortOrder** | بحث | `Lookup(FiringKinds.SortOrder via Kind)` |
| **Firings.TargetName** | بحث | `Lookup(Statements.Name via Target)` |
| **Firings.TargetSubjectLabel** | بحث | `Lookup(Statements.SubjectLabel via Target)` |
| **Firings.TargetPropertyLabel** | بحث | `Lookup(Statements.PropertyLabel via Target)` |
| **Firings.TargetObjectText** | بحث | `Lookup(Statements.ObjectText via Target)` |
| **Firings.TargetIsConclusion** | بحث | `Lookup(Statements.IsConclusion via Target)` |
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
| **Firings.SecondSortOrder** | بحث | `Lookup(Statements.SortOrder via Second)` |
| **Firings.InverseFiredUnit** | صيغة | `If(And(RuleEnabled, Kind = "inverse", FirstIsInEffect), 1, 0)` |
| **Firings.SymmetricFiredUnit** | صيغة | `If(And(RuleEnabled, Kind = "symmetric", FirstIsInEffect), 1, 0)` |
| **Firings.ChainFiredUnit** | صيغة | `If(And(RuleEnabled, Kind = "chain", FirstHoldsBeforeChain, SecondHoldsBeforeChain), 1, 0)` |
| **Firings.TypeFiredUnit** | صيغة | `If(And(RuleEnabled, Or(Kind = "domain", Kind = "range"), FirstHoldsBeforeTyping), 1, 0)` |
| **Firings.FiredUnit** | صيغة | `InverseFiredUnit + SymmetricFiredUnit + ChainFiredUnit + TypeFiredUnit` |
| **Firings.OrderKey** | صيغة | `FirstSortOrder * 100000 + KindSortOrder * 1000 + If(Second = "", 0, SecondSortOrder)` |
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

---

> **RuleSpeak®** — صيغة قواعد العمل التصريحية التي ابتكرها **Ronald G. Ross** وفق اصطلاحات **SBVR** — قادمة قريبًا. إلى ذلك الحين تعرض هذه الصفحة التوثيق المعتاد. [www.RonRoss.info](https://www.RonRoss.info)
