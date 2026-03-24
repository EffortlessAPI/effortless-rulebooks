# Specification Document for ACME, LLC Rulebook

## Overview
This specification document outlines the structure and computation methods for the rulebook generated from the Airtable base "ACME, LLC (template)". It details the entities involved, their fields, and how to compute calculated fields based on raw input data.

## Customers Entity

### Input Fields
The following input fields are available in the Customers entity:

1. **CustomerId**
   - **Type:** String
   - **Description:** Unique identifier for each customer.

2. **Customer**
   - **Type:** String
   - **Description:** Identifier for the customers.

3. **EmailAddress**
   - **Type:** String
   - **Description:** The customer's email address.

4. **FirstName**
   - **Type:** String
   - **Description:** First name of the customer, used to create the full name.

5. **LastName**
   - **Type:** String
   - **Description:** Last name of the customer, used to create the full name.

### Calculated Field

#### FullName
- **Type:** Calculated
- **Description:** The full name of the customer is computed by combining the last name and first name, formatted as "LastName, FirstName".

**Computation Method:**
To compute the `FullName`, concatenate the `LastName` and `FirstName` fields with a comma and a space in between. The formula for this operation is:

```
={{LastName}} & ", " & {{FirstName}}
```

**Example Calculation:**
Using the provided data:

- For the customer with `FirstName` "Mary" and `LastName` "Smith":
  - FullName = "Smith, Mary"

- For the customer with `FirstName` "John" and `LastName` "Doe":
  - FullName = "Doe, John"

- For the customer with `FirstName` "Emily" and `LastName` "Jones":
  - FullName = "Jones, Emily"

This results in the following computed `FullName` values:
- "Smith, Mary"
- "Doe, John"
- "Jones, Emily"

By following the above computation method, one can accurately derive the `FullName` for any customer using their `FirstName` and `LastName`.