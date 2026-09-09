# 📘 v3: NakedClaude Demo — RuleSpeak®

_Rulebook generated from Airtable base 'v3: NakedClaude Demo'._

> Declarative business rules rendered from the rulebook. Every statement
> below expresses truth in the business domain — it is neither a procedure
> nor an imperative. The rulebook's formulas are the single source of truth;
> this document is their plain-language reading.

## 1 Business Vocabulary

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Customer** | A customer is identified by its name and is related to optionally a status (its status). | — |
| Name | A defined attribute. | _Human-readable customer name._ |
| Notes | A defined attribute. | _Free-form notes about the customer._ |
| Is Stopped | True when at least one of the following holds: all of the following hold: the status is blocking flag is set and the name is “Bobby” or the multi payment orders flag is set. | _True when the customer's current Status is a blocking status. Lookup of Statuses.IsBlocking._ |
| Status is Blocking | True when the linked status is blocking. | _True when the customer's current Status is a blocking status. Lookup of Statuses.IsBlocking._ |
| Status | A defined attribute. | _Link to the customer's current Status in the Statuses table._ |
| Status Display Name | Taken from the linked status. | _Human-readable display name of the customer's current Status. Lookup of Statuses.DisplayName._ |
| Status Description | Taken from the linked status. | _Description of the customer's current Status. Lookup of Statuses.Description._ |
| Orders | A defined attribute. | _Reverse link to all Orders placed by this customer._ |
| Email | A defined attribute. | _Primary email address for the customer._ |
| Phone | A defined attribute. | _Primary phone number for the customer._ |
| Company Name | A defined attribute. | _Company the customer represents, if applicable._ |
| Billing Address | A defined attribute. | _Default billing address for the customer._ |
| Shipping Address | A defined attribute. | _Default shipping address for the customer._ |
| Created At | A defined attribute. | _Timestamp when the customer record was created._ |
| Average Order Value | Computed as “Unable to generate formula”. | — |
| Lifetime Margin Percent | Computed as “Unable to generate formula”. | — |
| Customer Since Days | Determined by priority: the number of days from the created at to the current date and time if the created at flag is set; in all other cases, no value. ⚠︎ mechanical <!-- rulespeak:reword --> | — |
| Days Since Last Order | Computed as “Unable to generate formula”. | — |
| Count of Multiple Payment Orders | The number of orders related to the customer. | — |
| Has Multi Payment Orders | True when the count of multiple payment orders is greater than 0. | — |
| **Status** | A status is identified by its name. | — |
| Name | Computed as the lower-cased display name. | _Lowercase, dash-separated compound key derived from DisplayName. Used as the natural key for the status._ |
| Display Name | A defined attribute. | _Human-readable display name of the status (e.g., "On-Hold", "In-Review")._ |
| Description | A defined attribute. | _Free-form description of what this status means and when it applies._ |
| Is Blocking | True when an empty string. | _True when customers in this status should be considered stopped (blocked from progressing)._ |
| Customers | A defined attribute. | _Reverse link to all Customers currently in this status._ |
| Sort Order | A defined attribute. | _Display order for this status in lists._ |
| **Product** | A product is identified by its name. | — |
| Name | Computed as the lower-cased SKU with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> | _Lowercase, dash-separated compound key derived from SKU. Used as the natural key for the product._ |
| SKU | A defined attribute. | _Stock-keeping unit identifier for the product. The natural compound key._ |
| Order Line Items | A defined attribute. | _Reverse link to all OrderLineItems that reference this product._ |
| Display Name | A defined attribute. | _Human-readable product name shown in catalogs and orders._ |
| Description | A defined attribute. | _Long-form product description._ |
| Unit Price | A defined attribute. | _Default list price per unit._ |
| Cost | A defined attribute. | _Internal cost per unit._ |
| Is Active | True when an empty string. | _True when the product is currently sellable._ |
| Stock Quantity | A defined attribute. | _Number of units currently in stock._ |
| **Order** | An order is identified by its name and is related to optionally a customer. | — |
| Name | Computed as the lower-cased customer, followed by a hyphen, followed by the order number with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> | _Lowercase compound key combining the linked Customer and OrderNumber (e.g. 'bob-1001'). Used as the natural key for the order._ |
| Order Number | A defined attribute. | _Sequential or human-assigned order identifier (e.g. '1001'). Combined with Customer to form the order's natural key._ |
| Customer | A defined attribute. | _Link to the Customer who placed this order._ |
| Order Line Items | A defined attribute. | _Reverse link to all OrderLineItems on this order._ |
| Payments | A defined attribute. | _Reverse link to all Payments applied to this order._ |
| Order Date | A defined attribute. | _When the order was placed._ |
| Order Status | A defined attribute. | _Lifecycle state of the order._ |
| Shipping Address | A defined attribute. | _Where this specific order ships to._ |
| Billing Address | A defined attribute. | _Billing address used for this specific order._ |
| Notes | A defined attribute. | _Free-form notes about this order._ |
| Item Count | The number of order line items related to the order. | — |
| Total Quantity | The total quantity across the order line items related to the order. | — |
| Sub Total | The total sub total across the order line items related to the order. | — |
| Tax Rate | A defined attribute. | _Sales tax rate applied to this order._ |
| Tax Amount | Computed as the sub total times the tax rate. | — |
| Order Total | Computed as the sub total plus the tax amount. | — |
| Total Paid | The total completed amount across the payments related to the order. | — |
| Amount Due | Computed as the order total minus the total paid rounded to 2 decimal place(s). | — |
| Is Paid in Full | True when the amount due is at most 0. | — |
| Payment Count | The number of payments related to the order. | — |
| Is Multi Payment Order | True when the payment count is greater than 1. | — |
| Last Payment Date | The total payment date across the payments related to the order. | — |
| Payment Status Label | Determined by priority: “Paid” if the paid in full flag is set; “Unpaid” if the total paid is 0; in all other cases, “Partial”. | — |
| **Order Line Item** | An order line item is identified by its name and is related to optionally an order and optionally a product. | — |
| Name | Computed as the lower-cased order, followed by “-line-”, followed by the line number with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> | _Lowercase compound key combining the linked Order and LineNumber (e.g. 'bob-1001-line-1'). Used as the natural key for the line item._ |
| Line Number | A defined attribute. | _Position of this line within its parent Order (e.g. '1', '2'). Combined with Order to form the line item's natural key._ |
| Order | A defined attribute. | _Link to the parent Order this line item belongs to._ |
| Product | A defined attribute. | _Link to the Product being ordered on this line._ |
| Quantity | A defined attribute. | _Number of units ordered on this line._ |
| Unit Price | A defined attribute. | _Price per unit captured at the time of ordering (may differ from current Product.UnitPrice)._ |
| Pre Discount | Computed as the quantity times the unit price. | — |
| Discount Percent | A defined attribute. | _Per-line discount applied to this item._ |
| Discount Amount | Computed as the pre discount times the discount percent. | — |
| Sub Total | Computed as the pre discount minus the discount amount. | — |
| Notes | A defined attribute. | _Free-form notes about this line item._ |
| **Payment** | A payment is identified by its name and is related to optionally an order. | — |
| Name | Computed as the lower-cased order, followed by “-pmt-”, followed by the payment number with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> | _Lowercase compound key combining the linked Order and PaymentNumber (e.g. 'bob-1001-pmt-1'). Used as the natural key for the payment._ |
| Payment Number | A defined attribute. | _Sequential payment identifier within the parent Order (e.g. '1', '2'). Combined with Order to form the payment's natural key._ |
| Order | A defined attribute. | _Link to the Order this payment is being applied to._ |
| Payment Date | A defined attribute. | _When the payment was received._ |
| Payment Method | A defined attribute. | _How the payment was made._ |
| Transaction ID | A defined attribute. | _External processor transaction identifier._ |
| Notes | A defined attribute. | _Free-form notes about this payment._ |
| Order Date | Taken from the linked order. | — |
| Order Number | Taken from the linked order. | — |
| Order Status | Taken from the linked order. | — |
| Order Total | Taken from the linked order. | — |
| Amount | A defined attribute. | _Amount paid._ |
| Payment Status | A defined attribute. | _Lifecycle state of the payment._ |
| Is Completed | True when the payment status is “Completed”. | — |
| Completed Amount | Determined by priority: the amount if the payment status is “completed”; in all other cases, 0. | — |
| Order Amount Due | Taken from the linked order. | — |
| Order is Paid in Full | True when the linked order is a paid in full. | — |
| **ERB Version** | An ERB version is identified by its name. | — |
| Name | A defined attribute. | — |
| Base ID | A defined attribute. | — |
| Version | A defined attribute. | — |
| Message | A defined attribute. | — |
| Notes | A defined attribute. | — |
| Commit Date | A defined attribute. | — |
| Is Published | True when an empty string. | — |
| Author | A defined attribute. | — |
| **ERB Customization** | An ERB customization is identified by its name. | — |
| Name | A defined attribute. | — |
| Title | A defined attribute. | — |
| SQL Code | A defined attribute. | — |
| SQL Target | A defined attribute. | — |
| Customization Type | A defined attribute. | — |

## 2 Fact Types

- a **customer** may reference one **status**
- an **order** may reference one **customer**
- an **order line item** may reference one **order**
- an **order line item** may reference one **product**
- a **payment** may reference one **order**

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
| **DR-1 Is Stopped** | A customer is considered stopped if at least one of the following holds: all of the following hold: the status is blocking flag is set and the name is “Bobby” or the multi payment orders flag is set. |
| **DR-2 Status is Blocking** | A customer's status is blocking when the linked status is blocking. |
| **DR-3 Status Display Name** | A customer's status display name — taken from the linked status. |
| **DR-4 Status Description** | A customer's status description — taken from the linked status. |
| **DR-5 Average Order Value** | A customer's average order value is computed as “Unable to generate formula”. |
| **DR-6 Lifetime Margin Percent** | A customer's lifetime margin percent is computed as “Unable to generate formula”. |
| **DR-7 Customer Since Days** | The customer's customer since days is determined by the following priority:<br>1. the number of days from the created at to the current date and time, if the created at flag is set;<br>2. in all other cases, no value. ⚠︎ mechanical <!-- rulespeak:reword --> |
| **DR-8 Days Since Last Order** | A customer's days since last order is computed as “Unable to generate formula”. |
| **DR-9 Count of Multiple Payment Orders** | A customer's count of multiple payment orders is the number of orders related to the customer. |
| **DR-10 Has Multi Payment Orders** | A customer is considered to have a multi payment orders if the count of multiple payment orders is greater than 0. |
| **DR-11 Name** | A status's name is computed as the lower-cased display name. |
| **DR-12 Name** | A product's name is computed as the lower-cased SKU with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> |
| **DR-13 Name** | An order's name is computed as the lower-cased customer, followed by a hyphen, followed by the order number with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> |
| **DR-14 Item Count** | An order's item count is the number of order line items related to the order. |
| **DR-15 Total Quantity** | An order's total quantity is the total quantity across the order line items related to the order. |
| **DR-16 Sub Total** | An order's sub total is the total sub total across the order line items related to the order. |
| **DR-17 Tax Amount** | An order's tax amount is computed as the sub total times the tax rate. |
| **DR-18 Order Total** | An order's order total is computed as the sub total plus the tax amount. |
| **DR-19 Total Paid** | An order's total paid is the total completed amount across the payments related to the order. |
| **DR-20 Amount Due** | An order's amount due is computed as the order total minus the total paid rounded to 2 decimal place(s). |
| **DR-21 Is Paid in Full** | An order is considered a paid in full if the amount due is at most 0. |
| **DR-22 Payment Count** | An order's payment count is the number of payments related to the order. |
| **DR-23 Is Multi Payment Order** | An order is considered a multi payment order if the payment count is greater than 1. |
| **DR-24 Last Payment Date** | An order's last payment date is the total payment date across the payments related to the order. |
| **DR-25 Payment Status Label** | The order's payment status label is determined by the following priority:<br>1. “Paid”, if the paid in full flag is set;<br>2. “Unpaid”, if the total paid is 0;<br>3. in all other cases, “Partial”. |
| **DR-26 Name** | An order line item's name is computed as the lower-cased order, followed by “-line-”, followed by the line number with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> |
| **DR-27 Pre Discount** | An order line item's pre discount is computed as the quantity times the unit price. |
| **DR-28 Discount Amount** | An order line item's discount amount is computed as the pre discount times the discount percent. |
| **DR-29 Sub Total** | An order line item's sub total is computed as the pre discount minus the discount amount. |
| **DR-30 Name** | A payment's name is computed as the lower-cased order, followed by “-pmt-”, followed by the payment number with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> |
| **DR-31 Order Date** | A payment's order date — taken from the linked order. |
| **DR-32 Order Number** | A payment's order number — taken from the linked order. |
| **DR-33 Order Status** | A payment's order status — taken from the linked order. |
| **DR-34 Order Total** | A payment's order total — taken from the linked order. |
| **DR-35 Is Completed** | A payment is considered completed if the payment status is “Completed”. |
| **DR-36 Completed Amount** | The payment's completed amount is determined by the following priority:<br>1. the amount, if the payment status is “completed”;<br>2. in all other cases, 0. |
| **DR-37 Order Amount Due** | A payment's order amount due — taken from the linked order. |
| **DR-38 Order is Paid in Full** | A payment's order is paid in full when the linked order is a paid in full. |

## 5 Traceability to Schema

_The expression column is the rule's definition in RuleSpeak® notation —
the same logic the rulebook stores, written for a business reader._

| Schema element | Kind | Expression |
|----------------|------|------------|
| **Customers.IsStopped** | formula | `Or(And(If(StatusIsBlocking, True(), False()), Name = "Bobby"), HasMultiPaymentOrders)` |
| **Customers.StatusIsBlocking** | lookup | `Lookup(Statuses.IsBlocking via Status)` |
| **Customers.StatusDisplayName** | lookup | `Lookup(Statuses.DisplayName via Status)` |
| **Customers.StatusDescription** | lookup | `Lookup(Statuses.Description via Status)` |
| **Customers.AverageOrderValue** | formula | `"Unable to generate formula"` |
| **Customers.LifetimeMarginPercent** | formula | `"Unable to generate formula"` |
| **Customers.CustomerSinceDays** | formula | `If(CreatedAt, DaysBetween(Now(), CreatedAt), Blank())` |
| **Customers.DaysSinceLastOrder** | formula | `"Unable to generate formula"` |
| **Customers.CountOfMultiplePaymentOrders** | rollup | `Count(Orders via Customer)` |
| **Customers.HasMultiPaymentOrders** | formula | `CountOfMultiplePaymentOrders > 0` |
| **Statuses.Name** | formula | `Lower(DisplayName)` |
| **Products.Name** | formula | `Lower(Replace(SKU, " ", "-"))` |
| **Orders.Name** | formula | `Lower(Replace(Customer & "-" & OrderNumber, " ", "-"))` |
| **Orders.ItemCount** | rollup | `Count(OrderLineItems via Order)` |
| **Orders.TotalQuantity** | rollup | `Sum(OrderLineItems.Quantity via Order)` |
| **Orders.SubTotal** | rollup | `Sum(OrderLineItems.SubTotal via Order)` |
| **Orders.TaxAmount** | formula | `SubTotal * TaxRate` |
| **Orders.OrderTotal** | formula | `SubTotal + TaxAmount` |
| **Orders.TotalPaid** | rollup | `Sum(Payments.CompletedAmount via Order)` |
| **Orders.AmountDue** | formula | `Round(OrderTotal - TotalPaid, 2)` |
| **Orders.IsPaidInFull** | formula | `If(AmountDue <= 0, True(), False())` |
| **Orders.PaymentCount** | rollup | `Count(Payments via Order)` |
| **Orders.IsMultiPaymentOrder** | formula | `PaymentCount > 1` |
| **Orders.LastPaymentDate** | rollup | `Sum(Payments.PaymentDate via Order)` |
| **Orders.PaymentStatusLabel** | formula | `If(IsPaidInFull, "Paid", If(TotalPaid = 0, "Unpaid", "Partial"))` |
| **OrderLineItems.Name** | formula | `Lower(Replace(Order & "-line-" & LineNumber, " ", "-"))` |
| **OrderLineItems.PreDiscount** | formula | `Quantity * UnitPrice` |
| **OrderLineItems.DiscountAmount** | formula | `PreDiscount * DiscountPercent` |
| **OrderLineItems.SubTotal** | formula | `PreDiscount - DiscountAmount` |
| **Payments.Name** | formula | `Lower(Replace(Order & "-pmt-" & PaymentNumber, " ", "-"))` |
| **Payments.OrderDate** | lookup | `Lookup(Orders.OrderDate via Order)` |
| **Payments.OrderNumber** | lookup | `Lookup(Orders.OrderNumber via Order)` |
| **Payments.OrderStatus** | lookup | `Lookup(Orders.OrderStatus via Order)` |
| **Payments.OrderTotal** | lookup | `Lookup(Orders.OrderTotal via Order)` |
| **Payments.IsCompleted** | formula | `If(PaymentStatus = "Completed", True(), False())` |
| **Payments.CompletedAmount** | formula | `If(Lower(PaymentStatus) = "completed", Amount, 0)` |
| **Payments.OrderAmountDue** | lookup | `Lookup(Orders.AmountDue via Order)` |
| **Payments.OrderIsPaidInFull** | lookup | `Lookup(Orders.IsPaidInFull via Order)` |

---

_This document is rendered in **RuleSpeak®**, the declarative business-rule
notation created by **Ronald G. Ross**, and follows the conventions of
**SBVR** (Semantics of Business Vocabulary and Business Rules). With thanks to
Ronald G. Ross for RuleSpeak® and his foundational work on business rules —
[www.RonRoss.info](https://www.RonRoss.info)._
