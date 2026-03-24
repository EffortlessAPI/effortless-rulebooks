# ACME Corporation Rulebook Specification

## Overview
This specification document outlines the rules and calculations defined in the ACME Corporation rulebook. The rulebook is generated from an Airtable base and contains a schema for customer data, including raw input fields and calculated fields. The primary focus is on how to compute the calculated fields based on the provided raw data.

## Customers Table

### Input Fields
The following input fields are defined in the Customers table:

1. **CustomerId**
   - **Type:** String
   - **Description:** A unique identifier for the customer. This field is mandatory.

2. **Customer**
   - **Type:** String
   - **Description:** An identifier for the customers. This field is optional.

3. **EmailAddress**
   - **Type:** String
   - **Description:** The customer's email address. This field is optional.

4. **FirstName**
   - **Type:** String
   - **Description:** The first name of the customer, used to create the full name. This field is optional.

5. **LastName**
   - **Type:** String
   - **Description:** The last name of the customer, used to create the full name. This field is optional.

### Calculated Fields

#### FullName
- **Type:** Calculated
- **Description:** The full name of the customer is computed by combining the last name and first name in the format "LastName, FirstName".

**Calculation Method:**
To compute the FullName, concatenate the LastName and FirstName fields with a comma and a space in between. If either the LastName or FirstName is missing, the output should reflect that accordingly.

**Formula:**
```
={{LastName}} & ", " & {{FirstName}}
```

**Example Calculation:**
Using the provided data:

- For the first customer:
  - **LastName:** Smith
  - **FirstName:** Larry
  - **FullName Calculation:** "Smith, Larry"

- For the second customer:
  - **LastName:** Doe
  - **FirstName:** John
  - **FullName Calculation:** "Doe, John"

- For the third customer:
  - **LastName:** Jones
  - **FirstName:** Emily
  - **FullName Calculation:** "Jones, Emily"

This specification provides a clear understanding of how to compute the FullName field based on the raw input fields available in the Customers table of the ACME Corporation rulebook.