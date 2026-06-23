# 📘 ACME, LLC — RuleSpeak

_Smallest viable rulebook with a calculated field — the "Hello, formulas" tutorial._

> Deklarative Geschäftsregeln, aus dem Regelbuch gerendert. Jede Aussage
> unten drückt eine Wahrheit der Geschäftsdomäne aus — sie ist weder eine
> Prozedur noch ein Befehl. Die Formeln des Regelbuchs sind die einzige Quelle
> der Wahrheit; dieses Dokument ist ihre klarsprachliche Lesart.

## 1 Geschäftsvokabular

| Begriff | Beschreibung | Erläuternder Kommentar |
|---------|--------------|------------------------|
| **Customer** | Ein customer wird durch seinen name identifiziert. | — |
| Name | Berechnet als der email address, wobei jedes der Wert „@“ durch ein Bindestrich ersetzt wird. | _Identifier for the customer._ |
| Initials | Berechnet als die ersten 1 Zeichen von der first name, gefolgt von die ersten 1 Zeichen von der last name. | _Customer initials — the first letter of FirstName followed by the first letter of LastName._ |
| Full Name | Berechnet als der first name, gefolgt von ein Leerzeichen, gefolgt von der last name. | _Full name is computed from the first and last name of the customer_ |
| **ERB Version** | Ein ERB version wird durch seinen name identifiziert. | — |
| **ERB Customization** | Ein ERB customization wird durch seinen name identifiziert. | — |

## 3 Operative Regeln

_Noch keine operativen Regeln. Pflichtfelder und Fremdschlüssel erzeugen automatisch
strukturelle `muss`-Regeln; um semantische Verpflichtungen zu deklarieren (`muss` / `darf nicht` / `sollte`), fügen Sie eine **Constraints**-Tabelle hinzu, deren Zeilen auf
boolesche berechnete Felder verweisen. Den Spaltenvertrag finden Sie in der README des Tools._

## 4 Definitorische Regeln

_Alle Aussagen drücken eine Wahrheit der Geschäftsdomäne aus; sie sind weder
Prozeduren noch Befehle. "genau dann, wenn" wird zugunsten von "nur dann, wenn"
vermieden, damit eine einseitige Notwendigkeit nicht mit einer Äquivalenz verwechselt
wird. Ein **⚠︎ mechanisch**-Chip kennzeichnet eine Regel, deren deterministische
Formulierung getreu, aber holprig ist — ein Hinweis für eine optionale spätere
Umformulierung, kein Fehler._

| ID | Deklarative Regel |
|----|-------------------|
| **DR-1 Name** | Ein customer: Name wird berechnet als der email address, wobei jedes der Wert „@“ durch ein Bindestrich ersetzt wird. |
| **DR-2 Initials** | Ein customer: Initials wird berechnet als die ersten 1 Zeichen von der first name, gefolgt von die ersten 1 Zeichen von der last name. |
| **DR-3 Full Name** | Ein customer: Full name wird berechnet als der first name, gefolgt von ein Leerzeichen, gefolgt von der last name. |

## 5 Rückverfolgbarkeit zum Schema

_Die Ausdruck-Spalte ist die Definition der Regel in RuleSpeak-Notation —
dieselbe Logik, die das Regelbuch speichert, für einen Geschäftsleser geschrieben._

| Schema-Element | Art | Ausdruck |
|----------------|-----|----------|
| **Customers.Name** | Formel | `Ersetzen(EmailAddress, "@", "-")` |
| **Customers.Initials** | Formel | `Left(FirstName, 1) & Left(LastName, 1)` |
| **Customers.FullName** | Formel | `FirstName & " " & LastName` |

---

_Dieses Dokument ist in **RuleSpeak®** gerendert, der deklarativen Geschäftsregel-
Notation von **Ronald G. Ross**, und folgt den Konventionen von
**SBVR** (Semantics of Business Vocabulary and Business Rules). Mit Dank an
Ronald G. Ross für RuleSpeak und seine grundlegende Arbeit zu Geschäftsregeln —
[www.RonRoss.info](https://www.RonRoss.info)._
