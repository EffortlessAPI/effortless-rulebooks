# 📘 South Park ontology explainer — Документация

_The barebones South Park ontology from Jessica Talisman's explainer: ten stated facts and five rules that can each be switched on. Conclusions are formulas._

> Декларативные бизнес-правила, построенные из книги правил. Каждое утверждение
> выражает истину предметной области — это не процедура и не императив.
> Формулы книги правил — единственный источник истины; этот документ — их
> прочтение на простом языке.

## 1 Бизнес-словарь

| Термин | Описание | Пояснение |
|------|-------------|-------------------|
| **Graph** | The whole example. | — |
| Name | Совпадает с graph ID. | — |
| Count of Stated Facts | Сумма «stated unit» по записям «statements», связанным с graph. | — |
| Count of Conclusions | Сумма «conclusion unit» по записям «statements», связанным с graph. | _Facts that hold but were never stated._ |
| Count of Violations | Сумма «violation unit» по записям «disjoint checks», связанным с graph. | — |
| Is Consistent | Истинно, когда count of violations равно 0. | — |

<details open>
<summary>Примеры данных</summary>

| Name ƒ | Count of Stated Facts ƒ | Count of Conclusions ƒ | Count of Violations ƒ | Is Consistent ƒ |
|---|---|---|---|---|
| south-park | 10 | 8 | 0 | true |

_ƒ обозначает вычисляемый столбец._

</details>

| Термин | Описание | Пояснение |
|------|-------------|-------------------|
| **Rule** | The five rules; IsEnabled is the checkbox. | — |
| Label | Определённый атрибут. | — |
| Explanation | Определённый атрибут. | — |
| Is Enabled | Истинно, когда пустая строка. | — |
| Name | Совпадает с label. | — |
| Count of Firings | Сумма «fired unit» по записям «firings», связанным с rule. | _How many times the rule produced a fact._ |

<details open>
<summary>Примеры данных</summary>

| Label | Explanation | Is Enabled | Name ƒ | Count of Firings ƒ |
|---|---|---|---|---|
| Inverse | A parent link also gives a child link between the same two people. | true | Inverse | 3 |
| Symmetric | A friend link or a spouse link is true for both people. | true | Symmetric | 2 |
| Property chain | A parent of a parent is a grandparent. | true | Property chain | 2 |

_ƒ обозначает вычисляемый столбец._

</details>

| Термин | Описание | Пояснение |
|------|-------------|-------------------|
| **Thing** | A node in the picture: a person, a place, a class or a role value. | — |
| Label | Определённый атрибут. | — |
| Kind | Определённый атрибут. | — |
| Name | Совпадает с label. | — |
| Count of Children | Сумма «parent unit» по записям «statements», связанным с thing. | _Added: a count, which a reasoner alone does not give._ |
| Count of Grandchildren | Сумма «grandparent unit» по записям «statements», связанным с thing. | _Added: counts concluded grandchildren._ |

<details open>
<summary>Примеры данных</summary>

| Label | Kind | Name ƒ | Count of Children ƒ | Count of Grandchildren ƒ |
|---|---|---|---|---|
| Marvin | individual | Marvin | 1 | 2 |
| Randy | individual | Randy | 2 | 0 |
| Sharon | individual | Sharon | 0 | 0 |

_ƒ обозначает вычисляемый столбец._

</details>

| Термин | Описание | Пояснение |
|------|-------------|-------------------|
| **Property** | A kind of link, with the characteristics the rules use. | — |
| Label | Определённый атрибут. | — |
| Inverse of | Определённый атрибут. | — |
| Is Symmetric | Истинно, когда пустая строка. | — |
| Chain First | Определённый атрибут. | — |
| Chain Second | Определённый атрибут. | — |
| Domain Class | Определённый атрибут. | — |
| Range Class | Определённый атрибут. | — |
| Disjoint With | Определённый атрибут. | — |
| Name | Совпадает с label. | — |

<details open>
<summary>Примеры данных</summary>

| Label | Is Symmetric | Name ƒ |
|---|---|---|
| is parent of | false | is parent of |
| is child of | false | is child of |
| is spouse of | true | is spouse of |

_ƒ обозначает вычисляемый столбец._

</details>

| Термин | Описание | Пояснение |
|------|-------------|-------------------|
| **Statement** | A fact that is stated, or could be concluded: Subject, Property, Object. | — |
| Graph | Определённый атрибут. | — |
| Subject | Определённый атрибут. | — |
| Property | Определённый атрибут. | — |
| Object Thing | Определённый атрибут. | — |
| Number Value | Определённый атрибут. | — |
| Is Stated | Истинно, когда пустая строка. | _Drawn as a solid line in the explainer._ |
| Subject Label | Берётся из связанного «subject». | — |
| Property Label | Берётся из связанного «property». | — |
| Object Label | Берётся из связанного «object thing». | — |
| Name | Вычисляется как subject label, затем пробел, затем property label, затем пробел, затем number value, если object thing пусто, во всех остальных случаях — object label. | — |
| Count of Inverse Firings | Сумма «inverse fired unit» по записям «firings», связанным с statement. | — |
| Count of Symmetric Firings | Сумма «symmetric fired unit» по записям «firings», связанным с statement. | — |
| Holds Before Chain | Истинно, когда выполняется хотя бы одно из следующего: признак «stated» установлен; count of inverse firings больше 0; или count of symmetric firings больше 0. | — |
| Count of Chain Firings | Сумма «chain fired unit» по записям «firings», связанным с statement. | — |
| Holds Before Typing | Истинно, когда выполняется хотя бы одно из следующего: признак «holds before chain» установлен или count of chain firings больше 0. | — |
| Count of Type Firings | Сумма «type fired unit» по записям «firings», связанным с statement. | — |
| Holds | Истинно, когда выполняется хотя бы одно из следующего: признак «holds before typing» установлен или count of type firings больше 0. | _True when stated or concluded by an enabled rule._ |
| Is Conclusion | Истинно, когда выполняется всё следующее: признак «holds» установлен и признак «stated» не установлен. | _Drawn as a dashed line in the explainer._ |
| Worked Out by | Определяется по приоритету: пустая строка, если признак «conclusion» не установлен; «inverse», если count of inverse firings больше 0; «symmetric», если count of symmetric firings больше 0; «property chain», если count of chain firings больше 0; во всех остальных случаях — «domain and range». | — |
| Stated Unit | Определяется по приоритету: 1, если признак «stated» установлен; во всех остальных случаях — 0. | — |
| Conclusion Unit | Определяется по приоритету: 1, если признак «conclusion» установлен; во всех остальных случаях — 0. | — |
| Parent Unit | Определяется по приоритету: 1, если выполняется всё следующее: признак «holds» установлен и property равно «isParentOf»; во всех остальных случаях — 0. | — |
| Grandparent Unit | Определяется по приоритету: 1, если выполняется всё следующее: признак «holds» установлен и property равно «isGrandparentOf»; во всех остальных случаях — 0. | — |

<details open>
<summary>Примеры данных</summary>

| Number Value | Is Stated | Subject Label ƒ | Property Label ƒ | Object Label ƒ | Name ƒ | Count of Inverse Firings ƒ | Count of Symmetric Firings ƒ | Holds Before Chain ƒ | Count of Chain Firings ƒ | Holds Before Typing ƒ | Count of Type Firings ƒ | Holds ƒ | Is Conclusion ƒ | Worked Out by ƒ | Stated Unit ƒ | Conclusion Unit ƒ | Parent Unit ƒ | Grandparent Unit ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| — | true | Marvin | is parent of | Randy | Marvin is parent of Randy | 0 | 0 | true | 0 | true | 0 | true | false | — | 1 | 0 | 1 | 0 |
| — | false | Marvin | is parent of | Sharon | Marvin is parent of Sharon | 0 | 0 | false | 0 | false | 0 | false | false | — | 0 | 0 | 0 | 0 |
| — | false | Marvin | is parent of | Shelly | Marvin is parent of Shelly | 0 | 0 | false | 0 | false | 0 | false | false | — | 0 | 0 | 0 | 0 |

_ƒ обозначает вычисляемый столбец._

</details>

| Термин | Описание | Пояснение |
|------|-------------|-------------------|
| **Firing** | One way one rule could produce one fact from one or two others. | — |
| Rule | Определённый атрибут. | — |
| Kind | Определённый атрибут. | — |
| Target | Определённый атрибут. | — |
| First | Определённый атрибут. | — |
| Second | Определённый атрибут. | _Same as First when the rule needs one premise._ |
| Rule Enabled | Истинно, когда связанный «rule» является «enabled». | — |
| Rule Label | Берётся из связанного «rule». | — |
| Target Name | Берётся из связанного «target». | — |
| First Name | Берётся из связанного «first». | — |
| Second Name | Берётся из связанного «second». | — |
| First is Stated | Истинно, когда связанный «first» является «stated». | — |
| First Holds Before Chain | Истинно, когда связанный «first» является «holds before chain». | — |
| Second Holds Before Chain | Истинно, когда связанный «second» является «holds before chain». | — |
| First Holds Before Typing | Истинно, когда связанный «first» является «holds before typing». | — |
| Inverse Fired Unit | Определяется по приоритету: 1, если выполняется всё следующее: признак «rule enabled» установлен; kind равно «inverse»; и признак «first is stated» установлен; во всех остальных случаях — 0. | — |
| Symmetric Fired Unit | Определяется по приоритету: 1, если выполняется всё следующее: признак «rule enabled» установлен; kind равно «symmetric»; и признак «first is stated» установлен; во всех остальных случаях — 0. | — |
| Chain Fired Unit | Определяется по приоритету: 1, если выполняется всё следующее: признак «rule enabled» установлен; kind равно «chain»; признак «first holds before chain» установлен; и признак «second holds before chain» установлен; во всех остальных случаях — 0. | — |
| Type Fired Unit | Определяется по приоритету: 1, если выполняется всё следующее: признак «rule enabled» установлен; выполняется хотя бы одно из следующего: kind равно «domain» или kind равно «range»; и признак «first holds before typing» установлен; во всех остальных случаях — 0. | — |
| Fired Unit | Вычисляется как inverse fired unit плюс symmetric fired unit плюс chain fired unit плюс type fired unit. | — |
| Name | Определяется по приоритету: target name, затем «: worked out by the », затем rule label, затем « rule from '», затем first name, затем «'», затем « and '», затем second name, затем «'», если fired unit больше 0; во всех остальных случаях — firing ID. | _The explainer's conclusion card._ |

<details open>
<summary>Примеры данных</summary>

| Kind | Rule Enabled ƒ | Rule Label ƒ | Target Name ƒ | First Name ƒ | Second Name ƒ | First is Stated ƒ | First Holds Before Chain ƒ | Second Holds Before Chain ƒ | First Holds Before Typing ƒ | Inverse Fired Unit ƒ | Symmetric Fired Unit ƒ | Chain Fired Unit ƒ | Type Fired Unit ƒ | Fired Unit ƒ | Name ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| inverse | true | Inverse | Randy is child of Marvin | Marvin is parent of Randy | Marvin is parent of Randy | true | true | true | true | 1 | 0 | 0 | 0 | 1 | Randy is child of Marvin: worked out by the Inverse rule from 'Marvin is parent of Randy' |
| inverse | true | Inverse | Sharon is child of Marvin | Marvin is parent of Sharon | Marvin is parent of Sharon | false | false | false | false | 0 | 0 | 0 | 0 | 0 | inverse:Marvin\|isParentOf\|Sharon>Sharon\|isChildOf\|Marvin |
| inverse | true | Inverse | Shelly is child of Marvin | Marvin is parent of Shelly | Marvin is parent of Shelly | false | false | false | false | 0 | 0 | 0 | 0 | 0 | inverse:Marvin\|isParentOf\|Shelly>Shelly\|isChildOf\|Marvin |

_ƒ обозначает вычисляемый столбец._

</details>

| Термин | Описание | Пояснение |
|------|-------------|-------------------|
| **Disjoint Check** | Two statements that must not both hold. | — |
| Graph | Определённый атрибут. | — |
| Rule | Определённый атрибут. | — |
| A | Определённый атрибут. | — |
| B | Определённый атрибут. | — |
| Name | Совпадает с disjoint check ID. | — |
| Rule Enabled | Истинно, когда связанный «rule» является «enabled». | — |
| A Holds | Истинно, когда связанный «a» является «holds». | — |
| B Holds | Истинно, когда связанный «b» является «holds». | — |
| Violation Unit | Определяется по приоритету: 1, если выполняется всё следующее: признак «rule enabled» установлен; признак «a holds» установлен; и признак «b holds» установлен; во всех остальных случаях — 0. | — |

<details open>
<summary>Примеры данных</summary>

| Name ƒ | Rule Enabled ƒ | A Holds ƒ | B Holds ƒ | Violation Unit ƒ |
|---|---|---|---|---|
| Marvin\|isParentOf\|Randy~isSpouseOf | true | true | false | 0 |
| Marvin\|isParentOf\|Sharon~isSpouseOf | true | false | false | 0 |
| Marvin\|isParentOf\|Shelly~isSpouseOf | true | false | false | 0 |

_ƒ обозначает вычисляемый столбец._

</details>

## 2 Типы фактов

- **property** может ссылаться на один **property**
- **property** может ссылаться на один **thing**
- **statement** ссылается ровно на один **graph**
- **statement** ссылается ровно на один **thing**
- **statement** ссылается ровно на один **property**
- **firing** ссылается ровно на один **rule**
- **firing** ссылается ровно на один **statement**
- **disjoint check** ссылается ровно на один **graph**
- **disjoint check** ссылается ровно на один **rule**
- **disjoint check** ссылается ровно на один **statement**

## 3 Оперативные правила

_Оперативные правила формулируют, что бизнес **обязывает**, **запрещает** или
рекомендует (**следует**). Структурные правила берутся из обязательных полей и внешних
ключей; семантические — из таблицы Constraints, каждое опирается на булево значение,
которое книга правил уже вычисляет (со ссылкой DR-N в определяющих правилах ниже)._

### Структурные ограничения (из схемы)

- Rule **обязан** иметь label и explanation, а также фиксировать то, он является «enabled» ли.
- Thing **обязан** иметь label и kind.
- Property **обязан** иметь label, а также фиксировать то, он является «symmetric» ли.
- Statement **обязан** ссылаться ровно на один «graph».
- Statement **обязан** ссылаться ровно на один «thing» в роли «subject».
- Statement **обязан** ссылаться ровно на один «property».
- Statement **обязан** фиксировать то, он является «stated» ли.
- Firing **обязан** ссылаться ровно на один «rule».
- Firing **обязан** ссылаться ровно на один «statement» в роли «target».
- Firing **обязан** ссылаться ровно на один «statement» в роли «first».
- Firing **обязан** ссылаться ровно на один «statement» в роли «second».
- Firing **обязан** иметь kind.
- Disjoint check **обязан** ссылаться ровно на один «graph».
- Disjoint check **обязан** ссылаться ровно на один «rule».
- Disjoint check **обязан** ссылаться ровно на один «statement» в роли «a».
- Disjoint check **обязан** ссылаться ровно на один «statement» в роли «b».

## 4 Определяющие правила

_Все утверждения выражают истины предметной области; это не процедуры и не
императивы. Вместо «тогда и только тогда» используется «только если», чтобы
одностороннюю необходимость не приняли за эквивалентность. Пометка
**⚠︎ механически** указывает правило, чья детерминированная формулировка верна,
но тяжеловесна — сигнал для необязательной переформулировки, а не дефект._

| ID | Декларативное правило |
|----|------------------|
| **DR-1 Name** | Graph: «name» совпадает с graph ID. |
| **DR-2 Count of Stated Facts** | Graph: «count of stated facts» — это сумма «stated unit» по записям «statements», связанным с graph. |
| **DR-3 Count of Conclusions** | Graph: «count of conclusions» — это сумма «conclusion unit» по записям «statements», связанным с graph. |
| **DR-4 Count of Violations** | Graph: «count of violations» — это сумма «violation unit» по записям «disjoint checks», связанным с graph. |
| **DR-5 Is Consistent** | Graph считается «consistent», если count of violations равно 0. |
| **DR-6 Name** | Rule: «name» совпадает с label. |
| **DR-7 Count of Firings** | Rule: «count of firings» — это сумма «fired unit» по записям «firings», связанным с rule. |
| **DR-8 Name** | Thing: «name» совпадает с label. |
| **DR-9 Count of Children** | Thing: «count of children» — это сумма «parent unit» по записям «statements», связанным с thing. |
| **DR-10 Count of Grandchildren** | Thing: «count of grandchildren» — это сумма «grandparent unit» по записям «statements», связанным с thing. |
| **DR-11 Name** | Property: «name» совпадает с label. |
| **DR-12 Subject Label** | Statement: «subject label» — берётся из связанного «subject». |
| **DR-13 Property Label** | Statement: «property label» — берётся из связанного «property». |
| **DR-14 Object Label** | Statement: «object label» — берётся из связанного «object thing». |
| **DR-15 Name** | Statement: «name» вычисляется как subject label, затем пробел, затем property label, затем пробел, затем number value, если object thing пусто, во всех остальных случаях — object label. |
| **DR-16 Count of Inverse Firings** | Statement: «count of inverse firings» — это сумма «inverse fired unit» по записям «firings», связанным с statement. |
| **DR-17 Count of Symmetric Firings** | Statement: «count of symmetric firings» — это сумма «symmetric fired unit» по записям «firings», связанным с statement. |
| **DR-18 Holds Before Chain** | Statement считается удовлетворяющим условию «before chain», если выполняется хотя бы одно из следующего: признак «stated» установлен; count of inverse firings больше 0; или count of symmetric firings больше 0. |
| **DR-19 Count of Chain Firings** | Statement: «count of chain firings» — это сумма «chain fired unit» по записям «firings», связанным с statement. |
| **DR-20 Holds Before Typing** | Statement считается удовлетворяющим условию «before typing», если выполняется хотя бы одно из следующего: признак «holds before chain» установлен или count of chain firings больше 0. |
| **DR-21 Count of Type Firings** | Statement: «count of type firings» — это сумма «type fired unit» по записям «firings», связанным с statement. |
| **DR-22 Holds** | Statement считается удовлетворяющим условию «», если выполняется хотя бы одно из следующего: признак «holds before typing» установлен или count of type firings больше 0. |
| **DR-23 Is Conclusion** | Statement считается «conclusion», если выполняется всё следующее: признак «holds» установлен и признак «stated» не установлен. |
| **DR-24 Worked Out by** | Поле «worked out by» (Statement) определяется по следующему приоритету:<br>1. пустая строка, если признак «conclusion» не установлен;<br>2. «inverse», если count of inverse firings больше 0;<br>3. «symmetric», если count of symmetric firings больше 0;<br>4. «property chain», если count of chain firings больше 0;<br>5. во всех остальных случаях — «domain and range». |
| **DR-25 Stated Unit** | Поле «stated unit» (Statement) определяется по следующему приоритету:<br>1. 1, если признак «stated» установлен;<br>2. во всех остальных случаях — 0. |
| **DR-26 Conclusion Unit** | Поле «conclusion unit» (Statement) определяется по следующему приоритету:<br>1. 1, если признак «conclusion» установлен;<br>2. во всех остальных случаях — 0. |
| **DR-27 Parent Unit** | Поле «parent unit» (Statement) определяется по следующему приоритету:<br>1. 1, если выполняется всё следующее: признак «holds» установлен и property равно «isParentOf»;<br>2. во всех остальных случаях — 0. |
| **DR-28 Grandparent Unit** | Поле «grandparent unit» (Statement) определяется по следующему приоритету:<br>1. 1, если выполняется всё следующее: признак «holds» установлен и property равно «isGrandparentOf»;<br>2. во всех остальных случаях — 0. |
| **DR-29 Rule Enabled** | Firing: «rule enabled» истинно, когда связанный «rule» является «enabled». |
| **DR-30 Rule Label** | Firing: «rule label» — берётся из связанного «rule». |
| **DR-31 Target Name** | Firing: «target name» — берётся из связанного «target». |
| **DR-32 First Name** | Firing: «first name» — берётся из связанного «first». |
| **DR-33 Second Name** | Firing: «second name» — берётся из связанного «second». |
| **DR-34 First is Stated** | Firing: «first is stated» истинно, когда связанный «first» является «stated». |
| **DR-35 First Holds Before Chain** | Firing: «first holds before chain» истинно, когда связанный «first» является «holds before chain». |
| **DR-36 Second Holds Before Chain** | Firing: «second holds before chain» истинно, когда связанный «second» является «holds before chain». |
| **DR-37 First Holds Before Typing** | Firing: «first holds before typing» истинно, когда связанный «first» является «holds before typing». |
| **DR-38 Inverse Fired Unit** | Поле «inverse fired unit» (Firing) определяется по следующему приоритету:<br>1. 1, если выполняется всё следующее: признак «rule enabled» установлен; kind равно «inverse»; и признак «first is stated» установлен;<br>2. во всех остальных случаях — 0. |
| **DR-39 Symmetric Fired Unit** | Поле «symmetric fired unit» (Firing) определяется по следующему приоритету:<br>1. 1, если выполняется всё следующее: признак «rule enabled» установлен; kind равно «symmetric»; и признак «first is stated» установлен;<br>2. во всех остальных случаях — 0. |
| **DR-40 Chain Fired Unit** | Поле «chain fired unit» (Firing) определяется по следующему приоритету:<br>1. 1, если выполняется всё следующее: признак «rule enabled» установлен; kind равно «chain»; признак «first holds before chain» установлен; и признак «second holds before chain» установлен;<br>2. во всех остальных случаях — 0. |
| **DR-41 Type Fired Unit** | Поле «type fired unit» (Firing) определяется по следующему приоритету:<br>1. 1, если выполняется всё следующее: признак «rule enabled» установлен; выполняется хотя бы одно из следующего: kind равно «domain» или kind равно «range»; и признак «first holds before typing» установлен;<br>2. во всех остальных случаях — 0. |
| **DR-42 Fired Unit** | Firing: «fired unit» вычисляется как inverse fired unit плюс symmetric fired unit плюс chain fired unit плюс type fired unit. |
| **DR-43 Name** | Поле «name» (Firing) определяется по следующему приоритету:<br>1. target name, затем «: worked out by the », затем rule label, затем « rule from '», затем first name, затем «'», затем « and '», затем second name, затем «'», если fired unit больше 0;<br>2. во всех остальных случаях — firing ID. |
| **DR-44 Name** | Disjoint check: «name» совпадает с disjoint check ID. |
| **DR-45 Rule Enabled** | Disjoint check: «rule enabled» истинно, когда связанный «rule» является «enabled». |
| **DR-46 A Holds** | Disjoint check: «a holds» истинно, когда связанный «a» является «holds». |
| **DR-47 B Holds** | Disjoint check: «b holds» истинно, когда связанный «b» является «holds». |
| **DR-48 Violation Unit** | Поле «violation unit» (Disjoint check) определяется по следующему приоритету:<br>1. 1, если выполняется всё следующее: признак «rule enabled» установлен; признак «a holds» установлен; и признак «b holds» установлен;<br>2. во всех остальных случаях — 0. |

## 5 Прослеживаемость к схеме

_Колонка выражения — определение правила в нотации Документация:
та же логика, что хранит книга правил, записанная для бизнес-читателя._

| Элемент схемы | Вид | Выражение |
|----------------|------|------------|
| **Graphs.Name** | формула | `GraphId` |
| **Graphs.CountOfStatedFacts** | свёртка | `Sum(Statements.StatedUnit via Graph)` |
| **Graphs.CountOfConclusions** | свёртка | `Sum(Statements.ConclusionUnit via Graph)` |
| **Graphs.CountOfViolations** | свёртка | `Sum(DisjointChecks.ViolationUnit via Graph)` |
| **Graphs.IsConsistent** | формула | `CountOfViolations = 0` |
| **Rules.Name** | формула | `Label` |
| **Rules.CountOfFirings** | свёртка | `Sum(Firings.FiredUnit via Rule)` |
| **Things.Name** | формула | `Label` |
| **Things.CountOfChildren** | свёртка | `Sum(Statements.ParentUnit via Subject)` |
| **Things.CountOfGrandchildren** | свёртка | `Sum(Statements.GrandparentUnit via Subject)` |
| **Properties.Name** | формула | `Label` |
| **Statements.SubjectLabel** | подстановка | `Lookup(Things.Label via Subject)` |
| **Statements.PropertyLabel** | подстановка | `Lookup(Properties.Label via Property)` |
| **Statements.ObjectLabel** | подстановка | `Lookup(Things.Label via ObjectThing)` |
| **Statements.Name** | формула | `SubjectLabel & " " & PropertyLabel & " " & If(ObjectThing = Blank(), NumberValue, ObjectLabel)` |
| **Statements.CountOfInverseFirings** | свёртка | `Sum(Firings.InverseFiredUnit via Target)` |
| **Statements.CountOfSymmetricFirings** | свёртка | `Sum(Firings.SymmetricFiredUnit via Target)` |
| **Statements.HoldsBeforeChain** | формула | `Or(IsStated, CountOfInverseFirings > 0, CountOfSymmetricFirings > 0)` |
| **Statements.CountOfChainFirings** | свёртка | `Sum(Firings.ChainFiredUnit via Target)` |
| **Statements.HoldsBeforeTyping** | формула | `Or(HoldsBeforeChain, CountOfChainFirings > 0)` |
| **Statements.CountOfTypeFirings** | свёртка | `Sum(Firings.TypeFiredUnit via Target)` |
| **Statements.Holds** | формула | `Or(HoldsBeforeTyping, CountOfTypeFirings > 0)` |
| **Statements.IsConclusion** | формула | `And(Holds, Not(IsStated))` |
| **Statements.WorkedOutBy** | формула | `If(Not(IsConclusion), "", If(CountOfInverseFirings > 0, "inverse", If(CountOfSymmetricFirings > 0, "symmetric", If(CountOfChainFirings > 0, "property chain", "domain and range"))))` |
| **Statements.StatedUnit** | формула | `If(IsStated, 1, 0)` |
| **Statements.ConclusionUnit** | формула | `If(IsConclusion, 1, 0)` |
| **Statements.ParentUnit** | формула | `If(And(Holds, Property = "isParentOf"), 1, 0)` |
| **Statements.GrandparentUnit** | формула | `If(And(Holds, Property = "isGrandparentOf"), 1, 0)` |
| **Firings.RuleEnabled** | подстановка | `Lookup(Rules.IsEnabled via Rule)` |
| **Firings.RuleLabel** | подстановка | `Lookup(Rules.Label via Rule)` |
| **Firings.TargetName** | подстановка | `Lookup(Statements.Name via Target)` |
| **Firings.FirstName** | подстановка | `Lookup(Statements.Name via First)` |
| **Firings.SecondName** | подстановка | `Lookup(Statements.Name via Second)` |
| **Firings.FirstIsStated** | подстановка | `Lookup(Statements.IsStated via First)` |
| **Firings.FirstHoldsBeforeChain** | подстановка | `Lookup(Statements.HoldsBeforeChain via First)` |
| **Firings.SecondHoldsBeforeChain** | подстановка | `Lookup(Statements.HoldsBeforeChain via Second)` |
| **Firings.FirstHoldsBeforeTyping** | подстановка | `Lookup(Statements.HoldsBeforeTyping via First)` |
| **Firings.InverseFiredUnit** | формула | `If(And(RuleEnabled, Kind = "inverse", FirstIsStated), 1, 0)` |
| **Firings.SymmetricFiredUnit** | формула | `If(And(RuleEnabled, Kind = "symmetric", FirstIsStated), 1, 0)` |
| **Firings.ChainFiredUnit** | формула | `If(And(RuleEnabled, Kind = "chain", FirstHoldsBeforeChain, SecondHoldsBeforeChain), 1, 0)` |
| **Firings.TypeFiredUnit** | формула | `If(And(RuleEnabled, Or(Kind = "domain", Kind = "range"), FirstHoldsBeforeTyping), 1, 0)` |
| **Firings.FiredUnit** | формула | `InverseFiredUnit + SymmetricFiredUnit + ChainFiredUnit + TypeFiredUnit` |
| **Firings.Name** | формула | `If(FiredUnit > 0, TargetName & ": worked out by the " & RuleLabel & " rule from '" & FirstName & "'" & If(Kind = "chain", " and '" & SecondName & "'", ""), FiringId)` |
| **DisjointChecks.Name** | формула | `DisjointCheckId` |
| **DisjointChecks.RuleEnabled** | подстановка | `Lookup(Rules.IsEnabled via Rule)` |
| **DisjointChecks.AHolds** | подстановка | `Lookup(Statements.Holds via A)` |
| **DisjointChecks.BHolds** | подстановка | `Lookup(Statements.Holds via B)` |
| **DisjointChecks.ViolationUnit** | формула | `If(And(RuleEnabled, AHolds, BHolds), 1, 0)` |
