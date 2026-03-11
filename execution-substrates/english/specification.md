# Specification Document for DEMO: Customer FullName Rulebook

## Overview
This document outlines the specifications for calculating fields within the "DEMO: Customer FullName" rulebook, which is derived from an Airtable base. The primary focus is on computing the full name of customers based on their first and last names. 

## Customers Table

### Input Fields
The following input fields are used to compute the calculated fields in the Customers table:

1. **CustomerId**
   - **Type:** String
   - **Description:** A unique identifier for each customer. This field is mandatory.

2. **Customer**
   - **Type:** String
   - **Description:** An identifier for the customers. This field is optional.

3. **EmailAddress**
   - **Type:** String
   - **Description:** The customer's email address. This field is optional.

4. **FirstName**
   - **Type:** String
   - **Description:** The first name of the customer, used in the computation of the full name. This field is optional.

5. **LastName**
   - **Type:** String
   - **Description:** The last name of the customer, used in the computation of the full name. This field is optional.

### Calculated Fields

#### FullName
- **Type:** Calculated
- **Description:** The full name is computed by combining the last name and first name of the customer in the format "LastName, FirstName".
- **Computation Method:**
  To compute the `FullName`, concatenate the `LastName` and `FirstName` fields with a comma and a space in between. If either the `FirstName` or `LastName` is missing, the result will be formatted accordingly, but the full name will still follow the same structure.
  
- **Formula for Reference:**
  ```
  ={{LastName}} & ", " & {{FirstName}}
  ```

- **Concrete Examples:**
  1. For the customer with `CustomerId` "cust0001":
     - **FirstName:** Jane
     - **LastName:** Smith
     - **Computed FullName:** "Smith, Jane"

  2. For the customer with `CustomerId` "cust0002":
     - **FirstName:** John
     - **LastName:** Doe
     - **Computed FullName:** "Doe, John"

  3. For the customer with `CustomerId` "cust0003":
     - **FirstName:** Emily
     - **LastName:** Jones
     - **Computed FullName:** "Jones, Emily"

This specification provides a clear guide on how to compute the `FullName` field using the `FirstName` and `LastName` inputs. By following the outlined method, one can accurately derive the full names of customers as specified in the rulebook.