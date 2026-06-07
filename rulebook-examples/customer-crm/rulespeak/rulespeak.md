# 📘 Customer CRM — RuleSpeak

_Fighter-jet FCS sales pipeline rolling revenue up by order, FCS variant, and jet model._

> Declarative business rules rendered from the rulebook. Every statement
> below expresses truth in the business domain — it is neither a procedure
> nor an imperative. The rulebook's formulas are the single source of truth;
> this document is their plain-language reading.

## 1 Business Vocabulary

| Term | Meaning (business sense) |
|------|--------------------------|
| **User** | Dev-login identities for the demo app. Email is the PK. |
| Full Name | LastName, FirstName. |
| **Customer** | A CRM contact. Email is the PK. |
| Full Name | LastName, FirstName. |
| Lifetime Sales | Sum of this customer's order totals from the last 60 days (orders older than 60 days do not count). |
| Unpaid Order Count | Number of this customer's orders that are not yet paid in full. |
| Large Order Count | Number of this customer's orders with a Total over $500. SUMIFS expression-mode workaround: agg_range yields 1 per row, then a real WHERE clause filters via the operator criterion ">500" (Program.cs ExtractCriteriaOp, supported on SUMIFS but not yet on COUNTIFS). |
| Is VIP | True when LifetimeSales > 1000 AND the customer has at most 1 unpaid order. |
| **Order** | A customer order. OrderNumber is the PK. |
| Customer First Name | Customer's first name, looked up via Customer FK. |
| Customer Last Name | Customer's last name, looked up via Customer FK. |
| Customer Full Name | Customer's full name, looked up via Customer FK. |
| Customer Email | Customer's email, looked up via Customer FK. |
| Amount Paid | Sum of all payments recorded against this order. |
| Balance | Outstanding balance: Total - AmountPaid. |
| Is Paid | True when Balance <= 0. |
| Unpaid Flag | 1 when the order is unpaid, 0 otherwise — summed by Customers.UnpaidOrderCount. |
| Is Recent | True when OrderDate is within the last 60 days. Drives the 60-day window on Customers.LifetimeSales. |
| Recent Total | Total if the order is recent, 0 otherwise — summed by Customers.LifetimeSales. |
| FCS Subtotal | Sum of LineTotal across this order's FCS line items (in $M). |
| FCS Unit Count | Total FCS units shipped on this order. |
| **Payment** | A payment recorded against an order. PaymentNumber is the PK. |
| Order Number | Parent order's human-readable number. |
| Order Date | Parent order's date. |
| Order Total | Parent order's total. |
| Order Amount Paid | Parent order's total paid (across all its payments). |
| Order Balance | Parent order's outstanding balance. |
| Order is Paid | Parent order's paid-in-full flag. |
| Order Customer | Parent order's customer FK (email). |
| Customer Full Name | Parent order's customer full name (two-hop). |
| Customer Email | Parent order's customer email (two-hop). |
| **Jet Model** | Catalog of fighter-jet airframes that flight-control systems are built for. ModelCode is the PK. |
| Is Fifth Gen | True for 5th-generation airframes (stealth-era). |
| FCS Variant Count | Number of distinct FCS variants offered for this airframe. |
| Total Units Ordered | Total FCS units ordered across all variants for this airframe. |
| Total Revenue | Total FCS revenue (in $M) across all variants for this airframe. |
| **Flight Control System** | Flight-control-system product variants. Each variant targets one jet model with a specific control architecture and redundancy level. FCSCode is the PK. |
| Jet Manufacturer | Airframe manufacturer, via JetModelId. |
| Jet Generation | Airframe generation, via JetModelId. |
| Is Triple Redundant | True when RedundancyChannels >= 3 (meets typical fighter airworthiness floor). |
| Is Quad Redundant | True when RedundancyChannels >= 4 (5th-gen / carrier-grade). |
| Meets Fifth Gen Spec | Quad-redundant fly-by-wire or fly-by-light on a 5th-gen airframe. |
| Total Units Ordered | Sum of order-line quantities across all orders. |
| Total Revenue | Sum of order-line revenue (in $M). |
| **Order Line** | FCS line items on an order — links Orders to FlightControlSystems with a quantity. LineNumber is the PK. |
| FCS Unit Price | Per-unit price from the FCS catalog (in $M). |
| FCS Architecture | FCS control architecture. |
| FCS Jet Model | Airframe this FCS targets. |
| FCS Meets Fifth Gen Spec | Whether the FCS meets 5th-gen spec (two-hop calc). |
| Order Number | Parent order number. |
| Order Customer | Parent order customer FK. |
| Line Total | Quantity * FCSUnitPrice (in $M). |

## 3 Definitional Rules

_All statements express truth in the business domain; they are neither
procedures nor imperatives. "iff" is avoided in favor of "only if" so a
one-directional necessity is not mistaken for an equivalence._

| ID | Declarative rule |
|----|------------------|
| **DR-1 Full Name** | A user's full name is computed as the last name, followed by a comma followed by a space, followed by the first name. |
| **DR-2 Full Name** | A customer's full name is computed as the last name, followed by a comma followed by a space, followed by the first name. |
| **DR-3 Lifetime Sales** | A customer's lifetime sales is the total recent total across the orders related to the customer. |
| **DR-4 Unpaid Order Count** | A customer's unpaid order count is the total unpaid flag across the orders related to the customer. |
| **DR-5 Large Order Count** | A customer's large order count is rolled up from its related records (`Sum(Orders.Total*0+1, Orders.Customer, Name, Orders.Total, ">500")`). |
| **DR-6 Is VIP** | A customer is considered a VIP if all of the following hold: the lifetime sales is greater than 1000 and the unpaid order count is at most 1. |
| **DR-7 Customer First Name** | An order's customer first name is the first name of the order's customer. |
| **DR-8 Customer Last Name** | An order's customer last name is the last name of the order's customer. |
| **DR-9 Customer Full Name** | An order's customer full name is the full name of the order's customer. |
| **DR-10 Customer Email** | An order's customer email is the email of the order's customer. |
| **DR-11 Amount Paid** | An order's amount paid is the total amount across the payments related to the order. |
| **DR-12 Balance** | An order's balance is computed as `Total - AmountPaid`. |
| **DR-13 Is Paid** | An order is considered a paid if the balance is at most 0. |
| **DR-14 Unpaid Flag** | The order's unpaid flag is determined by the following priority:<br>1. 0, if the is paid flag is set;<br>2. otherwise 1. |
| **DR-15 Is Recent** | An order is considered a recent if `Datediff(Today(), OrderDate, "days")` is at most 90. |
| **DR-16 Recent Total** | The order's recent total is determined by the following priority:<br>1. the total, if the is recent flag is set;<br>2. otherwise 0. |
| **DR-17 FCS Subtotal** | An order's FCS subtotal is the total line total across the order lines related to the order. |
| **DR-18 FCS Unit Count** | An order's FCS unit count is the total quantity across the order lines related to the order. |
| **DR-19 Order Number** | A payment's order number is the order number of the payment's order ID. |
| **DR-20 Order Date** | A payment's order date is the order date of the payment's order ID. |
| **DR-21 Order Total** | A payment's order total is the total of the payment's order ID. |
| **DR-22 Order Amount Paid** | A payment's order amount paid is the amount paid of the payment's order ID. |
| **DR-23 Order Balance** | A payment's order balance is the balance of the payment's order ID. |
| **DR-24 Order is Paid** | A payment's order is paid is true when the payment's order ID is a paid. |
| **DR-25 Order Customer** | A payment's order customer is the customer of the payment's order ID. |
| **DR-26 Customer Full Name** | A payment's customer full name is the customer full name of the payment's order ID. |
| **DR-27 Customer Email** | A payment's customer email is the customer email of the payment's order ID. |
| **DR-28 Is Fifth Gen** | A jet model is considered a fifth gen if the generation is at least 5. |
| **DR-29 FCS Variant Count** | A jet model's FCS variant count is rolled up from its related records (`Sum(FlightControlSystems.UnitPrice*0+1, FlightControlSystems.JetModelId, Name)`). |
| **DR-30 Total Units Ordered** | A jet model's total units ordered is the total total units ordered across the flight control systems related to the jet model. |
| **DR-31 Total Revenue** | A jet model's total revenue is the total total revenue across the flight control systems related to the jet model. |
| **DR-32 Jet Manufacturer** | A flight control system's jet manufacturer is the manufacturer of the flight control system's jet model ID. |
| **DR-33 Jet Generation** | A flight control system's jet generation is the generation of the flight control system's jet model ID. |
| **DR-34 Is Triple Redundant** | A flight control system is considered a triple redundant if the redundancy channels is at least 3. |
| **DR-35 Is Quad Redundant** | A flight control system is considered a quad redundant if the redundancy channels is at least 4. |
| **DR-36 Meets Fifth Gen Spec** | A flight control system is considered to meet fifth gen spec if all of the following hold: the is quad redundant flag is set; at least one of the following holds: the architecture is the literal “fly-by-wire” or the architecture is the literal “fly-by-light”; and the jet generation is at least 5. |
| **DR-37 Total Units Ordered** | A flight control system's total units ordered is the total quantity across the order lines related to the flight control system. |
| **DR-38 Total Revenue** | A flight control system's total revenue is the total line total across the order lines related to the flight control system. |
| **DR-39 FCS Unit Price** | An order line's FCS unit price is the unit price of the order line's FCS ID. |
| **DR-40 FCS Architecture** | An order line's FCS architecture is the architecture of the order line's FCS ID. |
| **DR-41 FCS Jet Model** | An order line's FCS jet model is the jet model ID of the order line's FCS ID. |
| **DR-42 FCS Meets Fifth Gen Spec** | An order line's FCS meets fifth gen spec is true when the order line's FCS ID is meets fifth gen spec. |
| **DR-43 Order Number** | An order line's order number is the order number of the order line's order ID. |
| **DR-44 Order Customer** | An order line's order customer is the customer of the order line's order ID. |
| **DR-45 Line Total** | An order line's line total is computed as `Quantity * FCSUnitPrice`. |

## 4 Traceability to Schema

_The expression column is the rule's definition in RuleSpeak notation —
the same logic the rulebook stores, written for a business reader._

| Schema element | Kind | Expression |
|----------------|------|------------|
| **Users.FullName** | formula | `Concat(LastName, ", ", FirstName)` |
| **Customers.FullName** | formula | `Concat(LastName, ", ", FirstName)` |
| **Customers.LifetimeSales** | rollup | `Sum(Orders.RecentTotal via Customer)` |
| **Customers.UnpaidOrderCount** | rollup | `Sum(Orders.UnpaidFlag via Customer)` |
| **Customers.LargeOrderCount** | rollup | `Sum(Orders.Total*0+1, Orders.Customer, Name, Orders.Total, ">500")` |
| **Customers.IsVIP** | formula | `And(LifetimeSales > 1000, UnpaidOrderCount <= 1)` |
| **Orders.CustomerFirstName** | lookup | `Lookup(Customers.FirstName via Customer)` |
| **Orders.CustomerLastName** | lookup | `Lookup(Customers.LastName via Customer)` |
| **Orders.CustomerFullName** | lookup | `Lookup(Customers.FullName via Customer)` |
| **Orders.CustomerEmail** | lookup | `Lookup(Customers.Email via Customer)` |
| **Orders.AmountPaid** | rollup | `Sum(Payments.Amount via OrderId)` |
| **Orders.Balance** | formula | `Total - AmountPaid` |
| **Orders.IsPaid** | formula | `Balance <= 0` |
| **Orders.UnpaidFlag** | formula | `If(IsPaid, 0, 1)` |
| **Orders.IsRecent** | formula | `Datediff(Today(), OrderDate, "days") <= 90` |
| **Orders.RecentTotal** | formula | `If(IsRecent, Total, 0)` |
| **Orders.FCSSubtotal** | rollup | `Sum(OrderLines.LineTotal via OrderId)` |
| **Orders.FCSUnitCount** | rollup | `Sum(OrderLines.Quantity via OrderId)` |
| **Payments.OrderNumber** | lookup | `Lookup(Orders.OrderNumber via OrderId)` |
| **Payments.OrderDate** | lookup | `Lookup(Orders.OrderDate via OrderId)` |
| **Payments.OrderTotal** | lookup | `Lookup(Orders.Total via OrderId)` |
| **Payments.OrderAmountPaid** | lookup | `Lookup(Orders.AmountPaid via OrderId)` |
| **Payments.OrderBalance** | lookup | `Lookup(Orders.Balance via OrderId)` |
| **Payments.OrderIsPaid** | lookup | `Lookup(Orders.IsPaid via OrderId)` |
| **Payments.OrderCustomer** | lookup | `Lookup(Orders.Customer via OrderId)` |
| **Payments.CustomerFullName** | lookup | `Lookup(Orders.CustomerFullName via OrderId)` |
| **Payments.CustomerEmail** | lookup | `Lookup(Orders.CustomerEmail via OrderId)` |
| **JetModels.IsFifthGen** | formula | `Generation >= 5` |
| **JetModels.FCSVariantCount** | rollup | `Sum(FlightControlSystems.UnitPrice*0+1, FlightControlSystems.JetModelId, Name)` |
| **JetModels.TotalUnitsOrdered** | rollup | `Sum(FlightControlSystems.TotalUnitsOrdered via JetModelId)` |
| **JetModels.TotalRevenue** | rollup | `Sum(FlightControlSystems.TotalRevenue via JetModelId)` |
| **FlightControlSystems.JetManufacturer** | lookup | `Lookup(JetModels.Manufacturer via JetModelId)` |
| **FlightControlSystems.JetGeneration** | lookup | `Lookup(JetModels.Generation via JetModelId)` |
| **FlightControlSystems.IsTripleRedundant** | formula | `RedundancyChannels >= 3` |
| **FlightControlSystems.IsQuadRedundant** | formula | `RedundancyChannels >= 4` |
| **FlightControlSystems.MeetsFifthGenSpec** | formula | `And(IsQuadRedundant, Or(Architecture="fly-by-wire", Architecture="fly-by-light"), JetGeneration >= 5)` |
| **FlightControlSystems.TotalUnitsOrdered** | rollup | `Sum(OrderLines.Quantity via FCSId)` |
| **FlightControlSystems.TotalRevenue** | rollup | `Sum(OrderLines.LineTotal via FCSId)` |
| **OrderLines.FCSUnitPrice** | lookup | `Lookup(FlightControlSystems.UnitPrice via FCSId)` |
| **OrderLines.FCSArchitecture** | lookup | `Lookup(FlightControlSystems.Architecture via FCSId)` |
| **OrderLines.FCSJetModel** | lookup | `Lookup(FlightControlSystems.JetModelId via FCSId)` |
| **OrderLines.FCSMeetsFifthGenSpec** | lookup | `Lookup(FlightControlSystems.MeetsFifthGenSpec via FCSId)` |
| **OrderLines.OrderNumber** | lookup | `Lookup(Orders.OrderNumber via OrderId)` |
| **OrderLines.OrderCustomer** | lookup | `Lookup(Orders.Customer via OrderId)` |
| **OrderLines.LineTotal** | formula | `Quantity * FCSUnitPrice` |
