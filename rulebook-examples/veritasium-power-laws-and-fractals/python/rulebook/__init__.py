"""
FractalsAndPowerLaws_CMCC - Python Implementation

Auto-generated from rulebook: 2025-12-11 21:36:10 UTC

This package provides Python dataclasses for analyzing fractal
and power-law systems.
"""

from .models import System, Scale, SystemStats
from .data import load_sample_data
from .utils import build_systems_dict, calculate_all_scales, calculate_all_system_stats, validate_system

__all__ = [
    "System", "Scale", "SystemStats",
    "load_sample_data",
    "build_systems_dict", "calculate_all_scales", "calculate_all_system_stats", "validate_system"
]
