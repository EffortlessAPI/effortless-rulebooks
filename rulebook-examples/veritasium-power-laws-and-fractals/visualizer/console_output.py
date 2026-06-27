#!/usr/bin/env python3
"""
Shared Console Output Library for Power Laws & Fractals

Provides unified, beautiful console visualization for all test runners:
- Python, PostgreSQL, and Go (via subprocess)

Features:
- All 8 iterations per system (actual + projected with color coding)
- ASCII log-log plots with theoretical slope lines
- Validation summaries
- Consistent formatting across platforms
"""

import json
import math
from pathlib import Path
from datetime import datetime, timezone
from typing import Dict, List, Tuple, Optional, Any

# ANSI colors
GREEN = '\033[92m'
YELLOW = '\033[93m'
CYAN = '\033[96m'
RED = '\033[91m'
DIM = '\033[2m'
RESET = '\033[0m'
BOLD = '\033[1m'
MAGENTA = '\033[95m'
BLUE = '\033[94m'
WHITE = '\033[97m'

# ASCII plot characters
PLOT_CHARS = {
    'actual': '●',
    'projected': '◌',
    'theoretical': '·',
}


def render_ascii_plot(scales: List[Dict], system: Dict, width: int = 50, height: int = 12) -> str:
    """
    Render an ASCII log-log plot for a system's scale data.
    
    Args:
        scales: List of scale dictionaries with LogScale and LogMeasure
        system: System dictionary with TheoreticalLogLogSlope
        width: Plot width in characters
        height: Plot height in lines
    
    Returns:
        Multi-line string containing the ASCII plot
    """
    if not scales:
        return "  (No data)"
    
    # Get data points
    points = []
    for s in scales:
        log_scale = s.get('LogScale')
        log_measure = s.get('LogMeasure')
        is_projected = s.get('IsProjected', False)
        if log_scale is not None and log_measure is not None:
            points.append((log_scale, log_measure, is_projected))
    
    if not points:
        return "  (No valid data points)"
    
    # Calculate bounds
    x_vals = [p[0] for p in points]
    y_vals = [p[1] for p in points]
    
    x_min, x_max = min(x_vals), max(x_vals)
    y_min, y_max = min(y_vals), max(y_vals)
    
    # Add padding
    x_range = x_max - x_min if x_max != x_min else 1
    y_range = y_max - y_min if y_max != y_min else 1
    
    # Create plot grid
    grid = [[' ' for _ in range(width)] for _ in range(height)]
    
    # Map coordinates to grid
    def to_grid(x, y):
        gx = int((x - x_min) / x_range * (width - 1)) if x_range else width // 2
        gy = height - 1 - int((y - y_min) / y_range * (height - 1)) if y_range else height // 2
        return max(0, min(width - 1, gx)), max(0, min(height - 1, gy))
    
    # Draw theoretical slope line
    slope = system.get('TheoreticalLogLogSlope', 0)
    if slope != 0:
        # Line passes through first point with given slope
        x0, y0 = x_vals[0], y_vals[0]
        for i in range(width):
            x = x_min + (i / (width - 1)) * x_range
            y = y0 + slope * (x - x0)
            if y_min <= y <= y_max:
                gx, gy = to_grid(x, y)
                if grid[gy][gx] == ' ':
                    grid[gy][gx] = f'{DIM}{PLOT_CHARS["theoretical"]}{RESET}'
    
    # Plot data points (actual first, then projected on top)
    for x, y, is_projected in sorted(points, key=lambda p: p[2]):
        gx, gy = to_grid(x, y)
        if is_projected:
            grid[gy][gx] = f'{MAGENTA}{PLOT_CHARS["projected"]}{RESET}'
        else:
            grid[gy][gx] = f'{GREEN}{PLOT_CHARS["actual"]}{RESET}'
    
    # Build output
    lines = []
    
    # Y-axis label
    lines.append(f"  {DIM}log(Measure){RESET}")
    
    # Top y value
    lines.append(f"  {y_max:>7.2f} ┤")
    
    # Grid rows
    for i, row in enumerate(grid):
        prefix = "        │" if i != len(grid) - 1 else f"  {y_min:>7.2f} ┤"
        lines.append(prefix + ''.join(row))
    
    # X-axis
    lines.append(f"         └{'─' * width}")
    lines.append(f"         {x_min:<7.2f}{' ' * (width - 14)}{x_max:>7.2f}")
    lines.append(f"  {DIM}{'log(Scale)':^{width + 9}}{RESET}")
    
    # Legend
    lines.append(f"  {GREEN}●{RESET} Actual   {MAGENTA}◌{RESET} Projected   {DIM}·{RESET} Theoretical (slope={slope})")
    
    return '\n'.join(lines)


def print_system_table(scales: List[Dict], system: Dict, show_all_columns: bool = True):
    """
    Print a colored table for a system's scales.
    
    Args:
        scales: List of scale dictionaries
        system: System dictionary
        show_all_columns: If True, show extended columns including ScaleFactorPower
    """
    icon = "🔺" if system.get('Class') == "fractal" else "📈"
    
    print(f"\n{icon} {BOLD}{system.get('DisplayName', system.get('SystemID', 'Unknown'))}{RESET}")
    print(f"  {DIM}Theoretical slope: {system.get('TheoreticalLogLogSlope', 'N/A')}{RESET}")
    
    # Header
    if show_all_columns:
        print(f"\n  {'Iter':>4}  {'Measure':>12}  {'Scale':>14}  {'LogScale':>10}  {'LogMeasure':>12}  {'Type':>10}")
        print(f"  {'─' * 70}")
    else:
        print(f"\n  {'Iter':>4}  {'Measure':>12}  {'Scale':>12}  {'LogScale':>10}  {'LogMeasure':>12}")
        print(f"  {'─' * 54}")
    
    # Data rows with colors
    for s in sorted(scales, key=lambda x: x.get('Iteration', 0)):
        is_proj = s.get('IsProjected', False)
        color = MAGENTA if is_proj else GREEN
        marker = "◌" if is_proj else "●"
        type_label = "projected" if is_proj else "actual"
        
        measure = s.get('Measure', 0)
        scale = s.get('Scale', 0)
        log_scale = s.get('LogScale', 0)
        log_measure = s.get('LogMeasure', 0)
        iteration = s.get('Iteration', 0)
        
        if show_all_columns:
            print(f"  {color}{iteration:>4}  {measure:>12.6f}  {scale:>14.8f}  {log_scale:>10.5f}  {log_measure:>12.5f}  {marker} {type_label}{RESET}")
        else:
            print(f"  {color}{iteration:>4}  {measure:>12.6f}  {scale:>12.8f}  {log_scale:>10.5f}  {log_measure:>12.5f}{RESET}")
    
    print(f"\n  {DIM}Row count: {len(scales)}{RESET}")


def print_validation_results(pass_count: int, fail_count: int, failures: List[Tuple]):
    """
    Print validation results summary.
    
    Args:
        pass_count: Number of passing scales
        fail_count: Number of failing scales
        failures: List of (scale_id, mismatches) tuples
    """
    print(f"\n{'=' * 80}")
    print(f"{CYAN}Validation Results (projected scales vs answer-key):{RESET}")
    print(f"{'─' * 80}")
    
    if fail_count == 0:
        print(f"  {GREEN}✓ All {pass_count} projected scales validated successfully!{RESET}")
    else:
        print(f"  {YELLOW}⚠ {pass_count} passed, {fail_count} failed{RESET}")
        for scale_id, mismatches in failures[:5]:
            print(f"    • {scale_id}:")
            if isinstance(mismatches, list):
                for m in mismatches:
                    print(f"      - {m}")
            else:
                print(f"      - {mismatches}")


def print_summary(all_scales: List[Dict], systems_count: int, platform: str):
    """
    Print final summary statistics.
    
    Args:
        all_scales: All scale dictionaries
        systems_count: Number of systems
        platform: Platform name (python, postgres, golang)
    """
    total_scales = len(all_scales)
    actual_count = sum(1 for s in all_scales if not s.get('IsProjected', False))
    projected_count = sum(1 for s in all_scales if s.get('IsProjected', False))
    
    print(f"\n{'=' * 80}")
    print(f"  {BOLD}Summary:{RESET}")
    print(f"    Systems: {systems_count}")
    print(f"    Total scales: {total_scales} ({total_scales // systems_count if systems_count else 0} per system)")
    print(f"    Actual (0-3): {actual_count}")
    print(f"    Projected (4-7): {projected_count}")
    print(f"{'=' * 80}")
    
    icons = {'python': '🐍', 'postgres': '🐘', 'golang': '🐹'}
    names = {'python': 'Python', 'postgres': 'PostgreSQL', 'golang': 'Go'}
    
    print(f"  {GREEN}✓ {names.get(platform, platform)} test run complete!{RESET}")
    print(f"{'=' * 80}\n")


def print_full_report(
    platform: str,
    all_scales: List[Dict],
    systems: Dict[str, Dict],
    pass_count: int,
    fail_count: int,
    failures: List[Tuple],
    show_plots: bool = True
):
    """
    Print the complete test report with all visualizations.
    
    Args:
        platform: Platform name (python, postgres, golang)
        all_scales: All scale dictionaries (base + test)
        systems: Systems lookup dictionary {system_id: system_dict}
        pass_count: Validation pass count
        fail_count: Validation fail count
        failures: List of validation failures
        show_plots: Whether to show ASCII plots
    """
    icons = {'python': '🐍', 'postgres': '🐘', 'golang': '🐹'}
    names = {'python': 'Python', 'postgres': 'PostgreSQL', 'golang': 'Go'}
    
    icon = icons.get(platform, '⚙️')
    name = names.get(platform, platform)
    
    print(f"\n{'=' * 80}")
    print(f"  {BOLD}{icon} POWER LAWS & FRACTALS - {name} Test Runner{RESET}")
    print(f"{'=' * 80}")
    
    print(f"\n{CYAN}All Computed Values (from {name}):{RESET}")
    print(f"  {GREEN}● Green{RESET} = Actual Data (iterations 0-3)")
    print(f"  {MAGENTA}◌ Magenta{RESET} = Projected/Computed (iterations 4-7)")
    print(f"{'─' * 80}")
    
    # Group scales by system
    by_system = {}
    for scale in all_scales:
        sys_id = scale.get('System')
        if sys_id not in by_system:
            by_system[sys_id] = []
        by_system[sys_id].append(scale)
    
    # Print each system
    for system_id in sorted(by_system.keys()):
        scales = by_system[system_id]
        system = systems.get(system_id, {'SystemID': system_id})
        
        # Print table
        print_system_table(scales, system)
        
        # Print ASCII plot
        if show_plots:
            print(f"\n{CYAN}  Log-Log Plot:{RESET}")
            plot = render_ascii_plot(scales, system)
            print(plot)
    
    # Print validation
    print_validation_results(pass_count, fail_count, failures)
    
    # Print summary
    print_summary(all_scales, len(by_system), platform)


def load_json(path: Path) -> Optional[Dict]:
    """Load JSON file safely"""
    try:
        with open(path, 'r') as f:
            return json.load(f)
    except Exception:
        return None


def merge_scales(base_scales: List[Dict], test_scales: List[Dict]) -> List[Dict]:
    """
    Merge base scales and test scales into a single list.
    
    Args:
        base_scales: Scales from base-data.json (iterations 0-3)
        test_scales: Scales from computed results (iterations 4-7)
    
    Returns:
        Combined list of all scales
    """
    # Create lookup for test scales
    test_by_id = {s.get('ScaleID'): s for s in test_scales}
    
    # Start with base scales
    all_scales = list(base_scales)
    
    # Add test scales that aren't already in base
    for scale_id, scale in test_by_id.items():
        if not any(s.get('ScaleID') == scale_id for s in all_scales):
            all_scales.append(scale)
    
    return all_scales



