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

// =============================================================================
// WORKFLOWS TABLE
// =============================================================================

// Workflow represents a row in the Workflows table
type Workflow struct {
	WorkflowId string `json:"workflow_id"`
	DisplayName *string `json:"display_name"`
	Title *string `json:"title"`
	Description *string `json:"description"`
	Identifier *string `json:"identifier"`
	Modified *string `json:"modified"`
	WorkflowSteps *string `json:"workflow_steps"`
	CountOfSteps *int `json:"count_of_steps"`
	Name *string `json:"name"`
	HasMoreThan1Step *bool `json:"has_more_than1_step"`
}

// --- Individual Calculation Functions ---

// CalcName computes the Name calculated field
// Formula: =SUBSTITUTE(LOWER({{DisplayName}}), " ", "-")
func (tc *Workflow) CalcName() string {
	return strings.ReplaceAll(strings.ToLower(stringVal(tc.DisplayName)), " ", "-")
}

// CalcHasMoreThan1Step computes the HasMoreThan1Step calculated field
// Formula: ={{CountOfSteps}} > 1
func (tc *Workflow) CalcHasMoreThan1Step() bool {
	return (tc.CountOfSteps != nil && *tc.CountOfSteps > 1)
}

// --- Compute All Calculated Fields ---

// ComputeAll computes all calculated fields and returns an updated struct
func (tc *Workflow) ComputeAll() *Workflow {
	// Level 1 calculations
	name := strings.ReplaceAll(strings.ToLower(stringVal(tc.DisplayName)), " ", "-")
	hasMoreThan1Step := (tc.CountOfSteps != nil && *tc.CountOfSteps > 1)

	return &Workflow{
		WorkflowId: tc.WorkflowId,
		DisplayName: tc.DisplayName,
		Title: tc.Title,
		Description: tc.Description,
		Identifier: tc.Identifier,
		Modified: tc.Modified,
		WorkflowSteps: tc.WorkflowSteps,
		CountOfSteps: tc.CountOfSteps,
		Name: nilIfEmpty(name),
		HasMoreThan1Step: &hasMoreThan1Step,
	}
}

// =============================================================================
// WORKFLOWSTEPS TABLE
// =============================================================================

// WorkflowStep represents a row in the WorkflowSteps table
type WorkflowStep struct {
	WorkflowStepId string `json:"workflow_step_id"`
	DisplayName *string `json:"display_name"`
	Workflow *string `json:"workflow"`
	SequencePosition *int `json:"sequence_position"`
	AssignedRole *string `json:"assigned_role"`
	RequiresHumanApproval *bool `json:"requires_human_approval"`
	ApprovalGate *string `json:"approval_gate"`
	PrecededBySteps *string `json:"preceded_by_steps"`
	Name *string `json:"name"`
}

// --- Individual Calculation Functions ---

// CalcName computes the Name calculated field
// Formula: =SUBSTITUTE(LOWER({{DisplayName}}), " ", "-")
func (tc *WorkflowStep) CalcName() string {
	return strings.ReplaceAll(strings.ToLower(stringVal(tc.DisplayName)), " ", "-")
}

// --- Compute All Calculated Fields ---

// ComputeAll computes all calculated fields and returns an updated struct
func (tc *WorkflowStep) ComputeAll() *WorkflowStep {
	// Level 1 calculations
	name := strings.ReplaceAll(strings.ToLower(stringVal(tc.DisplayName)), " ", "-")

	return &WorkflowStep{
		WorkflowStepId: tc.WorkflowStepId,
		DisplayName: tc.DisplayName,
		Workflow: tc.Workflow,
		SequencePosition: tc.SequencePosition,
		AssignedRole: tc.AssignedRole,
		RequiresHumanApproval: tc.RequiresHumanApproval,
		ApprovalGate: tc.ApprovalGate,
		PrecededBySteps: tc.PrecededBySteps,
		Name: nilIfEmpty(name),
	}
}

// =============================================================================
// APPROVALGATES TABLE
// =============================================================================

// ApprovalGate represents a row in the ApprovalGates table
type ApprovalGate struct {
	ApprovalGateId string `json:"approval_gate_id"`
	DisplayName *string `json:"display_name"`
	WorkflowSteps *string `json:"workflow_steps"`
	EscalationThresholdHours *int `json:"escalation_threshold_hours"`
	Name *string `json:"name"`
}

// --- Individual Calculation Functions ---

// CalcName computes the Name calculated field
// Formula: =SUBSTITUTE(LOWER({{DisplayName}}), " ", "-")
func (tc *ApprovalGate) CalcName() string {
	return strings.ReplaceAll(strings.ToLower(stringVal(tc.DisplayName)), " ", "-")
}

// --- Compute All Calculated Fields ---

// ComputeAll computes all calculated fields and returns an updated struct
func (tc *ApprovalGate) ComputeAll() *ApprovalGate {
	// Level 1 calculations
	name := strings.ReplaceAll(strings.ToLower(stringVal(tc.DisplayName)), " ", "-")

	return &ApprovalGate{
		ApprovalGateId: tc.ApprovalGateId,
		DisplayName: tc.DisplayName,
		WorkflowSteps: tc.WorkflowSteps,
		EscalationThresholdHours: tc.EscalationThresholdHours,
		Name: nilIfEmpty(name),
	}
}

// =============================================================================
// PRECEDESSTEPS TABLE
// =============================================================================

// PrecedesStep represents a row in the PrecedesSteps table
type PrecedesStep struct {
	PrecedesStepId string `json:"precedes_step_id"`
	Name *string `json:"name"`
	StepNumber *int `json:"step_number"`
	WorkflowStep *string `json:"workflow_step"`
	DisplayName *string `json:"display_name"`
}

// --- Individual Calculation Functions ---

// CalcDisplayName computes the DisplayName calculated field
// Formula: ="Step-" & {{StepNumber}}
func (tc *PrecedesStep) CalcDisplayName() string {
	return "Step-" + intToString(tc.StepNumber)
}

// --- Compute All Calculated Fields ---

// ComputeAll computes all calculated fields and returns an updated struct
func (tc *PrecedesStep) ComputeAll() *PrecedesStep {
	// Level 1 calculations
	displayName := "Step-" + intToString(tc.StepNumber)

	return &PrecedesStep{
		PrecedesStepId: tc.PrecedesStepId,
		Name: tc.Name,
		StepNumber: tc.StepNumber,
		WorkflowStep: tc.WorkflowStep,
		DisplayName: nilIfEmpty(displayName),
	}
}

// =============================================================================
// ROLES TABLE
// =============================================================================

// Role represents a row in the Roles table
type Role struct {
	RoleId string `json:"role_id"`
	DisplayName *string `json:"display_name"`
	Label *string `json:"label"`
	Comment *string `json:"comment"`
	FilledByHumanAgent *string `json:"filled_by_human_agent"`
	FilledByAIAgent *string `json:"filled_by_ai_agent"`
	FilledByAutomatedPipeline *string `json:"filled_by_automated_pipeline"`
	OwnedBy *string `json:"owned_by"`
	DelegatesTo *string `json:"delegates_to"`
	WorkflowSteps *string `json:"workflow_steps"`
	FromDelegatesTo *string `json:"from_delegates_to"`
	Name *string `json:"name"`
}

// --- Individual Calculation Functions ---

// CalcName computes the Name calculated field
// Formula: =LOWER({{DisplayName}})
func (tc *Role) CalcName() string {
	return strings.ToLower(stringVal(tc.DisplayName))
}

// --- Compute All Calculated Fields ---

// ComputeAll computes all calculated fields and returns an updated struct
func (tc *Role) ComputeAll() *Role {
	// Level 1 calculations
	name := strings.ToLower(stringVal(tc.DisplayName))

	return &Role{
		RoleId: tc.RoleId,
		DisplayName: tc.DisplayName,
		Label: tc.Label,
		Comment: tc.Comment,
		FilledByHumanAgent: tc.FilledByHumanAgent,
		FilledByAIAgent: tc.FilledByAIAgent,
		FilledByAutomatedPipeline: tc.FilledByAutomatedPipeline,
		OwnedBy: tc.OwnedBy,
		DelegatesTo: tc.DelegatesTo,
		WorkflowSteps: tc.WorkflowSteps,
		FromDelegatesTo: tc.FromDelegatesTo,
		Name: nilIfEmpty(name),
	}
}

// =============================================================================
// DEPARTMENTS TABLE
// =============================================================================

// Department represents a row in the Departments table
type Department struct {
	DepartmentId string `json:"department_id"`
	Title *string `json:"title"`
	DisplayName *string `json:"display_name"`
	Roles *string `json:"roles"`
	Name *string `json:"name"`
}

// --- Individual Calculation Functions ---

// CalcName computes the Name calculated field
// Formula: =SUBSTITUTE(LOWER({{DisplayName}}), " ", "-")
func (tc *Department) CalcName() string {
	return strings.ReplaceAll(strings.ToLower(stringVal(tc.DisplayName)), " ", "-")
}

// --- Compute All Calculated Fields ---

// ComputeAll computes all calculated fields and returns an updated struct
func (tc *Department) ComputeAll() *Department {
	// Level 1 calculations
	name := strings.ReplaceAll(strings.ToLower(stringVal(tc.DisplayName)), " ", "-")

	return &Department{
		DepartmentId: tc.DepartmentId,
		Title: tc.Title,
		DisplayName: tc.DisplayName,
		Roles: tc.Roles,
		Name: nilIfEmpty(name),
	}
}

// =============================================================================
// HUMANAGENTS TABLE
// =============================================================================

// HumanAgent represents a row in the HumanAgents table
type HumanAgent struct {
	HumanAgentId string `json:"human_agent_id"`
	Name *string `json:"name"`
	DisplayName *string `json:"display_name"`
	Mbox *string `json:"mbox"`
	Roles *string `json:"roles"`
}

// =============================================================================
// AIAGENTS TABLE
// =============================================================================

// AIAgent represents a row in the AIAgents table
type AIAgent struct {
	AIAgentId string `json:"ai_agent_id"`
	Name *string `json:"name"`
	Title *string `json:"title"`
	DisplayName *string `json:"display_name"`
	ModelVersion *string `json:"model_version"`
	Roles *string `json:"roles"`
}

// =============================================================================
// AUTOMATEDPIPELINES TABLE
// =============================================================================

// AutomatedPipeline represents a row in the AutomatedPipelines table
type AutomatedPipeline struct {
	AutomatedPipelineId string `json:"automated_pipeline_id"`
	Name *string `json:"name"`
	Description *string `json:"description"`
	DisplayName *string `json:"display_name"`
	Roles *string `json:"roles"`
}

// =============================================================================
// FILE I/O FUNCTIONS (for all tables with calculated fields)
// =============================================================================

// LoadWorkflowRecords loads Workflows records from a JSON file
func LoadWorkflowRecords(path string) ([]Workflow, error) {
	data, err := os.ReadFile(path)
	if err != nil {
		return nil, fmt.Errorf("failed to read file: %w", err)
	}

	var records []Workflow
	if err := json.Unmarshal(data, &records); err != nil {
		return nil, fmt.Errorf("failed to parse file: %w", err)
	}

	return records, nil
}

// SaveWorkflowRecords saves computed Workflows records to a JSON file
func SaveWorkflowRecords(path string, records []Workflow) error {
	data, err := json.MarshalIndent(records, "", "  ")
	if err != nil {
		return fmt.Errorf("failed to marshal records: %w", err)
	}

	if err := os.WriteFile(path, data, 0644); err != nil {
		return fmt.Errorf("failed to write records: %w", err)
	}

	return nil
}

// LoadWorkflowStepRecords loads WorkflowSteps records from a JSON file
func LoadWorkflowStepRecords(path string) ([]WorkflowStep, error) {
	data, err := os.ReadFile(path)
	if err != nil {
		return nil, fmt.Errorf("failed to read file: %w", err)
	}

	var records []WorkflowStep
	if err := json.Unmarshal(data, &records); err != nil {
		return nil, fmt.Errorf("failed to parse file: %w", err)
	}

	return records, nil
}

// SaveWorkflowStepRecords saves computed WorkflowSteps records to a JSON file
func SaveWorkflowStepRecords(path string, records []WorkflowStep) error {
	data, err := json.MarshalIndent(records, "", "  ")
	if err != nil {
		return fmt.Errorf("failed to marshal records: %w", err)
	}

	if err := os.WriteFile(path, data, 0644); err != nil {
		return fmt.Errorf("failed to write records: %w", err)
	}

	return nil
}

// LoadApprovalGateRecords loads ApprovalGates records from a JSON file
func LoadApprovalGateRecords(path string) ([]ApprovalGate, error) {
	data, err := os.ReadFile(path)
	if err != nil {
		return nil, fmt.Errorf("failed to read file: %w", err)
	}

	var records []ApprovalGate
	if err := json.Unmarshal(data, &records); err != nil {
		return nil, fmt.Errorf("failed to parse file: %w", err)
	}

	return records, nil
}

// SaveApprovalGateRecords saves computed ApprovalGates records to a JSON file
func SaveApprovalGateRecords(path string, records []ApprovalGate) error {
	data, err := json.MarshalIndent(records, "", "  ")
	if err != nil {
		return fmt.Errorf("failed to marshal records: %w", err)
	}

	if err := os.WriteFile(path, data, 0644); err != nil {
		return fmt.Errorf("failed to write records: %w", err)
	}

	return nil
}

// LoadPrecedesStepRecords loads PrecedesSteps records from a JSON file
func LoadPrecedesStepRecords(path string) ([]PrecedesStep, error) {
	data, err := os.ReadFile(path)
	if err != nil {
		return nil, fmt.Errorf("failed to read file: %w", err)
	}

	var records []PrecedesStep
	if err := json.Unmarshal(data, &records); err != nil {
		return nil, fmt.Errorf("failed to parse file: %w", err)
	}

	return records, nil
}

// SavePrecedesStepRecords saves computed PrecedesSteps records to a JSON file
func SavePrecedesStepRecords(path string, records []PrecedesStep) error {
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

// LoadDepartmentRecords loads Departments records from a JSON file
func LoadDepartmentRecords(path string) ([]Department, error) {
	data, err := os.ReadFile(path)
	if err != nil {
		return nil, fmt.Errorf("failed to read file: %w", err)
	}

	var records []Department
	if err := json.Unmarshal(data, &records); err != nil {
		return nil, fmt.Errorf("failed to parse file: %w", err)
	}

	return records, nil
}

// SaveDepartmentRecords saves computed Departments records to a JSON file
func SaveDepartmentRecords(path string, records []Department) error {
	data, err := json.MarshalIndent(records, "", "  ")
	if err != nil {
		return fmt.Errorf("failed to marshal records: %w", err)
	}

	if err := os.WriteFile(path, data, 0644); err != nil {
		return fmt.Errorf("failed to write records: %w", err)
	}

	return nil
}
