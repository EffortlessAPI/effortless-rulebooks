#!/usr/bin/env python3
"""
Power Laws & Fractals - Python Test Runner

Follows the unified testing protocol:
1. Load base-data.json (for systems configuration + base scales)
2. Load test-input.json (raw facts only)
3. Compute derived values for test scales
4. Output results to test-results/python-results.json
5. Validate against answer-key.json
6. Display with unified visualization (all 8 iterations, colors, ASCII plots)
"""

import json
import math
import sys
from pathlib import Path
from datetime import datetime, timezone
from dataclasses import dataclass
from typing import Dict, List, Optional

# Add visualizer to path for shared library
SCRIPT_DIR = Path(__file__).parent
PROJECT_ROOT = SCRIPT_DIR.parent
sys.path.insert(0, str(PROJECT_ROOT / 'visualizer'))

from console_output import print_full_report, merge_scales

# Paths
TEST_DATA_DIR = PROJECT_ROOT / 'test-data'
TEST_RESULTS_DIR = PROJECT_ROOT / 'test-results'

# ANSI colors (for error messages before shared lib loads)
RED = '\033[91m'
RESET = '\033[0m'

# Tolerance for validation (allows for floating-point precision in 6dp comparisons)
# Using 0.0000015 to handle rounding at the 6th decimal place boundary
TOLERANCE = 0.0000015


@dataclass
class System:
    """System configuration from ERB"""
    SystemID: str
    DisplayName: str
    Class: str
    BaseScale: float
    ScaleFactor: float
    MeasureName: str
    FractalDimension: Optional[float]
    TheoreticalLogLogSlope: float


@dataclass
class Scale:
    """Scale measurement with computed values"""
    ScaleID: str
    System: str
    Iteration: int
    Measure: float
    BaseScale: Optional[float] = None
    ScaleFactor: Optional[float] = None
    ScaleFactorPower: Optional[float] = None
    Scale: Optional[float] = None
    LogScale: Optional[float] = None
    LogMeasure: Optional[float] = None
    IsProjected: bool = False


def load_json(path: Path) -> Dict:
    """Load JSON file"""
    with open(path, 'r') as f:
        return json.load(f)


def save_json(path: Path, data: Dict):
    """Save JSON file"""
    path.parent.mkdir(parents=True, exist_ok=True)
    with open(path, 'w') as f:
        json.dump(data, f, indent=2)


def load_systems(base_data: Dict) -> Dict[str, System]:
    """Load systems from base data and create lookup dict"""
    systems = {}
    for s in base_data.get('systems', []):
        system = System(
            SystemID=s['SystemID'],
            DisplayName=s['DisplayName'],
            Class=s['Class'],
            BaseScale=s['BaseScale'],
            ScaleFactor=s['ScaleFactor'],
            MeasureName=s['MeasureName'],
            FractalDimension=s.get('FractalDimension'),
            TheoreticalLogLogSlope=s['TheoreticalLogLogSlope']
        )
        systems[system.SystemID] = system
    return systems


def load_test_scales(test_input: Dict) -> List[Scale]:
    """Load test scales from test input (raw facts only)"""
    scales = []
    for s in test_input.get('scales', []):
        scale = Scale(
            ScaleID=s['ScaleID'],
            System=s['System'],
            Iteration=s['Iteration'],
            Measure=s['Measure'],
            IsProjected=s.get('IsProjected', True)
        )
        scales.append(scale)
    return scales


def compute_derived_values(scale: Scale, systems: Dict[str, System]) -> Scale:
    """Compute all derived values for a scale"""
    system = systems.get(scale.System)
    if not system:
        raise ValueError(f"System not found: {scale.System}")
    
    # Lookup values from parent system
    scale.BaseScale = system.BaseScale
    scale.ScaleFactor = system.ScaleFactor
    
    # Calculated values
    scale.ScaleFactorPower = math.pow(scale.ScaleFactor, scale.Iteration)
    scale.Scale = scale.BaseScale * scale.ScaleFactorPower
    scale.LogScale = math.log10(scale.Scale) if scale.Scale > 0 else 0
    scale.LogMeasure = math.log10(scale.Measure) if scale.Measure > 0 else 0
    
    return scale


def round_for_comparison(value: float, decimals: int = 6) -> float:
    """Round a float for consistent comparison (6 decimal places)"""
    if value is None:
        return None
    return round(value, decimals)


def scale_to_dict(scale: Scale) -> Dict:
    """Convert Scale to dict with rounded values for JSON output (6 decimal places)"""
    return {
        'ScaleID': scale.ScaleID,
        'System': scale.System,
        'Iteration': scale.Iteration,
        'Measure': round_for_comparison(scale.Measure),
        'BaseScale': round_for_comparison(scale.BaseScale),
        'ScaleFactor': round_for_comparison(scale.ScaleFactor),
        'ScaleFactorPower': round_for_comparison(scale.ScaleFactorPower),
        'Scale': round_for_comparison(scale.Scale),
        'LogScale': round_for_comparison(scale.LogScale),
        'LogMeasure': round_for_comparison(scale.LogMeasure),
        'IsProjected': scale.IsProjected
    }


def system_to_dict(system: System) -> Dict:
    """Convert System dataclass to dict for visualization"""
    return {
        'SystemID': system.SystemID,
        'DisplayName': system.DisplayName,
        'Class': system.Class,
        'BaseScale': system.BaseScale,
        'ScaleFactor': system.ScaleFactor,
        'MeasureName': system.MeasureName,
        'FractalDimension': system.FractalDimension,
        'TheoreticalLogLogSlope': system.TheoreticalLogLogSlope
    }


def compare_values(expected, actual, tolerance: float = TOLERANCE) -> bool:
    """Compare two values with tolerance for floats"""
    if expected is None and actual is None:
        return True
    if expected is None or actual is None:
        return False
    if isinstance(expected, (int, float)) and isinstance(actual, (int, float)):
        return abs(expected - actual) < tolerance
    return expected == actual


def validate_results(computed_scales: List[Dict], answer_key: Dict) -> tuple:
    """Validate computed scales against answer key"""
    expected_by_id = {s['ScaleID']: s for s in answer_key.get('scales', [])}
    
    pass_count = 0
    fail_count = 0
    failures = []
    
    computed_fields = ['BaseScale', 'ScaleFactor', 'ScaleFactorPower', 'Scale', 'LogScale', 'LogMeasure']
    
    for scale in computed_scales:
        scale_id = scale.get('ScaleID')
        expected = expected_by_id.get(scale_id)
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
            failures.append((scale_id, mismatches))
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
    answer_key = load_json(answer_key_path)
    
    # Load systems
    systems = load_systems(base_data)
    
    # Load test scales (raw facts only)
    test_scales = load_test_scales(test_input)
    
    # Compute derived values for test scales
    computed_test_scales = []
    for scale in test_scales:
        computed = compute_derived_values(scale, systems)
        computed_test_scales.append(computed)
    
    # Convert to dicts for output
    computed_test_dicts = [scale_to_dict(s) for s in computed_test_scales]
    
    # Save results (test scales only for validation compatibility)
    results = {
        'platform': 'python',
        'timestamp': datetime.now(timezone.utc).isoformat(),
        'scales': computed_test_dicts
    }
    
    results_path = TEST_RESULTS_DIR / 'python-results.json'
    save_json(results_path, results)
    
    # Merge base scales with computed test scales for full visualization
    base_scales = base_data.get('scales', [])
    all_scales = merge_scales(base_scales, computed_test_dicts)
    
    # Validate against answer key
    pass_count, fail_count, failures = validate_results(computed_test_dicts, answer_key)
    
    # Convert systems to dict format for visualization
    systems_dict = {sid: system_to_dict(sys) for sid, sys in systems.items()}
    
    # Print full report using shared library
    print_full_report(
        platform='python',
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
