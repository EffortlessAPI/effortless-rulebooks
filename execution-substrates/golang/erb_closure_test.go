package main

import (
	"encoding/json"
	"os"
	"path/filepath"
	"sort"
	"testing"
)

// The contract mirrors the Postgres cycle-safe WITH RECURSIVE view
// vw_<entity>_closure: hop_distance is the shortest derivation and
// is_inferred is true iff no directly-asserted hop-1 edge states the pair.
// Run: cd execution-substrates/golang && go test ./...

func key(r ClosureRow) [4]interface{} {
	return [4]interface{}{r.FromID, r.ToID, r.HopDistance, r.IsInferred}
}

func normalize(rows []ClosureRow) [][4]interface{} {
	out := make([][4]interface{}, 0, len(rows))
	for _, r := range rows {
		out = append(out, key(r))
	}
	sort.Slice(out, func(i, j int) bool {
		a, b := out[i], out[j]
		if a[0].(string) != b[0].(string) {
			return a[0].(string) < b[0].(string)
		}
		return a[1].(string) < b[1].(string)
	})
	return out
}

func assertRows(t *testing.T, got []ClosureRow, want [][4]interface{}) {
	t.Helper()
	g := normalize(got)
	if len(g) != len(want) {
		t.Fatalf("row count: got %d, want %d (got=%v)", len(g), len(want), g)
	}
	for i := range g {
		if g[i] != want[i] {
			t.Errorf("row %d: got %v, want %v", i, g[i], want[i])
		}
	}
}

// --- self-referential FK shape -------------------------------------------

func TestSelfRefSimpleChainInfersTransitivePair(t *testing.T) {
	edges := []ClosureEdge{{From: "A", To: "B"}, {From: "B", To: "C"}}
	assertRows(t, ComputeClosureRelation(edges), [][4]interface{}{
		{"A", "B", 1, false},
		{"A", "C", 2, true},
		{"B", "C", 1, false},
	})
}

func TestEmptyEndpointsAreNotEdges(t *testing.T) {
	// The transpiler stores absent relationships as "" rather than NULL.
	edges := []ClosureEdge{{From: "A", To: ""}, {From: "", To: "B"}}
	if got := ComputeClosureRelation(edges); len(got) != 0 {
		t.Fatalf("expected no rows, got %v", got)
	}
}

func TestCycleTerminatesAndIsCycleSafe(t *testing.T) {
	edges := []ClosureEdge{{From: "A", To: "B"}, {From: "B", To: "A"}}
	assertRows(t, ComputeClosureRelation(edges), [][4]interface{}{
		{"A", "A", 2, true},
		{"A", "B", 1, false},
		{"B", "A", 1, false},
		{"B", "B", 2, true},
	})
}

func TestSelfLoopTerminates(t *testing.T) {
	edges := []ClosureEdge{{From: "A", To: "A"}}
	assertRows(t, ComputeClosureRelation(edges), [][4]interface{}{
		{"A", "A", 1, false},
	})
}

// --- edge-table shape -----------------------------------------------------

func TestFiveNodeChainYieldsTenPairs(t *testing.T) {
	edges := []ClosureEdge{
		{From: "1", To: "2"}, {From: "2", To: "3"},
		{From: "3", To: "4"}, {From: "4", To: "5"},
	}
	got := ComputeClosureRelation(edges)
	if len(got) != 10 {
		t.Fatalf("expected 10 pairs, got %d", len(got))
	}
	var asserted, inferred int
	for _, r := range got {
		if r.IsInferred {
			inferred++
		} else {
			asserted++
		}
		// The headline never-asserted inference.
		if r.FromID == "1" && r.ToID == "5" {
			if r.HopDistance != 4 || !r.IsInferred {
				t.Errorf("1->5: got hop=%d inferred=%v, want hop=4 inferred=true",
					r.HopDistance, r.IsInferred)
			}
		}
	}
	if asserted != 4 || inferred != 6 {
		t.Errorf("got %d asserted / %d inferred, want 4/6", asserted, inferred)
	}
}

func TestDiamondTakesShortestHopDistance(t *testing.T) {
	edges := []ClosureEdge{
		{From: "A", To: "B"}, {From: "A", To: "C"},
		{From: "B", To: "D"}, {From: "C", To: "D"},
		{From: "A", To: "D"},
	}
	for _, r := range ComputeClosureRelation(edges) {
		if r.FromID == "A" && r.ToID == "D" {
			if r.HopDistance != 1 || r.IsInferred {
				t.Errorf("A->D: got hop=%d inferred=%v, want hop=1 inferred=false",
					r.HopDistance, r.IsInferred)
			}
		}
	}
}

func TestAssertedPairAlsoReachableTransitivelyIsNotInferred(t *testing.T) {
	edges := []ClosureEdge{
		{From: "A", To: "B"}, {From: "B", To: "C"}, {From: "A", To: "C"},
	}
	for _, r := range ComputeClosureRelation(edges) {
		if r.FromID == "A" && r.ToID == "C" && (r.HopDistance != 1 || r.IsInferred) {
			t.Errorf("A->C: got hop=%d inferred=%v, want hop=1 inferred=false",
				r.HopDistance, r.IsInferred)
		}
	}
}

func TestDisconnectedFragmentsDoNotCross(t *testing.T) {
	edges := []ClosureEdge{{From: "A", To: "B"}, {From: "X", To: "Y"}}
	assertRows(t, ComputeClosureRelation(edges), [][4]interface{}{
		{"A", "B", 1, false},
		{"X", "Y", 1, false},
	})
}

func TestEmptyEdgeSet(t *testing.T) {
	if got := ComputeClosureRelation(nil); len(got) != 0 {
		t.Fatalf("expected no rows, got %v", got)
	}
}

// --- golden files dumped from the Postgres oracle -------------------------

type goldenRow struct {
	FromID      string `json:"from_id"`
	ToID        string `json:"to_id"`
	HopDistance int    `json:"hop_distance"`
	IsInferred  bool   `json:"is_inferred"`
}

func loadGolden(t *testing.T, view string) [][4]interface{} {
	t.Helper()
	path := filepath.Join("..", "..", "testing", "closure-golden", view+".json")
	raw, err := os.ReadFile(path)
	if err != nil {
		t.Fatalf("read golden %s: %v", view, err)
	}
	var rows []goldenRow
	if err := json.Unmarshal(raw, &rows); err != nil {
		t.Fatalf("parse golden %s: %v", view, err)
	}
	out := make([][4]interface{}, 0, len(rows))
	for _, r := range rows {
		out = append(out, [4]interface{}{r.FromID, r.ToID, r.HopDistance, r.IsInferred})
	}
	sort.Slice(out, func(i, j int) bool {
		a, b := out[i], out[j]
		if a[0].(string) != b[0].(string) {
			return a[0].(string) < b[0].(string)
		}
		return a[1].(string) < b[1].(string)
	})
	return out
}

func TestStepPrecedenceClosureMatchesPostgres(t *testing.T) {
	// 1->2->3->4->5 as asserted in Talisman's StepPrecedence rows.
	edges := []ClosureEdge{
		{From: "prod-deploy-step-1", To: "prod-deploy-step-2"},
		{From: "prod-deploy-step-2", To: "prod-deploy-step-3"},
		{From: "prod-deploy-step-3", To: "prod-deploy-step-4"},
		{From: "prod-deploy-step-4", To: "prod-deploy-step-5"},
	}
	assertRows(t, ComputeClosureRelation(edges),
		loadGolden(t, "vw_step_precedence_closure"))
}

func TestRolesClosureMatchesPostgres(t *testing.T) {
	edges := []ClosureEdge{
		{From: "ntwf-release-manager-role", To: "ntwf-vp-engineering-role"},
		{From: "ntwf-vp-engineering-role", To: "ntwf-cto-role"},
	}
	assertRows(t, ComputeClosureRelation(edges), loadGolden(t, "vw_roles_closure"))
}

func TestWorkflowArtifactsClosureMatchesPostgres(t *testing.T) {
	edges := []ClosureEdge{
		{From: "artifact-legal-clearance", To: "artifact-risk-report"},
		{From: "artifact-release-authorization", To: "artifact-legal-clearance"},
		{From: "artifact-deployment-log", To: "artifact-release-authorization"},
		{From: "artifact-post-deploy-report", To: "artifact-deployment-log"},
	}
	assertRows(t, ComputeClosureRelation(edges),
		loadGolden(t, "vw_workflow_artifacts_closure"))
}
