# ACME, LLC Rulebook Specification

## Overview
This rulebook defines the structure and calculations for managing customer data at ACME, LLC. It includes a single entity, `Customers`, which contains raw input fields and calculated fields that derive additional identifiers for each customer. The calculated fields automatically update based on changes to the raw input fields, ensuring consistency across all data representations.

## Entity: Customers

### Input Fields
1. **EmailAddress**
   - **Type:** String (raw)
   - **Description:** The customer's email address.

2. **FirstName**
   - **Type:** String (raw)
   - **Description:** The first name of the customer, used to create the full name.

3. **LastName**
   - **Type:** String (raw)
   - **Description:** The last name of the customer, used to create the full name.

4. **CustomerId**
   - **Type:** String (raw)
   - **Description:** A unique identifier for each customer.

### Calculated Fields
1. **Name**
   - **Type:** String (calculated)
   - **Description:** A handle derived from the email address.
   - **Computation:** The `Name` field is computed by replacing the "@" symbol in the `EmailAddress` with a hyphen ("-"). This creates a slugified version of the email.
   - **Original Formula:** `=SUBSTITUTE({{EmailAddress}}, "@", "-")`
   - **Example:** If `EmailAddress` is `jane.smith@email.com`, then `Name` becomes `jane.smith-email.com`.

2. **Initials**
   - **Type:** String (calculated)
   - **Description:** The initials of the customer, derived from their first and last names.
   - **Computation:** The `Initials` field is computed by taking the first character of the `FirstName` and the first character of the `LastName`, and concatenating them.
   - **Original Formula:** `=LEFT({{FirstName}}, 1) & LEFT({{LastName}}, 1)`
   - **Example:** If `FirstName` is `Bobby` and `LastName` is `Smith`, then `Initials` becomes `BS`.

3. **FullName**
   - **Type:** String (calculated)
   - **Description:** The full name of the customer formatted in a last-name-first style.
   - **Computation:** The `FullName` field is computed by concatenating the `LastName`, a comma, and the `FirstName`. This formats the name in a courtroom-style representation.
   - **Original Formula:** `={{FirstName}} & " " & {{LastName}}`
   - **Example:** If `FirstName` is `Bobby` and `LastName` is `Smith`, then `FullName` becomes `Smith, Bobby`.

### Data Example
Here’s how the fields would be populated for a customer:

- **EmailAddress:** `jane.smith@email.com`
- **FirstName:** `Bobby`
- **LastName:** `Smith`
- **CustomerId:** `jane-smith-email-com`

From the above inputs, the calculated fields would derive as follows:

- **Name:** `jane.smith-email.com` (derived from `EmailAddress`)
- **Initials:** `BS` (derived from `FirstName` and `LastName`)
- **FullName:** `Smith, Bobby` (derived from `FirstName` and `LastName`)

This specification provides a clear understanding of how to compute the derived fields based on the raw input fields for the `Customers` entity in the ACME, LLC rulebook.