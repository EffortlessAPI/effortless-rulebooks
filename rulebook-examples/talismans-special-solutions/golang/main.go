// ERB SDK - Go Test Runner (GENERATED - DO NOT EDIT)
// =======================================================
// This file is REGENERATED every time inject-into-golang.py runs.
// It must stay in sync with erb_sdk.go and the rulebook.
//
// Tables with computed fields: Workflows, WorkflowSteps, ApprovalGates, StepPrecedence, Roles, RoleAssignments, Departments, HumanAgents, AIAgents, AutomatedPipelines, WorkflowStatusConcepts, AgentCapabilityConcepts, ArtifactTypeConcepts, Datasets, WorkflowArtifacts, GovernanceRoles, ChangeLog, VocabularyReconciliations, Scenarios, CompetencyQuestions, ScenarioCQEffects, ConformanceTests, __meta__
// Tables with aggregations: Workflows, WorkflowSteps, AIAgents
//
// IMPORTANT: This runner processes ALL tables, not just a "primary" one.
// If ANY table fails to process, the entire run fails with exit code 1.

package main

import (
	"fmt"
	"os"
	"path/filepath"
	"sort"
)

func main() {
	// ERB_TESTING_DIR is required — defaulting to the repo testing dir
	// silently uses the wrong domain.
	erbTesting := os.Getenv("ERB_TESTING_DIR")
	if erbTesting == "" {
		fmt.Fprintln(os.Stderr, "FATAL: ERB_TESTING_DIR is not set. main.go must be")
		fmt.Fprintln(os.Stderr, "  invoked by the orchestrator with ERB_TESTING_DIR pointing")
		fmt.Fprintln(os.Stderr, "  at the active domain\u0027s testing/ directory.")
		os.Exit(1)
	}
	blankTestsDir := filepath.Join(erbTesting, "blank-tests")
	testAnswersDir := filepath.Join(erbTesting, "golang", "test-answers")

	// Ensure output directory exists
	if err := os.MkdirAll(testAnswersDir, 0755); err != nil {
		fmt.Fprintf(os.Stderr, "FATAL: Failed to create test-answers directory: %v\n", err)
		os.Exit(1)
	}

	fmt.Println("Golang substrate: Processing 23 tables with calculated fields...")
	fmt.Println("  Expected tables: Workflows, WorkflowSteps, ApprovalGates, StepPrecedence, Roles, RoleAssignments, Departments, HumanAgents, AIAgents, AutomatedPipelines, WorkflowStatusConcepts, AgentCapabilityConcepts, ArtifactTypeConcepts, Datasets, WorkflowArtifacts, GovernanceRoles, ChangeLog, VocabularyReconciliations, Scenarios, CompetencyQuestions, ScenarioCQEffects, ConformanceTests, __meta__")
	fmt.Println("")

	// Track success/failure for ALL tables
	var errors []string
	var totalRecords int

	// ─────────────────────────────────────────────────────────────────
	// Load related tables for aggregation calculations
	// ─────────────────────────────────────────────────────────────────
	// Note: SUMIFS loads from answer-keys (has computed fields)
	//       COUNTIFS loads from blank-tests

	workflow_artifactsData, err := LoadWorkflowArtifactRecords(filepath.Join(blankTestsDir, "workflow_artifacts.json"))
	if err != nil {
		fmt.Fprintf(os.Stderr, "Warning: Could not load WorkflowArtifacts for aggregations: %v\n", err)
		workflow_artifactsData = nil
	}
	workflow_stepsData, err := LoadWorkflowStepRecords(filepath.Join(blankTestsDir, "workflow_steps.json"))
	if err != nil {
		fmt.Fprintf(os.Stderr, "Warning: Could not load WorkflowSteps for aggregations: %v\n", err)
		workflow_stepsData = nil
	}
	// Sort workflow_steps by sequence_position for proper ordering
	if workflow_stepsData != nil {
		sort.Slice(workflow_stepsData, func(i, j int) bool {
			vi, vj := 0, 0
			if workflow_stepsData[i].SequencePosition != nil {
				vi = *workflow_stepsData[i].SequencePosition
			}
			if workflow_stepsData[j].SequencePosition != nil {
				vj = *workflow_stepsData[j].SequencePosition
			}
			return vi < vj
		})
	}
	vw_step_precedence_closureData, err := Loadvw_step_precedence_closureRecords(filepath.Join(blankTestsDir, "vw_step_precedence_closure.json"))
	if err != nil {
		fmt.Fprintf(os.Stderr, "Warning: Could not load vw_step_precedence_closure for aggregations: %v\n", err)
		vw_step_precedence_closureData = nil
	}

	// ─────────────────────────────────────────────────────────────────
	// Process Workflows
	// ─────────────────────────────────────────────────────────────────
	fmt.Println("Processing Workflows...")
	workflowsInput := filepath.Join(blankTestsDir, "workflows.json")
	workflowsOutput := filepath.Join(testAnswersDir, "workflows.json")

	workflowsRecords, err := LoadWorkflowRecords(workflowsInput)
	if err != nil {
		errMsg := fmt.Sprintf("Workflows: failed to load - %v", err)
		fmt.Fprintf(os.Stderr, "ERROR: %s\n", errMsg)
		errors = append(errors, errMsg)
	} else {
		// Compute aggregations for Workflows
		count_of_non_proposed_stepsCountMap := make(map[string]int)
		if workflow_stepsData != nil {
			for _, rel := range workflow_stepsData {
				if rel.Workflow != nil {
					count_of_non_proposed_stepsCountMap[*rel.Workflow]++
				}
			}
		}

		count_workflow_artifactsCountMap := make(map[string]int)
		if workflow_artifactsData != nil {
			for _, rel := range workflow_artifactsData {
				if rel.ProducedByWorkflow != nil {
					count_workflow_artifactsCountMap[*rel.ProducedByWorkflow]++
				}
			}
		}

		// Update records with aggregation values
		for i := range workflowsRecords {
			if workflowsRecords[i].WorkflowId != "" {
				count := count_of_non_proposed_stepsCountMap[workflowsRecords[i].WorkflowId]
				workflowsRecords[i].CountOfNonProposedSteps = &count
			}
			if workflowsRecords[i].WorkflowId != "" {
				count := count_workflow_artifactsCountMap[workflowsRecords[i].WorkflowId]
				workflowsRecords[i].CountWorkflowArtifacts = &count
			}
		}

		var computedWorkflow []Workflow
		for _, r := range workflowsRecords {
			computedWorkflow = append(computedWorkflow, *r.ComputeAll())
		}

		if err := SaveWorkflowRecords(workflowsOutput, computedWorkflow); err != nil {
			errMsg := fmt.Sprintf("Workflows: failed to save - %v", err)
			fmt.Fprintf(os.Stderr, "ERROR: %s\n", errMsg)
			errors = append(errors, errMsg)
		} else {
			fmt.Printf("  ✓ workflows: %d records processed\n", len(computedWorkflow))
			totalRecords += len(computedWorkflow)
		}
	}
	fmt.Println("")

	// ─────────────────────────────────────────────────────────────────
	// Process WorkflowSteps
	// ─────────────────────────────────────────────────────────────────
	fmt.Println("Processing WorkflowSteps...")
	workflow_stepsInput := filepath.Join(blankTestsDir, "workflow_steps.json")
	workflow_stepsOutput := filepath.Join(testAnswersDir, "workflow_steps.json")

	workflow_stepsRecords, err := LoadWorkflowStepRecords(workflow_stepsInput)
	if err != nil {
		errMsg := fmt.Sprintf("WorkflowSteps: failed to load - %v", err)
		fmt.Fprintf(os.Stderr, "ERROR: %s\n", errMsg)
		errors = append(errors, errMsg)
	} else {
		// Compute aggregations for WorkflowSteps
		preceding_step_countCountMap := make(map[string]int)
		if vw_step_precedence_closureData != nil {
			for _, rel := range vw_step_precedence_closureData {
				if rel.ToId != nil {
					preceding_step_countCountMap[*rel.ToId]++
				}
			}
		}

		// Update records with aggregation values
		for i := range workflow_stepsRecords {
			if workflow_stepsRecords[i].WorkflowStepId != "" {
				count := preceding_step_countCountMap[workflow_stepsRecords[i].WorkflowStepId]
				workflow_stepsRecords[i].PrecedingStepCount = &count
			}
		}

		var computedWorkflowStep []WorkflowStep
		for _, r := range workflow_stepsRecords {
			computedWorkflowStep = append(computedWorkflowStep, *r.ComputeAll())
		}

		if err := SaveWorkflowStepRecords(workflow_stepsOutput, computedWorkflowStep); err != nil {
			errMsg := fmt.Sprintf("WorkflowSteps: failed to save - %v", err)
			fmt.Fprintf(os.Stderr, "ERROR: %s\n", errMsg)
			errors = append(errors, errMsg)
		} else {
			fmt.Printf("  ✓ workflow_steps: %d records processed\n", len(computedWorkflowStep))
			totalRecords += len(computedWorkflowStep)
		}
		workflow_stepsData = computedWorkflowStep
	}
	fmt.Println("")

	// ─────────────────────────────────────────────────────────────────
	// Process ApprovalGates
	// ─────────────────────────────────────────────────────────────────
	fmt.Println("Processing ApprovalGates...")
	approval_gatesInput := filepath.Join(blankTestsDir, "approval_gates.json")
	approval_gatesOutput := filepath.Join(testAnswersDir, "approval_gates.json")

	approval_gatesRecords, err := LoadApprovalGateRecords(approval_gatesInput)
	if err != nil {
		errMsg := fmt.Sprintf("ApprovalGates: failed to load - %v", err)
		fmt.Fprintf(os.Stderr, "ERROR: %s\n", errMsg)
		errors = append(errors, errMsg)
	} else {
		var computedApprovalGate []ApprovalGate
		for _, r := range approval_gatesRecords {
			computedApprovalGate = append(computedApprovalGate, *r.ComputeAll())
		}

		if err := SaveApprovalGateRecords(approval_gatesOutput, computedApprovalGate); err != nil {
			errMsg := fmt.Sprintf("ApprovalGates: failed to save - %v", err)
			fmt.Fprintf(os.Stderr, "ERROR: %s\n", errMsg)
			errors = append(errors, errMsg)
		} else {
			fmt.Printf("  ✓ approval_gates: %d records processed\n", len(computedApprovalGate))
			totalRecords += len(computedApprovalGate)
		}
	}
	fmt.Println("")

	// ─────────────────────────────────────────────────────────────────
	// Process StepPrecedence
	// ─────────────────────────────────────────────────────────────────
	fmt.Println("Processing StepPrecedence...")
	step_precedenceInput := filepath.Join(blankTestsDir, "step_precedence.json")
	step_precedenceOutput := filepath.Join(testAnswersDir, "step_precedence.json")

	step_precedenceRecords, err := LoadStepPrecedenceRecords(step_precedenceInput)
	if err != nil {
		errMsg := fmt.Sprintf("StepPrecedence: failed to load - %v", err)
		fmt.Fprintf(os.Stderr, "ERROR: %s\n", errMsg)
		errors = append(errors, errMsg)
	} else {
		var computedStepPrecedence []StepPrecedence
		for _, r := range step_precedenceRecords {
			computedStepPrecedence = append(computedStepPrecedence, *r.ComputeAll())
		}

		if err := SaveStepPrecedenceRecords(step_precedenceOutput, computedStepPrecedence); err != nil {
			errMsg := fmt.Sprintf("StepPrecedence: failed to save - %v", err)
			fmt.Fprintf(os.Stderr, "ERROR: %s\n", errMsg)
			errors = append(errors, errMsg)
		} else {
			fmt.Printf("  ✓ step_precedence: %d records processed\n", len(computedStepPrecedence))
			totalRecords += len(computedStepPrecedence)
		}
	}
	fmt.Println("")

	// ─────────────────────────────────────────────────────────────────
	// Process Roles
	// ─────────────────────────────────────────────────────────────────
	fmt.Println("Processing Roles...")
	rolesInput := filepath.Join(blankTestsDir, "roles.json")
	rolesOutput := filepath.Join(testAnswersDir, "roles.json")

	rolesRecords, err := LoadRoleRecords(rolesInput)
	if err != nil {
		errMsg := fmt.Sprintf("Roles: failed to load - %v", err)
		fmt.Fprintf(os.Stderr, "ERROR: %s\n", errMsg)
		errors = append(errors, errMsg)
	} else {
		var computedRole []Role
		for _, r := range rolesRecords {
			computedRole = append(computedRole, *r.ComputeAll())
		}

		if err := SaveRoleRecords(rolesOutput, computedRole); err != nil {
			errMsg := fmt.Sprintf("Roles: failed to save - %v", err)
			fmt.Fprintf(os.Stderr, "ERROR: %s\n", errMsg)
			errors = append(errors, errMsg)
		} else {
			fmt.Printf("  ✓ roles: %d records processed\n", len(computedRole))
			totalRecords += len(computedRole)
		}
	}
	fmt.Println("")

	// ─────────────────────────────────────────────────────────────────
	// Process RoleAssignments
	// ─────────────────────────────────────────────────────────────────
	fmt.Println("Processing RoleAssignments...")
	role_assignmentsInput := filepath.Join(blankTestsDir, "role_assignments.json")
	role_assignmentsOutput := filepath.Join(testAnswersDir, "role_assignments.json")

	role_assignmentsRecords, err := LoadRoleAssignmentRecords(role_assignmentsInput)
	if err != nil {
		errMsg := fmt.Sprintf("RoleAssignments: failed to load - %v", err)
		fmt.Fprintf(os.Stderr, "ERROR: %s\n", errMsg)
		errors = append(errors, errMsg)
	} else {
		var computedRoleAssignment []RoleAssignment
		for _, r := range role_assignmentsRecords {
			computedRoleAssignment = append(computedRoleAssignment, *r.ComputeAll())
		}

		if err := SaveRoleAssignmentRecords(role_assignmentsOutput, computedRoleAssignment); err != nil {
			errMsg := fmt.Sprintf("RoleAssignments: failed to save - %v", err)
			fmt.Fprintf(os.Stderr, "ERROR: %s\n", errMsg)
			errors = append(errors, errMsg)
		} else {
			fmt.Printf("  ✓ role_assignments: %d records processed\n", len(computedRoleAssignment))
			totalRecords += len(computedRoleAssignment)
		}
	}
	fmt.Println("")

	// ─────────────────────────────────────────────────────────────────
	// Process Departments
	// ─────────────────────────────────────────────────────────────────
	fmt.Println("Processing Departments...")
	departmentsInput := filepath.Join(blankTestsDir, "departments.json")
	departmentsOutput := filepath.Join(testAnswersDir, "departments.json")

	departmentsRecords, err := LoadDepartmentRecords(departmentsInput)
	if err != nil {
		errMsg := fmt.Sprintf("Departments: failed to load - %v", err)
		fmt.Fprintf(os.Stderr, "ERROR: %s\n", errMsg)
		errors = append(errors, errMsg)
	} else {
		var computedDepartment []Department
		for _, r := range departmentsRecords {
			computedDepartment = append(computedDepartment, *r.ComputeAll())
		}

		if err := SaveDepartmentRecords(departmentsOutput, computedDepartment); err != nil {
			errMsg := fmt.Sprintf("Departments: failed to save - %v", err)
			fmt.Fprintf(os.Stderr, "ERROR: %s\n", errMsg)
			errors = append(errors, errMsg)
		} else {
			fmt.Printf("  ✓ departments: %d records processed\n", len(computedDepartment))
			totalRecords += len(computedDepartment)
		}
	}
	fmt.Println("")

	// ─────────────────────────────────────────────────────────────────
	// Process HumanAgents
	// ─────────────────────────────────────────────────────────────────
	fmt.Println("Processing HumanAgents...")
	human_agentsInput := filepath.Join(blankTestsDir, "human_agents.json")
	human_agentsOutput := filepath.Join(testAnswersDir, "human_agents.json")

	human_agentsRecords, err := LoadHumanAgentRecords(human_agentsInput)
	if err != nil {
		errMsg := fmt.Sprintf("HumanAgents: failed to load - %v", err)
		fmt.Fprintf(os.Stderr, "ERROR: %s\n", errMsg)
		errors = append(errors, errMsg)
	} else {
		var computedHumanAgent []HumanAgent
		for _, r := range human_agentsRecords {
			computedHumanAgent = append(computedHumanAgent, *r.ComputeAll())
		}

		if err := SaveHumanAgentRecords(human_agentsOutput, computedHumanAgent); err != nil {
			errMsg := fmt.Sprintf("HumanAgents: failed to save - %v", err)
			fmt.Fprintf(os.Stderr, "ERROR: %s\n", errMsg)
			errors = append(errors, errMsg)
		} else {
			fmt.Printf("  ✓ human_agents: %d records processed\n", len(computedHumanAgent))
			totalRecords += len(computedHumanAgent)
		}
	}
	fmt.Println("")

	// ─────────────────────────────────────────────────────────────────
	// Process AIAgents
	// ─────────────────────────────────────────────────────────────────
	fmt.Println("Processing AIAgents...")
	ai_agentsInput := filepath.Join(blankTestsDir, "ai_agents.json")
	ai_agentsOutput := filepath.Join(testAnswersDir, "ai_agents.json")

	ai_agentsRecords, err := LoadAIAgentRecords(ai_agentsInput)
	if err != nil {
		errMsg := fmt.Sprintf("AIAgents: failed to load - %v", err)
		fmt.Fprintf(os.Stderr, "ERROR: %s\n", errMsg)
		errors = append(errors, errMsg)
	} else {
		// Compute aggregations for AIAgents
		count_attributed_artifactsCountMap := make(map[string]int)
		if workflow_artifactsData != nil {
			for _, rel := range workflow_artifactsData {
				if rel.AttributedToAIAgent != nil {
					count_attributed_artifactsCountMap[*rel.AttributedToAIAgent]++
				}
			}
		}

		// Update records with aggregation values
		for i := range ai_agentsRecords {
			if ai_agentsRecords[i].AIAgentId != "" {
				count := count_attributed_artifactsCountMap[ai_agentsRecords[i].AIAgentId]
				ai_agentsRecords[i].CountAttributedArtifacts = &count
			}
		}

		var computedAIAgent []AIAgent
		for _, r := range ai_agentsRecords {
			computedAIAgent = append(computedAIAgent, *r.ComputeAll())
		}

		if err := SaveAIAgentRecords(ai_agentsOutput, computedAIAgent); err != nil {
			errMsg := fmt.Sprintf("AIAgents: failed to save - %v", err)
			fmt.Fprintf(os.Stderr, "ERROR: %s\n", errMsg)
			errors = append(errors, errMsg)
		} else {
			fmt.Printf("  ✓ ai_agents: %d records processed\n", len(computedAIAgent))
			totalRecords += len(computedAIAgent)
		}
	}
	fmt.Println("")

	// ─────────────────────────────────────────────────────────────────
	// Process AutomatedPipelines
	// ─────────────────────────────────────────────────────────────────
	fmt.Println("Processing AutomatedPipelines...")
	automated_pipelinesInput := filepath.Join(blankTestsDir, "automated_pipelines.json")
	automated_pipelinesOutput := filepath.Join(testAnswersDir, "automated_pipelines.json")

	automated_pipelinesRecords, err := LoadAutomatedPipelineRecords(automated_pipelinesInput)
	if err != nil {
		errMsg := fmt.Sprintf("AutomatedPipelines: failed to load - %v", err)
		fmt.Fprintf(os.Stderr, "ERROR: %s\n", errMsg)
		errors = append(errors, errMsg)
	} else {
		var computedAutomatedPipeline []AutomatedPipeline
		for _, r := range automated_pipelinesRecords {
			computedAutomatedPipeline = append(computedAutomatedPipeline, *r.ComputeAll())
		}

		if err := SaveAutomatedPipelineRecords(automated_pipelinesOutput, computedAutomatedPipeline); err != nil {
			errMsg := fmt.Sprintf("AutomatedPipelines: failed to save - %v", err)
			fmt.Fprintf(os.Stderr, "ERROR: %s\n", errMsg)
			errors = append(errors, errMsg)
		} else {
			fmt.Printf("  ✓ automated_pipelines: %d records processed\n", len(computedAutomatedPipeline))
			totalRecords += len(computedAutomatedPipeline)
		}
	}
	fmt.Println("")

	// ─────────────────────────────────────────────────────────────────
	// Process WorkflowStatusConcepts
	// ─────────────────────────────────────────────────────────────────
	fmt.Println("Processing WorkflowStatusConcepts...")
	workflow_status_conceptsInput := filepath.Join(blankTestsDir, "workflow_status_concepts.json")
	workflow_status_conceptsOutput := filepath.Join(testAnswersDir, "workflow_status_concepts.json")

	workflow_status_conceptsRecords, err := LoadWorkflowStatusConceptRecords(workflow_status_conceptsInput)
	if err != nil {
		errMsg := fmt.Sprintf("WorkflowStatusConcepts: failed to load - %v", err)
		fmt.Fprintf(os.Stderr, "ERROR: %s\n", errMsg)
		errors = append(errors, errMsg)
	} else {
		var computedWorkflowStatusConcept []WorkflowStatusConcept
		for _, r := range workflow_status_conceptsRecords {
			computedWorkflowStatusConcept = append(computedWorkflowStatusConcept, *r.ComputeAll())
		}

		if err := SaveWorkflowStatusConceptRecords(workflow_status_conceptsOutput, computedWorkflowStatusConcept); err != nil {
			errMsg := fmt.Sprintf("WorkflowStatusConcepts: failed to save - %v", err)
			fmt.Fprintf(os.Stderr, "ERROR: %s\n", errMsg)
			errors = append(errors, errMsg)
		} else {
			fmt.Printf("  ✓ workflow_status_concepts: %d records processed\n", len(computedWorkflowStatusConcept))
			totalRecords += len(computedWorkflowStatusConcept)
		}
	}
	fmt.Println("")

	// ─────────────────────────────────────────────────────────────────
	// Process AgentCapabilityConcepts
	// ─────────────────────────────────────────────────────────────────
	fmt.Println("Processing AgentCapabilityConcepts...")
	agent_capability_conceptsInput := filepath.Join(blankTestsDir, "agent_capability_concepts.json")
	agent_capability_conceptsOutput := filepath.Join(testAnswersDir, "agent_capability_concepts.json")

	agent_capability_conceptsRecords, err := LoadAgentCapabilityConceptRecords(agent_capability_conceptsInput)
	if err != nil {
		errMsg := fmt.Sprintf("AgentCapabilityConcepts: failed to load - %v", err)
		fmt.Fprintf(os.Stderr, "ERROR: %s\n", errMsg)
		errors = append(errors, errMsg)
	} else {
		var computedAgentCapabilityConcept []AgentCapabilityConcept
		for _, r := range agent_capability_conceptsRecords {
			computedAgentCapabilityConcept = append(computedAgentCapabilityConcept, *r.ComputeAll())
		}

		if err := SaveAgentCapabilityConceptRecords(agent_capability_conceptsOutput, computedAgentCapabilityConcept); err != nil {
			errMsg := fmt.Sprintf("AgentCapabilityConcepts: failed to save - %v", err)
			fmt.Fprintf(os.Stderr, "ERROR: %s\n", errMsg)
			errors = append(errors, errMsg)
		} else {
			fmt.Printf("  ✓ agent_capability_concepts: %d records processed\n", len(computedAgentCapabilityConcept))
			totalRecords += len(computedAgentCapabilityConcept)
		}
	}
	fmt.Println("")

	// ─────────────────────────────────────────────────────────────────
	// Process ArtifactTypeConcepts
	// ─────────────────────────────────────────────────────────────────
	fmt.Println("Processing ArtifactTypeConcepts...")
	artifact_type_conceptsInput := filepath.Join(blankTestsDir, "artifact_type_concepts.json")
	artifact_type_conceptsOutput := filepath.Join(testAnswersDir, "artifact_type_concepts.json")

	artifact_type_conceptsRecords, err := LoadArtifactTypeConceptRecords(artifact_type_conceptsInput)
	if err != nil {
		errMsg := fmt.Sprintf("ArtifactTypeConcepts: failed to load - %v", err)
		fmt.Fprintf(os.Stderr, "ERROR: %s\n", errMsg)
		errors = append(errors, errMsg)
	} else {
		var computedArtifactTypeConcept []ArtifactTypeConcept
		for _, r := range artifact_type_conceptsRecords {
			computedArtifactTypeConcept = append(computedArtifactTypeConcept, *r.ComputeAll())
		}

		if err := SaveArtifactTypeConceptRecords(artifact_type_conceptsOutput, computedArtifactTypeConcept); err != nil {
			errMsg := fmt.Sprintf("ArtifactTypeConcepts: failed to save - %v", err)
			fmt.Fprintf(os.Stderr, "ERROR: %s\n", errMsg)
			errors = append(errors, errMsg)
		} else {
			fmt.Printf("  ✓ artifact_type_concepts: %d records processed\n", len(computedArtifactTypeConcept))
			totalRecords += len(computedArtifactTypeConcept)
		}
	}
	fmt.Println("")

	// ─────────────────────────────────────────────────────────────────
	// Process Datasets
	// ─────────────────────────────────────────────────────────────────
	fmt.Println("Processing Datasets...")
	datasetsInput := filepath.Join(blankTestsDir, "datasets.json")
	datasetsOutput := filepath.Join(testAnswersDir, "datasets.json")

	datasetsRecords, err := LoadDatasetRecords(datasetsInput)
	if err != nil {
		errMsg := fmt.Sprintf("Datasets: failed to load - %v", err)
		fmt.Fprintf(os.Stderr, "ERROR: %s\n", errMsg)
		errors = append(errors, errMsg)
	} else {
		var computedDataset []Dataset
		for _, r := range datasetsRecords {
			computedDataset = append(computedDataset, *r.ComputeAll())
		}

		if err := SaveDatasetRecords(datasetsOutput, computedDataset); err != nil {
			errMsg := fmt.Sprintf("Datasets: failed to save - %v", err)
			fmt.Fprintf(os.Stderr, "ERROR: %s\n", errMsg)
			errors = append(errors, errMsg)
		} else {
			fmt.Printf("  ✓ datasets: %d records processed\n", len(computedDataset))
			totalRecords += len(computedDataset)
		}
	}
	fmt.Println("")

	// ─────────────────────────────────────────────────────────────────
	// Process WorkflowArtifacts
	// ─────────────────────────────────────────────────────────────────
	fmt.Println("Processing WorkflowArtifacts...")
	workflow_artifactsInput := filepath.Join(blankTestsDir, "workflow_artifacts.json")
	workflow_artifactsOutput := filepath.Join(testAnswersDir, "workflow_artifacts.json")

	workflow_artifactsRecords, err := LoadWorkflowArtifactRecords(workflow_artifactsInput)
	if err != nil {
		errMsg := fmt.Sprintf("WorkflowArtifacts: failed to load - %v", err)
		fmt.Fprintf(os.Stderr, "ERROR: %s\n", errMsg)
		errors = append(errors, errMsg)
	} else {
		var computedWorkflowArtifact []WorkflowArtifact
		for _, r := range workflow_artifactsRecords {
			computedWorkflowArtifact = append(computedWorkflowArtifact, *r.ComputeAll())
		}

		if err := SaveWorkflowArtifactRecords(workflow_artifactsOutput, computedWorkflowArtifact); err != nil {
			errMsg := fmt.Sprintf("WorkflowArtifacts: failed to save - %v", err)
			fmt.Fprintf(os.Stderr, "ERROR: %s\n", errMsg)
			errors = append(errors, errMsg)
		} else {
			fmt.Printf("  ✓ workflow_artifacts: %d records processed\n", len(computedWorkflowArtifact))
			totalRecords += len(computedWorkflowArtifact)
		}
		workflow_artifactsData = computedWorkflowArtifact
	}
	fmt.Println("")

	// ─────────────────────────────────────────────────────────────────
	// Process GovernanceRoles
	// ─────────────────────────────────────────────────────────────────
	fmt.Println("Processing GovernanceRoles...")
	governance_rolesInput := filepath.Join(blankTestsDir, "governance_roles.json")
	governance_rolesOutput := filepath.Join(testAnswersDir, "governance_roles.json")

	governance_rolesRecords, err := LoadGovernanceRoleRecords(governance_rolesInput)
	if err != nil {
		errMsg := fmt.Sprintf("GovernanceRoles: failed to load - %v", err)
		fmt.Fprintf(os.Stderr, "ERROR: %s\n", errMsg)
		errors = append(errors, errMsg)
	} else {
		var computedGovernanceRole []GovernanceRole
		for _, r := range governance_rolesRecords {
			computedGovernanceRole = append(computedGovernanceRole, *r.ComputeAll())
		}

		if err := SaveGovernanceRoleRecords(governance_rolesOutput, computedGovernanceRole); err != nil {
			errMsg := fmt.Sprintf("GovernanceRoles: failed to save - %v", err)
			fmt.Fprintf(os.Stderr, "ERROR: %s\n", errMsg)
			errors = append(errors, errMsg)
		} else {
			fmt.Printf("  ✓ governance_roles: %d records processed\n", len(computedGovernanceRole))
			totalRecords += len(computedGovernanceRole)
		}
	}
	fmt.Println("")

	// ─────────────────────────────────────────────────────────────────
	// Process ChangeLog
	// ─────────────────────────────────────────────────────────────────
	fmt.Println("Processing ChangeLog...")
	change_logInput := filepath.Join(blankTestsDir, "change_log.json")
	change_logOutput := filepath.Join(testAnswersDir, "change_log.json")

	change_logRecords, err := LoadChangeLogRecords(change_logInput)
	if err != nil {
		errMsg := fmt.Sprintf("ChangeLog: failed to load - %v", err)
		fmt.Fprintf(os.Stderr, "ERROR: %s\n", errMsg)
		errors = append(errors, errMsg)
	} else {
		var computedChangeLog []ChangeLog
		for _, r := range change_logRecords {
			computedChangeLog = append(computedChangeLog, *r.ComputeAll())
		}

		if err := SaveChangeLogRecords(change_logOutput, computedChangeLog); err != nil {
			errMsg := fmt.Sprintf("ChangeLog: failed to save - %v", err)
			fmt.Fprintf(os.Stderr, "ERROR: %s\n", errMsg)
			errors = append(errors, errMsg)
		} else {
			fmt.Printf("  ✓ change_log: %d records processed\n", len(computedChangeLog))
			totalRecords += len(computedChangeLog)
		}
	}
	fmt.Println("")

	// ─────────────────────────────────────────────────────────────────
	// Process VocabularyReconciliations
	// ─────────────────────────────────────────────────────────────────
	fmt.Println("Processing VocabularyReconciliations...")
	vocabulary_reconciliationsInput := filepath.Join(blankTestsDir, "vocabulary_reconciliations.json")
	vocabulary_reconciliationsOutput := filepath.Join(testAnswersDir, "vocabulary_reconciliations.json")

	vocabulary_reconciliationsRecords, err := LoadVocabularyReconciliationRecords(vocabulary_reconciliationsInput)
	if err != nil {
		errMsg := fmt.Sprintf("VocabularyReconciliations: failed to load - %v", err)
		fmt.Fprintf(os.Stderr, "ERROR: %s\n", errMsg)
		errors = append(errors, errMsg)
	} else {
		var computedVocabularyReconciliation []VocabularyReconciliation
		for _, r := range vocabulary_reconciliationsRecords {
			computedVocabularyReconciliation = append(computedVocabularyReconciliation, *r.ComputeAll())
		}

		if err := SaveVocabularyReconciliationRecords(vocabulary_reconciliationsOutput, computedVocabularyReconciliation); err != nil {
			errMsg := fmt.Sprintf("VocabularyReconciliations: failed to save - %v", err)
			fmt.Fprintf(os.Stderr, "ERROR: %s\n", errMsg)
			errors = append(errors, errMsg)
		} else {
			fmt.Printf("  ✓ vocabulary_reconciliations: %d records processed\n", len(computedVocabularyReconciliation))
			totalRecords += len(computedVocabularyReconciliation)
		}
	}
	fmt.Println("")

	// ─────────────────────────────────────────────────────────────────
	// Process Scenarios
	// ─────────────────────────────────────────────────────────────────
	fmt.Println("Processing Scenarios...")
	scenariosInput := filepath.Join(blankTestsDir, "scenarios.json")
	scenariosOutput := filepath.Join(testAnswersDir, "scenarios.json")

	scenariosRecords, err := LoadScenarioRecords(scenariosInput)
	if err != nil {
		errMsg := fmt.Sprintf("Scenarios: failed to load - %v", err)
		fmt.Fprintf(os.Stderr, "ERROR: %s\n", errMsg)
		errors = append(errors, errMsg)
	} else {
		var computedScenario []Scenario
		for _, r := range scenariosRecords {
			computedScenario = append(computedScenario, *r.ComputeAll())
		}

		if err := SaveScenarioRecords(scenariosOutput, computedScenario); err != nil {
			errMsg := fmt.Sprintf("Scenarios: failed to save - %v", err)
			fmt.Fprintf(os.Stderr, "ERROR: %s\n", errMsg)
			errors = append(errors, errMsg)
		} else {
			fmt.Printf("  ✓ scenarios: %d records processed\n", len(computedScenario))
			totalRecords += len(computedScenario)
		}
	}
	fmt.Println("")

	// ─────────────────────────────────────────────────────────────────
	// Process CompetencyQuestions
	// ─────────────────────────────────────────────────────────────────
	fmt.Println("Processing CompetencyQuestions...")
	competency_questionsInput := filepath.Join(blankTestsDir, "competency_questions.json")
	competency_questionsOutput := filepath.Join(testAnswersDir, "competency_questions.json")

	competency_questionsRecords, err := LoadCompetencyQuestionRecords(competency_questionsInput)
	if err != nil {
		errMsg := fmt.Sprintf("CompetencyQuestions: failed to load - %v", err)
		fmt.Fprintf(os.Stderr, "ERROR: %s\n", errMsg)
		errors = append(errors, errMsg)
	} else {
		var computedCompetencyQuestion []CompetencyQuestion
		for _, r := range competency_questionsRecords {
			computedCompetencyQuestion = append(computedCompetencyQuestion, *r.ComputeAll())
		}

		if err := SaveCompetencyQuestionRecords(competency_questionsOutput, computedCompetencyQuestion); err != nil {
			errMsg := fmt.Sprintf("CompetencyQuestions: failed to save - %v", err)
			fmt.Fprintf(os.Stderr, "ERROR: %s\n", errMsg)
			errors = append(errors, errMsg)
		} else {
			fmt.Printf("  ✓ competency_questions: %d records processed\n", len(computedCompetencyQuestion))
			totalRecords += len(computedCompetencyQuestion)
		}
	}
	fmt.Println("")

	// ─────────────────────────────────────────────────────────────────
	// Process ScenarioCQEffects
	// ─────────────────────────────────────────────────────────────────
	fmt.Println("Processing ScenarioCQEffects...")
	scenario_cq_effectsInput := filepath.Join(blankTestsDir, "scenario_cq_effects.json")
	scenario_cq_effectsOutput := filepath.Join(testAnswersDir, "scenario_cq_effects.json")

	scenario_cq_effectsRecords, err := LoadScenarioCQEffectRecords(scenario_cq_effectsInput)
	if err != nil {
		errMsg := fmt.Sprintf("ScenarioCQEffects: failed to load - %v", err)
		fmt.Fprintf(os.Stderr, "ERROR: %s\n", errMsg)
		errors = append(errors, errMsg)
	} else {
		var computedScenarioCQEffect []ScenarioCQEffect
		for _, r := range scenario_cq_effectsRecords {
			computedScenarioCQEffect = append(computedScenarioCQEffect, *r.ComputeAll())
		}

		if err := SaveScenarioCQEffectRecords(scenario_cq_effectsOutput, computedScenarioCQEffect); err != nil {
			errMsg := fmt.Sprintf("ScenarioCQEffects: failed to save - %v", err)
			fmt.Fprintf(os.Stderr, "ERROR: %s\n", errMsg)
			errors = append(errors, errMsg)
		} else {
			fmt.Printf("  ✓ scenario_cq_effects: %d records processed\n", len(computedScenarioCQEffect))
			totalRecords += len(computedScenarioCQEffect)
		}
	}
	fmt.Println("")

	// ─────────────────────────────────────────────────────────────────
	// Process ConformanceTests
	// ─────────────────────────────────────────────────────────────────
	fmt.Println("Processing ConformanceTests...")
	conformance_testsInput := filepath.Join(blankTestsDir, "conformance_tests.json")
	conformance_testsOutput := filepath.Join(testAnswersDir, "conformance_tests.json")

	conformance_testsRecords, err := LoadConformanceTestRecords(conformance_testsInput)
	if err != nil {
		errMsg := fmt.Sprintf("ConformanceTests: failed to load - %v", err)
		fmt.Fprintf(os.Stderr, "ERROR: %s\n", errMsg)
		errors = append(errors, errMsg)
	} else {
		var computedConformanceTest []ConformanceTest
		for _, r := range conformance_testsRecords {
			computedConformanceTest = append(computedConformanceTest, *r.ComputeAll())
		}

		if err := SaveConformanceTestRecords(conformance_testsOutput, computedConformanceTest); err != nil {
			errMsg := fmt.Sprintf("ConformanceTests: failed to save - %v", err)
			fmt.Fprintf(os.Stderr, "ERROR: %s\n", errMsg)
			errors = append(errors, errMsg)
		} else {
			fmt.Printf("  ✓ conformance_tests: %d records processed\n", len(computedConformanceTest))
			totalRecords += len(computedConformanceTest)
		}
	}
	fmt.Println("")

	// ─────────────────────────────────────────────────────────────────
	// Process __meta__
	// ─────────────────────────────────────────────────────────────────
	fmt.Println("Processing __meta__...")
	__meta__Input := filepath.Join(blankTestsDir, "__meta__.json")
	__meta__Output := filepath.Join(testAnswersDir, "__meta__.json")

	__meta__Records, err := Load__meta__Records(__meta__Input)
	if err != nil {
		errMsg := fmt.Sprintf("__meta__: failed to load - %v", err)
		fmt.Fprintf(os.Stderr, "ERROR: %s\n", errMsg)
		errors = append(errors, errMsg)
	} else {
		var computed__meta__ []__meta__
		for _, r := range __meta__Records {
			computed__meta__ = append(computed__meta__, *r.ComputeAll())
		}

		if err := Save__meta__Records(__meta__Output, computed__meta__); err != nil {
			errMsg := fmt.Sprintf("__meta__: failed to save - %v", err)
			fmt.Fprintf(os.Stderr, "ERROR: %s\n", errMsg)
			errors = append(errors, errMsg)
		} else {
			fmt.Printf("  ✓ __meta__: %d records processed\n", len(computed__meta__))
			totalRecords += len(computed__meta__)
		}
	}
	fmt.Println("")

	// ─────────────────────────────────────────────────────────────────
	// Final validation - FAIL LOUDLY if any errors occurred
	// ─────────────────────────────────────────────────────────────────
	if len(errors) > 0 {
		fmt.Fprintf(os.Stderr, "\n")
		fmt.Fprintf(os.Stderr, "════════════════════════════════════════════════════════════════\n")
		fmt.Fprintf(os.Stderr, "FATAL: %d table(s) FAILED to process\n", len(errors))
		fmt.Fprintf(os.Stderr, "════════════════════════════════════════════════════════════════\n")
		for _, e := range errors {
			fmt.Fprintf(os.Stderr, "  • %s\n", e)
		}
		fmt.Fprintf(os.Stderr, "\n")
		os.Exit(1)
	}

	fmt.Println("════════════════════════════════════════════════════════════════")
	fmt.Printf("Golang substrate: ALL %d tables processed successfully (%d total records)\n", 23, totalRecords)
	fmt.Println("════════════════════════════════════════════════════════════════")
}