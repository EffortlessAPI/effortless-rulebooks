# ACME, LLC Rulebook Specification Document

## Overview
This document provides a detailed specification for the rulebook generated from the Airtable base "ACME, LLC (template)". It outlines the input fields and explains how to compute each derived field within the Customers table.

## Customers Table

### Input Fields
The following fields are defined as raw inputs in the Customers table:

1. **CustomerId**
   - **Type:** String
   - **Description:** Unique identifier for each customer. This field is mandatory (not nullable).

2. **EmailAddress**
   - **Type:** String
   - **Description:** The customer's email address. This field is optional (nullable).

3. **FirstName**
   - **Type:** String
   - **Description:** The first name of the customer, used to create the full name. This field is optional (nullable).

4. **LastName**
   - **Type:** String
   - **Description:** The last name of the customer, used to create the full name. This field is optional (nullable).

### Derived Fields
The following fields are calculated based on the input fields:

1. **Name**
   - **Type:** Calculated
   - **Description:** Identifier for the customers.
   - **Computation:** This field is computed by taking the customer's email address, replacing the "@" symbol with a "-" symbol. 
   - **Original Formula:** `=SUBSTITUTE({{EmailAddress}}, "@", "-")`
   - **Example:** For a customer with the email address `jane.smith@email.com`, the computed Name would be `jane.smith-email.com`.

2. **FullName**
   - **Type:** Calculated
   - **Description:** Full name is computed from the first and last name of the customer.
   - **Computation:** This field is created by concatenating the FirstName and LastName fields with a space in between.
   - **Original Formula:** `={{FirstName}} & " " & {{LastName}}`
   - **Example:** For a customer with FirstName as `Bob` and LastName as `Smith`, the computed FullName would be `Bob Smith`.

### Summary of Examples
- For the customer with `EmailAddress` `jane.smith@email.com`, the computed `Name` is `jane.smith-email.com` and `FullName` is `Bob Smith`.
- For the customer with `EmailAddress` `john.doe@email.com`, the computed `Name` is `john.doe-email.com` and `FullName` is `Jimmy Doe`.
- For the customer with `EmailAddress` `emily.jones@email.com`, the computed `Name` is `emily.jones-email.com` and `FullName` is `Mary Jones`.

This specification provides a clear guide to computing the derived fields in the Customers table of the ACME, LLC rulebook, ensuring accurate and consistent results based on the defined inputs.