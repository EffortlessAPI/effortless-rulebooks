# 📘 v4: NakedClaude Demo — RuleSpeak

_Rulebook generated from Airtable base 'v4: NakedClaude Demo'._

> Declarative business rules rendered from the rulebook. Every statement
> below expresses truth in the business domain — it is neither a procedure
> nor an imperative. The rulebook's formulas are the single source of truth;
> this document is their plain-language reading.

## 1 Business Vocabulary

| Term | Meaning (business sense) |
|------|--------------------------|
| **App User** | An app user tracked by the business. |
| **Client** | A client tracked by the business. |
| Category Name | The name of the client's category. |
| Category Discount | The discount of the client's category. |
| Is Stopped | True when the customer's current Status is a blocking status. Lookup of Statuses.IsBlocking. |
| Status Display Name | Human-readable display name of the customer's current Status. Lookup of Statuses.DisplayName. |
| Status Description | Description of the customer's current Status. Lookup of Statuses.Description. |
| Count of Big Invoices | The number of invoices related to the client. |
| Billing Address State Tax Rate | The state tax rate of the client's addresses. |
| Billing Address | Default billing address for the customer. |
| Shipping Address | Default shipping address for the customer. |
| Average Order Value | The average invoice total across the client's invoices. |
| Is VIP | True when `If(And(AverageOrderValue` is greater than `500, HasRecentInvoices), True(), False())`. |
| Has Recent Invoices | True when `If(LastInvoice` is greater than `Dateadd(Today(), -220, 'days'), True(), False())`. |
| Last Invoice | The total order date across the client's invoices. |
| Customer Since Days | Computed: `If(CreatedAt, Datetime_diff(Now(), CreatedAt, 'days'), Blank())`. |
| **Client Approval** | A client approval tracked by the business. |
| Approved by Contact Name | The contact name of the client approval's approved by. |
| Approved by Email Address | The email address of the client approval's approved by. |
| Approved by Phone Number | The phone number of the client approval's approved by. |
| Approved by Role | The role of the client approval's approved by. |
| Is Approved | True when it is not the case that the approved by is `Blank()`. |
| Client Name | The name of the client approval's client. |
| Client Email | The email of the client approval's client. |
| Client Phone | The phone of the client approval's client. |
| Client Category | The category of the client approval's client. |
| **Client Category** | A client category tracked by the business. |
| Count of Clients | The number of clients related to the client category. |
| **Status** | A status tracked by the business. |
| Count of Clients | The number of clients related to the status. |
| **Product** | A product tracked by the business. |
| Profit | Computed as `UnitPrice - Cost`. |
| Margin | Computed as `1 - (Cost / UnitPrice)`. |
| Is High Margin | True when the margin is greater than 0.65. |
| Stock Quantity | Number of units currently in stock. |
| COGS | Computed as `StockQuantity * Cost`. |
| Count of VIP Orders | The number of invoice line items related to the product. |
| Has Been Ordered by VIP Customers | True when `If(And(CountOfVIPOrders` is greater than `0, StockQuantity > 250), True(), False())`. |
| **Invoice** | An invoice tracked by the business. |
| Client is VIP | True when the invoice's client is a VIP. |
| Client Hast Recent Invoices | True when the invoice's client has a recent invoices. |
| Client Email | The email of the invoice's client. |
| Client Phone | The phone of the invoice's client. |
| Client Company Name | The company name of the invoice's client. |
| Client Category Name | The category name of the invoice's client. |
| Client Category Discount | The category discount of the invoice's client. |
| Item Count | The number of invoice line items related to the invoice. |
| Total Quantity | The total quantity across the invoice's invoice line items. |
| Sub Total | The total sub total across the invoice's invoice line items. |
| Is Big Order | True when the sub total is greater than 350. |
| Tax Rate | Sales tax rate applied to this order. |
| Tax Amount | Computed as `SubTotal * TaxRate`. |
| Invoice Total | Computed as `SubTotal + TaxAmount`. |
| Total Paid | The total completed amount across the invoice's payments. |
| Amount Due | Computed as `Round(InvoiceTotal - TotalPaid, 2)`. |
| Is Paid in Full | True when `If(AmountDue` is at most `0, True(), False())`. |
| Payment Count | The number of payments related to the invoice. |
| Last Payment Date | The total payment date across the invoice's payments. |
| Payment Status Label | Computed: `If(IsPaidInFull, "Paid", If(TotalPaid = 0, "Unpaid", "Partial"))`. |
| **Invoice Line Item** | An invoice line item tracked by the business. |
| Client is VIP | True when the invoice line item's invoice is a VIP. |
| Client Hast Recent Invoices | True when the invoice line item's invoice is client hast recent invoices. |
| invoice Number | The invoice number of the invoice line item's invoice. |
| Invoice Total | The invoice total of the invoice line item's invoice. |
| Invoice Total Paid | The total paid of the invoice line item's invoice. |
| Product SKU | The SKU of the invoice line item's product. |
| Product Display Name | The display name of the invoice line item's product. |
| Unit Price | Price per unit captured at the time of ordering (may differ from current Product.UnitPrice). |
| Pre Discount | Computed as `Quantity * UnitPrice`. |
| Discount Percent | Per-line discount applied to this item. |
| Discount Amount | Computed as `PreDiscount * DiscountPercent`. |
| Sub Total | Computed as `PreDiscount - DiscountAmount`. |
| **Inventory Adjustment** | An inventory adjustment tracked by the business. |
| Adjustment Name | Computed as the product, followed by a hyphen, followed by the first 10 character(s) of the date, followed by the literal “-qty”, followed by the quantity. |
| **Payment** | A payment tracked by the business. |
| Invoice Date | The order date of the payment's invoice. |
| Invoice Number | The invoice number of the payment's invoice. |
| Invoice Status | The order status of the payment's invoice. |
| Invoice Total | The invoice total of the payment's invoice. |
| Is Completed | True when `If(PaymentStatus` is `"Completed", True(), False())`. |
| Completed Amount | Computed: `If(PaymentStatus = "Completed", Amount, 0)`. |
| Order Amount Due | The amount due of the payment's invoice. |
| Order is Paid in Full | True when the payment's invoice is a paid in full. |
| **Address** | An address tracked by the business. |
| Type of Address is Shipping Address | True when the address's type of address is a shipping address. |
| Type of Address is Billing Address | True when the address's type of address is a billing address. |
| State Code | The code of the address's state. |
| State Tax Rate | The tax rate of the address's state. |
| **State** | A state tracked by the business. |
| **Type of Addresses** | A type of addresses tracked by the business. |
| **ERB Version** | An ERB version tracked by the business. |
| **ERB Customization** | An ERB customization tracked by the business. |

## 2 Fact Types

- a **client** may reference one **client category**
- a **client** may reference one **status**
- a **client approval** may reference one **client**
- a **client approval** may reference one **app user**
- an **invoice** may reference one **client**
- an **invoice line item** may reference one **invoice**
- an **invoice line item** may reference one **product**
- an **inventory adjustment** may reference one **product**
- a **payment** may reference one **invoice**
- an **address** may reference one **type of addresses**
- an **address** may reference one **state**

## 3 Definitional Rules

_All statements express truth in the business domain; they are neither
procedures nor imperatives. "iff" is avoided in favor of "only if" so a
one-directional necessity is not mistaken for an equivalence._

| ID | Declarative rule |
|----|------------------|
| **DR-1 Category Name** | A client's category name is the name of the client's category. |
| **DR-2 Category Discount** | A client's category discount is the discount of the client's category. |
| **DR-3 Is Stopped** | A client's is stopped is true when the client's status is blocking. |
| **DR-4 Status Display Name** | A client's status display name is the display name of the client's status. |
| **DR-5 Status Description** | A client's status description is the description of the client's status. |
| **DR-6 Count of Big Invoices** | A client's count of big invoices is the number of invoices related to the client. |
| **DR-7 Billing Address State Tax Rate** | A client's billing address state tax rate is the state tax rate of the client's addresses. |
| **DR-8 Billing Address** | A client's billing address is the name of the client's addresses. |
| **DR-9 Shipping Address** | A client's shipping address is the name of the client's addresses. |
| **DR-10 Average Order Value** | A client's average order value is the average invoice total across the invoices related to the client. |
| **DR-11 Is VIP** | A client is considered a VIP if `If(And(AverageOrderValue` is greater than `500, HasRecentInvoices), True(), False())`. |
| **DR-12 Has Recent Invoices** | A client is considered to have a recent invoices if `If(LastInvoice` is greater than `Dateadd(Today(), -220, 'days'), True(), False())`. |
| **DR-13 Last Invoice** | A client's last invoice is the total order date across the invoices related to the client. |
| **DR-14 Customer Since Days** | The client's customer since days is determined by the following priority:<br>1. `Datetime_diff(Now(), CreatedAt, 'days')`, if the created at flag is set;<br>2. otherwise `Blank()`. |
| **DR-15 Approved by Contact Name** | A client approval's approved by contact name is the contact name of the client approval's approved by. |
| **DR-16 Approved by Email Address** | A client approval's approved by email address is the email address of the client approval's approved by. |
| **DR-17 Approved by Phone Number** | A client approval's approved by phone number is the phone number of the client approval's approved by. |
| **DR-18 Approved by Role** | A client approval's approved by role is the role of the client approval's approved by. |
| **DR-19 Is Approved** | A client approval is considered approved if it is not the case that the approved by is `Blank()`. |
| **DR-20 Client Name** | A client approval's client name is the name of the client approval's client. |
| **DR-21 Client Email** | A client approval's client email is the email of the client approval's client. |
| **DR-22 Client Phone** | A client approval's client phone is the phone of the client approval's client. |
| **DR-23 Client Category** | A client approval's client category is the category of the client approval's client. |
| **DR-24 Count of Clients** | A client category's count of clients is the number of clients related to the client category. |
| **DR-25 Count of Clients** | A status's count of clients is the number of clients related to the status. |
| **DR-26 Profit** | A product's profit is computed as `UnitPrice - Cost`. |
| **DR-27 Margin** | A product's margin is computed as `1 - (Cost / UnitPrice)`. |
| **DR-28 Is High Margin** | A product is considered a high margin if the margin is greater than 0.65. |
| **DR-29 Stock Quantity** | A product's stock quantity is the total quantity across the inventory adjustments related to the product. |
| **DR-30 COGS** | A product's COGS is computed as `StockQuantity * Cost`. |
| **DR-31 Count of VIP Orders** | A product's count of VIP orders is the number of invoice line items related to the product. |
| **DR-32 Has Been Ordered by VIP Customers** | A product is considered to have a been ordered by VIP customers if `If(And(CountOfVIPOrders` is greater than `0, StockQuantity > 250), True(), False())`. |
| **DR-33 Client is VIP** | An invoice's client is VIP is true when the invoice's client is a VIP. |
| **DR-34 Client Hast Recent Invoices** | An invoice's client hast recent invoices is true when the invoice's client has a recent invoices. |
| **DR-35 Client Email** | An invoice's client email is the email of the invoice's client. |
| **DR-36 Client Phone** | An invoice's client phone is the phone of the invoice's client. |
| **DR-37 Client Company Name** | An invoice's client company name is the company name of the invoice's client. |
| **DR-38 Client Category Name** | An invoice's client category name is the category name of the invoice's client. |
| **DR-39 Client Category Discount** | An invoice's client category discount is the category discount of the invoice's client. |
| **DR-40 Item Count** | An invoice's item count is the number of invoice line items related to the invoice. |
| **DR-41 Total Quantity** | An invoice's total quantity is the total quantity across the invoice line items related to the invoice. |
| **DR-42 Sub Total** | An invoice's sub total is the total sub total across the invoice line items related to the invoice. |
| **DR-43 Is Big Order** | An invoice is considered a big order if the sub total is greater than 350. |
| **DR-44 Tax Rate** | An invoice's tax rate is the billing address state tax rate of the invoice's client. |
| **DR-45 Tax Amount** | An invoice's tax amount is computed as `SubTotal * TaxRate`. |
| **DR-46 Invoice Total** | An invoice's invoice total is computed as `SubTotal + TaxAmount`. |
| **DR-47 Total Paid** | An invoice's total paid is the total completed amount across the payments related to the invoice. |
| **DR-48 Amount Due** | An invoice's amount due is computed as `Round(InvoiceTotal - TotalPaid, 2)`. |
| **DR-49 Is Paid in Full** | An invoice is considered a paid in full if `If(AmountDue` is at most `0, True(), False())`. |
| **DR-50 Payment Count** | An invoice's payment count is the number of payments related to the invoice. |
| **DR-51 Last Payment Date** | An invoice's last payment date is the total payment date across the payments related to the invoice. |
| **DR-52 Payment Status Label** | The invoice's payment status label is determined by the following priority:<br>1. the literal “Paid”, if the is paid in full flag is set;<br>2. the literal “Unpaid”, if the total paid is 0;<br>3. otherwise the literal “Partial”. |
| **DR-53 Client is VIP** | An invoice line item's client is VIP is true when the invoice line item's invoice is a VIP. |
| **DR-54 Client Hast Recent Invoices** | An invoice line item's client hast recent invoices is true when the invoice line item's invoice is client hast recent invoices. |
| **DR-55 invoice Number** | An invoice line item's invoice number is the invoice number of the invoice line item's invoice. |
| **DR-56 Invoice Total** | An invoice line item's invoice total is the invoice total of the invoice line item's invoice. |
| **DR-57 Invoice Total Paid** | An invoice line item's invoice total paid is the total paid of the invoice line item's invoice. |
| **DR-58 Product SKU** | An invoice line item's product SKU is the SKU of the invoice line item's product. |
| **DR-59 Product Display Name** | An invoice line item's product display name is the display name of the invoice line item's product. |
| **DR-60 Unit Price** | An invoice line item's unit price is the unit price of the invoice line item's product. |
| **DR-61 Pre Discount** | An invoice line item's pre discount is computed as `Quantity * UnitPrice`. |
| **DR-62 Discount Percent** | An invoice line item's discount percent is the client category discount of the invoice line item's invoice. |
| **DR-63 Discount Amount** | An invoice line item's discount amount is computed as `PreDiscount * DiscountPercent`. |
| **DR-64 Sub Total** | An invoice line item's sub total is computed as `PreDiscount - DiscountAmount`. |
| **DR-65 Adjustment Name** | An inventory adjustment's adjustment name is computed as the product, followed by a hyphen, followed by the first 10 character(s) of the date, followed by the literal “-qty”, followed by the quantity. |
| **DR-66 Invoice Date** | A payment's invoice date is the order date of the payment's invoice. |
| **DR-67 Invoice Number** | A payment's invoice number is the invoice number of the payment's invoice. |
| **DR-68 Invoice Status** | A payment's invoice status is the order status of the payment's invoice. |
| **DR-69 Invoice Total** | A payment's invoice total is the invoice total of the payment's invoice. |
| **DR-70 Is Completed** | A payment is considered completed if `If(PaymentStatus` is `"Completed", True(), False())`. |
| **DR-71 Completed Amount** | The payment's completed amount is determined by the following priority:<br>1. the amount, if the payment status is the literal “Completed”;<br>2. otherwise 0. |
| **DR-72 Order Amount Due** | A payment's order amount due is the amount due of the payment's invoice. |
| **DR-73 Order is Paid in Full** | A payment's order is paid in full is true when the payment's invoice is a paid in full. |
| **DR-74 Type of Address is Shipping Address** | An address's type of address is shipping address is true when the address's type of address is a shipping address. |
| **DR-75 Type of Address is Billing Address** | An address's type of address is billing address is true when the address's type of address is a billing address. |
| **DR-76 State Code** | An address's state code is the code of the address's state. |
| **DR-77 State Tax Rate** | An address's state tax rate is the tax rate of the address's state. |

## 4 Traceability to Schema

_The expression column is the rule's definition in RuleSpeak notation —
the same logic the rulebook stores, written for a business reader._

| Schema element | Kind | Expression |
|----------------|------|------------|
| **Clients.CategoryName** | lookup | `Lookup(ClientCategories.Name via Category)` |
| **Clients.CategoryDiscount** | lookup | `Lookup(ClientCategories.Discount via Category)` |
| **Clients.IsStopped** | lookup | `Lookup(Statuses.IsBlocking via Status)` |
| **Clients.StatusDisplayName** | lookup | `Lookup(Statuses.DisplayName via Status)` |
| **Clients.StatusDescription** | lookup | `Lookup(Statuses.Description via Status)` |
| **Clients.CountOfBigInvoices** | rollup | `Count(Invoices via Client)` |
| **Clients.BillingAddressStateTaxRate** | lookup | `Lookup(Addresses.StateTaxRate via Addresses)` |
| **Clients.BillingAddress** | lookup | `Lookup(Addresses.Name via Addresses)` |
| **Clients.ShippingAddress** | lookup | `Lookup(Addresses.Name via Addresses)` |
| **Clients.AverageOrderValue** | rollup | `Average(Invoices.InvoiceTotal via Client)` |
| **Clients.IsVIP** | formula | `If(And(AverageOrderValue > 500, HasRecentInvoices), True(), False())` |
| **Clients.HasRecentInvoices** | formula | `If(LastInvoice > Dateadd(Today(), -220, 'days'), True(), False())` |
| **Clients.LastInvoice** | rollup | `Sum(Invoices.OrderDate via Client)` |
| **Clients.CustomerSinceDays** | formula | `If(CreatedAt, Datetime_diff(Now(), CreatedAt, 'days'), Blank())` |
| **ClientApprovals.ApprovedByContactName** | lookup | `Lookup(AppUsers.ContactName via ApprovedBy)` |
| **ClientApprovals.ApprovedByEmailAddress** | lookup | `Lookup(AppUsers.EmailAddress via ApprovedBy)` |
| **ClientApprovals.ApprovedByPhoneNumber** | lookup | `Lookup(AppUsers.PhoneNumber via ApprovedBy)` |
| **ClientApprovals.ApprovedByRole** | lookup | `Lookup(AppUsers.Role via ApprovedBy)` |
| **ClientApprovals.IsApproved** | formula | `Not(ApprovedBy = Blank())` |
| **ClientApprovals.ClientName** | lookup | `Lookup(Clients.Name via Client)` |
| **ClientApprovals.ClientEmail** | lookup | `Lookup(Clients.Email via Client)` |
| **ClientApprovals.ClientPhone** | lookup | `Lookup(Clients.Phone via Client)` |
| **ClientApprovals.ClientCategory** | lookup | `Lookup(Clients.Category via Client)` |
| **ClientCategories.CountOfClients** | rollup | `Count(Clients via Category)` |
| **Statuses.CountOfClients** | rollup | `Count(Clients via Status)` |
| **Products.Profit** | formula | `UnitPrice - Cost` |
| **Products.Margin** | formula | `1 - (Cost / UnitPrice)` |
| **Products.IsHighMargin** | formula | `Margin > 0.65` |
| **Products.StockQuantity** | rollup | `Sum(InventoryAdjustments.Quantity via Product)` |
| **Products.COGS** | formula | `StockQuantity * Cost` |
| **Products.CountOfVIPOrders** | rollup | `Count(InvoiceLineItems via Product)` |
| **Products.HasBeenOrderedByVIPCustomers** | formula | `If(And(CountOfVIPOrders > 0, StockQuantity > 250), True(), False())` |
| **Invoices.ClientIsVIP** | lookup | `Lookup(Clients.IsVIP via Client)` |
| **Invoices.ClientHastRecentInvoices** | lookup | `Lookup(Clients.HasRecentInvoices via Client)` |
| **Invoices.ClientEmail** | lookup | `Lookup(Clients.Email via Client)` |
| **Invoices.ClientPhone** | lookup | `Lookup(Clients.Phone via Client)` |
| **Invoices.ClientCompanyName** | lookup | `Lookup(Clients.CompanyName via Client)` |
| **Invoices.ClientCategoryName** | lookup | `Lookup(Clients.CategoryName via Client)` |
| **Invoices.ClientCategoryDiscount** | lookup | `Lookup(Clients.CategoryDiscount via Client)` |
| **Invoices.ItemCount** | rollup | `Count(InvoiceLineItems via Invoice)` |
| **Invoices.TotalQuantity** | rollup | `Sum(InvoiceLineItems.Quantity via Invoice)` |
| **Invoices.SubTotal** | rollup | `Sum(InvoiceLineItems.SubTotal via Invoice)` |
| **Invoices.IsBigOrder** | formula | `SubTotal > 350` |
| **Invoices.TaxRate** | lookup | `Lookup(Clients.BillingAddressStateTaxRate via Client)` |
| **Invoices.TaxAmount** | formula | `SubTotal * TaxRate` |
| **Invoices.InvoiceTotal** | formula | `SubTotal + TaxAmount` |
| **Invoices.TotalPaid** | rollup | `Sum(Payments.CompletedAmount via Invoice)` |
| **Invoices.AmountDue** | formula | `Round(InvoiceTotal - TotalPaid, 2)` |
| **Invoices.IsPaidInFull** | formula | `If(AmountDue <= 0, True(), False())` |
| **Invoices.PaymentCount** | rollup | `Count(Payments via Invoice)` |
| **Invoices.LastPaymentDate** | rollup | `Sum(Payments.PaymentDate via Invoice)` |
| **Invoices.PaymentStatusLabel** | formula | `If(IsPaidInFull, "Paid", If(TotalPaid = 0, "Unpaid", "Partial"))` |
| **InvoiceLineItems.ClientIsVIP** | lookup | `Lookup(Invoices.ClientIsVIP via Invoice)` |
| **InvoiceLineItems.ClientHastRecentInvoices** | lookup | `Lookup(Invoices.ClientHastRecentInvoices via Invoice)` |
| **InvoiceLineItems.invoiceNumber** | lookup | `Lookup(Invoices.invoiceNumber via Invoice)` |
| **InvoiceLineItems.InvoiceTotal** | lookup | `Lookup(Invoices.InvoiceTotal via Invoice)` |
| **InvoiceLineItems.InvoiceTotalPaid** | lookup | `Lookup(Invoices.TotalPaid via Invoice)` |
| **InvoiceLineItems.ProductSKU** | lookup | `Lookup(Products.SKU via Product)` |
| **InvoiceLineItems.ProductDisplayName** | lookup | `Lookup(Products.DisplayName via Product)` |
| **InvoiceLineItems.UnitPrice** | lookup | `Lookup(Products.UnitPrice via Product)` |
| **InvoiceLineItems.PreDiscount** | formula | `Quantity * UnitPrice` |
| **InvoiceLineItems.DiscountPercent** | lookup | `Lookup(Invoices.ClientCategoryDiscount via Invoice)` |
| **InvoiceLineItems.DiscountAmount** | formula | `PreDiscount * DiscountPercent` |
| **InvoiceLineItems.SubTotal** | formula | `PreDiscount - DiscountAmount` |
| **InventoryAdjustments.Adjustment_Name** | formula | `Product & "-" & Left(Date, 10) & "-qty" & Quantity` |
| **Payments.InvoiceDate** | lookup | `Lookup(Invoices.OrderDate via Invoice)` |
| **Payments.InvoiceNumber** | lookup | `Lookup(Invoices.invoiceNumber via Invoice)` |
| **Payments.InvoiceStatus** | lookup | `Lookup(Invoices.OrderStatus via Invoice)` |
| **Payments.InvoiceTotal** | lookup | `Lookup(Invoices.InvoiceTotal via Invoice)` |
| **Payments.IsCompleted** | formula | `If(PaymentStatus = "Completed", True(), False())` |
| **Payments.CompletedAmount** | formula | `If(PaymentStatus = "Completed", Amount, 0)` |
| **Payments.OrderAmountDue** | lookup | `Lookup(Invoices.AmountDue via Invoice)` |
| **Payments.OrderIsPaidInFull** | lookup | `Lookup(Invoices.IsPaidInFull via Invoice)` |
| **Addresses.TypeOfAddressIsShippingAddress** | lookup | `Lookup(TypesOfAddresses.IsShippingAddress via TypeOfAddress)` |
| **Addresses.TypeOfAddressIsBillingAddress** | lookup | `Lookup(TypesOfAddresses.IsBillingAddress via TypeOfAddress)` |
| **Addresses.StateCode** | lookup | `Lookup(States.Code via State)` |
| **Addresses.StateTaxRate** | lookup | `Lookup(States.TaxRate via State)` |
