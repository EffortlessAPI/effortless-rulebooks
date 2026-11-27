# 📘 Talisman's South Park ontology explainer: the concept form — Documentation

_This ontology in the concept form: a table of individuals, a table per class, a table or a column per property, and ERBTables and ERBFields for what the structure cannot say. orchestration/ontology_form.py wrote it; injecting it over the ontology brings the ontology's things, properties, axioms and facts in line with these tables._

> Règles métier déclaratives générées à partir du livre de règles. Chaque énoncé
> exprime une vérité du domaine métier — ce n'est ni une procédure ni un impératif.
> Les formules du livre de règles sont l'unique source de vérité ; ce document en
> est la lecture en langage clair.

## 1 Vocabulaire métier

| Terme | Description | Commentaire narratif |
|------|-------------|-------------------|
| **Thing** | Every individual of the ontology (owl:Thing). A class's table lists the individuals that belong to it. | — |
| Label | Un attribut défini. | _The name the page shows._ |
| Sort Order | Un attribut défini. | _Position in the cast._ |
| Portrait | Un attribut défini. | _The portrait drawn for this thing; set when its name matches a portrait alias._ |

<details open>
<summary>Données d'exemple</summary>

| Label | Sort Order |
|---|---|
| Marvin | 1 |
| Randy | 2 |
| Sharon | 3 |

</details>

| Terme | Description | Commentaire narratif |
|------|-------------|-------------------|
| **Person** | The class Person: one row per individual that belongs to it. Its label and place are its ERBTables row. | — |
| Thing | Un attribut défini. | _The individual's row in Things._ |
| Is Asserted | Vrai lorsque une chaîne vide. | _True when a fact states it is a Person. False when only a property's domain or range implies it: such a row follows from the facts and is not added by hand._ |
| has Grade Level | Un attribut défini. | _has grade level: the one current value. A property of the ontology; its other attributes are its ERBFields row._ |
| has Role | Un attribut défini. | _has role: the one current value. A property of the ontology; its other attributes are its ERBFields row._ |

<details open>
<summary>Données d'exemple</summary>

| Is Asserted | has Grade Level | has Role |
|---|---|---|
| true | 4 | Student |

</details>

| Terme | Description | Commentaire narratif |
|------|-------------|-------------------|
| **School** | The class School: one row per individual that belongs to it. Its label and place are its ERBTables row. | — |
| Thing | Un attribut défini. | _The individual's row in Things._ |
| Is Asserted | Vrai lorsque une chaîne vide. | _True when a fact states it is a School. False when only a property's domain or range implies it: such a row follows from the facts and is not added by hand._ |

<details open>
<summary>Données d'exemple</summary>

| Is Asserted |
|---|
| false |

</details>

| Terme | Description | Commentaire narratif |
|------|-------------|-------------------|
| **is Parent of** | is parent of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | Un attribut défini. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | Un attribut défini. | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| Terme | Description | Commentaire narratif |
|------|-------------|-------------------|
| **is Child of** | is child of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | Un attribut défini. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | Un attribut défini. | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| Terme | Description | Commentaire narratif |
|------|-------------|-------------------|
| **is Spouse of** | is spouse of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | Un attribut défini. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | Un attribut défini. | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| Terme | Description | Commentaire narratif |
|------|-------------|-------------------|
| **is Friend of** | is friend of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | Un attribut défini. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | Un attribut défini. | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| Terme | Description | Commentaire narratif |
|------|-------------|-------------------|
| **attend** | attends: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | Un attribut défini. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | Un attribut défini. | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| Terme | Description | Commentaire narratif |
|------|-------------|-------------------|
| **has Age** | has age: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | Un attribut défini. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Value | Un attribut défini. | _The value._ |

<details open>
<summary>Données d'exemple</summary>

| Value |
|---|
| 10 |

</details>

| Terme | Description | Commentaire narratif |
|------|-------------|-------------------|
| **is Grandparent of** | is grandparent of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row. | — |
| Subject | Un attribut défini. | _Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing._ |
| Object | Un attribut défini. | _What it links to. The table it links to is the property's range: a class's table, or Things for any thing._ |

| Terme | Description | Commentaire narratif |
|------|-------------|-------------------|
| **Portrait** | The page's Fandom portraits, including those for characters a reader may add. | — |
| Display Name | Un attribut défini. | _The name on the portrait's Fandom page._ |
| Page | Un attribut défini. | _The Fandom page credited for the portrait._ |
| Image | Un attribut défini. | _The original Fandom image._ |
| Crop | Un attribut défini. | _The crop as an SVG viewBox: x y width height._ |
| Width | Un attribut défini. | _The original image's width in pixels._ |
| Height | Un attribut défini. | _The original image's height in pixels._ |
| Sort Order | Un attribut défini. | _Position in the credits._ |
| Name | Identique à display name. | _Order 1. The portrait's name._ |

<details open>
<summary>Données d'exemple</summary>

| Display Name | Page | Image | Crop | Width | Height | Sort Order | Name ƒ |
|---|---|---|---|---|---|---|---|
| Marvin Marsh | https://southpark.fandom.com/wiki/Marvin_Marsh | https://static.wikia.nocookie.net/southpark/images/d/db/Marsh-family-grandpa-marsh-w-purple-blanket.png/revision/latest | 48 -5 280 280 | 375 | 536 | 1 | Marvin Marsh |
| Randy Marsh | https://southpark.fandom.com/wiki/Randy_Marsh | https://static.wikia.nocookie.net/southparkfanon/images/d/d6/Randy_Marsh.png/revision/latest | 0 -10 428 428 | 428 | 1057 | 2 | Randy Marsh |
| Sharon Marsh | https://southpark.fandom.com/wiki/Sharon_Marsh | https://static.wikia.nocookie.net/southpark/images/9/99/Sharon_Marsh.png/revision/latest | 20 -10 390 390 | 439 | 1049 | 3 | Sharon Marsh |

_ƒ signale une colonne calculée._

</details>

| Terme | Description | Commentaire narratif |
|------|-------------|-------------------|
| **ERB Table** | One row per table of this rulebook (the platform's Schema module), and on it what the table's structure does not say. A class: its label and its place in the cast. A property that is a table: its label, its wording on the page, whether it is symmetric, and its inverse, chain and disjoint axioms. What a table IS is read from its fields: Thing and IsAsserted make a class; Subject with Object or Value make a property, whose domain and range are the tables they link to. | — |
| Table Name | Un attribut défini. | _The table's name again, as a plain column._ |
| Description | Un attribut défini. | _What the table holds. Follows the table._ |
| Is Meta Table | Vrai lorsque une chaîne vide. | _True for a table that serves the rulebook itself (ERBTables, ERBFields) and not the ontology._ |
| Is Tenant Scoped | Vrai lorsque une chaîne vide. | _The Schema module's: rows belong to one tenant. Not used by the ontology._ |
| Tenant Parent Table | Un attribut défini. | _The Schema module's: the table tenancy is reached through. Not used by the ontology._ |
| Tenant Parent Field | Un attribut défini. | _The Schema module's: the field that points at TenantParentTable. Not used by the ontology._ |
| Sort Order | Un attribut défini. | _Display order. For a class: Position in the cast. For a property: Position in the Add fact form's property list._ |
| Label | Un attribut défini. | _For a class: The name the page shows. For a property: The property's wording on the page._ |
| Portrait | Un attribut défini. | _For a class: The portrait drawn for this thing; set when its name matches a portrait alias._ |
| Is User Addable | Vrai lorsque une chaîne vide. | _For a property: Offered in the page's Add fact form._ |
| Is Symmetric | Vrai lorsque une chaîne vide. | _For a property: The symmetric rule makes this link true in both directions._ |
| Value Node Label | Un attribut défini. | _For a property: The small label under a value node for this property: 'role value' or 'number value'._ |
| Current Value Wording | Un attribut défini. | _For a property: How the Add fact form names a replaced value, as in 'Stan now has grade level 5'._ |
| Input Name | Un attribut défini. | _For a property: How the integer check names this property's value, as in 'for grade level'._ |
| Record Kind | Un attribut défini. | _For a property: The word the current-record summary uses for this property's values: role, grade or school._ |
| Record Order | Un attribut défini. | _For a property: Position of this property in the current-record summary._ |
| Inverse of | Un attribut défini. | _The property that reads this one backwards (OWL inverseOf): when A has this link to B, the inverse rule concludes that B has that one to A. Names a property: a property's table, or a column that is a property._ |
| Chain First | Un attribut défini. | _With ChainSecond, the property chain that concludes this property (OWL propertyChainAxiom): a ChainFirst link followed by a ChainSecond link._ |
| Chain Second | Un attribut défini. | _The chain's second link. Set together with ChainFirst._ |
| Disjoint With | Un attribut défini. | _Properties that never both hold between the same two things (OWL propertyDisjointWith). Several are separated by ', '._ |

<details open>
<summary>Données d'exemple</summary>

| Table Name | Description | Is Meta Table | Is Tenant Scoped | Tenant Parent Field | Sort Order | Label | Is User Addable | Is Symmetric | Value Node Label | Current Value Wording | Input Name | Record Kind | Record Order | Inverse of | Chain First | Chain Second | Disjoint With |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| Things | Every individual of the ontology (owl:Thing). A class's table lists the individuals that belong to it. | false | — | — | — | — | — | — | — | — | — | — | — | — | — | — | — |
| Person | The class Person: one row per individual that belongs to it. Its label and place are its ERBTables row. | false | — | — | 8 | Person | — | — | — | — | — | — | — | — | — | — | — |
| School | The class School: one row per individual that belongs to it. Its label and place are its ERBTables row. | false | — | — | 9 | School | — | — | — | — | — | — | — | — | — | — | — |

</details>

| Terme | Description | Commentaire narratif |
|------|-------------|-------------------|
| **ERB Field** | One row per field of every table (the platform's Schema module), and on it the same attributes for a property that is a column: a property with one current value, on its domain's class table (on Things when it has no domain class). | — |
| ERB Table | Un attribut défini. | _The table the field is on._ |
| Field Name | Un attribut défini. | _The field's name. For a column that is a property: the property's id._ |
| Field Type | Un attribut défini. | _raw or relationship. Follows the field._ |
| Datatype | Un attribut défini. | _The rulebook datatype. Follows the field._ |
| Is Primary Key | Vrai lorsque une chaîne vide. | _True for the field that identifies a row of its table._ |
| Description | Un attribut défini. | _What the field means. Follows the field._ |
| Label | Un attribut défini. | _For a property: The property's wording on the page._ |
| Is User Addable | Vrai lorsque une chaîne vide. | _For a property: Offered in the page's Add fact form._ |
| Is Symmetric | Vrai lorsque une chaîne vide. | _For a property: The symmetric rule makes this link true in both directions._ |
| Sort Order | Un attribut défini. | _For a property: Position in the Add fact form's property list._ |
| Value Node Label | Un attribut défini. | _For a property: The small label under a value node for this property: 'role value' or 'number value'._ |
| Current Value Wording | Un attribut défini. | _For a property: How the Add fact form names a replaced value, as in 'Stan now has grade level 5'._ |
| Input Name | Un attribut défini. | _For a property: How the integer check names this property's value, as in 'for grade level'._ |
| Record Kind | Un attribut défini. | _For a property: The word the current-record summary uses for this property's values: role, grade or school._ |
| Record Order | Un attribut défini. | _For a property: Position of this property in the current-record summary._ |
| Inverse of | Un attribut défini. | _The property that reads this one backwards (OWL inverseOf): when A has this link to B, the inverse rule concludes that B has that one to A. Names a property: a property's table, or a column that is a property._ |
| Chain First | Un attribut défini. | _With ChainSecond, the property chain that concludes this property (OWL propertyChainAxiom): a ChainFirst link followed by a ChainSecond link._ |
| Chain Second | Un attribut défini. | _The chain's second link. Set together with ChainFirst._ |
| Disjoint With | Un attribut défini. | _Properties that never both hold between the same two things (OWL propertyDisjointWith). Several are separated by ', '._ |

<details open>
<summary>Données d'exemple</summary>

| Field Name | Field Type | Datatype | Is Primary Key | Description | Label | Is User Addable | Is Symmetric | Sort Order | Value Node Label | Current Value Wording | Input Name | Record Kind | Record Order | Inverse of | Chain First | Chain Second | Disjoint With |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| ThingId | raw | string | true | Key of the thing; also its local name in the model namespace. | — | — | — | — | — | — | — | — | — | — | — | — | — |
| Label | raw | string | false | The name the page shows. | — | — | — | — | — | — | — | — | — | — | — | — | — |
| SortOrder | raw | integer | false | Position in the cast. | — | — | — | — | — | — | — | — | — | — | — | — | — |

</details>

## 2 Types de faits

- un **thing** peut référencer un **portrait**
- un **person** référence exactement un **thing**
- un **school** référence exactement un **thing**
- un **is parent of** référence exactement un **thing**
- un **is child of** référence exactement un **thing**
- un **is spouse of** référence exactement un **thing**
- un **is friend of** référence exactement un **thing**
- un **attend** référence exactement un **person**
- un **attend** référence exactement un **school**
- un **has age** référence exactement un **thing**
- un **is grandparent of** référence exactement un **thing**
- un **ERB table** peut référencer un **ERB table**
- un **ERB table** peut référencer un **portrait**
- un **ERB field** peut référencer un **ERB table**

## 3 Règles opératives

_Les règles opératives énoncent ce que le métier **oblige**, **interdit** ou
conseille (**devrait**). Les règles structurelles proviennent des champs obligatoires et
des clés étrangères ; les règles sémantiques proviennent de la table Constraints, chacune
s'appuyant sur un booléen déjà calculé (référencé DR-N dans les règles définitionnelles)._

### Contraintes structurelles (issues du schéma)

- Un thing **doit** avoir un label et un sort order.
- Un person **doit** référencer exactement un thing.
- Un person **doit** consigner si il est asserted.
- Un school **doit** référencer exactement un thing.
- Un school **doit** consigner si il est asserted.
- Un is parent of **doit** référencer exactement un thing comme son subject.
- Un is parent of **doit** référencer exactement un thing comme son object.
- Un is child of **doit** référencer exactement un thing comme son subject.
- Un is child of **doit** référencer exactement un thing comme son object.
- Un is spouse of **doit** référencer exactement un thing comme son subject.
- Un is spouse of **doit** référencer exactement un thing comme son object.
- Un is friend of **doit** référencer exactement un thing comme son subject.
- Un is friend of **doit** référencer exactement un thing comme son object.
- Un attend **doit** référencer exactement un person comme son subject.
- Un attend **doit** référencer exactement un school comme son object.
- Un has age **doit** référencer exactement un thing comme son subject.
- Un has age **doit** avoir un value.
- Un is grandparent of **doit** référencer exactement un thing comme son subject.
- Un is grandparent of **doit** référencer exactement un thing comme son object.
- Un portrait **doit** avoir un display name; un page; un image; un crop; un width; un height; et un sort order.

## 4 Règles définitionnelles

_Tous les énoncés expriment des vérités du domaine métier ; ce ne sont ni des procédures
ni des impératifs. « si et seulement si » est évité au profit de « seulement si », afin
qu'une nécessité unidirectionnelle ne soit pas confondue avec une équivalence. Une puce
**⚠︎ mécanique** signale une règle dont la formulation déterministe est fidèle mais
lourde — un signal pour une reformulation optionnelle, pas un défaut._

| ID | Règle déclarative |
|----|------------------|
| **DR-1 Name** | Un portrait : name est identique à display name. |

## 5 Traçabilité vers le schéma

_La colonne expression est la définition de la règle en notation Documentation —
la même logique que stocke le livre de règles, écrite pour un lecteur métier._

| Élément du schéma | Type | Expression |
|----------------|------|------------|
| **Portraits.Name** | formule | `DisplayName` |

---

> **RuleSpeak®** — la notation déclarative de règles métier créée par **Ronald G. Ross**, selon les conventions de **SBVR** — arrive bientôt. D’ici là, cette page présente la documentation habituelle. [www.RonRoss.info](https://www.RonRoss.info)
