# ACME, LLC Rulebook Specification Document

## Overview
This document outlines the specifications for the ACME, LLC rulebook, which defines the structure and calculations for customer data. The rulebook includes raw input fields and derived fields that are calculated based on the input data. The goal is to provide a clear understanding of how to compute each derived field using the specified formulas and data.

## Entity: Customers

### Input Fields (Type: Raw)
1. **CustomerId**
   - **Type:** String
   - **Description:** Unique identifier for each customer. This field is mandatory.

2. **EmailAddress**
   - **Type:** String
   - **Description:** The customer's email address. This field is optional.

3. **FirstName**
   - **Type:** String
   - **Description:** The first name of the customer, used to create the full name. This field is optional.

4. **LastName**
   - **Type:** String
   - **Description:** The last name of the customer, used to create the full name. This field is optional.

### Derived Fields

1. **Name**
   - **Type:** Calculated
   - **Description:** This field serves as an identifier for the customers. It is computed by replacing the "@" symbol in the customer's email address with a hyphen ("-").
   - **Computation:** 
     - Take the value from the **EmailAddress** field.
     - Replace "@" with "-".
   - **Original Formula:** `=SUBSTITUTE({{EmailAddress}}, "@", "-")`
   - **Example:** For a customer with the email address `jane.smith@email.com`, the computed Name would be `jane.smith-email.com`.

2. **FullName**
   - **Type:** Calculated
   - **Description:** This field represents the full name of the customer, which is derived from the combination of the first and last names.
   - **Computation:** 
     - Concatenate the values from the **FirstName** and **LastName** fields, separated by a space.
   - **Original Formula:** `={{FirstName}} & " " & {{LastName}}`
   - **Example:** For a customer with FirstName as `Bob` and LastName as `Smith`, the computed FullName would be `Bob Smith`.

### Example Data
Here are examples of how the derived fields are computed based on the provided customer data:

- **Customer 1:**
  - **CustomerId:** `jane-smith-email-com`
  - **EmailAddress:** `jane.smith@email.com`
  - **FirstName:** `Bob`
  - **LastName:** `Smith`
  - **Computed Name:** `jane.smith-email.com` (derived from `jane.smith@email.com`)
  - **Computed FullName:** `Bob Smith` (derived from `Bob` and `Smith`)

- **Customer 2:**
  - **CustomerId:** `john-doe-email-com`
  - **EmailAddress:** `john.doe@email.com`
  - **FirstName:** `Jimmy`
  - **LastName:** `Doe`
  - **Computed Name:** `john.doe-email.com` (derived from `john.doe@email.com`)
  - **Computed FullName:** `Jimmy Doe` (derived from `Jimmy` and `Doe`)

- **Customer 3:**
  - **CustomerId:** `emily-jones-email-com`
  - **EmailAddress:** `emily.jones@email.com`
  - **FirstName:** `Mary`
  - **LastName:** `Jones`
  - **Computed Name:** `emily.jones-email.com` (derived from `emily.jones@email.com`)
  - **Computed FullName:** `Mary Jones` (derived from `Mary` and `Jones`)

This specification provides a comprehensive guide to understanding how to compute the derived fields in the Customers table of the ACME, LLC rulebook. Each calculation is based on the raw input fields, ensuring clarity and accuracy in data processing.