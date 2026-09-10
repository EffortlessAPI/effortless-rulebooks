// ERB SDK - Go Test Runner (GENERATED - DO NOT EDIT)
// ===================================================
// Loads every table's blank tests from $ERB_TESTING_DIR/blank-tests, computes
// every lookup, aggregation and calculated field, and writes the answers to
// $ERB_TESTING_DIR/$ERB_SUBSTRATE_NAME/test-answers. See erbRun in erb_runtime.go.

package main

func main() {
	erbRun(erbTables, erbClosures)
}
