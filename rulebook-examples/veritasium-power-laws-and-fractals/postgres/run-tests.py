#!/usr/bin/env python3
"""
Power Laws & Fractals - PostgreSQL Test Runner

Follows the unified testing protocol:
1. Initialize database with base-data.json (systems + base scales)
2. Insert test-input.json scales (raw facts only)
3. Query vw_scales to get ALL computed values from PostgreSQL (all 8 iterations)
4. Output results to test-results/postgres-results.json
5. Validate against answer-key.json
6. Display with unified visualization (all 8 iterations, colors, ASCII plots)

Requires: PostgreSQL running on localhost:5432 with database 'demo'
"""

import json
import subprocess
import sys
from pathlib import Path
from datetime import datetime, timezone
from typing import Dict, List, Tuple

# Add visualizer to path for shared library
SCRIPT_DIR = Path(__file__).parent
PROJECT_ROOT = SCRIPT_DIR.parent
sys.path.insert(0, str(PROJECT_ROOT / 'visualizer'))

from console_output import print_full_report

# Paths
TEST_DATA_DIR = PROJECT_ROOT / 'test-data'
TEST_RESULTS_DIR = PROJECT_ROOT / 'test-results'

# ANSI colors (for initialization messages)
GREEN = '\033[92m'
CYAN = '\033[96m'
RED = '\033[91m'
DIM = '\033[2m'
RESET = '\033[0m'

# Database connection
DB_CONN = "postgresql://postgres@localhost:5432/demo"

# Tolerance for validation (allows for floating-point precision in 6dp comparisons)
# Using 0.0000015 to handle rounding at the 6th decimal place boundary
TOLERANCE = 0.0000015


def run_sql(query: str, fetch: bool = True) -> List[List[str]]:
    """Run SQL query using psql and return results"""
    cmd = ['psql', DB_CONN, '-t', '-A', '-F', '|', '-c', query]
    try:
        result = subprocess.run(cmd, capture_output=True, text=True, check=True)
        if fetch and result.stdout.strip():
            rows = []
            for line in result.stdout.strip().split('\n'):
                if line:
                    rows.append(line.split('|'))
            return rows
        return []
    except subprocess.CalledProcessError as e:
        print(f"{RED}SQL Error: {e.stderr}{RESET}")
        return []
    except FileNotFoundError:
        print(f"{RED}Error: psql not found. Install PostgreSQL client tools.{RESET}")
        sys.exit(1)


def run_sql_file(filepath: Path) -> bool:
    """Execute a SQL file"""
    cmd = ['psql', DB_CONN, '-f', str(filepath), '-v', 'ON_ERROR_STOP=1']
    try:
        result = subprocess.run(cmd, capture_output=True, text=True)
        return result.returncode == 0
    except Exception as e:
        print(f"{RED}Error running SQL file: {e}{RESET}")
        return False


def load_json(path: Path) -> Dict:
    """Load JSON file"""
    with open(path, 'r') as f:
        return json.load(f)


def save_json(path: Path, data: Dict):
    """Save JSON file"""
    path.parent.mkdir(parents=True, exist_ok=True)
    with open(path, 'w') as f:
        json.dump(data, f, indent=2)


def init_database() -> bool:
    """Initialize database with schema"""
    print(f"  Initializing database schema...")
    
    # Run schema files in order
    sql_files = [
        SCRIPT_DIR / '01-drop-and-create-tables.sql',
        SCRIPT_DIR / '02-create-functions.sql',
        SCRIPT_DIR / '03-create-views.sql',
    ]
    
    for sql_file in sql_files:
        if not sql_file.exists():
            print(f"  {RED}SQL file not found: {sql_file.name}{RESET}")
            return False
        if not run_sql_file(sql_file):
            print(f"  {RED}Failed to run: {sql_file.name}{RESET}")
            return False
    
    return True


def insert_systems(systems: List[Dict]) -> bool:
    """Insert systems from base data"""
    print(f"  Inserting {len(systems)} systems...")
    
    for s in systems:
        # Handle NULL for fractal_dimension
        fd = 'NULL' if s.get('FractalDimension') is None else s['FractalDimension']
        
        query = f"""
            INSERT INTO systems (system_id, display_name, class, base_scale, scale_factor, 
                                measure_name, fractal_dimension, theoretical_log_log_slope)
            VALUES ('{s['SystemID']}', '{s['DisplayName']}', '{s['Class']}', 
                   {s['BaseScale']}, {s['ScaleFactor']}, '{s['MeasureName']}', 
                   {fd}, {s['TheoreticalLogLogSlope']})
            ON CONFLICT (system_id) DO NOTHING
        """
        run_sql(query, fetch=False)
    
    return True


def insert_base_scales(scales: List[Dict]) -> bool:
    """Insert base scales from base data"""
    print(f"  Inserting {len(scales)} base scales...")
    
    for s in scales:
        is_projected = 'true' if s.get('IsProjected', False) else 'false'
        query = f"""
            INSERT INTO scales (scale_id, "system", iteration, measure, is_projected)
            VALUES ('{s['ScaleID']}', '{s['System']}', {s['Iteration']}, {s['Measure']}, {is_projected})
            ON CONFLICT (scale_id) DO NOTHING
        """
        run_sql(query, fetch=False)
    
    return True


def insert_test_scales(scales: List[Dict]) -> bool:
    """Insert test scales (raw facts only)"""
    print(f"  Inserting {len(scales)} test scales...")
    
    for s in scales:
        is_projected = 'true' if s.get('IsProjected', True) else 'false'
        query = f"""
            INSERT INTO scales (scale_id, "system", iteration, measure, is_projected)
            VALUES ('{s['ScaleID']}', '{s['System']}', {s['Iteration']}, {s['Measure']}, {is_projected})
            ON CONFLICT (scale_id) DO NOTHING
        """
        run_sql(query, fetch=False)
    
    return True


def query_all_scales() -> List[Dict]:
    """Query ALL computed values from vw_scales view (rounded to 6 decimal places)"""
    
    query = """
        SELECT scale_id, "system", iteration, measure, 
               base_scale, scale_factor, scale_factor_power, 
               scale, log_scale, log_measure, is_projected
        FROM vw_scales
        ORDER BY "system", iteration
    """
    
    rows = run_sql(query)
    
    scales = []
    for row in rows:
        scales.append({
            'ScaleID': row[0],
            'System': row[1],
            'Iteration': int(row[2]) if row[2] else 0,
            'Measure': round(float(row[3]), 6) if row[3] else 0,
            'BaseScale': round(float(row[4]), 6) if row[4] else None,
            'ScaleFactor': round(float(row[5]), 6) if row[5] else None,
            'ScaleFactorPower': round(float(row[6]), 6) if row[6] else None,
            'Scale': round(float(row[7]), 6) if row[7] else None,
            'LogScale': round(float(row[8]), 6) if row[8] else None,
            'LogMeasure': round(float(row[9]), 6) if row[9] else None,
            'IsProjected': row[10] == 't' if row[10] else False
        })
    
    return scales


def compare_values(expected, actual, tolerance: float = TOLERANCE) -> bool:
    """Compare two values with tolerance for floats"""
    if expected is None and actual is None:
        return True
    if expected is None or actual is None:
        return False
    if isinstance(expected, (int, float)) and isinstance(actual, (int, float)):
        return abs(expected - actual) < tolerance
    return expected == actual


def validate_results(computed_scales: List[Dict], answer_key: Dict) -> Tuple[int, int, List]:
    """Validate computed scales against answer key"""
    expected_by_id = {s['ScaleID']: s for s in answer_key.get('scales', [])}
    
    pass_count = 0
    fail_count = 0
    failures = []
    
    computed_fields = ['BaseScale', 'ScaleFactor', 'ScaleFactorPower', 'Scale', 'LogScale', 'LogMeasure']
    
    for scale in computed_scales:
        expected = expected_by_id.get(scale['ScaleID'])
        if not expected:
            # Not a test scale - skip validation
            continue
        
        mismatches = []
        
        for field in computed_fields:
            exp_val = expected.get(field)
            act_val = scale.get(field)
            
            if not compare_values(exp_val, act_val):
                mismatches.append(f"{field}: expected {exp_val}, got {act_val}")
        
        if mismatches:
            fail_count += 1
            failures.append((scale['ScaleID'], mismatches))
        else:
            pass_count += 1
    
    return pass_count, fail_count, failures


def main():
    """Main test runner"""
    # Check if test data exists
    base_data_path = TEST_DATA_DIR / 'base-data.json'
    test_input_path = TEST_DATA_DIR / 'test-input.json'
    answer_key_path = TEST_DATA_DIR / 'answer-key.json'
    
    if not base_data_path.exists() or not test_input_path.exists():
        print(f"{RED}Error: Test data not found. Run generate-test-data.py first.{RESET}")
        sys.exit(1)
    
    # Load data
    base_data = load_json(base_data_path)
    test_input = load_json(test_input_path)
    
    print(f"\n{CYAN}Initializing PostgreSQL database...{RESET}")
    
    # Initialize database schema
    if not init_database():
        print(f"{RED}Failed to initialize database. Is PostgreSQL running?{RESET}")
        print(f"{DIM}Start with: docker run -d -p 5432:5432 -e POSTGRES_HOST_AUTH_METHOD=trust postgres{RESET}")
        sys.exit(1)
    
    # Insert systems
    if not insert_systems(base_data.get('systems', [])):
        print(f"{RED}Failed to insert systems{RESET}")
        sys.exit(1)
    
    # Insert base scales (iterations 0-3)
    if not insert_base_scales(base_data.get('scales', [])):
        print(f"{RED}Failed to insert base scales{RESET}")
        sys.exit(1)
    
    # Insert test scales (iterations 4-7)
    if not insert_test_scales(test_input.get('scales', [])):
        print(f"{RED}Failed to insert test scales{RESET}")
        sys.exit(1)
    
    print(f"  {GREEN}✓ Database initialized with test data{RESET}")
    
    # Query ALL computed values from view (all 8 iterations)
    print(f"\n{CYAN}Querying ALL computed values from vw_scales...{RESET}")
    all_scales = query_all_scales()
    print(f"  Retrieved {len(all_scales)} computed scales (all iterations)")
    
    # Save full results (all 8 iterations per system)
    full_results = {
        'platform': 'postgres',
        'scales': all_scales
    }
    
    full_results_path = TEST_RESULTS_DIR / 'postgres-results.json'
    save_json(full_results_path, full_results)
    
    # Validate only the projected scales against answer key
    answer_key = load_json(answer_key_path)
    test_scale_ids = [s['ScaleID'] for s in test_input.get('scales', [])]
    test_scales = [s for s in all_scales if s['ScaleID'] in test_scale_ids]
    pass_count, fail_count, failures = validate_results(test_scales, answer_key)
    
    # Build systems dict for visualization
    systems_dict = {s['SystemID']: s for s in base_data.get('systems', [])}
    
    # Print full report using shared library
    print_full_report(
        platform='postgres',
        all_scales=all_scales,
        systems=systems_dict,
        pass_count=pass_count,
        fail_count=fail_count,
        failures=failures,
        show_plots=True
    )
    
    # Exit with appropriate code
    sys.exit(0 if fail_count == 0 else 1)


if __name__ == '__main__':
    main()
