# 📘 v4: NakedClaude Demo — RuleSpeak®

_Rulebook generated from Airtable base 'v4: NakedClaude Demo'._

> Declarative business rules rendered from the rulebook. Every statement
> below expresses truth in the business domain — it is neither a procedure
> nor an imperative. The rulebook's formulas are the single source of truth;
> this document is their plain-language reading.

## 1 Business Vocabulary

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **App User** | An app user is identified by its name. | — |
| Name | Computed as the lower-cased email address, followed by a hyphen, followed by the role with every “@” replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> | — |
| Contact Name | A defined attribute. | — |
| Email Address | A defined attribute. | — |
| Phone Number | A defined attribute. | — |
| Role | A defined attribute. | — |
| Notes | A defined attribute. | — |
| Client Approvals | A defined attribute. | — |
| **Client** | A client is identified by its name and is related to optionally a client category (its category) and optionally a status (its status). | — |
| Name | A defined attribute. | _Human-readable customer name._ |
| Notes | A defined attribute. | _Free-form notes about the customer._ |
| Category | A defined attribute. | — |
| Category Name | Taken from the linked category. | — |
| Category Discount | Taken from the linked category. | — |
| Is Stopped | True when the client's status is blocking. | _True when the customer's current Status is a blocking status. Lookup of Statuses.IsBlocking._ |
| Status | A defined attribute. | _Link to the customer's current Status in the Statuses table._ |
| Status Display Name | Taken from the linked status. | _Human-readable display name of the customer's current Status. Lookup of Statuses.DisplayName._ |
| Status Description | Taken from the linked status. | _Description of the customer's current Status. Lookup of Statuses.Description._ |
| Invoices | A defined attribute. | _Reverse link to all Orders placed by this customer._ |
| Count of Big Invoices | The number of invoices related to the client. | — |
| Email | A defined attribute. | _Primary email address for the customer._ |
| Phone | A defined attribute. | _Primary phone number for the customer._ |
| Company Name | A defined attribute. | _Company the customer represents, if applicable._ |
| Addresses | A defined attribute. | — |
| Billing Address State Tax Rate | Taken from the linked addresses. | — |
| Billing Address | The name of the client's addresses. | _Default billing address for the customer._ |
| Shipping Address | The name of the client's addresses. | _Default shipping address for the customer._ |
| Created At | A defined attribute. | _Timestamp when the customer record was created._ |
| Average Order Value | The average invoice total across the invoices related to the client. | — |
| Is VIP | True when all of the following hold: the average order value is greater than 500 and the recent invoices flag is set. | — |
| Has Recent Invoices | True when the last invoice is greater than the negative of 220 days after today's date. ⚠︎ mechanical <!-- rulespeak:reword --> | — |
| Last Invoice | The total order date across the invoices related to the client. | — |
| Customer Since Days | Determined by priority: the number of days from the created at to the current date and time if the created at flag is set; in all other cases, no value. ⚠︎ mechanical <!-- rulespeak:reword --> | — |
| Client Approvals | A defined attribute. | — |
| **Client Approval** | A client approval is identified by its name and is related to optionally a client and optionally an app user (its approved by). | — |
| Name | Computed as the client, followed by a hyphen, followed by “APVD” if the approved flag is set, in all other cases “PEND”. | — |
| Client | A defined attribute. | — |
| Approved by | A defined attribute. | — |
| Approved by Contact Name | Taken from the linked approved by. | — |
| Approved by Email Address | Taken from the linked approved by. | — |
| Approved by Phone Number | Taken from the linked approved by. | — |
| Approved by Role | Taken from the linked approved by. | — |
| Is Approved | True when the approved by has a value. | — |
| Notes | A defined attribute. | — |
| Client Name | Taken from the linked client. | — |
| Client Email | Taken from the linked client. | — |
| Client Phone | Taken from the linked client. | — |
| Client Category | Taken from the linked client. | — |
| **Client Category** | A client category is identified by its name. | — |
| Name | A defined attribute. | — |
| Clients | A defined attribute. | — |
| Discount | A defined attribute. | — |
| Notes | A defined attribute. | — |
| Count of Clients | The number of clients related to the client category. | — |
| **Status** | A status is identified by its name. | — |
| Name | Computed as the lower-cased display name. | _Lowercase, dash-separated compound key derived from DisplayName. Used as the natural key for the status._ |
| Display Name | A defined attribute. | _Human-readable display name of the status (e.g., "On-Hold", "In-Review")._ |
| Description | A defined attribute. | _Free-form description of what this status means and when it applies._ |
| Is Blocking | True when an empty string. | _True when customers in this status should be considered stopped (blocked from progressing)._ |
| Clients | A defined attribute. | _Reverse link to all Customers currently in this status._ |
| Count of Clients | The number of clients related to the status. | — |
| Sort Order | A defined attribute. | _Display order for this status in lists._ |
| **Product** | A product is identified by its name. | — |
| Name | Computed as the lower-cased SKU with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> | _Lowercase, dash-separated compound key derived from SKU. Used as the natural key for the product._ |
| SKU | A defined attribute. | _Stock-keeping unit identifier for the product. The natural compound key._ |
| Invoice Line Items | A defined attribute. | _Reverse link to all OrderLineItems that reference this product._ |
| Display Name | A defined attribute. | _Human-readable product name shown in catalogs and orders._ |
| Description | A defined attribute. | _Long-form product description._ |
| Unit Price | A defined attribute. | _Default list price per unit._ |
| Cost | A defined attribute. | _Internal cost per unit._ |
| Profit | Computed as the unit price minus the cost. | — |
| Margin | Computed as 1 minus the cost divided by the unit price. | — |
| Is Active | True when an empty string. | _True when the product is currently sellable._ |
| Is High Margin | True when the margin is greater than 0.65. | — |
| Stock Quantity | The total quantity across the inventory adjustments related to the product. | _Number of units currently in stock._ |
| COGS | Computed as the stock quantity times the cost. | — |
| Inventory Adjustments | A defined attribute. | — |
| Count of VIP Orders | The number of invoice line items related to the product. | — |
| Has Been Ordered by VIP Customers | True when all of the following hold: the count of VIP orders is greater than 0 and the stock quantity is greater than 250. | — |
| **Invoice** | An invoice is identified by its name and is related to optionally a client. | — |
| Name | Computed as the lower-cased client, followed by a hyphen, followed by the invoice number with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> | _Lowercase compound key combining the linked Customer and OrderNumber (e.g. 'bob-1001'). Used as the natural key for the order._ |
| invoice Number | A defined attribute. | _Sequential or human-assigned order identifier (e.g. '1001'). Combined with Customer to form the order's natural key._ |
| Client | A defined attribute. | _Link to the Customer who placed this order._ |
| Client is VIP | True when the invoice's client is a VIP. | — |
| Client Hast Recent Invoices | True when the linked client has a recent invoices. | — |
| Client Email | Taken from the linked client. | — |
| Client Phone | Taken from the linked client. | — |
| Client Company Name | Taken from the linked client. | — |
| Client Category Name | Taken from the linked client. | — |
| Client Category Discount | Taken from the linked client. | — |
| Invoice Line Items | A defined attribute. | _Reverse link to all OrderLineItems on this order._ |
| Payments | A defined attribute. | _Reverse link to all Payments applied to this order._ |
| Order Date | A defined attribute. | _When the order was placed._ |
| Order Status | A defined attribute. | _Lifecycle state of the order._ |
| Shipping Address | A defined attribute. | _Where this specific order ships to._ |
| Billing Address | A defined attribute. | _Billing address used for this specific order._ |
| Notes | A defined attribute. | _Free-form notes about this order._ |
| Item Count | The number of invoice line items related to the invoice. | — |
| Total Quantity | The total quantity across the invoice line items related to the invoice. | — |
| Sub Total | The total sub total across the invoice line items related to the invoice. | — |
| Is Big Order | True when the sub total is greater than 350. | — |
| Tax Rate | The billing address state tax rate of the invoice's client. | _Sales tax rate applied to this order._ |
| Tax Amount | Computed as the sub total times the tax rate. | — |
| Invoice Total | Computed as the sub total plus the tax amount. | — |
| Total Paid | The total completed amount across the payments related to the invoice. | — |
| Amount Due | Computed as the invoice total minus the total paid rounded to 2 decimal place(s). | — |
| Is Paid in Full | True when the amount due is at most 0. | — |
| Payment Count | The number of payments related to the invoice. | — |
| Last Payment Date | The total payment date across the payments related to the invoice. | — |
| Payment Status Label | Determined by priority: “Paid” if the paid in full flag is set; “Unpaid” if the total paid is 0; in all other cases, “Partial”. | — |
| **Invoice Line Item** | An invoice line item is identified by its name and is related to optionally an invoice and optionally a product. | — |
| Name | Computed as the lower-cased invoice, followed by “-line-”, followed by the line number with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> | _Lowercase compound key combining the linked Order and LineNumber (e.g. 'bob-1001-line-1'). Used as the natural key for the line item._ |
| Line Number | A defined attribute. | _Position of this line within its parent Order (e.g. '1', '2'). Combined with Order to form the line item's natural key._ |
| Invoice | A defined attribute. | _Link to the parent Order this line item belongs to._ |
| Client is VIP | True when the invoice line item's invoice is a VIP. | — |
| Client Hast Recent Invoices | True when the linked invoice is client hast recent invoices. | — |
| invoice Number | Taken from the linked invoice. | — |
| Invoice Total | Taken from the linked invoice. | — |
| Invoice Total Paid | Taken from the linked invoice. | — |
| Product | A defined attribute. | _Link to the Product being ordered on this line._ |
| Product SKU | Taken from the linked product. | — |
| Product Display Name | Taken from the linked product. | — |
| Quantity | A defined attribute. | _Number of units ordered on this line._ |
| Unit Price | Taken from the linked product. | _Price per unit captured at the time of ordering (may differ from current Product.UnitPrice)._ |
| Pre Discount | Computed as the quantity times the unit price. | — |
| Discount Percent | The client category discount of the invoice line item's invoice. | _Per-line discount applied to this item._ |
| Discount Amount | Computed as the pre discount times the discount percent. | — |
| Sub Total | Computed as the pre discount minus the discount amount. | — |
| Notes | A defined attribute. | _Free-form notes about this line item._ |
| **Inventory Adjustment** | An inventory adjustment is identified by its date and is related to optionally a product. | — |
| Adjustment Name | Computed as the product, followed by a hyphen, followed by the first 10 character(s) of the date, followed by “-qty”, followed by the quantity. | — |
| Product | A defined attribute. | — |
| Date | A defined attribute. | — |
| Adjustment Type | A defined attribute. | — |
| Quantity | A defined attribute. | — |
| Reason | A defined attribute. | — |
| Notes | A defined attribute. | — |
| Adjusted by | A defined attribute. | — |
| **Payment** | A payment is identified by its name and is related to optionally an invoice. | — |
| Name | Computed as the lower-cased invoice, followed by “-pmt-”, followed by the payment number with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> | _Lowercase compound key combining the linked Order and PaymentNumber (e.g. 'bob-1001-pmt-1'). Used as the natural key for the payment._ |
| Payment Number | A defined attribute. | _Sequential payment identifier within the parent Order (e.g. '1', '2'). Combined with Order to form the payment's natural key._ |
| Invoice | A defined attribute. | _Link to the Order this payment is being applied to._ |
| Payment Date | A defined attribute. | _When the payment was received._ |
| Payment Method | A defined attribute. | _How the payment was made._ |
| Transaction ID | A defined attribute. | _External processor transaction identifier._ |
| Notes | A defined attribute. | _Free-form notes about this payment._ |
| Invoice Date | The order date of the payment's invoice. | — |
| Invoice Number | Taken from the linked invoice. | — |
| Invoice Status | The order status of the payment's invoice. | — |
| Invoice Total | Taken from the linked invoice. | — |
| Amount | A defined attribute. | _Amount paid._ |
| Payment Status | A defined attribute. | _Lifecycle state of the payment._ |
| Is Completed | True when the payment status is “Completed”. | — |
| Completed Amount | Determined by priority: the amount if the payment status is “Completed”; in all other cases, 0. | — |
| Order Amount Due | Taken from the linked invoice. | — |
| Order is Paid in Full | True when the linked invoice is a paid in full. | — |
| **Address** | An address is identified by its name and is related to optionally a type of addresses (its type of address) and optionally a state. | — |
| Name | Computed as the address1, followed by a comma followed by a space, followed by the city, followed by a comma followed by a space, followed by the state, followed by a space, followed by the zip. | — |
| Clients | A defined attribute. | — |
| Type of Address | A defined attribute. | — |
| Type of Address is Shipping Address | True when the linked type of address is a shipping address. | — |
| Type of Address is Billing Address | True when the linked type of address is a billing address. | — |
| Attention | A defined attribute. | — |
| Address1 | A defined attribute. | — |
| Address2 | A defined attribute. | — |
| City | A defined attribute. | — |
| State | A defined attribute. | — |
| State Code | Taken from the linked state. | — |
| State Tax Rate | Taken from the linked state. | — |
| Zip | A defined attribute. | — |
| **State** | A state is identified by its name. | — |
| Name | Computed as the lower-cased code, followed by a hyphen, followed by the text with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> | — |
| Code | A defined attribute. | — |
| Text | A defined attribute. | — |
| Tax Rate | A defined attribute. | — |
| Addresses | A defined attribute. | — |
| **Type of Addresses** | A type of addresses is identified by its name. | — |
| Name | A defined attribute. | — |
| Addresses | A defined attribute. | — |
| Is Shipping Address | True when an empty string. | — |
| Is Billing Address | True when an empty string. | — |
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
| **DR-1 Name** | An app user's name is computed as the lower-cased email address, followed by a hyphen, followed by the role with every “@” replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> |
| **DR-2 Category Name** | A client's category name — taken from the linked category. |
| **DR-3 Category Discount** | A client's category discount — taken from the linked category. |
| **DR-4 Is Stopped** | A client's is stopped is true when the client's status is blocking. |
| **DR-5 Status Display Name** | A client's status display name — taken from the linked status. |
| **DR-6 Status Description** | A client's status description — taken from the linked status. |
| **DR-7 Count of Big Invoices** | A client's count of big invoices is the number of invoices related to the client. |
| **DR-8 Billing Address State Tax Rate** | A client's billing address state tax rate — taken from the linked addresses. |
| **DR-9 Billing Address** | A client's billing address is the name of the client's addresses. |
| **DR-10 Shipping Address** | A client's shipping address is the name of the client's addresses. |
| **DR-11 Average Order Value** | A client's average order value is the average invoice total across the invoices related to the client. |
| **DR-12 Is VIP** | A client is considered a VIP if all of the following hold: the average order value is greater than 500 and the recent invoices flag is set. |
| **DR-13 Has Recent Invoices** | A client is considered to have a recent invoices if the last invoice is greater than the negative of 220 days after today's date. ⚠︎ mechanical <!-- rulespeak:reword --> |
| **DR-14 Last Invoice** | A client's last invoice is the total order date across the invoices related to the client. |
| **DR-15 Customer Since Days** | The client's customer since days is determined by the following priority:<br>1. the number of days from the created at to the current date and time, if the created at flag is set;<br>2. in all other cases, no value. ⚠︎ mechanical <!-- rulespeak:reword --> |
| **DR-16 Name** | A client approval's name is computed as the client, followed by a hyphen, followed by “APVD” if the approved flag is set, in all other cases “PEND”. |
| **DR-17 Approved by Contact Name** | A client approval's approved by contact name — taken from the linked approved by. |
| **DR-18 Approved by Email Address** | A client approval's approved by email address — taken from the linked approved by. |
| **DR-19 Approved by Phone Number** | A client approval's approved by phone number — taken from the linked approved by. |
| **DR-20 Approved by Role** | A client approval's approved by role — taken from the linked approved by. |
| **DR-21 Is Approved** | A client approval is considered approved if the approved by has a value. |
| **DR-22 Client Name** | A client approval's client name — taken from the linked client. |
| **DR-23 Client Email** | A client approval's client email — taken from the linked client. |
| **DR-24 Client Phone** | A client approval's client phone — taken from the linked client. |
| **DR-25 Client Category** | A client approval's client category — taken from the linked client. |
| **DR-26 Count of Clients** | A client category's count of clients is the number of clients related to the client category. |
| **DR-27 Name** | A status's name is computed as the lower-cased display name. |
| **DR-28 Count of Clients** | A status's count of clients is the number of clients related to the status. |
| **DR-29 Name** | A product's name is computed as the lower-cased SKU with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> |
| **DR-30 Profit** | A product's profit is computed as the unit price minus the cost. |
| **DR-31 Margin** | A product's margin is computed as 1 minus the cost divided by the unit price. |
| **DR-32 Is High Margin** | A product is considered a high margin if the margin is greater than 0.65. |
| **DR-33 Stock Quantity** | A product's stock quantity is the total quantity across the inventory adjustments related to the product. |
| **DR-34 COGS** | A product's COGS is computed as the stock quantity times the cost. |
| **DR-35 Count of VIP Orders** | A product's count of VIP orders is the number of invoice line items related to the product. |
| **DR-36 Has Been Ordered by VIP Customers** | A product is considered to have a been ordered by VIP customers if all of the following hold: the count of VIP orders is greater than 0 and the stock quantity is greater than 250. |
| **DR-37 Name** | An invoice's name is computed as the lower-cased client, followed by a hyphen, followed by the invoice number with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> |
| **DR-38 Client is VIP** | An invoice's client is VIP is true when the invoice's client is a VIP. |
| **DR-39 Client Hast Recent Invoices** | An invoice's client hast recent invoices when the linked client has a recent invoices. |
| **DR-40 Client Email** | An invoice's client email — taken from the linked client. |
| **DR-41 Client Phone** | An invoice's client phone — taken from the linked client. |
| **DR-42 Client Company Name** | An invoice's client company name — taken from the linked client. |
| **DR-43 Client Category Name** | An invoice's client category name — taken from the linked client. |
| **DR-44 Client Category Discount** | An invoice's client category discount — taken from the linked client. |
| **DR-45 Item Count** | An invoice's item count is the number of invoice line items related to the invoice. |
| **DR-46 Total Quantity** | An invoice's total quantity is the total quantity across the invoice line items related to the invoice. |
| **DR-47 Sub Total** | An invoice's sub total is the total sub total across the invoice line items related to the invoice. |
| **DR-48 Is Big Order** | An invoice is considered a big order if the sub total is greater than 350. |
| **DR-49 Tax Rate** | An invoice's tax rate is the billing address state tax rate of the invoice's client. |
| **DR-50 Tax Amount** | An invoice's tax amount is computed as the sub total times the tax rate. |
| **DR-51 Invoice Total** | An invoice's invoice total is computed as the sub total plus the tax amount. |
| **DR-52 Total Paid** | An invoice's total paid is the total completed amount across the payments related to the invoice. |
| **DR-53 Amount Due** | An invoice's amount due is computed as the invoice total minus the total paid rounded to 2 decimal place(s). |
| **DR-54 Is Paid in Full** | An invoice is considered a paid in full if the amount due is at most 0. |
| **DR-55 Payment Count** | An invoice's payment count is the number of payments related to the invoice. |
| **DR-56 Last Payment Date** | An invoice's last payment date is the total payment date across the payments related to the invoice. |
| **DR-57 Payment Status Label** | The invoice's payment status label is determined by the following priority:<br>1. “Paid”, if the paid in full flag is set;<br>2. “Unpaid”, if the total paid is 0;<br>3. in all other cases, “Partial”. |
| **DR-58 Name** | An invoice line item's name is computed as the lower-cased invoice, followed by “-line-”, followed by the line number with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> |
| **DR-59 Client is VIP** | An invoice line item's client is VIP is true when the invoice line item's invoice is a VIP. |
| **DR-60 Client Hast Recent Invoices** | An invoice line item's client hast recent invoices when the linked invoice is client hast recent invoices. |
| **DR-61 invoice Number** | An invoice line item's invoice number — taken from the linked invoice. |
| **DR-62 Invoice Total** | An invoice line item's invoice total — taken from the linked invoice. |
| **DR-63 Invoice Total Paid** | An invoice line item's invoice total paid — taken from the linked invoice. |
| **DR-64 Product SKU** | An invoice line item's product SKU — taken from the linked product. |
| **DR-65 Product Display Name** | An invoice line item's product display name — taken from the linked product. |
| **DR-66 Unit Price** | An invoice line item's unit price — taken from the linked product. |
| **DR-67 Pre Discount** | An invoice line item's pre discount is computed as the quantity times the unit price. |
| **DR-68 Discount Percent** | An invoice line item's discount percent is the client category discount of the invoice line item's invoice. |
| **DR-69 Discount Amount** | An invoice line item's discount amount is computed as the pre discount times the discount percent. |
| **DR-70 Sub Total** | An invoice line item's sub total is computed as the pre discount minus the discount amount. |
| **DR-71 Adjustment Name** | An inventory adjustment's adjustment name is computed as the product, followed by a hyphen, followed by the first 10 character(s) of the date, followed by “-qty”, followed by the quantity. |
| **DR-72 Name** | A payment's name is computed as the lower-cased invoice, followed by “-pmt-”, followed by the payment number with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> |
| **DR-73 Invoice Date** | A payment's invoice date is the order date of the payment's invoice. |
| **DR-74 Invoice Number** | A payment's invoice number — taken from the linked invoice. |
| **DR-75 Invoice Status** | A payment's invoice status is the order status of the payment's invoice. |
| **DR-76 Invoice Total** | A payment's invoice total — taken from the linked invoice. |
| **DR-77 Is Completed** | A payment is considered completed if the payment status is “Completed”. |
| **DR-78 Completed Amount** | The payment's completed amount is determined by the following priority:<br>1. the amount, if the payment status is “Completed”;<br>2. in all other cases, 0. |
| **DR-79 Order Amount Due** | A payment's order amount due — taken from the linked invoice. |
| **DR-80 Order is Paid in Full** | A payment's order is paid in full when the linked invoice is a paid in full. |
| **DR-81 Name** | An address's name is computed as the address1, followed by a comma followed by a space, followed by the city, followed by a comma followed by a space, followed by the state, followed by a space, followed by the zip. |
| **DR-82 Type of Address is Shipping Address** | An address's type of address is shipping address when the linked type of address is a shipping address. |
| **DR-83 Type of Address is Billing Address** | An address's type of address is billing address when the linked type of address is a billing address. |
| **DR-84 State Code** | An address's state code — taken from the linked state. |
| **DR-85 State Tax Rate** | An address's state tax rate — taken from the linked state. |
| **DR-86 Name** | A state's name is computed as the lower-cased code, followed by a hyphen, followed by the text with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> |

## 5 Traceability to Schema

_The expression column is the rule's definition in RuleSpeak® notation —
the same logic the rulebook stores, written for a business reader._

| Schema element | Kind | Expression |
|----------------|------|------------|
| **AppUsers.Name** | formula | `Replace(Lower(EmailAddress & "-" & Role), "@", "-")` |
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
| **Clients.HasRecentInvoices** | formula | `If(LastInvoice > DateAdd(Today(), -220, "days"), True(), False())` |
| **Clients.LastInvoice** | rollup | `Sum(Invoices.OrderDate via Client)` |
| **Clients.CustomerSinceDays** | formula | `If(CreatedAt, DaysBetween(Now(), CreatedAt), Blank())` |
| **ClientApprovals.Name** | formula | `Client & "-" & If(IsApproved, "APVD", "PEND")` |
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
| **Statuses.Name** | formula | `Lower(DisplayName)` |
| **Statuses.CountOfClients** | rollup | `Count(Clients via Status)` |
| **Products.Name** | formula | `Lower(Replace(SKU, " ", "-"))` |
| **Products.Profit** | formula | `UnitPrice - Cost` |
| **Products.Margin** | formula | `1 - Cost / UnitPrice` |
| **Products.IsHighMargin** | formula | `Margin > 0.65` |
| **Products.StockQuantity** | rollup | `Sum(InventoryAdjustments.Quantity via Product)` |
| **Products.COGS** | formula | `StockQuantity * Cost` |
| **Products.CountOfVIPOrders** | rollup | `Count(InvoiceLineItems via Product)` |
| **Products.HasBeenOrderedByVIPCustomers** | formula | `If(And(CountOfVIPOrders > 0, StockQuantity > 250), True(), False())` |
| **Invoices.Name** | formula | `Lower(Replace(Client & "-" & invoiceNumber, " ", "-"))` |
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
| **InvoiceLineItems.Name** | formula | `Lower(Replace(Invoice & "-line-" & LineNumber, " ", "-"))` |
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
| **Payments.Name** | formula | `Lower(Replace(Invoice & "-pmt-" & PaymentNumber, " ", "-"))` |
| **Payments.InvoiceDate** | lookup | `Lookup(Invoices.OrderDate via Invoice)` |
| **Payments.InvoiceNumber** | lookup | `Lookup(Invoices.invoiceNumber via Invoice)` |
| **Payments.InvoiceStatus** | lookup | `Lookup(Invoices.OrderStatus via Invoice)` |
| **Payments.InvoiceTotal** | lookup | `Lookup(Invoices.InvoiceTotal via Invoice)` |
| **Payments.IsCompleted** | formula | `If(PaymentStatus = "Completed", True(), False())` |
| **Payments.CompletedAmount** | formula | `If(PaymentStatus = "Completed", Amount, 0)` |
| **Payments.OrderAmountDue** | lookup | `Lookup(Invoices.AmountDue via Invoice)` |
| **Payments.OrderIsPaidInFull** | lookup | `Lookup(Invoices.IsPaidInFull via Invoice)` |
| **Addresses.Name** | formula | `Address1 & ", " & City & ", " & State & " " & Zip` |
| **Addresses.TypeOfAddressIsShippingAddress** | lookup | `Lookup(TypesOfAddresses.IsShippingAddress via TypeOfAddress)` |
| **Addresses.TypeOfAddressIsBillingAddress** | lookup | `Lookup(TypesOfAddresses.IsBillingAddress via TypeOfAddress)` |
| **Addresses.StateCode** | lookup | `Lookup(States.Code via State)` |
| **Addresses.StateTaxRate** | lookup | `Lookup(States.TaxRate via State)` |
| **States.Name** | formula | `Lower(Code & "-" & Replace(Text, " ", "-"))` |

---

_This document is rendered in **RuleSpeak®**, the declarative business-rule
notation created by **Ronald G. Ross**, and follows the conventions of
**SBVR** (Semantics of Business Vocabulary and Business Rules). With thanks to
Ronald G. Ross for RuleSpeak® and his foundational work on business rules —
[www.RonRoss.info](https://www.RonRoss.info)._
