# Specification Document for DEMO: Customer FullName Rulebook

## Overview
This rulebook provides a structured approach to managing customer data, specifically focusing on the computation of the full name of customers based on their first and last names. It is derived from an Airtable base and includes a schema for the customer data as well as a calculated field for the full name.

## Customers Table

### Input Fields
The following input fields are used to compute the calculated field:

1. **FirstName**
   - **Type:** String
   - **Description:** The first name of the customer, used to create the full name.

2. **LastName**
   - **Type:** String
   - **Description:** The last name of the customer, used to create the full name.

### Calculated Field

#### FullName
- **Type:** Calculated
- **Description:** The full name of the customer is computed by concatenating the first name and last name with a space in between.

**Computation Explanation:**
To compute the `FullName`, take the value from the `FirstName` field and the value from the `LastName` field. Concatenate these two values together with a space character in between.

**Formula for Reference:**
```
={{FirstName}} & " " & {{LastName}}
```

**Concrete Example:**
Using the data provided in the rulebook:

- For the first customer:
  - **FirstName:** "Jane"
  - **LastName:** "Smith"
  - **Computed FullName:** "Jane Smith"

- For the second customer:
  - **FirstName:** "John"
  - **LastName:** "Doe"
  - **Computed FullName:** "John Doe"

- For the third customer:
  - **FirstName:** "Emily"
  - **LastName:** "Jones"
  - **Computed FullName:** "Emily Jones"

This specification allows for the accurate computation of the `FullName` field based on the provided first and last names of customers.