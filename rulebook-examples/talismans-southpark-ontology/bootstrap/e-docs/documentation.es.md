# 📘 South Park ontology explainer — Documentación

_The barebones South Park ontology from Jessica Talisman's explainer: ten stated facts and five rules that can each be switched on. Conclusions are formulas._

> Reglas de negocio declarativas generadas a partir del libro de reglas. Cada
> enunciado expresa una verdad del dominio de negocio — no es un procedimiento
> ni un imperativo. Las fórmulas del libro de reglas son la única fuente de la
> verdad; este documento es su lectura en lenguaje claro.

## 1 Vocabulario de negocio

| Término | Descripción | Comentario narrativo |
|------|-------------|-------------------|
| **Graph** | The whole example. | — |
| Name | Igual a graph ID. | — |
| Count of Stated Facts | El total de stated unit entre los statements relacionados con el graph. | — |
| Count of Conclusions | El total de conclusion unit entre los statements relacionados con el graph. | _Facts that hold but were never stated._ |
| Count of Violations | El total de violation unit entre los disjoint checks relacionados con el graph. | — |
| Is Consistent | Verdadero cuando el count of violations es 0. | — |

<details open>
<summary>Datos de ejemplo</summary>

| Name ƒ | Count of Stated Facts ƒ | Count of Conclusions ƒ | Count of Violations ƒ | Is Consistent ƒ |
|---|---|---|---|---|
| south-park | 11 | 9 | 0 | true |

_ƒ indica una columna calculada._

</details>

| Término | Descripción | Comentario narrativo |
|------|-------------|-------------------|
| **Rule** | The five rules; IsEnabled is the checkbox. | — |
| Label | Un atributo definido. | — |
| Explanation | Un atributo definido. | — |
| Is Enabled | Verdadero cuando una cadena vacía. | — |
| Name | Igual a label. | — |
| Count of Firings | El total de fired unit entre los firings relacionados con el rule. | _How many times the rule produced a fact._ |

<details open>
<summary>Datos de ejemplo</summary>

| Label | Explanation | Is Enabled | Name ƒ | Count of Firings ƒ |
|---|---|---|---|---|
| Inverse | A parent link also gives a child link between the same two people. | true | Inverse | 4 |
| Symmetric | A friend link or a spouse link is true for both people. | true | Symmetric | 2 |
| Property chain | A parent of a parent is a grandparent. | true | Property chain | 2 |

_ƒ indica una columna calculada._

</details>

| Término | Descripción | Comentario narrativo |
|------|-------------|-------------------|
| **Thing** | A node in the picture: a person, a place, a class or a role value. | — |
| Label | Un atributo definido. | — |
| Kind | Un atributo definido. | — |
| Name | Igual a label. | — |
| Count of Children | El total de parent unit entre los statements relacionados con el thing. | _Added: a count, which a reasoner alone does not give._ |
| Count of Grandchildren | El total de grandparent unit entre los statements relacionados con el thing. | _Added: counts concluded grandchildren._ |

<details open>
<summary>Datos de ejemplo</summary>

| Label | Kind | Name ƒ | Count of Children ƒ | Count of Grandchildren ƒ |
|---|---|---|---|---|
| Marvin | individual | Marvin | 1 | 2 |
| Randy | individual | Randy | 2 | 0 |
| Sharon | individual | Sharon | 1 | 0 |

_ƒ indica una columna calculada._

</details>

| Término | Descripción | Comentario narrativo |
|------|-------------|-------------------|
| **Property** | A kind of link, with the characteristics the rules use. | — |
| Label | Un atributo definido. | — |
| Inverse of | Un atributo definido. | — |
| Is Symmetric | Verdadero cuando una cadena vacía. | — |
| Chain First | Un atributo definido. | — |
| Chain Second | Un atributo definido. | — |
| Domain Class | Un atributo definido. | — |
| Range Class | Un atributo definido. | — |
| Disjoint With | Un atributo definido. | — |
| Name | Igual a label. | — |

<details open>
<summary>Datos de ejemplo</summary>

| Label | Is Symmetric | Name ƒ |
|---|---|---|
| is parent of | false | is parent of |
| is child of | false | is child of |
| is spouse of | true | is spouse of |

_ƒ indica una columna calculada._

</details>

| Término | Descripción | Comentario narrativo |
|------|-------------|-------------------|
| **Statement** | A fact that is stated, or could be concluded: Subject, Property, Object. | — |
| Graph | Un atributo definido. | — |
| Subject | Un atributo definido. | — |
| Property | Un atributo definido. | — |
| Object Thing | Un atributo definido. | — |
| Number Value | Un atributo definido. | — |
| Is Stated | Verdadero cuando una cadena vacía. | _Drawn as a solid line in the explainer._ |
| Subject Label | Tomado del subject vinculado. | — |
| Property Label | Tomado del property vinculado. | — |
| Object Label | Tomado del object thing vinculado. | — |
| Name | Calculado como el subject label, seguido de un espacio, seguido de el property label, seguido de un espacio, seguido de el number value si el object thing está vacío, en todos los demás casos el object label. | — |
| Count of Inverse Firings | El total de inverse fired unit entre los firings relacionados con el statement. | — |
| Count of Symmetric Firings | El total de symmetric fired unit entre los firings relacionados con el statement. | — |
| Holds Before Chain | Verdadero cuando se cumple al menos una de las siguientes: el indicador «stated» está activado; el count of inverse firings es mayor que 0; o el count of symmetric firings es mayor que 0. | — |
| Count of Chain Firings | El total de chain fired unit entre los firings relacionados con el statement. | — |
| Holds Before Typing | Verdadero cuando se cumple al menos una de las siguientes: el indicador «holds before chain» está activado o el count of chain firings es mayor que 0. | — |
| Count of Type Firings | El total de type fired unit entre los firings relacionados con el statement. | — |
| Holds | Verdadero cuando se cumple al menos una de las siguientes: el indicador «holds before typing» está activado o el count of type firings es mayor que 0. | _True when stated or concluded by an enabled rule._ |
| Is Conclusion | Verdadero cuando se cumple todo lo siguiente: el indicador «holds» está activado y el indicador «stated» no está activado. | _Drawn as a dashed line in the explainer._ |
| Worked Out by | Determinado por prioridad: una cadena vacía si el indicador «conclusion» no está activado; «inverse» si el count of inverse firings es mayor que 0; «symmetric» si el count of symmetric firings es mayor que 0; «property chain» si el count of chain firings es mayor que 0; en todos los demás casos, «domain and range». | — |
| Stated Unit | Determinado por prioridad: 1 si el indicador «stated» está activado; en todos los demás casos, 0. | — |
| Conclusion Unit | Determinado por prioridad: 1 si el indicador «conclusion» está activado; en todos los demás casos, 0. | — |
| Parent Unit | Determinado por prioridad: 1 si se cumple todo lo siguiente: el indicador «holds» está activado y el property es «isParentOf»; en todos los demás casos, 0. | — |
| Grandparent Unit | Determinado por prioridad: 1 si se cumple todo lo siguiente: el indicador «holds» está activado y el property es «isGrandparentOf»; en todos los demás casos, 0. | — |

<details open>
<summary>Datos de ejemplo</summary>

| Number Value | Is Stated | Subject Label ƒ | Property Label ƒ | Object Label ƒ | Name ƒ | Count of Inverse Firings ƒ | Count of Symmetric Firings ƒ | Holds Before Chain ƒ | Count of Chain Firings ƒ | Holds Before Typing ƒ | Count of Type Firings ƒ | Holds ƒ | Is Conclusion ƒ | Worked Out by ƒ | Stated Unit ƒ | Conclusion Unit ƒ | Parent Unit ƒ | Grandparent Unit ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| — | true | Marvin | is parent of | Randy | Marvin is parent of Randy | 0 | 0 | true | 0 | true | 0 | true | false | — | 1 | 0 | 1 | 0 |
| — | false | Marvin | is parent of | Sharon | Marvin is parent of Sharon | 0 | 0 | false | 0 | false | 0 | false | false | — | 0 | 0 | 0 | 0 |
| — | false | Marvin | is parent of | Shelly | Marvin is parent of Shelly | 0 | 0 | false | 0 | false | 0 | false | false | — | 0 | 0 | 0 | 0 |

_ƒ indica una columna calculada._

</details>

| Término | Descripción | Comentario narrativo |
|------|-------------|-------------------|
| **Firing** | One way one rule could produce one fact from one or two others. | — |
| Rule | Un atributo definido. | — |
| Kind | Un atributo definido. | — |
| Target | Un atributo definido. | — |
| First | Un atributo definido. | — |
| Second | Un atributo definido. | _Same as First when the rule needs one premise._ |
| Rule Enabled | Verdadero cuando el rule vinculado es enabled. | — |
| Rule Label | Tomado del rule vinculado. | — |
| Target Name | Tomado del target vinculado. | — |
| First Name | Tomado del first vinculado. | — |
| Second Name | Tomado del second vinculado. | — |
| First is Stated | Verdadero cuando el first vinculado es stated. | — |
| First Holds Before Chain | Verdadero cuando el first vinculado es holds before chain. | — |
| Second Holds Before Chain | Verdadero cuando el second vinculado es holds before chain. | — |
| First Holds Before Typing | Verdadero cuando el first vinculado es holds before typing. | — |
| Inverse Fired Unit | Determinado por prioridad: 1 si se cumple todo lo siguiente: el indicador «rule enabled» está activado; el kind es «inverse»; y el indicador «first is stated» está activado; en todos los demás casos, 0. | — |
| Symmetric Fired Unit | Determinado por prioridad: 1 si se cumple todo lo siguiente: el indicador «rule enabled» está activado; el kind es «symmetric»; y el indicador «first is stated» está activado; en todos los demás casos, 0. | — |
| Chain Fired Unit | Determinado por prioridad: 1 si se cumple todo lo siguiente: el indicador «rule enabled» está activado; el kind es «chain»; el indicador «first holds before chain» está activado; y el indicador «second holds before chain» está activado; en todos los demás casos, 0. | — |
| Type Fired Unit | Determinado por prioridad: 1 si se cumple todo lo siguiente: el indicador «rule enabled» está activado; se cumple al menos una de las siguientes: el kind es «domain» o el kind es «range»; y el indicador «first holds before typing» está activado; en todos los demás casos, 0. | — |
| Fired Unit | Calculado como el inverse fired unit más el symmetric fired unit más el chain fired unit más el type fired unit. | — |
| Name | Determinado por prioridad: el target name, seguido de «: concluded by the », seguido de el rule label, seguido de « rule from '», seguido de el first name, seguido de «'», seguido de « and '», seguido de el second name, seguido de «'» si el fired unit es mayor que 0; en todos los demás casos, el firing ID. | _The explainer's conclusion card._ |

<details open>
<summary>Datos de ejemplo</summary>

| Kind | Rule Enabled ƒ | Rule Label ƒ | Target Name ƒ | First Name ƒ | Second Name ƒ | First is Stated ƒ | First Holds Before Chain ƒ | Second Holds Before Chain ƒ | First Holds Before Typing ƒ | Inverse Fired Unit ƒ | Symmetric Fired Unit ƒ | Chain Fired Unit ƒ | Type Fired Unit ƒ | Fired Unit ƒ | Name ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| inverse | true | Inverse | Randy is child of Marvin | Marvin is parent of Randy | Marvin is parent of Randy | true | true | true | true | 1 | 0 | 0 | 0 | 1 | Randy is child of Marvin: concluded by the Inverse rule from 'Marvin is parent of Randy' |
| inverse | true | Inverse | Sharon is child of Marvin | Marvin is parent of Sharon | Marvin is parent of Sharon | false | false | false | false | 0 | 0 | 0 | 0 | 0 | inverse:Marvin\|isParentOf\|Sharon>Sharon\|isChildOf\|Marvin |
| inverse | true | Inverse | Shelly is child of Marvin | Marvin is parent of Shelly | Marvin is parent of Shelly | false | false | false | false | 0 | 0 | 0 | 0 | 0 | inverse:Marvin\|isParentOf\|Shelly>Shelly\|isChildOf\|Marvin |

_ƒ indica una columna calculada._

</details>

| Término | Descripción | Comentario narrativo |
|------|-------------|-------------------|
| **Disjoint Check** | Two statements that must not both hold. | — |
| Graph | Un atributo definido. | — |
| Rule | Un atributo definido. | — |
| A | Un atributo definido. | — |
| B | Un atributo definido. | — |
| Name | Igual a disjoint check ID. | — |
| Rule Enabled | Verdadero cuando el rule vinculado es enabled. | — |
| A Holds | Verdadero cuando el a vinculado es holds. | — |
| B Holds | Verdadero cuando el b vinculado es holds. | — |
| Violation Unit | Determinado por prioridad: 1 si se cumple todo lo siguiente: el indicador «rule enabled» está activado; el indicador «a holds» está activado; y el indicador «b holds» está activado; en todos los demás casos, 0. | — |

<details open>
<summary>Datos de ejemplo</summary>

| Name ƒ | Rule Enabled ƒ | A Holds ƒ | B Holds ƒ | Violation Unit ƒ |
|---|---|---|---|---|
| Marvin\|isParentOf\|Randy~isSpouseOf | true | true | false | 0 |
| Marvin\|isParentOf\|Sharon~isSpouseOf | true | false | false | 0 |
| Marvin\|isParentOf\|Shelly~isSpouseOf | true | false | false | 0 |

_ƒ indica una columna calculada._

</details>

## 2 Tipos de hechos

- un **property** puede referenciar un **property**
- un **property** puede referenciar un **thing**
- un **statement** referencia exactamente un **graph**
- un **statement** referencia exactamente un **thing**
- un **statement** referencia exactamente un **property**
- un **firing** referencia exactamente un **rule**
- un **firing** referencia exactamente un **statement**
- un **disjoint check** referencia exactamente un **graph**
- un **disjoint check** referencia exactamente un **rule**
- un **disjoint check** referencia exactamente un **statement**

## 3 Reglas operativas

_Las reglas operativas enuncian lo que el negocio **obliga**, **prohíbe** o
aconseja (**debería**). Las reglas estructurales provienen de campos obligatorios y claves
foráneas; las semánticas provienen de la tabla Constraints, cada una apoyada en un booleano
que el libro de reglas ya calcula (referenciado como DR-N en las reglas definitorias)._

### Restricciones estructurales (del esquema)

- Un rule **debe** tener un label y un explanation, y registrar si es enabled.
- Un thing **debe** tener un label y un kind.
- Un property **debe** tener un label, y registrar si es symmetric.
- Un statement **debe** referenciar exactamente un graph.
- Un statement **debe** referenciar exactamente un thing como su subject.
- Un statement **debe** referenciar exactamente un property.
- Un statement **debe** registrar si es stated.
- Un firing **debe** referenciar exactamente un rule.
- Un firing **debe** referenciar exactamente un statement como su target.
- Un firing **debe** referenciar exactamente un statement como su first.
- Un firing **debe** referenciar exactamente un statement como su second.
- Un firing **debe** tener un kind.
- Un disjoint check **debe** referenciar exactamente un graph.
- Un disjoint check **debe** referenciar exactamente un rule.
- Un disjoint check **debe** referenciar exactamente un statement como su a.
- Un disjoint check **debe** referenciar exactamente un statement como su b.

## 4 Reglas definitorias

_Todos los enunciados expresan verdades del dominio de negocio; no son procedimientos
ni imperativos. Se evita «si y solo si» en favor de «solo si» para que una necesidad
unidireccional no se confunda con una equivalencia. Una marca
**⚠︎ mecánica** señala una regla cuya redacción determinista es fiel pero
tosca — una señal para una reescritura opcional, no un defecto._

| ID | Regla declarativa |
|----|------------------|
| **DR-1 Name** | Un graph: name es igual a graph ID. |
| **DR-2 Count of Stated Facts** | Un graph: count of stated facts es el total de stated unit entre los statements relacionados con el graph. |
| **DR-3 Count of Conclusions** | Un graph: count of conclusions es el total de conclusion unit entre los statements relacionados con el graph. |
| **DR-4 Count of Violations** | Un graph: count of violations es el total de violation unit entre los disjoint checks relacionados con el graph. |
| **DR-5 Is Consistent** | Un graph se considera consistent si el count of violations es 0. |
| **DR-6 Name** | Un rule: name es igual a label. |
| **DR-7 Count of Firings** | Un rule: count of firings es el total de fired unit entre los firings relacionados con el rule. |
| **DR-8 Name** | Un thing: name es igual a label. |
| **DR-9 Count of Children** | Un thing: count of children es el total de parent unit entre los statements relacionados con el thing. |
| **DR-10 Count of Grandchildren** | Un thing: count of grandchildren es el total de grandparent unit entre los statements relacionados con el thing. |
| **DR-11 Name** | Un property: name es igual a label. |
| **DR-12 Subject Label** | Un statement: subject label — tomado del subject vinculado. |
| **DR-13 Property Label** | Un statement: property label — tomado del property vinculado. |
| **DR-14 Object Label** | Un statement: object label — tomado del object thing vinculado. |
| **DR-15 Name** | Un statement: name se calcula como el subject label, seguido de un espacio, seguido de el property label, seguido de un espacio, seguido de el number value si el object thing está vacío, en todos los demás casos el object label. |
| **DR-16 Count of Inverse Firings** | Un statement: count of inverse firings es el total de inverse fired unit entre los firings relacionados con el statement. |
| **DR-17 Count of Symmetric Firings** | Un statement: count of symmetric firings es el total de symmetric fired unit entre los firings relacionados con el statement. |
| **DR-18 Holds Before Chain** | Un statement se considera que cumple: before chain si se cumple al menos una de las siguientes: el indicador «stated» está activado; el count of inverse firings es mayor que 0; o el count of symmetric firings es mayor que 0. |
| **DR-19 Count of Chain Firings** | Un statement: count of chain firings es el total de chain fired unit entre los firings relacionados con el statement. |
| **DR-20 Holds Before Typing** | Un statement se considera que cumple: before typing si se cumple al menos una de las siguientes: el indicador «holds before chain» está activado o el count of chain firings es mayor que 0. |
| **DR-21 Count of Type Firings** | Un statement: count of type firings es el total de type fired unit entre los firings relacionados con el statement. |
| **DR-22 Holds** | Un statement se considera que cumple: si se cumple al menos una de las siguientes: el indicador «holds before typing» está activado o el count of type firings es mayor que 0. |
| **DR-23 Is Conclusion** | Un statement se considera un conclusion si se cumple todo lo siguiente: el indicador «holds» está activado y el indicador «stated» no está activado. |
| **DR-24 Worked Out by** | El campo «worked out by» del statement se determina según la siguiente prioridad:<br>1. una cadena vacía, si el indicador «conclusion» no está activado;<br>2. «inverse», si el count of inverse firings es mayor que 0;<br>3. «symmetric», si el count of symmetric firings es mayor que 0;<br>4. «property chain», si el count of chain firings es mayor que 0;<br>5. en todos los demás casos, «domain and range». |
| **DR-25 Stated Unit** | El campo «stated unit» del statement se determina según la siguiente prioridad:<br>1. 1, si el indicador «stated» está activado;<br>2. en todos los demás casos, 0. |
| **DR-26 Conclusion Unit** | El campo «conclusion unit» del statement se determina según la siguiente prioridad:<br>1. 1, si el indicador «conclusion» está activado;<br>2. en todos los demás casos, 0. |
| **DR-27 Parent Unit** | El campo «parent unit» del statement se determina según la siguiente prioridad:<br>1. 1, si se cumple todo lo siguiente: el indicador «holds» está activado y el property es «isParentOf»;<br>2. en todos los demás casos, 0. |
| **DR-28 Grandparent Unit** | El campo «grandparent unit» del statement se determina según la siguiente prioridad:<br>1. 1, si se cumple todo lo siguiente: el indicador «holds» está activado y el property es «isGrandparentOf»;<br>2. en todos los demás casos, 0. |
| **DR-29 Rule Enabled** | Un firing: rule enabled es verdadero cuando el rule vinculado es enabled. |
| **DR-30 Rule Label** | Un firing: rule label — tomado del rule vinculado. |
| **DR-31 Target Name** | Un firing: target name — tomado del target vinculado. |
| **DR-32 First Name** | Un firing: first name — tomado del first vinculado. |
| **DR-33 Second Name** | Un firing: second name — tomado del second vinculado. |
| **DR-34 First is Stated** | Un firing: first is stated es verdadero cuando el first vinculado es stated. |
| **DR-35 First Holds Before Chain** | Un firing: first holds before chain es verdadero cuando el first vinculado es holds before chain. |
| **DR-36 Second Holds Before Chain** | Un firing: second holds before chain es verdadero cuando el second vinculado es holds before chain. |
| **DR-37 First Holds Before Typing** | Un firing: first holds before typing es verdadero cuando el first vinculado es holds before typing. |
| **DR-38 Inverse Fired Unit** | El campo «inverse fired unit» del firing se determina según la siguiente prioridad:<br>1. 1, si se cumple todo lo siguiente: el indicador «rule enabled» está activado; el kind es «inverse»; y el indicador «first is stated» está activado;<br>2. en todos los demás casos, 0. |
| **DR-39 Symmetric Fired Unit** | El campo «symmetric fired unit» del firing se determina según la siguiente prioridad:<br>1. 1, si se cumple todo lo siguiente: el indicador «rule enabled» está activado; el kind es «symmetric»; y el indicador «first is stated» está activado;<br>2. en todos los demás casos, 0. |
| **DR-40 Chain Fired Unit** | El campo «chain fired unit» del firing se determina según la siguiente prioridad:<br>1. 1, si se cumple todo lo siguiente: el indicador «rule enabled» está activado; el kind es «chain»; el indicador «first holds before chain» está activado; y el indicador «second holds before chain» está activado;<br>2. en todos los demás casos, 0. |
| **DR-41 Type Fired Unit** | El campo «type fired unit» del firing se determina según la siguiente prioridad:<br>1. 1, si se cumple todo lo siguiente: el indicador «rule enabled» está activado; se cumple al menos una de las siguientes: el kind es «domain» o el kind es «range»; y el indicador «first holds before typing» está activado;<br>2. en todos los demás casos, 0. |
| **DR-42 Fired Unit** | Un firing: fired unit se calcula como el inverse fired unit más el symmetric fired unit más el chain fired unit más el type fired unit. |
| **DR-43 Name** | El campo «name» del firing se determina según la siguiente prioridad:<br>1. el target name, seguido de «: concluded by the », seguido de el rule label, seguido de « rule from '», seguido de el first name, seguido de «'», seguido de « and '», seguido de el second name, seguido de «'», si el fired unit es mayor que 0;<br>2. en todos los demás casos, el firing ID. |
| **DR-44 Name** | Un disjoint check: name es igual a disjoint check ID. |
| **DR-45 Rule Enabled** | Un disjoint check: rule enabled es verdadero cuando el rule vinculado es enabled. |
| **DR-46 A Holds** | Un disjoint check: a holds es verdadero cuando el a vinculado es holds. |
| **DR-47 B Holds** | Un disjoint check: b holds es verdadero cuando el b vinculado es holds. |
| **DR-48 Violation Unit** | El campo «violation unit» del disjoint check se determina según la siguiente prioridad:<br>1. 1, si se cumple todo lo siguiente: el indicador «rule enabled» está activado; el indicador «a holds» está activado; y el indicador «b holds» está activado;<br>2. en todos los demás casos, 0. |

## 5 Trazabilidad al esquema

_La columna de expresión es la definición de la regla en notación Documentación —
la misma lógica que almacena el libro de reglas, escrita para un lector de negocio._

| Elemento del esquema | Tipo | Expresión |
|----------------|------|------------|
| **Graphs.Name** | fórmula | `GraphId` |
| **Graphs.CountOfStatedFacts** | agregado | `Sum(Statements.StatedUnit via Graph)` |
| **Graphs.CountOfConclusions** | agregado | `Sum(Statements.ConclusionUnit via Graph)` |
| **Graphs.CountOfViolations** | agregado | `Sum(DisjointChecks.ViolationUnit via Graph)` |
| **Graphs.IsConsistent** | fórmula | `CountOfViolations = 0` |
| **Rules.Name** | fórmula | `Label` |
| **Rules.CountOfFirings** | agregado | `Sum(Firings.FiredUnit via Rule)` |
| **Things.Name** | fórmula | `Label` |
| **Things.CountOfChildren** | agregado | `Sum(Statements.ParentUnit via Subject)` |
| **Things.CountOfGrandchildren** | agregado | `Sum(Statements.GrandparentUnit via Subject)` |
| **Properties.Name** | fórmula | `Label` |
| **Statements.SubjectLabel** | búsqueda | `Lookup(Things.Label via Subject)` |
| **Statements.PropertyLabel** | búsqueda | `Lookup(Properties.Label via Property)` |
| **Statements.ObjectLabel** | búsqueda | `Lookup(Things.Label via ObjectThing)` |
| **Statements.Name** | fórmula | `SubjectLabel & " " & PropertyLabel & " " & If(ObjectThing = Blank(), NumberValue, ObjectLabel)` |
| **Statements.CountOfInverseFirings** | agregado | `Sum(Firings.InverseFiredUnit via Target)` |
| **Statements.CountOfSymmetricFirings** | agregado | `Sum(Firings.SymmetricFiredUnit via Target)` |
| **Statements.HoldsBeforeChain** | fórmula | `Or(IsStated, CountOfInverseFirings > 0, CountOfSymmetricFirings > 0)` |
| **Statements.CountOfChainFirings** | agregado | `Sum(Firings.ChainFiredUnit via Target)` |
| **Statements.HoldsBeforeTyping** | fórmula | `Or(HoldsBeforeChain, CountOfChainFirings > 0)` |
| **Statements.CountOfTypeFirings** | agregado | `Sum(Firings.TypeFiredUnit via Target)` |
| **Statements.Holds** | fórmula | `Or(HoldsBeforeTyping, CountOfTypeFirings > 0)` |
| **Statements.IsConclusion** | fórmula | `And(Holds, Not(IsStated))` |
| **Statements.WorkedOutBy** | fórmula | `If(Not(IsConclusion), "", If(CountOfInverseFirings > 0, "inverse", If(CountOfSymmetricFirings > 0, "symmetric", If(CountOfChainFirings > 0, "property chain", "domain and range"))))` |
| **Statements.StatedUnit** | fórmula | `If(IsStated, 1, 0)` |
| **Statements.ConclusionUnit** | fórmula | `If(IsConclusion, 1, 0)` |
| **Statements.ParentUnit** | fórmula | `If(And(Holds, Property = "isParentOf"), 1, 0)` |
| **Statements.GrandparentUnit** | fórmula | `If(And(Holds, Property = "isGrandparentOf"), 1, 0)` |
| **Firings.RuleEnabled** | búsqueda | `Lookup(Rules.IsEnabled via Rule)` |
| **Firings.RuleLabel** | búsqueda | `Lookup(Rules.Label via Rule)` |
| **Firings.TargetName** | búsqueda | `Lookup(Statements.Name via Target)` |
| **Firings.FirstName** | búsqueda | `Lookup(Statements.Name via First)` |
| **Firings.SecondName** | búsqueda | `Lookup(Statements.Name via Second)` |
| **Firings.FirstIsStated** | búsqueda | `Lookup(Statements.IsStated via First)` |
| **Firings.FirstHoldsBeforeChain** | búsqueda | `Lookup(Statements.HoldsBeforeChain via First)` |
| **Firings.SecondHoldsBeforeChain** | búsqueda | `Lookup(Statements.HoldsBeforeChain via Second)` |
| **Firings.FirstHoldsBeforeTyping** | búsqueda | `Lookup(Statements.HoldsBeforeTyping via First)` |
| **Firings.InverseFiredUnit** | fórmula | `If(And(RuleEnabled, Kind = "inverse", FirstIsStated), 1, 0)` |
| **Firings.SymmetricFiredUnit** | fórmula | `If(And(RuleEnabled, Kind = "symmetric", FirstIsStated), 1, 0)` |
| **Firings.ChainFiredUnit** | fórmula | `If(And(RuleEnabled, Kind = "chain", FirstHoldsBeforeChain, SecondHoldsBeforeChain), 1, 0)` |
| **Firings.TypeFiredUnit** | fórmula | `If(And(RuleEnabled, Or(Kind = "domain", Kind = "range"), FirstHoldsBeforeTyping), 1, 0)` |
| **Firings.FiredUnit** | fórmula | `InverseFiredUnit + SymmetricFiredUnit + ChainFiredUnit + TypeFiredUnit` |
| **Firings.Name** | fórmula | `If(FiredUnit > 0, TargetName & ": concluded by the " & RuleLabel & " rule from '" & FirstName & "'" & If(Kind = "chain", " and '" & SecondName & "'", ""), FiringId)` |
| **DisjointChecks.Name** | fórmula | `DisjointCheckId` |
| **DisjointChecks.RuleEnabled** | búsqueda | `Lookup(Rules.IsEnabled via Rule)` |
| **DisjointChecks.AHolds** | búsqueda | `Lookup(Statements.Holds via A)` |
| **DisjointChecks.BHolds** | búsqueda | `Lookup(Statements.Holds via B)` |
| **DisjointChecks.ViolationUnit** | fórmula | `If(And(RuleEnabled, AHolds, BHolds), 1, 0)` |
