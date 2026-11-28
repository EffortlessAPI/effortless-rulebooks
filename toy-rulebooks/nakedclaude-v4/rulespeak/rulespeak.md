# 📘 v4: NakedClaude Demo — Docs

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

<details open>
<summary>Example data</summary>

| Name ƒ | Contact Name | Email Address | Phone Number | Role | Notes |
|---|---|---|---|---|---|
| customer-ssot.me-customer | Bob Customer | customer@ssot.me | (555) 555-1234 | Customer | — |
| ej-ssot.me-admin | EJ Alexandra | ej@ssot.me | (555) 555-1111 | Admin | — |
| manager-ssot.me-manager | Mark Manager | manager@ssot.me | (555) 555-4321 | Manager | — |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Client** | A client is identified by its name and is related to optionally a client category (its category) and optionally a status. | — |
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

<details open>
<summary>Example data</summary>

| Name | Notes | Category Name ƒ | Category Discount ƒ | Is Stopped ƒ | Status Display Name ƒ | Status Description ƒ | Count of Big Invoices ƒ | Email | Phone | Company Name | Billing Address State Tax Rate ƒ | Billing Address ƒ | Shipping Address ƒ | Created At | Average Order Value ƒ | Is VIP ƒ | Has Recent Invoices ƒ | Last Invoice ƒ | Customer Since Days ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| Bob | A customer currently in launch. | Active | 0.05 | true | On-Hold | This status indicates the customer or request is on hold and temporarily paused. | 1 | bob@launchpad.example | +1-415-555-0101 | Launchpad LLC | 0.0625 | 742 Evergreen Terrace, Springfield, il-illinois 97477 | 742 Evergreen Terrace, Springfield, il-illinois 97477 | 2025-08-14T09:30:00Z | 150.3565 | false | false | 0 | 471 |
| Alice Johnson | Initial mock entry for Alice. | Active | 0.05 | true | Pending | This status shows the customer or request is pending further action or review. | 1 | alice.johnson@northwind.example | +1-206-555-0142 | Northwind Traders | 0.065 | 500 Pine St, Suite 200, Seattle, wa-washington 98101 | 500 Pine St, Suite 200, Seattle, wa-washington 98101 | 2025-09-02T14:15:00Z | 5412.1036875 | false | false | 0 | 452 |
| Brian Lee | Initial mock entry for Brian. | Prospect | 0.0 | false | New | This status indicates a newly created customer or request that has not yet been processed. | 3 | brian.lee@contoso.example | +1-512-555-0177 | Contoso Manufacturing | 0.0625 | 88 Industrial Way, Austin, tx-texas 78701 | 88 Industrial Way, Austin, tx-texas 78701 | 2025-10-11T11:00:00Z | 657.6414583333334 | false | true | 0 | 413 |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
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

<details open>
<summary>Example data</summary>

| Name ƒ | Approved by Contact Name ƒ | Approved by Email Address ƒ | Approved by Phone Number ƒ | Approved by Role ƒ | Is Approved ƒ | Notes | Client Name ƒ | Client Email ƒ | Client Phone ƒ | Client Category ƒ |
|---|---|---|---|---|---|---|---|---|---|---|
| alice-johnson-APVD | EJ Alexandra | ej@ssot.me | (555) 555-1111 | Admin | true | — | Alice Johnson | alice.johnson@northwind.example | +1-206-555-0142 | active |
| brian-lee-APVD | EJ Alexandra | ej@ssot.me | (555) 555-1111 | Admin | true | — | Brian Lee | brian.lee@contoso.example | +1-512-555-0177 | prospect |
| brian-lee-PEND | — | — | — | — | false | — | Brian Lee | brian.lee@contoso.example | +1-512-555-0177 | prospect |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Client Category** | A client category is identified by its name. | — |
| Name | A defined attribute. | — |
| Clients | A defined attribute. | — |
| Discount | A defined attribute. | — |
| Notes | A defined attribute. | — |
| Count of Clients | The number of clients related to the client category. | — |

<details open>
<summary>Example data</summary>

| Name | Discount | Notes | Count of Clients ƒ |
|---|---|---|---|
| Prospect | 0 | — | 1 |
| Gold | 0.15 | — | 1 |
| Active | 0.05 | — | 3 |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Status** | A status is identified by its name. | — |
| Name | Computed as the lower-cased display name. | _Lowercase, dash-separated compound key derived from DisplayName. Used as the natural key for the status._ |
| Display Name | A defined attribute. | _Human-readable display name of the status (e.g., "On-Hold", "In-Review")._ |
| Description | A defined attribute. | _Free-form description of what this status means and when it applies._ |
| Is Blocking | True when an empty string. | _True when customers in this status should be considered stopped (blocked from progressing)._ |
| Clients | A defined attribute. | _Reverse link to all Customers currently in this status._ |
| Count of Clients | The number of clients related to the status. | — |
| Sort Order | A defined attribute. | _Display order for this status in lists._ |

<details open>
<summary>Example data</summary>

| Name ƒ | Display Name | Description | Is Blocking | Count of Clients ƒ | Sort Order |
|---|---|---|---|---|---|
| delayed | Delayed | This status is used when the customer or request has been delayed and is not progressing as scheduled. | false | 2 | 6 |
| cancelled | Cancelled | This status means the customer or request has been cancelled and will not proceed further. | false | 0 | 7 |
| processing | Processing | This status means the customer or request is currently being processed. | false | 0 | 2 |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
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

<details open>
<summary>Example data</summary>

| Name ƒ | SKU | Display Name | Description | Unit Price | Cost | Profit ƒ | Margin ƒ | Is Active | Is High Margin ƒ | Stock Quantity ƒ | COGS ƒ | Count of VIP Orders ƒ | Has Been Ordered by VIP Customers ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| gadget-200 | GADGET-200 | Pro Gadget | Professional-grade gadget with extended battery life. | 89 | 32 | 57.0 | 0.6404494382022472 | true | false | 75 | 2400.0 | 3 | false |
| hub-7port | HUB-7PORT | 7-Port USB Hub | Powered 7-port USB-A hub with individual switches. | 34.99 | 11.8 | 23.19 | 0.662760788796799 | true | true | 240 | 2832.0 | 2 | false |
| cable-usb-c | CABLE-USB-C | USB-C Cable (2m) | Two-meter braided USB-C to USB-C cable, 100W power delivery. | 12 | 3.4 | 8.6 | 0.7166666666666667 | true | true | 1240 | 4216.0 | 4 | true |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
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

<details open>
<summary>Example data</summary>

| Name ƒ | invoice Number | Client is VIP ƒ | Client Hast Recent Invoices ƒ | Client Email ƒ | Client Phone ƒ | Client Company Name ƒ | Client Category Name ƒ | Client Category Discount ƒ | Order Date | Order Status | Shipping Address | Billing Address | Notes | Item Count ƒ | Total Quantity ƒ | Sub Total ƒ | Is Big Order ƒ | Tax Rate ƒ | Tax Amount ƒ | Invoice Total ƒ | Total Paid ƒ | Amount Due ƒ | Is Paid in Full ƒ | Payment Count ƒ | Last Payment Date ƒ | Payment Status Label ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| brian-lee-1042 | 1042 | false | — | brian.lee@contoso.example | +1-512-555-0177 | Contoso Manufacturing | Prospect | 0.0 | 2026-02-20T13:15:00Z | Pending | 742 Evergreen Terrace<br>Springfield, OR 97477 | 742 Evergreen Terrace<br>Springfield, OR 97477 | Awaiting payment confirmation. Customer is on-hold. | 2 | 2 | 123.99000000000001 | false | 0.0625 | 7.749375000000001 | 131.739375 | 0.0 | 131.74 | false | 1 | 0 | Unpaid |
| caroline-1045 | 1045 | false | — | caroline@solo.example | +1-718-555-0199 | Solo Studio | Gold | 0.15 | 2026-03-12T10:00:00Z | New | 23 Hudson Lane<br>Brooklyn, NY 11201 | 23 Hudson Lane<br>Brooklyn, NY 11201 | First-time customer. Marketing wants white-glove handling. | 3 | 4 | 99.00800000000001 | false | 0.04 | 3.9603200000000003 | 102.96832 | 126.82 | -23.85 | true | 2 | 0 | Paid |
| alice-johnson-1010 | 1010 | false | — | alice.johnson@northwind.example | +1-206-555-0142 | Northwind Traders | Active | 0.05 | 2025-10-05T09:00:00Z | Shipped | 500 Pine St, Suite 200<br>Seattle, WA 98101 | 500 Pine St, Suite 200<br>Seattle, WA 98101 | Bulk order for Q4 inventory. | 4 | 250 | 5081.7875 | true | 0.065 | 330.3161875 | 5412.1036875 | 4770.4400000000005 | 641.66 | false | 2 | 0 | Partial |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
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

<details open>
<summary>Example data</summary>

| Name ƒ | Line Number | Client is VIP ƒ | Client Hast Recent Invoices ƒ | invoice Number ƒ | Invoice Total ƒ | Invoice Total Paid ƒ | Product SKU ƒ | Product Display Name ƒ | Quantity | Unit Price ƒ | Pre Discount ƒ | Discount Percent ƒ | Discount Amount ƒ | Sub Total ƒ | Notes |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| brian-lee-1037-line-2 | 2 | false | — | 1037 | 1089.0625 | 203.73 | GADGET-200 | Pro Gadget | 10 | 89.0 | 890.0 | 0.0 | 0.0 | 890.0 | Goodwill discount for prior return. |
| carla-smith-1024-line-2 | 2 | false | — | 1024 | 225.568 | 0.0 | GADGET-200 | Pro Gadget | 1 | 89.0 | 89.0 | 0.05 | 4.45 | 84.55 | Returned: damaged in transit. |
| caroline-1045-line-3 | 3 | false | — | 1045 | 102.96832 | 126.82 | CABLE-USB-C | USB-C Cable (2m) | 1 | 12.0 | 12.0 | 0.15 | 1.7999999999999998 | 10.2 | Free with welcome bundle. |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Inventory Adjustment** | An inventory adjustment is identified by its date and is related to optionally a product. | — |
| Adjustment Name | Computed as the product, followed by a hyphen, followed by the first 10 character(s) of the date, followed by “-qty”, followed by the quantity. | — |
| Product | A defined attribute. | — |
| Date | A defined attribute. | — |
| Adjustment Type | A defined attribute. | — |
| Quantity | A defined attribute. | — |
| Reason | A defined attribute. | — |
| Notes | A defined attribute. | — |
| Adjusted by | A defined attribute. | — |

<details open>
<summary>Example data</summary>

| Adjustment Name ƒ | Date | Adjustment Type | Quantity | Reason | Notes | Adjusted by |
|---|---|---|---|---|---|---|
| widget-002-2026-04-01-qty180 | 2026-04-01T22:10:00Z | Correction | 180 | Transfer | — | bob |
| hub-7port-2026-04-25-qty240 | 2026-04-25T22:11:00Z | Correction | 240 | Transfer | — | larry |
| gadget-100-2026-04-09-qty612 | 2026-04-09T22:10:00Z | Correction | 612 | Transfer | — | larry |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
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

<details open>
<summary>Example data</summary>

| Name ƒ | Payment Number | Payment Date | Payment Method | Transaction ID | Notes | Invoice Date ƒ | Invoice Number ƒ | Invoice Status ƒ | Invoice Total ƒ | Amount | Payment Status | Is Completed ƒ | Completed Amount ƒ | Order Amount Due ƒ | Order is Paid in Full ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| caroline-1045-pmt-1 | 1 | 2026-03-12T10:05:00Z | CreditCard | ch_failed_9981 | Card declined; customer to retry. | 2026-03-12T10:00:00Z | 1045 | New | 102.96832 | 126.82 | Failed | false | 0.0 | -23.85 | true |
| caroline-1045-pmt-2 | 2 | 2026-03-12T11:30:00Z | Cash | CASH-NY-20260312-001 | Customer paid in cash at pickup. | 2026-03-12T10:00:00Z | 1045 | New | 102.96832 | 126.82 | Completed | true | 126.82 | -23.85 | true |
| alice-johnson-1010-pmt-2 | 2 | 2025-10-12T11:30:00Z | BankTransfer | wire_NWT_20251012_b | Final balance. | 2025-10-05T09:00:00Z | 1010 | Shipped | 5412.1036875 | 3270.44 | Completed | true | 3270.44 | 641.66 | false |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
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

<details open>
<summary>Example data</summary>

| Name ƒ | Type of Address is Shipping Address ƒ | Type of Address is Billing Address ƒ | Attention | Address1 | Address2 | City | State Code ƒ | State Tax Rate ƒ | Zip |
|---|---|---|---|---|---|---|---|---|---|
| 500 Pine St, Suite 200, Seattle, wa-washington 98101 | false | true | — | 500 Pine St, Suite 200 | — | Seattle | WA | 0.065 | 98101 |
| 742 Evergreen Terrace, Springfield, il-illinois 97477 | false | true | — | 742 Evergreen Terrace | — | Springfield | IL | 0.0625 | 97477 |
| 23 Hudson Lane, Brooklyn, ny-new-york 11201 | false | true | — | 23 Hudson Lane | — | Brooklyn | NY | 0.04 | 11201 |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **State** | A state is identified by its name. | — |
| Name | Computed as the lower-cased code, followed by a hyphen, followed by the text with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> | — |
| Code | A defined attribute. | — |
| Text | A defined attribute. | — |
| Tax Rate | A defined attribute. | — |
| Addresses | A defined attribute. | — |

<details open>
<summary>Example data</summary>

| Name ƒ | Code | Text | Tax Rate |
|---|---|---|---|
| ia-iowa | IA | Iowa | 0.06 |
| mo-missouri | MO | Missouri | 0.04225 |
| az-arizona | AZ | Arizona | 0.056 |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Type of Addresses** | A type of addresses is identified by its name. | — |
| Name | A defined attribute. | — |
| Addresses | A defined attribute. | — |
| Is Shipping Address | True when an empty string. | — |
| Is Billing Address | True when an empty string. | — |

<details open>
<summary>Example data</summary>

| Name | Is Shipping Address | Is Billing Address |
|---|---|---|
| Shipping | true | false |
| Billing | false | true |

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **ERB Version** | An ERB version is identified by its name. | — |
| Name | A defined attribute. | — |
| Base ID | A defined attribute. | — |
| Version | A defined attribute. | — |
| Message | A defined attribute. | — |
| Notes | A defined attribute. | — |
| Commit Date | A defined attribute. | — |
| Is Published | True when an empty string. | — |
| Author | A defined attribute. | — |

<details open>
<summary>Example data</summary>

| Name | Base ID | Version | Message | Notes | Commit Date | Is Published | Author |
|---|---|---|---|---|---|---|---|
| v2026-04-07t00-45-14-766z | 8d009876-5ceb-47a0-9b94-db653e0774dd | Version 2026-04-07 00:45:14 | Testing function fixes. | — | 2026-04-07 | false | ej@ssot.me |
| Version 2026-04-07 01:56:19 | 8d009876-5ceb-47a0-9b94-db653e0774dd | — | fixed payments | — | 2026-04-07 | false | ej@ssot.me |
| Version 2026-04-07 01:54:04 | 8d009876-5ceb-47a0-9b94-db653e0774dd | — | tixed functions | — | 2026-04-07 | false | ej@ssot.me |

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **ERB Customization** | An ERB customization is identified by its name. | — |
| Name | A defined attribute. | — |
| Title | A defined attribute. | — |
| SQL Code | A defined attribute. | — |
| SQL Target | A defined attribute. | — |
| Customization Type | A defined attribute. | — |

<details open>
<summary>Example data</summary>

| Name | Title | SQL Code | SQL Target | Customization Type |
|---|---|---|---|---|
| calc_orders_item_count | — | -- FunctionOverride: calc_orders_item_count<br>-- Errors in this file are non-fatal (runs without ON_ERROR_STOP).<br><br>CREATE OR REPLACE FUNCTION public.calc_orders_item_count(p_order_id text)<br> RETURNS integer<br> LANGUAGE plpgsql<br> STABLE SECURITY DEFINER<br>AS $function$<br>BEGIN<br>  RETURN ((SELECT COUNT(*) FROM order_line_items WHERE "order" = p_order_id))::integer;<br>END;<br>$function$;<br> | Postgres | FunctionOverride |
| calc_orders_last_payment_date | — | -- FunctionOverride: calc_orders_last_payment_date<br>-- Errors in this file are non-fatal (runs without ON_ERROR_STOP).<br><br>CREATE OR REPLACE FUNCTION public.calc_orders_last_payment_date(p_order_id text)<br> RETURNS timestamp with time zone<br> LANGUAGE plpgsql<br> STABLE SECURITY DEFINER<br>AS $function$<br>BEGIN<br>  RETURN (SELECT MAX(payment_date) FROM payments WHERE "order" = p_order_id);<br>END;<br>$function$;<br> | Postgres | FunctionOverride |
| calc_clients_hast_recent_invoices | — | -- FunctionOverride: calc_clients_hast_recent_invoices<br>-- Errors in this file are non-fatal (runs without ON_ERROR_STOP).<br>-- Original Airtable formula: =IF({{LastInvoice}} > DATEADD(TODAY(), -180, 'days'), TRUE(), FALSE())<br>-- Translation failed because DATEADD unit 'days' is unsupported. Reimplement directly.<br><br>CREATE OR REPLACE FUNCTION public.calc_clients_hast_recent_invoices(p_client_id text)<br> RETURNS boolean<br> LANGUAGE plpgsql<br> STABLE SECURITY DEFINER<br>AS $function$<br>BEGIN<br>  RETURN COALESCE(<br>    (SELECT MAX(order_date) FROM invoices WHERE client = p_client_id) > (NOW() - INTERVAL '180 days'),<br>    FALSE<br>  );<br>END;<br>$function$;<br> | Postgres | FunctionOverride |

</details>

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

_The expression column is the rule's definition in Docs notation —
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

> **RuleSpeak®** — the declarative business-rule notation created by **Ronald G. Ross**, following the conventions of **SBVR** — is coming soon. Until then this page shows the regular documentation. [www.RonRoss.info](https://www.RonRoss.info)
