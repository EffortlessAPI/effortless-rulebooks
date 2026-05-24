# Specification Document for DEMO: Customer FullName Rulebook

## Overview
This document provides a detailed specification for the rulebook generated from the Airtable base titled "DEMO: Customer FullName." The rulebook defines the structure and computation of customer data, including how to derive the full name of each customer based on their first and last names.

## Entities

### Customers
**Description**: This entity represents the customers in the system.

#### Input Fields
1. **CustomerId**
   - **Type**: String
   - **Description**: A unique identifier for each customer. This field is mandatory and cannot be null.

2. **Customer**
   - **Type**: String
   - **Description**: An identifier for the customers. This field is optional and can be null.

3. **EmailAddress**
   - **Type**: String
   - **Description**: The customer's email address. This field is optional and can be null.

4. **FirstName**
   - **Type**: String
   - **Description**: The first name of the customer, used to construct the full name. This field is optional and can be null.

5. **LastName**
   - **Type**: String
   - **Description**: The last name of the customer, used to construct the full name. This field is optional and can be null.

#### Derived Fields
1. **FullName**
   - **Type**: Calculated
   - **Description**: The full name of the customer is computed by combining the last name and first name.
   - **Computation**: The full name is derived by concatenating the `LastName` and `FirstName` fields, formatted as "LastName, FirstName". If either `FirstName` or `LastName` is null, the resulting `FullName` will also be null.
   - **Original Formula**: `={{LastName}} & ", " & {{FirstName}}`
   - **Example**: 
     - For a customer with `FirstName` = "Jane" and `LastName` = "Smith", the `FullName` will be computed as "Smith, Jane".
     - For a customer with `FirstName` = "John" and `LastName` = "Doe", the `FullName` will be computed as "Doe, John".
     - For a customer with `FirstName` = null and `LastName` = "Jones", the `FullName` will be null.

### Summary of Example Data
- Customer with `CustomerId` "cust0001":
  - **FirstName**: "Jane"
  - **LastName**: "Smith"
  - **FullName**: "Smith, Jane"
  
- Customer with `CustomerId` "cust0002":
  - **FirstName**: "John"
  - **LastName**: "Doe"
  - **FullName**: "Doe, John"
  
- Customer with `CustomerId` "cust0003":
  - **FirstName**: "Emily"
  - **LastName**: "Jones"
  - **FullName**: "Jones, Emily"

This specification document provides a clear understanding of how to derive the `FullName` field from the `FirstName` and `LastName` fields within the Customers entity, ensuring accurate computation of customer full names based on the provided data.