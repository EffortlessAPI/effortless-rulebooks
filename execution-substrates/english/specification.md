# Specification Document for DEMO: Customer FullName Rulebook

## Overview
This specification document outlines the structure and calculation methods used in the "DEMO: Customer FullName" rulebook, derived from an Airtable base. The primary purpose of this rulebook is to compute the full names of customers based on their first and last names.

## Entity: Customers

### Input Fields
The following input fields are utilized to compute the calculated fields:

1. **CustomerId**
   - **Type:** String
   - **Description:** Unique identifier for each customer. This field is mandatory and cannot be null.

2. **Customer**
   - **Type:** String
   - **Description:** Identifier for the customers. This field is optional and can be null.

3. **EmailAddress**
   - **Type:** String
   - **Description:** The customer's email address. This field is optional and can be null.

4. **FirstName**
   - **Type:** String
   - **Description:** The first name of the customer, used to create the full name. This field is optional and can be null.

5. **LastName**
   - **Type:** String
   - **Description:** The last name of the customer, used to create the full name. This field is optional and can be null.

### Calculated Field

#### FullName
- **Type:** Calculated
- **Description:** The full name is computed from the first and last name of the customer.
- **Calculation Method:** 
  To compute the `FullName`, concatenate the `LastName` and `FirstName` fields, separated by a comma and a space. The format will be: 
  ```
  LastName, FirstName
  ```
- **Formula for Reference:** 
  ```
  ={{LastName}} & ", " & {{FirstName}}
  ```

#### Example Calculation
Using the data provided in the rulebook, the following examples illustrate how to compute the `FullName`:

1. **Customer Data:**
   - **FirstName:** Jane
   - **LastName:** Smith
   - **Computed FullName:** 
     ```
     Smith, Jane
     ```

2. **Customer Data:**
   - **FirstName:** John
   - **LastName:** Doe
   - **Computed FullName:** 
     ```
     Doe, John
     ```

3. **Customer Data:**
   - **FirstName:** Emily
   - **LastName:** Jones
   - **Computed FullName:** 
     ```
     Jones, Emily
     ```

By following the above instructions, one can accurately compute the `FullName` for each customer based on their first and last names.