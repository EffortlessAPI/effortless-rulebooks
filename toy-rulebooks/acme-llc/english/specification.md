# ACME, LLC Rulebook Specification

## Overview
This rulebook defines the structure and calculations for managing customer data at ACME, LLC. It includes a single entity, `Customers`, which contains both raw and calculated fields. The calculated fields derive their values based on the input fields, allowing for a dynamic and responsive customer ledger.

## Entity: Customers

### Input Fields
The following are the raw input fields for the `Customers` entity:

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
The following are the calculated fields for the `Customers` entity, along with their computation methods:

1. **Name**
   - **Type:** calculated
   - **Formula:** `=SUBSTITUTE({{EmailAddress}}, "@", "-")`
   - **Computation:** This field is derived from the `EmailAddress` by replacing the "@" symbol with a hyphen ("-"). This transformation creates a slugified handle for the customer.
   - **Example:** If `EmailAddress` is `jane.smith@email.com`, then `Name` becomes `jane.smith-email.com`.

2. **Initials**
   - **Type:** calculated
   - **Formula:** `=LEFT({{FirstName}}, 1) & LEFT({{LastName}}, 1)`
   - **Computation:** This field is computed by taking the first letter of the `FirstName` and concatenating it with the first letter of the `LastName`.
   - **Example:** If `FirstName` is `Bobby` and `LastName` is `Smith`, then `Initials` becomes `BS`.

3. **FullName**
   - **Type:** calculated
   - **Formula:** `={{FirstName}} & " " & {{LastName}}`
   - **Computation:** This field is constructed by concatenating the `FirstName` and `LastName` with a space in between. The order is important as it formats the name in a standard way.
   - **Example:** If `FirstName` is `Bobby` and `LastName` is `Smith`, then `FullName` becomes `Bobby Smith`.

### Summary of Calculated Fields
- **Name**: Derived from `EmailAddress` by replacing "@" with "-".
- **Initials**: First letter of `FirstName` + first letter of `LastName`.
- **FullName**: Concatenation of `FirstName` and `LastName` with a space.

### Data Examples
Here are some examples of how the calculated fields are derived from the raw inputs:

1. For a customer with:
   - `EmailAddress`: `jane.smith@email.com`
   - `FirstName`: `Bobby`
   - `LastName`: `Smith`
   
   The derived fields would be:
   - `Name`: `jane.smith-email.com`
   - `Initials`: `BS`
   - `FullName`: `Bobby Smith`

2. For another customer with:
   - `EmailAddress`: `john.doe@email.com`
   - `FirstName`: `Jimmy`
   - `LastName`: `Doe`
   
   The derived fields would be:
   - `Name`: `john.doe-email.com`
   - `Initials`: `JD`
   - `FullName`: `Jimmy Doe`

This specification provides a clear understanding of how to compute each field within the `Customers` entity of the ACME, LLC rulebook, ensuring accurate and consistent data representation.