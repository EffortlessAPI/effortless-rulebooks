# 📘 South Park ontology explainer — Documentation

_The barebones South Park ontology from Jessica Talisman's explainer: ten stated facts and five rules that can each be switched on. Conclusions are formulas._

> Règles métier déclaratives générées à partir du livre de règles. Chaque énoncé
> exprime une vérité du domaine métier — ce n'est ni une procédure ni un impératif.
> Les formules du livre de règles sont l'unique source de vérité ; ce document en
> est la lecture en langage clair.

## 1 Vocabulaire métier

| Terme | Description | Commentaire narratif |
|------|-------------|-------------------|
| **Graph** | The whole example. | — |
| Name | Identique à graph ID. | — |
| Count of Stated Facts | Le total de stated unit sur les statements liés à le graph. | — |
| Count of Conclusions | Le total de conclusion unit sur les statements liés à le graph. | _Facts that hold but were never stated._ |
| Count of Violations | Le total de violation unit sur les disjoint checks liés à le graph. | — |
| Is Consistent | Vrai lorsque le count of violations est 0. | — |

<details open>
<summary>Données d'exemple</summary>

| Name ƒ | Count of Stated Facts ƒ | Count of Conclusions ƒ | Count of Violations ƒ | Is Consistent ƒ |
|---|---|---|---|---|
| south-park | 10 | 8 | 0 | true |

_ƒ signale une colonne calculée._

</details>

| Terme | Description | Commentaire narratif |
|------|-------------|-------------------|
| **Rule** | The five rules; IsEnabled is the checkbox. | — |
| Label | Un attribut défini. | — |
| Explanation | Un attribut défini. | — |
| Is Enabled | Vrai lorsque une chaîne vide. | — |
| Name | Identique à label. | — |
| Count of Firings | Le total de fired unit sur les firings liés à le rule. | _How many times the rule produced a fact._ |

<details open>
<summary>Données d'exemple</summary>

| Label | Explanation | Is Enabled | Name ƒ | Count of Firings ƒ |
|---|---|---|---|---|
| Inverse | A parent link also gives a child link between the same two people. | true | Inverse | 3 |
| Symmetric | A friend link or a spouse link is true for both people. | true | Symmetric | 2 |
| Property chain | A parent of a parent is a grandparent. | true | Property chain | 2 |

_ƒ signale une colonne calculée._

</details>

| Terme | Description | Commentaire narratif |
|------|-------------|-------------------|
| **Thing** | A node in the picture: a person, a place, a class or a role value. | — |
| Label | Un attribut défini. | — |
| Kind | Un attribut défini. | — |
| Name | Identique à label. | — |
| Count of Children | Le total de parent unit sur les statements liés à le thing. | _Added: a count, which a reasoner alone does not give._ |
| Count of Grandchildren | Le total de grandparent unit sur les statements liés à le thing. | _Added: counts concluded grandchildren._ |

<details open>
<summary>Données d'exemple</summary>

| Label | Kind | Name ƒ | Count of Children ƒ | Count of Grandchildren ƒ |
|---|---|---|---|---|
| Marvin | individual | Marvin | 1 | 2 |
| Randy | individual | Randy | 2 | 0 |
| Sharon | individual | Sharon | 0 | 0 |

_ƒ signale une colonne calculée._

</details>

| Terme | Description | Commentaire narratif |
|------|-------------|-------------------|
| **Property** | A kind of link, with the characteristics the rules use. | — |
| Label | Un attribut défini. | — |
| Inverse of | Un attribut défini. | — |
| Is Symmetric | Vrai lorsque une chaîne vide. | — |
| Chain First | Un attribut défini. | — |
| Chain Second | Un attribut défini. | — |
| Domain Class | Un attribut défini. | — |
| Range Class | Un attribut défini. | — |
| Disjoint With | Un attribut défini. | — |
| Name | Identique à label. | — |

<details open>
<summary>Données d'exemple</summary>

| Label | Is Symmetric | Name ƒ |
|---|---|---|
| is parent of | false | is parent of |
| is child of | false | is child of |
| is spouse of | true | is spouse of |

_ƒ signale une colonne calculée._

</details>

| Terme | Description | Commentaire narratif |
|------|-------------|-------------------|
| **Statement** | A fact that is stated, or could be concluded: Subject, Property, Object. | — |
| Graph | Un attribut défini. | — |
| Subject | Un attribut défini. | — |
| Property | Un attribut défini. | — |
| Object Thing | Un attribut défini. | — |
| Number Value | Un attribut défini. | — |
| Is Stated | Vrai lorsque une chaîne vide. | _Drawn as a solid line in the explainer._ |
| Subject Label | Repris du subject lié. | — |
| Property Label | Repris du property lié. | — |
| Object Label | Repris du object thing lié. | — |
| Name | Calculé comme le subject label, suivi de une espace, suivi de le property label, suivi de une espace, suivi de le number value si l'object thing est vide, dans tous les autres cas l'object label. | — |
| Count of Inverse Firings | Le total de inverse fired unit sur les firings liés à le statement. | — |
| Count of Symmetric Firings | Le total de symmetric fired unit sur les firings liés à le statement. | — |
| Holds Before Chain | Vrai lorsque au moins une des conditions suivantes est vraie : l'indicateur « stated » est activé; le count of inverse firings est supérieur à 0; ou le count of symmetric firings est supérieur à 0. | — |
| Count of Chain Firings | Le total de chain fired unit sur les firings liés à le statement. | — |
| Holds Before Typing | Vrai lorsque au moins une des conditions suivantes est vraie : l'indicateur « holds before chain » est activé ou le count of chain firings est supérieur à 0. | — |
| Count of Type Firings | Le total de type fired unit sur les firings liés à le statement. | — |
| Holds | Vrai lorsque au moins une des conditions suivantes est vraie : l'indicateur « holds before typing » est activé ou le count of type firings est supérieur à 0. | _True when stated or concluded by an enabled rule._ |
| Is Conclusion | Vrai lorsque tout ce qui suit est vrai : l'indicateur « holds » est activé et l'indicateur « stated » n'est pas activé. | _Drawn as a dashed line in the explainer._ |
| Worked Out by | Déterminé par priorité : une chaîne vide si l'indicateur « conclusion » n'est pas activé; « inverse » si le count of inverse firings est supérieur à 0; « symmetric » si le count of symmetric firings est supérieur à 0; « property chain » si le count of chain firings est supérieur à 0 ; dans tous les autres cas, « domain and range ». | — |
| Stated Unit | Déterminé par priorité : 1 si l'indicateur « stated » est activé ; dans tous les autres cas, 0. | — |
| Conclusion Unit | Déterminé par priorité : 1 si l'indicateur « conclusion » est activé ; dans tous les autres cas, 0. | — |
| Parent Unit | Déterminé par priorité : 1 si tout ce qui suit est vrai : l'indicateur « holds » est activé et le property est « isParentOf » ; dans tous les autres cas, 0. | — |
| Grandparent Unit | Déterminé par priorité : 1 si tout ce qui suit est vrai : l'indicateur « holds » est activé et le property est « isGrandparentOf » ; dans tous les autres cas, 0. | — |

<details open>
<summary>Données d'exemple</summary>

| Number Value | Is Stated | Subject Label ƒ | Property Label ƒ | Object Label ƒ | Name ƒ | Count of Inverse Firings ƒ | Count of Symmetric Firings ƒ | Holds Before Chain ƒ | Count of Chain Firings ƒ | Holds Before Typing ƒ | Count of Type Firings ƒ | Holds ƒ | Is Conclusion ƒ | Worked Out by ƒ | Stated Unit ƒ | Conclusion Unit ƒ | Parent Unit ƒ | Grandparent Unit ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| — | true | Marvin | is parent of | Randy | Marvin is parent of Randy | 0 | 0 | true | 0 | true | 0 | true | false | — | 1 | 0 | 1 | 0 |
| — | false | Marvin | is parent of | Sharon | Marvin is parent of Sharon | 0 | 0 | false | 0 | false | 0 | false | false | — | 0 | 0 | 0 | 0 |
| — | false | Marvin | is parent of | Shelly | Marvin is parent of Shelly | 0 | 0 | false | 0 | false | 0 | false | false | — | 0 | 0 | 0 | 0 |

_ƒ signale une colonne calculée._

</details>

| Terme | Description | Commentaire narratif |
|------|-------------|-------------------|
| **Firing** | One way one rule could produce one fact from one or two others. | — |
| Rule | Un attribut défini. | — |
| Kind | Un attribut défini. | — |
| Target | Un attribut défini. | — |
| First | Un attribut défini. | — |
| Second | Un attribut défini. | _Same as First when the rule needs one premise._ |
| Rule Enabled | Vrai lorsque le rule lié est enabled. | — |
| Rule Label | Repris du rule lié. | — |
| Target Name | Repris du target lié. | — |
| First Name | Repris du first lié. | — |
| Second Name | Repris du second lié. | — |
| First is Stated | Vrai lorsque le first lié est stated. | — |
| First Holds Before Chain | Vrai lorsque le first lié est holds before chain. | — |
| Second Holds Before Chain | Vrai lorsque le second lié est holds before chain. | — |
| First Holds Before Typing | Vrai lorsque le first lié est holds before typing. | — |
| Inverse Fired Unit | Déterminé par priorité : 1 si tout ce qui suit est vrai : l'indicateur « rule enabled » est activé; le kind est « inverse »; et l'indicateur « first is stated » est activé ; dans tous les autres cas, 0. | — |
| Symmetric Fired Unit | Déterminé par priorité : 1 si tout ce qui suit est vrai : l'indicateur « rule enabled » est activé; le kind est « symmetric »; et l'indicateur « first is stated » est activé ; dans tous les autres cas, 0. | — |
| Chain Fired Unit | Déterminé par priorité : 1 si tout ce qui suit est vrai : l'indicateur « rule enabled » est activé; le kind est « chain »; l'indicateur « first holds before chain » est activé; et l'indicateur « second holds before chain » est activé ; dans tous les autres cas, 0. | — |
| Type Fired Unit | Déterminé par priorité : 1 si tout ce qui suit est vrai : l'indicateur « rule enabled » est activé; au moins une des conditions suivantes est vraie : le kind est « domain » ou le kind est « range »; et l'indicateur « first holds before typing » est activé ; dans tous les autres cas, 0. | — |
| Fired Unit | Calculé comme l'inverse fired unit plus le symmetric fired unit plus le chain fired unit plus le type fired unit. | — |
| Name | Déterminé par priorité : le target name, suivi de « : worked out by the », suivi de le rule label, suivi de « rule from ' », suivi de le first name, suivi de « ' », suivi de « and ' », suivi de le second name, suivi de « ' » si le fired unit est supérieur à 0 ; dans tous les autres cas, le firing ID. | _The explainer's conclusion card._ |

<details open>
<summary>Données d'exemple</summary>

| Kind | Rule Enabled ƒ | Rule Label ƒ | Target Name ƒ | First Name ƒ | Second Name ƒ | First is Stated ƒ | First Holds Before Chain ƒ | Second Holds Before Chain ƒ | First Holds Before Typing ƒ | Inverse Fired Unit ƒ | Symmetric Fired Unit ƒ | Chain Fired Unit ƒ | Type Fired Unit ƒ | Fired Unit ƒ | Name ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| inverse | true | Inverse | Randy is child of Marvin | Marvin is parent of Randy | Marvin is parent of Randy | true | true | true | true | 1 | 0 | 0 | 0 | 1 | Randy is child of Marvin: worked out by the Inverse rule from 'Marvin is parent of Randy' |
| inverse | true | Inverse | Sharon is child of Marvin | Marvin is parent of Sharon | Marvin is parent of Sharon | false | false | false | false | 0 | 0 | 0 | 0 | 0 | inverse:Marvin\|isParentOf\|Sharon>Sharon\|isChildOf\|Marvin |
| inverse | true | Inverse | Shelly is child of Marvin | Marvin is parent of Shelly | Marvin is parent of Shelly | false | false | false | false | 0 | 0 | 0 | 0 | 0 | inverse:Marvin\|isParentOf\|Shelly>Shelly\|isChildOf\|Marvin |

_ƒ signale une colonne calculée._

</details>

| Terme | Description | Commentaire narratif |
|------|-------------|-------------------|
| **Disjoint Check** | Two statements that must not both hold. | — |
| Graph | Un attribut défini. | — |
| Rule | Un attribut défini. | — |
| A | Un attribut défini. | — |
| B | Un attribut défini. | — |
| Name | Identique à disjoint check ID. | — |
| Rule Enabled | Vrai lorsque le rule lié est enabled. | — |
| A Holds | Vrai lorsque le a lié est holds. | — |
| B Holds | Vrai lorsque le b lié est holds. | — |
| Violation Unit | Déterminé par priorité : 1 si tout ce qui suit est vrai : l'indicateur « rule enabled » est activé; l'indicateur « a holds » est activé; et l'indicateur « b holds » est activé ; dans tous les autres cas, 0. | — |

<details open>
<summary>Données d'exemple</summary>

| Name ƒ | Rule Enabled ƒ | A Holds ƒ | B Holds ƒ | Violation Unit ƒ |
|---|---|---|---|---|
| Marvin\|isParentOf\|Randy~isSpouseOf | true | true | false | 0 |
| Marvin\|isParentOf\|Sharon~isSpouseOf | true | false | false | 0 |
| Marvin\|isParentOf\|Shelly~isSpouseOf | true | false | false | 0 |

_ƒ signale une colonne calculée._

</details>

## 2 Types de faits

- un **property** peut référencer un **property**
- un **property** peut référencer un **thing**
- un **statement** référence exactement un **graph**
- un **statement** référence exactement un **thing**
- un **statement** référence exactement un **property**
- un **firing** référence exactement un **rule**
- un **firing** référence exactement un **statement**
- un **disjoint check** référence exactement un **graph**
- un **disjoint check** référence exactement un **rule**
- un **disjoint check** référence exactement un **statement**

## 3 Règles opératives

_Les règles opératives énoncent ce que le métier **oblige**, **interdit** ou
conseille (**devrait**). Les règles structurelles proviennent des champs obligatoires et
des clés étrangères ; les règles sémantiques proviennent de la table Constraints, chacune
s'appuyant sur un booléen déjà calculé (référencé DR-N dans les règles définitionnelles)._

### Contraintes structurelles (issues du schéma)

- Un rule **doit** avoir un label et une explanation, et consigner si il est enabled.
- Un thing **doit** avoir un label et un kind.
- Un property **doit** avoir un label, et consigner si il est symmetric.
- Un statement **doit** référencer exactement un graph.
- Un statement **doit** référencer exactement un thing comme son subject.
- Un statement **doit** référencer exactement un property.
- Un statement **doit** consigner si il est stated.
- Un firing **doit** référencer exactement un rule.
- Un firing **doit** référencer exactement un statement comme son target.
- Un firing **doit** référencer exactement un statement comme son first.
- Un firing **doit** référencer exactement un statement comme son second.
- Un firing **doit** avoir un kind.
- Un disjoint check **doit** référencer exactement un graph.
- Un disjoint check **doit** référencer exactement un rule.
- Un disjoint check **doit** référencer exactement un statement comme son a.
- Un disjoint check **doit** référencer exactement un statement comme son b.

## 4 Règles définitionnelles

_Tous les énoncés expriment des vérités du domaine métier ; ce ne sont ni des procédures
ni des impératifs. « si et seulement si » est évité au profit de « seulement si », afin
qu'une nécessité unidirectionnelle ne soit pas confondue avec une équivalence. Une puce
**⚠︎ mécanique** signale une règle dont la formulation déterministe est fidèle mais
lourde — un signal pour une reformulation optionnelle, pas un défaut._

| ID | Règle déclarative |
|----|------------------|
| **DR-1 Name** | Un graph : name est identique à graph ID. |
| **DR-2 Count of Stated Facts** | Un graph : count of stated facts est le total de stated unit sur les statements liés à le graph. |
| **DR-3 Count of Conclusions** | Un graph : count of conclusions est le total de conclusion unit sur les statements liés à le graph. |
| **DR-4 Count of Violations** | Un graph : count of violations est le total de violation unit sur les disjoint checks liés à le graph. |
| **DR-5 Is Consistent** | Un graph est considéré comme consistent si le count of violations est 0. |
| **DR-6 Name** | Un rule : name est identique à label. |
| **DR-7 Count of Firings** | Un rule : count of firings est le total de fired unit sur les firings liés à le rule. |
| **DR-8 Name** | Un thing : name est identique à label. |
| **DR-9 Count of Children** | Un thing : count of children est le total de parent unit sur les statements liés à le thing. |
| **DR-10 Count of Grandchildren** | Un thing : count of grandchildren est le total de grandparent unit sur les statements liés à le thing. |
| **DR-11 Name** | Un property : name est identique à label. |
| **DR-12 Subject Label** | Un statement : subject label — repris du subject lié. |
| **DR-13 Property Label** | Un statement : property label — repris du property lié. |
| **DR-14 Object Label** | Un statement : object label — repris du object thing lié. |
| **DR-15 Name** | Un statement : name est calculé comme le subject label, suivi de une espace, suivi de le property label, suivi de une espace, suivi de le number value si l'object thing est vide, dans tous les autres cas l'object label. |
| **DR-16 Count of Inverse Firings** | Un statement : count of inverse firings est le total de inverse fired unit sur les firings liés à le statement. |
| **DR-17 Count of Symmetric Firings** | Un statement : count of symmetric firings est le total de symmetric fired unit sur les firings liés à le statement. |
| **DR-18 Holds Before Chain** | Un statement est considéré comme satisfaisant : before chain si au moins une des conditions suivantes est vraie : l'indicateur « stated » est activé; le count of inverse firings est supérieur à 0; ou le count of symmetric firings est supérieur à 0. |
| **DR-19 Count of Chain Firings** | Un statement : count of chain firings est le total de chain fired unit sur les firings liés à le statement. |
| **DR-20 Holds Before Typing** | Un statement est considéré comme satisfaisant : before typing si au moins une des conditions suivantes est vraie : l'indicateur « holds before chain » est activé ou le count of chain firings est supérieur à 0. |
| **DR-21 Count of Type Firings** | Un statement : count of type firings est le total de type fired unit sur les firings liés à le statement. |
| **DR-22 Holds** | Un statement est considéré comme satisfaisant : si au moins une des conditions suivantes est vraie : l'indicateur « holds before typing » est activé ou le count of type firings est supérieur à 0. |
| **DR-23 Is Conclusion** | Un statement est considéré comme une conclusion si tout ce qui suit est vrai : l'indicateur « holds » est activé et l'indicateur « stated » n'est pas activé. |
| **DR-24 Worked Out by** | Le champ « worked out by » du statement est déterminé selon la priorité suivante :<br>1. une chaîne vide, si l'indicateur « conclusion » n'est pas activé ;<br>2. « inverse », si le count of inverse firings est supérieur à 0 ;<br>3. « symmetric », si le count of symmetric firings est supérieur à 0 ;<br>4. « property chain », si le count of chain firings est supérieur à 0 ;<br>5. dans tous les autres cas, « domain and range ». |
| **DR-25 Stated Unit** | Le champ « stated unit » du statement est déterminé selon la priorité suivante :<br>1. 1, si l'indicateur « stated » est activé ;<br>2. dans tous les autres cas, 0. |
| **DR-26 Conclusion Unit** | Le champ « conclusion unit » du statement est déterminé selon la priorité suivante :<br>1. 1, si l'indicateur « conclusion » est activé ;<br>2. dans tous les autres cas, 0. |
| **DR-27 Parent Unit** | Le champ « parent unit » du statement est déterminé selon la priorité suivante :<br>1. 1, si tout ce qui suit est vrai : l'indicateur « holds » est activé et le property est « isParentOf » ;<br>2. dans tous les autres cas, 0. |
| **DR-28 Grandparent Unit** | Le champ « grandparent unit » du statement est déterminé selon la priorité suivante :<br>1. 1, si tout ce qui suit est vrai : l'indicateur « holds » est activé et le property est « isGrandparentOf » ;<br>2. dans tous les autres cas, 0. |
| **DR-29 Rule Enabled** | Un firing : rule enabled est vrai lorsque le rule lié est enabled. |
| **DR-30 Rule Label** | Un firing : rule label — repris du rule lié. |
| **DR-31 Target Name** | Un firing : target name — repris du target lié. |
| **DR-32 First Name** | Un firing : first name — repris du first lié. |
| **DR-33 Second Name** | Un firing : second name — repris du second lié. |
| **DR-34 First is Stated** | Un firing : first is stated est vrai lorsque le first lié est stated. |
| **DR-35 First Holds Before Chain** | Un firing : first holds before chain est vrai lorsque le first lié est holds before chain. |
| **DR-36 Second Holds Before Chain** | Un firing : second holds before chain est vrai lorsque le second lié est holds before chain. |
| **DR-37 First Holds Before Typing** | Un firing : first holds before typing est vrai lorsque le first lié est holds before typing. |
| **DR-38 Inverse Fired Unit** | Le champ « inverse fired unit » du firing est déterminé selon la priorité suivante :<br>1. 1, si tout ce qui suit est vrai : l'indicateur « rule enabled » est activé; le kind est « inverse »; et l'indicateur « first is stated » est activé ;<br>2. dans tous les autres cas, 0. |
| **DR-39 Symmetric Fired Unit** | Le champ « symmetric fired unit » du firing est déterminé selon la priorité suivante :<br>1. 1, si tout ce qui suit est vrai : l'indicateur « rule enabled » est activé; le kind est « symmetric »; et l'indicateur « first is stated » est activé ;<br>2. dans tous les autres cas, 0. |
| **DR-40 Chain Fired Unit** | Le champ « chain fired unit » du firing est déterminé selon la priorité suivante :<br>1. 1, si tout ce qui suit est vrai : l'indicateur « rule enabled » est activé; le kind est « chain »; l'indicateur « first holds before chain » est activé; et l'indicateur « second holds before chain » est activé ;<br>2. dans tous les autres cas, 0. |
| **DR-41 Type Fired Unit** | Le champ « type fired unit » du firing est déterminé selon la priorité suivante :<br>1. 1, si tout ce qui suit est vrai : l'indicateur « rule enabled » est activé; au moins une des conditions suivantes est vraie : le kind est « domain » ou le kind est « range »; et l'indicateur « first holds before typing » est activé ;<br>2. dans tous les autres cas, 0. |
| **DR-42 Fired Unit** | Un firing : fired unit est calculé comme l'inverse fired unit plus le symmetric fired unit plus le chain fired unit plus le type fired unit. |
| **DR-43 Name** | Le champ « name » du firing est déterminé selon la priorité suivante :<br>1. le target name, suivi de « : worked out by the », suivi de le rule label, suivi de « rule from ' », suivi de le first name, suivi de « ' », suivi de « and ' », suivi de le second name, suivi de « ' », si le fired unit est supérieur à 0 ;<br>2. dans tous les autres cas, le firing ID. |
| **DR-44 Name** | Un disjoint check : name est identique à disjoint check ID. |
| **DR-45 Rule Enabled** | Un disjoint check : rule enabled est vrai lorsque le rule lié est enabled. |
| **DR-46 A Holds** | Un disjoint check : a holds est vrai lorsque le a lié est holds. |
| **DR-47 B Holds** | Un disjoint check : b holds est vrai lorsque le b lié est holds. |
| **DR-48 Violation Unit** | Le champ « violation unit » du disjoint check est déterminé selon la priorité suivante :<br>1. 1, si tout ce qui suit est vrai : l'indicateur « rule enabled » est activé; l'indicateur « a holds » est activé; et l'indicateur « b holds » est activé ;<br>2. dans tous les autres cas, 0. |

## 5 Traçabilité vers le schéma

_La colonne expression est la définition de la règle en notation Documentation —
la même logique que stocke le livre de règles, écrite pour un lecteur métier._

| Élément du schéma | Type | Expression |
|----------------|------|------------|
| **Graphs.Name** | formule | `GraphId` |
| **Graphs.CountOfStatedFacts** | agrégat | `Sum(Statements.StatedUnit via Graph)` |
| **Graphs.CountOfConclusions** | agrégat | `Sum(Statements.ConclusionUnit via Graph)` |
| **Graphs.CountOfViolations** | agrégat | `Sum(DisjointChecks.ViolationUnit via Graph)` |
| **Graphs.IsConsistent** | formule | `CountOfViolations = 0` |
| **Rules.Name** | formule | `Label` |
| **Rules.CountOfFirings** | agrégat | `Sum(Firings.FiredUnit via Rule)` |
| **Things.Name** | formule | `Label` |
| **Things.CountOfChildren** | agrégat | `Sum(Statements.ParentUnit via Subject)` |
| **Things.CountOfGrandchildren** | agrégat | `Sum(Statements.GrandparentUnit via Subject)` |
| **Properties.Name** | formule | `Label` |
| **Statements.SubjectLabel** | recherche | `Lookup(Things.Label via Subject)` |
| **Statements.PropertyLabel** | recherche | `Lookup(Properties.Label via Property)` |
| **Statements.ObjectLabel** | recherche | `Lookup(Things.Label via ObjectThing)` |
| **Statements.Name** | formule | `SubjectLabel & " " & PropertyLabel & " " & If(ObjectThing = Blank(), NumberValue, ObjectLabel)` |
| **Statements.CountOfInverseFirings** | agrégat | `Sum(Firings.InverseFiredUnit via Target)` |
| **Statements.CountOfSymmetricFirings** | agrégat | `Sum(Firings.SymmetricFiredUnit via Target)` |
| **Statements.HoldsBeforeChain** | formule | `Or(IsStated, CountOfInverseFirings > 0, CountOfSymmetricFirings > 0)` |
| **Statements.CountOfChainFirings** | agrégat | `Sum(Firings.ChainFiredUnit via Target)` |
| **Statements.HoldsBeforeTyping** | formule | `Or(HoldsBeforeChain, CountOfChainFirings > 0)` |
| **Statements.CountOfTypeFirings** | agrégat | `Sum(Firings.TypeFiredUnit via Target)` |
| **Statements.Holds** | formule | `Or(HoldsBeforeTyping, CountOfTypeFirings > 0)` |
| **Statements.IsConclusion** | formule | `And(Holds, Not(IsStated))` |
| **Statements.WorkedOutBy** | formule | `If(Not(IsConclusion), "", If(CountOfInverseFirings > 0, "inverse", If(CountOfSymmetricFirings > 0, "symmetric", If(CountOfChainFirings > 0, "property chain", "domain and range"))))` |
| **Statements.StatedUnit** | formule | `If(IsStated, 1, 0)` |
| **Statements.ConclusionUnit** | formule | `If(IsConclusion, 1, 0)` |
| **Statements.ParentUnit** | formule | `If(And(Holds, Property = "isParentOf"), 1, 0)` |
| **Statements.GrandparentUnit** | formule | `If(And(Holds, Property = "isGrandparentOf"), 1, 0)` |
| **Firings.RuleEnabled** | recherche | `Lookup(Rules.IsEnabled via Rule)` |
| **Firings.RuleLabel** | recherche | `Lookup(Rules.Label via Rule)` |
| **Firings.TargetName** | recherche | `Lookup(Statements.Name via Target)` |
| **Firings.FirstName** | recherche | `Lookup(Statements.Name via First)` |
| **Firings.SecondName** | recherche | `Lookup(Statements.Name via Second)` |
| **Firings.FirstIsStated** | recherche | `Lookup(Statements.IsStated via First)` |
| **Firings.FirstHoldsBeforeChain** | recherche | `Lookup(Statements.HoldsBeforeChain via First)` |
| **Firings.SecondHoldsBeforeChain** | recherche | `Lookup(Statements.HoldsBeforeChain via Second)` |
| **Firings.FirstHoldsBeforeTyping** | recherche | `Lookup(Statements.HoldsBeforeTyping via First)` |
| **Firings.InverseFiredUnit** | formule | `If(And(RuleEnabled, Kind = "inverse", FirstIsStated), 1, 0)` |
| **Firings.SymmetricFiredUnit** | formule | `If(And(RuleEnabled, Kind = "symmetric", FirstIsStated), 1, 0)` |
| **Firings.ChainFiredUnit** | formule | `If(And(RuleEnabled, Kind = "chain", FirstHoldsBeforeChain, SecondHoldsBeforeChain), 1, 0)` |
| **Firings.TypeFiredUnit** | formule | `If(And(RuleEnabled, Or(Kind = "domain", Kind = "range"), FirstHoldsBeforeTyping), 1, 0)` |
| **Firings.FiredUnit** | formule | `InverseFiredUnit + SymmetricFiredUnit + ChainFiredUnit + TypeFiredUnit` |
| **Firings.Name** | formule | `If(FiredUnit > 0, TargetName & ": worked out by the " & RuleLabel & " rule from '" & FirstName & "'" & If(Kind = "chain", " and '" & SecondName & "'", ""), FiringId)` |
| **DisjointChecks.Name** | formule | `DisjointCheckId` |
| **DisjointChecks.RuleEnabled** | recherche | `Lookup(Rules.IsEnabled via Rule)` |
| **DisjointChecks.AHolds** | recherche | `Lookup(Statements.Holds via A)` |
| **DisjointChecks.BHolds** | recherche | `Lookup(Statements.Holds via B)` |
| **DisjointChecks.ViolationUnit** | formule | `If(And(RuleEnabled, AHolds, BHolds), 1, 0)` |
