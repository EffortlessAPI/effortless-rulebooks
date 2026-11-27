# 📘 Talisman's South Park ontology explainer — Documentación

_Jessica Talisman's South Park Ontology Explainer, as a rulebook: the page's ten facts, five rule switches, property wiring, provenance wording and current-value policy are rows, and every conclusion is a formula. The rules apply in three fixed strata (inverse and symmetric, then the property chain, then domain and range); property wiring that feeds an earlier stratum would need repeated rounds. Every fact that could hold over the cast is a candidate row; app/lib/candidates.mjs writes the candidates when a thing, value or property is added._

> Reglas de negocio declarativas generadas a partir del libro de reglas. Cada
> enunciado expresa una verdad del dominio de negocio — no es un procedimiento
> ni un imperativo. Las fórmulas del libro de reglas son la única fuente de la
> verdad; este documento es su lectura en lenguaje claro.

## 1 Vocabulario de negocio

| Término | Descripción | Comentario narrativo |
|------|-------------|-------------------|
| **Graph** | The explainer page as one graph: its wording variables and its totals. | — |
| Label | Un atributo definido. | _Display name of the graph._ |
| Literal Quote | Un atributo definido. | _The quote mark written around a literal value in a fact's wording._ |
| Literal Type Marker | Un atributo definido. | _The marker written between a literal value and its datatype, as in Turtle._ |
| Built in Source Text | Un atributo definido. | _Source line shown for a fact the page states as a teaching example._ |
| User Source Text | Un atributo definido. | _Source line shown for a fact the reader stated._ |
| No Conclusions Text | Un atributo definido. | _Message shown when the selected facts and rules add no conclusion._ |
| Name | Igual a label. | _Order 1. Display name of the graph._ |
| Count of Stated Facts | El total de in effect unit entre los statements relacionados con el graph. | _Order 5. Facts that are switched on and in effect._ |
| Count of Conclusions | El total de conclusion unit entre los statements relacionados con el graph. | _Order 18. Facts that hold only because an enabled rule concluded them._ |
| Count of Violations | El total de violation unit entre los disjoint checks relacionados con el graph. | _Order 18. Disjointness checks that found both links between the same pair._ |
| Is Consistent | Verdadero cuando el count of violations es 0. | _Order 19. True when no enabled disjointness rule is violated._ |
| Conclusions Message | Determinado por prioridad: el no conclusions text si el count of conclusions es 0; en todos los demás casos, una cadena vacía. | _Order 19. The empty-state message for the conclusions panel, blank when there are conclusions._ |
| Count of Facts to Add | El total de listed off unit entre los statements relacionados con el graph. | _Order 5. Listed facts that are not in effect; the Add next fact button is enabled while this is above zero._ |
| Has Facts to Add | Verdadero cuando el count of facts to add es mayor que 0. | _Order 6. True while some listed fact can still be added._ |
| Next Fact Sort Order | El menor valor de sort order entre los statements del graph que son listed off unit. | _Order 5. List position of the first listed fact that is not in effect._ |
| Max Activation Seq | El mayor valor de activation seq entre los statements relacionados con el graph. | _Order 1. Highest activation number used so far._ |
| Next Activation Seq | Calculado como el max activation seq más 1. | _Order 2. The activation number the next switched-on fact takes._ |
| Max Listed Sort Order | El mayor valor de sort order entre los statements del graph que son listed unit. | _Order 2. Highest list position among listed facts._ |
| Next Listed Sort Order | Calculado como el max listed sort order más 1. | _Order 3. The list position a newly added fact takes._ |
| Max Sort Order | El mayor valor de sort order entre los statements relacionados con el graph. | _Order 1. Highest sort order of any fact, listed or candidate._ |
| Next Candidate Sort Order | Calculado como el max sort order más 1. | _Order 2. The sort order a new candidate fact, or a fact removed from the list, takes._ |

<details open>
<summary>Datos de ejemplo</summary>

| Label | Literal Quote | Literal Type Marker | Built in Source Text | User Source Text | No Conclusions Text | Name ƒ | Count of Stated Facts ƒ | Count of Conclusions ƒ | Count of Violations ƒ | Is Consistent ƒ | Conclusions Message ƒ | Count of Facts to Add ƒ | Has Facts to Add ƒ | Next Fact Sort Order ƒ | Max Activation Seq ƒ | Next Activation Seq ƒ | Max Listed Sort Order ƒ | Next Listed Sort Order ƒ | Max Sort Order ƒ | Next Candidate Sort Order ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| South Park | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | These facts and selected rules add no further conclusions. Try another fact or rule. | South Park | 10 | 0 | 0 | true | These facts and selected rules add no further conclusions. Try another fact or rule. | 0 | false | 0 | 10 | 11 | 10 | 11 | 100269 | 100270 |

_ƒ indica una columna calculada._

</details>

| Término | Descripción | Comentario narrativo |
|------|-------------|-------------------|
| **Rule** | The five rule switches on the page. | — |
| Label | Un atributo definido. | _The rule's name on the page._ |
| Explanation | Un atributo definido. | _The rule's one-line explanation on the page._ |
| Is Enabled | Verdadero cuando una cadena vacía. | _The rule's checkbox._ |
| Sort Order | Un atributo definido. | _Position in the rule list._ |
| Name | Igual a label. | _Order 1. The rule's name._ |
| Count of Firings | El total de fired unit entre los firings relacionados con el rule. | _Order 15. Rule applications that fired, counting every derivation._ |

<details open>
<summary>Datos de ejemplo</summary>

| Label | Explanation | Is Enabled | Sort Order | Name ƒ | Count of Firings ƒ |
|---|---|---|---|---|---|
| Inverse | A parent link also gives a child link between the same two people. | false | 1 | Inverse | 0 |
| Symmetric | A friend link or a spouse link is true for both people. | false | 2 | Symmetric | 0 |
| Property chain | A parent of a parent is a grandparent. | false | 3 | Property chain | 0 |

_ƒ indica una columna calculada._

</details>

| Término | Descripción | Comentario narrativo |
|------|-------------|-------------------|
| **Firing Kind** | The ways a rule can apply. Domain and range belong to one rule but are worded apart; 'disjoint' names the rule the disjointness checks answer to. | — |
| Rule | Un atributo definido. | _The rule switch this kind belongs to._ |
| Wording | Un atributo definido. | _How a conclusion card names the rule, as in 'Worked out by the inverse rule'._ |
| Sort Order | Un atributo definido. | _Order in which the page's reasoner tries the kinds for one fact._ |
| Name | Igual a wording. | _Order 1. The kind's wording._ |
| Rule Enabled | Verdadero cuando el rule vinculado es enabled. | _Order 1. Whether the owning rule is switched on._ |

<details open>
<summary>Datos de ejemplo</summary>

| Wording | Sort Order | Name ƒ | Rule Enabled ƒ |
|---|---|---|---|
| inverse rule | 1 | inverse rule | false |
| symmetric rule | 2 | symmetric rule | false |
| property chain rule | 3 | property chain rule | false |

_ƒ indica una columna calculada._

</details>

| Término | Descripción | Comentario narrativo |
|------|-------------|-------------------|
| **Thing** | Every individual and class the page can draw. | — |
| Label | Un atributo definido. | _The name the page shows._ |
| Kind | Un atributo definido. | _'individual' or 'class'._ |
| Sort Order | Un atributo definido. | _Position in the cast._ |
| Portrait | Un atributo definido. | _The portrait drawn for this thing; set when its name matches a portrait alias._ |
| Portrait Name | El display name del portrait vinculado del thing. | _Order 1. The name on the portrait's Fandom page._ |
| Portrait Page | Tomado del portrait vinculado. | _Order 1. The Fandom page credited for the portrait._ |
| Portrait Image | Tomado del portrait vinculado. | _Order 1. The original Fandom image._ |
| Portrait Crop | Tomado del portrait vinculado. | _Order 1. The portrait's crop as an SVG viewBox._ |
| Portrait Width | Tomado del portrait vinculado. | _Order 1. The original image's width in pixels._ |
| Portrait Height | Tomado del portrait vinculado. | _Order 1. The original image's height in pixels._ |
| Name | Igual a label. | _Order 1. The thing's name._ |
| Node Key | Igual a thing ID. | _Order 1. The key the graph draws this thing under._ |
| Count of Children | El total de parent unit entre los statements relacionados con el thing. | _Order 17. People this thing is a parent of, stated or concluded._ |
| Count of Grandchildren | El total de grandparent unit entre los statements relacionados con el thing. | _Order 17. People this thing is a grandparent of._ |
| Count As Place | El total de place unit entre los statements relacionados con el thing. | _Order 17. Holding attends facts whose object is this thing._ |
| Count As Subject | El total de holds unit entre los statements relacionados con el thing. | _Order 17. Holding facts about this thing._ |
| Count As Object | El total de holds unit entre los statements relacionados con el thing. | _Order 17. Holding facts that point at this thing._ |
| Is in Graph | Verdadero cuando se cumple al menos una de las siguientes: el count as subject es mayor que 0 o el count as object es mayor que 0. | _Order 18. True when the graph draws this thing._ |
| Node Kind | Determinado por prioridad: una cadena vacía si el kind está vacío; «class» si el kind es «class»; «place» si el count as place es mayor que 0; en todos los demás casos, «person». | _Order 18. How the graph draws the thing: a class, a place someone attends, or a person._ |

<details open>
<summary>Datos de ejemplo</summary>

| Label | Kind | Sort Order | Portrait Name ƒ | Portrait Page ƒ | Portrait Image ƒ | Portrait Crop ƒ | Portrait Width ƒ | Portrait Height ƒ | Name ƒ | Node Key ƒ | Count of Children ƒ | Count of Grandchildren ƒ | Count As Place ƒ | Count As Subject ƒ | Count As Object ƒ | Is in Graph ƒ | Node Kind ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| Marvin | individual | 1 | Marvin Marsh | https://southpark.fandom.com/wiki/Marvin_Marsh | https://static.wikia.nocookie.net/southpark/images/d/db/Marsh-family-grandpa-marsh-w-purple-blanket.png/revision/latest | 48 -5 280 280 | 375 | 536 | Marvin | Marvin | 1 | 0 | 0 | 1 | 0 | true | person |
| Randy | individual | 2 | Randy Marsh | https://southpark.fandom.com/wiki/Randy_Marsh | https://static.wikia.nocookie.net/southparkfanon/images/d/d6/Randy_Marsh.png/revision/latest | 0 -10 428 428 | 428 | 1057 | Randy | Randy | 2 | 0 | 0 | 3 | 1 | true | person |
| Sharon | individual | 3 | Sharon Marsh | https://southpark.fandom.com/wiki/Sharon_Marsh | https://static.wikia.nocookie.net/southpark/images/9/99/Sharon_Marsh.png/revision/latest | 20 -10 390 390 | 439 | 1049 | Sharon | Sharon | 0 | 0 | 0 | 0 | 1 | true | person |

_ƒ indica una columna calculada._

</details>

| Término | Descripción | Comentario narrativo |
|------|-------------|-------------------|
| **Portrait** | The page's Fandom portraits, including those for characters a reader may add. | — |
| Display Name | Un atributo definido. | _The name on the portrait's Fandom page._ |
| Page | Un atributo definido. | _The Fandom page credited for the portrait._ |
| Image | Un atributo definido. | _The original Fandom image._ |
| Crop | Un atributo definido. | _The crop as an SVG viewBox: x y width height._ |
| Width | Un atributo definido. | _The original image's width in pixels._ |
| Height | Un atributo definido. | _The original image's height in pixels._ |
| Sort Order | Un atributo definido. | _Position in the credits._ |
| Name | Igual a display name. | _Order 1. The portrait's name._ |

<details open>
<summary>Datos de ejemplo</summary>

| Display Name | Page | Image | Crop | Width | Height | Sort Order | Name ƒ |
|---|---|---|---|---|---|---|---|
| Marvin Marsh | https://southpark.fandom.com/wiki/Marvin_Marsh | https://static.wikia.nocookie.net/southpark/images/d/db/Marsh-family-grandpa-marsh-w-purple-blanket.png/revision/latest | 48 -5 280 280 | 375 | 536 | 1 | Marvin Marsh |
| Randy Marsh | https://southpark.fandom.com/wiki/Randy_Marsh | https://static.wikia.nocookie.net/southparkfanon/images/d/d6/Randy_Marsh.png/revision/latest | 0 -10 428 428 | 428 | 1057 | 2 | Randy Marsh |
| Sharon Marsh | https://southpark.fandom.com/wiki/Sharon_Marsh | https://static.wikia.nocookie.net/southpark/images/9/99/Sharon_Marsh.png/revision/latest | 20 -10 390 390 | 439 | 1049 | 3 | Sharon Marsh |

_ƒ indica una columna calculada._

</details>

| Término | Descripción | Comentario narrativo |
|------|-------------|-------------------|
| **Portrait Aliase** | The names a portrait answers to. A new thing whose name matches an alias gets that portrait. | — |
| Portrait | Un atributo definido. | _The portrait this name selects._ |
| Name | Igual a portrait alias ID. | _Order 1. The alias._ |

<details open>
<summary>Datos de ejemplo</summary>

| Name ƒ |
|---|
| marvin |
| marvin marsh |
| grandpa marsh |

_ƒ indica una columna calculada._

</details>

| Término | Descripción | Comentario narrativo |
|------|-------------|-------------------|
| **Property** | The ten properties, with what the symmetric, domain and range rules read. Axioms that relate one property to another (inverse, chain, disjoint) are rows of PropertyAxioms. | — |
| Label | Un atributo definido. | _The property's wording on the page._ |
| Property Kind | Un atributo definido. | _'object' links two things, 'datatype' links a thing to a value, 'type' states class membership._ |
| Datatype | Un atributo definido. | _The value's XML Schema datatype for a datatype property; blank otherwise._ |
| Is User Addable | Verdadero cuando una cadena vacía. | _Offered in the page's Add fact form._ |
| Has One Current Value | Verdadero cuando una cadena vacía. | _The page keeps one current value per person: switching a value on replaces the earlier one._ |
| Is Symmetric | Verdadero cuando una cadena vacía. | _The symmetric rule makes this link true in both directions._ |
| Domain Class | Un atributo definido. | _The class the domain rule gives this property's subject._ |
| Range Class | Un atributo definido. | _The class the range rule gives this property's object._ |
| Sort Order | Un atributo definido. | _Position in the Add fact form's property list._ |
| Value Node Label | Un atributo definido. | _The small label under a value node for this property: 'role value' or 'number value'._ |
| Current Value Wording | Un atributo definido. | _How the Add fact form names a replaced value, as in 'Stan now has grade level 5'._ |
| Input Name | Un atributo definido. | _How the integer check names this property's value, as in 'for grade level'._ |
| Record Kind | Un atributo definido. | _The word the current-record summary uses for this property's values: role, grade or school._ |
| Record Order | Un atributo definido. | _Position of this property in the current-record summary._ |
| Name | Igual a label. | _Order 1. The property's wording._ |

<details open>
<summary>Datos de ejemplo</summary>

| Label | Property Kind | Datatype | Is User Addable | Has One Current Value | Is Symmetric | Sort Order | Value Node Label | Current Value Wording | Input Name | Record Kind | Record Order | Name ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|
| is parent of | object | — | true | false | false | 1 | — | — | — | — | — | is parent of |
| is child of | object | — | true | false | false | 2 | — | — | — | — | — | is child of |
| is spouse of | object | — | true | false | true | 3 | — | — | — | — | — | is spouse of |

_ƒ indica una columna calculada._

</details>

| Término | Descripción | Comentario narrativo |
|------|-------------|-------------------|
| **Property Axiom** | Axioms that relate one property to others, as OWL states them: an inverse, a two-link chain, or a disjoint pair. The candidate generator reads these rows. | — |
| Kind | Un atributo definido. | _inverse, chain or disjoint._ |
| Property | Un atributo definido. | _The property the axiom is about: the one inverted, the one a chain derives, or the first of a disjoint pair._ |
| First | Un atributo definido. | _The inverse property, the chain's first link, or the other property of a disjoint pair._ |
| Second | Un atributo definido. | _The chain's second link._ |
| Name | Igual a property axiom ID. | _Order 1. The axiom's key._ |

<details open>
<summary>Datos de ejemplo</summary>

| Name ƒ |
|---|
| inverse:isParentOf |
| disjoint:isParentOf~isSpouseOf |
| inverse:isChildOf |

_ƒ indica una columna calculada._

</details>

| Término | Descripción | Comentario narrativo |
|------|-------------|-------------------|
| **Statement** | Every fact the page can state or conclude about the cast. A listed fact appears in the page's fact list; the rest are candidates that only a rule can make hold. | — |
| Graph | Un atributo definido. | _The graph the fact belongs to._ |
| Subject | Un atributo definido. | _What the fact describes._ |
| Property | Un atributo definido. | _How the subject connects to the object._ |
| Object Thing | Un atributo definido. | _The related thing, for an object or type property._ |
| Object Value | Un atributo definido. | _The literal value, for a datatype property._ |
| Is Listed | Verdadero cuando una cadena vacía. | _Shown in the page's fact list._ |
| Is Built in | Verdadero cuando una cadena vacía. | _One of the page's teaching facts; the reader cannot remove it._ |
| Is Detail | Verdadero cuando una cadena vacía. | _One of Stan's detail facts that Show Stan's details switches on._ |
| Is Stated | Verdadero cuando una cadena vacía. | _The fact's checkbox._ |
| Activation Seq | Un atributo definido. | _When the fact was last switched on, as a running number; decides which value is current._ |
| Sort Order | Un atributo definido. | _Position in the fact list; candidates follow the listed facts._ |
| Subject Label | Tomado del subject vinculado. | _Order 1. The subject's name._ |
| Property Label | Tomado del property vinculado. | _Order 1. The property's wording._ |
| Object Label | Tomado del object thing vinculado. | _Order 1. The object's name, for an object or type property._ |
| Property Datatype | Tomado del property vinculado. | _Order 1. The value's datatype, for a datatype property._ |
| Property Has One Current Value | Verdadero cuando el property vinculado tiene un one current value. | _Order 1. Whether the property keeps one current value per person._ |
| Property Domain Class | Tomado del property vinculado. | _Order 1. The property's domain class._ |
| Property Range Class | Tomado del property vinculado. | _Order 1. The property's range class._ |
| Literal Quote | Tomado del graph vinculado. | _Order 1. The graph's literal quote mark._ |
| Literal Type Marker | Tomado del graph vinculado. | _Order 1. The graph's datatype marker._ |
| Built in Source Text | Tomado del graph vinculado. | _Order 1. The graph's source line for a teaching fact._ |
| User Source Text | Tomado del graph vinculado. | _Order 1. The graph's source line for a reader's fact._ |
| Next Fact Sort Order | Tomado del graph vinculado. | _Order 6. List position of the next fact to add._ |
| Is Literal | Verdadero cuando el object thing está vacío. | _Order 1. True when the object is a value rather than a thing._ |
| Object Text | Determinado por prioridad: el object value si el object thing está vacío; en todos los demás casos, el object label. | _Order 2. The object's name or the value._ |
| Name | Calculado como el subject label, seguido de un espacio, seguido de el property label, seguido de un espacio, seguido de el object text. | _Order 3. The fact in words._ |
| Words | Calculado como el subject label, seguido de un espacio, seguido de el property label, seguido de un espacio, seguido de el literal quote, seguido de el object value, seguido de el literal quote, seguido de el literal type marker, seguido de el property datatype si el object thing está vacío, en todos los demás casos el object label. ⚠︎ mecánica <!-- rulespeak:reword --> | _Order 2. The fact as a conclusion card quotes it, with a literal written in Turtle form._ |
| Source | Determinado por prioridad: el built in source text si el indicador «built in» está activado; en todos los demás casos, el user source text. | _Order 2. Where a listed fact came from._ |
| Subject Property Key | Calculado como el subject, seguido de «|», seguido de el property. | _Order 1. Groups the values one subject has for one property._ |
| Latest Sibling Activation Seq | El mayor valor de activation seq entre los statements relacionados con el statement. | _Order 2. The most recent activation among this subject's values for this property._ |
| Is in Effect | Verdadero cuando se cumple todo lo siguiente: el indicador «stated» está activado y se cumple al menos una de las siguientes: el indicador «property has one current value» no está activado o el activation seq es el latest sibling activation seq. | _Order 3. Switched on and, for a one-value property, the most recently switched-on value. Drawn as a solid line._ |
| Listed Unit | Determinado por prioridad: 1 si el indicador «listed» está activado; en todos los demás casos, 0. | _Order 1. 1 when the fact is in the fact list._ |
| In Effect Unit | Determinado por prioridad: 1 si el indicador «in effect» está activado; en todos los demás casos, 0. | _Order 4. 1 when the fact is in effect._ |
| Listed Off Unit | Determinado por prioridad: 1 si se cumple todo lo siguiente: el indicador «listed» está activado y el indicador «in effect» no está activado; en todos los demás casos, 0. | _Order 4. 1 when the fact is listed but not in effect._ |
| Is Next to Add | Verdadero cuando se cumple todo lo siguiente: el listed off unit es 1 y el sort order es el next fact sort order. | _Order 7. The fact the Add next fact button switches on._ |
| Count of Inverse Firings | El total de inverse fired unit entre los firings relacionados con el statement. | _Order 6. Inverse rule applications that conclude this fact._ |
| Count of Symmetric Firings | El total de symmetric fired unit entre los firings relacionados con el statement. | _Order 6. Symmetric rule applications that conclude this fact._ |
| Holds Before Chain | Verdadero cuando el IF de el OR de el is in effect; el count of inverse firings es mayor que 0; y el count of symmetric firings es mayor que 0 y verdadero. | _Order 7. TRUE when the fact is in effect or the inverse or symmetric rule concludes it. Blank when nothing states or concludes it: not known, never FALSE (open world: removing a statement does not say that its opposite is true)._ |
| Count of Chain Firings | El total de chain fired unit entre los firings relacionados con el statement. | _Order 10. Property chain applications that conclude this fact._ |
| Holds Before Typing | Verdadero cuando el IF de el OR de el holds before chain y el count of chain firings es mayor que 0 y verdadero. | _Order 11. TRUE when the fact holds before the domain and range rules apply. Blank when nothing states or concludes it: not known, never FALSE (open world: removing a statement does not say that its opposite is true)._ |
| Count of Type Firings | El total de type fired unit entre los firings relacionados con el statement. | _Order 14. Domain and range applications that conclude this fact._ |
| Holds | Verdadero cuando el IF de el OR de el holds before typing y el count of type firings es mayor que 0 y verdadero. | _Order 15. TRUE when in effect or concluded by an enabled rule; the graph draws it. Blank when nothing states or concludes it: not known, never FALSE (open world: removing a statement does not say that its opposite is true)._ |
| Is Conclusion | Verdadero cuando el IF de el AND de el holds y el indicador «in effect» no está activado y verdadero. | _Order 16. TRUE when the fact holds only because a rule concluded it; drawn dashed. Blank otherwise: a stated fact is not a conclusion, and a fact nothing concludes is not known (never FALSE)._ |
| Holds Unit | Determinado por prioridad: 1 si el indicador «holds» está activado; en todos los demás casos, 0. | _Order 16. 1 when the fact holds._ |
| Conclusion Unit | Determinado por prioridad: 1 si el indicador «conclusion» está activado; en todos los demás casos, 0. | _Order 17. 1 when the fact is a conclusion._ |
| Parent Unit | Determinado por prioridad: 1 si se cumple todo lo siguiente: el indicador «holds» está activado y el property es «isParentOf»; en todos los demás casos, 0. | _Order 16. 1 for a holding parent link._ |
| Grandparent Unit | Determinado por prioridad: 1 si se cumple todo lo siguiente: el indicador «holds» está activado y el property es «isGrandparentOf»; en todos los demás casos, 0. | _Order 16. 1 for a holding grandparent link._ |
| Place Unit | Determinado por prioridad: 1 si se cumple todo lo siguiente: el indicador «holds» está activado y el property es «attends»; en todos los demás casos, 0. | _Order 16. 1 for a holding attends link._ |
| First Pre Chain Order Key | El menor valor de pre chain order key entre los firings del statement que son pre chain fired unit. | _Order 7. Order key of the first inverse or symmetric application that concludes this fact._ |
| Snap Position | Determinado por prioridad: el sort order si el indicador «in effect» está activado; 100000 más el first pre chain order key si el indicador «holds before chain» está activado; en todos los demás casos, 0. | _Order 8. Where the page's reasoner meets this fact as a premise: a stated fact at its list position, a fact concluded by the inverse or symmetric rule after every stated fact, in the order it was concluded; 0 for a fact that does not hold yet, so a link to no fact reads the same on every engine._ |
| First Fired Order Key | El menor valor de order key entre los firings del statement que son fired unit. | _Order 15. Order key of the first rule application that concludes this fact, in the page reasoner's order._ |
| Count of Chosen Firings | El total de chosen unit entre los firings relacionados con el statement. | _Order 20. Rule applications shown as this fact's conclusion card: one for a conclusion, zero otherwise._ |
| Subject Node Key | Igual a subject. | _Order 1. The graph node this fact starts from._ |
| Object Node Key | Determinado por prioridad: «literal|», seguido de el object value, seguido de «|», seguido de el property datatype si el object thing está vacío; en todos los demás casos, el object thing. | _Order 2. The graph node this fact points at: the thing, or one node per value and datatype._ |
| Subject Node Kind | Tomado del subject vinculado. | _Order 19. How the graph draws the subject._ |
| Object Thing Node Kind | Tomado del object thing vinculado. | _Order 19. How the graph draws the object thing._ |
| Object Node Kind | Determinado por prioridad: «value» si el object thing está vacío; en todos los demás casos, el object thing node kind. | _Order 20. How the graph draws the object: a value, a class, a place or a person._ |
| Object Value Node Label | Tomado del property vinculado. | _Order 1. The small label under a value node._ |
| Count of Violations As First | El total de violation unit entre los disjoint checks relacionados con el statement. | _Order 18. Violated disjointness checks naming this fact first._ |
| Count of Violations As Second | El total de violation unit entre los disjoint checks relacionados con el statement. | _Order 18. Violated disjointness checks naming this fact second._ |
| Is in Contradiction | Verdadero cuando se cumple al menos una de las siguientes: el count of violations as first es mayor que 0 o el count of violations as second es mayor que 0. | _Order 19. Part of a contradiction the disjoint rule found; drawn in the alarm colour._ |

<details open>
<summary>Datos de ejemplo</summary>

| Object Value | Is Listed | Is Built in | Is Detail | Is Stated | Activation Seq | Sort Order | Subject Label ƒ | Property Label ƒ | Object Label ƒ | Property Datatype ƒ | Property Has One Current Value ƒ | Property Domain Class ƒ | Property Range Class ƒ | Literal Quote ƒ | Literal Type Marker ƒ | Built in Source Text ƒ | User Source Text ƒ | Next Fact Sort Order ƒ | Is Literal ƒ | Object Text ƒ | Name ƒ | Words ƒ | Source ƒ | Subject Property Key ƒ | Latest Sibling Activation Seq ƒ | Is in Effect ƒ | Listed Unit ƒ | In Effect Unit ƒ | Listed Off Unit ƒ | Is Next to Add ƒ | Count of Inverse Firings ƒ | Count of Symmetric Firings ƒ | Holds Before Chain ƒ | Count of Chain Firings ƒ | Holds Before Typing ƒ | Count of Type Firings ƒ | Holds ƒ | Is Conclusion ƒ | Holds Unit ƒ | Conclusion Unit ƒ | Parent Unit ƒ | Grandparent Unit ƒ | Place Unit ƒ | First Pre Chain Order Key ƒ | Snap Position ƒ | First Fired Order Key ƒ | Count of Chosen Firings ƒ | Subject Node Key ƒ | Object Node Key ƒ | Subject Node Kind ƒ | Object Thing Node Kind ƒ | Object Node Kind ƒ | Object Value Node Label ƒ | Count of Violations As First ƒ | Count of Violations As Second ƒ | Is in Contradiction ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| — | true | true | false | true | 1 | 1 | Marvin | is parent of | Randy | — | false | — | — | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | 0 | false | Randy | Marvin is parent of Randy | Marvin is parent of Randy | Stated as a teaching example on this page. | Marvin\|isParentOf | 1 | true | 1 | 1 | 0 | false | 0 | 0 | true | 0 | true | 0 | true | — | 1 | 0 | 1 | 0 | 0 | 0.0 | 1.0 | 0.0 | 0 | Marvin | Randy | person | person | person | — | 0 | 0 | false |
| — | false | false | false | false | 0 | 100001 | Marvin | is parent of | Sharon | — | false | — | — | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | 0 | false | Sharon | Marvin is parent of Sharon | Marvin is parent of Sharon | Stated by you on this page. | Marvin\|isParentOf | 1 | false | 0 | 0 | 0 | false | 0 | 0 | — | 0 | — | 0 | — | — | 0 | 0 | 0 | 0 | 0 | 0.0 | 0.0 | 0.0 | 0 | Marvin | Sharon | person | person | person | — | 0 | 0 | false |
| — | false | false | false | false | 0 | 100002 | Marvin | is parent of | Stan | — | false | — | — | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | 0 | false | Stan | Marvin is parent of Stan | Marvin is parent of Stan | Stated by you on this page. | Marvin\|isParentOf | 1 | false | 0 | 0 | 0 | false | 0 | 0 | — | 0 | — | 0 | — | — | 0 | 0 | 0 | 0 | 0 | 0.0 | 0.0 | 0.0 | 0 | Marvin | Stan | person | person | person | — | 0 | 0 | false |

_ƒ indica una columna calculada._

</details>

| Término | Descripción | Comentario narrativo |
|------|-------------|-------------------|
| **Firing** | Every way an enabled rule could conclude a candidate fact from the facts that hold. | — |
| Kind | Un atributo definido. | _How the rule applies._ |
| Rule | Un atributo definido. | _The rule switch._ |
| Target | Un atributo definido. | _The fact this application concludes._ |
| First | Un atributo definido. | _The premise._ |
| Second | Un atributo definido. | _The second premise, for the property chain._ |
| Rule Enabled | Verdadero cuando el rule vinculado es enabled. | _Order 1. Whether the rule is switched on._ |
| Kind Wording | Tomado del kind vinculado. | _Order 1. How the card names the rule._ |
| Kind Sort Order | Tomado del kind vinculado. | _Order 1. The kind's place in the reasoner's order._ |
| Target Name | Tomado del target vinculado. | _Order 4. The concluded fact in words._ |
| Target Subject Label | Tomado del target vinculado. | _Order 2. The concluded fact's subject._ |
| Target Property Label | Tomado del target vinculado. | _Order 2. The concluded fact's property._ |
| Target Object Text | Tomado del target vinculado. | _Order 3. The concluded fact's object._ |
| Target is Conclusion | Verdadero cuando el target vinculado es un conclusion. | _Order 17. Whether the concluded fact is new._ |
| Target Subject Node Key | Tomado del target vinculado. | _Order 2. The concluded fact's subject node._ |
| Target Object Node Key | Tomado del target vinculado. | _Order 3. The concluded fact's object node._ |
| Target First Fired Order Key | Tomado del target vinculado. | _Order 16. The order key of the concluded fact's first application._ |
| First Words | Tomado del first vinculado. | _Order 3. The premise as the card quotes it._ |
| Second Words | Tomado del second vinculado. | _Order 3. The second premise as the card quotes it._ |
| First is in Effect | Verdadero cuando el first vinculado es in effect. | _Order 4. Whether the premise is in effect._ |
| First Holds Before Chain | Verdadero cuando el first vinculado es holds before chain. | _Order 8. Whether the premise holds before the chain._ |
| Second Holds Before Chain | Verdadero cuando el second vinculado es holds before chain. | _Order 8. Whether the second premise holds before the chain._ |
| First Holds Before Typing | Verdadero cuando el first vinculado es holds before typing. | _Order 12. Whether the premise holds before typing._ |
| First Property | Tomado del first vinculado. | _Order 1. The premise's property key, as the card's axiom names it._ |
| First Property Domain Class | Tomado del first vinculado. | _Order 2. The premise property's domain class._ |
| First Property Range Class | Tomado del first vinculado. | _Order 2. The premise property's range class._ |
| First Sort Order | Tomado del first vinculado. | _Order 1. The premise's list position._ |
| Second is in Effect | Verdadero cuando el second vinculado es in effect. | _Order 4. Whether the second premise is in effect._ |
| First Snap Position | Tomado del first vinculado. | _Order 9. Where the reasoner meets the premise._ |
| Second Snap Position | Tomado del second vinculado. | _Order 9. Where the reasoner meets the second premise._ |
| Inverse Fired Unit | Determinado por prioridad: 1 si se cumple todo lo siguiente: el indicador «rule enabled» está activado; el kind es «inverse»; y el indicador «first is in effect» está activado; en todos los demás casos, 0. | _Order 5. 1 when the inverse rule fires here._ |
| Symmetric Fired Unit | Determinado por prioridad: 1 si se cumple todo lo siguiente: el indicador «rule enabled» está activado; el kind es «symmetric»; y el indicador «first is in effect» está activado; en todos los demás casos, 0. | _Order 5. 1 when the symmetric rule fires here._ |
| Chain Fired Unit | Determinado por prioridad: 1 si se cumple todo lo siguiente: el indicador «rule enabled» está activado; el kind es «chain»; el indicador «first holds before chain» está activado; y el indicador «second holds before chain» está activado; en todos los demás casos, 0. | _Order 9. 1 when the property chain fires here._ |
| Type Fired Unit | Determinado por prioridad: 1 si se cumple todo lo siguiente: el indicador «rule enabled» está activado; se cumple al menos una de las siguientes: el kind es «domain» o el kind es «range»; y el indicador «first holds before typing» está activado; en todos los demás casos, 0. | _Order 13. 1 when the domain or range rule fires here._ |
| Fired Unit | Calculado como el inverse fired unit más el symmetric fired unit más el chain fired unit más el type fired unit. | _Order 14. 1 when this application fires._ |
| Pre Chain Fired Unit | Calculado como el inverse fired unit más el symmetric fired unit. | _Order 6. 1 when this is an inverse or symmetric application that fires._ |
| Pre Chain Order Key | Calculado como el first sort order por 10 más el kind sort order. | _Order 2. For an inverse or symmetric application: its place in the reasoner's first round._ |
| Round | Determinado por prioridad: 2 si se cumple todo lo siguiente: el kind es «chain» y no se da el caso de que se cumple todo lo siguiente: el indicador «first is in effect» está activado y el indicador «second is in effect» está activado; en todos los demás casos, 1. | _Order 5. The reasoner's round: 1 when every premise is a stated fact, 2 when the property chain reads a concluded fact._ |
| Order Key | Calculado como el round por 1000000000000000 más el first snap position por 100000000 más el kind sort order por 10000000 más 0 si el second está vacío, en todos los demás casos el second snap position. | _Order 10. The page reasoner's order: by round, then premise, then rule kind, then second premise._ |
| Is Chosen | Verdadero cuando se cumple todo lo siguiente: el fired unit es 1; el indicador «target is conclusion» está activado; y el order key es el target first fired order key. | _Order 18. The application the conclusions panel shows for its fact: the first one in the reasoner's order._ |
| Chosen Unit | Determinado por prioridad: 1 si el indicador «chosen» está activado; en todos los demás casos, 0. | _Order 19. 1 when this application is shown._ |
| Card Text | Determinado por prioridad: «Worked out by the », seguido de el kind wording, seguido de « from “», seguido de el first words, seguido de «”», seguido de « and “», seguido de el second words, seguido de «”» si el kind es «chain», en todos los demás casos « and “», seguido de el first property, seguido de « has domain », seguido de el first property domain class, seguido de «”» si el kind es «domain», en todos los demás casos « and “», seguido de el first property, seguido de « has range », seguido de el first property range class, seguido de «”», seguido de un punto si el indicador «chosen» está activado; en todos los demás casos, una cadena vacía. ⚠︎ mecánica <!-- rulespeak:reword --> | _Order 19. The conclusion card's provenance line, in the page's wording._ |
| Name | Igual a firing ID. | _Order 1. The application's key._ |

<details open>
<summary>Datos de ejemplo</summary>

| Rule Enabled ƒ | Kind Wording ƒ | Kind Sort Order ƒ | Target Name ƒ | Target Subject Label ƒ | Target Property Label ƒ | Target Object Text ƒ | Target is Conclusion ƒ | Target Subject Node Key ƒ | Target Object Node Key ƒ | Target First Fired Order Key ƒ | First Words ƒ | Second Words ƒ | First is in Effect ƒ | First Holds Before Chain ƒ | Second Holds Before Chain ƒ | First Holds Before Typing ƒ | First Property ƒ | First Property Domain Class ƒ | First Property Range Class ƒ | First Sort Order ƒ | Second is in Effect ƒ | First Snap Position ƒ | Second Snap Position ƒ | Inverse Fired Unit ƒ | Symmetric Fired Unit ƒ | Chain Fired Unit ƒ | Type Fired Unit ƒ | Fired Unit ƒ | Pre Chain Fired Unit ƒ | Pre Chain Order Key ƒ | Round ƒ | Order Key ƒ | Is Chosen ƒ | Chosen Unit ƒ | Card Text ƒ | Name ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| false | inverse rule | 1 | Randy is child of Marvin | Randy | is child of | Marvin | — | Randy | Marvin | 0.0 | Marvin is parent of Randy |    | true | true | — | true | isParentOf | — | — | 1 | false | 1.0 | 0.0 | 0 | 0 | 0 | 0 | 0 | 0 | 11.0 | 1 | 1000000110000000.0 | false | 0 | — | inverse:Marvin\|isParentOf\|Randy>Randy\|isChildOf\|Marvin |
| false | inverse rule | 1 | Sharon is child of Marvin | Sharon | is child of | Marvin | — | Sharon | Marvin | 0.0 | Marvin is parent of Sharon |    | false | — | — | — | isParentOf | — | — | 100001 | false | 0.0 | 0.0 | 0 | 0 | 0 | 0 | 0 | 0 | 1000011.0 | 1 | 1000000010000000.0 | false | 0 | — | inverse:Marvin\|isParentOf\|Sharon>Sharon\|isChildOf\|Marvin |
| false | inverse rule | 1 | Stan is child of Marvin | Stan | is child of | Marvin | — | Stan | Marvin | 0.0 | Marvin is parent of Stan |    | false | — | — | — | isParentOf | — | — | 100002 | false | 0.0 | 0.0 | 0 | 0 | 0 | 0 | 0 | 0 | 1000021.0 | 1 | 1000000010000000.0 | false | 0 | — | inverse:Marvin\|isParentOf\|Stan>Stan\|isChildOf\|Marvin |

_ƒ indica una columna calculada._

</details>

| Término | Descripción | Comentario narrativo |
|------|-------------|-------------------|
| **Disjoint Check** | Every pair the disjointness rule watches: the same two people linked by two disjoint properties. | — |
| Graph | Un atributo definido. | _The graph._ |
| Rule | Un atributo definido. | _The rule switch._ |
| First | Un atributo definido. | _The first link._ |
| Second | Un atributo definido. | _The disjoint link between the same pair, in the same direction._ |
| Rule Enabled | Verdadero cuando el rule vinculado es enabled. | _Order 1. Whether the disjointness rule is on._ |
| First Holds | Verdadero cuando el first vinculado es holds. | _Order 16. Whether the first link holds._ |
| Second Holds | Verdadero cuando el second vinculado es holds. | _Order 16. Whether the second link holds._ |
| Subject Label | Tomado del first vinculado. | _Order 2. The person recorded with both links._ |
| Object Label | Tomado del first vinculado. | _Order 2. The other person._ |
| Violation Unit | Determinado por prioridad: 1 si se cumple todo lo siguiente: el indicador «rule enabled» está activado; el indicador «first holds» está activado; y el indicador «second holds» está activado; en todos los demás casos, 0. | _Order 17. 1 when both links hold._ |
| Alarm Text | Determinado por prioridad: «Contradiction found. », seguido de el subject label, seguido de « is recorded as both parent and spouse of », seguido de el object label, seguido de «. Check those facts and the selected disjoint rule.» si el violation unit es 1; en todos los demás casos, una cadena vacía. | _Order 18. The page's contradiction alert, blank when there is none._ |
| Name | Igual a disjoint check ID. | _Order 1. The check's key._ |

<details open>
<summary>Datos de ejemplo</summary>

| Rule Enabled ƒ | First Holds ƒ | Second Holds ƒ | Subject Label ƒ | Object Label ƒ | Violation Unit ƒ | Alarm Text ƒ | Name ƒ |
|---|---|---|---|---|---|---|---|
| false | true | — | Marvin | Randy | 0 | — | Marvin\|isParentOf\|Randy~isSpouseOf |
| false | — | — | Marvin | Sharon | 0 | — | Marvin\|isParentOf\|Sharon~isSpouseOf |
| false | — | — | Marvin | Stan | 0 | — | Marvin\|isParentOf\|Stan~isSpouseOf |

_ƒ indica una columna calculada._

</details>

| Término | Descripción | Comentario narrativo |
|------|-------------|-------------------|
| **Scenario** | The page's buttons. Each one is a fact baseline, explicit fact changes and rule settings, applied in that order. | — |
| Graph | Un atributo definido. | _The graph._ |
| Label | Un atributo definido. | _The button's text._ |
| Note | Un atributo definido. | _The note shown under the buttons after this one is pressed; blank clears it._ |
| Sort Order | Un atributo definido. | _Position among the buttons._ |
| Button Group | Un atributo definido. | _'stan' for the two imagined changes, 'facts' for the buttons under the fact list._ |
| Is Primary | Verdadero cuando una cadena vacía. | _Drawn as the filled button._ |
| Fact Baseline | Un atributo definido. | _What happens to every listed fact first: keep, built-in (teaching facts on, others off), built-in-detail, all-off, activate-all (each switched on in list order), or next (the next fact on)._ |
| Layout | Un atributo definido. | _The node layout the graph returns to; blank keeps the current positions._ |
| Clears Selection | Verdadero cuando una cadena vacía. | _Clears the selected node or line._ |
| Clears Form Message | Verdadero cuando una cadena vacía. | _Clears the Add fact form's message._ |
| Name | Igual a label. | _Order 1. The button's text._ |
| Graph Has Facts to Add | Verdadero cuando el graph vinculado tiene un facts to add. | _Order 7. Whether a listed fact can still be added._ |
| Is Available | Verdadero cuando se cumple al menos una de las siguientes: el fact baseline no es «next» o el indicador «graph has facts to add» está activado. | _Order 8. False only for Add next fact once every listed fact is in effect._ |

<details open>
<summary>Datos de ejemplo</summary>

| Label | Note | Sort Order | Button Group | Is Primary | Fact Baseline | Layout | Clears Selection | Clears Form Message | Name ƒ | Graph Has Facts to Add ƒ | Is Available ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|
| Move Stan to grade 5 | This imagined change updates Stan’s grade to 5. His identifier, Person statement and family links stay the same. His other selected details are unchanged. | 1 | stan | false | keep | — | true | true | Move Stan to grade 5 | false | true |
| Stan leaves school | This imagined change clears Stan’s current school details. His identifier, Person statement and family links remain. Removing a current fact does not say that its opposite is true; school history needs dates. | 2 | stan | false | keep | — | true | true | Stan leaves school | false | true |
| Add next fact | — | 3 | facts | true | next | — | false | false | Add next fact | false | false |

_ƒ indica una columna calculada._

</details>

| Término | Descripción | Comentario narrativo |
|------|-------------|-------------------|
| **Scenario Fact Change** | Fact changes a button makes after its baseline. A blank object key matches every listed value of the property. | — |
| Scenario | Un atributo definido. | _The button._ |
| Subject | Un atributo definido. | _The subject whose facts change._ |
| Property | Un atributo definido. | _The property whose facts change._ |
| Object Key | Un atributo definido. | _The object thing's key or the literal value; blank for every listed value._ |
| Action | Un atributo definido. | _'on' switches the fact on as the newest value; 'off' switches it off._ |
| Adds to List | Verdadero cuando una cadena vacía. | _Adds the fact to the fact list first when it is not listed._ |
| Sort Order | Un atributo definido. | _Order of the change within the button._ |
| Name | Igual a scenario fact change ID. | _Order 1. The change's key._ |

<details open>
<summary>Datos de ejemplo</summary>

| Object Key | Action | Adds to List | Sort Order | Name ƒ |
|---|---|---|---|---|
| Person | on | true | 1 | stan-grade-five:Stan\|isA\|Person |
| 5 | on | true | 2 | stan-grade-five:Stan\|hasGradeLevel\|5 |
| Person | on | true | 3 | stan-leaves-school:Stan\|isA\|Person |

_ƒ indica una columna calculada._

</details>

| Término | Descripción | Comentario narrativo |
|------|-------------|-------------------|
| **Scenario Rule Setting** | Rule switches a button sets. | — |
| Scenario | Un atributo definido. | _The button._ |
| Rule | Un atributo definido. | _The rule switch._ |
| Is Enabled | Verdadero cuando una cadena vacía. | _The state the button puts the switch in._ |
| Name | Igual a scenario rule setting ID. | _Order 1. The setting's key._ |

<details open>
<summary>Datos de ejemplo</summary>

| Is Enabled | Name ƒ |
|---|---|
| false | reset-original:inverse |
| false | reset-original:symmetric |
| false | reset-original:chain |

_ƒ indica una columna calculada._

</details>

| Término | Descripción | Comentario narrativo |
|------|-------------|-------------------|
| **Node Layout** | Node positions in the graph's 380 by 480 drawing, one set per layout. | — |
| Layout | Un atributo definido. | _The layout: 'default' or 'stan-details'._ |
| Node Key | Un atributo definido. | _The node: a thing's key, or literal\|value\|datatype._ |
| X | Un atributo definido. | _Horizontal position._ |
| Y | Un atributo definido. | _Vertical position._ |
| Name | Igual a node layout ID. | _Order 1. The position's key._ |

<details open>
<summary>Datos de ejemplo</summary>

| Layout | Node Key | X | Y | Name ƒ |
|---|---|---|---|---|
| default | Marvin | 70 | 45 | default:Marvin |
| default | Randy | 195 | 125 | default:Randy |
| default | Sharon | 318 | 55 | default:Sharon |

_ƒ indica una columna calculada._

</details>

| Término | Descripción | Comentario narrativo |
|------|-------------|-------------------|
| **Layout Slot** | Places for nodes no layout positions, used in turn. | — |
| Sort Order | Un atributo definido. | _Order in which slots are used._ |
| X | Un atributo definido. | _Horizontal position._ |
| Y | Un atributo definido. | _Vertical position._ |
| Name | Igual a layout slot ID. | _Order 1. The slot's key._ |

<details open>
<summary>Datos de ejemplo</summary>

| Sort Order | X | Y | Name ƒ |
|---|---|---|---|
| 1 | 330 | 135 | slot-1 |
| 2 | 45 | 120 | slot-2 |
| 3 | 330 | 400 | slot-3 |

_ƒ indica una columna calculada._

</details>

| Término | Descripción | Comentario narrativo |
|------|-------------|-------------------|
| **Page Text** | The page's wording, one row per string. {name}-style placeholders are filled by the page. | — |
| Text | Un atributo definido. | _The string._ |
| Name | Igual a page text ID. | _Order 1. The string's key._ |

<details open>
<summary>Datos de ejemplo</summary>

| Text | Name ƒ |
|---|---|
| South Park Ontology Explainer, on one rulebook | page-title |
| A rulebook rebuild of the explainer by | byline-prefix |
| Jessica Talisman | byline-author |

_ƒ indica una columna calculada._

</details>

| Término | Descripción | Comentario narrativo |
|------|-------------|-------------------|
| **Engine** | The execution engines the page can be computed by. Each is a build output of this rulebook. | — |
| Label | Un atributo definido. | _The engine's name in the switcher._ |
| Substrate | Un atributo definido. | _The conformance harness's name for this substrate._ |
| Is Default | Verdadero cuando una cadena vacía. | _The engine a new visitor starts on._ |
| Description | Un atributo definido. | _What computes the page on this engine._ |
| Sort Order | Un atributo definido. | _Position in the switcher._ |
| Name | Igual a label. | _Order 1. The engine's name._ |

<details open>
<summary>Datos de ejemplo</summary>

| Label | Substrate | Is Default | Description | Sort Order | Name ƒ |
|---|---|---|---|---|---|
| Postgres views | effortless-postgres | true | Every value is a column of a generated view; the database computes it when the page reads it. | 1 | Postgres views |
| TypeScript | effortless-typescript | false | The generated TypeScript computes every field from the same rows, with no database. | 2 | TypeScript |
| OWL and SHACL rules | effortless-owl | false | The generated ontology and SHACL rules, run over the same rows by a SPARQL engine. | 3 | OWL and SHACL rules |

_ƒ indica una columna calculada._

</details>

| Término | Descripción | Comentario narrativo |
|------|-------------|-------------------|
| **Rulebook Tool** | What each rulebook tool adds to the original explainer; the tools panel lists them. | — |
| Label | Un atributo definido. | _The tool's name._ |
| Adds | Un atributo definido. | _What it adds to the original explainer._ |
| Href | Un atributo definido. | _Where its link goes; 'editor' means the running rulebook editor._ |
| Link Text | Un atributo definido. | _The link's text._ |
| Sort Order | Un atributo definido. | _Position in the panel._ |
| Name | Igual a label. | _Order 1. The tool's name._ |

<details open>
<summary>Datos de ejemplo</summary>

| Label | Adds | Href | Link Text | Sort Order | Name ƒ |
|---|---|---|---|---|---|
| Interchangeable engines | The same page, computed by Postgres views, generated TypeScript or the generated OWL and SHACL rules. Switch with the control above; nothing else changes. | — | — | 1 | Interchangeable engines |
| Conformance | Every engine's answers for the current facts, compared value by value with the conformance harness's own rule for when two values are equal. | — | — | 2 | Conformance |
| Explainer | Hover a dotted value for its rule in plain words and its witness: the values it came from, down to the stated facts. | /rulebook-explainer-dag/pages/index.html | Open the explainer | 3 | Explainer |

_ƒ indica una columna calculada._

</details>

## 2 Tipos de hechos

- un **firing kind** referencia exactamente un **rule**
- un **thing** puede referenciar un **portrait**
- un **portrait aliase** referencia exactamente un **portrait**
- un **property** puede referenciar un **thing**
- un **property axiom** referencia exactamente un **firing kind**
- un **property axiom** referencia exactamente un **property**
- un **statement** referencia exactamente un **graph**
- un **statement** referencia exactamente un **thing**
- un **statement** referencia exactamente un **property**
- un **firing** referencia exactamente un **firing kind**
- un **firing** referencia exactamente un **rule**
- un **firing** referencia exactamente un **statement**
- un **disjoint check** referencia exactamente un **graph**
- un **disjoint check** referencia exactamente un **rule**
- un **disjoint check** referencia exactamente un **statement**
- un **scenario** referencia exactamente un **graph**
- un **scenario fact change** referencia exactamente un **scenario**
- un **scenario fact change** referencia exactamente un **thing**
- un **scenario fact change** referencia exactamente un **property**
- un **scenario rule setting** referencia exactamente un **scenario**
- un **scenario rule setting** referencia exactamente un **rule**

## 3 Reglas operativas

_Las reglas operativas enuncian lo que el negocio **obliga**, **prohíbe** o
aconseja (**debería**). Las reglas estructurales provienen de campos obligatorios y claves
foráneas; las semánticas provienen de la tabla Constraints, cada una apoyada en un booleano
que el libro de reglas ya calcula (referenciado como DR-N en las reglas definitorias)._

### Restricciones estructurales (del esquema)

- Un graph **debe** tener un label; un literal quote; un literal type marker; un built in source text; un user source text; y un no conclusions text.
- Un rule **debe** tener un label; un explanation; y un sort order, y registrar si es enabled.
- Un firing kind **debe** referenciar exactamente un rule.
- Un firing kind **debe** tener un wording y un sort order.
- Un thing **debe** tener un label; un kind; y un sort order.
- Un portrait **debe** tener un display name; un page; un image; un crop; un width; un height; y un sort order.
- Un portrait aliase **debe** referenciar exactamente un portrait.
- Un property **debe** tener un label; un property kind; y un sort order, y registrar si es user addable; si tiene un one current value; y si es symmetric.
- Un property axiom **debe** referenciar exactamente un firing kind como su kind.
- Un property axiom **debe** referenciar exactamente un property.
- Un property axiom **debe** referenciar exactamente un property como su first.
- Un statement **debe** referenciar exactamente un graph.
- Un statement **debe** referenciar exactamente un thing como su subject.
- Un statement **debe** referenciar exactamente un property.
- Un statement **debe** tener un activation seq y un sort order, y registrar si es listed; si es un built in; si es un detail; y si es stated.
- Un firing **debe** referenciar exactamente un firing kind como su kind.
- Un firing **debe** referenciar exactamente un rule.
- Un firing **debe** referenciar exactamente un statement como su target.
- Un firing **debe** referenciar exactamente un statement como su first.
- Un disjoint check **debe** referenciar exactamente un graph.
- Un disjoint check **debe** referenciar exactamente un rule.
- Un disjoint check **debe** referenciar exactamente un statement como su first.
- Un disjoint check **debe** referenciar exactamente un statement como su second.
- Un scenario **debe** referenciar exactamente un graph.
- Un scenario **debe** tener un label; un sort order; un button group; y un fact baseline, y registrar si es primary; si es clears selection; y si es clears form message.
- Un scenario fact change **debe** referenciar exactamente un scenario.
- Un scenario fact change **debe** referenciar exactamente un thing como su subject.
- Un scenario fact change **debe** referenciar exactamente un property.
- Un scenario fact change **debe** tener un action y un sort order, y registrar si es adds to list.
- Un scenario rule setting **debe** referenciar exactamente un scenario.
- Un scenario rule setting **debe** referenciar exactamente un rule.
- Un scenario rule setting **debe** registrar si es enabled.
- Un node layout **debe** tener un layout; un node key; un x; y un y.
- Un layout slot **debe** tener un sort order; un x; y un y.
- Un page text **debe** tener un text.
- Un engine **debe** tener un label; un substrate; un description; y un sort order, y registrar si es un default.
- Un rulebook tool **debe** tener un label; un adds; y un sort order.

## 4 Reglas definitorias

_Todos los enunciados expresan verdades del dominio de negocio; no son procedimientos
ni imperativos. Se evita «si y solo si» en favor de «solo si» para que una necesidad
unidireccional no se confunda con una equivalencia. Una marca
**⚠︎ mecánica** señala una regla cuya redacción determinista es fiel pero
tosca — una señal para una reescritura opcional, no un defecto._

| ID | Regla declarativa |
|----|------------------|
| **DR-1 Name** | Un graph: name es igual a label. |
| **DR-2 Count of Stated Facts** | Un graph: count of stated facts es el total de in effect unit entre los statements relacionados con el graph. |
| **DR-3 Count of Conclusions** | Un graph: count of conclusions es el total de conclusion unit entre los statements relacionados con el graph. |
| **DR-4 Count of Violations** | Un graph: count of violations es el total de violation unit entre los disjoint checks relacionados con el graph. |
| **DR-5 Is Consistent** | Un graph se considera consistent si el count of violations es 0. |
| **DR-6 Conclusions Message** | El campo «conclusions message» del graph se determina según la siguiente prioridad:<br>1. el no conclusions text, si el count of conclusions es 0;<br>2. en todos los demás casos, una cadena vacía. |
| **DR-7 Count of Facts to Add** | Un graph: count of facts to add es el total de listed off unit entre los statements relacionados con el graph. |
| **DR-8 Has Facts to Add** | Un graph se considera que tiene un facts to add si el count of facts to add es mayor que 0. |
| **DR-9 Next Fact Sort Order** | Un graph: next fact sort order es el menor valor de sort order entre los statements del graph que son listed off unit. |
| **DR-10 Max Activation Seq** | Un graph: max activation seq es el mayor valor de activation seq entre los statements relacionados con el graph. |
| **DR-11 Next Activation Seq** | Un graph: next activation seq se calcula como el max activation seq más 1. |
| **DR-12 Max Listed Sort Order** | Un graph: max listed sort order es el mayor valor de sort order entre los statements del graph que son listed unit. |
| **DR-13 Next Listed Sort Order** | Un graph: next listed sort order se calcula como el max listed sort order más 1. |
| **DR-14 Max Sort Order** | Un graph: max sort order es el mayor valor de sort order entre los statements relacionados con el graph. |
| **DR-15 Next Candidate Sort Order** | Un graph: next candidate sort order se calcula como el max sort order más 1. |
| **DR-16 Name** | Un rule: name es igual a label. |
| **DR-17 Count of Firings** | Un rule: count of firings es el total de fired unit entre los firings relacionados con el rule. |
| **DR-18 Name** | Un firing kind: name es igual a wording. |
| **DR-19 Rule Enabled** | Un firing kind: rule enabled es verdadero cuando el rule vinculado es enabled. |
| **DR-20 Portrait Name** | Un thing: portrait name es el display name del portrait vinculado. |
| **DR-21 Portrait Page** | Un thing: portrait page — tomado del portrait vinculado. |
| **DR-22 Portrait Image** | Un thing: portrait image — tomado del portrait vinculado. |
| **DR-23 Portrait Crop** | Un thing: portrait crop — tomado del portrait vinculado. |
| **DR-24 Portrait Width** | Un thing: portrait width — tomado del portrait vinculado. |
| **DR-25 Portrait Height** | Un thing: portrait height — tomado del portrait vinculado. |
| **DR-26 Name** | Un thing: name es igual a label. |
| **DR-27 Node Key** | Un thing: node key es igual a thing ID. |
| **DR-28 Count of Children** | Un thing: count of children es el total de parent unit entre los statements relacionados con el thing. |
| **DR-29 Count of Grandchildren** | Un thing: count of grandchildren es el total de grandparent unit entre los statements relacionados con el thing. |
| **DR-30 Count As Place** | Un thing: count as place es el total de place unit entre los statements relacionados con el thing. |
| **DR-31 Count As Subject** | Un thing: count as subject es el total de holds unit entre los statements relacionados con el thing. |
| **DR-32 Count As Object** | Un thing: count as object es el total de holds unit entre los statements relacionados con el thing. |
| **DR-33 Is in Graph** | Un thing se considera in graph si se cumple al menos una de las siguientes: el count as subject es mayor que 0 o el count as object es mayor que 0. |
| **DR-34 Node Kind** | El campo «node kind» del thing se determina según la siguiente prioridad:<br>1. una cadena vacía, si el kind está vacío;<br>2. «class», si el kind es «class»;<br>3. «place», si el count as place es mayor que 0;<br>4. en todos los demás casos, «person». |
| **DR-35 Name** | Un portrait: name es igual a display name. |
| **DR-36 Name** | Un portrait aliase: name es igual a portrait alias ID. |
| **DR-37 Name** | Un property: name es igual a label. |
| **DR-38 Name** | Un property axiom: name es igual a property axiom ID. |
| **DR-39 Subject Label** | Un statement: subject label — tomado del subject vinculado. |
| **DR-40 Property Label** | Un statement: property label — tomado del property vinculado. |
| **DR-41 Object Label** | Un statement: object label — tomado del object thing vinculado. |
| **DR-42 Property Datatype** | Un statement: property datatype — tomado del property vinculado. |
| **DR-43 Property Has One Current Value** | Un statement: property has one current value es verdadero cuando el property vinculado tiene un one current value. |
| **DR-44 Property Domain Class** | Un statement: property domain class — tomado del property vinculado. |
| **DR-45 Property Range Class** | Un statement: property range class — tomado del property vinculado. |
| **DR-46 Literal Quote** | Un statement: literal quote — tomado del graph vinculado. |
| **DR-47 Literal Type Marker** | Un statement: literal type marker — tomado del graph vinculado. |
| **DR-48 Built in Source Text** | Un statement: built in source text — tomado del graph vinculado. |
| **DR-49 User Source Text** | Un statement: user source text — tomado del graph vinculado. |
| **DR-50 Next Fact Sort Order** | Un statement: next fact sort order — tomado del graph vinculado. |
| **DR-51 Is Literal** | Un statement se considera literal si el object thing está vacío. |
| **DR-52 Object Text** | El campo «object text» del statement se determina según la siguiente prioridad:<br>1. el object value, si el object thing está vacío;<br>2. en todos los demás casos, el object label. |
| **DR-53 Name** | Un statement: name se calcula como el subject label, seguido de un espacio, seguido de el property label, seguido de un espacio, seguido de el object text. |
| **DR-54 Words** | Un statement: words se calcula como el subject label, seguido de un espacio, seguido de el property label, seguido de un espacio, seguido de el literal quote, seguido de el object value, seguido de el literal quote, seguido de el literal type marker, seguido de el property datatype si el object thing está vacío, en todos los demás casos el object label. ⚠︎ mecánica <!-- rulespeak:reword --> |
| **DR-55 Source** | El campo «source» del statement se determina según la siguiente prioridad:<br>1. el built in source text, si el indicador «built in» está activado;<br>2. en todos los demás casos, el user source text. |
| **DR-56 Subject Property Key** | Un statement: subject property key se calcula como el subject, seguido de «|», seguido de el property. |
| **DR-57 Latest Sibling Activation Seq** | Un statement: latest sibling activation seq es el mayor valor de activation seq entre los statements relacionados con el statement. |
| **DR-58 Is in Effect** | Un statement se considera in effect si se cumple todo lo siguiente: el indicador «stated» está activado y se cumple al menos una de las siguientes: el indicador «property has one current value» no está activado o el activation seq es el latest sibling activation seq. |
| **DR-59 Listed Unit** | El campo «listed unit» del statement se determina según la siguiente prioridad:<br>1. 1, si el indicador «listed» está activado;<br>2. en todos los demás casos, 0. |
| **DR-60 In Effect Unit** | El campo «in effect unit» del statement se determina según la siguiente prioridad:<br>1. 1, si el indicador «in effect» está activado;<br>2. en todos los demás casos, 0. |
| **DR-61 Listed Off Unit** | El campo «listed off unit» del statement se determina según la siguiente prioridad:<br>1. 1, si se cumple todo lo siguiente: el indicador «listed» está activado y el indicador «in effect» no está activado;<br>2. en todos los demás casos, 0. |
| **DR-62 Is Next to Add** | Un statement se considera un next to add si se cumple todo lo siguiente: el listed off unit es 1 y el sort order es el next fact sort order. |
| **DR-63 Count of Inverse Firings** | Un statement: count of inverse firings es el total de inverse fired unit entre los firings relacionados con el statement. |
| **DR-64 Count of Symmetric Firings** | Un statement: count of symmetric firings es el total de symmetric fired unit entre los firings relacionados con el statement. |
| **DR-65 Holds Before Chain** | Un statement se considera que cumple: before chain si el IF de el OR de el is in effect; el count of inverse firings es mayor que 0; y el count of symmetric firings es mayor que 0 y verdadero. |
| **DR-66 Count of Chain Firings** | Un statement: count of chain firings es el total de chain fired unit entre los firings relacionados con el statement. |
| **DR-67 Holds Before Typing** | Un statement se considera que cumple: before typing si el IF de el OR de el holds before chain y el count of chain firings es mayor que 0 y verdadero. |
| **DR-68 Count of Type Firings** | Un statement: count of type firings es el total de type fired unit entre los firings relacionados con el statement. |
| **DR-69 Holds** | Un statement se considera que cumple: si el IF de el OR de el holds before typing y el count of type firings es mayor que 0 y verdadero. |
| **DR-70 Is Conclusion** | Un statement se considera un conclusion si el IF de el AND de el holds y el indicador «in effect» no está activado y verdadero. |
| **DR-71 Holds Unit** | El campo «holds unit» del statement se determina según la siguiente prioridad:<br>1. 1, si el indicador «holds» está activado;<br>2. en todos los demás casos, 0. |
| **DR-72 Conclusion Unit** | El campo «conclusion unit» del statement se determina según la siguiente prioridad:<br>1. 1, si el indicador «conclusion» está activado;<br>2. en todos los demás casos, 0. |
| **DR-73 Parent Unit** | El campo «parent unit» del statement se determina según la siguiente prioridad:<br>1. 1, si se cumple todo lo siguiente: el indicador «holds» está activado y el property es «isParentOf»;<br>2. en todos los demás casos, 0. |
| **DR-74 Grandparent Unit** | El campo «grandparent unit» del statement se determina según la siguiente prioridad:<br>1. 1, si se cumple todo lo siguiente: el indicador «holds» está activado y el property es «isGrandparentOf»;<br>2. en todos los demás casos, 0. |
| **DR-75 Place Unit** | El campo «place unit» del statement se determina según la siguiente prioridad:<br>1. 1, si se cumple todo lo siguiente: el indicador «holds» está activado y el property es «attends»;<br>2. en todos los demás casos, 0. |
| **DR-76 First Pre Chain Order Key** | Un statement: first pre chain order key es el menor valor de pre chain order key entre los firings del statement que son pre chain fired unit. |
| **DR-77 Snap Position** | El campo «snap position» del statement se determina según la siguiente prioridad:<br>1. el sort order, si el indicador «in effect» está activado;<br>2. 100000 más el first pre chain order key, si el indicador «holds before chain» está activado;<br>3. en todos los demás casos, 0. |
| **DR-78 First Fired Order Key** | Un statement: first fired order key es el menor valor de order key entre los firings del statement que son fired unit. |
| **DR-79 Count of Chosen Firings** | Un statement: count of chosen firings es el total de chosen unit entre los firings relacionados con el statement. |
| **DR-80 Subject Node Key** | Un statement: subject node key es igual a subject. |
| **DR-81 Object Node Key** | El campo «object node key» del statement se determina según la siguiente prioridad:<br>1. «literal|», seguido de el object value, seguido de «|», seguido de el property datatype, si el object thing está vacío;<br>2. en todos los demás casos, el object thing. |
| **DR-82 Subject Node Kind** | Un statement: subject node kind — tomado del subject vinculado. |
| **DR-83 Object Thing Node Kind** | Un statement: object thing node kind — tomado del object thing vinculado. |
| **DR-84 Object Node Kind** | El campo «object node kind» del statement se determina según la siguiente prioridad:<br>1. «value», si el object thing está vacío;<br>2. en todos los demás casos, el object thing node kind. |
| **DR-85 Object Value Node Label** | Un statement: object value node label — tomado del property vinculado. |
| **DR-86 Count of Violations As First** | Un statement: count of violations as first es el total de violation unit entre los disjoint checks relacionados con el statement. |
| **DR-87 Count of Violations As Second** | Un statement: count of violations as second es el total de violation unit entre los disjoint checks relacionados con el statement. |
| **DR-88 Is in Contradiction** | Un statement se considera in contradiction si se cumple al menos una de las siguientes: el count of violations as first es mayor que 0 o el count of violations as second es mayor que 0. |
| **DR-89 Rule Enabled** | Un firing: rule enabled es verdadero cuando el rule vinculado es enabled. |
| **DR-90 Kind Wording** | Un firing: kind wording — tomado del kind vinculado. |
| **DR-91 Kind Sort Order** | Un firing: kind sort order — tomado del kind vinculado. |
| **DR-92 Target Name** | Un firing: target name — tomado del target vinculado. |
| **DR-93 Target Subject Label** | Un firing: target subject label — tomado del target vinculado. |
| **DR-94 Target Property Label** | Un firing: target property label — tomado del target vinculado. |
| **DR-95 Target Object Text** | Un firing: target object text — tomado del target vinculado. |
| **DR-96 Target is Conclusion** | Un firing: target is conclusion es verdadero cuando el target vinculado es un conclusion. |
| **DR-97 Target Subject Node Key** | Un firing: target subject node key — tomado del target vinculado. |
| **DR-98 Target Object Node Key** | Un firing: target object node key — tomado del target vinculado. |
| **DR-99 Target First Fired Order Key** | Un firing: target first fired order key — tomado del target vinculado. |
| **DR-100 First Words** | Un firing: first words — tomado del first vinculado. |
| **DR-101 Second Words** | Un firing: second words — tomado del second vinculado. |
| **DR-102 First is in Effect** | Un firing: first is in effect es verdadero cuando el first vinculado es in effect. |
| **DR-103 First Holds Before Chain** | Un firing: first holds before chain es verdadero cuando el first vinculado es holds before chain. |
| **DR-104 Second Holds Before Chain** | Un firing: second holds before chain es verdadero cuando el second vinculado es holds before chain. |
| **DR-105 First Holds Before Typing** | Un firing: first holds before typing es verdadero cuando el first vinculado es holds before typing. |
| **DR-106 First Property** | Un firing: first property — tomado del first vinculado. |
| **DR-107 First Property Domain Class** | Un firing: first property domain class — tomado del first vinculado. |
| **DR-108 First Property Range Class** | Un firing: first property range class — tomado del first vinculado. |
| **DR-109 First Sort Order** | Un firing: first sort order — tomado del first vinculado. |
| **DR-110 Second is in Effect** | Un firing: second is in effect es verdadero cuando el second vinculado es in effect. |
| **DR-111 First Snap Position** | Un firing: first snap position — tomado del first vinculado. |
| **DR-112 Second Snap Position** | Un firing: second snap position — tomado del second vinculado. |
| **DR-113 Inverse Fired Unit** | El campo «inverse fired unit» del firing se determina según la siguiente prioridad:<br>1. 1, si se cumple todo lo siguiente: el indicador «rule enabled» está activado; el kind es «inverse»; y el indicador «first is in effect» está activado;<br>2. en todos los demás casos, 0. |
| **DR-114 Symmetric Fired Unit** | El campo «symmetric fired unit» del firing se determina según la siguiente prioridad:<br>1. 1, si se cumple todo lo siguiente: el indicador «rule enabled» está activado; el kind es «symmetric»; y el indicador «first is in effect» está activado;<br>2. en todos los demás casos, 0. |
| **DR-115 Chain Fired Unit** | El campo «chain fired unit» del firing se determina según la siguiente prioridad:<br>1. 1, si se cumple todo lo siguiente: el indicador «rule enabled» está activado; el kind es «chain»; el indicador «first holds before chain» está activado; y el indicador «second holds before chain» está activado;<br>2. en todos los demás casos, 0. |
| **DR-116 Type Fired Unit** | El campo «type fired unit» del firing se determina según la siguiente prioridad:<br>1. 1, si se cumple todo lo siguiente: el indicador «rule enabled» está activado; se cumple al menos una de las siguientes: el kind es «domain» o el kind es «range»; y el indicador «first holds before typing» está activado;<br>2. en todos los demás casos, 0. |
| **DR-117 Fired Unit** | Un firing: fired unit se calcula como el inverse fired unit más el symmetric fired unit más el chain fired unit más el type fired unit. |
| **DR-118 Pre Chain Fired Unit** | Un firing: pre chain fired unit se calcula como el inverse fired unit más el symmetric fired unit. |
| **DR-119 Pre Chain Order Key** | Un firing: pre chain order key se calcula como el first sort order por 10 más el kind sort order. |
| **DR-120 Round** | El campo «round» del firing se determina según la siguiente prioridad:<br>1. 2, si se cumple todo lo siguiente: el kind es «chain» y no se da el caso de que se cumple todo lo siguiente: el indicador «first is in effect» está activado y el indicador «second is in effect» está activado;<br>2. en todos los demás casos, 1. |
| **DR-121 Order Key** | Un firing: order key se calcula como el round por 1000000000000000 más el first snap position por 100000000 más el kind sort order por 10000000 más 0 si el second está vacío, en todos los demás casos el second snap position. |
| **DR-122 Is Chosen** | Un firing se considera un chosen si se cumple todo lo siguiente: el fired unit es 1; el indicador «target is conclusion» está activado; y el order key es el target first fired order key. |
| **DR-123 Chosen Unit** | El campo «chosen unit» del firing se determina según la siguiente prioridad:<br>1. 1, si el indicador «chosen» está activado;<br>2. en todos los demás casos, 0. |
| **DR-124 Card Text** | El campo «card text» del firing se determina según la siguiente prioridad:<br>1. «Worked out by the », seguido de el kind wording, seguido de « from “», seguido de el first words, seguido de «”», seguido de « and “», seguido de el second words, seguido de «”» si el kind es «chain», en todos los demás casos « and “», seguido de el first property, seguido de « has domain », seguido de el first property domain class, seguido de «”» si el kind es «domain», en todos los demás casos « and “», seguido de el first property, seguido de « has range », seguido de el first property range class, seguido de «”», seguido de un punto, si el indicador «chosen» está activado;<br>2. en todos los demás casos, una cadena vacía. ⚠︎ mecánica <!-- rulespeak:reword --> |
| **DR-125 Name** | Un firing: name es igual a firing ID. |
| **DR-126 Rule Enabled** | Un disjoint check: rule enabled es verdadero cuando el rule vinculado es enabled. |
| **DR-127 First Holds** | Un disjoint check: first holds es verdadero cuando el first vinculado es holds. |
| **DR-128 Second Holds** | Un disjoint check: second holds es verdadero cuando el second vinculado es holds. |
| **DR-129 Subject Label** | Un disjoint check: subject label — tomado del first vinculado. |
| **DR-130 Object Label** | Un disjoint check: object label — tomado del first vinculado. |
| **DR-131 Violation Unit** | El campo «violation unit» del disjoint check se determina según la siguiente prioridad:<br>1. 1, si se cumple todo lo siguiente: el indicador «rule enabled» está activado; el indicador «first holds» está activado; y el indicador «second holds» está activado;<br>2. en todos los demás casos, 0. |
| **DR-132 Alarm Text** | El campo «alarm text» del disjoint check se determina según la siguiente prioridad:<br>1. «Contradiction found. », seguido de el subject label, seguido de « is recorded as both parent and spouse of », seguido de el object label, seguido de «. Check those facts and the selected disjoint rule.», si el violation unit es 1;<br>2. en todos los demás casos, una cadena vacía. |
| **DR-133 Name** | Un disjoint check: name es igual a disjoint check ID. |
| **DR-134 Name** | Un scenario: name es igual a label. |
| **DR-135 Graph Has Facts to Add** | Un scenario: graph has facts to add es verdadero cuando el graph vinculado tiene un facts to add. |
| **DR-136 Is Available** | Un scenario se considera available si se cumple al menos una de las siguientes: el fact baseline no es «next» o el indicador «graph has facts to add» está activado. |
| **DR-137 Name** | Un scenario fact change: name es igual a scenario fact change ID. |
| **DR-138 Name** | Un scenario rule setting: name es igual a scenario rule setting ID. |
| **DR-139 Name** | Un node layout: name es igual a node layout ID. |
| **DR-140 Name** | Un layout slot: name es igual a layout slot ID. |
| **DR-141 Name** | Un page text: name es igual a page text ID. |
| **DR-142 Name** | Un engine: name es igual a label. |
| **DR-143 Name** | Un rulebook tool: name es igual a label. |

## 5 Trazabilidad al esquema

_La columna de expresión es la definición de la regla en notación Documentación —
la misma lógica que almacena el libro de reglas, escrita para un lector de negocio._

| Elemento del esquema | Tipo | Expresión |
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
| **Graphs.MaxSortOrder** | agregado | `Max(Statements.SortOrder via Graph)` |
| **Graphs.NextCandidateSortOrder** | fórmula | `MaxSortOrder + 1` |
| **Rules.Name** | fórmula | `Label` |
| **Rules.CountOfFirings** | agregado | `Sum(Firings.FiredUnit via Rule)` |
| **FiringKinds.Name** | fórmula | `Wording` |
| **FiringKinds.RuleEnabled** | búsqueda | `Lookup(Rules.IsEnabled via Rule)` |
| **Things.PortraitName** | búsqueda | `Lookup(Portraits.DisplayName via Portrait)` |
| **Things.PortraitPage** | búsqueda | `Lookup(Portraits.Page via Portrait)` |
| **Things.PortraitImage** | búsqueda | `Lookup(Portraits.Image via Portrait)` |
| **Things.PortraitCrop** | búsqueda | `Lookup(Portraits.Crop via Portrait)` |
| **Things.PortraitWidth** | búsqueda | `Lookup(Portraits.Width via Portrait)` |
| **Things.PortraitHeight** | búsqueda | `Lookup(Portraits.Height via Portrait)` |
| **Things.Name** | fórmula | `Label` |
| **Things.NodeKey** | fórmula | `ThingId` |
| **Things.CountOfChildren** | agregado | `Sum(Statements.ParentUnit via Subject)` |
| **Things.CountOfGrandchildren** | agregado | `Sum(Statements.GrandparentUnit via Subject)` |
| **Things.CountAsPlace** | agregado | `Sum(Statements.PlaceUnit via ObjectThing)` |
| **Things.CountAsSubject** | agregado | `Sum(Statements.HoldsUnit via Subject)` |
| **Things.CountAsObject** | agregado | `Sum(Statements.HoldsUnit via ObjectThing)` |
| **Things.IsInGraph** | fórmula | `Or(CountAsSubject > 0, CountAsObject > 0)` |
| **Things.NodeKind** | fórmula | `If(Kind = "", "", If(Kind = "class", "class", If(CountAsPlace > 0, "place", "person")))` |
| **Portraits.Name** | fórmula | `DisplayName` |
| **PortraitAliases.Name** | fórmula | `PortraitAliasId` |
| **Properties.Name** | fórmula | `Label` |
| **PropertyAxioms.Name** | fórmula | `PropertyAxiomId` |
| **Statements.SubjectLabel** | búsqueda | `Lookup(Things.Label via Subject)` |
| **Statements.PropertyLabel** | búsqueda | `Lookup(Properties.Label via Property)` |
| **Statements.ObjectLabel** | búsqueda | `Lookup(Things.Label via ObjectThing)` |
| **Statements.PropertyDatatype** | búsqueda | `Lookup(Properties.Datatype via Property)` |
| **Statements.PropertyHasOneCurrentValue** | búsqueda | `Lookup(Properties.HasOneCurrentValue via Property)` |
| **Statements.PropertyDomainClass** | búsqueda | `Lookup(Properties.DomainClass via Property)` |
| **Statements.PropertyRangeClass** | búsqueda | `Lookup(Properties.RangeClass via Property)` |
| **Statements.LiteralQuote** | búsqueda | `Lookup(Graphs.LiteralQuote via Graph)` |
| **Statements.LiteralTypeMarker** | búsqueda | `Lookup(Graphs.LiteralTypeMarker via Graph)` |
| **Statements.BuiltInSourceText** | búsqueda | `Lookup(Graphs.BuiltInSourceText via Graph)` |
| **Statements.UserSourceText** | búsqueda | `Lookup(Graphs.UserSourceText via Graph)` |
| **Statements.NextFactSortOrder** | búsqueda | `Lookup(Graphs.NextFactSortOrder via Graph)` |
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
| **Statements.HoldsBeforeChain** | fórmula | `If(Or(IsInEffect, CountOfInverseFirings > 0, CountOfSymmetricFirings > 0), True())` |
| **Statements.CountOfChainFirings** | agregado | `Sum(Firings.ChainFiredUnit via Target)` |
| **Statements.HoldsBeforeTyping** | fórmula | `If(Or(HoldsBeforeChain, CountOfChainFirings > 0), True())` |
| **Statements.CountOfTypeFirings** | agregado | `Sum(Firings.TypeFiredUnit via Target)` |
| **Statements.Holds** | fórmula | `If(Or(HoldsBeforeTyping, CountOfTypeFirings > 0), True())` |
| **Statements.IsConclusion** | fórmula | `If(And(Holds, Not(IsInEffect)), True())` |
| **Statements.HoldsUnit** | fórmula | `If(Holds, 1, 0)` |
| **Statements.ConclusionUnit** | fórmula | `If(IsConclusion, 1, 0)` |
| **Statements.ParentUnit** | fórmula | `If(And(Holds, Property = "isParentOf"), 1, 0)` |
| **Statements.GrandparentUnit** | fórmula | `If(And(Holds, Property = "isGrandparentOf"), 1, 0)` |
| **Statements.PlaceUnit** | fórmula | `If(And(Holds, Property = "attends"), 1, 0)` |
| **Statements.FirstPreChainOrderKey** | agregado | `Min(Firings.PreChainOrderKey via Target)` |
| **Statements.SnapPosition** | fórmula | `If(IsInEffect, SortOrder, If(HoldsBeforeChain, 100000 + FirstPreChainOrderKey, 0))` |
| **Statements.FirstFiredOrderKey** | agregado | `Min(Firings.OrderKey via Target)` |
| **Statements.CountOfChosenFirings** | agregado | `Sum(Firings.ChosenUnit via Target)` |
| **Statements.SubjectNodeKey** | fórmula | `Subject` |
| **Statements.ObjectNodeKey** | fórmula | `If(ObjectThing = "", Concat("literal\|", ObjectValue, "\|", PropertyDatatype), ObjectThing)` |
| **Statements.SubjectNodeKind** | búsqueda | `Lookup(Things.NodeKind via Subject)` |
| **Statements.ObjectThingNodeKind** | búsqueda | `Lookup(Things.NodeKind via ObjectThing)` |
| **Statements.ObjectNodeKind** | fórmula | `If(ObjectThing = "", "value", ObjectThingNodeKind)` |
| **Statements.ObjectValueNodeLabel** | búsqueda | `Lookup(Properties.ValueNodeLabel via Property)` |
| **Statements.CountOfViolationsAsFirst** | agregado | `Sum(DisjointChecks.ViolationUnit via First)` |
| **Statements.CountOfViolationsAsSecond** | agregado | `Sum(DisjointChecks.ViolationUnit via Second)` |
| **Statements.IsInContradiction** | fórmula | `Or(CountOfViolationsAsFirst > 0, CountOfViolationsAsSecond > 0)` |
| **Firings.RuleEnabled** | búsqueda | `Lookup(Rules.IsEnabled via Rule)` |
| **Firings.KindWording** | búsqueda | `Lookup(FiringKinds.Wording via Kind)` |
| **Firings.KindSortOrder** | búsqueda | `Lookup(FiringKinds.SortOrder via Kind)` |
| **Firings.TargetName** | búsqueda | `Lookup(Statements.Name via Target)` |
| **Firings.TargetSubjectLabel** | búsqueda | `Lookup(Statements.SubjectLabel via Target)` |
| **Firings.TargetPropertyLabel** | búsqueda | `Lookup(Statements.PropertyLabel via Target)` |
| **Firings.TargetObjectText** | búsqueda | `Lookup(Statements.ObjectText via Target)` |
| **Firings.TargetIsConclusion** | búsqueda | `Lookup(Statements.IsConclusion via Target)` |
| **Firings.TargetSubjectNodeKey** | búsqueda | `Lookup(Statements.SubjectNodeKey via Target)` |
| **Firings.TargetObjectNodeKey** | búsqueda | `Lookup(Statements.ObjectNodeKey via Target)` |
| **Firings.TargetFirstFiredOrderKey** | búsqueda | `Lookup(Statements.FirstFiredOrderKey via Target)` |
| **Firings.FirstWords** | búsqueda | `Lookup(Statements.Words via First)` |
| **Firings.SecondWords** | búsqueda | `Lookup(Statements.Words via Second)` |
| **Firings.FirstIsInEffect** | búsqueda | `Lookup(Statements.IsInEffect via First)` |
| **Firings.FirstHoldsBeforeChain** | búsqueda | `Lookup(Statements.HoldsBeforeChain via First)` |
| **Firings.SecondHoldsBeforeChain** | búsqueda | `Lookup(Statements.HoldsBeforeChain via Second)` |
| **Firings.FirstHoldsBeforeTyping** | búsqueda | `Lookup(Statements.HoldsBeforeTyping via First)` |
| **Firings.FirstProperty** | búsqueda | `Lookup(Statements.Property via First)` |
| **Firings.FirstPropertyDomainClass** | búsqueda | `Lookup(Statements.PropertyDomainClass via First)` |
| **Firings.FirstPropertyRangeClass** | búsqueda | `Lookup(Statements.PropertyRangeClass via First)` |
| **Firings.FirstSortOrder** | búsqueda | `Lookup(Statements.SortOrder via First)` |
| **Firings.SecondIsInEffect** | búsqueda | `Lookup(Statements.IsInEffect via Second)` |
| **Firings.FirstSnapPosition** | búsqueda | `Lookup(Statements.SnapPosition via First)` |
| **Firings.SecondSnapPosition** | búsqueda | `Lookup(Statements.SnapPosition via Second)` |
| **Firings.InverseFiredUnit** | fórmula | `If(And(RuleEnabled, Kind = "inverse", FirstIsInEffect), 1, 0)` |
| **Firings.SymmetricFiredUnit** | fórmula | `If(And(RuleEnabled, Kind = "symmetric", FirstIsInEffect), 1, 0)` |
| **Firings.ChainFiredUnit** | fórmula | `If(And(RuleEnabled, Kind = "chain", FirstHoldsBeforeChain, SecondHoldsBeforeChain), 1, 0)` |
| **Firings.TypeFiredUnit** | fórmula | `If(And(RuleEnabled, Or(Kind = "domain", Kind = "range"), FirstHoldsBeforeTyping), 1, 0)` |
| **Firings.FiredUnit** | fórmula | `InverseFiredUnit + SymmetricFiredUnit + ChainFiredUnit + TypeFiredUnit` |
| **Firings.PreChainFiredUnit** | fórmula | `InverseFiredUnit + SymmetricFiredUnit` |
| **Firings.PreChainOrderKey** | fórmula | `FirstSortOrder * 10 + KindSortOrder` |
| **Firings.Round** | fórmula | `If(And(Kind = "chain", Not(And(FirstIsInEffect, SecondIsInEffect))), 2, 1)` |
| **Firings.OrderKey** | fórmula | `Round * 1000000000000000 + FirstSnapPosition * 100000000 + KindSortOrder * 10000000 + If(Second = "", 0, SecondSnapPosition)` |
| **Firings.IsChosen** | fórmula | `And(FiredUnit = 1, TargetIsConclusion, OrderKey = TargetFirstFiredOrderKey)` |
| **Firings.ChosenUnit** | fórmula | `If(IsChosen, 1, 0)` |
| **Firings.CardText** | fórmula | `If(IsChosen, Concat("Worked out by the ", KindWording, " from “", FirstWords, "”", If(Kind = "chain", Concat(" and “", SecondWords, "”"), If(Kind = "domain", Concat(" and “", FirstProperty, " has domain ", FirstPropertyDomainClass, "”"), If(Kind = "range", Concat(" and “", FirstProperty, " has range ", FirstPropertyRangeClass, "”"), ""))), "."), "")` |
| **Firings.Name** | fórmula | `FiringId` |
| **DisjointChecks.RuleEnabled** | búsqueda | `Lookup(Rules.IsEnabled via Rule)` |
| **DisjointChecks.FirstHolds** | búsqueda | `Lookup(Statements.Holds via First)` |
| **DisjointChecks.SecondHolds** | búsqueda | `Lookup(Statements.Holds via Second)` |
| **DisjointChecks.SubjectLabel** | búsqueda | `Lookup(Statements.SubjectLabel via First)` |
| **DisjointChecks.ObjectLabel** | búsqueda | `Lookup(Statements.ObjectLabel via First)` |
| **DisjointChecks.ViolationUnit** | fórmula | `If(And(RuleEnabled, FirstHolds, SecondHolds), 1, 0)` |
| **DisjointChecks.AlarmText** | fórmula | `If(ViolationUnit = 1, Concat("Contradiction found. ", SubjectLabel, " is recorded as both parent and spouse of ", ObjectLabel, ". Check those facts and the selected disjoint rule."), "")` |
| **DisjointChecks.Name** | fórmula | `DisjointCheckId` |
| **Scenarios.Name** | fórmula | `Label` |
| **Scenarios.GraphHasFactsToAdd** | búsqueda | `Lookup(Graphs.HasFactsToAdd via Graph)` |
| **Scenarios.IsAvailable** | fórmula | `Or(FactBaseline <> "next", GraphHasFactsToAdd)` |
| **ScenarioFactChanges.Name** | fórmula | `ScenarioFactChangeId` |
| **ScenarioRuleSettings.Name** | fórmula | `ScenarioRuleSettingId` |
| **NodeLayouts.Name** | fórmula | `NodeLayoutId` |
| **LayoutSlots.Name** | fórmula | `LayoutSlotId` |
| **PageTexts.Name** | fórmula | `PageTextId` |
| **Engines.Name** | fórmula | `Label` |
| **RulebookTools.Name** | fórmula | `Label` |

---

> **RuleSpeak®** — la notación declarativa de reglas de negocio creada por **Ronald G. Ross**, conforme a las convenciones de **SBVR** — llegará pronto. Mientras tanto, esta página muestra la documentación habitual. [www.RonRoss.info](https://www.RonRoss.info)
