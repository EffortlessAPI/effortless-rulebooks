// ERB SDK - Go Test Runner (GENERATED - DO NOT EDIT)
// =======================================================
// This file is REGENERATED every time inject-into-golang.py runs.
// It must stay in sync with erb_sdk.go and the rulebook.
//
// Tables with computed fields: Client, Roles, Employees, TypesOfProject, Projects
// Tables with aggregations: Roles, TypesOfProject
// Tables with lookups: Projects, Employees
//
// IMPORTANT: This runner processes ALL tables, not just a "primary" one.
// If ANY table fails to process, the entire run fails with exit code 1.

package main

import (
	"fmt"
	"os"
	"path/filepath"
)

func main() {
	scriptDir, err := os.Getwd()
	if err != nil {
		fmt.Fprintf(os.Stderr, "FATAL: Failed to get working directory: %v\n", err)
		os.Exit(1)
	}

	// Shared blank-tests directory at project root
	blankTestsDir := filepath.Join(scriptDir, "..", "..", "testing", "blank-tests")
	testAnswersDir := filepath.Join(scriptDir, "test-answers")

	// Ensure output directory exists
	if err := os.MkdirAll(testAnswersDir, 0755); err != nil {
		fmt.Fprintf(os.Stderr, "FATAL: Failed to create test-answers directory: %v\n", err)
		os.Exit(1)
	}

	fmt.Println("Golang substrate: Processing 5 tables with calculated fields...")
	fmt.Println("  Expected tables: Client, Roles, Employees, TypesOfProject, Projects")
	fmt.Println("")

	// Track success/failure for ALL tables
	var errors []string
	var totalRecords int

	// ─────────────────────────────────────────────────────────────────
	// Load related tables for aggregation calculations
	// ─────────────────────────────────────────────────────────────────
	// Note: SUMIFS loads from answer-keys (has computed fields)
	//       COUNTIFS loads from blank-tests

	employeesData, err := LoadEmployeeRecords(filepath.Join(blankTestsDir, "employees.json"))
	if err != nil {
		fmt.Fprintf(os.Stderr, "Warning: Could not load Employees for aggregations: %v\n", err)
		employeesData = nil
	}
	projectsData, err := LoadProjectRecords(filepath.Join(blankTestsDir, "projects.json"))
	if err != nil {
		fmt.Fprintf(os.Stderr, "Warning: Could not load Projects for aggregations: %v\n", err)
		projectsData = nil
	}
	rolesData, err := LoadRoleRecords(filepath.Join(blankTestsDir, "roles.json"))
	if err != nil {
		fmt.Fprintf(os.Stderr, "Warning: Could not load Roles for aggregations: %v\n", err)
		rolesData = nil
	}
	types_of_projectData, err := LoadTypesOfProjectRecords(filepath.Join(blankTestsDir, "types_of_project.json"))
	if err != nil {
		fmt.Fprintf(os.Stderr, "Warning: Could not load TypesOfProject for aggregations: %v\n", err)
		types_of_projectData = nil
	}

	// ─────────────────────────────────────────────────────────────────
	// Process Client
	// ─────────────────────────────────────────────────────────────────
	fmt.Println("Processing Client...")
	clientInput := filepath.Join(blankTestsDir, "client.json")
	clientOutput := filepath.Join(testAnswersDir, "client.json")

	clientRecords, err := LoadClientRecords(clientInput)
	if err != nil {
		errMsg := fmt.Sprintf("Client: failed to load - %v", err)
		fmt.Fprintf(os.Stderr, "ERROR: %s\n", errMsg)
		errors = append(errors, errMsg)
	} else {
		var computedClient []Client
		for _, r := range clientRecords {
			computedClient = append(computedClient, *r.ComputeAll())
		}

		if err := SaveClientRecords(clientOutput, computedClient); err != nil {
			errMsg := fmt.Sprintf("Client: failed to save - %v", err)
			fmt.Fprintf(os.Stderr, "ERROR: %s\n", errMsg)
			errors = append(errors, errMsg)
		} else {
			fmt.Printf("  ✓ client: %d records processed\n", len(computedClient))
			totalRecords += len(computedClient)
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
		// Compute aggregations for Roles
		count_of_employeesCountMap := make(map[string]int)
		if employeesData != nil {
			for _, rel := range employeesData {
				if rel.Role != nil {
					count_of_employeesCountMap[*rel.Role]++
				}
			}
		}

		// Update records with aggregation values
		for i := range rolesRecords {
			if rolesRecords[i].RoleId != "" {
				count := count_of_employeesCountMap[rolesRecords[i].RoleId]
				rolesRecords[i].CountOfEmployees = &count
			}
		}

		var computedRole []Role
		for _, r := range rolesRecords {
			computedRole = append(computedRole, r)
		}

		if err := SaveRoleRecords(rolesOutput, computedRole); err != nil {
			errMsg := fmt.Sprintf("Roles: failed to save - %v", err)
			fmt.Fprintf(os.Stderr, "ERROR: %s\n", errMsg)
			errors = append(errors, errMsg)
		} else {
			fmt.Printf("  ✓ roles: %d records processed\n", len(computedRole))
			totalRecords += len(computedRole)
		}
		rolesData = computedRole
	}
	fmt.Println("")

	// ─────────────────────────────────────────────────────────────────
	// Process Employees
	// ─────────────────────────────────────────────────────────────────
	fmt.Println("Processing Employees...")
	employeesInput := filepath.Join(blankTestsDir, "employees.json")
	employeesOutput := filepath.Join(testAnswersDir, "employees.json")

	employeesRecords, err := LoadEmployeeRecords(employeesInput)
	if err != nil {
		errMsg := fmt.Sprintf("Employees: failed to load - %v", err)
		fmt.Fprintf(os.Stderr, "ERROR: %s\n", errMsg)
		errors = append(errors, errMsg)
	} else {
		// Compute lookups for Employees
		role_nameLookupMap := make(map[string]string)
		if rolesData != nil {
			for _, rel := range rolesData {
				if rel.RoleId != "" {
					var val string = ""
					if rel.Name != nil {
						val = *rel.Name
					}
					role_nameLookupMap[rel.RoleId] = val
				}
			}
		}

		role_descriptionLookupMap := make(map[string]string)
		if rolesData != nil {
			for _, rel := range rolesData {
				if rel.RoleId != "" {
					var val string = ""
					if rel.Description != nil {
						val = *rel.Description
					}
					role_descriptionLookupMap[rel.RoleId] = val
				}
			}
		}

		role_is_managerLookupMap := make(map[string]bool)
		if rolesData != nil {
			for _, rel := range rolesData {
				if rel.RoleId != "" {
					var val bool = false
					if rel.IsManager != nil {
						val = *rel.IsManager
					}
					role_is_managerLookupMap[rel.RoleId] = val
				}
			}
		}

		// Apply lookup values to records
		for i := range employeesRecords {
			if employeesRecords[i].Role != nil && *employeesRecords[i].Role != "" {
				if val, ok := role_nameLookupMap[*employeesRecords[i].Role]; ok {
					employeesRecords[i].RoleName = &val
				}
			}
			if employeesRecords[i].Role != nil && *employeesRecords[i].Role != "" {
				if val, ok := role_descriptionLookupMap[*employeesRecords[i].Role]; ok {
					employeesRecords[i].RoleDescription = &val
				}
			}
			if employeesRecords[i].Role != nil && *employeesRecords[i].Role != "" {
				if val, ok := role_is_managerLookupMap[*employeesRecords[i].Role]; ok {
					employeesRecords[i].RoleIsManager = &val
				}
			}
		}

		var computedEmployee []Employee
		for _, r := range employeesRecords {
			computedEmployee = append(computedEmployee, r)
		}

		if err := SaveEmployeeRecords(employeesOutput, computedEmployee); err != nil {
			errMsg := fmt.Sprintf("Employees: failed to save - %v", err)
			fmt.Fprintf(os.Stderr, "ERROR: %s\n", errMsg)
			errors = append(errors, errMsg)
		} else {
			fmt.Printf("  ✓ employees: %d records processed\n", len(computedEmployee))
			totalRecords += len(computedEmployee)
		}
		employeesData = computedEmployee
	}
	fmt.Println("")

	// ─────────────────────────────────────────────────────────────────
	// Process TypesOfProject
	// ─────────────────────────────────────────────────────────────────
	fmt.Println("Processing TypesOfProject...")
	types_of_projectInput := filepath.Join(blankTestsDir, "types_of_project.json")
	types_of_projectOutput := filepath.Join(testAnswersDir, "types_of_project.json")

	types_of_projectRecords, err := LoadTypesOfProjectRecords(types_of_projectInput)
	if err != nil {
		errMsg := fmt.Sprintf("TypesOfProject: failed to load - %v", err)
		fmt.Fprintf(os.Stderr, "ERROR: %s\n", errMsg)
		errors = append(errors, errMsg)
	} else {
		// Compute aggregations for TypesOfProject
		count_of_projectsCountMap := make(map[string]int)
		if projectsData != nil {
			for _, rel := range projectsData {
				if rel.ProjectType != nil {
					count_of_projectsCountMap[*rel.ProjectType]++
				}
			}
		}

		// Update records with aggregation values
		for i := range types_of_projectRecords {
			if types_of_projectRecords[i].TypesOfProjectId != "" {
				count := count_of_projectsCountMap[types_of_projectRecords[i].TypesOfProjectId]
				types_of_projectRecords[i].CountOfProjects = &count
			}
		}

		var computedTypesOfProject []TypesOfProject
		for _, r := range types_of_projectRecords {
			computedTypesOfProject = append(computedTypesOfProject, r)
		}

		if err := SaveTypesOfProjectRecords(types_of_projectOutput, computedTypesOfProject); err != nil {
			errMsg := fmt.Sprintf("TypesOfProject: failed to save - %v", err)
			fmt.Fprintf(os.Stderr, "ERROR: %s\n", errMsg)
			errors = append(errors, errMsg)
		} else {
			fmt.Printf("  ✓ types_of_project: %d records processed\n", len(computedTypesOfProject))
			totalRecords += len(computedTypesOfProject)
		}
		types_of_projectData = computedTypesOfProject
	}
	fmt.Println("")

	// ─────────────────────────────────────────────────────────────────
	// Process Projects
	// ─────────────────────────────────────────────────────────────────
	fmt.Println("Processing Projects...")
	projectsInput := filepath.Join(blankTestsDir, "projects.json")
	projectsOutput := filepath.Join(testAnswersDir, "projects.json")

	projectsRecords, err := LoadProjectRecords(projectsInput)
	if err != nil {
		errMsg := fmt.Sprintf("Projects: failed to load - %v", err)
		fmt.Fprintf(os.Stderr, "ERROR: %s\n", errMsg)
		errors = append(errors, errMsg)
	} else {
		// Compute lookups for Projects
		project_type_nameLookupMap := make(map[string]string)
		if types_of_projectData != nil {
			for _, rel := range types_of_projectData {
				if rel.TypesOfProjectId != "" {
					var val string = ""
					if rel.Name != nil {
						val = *rel.Name
					}
					project_type_nameLookupMap[rel.TypesOfProjectId] = val
				}
			}
		}

		project_type_descriptionLookupMap := make(map[string]string)
		if types_of_projectData != nil {
			for _, rel := range types_of_projectData {
				if rel.TypesOfProjectId != "" {
					var val string = ""
					if rel.Description != nil {
						val = *rel.Description
					}
					project_type_descriptionLookupMap[rel.TypesOfProjectId] = val
				}
			}
		}

		project_type_requires_manager_approvalLookupMap := make(map[string]bool)
		if types_of_projectData != nil {
			for _, rel := range types_of_projectData {
				if rel.TypesOfProjectId != "" {
					var val bool = false
					if rel.RequiresManagerApproval != nil {
						val = *rel.RequiresManagerApproval
					}
					project_type_requires_manager_approvalLookupMap[rel.TypesOfProjectId] = val
				}
			}
		}

		approved_by_role_is_managerLookupMap := make(map[string]bool)
		if employeesData != nil {
			for _, rel := range employeesData {
				if rel.EmployeeId != "" {
					var val bool = false
					if rel.RoleIsManager != nil {
						val = *rel.RoleIsManager
					}
					approved_by_role_is_managerLookupMap[rel.EmployeeId] = val
				}
			}
		}

		approved_by_nameLookupMap := make(map[string]string)
		if employeesData != nil {
			for _, rel := range employeesData {
				if rel.EmployeeId != "" {
					var val string = ""
					if rel.Name != nil {
						val = *rel.Name
					}
					approved_by_nameLookupMap[rel.EmployeeId] = val
				}
			}
		}

		approved_by_email_addressLookupMap := make(map[string]string)
		if employeesData != nil {
			for _, rel := range employeesData {
				if rel.EmployeeId != "" {
					var val string = ""
					if rel.EmailAddress != nil {
						val = *rel.EmailAddress
					}
					approved_by_email_addressLookupMap[rel.EmployeeId] = val
				}
			}
		}

		approved_by_phone_numberLookupMap := make(map[string]string)
		if employeesData != nil {
			for _, rel := range employeesData {
				if rel.EmployeeId != "" {
					var val string = ""
					if rel.PhoneNumber != nil {
						val = *rel.PhoneNumber
					}
					approved_by_phone_numberLookupMap[rel.EmployeeId] = val
				}
			}
		}

		// Apply lookup values to records
		for i := range projectsRecords {
			if projectsRecords[i].ProjectType != nil && *projectsRecords[i].ProjectType != "" {
				if val, ok := project_type_nameLookupMap[*projectsRecords[i].ProjectType]; ok {
					projectsRecords[i].ProjectTypeName = &val
				}
			}
			if projectsRecords[i].ProjectType != nil && *projectsRecords[i].ProjectType != "" {
				if val, ok := project_type_descriptionLookupMap[*projectsRecords[i].ProjectType]; ok {
					projectsRecords[i].ProjectTypeDescription = &val
				}
			}
			if projectsRecords[i].ProjectType != nil && *projectsRecords[i].ProjectType != "" {
				if val, ok := project_type_requires_manager_approvalLookupMap[*projectsRecords[i].ProjectType]; ok {
					projectsRecords[i].ProjectTypeRequiresManagerApproval = &val
				}
			}
			if projectsRecords[i].ApprovedBy != nil && *projectsRecords[i].ApprovedBy != "" {
				if val, ok := approved_by_role_is_managerLookupMap[*projectsRecords[i].ApprovedBy]; ok {
					projectsRecords[i].ApprovedByRoleIsManager = &val
				}
			}
			if projectsRecords[i].ApprovedBy != nil && *projectsRecords[i].ApprovedBy != "" {
				if val, ok := approved_by_nameLookupMap[*projectsRecords[i].ApprovedBy]; ok {
					projectsRecords[i].ApprovedByName = &val
				}
			}
			if projectsRecords[i].ApprovedBy != nil && *projectsRecords[i].ApprovedBy != "" {
				if val, ok := approved_by_email_addressLookupMap[*projectsRecords[i].ApprovedBy]; ok {
					projectsRecords[i].ApprovedByEmailAddress = &val
				}
			}
			if projectsRecords[i].ApprovedBy != nil && *projectsRecords[i].ApprovedBy != "" {
				if val, ok := approved_by_phone_numberLookupMap[*projectsRecords[i].ApprovedBy]; ok {
					projectsRecords[i].ApprovedByPhoneNumber = &val
				}
			}
		}

		var computedProject []Project
		for _, r := range projectsRecords {
			computedProject = append(computedProject, r)
		}

		if err := SaveProjectRecords(projectsOutput, computedProject); err != nil {
			errMsg := fmt.Sprintf("Projects: failed to save - %v", err)
			fmt.Fprintf(os.Stderr, "ERROR: %s\n", errMsg)
			errors = append(errors, errMsg)
		} else {
			fmt.Printf("  ✓ projects: %d records processed\n", len(computedProject))
			totalRecords += len(computedProject)
		}
		projectsData = computedProject
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
	fmt.Printf("Golang substrate: ALL %d tables processed successfully (%d total records)\n", 5, totalRecords)
	fmt.Println("════════════════════════════════════════════════════════════════")
}