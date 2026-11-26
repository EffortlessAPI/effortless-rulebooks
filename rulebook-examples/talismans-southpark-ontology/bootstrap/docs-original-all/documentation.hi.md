# 📘 South Park ontology explainer — दस्तावेज़ीकरण

_The barebones South Park ontology from Jessica Talisman's explainer: ten stated facts and five rules that can each be switched on. Conclusions are formulas._

> नियम-पुस्तिका से प्रस्तुत घोषणात्मक व्यावसायिक नियम। नीचे दिया गया हर कथन
> व्यावसायिक क्षेत्र का सत्य व्यक्त करता है — यह न कोई प्रक्रिया है, न कोई आदेश।
> नियम-पुस्तिका के सूत्र ही सत्य के एकमात्र स्रोत हैं; यह दस्तावेज़ उनका
> सरल भाषा में पठन है।

## 1 व्यावसायिक शब्दावली

| पद | विवरण | व्याख्यात्मक टिप्पणी |
|------|-------------|-------------------|
| **Graph** | The whole example. | — |
| Name | उसके “graph ID” के समान। | — |
| Count of Stated Facts | Graph से संबंधित “statements” पर कुल “stated unit”। | — |
| Count of Conclusions | Graph से संबंधित “statements” पर कुल “conclusion unit”। | _Facts that hold but were never stated._ |
| Count of Violations | Graph से संबंधित “disjoint checks” पर कुल “violation unit”। | — |
| Is Consistent | तब सत्य, जब count of violations 0 है। | — |

<details open>
<summary>उदाहरण डेटा</summary>

| Name ƒ | Count of Stated Facts ƒ | Count of Conclusions ƒ | Count of Violations ƒ | Is Consistent ƒ |
|---|---|---|---|---|
| south-park | 10 | 8 | 0 | true |

_ƒ परिकलित स्तंभ को चिह्नित करता है।_

</details>

| पद | विवरण | व्याख्यात्मक टिप्पणी |
|------|-------------|-------------------|
| **Rule** | The five rules; IsEnabled is the checkbox. | — |
| Label | एक परिभाषित विशेषता। | — |
| Explanation | एक परिभाषित विशेषता। | — |
| Is Enabled | तब सत्य, जब एक खाली स्ट्रिंग। | — |
| Name | उसके “label” के समान। | — |
| Count of Firings | Rule से संबंधित “firings” पर कुल “fired unit”। | _How many times the rule produced a fact._ |

<details open>
<summary>उदाहरण डेटा</summary>

| Label | Explanation | Is Enabled | Name ƒ | Count of Firings ƒ |
|---|---|---|---|---|
| Inverse | A parent link also gives a child link between the same two people. | true | Inverse | 3 |
| Symmetric | A friend link or a spouse link is true for both people. | true | Symmetric | 2 |
| Property chain | A parent of a parent is a grandparent. | true | Property chain | 2 |

_ƒ परिकलित स्तंभ को चिह्नित करता है।_

</details>

| पद | विवरण | व्याख्यात्मक टिप्पणी |
|------|-------------|-------------------|
| **Thing** | A node in the picture: a person, a place, a class or a role value. | — |
| Label | एक परिभाषित विशेषता। | — |
| Kind | एक परिभाषित विशेषता। | — |
| Name | उसके “label” के समान। | — |
| Count of Children | Thing से संबंधित “statements” पर कुल “parent unit”। | _Added: a count, which a reasoner alone does not give._ |
| Count of Grandchildren | Thing से संबंधित “statements” पर कुल “grandparent unit”। | _Added: counts concluded grandchildren._ |

<details open>
<summary>उदाहरण डेटा</summary>

| Label | Kind | Name ƒ | Count of Children ƒ | Count of Grandchildren ƒ |
|---|---|---|---|---|
| Marvin | individual | Marvin | 1 | 2 |
| Randy | individual | Randy | 2 | 0 |
| Sharon | individual | Sharon | 0 | 0 |

_ƒ परिकलित स्तंभ को चिह्नित करता है।_

</details>

| पद | विवरण | व्याख्यात्मक टिप्पणी |
|------|-------------|-------------------|
| **Property** | A kind of link, with the characteristics the rules use. | — |
| Label | एक परिभाषित विशेषता। | — |
| Inverse of | एक परिभाषित विशेषता। | — |
| Is Symmetric | तब सत्य, जब एक खाली स्ट्रिंग। | — |
| Chain First | एक परिभाषित विशेषता। | — |
| Chain Second | एक परिभाषित विशेषता। | — |
| Domain Class | एक परिभाषित विशेषता। | — |
| Range Class | एक परिभाषित विशेषता। | — |
| Disjoint With | एक परिभाषित विशेषता। | — |
| Name | उसके “label” के समान। | — |

<details open>
<summary>उदाहरण डेटा</summary>

| Label | Is Symmetric | Name ƒ |
|---|---|---|
| is parent of | false | is parent of |
| is child of | false | is child of |
| is spouse of | true | is spouse of |

_ƒ परिकलित स्तंभ को चिह्नित करता है।_

</details>

| पद | विवरण | व्याख्यात्मक टिप्पणी |
|------|-------------|-------------------|
| **Statement** | A fact that is stated, or could be concluded: Subject, Property, Object. | — |
| Graph | एक परिभाषित विशेषता। | — |
| Subject | एक परिभाषित विशेषता। | — |
| Property | एक परिभाषित विशेषता। | — |
| Object Thing | एक परिभाषित विशेषता। | — |
| Number Value | एक परिभाषित विशेषता। | — |
| Is Stated | तब सत्य, जब एक खाली स्ट्रिंग। | _Drawn as a solid line in the explainer._ |
| Subject Label | लिंक किए गए “subject” से लिया जाता है। | — |
| Property Label | लिंक किए गए “property” से लिया जाता है। | — |
| Object Label | लिंक किए गए “object thing” से लिया जाता है। | — |
| Name | subject label, उसके बाद एक रिक्त स्थान, उसके बाद property label, उसके बाद एक रिक्त स्थान, उसके बाद number value, यदि object thing खाली है; अन्य सभी स्थितियों में object label के रूप में गणना किया जाता है। | — |
| Count of Inverse Firings | Statement से संबंधित “firings” पर कुल “inverse fired unit”। | — |
| Count of Symmetric Firings | Statement से संबंधित “firings” पर कुल “symmetric fired unit”। | — |
| Holds Before Chain | तब सत्य, जब निम्नलिखित में से कम से कम एक सही है: “stated” ध्वज लगा हुआ है, count of inverse firings 0 से अधिक है या count of symmetric firings 0 से अधिक है। | — |
| Count of Chain Firings | Statement से संबंधित “firings” पर कुल “chain fired unit”। | — |
| Holds Before Typing | तब सत्य, जब निम्नलिखित में से कम से कम एक सही है: “holds before chain” ध्वज लगा हुआ है या count of chain firings 0 से अधिक है। | — |
| Count of Type Firings | Statement से संबंधित “firings” पर कुल “type fired unit”। | — |
| Holds | तब सत्य, जब निम्नलिखित में से कम से कम एक सही है: “holds before typing” ध्वज लगा हुआ है या count of type firings 0 से अधिक है। | _True when stated or concluded by an enabled rule._ |
| Is Conclusion | तब सत्य, जब निम्नलिखित सभी सही हैं: “holds” ध्वज लगा हुआ है और “stated” ध्वज लगा हुआ नहीं है। | _Drawn as a dashed line in the explainer._ |
| Worked Out by | प्राथमिकता से निर्धारित: एक खाली स्ट्रिंग, यदि “conclusion” ध्वज लगा हुआ नहीं है; “inverse”, यदि count of inverse firings 0 से अधिक है; “symmetric”, यदि count of symmetric firings 0 से अधिक है; “property chain”, यदि count of chain firings 0 से अधिक है; अन्य सभी स्थितियों में, “domain and range”। | — |
| Stated Unit | प्राथमिकता से निर्धारित: 1, यदि “stated” ध्वज लगा हुआ है; अन्य सभी स्थितियों में, 0। | — |
| Conclusion Unit | प्राथमिकता से निर्धारित: 1, यदि “conclusion” ध्वज लगा हुआ है; अन्य सभी स्थितियों में, 0। | — |
| Parent Unit | प्राथमिकता से निर्धारित: 1, यदि निम्नलिखित सभी सही हैं: “holds” ध्वज लगा हुआ है और property “isParentOf” है; अन्य सभी स्थितियों में, 0। | — |
| Grandparent Unit | प्राथमिकता से निर्धारित: 1, यदि निम्नलिखित सभी सही हैं: “holds” ध्वज लगा हुआ है और property “isGrandparentOf” है; अन्य सभी स्थितियों में, 0। | — |

<details open>
<summary>उदाहरण डेटा</summary>

| Number Value | Is Stated | Subject Label ƒ | Property Label ƒ | Object Label ƒ | Name ƒ | Count of Inverse Firings ƒ | Count of Symmetric Firings ƒ | Holds Before Chain ƒ | Count of Chain Firings ƒ | Holds Before Typing ƒ | Count of Type Firings ƒ | Holds ƒ | Is Conclusion ƒ | Worked Out by ƒ | Stated Unit ƒ | Conclusion Unit ƒ | Parent Unit ƒ | Grandparent Unit ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| — | true | Marvin | is parent of | Randy | Marvin is parent of Randy | 0 | 0 | true | 0 | true | 0 | true | false | — | 1 | 0 | 1 | 0 |
| — | false | Marvin | is parent of | Sharon | Marvin is parent of Sharon | 0 | 0 | false | 0 | false | 0 | false | false | — | 0 | 0 | 0 | 0 |
| — | false | Marvin | is parent of | Shelly | Marvin is parent of Shelly | 0 | 0 | false | 0 | false | 0 | false | false | — | 0 | 0 | 0 | 0 |

_ƒ परिकलित स्तंभ को चिह्नित करता है।_

</details>

| पद | विवरण | व्याख्यात्मक टिप्पणी |
|------|-------------|-------------------|
| **Firing** | One way one rule could produce one fact from one or two others. | — |
| Rule | एक परिभाषित विशेषता। | — |
| Kind | एक परिभाषित विशेषता। | — |
| Target | एक परिभाषित विशेषता। | — |
| First | एक परिभाषित विशेषता। | — |
| Second | एक परिभाषित विशेषता। | _Same as First when the rule needs one premise._ |
| Rule Enabled | तब सत्य, जब firing के “rule” का “enabled” है। | — |
| Rule Label | लिंक किए गए “rule” से लिया जाता है। | — |
| Target Name | लिंक किए गए “target” से लिया जाता है। | — |
| First Name | लिंक किए गए “first” से लिया जाता है। | — |
| Second Name | लिंक किए गए “second” से लिया जाता है। | — |
| First is Stated | तब सत्य, जब firing के “first” का “stated” है। | — |
| First Holds Before Chain | तब सत्य, जब firing के “first” का “holds before chain” है। | — |
| Second Holds Before Chain | तब सत्य, जब firing के “second” का “holds before chain” है। | — |
| First Holds Before Typing | तब सत्य, जब firing के “first” का “holds before typing” है। | — |
| Inverse Fired Unit | प्राथमिकता से निर्धारित: 1, यदि निम्नलिखित सभी सही हैं: “rule enabled” ध्वज लगा हुआ है, kind “inverse” है और “first is stated” ध्वज लगा हुआ है; अन्य सभी स्थितियों में, 0। | — |
| Symmetric Fired Unit | प्राथमिकता से निर्धारित: 1, यदि निम्नलिखित सभी सही हैं: “rule enabled” ध्वज लगा हुआ है, kind “symmetric” है और “first is stated” ध्वज लगा हुआ है; अन्य सभी स्थितियों में, 0। | — |
| Chain Fired Unit | प्राथमिकता से निर्धारित: 1, यदि निम्नलिखित सभी सही हैं: “rule enabled” ध्वज लगा हुआ है, kind “chain” है, “first holds before chain” ध्वज लगा हुआ है और “second holds before chain” ध्वज लगा हुआ है; अन्य सभी स्थितियों में, 0। | — |
| Type Fired Unit | प्राथमिकता से निर्धारित: 1, यदि निम्नलिखित सभी सही हैं: “rule enabled” ध्वज लगा हुआ है, निम्नलिखित में से कम से कम एक सही है: kind “domain” है या kind “range” है और “first holds before typing” ध्वज लगा हुआ है; अन्य सभी स्थितियों में, 0। | — |
| Fired Unit | inverse fired unit जोड़ symmetric fired unit जोड़ chain fired unit जोड़ type fired unit के रूप में गणना किया जाता है। | — |
| Name | प्राथमिकता से निर्धारित: target name, उसके बाद “: worked out by the ”, उसके बाद rule label, उसके बाद “ rule from '”, उसके बाद first name, उसके बाद “'”, उसके बाद “ and '”, उसके बाद second name, उसके बाद “'”, यदि fired unit 0 से अधिक है; अन्य सभी स्थितियों में, firing ID। | _The explainer's conclusion card._ |

<details open>
<summary>उदाहरण डेटा</summary>

| Kind | Rule Enabled ƒ | Rule Label ƒ | Target Name ƒ | First Name ƒ | Second Name ƒ | First is Stated ƒ | First Holds Before Chain ƒ | Second Holds Before Chain ƒ | First Holds Before Typing ƒ | Inverse Fired Unit ƒ | Symmetric Fired Unit ƒ | Chain Fired Unit ƒ | Type Fired Unit ƒ | Fired Unit ƒ | Name ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| inverse | true | Inverse | Randy is child of Marvin | Marvin is parent of Randy | Marvin is parent of Randy | true | true | true | true | 1 | 0 | 0 | 0 | 1 | Randy is child of Marvin: worked out by the Inverse rule from 'Marvin is parent of Randy' |
| inverse | true | Inverse | Sharon is child of Marvin | Marvin is parent of Sharon | Marvin is parent of Sharon | false | false | false | false | 0 | 0 | 0 | 0 | 0 | inverse:Marvin\|isParentOf\|Sharon>Sharon\|isChildOf\|Marvin |
| inverse | true | Inverse | Shelly is child of Marvin | Marvin is parent of Shelly | Marvin is parent of Shelly | false | false | false | false | 0 | 0 | 0 | 0 | 0 | inverse:Marvin\|isParentOf\|Shelly>Shelly\|isChildOf\|Marvin |

_ƒ परिकलित स्तंभ को चिह्नित करता है।_

</details>

| पद | विवरण | व्याख्यात्मक टिप्पणी |
|------|-------------|-------------------|
| **Disjoint Check** | Two statements that must not both hold. | — |
| Graph | एक परिभाषित विशेषता। | — |
| Rule | एक परिभाषित विशेषता। | — |
| A | एक परिभाषित विशेषता। | — |
| B | एक परिभाषित विशेषता। | — |
| Name | उसके “disjoint check ID” के समान। | — |
| Rule Enabled | तब सत्य, जब disjoint check के “rule” का “enabled” है। | — |
| A Holds | तब सत्य, जब disjoint check के “a” का “holds” है। | — |
| B Holds | तब सत्य, जब disjoint check के “b” का “holds” है। | — |
| Violation Unit | प्राथमिकता से निर्धारित: 1, यदि निम्नलिखित सभी सही हैं: “rule enabled” ध्वज लगा हुआ है, “a holds” ध्वज लगा हुआ है और “b holds” ध्वज लगा हुआ है; अन्य सभी स्थितियों में, 0। | — |

<details open>
<summary>उदाहरण डेटा</summary>

| Name ƒ | Rule Enabled ƒ | A Holds ƒ | B Holds ƒ | Violation Unit ƒ |
|---|---|---|---|---|
| Marvin\|isParentOf\|Randy~isSpouseOf | true | true | false | 0 |
| Marvin\|isParentOf\|Sharon~isSpouseOf | true | false | false | 0 |
| Marvin\|isParentOf\|Shelly~isSpouseOf | true | false | false | 0 |

_ƒ परिकलित स्तंभ को चिह्नित करता है।_

</details>

## 2 तथ्य-प्रकार

- **property** एक **property** का संदर्भ दे सकता है
- **property** एक **thing** का संदर्भ दे सकता है
- **statement** ठीक एक **graph** का संदर्भ देता है
- **statement** ठीक एक **thing** का संदर्भ देता है
- **statement** ठीक एक **property** का संदर्भ देता है
- **firing** ठीक एक **rule** का संदर्भ देता है
- **firing** ठीक एक **statement** का संदर्भ देता है
- **disjoint check** ठीक एक **graph** का संदर्भ देता है
- **disjoint check** ठीक एक **rule** का संदर्भ देता है
- **disjoint check** ठीक एक **statement** का संदर्भ देता है

## 3 प्रवर्तनकारी नियम

_प्रवर्तनकारी नियम बताते हैं कि व्यवसाय क्या **अनिवार्य** करता है, क्या **निषिद्ध** करता है,
या किसकी सलाह देता है (**चाहिए**)। संरचनात्मक नियम अनिवार्य फ़ील्डों और विदेशी कुंजियों से आते हैं;
अर्थगत नियम Constraints तालिका से आते हैं, जिनमें से हर एक किसी ऐसे बूलियन पर आधारित है जिसे
नियम-पुस्तिका पहले से गणना करती है (नीचे परिभाषात्मक नियमों में DR-N के रूप में संदर्भित)।_

### संरचनात्मक प्रतिबंध (स्कीमा से)

- rule को label और explanation रखना, तथा यह कि वह “enabled” है या नहीं दर्ज करना **अनिवार्य** है।
- thing को label और kind रखना **अनिवार्य** है।
- property को label रखना, तथा यह कि वह “symmetric” है या नहीं दर्ज करना **अनिवार्य** है।
- statement को ठीक एक “graph” का संदर्भ देना **अनिवार्य** है।
- statement को अपने “subject” के रूप में ठीक एक “thing” का संदर्भ देना **अनिवार्य** है।
- statement को ठीक एक “property” का संदर्भ देना **अनिवार्य** है।
- statement को यह कि वह “stated” है या नहीं दर्ज करना **अनिवार्य** है।
- firing को ठीक एक “rule” का संदर्भ देना **अनिवार्य** है।
- firing को अपने “target” के रूप में ठीक एक “statement” का संदर्भ देना **अनिवार्य** है।
- firing को अपने “first” के रूप में ठीक एक “statement” का संदर्भ देना **अनिवार्य** है।
- firing को अपने “second” के रूप में ठीक एक “statement” का संदर्भ देना **अनिवार्य** है।
- firing को kind रखना **अनिवार्य** है।
- disjoint check को ठीक एक “graph” का संदर्भ देना **अनिवार्य** है।
- disjoint check को ठीक एक “rule” का संदर्भ देना **अनिवार्य** है।
- disjoint check को अपने “a” के रूप में ठीक एक “statement” का संदर्भ देना **अनिवार्य** है।
- disjoint check को अपने “b” के रूप में ठीक एक “statement” का संदर्भ देना **अनिवार्य** है।

## 4 परिभाषात्मक नियम

_सभी कथन व्यावसायिक क्षेत्र का सत्य व्यक्त करते हैं; ये न प्रक्रियाएँ हैं, न आदेश।
"यदि और केवल यदि" के स्थान पर "केवल तभी जब" का प्रयोग किया गया है, ताकि एकतरफ़ा
अनिवार्यता को समतुल्यता न समझ लिया जाए। **⚠︎ यांत्रिक** चिह्न ऐसे नियम को दर्शाता है
जिसका निर्धारित शब्द-विन्यास सही तो है पर बोझिल — यह वैकल्पिक पुनर्लेखन का संकेत है,
कोई दोष नहीं।_

| क्रमांक | घोषणात्मक नियम |
|----|------------------|
| **DR-1 Name** | graph का “name” उसके “graph ID” के समान है। |
| **DR-2 Count of Stated Facts** | graph का “count of stated facts” graph से संबंधित “statements” पर कुल “stated unit” है। |
| **DR-3 Count of Conclusions** | graph का “count of conclusions” graph से संबंधित “statements” पर कुल “conclusion unit” है। |
| **DR-4 Count of Violations** | graph का “count of violations” graph से संबंधित “disjoint checks” पर कुल “violation unit” है। |
| **DR-5 Is Consistent** | यदि count of violations 0 है, तो graph “consistent” माना जाता है। |
| **DR-6 Name** | rule का “name” उसके “label” के समान है। |
| **DR-7 Count of Firings** | rule का “count of firings” rule से संबंधित “firings” पर कुल “fired unit” है। |
| **DR-8 Name** | thing का “name” उसके “label” के समान है। |
| **DR-9 Count of Children** | thing का “count of children” thing से संबंधित “statements” पर कुल “parent unit” है। |
| **DR-10 Count of Grandchildren** | thing का “count of grandchildren” thing से संबंधित “statements” पर कुल “grandparent unit” है। |
| **DR-11 Name** | property का “name” उसके “label” के समान है। |
| **DR-12 Subject Label** | statement का “subject label” — लिंक किए गए “subject” से लिया जाता है। |
| **DR-13 Property Label** | statement का “property label” — लिंक किए गए “property” से लिया जाता है। |
| **DR-14 Object Label** | statement का “object label” — लिंक किए गए “object thing” से लिया जाता है। |
| **DR-15 Name** | statement का “name” subject label, उसके बाद एक रिक्त स्थान, उसके बाद property label, उसके बाद एक रिक्त स्थान, उसके बाद number value, यदि object thing खाली है; अन्य सभी स्थितियों में object label के रूप में गणना किया जाता है। |
| **DR-16 Count of Inverse Firings** | statement का “count of inverse firings” statement से संबंधित “firings” पर कुल “inverse fired unit” है। |
| **DR-17 Count of Symmetric Firings** | statement का “count of symmetric firings” statement से संबंधित “firings” पर कुल “symmetric fired unit” है। |
| **DR-18 Holds Before Chain** | यदि निम्नलिखित में से कम से कम एक सही है: “stated” ध्वज लगा हुआ है, count of inverse firings 0 से अधिक है या count of symmetric firings 0 से अधिक है, तो statement “before chain” की शर्त पूरी करने वाला माना जाता है। |
| **DR-19 Count of Chain Firings** | statement का “count of chain firings” statement से संबंधित “firings” पर कुल “chain fired unit” है। |
| **DR-20 Holds Before Typing** | यदि निम्नलिखित में से कम से कम एक सही है: “holds before chain” ध्वज लगा हुआ है या count of chain firings 0 से अधिक है, तो statement “before typing” की शर्त पूरी करने वाला माना जाता है। |
| **DR-21 Count of Type Firings** | statement का “count of type firings” statement से संबंधित “firings” पर कुल “type fired unit” है। |
| **DR-22 Holds** | यदि निम्नलिखित में से कम से कम एक सही है: “holds before typing” ध्वज लगा हुआ है या count of type firings 0 से अधिक है, तो statement “” की शर्त पूरी करने वाला माना जाता है। |
| **DR-23 Is Conclusion** | यदि निम्नलिखित सभी सही हैं: “holds” ध्वज लगा हुआ है और “stated” ध्वज लगा हुआ नहीं है, तो statement “conclusion” माना जाता है। |
| **DR-24 Worked Out by** | Statement का “worked out by” निम्नलिखित प्राथमिकता के अनुसार निर्धारित होता है:<br>1. एक खाली स्ट्रिंग, यदि “conclusion” ध्वज लगा हुआ नहीं है;<br>2. “inverse”, यदि count of inverse firings 0 से अधिक है;<br>3. “symmetric”, यदि count of symmetric firings 0 से अधिक है;<br>4. “property chain”, यदि count of chain firings 0 से अधिक है;<br>5. अन्य सभी स्थितियों में, “domain and range”। |
| **DR-25 Stated Unit** | Statement का “stated unit” निम्नलिखित प्राथमिकता के अनुसार निर्धारित होता है:<br>1. 1, यदि “stated” ध्वज लगा हुआ है;<br>2. अन्य सभी स्थितियों में, 0। |
| **DR-26 Conclusion Unit** | Statement का “conclusion unit” निम्नलिखित प्राथमिकता के अनुसार निर्धारित होता है:<br>1. 1, यदि “conclusion” ध्वज लगा हुआ है;<br>2. अन्य सभी स्थितियों में, 0। |
| **DR-27 Parent Unit** | Statement का “parent unit” निम्नलिखित प्राथमिकता के अनुसार निर्धारित होता है:<br>1. 1, यदि निम्नलिखित सभी सही हैं: “holds” ध्वज लगा हुआ है और property “isParentOf” है;<br>2. अन्य सभी स्थितियों में, 0। |
| **DR-28 Grandparent Unit** | Statement का “grandparent unit” निम्नलिखित प्राथमिकता के अनुसार निर्धारित होता है:<br>1. 1, यदि निम्नलिखित सभी सही हैं: “holds” ध्वज लगा हुआ है और property “isGrandparentOf” है;<br>2. अन्य सभी स्थितियों में, 0। |
| **DR-29 Rule Enabled** | firing का “rule enabled” तब सत्य होता है, जब लिंक किए गए “rule” का “enabled” है। |
| **DR-30 Rule Label** | firing का “rule label” — लिंक किए गए “rule” से लिया जाता है। |
| **DR-31 Target Name** | firing का “target name” — लिंक किए गए “target” से लिया जाता है। |
| **DR-32 First Name** | firing का “first name” — लिंक किए गए “first” से लिया जाता है। |
| **DR-33 Second Name** | firing का “second name” — लिंक किए गए “second” से लिया जाता है। |
| **DR-34 First is Stated** | firing का “first is stated” तब सत्य होता है, जब लिंक किए गए “first” का “stated” है। |
| **DR-35 First Holds Before Chain** | firing का “first holds before chain” तब सत्य होता है, जब लिंक किए गए “first” का “holds before chain” है। |
| **DR-36 Second Holds Before Chain** | firing का “second holds before chain” तब सत्य होता है, जब लिंक किए गए “second” का “holds before chain” है। |
| **DR-37 First Holds Before Typing** | firing का “first holds before typing” तब सत्य होता है, जब लिंक किए गए “first” का “holds before typing” है। |
| **DR-38 Inverse Fired Unit** | Firing का “inverse fired unit” निम्नलिखित प्राथमिकता के अनुसार निर्धारित होता है:<br>1. 1, यदि निम्नलिखित सभी सही हैं: “rule enabled” ध्वज लगा हुआ है, kind “inverse” है और “first is stated” ध्वज लगा हुआ है;<br>2. अन्य सभी स्थितियों में, 0। |
| **DR-39 Symmetric Fired Unit** | Firing का “symmetric fired unit” निम्नलिखित प्राथमिकता के अनुसार निर्धारित होता है:<br>1. 1, यदि निम्नलिखित सभी सही हैं: “rule enabled” ध्वज लगा हुआ है, kind “symmetric” है और “first is stated” ध्वज लगा हुआ है;<br>2. अन्य सभी स्थितियों में, 0। |
| **DR-40 Chain Fired Unit** | Firing का “chain fired unit” निम्नलिखित प्राथमिकता के अनुसार निर्धारित होता है:<br>1. 1, यदि निम्नलिखित सभी सही हैं: “rule enabled” ध्वज लगा हुआ है, kind “chain” है, “first holds before chain” ध्वज लगा हुआ है और “second holds before chain” ध्वज लगा हुआ है;<br>2. अन्य सभी स्थितियों में, 0। |
| **DR-41 Type Fired Unit** | Firing का “type fired unit” निम्नलिखित प्राथमिकता के अनुसार निर्धारित होता है:<br>1. 1, यदि निम्नलिखित सभी सही हैं: “rule enabled” ध्वज लगा हुआ है, निम्नलिखित में से कम से कम एक सही है: kind “domain” है या kind “range” है और “first holds before typing” ध्वज लगा हुआ है;<br>2. अन्य सभी स्थितियों में, 0। |
| **DR-42 Fired Unit** | firing का “fired unit” inverse fired unit जोड़ symmetric fired unit जोड़ chain fired unit जोड़ type fired unit के रूप में गणना किया जाता है। |
| **DR-43 Name** | Firing का “name” निम्नलिखित प्राथमिकता के अनुसार निर्धारित होता है:<br>1. target name, उसके बाद “: worked out by the ”, उसके बाद rule label, उसके बाद “ rule from '”, उसके बाद first name, उसके बाद “'”, उसके बाद “ and '”, उसके बाद second name, उसके बाद “'”, यदि fired unit 0 से अधिक है;<br>2. अन्य सभी स्थितियों में, firing ID। |
| **DR-44 Name** | disjoint check का “name” उसके “disjoint check ID” के समान है। |
| **DR-45 Rule Enabled** | disjoint check का “rule enabled” तब सत्य होता है, जब लिंक किए गए “rule” का “enabled” है। |
| **DR-46 A Holds** | disjoint check का “a holds” तब सत्य होता है, जब लिंक किए गए “a” का “holds” है। |
| **DR-47 B Holds** | disjoint check का “b holds” तब सत्य होता है, जब लिंक किए गए “b” का “holds” है। |
| **DR-48 Violation Unit** | Disjoint check का “violation unit” निम्नलिखित प्राथमिकता के अनुसार निर्धारित होता है:<br>1. 1, यदि निम्नलिखित सभी सही हैं: “rule enabled” ध्वज लगा हुआ है, “a holds” ध्वज लगा हुआ है और “b holds” ध्वज लगा हुआ है;<br>2. अन्य सभी स्थितियों में, 0। |

## 5 स्कीमा तक अनुरेखणीयता

_व्यंजक स्तंभ दस्तावेज़ीकरण संकेतन में नियम की परिभाषा है —
वही तर्क जो नियम-पुस्तिका रखती है, व्यावसायिक पाठक के लिए लिखा हुआ।_

| स्कीमा तत्व | प्रकार | व्यंजक |
|----------------|------|------------|
| **Graphs.Name** | सूत्र | `GraphId` |
| **Graphs.CountOfStatedFacts** | समुच्चय | `Sum(Statements.StatedUnit via Graph)` |
| **Graphs.CountOfConclusions** | समुच्चय | `Sum(Statements.ConclusionUnit via Graph)` |
| **Graphs.CountOfViolations** | समुच्चय | `Sum(DisjointChecks.ViolationUnit via Graph)` |
| **Graphs.IsConsistent** | सूत्र | `CountOfViolations = 0` |
| **Rules.Name** | सूत्र | `Label` |
| **Rules.CountOfFirings** | समुच्चय | `Sum(Firings.FiredUnit via Rule)` |
| **Things.Name** | सूत्र | `Label` |
| **Things.CountOfChildren** | समुच्चय | `Sum(Statements.ParentUnit via Subject)` |
| **Things.CountOfGrandchildren** | समुच्चय | `Sum(Statements.GrandparentUnit via Subject)` |
| **Properties.Name** | सूत्र | `Label` |
| **Statements.SubjectLabel** | संदर्भ-मान | `Lookup(Things.Label via Subject)` |
| **Statements.PropertyLabel** | संदर्भ-मान | `Lookup(Properties.Label via Property)` |
| **Statements.ObjectLabel** | संदर्भ-मान | `Lookup(Things.Label via ObjectThing)` |
| **Statements.Name** | सूत्र | `SubjectLabel & " " & PropertyLabel & " " & If(ObjectThing = Blank(), NumberValue, ObjectLabel)` |
| **Statements.CountOfInverseFirings** | समुच्चय | `Sum(Firings.InverseFiredUnit via Target)` |
| **Statements.CountOfSymmetricFirings** | समुच्चय | `Sum(Firings.SymmetricFiredUnit via Target)` |
| **Statements.HoldsBeforeChain** | सूत्र | `Or(IsStated, CountOfInverseFirings > 0, CountOfSymmetricFirings > 0)` |
| **Statements.CountOfChainFirings** | समुच्चय | `Sum(Firings.ChainFiredUnit via Target)` |
| **Statements.HoldsBeforeTyping** | सूत्र | `Or(HoldsBeforeChain, CountOfChainFirings > 0)` |
| **Statements.CountOfTypeFirings** | समुच्चय | `Sum(Firings.TypeFiredUnit via Target)` |
| **Statements.Holds** | सूत्र | `Or(HoldsBeforeTyping, CountOfTypeFirings > 0)` |
| **Statements.IsConclusion** | सूत्र | `And(Holds, Not(IsStated))` |
| **Statements.WorkedOutBy** | सूत्र | `If(Not(IsConclusion), "", If(CountOfInverseFirings > 0, "inverse", If(CountOfSymmetricFirings > 0, "symmetric", If(CountOfChainFirings > 0, "property chain", "domain and range"))))` |
| **Statements.StatedUnit** | सूत्र | `If(IsStated, 1, 0)` |
| **Statements.ConclusionUnit** | सूत्र | `If(IsConclusion, 1, 0)` |
| **Statements.ParentUnit** | सूत्र | `If(And(Holds, Property = "isParentOf"), 1, 0)` |
| **Statements.GrandparentUnit** | सूत्र | `If(And(Holds, Property = "isGrandparentOf"), 1, 0)` |
| **Firings.RuleEnabled** | संदर्भ-मान | `Lookup(Rules.IsEnabled via Rule)` |
| **Firings.RuleLabel** | संदर्भ-मान | `Lookup(Rules.Label via Rule)` |
| **Firings.TargetName** | संदर्भ-मान | `Lookup(Statements.Name via Target)` |
| **Firings.FirstName** | संदर्भ-मान | `Lookup(Statements.Name via First)` |
| **Firings.SecondName** | संदर्भ-मान | `Lookup(Statements.Name via Second)` |
| **Firings.FirstIsStated** | संदर्भ-मान | `Lookup(Statements.IsStated via First)` |
| **Firings.FirstHoldsBeforeChain** | संदर्भ-मान | `Lookup(Statements.HoldsBeforeChain via First)` |
| **Firings.SecondHoldsBeforeChain** | संदर्भ-मान | `Lookup(Statements.HoldsBeforeChain via Second)` |
| **Firings.FirstHoldsBeforeTyping** | संदर्भ-मान | `Lookup(Statements.HoldsBeforeTyping via First)` |
| **Firings.InverseFiredUnit** | सूत्र | `If(And(RuleEnabled, Kind = "inverse", FirstIsStated), 1, 0)` |
| **Firings.SymmetricFiredUnit** | सूत्र | `If(And(RuleEnabled, Kind = "symmetric", FirstIsStated), 1, 0)` |
| **Firings.ChainFiredUnit** | सूत्र | `If(And(RuleEnabled, Kind = "chain", FirstHoldsBeforeChain, SecondHoldsBeforeChain), 1, 0)` |
| **Firings.TypeFiredUnit** | सूत्र | `If(And(RuleEnabled, Or(Kind = "domain", Kind = "range"), FirstHoldsBeforeTyping), 1, 0)` |
| **Firings.FiredUnit** | सूत्र | `InverseFiredUnit + SymmetricFiredUnit + ChainFiredUnit + TypeFiredUnit` |
| **Firings.Name** | सूत्र | `If(FiredUnit > 0, TargetName & ": worked out by the " & RuleLabel & " rule from '" & FirstName & "'" & If(Kind = "chain", " and '" & SecondName & "'", ""), FiringId)` |
| **DisjointChecks.Name** | सूत्र | `DisjointCheckId` |
| **DisjointChecks.RuleEnabled** | संदर्भ-मान | `Lookup(Rules.IsEnabled via Rule)` |
| **DisjointChecks.AHolds** | संदर्भ-मान | `Lookup(Statements.Holds via A)` |
| **DisjointChecks.BHolds** | संदर्भ-मान | `Lookup(Statements.Holds via B)` |
| **DisjointChecks.ViolationUnit** | सूत्र | `If(And(RuleEnabled, AHolds, BHolds), 1, 0)` |
