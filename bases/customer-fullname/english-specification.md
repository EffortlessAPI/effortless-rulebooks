# Specification Document for DEMO: Customer FullName Rulebook

## Overview
This specification document outlines the rules and calculations defined in the "DEMO: Customer FullName" rulebook. The rulebook is generated from an Airtable base and provides a schema for customer data, including fields for customer identification, email addresses, and names. The primary focus is on how to compute the `FullName` field based on the provided first and last names of customers.

## Customers Table

### Input Fields
The following input fields are defined in the Customers table, which are of type "raw":

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

### Calculated Field
The following calculated field is defined in the Customers table:

1. **FullName**
   - **Type:** String
   - **Description:** The full name is computed by concatenating the first and last names of the customer.
   - **Calculation Explanation:** 
     - To compute the `FullName`, take the value from the `FirstName` field and concatenate it with a space and the value from the `LastName` field. 
     - If either `FirstName` or `LastName` is null, the resulting `FullName` will reflect that by omitting the missing part.
   - **Formula for Reference:** `={{FirstName}} & " " & {{LastName}}`
   - **Concrete Example:**
     - For a customer with the following data:
       - `FirstName`: "Jane"
       - `LastName`: "Smith"
     - The computed `FullName` would be:
       - **Result:** "Jane Smith"
     - For a customer with:
       - `FirstName`: "John"
       - `LastName`: null
     - The computed `FullName` would be:
       - **Result:** "John" (the last name is omitted)

This specification provides a clear understanding of how to compute the `FullName` field using the `FirstName` and `LastName` fields, ensuring that the computation can be replicated accurately based on the provided data.