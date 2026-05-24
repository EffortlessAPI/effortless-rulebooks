// ERB SDK - Go Implementation (GENERATED - DO NOT EDIT)
// ======================================================
// Generated from: effortless-rulebook/effortless-rulebook.json
//
// This file contains structs and calculation functions
// for all tables defined in the rulebook.

package main

import (
	"encoding/json"
	"fmt"
	"os"
	"strconv"
)

// =============================================================================
// HELPER FUNCTIONS
// =============================================================================

// boolVal safely dereferences a *bool, returning false if nil
func boolVal(b *bool) bool {
	if b == nil {
		return false
	}
	return *b
}

// stringVal safely dereferences a *string, returning "" if nil
func stringVal(s *string) string {
	if s == nil {
		return ""
	}
	return *s
}

// nilIfEmpty returns nil for empty strings, otherwise a pointer to the string
func nilIfEmpty(s string) *string {
	if s == "" {
		return nil
	}
	return &s
}

// intToString safely converts a *int to string, returning "" if nil
func intToString(i *int) string {
	if i == nil {
		return ""
	}
	return strconv.Itoa(*i)
}

// boolToString converts a bool to "true" or "false"
func boolToString(b bool) string {
	if b {
		return "true"
	}
	return "false"
}

// FlexibleString is a type that can unmarshal from both string and number JSON values
// This is needed for aggregation fields that return 0 (int) when empty or string values
type FlexibleString string

func (f *FlexibleString) UnmarshalJSON(data []byte) error {
	// First try as string
	var s string
	if err := json.Unmarshal(data, &s); err == nil {
		*f = FlexibleString(s)
		return nil
	}
	// Try as number
	var n float64
	if err := json.Unmarshal(data, &n); err == nil {
		// Convert number to string, but treat 0 as empty
		if n == 0 {
			*f = FlexibleString("0")
		} else {
			*f = FlexibleString(fmt.Sprintf("%v", n))
		}
		return nil
	}
	return fmt.Errorf("cannot unmarshal %s into FlexibleString", string(data))
}

// String returns the underlying string value
func (f FlexibleString) String() string {
	return string(f)
}

// =============================================================================
// CLIENT TABLE
// Table: Client
// =============================================================================

// Client represents a row in the Client table
// Table: Client
type Client struct {
	ClientId string `json:"client_id"`
	Customer *string `json:"customer"` // Identifier for the customers.
	EmailAddress *string `json:"email_address"` // Thec ustomers email address
	FirstName *string `json:"first_name"` // First Name of the customer - used to make the full name
	LastName *string `json:"last_name"` // Last Name of the customer - used to make the full name
	FullName *string `json:"full_name"` // Full name is computed from the first and last name of the customer
}

// --- Individual Calculation Functions ---

// CalcFullName computes the FullName calculated field
// Full name is computed from the first and last name of the customer
// Formula: ={{FirstName}} & " " & {{LastName}}
func (tc *Client) CalcFullName() string {
	return stringVal(tc.FirstName) + " " + stringVal(tc.LastName)
}

// --- Compute All Calculated Fields ---

// ComputeAll computes all calculated fields and returns an updated struct
func (tc *Client) ComputeAll() *Client {
	// Level 1 calculations
	fullName := stringVal(tc.FirstName) + " " + stringVal(tc.LastName)

	return &Client{
		ClientId: tc.ClientId,
		Customer: tc.Customer,
		EmailAddress: tc.EmailAddress,
		FirstName: tc.FirstName,
		LastName: tc.LastName,
		FullName: nilIfEmpty(fullName),
	}
}

// =============================================================================
// PROJECTS TABLE
// Table: Projects
// =============================================================================

// Project represents a row in the Projects table
// Table: Projects
type Project struct {
	ProjectId string `json:"project_id"`
	Name *string `json:"name"`
	Description *string `json:"description"`
	ProjectType *string `json:"project_type"`
	ProjectTypeName *string `json:"project_type_name"`
	ProjectTypeDescription *string `json:"project_type_description"`
	ProjectTypeRequiresManagerApproval *bool `json:"project_type_requires_manager_approval"`
	DueDate *string `json:"due_date"`
	IsApproved *bool `json:"is_approved"`
	ApprovedBy *string `json:"approved_by"`
	ApprovedByRoleIsManager *bool `json:"approved_by_role_is_manager"`
	ApprovedByName *string `json:"approved_by_name"`
	ApprovedByEmailAddress *string `json:"approved_by_email_address"`
	ApprovedByPhoneNumber *string `json:"approved_by_phone_number"`
}

// =============================================================================
// EMPLOYEES TABLE
// Table: Employees
// =============================================================================

// Employee represents a row in the Employees table
// Table: Employees
type Employee struct {
	EmployeeId string `json:"employee_id"`
	Name *string `json:"name"`
	EmailAddress *string `json:"email_address"`
	PhoneNumber *string `json:"phone_number"`
	Role *string `json:"role"`
	RoleName *string `json:"role_name"`
	RoleDescription *string `json:"role_description"`
	Projects *string `json:"projects"`
	RoleIsManager *bool `json:"role_is_manager"`
}

// =============================================================================
// ROLES TABLE
// Table: Roles
// =============================================================================

// Role represents a row in the Roles table
// Table: Roles
type Role struct {
	RoleId string `json:"role_id"`
	Name *string `json:"name"`
	Description *string `json:"description"`
	IsManager *bool `json:"is_manager"`
	Employees *string `json:"employees"`
	CountOfEmployees *int `json:"count_of_employees"`
}

// =============================================================================
// TYPESOFPROJECT TABLE
// Table: TypesOfProject
// =============================================================================

// TypesOfProject represents a row in the TypesOfProject table
// Table: TypesOfProject
type TypesOfProject struct {
	TypesOfProjectId string `json:"types_of_project_id"`
	Name *string `json:"name"`
	Description *string `json:"description"`
	RequiresManagerApproval *bool `json:"requires_manager_approval"`
	Projects *string `json:"projects"`
	CountOfProjects *int `json:"count_of_projects"`
}

// =============================================================================
// FILE I/O FUNCTIONS
// Load: all tables referenced by main.go (computed + lookup/aggregation targets)
// Save: only tables that have computed fields to write back
// =============================================================================

// LoadClientRecords loads Client records from a JSON file
func LoadClientRecords(path string) ([]Client, error) {
	data, err := os.ReadFile(path)
	if err != nil {
		return nil, fmt.Errorf("failed to read file: %w", err)
	}

	var records []Client
	if err := json.Unmarshal(data, &records); err != nil {
		return nil, fmt.Errorf("failed to parse file: %w", err)
	}

	return records, nil
}

// SaveClientRecords saves computed Client records to a JSON file
func SaveClientRecords(path string, records []Client) error {
	data, err := json.MarshalIndent(records, "", "  ")
	if err != nil {
		return fmt.Errorf("failed to marshal records: %w", err)
	}

	if err := os.WriteFile(path, data, 0644); err != nil {
		return fmt.Errorf("failed to write records: %w", err)
	}

	return nil
}

// LoadProjectRecords loads Projects records from a JSON file
func LoadProjectRecords(path string) ([]Project, error) {
	data, err := os.ReadFile(path)
	if err != nil {
		return nil, fmt.Errorf("failed to read file: %w", err)
	}

	var records []Project
	if err := json.Unmarshal(data, &records); err != nil {
		return nil, fmt.Errorf("failed to parse file: %w", err)
	}

	return records, nil
}

// SaveProjectRecords saves computed Projects records to a JSON file
func SaveProjectRecords(path string, records []Project) error {
	data, err := json.MarshalIndent(records, "", "  ")
	if err != nil {
		return fmt.Errorf("failed to marshal records: %w", err)
	}

	if err := os.WriteFile(path, data, 0644); err != nil {
		return fmt.Errorf("failed to write records: %w", err)
	}

	return nil
}

// LoadEmployeeRecords loads Employees records from a JSON file
func LoadEmployeeRecords(path string) ([]Employee, error) {
	data, err := os.ReadFile(path)
	if err != nil {
		return nil, fmt.Errorf("failed to read file: %w", err)
	}

	var records []Employee
	if err := json.Unmarshal(data, &records); err != nil {
		return nil, fmt.Errorf("failed to parse file: %w", err)
	}

	return records, nil
}

// SaveEmployeeRecords saves computed Employees records to a JSON file
func SaveEmployeeRecords(path string, records []Employee) error {
	data, err := json.MarshalIndent(records, "", "  ")
	if err != nil {
		return fmt.Errorf("failed to marshal records: %w", err)
	}

	if err := os.WriteFile(path, data, 0644); err != nil {
		return fmt.Errorf("failed to write records: %w", err)
	}

	return nil
}

// LoadRoleRecords loads Roles records from a JSON file
func LoadRoleRecords(path string) ([]Role, error) {
	data, err := os.ReadFile(path)
	if err != nil {
		return nil, fmt.Errorf("failed to read file: %w", err)
	}

	var records []Role
	if err := json.Unmarshal(data, &records); err != nil {
		return nil, fmt.Errorf("failed to parse file: %w", err)
	}

	return records, nil
}

// SaveRoleRecords saves computed Roles records to a JSON file
func SaveRoleRecords(path string, records []Role) error {
	data, err := json.MarshalIndent(records, "", "  ")
	if err != nil {
		return fmt.Errorf("failed to marshal records: %w", err)
	}

	if err := os.WriteFile(path, data, 0644); err != nil {
		return fmt.Errorf("failed to write records: %w", err)
	}

	return nil
}

// LoadTypesOfProjectRecords loads TypesOfProject records from a JSON file
func LoadTypesOfProjectRecords(path string) ([]TypesOfProject, error) {
	data, err := os.ReadFile(path)
	if err != nil {
		return nil, fmt.Errorf("failed to read file: %w", err)
	}

	var records []TypesOfProject
	if err := json.Unmarshal(data, &records); err != nil {
		return nil, fmt.Errorf("failed to parse file: %w", err)
	}

	return records, nil
}

// SaveTypesOfProjectRecords saves computed TypesOfProject records to a JSON file
func SaveTypesOfProjectRecords(path string, records []TypesOfProject) error {
	data, err := json.MarshalIndent(records, "", "  ")
	if err != nil {
		return fmt.Errorf("failed to marshal records: %w", err)
	}

	if err := os.WriteFile(path, data, 0644); err != nil {
		return fmt.Errorf("failed to write records: %w", err)
	}

	return nil
}
