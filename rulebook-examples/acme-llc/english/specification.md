# ACME, LLC Rulebook Specification

## Overview

This rulebook defines the structure and calculations for managing customer data within ACME, LLC. It includes a single entity, `Customers`, which captures essential customer information and derives two calculated fields: `Name` and `FullName`. The rulebook ensures that these derived fields automatically update based on changes to the raw input fields, providing a seamless experience for data management.

---

## Entity: Customers

### Input Fields

1. **CustomerId**
   - **Type:** string
   - **Description:** Unique identifier for the customer.

2. **EmailAddress**
   - **Type:** string
   - **Description:** The customer's email address.

3. **FirstName**
   - **Type:** string
   - **Description:** First name of the customer, used to create the full name.

4. **LastName**
   - **Type:** string
   - **Description:** Last name of the customer, used to create the full name.

### Calculated Fields

1. **Name**
   - **Type:** calculated
   - **Formula:** `=SUBSTITUTE({{EmailAddress}}, "@", "-")`
   - **Computation:** The `Name` field is derived from the `EmailAddress` by replacing the "@" symbol with a hyphen ("-"). This transformation creates a slugified version of the email address.
   - **Example:** If the `EmailAddress` is `jane.smith@email.com`, the `Name` will be `jane.smith-email.com`.

2. **FullName**
   - **Type:** calculated
   - **Formula:** `={{FirstName}} & " " & {{LastName}}`
   - **Computation:** The `FullName` field is constructed by concatenating the `FirstName` and `LastName` fields with a space in between. This field formats the customer's name in a standard way.
   - **Example:** If `FirstName` is `Bobby` and `LastName` is `Smith`, the `FullName` will be `Bobby Smith`.

### Additional Calculated Field

3. **Initials**
   - **Type:** calculated
   - **Formula:** `=LEFT({{FirstName}}, 1) & LEFT({{LastName}}, 1)`
   - **Computation:** The `Initials` field is created by taking the first character of the `FirstName` and the first character of the `LastName` and concatenating them together.
   - **Example:** If `FirstName` is `Bobby` and `LastName` is `Smith`, the `Initials` will be `BS`.

---

## Summary of Calculated Fields

- **Name**: Derived from `EmailAddress` by replacing "@" with "-".
- **FullName**: Constructed from `FirstName` and `LastName` with a space in between.
- **Initials**: Formed by concatenating the first letters of `FirstName` and `LastName`.

This specification provides a clear understanding of how to compute the derived fields based on the raw input fields within the `Customers` entity of ACME, LLC's rulebook.