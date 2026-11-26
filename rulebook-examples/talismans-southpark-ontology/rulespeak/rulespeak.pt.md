# 📘 Talisman's South Park ontology explainer — Documentação

_Jessica Talisman's South Park Ontology Explainer, as a rulebook: the page's ten facts, five rule switches, property wiring, provenance wording and current-value policy are rows, and every conclusion is a formula. The rules apply in three fixed strata (inverse and symmetric, then the property chain, then domain and range); property wiring that feeds an earlier stratum would need repeated rounds._

> Regras de negócio declarativas geradas a partir do livro de regras. Cada
> enunciado expressa uma verdade do domínio de negócio — não é um procedimento
> nem um imperativo. As fórmulas do livro de regras são a única fonte da verdade;
> este documento é a sua leitura em linguagem clara.

## 1 Vocabulário de negócio

| Termo | Descrição | Comentário narrativo |
|------|-------------|-------------------|
| **Graph** | The explainer page as one graph: its wording variables and its totals. | — |
| Label | Um atributo definido. | _Display name of the graph._ |
| Literal Quote | Um atributo definido. | _The quote mark written around a literal value in a fact's wording._ |
| Literal Type Marker | Um atributo definido. | _The marker written between a literal value and its datatype, as in Turtle._ |
| Built in Source Text | Um atributo definido. | _Source line shown for a fact the page states as a teaching example._ |
| User Source Text | Um atributo definido. | _Source line shown for a fact the reader stated._ |
| No Conclusions Text | Um atributo definido. | _Message shown when the selected facts and rules add no conclusion._ |
| Name | Igual a label. | _Order 1. Display name of the graph._ |
| Count of Stated Facts | O total de in effect unit entre os statements relacionados a o graph. | _Order 5. Facts that are switched on and in effect._ |
| Count of Conclusions | O total de conclusion unit entre os statements relacionados a o graph. | _Order 18. Facts that hold only because an enabled rule concluded them._ |
| Count of Violations | O total de violation unit entre os disjoint checks relacionados a o graph. | _Order 18. Disjointness checks that found both links between the same pair._ |
| Is Consistent | Verdadeiro quando o count of violations é 0. | _Order 19. True when no enabled disjointness rule is violated._ |
| Conclusions Message | Determinado por prioridade: o no conclusions text se o count of conclusions é 0; em todos os outros casos, uma cadeia vazia. | _Order 19. The empty-state message for the conclusions panel, blank when there are conclusions._ |
| Count of Facts to Add | O total de listed off unit entre os statements relacionados a o graph. | _Order 5. Listed facts that are not in effect; the Add next fact button is enabled while this is above zero._ |
| Has Facts to Add | Verdadeiro quando o count of facts to add é maior que 0. | _Order 6. True while some listed fact can still be added._ |
| Next Fact Sort Order | O menor valor de sort order entre os statements do graph que são listed off unit. | _Order 5. List position of the first listed fact that is not in effect._ |
| Max Activation Seq | O maior valor de activation seq entre os statements relacionados a o graph. | _Order 1. Highest activation number used so far._ |
| Next Activation Seq | Calculado como o max activation seq mais 1. | _Order 2. The activation number the next switched-on fact takes._ |
| Max Listed Sort Order | O maior valor de sort order entre os statements do graph que são listed unit. | _Order 2. Highest list position among listed facts._ |
| Next Listed Sort Order | Calculado como o max listed sort order mais 1. | _Order 3. The list position a newly added fact takes._ |

<details open>
<summary>Dados de exemplo</summary>

| Label | Literal Quote | Literal Type Marker | Built in Source Text | User Source Text | No Conclusions Text | Name ƒ | Count of Stated Facts ƒ | Count of Conclusions ƒ | Count of Violations ƒ | Is Consistent ƒ | Conclusions Message ƒ | Count of Facts to Add ƒ | Has Facts to Add ƒ | Next Fact Sort Order ƒ | Max Activation Seq ƒ | Next Activation Seq ƒ | Max Listed Sort Order ƒ | Next Listed Sort Order ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| South Park | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | These facts and selected rules add no further conclusions. Try another fact or rule. | South Park | 10 | 0 | 0 | true | These facts and selected rules add no further conclusions. Try another fact or rule. | 0 | false | — | 10 | 11 | 10 | 11 |

_ƒ marca uma coluna calculada._

</details>

| Termo | Descrição | Comentário narrativo |
|------|-------------|-------------------|
| **Rule** | The five rule switches on the page. | — |
| Label | Um atributo definido. | _The rule's name on the page._ |
| Explanation | Um atributo definido. | _The rule's one-line explanation on the page._ |
| Is Enabled | Verdadeiro quando uma cadeia vazia. | _The rule's checkbox._ |
| Sort Order | Um atributo definido. | _Position in the rule list._ |
| Name | Igual a label. | _Order 1. The rule's name._ |
| Count of Firings | O total de fired unit entre os firings relacionados a o rule. | _Order 15. Rule applications that fired, counting every derivation._ |

<details open>
<summary>Dados de exemplo</summary>

| Label | Explanation | Is Enabled | Sort Order | Name ƒ | Count of Firings ƒ |
|---|---|---|---|---|---|
| Inverse | A parent link also gives a child link between the same two people. | false | 1 | Inverse | 0 |
| Symmetric | A friend link or a spouse link is true for both people. | false | 2 | Symmetric | 0 |
| Property chain | A parent of a parent is a grandparent. | false | 3 | Property chain | 0 |

_ƒ marca uma coluna calculada._

</details>

| Termo | Descrição | Comentário narrativo |
|------|-------------|-------------------|
| **Firing Kind** | The ways a rule can apply. Domain and range belong to one rule but are worded apart. | — |
| Rule | Um atributo definido. | _The rule switch this kind belongs to._ |
| Wording | Um atributo definido. | _How a conclusion card names the rule, as in 'Worked out by the inverse rule'._ |
| Sort Order | Um atributo definido. | _Order in which the page's reasoner tries the kinds for one fact._ |
| Name | Igual a wording. | _Order 1. The kind's wording._ |
| Rule Enabled | Verdadeiro quando o rule vinculado é enabled. | _Order 1. Whether the owning rule is switched on._ |

<details open>
<summary>Dados de exemplo</summary>

| Wording | Sort Order | Name ƒ | Rule Enabled ƒ |
|---|---|---|---|
| inverse rule | 1 | inverse rule | false |
| symmetric rule | 2 | symmetric rule | false |
| property chain rule | 3 | property chain rule | false |

_ƒ marca uma coluna calculada._

</details>

| Termo | Descrição | Comentário narrativo |
|------|-------------|-------------------|
| **Thing** | Every individual and class the page can draw. | — |
| Label | Um atributo definido. | _The name the page shows._ |
| Kind | Um atributo definido. | _'individual' or 'class'._ |
| Sort Order | Um atributo definido. | _Position in the cast._ |
| Portrait Name | Um atributo definido. | _The name on the portrait's Fandom page._ |
| Portrait Page | Um atributo definido. | _The Fandom page credited for the portrait._ |
| Portrait Image | Um atributo definido. | _The original Fandom image._ |
| Portrait Crop | Um atributo definido. | _The portrait's crop as an SVG viewBox: x y width height._ |
| Portrait Width | Um atributo definido. | _The original image's width in pixels._ |
| Portrait Height | Um atributo definido. | _The original image's height in pixels._ |
| Name | Igual a label. | _Order 1. The thing's name._ |
| Node Key | Igual a thing ID. | _Order 1. The key the graph draws this thing under._ |
| Count of Children | O total de parent unit entre os statements relacionados a o thing. | _Order 17. People this thing is a parent of, stated or concluded._ |
| Count of Grandchildren | O total de grandparent unit entre os statements relacionados a o thing. | _Order 17. People this thing is a grandparent of._ |
| Count As Place | O total de place unit entre os statements relacionados a o thing. | _Order 17. Holding attends facts whose object is this thing._ |
| Count As Subject | O total de holds unit entre os statements relacionados a o thing. | _Order 17. Holding facts about this thing._ |
| Count As Object | O total de holds unit entre os statements relacionados a o thing. | _Order 17. Holding facts that point at this thing._ |
| Is in Graph | Verdadeiro quando pelo menos uma das seguintes é verdadeira: o count as subject é maior que 0 ou o count as object é maior que 0. | _Order 18. True when the graph draws this thing._ |
| Node Kind | Determinado por prioridade: “class” se o kind é “class”; “place” se o count as place é maior que 0; em todos os outros casos, “person”. | _Order 18. How the graph draws the thing: a class, a place someone attends, or a person._ |

<details open>
<summary>Dados de exemplo</summary>

| Label | Kind | Sort Order | Portrait Name | Portrait Page | Portrait Image | Portrait Crop | Portrait Width | Portrait Height | Name ƒ | Node Key ƒ | Count of Children ƒ | Count of Grandchildren ƒ | Count As Place ƒ | Count As Subject ƒ | Count As Object ƒ | Is in Graph ƒ | Node Kind ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| Marvin | individual | 1 | Marvin Marsh | https://southpark.fandom.com/wiki/Marvin_Marsh | https://static.wikia.nocookie.net/southpark/images/d/db/Marsh-family-grandpa-marsh-w-purple-blanket.png/revision/latest | 48 -5 280 280 | 375 | 536 | Marvin | Marvin | 1 | 0 | 0 | 1 | 0 | true | person |
| Randy | individual | 2 | Randy Marsh | https://southpark.fandom.com/wiki/Randy_Marsh | https://static.wikia.nocookie.net/southparkfanon/images/d/d6/Randy_Marsh.png/revision/latest | 0 -10 428 428 | 428 | 1057 | Randy | Randy | 2 | 0 | 0 | 3 | 1 | true | person |
| Sharon | individual | 3 | Sharon Marsh | https://southpark.fandom.com/wiki/Sharon_Marsh | https://static.wikia.nocookie.net/southpark/images/9/99/Sharon_Marsh.png/revision/latest | 20 -10 390 390 | 439 | 1049 | Sharon | Sharon | 0 | 0 | 0 | 0 | 1 | true | person |

_ƒ marca uma coluna calculada._

</details>

| Termo | Descrição | Comentário narrativo |
|------|-------------|-------------------|
| **Property** | The ten properties and the wiring every rule reads. | — |
| Label | Um atributo definido. | _The property's wording on the page._ |
| Property Kind | Um atributo definido. | _'object' links two things, 'datatype' links a thing to a value, 'type' states class membership._ |
| Datatype | Um atributo definido. | _The value's XML Schema datatype for a datatype property; blank otherwise._ |
| Is User Addable | Verdadeiro quando uma cadeia vazia. | _Offered in the page's Add fact form._ |
| Has One Current Value | Verdadeiro quando uma cadeia vazia. | _The page keeps one current value per person: switching a value on replaces the earlier one._ |
| Inverse of | Um atributo definido. | _The property the inverse rule derives from this one._ |
| Is Symmetric | Verdadeiro quando uma cadeia vazia. | _The symmetric rule makes this link true in both directions._ |
| Chain First | Um atributo definido. | _First link of the property chain that derives this property._ |
| Chain Second | Um atributo definido. | _Second link of the property chain that derives this property._ |
| Domain Class | Um atributo definido. | _The class the domain rule gives this property's subject._ |
| Range Class | Um atributo definido. | _The class the range rule gives this property's object._ |
| Disjoint With | Um atributo definido. | _A property that may not link the same pair, in the same direction, as this one._ |
| Sort Order | Um atributo definido. | _Position in the Add fact form's property list._ |
| Value Node Label | Um atributo definido. | _The small label under a value node for this property: 'role value' or 'number value'._ |
| Current Value Wording | Um atributo definido. | _How the Add fact form names a replaced value, as in 'Stan now has grade level 5'._ |
| Input Name | Um atributo definido. | _How the integer check names this property's value, as in 'for grade level'._ |
| Record Kind | Um atributo definido. | _The word the current-record summary uses for this property's values: role, grade or school._ |
| Record Order | Um atributo definido. | _Position of this property in the current-record summary._ |
| Name | Igual a label. | _Order 1. The property's wording._ |

<details open>
<summary>Dados de exemplo</summary>

| Label | Property Kind | Datatype | Is User Addable | Has One Current Value | Is Symmetric | Sort Order | Value Node Label | Current Value Wording | Input Name | Record Kind | Record Order | Name ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|
| is parent of | object | — | true | false | false | 1 | — | — | — | — | — | is parent of |
| is child of | object | — | true | false | false | 2 | — | — | — | — | — | is child of |
| is spouse of | object | — | true | false | true | 3 | — | — | — | — | — | is spouse of |

_ƒ marca uma coluna calculada._

</details>

| Termo | Descrição | Comentário narrativo |
|------|-------------|-------------------|
| **Statement** | Every fact the page can state or conclude about the cast. A listed fact appears in the page's fact list; the rest are candidates that only a rule can make hold. | — |
| Graph | Um atributo definido. | _The graph the fact belongs to._ |
| Subject | Um atributo definido. | _What the fact describes._ |
| Property | Um atributo definido. | _How the subject connects to the object._ |
| Object Thing | Um atributo definido. | _The related thing, for an object or type property._ |
| Object Value | Um atributo definido. | _The literal value, for a datatype property._ |
| Is Listed | Verdadeiro quando uma cadeia vazia. | _Shown in the page's fact list._ |
| Is Built in | Verdadeiro quando uma cadeia vazia. | _One of the page's teaching facts; the reader cannot remove it._ |
| Is Detail | Verdadeiro quando uma cadeia vazia. | _One of Stan's detail facts that Show Stan's details switches on._ |
| Is Stated | Verdadeiro quando uma cadeia vazia. | _The fact's checkbox._ |
| Activation Seq | Um atributo definido. | _When the fact was last switched on, as a running number; decides which value is current._ |
| Sort Order | Um atributo definido. | _Position in the fact list; candidates follow the listed facts._ |
| Subject Label | Obtido do subject vinculado. | _Order 1. The subject's name._ |
| Property Label | Obtido do property vinculado. | _Order 1. The property's wording._ |
| Object Label | Obtido do object thing vinculado. | _Order 1. The object's name, for an object or type property._ |
| Property Datatype | Obtido do property vinculado. | _Order 1. The value's datatype, for a datatype property._ |
| Property Has One Current Value | Verdadeiro quando o property vinculado tem um one current value. | _Order 1. Whether the property keeps one current value per person._ |
| Property Domain Class | Obtido do property vinculado. | _Order 1. The property's domain class._ |
| Property Range Class | Obtido do property vinculado. | _Order 1. The property's range class._ |
| Literal Quote | Obtido do graph vinculado. | _Order 1. The graph's literal quote mark._ |
| Literal Type Marker | Obtido do graph vinculado. | _Order 1. The graph's datatype marker._ |
| Built in Source Text | Obtido do graph vinculado. | _Order 1. The graph's source line for a teaching fact._ |
| User Source Text | Obtido do graph vinculado. | _Order 1. The graph's source line for a reader's fact._ |
| Next Fact Sort Order | Obtido do graph vinculado. | _Order 6. List position of the next fact to add._ |
| Is Literal | Verdadeiro quando o object thing está vazio. | _Order 1. True when the object is a value rather than a thing._ |
| Object Text | Determinado por prioridade: o object value se o object thing está vazio; em todos os outros casos, o object label. | _Order 2. The object's name or the value._ |
| Name | Calculado como o subject label, seguido de um espaço, seguido de o property label, seguido de um espaço, seguido de o object text. | _Order 3. The fact in words._ |
| Words | Calculado como o subject label, seguido de um espaço, seguido de o property label, seguido de um espaço, seguido de o literal quote, seguido de o object value, seguido de o literal quote, seguido de o literal type marker, seguido de o property datatype se o object thing está vazio, em todos os outros casos o object label. ⚠︎ mecânica <!-- rulespeak:reword --> | _Order 2. The fact as a conclusion card quotes it, with a literal written in Turtle form._ |
| Source | Determinado por prioridade: o built in source text se o indicador “built in” está ativado; em todos os outros casos, o user source text. | _Order 2. Where a listed fact came from._ |
| Subject Property Key | Calculado como o subject, seguido de “|”, seguido de o property. | _Order 1. Groups the values one subject has for one property._ |
| Latest Sibling Activation Seq | O maior valor de activation seq entre os statements relacionados a o statement. | _Order 2. The most recent activation among this subject's values for this property._ |
| Is in Effect | Verdadeiro quando tudo o que segue é verdadeiro: o indicador “stated” está ativado e pelo menos uma das seguintes é verdadeira: o indicador “property has one current value” não está ativado ou o activation seq é o latest sibling activation seq. | _Order 3. Switched on and, for a one-value property, the most recently switched-on value. Drawn as a solid line._ |
| Listed Unit | Determinado por prioridade: 1 se o indicador “listed” está ativado; em todos os outros casos, 0. | _Order 1. 1 when the fact is in the fact list._ |
| In Effect Unit | Determinado por prioridade: 1 se o indicador “in effect” está ativado; em todos os outros casos, 0. | _Order 4. 1 when the fact is in effect._ |
| Listed Off Unit | Determinado por prioridade: 1 se tudo o que segue é verdadeiro: o indicador “listed” está ativado e o indicador “in effect” não está ativado; em todos os outros casos, 0. | _Order 4. 1 when the fact is listed but not in effect._ |
| Is Next to Add | Verdadeiro quando tudo o que segue é verdadeiro: o listed off unit é 1 e o sort order é o next fact sort order. | _Order 7. The fact the Add next fact button switches on._ |
| Count of Inverse Firings | O total de inverse fired unit entre os firings relacionados a o statement. | _Order 6. Inverse rule applications that conclude this fact._ |
| Count of Symmetric Firings | O total de symmetric fired unit entre os firings relacionados a o statement. | _Order 6. Symmetric rule applications that conclude this fact._ |
| Holds Before Chain | Verdadeiro quando pelo menos uma das seguintes é verdadeira: o indicador “in effect” está ativado; o count of inverse firings é maior que 0; ou o count of symmetric firings é maior que 0. | _Order 7. Holds from facts in effect plus the inverse and symmetric rules._ |
| Count of Chain Firings | O total de chain fired unit entre os firings relacionados a o statement. | _Order 10. Property chain applications that conclude this fact._ |
| Holds Before Typing | Verdadeiro quando pelo menos uma das seguintes é verdadeira: o indicador “holds before chain” está ativado ou o count of chain firings é maior que 0. | _Order 11. Holds before the domain and range rules apply._ |
| Count of Type Firings | O total de type fired unit entre os firings relacionados a o statement. | _Order 14. Domain and range applications that conclude this fact._ |
| Holds | Verdadeiro quando pelo menos uma das seguintes é verdadeira: o indicador “holds before typing” está ativado ou o count of type firings é maior que 0. | _Order 15. True when in effect or concluded by an enabled rule; the graph draws it._ |
| Is Conclusion | Verdadeiro quando tudo o que segue é verdadeiro: o indicador “holds” está ativado e o indicador “in effect” não está ativado. | _Order 16. Holds only because a rule concluded it. Drawn dashed._ |
| Holds Unit | Determinado por prioridade: 1 se o indicador “holds” está ativado; em todos os outros casos, 0. | _Order 16. 1 when the fact holds._ |
| Conclusion Unit | Determinado por prioridade: 1 se o indicador “conclusion” está ativado; em todos os outros casos, 0. | _Order 17. 1 when the fact is a conclusion._ |
| Parent Unit | Determinado por prioridade: 1 se tudo o que segue é verdadeiro: o indicador “holds” está ativado e o property é “isParentOf”; em todos os outros casos, 0. | _Order 16. 1 for a holding parent link._ |
| Grandparent Unit | Determinado por prioridade: 1 se tudo o que segue é verdadeiro: o indicador “holds” está ativado e o property é “isGrandparentOf”; em todos os outros casos, 0. | _Order 16. 1 for a holding grandparent link._ |
| Place Unit | Determinado por prioridade: 1 se tudo o que segue é verdadeiro: o indicador “holds” está ativado e o property é “attends”; em todos os outros casos, 0. | _Order 16. 1 for a holding attends link._ |
| First Fired Order Key | O menor valor de order key entre os firings do statement que são fired unit. | _Order 15. Order key of the first rule application that concludes this fact, in the page reasoner's order._ |
| Count of Chosen Firings | O total de chosen unit entre os firings relacionados a o statement. | _Order 20. Rule applications shown as this fact's conclusion card: one for a conclusion, zero otherwise._ |
| Subject Node Key | Igual a subject. | _Order 1. The graph node this fact starts from._ |
| Object Node Key | Determinado por prioridade: “literal|”, seguido de o object value, seguido de “|”, seguido de o property datatype se o object thing está vazio; em todos os outros casos, o object thing. | _Order 2. The graph node this fact points at: the thing, or one node per value and datatype._ |
| Subject Node Kind | Obtido do subject vinculado. | _Order 19. How the graph draws the subject._ |
| Object Thing Node Kind | Obtido do object thing vinculado. | _Order 19. How the graph draws the object thing._ |
| Object Node Kind | Determinado por prioridade: “value” se o object thing está vazio; em todos os outros casos, o object thing node kind. | _Order 20. How the graph draws the object: a value, a class, a place or a person._ |
| Object Value Node Label | Obtido do property vinculado. | _Order 1. The small label under a value node._ |
| Count of Violations As First | O total de violation unit entre os disjoint checks relacionados a o statement. | _Order 18. Violated disjointness checks naming this fact first._ |
| Count of Violations As Second | O total de violation unit entre os disjoint checks relacionados a o statement. | _Order 18. Violated disjointness checks naming this fact second._ |
| Is in Contradiction | Verdadeiro quando pelo menos uma das seguintes é verdadeira: o count of violations as first é maior que 0 ou o count of violations as second é maior que 0. | _Order 19. Part of a contradiction the disjoint rule found; drawn in the alarm colour._ |

<details open>
<summary>Dados de exemplo</summary>

| Object Value | Is Listed | Is Built in | Is Detail | Is Stated | Activation Seq | Sort Order | Subject Label ƒ | Property Label ƒ | Object Label ƒ | Property Datatype ƒ | Property Has One Current Value ƒ | Property Domain Class ƒ | Property Range Class ƒ | Literal Quote ƒ | Literal Type Marker ƒ | Built in Source Text ƒ | User Source Text ƒ | Next Fact Sort Order ƒ | Is Literal ƒ | Object Text ƒ | Name ƒ | Words ƒ | Source ƒ | Subject Property Key ƒ | Latest Sibling Activation Seq ƒ | Is in Effect ƒ | Listed Unit ƒ | In Effect Unit ƒ | Listed Off Unit ƒ | Is Next to Add ƒ | Count of Inverse Firings ƒ | Count of Symmetric Firings ƒ | Holds Before Chain ƒ | Count of Chain Firings ƒ | Holds Before Typing ƒ | Count of Type Firings ƒ | Holds ƒ | Is Conclusion ƒ | Holds Unit ƒ | Conclusion Unit ƒ | Parent Unit ƒ | Grandparent Unit ƒ | Place Unit ƒ | First Fired Order Key ƒ | Count of Chosen Firings ƒ | Subject Node Key ƒ | Object Node Key ƒ | Subject Node Kind ƒ | Object Thing Node Kind ƒ | Object Node Kind ƒ | Object Value Node Label ƒ | Count of Violations As First ƒ | Count of Violations As Second ƒ | Is in Contradiction ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| — | true | true | false | true | 1 | 1 | Marvin | is parent of | Randy | — | false | — | — | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | — | false | Randy | Marvin is parent of Randy | Marvin is parent of Randy | Stated as a teaching example on this page. | Marvin\|isParentOf | 1 | true | 1 | 1 | 0 | false | 0 | 0 | true | 0 | true | 0 | true | false | 1 | 0 | 1 | 0 | 0 | — | 0 | Marvin | Randy | person | person | person | — | 0 | 0 | false |
| — | false | false | false | false | 0 | 101 | Marvin | is parent of | Sharon | — | false | — | — | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | — | false | Sharon | Marvin is parent of Sharon | Marvin is parent of Sharon | Stated by you on this page. | Marvin\|isParentOf | 1 | false | 0 | 0 | 0 | false | 0 | 0 | false | 0 | false | 0 | false | false | 0 | 0 | 0 | 0 | 0 | — | 0 | Marvin | Sharon | person | person | person | — | 0 | 0 | false |
| — | false | false | false | false | 0 | 102 | Marvin | is parent of | Stan | — | false | — | — | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | — | false | Stan | Marvin is parent of Stan | Marvin is parent of Stan | Stated by you on this page. | Marvin\|isParentOf | 1 | false | 0 | 0 | 0 | false | 0 | 0 | false | 0 | false | 0 | false | false | 0 | 0 | 0 | 0 | 0 | — | 0 | Marvin | Stan | person | person | person | — | 0 | 0 | false |

_ƒ marca uma coluna calculada._

</details>

| Termo | Descrição | Comentário narrativo |
|------|-------------|-------------------|
| **Firing** | Every way an enabled rule could conclude a candidate fact from the facts that hold. | — |
| Kind | Um atributo definido. | _How the rule applies._ |
| Rule | Um atributo definido. | _The rule switch._ |
| Target | Um atributo definido. | _The fact this application concludes._ |
| First | Um atributo definido. | _The premise._ |
| Second | Um atributo definido. | _The second premise, for the property chain._ |
| Rule Enabled | Verdadeiro quando o rule vinculado é enabled. | _Order 1. Whether the rule is switched on._ |
| Kind Wording | Obtido do kind vinculado. | _Order 1. How the card names the rule._ |
| Kind Sort Order | Obtido do kind vinculado. | _Order 1. The kind's place in the reasoner's order._ |
| Target Name | Obtido do target vinculado. | _Order 4. The concluded fact in words._ |
| Target Subject Label | Obtido do target vinculado. | _Order 2. The concluded fact's subject._ |
| Target Property Label | Obtido do target vinculado. | _Order 2. The concluded fact's property._ |
| Target Object Text | Obtido do target vinculado. | _Order 3. The concluded fact's object._ |
| Target is Conclusion | Verdadeiro quando o target vinculado é um conclusion. | _Order 17. Whether the concluded fact is new._ |
| Target Subject Node Key | Obtido do target vinculado. | _Order 2. The concluded fact's subject node._ |
| Target Object Node Key | Obtido do target vinculado. | _Order 3. The concluded fact's object node._ |
| Target First Fired Order Key | Obtido do target vinculado. | _Order 16. The order key of the concluded fact's first application._ |
| First Words | Obtido do first vinculado. | _Order 3. The premise as the card quotes it._ |
| Second Words | Obtido do second vinculado. | _Order 3. The second premise as the card quotes it._ |
| First is in Effect | Verdadeiro quando o first vinculado é in effect. | _Order 4. Whether the premise is in effect._ |
| First Holds Before Chain | Verdadeiro quando o first vinculado é holds before chain. | _Order 8. Whether the premise holds before the chain._ |
| Second Holds Before Chain | Verdadeiro quando o second vinculado é holds before chain. | _Order 8. Whether the second premise holds before the chain._ |
| First Holds Before Typing | Verdadeiro quando o first vinculado é holds before typing. | _Order 12. Whether the premise holds before typing._ |
| First Property | Obtido do first vinculado. | _Order 1. The premise's property key, as the card's axiom names it._ |
| First Property Domain Class | Obtido do first vinculado. | _Order 2. The premise property's domain class._ |
| First Property Range Class | Obtido do first vinculado. | _Order 2. The premise property's range class._ |
| First Sort Order | Obtido do first vinculado. | _Order 1. The premise's list position._ |
| Second Sort Order | Obtido do second vinculado. | _Order 1. The second premise's list position._ |
| Inverse Fired Unit | Determinado por prioridade: 1 se tudo o que segue é verdadeiro: o indicador “rule enabled” está ativado; o kind é “inverse”; e o indicador “first is in effect” está ativado; em todos os outros casos, 0. | _Order 5. 1 when the inverse rule fires here._ |
| Symmetric Fired Unit | Determinado por prioridade: 1 se tudo o que segue é verdadeiro: o indicador “rule enabled” está ativado; o kind é “symmetric”; e o indicador “first is in effect” está ativado; em todos os outros casos, 0. | _Order 5. 1 when the symmetric rule fires here._ |
| Chain Fired Unit | Determinado por prioridade: 1 se tudo o que segue é verdadeiro: o indicador “rule enabled” está ativado; o kind é “chain”; o indicador “first holds before chain” está ativado; e o indicador “second holds before chain” está ativado; em todos os outros casos, 0. | _Order 9. 1 when the property chain fires here._ |
| Type Fired Unit | Determinado por prioridade: 1 se tudo o que segue é verdadeiro: o indicador “rule enabled” está ativado; pelo menos uma das seguintes é verdadeira: o kind é “domain” ou o kind é “range”; e o indicador “first holds before typing” está ativado; em todos os outros casos, 0. | _Order 13. 1 when the domain or range rule fires here._ |
| Fired Unit | Calculado como o inverse fired unit mais o symmetric fired unit mais o chain fired unit mais o type fired unit. | _Order 14. 1 when this application fires._ |
| Order Key | Calculado como o first sort order vezes 100000 mais o kind sort order vezes 1000 mais 0 se o second está vazio, em todos os outros casos o second sort order. | _Order 2. The page reasoner's order: by premise, then rule kind, then second premise._ |
| Is Chosen | Verdadeiro quando tudo o que segue é verdadeiro: o fired unit é 1; o indicador “target is conclusion” está ativado; e o order key é o target first fired order key. | _Order 18. The application the conclusions panel shows for its fact: the first one in the reasoner's order._ |
| Chosen Unit | Determinado por prioridade: 1 se o indicador “chosen” está ativado; em todos os outros casos, 0. | _Order 19. 1 when this application is shown._ |
| Card Text | Determinado por prioridade: “Worked out by the ”, seguido de o kind wording, seguido de “ from “”, seguido de o first words, seguido de “””, seguido de “ and “”, seguido de o second words, seguido de “”” se o kind é “chain”, em todos os outros casos “ and “”, seguido de o first property, seguido de “ has domain ”, seguido de o first property domain class, seguido de “”” se o kind é “domain”, em todos os outros casos “ and “”, seguido de o first property, seguido de “ has range ”, seguido de o first property range class, seguido de “””, seguido de um ponto se o indicador “chosen” está ativado; em todos os outros casos, uma cadeia vazia. ⚠︎ mecânica <!-- rulespeak:reword --> | _Order 19. The conclusion card's provenance line, in the page's wording._ |
| Name | Igual a firing ID. | _Order 1. The application's key._ |

<details open>
<summary>Dados de exemplo</summary>

| Rule Enabled ƒ | Kind Wording ƒ | Kind Sort Order ƒ | Target Name ƒ | Target Subject Label ƒ | Target Property Label ƒ | Target Object Text ƒ | Target is Conclusion ƒ | Target Subject Node Key ƒ | Target Object Node Key ƒ | Target First Fired Order Key ƒ | First Words ƒ | Second Words ƒ | First is in Effect ƒ | First Holds Before Chain ƒ | Second Holds Before Chain ƒ | First Holds Before Typing ƒ | First Property ƒ | First Property Domain Class ƒ | First Property Range Class ƒ | First Sort Order ƒ | Second Sort Order ƒ | Inverse Fired Unit ƒ | Symmetric Fired Unit ƒ | Chain Fired Unit ƒ | Type Fired Unit ƒ | Fired Unit ƒ | Order Key ƒ | Is Chosen ƒ | Chosen Unit ƒ | Card Text ƒ | Name ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| false | inverse rule | 1 | Randy is child of Marvin | Randy | is child of | Marvin | false | Randy | Marvin | — | Marvin is parent of Randy |    | true | true | false | true | isParentOf | — | — | 1 | — | 0 | 0 | 0 | 0 | 0 | 101000 | false | 0 | — | inverse:Marvin\|isParentOf\|Randy>Randy\|isChildOf\|Marvin |
| false | inverse rule | 1 | Sharon is child of Marvin | Sharon | is child of | Marvin | false | Sharon | Marvin | — | Marvin is parent of Sharon |    | false | false | false | false | isParentOf | — | — | 101 | — | 0 | 0 | 0 | 0 | 0 | 10101000 | false | 0 | — | inverse:Marvin\|isParentOf\|Sharon>Sharon\|isChildOf\|Marvin |
| false | inverse rule | 1 | Stan is child of Marvin | Stan | is child of | Marvin | false | Stan | Marvin | — | Marvin is parent of Stan |    | false | false | false | false | isParentOf | — | — | 102 | — | 0 | 0 | 0 | 0 | 0 | 10201000 | false | 0 | — | inverse:Marvin\|isParentOf\|Stan>Stan\|isChildOf\|Marvin |

_ƒ marca uma coluna calculada._

</details>

| Termo | Descrição | Comentário narrativo |
|------|-------------|-------------------|
| **Disjoint Check** | Every pair the disjointness rule watches: the same two people linked by two disjoint properties. | — |
| Graph | Um atributo definido. | _The graph._ |
| Rule | Um atributo definido. | _The rule switch._ |
| First | Um atributo definido. | _The first link._ |
| Second | Um atributo definido. | _The disjoint link between the same pair, in the same direction._ |
| Rule Enabled | Verdadeiro quando o rule vinculado é enabled. | _Order 1. Whether the disjointness rule is on._ |
| First Holds | Verdadeiro quando o first vinculado é holds. | _Order 16. Whether the first link holds._ |
| Second Holds | Verdadeiro quando o second vinculado é holds. | _Order 16. Whether the second link holds._ |
| Subject Label | Obtido do first vinculado. | _Order 2. The person recorded with both links._ |
| Object Label | Obtido do first vinculado. | _Order 2. The other person._ |
| Violation Unit | Determinado por prioridade: 1 se tudo o que segue é verdadeiro: o indicador “rule enabled” está ativado; o indicador “first holds” está ativado; e o indicador “second holds” está ativado; em todos os outros casos, 0. | _Order 17. 1 when both links hold._ |
| Alarm Text | Determinado por prioridade: “Contradiction found. ”, seguido de o subject label, seguido de “ is recorded as both parent and spouse of ”, seguido de o object label, seguido de “. Check those facts and the selected disjoint rule.” se o violation unit é 1; em todos os outros casos, uma cadeia vazia. | _Order 18. The page's contradiction alert, blank when there is none._ |
| Name | Igual a disjoint check ID. | _Order 1. The check's key._ |

<details open>
<summary>Dados de exemplo</summary>

| Rule Enabled ƒ | First Holds ƒ | Second Holds ƒ | Subject Label ƒ | Object Label ƒ | Violation Unit ƒ | Alarm Text ƒ | Name ƒ |
|---|---|---|---|---|---|---|---|
| false | true | false | Marvin | Randy | 0 | — | Marvin\|isParentOf\|Randy~isSpouseOf |
| false | false | false | Marvin | Sharon | 0 | — | Marvin\|isParentOf\|Sharon~isSpouseOf |
| false | false | false | Marvin | Stan | 0 | — | Marvin\|isParentOf\|Stan~isSpouseOf |

_ƒ marca uma coluna calculada._

</details>

| Termo | Descrição | Comentário narrativo |
|------|-------------|-------------------|
| **Scenario** | The page's buttons. Each one is a fact baseline, explicit fact changes and rule settings, applied in that order. | — |
| Graph | Um atributo definido. | _The graph._ |
| Label | Um atributo definido. | _The button's text._ |
| Note | Um atributo definido. | _The note shown under the buttons after this one is pressed; blank clears it._ |
| Sort Order | Um atributo definido. | _Position among the buttons._ |
| Button Group | Um atributo definido. | _'stan' for the two imagined changes, 'facts' for the buttons under the fact list._ |
| Is Primary | Verdadeiro quando uma cadeia vazia. | _Drawn as the filled button._ |
| Fact Baseline | Um atributo definido. | _What happens to every listed fact first: keep, built-in (teaching facts on, others off), built-in-detail, all-off, activate-all (each switched on in list order), or next (the next fact on)._ |
| Layout | Um atributo definido. | _The node layout the graph returns to; blank keeps the current positions._ |
| Clears Selection | Verdadeiro quando uma cadeia vazia. | _Clears the selected node or line._ |
| Clears Form Message | Verdadeiro quando uma cadeia vazia. | _Clears the Add fact form's message._ |
| Name | Igual a label. | _Order 1. The button's text._ |
| Graph Has Facts to Add | Verdadeiro quando o graph vinculado tem um facts to add. | _Order 7. Whether a listed fact can still be added._ |
| Is Available | Verdadeiro quando pelo menos uma das seguintes é verdadeira: o fact baseline não é “next” ou o indicador “graph has facts to add” está ativado. | _Order 8. False only for Add next fact once every listed fact is in effect._ |

<details open>
<summary>Dados de exemplo</summary>

| Label | Note | Sort Order | Button Group | Is Primary | Fact Baseline | Layout | Clears Selection | Clears Form Message | Name ƒ | Graph Has Facts to Add ƒ | Is Available ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|
| Move Stan to grade 5 | This imagined change updates Stan’s grade to 5. His identifier, Person statement and family links stay the same. His other selected details are unchanged. | 1 | stan | false | keep | — | true | true | Move Stan to grade 5 | false | true |
| Stan leaves school | This imagined change clears Stan’s current school details. His identifier, Person statement and family links remain. Removing a current fact does not say that its opposite is true; school history needs dates. | 2 | stan | false | keep | — | true | true | Stan leaves school | false | true |
| Add next fact | — | 3 | facts | true | next | — | false | false | Add next fact | false | false |

_ƒ marca uma coluna calculada._

</details>

| Termo | Descrição | Comentário narrativo |
|------|-------------|-------------------|
| **Scenario Fact Change** | Fact changes a button makes after its baseline. A blank object key matches every listed value of the property. | — |
| Scenario | Um atributo definido. | _The button._ |
| Subject | Um atributo definido. | _The subject whose facts change._ |
| Property | Um atributo definido. | _The property whose facts change._ |
| Object Key | Um atributo definido. | _The object thing's key or the literal value; blank for every listed value._ |
| Action | Um atributo definido. | _'on' switches the fact on as the newest value; 'off' switches it off._ |
| Adds to List | Verdadeiro quando uma cadeia vazia. | _Adds the fact to the fact list first when it is not listed._ |
| Sort Order | Um atributo definido. | _Order of the change within the button._ |
| Name | Igual a scenario fact change ID. | _Order 1. The change's key._ |

<details open>
<summary>Dados de exemplo</summary>

| Object Key | Action | Adds to List | Sort Order | Name ƒ |
|---|---|---|---|---|
| Person | on | true | 1 | stan-grade-five:Stan\|isA\|Person |
| 5 | on | true | 2 | stan-grade-five:Stan\|hasGradeLevel\|5 |
| Person | on | true | 3 | stan-leaves-school:Stan\|isA\|Person |

_ƒ marca uma coluna calculada._

</details>

| Termo | Descrição | Comentário narrativo |
|------|-------------|-------------------|
| **Scenario Rule Setting** | Rule switches a button sets. | — |
| Scenario | Um atributo definido. | _The button._ |
| Rule | Um atributo definido. | _The rule switch._ |
| Is Enabled | Verdadeiro quando uma cadeia vazia. | _The state the button puts the switch in._ |
| Name | Igual a scenario rule setting ID. | _Order 1. The setting's key._ |

<details open>
<summary>Dados de exemplo</summary>

| Is Enabled | Name ƒ |
|---|---|
| false | reset-original:inverse |
| false | reset-original:symmetric |
| false | reset-original:chain |

_ƒ marca uma coluna calculada._

</details>

| Termo | Descrição | Comentário narrativo |
|------|-------------|-------------------|
| **Node Layout** | Node positions in the graph's 380 by 480 drawing, one set per layout. | — |
| Layout | Um atributo definido. | _The layout: 'default' or 'stan-details'._ |
| Node Key | Um atributo definido. | _The node: a thing's key, or literal\|value\|datatype._ |
| X | Um atributo definido. | _Horizontal position._ |
| Y | Um atributo definido. | _Vertical position._ |
| Name | Igual a node layout ID. | _Order 1. The position's key._ |

<details open>
<summary>Dados de exemplo</summary>

| Layout | Node Key | X | Y | Name ƒ |
|---|---|---|---|---|
| default | Marvin | 70 | 45 | default:Marvin |
| default | Randy | 195 | 125 | default:Randy |
| default | Sharon | 318 | 55 | default:Sharon |

_ƒ marca uma coluna calculada._

</details>

| Termo | Descrição | Comentário narrativo |
|------|-------------|-------------------|
| **Layout Slot** | Places for nodes no layout positions, used in turn. | — |
| Sort Order | Um atributo definido. | _Order in which slots are used._ |
| X | Um atributo definido. | _Horizontal position._ |
| Y | Um atributo definido. | _Vertical position._ |
| Name | Igual a layout slot ID. | _Order 1. The slot's key._ |

<details open>
<summary>Dados de exemplo</summary>

| Sort Order | X | Y | Name ƒ |
|---|---|---|---|
| 1 | 330 | 135 | slot-1 |
| 2 | 45 | 120 | slot-2 |
| 3 | 330 | 400 | slot-3 |

_ƒ marca uma coluna calculada._

</details>

| Termo | Descrição | Comentário narrativo |
|------|-------------|-------------------|
| **Page Text** | The page's wording, one row per string. {name}-style placeholders are filled by the page. | — |
| Text | Um atributo definido. | _The string._ |
| Name | Igual a page text ID. | _Order 1. The string's key._ |

<details open>
<summary>Dados de exemplo</summary>

| Text | Name ƒ |
|---|---|
| South Park Ontology Explainer, on one rulebook | page-title |
| A rulebook rebuild of the explainer by | byline-prefix |
| Jessica Talisman | byline-author |

_ƒ marca uma coluna calculada._

</details>

## 2 Tipos de fatos

- um **firing kind** referencia exatamente um **rule**
- um **property** pode referenciar um **property**
- um **property** pode referenciar um **thing**
- um **statement** referencia exatamente um **graph**
- um **statement** referencia exatamente um **thing**
- um **statement** referencia exatamente um **property**
- um **firing** referencia exatamente um **firing kind**
- um **firing** referencia exatamente um **rule**
- um **firing** referencia exatamente um **statement**
- um **disjoint check** referencia exatamente um **graph**
- um **disjoint check** referencia exatamente um **rule**
- um **disjoint check** referencia exatamente um **statement**
- um **scenario** referencia exatamente um **graph**
- um **scenario fact change** referencia exatamente um **scenario**
- um **scenario fact change** referencia exatamente um **thing**
- um **scenario fact change** referencia exatamente um **property**
- um **scenario rule setting** referencia exatamente um **scenario**
- um **scenario rule setting** referencia exatamente um **rule**

## 3 Regras operativas

_As regras operativas enunciam o que o negócio **obriga**, **proíbe** ou
aconselha (**deveria**). As regras estruturais vêm de campos obrigatórios e chaves
estrangeiras; as semânticas vêm da tabela Constraints, cada uma baseada num booleano
que o livro de regras já calcula (referenciado como DR-N nas regras definidoras)._

### Restrições estruturais (do esquema)

- Um graph **deve** ter um label; um literal quote; um literal type marker; um built in source text; um user source text; e um no conclusions text.
- Um rule **deve** ter um label; um explanation; e um sort order, e registrar se é enabled.
- Um firing kind **deve** referenciar exatamente um rule.
- Um firing kind **deve** ter um wording e um sort order.
- Um thing **deve** ter um label; um kind; e um sort order.
- Um property **deve** ter um label; um property kind; e um sort order, e registrar se é user addable; se tem um one current value; e se é symmetric.
- Um statement **deve** referenciar exatamente um graph.
- Um statement **deve** referenciar exatamente um thing como seu subject.
- Um statement **deve** referenciar exatamente um property.
- Um statement **deve** ter um activation seq e um sort order, e registrar se é listed; se é um built in; se é um detail; e se é stated.
- Um firing **deve** referenciar exatamente um firing kind como seu kind.
- Um firing **deve** referenciar exatamente um rule.
- Um firing **deve** referenciar exatamente um statement como seu target.
- Um firing **deve** referenciar exatamente um statement como seu first.
- Um disjoint check **deve** referenciar exatamente um graph.
- Um disjoint check **deve** referenciar exatamente um rule.
- Um disjoint check **deve** referenciar exatamente um statement como seu first.
- Um disjoint check **deve** referenciar exatamente um statement como seu second.
- Um scenario **deve** referenciar exatamente um graph.
- Um scenario **deve** ter um label; um sort order; um button group; e um fact baseline, e registrar se é primary; se é clears selection; e se é clears form message.
- Um scenario fact change **deve** referenciar exatamente um scenario.
- Um scenario fact change **deve** referenciar exatamente um thing como seu subject.
- Um scenario fact change **deve** referenciar exatamente um property.
- Um scenario fact change **deve** ter um action e um sort order, e registrar se é adds to list.
- Um scenario rule setting **deve** referenciar exatamente um scenario.
- Um scenario rule setting **deve** referenciar exatamente um rule.
- Um scenario rule setting **deve** registrar se é enabled.
- Um node layout **deve** ter um layout; um node key; um x; e um y.
- Um layout slot **deve** ter um sort order; um x; e um y.
- Um page text **deve** ter um text.

## 4 Regras definidoras

_Todos os enunciados expressam verdades do domínio de negócio; não são procedimentos
nem imperativos. Evita-se «se e somente se» em favor de «somente se», para que uma
necessidade unidirecional não seja confundida com uma equivalência. Uma marca
**⚠︎ mecânica** sinaliza uma regra cuja redação determinista é fiel mas
dura — um sinal para uma reescrita opcional, não um defeito._

| ID | Regra declarativa |
|----|------------------|
| **DR-1 Name** | Um graph: name é igual a label. |
| **DR-2 Count of Stated Facts** | Um graph: count of stated facts é o total de in effect unit entre os statements relacionados a o graph. |
| **DR-3 Count of Conclusions** | Um graph: count of conclusions é o total de conclusion unit entre os statements relacionados a o graph. |
| **DR-4 Count of Violations** | Um graph: count of violations é o total de violation unit entre os disjoint checks relacionados a o graph. |
| **DR-5 Is Consistent** | Um graph é considerado consistent se o count of violations é 0. |
| **DR-6 Conclusions Message** | O campo “conclusions message” do graph é determinado pela seguinte prioridade:<br>1. o no conclusions text, se o count of conclusions é 0;<br>2. em todos os outros casos, uma cadeia vazia. |
| **DR-7 Count of Facts to Add** | Um graph: count of facts to add é o total de listed off unit entre os statements relacionados a o graph. |
| **DR-8 Has Facts to Add** | Um graph é considerado como tendo um facts to add se o count of facts to add é maior que 0. |
| **DR-9 Next Fact Sort Order** | Um graph: next fact sort order é o menor valor de sort order entre os statements do graph que são listed off unit. |
| **DR-10 Max Activation Seq** | Um graph: max activation seq é o maior valor de activation seq entre os statements relacionados a o graph. |
| **DR-11 Next Activation Seq** | Um graph: next activation seq é calculado como o max activation seq mais 1. |
| **DR-12 Max Listed Sort Order** | Um graph: max listed sort order é o maior valor de sort order entre os statements do graph que são listed unit. |
| **DR-13 Next Listed Sort Order** | Um graph: next listed sort order é calculado como o max listed sort order mais 1. |
| **DR-14 Name** | Um rule: name é igual a label. |
| **DR-15 Count of Firings** | Um rule: count of firings é o total de fired unit entre os firings relacionados a o rule. |
| **DR-16 Name** | Um firing kind: name é igual a wording. |
| **DR-17 Rule Enabled** | Um firing kind: rule enabled é verdadeiro quando o rule vinculado é enabled. |
| **DR-18 Name** | Um thing: name é igual a label. |
| **DR-19 Node Key** | Um thing: node key é igual a thing ID. |
| **DR-20 Count of Children** | Um thing: count of children é o total de parent unit entre os statements relacionados a o thing. |
| **DR-21 Count of Grandchildren** | Um thing: count of grandchildren é o total de grandparent unit entre os statements relacionados a o thing. |
| **DR-22 Count As Place** | Um thing: count as place é o total de place unit entre os statements relacionados a o thing. |
| **DR-23 Count As Subject** | Um thing: count as subject é o total de holds unit entre os statements relacionados a o thing. |
| **DR-24 Count As Object** | Um thing: count as object é o total de holds unit entre os statements relacionados a o thing. |
| **DR-25 Is in Graph** | Um thing é considerado in graph se pelo menos uma das seguintes é verdadeira: o count as subject é maior que 0 ou o count as object é maior que 0. |
| **DR-26 Node Kind** | O campo “node kind” do thing é determinado pela seguinte prioridade:<br>1. “class”, se o kind é “class”;<br>2. “place”, se o count as place é maior que 0;<br>3. em todos os outros casos, “person”. |
| **DR-27 Name** | Um property: name é igual a label. |
| **DR-28 Subject Label** | Um statement: subject label — obtido do subject vinculado. |
| **DR-29 Property Label** | Um statement: property label — obtido do property vinculado. |
| **DR-30 Object Label** | Um statement: object label — obtido do object thing vinculado. |
| **DR-31 Property Datatype** | Um statement: property datatype — obtido do property vinculado. |
| **DR-32 Property Has One Current Value** | Um statement: property has one current value é verdadeiro quando o property vinculado tem um one current value. |
| **DR-33 Property Domain Class** | Um statement: property domain class — obtido do property vinculado. |
| **DR-34 Property Range Class** | Um statement: property range class — obtido do property vinculado. |
| **DR-35 Literal Quote** | Um statement: literal quote — obtido do graph vinculado. |
| **DR-36 Literal Type Marker** | Um statement: literal type marker — obtido do graph vinculado. |
| **DR-37 Built in Source Text** | Um statement: built in source text — obtido do graph vinculado. |
| **DR-38 User Source Text** | Um statement: user source text — obtido do graph vinculado. |
| **DR-39 Next Fact Sort Order** | Um statement: next fact sort order — obtido do graph vinculado. |
| **DR-40 Is Literal** | Um statement é considerado literal se o object thing está vazio. |
| **DR-41 Object Text** | O campo “object text” do statement é determinado pela seguinte prioridade:<br>1. o object value, se o object thing está vazio;<br>2. em todos os outros casos, o object label. |
| **DR-42 Name** | Um statement: name é calculado como o subject label, seguido de um espaço, seguido de o property label, seguido de um espaço, seguido de o object text. |
| **DR-43 Words** | Um statement: words é calculado como o subject label, seguido de um espaço, seguido de o property label, seguido de um espaço, seguido de o literal quote, seguido de o object value, seguido de o literal quote, seguido de o literal type marker, seguido de o property datatype se o object thing está vazio, em todos os outros casos o object label. ⚠︎ mecânica <!-- rulespeak:reword --> |
| **DR-44 Source** | O campo “source” do statement é determinado pela seguinte prioridade:<br>1. o built in source text, se o indicador “built in” está ativado;<br>2. em todos os outros casos, o user source text. |
| **DR-45 Subject Property Key** | Um statement: subject property key é calculado como o subject, seguido de “|”, seguido de o property. |
| **DR-46 Latest Sibling Activation Seq** | Um statement: latest sibling activation seq é o maior valor de activation seq entre os statements relacionados a o statement. |
| **DR-47 Is in Effect** | Um statement é considerado in effect se tudo o que segue é verdadeiro: o indicador “stated” está ativado e pelo menos uma das seguintes é verdadeira: o indicador “property has one current value” não está ativado ou o activation seq é o latest sibling activation seq. |
| **DR-48 Listed Unit** | O campo “listed unit” do statement é determinado pela seguinte prioridade:<br>1. 1, se o indicador “listed” está ativado;<br>2. em todos os outros casos, 0. |
| **DR-49 In Effect Unit** | O campo “in effect unit” do statement é determinado pela seguinte prioridade:<br>1. 1, se o indicador “in effect” está ativado;<br>2. em todos os outros casos, 0. |
| **DR-50 Listed Off Unit** | O campo “listed off unit” do statement é determinado pela seguinte prioridade:<br>1. 1, se tudo o que segue é verdadeiro: o indicador “listed” está ativado e o indicador “in effect” não está ativado;<br>2. em todos os outros casos, 0. |
| **DR-51 Is Next to Add** | Um statement é considerado um next to add se tudo o que segue é verdadeiro: o listed off unit é 1 e o sort order é o next fact sort order. |
| **DR-52 Count of Inverse Firings** | Um statement: count of inverse firings é o total de inverse fired unit entre os firings relacionados a o statement. |
| **DR-53 Count of Symmetric Firings** | Um statement: count of symmetric firings é o total de symmetric fired unit entre os firings relacionados a o statement. |
| **DR-54 Holds Before Chain** | Um statement é considerado como satisfazendo: before chain se pelo menos uma das seguintes é verdadeira: o indicador “in effect” está ativado; o count of inverse firings é maior que 0; ou o count of symmetric firings é maior que 0. |
| **DR-55 Count of Chain Firings** | Um statement: count of chain firings é o total de chain fired unit entre os firings relacionados a o statement. |
| **DR-56 Holds Before Typing** | Um statement é considerado como satisfazendo: before typing se pelo menos uma das seguintes é verdadeira: o indicador “holds before chain” está ativado ou o count of chain firings é maior que 0. |
| **DR-57 Count of Type Firings** | Um statement: count of type firings é o total de type fired unit entre os firings relacionados a o statement. |
| **DR-58 Holds** | Um statement é considerado como satisfazendo: se pelo menos uma das seguintes é verdadeira: o indicador “holds before typing” está ativado ou o count of type firings é maior que 0. |
| **DR-59 Is Conclusion** | Um statement é considerado um conclusion se tudo o que segue é verdadeiro: o indicador “holds” está ativado e o indicador “in effect” não está ativado. |
| **DR-60 Holds Unit** | O campo “holds unit” do statement é determinado pela seguinte prioridade:<br>1. 1, se o indicador “holds” está ativado;<br>2. em todos os outros casos, 0. |
| **DR-61 Conclusion Unit** | O campo “conclusion unit” do statement é determinado pela seguinte prioridade:<br>1. 1, se o indicador “conclusion” está ativado;<br>2. em todos os outros casos, 0. |
| **DR-62 Parent Unit** | O campo “parent unit” do statement é determinado pela seguinte prioridade:<br>1. 1, se tudo o que segue é verdadeiro: o indicador “holds” está ativado e o property é “isParentOf”;<br>2. em todos os outros casos, 0. |
| **DR-63 Grandparent Unit** | O campo “grandparent unit” do statement é determinado pela seguinte prioridade:<br>1. 1, se tudo o que segue é verdadeiro: o indicador “holds” está ativado e o property é “isGrandparentOf”;<br>2. em todos os outros casos, 0. |
| **DR-64 Place Unit** | O campo “place unit” do statement é determinado pela seguinte prioridade:<br>1. 1, se tudo o que segue é verdadeiro: o indicador “holds” está ativado e o property é “attends”;<br>2. em todos os outros casos, 0. |
| **DR-65 First Fired Order Key** | Um statement: first fired order key é o menor valor de order key entre os firings do statement que são fired unit. |
| **DR-66 Count of Chosen Firings** | Um statement: count of chosen firings é o total de chosen unit entre os firings relacionados a o statement. |
| **DR-67 Subject Node Key** | Um statement: subject node key é igual a subject. |
| **DR-68 Object Node Key** | O campo “object node key” do statement é determinado pela seguinte prioridade:<br>1. “literal|”, seguido de o object value, seguido de “|”, seguido de o property datatype, se o object thing está vazio;<br>2. em todos os outros casos, o object thing. |
| **DR-69 Subject Node Kind** | Um statement: subject node kind — obtido do subject vinculado. |
| **DR-70 Object Thing Node Kind** | Um statement: object thing node kind — obtido do object thing vinculado. |
| **DR-71 Object Node Kind** | O campo “object node kind” do statement é determinado pela seguinte prioridade:<br>1. “value”, se o object thing está vazio;<br>2. em todos os outros casos, o object thing node kind. |
| **DR-72 Object Value Node Label** | Um statement: object value node label — obtido do property vinculado. |
| **DR-73 Count of Violations As First** | Um statement: count of violations as first é o total de violation unit entre os disjoint checks relacionados a o statement. |
| **DR-74 Count of Violations As Second** | Um statement: count of violations as second é o total de violation unit entre os disjoint checks relacionados a o statement. |
| **DR-75 Is in Contradiction** | Um statement é considerado in contradiction se pelo menos uma das seguintes é verdadeira: o count of violations as first é maior que 0 ou o count of violations as second é maior que 0. |
| **DR-76 Rule Enabled** | Um firing: rule enabled é verdadeiro quando o rule vinculado é enabled. |
| **DR-77 Kind Wording** | Um firing: kind wording — obtido do kind vinculado. |
| **DR-78 Kind Sort Order** | Um firing: kind sort order — obtido do kind vinculado. |
| **DR-79 Target Name** | Um firing: target name — obtido do target vinculado. |
| **DR-80 Target Subject Label** | Um firing: target subject label — obtido do target vinculado. |
| **DR-81 Target Property Label** | Um firing: target property label — obtido do target vinculado. |
| **DR-82 Target Object Text** | Um firing: target object text — obtido do target vinculado. |
| **DR-83 Target is Conclusion** | Um firing: target is conclusion é verdadeiro quando o target vinculado é um conclusion. |
| **DR-84 Target Subject Node Key** | Um firing: target subject node key — obtido do target vinculado. |
| **DR-85 Target Object Node Key** | Um firing: target object node key — obtido do target vinculado. |
| **DR-86 Target First Fired Order Key** | Um firing: target first fired order key — obtido do target vinculado. |
| **DR-87 First Words** | Um firing: first words — obtido do first vinculado. |
| **DR-88 Second Words** | Um firing: second words — obtido do second vinculado. |
| **DR-89 First is in Effect** | Um firing: first is in effect é verdadeiro quando o first vinculado é in effect. |
| **DR-90 First Holds Before Chain** | Um firing: first holds before chain é verdadeiro quando o first vinculado é holds before chain. |
| **DR-91 Second Holds Before Chain** | Um firing: second holds before chain é verdadeiro quando o second vinculado é holds before chain. |
| **DR-92 First Holds Before Typing** | Um firing: first holds before typing é verdadeiro quando o first vinculado é holds before typing. |
| **DR-93 First Property** | Um firing: first property — obtido do first vinculado. |
| **DR-94 First Property Domain Class** | Um firing: first property domain class — obtido do first vinculado. |
| **DR-95 First Property Range Class** | Um firing: first property range class — obtido do first vinculado. |
| **DR-96 First Sort Order** | Um firing: first sort order — obtido do first vinculado. |
| **DR-97 Second Sort Order** | Um firing: second sort order — obtido do second vinculado. |
| **DR-98 Inverse Fired Unit** | O campo “inverse fired unit” do firing é determinado pela seguinte prioridade:<br>1. 1, se tudo o que segue é verdadeiro: o indicador “rule enabled” está ativado; o kind é “inverse”; e o indicador “first is in effect” está ativado;<br>2. em todos os outros casos, 0. |
| **DR-99 Symmetric Fired Unit** | O campo “symmetric fired unit” do firing é determinado pela seguinte prioridade:<br>1. 1, se tudo o que segue é verdadeiro: o indicador “rule enabled” está ativado; o kind é “symmetric”; e o indicador “first is in effect” está ativado;<br>2. em todos os outros casos, 0. |
| **DR-100 Chain Fired Unit** | O campo “chain fired unit” do firing é determinado pela seguinte prioridade:<br>1. 1, se tudo o que segue é verdadeiro: o indicador “rule enabled” está ativado; o kind é “chain”; o indicador “first holds before chain” está ativado; e o indicador “second holds before chain” está ativado;<br>2. em todos os outros casos, 0. |
| **DR-101 Type Fired Unit** | O campo “type fired unit” do firing é determinado pela seguinte prioridade:<br>1. 1, se tudo o que segue é verdadeiro: o indicador “rule enabled” está ativado; pelo menos uma das seguintes é verdadeira: o kind é “domain” ou o kind é “range”; e o indicador “first holds before typing” está ativado;<br>2. em todos os outros casos, 0. |
| **DR-102 Fired Unit** | Um firing: fired unit é calculado como o inverse fired unit mais o symmetric fired unit mais o chain fired unit mais o type fired unit. |
| **DR-103 Order Key** | Um firing: order key é calculado como o first sort order vezes 100000 mais o kind sort order vezes 1000 mais 0 se o second está vazio, em todos os outros casos o second sort order. |
| **DR-104 Is Chosen** | Um firing é considerado um chosen se tudo o que segue é verdadeiro: o fired unit é 1; o indicador “target is conclusion” está ativado; e o order key é o target first fired order key. |
| **DR-105 Chosen Unit** | O campo “chosen unit” do firing é determinado pela seguinte prioridade:<br>1. 1, se o indicador “chosen” está ativado;<br>2. em todos os outros casos, 0. |
| **DR-106 Card Text** | O campo “card text” do firing é determinado pela seguinte prioridade:<br>1. “Worked out by the ”, seguido de o kind wording, seguido de “ from “”, seguido de o first words, seguido de “””, seguido de “ and “”, seguido de o second words, seguido de “”” se o kind é “chain”, em todos os outros casos “ and “”, seguido de o first property, seguido de “ has domain ”, seguido de o first property domain class, seguido de “”” se o kind é “domain”, em todos os outros casos “ and “”, seguido de o first property, seguido de “ has range ”, seguido de o first property range class, seguido de “””, seguido de um ponto, se o indicador “chosen” está ativado;<br>2. em todos os outros casos, uma cadeia vazia. ⚠︎ mecânica <!-- rulespeak:reword --> |
| **DR-107 Name** | Um firing: name é igual a firing ID. |
| **DR-108 Rule Enabled** | Um disjoint check: rule enabled é verdadeiro quando o rule vinculado é enabled. |
| **DR-109 First Holds** | Um disjoint check: first holds é verdadeiro quando o first vinculado é holds. |
| **DR-110 Second Holds** | Um disjoint check: second holds é verdadeiro quando o second vinculado é holds. |
| **DR-111 Subject Label** | Um disjoint check: subject label — obtido do first vinculado. |
| **DR-112 Object Label** | Um disjoint check: object label — obtido do first vinculado. |
| **DR-113 Violation Unit** | O campo “violation unit” do disjoint check é determinado pela seguinte prioridade:<br>1. 1, se tudo o que segue é verdadeiro: o indicador “rule enabled” está ativado; o indicador “first holds” está ativado; e o indicador “second holds” está ativado;<br>2. em todos os outros casos, 0. |
| **DR-114 Alarm Text** | O campo “alarm text” do disjoint check é determinado pela seguinte prioridade:<br>1. “Contradiction found. ”, seguido de o subject label, seguido de “ is recorded as both parent and spouse of ”, seguido de o object label, seguido de “. Check those facts and the selected disjoint rule.”, se o violation unit é 1;<br>2. em todos os outros casos, uma cadeia vazia. |
| **DR-115 Name** | Um disjoint check: name é igual a disjoint check ID. |
| **DR-116 Name** | Um scenario: name é igual a label. |
| **DR-117 Graph Has Facts to Add** | Um scenario: graph has facts to add é verdadeiro quando o graph vinculado tem um facts to add. |
| **DR-118 Is Available** | Um scenario é considerado available se pelo menos uma das seguintes é verdadeira: o fact baseline não é “next” ou o indicador “graph has facts to add” está ativado. |
| **DR-119 Name** | Um scenario fact change: name é igual a scenario fact change ID. |
| **DR-120 Name** | Um scenario rule setting: name é igual a scenario rule setting ID. |
| **DR-121 Name** | Um node layout: name é igual a node layout ID. |
| **DR-122 Name** | Um layout slot: name é igual a layout slot ID. |
| **DR-123 Name** | Um page text: name é igual a page text ID. |

## 5 Rastreabilidade ao esquema

_A coluna de expressão é a definição da regra em notação Documentação —
a mesma lógica que o livro de regras armazena, escrita para um leitor de negócio._

| Elemento do esquema | Tipo | Expressão |
|----------------|------|------------|
| **Graphs.Name** | fórmula | `Label` |
| **Graphs.CountOfStatedFacts** | agregado | `Sum(Statements.InEffectUnit via Graph)` |
| **Graphs.CountOfConclusions** | agregado | `Sum(Statements.ConclusionUnit via Graph)` |
| **Graphs.CountOfViolations** | agregado | `Sum(DisjointChecks.ViolationUnit via Graph)` |
| **Graphs.IsConsistent** | fórmula | `CountOfViolations = 0` |
| **Graphs.ConclusionsMessage** | fórmula | `If(CountOfConclusions = 0, NoConclusionsText, "")` |
| **Graphs.CountOfFactsToAdd** | agregado | `Sum(Statements.ListedOffUnit via Graph)` |
| **Graphs.HasFactsToAdd** | fórmula | `CountOfFactsToAdd > 0` |
| **Graphs.NextFactSortOrder** | agregado | `Min(Statements.SortOrder via Graph)` |
| **Graphs.MaxActivationSeq** | agregado | `Max(Statements.ActivationSeq via Graph)` |
| **Graphs.NextActivationSeq** | fórmula | `MaxActivationSeq + 1` |
| **Graphs.MaxListedSortOrder** | agregado | `Max(Statements.SortOrder via Graph)` |
| **Graphs.NextListedSortOrder** | fórmula | `MaxListedSortOrder + 1` |
| **Rules.Name** | fórmula | `Label` |
| **Rules.CountOfFirings** | agregado | `Sum(Firings.FiredUnit via Rule)` |
| **FiringKinds.Name** | fórmula | `Wording` |
| **FiringKinds.RuleEnabled** | consulta | `Lookup(Rules.IsEnabled via Rule)` |
| **Things.Name** | fórmula | `Label` |
| **Things.NodeKey** | fórmula | `ThingId` |
| **Things.CountOfChildren** | agregado | `Sum(Statements.ParentUnit via Subject)` |
| **Things.CountOfGrandchildren** | agregado | `Sum(Statements.GrandparentUnit via Subject)` |
| **Things.CountAsPlace** | agregado | `Sum(Statements.PlaceUnit via ObjectThing)` |
| **Things.CountAsSubject** | agregado | `Sum(Statements.HoldsUnit via Subject)` |
| **Things.CountAsObject** | agregado | `Sum(Statements.HoldsUnit via ObjectThing)` |
| **Things.IsInGraph** | fórmula | `Or(CountAsSubject > 0, CountAsObject > 0)` |
| **Things.NodeKind** | fórmula | `If(Kind = "class", "class", If(CountAsPlace > 0, "place", "person"))` |
| **Properties.Name** | fórmula | `Label` |
| **Statements.SubjectLabel** | consulta | `Lookup(Things.Label via Subject)` |
| **Statements.PropertyLabel** | consulta | `Lookup(Properties.Label via Property)` |
| **Statements.ObjectLabel** | consulta | `Lookup(Things.Label via ObjectThing)` |
| **Statements.PropertyDatatype** | consulta | `Lookup(Properties.Datatype via Property)` |
| **Statements.PropertyHasOneCurrentValue** | consulta | `Lookup(Properties.HasOneCurrentValue via Property)` |
| **Statements.PropertyDomainClass** | consulta | `Lookup(Properties.DomainClass via Property)` |
| **Statements.PropertyRangeClass** | consulta | `Lookup(Properties.RangeClass via Property)` |
| **Statements.LiteralQuote** | consulta | `Lookup(Graphs.LiteralQuote via Graph)` |
| **Statements.LiteralTypeMarker** | consulta | `Lookup(Graphs.LiteralTypeMarker via Graph)` |
| **Statements.BuiltInSourceText** | consulta | `Lookup(Graphs.BuiltInSourceText via Graph)` |
| **Statements.UserSourceText** | consulta | `Lookup(Graphs.UserSourceText via Graph)` |
| **Statements.NextFactSortOrder** | consulta | `Lookup(Graphs.NextFactSortOrder via Graph)` |
| **Statements.IsLiteral** | fórmula | `ObjectThing = ""` |
| **Statements.ObjectText** | fórmula | `If(ObjectThing = "", ObjectValue, ObjectLabel)` |
| **Statements.Name** | fórmula | `Concat(SubjectLabel, " ", PropertyLabel, " ", ObjectText)` |
| **Statements.Words** | fórmula | `Concat(SubjectLabel, " ", PropertyLabel, " ", If(ObjectThing = "", Concat(LiteralQuote, ObjectValue, LiteralQuote, LiteralTypeMarker, PropertyDatatype), ObjectLabel))` |
| **Statements.Source** | fórmula | `If(IsBuiltIn, BuiltInSourceText, UserSourceText)` |
| **Statements.SubjectPropertyKey** | fórmula | `Concat(Subject, "\|", Property)` |
| **Statements.LatestSiblingActivationSeq** | agregado | `Max(Statements.ActivationSeq via SubjectPropertyKey)` |
| **Statements.IsInEffect** | fórmula | `And(IsStated, Or(Not(PropertyHasOneCurrentValue), ActivationSeq = LatestSiblingActivationSeq))` |
| **Statements.ListedUnit** | fórmula | `If(IsListed, 1, 0)` |
| **Statements.InEffectUnit** | fórmula | `If(IsInEffect, 1, 0)` |
| **Statements.ListedOffUnit** | fórmula | `If(And(IsListed, Not(IsInEffect)), 1, 0)` |
| **Statements.IsNextToAdd** | fórmula | `And(ListedOffUnit = 1, SortOrder = NextFactSortOrder)` |
| **Statements.CountOfInverseFirings** | agregado | `Sum(Firings.InverseFiredUnit via Target)` |
| **Statements.CountOfSymmetricFirings** | agregado | `Sum(Firings.SymmetricFiredUnit via Target)` |
| **Statements.HoldsBeforeChain** | fórmula | `Or(IsInEffect, CountOfInverseFirings > 0, CountOfSymmetricFirings > 0)` |
| **Statements.CountOfChainFirings** | agregado | `Sum(Firings.ChainFiredUnit via Target)` |
| **Statements.HoldsBeforeTyping** | fórmula | `Or(HoldsBeforeChain, CountOfChainFirings > 0)` |
| **Statements.CountOfTypeFirings** | agregado | `Sum(Firings.TypeFiredUnit via Target)` |
| **Statements.Holds** | fórmula | `Or(HoldsBeforeTyping, CountOfTypeFirings > 0)` |
| **Statements.IsConclusion** | fórmula | `And(Holds, Not(IsInEffect))` |
| **Statements.HoldsUnit** | fórmula | `If(Holds, 1, 0)` |
| **Statements.ConclusionUnit** | fórmula | `If(IsConclusion, 1, 0)` |
| **Statements.ParentUnit** | fórmula | `If(And(Holds, Property = "isParentOf"), 1, 0)` |
| **Statements.GrandparentUnit** | fórmula | `If(And(Holds, Property = "isGrandparentOf"), 1, 0)` |
| **Statements.PlaceUnit** | fórmula | `If(And(Holds, Property = "attends"), 1, 0)` |
| **Statements.FirstFiredOrderKey** | agregado | `Min(Firings.OrderKey via Target)` |
| **Statements.CountOfChosenFirings** | agregado | `Sum(Firings.ChosenUnit via Target)` |
| **Statements.SubjectNodeKey** | fórmula | `Subject` |
| **Statements.ObjectNodeKey** | fórmula | `If(ObjectThing = "", Concat("literal\|", ObjectValue, "\|", PropertyDatatype), ObjectThing)` |
| **Statements.SubjectNodeKind** | consulta | `Lookup(Things.NodeKind via Subject)` |
| **Statements.ObjectThingNodeKind** | consulta | `Lookup(Things.NodeKind via ObjectThing)` |
| **Statements.ObjectNodeKind** | fórmula | `If(ObjectThing = "", "value", ObjectThingNodeKind)` |
| **Statements.ObjectValueNodeLabel** | consulta | `Lookup(Properties.ValueNodeLabel via Property)` |
| **Statements.CountOfViolationsAsFirst** | agregado | `Sum(DisjointChecks.ViolationUnit via First)` |
| **Statements.CountOfViolationsAsSecond** | agregado | `Sum(DisjointChecks.ViolationUnit via Second)` |
| **Statements.IsInContradiction** | fórmula | `Or(CountOfViolationsAsFirst > 0, CountOfViolationsAsSecond > 0)` |
| **Firings.RuleEnabled** | consulta | `Lookup(Rules.IsEnabled via Rule)` |
| **Firings.KindWording** | consulta | `Lookup(FiringKinds.Wording via Kind)` |
| **Firings.KindSortOrder** | consulta | `Lookup(FiringKinds.SortOrder via Kind)` |
| **Firings.TargetName** | consulta | `Lookup(Statements.Name via Target)` |
| **Firings.TargetSubjectLabel** | consulta | `Lookup(Statements.SubjectLabel via Target)` |
| **Firings.TargetPropertyLabel** | consulta | `Lookup(Statements.PropertyLabel via Target)` |
| **Firings.TargetObjectText** | consulta | `Lookup(Statements.ObjectText via Target)` |
| **Firings.TargetIsConclusion** | consulta | `Lookup(Statements.IsConclusion via Target)` |
| **Firings.TargetSubjectNodeKey** | consulta | `Lookup(Statements.SubjectNodeKey via Target)` |
| **Firings.TargetObjectNodeKey** | consulta | `Lookup(Statements.ObjectNodeKey via Target)` |
| **Firings.TargetFirstFiredOrderKey** | consulta | `Lookup(Statements.FirstFiredOrderKey via Target)` |
| **Firings.FirstWords** | consulta | `Lookup(Statements.Words via First)` |
| **Firings.SecondWords** | consulta | `Lookup(Statements.Words via Second)` |
| **Firings.FirstIsInEffect** | consulta | `Lookup(Statements.IsInEffect via First)` |
| **Firings.FirstHoldsBeforeChain** | consulta | `Lookup(Statements.HoldsBeforeChain via First)` |
| **Firings.SecondHoldsBeforeChain** | consulta | `Lookup(Statements.HoldsBeforeChain via Second)` |
| **Firings.FirstHoldsBeforeTyping** | consulta | `Lookup(Statements.HoldsBeforeTyping via First)` |
| **Firings.FirstProperty** | consulta | `Lookup(Statements.Property via First)` |
| **Firings.FirstPropertyDomainClass** | consulta | `Lookup(Statements.PropertyDomainClass via First)` |
| **Firings.FirstPropertyRangeClass** | consulta | `Lookup(Statements.PropertyRangeClass via First)` |
| **Firings.FirstSortOrder** | consulta | `Lookup(Statements.SortOrder via First)` |
| **Firings.SecondSortOrder** | consulta | `Lookup(Statements.SortOrder via Second)` |
| **Firings.InverseFiredUnit** | fórmula | `If(And(RuleEnabled, Kind = "inverse", FirstIsInEffect), 1, 0)` |
| **Firings.SymmetricFiredUnit** | fórmula | `If(And(RuleEnabled, Kind = "symmetric", FirstIsInEffect), 1, 0)` |
| **Firings.ChainFiredUnit** | fórmula | `If(And(RuleEnabled, Kind = "chain", FirstHoldsBeforeChain, SecondHoldsBeforeChain), 1, 0)` |
| **Firings.TypeFiredUnit** | fórmula | `If(And(RuleEnabled, Or(Kind = "domain", Kind = "range"), FirstHoldsBeforeTyping), 1, 0)` |
| **Firings.FiredUnit** | fórmula | `InverseFiredUnit + SymmetricFiredUnit + ChainFiredUnit + TypeFiredUnit` |
| **Firings.OrderKey** | fórmula | `FirstSortOrder * 100000 + KindSortOrder * 1000 + If(Second = "", 0, SecondSortOrder)` |
| **Firings.IsChosen** | fórmula | `And(FiredUnit = 1, TargetIsConclusion, OrderKey = TargetFirstFiredOrderKey)` |
| **Firings.ChosenUnit** | fórmula | `If(IsChosen, 1, 0)` |
| **Firings.CardText** | fórmula | `If(IsChosen, Concat("Worked out by the ", KindWording, " from “", FirstWords, "”", If(Kind = "chain", Concat(" and “", SecondWords, "”"), If(Kind = "domain", Concat(" and “", FirstProperty, " has domain ", FirstPropertyDomainClass, "”"), If(Kind = "range", Concat(" and “", FirstProperty, " has range ", FirstPropertyRangeClass, "”"), ""))), "."), "")` |
| **Firings.Name** | fórmula | `FiringId` |
| **DisjointChecks.RuleEnabled** | consulta | `Lookup(Rules.IsEnabled via Rule)` |
| **DisjointChecks.FirstHolds** | consulta | `Lookup(Statements.Holds via First)` |
| **DisjointChecks.SecondHolds** | consulta | `Lookup(Statements.Holds via Second)` |
| **DisjointChecks.SubjectLabel** | consulta | `Lookup(Statements.SubjectLabel via First)` |
| **DisjointChecks.ObjectLabel** | consulta | `Lookup(Statements.ObjectLabel via First)` |
| **DisjointChecks.ViolationUnit** | fórmula | `If(And(RuleEnabled, FirstHolds, SecondHolds), 1, 0)` |
| **DisjointChecks.AlarmText** | fórmula | `If(ViolationUnit = 1, Concat("Contradiction found. ", SubjectLabel, " is recorded as both parent and spouse of ", ObjectLabel, ". Check those facts and the selected disjoint rule."), "")` |
| **DisjointChecks.Name** | fórmula | `DisjointCheckId` |
| **Scenarios.Name** | fórmula | `Label` |
| **Scenarios.GraphHasFactsToAdd** | consulta | `Lookup(Graphs.HasFactsToAdd via Graph)` |
| **Scenarios.IsAvailable** | fórmula | `Or(FactBaseline <> "next", GraphHasFactsToAdd)` |
| **ScenarioFactChanges.Name** | fórmula | `ScenarioFactChangeId` |
| **ScenarioRuleSettings.Name** | fórmula | `ScenarioRuleSettingId` |
| **NodeLayouts.Name** | fórmula | `NodeLayoutId` |
| **LayoutSlots.Name** | fórmula | `LayoutSlotId` |
| **PageTexts.Name** | fórmula | `PageTextId` |

---

> **RuleSpeak®** — a notação declarativa de regras de negócio criada por **Ronald G. Ross**, seguindo as convenções do **SBVR** — chegará em breve. Até lá, esta página mostra a documentação comum. [www.RonRoss.info](https://www.RonRoss.info)
