# 📘 Customer CRM — RuleSpeak®

_Fighter-jet FCS sales pipeline rolling revenue up by order, FCS variant, and jet model._

> Declarative business rules rendered from the rulebook. Every statement
> below expresses truth in the business domain — it is neither a procedure
> nor an imperative. The rulebook's formulas are the single source of truth;
> this document is their plain-language reading.

## 1 Business Vocabulary

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **User** | Dev-login identities for the demo app. Email is the PK. | — |
| Name | The same as its email. | _PK = Email_ |
| Email | A defined attribute. | _Login email (PK source)._ |
| First Name | A defined attribute. | _Given name._ |
| Last Name | A defined attribute. | _Family name._ |
| Role | A defined attribute. | _admin or viewer._ |
| Full Name | Computed as the last name, followed by a comma followed by a space, followed by the first name. | _LastName, FirstName._ |
| **Customer** | A CRM contact. Email is the PK. | — |
| Name | The same as its email. | _PK = Email_ |
| Email | A defined attribute. | _Customer email (PK source)._ |
| First Name | A defined attribute. | _Given name._ |
| Last Name | A defined attribute. | _Family name._ |
| Full Name | Computed as the last name, followed by a comma followed by a space, followed by the first name. | _LastName, FirstName._ |
| Lifetime Sales | The total recent total across the orders related to the customer. | _Sum of this customer's order totals from the last 60 days (orders older than 60 days do not count)._ |
| Unpaid Order Count | The total unpaid flag across the orders related to the customer. | _Number of this customer's orders that are not yet paid in full._ |
| Large Order Count | The number of the customer's large orders. | _Number of this customer's orders with a Total over $500. SUMIFS expression-mode workaround: agg_range yields 1 per row, then a real WHERE clause filters via the operator criterion ">500" (Program.cs ExtractCriteriaOp, supported on SUMIFS but not yet on COUNTIFS)._ |
| Is VIP | True when all of the following hold: the lifetime sales is greater than 1000 and the unpaid order count is at most 1. | _True when LifetimeSales > 1000 AND the customer has at most 1 unpaid order._ |
| **Order** | A customer order. OrderNumber is the PK. | — |
| Name | The same as its order number. | _PK = OrderNumber_ |
| Order Number | A defined attribute. | _Human-readable order number (PK source)._ |
| Order Date | A defined attribute. | _Date the order was placed._ |
| Customer | A defined attribute. | _FK -> Customers.Name (the customer's email)._ |
| Customer First Name | Taken from the linked customer. | _Customer's first name, looked up via Customer FK._ |
| Customer Last Name | Taken from the linked customer. | _Customer's last name, looked up via Customer FK._ |
| Customer Full Name | Taken from the linked customer. | _Customer's full name, looked up via Customer FK._ |
| Customer Email | Taken from the linked customer. | _Customer's email, looked up via Customer FK._ |
| Total | A defined attribute. | _Order total in dollars._ |
| Amount Paid | The total amount across the payments related to the order. | _Sum of all payments recorded against this order._ |
| Balance | Computed as the total minus the amount paid. | _Outstanding balance: Total - AmountPaid._ |
| Is Paid | True when the balance is at most 0. | _True when Balance <= 0._ |
| Unpaid Flag | Determined by priority: 0 if the paid flag is set; in all other cases, 1. | _1 when the order is unpaid, 0 otherwise — summed by Customers.UnpaidOrderCount._ |
| Is Recent | True when the number of days from the order date to today's date is at most 90. ⚠︎ mechanical <!-- rulespeak:reword --> | _True when OrderDate is within the last 60 days. Drives the 60-day window on Customers.LifetimeSales._ |
| Recent Total | Determined by priority: the total if the recent flag is set; in all other cases, 0. | _Total if the order is recent, 0 otherwise — summed by Customers.LifetimeSales._ |
| FCS Subtotal | The total line total across the order lines related to the order. | _Sum of LineTotal across this order's FCS line items (in $M)._ |
| FCS Unit Count | The total quantity across the order lines related to the order. | _Total FCS units shipped on this order._ |
| **Payment** | A payment recorded against an order. PaymentNumber is the PK. | — |
| Name | The same as its payment number. | _PK = PaymentNumber_ |
| Payment Number | A defined attribute. | _Human-readable payment number (PK source)._ |
| Order ID | A defined attribute. | _FK -> Orders.Name (the order number)._ |
| Payment Date | A defined attribute. | _Date the payment was received._ |
| Amount | A defined attribute. | _Payment amount in dollars._ |
| Method | A defined attribute. | _Payment method: card, check, cash, wire, ach._ |
| Order Number | Taken from the linked order ID. | _Parent order's human-readable number._ |
| Order Date | Taken from the linked order ID. | _Parent order's date._ |
| Order Total | Taken from the linked order ID. | _Parent order's total._ |
| Order Amount Paid | Taken from the linked order ID. | _Parent order's total paid (across all its payments)._ |
| Order Balance | Taken from the linked order ID. | _Parent order's outstanding balance._ |
| Order is Paid | True when the linked order ID is a paid. | _Parent order's paid-in-full flag._ |
| Order Customer | Taken from the linked order ID. | _Parent order's customer FK (email)._ |
| Customer Full Name | Taken from the linked order ID. | _Parent order's customer full name (two-hop)._ |
| Customer Email | Taken from the linked order ID. | _Parent order's customer email (two-hop)._ |
| **Jet Model** | Catalog of fighter-jet airframes that flight-control systems are built for. ModelCode is the PK. | — |
| Name | The same as its model code. | _PK = ModelCode_ |
| Model Code | A defined attribute. | _Short model designation, e.g. F-16C._ |
| Manufacturer | A defined attribute. | _Airframe OEM._ |
| Generation | A defined attribute. | _Fighter generation (4, 4.5, 5)._ |
| Is Fifth Gen | True when the generation is at least 5. | _True for 5th-generation airframes (stealth-era)._ |
| FCS Variant Count | The number of the jet model's FCS variants. | _Number of distinct FCS variants offered for this airframe._ |
| Total Units Ordered | The total total units ordered across the flight control systems related to the jet model. | _Total FCS units ordered across all variants for this airframe._ |
| Total Revenue | The total total revenue across the flight control systems related to the jet model. | _Total FCS revenue (in $M) across all variants for this airframe._ |
| **Flight Control System** | Flight-control-system product variants. Each variant targets one jet model with a specific control architecture and redundancy level. FCSCode is the PK. | — |
| Name | The same as its FCS code. | _PK = FCSCode_ |
| FCS Code | A defined attribute. | _Product code, e.g. FCS-F35-FBW4._ |
| Jet Model ID | A defined attribute. | _FK -> JetModels.Name._ |
| Jet Manufacturer | Taken from the linked jet model ID. | _Airframe manufacturer, via JetModelId._ |
| Jet Generation | Taken from the linked jet model ID. | _Airframe generation, via JetModelId._ |
| Architecture | A defined attribute. | _Control architecture: fly-by-wire, fly-by-light, analog-hybrid._ |
| Redundancy Channels | A defined attribute. | _Number of independent control channels (2, 3, or 4)._ |
| Unit Price | A defined attribute. | _Per-unit price in $M USD._ |
| Is Triple Redundant | True when the redundancy channels is at least 3. | _True when RedundancyChannels >= 3 (meets typical fighter airworthiness floor)._ |
| Is Quad Redundant | True when the redundancy channels is at least 4. | _True when RedundancyChannels >= 4 (5th-gen / carrier-grade)._ |
| Meets Fifth Gen Spec | True when all of the following hold: the quad redundant flag is set; at least one of the following holds: the architecture is “fly-by-wire” or the architecture is “fly-by-light”; and the jet generation is at least 5. | _Quad-redundant fly-by-wire or fly-by-light on a 5th-gen airframe._ |
| Total Units Ordered | The total quantity across the order lines related to the flight control system. | _Sum of order-line quantities across all orders._ |
| Total Revenue | The total line total across the order lines related to the flight control system. | _Sum of order-line revenue (in $M)._ |
| **Order Line** | FCS line items on an order — links Orders to FlightControlSystems with a quantity. LineNumber is the PK. | — |
| Name | The same as its line number. | _PK = LineNumber_ |
| Line Number | A defined attribute. | _Human-readable line code, e.g. OL-3001._ |
| Order ID | A defined attribute. | _FK -> Orders.Name._ |
| FCS ID | A defined attribute. | _FK -> FlightControlSystems.Name._ |
| Quantity | A defined attribute. | _Number of FCS units on this line._ |
| FCS Unit Price | Taken from the linked FCS ID. | _Per-unit price from the FCS catalog (in $M)._ |
| FCS Architecture | Taken from the linked FCS ID. | _FCS control architecture._ |
| FCS Jet Model | The jet model ID of the order line's FCS ID. | _Airframe this FCS targets._ |
| FCS Meets Fifth Gen Spec | True when the linked FCS ID is meets fifth gen spec. | _Whether the FCS meets 5th-gen spec (two-hop calc)._ |
| Order Number | Taken from the linked order ID. | _Parent order number._ |
| Order Customer | Taken from the linked order ID. | _Parent order customer FK._ |
| Line Total | Computed as the quantity times the FCS unit price. | _Quantity * FCSUnitPrice (in $M)._ |

## 3 Operative Rules

_Operative rules state what the business **obliges**, **prohibits**, or
advises (**should**). Structural rules come from required fields and foreign keys;
semantic rules come from the Constraints table, each keyed on a boolean the rulebook
already computes (cross-referenced as DR-N in the Definitional Rules below)._

### Structural Constraints (from the schema)

- A user **must** have an email and a role.
- A customer **must** have an email.
- An order **must** have an order number, an order date, a customer, and a total.
- A payment **must** have a payment number, an order ID, a payment date, an amount, and a method.
- A jet model **must** have a model code, a manufacturer, and a generation.
- A flight control system **must** have an FCS code, a jet model ID, an architecture, a redundancy channels, and a unit price.
- An order line **must** have a line number, an order ID, an FCS ID, and a quantity.

## 4 Definitional Rules

_All statements express truth in the business domain; they are neither
procedures nor imperatives. "iff" is avoided in favor of "only if" so a
one-directional necessity is not mistaken for an equivalence. A
**⚠︎ mechanical** chip marks a rule whose deterministic wording is faithful
but clunky — a flag for an optional downstream reword pass, not a defect._

| ID | Declarative rule |
|----|------------------|
| **DR-1 Name** | A user's name is the same as its email. |
| **DR-2 Full Name** | A user's full name is computed as the last name, followed by a comma followed by a space, followed by the first name. |
| **DR-3 Name** | A customer's name is the same as its email. |
| **DR-4 Full Name** | A customer's full name is computed as the last name, followed by a comma followed by a space, followed by the first name. |
| **DR-5 Lifetime Sales** | A customer's lifetime sales is the total recent total across the orders related to the customer. |
| **DR-6 Unpaid Order Count** | A customer's unpaid order count is the total unpaid flag across the orders related to the customer. |
| **DR-7 Large Order Count** | A customer's large order count is the number of the customer's large orders. |
| **DR-8 Is VIP** | A customer is considered a VIP if all of the following hold: the lifetime sales is greater than 1000 and the unpaid order count is at most 1. |
| **DR-9 Name** | An order's name is the same as its order number. |
| **DR-10 Customer First Name** | An order's customer first name — taken from the linked customer. |
| **DR-11 Customer Last Name** | An order's customer last name — taken from the linked customer. |
| **DR-12 Customer Full Name** | An order's customer full name — taken from the linked customer. |
| **DR-13 Customer Email** | An order's customer email — taken from the linked customer. |
| **DR-14 Amount Paid** | An order's amount paid is the total amount across the payments related to the order. |
| **DR-15 Balance** | An order's balance is computed as the total minus the amount paid. |
| **DR-16 Is Paid** | An order is considered a paid if the balance is at most 0. |
| **DR-17 Unpaid Flag** | The order's unpaid flag is determined by the following priority:<br>1. 0, if the paid flag is set;<br>2. in all other cases, 1. |
| **DR-18 Is Recent** | An order is considered a recent if the number of days from the order date to today's date is at most 90. ⚠︎ mechanical <!-- rulespeak:reword --> |
| **DR-19 Recent Total** | The order's recent total is determined by the following priority:<br>1. the total, if the recent flag is set;<br>2. in all other cases, 0. |
| **DR-20 FCS Subtotal** | An order's FCS subtotal is the total line total across the order lines related to the order. |
| **DR-21 FCS Unit Count** | An order's FCS unit count is the total quantity across the order lines related to the order. |
| **DR-22 Name** | A payment's name is the same as its payment number. |
| **DR-23 Order Number** | A payment's order number — taken from the linked order ID. |
| **DR-24 Order Date** | A payment's order date — taken from the linked order ID. |
| **DR-25 Order Total** | A payment's order total — taken from the linked order ID. |
| **DR-26 Order Amount Paid** | A payment's order amount paid — taken from the linked order ID. |
| **DR-27 Order Balance** | A payment's order balance — taken from the linked order ID. |
| **DR-28 Order is Paid** | A payment's order is paid when the linked order ID is a paid. |
| **DR-29 Order Customer** | A payment's order customer — taken from the linked order ID. |
| **DR-30 Customer Full Name** | A payment's customer full name — taken from the linked order ID. |
| **DR-31 Customer Email** | A payment's customer email — taken from the linked order ID. |
| **DR-32 Name** | A jet model's name is the same as its model code. |
| **DR-33 Is Fifth Gen** | A jet model is considered a fifth gen if the generation is at least 5. |
| **DR-34 FCS Variant Count** | A jet model's FCS variant count is the number of the jet model's FCS variants. |
| **DR-35 Total Units Ordered** | A jet model's total units ordered is the total total units ordered across the flight control systems related to the jet model. |
| **DR-36 Total Revenue** | A jet model's total revenue is the total total revenue across the flight control systems related to the jet model. |
| **DR-37 Name** | A flight control system's name is the same as its FCS code. |
| **DR-38 Jet Manufacturer** | A flight control system's jet manufacturer — taken from the linked jet model ID. |
| **DR-39 Jet Generation** | A flight control system's jet generation — taken from the linked jet model ID. |
| **DR-40 Is Triple Redundant** | A flight control system is considered a triple redundant if the redundancy channels is at least 3. |
| **DR-41 Is Quad Redundant** | A flight control system is considered a quad redundant if the redundancy channels is at least 4. |
| **DR-42 Meets Fifth Gen Spec** | A flight control system is considered to meet a fifth gen spec if all of the following hold: the quad redundant flag is set; at least one of the following holds: the architecture is “fly-by-wire” or the architecture is “fly-by-light”; and the jet generation is at least 5. |
| **DR-43 Total Units Ordered** | A flight control system's total units ordered is the total quantity across the order lines related to the flight control system. |
| **DR-44 Total Revenue** | A flight control system's total revenue is the total line total across the order lines related to the flight control system. |
| **DR-45 Name** | An order line's name is the same as its line number. |
| **DR-46 FCS Unit Price** | An order line's FCS unit price — taken from the linked FCS ID. |
| **DR-47 FCS Architecture** | An order line's FCS architecture — taken from the linked FCS ID. |
| **DR-48 FCS Jet Model** | An order line's FCS jet model is the jet model ID of the order line's FCS ID. |
| **DR-49 FCS Meets Fifth Gen Spec** | An order line's FCS meets fifth gen spec when the linked FCS ID is meets fifth gen spec. |
| **DR-50 Order Number** | An order line's order number — taken from the linked order ID. |
| **DR-51 Order Customer** | An order line's order customer — taken from the linked order ID. |
| **DR-52 Line Total** | An order line's line total is computed as the quantity times the FCS unit price. |

## 5 Traceability to Schema

_The expression column is the rule's definition in RuleSpeak® notation —
the same logic the rulebook stores, written for a business reader._

| Schema element | Kind | Expression |
|----------------|------|------------|
| **Users.Name** | formula | `Email` |
| **Users.FullName** | formula | `Concat(LastName, ", ", FirstName)` |
| **Customers.Name** | formula | `Email` |
| **Customers.FullName** | formula | `Concat(LastName, ", ", FirstName)` |
| **Customers.LifetimeSales** | rollup | `Sum(Orders.RecentTotal via Customer)` |
| **Customers.UnpaidOrderCount** | rollup | `Sum(Orders.UnpaidFlag via Customer)` |
| **Customers.LargeOrderCount** | rollup | `Sum(Orders.Total * 0 + 1, Orders.Customer, Name, Orders.Total, ">500")` |
| **Customers.IsVIP** | formula | `And(LifetimeSales > 1000, UnpaidOrderCount <= 1)` |
| **Orders.Name** | formula | `OrderNumber` |
| **Orders.CustomerFirstName** | lookup | `Lookup(Customers.FirstName via Customer)` |
| **Orders.CustomerLastName** | lookup | `Lookup(Customers.LastName via Customer)` |
| **Orders.CustomerFullName** | lookup | `Lookup(Customers.FullName via Customer)` |
| **Orders.CustomerEmail** | lookup | `Lookup(Customers.Email via Customer)` |
| **Orders.AmountPaid** | rollup | `Sum(Payments.Amount via OrderId)` |
| **Orders.Balance** | formula | `Total - AmountPaid` |
| **Orders.IsPaid** | formula | `Balance <= 0` |
| **Orders.UnpaidFlag** | formula | `If(IsPaid, 0, 1)` |
| **Orders.IsRecent** | formula | `DaysBetween(Today(), OrderDate) <= 90` |
| **Orders.RecentTotal** | formula | `If(IsRecent, Total, 0)` |
| **Orders.FCSSubtotal** | rollup | `Sum(OrderLines.LineTotal via OrderId)` |
| **Orders.FCSUnitCount** | rollup | `Sum(OrderLines.Quantity via OrderId)` |
| **Payments.Name** | formula | `PaymentNumber` |
| **Payments.OrderNumber** | lookup | `Lookup(Orders.OrderNumber via OrderId)` |
| **Payments.OrderDate** | lookup | `Lookup(Orders.OrderDate via OrderId)` |
| **Payments.OrderTotal** | lookup | `Lookup(Orders.Total via OrderId)` |
| **Payments.OrderAmountPaid** | lookup | `Lookup(Orders.AmountPaid via OrderId)` |
| **Payments.OrderBalance** | lookup | `Lookup(Orders.Balance via OrderId)` |
| **Payments.OrderIsPaid** | lookup | `Lookup(Orders.IsPaid via OrderId)` |
| **Payments.OrderCustomer** | lookup | `Lookup(Orders.Customer via OrderId)` |
| **Payments.CustomerFullName** | lookup | `Lookup(Orders.CustomerFullName via OrderId)` |
| **Payments.CustomerEmail** | lookup | `Lookup(Orders.CustomerEmail via OrderId)` |
| **JetModels.Name** | formula | `ModelCode` |
| **JetModels.IsFifthGen** | formula | `Generation >= 5` |
| **JetModels.FCSVariantCount** | rollup | `Sum(FlightControlSystems.UnitPrice * 0 + 1, FlightControlSystems.JetModelId, Name)` |
| **JetModels.TotalUnitsOrdered** | rollup | `Sum(FlightControlSystems.TotalUnitsOrdered via JetModelId)` |
| **JetModels.TotalRevenue** | rollup | `Sum(FlightControlSystems.TotalRevenue via JetModelId)` |
| **FlightControlSystems.Name** | formula | `FCSCode` |
| **FlightControlSystems.JetManufacturer** | lookup | `Lookup(JetModels.Manufacturer via JetModelId)` |
| **FlightControlSystems.JetGeneration** | lookup | `Lookup(JetModels.Generation via JetModelId)` |
| **FlightControlSystems.IsTripleRedundant** | formula | `RedundancyChannels >= 3` |
| **FlightControlSystems.IsQuadRedundant** | formula | `RedundancyChannels >= 4` |
| **FlightControlSystems.MeetsFifthGenSpec** | formula | `And(IsQuadRedundant, Or(Architecture = "fly-by-wire", Architecture = "fly-by-light"), JetGeneration >= 5)` |
| **FlightControlSystems.TotalUnitsOrdered** | rollup | `Sum(OrderLines.Quantity via FCSId)` |
| **FlightControlSystems.TotalRevenue** | rollup | `Sum(OrderLines.LineTotal via FCSId)` |
| **OrderLines.Name** | formula | `LineNumber` |
| **OrderLines.FCSUnitPrice** | lookup | `Lookup(FlightControlSystems.UnitPrice via FCSId)` |
| **OrderLines.FCSArchitecture** | lookup | `Lookup(FlightControlSystems.Architecture via FCSId)` |
| **OrderLines.FCSJetModel** | lookup | `Lookup(FlightControlSystems.JetModelId via FCSId)` |
| **OrderLines.FCSMeetsFifthGenSpec** | lookup | `Lookup(FlightControlSystems.MeetsFifthGenSpec via FCSId)` |
| **OrderLines.OrderNumber** | lookup | `Lookup(Orders.OrderNumber via OrderId)` |
| **OrderLines.OrderCustomer** | lookup | `Lookup(Orders.Customer via OrderId)` |
| **OrderLines.LineTotal** | formula | `Quantity * FCSUnitPrice` |

---

_This document is rendered in **RuleSpeak®**, the declarative business-rule
notation created by **Ronald G. Ross**, and follows the conventions of
**SBVR** (Semantics of Business Vocabulary and Business Rules). With thanks to
Ronald G. Ross for RuleSpeak® and his foundational work on business rules —
[www.RonRoss.info](https://www.RonRoss.info)._
