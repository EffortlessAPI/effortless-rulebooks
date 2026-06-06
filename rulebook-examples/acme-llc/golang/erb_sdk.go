// ERB SDK - Go Implementation (GENERATED - DO NOT EDIT)
// ======================================================
// Generated from: effortless-rulebook/acme-llc-rulebook.json
//
// This file contains structs and calculation functions
// for all tables defined in the rulebook.

package main

import (
	"encoding/json"
	"fmt"
	"os"
	"strconv"
	"strings"
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
// CUSTOMERS TABLE
// Table: Customers
// =============================================================================

// Customer represents a row in the Customers table
// Table: Customers
type Customer struct {
	CustomerId string `json:"customer_id"`
	EmailAddress *string `json:"email_address"` // Thec ustomers email address
	FirstName *string `json:"first_name"` // First Name of the customer - used to make the full name
	LastName *string `json:"last_name"` // Last Name of the customer - used to make the full name
	Name *string `json:"name"` // Identifier for the cusfdsfdstomers.
	Initials *string `json:"initials"`
	FullName *string `json:"full_name"` // Full name is computed from the first and last name of the customer
}

// --- Individual Calculation Functions ---

// CalcName computes the Name calculated field
// Identifier for the cusfdsfdstomers.
// Formula: =SUBSTITUTE({{EmailAddress}}, "@", "-")
func (tc *Customer) CalcName() string {
	return strings.ReplaceAll(stringVal(tc.EmailAddress), "@", "-")
}

// CalcInitials computes the Initials calculated field
// Formula: =LEFT({{FirstName}}, 1) & LEFT({{LastName}}, 1)
func (tc *Customer) CalcInitials() string {
	return func() interface{} { panic("Formula parse error: Unknown function: LEFT") }()
}

// CalcFullName computes the FullName calculated field
// Full name is computed from the first and last name of the customer
// Formula: ={{FirstName}} & " " & {{LastName}}
func (tc *Customer) CalcFullName() string {
	return stringVal(tc.FirstName) + " " + stringVal(tc.LastName)
}

// --- Compute All Calculated Fields ---

// ComputeAll computes all calculated fields and returns an updated struct
func (tc *Customer) ComputeAll() *Customer {
	// Level 1 calculations
	name := strings.ReplaceAll(stringVal(tc.EmailAddress), "@", "-")
	initials := func() interface{} { panic("Formula parse error: Unknown function: LEFT") }()
	fullName := stringVal(tc.FirstName) + " " + stringVal(tc.LastName)

	return &Customer{
		CustomerId: tc.CustomerId,
		EmailAddress: tc.EmailAddress,
		FirstName: tc.FirstName,
		LastName: tc.LastName,
		Name: nilIfEmpty(name),
		Initials: nilIfEmpty(initials),
		FullName: nilIfEmpty(fullName),
	}
}

// =============================================================================
// ERBVERSIONS TABLE
// Table: ERBVersions
// =============================================================================

// ERBVersion represents a row in the ERBVersions table
// Table: ERBVersions
type ERBVersion struct {
	ERBVersionId string `json:"erb_version_id"`
	BaseId *string `json:"base_id"`
	Name *string `json:"name"`
	Message *string `json:"message"`
	Notes *string `json:"notes"`
	CommitDate *string `json:"commit_date"`
	IsPublished *bool `json:"is_published"`
}

// =============================================================================
// ERBCUSTOMIZATIONS TABLE
// Table: ERBCustomizations
// =============================================================================

// ERBCustomization represents a row in the ERBCustomizations table
// Table: ERBCustomizations
type ERBCustomization struct {
	ERBCustomizationId string `json:"erb_customization_id"`
	Name *string `json:"name"`
	Title *string `json:"title"`
	SQLCode *string `json:"sql_code"`
	SQLTarget *string `json:"sql_target"`
	CustomizationType *string `json:"customization_type"`
}

// =============================================================================
// __META__ TABLE
// Project-level metadata that travels with the rulebook: tagline, motif, narrative descriptions, substrate list, signature rows, etc. One row per metadata key. Use ValueType to interpret StringValue vs JsonValue.
// =============================================================================

// __meta__ represents a row in the __meta__ table
// Project-level metadata that travels with the rulebook: tagline, motif, narrative descriptions, substrate list, signature rows, etc. One row per metadata key. Use ValueType to interpret StringValue vs JsonValue.
type __meta__ struct {
	MetaKey string `json:"meta_key"` // The metadata key (e.g. 'tagline', 'motif_palette', 'substrates'). Unique within the table.
	ValueType string `json:"value_type"` // How to interpret the value columns: 'string' (use StringValue), 'object' (parse JsonValue as JSON object), 'array' (parse JsonValue as JSON array).
	StringValue *string `json:"string_value"` // Plain string value. Populated when ValueType == 'string'; null otherwise.
	JsonValue *string `json:"json_value"` // JSON-encoded value. Populated when ValueType == 'object' or 'array'; null when ValueType == 'string'.
	Name string `json:"name"` // Identifier for this metadata entry. Mirrors MetaKey so the row is addressable by Name like every other table.
}

// --- Individual Calculation Functions ---

// CalcName computes the Name calculated field
// Identifier for this metadata entry. Mirrors MetaKey so the row is addressable by Name like every other table.
// Formula: ={{MetaKey}}
func (tc *__meta__) CalcName() string {
	return tc.MetaKey
}

// --- Compute All Calculated Fields ---

// ComputeAll computes all calculated fields and returns an updated struct
func (tc *__meta__) ComputeAll() *__meta__ {
	// Level 1 calculations
	name := tc.MetaKey

	return &__meta__{
		MetaKey: tc.MetaKey,
		ValueType: tc.ValueType,
		StringValue: tc.StringValue,
		JsonValue: tc.JsonValue,
		Name: nilIfEmpty(name),
	}
}

// =============================================================================
// FILE I/O FUNCTIONS
// Load: all tables referenced by main.go (computed + lookup/aggregation targets)
// Save: only tables that have computed fields to write back
// =============================================================================

// LoadCustomerRecords loads Customers records from a JSON file
func LoadCustomerRecords(path string) ([]Customer, error) {
	data, err := os.ReadFile(path)
	if err != nil {
		return nil, fmt.Errorf("failed to read file: %w", err)
	}

	var records []Customer
	if err := json.Unmarshal(data, &records); err != nil {
		return nil, fmt.Errorf("failed to parse file: %w", err)
	}

	return records, nil
}

// SaveCustomerRecords saves computed Customers records to a JSON file
func SaveCustomerRecords(path string, records []Customer) error {
	data, err := json.MarshalIndent(records, "", "  ")
	if err != nil {
		return fmt.Errorf("failed to marshal records: %w", err)
	}

	if err := os.WriteFile(path, data, 0644); err != nil {
		return fmt.Errorf("failed to write records: %w", err)
	}

	return nil
}

// Load__meta__Records loads __meta__ records from a JSON file
func Load__meta__Records(path string) ([]__meta__, error) {
	data, err := os.ReadFile(path)
	if err != nil {
		return nil, fmt.Errorf("failed to read file: %w", err)
	}

	var records []__meta__
	if err := json.Unmarshal(data, &records); err != nil {
		return nil, fmt.Errorf("failed to parse file: %w", err)
	}

	return records, nil
}

// Save__meta__Records saves computed __meta__ records to a JSON file
func Save__meta__Records(path string, records []__meta__) error {
	data, err := json.MarshalIndent(records, "", "  ")
	if err != nil {
		return fmt.Errorf("failed to marshal records: %w", err)
	}

	if err := os.WriteFile(path, data, 0644); err != nil {
		return fmt.Errorf("failed to write records: %w", err)
	}

	return nil
}
