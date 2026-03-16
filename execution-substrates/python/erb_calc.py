"""
ERB Calculation Library (GENERATED - DO NOT EDIT)
=================================================
Generated from: effortless-rulebook/effortless-rulebook.json

This file contains pure functions that compute calculated fields
from raw field values. Supports multiple entities.
"""

from typing import Optional, Any



# =============================================================================
# DISPATCHER FUNCTION
# =============================================================================

def compute_all_calculated_fields(record: dict, entity_name: str = None) -> dict:
    """
    Compute all calculated fields for a record.
    
    This is the main entry point for computing calculated fields.
    It routes to the appropriate entity-specific compute function.
    
    Args:
        record: The record dict with raw field values
        entity_name: Entity name (snake_case or PascalCase)
    
    Returns:
        Record dict with calculated fields filled in
    """
    if entity_name is None:
        # No entity specified - return record unchanged
        return dict(record)

    # Normalize to snake_case to support "LineItem", "line_item", "line-item"
    entity_lower = entity_name.lower().replace('-', '_')

    else:
        # Unknown entity - return record unchanged (no error)
        return dict(record)