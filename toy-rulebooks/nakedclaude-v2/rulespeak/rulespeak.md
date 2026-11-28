# 📘 v2: NakedClaude Demo — Docs

_Rulebook generated from Airtable base 'v2: NakedClaude Demo'._

> Declarative business rules rendered from the rulebook. Every statement
> below expresses truth in the business domain — it is neither a procedure
> nor an imperative. The rulebook's formulas are the single source of truth;
> this document is their plain-language reading.

## 1 Business Vocabulary

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Customer** | A customer is identified by its name and is related to optionally a status. | — |
| Name | Computed as the lower-cased full name with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> | — |
| First Name | A defined attribute. | — |
| Last Name | A defined attribute. | — |
| Full Name | Computed as the first name, followed by a space, followed by the last name. | — |
| Notes | A defined attribute. | — |
| Status Display Name | Taken from the linked status. | — |
| Status is Blocking | True when the linked status is blocking. | — |
| Is Stopped | True when all of the following hold: the status is blocking flag is set and the first name is “Bob”. | — |
| Status Description | Taken from the linked status. | — |
| Status | A defined attribute. | — |
| Owner Email | A defined attribute. | _Email address of the user who owns this customer record. Used by Row-Level Security to scope visibility to the JWT email of the requester._ |

<details open>
<summary>Example data</summary>

| Name ƒ | First Name | Last Name | Full Name ƒ | Notes | Status Display Name ƒ | Status is Blocking ƒ | Is Stopped ƒ | Status Description ƒ | Owner Email |
|---|---|---|---|---|---|---|---|---|---|
| bob- | Bob | — | Bob  | A customer currently in launch. | On-Hold | true | true | This status indicates the customer or request is on hold and temporarily paused. | eejai42@gmail.com |
| alice-johnson | Alice | Johnson | Alice Johnson | Initial mock entry for Alice. | Pending | true | false | This status shows the customer or request is pending further action or review. | eejai42@gmail.com |
| brian-lee | Brian | Lee | Brian Lee | Initial mock entry for Brian. | New | false | false | This status indicates a newly created customer or request that has not yet been processed. | eejai42@gmail.com |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Status** | A status is identified by its name. | — |
| Name | Computed as the lower-cased display name. | — |
| Display Name | A defined attribute. | — |
| Description | A defined attribute. | — |
| Is Blocking | True when an empty string. | — |
| Customers | A defined attribute. | — |
| Owner Email | A defined attribute. | _Email address of the user who owns this status. Used by Row-Level Security to scope visibility._ |

<details open>
<summary>Example data</summary>

| Name ƒ | Display Name | Description | Is Blocking | Owner Email |
|---|---|---|---|---|
| delayed | Delayed | This status is used when the customer or request has been delayed and is not progressing as scheduled. | false | ej@ssot.me |
| cancelled | Cancelled | This status means the customer or request has been cancelled and will not proceed further. | true | ej@ssot.me |
| processing | Processing | This status means the customer or request is currently being processed. | false | ej@ssot.me |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **App User** | An app user is identified by its name. | — |
| Name | Computed as the lower-cased email address with every “. ” replaced by a hyphen with every “@” replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> | — |
| Email Address | A defined attribute. | _Primary identity — matches the JWT email of the signed-in user._ |
| Role | A defined attribute. | _admin = full access; customer = sees only owned customers (RLS DAG)._ |

<details open>
<summary>Example data</summary>

| Name ƒ | Email Address | Role |
|---|---|---|
| eejai42-gmail.com | eejai42@gmail.com | customer |
| ej-ssot.me | ej@ssot.me | admin |
| alice-example.com | alice@example.com | customer |

_ƒ marks a computed column._

</details>

## 2 Fact Types

- a **customer** may reference one **status**

## 3 Operative Rules

_No operative rules yet. Required fields and foreign keys imply structural
`must`-rules automatically; to declare semantic obligations (`must` / `must not` / `should`), add a **Constraints** table whose rows point at
boolean calculated fields. See the tool README for the column contract._

## 4 Definitional Rules

_All statements express truth in the business domain; they are neither
procedures nor imperatives. "iff" is avoided in favor of "only if" so a
one-directional necessity is not mistaken for an equivalence. A
**⚠︎ mechanical** chip marks a rule whose deterministic wording is faithful
but clunky — a flag for an optional downstream reword pass, not a defect._

| ID | Declarative rule |
|----|------------------|
| **DR-1 Name** | A customer's name is computed as the lower-cased full name with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> |
| **DR-2 Full Name** | A customer's full name is computed as the first name, followed by a space, followed by the last name. |
| **DR-3 Status Display Name** | A customer's status display name — taken from the linked status. |
| **DR-4 Status is Blocking** | A customer's status is blocking when the linked status is blocking. |
| **DR-5 Is Stopped** | A customer is considered stopped if all of the following hold: the status is blocking flag is set and the first name is “Bob”. |
| **DR-6 Status Description** | A customer's status description — taken from the linked status. |
| **DR-7 Name** | A status's name is computed as the lower-cased display name. |
| **DR-8 Name** | An app user's name is computed as the lower-cased email address with every “. ” replaced by a hyphen with every “@” replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> |

## 5 Traceability to Schema

_The expression column is the rule's definition in Docs notation —
the same logic the rulebook stores, written for a business reader._

| Schema element | Kind | Expression |
|----------------|------|------------|
| **Customers.Name** | formula | `Replace(Lower(FullName), " ", "-")` |
| **Customers.FullName** | formula | `FirstName & " " & LastName` |
| **Customers.StatusDisplayName** | lookup | `Lookup(Statuses.DisplayName via Status)` |
| **Customers.StatusIsBlocking** | lookup | `Lookup(Statuses.IsBlocking via Status)` |
| **Customers.IsStopped** | formula | `If(And(StatusIsBlocking, FirstName = "Bob"), True(), False())` |
| **Customers.StatusDescription** | lookup | `Lookup(Statuses.Description via Status)` |
| **Statuses.Name** | formula | `Lower(DisplayName)` |
| **AppUsers.Name** | formula | `Lower(Replace(Replace(EmailAddress, ". ", "-"), "@", "-"))` |

---

> **RuleSpeak®** — the declarative business-rule notation created by **Ronald G. Ross**, following the conventions of **SBVR** — is coming soon. Until then this page shows the regular documentation. [www.RonRoss.info](https://www.RonRoss.info)
