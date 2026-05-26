# ACME, LLC Rulebook Specification

## Overview
This rulebook defines the structure and calculations for customer data management at ACME, LLC. It includes a schema for customers, ERB versions, and ERB customizations, with specific calculated fields for customer identifiers and full names. This document outlines how to compute each derived field based on the raw input fields.

---

## Customers Table

### Input Fields
1. **CustomerId**
   - **Type:** string
   - **Description:** Unique identifier for the customer. This field is mandatory and cannot be null.

2. **EmailAddress**
   - **Type:** string
   - **Description:** The customer's email address. This field is optional and can be null.

3. **FirstName**
   - **Type:** string
   - **Description:** The first name of the customer, used to create the full name. This field is optional and can be null.

4. **LastName**
   - **Type:** string
   - **Description:** The last name of the customer, used to create the full name. This field is optional and can be null.

### Calculated Fields
1. **Name**
   - **Type:** calculated
   - **Description:** Identifier for the customers, derived from their email address.
   - **Computation:** The Name is computed by replacing the "@" symbol in the EmailAddress with a hyphen ("-"). 
   - **Formula:** `=SUBSTITUTE({{EmailAddress}}, "@", "-")`
   - **Example:** For a customer with the EmailAddress `jane.smith@email.com`, the Name would be computed as `jane.smith-email.com`.

2. **FullName**
   - **Type:** calculated
   - **Description:** The full name of the customer, constructed from their first and last names.
   - **Computation:** The FullName is created by concatenating the LastName, a comma and space, and the FirstName.
   - **Formula:** `={{LastName}} & ", " & {{FirstName}}`
   - **Example:** For a customer with FirstName `Bobby` and LastName `Smith`, the FullName would be computed as `Smith, Bobby`.

---

## ERBVersions Table

### Input Fields
1. **ERBVersionId**
   - **Type:** string
   - **Description:** Unique identifier for the ERB version. This field is mandatory and cannot be null.

2. **BaseId**
   - **Type:** string
   - **Description:** Identifier for the base version. This field is optional and can be null.

3. **Name**
   - **Type:** string
   - **Description:** Name of the ERB version. This field is optional and can be null.

4. **Message**
   - **Type:** string
   - **Description:** Message associated with the ERB version. This field is optional and can be null.

5. **Notes**
   - **Type:** string
   - **Description:** Additional notes regarding the ERB version. This field is optional and can be null.

6. **CommitDate**
   - **Type:** datetime
   - **Description:** Date and time of the commit for this ERB version. This field is optional and can be null.

7. **IsPublished**
   - **Type:** boolean
   - **Description:** Indicates whether the ERB version is published. This field is optional and can be null.

### Calculated Fields
- **No calculated fields are defined in the ERBVersions table.**

---

## ERBCustomizations Table

### Input Fields
1. **ERBCustomizationId**
   - **Type:** string
   - **Description:** Unique identifier for the ERB customization. This field is mandatory and cannot be null.

2. **Name**
   - **Type:** string
   - **Description:** Name of the ERB customization. This field is optional and can be null.

3. **Title**
   - **Type:** string
   - **Description:** Title of the ERB customization. This field is optional and can be null.

4. **SQLCode**
   - **Type:** string
   - **Description:** SQL code associated with the customization. This field is optional and can be null.

5. **SQLTarget**
   - **Type:** string
   - **Description:** Target database for the SQL code. This field is optional and can be null.

6. **CustomizationType**
   - **Type:** string
   - **Description:** Type of customization (e.g., Schema, Functions, Views, RLS, Data). This field is optional and can be null.

### Calculated Fields
- **No calculated fields are defined in the ERBCustomizations table.**

---

This specification document provides a clear understanding of how to compute derived fields in the ACME, LLC rulebook, ensuring accurate data representation and management.