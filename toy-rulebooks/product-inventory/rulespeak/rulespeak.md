# 📘 Product Inventory — RuleSpeak

_Products with transactions adjusting quantities and low-stock alerts._

> Declarative business rules rendered from the rulebook. Every statement
> below expresses truth in the business domain — it is neither a procedure
> nor an imperative. The rulebook's formulas are the single source of truth;
> this document is their plain-language reading.

## 1 Business Vocabulary

| Term | Meaning (business sense) |
|------|--------------------------|
| **Transaction Type** | A transaction type tracked by the business. |
| **Product** | A product tracked by the business. |
| Current Quantity | Running total of all transaction quantities for this product. |
| Is Low Stock | True if current quantity falls below the reorder level. |
| Reorder Status | Urgent reorder message if stock is low, otherwise in stock status. |
| **Transaction** | A transaction tracked by the business. |
| Product Name | Product display name via lookup. |
| Product Unit Price | Product unit price via lookup, for calculating transaction amount. |
| Transaction Type Name | Transaction type display name via lookup. |
| Amount | Transaction value = Quantity × UnitPrice. |

## 2 Fact Types

- a **transaction** references exactly one **product**
- a **transaction** references exactly one **transaction type**

## 3 Definitional Rules

_All statements express truth in the business domain; they are neither
procedures nor imperatives. "iff" is avoided in favor of "only if" so a
one-directional necessity is not mistaken for an equivalence._

| ID | Declarative rule |
|----|------------------|
| **DR-1 Current Quantity** | A product's current quantity is the total quantity across the transactions related to the product. |
| **DR-2 Is Low Stock** | A product is considered a low stock if the current quantity is less than the reorder level. |
| **DR-3 Reorder Status** | The product's reorder status is determined by the following priority:<br>1. the literal “This needs to be reordered IMMEDIATELY!!”, if the is low stock flag is set;<br>2. otherwise the literal “In Stock”. |
| **DR-4 Product Name** | A transaction's product name is the name of the transaction's product. |
| **DR-5 Product Unit Price** | A transaction's product unit price is the unit price of the transaction's product. |
| **DR-6 Transaction Type Name** | A transaction's transaction type name is the name of the transaction's transaction type. |
| **DR-7 Amount** | A transaction's amount is computed as `Quantity * ProductUnitPrice`. |

## 4 Traceability to Schema

_The expression column is the rule's definition in RuleSpeak notation —
the same logic the rulebook stores, written for a business reader._

| Schema element | Kind | Expression |
|----------------|------|------------|
| **Products.CurrentQuantity** | rollup | `Sum(Transactions.Quantity via Product)` |
| **Products.IsLowStock** | formula | `CurrentQuantity < ReorderLevel` |
| **Products.ReorderStatus** | formula | `If(IsLowStock, "This needs to be reordered IMMEDIATELY!!", "In Stock")` |
| **Transactions.ProductName** | lookup | `Lookup(Products.Name via Product)` |
| **Transactions.ProductUnitPrice** | lookup | `Lookup(Products.UnitPrice via Product)` |
| **Transactions.TransactionTypeName** | lookup | `Lookup(TransactionTypes.Name via TransactionType)` |
| **Transactions.Amount** | formula | `Quantity * ProductUnitPrice` |
