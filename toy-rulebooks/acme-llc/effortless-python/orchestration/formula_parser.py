#!/usr/bin/env python3
"""
Shared Formula Parser for ERB Execution Substrates

This module provides a reusable formula parser that converts Excel-dialect
formulas (from effortless-rulebook.json) into an expression tree.

Each substrate then compiles the expression tree to its target language:
- Python: compile_to_python()
- JavaScript: compile_to_javascript()
- Go: compile_to_go()
- SPARQL: compile_to_sparql()

Extracted from: execution-substrates/owl/inject-into-owl.py
"""

import datetime
import math
import re
from dataclasses import dataclass
from typing import List, Any
from enum import Enum, auto


# =============================================================================
# EXPRESSION NODE TYPES
# =============================================================================

@dataclass
class ExprNode:
    """Base class for expression nodes"""
    pass


@dataclass
class LiteralBool(ExprNode):
    value: bool


@dataclass
class LiteralInt(ExprNode):
    value: int


@dataclass
class LiteralFloat(ExprNode):
    value: float


@dataclass
class LiteralString(ExprNode):
    value: str


@dataclass
class FieldRef(ExprNode):
    name: str  # Field name without {{ }}


@dataclass
class BinaryOp(ExprNode):
    # '=', '<>', '<', '<=', '>', '>=', '+', '-', '*', '/'
    op: str
    left: ExprNode
    right: ExprNode


@dataclass
class UnaryOp(ExprNode):
    op: str  # 'NOT' or '-'
    operand: ExprNode


@dataclass
class FuncCall(ExprNode):
    name: str  # 'AND', 'OR', 'IF', 'LOWER', 'FIND', 'CAST'
    args: List[ExprNode]


@dataclass
class Concat(ExprNode):
    parts: List[ExprNode]


# =============================================================================
# LEXER
# =============================================================================

class TokenType(Enum):
    STRING = auto()
    NUMBER = auto()
    FIELD_REF = auto()
    FUNC_NAME = auto()
    LPAREN = auto()
    RPAREN = auto()
    COMMA = auto()
    AMPERSAND = auto()
    EQUALS = auto()
    NOT_EQUALS = auto()
    LT = auto()
    LE = auto()
    GT = auto()
    GE = auto()
    PLUS = auto()
    MINUS = auto()
    STAR = auto()
    SLASH = auto()
    EOF = auto()


@dataclass
class Token:
    type: TokenType
    value: Any
    pos: int


def tokenize(formula: str) -> List[Token]:
    """Tokenize an Excel-dialect formula."""
    tokens = []

    # Remove leading = if present
    if formula.startswith('='):
        formula = formula[1:]

    i = 0
    while i < len(formula):
        c = formula[i]

        # Skip whitespace
        if c in ' \t\n\r':
            i += 1
            continue

        # String literal — both double (") and single (') quotes delimit a
        # string, matching the Excel/Airtable dialect. Single quotes appear in
        # unit args like DATETIME_DIFF(..., 'hours') / DATEADD(..., 'days').
        if c == '"' or c == "'":
            quote = c
            j = i + 1
            while j < len(formula) and formula[j] != quote:
                if formula[j] == '\\':
                    j += 2
                else:
                    j += 1
            if j >= len(formula):
                raise SyntaxError(f"Unterminated string at position {i}")
            value = formula[i+1:j]
            tokens.append(Token(TokenType.STRING, value, i))
            i = j + 1
            continue

        # Field reference {{Name}}
        if formula[i:i+2] == '{{':
            j = formula.find('}}', i)
            if j == -1:
                raise SyntaxError(f"Unterminated field reference at position {i}")
            field_name = formula[i+2:j]
            tokens.append(Token(TokenType.FIELD_REF, field_name, i))
            i = j + 2
            continue

        # Number (int or float). Unary minus is handled by the parser, so
        # we never bake the sign into a NUMBER token.
        if c.isdigit() or (c == '.' and i + 1 < len(formula) and formula[i+1].isdigit()):
            j = i
            while j < len(formula) and formula[j].isdigit():
                j += 1
            is_float = False
            if j < len(formula) and formula[j] == '.':
                is_float = True
                j += 1
                while j < len(formula) and formula[j].isdigit():
                    j += 1
            value = float(formula[i:j]) if is_float else int(formula[i:j])
            tokens.append(Token(TokenType.NUMBER, value, i))
            i = j
            continue

        # Operators
        if formula[i:i+2] == '<>':
            tokens.append(Token(TokenType.NOT_EQUALS, '<>', i))
            i += 2
            continue
        if formula[i:i+2] == '<=':
            tokens.append(Token(TokenType.LE, '<=', i))
            i += 2
            continue
        if formula[i:i+2] == '>=':
            tokens.append(Token(TokenType.GE, '>=', i))
            i += 2
            continue
        if c == '<':
            tokens.append(Token(TokenType.LT, '<', i))
            i += 1
            continue
        if c == '>':
            tokens.append(Token(TokenType.GT, '>', i))
            i += 1
            continue
        if c == '=':
            tokens.append(Token(TokenType.EQUALS, '=', i))
            i += 1
            continue
        if c == '&':
            tokens.append(Token(TokenType.AMPERSAND, '&', i))
            i += 1
            continue
        if c == '(':
            tokens.append(Token(TokenType.LPAREN, '(', i))
            i += 1
            continue
        if c == ')':
            tokens.append(Token(TokenType.RPAREN, ')', i))
            i += 1
            continue
        if c == ',':
            tokens.append(Token(TokenType.COMMA, ',', i))
            i += 1
            continue
        if c == '+':
            tokens.append(Token(TokenType.PLUS, '+', i))
            i += 1
            continue
        if c == '-':
            tokens.append(Token(TokenType.MINUS, '-', i))
            i += 1
            continue
        if c == '*':
            tokens.append(Token(TokenType.STAR, '*', i))
            i += 1
            continue
        if c == '/':
            tokens.append(Token(TokenType.SLASH, '/', i))
            i += 1
            continue

        # Function names / identifiers
        if c.isalpha() or c == '_':
            j = i
            while j < len(formula) and (formula[j].isalnum() or formula[j] == '_'):
                j += 1
            name = formula[i:j].upper()
            tokens.append(Token(TokenType.FUNC_NAME, name, i))
            i = j
            continue

        raise SyntaxError(f"Unexpected character '{c}' at position {i}")

    tokens.append(Token(TokenType.EOF, None, len(formula)))
    return tokens


# =============================================================================
# PARSER
# =============================================================================

class Parser:
    """Recursive descent parser for Excel-dialect formulas."""

    def __init__(self, tokens: List[Token]):
        self.tokens = tokens
        self.pos = 0

    def current(self) -> Token:
        return self.tokens[self.pos]

    def consume(self, expected: TokenType = None) -> Token:
        tok = self.current()
        if expected and tok.type != expected:
            raise SyntaxError(f"Expected {expected}, got {tok.type} at position {tok.pos}")
        self.pos += 1
        return tok

    def parse(self) -> ExprNode:
        result = self.parse_concat()
        if self.current().type != TokenType.EOF:
            raise SyntaxError(f"Unexpected token {self.current()} after expression")
        return result

    def parse_concat(self) -> ExprNode:
        left = self.parse_comparison()
        parts = [left]
        while self.current().type == TokenType.AMPERSAND:
            self.consume(TokenType.AMPERSAND)
            right = self.parse_comparison()
            parts.append(right)
        if len(parts) == 1:
            return parts[0]
        return Concat(parts=parts)

    def parse_comparison(self) -> ExprNode:
        left = self.parse_addition()
        op_map = {
            TokenType.EQUALS: '=',
            TokenType.NOT_EQUALS: '<>',
            TokenType.LT: '<',
            TokenType.LE: '<=',
            TokenType.GT: '>',
            TokenType.GE: '>=',
        }
        if self.current().type in op_map:
            op = op_map[self.current().type]
            self.consume()
            right = self.parse_addition()
            return BinaryOp(op=op, left=left, right=right)
        return left

    def parse_addition(self) -> ExprNode:
        left = self.parse_multiplication()
        while self.current().type in (TokenType.PLUS, TokenType.MINUS):
            op = '+' if self.current().type == TokenType.PLUS else '-'
            self.consume()
            right = self.parse_multiplication()
            left = BinaryOp(op=op, left=left, right=right)
        return left

    def parse_multiplication(self) -> ExprNode:
        left = self.parse_unary()
        while self.current().type in (TokenType.STAR, TokenType.SLASH):
            op = '*' if self.current().type == TokenType.STAR else '/'
            self.consume()
            right = self.parse_unary()
            left = BinaryOp(op=op, left=left, right=right)
        return left

    def parse_unary(self) -> ExprNode:
        if self.current().type == TokenType.MINUS:
            self.consume()
            operand = self.parse_unary()
            return UnaryOp(op='-', operand=operand)
        if self.current().type == TokenType.PLUS:
            self.consume()
            return self.parse_unary()
        return self.parse_primary()

    def parse_primary(self) -> ExprNode:
        tok = self.current()

        if tok.type == TokenType.STRING:
            self.consume()
            return LiteralString(value=tok.value)

        if tok.type == TokenType.NUMBER:
            self.consume()
            if isinstance(tok.value, float):
                return LiteralFloat(value=tok.value)
            return LiteralInt(value=tok.value)

        if tok.type == TokenType.FIELD_REF:
            self.consume()
            return FieldRef(name=tok.value)

        if tok.type == TokenType.FUNC_NAME:
            name = tok.value.upper()
            self.consume()

            if name == 'TRUE':
                if self.current().type == TokenType.LPAREN:
                    self.consume(TokenType.LPAREN)
                    self.consume(TokenType.RPAREN)
                return LiteralBool(value=True)

            if name == 'FALSE':
                if self.current().type == TokenType.LPAREN:
                    self.consume(TokenType.LPAREN)
                    self.consume(TokenType.RPAREN)
                return LiteralBool(value=False)

            self.consume(TokenType.LPAREN)
            args = []
            if self.current().type != TokenType.RPAREN:
                args.append(self.parse_concat())
                while self.current().type == TokenType.COMMA:
                    self.consume(TokenType.COMMA)
                    args.append(self.parse_concat())
            self.consume(TokenType.RPAREN)

            if name == 'NOT' and len(args) == 1:
                return UnaryOp(op='NOT', operand=args[0])

            return FuncCall(name=name, args=args)

        if tok.type == TokenType.LPAREN:
            self.consume(TokenType.LPAREN)
            expr = self.parse_concat()
            self.consume(TokenType.RPAREN)
            return expr

        raise SyntaxError(f"Unexpected token {tok.type} at position {tok.pos}")


def parse_formula(formula_text: str) -> ExprNode:
    """Parse an Excel-dialect formula into an expression tree."""
    tokens = tokenize(formula_text)
    parser = Parser(tokens)
    return parser.parse()


# =============================================================================
# HELPER FUNCTIONS
# =============================================================================

def to_snake_case(name: str) -> str:
    """Convert PascalCase/CamelCase to snake_case.

    Examples:
        HasLinearDecodingPressure -> has_linear_decoding_pressure
        StableOntologyReference -> stable_ontology_reference
        Bio_HockettScore -> bio_hockett_score
        Name -> name
    """
    # Use [^_] to avoid doubling underscores when input already has them
    s1 = re.sub('([^_])([A-Z][a-z]+)', r'\1_\2', name)
    return re.sub('([a-z0-9])([A-Z])', r'\1_\2', s1).lower()


def to_camel_case(name: str) -> str:
    """Convert PascalCase to camelCase.

    Examples:
        HasLinearDecodingPressure -> hasLinearDecodingPressure
        Name -> name
    """
    if not name:
        return name
    return name[0].lower() + name[1:]


def to_pascal_case(snake_name: str) -> str:
    """Convert snake_case to PascalCase.

    Examples:
        has_linear_decoding_pressure -> HasLinearDecodingPressure
        name -> Name
    """
    return ''.join(word.capitalize() for word in snake_name.split('_'))


def get_field_dependencies(expr: ExprNode) -> List[str]:
    """Extract all field references from an expression tree.

    Returns a list of field names (PascalCase as they appear in formulas).
    Used for DAG ordering and dependency tracking.
    """
    deps = []

    def visit(node: ExprNode):
        if isinstance(node, FieldRef):
            if node.name not in deps:
                deps.append(node.name)
        elif isinstance(node, BinaryOp):
            visit(node.left)
            visit(node.right)
        elif isinstance(node, UnaryOp):
            visit(node.operand)
        elif isinstance(node, FuncCall):
            for arg in node.args:
                visit(arg)
        elif isinstance(node, Concat):
            for part in node.parts:
                visit(part)

    visit(expr)
    return deps


# =============================================================================
# PYTHON CODE GENERATOR
# =============================================================================

# The schema of the table whose formulas are being compiled, keyed by snake_case
# field name. The Postgres oracle reads a raw column differently from a
# calculated one — NULLIF(text, '') for a raw value, COALESCE(bool, FALSE) for a
# raw boolean in a boolean context, a bare calc_* call (NULL propagates) for a
# derived value — so compiled Python cannot agree with it without knowing which
# kind each {{Field}} is. Set per table with set_compile_fields().
_COMPILE_FIELDS = None


def set_compile_fields(schema):
    """Name the table (its schema field list) that compile_to_python compiles for."""
    global _COMPILE_FIELDS
    _COMPILE_FIELDS = {to_snake_case(f['name']): f for f in schema}


def _referenced_field(expr: ExprNode):
    """The schema entry a FieldRef names, or None for any other node."""
    if not isinstance(expr, FieldRef):
        return None
    if _COMPILE_FIELDS is None:
        raise RuntimeError(
            "compile_to_python needs the table's schema: call "
            "set_compile_fields(schema) before compiling its formulas.")
    field = _COMPILE_FIELDS.get(to_snake_case(expr.name))
    if field is None:
        raise ValueError(f"{{{{{expr.name}}}}} is not a field of the table being compiled")
    return field


def _is_raw_field(field) -> bool:
    return field is not None and field.get('type', 'raw') in ('raw', 'relationship')


def _compile_value(expr: ExprNode) -> str:
    """An operand read the way the oracle reads it: a raw column is NULLIF(x, ''),
    so a blank raw value compares, finds and counts as NULL."""
    compiled = compile_to_python(expr)
    if _is_raw_field(_referenced_field(expr)):
        return f'_erb.erb_nullif({compiled})'
    return compiled


def _compile_condition(expr: ExprNode) -> str:
    """An AND/OR/NOT/IF operand, coerced the way the oracle coerces it to boolean.

    A raw boolean column is COALESCE(x, FALSE); any other raw column is
    `x IS NOT NULL` over NULLIF(x, ''). A derived value is left alone, so a NULL
    calculated boolean stays NULL and three-valued logic applies to it.
    """
    compiled = compile_to_python(expr)
    field = _referenced_field(expr)
    if _is_raw_field(field):
        if field.get('datatype') == 'boolean':
            return f'({compiled} is True)'
        return f'(not _erb.erb_blank({compiled}))'
    return f'_erb.erb_bool3({compiled})'


def erb_bool3(value):
    """A three-valued boolean: TRUE, FALSE or NULL (None). Anything that is not a
    boolean reads as FALSE, as it always has in compiled code."""
    if value is None or isinstance(value, bool):
        return value
    return False


def erb_and(*values):
    """SQL AND: FALSE if any operand is FALSE, else NULL if any is NULL, else TRUE."""
    if any(v is False for v in values):
        return False
    if any(v is None for v in values):
        return None
    return True


def erb_or(*values):
    """SQL OR: TRUE if any operand is TRUE, else NULL if any is NULL, else FALSE."""
    if any(v is True for v in values):
        return True
    if any(v is None for v in values):
        return None
    return False


def erb_not(value):
    """SQL NOT: NOT NULL is NULL."""
    return None if value is None else (not value)


def erb_nullif(value):
    """NULLIF(x, ''): the oracle stores a blank raw text value as NULL."""
    return None if value == '' else value


def erb_eq(a, b):
    """SQL `=`: NULL when either side is NULL."""
    if a is None or b is None:
        return None
    return a == b


def erb_ne(a, b):
    """SQL `<>`: NULL when either side is NULL."""
    if a is None or b is None:
        return None
    return a != b


def erb_find(needle, haystack):
    """FIND(needle, haystack) as the oracle's POSITION(needle IN haystack): the
    1-based position, 0 when absent, NULL when either side is NULL."""
    if needle is None or haystack is None:
        return None
    return str(haystack).find(str(needle)) + 1


def erb_roundup(value, digits=0):
    """ROUNDUP(x, d) as the oracle's CEIL(x * 10^d) / 10^d. NULL propagates."""
    if value is None or value == '':
        return None
    from decimal import Decimal, ROUND_CEILING
    d = int(digits) if digits is not None and digits != '' else 0
    quant = Decimal(1).scaleb(-d)
    return float(Decimal(str(value)).quantize(quant, rounding=ROUND_CEILING))


def erb_integer(value):
    """A field declared `integer` is cast `::integer` by the oracle, which rounds
    half away from zero. NULL stays NULL; a value that is not a number is
    returned unchanged."""
    if value is None or isinstance(value, bool):
        return value
    number = _to_number(value)
    if number is None:
        return value
    from decimal import Decimal, ROUND_HALF_UP
    return int(Decimal(str(number)).quantize(Decimal(1), rounding=ROUND_HALF_UP))


def erb_timestamptz_text(value):
    """How the oracle renders a datetime inside CONCAT: a timestamptz cast to
    text — `2026-01-01 00:00:00-06`, a space instead of the ISO `T`, and the UTC
    offset shortened to hours unless it has minutes. NULL renders as ''."""
    if value is None or value == '':
        return ''
    if isinstance(value, datetime.datetime):
        dt = value
    else:
        dt = datetime.datetime.fromisoformat(str(value).replace('Z', '+00:00'))
    if dt.tzinfo is None:
        raise ValueError(
            f"datetime {value!r} carries no UTC offset, so its timestamptz text "
            f"depends on the database session's time zone and cannot be rendered")
    text = dt.strftime('%Y-%m-%d %H:%M:%S')
    if dt.microsecond:
        text += ('.%06d' % dt.microsecond).rstrip('0')
    offset_minutes = int(dt.utcoffset().total_seconds() // 60)
    sign = '-' if offset_minutes < 0 else '+'
    hours, minutes = divmod(abs(offset_minutes), 60)
    text += f'{sign}{hours:02d}' + (f':{minutes:02d}' if minutes else '')
    return text


def _compile_concat_part(part: ExprNode) -> str:
    """One CONCAT / `&` operand. NULL contributes nothing; a datetime renders as
    the oracle's timestamptz text rather than Python's ISO string."""
    if isinstance(part, LiteralString):
        return repr(part.value)
    compiled = compile_to_python(part)
    field = _referenced_field(part)
    if field is not None:
        if field.get('datatype') == 'datetime':
            return f'_erb.erb_timestamptz_text({compiled})'
        return f'str({compiled} or "")'
    return f'str({compiled} if {compiled} is not None else "")'


def erb_cmp(a, op, b):
    """Ordered comparison (< <= > >=) for emitted code, with the same rules as
    the interpreter's _compare_operands: numbers and numeric strings compare as
    numbers, two strings compare as strings, and anything that cannot be ordered
    at all is False rather than a TypeError that would take the whole generated
    module down. A NULL operand makes the comparison NULL, as it does in the
    Postgres oracle — so NOT({{X}} > 0) over a NULL X stays NULL, not TRUE.
    """
    if a is None or b is None:
        return None
    pair = _compare_operands(a, b)
    if pair is None:
        return False
    left, right = pair
    if op == '<':
        return left < right
    if op == '<=':
        return left <= right
    if op == '>':
        return left > right
    return left >= right


def compile_to_python(expr: ExprNode) -> str:
    """Compile an expression tree to a Python expression.

    Handles None values by using 'is True' and 'is not True' patterns.
    Field references are converted to snake_case variable names.
    """
    if isinstance(expr, LiteralBool):
        return 'True' if expr.value else 'False'

    if isinstance(expr, LiteralInt):
        return str(expr.value)

    if isinstance(expr, LiteralFloat):
        return str(expr.value)

    if isinstance(expr, LiteralString):
        return repr(expr.value)

    if isinstance(expr, FieldRef):
        return to_snake_case(expr.name)

    if isinstance(expr, UnaryOp):
        if expr.op == 'NOT':
            return f'_erb.erb_not({_compile_condition(expr.operand)})'
        if expr.op == '-':
            # Numeric negation, e.g. IF(cond, -50, ...). None coerces to 0,
            # matching the None-safety convention BinaryOp uses for +/-/*//.
            operand = compile_to_python(expr.operand)
            return f'(-(({operand}) or 0))'
        raise ValueError(f"Unknown unary op: {expr.op}")

    if isinstance(expr, BinaryOp):
        left = compile_to_python(expr.left)
        right = compile_to_python(expr.right)
        if expr.op == '+':
            # erb_add falls back to plain None-safe numeric addition unless one
            # side is a recognizable date, in which case it's date-plus-days
            # (the dialect's spelling of SQL `date + integer`).
            return f'_erb.erb_add({left}, {right})'
        if expr.op == '-':
            return f'_erb.erb_sub({left}, {right})'
        if expr.op == '*':
            # Routed through the runtime rather than emitted as a bare Python
            # `*`: a declared number that arrived as a string would otherwise be
            # string-repeated ('12000' * 2 -> '1200012000') silently. erb_mul
            # coerces the same way the interpreter does.
            return f'_erb.erb_mul({left}, {right})'
        if expr.op == '/':
            # Same coercion, plus divide-by-zero -> None instead of
            # ZeroDivisionError (matches the interpreter and the SQL oracle).
            return f'_erb.erb_div({left}, {right})'
        if expr.op in ('<', '<=', '>', '>='):
            # Routed through the runtime rather than emitted as a bare Python
            # comparison. The None guard this replaces still raised TypeError on
            # a declared number that arrived as a string ('12000' > 0), which is
            # the one case the guard could not see. erb_cmp applies the
            # interpreter's own rules, so the two agree by construction.
            return f"_erb.erb_cmp({_compile_value(expr.left)}, '{expr.op}', {_compile_value(expr.right)})"
        if expr.op in ('=', '<>') and _is_empty_string_literal(expr.right):
            # Comparing against "" is the dialect's blank check, and it is
            # null-safe: a NULL column is blank exactly as an empty string is.
            # Plain Python disagrees (None != "" is True), which made a NULL
            # read as present and broke the override/resolve idiom.
            test = f'({left} is None or {left} == "")'
            return test if expr.op == '=' else f'(not {test})'
        if expr.op in ('=', '<>') and _is_empty_string_literal(expr.left):
            test = f'({right} is None or {right} == "")'
            return test if expr.op == '=' else f'(not {test})'

        helper = 'erb_eq' if expr.op == '=' else 'erb_ne'
        return f'_erb.{helper}({_compile_value(expr.left)}, {_compile_value(expr.right)})'

    if isinstance(expr, FuncCall):
        if expr.name == 'AND':
            return '_erb.erb_and(' + ', '.join(_compile_condition(a) for a in expr.args) + ')'

        if expr.name == 'OR':
            return '_erb.erb_or(' + ', '.join(_compile_condition(a) for a in expr.args) + ')'

        if expr.name == 'IF':
            if len(expr.args) < 2:
                raise ValueError("IF requires at least 2 arguments")
            cond = _compile_condition(expr.args[0])
            then_val = compile_to_python(expr.args[1])
            else_val = compile_to_python(expr.args[2]) if len(expr.args) > 2 else 'None'
            return f'({then_val} if {cond} else {else_val})'

        if expr.name == 'NOT':
            if len(expr.args) != 1:
                raise ValueError("NOT requires 1 argument")
            return f'_erb.erb_not({_compile_condition(expr.args[0])})'

        if expr.name == 'LOWER':
            if len(expr.args) != 1:
                raise ValueError("LOWER requires 1 argument")
            arg = compile_to_python(expr.args[0])
            return f'(({arg} or "").lower())'

        if expr.name == 'UPPER':
            if len(expr.args) != 1:
                raise ValueError("UPPER requires 1 argument")
            arg = compile_to_python(expr.args[0])
            return f'(({arg} or "").upper())'

        if expr.name == 'LEN':
            if len(expr.args) != 1:
                raise ValueError("LEN requires 1 argument")
            arg = compile_to_python(expr.args[0])
            return f'len({arg} or "")'

        if expr.name == 'LEFT':
            if len(expr.args) != 2:
                raise ValueError("LEFT requires 2 arguments")
            text = compile_to_python(expr.args[0])
            count = compile_to_python(expr.args[1])
            return f'(({text} or "")[:({count} or 0)])'

        if expr.name == 'RIGHT':
            if len(expr.args) != 2:
                raise ValueError("RIGHT requires 2 arguments")
            text = compile_to_python(expr.args[0])
            count = compile_to_python(expr.args[1])
            return f'_erb.erb_right({text}, {count})'

        if expr.name == 'MID':
            if len(expr.args) != 3:
                raise ValueError("MID requires 3 arguments")
            text = compile_to_python(expr.args[0])
            start = compile_to_python(expr.args[1])
            count = compile_to_python(expr.args[2])
            return f'_erb.erb_mid({text}, {start}, {count})'

        if expr.name == 'CONCAT':
            # Variadic string join — same None-safety convention as the `&`
            # operator's Concat node below (FieldRef -> "" for None; any other
            # expression -> "" only when None, so a real empty string survives).
            return '(' + ' + '.join(_compile_concat_part(a) for a in expr.args) + ')'

        if expr.name == 'COALESCE':
            if not expr.args:
                raise ValueError("COALESCE requires at least 1 argument")
            parts = [compile_to_python(a) for a in expr.args]
            return '_erb.erb_coalesce(' + ', '.join(parts) + ')'

        if expr.name == 'IFERROR':
            if len(expr.args) != 2:
                raise ValueError("IFERROR requires 2 arguments")
            main_expr = compile_to_python(expr.args[0])
            fallback_expr = compile_to_python(expr.args[1])
            return f'_erb.erb_try(lambda: ({main_expr}), lambda: ({fallback_expr}))'

        if expr.name == 'ISERROR':
            if len(expr.args) != 1:
                raise ValueError("ISERROR requires 1 argument")
            main_expr = compile_to_python(expr.args[0])
            return f'_erb.erb_is_error(lambda: ({main_expr}))'

        if expr.name == 'ABS':
            if len(expr.args) != 1:
                raise ValueError("ABS requires 1 argument")
            arg = compile_to_python(expr.args[0])
            return f'abs(({arg}) or 0)'

        if expr.name == 'POWER':
            if len(expr.args) != 2:
                raise ValueError("POWER requires 2 arguments")
            base = compile_to_python(expr.args[0])
            exponent = compile_to_python(expr.args[1])
            return f'((({base}) or 0) ** (({exponent}) or 0))'

        if expr.name == 'SQRT':
            if len(expr.args) != 1:
                raise ValueError("SQRT requires 1 argument")
            arg = compile_to_python(expr.args[0])
            return f'_erb.math.sqrt(({arg}) or 0)'

        if expr.name == 'LOG':
            if len(expr.args) not in (1, 2):
                raise ValueError("LOG requires 1 or 2 arguments")
            arg = compile_to_python(expr.args[0])
            if len(expr.args) == 2:
                base = compile_to_python(expr.args[1])
                return f'_erb.math.log(({arg}) or 0, ({base}) or 0)'
            return f'_erb.math.log10(({arg}) or 0)'

        if expr.name == 'TAN':
            if len(expr.args) != 1:
                raise ValueError("TAN requires 1 argument")
            arg = compile_to_python(expr.args[0])
            return f'_erb.math.tan(({arg}) or 0)'

        if expr.name == 'PI':
            if expr.args:
                raise ValueError("PI takes no arguments")
            return '_erb.math.pi'

        if expr.name in ('MAX', 'MIN'):
            # Scalar element-wise MAX/MIN over inline arguments, e.g.
            # MAX(0, {{DaysSinceDueDate}} - {{GraceDays}}). Distinct from the
            # bare-table-rollup MAX(Table!{{Field}})/MAXIFS(...) that
            # compute_aggregations' parse_ifs_aggregate() handles for
            # `type: aggregation` fields — a `type: calculated` field's inline
            # MAX/MIN never references another table, so there's no ambiguity
            # at the point either interpreter actually runs.
            if not expr.args:
                raise ValueError(f"{expr.name} requires at least 1 argument")
            parts = [f'(({compile_to_python(a)}) or 0)' for a in expr.args]
            pyfunc = 'max' if expr.name == 'MAX' else 'min'
            return f'{pyfunc}(' + ', '.join(parts) + ')'

        if expr.name == 'FIND':
            if len(expr.args) != 2:
                raise ValueError("FIND requires 2 arguments")
            return f'_erb.erb_find({_compile_value(expr.args[0])}, {_compile_value(expr.args[1])})'

        if expr.name == 'CAST':
            # CAST(x AS TEXT) -> str(x) if x else ""
            if len(expr.args) >= 1:
                arg = compile_to_python(expr.args[0])
                return f'(str({arg}) if {arg} else "")'
            raise ValueError("CAST requires at least 1 argument")

        if expr.name == 'SUM':
            # SUM(a, b, c) -> (a + b + c)
            # Typically used as SUM(IF(cond,1,0), IF(cond,1,0), ...)
            if not expr.args:
                return '0'
            parts = [compile_to_python(arg) for arg in expr.args]
            return '(' + ' + '.join(parts) + ')'

        if expr.name == 'SUBSTITUTE':
            # SUBSTITUTE(text, old_text, new_text) -> text.replace(old_text, new_text)
            if len(expr.args) != 3:
                raise ValueError("SUBSTITUTE requires 3 arguments")
            text = compile_to_python(expr.args[0])
            old_text = compile_to_python(expr.args[1])
            new_text = compile_to_python(expr.args[2])
            return f'(({text} or "").replace({old_text}, {new_text}))'

        if expr.name == 'BLANK':
            # BLANK() -> None (empty/null value)
            return 'None'

        if expr.name == 'ISBLANK':
            # ISBLANK(x) -> x is None or x == ""
            # Matches the SQL the Postgres transpiler emits (IS NULL OR
            # ::text = ''): absent relationships are stored as '' rather
            # than NULL, so both spellings of "absent" must count as blank.
            if len(expr.args) != 1:
                raise ValueError("ISBLANK requires 1 argument")
            arg = compile_to_python(expr.args[0])
            return f'({arg} is None or {arg} == "")'

        if expr.name == 'ROUNDUP':
            if len(expr.args) not in (1, 2):
                raise ValueError("ROUNDUP requires 1 or 2 arguments")
            value = compile_to_python(expr.args[0])
            digits = compile_to_python(expr.args[1]) if len(expr.args) == 2 else '0'
            return f'_erb.erb_roundup({value}, {digits})'

        if expr.name == 'ROUND':
            if len(expr.args) not in (1, 2):
                raise ValueError("ROUND requires 1 or 2 arguments")
            value = compile_to_python(expr.args[0])
            digits = compile_to_python(expr.args[1]) if len(expr.args) == 2 else '0'
            return f'_erb.erb_round({value}, {digits})'

        if expr.name in ('NOW', 'TODAY'):
            return '_erb.erb_now()'

        if expr.name in ('DATETIME_DIFF', 'DATEDIFF'):
            # Delegates to the interpreter's helper so the calendar-month
            # semantics that match the Postgres oracle have exactly one
            # implementation to drift from.
            if len(expr.args) < 2:
                raise ValueError(f"{expr.name} requires at least 2 arguments")
            end = compile_to_python(expr.args[0])
            start = compile_to_python(expr.args[1])
            unit = compile_to_python(expr.args[2]) if len(expr.args) > 2 else '"day"'
            return f'_erb.erb_datetime_diff({end}, {start}, {unit})'

        raise ValueError(f"Unknown function: {expr.name}")

    if isinstance(expr, Concat):
        return '(' + ' + '.join(_compile_concat_part(p) for p in expr.parts) + ')'

    raise ValueError(f"Unknown expression node type: {type(expr)}")


# =============================================================================
# JAVASCRIPT CODE GENERATOR
# =============================================================================

def compile_to_javascript(expr: ExprNode, obj_name: str = 'candidate') -> str:
    """Compile an expression tree to a JavaScript expression.

    Uses explicit === true / !== true for proper null handling.
    Field references use camelCase with object prefix.
    """
    if isinstance(expr, LiteralBool):
        return 'true' if expr.value else 'false'

    if isinstance(expr, LiteralInt):
        return str(expr.value)

    if isinstance(expr, LiteralString):
        escaped = expr.value.replace('\\', '\\\\').replace("'", "\\'")
        return f"'{escaped}'"

    if isinstance(expr, FieldRef):
        return f'{obj_name}.{to_camel_case(expr.name)}'

    if isinstance(expr, UnaryOp):
        if expr.op == 'NOT':
            operand = compile_to_javascript(expr.operand, obj_name)
            return f'({operand} !== true)'
        raise ValueError(f"Unknown unary op: {expr.op}")

    if isinstance(expr, BinaryOp):
        left = compile_to_javascript(expr.left, obj_name)
        right = compile_to_javascript(expr.right, obj_name)
        # Arithmetic first. These were absent from op_map entirely, so every
        # arithmetic formula raised KeyError inside the compiler. Operands go
        # through Number(x) || 0, which gives the same answers as the
        # interpreter and the SQL oracle: a numeric string converts, blank and
        # null and unparsable text all become 0.
        if expr.op in ('+', '-', '*'):
            return f'((Number({left}) || 0) {expr.op} (Number({right}) || 0))'
        if expr.op == '/':
            # Divide-by-zero is null, not Infinity — matching the interpreter.
            return (f'((Number({right}) || 0) === 0 ? null : '
                    f'(Number({left}) || 0) / (Number({right}) || 0))')

        op_map = {'=': '===', '<>': '!==', '<': '<', '<=': '<=', '>': '>', '>=': '>='}
        return f'({left} {op_map[expr.op]} {right})'

    if isinstance(expr, FuncCall):
        if expr.name == 'AND':
            parts = [f'({compile_to_javascript(arg, obj_name)} === true)' for arg in expr.args]
            return '(' + ' && '.join(parts) + ')'

        if expr.name == 'OR':
            parts = [f'({compile_to_javascript(arg, obj_name)} === true)' for arg in expr.args]
            return '(' + ' || '.join(parts) + ')'

        if expr.name == 'IF':
            if len(expr.args) < 2:
                raise ValueError("IF requires at least 2 arguments")
            cond = compile_to_javascript(expr.args[0], obj_name)
            then_val = compile_to_javascript(expr.args[1], obj_name)
            else_val = compile_to_javascript(expr.args[2], obj_name) if len(expr.args) > 2 else 'null'
            return f'({cond} ? {then_val} : {else_val})'

        if expr.name == 'NOT':
            if len(expr.args) != 1:
                raise ValueError("NOT requires 1 argument")
            operand = compile_to_javascript(expr.args[0], obj_name)
            return f'({operand} !== true)'

        if expr.name == 'LOWER':
            if len(expr.args) != 1:
                raise ValueError("LOWER requires 1 argument")
            arg = compile_to_javascript(expr.args[0], obj_name)
            return f'(({arg} || "").toLowerCase())'

        if expr.name == 'FIND':
            if len(expr.args) != 2:
                raise ValueError("FIND requires 2 arguments")
            needle = compile_to_javascript(expr.args[0], obj_name)
            haystack = compile_to_javascript(expr.args[1], obj_name)
            return f'(({haystack} || "").includes({needle}))'

        if expr.name == 'CAST':
            if len(expr.args) >= 1:
                arg = compile_to_javascript(expr.args[0], obj_name)
                return f'({arg} ? String({arg}) : "")'
            raise ValueError("CAST requires at least 1 argument")

        if expr.name == 'SUM':
            # SUM(a, b, c) -> (a + b + c)
            if not expr.args:
                return '0'
            parts = [compile_to_javascript(arg, obj_name) for arg in expr.args]
            return '(' + ' + '.join(parts) + ')'

        if expr.name == 'BLANK':
            # BLANK() -> null
            return 'null'

        raise ValueError(f"Unknown function: {expr.name}")

    if isinstance(expr, Concat):
        parts = []
        for part in expr.parts:
            if isinstance(part, LiteralString):
                escaped = part.value.replace('\\', '\\\\').replace('`', '\\`').replace('$', '\\$')
                parts.append(escaped)
            else:
                var = compile_to_javascript(part, obj_name)
                parts.append('${' + f'{var} || ""' + '}')
        return '`' + ''.join(parts) + '`'

    raise ValueError(f"Unknown expression node type: {type(expr)}")


# =============================================================================
# GO CODE GENERATOR
# =============================================================================

def _go_accessor_for(*nodes) -> str:
    """Pick the nil-safe Go accessor for a comparison between field refs.

    field_types is the trailing argument. Integer fields need intVal, strings
    stringVal, and booleans boolVal; a mixed or unknown pair falls back to
    stringVal, which is defined for any pointer field.
    """
    *refs, field_types = nodes
    field_types = field_types or {}
    datatypes = {
        (field_types.get(node.name) or 'string').lower()
        for node in refs if isinstance(node, FieldRef)
    }
    if datatypes == {'integer'}:
        return 'intVal'
    if datatypes == {'boolean'}:
        return 'boolVal'
    return 'stringVal'


def _compile_to_go_int(expr: ExprNode, struct_name: str, field_types: dict) -> str:
    """Compile an expression node to a Go expression that returns an int.

    This is used for SUM arguments where IF(cond, 1, 0) should return int, not string.
    """
    if isinstance(expr, LiteralInt):
        return str(expr.value)

    if isinstance(expr, FuncCall) and expr.name == 'IF':
        # IF(cond, then, else) -> func() int { if cond { return then }; return else }()
        if len(expr.args) < 2:
            raise ValueError("IF requires at least 2 arguments")
        cond = compile_to_go(expr.args[0], struct_name, field_types)
        then_val = _compile_to_go_int(expr.args[1], struct_name, field_types)
        else_val = _compile_to_go_int(expr.args[2], struct_name, field_types) if len(expr.args) > 2 else '0'
        # Wrap condition in boolVal if it's a field ref
        if isinstance(expr.args[0], FieldRef):
            cond = f'boolVal({cond})'
        return f'func() int {{ if {cond} {{ return {then_val} }}; return {else_val} }}()'

    # For other expression shapes, try the regular compiler.
    return compile_to_go(expr, struct_name, field_types)


def compile_to_go(expr: ExprNode, struct_name: str = 'lc', field_types: dict = None) -> str:
    """Compile an expression tree to a Go expression.

    Uses boolVal() helper for nil-safe boolean access.
    Field references use PascalCase struct field names.

    Args:
        expr: The expression node to compile
        struct_name: Variable name for the struct (e.g., 'tc' for tc.FieldName)
        field_types: Optional dict mapping field names to their datatypes
                     (e.g., {'OrderNumber': 'integer', 'Name': 'string'})
    """
    if field_types is None:
        field_types = {}

    if isinstance(expr, LiteralBool):
        return 'true' if expr.value else 'false'

    if isinstance(expr, LiteralInt):
        return str(expr.value)

    if isinstance(expr, LiteralString):
        escaped = expr.value.replace('\\', '\\\\').replace('"', '\\"')
        return f'"{escaped}"'

    if isinstance(expr, FieldRef):
        # Go struct fields are PascalCase
        field_name = expr.name  # Already PascalCase in rulebook
        return f'{struct_name}.{field_name}'

    if isinstance(expr, UnaryOp):
        if expr.op == 'NOT':
            operand = compile_to_go(expr.operand, struct_name, field_types)
            # Wrap in boolVal for nil-safe access
            if isinstance(expr.operand, FieldRef):
                return f'!boolVal({operand})'
            return f'!({operand})'
        raise ValueError(f"Unknown unary op: {expr.op}")

    if isinstance(expr, BinaryOp):
        if expr.op in ('+', '-', '*', '/'):
            # Arithmetic operands are numeric, so the nil-safe wrappers the
            # comparison paths below apply (boolVal/stringVal) are wrong here.
            left = _compile_to_go_int(expr.left, struct_name, field_types)
            right = _compile_to_go_int(expr.right, struct_name, field_types)
            return f'({left} {expr.op} {right})'

        # Handle comparisons involving field refs (pointer fields in Go)
        if isinstance(expr.left, FieldRef) and isinstance(expr.right, FieldRef):
            # The nil-safe accessor depends on the fields' declared datatype,
            # not on the shape of the expression: boolVal() on an *int does
            # not compile, and `>` is not defined on bool.
            left = compile_to_go(expr.left, struct_name, field_types)
            right = compile_to_go(expr.right, struct_name, field_types)
            op_map = {'=': '==', '<>': '!=', '<': '<', '<=': '<=', '>': '>', '>=': '>='}
            accessor = _go_accessor_for(expr.left, expr.right, field_types)
            return f'({accessor}({left}) {op_map[expr.op]} {accessor}({right}))'

        if isinstance(expr.left, FieldRef) and isinstance(expr.right, LiteralInt):
            # Field ref compared to integer - need nil check and dereference
            left_field = expr.left.name
            right = compile_to_go(expr.right, struct_name, field_types)
            op_go = {'=': '==', '<>': '!=', '<': '<', '<=': '<=', '>': '>', '>=': '>='}[expr.op]
            if expr.op == '=':
                return f'({struct_name}.{left_field} != nil && *{struct_name}.{left_field} == {right})'
            elif expr.op == '<>':
                return f'({struct_name}.{left_field} == nil || *{struct_name}.{left_field} != {right})'
            else:
                # For <, <=, >, >= - nil is treated as false (0 comparison semantics)
                return f'({struct_name}.{left_field} != nil && *{struct_name}.{left_field} {op_go} {right})'

        if isinstance(expr.left, FieldRef) and isinstance(expr.right, LiteralBool):
            # Field ref compared to boolean literal - use boolVal for nil-safe access
            left = compile_to_go(expr.left, struct_name, field_types)
            right = compile_to_go(expr.right, struct_name, field_types)
            op_map = {'=': '==', '<>': '!='}
            return f'(boolVal({left}) {op_map[expr.op]} {right})'

        if isinstance(expr.left, FieldRef) and isinstance(expr.right, LiteralString):
            # Field ref compared to string literal - use stringVal for nil-safe access
            left = compile_to_go(expr.left, struct_name, field_types)
            right = compile_to_go(expr.right, struct_name, field_types)
            op_map = {'=': '==', '<>': '!='}
            return f'(stringVal({left}) {op_map[expr.op]} {right})'

        if isinstance(expr.left, LiteralString) and isinstance(expr.right, FieldRef):
            # String literal compared to field ref - use stringVal for nil-safe access
            left = compile_to_go(expr.left, struct_name, field_types)
            right = compile_to_go(expr.right, struct_name, field_types)
            op_map = {'=': '==', '<>': '!='}
            return f'({left} {op_map[expr.op]} stringVal({right}))'

        left = compile_to_go(expr.left, struct_name, field_types)
        right = compile_to_go(expr.right, struct_name, field_types)
        op_map = {'=': '==', '<>': '!=', '<': '<', '<=': '<=', '>': '>', '>=': '>='}
        return f'({left} {op_map[expr.op]} {right})'

    if isinstance(expr, FuncCall):
        if expr.name == 'AND':
            parts = []
            for arg in expr.args:
                compiled = compile_to_go(arg, struct_name, field_types)
                if isinstance(arg, FieldRef):
                    parts.append(f'boolVal({compiled})')
                elif isinstance(arg, UnaryOp) and arg.op == 'NOT':
                    # NOT already handles boolVal
                    parts.append(compiled)
                elif isinstance(arg, BinaryOp):
                    # Binary ops handle their own nil checks
                    parts.append(compiled)
                else:
                    parts.append(compiled)
            return '(' + ' && '.join(parts) + ')'

        if expr.name == 'OR':
            parts = []
            for arg in expr.args:
                compiled = compile_to_go(arg, struct_name, field_types)
                if isinstance(arg, FieldRef):
                    parts.append(f'boolVal({compiled})')
                else:
                    parts.append(compiled)
            return '(' + ' || '.join(parts) + ')'

        if expr.name == 'IF':
            if len(expr.args) < 2:
                raise ValueError("IF requires at least 2 arguments")
            cond = compile_to_go(expr.args[0], struct_name, field_types)
            then_val = compile_to_go(expr.args[1], struct_name, field_types)
            else_val = compile_to_go(expr.args[2], struct_name, field_types) if len(expr.args) > 2 else '""'
            # Go doesn't have ternary - generate inline func
            return f'func() string {{ if {cond} {{ return {then_val} }}; return {else_val} }}()'

        if expr.name == 'NOT':
            if len(expr.args) != 1:
                raise ValueError("NOT requires 1 argument")
            operand = compile_to_go(expr.args[0], struct_name, field_types)
            if isinstance(expr.args[0], FieldRef):
                return f'!boolVal({operand})'
            return f'!({operand})'

        if expr.name == 'LOWER':
            if len(expr.args) != 1:
                raise ValueError("LOWER requires 1 argument")
            arg = compile_to_go(expr.args[0], struct_name, field_types)
            return f'strings.ToLower(stringVal({arg}))'

        if expr.name == 'FIND':
            if len(expr.args) != 2:
                raise ValueError("FIND requires 2 arguments")
            needle = compile_to_go(expr.args[0], struct_name, field_types)
            haystack = compile_to_go(expr.args[1], struct_name, field_types)
            return f'strings.Contains(stringVal({haystack}), {needle})'

        if expr.name == 'CAST':
            if len(expr.args) >= 1:
                arg = compile_to_go(expr.args[0], struct_name, field_types)
                if isinstance(expr.args[0], FieldRef):
                    # Check field type for appropriate conversion
                    field_type = field_types.get(expr.args[0].name, 'boolean').lower()
                    if field_type == 'integer':
                        return f'intToString({arg})'
                    elif field_type == 'boolean':
                        return f'boolToString(boolVal({arg}))'
                    else:
                        return f'stringVal({arg})'
                return f'fmt.Sprintf("%v", {arg})'
            raise ValueError("CAST requires at least 1 argument")

        if expr.name == 'SUM':
            # SUM(a, b, c) -> (a + b + c)
            # Typically used as SUM(IF(cond,1,0), IF(cond,1,0), ...)
            # For Go, we need IF to return int, not string
            if not expr.args:
                return '0'
            parts = []
            for arg in expr.args:
                parts.append(_compile_to_go_int(arg, struct_name, field_types))
            return '(' + ' + '.join(parts) + ')'

        if expr.name == 'SUBSTITUTE':
            # SUBSTITUTE(text, old_text, new_text) -> strings.ReplaceAll(text, old_text, new_text)
            if len(expr.args) != 3:
                raise ValueError("SUBSTITUTE requires 3 arguments")
            text = compile_to_go(expr.args[0], struct_name, field_types)
            old_text = compile_to_go(expr.args[1], struct_name, field_types)
            new_text = compile_to_go(expr.args[2], struct_name, field_types)
            # Wrap field refs in stringVal for nil-safe access
            if isinstance(expr.args[0], FieldRef):
                text = f'stringVal({text})'
            return f'strings.ReplaceAll({text}, {old_text}, {new_text})'

        if expr.name == 'BLANK':
            # BLANK() -> "" (empty string in Go context)
            return '""'

        if expr.name == 'ISBLANK':
            # ISBLANK(x) -> the value is absent or empty.
            # Nullable scalars are pointers here, so a blank check must cover
            # both the nil pointer and the empty string it points at — the
            # same pair the Postgres transpiler emits as IS NULL OR = ''.
            if len(expr.args) != 1:
                raise ValueError("ISBLANK requires 1 argument")
            if isinstance(expr.args[0], FieldRef):
                ref = compile_to_go(expr.args[0], struct_name, field_types)
                return f'({ref} == nil || stringVal({ref}) == "")'
            arg = compile_to_go(expr.args[0], struct_name, field_types)
            return f'({arg} == "")'

        raise ValueError(f"Unknown function: {expr.name}")

    if isinstance(expr, Concat):
        parts = []
        for part in expr.parts:
            if isinstance(part, LiteralString):
                escaped = part.value.replace('\\', '\\\\').replace('"', '\\"')
                parts.append(f'"{escaped}"')
            else:
                var = compile_to_go(part, struct_name, field_types)
                if isinstance(part, FieldRef):
                    # Check field type to use appropriate conversion
                    field_type = field_types.get(part.name, 'string').lower()
                    if field_type == 'integer':
                        parts.append(f'intToString({var})')
                    elif field_type == 'boolean':
                        parts.append(f'boolToString(boolVal({var}))')
                    else:
                        parts.append(f'stringVal({var})')
                else:
                    parts.append(var)
        if len(parts) == 1:
            return parts[0]
        return ' + '.join(parts)

    raise ValueError(f"Unknown expression node type: {type(expr)}")


# =============================================================================
# CANONICAL EVALUATOR
# =============================================================================
# Direct formula evaluation for generating answer keys from the rulebook.
# This evaluator is the source of truth - all substrates must match its output.

def _is_empty_string_literal(node) -> bool:
    """True when the node is the dialect's blank marker: the literal "", or a
    bare BLANK() call — `{{X}} = BLANK()` and `{{X}} = ""` are the same test.
    Both compile to the SAME null-safe check (an absent relationship/lookup is
    stored as '' rather than NULL, so plain `x == None` would silently miss
    it — see every `=`/`<>` call site of this helper).
    """
    if isinstance(node, LiteralString) and node.value == '':
        return True
    if isinstance(node, FuncCall) and node.name == 'BLANK' and not node.args:
        return True
    return False


def erb_blank(value) -> bool:
    """Blank means absent: NULL or the empty string, matching Postgres."""
    return value is None or value == ''


def erb_date_or_none(value):
    """Parse a date/datetime-like value into a naive datetime, or None if it
    isn't one. Deliberately narrow (must START with YYYY-MM-DD) so a plain
    numeric string like "5" is never misread as a date — only used to decide
    whether `+`/`-` means date arithmetic (see erb_add/erb_sub).
    """
    if isinstance(value, str):
        s = value.strip()
        if re.match(r'^\d{4}-\d{2}-\d{2}', s):
            try:
                return datetime.datetime.fromisoformat(s.replace('Z', '+00:00')).replace(tzinfo=None)
            except ValueError:
                try:
                    return datetime.datetime.strptime(s[:10], '%Y-%m-%d')
                except ValueError:
                    return None
    return None


def _erb_days_delta(value):
    try:
        return datetime.timedelta(days=float(value or 0))
    except (TypeError, ValueError):
        return datetime.timedelta(0)


def erb_mul(a, b):
    """BinaryOp '*': numeric multiplication with the same encoding tolerance as
    the interpreter. Without coercion `'12000' * 2` is Python string repetition
    — '1200012000' — which is silently wrong rather than an error.
    """
    return _num_or_zero(a) * _num_or_zero(b)


def erb_div(a, b):
    """BinaryOp '/': numeric division. Division by zero returns None (Excel's
    #DIV/0! treated as blank), matching both the interpreter and the Postgres
    oracle's NULLIF(divisor, 0) guard, instead of raising ZeroDivisionError and
    taking the whole field evaluation down.
    """
    divisor = _num_or_zero(b)
    if divisor == 0:
        return None
    return _num_or_zero(a) / divisor


def erb_add(a, b):
    """BinaryOp '+': numeric addition, EXCEPT date-plus-days — the dialect
    spells `EventDate + GraceDays` with plain `+`, matching SQL's
    `date + integer` day-arithmetic, rather than a dedicated DATEADD call.
    Falls back to plain None-safe numeric addition when neither side is a
    recognizable date (erb_date_or_none), so ordinary arithmetic is unchanged.
    """
    a_dt, b_dt = erb_date_or_none(a), erb_date_or_none(b)
    if a_dt is not None and b_dt is None:
        return a_dt + _erb_days_delta(b)
    if b_dt is not None and a_dt is None:
        return b_dt + _erb_days_delta(a)
    return _num_or_zero(a) + _num_or_zero(b)


def erb_sub(a, b):
    """BinaryOp '-': numeric subtraction, EXCEPT date-minus-days (see
    erb_add) — e.g. RegistrationDeadline = EventDate - DaysBeforeEvent.
    """
    a_dt = erb_date_or_none(a)
    if a_dt is not None and erb_date_or_none(b) is None:
        return a_dt - _erb_days_delta(b)
    return _num_or_zero(a) - _num_or_zero(b)


def erb_right(text, count) -> str:
    """RIGHT(text, count): last `count` characters, blank-safe both ways."""
    s = text or ''
    n = count or 0
    if n <= 0:
        return ''
    return s[-n:]


def erb_mid(text, start, count) -> str:
    """MID(text, start, count): `count` characters from 1-based `start`
    (the dialect's string functions are 1-indexed, like Excel's), blank-safe.
    """
    s = text or ''
    n = count or 0
    start_idx = max(int(start or 1) - 1, 0)
    if n <= 0:
        return ''
    return s[start_idx:start_idx + n]


def erb_coalesce(*values):
    """COALESCE(a, b, ...): first argument that isn't NULL/blank."""
    for v in values:
        if not erb_blank(v):
            return v
    return None


def erb_try(fn, fallback_fn):
    """IFERROR(expr, fallback): evaluate expr; if it raises, evaluate fallback
    instead. Mirrors the spreadsheet dialect's error-trapping — e.g. #DIV/0!
    from a division compiled with a zero/blank divisor (compile_to_python's
    arithmetic BinaryOp intentionally lets that raise ZeroDivisionError rather
    than silently returning 0 or None, so IFERROR is how a formula opts into
    catching it).
    """
    try:
        return fn()
    except Exception:
        return fallback_fn()


def erb_is_error(fn) -> bool:
    """ISERROR(expr): True if evaluating expr raises, matching IFERROR's
    notion of what counts as an error.
    """
    try:
        fn()
        return False
    except Exception:
        return True


def erb_now():
    """NOW()/TODAY() anchored so answer keys are reproducible.

    FORMULA_NOW (ISO-8601) pins the clock; without it this is realtime.
    """
    import datetime, os
    override = os.environ.get('FORMULA_NOW')
    if override:
        return datetime.datetime.fromisoformat(override)
    return datetime.datetime.utcnow()


def erb_round(value, digits=0):
    """ROUND(value, digits): round-half-AWAY-FROM-ZERO, matching standard SQL
    ROUND() (and the Postgres oracle) rather than Python's builtin round(),
    which is round-half-to-EVEN ("banker's rounding") — ROUND(2.5, 0) is 3 in
    SQL/here, but round(2.5) is 2 in plain Python. NULL propagates: a blank
    input has nothing to round, so the result is NULL, not 0.

    Shared by the interpreter and by compiled Python, so there is exactly one
    implementation of the semantics.
    """
    if value is None or value == '':
        return None
    from decimal import Decimal, ROUND_HALF_UP
    d = int(digits) if digits is not None and digits != '' else 0
    quant = Decimal(1).scaleb(-d)
    return float(Decimal(str(value)).quantize(quant, rounding=ROUND_HALF_UP))


def erb_datetime_diff(end, start, unit='day'):
    """Whole elapsed `unit`s between two datetimes, positive when end > start.

    Month and year counting follows Postgres AGE() semantics — whole elapsed
    calendar months, not days//30 — so a substrate calling this agrees with
    the Postgres oracle on month boundaries instead of drifting.

    Shared by the interpreter and by compiled Python, so there is exactly one
    implementation of the semantics.
    """
    import datetime
    unit = str(unit or 'day').lower().rstrip('s')
    if end is None or start is None:
        return None

    def _coerce(d):
        if isinstance(d, (datetime.datetime, datetime.date)):
            return d if isinstance(d, datetime.datetime) else \
                datetime.datetime.combine(d, datetime.time())
        if isinstance(d, str):
            try:
                return datetime.datetime.fromisoformat(d.replace('Z', '+00:00'))
            except ValueError:
                return datetime.datetime.strptime(d, '%Y-%m-%d')
        raise TypeError(f"DATETIME_DIFF: cannot coerce {d!r} to datetime")

    end_dt = _coerce(end)
    start_dt = _coerce(start)
    # Strip tzinfo to allow naive/aware mixing without raising.
    if end_dt.tzinfo is not None:
        end_dt = end_dt.replace(tzinfo=None)
    if start_dt.tzinfo is not None:
        start_dt = start_dt.replace(tzinfo=None)

    delta = end_dt - start_dt
    # The oracle's units: days is `end::date - start::date` (calendar days, not
    # elapsed 24-hour periods); hours, minutes and seconds are the epoch
    # difference divided out, unrounded — a field declared integer rounds it.
    if unit == 'day':
        return (end_dt.date() - start_dt.date()).days
    if unit == 'hour':
        return delta.total_seconds() / 3600
    if unit == 'minute':
        return delta.total_seconds() / 60
    if unit == 'second':
        return delta.total_seconds()
    if unit == 'week':
        return delta.days // 7
    if unit in ('month', 'year'):
        months = (end_dt.year - start_dt.year) * 12 + (end_dt.month - start_dt.month)
        # AGE() does not count the final partial month.
        if (end_dt.day, end_dt.hour, end_dt.minute, end_dt.second) \
                < (start_dt.day, start_dt.hour, start_dt.minute, start_dt.second):
            months -= 1
        return months if unit == 'month' else months // 12
    raise ValueError(f"DATETIME_DIFF: unsupported unit {unit!r}")


def evaluate(formula: str, context: dict) -> any:
    """
    Evaluate a formula directly given field values.

    This is the canonical evaluator - the single source of truth for formula
    semantics. Answer keys are generated using this evaluator, and all
    execution substrates (Python, Go, Postgres, etc.) must produce identical
    results to pass conformance testing.

    Args:
        formula: An Excel-dialect formula string (e.g., "=IF({{Name}}=\"\", \"Unknown\", {{Name}})")
        context: A dict mapping field names (PascalCase) to their values

    Returns:
        The evaluated result (bool, int, str, or None)

    Example:
        >>> evaluate('={{FirstName}} & " " & {{LastName}}', {'FirstName': 'John', 'LastName': 'Doe'})
        'John Doe'
    """
    expr = parse_formula(formula)
    return _eval_expr(expr, context)


def _to_number(value):
    """
    Coerce one value to a number for arithmetic/comparison, or return None when
    it cannot be one.

    The rulebook declares a field's datatype; it does not promise the ENCODING
    the value arrives in. A number may reach here as int, float, Decimal, or as
    a string holding a number — the last is what any writer that round-trips
    through a driver returning NUMERIC as text produces. Treating that string as
    text is how `{{Price}} * 2` silently became string repetition instead of
    multiplication.

    Booleans are deliberately NOT numbers here: `True * 2` meaning 2 is a Python
    accident, not a rulebook rule.
    """
    if value is None or isinstance(value, bool):
        return None
    if isinstance(value, (int, float)):
        return value
    if isinstance(value, str):
        stripped = value.strip()
        if not stripped:
            return None
        try:
            return int(stripped)
        except ValueError:
            pass
        try:
            return float(stripped)
        except ValueError:
            return None
    return None


def _num_or_zero(value):
    """
    Arithmetic operand coercion. Blank is 0 (Excel's rule), a numeric string is
    its number, and anything that is not a number at all is 0 — which is what
    the Postgres oracle computes for the same formula, since its safe numeric
    cast yields NULL for unparsable text and COALESCE(..., 0) turns that into 0.
    Matching the oracle matters more here than raising: a conformance run
    comparing substrates must not disagree because one of them crashed.
    """
    number = _to_number(value)
    return 0 if number is None else number


def _compare_operands(left, right):
    """
    Prepare two operands for an ordered comparison (< <= > >=).

    Returns a (left, right) pair of the same comparable kind, or None when the
    two cannot be ordered at all. Numbers and numeric strings compare as
    numbers; two strings compare as strings; a number against a non-numeric
    string is not ordered (the caller reports False rather than raising, which
    is what it already did for None).
    """
    if left is None or right is None:
        return None

    left_number, right_number = _to_number(left), _to_number(right)
    if left_number is not None and right_number is not None:
        return left_number, right_number

    if isinstance(left, str) and isinstance(right, str):
        return left, right

    if isinstance(left, bool) and isinstance(right, bool):
        return left, right

    return None


def _values_equal(left, right):
    """
    Equality with the same encoding-tolerance as the ordered comparisons: 12000
    equals "12000", because one of those is a declared number that travelled as
    text. Two strings still compare as strings, so `{{Status}} = "open"` is
    unaffected.
    """
    if isinstance(left, str) and isinstance(right, str):
        return left == right
    if isinstance(left, bool) or isinstance(right, bool):
        return left == right

    left_number, right_number = _to_number(left), _to_number(right)
    if left_number is not None and right_number is not None:
        return left_number == right_number

    return left == right


def _eval_expr(node: ExprNode, ctx: dict) -> any:
    """
    Recursively evaluate an expression node given a context dict.

    Implements Excel-style semantics:
    - None/null values propagate sensibly
    - Empty string vs None distinction preserved
    - Boolean coercion follows Excel conventions
    """
    if isinstance(node, LiteralBool):
        return node.value

    if isinstance(node, LiteralInt):
        return node.value

    if isinstance(node, LiteralFloat):
        return node.value

    if isinstance(node, LiteralString):
        return node.value

    if isinstance(node, FieldRef):
        # Field names in formulas are PascalCase
        return ctx.get(node.name)

    if isinstance(node, UnaryOp):
        if node.op == 'NOT':
            operand = _eval_expr(node.operand, ctx)
            # None is treated as False for NOT
            if operand is None:
                return True
            return not bool(operand)
        if node.op == '-':
            operand = _eval_expr(node.operand, ctx)
            if operand is None:
                return 0
            return -operand
        raise ValueError(f"Unknown unary op: {node.op}")

    if isinstance(node, BinaryOp):
        left = _eval_expr(node.left, ctx)
        right = _eval_expr(node.right, ctx)

        # Comparing against "" is the dialect's null-safe blank check.
        if node.op in ('=', '<>'):
            if _is_empty_string_literal(node.right):
                blank = erb_blank(left)
                return blank if node.op == '=' else not blank
            if _is_empty_string_literal(node.left):
                blank = erb_blank(right)
                return blank if node.op == '=' else not blank
        if node.op == '=':
            return _values_equal(left, right)
        if node.op == '<>':
            return not _values_equal(left, right)
        if node.op in ('<', '<=', '>', '>='):
            # Encoding-tolerant ordering. A declared number that arrived as a
            # string must still compare as a number, and two values that cannot
            # be ordered at all report False rather than raising a TypeError
            # that would take the whole evaluation down.
            pair = _compare_operands(left, right)
            if pair is None:
                return False
            l, r = pair
            if node.op == '<':
                return l < r
            if node.op == '<=':
                return l <= r
            if node.op == '>':
                return l > r
            return l >= r
        # Arithmetic — Excel treats None/blank as 0 in arithmetic context, and a
        # declared number that arrived as a string is still a number (see
        # _num_or_zero). Without that coercion `{{Price}} * 2` on "12000"
        # returned '1200012000' — string repetition, silently, with no error.
        if node.op in ('+', '-', '*', '/'):
            l = _num_or_zero(left)
            r = _num_or_zero(right)
            if node.op == '+':
                return l + r
            if node.op == '-':
                return l - r
            if node.op == '*':
                return l * r
            # '/' — return None on divide-by-zero rather than raising, to
            # match Excel's #DIV/0! semantics (callers can treat as blank).
            if r == 0:
                return None
            return l / r
        raise ValueError(f"Unknown binary op: {node.op}")

    if isinstance(node, FuncCall):
        return _eval_func(node, ctx)

    if isinstance(node, Concat):
        parts = []
        for part in node.parts:
            val = _eval_expr(part, ctx)
            # Convert None to empty string for concatenation
            parts.append(str(val) if val is not None else '')
        return ''.join(parts)

    raise ValueError(f"Unknown expression node type: {type(node)}")


def _eval_func(node: FuncCall, ctx: dict) -> any:
    """Evaluate a function call expression node."""
    name = node.name
    args = node.args

    if name == 'AND':
        for arg in args:
            val = _eval_expr(arg, ctx)
            # None or False fails AND
            if val is None or val is False or val == 0:
                return False
        return True

    if name == 'OR':
        for arg in args:
            val = _eval_expr(arg, ctx)
            # Any truthy value passes OR
            if val is True or (val is not None and val != False and val != 0):
                return True
        return False

    if name == 'NOT':
        if len(args) != 1:
            raise ValueError("NOT requires 1 argument")
        val = _eval_expr(args[0], ctx)
        if val is None:
            return True
        return not bool(val)

    if name == 'IF':
        if len(args) < 2:
            raise ValueError("IF requires at least 2 arguments")
        cond = _eval_expr(args[0], ctx)
        # Condition is truthy if not None/False/0/""
        is_true = cond is not None and cond is not False and cond != 0 and cond != ""
        if is_true:
            return _eval_expr(args[1], ctx)
        elif len(args) > 2:
            return _eval_expr(args[2], ctx)
        else:
            return None

    if name == 'LOWER':
        if len(args) != 1:
            raise ValueError("LOWER requires 1 argument")
        val = _eval_expr(args[0], ctx)
        if val is None:
            return ''
        return str(val).lower()

    if name == 'UPPER':
        if len(args) != 1:
            raise ValueError("UPPER requires 1 argument")
        val = _eval_expr(args[0], ctx)
        if val is None:
            return ''
        return str(val).upper()

    if name == 'FIND':
        if len(args) != 2:
            raise ValueError("FIND requires 2 arguments")
        needle = _eval_expr(args[0], ctx)
        haystack = _eval_expr(args[1], ctx)
        if needle is None or haystack is None:
            return False
        return str(needle) in str(haystack)

    if name == 'CAST':
        if len(args) < 1:
            raise ValueError("CAST requires at least 1 argument")
        val = _eval_expr(args[0], ctx)
        if val is None:
            return ''
        return str(val)

    if name == 'SUM':
        total = 0
        for arg in args:
            val = _eval_expr(arg, ctx)
            if val is not None and isinstance(val, (int, float)):
                total += val
        return total

    if name == 'SUBSTITUTE':
        if len(args) != 3:
            raise ValueError("SUBSTITUTE requires 3 arguments")
        text = _eval_expr(args[0], ctx)
        old_text = _eval_expr(args[1], ctx)
        new_text = _eval_expr(args[2], ctx)
        if text is None:
            return ''
        return str(text).replace(str(old_text or ''), str(new_text or ''))

    if name == 'LEFT':
        if len(args) != 2:
            raise ValueError("LEFT requires 2 arguments")
        text = _eval_expr(args[0], ctx)
        num = _eval_expr(args[1], ctx)
        if text is None:
            return ''
        return str(text)[:int(num)]

    if name == 'RIGHT':
        if len(args) != 2:
            raise ValueError("RIGHT requires 2 arguments")
        text = _eval_expr(args[0], ctx)
        num = _eval_expr(args[1], ctx)
        if text is None:
            return ''
        n = int(num)
        return str(text)[-n:] if n > 0 else ''

    if name == 'LEN':
        if len(args) != 1:
            raise ValueError("LEN requires 1 argument")
        val = _eval_expr(args[0], ctx)
        if val is None:
            return 0
        return len(str(val))

    if name == 'CONCAT':
        parts = []
        for arg in args:
            val = _eval_expr(arg, ctx)
            parts.append(str(val) if val is not None else '')
        return ''.join(parts)

    if name == 'COUNT':
        # COUNT of non-null values
        count = 0
        for arg in args:
            val = _eval_expr(arg, ctx)
            if val is not None:
                count += 1
        return count

    if name == 'AVERAGE':
        total = 0
        count = 0
        for arg in args:
            val = _eval_expr(arg, ctx)
            if val is not None and isinstance(val, (int, float)):
                total += val
                count += 1
        return total / count if count > 0 else None

    if name == 'MIN':
        values = []
        for arg in args:
            val = _eval_expr(arg, ctx)
            if val is not None and isinstance(val, (int, float)):
                values.append(val)
        return min(values) if values else None

    if name == 'MAX':
        values = []
        for arg in args:
            val = _eval_expr(arg, ctx)
            if val is not None and isinstance(val, (int, float)):
                values.append(val)
        return max(values) if values else None

    if name == 'BLANK':
        # BLANK() -> None (empty/null value)
        return None

    if name == 'ISBLANK':
        if len(args) != 1:
            raise ValueError("ISBLANK requires 1 argument")
        val = _eval_expr(args[0], ctx)
        return val is None or val == ''

    if name == 'NOW' or name == 'TODAY':
        return erb_now()

    if name in ('DATETIME_DIFF', 'DATEDIFF'):
        # DATETIME_DIFF(end, start, unit) -> integer difference in unit.
        # Excel/Airtable convention: positive when end > start.
        if len(args) < 2:
            raise ValueError(f"{name} requires at least 2 arguments")
        end = _eval_expr(args[0], ctx)
        start = _eval_expr(args[1], ctx)
        unit = (_eval_expr(args[2], ctx) if len(args) > 2 else 'day') or 'day'
        return erb_datetime_diff(end, start, unit)

    raise ValueError(f"Unknown function: {name}")


def evaluate_field(formula: str, record: dict, field_name_mapping: dict = None) -> any:
    """
    Convenience function to evaluate a formula against a record.

    Args:
        formula: The formula string
        record: A dict with snake_case field names (as stored in JSON)
        field_name_mapping: Optional dict mapping snake_case -> PascalCase
                           If None, attempts to convert automatically

    Returns:
        The evaluated result
    """
    # Build context with PascalCase keys (as used in formulas)
    context = {}
    for key, value in record.items():
        # Convert snake_case to PascalCase for the context
        pascal_key = to_pascal_case(key)
        context[pascal_key] = value
        # Also keep original key in case formula uses it directly
        context[key] = value

    return evaluate(formula, context)


# =============================================================================
# COBOL Code Generation
# =============================================================================

def to_cobol_name(name: str) -> str:
    """Convert a field name to COBOL format (uppercase, hyphens)."""
    # Convert camelCase/PascalCase to hyphen-separated uppercase
    import re
    # Insert hyphen before uppercase letters (except at start)
    s1 = re.sub('(.)([A-Z][a-z]+)', r'\1-\2', name)
    s2 = re.sub('([a-z0-9])([A-Z])', r'\1-\2', s1)
    # Replace underscores with hyphens and uppercase
    return s2.replace('_', '-').upper()


def compile_to_cobol(expr: ExprNode, prefix: str = "RECORD") -> str:
    """
    Compile an expression tree to COBOL expression.
    Returns COBOL code that can be used in MOVE or condition statements.
    """
    if isinstance(expr, LiteralString):
        # COBOL string literal (escape quotes by doubling)
        escaped = expr.value.replace('"', '""')
        return f'"{escaped}"'

    if isinstance(expr, LiteralBool):
        return '"true"' if expr.value else '"false"'

    if isinstance(expr, LiteralInt):
        return str(expr.value)

    if isinstance(expr, Concat):
        # Handle Concat expression type directly
        parts = [compile_to_cobol(p, prefix) for p in expr.parts]
        return ('CONCAT', *parts)

    if isinstance(expr, FieldRef):
        cobol_name = to_cobol_name(expr.name)
        return f'{prefix}-{cobol_name}'

    if isinstance(expr, UnaryOp):
        if expr.op == 'NOT':
            operand = compile_to_cobol(expr.operand, prefix)
            # NOT in COBOL context - handled in IF statement
            # If operand is already a comparison, don't add = "true"
            if isinstance(operand, str) and (' = ' in operand or ' < ' in operand or ' > ' in operand or ' NOT ' in operand):
                return f'NOT {operand}'
            return f'NOT ({operand} = "true")'
        raise ValueError(f"Unknown unary operator: {expr.op}")

    if isinstance(expr, BinaryOp):
        left = compile_to_cobol(expr.left, prefix)
        right = compile_to_cobol(expr.right, prefix)
        op_map = {
            '=': '=', '<>': 'NOT =', '<': '<', '<=': '<=', '>': '>', '>=': '>=',
            '+': '+', '-': '-', '*': '*', '/': '/'
        }
        if expr.op == '&':
            # String concatenation - return as tuple for special handling
            return ('CONCAT', left, right)
        return f'({left} {op_map.get(expr.op, expr.op)} {right})'

    if isinstance(expr, FuncCall):
        if expr.name == 'AND':
            conditions = [compile_to_cobol(arg, prefix) for arg in expr.args]
            # Wrap each condition properly for COBOL
            wrapped = []
            for cond in conditions:
                if isinstance(cond, tuple) and cond[0] == 'CONDITION':
                    wrapped.append(cond[1])
                elif ' = ' in str(cond) or ' NOT ' in str(cond):
                    wrapped.append(f'({cond})')
                else:
                    wrapped.append(f'({cond} = "true")')
            return '(' + ' AND '.join(wrapped) + ')'

        if expr.name == 'OR':
            conditions = [compile_to_cobol(arg, prefix) for arg in expr.args]
            wrapped = []
            for cond in conditions:
                if isinstance(cond, tuple) and cond[0] == 'CONDITION':
                    wrapped.append(cond[1])
                elif ' = ' in str(cond) or ' NOT ' in str(cond):
                    wrapped.append(f'({cond})')
                else:
                    wrapped.append(f'({cond} = "true")')
            return '(' + ' OR '.join(wrapped) + ')'

        if expr.name == 'NOT':
            if len(expr.args) != 1:
                raise ValueError("NOT requires 1 argument")
            operand = compile_to_cobol(expr.args[0], prefix)
            # If operand is already a comparison, don't add = "true"
            if isinstance(operand, str) and (' = ' in operand or ' < ' in operand or ' > ' in operand or ' NOT ' in operand):
                return f'NOT {operand}'
            return f'NOT ({operand} = "true")'

        if expr.name == 'IF':
            # IF in COBOL is a statement, not an expression
            # Return a marker for the caller to handle
            cond = compile_to_cobol(expr.args[0], prefix)
            then_val = compile_to_cobol(expr.args[1], prefix)
            else_val = compile_to_cobol(expr.args[2], prefix) if len(expr.args) > 2 else '"false"'
            return ('IF', cond, then_val, else_val)

        if expr.name == 'CONCAT':
            parts = [compile_to_cobol(arg, prefix) for arg in expr.args]
            return ('CONCAT', *parts)

        if expr.name == 'LOWER':
            arg = compile_to_cobol(expr.args[0], prefix)
            return ('LOWER', arg)

        if expr.name == 'TRIM':
            arg = compile_to_cobol(expr.args[0], prefix)
            return ('TRIM', arg)

        if expr.name == 'SUM':
            parts = [compile_to_cobol(arg, prefix) for arg in expr.args]
            return ('SUM', parts)

        if expr.name == 'FIND':
            needle = compile_to_cobol(expr.args[0], prefix)
            haystack = compile_to_cobol(expr.args[1], prefix)
            return ('FIND', needle, haystack)

        if expr.name == 'CAST':
            return compile_to_cobol(expr.args[0], prefix)

        if expr.name == 'SUBSTITUTE':
            # SUBSTITUTE(text, old_text, new_text)
            text = compile_to_cobol(expr.args[0], prefix)
            old_text = compile_to_cobol(expr.args[1], prefix)
            new_text = compile_to_cobol(expr.args[2], prefix)
            return ('SUBSTITUTE', text, old_text, new_text)

        raise ValueError(f"COBOL: Unknown function: {expr.name}")

    raise ValueError(f"COBOL: Unknown expression type: {type(expr)}")


def cobol_expr_to_statements(expr_result, result_var: str, temp_vars: list) -> list:
    """
    Convert compile_to_cobol result (string or tuple) into a list of COBOL statements.

    Args:
        expr_result: Result from compile_to_cobol (string or tuple)
        result_var: Target variable to store result (e.g., "RECORD-FULL-NAME")
        temp_vars: List of available temp variables (e.g., ["WS-TEMP-1", "WS-TEMP-2", ...])

    Returns:
        List of COBOL statement strings (without leading spaces)
    """
    temp_idx = [0]  # Use list for mutable closure

    def get_temp():
        if temp_idx[0] >= len(temp_vars):
            raise ValueError("Ran out of temp variables")
        var = temp_vars[temp_idx[0]]
        temp_idx[0] += 1
        return var

    def flatten_concat(tup):
        """Flatten nested CONCAT tuples into a list of parts."""
        parts = []
        for item in tup[1:]:
            if isinstance(item, tuple) and item[0] == 'CONCAT':
                parts.extend(flatten_concat(item))
            else:
                parts.append(item)
        return parts

    def is_comparison_expr(s):
        """Check if string contains a COBOL comparison operator."""
        # Look for comparison operators that indicate a boolean expression
        # These cannot be used in MOVE statements
        comparison_ops = [' > ', ' < ', ' >= ', ' <= ', ' NOT = ', ' AND ', ' OR ']
        for op in comparison_ops:
            if op in s:
                return True
        # Check for parenthesized equality comparison: (X = Y)
        # BinaryOp comparisons are always wrapped in parens
        # Match patterns like (WS-REC-HAS-SYNTAX = "true") or (X = Y)
        import re
        # Match: starts with (, has = comparison, ends with )
        if re.match(r'^\(.+\s+=\s+.+\)$', s):
            return True
        return False

    def process(expr, target_var):
        """Process an expression and return statements that store result in target_var."""
        stmts = []

        if isinstance(expr, str):
            # Check if this is a comparison expression (boolean result)
            if is_comparison_expr(expr):
                # Comparisons must use IF/ELSE in COBOL, not MOVE
                stmts.append(f'IF {expr}')
                stmts.append(f'    MOVE "true" TO {target_var}')
                stmts.append('ELSE')
                stmts.append(f'    MOVE "false" TO {target_var}')
                stmts.append('END-IF')
            else:
                # Simple value - just MOVE it
                # Handle empty string literals - COBOL doesn't like ""
                if expr == '""':
                    stmts.append(f'MOVE SPACES TO {target_var}')
                else:
                    stmts.append(f'MOVE {expr} TO {target_var}')

        elif isinstance(expr, tuple):
            op = expr[0]

            if op == 'CONCAT':
                # STRING concatenation - use FUNCTION TRIM for field values
                # Break across multiple lines to avoid 512-byte line limit
                # IMPORTANT: All nested operations must be evaluated BEFORE the STRING statement
                parts = flatten_concat(expr)
                stmts.append(f'MOVE SPACES TO {target_var}')

                # First pass: evaluate all nested operations to temps
                string_parts = []  # (is_literal, value) tuples
                for p in parts:
                    if isinstance(p, tuple):
                        # Nested operation - evaluate to temp first
                        tmp = get_temp()
                        stmts.extend(process(p, tmp))
                        string_parts.append((False, tmp))
                    elif p.startswith('"'):
                        # String literal
                        string_parts.append((True, p))
                    else:
                        # Field reference
                        string_parts.append((False, p))

                # Second pass: build STRING statement
                # Use TRAILING trim to preserve leading spaces in concatenated values
                stmts.append('STRING')
                for is_literal, val in string_parts:
                    if is_literal:
                        stmts.append(f'    {val} DELIMITED SIZE')
                    else:
                        stmts.append(f'    FUNCTION TRIM({val} TRAILING) DELIMITED SIZE')
                stmts.append(f'    INTO {target_var}')

            elif op == 'IF':
                _, cond, then_val, else_val = expr
                # Handle condition which might be a tuple
                if isinstance(cond, tuple):
                    cond_str = format_condition(cond)
                else:
                    cond_str = cond if ' = ' in cond or ' NOT ' in cond else f'{cond} = "true"'

                stmts.append(f'IF {cond_str}')
                then_stmts = process(then_val, target_var)
                for s in then_stmts:
                    stmts.append(f'    {s}')
                stmts.append('ELSE')
                else_stmts = process(else_val, target_var)
                for s in else_stmts:
                    stmts.append(f'    {s}')
                stmts.append('END-IF')

            elif op == 'LOWER':
                _, arg = expr
                if isinstance(arg, tuple):
                    tmp = get_temp()
                    stmts.extend(process(arg, tmp))
                    stmts.append(f'MOVE FUNCTION LOWER-CASE({tmp}) TO {target_var}')
                else:
                    stmts.append(f'MOVE FUNCTION LOWER-CASE({arg}) TO {target_var}')

            elif op == 'TRIM':
                _, arg = expr
                if isinstance(arg, tuple):
                    tmp = get_temp()
                    stmts.extend(process(arg, tmp))
                    stmts.append(f'MOVE FUNCTION TRIM({tmp}) TO {target_var}')
                else:
                    stmts.append(f'MOVE FUNCTION TRIM({arg}) TO {target_var}')

            elif op == 'SUM':
                parts = expr[1]
                # COMPUTE with addition
                compute_expr = ' + '.join(str(p) for p in parts)
                stmts.append(f'COMPUTE {target_var} = {compute_expr}')

            elif op == 'FIND':
                _, needle, haystack = expr
                # Use INSPECT or helper paragraph
                stmts.append(f'MOVE {needle} TO WS-FIND-NEEDLE')
                stmts.append(f'MOVE {haystack} TO WS-FIND-HAYSTACK')
                stmts.append('PERFORM FIND-CONTAINS')
                stmts.append(f'MOVE WS-FIND-RESULT TO {target_var}')

            elif op == 'SUBSTITUTE':
                # SUBSTITUTE(text, old_text, new_text)
                _, text, old_text, new_text = expr
                # Evaluate nested expressions to temps if needed
                if isinstance(text, tuple):
                    text_tmp = get_temp()
                    stmts.extend(process(text, text_tmp))
                    text = text_tmp
                if isinstance(old_text, tuple):
                    old_tmp = get_temp()
                    stmts.extend(process(old_text, old_tmp))
                    old_text = old_tmp
                if isinstance(new_text, tuple):
                    new_tmp = get_temp()
                    stmts.extend(process(new_text, new_tmp))
                    new_text = new_tmp
                # Use helper variables for SUBSTITUTE
                stmts.append(f'MOVE {text} TO WS-SUBST-INPUT')
                stmts.append(f'MOVE {old_text} TO WS-SUBST-OLD')
                stmts.append(f'MOVE {new_text} TO WS-SUBST-NEW')
                stmts.append('PERFORM SUBSTITUTE-ALL')
                stmts.append(f'MOVE WS-SUBST-OUTPUT TO {target_var}')

            else:
                raise ValueError(f"Unknown COBOL tuple operation: {op}")

        else:
            raise ValueError(f"Unknown expression type: {type(expr)}")

        return stmts

    def format_condition(cond):
        """Format a condition for use in IF statement."""
        if isinstance(cond, str):
            if ' = ' in cond or ' NOT ' in cond or ' < ' in cond or ' > ' in cond:
                return cond
            return f'{cond} = "true"'
        elif isinstance(cond, tuple):
            op = cond[0]
            if op == 'CONCAT':
                # Concatenation result used as condition - evaluate first
                return f'{cond} = "true"'  # This shouldn't happen normally
            # For other tuples, format recursively
            return str(cond)
        return str(cond)

    return process(expr_result, result_var)
