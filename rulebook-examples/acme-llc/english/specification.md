# ACME, LLC Rulebook Specification

## Overview
This rulebook outlines the structure and calculations for managing customer data at ACME, LLC. It provides a client ledger where each customer is identified by two unique identifiers: a slugified handle derived from their email and a full name formatted in a legal-style (last-name-first). The rulebook allows for automatic recalculation of these identifiers based on changes to the raw input fields.

## Entity: Customers

### Input Fields
1. **CustomerId**
   - **Type:** string
   - **Description:** Unique identifier for each customer.

2. **EmailAddress**
   - **Type:** string
   - **Description:** The customer's email address.

3. **FirstName**
   - **Type:** string
   - **Description:** The first name of the customer, used to create the full name.

4. **LastName**
   - **Type:** string
   - **Description:** The last name of the customer, used to create the full name.

### Derived Fields
1. **Name**
   - **Type:** calculated
   - **Formula:** `=SUBSTITUTE({{EmailAddress}}, "@", "-")`
   - **Computation:** The `Name` field is derived by replacing the "@" character in the `EmailAddress` with a hyphen ("-"). This creates a slugified version of the email address.
   - **Example:** If `EmailAddress` is `jane.smith@email.com`, then `Name` becomes `jane.smith-email.com`.

2. **Initials**
   - **Type:** calculated
   - **Formula:** `=LEFT({{FirstName}}, 1) & LEFT({{LastName}}, 1)`
   - **Computation:** The `Initials` field is computed by taking the first letter of the `FirstName` and concatenating it with the first letter of the `LastName`.
   - **Example:** If `FirstName` is `Bobby` and `LastName` is `Smith`, then `Initials` becomes `BS`.

3. **FullName**
   - **Type:** calculated
   - **Formula:** `={{FirstName}} & " " & {{LastName}}`
   - **Computation:** The `FullName` field is constructed by concatenating the `FirstName` and `LastName` with a space in between. The order is last-name-first, formatted as "LastName, FirstName".
   - **Example:** If `FirstName` is `Bobby` and `LastName` is `Smith`, then `FullName` becomes `Smith, Bobby`.

### Summary of Examples
- For a customer with:
  - `EmailAddress`: `jane.smith@email.com`
  - `FirstName`: `Bobby`
  - `LastName`: `Smith`
  
  The computed fields would be:
  - `Name`: `jane.smith-email.com`
  - `Initials`: `BS`
  - `FullName`: `Smith, Bobby`

This specification provides a clear guide for deriving the necessary fields from the raw inputs for the Customers entity in the ACME, LLC rulebook. Each calculation is straightforward and can be replicated using the provided examples.