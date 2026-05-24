# ACME, LLC Rulebook Specification Document

## Overview
This document outlines the specifications for the ACME, LLC rulebook, which is generated from an Airtable base. It describes the structure of the data, including input fields and derived fields, along with detailed instructions on how to compute each derived field. The rulebook consists of three main entities: Customers, ERBVersions, and ERBCustomizations.

---

## 1. Customers

### Input Fields
- **CustomerId**
  - **Type:** string
  - **Description:** Unique identifier for the customer. This field is mandatory.
  
- **EmailAddress**
  - **Type:** string
  - **Description:** The customer's email address. This field is optional.
  
- **FirstName**
  - **Type:** string
  - **Description:** The first name of the customer. This field is optional.
  
- **LastName**
  - **Type:** string
  - **Description:** The last name of the customer. This field is optional.

### Derived Fields
- **Name**
  - **Type:** calculated
  - **Description:** This field serves as an identifier for the customers, derived from the EmailAddress.
  - **Computation:** Replace the "@" symbol in the EmailAddress with a hyphen ("-"). 
  - **Original Formula:** `=SUBSTITUTE({{EmailAddress}}, "@", "-")`
  - **Example:** For the email `jane.smith@email.com`, the Name would be `jane.smith-email.com`.

- **FullName**
  - **Type:** calculated
  - **Description:** This field represents the full name of the customer, combining the LastName and FirstName.
  - **Computation:** Concatenate the LastName, a comma, a space, and the FirstName.
  - **Original Formula:** `={{LastName}} & ", " & {{FirstName}}`
  - **Example:** For LastName `Smith` and FirstName `Bobby`, the FullName would be `Smith, Bobby`.

---

## 2. ERBVersions

### Input Fields
- **ERBVersionId**
  - **Type:** string
  - **Description:** Unique identifier for the ERB version. This field is mandatory.
  
- **BaseId**
  - **Type:** string
  - **Description:** Identifier for the base. This field is optional.
  
- **Name**
  - **Type:** string
  - **Description:** Name of the ERB version. This field is optional.
  
- **Message**
  - **Type:** string
  - **Description:** Message associated with the ERB version. This field is optional.
  
- **Notes**
  - **Type:** string
  - **Description:** Additional notes for the ERB version. This field is optional.
  
- **CommitDate**
  - **Type:** datetime
  - **Description:** Date when the ERB version was committed. This field is optional.
  
- **IsPublished**
  - **Type:** boolean
  - **Description:** Indicates whether the ERB version is published. This field is optional.

### Derived Fields
- **No derived fields are specified for the ERBVersions entity.**

---

## 3. ERBCustomizations

### Input Fields
- **ERBCustomizationId**
  - **Type:** string
  - **Description:** Unique identifier for the ERB customization. This field is mandatory.
  
- **Name**
  - **Type:** string
  - **Description:** Name of the ERB customization. This field is optional.
  
- **Title**
  - **Type:** string
  - **Description:** Title of the ERB customization. This field is optional.
  
- **SQLCode**
  - **Type:** string
  - **Description:** SQL code associated with the ERB customization. This field is optional.
  
- **SQLTarget**
  - **Type:** string
  - **Description:** Target database for the SQL code. This field is optional.
  
- **CustomizationType**
  - **Type:** string
  - **Description:** Type of customization (e.g., Schema, Functions, Views, RLS, Data). This field is optional.

### Derived Fields
- **No derived fields are specified for the ERBCustomizations entity.**

---

This specification provides a comprehensive guide for computing derived fields within the ACME, LLC rulebook. Each derived field's computation is clearly defined, allowing for accurate value generation based on the input fields.