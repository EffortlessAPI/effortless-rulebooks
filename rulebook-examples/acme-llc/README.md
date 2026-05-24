# ACME, LLC Ontology

**A lean variant of ACME Corporation—essential business rules in a simplified schema.**

ACME, LLC strips away complexity to show the minimum viable model for order-to-fulfillment workflows. It's ideal for learning ERB patterns or for domains with streamlined operations.

---

## What This Models

### Core Entities

- **Clients**: Minimal customer master (name, contact)
- **Orders**: Purchase orders (order number, date, client, total)
- **Line Items**: Order detail (product, quantity, price)
- **Fulfillment**: Simple shipment tracking (date, quantity shipped, status)

### Business Logic

- **Order Status**: Open → Shipped → Complete
- **Order Total**: Sum of line items
- **Fulfillment %**: Percentage of order quantity shipped

---

## Tables

| Table | Purpose | Key Fields |
|-------|---------|-----------|
| **Clients** | Customer master | `ClientName`, `Email`, `Phone` |
| **Orders** | Purchase orders | `OrderNumber` (Name/PK), `OrderDate`, `ClientId`, `OrderTotal` |
| **LineItems** | Order detail | `Product`, `Quantity`, `UnitPrice`, `ExtendedPrice` |
| **Fulfillment** | Shipments | `OrderId`, `ShipDate`, `QuantityShipped`, `Status` |

---

## Example Formulas

### Order.OrderNumber (Name)
```
"ORD-" & TEXT(OrderDate, "YYYYMMDD") & "-" & ClientId
```

### Order.OrderTotal (Aggregation)
```
SUM(LineItems.ExtendedPrice WHERE LineItems.Order == Orders.Name)
```

### Order.Status (Derived)
```
IF(FulfillmentComplete == 100%, "Complete",
   IF(FulfillmentComplete > 0, "Shipped",
      "Open"))
```

---

## Use Case: Order Fulfillment Workflow

1. **Create order** for a client → `OrderNumber` auto-generates
2. **Add line items** → `OrderTotal` auto-computes
3. **Ship items** → Create fulfillment record
4. **Monitor progress** → `Status` updates to "Shipped"
5. **Complete shipment** → `Status` becomes "Complete"

All logic lives in the rulebook; Postgres, Python, and Go all enforce it identically.

---

## Editing

### From Airtable (base ID: `appWrXPvXbkgQGOxt`)

```bash
effortless airtabletorulebook
effortless build
```

### Directly

Edit `effortless-rulebook/effortless-rulebook.json`, then:

```bash
effortless build
```

---

**The rulebook is the specification. Everything else is derived.**
