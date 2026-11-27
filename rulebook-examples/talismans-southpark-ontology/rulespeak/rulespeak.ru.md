# 📘 Talisman's South Park ontology explainer — Документация

_Jessica Talisman's South Park Ontology Explainer, as a rulebook: the page's ten facts, five rule switches, property wiring, provenance wording and current-value policy are rows, and every conclusion is a formula. The rules apply in three fixed strata (inverse and symmetric, then the property chain, then domain and range); property wiring that feeds an earlier stratum would need repeated rounds. Every fact that could hold over the cast is a candidate row; app/lib/candidates.mjs writes the candidates when a thing, value or property is added._

> Декларативные бизнес-правила, построенные из книги правил. Каждое утверждение
> выражает истину предметной области — это не процедура и не императив.
> Формулы книги правил — единственный источник истины; этот документ — их
> прочтение на простом языке.

## 1 Бизнес-словарь

| Термин | Описание | Пояснение |
|------|-------------|-------------------|
| **Graph** | The explainer page as one graph: its wording variables and its totals. | — |
| Label | Определённый атрибут. | _Display name of the graph._ |
| Literal Quote | Определённый атрибут. | _The quote mark written around a literal value in a fact's wording._ |
| Literal Type Marker | Определённый атрибут. | _The marker written between a literal value and its datatype, as in Turtle._ |
| Built in Source Text | Определённый атрибут. | _Source line shown for a fact the page states as a teaching example._ |
| User Source Text | Определённый атрибут. | _Source line shown for a fact the reader stated._ |
| No Conclusions Text | Определённый атрибут. | _Message shown when the selected facts and rules add no conclusion._ |
| Name | Совпадает с label. | _Order 1. Display name of the graph._ |
| Count of Stated Facts | Сумма «in effect unit» по записям «statements», связанным с graph. | _Order 5. Facts that are switched on and in effect._ |
| Count of Conclusions | Сумма «conclusion unit» по записям «statements», связанным с graph. | _Order 18. Facts that hold only because an enabled rule concluded them._ |
| Count of Violations | Сумма «violation unit» по записям «disjoint checks», связанным с graph. | _Order 18. Disjointness checks that found both links between the same pair._ |
| Is Consistent | Истинно, когда count of violations равно 0. | _Order 19. True when no enabled disjointness rule is violated._ |
| Conclusions Message | Определяется по приоритету: no conclusions text, если count of conclusions равно 0; во всех остальных случаях — пустая строка. | _Order 19. The empty-state message for the conclusions panel, blank when there are conclusions._ |
| Count of Facts to Add | Сумма «listed off unit» по записям «statements», связанным с graph. | _Order 5. Listed facts that are not in effect; the Add next fact button is enabled while this is above zero._ |
| Has Facts to Add | Истинно, когда count of facts to add больше 0. | _Order 6. True while some listed fact can still be added._ |
| Next Fact Sort Order | Наименьшее значение «sort order» по записям «statements» (graph), которые являются «listed off unit». | _Order 5. List position of the first listed fact that is not in effect._ |
| Max Activation Seq | Наибольшее значение «activation seq» по записям «statements», связанным с graph. | _Order 1. Highest activation number used so far._ |
| Next Activation Seq | Вычисляется как max activation seq плюс 1. | _Order 2. The activation number the next switched-on fact takes._ |
| Max Listed Sort Order | Наибольшее значение «sort order» по записям «statements» (graph), которые являются «listed unit». | _Order 2. Highest list position among listed facts._ |
| Next Listed Sort Order | Вычисляется как max listed sort order плюс 1. | _Order 3. The list position a newly added fact takes._ |
| Max Sort Order | Наибольшее значение «sort order» по записям «statements», связанным с graph. | _Order 1. Highest sort order of any fact, listed or candidate._ |
| Next Candidate Sort Order | Вычисляется как max sort order плюс 1. | _Order 2. The sort order a new candidate fact, or a fact removed from the list, takes._ |

<details open>
<summary>Примеры данных</summary>

| Label | Literal Quote | Literal Type Marker | Built in Source Text | User Source Text | No Conclusions Text | Name ƒ | Count of Stated Facts ƒ | Count of Conclusions ƒ | Count of Violations ƒ | Is Consistent ƒ | Conclusions Message ƒ | Count of Facts to Add ƒ | Has Facts to Add ƒ | Next Fact Sort Order ƒ | Max Activation Seq ƒ | Next Activation Seq ƒ | Max Listed Sort Order ƒ | Next Listed Sort Order ƒ | Max Sort Order ƒ | Next Candidate Sort Order ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| South Park | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | These facts and selected rules add no further conclusions. Try another fact or rule. | South Park | 10 | 0 | 0 | true | These facts and selected rules add no further conclusions. Try another fact or rule. | 0 | false | — | 10 | 11 | 10 | 11 | 100269 | 100270 |

_ƒ обозначает вычисляемый столбец._

</details>

| Термин | Описание | Пояснение |
|------|-------------|-------------------|
| **Rule** | The five rule switches on the page. | — |
| Label | Определённый атрибут. | _The rule's name on the page._ |
| Explanation | Определённый атрибут. | _The rule's one-line explanation on the page._ |
| Is Enabled | Истинно, когда пустая строка. | _The rule's checkbox._ |
| Sort Order | Определённый атрибут. | _Position in the rule list._ |
| Name | Совпадает с label. | _Order 1. The rule's name._ |
| Count of Firings | Сумма «fired unit» по записям «firings», связанным с rule. | _Order 15. Rule applications that fired, counting every derivation._ |

<details open>
<summary>Примеры данных</summary>

| Label | Explanation | Is Enabled | Sort Order | Name ƒ | Count of Firings ƒ |
|---|---|---|---|---|---|
| Inverse | A parent link also gives a child link between the same two people. | false | 1 | Inverse | 0 |
| Symmetric | A friend link or a spouse link is true for both people. | false | 2 | Symmetric | 0 |
| Property chain | A parent of a parent is a grandparent. | false | 3 | Property chain | 0 |

_ƒ обозначает вычисляемый столбец._

</details>

| Термин | Описание | Пояснение |
|------|-------------|-------------------|
| **Firing Kind** | The ways a rule can apply. Domain and range belong to one rule but are worded apart; 'disjoint' names the rule the disjointness checks answer to. | — |
| Rule | Определённый атрибут. | _The rule switch this kind belongs to._ |
| Wording | Определённый атрибут. | _How a conclusion card names the rule, as in 'Worked out by the inverse rule'._ |
| Sort Order | Определённый атрибут. | _Order in which the page's reasoner tries the kinds for one fact._ |
| Name | Совпадает с wording. | _Order 1. The kind's wording._ |
| Rule Enabled | Истинно, когда связанный «rule» является «enabled». | _Order 1. Whether the owning rule is switched on._ |

<details open>
<summary>Примеры данных</summary>

| Wording | Sort Order | Name ƒ | Rule Enabled ƒ |
|---|---|---|---|
| inverse rule | 1 | inverse rule | false |
| symmetric rule | 2 | symmetric rule | false |
| property chain rule | 3 | property chain rule | false |

_ƒ обозначает вычисляемый столбец._

</details>

| Термин | Описание | Пояснение |
|------|-------------|-------------------|
| **Thing** | Every individual and class the page can draw. | — |
| Label | Определённый атрибут. | _The name the page shows._ |
| Kind | Определённый атрибут. | _'individual' or 'class'._ |
| Sort Order | Определённый атрибут. | _Position in the cast._ |
| Portrait | Определённый атрибут. | _The portrait drawn for this thing; set when its name matches a portrait alias._ |
| Portrait Name | «display name» связанного «portrait». | _Order 1. The name on the portrait's Fandom page._ |
| Portrait Page | Берётся из связанного «portrait». | _Order 1. The Fandom page credited for the portrait._ |
| Portrait Image | Берётся из связанного «portrait». | _Order 1. The original Fandom image._ |
| Portrait Crop | Берётся из связанного «portrait». | _Order 1. The portrait's crop as an SVG viewBox._ |
| Portrait Width | Берётся из связанного «portrait». | _Order 1. The original image's width in pixels._ |
| Portrait Height | Берётся из связанного «portrait». | _Order 1. The original image's height in pixels._ |
| Name | Совпадает с label. | _Order 1. The thing's name._ |
| Node Key | Совпадает с thing ID. | _Order 1. The key the graph draws this thing under._ |
| Count of Children | Сумма «parent unit» по записям «statements», связанным с thing. | _Order 17. People this thing is a parent of, stated or concluded._ |
| Count of Grandchildren | Сумма «grandparent unit» по записям «statements», связанным с thing. | _Order 17. People this thing is a grandparent of._ |
| Count As Place | Сумма «place unit» по записям «statements», связанным с thing. | _Order 17. Holding attends facts whose object is this thing._ |
| Count As Subject | Сумма «holds unit» по записям «statements», связанным с thing. | _Order 17. Holding facts about this thing._ |
| Count As Object | Сумма «holds unit» по записям «statements», связанным с thing. | _Order 17. Holding facts that point at this thing._ |
| Is in Graph | Истинно, когда выполняется хотя бы одно из следующего: count as subject больше 0 или count as object больше 0. | _Order 18. True when the graph draws this thing._ |
| Node Kind | Определяется по приоритету: пустая строка, если kind пусто; «class», если kind равно «class»; «place», если count as place больше 0; во всех остальных случаях — «person». | _Order 18. How the graph draws the thing: a class, a place someone attends, or a person._ |

<details open>
<summary>Примеры данных</summary>

| Label | Kind | Sort Order | Portrait Name ƒ | Portrait Page ƒ | Portrait Image ƒ | Portrait Crop ƒ | Portrait Width ƒ | Portrait Height ƒ | Name ƒ | Node Key ƒ | Count of Children ƒ | Count of Grandchildren ƒ | Count As Place ƒ | Count As Subject ƒ | Count As Object ƒ | Is in Graph ƒ | Node Kind ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| Marvin | individual | 1 | Marvin Marsh | https://southpark.fandom.com/wiki/Marvin_Marsh | https://static.wikia.nocookie.net/southpark/images/d/db/Marsh-family-grandpa-marsh-w-purple-blanket.png/revision/latest | 48 -5 280 280 | 375 | 536 | Marvin | Marvin | 1 | 0 | 0 | 1 | 0 | true | person |
| Randy | individual | 2 | Randy Marsh | https://southpark.fandom.com/wiki/Randy_Marsh | https://static.wikia.nocookie.net/southparkfanon/images/d/d6/Randy_Marsh.png/revision/latest | 0 -10 428 428 | 428 | 1057 | Randy | Randy | 2 | 0 | 0 | 3 | 1 | true | person |
| Sharon | individual | 3 | Sharon Marsh | https://southpark.fandom.com/wiki/Sharon_Marsh | https://static.wikia.nocookie.net/southpark/images/9/99/Sharon_Marsh.png/revision/latest | 20 -10 390 390 | 439 | 1049 | Sharon | Sharon | 0 | 0 | 0 | 0 | 1 | true | person |

_ƒ обозначает вычисляемый столбец._

</details>

| Термин | Описание | Пояснение |
|------|-------------|-------------------|
| **Portrait** | The page's Fandom portraits, including those for characters a reader may add. | — |
| Display Name | Определённый атрибут. | _The name on the portrait's Fandom page._ |
| Page | Определённый атрибут. | _The Fandom page credited for the portrait._ |
| Image | Определённый атрибут. | _The original Fandom image._ |
| Crop | Определённый атрибут. | _The crop as an SVG viewBox: x y width height._ |
| Width | Определённый атрибут. | _The original image's width in pixels._ |
| Height | Определённый атрибут. | _The original image's height in pixels._ |
| Sort Order | Определённый атрибут. | _Position in the credits._ |
| Name | Совпадает с display name. | _Order 1. The portrait's name._ |

<details open>
<summary>Примеры данных</summary>

| Display Name | Page | Image | Crop | Width | Height | Sort Order | Name ƒ |
|---|---|---|---|---|---|---|---|
| Marvin Marsh | https://southpark.fandom.com/wiki/Marvin_Marsh | https://static.wikia.nocookie.net/southpark/images/d/db/Marsh-family-grandpa-marsh-w-purple-blanket.png/revision/latest | 48 -5 280 280 | 375 | 536 | 1 | Marvin Marsh |
| Randy Marsh | https://southpark.fandom.com/wiki/Randy_Marsh | https://static.wikia.nocookie.net/southparkfanon/images/d/d6/Randy_Marsh.png/revision/latest | 0 -10 428 428 | 428 | 1057 | 2 | Randy Marsh |
| Sharon Marsh | https://southpark.fandom.com/wiki/Sharon_Marsh | https://static.wikia.nocookie.net/southpark/images/9/99/Sharon_Marsh.png/revision/latest | 20 -10 390 390 | 439 | 1049 | 3 | Sharon Marsh |

_ƒ обозначает вычисляемый столбец._

</details>

| Термин | Описание | Пояснение |
|------|-------------|-------------------|
| **Portrait Aliase** | The names a portrait answers to. A new thing whose name matches an alias gets that portrait. | — |
| Portrait | Определённый атрибут. | _The portrait this name selects._ |
| Name | Совпадает с portrait alias ID. | _Order 1. The alias._ |

<details open>
<summary>Примеры данных</summary>

| Name ƒ |
|---|
| marvin |
| marvin marsh |
| grandpa marsh |

_ƒ обозначает вычисляемый столбец._

</details>

| Термин | Описание | Пояснение |
|------|-------------|-------------------|
| **Property** | The ten properties, with what the symmetric, domain and range rules read. Axioms that relate one property to another (inverse, chain, disjoint) are rows of PropertyAxioms. | — |
| Label | Определённый атрибут. | _The property's wording on the page._ |
| Property Kind | Определённый атрибут. | _'object' links two things, 'datatype' links a thing to a value, 'type' states class membership._ |
| Datatype | Определённый атрибут. | _The value's XML Schema datatype for a datatype property; blank otherwise._ |
| Is User Addable | Истинно, когда пустая строка. | _Offered in the page's Add fact form._ |
| Has One Current Value | Истинно, когда пустая строка. | _The page keeps one current value per person: switching a value on replaces the earlier one._ |
| Is Symmetric | Истинно, когда пустая строка. | _The symmetric rule makes this link true in both directions._ |
| Domain Class | Определённый атрибут. | _The class the domain rule gives this property's subject._ |
| Range Class | Определённый атрибут. | _The class the range rule gives this property's object._ |
| Sort Order | Определённый атрибут. | _Position in the Add fact form's property list._ |
| Value Node Label | Определённый атрибут. | _The small label under a value node for this property: 'role value' or 'number value'._ |
| Current Value Wording | Определённый атрибут. | _How the Add fact form names a replaced value, as in 'Stan now has grade level 5'._ |
| Input Name | Определённый атрибут. | _How the integer check names this property's value, as in 'for grade level'._ |
| Record Kind | Определённый атрибут. | _The word the current-record summary uses for this property's values: role, grade or school._ |
| Record Order | Определённый атрибут. | _Position of this property in the current-record summary._ |
| Name | Совпадает с label. | _Order 1. The property's wording._ |

<details open>
<summary>Примеры данных</summary>

| Label | Property Kind | Datatype | Is User Addable | Has One Current Value | Is Symmetric | Sort Order | Value Node Label | Current Value Wording | Input Name | Record Kind | Record Order | Name ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|
| is parent of | object | — | true | false | false | 1 | — | — | — | — | — | is parent of |
| is child of | object | — | true | false | false | 2 | — | — | — | — | — | is child of |
| is spouse of | object | — | true | false | true | 3 | — | — | — | — | — | is spouse of |

_ƒ обозначает вычисляемый столбец._

</details>

| Термин | Описание | Пояснение |
|------|-------------|-------------------|
| **Property Axiom** | Axioms that relate one property to others, as OWL states them: an inverse, a two-link chain, or a disjoint pair. The candidate generator reads these rows. | — |
| Kind | Определённый атрибут. | _inverse, chain or disjoint._ |
| Property | Определённый атрибут. | _The property the axiom is about: the one inverted, the one a chain derives, or the first of a disjoint pair._ |
| First | Определённый атрибут. | _The inverse property, the chain's first link, or the other property of a disjoint pair._ |
| Second | Определённый атрибут. | _The chain's second link._ |
| Name | Совпадает с property axiom ID. | _Order 1. The axiom's key._ |

<details open>
<summary>Примеры данных</summary>

| Name ƒ |
|---|
| inverse:isParentOf |
| disjoint:isParentOf~isSpouseOf |
| inverse:isChildOf |

_ƒ обозначает вычисляемый столбец._

</details>

| Термин | Описание | Пояснение |
|------|-------------|-------------------|
| **Statement** | Every fact the page can state or conclude about the cast. A listed fact appears in the page's fact list; the rest are candidates that only a rule can make hold. | — |
| Graph | Определённый атрибут. | _The graph the fact belongs to._ |
| Subject | Определённый атрибут. | _What the fact describes._ |
| Property | Определённый атрибут. | _How the subject connects to the object._ |
| Object Thing | Определённый атрибут. | _The related thing, for an object or type property._ |
| Object Value | Определённый атрибут. | _The literal value, for a datatype property._ |
| Is Listed | Истинно, когда пустая строка. | _Shown in the page's fact list._ |
| Is Built in | Истинно, когда пустая строка. | _One of the page's teaching facts; the reader cannot remove it._ |
| Is Detail | Истинно, когда пустая строка. | _One of Stan's detail facts that Show Stan's details switches on._ |
| Is Stated | Истинно, когда пустая строка. | _The fact's checkbox._ |
| Activation Seq | Определённый атрибут. | _When the fact was last switched on, as a running number; decides which value is current._ |
| Sort Order | Определённый атрибут. | _Position in the fact list; candidates follow the listed facts._ |
| Subject Label | Берётся из связанного «subject». | _Order 1. The subject's name._ |
| Property Label | Берётся из связанного «property». | _Order 1. The property's wording._ |
| Object Label | Берётся из связанного «object thing». | _Order 1. The object's name, for an object or type property._ |
| Property Datatype | Берётся из связанного «property». | _Order 1. The value's datatype, for a datatype property._ |
| Property Has One Current Value | Истинно, когда связанный «property» имеет «one current value». | _Order 1. Whether the property keeps one current value per person._ |
| Property Domain Class | Берётся из связанного «property». | _Order 1. The property's domain class._ |
| Property Range Class | Берётся из связанного «property». | _Order 1. The property's range class._ |
| Literal Quote | Берётся из связанного «graph». | _Order 1. The graph's literal quote mark._ |
| Literal Type Marker | Берётся из связанного «graph». | _Order 1. The graph's datatype marker._ |
| Built in Source Text | Берётся из связанного «graph». | _Order 1. The graph's source line for a teaching fact._ |
| User Source Text | Берётся из связанного «graph». | _Order 1. The graph's source line for a reader's fact._ |
| Next Fact Sort Order | Берётся из связанного «graph». | _Order 6. List position of the next fact to add._ |
| Is Literal | Истинно, когда object thing пусто. | _Order 1. True when the object is a value rather than a thing._ |
| Object Text | Определяется по приоритету: object value, если object thing пусто; во всех остальных случаях — object label. | _Order 2. The object's name or the value._ |
| Name | Вычисляется как subject label, затем пробел, затем property label, затем пробел, затем object text. | _Order 3. The fact in words._ |
| Words | Вычисляется как subject label, затем пробел, затем property label, затем пробел, затем literal quote, затем object value, затем literal quote, затем literal type marker, затем property datatype, если object thing пусто, во всех остальных случаях — object label. ⚠︎ механически <!-- rulespeak:reword --> | _Order 2. The fact as a conclusion card quotes it, with a literal written in Turtle form._ |
| Source | Определяется по приоритету: built in source text, если признак «built in» установлен; во всех остальных случаях — user source text. | _Order 2. Where a listed fact came from._ |
| Subject Property Key | Вычисляется как subject, затем «|», затем property. | _Order 1. Groups the values one subject has for one property._ |
| Latest Sibling Activation Seq | Наибольшее значение «activation seq» по записям «statements», связанным с statement. | _Order 2. The most recent activation among this subject's values for this property._ |
| Is in Effect | Истинно, когда выполняется всё следующее: признак «stated» установлен и выполняется хотя бы одно из следующего: признак «property has one current value» не установлен или activation seq равно latest sibling activation seq. | _Order 3. Switched on and, for a one-value property, the most recently switched-on value. Drawn as a solid line._ |
| Listed Unit | Определяется по приоритету: 1, если признак «listed» установлен; во всех остальных случаях — 0. | _Order 1. 1 when the fact is in the fact list._ |
| In Effect Unit | Определяется по приоритету: 1, если признак «in effect» установлен; во всех остальных случаях — 0. | _Order 4. 1 when the fact is in effect._ |
| Listed Off Unit | Определяется по приоритету: 1, если выполняется всё следующее: признак «listed» установлен и признак «in effect» не установлен; во всех остальных случаях — 0. | _Order 4. 1 when the fact is listed but not in effect._ |
| Is Next to Add | Истинно, когда выполняется всё следующее: listed off unit равно 1 и sort order равно next fact sort order. | _Order 7. The fact the Add next fact button switches on._ |
| Count of Inverse Firings | Сумма «inverse fired unit» по записям «firings», связанным с statement. | _Order 6. Inverse rule applications that conclude this fact._ |
| Count of Symmetric Firings | Сумма «symmetric fired unit» по записям «firings», связанным с statement. | _Order 6. Symmetric rule applications that conclude this fact._ |
| Holds Before Chain | Истинно, когда IF от OR от is in effect; count of inverse firings больше 0; и count of symmetric firings больше 0 и истина. | _Order 7. TRUE when the fact is in effect or the inverse or symmetric rule concludes it. Blank when nothing states or concludes it: not known, never FALSE (open world: removing a statement does not say that its opposite is true)._ |
| Count of Chain Firings | Сумма «chain fired unit» по записям «firings», связанным с statement. | _Order 10. Property chain applications that conclude this fact._ |
| Holds Before Typing | Истинно, когда IF от OR от holds before chain и count of chain firings больше 0 и истина. | _Order 11. TRUE when the fact holds before the domain and range rules apply. Blank when nothing states or concludes it: not known, never FALSE (open world: removing a statement does not say that its opposite is true)._ |
| Count of Type Firings | Сумма «type fired unit» по записям «firings», связанным с statement. | _Order 14. Domain and range applications that conclude this fact._ |
| Holds | Истинно, когда IF от OR от holds before typing и count of type firings больше 0 и истина. | _Order 15. TRUE when in effect or concluded by an enabled rule; the graph draws it. Blank when nothing states or concludes it: not known, never FALSE (open world: removing a statement does not say that its opposite is true)._ |
| Is Conclusion | Истинно, когда IF от AND от holds и признак «in effect» не установлен и истина. | _Order 16. TRUE when the fact holds only because a rule concluded it; drawn dashed. Blank otherwise: a stated fact is not a conclusion, and a fact nothing concludes is not known (never FALSE)._ |
| Holds Unit | Определяется по приоритету: 1, если признак «holds» установлен; во всех остальных случаях — 0. | _Order 16. 1 when the fact holds._ |
| Conclusion Unit | Определяется по приоритету: 1, если признак «conclusion» установлен; во всех остальных случаях — 0. | _Order 17. 1 when the fact is a conclusion._ |
| Parent Unit | Определяется по приоритету: 1, если выполняется всё следующее: признак «holds» установлен и property равно «isParentOf»; во всех остальных случаях — 0. | _Order 16. 1 for a holding parent link._ |
| Grandparent Unit | Определяется по приоритету: 1, если выполняется всё следующее: признак «holds» установлен и property равно «isGrandparentOf»; во всех остальных случаях — 0. | _Order 16. 1 for a holding grandparent link._ |
| Place Unit | Определяется по приоритету: 1, если выполняется всё следующее: признак «holds» установлен и property равно «attends»; во всех остальных случаях — 0. | _Order 16. 1 for a holding attends link._ |
| First Pre Chain Order Key | Наименьшее значение «pre chain order key» по записям «firings» (statement), которые являются «pre chain fired unit». | _Order 7. Order key of the first inverse or symmetric application that concludes this fact._ |
| Snap Position | Определяется по приоритету: sort order, если признак «in effect» установлен; 100000 плюс first pre chain order key, если признак «holds before chain» установлен; во всех остальных случаях — 0. | _Order 8. Where the page's reasoner meets this fact as a premise: a stated fact at its list position, a fact concluded by the inverse or symmetric rule after every stated fact, in the order it was concluded; 0 for a fact that does not hold yet, so a link to no fact reads the same on every engine._ |
| First Fired Order Key | Наименьшее значение «order key» по записям «firings» (statement), которые являются «fired unit». | _Order 15. Order key of the first rule application that concludes this fact, in the page reasoner's order._ |
| Count of Chosen Firings | Сумма «chosen unit» по записям «firings», связанным с statement. | _Order 20. Rule applications shown as this fact's conclusion card: one for a conclusion, zero otherwise._ |
| Subject Node Key | Совпадает с subject. | _Order 1. The graph node this fact starts from._ |
| Object Node Key | Определяется по приоритету: «literal|», затем object value, затем «|», затем property datatype, если object thing пусто; во всех остальных случаях — object thing. | _Order 2. The graph node this fact points at: the thing, or one node per value and datatype._ |
| Subject Node Kind | Берётся из связанного «subject». | _Order 19. How the graph draws the subject._ |
| Object Thing Node Kind | Берётся из связанного «object thing». | _Order 19. How the graph draws the object thing._ |
| Object Node Kind | Определяется по приоритету: «value», если object thing пусто; во всех остальных случаях — object thing node kind. | _Order 20. How the graph draws the object: a value, a class, a place or a person._ |
| Object Value Node Label | Берётся из связанного «property». | _Order 1. The small label under a value node._ |
| Count of Violations As First | Сумма «violation unit» по записям «disjoint checks», связанным с statement. | _Order 18. Violated disjointness checks naming this fact first._ |
| Count of Violations As Second | Сумма «violation unit» по записям «disjoint checks», связанным с statement. | _Order 18. Violated disjointness checks naming this fact second._ |
| Is in Contradiction | Истинно, когда выполняется хотя бы одно из следующего: count of violations as first больше 0 или count of violations as second больше 0. | _Order 19. Part of a contradiction the disjoint rule found; drawn in the alarm colour._ |

<details open>
<summary>Примеры данных</summary>

| Object Value | Is Listed | Is Built in | Is Detail | Is Stated | Activation Seq | Sort Order | Subject Label ƒ | Property Label ƒ | Object Label ƒ | Property Datatype ƒ | Property Has One Current Value ƒ | Property Domain Class ƒ | Property Range Class ƒ | Literal Quote ƒ | Literal Type Marker ƒ | Built in Source Text ƒ | User Source Text ƒ | Next Fact Sort Order ƒ | Is Literal ƒ | Object Text ƒ | Name ƒ | Words ƒ | Source ƒ | Subject Property Key ƒ | Latest Sibling Activation Seq ƒ | Is in Effect ƒ | Listed Unit ƒ | In Effect Unit ƒ | Listed Off Unit ƒ | Is Next to Add ƒ | Count of Inverse Firings ƒ | Count of Symmetric Firings ƒ | Holds Before Chain ƒ | Count of Chain Firings ƒ | Holds Before Typing ƒ | Count of Type Firings ƒ | Holds ƒ | Is Conclusion ƒ | Holds Unit ƒ | Conclusion Unit ƒ | Parent Unit ƒ | Grandparent Unit ƒ | Place Unit ƒ | First Pre Chain Order Key ƒ | Snap Position ƒ | First Fired Order Key ƒ | Count of Chosen Firings ƒ | Subject Node Key ƒ | Object Node Key ƒ | Subject Node Kind ƒ | Object Thing Node Kind ƒ | Object Node Kind ƒ | Object Value Node Label ƒ | Count of Violations As First ƒ | Count of Violations As Second ƒ | Is in Contradiction ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| — | true | true | false | true | 1 | 1 | Marvin | is parent of | Randy | — | false | — | — | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | — | false | Randy | Marvin is parent of Randy | Marvin is parent of Randy | Stated as a teaching example on this page. | Marvin\|isParentOf | 1 | true | 1 | 1 | 0 | false | 0 | 0 | true | 0 | true | 0 | true | — | 1 | 0 | 1 | 0 | 0 | — | 1.0 | — | 0 | Marvin | Randy | person | person | person | — | 0 | 0 | false |
| — | false | false | false | false | 0 | 100001 | Marvin | is parent of | Sharon | — | false | — | — | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | — | false | Sharon | Marvin is parent of Sharon | Marvin is parent of Sharon | Stated by you on this page. | Marvin\|isParentOf | 1 | false | 0 | 0 | 0 | false | 0 | 0 | — | 0 | — | 0 | — | — | 0 | 0 | 0 | 0 | 0 | — | 0.0 | — | 0 | Marvin | Sharon | person | person | person | — | 0 | 0 | false |
| — | false | false | false | false | 0 | 100002 | Marvin | is parent of | Stan | — | false | — | — | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | — | false | Stan | Marvin is parent of Stan | Marvin is parent of Stan | Stated by you on this page. | Marvin\|isParentOf | 1 | false | 0 | 0 | 0 | false | 0 | 0 | — | 0 | — | 0 | — | — | 0 | 0 | 0 | 0 | 0 | — | 0.0 | — | 0 | Marvin | Stan | person | person | person | — | 0 | 0 | false |

_ƒ обозначает вычисляемый столбец._

</details>

| Термин | Описание | Пояснение |
|------|-------------|-------------------|
| **Firing** | Every way an enabled rule could conclude a candidate fact from the facts that hold. | — |
| Kind | Определённый атрибут. | _How the rule applies._ |
| Rule | Определённый атрибут. | _The rule switch._ |
| Target | Определённый атрибут. | _The fact this application concludes._ |
| First | Определённый атрибут. | _The premise._ |
| Second | Определённый атрибут. | _The second premise, for the property chain._ |
| Rule Enabled | Истинно, когда связанный «rule» является «enabled». | _Order 1. Whether the rule is switched on._ |
| Kind Wording | Берётся из связанного «kind». | _Order 1. How the card names the rule._ |
| Kind Sort Order | Берётся из связанного «kind». | _Order 1. The kind's place in the reasoner's order._ |
| Target Name | Берётся из связанного «target». | _Order 4. The concluded fact in words._ |
| Target Subject Label | Берётся из связанного «target». | _Order 2. The concluded fact's subject._ |
| Target Property Label | Берётся из связанного «target». | _Order 2. The concluded fact's property._ |
| Target Object Text | Берётся из связанного «target». | _Order 3. The concluded fact's object._ |
| Target is Conclusion | Истинно, когда связанный «target» является «conclusion». | _Order 17. Whether the concluded fact is new._ |
| Target Subject Node Key | Берётся из связанного «target». | _Order 2. The concluded fact's subject node._ |
| Target Object Node Key | Берётся из связанного «target». | _Order 3. The concluded fact's object node._ |
| Target First Fired Order Key | Берётся из связанного «target». | _Order 16. The order key of the concluded fact's first application._ |
| First Words | Берётся из связанного «first». | _Order 3. The premise as the card quotes it._ |
| Second Words | Берётся из связанного «second». | _Order 3. The second premise as the card quotes it._ |
| First is in Effect | Истинно, когда связанный «first» является «in effect». | _Order 4. Whether the premise is in effect._ |
| First Holds Before Chain | Истинно, когда связанный «first» является «holds before chain». | _Order 8. Whether the premise holds before the chain._ |
| Second Holds Before Chain | Истинно, когда связанный «second» является «holds before chain». | _Order 8. Whether the second premise holds before the chain._ |
| First Holds Before Typing | Истинно, когда связанный «first» является «holds before typing». | _Order 12. Whether the premise holds before typing._ |
| First Property | Берётся из связанного «first». | _Order 1. The premise's property key, as the card's axiom names it._ |
| First Property Domain Class | Берётся из связанного «first». | _Order 2. The premise property's domain class._ |
| First Property Range Class | Берётся из связанного «first». | _Order 2. The premise property's range class._ |
| First Sort Order | Берётся из связанного «first». | _Order 1. The premise's list position._ |
| Second is in Effect | Истинно, когда связанный «second» является «in effect». | _Order 4. Whether the second premise is in effect._ |
| First Snap Position | Берётся из связанного «first». | _Order 9. Where the reasoner meets the premise._ |
| Second Snap Position | Берётся из связанного «second». | _Order 9. Where the reasoner meets the second premise._ |
| Inverse Fired Unit | Определяется по приоритету: 1, если выполняется всё следующее: признак «rule enabled» установлен; kind равно «inverse»; и признак «first is in effect» установлен; во всех остальных случаях — 0. | _Order 5. 1 when the inverse rule fires here._ |
| Symmetric Fired Unit | Определяется по приоритету: 1, если выполняется всё следующее: признак «rule enabled» установлен; kind равно «symmetric»; и признак «first is in effect» установлен; во всех остальных случаях — 0. | _Order 5. 1 when the symmetric rule fires here._ |
| Chain Fired Unit | Определяется по приоритету: 1, если выполняется всё следующее: признак «rule enabled» установлен; kind равно «chain»; признак «first holds before chain» установлен; и признак «second holds before chain» установлен; во всех остальных случаях — 0. | _Order 9. 1 when the property chain fires here._ |
| Type Fired Unit | Определяется по приоритету: 1, если выполняется всё следующее: признак «rule enabled» установлен; выполняется хотя бы одно из следующего: kind равно «domain» или kind равно «range»; и признак «first holds before typing» установлен; во всех остальных случаях — 0. | _Order 13. 1 when the domain or range rule fires here._ |
| Fired Unit | Вычисляется как inverse fired unit плюс symmetric fired unit плюс chain fired unit плюс type fired unit. | _Order 14. 1 when this application fires._ |
| Pre Chain Fired Unit | Вычисляется как inverse fired unit плюс symmetric fired unit. | _Order 6. 1 when this is an inverse or symmetric application that fires._ |
| Pre Chain Order Key | Вычисляется как first sort order умножить на 10 плюс kind sort order. | _Order 2. For an inverse or symmetric application: its place in the reasoner's first round._ |
| Round | Определяется по приоритету: 2, если выполняется всё следующее: kind равно «chain» и неверно, что выполняется всё следующее: признак «first is in effect» установлен и признак «second is in effect» установлен; во всех остальных случаях — 1. | _Order 5. The reasoner's round: 1 when every premise is a stated fact, 2 when the property chain reads a concluded fact._ |
| Order Key | Вычисляется как round умножить на 1000000000000000 плюс first snap position умножить на 100000000 плюс kind sort order умножить на 10000000 плюс 0, если second пусто, во всех остальных случаях — second snap position. | _Order 10. The page reasoner's order: by round, then premise, then rule kind, then second premise._ |
| Is Chosen | Истинно, когда выполняется всё следующее: fired unit равно 1; признак «target is conclusion» установлен; и order key равно target first fired order key. | _Order 18. The application the conclusions panel shows for its fact: the first one in the reasoner's order._ |
| Chosen Unit | Определяется по приоритету: 1, если признак «chosen» установлен; во всех остальных случаях — 0. | _Order 19. 1 when this application is shown._ |
| Card Text | Определяется по приоритету: «Worked out by the », затем kind wording, затем « from “», затем first words, затем «”», затем « and “», затем second words, затем «”», если kind равно «chain», во всех остальных случаях — « and “», затем first property, затем « has domain », затем first property domain class, затем «”», если kind равно «domain», во всех остальных случаях — « and “», затем first property, затем « has range », затем first property range class, затем «”», затем точка, если признак «chosen» установлен; во всех остальных случаях — пустая строка. ⚠︎ механически <!-- rulespeak:reword --> | _Order 19. The conclusion card's provenance line, in the page's wording._ |
| Name | Совпадает с firing ID. | _Order 1. The application's key._ |

<details open>
<summary>Примеры данных</summary>

| Rule Enabled ƒ | Kind Wording ƒ | Kind Sort Order ƒ | Target Name ƒ | Target Subject Label ƒ | Target Property Label ƒ | Target Object Text ƒ | Target is Conclusion ƒ | Target Subject Node Key ƒ | Target Object Node Key ƒ | Target First Fired Order Key ƒ | First Words ƒ | Second Words ƒ | First is in Effect ƒ | First Holds Before Chain ƒ | Second Holds Before Chain ƒ | First Holds Before Typing ƒ | First Property ƒ | First Property Domain Class ƒ | First Property Range Class ƒ | First Sort Order ƒ | Second is in Effect ƒ | First Snap Position ƒ | Second Snap Position ƒ | Inverse Fired Unit ƒ | Symmetric Fired Unit ƒ | Chain Fired Unit ƒ | Type Fired Unit ƒ | Fired Unit ƒ | Pre Chain Fired Unit ƒ | Pre Chain Order Key ƒ | Round ƒ | Order Key ƒ | Is Chosen ƒ | Chosen Unit ƒ | Card Text ƒ | Name ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| false | inverse rule | 1 | Randy is child of Marvin | Randy | is child of | Marvin | — | Randy | Marvin | — | Marvin is parent of Randy |    | true | true | — | true | isParentOf | — | — | 1 | false | 1.0 | 0.0 | 0 | 0 | 0 | 0 | 0 | 0 | 11.0 | 1 | 1000000110000000.0 | false | 0 | — | inverse:Marvin\|isParentOf\|Randy>Randy\|isChildOf\|Marvin |
| false | inverse rule | 1 | Sharon is child of Marvin | Sharon | is child of | Marvin | — | Sharon | Marvin | — | Marvin is parent of Sharon |    | false | — | — | — | isParentOf | — | — | 100001 | false | 0.0 | 0.0 | 0 | 0 | 0 | 0 | 0 | 0 | 1000011.0 | 1 | 1000000010000000.0 | false | 0 | — | inverse:Marvin\|isParentOf\|Sharon>Sharon\|isChildOf\|Marvin |
| false | inverse rule | 1 | Stan is child of Marvin | Stan | is child of | Marvin | — | Stan | Marvin | — | Marvin is parent of Stan |    | false | — | — | — | isParentOf | — | — | 100002 | false | 0.0 | 0.0 | 0 | 0 | 0 | 0 | 0 | 0 | 1000021.0 | 1 | 1000000010000000.0 | false | 0 | — | inverse:Marvin\|isParentOf\|Stan>Stan\|isChildOf\|Marvin |

_ƒ обозначает вычисляемый столбец._

</details>

| Термин | Описание | Пояснение |
|------|-------------|-------------------|
| **Disjoint Check** | Every pair the disjointness rule watches: the same two people linked by two disjoint properties. | — |
| Graph | Определённый атрибут. | _The graph._ |
| Rule | Определённый атрибут. | _The rule switch._ |
| First | Определённый атрибут. | _The first link._ |
| Second | Определённый атрибут. | _The disjoint link between the same pair, in the same direction._ |
| Rule Enabled | Истинно, когда связанный «rule» является «enabled». | _Order 1. Whether the disjointness rule is on._ |
| First Holds | Истинно, когда связанный «first» является «holds». | _Order 16. Whether the first link holds._ |
| Second Holds | Истинно, когда связанный «second» является «holds». | _Order 16. Whether the second link holds._ |
| Subject Label | Берётся из связанного «first». | _Order 2. The person recorded with both links._ |
| Object Label | Берётся из связанного «first». | _Order 2. The other person._ |
| Violation Unit | Определяется по приоритету: 1, если выполняется всё следующее: признак «rule enabled» установлен; признак «first holds» установлен; и признак «second holds» установлен; во всех остальных случаях — 0. | _Order 17. 1 when both links hold._ |
| Alarm Text | Определяется по приоритету: «Contradiction found. », затем subject label, затем « is recorded as both parent and spouse of », затем object label, затем «. Check those facts and the selected disjoint rule.», если violation unit равно 1; во всех остальных случаях — пустая строка. | _Order 18. The page's contradiction alert, blank when there is none._ |
| Name | Совпадает с disjoint check ID. | _Order 1. The check's key._ |

<details open>
<summary>Примеры данных</summary>

| Rule Enabled ƒ | First Holds ƒ | Second Holds ƒ | Subject Label ƒ | Object Label ƒ | Violation Unit ƒ | Alarm Text ƒ | Name ƒ |
|---|---|---|---|---|---|---|---|
| false | true | — | Marvin | Randy | 0 | — | Marvin\|isParentOf\|Randy~isSpouseOf |
| false | — | — | Marvin | Sharon | 0 | — | Marvin\|isParentOf\|Sharon~isSpouseOf |
| false | — | — | Marvin | Stan | 0 | — | Marvin\|isParentOf\|Stan~isSpouseOf |

_ƒ обозначает вычисляемый столбец._

</details>

| Термин | Описание | Пояснение |
|------|-------------|-------------------|
| **Scenario** | The page's buttons. Each one is a fact baseline, explicit fact changes and rule settings, applied in that order. | — |
| Graph | Определённый атрибут. | _The graph._ |
| Label | Определённый атрибут. | _The button's text._ |
| Note | Определённый атрибут. | _The note shown under the buttons after this one is pressed; blank clears it._ |
| Sort Order | Определённый атрибут. | _Position among the buttons._ |
| Button Group | Определённый атрибут. | _'stan' for the two imagined changes, 'facts' for the buttons under the fact list._ |
| Is Primary | Истинно, когда пустая строка. | _Drawn as the filled button._ |
| Fact Baseline | Определённый атрибут. | _What happens to every listed fact first: keep, built-in (teaching facts on, others off), built-in-detail, all-off, activate-all (each switched on in list order), or next (the next fact on)._ |
| Layout | Определённый атрибут. | _The node layout the graph returns to; blank keeps the current positions._ |
| Clears Selection | Истинно, когда пустая строка. | _Clears the selected node or line._ |
| Clears Form Message | Истинно, когда пустая строка. | _Clears the Add fact form's message._ |
| Name | Совпадает с label. | _Order 1. The button's text._ |
| Graph Has Facts to Add | Истинно, когда связанный «graph» имеет «facts to add». | _Order 7. Whether a listed fact can still be added._ |
| Is Available | Истинно, когда выполняется хотя бы одно из следующего: fact baseline не равно «next» или признак «graph has facts to add» установлен. | _Order 8. False only for Add next fact once every listed fact is in effect._ |

<details open>
<summary>Примеры данных</summary>

| Label | Note | Sort Order | Button Group | Is Primary | Fact Baseline | Layout | Clears Selection | Clears Form Message | Name ƒ | Graph Has Facts to Add ƒ | Is Available ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|
| Move Stan to grade 5 | This imagined change updates Stan’s grade to 5. His identifier, Person statement and family links stay the same. His other selected details are unchanged. | 1 | stan | false | keep | — | true | true | Move Stan to grade 5 | false | true |
| Stan leaves school | This imagined change clears Stan’s current school details. His identifier, Person statement and family links remain. Removing a current fact does not say that its opposite is true; school history needs dates. | 2 | stan | false | keep | — | true | true | Stan leaves school | false | true |
| Add next fact | — | 3 | facts | true | next | — | false | false | Add next fact | false | false |

_ƒ обозначает вычисляемый столбец._

</details>

| Термин | Описание | Пояснение |
|------|-------------|-------------------|
| **Scenario Fact Change** | Fact changes a button makes after its baseline. A blank object key matches every listed value of the property. | — |
| Scenario | Определённый атрибут. | _The button._ |
| Subject | Определённый атрибут. | _The subject whose facts change._ |
| Property | Определённый атрибут. | _The property whose facts change._ |
| Object Key | Определённый атрибут. | _The object thing's key or the literal value; blank for every listed value._ |
| Action | Определённый атрибут. | _'on' switches the fact on as the newest value; 'off' switches it off._ |
| Adds to List | Истинно, когда пустая строка. | _Adds the fact to the fact list first when it is not listed._ |
| Sort Order | Определённый атрибут. | _Order of the change within the button._ |
| Name | Совпадает с scenario fact change ID. | _Order 1. The change's key._ |

<details open>
<summary>Примеры данных</summary>

| Object Key | Action | Adds to List | Sort Order | Name ƒ |
|---|---|---|---|---|
| Person | on | true | 1 | stan-grade-five:Stan\|isA\|Person |
| 5 | on | true | 2 | stan-grade-five:Stan\|hasGradeLevel\|5 |
| Person | on | true | 3 | stan-leaves-school:Stan\|isA\|Person |

_ƒ обозначает вычисляемый столбец._

</details>

| Термин | Описание | Пояснение |
|------|-------------|-------------------|
| **Scenario Rule Setting** | Rule switches a button sets. | — |
| Scenario | Определённый атрибут. | _The button._ |
| Rule | Определённый атрибут. | _The rule switch._ |
| Is Enabled | Истинно, когда пустая строка. | _The state the button puts the switch in._ |
| Name | Совпадает с scenario rule setting ID. | _Order 1. The setting's key._ |

<details open>
<summary>Примеры данных</summary>

| Is Enabled | Name ƒ |
|---|---|
| false | reset-original:inverse |
| false | reset-original:symmetric |
| false | reset-original:chain |

_ƒ обозначает вычисляемый столбец._

</details>

| Термин | Описание | Пояснение |
|------|-------------|-------------------|
| **Node Layout** | Node positions in the graph's 380 by 480 drawing, one set per layout. | — |
| Layout | Определённый атрибут. | _The layout: 'default' or 'stan-details'._ |
| Node Key | Определённый атрибут. | _The node: a thing's key, or literal\|value\|datatype._ |
| X | Определённый атрибут. | _Horizontal position._ |
| Y | Определённый атрибут. | _Vertical position._ |
| Name | Совпадает с node layout ID. | _Order 1. The position's key._ |

<details open>
<summary>Примеры данных</summary>

| Layout | Node Key | X | Y | Name ƒ |
|---|---|---|---|---|
| default | Marvin | 70 | 45 | default:Marvin |
| default | Randy | 195 | 125 | default:Randy |
| default | Sharon | 318 | 55 | default:Sharon |

_ƒ обозначает вычисляемый столбец._

</details>

| Термин | Описание | Пояснение |
|------|-------------|-------------------|
| **Layout Slot** | Places for nodes no layout positions, used in turn. | — |
| Sort Order | Определённый атрибут. | _Order in which slots are used._ |
| X | Определённый атрибут. | _Horizontal position._ |
| Y | Определённый атрибут. | _Vertical position._ |
| Name | Совпадает с layout slot ID. | _Order 1. The slot's key._ |

<details open>
<summary>Примеры данных</summary>

| Sort Order | X | Y | Name ƒ |
|---|---|---|---|
| 1 | 330 | 135 | slot-1 |
| 2 | 45 | 120 | slot-2 |
| 3 | 330 | 400 | slot-3 |

_ƒ обозначает вычисляемый столбец._

</details>

| Термин | Описание | Пояснение |
|------|-------------|-------------------|
| **Page Text** | The page's wording, one row per string. {name}-style placeholders are filled by the page. | — |
| Text | Определённый атрибут. | _The string._ |
| Name | Совпадает с page text ID. | _Order 1. The string's key._ |

<details open>
<summary>Примеры данных</summary>

| Text | Name ƒ |
|---|---|
| South Park Ontology Explainer, on one rulebook | page-title |
| A rulebook rebuild of the explainer by | byline-prefix |
| Jessica Talisman | byline-author |

_ƒ обозначает вычисляемый столбец._

</details>

| Термин | Описание | Пояснение |
|------|-------------|-------------------|
| **Engine** | The execution engines the page can be computed by. Each is a build output of this rulebook. | — |
| Label | Определённый атрибут. | _The engine's name in the switcher._ |
| Substrate | Определённый атрибут. | _The conformance harness's name for this substrate._ |
| Is Default | Истинно, когда пустая строка. | _The engine a new visitor starts on._ |
| Description | Определённый атрибут. | _What computes the page on this engine._ |
| Sort Order | Определённый атрибут. | _Position in the switcher._ |
| Name | Совпадает с label. | _Order 1. The engine's name._ |

<details open>
<summary>Примеры данных</summary>

| Label | Substrate | Is Default | Description | Sort Order | Name ƒ |
|---|---|---|---|---|---|
| Postgres views | effortless-postgres | true | Every value is a column of a generated view; the database computes it when the page reads it. | 1 | Postgres views |
| TypeScript | effortless-typescript | false | The generated TypeScript computes every field from the same rows, with no database. | 2 | TypeScript |
| OWL and SHACL rules | effortless-owl | false | The generated ontology and SHACL rules, run over the same rows by a SPARQL engine. | 3 | OWL and SHACL rules |

_ƒ обозначает вычисляемый столбец._

</details>

| Термин | Описание | Пояснение |
|------|-------------|-------------------|
| **Rulebook Tool** | What each rulebook tool adds, for each form of the rulebook. The explainer page and the ontology form's editor list the ontology form's tools; the concept form's editor lists the concept form's. The two forms are the same facts, so they list the same tools. | — |
| Label | Определённый атрибут. | _The tool's name._ |
| Adds | Определённый атрибут. | _What it adds, for this form of the rulebook._ |
| Href | Определённый атрибут. | _Where its link goes; 'editor' means this form's running rulebook editor._ |
| Link Text | Определённый атрибут. | _The link's text._ |
| Sort Order | Определённый атрибут. | _Position in the panel._ |
| Form | Определённый атрибут. | _Which form of the rulebook the tool is about: ontology (things, properties, statements) or concept (tables)._ |
| Name | Совпадает с label. | _Order 1. The tool's name._ |

<details open>
<summary>Примеры данных</summary>

| Label | Adds | Href | Link Text | Sort Order | Form | Name ƒ |
|---|---|---|---|---|---|---|
| Interchangeable engines | The same page, computed by Postgres views, generated TypeScript or the generated OWL and SHACL rules. Switch with the control above; nothing else changes. | — | — | 1 | ontology | Interchangeable engines |
| Conformance | Every engine's answers for the current facts, compared value by value with the conformance harness's own rule for when two values are equal. | — | — | 2 | ontology | Conformance |
| Explainer | Hover a dotted value for its rule in plain words and its witness: the values it came from, down to the stated facts. | /rulebook-explainer-dag/pages/index.html | Open the explainer | 3 | ontology | Explainer |

_ƒ обозначает вычисляемый столбец._

</details>

## 2 Типы фактов

- **firing kind** ссылается ровно на один **rule**
- **thing** может ссылаться на один **portrait**
- **portrait aliase** ссылается ровно на один **portrait**
- **property** может ссылаться на один **thing**
- **property axiom** ссылается ровно на один **firing kind**
- **property axiom** ссылается ровно на один **property**
- **statement** ссылается ровно на один **graph**
- **statement** ссылается ровно на один **thing**
- **statement** ссылается ровно на один **property**
- **firing** ссылается ровно на один **firing kind**
- **firing** ссылается ровно на один **rule**
- **firing** ссылается ровно на один **statement**
- **disjoint check** ссылается ровно на один **graph**
- **disjoint check** ссылается ровно на один **rule**
- **disjoint check** ссылается ровно на один **statement**
- **scenario** ссылается ровно на один **graph**
- **scenario fact change** ссылается ровно на один **scenario**
- **scenario fact change** ссылается ровно на один **thing**
- **scenario fact change** ссылается ровно на один **property**
- **scenario rule setting** ссылается ровно на один **scenario**
- **scenario rule setting** ссылается ровно на один **rule**

## 3 Оперативные правила

_Оперативные правила формулируют, что бизнес **обязывает**, **запрещает** или
рекомендует (**следует**). Структурные правила берутся из обязательных полей и внешних
ключей; семантические — из таблицы Constraints, каждое опирается на булево значение,
которое книга правил уже вычисляет (со ссылкой DR-N в определяющих правилах ниже)._

### Структурные ограничения (из схемы)

- Graph **обязан** иметь label; literal quote; literal type marker; built in source text; user source text; и no conclusions text.
- Rule **обязан** иметь label; explanation; и sort order, а также фиксировать то, он является «enabled» ли.
- Firing kind **обязан** ссылаться ровно на один «rule».
- Firing kind **обязан** иметь wording и sort order.
- Thing **обязан** иметь label; kind; и sort order.
- Portrait **обязан** иметь display name; page; image; crop; width; height; и sort order.
- Portrait aliase **обязан** ссылаться ровно на один «portrait».
- Property **обязан** иметь label; property kind; и sort order, а также фиксировать то, он является «user addable» ли; то, он имеет «one current value» ли; и то, он является «symmetric» ли.
- Property axiom **обязан** ссылаться ровно на один «firing kind» в роли «kind».
- Property axiom **обязан** ссылаться ровно на один «property».
- Property axiom **обязан** ссылаться ровно на один «property» в роли «first».
- Statement **обязан** ссылаться ровно на один «graph».
- Statement **обязан** ссылаться ровно на один «thing» в роли «subject».
- Statement **обязан** ссылаться ровно на один «property».
- Statement **обязан** иметь activation seq и sort order, а также фиксировать то, он является «listed» ли; то, он является «built in» ли; то, он является «detail» ли; и то, он является «stated» ли.
- Firing **обязан** ссылаться ровно на один «firing kind» в роли «kind».
- Firing **обязан** ссылаться ровно на один «rule».
- Firing **обязан** ссылаться ровно на один «statement» в роли «target».
- Firing **обязан** ссылаться ровно на один «statement» в роли «first».
- Disjoint check **обязан** ссылаться ровно на один «graph».
- Disjoint check **обязан** ссылаться ровно на один «rule».
- Disjoint check **обязан** ссылаться ровно на один «statement» в роли «first».
- Disjoint check **обязан** ссылаться ровно на один «statement» в роли «second».
- Scenario **обязан** ссылаться ровно на один «graph».
- Scenario **обязан** иметь label; sort order; button group; и fact baseline, а также фиксировать то, он является «primary» ли; то, он является «clears selection» ли; и то, он является «clears form message» ли.
- Scenario fact change **обязан** ссылаться ровно на один «scenario».
- Scenario fact change **обязан** ссылаться ровно на один «thing» в роли «subject».
- Scenario fact change **обязан** ссылаться ровно на один «property».
- Scenario fact change **обязан** иметь action и sort order, а также фиксировать то, он является «adds to list» ли.
- Scenario rule setting **обязан** ссылаться ровно на один «scenario».
- Scenario rule setting **обязан** ссылаться ровно на один «rule».
- Scenario rule setting **обязан** фиксировать то, он является «enabled» ли.
- Node layout **обязан** иметь layout; node key; x; и y.
- Layout slot **обязан** иметь sort order; x; и y.
- Page text **обязан** иметь text.
- Engine **обязан** иметь label; substrate; description; и sort order, а также фиксировать то, он является «default» ли.
- Rulebook tool **обязан** иметь label; adds; sort order; и form.

## 4 Определяющие правила

_Все утверждения выражают истины предметной области; это не процедуры и не
императивы. Вместо «тогда и только тогда» используется «только если», чтобы
одностороннюю необходимость не приняли за эквивалентность. Пометка
**⚠︎ механически** указывает правило, чья детерминированная формулировка верна,
но тяжеловесна — сигнал для необязательной переформулировки, а не дефект._

| ID | Декларативное правило |
|----|------------------|
| **DR-1 Name** | Graph: «name» совпадает с label. |
| **DR-2 Count of Stated Facts** | Graph: «count of stated facts» — это сумма «in effect unit» по записям «statements», связанным с graph. |
| **DR-3 Count of Conclusions** | Graph: «count of conclusions» — это сумма «conclusion unit» по записям «statements», связанным с graph. |
| **DR-4 Count of Violations** | Graph: «count of violations» — это сумма «violation unit» по записям «disjoint checks», связанным с graph. |
| **DR-5 Is Consistent** | Graph считается «consistent», если count of violations равно 0. |
| **DR-6 Conclusions Message** | Поле «conclusions message» (Graph) определяется по следующему приоритету:<br>1. no conclusions text, если count of conclusions равно 0;<br>2. во всех остальных случаях — пустая строка. |
| **DR-7 Count of Facts to Add** | Graph: «count of facts to add» — это сумма «listed off unit» по записям «statements», связанным с graph. |
| **DR-8 Has Facts to Add** | Graph считается имеющим «facts to add», если count of facts to add больше 0. |
| **DR-9 Next Fact Sort Order** | Graph: «next fact sort order» — это наименьшее значение «sort order» по записям «statements» (graph), которые являются «listed off unit». |
| **DR-10 Max Activation Seq** | Graph: «max activation seq» — это наибольшее значение «activation seq» по записям «statements», связанным с graph. |
| **DR-11 Next Activation Seq** | Graph: «next activation seq» вычисляется как max activation seq плюс 1. |
| **DR-12 Max Listed Sort Order** | Graph: «max listed sort order» — это наибольшее значение «sort order» по записям «statements» (graph), которые являются «listed unit». |
| **DR-13 Next Listed Sort Order** | Graph: «next listed sort order» вычисляется как max listed sort order плюс 1. |
| **DR-14 Max Sort Order** | Graph: «max sort order» — это наибольшее значение «sort order» по записям «statements», связанным с graph. |
| **DR-15 Next Candidate Sort Order** | Graph: «next candidate sort order» вычисляется как max sort order плюс 1. |
| **DR-16 Name** | Rule: «name» совпадает с label. |
| **DR-17 Count of Firings** | Rule: «count of firings» — это сумма «fired unit» по записям «firings», связанным с rule. |
| **DR-18 Name** | Firing kind: «name» совпадает с wording. |
| **DR-19 Rule Enabled** | Firing kind: «rule enabled» истинно, когда связанный «rule» является «enabled». |
| **DR-20 Portrait Name** | Thing: «portrait name» — это «display name» связанного «portrait». |
| **DR-21 Portrait Page** | Thing: «portrait page» — берётся из связанного «portrait». |
| **DR-22 Portrait Image** | Thing: «portrait image» — берётся из связанного «portrait». |
| **DR-23 Portrait Crop** | Thing: «portrait crop» — берётся из связанного «portrait». |
| **DR-24 Portrait Width** | Thing: «portrait width» — берётся из связанного «portrait». |
| **DR-25 Portrait Height** | Thing: «portrait height» — берётся из связанного «portrait». |
| **DR-26 Name** | Thing: «name» совпадает с label. |
| **DR-27 Node Key** | Thing: «node key» совпадает с thing ID. |
| **DR-28 Count of Children** | Thing: «count of children» — это сумма «parent unit» по записям «statements», связанным с thing. |
| **DR-29 Count of Grandchildren** | Thing: «count of grandchildren» — это сумма «grandparent unit» по записям «statements», связанным с thing. |
| **DR-30 Count As Place** | Thing: «count as place» — это сумма «place unit» по записям «statements», связанным с thing. |
| **DR-31 Count As Subject** | Thing: «count as subject» — это сумма «holds unit» по записям «statements», связанным с thing. |
| **DR-32 Count As Object** | Thing: «count as object» — это сумма «holds unit» по записям «statements», связанным с thing. |
| **DR-33 Is in Graph** | Thing считается «in graph», если выполняется хотя бы одно из следующего: count as subject больше 0 или count as object больше 0. |
| **DR-34 Node Kind** | Поле «node kind» (Thing) определяется по следующему приоритету:<br>1. пустая строка, если kind пусто;<br>2. «class», если kind равно «class»;<br>3. «place», если count as place больше 0;<br>4. во всех остальных случаях — «person». |
| **DR-35 Name** | Portrait: «name» совпадает с display name. |
| **DR-36 Name** | Portrait aliase: «name» совпадает с portrait alias ID. |
| **DR-37 Name** | Property: «name» совпадает с label. |
| **DR-38 Name** | Property axiom: «name» совпадает с property axiom ID. |
| **DR-39 Subject Label** | Statement: «subject label» — берётся из связанного «subject». |
| **DR-40 Property Label** | Statement: «property label» — берётся из связанного «property». |
| **DR-41 Object Label** | Statement: «object label» — берётся из связанного «object thing». |
| **DR-42 Property Datatype** | Statement: «property datatype» — берётся из связанного «property». |
| **DR-43 Property Has One Current Value** | Statement: «property has one current value» истинно, когда связанный «property» имеет «one current value». |
| **DR-44 Property Domain Class** | Statement: «property domain class» — берётся из связанного «property». |
| **DR-45 Property Range Class** | Statement: «property range class» — берётся из связанного «property». |
| **DR-46 Literal Quote** | Statement: «literal quote» — берётся из связанного «graph». |
| **DR-47 Literal Type Marker** | Statement: «literal type marker» — берётся из связанного «graph». |
| **DR-48 Built in Source Text** | Statement: «built in source text» — берётся из связанного «graph». |
| **DR-49 User Source Text** | Statement: «user source text» — берётся из связанного «graph». |
| **DR-50 Next Fact Sort Order** | Statement: «next fact sort order» — берётся из связанного «graph». |
| **DR-51 Is Literal** | Statement считается «literal», если object thing пусто. |
| **DR-52 Object Text** | Поле «object text» (Statement) определяется по следующему приоритету:<br>1. object value, если object thing пусто;<br>2. во всех остальных случаях — object label. |
| **DR-53 Name** | Statement: «name» вычисляется как subject label, затем пробел, затем property label, затем пробел, затем object text. |
| **DR-54 Words** | Statement: «words» вычисляется как subject label, затем пробел, затем property label, затем пробел, затем literal quote, затем object value, затем literal quote, затем literal type marker, затем property datatype, если object thing пусто, во всех остальных случаях — object label. ⚠︎ механически <!-- rulespeak:reword --> |
| **DR-55 Source** | Поле «source» (Statement) определяется по следующему приоритету:<br>1. built in source text, если признак «built in» установлен;<br>2. во всех остальных случаях — user source text. |
| **DR-56 Subject Property Key** | Statement: «subject property key» вычисляется как subject, затем «|», затем property. |
| **DR-57 Latest Sibling Activation Seq** | Statement: «latest sibling activation seq» — это наибольшее значение «activation seq» по записям «statements», связанным с statement. |
| **DR-58 Is in Effect** | Statement считается «in effect», если выполняется всё следующее: признак «stated» установлен и выполняется хотя бы одно из следующего: признак «property has one current value» не установлен или activation seq равно latest sibling activation seq. |
| **DR-59 Listed Unit** | Поле «listed unit» (Statement) определяется по следующему приоритету:<br>1. 1, если признак «listed» установлен;<br>2. во всех остальных случаях — 0. |
| **DR-60 In Effect Unit** | Поле «in effect unit» (Statement) определяется по следующему приоритету:<br>1. 1, если признак «in effect» установлен;<br>2. во всех остальных случаях — 0. |
| **DR-61 Listed Off Unit** | Поле «listed off unit» (Statement) определяется по следующему приоритету:<br>1. 1, если выполняется всё следующее: признак «listed» установлен и признак «in effect» не установлен;<br>2. во всех остальных случаях — 0. |
| **DR-62 Is Next to Add** | Statement считается «next to add», если выполняется всё следующее: listed off unit равно 1 и sort order равно next fact sort order. |
| **DR-63 Count of Inverse Firings** | Statement: «count of inverse firings» — это сумма «inverse fired unit» по записям «firings», связанным с statement. |
| **DR-64 Count of Symmetric Firings** | Statement: «count of symmetric firings» — это сумма «symmetric fired unit» по записям «firings», связанным с statement. |
| **DR-65 Holds Before Chain** | Statement считается удовлетворяющим условию «before chain», если IF от OR от is in effect; count of inverse firings больше 0; и count of symmetric firings больше 0 и истина. |
| **DR-66 Count of Chain Firings** | Statement: «count of chain firings» — это сумма «chain fired unit» по записям «firings», связанным с statement. |
| **DR-67 Holds Before Typing** | Statement считается удовлетворяющим условию «before typing», если IF от OR от holds before chain и count of chain firings больше 0 и истина. |
| **DR-68 Count of Type Firings** | Statement: «count of type firings» — это сумма «type fired unit» по записям «firings», связанным с statement. |
| **DR-69 Holds** | Statement считается удовлетворяющим условию «», если IF от OR от holds before typing и count of type firings больше 0 и истина. |
| **DR-70 Is Conclusion** | Statement считается «conclusion», если IF от AND от holds и признак «in effect» не установлен и истина. |
| **DR-71 Holds Unit** | Поле «holds unit» (Statement) определяется по следующему приоритету:<br>1. 1, если признак «holds» установлен;<br>2. во всех остальных случаях — 0. |
| **DR-72 Conclusion Unit** | Поле «conclusion unit» (Statement) определяется по следующему приоритету:<br>1. 1, если признак «conclusion» установлен;<br>2. во всех остальных случаях — 0. |
| **DR-73 Parent Unit** | Поле «parent unit» (Statement) определяется по следующему приоритету:<br>1. 1, если выполняется всё следующее: признак «holds» установлен и property равно «isParentOf»;<br>2. во всех остальных случаях — 0. |
| **DR-74 Grandparent Unit** | Поле «grandparent unit» (Statement) определяется по следующему приоритету:<br>1. 1, если выполняется всё следующее: признак «holds» установлен и property равно «isGrandparentOf»;<br>2. во всех остальных случаях — 0. |
| **DR-75 Place Unit** | Поле «place unit» (Statement) определяется по следующему приоритету:<br>1. 1, если выполняется всё следующее: признак «holds» установлен и property равно «attends»;<br>2. во всех остальных случаях — 0. |
| **DR-76 First Pre Chain Order Key** | Statement: «first pre chain order key» — это наименьшее значение «pre chain order key» по записям «firings» (statement), которые являются «pre chain fired unit». |
| **DR-77 Snap Position** | Поле «snap position» (Statement) определяется по следующему приоритету:<br>1. sort order, если признак «in effect» установлен;<br>2. 100000 плюс first pre chain order key, если признак «holds before chain» установлен;<br>3. во всех остальных случаях — 0. |
| **DR-78 First Fired Order Key** | Statement: «first fired order key» — это наименьшее значение «order key» по записям «firings» (statement), которые являются «fired unit». |
| **DR-79 Count of Chosen Firings** | Statement: «count of chosen firings» — это сумма «chosen unit» по записям «firings», связанным с statement. |
| **DR-80 Subject Node Key** | Statement: «subject node key» совпадает с subject. |
| **DR-81 Object Node Key** | Поле «object node key» (Statement) определяется по следующему приоритету:<br>1. «literal|», затем object value, затем «|», затем property datatype, если object thing пусто;<br>2. во всех остальных случаях — object thing. |
| **DR-82 Subject Node Kind** | Statement: «subject node kind» — берётся из связанного «subject». |
| **DR-83 Object Thing Node Kind** | Statement: «object thing node kind» — берётся из связанного «object thing». |
| **DR-84 Object Node Kind** | Поле «object node kind» (Statement) определяется по следующему приоритету:<br>1. «value», если object thing пусто;<br>2. во всех остальных случаях — object thing node kind. |
| **DR-85 Object Value Node Label** | Statement: «object value node label» — берётся из связанного «property». |
| **DR-86 Count of Violations As First** | Statement: «count of violations as first» — это сумма «violation unit» по записям «disjoint checks», связанным с statement. |
| **DR-87 Count of Violations As Second** | Statement: «count of violations as second» — это сумма «violation unit» по записям «disjoint checks», связанным с statement. |
| **DR-88 Is in Contradiction** | Statement считается «in contradiction», если выполняется хотя бы одно из следующего: count of violations as first больше 0 или count of violations as second больше 0. |
| **DR-89 Rule Enabled** | Firing: «rule enabled» истинно, когда связанный «rule» является «enabled». |
| **DR-90 Kind Wording** | Firing: «kind wording» — берётся из связанного «kind». |
| **DR-91 Kind Sort Order** | Firing: «kind sort order» — берётся из связанного «kind». |
| **DR-92 Target Name** | Firing: «target name» — берётся из связанного «target». |
| **DR-93 Target Subject Label** | Firing: «target subject label» — берётся из связанного «target». |
| **DR-94 Target Property Label** | Firing: «target property label» — берётся из связанного «target». |
| **DR-95 Target Object Text** | Firing: «target object text» — берётся из связанного «target». |
| **DR-96 Target is Conclusion** | Firing: «target is conclusion» истинно, когда связанный «target» является «conclusion». |
| **DR-97 Target Subject Node Key** | Firing: «target subject node key» — берётся из связанного «target». |
| **DR-98 Target Object Node Key** | Firing: «target object node key» — берётся из связанного «target». |
| **DR-99 Target First Fired Order Key** | Firing: «target first fired order key» — берётся из связанного «target». |
| **DR-100 First Words** | Firing: «first words» — берётся из связанного «first». |
| **DR-101 Second Words** | Firing: «second words» — берётся из связанного «second». |
| **DR-102 First is in Effect** | Firing: «first is in effect» истинно, когда связанный «first» является «in effect». |
| **DR-103 First Holds Before Chain** | Firing: «first holds before chain» истинно, когда связанный «first» является «holds before chain». |
| **DR-104 Second Holds Before Chain** | Firing: «second holds before chain» истинно, когда связанный «second» является «holds before chain». |
| **DR-105 First Holds Before Typing** | Firing: «first holds before typing» истинно, когда связанный «first» является «holds before typing». |
| **DR-106 First Property** | Firing: «first property» — берётся из связанного «first». |
| **DR-107 First Property Domain Class** | Firing: «first property domain class» — берётся из связанного «first». |
| **DR-108 First Property Range Class** | Firing: «first property range class» — берётся из связанного «first». |
| **DR-109 First Sort Order** | Firing: «first sort order» — берётся из связанного «first». |
| **DR-110 Second is in Effect** | Firing: «second is in effect» истинно, когда связанный «second» является «in effect». |
| **DR-111 First Snap Position** | Firing: «first snap position» — берётся из связанного «first». |
| **DR-112 Second Snap Position** | Firing: «second snap position» — берётся из связанного «second». |
| **DR-113 Inverse Fired Unit** | Поле «inverse fired unit» (Firing) определяется по следующему приоритету:<br>1. 1, если выполняется всё следующее: признак «rule enabled» установлен; kind равно «inverse»; и признак «first is in effect» установлен;<br>2. во всех остальных случаях — 0. |
| **DR-114 Symmetric Fired Unit** | Поле «symmetric fired unit» (Firing) определяется по следующему приоритету:<br>1. 1, если выполняется всё следующее: признак «rule enabled» установлен; kind равно «symmetric»; и признак «first is in effect» установлен;<br>2. во всех остальных случаях — 0. |
| **DR-115 Chain Fired Unit** | Поле «chain fired unit» (Firing) определяется по следующему приоритету:<br>1. 1, если выполняется всё следующее: признак «rule enabled» установлен; kind равно «chain»; признак «first holds before chain» установлен; и признак «second holds before chain» установлен;<br>2. во всех остальных случаях — 0. |
| **DR-116 Type Fired Unit** | Поле «type fired unit» (Firing) определяется по следующему приоритету:<br>1. 1, если выполняется всё следующее: признак «rule enabled» установлен; выполняется хотя бы одно из следующего: kind равно «domain» или kind равно «range»; и признак «first holds before typing» установлен;<br>2. во всех остальных случаях — 0. |
| **DR-117 Fired Unit** | Firing: «fired unit» вычисляется как inverse fired unit плюс symmetric fired unit плюс chain fired unit плюс type fired unit. |
| **DR-118 Pre Chain Fired Unit** | Firing: «pre chain fired unit» вычисляется как inverse fired unit плюс symmetric fired unit. |
| **DR-119 Pre Chain Order Key** | Firing: «pre chain order key» вычисляется как first sort order умножить на 10 плюс kind sort order. |
| **DR-120 Round** | Поле «round» (Firing) определяется по следующему приоритету:<br>1. 2, если выполняется всё следующее: kind равно «chain» и неверно, что выполняется всё следующее: признак «first is in effect» установлен и признак «second is in effect» установлен;<br>2. во всех остальных случаях — 1. |
| **DR-121 Order Key** | Firing: «order key» вычисляется как round умножить на 1000000000000000 плюс first snap position умножить на 100000000 плюс kind sort order умножить на 10000000 плюс 0, если second пусто, во всех остальных случаях — second snap position. |
| **DR-122 Is Chosen** | Firing считается «chosen», если выполняется всё следующее: fired unit равно 1; признак «target is conclusion» установлен; и order key равно target first fired order key. |
| **DR-123 Chosen Unit** | Поле «chosen unit» (Firing) определяется по следующему приоритету:<br>1. 1, если признак «chosen» установлен;<br>2. во всех остальных случаях — 0. |
| **DR-124 Card Text** | Поле «card text» (Firing) определяется по следующему приоритету:<br>1. «Worked out by the », затем kind wording, затем « from “», затем first words, затем «”», затем « and “», затем second words, затем «”», если kind равно «chain», во всех остальных случаях — « and “», затем first property, затем « has domain », затем first property domain class, затем «”», если kind равно «domain», во всех остальных случаях — « and “», затем first property, затем « has range », затем first property range class, затем «”», затем точка, если признак «chosen» установлен;<br>2. во всех остальных случаях — пустая строка. ⚠︎ механически <!-- rulespeak:reword --> |
| **DR-125 Name** | Firing: «name» совпадает с firing ID. |
| **DR-126 Rule Enabled** | Disjoint check: «rule enabled» истинно, когда связанный «rule» является «enabled». |
| **DR-127 First Holds** | Disjoint check: «first holds» истинно, когда связанный «first» является «holds». |
| **DR-128 Second Holds** | Disjoint check: «second holds» истинно, когда связанный «second» является «holds». |
| **DR-129 Subject Label** | Disjoint check: «subject label» — берётся из связанного «first». |
| **DR-130 Object Label** | Disjoint check: «object label» — берётся из связанного «first». |
| **DR-131 Violation Unit** | Поле «violation unit» (Disjoint check) определяется по следующему приоритету:<br>1. 1, если выполняется всё следующее: признак «rule enabled» установлен; признак «first holds» установлен; и признак «second holds» установлен;<br>2. во всех остальных случаях — 0. |
| **DR-132 Alarm Text** | Поле «alarm text» (Disjoint check) определяется по следующему приоритету:<br>1. «Contradiction found. », затем subject label, затем « is recorded as both parent and spouse of », затем object label, затем «. Check those facts and the selected disjoint rule.», если violation unit равно 1;<br>2. во всех остальных случаях — пустая строка. |
| **DR-133 Name** | Disjoint check: «name» совпадает с disjoint check ID. |
| **DR-134 Name** | Scenario: «name» совпадает с label. |
| **DR-135 Graph Has Facts to Add** | Scenario: «graph has facts to add» истинно, когда связанный «graph» имеет «facts to add». |
| **DR-136 Is Available** | Scenario считается «available», если выполняется хотя бы одно из следующего: fact baseline не равно «next» или признак «graph has facts to add» установлен. |
| **DR-137 Name** | Scenario fact change: «name» совпадает с scenario fact change ID. |
| **DR-138 Name** | Scenario rule setting: «name» совпадает с scenario rule setting ID. |
| **DR-139 Name** | Node layout: «name» совпадает с node layout ID. |
| **DR-140 Name** | Layout slot: «name» совпадает с layout slot ID. |
| **DR-141 Name** | Page text: «name» совпадает с page text ID. |
| **DR-142 Name** | Engine: «name» совпадает с label. |
| **DR-143 Name** | Rulebook tool: «name» совпадает с label. |

## 5 Прослеживаемость к схеме

_Колонка выражения — определение правила в нотации Документация:
та же логика, что хранит книга правил, записанная для бизнес-читателя._

| Элемент схемы | Вид | Выражение |
|----------------|------|------------|
| **Graphs.Name** | формула | `Label` |
| **Graphs.CountOfStatedFacts** | свёртка | `Sum(Statements.InEffectUnit via Graph)` |
| **Graphs.CountOfConclusions** | свёртка | `Sum(Statements.ConclusionUnit via Graph)` |
| **Graphs.CountOfViolations** | свёртка | `Sum(DisjointChecks.ViolationUnit via Graph)` |
| **Graphs.IsConsistent** | формула | `CountOfViolations = 0` |
| **Graphs.ConclusionsMessage** | формула | `If(CountOfConclusions = 0, NoConclusionsText, "")` |
| **Graphs.CountOfFactsToAdd** | свёртка | `Sum(Statements.ListedOffUnit via Graph)` |
| **Graphs.HasFactsToAdd** | формула | `CountOfFactsToAdd > 0` |
| **Graphs.NextFactSortOrder** | свёртка | `Min(Statements.SortOrder via Graph)` |
| **Graphs.MaxActivationSeq** | свёртка | `Max(Statements.ActivationSeq via Graph)` |
| **Graphs.NextActivationSeq** | формула | `MaxActivationSeq + 1` |
| **Graphs.MaxListedSortOrder** | свёртка | `Max(Statements.SortOrder via Graph)` |
| **Graphs.NextListedSortOrder** | формула | `MaxListedSortOrder + 1` |
| **Graphs.MaxSortOrder** | свёртка | `Max(Statements.SortOrder via Graph)` |
| **Graphs.NextCandidateSortOrder** | формула | `MaxSortOrder + 1` |
| **Rules.Name** | формула | `Label` |
| **Rules.CountOfFirings** | свёртка | `Sum(Firings.FiredUnit via Rule)` |
| **FiringKinds.Name** | формула | `Wording` |
| **FiringKinds.RuleEnabled** | подстановка | `Lookup(Rules.IsEnabled via Rule)` |
| **Things.PortraitName** | подстановка | `Lookup(Portraits.DisplayName via Portrait)` |
| **Things.PortraitPage** | подстановка | `Lookup(Portraits.Page via Portrait)` |
| **Things.PortraitImage** | подстановка | `Lookup(Portraits.Image via Portrait)` |
| **Things.PortraitCrop** | подстановка | `Lookup(Portraits.Crop via Portrait)` |
| **Things.PortraitWidth** | подстановка | `Lookup(Portraits.Width via Portrait)` |
| **Things.PortraitHeight** | подстановка | `Lookup(Portraits.Height via Portrait)` |
| **Things.Name** | формула | `Label` |
| **Things.NodeKey** | формула | `ThingId` |
| **Things.CountOfChildren** | свёртка | `Sum(Statements.ParentUnit via Subject)` |
| **Things.CountOfGrandchildren** | свёртка | `Sum(Statements.GrandparentUnit via Subject)` |
| **Things.CountAsPlace** | свёртка | `Sum(Statements.PlaceUnit via ObjectThing)` |
| **Things.CountAsSubject** | свёртка | `Sum(Statements.HoldsUnit via Subject)` |
| **Things.CountAsObject** | свёртка | `Sum(Statements.HoldsUnit via ObjectThing)` |
| **Things.IsInGraph** | формула | `Or(CountAsSubject > 0, CountAsObject > 0)` |
| **Things.NodeKind** | формула | `If(Kind = "", "", If(Kind = "class", "class", If(CountAsPlace > 0, "place", "person")))` |
| **Portraits.Name** | формула | `DisplayName` |
| **PortraitAliases.Name** | формула | `PortraitAliasId` |
| **Properties.Name** | формула | `Label` |
| **PropertyAxioms.Name** | формула | `PropertyAxiomId` |
| **Statements.SubjectLabel** | подстановка | `Lookup(Things.Label via Subject)` |
| **Statements.PropertyLabel** | подстановка | `Lookup(Properties.Label via Property)` |
| **Statements.ObjectLabel** | подстановка | `Lookup(Things.Label via ObjectThing)` |
| **Statements.PropertyDatatype** | подстановка | `Lookup(Properties.Datatype via Property)` |
| **Statements.PropertyHasOneCurrentValue** | подстановка | `Lookup(Properties.HasOneCurrentValue via Property)` |
| **Statements.PropertyDomainClass** | подстановка | `Lookup(Properties.DomainClass via Property)` |
| **Statements.PropertyRangeClass** | подстановка | `Lookup(Properties.RangeClass via Property)` |
| **Statements.LiteralQuote** | подстановка | `Lookup(Graphs.LiteralQuote via Graph)` |
| **Statements.LiteralTypeMarker** | подстановка | `Lookup(Graphs.LiteralTypeMarker via Graph)` |
| **Statements.BuiltInSourceText** | подстановка | `Lookup(Graphs.BuiltInSourceText via Graph)` |
| **Statements.UserSourceText** | подстановка | `Lookup(Graphs.UserSourceText via Graph)` |
| **Statements.NextFactSortOrder** | подстановка | `Lookup(Graphs.NextFactSortOrder via Graph)` |
| **Statements.IsLiteral** | формула | `ObjectThing = ""` |
| **Statements.ObjectText** | формула | `If(ObjectThing = "", ObjectValue, ObjectLabel)` |
| **Statements.Name** | формула | `Concat(SubjectLabel, " ", PropertyLabel, " ", ObjectText)` |
| **Statements.Words** | формула | `Concat(SubjectLabel, " ", PropertyLabel, " ", If(ObjectThing = "", Concat(LiteralQuote, ObjectValue, LiteralQuote, LiteralTypeMarker, PropertyDatatype), ObjectLabel))` |
| **Statements.Source** | формула | `If(IsBuiltIn, BuiltInSourceText, UserSourceText)` |
| **Statements.SubjectPropertyKey** | формула | `Concat(Subject, "\|", Property)` |
| **Statements.LatestSiblingActivationSeq** | свёртка | `Max(Statements.ActivationSeq via SubjectPropertyKey)` |
| **Statements.IsInEffect** | формула | `And(IsStated, Or(Not(PropertyHasOneCurrentValue), ActivationSeq = LatestSiblingActivationSeq))` |
| **Statements.ListedUnit** | формула | `If(IsListed, 1, 0)` |
| **Statements.InEffectUnit** | формула | `If(IsInEffect, 1, 0)` |
| **Statements.ListedOffUnit** | формула | `If(And(IsListed, Not(IsInEffect)), 1, 0)` |
| **Statements.IsNextToAdd** | формула | `And(ListedOffUnit = 1, SortOrder = NextFactSortOrder)` |
| **Statements.CountOfInverseFirings** | свёртка | `Sum(Firings.InverseFiredUnit via Target)` |
| **Statements.CountOfSymmetricFirings** | свёртка | `Sum(Firings.SymmetricFiredUnit via Target)` |
| **Statements.HoldsBeforeChain** | формула | `If(Or(IsInEffect, CountOfInverseFirings > 0, CountOfSymmetricFirings > 0), True())` |
| **Statements.CountOfChainFirings** | свёртка | `Sum(Firings.ChainFiredUnit via Target)` |
| **Statements.HoldsBeforeTyping** | формула | `If(Or(HoldsBeforeChain, CountOfChainFirings > 0), True())` |
| **Statements.CountOfTypeFirings** | свёртка | `Sum(Firings.TypeFiredUnit via Target)` |
| **Statements.Holds** | формула | `If(Or(HoldsBeforeTyping, CountOfTypeFirings > 0), True())` |
| **Statements.IsConclusion** | формула | `If(And(Holds, Not(IsInEffect)), True())` |
| **Statements.HoldsUnit** | формула | `If(Holds, 1, 0)` |
| **Statements.ConclusionUnit** | формула | `If(IsConclusion, 1, 0)` |
| **Statements.ParentUnit** | формула | `If(And(Holds, Property = "isParentOf"), 1, 0)` |
| **Statements.GrandparentUnit** | формула | `If(And(Holds, Property = "isGrandparentOf"), 1, 0)` |
| **Statements.PlaceUnit** | формула | `If(And(Holds, Property = "attends"), 1, 0)` |
| **Statements.FirstPreChainOrderKey** | свёртка | `Min(Firings.PreChainOrderKey via Target)` |
| **Statements.SnapPosition** | формула | `If(IsInEffect, SortOrder, If(HoldsBeforeChain, 100000 + FirstPreChainOrderKey, 0))` |
| **Statements.FirstFiredOrderKey** | свёртка | `Min(Firings.OrderKey via Target)` |
| **Statements.CountOfChosenFirings** | свёртка | `Sum(Firings.ChosenUnit via Target)` |
| **Statements.SubjectNodeKey** | формула | `Subject` |
| **Statements.ObjectNodeKey** | формула | `If(ObjectThing = "", Concat("literal\|", ObjectValue, "\|", PropertyDatatype), ObjectThing)` |
| **Statements.SubjectNodeKind** | подстановка | `Lookup(Things.NodeKind via Subject)` |
| **Statements.ObjectThingNodeKind** | подстановка | `Lookup(Things.NodeKind via ObjectThing)` |
| **Statements.ObjectNodeKind** | формула | `If(ObjectThing = "", "value", ObjectThingNodeKind)` |
| **Statements.ObjectValueNodeLabel** | подстановка | `Lookup(Properties.ValueNodeLabel via Property)` |
| **Statements.CountOfViolationsAsFirst** | свёртка | `Sum(DisjointChecks.ViolationUnit via First)` |
| **Statements.CountOfViolationsAsSecond** | свёртка | `Sum(DisjointChecks.ViolationUnit via Second)` |
| **Statements.IsInContradiction** | формула | `Or(CountOfViolationsAsFirst > 0, CountOfViolationsAsSecond > 0)` |
| **Firings.RuleEnabled** | подстановка | `Lookup(Rules.IsEnabled via Rule)` |
| **Firings.KindWording** | подстановка | `Lookup(FiringKinds.Wording via Kind)` |
| **Firings.KindSortOrder** | подстановка | `Lookup(FiringKinds.SortOrder via Kind)` |
| **Firings.TargetName** | подстановка | `Lookup(Statements.Name via Target)` |
| **Firings.TargetSubjectLabel** | подстановка | `Lookup(Statements.SubjectLabel via Target)` |
| **Firings.TargetPropertyLabel** | подстановка | `Lookup(Statements.PropertyLabel via Target)` |
| **Firings.TargetObjectText** | подстановка | `Lookup(Statements.ObjectText via Target)` |
| **Firings.TargetIsConclusion** | подстановка | `Lookup(Statements.IsConclusion via Target)` |
| **Firings.TargetSubjectNodeKey** | подстановка | `Lookup(Statements.SubjectNodeKey via Target)` |
| **Firings.TargetObjectNodeKey** | подстановка | `Lookup(Statements.ObjectNodeKey via Target)` |
| **Firings.TargetFirstFiredOrderKey** | подстановка | `Lookup(Statements.FirstFiredOrderKey via Target)` |
| **Firings.FirstWords** | подстановка | `Lookup(Statements.Words via First)` |
| **Firings.SecondWords** | подстановка | `Lookup(Statements.Words via Second)` |
| **Firings.FirstIsInEffect** | подстановка | `Lookup(Statements.IsInEffect via First)` |
| **Firings.FirstHoldsBeforeChain** | подстановка | `Lookup(Statements.HoldsBeforeChain via First)` |
| **Firings.SecondHoldsBeforeChain** | подстановка | `Lookup(Statements.HoldsBeforeChain via Second)` |
| **Firings.FirstHoldsBeforeTyping** | подстановка | `Lookup(Statements.HoldsBeforeTyping via First)` |
| **Firings.FirstProperty** | подстановка | `Lookup(Statements.Property via First)` |
| **Firings.FirstPropertyDomainClass** | подстановка | `Lookup(Statements.PropertyDomainClass via First)` |
| **Firings.FirstPropertyRangeClass** | подстановка | `Lookup(Statements.PropertyRangeClass via First)` |
| **Firings.FirstSortOrder** | подстановка | `Lookup(Statements.SortOrder via First)` |
| **Firings.SecondIsInEffect** | подстановка | `Lookup(Statements.IsInEffect via Second)` |
| **Firings.FirstSnapPosition** | подстановка | `Lookup(Statements.SnapPosition via First)` |
| **Firings.SecondSnapPosition** | подстановка | `Lookup(Statements.SnapPosition via Second)` |
| **Firings.InverseFiredUnit** | формула | `If(And(RuleEnabled, Kind = "inverse", FirstIsInEffect), 1, 0)` |
| **Firings.SymmetricFiredUnit** | формула | `If(And(RuleEnabled, Kind = "symmetric", FirstIsInEffect), 1, 0)` |
| **Firings.ChainFiredUnit** | формула | `If(And(RuleEnabled, Kind = "chain", FirstHoldsBeforeChain, SecondHoldsBeforeChain), 1, 0)` |
| **Firings.TypeFiredUnit** | формула | `If(And(RuleEnabled, Or(Kind = "domain", Kind = "range"), FirstHoldsBeforeTyping), 1, 0)` |
| **Firings.FiredUnit** | формула | `InverseFiredUnit + SymmetricFiredUnit + ChainFiredUnit + TypeFiredUnit` |
| **Firings.PreChainFiredUnit** | формула | `InverseFiredUnit + SymmetricFiredUnit` |
| **Firings.PreChainOrderKey** | формула | `FirstSortOrder * 10 + KindSortOrder` |
| **Firings.Round** | формула | `If(And(Kind = "chain", Not(And(FirstIsInEffect, SecondIsInEffect))), 2, 1)` |
| **Firings.OrderKey** | формула | `Round * 1000000000000000 + FirstSnapPosition * 100000000 + KindSortOrder * 10000000 + If(Second = "", 0, SecondSnapPosition)` |
| **Firings.IsChosen** | формула | `And(FiredUnit = 1, TargetIsConclusion, OrderKey = TargetFirstFiredOrderKey)` |
| **Firings.ChosenUnit** | формула | `If(IsChosen, 1, 0)` |
| **Firings.CardText** | формула | `If(IsChosen, Concat("Worked out by the ", KindWording, " from “", FirstWords, "”", If(Kind = "chain", Concat(" and “", SecondWords, "”"), If(Kind = "domain", Concat(" and “", FirstProperty, " has domain ", FirstPropertyDomainClass, "”"), If(Kind = "range", Concat(" and “", FirstProperty, " has range ", FirstPropertyRangeClass, "”"), ""))), "."), "")` |
| **Firings.Name** | формула | `FiringId` |
| **DisjointChecks.RuleEnabled** | подстановка | `Lookup(Rules.IsEnabled via Rule)` |
| **DisjointChecks.FirstHolds** | подстановка | `Lookup(Statements.Holds via First)` |
| **DisjointChecks.SecondHolds** | подстановка | `Lookup(Statements.Holds via Second)` |
| **DisjointChecks.SubjectLabel** | подстановка | `Lookup(Statements.SubjectLabel via First)` |
| **DisjointChecks.ObjectLabel** | подстановка | `Lookup(Statements.ObjectLabel via First)` |
| **DisjointChecks.ViolationUnit** | формула | `If(And(RuleEnabled, FirstHolds, SecondHolds), 1, 0)` |
| **DisjointChecks.AlarmText** | формула | `If(ViolationUnit = 1, Concat("Contradiction found. ", SubjectLabel, " is recorded as both parent and spouse of ", ObjectLabel, ". Check those facts and the selected disjoint rule."), "")` |
| **DisjointChecks.Name** | формула | `DisjointCheckId` |
| **Scenarios.Name** | формула | `Label` |
| **Scenarios.GraphHasFactsToAdd** | подстановка | `Lookup(Graphs.HasFactsToAdd via Graph)` |
| **Scenarios.IsAvailable** | формула | `Or(FactBaseline <> "next", GraphHasFactsToAdd)` |
| **ScenarioFactChanges.Name** | формула | `ScenarioFactChangeId` |
| **ScenarioRuleSettings.Name** | формула | `ScenarioRuleSettingId` |
| **NodeLayouts.Name** | формула | `NodeLayoutId` |
| **LayoutSlots.Name** | формула | `LayoutSlotId` |
| **PageTexts.Name** | формула | `PageTextId` |
| **Engines.Name** | формула | `Label` |
| **RulebookTools.Name** | формула | `Label` |

---

> **RuleSpeak®** — декларативная нотация бизнес-правил, созданная **Рональдом Г. Россом** по соглашениям **SBVR**, — скоро появится. А пока эта страница показывает обычную документацию. [www.RonRoss.info](https://www.RonRoss.info)
