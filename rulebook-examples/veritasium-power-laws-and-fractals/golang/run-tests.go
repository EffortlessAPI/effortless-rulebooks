// Power Laws & Fractals - Go Test Runner
//
// Follows the unified testing protocol:
// 1. Load base-data.json (for systems configuration + base scales)
// 2. Load test-input.json (raw facts only)
// 3. Compute derived values for test scales
// 4. Output results to test-results/golang-results.json
// 5. Validate against answer-key.json
// 6. Display with unified visualization (all 8 iterations, colors, ASCII plots)

package main

import (
	"fmt"
	"os"
	"path/filepath"
	"sort"
	"strings"

	"erb-power-laws/pkg/rulebook"
)

// ANSI colors
const (
	green   = "\033[92m"
	yellow  = "\033[93m"
	cyan    = "\033[96m"
	red     = "\033[91m"
	dim     = "\033[2m"
	reset   = "\033[0m"
	bold    = "\033[1m"
	magenta = "\033[95m"
)

// Plot characters
const (
	plotActual      = "●"
	plotProjected   = "◌"
	plotTheoretical = "·"
)

func main() {
	// Find project root (parent of golang directory)
	execPath, _ := os.Getwd()
	projectRoot := filepath.Dir(execPath)

	// If we're already at project root (golang is a subdirectory)
	if _, err := os.Stat(filepath.Join(execPath, "test-data")); err == nil {
		projectRoot = execPath
	}

	// Paths
	testDataDir := filepath.Join(projectRoot, "test-data")
	testResultsDir := filepath.Join(projectRoot, "test-results")

	baseDataPath := filepath.Join(testDataDir, "base-data.json")
	testInputPath := filepath.Join(testDataDir, "test-input.json")
	answerKeyPath := filepath.Join(testDataDir, "answer-key.json")
	resultsPath := filepath.Join(testResultsDir, "golang-results.json")

	// Ensure results directory exists
	os.MkdirAll(testResultsDir, 0755)

	// Load base data
	baseData, err := rulebook.LoadBaseData(baseDataPath)
	if err != nil {
		fmt.Printf("%sError: Could not load base-data.json: %v%s\n", red, err, reset)
		os.Exit(1)
	}

	// Load test input
	testInput, err := rulebook.LoadTestInput(testInputPath)
	if err != nil {
		fmt.Printf("%sError: Could not load test-input.json: %v%s\n", red, err, reset)
		os.Exit(1)
	}

	// Load answer key
	answerKey, err := rulebook.LoadAnswerKey(answerKeyPath)
	if err != nil {
		fmt.Printf("%sError: Could not load answer-key.json: %v%s\n", red, err, reset)
		os.Exit(1)
	}

	// Build systems map
	systemsMap := rulebook.BuildSystemsMap(baseData.Systems)

	// Compute derived values for test scales
	computedTestScales := make([]map[string]interface{}, 0, len(testInput.Scales))

	for i := range testInput.Scales {
		scale := &testInput.Scales[i]
		scale.CalculateAllFields(systemsMap)
		computedTestScales = append(computedTestScales, scale.ToOutputMap())
	}

	// Save results (test scales only for validation)
	results := &rulebook.TestResults{
		Platform: "golang",
		Scales:   computedTestScales,
	}

	err = rulebook.SaveResults(resultsPath, results)
	if err != nil {
		fmt.Printf("%sError: Could not save results: %v%s\n", red, err, reset)
		os.Exit(1)
	}

	// Merge base scales with computed test scales for full visualization
	allScales := mergeScales(baseData.Scales, computedTestScales, systemsMap)

	// Validate against answer key
	passCount, failCount, failures := rulebook.ValidateAllScales(computedTestScales, answerKey)

	// Print full report
	printFullReport(systemsMap, allScales, passCount, failCount, failures)

	// Exit with appropriate code
	if failCount > 0 {
		os.Exit(1)
	}
}

// mergeScales combines base scales with computed test scales
func mergeScales(baseScales []rulebook.Scale, testScales []map[string]interface{}, systems rulebook.SystemsMap) []map[string]interface{} {
	// Convert base scales to maps
	all := make([]map[string]interface{}, 0, len(baseScales)+len(testScales))

	for i := range baseScales {
		scale := &baseScales[i]
		scale.CalculateAllFields(systems)
		all = append(all, scale.ToOutputMap())
	}

	// Add test scales
	all = append(all, testScales...)

	return all
}

// renderASCIIPlot creates an ASCII log-log plot
func renderASCIIPlot(scales []map[string]interface{}, system *rulebook.System, width, height int) string {
	if len(scales) == 0 {
		return "  (No data)"
	}

	// Extract points
	type point struct {
		x, y        float64
		isProjected bool
	}

	var points []point
	for _, s := range scales {
		logScale, ok1 := s["LogScale"].(float64)
		logMeasure, ok2 := s["LogMeasure"].(float64)
		isProj, _ := s["IsProjected"].(bool)
		if ok1 && ok2 {
			points = append(points, point{logScale, logMeasure, isProj})
		}
	}

	if len(points) == 0 {
		return "  (No valid data points)"
	}

	// Calculate bounds
	xMin, xMax := points[0].x, points[0].x
	yMin, yMax := points[0].y, points[0].y
	for _, p := range points {
		if p.x < xMin {
			xMin = p.x
		}
		if p.x > xMax {
			xMax = p.x
		}
		if p.y < yMin {
			yMin = p.y
		}
		if p.y > yMax {
			yMax = p.y
		}
	}

	xRange := xMax - xMin
	if xRange == 0 {
		xRange = 1
	}
	yRange := yMax - yMin
	if yRange == 0 {
		yRange = 1
	}

	// Create grid
	grid := make([][]string, height)
	for i := range grid {
		grid[i] = make([]string, width)
		for j := range grid[i] {
			grid[i][j] = " "
		}
	}

	// Map to grid coordinates
	toGrid := func(x, y float64) (int, int) {
		gx := int((x - xMin) / xRange * float64(width-1))
		gy := height - 1 - int((y-yMin)/yRange*float64(height-1))
		if gx < 0 {
			gx = 0
		}
		if gx >= width {
			gx = width - 1
		}
		if gy < 0 {
			gy = 0
		}
		if gy >= height {
			gy = height - 1
		}
		return gx, gy
	}

	// Draw theoretical slope line
	slope := system.TheoreticalLogLogSlope
	if slope != 0 {
		x0, y0 := points[0].x, points[0].y
		for i := 0; i < width; i++ {
			x := xMin + (float64(i)/float64(width-1))*xRange
			y := y0 + slope*(x-x0)
			if y >= yMin && y <= yMax {
				gx, gy := toGrid(x, y)
				if grid[gy][gx] == " " {
					grid[gy][gx] = dim + plotTheoretical + reset
				}
			}
		}
	}

	// Sort: actual first, then projected (so projected overlays)
	sort.Slice(points, func(i, j int) bool {
		return !points[i].isProjected && points[j].isProjected
	})

	// Plot points
	for _, p := range points {
		gx, gy := toGrid(p.x, p.y)
		if p.isProjected {
			grid[gy][gx] = magenta + plotProjected + reset
		} else {
			grid[gy][gx] = green + plotActual + reset
		}
	}

	// Build output
	var lines []string

	lines = append(lines, fmt.Sprintf("  %slog(Measure)%s", dim, reset))
	lines = append(lines, fmt.Sprintf("  %7.2f ┤", yMax))

	for i, row := range grid {
		prefix := "        │"
		if i == len(grid)-1 {
			prefix = fmt.Sprintf("  %7.2f ┤", yMin)
		}
		lines = append(lines, prefix+strings.Join(row, ""))
	}

	lines = append(lines, fmt.Sprintf("         └%s", strings.Repeat("─", width)))
	lines = append(lines, fmt.Sprintf("         %-7.2f%s%7.2f", xMin, strings.Repeat(" ", width-14), xMax))
	lines = append(lines, fmt.Sprintf("  %s%s%s", dim, center("log(Scale)", width+9), reset))
	lines = append(lines, fmt.Sprintf("  %s●%s Actual   %s◌%s Projected   %s·%s Theoretical (slope=%.3f)",
		green, reset, magenta, reset, dim, reset, slope))

	return strings.Join(lines, "\n")
}

func center(s string, width int) string {
	if len(s) >= width {
		return s
	}
	padding := (width - len(s)) / 2
	return strings.Repeat(" ", padding) + s + strings.Repeat(" ", width-len(s)-padding)
}

func printSystemTable(scales []map[string]interface{}, system *rulebook.System) {
	icon := "📈"
	if system != nil && system.Class == "fractal" {
		icon = "🔺"
	}

	displayName := system.SystemID
	if system != nil {
		displayName = system.DisplayName
	}

	fmt.Printf("\n%s %s%s%s\n", icon, bold, displayName, reset)
	fmt.Printf("  %sTheoretical slope: %.3f%s\n", dim, system.TheoreticalLogLogSlope, reset)

	fmt.Printf("\n  %4s  %12s  %14s  %10s  %12s  %10s\n", "Iter", "Measure", "Scale", "LogScale", "LogMeasure", "Type")
	fmt.Println("  " + strings.Repeat("─", 70))

	// Sort by iteration
	sort.Slice(scales, func(i, j int) bool {
		return scales[i]["Iteration"].(int) < scales[j]["Iteration"].(int)
	})

	for _, s := range scales {
		isProj, _ := s["IsProjected"].(bool)
		color := green
		marker := "●"
		typeLabel := "actual"
		if isProj {
			color = magenta
			marker = "◌"
			typeLabel = "projected"
		}

		fmt.Printf("  %s%4d  %12.6f  %14.8f  %10.5f  %12.5f  %s %s%s\n",
			color,
			s["Iteration"].(int),
			s["Measure"].(float64),
			s["Scale"].(float64),
			s["LogScale"].(float64),
			s["LogMeasure"].(float64),
			marker,
			typeLabel,
			reset)
	}

	fmt.Printf("\n  %sRow count: %d%s\n", dim, len(scales), reset)
}

func printFullReport(systems rulebook.SystemsMap, allScales []map[string]interface{},
	passCount, failCount int, failures []rulebook.ValidationResult) {

	fmt.Printf("\n%s================================================================================\n", bold)
	fmt.Printf("  🐹 POWER LAWS & FRACTALS - Go Test Runner%s\n", reset)
	fmt.Printf("%s================================================================================\n", reset)

	fmt.Printf("\n%sAll Computed Values (from Go):%s\n", cyan, reset)
	fmt.Printf("  %s●%s Green = Actual Data (iterations 0-3)\n", green, reset)
	fmt.Printf("  %s◌%s Magenta = Projected/Computed (iterations 4-7)\n", magenta, reset)
	fmt.Println(strings.Repeat("─", 80))

	// Group scales by system
	bySystem := make(map[string][]map[string]interface{})
	for _, scale := range allScales {
		systemID := scale["System"].(string)
		bySystem[systemID] = append(bySystem[systemID], scale)
	}

	// Get sorted system IDs
	systemIDs := make([]string, 0, len(bySystem))
	for id := range bySystem {
		systemIDs = append(systemIDs, id)
	}
	sort.Strings(systemIDs)

	for _, systemID := range systemIDs {
		scales := bySystem[systemID]
		system := systems[systemID]

		// Print table
		printSystemTable(scales, system)

		// Print ASCII plot
		fmt.Printf("\n%s  Log-Log Plot:%s\n", cyan, reset)
		plot := renderASCIIPlot(scales, system, 50, 12)
		fmt.Println(plot)
	}

	// Validation results
	fmt.Printf("\n%s================================================================================\n", reset)
	fmt.Printf("%sValidation Results (projected scales vs answer-key):%s\n", cyan, reset)
	fmt.Println(strings.Repeat("─", 80))

	if failCount == 0 {
		fmt.Printf("  %s✓ All %d projected scales validated successfully!%s\n", green, passCount, reset)
	} else {
		fmt.Printf("  %s⚠ %d passed, %d failed%s\n", yellow, passCount, failCount, reset)
		for i, failure := range failures {
			if i >= 5 {
				break
			}
			fmt.Printf("    • %s:\n", failure.ScaleID)
			for _, m := range failure.Mismatches {
				fmt.Printf("      - %s\n", m)
			}
		}
	}

	// Summary
	totalScales := len(allScales)
	actualCount := 0
	projectedCount := 0
	for _, s := range allScales {
		if isProj, ok := s["IsProjected"].(bool); ok && isProj {
			projectedCount++
		} else {
			actualCount++
		}
	}

	fmt.Printf("\n%s================================================================================\n", reset)
	fmt.Printf("  %sSummary:%s\n", bold, reset)
	fmt.Printf("    Systems: %d\n", len(bySystem))
	fmt.Printf("    Total scales: %d (%d per system)\n", totalScales, totalScales/len(bySystem))
	fmt.Printf("    Actual (0-3): %d\n", actualCount)
	fmt.Printf("    Projected (4-7): %d\n", projectedCount)
	fmt.Println("================================================================================")
	fmt.Printf("  %s✓ Go test run complete!%s\n", green, reset)
	fmt.Println("================================================================================\n")
}
