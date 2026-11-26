# 📘 Talisman's South Park ontology explainer — 文档

_Jessica Talisman's South Park Ontology Explainer, as a rulebook: the page's ten facts, five rule switches, property wiring, provenance wording and current-value policy are rows, and every conclusion is a formula. The rules apply in three fixed strata (inverse and symmetric, then the property chain, then domain and range); property wiring that feeds an earlier stratum would need repeated rounds. Every fact that could hold over the cast is a candidate row; app/lib/candidates.mjs writes the candidates when a thing, value or property is added._

> 由规则手册生成的声明式业务规则。以下每个语句表达业务领域中的一个真理——
> 它既不是过程,也不是命令。规则手册中的公式是唯一的真实来源;
> 本文档是它们的通俗读法。

## 1 业务词汇

| 术语 | 描述 | 补充说明 |
|------|-------------|-------------------|
| **Graph** | The explainer page as one graph: its wording variables and its totals. | — |
| Label | 一个已定义的属性。 | _Display name of the graph._ |
| Literal Quote | 一个已定义的属性。 | _The quote mark written around a literal value in a fact's wording._ |
| Literal Type Marker | 一个已定义的属性。 | _The marker written between a literal value and its datatype, as in Turtle._ |
| Built in Source Text | 一个已定义的属性。 | _Source line shown for a fact the page states as a teaching example._ |
| User Source Text | 一个已定义的属性。 | _Source line shown for a fact the reader stated._ |
| No Conclusions Text | 一个已定义的属性。 | _Message shown when the selected facts and rules add no conclusion._ |
| Name | 与 label 相同。 | _Order 1. Display name of the graph._ |
| Count of Stated Facts | 与graph相关的所有“statements”的“in effect unit”的总和。 | _Order 5. Facts that are switched on and in effect._ |
| Count of Conclusions | 与graph相关的所有“statements”的“conclusion unit”的总和。 | _Order 18. Facts that hold only because an enabled rule concluded them._ |
| Count of Violations | 与graph相关的所有“disjoint checks”的“violation unit”的总和。 | _Order 18. Disjointness checks that found both links between the same pair._ |
| Is Consistent | 当count of violations为0时为真。 | _Order 19. True when no enabled disjointness rule is violated._ |
| Conclusions Message | 按优先级确定:当count of conclusions为0时取 no conclusions text;其余情况取 空字符串。 | _Order 19. The empty-state message for the conclusions panel, blank when there are conclusions._ |
| Count of Facts to Add | 与graph相关的所有“statements”的“listed off unit”的总和。 | _Order 5. Listed facts that are not in effect; the Add next fact button is enabled while this is above zero._ |
| Has Facts to Add | 当count of facts to add大于0时为真。 | _Order 6. True while some listed fact can still be added._ |
| Next Fact Sort Order | Graph的“statements”中是“Listed Off Unit”的“sort order”的最小值。 | _Order 5. List position of the first listed fact that is not in effect._ |
| Max Activation Seq | 与graph相关的所有“statements”的“activation seq”的最大值。 | _Order 1. Highest activation number used so far._ |
| Next Activation Seq | 按 max activation seq加1 计算。 | _Order 2. The activation number the next switched-on fact takes._ |
| Max Listed Sort Order | Graph的“statements”中是“Listed Unit”的“sort order”的最大值。 | _Order 2. Highest list position among listed facts._ |
| Next Listed Sort Order | 按 max listed sort order加1 计算。 | _Order 3. The list position a newly added fact takes._ |
| Max Sort Order | 与graph相关的所有“statements”的“sort order”的最大值。 | _Order 1. Highest sort order of any fact, listed or candidate._ |
| Next Candidate Sort Order | 按 max sort order加1 计算。 | _Order 2. The sort order a new candidate fact, or a fact removed from the list, takes._ |

<details open>
<summary>示例数据</summary>

| Label | Literal Quote | Literal Type Marker | Built in Source Text | User Source Text | No Conclusions Text | Name ƒ | Count of Stated Facts ƒ | Count of Conclusions ƒ | Count of Violations ƒ | Is Consistent ƒ | Conclusions Message ƒ | Count of Facts to Add ƒ | Has Facts to Add ƒ | Next Fact Sort Order ƒ | Max Activation Seq ƒ | Next Activation Seq ƒ | Max Listed Sort Order ƒ | Next Listed Sort Order ƒ | Max Sort Order ƒ | Next Candidate Sort Order ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| South Park | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | These facts and selected rules add no further conclusions. Try another fact or rule. | South Park | 10 | 0 | 0 | true | These facts and selected rules add no further conclusions. Try another fact or rule. | 0 | false | — | 10 | 11 | 10 | 11 | 100269 | 100270 |

_ƒ 表示计算列。_

</details>

| 术语 | 描述 | 补充说明 |
|------|-------------|-------------------|
| **Rule** | The five rule switches on the page. | — |
| Label | 一个已定义的属性。 | _The rule's name on the page._ |
| Explanation | 一个已定义的属性。 | _The rule's one-line explanation on the page._ |
| Is Enabled | 当空字符串时为真。 | _The rule's checkbox._ |
| Sort Order | 一个已定义的属性。 | _Position in the rule list._ |
| Name | 与 label 相同。 | _Order 1. The rule's name._ |
| Count of Firings | 与rule相关的所有“firings”的“fired unit”的总和。 | _Order 15. Rule applications that fired, counting every derivation._ |

<details open>
<summary>示例数据</summary>

| Label | Explanation | Is Enabled | Sort Order | Name ƒ | Count of Firings ƒ |
|---|---|---|---|---|---|
| Inverse | A parent link also gives a child link between the same two people. | false | 1 | Inverse | 0 |
| Symmetric | A friend link or a spouse link is true for both people. | false | 2 | Symmetric | 0 |
| Property chain | A parent of a parent is a grandparent. | false | 3 | Property chain | 0 |

_ƒ 表示计算列。_

</details>

| 术语 | 描述 | 补充说明 |
|------|-------------|-------------------|
| **Firing Kind** | The ways a rule can apply. Domain and range belong to one rule but are worded apart; 'disjoint' names the rule the disjointness checks answer to. | — |
| Rule | 一个已定义的属性。 | _The rule switch this kind belongs to._ |
| Wording | 一个已定义的属性。 | _How a conclusion card names the rule, as in 'Worked out by the inverse rule'._ |
| Sort Order | 一个已定义的属性。 | _Order in which the page's reasoner tries the kinds for one fact._ |
| Name | 与 wording 相同。 | _Order 1. The kind's wording._ |
| Rule Enabled | 当所关联的“rule”是“Enabled”时为真。 | _Order 1. Whether the owning rule is switched on._ |

<details open>
<summary>示例数据</summary>

| Wording | Sort Order | Name ƒ | Rule Enabled ƒ |
|---|---|---|---|
| inverse rule | 1 | inverse rule | false |
| symmetric rule | 2 | symmetric rule | false |
| property chain rule | 3 | property chain rule | false |

_ƒ 表示计算列。_

</details>

| 术语 | 描述 | 补充说明 |
|------|-------------|-------------------|
| **Thing** | Every individual and class the page can draw. | — |
| Label | 一个已定义的属性。 | _The name the page shows._ |
| Kind | 一个已定义的属性。 | _'individual' or 'class'._ |
| Sort Order | 一个已定义的属性。 | _Position in the cast._ |
| Portrait | 一个已定义的属性。 | _The portrait drawn for this thing; set when its name matches a portrait alias._ |
| Portrait Name | 所关联的“portrait”的“display name”。 | _Order 1. The name on the portrait's Fandom page._ |
| Portrait Page | 取自所关联的“portrait”。 | _Order 1. The Fandom page credited for the portrait._ |
| Portrait Image | 取自所关联的“portrait”。 | _Order 1. The original Fandom image._ |
| Portrait Crop | 取自所关联的“portrait”。 | _Order 1. The portrait's crop as an SVG viewBox._ |
| Portrait Width | 取自所关联的“portrait”。 | _Order 1. The original image's width in pixels._ |
| Portrait Height | 取自所关联的“portrait”。 | _Order 1. The original image's height in pixels._ |
| Name | 与 label 相同。 | _Order 1. The thing's name._ |
| Node Key | 与 thing ID 相同。 | _Order 1. The key the graph draws this thing under._ |
| Count of Children | 与thing相关的所有“statements”的“parent unit”的总和。 | _Order 17. People this thing is a parent of, stated or concluded._ |
| Count of Grandchildren | 与thing相关的所有“statements”的“grandparent unit”的总和。 | _Order 17. People this thing is a grandparent of._ |
| Count As Place | 与thing相关的所有“statements”的“place unit”的总和。 | _Order 17. Holding attends facts whose object is this thing._ |
| Count As Subject | 与thing相关的所有“statements”的“holds unit”的总和。 | _Order 17. Holding facts about this thing._ |
| Count As Object | 与thing相关的所有“statements”的“holds unit”的总和。 | _Order 17. Holding facts that point at this thing._ |
| Is in Graph | 当以下至少一项成立:count as subject大于0或count as object大于0时为真。 | _Order 18. True when the graph draws this thing._ |
| Node Kind | 按优先级确定:当kind为空时取 空字符串; 当kind为“class”时取 “class”; 当count as place大于0时取 “place”;其余情况取 “person”。 | _Order 18. How the graph draws the thing: a class, a place someone attends, or a person._ |

<details open>
<summary>示例数据</summary>

| Label | Kind | Sort Order | Portrait Name ƒ | Portrait Page ƒ | Portrait Image ƒ | Portrait Crop ƒ | Portrait Width ƒ | Portrait Height ƒ | Name ƒ | Node Key ƒ | Count of Children ƒ | Count of Grandchildren ƒ | Count As Place ƒ | Count As Subject ƒ | Count As Object ƒ | Is in Graph ƒ | Node Kind ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| Marvin | individual | 1 | Marvin Marsh | https://southpark.fandom.com/wiki/Marvin_Marsh | https://static.wikia.nocookie.net/southpark/images/d/db/Marsh-family-grandpa-marsh-w-purple-blanket.png/revision/latest | 48 -5 280 280 | 375 | 536 | Marvin | Marvin | 1 | 0 | 0 | 1 | 0 | true | person |
| Randy | individual | 2 | Randy Marsh | https://southpark.fandom.com/wiki/Randy_Marsh | https://static.wikia.nocookie.net/southparkfanon/images/d/d6/Randy_Marsh.png/revision/latest | 0 -10 428 428 | 428 | 1057 | Randy | Randy | 2 | 0 | 0 | 3 | 1 | true | person |
| Sharon | individual | 3 | Sharon Marsh | https://southpark.fandom.com/wiki/Sharon_Marsh | https://static.wikia.nocookie.net/southpark/images/9/99/Sharon_Marsh.png/revision/latest | 20 -10 390 390 | 439 | 1049 | Sharon | Sharon | 0 | 0 | 0 | 0 | 1 | true | person |

_ƒ 表示计算列。_

</details>

| 术语 | 描述 | 补充说明 |
|------|-------------|-------------------|
| **Portrait** | The page's Fandom portraits, including those for characters a reader may add. | — |
| Display Name | 一个已定义的属性。 | _The name on the portrait's Fandom page._ |
| Page | 一个已定义的属性。 | _The Fandom page credited for the portrait._ |
| Image | 一个已定义的属性。 | _The original Fandom image._ |
| Crop | 一个已定义的属性。 | _The crop as an SVG viewBox: x y width height._ |
| Width | 一个已定义的属性。 | _The original image's width in pixels._ |
| Height | 一个已定义的属性。 | _The original image's height in pixels._ |
| Sort Order | 一个已定义的属性。 | _Position in the credits._ |
| Name | 与 display name 相同。 | _Order 1. The portrait's name._ |

<details open>
<summary>示例数据</summary>

| Display Name | Page | Image | Crop | Width | Height | Sort Order | Name ƒ |
|---|---|---|---|---|---|---|---|
| Marvin Marsh | https://southpark.fandom.com/wiki/Marvin_Marsh | https://static.wikia.nocookie.net/southpark/images/d/db/Marsh-family-grandpa-marsh-w-purple-blanket.png/revision/latest | 48 -5 280 280 | 375 | 536 | 1 | Marvin Marsh |
| Randy Marsh | https://southpark.fandom.com/wiki/Randy_Marsh | https://static.wikia.nocookie.net/southparkfanon/images/d/d6/Randy_Marsh.png/revision/latest | 0 -10 428 428 | 428 | 1057 | 2 | Randy Marsh |
| Sharon Marsh | https://southpark.fandom.com/wiki/Sharon_Marsh | https://static.wikia.nocookie.net/southpark/images/9/99/Sharon_Marsh.png/revision/latest | 20 -10 390 390 | 439 | 1049 | 3 | Sharon Marsh |

_ƒ 表示计算列。_

</details>

| 术语 | 描述 | 补充说明 |
|------|-------------|-------------------|
| **Portrait Aliase** | The names a portrait answers to. A new thing whose name matches an alias gets that portrait. | — |
| Portrait | 一个已定义的属性。 | _The portrait this name selects._ |
| Name | 与 portrait alias ID 相同。 | _Order 1. The alias._ |

<details open>
<summary>示例数据</summary>

| Name ƒ |
|---|
| marvin |
| marvin marsh |
| grandpa marsh |

_ƒ 表示计算列。_

</details>

| 术语 | 描述 | 补充说明 |
|------|-------------|-------------------|
| **Property** | The ten properties, with what the symmetric, domain and range rules read. Axioms that relate one property to another (inverse, chain, disjoint) are rows of PropertyAxioms. | — |
| Label | 一个已定义的属性。 | _The property's wording on the page._ |
| Property Kind | 一个已定义的属性。 | _'object' links two things, 'datatype' links a thing to a value, 'type' states class membership._ |
| Datatype | 一个已定义的属性。 | _The value's XML Schema datatype for a datatype property; blank otherwise._ |
| Is User Addable | 当空字符串时为真。 | _Offered in the page's Add fact form._ |
| Has One Current Value | 当空字符串时为真。 | _The page keeps one current value per person: switching a value on replaces the earlier one._ |
| Is Symmetric | 当空字符串时为真。 | _The symmetric rule makes this link true in both directions._ |
| Domain Class | 一个已定义的属性。 | _The class the domain rule gives this property's subject._ |
| Range Class | 一个已定义的属性。 | _The class the range rule gives this property's object._ |
| Sort Order | 一个已定义的属性。 | _Position in the Add fact form's property list._ |
| Value Node Label | 一个已定义的属性。 | _The small label under a value node for this property: 'role value' or 'number value'._ |
| Current Value Wording | 一个已定义的属性。 | _How the Add fact form names a replaced value, as in 'Stan now has grade level 5'._ |
| Input Name | 一个已定义的属性。 | _How the integer check names this property's value, as in 'for grade level'._ |
| Record Kind | 一个已定义的属性。 | _The word the current-record summary uses for this property's values: role, grade or school._ |
| Record Order | 一个已定义的属性。 | _Position of this property in the current-record summary._ |
| Name | 与 label 相同。 | _Order 1. The property's wording._ |

<details open>
<summary>示例数据</summary>

| Label | Property Kind | Datatype | Is User Addable | Has One Current Value | Is Symmetric | Sort Order | Value Node Label | Current Value Wording | Input Name | Record Kind | Record Order | Name ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|
| is parent of | object | — | true | false | false | 1 | — | — | — | — | — | is parent of |
| is child of | object | — | true | false | false | 2 | — | — | — | — | — | is child of |
| is spouse of | object | — | true | false | true | 3 | — | — | — | — | — | is spouse of |

_ƒ 表示计算列。_

</details>

| 术语 | 描述 | 补充说明 |
|------|-------------|-------------------|
| **Property Axiom** | Axioms that relate one property to others, as OWL states them: an inverse, a two-link chain, or a disjoint pair. The candidate generator reads these rows. | — |
| Kind | 一个已定义的属性。 | _inverse, chain or disjoint._ |
| Property | 一个已定义的属性。 | _The property the axiom is about: the one inverted, the one a chain derives, or the first of a disjoint pair._ |
| First | 一个已定义的属性。 | _The inverse property, the chain's first link, or the other property of a disjoint pair._ |
| Second | 一个已定义的属性。 | _The chain's second link._ |
| Name | 与 property axiom ID 相同。 | _Order 1. The axiom's key._ |

<details open>
<summary>示例数据</summary>

| Name ƒ |
|---|
| inverse:isParentOf |
| disjoint:isParentOf~isSpouseOf |
| inverse:isChildOf |

_ƒ 表示计算列。_

</details>

| 术语 | 描述 | 补充说明 |
|------|-------------|-------------------|
| **Statement** | Every fact the page can state or conclude about the cast. A listed fact appears in the page's fact list; the rest are candidates that only a rule can make hold. | — |
| Graph | 一个已定义的属性。 | _The graph the fact belongs to._ |
| Subject | 一个已定义的属性。 | _What the fact describes._ |
| Property | 一个已定义的属性。 | _How the subject connects to the object._ |
| Object Thing | 一个已定义的属性。 | _The related thing, for an object or type property._ |
| Object Value | 一个已定义的属性。 | _The literal value, for a datatype property._ |
| Is Listed | 当空字符串时为真。 | _Shown in the page's fact list._ |
| Is Built in | 当空字符串时为真。 | _One of the page's teaching facts; the reader cannot remove it._ |
| Is Detail | 当空字符串时为真。 | _One of Stan's detail facts that Show Stan's details switches on._ |
| Is Stated | 当空字符串时为真。 | _The fact's checkbox._ |
| Activation Seq | 一个已定义的属性。 | _When the fact was last switched on, as a running number; decides which value is current._ |
| Sort Order | 一个已定义的属性。 | _Position in the fact list; candidates follow the listed facts._ |
| Subject Label | 取自所关联的“subject”。 | _Order 1. The subject's name._ |
| Property Label | 取自所关联的“property”。 | _Order 1. The property's wording._ |
| Object Label | 取自所关联的“object thing”。 | _Order 1. The object's name, for an object or type property._ |
| Property Datatype | 取自所关联的“property”。 | _Order 1. The value's datatype, for a datatype property._ |
| Property Has One Current Value | 当所关联的“property”具有“One Current Value”时为真。 | _Order 1. Whether the property keeps one current value per person._ |
| Property Domain Class | 取自所关联的“property”。 | _Order 1. The property's domain class._ |
| Property Range Class | 取自所关联的“property”。 | _Order 1. The property's range class._ |
| Literal Quote | 取自所关联的“graph”。 | _Order 1. The graph's literal quote mark._ |
| Literal Type Marker | 取自所关联的“graph”。 | _Order 1. The graph's datatype marker._ |
| Built in Source Text | 取自所关联的“graph”。 | _Order 1. The graph's source line for a teaching fact._ |
| User Source Text | 取自所关联的“graph”。 | _Order 1. The graph's source line for a reader's fact._ |
| Next Fact Sort Order | 取自所关联的“graph”。 | _Order 6. List position of the next fact to add._ |
| Is Literal | 当object thing为空时为真。 | _Order 1. True when the object is a value rather than a thing._ |
| Object Text | 按优先级确定:当object thing为空时取 object value;其余情况取 object label。 | _Order 2. The object's name or the value._ |
| Name | 按 subject label,接着是一个空格,接着是property label,接着是一个空格,接着是object text 计算。 | _Order 3. The fact in words._ |
| Words | 按 subject label,接着是一个空格,接着是property label,接着是一个空格,接着是当object thing为空时为 literal quote,接着是object value,接着是literal quote,接着是literal type marker,接着是property datatype,其余情况为 object label 计算。 ⚠︎ 机械 <!-- rulespeak:reword --> | _Order 2. The fact as a conclusion card quotes it, with a literal written in Turtle form._ |
| Source | 按优先级确定:当标志“built in”已设置时取 built in source text;其余情况取 user source text。 | _Order 2. Where a listed fact came from._ |
| Subject Property Key | 按 subject,接着是“|”,接着是property 计算。 | _Order 1. Groups the values one subject has for one property._ |
| Latest Sibling Activation Seq | 与statement相关的所有“statements”的“activation seq”的最大值。 | _Order 2. The most recent activation among this subject's values for this property._ |
| Is in Effect | 当以下全部成立:标志“stated”已设置且以下至少一项成立:标志“property has one current value”未设置或activation seq为latest sibling activation seq时为真。 | _Order 3. Switched on and, for a one-value property, the most recently switched-on value. Drawn as a solid line._ |
| Listed Unit | 按优先级确定:当标志“listed”已设置时取 1;其余情况取 0。 | _Order 1. 1 when the fact is in the fact list._ |
| In Effect Unit | 按优先级确定:当标志“in effect”已设置时取 1;其余情况取 0。 | _Order 4. 1 when the fact is in effect._ |
| Listed Off Unit | 按优先级确定:当以下全部成立:标志“listed”已设置且标志“in effect”未设置时取 1;其余情况取 0。 | _Order 4. 1 when the fact is listed but not in effect._ |
| Is Next to Add | 当以下全部成立:listed off unit为1且sort order为next fact sort order时为真。 | _Order 7. The fact the Add next fact button switches on._ |
| Count of Inverse Firings | 与statement相关的所有“firings”的“inverse fired unit”的总和。 | _Order 6. Inverse rule applications that conclude this fact._ |
| Count of Symmetric Firings | 与statement相关的所有“firings”的“symmetric fired unit”的总和。 | _Order 6. Symmetric rule applications that conclude this fact._ |
| Holds Before Chain | 当以下至少一项成立:标志“in effect”已设置、count of inverse firings大于0,或count of symmetric firings大于0时为真。 | _Order 7. Holds from facts in effect plus the inverse and symmetric rules._ |
| Count of Chain Firings | 与statement相关的所有“firings”的“chain fired unit”的总和。 | _Order 10. Property chain applications that conclude this fact._ |
| Holds Before Typing | 当以下至少一项成立:标志“holds before chain”已设置或count of chain firings大于0时为真。 | _Order 11. Holds before the domain and range rules apply._ |
| Count of Type Firings | 与statement相关的所有“firings”的“type fired unit”的总和。 | _Order 14. Domain and range applications that conclude this fact._ |
| Holds | 当以下至少一项成立:标志“holds before typing”已设置或count of type firings大于0时为真。 | _Order 15. True when in effect or concluded by an enabled rule; the graph draws it._ |
| Is Conclusion | 当以下全部成立:标志“holds”已设置且标志“in effect”未设置时为真。 | _Order 16. Holds only because a rule concluded it. Drawn dashed._ |
| Holds Unit | 按优先级确定:当标志“holds”已设置时取 1;其余情况取 0。 | _Order 16. 1 when the fact holds._ |
| Conclusion Unit | 按优先级确定:当标志“conclusion”已设置时取 1;其余情况取 0。 | _Order 17. 1 when the fact is a conclusion._ |
| Parent Unit | 按优先级确定:当以下全部成立:标志“holds”已设置且property为“isParentOf”时取 1;其余情况取 0。 | _Order 16. 1 for a holding parent link._ |
| Grandparent Unit | 按优先级确定:当以下全部成立:标志“holds”已设置且property为“isGrandparentOf”时取 1;其余情况取 0。 | _Order 16. 1 for a holding grandparent link._ |
| Place Unit | 按优先级确定:当以下全部成立:标志“holds”已设置且property为“attends”时取 1;其余情况取 0。 | _Order 16. 1 for a holding attends link._ |
| First Pre Chain Order Key | Statement的“firings”中是“Pre Chain Fired Unit”的“pre chain order key”的最小值。 | _Order 7. Order key of the first inverse or symmetric application that concludes this fact._ |
| Snap Position | 按优先级确定:当标志“in effect”已设置时取 sort order; 当标志“holds before chain”已设置时取 100000加first pre chain order key;其余情况取 0。 | _Order 8. Where the page's reasoner meets this fact as a premise: a stated fact at its list position, a fact concluded by the inverse or symmetric rule after every stated fact, in the order it was concluded; 0 for a fact that does not hold yet, so a link to no fact reads the same on every engine._ |
| First Fired Order Key | Statement的“firings”中是“Fired Unit”的“order key”的最小值。 | _Order 15. Order key of the first rule application that concludes this fact, in the page reasoner's order._ |
| Count of Chosen Firings | 与statement相关的所有“firings”的“chosen unit”的总和。 | _Order 20. Rule applications shown as this fact's conclusion card: one for a conclusion, zero otherwise._ |
| Subject Node Key | 与 subject 相同。 | _Order 1. The graph node this fact starts from._ |
| Object Node Key | 按优先级确定:当object thing为空时取 “literal|”,接着是object value,接着是“|”,接着是property datatype;其余情况取 object thing。 | _Order 2. The graph node this fact points at: the thing, or one node per value and datatype._ |
| Subject Node Kind | 取自所关联的“subject”。 | _Order 19. How the graph draws the subject._ |
| Object Thing Node Kind | 取自所关联的“object thing”。 | _Order 19. How the graph draws the object thing._ |
| Object Node Kind | 按优先级确定:当object thing为空时取 “value”;其余情况取 object thing node kind。 | _Order 20. How the graph draws the object: a value, a class, a place or a person._ |
| Object Value Node Label | 取自所关联的“property”。 | _Order 1. The small label under a value node._ |
| Count of Violations As First | 与statement相关的所有“disjoint checks”的“violation unit”的总和。 | _Order 18. Violated disjointness checks naming this fact first._ |
| Count of Violations As Second | 与statement相关的所有“disjoint checks”的“violation unit”的总和。 | _Order 18. Violated disjointness checks naming this fact second._ |
| Is in Contradiction | 当以下至少一项成立:count of violations as first大于0或count of violations as second大于0时为真。 | _Order 19. Part of a contradiction the disjoint rule found; drawn in the alarm colour._ |

<details open>
<summary>示例数据</summary>

| Object Value | Is Listed | Is Built in | Is Detail | Is Stated | Activation Seq | Sort Order | Subject Label ƒ | Property Label ƒ | Object Label ƒ | Property Datatype ƒ | Property Has One Current Value ƒ | Property Domain Class ƒ | Property Range Class ƒ | Literal Quote ƒ | Literal Type Marker ƒ | Built in Source Text ƒ | User Source Text ƒ | Next Fact Sort Order ƒ | Is Literal ƒ | Object Text ƒ | Name ƒ | Words ƒ | Source ƒ | Subject Property Key ƒ | Latest Sibling Activation Seq ƒ | Is in Effect ƒ | Listed Unit ƒ | In Effect Unit ƒ | Listed Off Unit ƒ | Is Next to Add ƒ | Count of Inverse Firings ƒ | Count of Symmetric Firings ƒ | Holds Before Chain ƒ | Count of Chain Firings ƒ | Holds Before Typing ƒ | Count of Type Firings ƒ | Holds ƒ | Is Conclusion ƒ | Holds Unit ƒ | Conclusion Unit ƒ | Parent Unit ƒ | Grandparent Unit ƒ | Place Unit ƒ | First Pre Chain Order Key ƒ | Snap Position ƒ | First Fired Order Key ƒ | Count of Chosen Firings ƒ | Subject Node Key ƒ | Object Node Key ƒ | Subject Node Kind ƒ | Object Thing Node Kind ƒ | Object Node Kind ƒ | Object Value Node Label ƒ | Count of Violations As First ƒ | Count of Violations As Second ƒ | Is in Contradiction ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| — | true | true | false | true | 1 | 1 | Marvin | is parent of | Randy | — | false | — | — | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | — | false | Randy | Marvin is parent of Randy | Marvin is parent of Randy | Stated as a teaching example on this page. | Marvin\|isParentOf | 1 | true | 1 | 1 | 0 | false | 0 | 0 | true | 0 | true | 0 | true | false | 1 | 0 | 1 | 0 | 0 | — | 1.0 | — | 0 | Marvin | Randy | person | person | person | — | 0 | 0 | false |
| — | false | false | false | false | 0 | 100001 | Marvin | is parent of | Sharon | — | false | — | — | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | — | false | Sharon | Marvin is parent of Sharon | Marvin is parent of Sharon | Stated by you on this page. | Marvin\|isParentOf | 1 | false | 0 | 0 | 0 | false | 0 | 0 | false | 0 | false | 0 | false | false | 0 | 0 | 0 | 0 | 0 | — | 0.0 | — | 0 | Marvin | Sharon | person | person | person | — | 0 | 0 | false |
| — | false | false | false | false | 0 | 100002 | Marvin | is parent of | Stan | — | false | — | — | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | — | false | Stan | Marvin is parent of Stan | Marvin is parent of Stan | Stated by you on this page. | Marvin\|isParentOf | 1 | false | 0 | 0 | 0 | false | 0 | 0 | false | 0 | false | 0 | false | false | 0 | 0 | 0 | 0 | 0 | — | 0.0 | — | 0 | Marvin | Stan | person | person | person | — | 0 | 0 | false |

_ƒ 表示计算列。_

</details>

| 术语 | 描述 | 补充说明 |
|------|-------------|-------------------|
| **Firing** | Every way an enabled rule could conclude a candidate fact from the facts that hold. | — |
| Kind | 一个已定义的属性。 | _How the rule applies._ |
| Rule | 一个已定义的属性。 | _The rule switch._ |
| Target | 一个已定义的属性。 | _The fact this application concludes._ |
| First | 一个已定义的属性。 | _The premise._ |
| Second | 一个已定义的属性。 | _The second premise, for the property chain._ |
| Rule Enabled | 当所关联的“rule”是“Enabled”时为真。 | _Order 1. Whether the rule is switched on._ |
| Kind Wording | 取自所关联的“kind”。 | _Order 1. How the card names the rule._ |
| Kind Sort Order | 取自所关联的“kind”。 | _Order 1. The kind's place in the reasoner's order._ |
| Target Name | 取自所关联的“target”。 | _Order 4. The concluded fact in words._ |
| Target Subject Label | 取自所关联的“target”。 | _Order 2. The concluded fact's subject._ |
| Target Property Label | 取自所关联的“target”。 | _Order 2. The concluded fact's property._ |
| Target Object Text | 取自所关联的“target”。 | _Order 3. The concluded fact's object._ |
| Target is Conclusion | 当所关联的“target”是“Conclusion”时为真。 | _Order 17. Whether the concluded fact is new._ |
| Target Subject Node Key | 取自所关联的“target”。 | _Order 2. The concluded fact's subject node._ |
| Target Object Node Key | 取自所关联的“target”。 | _Order 3. The concluded fact's object node._ |
| Target First Fired Order Key | 取自所关联的“target”。 | _Order 16. The order key of the concluded fact's first application._ |
| First Words | 取自所关联的“first”。 | _Order 3. The premise as the card quotes it._ |
| Second Words | 取自所关联的“second”。 | _Order 3. The second premise as the card quotes it._ |
| First is in Effect | 当所关联的“first”是“In Effect”时为真。 | _Order 4. Whether the premise is in effect._ |
| First Holds Before Chain | 当所关联的“first”是“Holds Before Chain”时为真。 | _Order 8. Whether the premise holds before the chain._ |
| Second Holds Before Chain | 当所关联的“second”是“Holds Before Chain”时为真。 | _Order 8. Whether the second premise holds before the chain._ |
| First Holds Before Typing | 当所关联的“first”是“Holds Before Typing”时为真。 | _Order 12. Whether the premise holds before typing._ |
| First Property | 取自所关联的“first”。 | _Order 1. The premise's property key, as the card's axiom names it._ |
| First Property Domain Class | 取自所关联的“first”。 | _Order 2. The premise property's domain class._ |
| First Property Range Class | 取自所关联的“first”。 | _Order 2. The premise property's range class._ |
| First Sort Order | 取自所关联的“first”。 | _Order 1. The premise's list position._ |
| Second is in Effect | 当所关联的“second”是“In Effect”时为真。 | _Order 4. Whether the second premise is in effect._ |
| First Snap Position | 取自所关联的“first”。 | _Order 9. Where the reasoner meets the premise._ |
| Second Snap Position | 取自所关联的“second”。 | _Order 9. Where the reasoner meets the second premise._ |
| Inverse Fired Unit | 按优先级确定:当以下全部成立:标志“rule enabled”已设置、kind为“inverse”,且标志“first is in effect”已设置时取 1;其余情况取 0。 | _Order 5. 1 when the inverse rule fires here._ |
| Symmetric Fired Unit | 按优先级确定:当以下全部成立:标志“rule enabled”已设置、kind为“symmetric”,且标志“first is in effect”已设置时取 1;其余情况取 0。 | _Order 5. 1 when the symmetric rule fires here._ |
| Chain Fired Unit | 按优先级确定:当以下全部成立:标志“rule enabled”已设置、kind为“chain”、标志“first holds before chain”已设置,且标志“second holds before chain”已设置时取 1;其余情况取 0。 | _Order 9. 1 when the property chain fires here._ |
| Type Fired Unit | 按优先级确定:当以下全部成立:标志“rule enabled”已设置、以下至少一项成立:kind为“domain”或kind为“range”,且标志“first holds before typing”已设置时取 1;其余情况取 0。 | _Order 13. 1 when the domain or range rule fires here._ |
| Fired Unit | 按 inverse fired unit加symmetric fired unit加chain fired unit加type fired unit 计算。 | _Order 14. 1 when this application fires._ |
| Pre Chain Fired Unit | 按 inverse fired unit加symmetric fired unit 计算。 | _Order 6. 1 when this is an inverse or symmetric application that fires._ |
| Pre Chain Order Key | 按 first sort order乘10加kind sort order 计算。 | _Order 2. For an inverse or symmetric application: its place in the reasoner's first round._ |
| Round | 按优先级确定:当以下全部成立:kind为“chain”且并非以下全部成立:标志“first is in effect”已设置且标志“second is in effect”已设置时取 2;其余情况取 1。 | _Order 5. The reasoner's round: 1 when every premise is a stated fact, 2 when the property chain reads a concluded fact._ |
| Order Key | 按 round乘1000000000000000加first snap position乘100000000加kind sort order乘10000000加当second为空时为 0,其余情况为 second snap position 计算。 | _Order 10. The page reasoner's order: by round, then premise, then rule kind, then second premise._ |
| Is Chosen | 当以下全部成立:fired unit为1、标志“target is conclusion”已设置,且order key为target first fired order key时为真。 | _Order 18. The application the conclusions panel shows for its fact: the first one in the reasoner's order._ |
| Chosen Unit | 按优先级确定:当标志“chosen”已设置时取 1;其余情况取 0。 | _Order 19. 1 when this application is shown._ |
| Card Text | 按优先级确定:当标志“chosen”已设置时取 “Worked out by the ”,接着是kind wording,接着是“ from “”,接着是first words,接着是“””,接着是当kind为“chain”时为 “ and “”,接着是second words,接着是“””,其余情况为 当kind为“domain”时为 “ and “”,接着是first property,接着是“ has domain ”,接着是first property domain class,接着是“””,其余情况为 “ and “”,接着是first property,接着是“ has range ”,接着是first property range class,接着是“””,接着是一个句点;其余情况取 空字符串。 ⚠︎ 机械 <!-- rulespeak:reword --> | _Order 19. The conclusion card's provenance line, in the page's wording._ |
| Name | 与 firing ID 相同。 | _Order 1. The application's key._ |

<details open>
<summary>示例数据</summary>

| Rule Enabled ƒ | Kind Wording ƒ | Kind Sort Order ƒ | Target Name ƒ | Target Subject Label ƒ | Target Property Label ƒ | Target Object Text ƒ | Target is Conclusion ƒ | Target Subject Node Key ƒ | Target Object Node Key ƒ | Target First Fired Order Key ƒ | First Words ƒ | Second Words ƒ | First is in Effect ƒ | First Holds Before Chain ƒ | Second Holds Before Chain ƒ | First Holds Before Typing ƒ | First Property ƒ | First Property Domain Class ƒ | First Property Range Class ƒ | First Sort Order ƒ | Second is in Effect ƒ | First Snap Position ƒ | Second Snap Position ƒ | Inverse Fired Unit ƒ | Symmetric Fired Unit ƒ | Chain Fired Unit ƒ | Type Fired Unit ƒ | Fired Unit ƒ | Pre Chain Fired Unit ƒ | Pre Chain Order Key ƒ | Round ƒ | Order Key ƒ | Is Chosen ƒ | Chosen Unit ƒ | Card Text ƒ | Name ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| false | inverse rule | 1 | Randy is child of Marvin | Randy | is child of | Marvin | false | Randy | Marvin | — | Marvin is parent of Randy |    | true | true | false | true | isParentOf | — | — | 1 | false | 1.0 | 0.0 | 0 | 0 | 0 | 0 | 0 | 0 | 11.0 | 1 | 1000000110000000.0 | false | 0 | — | inverse:Marvin\|isParentOf\|Randy>Randy\|isChildOf\|Marvin |
| false | inverse rule | 1 | Sharon is child of Marvin | Sharon | is child of | Marvin | false | Sharon | Marvin | — | Marvin is parent of Sharon |    | false | false | false | false | isParentOf | — | — | 100001 | false | 0.0 | 0.0 | 0 | 0 | 0 | 0 | 0 | 0 | 1000011.0 | 1 | 1000000010000000.0 | false | 0 | — | inverse:Marvin\|isParentOf\|Sharon>Sharon\|isChildOf\|Marvin |
| false | inverse rule | 1 | Stan is child of Marvin | Stan | is child of | Marvin | false | Stan | Marvin | — | Marvin is parent of Stan |    | false | false | false | false | isParentOf | — | — | 100002 | false | 0.0 | 0.0 | 0 | 0 | 0 | 0 | 0 | 0 | 1000021.0 | 1 | 1000000010000000.0 | false | 0 | — | inverse:Marvin\|isParentOf\|Stan>Stan\|isChildOf\|Marvin |

_ƒ 表示计算列。_

</details>

| 术语 | 描述 | 补充说明 |
|------|-------------|-------------------|
| **Disjoint Check** | Every pair the disjointness rule watches: the same two people linked by two disjoint properties. | — |
| Graph | 一个已定义的属性。 | _The graph._ |
| Rule | 一个已定义的属性。 | _The rule switch._ |
| First | 一个已定义的属性。 | _The first link._ |
| Second | 一个已定义的属性。 | _The disjoint link between the same pair, in the same direction._ |
| Rule Enabled | 当所关联的“rule”是“Enabled”时为真。 | _Order 1. Whether the disjointness rule is on._ |
| First Holds | 当所关联的“first”是“Holds”时为真。 | _Order 16. Whether the first link holds._ |
| Second Holds | 当所关联的“second”是“Holds”时为真。 | _Order 16. Whether the second link holds._ |
| Subject Label | 取自所关联的“first”。 | _Order 2. The person recorded with both links._ |
| Object Label | 取自所关联的“first”。 | _Order 2. The other person._ |
| Violation Unit | 按优先级确定:当以下全部成立:标志“rule enabled”已设置、标志“first holds”已设置,且标志“second holds”已设置时取 1;其余情况取 0。 | _Order 17. 1 when both links hold._ |
| Alarm Text | 按优先级确定:当violation unit为1时取 “Contradiction found. ”,接着是subject label,接着是“ is recorded as both parent and spouse of ”,接着是object label,接着是“. Check those facts and the selected disjoint rule.”;其余情况取 空字符串。 | _Order 18. The page's contradiction alert, blank when there is none._ |
| Name | 与 disjoint check ID 相同。 | _Order 1. The check's key._ |

<details open>
<summary>示例数据</summary>

| Rule Enabled ƒ | First Holds ƒ | Second Holds ƒ | Subject Label ƒ | Object Label ƒ | Violation Unit ƒ | Alarm Text ƒ | Name ƒ |
|---|---|---|---|---|---|---|---|
| false | true | false | Marvin | Randy | 0 | — | Marvin\|isParentOf\|Randy~isSpouseOf |
| false | false | false | Marvin | Sharon | 0 | — | Marvin\|isParentOf\|Sharon~isSpouseOf |
| false | false | false | Marvin | Stan | 0 | — | Marvin\|isParentOf\|Stan~isSpouseOf |

_ƒ 表示计算列。_

</details>

| 术语 | 描述 | 补充说明 |
|------|-------------|-------------------|
| **Scenario** | The page's buttons. Each one is a fact baseline, explicit fact changes and rule settings, applied in that order. | — |
| Graph | 一个已定义的属性。 | _The graph._ |
| Label | 一个已定义的属性。 | _The button's text._ |
| Note | 一个已定义的属性。 | _The note shown under the buttons after this one is pressed; blank clears it._ |
| Sort Order | 一个已定义的属性。 | _Position among the buttons._ |
| Button Group | 一个已定义的属性。 | _'stan' for the two imagined changes, 'facts' for the buttons under the fact list._ |
| Is Primary | 当空字符串时为真。 | _Drawn as the filled button._ |
| Fact Baseline | 一个已定义的属性。 | _What happens to every listed fact first: keep, built-in (teaching facts on, others off), built-in-detail, all-off, activate-all (each switched on in list order), or next (the next fact on)._ |
| Layout | 一个已定义的属性。 | _The node layout the graph returns to; blank keeps the current positions._ |
| Clears Selection | 当空字符串时为真。 | _Clears the selected node or line._ |
| Clears Form Message | 当空字符串时为真。 | _Clears the Add fact form's message._ |
| Name | 与 label 相同。 | _Order 1. The button's text._ |
| Graph Has Facts to Add | 当所关联的“graph”具有“Facts to Add”时为真。 | _Order 7. Whether a listed fact can still be added._ |
| Is Available | 当以下至少一项成立:fact baseline不为“next”或标志“graph has facts to add”已设置时为真。 | _Order 8. False only for Add next fact once every listed fact is in effect._ |

<details open>
<summary>示例数据</summary>

| Label | Note | Sort Order | Button Group | Is Primary | Fact Baseline | Layout | Clears Selection | Clears Form Message | Name ƒ | Graph Has Facts to Add ƒ | Is Available ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|
| Move Stan to grade 5 | This imagined change updates Stan’s grade to 5. His identifier, Person statement and family links stay the same. His other selected details are unchanged. | 1 | stan | false | keep | — | true | true | Move Stan to grade 5 | false | true |
| Stan leaves school | This imagined change clears Stan’s current school details. His identifier, Person statement and family links remain. Removing a current fact does not say that its opposite is true; school history needs dates. | 2 | stan | false | keep | — | true | true | Stan leaves school | false | true |
| Add next fact | — | 3 | facts | true | next | — | false | false | Add next fact | false | false |

_ƒ 表示计算列。_

</details>

| 术语 | 描述 | 补充说明 |
|------|-------------|-------------------|
| **Scenario Fact Change** | Fact changes a button makes after its baseline. A blank object key matches every listed value of the property. | — |
| Scenario | 一个已定义的属性。 | _The button._ |
| Subject | 一个已定义的属性。 | _The subject whose facts change._ |
| Property | 一个已定义的属性。 | _The property whose facts change._ |
| Object Key | 一个已定义的属性。 | _The object thing's key or the literal value; blank for every listed value._ |
| Action | 一个已定义的属性。 | _'on' switches the fact on as the newest value; 'off' switches it off._ |
| Adds to List | 当空字符串时为真。 | _Adds the fact to the fact list first when it is not listed._ |
| Sort Order | 一个已定义的属性。 | _Order of the change within the button._ |
| Name | 与 scenario fact change ID 相同。 | _Order 1. The change's key._ |

<details open>
<summary>示例数据</summary>

| Object Key | Action | Adds to List | Sort Order | Name ƒ |
|---|---|---|---|---|
| Person | on | true | 1 | stan-grade-five:Stan\|isA\|Person |
| 5 | on | true | 2 | stan-grade-five:Stan\|hasGradeLevel\|5 |
| Person | on | true | 3 | stan-leaves-school:Stan\|isA\|Person |

_ƒ 表示计算列。_

</details>

| 术语 | 描述 | 补充说明 |
|------|-------------|-------------------|
| **Scenario Rule Setting** | Rule switches a button sets. | — |
| Scenario | 一个已定义的属性。 | _The button._ |
| Rule | 一个已定义的属性。 | _The rule switch._ |
| Is Enabled | 当空字符串时为真。 | _The state the button puts the switch in._ |
| Name | 与 scenario rule setting ID 相同。 | _Order 1. The setting's key._ |

<details open>
<summary>示例数据</summary>

| Is Enabled | Name ƒ |
|---|---|
| false | reset-original:inverse |
| false | reset-original:symmetric |
| false | reset-original:chain |

_ƒ 表示计算列。_

</details>

| 术语 | 描述 | 补充说明 |
|------|-------------|-------------------|
| **Node Layout** | Node positions in the graph's 380 by 480 drawing, one set per layout. | — |
| Layout | 一个已定义的属性。 | _The layout: 'default' or 'stan-details'._ |
| Node Key | 一个已定义的属性。 | _The node: a thing's key, or literal\|value\|datatype._ |
| X | 一个已定义的属性。 | _Horizontal position._ |
| Y | 一个已定义的属性。 | _Vertical position._ |
| Name | 与 node layout ID 相同。 | _Order 1. The position's key._ |

<details open>
<summary>示例数据</summary>

| Layout | Node Key | X | Y | Name ƒ |
|---|---|---|---|---|
| default | Marvin | 70 | 45 | default:Marvin |
| default | Randy | 195 | 125 | default:Randy |
| default | Sharon | 318 | 55 | default:Sharon |

_ƒ 表示计算列。_

</details>

| 术语 | 描述 | 补充说明 |
|------|-------------|-------------------|
| **Layout Slot** | Places for nodes no layout positions, used in turn. | — |
| Sort Order | 一个已定义的属性。 | _Order in which slots are used._ |
| X | 一个已定义的属性。 | _Horizontal position._ |
| Y | 一个已定义的属性。 | _Vertical position._ |
| Name | 与 layout slot ID 相同。 | _Order 1. The slot's key._ |

<details open>
<summary>示例数据</summary>

| Sort Order | X | Y | Name ƒ |
|---|---|---|---|
| 1 | 330 | 135 | slot-1 |
| 2 | 45 | 120 | slot-2 |
| 3 | 330 | 400 | slot-3 |

_ƒ 表示计算列。_

</details>

| 术语 | 描述 | 补充说明 |
|------|-------------|-------------------|
| **Page Text** | The page's wording, one row per string. {name}-style placeholders are filled by the page. | — |
| Text | 一个已定义的属性。 | _The string._ |
| Name | 与 page text ID 相同。 | _Order 1. The string's key._ |

<details open>
<summary>示例数据</summary>

| Text | Name ƒ |
|---|---|
| South Park Ontology Explainer, on one rulebook | page-title |
| A rulebook rebuild of the explainer by | byline-prefix |
| Jessica Talisman | byline-author |

_ƒ 表示计算列。_

</details>

| 术语 | 描述 | 补充说明 |
|------|-------------|-------------------|
| **Engine** | The execution engines the page can be computed by. Each is a build output of this rulebook. | — |
| Label | 一个已定义的属性。 | _The engine's name in the switcher._ |
| Substrate | 一个已定义的属性。 | _The conformance harness's name for this substrate._ |
| Is Default | 当空字符串时为真。 | _The engine a new visitor starts on._ |
| Description | 一个已定义的属性。 | _What computes the page on this engine._ |
| Sort Order | 一个已定义的属性。 | _Position in the switcher._ |
| Name | 与 label 相同。 | _Order 1. The engine's name._ |

<details open>
<summary>示例数据</summary>

| Label | Substrate | Is Default | Description | Sort Order | Name ƒ |
|---|---|---|---|---|---|
| Postgres views | effortless-postgres | true | Every value is a column of a generated view; the database computes it when the page reads it. | 1 | Postgres views |
| TypeScript | effortless-typescript | false | The generated TypeScript computes every field from the same rows, with no database. | 2 | TypeScript |
| OWL and SHACL rules | effortless-owl | false | The generated ontology and SHACL rules, run over the same rows by a SPARQL engine. | 3 | OWL and SHACL rules |

_ƒ 表示计算列。_

</details>

| 术语 | 描述 | 补充说明 |
|------|-------------|-------------------|
| **Rulebook Tool** | What each rulebook tool adds to the original explainer; the tools panel lists them. | — |
| Label | 一个已定义的属性。 | _The tool's name._ |
| Adds | 一个已定义的属性。 | _What it adds to the original explainer._ |
| Href | 一个已定义的属性。 | _Where its link goes; 'editor' means the running rulebook editor._ |
| Link Text | 一个已定义的属性。 | _The link's text._ |
| Sort Order | 一个已定义的属性。 | _Position in the panel._ |
| Name | 与 label 相同。 | _Order 1. The tool's name._ |

<details open>
<summary>示例数据</summary>

| Label | Adds | Href | Link Text | Sort Order | Name ƒ |
|---|---|---|---|---|---|
| Interchangeable engines | The same page, computed by Postgres views, generated TypeScript or the generated OWL and SHACL rules. Switch with the control above; nothing else changes. | — | — | 1 | Interchangeable engines |
| Conformance | Every engine's answers for the current facts, compared value by value with the conformance harness's own rule for when two values are equal. | — | — | 2 | Conformance |
| Explainer | Hover a dotted value for its rule in plain words and its witness: the values it came from, down to the stated facts. | /rulebook-explainer-dag/pages/index.html | Open the explainer | 3 | Explainer |

_ƒ 表示计算列。_

</details>

## 2 事实类型

- **firing kind**恰好引用一个**rule**
- **thing**可引用一个**portrait**
- **portrait aliase**恰好引用一个**portrait**
- **property**可引用一个**thing**
- **property axiom**恰好引用一个**firing kind**
- **property axiom**恰好引用一个**property**
- **statement**恰好引用一个**graph**
- **statement**恰好引用一个**thing**
- **statement**恰好引用一个**property**
- **firing**恰好引用一个**firing kind**
- **firing**恰好引用一个**rule**
- **firing**恰好引用一个**statement**
- **disjoint check**恰好引用一个**graph**
- **disjoint check**恰好引用一个**rule**
- **disjoint check**恰好引用一个**statement**
- **scenario**恰好引用一个**graph**
- **scenario fact change**恰好引用一个**scenario**
- **scenario fact change**恰好引用一个**thing**
- **scenario fact change**恰好引用一个**property**
- **scenario rule setting**恰好引用一个**scenario**
- **scenario rule setting**恰好引用一个**rule**

## 3 操作性规则

_操作性规则陈述业务**强制**、**禁止**或**建议**的内容。结构性规则来自必填字段和
外键;语义规则来自 Constraints 表,每条都依据规则手册已经计算的布尔字段
(在下方定义性规则中以 DR-N 交叉引用)。_

### 结构性约束(来自模式)

- graph**必须**具有label、literal quote、literal type marker、built in source text、user source text,且no conclusions text。
- rule**必须**具有label、explanation,且sort order,并记录其是“Enabled”与否。
- firing kind**必须**恰好引用一个“rule”。
- firing kind**必须**具有wording且sort order。
- thing**必须**具有label、kind,且sort order。
- portrait**必须**具有display name、page、image、crop、width、height,且sort order。
- portrait aliase**必须**恰好引用一个“portrait”。
- property**必须**具有label、property kind,且sort order,并记录其是“User Addable”与否、其具有“One Current Value”与否,且其是“Symmetric”与否。
- property axiom**必须**恰好引用一个“firing kind”作为其kind。
- property axiom**必须**恰好引用一个“property”。
- property axiom**必须**恰好引用一个“property”作为其first。
- statement**必须**恰好引用一个“graph”。
- statement**必须**恰好引用一个“thing”作为其subject。
- statement**必须**恰好引用一个“property”。
- statement**必须**具有activation seq且sort order,并记录其是“Listed”与否、其是“Built in”与否、其是“Detail”与否,且其是“Stated”与否。
- firing**必须**恰好引用一个“firing kind”作为其kind。
- firing**必须**恰好引用一个“rule”。
- firing**必须**恰好引用一个“statement”作为其target。
- firing**必须**恰好引用一个“statement”作为其first。
- disjoint check**必须**恰好引用一个“graph”。
- disjoint check**必须**恰好引用一个“rule”。
- disjoint check**必须**恰好引用一个“statement”作为其first。
- disjoint check**必须**恰好引用一个“statement”作为其second。
- scenario**必须**恰好引用一个“graph”。
- scenario**必须**具有label、sort order、button group,且fact baseline,并记录其是“Primary”与否、其是“Clears Selection”与否,且其是“Clears Form Message”与否。
- scenario fact change**必须**恰好引用一个“scenario”。
- scenario fact change**必须**恰好引用一个“thing”作为其subject。
- scenario fact change**必须**恰好引用一个“property”。
- scenario fact change**必须**具有action且sort order,并记录其是“Adds to List”与否。
- scenario rule setting**必须**恰好引用一个“scenario”。
- scenario rule setting**必须**恰好引用一个“rule”。
- scenario rule setting**必须**记录其是“Enabled”与否。
- node layout**必须**具有layout、node key、x,且y。
- layout slot**必须**具有sort order、x,且y。
- page text**必须**具有text。
- engine**必须**具有label、substrate、description,且sort order,并记录其是“Default”与否。
- rulebook tool**必须**具有label、adds,且sort order。

## 4 定义性规则

_所有语句都表达业务领域中的真理;它们既不是过程,也不是命令。避免使用
"当且仅当",改用"仅当",以免把单向必要条件误当作等价关系。
**⚠︎ 机械** 标记表示某条规则的确定性措辞忠实但生硬——
这是可选改写的信号,不是缺陷。_

| ID | 声明式规则 |
|----|------------------|
| **DR-1 Name** | graph的“name”与 label 相同。 |
| **DR-2 Count of Stated Facts** | graph的“count of stated facts”为 与graph相关的所有“statements”的“in effect unit”的总和。 |
| **DR-3 Count of Conclusions** | graph的“count of conclusions”为 与graph相关的所有“statements”的“conclusion unit”的总和。 |
| **DR-4 Count of Violations** | graph的“count of violations”为 与graph相关的所有“disjoint checks”的“violation unit”的总和。 |
| **DR-5 Is Consistent** | 当count of violations为0时,graph被视为“Consistent”。 |
| **DR-6 Conclusions Message** | Graph的“conclusions message”按以下优先级确定:<br>1. 当count of conclusions为0时,取 no conclusions text;<br>2. 其余情况取 空字符串。 |
| **DR-7 Count of Facts to Add** | graph的“count of facts to add”为 与graph相关的所有“statements”的“listed off unit”的总和。 |
| **DR-8 Has Facts to Add** | 当count of facts to add大于0时,graph被视为具有“Facts to Add”。 |
| **DR-9 Next Fact Sort Order** | graph的“next fact sort order”为 graph的“statements”中是“Listed Off Unit”的“sort order”的最小值。 |
| **DR-10 Max Activation Seq** | graph的“max activation seq”为 与graph相关的所有“statements”的“activation seq”的最大值。 |
| **DR-11 Next Activation Seq** | graph的“next activation seq”按 max activation seq加1 计算。 |
| **DR-12 Max Listed Sort Order** | graph的“max listed sort order”为 graph的“statements”中是“Listed Unit”的“sort order”的最大值。 |
| **DR-13 Next Listed Sort Order** | graph的“next listed sort order”按 max listed sort order加1 计算。 |
| **DR-14 Max Sort Order** | graph的“max sort order”为 与graph相关的所有“statements”的“sort order”的最大值。 |
| **DR-15 Next Candidate Sort Order** | graph的“next candidate sort order”按 max sort order加1 计算。 |
| **DR-16 Name** | rule的“name”与 label 相同。 |
| **DR-17 Count of Firings** | rule的“count of firings”为 与rule相关的所有“firings”的“fired unit”的总和。 |
| **DR-18 Name** | firing kind的“name”与 wording 相同。 |
| **DR-19 Rule Enabled** | firing kind的“rule enabled”在所关联的“rule”是“Enabled”时为真。 |
| **DR-20 Portrait Name** | thing的“portrait name”是所关联的“portrait”的“display name”。 |
| **DR-21 Portrait Page** | thing的“portrait page”——取自所关联的“portrait”。 |
| **DR-22 Portrait Image** | thing的“portrait image”——取自所关联的“portrait”。 |
| **DR-23 Portrait Crop** | thing的“portrait crop”——取自所关联的“portrait”。 |
| **DR-24 Portrait Width** | thing的“portrait width”——取自所关联的“portrait”。 |
| **DR-25 Portrait Height** | thing的“portrait height”——取自所关联的“portrait”。 |
| **DR-26 Name** | thing的“name”与 label 相同。 |
| **DR-27 Node Key** | thing的“node key”与 thing ID 相同。 |
| **DR-28 Count of Children** | thing的“count of children”为 与thing相关的所有“statements”的“parent unit”的总和。 |
| **DR-29 Count of Grandchildren** | thing的“count of grandchildren”为 与thing相关的所有“statements”的“grandparent unit”的总和。 |
| **DR-30 Count As Place** | thing的“count as place”为 与thing相关的所有“statements”的“place unit”的总和。 |
| **DR-31 Count As Subject** | thing的“count as subject”为 与thing相关的所有“statements”的“holds unit”的总和。 |
| **DR-32 Count As Object** | thing的“count as object”为 与thing相关的所有“statements”的“holds unit”的总和。 |
| **DR-33 Is in Graph** | 当以下至少一项成立:count as subject大于0或count as object大于0时,thing被视为“In Graph”。 |
| **DR-34 Node Kind** | Thing的“node kind”按以下优先级确定:<br>1. 当kind为空时,取 空字符串;<br>2. 当kind为“class”时,取 “class”;<br>3. 当count as place大于0时,取 “place”;<br>4. 其余情况取 “person”。 |
| **DR-35 Name** | portrait的“name”与 display name 相同。 |
| **DR-36 Name** | portrait aliase的“name”与 portrait alias ID 相同。 |
| **DR-37 Name** | property的“name”与 label 相同。 |
| **DR-38 Name** | property axiom的“name”与 property axiom ID 相同。 |
| **DR-39 Subject Label** | statement的“subject label”——取自所关联的“subject”。 |
| **DR-40 Property Label** | statement的“property label”——取自所关联的“property”。 |
| **DR-41 Object Label** | statement的“object label”——取自所关联的“object thing”。 |
| **DR-42 Property Datatype** | statement的“property datatype”——取自所关联的“property”。 |
| **DR-43 Property Has One Current Value** | statement的“property has one current value”在所关联的“property”具有“One Current Value”时为真。 |
| **DR-44 Property Domain Class** | statement的“property domain class”——取自所关联的“property”。 |
| **DR-45 Property Range Class** | statement的“property range class”——取自所关联的“property”。 |
| **DR-46 Literal Quote** | statement的“literal quote”——取自所关联的“graph”。 |
| **DR-47 Literal Type Marker** | statement的“literal type marker”——取自所关联的“graph”。 |
| **DR-48 Built in Source Text** | statement的“built in source text”——取自所关联的“graph”。 |
| **DR-49 User Source Text** | statement的“user source text”——取自所关联的“graph”。 |
| **DR-50 Next Fact Sort Order** | statement的“next fact sort order”——取自所关联的“graph”。 |
| **DR-51 Is Literal** | 当object thing为空时,statement被视为“Literal”。 |
| **DR-52 Object Text** | Statement的“object text”按以下优先级确定:<br>1. 当object thing为空时,取 object value;<br>2. 其余情况取 object label。 |
| **DR-53 Name** | statement的“name”按 subject label,接着是一个空格,接着是property label,接着是一个空格,接着是object text 计算。 |
| **DR-54 Words** | statement的“words”按 subject label,接着是一个空格,接着是property label,接着是一个空格,接着是当object thing为空时为 literal quote,接着是object value,接着是literal quote,接着是literal type marker,接着是property datatype,其余情况为 object label 计算。 ⚠︎ 机械 <!-- rulespeak:reword --> |
| **DR-55 Source** | Statement的“source”按以下优先级确定:<br>1. 当标志“built in”已设置时,取 built in source text;<br>2. 其余情况取 user source text。 |
| **DR-56 Subject Property Key** | statement的“subject property key”按 subject,接着是“|”,接着是property 计算。 |
| **DR-57 Latest Sibling Activation Seq** | statement的“latest sibling activation seq”为 与statement相关的所有“statements”的“activation seq”的最大值。 |
| **DR-58 Is in Effect** | 当以下全部成立:标志“stated”已设置且以下至少一项成立:标志“property has one current value”未设置或activation seq为latest sibling activation seq时,statement被视为“In Effect”。 |
| **DR-59 Listed Unit** | Statement的“listed unit”按以下优先级确定:<br>1. 当标志“listed”已设置时,取 1;<br>2. 其余情况取 0。 |
| **DR-60 In Effect Unit** | Statement的“in effect unit”按以下优先级确定:<br>1. 当标志“in effect”已设置时,取 1;<br>2. 其余情况取 0。 |
| **DR-61 Listed Off Unit** | Statement的“listed off unit”按以下优先级确定:<br>1. 当以下全部成立:标志“listed”已设置且标志“in effect”未设置时,取 1;<br>2. 其余情况取 0。 |
| **DR-62 Is Next to Add** | 当以下全部成立:listed off unit为1且sort order为next fact sort order时,statement被视为“Next to Add”。 |
| **DR-63 Count of Inverse Firings** | statement的“count of inverse firings”为 与statement相关的所有“firings”的“inverse fired unit”的总和。 |
| **DR-64 Count of Symmetric Firings** | statement的“count of symmetric firings”为 与statement相关的所有“firings”的“symmetric fired unit”的总和。 |
| **DR-65 Holds Before Chain** | 当以下至少一项成立:标志“in effect”已设置、count of inverse firings大于0,或count of symmetric firings大于0时,statement被视为满足“Before Chain”。 |
| **DR-66 Count of Chain Firings** | statement的“count of chain firings”为 与statement相关的所有“firings”的“chain fired unit”的总和。 |
| **DR-67 Holds Before Typing** | 当以下至少一项成立:标志“holds before chain”已设置或count of chain firings大于0时,statement被视为满足“Before Typing”。 |
| **DR-68 Count of Type Firings** | statement的“count of type firings”为 与statement相关的所有“firings”的“type fired unit”的总和。 |
| **DR-69 Holds** | 当以下至少一项成立:标志“holds before typing”已设置或count of type firings大于0时,statement被视为满足“”。 |
| **DR-70 Is Conclusion** | 当以下全部成立:标志“holds”已设置且标志“in effect”未设置时,statement被视为“Conclusion”。 |
| **DR-71 Holds Unit** | Statement的“holds unit”按以下优先级确定:<br>1. 当标志“holds”已设置时,取 1;<br>2. 其余情况取 0。 |
| **DR-72 Conclusion Unit** | Statement的“conclusion unit”按以下优先级确定:<br>1. 当标志“conclusion”已设置时,取 1;<br>2. 其余情况取 0。 |
| **DR-73 Parent Unit** | Statement的“parent unit”按以下优先级确定:<br>1. 当以下全部成立:标志“holds”已设置且property为“isParentOf”时,取 1;<br>2. 其余情况取 0。 |
| **DR-74 Grandparent Unit** | Statement的“grandparent unit”按以下优先级确定:<br>1. 当以下全部成立:标志“holds”已设置且property为“isGrandparentOf”时,取 1;<br>2. 其余情况取 0。 |
| **DR-75 Place Unit** | Statement的“place unit”按以下优先级确定:<br>1. 当以下全部成立:标志“holds”已设置且property为“attends”时,取 1;<br>2. 其余情况取 0。 |
| **DR-76 First Pre Chain Order Key** | statement的“first pre chain order key”为 statement的“firings”中是“Pre Chain Fired Unit”的“pre chain order key”的最小值。 |
| **DR-77 Snap Position** | Statement的“snap position”按以下优先级确定:<br>1. 当标志“in effect”已设置时,取 sort order;<br>2. 当标志“holds before chain”已设置时,取 100000加first pre chain order key;<br>3. 其余情况取 0。 |
| **DR-78 First Fired Order Key** | statement的“first fired order key”为 statement的“firings”中是“Fired Unit”的“order key”的最小值。 |
| **DR-79 Count of Chosen Firings** | statement的“count of chosen firings”为 与statement相关的所有“firings”的“chosen unit”的总和。 |
| **DR-80 Subject Node Key** | statement的“subject node key”与 subject 相同。 |
| **DR-81 Object Node Key** | Statement的“object node key”按以下优先级确定:<br>1. 当object thing为空时,取 “literal|”,接着是object value,接着是“|”,接着是property datatype;<br>2. 其余情况取 object thing。 |
| **DR-82 Subject Node Kind** | statement的“subject node kind”——取自所关联的“subject”。 |
| **DR-83 Object Thing Node Kind** | statement的“object thing node kind”——取自所关联的“object thing”。 |
| **DR-84 Object Node Kind** | Statement的“object node kind”按以下优先级确定:<br>1. 当object thing为空时,取 “value”;<br>2. 其余情况取 object thing node kind。 |
| **DR-85 Object Value Node Label** | statement的“object value node label”——取自所关联的“property”。 |
| **DR-86 Count of Violations As First** | statement的“count of violations as first”为 与statement相关的所有“disjoint checks”的“violation unit”的总和。 |
| **DR-87 Count of Violations As Second** | statement的“count of violations as second”为 与statement相关的所有“disjoint checks”的“violation unit”的总和。 |
| **DR-88 Is in Contradiction** | 当以下至少一项成立:count of violations as first大于0或count of violations as second大于0时,statement被视为“In Contradiction”。 |
| **DR-89 Rule Enabled** | firing的“rule enabled”在所关联的“rule”是“Enabled”时为真。 |
| **DR-90 Kind Wording** | firing的“kind wording”——取自所关联的“kind”。 |
| **DR-91 Kind Sort Order** | firing的“kind sort order”——取自所关联的“kind”。 |
| **DR-92 Target Name** | firing的“target name”——取自所关联的“target”。 |
| **DR-93 Target Subject Label** | firing的“target subject label”——取自所关联的“target”。 |
| **DR-94 Target Property Label** | firing的“target property label”——取自所关联的“target”。 |
| **DR-95 Target Object Text** | firing的“target object text”——取自所关联的“target”。 |
| **DR-96 Target is Conclusion** | firing的“target is conclusion”在所关联的“target”是“Conclusion”时为真。 |
| **DR-97 Target Subject Node Key** | firing的“target subject node key”——取自所关联的“target”。 |
| **DR-98 Target Object Node Key** | firing的“target object node key”——取自所关联的“target”。 |
| **DR-99 Target First Fired Order Key** | firing的“target first fired order key”——取自所关联的“target”。 |
| **DR-100 First Words** | firing的“first words”——取自所关联的“first”。 |
| **DR-101 Second Words** | firing的“second words”——取自所关联的“second”。 |
| **DR-102 First is in Effect** | firing的“first is in effect”在所关联的“first”是“In Effect”时为真。 |
| **DR-103 First Holds Before Chain** | firing的“first holds before chain”在所关联的“first”是“Holds Before Chain”时为真。 |
| **DR-104 Second Holds Before Chain** | firing的“second holds before chain”在所关联的“second”是“Holds Before Chain”时为真。 |
| **DR-105 First Holds Before Typing** | firing的“first holds before typing”在所关联的“first”是“Holds Before Typing”时为真。 |
| **DR-106 First Property** | firing的“first property”——取自所关联的“first”。 |
| **DR-107 First Property Domain Class** | firing的“first property domain class”——取自所关联的“first”。 |
| **DR-108 First Property Range Class** | firing的“first property range class”——取自所关联的“first”。 |
| **DR-109 First Sort Order** | firing的“first sort order”——取自所关联的“first”。 |
| **DR-110 Second is in Effect** | firing的“second is in effect”在所关联的“second”是“In Effect”时为真。 |
| **DR-111 First Snap Position** | firing的“first snap position”——取自所关联的“first”。 |
| **DR-112 Second Snap Position** | firing的“second snap position”——取自所关联的“second”。 |
| **DR-113 Inverse Fired Unit** | Firing的“inverse fired unit”按以下优先级确定:<br>1. 当以下全部成立:标志“rule enabled”已设置、kind为“inverse”,且标志“first is in effect”已设置时,取 1;<br>2. 其余情况取 0。 |
| **DR-114 Symmetric Fired Unit** | Firing的“symmetric fired unit”按以下优先级确定:<br>1. 当以下全部成立:标志“rule enabled”已设置、kind为“symmetric”,且标志“first is in effect”已设置时,取 1;<br>2. 其余情况取 0。 |
| **DR-115 Chain Fired Unit** | Firing的“chain fired unit”按以下优先级确定:<br>1. 当以下全部成立:标志“rule enabled”已设置、kind为“chain”、标志“first holds before chain”已设置,且标志“second holds before chain”已设置时,取 1;<br>2. 其余情况取 0。 |
| **DR-116 Type Fired Unit** | Firing的“type fired unit”按以下优先级确定:<br>1. 当以下全部成立:标志“rule enabled”已设置、以下至少一项成立:kind为“domain”或kind为“range”,且标志“first holds before typing”已设置时,取 1;<br>2. 其余情况取 0。 |
| **DR-117 Fired Unit** | firing的“fired unit”按 inverse fired unit加symmetric fired unit加chain fired unit加type fired unit 计算。 |
| **DR-118 Pre Chain Fired Unit** | firing的“pre chain fired unit”按 inverse fired unit加symmetric fired unit 计算。 |
| **DR-119 Pre Chain Order Key** | firing的“pre chain order key”按 first sort order乘10加kind sort order 计算。 |
| **DR-120 Round** | Firing的“round”按以下优先级确定:<br>1. 当以下全部成立:kind为“chain”且并非以下全部成立:标志“first is in effect”已设置且标志“second is in effect”已设置时,取 2;<br>2. 其余情况取 1。 |
| **DR-121 Order Key** | firing的“order key”按 round乘1000000000000000加first snap position乘100000000加kind sort order乘10000000加当second为空时为 0,其余情况为 second snap position 计算。 |
| **DR-122 Is Chosen** | 当以下全部成立:fired unit为1、标志“target is conclusion”已设置,且order key为target first fired order key时,firing被视为“Chosen”。 |
| **DR-123 Chosen Unit** | Firing的“chosen unit”按以下优先级确定:<br>1. 当标志“chosen”已设置时,取 1;<br>2. 其余情况取 0。 |
| **DR-124 Card Text** | Firing的“card text”按以下优先级确定:<br>1. 当标志“chosen”已设置时,取 “Worked out by the ”,接着是kind wording,接着是“ from “”,接着是first words,接着是“””,接着是当kind为“chain”时为 “ and “”,接着是second words,接着是“””,其余情况为 当kind为“domain”时为 “ and “”,接着是first property,接着是“ has domain ”,接着是first property domain class,接着是“””,其余情况为 “ and “”,接着是first property,接着是“ has range ”,接着是first property range class,接着是“””,接着是一个句点;<br>2. 其余情况取 空字符串。 ⚠︎ 机械 <!-- rulespeak:reword --> |
| **DR-125 Name** | firing的“name”与 firing ID 相同。 |
| **DR-126 Rule Enabled** | disjoint check的“rule enabled”在所关联的“rule”是“Enabled”时为真。 |
| **DR-127 First Holds** | disjoint check的“first holds”在所关联的“first”是“Holds”时为真。 |
| **DR-128 Second Holds** | disjoint check的“second holds”在所关联的“second”是“Holds”时为真。 |
| **DR-129 Subject Label** | disjoint check的“subject label”——取自所关联的“first”。 |
| **DR-130 Object Label** | disjoint check的“object label”——取自所关联的“first”。 |
| **DR-131 Violation Unit** | Disjoint check的“violation unit”按以下优先级确定:<br>1. 当以下全部成立:标志“rule enabled”已设置、标志“first holds”已设置,且标志“second holds”已设置时,取 1;<br>2. 其余情况取 0。 |
| **DR-132 Alarm Text** | Disjoint check的“alarm text”按以下优先级确定:<br>1. 当violation unit为1时,取 “Contradiction found. ”,接着是subject label,接着是“ is recorded as both parent and spouse of ”,接着是object label,接着是“. Check those facts and the selected disjoint rule.”;<br>2. 其余情况取 空字符串。 |
| **DR-133 Name** | disjoint check的“name”与 disjoint check ID 相同。 |
| **DR-134 Name** | scenario的“name”与 label 相同。 |
| **DR-135 Graph Has Facts to Add** | scenario的“graph has facts to add”在所关联的“graph”具有“Facts to Add”时为真。 |
| **DR-136 Is Available** | 当以下至少一项成立:fact baseline不为“next”或标志“graph has facts to add”已设置时,scenario被视为“Available”。 |
| **DR-137 Name** | scenario fact change的“name”与 scenario fact change ID 相同。 |
| **DR-138 Name** | scenario rule setting的“name”与 scenario rule setting ID 相同。 |
| **DR-139 Name** | node layout的“name”与 node layout ID 相同。 |
| **DR-140 Name** | layout slot的“name”与 layout slot ID 相同。 |
| **DR-141 Name** | page text的“name”与 page text ID 相同。 |
| **DR-142 Name** | engine的“name”与 label 相同。 |
| **DR-143 Name** | rulebook tool的“name”与 label 相同。 |

## 5 到模式的可追溯性

_表达式列是规则在 文档 记法中的定义——
与规则手册存储的逻辑相同,只是为业务读者书写。_

| 模式元素 | 类别 | 表达式 |
|----------------|------|------------|
| **Graphs.Name** | 公式 | `Label` |
| **Graphs.CountOfStatedFacts** | 汇总 | `Sum(Statements.InEffectUnit via Graph)` |
| **Graphs.CountOfConclusions** | 汇总 | `Sum(Statements.ConclusionUnit via Graph)` |
| **Graphs.CountOfViolations** | 汇总 | `Sum(DisjointChecks.ViolationUnit via Graph)` |
| **Graphs.IsConsistent** | 公式 | `CountOfViolations = 0` |
| **Graphs.ConclusionsMessage** | 公式 | `If(CountOfConclusions = 0, NoConclusionsText, "")` |
| **Graphs.CountOfFactsToAdd** | 汇总 | `Sum(Statements.ListedOffUnit via Graph)` |
| **Graphs.HasFactsToAdd** | 公式 | `CountOfFactsToAdd > 0` |
| **Graphs.NextFactSortOrder** | 汇总 | `Min(Statements.SortOrder via Graph)` |
| **Graphs.MaxActivationSeq** | 汇总 | `Max(Statements.ActivationSeq via Graph)` |
| **Graphs.NextActivationSeq** | 公式 | `MaxActivationSeq + 1` |
| **Graphs.MaxListedSortOrder** | 汇总 | `Max(Statements.SortOrder via Graph)` |
| **Graphs.NextListedSortOrder** | 公式 | `MaxListedSortOrder + 1` |
| **Graphs.MaxSortOrder** | 汇总 | `Max(Statements.SortOrder via Graph)` |
| **Graphs.NextCandidateSortOrder** | 公式 | `MaxSortOrder + 1` |
| **Rules.Name** | 公式 | `Label` |
| **Rules.CountOfFirings** | 汇总 | `Sum(Firings.FiredUnit via Rule)` |
| **FiringKinds.Name** | 公式 | `Wording` |
| **FiringKinds.RuleEnabled** | 查找 | `Lookup(Rules.IsEnabled via Rule)` |
| **Things.PortraitName** | 查找 | `Lookup(Portraits.DisplayName via Portrait)` |
| **Things.PortraitPage** | 查找 | `Lookup(Portraits.Page via Portrait)` |
| **Things.PortraitImage** | 查找 | `Lookup(Portraits.Image via Portrait)` |
| **Things.PortraitCrop** | 查找 | `Lookup(Portraits.Crop via Portrait)` |
| **Things.PortraitWidth** | 查找 | `Lookup(Portraits.Width via Portrait)` |
| **Things.PortraitHeight** | 查找 | `Lookup(Portraits.Height via Portrait)` |
| **Things.Name** | 公式 | `Label` |
| **Things.NodeKey** | 公式 | `ThingId` |
| **Things.CountOfChildren** | 汇总 | `Sum(Statements.ParentUnit via Subject)` |
| **Things.CountOfGrandchildren** | 汇总 | `Sum(Statements.GrandparentUnit via Subject)` |
| **Things.CountAsPlace** | 汇总 | `Sum(Statements.PlaceUnit via ObjectThing)` |
| **Things.CountAsSubject** | 汇总 | `Sum(Statements.HoldsUnit via Subject)` |
| **Things.CountAsObject** | 汇总 | `Sum(Statements.HoldsUnit via ObjectThing)` |
| **Things.IsInGraph** | 公式 | `Or(CountAsSubject > 0, CountAsObject > 0)` |
| **Things.NodeKind** | 公式 | `If(Kind = "", "", If(Kind = "class", "class", If(CountAsPlace > 0, "place", "person")))` |
| **Portraits.Name** | 公式 | `DisplayName` |
| **PortraitAliases.Name** | 公式 | `PortraitAliasId` |
| **Properties.Name** | 公式 | `Label` |
| **PropertyAxioms.Name** | 公式 | `PropertyAxiomId` |
| **Statements.SubjectLabel** | 查找 | `Lookup(Things.Label via Subject)` |
| **Statements.PropertyLabel** | 查找 | `Lookup(Properties.Label via Property)` |
| **Statements.ObjectLabel** | 查找 | `Lookup(Things.Label via ObjectThing)` |
| **Statements.PropertyDatatype** | 查找 | `Lookup(Properties.Datatype via Property)` |
| **Statements.PropertyHasOneCurrentValue** | 查找 | `Lookup(Properties.HasOneCurrentValue via Property)` |
| **Statements.PropertyDomainClass** | 查找 | `Lookup(Properties.DomainClass via Property)` |
| **Statements.PropertyRangeClass** | 查找 | `Lookup(Properties.RangeClass via Property)` |
| **Statements.LiteralQuote** | 查找 | `Lookup(Graphs.LiteralQuote via Graph)` |
| **Statements.LiteralTypeMarker** | 查找 | `Lookup(Graphs.LiteralTypeMarker via Graph)` |
| **Statements.BuiltInSourceText** | 查找 | `Lookup(Graphs.BuiltInSourceText via Graph)` |
| **Statements.UserSourceText** | 查找 | `Lookup(Graphs.UserSourceText via Graph)` |
| **Statements.NextFactSortOrder** | 查找 | `Lookup(Graphs.NextFactSortOrder via Graph)` |
| **Statements.IsLiteral** | 公式 | `ObjectThing = ""` |
| **Statements.ObjectText** | 公式 | `If(ObjectThing = "", ObjectValue, ObjectLabel)` |
| **Statements.Name** | 公式 | `Concat(SubjectLabel, " ", PropertyLabel, " ", ObjectText)` |
| **Statements.Words** | 公式 | `Concat(SubjectLabel, " ", PropertyLabel, " ", If(ObjectThing = "", Concat(LiteralQuote, ObjectValue, LiteralQuote, LiteralTypeMarker, PropertyDatatype), ObjectLabel))` |
| **Statements.Source** | 公式 | `If(IsBuiltIn, BuiltInSourceText, UserSourceText)` |
| **Statements.SubjectPropertyKey** | 公式 | `Concat(Subject, "\|", Property)` |
| **Statements.LatestSiblingActivationSeq** | 汇总 | `Max(Statements.ActivationSeq via SubjectPropertyKey)` |
| **Statements.IsInEffect** | 公式 | `And(IsStated, Or(Not(PropertyHasOneCurrentValue), ActivationSeq = LatestSiblingActivationSeq))` |
| **Statements.ListedUnit** | 公式 | `If(IsListed, 1, 0)` |
| **Statements.InEffectUnit** | 公式 | `If(IsInEffect, 1, 0)` |
| **Statements.ListedOffUnit** | 公式 | `If(And(IsListed, Not(IsInEffect)), 1, 0)` |
| **Statements.IsNextToAdd** | 公式 | `And(ListedOffUnit = 1, SortOrder = NextFactSortOrder)` |
| **Statements.CountOfInverseFirings** | 汇总 | `Sum(Firings.InverseFiredUnit via Target)` |
| **Statements.CountOfSymmetricFirings** | 汇总 | `Sum(Firings.SymmetricFiredUnit via Target)` |
| **Statements.HoldsBeforeChain** | 公式 | `Or(IsInEffect, CountOfInverseFirings > 0, CountOfSymmetricFirings > 0)` |
| **Statements.CountOfChainFirings** | 汇总 | `Sum(Firings.ChainFiredUnit via Target)` |
| **Statements.HoldsBeforeTyping** | 公式 | `Or(HoldsBeforeChain, CountOfChainFirings > 0)` |
| **Statements.CountOfTypeFirings** | 汇总 | `Sum(Firings.TypeFiredUnit via Target)` |
| **Statements.Holds** | 公式 | `Or(HoldsBeforeTyping, CountOfTypeFirings > 0)` |
| **Statements.IsConclusion** | 公式 | `And(Holds, Not(IsInEffect))` |
| **Statements.HoldsUnit** | 公式 | `If(Holds, 1, 0)` |
| **Statements.ConclusionUnit** | 公式 | `If(IsConclusion, 1, 0)` |
| **Statements.ParentUnit** | 公式 | `If(And(Holds, Property = "isParentOf"), 1, 0)` |
| **Statements.GrandparentUnit** | 公式 | `If(And(Holds, Property = "isGrandparentOf"), 1, 0)` |
| **Statements.PlaceUnit** | 公式 | `If(And(Holds, Property = "attends"), 1, 0)` |
| **Statements.FirstPreChainOrderKey** | 汇总 | `Min(Firings.PreChainOrderKey via Target)` |
| **Statements.SnapPosition** | 公式 | `If(IsInEffect, SortOrder, If(HoldsBeforeChain, 100000 + FirstPreChainOrderKey, 0))` |
| **Statements.FirstFiredOrderKey** | 汇总 | `Min(Firings.OrderKey via Target)` |
| **Statements.CountOfChosenFirings** | 汇总 | `Sum(Firings.ChosenUnit via Target)` |
| **Statements.SubjectNodeKey** | 公式 | `Subject` |
| **Statements.ObjectNodeKey** | 公式 | `If(ObjectThing = "", Concat("literal\|", ObjectValue, "\|", PropertyDatatype), ObjectThing)` |
| **Statements.SubjectNodeKind** | 查找 | `Lookup(Things.NodeKind via Subject)` |
| **Statements.ObjectThingNodeKind** | 查找 | `Lookup(Things.NodeKind via ObjectThing)` |
| **Statements.ObjectNodeKind** | 公式 | `If(ObjectThing = "", "value", ObjectThingNodeKind)` |
| **Statements.ObjectValueNodeLabel** | 查找 | `Lookup(Properties.ValueNodeLabel via Property)` |
| **Statements.CountOfViolationsAsFirst** | 汇总 | `Sum(DisjointChecks.ViolationUnit via First)` |
| **Statements.CountOfViolationsAsSecond** | 汇总 | `Sum(DisjointChecks.ViolationUnit via Second)` |
| **Statements.IsInContradiction** | 公式 | `Or(CountOfViolationsAsFirst > 0, CountOfViolationsAsSecond > 0)` |
| **Firings.RuleEnabled** | 查找 | `Lookup(Rules.IsEnabled via Rule)` |
| **Firings.KindWording** | 查找 | `Lookup(FiringKinds.Wording via Kind)` |
| **Firings.KindSortOrder** | 查找 | `Lookup(FiringKinds.SortOrder via Kind)` |
| **Firings.TargetName** | 查找 | `Lookup(Statements.Name via Target)` |
| **Firings.TargetSubjectLabel** | 查找 | `Lookup(Statements.SubjectLabel via Target)` |
| **Firings.TargetPropertyLabel** | 查找 | `Lookup(Statements.PropertyLabel via Target)` |
| **Firings.TargetObjectText** | 查找 | `Lookup(Statements.ObjectText via Target)` |
| **Firings.TargetIsConclusion** | 查找 | `Lookup(Statements.IsConclusion via Target)` |
| **Firings.TargetSubjectNodeKey** | 查找 | `Lookup(Statements.SubjectNodeKey via Target)` |
| **Firings.TargetObjectNodeKey** | 查找 | `Lookup(Statements.ObjectNodeKey via Target)` |
| **Firings.TargetFirstFiredOrderKey** | 查找 | `Lookup(Statements.FirstFiredOrderKey via Target)` |
| **Firings.FirstWords** | 查找 | `Lookup(Statements.Words via First)` |
| **Firings.SecondWords** | 查找 | `Lookup(Statements.Words via Second)` |
| **Firings.FirstIsInEffect** | 查找 | `Lookup(Statements.IsInEffect via First)` |
| **Firings.FirstHoldsBeforeChain** | 查找 | `Lookup(Statements.HoldsBeforeChain via First)` |
| **Firings.SecondHoldsBeforeChain** | 查找 | `Lookup(Statements.HoldsBeforeChain via Second)` |
| **Firings.FirstHoldsBeforeTyping** | 查找 | `Lookup(Statements.HoldsBeforeTyping via First)` |
| **Firings.FirstProperty** | 查找 | `Lookup(Statements.Property via First)` |
| **Firings.FirstPropertyDomainClass** | 查找 | `Lookup(Statements.PropertyDomainClass via First)` |
| **Firings.FirstPropertyRangeClass** | 查找 | `Lookup(Statements.PropertyRangeClass via First)` |
| **Firings.FirstSortOrder** | 查找 | `Lookup(Statements.SortOrder via First)` |
| **Firings.SecondIsInEffect** | 查找 | `Lookup(Statements.IsInEffect via Second)` |
| **Firings.FirstSnapPosition** | 查找 | `Lookup(Statements.SnapPosition via First)` |
| **Firings.SecondSnapPosition** | 查找 | `Lookup(Statements.SnapPosition via Second)` |
| **Firings.InverseFiredUnit** | 公式 | `If(And(RuleEnabled, Kind = "inverse", FirstIsInEffect), 1, 0)` |
| **Firings.SymmetricFiredUnit** | 公式 | `If(And(RuleEnabled, Kind = "symmetric", FirstIsInEffect), 1, 0)` |
| **Firings.ChainFiredUnit** | 公式 | `If(And(RuleEnabled, Kind = "chain", FirstHoldsBeforeChain, SecondHoldsBeforeChain), 1, 0)` |
| **Firings.TypeFiredUnit** | 公式 | `If(And(RuleEnabled, Or(Kind = "domain", Kind = "range"), FirstHoldsBeforeTyping), 1, 0)` |
| **Firings.FiredUnit** | 公式 | `InverseFiredUnit + SymmetricFiredUnit + ChainFiredUnit + TypeFiredUnit` |
| **Firings.PreChainFiredUnit** | 公式 | `InverseFiredUnit + SymmetricFiredUnit` |
| **Firings.PreChainOrderKey** | 公式 | `FirstSortOrder * 10 + KindSortOrder` |
| **Firings.Round** | 公式 | `If(And(Kind = "chain", Not(And(FirstIsInEffect, SecondIsInEffect))), 2, 1)` |
| **Firings.OrderKey** | 公式 | `Round * 1000000000000000 + FirstSnapPosition * 100000000 + KindSortOrder * 10000000 + If(Second = "", 0, SecondSnapPosition)` |
| **Firings.IsChosen** | 公式 | `And(FiredUnit = 1, TargetIsConclusion, OrderKey = TargetFirstFiredOrderKey)` |
| **Firings.ChosenUnit** | 公式 | `If(IsChosen, 1, 0)` |
| **Firings.CardText** | 公式 | `If(IsChosen, Concat("Worked out by the ", KindWording, " from “", FirstWords, "”", If(Kind = "chain", Concat(" and “", SecondWords, "”"), If(Kind = "domain", Concat(" and “", FirstProperty, " has domain ", FirstPropertyDomainClass, "”"), If(Kind = "range", Concat(" and “", FirstProperty, " has range ", FirstPropertyRangeClass, "”"), ""))), "."), "")` |
| **Firings.Name** | 公式 | `FiringId` |
| **DisjointChecks.RuleEnabled** | 查找 | `Lookup(Rules.IsEnabled via Rule)` |
| **DisjointChecks.FirstHolds** | 查找 | `Lookup(Statements.Holds via First)` |
| **DisjointChecks.SecondHolds** | 查找 | `Lookup(Statements.Holds via Second)` |
| **DisjointChecks.SubjectLabel** | 查找 | `Lookup(Statements.SubjectLabel via First)` |
| **DisjointChecks.ObjectLabel** | 查找 | `Lookup(Statements.ObjectLabel via First)` |
| **DisjointChecks.ViolationUnit** | 公式 | `If(And(RuleEnabled, FirstHolds, SecondHolds), 1, 0)` |
| **DisjointChecks.AlarmText** | 公式 | `If(ViolationUnit = 1, Concat("Contradiction found. ", SubjectLabel, " is recorded as both parent and spouse of ", ObjectLabel, ". Check those facts and the selected disjoint rule."), "")` |
| **DisjointChecks.Name** | 公式 | `DisjointCheckId` |
| **Scenarios.Name** | 公式 | `Label` |
| **Scenarios.GraphHasFactsToAdd** | 查找 | `Lookup(Graphs.HasFactsToAdd via Graph)` |
| **Scenarios.IsAvailable** | 公式 | `Or(FactBaseline <> "next", GraphHasFactsToAdd)` |
| **ScenarioFactChanges.Name** | 公式 | `ScenarioFactChangeId` |
| **ScenarioRuleSettings.Name** | 公式 | `ScenarioRuleSettingId` |
| **NodeLayouts.Name** | 公式 | `NodeLayoutId` |
| **LayoutSlots.Name** | 公式 | `LayoutSlotId` |
| **PageTexts.Name** | 公式 | `PageTextId` |
| **Engines.Name** | 公式 | `Label` |
| **RulebookTools.Name** | 公式 | `Label` |

---

> **RuleSpeak®**——由 **Ronald G. Ross** 创立、遵循 **SBVR** 约定的声明式业务规则表示法——即将推出。在此之前，本页显示常规文档。 [www.RonRoss.info](https://www.RonRoss.info)
