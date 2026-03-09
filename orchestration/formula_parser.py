#!/usr/bin/env python3
"""
Shared Formula Parser for ERB Execution Substrates

This module provides a reusable formula parser that converts Excel-dialect
formulas (from effortless-rulebook.json) into an Abstract Syntax Tree (AST).

Each substrate then compiles the AST to its target language:
- Python: compile_to_python()
- JavaScript: compile_to_javascript()
- Go: compile_to_go()
- SPARQL: compile_to_sparql()

Extracted from: execution-substrates/owl/inject-into-owl.py
"""

import re
from dataclasses import dataclass
from typing import List, Any
from enum import Enum, auto


# =============================================================================
# AST NODE TYPES
# =============================================================================

@dataclass
class ASTNode:
    """Base class for AST nodes"""
    pass


@dataclass
class LiteralBool(ASTNode):
    value: bool


@dataclass
class LiteralInt(ASTNode):
    value: int


@dataclass
class LiteralString(ASTNode):
    value: str


@dataclass
class FieldRef(ASTNode):
    name: str  # Field name without {{ }}


@dataclass
class BinaryOp(ASTNode):
    op: str  # '=', '<>', '<', '<=', '>', '>='
    left: ASTNode
    right: ASTNode


@dataclass
class UnaryOp(ASTNode):
    op: str  # 'NOT'
    operand: ASTNode


@dataclass
class FuncCall(ASTNode):
    name: str  # 'AND', 'OR', 'IF', 'LOWER', 'FIND', 'CAST'
    args: List[ASTNode]


@dataclass
class Concat(ASTNode):
    parts: List[ASTNode]


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

        # String literal
        if c == '"':
            j = i + 1
            while j < len(formula) and formula[j] != '"':
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

        # Number
        if c.isdigit() or (c == '-' and i + 1 < len(formula) and formula[i+1].isdigit()):
            j = i
            if c == '-':
                j += 1
            while j < len(formula) and formula[j].isdigit():
                j += 1
            value = int(formula[i:j])
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

    def parse(self) -> ASTNode:
        result = self.parse_concat()
        if self.current().type != TokenType.EOF:
            raise SyntaxError(f"Unexpected token {self.current()} after expression")
        return result

    def parse_concat(self) -> ASTNode:
        left = self.parse_comparison()
        parts = [left]
        while self.current().type == TokenType.AMPERSAND:
            self.consume(TokenType.AMPERSAND)
            right = self.parse_comparison()
            parts.append(right)
        if len(parts) == 1:
            return parts[0]
        return Concat(parts=parts)

    def parse_comparison(self) -> ASTNode:
        left = self.parse_primary()
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
            right = self.parse_primary()
            return BinaryOp(op=op, left=left, right=right)
        return left

    def parse_primary(self) -> ASTNode:
        tok = self.current()

        if tok.type == TokenType.STRING:
            self.consume()
            return LiteralString(value=tok.value)

        if tok.type == TokenType.NUMBER:
            self.consume()
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


def parse_formula(formula_text: str) -> ASTNode:
    """Parse an Excel-dialect formula into an AST."""
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


def get_field_dependencies(ast: ASTNode) -> List[str]:
    """Extract all field references from an AST.

    Returns a list of field names (PascalCase as they appear in formulas).
    Used for DAG ordering and dependency tracking.
    """
    deps = []

    def visit(node: ASTNode):
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

    visit(ast)
    return deps


# =============================================================================
# PYTHON CODE GENERATOR
# =============================================================================

def _is_boolean_expr(ast: ASTNode) -> bool:
    """Check if an AST node produces a boolean result."""
    if isinstance(ast, LiteralBool):
        return True
    if isinstance(ast, UnaryOp) and ast.op == 'NOT':
        return True
    if isinstance(ast, BinaryOp):
        return True  # Comparisons return bool
    if isinstance(ast, FuncCall) and ast.name in ('AND', 'OR', 'NOT'):
        return True
    return False


def _compile_and_arg(ast: ASTNode) -> str:
    """Compile an AND/OR argument with appropriate boolean coercion."""
    compiled = compile_to_python(ast)
    # If it's already a boolean expression, don't wrap with 'is True'
    if _is_boolean_expr(ast):
        return compiled
    # Field references need 'is True' for None handling
    return f'({compiled} is True)'


def compile_to_python(ast: ASTNode) -> str:
    """Compile an AST to a Python expression.

    Handles None values by using 'is True' and 'is not True' patterns.
    Field references are converted to snake_case variable names.
    """
    if isinstance(ast, LiteralBool):
        return 'True' if ast.value else 'False'

    if isinstance(ast, LiteralInt):
        return str(ast.value)

    if isinstance(ast, LiteralString):
        return repr(ast.value)

    if isinstance(ast, FieldRef):
        return to_snake_case(ast.name)

    if isinstance(ast, UnaryOp):
        if ast.op == 'NOT':
            operand = compile_to_python(ast.operand)
            # For field refs, use 'is not True' for None safety
            if isinstance(ast.operand, FieldRef):
                return f'({operand} is not True)'
            # For other expressions, use regular not
            return f'(not {operand})'
        raise ValueError(f"Unknown unary op: {ast.op}")

    if isinstance(ast, BinaryOp):
        left = compile_to_python(ast.left)
        right = compile_to_python(ast.right)
        op_map = {'=': '==', '<>': '!=', '<': '<', '<=': '<=', '>': '>', '>=': '>='}
        return f'({left} {op_map[ast.op]} {right})'

    if isinstance(ast, FuncCall):
        if ast.name == 'AND':
            parts = [_compile_and_arg(arg) for arg in ast.args]
            return '(' + ' and '.join(parts) + ')'

        if ast.name == 'OR':
            parts = [_compile_and_arg(arg) for arg in ast.args]
            return '(' + ' or '.join(parts) + ')'

        if ast.name == 'IF':
            if len(ast.args) < 2:
                raise ValueError("IF requires at least 2 arguments")
            cond = compile_to_python(ast.args[0])
            then_val = compile_to_python(ast.args[1])
            else_val = compile_to_python(ast.args[2]) if len(ast.args) > 2 else 'None'
            return f'({then_val} if {cond} else {else_val})'

        if ast.name == 'NOT':
            if len(ast.args) != 1:
                raise ValueError("NOT requires 1 argument")
            operand = compile_to_python(ast.args[0])
            return f'({operand} is not True)'

        if ast.name == 'LOWER':
            if len(ast.args) != 1:
                raise ValueError("LOWER requires 1 argument")
            arg = compile_to_python(ast.args[0])
            return f'(({arg} or "").lower())'

        if ast.name == 'FIND':
            if len(ast.args) != 2:
                raise ValueError("FIND requires 2 arguments")
            needle = compile_to_python(ast.args[0])
            haystack = compile_to_python(ast.args[1])
            return f'({needle} in ({haystack} or ""))'

        if ast.name == 'CAST':
            # CAST(x AS TEXT) -> str(x) if x else ""
            if len(ast.args) >= 1:
                arg = compile_to_python(ast.args[0])
                return f'(str({arg}) if {arg} else "")'
            raise ValueError("CAST requires at least 1 argument")

        if ast.name == 'SUM':
            # SUM(a, b, c) -> (a + b + c)
            # Typically used as SUM(IF(cond,1,0), IF(cond,1,0), ...)
            if not ast.args:
                return '0'
            parts = [compile_to_python(arg) for arg in ast.args]
            return '(' + ' + '.join(parts) + ')'

        if ast.name == 'SUBSTITUTE':
            # SUBSTITUTE(text, old_text, new_text) -> text.replace(old_text, new_text)
            if len(ast.args) != 3:
                raise ValueError("SUBSTITUTE requires 3 arguments")
            text = compile_to_python(ast.args[0])
            old_text = compile_to_python(ast.args[1])
            new_text = compile_to_python(ast.args[2])
            return f'(({text} or "").replace({old_text}, {new_text}))'

        raise ValueError(f"Unknown function: {ast.name}")

    if isinstance(ast, Concat):
        # Use string concatenation to avoid nested f-string issues
        parts = []
        for part in ast.parts:
            if isinstance(part, LiteralString):
                parts.append(repr(part.value))
            elif isinstance(part, FieldRef):
                var = compile_to_python(part)
                parts.append(f'str({var} or "")')
            else:
                # Complex expression - wrap in str() with None handling
                expr = compile_to_python(part)
                parts.append(f'str({expr} if {expr} is not None else "")')
        return '(' + ' + '.join(parts) + ')'

    raise ValueError(f"Unknown AST node type: {type(ast)}")


# =============================================================================
# JAVASCRIPT CODE GENERATOR
# =============================================================================

def compile_to_javascript(ast: ASTNode, obj_name: str = 'candidate') -> str:
    """Compile an AST to a JavaScript expression.

    Uses explicit === true / !== true for proper null handling.
    Field references use camelCase with object prefix.
    """
    if isinstance(ast, LiteralBool):
        return 'true' if ast.value else 'false'

    if isinstance(ast, LiteralInt):
        return str(ast.value)

    if isinstance(ast, LiteralString):
        escaped = ast.value.replace('\\', '\\\\').replace("'", "\\'")
        return f"'{escaped}'"

    if isinstance(ast, FieldRef):
        return f'{obj_name}.{to_camel_case(ast.name)}'

    if isinstance(ast, UnaryOp):
        if ast.op == 'NOT':
            operand = compile_to_javascript(ast.operand, obj_name)
            return f'({operand} !== true)'
        raise ValueError(f"Unknown unary op: {ast.op}")

    if isinstance(ast, BinaryOp):
        left = compile_to_javascript(ast.left, obj_name)
        right = compile_to_javascript(ast.right, obj_name)
        op_map = {'=': '===', '<>': '!==', '<': '<', '<=': '<=', '>': '>', '>=': '>='}
        return f'({left} {op_map[ast.op]} {right})'

    if isinstance(ast, FuncCall):
        if ast.name == 'AND':
            parts = [f'({compile_to_javascript(arg, obj_name)} === true)' for arg in ast.args]
            return '(' + ' && '.join(parts) + ')'

        if ast.name == 'OR':
            parts = [f'({compile_to_javascript(arg, obj_name)} === true)' for arg in ast.args]
            return '(' + ' || '.join(parts) + ')'

        if ast.name == 'IF':
            if len(ast.args) < 2:
                raise ValueError("IF requires at least 2 arguments")
            cond = compile_to_javascript(ast.args[0], obj_name)
            then_val = compile_to_javascript(ast.args[1], obj_name)
            else_val = compile_to_javascript(ast.args[2], obj_name) if len(ast.args) > 2 else 'null'
            return f'({cond} ? {then_val} : {else_val})'

        if ast.name == 'NOT':
            if len(ast.args) != 1:
                raise ValueError("NOT requires 1 argument")
            operand = compile_to_javascript(ast.args[0], obj_name)
            return f'({operand} !== true)'

        if ast.name == 'LOWER':
            if len(ast.args) != 1:
                raise ValueError("LOWER requires 1 argument")
            arg = compile_to_javascript(ast.args[0], obj_name)
            return f'(({arg} || "").toLowerCase())'

        if ast.name == 'FIND':
            if len(ast.args) != 2:
                raise ValueError("FIND requires 2 arguments")
            needle = compile_to_javascript(ast.args[0], obj_name)
            haystack = compile_to_javascript(ast.args[1], obj_name)
            return f'(({haystack} || "").includes({needle}))'

        if ast.name == 'CAST':
            if len(ast.args) >= 1:
                arg = compile_to_javascript(ast.args[0], obj_name)
                return f'({arg} ? String({arg}) : "")'
            raise ValueError("CAST requires at least 1 argument")

        if ast.name == 'SUM':
            # SUM(a, b, c) -> (a + b + c)
            if not ast.args:
                return '0'
            parts = [compile_to_javascript(arg, obj_name) for arg in ast.args]
            return '(' + ' + '.join(parts) + ')'

        raise ValueError(f"Unknown function: {ast.name}")

    if isinstance(ast, Concat):
        parts = []
        for part in ast.parts:
            if isinstance(part, LiteralString):
                escaped = part.value.replace('\\', '\\\\').replace('`', '\\`').replace('$', '\\$')
                parts.append(escaped)
            else:
                var = compile_to_javascript(part, obj_name)
                parts.append('${' + f'{var} || ""' + '}')
        return '`' + ''.join(parts) + '`'

    raise ValueError(f"Unknown AST node type: {type(ast)}")


# =============================================================================
# GO CODE GENERATOR
# =============================================================================

def _compile_to_go_int(ast: ASTNode, struct_name: str, field_types: dict) -> str:
    """Compile an AST node to a Go expression that returns an int.

    This is used for SUM arguments where IF(cond, 1, 0) should return int, not string.
    """
    if isinstance(ast, LiteralInt):
        return str(ast.value)

    if isinstance(ast, FuncCall) and ast.name == 'IF':
        # IF(cond, then, else) -> func() int { if cond { return then }; return else }()
        if len(ast.args) < 2:
            raise ValueError("IF requires at least 2 arguments")
        cond = compile_to_go(ast.args[0], struct_name, field_types)
        then_val = _compile_to_go_int(ast.args[1], struct_name, field_types)
        else_val = _compile_to_go_int(ast.args[2], struct_name, field_types) if len(ast.args) > 2 else '0'
        # Wrap condition in boolVal if it's a field ref
        if isinstance(ast.args[0], FieldRef):
            cond = f'boolVal({cond})'
        return f'func() int {{ if {cond} {{ return {then_val} }}; return {else_val} }}()'

    # For other expressions, fall back to the regular compiler
    # (shouldn't happen in practice for SUM(IF(...), IF(...)))
    return compile_to_go(ast, struct_name, field_types)


def compile_to_go(ast: ASTNode, struct_name: str = 'lc', field_types: dict = None) -> str:
    """Compile an AST to a Go expression.

    Uses boolVal() helper for nil-safe boolean access.
    Field references use PascalCase struct field names.

    Args:
        ast: The AST node to compile
        struct_name: Variable name for the struct (e.g., 'tc' for tc.FieldName)
        field_types: Optional dict mapping field names to their datatypes
                     (e.g., {'OrderNumber': 'integer', 'Name': 'string'})
    """
    if field_types is None:
        field_types = {}

    if isinstance(ast, LiteralBool):
        return 'true' if ast.value else 'false'

    if isinstance(ast, LiteralInt):
        return str(ast.value)

    if isinstance(ast, LiteralString):
        escaped = ast.value.replace('\\', '\\\\').replace('"', '\\"')
        return f'"{escaped}"'

    if isinstance(ast, FieldRef):
        # Go struct fields are PascalCase
        field_name = ast.name  # Already PascalCase in rulebook
        return f'{struct_name}.{field_name}'

    if isinstance(ast, UnaryOp):
        if ast.op == 'NOT':
            operand = compile_to_go(ast.operand, struct_name, field_types)
            # Wrap in boolVal for nil-safe access
            if isinstance(ast.operand, FieldRef):
                return f'!boolVal({operand})'
            return f'!({operand})'
        raise ValueError(f"Unknown unary op: {ast.op}")

    if isinstance(ast, BinaryOp):
        # Handle comparisons involving field refs (pointer fields in Go)
        if isinstance(ast.left, FieldRef) and isinstance(ast.right, FieldRef):
            # Both sides are field refs - wrap both in boolVal for nil-safe comparison
            left = compile_to_go(ast.left, struct_name, field_types)
            right = compile_to_go(ast.right, struct_name, field_types)
            op_map = {'=': '==', '<>': '!=', '<': '<', '<=': '<=', '>': '>', '>=': '>='}
            return f'(boolVal({left}) {op_map[ast.op]} boolVal({right}))'

        if isinstance(ast.left, FieldRef) and isinstance(ast.right, LiteralInt):
            # Field ref compared to integer - need nil check and dereference
            left_field = ast.left.name
            right = compile_to_go(ast.right, struct_name, field_types)
            op_go = {'=': '==', '<>': '!=', '<': '<', '<=': '<=', '>': '>', '>=': '>='}[ast.op]
            if ast.op == '=':
                return f'({struct_name}.{left_field} != nil && *{struct_name}.{left_field} == {right})'
            elif ast.op == '<>':
                return f'({struct_name}.{left_field} == nil || *{struct_name}.{left_field} != {right})'
            else:
                # For <, <=, >, >= - nil is treated as false (0 comparison semantics)
                return f'({struct_name}.{left_field} != nil && *{struct_name}.{left_field} {op_go} {right})'

        if isinstance(ast.left, FieldRef) and isinstance(ast.right, LiteralBool):
            # Field ref compared to boolean literal - use boolVal for nil-safe access
            left = compile_to_go(ast.left, struct_name, field_types)
            right = compile_to_go(ast.right, struct_name, field_types)
            op_map = {'=': '==', '<>': '!='}
            return f'(boolVal({left}) {op_map[ast.op]} {right})'

        if isinstance(ast.left, FieldRef) and isinstance(ast.right, LiteralString):
            # Field ref compared to string literal - use stringVal for nil-safe access
            left = compile_to_go(ast.left, struct_name, field_types)
            right = compile_to_go(ast.right, struct_name, field_types)
            op_map = {'=': '==', '<>': '!='}
            return f'(stringVal({left}) {op_map[ast.op]} {right})'

        if isinstance(ast.left, LiteralString) and isinstance(ast.right, FieldRef):
            # String literal compared to field ref - use stringVal for nil-safe access
            left = compile_to_go(ast.left, struct_name, field_types)
            right = compile_to_go(ast.right, struct_name, field_types)
            op_map = {'=': '==', '<>': '!='}
            return f'({left} {op_map[ast.op]} stringVal({right}))'

        left = compile_to_go(ast.left, struct_name, field_types)
        right = compile_to_go(ast.right, struct_name, field_types)
        op_map = {'=': '==', '<>': '!=', '<': '<', '<=': '<=', '>': '>', '>=': '>='}
        return f'({left} {op_map[ast.op]} {right})'

    if isinstance(ast, FuncCall):
        if ast.name == 'AND':
            parts = []
            for arg in ast.args:
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

        if ast.name == 'OR':
            parts = []
            for arg in ast.args:
                compiled = compile_to_go(arg, struct_name, field_types)
                if isinstance(arg, FieldRef):
                    parts.append(f'boolVal({compiled})')
                else:
                    parts.append(compiled)
            return '(' + ' || '.join(parts) + ')'

        if ast.name == 'IF':
            if len(ast.args) < 2:
                raise ValueError("IF requires at least 2 arguments")
            cond = compile_to_go(ast.args[0], struct_name, field_types)
            then_val = compile_to_go(ast.args[1], struct_name, field_types)
            else_val = compile_to_go(ast.args[2], struct_name, field_types) if len(ast.args) > 2 else '""'
            # Go doesn't have ternary - generate inline func
            return f'func() string {{ if {cond} {{ return {then_val} }}; return {else_val} }}()'

        if ast.name == 'NOT':
            if len(ast.args) != 1:
                raise ValueError("NOT requires 1 argument")
            operand = compile_to_go(ast.args[0], struct_name, field_types)
            if isinstance(ast.args[0], FieldRef):
                return f'!boolVal({operand})'
            return f'!({operand})'

        if ast.name == 'LOWER':
            if len(ast.args) != 1:
                raise ValueError("LOWER requires 1 argument")
            arg = compile_to_go(ast.args[0], struct_name, field_types)
            return f'strings.ToLower(stringVal({arg}))'

        if ast.name == 'FIND':
            if len(ast.args) != 2:
                raise ValueError("FIND requires 2 arguments")
            needle = compile_to_go(ast.args[0], struct_name, field_types)
            haystack = compile_to_go(ast.args[1], struct_name, field_types)
            return f'strings.Contains(stringVal({haystack}), {needle})'

        if ast.name == 'CAST':
            if len(ast.args) >= 1:
                arg = compile_to_go(ast.args[0], struct_name, field_types)
                if isinstance(ast.args[0], FieldRef):
                    # Check field type for appropriate conversion
                    field_type = field_types.get(ast.args[0].name, 'boolean').lower()
                    if field_type == 'integer':
                        return f'intToString({arg})'
                    elif field_type == 'boolean':
                        return f'boolToString(boolVal({arg}))'
                    else:
                        return f'stringVal({arg})'
                return f'fmt.Sprintf("%v", {arg})'
            raise ValueError("CAST requires at least 1 argument")

        if ast.name == 'SUM':
            # SUM(a, b, c) -> (a + b + c)
            # Typically used as SUM(IF(cond,1,0), IF(cond,1,0), ...)
            # For Go, we need IF to return int, not string
            if not ast.args:
                return '0'
            parts = []
            for arg in ast.args:
                parts.append(_compile_to_go_int(arg, struct_name, field_types))
            return '(' + ' + '.join(parts) + ')'

        if ast.name == 'SUBSTITUTE':
            # SUBSTITUTE(text, old_text, new_text) -> strings.ReplaceAll(text, old_text, new_text)
            if len(ast.args) != 3:
                raise ValueError("SUBSTITUTE requires 3 arguments")
            text = compile_to_go(ast.args[0], struct_name, field_types)
            old_text = compile_to_go(ast.args[1], struct_name, field_types)
            new_text = compile_to_go(ast.args[2], struct_name, field_types)
            # Wrap field refs in stringVal for nil-safe access
            if isinstance(ast.args[0], FieldRef):
                text = f'stringVal({text})'
            return f'strings.ReplaceAll({text}, {old_text}, {new_text})'

        raise ValueError(f"Unknown function: {ast.name}")

    if isinstance(ast, Concat):
        parts = []
        for part in ast.parts:
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

    raise ValueError(f"Unknown AST node type: {type(ast)}")
