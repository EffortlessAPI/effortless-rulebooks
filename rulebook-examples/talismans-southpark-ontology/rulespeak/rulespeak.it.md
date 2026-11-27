# 📘 Talisman's South Park ontology explainer — Documentazione

_Jessica Talisman's South Park Ontology Explainer, as a rulebook: the page's ten facts, five rule switches, property wiring, provenance wording and current-value policy are rows, and every conclusion is a formula. The rules apply in three fixed strata (inverse and symmetric, then the property chain, then domain and range); property wiring that feeds an earlier stratum would need repeated rounds. Every fact that could hold over the cast is a candidate row; app/lib/candidates.mjs writes the candidates when a thing, value or property is added._

> Regole di business dichiarative generate dal libro delle regole. Ogni enunciato
> esprime una verità del dominio di business — non è una procedura né un imperativo.
> Le formule del libro delle regole sono l'unica fonte di verità; questo documento
> ne è la lettura in linguaggio chiaro.

## 1 Vocabolario di business

| Termine | Descrizione | Commento narrativo |
|------|-------------|-------------------|
| **Graph** | The explainer page as one graph: its wording variables and its totals. | — |
| Label | Un attributo definito. | _Display name of the graph._ |
| Literal Quote | Un attributo definito. | _The quote mark written around a literal value in a fact's wording._ |
| Literal Type Marker | Un attributo definito. | _The marker written between a literal value and its datatype, as in Turtle._ |
| Built in Source Text | Un attributo definito. | _Source line shown for a fact the page states as a teaching example._ |
| User Source Text | Un attributo definito. | _Source line shown for a fact the reader stated._ |
| No Conclusions Text | Un attributo definito. | _Message shown when the selected facts and rules add no conclusion._ |
| Name | Uguale a label. | _Order 1. Display name of the graph._ |
| Count of Stated Facts | Il totale di in effect unit tra i statements collegati a il graph. | _Order 5. Facts that are switched on and in effect._ |
| Count of Conclusions | Il totale di conclusion unit tra i statements collegati a il graph. | _Order 18. Facts that hold only because an enabled rule concluded them._ |
| Count of Violations | Il totale di violation unit tra i disjoint checks collegati a il graph. | _Order 18. Disjointness checks that found both links between the same pair._ |
| Is Consistent | Vero quando il count of violations è 0. | _Order 19. True when no enabled disjointness rule is violated._ |
| Conclusions Message | Determinato per priorità: il no conclusions text se il count of conclusions è 0; in tutti gli altri casi, una stringa vuota. | _Order 19. The empty-state message for the conclusions panel, blank when there are conclusions._ |
| Count of Facts to Add | Il totale di listed off unit tra i statements collegati a il graph. | _Order 5. Listed facts that are not in effect; the Add next fact button is enabled while this is above zero._ |
| Has Facts to Add | Vero quando il count of facts to add è maggiore di 0. | _Order 6. True while some listed fact can still be added._ |
| Next Fact Sort Order | Il valore minimo di sort order tra i statements del graph che sono listed off unit. | _Order 5. List position of the first listed fact that is not in effect._ |
| Max Activation Seq | Il valore massimo di activation seq tra i statements collegati a il graph. | _Order 1. Highest activation number used so far._ |
| Next Activation Seq | Calcolato come il max activation seq più 1. | _Order 2. The activation number the next switched-on fact takes._ |
| Max Listed Sort Order | Il valore massimo di sort order tra i statements del graph che sono listed unit. | _Order 2. Highest list position among listed facts._ |
| Next Listed Sort Order | Calcolato come il max listed sort order più 1. | _Order 3. The list position a newly added fact takes._ |
| Max Sort Order | Il valore massimo di sort order tra i statements collegati a il graph. | _Order 1. Highest sort order of any fact, listed or candidate._ |
| Next Candidate Sort Order | Calcolato come il max sort order più 1. | _Order 2. The sort order a new candidate fact, or a fact removed from the list, takes._ |

<details open>
<summary>Dati di esempio</summary>

| Label | Literal Quote | Literal Type Marker | Built in Source Text | User Source Text | No Conclusions Text | Name ƒ | Count of Stated Facts ƒ | Count of Conclusions ƒ | Count of Violations ƒ | Is Consistent ƒ | Conclusions Message ƒ | Count of Facts to Add ƒ | Has Facts to Add ƒ | Next Fact Sort Order ƒ | Max Activation Seq ƒ | Next Activation Seq ƒ | Max Listed Sort Order ƒ | Next Listed Sort Order ƒ | Max Sort Order ƒ | Next Candidate Sort Order ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| South Park | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | These facts and selected rules add no further conclusions. Try another fact or rule. | South Park | 10 | 0 | 0 | true | These facts and selected rules add no further conclusions. Try another fact or rule. | 0 | false | 0 | 10 | 11 | 10 | 11 | 100269 | 100270 |

_ƒ indica una colonna calcolata._

</details>

| Termine | Descrizione | Commento narrativo |
|------|-------------|-------------------|
| **Rule** | The five rule switches on the page. | — |
| Label | Un attributo definito. | _The rule's name on the page._ |
| Explanation | Un attributo definito. | _The rule's one-line explanation on the page._ |
| Is Enabled | Vero quando una stringa vuota. | _The rule's checkbox._ |
| Sort Order | Un attributo definito. | _Position in the rule list._ |
| Name | Uguale a label. | _Order 1. The rule's name._ |
| Count of Firings | Il totale di fired unit tra i firings collegati a il rule. | _Order 15. Rule applications that fired, counting every derivation._ |

<details open>
<summary>Dati di esempio</summary>

| Label | Explanation | Is Enabled | Sort Order | Name ƒ | Count of Firings ƒ |
|---|---|---|---|---|---|
| Inverse | A parent link also gives a child link between the same two people. | false | 1 | Inverse | 0 |
| Symmetric | A friend link or a spouse link is true for both people. | false | 2 | Symmetric | 0 |
| Property chain | A parent of a parent is a grandparent. | false | 3 | Property chain | 0 |

_ƒ indica una colonna calcolata._

</details>

| Termine | Descrizione | Commento narrativo |
|------|-------------|-------------------|
| **Firing Kind** | The ways a rule can apply. Domain and range belong to one rule but are worded apart; 'disjoint' names the rule the disjointness checks answer to. | — |
| Rule | Un attributo definito. | _The rule switch this kind belongs to._ |
| Wording | Un attributo definito. | _How a conclusion card names the rule, as in 'Worked out by the inverse rule'._ |
| Sort Order | Un attributo definito. | _Order in which the page's reasoner tries the kinds for one fact._ |
| Name | Uguale a wording. | _Order 1. The kind's wording._ |
| Rule Enabled | Vero quando il rule collegato è enabled. | _Order 1. Whether the owning rule is switched on._ |

<details open>
<summary>Dati di esempio</summary>

| Wording | Sort Order | Name ƒ | Rule Enabled ƒ |
|---|---|---|---|
| inverse rule | 1 | inverse rule | false |
| symmetric rule | 2 | symmetric rule | false |
| property chain rule | 3 | property chain rule | false |

_ƒ indica una colonna calcolata._

</details>

| Termine | Descrizione | Commento narrativo |
|------|-------------|-------------------|
| **Thing** | Every individual and class the page can draw. | — |
| Label | Un attributo definito. | _The name the page shows._ |
| Kind | Un attributo definito. | _'individual' or 'class'._ |
| Sort Order | Un attributo definito. | _Position in the cast._ |
| Portrait | Un attributo definito. | _The portrait drawn for this thing; set when its name matches a portrait alias._ |
| Portrait Name | Il display name del portrait collegato del thing. | _Order 1. The name on the portrait's Fandom page._ |
| Portrait Page | Ripreso dal portrait collegato. | _Order 1. The Fandom page credited for the portrait._ |
| Portrait Image | Ripreso dal portrait collegato. | _Order 1. The original Fandom image._ |
| Portrait Crop | Ripreso dal portrait collegato. | _Order 1. The portrait's crop as an SVG viewBox._ |
| Portrait Width | Ripreso dal portrait collegato. | _Order 1. The original image's width in pixels._ |
| Portrait Height | Ripreso dal portrait collegato. | _Order 1. The original image's height in pixels._ |
| Name | Uguale a label. | _Order 1. The thing's name._ |
| Node Key | Uguale a thing ID. | _Order 1. The key the graph draws this thing under._ |
| Count of Children | Il totale di parent unit tra i statements collegati a il thing. | _Order 17. People this thing is a parent of, stated or concluded._ |
| Count of Grandchildren | Il totale di grandparent unit tra i statements collegati a il thing. | _Order 17. People this thing is a grandparent of._ |
| Count As Place | Il totale di place unit tra i statements collegati a il thing. | _Order 17. Holding attends facts whose object is this thing._ |
| Count As Subject | Il totale di holds unit tra i statements collegati a il thing. | _Order 17. Holding facts about this thing._ |
| Count As Object | Il totale di holds unit tra i statements collegati a il thing. | _Order 17. Holding facts that point at this thing._ |
| Is in Graph | Vero quando vale almeno una delle seguenti: il count as subject è maggiore di 0 o il count as object è maggiore di 0. | _Order 18. True when the graph draws this thing._ |
| Node Kind | Determinato per priorità: una stringa vuota se il kind è vuoto; «class» se il kind è «class»; «place» se il count as place è maggiore di 0; in tutti gli altri casi, «person». | _Order 18. How the graph draws the thing: a class, a place someone attends, or a person._ |

<details open>
<summary>Dati di esempio</summary>

| Label | Kind | Sort Order | Portrait Name ƒ | Portrait Page ƒ | Portrait Image ƒ | Portrait Crop ƒ | Portrait Width ƒ | Portrait Height ƒ | Name ƒ | Node Key ƒ | Count of Children ƒ | Count of Grandchildren ƒ | Count As Place ƒ | Count As Subject ƒ | Count As Object ƒ | Is in Graph ƒ | Node Kind ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| Marvin | individual | 1 | Marvin Marsh | https://southpark.fandom.com/wiki/Marvin_Marsh | https://static.wikia.nocookie.net/southpark/images/d/db/Marsh-family-grandpa-marsh-w-purple-blanket.png/revision/latest | 48 -5 280 280 | 375 | 536 | Marvin | Marvin | 1 | 0 | 0 | 1 | 0 | true | person |
| Randy | individual | 2 | Randy Marsh | https://southpark.fandom.com/wiki/Randy_Marsh | https://static.wikia.nocookie.net/southparkfanon/images/d/d6/Randy_Marsh.png/revision/latest | 0 -10 428 428 | 428 | 1057 | Randy | Randy | 2 | 0 | 0 | 3 | 1 | true | person |
| Sharon | individual | 3 | Sharon Marsh | https://southpark.fandom.com/wiki/Sharon_Marsh | https://static.wikia.nocookie.net/southpark/images/9/99/Sharon_Marsh.png/revision/latest | 20 -10 390 390 | 439 | 1049 | Sharon | Sharon | 0 | 0 | 0 | 0 | 1 | true | person |

_ƒ indica una colonna calcolata._

</details>

| Termine | Descrizione | Commento narrativo |
|------|-------------|-------------------|
| **Portrait** | The page's Fandom portraits, including those for characters a reader may add. | — |
| Display Name | Un attributo definito. | _The name on the portrait's Fandom page._ |
| Page | Un attributo definito. | _The Fandom page credited for the portrait._ |
| Image | Un attributo definito. | _The original Fandom image._ |
| Crop | Un attributo definito. | _The crop as an SVG viewBox: x y width height._ |
| Width | Un attributo definito. | _The original image's width in pixels._ |
| Height | Un attributo definito. | _The original image's height in pixels._ |
| Sort Order | Un attributo definito. | _Position in the credits._ |
| Name | Uguale a display name. | _Order 1. The portrait's name._ |

<details open>
<summary>Dati di esempio</summary>

| Display Name | Page | Image | Crop | Width | Height | Sort Order | Name ƒ |
|---|---|---|---|---|---|---|---|
| Marvin Marsh | https://southpark.fandom.com/wiki/Marvin_Marsh | https://static.wikia.nocookie.net/southpark/images/d/db/Marsh-family-grandpa-marsh-w-purple-blanket.png/revision/latest | 48 -5 280 280 | 375 | 536 | 1 | Marvin Marsh |
| Randy Marsh | https://southpark.fandom.com/wiki/Randy_Marsh | https://static.wikia.nocookie.net/southparkfanon/images/d/d6/Randy_Marsh.png/revision/latest | 0 -10 428 428 | 428 | 1057 | 2 | Randy Marsh |
| Sharon Marsh | https://southpark.fandom.com/wiki/Sharon_Marsh | https://static.wikia.nocookie.net/southpark/images/9/99/Sharon_Marsh.png/revision/latest | 20 -10 390 390 | 439 | 1049 | 3 | Sharon Marsh |

_ƒ indica una colonna calcolata._

</details>

| Termine | Descrizione | Commento narrativo |
|------|-------------|-------------------|
| **Portrait Aliase** | The names a portrait answers to. A new thing whose name matches an alias gets that portrait. | — |
| Portrait | Un attributo definito. | _The portrait this name selects._ |
| Name | Uguale a portrait alias ID. | _Order 1. The alias._ |

<details open>
<summary>Dati di esempio</summary>

| Name ƒ |
|---|
| marvin |
| marvin marsh |
| grandpa marsh |

_ƒ indica una colonna calcolata._

</details>

| Termine | Descrizione | Commento narrativo |
|------|-------------|-------------------|
| **Property** | The ten properties, with what the symmetric, domain and range rules read. Axioms that relate one property to another (inverse, chain, disjoint) are rows of PropertyAxioms. | — |
| Label | Un attributo definito. | _The property's wording on the page._ |
| Property Kind | Un attributo definito. | _'object' links two things, 'datatype' links a thing to a value, 'type' states class membership._ |
| Datatype | Un attributo definito. | _The value's XML Schema datatype for a datatype property; blank otherwise._ |
| Is User Addable | Vero quando una stringa vuota. | _Offered in the page's Add fact form._ |
| Has One Current Value | Vero quando una stringa vuota. | _The page keeps one current value per person: switching a value on replaces the earlier one._ |
| Is Symmetric | Vero quando una stringa vuota. | _The symmetric rule makes this link true in both directions._ |
| Domain Class | Un attributo definito. | _The class the domain rule gives this property's subject._ |
| Range Class | Un attributo definito. | _The class the range rule gives this property's object._ |
| Sort Order | Un attributo definito. | _Position in the Add fact form's property list._ |
| Value Node Label | Un attributo definito. | _The small label under a value node for this property: 'role value' or 'number value'._ |
| Current Value Wording | Un attributo definito. | _How the Add fact form names a replaced value, as in 'Stan now has grade level 5'._ |
| Input Name | Un attributo definito. | _How the integer check names this property's value, as in 'for grade level'._ |
| Record Kind | Un attributo definito. | _The word the current-record summary uses for this property's values: role, grade or school._ |
| Record Order | Un attributo definito. | _Position of this property in the current-record summary._ |
| Name | Uguale a label. | _Order 1. The property's wording._ |

<details open>
<summary>Dati di esempio</summary>

| Label | Property Kind | Datatype | Is User Addable | Has One Current Value | Is Symmetric | Sort Order | Value Node Label | Current Value Wording | Input Name | Record Kind | Record Order | Name ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|
| is parent of | object | — | true | false | false | 1 | — | — | — | — | — | is parent of |
| is child of | object | — | true | false | false | 2 | — | — | — | — | — | is child of |
| is spouse of | object | — | true | false | true | 3 | — | — | — | — | — | is spouse of |

_ƒ indica una colonna calcolata._

</details>

| Termine | Descrizione | Commento narrativo |
|------|-------------|-------------------|
| **Property Axiom** | Axioms that relate one property to others, as OWL states them: an inverse, a two-link chain, or a disjoint pair. The candidate generator reads these rows. | — |
| Kind | Un attributo definito. | _inverse, chain or disjoint._ |
| Property | Un attributo definito. | _The property the axiom is about: the one inverted, the one a chain derives, or the first of a disjoint pair._ |
| First | Un attributo definito. | _The inverse property, the chain's first link, or the other property of a disjoint pair._ |
| Second | Un attributo definito. | _The chain's second link._ |
| Name | Uguale a property axiom ID. | _Order 1. The axiom's key._ |

<details open>
<summary>Dati di esempio</summary>

| Name ƒ |
|---|
| inverse:isParentOf |
| disjoint:isParentOf~isSpouseOf |
| inverse:isChildOf |

_ƒ indica una colonna calcolata._

</details>

| Termine | Descrizione | Commento narrativo |
|------|-------------|-------------------|
| **Statement** | Every fact the page can state or conclude about the cast. A listed fact appears in the page's fact list; the rest are candidates that only a rule can make hold. | — |
| Graph | Un attributo definito. | _The graph the fact belongs to._ |
| Subject | Un attributo definito. | _What the fact describes._ |
| Property | Un attributo definito. | _How the subject connects to the object._ |
| Object Thing | Un attributo definito. | _The related thing, for an object or type property._ |
| Object Value | Un attributo definito. | _The literal value, for a datatype property._ |
| Is Listed | Vero quando una stringa vuota. | _Shown in the page's fact list._ |
| Is Built in | Vero quando una stringa vuota. | _One of the page's teaching facts; the reader cannot remove it._ |
| Is Detail | Vero quando una stringa vuota. | _One of Stan's detail facts that Show Stan's details switches on._ |
| Is Stated | Vero quando una stringa vuota. | _The fact's checkbox._ |
| Activation Seq | Un attributo definito. | _When the fact was last switched on, as a running number; decides which value is current._ |
| Sort Order | Un attributo definito. | _Position in the fact list; candidates follow the listed facts._ |
| Subject Label | Ripreso dal subject collegato. | _Order 1. The subject's name._ |
| Property Label | Ripreso dal property collegato. | _Order 1. The property's wording._ |
| Object Label | Ripreso dal object thing collegato. | _Order 1. The object's name, for an object or type property._ |
| Property Datatype | Ripreso dal property collegato. | _Order 1. The value's datatype, for a datatype property._ |
| Property Has One Current Value | Vero quando il property collegato ha un one current value. | _Order 1. Whether the property keeps one current value per person._ |
| Property Domain Class | Ripreso dal property collegato. | _Order 1. The property's domain class._ |
| Property Range Class | Ripreso dal property collegato. | _Order 1. The property's range class._ |
| Literal Quote | Ripreso dal graph collegato. | _Order 1. The graph's literal quote mark._ |
| Literal Type Marker | Ripreso dal graph collegato. | _Order 1. The graph's datatype marker._ |
| Built in Source Text | Ripreso dal graph collegato. | _Order 1. The graph's source line for a teaching fact._ |
| User Source Text | Ripreso dal graph collegato. | _Order 1. The graph's source line for a reader's fact._ |
| Next Fact Sort Order | Ripreso dal graph collegato. | _Order 6. List position of the next fact to add._ |
| Is Literal | Vero quando l'object thing è vuoto. | _Order 1. True when the object is a value rather than a thing._ |
| Object Text | Determinato per priorità: l'object value se l'object thing è vuoto; in tutti gli altri casi, l'object label. | _Order 2. The object's name or the value._ |
| Name | Calcolato come il subject label, seguito da uno spazio, seguito da il property label, seguito da uno spazio, seguito da l'object text. | _Order 3. The fact in words._ |
| Words | Calcolato come il subject label, seguito da uno spazio, seguito da il property label, seguito da uno spazio, seguito da il literal quote, seguito da l'object value, seguito da il literal quote, seguito da il literal type marker, seguito da il property datatype se l'object thing è vuoto, in tutti gli altri casi l'object label. ⚠︎ meccanica <!-- rulespeak:reword --> | _Order 2. The fact as a conclusion card quotes it, with a literal written in Turtle form._ |
| Source | Determinato per priorità: il built in source text se l'indicatore “built in” è attivo; in tutti gli altri casi, l'user source text. | _Order 2. Where a listed fact came from._ |
| Subject Property Key | Calcolato come il subject, seguito da «|», seguito da il property. | _Order 1. Groups the values one subject has for one property._ |
| Latest Sibling Activation Seq | Il valore massimo di activation seq tra i statements collegati a il statement. | _Order 2. The most recent activation among this subject's values for this property._ |
| Is in Effect | Vero quando vale tutto quanto segue: l'indicatore “stated” è attivo e vale almeno una delle seguenti: l'indicatore “property has one current value” non è attivo o l'activation seq è il latest sibling activation seq. | _Order 3. Switched on and, for a one-value property, the most recently switched-on value. Drawn as a solid line._ |
| Listed Unit | Determinato per priorità: 1 se l'indicatore “listed” è attivo; in tutti gli altri casi, 0. | _Order 1. 1 when the fact is in the fact list._ |
| In Effect Unit | Determinato per priorità: 1 se l'indicatore “in effect” è attivo; in tutti gli altri casi, 0. | _Order 4. 1 when the fact is in effect._ |
| Listed Off Unit | Determinato per priorità: 1 se vale tutto quanto segue: l'indicatore “listed” è attivo e l'indicatore “in effect” non è attivo; in tutti gli altri casi, 0. | _Order 4. 1 when the fact is listed but not in effect._ |
| Is Next to Add | Vero quando vale tutto quanto segue: il listed off unit è 1 e il sort order è il next fact sort order. | _Order 7. The fact the Add next fact button switches on._ |
| Count of Inverse Firings | Il totale di inverse fired unit tra i firings collegati a il statement. | _Order 6. Inverse rule applications that conclude this fact._ |
| Count of Symmetric Firings | Il totale di symmetric fired unit tra i firings collegati a il statement. | _Order 6. Symmetric rule applications that conclude this fact._ |
| Holds Before Chain | Vero quando il IF di il OR di l'is in effect; il count of inverse firings è maggiore di 0; e il count of symmetric firings è maggiore di 0 e vero. | _Order 7. TRUE when the fact is in effect or the inverse or symmetric rule concludes it. Blank when nothing states or concludes it: not known, never FALSE (open world: removing a statement does not say that its opposite is true)._ |
| Count of Chain Firings | Il totale di chain fired unit tra i firings collegati a il statement. | _Order 10. Property chain applications that conclude this fact._ |
| Holds Before Typing | Vero quando il IF di il OR di il holds before chain e il count of chain firings è maggiore di 0 e vero. | _Order 11. TRUE when the fact holds before the domain and range rules apply. Blank when nothing states or concludes it: not known, never FALSE (open world: removing a statement does not say that its opposite is true)._ |
| Count of Type Firings | Il totale di type fired unit tra i firings collegati a il statement. | _Order 14. Domain and range applications that conclude this fact._ |
| Holds | Vero quando il IF di il OR di il holds before typing e il count of type firings è maggiore di 0 e vero. | _Order 15. TRUE when in effect or concluded by an enabled rule; the graph draws it. Blank when nothing states or concludes it: not known, never FALSE (open world: removing a statement does not say that its opposite is true)._ |
| Is Conclusion | Vero quando il IF di il AND di il holds e l'indicatore “in effect” non è attivo e vero. | _Order 16. TRUE when the fact holds only because a rule concluded it; drawn dashed. Blank otherwise: a stated fact is not a conclusion, and a fact nothing concludes is not known (never FALSE)._ |
| Holds Unit | Determinato per priorità: 1 se l'indicatore “holds” è attivo; in tutti gli altri casi, 0. | _Order 16. 1 when the fact holds._ |
| Conclusion Unit | Determinato per priorità: 1 se l'indicatore “conclusion” è attivo; in tutti gli altri casi, 0. | _Order 17. 1 when the fact is a conclusion._ |
| Parent Unit | Determinato per priorità: 1 se vale tutto quanto segue: l'indicatore “holds” è attivo e il property è «isParentOf»; in tutti gli altri casi, 0. | _Order 16. 1 for a holding parent link._ |
| Grandparent Unit | Determinato per priorità: 1 se vale tutto quanto segue: l'indicatore “holds” è attivo e il property è «isGrandparentOf»; in tutti gli altri casi, 0. | _Order 16. 1 for a holding grandparent link._ |
| Place Unit | Determinato per priorità: 1 se vale tutto quanto segue: l'indicatore “holds” è attivo e il property è «attends»; in tutti gli altri casi, 0. | _Order 16. 1 for a holding attends link._ |
| First Pre Chain Order Key | Il valore minimo di pre chain order key tra i firings del statement che sono pre chain fired unit. | _Order 7. Order key of the first inverse or symmetric application that concludes this fact._ |
| Snap Position | Determinato per priorità: il sort order se l'indicatore “in effect” è attivo; 100000 più il first pre chain order key se l'indicatore “holds before chain” è attivo; in tutti gli altri casi, 0. | _Order 8. Where the page's reasoner meets this fact as a premise: a stated fact at its list position, a fact concluded by the inverse or symmetric rule after every stated fact, in the order it was concluded; 0 for a fact that does not hold yet, so a link to no fact reads the same on every engine._ |
| First Fired Order Key | Il valore minimo di order key tra i firings del statement che sono fired unit. | _Order 15. Order key of the first rule application that concludes this fact, in the page reasoner's order._ |
| Count of Chosen Firings | Il totale di chosen unit tra i firings collegati a il statement. | _Order 20. Rule applications shown as this fact's conclusion card: one for a conclusion, zero otherwise._ |
| Subject Node Key | Uguale a subject. | _Order 1. The graph node this fact starts from._ |
| Object Node Key | Determinato per priorità: «literal|», seguito da l'object value, seguito da «|», seguito da il property datatype se l'object thing è vuoto; in tutti gli altri casi, l'object thing. | _Order 2. The graph node this fact points at: the thing, or one node per value and datatype._ |
| Subject Node Kind | Ripreso dal subject collegato. | _Order 19. How the graph draws the subject._ |
| Object Thing Node Kind | Ripreso dal object thing collegato. | _Order 19. How the graph draws the object thing._ |
| Object Node Kind | Determinato per priorità: «value» se l'object thing è vuoto; in tutti gli altri casi, l'object thing node kind. | _Order 20. How the graph draws the object: a value, a class, a place or a person._ |
| Object Value Node Label | Ripreso dal property collegato. | _Order 1. The small label under a value node._ |
| Count of Violations As First | Il totale di violation unit tra i disjoint checks collegati a il statement. | _Order 18. Violated disjointness checks naming this fact first._ |
| Count of Violations As Second | Il totale di violation unit tra i disjoint checks collegati a il statement. | _Order 18. Violated disjointness checks naming this fact second._ |
| Is in Contradiction | Vero quando vale almeno una delle seguenti: il count of violations as first è maggiore di 0 o il count of violations as second è maggiore di 0. | _Order 19. Part of a contradiction the disjoint rule found; drawn in the alarm colour._ |

<details open>
<summary>Dati di esempio</summary>

| Object Value | Is Listed | Is Built in | Is Detail | Is Stated | Activation Seq | Sort Order | Subject Label ƒ | Property Label ƒ | Object Label ƒ | Property Datatype ƒ | Property Has One Current Value ƒ | Property Domain Class ƒ | Property Range Class ƒ | Literal Quote ƒ | Literal Type Marker ƒ | Built in Source Text ƒ | User Source Text ƒ | Next Fact Sort Order ƒ | Is Literal ƒ | Object Text ƒ | Name ƒ | Words ƒ | Source ƒ | Subject Property Key ƒ | Latest Sibling Activation Seq ƒ | Is in Effect ƒ | Listed Unit ƒ | In Effect Unit ƒ | Listed Off Unit ƒ | Is Next to Add ƒ | Count of Inverse Firings ƒ | Count of Symmetric Firings ƒ | Holds Before Chain ƒ | Count of Chain Firings ƒ | Holds Before Typing ƒ | Count of Type Firings ƒ | Holds ƒ | Is Conclusion ƒ | Holds Unit ƒ | Conclusion Unit ƒ | Parent Unit ƒ | Grandparent Unit ƒ | Place Unit ƒ | First Pre Chain Order Key ƒ | Snap Position ƒ | First Fired Order Key ƒ | Count of Chosen Firings ƒ | Subject Node Key ƒ | Object Node Key ƒ | Subject Node Kind ƒ | Object Thing Node Kind ƒ | Object Node Kind ƒ | Object Value Node Label ƒ | Count of Violations As First ƒ | Count of Violations As Second ƒ | Is in Contradiction ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| — | true | true | false | true | 1 | 1 | Marvin | is parent of | Randy | — | false | — | — | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | 0 | false | Randy | Marvin is parent of Randy | Marvin is parent of Randy | Stated as a teaching example on this page. | Marvin\|isParentOf | 1 | true | 1 | 1 | 0 | false | 0 | 0 | true | 0 | true | 0 | true | — | 1 | 0 | 1 | 0 | 0 | 0.0 | 1.0 | 0.0 | 0 | Marvin | Randy | person | person | person | — | 0 | 0 | false |
| — | false | false | false | false | 0 | 100001 | Marvin | is parent of | Sharon | — | false | — | — | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | 0 | false | Sharon | Marvin is parent of Sharon | Marvin is parent of Sharon | Stated by you on this page. | Marvin\|isParentOf | 1 | false | 0 | 0 | 0 | false | 0 | 0 | — | 0 | — | 0 | — | — | 0 | 0 | 0 | 0 | 0 | 0.0 | 0.0 | 0.0 | 0 | Marvin | Sharon | person | person | person | — | 0 | 0 | false |
| — | false | false | false | false | 0 | 100002 | Marvin | is parent of | Stan | — | false | — | — | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | 0 | false | Stan | Marvin is parent of Stan | Marvin is parent of Stan | Stated by you on this page. | Marvin\|isParentOf | 1 | false | 0 | 0 | 0 | false | 0 | 0 | — | 0 | — | 0 | — | — | 0 | 0 | 0 | 0 | 0 | 0.0 | 0.0 | 0.0 | 0 | Marvin | Stan | person | person | person | — | 0 | 0 | false |

_ƒ indica una colonna calcolata._

</details>

| Termine | Descrizione | Commento narrativo |
|------|-------------|-------------------|
| **Firing** | Every way an enabled rule could conclude a candidate fact from the facts that hold. | — |
| Kind | Un attributo definito. | _How the rule applies._ |
| Rule | Un attributo definito. | _The rule switch._ |
| Target | Un attributo definito. | _The fact this application concludes._ |
| First | Un attributo definito. | _The premise._ |
| Second | Un attributo definito. | _The second premise, for the property chain._ |
| Rule Enabled | Vero quando il rule collegato è enabled. | _Order 1. Whether the rule is switched on._ |
| Kind Wording | Ripreso dal kind collegato. | _Order 1. How the card names the rule._ |
| Kind Sort Order | Ripreso dal kind collegato. | _Order 1. The kind's place in the reasoner's order._ |
| Target Name | Ripreso dal target collegato. | _Order 4. The concluded fact in words._ |
| Target Subject Label | Ripreso dal target collegato. | _Order 2. The concluded fact's subject._ |
| Target Property Label | Ripreso dal target collegato. | _Order 2. The concluded fact's property._ |
| Target Object Text | Ripreso dal target collegato. | _Order 3. The concluded fact's object._ |
| Target is Conclusion | Vero quando il target collegato è un conclusion. | _Order 17. Whether the concluded fact is new._ |
| Target Subject Node Key | Ripreso dal target collegato. | _Order 2. The concluded fact's subject node._ |
| Target Object Node Key | Ripreso dal target collegato. | _Order 3. The concluded fact's object node._ |
| Target First Fired Order Key | Ripreso dal target collegato. | _Order 16. The order key of the concluded fact's first application._ |
| First Words | Ripreso dal first collegato. | _Order 3. The premise as the card quotes it._ |
| Second Words | Ripreso dal second collegato. | _Order 3. The second premise as the card quotes it._ |
| First is in Effect | Vero quando il first collegato è in effect. | _Order 4. Whether the premise is in effect._ |
| First Holds Before Chain | Vero quando il first collegato è holds before chain. | _Order 8. Whether the premise holds before the chain._ |
| Second Holds Before Chain | Vero quando il second collegato è holds before chain. | _Order 8. Whether the second premise holds before the chain._ |
| First Holds Before Typing | Vero quando il first collegato è holds before typing. | _Order 12. Whether the premise holds before typing._ |
| First Property | Ripreso dal first collegato. | _Order 1. The premise's property key, as the card's axiom names it._ |
| First Property Domain Class | Ripreso dal first collegato. | _Order 2. The premise property's domain class._ |
| First Property Range Class | Ripreso dal first collegato. | _Order 2. The premise property's range class._ |
| First Sort Order | Ripreso dal first collegato. | _Order 1. The premise's list position._ |
| Second is in Effect | Vero quando il second collegato è in effect. | _Order 4. Whether the second premise is in effect._ |
| First Snap Position | Ripreso dal first collegato. | _Order 9. Where the reasoner meets the premise._ |
| Second Snap Position | Ripreso dal second collegato. | _Order 9. Where the reasoner meets the second premise._ |
| Inverse Fired Unit | Determinato per priorità: 1 se vale tutto quanto segue: l'indicatore “rule enabled” è attivo; il kind è «inverse»; e l'indicatore “first is in effect” è attivo; in tutti gli altri casi, 0. | _Order 5. 1 when the inverse rule fires here._ |
| Symmetric Fired Unit | Determinato per priorità: 1 se vale tutto quanto segue: l'indicatore “rule enabled” è attivo; il kind è «symmetric»; e l'indicatore “first is in effect” è attivo; in tutti gli altri casi, 0. | _Order 5. 1 when the symmetric rule fires here._ |
| Chain Fired Unit | Determinato per priorità: 1 se vale tutto quanto segue: l'indicatore “rule enabled” è attivo; il kind è «chain»; l'indicatore “first holds before chain” è attivo; e l'indicatore “second holds before chain” è attivo; in tutti gli altri casi, 0. | _Order 9. 1 when the property chain fires here._ |
| Type Fired Unit | Determinato per priorità: 1 se vale tutto quanto segue: l'indicatore “rule enabled” è attivo; vale almeno una delle seguenti: il kind è «domain» o il kind è «range»; e l'indicatore “first holds before typing” è attivo; in tutti gli altri casi, 0. | _Order 13. 1 when the domain or range rule fires here._ |
| Fired Unit | Calcolato come l'inverse fired unit più il symmetric fired unit più il chain fired unit più il type fired unit. | _Order 14. 1 when this application fires._ |
| Pre Chain Fired Unit | Calcolato come l'inverse fired unit più il symmetric fired unit. | _Order 6. 1 when this is an inverse or symmetric application that fires._ |
| Pre Chain Order Key | Calcolato come il first sort order per 10 più il kind sort order. | _Order 2. For an inverse or symmetric application: its place in the reasoner's first round._ |
| Round | Determinato per priorità: 2 se vale tutto quanto segue: il kind è «chain» e non è il caso che vale tutto quanto segue: l'indicatore “first is in effect” è attivo e l'indicatore “second is in effect” è attivo; in tutti gli altri casi, 1. | _Order 5. The reasoner's round: 1 when every premise is a stated fact, 2 when the property chain reads a concluded fact._ |
| Order Key | Calcolato come il round per 1000000000000000 più il first snap position per 100000000 più il kind sort order per 10000000 più 0 se il second è vuoto, in tutti gli altri casi il second snap position. | _Order 10. The page reasoner's order: by round, then premise, then rule kind, then second premise._ |
| Is Chosen | Vero quando vale tutto quanto segue: il fired unit è 1; l'indicatore “target is conclusion” è attivo; e l'order key è il target first fired order key. | _Order 18. The application the conclusions panel shows for its fact: the first one in the reasoner's order._ |
| Chosen Unit | Determinato per priorità: 1 se l'indicatore “chosen” è attivo; in tutti gli altri casi, 0. | _Order 19. 1 when this application is shown._ |
| Card Text | Determinato per priorità: «Worked out by the », seguito da il kind wording, seguito da « from “», seguito da il first words, seguito da «”», seguito da « and “», seguito da il second words, seguito da «”» se il kind è «chain», in tutti gli altri casi « and “», seguito da il first property, seguito da « has domain », seguito da il first property domain class, seguito da «”» se il kind è «domain», in tutti gli altri casi « and “», seguito da il first property, seguito da « has range », seguito da il first property range class, seguito da «”», seguito da un punto se l'indicatore “chosen” è attivo; in tutti gli altri casi, una stringa vuota. ⚠︎ meccanica <!-- rulespeak:reword --> | _Order 19. The conclusion card's provenance line, in the page's wording._ |
| Name | Uguale a firing ID. | _Order 1. The application's key._ |

<details open>
<summary>Dati di esempio</summary>

| Rule Enabled ƒ | Kind Wording ƒ | Kind Sort Order ƒ | Target Name ƒ | Target Subject Label ƒ | Target Property Label ƒ | Target Object Text ƒ | Target is Conclusion ƒ | Target Subject Node Key ƒ | Target Object Node Key ƒ | Target First Fired Order Key ƒ | First Words ƒ | Second Words ƒ | First is in Effect ƒ | First Holds Before Chain ƒ | Second Holds Before Chain ƒ | First Holds Before Typing ƒ | First Property ƒ | First Property Domain Class ƒ | First Property Range Class ƒ | First Sort Order ƒ | Second is in Effect ƒ | First Snap Position ƒ | Second Snap Position ƒ | Inverse Fired Unit ƒ | Symmetric Fired Unit ƒ | Chain Fired Unit ƒ | Type Fired Unit ƒ | Fired Unit ƒ | Pre Chain Fired Unit ƒ | Pre Chain Order Key ƒ | Round ƒ | Order Key ƒ | Is Chosen ƒ | Chosen Unit ƒ | Card Text ƒ | Name ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| false | inverse rule | 1 | Randy is child of Marvin | Randy | is child of | Marvin | — | Randy | Marvin | 0.0 | Marvin is parent of Randy |    | true | true | — | true | isParentOf | — | — | 1 | false | 1.0 | 0.0 | 0 | 0 | 0 | 0 | 0 | 0 | 11.0 | 1 | 1000000110000000.0 | false | 0 | — | inverse:Marvin\|isParentOf\|Randy>Randy\|isChildOf\|Marvin |
| false | inverse rule | 1 | Sharon is child of Marvin | Sharon | is child of | Marvin | — | Sharon | Marvin | 0.0 | Marvin is parent of Sharon |    | false | — | — | — | isParentOf | — | — | 100001 | false | 0.0 | 0.0 | 0 | 0 | 0 | 0 | 0 | 0 | 1000011.0 | 1 | 1000000010000000.0 | false | 0 | — | inverse:Marvin\|isParentOf\|Sharon>Sharon\|isChildOf\|Marvin |
| false | inverse rule | 1 | Stan is child of Marvin | Stan | is child of | Marvin | — | Stan | Marvin | 0.0 | Marvin is parent of Stan |    | false | — | — | — | isParentOf | — | — | 100002 | false | 0.0 | 0.0 | 0 | 0 | 0 | 0 | 0 | 0 | 1000021.0 | 1 | 1000000010000000.0 | false | 0 | — | inverse:Marvin\|isParentOf\|Stan>Stan\|isChildOf\|Marvin |

_ƒ indica una colonna calcolata._

</details>

| Termine | Descrizione | Commento narrativo |
|------|-------------|-------------------|
| **Disjoint Check** | Every pair the disjointness rule watches: the same two people linked by two disjoint properties. | — |
| Graph | Un attributo definito. | _The graph._ |
| Rule | Un attributo definito. | _The rule switch._ |
| First | Un attributo definito. | _The first link._ |
| Second | Un attributo definito. | _The disjoint link between the same pair, in the same direction._ |
| Rule Enabled | Vero quando il rule collegato è enabled. | _Order 1. Whether the disjointness rule is on._ |
| First Holds | Vero quando il first collegato è holds. | _Order 16. Whether the first link holds._ |
| Second Holds | Vero quando il second collegato è holds. | _Order 16. Whether the second link holds._ |
| Subject Label | Ripreso dal first collegato. | _Order 2. The person recorded with both links._ |
| Object Label | Ripreso dal first collegato. | _Order 2. The other person._ |
| Violation Unit | Determinato per priorità: 1 se vale tutto quanto segue: l'indicatore “rule enabled” è attivo; l'indicatore “first holds” è attivo; e l'indicatore “second holds” è attivo; in tutti gli altri casi, 0. | _Order 17. 1 when both links hold._ |
| Alarm Text | Determinato per priorità: «Contradiction found. », seguito da il subject label, seguito da « is recorded as both parent and spouse of », seguito da l'object label, seguito da «. Check those facts and the selected disjoint rule.» se il violation unit è 1; in tutti gli altri casi, una stringa vuota. | _Order 18. The page's contradiction alert, blank when there is none._ |
| Name | Uguale a disjoint check ID. | _Order 1. The check's key._ |

<details open>
<summary>Dati di esempio</summary>

| Rule Enabled ƒ | First Holds ƒ | Second Holds ƒ | Subject Label ƒ | Object Label ƒ | Violation Unit ƒ | Alarm Text ƒ | Name ƒ |
|---|---|---|---|---|---|---|---|
| false | true | — | Marvin | Randy | 0 | — | Marvin\|isParentOf\|Randy~isSpouseOf |
| false | — | — | Marvin | Sharon | 0 | — | Marvin\|isParentOf\|Sharon~isSpouseOf |
| false | — | — | Marvin | Stan | 0 | — | Marvin\|isParentOf\|Stan~isSpouseOf |

_ƒ indica una colonna calcolata._

</details>

| Termine | Descrizione | Commento narrativo |
|------|-------------|-------------------|
| **Scenario** | The page's buttons. Each one is a fact baseline, explicit fact changes and rule settings, applied in that order. | — |
| Graph | Un attributo definito. | _The graph._ |
| Label | Un attributo definito. | _The button's text._ |
| Note | Un attributo definito. | _The note shown under the buttons after this one is pressed; blank clears it._ |
| Sort Order | Un attributo definito. | _Position among the buttons._ |
| Button Group | Un attributo definito. | _'stan' for the two imagined changes, 'facts' for the buttons under the fact list._ |
| Is Primary | Vero quando una stringa vuota. | _Drawn as the filled button._ |
| Fact Baseline | Un attributo definito. | _What happens to every listed fact first: keep, built-in (teaching facts on, others off), built-in-detail, all-off, activate-all (each switched on in list order), or next (the next fact on)._ |
| Layout | Un attributo definito. | _The node layout the graph returns to; blank keeps the current positions._ |
| Clears Selection | Vero quando una stringa vuota. | _Clears the selected node or line._ |
| Clears Form Message | Vero quando una stringa vuota. | _Clears the Add fact form's message._ |
| Name | Uguale a label. | _Order 1. The button's text._ |
| Graph Has Facts to Add | Vero quando il graph collegato ha un facts to add. | _Order 7. Whether a listed fact can still be added._ |
| Is Available | Vero quando vale almeno una delle seguenti: il fact baseline non è «next» o l'indicatore “graph has facts to add” è attivo. | _Order 8. False only for Add next fact once every listed fact is in effect._ |

<details open>
<summary>Dati di esempio</summary>

| Label | Note | Sort Order | Button Group | Is Primary | Fact Baseline | Layout | Clears Selection | Clears Form Message | Name ƒ | Graph Has Facts to Add ƒ | Is Available ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|
| Move Stan to grade 5 | This imagined change updates Stan’s grade to 5. His identifier, Person statement and family links stay the same. His other selected details are unchanged. | 1 | stan | false | keep | — | true | true | Move Stan to grade 5 | false | true |
| Stan leaves school | This imagined change clears Stan’s current school details. His identifier, Person statement and family links remain. Removing a current fact does not say that its opposite is true; school history needs dates. | 2 | stan | false | keep | — | true | true | Stan leaves school | false | true |
| Add next fact | — | 3 | facts | true | next | — | false | false | Add next fact | false | false |

_ƒ indica una colonna calcolata._

</details>

| Termine | Descrizione | Commento narrativo |
|------|-------------|-------------------|
| **Scenario Fact Change** | Fact changes a button makes after its baseline. A blank object key matches every listed value of the property. | — |
| Scenario | Un attributo definito. | _The button._ |
| Subject | Un attributo definito. | _The subject whose facts change._ |
| Property | Un attributo definito. | _The property whose facts change._ |
| Object Key | Un attributo definito. | _The object thing's key or the literal value; blank for every listed value._ |
| Action | Un attributo definito. | _'on' switches the fact on as the newest value; 'off' switches it off._ |
| Adds to List | Vero quando una stringa vuota. | _Adds the fact to the fact list first when it is not listed._ |
| Sort Order | Un attributo definito. | _Order of the change within the button._ |
| Name | Uguale a scenario fact change ID. | _Order 1. The change's key._ |

<details open>
<summary>Dati di esempio</summary>

| Object Key | Action | Adds to List | Sort Order | Name ƒ |
|---|---|---|---|---|
| Person | on | true | 1 | stan-grade-five:Stan\|isA\|Person |
| 5 | on | true | 2 | stan-grade-five:Stan\|hasGradeLevel\|5 |
| Person | on | true | 3 | stan-leaves-school:Stan\|isA\|Person |

_ƒ indica una colonna calcolata._

</details>

| Termine | Descrizione | Commento narrativo |
|------|-------------|-------------------|
| **Scenario Rule Setting** | Rule switches a button sets. | — |
| Scenario | Un attributo definito. | _The button._ |
| Rule | Un attributo definito. | _The rule switch._ |
| Is Enabled | Vero quando una stringa vuota. | _The state the button puts the switch in._ |
| Name | Uguale a scenario rule setting ID. | _Order 1. The setting's key._ |

<details open>
<summary>Dati di esempio</summary>

| Is Enabled | Name ƒ |
|---|---|
| false | reset-original:inverse |
| false | reset-original:symmetric |
| false | reset-original:chain |

_ƒ indica una colonna calcolata._

</details>

| Termine | Descrizione | Commento narrativo |
|------|-------------|-------------------|
| **Node Layout** | Node positions in the graph's 380 by 480 drawing, one set per layout. | — |
| Layout | Un attributo definito. | _The layout: 'default' or 'stan-details'._ |
| Node Key | Un attributo definito. | _The node: a thing's key, or literal\|value\|datatype._ |
| X | Un attributo definito. | _Horizontal position._ |
| Y | Un attributo definito. | _Vertical position._ |
| Name | Uguale a node layout ID. | _Order 1. The position's key._ |

<details open>
<summary>Dati di esempio</summary>

| Layout | Node Key | X | Y | Name ƒ |
|---|---|---|---|---|
| default | Marvin | 70 | 45 | default:Marvin |
| default | Randy | 195 | 125 | default:Randy |
| default | Sharon | 318 | 55 | default:Sharon |

_ƒ indica una colonna calcolata._

</details>

| Termine | Descrizione | Commento narrativo |
|------|-------------|-------------------|
| **Layout Slot** | Places for nodes no layout positions, used in turn. | — |
| Sort Order | Un attributo definito. | _Order in which slots are used._ |
| X | Un attributo definito. | _Horizontal position._ |
| Y | Un attributo definito. | _Vertical position._ |
| Name | Uguale a layout slot ID. | _Order 1. The slot's key._ |

<details open>
<summary>Dati di esempio</summary>

| Sort Order | X | Y | Name ƒ |
|---|---|---|---|
| 1 | 330 | 135 | slot-1 |
| 2 | 45 | 120 | slot-2 |
| 3 | 330 | 400 | slot-3 |

_ƒ indica una colonna calcolata._

</details>

| Termine | Descrizione | Commento narrativo |
|------|-------------|-------------------|
| **Page Text** | The page's wording, one row per string. {name}-style placeholders are filled by the page. | — |
| Text | Un attributo definito. | _The string._ |
| Name | Uguale a page text ID. | _Order 1. The string's key._ |

<details open>
<summary>Dati di esempio</summary>

| Text | Name ƒ |
|---|---|
| South Park Ontology Explainer, on one rulebook | page-title |
| A rulebook rebuild of the explainer by | byline-prefix |
| Jessica Talisman | byline-author |

_ƒ indica una colonna calcolata._

</details>

| Termine | Descrizione | Commento narrativo |
|------|-------------|-------------------|
| **Engine** | The execution engines the page can be computed by. Each is a build output of this rulebook. | — |
| Label | Un attributo definito. | _The engine's name in the switcher._ |
| Substrate | Un attributo definito. | _The conformance harness's name for this substrate._ |
| Is Default | Vero quando una stringa vuota. | _The engine a new visitor starts on._ |
| Description | Un attributo definito. | _What computes the page on this engine._ |
| Sort Order | Un attributo definito. | _Position in the switcher._ |
| Name | Uguale a label. | _Order 1. The engine's name._ |

<details open>
<summary>Dati di esempio</summary>

| Label | Substrate | Is Default | Description | Sort Order | Name ƒ |
|---|---|---|---|---|---|
| Postgres views | effortless-postgres | true | Every value is a column of a generated view; the database computes it when the page reads it. | 1 | Postgres views |
| TypeScript | effortless-typescript | false | The generated TypeScript computes every field from the same rows, with no database. | 2 | TypeScript |
| OWL and SHACL rules | effortless-owl | false | The generated ontology and SHACL rules, run over the same rows by a SPARQL engine. | 3 | OWL and SHACL rules |

_ƒ indica una colonna calcolata._

</details>

| Termine | Descrizione | Commento narrativo |
|------|-------------|-------------------|
| **Rulebook Tool** | What each rulebook tool adds to the original explainer; the tools panel lists them. | — |
| Label | Un attributo definito. | _The tool's name._ |
| Adds | Un attributo definito. | _What it adds to the original explainer._ |
| Href | Un attributo definito. | _Where its link goes; 'editor' means the running rulebook editor._ |
| Link Text | Un attributo definito. | _The link's text._ |
| Sort Order | Un attributo definito. | _Position in the panel._ |
| Name | Uguale a label. | _Order 1. The tool's name._ |

<details open>
<summary>Dati di esempio</summary>

| Label | Adds | Href | Link Text | Sort Order | Name ƒ |
|---|---|---|---|---|---|
| Interchangeable engines | The same page, computed by Postgres views, generated TypeScript or the generated OWL and SHACL rules. Switch with the control above; nothing else changes. | — | — | 1 | Interchangeable engines |
| Conformance | Every engine's answers for the current facts, compared value by value with the conformance harness's own rule for when two values are equal. | — | — | 2 | Conformance |
| Explainer | Hover a dotted value for its rule in plain words and its witness: the values it came from, down to the stated facts. | /rulebook-explainer-dag/pages/index.html | Open the explainer | 3 | Explainer |

_ƒ indica una colonna calcolata._

</details>

## 2 Tipi di fatto

- un **firing kind** referenzia esattamente un **rule**
- un **thing** può referenziare un **portrait**
- un **portrait aliase** referenzia esattamente un **portrait**
- un **property** può referenziare un **thing**
- un **property axiom** referenzia esattamente un **firing kind**
- un **property axiom** referenzia esattamente un **property**
- un **statement** referenzia esattamente un **graph**
- un **statement** referenzia esattamente un **thing**
- un **statement** referenzia esattamente un **property**
- un **firing** referenzia esattamente un **firing kind**
- un **firing** referenzia esattamente un **rule**
- un **firing** referenzia esattamente un **statement**
- un **disjoint check** referenzia esattamente un **graph**
- un **disjoint check** referenzia esattamente un **rule**
- un **disjoint check** referenzia esattamente un **statement**
- un **scenario** referenzia esattamente un **graph**
- un **scenario fact change** referenzia esattamente un **scenario**
- un **scenario fact change** referenzia esattamente un **thing**
- un **scenario fact change** referenzia esattamente un **property**
- un **scenario rule setting** referenzia esattamente un **scenario**
- un **scenario rule setting** referenzia esattamente un **rule**

## 3 Regole operative

_Le regole operative enunciano ciò che il business **obbliga**, **vieta** o
consiglia (**dovrebbe**). Le regole strutturali derivano da campi obbligatori e chiavi
esterne; quelle semantiche dalla tabella Constraints, ognuna basata su un booleano che
il libro delle regole già calcola (referenziato come DR-N nelle regole definitorie)._

### Vincoli strutturali (dallo schema)

- Un graph **deve** avere un label; un literal quote; un literal type marker; un built in source text; un user source text; e un no conclusions text.
- Un rule **deve** avere un label; un explanation; e un sort order, e registrare se è enabled.
- Un firing kind **deve** referenziare esattamente un rule.
- Un firing kind **deve** avere un wording e un sort order.
- Un thing **deve** avere un label; un kind; e un sort order.
- Un portrait **deve** avere un display name; un page; un image; un crop; un width; un height; e un sort order.
- Un portrait aliase **deve** referenziare esattamente un portrait.
- Un property **deve** avere un label; un property kind; e un sort order, e registrare se è user addable; se ha un one current value; e se è symmetric.
- Un property axiom **deve** referenziare esattamente un firing kind come suo kind.
- Un property axiom **deve** referenziare esattamente un property.
- Un property axiom **deve** referenziare esattamente un property come suo first.
- Un statement **deve** referenziare esattamente un graph.
- Un statement **deve** referenziare esattamente un thing come suo subject.
- Un statement **deve** referenziare esattamente un property.
- Un statement **deve** avere un activation seq e un sort order, e registrare se è listed; se è un built in; se è un detail; e se è stated.
- Un firing **deve** referenziare esattamente un firing kind come suo kind.
- Un firing **deve** referenziare esattamente un rule.
- Un firing **deve** referenziare esattamente un statement come suo target.
- Un firing **deve** referenziare esattamente un statement come suo first.
- Un disjoint check **deve** referenziare esattamente un graph.
- Un disjoint check **deve** referenziare esattamente un rule.
- Un disjoint check **deve** referenziare esattamente un statement come suo first.
- Un disjoint check **deve** referenziare esattamente un statement come suo second.
- Un scenario **deve** referenziare esattamente un graph.
- Un scenario **deve** avere un label; un sort order; un button group; e un fact baseline, e registrare se è primary; se è clears selection; e se è clears form message.
- Un scenario fact change **deve** referenziare esattamente un scenario.
- Un scenario fact change **deve** referenziare esattamente un thing come suo subject.
- Un scenario fact change **deve** referenziare esattamente un property.
- Un scenario fact change **deve** avere un action e un sort order, e registrare se è adds to list.
- Un scenario rule setting **deve** referenziare esattamente un scenario.
- Un scenario rule setting **deve** referenziare esattamente un rule.
- Un scenario rule setting **deve** registrare se è enabled.
- Un node layout **deve** avere un layout; un node key; un x; e un y.
- Un layout slot **deve** avere un sort order; un x; e un y.
- Un page text **deve** avere un text.
- Un engine **deve** avere un label; un substrate; un description; e un sort order, e registrare se è un default.
- Un rulebook tool **deve** avere un label; un adds; e un sort order.

## 4 Regole definitorie

_Tutti gli enunciati esprimono verità del dominio di business; non sono procedure
né imperativi. Si evita «se e solo se» a favore di «solo se», affinché una necessità
unidirezionale non venga scambiata per un'equivalenza. Un contrassegno
**⚠︎ meccanica** segnala una regola la cui formulazione deterministica è fedele ma
legnosa — un segnale per una riscrittura opzionale, non un difetto._

| ID | Regola dichiarativa |
|----|------------------|
| **DR-1 Name** | Un graph: name è uguale a label. |
| **DR-2 Count of Stated Facts** | Un graph: count of stated facts è il totale di in effect unit tra i statements collegati a il graph. |
| **DR-3 Count of Conclusions** | Un graph: count of conclusions è il totale di conclusion unit tra i statements collegati a il graph. |
| **DR-4 Count of Violations** | Un graph: count of violations è il totale di violation unit tra i disjoint checks collegati a il graph. |
| **DR-5 Is Consistent** | Un graph è considerato consistent se il count of violations è 0. |
| **DR-6 Conclusions Message** | Il campo “conclusions message” del graph è determinato secondo la seguente priorità:<br>1. il no conclusions text, se il count of conclusions è 0;<br>2. in tutti gli altri casi, una stringa vuota. |
| **DR-7 Count of Facts to Add** | Un graph: count of facts to add è il totale di listed off unit tra i statements collegati a il graph. |
| **DR-8 Has Facts to Add** | Un graph è considerato come dotato di un facts to add se il count of facts to add è maggiore di 0. |
| **DR-9 Next Fact Sort Order** | Un graph: next fact sort order è il valore minimo di sort order tra i statements del graph che sono listed off unit. |
| **DR-10 Max Activation Seq** | Un graph: max activation seq è il valore massimo di activation seq tra i statements collegati a il graph. |
| **DR-11 Next Activation Seq** | Un graph: next activation seq è calcolato come il max activation seq più 1. |
| **DR-12 Max Listed Sort Order** | Un graph: max listed sort order è il valore massimo di sort order tra i statements del graph che sono listed unit. |
| **DR-13 Next Listed Sort Order** | Un graph: next listed sort order è calcolato come il max listed sort order più 1. |
| **DR-14 Max Sort Order** | Un graph: max sort order è il valore massimo di sort order tra i statements collegati a il graph. |
| **DR-15 Next Candidate Sort Order** | Un graph: next candidate sort order è calcolato come il max sort order più 1. |
| **DR-16 Name** | Un rule: name è uguale a label. |
| **DR-17 Count of Firings** | Un rule: count of firings è il totale di fired unit tra i firings collegati a il rule. |
| **DR-18 Name** | Un firing kind: name è uguale a wording. |
| **DR-19 Rule Enabled** | Un firing kind: rule enabled è vero quando il rule collegato è enabled. |
| **DR-20 Portrait Name** | Un thing: portrait name è il display name del portrait collegato. |
| **DR-21 Portrait Page** | Un thing: portrait page — ripreso dal portrait collegato. |
| **DR-22 Portrait Image** | Un thing: portrait image — ripreso dal portrait collegato. |
| **DR-23 Portrait Crop** | Un thing: portrait crop — ripreso dal portrait collegato. |
| **DR-24 Portrait Width** | Un thing: portrait width — ripreso dal portrait collegato. |
| **DR-25 Portrait Height** | Un thing: portrait height — ripreso dal portrait collegato. |
| **DR-26 Name** | Un thing: name è uguale a label. |
| **DR-27 Node Key** | Un thing: node key è uguale a thing ID. |
| **DR-28 Count of Children** | Un thing: count of children è il totale di parent unit tra i statements collegati a il thing. |
| **DR-29 Count of Grandchildren** | Un thing: count of grandchildren è il totale di grandparent unit tra i statements collegati a il thing. |
| **DR-30 Count As Place** | Un thing: count as place è il totale di place unit tra i statements collegati a il thing. |
| **DR-31 Count As Subject** | Un thing: count as subject è il totale di holds unit tra i statements collegati a il thing. |
| **DR-32 Count As Object** | Un thing: count as object è il totale di holds unit tra i statements collegati a il thing. |
| **DR-33 Is in Graph** | Un thing è considerato in graph se vale almeno una delle seguenti: il count as subject è maggiore di 0 o il count as object è maggiore di 0. |
| **DR-34 Node Kind** | Il campo “node kind” del thing è determinato secondo la seguente priorità:<br>1. una stringa vuota, se il kind è vuoto;<br>2. «class», se il kind è «class»;<br>3. «place», se il count as place è maggiore di 0;<br>4. in tutti gli altri casi, «person». |
| **DR-35 Name** | Un portrait: name è uguale a display name. |
| **DR-36 Name** | Un portrait aliase: name è uguale a portrait alias ID. |
| **DR-37 Name** | Un property: name è uguale a label. |
| **DR-38 Name** | Un property axiom: name è uguale a property axiom ID. |
| **DR-39 Subject Label** | Un statement: subject label — ripreso dal subject collegato. |
| **DR-40 Property Label** | Un statement: property label — ripreso dal property collegato. |
| **DR-41 Object Label** | Un statement: object label — ripreso dal object thing collegato. |
| **DR-42 Property Datatype** | Un statement: property datatype — ripreso dal property collegato. |
| **DR-43 Property Has One Current Value** | Un statement: property has one current value è vero quando il property collegato ha un one current value. |
| **DR-44 Property Domain Class** | Un statement: property domain class — ripreso dal property collegato. |
| **DR-45 Property Range Class** | Un statement: property range class — ripreso dal property collegato. |
| **DR-46 Literal Quote** | Un statement: literal quote — ripreso dal graph collegato. |
| **DR-47 Literal Type Marker** | Un statement: literal type marker — ripreso dal graph collegato. |
| **DR-48 Built in Source Text** | Un statement: built in source text — ripreso dal graph collegato. |
| **DR-49 User Source Text** | Un statement: user source text — ripreso dal graph collegato. |
| **DR-50 Next Fact Sort Order** | Un statement: next fact sort order — ripreso dal graph collegato. |
| **DR-51 Is Literal** | Un statement è considerato literal se l'object thing è vuoto. |
| **DR-52 Object Text** | Il campo “object text” del statement è determinato secondo la seguente priorità:<br>1. l'object value, se l'object thing è vuoto;<br>2. in tutti gli altri casi, l'object label. |
| **DR-53 Name** | Un statement: name è calcolato come il subject label, seguito da uno spazio, seguito da il property label, seguito da uno spazio, seguito da l'object text. |
| **DR-54 Words** | Un statement: words è calcolato come il subject label, seguito da uno spazio, seguito da il property label, seguito da uno spazio, seguito da il literal quote, seguito da l'object value, seguito da il literal quote, seguito da il literal type marker, seguito da il property datatype se l'object thing è vuoto, in tutti gli altri casi l'object label. ⚠︎ meccanica <!-- rulespeak:reword --> |
| **DR-55 Source** | Il campo “source” del statement è determinato secondo la seguente priorità:<br>1. il built in source text, se l'indicatore “built in” è attivo;<br>2. in tutti gli altri casi, l'user source text. |
| **DR-56 Subject Property Key** | Un statement: subject property key è calcolato come il subject, seguito da «|», seguito da il property. |
| **DR-57 Latest Sibling Activation Seq** | Un statement: latest sibling activation seq è il valore massimo di activation seq tra i statements collegati a il statement. |
| **DR-58 Is in Effect** | Un statement è considerato in effect se vale tutto quanto segue: l'indicatore “stated” è attivo e vale almeno una delle seguenti: l'indicatore “property has one current value” non è attivo o l'activation seq è il latest sibling activation seq. |
| **DR-59 Listed Unit** | Il campo “listed unit” del statement è determinato secondo la seguente priorità:<br>1. 1, se l'indicatore “listed” è attivo;<br>2. in tutti gli altri casi, 0. |
| **DR-60 In Effect Unit** | Il campo “in effect unit” del statement è determinato secondo la seguente priorità:<br>1. 1, se l'indicatore “in effect” è attivo;<br>2. in tutti gli altri casi, 0. |
| **DR-61 Listed Off Unit** | Il campo “listed off unit” del statement è determinato secondo la seguente priorità:<br>1. 1, se vale tutto quanto segue: l'indicatore “listed” è attivo e l'indicatore “in effect” non è attivo;<br>2. in tutti gli altri casi, 0. |
| **DR-62 Is Next to Add** | Un statement è considerato un next to add se vale tutto quanto segue: il listed off unit è 1 e il sort order è il next fact sort order. |
| **DR-63 Count of Inverse Firings** | Un statement: count of inverse firings è il totale di inverse fired unit tra i firings collegati a il statement. |
| **DR-64 Count of Symmetric Firings** | Un statement: count of symmetric firings è il totale di symmetric fired unit tra i firings collegati a il statement. |
| **DR-65 Holds Before Chain** | Un statement è considerato come soddisfacente: before chain se il IF di il OR di l'is in effect; il count of inverse firings è maggiore di 0; e il count of symmetric firings è maggiore di 0 e vero. |
| **DR-66 Count of Chain Firings** | Un statement: count of chain firings è il totale di chain fired unit tra i firings collegati a il statement. |
| **DR-67 Holds Before Typing** | Un statement è considerato come soddisfacente: before typing se il IF di il OR di il holds before chain e il count of chain firings è maggiore di 0 e vero. |
| **DR-68 Count of Type Firings** | Un statement: count of type firings è il totale di type fired unit tra i firings collegati a il statement. |
| **DR-69 Holds** | Un statement è considerato come soddisfacente: se il IF di il OR di il holds before typing e il count of type firings è maggiore di 0 e vero. |
| **DR-70 Is Conclusion** | Un statement è considerato un conclusion se il IF di il AND di il holds e l'indicatore “in effect” non è attivo e vero. |
| **DR-71 Holds Unit** | Il campo “holds unit” del statement è determinato secondo la seguente priorità:<br>1. 1, se l'indicatore “holds” è attivo;<br>2. in tutti gli altri casi, 0. |
| **DR-72 Conclusion Unit** | Il campo “conclusion unit” del statement è determinato secondo la seguente priorità:<br>1. 1, se l'indicatore “conclusion” è attivo;<br>2. in tutti gli altri casi, 0. |
| **DR-73 Parent Unit** | Il campo “parent unit” del statement è determinato secondo la seguente priorità:<br>1. 1, se vale tutto quanto segue: l'indicatore “holds” è attivo e il property è «isParentOf»;<br>2. in tutti gli altri casi, 0. |
| **DR-74 Grandparent Unit** | Il campo “grandparent unit” del statement è determinato secondo la seguente priorità:<br>1. 1, se vale tutto quanto segue: l'indicatore “holds” è attivo e il property è «isGrandparentOf»;<br>2. in tutti gli altri casi, 0. |
| **DR-75 Place Unit** | Il campo “place unit” del statement è determinato secondo la seguente priorità:<br>1. 1, se vale tutto quanto segue: l'indicatore “holds” è attivo e il property è «attends»;<br>2. in tutti gli altri casi, 0. |
| **DR-76 First Pre Chain Order Key** | Un statement: first pre chain order key è il valore minimo di pre chain order key tra i firings del statement che sono pre chain fired unit. |
| **DR-77 Snap Position** | Il campo “snap position” del statement è determinato secondo la seguente priorità:<br>1. il sort order, se l'indicatore “in effect” è attivo;<br>2. 100000 più il first pre chain order key, se l'indicatore “holds before chain” è attivo;<br>3. in tutti gli altri casi, 0. |
| **DR-78 First Fired Order Key** | Un statement: first fired order key è il valore minimo di order key tra i firings del statement che sono fired unit. |
| **DR-79 Count of Chosen Firings** | Un statement: count of chosen firings è il totale di chosen unit tra i firings collegati a il statement. |
| **DR-80 Subject Node Key** | Un statement: subject node key è uguale a subject. |
| **DR-81 Object Node Key** | Il campo “object node key” del statement è determinato secondo la seguente priorità:<br>1. «literal|», seguito da l'object value, seguito da «|», seguito da il property datatype, se l'object thing è vuoto;<br>2. in tutti gli altri casi, l'object thing. |
| **DR-82 Subject Node Kind** | Un statement: subject node kind — ripreso dal subject collegato. |
| **DR-83 Object Thing Node Kind** | Un statement: object thing node kind — ripreso dal object thing collegato. |
| **DR-84 Object Node Kind** | Il campo “object node kind” del statement è determinato secondo la seguente priorità:<br>1. «value», se l'object thing è vuoto;<br>2. in tutti gli altri casi, l'object thing node kind. |
| **DR-85 Object Value Node Label** | Un statement: object value node label — ripreso dal property collegato. |
| **DR-86 Count of Violations As First** | Un statement: count of violations as first è il totale di violation unit tra i disjoint checks collegati a il statement. |
| **DR-87 Count of Violations As Second** | Un statement: count of violations as second è il totale di violation unit tra i disjoint checks collegati a il statement. |
| **DR-88 Is in Contradiction** | Un statement è considerato in contradiction se vale almeno una delle seguenti: il count of violations as first è maggiore di 0 o il count of violations as second è maggiore di 0. |
| **DR-89 Rule Enabled** | Un firing: rule enabled è vero quando il rule collegato è enabled. |
| **DR-90 Kind Wording** | Un firing: kind wording — ripreso dal kind collegato. |
| **DR-91 Kind Sort Order** | Un firing: kind sort order — ripreso dal kind collegato. |
| **DR-92 Target Name** | Un firing: target name — ripreso dal target collegato. |
| **DR-93 Target Subject Label** | Un firing: target subject label — ripreso dal target collegato. |
| **DR-94 Target Property Label** | Un firing: target property label — ripreso dal target collegato. |
| **DR-95 Target Object Text** | Un firing: target object text — ripreso dal target collegato. |
| **DR-96 Target is Conclusion** | Un firing: target is conclusion è vero quando il target collegato è un conclusion. |
| **DR-97 Target Subject Node Key** | Un firing: target subject node key — ripreso dal target collegato. |
| **DR-98 Target Object Node Key** | Un firing: target object node key — ripreso dal target collegato. |
| **DR-99 Target First Fired Order Key** | Un firing: target first fired order key — ripreso dal target collegato. |
| **DR-100 First Words** | Un firing: first words — ripreso dal first collegato. |
| **DR-101 Second Words** | Un firing: second words — ripreso dal second collegato. |
| **DR-102 First is in Effect** | Un firing: first is in effect è vero quando il first collegato è in effect. |
| **DR-103 First Holds Before Chain** | Un firing: first holds before chain è vero quando il first collegato è holds before chain. |
| **DR-104 Second Holds Before Chain** | Un firing: second holds before chain è vero quando il second collegato è holds before chain. |
| **DR-105 First Holds Before Typing** | Un firing: first holds before typing è vero quando il first collegato è holds before typing. |
| **DR-106 First Property** | Un firing: first property — ripreso dal first collegato. |
| **DR-107 First Property Domain Class** | Un firing: first property domain class — ripreso dal first collegato. |
| **DR-108 First Property Range Class** | Un firing: first property range class — ripreso dal first collegato. |
| **DR-109 First Sort Order** | Un firing: first sort order — ripreso dal first collegato. |
| **DR-110 Second is in Effect** | Un firing: second is in effect è vero quando il second collegato è in effect. |
| **DR-111 First Snap Position** | Un firing: first snap position — ripreso dal first collegato. |
| **DR-112 Second Snap Position** | Un firing: second snap position — ripreso dal second collegato. |
| **DR-113 Inverse Fired Unit** | Il campo “inverse fired unit” del firing è determinato secondo la seguente priorità:<br>1. 1, se vale tutto quanto segue: l'indicatore “rule enabled” è attivo; il kind è «inverse»; e l'indicatore “first is in effect” è attivo;<br>2. in tutti gli altri casi, 0. |
| **DR-114 Symmetric Fired Unit** | Il campo “symmetric fired unit” del firing è determinato secondo la seguente priorità:<br>1. 1, se vale tutto quanto segue: l'indicatore “rule enabled” è attivo; il kind è «symmetric»; e l'indicatore “first is in effect” è attivo;<br>2. in tutti gli altri casi, 0. |
| **DR-115 Chain Fired Unit** | Il campo “chain fired unit” del firing è determinato secondo la seguente priorità:<br>1. 1, se vale tutto quanto segue: l'indicatore “rule enabled” è attivo; il kind è «chain»; l'indicatore “first holds before chain” è attivo; e l'indicatore “second holds before chain” è attivo;<br>2. in tutti gli altri casi, 0. |
| **DR-116 Type Fired Unit** | Il campo “type fired unit” del firing è determinato secondo la seguente priorità:<br>1. 1, se vale tutto quanto segue: l'indicatore “rule enabled” è attivo; vale almeno una delle seguenti: il kind è «domain» o il kind è «range»; e l'indicatore “first holds before typing” è attivo;<br>2. in tutti gli altri casi, 0. |
| **DR-117 Fired Unit** | Un firing: fired unit è calcolato come l'inverse fired unit più il symmetric fired unit più il chain fired unit più il type fired unit. |
| **DR-118 Pre Chain Fired Unit** | Un firing: pre chain fired unit è calcolato come l'inverse fired unit più il symmetric fired unit. |
| **DR-119 Pre Chain Order Key** | Un firing: pre chain order key è calcolato come il first sort order per 10 più il kind sort order. |
| **DR-120 Round** | Il campo “round” del firing è determinato secondo la seguente priorità:<br>1. 2, se vale tutto quanto segue: il kind è «chain» e non è il caso che vale tutto quanto segue: l'indicatore “first is in effect” è attivo e l'indicatore “second is in effect” è attivo;<br>2. in tutti gli altri casi, 1. |
| **DR-121 Order Key** | Un firing: order key è calcolato come il round per 1000000000000000 più il first snap position per 100000000 più il kind sort order per 10000000 più 0 se il second è vuoto, in tutti gli altri casi il second snap position. |
| **DR-122 Is Chosen** | Un firing è considerato un chosen se vale tutto quanto segue: il fired unit è 1; l'indicatore “target is conclusion” è attivo; e l'order key è il target first fired order key. |
| **DR-123 Chosen Unit** | Il campo “chosen unit” del firing è determinato secondo la seguente priorità:<br>1. 1, se l'indicatore “chosen” è attivo;<br>2. in tutti gli altri casi, 0. |
| **DR-124 Card Text** | Il campo “card text” del firing è determinato secondo la seguente priorità:<br>1. «Worked out by the », seguito da il kind wording, seguito da « from “», seguito da il first words, seguito da «”», seguito da « and “», seguito da il second words, seguito da «”» se il kind è «chain», in tutti gli altri casi « and “», seguito da il first property, seguito da « has domain », seguito da il first property domain class, seguito da «”» se il kind è «domain», in tutti gli altri casi « and “», seguito da il first property, seguito da « has range », seguito da il first property range class, seguito da «”», seguito da un punto, se l'indicatore “chosen” è attivo;<br>2. in tutti gli altri casi, una stringa vuota. ⚠︎ meccanica <!-- rulespeak:reword --> |
| **DR-125 Name** | Un firing: name è uguale a firing ID. |
| **DR-126 Rule Enabled** | Un disjoint check: rule enabled è vero quando il rule collegato è enabled. |
| **DR-127 First Holds** | Un disjoint check: first holds è vero quando il first collegato è holds. |
| **DR-128 Second Holds** | Un disjoint check: second holds è vero quando il second collegato è holds. |
| **DR-129 Subject Label** | Un disjoint check: subject label — ripreso dal first collegato. |
| **DR-130 Object Label** | Un disjoint check: object label — ripreso dal first collegato. |
| **DR-131 Violation Unit** | Il campo “violation unit” del disjoint check è determinato secondo la seguente priorità:<br>1. 1, se vale tutto quanto segue: l'indicatore “rule enabled” è attivo; l'indicatore “first holds” è attivo; e l'indicatore “second holds” è attivo;<br>2. in tutti gli altri casi, 0. |
| **DR-132 Alarm Text** | Il campo “alarm text” del disjoint check è determinato secondo la seguente priorità:<br>1. «Contradiction found. », seguito da il subject label, seguito da « is recorded as both parent and spouse of », seguito da l'object label, seguito da «. Check those facts and the selected disjoint rule.», se il violation unit è 1;<br>2. in tutti gli altri casi, una stringa vuota. |
| **DR-133 Name** | Un disjoint check: name è uguale a disjoint check ID. |
| **DR-134 Name** | Un scenario: name è uguale a label. |
| **DR-135 Graph Has Facts to Add** | Un scenario: graph has facts to add è vero quando il graph collegato ha un facts to add. |
| **DR-136 Is Available** | Un scenario è considerato available se vale almeno una delle seguenti: il fact baseline non è «next» o l'indicatore “graph has facts to add” è attivo. |
| **DR-137 Name** | Un scenario fact change: name è uguale a scenario fact change ID. |
| **DR-138 Name** | Un scenario rule setting: name è uguale a scenario rule setting ID. |
| **DR-139 Name** | Un node layout: name è uguale a node layout ID. |
| **DR-140 Name** | Un layout slot: name è uguale a layout slot ID. |
| **DR-141 Name** | Un page text: name è uguale a page text ID. |
| **DR-142 Name** | Un engine: name è uguale a label. |
| **DR-143 Name** | Un rulebook tool: name è uguale a label. |

## 5 Tracciabilità allo schema

_La colonna espressione è la definizione della regola in notazione Documentazione —
la stessa logica che il libro delle regole memorizza, scritta per un lettore di business._

| Elemento dello schema | Tipo | Espressione |
|----------------|------|------------|
| **Graphs.Name** | formula | `Label` |
| **Graphs.CountOfStatedFacts** | aggregato | `Sum(Statements.InEffectUnit via Graph)` |
| **Graphs.CountOfConclusions** | aggregato | `Sum(Statements.ConclusionUnit via Graph)` |
| **Graphs.CountOfViolations** | aggregato | `Sum(DisjointChecks.ViolationUnit via Graph)` |
| **Graphs.IsConsistent** | formula | `CountOfViolations = 0` |
| **Graphs.ConclusionsMessage** | formula | `If(CountOfConclusions = 0, NoConclusionsText, "")` |
| **Graphs.CountOfFactsToAdd** | aggregato | `Sum(Statements.ListedOffUnit via Graph)` |
| **Graphs.HasFactsToAdd** | formula | `CountOfFactsToAdd > 0` |
| **Graphs.NextFactSortOrder** | aggregato | `Min(Statements.SortOrder via Graph)` |
| **Graphs.MaxActivationSeq** | aggregato | `Max(Statements.ActivationSeq via Graph)` |
| **Graphs.NextActivationSeq** | formula | `MaxActivationSeq + 1` |
| **Graphs.MaxListedSortOrder** | aggregato | `Max(Statements.SortOrder via Graph)` |
| **Graphs.NextListedSortOrder** | formula | `MaxListedSortOrder + 1` |
| **Graphs.MaxSortOrder** | aggregato | `Max(Statements.SortOrder via Graph)` |
| **Graphs.NextCandidateSortOrder** | formula | `MaxSortOrder + 1` |
| **Rules.Name** | formula | `Label` |
| **Rules.CountOfFirings** | aggregato | `Sum(Firings.FiredUnit via Rule)` |
| **FiringKinds.Name** | formula | `Wording` |
| **FiringKinds.RuleEnabled** | ricerca | `Lookup(Rules.IsEnabled via Rule)` |
| **Things.PortraitName** | ricerca | `Lookup(Portraits.DisplayName via Portrait)` |
| **Things.PortraitPage** | ricerca | `Lookup(Portraits.Page via Portrait)` |
| **Things.PortraitImage** | ricerca | `Lookup(Portraits.Image via Portrait)` |
| **Things.PortraitCrop** | ricerca | `Lookup(Portraits.Crop via Portrait)` |
| **Things.PortraitWidth** | ricerca | `Lookup(Portraits.Width via Portrait)` |
| **Things.PortraitHeight** | ricerca | `Lookup(Portraits.Height via Portrait)` |
| **Things.Name** | formula | `Label` |
| **Things.NodeKey** | formula | `ThingId` |
| **Things.CountOfChildren** | aggregato | `Sum(Statements.ParentUnit via Subject)` |
| **Things.CountOfGrandchildren** | aggregato | `Sum(Statements.GrandparentUnit via Subject)` |
| **Things.CountAsPlace** | aggregato | `Sum(Statements.PlaceUnit via ObjectThing)` |
| **Things.CountAsSubject** | aggregato | `Sum(Statements.HoldsUnit via Subject)` |
| **Things.CountAsObject** | aggregato | `Sum(Statements.HoldsUnit via ObjectThing)` |
| **Things.IsInGraph** | formula | `Or(CountAsSubject > 0, CountAsObject > 0)` |
| **Things.NodeKind** | formula | `If(Kind = "", "", If(Kind = "class", "class", If(CountAsPlace > 0, "place", "person")))` |
| **Portraits.Name** | formula | `DisplayName` |
| **PortraitAliases.Name** | formula | `PortraitAliasId` |
| **Properties.Name** | formula | `Label` |
| **PropertyAxioms.Name** | formula | `PropertyAxiomId` |
| **Statements.SubjectLabel** | ricerca | `Lookup(Things.Label via Subject)` |
| **Statements.PropertyLabel** | ricerca | `Lookup(Properties.Label via Property)` |
| **Statements.ObjectLabel** | ricerca | `Lookup(Things.Label via ObjectThing)` |
| **Statements.PropertyDatatype** | ricerca | `Lookup(Properties.Datatype via Property)` |
| **Statements.PropertyHasOneCurrentValue** | ricerca | `Lookup(Properties.HasOneCurrentValue via Property)` |
| **Statements.PropertyDomainClass** | ricerca | `Lookup(Properties.DomainClass via Property)` |
| **Statements.PropertyRangeClass** | ricerca | `Lookup(Properties.RangeClass via Property)` |
| **Statements.LiteralQuote** | ricerca | `Lookup(Graphs.LiteralQuote via Graph)` |
| **Statements.LiteralTypeMarker** | ricerca | `Lookup(Graphs.LiteralTypeMarker via Graph)` |
| **Statements.BuiltInSourceText** | ricerca | `Lookup(Graphs.BuiltInSourceText via Graph)` |
| **Statements.UserSourceText** | ricerca | `Lookup(Graphs.UserSourceText via Graph)` |
| **Statements.NextFactSortOrder** | ricerca | `Lookup(Graphs.NextFactSortOrder via Graph)` |
| **Statements.IsLiteral** | formula | `ObjectThing = ""` |
| **Statements.ObjectText** | formula | `If(ObjectThing = "", ObjectValue, ObjectLabel)` |
| **Statements.Name** | formula | `Concat(SubjectLabel, " ", PropertyLabel, " ", ObjectText)` |
| **Statements.Words** | formula | `Concat(SubjectLabel, " ", PropertyLabel, " ", If(ObjectThing = "", Concat(LiteralQuote, ObjectValue, LiteralQuote, LiteralTypeMarker, PropertyDatatype), ObjectLabel))` |
| **Statements.Source** | formula | `If(IsBuiltIn, BuiltInSourceText, UserSourceText)` |
| **Statements.SubjectPropertyKey** | formula | `Concat(Subject, "\|", Property)` |
| **Statements.LatestSiblingActivationSeq** | aggregato | `Max(Statements.ActivationSeq via SubjectPropertyKey)` |
| **Statements.IsInEffect** | formula | `And(IsStated, Or(Not(PropertyHasOneCurrentValue), ActivationSeq = LatestSiblingActivationSeq))` |
| **Statements.ListedUnit** | formula | `If(IsListed, 1, 0)` |
| **Statements.InEffectUnit** | formula | `If(IsInEffect, 1, 0)` |
| **Statements.ListedOffUnit** | formula | `If(And(IsListed, Not(IsInEffect)), 1, 0)` |
| **Statements.IsNextToAdd** | formula | `And(ListedOffUnit = 1, SortOrder = NextFactSortOrder)` |
| **Statements.CountOfInverseFirings** | aggregato | `Sum(Firings.InverseFiredUnit via Target)` |
| **Statements.CountOfSymmetricFirings** | aggregato | `Sum(Firings.SymmetricFiredUnit via Target)` |
| **Statements.HoldsBeforeChain** | formula | `If(Or(IsInEffect, CountOfInverseFirings > 0, CountOfSymmetricFirings > 0), True())` |
| **Statements.CountOfChainFirings** | aggregato | `Sum(Firings.ChainFiredUnit via Target)` |
| **Statements.HoldsBeforeTyping** | formula | `If(Or(HoldsBeforeChain, CountOfChainFirings > 0), True())` |
| **Statements.CountOfTypeFirings** | aggregato | `Sum(Firings.TypeFiredUnit via Target)` |
| **Statements.Holds** | formula | `If(Or(HoldsBeforeTyping, CountOfTypeFirings > 0), True())` |
| **Statements.IsConclusion** | formula | `If(And(Holds, Not(IsInEffect)), True())` |
| **Statements.HoldsUnit** | formula | `If(Holds, 1, 0)` |
| **Statements.ConclusionUnit** | formula | `If(IsConclusion, 1, 0)` |
| **Statements.ParentUnit** | formula | `If(And(Holds, Property = "isParentOf"), 1, 0)` |
| **Statements.GrandparentUnit** | formula | `If(And(Holds, Property = "isGrandparentOf"), 1, 0)` |
| **Statements.PlaceUnit** | formula | `If(And(Holds, Property = "attends"), 1, 0)` |
| **Statements.FirstPreChainOrderKey** | aggregato | `Min(Firings.PreChainOrderKey via Target)` |
| **Statements.SnapPosition** | formula | `If(IsInEffect, SortOrder, If(HoldsBeforeChain, 100000 + FirstPreChainOrderKey, 0))` |
| **Statements.FirstFiredOrderKey** | aggregato | `Min(Firings.OrderKey via Target)` |
| **Statements.CountOfChosenFirings** | aggregato | `Sum(Firings.ChosenUnit via Target)` |
| **Statements.SubjectNodeKey** | formula | `Subject` |
| **Statements.ObjectNodeKey** | formula | `If(ObjectThing = "", Concat("literal\|", ObjectValue, "\|", PropertyDatatype), ObjectThing)` |
| **Statements.SubjectNodeKind** | ricerca | `Lookup(Things.NodeKind via Subject)` |
| **Statements.ObjectThingNodeKind** | ricerca | `Lookup(Things.NodeKind via ObjectThing)` |
| **Statements.ObjectNodeKind** | formula | `If(ObjectThing = "", "value", ObjectThingNodeKind)` |
| **Statements.ObjectValueNodeLabel** | ricerca | `Lookup(Properties.ValueNodeLabel via Property)` |
| **Statements.CountOfViolationsAsFirst** | aggregato | `Sum(DisjointChecks.ViolationUnit via First)` |
| **Statements.CountOfViolationsAsSecond** | aggregato | `Sum(DisjointChecks.ViolationUnit via Second)` |
| **Statements.IsInContradiction** | formula | `Or(CountOfViolationsAsFirst > 0, CountOfViolationsAsSecond > 0)` |
| **Firings.RuleEnabled** | ricerca | `Lookup(Rules.IsEnabled via Rule)` |
| **Firings.KindWording** | ricerca | `Lookup(FiringKinds.Wording via Kind)` |
| **Firings.KindSortOrder** | ricerca | `Lookup(FiringKinds.SortOrder via Kind)` |
| **Firings.TargetName** | ricerca | `Lookup(Statements.Name via Target)` |
| **Firings.TargetSubjectLabel** | ricerca | `Lookup(Statements.SubjectLabel via Target)` |
| **Firings.TargetPropertyLabel** | ricerca | `Lookup(Statements.PropertyLabel via Target)` |
| **Firings.TargetObjectText** | ricerca | `Lookup(Statements.ObjectText via Target)` |
| **Firings.TargetIsConclusion** | ricerca | `Lookup(Statements.IsConclusion via Target)` |
| **Firings.TargetSubjectNodeKey** | ricerca | `Lookup(Statements.SubjectNodeKey via Target)` |
| **Firings.TargetObjectNodeKey** | ricerca | `Lookup(Statements.ObjectNodeKey via Target)` |
| **Firings.TargetFirstFiredOrderKey** | ricerca | `Lookup(Statements.FirstFiredOrderKey via Target)` |
| **Firings.FirstWords** | ricerca | `Lookup(Statements.Words via First)` |
| **Firings.SecondWords** | ricerca | `Lookup(Statements.Words via Second)` |
| **Firings.FirstIsInEffect** | ricerca | `Lookup(Statements.IsInEffect via First)` |
| **Firings.FirstHoldsBeforeChain** | ricerca | `Lookup(Statements.HoldsBeforeChain via First)` |
| **Firings.SecondHoldsBeforeChain** | ricerca | `Lookup(Statements.HoldsBeforeChain via Second)` |
| **Firings.FirstHoldsBeforeTyping** | ricerca | `Lookup(Statements.HoldsBeforeTyping via First)` |
| **Firings.FirstProperty** | ricerca | `Lookup(Statements.Property via First)` |
| **Firings.FirstPropertyDomainClass** | ricerca | `Lookup(Statements.PropertyDomainClass via First)` |
| **Firings.FirstPropertyRangeClass** | ricerca | `Lookup(Statements.PropertyRangeClass via First)` |
| **Firings.FirstSortOrder** | ricerca | `Lookup(Statements.SortOrder via First)` |
| **Firings.SecondIsInEffect** | ricerca | `Lookup(Statements.IsInEffect via Second)` |
| **Firings.FirstSnapPosition** | ricerca | `Lookup(Statements.SnapPosition via First)` |
| **Firings.SecondSnapPosition** | ricerca | `Lookup(Statements.SnapPosition via Second)` |
| **Firings.InverseFiredUnit** | formula | `If(And(RuleEnabled, Kind = "inverse", FirstIsInEffect), 1, 0)` |
| **Firings.SymmetricFiredUnit** | formula | `If(And(RuleEnabled, Kind = "symmetric", FirstIsInEffect), 1, 0)` |
| **Firings.ChainFiredUnit** | formula | `If(And(RuleEnabled, Kind = "chain", FirstHoldsBeforeChain, SecondHoldsBeforeChain), 1, 0)` |
| **Firings.TypeFiredUnit** | formula | `If(And(RuleEnabled, Or(Kind = "domain", Kind = "range"), FirstHoldsBeforeTyping), 1, 0)` |
| **Firings.FiredUnit** | formula | `InverseFiredUnit + SymmetricFiredUnit + ChainFiredUnit + TypeFiredUnit` |
| **Firings.PreChainFiredUnit** | formula | `InverseFiredUnit + SymmetricFiredUnit` |
| **Firings.PreChainOrderKey** | formula | `FirstSortOrder * 10 + KindSortOrder` |
| **Firings.Round** | formula | `If(And(Kind = "chain", Not(And(FirstIsInEffect, SecondIsInEffect))), 2, 1)` |
| **Firings.OrderKey** | formula | `Round * 1000000000000000 + FirstSnapPosition * 100000000 + KindSortOrder * 10000000 + If(Second = "", 0, SecondSnapPosition)` |
| **Firings.IsChosen** | formula | `And(FiredUnit = 1, TargetIsConclusion, OrderKey = TargetFirstFiredOrderKey)` |
| **Firings.ChosenUnit** | formula | `If(IsChosen, 1, 0)` |
| **Firings.CardText** | formula | `If(IsChosen, Concat("Worked out by the ", KindWording, " from “", FirstWords, "”", If(Kind = "chain", Concat(" and “", SecondWords, "”"), If(Kind = "domain", Concat(" and “", FirstProperty, " has domain ", FirstPropertyDomainClass, "”"), If(Kind = "range", Concat(" and “", FirstProperty, " has range ", FirstPropertyRangeClass, "”"), ""))), "."), "")` |
| **Firings.Name** | formula | `FiringId` |
| **DisjointChecks.RuleEnabled** | ricerca | `Lookup(Rules.IsEnabled via Rule)` |
| **DisjointChecks.FirstHolds** | ricerca | `Lookup(Statements.Holds via First)` |
| **DisjointChecks.SecondHolds** | ricerca | `Lookup(Statements.Holds via Second)` |
| **DisjointChecks.SubjectLabel** | ricerca | `Lookup(Statements.SubjectLabel via First)` |
| **DisjointChecks.ObjectLabel** | ricerca | `Lookup(Statements.ObjectLabel via First)` |
| **DisjointChecks.ViolationUnit** | formula | `If(And(RuleEnabled, FirstHolds, SecondHolds), 1, 0)` |
| **DisjointChecks.AlarmText** | formula | `If(ViolationUnit = 1, Concat("Contradiction found. ", SubjectLabel, " is recorded as both parent and spouse of ", ObjectLabel, ". Check those facts and the selected disjoint rule."), "")` |
| **DisjointChecks.Name** | formula | `DisjointCheckId` |
| **Scenarios.Name** | formula | `Label` |
| **Scenarios.GraphHasFactsToAdd** | ricerca | `Lookup(Graphs.HasFactsToAdd via Graph)` |
| **Scenarios.IsAvailable** | formula | `Or(FactBaseline <> "next", GraphHasFactsToAdd)` |
| **ScenarioFactChanges.Name** | formula | `ScenarioFactChangeId` |
| **ScenarioRuleSettings.Name** | formula | `ScenarioRuleSettingId` |
| **NodeLayouts.Name** | formula | `NodeLayoutId` |
| **LayoutSlots.Name** | formula | `LayoutSlotId` |
| **PageTexts.Name** | formula | `PageTextId` |
| **Engines.Name** | formula | `Label` |
| **RulebookTools.Name** | formula | `Label` |

---

> **RuleSpeak®** — la notazione dichiarativa delle regole di business creata da **Ronald G. Ross**, secondo le convenzioni di **SBVR** — arriverà presto. Fino ad allora questa pagina mostra la documentazione normale. [www.RonRoss.info](https://www.RonRoss.info)
