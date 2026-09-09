# 📘 Product Inventory — RuleSpeak®

_Products with transactions adjusting quantities and low-stock alerts._

> Declarative business rules rendered from the rulebook. Every statement
> below expresses truth in the business domain — it is neither a procedure
> nor an imperative. The rulebook's formulas are the single source of truth;
> this document is their plain-language reading.

## 1 Business Vocabulary

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Transaction Type** | A transaction type is identified by its name. | — |
| Name | The same as its transaction type ID. | _Display name, derived from TransactionTypeId._ |
| Description | A defined attribute. | _What this transaction type represents._ |
| **Product** | A product is identified by its name. | — |
| Name | The same as its product ID. | _Display name, derived from ProductId._ |
| Unit Price | A defined attribute. | _Price per unit in dollars._ |
| Reorder Level | A defined attribute. | _Minimum quantity before reorder is recommended._ |
| Current Quantity | The total quantity across the transactions related to the product. | _Running total of all transaction quantities for this product._ |
| Is Low Stock | True when the current quantity is less than the reorder level. | _True if current quantity falls below the reorder level._ |
| Reorder Status | Determined by priority: “This needs to be reordered IMMEDIATELY!!” if the low stock flag is set; in all other cases, “In Stock”. | _Urgent reorder message if stock is low, otherwise in stock status._ |
| **Transaction** | A transaction is identified by its name and is related to a product and a transaction type. | — |
| Name | The same as its transaction ID. | _Display name, derived from TransactionId._ |
| Product | A defined attribute. | _Reference to the product being adjusted._ |
| Product Name | Taken from the linked product. | _Product display name via lookup._ |
| Product Unit Price | Taken from the linked product. | _Product unit price via lookup, for calculating transaction amount._ |
| Transaction Type | A defined attribute. | _Type of transaction (Purchase, Sale, Adjustment)._ |
| Transaction Type Name | Taken from the linked transaction type. | _Transaction type display name via lookup._ |
| Quantity | A defined attribute. | _Quantity adjustment (positive for purchases/returns, negative for sales)._ |
| Amount | Computed as the quantity times the product unit price. | _Transaction value = Quantity × UnitPrice._ |
| Transaction Date | A defined attribute. | _When the transaction occurred._ |
| Notes | A defined attribute. | _Optional notes about this transaction._ |

## 2 Fact Types

- a **transaction** references exactly one **product**
- a **transaction** references exactly one **transaction type**

## 3 Operative Rules

_Operative rules state what the business **obliges**, **prohibits**, or
advises (**should**). Structural rules come from required fields and foreign keys;
semantic rules come from the Constraints table, each keyed on a boolean the rulebook
already computes (cross-referenced as DR-N in the Definitional Rules below)._

### Structural Constraints (from the schema)

- A product **must** have a unit price and a reorder level.
- A transaction **must** reference exactly one product.
- A transaction **must** reference exactly one transaction type.
- A transaction **must** have a quantity and a transaction date.

## 4 Definitional Rules

_All statements express truth in the business domain; they are neither
procedures nor imperatives. "iff" is avoided in favor of "only if" so a
one-directional necessity is not mistaken for an equivalence. A
**⚠︎ mechanical** chip marks a rule whose deterministic wording is faithful
but clunky — a flag for an optional downstream reword pass, not a defect._

| ID | Declarative rule |
|----|------------------|
| **DR-1 Name** | A transaction type's name is the same as its transaction type ID. |
| **DR-2 Name** | A product's name is the same as its product ID. |
| **DR-3 Current Quantity** | A product's current quantity is the total quantity across the transactions related to the product. |
| **DR-4 Is Low Stock** | A product is considered a low stock if the current quantity is less than the reorder level. |
| **DR-5 Reorder Status** | The product's reorder status is determined by the following priority:<br>1. “This needs to be reordered IMMEDIATELY!!”, if the low stock flag is set;<br>2. in all other cases, “In Stock”. |
| **DR-6 Name** | A transaction's name is the same as its transaction ID. |
| **DR-7 Product Name** | A transaction's product name — taken from the linked product. |
| **DR-8 Product Unit Price** | A transaction's product unit price — taken from the linked product. |
| **DR-9 Transaction Type Name** | A transaction's transaction type name — taken from the linked transaction type. |
| **DR-10 Amount** | A transaction's amount is computed as the quantity times the product unit price. |

## 5 Traceability to Schema

_The expression column is the rule's definition in RuleSpeak® notation —
the same logic the rulebook stores, written for a business reader._

| Schema element | Kind | Expression |
|----------------|------|------------|
| **TransactionTypes.Name** | formula | `TransactionTypeId` |
| **Products.Name** | formula | `ProductId` |
| **Products.CurrentQuantity** | rollup | `Sum(Transactions.Quantity via Product)` |
| **Products.IsLowStock** | formula | `CurrentQuantity < ReorderLevel` |
| **Products.ReorderStatus** | formula | `If(IsLowStock, "This needs to be reordered IMMEDIATELY!!", "In Stock")` |
| **Transactions.Name** | formula | `TransactionId` |
| **Transactions.ProductName** | lookup | `Lookup(Products.Name via Product)` |
| **Transactions.ProductUnitPrice** | lookup | `Lookup(Products.UnitPrice via Product)` |
| **Transactions.TransactionTypeName** | lookup | `Lookup(TransactionTypes.Name via TransactionType)` |
| **Transactions.Amount** | formula | `Quantity * ProductUnitPrice` |

---

_This document is rendered in **RuleSpeak®**, the declarative business-rule
notation created by **Ronald G. Ross**, and follows the conventions of
**SBVR** (Semantics of Business Vocabulary and Business Rules). With thanks to
Ronald G. Ross for RuleSpeak® and his foundational work on business rules —
[www.RonRoss.info](https://www.RonRoss.info)._
