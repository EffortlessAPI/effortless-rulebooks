# 📘 South Park ontology explainer — 문서

_The barebones South Park ontology from Jessica Talisman's explainer: ten stated facts and five rules that can each be switched on. Conclusions are formulas._

> 룰북에서 생성한 선언적 비즈니스 규칙입니다. 아래의 모든 문장은 비즈니스 영역에서
> 성립하는 사실을 서술하며, 절차나 명령이 아닙니다. 룰북의 수식이 단일 진실 공급원이고,
> 이 문서는 그 수식을 평이한 문장으로 풀어 쓴 것입니다.

## 1 비즈니스 용어

| 용어 | 설명 | 부연 설명 |
|------|-------------|-------------------|
| **Graph** | The whole example. | — |
| Name | 같은 레코드의 ‘graph ID’ 값과 같습니다. | — |
| Count of Stated Facts | 해당 ‘graph’ 레코드에 연결된 모든 ‘statements’ 레코드의 총 ‘stated unit’. | — |
| Count of Conclusions | 해당 ‘graph’ 레코드에 연결된 모든 ‘statements’ 레코드의 총 ‘conclusion unit’. | _Facts that hold but were never stated._ |
| Count of Violations | 해당 ‘graph’ 레코드에 연결된 모든 ‘disjoint checks’ 레코드의 총 ‘violation unit’. | — |
| Is Consistent | ‘count of violations’ 값이 0인 경우에 참입니다. | — |

<details open>
<summary>예시 데이터</summary>

| Name ƒ | Count of Stated Facts ƒ | Count of Conclusions ƒ | Count of Violations ƒ | Is Consistent ƒ |
|---|---|---|---|---|
| south-park | 10 | 8 | 0 | true |

_ƒ 표시는 계산된 열을 나타냅니다._

</details>

| 용어 | 설명 | 부연 설명 |
|------|-------------|-------------------|
| **Rule** | The five rules; IsEnabled is the checkbox. | — |
| Label | 정의된 속성입니다. | — |
| Explanation | 정의된 속성입니다. | — |
| Is Enabled | 빈 문자열에 참입니다. | — |
| Name | 같은 레코드의 ‘label’ 값과 같습니다. | — |
| Count of Firings | 해당 ‘rule’ 레코드에 연결된 모든 ‘firings’ 레코드의 총 ‘fired unit’. | _How many times the rule produced a fact._ |

<details open>
<summary>예시 데이터</summary>

| Label | Explanation | Is Enabled | Name ƒ | Count of Firings ƒ |
|---|---|---|---|---|
| Inverse | A parent link also gives a child link between the same two people. | true | Inverse | 3 |
| Symmetric | A friend link or a spouse link is true for both people. | true | Symmetric | 2 |
| Property chain | A parent of a parent is a grandparent. | true | Property chain | 2 |

_ƒ 표시는 계산된 열을 나타냅니다._

</details>

| 용어 | 설명 | 부연 설명 |
|------|-------------|-------------------|
| **Thing** | A node in the picture: a person, a place, a class or a role value. | — |
| Label | 정의된 속성입니다. | — |
| Kind | 정의된 속성입니다. | — |
| Name | 같은 레코드의 ‘label’ 값과 같습니다. | — |
| Count of Children | 해당 ‘thing’ 레코드에 연결된 모든 ‘statements’ 레코드의 총 ‘parent unit’. | _Added: a count, which a reasoner alone does not give._ |
| Count of Grandchildren | 해당 ‘thing’ 레코드에 연결된 모든 ‘statements’ 레코드의 총 ‘grandparent unit’. | _Added: counts concluded grandchildren._ |

<details open>
<summary>예시 데이터</summary>

| Label | Kind | Name ƒ | Count of Children ƒ | Count of Grandchildren ƒ |
|---|---|---|---|---|
| Marvin | individual | Marvin | 1 | 2 |
| Randy | individual | Randy | 2 | 0 |
| Sharon | individual | Sharon | 0 | 0 |

_ƒ 표시는 계산된 열을 나타냅니다._

</details>

| 용어 | 설명 | 부연 설명 |
|------|-------------|-------------------|
| **Property** | A kind of link, with the characteristics the rules use. | — |
| Label | 정의된 속성입니다. | — |
| Inverse of | 정의된 속성입니다. | — |
| Is Symmetric | 빈 문자열에 참입니다. | — |
| Chain First | 정의된 속성입니다. | — |
| Chain Second | 정의된 속성입니다. | — |
| Domain Class | 정의된 속성입니다. | — |
| Range Class | 정의된 속성입니다. | — |
| Disjoint With | 정의된 속성입니다. | — |
| Name | 같은 레코드의 ‘label’ 값과 같습니다. | — |

<details open>
<summary>예시 데이터</summary>

| Label | Is Symmetric | Name ƒ |
|---|---|---|
| is parent of | false | is parent of |
| is child of | false | is child of |
| is spouse of | true | is spouse of |

_ƒ 표시는 계산된 열을 나타냅니다._

</details>

| 용어 | 설명 | 부연 설명 |
|------|-------------|-------------------|
| **Statement** | A fact that is stated, or could be concluded: Subject, Property, Object. | — |
| Graph | 정의된 속성입니다. | — |
| Subject | 정의된 속성입니다. | — |
| Property | 정의된 속성입니다. | — |
| Object Thing | 정의된 속성입니다. | — |
| Number Value | 정의된 속성입니다. | — |
| Is Stated | 빈 문자열에 참입니다. | _Drawn as a solid line in the explainer._ |
| Subject Label | 연결된 ‘subject’ 레코드에서 가져온 값입니다. | — |
| Property Label | 연결된 ‘property’ 레코드에서 가져온 값입니다. | — |
| Object Label | 연결된 ‘object thing’ 레코드에서 가져온 값입니다. | — |
| Name | 다음과 같이 계산됩니다: ‘subject label’, 이어서 공백 한 칸, 이어서 ‘property label’, 이어서 공백 한 칸, 이어서 ‘object thing’ 값이 비어 있는 경우에는 ‘number value’, 그 밖의 경우에는 ‘object label’. | — |
| Count of Inverse Firings | 해당 ‘statement’ 레코드에 연결된 모든 ‘firings’ 레코드의 총 ‘inverse fired unit’. | — |
| Count of Symmetric Firings | 해당 ‘statement’ 레코드에 연결된 모든 ‘firings’ 레코드의 총 ‘symmetric fired unit’. | — |
| Holds Before Chain | 다음 중 하나 이상에 해당하는 경우(‘stated’ 플래그가 설정된 경우, ‘count of inverse firings’ 값이 0보다 큰 경우 또는 ‘count of symmetric firings’ 값이 0보다 큰 경우)에 참입니다. | — |
| Count of Chain Firings | 해당 ‘statement’ 레코드에 연결된 모든 ‘firings’ 레코드의 총 ‘chain fired unit’. | — |
| Holds Before Typing | 다음 중 하나 이상에 해당하는 경우(‘holds before chain’ 플래그가 설정된 경우 또는 ‘count of chain firings’ 값이 0보다 큰 경우)에 참입니다. | — |
| Count of Type Firings | 해당 ‘statement’ 레코드에 연결된 모든 ‘firings’ 레코드의 총 ‘type fired unit’. | — |
| Holds | 다음 중 하나 이상에 해당하는 경우(‘holds before typing’ 플래그가 설정된 경우 또는 ‘count of type firings’ 값이 0보다 큰 경우)에 참입니다. | _True when stated or concluded by an enabled rule._ |
| Is Conclusion | 다음 모두에 해당하는 경우(‘holds’ 플래그가 설정된 경우 및 ‘stated’ 플래그가 설정되지 않은 경우)에 참입니다. | _Drawn as a dashed line in the explainer._ |
| Worked Out by | 우선순위에 따라 결정됩니다: ‘conclusion’ 플래그가 설정되지 않은 경우에는 빈 문자열; ‘count of inverse firings’ 값이 0보다 큰 경우에는 “inverse”; ‘count of symmetric firings’ 값이 0보다 큰 경우에는 “symmetric”; ‘count of chain firings’ 값이 0보다 큰 경우에는 “property chain”; 그 밖의 경우에는 “domain and range”. | — |
| Stated Unit | 우선순위에 따라 결정됩니다: ‘stated’ 플래그가 설정된 경우에는 1; 그 밖의 경우에는 0. | — |
| Conclusion Unit | 우선순위에 따라 결정됩니다: ‘conclusion’ 플래그가 설정된 경우에는 1; 그 밖의 경우에는 0. | — |
| Parent Unit | 우선순위에 따라 결정됩니다: 다음 모두에 해당하는 경우(‘holds’ 플래그가 설정된 경우 및 ‘property’ 값이 “isParentOf”인 경우)에는 1; 그 밖의 경우에는 0. | — |
| Grandparent Unit | 우선순위에 따라 결정됩니다: 다음 모두에 해당하는 경우(‘holds’ 플래그가 설정된 경우 및 ‘property’ 값이 “isGrandparentOf”인 경우)에는 1; 그 밖의 경우에는 0. | — |

<details open>
<summary>예시 데이터</summary>

| Number Value | Is Stated | Subject Label ƒ | Property Label ƒ | Object Label ƒ | Name ƒ | Count of Inverse Firings ƒ | Count of Symmetric Firings ƒ | Holds Before Chain ƒ | Count of Chain Firings ƒ | Holds Before Typing ƒ | Count of Type Firings ƒ | Holds ƒ | Is Conclusion ƒ | Worked Out by ƒ | Stated Unit ƒ | Conclusion Unit ƒ | Parent Unit ƒ | Grandparent Unit ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| — | true | Marvin | is parent of | Randy | Marvin is parent of Randy | 0 | 0 | true | 0 | true | 0 | true | false | — | 1 | 0 | 1 | 0 |
| — | false | Marvin | is parent of | Sharon | Marvin is parent of Sharon | 0 | 0 | false | 0 | false | 0 | false | false | — | 0 | 0 | 0 | 0 |
| — | false | Marvin | is parent of | Shelly | Marvin is parent of Shelly | 0 | 0 | false | 0 | false | 0 | false | false | — | 0 | 0 | 0 | 0 |

_ƒ 표시는 계산된 열을 나타냅니다._

</details>

| 용어 | 설명 | 부연 설명 |
|------|-------------|-------------------|
| **Firing** | One way one rule could produce one fact from one or two others. | — |
| Rule | 정의된 속성입니다. | — |
| Kind | 정의된 속성입니다. | — |
| Target | 정의된 속성입니다. | — |
| First | 정의된 속성입니다. | — |
| Second | 정의된 속성입니다. | _Same as First when the rule needs one premise._ |
| Rule Enabled | 연결된 ‘rule’ 레코드가 ‘enabled’에 해당하는 경우에 참입니다. | — |
| Rule Label | 연결된 ‘rule’ 레코드에서 가져온 값입니다. | — |
| Target Name | 연결된 ‘target’ 레코드에서 가져온 값입니다. | — |
| First Name | 연결된 ‘first’ 레코드에서 가져온 값입니다. | — |
| Second Name | 연결된 ‘second’ 레코드에서 가져온 값입니다. | — |
| First is Stated | 연결된 ‘first’ 레코드가 ‘stated’에 해당하는 경우에 참입니다. | — |
| First Holds Before Chain | 연결된 ‘first’ 레코드가 ‘holds before chain’에 해당하는 경우에 참입니다. | — |
| Second Holds Before Chain | 연결된 ‘second’ 레코드가 ‘holds before chain’에 해당하는 경우에 참입니다. | — |
| First Holds Before Typing | 연결된 ‘first’ 레코드가 ‘holds before typing’에 해당하는 경우에 참입니다. | — |
| Inverse Fired Unit | 우선순위에 따라 결정됩니다: 다음 모두에 해당하는 경우(‘rule enabled’ 플래그가 설정된 경우, ‘kind’ 값이 “inverse”인 경우 및 ‘first is stated’ 플래그가 설정된 경우)에는 1; 그 밖의 경우에는 0. | — |
| Symmetric Fired Unit | 우선순위에 따라 결정됩니다: 다음 모두에 해당하는 경우(‘rule enabled’ 플래그가 설정된 경우, ‘kind’ 값이 “symmetric”인 경우 및 ‘first is stated’ 플래그가 설정된 경우)에는 1; 그 밖의 경우에는 0. | — |
| Chain Fired Unit | 우선순위에 따라 결정됩니다: 다음 모두에 해당하는 경우(‘rule enabled’ 플래그가 설정된 경우, ‘kind’ 값이 “chain”인 경우, ‘first holds before chain’ 플래그가 설정된 경우 및 ‘second holds before chain’ 플래그가 설정된 경우)에는 1; 그 밖의 경우에는 0. | — |
| Type Fired Unit | 우선순위에 따라 결정됩니다: 다음 모두에 해당하는 경우(‘rule enabled’ 플래그가 설정된 경우, 다음 중 하나 이상에 해당하는 경우(‘kind’ 값이 “domain”인 경우 또는 ‘kind’ 값이 “range”인 경우) 및 ‘first holds before typing’ 플래그가 설정된 경우)에는 1; 그 밖의 경우에는 0. | — |
| Fired Unit | 다음과 같이 계산됩니다: ‘inverse fired unit’ 더하기 ‘symmetric fired unit’ 더하기 ‘chain fired unit’ 더하기 ‘type fired unit’. | — |
| Name | 우선순위에 따라 결정됩니다: ‘fired unit’ 값이 0보다 큰 경우에는 ‘target name’, 이어서 “: worked out by the ”, 이어서 ‘rule label’, 이어서 “ rule from '”, 이어서 ‘first name’, 이어서 “'”, 이어서 “ and '”, 이어서 ‘second name’, 이어서 “'”; 그 밖의 경우에는 ‘firing ID’. | _The explainer's conclusion card._ |

<details open>
<summary>예시 데이터</summary>

| Kind | Rule Enabled ƒ | Rule Label ƒ | Target Name ƒ | First Name ƒ | Second Name ƒ | First is Stated ƒ | First Holds Before Chain ƒ | Second Holds Before Chain ƒ | First Holds Before Typing ƒ | Inverse Fired Unit ƒ | Symmetric Fired Unit ƒ | Chain Fired Unit ƒ | Type Fired Unit ƒ | Fired Unit ƒ | Name ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| inverse | true | Inverse | Randy is child of Marvin | Marvin is parent of Randy | Marvin is parent of Randy | true | true | true | true | 1 | 0 | 0 | 0 | 1 | Randy is child of Marvin: worked out by the Inverse rule from 'Marvin is parent of Randy' |
| inverse | true | Inverse | Sharon is child of Marvin | Marvin is parent of Sharon | Marvin is parent of Sharon | false | false | false | false | 0 | 0 | 0 | 0 | 0 | inverse:Marvin\|isParentOf\|Sharon>Sharon\|isChildOf\|Marvin |
| inverse | true | Inverse | Shelly is child of Marvin | Marvin is parent of Shelly | Marvin is parent of Shelly | false | false | false | false | 0 | 0 | 0 | 0 | 0 | inverse:Marvin\|isParentOf\|Shelly>Shelly\|isChildOf\|Marvin |

_ƒ 표시는 계산된 열을 나타냅니다._

</details>

| 용어 | 설명 | 부연 설명 |
|------|-------------|-------------------|
| **Disjoint Check** | Two statements that must not both hold. | — |
| Graph | 정의된 속성입니다. | — |
| Rule | 정의된 속성입니다. | — |
| A | 정의된 속성입니다. | — |
| B | 정의된 속성입니다. | — |
| Name | 같은 레코드의 ‘disjoint check ID’ 값과 같습니다. | — |
| Rule Enabled | 연결된 ‘rule’ 레코드가 ‘enabled’에 해당하는 경우에 참입니다. | — |
| A Holds | 연결된 ‘a’ 레코드가 ‘holds’에 해당하는 경우에 참입니다. | — |
| B Holds | 연결된 ‘b’ 레코드가 ‘holds’에 해당하는 경우에 참입니다. | — |
| Violation Unit | 우선순위에 따라 결정됩니다: 다음 모두에 해당하는 경우(‘rule enabled’ 플래그가 설정된 경우, ‘a holds’ 플래그가 설정된 경우 및 ‘b holds’ 플래그가 설정된 경우)에는 1; 그 밖의 경우에는 0. | — |

<details open>
<summary>예시 데이터</summary>

| Name ƒ | Rule Enabled ƒ | A Holds ƒ | B Holds ƒ | Violation Unit ƒ |
|---|---|---|---|---|
| Marvin\|isParentOf\|Randy~isSpouseOf | true | true | false | 0 |
| Marvin\|isParentOf\|Sharon~isSpouseOf | true | false | false | 0 |
| Marvin\|isParentOf\|Shelly~isSpouseOf | true | false | false | 0 |

_ƒ 표시는 계산된 열을 나타냅니다._

</details>

## 2 사실 유형

- **property** 레코드는 **property** 레코드를 하나 참조할 수 있습니다
- **property** 레코드는 **thing** 레코드를 하나 참조할 수 있습니다
- **statement** 레코드는 **graph** 레코드를 정확히 하나 참조합니다
- **statement** 레코드는 **thing** 레코드를 정확히 하나 참조합니다
- **statement** 레코드는 **property** 레코드를 정확히 하나 참조합니다
- **firing** 레코드는 **rule** 레코드를 정확히 하나 참조합니다
- **firing** 레코드는 **statement** 레코드를 정확히 하나 참조합니다
- **disjoint check** 레코드는 **graph** 레코드를 정확히 하나 참조합니다
- **disjoint check** 레코드는 **rule** 레코드를 정확히 하나 참조합니다
- **disjoint check** 레코드는 **statement** 레코드를 정확히 하나 참조합니다

## 3 운영 규칙

_운영 규칙은 비즈니스가 **의무화**하거나 **금지**하거나 권고하는(**하는 것이 좋습니다**)
사항을 서술합니다. 구조적 규칙은 필수 필드와 외래 키에서 도출되고, 의미적 규칙은
Constraints 테이블에서 도출되며, 각 의미적 규칙은 룰북이 이미 계산하는 불리언 값을
판단 기준으로 삼습니다(아래 정의 규칙에서 DR-N으로 상호 참조)._

### 구조적 제약(스키마에서 도출)

- ‘rule’ 레코드는 ‘label’ 및 ‘explanation’ 값을 포함하고, 해당 레코드가 ‘enabled’에 해당하는지 여부를 기록**해야 합니다**.
- ‘thing’ 레코드는 ‘label’ 및 ‘kind’ 값을 포함**해야 합니다**.
- ‘property’ 레코드는 ‘label’ 값을 포함하고, 해당 레코드가 ‘symmetric’에 해당하는지 여부를 기록**해야 합니다**.
- ‘statement’ 레코드는 ‘graph’ 레코드를 정확히 하나 참조**해야 합니다**.
- ‘statement’ 레코드는 ‘subject’ 필드를 통해 ‘thing’ 레코드를 정확히 하나 참조**해야 합니다**.
- ‘statement’ 레코드는 ‘property’ 레코드를 정확히 하나 참조**해야 합니다**.
- ‘statement’ 레코드는 해당 레코드가 ‘stated’에 해당하는지 여부를 기록**해야 합니다**.
- ‘firing’ 레코드는 ‘rule’ 레코드를 정확히 하나 참조**해야 합니다**.
- ‘firing’ 레코드는 ‘target’ 필드를 통해 ‘statement’ 레코드를 정확히 하나 참조**해야 합니다**.
- ‘firing’ 레코드는 ‘first’ 필드를 통해 ‘statement’ 레코드를 정확히 하나 참조**해야 합니다**.
- ‘firing’ 레코드는 ‘second’ 필드를 통해 ‘statement’ 레코드를 정확히 하나 참조**해야 합니다**.
- ‘firing’ 레코드는 ‘kind’ 값을 포함**해야 합니다**.
- ‘disjoint check’ 레코드는 ‘graph’ 레코드를 정확히 하나 참조**해야 합니다**.
- ‘disjoint check’ 레코드는 ‘rule’ 레코드를 정확히 하나 참조**해야 합니다**.
- ‘disjoint check’ 레코드는 ‘a’ 필드를 통해 ‘statement’ 레코드를 정확히 하나 참조**해야 합니다**.
- ‘disjoint check’ 레코드는 ‘b’ 필드를 통해 ‘statement’ 레코드를 정확히 하나 참조**해야 합니다**.

## 4 정의 규칙

_모든 문장은 비즈니스 영역에서 성립하는 사실을 서술하며, 절차나 명령이 아닙니다.
한 방향의 필요조건이 동치로 오해되지 않도록 “iff”(필요충분조건) 대신 “경우에만”이라는
표현을 사용합니다. **⚠︎ 기계적 표현** 표시는 규칙에서 그대로 생성한 문장이 정확하기는 하지만
어색한 규칙에 붙습니다. 원하면 문장을 다듬어도 좋다는 표시일 뿐, 결함이 아닙니다._

| ID | 선언적 규칙 |
|----|------------------|
| **DR-1 Name** | ‘graph’ 레코드의 ‘name’ 값은 같은 레코드의 ‘graph ID’ 값과 같습니다. |
| **DR-2 Count of Stated Facts** | ‘graph’ 레코드의 ‘count of stated facts’ 값은 해당 ‘graph’ 레코드에 연결된 모든 ‘statements’ 레코드의 총 ‘stated unit’입니다. |
| **DR-3 Count of Conclusions** | ‘graph’ 레코드의 ‘count of conclusions’ 값은 해당 ‘graph’ 레코드에 연결된 모든 ‘statements’ 레코드의 총 ‘conclusion unit’입니다. |
| **DR-4 Count of Violations** | ‘graph’ 레코드의 ‘count of violations’ 값은 해당 ‘graph’ 레코드에 연결된 모든 ‘disjoint checks’ 레코드의 총 ‘violation unit’입니다. |
| **DR-5 Is Consistent** | ‘count of violations’ 값이 0인 경우, ‘graph’ 레코드는 ‘consistent’에 해당하는 것으로 간주됩니다. |
| **DR-6 Name** | ‘rule’ 레코드의 ‘name’ 값은 같은 레코드의 ‘label’ 값과 같습니다. |
| **DR-7 Count of Firings** | ‘rule’ 레코드의 ‘count of firings’ 값은 해당 ‘rule’ 레코드에 연결된 모든 ‘firings’ 레코드의 총 ‘fired unit’입니다. |
| **DR-8 Name** | ‘thing’ 레코드의 ‘name’ 값은 같은 레코드의 ‘label’ 값과 같습니다. |
| **DR-9 Count of Children** | ‘thing’ 레코드의 ‘count of children’ 값은 해당 ‘thing’ 레코드에 연결된 모든 ‘statements’ 레코드의 총 ‘parent unit’입니다. |
| **DR-10 Count of Grandchildren** | ‘thing’ 레코드의 ‘count of grandchildren’ 값은 해당 ‘thing’ 레코드에 연결된 모든 ‘statements’ 레코드의 총 ‘grandparent unit’입니다. |
| **DR-11 Name** | ‘property’ 레코드의 ‘name’ 값은 같은 레코드의 ‘label’ 값과 같습니다. |
| **DR-12 Subject Label** | ‘statement’ 레코드의 ‘subject label’ 값은 연결된 ‘subject’ 레코드에서 가져옵니다. |
| **DR-13 Property Label** | ‘statement’ 레코드의 ‘property label’ 값은 연결된 ‘property’ 레코드에서 가져옵니다. |
| **DR-14 Object Label** | ‘statement’ 레코드의 ‘object label’ 값은 연결된 ‘object thing’ 레코드에서 가져옵니다. |
| **DR-15 Name** | ‘statement’ 레코드의 ‘name’ 값은 다음과 같이 계산됩니다: ‘subject label’, 이어서 공백 한 칸, 이어서 ‘property label’, 이어서 공백 한 칸, 이어서 ‘object thing’ 값이 비어 있는 경우에는 ‘number value’, 그 밖의 경우에는 ‘object label’. |
| **DR-16 Count of Inverse Firings** | ‘statement’ 레코드의 ‘count of inverse firings’ 값은 해당 ‘statement’ 레코드에 연결된 모든 ‘firings’ 레코드의 총 ‘inverse fired unit’입니다. |
| **DR-17 Count of Symmetric Firings** | ‘statement’ 레코드의 ‘count of symmetric firings’ 값은 해당 ‘statement’ 레코드에 연결된 모든 ‘firings’ 레코드의 총 ‘symmetric fired unit’입니다. |
| **DR-18 Holds Before Chain** | 다음 중 하나 이상에 해당하는 경우(‘stated’ 플래그가 설정된 경우, ‘count of inverse firings’ 값이 0보다 큰 경우 또는 ‘count of symmetric firings’ 값이 0보다 큰 경우), ‘statement’ 레코드는 ‘holds before chain’ 조건에 해당하는 것으로 간주됩니다. |
| **DR-19 Count of Chain Firings** | ‘statement’ 레코드의 ‘count of chain firings’ 값은 해당 ‘statement’ 레코드에 연결된 모든 ‘firings’ 레코드의 총 ‘chain fired unit’입니다. |
| **DR-20 Holds Before Typing** | 다음 중 하나 이상에 해당하는 경우(‘holds before chain’ 플래그가 설정된 경우 또는 ‘count of chain firings’ 값이 0보다 큰 경우), ‘statement’ 레코드는 ‘holds before typing’ 조건에 해당하는 것으로 간주됩니다. |
| **DR-21 Count of Type Firings** | ‘statement’ 레코드의 ‘count of type firings’ 값은 해당 ‘statement’ 레코드에 연결된 모든 ‘firings’ 레코드의 총 ‘type fired unit’입니다. |
| **DR-22 Holds** | 다음 중 하나 이상에 해당하는 경우(‘holds before typing’ 플래그가 설정된 경우 또는 ‘count of type firings’ 값이 0보다 큰 경우), ‘statement’ 레코드는 ‘holds’ 조건에 해당하는 것으로 간주됩니다. |
| **DR-23 Is Conclusion** | 다음 모두에 해당하는 경우(‘holds’ 플래그가 설정된 경우 및 ‘stated’ 플래그가 설정되지 않은 경우), ‘statement’ 레코드는 ‘conclusion’에 해당하는 것으로 간주됩니다. |
| **DR-24 Worked Out by** | ‘statement’ 레코드의 ‘worked out by’ 값은 다음 우선순위에 따라 결정됩니다:<br>1. ‘conclusion’ 플래그가 설정되지 않은 경우: 빈 문자열<br>2. ‘count of inverse firings’ 값이 0보다 큰 경우: “inverse”<br>3. ‘count of symmetric firings’ 값이 0보다 큰 경우: “symmetric”<br>4. ‘count of chain firings’ 값이 0보다 큰 경우: “property chain”<br>5. 그 밖의 경우: “domain and range”. |
| **DR-25 Stated Unit** | ‘statement’ 레코드의 ‘stated unit’ 값은 다음 우선순위에 따라 결정됩니다:<br>1. ‘stated’ 플래그가 설정된 경우: 1<br>2. 그 밖의 경우: 0. |
| **DR-26 Conclusion Unit** | ‘statement’ 레코드의 ‘conclusion unit’ 값은 다음 우선순위에 따라 결정됩니다:<br>1. ‘conclusion’ 플래그가 설정된 경우: 1<br>2. 그 밖의 경우: 0. |
| **DR-27 Parent Unit** | ‘statement’ 레코드의 ‘parent unit’ 값은 다음 우선순위에 따라 결정됩니다:<br>1. 다음 모두에 해당하는 경우(‘holds’ 플래그가 설정된 경우 및 ‘property’ 값이 “isParentOf”인 경우): 1<br>2. 그 밖의 경우: 0. |
| **DR-28 Grandparent Unit** | ‘statement’ 레코드의 ‘grandparent unit’ 값은 다음 우선순위에 따라 결정됩니다:<br>1. 다음 모두에 해당하는 경우(‘holds’ 플래그가 설정된 경우 및 ‘property’ 값이 “isGrandparentOf”인 경우): 1<br>2. 그 밖의 경우: 0. |
| **DR-29 Rule Enabled** | ‘firing’ 레코드의 ‘rule enabled’ 값은 연결된 ‘rule’ 레코드가 ‘enabled’에 해당하는 경우에 참입니다. |
| **DR-30 Rule Label** | ‘firing’ 레코드의 ‘rule label’ 값은 연결된 ‘rule’ 레코드에서 가져옵니다. |
| **DR-31 Target Name** | ‘firing’ 레코드의 ‘target name’ 값은 연결된 ‘target’ 레코드에서 가져옵니다. |
| **DR-32 First Name** | ‘firing’ 레코드의 ‘first name’ 값은 연결된 ‘first’ 레코드에서 가져옵니다. |
| **DR-33 Second Name** | ‘firing’ 레코드의 ‘second name’ 값은 연결된 ‘second’ 레코드에서 가져옵니다. |
| **DR-34 First is Stated** | ‘firing’ 레코드의 ‘first is stated’ 값은 연결된 ‘first’ 레코드가 ‘stated’에 해당하는 경우에 참입니다. |
| **DR-35 First Holds Before Chain** | ‘firing’ 레코드의 ‘first holds before chain’ 값은 연결된 ‘first’ 레코드가 ‘holds before chain’에 해당하는 경우에 참입니다. |
| **DR-36 Second Holds Before Chain** | ‘firing’ 레코드의 ‘second holds before chain’ 값은 연결된 ‘second’ 레코드가 ‘holds before chain’에 해당하는 경우에 참입니다. |
| **DR-37 First Holds Before Typing** | ‘firing’ 레코드의 ‘first holds before typing’ 값은 연결된 ‘first’ 레코드가 ‘holds before typing’에 해당하는 경우에 참입니다. |
| **DR-38 Inverse Fired Unit** | ‘firing’ 레코드의 ‘inverse fired unit’ 값은 다음 우선순위에 따라 결정됩니다:<br>1. 다음 모두에 해당하는 경우(‘rule enabled’ 플래그가 설정된 경우, ‘kind’ 값이 “inverse”인 경우 및 ‘first is stated’ 플래그가 설정된 경우): 1<br>2. 그 밖의 경우: 0. |
| **DR-39 Symmetric Fired Unit** | ‘firing’ 레코드의 ‘symmetric fired unit’ 값은 다음 우선순위에 따라 결정됩니다:<br>1. 다음 모두에 해당하는 경우(‘rule enabled’ 플래그가 설정된 경우, ‘kind’ 값이 “symmetric”인 경우 및 ‘first is stated’ 플래그가 설정된 경우): 1<br>2. 그 밖의 경우: 0. |
| **DR-40 Chain Fired Unit** | ‘firing’ 레코드의 ‘chain fired unit’ 값은 다음 우선순위에 따라 결정됩니다:<br>1. 다음 모두에 해당하는 경우(‘rule enabled’ 플래그가 설정된 경우, ‘kind’ 값이 “chain”인 경우, ‘first holds before chain’ 플래그가 설정된 경우 및 ‘second holds before chain’ 플래그가 설정된 경우): 1<br>2. 그 밖의 경우: 0. |
| **DR-41 Type Fired Unit** | ‘firing’ 레코드의 ‘type fired unit’ 값은 다음 우선순위에 따라 결정됩니다:<br>1. 다음 모두에 해당하는 경우(‘rule enabled’ 플래그가 설정된 경우, 다음 중 하나 이상에 해당하는 경우(‘kind’ 값이 “domain”인 경우 또는 ‘kind’ 값이 “range”인 경우) 및 ‘first holds before typing’ 플래그가 설정된 경우): 1<br>2. 그 밖의 경우: 0. |
| **DR-42 Fired Unit** | ‘firing’ 레코드의 ‘fired unit’ 값은 다음과 같이 계산됩니다: ‘inverse fired unit’ 더하기 ‘symmetric fired unit’ 더하기 ‘chain fired unit’ 더하기 ‘type fired unit’. |
| **DR-43 Name** | ‘firing’ 레코드의 ‘name’ 값은 다음 우선순위에 따라 결정됩니다:<br>1. ‘fired unit’ 값이 0보다 큰 경우: ‘target name’, 이어서 “: worked out by the ”, 이어서 ‘rule label’, 이어서 “ rule from '”, 이어서 ‘first name’, 이어서 “'”, 이어서 “ and '”, 이어서 ‘second name’, 이어서 “'”<br>2. 그 밖의 경우: ‘firing ID’. |
| **DR-44 Name** | ‘disjoint check’ 레코드의 ‘name’ 값은 같은 레코드의 ‘disjoint check ID’ 값과 같습니다. |
| **DR-45 Rule Enabled** | ‘disjoint check’ 레코드의 ‘rule enabled’ 값은 연결된 ‘rule’ 레코드가 ‘enabled’에 해당하는 경우에 참입니다. |
| **DR-46 A Holds** | ‘disjoint check’ 레코드의 ‘a holds’ 값은 연결된 ‘a’ 레코드가 ‘holds’에 해당하는 경우에 참입니다. |
| **DR-47 B Holds** | ‘disjoint check’ 레코드의 ‘b holds’ 값은 연결된 ‘b’ 레코드가 ‘holds’에 해당하는 경우에 참입니다. |
| **DR-48 Violation Unit** | ‘disjoint check’ 레코드의 ‘violation unit’ 값은 다음 우선순위에 따라 결정됩니다:<br>1. 다음 모두에 해당하는 경우(‘rule enabled’ 플래그가 설정된 경우, ‘a holds’ 플래그가 설정된 경우 및 ‘b holds’ 플래그가 설정된 경우): 1<br>2. 그 밖의 경우: 0. |

## 5 스키마 추적성

_표현식 열은 문서 표기법으로 적은 규칙의 정의입니다.
룰북에 저장된 것과 같은 논리를 비즈니스 독자를 위해 풀어 쓴 것입니다._

| 스키마 요소 | 종류 | 표현식 |
|----------------|------|------------|
| **Graphs.Name** | 수식 | `GraphId` |
| **Graphs.CountOfStatedFacts** | 집계 | `Sum(Statements.StatedUnit via Graph)` |
| **Graphs.CountOfConclusions** | 집계 | `Sum(Statements.ConclusionUnit via Graph)` |
| **Graphs.CountOfViolations** | 집계 | `Sum(DisjointChecks.ViolationUnit via Graph)` |
| **Graphs.IsConsistent** | 수식 | `CountOfViolations = 0` |
| **Rules.Name** | 수식 | `Label` |
| **Rules.CountOfFirings** | 집계 | `Sum(Firings.FiredUnit via Rule)` |
| **Things.Name** | 수식 | `Label` |
| **Things.CountOfChildren** | 집계 | `Sum(Statements.ParentUnit via Subject)` |
| **Things.CountOfGrandchildren** | 집계 | `Sum(Statements.GrandparentUnit via Subject)` |
| **Properties.Name** | 수식 | `Label` |
| **Statements.SubjectLabel** | 조회 | `Lookup(Things.Label via Subject)` |
| **Statements.PropertyLabel** | 조회 | `Lookup(Properties.Label via Property)` |
| **Statements.ObjectLabel** | 조회 | `Lookup(Things.Label via ObjectThing)` |
| **Statements.Name** | 수식 | `SubjectLabel & " " & PropertyLabel & " " & If(ObjectThing = Blank(), NumberValue, ObjectLabel)` |
| **Statements.CountOfInverseFirings** | 집계 | `Sum(Firings.InverseFiredUnit via Target)` |
| **Statements.CountOfSymmetricFirings** | 집계 | `Sum(Firings.SymmetricFiredUnit via Target)` |
| **Statements.HoldsBeforeChain** | 수식 | `Or(IsStated, CountOfInverseFirings > 0, CountOfSymmetricFirings > 0)` |
| **Statements.CountOfChainFirings** | 집계 | `Sum(Firings.ChainFiredUnit via Target)` |
| **Statements.HoldsBeforeTyping** | 수식 | `Or(HoldsBeforeChain, CountOfChainFirings > 0)` |
| **Statements.CountOfTypeFirings** | 집계 | `Sum(Firings.TypeFiredUnit via Target)` |
| **Statements.Holds** | 수식 | `Or(HoldsBeforeTyping, CountOfTypeFirings > 0)` |
| **Statements.IsConclusion** | 수식 | `And(Holds, Not(IsStated))` |
| **Statements.WorkedOutBy** | 수식 | `If(Not(IsConclusion), "", If(CountOfInverseFirings > 0, "inverse", If(CountOfSymmetricFirings > 0, "symmetric", If(CountOfChainFirings > 0, "property chain", "domain and range"))))` |
| **Statements.StatedUnit** | 수식 | `If(IsStated, 1, 0)` |
| **Statements.ConclusionUnit** | 수식 | `If(IsConclusion, 1, 0)` |
| **Statements.ParentUnit** | 수식 | `If(And(Holds, Property = "isParentOf"), 1, 0)` |
| **Statements.GrandparentUnit** | 수식 | `If(And(Holds, Property = "isGrandparentOf"), 1, 0)` |
| **Firings.RuleEnabled** | 조회 | `Lookup(Rules.IsEnabled via Rule)` |
| **Firings.RuleLabel** | 조회 | `Lookup(Rules.Label via Rule)` |
| **Firings.TargetName** | 조회 | `Lookup(Statements.Name via Target)` |
| **Firings.FirstName** | 조회 | `Lookup(Statements.Name via First)` |
| **Firings.SecondName** | 조회 | `Lookup(Statements.Name via Second)` |
| **Firings.FirstIsStated** | 조회 | `Lookup(Statements.IsStated via First)` |
| **Firings.FirstHoldsBeforeChain** | 조회 | `Lookup(Statements.HoldsBeforeChain via First)` |
| **Firings.SecondHoldsBeforeChain** | 조회 | `Lookup(Statements.HoldsBeforeChain via Second)` |
| **Firings.FirstHoldsBeforeTyping** | 조회 | `Lookup(Statements.HoldsBeforeTyping via First)` |
| **Firings.InverseFiredUnit** | 수식 | `If(And(RuleEnabled, Kind = "inverse", FirstIsStated), 1, 0)` |
| **Firings.SymmetricFiredUnit** | 수식 | `If(And(RuleEnabled, Kind = "symmetric", FirstIsStated), 1, 0)` |
| **Firings.ChainFiredUnit** | 수식 | `If(And(RuleEnabled, Kind = "chain", FirstHoldsBeforeChain, SecondHoldsBeforeChain), 1, 0)` |
| **Firings.TypeFiredUnit** | 수식 | `If(And(RuleEnabled, Or(Kind = "domain", Kind = "range"), FirstHoldsBeforeTyping), 1, 0)` |
| **Firings.FiredUnit** | 수식 | `InverseFiredUnit + SymmetricFiredUnit + ChainFiredUnit + TypeFiredUnit` |
| **Firings.Name** | 수식 | `If(FiredUnit > 0, TargetName & ": worked out by the " & RuleLabel & " rule from '" & FirstName & "'" & If(Kind = "chain", " and '" & SecondName & "'", ""), FiringId)` |
| **DisjointChecks.Name** | 수식 | `DisjointCheckId` |
| **DisjointChecks.RuleEnabled** | 조회 | `Lookup(Rules.IsEnabled via Rule)` |
| **DisjointChecks.AHolds** | 조회 | `Lookup(Statements.Holds via A)` |
| **DisjointChecks.BHolds** | 조회 | `Lookup(Statements.Holds via B)` |
| **DisjointChecks.ViolationUnit** | 수식 | `If(And(RuleEnabled, AHolds, BHolds), 1, 0)` |
