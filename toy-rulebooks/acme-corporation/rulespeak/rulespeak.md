# 📘 ACME Corporation — RuleSpeak®

_CRUD starter template — clients, projects, employees, roles. The "starter-sized" demo for new authors._

> Declarative business rules rendered from the rulebook. Every statement
> below expresses truth in the business domain — it is neither a procedure
> nor an imperative. The rulebook's formulas are the single source of truth;
> this document is their plain-language reading.

## 1 Business Vocabulary

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Client** | A client is identified by its customer. | — |
| Customer | A defined attribute. | _Identifier for the customers._ |
| Email Address | A defined attribute. | _Thec ustomers email address_ |
| First Name | A defined attribute. | _First Name of the customer - used to make the full name_ |
| Last Name | A defined attribute. | _Last Name of the customer - used to make the full name_ |
| Full Name | Computed as the first name, followed by a space, followed by the last name. | _The client's full name, written first name then last name (FirstName & " " & LastName)._ |
| **Project** | A project is identified by its name and is related to optionally a type of project (its project type) and optionally an employee (its approved by). | — |
| Name | A defined attribute. | — |
| Description | A defined attribute. | — |
| Project Type | A defined attribute. | — |
| Project Type Name | Taken from the linked project type. | — |
| Project Type Description | Taken from the linked project type. | — |
| Project Type Requires Manager Approval | True when the linked project type requires manager approval. | — |
| Due Date | A defined attribute. | — |
| Is Approved | True when at least one of the following holds: the project type requires manager approval flag is not set or the approved by role is manager flag is set. | _Whether the project is validly approved. True when the project's type doesn't require manager approval, OR the person in ApprovedBy holds a manager role._ |
| Approved by | A defined attribute. | — |
| Approved by Role is Manager | True when the linked approved by is a manager. | — |
| Approved by Name | Taken from the linked approved by. | — |
| Approved by Email Address | Taken from the linked approved by. | — |
| Approved by Phone Number | Taken from the linked approved by. | — |
| **Employee** | An employee is identified by its name and is related to optionally a role. | — |
| Name | A defined attribute. | — |
| Email Address | A defined attribute. | — |
| Phone Number | A defined attribute. | — |
| Role | A defined attribute. | — |
| Role Name | Taken from the linked role. | — |
| Role Description | Taken from the linked role. | — |
| Projects | A defined attribute. | — |
| Role is Manager | True when the linked role is a manager. | — |
| **Role** | A role is identified by its name. | — |
| Name | A defined attribute. | — |
| Description | A defined attribute. | — |
| Is Manager | True when an empty string. | — |
| Employees | A defined attribute. | — |
| Count of Employees | The number of employees related to the role. | — |
| **Type of Project** | A type of project is identified by its name. | — |
| Name | A defined attribute. | — |
| Description | A defined attribute. | — |
| Requires Manager Approval | True when an empty string. | — |
| Projects | A defined attribute. | — |
| Count of Projects | The number of projects related to the type of project. | — |

## 2 Fact Types

- a **project** may reference one **type of project**
- a **project** may reference one **employee**
- an **employee** may reference one **role**

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
| **DR-1 Full Name** | A client's full name is computed as the first name, followed by a space, followed by the last name. |
| **DR-2 Project Type Name** | A project's project type name — taken from the linked project type. |
| **DR-3 Project Type Description** | A project's project type description — taken from the linked project type. |
| **DR-4 Project Type Requires Manager Approval** | A project's project type requires manager approval when the linked project type requires manager approval. |
| **DR-5 Is Approved** | A project is considered approved if at least one of the following holds: the project type requires manager approval flag is not set or the approved by role is manager flag is set. |
| **DR-6 Approved by Role is Manager** | A project's approved by role is manager when the linked approved by is a manager. |
| **DR-7 Approved by Name** | A project's approved by name — taken from the linked approved by. |
| **DR-8 Approved by Email Address** | A project's approved by email address — taken from the linked approved by. |
| **DR-9 Approved by Phone Number** | A project's approved by phone number — taken from the linked approved by. |
| **DR-10 Role Name** | An employee's role name — taken from the linked role. |
| **DR-11 Role Description** | An employee's role description — taken from the linked role. |
| **DR-12 Role is Manager** | An employee's role is manager when the linked role is a manager. |
| **DR-13 Count of Employees** | A role's count of employees is the number of employees related to the role. |
| **DR-14 Count of Projects** | A type of project's count of projects is the number of projects related to the type of project. |

## 5 Traceability to Schema

_The expression column is the rule's definition in RuleSpeak® notation —
the same logic the rulebook stores, written for a business reader._

| Schema element | Kind | Expression |
|----------------|------|------------|
| **Client.FullName** | formula | `FirstName & " " & LastName` |
| **Projects.ProjectTypeName** | lookup | `Lookup(TypesOfProject.Name via ProjectType)` |
| **Projects.ProjectTypeDescription** | lookup | `Lookup(TypesOfProject.Description via ProjectType)` |
| **Projects.ProjectTypeRequiresManagerApproval** | lookup | `Lookup(TypesOfProject.RequiresManagerApproval via ProjectType)` |
| **Projects.IsApproved** | formula | `Or(Not(ProjectTypeRequiresManagerApproval), ApprovedByRoleIsManager)` |
| **Projects.ApprovedByRoleIsManager** | lookup | `Lookup(Employees.RoleIsManager via ApprovedBy)` |
| **Projects.ApprovedByName** | lookup | `Lookup(Employees.Name via ApprovedBy)` |
| **Projects.ApprovedByEmailAddress** | lookup | `Lookup(Employees.EmailAddress via ApprovedBy)` |
| **Projects.ApprovedByPhoneNumber** | lookup | `Lookup(Employees.PhoneNumber via ApprovedBy)` |
| **Employees.RoleName** | lookup | `Lookup(Roles.Name via Role)` |
| **Employees.RoleDescription** | lookup | `Lookup(Roles.Description via Role)` |
| **Employees.RoleIsManager** | lookup | `Lookup(Roles.IsManager via Role)` |
| **Roles.CountOfEmployees** | rollup | `Count(Employees via Role)` |
| **TypesOfProject.CountOfProjects** | rollup | `Count(Projects via ProjectType)` |

---

_This document is rendered in **RuleSpeak®**, the declarative business-rule
notation created by **Ronald G. Ross**, and follows the conventions of
**SBVR** (Semantics of Business Vocabulary and Business Rules). With thanks to
Ronald G. Ross for RuleSpeak® and his foundational work on business rules —
[www.RonRoss.info](https://www.RonRoss.info)._
