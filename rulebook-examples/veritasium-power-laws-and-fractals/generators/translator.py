"""
Formula Translator

Converts Excel-dialect formulas from the rulebook into platform-specific code.

Supported Excel functions:
- POWER(x, y) → language-specific power function
- LOG10(x) → language-specific log10 function
- INDEX/MATCH → dictionary/map lookup
- COUNTIF → filtering and counting
- MINIFS/MAXIFS → filtering and min/max
- Arithmetic: +, -, *, /
"""

import re
from typing import Dict, List, Optional, Tuple
from enum import Enum


class Language(Enum):
    """Supported target languages"""
    PYTHON = "python"
    GOLANG = "golang"
    TYPESCRIPT = "typescript"
    JAVASCRIPT = "javascript"
    SQL = "sql"


class FormulaTranslator:
    """Translates Excel formulas to target programming languages"""

    def __init__(self, language: Language):
        self.language = language

        # Function mapping: Excel function → target language function
        self.function_map = {
            Language.PYTHON: {
                'POWER': 'math.pow',
                'LOG10': 'math.log10',
                'ABS': 'abs',
                'SQRT': 'math.sqrt',
            },
            Language.GOLANG: {
                'POWER': 'math.Pow',
                'LOG10': 'math.Log10',
                'ABS': 'math.Abs',
                'SQRT': 'math.Sqrt',
            },
            Language.TYPESCRIPT: {
                'POWER': 'Math.pow',
                'LOG10': 'Math.log10',
                'ABS': 'Math.abs',
                'SQRT': 'Math.sqrt',
            },
            Language.JAVASCRIPT: {
                'POWER': 'Math.pow',
                'LOG10': 'Math.log10',
                'ABS': 'Math.abs',
                'SQRT': 'Math.sqrt',
            },
            Language.SQL: {
                'POWER': 'POWER',
                'LOG10': 'LOG10',
                'ABS': 'ABS',
                'SQRT': 'SQRT',
            },
        }

    def translate(self, formula: str, context: Optional[Dict] = None) -> str:
        """
        Translate an Excel formula to the target language.

        Args:
            formula: Excel formula (e.g., "=POWER({{ScaleFactor}}, {{Iteration}})")
            context: Additional context like table names, field types

        Returns:
            Translated code in target language
        """
        # Remove leading = sign
        if formula.startswith('='):
            formula = formula[1:]

        # Detect formula type
        if 'INDEX' in formula and 'MATCH' in formula:
            return self._translate_index_match(formula, context)
        elif 'COUNTIF' in formula:
            return self._translate_countif(formula, context)
        elif 'MINIFS' in formula or 'MAXIFS' in formula:
            return self._translate_minmax_ifs(formula, context)
        else:
            return self._translate_expression(formula, context)

    def _translate_expression(self, expr: str, context: Optional[Dict] = None) -> str:
        """Translate a simple expression or function call"""
        # Replace {{FieldName}} with appropriate variable access
        expr = self._replace_field_references(expr, context)

        # Replace Excel functions with target language functions
        for excel_func, target_func in self.function_map[self.language].items():
            # Match function calls: FUNCTION(args)
            pattern = rf'\b{excel_func}\s*\('
            expr = re.sub(pattern, f'{target_func}(', expr, flags=re.IGNORECASE)

        return expr

    def _translate_index_match(self, formula: str, context: Optional[Dict] = None) -> str:
        """
        Translate INDEX/MATCH lookup formula.

        Excel: =INDEX(systems!{{BaseScale}}, MATCH(scales!{{System}}, systems!{{SystemID}}, 0))
        Python: systems_dict[self.system].base_scale
        Golang: systemsMap[s.System].BaseScale
        TypeScript: systemsMap[scale.system].baseScale
        """
        # Extract the field being looked up and the foreign key
        # Pattern: INDEX(table!{{Field}}, MATCH(current_table!{{FKField}}, table!{{PKField}}, 0))

        index_match = r'INDEX\((\w+)!\{\{([^}]+)\}\},\s*MATCH\((\w+)!\{\{([^}]+)\}\},\s*(\w+)!\{\{([^}]+)\}\},\s*0\)\)'
        match = re.search(index_match, formula, re.IGNORECASE)

        if not match:
            return f"/* Could not parse INDEX/MATCH: {formula} */"

        target_table = match.group(1)  # e.g., "systems"
        target_field = match.group(2)  # e.g., "BaseScale"
        source_table = match.group(3)  # e.g., "scales"
        fk_field = match.group(4)      # e.g., "System"
        pk_field = match.group(6)      # e.g., "SystemID"

        # Generate code based on target language
        if self.language == Language.PYTHON:
            table_var = f"{target_table}_dict"
            fk_access = self._to_snake_case(fk_field)
            field_access = self._to_snake_case(target_field)
            return f"{table_var}.get(self.{fk_access}).{field_access} if {table_var}.get(self.{fk_access}) else None"

        elif self.language == Language.GOLANG:
            table_var = f"{target_table}Map"
            fk_access = self._to_pascal_case(fk_field)
            field_access = self._to_pascal_case(target_field)
            return f"{table_var}[s.{fk_access}].{field_access}"

        elif self.language in [Language.TYPESCRIPT, Language.JAVASCRIPT]:
            table_var = f"{target_table}Map"
            fk_access = self._to_camel_case(fk_field)
            field_access = self._to_camel_case(target_field)
            return f"{table_var}[scale.{fk_access}]?.{field_access}"

        return formula

    def _translate_countif(self, formula: str, context: Optional[Dict] = None) -> str:
        """
        Translate COUNTIF formula.

        Excel: =COUNTIF(scales!{{System}}, {{System}})
        Python: sum(1 for s in scales if s.system == self.system)
        Golang: count := 0; for _, s := range scales { if s.System == ss.System { count++ }}
        """
        # Pattern: COUNTIF(table!{{Field}}, {{Value}})
        pattern = r'COUNTIF\((\w+)!\{\{([^}]+)\}\},\s*\{\{([^}]+)\}\}\)'
        match = re.search(pattern, formula, re.IGNORECASE)

        if not match:
            return f"/* Could not parse COUNTIF: {formula} */"

        table = match.group(1)  # e.g., "scales"
        field = match.group(2)  # e.g., "System"
        value = match.group(3)  # e.g., "System"

        if self.language == Language.PYTHON:
            field_snake = self._to_snake_case(field)
            value_snake = self._to_snake_case(value)
            return f"sum(1 for s in {table} if s.{field_snake} == self.{value_snake})"

        elif self.language == Language.GOLANG:
            field_pascal = self._to_pascal_case(field)
            value_pascal = self._to_pascal_case(value)
            code = f"""
count := 0
for _, s := range {table} {{
    if s.{field_pascal} == ss.{value_pascal} {{
        count++
    }}
}}
return count
""".strip()
            return code

        elif self.language in [Language.TYPESCRIPT, Language.JAVASCRIPT]:
            field_camel = self._to_camel_case(field)
            value_camel = self._to_camel_case(value)
            return f"{table}.filter(s => s.{field_camel} === stats.{value_camel}).length"

        return formula

    def _translate_minmax_ifs(self, formula: str, context: Optional[Dict] = None) -> str:
        """
        Translate MINIFS/MAXIFS formulas.

        Excel: =MINIFS(scales!{{LogScale}}, scales!{{System}}, {{System}})
        Python: min((s.log_scale for s in scales if s.system == self.system), default=0)
        """
        # Determine if MIN or MAX
        is_min = 'MINIFS' in formula.upper()
        func_name = 'min' if is_min else 'max'

        # Pattern: MINIFS/MAXIFS(table!{{ResultField}}, table!{{CondField}}, {{CondValue}})
        pattern = r'(MIN|MAX)IFS\((\w+)!\{\{([^}]+)\}\},\s*(\w+)!\{\{([^}]+)\}\},\s*\{\{([^}]+)\}\}\)'
        match = re.search(pattern, formula, re.IGNORECASE)

        if not match:
            return f"/* Could not parse MINIFS/MAXIFS: {formula} */"

        table = match.group(2)       # e.g., "scales"
        result_field = match.group(3) # e.g., "LogScale"
        cond_field = match.group(5)   # e.g., "System"
        cond_value = match.group(6)   # e.g., "System"

        if self.language == Language.PYTHON:
            result_snake = self._to_snake_case(result_field)
            cond_field_snake = self._to_snake_case(cond_field)
            cond_value_snake = self._to_snake_case(cond_value)

            # Check if result field is a calculated field in the child table
            all_calc_fields = context.get('all_calculated_fields', {}) if context else {}
            child_table_calc_fields = all_calc_fields.get(table, set())
            is_result_calculated = result_field in child_table_calc_fields

            # For calculated fields, access the cached value
            if is_result_calculated:
                result_access = f"s._{result_snake}"
            else:
                result_access = f"s.{result_snake}"

            return f"{func_name}(({result_access} for s in {table} if s.{cond_field_snake} == self.{cond_value_snake}), default=0)"

        elif self.language == Language.GOLANG:
            result_pascal = self._to_pascal_case(result_field)
            cond_field_pascal = self._to_pascal_case(cond_field)
            cond_value_pascal = self._to_pascal_case(cond_value)

            comp_op = '<' if is_min else '>'
            init_val = 'math.MaxFloat64' if is_min else '-math.MaxFloat64'

            code = f"""
result := {init_val}
for _, s := range {table} {{
    if s.{cond_field_pascal} == ss.{cond_value_pascal} {{
        val := s.Calculate{result_pascal}({table}Map)
        if val {comp_op} result {{
            result = val
        }}
    }}
}}
return result
""".strip()
            return code

        elif self.language in [Language.TYPESCRIPT, Language.JAVASCRIPT]:
            result_camel = self._to_camel_case(result_field)
            cond_field_camel = self._to_camel_case(cond_field)
            cond_value_camel = self._to_camel_case(cond_value)

            func = 'Math.min' if is_min else 'Math.max'
            return f"{func}(...{table}.filter(s => s.{cond_field_camel} === stats.{cond_value_camel}).map(s => s.{result_camel}))"

        return formula

    def _replace_field_references(self, expr: str, context: Optional[Dict] = None) -> str:
        """Replace {{FieldName}} with appropriate variable access"""
        calculated_fields = context.get('calculated_fields', set()) if context else set()

        def replace_ref(match):
            field_name = match.group(1)
            is_calculated = field_name in calculated_fields

            if self.language == Language.PYTHON:
                snake_name = self._to_snake_case(field_name)
                # For calculated fields, reference the cached value
                if is_calculated:
                    return f"self._{snake_name}"
                return f"self.{snake_name}"
            elif self.language == Language.GOLANG:
                pascal_name = self._to_pascal_case(field_name)
                # For calculated fields in Go, reference the cached pointer value
                if is_calculated:
                    return f"*s.cached{pascal_name}"
                return f"s.{pascal_name}"
            elif self.language in [Language.TYPESCRIPT, Language.JAVASCRIPT]:
                return f"this.{self._to_camel_case(field_name)}"
            else:
                return match.group(0)

        return re.sub(r'\{\{([A-Za-z_][A-Za-z0-9_]*)\}\}', replace_ref, expr)

    def _to_snake_case(self, name: str) -> str:
        """Convert PascalCase/camelCase to snake_case"""
        # Insert underscore before capitals, then lowercase
        s1 = re.sub('(.)([A-Z][a-z]+)', r'\1_\2', name)
        return re.sub('([a-z0-9])([A-Z])', r'\1_\2', s1).lower()

    def _to_pascal_case(self, name: str) -> str:
        """Convert snake_case to PascalCase"""
        return ''.join(word.capitalize() for word in name.split('_'))

    def _to_camel_case(self, name: str) -> str:
        """Convert snake_case to camelCase"""
        pascal = self._to_pascal_case(name)
        return pascal[0].lower() + pascal[1:] if pascal else ''

    def get_required_imports(self) -> List[str]:
        """Get list of required imports for the target language"""
        if self.language == Language.PYTHON:
            return ['import math', 'from typing import Optional, Dict, List']
        elif self.language == Language.GOLANG:
            return ['import "math"']
        elif self.language in [Language.TYPESCRIPT, Language.JAVASCRIPT]:
            return []
        return []
