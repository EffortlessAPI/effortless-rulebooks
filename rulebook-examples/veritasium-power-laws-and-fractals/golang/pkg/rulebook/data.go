//
// Data Loading Utilities
//
// Loads test data from JSON files following the unified testing protocol
//

package rulebook

import (
	"encoding/json"
	"os"
)

// BaseData represents the structure of base-data.json
type BaseData struct {
	Description string    `json:"description"`
	Generated   string    `json:"generated"`
	Source      string    `json:"source"`
	Systems     []System  `json:"systems"`
	Scales      []Scale   `json:"scales"`
}

// TestInput represents the structure of test-input.json
type TestInput struct {
	Description string  `json:"description"`
	Generated   string  `json:"generated"`
	Source      string  `json:"source"`
	Scales      []Scale `json:"scales"`
}

// AnswerKey represents the structure of answer-key.json
type AnswerKey struct {
	Description string                   `json:"description"`
	Generated   string                   `json:"generated"`
	Source      string                   `json:"source"`
	Scales      []map[string]interface{} `json:"scales"`
}

// TestResults represents the output format
type TestResults struct {
	Platform  string                   `json:"platform"`
	Timestamp string                   `json:"timestamp"`
	Scales    []map[string]interface{} `json:"scales"`
}

// LoadBaseData loads base-data.json
func LoadBaseData(path string) (*BaseData, error) {
	data, err := os.ReadFile(path)
	if err != nil {
		return nil, err
	}
	
	var baseData BaseData
	err = json.Unmarshal(data, &baseData)
	if err != nil {
		return nil, err
	}
	
	return &baseData, nil
}

// LoadTestInput loads test-input.json
func LoadTestInput(path string) (*TestInput, error) {
	data, err := os.ReadFile(path)
	if err != nil {
		return nil, err
	}
	
	var testInput TestInput
	err = json.Unmarshal(data, &testInput)
	if err != nil {
		return nil, err
	}
	
	return &testInput, nil
}

// LoadAnswerKey loads answer-key.json
func LoadAnswerKey(path string) (*AnswerKey, error) {
	data, err := os.ReadFile(path)
	if err != nil {
		return nil, err
	}
	
	var answerKey AnswerKey
	err = json.Unmarshal(data, &answerKey)
	if err != nil {
		return nil, err
	}
	
	return &answerKey, nil
}

// SaveResults saves results to JSON file
func SaveResults(path string, results *TestResults) error {
	data, err := json.MarshalIndent(results, "", "  ")
	if err != nil {
		return err
	}
	
	return os.WriteFile(path, data, 0644)
}

// BuildSystemsMap creates a lookup map from systems slice
func BuildSystemsMap(systems []System) SystemsMap {
	m := make(SystemsMap)
	for i := range systems {
		m[systems[i].SystemID] = &systems[i]
	}
	return m
}
