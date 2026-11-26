# 📘 South Park ontology explainer — Documentação

_The barebones South Park ontology from Jessica Talisman's explainer: ten stated facts and five rules that can each be switched on. Conclusions are formulas._

> Regras de negócio declarativas geradas a partir do livro de regras. Cada
> enunciado expressa uma verdade do domínio de negócio — não é um procedimento
> nem um imperativo. As fórmulas do livro de regras são a única fonte da verdade;
> este documento é a sua leitura em linguagem clara.

## 1 Vocabulário de negócio

| Termo | Descrição | Comentário narrativo |
|------|-------------|-------------------|
| **Graph** | The whole example. | — |
| Name | Igual a graph ID. | — |
| Count of Stated Facts | O total de stated unit entre os statements relacionados a o graph. | — |
| Count of Conclusions | O total de conclusion unit entre os statements relacionados a o graph. | _Facts that hold but were never stated._ |
| Count of Violations | O total de violation unit entre os disjoint checks relacionados a o graph. | — |
| Is Consistent | Verdadeiro quando o count of violations é 0. | — |

<details open>
<summary>Dados de exemplo</summary>

| Name ƒ | Count of Stated Facts ƒ | Count of Conclusions ƒ | Count of Violations ƒ | Is Consistent ƒ |
|---|---|---|---|---|
| south-park | 10 | 8 | 0 | true |

_ƒ marca uma coluna calculada._

</details>

| Termo | Descrição | Comentário narrativo |
|------|-------------|-------------------|
| **Rule** | The five rules; IsEnabled is the checkbox. | — |
| Label | Um atributo definido. | — |
| Explanation | Um atributo definido. | — |
| Is Enabled | Verdadeiro quando uma cadeia vazia. | — |
| Name | Igual a label. | — |
| Count of Firings | O total de fired unit entre os firings relacionados a o rule. | _How many times the rule produced a fact._ |

<details open>
<summary>Dados de exemplo</summary>

| Label | Explanation | Is Enabled | Name ƒ | Count of Firings ƒ |
|---|---|---|---|---|
| Inverse | A parent link also gives a child link between the same two people. | true | Inverse | 3 |
| Symmetric | A friend link or a spouse link is true for both people. | true | Symmetric | 2 |
| Property chain | A parent of a parent is a grandparent. | true | Property chain | 2 |

_ƒ marca uma coluna calculada._

</details>

| Termo | Descrição | Comentário narrativo |
|------|-------------|-------------------|
| **Thing** | A node in the picture: a person, a place, a class or a role value. | — |
| Label | Um atributo definido. | — |
| Kind | Um atributo definido. | — |
| Name | Igual a label. | — |
| Count of Children | O total de parent unit entre os statements relacionados a o thing. | _Added: a count, which a reasoner alone does not give._ |
| Count of Grandchildren | O total de grandparent unit entre os statements relacionados a o thing. | _Added: counts concluded grandchildren._ |

<details open>
<summary>Dados de exemplo</summary>

| Label | Kind | Name ƒ | Count of Children ƒ | Count of Grandchildren ƒ |
|---|---|---|---|---|
| Marvin | individual | Marvin | 1 | 2 |
| Randy | individual | Randy | 2 | 0 |
| Sharon | individual | Sharon | 0 | 0 |

_ƒ marca uma coluna calculada._

</details>

| Termo | Descrição | Comentário narrativo |
|------|-------------|-------------------|
| **Property** | A kind of link, with the characteristics the rules use. | — |
| Label | Um atributo definido. | — |
| Inverse of | Um atributo definido. | — |
| Is Symmetric | Verdadeiro quando uma cadeia vazia. | — |
| Chain First | Um atributo definido. | — |
| Chain Second | Um atributo definido. | — |
| Domain Class | Um atributo definido. | — |
| Range Class | Um atributo definido. | — |
| Disjoint With | Um atributo definido. | — |
| Name | Igual a label. | — |

<details open>
<summary>Dados de exemplo</summary>

| Label | Is Symmetric | Name ƒ |
|---|---|---|
| is parent of | false | is parent of |
| is child of | false | is child of |
| is spouse of | true | is spouse of |

_ƒ marca uma coluna calculada._

</details>

| Termo | Descrição | Comentário narrativo |
|------|-------------|-------------------|
| **Statement** | A fact that is stated, or could be concluded: Subject, Property, Object. | — |
| Graph | Um atributo definido. | — |
| Subject | Um atributo definido. | — |
| Property | Um atributo definido. | — |
| Object Thing | Um atributo definido. | — |
| Number Value | Um atributo definido. | — |
| Is Stated | Verdadeiro quando uma cadeia vazia. | _Drawn as a solid line in the explainer._ |
| Subject Label | Obtido do subject vinculado. | — |
| Property Label | Obtido do property vinculado. | — |
| Object Label | Obtido do object thing vinculado. | — |
| Name | Calculado como o subject label, seguido de um espaço, seguido de o property label, seguido de um espaço, seguido de o number value se o object thing está vazio, em todos os outros casos o object label. | — |
| Count of Inverse Firings | O total de inverse fired unit entre os firings relacionados a o statement. | — |
| Count of Symmetric Firings | O total de symmetric fired unit entre os firings relacionados a o statement. | — |
| Holds Before Chain | Verdadeiro quando pelo menos uma das seguintes é verdadeira: o indicador “stated” está ativado; o count of inverse firings é maior que 0; ou o count of symmetric firings é maior que 0. | — |
| Count of Chain Firings | O total de chain fired unit entre os firings relacionados a o statement. | — |
| Holds Before Typing | Verdadeiro quando pelo menos uma das seguintes é verdadeira: o indicador “holds before chain” está ativado ou o count of chain firings é maior que 0. | — |
| Count of Type Firings | O total de type fired unit entre os firings relacionados a o statement. | — |
| Holds | Verdadeiro quando pelo menos uma das seguintes é verdadeira: o indicador “holds before typing” está ativado ou o count of type firings é maior que 0. | _True when stated or concluded by an enabled rule._ |
| Is Conclusion | Verdadeiro quando tudo o que segue é verdadeiro: o indicador “holds” está ativado e o indicador “stated” não está ativado. | _Drawn as a dashed line in the explainer._ |
| Worked Out by | Determinado por prioridade: uma cadeia vazia se o indicador “conclusion” não está ativado; “inverse” se o count of inverse firings é maior que 0; “symmetric” se o count of symmetric firings é maior que 0; “property chain” se o count of chain firings é maior que 0; em todos os outros casos, “domain and range”. | — |
| Stated Unit | Determinado por prioridade: 1 se o indicador “stated” está ativado; em todos os outros casos, 0. | — |
| Conclusion Unit | Determinado por prioridade: 1 se o indicador “conclusion” está ativado; em todos os outros casos, 0. | — |
| Parent Unit | Determinado por prioridade: 1 se tudo o que segue é verdadeiro: o indicador “holds” está ativado e o property é “isParentOf”; em todos os outros casos, 0. | — |
| Grandparent Unit | Determinado por prioridade: 1 se tudo o que segue é verdadeiro: o indicador “holds” está ativado e o property é “isGrandparentOf”; em todos os outros casos, 0. | — |

<details open>
<summary>Dados de exemplo</summary>

| Number Value | Is Stated | Subject Label ƒ | Property Label ƒ | Object Label ƒ | Name ƒ | Count of Inverse Firings ƒ | Count of Symmetric Firings ƒ | Holds Before Chain ƒ | Count of Chain Firings ƒ | Holds Before Typing ƒ | Count of Type Firings ƒ | Holds ƒ | Is Conclusion ƒ | Worked Out by ƒ | Stated Unit ƒ | Conclusion Unit ƒ | Parent Unit ƒ | Grandparent Unit ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| — | true | Marvin | is parent of | Randy | Marvin is parent of Randy | 0 | 0 | true | 0 | true | 0 | true | false | — | 1 | 0 | 1 | 0 |
| — | false | Marvin | is parent of | Sharon | Marvin is parent of Sharon | 0 | 0 | false | 0 | false | 0 | false | false | — | 0 | 0 | 0 | 0 |
| — | false | Marvin | is parent of | Shelly | Marvin is parent of Shelly | 0 | 0 | false | 0 | false | 0 | false | false | — | 0 | 0 | 0 | 0 |

_ƒ marca uma coluna calculada._

</details>

| Termo | Descrição | Comentário narrativo |
|------|-------------|-------------------|
| **Firing** | One way one rule could produce one fact from one or two others. | — |
| Rule | Um atributo definido. | — |
| Kind | Um atributo definido. | — |
| Target | Um atributo definido. | — |
| First | Um atributo definido. | — |
| Second | Um atributo definido. | _Same as First when the rule needs one premise._ |
| Rule Enabled | Verdadeiro quando o rule vinculado é enabled. | — |
| Rule Label | Obtido do rule vinculado. | — |
| Target Name | Obtido do target vinculado. | — |
| First Name | Obtido do first vinculado. | — |
| Second Name | Obtido do second vinculado. | — |
| First is Stated | Verdadeiro quando o first vinculado é stated. | — |
| First Holds Before Chain | Verdadeiro quando o first vinculado é holds before chain. | — |
| Second Holds Before Chain | Verdadeiro quando o second vinculado é holds before chain. | — |
| First Holds Before Typing | Verdadeiro quando o first vinculado é holds before typing. | — |
| Inverse Fired Unit | Determinado por prioridade: 1 se tudo o que segue é verdadeiro: o indicador “rule enabled” está ativado; o kind é “inverse”; e o indicador “first is stated” está ativado; em todos os outros casos, 0. | — |
| Symmetric Fired Unit | Determinado por prioridade: 1 se tudo o que segue é verdadeiro: o indicador “rule enabled” está ativado; o kind é “symmetric”; e o indicador “first is stated” está ativado; em todos os outros casos, 0. | — |
| Chain Fired Unit | Determinado por prioridade: 1 se tudo o que segue é verdadeiro: o indicador “rule enabled” está ativado; o kind é “chain”; o indicador “first holds before chain” está ativado; e o indicador “second holds before chain” está ativado; em todos os outros casos, 0. | — |
| Type Fired Unit | Determinado por prioridade: 1 se tudo o que segue é verdadeiro: o indicador “rule enabled” está ativado; pelo menos uma das seguintes é verdadeira: o kind é “domain” ou o kind é “range”; e o indicador “first holds before typing” está ativado; em todos os outros casos, 0. | — |
| Fired Unit | Calculado como o inverse fired unit mais o symmetric fired unit mais o chain fired unit mais o type fired unit. | — |
| Name | Determinado por prioridade: o target name, seguido de “: worked out by the ”, seguido de o rule label, seguido de “ rule from '”, seguido de o first name, seguido de “'”, seguido de “ and '”, seguido de o second name, seguido de “'” se o fired unit é maior que 0; em todos os outros casos, o firing ID. | _The explainer's conclusion card._ |

<details open>
<summary>Dados de exemplo</summary>

| Kind | Rule Enabled ƒ | Rule Label ƒ | Target Name ƒ | First Name ƒ | Second Name ƒ | First is Stated ƒ | First Holds Before Chain ƒ | Second Holds Before Chain ƒ | First Holds Before Typing ƒ | Inverse Fired Unit ƒ | Symmetric Fired Unit ƒ | Chain Fired Unit ƒ | Type Fired Unit ƒ | Fired Unit ƒ | Name ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| inverse | true | Inverse | Randy is child of Marvin | Marvin is parent of Randy | Marvin is parent of Randy | true | true | true | true | 1 | 0 | 0 | 0 | 1 | Randy is child of Marvin: worked out by the Inverse rule from 'Marvin is parent of Randy' |
| inverse | true | Inverse | Sharon is child of Marvin | Marvin is parent of Sharon | Marvin is parent of Sharon | false | false | false | false | 0 | 0 | 0 | 0 | 0 | inverse:Marvin\|isParentOf\|Sharon>Sharon\|isChildOf\|Marvin |
| inverse | true | Inverse | Shelly is child of Marvin | Marvin is parent of Shelly | Marvin is parent of Shelly | false | false | false | false | 0 | 0 | 0 | 0 | 0 | inverse:Marvin\|isParentOf\|Shelly>Shelly\|isChildOf\|Marvin |

_ƒ marca uma coluna calculada._

</details>

| Termo | Descrição | Comentário narrativo |
|------|-------------|-------------------|
| **Disjoint Check** | Two statements that must not both hold. | — |
| Graph | Um atributo definido. | — |
| Rule | Um atributo definido. | — |
| A | Um atributo definido. | — |
| B | Um atributo definido. | — |
| Name | Igual a disjoint check ID. | — |
| Rule Enabled | Verdadeiro quando o rule vinculado é enabled. | — |
| A Holds | Verdadeiro quando o a vinculado é holds. | — |
| B Holds | Verdadeiro quando o b vinculado é holds. | — |
| Violation Unit | Determinado por prioridade: 1 se tudo o que segue é verdadeiro: o indicador “rule enabled” está ativado; o indicador “a holds” está ativado; e o indicador “b holds” está ativado; em todos os outros casos, 0. | — |

<details open>
<summary>Dados de exemplo</summary>

| Name ƒ | Rule Enabled ƒ | A Holds ƒ | B Holds ƒ | Violation Unit ƒ |
|---|---|---|---|---|
| Marvin\|isParentOf\|Randy~isSpouseOf | true | true | false | 0 |
| Marvin\|isParentOf\|Sharon~isSpouseOf | true | false | false | 0 |
| Marvin\|isParentOf\|Shelly~isSpouseOf | true | false | false | 0 |

_ƒ marca uma coluna calculada._

</details>

## 2 Tipos de fatos

- um **property** pode referenciar um **property**
- um **property** pode referenciar um **thing**
- um **statement** referencia exatamente um **graph**
- um **statement** referencia exatamente um **thing**
- um **statement** referencia exatamente um **property**
- um **firing** referencia exatamente um **rule**
- um **firing** referencia exatamente um **statement**
- um **disjoint check** referencia exatamente um **graph**
- um **disjoint check** referencia exatamente um **rule**
- um **disjoint check** referencia exatamente um **statement**

## 3 Regras operativas

_As regras operativas enunciam o que o negócio **obriga**, **proíbe** ou
aconselha (**deveria**). As regras estruturais vêm de campos obrigatórios e chaves
estrangeiras; as semânticas vêm da tabela Constraints, cada uma baseada num booleano
que o livro de regras já calcula (referenciado como DR-N nas regras definidoras)._

### Restrições estruturais (do esquema)

- Um rule **deve** ter um label e um explanation, e registrar se é enabled.
- Um thing **deve** ter um label e um kind.
- Um property **deve** ter um label, e registrar se é symmetric.
- Um statement **deve** referenciar exatamente um graph.
- Um statement **deve** referenciar exatamente um thing como seu subject.
- Um statement **deve** referenciar exatamente um property.
- Um statement **deve** registrar se é stated.
- Um firing **deve** referenciar exatamente um rule.
- Um firing **deve** referenciar exatamente um statement como seu target.
- Um firing **deve** referenciar exatamente um statement como seu first.
- Um firing **deve** referenciar exatamente um statement como seu second.
- Um firing **deve** ter um kind.
- Um disjoint check **deve** referenciar exatamente um graph.
- Um disjoint check **deve** referenciar exatamente um rule.
- Um disjoint check **deve** referenciar exatamente um statement como seu a.
- Um disjoint check **deve** referenciar exatamente um statement como seu b.

## 4 Regras definidoras

_Todos os enunciados expressam verdades do domínio de negócio; não são procedimentos
nem imperativos. Evita-se «se e somente se» em favor de «somente se», para que uma
necessidade unidirecional não seja confundida com uma equivalência. Uma marca
**⚠︎ mecânica** sinaliza uma regra cuja redação determinista é fiel mas
dura — um sinal para uma reescrita opcional, não um defeito._

| ID | Regra declarativa |
|----|------------------|
| **DR-1 Name** | Um graph: name é igual a graph ID. |
| **DR-2 Count of Stated Facts** | Um graph: count of stated facts é o total de stated unit entre os statements relacionados a o graph. |
| **DR-3 Count of Conclusions** | Um graph: count of conclusions é o total de conclusion unit entre os statements relacionados a o graph. |
| **DR-4 Count of Violations** | Um graph: count of violations é o total de violation unit entre os disjoint checks relacionados a o graph. |
| **DR-5 Is Consistent** | Um graph é considerado consistent se o count of violations é 0. |
| **DR-6 Name** | Um rule: name é igual a label. |
| **DR-7 Count of Firings** | Um rule: count of firings é o total de fired unit entre os firings relacionados a o rule. |
| **DR-8 Name** | Um thing: name é igual a label. |
| **DR-9 Count of Children** | Um thing: count of children é o total de parent unit entre os statements relacionados a o thing. |
| **DR-10 Count of Grandchildren** | Um thing: count of grandchildren é o total de grandparent unit entre os statements relacionados a o thing. |
| **DR-11 Name** | Um property: name é igual a label. |
| **DR-12 Subject Label** | Um statement: subject label — obtido do subject vinculado. |
| **DR-13 Property Label** | Um statement: property label — obtido do property vinculado. |
| **DR-14 Object Label** | Um statement: object label — obtido do object thing vinculado. |
| **DR-15 Name** | Um statement: name é calculado como o subject label, seguido de um espaço, seguido de o property label, seguido de um espaço, seguido de o number value se o object thing está vazio, em todos os outros casos o object label. |
| **DR-16 Count of Inverse Firings** | Um statement: count of inverse firings é o total de inverse fired unit entre os firings relacionados a o statement. |
| **DR-17 Count of Symmetric Firings** | Um statement: count of symmetric firings é o total de symmetric fired unit entre os firings relacionados a o statement. |
| **DR-18 Holds Before Chain** | Um statement é considerado como satisfazendo: before chain se pelo menos uma das seguintes é verdadeira: o indicador “stated” está ativado; o count of inverse firings é maior que 0; ou o count of symmetric firings é maior que 0. |
| **DR-19 Count of Chain Firings** | Um statement: count of chain firings é o total de chain fired unit entre os firings relacionados a o statement. |
| **DR-20 Holds Before Typing** | Um statement é considerado como satisfazendo: before typing se pelo menos uma das seguintes é verdadeira: o indicador “holds before chain” está ativado ou o count of chain firings é maior que 0. |
| **DR-21 Count of Type Firings** | Um statement: count of type firings é o total de type fired unit entre os firings relacionados a o statement. |
| **DR-22 Holds** | Um statement é considerado como satisfazendo: se pelo menos uma das seguintes é verdadeira: o indicador “holds before typing” está ativado ou o count of type firings é maior que 0. |
| **DR-23 Is Conclusion** | Um statement é considerado um conclusion se tudo o que segue é verdadeiro: o indicador “holds” está ativado e o indicador “stated” não está ativado. |
| **DR-24 Worked Out by** | O campo “worked out by” do statement é determinado pela seguinte prioridade:<br>1. uma cadeia vazia, se o indicador “conclusion” não está ativado;<br>2. “inverse”, se o count of inverse firings é maior que 0;<br>3. “symmetric”, se o count of symmetric firings é maior que 0;<br>4. “property chain”, se o count of chain firings é maior que 0;<br>5. em todos os outros casos, “domain and range”. |
| **DR-25 Stated Unit** | O campo “stated unit” do statement é determinado pela seguinte prioridade:<br>1. 1, se o indicador “stated” está ativado;<br>2. em todos os outros casos, 0. |
| **DR-26 Conclusion Unit** | O campo “conclusion unit” do statement é determinado pela seguinte prioridade:<br>1. 1, se o indicador “conclusion” está ativado;<br>2. em todos os outros casos, 0. |
| **DR-27 Parent Unit** | O campo “parent unit” do statement é determinado pela seguinte prioridade:<br>1. 1, se tudo o que segue é verdadeiro: o indicador “holds” está ativado e o property é “isParentOf”;<br>2. em todos os outros casos, 0. |
| **DR-28 Grandparent Unit** | O campo “grandparent unit” do statement é determinado pela seguinte prioridade:<br>1. 1, se tudo o que segue é verdadeiro: o indicador “holds” está ativado e o property é “isGrandparentOf”;<br>2. em todos os outros casos, 0. |
| **DR-29 Rule Enabled** | Um firing: rule enabled é verdadeiro quando o rule vinculado é enabled. |
| **DR-30 Rule Label** | Um firing: rule label — obtido do rule vinculado. |
| **DR-31 Target Name** | Um firing: target name — obtido do target vinculado. |
| **DR-32 First Name** | Um firing: first name — obtido do first vinculado. |
| **DR-33 Second Name** | Um firing: second name — obtido do second vinculado. |
| **DR-34 First is Stated** | Um firing: first is stated é verdadeiro quando o first vinculado é stated. |
| **DR-35 First Holds Before Chain** | Um firing: first holds before chain é verdadeiro quando o first vinculado é holds before chain. |
| **DR-36 Second Holds Before Chain** | Um firing: second holds before chain é verdadeiro quando o second vinculado é holds before chain. |
| **DR-37 First Holds Before Typing** | Um firing: first holds before typing é verdadeiro quando o first vinculado é holds before typing. |
| **DR-38 Inverse Fired Unit** | O campo “inverse fired unit” do firing é determinado pela seguinte prioridade:<br>1. 1, se tudo o que segue é verdadeiro: o indicador “rule enabled” está ativado; o kind é “inverse”; e o indicador “first is stated” está ativado;<br>2. em todos os outros casos, 0. |
| **DR-39 Symmetric Fired Unit** | O campo “symmetric fired unit” do firing é determinado pela seguinte prioridade:<br>1. 1, se tudo o que segue é verdadeiro: o indicador “rule enabled” está ativado; o kind é “symmetric”; e o indicador “first is stated” está ativado;<br>2. em todos os outros casos, 0. |
| **DR-40 Chain Fired Unit** | O campo “chain fired unit” do firing é determinado pela seguinte prioridade:<br>1. 1, se tudo o que segue é verdadeiro: o indicador “rule enabled” está ativado; o kind é “chain”; o indicador “first holds before chain” está ativado; e o indicador “second holds before chain” está ativado;<br>2. em todos os outros casos, 0. |
| **DR-41 Type Fired Unit** | O campo “type fired unit” do firing é determinado pela seguinte prioridade:<br>1. 1, se tudo o que segue é verdadeiro: o indicador “rule enabled” está ativado; pelo menos uma das seguintes é verdadeira: o kind é “domain” ou o kind é “range”; e o indicador “first holds before typing” está ativado;<br>2. em todos os outros casos, 0. |
| **DR-42 Fired Unit** | Um firing: fired unit é calculado como o inverse fired unit mais o symmetric fired unit mais o chain fired unit mais o type fired unit. |
| **DR-43 Name** | O campo “name” do firing é determinado pela seguinte prioridade:<br>1. o target name, seguido de “: worked out by the ”, seguido de o rule label, seguido de “ rule from '”, seguido de o first name, seguido de “'”, seguido de “ and '”, seguido de o second name, seguido de “'”, se o fired unit é maior que 0;<br>2. em todos os outros casos, o firing ID. |
| **DR-44 Name** | Um disjoint check: name é igual a disjoint check ID. |
| **DR-45 Rule Enabled** | Um disjoint check: rule enabled é verdadeiro quando o rule vinculado é enabled. |
| **DR-46 A Holds** | Um disjoint check: a holds é verdadeiro quando o a vinculado é holds. |
| **DR-47 B Holds** | Um disjoint check: b holds é verdadeiro quando o b vinculado é holds. |
| **DR-48 Violation Unit** | O campo “violation unit” do disjoint check é determinado pela seguinte prioridade:<br>1. 1, se tudo o que segue é verdadeiro: o indicador “rule enabled” está ativado; o indicador “a holds” está ativado; e o indicador “b holds” está ativado;<br>2. em todos os outros casos, 0. |

## 5 Rastreabilidade ao esquema

_A coluna de expressão é a definição da regra em notação Documentação —
a mesma lógica que o livro de regras armazena, escrita para um leitor de negócio._

| Elemento do esquema | Tipo | Expressão |
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
| **Statements.SubjectLabel** | consulta | `Lookup(Things.Label via Subject)` |
| **Statements.PropertyLabel** | consulta | `Lookup(Properties.Label via Property)` |
| **Statements.ObjectLabel** | consulta | `Lookup(Things.Label via ObjectThing)` |
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
| **Firings.RuleEnabled** | consulta | `Lookup(Rules.IsEnabled via Rule)` |
| **Firings.RuleLabel** | consulta | `Lookup(Rules.Label via Rule)` |
| **Firings.TargetName** | consulta | `Lookup(Statements.Name via Target)` |
| **Firings.FirstName** | consulta | `Lookup(Statements.Name via First)` |
| **Firings.SecondName** | consulta | `Lookup(Statements.Name via Second)` |
| **Firings.FirstIsStated** | consulta | `Lookup(Statements.IsStated via First)` |
| **Firings.FirstHoldsBeforeChain** | consulta | `Lookup(Statements.HoldsBeforeChain via First)` |
| **Firings.SecondHoldsBeforeChain** | consulta | `Lookup(Statements.HoldsBeforeChain via Second)` |
| **Firings.FirstHoldsBeforeTyping** | consulta | `Lookup(Statements.HoldsBeforeTyping via First)` |
| **Firings.InverseFiredUnit** | fórmula | `If(And(RuleEnabled, Kind = "inverse", FirstIsStated), 1, 0)` |
| **Firings.SymmetricFiredUnit** | fórmula | `If(And(RuleEnabled, Kind = "symmetric", FirstIsStated), 1, 0)` |
| **Firings.ChainFiredUnit** | fórmula | `If(And(RuleEnabled, Kind = "chain", FirstHoldsBeforeChain, SecondHoldsBeforeChain), 1, 0)` |
| **Firings.TypeFiredUnit** | fórmula | `If(And(RuleEnabled, Or(Kind = "domain", Kind = "range"), FirstHoldsBeforeTyping), 1, 0)` |
| **Firings.FiredUnit** | fórmula | `InverseFiredUnit + SymmetricFiredUnit + ChainFiredUnit + TypeFiredUnit` |
| **Firings.Name** | fórmula | `If(FiredUnit > 0, TargetName & ": worked out by the " & RuleLabel & " rule from '" & FirstName & "'" & If(Kind = "chain", " and '" & SecondName & "'", ""), FiringId)` |
| **DisjointChecks.Name** | fórmula | `DisjointCheckId` |
| **DisjointChecks.RuleEnabled** | consulta | `Lookup(Rules.IsEnabled via Rule)` |
| **DisjointChecks.AHolds** | consulta | `Lookup(Statements.Holds via A)` |
| **DisjointChecks.BHolds** | consulta | `Lookup(Statements.Holds via B)` |
| **DisjointChecks.ViolationUnit** | fórmula | `If(And(RuleEnabled, AHolds, BHolds), 1, 0)` |
