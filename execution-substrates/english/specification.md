# ACME, LLC Rulebook Specification

## Overview
This rulebook outlines the structure and calculations for customer data management at ACME, LLC. It includes raw input fields for customer information and derived fields that compute identifiers based on the provided data. The calculated fields are designed to automatically update when the underlying raw data changes, ensuring consistency across all generated outputs.

## Customers Entity

### Input Fields
The following are the raw input fields for the Customers entity:

1. **EmailAddress**
   - **Type:** string
   - **Description:** The customer's email address.

2. **FirstName**
   - **Type:** string
   - **Description:** First name of the customer, used to create the full name.

3. **LastName**
   - **Type:** string
   - **Description:** Last name of the customer, used to create the full name.

4. **CustomerId**
   - **Type:** string
   - **Description:** Unique identifier for the customer.

### Derived Fields

1. **Name**
   - **Type:** calculated
   - **Description:** An identifier for the customer derived from their email address.
   - **Computation:** The `Name` is computed by replacing the "@" character in the `EmailAddress` with a hyphen ("-"). 
   - **Original Formula:** `=SUBSTITUTE({{EmailAddress}}, "@", "-")`
   - **Example:** For a customer with `EmailAddress` of `bob@gmail.com`, the calculation results in `Name = bob-gmail.com`.

2. **FullName**
   - **Type:** calculated
   - **Description:** The full name of the customer, created by combining the first and last names.
   - **Computation:** The `FullName` is computed by concatenating the `FirstName` and `LastName` with a space in between.
   - **Original Formula:** `={{FirstName}} & " " & {{LastName}}`
   - **Example:** For a customer with `FirstName` of `Bobby` and `LastName` of `Smith`, the calculation results in `FullName = "Bobby Smith"`.

### Data Examples
Here are examples of how the derived fields are computed based on the raw input fields:

- **Customer 1:**
  - **EmailAddress:** `bob@gmail.com`
  - **FirstName:** `Bobby`
  - **LastName:** `Smith`
  - **Computed Name:** `bob-gmail.com`
  - **Computed FullName:** `Bobby Smith`

- **Customer 2:**
  - **EmailAddress:** `jimmy@gmail.com`
  - **FirstName:** `Jimmy`
  - **LastName:** `Doe`
  - **Computed Name:** `jimmy-gmail.com`
  - **Computed FullName:** `Jimmy Doe`

- **Customer 3:**
  - **EmailAddress:** `mary@gmail.com`
  - **FirstName:** `Mary`
  - **LastName:** `Jones`
  - **Computed Name:** `mary-gmail.com`
  - **Computed FullName:** `Mary Jones`

This specification provides a clear understanding of how to compute the derived fields for the Customers entity in the ACME, LLC rulebook, ensuring accurate data representation and consistency across all outputs.