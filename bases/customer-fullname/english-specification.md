# Specification Document for DEMO: Customer FullName Rulebook

## Overview
This specification document outlines the structure and computation methods for the "DEMO: Customer FullName" rulebook, which is derived from an Airtable base. The primary purpose of this rulebook is to calculate the full names of customers based on their first and last names. The rulebook includes a schema defining the data types and relationships, along with the necessary formulas for calculated fields.

## Entity: Customers

### Input Fields
The following input fields are defined in the "Customers" table, which are used to compute the calculated fields:

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
   - **Description:** First name of the customer, used to create the full name. This field is optional and can be null.

5. **LastName**
   - **Type:** String
   - **Description:** Last name of the customer, used to create the full name. This field is optional and can be null.

### Calculated Fields

#### FullName
- **Type:** Calculated
- **Description:** The full name is computed by concatenating the first name and last name of the customer, separated by a space.
- **Computation Method:** 
  To compute the `FullName`, take the value from the `FirstName` field and the value from the `LastName` field. If either the `FirstName` or `LastName` is null, the resulting `FullName` will only include the non-null value. If both are null, the `FullName` will also be null.

- **Formula for Reference:** 
  ```
  ={{FirstName}} & " " & {{LastName}}
  ```

- **Concrete Examples:**
  1. For the customer with `CustomerId` "cust0001":
     - `FirstName`: "Jannice"
     - `LastName`: "Smith"
     - **Computed FullName:** "Jannice Smith"

  2. For the customer with `CustomerId` "cust0002":
     - `FirstName`: "John"
     - `LastName`: "Doe"
     - **Computed FullName:** "John Doe"

  3. For the customer with `CustomerId` "cust0003":
     - `FirstName`: "Emily"
     - `LastName`: "Jones"
     - **Computed FullName:** "Emily Jones"

This specification provides a clear understanding of how to compute the `FullName` field based on the provided input fields in the "Customers" table. By following the outlined method, one can accurately derive the full names of customers as intended in the rulebook.