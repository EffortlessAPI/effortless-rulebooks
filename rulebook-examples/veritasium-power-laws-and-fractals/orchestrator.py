#!/usr/bin/env python3
"""
ERB Test Orchestrator

Master test runner that:
1. Generates test data from SSoT
2. Runs tests on each platform (Python, Postgres, Golang)
3. Collects results and compares against answer key
4. Generates console and HTML reports
"""

import argparse
import json
import subprocess
import sys
from pathlib import Path
from datetime import datetime, timezone
from typing import Dict, List, Optional, Tuple

# Paths
SCRIPT_DIR = Path(__file__).parent
TEST_DATA_DIR = SCRIPT_DIR / 'test-data'
TEST_RESULTS_DIR = SCRIPT_DIR / 'test-results'
VISUALIZER_DIR = SCRIPT_DIR / 'visualizer'

# ANSI colors
GREEN = '\033[92m'
YELLOW = '\033[93m'
CYAN = '\033[96m'
RED = '\033[91m'
MAGENTA = '\033[95m'
DIM = '\033[2m'
RESET = '\033[0m'
BOLD = '\033[1m'

# Tolerance for floating point comparisons (6 decimal places with margin)
TOLERANCE = 0.0000015


class Platform:
    """Represents a test platform"""
    def __init__(self, name: str, runner_path: str, runner_cmd: List[str]):
        self.name = name
        self.runner_path = SCRIPT_DIR / runner_path
        self.runner_cmd = runner_cmd
        self.results_file = TEST_RESULTS_DIR / f'{name}-results.json'
    
    def exists(self) -> bool:
        """Check if platform runner exists"""
        return self.runner_path.exists()
    
    def run(self, verbose: bool = False) -> Tuple[bool, str]:
        """Run the platform test and return (success, message)"""
        if not self.exists():
            return False, f"Runner not found: {self.runner_path}"
        
        try:
            result = subprocess.run(
                self.runner_cmd,
                cwd=self.runner_path.parent,
                capture_output=True,
                text=True,
                timeout=120
            )
            
            # Check if results file exists (even if exit code non-zero)
            # Exit code 1 usually means validation failed, not execution error
            if not self.results_file.exists():
                if verbose:
                    print(f"{DIM}{result.stderr}{RESET}")
                return False, f"Results file not generated (exit code {result.returncode})"
            
            # Results file exists - we can validate even if exit code was non-zero
            return True, "OK"
        
        except subprocess.TimeoutExpired:
            return False, "Timeout (120s)"
        except Exception as e:
            return False, str(e)


# Platform definitions
PLATFORMS = {
    'python': Platform('python', 'python/run-tests.py', ['python3', 'run-tests.py']),
    'postgres': Platform('postgres', 'postgres/run-tests.py', ['python3', 'run-tests.py']),
    'golang': Platform('golang', 'golang/run-tests.go', ['go', 'run', '.']),
}


def load_json(path: Path) -> Dict:
    """Load JSON file"""
    with open(path, 'r') as f:
        return json.load(f)


def save_json(path: Path, data: Dict):
    """Save JSON file"""
    with open(path, 'w') as f:
        json.dump(data, f, indent=2)


def compare_values(expected, actual, tolerance: float = TOLERANCE) -> bool:
    """Compare two values with tolerance for floats"""
    if expected is None and actual is None:
        return True
    if expected is None or actual is None:
        return False
    if isinstance(expected, (int, float)) and isinstance(actual, (int, float)):
        return abs(expected - actual) < tolerance
    return expected == actual


def compare_scale(expected: Dict, actual: Dict) -> List[str]:
    """Compare a scale record and return list of mismatches"""
    mismatches = []
    
    # Fields to compare
    computed_fields = ['BaseScale', 'ScaleFactor', 'ScaleFactorPower', 'Scale', 'LogScale', 'LogMeasure']
    
    for field in computed_fields:
        exp_val = expected.get(field)
        act_val = actual.get(field)
        
        if not compare_values(exp_val, act_val):
            mismatches.append(f"{field}: expected {exp_val}, got {act_val}")
    
    return mismatches


def validate_results(platform_name: str, verbose: bool = False) -> Tuple[bool, int, int, List[Dict]]:
    """
    Validate platform results against answer key.
    Only validates PROJECTED scales (iterations 4-7) since those are what platforms compute.
    Returns: (all_passed, pass_count, fail_count, failures_list)
    """
    answer_key = load_json(TEST_DATA_DIR / 'answer-key.json')
    results_file = TEST_RESULTS_DIR / f'{platform_name}-results.json'
    
    if not results_file.exists():
        return False, 0, 0, [{'error': 'Results file not found'}]
    
    results = load_json(results_file)
    
    # Only validate PROJECTED scales (IsProjected=True, iterations 4-7)
    # These are the scales that platforms are tested on
    projected_scales = [s for s in answer_key['scales'] if s.get('IsProjected', False)]
    expected_by_id = {s['ScaleID']: s for s in projected_scales}
    actual_by_id = {s['ScaleID']: s for s in results.get('scales', [])}
    
    pass_count = 0
    fail_count = 0
    failures = []
    
    for scale_id, expected in expected_by_id.items():
        actual = actual_by_id.get(scale_id)
        
        if actual is None:
            fail_count += 1
            failures.append({
                'ScaleID': scale_id,
                'System': expected.get('System'),
                'error': 'Scale not found in results'
            })
            continue
        
        mismatches = compare_scale(expected, actual)
        
        if mismatches:
            fail_count += 1
            failures.append({
                'ScaleID': scale_id,
                'System': expected.get('System'),
                'mismatches': mismatches
            })
        else:
            pass_count += 1
    
    all_passed = fail_count == 0
    return all_passed, pass_count, fail_count, failures


def generate_test_data():
    """Regenerate test data from SSoT"""
    print(f"{CYAN}Generating test data from SSoT...{RESET}")
    
    result = subprocess.run(
        ['python3', 'generate-test-data.py'],
        cwd=SCRIPT_DIR,
        capture_output=True,
        text=True
    )
    
    if result.returncode == 0:
        print(f"  {GREEN}✓ Test data generated{RESET}")
        return True
    else:
        print(f"  {RED}✗ Failed to generate test data{RESET}")
        print(f"{DIM}{result.stderr}{RESET}")
        return False


def run_platform_tests(platforms: List[str], verbose: bool = False) -> Dict[str, Dict]:
    """Run tests for specified platforms"""
    results = {}
    
    for platform_name in platforms:
        if platform_name not in PLATFORMS:
            print(f"  {YELLOW}⚠ Unknown platform: {platform_name}{RESET}")
            continue
        
        platform = PLATFORMS[platform_name]
        
        if not platform.exists():
            print(f"  {DIM}⊘ {platform_name}: Not implemented{RESET}")
            results[platform_name] = {'status': 'not_implemented'}
            continue
        
        print(f"  Running {platform_name}...", end=' ', flush=True)
        success, message = platform.run(verbose)
        
        if success:
            print(f"{GREEN}✓{RESET}")
            
            # Validate results
            all_passed, pass_count, fail_count, failures = validate_results(platform_name, verbose)
            
            results[platform_name] = {
                'status': 'passed' if all_passed else 'failed',
                'pass_count': pass_count,
                'fail_count': fail_count,
                'failures': failures
            }
        else:
            print(f"{RED}✗ {message}{RESET}")
            results[platform_name] = {
                'status': 'error',
                'message': message
            }
    
    return results


def print_summary(results: Dict[str, Dict]):
    """Print summary of test results"""
    print(f"\n{BOLD}{'=' * 70}{RESET}")
    print(f"{BOLD}Test Summary{RESET}")
    print(f"{'=' * 70}")
    
    all_passed = True
    
    for platform_name, result in results.items():
        status = result.get('status')
        
        if status == 'not_implemented':
            icon = '⊘'
            color = DIM
            msg = "Not implemented"
        elif status == 'passed':
            icon = '✓'
            color = GREEN
            msg = f"{result['pass_count']} scales validated"
        elif status == 'failed':
            icon = '✗'
            color = YELLOW
            msg = f"{result['pass_count']} passed, {result['fail_count']} failed"
            all_passed = False
        else:  # error
            icon = '!'
            color = RED
            msg = result.get('message', 'Unknown error')
            all_passed = False
        
        print(f"  {color}{icon} {platform_name:15} {msg}{RESET}")
    
    print(f"{'=' * 70}")
    
    if all_passed:
        print(f"\n{GREEN}✓ All platform tests passed!{RESET}\n")
    else:
        print(f"\n{YELLOW}⚠ Some tests failed. See details above.{RESET}\n")
    
    return all_passed


def print_failures(results: Dict[str, Dict]):
    """Print detailed failure information"""
    for platform_name, result in results.items():
        failures = result.get('failures', [])
        if failures:
            print(f"\n{BOLD}{platform_name} Failures:{RESET}")
            for failure in failures[:5]:  # Show first 5
                print(f"  • {failure.get('ScaleID', 'Unknown')}")
                for mismatch in failure.get('mismatches', []):
                    print(f"    - {mismatch}")


def generate_html_report(results: Dict[str, Dict]):
    """Generate HTML report using generate_report.py"""
    try:
        result = subprocess.run(
            ['python3', 'generate_report.py'],
            cwd=VISUALIZER_DIR,
            capture_output=True,
            text=True
        )
        
        if result.returncode == 0:
            print(f"  {GREEN}✓ HTML report generated: visualizer/report.html{RESET}")
        else:
            print(f"  {YELLOW}⚠ Could not generate HTML report{RESET}")
            if result.stderr:
                print(f"  {DIM}{result.stderr}{RESET}")
    except Exception as e:
        print(f"  {YELLOW}⚠ HTML report generation skipped: {e}{RESET}")


def main():
    parser = argparse.ArgumentParser(
        description='ERB Test Orchestrator - Run and validate platform tests'
    )
    parser.add_argument(
        '--all', '-a',
        action='store_true',
        help='Run all available platforms'
    )
    parser.add_argument(
        '--platform', '-p',
        action='append',
        choices=['python', 'postgres', 'golang'],
        help='Run specific platform(s)'
    )
    parser.add_argument(
        '--regenerate', '-r',
        action='store_true',
        help='Regenerate test data from SSoT before running'
    )
    parser.add_argument(
        '--report',
        action='store_true',
        help='Generate HTML report after tests'
    )
    parser.add_argument(
        '--verbose', '-v',
        action='store_true',
        help='Show verbose output'
    )
    
    args = parser.parse_args()
    
    # Banner
    print(f"\n{BOLD}{'═' * 70}{RESET}")
    print(f"{BOLD}  🧪 ERB Test Orchestrator - Power Laws & Fractals{RESET}")
    print(f"{BOLD}{'═' * 70}{RESET}\n")
    
    # Determine which platforms to run
    if args.all:
        platforms = list(PLATFORMS.keys())
    elif args.platform:
        platforms = args.platform
    else:
        platforms = list(PLATFORMS.keys())  # Default to all
    
    # Regenerate test data if requested
    if args.regenerate:
        if not generate_test_data():
            sys.exit(1)
        print()
    
    # Run platform tests
    print(f"{CYAN}Running platform tests...{RESET}")
    results = run_platform_tests(platforms, args.verbose)
    
    # Print summary
    all_passed = print_summary(results)
    
    # Print failures if any
    if args.verbose or not all_passed:
        print_failures(results)
    
    # Generate HTML report if requested
    if args.report:
        print(f"\n{CYAN}Generating HTML report...{RESET}")
        generate_html_report(results)
    
    # Exit with appropriate code
    sys.exit(0 if all_passed else 1)


if __name__ == '__main__':
    main()

