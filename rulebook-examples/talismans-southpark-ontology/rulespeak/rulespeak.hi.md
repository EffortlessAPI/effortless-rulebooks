# 📘 Talisman's South Park ontology explainer — दस्तावेज़ीकरण

_Jessica Talisman's South Park Ontology Explainer, as a rulebook: the page's ten facts, five rule switches, property wiring, provenance wording and current-value policy are rows, and every conclusion is a formula. The rules apply in three fixed strata (inverse and symmetric, then the property chain, then domain and range); property wiring that feeds an earlier stratum would need repeated rounds. Every fact that could hold over the cast is a candidate row; app/lib/candidates.mjs writes the candidates when a thing, value or property is added._

> नियम-पुस्तिका से प्रस्तुत घोषणात्मक व्यावसायिक नियम। नीचे दिया गया हर कथन
> व्यावसायिक क्षेत्र का सत्य व्यक्त करता है — यह न कोई प्रक्रिया है, न कोई आदेश।
> नियम-पुस्तिका के सूत्र ही सत्य के एकमात्र स्रोत हैं; यह दस्तावेज़ उनका
> सरल भाषा में पठन है।

## 1 व्यावसायिक शब्दावली

| पद | विवरण | व्याख्यात्मक टिप्पणी |
|------|-------------|-------------------|
| **Graph** | The explainer page as one graph: its wording variables and its totals. | — |
| Label | एक परिभाषित विशेषता। | _Display name of the graph._ |
| Literal Quote | एक परिभाषित विशेषता। | _The quote mark written around a literal value in a fact's wording._ |
| Literal Type Marker | एक परिभाषित विशेषता। | _The marker written between a literal value and its datatype, as in Turtle._ |
| Built in Source Text | एक परिभाषित विशेषता। | _Source line shown for a fact the page states as a teaching example._ |
| User Source Text | एक परिभाषित विशेषता। | _Source line shown for a fact the reader stated._ |
| No Conclusions Text | एक परिभाषित विशेषता। | _Message shown when the selected facts and rules add no conclusion._ |
| Name | उसके “label” के समान। | _Order 1. Display name of the graph._ |
| Count of Stated Facts | Graph से संबंधित “statements” पर कुल “in effect unit”। | _Order 5. Facts that are switched on and in effect._ |
| Count of Conclusions | Graph से संबंधित “statements” पर कुल “conclusion unit”। | _Order 18. Facts that hold only because an enabled rule concluded them._ |
| Count of Violations | Graph से संबंधित “disjoint checks” पर कुल “violation unit”। | _Order 18. Disjointness checks that found both links between the same pair._ |
| Is Consistent | तब सत्य, जब count of violations 0 है। | _Order 19. True when no enabled disjointness rule is violated._ |
| Conclusions Message | प्राथमिकता से निर्धारित: no conclusions text, यदि count of conclusions 0 है; अन्य सभी स्थितियों में, एक खाली स्ट्रिंग। | _Order 19. The empty-state message for the conclusions panel, blank when there are conclusions._ |
| Count of Facts to Add | Graph से संबंधित “statements” पर कुल “listed off unit”। | _Order 5. Listed facts that are not in effect; the Add next fact button is enabled while this is above zero._ |
| Has Facts to Add | तब सत्य, जब count of facts to add 0 से अधिक है। | _Order 6. True while some listed fact can still be added._ |
| Next Fact Sort Order | Graph के उन “statements” पर न्यूनतम “sort order”, जो “listed off unit” हैं। | _Order 5. List position of the first listed fact that is not in effect._ |
| Max Activation Seq | Graph से संबंधित “statements” पर अधिकतम “activation seq”। | _Order 1. Highest activation number used so far._ |
| Next Activation Seq | max activation seq जोड़ 1 के रूप में गणना किया जाता है। | _Order 2. The activation number the next switched-on fact takes._ |
| Max Listed Sort Order | Graph के उन “statements” पर अधिकतम “sort order”, जो “listed unit” हैं। | _Order 2. Highest list position among listed facts._ |
| Next Listed Sort Order | max listed sort order जोड़ 1 के रूप में गणना किया जाता है। | _Order 3. The list position a newly added fact takes._ |
| Max Sort Order | Graph से संबंधित “statements” पर अधिकतम “sort order”। | _Order 1. Highest sort order of any fact, listed or candidate._ |
| Next Candidate Sort Order | max sort order जोड़ 1 के रूप में गणना किया जाता है। | _Order 2. The sort order a new candidate fact, or a fact removed from the list, takes._ |

<details open>
<summary>उदाहरण डेटा</summary>

| Label | Literal Quote | Literal Type Marker | Built in Source Text | User Source Text | No Conclusions Text | Name ƒ | Count of Stated Facts ƒ | Count of Conclusions ƒ | Count of Violations ƒ | Is Consistent ƒ | Conclusions Message ƒ | Count of Facts to Add ƒ | Has Facts to Add ƒ | Next Fact Sort Order ƒ | Max Activation Seq ƒ | Next Activation Seq ƒ | Max Listed Sort Order ƒ | Next Listed Sort Order ƒ | Max Sort Order ƒ | Next Candidate Sort Order ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| South Park | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | These facts and selected rules add no further conclusions. Try another fact or rule. | South Park | 10 | 0 | 0 | true | These facts and selected rules add no further conclusions. Try another fact or rule. | 0 | false | — | 10 | 11 | 10 | 11 | 100269 | 100270 |

_ƒ परिकलित स्तंभ को चिह्नित करता है।_

</details>

| पद | विवरण | व्याख्यात्मक टिप्पणी |
|------|-------------|-------------------|
| **Rule** | The five rule switches on the page. | — |
| Label | एक परिभाषित विशेषता। | _The rule's name on the page._ |
| Explanation | एक परिभाषित विशेषता। | _The rule's one-line explanation on the page._ |
| Is Enabled | तब सत्य, जब एक खाली स्ट्रिंग। | _The rule's checkbox._ |
| Sort Order | एक परिभाषित विशेषता। | _Position in the rule list._ |
| Name | उसके “label” के समान। | _Order 1. The rule's name._ |
| Count of Firings | Rule से संबंधित “firings” पर कुल “fired unit”। | _Order 15. Rule applications that fired, counting every derivation._ |

<details open>
<summary>उदाहरण डेटा</summary>

| Label | Explanation | Is Enabled | Sort Order | Name ƒ | Count of Firings ƒ |
|---|---|---|---|---|---|
| Inverse | A parent link also gives a child link between the same two people. | false | 1 | Inverse | 0 |
| Symmetric | A friend link or a spouse link is true for both people. | false | 2 | Symmetric | 0 |
| Property chain | A parent of a parent is a grandparent. | false | 3 | Property chain | 0 |

_ƒ परिकलित स्तंभ को चिह्नित करता है।_

</details>

| पद | विवरण | व्याख्यात्मक टिप्पणी |
|------|-------------|-------------------|
| **Firing Kind** | The ways a rule can apply. Domain and range belong to one rule but are worded apart; 'disjoint' names the rule the disjointness checks answer to. | — |
| Rule | एक परिभाषित विशेषता। | _The rule switch this kind belongs to._ |
| Wording | एक परिभाषित विशेषता। | _How a conclusion card names the rule, as in 'Worked out by the inverse rule'._ |
| Sort Order | एक परिभाषित विशेषता। | _Order in which the page's reasoner tries the kinds for one fact._ |
| Name | उसके “wording” के समान। | _Order 1. The kind's wording._ |
| Rule Enabled | तब सत्य, जब firing kind के “rule” का “enabled” है। | _Order 1. Whether the owning rule is switched on._ |

<details open>
<summary>उदाहरण डेटा</summary>

| Wording | Sort Order | Name ƒ | Rule Enabled ƒ |
|---|---|---|---|
| inverse rule | 1 | inverse rule | false |
| symmetric rule | 2 | symmetric rule | false |
| property chain rule | 3 | property chain rule | false |

_ƒ परिकलित स्तंभ को चिह्नित करता है।_

</details>

| पद | विवरण | व्याख्यात्मक टिप्पणी |
|------|-------------|-------------------|
| **Thing** | Every individual and class the page can draw. | — |
| Label | एक परिभाषित विशेषता। | _The name the page shows._ |
| Kind | एक परिभाषित विशेषता। | _'individual' or 'class'._ |
| Sort Order | एक परिभाषित विशेषता। | _Position in the cast._ |
| Portrait | एक परिभाषित विशेषता। | _The portrait drawn for this thing; set when its name matches a portrait alias._ |
| Portrait Name | thing के “portrait” का “display name”। | _Order 1. The name on the portrait's Fandom page._ |
| Portrait Page | लिंक किए गए “portrait” से लिया जाता है। | _Order 1. The Fandom page credited for the portrait._ |
| Portrait Image | लिंक किए गए “portrait” से लिया जाता है। | _Order 1. The original Fandom image._ |
| Portrait Crop | लिंक किए गए “portrait” से लिया जाता है। | _Order 1. The portrait's crop as an SVG viewBox._ |
| Portrait Width | लिंक किए गए “portrait” से लिया जाता है। | _Order 1. The original image's width in pixels._ |
| Portrait Height | लिंक किए गए “portrait” से लिया जाता है। | _Order 1. The original image's height in pixels._ |
| Name | उसके “label” के समान। | _Order 1. The thing's name._ |
| Node Key | उसके “thing ID” के समान। | _Order 1. The key the graph draws this thing under._ |
| Count of Children | Thing से संबंधित “statements” पर कुल “parent unit”। | _Order 17. People this thing is a parent of, stated or concluded._ |
| Count of Grandchildren | Thing से संबंधित “statements” पर कुल “grandparent unit”। | _Order 17. People this thing is a grandparent of._ |
| Count As Place | Thing से संबंधित “statements” पर कुल “place unit”। | _Order 17. Holding attends facts whose object is this thing._ |
| Count As Subject | Thing से संबंधित “statements” पर कुल “holds unit”। | _Order 17. Holding facts about this thing._ |
| Count As Object | Thing से संबंधित “statements” पर कुल “holds unit”। | _Order 17. Holding facts that point at this thing._ |
| Is in Graph | तब सत्य, जब निम्नलिखित में से कम से कम एक सही है: count as subject 0 से अधिक है या count as object 0 से अधिक है। | _Order 18. True when the graph draws this thing._ |
| Node Kind | प्राथमिकता से निर्धारित: एक खाली स्ट्रिंग, यदि kind खाली है; “class”, यदि kind “class” है; “place”, यदि count as place 0 से अधिक है; अन्य सभी स्थितियों में, “person”। | _Order 18. How the graph draws the thing: a class, a place someone attends, or a person._ |

<details open>
<summary>उदाहरण डेटा</summary>

| Label | Kind | Sort Order | Portrait Name ƒ | Portrait Page ƒ | Portrait Image ƒ | Portrait Crop ƒ | Portrait Width ƒ | Portrait Height ƒ | Name ƒ | Node Key ƒ | Count of Children ƒ | Count of Grandchildren ƒ | Count As Place ƒ | Count As Subject ƒ | Count As Object ƒ | Is in Graph ƒ | Node Kind ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| Marvin | individual | 1 | Marvin Marsh | https://southpark.fandom.com/wiki/Marvin_Marsh | https://static.wikia.nocookie.net/southpark/images/d/db/Marsh-family-grandpa-marsh-w-purple-blanket.png/revision/latest | 48 -5 280 280 | 375 | 536 | Marvin | Marvin | 1 | 0 | 0 | 1 | 0 | true | person |
| Randy | individual | 2 | Randy Marsh | https://southpark.fandom.com/wiki/Randy_Marsh | https://static.wikia.nocookie.net/southparkfanon/images/d/d6/Randy_Marsh.png/revision/latest | 0 -10 428 428 | 428 | 1057 | Randy | Randy | 2 | 0 | 0 | 3 | 1 | true | person |
| Sharon | individual | 3 | Sharon Marsh | https://southpark.fandom.com/wiki/Sharon_Marsh | https://static.wikia.nocookie.net/southpark/images/9/99/Sharon_Marsh.png/revision/latest | 20 -10 390 390 | 439 | 1049 | Sharon | Sharon | 0 | 0 | 0 | 0 | 1 | true | person |

_ƒ परिकलित स्तंभ को चिह्नित करता है।_

</details>

| पद | विवरण | व्याख्यात्मक टिप्पणी |
|------|-------------|-------------------|
| **Portrait** | The page's Fandom portraits, including those for characters a reader may add. | — |
| Display Name | एक परिभाषित विशेषता। | _The name on the portrait's Fandom page._ |
| Page | एक परिभाषित विशेषता। | _The Fandom page credited for the portrait._ |
| Image | एक परिभाषित विशेषता। | _The original Fandom image._ |
| Crop | एक परिभाषित विशेषता। | _The crop as an SVG viewBox: x y width height._ |
| Width | एक परिभाषित विशेषता। | _The original image's width in pixels._ |
| Height | एक परिभाषित विशेषता। | _The original image's height in pixels._ |
| Sort Order | एक परिभाषित विशेषता। | _Position in the credits._ |
| Name | उसके “display name” के समान। | _Order 1. The portrait's name._ |

<details open>
<summary>उदाहरण डेटा</summary>

| Display Name | Page | Image | Crop | Width | Height | Sort Order | Name ƒ |
|---|---|---|---|---|---|---|---|
| Marvin Marsh | https://southpark.fandom.com/wiki/Marvin_Marsh | https://static.wikia.nocookie.net/southpark/images/d/db/Marsh-family-grandpa-marsh-w-purple-blanket.png/revision/latest | 48 -5 280 280 | 375 | 536 | 1 | Marvin Marsh |
| Randy Marsh | https://southpark.fandom.com/wiki/Randy_Marsh | https://static.wikia.nocookie.net/southparkfanon/images/d/d6/Randy_Marsh.png/revision/latest | 0 -10 428 428 | 428 | 1057 | 2 | Randy Marsh |
| Sharon Marsh | https://southpark.fandom.com/wiki/Sharon_Marsh | https://static.wikia.nocookie.net/southpark/images/9/99/Sharon_Marsh.png/revision/latest | 20 -10 390 390 | 439 | 1049 | 3 | Sharon Marsh |

_ƒ परिकलित स्तंभ को चिह्नित करता है।_

</details>

| पद | विवरण | व्याख्यात्मक टिप्पणी |
|------|-------------|-------------------|
| **Portrait Aliase** | The names a portrait answers to. A new thing whose name matches an alias gets that portrait. | — |
| Portrait | एक परिभाषित विशेषता। | _The portrait this name selects._ |
| Name | उसके “portrait alias ID” के समान। | _Order 1. The alias._ |

<details open>
<summary>उदाहरण डेटा</summary>

| Name ƒ |
|---|
| marvin |
| marvin marsh |
| grandpa marsh |

_ƒ परिकलित स्तंभ को चिह्नित करता है।_

</details>

| पद | विवरण | व्याख्यात्मक टिप्पणी |
|------|-------------|-------------------|
| **Property** | The ten properties, with what the symmetric, domain and range rules read. Axioms that relate one property to another (inverse, chain, disjoint) are rows of PropertyAxioms. | — |
| Label | एक परिभाषित विशेषता। | _The property's wording on the page._ |
| Property Kind | एक परिभाषित विशेषता। | _'object' links two things, 'datatype' links a thing to a value, 'type' states class membership._ |
| Datatype | एक परिभाषित विशेषता। | _The value's XML Schema datatype for a datatype property; blank otherwise._ |
| Is User Addable | तब सत्य, जब एक खाली स्ट्रिंग। | _Offered in the page's Add fact form._ |
| Has One Current Value | तब सत्य, जब एक खाली स्ट्रिंग। | _The page keeps one current value per person: switching a value on replaces the earlier one._ |
| Is Symmetric | तब सत्य, जब एक खाली स्ट्रिंग। | _The symmetric rule makes this link true in both directions._ |
| Domain Class | एक परिभाषित विशेषता। | _The class the domain rule gives this property's subject._ |
| Range Class | एक परिभाषित विशेषता। | _The class the range rule gives this property's object._ |
| Sort Order | एक परिभाषित विशेषता। | _Position in the Add fact form's property list._ |
| Value Node Label | एक परिभाषित विशेषता। | _The small label under a value node for this property: 'role value' or 'number value'._ |
| Current Value Wording | एक परिभाषित विशेषता। | _How the Add fact form names a replaced value, as in 'Stan now has grade level 5'._ |
| Input Name | एक परिभाषित विशेषता। | _How the integer check names this property's value, as in 'for grade level'._ |
| Record Kind | एक परिभाषित विशेषता। | _The word the current-record summary uses for this property's values: role, grade or school._ |
| Record Order | एक परिभाषित विशेषता। | _Position of this property in the current-record summary._ |
| Name | उसके “label” के समान। | _Order 1. The property's wording._ |

<details open>
<summary>उदाहरण डेटा</summary>

| Label | Property Kind | Datatype | Is User Addable | Has One Current Value | Is Symmetric | Sort Order | Value Node Label | Current Value Wording | Input Name | Record Kind | Record Order | Name ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|
| is parent of | object | — | true | false | false | 1 | — | — | — | — | — | is parent of |
| is child of | object | — | true | false | false | 2 | — | — | — | — | — | is child of |
| is spouse of | object | — | true | false | true | 3 | — | — | — | — | — | is spouse of |

_ƒ परिकलित स्तंभ को चिह्नित करता है।_

</details>

| पद | विवरण | व्याख्यात्मक टिप्पणी |
|------|-------------|-------------------|
| **Property Axiom** | Axioms that relate one property to others, as OWL states them: an inverse, a two-link chain, or a disjoint pair. The candidate generator reads these rows. | — |
| Kind | एक परिभाषित विशेषता। | _inverse, chain or disjoint._ |
| Property | एक परिभाषित विशेषता। | _The property the axiom is about: the one inverted, the one a chain derives, or the first of a disjoint pair._ |
| First | एक परिभाषित विशेषता। | _The inverse property, the chain's first link, or the other property of a disjoint pair._ |
| Second | एक परिभाषित विशेषता। | _The chain's second link._ |
| Name | उसके “property axiom ID” के समान। | _Order 1. The axiom's key._ |

<details open>
<summary>उदाहरण डेटा</summary>

| Name ƒ |
|---|
| inverse:isParentOf |
| disjoint:isParentOf~isSpouseOf |
| inverse:isChildOf |

_ƒ परिकलित स्तंभ को चिह्नित करता है।_

</details>

| पद | विवरण | व्याख्यात्मक टिप्पणी |
|------|-------------|-------------------|
| **Statement** | Every fact the page can state or conclude about the cast. A listed fact appears in the page's fact list; the rest are candidates that only a rule can make hold. | — |
| Graph | एक परिभाषित विशेषता। | _The graph the fact belongs to._ |
| Subject | एक परिभाषित विशेषता। | _What the fact describes._ |
| Property | एक परिभाषित विशेषता। | _How the subject connects to the object._ |
| Object Thing | एक परिभाषित विशेषता। | _The related thing, for an object or type property._ |
| Object Value | एक परिभाषित विशेषता। | _The literal value, for a datatype property._ |
| Is Listed | तब सत्य, जब एक खाली स्ट्रिंग। | _Shown in the page's fact list._ |
| Is Built in | तब सत्य, जब एक खाली स्ट्रिंग। | _One of the page's teaching facts; the reader cannot remove it._ |
| Is Detail | तब सत्य, जब एक खाली स्ट्रिंग। | _One of Stan's detail facts that Show Stan's details switches on._ |
| Is Stated | तब सत्य, जब एक खाली स्ट्रिंग। | _The fact's checkbox._ |
| Activation Seq | एक परिभाषित विशेषता। | _When the fact was last switched on, as a running number; decides which value is current._ |
| Sort Order | एक परिभाषित विशेषता। | _Position in the fact list; candidates follow the listed facts._ |
| Subject Label | लिंक किए गए “subject” से लिया जाता है। | _Order 1. The subject's name._ |
| Property Label | लिंक किए गए “property” से लिया जाता है। | _Order 1. The property's wording._ |
| Object Label | लिंक किए गए “object thing” से लिया जाता है। | _Order 1. The object's name, for an object or type property._ |
| Property Datatype | लिंक किए गए “property” से लिया जाता है। | _Order 1. The value's datatype, for a datatype property._ |
| Property Has One Current Value | तब सत्य, जब statement के “property” का “one current value” रखता है। | _Order 1. Whether the property keeps one current value per person._ |
| Property Domain Class | लिंक किए गए “property” से लिया जाता है। | _Order 1. The property's domain class._ |
| Property Range Class | लिंक किए गए “property” से लिया जाता है। | _Order 1. The property's range class._ |
| Literal Quote | लिंक किए गए “graph” से लिया जाता है। | _Order 1. The graph's literal quote mark._ |
| Literal Type Marker | लिंक किए गए “graph” से लिया जाता है। | _Order 1. The graph's datatype marker._ |
| Built in Source Text | लिंक किए गए “graph” से लिया जाता है। | _Order 1. The graph's source line for a teaching fact._ |
| User Source Text | लिंक किए गए “graph” से लिया जाता है। | _Order 1. The graph's source line for a reader's fact._ |
| Next Fact Sort Order | लिंक किए गए “graph” से लिया जाता है। | _Order 6. List position of the next fact to add._ |
| Is Literal | तब सत्य, जब object thing खाली है। | _Order 1. True when the object is a value rather than a thing._ |
| Object Text | प्राथमिकता से निर्धारित: object value, यदि object thing खाली है; अन्य सभी स्थितियों में, object label। | _Order 2. The object's name or the value._ |
| Name | subject label, उसके बाद एक रिक्त स्थान, उसके बाद property label, उसके बाद एक रिक्त स्थान, उसके बाद object text के रूप में गणना किया जाता है। | _Order 3. The fact in words._ |
| Words | subject label, उसके बाद एक रिक्त स्थान, उसके बाद property label, उसके बाद एक रिक्त स्थान, उसके बाद literal quote, उसके बाद object value, उसके बाद literal quote, उसके बाद literal type marker, उसके बाद property datatype, यदि object thing खाली है; अन्य सभी स्थितियों में object label के रूप में गणना किया जाता है। ⚠︎ यांत्रिक <!-- rulespeak:reword --> | _Order 2. The fact as a conclusion card quotes it, with a literal written in Turtle form._ |
| Source | प्राथमिकता से निर्धारित: built in source text, यदि “built in” ध्वज लगा हुआ है; अन्य सभी स्थितियों में, user source text। | _Order 2. Where a listed fact came from._ |
| Subject Property Key | subject, उसके बाद “|”, उसके बाद property के रूप में गणना किया जाता है। | _Order 1. Groups the values one subject has for one property._ |
| Latest Sibling Activation Seq | Statement से संबंधित “statements” पर अधिकतम “activation seq”। | _Order 2. The most recent activation among this subject's values for this property._ |
| Is in Effect | तब सत्य, जब निम्नलिखित सभी सही हैं: “stated” ध्वज लगा हुआ है और निम्नलिखित में से कम से कम एक सही है: “property has one current value” ध्वज लगा हुआ नहीं है या activation seq latest sibling activation seq है। | _Order 3. Switched on and, for a one-value property, the most recently switched-on value. Drawn as a solid line._ |
| Listed Unit | प्राथमिकता से निर्धारित: 1, यदि “listed” ध्वज लगा हुआ है; अन्य सभी स्थितियों में, 0। | _Order 1. 1 when the fact is in the fact list._ |
| In Effect Unit | प्राथमिकता से निर्धारित: 1, यदि “in effect” ध्वज लगा हुआ है; अन्य सभी स्थितियों में, 0। | _Order 4. 1 when the fact is in effect._ |
| Listed Off Unit | प्राथमिकता से निर्धारित: 1, यदि निम्नलिखित सभी सही हैं: “listed” ध्वज लगा हुआ है और “in effect” ध्वज लगा हुआ नहीं है; अन्य सभी स्थितियों में, 0। | _Order 4. 1 when the fact is listed but not in effect._ |
| Is Next to Add | तब सत्य, जब निम्नलिखित सभी सही हैं: listed off unit 1 है और sort order next fact sort order है। | _Order 7. The fact the Add next fact button switches on._ |
| Count of Inverse Firings | Statement से संबंधित “firings” पर कुल “inverse fired unit”। | _Order 6. Inverse rule applications that conclude this fact._ |
| Count of Symmetric Firings | Statement से संबंधित “firings” पर कुल “symmetric fired unit”। | _Order 6. Symmetric rule applications that conclude this fact._ |
| Holds Before Chain | तब सत्य, जब निम्नलिखित में से कम से कम एक सही है: “in effect” ध्वज लगा हुआ है, count of inverse firings 0 से अधिक है या count of symmetric firings 0 से अधिक है। | _Order 7. Holds from facts in effect plus the inverse and symmetric rules._ |
| Count of Chain Firings | Statement से संबंधित “firings” पर कुल “chain fired unit”। | _Order 10. Property chain applications that conclude this fact._ |
| Holds Before Typing | तब सत्य, जब निम्नलिखित में से कम से कम एक सही है: “holds before chain” ध्वज लगा हुआ है या count of chain firings 0 से अधिक है। | _Order 11. Holds before the domain and range rules apply._ |
| Count of Type Firings | Statement से संबंधित “firings” पर कुल “type fired unit”। | _Order 14. Domain and range applications that conclude this fact._ |
| Holds | तब सत्य, जब निम्नलिखित में से कम से कम एक सही है: “holds before typing” ध्वज लगा हुआ है या count of type firings 0 से अधिक है। | _Order 15. True when in effect or concluded by an enabled rule; the graph draws it._ |
| Is Conclusion | तब सत्य, जब निम्नलिखित सभी सही हैं: “holds” ध्वज लगा हुआ है और “in effect” ध्वज लगा हुआ नहीं है। | _Order 16. Holds only because a rule concluded it. Drawn dashed._ |
| Holds Unit | प्राथमिकता से निर्धारित: 1, यदि “holds” ध्वज लगा हुआ है; अन्य सभी स्थितियों में, 0। | _Order 16. 1 when the fact holds._ |
| Conclusion Unit | प्राथमिकता से निर्धारित: 1, यदि “conclusion” ध्वज लगा हुआ है; अन्य सभी स्थितियों में, 0। | _Order 17. 1 when the fact is a conclusion._ |
| Parent Unit | प्राथमिकता से निर्धारित: 1, यदि निम्नलिखित सभी सही हैं: “holds” ध्वज लगा हुआ है और property “isParentOf” है; अन्य सभी स्थितियों में, 0। | _Order 16. 1 for a holding parent link._ |
| Grandparent Unit | प्राथमिकता से निर्धारित: 1, यदि निम्नलिखित सभी सही हैं: “holds” ध्वज लगा हुआ है और property “isGrandparentOf” है; अन्य सभी स्थितियों में, 0। | _Order 16. 1 for a holding grandparent link._ |
| Place Unit | प्राथमिकता से निर्धारित: 1, यदि निम्नलिखित सभी सही हैं: “holds” ध्वज लगा हुआ है और property “attends” है; अन्य सभी स्थितियों में, 0। | _Order 16. 1 for a holding attends link._ |
| First Pre Chain Order Key | Statement के उन “firings” पर न्यूनतम “pre chain order key”, जो “pre chain fired unit” हैं। | _Order 7. Order key of the first inverse or symmetric application that concludes this fact._ |
| Snap Position | प्राथमिकता से निर्धारित: sort order, यदि “in effect” ध्वज लगा हुआ है; 100000 जोड़ first pre chain order key, यदि “holds before chain” ध्वज लगा हुआ है; अन्य सभी स्थितियों में, 0। | _Order 8. Where the page's reasoner meets this fact as a premise: a stated fact at its list position, a fact concluded by the inverse or symmetric rule after every stated fact, in the order it was concluded; 0 for a fact that does not hold yet, so a link to no fact reads the same on every engine._ |
| First Fired Order Key | Statement के उन “firings” पर न्यूनतम “order key”, जो “fired unit” हैं। | _Order 15. Order key of the first rule application that concludes this fact, in the page reasoner's order._ |
| Count of Chosen Firings | Statement से संबंधित “firings” पर कुल “chosen unit”। | _Order 20. Rule applications shown as this fact's conclusion card: one for a conclusion, zero otherwise._ |
| Subject Node Key | उसके “subject” के समान। | _Order 1. The graph node this fact starts from._ |
| Object Node Key | प्राथमिकता से निर्धारित: “literal|”, उसके बाद object value, उसके बाद “|”, उसके बाद property datatype, यदि object thing खाली है; अन्य सभी स्थितियों में, object thing। | _Order 2. The graph node this fact points at: the thing, or one node per value and datatype._ |
| Subject Node Kind | लिंक किए गए “subject” से लिया जाता है। | _Order 19. How the graph draws the subject._ |
| Object Thing Node Kind | लिंक किए गए “object thing” से लिया जाता है। | _Order 19. How the graph draws the object thing._ |
| Object Node Kind | प्राथमिकता से निर्धारित: “value”, यदि object thing खाली है; अन्य सभी स्थितियों में, object thing node kind। | _Order 20. How the graph draws the object: a value, a class, a place or a person._ |
| Object Value Node Label | लिंक किए गए “property” से लिया जाता है। | _Order 1. The small label under a value node._ |
| Count of Violations As First | Statement से संबंधित “disjoint checks” पर कुल “violation unit”। | _Order 18. Violated disjointness checks naming this fact first._ |
| Count of Violations As Second | Statement से संबंधित “disjoint checks” पर कुल “violation unit”। | _Order 18. Violated disjointness checks naming this fact second._ |
| Is in Contradiction | तब सत्य, जब निम्नलिखित में से कम से कम एक सही है: count of violations as first 0 से अधिक है या count of violations as second 0 से अधिक है। | _Order 19. Part of a contradiction the disjoint rule found; drawn in the alarm colour._ |

<details open>
<summary>उदाहरण डेटा</summary>

| Object Value | Is Listed | Is Built in | Is Detail | Is Stated | Activation Seq | Sort Order | Subject Label ƒ | Property Label ƒ | Object Label ƒ | Property Datatype ƒ | Property Has One Current Value ƒ | Property Domain Class ƒ | Property Range Class ƒ | Literal Quote ƒ | Literal Type Marker ƒ | Built in Source Text ƒ | User Source Text ƒ | Next Fact Sort Order ƒ | Is Literal ƒ | Object Text ƒ | Name ƒ | Words ƒ | Source ƒ | Subject Property Key ƒ | Latest Sibling Activation Seq ƒ | Is in Effect ƒ | Listed Unit ƒ | In Effect Unit ƒ | Listed Off Unit ƒ | Is Next to Add ƒ | Count of Inverse Firings ƒ | Count of Symmetric Firings ƒ | Holds Before Chain ƒ | Count of Chain Firings ƒ | Holds Before Typing ƒ | Count of Type Firings ƒ | Holds ƒ | Is Conclusion ƒ | Holds Unit ƒ | Conclusion Unit ƒ | Parent Unit ƒ | Grandparent Unit ƒ | Place Unit ƒ | First Pre Chain Order Key ƒ | Snap Position ƒ | First Fired Order Key ƒ | Count of Chosen Firings ƒ | Subject Node Key ƒ | Object Node Key ƒ | Subject Node Kind ƒ | Object Thing Node Kind ƒ | Object Node Kind ƒ | Object Value Node Label ƒ | Count of Violations As First ƒ | Count of Violations As Second ƒ | Is in Contradiction ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| — | true | true | false | true | 1 | 1 | Marvin | is parent of | Randy | — | false | — | — | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | — | false | Randy | Marvin is parent of Randy | Marvin is parent of Randy | Stated as a teaching example on this page. | Marvin\|isParentOf | 1 | true | 1 | 1 | 0 | false | 0 | 0 | true | 0 | true | 0 | true | false | 1 | 0 | 1 | 0 | 0 | — | 1.0 | — | 0 | Marvin | Randy | person | person | person | — | 0 | 0 | false |
| — | false | false | false | false | 0 | 100001 | Marvin | is parent of | Sharon | — | false | — | — | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | — | false | Sharon | Marvin is parent of Sharon | Marvin is parent of Sharon | Stated by you on this page. | Marvin\|isParentOf | 1 | false | 0 | 0 | 0 | false | 0 | 0 | false | 0 | false | 0 | false | false | 0 | 0 | 0 | 0 | 0 | — | 0.0 | — | 0 | Marvin | Sharon | person | person | person | — | 0 | 0 | false |
| — | false | false | false | false | 0 | 100002 | Marvin | is parent of | Stan | — | false | — | — | " | ^^ | Stated as a teaching example on this page. | Stated by you on this page. | — | false | Stan | Marvin is parent of Stan | Marvin is parent of Stan | Stated by you on this page. | Marvin\|isParentOf | 1 | false | 0 | 0 | 0 | false | 0 | 0 | false | 0 | false | 0 | false | false | 0 | 0 | 0 | 0 | 0 | — | 0.0 | — | 0 | Marvin | Stan | person | person | person | — | 0 | 0 | false |

_ƒ परिकलित स्तंभ को चिह्नित करता है।_

</details>

| पद | विवरण | व्याख्यात्मक टिप्पणी |
|------|-------------|-------------------|
| **Firing** | Every way an enabled rule could conclude a candidate fact from the facts that hold. | — |
| Kind | एक परिभाषित विशेषता। | _How the rule applies._ |
| Rule | एक परिभाषित विशेषता। | _The rule switch._ |
| Target | एक परिभाषित विशेषता। | _The fact this application concludes._ |
| First | एक परिभाषित विशेषता। | _The premise._ |
| Second | एक परिभाषित विशेषता। | _The second premise, for the property chain._ |
| Rule Enabled | तब सत्य, जब firing के “rule” का “enabled” है। | _Order 1. Whether the rule is switched on._ |
| Kind Wording | लिंक किए गए “kind” से लिया जाता है। | _Order 1. How the card names the rule._ |
| Kind Sort Order | लिंक किए गए “kind” से लिया जाता है। | _Order 1. The kind's place in the reasoner's order._ |
| Target Name | लिंक किए गए “target” से लिया जाता है। | _Order 4. The concluded fact in words._ |
| Target Subject Label | लिंक किए गए “target” से लिया जाता है। | _Order 2. The concluded fact's subject._ |
| Target Property Label | लिंक किए गए “target” से लिया जाता है। | _Order 2. The concluded fact's property._ |
| Target Object Text | लिंक किए गए “target” से लिया जाता है। | _Order 3. The concluded fact's object._ |
| Target is Conclusion | तब सत्य, जब firing के “target” का “conclusion” है। | _Order 17. Whether the concluded fact is new._ |
| Target Subject Node Key | लिंक किए गए “target” से लिया जाता है। | _Order 2. The concluded fact's subject node._ |
| Target Object Node Key | लिंक किए गए “target” से लिया जाता है। | _Order 3. The concluded fact's object node._ |
| Target First Fired Order Key | लिंक किए गए “target” से लिया जाता है। | _Order 16. The order key of the concluded fact's first application._ |
| First Words | लिंक किए गए “first” से लिया जाता है। | _Order 3. The premise as the card quotes it._ |
| Second Words | लिंक किए गए “second” से लिया जाता है। | _Order 3. The second premise as the card quotes it._ |
| First is in Effect | तब सत्य, जब firing के “first” का “in effect” है। | _Order 4. Whether the premise is in effect._ |
| First Holds Before Chain | तब सत्य, जब firing के “first” का “holds before chain” है। | _Order 8. Whether the premise holds before the chain._ |
| Second Holds Before Chain | तब सत्य, जब firing के “second” का “holds before chain” है। | _Order 8. Whether the second premise holds before the chain._ |
| First Holds Before Typing | तब सत्य, जब firing के “first” का “holds before typing” है। | _Order 12. Whether the premise holds before typing._ |
| First Property | लिंक किए गए “first” से लिया जाता है। | _Order 1. The premise's property key, as the card's axiom names it._ |
| First Property Domain Class | लिंक किए गए “first” से लिया जाता है। | _Order 2. The premise property's domain class._ |
| First Property Range Class | लिंक किए गए “first” से लिया जाता है। | _Order 2. The premise property's range class._ |
| First Sort Order | लिंक किए गए “first” से लिया जाता है। | _Order 1. The premise's list position._ |
| Second is in Effect | तब सत्य, जब firing के “second” का “in effect” है। | _Order 4. Whether the second premise is in effect._ |
| First Snap Position | लिंक किए गए “first” से लिया जाता है। | _Order 9. Where the reasoner meets the premise._ |
| Second Snap Position | लिंक किए गए “second” से लिया जाता है। | _Order 9. Where the reasoner meets the second premise._ |
| Inverse Fired Unit | प्राथमिकता से निर्धारित: 1, यदि निम्नलिखित सभी सही हैं: “rule enabled” ध्वज लगा हुआ है, kind “inverse” है और “first is in effect” ध्वज लगा हुआ है; अन्य सभी स्थितियों में, 0। | _Order 5. 1 when the inverse rule fires here._ |
| Symmetric Fired Unit | प्राथमिकता से निर्धारित: 1, यदि निम्नलिखित सभी सही हैं: “rule enabled” ध्वज लगा हुआ है, kind “symmetric” है और “first is in effect” ध्वज लगा हुआ है; अन्य सभी स्थितियों में, 0। | _Order 5. 1 when the symmetric rule fires here._ |
| Chain Fired Unit | प्राथमिकता से निर्धारित: 1, यदि निम्नलिखित सभी सही हैं: “rule enabled” ध्वज लगा हुआ है, kind “chain” है, “first holds before chain” ध्वज लगा हुआ है और “second holds before chain” ध्वज लगा हुआ है; अन्य सभी स्थितियों में, 0। | _Order 9. 1 when the property chain fires here._ |
| Type Fired Unit | प्राथमिकता से निर्धारित: 1, यदि निम्नलिखित सभी सही हैं: “rule enabled” ध्वज लगा हुआ है, निम्नलिखित में से कम से कम एक सही है: kind “domain” है या kind “range” है और “first holds before typing” ध्वज लगा हुआ है; अन्य सभी स्थितियों में, 0। | _Order 13. 1 when the domain or range rule fires here._ |
| Fired Unit | inverse fired unit जोड़ symmetric fired unit जोड़ chain fired unit जोड़ type fired unit के रूप में गणना किया जाता है। | _Order 14. 1 when this application fires._ |
| Pre Chain Fired Unit | inverse fired unit जोड़ symmetric fired unit के रूप में गणना किया जाता है। | _Order 6. 1 when this is an inverse or symmetric application that fires._ |
| Pre Chain Order Key | first sort order गुणा 10 जोड़ kind sort order के रूप में गणना किया जाता है। | _Order 2. For an inverse or symmetric application: its place in the reasoner's first round._ |
| Round | प्राथमिकता से निर्धारित: 2, यदि निम्नलिखित सभी सही हैं: kind “chain” है और ऐसा नहीं है कि निम्नलिखित सभी सही हैं: “first is in effect” ध्वज लगा हुआ है और “second is in effect” ध्वज लगा हुआ है; अन्य सभी स्थितियों में, 1। | _Order 5. The reasoner's round: 1 when every premise is a stated fact, 2 when the property chain reads a concluded fact._ |
| Order Key | round गुणा 1000000000000000 जोड़ first snap position गुणा 100000000 जोड़ kind sort order गुणा 10000000 जोड़ 0, यदि second खाली है; अन्य सभी स्थितियों में second snap position के रूप में गणना किया जाता है। | _Order 10. The page reasoner's order: by round, then premise, then rule kind, then second premise._ |
| Is Chosen | तब सत्य, जब निम्नलिखित सभी सही हैं: fired unit 1 है, “target is conclusion” ध्वज लगा हुआ है और order key target first fired order key है। | _Order 18. The application the conclusions panel shows for its fact: the first one in the reasoner's order._ |
| Chosen Unit | प्राथमिकता से निर्धारित: 1, यदि “chosen” ध्वज लगा हुआ है; अन्य सभी स्थितियों में, 0। | _Order 19. 1 when this application is shown._ |
| Card Text | प्राथमिकता से निर्धारित: “Worked out by the ”, उसके बाद kind wording, उसके बाद “ from “”, उसके बाद first words, उसके बाद “””, उसके बाद “ and “”, उसके बाद second words, उसके बाद “””, यदि kind “chain” है; अन्य सभी स्थितियों में “ and “”, उसके बाद first property, उसके बाद “ has domain ”, उसके बाद first property domain class, उसके बाद “””, यदि kind “domain” है; अन्य सभी स्थितियों में “ and “”, उसके बाद first property, उसके बाद “ has range ”, उसके बाद first property range class, उसके बाद “””, उसके बाद एक बिंदु, यदि “chosen” ध्वज लगा हुआ है; अन्य सभी स्थितियों में, एक खाली स्ट्रिंग। ⚠︎ यांत्रिक <!-- rulespeak:reword --> | _Order 19. The conclusion card's provenance line, in the page's wording._ |
| Name | उसके “firing ID” के समान। | _Order 1. The application's key._ |

<details open>
<summary>उदाहरण डेटा</summary>

| Rule Enabled ƒ | Kind Wording ƒ | Kind Sort Order ƒ | Target Name ƒ | Target Subject Label ƒ | Target Property Label ƒ | Target Object Text ƒ | Target is Conclusion ƒ | Target Subject Node Key ƒ | Target Object Node Key ƒ | Target First Fired Order Key ƒ | First Words ƒ | Second Words ƒ | First is in Effect ƒ | First Holds Before Chain ƒ | Second Holds Before Chain ƒ | First Holds Before Typing ƒ | First Property ƒ | First Property Domain Class ƒ | First Property Range Class ƒ | First Sort Order ƒ | Second is in Effect ƒ | First Snap Position ƒ | Second Snap Position ƒ | Inverse Fired Unit ƒ | Symmetric Fired Unit ƒ | Chain Fired Unit ƒ | Type Fired Unit ƒ | Fired Unit ƒ | Pre Chain Fired Unit ƒ | Pre Chain Order Key ƒ | Round ƒ | Order Key ƒ | Is Chosen ƒ | Chosen Unit ƒ | Card Text ƒ | Name ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| false | inverse rule | 1 | Randy is child of Marvin | Randy | is child of | Marvin | false | Randy | Marvin | — | Marvin is parent of Randy |    | true | true | false | true | isParentOf | — | — | 1 | false | 1.0 | 0.0 | 0 | 0 | 0 | 0 | 0 | 0 | 11.0 | 1 | 1000000110000000.0 | false | 0 | — | inverse:Marvin\|isParentOf\|Randy>Randy\|isChildOf\|Marvin |
| false | inverse rule | 1 | Sharon is child of Marvin | Sharon | is child of | Marvin | false | Sharon | Marvin | — | Marvin is parent of Sharon |    | false | false | false | false | isParentOf | — | — | 100001 | false | 0.0 | 0.0 | 0 | 0 | 0 | 0 | 0 | 0 | 1000011.0 | 1 | 1000000010000000.0 | false | 0 | — | inverse:Marvin\|isParentOf\|Sharon>Sharon\|isChildOf\|Marvin |
| false | inverse rule | 1 | Stan is child of Marvin | Stan | is child of | Marvin | false | Stan | Marvin | — | Marvin is parent of Stan |    | false | false | false | false | isParentOf | — | — | 100002 | false | 0.0 | 0.0 | 0 | 0 | 0 | 0 | 0 | 0 | 1000021.0 | 1 | 1000000010000000.0 | false | 0 | — | inverse:Marvin\|isParentOf\|Stan>Stan\|isChildOf\|Marvin |

_ƒ परिकलित स्तंभ को चिह्नित करता है।_

</details>

| पद | विवरण | व्याख्यात्मक टिप्पणी |
|------|-------------|-------------------|
| **Disjoint Check** | Every pair the disjointness rule watches: the same two people linked by two disjoint properties. | — |
| Graph | एक परिभाषित विशेषता। | _The graph._ |
| Rule | एक परिभाषित विशेषता। | _The rule switch._ |
| First | एक परिभाषित विशेषता। | _The first link._ |
| Second | एक परिभाषित विशेषता। | _The disjoint link between the same pair, in the same direction._ |
| Rule Enabled | तब सत्य, जब disjoint check के “rule” का “enabled” है। | _Order 1. Whether the disjointness rule is on._ |
| First Holds | तब सत्य, जब disjoint check के “first” का “holds” है। | _Order 16. Whether the first link holds._ |
| Second Holds | तब सत्य, जब disjoint check के “second” का “holds” है। | _Order 16. Whether the second link holds._ |
| Subject Label | लिंक किए गए “first” से लिया जाता है। | _Order 2. The person recorded with both links._ |
| Object Label | लिंक किए गए “first” से लिया जाता है। | _Order 2. The other person._ |
| Violation Unit | प्राथमिकता से निर्धारित: 1, यदि निम्नलिखित सभी सही हैं: “rule enabled” ध्वज लगा हुआ है, “first holds” ध्वज लगा हुआ है और “second holds” ध्वज लगा हुआ है; अन्य सभी स्थितियों में, 0। | _Order 17. 1 when both links hold._ |
| Alarm Text | प्राथमिकता से निर्धारित: “Contradiction found. ”, उसके बाद subject label, उसके बाद “ is recorded as both parent and spouse of ”, उसके बाद object label, उसके बाद “. Check those facts and the selected disjoint rule.”, यदि violation unit 1 है; अन्य सभी स्थितियों में, एक खाली स्ट्रिंग। | _Order 18. The page's contradiction alert, blank when there is none._ |
| Name | उसके “disjoint check ID” के समान। | _Order 1. The check's key._ |

<details open>
<summary>उदाहरण डेटा</summary>

| Rule Enabled ƒ | First Holds ƒ | Second Holds ƒ | Subject Label ƒ | Object Label ƒ | Violation Unit ƒ | Alarm Text ƒ | Name ƒ |
|---|---|---|---|---|---|---|---|
| false | true | false | Marvin | Randy | 0 | — | Marvin\|isParentOf\|Randy~isSpouseOf |
| false | false | false | Marvin | Sharon | 0 | — | Marvin\|isParentOf\|Sharon~isSpouseOf |
| false | false | false | Marvin | Stan | 0 | — | Marvin\|isParentOf\|Stan~isSpouseOf |

_ƒ परिकलित स्तंभ को चिह्नित करता है।_

</details>

| पद | विवरण | व्याख्यात्मक टिप्पणी |
|------|-------------|-------------------|
| **Scenario** | The page's buttons. Each one is a fact baseline, explicit fact changes and rule settings, applied in that order. | — |
| Graph | एक परिभाषित विशेषता। | _The graph._ |
| Label | एक परिभाषित विशेषता। | _The button's text._ |
| Note | एक परिभाषित विशेषता। | _The note shown under the buttons after this one is pressed; blank clears it._ |
| Sort Order | एक परिभाषित विशेषता। | _Position among the buttons._ |
| Button Group | एक परिभाषित विशेषता। | _'stan' for the two imagined changes, 'facts' for the buttons under the fact list._ |
| Is Primary | तब सत्य, जब एक खाली स्ट्रिंग। | _Drawn as the filled button._ |
| Fact Baseline | एक परिभाषित विशेषता। | _What happens to every listed fact first: keep, built-in (teaching facts on, others off), built-in-detail, all-off, activate-all (each switched on in list order), or next (the next fact on)._ |
| Layout | एक परिभाषित विशेषता। | _The node layout the graph returns to; blank keeps the current positions._ |
| Clears Selection | तब सत्य, जब एक खाली स्ट्रिंग। | _Clears the selected node or line._ |
| Clears Form Message | तब सत्य, जब एक खाली स्ट्रिंग। | _Clears the Add fact form's message._ |
| Name | उसके “label” के समान। | _Order 1. The button's text._ |
| Graph Has Facts to Add | तब सत्य, जब scenario के “graph” का “facts to add” रखता है। | _Order 7. Whether a listed fact can still be added._ |
| Is Available | तब सत्य, जब निम्नलिखित में से कम से कम एक सही है: fact baseline “next” नहीं है या “graph has facts to add” ध्वज लगा हुआ है। | _Order 8. False only for Add next fact once every listed fact is in effect._ |

<details open>
<summary>उदाहरण डेटा</summary>

| Label | Note | Sort Order | Button Group | Is Primary | Fact Baseline | Layout | Clears Selection | Clears Form Message | Name ƒ | Graph Has Facts to Add ƒ | Is Available ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|
| Move Stan to grade 5 | This imagined change updates Stan’s grade to 5. His identifier, Person statement and family links stay the same. His other selected details are unchanged. | 1 | stan | false | keep | — | true | true | Move Stan to grade 5 | false | true |
| Stan leaves school | This imagined change clears Stan’s current school details. His identifier, Person statement and family links remain. Removing a current fact does not say that its opposite is true; school history needs dates. | 2 | stan | false | keep | — | true | true | Stan leaves school | false | true |
| Add next fact | — | 3 | facts | true | next | — | false | false | Add next fact | false | false |

_ƒ परिकलित स्तंभ को चिह्नित करता है।_

</details>

| पद | विवरण | व्याख्यात्मक टिप्पणी |
|------|-------------|-------------------|
| **Scenario Fact Change** | Fact changes a button makes after its baseline. A blank object key matches every listed value of the property. | — |
| Scenario | एक परिभाषित विशेषता। | _The button._ |
| Subject | एक परिभाषित विशेषता। | _The subject whose facts change._ |
| Property | एक परिभाषित विशेषता। | _The property whose facts change._ |
| Object Key | एक परिभाषित विशेषता। | _The object thing's key or the literal value; blank for every listed value._ |
| Action | एक परिभाषित विशेषता। | _'on' switches the fact on as the newest value; 'off' switches it off._ |
| Adds to List | तब सत्य, जब एक खाली स्ट्रिंग। | _Adds the fact to the fact list first when it is not listed._ |
| Sort Order | एक परिभाषित विशेषता। | _Order of the change within the button._ |
| Name | उसके “scenario fact change ID” के समान। | _Order 1. The change's key._ |

<details open>
<summary>उदाहरण डेटा</summary>

| Object Key | Action | Adds to List | Sort Order | Name ƒ |
|---|---|---|---|---|
| Person | on | true | 1 | stan-grade-five:Stan\|isA\|Person |
| 5 | on | true | 2 | stan-grade-five:Stan\|hasGradeLevel\|5 |
| Person | on | true | 3 | stan-leaves-school:Stan\|isA\|Person |

_ƒ परिकलित स्तंभ को चिह्नित करता है।_

</details>

| पद | विवरण | व्याख्यात्मक टिप्पणी |
|------|-------------|-------------------|
| **Scenario Rule Setting** | Rule switches a button sets. | — |
| Scenario | एक परिभाषित विशेषता। | _The button._ |
| Rule | एक परिभाषित विशेषता। | _The rule switch._ |
| Is Enabled | तब सत्य, जब एक खाली स्ट्रिंग। | _The state the button puts the switch in._ |
| Name | उसके “scenario rule setting ID” के समान। | _Order 1. The setting's key._ |

<details open>
<summary>उदाहरण डेटा</summary>

| Is Enabled | Name ƒ |
|---|---|
| false | reset-original:inverse |
| false | reset-original:symmetric |
| false | reset-original:chain |

_ƒ परिकलित स्तंभ को चिह्नित करता है।_

</details>

| पद | विवरण | व्याख्यात्मक टिप्पणी |
|------|-------------|-------------------|
| **Node Layout** | Node positions in the graph's 380 by 480 drawing, one set per layout. | — |
| Layout | एक परिभाषित विशेषता। | _The layout: 'default' or 'stan-details'._ |
| Node Key | एक परिभाषित विशेषता। | _The node: a thing's key, or literal\|value\|datatype._ |
| X | एक परिभाषित विशेषता। | _Horizontal position._ |
| Y | एक परिभाषित विशेषता। | _Vertical position._ |
| Name | उसके “node layout ID” के समान। | _Order 1. The position's key._ |

<details open>
<summary>उदाहरण डेटा</summary>

| Layout | Node Key | X | Y | Name ƒ |
|---|---|---|---|---|
| default | Marvin | 70 | 45 | default:Marvin |
| default | Randy | 195 | 125 | default:Randy |
| default | Sharon | 318 | 55 | default:Sharon |

_ƒ परिकलित स्तंभ को चिह्नित करता है।_

</details>

| पद | विवरण | व्याख्यात्मक टिप्पणी |
|------|-------------|-------------------|
| **Layout Slot** | Places for nodes no layout positions, used in turn. | — |
| Sort Order | एक परिभाषित विशेषता। | _Order in which slots are used._ |
| X | एक परिभाषित विशेषता। | _Horizontal position._ |
| Y | एक परिभाषित विशेषता। | _Vertical position._ |
| Name | उसके “layout slot ID” के समान। | _Order 1. The slot's key._ |

<details open>
<summary>उदाहरण डेटा</summary>

| Sort Order | X | Y | Name ƒ |
|---|---|---|---|
| 1 | 330 | 135 | slot-1 |
| 2 | 45 | 120 | slot-2 |
| 3 | 330 | 400 | slot-3 |

_ƒ परिकलित स्तंभ को चिह्नित करता है।_

</details>

| पद | विवरण | व्याख्यात्मक टिप्पणी |
|------|-------------|-------------------|
| **Page Text** | The page's wording, one row per string. {name}-style placeholders are filled by the page. | — |
| Text | एक परिभाषित विशेषता। | _The string._ |
| Name | उसके “page text ID” के समान। | _Order 1. The string's key._ |

<details open>
<summary>उदाहरण डेटा</summary>

| Text | Name ƒ |
|---|---|
| South Park Ontology Explainer, on one rulebook | page-title |
| A rulebook rebuild of the explainer by | byline-prefix |
| Jessica Talisman | byline-author |

_ƒ परिकलित स्तंभ को चिह्नित करता है।_

</details>

| पद | विवरण | व्याख्यात्मक टिप्पणी |
|------|-------------|-------------------|
| **Engine** | The execution engines the page can be computed by. Each is a build output of this rulebook. | — |
| Label | एक परिभाषित विशेषता। | _The engine's name in the switcher._ |
| Substrate | एक परिभाषित विशेषता। | _The conformance harness's name for this substrate._ |
| Is Default | तब सत्य, जब एक खाली स्ट्रिंग। | _The engine a new visitor starts on._ |
| Description | एक परिभाषित विशेषता। | _What computes the page on this engine._ |
| Sort Order | एक परिभाषित विशेषता। | _Position in the switcher._ |
| Name | उसके “label” के समान। | _Order 1. The engine's name._ |

<details open>
<summary>उदाहरण डेटा</summary>

| Label | Substrate | Is Default | Description | Sort Order | Name ƒ |
|---|---|---|---|---|---|
| Postgres views | effortless-postgres | true | Every value is a column of a generated view; the database computes it when the page reads it. | 1 | Postgres views |
| TypeScript | effortless-typescript | false | The generated TypeScript computes every field from the same rows, with no database. | 2 | TypeScript |
| OWL and SHACL rules | effortless-owl | false | The generated ontology and SHACL rules, run over the same rows by a SPARQL engine. | 3 | OWL and SHACL rules |

_ƒ परिकलित स्तंभ को चिह्नित करता है।_

</details>

| पद | विवरण | व्याख्यात्मक टिप्पणी |
|------|-------------|-------------------|
| **Rulebook Tool** | What each rulebook tool adds to the original explainer; the tools panel lists them. | — |
| Label | एक परिभाषित विशेषता। | _The tool's name._ |
| Adds | एक परिभाषित विशेषता। | _What it adds to the original explainer._ |
| Href | एक परिभाषित विशेषता। | _Where its link goes; 'editor' means the running rulebook editor._ |
| Link Text | एक परिभाषित विशेषता। | _The link's text._ |
| Sort Order | एक परिभाषित विशेषता। | _Position in the panel._ |
| Name | उसके “label” के समान। | _Order 1. The tool's name._ |

<details open>
<summary>उदाहरण डेटा</summary>

| Label | Adds | Href | Link Text | Sort Order | Name ƒ |
|---|---|---|---|---|---|
| Interchangeable engines | The same page, computed by Postgres views, generated TypeScript or the generated OWL and SHACL rules. Switch with the control above; nothing else changes. | — | — | 1 | Interchangeable engines |
| Conformance | Every engine's answers for the current facts, compared value by value with the conformance harness's own rule for when two values are equal. | — | — | 2 | Conformance |
| Explainer | Hover a dotted value for its rule in plain words and its witness: the values it came from, down to the stated facts. | /rulebook-explainer-dag/pages/index.html | Open the explainer | 3 | Explainer |

_ƒ परिकलित स्तंभ को चिह्नित करता है।_

</details>

## 2 तथ्य-प्रकार

- **firing kind** ठीक एक **rule** का संदर्भ देता है
- **thing** एक **portrait** का संदर्भ दे सकता है
- **portrait aliase** ठीक एक **portrait** का संदर्भ देता है
- **property** एक **thing** का संदर्भ दे सकता है
- **property axiom** ठीक एक **firing kind** का संदर्भ देता है
- **property axiom** ठीक एक **property** का संदर्भ देता है
- **statement** ठीक एक **graph** का संदर्भ देता है
- **statement** ठीक एक **thing** का संदर्भ देता है
- **statement** ठीक एक **property** का संदर्भ देता है
- **firing** ठीक एक **firing kind** का संदर्भ देता है
- **firing** ठीक एक **rule** का संदर्भ देता है
- **firing** ठीक एक **statement** का संदर्भ देता है
- **disjoint check** ठीक एक **graph** का संदर्भ देता है
- **disjoint check** ठीक एक **rule** का संदर्भ देता है
- **disjoint check** ठीक एक **statement** का संदर्भ देता है
- **scenario** ठीक एक **graph** का संदर्भ देता है
- **scenario fact change** ठीक एक **scenario** का संदर्भ देता है
- **scenario fact change** ठीक एक **thing** का संदर्भ देता है
- **scenario fact change** ठीक एक **property** का संदर्भ देता है
- **scenario rule setting** ठीक एक **scenario** का संदर्भ देता है
- **scenario rule setting** ठीक एक **rule** का संदर्भ देता है

## 3 प्रवर्तनकारी नियम

_प्रवर्तनकारी नियम बताते हैं कि व्यवसाय क्या **अनिवार्य** करता है, क्या **निषिद्ध** करता है,
या किसकी सलाह देता है (**चाहिए**)। संरचनात्मक नियम अनिवार्य फ़ील्डों और विदेशी कुंजियों से आते हैं;
अर्थगत नियम Constraints तालिका से आते हैं, जिनमें से हर एक किसी ऐसे बूलियन पर आधारित है जिसे
नियम-पुस्तिका पहले से गणना करती है (नीचे परिभाषात्मक नियमों में DR-N के रूप में संदर्भित)।_

### संरचनात्मक प्रतिबंध (स्कीमा से)

- graph को label, literal quote, literal type marker, built in source text, user source text और no conclusions text रखना **अनिवार्य** है।
- rule को label, explanation और sort order रखना, तथा यह कि वह “enabled” है या नहीं दर्ज करना **अनिवार्य** है।
- firing kind को ठीक एक “rule” का संदर्भ देना **अनिवार्य** है।
- firing kind को wording और sort order रखना **अनिवार्य** है।
- thing को label, kind और sort order रखना **अनिवार्य** है।
- portrait को display name, page, image, crop, width, height और sort order रखना **अनिवार्य** है।
- portrait aliase को ठीक एक “portrait” का संदर्भ देना **अनिवार्य** है।
- property को label, property kind और sort order रखना, तथा यह कि वह “user addable” है या नहीं, यह कि वह “one current value” रखता है या नहीं और यह कि वह “symmetric” है या नहीं दर्ज करना **अनिवार्य** है।
- property axiom को अपने “kind” के रूप में ठीक एक “firing kind” का संदर्भ देना **अनिवार्य** है।
- property axiom को ठीक एक “property” का संदर्भ देना **अनिवार्य** है।
- property axiom को अपने “first” के रूप में ठीक एक “property” का संदर्भ देना **अनिवार्य** है।
- statement को ठीक एक “graph” का संदर्भ देना **अनिवार्य** है।
- statement को अपने “subject” के रूप में ठीक एक “thing” का संदर्भ देना **अनिवार्य** है।
- statement को ठीक एक “property” का संदर्भ देना **अनिवार्य** है।
- statement को activation seq और sort order रखना, तथा यह कि वह “listed” है या नहीं, यह कि वह “built in” है या नहीं, यह कि वह “detail” है या नहीं और यह कि वह “stated” है या नहीं दर्ज करना **अनिवार्य** है।
- firing को अपने “kind” के रूप में ठीक एक “firing kind” का संदर्भ देना **अनिवार्य** है।
- firing को ठीक एक “rule” का संदर्भ देना **अनिवार्य** है।
- firing को अपने “target” के रूप में ठीक एक “statement” का संदर्भ देना **अनिवार्य** है।
- firing को अपने “first” के रूप में ठीक एक “statement” का संदर्भ देना **अनिवार्य** है।
- disjoint check को ठीक एक “graph” का संदर्भ देना **अनिवार्य** है।
- disjoint check को ठीक एक “rule” का संदर्भ देना **अनिवार्य** है।
- disjoint check को अपने “first” के रूप में ठीक एक “statement” का संदर्भ देना **अनिवार्य** है।
- disjoint check को अपने “second” के रूप में ठीक एक “statement” का संदर्भ देना **अनिवार्य** है।
- scenario को ठीक एक “graph” का संदर्भ देना **अनिवार्य** है।
- scenario को label, sort order, button group और fact baseline रखना, तथा यह कि वह “primary” है या नहीं, यह कि वह “clears selection” है या नहीं और यह कि वह “clears form message” है या नहीं दर्ज करना **अनिवार्य** है।
- scenario fact change को ठीक एक “scenario” का संदर्भ देना **अनिवार्य** है।
- scenario fact change को अपने “subject” के रूप में ठीक एक “thing” का संदर्भ देना **अनिवार्य** है।
- scenario fact change को ठीक एक “property” का संदर्भ देना **अनिवार्य** है।
- scenario fact change को action और sort order रखना, तथा यह कि वह “adds to list” है या नहीं दर्ज करना **अनिवार्य** है।
- scenario rule setting को ठीक एक “scenario” का संदर्भ देना **अनिवार्य** है।
- scenario rule setting को ठीक एक “rule” का संदर्भ देना **अनिवार्य** है।
- scenario rule setting को यह कि वह “enabled” है या नहीं दर्ज करना **अनिवार्य** है।
- node layout को layout, node key, x और y रखना **अनिवार्य** है।
- layout slot को sort order, x और y रखना **अनिवार्य** है।
- page text को text रखना **अनिवार्य** है।
- engine को label, substrate, description और sort order रखना, तथा यह कि वह “default” है या नहीं दर्ज करना **अनिवार्य** है।
- rulebook tool को label, adds और sort order रखना **अनिवार्य** है।

## 4 परिभाषात्मक नियम

_सभी कथन व्यावसायिक क्षेत्र का सत्य व्यक्त करते हैं; ये न प्रक्रियाएँ हैं, न आदेश।
"यदि और केवल यदि" के स्थान पर "केवल तभी जब" का प्रयोग किया गया है, ताकि एकतरफ़ा
अनिवार्यता को समतुल्यता न समझ लिया जाए। **⚠︎ यांत्रिक** चिह्न ऐसे नियम को दर्शाता है
जिसका निर्धारित शब्द-विन्यास सही तो है पर बोझिल — यह वैकल्पिक पुनर्लेखन का संकेत है,
कोई दोष नहीं।_

| क्रमांक | घोषणात्मक नियम |
|----|------------------|
| **DR-1 Name** | graph का “name” उसके “label” के समान है। |
| **DR-2 Count of Stated Facts** | graph का “count of stated facts” graph से संबंधित “statements” पर कुल “in effect unit” है। |
| **DR-3 Count of Conclusions** | graph का “count of conclusions” graph से संबंधित “statements” पर कुल “conclusion unit” है। |
| **DR-4 Count of Violations** | graph का “count of violations” graph से संबंधित “disjoint checks” पर कुल “violation unit” है। |
| **DR-5 Is Consistent** | यदि count of violations 0 है, तो graph “consistent” माना जाता है। |
| **DR-6 Conclusions Message** | Graph का “conclusions message” निम्नलिखित प्राथमिकता के अनुसार निर्धारित होता है:<br>1. no conclusions text, यदि count of conclusions 0 है;<br>2. अन्य सभी स्थितियों में, एक खाली स्ट्रिंग। |
| **DR-7 Count of Facts to Add** | graph का “count of facts to add” graph से संबंधित “statements” पर कुल “listed off unit” है। |
| **DR-8 Has Facts to Add** | यदि count of facts to add 0 से अधिक है, तो graph “facts to add” रखने वाला माना जाता है। |
| **DR-9 Next Fact Sort Order** | graph का “next fact sort order” graph के उन “statements” पर न्यूनतम “sort order”, जो “listed off unit” हैं है। |
| **DR-10 Max Activation Seq** | graph का “max activation seq” graph से संबंधित “statements” पर अधिकतम “activation seq” है। |
| **DR-11 Next Activation Seq** | graph का “next activation seq” max activation seq जोड़ 1 के रूप में गणना किया जाता है। |
| **DR-12 Max Listed Sort Order** | graph का “max listed sort order” graph के उन “statements” पर अधिकतम “sort order”, जो “listed unit” हैं है। |
| **DR-13 Next Listed Sort Order** | graph का “next listed sort order” max listed sort order जोड़ 1 के रूप में गणना किया जाता है। |
| **DR-14 Max Sort Order** | graph का “max sort order” graph से संबंधित “statements” पर अधिकतम “sort order” है। |
| **DR-15 Next Candidate Sort Order** | graph का “next candidate sort order” max sort order जोड़ 1 के रूप में गणना किया जाता है। |
| **DR-16 Name** | rule का “name” उसके “label” के समान है। |
| **DR-17 Count of Firings** | rule का “count of firings” rule से संबंधित “firings” पर कुल “fired unit” है। |
| **DR-18 Name** | firing kind का “name” उसके “wording” के समान है। |
| **DR-19 Rule Enabled** | firing kind का “rule enabled” तब सत्य होता है, जब लिंक किए गए “rule” का “enabled” है। |
| **DR-20 Portrait Name** | thing का “portrait name” लिंक किए गए “portrait” का “display name” है। |
| **DR-21 Portrait Page** | thing का “portrait page” — लिंक किए गए “portrait” से लिया जाता है। |
| **DR-22 Portrait Image** | thing का “portrait image” — लिंक किए गए “portrait” से लिया जाता है। |
| **DR-23 Portrait Crop** | thing का “portrait crop” — लिंक किए गए “portrait” से लिया जाता है। |
| **DR-24 Portrait Width** | thing का “portrait width” — लिंक किए गए “portrait” से लिया जाता है। |
| **DR-25 Portrait Height** | thing का “portrait height” — लिंक किए गए “portrait” से लिया जाता है। |
| **DR-26 Name** | thing का “name” उसके “label” के समान है। |
| **DR-27 Node Key** | thing का “node key” उसके “thing ID” के समान है। |
| **DR-28 Count of Children** | thing का “count of children” thing से संबंधित “statements” पर कुल “parent unit” है। |
| **DR-29 Count of Grandchildren** | thing का “count of grandchildren” thing से संबंधित “statements” पर कुल “grandparent unit” है। |
| **DR-30 Count As Place** | thing का “count as place” thing से संबंधित “statements” पर कुल “place unit” है। |
| **DR-31 Count As Subject** | thing का “count as subject” thing से संबंधित “statements” पर कुल “holds unit” है। |
| **DR-32 Count As Object** | thing का “count as object” thing से संबंधित “statements” पर कुल “holds unit” है। |
| **DR-33 Is in Graph** | यदि निम्नलिखित में से कम से कम एक सही है: count as subject 0 से अधिक है या count as object 0 से अधिक है, तो thing “in graph” माना जाता है। |
| **DR-34 Node Kind** | Thing का “node kind” निम्नलिखित प्राथमिकता के अनुसार निर्धारित होता है:<br>1. एक खाली स्ट्रिंग, यदि kind खाली है;<br>2. “class”, यदि kind “class” है;<br>3. “place”, यदि count as place 0 से अधिक है;<br>4. अन्य सभी स्थितियों में, “person”। |
| **DR-35 Name** | portrait का “name” उसके “display name” के समान है। |
| **DR-36 Name** | portrait aliase का “name” उसके “portrait alias ID” के समान है। |
| **DR-37 Name** | property का “name” उसके “label” के समान है। |
| **DR-38 Name** | property axiom का “name” उसके “property axiom ID” के समान है। |
| **DR-39 Subject Label** | statement का “subject label” — लिंक किए गए “subject” से लिया जाता है। |
| **DR-40 Property Label** | statement का “property label” — लिंक किए गए “property” से लिया जाता है। |
| **DR-41 Object Label** | statement का “object label” — लिंक किए गए “object thing” से लिया जाता है। |
| **DR-42 Property Datatype** | statement का “property datatype” — लिंक किए गए “property” से लिया जाता है। |
| **DR-43 Property Has One Current Value** | statement का “property has one current value” तब सत्य होता है, जब लिंक किए गए “property” का “one current value” रखता है। |
| **DR-44 Property Domain Class** | statement का “property domain class” — लिंक किए गए “property” से लिया जाता है। |
| **DR-45 Property Range Class** | statement का “property range class” — लिंक किए गए “property” से लिया जाता है। |
| **DR-46 Literal Quote** | statement का “literal quote” — लिंक किए गए “graph” से लिया जाता है। |
| **DR-47 Literal Type Marker** | statement का “literal type marker” — लिंक किए गए “graph” से लिया जाता है। |
| **DR-48 Built in Source Text** | statement का “built in source text” — लिंक किए गए “graph” से लिया जाता है। |
| **DR-49 User Source Text** | statement का “user source text” — लिंक किए गए “graph” से लिया जाता है। |
| **DR-50 Next Fact Sort Order** | statement का “next fact sort order” — लिंक किए गए “graph” से लिया जाता है। |
| **DR-51 Is Literal** | यदि object thing खाली है, तो statement “literal” माना जाता है। |
| **DR-52 Object Text** | Statement का “object text” निम्नलिखित प्राथमिकता के अनुसार निर्धारित होता है:<br>1. object value, यदि object thing खाली है;<br>2. अन्य सभी स्थितियों में, object label। |
| **DR-53 Name** | statement का “name” subject label, उसके बाद एक रिक्त स्थान, उसके बाद property label, उसके बाद एक रिक्त स्थान, उसके बाद object text के रूप में गणना किया जाता है। |
| **DR-54 Words** | statement का “words” subject label, उसके बाद एक रिक्त स्थान, उसके बाद property label, उसके बाद एक रिक्त स्थान, उसके बाद literal quote, उसके बाद object value, उसके बाद literal quote, उसके बाद literal type marker, उसके बाद property datatype, यदि object thing खाली है; अन्य सभी स्थितियों में object label के रूप में गणना किया जाता है। ⚠︎ यांत्रिक <!-- rulespeak:reword --> |
| **DR-55 Source** | Statement का “source” निम्नलिखित प्राथमिकता के अनुसार निर्धारित होता है:<br>1. built in source text, यदि “built in” ध्वज लगा हुआ है;<br>2. अन्य सभी स्थितियों में, user source text। |
| **DR-56 Subject Property Key** | statement का “subject property key” subject, उसके बाद “|”, उसके बाद property के रूप में गणना किया जाता है। |
| **DR-57 Latest Sibling Activation Seq** | statement का “latest sibling activation seq” statement से संबंधित “statements” पर अधिकतम “activation seq” है। |
| **DR-58 Is in Effect** | यदि निम्नलिखित सभी सही हैं: “stated” ध्वज लगा हुआ है और निम्नलिखित में से कम से कम एक सही है: “property has one current value” ध्वज लगा हुआ नहीं है या activation seq latest sibling activation seq है, तो statement “in effect” माना जाता है। |
| **DR-59 Listed Unit** | Statement का “listed unit” निम्नलिखित प्राथमिकता के अनुसार निर्धारित होता है:<br>1. 1, यदि “listed” ध्वज लगा हुआ है;<br>2. अन्य सभी स्थितियों में, 0। |
| **DR-60 In Effect Unit** | Statement का “in effect unit” निम्नलिखित प्राथमिकता के अनुसार निर्धारित होता है:<br>1. 1, यदि “in effect” ध्वज लगा हुआ है;<br>2. अन्य सभी स्थितियों में, 0। |
| **DR-61 Listed Off Unit** | Statement का “listed off unit” निम्नलिखित प्राथमिकता के अनुसार निर्धारित होता है:<br>1. 1, यदि निम्नलिखित सभी सही हैं: “listed” ध्वज लगा हुआ है और “in effect” ध्वज लगा हुआ नहीं है;<br>2. अन्य सभी स्थितियों में, 0। |
| **DR-62 Is Next to Add** | यदि निम्नलिखित सभी सही हैं: listed off unit 1 है और sort order next fact sort order है, तो statement “next to add” माना जाता है। |
| **DR-63 Count of Inverse Firings** | statement का “count of inverse firings” statement से संबंधित “firings” पर कुल “inverse fired unit” है। |
| **DR-64 Count of Symmetric Firings** | statement का “count of symmetric firings” statement से संबंधित “firings” पर कुल “symmetric fired unit” है। |
| **DR-65 Holds Before Chain** | यदि निम्नलिखित में से कम से कम एक सही है: “in effect” ध्वज लगा हुआ है, count of inverse firings 0 से अधिक है या count of symmetric firings 0 से अधिक है, तो statement “before chain” की शर्त पूरी करने वाला माना जाता है। |
| **DR-66 Count of Chain Firings** | statement का “count of chain firings” statement से संबंधित “firings” पर कुल “chain fired unit” है। |
| **DR-67 Holds Before Typing** | यदि निम्नलिखित में से कम से कम एक सही है: “holds before chain” ध्वज लगा हुआ है या count of chain firings 0 से अधिक है, तो statement “before typing” की शर्त पूरी करने वाला माना जाता है। |
| **DR-68 Count of Type Firings** | statement का “count of type firings” statement से संबंधित “firings” पर कुल “type fired unit” है। |
| **DR-69 Holds** | यदि निम्नलिखित में से कम से कम एक सही है: “holds before typing” ध्वज लगा हुआ है या count of type firings 0 से अधिक है, तो statement “” की शर्त पूरी करने वाला माना जाता है। |
| **DR-70 Is Conclusion** | यदि निम्नलिखित सभी सही हैं: “holds” ध्वज लगा हुआ है और “in effect” ध्वज लगा हुआ नहीं है, तो statement “conclusion” माना जाता है। |
| **DR-71 Holds Unit** | Statement का “holds unit” निम्नलिखित प्राथमिकता के अनुसार निर्धारित होता है:<br>1. 1, यदि “holds” ध्वज लगा हुआ है;<br>2. अन्य सभी स्थितियों में, 0। |
| **DR-72 Conclusion Unit** | Statement का “conclusion unit” निम्नलिखित प्राथमिकता के अनुसार निर्धारित होता है:<br>1. 1, यदि “conclusion” ध्वज लगा हुआ है;<br>2. अन्य सभी स्थितियों में, 0। |
| **DR-73 Parent Unit** | Statement का “parent unit” निम्नलिखित प्राथमिकता के अनुसार निर्धारित होता है:<br>1. 1, यदि निम्नलिखित सभी सही हैं: “holds” ध्वज लगा हुआ है और property “isParentOf” है;<br>2. अन्य सभी स्थितियों में, 0। |
| **DR-74 Grandparent Unit** | Statement का “grandparent unit” निम्नलिखित प्राथमिकता के अनुसार निर्धारित होता है:<br>1. 1, यदि निम्नलिखित सभी सही हैं: “holds” ध्वज लगा हुआ है और property “isGrandparentOf” है;<br>2. अन्य सभी स्थितियों में, 0। |
| **DR-75 Place Unit** | Statement का “place unit” निम्नलिखित प्राथमिकता के अनुसार निर्धारित होता है:<br>1. 1, यदि निम्नलिखित सभी सही हैं: “holds” ध्वज लगा हुआ है और property “attends” है;<br>2. अन्य सभी स्थितियों में, 0। |
| **DR-76 First Pre Chain Order Key** | statement का “first pre chain order key” statement के उन “firings” पर न्यूनतम “pre chain order key”, जो “pre chain fired unit” हैं है। |
| **DR-77 Snap Position** | Statement का “snap position” निम्नलिखित प्राथमिकता के अनुसार निर्धारित होता है:<br>1. sort order, यदि “in effect” ध्वज लगा हुआ है;<br>2. 100000 जोड़ first pre chain order key, यदि “holds before chain” ध्वज लगा हुआ है;<br>3. अन्य सभी स्थितियों में, 0। |
| **DR-78 First Fired Order Key** | statement का “first fired order key” statement के उन “firings” पर न्यूनतम “order key”, जो “fired unit” हैं है। |
| **DR-79 Count of Chosen Firings** | statement का “count of chosen firings” statement से संबंधित “firings” पर कुल “chosen unit” है। |
| **DR-80 Subject Node Key** | statement का “subject node key” उसके “subject” के समान है। |
| **DR-81 Object Node Key** | Statement का “object node key” निम्नलिखित प्राथमिकता के अनुसार निर्धारित होता है:<br>1. “literal|”, उसके बाद object value, उसके बाद “|”, उसके बाद property datatype, यदि object thing खाली है;<br>2. अन्य सभी स्थितियों में, object thing। |
| **DR-82 Subject Node Kind** | statement का “subject node kind” — लिंक किए गए “subject” से लिया जाता है। |
| **DR-83 Object Thing Node Kind** | statement का “object thing node kind” — लिंक किए गए “object thing” से लिया जाता है। |
| **DR-84 Object Node Kind** | Statement का “object node kind” निम्नलिखित प्राथमिकता के अनुसार निर्धारित होता है:<br>1. “value”, यदि object thing खाली है;<br>2. अन्य सभी स्थितियों में, object thing node kind। |
| **DR-85 Object Value Node Label** | statement का “object value node label” — लिंक किए गए “property” से लिया जाता है। |
| **DR-86 Count of Violations As First** | statement का “count of violations as first” statement से संबंधित “disjoint checks” पर कुल “violation unit” है। |
| **DR-87 Count of Violations As Second** | statement का “count of violations as second” statement से संबंधित “disjoint checks” पर कुल “violation unit” है। |
| **DR-88 Is in Contradiction** | यदि निम्नलिखित में से कम से कम एक सही है: count of violations as first 0 से अधिक है या count of violations as second 0 से अधिक है, तो statement “in contradiction” माना जाता है। |
| **DR-89 Rule Enabled** | firing का “rule enabled” तब सत्य होता है, जब लिंक किए गए “rule” का “enabled” है। |
| **DR-90 Kind Wording** | firing का “kind wording” — लिंक किए गए “kind” से लिया जाता है। |
| **DR-91 Kind Sort Order** | firing का “kind sort order” — लिंक किए गए “kind” से लिया जाता है। |
| **DR-92 Target Name** | firing का “target name” — लिंक किए गए “target” से लिया जाता है। |
| **DR-93 Target Subject Label** | firing का “target subject label” — लिंक किए गए “target” से लिया जाता है। |
| **DR-94 Target Property Label** | firing का “target property label” — लिंक किए गए “target” से लिया जाता है। |
| **DR-95 Target Object Text** | firing का “target object text” — लिंक किए गए “target” से लिया जाता है। |
| **DR-96 Target is Conclusion** | firing का “target is conclusion” तब सत्य होता है, जब लिंक किए गए “target” का “conclusion” है। |
| **DR-97 Target Subject Node Key** | firing का “target subject node key” — लिंक किए गए “target” से लिया जाता है। |
| **DR-98 Target Object Node Key** | firing का “target object node key” — लिंक किए गए “target” से लिया जाता है। |
| **DR-99 Target First Fired Order Key** | firing का “target first fired order key” — लिंक किए गए “target” से लिया जाता है। |
| **DR-100 First Words** | firing का “first words” — लिंक किए गए “first” से लिया जाता है। |
| **DR-101 Second Words** | firing का “second words” — लिंक किए गए “second” से लिया जाता है। |
| **DR-102 First is in Effect** | firing का “first is in effect” तब सत्य होता है, जब लिंक किए गए “first” का “in effect” है। |
| **DR-103 First Holds Before Chain** | firing का “first holds before chain” तब सत्य होता है, जब लिंक किए गए “first” का “holds before chain” है। |
| **DR-104 Second Holds Before Chain** | firing का “second holds before chain” तब सत्य होता है, जब लिंक किए गए “second” का “holds before chain” है। |
| **DR-105 First Holds Before Typing** | firing का “first holds before typing” तब सत्य होता है, जब लिंक किए गए “first” का “holds before typing” है। |
| **DR-106 First Property** | firing का “first property” — लिंक किए गए “first” से लिया जाता है। |
| **DR-107 First Property Domain Class** | firing का “first property domain class” — लिंक किए गए “first” से लिया जाता है। |
| **DR-108 First Property Range Class** | firing का “first property range class” — लिंक किए गए “first” से लिया जाता है। |
| **DR-109 First Sort Order** | firing का “first sort order” — लिंक किए गए “first” से लिया जाता है। |
| **DR-110 Second is in Effect** | firing का “second is in effect” तब सत्य होता है, जब लिंक किए गए “second” का “in effect” है। |
| **DR-111 First Snap Position** | firing का “first snap position” — लिंक किए गए “first” से लिया जाता है। |
| **DR-112 Second Snap Position** | firing का “second snap position” — लिंक किए गए “second” से लिया जाता है। |
| **DR-113 Inverse Fired Unit** | Firing का “inverse fired unit” निम्नलिखित प्राथमिकता के अनुसार निर्धारित होता है:<br>1. 1, यदि निम्नलिखित सभी सही हैं: “rule enabled” ध्वज लगा हुआ है, kind “inverse” है और “first is in effect” ध्वज लगा हुआ है;<br>2. अन्य सभी स्थितियों में, 0। |
| **DR-114 Symmetric Fired Unit** | Firing का “symmetric fired unit” निम्नलिखित प्राथमिकता के अनुसार निर्धारित होता है:<br>1. 1, यदि निम्नलिखित सभी सही हैं: “rule enabled” ध्वज लगा हुआ है, kind “symmetric” है और “first is in effect” ध्वज लगा हुआ है;<br>2. अन्य सभी स्थितियों में, 0। |
| **DR-115 Chain Fired Unit** | Firing का “chain fired unit” निम्नलिखित प्राथमिकता के अनुसार निर्धारित होता है:<br>1. 1, यदि निम्नलिखित सभी सही हैं: “rule enabled” ध्वज लगा हुआ है, kind “chain” है, “first holds before chain” ध्वज लगा हुआ है और “second holds before chain” ध्वज लगा हुआ है;<br>2. अन्य सभी स्थितियों में, 0। |
| **DR-116 Type Fired Unit** | Firing का “type fired unit” निम्नलिखित प्राथमिकता के अनुसार निर्धारित होता है:<br>1. 1, यदि निम्नलिखित सभी सही हैं: “rule enabled” ध्वज लगा हुआ है, निम्नलिखित में से कम से कम एक सही है: kind “domain” है या kind “range” है और “first holds before typing” ध्वज लगा हुआ है;<br>2. अन्य सभी स्थितियों में, 0। |
| **DR-117 Fired Unit** | firing का “fired unit” inverse fired unit जोड़ symmetric fired unit जोड़ chain fired unit जोड़ type fired unit के रूप में गणना किया जाता है। |
| **DR-118 Pre Chain Fired Unit** | firing का “pre chain fired unit” inverse fired unit जोड़ symmetric fired unit के रूप में गणना किया जाता है। |
| **DR-119 Pre Chain Order Key** | firing का “pre chain order key” first sort order गुणा 10 जोड़ kind sort order के रूप में गणना किया जाता है। |
| **DR-120 Round** | Firing का “round” निम्नलिखित प्राथमिकता के अनुसार निर्धारित होता है:<br>1. 2, यदि निम्नलिखित सभी सही हैं: kind “chain” है और ऐसा नहीं है कि निम्नलिखित सभी सही हैं: “first is in effect” ध्वज लगा हुआ है और “second is in effect” ध्वज लगा हुआ है;<br>2. अन्य सभी स्थितियों में, 1। |
| **DR-121 Order Key** | firing का “order key” round गुणा 1000000000000000 जोड़ first snap position गुणा 100000000 जोड़ kind sort order गुणा 10000000 जोड़ 0, यदि second खाली है; अन्य सभी स्थितियों में second snap position के रूप में गणना किया जाता है। |
| **DR-122 Is Chosen** | यदि निम्नलिखित सभी सही हैं: fired unit 1 है, “target is conclusion” ध्वज लगा हुआ है और order key target first fired order key है, तो firing “chosen” माना जाता है। |
| **DR-123 Chosen Unit** | Firing का “chosen unit” निम्नलिखित प्राथमिकता के अनुसार निर्धारित होता है:<br>1. 1, यदि “chosen” ध्वज लगा हुआ है;<br>2. अन्य सभी स्थितियों में, 0। |
| **DR-124 Card Text** | Firing का “card text” निम्नलिखित प्राथमिकता के अनुसार निर्धारित होता है:<br>1. “Worked out by the ”, उसके बाद kind wording, उसके बाद “ from “”, उसके बाद first words, उसके बाद “””, उसके बाद “ and “”, उसके बाद second words, उसके बाद “””, यदि kind “chain” है; अन्य सभी स्थितियों में “ and “”, उसके बाद first property, उसके बाद “ has domain ”, उसके बाद first property domain class, उसके बाद “””, यदि kind “domain” है; अन्य सभी स्थितियों में “ and “”, उसके बाद first property, उसके बाद “ has range ”, उसके बाद first property range class, उसके बाद “””, उसके बाद एक बिंदु, यदि “chosen” ध्वज लगा हुआ है;<br>2. अन्य सभी स्थितियों में, एक खाली स्ट्रिंग। ⚠︎ यांत्रिक <!-- rulespeak:reword --> |
| **DR-125 Name** | firing का “name” उसके “firing ID” के समान है। |
| **DR-126 Rule Enabled** | disjoint check का “rule enabled” तब सत्य होता है, जब लिंक किए गए “rule” का “enabled” है। |
| **DR-127 First Holds** | disjoint check का “first holds” तब सत्य होता है, जब लिंक किए गए “first” का “holds” है। |
| **DR-128 Second Holds** | disjoint check का “second holds” तब सत्य होता है, जब लिंक किए गए “second” का “holds” है। |
| **DR-129 Subject Label** | disjoint check का “subject label” — लिंक किए गए “first” से लिया जाता है। |
| **DR-130 Object Label** | disjoint check का “object label” — लिंक किए गए “first” से लिया जाता है। |
| **DR-131 Violation Unit** | Disjoint check का “violation unit” निम्नलिखित प्राथमिकता के अनुसार निर्धारित होता है:<br>1. 1, यदि निम्नलिखित सभी सही हैं: “rule enabled” ध्वज लगा हुआ है, “first holds” ध्वज लगा हुआ है और “second holds” ध्वज लगा हुआ है;<br>2. अन्य सभी स्थितियों में, 0। |
| **DR-132 Alarm Text** | Disjoint check का “alarm text” निम्नलिखित प्राथमिकता के अनुसार निर्धारित होता है:<br>1. “Contradiction found. ”, उसके बाद subject label, उसके बाद “ is recorded as both parent and spouse of ”, उसके बाद object label, उसके बाद “. Check those facts and the selected disjoint rule.”, यदि violation unit 1 है;<br>2. अन्य सभी स्थितियों में, एक खाली स्ट्रिंग। |
| **DR-133 Name** | disjoint check का “name” उसके “disjoint check ID” के समान है। |
| **DR-134 Name** | scenario का “name” उसके “label” के समान है। |
| **DR-135 Graph Has Facts to Add** | scenario का “graph has facts to add” तब सत्य होता है, जब लिंक किए गए “graph” का “facts to add” रखता है। |
| **DR-136 Is Available** | यदि निम्नलिखित में से कम से कम एक सही है: fact baseline “next” नहीं है या “graph has facts to add” ध्वज लगा हुआ है, तो scenario “available” माना जाता है। |
| **DR-137 Name** | scenario fact change का “name” उसके “scenario fact change ID” के समान है। |
| **DR-138 Name** | scenario rule setting का “name” उसके “scenario rule setting ID” के समान है। |
| **DR-139 Name** | node layout का “name” उसके “node layout ID” के समान है। |
| **DR-140 Name** | layout slot का “name” उसके “layout slot ID” के समान है। |
| **DR-141 Name** | page text का “name” उसके “page text ID” के समान है। |
| **DR-142 Name** | engine का “name” उसके “label” के समान है। |
| **DR-143 Name** | rulebook tool का “name” उसके “label” के समान है। |

## 5 स्कीमा तक अनुरेखणीयता

_व्यंजक स्तंभ दस्तावेज़ीकरण संकेतन में नियम की परिभाषा है —
वही तर्क जो नियम-पुस्तिका रखती है, व्यावसायिक पाठक के लिए लिखा हुआ।_

| स्कीमा तत्व | प्रकार | व्यंजक |
|----------------|------|------------|
| **Graphs.Name** | सूत्र | `Label` |
| **Graphs.CountOfStatedFacts** | समुच्चय | `Sum(Statements.InEffectUnit via Graph)` |
| **Graphs.CountOfConclusions** | समुच्चय | `Sum(Statements.ConclusionUnit via Graph)` |
| **Graphs.CountOfViolations** | समुच्चय | `Sum(DisjointChecks.ViolationUnit via Graph)` |
| **Graphs.IsConsistent** | सूत्र | `CountOfViolations = 0` |
| **Graphs.ConclusionsMessage** | सूत्र | `If(CountOfConclusions = 0, NoConclusionsText, "")` |
| **Graphs.CountOfFactsToAdd** | समुच्चय | `Sum(Statements.ListedOffUnit via Graph)` |
| **Graphs.HasFactsToAdd** | सूत्र | `CountOfFactsToAdd > 0` |
| **Graphs.NextFactSortOrder** | समुच्चय | `Min(Statements.SortOrder via Graph)` |
| **Graphs.MaxActivationSeq** | समुच्चय | `Max(Statements.ActivationSeq via Graph)` |
| **Graphs.NextActivationSeq** | सूत्र | `MaxActivationSeq + 1` |
| **Graphs.MaxListedSortOrder** | समुच्चय | `Max(Statements.SortOrder via Graph)` |
| **Graphs.NextListedSortOrder** | सूत्र | `MaxListedSortOrder + 1` |
| **Graphs.MaxSortOrder** | समुच्चय | `Max(Statements.SortOrder via Graph)` |
| **Graphs.NextCandidateSortOrder** | सूत्र | `MaxSortOrder + 1` |
| **Rules.Name** | सूत्र | `Label` |
| **Rules.CountOfFirings** | समुच्चय | `Sum(Firings.FiredUnit via Rule)` |
| **FiringKinds.Name** | सूत्र | `Wording` |
| **FiringKinds.RuleEnabled** | संदर्भ-मान | `Lookup(Rules.IsEnabled via Rule)` |
| **Things.PortraitName** | संदर्भ-मान | `Lookup(Portraits.DisplayName via Portrait)` |
| **Things.PortraitPage** | संदर्भ-मान | `Lookup(Portraits.Page via Portrait)` |
| **Things.PortraitImage** | संदर्भ-मान | `Lookup(Portraits.Image via Portrait)` |
| **Things.PortraitCrop** | संदर्भ-मान | `Lookup(Portraits.Crop via Portrait)` |
| **Things.PortraitWidth** | संदर्भ-मान | `Lookup(Portraits.Width via Portrait)` |
| **Things.PortraitHeight** | संदर्भ-मान | `Lookup(Portraits.Height via Portrait)` |
| **Things.Name** | सूत्र | `Label` |
| **Things.NodeKey** | सूत्र | `ThingId` |
| **Things.CountOfChildren** | समुच्चय | `Sum(Statements.ParentUnit via Subject)` |
| **Things.CountOfGrandchildren** | समुच्चय | `Sum(Statements.GrandparentUnit via Subject)` |
| **Things.CountAsPlace** | समुच्चय | `Sum(Statements.PlaceUnit via ObjectThing)` |
| **Things.CountAsSubject** | समुच्चय | `Sum(Statements.HoldsUnit via Subject)` |
| **Things.CountAsObject** | समुच्चय | `Sum(Statements.HoldsUnit via ObjectThing)` |
| **Things.IsInGraph** | सूत्र | `Or(CountAsSubject > 0, CountAsObject > 0)` |
| **Things.NodeKind** | सूत्र | `If(Kind = "", "", If(Kind = "class", "class", If(CountAsPlace > 0, "place", "person")))` |
| **Portraits.Name** | सूत्र | `DisplayName` |
| **PortraitAliases.Name** | सूत्र | `PortraitAliasId` |
| **Properties.Name** | सूत्र | `Label` |
| **PropertyAxioms.Name** | सूत्र | `PropertyAxiomId` |
| **Statements.SubjectLabel** | संदर्भ-मान | `Lookup(Things.Label via Subject)` |
| **Statements.PropertyLabel** | संदर्भ-मान | `Lookup(Properties.Label via Property)` |
| **Statements.ObjectLabel** | संदर्भ-मान | `Lookup(Things.Label via ObjectThing)` |
| **Statements.PropertyDatatype** | संदर्भ-मान | `Lookup(Properties.Datatype via Property)` |
| **Statements.PropertyHasOneCurrentValue** | संदर्भ-मान | `Lookup(Properties.HasOneCurrentValue via Property)` |
| **Statements.PropertyDomainClass** | संदर्भ-मान | `Lookup(Properties.DomainClass via Property)` |
| **Statements.PropertyRangeClass** | संदर्भ-मान | `Lookup(Properties.RangeClass via Property)` |
| **Statements.LiteralQuote** | संदर्भ-मान | `Lookup(Graphs.LiteralQuote via Graph)` |
| **Statements.LiteralTypeMarker** | संदर्भ-मान | `Lookup(Graphs.LiteralTypeMarker via Graph)` |
| **Statements.BuiltInSourceText** | संदर्भ-मान | `Lookup(Graphs.BuiltInSourceText via Graph)` |
| **Statements.UserSourceText** | संदर्भ-मान | `Lookup(Graphs.UserSourceText via Graph)` |
| **Statements.NextFactSortOrder** | संदर्भ-मान | `Lookup(Graphs.NextFactSortOrder via Graph)` |
| **Statements.IsLiteral** | सूत्र | `ObjectThing = ""` |
| **Statements.ObjectText** | सूत्र | `If(ObjectThing = "", ObjectValue, ObjectLabel)` |
| **Statements.Name** | सूत्र | `Concat(SubjectLabel, " ", PropertyLabel, " ", ObjectText)` |
| **Statements.Words** | सूत्र | `Concat(SubjectLabel, " ", PropertyLabel, " ", If(ObjectThing = "", Concat(LiteralQuote, ObjectValue, LiteralQuote, LiteralTypeMarker, PropertyDatatype), ObjectLabel))` |
| **Statements.Source** | सूत्र | `If(IsBuiltIn, BuiltInSourceText, UserSourceText)` |
| **Statements.SubjectPropertyKey** | सूत्र | `Concat(Subject, "\|", Property)` |
| **Statements.LatestSiblingActivationSeq** | समुच्चय | `Max(Statements.ActivationSeq via SubjectPropertyKey)` |
| **Statements.IsInEffect** | सूत्र | `And(IsStated, Or(Not(PropertyHasOneCurrentValue), ActivationSeq = LatestSiblingActivationSeq))` |
| **Statements.ListedUnit** | सूत्र | `If(IsListed, 1, 0)` |
| **Statements.InEffectUnit** | सूत्र | `If(IsInEffect, 1, 0)` |
| **Statements.ListedOffUnit** | सूत्र | `If(And(IsListed, Not(IsInEffect)), 1, 0)` |
| **Statements.IsNextToAdd** | सूत्र | `And(ListedOffUnit = 1, SortOrder = NextFactSortOrder)` |
| **Statements.CountOfInverseFirings** | समुच्चय | `Sum(Firings.InverseFiredUnit via Target)` |
| **Statements.CountOfSymmetricFirings** | समुच्चय | `Sum(Firings.SymmetricFiredUnit via Target)` |
| **Statements.HoldsBeforeChain** | सूत्र | `Or(IsInEffect, CountOfInverseFirings > 0, CountOfSymmetricFirings > 0)` |
| **Statements.CountOfChainFirings** | समुच्चय | `Sum(Firings.ChainFiredUnit via Target)` |
| **Statements.HoldsBeforeTyping** | सूत्र | `Or(HoldsBeforeChain, CountOfChainFirings > 0)` |
| **Statements.CountOfTypeFirings** | समुच्चय | `Sum(Firings.TypeFiredUnit via Target)` |
| **Statements.Holds** | सूत्र | `Or(HoldsBeforeTyping, CountOfTypeFirings > 0)` |
| **Statements.IsConclusion** | सूत्र | `And(Holds, Not(IsInEffect))` |
| **Statements.HoldsUnit** | सूत्र | `If(Holds, 1, 0)` |
| **Statements.ConclusionUnit** | सूत्र | `If(IsConclusion, 1, 0)` |
| **Statements.ParentUnit** | सूत्र | `If(And(Holds, Property = "isParentOf"), 1, 0)` |
| **Statements.GrandparentUnit** | सूत्र | `If(And(Holds, Property = "isGrandparentOf"), 1, 0)` |
| **Statements.PlaceUnit** | सूत्र | `If(And(Holds, Property = "attends"), 1, 0)` |
| **Statements.FirstPreChainOrderKey** | समुच्चय | `Min(Firings.PreChainOrderKey via Target)` |
| **Statements.SnapPosition** | सूत्र | `If(IsInEffect, SortOrder, If(HoldsBeforeChain, 100000 + FirstPreChainOrderKey, 0))` |
| **Statements.FirstFiredOrderKey** | समुच्चय | `Min(Firings.OrderKey via Target)` |
| **Statements.CountOfChosenFirings** | समुच्चय | `Sum(Firings.ChosenUnit via Target)` |
| **Statements.SubjectNodeKey** | सूत्र | `Subject` |
| **Statements.ObjectNodeKey** | सूत्र | `If(ObjectThing = "", Concat("literal\|", ObjectValue, "\|", PropertyDatatype), ObjectThing)` |
| **Statements.SubjectNodeKind** | संदर्भ-मान | `Lookup(Things.NodeKind via Subject)` |
| **Statements.ObjectThingNodeKind** | संदर्भ-मान | `Lookup(Things.NodeKind via ObjectThing)` |
| **Statements.ObjectNodeKind** | सूत्र | `If(ObjectThing = "", "value", ObjectThingNodeKind)` |
| **Statements.ObjectValueNodeLabel** | संदर्भ-मान | `Lookup(Properties.ValueNodeLabel via Property)` |
| **Statements.CountOfViolationsAsFirst** | समुच्चय | `Sum(DisjointChecks.ViolationUnit via First)` |
| **Statements.CountOfViolationsAsSecond** | समुच्चय | `Sum(DisjointChecks.ViolationUnit via Second)` |
| **Statements.IsInContradiction** | सूत्र | `Or(CountOfViolationsAsFirst > 0, CountOfViolationsAsSecond > 0)` |
| **Firings.RuleEnabled** | संदर्भ-मान | `Lookup(Rules.IsEnabled via Rule)` |
| **Firings.KindWording** | संदर्भ-मान | `Lookup(FiringKinds.Wording via Kind)` |
| **Firings.KindSortOrder** | संदर्भ-मान | `Lookup(FiringKinds.SortOrder via Kind)` |
| **Firings.TargetName** | संदर्भ-मान | `Lookup(Statements.Name via Target)` |
| **Firings.TargetSubjectLabel** | संदर्भ-मान | `Lookup(Statements.SubjectLabel via Target)` |
| **Firings.TargetPropertyLabel** | संदर्भ-मान | `Lookup(Statements.PropertyLabel via Target)` |
| **Firings.TargetObjectText** | संदर्भ-मान | `Lookup(Statements.ObjectText via Target)` |
| **Firings.TargetIsConclusion** | संदर्भ-मान | `Lookup(Statements.IsConclusion via Target)` |
| **Firings.TargetSubjectNodeKey** | संदर्भ-मान | `Lookup(Statements.SubjectNodeKey via Target)` |
| **Firings.TargetObjectNodeKey** | संदर्भ-मान | `Lookup(Statements.ObjectNodeKey via Target)` |
| **Firings.TargetFirstFiredOrderKey** | संदर्भ-मान | `Lookup(Statements.FirstFiredOrderKey via Target)` |
| **Firings.FirstWords** | संदर्भ-मान | `Lookup(Statements.Words via First)` |
| **Firings.SecondWords** | संदर्भ-मान | `Lookup(Statements.Words via Second)` |
| **Firings.FirstIsInEffect** | संदर्भ-मान | `Lookup(Statements.IsInEffect via First)` |
| **Firings.FirstHoldsBeforeChain** | संदर्भ-मान | `Lookup(Statements.HoldsBeforeChain via First)` |
| **Firings.SecondHoldsBeforeChain** | संदर्भ-मान | `Lookup(Statements.HoldsBeforeChain via Second)` |
| **Firings.FirstHoldsBeforeTyping** | संदर्भ-मान | `Lookup(Statements.HoldsBeforeTyping via First)` |
| **Firings.FirstProperty** | संदर्भ-मान | `Lookup(Statements.Property via First)` |
| **Firings.FirstPropertyDomainClass** | संदर्भ-मान | `Lookup(Statements.PropertyDomainClass via First)` |
| **Firings.FirstPropertyRangeClass** | संदर्भ-मान | `Lookup(Statements.PropertyRangeClass via First)` |
| **Firings.FirstSortOrder** | संदर्भ-मान | `Lookup(Statements.SortOrder via First)` |
| **Firings.SecondIsInEffect** | संदर्भ-मान | `Lookup(Statements.IsInEffect via Second)` |
| **Firings.FirstSnapPosition** | संदर्भ-मान | `Lookup(Statements.SnapPosition via First)` |
| **Firings.SecondSnapPosition** | संदर्भ-मान | `Lookup(Statements.SnapPosition via Second)` |
| **Firings.InverseFiredUnit** | सूत्र | `If(And(RuleEnabled, Kind = "inverse", FirstIsInEffect), 1, 0)` |
| **Firings.SymmetricFiredUnit** | सूत्र | `If(And(RuleEnabled, Kind = "symmetric", FirstIsInEffect), 1, 0)` |
| **Firings.ChainFiredUnit** | सूत्र | `If(And(RuleEnabled, Kind = "chain", FirstHoldsBeforeChain, SecondHoldsBeforeChain), 1, 0)` |
| **Firings.TypeFiredUnit** | सूत्र | `If(And(RuleEnabled, Or(Kind = "domain", Kind = "range"), FirstHoldsBeforeTyping), 1, 0)` |
| **Firings.FiredUnit** | सूत्र | `InverseFiredUnit + SymmetricFiredUnit + ChainFiredUnit + TypeFiredUnit` |
| **Firings.PreChainFiredUnit** | सूत्र | `InverseFiredUnit + SymmetricFiredUnit` |
| **Firings.PreChainOrderKey** | सूत्र | `FirstSortOrder * 10 + KindSortOrder` |
| **Firings.Round** | सूत्र | `If(And(Kind = "chain", Not(And(FirstIsInEffect, SecondIsInEffect))), 2, 1)` |
| **Firings.OrderKey** | सूत्र | `Round * 1000000000000000 + FirstSnapPosition * 100000000 + KindSortOrder * 10000000 + If(Second = "", 0, SecondSnapPosition)` |
| **Firings.IsChosen** | सूत्र | `And(FiredUnit = 1, TargetIsConclusion, OrderKey = TargetFirstFiredOrderKey)` |
| **Firings.ChosenUnit** | सूत्र | `If(IsChosen, 1, 0)` |
| **Firings.CardText** | सूत्र | `If(IsChosen, Concat("Worked out by the ", KindWording, " from “", FirstWords, "”", If(Kind = "chain", Concat(" and “", SecondWords, "”"), If(Kind = "domain", Concat(" and “", FirstProperty, " has domain ", FirstPropertyDomainClass, "”"), If(Kind = "range", Concat(" and “", FirstProperty, " has range ", FirstPropertyRangeClass, "”"), ""))), "."), "")` |
| **Firings.Name** | सूत्र | `FiringId` |
| **DisjointChecks.RuleEnabled** | संदर्भ-मान | `Lookup(Rules.IsEnabled via Rule)` |
| **DisjointChecks.FirstHolds** | संदर्भ-मान | `Lookup(Statements.Holds via First)` |
| **DisjointChecks.SecondHolds** | संदर्भ-मान | `Lookup(Statements.Holds via Second)` |
| **DisjointChecks.SubjectLabel** | संदर्भ-मान | `Lookup(Statements.SubjectLabel via First)` |
| **DisjointChecks.ObjectLabel** | संदर्भ-मान | `Lookup(Statements.ObjectLabel via First)` |
| **DisjointChecks.ViolationUnit** | सूत्र | `If(And(RuleEnabled, FirstHolds, SecondHolds), 1, 0)` |
| **DisjointChecks.AlarmText** | सूत्र | `If(ViolationUnit = 1, Concat("Contradiction found. ", SubjectLabel, " is recorded as both parent and spouse of ", ObjectLabel, ". Check those facts and the selected disjoint rule."), "")` |
| **DisjointChecks.Name** | सूत्र | `DisjointCheckId` |
| **Scenarios.Name** | सूत्र | `Label` |
| **Scenarios.GraphHasFactsToAdd** | संदर्भ-मान | `Lookup(Graphs.HasFactsToAdd via Graph)` |
| **Scenarios.IsAvailable** | सूत्र | `Or(FactBaseline <> "next", GraphHasFactsToAdd)` |
| **ScenarioFactChanges.Name** | सूत्र | `ScenarioFactChangeId` |
| **ScenarioRuleSettings.Name** | सूत्र | `ScenarioRuleSettingId` |
| **NodeLayouts.Name** | सूत्र | `NodeLayoutId` |
| **LayoutSlots.Name** | सूत्र | `LayoutSlotId` |
| **PageTexts.Name** | सूत्र | `PageTextId` |
| **Engines.Name** | सूत्र | `Label` |
| **RulebookTools.Name** | सूत्र | `Label` |

---

> **RuleSpeak®** — **Ronald G. Ross** द्वारा रचित, **SBVR** के नियमों का पालन करने वाली घोषणात्मक व्यावसायिक-नियम संकेत-पद्धति — जल्द आ रही है। तब तक यह पृष्ठ सामान्य दस्तावेज़ीकरण दिखाता है। [www.RonRoss.info](https://www.RonRoss.info)
