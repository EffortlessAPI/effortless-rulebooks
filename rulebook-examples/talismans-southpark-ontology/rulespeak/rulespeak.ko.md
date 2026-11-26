# 📘 Talisman's South Park ontology explainer — 문서

_Jessica Talisman's South Park Ontology Explainer, as a rulebook: the page's ten facts, five rule switches, property wiring, provenance wording and current-value policy are rows, and every conclusion is a formula. The rules apply in three fixed strata (inverse and symmetric, then the property chain, then domain and range); property wiring that feeds an earlier stratum would need repeated rounds._

> 룰북에서 생성한 선언적 비즈니스 규칙입니다. 아래의 모든 문장은 비즈니스 영역에서
> 성립하는 사실을 서술하며, 절차나 명령이 아닙니다. 룰북의 수식이 단일 진실 공급원이고,
> 이 문서는 그 수식을 평이한 문장으로 풀어 쓴 것입니다.

## 1 비즈니스 용어

| 용어 | 설명 | 부연 설명 |
|------|-------------|-------------------|
| **Graph** | The explainer page as one graph: its wording variables and its totals. | — |
| Label | 정의된 속성입니다. | _Display name of the graph._ |
| Literal Quote | 정의된 속성입니다. | _The quote mark written around a literal value in a fact's wording._ |
| Literal Type Marker | 정의된 속성입니다. | _The marker written between a literal value and its datatype, as in Turtle._ |
| Built in Source Text | 정의된 속성입니다. | _Source line shown for a fact the page states as a teaching example._ |
| User Source Text | 정의된 속성입니다. | _Source line shown for a fact the reader stated._ |
| No Conclusions Text | 정의된 속성입니다. | _Message shown when the selected facts and rules add no conclusion._ |
| Name | 같은 레코드의 ‘label’ 값과 같습니다. | _Order 1. Display name of the graph._ |
| Count of Stated Facts | 해당 ‘graph’ 레코드에 연결된 모든 ‘statements’ 레코드의 총 ‘in effect unit’. | _Order 5. Facts that are switched on and in effect._ |
| Count of Conclusions | 해당 ‘graph’ 레코드에 연결된 모든 ‘statements’ 레코드의 총 ‘conclusion unit’. | _Order 18. Facts that hold only because an enabled rule concluded them._ |
| Count of Violations | 해당 ‘graph’ 레코드에 연결된 모든 ‘disjoint checks’ 레코드의 총 ‘violation unit’. | _Order 18. Disjointness checks that found both links between the same pair._ |
| Is Consistent | ‘count of violations’ 값이 0인 경우에 참입니다. | _Order 19. True when no enabled disjointness rule is violated._ |
| Conclusions Message | 우선순위에 따라 결정됩니다: ‘count of conclusions’ 값이 0인 경우에는 ‘no conclusions text’; 그 밖의 경우에는 빈 문자열. | _Order 19. The empty-state message for the conclusions panel, blank when there are conclusions._ |
| Count of Facts to Add | 해당 ‘graph’ 레코드에 연결된 모든 ‘statements’ 레코드의 총 ‘listed off unit’. | _Order 5. Listed facts that are not in effect; the Add next fact button is enabled while this is above zero._ |
| Has Facts to Add | ‘count of facts to add’ 값이 0보다 큰 경우에 참입니다. | _Order 6. True while some listed fact can still be added._ |
| Next Fact Sort Order | 해당 ‘graph’ 레코드에 연결된 ‘statements’ 레코드 중 ‘listed off unit’에 해당하는 것 전체의 최소 ‘sort order’. | _Order 5. List position of the first listed fact that is not in effect._ |
| Max Activation Seq | 해당 ‘graph’ 레코드에 연결된 모든 ‘statements’ 레코드의 최대 ‘activation seq’. | _Order 1. Highest activation number used so far._ |
| Next Activation Seq | 다음과 같이 계산됩니다: ‘max activation seq’ 더하기 1. | _Order 2. The activation number the next switched-on fact takes._ |
| Max Listed Sort Order | 해당 ‘graph’ 레코드에 연결된 ‘statements’ 레코드 중 ‘listed unit’에 해당하는 것 전체의 최대 ‘sort order’. | _Order 2. Highest list position among listed facts._ |
| Next Listed Sort Order | 다음과 같이 계산됩니다: ‘max listed sort order’ 더하기 1. | _Order 3. The list position a newly added fact takes._ |

<details open>
<summary>예시 데이터</summary>

| Label | Literal Quote | Literal Type Marker | Built in Source Text | User Source Text | No Conclusions Text | Name ƒ | Count of Stated Facts ƒ | Count of Conclusions ƒ | Count of Violations ƒ | Is Consistent ƒ | Conclusions Message ƒ | Count of Facts to Add ƒ | Has Facts to Add ƒ | Next Fact Sort Order ƒ | Max Activation Seq ƒ | Next Activation Seq ƒ | Max Listed Sort Order ƒ | Next Listed Sort Order ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| South Park | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | These facts and selected rules add no further conclusions. Try another fact or rule. | South Park | 10 | 0 | 0 | true | These facts and selected rules add no further conclusions. Try another fact or rule. | 0 | false | — | 10 | 11 | 10 | 11 |

_ƒ 표시는 계산된 열을 나타냅니다._

</details>

| 용어 | 설명 | 부연 설명 |
|------|-------------|-------------------|
| **Rule** | The five rule switches on the page. | — |
| Label | 정의된 속성입니다. | _The rule's name on the page._ |
| Explanation | 정의된 속성입니다. | _The rule's one-line explanation on the page._ |
| Is Enabled | 빈 문자열에 참입니다. | _The rule's checkbox._ |
| Sort Order | 정의된 속성입니다. | _Position in the rule list._ |
| Name | 같은 레코드의 ‘label’ 값과 같습니다. | _Order 1. The rule's name._ |
| Count of Firings | 해당 ‘rule’ 레코드에 연결된 모든 ‘firings’ 레코드의 총 ‘fired unit’. | _Order 15. Rule applications that fired, counting every derivation._ |

<details open>
<summary>예시 데이터</summary>

| Label | Explanation | Is Enabled | Sort Order | Name ƒ | Count of Firings ƒ |
|---|---|---|---|---|---|
| Inverse | A parent link also gives a child link between the same two people. | false | 1 | Inverse | 0 |
| Symmetric | A friend link or a spouse link is true for both people. | false | 2 | Symmetric | 0 |
| Property chain | A parent of a parent is a grandparent. | false | 3 | Property chain | 0 |

_ƒ 표시는 계산된 열을 나타냅니다._

</details>

| 용어 | 설명 | 부연 설명 |
|------|-------------|-------------------|
| **Firing Kind** | The ways a rule can apply. Domain and range belong to one rule but are worded apart. | — |
| Rule | 정의된 속성입니다. | _The rule switch this kind belongs to._ |
| Wording | 정의된 속성입니다. | _How a conclusion card names the rule, as in 'Worked out by the inverse rule'._ |
| Sort Order | 정의된 속성입니다. | _Order in which the page's reasoner tries the kinds for one fact._ |
| Name | 같은 레코드의 ‘wording’ 값과 같습니다. | _Order 1. The kind's wording._ |
| Rule Enabled | 연결된 ‘rule’ 레코드가 ‘enabled’에 해당하는 경우에 참입니다. | _Order 1. Whether the owning rule is switched on._ |

<details open>
<summary>예시 데이터</summary>

| Wording | Sort Order | Name ƒ | Rule Enabled ƒ |
|---|---|---|---|
| inverse rule | 1 | inverse rule | false |
| symmetric rule | 2 | symmetric rule | false |
| property chain rule | 3 | property chain rule | false |

_ƒ 표시는 계산된 열을 나타냅니다._

</details>

| 용어 | 설명 | 부연 설명 |
|------|-------------|-------------------|
| **Thing** | Every individual and class the page can draw. | — |
| Label | 정의된 속성입니다. | _The name the page shows._ |
| Kind | 정의된 속성입니다. | _'individual' or 'class'._ |
| Sort Order | 정의된 속성입니다. | _Position in the cast._ |
| Portrait Name | 정의된 속성입니다. | _The name on the portrait's Fandom page._ |
| Portrait Page | 정의된 속성입니다. | _The Fandom page credited for the portrait._ |
| Portrait Image | 정의된 속성입니다. | _The original Fandom image._ |
| Portrait Crop | 정의된 속성입니다. | _The portrait's crop as an SVG viewBox: x y width height._ |
| Portrait Width | 정의된 속성입니다. | _The original image's width in pixels._ |
| Portrait Height | 정의된 속성입니다. | _The original image's height in pixels._ |
| Name | 같은 레코드의 ‘label’ 값과 같습니다. | _Order 1. The thing's name._ |
| Node Key | 같은 레코드의 ‘thing ID’ 값과 같습니다. | _Order 1. The key the graph draws this thing under._ |
| Count of Children | 해당 ‘thing’ 레코드에 연결된 모든 ‘statements’ 레코드의 총 ‘parent unit’. | _Order 17. People this thing is a parent of, stated or concluded._ |
| Count of Grandchildren | 해당 ‘thing’ 레코드에 연결된 모든 ‘statements’ 레코드의 총 ‘grandparent unit’. | _Order 17. People this thing is a grandparent of._ |
| Count As Place | 해당 ‘thing’ 레코드에 연결된 모든 ‘statements’ 레코드의 총 ‘place unit’. | _Order 17. Holding attends facts whose object is this thing._ |
| Count As Subject | 해당 ‘thing’ 레코드에 연결된 모든 ‘statements’ 레코드의 총 ‘holds unit’. | _Order 17. Holding facts about this thing._ |
| Count As Object | 해당 ‘thing’ 레코드에 연결된 모든 ‘statements’ 레코드의 총 ‘holds unit’. | _Order 17. Holding facts that point at this thing._ |
| Is in Graph | 다음 중 하나 이상에 해당하는 경우(‘count as subject’ 값이 0보다 큰 경우 또는 ‘count as object’ 값이 0보다 큰 경우)에 참입니다. | _Order 18. True when the graph draws this thing._ |
| Node Kind | 우선순위에 따라 결정됩니다: ‘kind’ 값이 “class”인 경우에는 “class”; ‘count as place’ 값이 0보다 큰 경우에는 “place”; 그 밖의 경우에는 “person”. | _Order 18. How the graph draws the thing: a class, a place someone attends, or a person._ |

<details open>
<summary>예시 데이터</summary>

| Label | Kind | Sort Order | Portrait Name | Portrait Page | Portrait Image | Portrait Crop | Portrait Width | Portrait Height | Name ƒ | Node Key ƒ | Count of Children ƒ | Count of Grandchildren ƒ | Count As Place ƒ | Count As Subject ƒ | Count As Object ƒ | Is in Graph ƒ | Node Kind ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| Marvin | individual | 1 | Marvin Marsh | https://southpark.fandom.com/wiki/Marvin_Marsh | https://static.wikia.nocookie.net/southpark/images/d/db/Marsh-family-grandpa-marsh-w-purple-blanket.png/revision/latest | 48 -5 280 280 | 375 | 536 | Marvin | Marvin | 1 | 0 | 0 | 1 | 0 | true | person |
| Randy | individual | 2 | Randy Marsh | https://southpark.fandom.com/wiki/Randy_Marsh | https://static.wikia.nocookie.net/southparkfanon/images/d/d6/Randy_Marsh.png/revision/latest | 0 -10 428 428 | 428 | 1057 | Randy | Randy | 2 | 0 | 0 | 3 | 1 | true | person |
| Sharon | individual | 3 | Sharon Marsh | https://southpark.fandom.com/wiki/Sharon_Marsh | https://static.wikia.nocookie.net/southpark/images/9/99/Sharon_Marsh.png/revision/latest | 20 -10 390 390 | 439 | 1049 | Sharon | Sharon | 0 | 0 | 0 | 0 | 1 | true | person |

_ƒ 표시는 계산된 열을 나타냅니다._

</details>

| 용어 | 설명 | 부연 설명 |
|------|-------------|-------------------|
| **Property** | The ten properties and the wiring every rule reads. | — |
| Label | 정의된 속성입니다. | _The property's wording on the page._ |
| Property Kind | 정의된 속성입니다. | _'object' links two things, 'datatype' links a thing to a value, 'type' states class membership._ |
| Datatype | 정의된 속성입니다. | _The value's XML Schema datatype for a datatype property; blank otherwise._ |
| Is User Addable | 빈 문자열에 참입니다. | _Offered in the page's Add fact form._ |
| Has One Current Value | 빈 문자열에 참입니다. | _The page keeps one current value per person: switching a value on replaces the earlier one._ |
| Inverse of | 정의된 속성입니다. | _The property the inverse rule derives from this one._ |
| Is Symmetric | 빈 문자열에 참입니다. | _The symmetric rule makes this link true in both directions._ |
| Chain First | 정의된 속성입니다. | _First link of the property chain that derives this property._ |
| Chain Second | 정의된 속성입니다. | _Second link of the property chain that derives this property._ |
| Domain Class | 정의된 속성입니다. | _The class the domain rule gives this property's subject._ |
| Range Class | 정의된 속성입니다. | _The class the range rule gives this property's object._ |
| Disjoint With | 정의된 속성입니다. | _A property that may not link the same pair, in the same direction, as this one._ |
| Sort Order | 정의된 속성입니다. | _Position in the Add fact form's property list._ |
| Value Node Label | 정의된 속성입니다. | _The small label under a value node for this property: 'role value' or 'number value'._ |
| Current Value Wording | 정의된 속성입니다. | _How the Add fact form names a replaced value, as in 'Stan now has grade level 5'._ |
| Input Name | 정의된 속성입니다. | _How the integer check names this property's value, as in 'for grade level'._ |
| Record Kind | 정의된 속성입니다. | _The word the current-record summary uses for this property's values: role, grade or school._ |
| Record Order | 정의된 속성입니다. | _Position of this property in the current-record summary._ |
| Name | 같은 레코드의 ‘label’ 값과 같습니다. | _Order 1. The property's wording._ |

<details open>
<summary>예시 데이터</summary>

| Label | Property Kind | Datatype | Is User Addable | Has One Current Value | Is Symmetric | Sort Order | Value Node Label | Current Value Wording | Input Name | Record Kind | Record Order | Name ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|
| is parent of | object | — | true | false | false | 1 | — | — | — | — | — | is parent of |
| is child of | object | — | true | false | false | 2 | — | — | — | — | — | is child of |
| is spouse of | object | — | true | false | true | 3 | — | — | — | — | — | is spouse of |

_ƒ 표시는 계산된 열을 나타냅니다._

</details>

| 용어 | 설명 | 부연 설명 |
|------|-------------|-------------------|
| **Statement** | Every fact the page can state or conclude about the cast. A listed fact appears in the page's fact list; the rest are candidates that only a rule can make hold. | — |
| Graph | 정의된 속성입니다. | _The graph the fact belongs to._ |
| Subject | 정의된 속성입니다. | _What the fact describes._ |
| Property | 정의된 속성입니다. | _How the subject connects to the object._ |
| Object Thing | 정의된 속성입니다. | _The related thing, for an object or type property._ |
| Object Value | 정의된 속성입니다. | _The literal value, for a datatype property._ |
| Is Listed | 빈 문자열에 참입니다. | _Shown in the page's fact list._ |
| Is Built in | 빈 문자열에 참입니다. | _One of the page's teaching facts; the reader cannot remove it._ |
| Is Detail | 빈 문자열에 참입니다. | _One of Stan's detail facts that Show Stan's details switches on._ |
| Is Stated | 빈 문자열에 참입니다. | _The fact's checkbox._ |
| Activation Seq | 정의된 속성입니다. | _When the fact was last switched on, as a running number; decides which value is current._ |
| Sort Order | 정의된 속성입니다. | _Position in the fact list; candidates follow the listed facts._ |
| Subject Label | 연결된 ‘subject’ 레코드에서 가져온 값입니다. | _Order 1. The subject's name._ |
| Property Label | 연결된 ‘property’ 레코드에서 가져온 값입니다. | _Order 1. The property's wording._ |
| Object Label | 연결된 ‘object thing’ 레코드에서 가져온 값입니다. | _Order 1. The object's name, for an object or type property._ |
| Property Datatype | 연결된 ‘property’ 레코드에서 가져온 값입니다. | _Order 1. The value's datatype, for a datatype property._ |
| Property Has One Current Value | 연결된 ‘property’ 레코드가 ‘one current value’ 항목이 있는 경우에 참입니다. | _Order 1. Whether the property keeps one current value per person._ |
| Property Domain Class | 연결된 ‘property’ 레코드에서 가져온 값입니다. | _Order 1. The property's domain class._ |
| Property Range Class | 연결된 ‘property’ 레코드에서 가져온 값입니다. | _Order 1. The property's range class._ |
| Literal Quote | 연결된 ‘graph’ 레코드에서 가져온 값입니다. | _Order 1. The graph's literal quote mark._ |
| Literal Type Marker | 연결된 ‘graph’ 레코드에서 가져온 값입니다. | _Order 1. The graph's datatype marker._ |
| Built in Source Text | 연결된 ‘graph’ 레코드에서 가져온 값입니다. | _Order 1. The graph's source line for a teaching fact._ |
| User Source Text | 연결된 ‘graph’ 레코드에서 가져온 값입니다. | _Order 1. The graph's source line for a reader's fact._ |
| Next Fact Sort Order | 연결된 ‘graph’ 레코드에서 가져온 값입니다. | _Order 6. List position of the next fact to add._ |
| Is Literal | ‘object thing’ 값이 비어 있는 경우에 참입니다. | _Order 1. True when the object is a value rather than a thing._ |
| Object Text | 우선순위에 따라 결정됩니다: ‘object thing’ 값이 비어 있는 경우에는 ‘object value’; 그 밖의 경우에는 ‘object label’. | _Order 2. The object's name or the value._ |
| Name | 다음과 같이 계산됩니다: ‘subject label’, 이어서 공백 한 칸, 이어서 ‘property label’, 이어서 공백 한 칸, 이어서 ‘object text’. | _Order 3. The fact in words._ |
| Words | 다음과 같이 계산됩니다: ‘subject label’, 이어서 공백 한 칸, 이어서 ‘property label’, 이어서 공백 한 칸, 이어서 ‘object thing’ 값이 비어 있는 경우에는 ‘literal quote’, 이어서 ‘object value’, 이어서 ‘literal quote’, 이어서 ‘literal type marker’, 이어서 ‘property datatype’, 그 밖의 경우에는 ‘object label’. ⚠︎ 기계적 표현 <!-- rulespeak:reword --> | _Order 2. The fact as a conclusion card quotes it, with a literal written in Turtle form._ |
| Source | 우선순위에 따라 결정됩니다: ‘built in’ 플래그가 설정된 경우에는 ‘built in source text’; 그 밖의 경우에는 ‘user source text’. | _Order 2. Where a listed fact came from._ |
| Subject Property Key | 다음과 같이 계산됩니다: ‘subject’, 이어서 “|”, 이어서 ‘property’. | _Order 1. Groups the values one subject has for one property._ |
| Latest Sibling Activation Seq | 해당 ‘statement’ 레코드에 연결된 모든 ‘statements’ 레코드의 최대 ‘activation seq’. | _Order 2. The most recent activation among this subject's values for this property._ |
| Is in Effect | 다음 모두에 해당하는 경우(‘stated’ 플래그가 설정된 경우 및 다음 중 하나 이상에 해당하는 경우(‘property has one current value’ 플래그가 설정되지 않은 경우 또는 ‘activation seq’ 값이 ‘latest sibling activation seq’인 경우))에 참입니다. | _Order 3. Switched on and, for a one-value property, the most recently switched-on value. Drawn as a solid line._ |
| Listed Unit | 우선순위에 따라 결정됩니다: ‘listed’ 플래그가 설정된 경우에는 1; 그 밖의 경우에는 0. | _Order 1. 1 when the fact is in the fact list._ |
| In Effect Unit | 우선순위에 따라 결정됩니다: ‘in effect’ 플래그가 설정된 경우에는 1; 그 밖의 경우에는 0. | _Order 4. 1 when the fact is in effect._ |
| Listed Off Unit | 우선순위에 따라 결정됩니다: 다음 모두에 해당하는 경우(‘listed’ 플래그가 설정된 경우 및 ‘in effect’ 플래그가 설정되지 않은 경우)에는 1; 그 밖의 경우에는 0. | _Order 4. 1 when the fact is listed but not in effect._ |
| Is Next to Add | 다음 모두에 해당하는 경우(‘listed off unit’ 값이 1인 경우 및 ‘sort order’ 값이 ‘next fact sort order’인 경우)에 참입니다. | _Order 7. The fact the Add next fact button switches on._ |
| Count of Inverse Firings | 해당 ‘statement’ 레코드에 연결된 모든 ‘firings’ 레코드의 총 ‘inverse fired unit’. | _Order 6. Inverse rule applications that conclude this fact._ |
| Count of Symmetric Firings | 해당 ‘statement’ 레코드에 연결된 모든 ‘firings’ 레코드의 총 ‘symmetric fired unit’. | _Order 6. Symmetric rule applications that conclude this fact._ |
| Holds Before Chain | 다음 중 하나 이상에 해당하는 경우(‘in effect’ 플래그가 설정된 경우, ‘count of inverse firings’ 값이 0보다 큰 경우 또는 ‘count of symmetric firings’ 값이 0보다 큰 경우)에 참입니다. | _Order 7. Holds from facts in effect plus the inverse and symmetric rules._ |
| Count of Chain Firings | 해당 ‘statement’ 레코드에 연결된 모든 ‘firings’ 레코드의 총 ‘chain fired unit’. | _Order 10. Property chain applications that conclude this fact._ |
| Holds Before Typing | 다음 중 하나 이상에 해당하는 경우(‘holds before chain’ 플래그가 설정된 경우 또는 ‘count of chain firings’ 값이 0보다 큰 경우)에 참입니다. | _Order 11. Holds before the domain and range rules apply._ |
| Count of Type Firings | 해당 ‘statement’ 레코드에 연결된 모든 ‘firings’ 레코드의 총 ‘type fired unit’. | _Order 14. Domain and range applications that conclude this fact._ |
| Holds | 다음 중 하나 이상에 해당하는 경우(‘holds before typing’ 플래그가 설정된 경우 또는 ‘count of type firings’ 값이 0보다 큰 경우)에 참입니다. | _Order 15. True when in effect or concluded by an enabled rule; the graph draws it._ |
| Is Conclusion | 다음 모두에 해당하는 경우(‘holds’ 플래그가 설정된 경우 및 ‘in effect’ 플래그가 설정되지 않은 경우)에 참입니다. | _Order 16. Holds only because a rule concluded it. Drawn dashed._ |
| Holds Unit | 우선순위에 따라 결정됩니다: ‘holds’ 플래그가 설정된 경우에는 1; 그 밖의 경우에는 0. | _Order 16. 1 when the fact holds._ |
| Conclusion Unit | 우선순위에 따라 결정됩니다: ‘conclusion’ 플래그가 설정된 경우에는 1; 그 밖의 경우에는 0. | _Order 17. 1 when the fact is a conclusion._ |
| Parent Unit | 우선순위에 따라 결정됩니다: 다음 모두에 해당하는 경우(‘holds’ 플래그가 설정된 경우 및 ‘property’ 값이 “isParentOf”인 경우)에는 1; 그 밖의 경우에는 0. | _Order 16. 1 for a holding parent link._ |
| Grandparent Unit | 우선순위에 따라 결정됩니다: 다음 모두에 해당하는 경우(‘holds’ 플래그가 설정된 경우 및 ‘property’ 값이 “isGrandparentOf”인 경우)에는 1; 그 밖의 경우에는 0. | _Order 16. 1 for a holding grandparent link._ |
| Place Unit | 우선순위에 따라 결정됩니다: 다음 모두에 해당하는 경우(‘holds’ 플래그가 설정된 경우 및 ‘property’ 값이 “attends”인 경우)에는 1; 그 밖의 경우에는 0. | _Order 16. 1 for a holding attends link._ |
| First Fired Order Key | 해당 ‘statement’ 레코드에 연결된 ‘firings’ 레코드 중 ‘fired unit’에 해당하는 것 전체의 최소 ‘order key’. | _Order 15. Order key of the first rule application that concludes this fact, in the page reasoner's order._ |
| Count of Chosen Firings | 해당 ‘statement’ 레코드에 연결된 모든 ‘firings’ 레코드의 총 ‘chosen unit’. | _Order 20. Rule applications shown as this fact's conclusion card: one for a conclusion, zero otherwise._ |
| Subject Node Key | 같은 레코드의 ‘subject’ 값과 같습니다. | _Order 1. The graph node this fact starts from._ |
| Object Node Key | 우선순위에 따라 결정됩니다: ‘object thing’ 값이 비어 있는 경우에는 “literal|”, 이어서 ‘object value’, 이어서 “|”, 이어서 ‘property datatype’; 그 밖의 경우에는 ‘object thing’. | _Order 2. The graph node this fact points at: the thing, or one node per value and datatype._ |
| Subject Node Kind | 연결된 ‘subject’ 레코드에서 가져온 값입니다. | _Order 19. How the graph draws the subject._ |
| Object Thing Node Kind | 연결된 ‘object thing’ 레코드에서 가져온 값입니다. | _Order 19. How the graph draws the object thing._ |
| Object Node Kind | 우선순위에 따라 결정됩니다: ‘object thing’ 값이 비어 있는 경우에는 “value”; 그 밖의 경우에는 ‘object thing node kind’. | _Order 20. How the graph draws the object: a value, a class, a place or a person._ |
| Object Value Node Label | 연결된 ‘property’ 레코드에서 가져온 값입니다. | _Order 1. The small label under a value node._ |
| Count of Violations As First | 해당 ‘statement’ 레코드에 연결된 모든 ‘disjoint checks’ 레코드의 총 ‘violation unit’. | _Order 18. Violated disjointness checks naming this fact first._ |
| Count of Violations As Second | 해당 ‘statement’ 레코드에 연결된 모든 ‘disjoint checks’ 레코드의 총 ‘violation unit’. | _Order 18. Violated disjointness checks naming this fact second._ |
| Is in Contradiction | 다음 중 하나 이상에 해당하는 경우(‘count of violations as first’ 값이 0보다 큰 경우 또는 ‘count of violations as second’ 값이 0보다 큰 경우)에 참입니다. | _Order 19. Part of a contradiction the disjoint rule found; drawn in the alarm colour._ |

<details open>
<summary>예시 데이터</summary>

| Object Value | Is Listed | Is Built in | Is Detail | Is Stated | Activation Seq | Sort Order | Subject Label ƒ | Property Label ƒ | Object Label ƒ | Property Datatype ƒ | Property Has One Current Value ƒ | Property Domain Class ƒ | Property Range Class ƒ | Literal Quote ƒ | Literal Type Marker ƒ | Built in Source Text ƒ | User Source Text ƒ | Next Fact Sort Order ƒ | Is Literal ƒ | Object Text ƒ | Name ƒ | Words ƒ | Source ƒ | Subject Property Key ƒ | Latest Sibling Activation Seq ƒ | Is in Effect ƒ | Listed Unit ƒ | In Effect Unit ƒ | Listed Off Unit ƒ | Is Next to Add ƒ | Count of Inverse Firings ƒ | Count of Symmetric Firings ƒ | Holds Before Chain ƒ | Count of Chain Firings ƒ | Holds Before Typing ƒ | Count of Type Firings ƒ | Holds ƒ | Is Conclusion ƒ | Holds Unit ƒ | Conclusion Unit ƒ | Parent Unit ƒ | Grandparent Unit ƒ | Place Unit ƒ | First Fired Order Key ƒ | Count of Chosen Firings ƒ | Subject Node Key ƒ | Object Node Key ƒ | Subject Node Kind ƒ | Object Thing Node Kind ƒ | Object Node Kind ƒ | Object Value Node Label ƒ | Count of Violations As First ƒ | Count of Violations As Second ƒ | Is in Contradiction ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| — | true | true | false | true | 1 | 1 | Marvin | is parent of | Randy | — | false | — | — | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | — | false | Randy | Marvin is parent of Randy | Marvin is parent of Randy | Stated as a teaching example on this page. | Marvin\|isParentOf | 1 | true | 1 | 1 | 0 | false | 0 | 0 | true | 0 | true | 0 | true | false | 1 | 0 | 1 | 0 | 0 | — | 0 | Marvin | Randy | person | person | person | — | 0 | 0 | false |
| — | false | false | false | false | 0 | 101 | Marvin | is parent of | Sharon | — | false | — | — | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | — | false | Sharon | Marvin is parent of Sharon | Marvin is parent of Sharon | Stated by you on this page. | Marvin\|isParentOf | 1 | false | 0 | 0 | 0 | false | 0 | 0 | false | 0 | false | 0 | false | false | 0 | 0 | 0 | 0 | 0 | — | 0 | Marvin | Sharon | person | person | person | — | 0 | 0 | false |
| — | false | false | false | false | 0 | 102 | Marvin | is parent of | Stan | — | false | — | — | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | — | false | Stan | Marvin is parent of Stan | Marvin is parent of Stan | Stated by you on this page. | Marvin\|isParentOf | 1 | false | 0 | 0 | 0 | false | 0 | 0 | false | 0 | false | 0 | false | false | 0 | 0 | 0 | 0 | 0 | — | 0 | Marvin | Stan | person | person | person | — | 0 | 0 | false |

_ƒ 표시는 계산된 열을 나타냅니다._

</details>

| 용어 | 설명 | 부연 설명 |
|------|-------------|-------------------|
| **Firing** | Every way an enabled rule could conclude a candidate fact from the facts that hold. | — |
| Kind | 정의된 속성입니다. | _How the rule applies._ |
| Rule | 정의된 속성입니다. | _The rule switch._ |
| Target | 정의된 속성입니다. | _The fact this application concludes._ |
| First | 정의된 속성입니다. | _The premise._ |
| Second | 정의된 속성입니다. | _The second premise, for the property chain._ |
| Rule Enabled | 연결된 ‘rule’ 레코드가 ‘enabled’에 해당하는 경우에 참입니다. | _Order 1. Whether the rule is switched on._ |
| Kind Wording | 연결된 ‘kind’ 레코드에서 가져온 값입니다. | _Order 1. How the card names the rule._ |
| Kind Sort Order | 연결된 ‘kind’ 레코드에서 가져온 값입니다. | _Order 1. The kind's place in the reasoner's order._ |
| Target Name | 연결된 ‘target’ 레코드에서 가져온 값입니다. | _Order 4. The concluded fact in words._ |
| Target Subject Label | 연결된 ‘target’ 레코드에서 가져온 값입니다. | _Order 2. The concluded fact's subject._ |
| Target Property Label | 연결된 ‘target’ 레코드에서 가져온 값입니다. | _Order 2. The concluded fact's property._ |
| Target Object Text | 연결된 ‘target’ 레코드에서 가져온 값입니다. | _Order 3. The concluded fact's object._ |
| Target is Conclusion | 연결된 ‘target’ 레코드가 ‘conclusion’에 해당하는 경우에 참입니다. | _Order 17. Whether the concluded fact is new._ |
| Target Subject Node Key | 연결된 ‘target’ 레코드에서 가져온 값입니다. | _Order 2. The concluded fact's subject node._ |
| Target Object Node Key | 연결된 ‘target’ 레코드에서 가져온 값입니다. | _Order 3. The concluded fact's object node._ |
| Target First Fired Order Key | 연결된 ‘target’ 레코드에서 가져온 값입니다. | _Order 16. The order key of the concluded fact's first application._ |
| First Words | 연결된 ‘first’ 레코드에서 가져온 값입니다. | _Order 3. The premise as the card quotes it._ |
| Second Words | 연결된 ‘second’ 레코드에서 가져온 값입니다. | _Order 3. The second premise as the card quotes it._ |
| First is in Effect | 연결된 ‘first’ 레코드가 ‘in effect’에 해당하는 경우에 참입니다. | _Order 4. Whether the premise is in effect._ |
| First Holds Before Chain | 연결된 ‘first’ 레코드가 ‘holds before chain’에 해당하는 경우에 참입니다. | _Order 8. Whether the premise holds before the chain._ |
| Second Holds Before Chain | 연결된 ‘second’ 레코드가 ‘holds before chain’에 해당하는 경우에 참입니다. | _Order 8. Whether the second premise holds before the chain._ |
| First Holds Before Typing | 연결된 ‘first’ 레코드가 ‘holds before typing’에 해당하는 경우에 참입니다. | _Order 12. Whether the premise holds before typing._ |
| First Property | 연결된 ‘first’ 레코드에서 가져온 값입니다. | _Order 1. The premise's property key, as the card's axiom names it._ |
| First Property Domain Class | 연결된 ‘first’ 레코드에서 가져온 값입니다. | _Order 2. The premise property's domain class._ |
| First Property Range Class | 연결된 ‘first’ 레코드에서 가져온 값입니다. | _Order 2. The premise property's range class._ |
| First Sort Order | 연결된 ‘first’ 레코드에서 가져온 값입니다. | _Order 1. The premise's list position._ |
| Second Sort Order | 연결된 ‘second’ 레코드에서 가져온 값입니다. | _Order 1. The second premise's list position._ |
| Inverse Fired Unit | 우선순위에 따라 결정됩니다: 다음 모두에 해당하는 경우(‘rule enabled’ 플래그가 설정된 경우, ‘kind’ 값이 “inverse”인 경우 및 ‘first is in effect’ 플래그가 설정된 경우)에는 1; 그 밖의 경우에는 0. | _Order 5. 1 when the inverse rule fires here._ |
| Symmetric Fired Unit | 우선순위에 따라 결정됩니다: 다음 모두에 해당하는 경우(‘rule enabled’ 플래그가 설정된 경우, ‘kind’ 값이 “symmetric”인 경우 및 ‘first is in effect’ 플래그가 설정된 경우)에는 1; 그 밖의 경우에는 0. | _Order 5. 1 when the symmetric rule fires here._ |
| Chain Fired Unit | 우선순위에 따라 결정됩니다: 다음 모두에 해당하는 경우(‘rule enabled’ 플래그가 설정된 경우, ‘kind’ 값이 “chain”인 경우, ‘first holds before chain’ 플래그가 설정된 경우 및 ‘second holds before chain’ 플래그가 설정된 경우)에는 1; 그 밖의 경우에는 0. | _Order 9. 1 when the property chain fires here._ |
| Type Fired Unit | 우선순위에 따라 결정됩니다: 다음 모두에 해당하는 경우(‘rule enabled’ 플래그가 설정된 경우, 다음 중 하나 이상에 해당하는 경우(‘kind’ 값이 “domain”인 경우 또는 ‘kind’ 값이 “range”인 경우) 및 ‘first holds before typing’ 플래그가 설정된 경우)에는 1; 그 밖의 경우에는 0. | _Order 13. 1 when the domain or range rule fires here._ |
| Fired Unit | 다음과 같이 계산됩니다: ‘inverse fired unit’ 더하기 ‘symmetric fired unit’ 더하기 ‘chain fired unit’ 더하기 ‘type fired unit’. | _Order 14. 1 when this application fires._ |
| Order Key | 다음과 같이 계산됩니다: ‘first sort order’ 곱하기 100000 더하기 ‘kind sort order’ 곱하기 1000 더하기 ‘second’ 값이 비어 있는 경우에는 0, 그 밖의 경우에는 ‘second sort order’. | _Order 2. The page reasoner's order: by premise, then rule kind, then second premise._ |
| Is Chosen | 다음 모두에 해당하는 경우(‘fired unit’ 값이 1인 경우, ‘target is conclusion’ 플래그가 설정된 경우 및 ‘order key’ 값이 ‘target first fired order key’인 경우)에 참입니다. | _Order 18. The application the conclusions panel shows for its fact: the first one in the reasoner's order._ |
| Chosen Unit | 우선순위에 따라 결정됩니다: ‘chosen’ 플래그가 설정된 경우에는 1; 그 밖의 경우에는 0. | _Order 19. 1 when this application is shown._ |
| Card Text | 우선순위에 따라 결정됩니다: ‘chosen’ 플래그가 설정된 경우에는 “Worked out by the ”, 이어서 ‘kind wording’, 이어서 “ from “”, 이어서 ‘first words’, 이어서 “””, 이어서 ‘kind’ 값이 “chain”인 경우에는 “ and “”, 이어서 ‘second words’, 이어서 “””, 그 밖의 경우에는 ‘kind’ 값이 “domain”인 경우에는 “ and “”, 이어서 ‘first property’, 이어서 “ has domain ”, 이어서 ‘first property domain class’, 이어서 “””, 그 밖의 경우에는 “ and “”, 이어서 ‘first property’, 이어서 “ has range ”, 이어서 ‘first property range class’, 이어서 “””, 이어서 마침표; 그 밖의 경우에는 빈 문자열. ⚠︎ 기계적 표현 <!-- rulespeak:reword --> | _Order 19. The conclusion card's provenance line, in the page's wording._ |
| Name | 같은 레코드의 ‘firing ID’ 값과 같습니다. | _Order 1. The application's key._ |

<details open>
<summary>예시 데이터</summary>

| Rule Enabled ƒ | Kind Wording ƒ | Kind Sort Order ƒ | Target Name ƒ | Target Subject Label ƒ | Target Property Label ƒ | Target Object Text ƒ | Target is Conclusion ƒ | Target Subject Node Key ƒ | Target Object Node Key ƒ | Target First Fired Order Key ƒ | First Words ƒ | Second Words ƒ | First is in Effect ƒ | First Holds Before Chain ƒ | Second Holds Before Chain ƒ | First Holds Before Typing ƒ | First Property ƒ | First Property Domain Class ƒ | First Property Range Class ƒ | First Sort Order ƒ | Second Sort Order ƒ | Inverse Fired Unit ƒ | Symmetric Fired Unit ƒ | Chain Fired Unit ƒ | Type Fired Unit ƒ | Fired Unit ƒ | Order Key ƒ | Is Chosen ƒ | Chosen Unit ƒ | Card Text ƒ | Name ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| false | inverse rule | 1 | Randy is child of Marvin | Randy | is child of | Marvin | false | Randy | Marvin | — | Marvin is parent of Randy |    | true | true | false | true | isParentOf | — | — | 1 | — | 0 | 0 | 0 | 0 | 0 | 101000 | false | 0 | — | inverse:Marvin\|isParentOf\|Randy>Randy\|isChildOf\|Marvin |
| false | inverse rule | 1 | Sharon is child of Marvin | Sharon | is child of | Marvin | false | Sharon | Marvin | — | Marvin is parent of Sharon |    | false | false | false | false | isParentOf | — | — | 101 | — | 0 | 0 | 0 | 0 | 0 | 10101000 | false | 0 | — | inverse:Marvin\|isParentOf\|Sharon>Sharon\|isChildOf\|Marvin |
| false | inverse rule | 1 | Stan is child of Marvin | Stan | is child of | Marvin | false | Stan | Marvin | — | Marvin is parent of Stan |    | false | false | false | false | isParentOf | — | — | 102 | — | 0 | 0 | 0 | 0 | 0 | 10201000 | false | 0 | — | inverse:Marvin\|isParentOf\|Stan>Stan\|isChildOf\|Marvin |

_ƒ 표시는 계산된 열을 나타냅니다._

</details>

| 용어 | 설명 | 부연 설명 |
|------|-------------|-------------------|
| **Disjoint Check** | Every pair the disjointness rule watches: the same two people linked by two disjoint properties. | — |
| Graph | 정의된 속성입니다. | _The graph._ |
| Rule | 정의된 속성입니다. | _The rule switch._ |
| First | 정의된 속성입니다. | _The first link._ |
| Second | 정의된 속성입니다. | _The disjoint link between the same pair, in the same direction._ |
| Rule Enabled | 연결된 ‘rule’ 레코드가 ‘enabled’에 해당하는 경우에 참입니다. | _Order 1. Whether the disjointness rule is on._ |
| First Holds | 연결된 ‘first’ 레코드가 ‘holds’에 해당하는 경우에 참입니다. | _Order 16. Whether the first link holds._ |
| Second Holds | 연결된 ‘second’ 레코드가 ‘holds’에 해당하는 경우에 참입니다. | _Order 16. Whether the second link holds._ |
| Subject Label | 연결된 ‘first’ 레코드에서 가져온 값입니다. | _Order 2. The person recorded with both links._ |
| Object Label | 연결된 ‘first’ 레코드에서 가져온 값입니다. | _Order 2. The other person._ |
| Violation Unit | 우선순위에 따라 결정됩니다: 다음 모두에 해당하는 경우(‘rule enabled’ 플래그가 설정된 경우, ‘first holds’ 플래그가 설정된 경우 및 ‘second holds’ 플래그가 설정된 경우)에는 1; 그 밖의 경우에는 0. | _Order 17. 1 when both links hold._ |
| Alarm Text | 우선순위에 따라 결정됩니다: ‘violation unit’ 값이 1인 경우에는 “Contradiction found. ”, 이어서 ‘subject label’, 이어서 “ is recorded as both parent and spouse of ”, 이어서 ‘object label’, 이어서 “. Check those facts and the selected disjoint rule.”; 그 밖의 경우에는 빈 문자열. | _Order 18. The page's contradiction alert, blank when there is none._ |
| Name | 같은 레코드의 ‘disjoint check ID’ 값과 같습니다. | _Order 1. The check's key._ |

<details open>
<summary>예시 데이터</summary>

| Rule Enabled ƒ | First Holds ƒ | Second Holds ƒ | Subject Label ƒ | Object Label ƒ | Violation Unit ƒ | Alarm Text ƒ | Name ƒ |
|---|---|---|---|---|---|---|---|
| false | true | false | Marvin | Randy | 0 | — | Marvin\|isParentOf\|Randy~isSpouseOf |
| false | false | false | Marvin | Sharon | 0 | — | Marvin\|isParentOf\|Sharon~isSpouseOf |
| false | false | false | Marvin | Stan | 0 | — | Marvin\|isParentOf\|Stan~isSpouseOf |

_ƒ 표시는 계산된 열을 나타냅니다._

</details>

| 용어 | 설명 | 부연 설명 |
|------|-------------|-------------------|
| **Scenario** | The page's buttons. Each one is a fact baseline, explicit fact changes and rule settings, applied in that order. | — |
| Graph | 정의된 속성입니다. | _The graph._ |
| Label | 정의된 속성입니다. | _The button's text._ |
| Note | 정의된 속성입니다. | _The note shown under the buttons after this one is pressed; blank clears it._ |
| Sort Order | 정의된 속성입니다. | _Position among the buttons._ |
| Button Group | 정의된 속성입니다. | _'stan' for the two imagined changes, 'facts' for the buttons under the fact list._ |
| Is Primary | 빈 문자열에 참입니다. | _Drawn as the filled button._ |
| Fact Baseline | 정의된 속성입니다. | _What happens to every listed fact first: keep, built-in (teaching facts on, others off), built-in-detail, all-off, activate-all (each switched on in list order), or next (the next fact on)._ |
| Layout | 정의된 속성입니다. | _The node layout the graph returns to; blank keeps the current positions._ |
| Clears Selection | 빈 문자열에 참입니다. | _Clears the selected node or line._ |
| Clears Form Message | 빈 문자열에 참입니다. | _Clears the Add fact form's message._ |
| Name | 같은 레코드의 ‘label’ 값과 같습니다. | _Order 1. The button's text._ |
| Graph Has Facts to Add | 연결된 ‘graph’ 레코드가 ‘facts to add’ 항목이 있는 경우에 참입니다. | _Order 7. Whether a listed fact can still be added._ |
| Is Available | 다음 중 하나 이상에 해당하는 경우(‘fact baseline’ 값이 “next” 이외의 값인 경우 또는 ‘graph has facts to add’ 플래그가 설정된 경우)에 참입니다. | _Order 8. False only for Add next fact once every listed fact is in effect._ |

<details open>
<summary>예시 데이터</summary>

| Label | Note | Sort Order | Button Group | Is Primary | Fact Baseline | Layout | Clears Selection | Clears Form Message | Name ƒ | Graph Has Facts to Add ƒ | Is Available ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|
| Move Stan to grade 5 | This imagined change updates Stan’s grade to 5. His identifier, Person statement and family links stay the same. His other selected details are unchanged. | 1 | stan | false | keep | — | true | true | Move Stan to grade 5 | false | true |
| Stan leaves school | This imagined change clears Stan’s current school details. His identifier, Person statement and family links remain. Removing a current fact does not say that its opposite is true; school history needs dates. | 2 | stan | false | keep | — | true | true | Stan leaves school | false | true |
| Add next fact | — | 3 | facts | true | next | — | false | false | Add next fact | false | false |

_ƒ 표시는 계산된 열을 나타냅니다._

</details>

| 용어 | 설명 | 부연 설명 |
|------|-------------|-------------------|
| **Scenario Fact Change** | Fact changes a button makes after its baseline. A blank object key matches every listed value of the property. | — |
| Scenario | 정의된 속성입니다. | _The button._ |
| Subject | 정의된 속성입니다. | _The subject whose facts change._ |
| Property | 정의된 속성입니다. | _The property whose facts change._ |
| Object Key | 정의된 속성입니다. | _The object thing's key or the literal value; blank for every listed value._ |
| Action | 정의된 속성입니다. | _'on' switches the fact on as the newest value; 'off' switches it off._ |
| Adds to List | 빈 문자열에 참입니다. | _Adds the fact to the fact list first when it is not listed._ |
| Sort Order | 정의된 속성입니다. | _Order of the change within the button._ |
| Name | 같은 레코드의 ‘scenario fact change ID’ 값과 같습니다. | _Order 1. The change's key._ |

<details open>
<summary>예시 데이터</summary>

| Object Key | Action | Adds to List | Sort Order | Name ƒ |
|---|---|---|---|---|
| Person | on | true | 1 | stan-grade-five:Stan\|isA\|Person |
| 5 | on | true | 2 | stan-grade-five:Stan\|hasGradeLevel\|5 |
| Person | on | true | 3 | stan-leaves-school:Stan\|isA\|Person |

_ƒ 표시는 계산된 열을 나타냅니다._

</details>

| 용어 | 설명 | 부연 설명 |
|------|-------------|-------------------|
| **Scenario Rule Setting** | Rule switches a button sets. | — |
| Scenario | 정의된 속성입니다. | _The button._ |
| Rule | 정의된 속성입니다. | _The rule switch._ |
| Is Enabled | 빈 문자열에 참입니다. | _The state the button puts the switch in._ |
| Name | 같은 레코드의 ‘scenario rule setting ID’ 값과 같습니다. | _Order 1. The setting's key._ |

<details open>
<summary>예시 데이터</summary>

| Is Enabled | Name ƒ |
|---|---|
| false | reset-original:inverse |
| false | reset-original:symmetric |
| false | reset-original:chain |

_ƒ 표시는 계산된 열을 나타냅니다._

</details>

| 용어 | 설명 | 부연 설명 |
|------|-------------|-------------------|
| **Node Layout** | Node positions in the graph's 380 by 480 drawing, one set per layout. | — |
| Layout | 정의된 속성입니다. | _The layout: 'default' or 'stan-details'._ |
| Node Key | 정의된 속성입니다. | _The node: a thing's key, or literal\|value\|datatype._ |
| X | 정의된 속성입니다. | _Horizontal position._ |
| Y | 정의된 속성입니다. | _Vertical position._ |
| Name | 같은 레코드의 ‘node layout ID’ 값과 같습니다. | _Order 1. The position's key._ |

<details open>
<summary>예시 데이터</summary>

| Layout | Node Key | X | Y | Name ƒ |
|---|---|---|---|---|
| default | Marvin | 70 | 45 | default:Marvin |
| default | Randy | 195 | 125 | default:Randy |
| default | Sharon | 318 | 55 | default:Sharon |

_ƒ 표시는 계산된 열을 나타냅니다._

</details>

| 용어 | 설명 | 부연 설명 |
|------|-------------|-------------------|
| **Layout Slot** | Places for nodes no layout positions, used in turn. | — |
| Sort Order | 정의된 속성입니다. | _Order in which slots are used._ |
| X | 정의된 속성입니다. | _Horizontal position._ |
| Y | 정의된 속성입니다. | _Vertical position._ |
| Name | 같은 레코드의 ‘layout slot ID’ 값과 같습니다. | _Order 1. The slot's key._ |

<details open>
<summary>예시 데이터</summary>

| Sort Order | X | Y | Name ƒ |
|---|---|---|---|
| 1 | 330 | 135 | slot-1 |
| 2 | 45 | 120 | slot-2 |
| 3 | 330 | 400 | slot-3 |

_ƒ 표시는 계산된 열을 나타냅니다._

</details>

| 용어 | 설명 | 부연 설명 |
|------|-------------|-------------------|
| **Page Text** | The page's wording, one row per string. {name}-style placeholders are filled by the page. | — |
| Text | 정의된 속성입니다. | _The string._ |
| Name | 같은 레코드의 ‘page text ID’ 값과 같습니다. | _Order 1. The string's key._ |

<details open>
<summary>예시 데이터</summary>

| Text | Name ƒ |
|---|---|
| South Park Ontology Explainer, on one rulebook | page-title |
| A rulebook rebuild of the explainer by | byline-prefix |
| Jessica Talisman | byline-author |

_ƒ 표시는 계산된 열을 나타냅니다._

</details>

## 2 사실 유형

- **firing kind** 레코드는 **rule** 레코드를 정확히 하나 참조합니다
- **property** 레코드는 **property** 레코드를 하나 참조할 수 있습니다
- **property** 레코드는 **thing** 레코드를 하나 참조할 수 있습니다
- **statement** 레코드는 **graph** 레코드를 정확히 하나 참조합니다
- **statement** 레코드는 **thing** 레코드를 정확히 하나 참조합니다
- **statement** 레코드는 **property** 레코드를 정확히 하나 참조합니다
- **firing** 레코드는 **firing kind** 레코드를 정확히 하나 참조합니다
- **firing** 레코드는 **rule** 레코드를 정확히 하나 참조합니다
- **firing** 레코드는 **statement** 레코드를 정확히 하나 참조합니다
- **disjoint check** 레코드는 **graph** 레코드를 정확히 하나 참조합니다
- **disjoint check** 레코드는 **rule** 레코드를 정확히 하나 참조합니다
- **disjoint check** 레코드는 **statement** 레코드를 정확히 하나 참조합니다
- **scenario** 레코드는 **graph** 레코드를 정확히 하나 참조합니다
- **scenario fact change** 레코드는 **scenario** 레코드를 정확히 하나 참조합니다
- **scenario fact change** 레코드는 **thing** 레코드를 정확히 하나 참조합니다
- **scenario fact change** 레코드는 **property** 레코드를 정확히 하나 참조합니다
- **scenario rule setting** 레코드는 **scenario** 레코드를 정확히 하나 참조합니다
- **scenario rule setting** 레코드는 **rule** 레코드를 정확히 하나 참조합니다

## 3 운영 규칙

_운영 규칙은 비즈니스가 **의무화**하거나 **금지**하거나 권고하는(**하는 것이 좋습니다**)
사항을 서술합니다. 구조적 규칙은 필수 필드와 외래 키에서 도출되고, 의미적 규칙은
Constraints 테이블에서 도출되며, 각 의미적 규칙은 룰북이 이미 계산하는 불리언 값을
판단 기준으로 삼습니다(아래 정의 규칙에서 DR-N으로 상호 참조)._

### 구조적 제약(스키마에서 도출)

- ‘graph’ 레코드는 ‘label’, ‘literal quote’, ‘literal type marker’, ‘built in source text’, ‘user source text’ 및 ‘no conclusions text’ 값을 포함**해야 합니다**.
- ‘rule’ 레코드는 ‘label’, ‘explanation’ 및 ‘sort order’ 값을 포함하고, 해당 레코드가 ‘enabled’에 해당하는지 여부를 기록**해야 합니다**.
- ‘firing kind’ 레코드는 ‘rule’ 레코드를 정확히 하나 참조**해야 합니다**.
- ‘firing kind’ 레코드는 ‘wording’ 및 ‘sort order’ 값을 포함**해야 합니다**.
- ‘thing’ 레코드는 ‘label’, ‘kind’ 및 ‘sort order’ 값을 포함**해야 합니다**.
- ‘property’ 레코드는 ‘label’, ‘property kind’ 및 ‘sort order’ 값을 포함하고, 해당 레코드가 ‘user addable’에 해당하는지 여부, 해당 레코드가 ‘one current value’ 항목이 있는지 여부 및 해당 레코드가 ‘symmetric’에 해당하는지 여부를 기록**해야 합니다**.
- ‘statement’ 레코드는 ‘graph’ 레코드를 정확히 하나 참조**해야 합니다**.
- ‘statement’ 레코드는 ‘subject’ 필드를 통해 ‘thing’ 레코드를 정확히 하나 참조**해야 합니다**.
- ‘statement’ 레코드는 ‘property’ 레코드를 정확히 하나 참조**해야 합니다**.
- ‘statement’ 레코드는 ‘activation seq’ 및 ‘sort order’ 값을 포함하고, 해당 레코드가 ‘listed’에 해당하는지 여부, 해당 레코드가 ‘built in’에 해당하는지 여부, 해당 레코드가 ‘detail’에 해당하는지 여부 및 해당 레코드가 ‘stated’에 해당하는지 여부를 기록**해야 합니다**.
- ‘firing’ 레코드는 ‘kind’ 필드를 통해 ‘firing kind’ 레코드를 정확히 하나 참조**해야 합니다**.
- ‘firing’ 레코드는 ‘rule’ 레코드를 정확히 하나 참조**해야 합니다**.
- ‘firing’ 레코드는 ‘target’ 필드를 통해 ‘statement’ 레코드를 정확히 하나 참조**해야 합니다**.
- ‘firing’ 레코드는 ‘first’ 필드를 통해 ‘statement’ 레코드를 정확히 하나 참조**해야 합니다**.
- ‘disjoint check’ 레코드는 ‘graph’ 레코드를 정확히 하나 참조**해야 합니다**.
- ‘disjoint check’ 레코드는 ‘rule’ 레코드를 정확히 하나 참조**해야 합니다**.
- ‘disjoint check’ 레코드는 ‘first’ 필드를 통해 ‘statement’ 레코드를 정확히 하나 참조**해야 합니다**.
- ‘disjoint check’ 레코드는 ‘second’ 필드를 통해 ‘statement’ 레코드를 정확히 하나 참조**해야 합니다**.
- ‘scenario’ 레코드는 ‘graph’ 레코드를 정확히 하나 참조**해야 합니다**.
- ‘scenario’ 레코드는 ‘label’, ‘sort order’, ‘button group’ 및 ‘fact baseline’ 값을 포함하고, 해당 레코드가 ‘primary’에 해당하는지 여부, 해당 레코드가 ‘clears selection’에 해당하는지 여부 및 해당 레코드가 ‘clears form message’에 해당하는지 여부를 기록**해야 합니다**.
- ‘scenario fact change’ 레코드는 ‘scenario’ 레코드를 정확히 하나 참조**해야 합니다**.
- ‘scenario fact change’ 레코드는 ‘subject’ 필드를 통해 ‘thing’ 레코드를 정확히 하나 참조**해야 합니다**.
- ‘scenario fact change’ 레코드는 ‘property’ 레코드를 정확히 하나 참조**해야 합니다**.
- ‘scenario fact change’ 레코드는 ‘action’ 및 ‘sort order’ 값을 포함하고, 해당 레코드가 ‘adds to list’에 해당하는지 여부를 기록**해야 합니다**.
- ‘scenario rule setting’ 레코드는 ‘scenario’ 레코드를 정확히 하나 참조**해야 합니다**.
- ‘scenario rule setting’ 레코드는 ‘rule’ 레코드를 정확히 하나 참조**해야 합니다**.
- ‘scenario rule setting’ 레코드는 해당 레코드가 ‘enabled’에 해당하는지 여부를 기록**해야 합니다**.
- ‘node layout’ 레코드는 ‘layout’, ‘node key’, ‘x’ 및 ‘y’ 값을 포함**해야 합니다**.
- ‘layout slot’ 레코드는 ‘sort order’, ‘x’ 및 ‘y’ 값을 포함**해야 합니다**.
- ‘page text’ 레코드는 ‘text’ 값을 포함**해야 합니다**.

## 4 정의 규칙

_모든 문장은 비즈니스 영역에서 성립하는 사실을 서술하며, 절차나 명령이 아닙니다.
한 방향의 필요조건이 동치로 오해되지 않도록 “iff”(필요충분조건) 대신 “경우에만”이라는
표현을 사용합니다. **⚠︎ 기계적 표현** 표시는 규칙에서 그대로 생성한 문장이 정확하기는 하지만
어색한 규칙에 붙습니다. 원하면 문장을 다듬어도 좋다는 표시일 뿐, 결함이 아닙니다._

| ID | 선언적 규칙 |
|----|------------------|
| **DR-1 Name** | ‘graph’ 레코드의 ‘name’ 값은 같은 레코드의 ‘label’ 값과 같습니다. |
| **DR-2 Count of Stated Facts** | ‘graph’ 레코드의 ‘count of stated facts’ 값은 해당 ‘graph’ 레코드에 연결된 모든 ‘statements’ 레코드의 총 ‘in effect unit’입니다. |
| **DR-3 Count of Conclusions** | ‘graph’ 레코드의 ‘count of conclusions’ 값은 해당 ‘graph’ 레코드에 연결된 모든 ‘statements’ 레코드의 총 ‘conclusion unit’입니다. |
| **DR-4 Count of Violations** | ‘graph’ 레코드의 ‘count of violations’ 값은 해당 ‘graph’ 레코드에 연결된 모든 ‘disjoint checks’ 레코드의 총 ‘violation unit’입니다. |
| **DR-5 Is Consistent** | ‘count of violations’ 값이 0인 경우, ‘graph’ 레코드는 ‘consistent’에 해당하는 것으로 간주됩니다. |
| **DR-6 Conclusions Message** | ‘graph’ 레코드의 ‘conclusions message’ 값은 다음 우선순위에 따라 결정됩니다:<br>1. ‘count of conclusions’ 값이 0인 경우: ‘no conclusions text’<br>2. 그 밖의 경우: 빈 문자열. |
| **DR-7 Count of Facts to Add** | ‘graph’ 레코드의 ‘count of facts to add’ 값은 해당 ‘graph’ 레코드에 연결된 모든 ‘statements’ 레코드의 총 ‘listed off unit’입니다. |
| **DR-8 Has Facts to Add** | ‘count of facts to add’ 값이 0보다 큰 경우, ‘graph’ 레코드는 ‘facts to add’ 항목이 있는 것으로 간주됩니다. |
| **DR-9 Next Fact Sort Order** | ‘graph’ 레코드의 ‘next fact sort order’ 값은 해당 ‘graph’ 레코드에 연결된 ‘statements’ 레코드 중 ‘listed off unit’에 해당하는 것 전체의 최소 ‘sort order’입니다. |
| **DR-10 Max Activation Seq** | ‘graph’ 레코드의 ‘max activation seq’ 값은 해당 ‘graph’ 레코드에 연결된 모든 ‘statements’ 레코드의 최대 ‘activation seq’입니다. |
| **DR-11 Next Activation Seq** | ‘graph’ 레코드의 ‘next activation seq’ 값은 다음과 같이 계산됩니다: ‘max activation seq’ 더하기 1. |
| **DR-12 Max Listed Sort Order** | ‘graph’ 레코드의 ‘max listed sort order’ 값은 해당 ‘graph’ 레코드에 연결된 ‘statements’ 레코드 중 ‘listed unit’에 해당하는 것 전체의 최대 ‘sort order’입니다. |
| **DR-13 Next Listed Sort Order** | ‘graph’ 레코드의 ‘next listed sort order’ 값은 다음과 같이 계산됩니다: ‘max listed sort order’ 더하기 1. |
| **DR-14 Name** | ‘rule’ 레코드의 ‘name’ 값은 같은 레코드의 ‘label’ 값과 같습니다. |
| **DR-15 Count of Firings** | ‘rule’ 레코드의 ‘count of firings’ 값은 해당 ‘rule’ 레코드에 연결된 모든 ‘firings’ 레코드의 총 ‘fired unit’입니다. |
| **DR-16 Name** | ‘firing kind’ 레코드의 ‘name’ 값은 같은 레코드의 ‘wording’ 값과 같습니다. |
| **DR-17 Rule Enabled** | ‘firing kind’ 레코드의 ‘rule enabled’ 값은 연결된 ‘rule’ 레코드가 ‘enabled’에 해당하는 경우에 참입니다. |
| **DR-18 Name** | ‘thing’ 레코드의 ‘name’ 값은 같은 레코드의 ‘label’ 값과 같습니다. |
| **DR-19 Node Key** | ‘thing’ 레코드의 ‘node key’ 값은 같은 레코드의 ‘thing ID’ 값과 같습니다. |
| **DR-20 Count of Children** | ‘thing’ 레코드의 ‘count of children’ 값은 해당 ‘thing’ 레코드에 연결된 모든 ‘statements’ 레코드의 총 ‘parent unit’입니다. |
| **DR-21 Count of Grandchildren** | ‘thing’ 레코드의 ‘count of grandchildren’ 값은 해당 ‘thing’ 레코드에 연결된 모든 ‘statements’ 레코드의 총 ‘grandparent unit’입니다. |
| **DR-22 Count As Place** | ‘thing’ 레코드의 ‘count as place’ 값은 해당 ‘thing’ 레코드에 연결된 모든 ‘statements’ 레코드의 총 ‘place unit’입니다. |
| **DR-23 Count As Subject** | ‘thing’ 레코드의 ‘count as subject’ 값은 해당 ‘thing’ 레코드에 연결된 모든 ‘statements’ 레코드의 총 ‘holds unit’입니다. |
| **DR-24 Count As Object** | ‘thing’ 레코드의 ‘count as object’ 값은 해당 ‘thing’ 레코드에 연결된 모든 ‘statements’ 레코드의 총 ‘holds unit’입니다. |
| **DR-25 Is in Graph** | 다음 중 하나 이상에 해당하는 경우(‘count as subject’ 값이 0보다 큰 경우 또는 ‘count as object’ 값이 0보다 큰 경우), ‘thing’ 레코드는 ‘in graph’에 해당하는 것으로 간주됩니다. |
| **DR-26 Node Kind** | ‘thing’ 레코드의 ‘node kind’ 값은 다음 우선순위에 따라 결정됩니다:<br>1. ‘kind’ 값이 “class”인 경우: “class”<br>2. ‘count as place’ 값이 0보다 큰 경우: “place”<br>3. 그 밖의 경우: “person”. |
| **DR-27 Name** | ‘property’ 레코드의 ‘name’ 값은 같은 레코드의 ‘label’ 값과 같습니다. |
| **DR-28 Subject Label** | ‘statement’ 레코드의 ‘subject label’ 값은 연결된 ‘subject’ 레코드에서 가져옵니다. |
| **DR-29 Property Label** | ‘statement’ 레코드의 ‘property label’ 값은 연결된 ‘property’ 레코드에서 가져옵니다. |
| **DR-30 Object Label** | ‘statement’ 레코드의 ‘object label’ 값은 연결된 ‘object thing’ 레코드에서 가져옵니다. |
| **DR-31 Property Datatype** | ‘statement’ 레코드의 ‘property datatype’ 값은 연결된 ‘property’ 레코드에서 가져옵니다. |
| **DR-32 Property Has One Current Value** | ‘statement’ 레코드의 ‘property has one current value’ 값은 연결된 ‘property’ 레코드가 ‘one current value’ 항목이 있는 경우에 참입니다. |
| **DR-33 Property Domain Class** | ‘statement’ 레코드의 ‘property domain class’ 값은 연결된 ‘property’ 레코드에서 가져옵니다. |
| **DR-34 Property Range Class** | ‘statement’ 레코드의 ‘property range class’ 값은 연결된 ‘property’ 레코드에서 가져옵니다. |
| **DR-35 Literal Quote** | ‘statement’ 레코드의 ‘literal quote’ 값은 연결된 ‘graph’ 레코드에서 가져옵니다. |
| **DR-36 Literal Type Marker** | ‘statement’ 레코드의 ‘literal type marker’ 값은 연결된 ‘graph’ 레코드에서 가져옵니다. |
| **DR-37 Built in Source Text** | ‘statement’ 레코드의 ‘built in source text’ 값은 연결된 ‘graph’ 레코드에서 가져옵니다. |
| **DR-38 User Source Text** | ‘statement’ 레코드의 ‘user source text’ 값은 연결된 ‘graph’ 레코드에서 가져옵니다. |
| **DR-39 Next Fact Sort Order** | ‘statement’ 레코드의 ‘next fact sort order’ 값은 연결된 ‘graph’ 레코드에서 가져옵니다. |
| **DR-40 Is Literal** | ‘object thing’ 값이 비어 있는 경우, ‘statement’ 레코드는 ‘literal’에 해당하는 것으로 간주됩니다. |
| **DR-41 Object Text** | ‘statement’ 레코드의 ‘object text’ 값은 다음 우선순위에 따라 결정됩니다:<br>1. ‘object thing’ 값이 비어 있는 경우: ‘object value’<br>2. 그 밖의 경우: ‘object label’. |
| **DR-42 Name** | ‘statement’ 레코드의 ‘name’ 값은 다음과 같이 계산됩니다: ‘subject label’, 이어서 공백 한 칸, 이어서 ‘property label’, 이어서 공백 한 칸, 이어서 ‘object text’. |
| **DR-43 Words** | ‘statement’ 레코드의 ‘words’ 값은 다음과 같이 계산됩니다: ‘subject label’, 이어서 공백 한 칸, 이어서 ‘property label’, 이어서 공백 한 칸, 이어서 ‘object thing’ 값이 비어 있는 경우에는 ‘literal quote’, 이어서 ‘object value’, 이어서 ‘literal quote’, 이어서 ‘literal type marker’, 이어서 ‘property datatype’, 그 밖의 경우에는 ‘object label’. ⚠︎ 기계적 표현 <!-- rulespeak:reword --> |
| **DR-44 Source** | ‘statement’ 레코드의 ‘source’ 값은 다음 우선순위에 따라 결정됩니다:<br>1. ‘built in’ 플래그가 설정된 경우: ‘built in source text’<br>2. 그 밖의 경우: ‘user source text’. |
| **DR-45 Subject Property Key** | ‘statement’ 레코드의 ‘subject property key’ 값은 다음과 같이 계산됩니다: ‘subject’, 이어서 “|”, 이어서 ‘property’. |
| **DR-46 Latest Sibling Activation Seq** | ‘statement’ 레코드의 ‘latest sibling activation seq’ 값은 해당 ‘statement’ 레코드에 연결된 모든 ‘statements’ 레코드의 최대 ‘activation seq’입니다. |
| **DR-47 Is in Effect** | 다음 모두에 해당하는 경우(‘stated’ 플래그가 설정된 경우 및 다음 중 하나 이상에 해당하는 경우(‘property has one current value’ 플래그가 설정되지 않은 경우 또는 ‘activation seq’ 값이 ‘latest sibling activation seq’인 경우)), ‘statement’ 레코드는 ‘in effect’에 해당하는 것으로 간주됩니다. |
| **DR-48 Listed Unit** | ‘statement’ 레코드의 ‘listed unit’ 값은 다음 우선순위에 따라 결정됩니다:<br>1. ‘listed’ 플래그가 설정된 경우: 1<br>2. 그 밖의 경우: 0. |
| **DR-49 In Effect Unit** | ‘statement’ 레코드의 ‘in effect unit’ 값은 다음 우선순위에 따라 결정됩니다:<br>1. ‘in effect’ 플래그가 설정된 경우: 1<br>2. 그 밖의 경우: 0. |
| **DR-50 Listed Off Unit** | ‘statement’ 레코드의 ‘listed off unit’ 값은 다음 우선순위에 따라 결정됩니다:<br>1. 다음 모두에 해당하는 경우(‘listed’ 플래그가 설정된 경우 및 ‘in effect’ 플래그가 설정되지 않은 경우): 1<br>2. 그 밖의 경우: 0. |
| **DR-51 Is Next to Add** | 다음 모두에 해당하는 경우(‘listed off unit’ 값이 1인 경우 및 ‘sort order’ 값이 ‘next fact sort order’인 경우), ‘statement’ 레코드는 ‘next to add’에 해당하는 것으로 간주됩니다. |
| **DR-52 Count of Inverse Firings** | ‘statement’ 레코드의 ‘count of inverse firings’ 값은 해당 ‘statement’ 레코드에 연결된 모든 ‘firings’ 레코드의 총 ‘inverse fired unit’입니다. |
| **DR-53 Count of Symmetric Firings** | ‘statement’ 레코드의 ‘count of symmetric firings’ 값은 해당 ‘statement’ 레코드에 연결된 모든 ‘firings’ 레코드의 총 ‘symmetric fired unit’입니다. |
| **DR-54 Holds Before Chain** | 다음 중 하나 이상에 해당하는 경우(‘in effect’ 플래그가 설정된 경우, ‘count of inverse firings’ 값이 0보다 큰 경우 또는 ‘count of symmetric firings’ 값이 0보다 큰 경우), ‘statement’ 레코드는 ‘holds before chain’ 조건에 해당하는 것으로 간주됩니다. |
| **DR-55 Count of Chain Firings** | ‘statement’ 레코드의 ‘count of chain firings’ 값은 해당 ‘statement’ 레코드에 연결된 모든 ‘firings’ 레코드의 총 ‘chain fired unit’입니다. |
| **DR-56 Holds Before Typing** | 다음 중 하나 이상에 해당하는 경우(‘holds before chain’ 플래그가 설정된 경우 또는 ‘count of chain firings’ 값이 0보다 큰 경우), ‘statement’ 레코드는 ‘holds before typing’ 조건에 해당하는 것으로 간주됩니다. |
| **DR-57 Count of Type Firings** | ‘statement’ 레코드의 ‘count of type firings’ 값은 해당 ‘statement’ 레코드에 연결된 모든 ‘firings’ 레코드의 총 ‘type fired unit’입니다. |
| **DR-58 Holds** | 다음 중 하나 이상에 해당하는 경우(‘holds before typing’ 플래그가 설정된 경우 또는 ‘count of type firings’ 값이 0보다 큰 경우), ‘statement’ 레코드는 ‘holds’ 조건에 해당하는 것으로 간주됩니다. |
| **DR-59 Is Conclusion** | 다음 모두에 해당하는 경우(‘holds’ 플래그가 설정된 경우 및 ‘in effect’ 플래그가 설정되지 않은 경우), ‘statement’ 레코드는 ‘conclusion’에 해당하는 것으로 간주됩니다. |
| **DR-60 Holds Unit** | ‘statement’ 레코드의 ‘holds unit’ 값은 다음 우선순위에 따라 결정됩니다:<br>1. ‘holds’ 플래그가 설정된 경우: 1<br>2. 그 밖의 경우: 0. |
| **DR-61 Conclusion Unit** | ‘statement’ 레코드의 ‘conclusion unit’ 값은 다음 우선순위에 따라 결정됩니다:<br>1. ‘conclusion’ 플래그가 설정된 경우: 1<br>2. 그 밖의 경우: 0. |
| **DR-62 Parent Unit** | ‘statement’ 레코드의 ‘parent unit’ 값은 다음 우선순위에 따라 결정됩니다:<br>1. 다음 모두에 해당하는 경우(‘holds’ 플래그가 설정된 경우 및 ‘property’ 값이 “isParentOf”인 경우): 1<br>2. 그 밖의 경우: 0. |
| **DR-63 Grandparent Unit** | ‘statement’ 레코드의 ‘grandparent unit’ 값은 다음 우선순위에 따라 결정됩니다:<br>1. 다음 모두에 해당하는 경우(‘holds’ 플래그가 설정된 경우 및 ‘property’ 값이 “isGrandparentOf”인 경우): 1<br>2. 그 밖의 경우: 0. |
| **DR-64 Place Unit** | ‘statement’ 레코드의 ‘place unit’ 값은 다음 우선순위에 따라 결정됩니다:<br>1. 다음 모두에 해당하는 경우(‘holds’ 플래그가 설정된 경우 및 ‘property’ 값이 “attends”인 경우): 1<br>2. 그 밖의 경우: 0. |
| **DR-65 First Fired Order Key** | ‘statement’ 레코드의 ‘first fired order key’ 값은 해당 ‘statement’ 레코드에 연결된 ‘firings’ 레코드 중 ‘fired unit’에 해당하는 것 전체의 최소 ‘order key’입니다. |
| **DR-66 Count of Chosen Firings** | ‘statement’ 레코드의 ‘count of chosen firings’ 값은 해당 ‘statement’ 레코드에 연결된 모든 ‘firings’ 레코드의 총 ‘chosen unit’입니다. |
| **DR-67 Subject Node Key** | ‘statement’ 레코드의 ‘subject node key’ 값은 같은 레코드의 ‘subject’ 값과 같습니다. |
| **DR-68 Object Node Key** | ‘statement’ 레코드의 ‘object node key’ 값은 다음 우선순위에 따라 결정됩니다:<br>1. ‘object thing’ 값이 비어 있는 경우: “literal|”, 이어서 ‘object value’, 이어서 “|”, 이어서 ‘property datatype’<br>2. 그 밖의 경우: ‘object thing’. |
| **DR-69 Subject Node Kind** | ‘statement’ 레코드의 ‘subject node kind’ 값은 연결된 ‘subject’ 레코드에서 가져옵니다. |
| **DR-70 Object Thing Node Kind** | ‘statement’ 레코드의 ‘object thing node kind’ 값은 연결된 ‘object thing’ 레코드에서 가져옵니다. |
| **DR-71 Object Node Kind** | ‘statement’ 레코드의 ‘object node kind’ 값은 다음 우선순위에 따라 결정됩니다:<br>1. ‘object thing’ 값이 비어 있는 경우: “value”<br>2. 그 밖의 경우: ‘object thing node kind’. |
| **DR-72 Object Value Node Label** | ‘statement’ 레코드의 ‘object value node label’ 값은 연결된 ‘property’ 레코드에서 가져옵니다. |
| **DR-73 Count of Violations As First** | ‘statement’ 레코드의 ‘count of violations as first’ 값은 해당 ‘statement’ 레코드에 연결된 모든 ‘disjoint checks’ 레코드의 총 ‘violation unit’입니다. |
| **DR-74 Count of Violations As Second** | ‘statement’ 레코드의 ‘count of violations as second’ 값은 해당 ‘statement’ 레코드에 연결된 모든 ‘disjoint checks’ 레코드의 총 ‘violation unit’입니다. |
| **DR-75 Is in Contradiction** | 다음 중 하나 이상에 해당하는 경우(‘count of violations as first’ 값이 0보다 큰 경우 또는 ‘count of violations as second’ 값이 0보다 큰 경우), ‘statement’ 레코드는 ‘in contradiction’에 해당하는 것으로 간주됩니다. |
| **DR-76 Rule Enabled** | ‘firing’ 레코드의 ‘rule enabled’ 값은 연결된 ‘rule’ 레코드가 ‘enabled’에 해당하는 경우에 참입니다. |
| **DR-77 Kind Wording** | ‘firing’ 레코드의 ‘kind wording’ 값은 연결된 ‘kind’ 레코드에서 가져옵니다. |
| **DR-78 Kind Sort Order** | ‘firing’ 레코드의 ‘kind sort order’ 값은 연결된 ‘kind’ 레코드에서 가져옵니다. |
| **DR-79 Target Name** | ‘firing’ 레코드의 ‘target name’ 값은 연결된 ‘target’ 레코드에서 가져옵니다. |
| **DR-80 Target Subject Label** | ‘firing’ 레코드의 ‘target subject label’ 값은 연결된 ‘target’ 레코드에서 가져옵니다. |
| **DR-81 Target Property Label** | ‘firing’ 레코드의 ‘target property label’ 값은 연결된 ‘target’ 레코드에서 가져옵니다. |
| **DR-82 Target Object Text** | ‘firing’ 레코드의 ‘target object text’ 값은 연결된 ‘target’ 레코드에서 가져옵니다. |
| **DR-83 Target is Conclusion** | ‘firing’ 레코드의 ‘target is conclusion’ 값은 연결된 ‘target’ 레코드가 ‘conclusion’에 해당하는 경우에 참입니다. |
| **DR-84 Target Subject Node Key** | ‘firing’ 레코드의 ‘target subject node key’ 값은 연결된 ‘target’ 레코드에서 가져옵니다. |
| **DR-85 Target Object Node Key** | ‘firing’ 레코드의 ‘target object node key’ 값은 연결된 ‘target’ 레코드에서 가져옵니다. |
| **DR-86 Target First Fired Order Key** | ‘firing’ 레코드의 ‘target first fired order key’ 값은 연결된 ‘target’ 레코드에서 가져옵니다. |
| **DR-87 First Words** | ‘firing’ 레코드의 ‘first words’ 값은 연결된 ‘first’ 레코드에서 가져옵니다. |
| **DR-88 Second Words** | ‘firing’ 레코드의 ‘second words’ 값은 연결된 ‘second’ 레코드에서 가져옵니다. |
| **DR-89 First is in Effect** | ‘firing’ 레코드의 ‘first is in effect’ 값은 연결된 ‘first’ 레코드가 ‘in effect’에 해당하는 경우에 참입니다. |
| **DR-90 First Holds Before Chain** | ‘firing’ 레코드의 ‘first holds before chain’ 값은 연결된 ‘first’ 레코드가 ‘holds before chain’에 해당하는 경우에 참입니다. |
| **DR-91 Second Holds Before Chain** | ‘firing’ 레코드의 ‘second holds before chain’ 값은 연결된 ‘second’ 레코드가 ‘holds before chain’에 해당하는 경우에 참입니다. |
| **DR-92 First Holds Before Typing** | ‘firing’ 레코드의 ‘first holds before typing’ 값은 연결된 ‘first’ 레코드가 ‘holds before typing’에 해당하는 경우에 참입니다. |
| **DR-93 First Property** | ‘firing’ 레코드의 ‘first property’ 값은 연결된 ‘first’ 레코드에서 가져옵니다. |
| **DR-94 First Property Domain Class** | ‘firing’ 레코드의 ‘first property domain class’ 값은 연결된 ‘first’ 레코드에서 가져옵니다. |
| **DR-95 First Property Range Class** | ‘firing’ 레코드의 ‘first property range class’ 값은 연결된 ‘first’ 레코드에서 가져옵니다. |
| **DR-96 First Sort Order** | ‘firing’ 레코드의 ‘first sort order’ 값은 연결된 ‘first’ 레코드에서 가져옵니다. |
| **DR-97 Second Sort Order** | ‘firing’ 레코드의 ‘second sort order’ 값은 연결된 ‘second’ 레코드에서 가져옵니다. |
| **DR-98 Inverse Fired Unit** | ‘firing’ 레코드의 ‘inverse fired unit’ 값은 다음 우선순위에 따라 결정됩니다:<br>1. 다음 모두에 해당하는 경우(‘rule enabled’ 플래그가 설정된 경우, ‘kind’ 값이 “inverse”인 경우 및 ‘first is in effect’ 플래그가 설정된 경우): 1<br>2. 그 밖의 경우: 0. |
| **DR-99 Symmetric Fired Unit** | ‘firing’ 레코드의 ‘symmetric fired unit’ 값은 다음 우선순위에 따라 결정됩니다:<br>1. 다음 모두에 해당하는 경우(‘rule enabled’ 플래그가 설정된 경우, ‘kind’ 값이 “symmetric”인 경우 및 ‘first is in effect’ 플래그가 설정된 경우): 1<br>2. 그 밖의 경우: 0. |
| **DR-100 Chain Fired Unit** | ‘firing’ 레코드의 ‘chain fired unit’ 값은 다음 우선순위에 따라 결정됩니다:<br>1. 다음 모두에 해당하는 경우(‘rule enabled’ 플래그가 설정된 경우, ‘kind’ 값이 “chain”인 경우, ‘first holds before chain’ 플래그가 설정된 경우 및 ‘second holds before chain’ 플래그가 설정된 경우): 1<br>2. 그 밖의 경우: 0. |
| **DR-101 Type Fired Unit** | ‘firing’ 레코드의 ‘type fired unit’ 값은 다음 우선순위에 따라 결정됩니다:<br>1. 다음 모두에 해당하는 경우(‘rule enabled’ 플래그가 설정된 경우, 다음 중 하나 이상에 해당하는 경우(‘kind’ 값이 “domain”인 경우 또는 ‘kind’ 값이 “range”인 경우) 및 ‘first holds before typing’ 플래그가 설정된 경우): 1<br>2. 그 밖의 경우: 0. |
| **DR-102 Fired Unit** | ‘firing’ 레코드의 ‘fired unit’ 값은 다음과 같이 계산됩니다: ‘inverse fired unit’ 더하기 ‘symmetric fired unit’ 더하기 ‘chain fired unit’ 더하기 ‘type fired unit’. |
| **DR-103 Order Key** | ‘firing’ 레코드의 ‘order key’ 값은 다음과 같이 계산됩니다: ‘first sort order’ 곱하기 100000 더하기 ‘kind sort order’ 곱하기 1000 더하기 ‘second’ 값이 비어 있는 경우에는 0, 그 밖의 경우에는 ‘second sort order’. |
| **DR-104 Is Chosen** | 다음 모두에 해당하는 경우(‘fired unit’ 값이 1인 경우, ‘target is conclusion’ 플래그가 설정된 경우 및 ‘order key’ 값이 ‘target first fired order key’인 경우), ‘firing’ 레코드는 ‘chosen’에 해당하는 것으로 간주됩니다. |
| **DR-105 Chosen Unit** | ‘firing’ 레코드의 ‘chosen unit’ 값은 다음 우선순위에 따라 결정됩니다:<br>1. ‘chosen’ 플래그가 설정된 경우: 1<br>2. 그 밖의 경우: 0. |
| **DR-106 Card Text** | ‘firing’ 레코드의 ‘card text’ 값은 다음 우선순위에 따라 결정됩니다:<br>1. ‘chosen’ 플래그가 설정된 경우: “Worked out by the ”, 이어서 ‘kind wording’, 이어서 “ from “”, 이어서 ‘first words’, 이어서 “””, 이어서 ‘kind’ 값이 “chain”인 경우에는 “ and “”, 이어서 ‘second words’, 이어서 “””, 그 밖의 경우에는 ‘kind’ 값이 “domain”인 경우에는 “ and “”, 이어서 ‘first property’, 이어서 “ has domain ”, 이어서 ‘first property domain class’, 이어서 “””, 그 밖의 경우에는 “ and “”, 이어서 ‘first property’, 이어서 “ has range ”, 이어서 ‘first property range class’, 이어서 “””, 이어서 마침표<br>2. 그 밖의 경우: 빈 문자열. ⚠︎ 기계적 표현 <!-- rulespeak:reword --> |
| **DR-107 Name** | ‘firing’ 레코드의 ‘name’ 값은 같은 레코드의 ‘firing ID’ 값과 같습니다. |
| **DR-108 Rule Enabled** | ‘disjoint check’ 레코드의 ‘rule enabled’ 값은 연결된 ‘rule’ 레코드가 ‘enabled’에 해당하는 경우에 참입니다. |
| **DR-109 First Holds** | ‘disjoint check’ 레코드의 ‘first holds’ 값은 연결된 ‘first’ 레코드가 ‘holds’에 해당하는 경우에 참입니다. |
| **DR-110 Second Holds** | ‘disjoint check’ 레코드의 ‘second holds’ 값은 연결된 ‘second’ 레코드가 ‘holds’에 해당하는 경우에 참입니다. |
| **DR-111 Subject Label** | ‘disjoint check’ 레코드의 ‘subject label’ 값은 연결된 ‘first’ 레코드에서 가져옵니다. |
| **DR-112 Object Label** | ‘disjoint check’ 레코드의 ‘object label’ 값은 연결된 ‘first’ 레코드에서 가져옵니다. |
| **DR-113 Violation Unit** | ‘disjoint check’ 레코드의 ‘violation unit’ 값은 다음 우선순위에 따라 결정됩니다:<br>1. 다음 모두에 해당하는 경우(‘rule enabled’ 플래그가 설정된 경우, ‘first holds’ 플래그가 설정된 경우 및 ‘second holds’ 플래그가 설정된 경우): 1<br>2. 그 밖의 경우: 0. |
| **DR-114 Alarm Text** | ‘disjoint check’ 레코드의 ‘alarm text’ 값은 다음 우선순위에 따라 결정됩니다:<br>1. ‘violation unit’ 값이 1인 경우: “Contradiction found. ”, 이어서 ‘subject label’, 이어서 “ is recorded as both parent and spouse of ”, 이어서 ‘object label’, 이어서 “. Check those facts and the selected disjoint rule.”<br>2. 그 밖의 경우: 빈 문자열. |
| **DR-115 Name** | ‘disjoint check’ 레코드의 ‘name’ 값은 같은 레코드의 ‘disjoint check ID’ 값과 같습니다. |
| **DR-116 Name** | ‘scenario’ 레코드의 ‘name’ 값은 같은 레코드의 ‘label’ 값과 같습니다. |
| **DR-117 Graph Has Facts to Add** | ‘scenario’ 레코드의 ‘graph has facts to add’ 값은 연결된 ‘graph’ 레코드가 ‘facts to add’ 항목이 있는 경우에 참입니다. |
| **DR-118 Is Available** | 다음 중 하나 이상에 해당하는 경우(‘fact baseline’ 값이 “next” 이외의 값인 경우 또는 ‘graph has facts to add’ 플래그가 설정된 경우), ‘scenario’ 레코드는 ‘available’에 해당하는 것으로 간주됩니다. |
| **DR-119 Name** | ‘scenario fact change’ 레코드의 ‘name’ 값은 같은 레코드의 ‘scenario fact change ID’ 값과 같습니다. |
| **DR-120 Name** | ‘scenario rule setting’ 레코드의 ‘name’ 값은 같은 레코드의 ‘scenario rule setting ID’ 값과 같습니다. |
| **DR-121 Name** | ‘node layout’ 레코드의 ‘name’ 값은 같은 레코드의 ‘node layout ID’ 값과 같습니다. |
| **DR-122 Name** | ‘layout slot’ 레코드의 ‘name’ 값은 같은 레코드의 ‘layout slot ID’ 값과 같습니다. |
| **DR-123 Name** | ‘page text’ 레코드의 ‘name’ 값은 같은 레코드의 ‘page text ID’ 값과 같습니다. |

## 5 스키마 추적성

_표현식 열은 문서 표기법으로 적은 규칙의 정의입니다.
룰북에 저장된 것과 같은 논리를 비즈니스 독자를 위해 풀어 쓴 것입니다._

| 스키마 요소 | 종류 | 표현식 |
|----------------|------|------------|
| **Graphs.Name** | 수식 | `Label` |
| **Graphs.CountOfStatedFacts** | 집계 | `Sum(Statements.InEffectUnit via Graph)` |
| **Graphs.CountOfConclusions** | 집계 | `Sum(Statements.ConclusionUnit via Graph)` |
| **Graphs.CountOfViolations** | 집계 | `Sum(DisjointChecks.ViolationUnit via Graph)` |
| **Graphs.IsConsistent** | 수식 | `CountOfViolations = 0` |
| **Graphs.ConclusionsMessage** | 수식 | `If(CountOfConclusions = 0, NoConclusionsText, "")` |
| **Graphs.CountOfFactsToAdd** | 집계 | `Sum(Statements.ListedOffUnit via Graph)` |
| **Graphs.HasFactsToAdd** | 수식 | `CountOfFactsToAdd > 0` |
| **Graphs.NextFactSortOrder** | 집계 | `Min(Statements.SortOrder via Graph)` |
| **Graphs.MaxActivationSeq** | 집계 | `Max(Statements.ActivationSeq via Graph)` |
| **Graphs.NextActivationSeq** | 수식 | `MaxActivationSeq + 1` |
| **Graphs.MaxListedSortOrder** | 집계 | `Max(Statements.SortOrder via Graph)` |
| **Graphs.NextListedSortOrder** | 수식 | `MaxListedSortOrder + 1` |
| **Rules.Name** | 수식 | `Label` |
| **Rules.CountOfFirings** | 집계 | `Sum(Firings.FiredUnit via Rule)` |
| **FiringKinds.Name** | 수식 | `Wording` |
| **FiringKinds.RuleEnabled** | 조회 | `Lookup(Rules.IsEnabled via Rule)` |
| **Things.Name** | 수식 | `Label` |
| **Things.NodeKey** | 수식 | `ThingId` |
| **Things.CountOfChildren** | 집계 | `Sum(Statements.ParentUnit via Subject)` |
| **Things.CountOfGrandchildren** | 집계 | `Sum(Statements.GrandparentUnit via Subject)` |
| **Things.CountAsPlace** | 집계 | `Sum(Statements.PlaceUnit via ObjectThing)` |
| **Things.CountAsSubject** | 집계 | `Sum(Statements.HoldsUnit via Subject)` |
| **Things.CountAsObject** | 집계 | `Sum(Statements.HoldsUnit via ObjectThing)` |
| **Things.IsInGraph** | 수식 | `Or(CountAsSubject > 0, CountAsObject > 0)` |
| **Things.NodeKind** | 수식 | `If(Kind = "class", "class", If(CountAsPlace > 0, "place", "person"))` |
| **Properties.Name** | 수식 | `Label` |
| **Statements.SubjectLabel** | 조회 | `Lookup(Things.Label via Subject)` |
| **Statements.PropertyLabel** | 조회 | `Lookup(Properties.Label via Property)` |
| **Statements.ObjectLabel** | 조회 | `Lookup(Things.Label via ObjectThing)` |
| **Statements.PropertyDatatype** | 조회 | `Lookup(Properties.Datatype via Property)` |
| **Statements.PropertyHasOneCurrentValue** | 조회 | `Lookup(Properties.HasOneCurrentValue via Property)` |
| **Statements.PropertyDomainClass** | 조회 | `Lookup(Properties.DomainClass via Property)` |
| **Statements.PropertyRangeClass** | 조회 | `Lookup(Properties.RangeClass via Property)` |
| **Statements.LiteralQuote** | 조회 | `Lookup(Graphs.LiteralQuote via Graph)` |
| **Statements.LiteralTypeMarker** | 조회 | `Lookup(Graphs.LiteralTypeMarker via Graph)` |
| **Statements.BuiltInSourceText** | 조회 | `Lookup(Graphs.BuiltInSourceText via Graph)` |
| **Statements.UserSourceText** | 조회 | `Lookup(Graphs.UserSourceText via Graph)` |
| **Statements.NextFactSortOrder** | 조회 | `Lookup(Graphs.NextFactSortOrder via Graph)` |
| **Statements.IsLiteral** | 수식 | `ObjectThing = ""` |
| **Statements.ObjectText** | 수식 | `If(ObjectThing = "", ObjectValue, ObjectLabel)` |
| **Statements.Name** | 수식 | `Concat(SubjectLabel, " ", PropertyLabel, " ", ObjectText)` |
| **Statements.Words** | 수식 | `Concat(SubjectLabel, " ", PropertyLabel, " ", If(ObjectThing = "", Concat(LiteralQuote, ObjectValue, LiteralQuote, LiteralTypeMarker, PropertyDatatype), ObjectLabel))` |
| **Statements.Source** | 수식 | `If(IsBuiltIn, BuiltInSourceText, UserSourceText)` |
| **Statements.SubjectPropertyKey** | 수식 | `Concat(Subject, "\|", Property)` |
| **Statements.LatestSiblingActivationSeq** | 집계 | `Max(Statements.ActivationSeq via SubjectPropertyKey)` |
| **Statements.IsInEffect** | 수식 | `And(IsStated, Or(Not(PropertyHasOneCurrentValue), ActivationSeq = LatestSiblingActivationSeq))` |
| **Statements.ListedUnit** | 수식 | `If(IsListed, 1, 0)` |
| **Statements.InEffectUnit** | 수식 | `If(IsInEffect, 1, 0)` |
| **Statements.ListedOffUnit** | 수식 | `If(And(IsListed, Not(IsInEffect)), 1, 0)` |
| **Statements.IsNextToAdd** | 수식 | `And(ListedOffUnit = 1, SortOrder = NextFactSortOrder)` |
| **Statements.CountOfInverseFirings** | 집계 | `Sum(Firings.InverseFiredUnit via Target)` |
| **Statements.CountOfSymmetricFirings** | 집계 | `Sum(Firings.SymmetricFiredUnit via Target)` |
| **Statements.HoldsBeforeChain** | 수식 | `Or(IsInEffect, CountOfInverseFirings > 0, CountOfSymmetricFirings > 0)` |
| **Statements.CountOfChainFirings** | 집계 | `Sum(Firings.ChainFiredUnit via Target)` |
| **Statements.HoldsBeforeTyping** | 수식 | `Or(HoldsBeforeChain, CountOfChainFirings > 0)` |
| **Statements.CountOfTypeFirings** | 집계 | `Sum(Firings.TypeFiredUnit via Target)` |
| **Statements.Holds** | 수식 | `Or(HoldsBeforeTyping, CountOfTypeFirings > 0)` |
| **Statements.IsConclusion** | 수식 | `And(Holds, Not(IsInEffect))` |
| **Statements.HoldsUnit** | 수식 | `If(Holds, 1, 0)` |
| **Statements.ConclusionUnit** | 수식 | `If(IsConclusion, 1, 0)` |
| **Statements.ParentUnit** | 수식 | `If(And(Holds, Property = "isParentOf"), 1, 0)` |
| **Statements.GrandparentUnit** | 수식 | `If(And(Holds, Property = "isGrandparentOf"), 1, 0)` |
| **Statements.PlaceUnit** | 수식 | `If(And(Holds, Property = "attends"), 1, 0)` |
| **Statements.FirstFiredOrderKey** | 집계 | `Min(Firings.OrderKey via Target)` |
| **Statements.CountOfChosenFirings** | 집계 | `Sum(Firings.ChosenUnit via Target)` |
| **Statements.SubjectNodeKey** | 수식 | `Subject` |
| **Statements.ObjectNodeKey** | 수식 | `If(ObjectThing = "", Concat("literal\|", ObjectValue, "\|", PropertyDatatype), ObjectThing)` |
| **Statements.SubjectNodeKind** | 조회 | `Lookup(Things.NodeKind via Subject)` |
| **Statements.ObjectThingNodeKind** | 조회 | `Lookup(Things.NodeKind via ObjectThing)` |
| **Statements.ObjectNodeKind** | 수식 | `If(ObjectThing = "", "value", ObjectThingNodeKind)` |
| **Statements.ObjectValueNodeLabel** | 조회 | `Lookup(Properties.ValueNodeLabel via Property)` |
| **Statements.CountOfViolationsAsFirst** | 집계 | `Sum(DisjointChecks.ViolationUnit via First)` |
| **Statements.CountOfViolationsAsSecond** | 집계 | `Sum(DisjointChecks.ViolationUnit via Second)` |
| **Statements.IsInContradiction** | 수식 | `Or(CountOfViolationsAsFirst > 0, CountOfViolationsAsSecond > 0)` |
| **Firings.RuleEnabled** | 조회 | `Lookup(Rules.IsEnabled via Rule)` |
| **Firings.KindWording** | 조회 | `Lookup(FiringKinds.Wording via Kind)` |
| **Firings.KindSortOrder** | 조회 | `Lookup(FiringKinds.SortOrder via Kind)` |
| **Firings.TargetName** | 조회 | `Lookup(Statements.Name via Target)` |
| **Firings.TargetSubjectLabel** | 조회 | `Lookup(Statements.SubjectLabel via Target)` |
| **Firings.TargetPropertyLabel** | 조회 | `Lookup(Statements.PropertyLabel via Target)` |
| **Firings.TargetObjectText** | 조회 | `Lookup(Statements.ObjectText via Target)` |
| **Firings.TargetIsConclusion** | 조회 | `Lookup(Statements.IsConclusion via Target)` |
| **Firings.TargetSubjectNodeKey** | 조회 | `Lookup(Statements.SubjectNodeKey via Target)` |
| **Firings.TargetObjectNodeKey** | 조회 | `Lookup(Statements.ObjectNodeKey via Target)` |
| **Firings.TargetFirstFiredOrderKey** | 조회 | `Lookup(Statements.FirstFiredOrderKey via Target)` |
| **Firings.FirstWords** | 조회 | `Lookup(Statements.Words via First)` |
| **Firings.SecondWords** | 조회 | `Lookup(Statements.Words via Second)` |
| **Firings.FirstIsInEffect** | 조회 | `Lookup(Statements.IsInEffect via First)` |
| **Firings.FirstHoldsBeforeChain** | 조회 | `Lookup(Statements.HoldsBeforeChain via First)` |
| **Firings.SecondHoldsBeforeChain** | 조회 | `Lookup(Statements.HoldsBeforeChain via Second)` |
| **Firings.FirstHoldsBeforeTyping** | 조회 | `Lookup(Statements.HoldsBeforeTyping via First)` |
| **Firings.FirstProperty** | 조회 | `Lookup(Statements.Property via First)` |
| **Firings.FirstPropertyDomainClass** | 조회 | `Lookup(Statements.PropertyDomainClass via First)` |
| **Firings.FirstPropertyRangeClass** | 조회 | `Lookup(Statements.PropertyRangeClass via First)` |
| **Firings.FirstSortOrder** | 조회 | `Lookup(Statements.SortOrder via First)` |
| **Firings.SecondSortOrder** | 조회 | `Lookup(Statements.SortOrder via Second)` |
| **Firings.InverseFiredUnit** | 수식 | `If(And(RuleEnabled, Kind = "inverse", FirstIsInEffect), 1, 0)` |
| **Firings.SymmetricFiredUnit** | 수식 | `If(And(RuleEnabled, Kind = "symmetric", FirstIsInEffect), 1, 0)` |
| **Firings.ChainFiredUnit** | 수식 | `If(And(RuleEnabled, Kind = "chain", FirstHoldsBeforeChain, SecondHoldsBeforeChain), 1, 0)` |
| **Firings.TypeFiredUnit** | 수식 | `If(And(RuleEnabled, Or(Kind = "domain", Kind = "range"), FirstHoldsBeforeTyping), 1, 0)` |
| **Firings.FiredUnit** | 수식 | `InverseFiredUnit + SymmetricFiredUnit + ChainFiredUnit + TypeFiredUnit` |
| **Firings.OrderKey** | 수식 | `FirstSortOrder * 100000 + KindSortOrder * 1000 + If(Second = "", 0, SecondSortOrder)` |
| **Firings.IsChosen** | 수식 | `And(FiredUnit = 1, TargetIsConclusion, OrderKey = TargetFirstFiredOrderKey)` |
| **Firings.ChosenUnit** | 수식 | `If(IsChosen, 1, 0)` |
| **Firings.CardText** | 수식 | `If(IsChosen, Concat("Worked out by the ", KindWording, " from “", FirstWords, "”", If(Kind = "chain", Concat(" and “", SecondWords, "”"), If(Kind = "domain", Concat(" and “", FirstProperty, " has domain ", FirstPropertyDomainClass, "”"), If(Kind = "range", Concat(" and “", FirstProperty, " has range ", FirstPropertyRangeClass, "”"), ""))), "."), "")` |
| **Firings.Name** | 수식 | `FiringId` |
| **DisjointChecks.RuleEnabled** | 조회 | `Lookup(Rules.IsEnabled via Rule)` |
| **DisjointChecks.FirstHolds** | 조회 | `Lookup(Statements.Holds via First)` |
| **DisjointChecks.SecondHolds** | 조회 | `Lookup(Statements.Holds via Second)` |
| **DisjointChecks.SubjectLabel** | 조회 | `Lookup(Statements.SubjectLabel via First)` |
| **DisjointChecks.ObjectLabel** | 조회 | `Lookup(Statements.ObjectLabel via First)` |
| **DisjointChecks.ViolationUnit** | 수식 | `If(And(RuleEnabled, FirstHolds, SecondHolds), 1, 0)` |
| **DisjointChecks.AlarmText** | 수식 | `If(ViolationUnit = 1, Concat("Contradiction found. ", SubjectLabel, " is recorded as both parent and spouse of ", ObjectLabel, ". Check those facts and the selected disjoint rule."), "")` |
| **DisjointChecks.Name** | 수식 | `DisjointCheckId` |
| **Scenarios.Name** | 수식 | `Label` |
| **Scenarios.GraphHasFactsToAdd** | 조회 | `Lookup(Graphs.HasFactsToAdd via Graph)` |
| **Scenarios.IsAvailable** | 수식 | `Or(FactBaseline <> "next", GraphHasFactsToAdd)` |
| **ScenarioFactChanges.Name** | 수식 | `ScenarioFactChangeId` |
| **ScenarioRuleSettings.Name** | 수식 | `ScenarioRuleSettingId` |
| **NodeLayouts.Name** | 수식 | `NodeLayoutId` |
| **LayoutSlots.Name** | 수식 | `LayoutSlotId` |
| **PageTexts.Name** | 수식 | `PageTextId` |

---

> **RuleSpeak®** — **Ronald G. Ross**가 만든, **SBVR** 관례를 따르는 선언형 비즈니스 규칙 표기법 — 은(는) 곧 제공됩니다. 그때까지 이 페이지는 일반 문서를 보여 줍니다. [www.RonRoss.info](https://www.RonRoss.info)
