package main

import "sort"

// Transitive-closure engine for the Go substrate.
//
// HAND-WRITTEN, NOT GENERATED. This is the Go substrate's own native
// implementation of what Postgres expresses as a cycle-safe WITH RECURSIVE
// view (vw_<entity>_closure) and OWL expresses as owl:TransitiveProperty
// resolved by SHACL-SPARQL reasoning. Each substrate must compute closure in
// its own semantics — calling out to another substrate's engine would defeat
// the point of conformance.

// ClosureEdge is one asserted edge. Both shapes in the rulebook reduce to
// this: an edge/junction table supplies FromColumn/ToColumn directly, and a
// self-referential FK supplies the row's own primary key plus the FK column.
type ClosureEdge struct {
	From string
	To   string
}

// ClosureRow mirrors the columns of the Postgres closure view.
type ClosureRow struct {
	FromID      string
	ToID        string
	HopDistance int
	IsInferred  bool
}

// ComputeClosureRelation returns every reachable pair with its shortest
// hop distance, flagging pairs that no directly-asserted edge states.
// An empty endpoint is not an edge: the transpiler stores absent
// relationships as "" rather than NULL, so both must be excluded.
func ComputeClosureRelation(edges []ClosureEdge) []ClosureRow {
	asserted := make(map[ClosureEdge]bool)
	adjacency := make(map[string][]string)
	origins := make([]string, 0, len(edges))

	for _, e := range edges {
		if e.From == "" || e.To == "" {
			continue
		}
		if _, seen := adjacency[e.From]; !seen {
			origins = append(origins, e.From)
		}
		asserted[e] = true
		adjacency[e.From] = append(adjacency[e.From], e.To)
	}

	if len(asserted) == 0 {
		return nil
	}

	type pair struct{ from, to string }
	shortest := make(map[pair]int)

	for _, origin := range origins {
		// Breadth-first, so the first arrival at a node is the shortest
		// derivation; carrying the visited path makes cyclic edge sets
		// terminate instead of looping forever.
		type walk struct {
			node string
			path map[string]bool
		}
		frontier := []walk{{node: origin, path: map[string]bool{origin: true}}}

		for hop := 1; len(frontier) > 0; hop++ {
			var next []walk
			for _, w := range frontier {
				for _, neighbor := range adjacency[w.node] {
					p := pair{origin, neighbor}
					if _, found := shortest[p]; !found {
						shortest[p] = hop
					}
					if w.path[neighbor] {
						continue
					}
					extended := make(map[string]bool, len(w.path)+1)
					for k := range w.path {
						extended[k] = true
					}
					extended[neighbor] = true
					next = append(next, walk{node: neighbor, path: extended})
				}
			}
			frontier = next
		}
	}

	rows := make([]ClosureRow, 0, len(shortest))
	for p, hop := range shortest {
		rows = append(rows, ClosureRow{
			FromID:      p.from,
			ToID:        p.to,
			HopDistance: hop,
			IsInferred:  !asserted[ClosureEdge{From: p.from, To: p.to}],
		})
	}

	sort.Slice(rows, func(i, j int) bool {
		if rows[i].FromID != rows[j].FromID {
			return rows[i].FromID < rows[j].FromID
		}
		return rows[i].ToID < rows[j].ToID
	})

	return rows
}
