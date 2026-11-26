# 📘 South Park ontology explainer — Dokumentation

_The barebones South Park ontology from Jessica Talisman's explainer: ten stated facts and five rules that can each be switched on. Conclusions are formulas._

> Deklarative Geschäftsregeln, aus dem Regelbuch gerendert. Jede Aussage
> unten drückt eine Wahrheit der Geschäftsdomäne aus — sie ist weder eine
> Prozedur noch ein Imperativ. Die Formeln des Regelbuchs sind die einzige
> Quelle der Wahrheit; dieses Dokument ist ihre klarsprachliche Lesart.

## 1 Geschäftsvokabular

| Begriff | Beschreibung | Erläuternder Kommentar |
|------|-------------|-------------------|
| **Graph** | The whole example. | — |
| Name | Entspricht Graph ID. | — |
| Count of Stated Facts | Die Gesamtsumme von Stated Unit über die mit dem Graph verbundenen Statements. | — |
| Count of Conclusions | Die Gesamtsumme von Conclusion Unit über die mit dem Graph verbundenen Statements. | _Facts that hold but were never stated._ |
| Count of Violations | Die Gesamtsumme von Violation Unit über die mit dem Graph verbundenen Disjoint Checks. | — |
| Is Consistent | Wahr, wenn der Count of Violations 0 ist. | — |

<details open>
<summary>Beispieldaten</summary>

| Name ƒ | Count of Stated Facts ƒ | Count of Conclusions ƒ | Count of Violations ƒ | Is Consistent ƒ |
|---|---|---|---|---|
| south-park | 11 | 9 | 0 | true |

_ƒ kennzeichnet eine berechnete Spalte._

</details>

| Begriff | Beschreibung | Erläuternder Kommentar |
|------|-------------|-------------------|
| **Rule** | The five rules; IsEnabled is the checkbox. | — |
| Label | Ein definiertes Attribut. | — |
| Explanation | Ein definiertes Attribut. | — |
| Is Enabled | Wahr, wenn eine leere Zeichenkette. | — |
| Name | Entspricht Label. | — |
| Count of Firings | Die Gesamtsumme von Fired Unit über die mit der Rule verbundenen Firings. | _How many times the rule produced a fact._ |

<details open>
<summary>Beispieldaten</summary>

| Label | Explanation | Is Enabled | Name ƒ | Count of Firings ƒ |
|---|---|---|---|---|
| Inverse | A parent link also gives a child link between the same two people. | true | Inverse | 4 |
| Symmetric | A friend link or a spouse link is true for both people. | true | Symmetric | 2 |
| Property chain | A parent of a parent is a grandparent. | true | Property chain | 2 |

_ƒ kennzeichnet eine berechnete Spalte._

</details>

| Begriff | Beschreibung | Erläuternder Kommentar |
|------|-------------|-------------------|
| **Thing** | A node in the picture: a person, a place, a class or a role value. | — |
| Label | Ein definiertes Attribut. | — |
| Kind | Ein definiertes Attribut. | — |
| Name | Entspricht Label. | — |
| Count of Children | Die Gesamtsumme von Parent Unit über die mit dem Thing verbundenen Statements. | _Added: a count, which a reasoner alone does not give._ |
| Count of Grandchildren | Die Gesamtsumme von Grandparent Unit über die mit dem Thing verbundenen Statements. | _Added: counts concluded grandchildren._ |

<details open>
<summary>Beispieldaten</summary>

| Label | Kind | Name ƒ | Count of Children ƒ | Count of Grandchildren ƒ |
|---|---|---|---|---|
| Marvin | individual | Marvin | 1 | 2 |
| Randy | individual | Randy | 2 | 0 |
| Sharon | individual | Sharon | 1 | 0 |

_ƒ kennzeichnet eine berechnete Spalte._

</details>

| Begriff | Beschreibung | Erläuternder Kommentar |
|------|-------------|-------------------|
| **Property** | A kind of link, with the characteristics the rules use. | — |
| Label | Ein definiertes Attribut. | — |
| Inverse of | Ein definiertes Attribut. | — |
| Is Symmetric | Wahr, wenn eine leere Zeichenkette. | — |
| Chain First | Ein definiertes Attribut. | — |
| Chain Second | Ein definiertes Attribut. | — |
| Domain Class | Ein definiertes Attribut. | — |
| Range Class | Ein definiertes Attribut. | — |
| Disjoint With | Ein definiertes Attribut. | — |
| Name | Entspricht Label. | — |

<details open>
<summary>Beispieldaten</summary>

| Label | Is Symmetric | Name ƒ |
|---|---|---|
| is parent of | false | is parent of |
| is child of | false | is child of |
| is spouse of | true | is spouse of |

_ƒ kennzeichnet eine berechnete Spalte._

</details>

| Begriff | Beschreibung | Erläuternder Kommentar |
|------|-------------|-------------------|
| **Statement** | A fact that is stated, or could be concluded: Subject, Property, Object. | — |
| Graph | Ein definiertes Attribut. | — |
| Subject | Ein definiertes Attribut. | — |
| Property | Ein definiertes Attribut. | — |
| Object Thing | Ein definiertes Attribut. | — |
| Number Value | Ein definiertes Attribut. | — |
| Is Stated | Wahr, wenn eine leere Zeichenkette. | _Drawn as a solid line in the explainer._ |
| Subject Label | Aus dem zugehörigen Subject übernommen. | — |
| Property Label | Aus dem zugehörigen Property übernommen. | — |
| Object Label | Aus dem zugehörigen Object Thing übernommen. | — |
| Name | Berechnet als der Subject Label, gefolgt von ein Leerzeichen, gefolgt von der Property Label, gefolgt von ein Leerzeichen, gefolgt von die Number Value, wenn der Object Thing leer ist, andernfalls der Object Label. | — |
| Count of Inverse Firings | Die Gesamtsumme von Inverse Fired Unit über die mit dem Statement verbundenen Firings. | — |
| Count of Symmetric Firings | Die Gesamtsumme von Symmetric Fired Unit über die mit dem Statement verbundenen Firings. | — |
| Holds Before Chain | Wahr, wenn mindestens eines des Folgenden gilt: das Kennzeichen „Is Stated“ gesetzt ist; der Count of Inverse Firings größer ist als 0; oder der Count of Symmetric Firings größer ist als 0. | — |
| Count of Chain Firings | Die Gesamtsumme von Chain Fired Unit über die mit dem Statement verbundenen Firings. | — |
| Holds Before Typing | Wahr, wenn mindestens eines des Folgenden gilt: das Kennzeichen „Holds Before Chain“ gesetzt ist oder der Count of Chain Firings größer ist als 0. | — |
| Count of Type Firings | Die Gesamtsumme von Type Fired Unit über die mit dem Statement verbundenen Firings. | — |
| Holds | Wahr, wenn mindestens eines des Folgenden gilt: das Kennzeichen „Holds Before Typing“ gesetzt ist oder der Count of Type Firings größer ist als 0. | _True when stated or concluded by an enabled rule._ |
| Is Conclusion | Wahr, wenn alles Folgende gilt: das Kennzeichen „Holds“ gesetzt ist und das Kennzeichen „Is Stated“ nicht gesetzt ist. | _Drawn as a dashed line in the explainer._ |
| Worked Out by | Nach Priorität bestimmt: eine leere Zeichenkette, wenn das Kennzeichen „Is Conclusion“ nicht gesetzt ist; „inverse“, wenn der Count of Inverse Firings größer ist als 0; „symmetric“, wenn der Count of Symmetric Firings größer ist als 0; „property chain“, wenn der Count of Chain Firings größer ist als 0; andernfalls „domain and range“. | — |
| Stated Unit | Nach Priorität bestimmt: 1, wenn das Kennzeichen „Is Stated“ gesetzt ist; andernfalls 0. | — |
| Conclusion Unit | Nach Priorität bestimmt: 1, wenn das Kennzeichen „Is Conclusion“ gesetzt ist; andernfalls 0. | — |
| Parent Unit | Nach Priorität bestimmt: 1, wenn alles Folgende gilt: das Kennzeichen „Holds“ gesetzt ist und der Property „isParentOf“ ist; andernfalls 0. | — |
| Grandparent Unit | Nach Priorität bestimmt: 1, wenn alles Folgende gilt: das Kennzeichen „Holds“ gesetzt ist und der Property „isGrandparentOf“ ist; andernfalls 0. | — |

<details open>
<summary>Beispieldaten</summary>

| Number Value | Is Stated | Subject Label ƒ | Property Label ƒ | Object Label ƒ | Name ƒ | Count of Inverse Firings ƒ | Count of Symmetric Firings ƒ | Holds Before Chain ƒ | Count of Chain Firings ƒ | Holds Before Typing ƒ | Count of Type Firings ƒ | Holds ƒ | Is Conclusion ƒ | Worked Out by ƒ | Stated Unit ƒ | Conclusion Unit ƒ | Parent Unit ƒ | Grandparent Unit ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| — | true | Marvin | is parent of | Randy | Marvin is parent of Randy | 0 | 0 | true | 0 | true | 0 | true | false | — | 1 | 0 | 1 | 0 |
| — | false | Marvin | is parent of | Sharon | Marvin is parent of Sharon | 0 | 0 | false | 0 | false | 0 | false | false | — | 0 | 0 | 0 | 0 |
| — | false | Marvin | is parent of | Shelly | Marvin is parent of Shelly | 0 | 0 | false | 0 | false | 0 | false | false | — | 0 | 0 | 0 | 0 |

_ƒ kennzeichnet eine berechnete Spalte._

</details>

| Begriff | Beschreibung | Erläuternder Kommentar |
|------|-------------|-------------------|
| **Firing** | One way one rule could produce one fact from one or two others. | — |
| Rule | Ein definiertes Attribut. | — |
| Kind | Ein definiertes Attribut. | — |
| Target | Ein definiertes Attribut. | — |
| First | Ein definiertes Attribut. | — |
| Second | Ein definiertes Attribut. | _Same as First when the rule needs one premise._ |
| Rule Enabled | Wahr, wenn der zugehörige Rule des Firing ist enabled. | — |
| Rule Label | Aus dem zugehörigen Rule übernommen. | — |
| Target Name | Aus dem zugehörigen Target übernommen. | — |
| First Name | Aus dem zugehörigen First übernommen. | — |
| Second Name | Aus dem zugehörigen Second übernommen. | — |
| First is Stated | Wahr, wenn der zugehörige First des Firing ist stated. | — |
| First Holds Before Chain | Wahr, wenn der zugehörige First des Firing ist Holds Before Chain. | — |
| Second Holds Before Chain | Wahr, wenn der zugehörige Second des Firing ist Holds Before Chain. | — |
| First Holds Before Typing | Wahr, wenn der zugehörige First des Firing ist Holds Before Typing. | — |
| Inverse Fired Unit | Nach Priorität bestimmt: 1, wenn alles Folgende gilt: das Kennzeichen „Rule Enabled“ gesetzt ist; der Kind „inverse“ ist; und das Kennzeichen „First is Stated“ gesetzt ist; andernfalls 0. | — |
| Symmetric Fired Unit | Nach Priorität bestimmt: 1, wenn alles Folgende gilt: das Kennzeichen „Rule Enabled“ gesetzt ist; der Kind „symmetric“ ist; und das Kennzeichen „First is Stated“ gesetzt ist; andernfalls 0. | — |
| Chain Fired Unit | Nach Priorität bestimmt: 1, wenn alles Folgende gilt: das Kennzeichen „Rule Enabled“ gesetzt ist; der Kind „chain“ ist; das Kennzeichen „First Holds Before Chain“ gesetzt ist; und das Kennzeichen „Second Holds Before Chain“ gesetzt ist; andernfalls 0. | — |
| Type Fired Unit | Nach Priorität bestimmt: 1, wenn alles Folgende gilt: das Kennzeichen „Rule Enabled“ gesetzt ist; mindestens eines des Folgenden gilt: der Kind „domain“ ist oder der Kind „range“ ist; und das Kennzeichen „First Holds Before Typing“ gesetzt ist; andernfalls 0. | — |
| Fired Unit | Berechnet als der Inverse Fired Unit plus der Symmetric Fired Unit plus der Chain Fired Unit plus der Type Fired Unit. | — |
| Name | Nach Priorität bestimmt: die Target Name, gefolgt von „: concluded by the “, gefolgt von der Rule Label, gefolgt von „ rule from '“, gefolgt von die First Name, gefolgt von „'“, gefolgt von „ and '“, gefolgt von die Second Name, gefolgt von „'“, wenn der Fired Unit größer ist als 0; andernfalls der Firing ID. | _The explainer's conclusion card._ |

<details open>
<summary>Beispieldaten</summary>

| Kind | Rule Enabled ƒ | Rule Label ƒ | Target Name ƒ | First Name ƒ | Second Name ƒ | First is Stated ƒ | First Holds Before Chain ƒ | Second Holds Before Chain ƒ | First Holds Before Typing ƒ | Inverse Fired Unit ƒ | Symmetric Fired Unit ƒ | Chain Fired Unit ƒ | Type Fired Unit ƒ | Fired Unit ƒ | Name ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| inverse | true | Inverse | Randy is child of Marvin | Marvin is parent of Randy | Marvin is parent of Randy | true | true | true | true | 1 | 0 | 0 | 0 | 1 | Randy is child of Marvin: concluded by the Inverse rule from 'Marvin is parent of Randy' |
| inverse | true | Inverse | Sharon is child of Marvin | Marvin is parent of Sharon | Marvin is parent of Sharon | false | false | false | false | 0 | 0 | 0 | 0 | 0 | inverse:Marvin\|isParentOf\|Sharon>Sharon\|isChildOf\|Marvin |
| inverse | true | Inverse | Shelly is child of Marvin | Marvin is parent of Shelly | Marvin is parent of Shelly | false | false | false | false | 0 | 0 | 0 | 0 | 0 | inverse:Marvin\|isParentOf\|Shelly>Shelly\|isChildOf\|Marvin |

_ƒ kennzeichnet eine berechnete Spalte._

</details>

| Begriff | Beschreibung | Erläuternder Kommentar |
|------|-------------|-------------------|
| **Disjoint Check** | Two statements that must not both hold. | — |
| Graph | Ein definiertes Attribut. | — |
| Rule | Ein definiertes Attribut. | — |
| A | Ein definiertes Attribut. | — |
| B | Ein definiertes Attribut. | — |
| Name | Entspricht Disjoint Check ID. | — |
| Rule Enabled | Wahr, wenn der zugehörige Rule des Disjoint Check ist enabled. | — |
| A Holds | Wahr, wenn der zugehörige A des Disjoint Check ist Holds. | — |
| B Holds | Wahr, wenn der zugehörige B des Disjoint Check ist Holds. | — |
| Violation Unit | Nach Priorität bestimmt: 1, wenn alles Folgende gilt: das Kennzeichen „Rule Enabled“ gesetzt ist; das Kennzeichen „A Holds“ gesetzt ist; und das Kennzeichen „B Holds“ gesetzt ist; andernfalls 0. | — |

<details open>
<summary>Beispieldaten</summary>

| Name ƒ | Rule Enabled ƒ | A Holds ƒ | B Holds ƒ | Violation Unit ƒ |
|---|---|---|---|---|
| Marvin\|isParentOf\|Randy~isSpouseOf | true | true | false | 0 |
| Marvin\|isParentOf\|Sharon~isSpouseOf | true | false | false | 0 |
| Marvin\|isParentOf\|Shelly~isSpouseOf | true | false | false | 0 |

_ƒ kennzeichnet eine berechnete Spalte._

</details>

## 2 Faktentypen

- ein **Property** kann auf ein **Property** verweisen
- ein **Property** kann auf ein **Thing** verweisen
- ein **Statement** verweist auf genau ein **Graph**
- ein **Statement** verweist auf genau ein **Thing**
- ein **Statement** verweist auf genau ein **Property**
- ein **Firing** verweist auf genau ein **Rule**
- ein **Firing** verweist auf genau ein **Statement**
- ein **Disjoint Check** verweist auf genau ein **Graph**
- ein **Disjoint Check** verweist auf genau ein **Rule**
- ein **Disjoint Check** verweist auf genau ein **Statement**

## 3 Operative Regeln

_Operative Regeln besagen, was das Geschäft **verpflichtet**, **verbietet** oder
empfiehlt (**sollte**). Strukturelle Regeln stammen aus Pflichtfeldern und Fremdschlüsseln;
semantische Regeln stammen aus der Constraints-Tabelle, jede gestützt auf einen booleschen Wert,
den das Regelbuch bereits berechnet (querverwiesen als DR-N in den definitorischen Regeln unten)._

### Strukturelle Einschränkungen (aus dem Schema)

- Eine Rule **muss** ein Label und eine Explanation haben und erfassen, ob es ist enabled.
- Ein Thing **muss** ein Label und ein Kind haben.
- Ein Property **muss** ein Label haben und erfassen, ob es ist ein Symmetric.
- Ein Statement **muss** auf genau einen Graph verweisen.
- Ein Statement **muss** auf genau einen Thing als seinen Subject verweisen.
- Ein Statement **muss** auf genau einen Property verweisen.
- Ein Statement **muss** erfassen, ob es ist stated.
- Ein Firing **muss** auf genau einen Rule verweisen.
- Ein Firing **muss** auf genau einen Statement als seinen Target verweisen.
- Ein Firing **muss** auf genau einen Statement als seinen First verweisen.
- Ein Firing **muss** auf genau einen Statement als seinen Second verweisen.
- Ein Firing **muss** ein Kind haben.
- Ein Disjoint Check **muss** auf genau einen Graph verweisen.
- Ein Disjoint Check **muss** auf genau einen Rule verweisen.
- Ein Disjoint Check **muss** auf genau einen Statement als seinen A verweisen.
- Ein Disjoint Check **muss** auf genau einen Statement als seinen B verweisen.

## 4 Definitorische Regeln

_Alle Aussagen drücken Wahrheiten der Geschäftsdomäne aus; sie sind weder Prozeduren
noch Imperative. Statt „genau dann, wenn“ wird „nur dann, wenn“ verwendet, damit eine
einseitige Notwendigkeit nicht mit einer Äquivalenz verwechselt wird. Ein
**⚠︎ mechanisch**-Hinweis markiert eine Regel, deren deterministische Formulierung treu,
aber hölzern ist — ein Signal für eine optionale Umformulierung, kein Fehler._

| ID | Deklarative Regel |
|----|------------------|
| **DR-1 Name** | Ein Graph: Name entspricht Graph ID. |
| **DR-2 Count of Stated Facts** | Ein Graph: Count of Stated Facts ist die Gesamtsumme von Stated Unit über die mit dem Graph verbundenen Statements. |
| **DR-3 Count of Conclusions** | Ein Graph: Count of Conclusions ist die Gesamtsumme von Conclusion Unit über die mit dem Graph verbundenen Statements. |
| **DR-4 Count of Violations** | Ein Graph: Count of Violations ist die Gesamtsumme von Violation Unit über die mit dem Graph verbundenen Disjoint Checks. |
| **DR-5 Is Consistent** | Ein Graph gilt als ein Consistent, wenn der Count of Violations 0 ist. |
| **DR-6 Name** | Eine Rule: Name entspricht Label. |
| **DR-7 Count of Firings** | Eine Rule: Count of Firings ist die Gesamtsumme von Fired Unit über die mit der Rule verbundenen Firings. |
| **DR-8 Name** | Ein Thing: Name entspricht Label. |
| **DR-9 Count of Children** | Ein Thing: Count of Children ist die Gesamtsumme von Parent Unit über die mit dem Thing verbundenen Statements. |
| **DR-10 Count of Grandchildren** | Ein Thing: Count of Grandchildren ist die Gesamtsumme von Grandparent Unit über die mit dem Thing verbundenen Statements. |
| **DR-11 Name** | Ein Property: Name entspricht Label. |
| **DR-12 Subject Label** | Ein Statement: Subject Label — aus dem zugehörigen Subject übernommen. |
| **DR-13 Property Label** | Ein Statement: Property Label — aus dem zugehörigen Property übernommen. |
| **DR-14 Object Label** | Ein Statement: Object Label — aus dem zugehörigen Object Thing übernommen. |
| **DR-15 Name** | Ein Statement: Name wird berechnet als der Subject Label, gefolgt von ein Leerzeichen, gefolgt von der Property Label, gefolgt von ein Leerzeichen, gefolgt von die Number Value, wenn der Object Thing leer ist, andernfalls der Object Label. |
| **DR-16 Count of Inverse Firings** | Ein Statement: Count of Inverse Firings ist die Gesamtsumme von Inverse Fired Unit über die mit dem Statement verbundenen Firings. |
| **DR-17 Count of Symmetric Firings** | Ein Statement: Count of Symmetric Firings ist die Gesamtsumme von Symmetric Fired Unit über die mit dem Statement verbundenen Firings. |
| **DR-18 Holds Before Chain** | Ein Statement gilt als Before Chain haltend, wenn mindestens eines des Folgenden gilt: das Kennzeichen „Is Stated“ gesetzt ist; der Count of Inverse Firings größer ist als 0; oder der Count of Symmetric Firings größer ist als 0. |
| **DR-19 Count of Chain Firings** | Ein Statement: Count of Chain Firings ist die Gesamtsumme von Chain Fired Unit über die mit dem Statement verbundenen Firings. |
| **DR-20 Holds Before Typing** | Ein Statement gilt als Before Typing haltend, wenn mindestens eines des Folgenden gilt: das Kennzeichen „Holds Before Chain“ gesetzt ist oder der Count of Chain Firings größer ist als 0. |
| **DR-21 Count of Type Firings** | Ein Statement: Count of Type Firings ist die Gesamtsumme von Type Fired Unit über die mit dem Statement verbundenen Firings. |
| **DR-22 Holds** | Ein Statement gilt als haltend, wenn mindestens eines des Folgenden gilt: das Kennzeichen „Holds Before Typing“ gesetzt ist oder der Count of Type Firings größer ist als 0. |
| **DR-23 Is Conclusion** | Ein Statement gilt als eine Conclusion, wenn alles Folgende gilt: das Kennzeichen „Holds“ gesetzt ist und das Kennzeichen „Is Stated“ nicht gesetzt ist. |
| **DR-24 Worked Out by** | Das Feld „Worked Out by“ des Statement wird nach folgender Priorität bestimmt:<br>1. eine leere Zeichenkette, wenn das Kennzeichen „Is Conclusion“ nicht gesetzt ist;<br>2. „inverse“, wenn der Count of Inverse Firings größer ist als 0;<br>3. „symmetric“, wenn der Count of Symmetric Firings größer ist als 0;<br>4. „property chain“, wenn der Count of Chain Firings größer ist als 0;<br>5. andernfalls „domain and range“. |
| **DR-25 Stated Unit** | Das Feld „Stated Unit“ des Statement wird nach folgender Priorität bestimmt:<br>1. 1, wenn das Kennzeichen „Is Stated“ gesetzt ist;<br>2. andernfalls 0. |
| **DR-26 Conclusion Unit** | Das Feld „Conclusion Unit“ des Statement wird nach folgender Priorität bestimmt:<br>1. 1, wenn das Kennzeichen „Is Conclusion“ gesetzt ist;<br>2. andernfalls 0. |
| **DR-27 Parent Unit** | Das Feld „Parent Unit“ des Statement wird nach folgender Priorität bestimmt:<br>1. 1, wenn alles Folgende gilt: das Kennzeichen „Holds“ gesetzt ist und der Property „isParentOf“ ist;<br>2. andernfalls 0. |
| **DR-28 Grandparent Unit** | Das Feld „Grandparent Unit“ des Statement wird nach folgender Priorität bestimmt:<br>1. 1, wenn alles Folgende gilt: das Kennzeichen „Holds“ gesetzt ist und der Property „isGrandparentOf“ ist;<br>2. andernfalls 0. |
| **DR-29 Rule Enabled** | Ein Firing: Rule Enabled ist wahr, wenn der zugehörige Rule des Firing ist enabled. |
| **DR-30 Rule Label** | Ein Firing: Rule Label — aus dem zugehörigen Rule übernommen. |
| **DR-31 Target Name** | Ein Firing: Target Name — aus dem zugehörigen Target übernommen. |
| **DR-32 First Name** | Ein Firing: First Name — aus dem zugehörigen First übernommen. |
| **DR-33 Second Name** | Ein Firing: Second Name — aus dem zugehörigen Second übernommen. |
| **DR-34 First is Stated** | Ein Firing: First is Stated ist wahr, wenn der zugehörige First des Firing ist stated. |
| **DR-35 First Holds Before Chain** | Ein Firing: First Holds Before Chain ist wahr, wenn der zugehörige First des Firing ist Holds Before Chain. |
| **DR-36 Second Holds Before Chain** | Ein Firing: Second Holds Before Chain ist wahr, wenn der zugehörige Second des Firing ist Holds Before Chain. |
| **DR-37 First Holds Before Typing** | Ein Firing: First Holds Before Typing ist wahr, wenn der zugehörige First des Firing ist Holds Before Typing. |
| **DR-38 Inverse Fired Unit** | Das Feld „Inverse Fired Unit“ des Firing wird nach folgender Priorität bestimmt:<br>1. 1, wenn alles Folgende gilt: das Kennzeichen „Rule Enabled“ gesetzt ist; der Kind „inverse“ ist; und das Kennzeichen „First is Stated“ gesetzt ist;<br>2. andernfalls 0. |
| **DR-39 Symmetric Fired Unit** | Das Feld „Symmetric Fired Unit“ des Firing wird nach folgender Priorität bestimmt:<br>1. 1, wenn alles Folgende gilt: das Kennzeichen „Rule Enabled“ gesetzt ist; der Kind „symmetric“ ist; und das Kennzeichen „First is Stated“ gesetzt ist;<br>2. andernfalls 0. |
| **DR-40 Chain Fired Unit** | Das Feld „Chain Fired Unit“ des Firing wird nach folgender Priorität bestimmt:<br>1. 1, wenn alles Folgende gilt: das Kennzeichen „Rule Enabled“ gesetzt ist; der Kind „chain“ ist; das Kennzeichen „First Holds Before Chain“ gesetzt ist; und das Kennzeichen „Second Holds Before Chain“ gesetzt ist;<br>2. andernfalls 0. |
| **DR-41 Type Fired Unit** | Das Feld „Type Fired Unit“ des Firing wird nach folgender Priorität bestimmt:<br>1. 1, wenn alles Folgende gilt: das Kennzeichen „Rule Enabled“ gesetzt ist; mindestens eines des Folgenden gilt: der Kind „domain“ ist oder der Kind „range“ ist; und das Kennzeichen „First Holds Before Typing“ gesetzt ist;<br>2. andernfalls 0. |
| **DR-42 Fired Unit** | Ein Firing: Fired Unit wird berechnet als der Inverse Fired Unit plus der Symmetric Fired Unit plus der Chain Fired Unit plus der Type Fired Unit. |
| **DR-43 Name** | Das Feld „Name“ des Firing wird nach folgender Priorität bestimmt:<br>1. die Target Name, gefolgt von „: concluded by the “, gefolgt von der Rule Label, gefolgt von „ rule from '“, gefolgt von die First Name, gefolgt von „'“, gefolgt von „ and '“, gefolgt von die Second Name, gefolgt von „'“, wenn der Fired Unit größer ist als 0;<br>2. andernfalls der Firing ID. |
| **DR-44 Name** | Ein Disjoint Check: Name entspricht Disjoint Check ID. |
| **DR-45 Rule Enabled** | Ein Disjoint Check: Rule Enabled ist wahr, wenn der zugehörige Rule des Disjoint Check ist enabled. |
| **DR-46 A Holds** | Ein Disjoint Check: A Holds ist wahr, wenn der zugehörige A des Disjoint Check ist Holds. |
| **DR-47 B Holds** | Ein Disjoint Check: B Holds ist wahr, wenn der zugehörige B des Disjoint Check ist Holds. |
| **DR-48 Violation Unit** | Das Feld „Violation Unit“ des Disjoint Check wird nach folgender Priorität bestimmt:<br>1. 1, wenn alles Folgende gilt: das Kennzeichen „Rule Enabled“ gesetzt ist; das Kennzeichen „A Holds“ gesetzt ist; und das Kennzeichen „B Holds“ gesetzt ist;<br>2. andernfalls 0. |

## 5 Rückverfolgbarkeit zum Schema

_Die Ausdrucksspalte ist die Definition der Regel in Dokumentation-Notation —
dieselbe Logik, die das Regelbuch speichert, geschrieben für Fachleser._

| Schema-Element | Art | Ausdruck |
|----------------|------|------------|
| **Graphs.Name** | Formel | `GraphId` |
| **Graphs.CountOfStatedFacts** | Rollup | `Sum(Statements.StatedUnit via Graph)` |
| **Graphs.CountOfConclusions** | Rollup | `Sum(Statements.ConclusionUnit via Graph)` |
| **Graphs.CountOfViolations** | Rollup | `Sum(DisjointChecks.ViolationUnit via Graph)` |
| **Graphs.IsConsistent** | Formel | `CountOfViolations = 0` |
| **Rules.Name** | Formel | `Label` |
| **Rules.CountOfFirings** | Rollup | `Sum(Firings.FiredUnit via Rule)` |
| **Things.Name** | Formel | `Label` |
| **Things.CountOfChildren** | Rollup | `Sum(Statements.ParentUnit via Subject)` |
| **Things.CountOfGrandchildren** | Rollup | `Sum(Statements.GrandparentUnit via Subject)` |
| **Properties.Name** | Formel | `Label` |
| **Statements.SubjectLabel** | Nachschlagewert | `Lookup(Things.Label via Subject)` |
| **Statements.PropertyLabel** | Nachschlagewert | `Lookup(Properties.Label via Property)` |
| **Statements.ObjectLabel** | Nachschlagewert | `Lookup(Things.Label via ObjectThing)` |
| **Statements.Name** | Formel | `SubjectLabel & " " & PropertyLabel & " " & If(ObjectThing = Blank(), NumberValue, ObjectLabel)` |
| **Statements.CountOfInverseFirings** | Rollup | `Sum(Firings.InverseFiredUnit via Target)` |
| **Statements.CountOfSymmetricFirings** | Rollup | `Sum(Firings.SymmetricFiredUnit via Target)` |
| **Statements.HoldsBeforeChain** | Formel | `Or(IsStated, CountOfInverseFirings > 0, CountOfSymmetricFirings > 0)` |
| **Statements.CountOfChainFirings** | Rollup | `Sum(Firings.ChainFiredUnit via Target)` |
| **Statements.HoldsBeforeTyping** | Formel | `Or(HoldsBeforeChain, CountOfChainFirings > 0)` |
| **Statements.CountOfTypeFirings** | Rollup | `Sum(Firings.TypeFiredUnit via Target)` |
| **Statements.Holds** | Formel | `Or(HoldsBeforeTyping, CountOfTypeFirings > 0)` |
| **Statements.IsConclusion** | Formel | `And(Holds, Not(IsStated))` |
| **Statements.WorkedOutBy** | Formel | `If(Not(IsConclusion), "", If(CountOfInverseFirings > 0, "inverse", If(CountOfSymmetricFirings > 0, "symmetric", If(CountOfChainFirings > 0, "property chain", "domain and range"))))` |
| **Statements.StatedUnit** | Formel | `If(IsStated, 1, 0)` |
| **Statements.ConclusionUnit** | Formel | `If(IsConclusion, 1, 0)` |
| **Statements.ParentUnit** | Formel | `If(And(Holds, Property = "isParentOf"), 1, 0)` |
| **Statements.GrandparentUnit** | Formel | `If(And(Holds, Property = "isGrandparentOf"), 1, 0)` |
| **Firings.RuleEnabled** | Nachschlagewert | `Lookup(Rules.IsEnabled via Rule)` |
| **Firings.RuleLabel** | Nachschlagewert | `Lookup(Rules.Label via Rule)` |
| **Firings.TargetName** | Nachschlagewert | `Lookup(Statements.Name via Target)` |
| **Firings.FirstName** | Nachschlagewert | `Lookup(Statements.Name via First)` |
| **Firings.SecondName** | Nachschlagewert | `Lookup(Statements.Name via Second)` |
| **Firings.FirstIsStated** | Nachschlagewert | `Lookup(Statements.IsStated via First)` |
| **Firings.FirstHoldsBeforeChain** | Nachschlagewert | `Lookup(Statements.HoldsBeforeChain via First)` |
| **Firings.SecondHoldsBeforeChain** | Nachschlagewert | `Lookup(Statements.HoldsBeforeChain via Second)` |
| **Firings.FirstHoldsBeforeTyping** | Nachschlagewert | `Lookup(Statements.HoldsBeforeTyping via First)` |
| **Firings.InverseFiredUnit** | Formel | `If(And(RuleEnabled, Kind = "inverse", FirstIsStated), 1, 0)` |
| **Firings.SymmetricFiredUnit** | Formel | `If(And(RuleEnabled, Kind = "symmetric", FirstIsStated), 1, 0)` |
| **Firings.ChainFiredUnit** | Formel | `If(And(RuleEnabled, Kind = "chain", FirstHoldsBeforeChain, SecondHoldsBeforeChain), 1, 0)` |
| **Firings.TypeFiredUnit** | Formel | `If(And(RuleEnabled, Or(Kind = "domain", Kind = "range"), FirstHoldsBeforeTyping), 1, 0)` |
| **Firings.FiredUnit** | Formel | `InverseFiredUnit + SymmetricFiredUnit + ChainFiredUnit + TypeFiredUnit` |
| **Firings.Name** | Formel | `If(FiredUnit > 0, TargetName & ": concluded by the " & RuleLabel & " rule from '" & FirstName & "'" & If(Kind = "chain", " and '" & SecondName & "'", ""), FiringId)` |
| **DisjointChecks.Name** | Formel | `DisjointCheckId` |
| **DisjointChecks.RuleEnabled** | Nachschlagewert | `Lookup(Rules.IsEnabled via Rule)` |
| **DisjointChecks.AHolds** | Nachschlagewert | `Lookup(Statements.Holds via A)` |
| **DisjointChecks.BHolds** | Nachschlagewert | `Lookup(Statements.Holds via B)` |
| **DisjointChecks.ViolationUnit** | Formel | `If(And(RuleEnabled, AHolds, BHolds), 1, 0)` |
