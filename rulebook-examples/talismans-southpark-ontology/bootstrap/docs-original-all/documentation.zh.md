# 📘 South Park ontology explainer — 文档

_The barebones South Park ontology from Jessica Talisman's explainer: ten stated facts and five rules that can each be switched on. Conclusions are formulas._

> 由规则手册生成的声明式业务规则。以下每个语句表达业务领域中的一个真理——
> 它既不是过程,也不是命令。规则手册中的公式是唯一的真实来源;
> 本文档是它们的通俗读法。

## 1 业务词汇

| 术语 | 描述 | 补充说明 |
|------|-------------|-------------------|
| **Graph** | The whole example. | — |
| Name | 与 graph ID 相同。 | — |
| Count of Stated Facts | 与graph相关的所有“statements”的“stated unit”的总和。 | — |
| Count of Conclusions | 与graph相关的所有“statements”的“conclusion unit”的总和。 | _Facts that hold but were never stated._ |
| Count of Violations | 与graph相关的所有“disjoint checks”的“violation unit”的总和。 | — |
| Is Consistent | 当count of violations为0时为真。 | — |

<details open>
<summary>示例数据</summary>

| Name ƒ | Count of Stated Facts ƒ | Count of Conclusions ƒ | Count of Violations ƒ | Is Consistent ƒ |
|---|---|---|---|---|
| south-park | 10 | 8 | 0 | true |

_ƒ 表示计算列。_

</details>

| 术语 | 描述 | 补充说明 |
|------|-------------|-------------------|
| **Rule** | The five rules; IsEnabled is the checkbox. | — |
| Label | 一个已定义的属性。 | — |
| Explanation | 一个已定义的属性。 | — |
| Is Enabled | 当空字符串时为真。 | — |
| Name | 与 label 相同。 | — |
| Count of Firings | 与rule相关的所有“firings”的“fired unit”的总和。 | _How many times the rule produced a fact._ |

<details open>
<summary>示例数据</summary>

| Label | Explanation | Is Enabled | Name ƒ | Count of Firings ƒ |
|---|---|---|---|---|
| Inverse | A parent link also gives a child link between the same two people. | true | Inverse | 3 |
| Symmetric | A friend link or a spouse link is true for both people. | true | Symmetric | 2 |
| Property chain | A parent of a parent is a grandparent. | true | Property chain | 2 |

_ƒ 表示计算列。_

</details>

| 术语 | 描述 | 补充说明 |
|------|-------------|-------------------|
| **Thing** | A node in the picture: a person, a place, a class or a role value. | — |
| Label | 一个已定义的属性。 | — |
| Kind | 一个已定义的属性。 | — |
| Name | 与 label 相同。 | — |
| Count of Children | 与thing相关的所有“statements”的“parent unit”的总和。 | _Added: a count, which a reasoner alone does not give._ |
| Count of Grandchildren | 与thing相关的所有“statements”的“grandparent unit”的总和。 | _Added: counts concluded grandchildren._ |

<details open>
<summary>示例数据</summary>

| Label | Kind | Name ƒ | Count of Children ƒ | Count of Grandchildren ƒ |
|---|---|---|---|---|
| Marvin | individual | Marvin | 1 | 2 |
| Randy | individual | Randy | 2 | 0 |
| Sharon | individual | Sharon | 0 | 0 |

_ƒ 表示计算列。_

</details>

| 术语 | 描述 | 补充说明 |
|------|-------------|-------------------|
| **Property** | A kind of link, with the characteristics the rules use. | — |
| Label | 一个已定义的属性。 | — |
| Inverse of | 一个已定义的属性。 | — |
| Is Symmetric | 当空字符串时为真。 | — |
| Chain First | 一个已定义的属性。 | — |
| Chain Second | 一个已定义的属性。 | — |
| Domain Class | 一个已定义的属性。 | — |
| Range Class | 一个已定义的属性。 | — |
| Disjoint With | 一个已定义的属性。 | — |
| Name | 与 label 相同。 | — |

<details open>
<summary>示例数据</summary>

| Label | Is Symmetric | Name ƒ |
|---|---|---|
| is parent of | false | is parent of |
| is child of | false | is child of |
| is spouse of | true | is spouse of |

_ƒ 表示计算列。_

</details>

| 术语 | 描述 | 补充说明 |
|------|-------------|-------------------|
| **Statement** | A fact that is stated, or could be concluded: Subject, Property, Object. | — |
| Graph | 一个已定义的属性。 | — |
| Subject | 一个已定义的属性。 | — |
| Property | 一个已定义的属性。 | — |
| Object Thing | 一个已定义的属性。 | — |
| Number Value | 一个已定义的属性。 | — |
| Is Stated | 当空字符串时为真。 | _Drawn as a solid line in the explainer._ |
| Subject Label | 取自所关联的“subject”。 | — |
| Property Label | 取自所关联的“property”。 | — |
| Object Label | 取自所关联的“object thing”。 | — |
| Name | 按 subject label,接着是一个空格,接着是property label,接着是一个空格,接着是当object thing为空时为 number value,其余情况为 object label 计算。 | — |
| Count of Inverse Firings | 与statement相关的所有“firings”的“inverse fired unit”的总和。 | — |
| Count of Symmetric Firings | 与statement相关的所有“firings”的“symmetric fired unit”的总和。 | — |
| Holds Before Chain | 当以下至少一项成立:标志“stated”已设置、count of inverse firings大于0,或count of symmetric firings大于0时为真。 | — |
| Count of Chain Firings | 与statement相关的所有“firings”的“chain fired unit”的总和。 | — |
| Holds Before Typing | 当以下至少一项成立:标志“holds before chain”已设置或count of chain firings大于0时为真。 | — |
| Count of Type Firings | 与statement相关的所有“firings”的“type fired unit”的总和。 | — |
| Holds | 当以下至少一项成立:标志“holds before typing”已设置或count of type firings大于0时为真。 | _True when stated or concluded by an enabled rule._ |
| Is Conclusion | 当以下全部成立:标志“holds”已设置且标志“stated”未设置时为真。 | _Drawn as a dashed line in the explainer._ |
| Worked Out by | 按优先级确定:当标志“conclusion”未设置时取 空字符串; 当count of inverse firings大于0时取 “inverse”; 当count of symmetric firings大于0时取 “symmetric”; 当count of chain firings大于0时取 “property chain”;其余情况取 “domain and range”。 | — |
| Stated Unit | 按优先级确定:当标志“stated”已设置时取 1;其余情况取 0。 | — |
| Conclusion Unit | 按优先级确定:当标志“conclusion”已设置时取 1;其余情况取 0。 | — |
| Parent Unit | 按优先级确定:当以下全部成立:标志“holds”已设置且property为“isParentOf”时取 1;其余情况取 0。 | — |
| Grandparent Unit | 按优先级确定:当以下全部成立:标志“holds”已设置且property为“isGrandparentOf”时取 1;其余情况取 0。 | — |

<details open>
<summary>示例数据</summary>

| Number Value | Is Stated | Subject Label ƒ | Property Label ƒ | Object Label ƒ | Name ƒ | Count of Inverse Firings ƒ | Count of Symmetric Firings ƒ | Holds Before Chain ƒ | Count of Chain Firings ƒ | Holds Before Typing ƒ | Count of Type Firings ƒ | Holds ƒ | Is Conclusion ƒ | Worked Out by ƒ | Stated Unit ƒ | Conclusion Unit ƒ | Parent Unit ƒ | Grandparent Unit ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| — | true | Marvin | is parent of | Randy | Marvin is parent of Randy | 0 | 0 | true | 0 | true | 0 | true | false | — | 1 | 0 | 1 | 0 |
| — | false | Marvin | is parent of | Sharon | Marvin is parent of Sharon | 0 | 0 | false | 0 | false | 0 | false | false | — | 0 | 0 | 0 | 0 |
| — | false | Marvin | is parent of | Shelly | Marvin is parent of Shelly | 0 | 0 | false | 0 | false | 0 | false | false | — | 0 | 0 | 0 | 0 |

_ƒ 表示计算列。_

</details>

| 术语 | 描述 | 补充说明 |
|------|-------------|-------------------|
| **Firing** | One way one rule could produce one fact from one or two others. | — |
| Rule | 一个已定义的属性。 | — |
| Kind | 一个已定义的属性。 | — |
| Target | 一个已定义的属性。 | — |
| First | 一个已定义的属性。 | — |
| Second | 一个已定义的属性。 | _Same as First when the rule needs one premise._ |
| Rule Enabled | 当所关联的“rule”是“Enabled”时为真。 | — |
| Rule Label | 取自所关联的“rule”。 | — |
| Target Name | 取自所关联的“target”。 | — |
| First Name | 取自所关联的“first”。 | — |
| Second Name | 取自所关联的“second”。 | — |
| First is Stated | 当所关联的“first”是“Stated”时为真。 | — |
| First Holds Before Chain | 当所关联的“first”是“Holds Before Chain”时为真。 | — |
| Second Holds Before Chain | 当所关联的“second”是“Holds Before Chain”时为真。 | — |
| First Holds Before Typing | 当所关联的“first”是“Holds Before Typing”时为真。 | — |
| Inverse Fired Unit | 按优先级确定:当以下全部成立:标志“rule enabled”已设置、kind为“inverse”,且标志“first is stated”已设置时取 1;其余情况取 0。 | — |
| Symmetric Fired Unit | 按优先级确定:当以下全部成立:标志“rule enabled”已设置、kind为“symmetric”,且标志“first is stated”已设置时取 1;其余情况取 0。 | — |
| Chain Fired Unit | 按优先级确定:当以下全部成立:标志“rule enabled”已设置、kind为“chain”、标志“first holds before chain”已设置,且标志“second holds before chain”已设置时取 1;其余情况取 0。 | — |
| Type Fired Unit | 按优先级确定:当以下全部成立:标志“rule enabled”已设置、以下至少一项成立:kind为“domain”或kind为“range”,且标志“first holds before typing”已设置时取 1;其余情况取 0。 | — |
| Fired Unit | 按 inverse fired unit加symmetric fired unit加chain fired unit加type fired unit 计算。 | — |
| Name | 按优先级确定:当fired unit大于0时取 target name,接着是“: worked out by the ”,接着是rule label,接着是“ rule from '”,接着是first name,接着是“'”,接着是“ and '”,接着是second name,接着是“'”;其余情况取 firing ID。 | _The explainer's conclusion card._ |

<details open>
<summary>示例数据</summary>

| Kind | Rule Enabled ƒ | Rule Label ƒ | Target Name ƒ | First Name ƒ | Second Name ƒ | First is Stated ƒ | First Holds Before Chain ƒ | Second Holds Before Chain ƒ | First Holds Before Typing ƒ | Inverse Fired Unit ƒ | Symmetric Fired Unit ƒ | Chain Fired Unit ƒ | Type Fired Unit ƒ | Fired Unit ƒ | Name ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| inverse | true | Inverse | Randy is child of Marvin | Marvin is parent of Randy | Marvin is parent of Randy | true | true | true | true | 1 | 0 | 0 | 0 | 1 | Randy is child of Marvin: worked out by the Inverse rule from 'Marvin is parent of Randy' |
| inverse | true | Inverse | Sharon is child of Marvin | Marvin is parent of Sharon | Marvin is parent of Sharon | false | false | false | false | 0 | 0 | 0 | 0 | 0 | inverse:Marvin\|isParentOf\|Sharon>Sharon\|isChildOf\|Marvin |
| inverse | true | Inverse | Shelly is child of Marvin | Marvin is parent of Shelly | Marvin is parent of Shelly | false | false | false | false | 0 | 0 | 0 | 0 | 0 | inverse:Marvin\|isParentOf\|Shelly>Shelly\|isChildOf\|Marvin |

_ƒ 表示计算列。_

</details>

| 术语 | 描述 | 补充说明 |
|------|-------------|-------------------|
| **Disjoint Check** | Two statements that must not both hold. | — |
| Graph | 一个已定义的属性。 | — |
| Rule | 一个已定义的属性。 | — |
| A | 一个已定义的属性。 | — |
| B | 一个已定义的属性。 | — |
| Name | 与 disjoint check ID 相同。 | — |
| Rule Enabled | 当所关联的“rule”是“Enabled”时为真。 | — |
| A Holds | 当所关联的“a”是“Holds”时为真。 | — |
| B Holds | 当所关联的“b”是“Holds”时为真。 | — |
| Violation Unit | 按优先级确定:当以下全部成立:标志“rule enabled”已设置、标志“a holds”已设置,且标志“b holds”已设置时取 1;其余情况取 0。 | — |

<details open>
<summary>示例数据</summary>

| Name ƒ | Rule Enabled ƒ | A Holds ƒ | B Holds ƒ | Violation Unit ƒ |
|---|---|---|---|---|
| Marvin\|isParentOf\|Randy~isSpouseOf | true | true | false | 0 |
| Marvin\|isParentOf\|Sharon~isSpouseOf | true | false | false | 0 |
| Marvin\|isParentOf\|Shelly~isSpouseOf | true | false | false | 0 |

_ƒ 表示计算列。_

</details>

## 2 事实类型

- **property**可引用一个**property**
- **property**可引用一个**thing**
- **statement**恰好引用一个**graph**
- **statement**恰好引用一个**thing**
- **statement**恰好引用一个**property**
- **firing**恰好引用一个**rule**
- **firing**恰好引用一个**statement**
- **disjoint check**恰好引用一个**graph**
- **disjoint check**恰好引用一个**rule**
- **disjoint check**恰好引用一个**statement**

## 3 操作性规则

_操作性规则陈述业务**强制**、**禁止**或**建议**的内容。结构性规则来自必填字段和
外键;语义规则来自 Constraints 表,每条都依据规则手册已经计算的布尔字段
(在下方定义性规则中以 DR-N 交叉引用)。_

### 结构性约束(来自模式)

- rule**必须**具有label且explanation,并记录其是“Enabled”与否。
- thing**必须**具有label且kind。
- property**必须**具有label,并记录其是“Symmetric”与否。
- statement**必须**恰好引用一个“graph”。
- statement**必须**恰好引用一个“thing”作为其subject。
- statement**必须**恰好引用一个“property”。
- statement**必须**记录其是“Stated”与否。
- firing**必须**恰好引用一个“rule”。
- firing**必须**恰好引用一个“statement”作为其target。
- firing**必须**恰好引用一个“statement”作为其first。
- firing**必须**恰好引用一个“statement”作为其second。
- firing**必须**具有kind。
- disjoint check**必须**恰好引用一个“graph”。
- disjoint check**必须**恰好引用一个“rule”。
- disjoint check**必须**恰好引用一个“statement”作为其a。
- disjoint check**必须**恰好引用一个“statement”作为其b。

## 4 定义性规则

_所有语句都表达业务领域中的真理;它们既不是过程,也不是命令。避免使用
"当且仅当",改用"仅当",以免把单向必要条件误当作等价关系。
**⚠︎ 机械** 标记表示某条规则的确定性措辞忠实但生硬——
这是可选改写的信号,不是缺陷。_

| ID | 声明式规则 |
|----|------------------|
| **DR-1 Name** | graph的“name”与 graph ID 相同。 |
| **DR-2 Count of Stated Facts** | graph的“count of stated facts”为 与graph相关的所有“statements”的“stated unit”的总和。 |
| **DR-3 Count of Conclusions** | graph的“count of conclusions”为 与graph相关的所有“statements”的“conclusion unit”的总和。 |
| **DR-4 Count of Violations** | graph的“count of violations”为 与graph相关的所有“disjoint checks”的“violation unit”的总和。 |
| **DR-5 Is Consistent** | 当count of violations为0时,graph被视为“Consistent”。 |
| **DR-6 Name** | rule的“name”与 label 相同。 |
| **DR-7 Count of Firings** | rule的“count of firings”为 与rule相关的所有“firings”的“fired unit”的总和。 |
| **DR-8 Name** | thing的“name”与 label 相同。 |
| **DR-9 Count of Children** | thing的“count of children”为 与thing相关的所有“statements”的“parent unit”的总和。 |
| **DR-10 Count of Grandchildren** | thing的“count of grandchildren”为 与thing相关的所有“statements”的“grandparent unit”的总和。 |
| **DR-11 Name** | property的“name”与 label 相同。 |
| **DR-12 Subject Label** | statement的“subject label”——取自所关联的“subject”。 |
| **DR-13 Property Label** | statement的“property label”——取自所关联的“property”。 |
| **DR-14 Object Label** | statement的“object label”——取自所关联的“object thing”。 |
| **DR-15 Name** | statement的“name”按 subject label,接着是一个空格,接着是property label,接着是一个空格,接着是当object thing为空时为 number value,其余情况为 object label 计算。 |
| **DR-16 Count of Inverse Firings** | statement的“count of inverse firings”为 与statement相关的所有“firings”的“inverse fired unit”的总和。 |
| **DR-17 Count of Symmetric Firings** | statement的“count of symmetric firings”为 与statement相关的所有“firings”的“symmetric fired unit”的总和。 |
| **DR-18 Holds Before Chain** | 当以下至少一项成立:标志“stated”已设置、count of inverse firings大于0,或count of symmetric firings大于0时,statement被视为满足“Before Chain”。 |
| **DR-19 Count of Chain Firings** | statement的“count of chain firings”为 与statement相关的所有“firings”的“chain fired unit”的总和。 |
| **DR-20 Holds Before Typing** | 当以下至少一项成立:标志“holds before chain”已设置或count of chain firings大于0时,statement被视为满足“Before Typing”。 |
| **DR-21 Count of Type Firings** | statement的“count of type firings”为 与statement相关的所有“firings”的“type fired unit”的总和。 |
| **DR-22 Holds** | 当以下至少一项成立:标志“holds before typing”已设置或count of type firings大于0时,statement被视为满足“”。 |
| **DR-23 Is Conclusion** | 当以下全部成立:标志“holds”已设置且标志“stated”未设置时,statement被视为“Conclusion”。 |
| **DR-24 Worked Out by** | Statement的“worked out by”按以下优先级确定:<br>1. 当标志“conclusion”未设置时,取 空字符串;<br>2. 当count of inverse firings大于0时,取 “inverse”;<br>3. 当count of symmetric firings大于0时,取 “symmetric”;<br>4. 当count of chain firings大于0时,取 “property chain”;<br>5. 其余情况取 “domain and range”。 |
| **DR-25 Stated Unit** | Statement的“stated unit”按以下优先级确定:<br>1. 当标志“stated”已设置时,取 1;<br>2. 其余情况取 0。 |
| **DR-26 Conclusion Unit** | Statement的“conclusion unit”按以下优先级确定:<br>1. 当标志“conclusion”已设置时,取 1;<br>2. 其余情况取 0。 |
| **DR-27 Parent Unit** | Statement的“parent unit”按以下优先级确定:<br>1. 当以下全部成立:标志“holds”已设置且property为“isParentOf”时,取 1;<br>2. 其余情况取 0。 |
| **DR-28 Grandparent Unit** | Statement的“grandparent unit”按以下优先级确定:<br>1. 当以下全部成立:标志“holds”已设置且property为“isGrandparentOf”时,取 1;<br>2. 其余情况取 0。 |
| **DR-29 Rule Enabled** | firing的“rule enabled”在所关联的“rule”是“Enabled”时为真。 |
| **DR-30 Rule Label** | firing的“rule label”——取自所关联的“rule”。 |
| **DR-31 Target Name** | firing的“target name”——取自所关联的“target”。 |
| **DR-32 First Name** | firing的“first name”——取自所关联的“first”。 |
| **DR-33 Second Name** | firing的“second name”——取自所关联的“second”。 |
| **DR-34 First is Stated** | firing的“first is stated”在所关联的“first”是“Stated”时为真。 |
| **DR-35 First Holds Before Chain** | firing的“first holds before chain”在所关联的“first”是“Holds Before Chain”时为真。 |
| **DR-36 Second Holds Before Chain** | firing的“second holds before chain”在所关联的“second”是“Holds Before Chain”时为真。 |
| **DR-37 First Holds Before Typing** | firing的“first holds before typing”在所关联的“first”是“Holds Before Typing”时为真。 |
| **DR-38 Inverse Fired Unit** | Firing的“inverse fired unit”按以下优先级确定:<br>1. 当以下全部成立:标志“rule enabled”已设置、kind为“inverse”,且标志“first is stated”已设置时,取 1;<br>2. 其余情况取 0。 |
| **DR-39 Symmetric Fired Unit** | Firing的“symmetric fired unit”按以下优先级确定:<br>1. 当以下全部成立:标志“rule enabled”已设置、kind为“symmetric”,且标志“first is stated”已设置时,取 1;<br>2. 其余情况取 0。 |
| **DR-40 Chain Fired Unit** | Firing的“chain fired unit”按以下优先级确定:<br>1. 当以下全部成立:标志“rule enabled”已设置、kind为“chain”、标志“first holds before chain”已设置,且标志“second holds before chain”已设置时,取 1;<br>2. 其余情况取 0。 |
| **DR-41 Type Fired Unit** | Firing的“type fired unit”按以下优先级确定:<br>1. 当以下全部成立:标志“rule enabled”已设置、以下至少一项成立:kind为“domain”或kind为“range”,且标志“first holds before typing”已设置时,取 1;<br>2. 其余情况取 0。 |
| **DR-42 Fired Unit** | firing的“fired unit”按 inverse fired unit加symmetric fired unit加chain fired unit加type fired unit 计算。 |
| **DR-43 Name** | Firing的“name”按以下优先级确定:<br>1. 当fired unit大于0时,取 target name,接着是“: worked out by the ”,接着是rule label,接着是“ rule from '”,接着是first name,接着是“'”,接着是“ and '”,接着是second name,接着是“'”;<br>2. 其余情况取 firing ID。 |
| **DR-44 Name** | disjoint check的“name”与 disjoint check ID 相同。 |
| **DR-45 Rule Enabled** | disjoint check的“rule enabled”在所关联的“rule”是“Enabled”时为真。 |
| **DR-46 A Holds** | disjoint check的“a holds”在所关联的“a”是“Holds”时为真。 |
| **DR-47 B Holds** | disjoint check的“b holds”在所关联的“b”是“Holds”时为真。 |
| **DR-48 Violation Unit** | Disjoint check的“violation unit”按以下优先级确定:<br>1. 当以下全部成立:标志“rule enabled”已设置、标志“a holds”已设置,且标志“b holds”已设置时,取 1;<br>2. 其余情况取 0。 |

## 5 到模式的可追溯性

_表达式列是规则在 文档 记法中的定义——
与规则手册存储的逻辑相同,只是为业务读者书写。_

| 模式元素 | 类别 | 表达式 |
|----------------|------|------------|
| **Graphs.Name** | 公式 | `GraphId` |
| **Graphs.CountOfStatedFacts** | 汇总 | `Sum(Statements.StatedUnit via Graph)` |
| **Graphs.CountOfConclusions** | 汇总 | `Sum(Statements.ConclusionUnit via Graph)` |
| **Graphs.CountOfViolations** | 汇总 | `Sum(DisjointChecks.ViolationUnit via Graph)` |
| **Graphs.IsConsistent** | 公式 | `CountOfViolations = 0` |
| **Rules.Name** | 公式 | `Label` |
| **Rules.CountOfFirings** | 汇总 | `Sum(Firings.FiredUnit via Rule)` |
| **Things.Name** | 公式 | `Label` |
| **Things.CountOfChildren** | 汇总 | `Sum(Statements.ParentUnit via Subject)` |
| **Things.CountOfGrandchildren** | 汇总 | `Sum(Statements.GrandparentUnit via Subject)` |
| **Properties.Name** | 公式 | `Label` |
| **Statements.SubjectLabel** | 查找 | `Lookup(Things.Label via Subject)` |
| **Statements.PropertyLabel** | 查找 | `Lookup(Properties.Label via Property)` |
| **Statements.ObjectLabel** | 查找 | `Lookup(Things.Label via ObjectThing)` |
| **Statements.Name** | 公式 | `SubjectLabel & " " & PropertyLabel & " " & If(ObjectThing = Blank(), NumberValue, ObjectLabel)` |
| **Statements.CountOfInverseFirings** | 汇总 | `Sum(Firings.InverseFiredUnit via Target)` |
| **Statements.CountOfSymmetricFirings** | 汇总 | `Sum(Firings.SymmetricFiredUnit via Target)` |
| **Statements.HoldsBeforeChain** | 公式 | `Or(IsStated, CountOfInverseFirings > 0, CountOfSymmetricFirings > 0)` |
| **Statements.CountOfChainFirings** | 汇总 | `Sum(Firings.ChainFiredUnit via Target)` |
| **Statements.HoldsBeforeTyping** | 公式 | `Or(HoldsBeforeChain, CountOfChainFirings > 0)` |
| **Statements.CountOfTypeFirings** | 汇总 | `Sum(Firings.TypeFiredUnit via Target)` |
| **Statements.Holds** | 公式 | `Or(HoldsBeforeTyping, CountOfTypeFirings > 0)` |
| **Statements.IsConclusion** | 公式 | `And(Holds, Not(IsStated))` |
| **Statements.WorkedOutBy** | 公式 | `If(Not(IsConclusion), "", If(CountOfInverseFirings > 0, "inverse", If(CountOfSymmetricFirings > 0, "symmetric", If(CountOfChainFirings > 0, "property chain", "domain and range"))))` |
| **Statements.StatedUnit** | 公式 | `If(IsStated, 1, 0)` |
| **Statements.ConclusionUnit** | 公式 | `If(IsConclusion, 1, 0)` |
| **Statements.ParentUnit** | 公式 | `If(And(Holds, Property = "isParentOf"), 1, 0)` |
| **Statements.GrandparentUnit** | 公式 | `If(And(Holds, Property = "isGrandparentOf"), 1, 0)` |
| **Firings.RuleEnabled** | 查找 | `Lookup(Rules.IsEnabled via Rule)` |
| **Firings.RuleLabel** | 查找 | `Lookup(Rules.Label via Rule)` |
| **Firings.TargetName** | 查找 | `Lookup(Statements.Name via Target)` |
| **Firings.FirstName** | 查找 | `Lookup(Statements.Name via First)` |
| **Firings.SecondName** | 查找 | `Lookup(Statements.Name via Second)` |
| **Firings.FirstIsStated** | 查找 | `Lookup(Statements.IsStated via First)` |
| **Firings.FirstHoldsBeforeChain** | 查找 | `Lookup(Statements.HoldsBeforeChain via First)` |
| **Firings.SecondHoldsBeforeChain** | 查找 | `Lookup(Statements.HoldsBeforeChain via Second)` |
| **Firings.FirstHoldsBeforeTyping** | 查找 | `Lookup(Statements.HoldsBeforeTyping via First)` |
| **Firings.InverseFiredUnit** | 公式 | `If(And(RuleEnabled, Kind = "inverse", FirstIsStated), 1, 0)` |
| **Firings.SymmetricFiredUnit** | 公式 | `If(And(RuleEnabled, Kind = "symmetric", FirstIsStated), 1, 0)` |
| **Firings.ChainFiredUnit** | 公式 | `If(And(RuleEnabled, Kind = "chain", FirstHoldsBeforeChain, SecondHoldsBeforeChain), 1, 0)` |
| **Firings.TypeFiredUnit** | 公式 | `If(And(RuleEnabled, Or(Kind = "domain", Kind = "range"), FirstHoldsBeforeTyping), 1, 0)` |
| **Firings.FiredUnit** | 公式 | `InverseFiredUnit + SymmetricFiredUnit + ChainFiredUnit + TypeFiredUnit` |
| **Firings.Name** | 公式 | `If(FiredUnit > 0, TargetName & ": worked out by the " & RuleLabel & " rule from '" & FirstName & "'" & If(Kind = "chain", " and '" & SecondName & "'", ""), FiringId)` |
| **DisjointChecks.Name** | 公式 | `DisjointCheckId` |
| **DisjointChecks.RuleEnabled** | 查找 | `Lookup(Rules.IsEnabled via Rule)` |
| **DisjointChecks.AHolds** | 查找 | `Lookup(Statements.Holds via A)` |
| **DisjointChecks.BHolds** | 查找 | `Lookup(Statements.Holds via B)` |
| **DisjointChecks.ViolationUnit** | 公式 | `If(And(RuleEnabled, AHolds, BHolds), 1, 0)` |
