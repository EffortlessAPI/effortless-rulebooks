# 📘 South Park ontology explainer — Documentazione

_The barebones South Park ontology from Jessica Talisman's explainer: ten stated facts and five rules that can each be switched on. Conclusions are formulas._

> Regole di business dichiarative generate dal libro delle regole. Ogni enunciato
> esprime una verità del dominio di business — non è una procedura né un imperativo.
> Le formule del libro delle regole sono l'unica fonte di verità; questo documento
> ne è la lettura in linguaggio chiaro.

## 1 Vocabolario di business

| Termine | Descrizione | Commento narrativo |
|------|-------------|-------------------|
| **Graph** | The whole example. | — |
| Name | Uguale a graph ID. | — |
| Count of Stated Facts | Il totale di stated unit tra i statements collegati a il graph. | — |
| Count of Conclusions | Il totale di conclusion unit tra i statements collegati a il graph. | _Facts that hold but were never stated._ |
| Count of Violations | Il totale di violation unit tra i disjoint checks collegati a il graph. | — |
| Is Consistent | Vero quando il count of violations è 0. | — |

<details open>
<summary>Dati di esempio</summary>

| Name ƒ | Count of Stated Facts ƒ | Count of Conclusions ƒ | Count of Violations ƒ | Is Consistent ƒ |
|---|---|---|---|---|
| south-park | 10 | 8 | 0 | true |

_ƒ indica una colonna calcolata._

</details>

| Termine | Descrizione | Commento narrativo |
|------|-------------|-------------------|
| **Rule** | The five rules; IsEnabled is the checkbox. | — |
| Label | Un attributo definito. | — |
| Explanation | Un attributo definito. | — |
| Is Enabled | Vero quando una stringa vuota. | — |
| Name | Uguale a label. | — |
| Count of Firings | Il totale di fired unit tra i firings collegati a il rule. | _How many times the rule produced a fact._ |

<details open>
<summary>Dati di esempio</summary>

| Label | Explanation | Is Enabled | Name ƒ | Count of Firings ƒ |
|---|---|---|---|---|
| Inverse | A parent link also gives a child link between the same two people. | true | Inverse | 3 |
| Symmetric | A friend link or a spouse link is true for both people. | true | Symmetric | 2 |
| Property chain | A parent of a parent is a grandparent. | true | Property chain | 2 |

_ƒ indica una colonna calcolata._

</details>

| Termine | Descrizione | Commento narrativo |
|------|-------------|-------------------|
| **Thing** | A node in the picture: a person, a place, a class or a role value. | — |
| Label | Un attributo definito. | — |
| Kind | Un attributo definito. | — |
| Name | Uguale a label. | — |
| Count of Children | Il totale di parent unit tra i statements collegati a il thing. | _Added: a count, which a reasoner alone does not give._ |
| Count of Grandchildren | Il totale di grandparent unit tra i statements collegati a il thing. | _Added: counts concluded grandchildren._ |

<details open>
<summary>Dati di esempio</summary>

| Label | Kind | Name ƒ | Count of Children ƒ | Count of Grandchildren ƒ |
|---|---|---|---|---|
| Marvin | individual | Marvin | 1 | 2 |
| Randy | individual | Randy | 2 | 0 |
| Sharon | individual | Sharon | 0 | 0 |

_ƒ indica una colonna calcolata._

</details>

| Termine | Descrizione | Commento narrativo |
|------|-------------|-------------------|
| **Property** | A kind of link, with the characteristics the rules use. | — |
| Label | Un attributo definito. | — |
| Inverse of | Un attributo definito. | — |
| Is Symmetric | Vero quando una stringa vuota. | — |
| Chain First | Un attributo definito. | — |
| Chain Second | Un attributo definito. | — |
| Domain Class | Un attributo definito. | — |
| Range Class | Un attributo definito. | — |
| Disjoint With | Un attributo definito. | — |
| Name | Uguale a label. | — |

<details open>
<summary>Dati di esempio</summary>

| Label | Is Symmetric | Name ƒ |
|---|---|---|
| is parent of | false | is parent of |
| is child of | false | is child of |
| is spouse of | true | is spouse of |

_ƒ indica una colonna calcolata._

</details>

| Termine | Descrizione | Commento narrativo |
|------|-------------|-------------------|
| **Statement** | A fact that is stated, or could be concluded: Subject, Property, Object. | — |
| Graph | Un attributo definito. | — |
| Subject | Un attributo definito. | — |
| Property | Un attributo definito. | — |
| Object Thing | Un attributo definito. | — |
| Number Value | Un attributo definito. | — |
| Is Stated | Vero quando una stringa vuota. | _Drawn as a solid line in the explainer._ |
| Subject Label | Ripreso dal subject collegato. | — |
| Property Label | Ripreso dal property collegato. | — |
| Object Label | Ripreso dal object thing collegato. | — |
| Name | Calcolato come il subject label, seguito da uno spazio, seguito da il property label, seguito da uno spazio, seguito da il number value se l'object thing è vuoto, in tutti gli altri casi l'object label. | — |
| Count of Inverse Firings | Il totale di inverse fired unit tra i firings collegati a il statement. | — |
| Count of Symmetric Firings | Il totale di symmetric fired unit tra i firings collegati a il statement. | — |
| Holds Before Chain | Vero quando vale almeno una delle seguenti: l'indicatore “stated” è attivo; il count of inverse firings è maggiore di 0; o il count of symmetric firings è maggiore di 0. | — |
| Count of Chain Firings | Il totale di chain fired unit tra i firings collegati a il statement. | — |
| Holds Before Typing | Vero quando vale almeno una delle seguenti: l'indicatore “holds before chain” è attivo o il count of chain firings è maggiore di 0. | — |
| Count of Type Firings | Il totale di type fired unit tra i firings collegati a il statement. | — |
| Holds | Vero quando vale almeno una delle seguenti: l'indicatore “holds before typing” è attivo o il count of type firings è maggiore di 0. | _True when stated or concluded by an enabled rule._ |
| Is Conclusion | Vero quando vale tutto quanto segue: l'indicatore “holds” è attivo e l'indicatore “stated” non è attivo. | _Drawn as a dashed line in the explainer._ |
| Worked Out by | Determinato per priorità: una stringa vuota se l'indicatore “conclusion” non è attivo; «inverse» se il count of inverse firings è maggiore di 0; «symmetric» se il count of symmetric firings è maggiore di 0; «property chain» se il count of chain firings è maggiore di 0; in tutti gli altri casi, «domain and range». | — |
| Stated Unit | Determinato per priorità: 1 se l'indicatore “stated” è attivo; in tutti gli altri casi, 0. | — |
| Conclusion Unit | Determinato per priorità: 1 se l'indicatore “conclusion” è attivo; in tutti gli altri casi, 0. | — |
| Parent Unit | Determinato per priorità: 1 se vale tutto quanto segue: l'indicatore “holds” è attivo e il property è «isParentOf»; in tutti gli altri casi, 0. | — |
| Grandparent Unit | Determinato per priorità: 1 se vale tutto quanto segue: l'indicatore “holds” è attivo e il property è «isGrandparentOf»; in tutti gli altri casi, 0. | — |

<details open>
<summary>Dati di esempio</summary>

| Number Value | Is Stated | Subject Label ƒ | Property Label ƒ | Object Label ƒ | Name ƒ | Count of Inverse Firings ƒ | Count of Symmetric Firings ƒ | Holds Before Chain ƒ | Count of Chain Firings ƒ | Holds Before Typing ƒ | Count of Type Firings ƒ | Holds ƒ | Is Conclusion ƒ | Worked Out by ƒ | Stated Unit ƒ | Conclusion Unit ƒ | Parent Unit ƒ | Grandparent Unit ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| — | true | Marvin | is parent of | Randy | Marvin is parent of Randy | 0 | 0 | true | 0 | true | 0 | true | false | — | 1 | 0 | 1 | 0 |
| — | false | Marvin | is parent of | Sharon | Marvin is parent of Sharon | 0 | 0 | false | 0 | false | 0 | false | false | — | 0 | 0 | 0 | 0 |
| — | false | Marvin | is parent of | Shelly | Marvin is parent of Shelly | 0 | 0 | false | 0 | false | 0 | false | false | — | 0 | 0 | 0 | 0 |

_ƒ indica una colonna calcolata._

</details>

| Termine | Descrizione | Commento narrativo |
|------|-------------|-------------------|
| **Firing** | One way one rule could produce one fact from one or two others. | — |
| Rule | Un attributo definito. | — |
| Kind | Un attributo definito. | — |
| Target | Un attributo definito. | — |
| First | Un attributo definito. | — |
| Second | Un attributo definito. | _Same as First when the rule needs one premise._ |
| Rule Enabled | Vero quando il rule collegato è enabled. | — |
| Rule Label | Ripreso dal rule collegato. | — |
| Target Name | Ripreso dal target collegato. | — |
| First Name | Ripreso dal first collegato. | — |
| Second Name | Ripreso dal second collegato. | — |
| First is Stated | Vero quando il first collegato è stated. | — |
| First Holds Before Chain | Vero quando il first collegato è holds before chain. | — |
| Second Holds Before Chain | Vero quando il second collegato è holds before chain. | — |
| First Holds Before Typing | Vero quando il first collegato è holds before typing. | — |
| Inverse Fired Unit | Determinato per priorità: 1 se vale tutto quanto segue: l'indicatore “rule enabled” è attivo; il kind è «inverse»; e l'indicatore “first is stated” è attivo; in tutti gli altri casi, 0. | — |
| Symmetric Fired Unit | Determinato per priorità: 1 se vale tutto quanto segue: l'indicatore “rule enabled” è attivo; il kind è «symmetric»; e l'indicatore “first is stated” è attivo; in tutti gli altri casi, 0. | — |
| Chain Fired Unit | Determinato per priorità: 1 se vale tutto quanto segue: l'indicatore “rule enabled” è attivo; il kind è «chain»; l'indicatore “first holds before chain” è attivo; e l'indicatore “second holds before chain” è attivo; in tutti gli altri casi, 0. | — |
| Type Fired Unit | Determinato per priorità: 1 se vale tutto quanto segue: l'indicatore “rule enabled” è attivo; vale almeno una delle seguenti: il kind è «domain» o il kind è «range»; e l'indicatore “first holds before typing” è attivo; in tutti gli altri casi, 0. | — |
| Fired Unit | Calcolato come l'inverse fired unit più il symmetric fired unit più il chain fired unit più il type fired unit. | — |
| Name | Determinato per priorità: il target name, seguito da «: worked out by the », seguito da il rule label, seguito da « rule from '», seguito da il first name, seguito da «'», seguito da « and '», seguito da il second name, seguito da «'» se il fired unit è maggiore di 0; in tutti gli altri casi, il firing ID. | _The explainer's conclusion card._ |

<details open>
<summary>Dati di esempio</summary>

| Kind | Rule Enabled ƒ | Rule Label ƒ | Target Name ƒ | First Name ƒ | Second Name ƒ | First is Stated ƒ | First Holds Before Chain ƒ | Second Holds Before Chain ƒ | First Holds Before Typing ƒ | Inverse Fired Unit ƒ | Symmetric Fired Unit ƒ | Chain Fired Unit ƒ | Type Fired Unit ƒ | Fired Unit ƒ | Name ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| inverse | true | Inverse | Randy is child of Marvin | Marvin is parent of Randy | Marvin is parent of Randy | true | true | true | true | 1 | 0 | 0 | 0 | 1 | Randy is child of Marvin: worked out by the Inverse rule from 'Marvin is parent of Randy' |
| inverse | true | Inverse | Sharon is child of Marvin | Marvin is parent of Sharon | Marvin is parent of Sharon | false | false | false | false | 0 | 0 | 0 | 0 | 0 | inverse:Marvin\|isParentOf\|Sharon>Sharon\|isChildOf\|Marvin |
| inverse | true | Inverse | Shelly is child of Marvin | Marvin is parent of Shelly | Marvin is parent of Shelly | false | false | false | false | 0 | 0 | 0 | 0 | 0 | inverse:Marvin\|isParentOf\|Shelly>Shelly\|isChildOf\|Marvin |

_ƒ indica una colonna calcolata._

</details>

| Termine | Descrizione | Commento narrativo |
|------|-------------|-------------------|
| **Disjoint Check** | Two statements that must not both hold. | — |
| Graph | Un attributo definito. | — |
| Rule | Un attributo definito. | — |
| A | Un attributo definito. | — |
| B | Un attributo definito. | — |
| Name | Uguale a disjoint check ID. | — |
| Rule Enabled | Vero quando il rule collegato è enabled. | — |
| A Holds | Vero quando il a collegato è holds. | — |
| B Holds | Vero quando il b collegato è holds. | — |
| Violation Unit | Determinato per priorità: 1 se vale tutto quanto segue: l'indicatore “rule enabled” è attivo; l'indicatore “a holds” è attivo; e l'indicatore “b holds” è attivo; in tutti gli altri casi, 0. | — |

<details open>
<summary>Dati di esempio</summary>

| Name ƒ | Rule Enabled ƒ | A Holds ƒ | B Holds ƒ | Violation Unit ƒ |
|---|---|---|---|---|
| Marvin\|isParentOf\|Randy~isSpouseOf | true | true | false | 0 |
| Marvin\|isParentOf\|Sharon~isSpouseOf | true | false | false | 0 |
| Marvin\|isParentOf\|Shelly~isSpouseOf | true | false | false | 0 |

_ƒ indica una colonna calcolata._

</details>

## 2 Tipi di fatto

- un **property** può referenziare un **property**
- un **property** può referenziare un **thing**
- un **statement** referenzia esattamente un **graph**
- un **statement** referenzia esattamente un **thing**
- un **statement** referenzia esattamente un **property**
- un **firing** referenzia esattamente un **rule**
- un **firing** referenzia esattamente un **statement**
- un **disjoint check** referenzia esattamente un **graph**
- un **disjoint check** referenzia esattamente un **rule**
- un **disjoint check** referenzia esattamente un **statement**

## 3 Regole operative

_Le regole operative enunciano ciò che il business **obbliga**, **vieta** o
consiglia (**dovrebbe**). Le regole strutturali derivano da campi obbligatori e chiavi
esterne; quelle semantiche dalla tabella Constraints, ognuna basata su un booleano che
il libro delle regole già calcola (referenziato come DR-N nelle regole definitorie)._

### Vincoli strutturali (dallo schema)

- Un rule **deve** avere un label e un explanation, e registrare se è enabled.
- Un thing **deve** avere un label e un kind.
- Un property **deve** avere un label, e registrare se è symmetric.
- Un statement **deve** referenziare esattamente un graph.
- Un statement **deve** referenziare esattamente un thing come suo subject.
- Un statement **deve** referenziare esattamente un property.
- Un statement **deve** registrare se è stated.
- Un firing **deve** referenziare esattamente un rule.
- Un firing **deve** referenziare esattamente un statement come suo target.
- Un firing **deve** referenziare esattamente un statement come suo first.
- Un firing **deve** referenziare esattamente un statement come suo second.
- Un firing **deve** avere un kind.
- Un disjoint check **deve** referenziare esattamente un graph.
- Un disjoint check **deve** referenziare esattamente un rule.
- Un disjoint check **deve** referenziare esattamente un statement come suo a.
- Un disjoint check **deve** referenziare esattamente un statement come suo b.

## 4 Regole definitorie

_Tutti gli enunciati esprimono verità del dominio di business; non sono procedure
né imperativi. Si evita «se e solo se» a favore di «solo se», affinché una necessità
unidirezionale non venga scambiata per un'equivalenza. Un contrassegno
**⚠︎ meccanica** segnala una regola la cui formulazione deterministica è fedele ma
legnosa — un segnale per una riscrittura opzionale, non un difetto._

| ID | Regola dichiarativa |
|----|------------------|
| **DR-1 Name** | Un graph: name è uguale a graph ID. |
| **DR-2 Count of Stated Facts** | Un graph: count of stated facts è il totale di stated unit tra i statements collegati a il graph. |
| **DR-3 Count of Conclusions** | Un graph: count of conclusions è il totale di conclusion unit tra i statements collegati a il graph. |
| **DR-4 Count of Violations** | Un graph: count of violations è il totale di violation unit tra i disjoint checks collegati a il graph. |
| **DR-5 Is Consistent** | Un graph è considerato consistent se il count of violations è 0. |
| **DR-6 Name** | Un rule: name è uguale a label. |
| **DR-7 Count of Firings** | Un rule: count of firings è il totale di fired unit tra i firings collegati a il rule. |
| **DR-8 Name** | Un thing: name è uguale a label. |
| **DR-9 Count of Children** | Un thing: count of children è il totale di parent unit tra i statements collegati a il thing. |
| **DR-10 Count of Grandchildren** | Un thing: count of grandchildren è il totale di grandparent unit tra i statements collegati a il thing. |
| **DR-11 Name** | Un property: name è uguale a label. |
| **DR-12 Subject Label** | Un statement: subject label — ripreso dal subject collegato. |
| **DR-13 Property Label** | Un statement: property label — ripreso dal property collegato. |
| **DR-14 Object Label** | Un statement: object label — ripreso dal object thing collegato. |
| **DR-15 Name** | Un statement: name è calcolato come il subject label, seguito da uno spazio, seguito da il property label, seguito da uno spazio, seguito da il number value se l'object thing è vuoto, in tutti gli altri casi l'object label. |
| **DR-16 Count of Inverse Firings** | Un statement: count of inverse firings è il totale di inverse fired unit tra i firings collegati a il statement. |
| **DR-17 Count of Symmetric Firings** | Un statement: count of symmetric firings è il totale di symmetric fired unit tra i firings collegati a il statement. |
| **DR-18 Holds Before Chain** | Un statement è considerato come soddisfacente: before chain se vale almeno una delle seguenti: l'indicatore “stated” è attivo; il count of inverse firings è maggiore di 0; o il count of symmetric firings è maggiore di 0. |
| **DR-19 Count of Chain Firings** | Un statement: count of chain firings è il totale di chain fired unit tra i firings collegati a il statement. |
| **DR-20 Holds Before Typing** | Un statement è considerato come soddisfacente: before typing se vale almeno una delle seguenti: l'indicatore “holds before chain” è attivo o il count of chain firings è maggiore di 0. |
| **DR-21 Count of Type Firings** | Un statement: count of type firings è il totale di type fired unit tra i firings collegati a il statement. |
| **DR-22 Holds** | Un statement è considerato come soddisfacente: se vale almeno una delle seguenti: l'indicatore “holds before typing” è attivo o il count of type firings è maggiore di 0. |
| **DR-23 Is Conclusion** | Un statement è considerato un conclusion se vale tutto quanto segue: l'indicatore “holds” è attivo e l'indicatore “stated” non è attivo. |
| **DR-24 Worked Out by** | Il campo “worked out by” del statement è determinato secondo la seguente priorità:<br>1. una stringa vuota, se l'indicatore “conclusion” non è attivo;<br>2. «inverse», se il count of inverse firings è maggiore di 0;<br>3. «symmetric», se il count of symmetric firings è maggiore di 0;<br>4. «property chain», se il count of chain firings è maggiore di 0;<br>5. in tutti gli altri casi, «domain and range». |
| **DR-25 Stated Unit** | Il campo “stated unit” del statement è determinato secondo la seguente priorità:<br>1. 1, se l'indicatore “stated” è attivo;<br>2. in tutti gli altri casi, 0. |
| **DR-26 Conclusion Unit** | Il campo “conclusion unit” del statement è determinato secondo la seguente priorità:<br>1. 1, se l'indicatore “conclusion” è attivo;<br>2. in tutti gli altri casi, 0. |
| **DR-27 Parent Unit** | Il campo “parent unit” del statement è determinato secondo la seguente priorità:<br>1. 1, se vale tutto quanto segue: l'indicatore “holds” è attivo e il property è «isParentOf»;<br>2. in tutti gli altri casi, 0. |
| **DR-28 Grandparent Unit** | Il campo “grandparent unit” del statement è determinato secondo la seguente priorità:<br>1. 1, se vale tutto quanto segue: l'indicatore “holds” è attivo e il property è «isGrandparentOf»;<br>2. in tutti gli altri casi, 0. |
| **DR-29 Rule Enabled** | Un firing: rule enabled è vero quando il rule collegato è enabled. |
| **DR-30 Rule Label** | Un firing: rule label — ripreso dal rule collegato. |
| **DR-31 Target Name** | Un firing: target name — ripreso dal target collegato. |
| **DR-32 First Name** | Un firing: first name — ripreso dal first collegato. |
| **DR-33 Second Name** | Un firing: second name — ripreso dal second collegato. |
| **DR-34 First is Stated** | Un firing: first is stated è vero quando il first collegato è stated. |
| **DR-35 First Holds Before Chain** | Un firing: first holds before chain è vero quando il first collegato è holds before chain. |
| **DR-36 Second Holds Before Chain** | Un firing: second holds before chain è vero quando il second collegato è holds before chain. |
| **DR-37 First Holds Before Typing** | Un firing: first holds before typing è vero quando il first collegato è holds before typing. |
| **DR-38 Inverse Fired Unit** | Il campo “inverse fired unit” del firing è determinato secondo la seguente priorità:<br>1. 1, se vale tutto quanto segue: l'indicatore “rule enabled” è attivo; il kind è «inverse»; e l'indicatore “first is stated” è attivo;<br>2. in tutti gli altri casi, 0. |
| **DR-39 Symmetric Fired Unit** | Il campo “symmetric fired unit” del firing è determinato secondo la seguente priorità:<br>1. 1, se vale tutto quanto segue: l'indicatore “rule enabled” è attivo; il kind è «symmetric»; e l'indicatore “first is stated” è attivo;<br>2. in tutti gli altri casi, 0. |
| **DR-40 Chain Fired Unit** | Il campo “chain fired unit” del firing è determinato secondo la seguente priorità:<br>1. 1, se vale tutto quanto segue: l'indicatore “rule enabled” è attivo; il kind è «chain»; l'indicatore “first holds before chain” è attivo; e l'indicatore “second holds before chain” è attivo;<br>2. in tutti gli altri casi, 0. |
| **DR-41 Type Fired Unit** | Il campo “type fired unit” del firing è determinato secondo la seguente priorità:<br>1. 1, se vale tutto quanto segue: l'indicatore “rule enabled” è attivo; vale almeno una delle seguenti: il kind è «domain» o il kind è «range»; e l'indicatore “first holds before typing” è attivo;<br>2. in tutti gli altri casi, 0. |
| **DR-42 Fired Unit** | Un firing: fired unit è calcolato come l'inverse fired unit più il symmetric fired unit più il chain fired unit più il type fired unit. |
| **DR-43 Name** | Il campo “name” del firing è determinato secondo la seguente priorità:<br>1. il target name, seguito da «: worked out by the », seguito da il rule label, seguito da « rule from '», seguito da il first name, seguito da «'», seguito da « and '», seguito da il second name, seguito da «'», se il fired unit è maggiore di 0;<br>2. in tutti gli altri casi, il firing ID. |
| **DR-44 Name** | Un disjoint check: name è uguale a disjoint check ID. |
| **DR-45 Rule Enabled** | Un disjoint check: rule enabled è vero quando il rule collegato è enabled. |
| **DR-46 A Holds** | Un disjoint check: a holds è vero quando il a collegato è holds. |
| **DR-47 B Holds** | Un disjoint check: b holds è vero quando il b collegato è holds. |
| **DR-48 Violation Unit** | Il campo “violation unit” del disjoint check è determinato secondo la seguente priorità:<br>1. 1, se vale tutto quanto segue: l'indicatore “rule enabled” è attivo; l'indicatore “a holds” è attivo; e l'indicatore “b holds” è attivo;<br>2. in tutti gli altri casi, 0. |

## 5 Tracciabilità allo schema

_La colonna espressione è la definizione della regola in notazione Documentazione —
la stessa logica che il libro delle regole memorizza, scritta per un lettore di business._

| Elemento dello schema | Tipo | Espressione |
|----------------|------|------------|
| **Graphs.Name** | formula | `GraphId` |
| **Graphs.CountOfStatedFacts** | aggregato | `Sum(Statements.StatedUnit via Graph)` |
| **Graphs.CountOfConclusions** | aggregato | `Sum(Statements.ConclusionUnit via Graph)` |
| **Graphs.CountOfViolations** | aggregato | `Sum(DisjointChecks.ViolationUnit via Graph)` |
| **Graphs.IsConsistent** | formula | `CountOfViolations = 0` |
| **Rules.Name** | formula | `Label` |
| **Rules.CountOfFirings** | aggregato | `Sum(Firings.FiredUnit via Rule)` |
| **Things.Name** | formula | `Label` |
| **Things.CountOfChildren** | aggregato | `Sum(Statements.ParentUnit via Subject)` |
| **Things.CountOfGrandchildren** | aggregato | `Sum(Statements.GrandparentUnit via Subject)` |
| **Properties.Name** | formula | `Label` |
| **Statements.SubjectLabel** | ricerca | `Lookup(Things.Label via Subject)` |
| **Statements.PropertyLabel** | ricerca | `Lookup(Properties.Label via Property)` |
| **Statements.ObjectLabel** | ricerca | `Lookup(Things.Label via ObjectThing)` |
| **Statements.Name** | formula | `SubjectLabel & " " & PropertyLabel & " " & If(ObjectThing = Blank(), NumberValue, ObjectLabel)` |
| **Statements.CountOfInverseFirings** | aggregato | `Sum(Firings.InverseFiredUnit via Target)` |
| **Statements.CountOfSymmetricFirings** | aggregato | `Sum(Firings.SymmetricFiredUnit via Target)` |
| **Statements.HoldsBeforeChain** | formula | `Or(IsStated, CountOfInverseFirings > 0, CountOfSymmetricFirings > 0)` |
| **Statements.CountOfChainFirings** | aggregato | `Sum(Firings.ChainFiredUnit via Target)` |
| **Statements.HoldsBeforeTyping** | formula | `Or(HoldsBeforeChain, CountOfChainFirings > 0)` |
| **Statements.CountOfTypeFirings** | aggregato | `Sum(Firings.TypeFiredUnit via Target)` |
| **Statements.Holds** | formula | `Or(HoldsBeforeTyping, CountOfTypeFirings > 0)` |
| **Statements.IsConclusion** | formula | `And(Holds, Not(IsStated))` |
| **Statements.WorkedOutBy** | formula | `If(Not(IsConclusion), "", If(CountOfInverseFirings > 0, "inverse", If(CountOfSymmetricFirings > 0, "symmetric", If(CountOfChainFirings > 0, "property chain", "domain and range"))))` |
| **Statements.StatedUnit** | formula | `If(IsStated, 1, 0)` |
| **Statements.ConclusionUnit** | formula | `If(IsConclusion, 1, 0)` |
| **Statements.ParentUnit** | formula | `If(And(Holds, Property = "isParentOf"), 1, 0)` |
| **Statements.GrandparentUnit** | formula | `If(And(Holds, Property = "isGrandparentOf"), 1, 0)` |
| **Firings.RuleEnabled** | ricerca | `Lookup(Rules.IsEnabled via Rule)` |
| **Firings.RuleLabel** | ricerca | `Lookup(Rules.Label via Rule)` |
| **Firings.TargetName** | ricerca | `Lookup(Statements.Name via Target)` |
| **Firings.FirstName** | ricerca | `Lookup(Statements.Name via First)` |
| **Firings.SecondName** | ricerca | `Lookup(Statements.Name via Second)` |
| **Firings.FirstIsStated** | ricerca | `Lookup(Statements.IsStated via First)` |
| **Firings.FirstHoldsBeforeChain** | ricerca | `Lookup(Statements.HoldsBeforeChain via First)` |
| **Firings.SecondHoldsBeforeChain** | ricerca | `Lookup(Statements.HoldsBeforeChain via Second)` |
| **Firings.FirstHoldsBeforeTyping** | ricerca | `Lookup(Statements.HoldsBeforeTyping via First)` |
| **Firings.InverseFiredUnit** | formula | `If(And(RuleEnabled, Kind = "inverse", FirstIsStated), 1, 0)` |
| **Firings.SymmetricFiredUnit** | formula | `If(And(RuleEnabled, Kind = "symmetric", FirstIsStated), 1, 0)` |
| **Firings.ChainFiredUnit** | formula | `If(And(RuleEnabled, Kind = "chain", FirstHoldsBeforeChain, SecondHoldsBeforeChain), 1, 0)` |
| **Firings.TypeFiredUnit** | formula | `If(And(RuleEnabled, Or(Kind = "domain", Kind = "range"), FirstHoldsBeforeTyping), 1, 0)` |
| **Firings.FiredUnit** | formula | `InverseFiredUnit + SymmetricFiredUnit + ChainFiredUnit + TypeFiredUnit` |
| **Firings.Name** | formula | `If(FiredUnit > 0, TargetName & ": worked out by the " & RuleLabel & " rule from '" & FirstName & "'" & If(Kind = "chain", " and '" & SecondName & "'", ""), FiringId)` |
| **DisjointChecks.Name** | formula | `DisjointCheckId` |
| **DisjointChecks.RuleEnabled** | ricerca | `Lookup(Rules.IsEnabled via Rule)` |
| **DisjointChecks.AHolds** | ricerca | `Lookup(Statements.Holds via A)` |
| **DisjointChecks.BHolds** | ricerca | `Lookup(Statements.Holds via B)` |
| **DisjointChecks.ViolationUnit** | formula | `If(And(RuleEnabled, AHolds, BHolds), 1, 0)` |
