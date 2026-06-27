"""
Rulebook JSON Parser

Parses the canonical rulebook JSON and extracts table definitions,
field schemas, relationships, and calculation dependencies.
"""

import json
import re
from typing import List, Dict, Optional, Set, Tuple
from dataclasses import dataclass


@dataclass
class Field:
    """Represents a field in a table"""
    name: str
    datatype: str  # string, number, integer, decimal
    field_type: str  # raw, relationship, lookup, calculated, aggregation
    formula: Optional[str] = None
    description: str = ""
    is_primary_key: bool = False
    related_to: Optional[str] = None  # For relationship fields

    def is_calculated_field(self) -> bool:
        """Returns True if this field needs to be calculated"""
        return self.field_type in ['lookup', 'calculated', 'aggregation']

    def get_dependencies(self) -> Set[str]:
        """Extract field names this field depends on (same-table only)"""
        if not self.formula:
            return set()

        # Find all {{FieldName}} references that are NOT prefixed with table!
        # We want: {{FieldName}} but not table!{{FieldName}}
        # First remove all cross-table references
        formula_clean = re.sub(r'\w+!\{\{[A-Za-z_][A-Za-z0-9_]*\}\}', '', self.formula)

        # Now find remaining {{FieldName}} patterns
        pattern = r'\{\{([A-Za-z_][A-Za-z0-9_]*)\}\}'
        matches = re.findall(pattern, formula_clean)
        return set(matches)


@dataclass
class Table:
    """Represents a table in the rulebook"""
    name: str
    description: str
    primary_key: List[str]
    fields: List[Field]
    data: List[Dict]

    def get_raw_fields(self) -> List[Field]:
        """Get all raw/relationship fields (stored fields)"""
        return [f for f in self.fields if f.field_type in ['raw', 'relationship']]

    def get_calculated_fields(self) -> List[Field]:
        """Get all calculated fields (derived fields)"""
        return [f for f in self.fields if f.is_calculated_field()]

    def get_field_by_name(self, name: str) -> Optional[Field]:
        """Find a field by name"""
        for field in self.fields:
            if field.name == name:
                return field
        return None

    def get_calculation_order(self) -> List[Field]:
        """
        Determine the order fields must be calculated in based on dependencies.
        Returns fields in topological sort order.
        """
        calculated = self.get_calculated_fields()
        calc_field_names = {f.name for f in calculated}

        # Build dependency graph (only dependencies on OTHER calculated fields in THIS table)
        dependencies = {}
        for field in calculated:
            # Filter to only include dependencies that are calculated fields in this table
            deps_in_table = field.get_dependencies() & calc_field_names
            dependencies[field.name] = deps_in_table

        # Topological sort
        ordered = []
        visited = set()
        visiting = set()

        def visit(field_name: str):
            if field_name in visited:
                return
            if field_name in visiting:
                raise ValueError(f"Circular dependency detected for field: {field_name}")

            visiting.add(field_name)

            # Visit dependencies first
            if field_name in dependencies:
                for dep in dependencies[field_name]:
                    visit(dep)

            visiting.remove(field_name)
            visited.add(field_name)

            # Add to ordered list
            field = self.get_field_by_name(field_name)
            if field:
                ordered.append(field)

        # Visit all calculated fields
        for field in calculated:
            visit(field.name)

        return ordered


class RulebookParser:
    """Parses the canonical rulebook JSON"""

    def __init__(self, json_path: str):
        """Load and parse the rulebook"""
        with open(json_path, 'r') as f:
            self.rulebook = json.load(f)

        self.model_name = self.rulebook.get('model_name', 'Unknown')
        self.description = self.rulebook.get('Description', '')
        self.tables = self._parse_tables()

    def _parse_tables(self) -> Dict[str, Table]:
        """Parse all tables from the rulebook"""
        tables = {}

        for table_name, table_def in self.rulebook.items():
            # Skip metadata fields
            if table_name.startswith('$') or table_name.startswith('_') or \
               table_name in ['model_name', 'Description']:
                continue

            # Parse table definition
            if not isinstance(table_def, dict):
                continue

            fields = self._parse_fields(table_def.get('schema', []))

            table = Table(
                name=table_name,
                description=table_def.get('Description', ''),
                primary_key=table_def.get('primary_key', []),
                fields=fields,
                data=table_def.get('data', [])
            )

            tables[table_name] = table

        return tables

    def _parse_fields(self, schema: List[Dict]) -> List[Field]:
        """Parse field definitions from schema"""
        fields = []

        for field_def in schema:
            field = Field(
                name=field_def.get('name', ''),
                datatype=field_def.get('datatype', 'string'),
                field_type=field_def.get('type', 'raw'),
                formula=field_def.get('formula'),
                description=field_def.get('Description', ''),
                is_primary_key=field_def.get('is_primary_key', False),
                related_to=field_def.get('RelatedTo')
            )
            fields.append(field)

        return fields

    def get_table(self, name: str) -> Optional[Table]:
        """Get a table by name"""
        return self.tables.get(name)

    def get_table_names(self) -> List[str]:
        """Get all table names"""
        return list(self.tables.keys())

    def get_relationships(self) -> Dict[str, List[Tuple[str, str, str]]]:
        """
        Get all relationships between tables.
        Returns: {table_name: [(field_name, related_table, related_field), ...]}
        """
        relationships = {}

        for table_name, table in self.tables.items():
            table_rels = []
            for field in table.fields:
                if field.field_type == 'relationship' and field.related_to:
                    # Assume FK points to PK of related table
                    related_table = self.get_table(field.related_to)
                    if related_table and related_table.primary_key:
                        related_field = related_table.primary_key[0]
                        table_rels.append((field.name, field.related_to, related_field))

            if table_rels:
                relationships[table_name] = table_rels

        return relationships

    def validate(self) -> List[str]:
        """
        Validate the rulebook for common issues.
        Returns list of warning/error messages.
        """
        issues = []

        for table_name, table in self.tables.items():
            # Check for primary key
            if not table.primary_key:
                issues.append(f"Table '{table_name}' has no primary key defined")

            # Check for circular dependencies
            try:
                table.get_calculation_order()
            except ValueError as e:
                issues.append(f"Table '{table_name}': {str(e)}")

            # Check relationship integrity
            for field in table.fields:
                if field.field_type == 'relationship':
                    if not field.related_to:
                        issues.append(f"Field '{table_name}.{field.name}' is a relationship but has no RelatedTo")
                    elif field.related_to not in self.tables:
                        issues.append(f"Field '{table_name}.{field.name}' relates to unknown table '{field.related_to}'")

        return issues
