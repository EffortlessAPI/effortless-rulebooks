#!/usr/bin/env python3
"""
Cross-Platform Test Results Comparison

Compares results from all platforms against the answer key and generates:
1. Console output with detailed comparison
2. HTML report with interactive visualization
"""

import argparse
import json
from pathlib import Path
from datetime import datetime, timezone
from typing import Dict, List, Tuple

# Paths
SCRIPT_DIR = Path(__file__).parent
PROJECT_ROOT = SCRIPT_DIR.parent
TEST_DATA_DIR = PROJECT_ROOT / 'test-data'
TEST_RESULTS_DIR = PROJECT_ROOT / 'test-results'

# ANSI colors
GREEN = '\033[92m'
YELLOW = '\033[93m'
CYAN = '\033[96m'
RED = '\033[91m'
DIM = '\033[2m'
RESET = '\033[0m'
BOLD = '\033[1m'

# Tolerance for comparisons (allows for floating-point precision in 6dp comparisons)
# Using 0.0000015 to handle rounding at the 6th decimal place boundary
TOLERANCE = 0.0000015

# Platforms to compare
PLATFORMS = ['python', 'postgres', 'golang']


def load_json(path: Path) -> Dict:
    """Load JSON file"""
    with open(path, 'r') as f:
        return json.load(f)


def compare_values(expected, actual, tolerance: float = TOLERANCE) -> bool:
    """Compare two values with tolerance for floats"""
    if expected is None and actual is None:
        return True
    if expected is None or actual is None:
        return False
    if isinstance(expected, (int, float)) and isinstance(actual, (int, float)):
        return abs(expected - actual) < tolerance
    return expected == actual


def load_all_results() -> Dict[str, Dict]:
    """Load results from all platforms"""
    results = {}
    
    for platform in PLATFORMS:
        results_path = TEST_RESULTS_DIR / f'{platform}-results.json'
        if results_path.exists():
            results[platform] = load_json(results_path)
        else:
            results[platform] = None
    
    return results


def compare_platform(platform: str, results: Dict, answer_key: Dict) -> Dict:
    """Compare platform results against answer key (only projected/test scales)"""
    if results is None:
        return {'status': 'not_run', 'pass_count': 0, 'fail_count': 0, 'details': []}
    
    # Only compare projected scales (IsProjected=True, iterations 4-7)
    # These are the scales platforms are tested on
    expected_scales = [s for s in answer_key.get('scales', []) if s.get('IsProjected', False)]
    expected_by_id = {s['ScaleID']: s for s in expected_scales}
    actual_by_id = {s['ScaleID']: s for s in results.get('scales', [])}
    
    computed_fields = ['BaseScale', 'ScaleFactor', 'ScaleFactorPower', 'Scale', 'LogScale', 'LogMeasure']
    
    details = []
    pass_count = 0
    fail_count = 0
    
    for scale_id, expected in expected_by_id.items():
        actual = actual_by_id.get(scale_id)
        
        if actual is None:
            fail_count += 1
            details.append({
                'ScaleID': scale_id,
                'System': expected.get('System'),
                'status': 'missing',
                'fields': {}
            })
            continue
        
        field_results = {}
        all_match = True
        
        for field in computed_fields:
            exp_val = expected.get(field)
            act_val = actual.get(field)
            match = compare_values(exp_val, act_val)
            
            field_results[field] = {
                'expected': exp_val,
                'actual': act_val,
                'match': match
            }
            
            if not match:
                all_match = False
        
        if all_match:
            pass_count += 1
        else:
            fail_count += 1
        
        details.append({
            'ScaleID': scale_id,
            'System': expected.get('System'),
            'Iteration': expected.get('Iteration'),
            'status': 'pass' if all_match else 'fail',
            'fields': field_results
        })
    
    return {
        'status': 'passed' if fail_count == 0 else 'failed',
        'pass_count': pass_count,
        'fail_count': fail_count,
        'details': details
    }


def print_console_output(comparisons: Dict[str, Dict], base_data: Dict):
    """Print comparison results to console"""
    print(f"\n{BOLD}{'=' * 75}{RESET}")
    print(f"{BOLD}  🔬 CROSS-PLATFORM TEST COMPARISON{RESET}")
    print(f"{BOLD}{'=' * 75}{RESET}")
    
    # Summary table
    print(f"\n{CYAN}Platform Summary:{RESET}")
    print(f"{'─' * 50}")
    print(f"  {'Platform':15} {'Status':12} {'Pass':>6} {'Fail':>6}")
    print(f"{'─' * 50}")
    
    all_passed = True
    
    for platform in PLATFORMS:
        comp = comparisons.get(platform, {})
        status = comp.get('status', 'not_run')
        pass_count = comp.get('pass_count', 0)
        fail_count = comp.get('fail_count', 0)
        
        if status == 'passed':
            status_str = f"{GREEN}✓ PASSED{RESET}"
        elif status == 'failed':
            status_str = f"{YELLOW}✗ FAILED{RESET}"
            all_passed = False
        else:
            status_str = f"{DIM}○ NOT RUN{RESET}"
            all_passed = False
        
        print(f"  {platform:15} {status_str:20} {pass_count:>6} {fail_count:>6}")
    
    print(f"{'─' * 50}")
    
    # Show failed scales by system
    systems = {s['SystemID']: s for s in base_data.get('systems', [])}
    
    for platform in PLATFORMS:
        comp = comparisons.get(platform, {})
        failures = [d for d in comp.get('details', []) if d.get('status') == 'fail']
        
        if failures:
            print(f"\n{YELLOW}{platform} Failures:{RESET}")
            
            # Group by system
            by_system = {}
            for f in failures:
                sys_id = f.get('System', 'Unknown')
                if sys_id not in by_system:
                    by_system[sys_id] = []
                by_system[sys_id].append(f)
            
            for sys_id, sys_failures in by_system.items():
                system = systems.get(sys_id, {})
                print(f"  • {system.get('DisplayName', sys_id)}")
                for f in sys_failures[:2]:  # Show first 2 per system
                    print(f"    - {f['ScaleID']}")
                    for field, data in f.get('fields', {}).items():
                        if not data.get('match', True):
                            print(f"      {field}: expected {data['expected']}, got {data['actual']}")
    
    print(f"\n{BOLD}{'=' * 75}{RESET}")
    if all_passed:
        print(f"{GREEN}✓ All platforms validated successfully!{RESET}")
    else:
        print(f"{YELLOW}⚠ Some platforms have failures. See details above.{RESET}")
    print(f"{BOLD}{'=' * 75}{RESET}\n")
    
    return all_passed


def generate_html_report(comparisons: Dict[str, Dict], base_data: Dict, answer_key: Dict):
    """Generate comprehensive HTML report with full data tables and log-log graphs"""
    
    import json as json_module
    
    systems = base_data.get('systems', [])
    all_answer_scales = answer_key.get('scales', [])
    system_lookup = {s['SystemID']: s for s in systems}
    
    # Group all scales by system
    scales_by_system = {}
    for s in all_answer_scales:
        sys_id = s['System']
        if sys_id not in scales_by_system:
            scales_by_system[sys_id] = []
        scales_by_system[sys_id].append(s)
    
    # Build chart data and full data for each system
    chart_data = {}
    full_data = {}
    
    for sys_id, sys_scales in scales_by_system.items():
        sorted_scales = sorted(sys_scales, key=lambda x: x.get('Iteration', 0))
        system_info = system_lookup.get(sys_id, {})
        
        actual_points = []
        projected_points = []
        all_rows = []
        
        for s in sorted_scales:
            point = {'x': s.get('LogScale', 0), 'y': s.get('LogMeasure', 0), 'iteration': s.get('Iteration', 0)}
            row = {
                'iteration': s.get('Iteration', 0),
                'measure': s.get('Measure', 0),
                'scale': s.get('Scale', 0),
                'logScale': s.get('LogScale', 0),
                'logMeasure': s.get('LogMeasure', 0),
                'scaleFactorPower': s.get('ScaleFactorPower', 0),
                'isProjected': s.get('IsProjected', False)
            }
            all_rows.append(row)
            
            if s.get('IsProjected', False):
                projected_points.append(point)
            else:
                actual_points.append(point)
        
        chart_data[sys_id] = {
            'actual': actual_points,
            'projected': projected_points,
            'slope': system_info.get('TheoreticalLogLogSlope', 0),
            'displayName': system_info.get('DisplayName', sys_id),
            'class': system_info.get('Class', 'power_law')
        }
        full_data[sys_id] = all_rows
    
    chart_data_json = json_module.dumps(chart_data)
    full_data_json = json_module.dumps(full_data)
    
    # Get validation summary for projected scales only
    expected_scales = [s for s in all_answer_scales if s.get('IsProjected', False)]
    
    html = f'''<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Power Laws & Fractals - Test Results</title>
    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
    <style>
        :root {{
            --bg-dark: #0d1117;
            --bg-card: #161b22;
            --bg-hover: #21262d;
            --border: #30363d;
            --text-primary: #c9d1d9;
            --text-secondary: #8b949e;
            --text-muted: #6e7681;
            --accent-blue: #58a6ff;
            --accent-green: #3fb950;
            --accent-yellow: #d29922;
            --accent-red: #f85149;
            --accent-purple: #a371f7;
        }}
        
        * {{
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }}
        
        body {{
            font-family: 'Segoe UI', -apple-system, BlinkMacSystemFont, sans-serif;
            background: var(--bg-dark);
            color: var(--text-primary);
            line-height: 1.6;
            padding: 2rem;
        }}
        
        .container {{
            max-width: 1400px;
            margin: 0 auto;
        }}
        
        header {{
            text-align: center;
            margin-bottom: 3rem;
            padding: 2rem;
            background: linear-gradient(135deg, var(--bg-card) 0%, #1a1f2e 100%);
            border-radius: 12px;
            border: 1px solid var(--border);
        }}
        
        h1 {{
            font-size: 2.5rem;
            margin-bottom: 0.5rem;
            background: linear-gradient(135deg, var(--accent-blue), var(--accent-purple));
            -webkit-background-clip: text;
            -webkit-text-fill-color: transparent;
            background-clip: text;
        }}
        
        .subtitle {{
            color: var(--text-secondary);
            font-size: 1.1rem;
        }}
        
        .timestamp {{
            color: var(--text-muted);
            font-size: 0.875rem;
            margin-top: 1rem;
        }}
        
        .summary-grid {{
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
            gap: 1.5rem;
            margin-bottom: 3rem;
        }}
        
        .platform-card {{
            background: var(--bg-card);
            border: 1px solid var(--border);
            border-radius: 12px;
            padding: 1.5rem;
            transition: transform 0.2s, box-shadow 0.2s;
        }}
        
        .platform-card:hover {{
            transform: translateY(-4px);
            box-shadow: 0 8px 24px rgba(0, 0, 0, 0.3);
        }}
        
        .platform-card.passed {{
            border-left: 4px solid var(--accent-green);
        }}
        
        .platform-card.failed {{
            border-left: 4px solid var(--accent-yellow);
        }}
        
        .platform-card.not-run {{
            border-left: 4px solid var(--text-muted);
            opacity: 0.7;
        }}
        
        .platform-name {{
            font-size: 1.25rem;
            font-weight: 600;
            margin-bottom: 0.5rem;
            display: flex;
            align-items: center;
            gap: 0.5rem;
        }}
        
        .platform-icon {{
            font-size: 1.5rem;
        }}
        
        .platform-stats {{
            display: flex;
            gap: 1.5rem;
            margin-top: 1rem;
        }}
        
        .stat {{
            text-align: center;
        }}
        
        .stat-value {{
            font-size: 1.75rem;
            font-weight: 700;
        }}
        
        .stat-label {{
            font-size: 0.75rem;
            color: var(--text-muted);
            text-transform: uppercase;
        }}
        
        .stat-value.pass {{
            color: var(--accent-green);
        }}
        
        .stat-value.fail {{
            color: var(--accent-yellow);
        }}
        
        .status-badge {{
            display: inline-block;
            padding: 0.25rem 0.75rem;
            border-radius: 12px;
            font-size: 0.875rem;
            font-weight: 500;
        }}
        
        .status-badge.passed {{
            background: rgba(63, 185, 80, 0.15);
            color: var(--accent-green);
        }}
        
        .status-badge.failed {{
            background: rgba(210, 153, 34, 0.15);
            color: var(--accent-yellow);
        }}
        
        .status-badge.not-run {{
            background: rgba(110, 118, 129, 0.15);
            color: var(--text-muted);
        }}
        
        section {{
            margin-bottom: 3rem;
        }}
        
        h2 {{
            font-size: 1.5rem;
            margin-bottom: 1.5rem;
            padding-bottom: 0.5rem;
            border-bottom: 1px solid var(--border);
        }}
        
        .system-section {{
            background: var(--bg-card);
            border: 1px solid var(--border);
            border-radius: 12px;
            padding: 1.5rem;
            margin-bottom: 1.5rem;
        }}
        
        .system-content {{
            display: grid;
            grid-template-columns: 1fr 400px;
            gap: 2rem;
            align-items: start;
        }}
        
        .chart-container {{
            background: var(--bg-dark);
            border-radius: 8px;
            padding: 1rem;
            height: 300px;
        }}
        
        @media (max-width: 1000px) {{
            .system-content {{
                grid-template-columns: 1fr;
            }}
        }}
        
        .system-header {{
            display: flex;
            align-items: center;
            gap: 0.75rem;
            margin-bottom: 1rem;
        }}
        
        .system-icon {{
            font-size: 1.5rem;
        }}
        
        .system-name {{
            font-size: 1.25rem;
            font-weight: 600;
        }}
        
        .system-type {{
            font-size: 0.75rem;
            padding: 0.25rem 0.5rem;
            border-radius: 4px;
            background: var(--bg-hover);
            color: var(--text-secondary);
        }}
        
        table {{
            width: 100%;
            border-collapse: collapse;
            font-size: 0.875rem;
        }}
        
        th, td {{
            padding: 0.75rem;
            text-align: left;
            border-bottom: 1px solid var(--border);
        }}
        
        th {{
            color: var(--text-secondary);
            font-weight: 500;
            text-transform: uppercase;
            font-size: 0.75rem;
        }}
        
        tr:hover {{
            background: var(--bg-hover);
        }}
        
        .value-match {{
            color: var(--accent-green);
        }}
        
        .value-mismatch {{
            color: var(--accent-yellow);
            font-weight: 600;
        }}
        
        .value-missing {{
            color: var(--text-muted);
            font-style: italic;
        }}
        
        footer {{
            text-align: center;
            padding: 2rem;
            color: var(--text-muted);
            border-top: 1px solid var(--border);
        }}
    </style>
</head>
<body>
    <div class="container">
        <header>
            <h1>🔺 Power Laws & Fractals</h1>
            <p class="subtitle">Cross-Platform Test Results Comparison</p>
            <p class="timestamp">Generated: {datetime.now(timezone.utc).strftime('%Y-%m-%d %H:%M:%S UTC')}</p>
        </header>
        
        <section>
            <h2>Platform Summary</h2>
            <div class="summary-grid">
'''
    
    platform_icons = {'python': '🐍', 'postgres': '🐘', 'golang': '🐹'}
    platform_names = {'python': 'Python', 'postgres': 'PostgreSQL', 'golang': 'Go'}
    
    for platform in PLATFORMS:
        comp = comparisons.get(platform, {})
        status = comp.get('status', 'not_run')
        pass_count = comp.get('pass_count', 0)
        fail_count = comp.get('fail_count', 0)
        
        status_class = 'passed' if status == 'passed' else ('failed' if status == 'failed' else 'not-run')
        status_text = 'PASSED' if status == 'passed' else ('FAILED' if status == 'failed' else 'NOT RUN')
        
        html += f'''
                <div class="platform-card {status_class}">
                    <div class="platform-name">
                        <span class="platform-icon">{platform_icons.get(platform, '⚙️')}</span>
                        {platform_names.get(platform, platform)}
                    </div>
                    <span class="status-badge {status_class}">{status_text}</span>
                    <div class="platform-stats">
                        <div class="stat">
                            <div class="stat-value pass">{pass_count}</div>
                            <div class="stat-label">Passed</div>
                        </div>
                        <div class="stat">
                            <div class="stat-value fail">{fail_count}</div>
                            <div class="stat-label">Failed</div>
                        </div>
                    </div>
                </div>
'''
    
    html += '''
            </div>
        </section>
        
        <section>
            <h2>Results by System</h2>
'''
    
    # Group scales by system
    by_system = {}
    for scale in all_scale_data:
        sys_id = scale['System']
        if sys_id not in by_system:
            by_system[sys_id] = []
        by_system[sys_id].append(scale)
    
    system_lookup = {s['SystemID']: s for s in systems}
    
    for sys_id in sorted(by_system.keys()):
        scales = by_system[sys_id]
        system = system_lookup.get(sys_id, {})
        
        icon = '🔺' if system.get('Class') == 'fractal' else '📈'
        type_label = 'Fractal' if system.get('Class') == 'fractal' else 'Power Law'
        
        html += f'''
            <div class="system-section">
                <div class="system-header">
                    <span class="system-icon">{icon}</span>
                    <span class="system-name">{system.get('DisplayName', sys_id)}</span>
                    <span class="system-type">{type_label}</span>
                    <span class="system-type">slope: {system.get('TheoreticalLogLogSlope', 0)}</span>
                </div>
                <div class="system-content">
                    <table>
                        <thead>
                            <tr>
                                <th>Iter</th>
                                <th>Field</th>
                                <th>Expected</th>
                                <th>Python</th>
                                <th>Postgres</th>
                                <th>Go</th>
                            </tr>
                        </thead>
                        <tbody>
'''
        
        for scale in sorted(scales, key=lambda x: x['Iteration']):
            iteration = scale['Iteration']
            expected = scale['expected']
            
            for field in ['Scale', 'LogScale', 'LogMeasure']:
                exp_val = expected.get(field)
                
                html += f'                        <tr>\n'
                html += f'                            <td>{iteration}</td>\n'
                html += f'                            <td>{field}</td>\n'
                exp_str = f"{exp_val:.5f}" if exp_val is not None else "-"
                html += f'                            <td>{exp_str}</td>\n'
                
                for platform in PLATFORMS:
                    platform_data = scale['platforms'].get(platform, {})
                    field_data = platform_data.get('fields', {}).get(field, {})
                    actual = field_data.get('actual')
                    match = field_data.get('match', True)
                    
                    if actual is None:
                        html += f'                            <td class="value-missing">-</td>\n'
                    elif match:
                        html += f'                            <td class="value-match">{actual:.5f}</td>\n'
                    else:
                        html += f'                            <td class="value-mismatch">{actual:.5f}</td>\n'
                
                html += '                        </tr>\n'
        
        html += f'''
                        </tbody>
                    </table>
                    <div class="chart-container">
                        <canvas id="chart-{sys_id}"></canvas>
                    </div>
                </div>
            </div>
'''
    
    html += f'''
        </section>
        
        <footer>
            <p>ERB Testing Protocol • Power Laws & Fractals • Veritasium Edition</p>
            <p>Testing {len(expected_scales)} projected scales across {len(systems)} systems</p>
        </footer>
    </div>
    
    <script>
        const chartData = {chart_data_json};
        
        // Create charts for each system
        Object.keys(chartData).forEach(systemId => {{
            const data = chartData[systemId];
            const canvas = document.getElementById('chart-' + systemId);
            if (!canvas) return;
            
            const ctx = canvas.getContext('2d');
            
            // Generate theoretical line points
            const allPoints = [...data.actual, ...data.projected];
            if (allPoints.length === 0) return;
            
            const xMin = Math.min(...allPoints.map(p => p.x));
            const xMax = Math.max(...allPoints.map(p => p.x));
            const firstPoint = allPoints[0];
            
            const theoreticalLine = [];
            for (let i = 0; i <= 10; i++) {{
                const x = xMin + (xMax - xMin) * i / 10;
                const y = firstPoint.y + data.slope * (x - firstPoint.x);
                theoreticalLine.push({{x, y}});
            }}
            
            new Chart(ctx, {{
                type: 'scatter',
                data: {{
                    datasets: [
                        {{
                            label: 'Actual (0-3)',
                            data: data.actual,
                            backgroundColor: '#3fb950',
                            borderColor: '#3fb950',
                            pointRadius: 8,
                            pointHoverRadius: 10
                        }},
                        {{
                            label: 'Projected (4-7)',
                            data: data.projected,
                            backgroundColor: '#a371f7',
                            borderColor: '#a371f7',
                            pointRadius: 8,
                            pointHoverRadius: 10,
                            pointStyle: 'circle'
                        }},
                        {{
                            label: 'Theoretical (slope=' + data.slope.toFixed(3) + ')',
                            data: theoreticalLine,
                            type: 'line',
                            borderColor: 'rgba(139, 148, 158, 0.5)',
                            borderDash: [5, 5],
                            pointRadius: 0,
                            fill: false
                        }}
                    ]
                }},
                options: {{
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: {{
                        legend: {{
                            display: true,
                            position: 'bottom',
                            labels: {{
                                color: '#c9d1d9',
                                font: {{ size: 10 }}
                            }}
                        }},
                        title: {{
                            display: true,
                            text: 'Log-Log Plot',
                            color: '#c9d1d9'
                        }}
                    }},
                    scales: {{
                        x: {{
                            title: {{
                                display: true,
                                text: 'log(Scale)',
                                color: '#8b949e'
                            }},
                            ticks: {{ color: '#8b949e' }},
                            grid: {{ color: '#30363d' }}
                        }},
                        y: {{
                            title: {{
                                display: true,
                                text: 'log(Measure)',
                                color: '#8b949e'
                            }},
                            ticks: {{ color: '#8b949e' }},
                            grid: {{ color: '#30363d' }}
                        }}
                    }}
                }}
            }});
        }});
    </script>
</body>
</html>
'''
    
    # Write HTML file
    report_path = SCRIPT_DIR / 'report.html'
    with open(report_path, 'w') as f:
        f.write(html)
    
    print(f"  {GREEN}✓ HTML report generated: {report_path}{RESET}")
    return report_path


def main():
    parser = argparse.ArgumentParser(description='Compare test results across platforms')
    parser.add_argument('--html', action='store_true', help='Generate HTML report')
    parser.add_argument('--quiet', '-q', action='store_true', help='Suppress console output')
    args = parser.parse_args()
    
    # Load data
    base_data_path = TEST_DATA_DIR / 'base-data.json'
    answer_key_path = TEST_DATA_DIR / 'answer-key.json'
    
    if not base_data_path.exists() or not answer_key_path.exists():
        print(f"{RED}Error: Test data not found. Run generate-test-data.py first.{RESET}")
        return 1
    
    base_data = load_json(base_data_path)
    answer_key = load_json(answer_key_path)
    
    # Load results from all platforms
    all_results = load_all_results()
    
    # Compare each platform
    comparisons = {}
    for platform in PLATFORMS:
        comparisons[platform] = compare_platform(platform, all_results[platform], answer_key)
    
    # Print console output
    if not args.quiet:
        all_passed = print_console_output(comparisons, base_data)
    else:
        all_passed = all(c.get('status') == 'passed' for c in comparisons.values())
    
    # Generate HTML report
    if args.html:
        generate_html_report(comparisons, base_data, answer_key)
    
    return 0 if all_passed else 1


if __name__ == '__main__':
    exit(main())

