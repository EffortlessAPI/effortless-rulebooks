# 📘 South Park ontology explainer — التوثيق

_The barebones South Park ontology from Jessica Talisman's explainer: ten stated facts and five rules that can each be switched on. Conclusions are formulas._

> قواعد عمل تصريحية مستخرجة من دفتر القواعد. كل عبارة واردة أدناه تعبّر عن حقيقة
> في مجال العمل؛ فهي ليست إجراءً ولا أمرًا. صيغ دفتر القواعد هي المصدر الوحيد للحقيقة،
> وهذه الوثيقة قراءتها بلغة مبسّطة.

## 1 مفردات العمل

| المصطلح | الوصف | تعليق توضيحي |
|------|-------------|-------------------|
| **Graph** | The whole example. | — |
| Name | يطابق حقل «graph ID» في السجل نفسه. | — |
| Count of Stated Facts | مجموع قيم حقل «stated unit» عبر سجلات «statements» المرتبطة بسجل «graph». | — |
| Count of Conclusions | مجموع قيم حقل «conclusion unit» عبر سجلات «statements» المرتبطة بسجل «graph». | _Facts that hold but were never stated._ |
| Count of Violations | مجموع قيم حقل «violation unit» عبر سجلات «disjoint checks» المرتبطة بسجل «graph». | — |
| Is Consistent | صحيح إذا تحقق أنّ حقل «count of violations» يساوي 0. | — |

<details open>
<summary>بيانات توضيحية</summary>

| Name ƒ | Count of Stated Facts ƒ | Count of Conclusions ƒ | Count of Violations ƒ | Is Consistent ƒ |
|---|---|---|---|---|
| south-park | 10 | 8 | 0 | true |

_ƒ تدل على عمود محسوب._

</details>

| المصطلح | الوصف | تعليق توضيحي |
|------|-------------|-------------------|
| **Rule** | The five rules; IsEnabled is the checkbox. | — |
| Label | سمة معرّفة. | — |
| Explanation | سمة معرّفة. | — |
| Is Enabled | صحيح إذا تحقق أنّ نص فارغ. | — |
| Name | يطابق حقل «label» في السجل نفسه. | — |
| Count of Firings | مجموع قيم حقل «fired unit» عبر سجلات «firings» المرتبطة بسجل «rule». | _How many times the rule produced a fact._ |

<details open>
<summary>بيانات توضيحية</summary>

| Label | Explanation | Is Enabled | Name ƒ | Count of Firings ƒ |
|---|---|---|---|---|
| Inverse | A parent link also gives a child link between the same two people. | true | Inverse | 3 |
| Symmetric | A friend link or a spouse link is true for both people. | true | Symmetric | 2 |
| Property chain | A parent of a parent is a grandparent. | true | Property chain | 2 |

_ƒ تدل على عمود محسوب._

</details>

| المصطلح | الوصف | تعليق توضيحي |
|------|-------------|-------------------|
| **Thing** | A node in the picture: a person, a place, a class or a role value. | — |
| Label | سمة معرّفة. | — |
| Kind | سمة معرّفة. | — |
| Name | يطابق حقل «label» في السجل نفسه. | — |
| Count of Children | مجموع قيم حقل «parent unit» عبر سجلات «statements» المرتبطة بسجل «thing». | _Added: a count, which a reasoner alone does not give._ |
| Count of Grandchildren | مجموع قيم حقل «grandparent unit» عبر سجلات «statements» المرتبطة بسجل «thing». | _Added: counts concluded grandchildren._ |

<details open>
<summary>بيانات توضيحية</summary>

| Label | Kind | Name ƒ | Count of Children ƒ | Count of Grandchildren ƒ |
|---|---|---|---|---|
| Marvin | individual | Marvin | 1 | 2 |
| Randy | individual | Randy | 2 | 0 |
| Sharon | individual | Sharon | 0 | 0 |

_ƒ تدل على عمود محسوب._

</details>

| المصطلح | الوصف | تعليق توضيحي |
|------|-------------|-------------------|
| **Property** | A kind of link, with the characteristics the rules use. | — |
| Label | سمة معرّفة. | — |
| Inverse of | سمة معرّفة. | — |
| Is Symmetric | صحيح إذا تحقق أنّ نص فارغ. | — |
| Chain First | سمة معرّفة. | — |
| Chain Second | سمة معرّفة. | — |
| Domain Class | سمة معرّفة. | — |
| Range Class | سمة معرّفة. | — |
| Disjoint With | سمة معرّفة. | — |
| Name | يطابق حقل «label» في السجل نفسه. | — |

<details open>
<summary>بيانات توضيحية</summary>

| Label | Is Symmetric | Name ƒ |
|---|---|---|
| is parent of | false | is parent of |
| is child of | false | is child of |
| is spouse of | true | is spouse of |

_ƒ تدل على عمود محسوب._

</details>

| المصطلح | الوصف | تعليق توضيحي |
|------|-------------|-------------------|
| **Statement** | A fact that is stated, or could be concluded: Subject, Property, Object. | — |
| Graph | سمة معرّفة. | — |
| Subject | سمة معرّفة. | — |
| Property | سمة معرّفة. | — |
| Object Thing | سمة معرّفة. | — |
| Number Value | سمة معرّفة. | — |
| Is Stated | صحيح إذا تحقق أنّ نص فارغ. | _Drawn as a solid line in the explainer._ |
| Subject Label | مأخوذ من السجل المرتبط عبر حقل «subject». | — |
| Property Label | مأخوذ من السجل المرتبط عبر حقل «property». | — |
| Object Label | مأخوذ من السجل المرتبط عبر حقل «object thing». | — |
| Name | يُحسب على النحو الآتي: حقل «subject label»، ثم مسافة، ثم حقل «property label»، ثم مسافة، ثم حقل «number value» إذا تحقق أنّ حقل «object thing» فارغ، وفي سائر الحالات حقل «object label». | — |
| Count of Inverse Firings | مجموع قيم حقل «inverse fired unit» عبر سجلات «firings» المرتبطة بسجل «statement». | — |
| Count of Symmetric Firings | مجموع قيم حقل «symmetric fired unit» عبر سجلات «firings» المرتبطة بسجل «statement». | — |
| Holds Before Chain | صحيح إذا تحقق أنّ واحد على الأقل من الشروط الآتية متحقق: علامة «stated» مفعّلة، أو حقل «count of inverse firings» أكبر من 0، أو حقل «count of symmetric firings» أكبر من 0. | — |
| Count of Chain Firings | مجموع قيم حقل «chain fired unit» عبر سجلات «firings» المرتبطة بسجل «statement». | — |
| Holds Before Typing | صحيح إذا تحقق أنّ واحد على الأقل من الشروط الآتية متحقق: علامة «holds before chain» مفعّلة أو حقل «count of chain firings» أكبر من 0. | — |
| Count of Type Firings | مجموع قيم حقل «type fired unit» عبر سجلات «firings» المرتبطة بسجل «statement». | — |
| Holds | صحيح إذا تحقق أنّ واحد على الأقل من الشروط الآتية متحقق: علامة «holds before typing» مفعّلة أو حقل «count of type firings» أكبر من 0. | _True when stated or concluded by an enabled rule._ |
| Is Conclusion | صحيح إذا تحقق أنّ جميع الشروط الآتية متحققة: علامة «holds» مفعّلة وعلامة «stated» غير مفعّلة. | _Drawn as a dashed line in the explainer._ |
| Worked Out by | يتحدد وفق الأولوية: نص فارغ إذا تحقق أنّ علامة «conclusion» غير مفعّلة; «inverse» إذا تحقق أنّ حقل «count of inverse firings» أكبر من 0; «symmetric» إذا تحقق أنّ حقل «count of symmetric firings» أكبر من 0; «property chain» إذا تحقق أنّ حقل «count of chain firings» أكبر من 0؛ وفي سائر الحالات «domain and range». | — |
| Stated Unit | يتحدد وفق الأولوية: 1 إذا تحقق أنّ علامة «stated» مفعّلة؛ وفي سائر الحالات 0. | — |
| Conclusion Unit | يتحدد وفق الأولوية: 1 إذا تحقق أنّ علامة «conclusion» مفعّلة؛ وفي سائر الحالات 0. | — |
| Parent Unit | يتحدد وفق الأولوية: 1 إذا تحقق أنّ جميع الشروط الآتية متحققة: علامة «holds» مفعّلة وحقل «property» يساوي «isParentOf»؛ وفي سائر الحالات 0. | — |
| Grandparent Unit | يتحدد وفق الأولوية: 1 إذا تحقق أنّ جميع الشروط الآتية متحققة: علامة «holds» مفعّلة وحقل «property» يساوي «isGrandparentOf»؛ وفي سائر الحالات 0. | — |

<details open>
<summary>بيانات توضيحية</summary>

| Number Value | Is Stated | Subject Label ƒ | Property Label ƒ | Object Label ƒ | Name ƒ | Count of Inverse Firings ƒ | Count of Symmetric Firings ƒ | Holds Before Chain ƒ | Count of Chain Firings ƒ | Holds Before Typing ƒ | Count of Type Firings ƒ | Holds ƒ | Is Conclusion ƒ | Worked Out by ƒ | Stated Unit ƒ | Conclusion Unit ƒ | Parent Unit ƒ | Grandparent Unit ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| — | true | Marvin | is parent of | Randy | Marvin is parent of Randy | 0 | 0 | true | 0 | true | 0 | true | false | — | 1 | 0 | 1 | 0 |
| — | false | Marvin | is parent of | Sharon | Marvin is parent of Sharon | 0 | 0 | false | 0 | false | 0 | false | false | — | 0 | 0 | 0 | 0 |
| — | false | Marvin | is parent of | Shelly | Marvin is parent of Shelly | 0 | 0 | false | 0 | false | 0 | false | false | — | 0 | 0 | 0 | 0 |

_ƒ تدل على عمود محسوب._

</details>

| المصطلح | الوصف | تعليق توضيحي |
|------|-------------|-------------------|
| **Firing** | One way one rule could produce one fact from one or two others. | — |
| Rule | سمة معرّفة. | — |
| Kind | سمة معرّفة. | — |
| Target | سمة معرّفة. | — |
| First | سمة معرّفة. | — |
| Second | سمة معرّفة. | _Same as First when the rule needs one premise._ |
| Rule Enabled | صحيح عندما يكون السجل المرتبط عبر حقل «rule» مصنّفًا على أنه «enabled». | — |
| Rule Label | مأخوذ من السجل المرتبط عبر حقل «rule». | — |
| Target Name | مأخوذ من السجل المرتبط عبر حقل «target». | — |
| First Name | مأخوذ من السجل المرتبط عبر حقل «first». | — |
| Second Name | مأخوذ من السجل المرتبط عبر حقل «second». | — |
| First is Stated | صحيح عندما يكون السجل المرتبط عبر حقل «first» مصنّفًا على أنه «stated». | — |
| First Holds Before Chain | صحيح عندما يكون السجل المرتبط عبر حقل «first» موسومًا بعلامة «holds before chain». | — |
| Second Holds Before Chain | صحيح عندما يكون السجل المرتبط عبر حقل «second» موسومًا بعلامة «holds before chain». | — |
| First Holds Before Typing | صحيح عندما يكون السجل المرتبط عبر حقل «first» موسومًا بعلامة «holds before typing». | — |
| Inverse Fired Unit | يتحدد وفق الأولوية: 1 إذا تحقق أنّ جميع الشروط الآتية متحققة: علامة «rule enabled» مفعّلة، وحقل «kind» يساوي «inverse»، وعلامة «first is stated» مفعّلة؛ وفي سائر الحالات 0. | — |
| Symmetric Fired Unit | يتحدد وفق الأولوية: 1 إذا تحقق أنّ جميع الشروط الآتية متحققة: علامة «rule enabled» مفعّلة، وحقل «kind» يساوي «symmetric»، وعلامة «first is stated» مفعّلة؛ وفي سائر الحالات 0. | — |
| Chain Fired Unit | يتحدد وفق الأولوية: 1 إذا تحقق أنّ جميع الشروط الآتية متحققة: علامة «rule enabled» مفعّلة، وحقل «kind» يساوي «chain»، وعلامة «first holds before chain» مفعّلة، وعلامة «second holds before chain» مفعّلة؛ وفي سائر الحالات 0. | — |
| Type Fired Unit | يتحدد وفق الأولوية: 1 إذا تحقق أنّ جميع الشروط الآتية متحققة: علامة «rule enabled» مفعّلة؛ وواحد على الأقل من الشروط الآتية متحقق: حقل «kind» يساوي «domain» أو حقل «kind» يساوي «range»؛ وعلامة «first holds before typing» مفعّلة؛ وفي سائر الحالات 0. | — |
| Fired Unit | يُحسب على النحو الآتي: حقل «inverse fired unit» زائد حقل «symmetric fired unit» زائد حقل «chain fired unit» زائد حقل «type fired unit». | — |
| Name | يتحدد وفق الأولوية: حقل «target name»، ثم «: worked out by the »، ثم حقل «rule label»، ثم « rule from '»، ثم حقل «first name»، ثم «'»، ثم « and '»، ثم حقل «second name»، ثم «'» إذا تحقق أنّ حقل «fired unit» أكبر من 0؛ وفي سائر الحالات «firing ID». | _The explainer's conclusion card._ |

<details open>
<summary>بيانات توضيحية</summary>

| Kind | Rule Enabled ƒ | Rule Label ƒ | Target Name ƒ | First Name ƒ | Second Name ƒ | First is Stated ƒ | First Holds Before Chain ƒ | Second Holds Before Chain ƒ | First Holds Before Typing ƒ | Inverse Fired Unit ƒ | Symmetric Fired Unit ƒ | Chain Fired Unit ƒ | Type Fired Unit ƒ | Fired Unit ƒ | Name ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| inverse | true | Inverse | Randy is child of Marvin | Marvin is parent of Randy | Marvin is parent of Randy | true | true | true | true | 1 | 0 | 0 | 0 | 1 | Randy is child of Marvin: worked out by the Inverse rule from 'Marvin is parent of Randy' |
| inverse | true | Inverse | Sharon is child of Marvin | Marvin is parent of Sharon | Marvin is parent of Sharon | false | false | false | false | 0 | 0 | 0 | 0 | 0 | inverse:Marvin\|isParentOf\|Sharon>Sharon\|isChildOf\|Marvin |
| inverse | true | Inverse | Shelly is child of Marvin | Marvin is parent of Shelly | Marvin is parent of Shelly | false | false | false | false | 0 | 0 | 0 | 0 | 0 | inverse:Marvin\|isParentOf\|Shelly>Shelly\|isChildOf\|Marvin |

_ƒ تدل على عمود محسوب._

</details>

| المصطلح | الوصف | تعليق توضيحي |
|------|-------------|-------------------|
| **Disjoint Check** | Two statements that must not both hold. | — |
| Graph | سمة معرّفة. | — |
| Rule | سمة معرّفة. | — |
| A | سمة معرّفة. | — |
| B | سمة معرّفة. | — |
| Name | يطابق حقل «disjoint check ID» في السجل نفسه. | — |
| Rule Enabled | صحيح عندما يكون السجل المرتبط عبر حقل «rule» مصنّفًا على أنه «enabled». | — |
| A Holds | صحيح عندما يكون السجل المرتبط عبر حقل «a» موسومًا بعلامة «holds». | — |
| B Holds | صحيح عندما يكون السجل المرتبط عبر حقل «b» موسومًا بعلامة «holds». | — |
| Violation Unit | يتحدد وفق الأولوية: 1 إذا تحقق أنّ جميع الشروط الآتية متحققة: علامة «rule enabled» مفعّلة، وعلامة «a holds» مفعّلة، وعلامة «b holds» مفعّلة؛ وفي سائر الحالات 0. | — |

<details open>
<summary>بيانات توضيحية</summary>

| Name ƒ | Rule Enabled ƒ | A Holds ƒ | B Holds ƒ | Violation Unit ƒ |
|---|---|---|---|---|
| Marvin\|isParentOf\|Randy~isSpouseOf | true | true | false | 0 |
| Marvin\|isParentOf\|Sharon~isSpouseOf | true | false | false | 0 |
| Marvin\|isParentOf\|Shelly~isSpouseOf | true | false | false | 0 |

_ƒ تدل على عمود محسوب._

</details>

## 2 أنواع الحقائق

- كل سجل «**property**» يجوز أن يشير إلى سجل «**property**» واحد
- كل سجل «**property**» يجوز أن يشير إلى سجل «**thing**» واحد
- كل سجل «**statement**» يشير إلى سجل «**graph**» واحد فقط
- كل سجل «**statement**» يشير إلى سجل «**thing**» واحد فقط
- كل سجل «**statement**» يشير إلى سجل «**property**» واحد فقط
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

- في كل سجل «rule»، **يجب أن** تُحدّد قيمة حقل «label» وحقل «explanation»، وأن يُسجّل ما إذا كان مصنّفًا على أنه «enabled».
- في كل سجل «thing»، **يجب أن** تُحدّد قيمة حقل «label» وحقل «kind».
- في كل سجل «property»، **يجب أن** تُحدّد قيمة حقل «label»، وأن يُسجّل ما إذا كان مصنّفًا على أنه «symmetric».
- **يجب أن** يشير كل سجل «statement» إلى سجل «graph» واحد فقط.
- **يجب أن** يشير كل سجل «statement» إلى سجل «thing» واحد فقط عبر حقل «subject».
- **يجب أن** يشير كل سجل «statement» إلى سجل «property» واحد فقط.
- في كل سجل «statement»، **يجب أن** يُسجّل ما إذا كان مصنّفًا على أنه «stated».
- **يجب أن** يشير كل سجل «firing» إلى سجل «rule» واحد فقط.
- **يجب أن** يشير كل سجل «firing» إلى سجل «statement» واحد فقط عبر حقل «target».
- **يجب أن** يشير كل سجل «firing» إلى سجل «statement» واحد فقط عبر حقل «first».
- **يجب أن** يشير كل سجل «firing» إلى سجل «statement» واحد فقط عبر حقل «second».
- في كل سجل «firing»، **يجب أن** تُحدّد قيمة حقل «kind».
- **يجب أن** يشير كل سجل «disjoint check» إلى سجل «graph» واحد فقط.
- **يجب أن** يشير كل سجل «disjoint check» إلى سجل «rule» واحد فقط.
- **يجب أن** يشير كل سجل «disjoint check» إلى سجل «statement» واحد فقط عبر حقل «a».
- **يجب أن** يشير كل سجل «disjoint check» إلى سجل «statement» واحد فقط عبر حقل «b».

## 4 القواعد التعريفية

_جميع العبارات تعبّر عن حقيقة في مجال العمل؛ فهي ليست إجراءات ولا أوامر.
يُتجنّب التعبير «إذا وفقط إذا» ويُستعمل «فقط إذا»، حتى لا تُحسب الضرورة في اتجاه واحد تكافؤًا.
وتدل شارة **⚠︎ آلي** على قاعدة صياغتها الحتمية أمينة لكنها متكلّفة —
فهي إشارة إلى إعادة صياغة اختيارية لاحقة، لا إلى عيب._

| المعرّف | القاعدة التصريحية |
|----|------------------|
| **DR-1 Name** | حقل «name» في سجل «graph» يطابق حقل «graph ID» في السجل نفسه. |
| **DR-2 Count of Stated Facts** | حقل «count of stated facts» في سجل «graph» هو مجموع قيم حقل «stated unit» عبر سجلات «statements» المرتبطة بسجل «graph». |
| **DR-3 Count of Conclusions** | حقل «count of conclusions» في سجل «graph» هو مجموع قيم حقل «conclusion unit» عبر سجلات «statements» المرتبطة بسجل «graph». |
| **DR-4 Count of Violations** | حقل «count of violations» في سجل «graph» هو مجموع قيم حقل «violation unit» عبر سجلات «disjoint checks» المرتبطة بسجل «graph». |
| **DR-5 Is Consistent** | يُصنّف سجل «graph» على أنه «consistent» إذا تحقق أنّ حقل «count of violations» يساوي 0. |
| **DR-6 Name** | حقل «name» في سجل «rule» يطابق حقل «label» في السجل نفسه. |
| **DR-7 Count of Firings** | حقل «count of firings» في سجل «rule» هو مجموع قيم حقل «fired unit» عبر سجلات «firings» المرتبطة بسجل «rule». |
| **DR-8 Name** | حقل «name» في سجل «thing» يطابق حقل «label» في السجل نفسه. |
| **DR-9 Count of Children** | حقل «count of children» في سجل «thing» هو مجموع قيم حقل «parent unit» عبر سجلات «statements» المرتبطة بسجل «thing». |
| **DR-10 Count of Grandchildren** | حقل «count of grandchildren» في سجل «thing» هو مجموع قيم حقل «grandparent unit» عبر سجلات «statements» المرتبطة بسجل «thing». |
| **DR-11 Name** | حقل «name» في سجل «property» يطابق حقل «label» في السجل نفسه. |
| **DR-12 Subject Label** | حقل «subject label» في سجل «statement» مأخوذ من السجل المرتبط به عبر حقل «subject». |
| **DR-13 Property Label** | حقل «property label» في سجل «statement» مأخوذ من السجل المرتبط به عبر حقل «property». |
| **DR-14 Object Label** | حقل «object label» في سجل «statement» مأخوذ من السجل المرتبط به عبر حقل «object thing». |
| **DR-15 Name** | حقل «name» في سجل «statement» يُحسب على النحو الآتي: حقل «subject label»، ثم مسافة، ثم حقل «property label»، ثم مسافة، ثم حقل «number value» إذا تحقق أنّ حقل «object thing» فارغ، وفي سائر الحالات حقل «object label». |
| **DR-16 Count of Inverse Firings** | حقل «count of inverse firings» في سجل «statement» هو مجموع قيم حقل «inverse fired unit» عبر سجلات «firings» المرتبطة بسجل «statement». |
| **DR-17 Count of Symmetric Firings** | حقل «count of symmetric firings» في سجل «statement» هو مجموع قيم حقل «symmetric fired unit» عبر سجلات «firings» المرتبطة بسجل «statement». |
| **DR-18 Holds Before Chain** | يُعدّ سجل «statement» محققًا لشرط «holds before chain» إذا تحقق أنّ واحد على الأقل من الشروط الآتية متحقق: علامة «stated» مفعّلة، أو حقل «count of inverse firings» أكبر من 0، أو حقل «count of symmetric firings» أكبر من 0. |
| **DR-19 Count of Chain Firings** | حقل «count of chain firings» في سجل «statement» هو مجموع قيم حقل «chain fired unit» عبر سجلات «firings» المرتبطة بسجل «statement». |
| **DR-20 Holds Before Typing** | يُعدّ سجل «statement» محققًا لشرط «holds before typing» إذا تحقق أنّ واحد على الأقل من الشروط الآتية متحقق: علامة «holds before chain» مفعّلة أو حقل «count of chain firings» أكبر من 0. |
| **DR-21 Count of Type Firings** | حقل «count of type firings» في سجل «statement» هو مجموع قيم حقل «type fired unit» عبر سجلات «firings» المرتبطة بسجل «statement». |
| **DR-22 Holds** | يُعدّ سجل «statement» محققًا لشرط «holds» إذا تحقق أنّ واحد على الأقل من الشروط الآتية متحقق: علامة «holds before typing» مفعّلة أو حقل «count of type firings» أكبر من 0. |
| **DR-23 Is Conclusion** | يُصنّف سجل «statement» على أنه «conclusion» إذا تحقق أنّ جميع الشروط الآتية متحققة: علامة «holds» مفعّلة وعلامة «stated» غير مفعّلة. |
| **DR-24 Worked Out by** | حقل «worked out by» في سجل «statement» يتحدد وفق ترتيب الأولوية الآتي:<br>1. نص فارغ، إذا تحقق أنّ علامة «conclusion» غير مفعّلة؛<br>2. «inverse»، إذا تحقق أنّ حقل «count of inverse firings» أكبر من 0؛<br>3. «symmetric»، إذا تحقق أنّ حقل «count of symmetric firings» أكبر من 0؛<br>4. «property chain»، إذا تحقق أنّ حقل «count of chain firings» أكبر من 0؛<br>5. «domain and range»، في سائر الحالات. |
| **DR-25 Stated Unit** | حقل «stated unit» في سجل «statement» يتحدد وفق ترتيب الأولوية الآتي:<br>1. 1، إذا تحقق أنّ علامة «stated» مفعّلة؛<br>2. 0، في سائر الحالات. |
| **DR-26 Conclusion Unit** | حقل «conclusion unit» في سجل «statement» يتحدد وفق ترتيب الأولوية الآتي:<br>1. 1، إذا تحقق أنّ علامة «conclusion» مفعّلة؛<br>2. 0، في سائر الحالات. |
| **DR-27 Parent Unit** | حقل «parent unit» في سجل «statement» يتحدد وفق ترتيب الأولوية الآتي:<br>1. 1، إذا تحقق أنّ جميع الشروط الآتية متحققة: علامة «holds» مفعّلة وحقل «property» يساوي «isParentOf»؛<br>2. 0، في سائر الحالات. |
| **DR-28 Grandparent Unit** | حقل «grandparent unit» في سجل «statement» يتحدد وفق ترتيب الأولوية الآتي:<br>1. 1، إذا تحقق أنّ جميع الشروط الآتية متحققة: علامة «holds» مفعّلة وحقل «property» يساوي «isGrandparentOf»؛<br>2. 0، في سائر الحالات. |
| **DR-29 Rule Enabled** | حقل «rule enabled» في سجل «firing» يكون صحيحًا عندما يكون السجل المرتبط به عبر حقل «rule» مصنّفًا على أنه «enabled». |
| **DR-30 Rule Label** | حقل «rule label» في سجل «firing» مأخوذ من السجل المرتبط به عبر حقل «rule». |
| **DR-31 Target Name** | حقل «target name» في سجل «firing» مأخوذ من السجل المرتبط به عبر حقل «target». |
| **DR-32 First Name** | حقل «first name» في سجل «firing» مأخوذ من السجل المرتبط به عبر حقل «first». |
| **DR-33 Second Name** | حقل «second name» في سجل «firing» مأخوذ من السجل المرتبط به عبر حقل «second». |
| **DR-34 First is Stated** | حقل «first is stated» في سجل «firing» يكون صحيحًا عندما يكون السجل المرتبط به عبر حقل «first» مصنّفًا على أنه «stated». |
| **DR-35 First Holds Before Chain** | حقل «first holds before chain» في سجل «firing» يكون صحيحًا عندما يكون السجل المرتبط به عبر حقل «first» موسومًا بعلامة «holds before chain». |
| **DR-36 Second Holds Before Chain** | حقل «second holds before chain» في سجل «firing» يكون صحيحًا عندما يكون السجل المرتبط به عبر حقل «second» موسومًا بعلامة «holds before chain». |
| **DR-37 First Holds Before Typing** | حقل «first holds before typing» في سجل «firing» يكون صحيحًا عندما يكون السجل المرتبط به عبر حقل «first» موسومًا بعلامة «holds before typing». |
| **DR-38 Inverse Fired Unit** | حقل «inverse fired unit» في سجل «firing» يتحدد وفق ترتيب الأولوية الآتي:<br>1. 1، إذا تحقق أنّ جميع الشروط الآتية متحققة: علامة «rule enabled» مفعّلة، وحقل «kind» يساوي «inverse»، وعلامة «first is stated» مفعّلة؛<br>2. 0، في سائر الحالات. |
| **DR-39 Symmetric Fired Unit** | حقل «symmetric fired unit» في سجل «firing» يتحدد وفق ترتيب الأولوية الآتي:<br>1. 1، إذا تحقق أنّ جميع الشروط الآتية متحققة: علامة «rule enabled» مفعّلة، وحقل «kind» يساوي «symmetric»، وعلامة «first is stated» مفعّلة؛<br>2. 0، في سائر الحالات. |
| **DR-40 Chain Fired Unit** | حقل «chain fired unit» في سجل «firing» يتحدد وفق ترتيب الأولوية الآتي:<br>1. 1، إذا تحقق أنّ جميع الشروط الآتية متحققة: علامة «rule enabled» مفعّلة، وحقل «kind» يساوي «chain»، وعلامة «first holds before chain» مفعّلة، وعلامة «second holds before chain» مفعّلة؛<br>2. 0، في سائر الحالات. |
| **DR-41 Type Fired Unit** | حقل «type fired unit» في سجل «firing» يتحدد وفق ترتيب الأولوية الآتي:<br>1. 1، إذا تحقق أنّ جميع الشروط الآتية متحققة: علامة «rule enabled» مفعّلة؛ وواحد على الأقل من الشروط الآتية متحقق: حقل «kind» يساوي «domain» أو حقل «kind» يساوي «range»؛ وعلامة «first holds before typing» مفعّلة؛<br>2. 0، في سائر الحالات. |
| **DR-42 Fired Unit** | حقل «fired unit» في سجل «firing» يُحسب على النحو الآتي: حقل «inverse fired unit» زائد حقل «symmetric fired unit» زائد حقل «chain fired unit» زائد حقل «type fired unit». |
| **DR-43 Name** | حقل «name» في سجل «firing» يتحدد وفق ترتيب الأولوية الآتي:<br>1. حقل «target name»، ثم «: worked out by the »، ثم حقل «rule label»، ثم « rule from '»، ثم حقل «first name»، ثم «'»، ثم « and '»، ثم حقل «second name»، ثم «'»، إذا تحقق أنّ حقل «fired unit» أكبر من 0؛<br>2. «firing ID»، في سائر الحالات. |
| **DR-44 Name** | حقل «name» في سجل «disjoint check» يطابق حقل «disjoint check ID» في السجل نفسه. |
| **DR-45 Rule Enabled** | حقل «rule enabled» في سجل «disjoint check» يكون صحيحًا عندما يكون السجل المرتبط به عبر حقل «rule» مصنّفًا على أنه «enabled». |
| **DR-46 A Holds** | حقل «a holds» في سجل «disjoint check» يكون صحيحًا عندما يكون السجل المرتبط به عبر حقل «a» موسومًا بعلامة «holds». |
| **DR-47 B Holds** | حقل «b holds» في سجل «disjoint check» يكون صحيحًا عندما يكون السجل المرتبط به عبر حقل «b» موسومًا بعلامة «holds». |
| **DR-48 Violation Unit** | حقل «violation unit» في سجل «disjoint check» يتحدد وفق ترتيب الأولوية الآتي:<br>1. 1، إذا تحقق أنّ جميع الشروط الآتية متحققة: علامة «rule enabled» مفعّلة، وعلامة «a holds» مفعّلة، وعلامة «b holds» مفعّلة؛<br>2. 0، في سائر الحالات. |

## 5 التتبّع إلى المخطط

_عمود التعبير هو تعريف القاعدة بترميز التوثيق —
المنطق نفسه الذي يخزّنه دفتر القواعد، مكتوبًا لقارئ من أهل العمل._

| عنصر المخطط | النوع | التعبير |
|----------------|------|------------|
| **Graphs.Name** | صيغة | `GraphId` |
| **Graphs.CountOfStatedFacts** | تجميع | `Sum(Statements.StatedUnit via Graph)` |
| **Graphs.CountOfConclusions** | تجميع | `Sum(Statements.ConclusionUnit via Graph)` |
| **Graphs.CountOfViolations** | تجميع | `Sum(DisjointChecks.ViolationUnit via Graph)` |
| **Graphs.IsConsistent** | صيغة | `CountOfViolations = 0` |
| **Rules.Name** | صيغة | `Label` |
| **Rules.CountOfFirings** | تجميع | `Sum(Firings.FiredUnit via Rule)` |
| **Things.Name** | صيغة | `Label` |
| **Things.CountOfChildren** | تجميع | `Sum(Statements.ParentUnit via Subject)` |
| **Things.CountOfGrandchildren** | تجميع | `Sum(Statements.GrandparentUnit via Subject)` |
| **Properties.Name** | صيغة | `Label` |
| **Statements.SubjectLabel** | بحث | `Lookup(Things.Label via Subject)` |
| **Statements.PropertyLabel** | بحث | `Lookup(Properties.Label via Property)` |
| **Statements.ObjectLabel** | بحث | `Lookup(Things.Label via ObjectThing)` |
| **Statements.Name** | صيغة | `SubjectLabel & " " & PropertyLabel & " " & If(ObjectThing = Blank(), NumberValue, ObjectLabel)` |
| **Statements.CountOfInverseFirings** | تجميع | `Sum(Firings.InverseFiredUnit via Target)` |
| **Statements.CountOfSymmetricFirings** | تجميع | `Sum(Firings.SymmetricFiredUnit via Target)` |
| **Statements.HoldsBeforeChain** | صيغة | `Or(IsStated, CountOfInverseFirings > 0, CountOfSymmetricFirings > 0)` |
| **Statements.CountOfChainFirings** | تجميع | `Sum(Firings.ChainFiredUnit via Target)` |
| **Statements.HoldsBeforeTyping** | صيغة | `Or(HoldsBeforeChain, CountOfChainFirings > 0)` |
| **Statements.CountOfTypeFirings** | تجميع | `Sum(Firings.TypeFiredUnit via Target)` |
| **Statements.Holds** | صيغة | `Or(HoldsBeforeTyping, CountOfTypeFirings > 0)` |
| **Statements.IsConclusion** | صيغة | `And(Holds, Not(IsStated))` |
| **Statements.WorkedOutBy** | صيغة | `If(Not(IsConclusion), "", If(CountOfInverseFirings > 0, "inverse", If(CountOfSymmetricFirings > 0, "symmetric", If(CountOfChainFirings > 0, "property chain", "domain and range"))))` |
| **Statements.StatedUnit** | صيغة | `If(IsStated, 1, 0)` |
| **Statements.ConclusionUnit** | صيغة | `If(IsConclusion, 1, 0)` |
| **Statements.ParentUnit** | صيغة | `If(And(Holds, Property = "isParentOf"), 1, 0)` |
| **Statements.GrandparentUnit** | صيغة | `If(And(Holds, Property = "isGrandparentOf"), 1, 0)` |
| **Firings.RuleEnabled** | بحث | `Lookup(Rules.IsEnabled via Rule)` |
| **Firings.RuleLabel** | بحث | `Lookup(Rules.Label via Rule)` |
| **Firings.TargetName** | بحث | `Lookup(Statements.Name via Target)` |
| **Firings.FirstName** | بحث | `Lookup(Statements.Name via First)` |
| **Firings.SecondName** | بحث | `Lookup(Statements.Name via Second)` |
| **Firings.FirstIsStated** | بحث | `Lookup(Statements.IsStated via First)` |
| **Firings.FirstHoldsBeforeChain** | بحث | `Lookup(Statements.HoldsBeforeChain via First)` |
| **Firings.SecondHoldsBeforeChain** | بحث | `Lookup(Statements.HoldsBeforeChain via Second)` |
| **Firings.FirstHoldsBeforeTyping** | بحث | `Lookup(Statements.HoldsBeforeTyping via First)` |
| **Firings.InverseFiredUnit** | صيغة | `If(And(RuleEnabled, Kind = "inverse", FirstIsStated), 1, 0)` |
| **Firings.SymmetricFiredUnit** | صيغة | `If(And(RuleEnabled, Kind = "symmetric", FirstIsStated), 1, 0)` |
| **Firings.ChainFiredUnit** | صيغة | `If(And(RuleEnabled, Kind = "chain", FirstHoldsBeforeChain, SecondHoldsBeforeChain), 1, 0)` |
| **Firings.TypeFiredUnit** | صيغة | `If(And(RuleEnabled, Or(Kind = "domain", Kind = "range"), FirstHoldsBeforeTyping), 1, 0)` |
| **Firings.FiredUnit** | صيغة | `InverseFiredUnit + SymmetricFiredUnit + ChainFiredUnit + TypeFiredUnit` |
| **Firings.Name** | صيغة | `If(FiredUnit > 0, TargetName & ": worked out by the " & RuleLabel & " rule from '" & FirstName & "'" & If(Kind = "chain", " and '" & SecondName & "'", ""), FiringId)` |
| **DisjointChecks.Name** | صيغة | `DisjointCheckId` |
| **DisjointChecks.RuleEnabled** | بحث | `Lookup(Rules.IsEnabled via Rule)` |
| **DisjointChecks.AHolds** | بحث | `Lookup(Statements.Holds via A)` |
| **DisjointChecks.BHolds** | بحث | `Lookup(Statements.Holds via B)` |
| **DisjointChecks.ViolationUnit** | صيغة | `If(And(RuleEnabled, AHolds, BHolds), 1, 0)` |
