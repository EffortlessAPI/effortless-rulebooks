//
// Utility Functions
//
// Validation and comparison utilities for the testing protocol
//

package rulebook

import (
	"fmt"
	"math"
)

// Tolerance for floating point comparisons (allows for floating-point precision in 6dp comparisons)
// Using 0.0000015 to handle rounding at the 6th decimal place boundary
const Tolerance = 0.0000015

// ValidationResult represents the result of validating a scale
type ValidationResult struct {
	ScaleID    string
	Passed     bool
	Mismatches []string
}

// CompareValues compares two values with tolerance for floats
func CompareValues(expected, actual interface{}) bool {
	if expected == nil && actual == nil {
		return true
	}
	if expected == nil || actual == nil {
		return false
	}
	
	// Handle numeric comparisons
	expFloat, expOk := toFloat64(expected)
	actFloat, actOk := toFloat64(actual)
	
	if expOk && actOk {
		return math.Abs(expFloat-actFloat) < Tolerance
	}
	
	// Handle string comparisons
	return fmt.Sprintf("%v", expected) == fmt.Sprintf("%v", actual)
}

// toFloat64 attempts to convert an interface to float64
func toFloat64(v interface{}) (float64, bool) {
	switch val := v.(type) {
	case float64:
		return val, true
	case float32:
		return float64(val), true
	case int:
		return float64(val), true
	case int64:
		return float64(val), true
	default:
		return 0, false
	}
}

// ValidateScale validates a computed scale against expected values
func ValidateScale(computed map[string]interface{}, expected map[string]interface{}) ValidationResult {
	result := ValidationResult{
		ScaleID:    computed["ScaleID"].(string),
		Passed:     true,
		Mismatches: []string{},
	}
	
	computedFields := []string{"BaseScale", "ScaleFactor", "ScaleFactorPower", "Scale", "LogScale", "LogMeasure"}
	
	for _, field := range computedFields {
		expVal := expected[field]
		actVal := computed[field]
		
		if !CompareValues(expVal, actVal) {
			result.Passed = false
			result.Mismatches = append(result.Mismatches, 
				fmt.Sprintf("%s: expected %v, got %v", field, expVal, actVal))
		}
	}
	
	return result
}

// ValidateAllScales validates all computed scales against answer key
func ValidateAllScales(computed []map[string]interface{}, answerKey *AnswerKey) (int, int, []ValidationResult) {
	// Build lookup by ScaleID
	expectedByID := make(map[string]map[string]interface{})
	for _, s := range answerKey.Scales {
		if scaleID, ok := s["ScaleID"].(string); ok {
			expectedByID[scaleID] = s
		}
	}
	
	passCount := 0
	failCount := 0
	failures := []ValidationResult{}
	
	for _, comp := range computed {
		scaleID, _ := comp["ScaleID"].(string)
		expected, found := expectedByID[scaleID]
		
		if !found {
			failCount++
			failures = append(failures, ValidationResult{
				ScaleID:    scaleID,
				Passed:     false,
				Mismatches: []string{"Not found in answer key"},
			})
			continue
		}
		
		result := ValidateScale(comp, expected)
		if result.Passed {
			passCount++
		} else {
			failCount++
			failures = append(failures, result)
		}
	}
	
	return passCount, failCount, failures
}
