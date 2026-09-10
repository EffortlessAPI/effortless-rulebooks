// ERB SDK (GENERATED - DO NOT EDIT)
// ===================================
// Generated from: effortless-rulebook/a1-effortless-init-sample-rulebook.json
//
// One struct per table, a Calc<Field>() method per calculated field, and the
// table registry main.go runs. Formulas compute through erb_runtime.go.

package main

// =============================================================================
// HELLOWHOS TABLE
// The smallest complete rulebook: an id, a display name, and a rule that derives a greeting from it.
// =============================================================================

// HelloWho represents a row in the HelloWhos table
type HelloWho struct {
	HelloWhoId string `json:"hello_who_id"`
	Name string `json:"name"`
	Introduction *string `json:"introduction"`
	IsBob *bool `json:"is_bob"`
	ErbErrors map[string]string `json:"_erb_errors,omitempty"`
	erbAggregates map[string]Value
}

// CalcIntroduction computes the Introduction calculated field
// Formula: ="Hi, " & {{Name}} & "."
func (tc *HelloWho) CalcIntroduction() *string {
	return toStringPtr(erbConcat(vS("Hi, "), erbTextOr(vStrPlain(tc.Name)), vS(".")))
}

// CalcIsBob computes the IsBob calculated field
// Formula: =OR({{Name}}="Bob", {{Name}}="Bobby", {{Name}}="Robert")
func (tc *HelloWho) CalcIsBob() *bool {
	return toBoolPtr(erbOr(erbBool3(erbEq(erbNullif(vStrPlain(tc.Name)), vS("Bob"))), erbBool3(erbEq(erbNullif(vStrPlain(tc.Name)), vS("Bobby"))), erbBool3(erbEq(erbNullif(vStrPlain(tc.Name)), vS("Robert")))))
}

// erbComputeCalculations computes every calculated field of the row in dependency order.
func (tc *HelloWho) erbComputeCalculations() {
	// Level 1
	calcGuard(tc, "introduction", func() { tc.Introduction = tc.CalcIntroduction() })
	calcGuard(tc, "is_bob", func() { tc.IsBob = tc.CalcIsBob() })
}

// ComputeAll computes every calculated field from the row's current inputs.
func (tc *HelloWho) ComputeAll() *HelloWho {
	tc.erbComputeCalculations()
	return tc
}

func (tc *HelloWho) erbGet(field string) Value {
	switch field {
	case "hello_who_id":
		return vStrPlain(tc.HelloWhoId)
	case "name":
		return vStrPlain(tc.Name)
	case "introduction":
		return vStr(tc.Introduction)
	case "is_bob":
		return vBool(tc.IsBob)
	}
	panic("HelloWhos has no field " + field)
}

func (tc *HelloWho) erbSet(field string, v Value) {
	switch field {
	case "hello_who_id":
		tc.HelloWhoId = strPlain(v)
	case "name":
		tc.Name = strPlain(v)
	case "introduction":
		tc.Introduction = toStringPtr(v)
	case "is_bob":
		tc.IsBob = toBoolPtr(v)
	default:
		panic("HelloWhos has no field " + field)
	}
}

func (tc *HelloWho) erbLoad(row map[string]any) {
	for key, value := range row {
		switch key {
		case "hello_who_id", "name", "introduction", "is_bob":
			tc.erbSet(key, fromJSON(value))
		}
	}
}

func (tc *HelloWho) erbErrors() map[string]string {
	if tc.ErbErrors == nil {
		tc.ErbErrors = map[string]string{}
	}
	return tc.ErbErrors
}

func (tc *HelloWho) erbResetErrors() { tc.ErbErrors = nil }

func (tc *HelloWho) erbAggregate(name string) Value { return tc.erbAggregates[name] }

func (tc *HelloWho) erbSetAggregate(name string, v Value) {
	if tc.erbAggregates == nil {
		tc.erbAggregates = map[string]Value{}
	}
	tc.erbAggregates[name] = v
}

// LoadHelloWhoRecords reads HelloWhos rows from a JSON array file.
func LoadHelloWhoRecords(path string) ([]HelloWho, error) {
	records, err := loadRecords(path, func() Record { return &HelloWho{} })
	if err != nil {
		return nil, err
	}
	rows := make([]HelloWho, len(records))
	for i, r := range records {
		rows[i] = *r.(*HelloWho)
	}
	return rows, nil
}

// calculatedFieldCount bounds the runner's passes over the dataset.
const calculatedFieldCount = 2

// erbTables is every table, in rulebook order.
var erbTables = []TableSpec{
	{Name: "HelloWhos", File: "hello_whos", RulebookRows: 4, New: func() Record { return &HelloWho{} },
		Lookups: []LookupSpec{},
		Aggregations: []AggregateSpec{}},
}

// erbClosures materializes each vw_<entity>_closure view aggregations read.
var erbClosures = []ClosureSpec{
}
