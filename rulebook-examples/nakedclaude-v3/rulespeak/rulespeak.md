# 📘 v3: NakedClaude Demo — RuleSpeak

_Rulebook generated from Airtable base 'v3: NakedClaude Demo'._

> Declarative business rules rendered from the rulebook. Every statement
> below expresses truth in the business domain — it is neither a procedure
> nor an imperative. The rulebook's formulas are the single source of truth;
> this document is their plain-language reading.

## 1 Business Vocabulary

| Term | Meaning (business sense) |
|------|--------------------------|
| **Customer** | A customer tracked by the business. |
| Is Stopped | True when the customer's current Status is a blocking status. Lookup of Statuses.IsBlocking. |
| Status is Blocking | True when the customer's current Status is a blocking status. Lookup of Statuses.IsBlocking. |
| Status Display Name | Human-readable display name of the customer's current Status. Lookup of Statuses.DisplayName. |
| Status Description | Description of the customer's current Status. Lookup of Statuses.Description. |
| Average Order Value | Computed as the literal “Unable to generate formula”. |
| Lifetime Margin Percent | Computed as the literal “Unable to generate formula”. |
| Customer Since Days | Computed: `If(CreatedAt, Datetime_diff(Now(), CreatedAt, 'days'), Blank())`. |
| Days Since Last Order | Computed as the literal “Unable to generate formula”. |
| Count of Multiple Payment Orders | The number of orders related to the customer. |
| Has Multi Payment Orders | True when the count of multiple payment orders is greater than 0. |
| **Status** | A status tracked by the business. |
| **Product** | A product tracked by the business. |
| **Order** | An order tracked by the business. |
| Item Count | The number of order line items related to the order. |
| Total Quantity | The total quantity across the order's order line items. |
| Sub Total | The total sub total across the order's order line items. |
| Tax Amount | Computed as `SubTotal * TaxRate`. |
| Order Total | Computed as `SubTotal + TaxAmount`. |
| Total Paid | The total completed amount across the order's payments. |
| Amount Due | Computed as `Round(OrderTotal - TotalPaid, 2)`. |
| Is Paid in Full | True when `If(AmountDue` is at most `0, True(), False())`. |
| Payment Count | The number of payments related to the order. |
| Is Multi Payment Order | True when the payment count is greater than 1. |
| Last Payment Date | The total payment date across the order's payments. |
| Payment Status Label | Computed: `If(IsPaidInFull, "Paid", If(TotalPaid = 0, "Unpaid", "Partial"))`. |
| **Order Line Item** | An order line item tracked by the business. |
| Pre Discount | Computed as `Quantity * UnitPrice`. |
| Discount Amount | Computed as `PreDiscount * DiscountPercent`. |
| Sub Total | Computed as `PreDiscount - DiscountAmount`. |
| **Payment** | A payment tracked by the business. |
| Order Date | The order date of the payment's order. |
| Order Number | The order number of the payment's order. |
| Order Status | The order status of the payment's order. |
| Order Total | The order total of the payment's order. |
| Is Completed | True when `If(PaymentStatus` is `"Completed", True(), False())`. |
| Completed Amount | Computed: `If(Lower(PaymentStatus) = "completed", Amount, 0)`. |
| Order Amount Due | The amount due of the payment's order. |
| Order is Paid in Full | True when the payment's order is a paid in full. |
| **ERB Version** | An ERB version tracked by the business. |
| **ERB Customization** | An ERB customization tracked by the business. |

## 2 Fact Types

- a **customer** may reference one **status**
- an **order** may reference one **customer**
- an **order line item** may reference one **order**
- an **order line item** may reference one **product**
- a **payment** may reference one **order**

## 3 Definitional Rules

_All statements express truth in the business domain; they are neither
procedures nor imperatives. "iff" is avoided in favor of "only if" so a
one-directional necessity is not mistaken for an equivalence._

| ID | Declarative rule |
|----|------------------|
| **DR-1 Is Stopped** | A customer is considered stopped if at least one of the following holds: all of the following hold: `If(StatusIsBlocking, True(), False())` and the name is the literal “Bobby” or the has multi payment orders flag is set. |
| **DR-2 Status is Blocking** | A customer's status is blocking is true when the customer's status is blocking. |
| **DR-3 Status Display Name** | A customer's status display name is the display name of the customer's status. |
| **DR-4 Status Description** | A customer's status description is the description of the customer's status. |
| **DR-5 Average Order Value** | A customer's average order value is computed as the literal “Unable to generate formula”. |
| **DR-6 Lifetime Margin Percent** | A customer's lifetime margin percent is computed as the literal “Unable to generate formula”. |
| **DR-7 Customer Since Days** | The customer's customer since days is determined by the following priority:<br>1. `Datetime_diff(Now(), CreatedAt, 'days')`, if the created at flag is set;<br>2. otherwise `Blank()`. |
| **DR-8 Days Since Last Order** | A customer's days since last order is computed as the literal “Unable to generate formula”. |
| **DR-9 Count of Multiple Payment Orders** | A customer's count of multiple payment orders is the number of orders related to the customer. |
| **DR-10 Has Multi Payment Orders** | A customer is considered to have a multi payment orders if the count of multiple payment orders is greater than 0. |
| **DR-11 Item Count** | An order's item count is the number of order line items related to the order. |
| **DR-12 Total Quantity** | An order's total quantity is the total quantity across the order line items related to the order. |
| **DR-13 Sub Total** | An order's sub total is the total sub total across the order line items related to the order. |
| **DR-14 Tax Amount** | An order's tax amount is computed as `SubTotal * TaxRate`. |
| **DR-15 Order Total** | An order's order total is computed as `SubTotal + TaxAmount`. |
| **DR-16 Total Paid** | An order's total paid is the total completed amount across the payments related to the order. |
| **DR-17 Amount Due** | An order's amount due is computed as `Round(OrderTotal - TotalPaid, 2)`. |
| **DR-18 Is Paid in Full** | An order is considered a paid in full if `If(AmountDue` is at most `0, True(), False())`. |
| **DR-19 Payment Count** | An order's payment count is the number of payments related to the order. |
| **DR-20 Is Multi Payment Order** | An order is considered a multi payment order if the payment count is greater than 1. |
| **DR-21 Last Payment Date** | An order's last payment date is the total payment date across the payments related to the order. |
| **DR-22 Payment Status Label** | The order's payment status label is determined by the following priority:<br>1. the literal “Paid”, if the is paid in full flag is set;<br>2. the literal “Unpaid”, if the total paid is 0;<br>3. otherwise the literal “Partial”. |
| **DR-23 Pre Discount** | An order line item's pre discount is computed as `Quantity * UnitPrice`. |
| **DR-24 Discount Amount** | An order line item's discount amount is computed as `PreDiscount * DiscountPercent`. |
| **DR-25 Sub Total** | An order line item's sub total is computed as `PreDiscount - DiscountAmount`. |
| **DR-26 Order Date** | A payment's order date is the order date of the payment's order. |
| **DR-27 Order Number** | A payment's order number is the order number of the payment's order. |
| **DR-28 Order Status** | A payment's order status is the order status of the payment's order. |
| **DR-29 Order Total** | A payment's order total is the order total of the payment's order. |
| **DR-30 Is Completed** | A payment is considered completed if `If(PaymentStatus` is `"Completed", True(), False())`. |
| **DR-31 Completed Amount** | The payment's completed amount is determined by the following priority:<br>1. the amount, if the lower-cased the payment status is the literal “completed”;<br>2. otherwise 0. |
| **DR-32 Order Amount Due** | A payment's order amount due is the amount due of the payment's order. |
| **DR-33 Order is Paid in Full** | A payment's order is paid in full is true when the payment's order is a paid in full. |

## 4 Traceability to Schema

_The expression column is the rule's definition in RuleSpeak notation —
the same logic the rulebook stores, written for a business reader._

| Schema element | Kind | Expression |
|----------------|------|------------|
| **Customers.IsStopped** | formula | `Or(And(If(StatusIsBlocking, True(), False()), Name="Bobby"), HasMultiPaymentOrders)` |
| **Customers.StatusIsBlocking** | lookup | `Lookup(Statuses.IsBlocking via Status)` |
| **Customers.StatusDisplayName** | lookup | `Lookup(Statuses.DisplayName via Status)` |
| **Customers.StatusDescription** | lookup | `Lookup(Statuses.Description via Status)` |
| **Customers.AverageOrderValue** | formula | `"Unable to generate formula"` |
| **Customers.LifetimeMarginPercent** | formula | `"Unable to generate formula"` |
| **Customers.CustomerSinceDays** | formula | `If(CreatedAt, Datetime_diff(Now(), CreatedAt, 'days'), Blank())` |
| **Customers.DaysSinceLastOrder** | formula | `"Unable to generate formula"` |
| **Customers.CountOfMultiplePaymentOrders** | rollup | `Count(Orders via Customer)` |
| **Customers.HasMultiPaymentOrders** | formula | `CountOfMultiplePaymentOrders > 0` |
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
| **OrderLineItems.PreDiscount** | formula | `Quantity * UnitPrice` |
| **OrderLineItems.DiscountAmount** | formula | `PreDiscount * DiscountPercent` |
| **OrderLineItems.SubTotal** | formula | `PreDiscount - DiscountAmount` |
| **Payments.OrderDate** | lookup | `Lookup(Orders.OrderDate via Order)` |
| **Payments.OrderNumber** | lookup | `Lookup(Orders.OrderNumber via Order)` |
| **Payments.OrderStatus** | lookup | `Lookup(Orders.OrderStatus via Order)` |
| **Payments.OrderTotal** | lookup | `Lookup(Orders.OrderTotal via Order)` |
| **Payments.IsCompleted** | formula | `If(PaymentStatus = "Completed", True(), False())` |
| **Payments.CompletedAmount** | formula | `If(Lower(PaymentStatus) = "completed", Amount, 0)` |
| **Payments.OrderAmountDue** | lookup | `Lookup(Orders.AmountDue via Order)` |
| **Payments.OrderIsPaidInFull** | lookup | `Lookup(Orders.IsPaidInFull via Order)` |
