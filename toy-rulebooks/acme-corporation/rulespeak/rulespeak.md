# 📘 ACME Corporation — RuleSpeak

_CRUD starter template — clients, projects, employees, roles. The "starter-sized" demo for new authors._

> Declarative business rules rendered from the rulebook. Every statement
> below expresses truth in the business domain — it is neither a procedure
> nor an imperative. The rulebook's formulas are the single source of truth;
> this document is their plain-language reading.

## 1 Business Vocabulary

| Term | Meaning (business sense) |
|------|--------------------------|
| **Client** | A client tracked by the business. |
| Full Name | The client's full name, written first name then last name (FirstName & " " & LastName). |
| **Project** | A project tracked by the business. |
| Project Type Name | The name of the project's project type. |
| Project Type Description | The description of the project's project type. |
| Project Type Requires Manager Approval | True when the project's project type requires manager approval. |
| Approved by Role is Manager | True when the project's approved by is a manager. |
| Approved by Name | The name of the project's approved by. |
| Approved by Email Address | The email address of the project's approved by. |
| Approved by Phone Number | The phone number of the project's approved by. |
| **Employee** | An employee tracked by the business. |
| Role Name | The name of the employee's role. |
| Role Description | The description of the employee's role. |
| Role is Manager | True when the employee's role is a manager. |
| **Role** | A role tracked by the business. |
| Count of Employees | The number of employees related to the role. |
| **Type of Project** | A type of project tracked by the business. |
| Count of Projects | The number of projects related to the type of project. |

## 2 Fact Types

- a **project** may reference one **type of project**
- a **project** may reference one **employee**
- an **employee** may reference one **role**

## 3 Definitional Rules

_All statements express truth in the business domain; they are neither
procedures nor imperatives. "iff" is avoided in favor of "only if" so a
one-directional necessity is not mistaken for an equivalence._

| ID | Declarative rule |
|----|------------------|
| **DR-1 Full Name** | A client's full name is computed as the first name, followed by a space, followed by the last name. |
| **DR-2 Project Type Name** | A project's project type name is the name of the project's project type. |
| **DR-3 Project Type Description** | A project's project type description is the description of the project's project type. |
| **DR-4 Project Type Requires Manager Approval** | A project's project type requires manager approval is true when the project's project type requires manager approval. |
| **DR-5 Approved by Role is Manager** | A project's approved by role is manager is true when the project's approved by is a manager. |
| **DR-6 Approved by Name** | A project's approved by name is the name of the project's approved by. |
| **DR-7 Approved by Email Address** | A project's approved by email address is the email address of the project's approved by. |
| **DR-8 Approved by Phone Number** | A project's approved by phone number is the phone number of the project's approved by. |
| **DR-9 Role Name** | An employee's role name is the name of the employee's role. |
| **DR-10 Role Description** | An employee's role description is the description of the employee's role. |
| **DR-11 Role is Manager** | An employee's role is manager is true when the employee's role is a manager. |
| **DR-12 Count of Employees** | A role's count of employees is the number of employees related to the role. |
| **DR-13 Count of Projects** | A type of project's count of projects is the number of projects related to the type of project. |

## 4 Traceability to Schema

_The expression column is the rule's definition in RuleSpeak notation —
the same logic the rulebook stores, written for a business reader._

| Schema element | Kind | Expression |
|----------------|------|------------|
| **Client.FullName** | formula | `FirstName & " " & LastName` |
| **Projects.ProjectTypeName** | lookup | `Lookup(TypesOfProject.Name via ProjectType)` |
| **Projects.ProjectTypeDescription** | lookup | `Lookup(TypesOfProject.Description via ProjectType)` |
| **Projects.ProjectTypeRequiresManagerApproval** | lookup | `Lookup(TypesOfProject.RequiresManagerApproval via ProjectType)` |
| **Projects.ApprovedByRoleIsManager** | lookup | `Lookup(Employees.RoleIsManager via ApprovedBy)` |
| **Projects.ApprovedByName** | lookup | `Lookup(Employees.Name via ApprovedBy)` |
| **Projects.ApprovedByEmailAddress** | lookup | `Lookup(Employees.EmailAddress via ApprovedBy)` |
| **Projects.ApprovedByPhoneNumber** | lookup | `Lookup(Employees.PhoneNumber via ApprovedBy)` |
| **Employees.RoleName** | lookup | `Lookup(Roles.Name via Role)` |
| **Employees.RoleDescription** | lookup | `Lookup(Roles.Description via Role)` |
| **Employees.RoleIsManager** | lookup | `Lookup(Roles.IsManager via Role)` |
| **Roles.CountOfEmployees** | rollup | `Count(Employees via Role)` |
| **TypesOfProject.CountOfProjects** | rollup | `Count(Projects via ProjectType)` |
