# ACME, LLC Rulebook Specification

## Overview
This rulebook defines the structure and calculations for managing customer data at ACME, LLC. It includes raw input fields for customer information and calculated fields that derive additional identifiers from these inputs. The calculated fields include `Name` and `FullName`, which are essential for generating customer records.

## Customer Entity

### Input Fields
The following are the raw input fields for the Customer entity:

1. **CustomerId**
   - **Type:** string
   - **Description:** A unique identifier for the customer.

2. **EmailAddress**
   - **Type:** string
   - **Description:** The customer's email address.

3. **FirstName**
   - **Type:** string
   - **Description:** The first name of the customer, used to construct the full name.

4. **LastName**
   - **Type:** string
   - **Description:** The last name of the customer, used to construct the full name.

### Calculated Fields
The following are the calculated fields derived from the input fields:

1. **Name**
   - **Description:** An identifier for the customer derived from their email address.
   - **Computation:** This field is computed by replacing the "@" symbol in the `EmailAddress` with a hyphen ("-"). 
   - **Original Formula:** `=SUBSTITUTE({{EmailAddress}}, "@", "-")`
   - **Example:** For a customer with `EmailAddress` of `bob@gmail.com`, the `Name` would be computed as `bob-gmail.com`.

2. **FullName**
   - **Description:** The full name of the customer, constructed from their first and last names.
   - **Computation:** This field is computed by concatenating the `FirstName` and `LastName` with a space in between.
   - **Original Formula:** `={{FirstName}} & " " & {{LastName}}`
   - **Example:** For a customer with `FirstName` of "Bobby" and `LastName` of "Smith", the `FullName` would be computed as `Bobby Smith`.

### Summary of Example Data
Here are examples of how the calculated fields are derived from the input fields for three customers:

- **Customer 1:**
  - **CustomerId:** bob-gmail-com
  - **EmailAddress:** bob@gmail.com
  - **FirstName:** Bobby
  - **LastName:** Smith
  - **Calculated Name:** `bob-gmail.com` (computed from `bob@gmail.com`)
  - **Calculated FullName:** `Bobby Smith` (computed from `Bobby` and `Smith`)

- **Customer 2:**
  - **CustomerId:** jimmy-gmail-com
  - **EmailAddress:** jimmy@gmail.com
  - **FirstName:** Jimmy
  - **LastName:** Doe
  - **Calculated Name:** `jimmy-gmail.com` (computed from `jimmy@gmail.com`)
  - **Calculated FullName:** `Jimmy Doe` (computed from `Jimmy` and `Doe`)

- **Customer 3:**
  - **CustomerId:** mary-gmail-com
  - **EmailAddress:** mary@gmail.com
  - **FirstName:** Mary
  - **LastName:** Jones
  - **Calculated Name:** `mary-gmail.com` (computed from `mary@gmail.com`)
  - **Calculated FullName:** `Mary Jones` (computed from `Mary` and `Jones`)

This specification outlines the necessary steps to compute the derived fields for the Customer entity in the ACME, LLC rulebook, ensuring clarity and accuracy in data management.