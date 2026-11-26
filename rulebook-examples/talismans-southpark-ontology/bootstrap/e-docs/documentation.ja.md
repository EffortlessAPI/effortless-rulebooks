# 📘 South Park ontology explainer — ドキュメント

_The barebones South Park ontology from Jessica Talisman's explainer: ten stated facts and five rules that can each be switched on. Conclusions are formulas._

> ルールブックから生成された宣言的ビジネスルールです。以下の各文はビジネス
> ドメインにおける真実を表し、手続きでも命令でもありません。ルールブックの
> 数式が唯一の真実の源であり、本書はその平易な読み下しです。

## 1 ビジネス語彙

| 用語 | 説明 | 補足コメント |
|------|-------------|-------------------|
| **Graph** | The whole example. | — |
| Name | graph ID と同じ。 | — |
| Count of Stated Facts | Graphに関連する「statements」全体での「stated unit」の合計。 | — |
| Count of Conclusions | Graphに関連する「statements」全体での「conclusion unit」の合計。 | _Facts that hold but were never stated._ |
| Count of Violations | Graphに関連する「disjoint checks」全体での「violation unit」の合計。 | — |
| Is Consistent | count of violations が 0 であるとき真。 | — |

<details open>
<summary>サンプルデータ</summary>

| Name ƒ | Count of Stated Facts ƒ | Count of Conclusions ƒ | Count of Violations ƒ | Is Consistent ƒ |
|---|---|---|---|---|
| south-park | 11 | 9 | 0 | true |

_ƒ は計算列を示します。_

</details>

| 用語 | 説明 | 補足コメント |
|------|-------------|-------------------|
| **Rule** | The five rules; IsEnabled is the checkbox. | — |
| Label | 定義済みの属性。 | — |
| Explanation | 定義済みの属性。 | — |
| Is Enabled | 空文字列とき真。 | — |
| Name | label と同じ。 | — |
| Count of Firings | Ruleに関連する「firings」全体での「fired unit」の合計。 | _How many times the rule produced a fact._ |

<details open>
<summary>サンプルデータ</summary>

| Label | Explanation | Is Enabled | Name ƒ | Count of Firings ƒ |
|---|---|---|---|---|
| Inverse | A parent link also gives a child link between the same two people. | true | Inverse | 4 |
| Symmetric | A friend link or a spouse link is true for both people. | true | Symmetric | 2 |
| Property chain | A parent of a parent is a grandparent. | true | Property chain | 2 |

_ƒ は計算列を示します。_

</details>

| 用語 | 説明 | 補足コメント |
|------|-------------|-------------------|
| **Thing** | A node in the picture: a person, a place, a class or a role value. | — |
| Label | 定義済みの属性。 | — |
| Kind | 定義済みの属性。 | — |
| Name | label と同じ。 | — |
| Count of Children | Thingに関連する「statements」全体での「parent unit」の合計。 | _Added: a count, which a reasoner alone does not give._ |
| Count of Grandchildren | Thingに関連する「statements」全体での「grandparent unit」の合計。 | _Added: counts concluded grandchildren._ |

<details open>
<summary>サンプルデータ</summary>

| Label | Kind | Name ƒ | Count of Children ƒ | Count of Grandchildren ƒ |
|---|---|---|---|---|
| Marvin | individual | Marvin | 1 | 2 |
| Randy | individual | Randy | 2 | 0 |
| Sharon | individual | Sharon | 1 | 0 |

_ƒ は計算列を示します。_

</details>

| 用語 | 説明 | 補足コメント |
|------|-------------|-------------------|
| **Property** | A kind of link, with the characteristics the rules use. | — |
| Label | 定義済みの属性。 | — |
| Inverse of | 定義済みの属性。 | — |
| Is Symmetric | 空文字列とき真。 | — |
| Chain First | 定義済みの属性。 | — |
| Chain Second | 定義済みの属性。 | — |
| Domain Class | 定義済みの属性。 | — |
| Range Class | 定義済みの属性。 | — |
| Disjoint With | 定義済みの属性。 | — |
| Name | label と同じ。 | — |

<details open>
<summary>サンプルデータ</summary>

| Label | Is Symmetric | Name ƒ |
|---|---|---|
| is parent of | false | is parent of |
| is child of | false | is child of |
| is spouse of | true | is spouse of |

_ƒ は計算列を示します。_

</details>

| 用語 | 説明 | 補足コメント |
|------|-------------|-------------------|
| **Statement** | A fact that is stated, or could be concluded: Subject, Property, Object. | — |
| Graph | 定義済みの属性。 | — |
| Subject | 定義済みの属性。 | — |
| Property | 定義済みの属性。 | — |
| Object Thing | 定義済みの属性。 | — |
| Number Value | 定義済みの属性。 | — |
| Is Stated | 空文字列とき真。 | _Drawn as a solid line in the explainer._ |
| Subject Label | リンクされた「subject」から取得。 | — |
| Property Label | リンクされた「property」から取得。 | — |
| Object Label | リンクされた「object thing」から取得。 | — |
| Name | subject label、続いて 空白 1 つ、続いて property label、続いて 空白 1 つ、続いて object thing が空である場合は number value、それ以外の場合は object label として計算。 | — |
| Count of Inverse Firings | Statementに関連する「firings」全体での「inverse fired unit」の合計。 | — |
| Count of Symmetric Firings | Statementに関連する「firings」全体での「symmetric fired unit」の合計。 | — |
| Holds Before Chain | 次のいずれかが成り立つ: フラグ「stated」が設定されている、count of inverse firings が 0 より大きい、またはcount of symmetric firings が 0 より大きいとき真。 | — |
| Count of Chain Firings | Statementに関連する「firings」全体での「chain fired unit」の合計。 | — |
| Holds Before Typing | 次のいずれかが成り立つ: フラグ「holds before chain」が設定されているまたはcount of chain firings が 0 より大きいとき真。 | — |
| Count of Type Firings | Statementに関連する「firings」全体での「type fired unit」の合計。 | — |
| Holds | 次のいずれかが成り立つ: フラグ「holds before typing」が設定されているまたはcount of type firings が 0 より大きいとき真。 | _True when stated or concluded by an enabled rule._ |
| Is Conclusion | 次のすべてが成り立つ: フラグ「holds」が設定されているおよびフラグ「stated」が設定されていないとき真。 | _Drawn as a dashed line in the explainer._ |
| Worked Out by | 優先順位で決定: フラグ「conclusion」が設定されていない場合は 空文字列; count of inverse firings が 0 より大きい場合は 「inverse」; count of symmetric firings が 0 より大きい場合は 「symmetric」; count of chain firings が 0 より大きい場合は 「property chain」; それ以外の場合は 「domain and range」。 | — |
| Stated Unit | 優先順位で決定: フラグ「stated」が設定されている場合は 1; それ以外の場合は 0。 | — |
| Conclusion Unit | 優先順位で決定: フラグ「conclusion」が設定されている場合は 1; それ以外の場合は 0。 | — |
| Parent Unit | 優先順位で決定: 次のすべてが成り立つ: フラグ「holds」が設定されているおよびproperty が 「isParentOf」 である場合は 1; それ以外の場合は 0。 | — |
| Grandparent Unit | 優先順位で決定: 次のすべてが成り立つ: フラグ「holds」が設定されているおよびproperty が 「isGrandparentOf」 である場合は 1; それ以外の場合は 0。 | — |

<details open>
<summary>サンプルデータ</summary>

| Number Value | Is Stated | Subject Label ƒ | Property Label ƒ | Object Label ƒ | Name ƒ | Count of Inverse Firings ƒ | Count of Symmetric Firings ƒ | Holds Before Chain ƒ | Count of Chain Firings ƒ | Holds Before Typing ƒ | Count of Type Firings ƒ | Holds ƒ | Is Conclusion ƒ | Worked Out by ƒ | Stated Unit ƒ | Conclusion Unit ƒ | Parent Unit ƒ | Grandparent Unit ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| — | true | Marvin | is parent of | Randy | Marvin is parent of Randy | 0 | 0 | true | 0 | true | 0 | true | false | — | 1 | 0 | 1 | 0 |
| — | false | Marvin | is parent of | Sharon | Marvin is parent of Sharon | 0 | 0 | false | 0 | false | 0 | false | false | — | 0 | 0 | 0 | 0 |
| — | false | Marvin | is parent of | Shelly | Marvin is parent of Shelly | 0 | 0 | false | 0 | false | 0 | false | false | — | 0 | 0 | 0 | 0 |

_ƒ は計算列を示します。_

</details>

| 用語 | 説明 | 補足コメント |
|------|-------------|-------------------|
| **Firing** | One way one rule could produce one fact from one or two others. | — |
| Rule | 定義済みの属性。 | — |
| Kind | 定義済みの属性。 | — |
| Target | 定義済みの属性。 | — |
| First | 定義済みの属性。 | — |
| Second | 定義済みの属性。 | _Same as First when the rule needs one premise._ |
| Rule Enabled | リンクされた「rule」が「Enabled」であるとき真。 | — |
| Rule Label | リンクされた「rule」から取得。 | — |
| Target Name | リンクされた「target」から取得。 | — |
| First Name | リンクされた「first」から取得。 | — |
| Second Name | リンクされた「second」から取得。 | — |
| First is Stated | リンクされた「first」が「Stated」であるとき真。 | — |
| First Holds Before Chain | リンクされた「first」が「Holds Before Chain」であるとき真。 | — |
| Second Holds Before Chain | リンクされた「second」が「Holds Before Chain」であるとき真。 | — |
| First Holds Before Typing | リンクされた「first」が「Holds Before Typing」であるとき真。 | — |
| Inverse Fired Unit | 優先順位で決定: 次のすべてが成り立つ: フラグ「rule enabled」が設定されている、kind が 「inverse」 である、およびフラグ「first is stated」が設定されている場合は 1; それ以外の場合は 0。 | — |
| Symmetric Fired Unit | 優先順位で決定: 次のすべてが成り立つ: フラグ「rule enabled」が設定されている、kind が 「symmetric」 である、およびフラグ「first is stated」が設定されている場合は 1; それ以外の場合は 0。 | — |
| Chain Fired Unit | 優先順位で決定: 次のすべてが成り立つ: フラグ「rule enabled」が設定されている、kind が 「chain」 である、フラグ「first holds before chain」が設定されている、およびフラグ「second holds before chain」が設定されている場合は 1; それ以外の場合は 0。 | — |
| Type Fired Unit | 優先順位で決定: 次のすべてが成り立つ: フラグ「rule enabled」が設定されている、次のいずれかが成り立つ: kind が 「domain」 であるまたはkind が 「range」 である、およびフラグ「first holds before typing」が設定されている場合は 1; それ以外の場合は 0。 | — |
| Fired Unit | inverse fired unit たす symmetric fired unit たす chain fired unit たす type fired unit として計算。 | — |
| Name | 優先順位で決定: fired unit が 0 より大きい場合は target name、続いて 「: concluded by the 」、続いて rule label、続いて 「 rule from '」、続いて first name、続いて 「'」、続いて 「 and '」、続いて second name、続いて 「'」; それ以外の場合は firing ID。 | _The explainer's conclusion card._ |

<details open>
<summary>サンプルデータ</summary>

| Kind | Rule Enabled ƒ | Rule Label ƒ | Target Name ƒ | First Name ƒ | Second Name ƒ | First is Stated ƒ | First Holds Before Chain ƒ | Second Holds Before Chain ƒ | First Holds Before Typing ƒ | Inverse Fired Unit ƒ | Symmetric Fired Unit ƒ | Chain Fired Unit ƒ | Type Fired Unit ƒ | Fired Unit ƒ | Name ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| inverse | true | Inverse | Randy is child of Marvin | Marvin is parent of Randy | Marvin is parent of Randy | true | true | true | true | 1 | 0 | 0 | 0 | 1 | Randy is child of Marvin: concluded by the Inverse rule from 'Marvin is parent of Randy' |
| inverse | true | Inverse | Sharon is child of Marvin | Marvin is parent of Sharon | Marvin is parent of Sharon | false | false | false | false | 0 | 0 | 0 | 0 | 0 | inverse:Marvin\|isParentOf\|Sharon>Sharon\|isChildOf\|Marvin |
| inverse | true | Inverse | Shelly is child of Marvin | Marvin is parent of Shelly | Marvin is parent of Shelly | false | false | false | false | 0 | 0 | 0 | 0 | 0 | inverse:Marvin\|isParentOf\|Shelly>Shelly\|isChildOf\|Marvin |

_ƒ は計算列を示します。_

</details>

| 用語 | 説明 | 補足コメント |
|------|-------------|-------------------|
| **Disjoint Check** | Two statements that must not both hold. | — |
| Graph | 定義済みの属性。 | — |
| Rule | 定義済みの属性。 | — |
| A | 定義済みの属性。 | — |
| B | 定義済みの属性。 | — |
| Name | disjoint check ID と同じ。 | — |
| Rule Enabled | リンクされた「rule」が「Enabled」であるとき真。 | — |
| A Holds | リンクされた「a」が「Holds」であるとき真。 | — |
| B Holds | リンクされた「b」が「Holds」であるとき真。 | — |
| Violation Unit | 優先順位で決定: 次のすべてが成り立つ: フラグ「rule enabled」が設定されている、フラグ「a holds」が設定されている、およびフラグ「b holds」が設定されている場合は 1; それ以外の場合は 0。 | — |

<details open>
<summary>サンプルデータ</summary>

| Name ƒ | Rule Enabled ƒ | A Holds ƒ | B Holds ƒ | Violation Unit ƒ |
|---|---|---|---|---|
| Marvin\|isParentOf\|Randy~isSpouseOf | true | true | false | 0 |
| Marvin\|isParentOf\|Sharon~isSpouseOf | true | false | false | 0 |
| Marvin\|isParentOf\|Shelly~isSpouseOf | true | false | false | 0 |

_ƒ は計算列を示します。_

</details>

## 2 ファクトタイプ

- **property**は**property**を 1 件参照できます
- **property**は**thing**を 1 件参照できます
- **statement**は**graph**をちょうど 1 件参照します
- **statement**は**thing**をちょうど 1 件参照します
- **statement**は**property**をちょうど 1 件参照します
- **firing**は**rule**をちょうど 1 件参照します
- **firing**は**statement**をちょうど 1 件参照します
- **disjoint check**は**graph**をちょうど 1 件参照します
- **disjoint check**は**rule**をちょうど 1 件参照します
- **disjoint check**は**statement**をちょうど 1 件参照します

## 3 運用ルール

_運用ルールは、ビジネスが**義務付ける**こと、**禁止する**こと、
**推奨する**ことを述べます。構造ルールは必須フィールドと外部キーから、
意味ルールは Constraints テーブルから得られ、いずれもルールブックが既に計算する
ブール値に基づきます(下の定義ルールで DR-N として相互参照)。_

### 構造的制約(スキーマ由来)

- ruleは labelおよびexplanation を持た、かつ 「Enabled」であるかどうかを記録し**なければなりません**。
- thingは labelおよびkind を持た**なければなりません**。
- propertyは label を持た、かつ 「Symmetric」であるかどうかを記録し**なければなりません**。
- statementは「graph」をちょうど 1 件参照し**なければなりません**。
- statementは、subjectとして「thing」をちょうど 1 件参照し**なければなりません**。
- statementは「property」をちょうど 1 件参照し**なければなりません**。
- statementは 「Stated」であるかどうかを記録し**なければなりません**。
- firingは「rule」をちょうど 1 件参照し**なければなりません**。
- firingは、targetとして「statement」をちょうど 1 件参照し**なければなりません**。
- firingは、firstとして「statement」をちょうど 1 件参照し**なければなりません**。
- firingは、secondとして「statement」をちょうど 1 件参照し**なければなりません**。
- firingは kind を持た**なければなりません**。
- disjoint checkは「graph」をちょうど 1 件参照し**なければなりません**。
- disjoint checkは「rule」をちょうど 1 件参照し**なければなりません**。
- disjoint checkは、aとして「statement」をちょうど 1 件参照し**なければなりません**。
- disjoint checkは、bとして「statement」をちょうど 1 件参照し**なければなりません**。

## 4 定義ルール

_すべての文はビジネスドメインの真実を表し、手続きでも命令でもありません。
一方向の必要条件を同値と取り違えないよう、「〜の場合に限り」を用います。
**⚠︎ 機械的** マークは、決定的な文言が忠実ながらぎこちないルールを示します —
任意の書き直しの合図であり、欠陥ではありません。_

| ID | 宣言的ルール |
|----|------------------|
| **DR-1 Name** | graphの「name」は graph ID と同じです。 |
| **DR-2 Count of Stated Facts** | graphの「count of stated facts」は graphに関連する「statements」全体での「stated unit」の合計 です。 |
| **DR-3 Count of Conclusions** | graphの「count of conclusions」は graphに関連する「statements」全体での「conclusion unit」の合計 です。 |
| **DR-4 Count of Violations** | graphの「count of violations」は graphに関連する「disjoint checks」全体での「violation unit」の合計 です。 |
| **DR-5 Is Consistent** | count of violations が 0 である場合、graphは「Consistent」と見なされます。 |
| **DR-6 Name** | ruleの「name」は label と同じです。 |
| **DR-7 Count of Firings** | ruleの「count of firings」は ruleに関連する「firings」全体での「fired unit」の合計 です。 |
| **DR-8 Name** | thingの「name」は label と同じです。 |
| **DR-9 Count of Children** | thingの「count of children」は thingに関連する「statements」全体での「parent unit」の合計 です。 |
| **DR-10 Count of Grandchildren** | thingの「count of grandchildren」は thingに関連する「statements」全体での「grandparent unit」の合計 です。 |
| **DR-11 Name** | propertyの「name」は label と同じです。 |
| **DR-12 Subject Label** | statementの「subject label」— リンクされた「subject」から取得されます。 |
| **DR-13 Property Label** | statementの「property label」— リンクされた「property」から取得されます。 |
| **DR-14 Object Label** | statementの「object label」— リンクされた「object thing」から取得されます。 |
| **DR-15 Name** | statementの「name」は subject label、続いて 空白 1 つ、続いて property label、続いて 空白 1 つ、続いて object thing が空である場合は number value、それ以外の場合は object label として計算されます。 |
| **DR-16 Count of Inverse Firings** | statementの「count of inverse firings」は statementに関連する「firings」全体での「inverse fired unit」の合計 です。 |
| **DR-17 Count of Symmetric Firings** | statementの「count of symmetric firings」は statementに関連する「firings」全体での「symmetric fired unit」の合計 です。 |
| **DR-18 Holds Before Chain** | 次のいずれかが成り立つ: フラグ「stated」が設定されている、count of inverse firings が 0 より大きい、またはcount of symmetric firings が 0 より大きい場合、statementは「Before Chain」を満たすと見なされます。 |
| **DR-19 Count of Chain Firings** | statementの「count of chain firings」は statementに関連する「firings」全体での「chain fired unit」の合計 です。 |
| **DR-20 Holds Before Typing** | 次のいずれかが成り立つ: フラグ「holds before chain」が設定されているまたはcount of chain firings が 0 より大きい場合、statementは「Before Typing」を満たすと見なされます。 |
| **DR-21 Count of Type Firings** | statementの「count of type firings」は statementに関連する「firings」全体での「type fired unit」の合計 です。 |
| **DR-22 Holds** | 次のいずれかが成り立つ: フラグ「holds before typing」が設定されているまたはcount of type firings が 0 より大きい場合、statementは「」を満たすと見なされます。 |
| **DR-23 Is Conclusion** | 次のすべてが成り立つ: フラグ「holds」が設定されているおよびフラグ「stated」が設定されていない場合、statementは「Conclusion」と見なされます。 |
| **DR-24 Worked Out by** | Statementの「worked out by」は、次の優先順位で決まります:<br>1. フラグ「conclusion」が設定されていない場合は 空文字列;<br>2. count of inverse firings が 0 より大きい場合は 「inverse」;<br>3. count of symmetric firings が 0 より大きい場合は 「symmetric」;<br>4. count of chain firings が 0 より大きい場合は 「property chain」;<br>5. それ以外の場合は 「domain and range」。 |
| **DR-25 Stated Unit** | Statementの「stated unit」は、次の優先順位で決まります:<br>1. フラグ「stated」が設定されている場合は 1;<br>2. それ以外の場合は 0。 |
| **DR-26 Conclusion Unit** | Statementの「conclusion unit」は、次の優先順位で決まります:<br>1. フラグ「conclusion」が設定されている場合は 1;<br>2. それ以外の場合は 0。 |
| **DR-27 Parent Unit** | Statementの「parent unit」は、次の優先順位で決まります:<br>1. 次のすべてが成り立つ: フラグ「holds」が設定されているおよびproperty が 「isParentOf」 である場合は 1;<br>2. それ以外の場合は 0。 |
| **DR-28 Grandparent Unit** | Statementの「grandparent unit」は、次の優先順位で決まります:<br>1. 次のすべてが成り立つ: フラグ「holds」が設定されているおよびproperty が 「isGrandparentOf」 である場合は 1;<br>2. それ以外の場合は 0。 |
| **DR-29 Rule Enabled** | firingの「rule enabled」は、リンクされた「rule」が「Enabled」であるとき真になります。 |
| **DR-30 Rule Label** | firingの「rule label」— リンクされた「rule」から取得されます。 |
| **DR-31 Target Name** | firingの「target name」— リンクされた「target」から取得されます。 |
| **DR-32 First Name** | firingの「first name」— リンクされた「first」から取得されます。 |
| **DR-33 Second Name** | firingの「second name」— リンクされた「second」から取得されます。 |
| **DR-34 First is Stated** | firingの「first is stated」は、リンクされた「first」が「Stated」であるとき真になります。 |
| **DR-35 First Holds Before Chain** | firingの「first holds before chain」は、リンクされた「first」が「Holds Before Chain」であるとき真になります。 |
| **DR-36 Second Holds Before Chain** | firingの「second holds before chain」は、リンクされた「second」が「Holds Before Chain」であるとき真になります。 |
| **DR-37 First Holds Before Typing** | firingの「first holds before typing」は、リンクされた「first」が「Holds Before Typing」であるとき真になります。 |
| **DR-38 Inverse Fired Unit** | Firingの「inverse fired unit」は、次の優先順位で決まります:<br>1. 次のすべてが成り立つ: フラグ「rule enabled」が設定されている、kind が 「inverse」 である、およびフラグ「first is stated」が設定されている場合は 1;<br>2. それ以外の場合は 0。 |
| **DR-39 Symmetric Fired Unit** | Firingの「symmetric fired unit」は、次の優先順位で決まります:<br>1. 次のすべてが成り立つ: フラグ「rule enabled」が設定されている、kind が 「symmetric」 である、およびフラグ「first is stated」が設定されている場合は 1;<br>2. それ以外の場合は 0。 |
| **DR-40 Chain Fired Unit** | Firingの「chain fired unit」は、次の優先順位で決まります:<br>1. 次のすべてが成り立つ: フラグ「rule enabled」が設定されている、kind が 「chain」 である、フラグ「first holds before chain」が設定されている、およびフラグ「second holds before chain」が設定されている場合は 1;<br>2. それ以外の場合は 0。 |
| **DR-41 Type Fired Unit** | Firingの「type fired unit」は、次の優先順位で決まります:<br>1. 次のすべてが成り立つ: フラグ「rule enabled」が設定されている、次のいずれかが成り立つ: kind が 「domain」 であるまたはkind が 「range」 である、およびフラグ「first holds before typing」が設定されている場合は 1;<br>2. それ以外の場合は 0。 |
| **DR-42 Fired Unit** | firingの「fired unit」は inverse fired unit たす symmetric fired unit たす chain fired unit たす type fired unit として計算されます。 |
| **DR-43 Name** | Firingの「name」は、次の優先順位で決まります:<br>1. fired unit が 0 より大きい場合は target name、続いて 「: concluded by the 」、続いて rule label、続いて 「 rule from '」、続いて first name、続いて 「'」、続いて 「 and '」、続いて second name、続いて 「'」;<br>2. それ以外の場合は firing ID。 |
| **DR-44 Name** | disjoint checkの「name」は disjoint check ID と同じです。 |
| **DR-45 Rule Enabled** | disjoint checkの「rule enabled」は、リンクされた「rule」が「Enabled」であるとき真になります。 |
| **DR-46 A Holds** | disjoint checkの「a holds」は、リンクされた「a」が「Holds」であるとき真になります。 |
| **DR-47 B Holds** | disjoint checkの「b holds」は、リンクされた「b」が「Holds」であるとき真になります。 |
| **DR-48 Violation Unit** | Disjoint checkの「violation unit」は、次の優先順位で決まります:<br>1. 次のすべてが成り立つ: フラグ「rule enabled」が設定されている、フラグ「a holds」が設定されている、およびフラグ「b holds」が設定されている場合は 1;<br>2. それ以外の場合は 0。 |

## 5 スキーマへのトレーサビリティ

_式の列は ドキュメント 記法によるルールの定義です —
ルールブックが保持するのと同じロジックを、ビジネス読者向けに記したものです。_

| スキーマ要素 | 種別 | 式 |
|----------------|------|------------|
| **Graphs.Name** | 数式 | `GraphId` |
| **Graphs.CountOfStatedFacts** | 集計 | `Sum(Statements.StatedUnit via Graph)` |
| **Graphs.CountOfConclusions** | 集計 | `Sum(Statements.ConclusionUnit via Graph)` |
| **Graphs.CountOfViolations** | 集計 | `Sum(DisjointChecks.ViolationUnit via Graph)` |
| **Graphs.IsConsistent** | 数式 | `CountOfViolations = 0` |
| **Rules.Name** | 数式 | `Label` |
| **Rules.CountOfFirings** | 集計 | `Sum(Firings.FiredUnit via Rule)` |
| **Things.Name** | 数式 | `Label` |
| **Things.CountOfChildren** | 集計 | `Sum(Statements.ParentUnit via Subject)` |
| **Things.CountOfGrandchildren** | 集計 | `Sum(Statements.GrandparentUnit via Subject)` |
| **Properties.Name** | 数式 | `Label` |
| **Statements.SubjectLabel** | 参照 | `Lookup(Things.Label via Subject)` |
| **Statements.PropertyLabel** | 参照 | `Lookup(Properties.Label via Property)` |
| **Statements.ObjectLabel** | 参照 | `Lookup(Things.Label via ObjectThing)` |
| **Statements.Name** | 数式 | `SubjectLabel & " " & PropertyLabel & " " & If(ObjectThing = Blank(), NumberValue, ObjectLabel)` |
| **Statements.CountOfInverseFirings** | 集計 | `Sum(Firings.InverseFiredUnit via Target)` |
| **Statements.CountOfSymmetricFirings** | 集計 | `Sum(Firings.SymmetricFiredUnit via Target)` |
| **Statements.HoldsBeforeChain** | 数式 | `Or(IsStated, CountOfInverseFirings > 0, CountOfSymmetricFirings > 0)` |
| **Statements.CountOfChainFirings** | 集計 | `Sum(Firings.ChainFiredUnit via Target)` |
| **Statements.HoldsBeforeTyping** | 数式 | `Or(HoldsBeforeChain, CountOfChainFirings > 0)` |
| **Statements.CountOfTypeFirings** | 集計 | `Sum(Firings.TypeFiredUnit via Target)` |
| **Statements.Holds** | 数式 | `Or(HoldsBeforeTyping, CountOfTypeFirings > 0)` |
| **Statements.IsConclusion** | 数式 | `And(Holds, Not(IsStated))` |
| **Statements.WorkedOutBy** | 数式 | `If(Not(IsConclusion), "", If(CountOfInverseFirings > 0, "inverse", If(CountOfSymmetricFirings > 0, "symmetric", If(CountOfChainFirings > 0, "property chain", "domain and range"))))` |
| **Statements.StatedUnit** | 数式 | `If(IsStated, 1, 0)` |
| **Statements.ConclusionUnit** | 数式 | `If(IsConclusion, 1, 0)` |
| **Statements.ParentUnit** | 数式 | `If(And(Holds, Property = "isParentOf"), 1, 0)` |
| **Statements.GrandparentUnit** | 数式 | `If(And(Holds, Property = "isGrandparentOf"), 1, 0)` |
| **Firings.RuleEnabled** | 参照 | `Lookup(Rules.IsEnabled via Rule)` |
| **Firings.RuleLabel** | 参照 | `Lookup(Rules.Label via Rule)` |
| **Firings.TargetName** | 参照 | `Lookup(Statements.Name via Target)` |
| **Firings.FirstName** | 参照 | `Lookup(Statements.Name via First)` |
| **Firings.SecondName** | 参照 | `Lookup(Statements.Name via Second)` |
| **Firings.FirstIsStated** | 参照 | `Lookup(Statements.IsStated via First)` |
| **Firings.FirstHoldsBeforeChain** | 参照 | `Lookup(Statements.HoldsBeforeChain via First)` |
| **Firings.SecondHoldsBeforeChain** | 参照 | `Lookup(Statements.HoldsBeforeChain via Second)` |
| **Firings.FirstHoldsBeforeTyping** | 参照 | `Lookup(Statements.HoldsBeforeTyping via First)` |
| **Firings.InverseFiredUnit** | 数式 | `If(And(RuleEnabled, Kind = "inverse", FirstIsStated), 1, 0)` |
| **Firings.SymmetricFiredUnit** | 数式 | `If(And(RuleEnabled, Kind = "symmetric", FirstIsStated), 1, 0)` |
| **Firings.ChainFiredUnit** | 数式 | `If(And(RuleEnabled, Kind = "chain", FirstHoldsBeforeChain, SecondHoldsBeforeChain), 1, 0)` |
| **Firings.TypeFiredUnit** | 数式 | `If(And(RuleEnabled, Or(Kind = "domain", Kind = "range"), FirstHoldsBeforeTyping), 1, 0)` |
| **Firings.FiredUnit** | 数式 | `InverseFiredUnit + SymmetricFiredUnit + ChainFiredUnit + TypeFiredUnit` |
| **Firings.Name** | 数式 | `If(FiredUnit > 0, TargetName & ": concluded by the " & RuleLabel & " rule from '" & FirstName & "'" & If(Kind = "chain", " and '" & SecondName & "'", ""), FiringId)` |
| **DisjointChecks.Name** | 数式 | `DisjointCheckId` |
| **DisjointChecks.RuleEnabled** | 参照 | `Lookup(Rules.IsEnabled via Rule)` |
| **DisjointChecks.AHolds** | 参照 | `Lookup(Statements.Holds via A)` |
| **DisjointChecks.BHolds** | 参照 | `Lookup(Statements.Holds via B)` |
| **DisjointChecks.ViolationUnit** | 数式 | `If(And(RuleEnabled, AHolds, BHolds), 1, 0)` |
