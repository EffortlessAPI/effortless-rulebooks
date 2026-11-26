# 📘 Talisman's South Park ontology explainer — ドキュメント

_Jessica Talisman's South Park Ontology Explainer, as a rulebook: the page's ten facts, five rule switches, property wiring, provenance wording and current-value policy are rows, and every conclusion is a formula. The rules apply in three fixed strata (inverse and symmetric, then the property chain, then domain and range); property wiring that feeds an earlier stratum would need repeated rounds._

> ルールブックから生成された宣言的ビジネスルールです。以下の各文はビジネス
> ドメインにおける真実を表し、手続きでも命令でもありません。ルールブックの
> 数式が唯一の真実の源であり、本書はその平易な読み下しです。

## 1 ビジネス語彙

| 用語 | 説明 | 補足コメント |
|------|-------------|-------------------|
| **Graph** | The explainer page as one graph: its wording variables and its totals. | — |
| Label | 定義済みの属性。 | _Display name of the graph._ |
| Literal Quote | 定義済みの属性。 | _The quote mark written around a literal value in a fact's wording._ |
| Literal Type Marker | 定義済みの属性。 | _The marker written between a literal value and its datatype, as in Turtle._ |
| Built in Source Text | 定義済みの属性。 | _Source line shown for a fact the page states as a teaching example._ |
| User Source Text | 定義済みの属性。 | _Source line shown for a fact the reader stated._ |
| No Conclusions Text | 定義済みの属性。 | _Message shown when the selected facts and rules add no conclusion._ |
| Name | label と同じ。 | _Order 1. Display name of the graph._ |
| Count of Stated Facts | Graphに関連する「statements」全体での「in effect unit」の合計。 | _Order 5. Facts that are switched on and in effect._ |
| Count of Conclusions | Graphに関連する「statements」全体での「conclusion unit」の合計。 | _Order 18. Facts that hold only because an enabled rule concluded them._ |
| Count of Violations | Graphに関連する「disjoint checks」全体での「violation unit」の合計。 | _Order 18. Disjointness checks that found both links between the same pair._ |
| Is Consistent | count of violations が 0 であるとき真。 | _Order 19. True when no enabled disjointness rule is violated._ |
| Conclusions Message | 優先順位で決定: count of conclusions が 0 である場合は no conclusions text; それ以外の場合は 空文字列。 | _Order 19. The empty-state message for the conclusions panel, blank when there are conclusions._ |
| Count of Facts to Add | Graphに関連する「statements」全体での「listed off unit」の合計。 | _Order 5. Listed facts that are not in effect; the Add next fact button is enabled while this is above zero._ |
| Has Facts to Add | count of facts to add が 0 より大きいとき真。 | _Order 6. True while some listed fact can still be added._ |
| Next Fact Sort Order | Graphの「statements」のうち「Listed Off Unit」であるものについての「sort order」の最小値。 | _Order 5. List position of the first listed fact that is not in effect._ |
| Max Activation Seq | Graphに関連する「statements」全体での「activation seq」の最大値。 | _Order 1. Highest activation number used so far._ |
| Next Activation Seq | max activation seq たす 1 として計算。 | _Order 2. The activation number the next switched-on fact takes._ |
| Max Listed Sort Order | Graphの「statements」のうち「Listed Unit」であるものについての「sort order」の最大値。 | _Order 2. Highest list position among listed facts._ |
| Next Listed Sort Order | max listed sort order たす 1 として計算。 | _Order 3. The list position a newly added fact takes._ |

<details open>
<summary>サンプルデータ</summary>

| Label | Literal Quote | Literal Type Marker | Built in Source Text | User Source Text | No Conclusions Text | Name ƒ | Count of Stated Facts ƒ | Count of Conclusions ƒ | Count of Violations ƒ | Is Consistent ƒ | Conclusions Message ƒ | Count of Facts to Add ƒ | Has Facts to Add ƒ | Next Fact Sort Order ƒ | Max Activation Seq ƒ | Next Activation Seq ƒ | Max Listed Sort Order ƒ | Next Listed Sort Order ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| South Park | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | These facts and selected rules add no further conclusions. Try another fact or rule. | South Park | 10 | 0 | 0 | true | These facts and selected rules add no further conclusions. Try another fact or rule. | 0 | false | — | 10 | 11 | 10 | 11 |

_ƒ は計算列を示します。_

</details>

| 用語 | 説明 | 補足コメント |
|------|-------------|-------------------|
| **Rule** | The five rule switches on the page. | — |
| Label | 定義済みの属性。 | _The rule's name on the page._ |
| Explanation | 定義済みの属性。 | _The rule's one-line explanation on the page._ |
| Is Enabled | 空文字列とき真。 | _The rule's checkbox._ |
| Sort Order | 定義済みの属性。 | _Position in the rule list._ |
| Name | label と同じ。 | _Order 1. The rule's name._ |
| Count of Firings | Ruleに関連する「firings」全体での「fired unit」の合計。 | _Order 15. Rule applications that fired, counting every derivation._ |

<details open>
<summary>サンプルデータ</summary>

| Label | Explanation | Is Enabled | Sort Order | Name ƒ | Count of Firings ƒ |
|---|---|---|---|---|---|
| Inverse | A parent link also gives a child link between the same two people. | false | 1 | Inverse | 0 |
| Symmetric | A friend link or a spouse link is true for both people. | false | 2 | Symmetric | 0 |
| Property chain | A parent of a parent is a grandparent. | false | 3 | Property chain | 0 |

_ƒ は計算列を示します。_

</details>

| 用語 | 説明 | 補足コメント |
|------|-------------|-------------------|
| **Firing Kind** | The ways a rule can apply. Domain and range belong to one rule but are worded apart. | — |
| Rule | 定義済みの属性。 | _The rule switch this kind belongs to._ |
| Wording | 定義済みの属性。 | _How a conclusion card names the rule, as in 'Worked out by the inverse rule'._ |
| Sort Order | 定義済みの属性。 | _Order in which the page's reasoner tries the kinds for one fact._ |
| Name | wording と同じ。 | _Order 1. The kind's wording._ |
| Rule Enabled | リンクされた「rule」が「Enabled」であるとき真。 | _Order 1. Whether the owning rule is switched on._ |

<details open>
<summary>サンプルデータ</summary>

| Wording | Sort Order | Name ƒ | Rule Enabled ƒ |
|---|---|---|---|
| inverse rule | 1 | inverse rule | false |
| symmetric rule | 2 | symmetric rule | false |
| property chain rule | 3 | property chain rule | false |

_ƒ は計算列を示します。_

</details>

| 用語 | 説明 | 補足コメント |
|------|-------------|-------------------|
| **Thing** | Every individual and class the page can draw. | — |
| Label | 定義済みの属性。 | _The name the page shows._ |
| Kind | 定義済みの属性。 | _'individual' or 'class'._ |
| Sort Order | 定義済みの属性。 | _Position in the cast._ |
| Portrait Name | 定義済みの属性。 | _The name on the portrait's Fandom page._ |
| Portrait Page | 定義済みの属性。 | _The Fandom page credited for the portrait._ |
| Portrait Image | 定義済みの属性。 | _The original Fandom image._ |
| Portrait Crop | 定義済みの属性。 | _The portrait's crop as an SVG viewBox: x y width height._ |
| Portrait Width | 定義済みの属性。 | _The original image's width in pixels._ |
| Portrait Height | 定義済みの属性。 | _The original image's height in pixels._ |
| Name | label と同じ。 | _Order 1. The thing's name._ |
| Node Key | thing ID と同じ。 | _Order 1. The key the graph draws this thing under._ |
| Count of Children | Thingに関連する「statements」全体での「parent unit」の合計。 | _Order 17. People this thing is a parent of, stated or concluded._ |
| Count of Grandchildren | Thingに関連する「statements」全体での「grandparent unit」の合計。 | _Order 17. People this thing is a grandparent of._ |
| Count As Place | Thingに関連する「statements」全体での「place unit」の合計。 | _Order 17. Holding attends facts whose object is this thing._ |
| Count As Subject | Thingに関連する「statements」全体での「holds unit」の合計。 | _Order 17. Holding facts about this thing._ |
| Count As Object | Thingに関連する「statements」全体での「holds unit」の合計。 | _Order 17. Holding facts that point at this thing._ |
| Is in Graph | 次のいずれかが成り立つ: count as subject が 0 より大きいまたはcount as object が 0 より大きいとき真。 | _Order 18. True when the graph draws this thing._ |
| Node Kind | 優先順位で決定: kind が 「class」 である場合は 「class」; count as place が 0 より大きい場合は 「place」; それ以外の場合は 「person」。 | _Order 18. How the graph draws the thing: a class, a place someone attends, or a person._ |

<details open>
<summary>サンプルデータ</summary>

| Label | Kind | Sort Order | Portrait Name | Portrait Page | Portrait Image | Portrait Crop | Portrait Width | Portrait Height | Name ƒ | Node Key ƒ | Count of Children ƒ | Count of Grandchildren ƒ | Count As Place ƒ | Count As Subject ƒ | Count As Object ƒ | Is in Graph ƒ | Node Kind ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| Marvin | individual | 1 | Marvin Marsh | https://southpark.fandom.com/wiki/Marvin_Marsh | https://static.wikia.nocookie.net/southpark/images/d/db/Marsh-family-grandpa-marsh-w-purple-blanket.png/revision/latest | 48 -5 280 280 | 375 | 536 | Marvin | Marvin | 1 | 0 | 0 | 1 | 0 | true | person |
| Randy | individual | 2 | Randy Marsh | https://southpark.fandom.com/wiki/Randy_Marsh | https://static.wikia.nocookie.net/southparkfanon/images/d/d6/Randy_Marsh.png/revision/latest | 0 -10 428 428 | 428 | 1057 | Randy | Randy | 2 | 0 | 0 | 3 | 1 | true | person |
| Sharon | individual | 3 | Sharon Marsh | https://southpark.fandom.com/wiki/Sharon_Marsh | https://static.wikia.nocookie.net/southpark/images/9/99/Sharon_Marsh.png/revision/latest | 20 -10 390 390 | 439 | 1049 | Sharon | Sharon | 0 | 0 | 0 | 0 | 1 | true | person |

_ƒ は計算列を示します。_

</details>

| 用語 | 説明 | 補足コメント |
|------|-------------|-------------------|
| **Property** | The ten properties and the wiring every rule reads. | — |
| Label | 定義済みの属性。 | _The property's wording on the page._ |
| Property Kind | 定義済みの属性。 | _'object' links two things, 'datatype' links a thing to a value, 'type' states class membership._ |
| Datatype | 定義済みの属性。 | _The value's XML Schema datatype for a datatype property; blank otherwise._ |
| Is User Addable | 空文字列とき真。 | _Offered in the page's Add fact form._ |
| Has One Current Value | 空文字列とき真。 | _The page keeps one current value per person: switching a value on replaces the earlier one._ |
| Inverse of | 定義済みの属性。 | _The property the inverse rule derives from this one._ |
| Is Symmetric | 空文字列とき真。 | _The symmetric rule makes this link true in both directions._ |
| Chain First | 定義済みの属性。 | _First link of the property chain that derives this property._ |
| Chain Second | 定義済みの属性。 | _Second link of the property chain that derives this property._ |
| Domain Class | 定義済みの属性。 | _The class the domain rule gives this property's subject._ |
| Range Class | 定義済みの属性。 | _The class the range rule gives this property's object._ |
| Disjoint With | 定義済みの属性。 | _A property that may not link the same pair, in the same direction, as this one._ |
| Sort Order | 定義済みの属性。 | _Position in the Add fact form's property list._ |
| Value Node Label | 定義済みの属性。 | _The small label under a value node for this property: 'role value' or 'number value'._ |
| Current Value Wording | 定義済みの属性。 | _How the Add fact form names a replaced value, as in 'Stan now has grade level 5'._ |
| Input Name | 定義済みの属性。 | _How the integer check names this property's value, as in 'for grade level'._ |
| Record Kind | 定義済みの属性。 | _The word the current-record summary uses for this property's values: role, grade or school._ |
| Record Order | 定義済みの属性。 | _Position of this property in the current-record summary._ |
| Name | label と同じ。 | _Order 1. The property's wording._ |

<details open>
<summary>サンプルデータ</summary>

| Label | Property Kind | Datatype | Is User Addable | Has One Current Value | Is Symmetric | Sort Order | Value Node Label | Current Value Wording | Input Name | Record Kind | Record Order | Name ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|
| is parent of | object | — | true | false | false | 1 | — | — | — | — | — | is parent of |
| is child of | object | — | true | false | false | 2 | — | — | — | — | — | is child of |
| is spouse of | object | — | true | false | true | 3 | — | — | — | — | — | is spouse of |

_ƒ は計算列を示します。_

</details>

| 用語 | 説明 | 補足コメント |
|------|-------------|-------------------|
| **Statement** | Every fact the page can state or conclude about the cast. A listed fact appears in the page's fact list; the rest are candidates that only a rule can make hold. | — |
| Graph | 定義済みの属性。 | _The graph the fact belongs to._ |
| Subject | 定義済みの属性。 | _What the fact describes._ |
| Property | 定義済みの属性。 | _How the subject connects to the object._ |
| Object Thing | 定義済みの属性。 | _The related thing, for an object or type property._ |
| Object Value | 定義済みの属性。 | _The literal value, for a datatype property._ |
| Is Listed | 空文字列とき真。 | _Shown in the page's fact list._ |
| Is Built in | 空文字列とき真。 | _One of the page's teaching facts; the reader cannot remove it._ |
| Is Detail | 空文字列とき真。 | _One of Stan's detail facts that Show Stan's details switches on._ |
| Is Stated | 空文字列とき真。 | _The fact's checkbox._ |
| Activation Seq | 定義済みの属性。 | _When the fact was last switched on, as a running number; decides which value is current._ |
| Sort Order | 定義済みの属性。 | _Position in the fact list; candidates follow the listed facts._ |
| Subject Label | リンクされた「subject」から取得。 | _Order 1. The subject's name._ |
| Property Label | リンクされた「property」から取得。 | _Order 1. The property's wording._ |
| Object Label | リンクされた「object thing」から取得。 | _Order 1. The object's name, for an object or type property._ |
| Property Datatype | リンクされた「property」から取得。 | _Order 1. The value's datatype, for a datatype property._ |
| Property Has One Current Value | リンクされた「property」が「One Current Value」を持つとき真。 | _Order 1. Whether the property keeps one current value per person._ |
| Property Domain Class | リンクされた「property」から取得。 | _Order 1. The property's domain class._ |
| Property Range Class | リンクされた「property」から取得。 | _Order 1. The property's range class._ |
| Literal Quote | リンクされた「graph」から取得。 | _Order 1. The graph's literal quote mark._ |
| Literal Type Marker | リンクされた「graph」から取得。 | _Order 1. The graph's datatype marker._ |
| Built in Source Text | リンクされた「graph」から取得。 | _Order 1. The graph's source line for a teaching fact._ |
| User Source Text | リンクされた「graph」から取得。 | _Order 1. The graph's source line for a reader's fact._ |
| Next Fact Sort Order | リンクされた「graph」から取得。 | _Order 6. List position of the next fact to add._ |
| Is Literal | object thing が空であるとき真。 | _Order 1. True when the object is a value rather than a thing._ |
| Object Text | 優先順位で決定: object thing が空である場合は object value; それ以外の場合は object label。 | _Order 2. The object's name or the value._ |
| Name | subject label、続いて 空白 1 つ、続いて property label、続いて 空白 1 つ、続いて object text として計算。 | _Order 3. The fact in words._ |
| Words | subject label、続いて 空白 1 つ、続いて property label、続いて 空白 1 つ、続いて object thing が空である場合は literal quote、続いて object value、続いて literal quote、続いて literal type marker、続いて property datatype、それ以外の場合は object label として計算。 ⚠︎ 機械的 <!-- rulespeak:reword --> | _Order 2. The fact as a conclusion card quotes it, with a literal written in Turtle form._ |
| Source | 優先順位で決定: フラグ「built in」が設定されている場合は built in source text; それ以外の場合は user source text。 | _Order 2. Where a listed fact came from._ |
| Subject Property Key | subject、続いて 「|」、続いて property として計算。 | _Order 1. Groups the values one subject has for one property._ |
| Latest Sibling Activation Seq | Statementに関連する「statements」全体での「activation seq」の最大値。 | _Order 2. The most recent activation among this subject's values for this property._ |
| Is in Effect | 次のすべてが成り立つ: フラグ「stated」が設定されているおよび次のいずれかが成り立つ: フラグ「property has one current value」が設定されていないまたはactivation seq が latest sibling activation seq であるとき真。 | _Order 3. Switched on and, for a one-value property, the most recently switched-on value. Drawn as a solid line._ |
| Listed Unit | 優先順位で決定: フラグ「listed」が設定されている場合は 1; それ以外の場合は 0。 | _Order 1. 1 when the fact is in the fact list._ |
| In Effect Unit | 優先順位で決定: フラグ「in effect」が設定されている場合は 1; それ以外の場合は 0。 | _Order 4. 1 when the fact is in effect._ |
| Listed Off Unit | 優先順位で決定: 次のすべてが成り立つ: フラグ「listed」が設定されているおよびフラグ「in effect」が設定されていない場合は 1; それ以外の場合は 0。 | _Order 4. 1 when the fact is listed but not in effect._ |
| Is Next to Add | 次のすべてが成り立つ: listed off unit が 1 であるおよびsort order が next fact sort order であるとき真。 | _Order 7. The fact the Add next fact button switches on._ |
| Count of Inverse Firings | Statementに関連する「firings」全体での「inverse fired unit」の合計。 | _Order 6. Inverse rule applications that conclude this fact._ |
| Count of Symmetric Firings | Statementに関連する「firings」全体での「symmetric fired unit」の合計。 | _Order 6. Symmetric rule applications that conclude this fact._ |
| Holds Before Chain | 次のいずれかが成り立つ: フラグ「in effect」が設定されている、count of inverse firings が 0 より大きい、またはcount of symmetric firings が 0 より大きいとき真。 | _Order 7. Holds from facts in effect plus the inverse and symmetric rules._ |
| Count of Chain Firings | Statementに関連する「firings」全体での「chain fired unit」の合計。 | _Order 10. Property chain applications that conclude this fact._ |
| Holds Before Typing | 次のいずれかが成り立つ: フラグ「holds before chain」が設定されているまたはcount of chain firings が 0 より大きいとき真。 | _Order 11. Holds before the domain and range rules apply._ |
| Count of Type Firings | Statementに関連する「firings」全体での「type fired unit」の合計。 | _Order 14. Domain and range applications that conclude this fact._ |
| Holds | 次のいずれかが成り立つ: フラグ「holds before typing」が設定されているまたはcount of type firings が 0 より大きいとき真。 | _Order 15. True when in effect or concluded by an enabled rule; the graph draws it._ |
| Is Conclusion | 次のすべてが成り立つ: フラグ「holds」が設定されているおよびフラグ「in effect」が設定されていないとき真。 | _Order 16. Holds only because a rule concluded it. Drawn dashed._ |
| Holds Unit | 優先順位で決定: フラグ「holds」が設定されている場合は 1; それ以外の場合は 0。 | _Order 16. 1 when the fact holds._ |
| Conclusion Unit | 優先順位で決定: フラグ「conclusion」が設定されている場合は 1; それ以外の場合は 0。 | _Order 17. 1 when the fact is a conclusion._ |
| Parent Unit | 優先順位で決定: 次のすべてが成り立つ: フラグ「holds」が設定されているおよびproperty が 「isParentOf」 である場合は 1; それ以外の場合は 0。 | _Order 16. 1 for a holding parent link._ |
| Grandparent Unit | 優先順位で決定: 次のすべてが成り立つ: フラグ「holds」が設定されているおよびproperty が 「isGrandparentOf」 である場合は 1; それ以外の場合は 0。 | _Order 16. 1 for a holding grandparent link._ |
| Place Unit | 優先順位で決定: 次のすべてが成り立つ: フラグ「holds」が設定されているおよびproperty が 「attends」 である場合は 1; それ以外の場合は 0。 | _Order 16. 1 for a holding attends link._ |
| First Fired Order Key | Statementの「firings」のうち「Fired Unit」であるものについての「order key」の最小値。 | _Order 15. Order key of the first rule application that concludes this fact, in the page reasoner's order._ |
| Count of Chosen Firings | Statementに関連する「firings」全体での「chosen unit」の合計。 | _Order 20. Rule applications shown as this fact's conclusion card: one for a conclusion, zero otherwise._ |
| Subject Node Key | subject と同じ。 | _Order 1. The graph node this fact starts from._ |
| Object Node Key | 優先順位で決定: object thing が空である場合は 「literal|」、続いて object value、続いて 「|」、続いて property datatype; それ以外の場合は object thing。 | _Order 2. The graph node this fact points at: the thing, or one node per value and datatype._ |
| Subject Node Kind | リンクされた「subject」から取得。 | _Order 19. How the graph draws the subject._ |
| Object Thing Node Kind | リンクされた「object thing」から取得。 | _Order 19. How the graph draws the object thing._ |
| Object Node Kind | 優先順位で決定: object thing が空である場合は 「value」; それ以外の場合は object thing node kind。 | _Order 20. How the graph draws the object: a value, a class, a place or a person._ |
| Object Value Node Label | リンクされた「property」から取得。 | _Order 1. The small label under a value node._ |
| Count of Violations As First | Statementに関連する「disjoint checks」全体での「violation unit」の合計。 | _Order 18. Violated disjointness checks naming this fact first._ |
| Count of Violations As Second | Statementに関連する「disjoint checks」全体での「violation unit」の合計。 | _Order 18. Violated disjointness checks naming this fact second._ |
| Is in Contradiction | 次のいずれかが成り立つ: count of violations as first が 0 より大きいまたはcount of violations as second が 0 より大きいとき真。 | _Order 19. Part of a contradiction the disjoint rule found; drawn in the alarm colour._ |

<details open>
<summary>サンプルデータ</summary>

| Object Value | Is Listed | Is Built in | Is Detail | Is Stated | Activation Seq | Sort Order | Subject Label ƒ | Property Label ƒ | Object Label ƒ | Property Datatype ƒ | Property Has One Current Value ƒ | Property Domain Class ƒ | Property Range Class ƒ | Literal Quote ƒ | Literal Type Marker ƒ | Built in Source Text ƒ | User Source Text ƒ | Next Fact Sort Order ƒ | Is Literal ƒ | Object Text ƒ | Name ƒ | Words ƒ | Source ƒ | Subject Property Key ƒ | Latest Sibling Activation Seq ƒ | Is in Effect ƒ | Listed Unit ƒ | In Effect Unit ƒ | Listed Off Unit ƒ | Is Next to Add ƒ | Count of Inverse Firings ƒ | Count of Symmetric Firings ƒ | Holds Before Chain ƒ | Count of Chain Firings ƒ | Holds Before Typing ƒ | Count of Type Firings ƒ | Holds ƒ | Is Conclusion ƒ | Holds Unit ƒ | Conclusion Unit ƒ | Parent Unit ƒ | Grandparent Unit ƒ | Place Unit ƒ | First Fired Order Key ƒ | Count of Chosen Firings ƒ | Subject Node Key ƒ | Object Node Key ƒ | Subject Node Kind ƒ | Object Thing Node Kind ƒ | Object Node Kind ƒ | Object Value Node Label ƒ | Count of Violations As First ƒ | Count of Violations As Second ƒ | Is in Contradiction ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| — | true | true | false | true | 1 | 1 | Marvin | is parent of | Randy | — | false | — | — | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | — | false | Randy | Marvin is parent of Randy | Marvin is parent of Randy | Stated as a teaching example on this page. | Marvin\|isParentOf | 1 | true | 1 | 1 | 0 | false | 0 | 0 | true | 0 | true | 0 | true | false | 1 | 0 | 1 | 0 | 0 | — | 0 | Marvin | Randy | person | person | person | — | 0 | 0 | false |
| — | false | false | false | false | 0 | 101 | Marvin | is parent of | Sharon | — | false | — | — | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | — | false | Sharon | Marvin is parent of Sharon | Marvin is parent of Sharon | Stated by you on this page. | Marvin\|isParentOf | 1 | false | 0 | 0 | 0 | false | 0 | 0 | false | 0 | false | 0 | false | false | 0 | 0 | 0 | 0 | 0 | — | 0 | Marvin | Sharon | person | person | person | — | 0 | 0 | false |
| — | false | false | false | false | 0 | 102 | Marvin | is parent of | Stan | — | false | — | — | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | — | false | Stan | Marvin is parent of Stan | Marvin is parent of Stan | Stated by you on this page. | Marvin\|isParentOf | 1 | false | 0 | 0 | 0 | false | 0 | 0 | false | 0 | false | 0 | false | false | 0 | 0 | 0 | 0 | 0 | — | 0 | Marvin | Stan | person | person | person | — | 0 | 0 | false |

_ƒ は計算列を示します。_

</details>

| 用語 | 説明 | 補足コメント |
|------|-------------|-------------------|
| **Firing** | Every way an enabled rule could conclude a candidate fact from the facts that hold. | — |
| Kind | 定義済みの属性。 | _How the rule applies._ |
| Rule | 定義済みの属性。 | _The rule switch._ |
| Target | 定義済みの属性。 | _The fact this application concludes._ |
| First | 定義済みの属性。 | _The premise._ |
| Second | 定義済みの属性。 | _The second premise, for the property chain._ |
| Rule Enabled | リンクされた「rule」が「Enabled」であるとき真。 | _Order 1. Whether the rule is switched on._ |
| Kind Wording | リンクされた「kind」から取得。 | _Order 1. How the card names the rule._ |
| Kind Sort Order | リンクされた「kind」から取得。 | _Order 1. The kind's place in the reasoner's order._ |
| Target Name | リンクされた「target」から取得。 | _Order 4. The concluded fact in words._ |
| Target Subject Label | リンクされた「target」から取得。 | _Order 2. The concluded fact's subject._ |
| Target Property Label | リンクされた「target」から取得。 | _Order 2. The concluded fact's property._ |
| Target Object Text | リンクされた「target」から取得。 | _Order 3. The concluded fact's object._ |
| Target is Conclusion | リンクされた「target」が「Conclusion」であるとき真。 | _Order 17. Whether the concluded fact is new._ |
| Target Subject Node Key | リンクされた「target」から取得。 | _Order 2. The concluded fact's subject node._ |
| Target Object Node Key | リンクされた「target」から取得。 | _Order 3. The concluded fact's object node._ |
| Target First Fired Order Key | リンクされた「target」から取得。 | _Order 16. The order key of the concluded fact's first application._ |
| First Words | リンクされた「first」から取得。 | _Order 3. The premise as the card quotes it._ |
| Second Words | リンクされた「second」から取得。 | _Order 3. The second premise as the card quotes it._ |
| First is in Effect | リンクされた「first」が「In Effect」であるとき真。 | _Order 4. Whether the premise is in effect._ |
| First Holds Before Chain | リンクされた「first」が「Holds Before Chain」であるとき真。 | _Order 8. Whether the premise holds before the chain._ |
| Second Holds Before Chain | リンクされた「second」が「Holds Before Chain」であるとき真。 | _Order 8. Whether the second premise holds before the chain._ |
| First Holds Before Typing | リンクされた「first」が「Holds Before Typing」であるとき真。 | _Order 12. Whether the premise holds before typing._ |
| First Property | リンクされた「first」から取得。 | _Order 1. The premise's property key, as the card's axiom names it._ |
| First Property Domain Class | リンクされた「first」から取得。 | _Order 2. The premise property's domain class._ |
| First Property Range Class | リンクされた「first」から取得。 | _Order 2. The premise property's range class._ |
| First Sort Order | リンクされた「first」から取得。 | _Order 1. The premise's list position._ |
| Second Sort Order | リンクされた「second」から取得。 | _Order 1. The second premise's list position._ |
| Inverse Fired Unit | 優先順位で決定: 次のすべてが成り立つ: フラグ「rule enabled」が設定されている、kind が 「inverse」 である、およびフラグ「first is in effect」が設定されている場合は 1; それ以外の場合は 0。 | _Order 5. 1 when the inverse rule fires here._ |
| Symmetric Fired Unit | 優先順位で決定: 次のすべてが成り立つ: フラグ「rule enabled」が設定されている、kind が 「symmetric」 である、およびフラグ「first is in effect」が設定されている場合は 1; それ以外の場合は 0。 | _Order 5. 1 when the symmetric rule fires here._ |
| Chain Fired Unit | 優先順位で決定: 次のすべてが成り立つ: フラグ「rule enabled」が設定されている、kind が 「chain」 である、フラグ「first holds before chain」が設定されている、およびフラグ「second holds before chain」が設定されている場合は 1; それ以外の場合は 0。 | _Order 9. 1 when the property chain fires here._ |
| Type Fired Unit | 優先順位で決定: 次のすべてが成り立つ: フラグ「rule enabled」が設定されている、次のいずれかが成り立つ: kind が 「domain」 であるまたはkind が 「range」 である、およびフラグ「first holds before typing」が設定されている場合は 1; それ以外の場合は 0。 | _Order 13. 1 when the domain or range rule fires here._ |
| Fired Unit | inverse fired unit たす symmetric fired unit たす chain fired unit たす type fired unit として計算。 | _Order 14. 1 when this application fires._ |
| Order Key | first sort order かける 100000 たす kind sort order かける 1000 たす second が空である場合は 0、それ以外の場合は second sort order として計算。 | _Order 2. The page reasoner's order: by premise, then rule kind, then second premise._ |
| Is Chosen | 次のすべてが成り立つ: fired unit が 1 である、フラグ「target is conclusion」が設定されている、およびorder key が target first fired order key であるとき真。 | _Order 18. The application the conclusions panel shows for its fact: the first one in the reasoner's order._ |
| Chosen Unit | 優先順位で決定: フラグ「chosen」が設定されている場合は 1; それ以外の場合は 0。 | _Order 19. 1 when this application is shown._ |
| Card Text | 優先順位で決定: フラグ「chosen」が設定されている場合は 「Worked out by the 」、続いて kind wording、続いて 「 from “」、続いて first words、続いて 「”」、続いて kind が 「chain」 である場合は 「 and “」、続いて second words、続いて 「”」、それ以外の場合は kind が 「domain」 である場合は 「 and “」、続いて first property、続いて 「 has domain 」、続いて first property domain class、続いて 「”」、それ以外の場合は 「 and “」、続いて first property、続いて 「 has range 」、続いて first property range class、続いて 「”」、続いて ピリオド; それ以外の場合は 空文字列。 ⚠︎ 機械的 <!-- rulespeak:reword --> | _Order 19. The conclusion card's provenance line, in the page's wording._ |
| Name | firing ID と同じ。 | _Order 1. The application's key._ |

<details open>
<summary>サンプルデータ</summary>

| Rule Enabled ƒ | Kind Wording ƒ | Kind Sort Order ƒ | Target Name ƒ | Target Subject Label ƒ | Target Property Label ƒ | Target Object Text ƒ | Target is Conclusion ƒ | Target Subject Node Key ƒ | Target Object Node Key ƒ | Target First Fired Order Key ƒ | First Words ƒ | Second Words ƒ | First is in Effect ƒ | First Holds Before Chain ƒ | Second Holds Before Chain ƒ | First Holds Before Typing ƒ | First Property ƒ | First Property Domain Class ƒ | First Property Range Class ƒ | First Sort Order ƒ | Second Sort Order ƒ | Inverse Fired Unit ƒ | Symmetric Fired Unit ƒ | Chain Fired Unit ƒ | Type Fired Unit ƒ | Fired Unit ƒ | Order Key ƒ | Is Chosen ƒ | Chosen Unit ƒ | Card Text ƒ | Name ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| false | inverse rule | 1 | Randy is child of Marvin | Randy | is child of | Marvin | false | Randy | Marvin | — | Marvin is parent of Randy |    | true | true | false | true | isParentOf | — | — | 1 | — | 0 | 0 | 0 | 0 | 0 | 101000 | false | 0 | — | inverse:Marvin\|isParentOf\|Randy>Randy\|isChildOf\|Marvin |
| false | inverse rule | 1 | Sharon is child of Marvin | Sharon | is child of | Marvin | false | Sharon | Marvin | — | Marvin is parent of Sharon |    | false | false | false | false | isParentOf | — | — | 101 | — | 0 | 0 | 0 | 0 | 0 | 10101000 | false | 0 | — | inverse:Marvin\|isParentOf\|Sharon>Sharon\|isChildOf\|Marvin |
| false | inverse rule | 1 | Stan is child of Marvin | Stan | is child of | Marvin | false | Stan | Marvin | — | Marvin is parent of Stan |    | false | false | false | false | isParentOf | — | — | 102 | — | 0 | 0 | 0 | 0 | 0 | 10201000 | false | 0 | — | inverse:Marvin\|isParentOf\|Stan>Stan\|isChildOf\|Marvin |

_ƒ は計算列を示します。_

</details>

| 用語 | 説明 | 補足コメント |
|------|-------------|-------------------|
| **Disjoint Check** | Every pair the disjointness rule watches: the same two people linked by two disjoint properties. | — |
| Graph | 定義済みの属性。 | _The graph._ |
| Rule | 定義済みの属性。 | _The rule switch._ |
| First | 定義済みの属性。 | _The first link._ |
| Second | 定義済みの属性。 | _The disjoint link between the same pair, in the same direction._ |
| Rule Enabled | リンクされた「rule」が「Enabled」であるとき真。 | _Order 1. Whether the disjointness rule is on._ |
| First Holds | リンクされた「first」が「Holds」であるとき真。 | _Order 16. Whether the first link holds._ |
| Second Holds | リンクされた「second」が「Holds」であるとき真。 | _Order 16. Whether the second link holds._ |
| Subject Label | リンクされた「first」から取得。 | _Order 2. The person recorded with both links._ |
| Object Label | リンクされた「first」から取得。 | _Order 2. The other person._ |
| Violation Unit | 優先順位で決定: 次のすべてが成り立つ: フラグ「rule enabled」が設定されている、フラグ「first holds」が設定されている、およびフラグ「second holds」が設定されている場合は 1; それ以外の場合は 0。 | _Order 17. 1 when both links hold._ |
| Alarm Text | 優先順位で決定: violation unit が 1 である場合は 「Contradiction found. 」、続いて subject label、続いて 「 is recorded as both parent and spouse of 」、続いて object label、続いて 「. Check those facts and the selected disjoint rule.」; それ以外の場合は 空文字列。 | _Order 18. The page's contradiction alert, blank when there is none._ |
| Name | disjoint check ID と同じ。 | _Order 1. The check's key._ |

<details open>
<summary>サンプルデータ</summary>

| Rule Enabled ƒ | First Holds ƒ | Second Holds ƒ | Subject Label ƒ | Object Label ƒ | Violation Unit ƒ | Alarm Text ƒ | Name ƒ |
|---|---|---|---|---|---|---|---|
| false | true | false | Marvin | Randy | 0 | — | Marvin\|isParentOf\|Randy~isSpouseOf |
| false | false | false | Marvin | Sharon | 0 | — | Marvin\|isParentOf\|Sharon~isSpouseOf |
| false | false | false | Marvin | Stan | 0 | — | Marvin\|isParentOf\|Stan~isSpouseOf |

_ƒ は計算列を示します。_

</details>

| 用語 | 説明 | 補足コメント |
|------|-------------|-------------------|
| **Scenario** | The page's buttons. Each one is a fact baseline, explicit fact changes and rule settings, applied in that order. | — |
| Graph | 定義済みの属性。 | _The graph._ |
| Label | 定義済みの属性。 | _The button's text._ |
| Note | 定義済みの属性。 | _The note shown under the buttons after this one is pressed; blank clears it._ |
| Sort Order | 定義済みの属性。 | _Position among the buttons._ |
| Button Group | 定義済みの属性。 | _'stan' for the two imagined changes, 'facts' for the buttons under the fact list._ |
| Is Primary | 空文字列とき真。 | _Drawn as the filled button._ |
| Fact Baseline | 定義済みの属性。 | _What happens to every listed fact first: keep, built-in (teaching facts on, others off), built-in-detail, all-off, activate-all (each switched on in list order), or next (the next fact on)._ |
| Layout | 定義済みの属性。 | _The node layout the graph returns to; blank keeps the current positions._ |
| Clears Selection | 空文字列とき真。 | _Clears the selected node or line._ |
| Clears Form Message | 空文字列とき真。 | _Clears the Add fact form's message._ |
| Name | label と同じ。 | _Order 1. The button's text._ |
| Graph Has Facts to Add | リンクされた「graph」が「Facts to Add」を持つとき真。 | _Order 7. Whether a listed fact can still be added._ |
| Is Available | 次のいずれかが成り立つ: fact baseline が 「next」 でないまたはフラグ「graph has facts to add」が設定されているとき真。 | _Order 8. False only for Add next fact once every listed fact is in effect._ |

<details open>
<summary>サンプルデータ</summary>

| Label | Note | Sort Order | Button Group | Is Primary | Fact Baseline | Layout | Clears Selection | Clears Form Message | Name ƒ | Graph Has Facts to Add ƒ | Is Available ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|
| Move Stan to grade 5 | This imagined change updates Stan’s grade to 5. His identifier, Person statement and family links stay the same. His other selected details are unchanged. | 1 | stan | false | keep | — | true | true | Move Stan to grade 5 | false | true |
| Stan leaves school | This imagined change clears Stan’s current school details. His identifier, Person statement and family links remain. Removing a current fact does not say that its opposite is true; school history needs dates. | 2 | stan | false | keep | — | true | true | Stan leaves school | false | true |
| Add next fact | — | 3 | facts | true | next | — | false | false | Add next fact | false | false |

_ƒ は計算列を示します。_

</details>

| 用語 | 説明 | 補足コメント |
|------|-------------|-------------------|
| **Scenario Fact Change** | Fact changes a button makes after its baseline. A blank object key matches every listed value of the property. | — |
| Scenario | 定義済みの属性。 | _The button._ |
| Subject | 定義済みの属性。 | _The subject whose facts change._ |
| Property | 定義済みの属性。 | _The property whose facts change._ |
| Object Key | 定義済みの属性。 | _The object thing's key or the literal value; blank for every listed value._ |
| Action | 定義済みの属性。 | _'on' switches the fact on as the newest value; 'off' switches it off._ |
| Adds to List | 空文字列とき真。 | _Adds the fact to the fact list first when it is not listed._ |
| Sort Order | 定義済みの属性。 | _Order of the change within the button._ |
| Name | scenario fact change ID と同じ。 | _Order 1. The change's key._ |

<details open>
<summary>サンプルデータ</summary>

| Object Key | Action | Adds to List | Sort Order | Name ƒ |
|---|---|---|---|---|
| Person | on | true | 1 | stan-grade-five:Stan\|isA\|Person |
| 5 | on | true | 2 | stan-grade-five:Stan\|hasGradeLevel\|5 |
| Person | on | true | 3 | stan-leaves-school:Stan\|isA\|Person |

_ƒ は計算列を示します。_

</details>

| 用語 | 説明 | 補足コメント |
|------|-------------|-------------------|
| **Scenario Rule Setting** | Rule switches a button sets. | — |
| Scenario | 定義済みの属性。 | _The button._ |
| Rule | 定義済みの属性。 | _The rule switch._ |
| Is Enabled | 空文字列とき真。 | _The state the button puts the switch in._ |
| Name | scenario rule setting ID と同じ。 | _Order 1. The setting's key._ |

<details open>
<summary>サンプルデータ</summary>

| Is Enabled | Name ƒ |
|---|---|
| false | reset-original:inverse |
| false | reset-original:symmetric |
| false | reset-original:chain |

_ƒ は計算列を示します。_

</details>

| 用語 | 説明 | 補足コメント |
|------|-------------|-------------------|
| **Node Layout** | Node positions in the graph's 380 by 480 drawing, one set per layout. | — |
| Layout | 定義済みの属性。 | _The layout: 'default' or 'stan-details'._ |
| Node Key | 定義済みの属性。 | _The node: a thing's key, or literal\|value\|datatype._ |
| X | 定義済みの属性。 | _Horizontal position._ |
| Y | 定義済みの属性。 | _Vertical position._ |
| Name | node layout ID と同じ。 | _Order 1. The position's key._ |

<details open>
<summary>サンプルデータ</summary>

| Layout | Node Key | X | Y | Name ƒ |
|---|---|---|---|---|
| default | Marvin | 70 | 45 | default:Marvin |
| default | Randy | 195 | 125 | default:Randy |
| default | Sharon | 318 | 55 | default:Sharon |

_ƒ は計算列を示します。_

</details>

| 用語 | 説明 | 補足コメント |
|------|-------------|-------------------|
| **Layout Slot** | Places for nodes no layout positions, used in turn. | — |
| Sort Order | 定義済みの属性。 | _Order in which slots are used._ |
| X | 定義済みの属性。 | _Horizontal position._ |
| Y | 定義済みの属性。 | _Vertical position._ |
| Name | layout slot ID と同じ。 | _Order 1. The slot's key._ |

<details open>
<summary>サンプルデータ</summary>

| Sort Order | X | Y | Name ƒ |
|---|---|---|---|
| 1 | 330 | 135 | slot-1 |
| 2 | 45 | 120 | slot-2 |
| 3 | 330 | 400 | slot-3 |

_ƒ は計算列を示します。_

</details>

| 用語 | 説明 | 補足コメント |
|------|-------------|-------------------|
| **Page Text** | The page's wording, one row per string. {name}-style placeholders are filled by the page. | — |
| Text | 定義済みの属性。 | _The string._ |
| Name | page text ID と同じ。 | _Order 1. The string's key._ |

<details open>
<summary>サンプルデータ</summary>

| Text | Name ƒ |
|---|---|
| South Park Ontology Explainer, on one rulebook | page-title |
| A rulebook rebuild of the explainer by | byline-prefix |
| Jessica Talisman | byline-author |

_ƒ は計算列を示します。_

</details>

## 2 ファクトタイプ

- **firing kind**は**rule**をちょうど 1 件参照します
- **property**は**property**を 1 件参照できます
- **property**は**thing**を 1 件参照できます
- **statement**は**graph**をちょうど 1 件参照します
- **statement**は**thing**をちょうど 1 件参照します
- **statement**は**property**をちょうど 1 件参照します
- **firing**は**firing kind**をちょうど 1 件参照します
- **firing**は**rule**をちょうど 1 件参照します
- **firing**は**statement**をちょうど 1 件参照します
- **disjoint check**は**graph**をちょうど 1 件参照します
- **disjoint check**は**rule**をちょうど 1 件参照します
- **disjoint check**は**statement**をちょうど 1 件参照します
- **scenario**は**graph**をちょうど 1 件参照します
- **scenario fact change**は**scenario**をちょうど 1 件参照します
- **scenario fact change**は**thing**をちょうど 1 件参照します
- **scenario fact change**は**property**をちょうど 1 件参照します
- **scenario rule setting**は**scenario**をちょうど 1 件参照します
- **scenario rule setting**は**rule**をちょうど 1 件参照します

## 3 運用ルール

_運用ルールは、ビジネスが**義務付ける**こと、**禁止する**こと、
**推奨する**ことを述べます。構造ルールは必須フィールドと外部キーから、
意味ルールは Constraints テーブルから得られ、いずれもルールブックが既に計算する
ブール値に基づきます(下の定義ルールで DR-N として相互参照)。_

### 構造的制約(スキーマ由来)

- graphは label、literal quote、literal type marker、built in source text、user source text、およびno conclusions text を持た**なければなりません**。
- ruleは label、explanation、およびsort order を持た、かつ 「Enabled」であるかどうかを記録し**なければなりません**。
- firing kindは「rule」をちょうど 1 件参照し**なければなりません**。
- firing kindは wordingおよびsort order を持た**なければなりません**。
- thingは label、kind、およびsort order を持た**なければなりません**。
- propertyは label、property kind、およびsort order を持た、かつ 「User Addable」であるかどうか、「One Current Value」を持つかどうか、および「Symmetric」であるかどうかを記録し**なければなりません**。
- statementは「graph」をちょうど 1 件参照し**なければなりません**。
- statementは、subjectとして「thing」をちょうど 1 件参照し**なければなりません**。
- statementは「property」をちょうど 1 件参照し**なければなりません**。
- statementは activation seqおよびsort order を持た、かつ 「Listed」であるかどうか、「Built in」であるかどうか、「Detail」であるかどうか、および「Stated」であるかどうかを記録し**なければなりません**。
- firingは、kindとして「firing kind」をちょうど 1 件参照し**なければなりません**。
- firingは「rule」をちょうど 1 件参照し**なければなりません**。
- firingは、targetとして「statement」をちょうど 1 件参照し**なければなりません**。
- firingは、firstとして「statement」をちょうど 1 件参照し**なければなりません**。
- disjoint checkは「graph」をちょうど 1 件参照し**なければなりません**。
- disjoint checkは「rule」をちょうど 1 件参照し**なければなりません**。
- disjoint checkは、firstとして「statement」をちょうど 1 件参照し**なければなりません**。
- disjoint checkは、secondとして「statement」をちょうど 1 件参照し**なければなりません**。
- scenarioは「graph」をちょうど 1 件参照し**なければなりません**。
- scenarioは label、sort order、button group、およびfact baseline を持た、かつ 「Primary」であるかどうか、「Clears Selection」であるかどうか、および「Clears Form Message」であるかどうかを記録し**なければなりません**。
- scenario fact changeは「scenario」をちょうど 1 件参照し**なければなりません**。
- scenario fact changeは、subjectとして「thing」をちょうど 1 件参照し**なければなりません**。
- scenario fact changeは「property」をちょうど 1 件参照し**なければなりません**。
- scenario fact changeは actionおよびsort order を持た、かつ 「Adds to List」であるかどうかを記録し**なければなりません**。
- scenario rule settingは「scenario」をちょうど 1 件参照し**なければなりません**。
- scenario rule settingは「rule」をちょうど 1 件参照し**なければなりません**。
- scenario rule settingは 「Enabled」であるかどうかを記録し**なければなりません**。
- node layoutは layout、node key、x、およびy を持た**なければなりません**。
- layout slotは sort order、x、およびy を持た**なければなりません**。
- page textは text を持た**なければなりません**。

## 4 定義ルール

_すべての文はビジネスドメインの真実を表し、手続きでも命令でもありません。
一方向の必要条件を同値と取り違えないよう、「〜の場合に限り」を用います。
**⚠︎ 機械的** マークは、決定的な文言が忠実ながらぎこちないルールを示します —
任意の書き直しの合図であり、欠陥ではありません。_

| ID | 宣言的ルール |
|----|------------------|
| **DR-1 Name** | graphの「name」は label と同じです。 |
| **DR-2 Count of Stated Facts** | graphの「count of stated facts」は graphに関連する「statements」全体での「in effect unit」の合計 です。 |
| **DR-3 Count of Conclusions** | graphの「count of conclusions」は graphに関連する「statements」全体での「conclusion unit」の合計 です。 |
| **DR-4 Count of Violations** | graphの「count of violations」は graphに関連する「disjoint checks」全体での「violation unit」の合計 です。 |
| **DR-5 Is Consistent** | count of violations が 0 である場合、graphは「Consistent」と見なされます。 |
| **DR-6 Conclusions Message** | Graphの「conclusions message」は、次の優先順位で決まります:<br>1. count of conclusions が 0 である場合は no conclusions text;<br>2. それ以外の場合は 空文字列。 |
| **DR-7 Count of Facts to Add** | graphの「count of facts to add」は graphに関連する「statements」全体での「listed off unit」の合計 です。 |
| **DR-8 Has Facts to Add** | count of facts to add が 0 より大きい場合、graphは「Facts to Add」を持つと見なされます。 |
| **DR-9 Next Fact Sort Order** | graphの「next fact sort order」は graphの「statements」のうち「Listed Off Unit」であるものについての「sort order」の最小値 です。 |
| **DR-10 Max Activation Seq** | graphの「max activation seq」は graphに関連する「statements」全体での「activation seq」の最大値 です。 |
| **DR-11 Next Activation Seq** | graphの「next activation seq」は max activation seq たす 1 として計算されます。 |
| **DR-12 Max Listed Sort Order** | graphの「max listed sort order」は graphの「statements」のうち「Listed Unit」であるものについての「sort order」の最大値 です。 |
| **DR-13 Next Listed Sort Order** | graphの「next listed sort order」は max listed sort order たす 1 として計算されます。 |
| **DR-14 Name** | ruleの「name」は label と同じです。 |
| **DR-15 Count of Firings** | ruleの「count of firings」は ruleに関連する「firings」全体での「fired unit」の合計 です。 |
| **DR-16 Name** | firing kindの「name」は wording と同じです。 |
| **DR-17 Rule Enabled** | firing kindの「rule enabled」は、リンクされた「rule」が「Enabled」であるとき真になります。 |
| **DR-18 Name** | thingの「name」は label と同じです。 |
| **DR-19 Node Key** | thingの「node key」は thing ID と同じです。 |
| **DR-20 Count of Children** | thingの「count of children」は thingに関連する「statements」全体での「parent unit」の合計 です。 |
| **DR-21 Count of Grandchildren** | thingの「count of grandchildren」は thingに関連する「statements」全体での「grandparent unit」の合計 です。 |
| **DR-22 Count As Place** | thingの「count as place」は thingに関連する「statements」全体での「place unit」の合計 です。 |
| **DR-23 Count As Subject** | thingの「count as subject」は thingに関連する「statements」全体での「holds unit」の合計 です。 |
| **DR-24 Count As Object** | thingの「count as object」は thingに関連する「statements」全体での「holds unit」の合計 です。 |
| **DR-25 Is in Graph** | 次のいずれかが成り立つ: count as subject が 0 より大きいまたはcount as object が 0 より大きい場合、thingは「In Graph」と見なされます。 |
| **DR-26 Node Kind** | Thingの「node kind」は、次の優先順位で決まります:<br>1. kind が 「class」 である場合は 「class」;<br>2. count as place が 0 より大きい場合は 「place」;<br>3. それ以外の場合は 「person」。 |
| **DR-27 Name** | propertyの「name」は label と同じです。 |
| **DR-28 Subject Label** | statementの「subject label」— リンクされた「subject」から取得されます。 |
| **DR-29 Property Label** | statementの「property label」— リンクされた「property」から取得されます。 |
| **DR-30 Object Label** | statementの「object label」— リンクされた「object thing」から取得されます。 |
| **DR-31 Property Datatype** | statementの「property datatype」— リンクされた「property」から取得されます。 |
| **DR-32 Property Has One Current Value** | statementの「property has one current value」は、リンクされた「property」が「One Current Value」を持つとき真になります。 |
| **DR-33 Property Domain Class** | statementの「property domain class」— リンクされた「property」から取得されます。 |
| **DR-34 Property Range Class** | statementの「property range class」— リンクされた「property」から取得されます。 |
| **DR-35 Literal Quote** | statementの「literal quote」— リンクされた「graph」から取得されます。 |
| **DR-36 Literal Type Marker** | statementの「literal type marker」— リンクされた「graph」から取得されます。 |
| **DR-37 Built in Source Text** | statementの「built in source text」— リンクされた「graph」から取得されます。 |
| **DR-38 User Source Text** | statementの「user source text」— リンクされた「graph」から取得されます。 |
| **DR-39 Next Fact Sort Order** | statementの「next fact sort order」— リンクされた「graph」から取得されます。 |
| **DR-40 Is Literal** | object thing が空である場合、statementは「Literal」と見なされます。 |
| **DR-41 Object Text** | Statementの「object text」は、次の優先順位で決まります:<br>1. object thing が空である場合は object value;<br>2. それ以外の場合は object label。 |
| **DR-42 Name** | statementの「name」は subject label、続いて 空白 1 つ、続いて property label、続いて 空白 1 つ、続いて object text として計算されます。 |
| **DR-43 Words** | statementの「words」は subject label、続いて 空白 1 つ、続いて property label、続いて 空白 1 つ、続いて object thing が空である場合は literal quote、続いて object value、続いて literal quote、続いて literal type marker、続いて property datatype、それ以外の場合は object label として計算されます。 ⚠︎ 機械的 <!-- rulespeak:reword --> |
| **DR-44 Source** | Statementの「source」は、次の優先順位で決まります:<br>1. フラグ「built in」が設定されている場合は built in source text;<br>2. それ以外の場合は user source text。 |
| **DR-45 Subject Property Key** | statementの「subject property key」は subject、続いて 「|」、続いて property として計算されます。 |
| **DR-46 Latest Sibling Activation Seq** | statementの「latest sibling activation seq」は statementに関連する「statements」全体での「activation seq」の最大値 です。 |
| **DR-47 Is in Effect** | 次のすべてが成り立つ: フラグ「stated」が設定されているおよび次のいずれかが成り立つ: フラグ「property has one current value」が設定されていないまたはactivation seq が latest sibling activation seq である場合、statementは「In Effect」と見なされます。 |
| **DR-48 Listed Unit** | Statementの「listed unit」は、次の優先順位で決まります:<br>1. フラグ「listed」が設定されている場合は 1;<br>2. それ以外の場合は 0。 |
| **DR-49 In Effect Unit** | Statementの「in effect unit」は、次の優先順位で決まります:<br>1. フラグ「in effect」が設定されている場合は 1;<br>2. それ以外の場合は 0。 |
| **DR-50 Listed Off Unit** | Statementの「listed off unit」は、次の優先順位で決まります:<br>1. 次のすべてが成り立つ: フラグ「listed」が設定されているおよびフラグ「in effect」が設定されていない場合は 1;<br>2. それ以外の場合は 0。 |
| **DR-51 Is Next to Add** | 次のすべてが成り立つ: listed off unit が 1 であるおよびsort order が next fact sort order である場合、statementは「Next to Add」と見なされます。 |
| **DR-52 Count of Inverse Firings** | statementの「count of inverse firings」は statementに関連する「firings」全体での「inverse fired unit」の合計 です。 |
| **DR-53 Count of Symmetric Firings** | statementの「count of symmetric firings」は statementに関連する「firings」全体での「symmetric fired unit」の合計 です。 |
| **DR-54 Holds Before Chain** | 次のいずれかが成り立つ: フラグ「in effect」が設定されている、count of inverse firings が 0 より大きい、またはcount of symmetric firings が 0 より大きい場合、statementは「Before Chain」を満たすと見なされます。 |
| **DR-55 Count of Chain Firings** | statementの「count of chain firings」は statementに関連する「firings」全体での「chain fired unit」の合計 です。 |
| **DR-56 Holds Before Typing** | 次のいずれかが成り立つ: フラグ「holds before chain」が設定されているまたはcount of chain firings が 0 より大きい場合、statementは「Before Typing」を満たすと見なされます。 |
| **DR-57 Count of Type Firings** | statementの「count of type firings」は statementに関連する「firings」全体での「type fired unit」の合計 です。 |
| **DR-58 Holds** | 次のいずれかが成り立つ: フラグ「holds before typing」が設定されているまたはcount of type firings が 0 より大きい場合、statementは「」を満たすと見なされます。 |
| **DR-59 Is Conclusion** | 次のすべてが成り立つ: フラグ「holds」が設定されているおよびフラグ「in effect」が設定されていない場合、statementは「Conclusion」と見なされます。 |
| **DR-60 Holds Unit** | Statementの「holds unit」は、次の優先順位で決まります:<br>1. フラグ「holds」が設定されている場合は 1;<br>2. それ以外の場合は 0。 |
| **DR-61 Conclusion Unit** | Statementの「conclusion unit」は、次の優先順位で決まります:<br>1. フラグ「conclusion」が設定されている場合は 1;<br>2. それ以外の場合は 0。 |
| **DR-62 Parent Unit** | Statementの「parent unit」は、次の優先順位で決まります:<br>1. 次のすべてが成り立つ: フラグ「holds」が設定されているおよびproperty が 「isParentOf」 である場合は 1;<br>2. それ以外の場合は 0。 |
| **DR-63 Grandparent Unit** | Statementの「grandparent unit」は、次の優先順位で決まります:<br>1. 次のすべてが成り立つ: フラグ「holds」が設定されているおよびproperty が 「isGrandparentOf」 である場合は 1;<br>2. それ以外の場合は 0。 |
| **DR-64 Place Unit** | Statementの「place unit」は、次の優先順位で決まります:<br>1. 次のすべてが成り立つ: フラグ「holds」が設定されているおよびproperty が 「attends」 である場合は 1;<br>2. それ以外の場合は 0。 |
| **DR-65 First Fired Order Key** | statementの「first fired order key」は statementの「firings」のうち「Fired Unit」であるものについての「order key」の最小値 です。 |
| **DR-66 Count of Chosen Firings** | statementの「count of chosen firings」は statementに関連する「firings」全体での「chosen unit」の合計 です。 |
| **DR-67 Subject Node Key** | statementの「subject node key」は subject と同じです。 |
| **DR-68 Object Node Key** | Statementの「object node key」は、次の優先順位で決まります:<br>1. object thing が空である場合は 「literal|」、続いて object value、続いて 「|」、続いて property datatype;<br>2. それ以外の場合は object thing。 |
| **DR-69 Subject Node Kind** | statementの「subject node kind」— リンクされた「subject」から取得されます。 |
| **DR-70 Object Thing Node Kind** | statementの「object thing node kind」— リンクされた「object thing」から取得されます。 |
| **DR-71 Object Node Kind** | Statementの「object node kind」は、次の優先順位で決まります:<br>1. object thing が空である場合は 「value」;<br>2. それ以外の場合は object thing node kind。 |
| **DR-72 Object Value Node Label** | statementの「object value node label」— リンクされた「property」から取得されます。 |
| **DR-73 Count of Violations As First** | statementの「count of violations as first」は statementに関連する「disjoint checks」全体での「violation unit」の合計 です。 |
| **DR-74 Count of Violations As Second** | statementの「count of violations as second」は statementに関連する「disjoint checks」全体での「violation unit」の合計 です。 |
| **DR-75 Is in Contradiction** | 次のいずれかが成り立つ: count of violations as first が 0 より大きいまたはcount of violations as second が 0 より大きい場合、statementは「In Contradiction」と見なされます。 |
| **DR-76 Rule Enabled** | firingの「rule enabled」は、リンクされた「rule」が「Enabled」であるとき真になります。 |
| **DR-77 Kind Wording** | firingの「kind wording」— リンクされた「kind」から取得されます。 |
| **DR-78 Kind Sort Order** | firingの「kind sort order」— リンクされた「kind」から取得されます。 |
| **DR-79 Target Name** | firingの「target name」— リンクされた「target」から取得されます。 |
| **DR-80 Target Subject Label** | firingの「target subject label」— リンクされた「target」から取得されます。 |
| **DR-81 Target Property Label** | firingの「target property label」— リンクされた「target」から取得されます。 |
| **DR-82 Target Object Text** | firingの「target object text」— リンクされた「target」から取得されます。 |
| **DR-83 Target is Conclusion** | firingの「target is conclusion」は、リンクされた「target」が「Conclusion」であるとき真になります。 |
| **DR-84 Target Subject Node Key** | firingの「target subject node key」— リンクされた「target」から取得されます。 |
| **DR-85 Target Object Node Key** | firingの「target object node key」— リンクされた「target」から取得されます。 |
| **DR-86 Target First Fired Order Key** | firingの「target first fired order key」— リンクされた「target」から取得されます。 |
| **DR-87 First Words** | firingの「first words」— リンクされた「first」から取得されます。 |
| **DR-88 Second Words** | firingの「second words」— リンクされた「second」から取得されます。 |
| **DR-89 First is in Effect** | firingの「first is in effect」は、リンクされた「first」が「In Effect」であるとき真になります。 |
| **DR-90 First Holds Before Chain** | firingの「first holds before chain」は、リンクされた「first」が「Holds Before Chain」であるとき真になります。 |
| **DR-91 Second Holds Before Chain** | firingの「second holds before chain」は、リンクされた「second」が「Holds Before Chain」であるとき真になります。 |
| **DR-92 First Holds Before Typing** | firingの「first holds before typing」は、リンクされた「first」が「Holds Before Typing」であるとき真になります。 |
| **DR-93 First Property** | firingの「first property」— リンクされた「first」から取得されます。 |
| **DR-94 First Property Domain Class** | firingの「first property domain class」— リンクされた「first」から取得されます。 |
| **DR-95 First Property Range Class** | firingの「first property range class」— リンクされた「first」から取得されます。 |
| **DR-96 First Sort Order** | firingの「first sort order」— リンクされた「first」から取得されます。 |
| **DR-97 Second Sort Order** | firingの「second sort order」— リンクされた「second」から取得されます。 |
| **DR-98 Inverse Fired Unit** | Firingの「inverse fired unit」は、次の優先順位で決まります:<br>1. 次のすべてが成り立つ: フラグ「rule enabled」が設定されている、kind が 「inverse」 である、およびフラグ「first is in effect」が設定されている場合は 1;<br>2. それ以外の場合は 0。 |
| **DR-99 Symmetric Fired Unit** | Firingの「symmetric fired unit」は、次の優先順位で決まります:<br>1. 次のすべてが成り立つ: フラグ「rule enabled」が設定されている、kind が 「symmetric」 である、およびフラグ「first is in effect」が設定されている場合は 1;<br>2. それ以外の場合は 0。 |
| **DR-100 Chain Fired Unit** | Firingの「chain fired unit」は、次の優先順位で決まります:<br>1. 次のすべてが成り立つ: フラグ「rule enabled」が設定されている、kind が 「chain」 である、フラグ「first holds before chain」が設定されている、およびフラグ「second holds before chain」が設定されている場合は 1;<br>2. それ以外の場合は 0。 |
| **DR-101 Type Fired Unit** | Firingの「type fired unit」は、次の優先順位で決まります:<br>1. 次のすべてが成り立つ: フラグ「rule enabled」が設定されている、次のいずれかが成り立つ: kind が 「domain」 であるまたはkind が 「range」 である、およびフラグ「first holds before typing」が設定されている場合は 1;<br>2. それ以外の場合は 0。 |
| **DR-102 Fired Unit** | firingの「fired unit」は inverse fired unit たす symmetric fired unit たす chain fired unit たす type fired unit として計算されます。 |
| **DR-103 Order Key** | firingの「order key」は first sort order かける 100000 たす kind sort order かける 1000 たす second が空である場合は 0、それ以外の場合は second sort order として計算されます。 |
| **DR-104 Is Chosen** | 次のすべてが成り立つ: fired unit が 1 である、フラグ「target is conclusion」が設定されている、およびorder key が target first fired order key である場合、firingは「Chosen」と見なされます。 |
| **DR-105 Chosen Unit** | Firingの「chosen unit」は、次の優先順位で決まります:<br>1. フラグ「chosen」が設定されている場合は 1;<br>2. それ以外の場合は 0。 |
| **DR-106 Card Text** | Firingの「card text」は、次の優先順位で決まります:<br>1. フラグ「chosen」が設定されている場合は 「Worked out by the 」、続いて kind wording、続いて 「 from “」、続いて first words、続いて 「”」、続いて kind が 「chain」 である場合は 「 and “」、続いて second words、続いて 「”」、それ以外の場合は kind が 「domain」 である場合は 「 and “」、続いて first property、続いて 「 has domain 」、続いて first property domain class、続いて 「”」、それ以外の場合は 「 and “」、続いて first property、続いて 「 has range 」、続いて first property range class、続いて 「”」、続いて ピリオド;<br>2. それ以外の場合は 空文字列。 ⚠︎ 機械的 <!-- rulespeak:reword --> |
| **DR-107 Name** | firingの「name」は firing ID と同じです。 |
| **DR-108 Rule Enabled** | disjoint checkの「rule enabled」は、リンクされた「rule」が「Enabled」であるとき真になります。 |
| **DR-109 First Holds** | disjoint checkの「first holds」は、リンクされた「first」が「Holds」であるとき真になります。 |
| **DR-110 Second Holds** | disjoint checkの「second holds」は、リンクされた「second」が「Holds」であるとき真になります。 |
| **DR-111 Subject Label** | disjoint checkの「subject label」— リンクされた「first」から取得されます。 |
| **DR-112 Object Label** | disjoint checkの「object label」— リンクされた「first」から取得されます。 |
| **DR-113 Violation Unit** | Disjoint checkの「violation unit」は、次の優先順位で決まります:<br>1. 次のすべてが成り立つ: フラグ「rule enabled」が設定されている、フラグ「first holds」が設定されている、およびフラグ「second holds」が設定されている場合は 1;<br>2. それ以外の場合は 0。 |
| **DR-114 Alarm Text** | Disjoint checkの「alarm text」は、次の優先順位で決まります:<br>1. violation unit が 1 である場合は 「Contradiction found. 」、続いて subject label、続いて 「 is recorded as both parent and spouse of 」、続いて object label、続いて 「. Check those facts and the selected disjoint rule.」;<br>2. それ以外の場合は 空文字列。 |
| **DR-115 Name** | disjoint checkの「name」は disjoint check ID と同じです。 |
| **DR-116 Name** | scenarioの「name」は label と同じです。 |
| **DR-117 Graph Has Facts to Add** | scenarioの「graph has facts to add」は、リンクされた「graph」が「Facts to Add」を持つとき真になります。 |
| **DR-118 Is Available** | 次のいずれかが成り立つ: fact baseline が 「next」 でないまたはフラグ「graph has facts to add」が設定されている場合、scenarioは「Available」と見なされます。 |
| **DR-119 Name** | scenario fact changeの「name」は scenario fact change ID と同じです。 |
| **DR-120 Name** | scenario rule settingの「name」は scenario rule setting ID と同じです。 |
| **DR-121 Name** | node layoutの「name」は node layout ID と同じです。 |
| **DR-122 Name** | layout slotの「name」は layout slot ID と同じです。 |
| **DR-123 Name** | page textの「name」は page text ID と同じです。 |

## 5 スキーマへのトレーサビリティ

_式の列は ドキュメント 記法によるルールの定義です —
ルールブックが保持するのと同じロジックを、ビジネス読者向けに記したものです。_

| スキーマ要素 | 種別 | 式 |
|----------------|------|------------|
| **Graphs.Name** | 数式 | `Label` |
| **Graphs.CountOfStatedFacts** | 集計 | `Sum(Statements.InEffectUnit via Graph)` |
| **Graphs.CountOfConclusions** | 集計 | `Sum(Statements.ConclusionUnit via Graph)` |
| **Graphs.CountOfViolations** | 集計 | `Sum(DisjointChecks.ViolationUnit via Graph)` |
| **Graphs.IsConsistent** | 数式 | `CountOfViolations = 0` |
| **Graphs.ConclusionsMessage** | 数式 | `If(CountOfConclusions = 0, NoConclusionsText, "")` |
| **Graphs.CountOfFactsToAdd** | 集計 | `Sum(Statements.ListedOffUnit via Graph)` |
| **Graphs.HasFactsToAdd** | 数式 | `CountOfFactsToAdd > 0` |
| **Graphs.NextFactSortOrder** | 集計 | `Min(Statements.SortOrder via Graph)` |
| **Graphs.MaxActivationSeq** | 集計 | `Max(Statements.ActivationSeq via Graph)` |
| **Graphs.NextActivationSeq** | 数式 | `MaxActivationSeq + 1` |
| **Graphs.MaxListedSortOrder** | 集計 | `Max(Statements.SortOrder via Graph)` |
| **Graphs.NextListedSortOrder** | 数式 | `MaxListedSortOrder + 1` |
| **Rules.Name** | 数式 | `Label` |
| **Rules.CountOfFirings** | 集計 | `Sum(Firings.FiredUnit via Rule)` |
| **FiringKinds.Name** | 数式 | `Wording` |
| **FiringKinds.RuleEnabled** | 参照 | `Lookup(Rules.IsEnabled via Rule)` |
| **Things.Name** | 数式 | `Label` |
| **Things.NodeKey** | 数式 | `ThingId` |
| **Things.CountOfChildren** | 集計 | `Sum(Statements.ParentUnit via Subject)` |
| **Things.CountOfGrandchildren** | 集計 | `Sum(Statements.GrandparentUnit via Subject)` |
| **Things.CountAsPlace** | 集計 | `Sum(Statements.PlaceUnit via ObjectThing)` |
| **Things.CountAsSubject** | 集計 | `Sum(Statements.HoldsUnit via Subject)` |
| **Things.CountAsObject** | 集計 | `Sum(Statements.HoldsUnit via ObjectThing)` |
| **Things.IsInGraph** | 数式 | `Or(CountAsSubject > 0, CountAsObject > 0)` |
| **Things.NodeKind** | 数式 | `If(Kind = "class", "class", If(CountAsPlace > 0, "place", "person"))` |
| **Properties.Name** | 数式 | `Label` |
| **Statements.SubjectLabel** | 参照 | `Lookup(Things.Label via Subject)` |
| **Statements.PropertyLabel** | 参照 | `Lookup(Properties.Label via Property)` |
| **Statements.ObjectLabel** | 参照 | `Lookup(Things.Label via ObjectThing)` |
| **Statements.PropertyDatatype** | 参照 | `Lookup(Properties.Datatype via Property)` |
| **Statements.PropertyHasOneCurrentValue** | 参照 | `Lookup(Properties.HasOneCurrentValue via Property)` |
| **Statements.PropertyDomainClass** | 参照 | `Lookup(Properties.DomainClass via Property)` |
| **Statements.PropertyRangeClass** | 参照 | `Lookup(Properties.RangeClass via Property)` |
| **Statements.LiteralQuote** | 参照 | `Lookup(Graphs.LiteralQuote via Graph)` |
| **Statements.LiteralTypeMarker** | 参照 | `Lookup(Graphs.LiteralTypeMarker via Graph)` |
| **Statements.BuiltInSourceText** | 参照 | `Lookup(Graphs.BuiltInSourceText via Graph)` |
| **Statements.UserSourceText** | 参照 | `Lookup(Graphs.UserSourceText via Graph)` |
| **Statements.NextFactSortOrder** | 参照 | `Lookup(Graphs.NextFactSortOrder via Graph)` |
| **Statements.IsLiteral** | 数式 | `ObjectThing = ""` |
| **Statements.ObjectText** | 数式 | `If(ObjectThing = "", ObjectValue, ObjectLabel)` |
| **Statements.Name** | 数式 | `Concat(SubjectLabel, " ", PropertyLabel, " ", ObjectText)` |
| **Statements.Words** | 数式 | `Concat(SubjectLabel, " ", PropertyLabel, " ", If(ObjectThing = "", Concat(LiteralQuote, ObjectValue, LiteralQuote, LiteralTypeMarker, PropertyDatatype), ObjectLabel))` |
| **Statements.Source** | 数式 | `If(IsBuiltIn, BuiltInSourceText, UserSourceText)` |
| **Statements.SubjectPropertyKey** | 数式 | `Concat(Subject, "\|", Property)` |
| **Statements.LatestSiblingActivationSeq** | 集計 | `Max(Statements.ActivationSeq via SubjectPropertyKey)` |
| **Statements.IsInEffect** | 数式 | `And(IsStated, Or(Not(PropertyHasOneCurrentValue), ActivationSeq = LatestSiblingActivationSeq))` |
| **Statements.ListedUnit** | 数式 | `If(IsListed, 1, 0)` |
| **Statements.InEffectUnit** | 数式 | `If(IsInEffect, 1, 0)` |
| **Statements.ListedOffUnit** | 数式 | `If(And(IsListed, Not(IsInEffect)), 1, 0)` |
| **Statements.IsNextToAdd** | 数式 | `And(ListedOffUnit = 1, SortOrder = NextFactSortOrder)` |
| **Statements.CountOfInverseFirings** | 集計 | `Sum(Firings.InverseFiredUnit via Target)` |
| **Statements.CountOfSymmetricFirings** | 集計 | `Sum(Firings.SymmetricFiredUnit via Target)` |
| **Statements.HoldsBeforeChain** | 数式 | `Or(IsInEffect, CountOfInverseFirings > 0, CountOfSymmetricFirings > 0)` |
| **Statements.CountOfChainFirings** | 集計 | `Sum(Firings.ChainFiredUnit via Target)` |
| **Statements.HoldsBeforeTyping** | 数式 | `Or(HoldsBeforeChain, CountOfChainFirings > 0)` |
| **Statements.CountOfTypeFirings** | 集計 | `Sum(Firings.TypeFiredUnit via Target)` |
| **Statements.Holds** | 数式 | `Or(HoldsBeforeTyping, CountOfTypeFirings > 0)` |
| **Statements.IsConclusion** | 数式 | `And(Holds, Not(IsInEffect))` |
| **Statements.HoldsUnit** | 数式 | `If(Holds, 1, 0)` |
| **Statements.ConclusionUnit** | 数式 | `If(IsConclusion, 1, 0)` |
| **Statements.ParentUnit** | 数式 | `If(And(Holds, Property = "isParentOf"), 1, 0)` |
| **Statements.GrandparentUnit** | 数式 | `If(And(Holds, Property = "isGrandparentOf"), 1, 0)` |
| **Statements.PlaceUnit** | 数式 | `If(And(Holds, Property = "attends"), 1, 0)` |
| **Statements.FirstFiredOrderKey** | 集計 | `Min(Firings.OrderKey via Target)` |
| **Statements.CountOfChosenFirings** | 集計 | `Sum(Firings.ChosenUnit via Target)` |
| **Statements.SubjectNodeKey** | 数式 | `Subject` |
| **Statements.ObjectNodeKey** | 数式 | `If(ObjectThing = "", Concat("literal\|", ObjectValue, "\|", PropertyDatatype), ObjectThing)` |
| **Statements.SubjectNodeKind** | 参照 | `Lookup(Things.NodeKind via Subject)` |
| **Statements.ObjectThingNodeKind** | 参照 | `Lookup(Things.NodeKind via ObjectThing)` |
| **Statements.ObjectNodeKind** | 数式 | `If(ObjectThing = "", "value", ObjectThingNodeKind)` |
| **Statements.ObjectValueNodeLabel** | 参照 | `Lookup(Properties.ValueNodeLabel via Property)` |
| **Statements.CountOfViolationsAsFirst** | 集計 | `Sum(DisjointChecks.ViolationUnit via First)` |
| **Statements.CountOfViolationsAsSecond** | 集計 | `Sum(DisjointChecks.ViolationUnit via Second)` |
| **Statements.IsInContradiction** | 数式 | `Or(CountOfViolationsAsFirst > 0, CountOfViolationsAsSecond > 0)` |
| **Firings.RuleEnabled** | 参照 | `Lookup(Rules.IsEnabled via Rule)` |
| **Firings.KindWording** | 参照 | `Lookup(FiringKinds.Wording via Kind)` |
| **Firings.KindSortOrder** | 参照 | `Lookup(FiringKinds.SortOrder via Kind)` |
| **Firings.TargetName** | 参照 | `Lookup(Statements.Name via Target)` |
| **Firings.TargetSubjectLabel** | 参照 | `Lookup(Statements.SubjectLabel via Target)` |
| **Firings.TargetPropertyLabel** | 参照 | `Lookup(Statements.PropertyLabel via Target)` |
| **Firings.TargetObjectText** | 参照 | `Lookup(Statements.ObjectText via Target)` |
| **Firings.TargetIsConclusion** | 参照 | `Lookup(Statements.IsConclusion via Target)` |
| **Firings.TargetSubjectNodeKey** | 参照 | `Lookup(Statements.SubjectNodeKey via Target)` |
| **Firings.TargetObjectNodeKey** | 参照 | `Lookup(Statements.ObjectNodeKey via Target)` |
| **Firings.TargetFirstFiredOrderKey** | 参照 | `Lookup(Statements.FirstFiredOrderKey via Target)` |
| **Firings.FirstWords** | 参照 | `Lookup(Statements.Words via First)` |
| **Firings.SecondWords** | 参照 | `Lookup(Statements.Words via Second)` |
| **Firings.FirstIsInEffect** | 参照 | `Lookup(Statements.IsInEffect via First)` |
| **Firings.FirstHoldsBeforeChain** | 参照 | `Lookup(Statements.HoldsBeforeChain via First)` |
| **Firings.SecondHoldsBeforeChain** | 参照 | `Lookup(Statements.HoldsBeforeChain via Second)` |
| **Firings.FirstHoldsBeforeTyping** | 参照 | `Lookup(Statements.HoldsBeforeTyping via First)` |
| **Firings.FirstProperty** | 参照 | `Lookup(Statements.Property via First)` |
| **Firings.FirstPropertyDomainClass** | 参照 | `Lookup(Statements.PropertyDomainClass via First)` |
| **Firings.FirstPropertyRangeClass** | 参照 | `Lookup(Statements.PropertyRangeClass via First)` |
| **Firings.FirstSortOrder** | 参照 | `Lookup(Statements.SortOrder via First)` |
| **Firings.SecondSortOrder** | 参照 | `Lookup(Statements.SortOrder via Second)` |
| **Firings.InverseFiredUnit** | 数式 | `If(And(RuleEnabled, Kind = "inverse", FirstIsInEffect), 1, 0)` |
| **Firings.SymmetricFiredUnit** | 数式 | `If(And(RuleEnabled, Kind = "symmetric", FirstIsInEffect), 1, 0)` |
| **Firings.ChainFiredUnit** | 数式 | `If(And(RuleEnabled, Kind = "chain", FirstHoldsBeforeChain, SecondHoldsBeforeChain), 1, 0)` |
| **Firings.TypeFiredUnit** | 数式 | `If(And(RuleEnabled, Or(Kind = "domain", Kind = "range"), FirstHoldsBeforeTyping), 1, 0)` |
| **Firings.FiredUnit** | 数式 | `InverseFiredUnit + SymmetricFiredUnit + ChainFiredUnit + TypeFiredUnit` |
| **Firings.OrderKey** | 数式 | `FirstSortOrder * 100000 + KindSortOrder * 1000 + If(Second = "", 0, SecondSortOrder)` |
| **Firings.IsChosen** | 数式 | `And(FiredUnit = 1, TargetIsConclusion, OrderKey = TargetFirstFiredOrderKey)` |
| **Firings.ChosenUnit** | 数式 | `If(IsChosen, 1, 0)` |
| **Firings.CardText** | 数式 | `If(IsChosen, Concat("Worked out by the ", KindWording, " from “", FirstWords, "”", If(Kind = "chain", Concat(" and “", SecondWords, "”"), If(Kind = "domain", Concat(" and “", FirstProperty, " has domain ", FirstPropertyDomainClass, "”"), If(Kind = "range", Concat(" and “", FirstProperty, " has range ", FirstPropertyRangeClass, "”"), ""))), "."), "")` |
| **Firings.Name** | 数式 | `FiringId` |
| **DisjointChecks.RuleEnabled** | 参照 | `Lookup(Rules.IsEnabled via Rule)` |
| **DisjointChecks.FirstHolds** | 参照 | `Lookup(Statements.Holds via First)` |
| **DisjointChecks.SecondHolds** | 参照 | `Lookup(Statements.Holds via Second)` |
| **DisjointChecks.SubjectLabel** | 参照 | `Lookup(Statements.SubjectLabel via First)` |
| **DisjointChecks.ObjectLabel** | 参照 | `Lookup(Statements.ObjectLabel via First)` |
| **DisjointChecks.ViolationUnit** | 数式 | `If(And(RuleEnabled, FirstHolds, SecondHolds), 1, 0)` |
| **DisjointChecks.AlarmText** | 数式 | `If(ViolationUnit = 1, Concat("Contradiction found. ", SubjectLabel, " is recorded as both parent and spouse of ", ObjectLabel, ". Check those facts and the selected disjoint rule."), "")` |
| **DisjointChecks.Name** | 数式 | `DisjointCheckId` |
| **Scenarios.Name** | 数式 | `Label` |
| **Scenarios.GraphHasFactsToAdd** | 参照 | `Lookup(Graphs.HasFactsToAdd via Graph)` |
| **Scenarios.IsAvailable** | 数式 | `Or(FactBaseline <> "next", GraphHasFactsToAdd)` |
| **ScenarioFactChanges.Name** | 数式 | `ScenarioFactChangeId` |
| **ScenarioRuleSettings.Name** | 数式 | `ScenarioRuleSettingId` |
| **NodeLayouts.Name** | 数式 | `NodeLayoutId` |
| **LayoutSlots.Name** | 数式 | `LayoutSlotId` |
| **PageTexts.Name** | 数式 | `PageTextId` |

---

> **RuleSpeak®** — **Ronald G. Ross** が考案し、**SBVR** の慣例に従う宣言的なビジネスルール表記 — は近日対応予定です。それまでは、このページに通常のドキュメントを表示します。 [www.RonRoss.info](https://www.RonRoss.info)
