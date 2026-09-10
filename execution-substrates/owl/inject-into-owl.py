#!/usr/bin/env python3
"""
OWL Execution Substrate - Formula-to-SHACL Compiler

This compiler:
1. Reads the rulebook (schema + formulas + data)
2. Generates ontology.owl (TBox - classes and properties)
3. Generates individuals.ttl (ABox - data instances)
4. Generates rules.shacl.ttl (SHACL-SPARQL rules from formulas)

The rulebook is the source of truth. OWL/SHACL is derived, not authored.
This script is 100% domain-agnostic - all field names come from the rulebook.
"""

import os
import sys
import re
from pathlib import Path
from dataclasses import dataclass
from typing import List, Dict, Any, Optional, Tuple, Union
from enum import Enum, auto

# Add project root to path for shared imports
sys.path.insert(0, str(Path(__file__).resolve().parent.parent.parent))

from orchestration.shared import load_rulebook, handle_clean_arg, get_rulebook_path


# =============================================================================
# ONTOLOGY NAMESPACE
# =============================================================================
#
# Every class, property, and individual in the emitted graph lives in ONE
# namespace — this IS the NTWF ontology as rendered by Effortless from the
# rulebook. We use the `effortless-ntwf:` prefix to be honest about provenance:
# the vocabulary is Jessica Talisman's NTWF (the rulebook's PK slugs are already
# `ntwf-*` / `prod-deploy-*` and the article uses the `ntwf:` prefix), but this
# graph is the Effortless rendering of it, minted from the rulebook PKs — not a
# claim to be her published serialization byte-for-byte.
#
# Base URI: `https://w3id.org/effortless-ntwf#` (w3id = persistent, redirectable
# community root). The article's canonical NTWF base is paywalled and is NOT
# declared in the rulebook, so we do NOT guess it into every IRI; if it later
# becomes known, aligning is a one-line change to ONT_NS below. The PREFIX and
# the BASE are the single source of truth for the whole file — every `effortless-ntwf:`
# token is produced via the helpers/constants here, never hard-coded inline.
ONT_PREFIX = "effortless-ntwf"
ONT_NS = "https://w3id.org/effortless-ntwf#"
# Convenience prefix token used when building Turtle/SPARQL terms by f-string.
NS = f"{ONT_PREFIX}:"


def prefix_decl() -> str:
    """The `@prefix` line declaring the ontology namespace (TBox/ABox/SHACL)."""
    return f"@prefix {ONT_PREFIX}: <{ONT_NS}> ."


# =============================================================================
# DATA TYPES
# =============================================================================

class DataType(Enum):
    BOOL = auto()
    INT = auto()
    STRING = auto()


# =============================================================================
# EXPRESSION NODE TYPES (reused from binary substrate pattern)
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
class LiteralString(ExprNode):
    value: str


@dataclass
class FieldRef(ExprNode):
    name: str  # Field name without {{ }}


@dataclass
class BinaryOp(ExprNode):
    op: str  # '=', '<>', '<', '<=', '>', '>='
    left: ExprNode
    right: ExprNode


@dataclass
class UnaryOp(ExprNode):
    op: str  # 'NOT'
    operand: ExprNode


@dataclass
class FuncCall(ExprNode):
    name: str  # 'AND', 'OR', 'IF', 'LOWER', 'FIND'
    args: List[ExprNode]


@dataclass
class Concat(ExprNode):
    parts: List[ExprNode]


@dataclass
class Arith(ExprNode):
    op: str  # '+', '-', '*', '/'
    left: ExprNode
    right: ExprNode


@dataclass
class Neg(ExprNode):
    operand: ExprNode


@dataclass
class QualifiedRef(ExprNode):
    """A table-qualified column reference, e.g. Roles!{{RoleId}}.

    Appears only inside INDEX(...) / MATCH(...) / COUNTIFS(...) argument
    positions, where it names a COLUMN of another table (the join target),
    not a value on the current row. The lookup/aggregation rule generators
    consume these directly; they never reach the scalar SPARQL compiler."""
    table: str
    column: str


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
    BANG = auto()  # table-qualifier separator: Roles!{{RoleId}}
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

        # Number — integer or decimal. We do NOT fold a leading '-' into the
        # literal here: '-' is tokenized as MINUS and the parser builds unary
        # negation, so 'a - 1' lexes as three tokens, not 'a' '(-1)'.
        if c.isdigit():
            j = i
            while j < len(formula) and formula[j].isdigit():
                j += 1
            # Optional decimal part (e.g. durations / month math).
            if j < len(formula) and formula[j] == '.' and j + 1 < len(formula) and formula[j+1].isdigit():
                j += 1
                while j < len(formula) and formula[j].isdigit():
                    j += 1
                value = float(formula[i:j])
            else:
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
        if c == '!':
            tokens.append(Token(TokenType.BANG, '!', i))
            i += 1
            continue

        # Function names / identifiers. We keep the ORIGINAL-CASE text as the
        # token value; the parser upper-cases when matching function names but
        # preserves case for table qualifiers (Roles!, WorkflowSteps!), whose
        # case is load-bearing (it must match the rulebook table name exactly).
        if c.isalpha() or c == '_':
            j = i
            while j < len(formula) and (formula[j].isalnum() or formula[j] == '_'):
                j += 1
            name = formula[i:j]
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
        left = self.parse_additive()
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
            right = self.parse_additive()
            return BinaryOp(op=op, left=left, right=right)
        return left

    def parse_additive(self) -> ExprNode:
        left = self.parse_multiplicative()
        while self.current().type in (TokenType.PLUS, TokenType.MINUS):
            op = '+' if self.current().type == TokenType.PLUS else '-'
            self.consume()
            right = self.parse_multiplicative()
            left = Arith(op=op, left=left, right=right)
        return left

    def parse_multiplicative(self) -> ExprNode:
        left = self.parse_unary()
        while self.current().type in (TokenType.STAR, TokenType.SLASH):
            op = '*' if self.current().type == TokenType.STAR else '/'
            self.consume()
            right = self.parse_unary()
            left = Arith(op=op, left=left, right=right)
        return left

    def parse_unary(self) -> ExprNode:
        if self.current().type == TokenType.MINUS:
            self.consume()
            return Neg(operand=self.parse_unary())
        return self.parse_primary()

    def parse_primary(self) -> ExprNode:
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
            raw_name = tok.value  # original case (table names are case-sensitive)
            name = tok.value.upper()

            # Table-qualified column reference: Table!{{Column}}
            if self.tokens[self.pos + 1].type == TokenType.BANG:
                self.consume(TokenType.FUNC_NAME)
                self.consume(TokenType.BANG)
                field_tok = self.consume(TokenType.FIELD_REF)
                return QualifiedRef(table=raw_name, column=field_tok.value)

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
# SPARQL EXPRESSION COMPILER
# =============================================================================

def field_to_sparql_var(field_name: str) -> str:
    """Convert field name to SPARQL variable (snake_case)."""
    # Convert CamelCase to snake_case
    s1 = re.sub('(.)([A-Z][a-z]+)', r'\1_\2', field_name)
    return '?' + re.sub('([a-z0-9])([A-Z])', r'\1_\2', s1).lower()


def field_to_property_uri(field_name: str) -> str:
    """Convert field name to property URI (camelCase)."""
    # Ensure first letter is lowercase for property
    if field_name:
        return NS + field_name[0].lower() + field_name[1:]
    return NS + 'unknown'


def escape_sparql_string(s: str) -> str:
    """Escape a string for SPARQL."""
    return s.replace('\\', '\\\\').replace('"', '\\"').replace("'", "\\'")


def _julian_day(d: str) -> str:
    """Julian Day Number of a date/dateTime's own calendar date (integer arithmetic,
    portable SPARQL 1.1). Subtracting two gives the exact day count Postgres gets
    from `end::date - start::date`."""
    a = f'FLOOR((14 - MONTH({d})) / 12)'
    y = f'(YEAR({d}) + 4800 - {a})'
    m = f'(MONTH({d}) + 12 * {a} - 3)'
    return (f'(DAY({d}) + FLOOR((153 * {m} + 2) / 5) + 365 * {y} + FLOOR({y} / 4) '
            f'- FLOOR({y} / 100) + FLOOR({y} / 400) - 32045)')


def _epoch_seconds(d: str) -> str:
    """Seconds since an arbitrary epoch for an absolute instant: the value's own
    wall-clock time minus its UTC offset, so two instants subtract to Postgres's
    EXTRACT(EPOCH FROM (end - start))."""
    tz = f'TZ({d})'
    offset = (f'IF({tz} = "" || {tz} = "Z", 0, IF(SUBSTR({tz}, 1, 1) = "-", -1, 1) * '
              f'(xsd:integer(SUBSTR({tz}, 2, 2)) * 3600 + xsd:integer(SUBSTR({tz}, 5, 2)) * 60))')
    return (f'({_julian_day(d)} * 86400 + COALESCE(HOURS({d}), 0) * 3600 + '
            f'COALESCE(MINUTES({d}), 0) * 60 + COALESCE(SECONDS({d}), 0) - {offset})')


def _raw_boolean_fields(table: Optional[str]) -> set:
    return {c.get('name') for c in TABLE_SCHEMAS.get(table or '', [])
            if c.get('type', 'raw') == 'raw' and (c.get('datatype') or '').lower() == 'boolean'}


def _boolean_operand(arg: ExprNode, field_bindings: Dict[str, str]) -> str:
    """Postgres reads a blank RAW boolean as FALSE inside NOT/AND/OR (it emits
    COALESCE(col, FALSE)); a derived boolean stays NULL. Mirror exactly that."""
    compiled = compile_to_sparql(arg, field_bindings)
    if isinstance(arg, FieldRef) and arg.name in _raw_boolean_fields(_COMPILE_CTX['table']):
        return f'COALESCE({compiled}, false)'
    return compiled


def compile_to_sparql(expr: ExprNode, field_bindings: Dict[str, str] = None) -> str:
    """Compile an expression node to a SPARQL expression."""
    if field_bindings is None:
        field_bindings = {}

    if isinstance(expr, LiteralBool):
        return 'true' if expr.value else 'false'

    if isinstance(expr, LiteralInt):
        return str(expr.value)

    if isinstance(expr, LiteralString):
        escaped = escape_sparql_string(expr.value)
        return f'"{escaped}"'

    if isinstance(expr, FieldRef):
        var_name = field_to_sparql_var(expr.name)
        field_bindings[expr.name] = var_name
        return var_name

    if isinstance(expr, UnaryOp):
        if expr.op == 'NOT':
            operand = _boolean_operand(expr.operand, field_bindings)
            return f'(!({operand}))'
        raise ValueError(f"Unknown unary op: {expr.op}")

    if isinstance(expr, BinaryOp):
        left = compile_to_sparql(expr.left, field_bindings)
        right = compile_to_sparql(expr.right, field_bindings)
        op_map = {'=': '=', '<>': '!=', '<': '<', '<=': '<=', '>': '>', '>=': '>='}
        sparql_op = op_map.get(expr.op, '=')

        # IRI-vs-string equality fix.
        #
        # A FieldRef can resolve to an IRI rather than a string literal: a
        # relationship FK, or a lookup that copies an FK (e.g. OwningDepartment =
        # INDEX(Roles!{{OwnedBy}}, ...), where OwnedBy is an owl:ObjectProperty,
        # so the looked-up value is effortless-ntwf:ntwf-engineering, an IRI — not the string
        # "ntwf-engineering"). A rulebook formula like
        #     ={{OwningDepartment}} = "ntwf-engineering"
        # then compiles to (?owning_department = "ntwf-engineering"), which in
        # SPARQL compares an IRI to a plain literal and is ALWAYS false — the bug
        # that left isEngineeringOwned/isLegalOwned (and their COUNTIFS rollups)
        # wrong while Postgres, comparing the FK's text column, got them right.
        #
        # Fix: when a field is compared for (in)equality against a STRING LITERAL,
        # compare on the field value's LOCAL NAME. REPLACE(STR(x), "^.*#", "") is
        # the right idiom because it is a NO-OP for a plain string literal (STR is
        # identity, no '#' to strip) and strips the namespace for an IRI — so it
        # is safe whether the field turns out to be IRI- or string-valued, with no
        # need to thread field-type information into this scalar compiler.
        if expr.op in ('=', '<>'):
            left_is_field = isinstance(expr.left, FieldRef)
            right_is_field = isinstance(expr.right, FieldRef)
            left_is_str = isinstance(expr.left, LiteralString)
            right_is_str = isinstance(expr.right, LiteralString)
            # COALESCE(STR(x), "") makes the wrapper NULL-SAFE: an UNBOUND field
            # (an OPTIONAL that matched nothing — e.g. a nullable override column
            # that is null) makes STR(x) raise, which would void the enclosing
            # BIND/IF and leave the result unbound. COALESCE swallows that error
            # and yields "" instead, so a blank-check like `{{Override}} <> ""`
            # correctly reads an absent value as blank (matching Postgres, where
            # the same `<> ""` compiles to `IS NOT NULL`). No effect on a bound
            # field: COALESCE(STR(bound), "") == STR(bound).
            # A literal that itself contains '#' (an IRI written as text, e.g.
            # "https://w3id.org/pko#nextVersion") can only equal the field's whole
            # text, so the namespace is not stripped for it.
            if left_is_field and right_is_str:
                left = (f'COALESCE(STR({left}), "")' if '#' in expr.right.value
                        else f'REPLACE(COALESCE(STR({left}), ""), "^.*#", "")')
            elif right_is_field and left_is_str:
                right = (f'COALESCE(STR({right}), "")' if '#' in expr.left.value
                         else f'REPLACE(COALESCE(STR({right}), ""), "^.*#", "")')

        # Ordered comparison (<, <=, >, >=) of a field against a STRING LITERAL.
        #
        # value_to_turtle() types ISO dates as xsd:date/xsd:dateTime literals
        # so the date-math functions (DATETIME_DIFF etc.) fire. But a formula like
        #     ={{ValidFrom}} <= "2026-03-01"
        # then compares an xsd:date to a PLAIN (xsd:string) literal. SPARQL's
        # ordered operators are undefined across those types, so the comparison
        # is a type error; rdflib does not raise — it falls back to an internal
        # ordering that silently INVERTS some results (the wasActiveAsOfAuditDate
        # bug: david/priya flipped). ISO-8601 dates sort correctly under plain
        # lexical string comparison, so coerce the field side to STR() — the
        # same lexical-compare the Postgres view documents ("ISO dates compare
        # lexically"). A field compared to a NUMBER literal is left numeric.
        elif expr.op in ('<', '<=', '>', '>='):
            if isinstance(expr.left, FieldRef) and isinstance(expr.right, LiteralString):
                left = f'STR({left})'
            elif isinstance(expr.right, FieldRef) and isinstance(expr.left, LiteralString):
                right = f'STR({right})'

        return f'({left} {sparql_op} {right})'

    if isinstance(expr, Arith):
        left = compile_to_sparql(expr.left, field_bindings)
        right = compile_to_sparql(expr.right, field_bindings)
        # SPARQL arithmetic operates on numeric literals; the operands here are
        # IF(...,1,0) sub-expressions and integer columns, both numeric.
        return f'({left} {expr.op} {right})'

    if isinstance(expr, Neg):
        operand = compile_to_sparql(expr.operand, field_bindings)
        return f'(- {operand})'

    if isinstance(expr, QualifiedRef):
        # A table-qualified ref names a column of ANOTHER table. It is only
        # meaningful inside a lookup/aggregation, which are compiled by dedicated
        # generators (compile_lookup_rule / compile_aggregation_rule), never by
        # this scalar path. Reaching here means a qualified ref leaked into a
        # scalar formula — surface it loudly rather than emitting nonsense.
        raise ValueError(
            f"table-qualified reference {expr.table}!{{{{{expr.column}}}}} is only "
            f"valid inside INDEX/MATCH/COUNTIFS, not a scalar expression"
        )

    if isinstance(expr, FuncCall):
        if expr.name in AGGREGATE_FUNCS:
            if _COMPILE_CTX['table'] is None:
                raise ValueError(f"{expr.name} is only valid inside a table's rule")
            var = f"?_agg{len(_COMPILE_CTX['blocks'])}"
            _COMPILE_CTX['blocks'].append(aggregation_block(_COMPILE_CTX['table'], expr, var))
            return var

        if expr.name == 'AND':
            parts = [_boolean_operand(arg, field_bindings) for arg in expr.args]
            return '(' + ' && '.join(parts) + ')'

        if expr.name == 'OR':
            parts = [_boolean_operand(arg, field_bindings) for arg in expr.args]
            return '(' + ' || '.join(parts) + ')'

        if expr.name == 'IF':
            if len(expr.args) < 2:
                raise ValueError("IF requires at least 2 arguments")
            cond = compile_to_sparql(expr.args[0], field_bindings)
            then_val = compile_to_sparql(expr.args[1], field_bindings)
            else_val = compile_to_sparql(expr.args[2], field_bindings) if len(expr.args) > 2 else '""'
            # Postgres's CASE WHEN takes the ELSE branch on a NULL condition; SPARQL's
            # IF would propagate the error and leave the whole result unbound.
            return f'IF(COALESCE({cond}, false), {then_val}, {else_val})'

        if expr.name == 'NOT':
            if len(expr.args) != 1:
                raise ValueError("NOT requires 1 argument")
            operand = _boolean_operand(expr.args[0], field_bindings)
            return f'(!({operand}))'

        if expr.name == 'LOWER':
            if len(expr.args) != 1:
                raise ValueError("LOWER requires 1 argument")
            arg = compile_to_sparql(expr.args[0], field_bindings)
            return f'LCASE({arg})'

        if expr.name == 'ISBLANK':
            # ISBLANK is true when the field is unset OR the empty string. In
            # this substrate a missing field is an unbound OPTIONAL var, and an
            # explicitly-empty FK is the literal "". Cover both.
            if len(expr.args) != 1:
                raise ValueError("ISBLANK requires 1 argument")
            arg = compile_to_sparql(expr.args[0], field_bindings)
            return f'(!BOUND({arg}) || STR({arg}) = "")'

        if expr.name == 'DATETIME_DIFF':
            # DATETIME_DIFF(end, start, unit) -> whole units between two dates.
            # We support the units the rulebook uses (months/days). Dates are
            # xsd:date/dateTime literals; SPARQL has no direct month-diff, so we
            # compute from the day delta (months ≈ days/30, floored), which is
            # the same approximation the other substrates' FORMULA engine uses.
            if len(expr.args) != 3:
                raise ValueError("DATETIME_DIFF requires 3 arguments (end, start, unit)")
            end = compile_to_sparql(expr.args[0], field_bindings)
            start = compile_to_sparql(expr.args[1], field_bindings)
            unit_node = expr.args[2]
            if not isinstance(unit_node, LiteralString):
                raise ValueError("DATETIME_DIFF unit must be a string literal")
            unit = unit_node.value.lower().rstrip('s')  # 'months' -> 'month'
            # Duration subtraction is uneven across SPARQL engines, so we compute
            # from the year/month/day accessors (YEAR/MONTH/DAY), which every
            # SPARQL 1.1 engine implements. This matches the month/day-30
            # approximation the other substrates' FORMULA engine uses.
            if unit == 'month':
                # Day-aware whole-month count, matching Postgres AGE(): a month
                # only counts once `end`'s day-of-month reaches `start`'s day. The
                # bare (YEAR*12 + MONTH) difference over-counts by one whenever
                # DAY(end) < DAY(start) (e.g. modified the 20th, today the 11th),
                # which would silently disagree with Postgres on the staleness
                # leading edge. Subtract one in that case so both substrates agree
                # for every date, not just when the days happen to line up.
                return (
                    f'((YEAR({end}) - YEAR({start})) * 12 + '
                    f'(MONTH({end}) - MONTH({start})) - '
                    f'IF(DAY({end}) < DAY({start}), 1, 0))'
                )
            if unit == 'day':
                # Exact calendar-day difference, as Postgres's end::date - start::date.
                return f'({_julian_day(end)} - {_julian_day(start)})'
            if unit == 'minute':
                # Postgres: ROUND(EXTRACT(EPOCH FROM (end - start)) / 60).
                return f'ROUND(({_epoch_seconds(end)} - {_epoch_seconds(start)}) / 60)'
            if unit == 'hour':
                # Postgres: EXTRACT(EPOCH FROM (end - start)) / 3600, unrounded.
                return f'(({_epoch_seconds(end)} - {_epoch_seconds(start)}) / 3600)'
            raise ValueError(f"DATETIME_DIFF unit not supported: {unit}")

        if expr.name == 'NOW':
            if expr.args:
                raise ValueError("NOW takes no arguments")
            return 'NOW()'

        if expr.name == 'FIND':
            if len(expr.args) != 2:
                raise ValueError("FIND requires 2 arguments")
            needle = f'STR({compile_to_sparql(expr.args[0], field_bindings)})'
            haystack = f'STR({compile_to_sparql(expr.args[1], field_bindings)})'
            # 1-based position, 0 when absent: Postgres POSITION(needle IN haystack).
            return f'IF(CONTAINS({haystack}, {needle}), STRLEN(STRBEFORE({haystack}, {needle})) + 1, 0)'

        if expr.name == 'LEFT':
            if len(expr.args) != 2:
                raise ValueError("LEFT requires 2 arguments")
            text = compile_to_sparql(expr.args[0], field_bindings)
            count = compile_to_sparql(expr.args[1], field_bindings)
            return f'SUBSTR(STR({text}), 1, {count})'

        if expr.name == 'LEN':
            if len(expr.args) != 1:
                raise ValueError("LEN requires 1 argument")
            text = f'STR({compile_to_sparql(expr.args[0], field_bindings)})'
            # Postgres takes LENGTH(NULLIF(text, '')): a blank has no length. ?__null
            # is never bound, so that branch leaves the result unbound.
            return f'IF({text} = "", ?__null, STRLEN({text}))'

        if expr.name == 'ROUNDUP':
            if len(expr.args) != 2 or not isinstance(expr.args[1], LiteralInt):
                raise ValueError("ROUNDUP requires (number, integer-literal digits)")
            number = compile_to_sparql(expr.args[0], field_bindings)
            scale = 10 ** expr.args[1].value
            # Postgres: CEIL(number * 10^digits) / 10^digits.
            return f'(CEIL(({number}) * {scale}) / {scale})'

        if expr.name == 'SUM':
            # SUM of values - use arithmetic addition
            parts = [compile_to_sparql(arg, field_bindings) for arg in expr.args]
            return '(' + ' + '.join(parts) + ')'

        if expr.name == 'SUBSTITUTE':
            # SUBSTITUTE(text, old_text, new_text) -> REPLACE(text, old_text, new_text)
            if len(expr.args) != 3:
                raise ValueError("SUBSTITUTE requires 3 arguments")
            text = compile_to_sparql(expr.args[0], field_bindings)
            old_text = compile_to_sparql(expr.args[1], field_bindings)
            new_text = compile_to_sparql(expr.args[2], field_bindings)
            return f'REPLACE({text}, {old_text}, {new_text})'

        raise ValueError(f"Unknown function: {expr.name}")

    if isinstance(expr, Concat):
        parts = []
        for part in expr.parts:
            c = compile_to_sparql(part, field_bindings)
            if isinstance(part, LiteralString):
                # Already a plain string literal — no coercion needed (and
                # keeping it bare preserves embedded '/' that the IRI branch
                # below would otherwise strip from a path separator).
                parts.append(c)
                continue
            # SPARQL CONCAT requires xsd:string arguments. A part that resolves
            # to an IRI (a relationship/FK ref) or a typed literal (xsd:date,
            # integer, boolean) is a type error that leaves the WHOLE CONCAT —
            # and therefore the field — unbound, the silent name-gap on
            # StepPrecedence/RoleAssignments/ChangeLog. Coerce each non-literal
            # part: an IRI to its trailing local name (the raw PK text Postgres
            # concatenates for an FK), any literal to its lexical string. A
            # date's lexical form is the TIMEZONE-INDEPENDENT ISO day
            # (2026-01-10) — so both substrates render the identical string on
            # every machine, with no -06/-05 offset baked in by a date->
            # timestamptz cast. (A bare string field keeps its slashes because
            # STR() is identity and the IRI branch never fires for a literal.)
            # Postgres CONCAT skips a NULL part, so an unbound part contributes "".
            # A dateTime renders as Postgres's timestamptz text, e.g.
            # "2026-01-01 00:00:00-06", taken in the value's own UTC offset.
            tz = f'TZ({c})'
            pg_datetime = (f'CONCAT(SUBSTR(STR({c}), 1, 10), " ", SUBSTR(STR({c}), 12, 8), '
                           f'IF({tz} = "Z", "+00", IF(STRENDS({tz}, ":00"), SUBSTR({tz}, 1, 3), {tz})))')
            parts.append(
                f'COALESCE(IF(isIRI({c}), REPLACE(STR({c}), "^.*[#/]", ""), '
                f'IF(DATATYPE({c}) = xsd:dateTime, {pg_datetime}, STR({c}))), "")')
        return 'CONCAT(' + ', '.join(parts) + ')'

    raise ValueError(f"Unknown expression node type: {type(expr)}")


# =============================================================================
# OWL/TURTLE GENERATORS
# =============================================================================

def datatype_to_xsd(datatype: str) -> str:
    """Convert rulebook datatype to XSD type."""
    dt = datatype.lower()
    if dt == 'boolean':
        return 'xsd:boolean'
    elif dt == 'integer':
        return 'xsd:integer'
    elif dt == 'date':
        return 'xsd:date'
    elif dt == 'datetime':
        return 'xsd:dateTime'
    else:
        return 'xsd:string'


# =============================================================================
# RELATIONAL → ONTOLOGICAL MODEL DISCOVERY
#
# The rulebook is the SSoT. Everything below is derived purely from the
# field-schema metadata — no hardcoded table or field names. The shapes we
# read:
#   - PK field         : the first `raw` field of a table (convention:
#                         <Table>Id / ConceptId / MetaKey). Its *value* in a
#                         row mints that individual's IRI.
#   - relationship     : type == 'relationship' with `RelatedTo: <TargetTable>`.
#                         A single-valued forward FK → owl:ObjectProperty edge
#                         to the target individual (resolved by the target's PK).
#   - closure          : type == 'closure' with EdgeTable/FromColumn/ToColumn
#                         (or just ToColumn for self-edges). Declares the *base*
#                         object property transitive so a reasoner derives the
#                         full closure the article's headline depends on.
#   - inverse rels     : two relationship fields A.x→B and B.y→A that point at
#                         each other are emitted as owl:inverseOf so the reasoner
#                         derives the reverse direction instead of us pre-baking
#                         comma-joined string lists (the old `effortless-ntwf:roles ""` noise).
# =============================================================================


def slugify_iri_local(value: str) -> str:
    """Turn a PK value into a safe Turtle IRI local-name.

    PK values like 'ntwf-maria-gonzalez' or 'prod-deploy-step-1' are already
    URI-safe; we only guard against whitespace/illegal chars so the emitted
    Turtle always parses. We do NOT lowercase or otherwise mangle — the PK IS
    the identity, and the reasoner matches on exact IRI.
    """
    s = str(value).strip()
    # Turtle PN_LOCAL is permissive but spaces and a few punctuation chars break
    # it; replace the unsafe ones with '-' deterministically.
    return re.sub(r'[^A-Za-z0-9_\-.]', '-', s)


def get_pk_field(schema: List[Dict[str, Any]]) -> Optional[str]:
    """The primary-key field of a table = its first raw field.

    Matches the existing IRI convention (individuals were keyed positionally,
    but every table's first raw column is its identifier: WorkflowStepId,
    HumanAgentId, ConceptId, MetaKey, ...). Returns the field name or None.
    """
    for col in schema:
        if col.get('type', 'raw') == 'raw':
            return col.get('name')
    # Fallback: first field of any type (a table with no raw field is unusual,
    # but we still need an identity column rather than silently guessing).
    return schema[0].get('name') if schema else None


def is_relationship(col: Dict[str, Any]) -> bool:
    return col.get('type') == 'relationship' and bool(col.get('RelatedTo'))


def build_pk_index(tables: Dict[str, Any]) -> Dict[str, str]:
    """Map table_name -> its PK field name, for FK resolution."""
    idx = {}
    for tname, tdef in tables.items():
        if tname.startswith('_') or tname.startswith('$'):
            continue
        pk = get_pk_field(tdef.get('schema', []))
        if pk:
            idx[tname] = pk
    return idx


# PK slugs that more than one table uses. A bare effortless-ntwf:<pk> IRI would merge
# those rows into ONE individual carrying both tables' properties, so these (and
# only these) are table-scoped. Populated by register_pk_collisions().
COLLIDING_PK_SLUGS: set = set()


def register_pk_collisions(tables: Dict[str, Any]) -> None:
    owners: Dict[str, set] = {}
    for tname, pk in build_pk_index(tables).items():
        for row in tables[tname].get('data', []):
            value = row.get(pk)
            if value is not None and str(value).strip():
                owners.setdefault(slugify_iri_local(value), set()).add(tname)
    COLLIDING_PK_SLUGS.clear()
    COLLIDING_PK_SLUGS.update(slug for slug, owning in owners.items() if len(owning) > 1)


def individual_iri(table_name: str, pk_value: Any) -> str:
    """Mint a stable, PK-keyed individual IRI: effortless-ntwf:<pkvalue>.

    The IRI is the bare PK — human-readable and stable across reorderings. Edges
    resolve by minting the same IRI from the FK value. The PK slug is the IRI's
    local name verbatim (kebab-case, colon-separated from the prefix) — the owly
    `effortless-ntwf:prod-deploy-step-3` form. A PK value shared by two tables is
    prefixed with its table (effortless-ntwf:RulebookReleases-erb-pko-1.0.0).
    """
    slug = slugify_iri_local(pk_value)
    if slug in COLLIDING_PK_SLUGS:
        return f'{NS}{table_name}-{slug}'
    return f'{NS}{slug}'


def find_inverse_pairs(tables: Dict[str, Any]) -> Dict[str, str]:
    """Detect inverse relationship pairs: A.fwd→B and B.rev→A.

    Returns {('Table','field'): ('Table','field')} flattened to
    {'Table.field': 'Table.field'} for the *forward* (single-valued, asserted)
    side → the reverse side, so the TBox can emit owl:inverseOf and we never
    pre-bake the reverse direction as literals.

    Heuristic: the forward side is the one whose value in the data is a single
    PK; the reverse side holds a list. We don't need data to decide direction
    for the TBox — owl:inverseOf is symmetric — so we just emit one inverseOf
    axiom per matched pair (keyed by the lexically-first side to dedupe).

    Disambiguation: when two tables share MORE THAN ONE relationship in a given
    direction (StepPrecedence.FromStep/ToStep both → WorkflowSteps, with
    WorkflowSteps.Precedes/PrecededBy both → StepPrecedence), the plain symmetric
    match above is ambiguous — every forward FK matches every reverse back-ref,
    and the last write wins, so BOTH forward FKs can collapse onto ONE reverse
    property. That is a *corrupting* axiom: two distinct FKs (fromStep, toStep)
    declared `owl:inverseOf` the SAME property makes OWL-RL fold them together,
    so every fromStep value also shows up as a toStep value (and the path/lookup
    fields built on them go multi-valued). A back-reference field whose
    Description pins its exact partner — "Inverse of <Table>.<Field>" — resolves
    the ambiguity deterministically; we honor that pointer over the heuristic.
    """
    rels = []  # (table, field, target_table)
    rel_names = set()  # {'Table.field'} for validating explicit pointers
    for tname, tdef in tables.items():
        if tname.startswith('_') or tname.startswith('$'):
            continue
        for col in tdef.get('schema', []):
            if is_relationship(col):
                rels.append((tname, col['name'], col['RelatedTo']))
                rel_names.add(f'{tname}.{col["name"]}')

    pairs = {}
    seen = set()
    for (t, f, target) in rels:
        # Does the target table have a relationship pointing back at t?
        for (t2, f2, target2) in rels:
            if t2 == target and target2 == t and (t, f) != (t2, f2):
                key = tuple(sorted([f'{t}.{f}', f'{t2}.{f2}']))
                if key not in seen:
                    seen.add(key)
                    pairs[key[0]] = key[1]

    # Override the heuristic with any explicit "Inverse of <Table>.<Field>"
    # pointer authored in a relationship field's Description. Each such pointer
    # pins one exact field<->field correspondence, so a 2x2 table-pair resolves
    # to the right 1:1 matching instead of collapsing both FKs onto one reverse.
    # An explicit pointer always wins; a pair it does not mention keeps the
    # heuristic's choice (correct for the common unambiguous 1:1 case).
    inv_re = re.compile(r'[Ii]nverse of ([A-Za-z_][A-Za-z0-9_]*)\.([A-Za-z_][A-Za-z0-9_]*)')
    for tname, tdef in tables.items():
        if tname.startswith('_') or tname.startswith('$'):
            continue
        for col in tdef.get('schema', []):
            if not is_relationship(col):
                continue
            m = inv_re.search(col.get('Description', '') or '')
            if not m:
                continue
            partner = f'{m.group(1)}.{m.group(2)}'
            if partner not in rel_names:
                continue  # pointer names a non-relationship / unknown field
            key = tuple(sorted([f'{tname}.{col["name"]}', partner]))
            pairs[key[0]] = key[1]
    return pairs


def value_to_turtle(value: Any, datatype: str) -> str:
    """Convert a Python value to Turtle literal syntax."""
    if value is None:
        return None
    dt = datatype.lower()
    if dt == 'boolean':
        return 'true' if value else 'false'
    elif dt == 'integer':
        return str(int(value)) if value is not None else '0'
    elif dt in ('date', 'datetime'):
        # ISO-8601 strings from the rulebook (with or without a time/offset
        # component) parse directly as xsd:date / xsd:dateTime — no reformat
        # needed. A typed literal is required for YEAR()/MONTH()/DAY() and
        # ordered comparison in SPARQL: an untyped (xsd:string) literal makes
        # those a silent type error that voids the enclosing BIND (rdflib
        # drops the row instead of raising), not a comparison mistake.
        xsd_type = 'xsd:date' if dt == 'date' else 'xsd:dateTime'
        escaped = str(value).replace('\\', '\\\\').replace('"', '\\"')
        return f'"{escaped}"^^{xsd_type}'
    else:
        # String - escape for Turtle
        s = str(value)
        # Use triple quotes for multi-line strings
        if '\n' in s or '\r' in s:
            # Escape backslashes and any triple quotes in content
            escaped = s.replace('\\', '\\\\').replace('"""', '\\"\\"\\"')
            return f'"""{escaped}"""'
        else:
            # Single-line string - use regular quotes
            escaped = s.replace('\\', '\\\\').replace('"', '\\"')
            return f'"{escaped}"'


def _esc_comment(text: str) -> str:
    return text.replace('\\', '\\\\').replace('"', '\\"')


def closure_base_property(closure_col: Dict[str, Any], rels_by_name: Dict[str, str]) -> Optional[str]:
    """Resolve the *base* object property a closure field is the closure OF.

    A closure field carries EdgeTable/FromColumn/ToColumn (junction-based, e.g.
    StepPrecedence(FromStep→ToStep)) or just ToColumn (self-edge, e.g.
    Roles.DelegatesTo). The base property is the relationship named by ToColumn;
    declaring IT transitive lets a reasoner derive the full closure. Returns the
    property URI (effortless-ntwf:precedesStep style) or None.
    """
    to_col = closure_col.get('ToColumn')
    edge_table = closure_col.get('EdgeTable')
    if edge_table and to_col:
        # Junction closure: the directed edge is FromColumn→ToColumn within the
        # edge table. The semantic base property is named after the closure's
        # intent; we derive it from ToColumn ('ToStep' → precedesStep is too
        # cute to guess, so we name it after the edge table's directed meaning:
        # <fromcol-stripped>Precedes... — simplest stable choice: 'precedes<To>').
        # We key the base property off the edge-table's column pair so it is
        # deterministic and unique.
        return f'{NS}{_lower_first(closure_col["name"].replace("Closure", ""))}'
    if to_col:
        # Self-edge closure: base property is the ToColumn relationship itself.
        return field_to_property_uri(to_col)
    return None


def _lower_first(s: str) -> str:
    return s[0].lower() + s[1:] if s else s


def colliding_relationship_names(tables: Dict[str, Any]) -> set:
    """Back-reference relationship field NAMES to suppress in the OWL output.

    A *back-reference* is a relationship field whose name equals its RelatedTo
    target table (the reverse-reference naming convention: HumanAgents.Roles ->
    Roles, Workflows.WorkflowSteps -> WorkflowSteps). These are the reverse
    direction of a forward FK and are the ones that collide to one multi-domain
    property URI under field_to_property_uri — poisoning OWL-RL type inference.
    We suppress them; the forward edge carries the relation and the reverse is
    recovered by querying forward.

    Crucially this does NOT catch a genuine forward FK whose name merely happens
    to repeat across tables (WorkflowSteps.Workflow and ComplianceVerdicts.Workflow
    both -> Workflows): 'Workflow' != 'Workflows', so it is kept. Suppressing it
    would break the structural-parent path chain (a step's path needs its
    Workflow edge).
    """
    out = set()
    for tname, tdef in tables.items():
        if tname.startswith('_') or tname.startswith('$'):
            continue
        for col in tdef.get('schema', []):
            if is_relationship(col) and col['name'] == col.get('RelatedTo'):
                out.add(col['name'])
    return out


def shared_field_names(tables: Dict[str, Any]) -> set:
    """ALL field names (any type) defined on more than one table.

    Because field_to_property_uri collapses a name to one property URI, a shared
    name yields one property with multiple rdfs:domain declarations. Under OWL-RL,
    `rdfs:domain C` means `x P _ ⟹ x a C`, so a property with 14 domains types
    EVERY subject as all 14 classes — the catastrophic collapse (e.g. the 'iri'
    and 'name' fields exist on every table). The fix: do not emit rdfs:domain for
    a property whose field name is shared. Domain is not needed for our queries —
    individuals are explicitly typed (`a effortless-ntwf:Class`) in the ABox, and ranges +
    the SHACL targetClass carry the rest. Unshared properties keep their domain.
    """
    import collections as _c
    name_tables = _c.defaultdict(set)
    for tname, tdef in tables.items():
        if tname.startswith('_') or tname.startswith('$'):
            continue
        for col in tdef.get('schema', []):
            name_tables[col.get('name', '')].add(tname)
    return {name for name, ts in name_tables.items() if len(ts) > 1}


def generate_ontology_owl(tables: Dict[str, Any]) -> str:
    """Generate OWL TBox (schema) from rulebook tables.

    Relationship fields become owl:ObjectProperty (edges between individuals),
    not owl:DatatypeProperty (string literals). Closure fields declare their
    base property owl:TransitiveProperty so a reasoner derives the full
    reachability closure. Single-valuedness is a SHACL cardinality constraint
    (NOT owl:FunctionalProperty, which would collapse individuals via sameAs).
    The three agent classes are pairwise owl:disjointWith. Unambiguous inverse
    relationship pairs get owl:inverseOf so the reverse direction is inferred.

    Relationship field names shared across tables (back-references like 'Roles')
    are SKIPPED — emitting them as one multi-domain property poisons OWL-RL type
    inference. See colliding_relationship_names.
    """
    lines = []
    pk_index = build_pk_index(tables)
    inverse_pairs = find_inverse_pairs(tables)
    colliding = colliding_relationship_names(tables)
    shared = shared_field_names(tables)  # names on >1 table → omit rdfs:domain

    # Prefixes
    lines.append('@prefix rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#> .')
    lines.append('@prefix rdfs: <http://www.w3.org/2000/01/rdf-schema#> .')
    lines.append('@prefix owl: <http://www.w3.org/2002/07/owl#> .')
    lines.append('@prefix xsd: <http://www.w3.org/2001/XMLSchema#> .')
    lines.append(prefix_decl())
    lines.append('')
    lines.append(f'<{ONT_NS}> a owl:Ontology ;')
    lines.append('    rdfs:label "NTWF Ontology (Effortless rendering)" ;')
    lines.append(f'    rdfs:comment "Generated by Effortless from {get_rulebook_path().name}. Vocabulary: Jessica Talisman NTWF." .')
    lines.append('')

    # Track object properties we declare so the transitive/functional axiom pass
    # can attach axioms without redefining the property. Maps prop_uri -> set of
    # axiom keywords ('owl:TransitiveProperty', 'owl:FunctionalProperty').
    object_props: Dict[str, set] = {}
    closure_base_props: Dict[str, Dict[str, str]] = {}  # base_prop -> {from,to tables}

    # ---- Pass 1: classes + datatype/object properties ----
    for table_name, table_def in sorted(tables.items()):
        if table_name.startswith('_') or table_name.startswith('$'):
            continue  # Skip metadata

        schema = table_def.get('schema', [])
        if not schema:
            continue

        class_uri = f'{NS}{table_name}'
        entity_description = table_def.get('Description', '')
        lines.append(f'# === Class: {table_name} ===')
        lines.append(f'{class_uri} a owl:Class ;')
        if entity_description:
            lines.append(f'    rdfs:label "{table_name}" ;')
            lines.append(f'    rdfs:comment "{_esc_comment(entity_description)}" .')
        else:
            lines.append(f'    rdfs:label "{table_name}" .')
        lines.append('')

        for col in schema:
            col_name = col.get('name', '')
            col_type = col.get('type', 'raw')
            col_description = col.get('Description', '')
            prop_uri = field_to_property_uri(col_name)

            if is_relationship(col):
                # Skip back-reference fields whose name collides across tables —
                # one multi-domain property poisons OWL-RL type inference.
                if col_name in colliding:
                    continue
                # FK → ObjectProperty edge to the related class. Omit rdfs:domain
                # when the field name is shared across tables (else OWL-RL types
                # every subject as every domain class).
                target = col['RelatedTo']
                target_class = f'{NS}{target}'
                lines.append(f'{prop_uri} a owl:ObjectProperty ;')
                if col_name not in shared:
                    lines.append(f'    rdfs:domain {class_uri} ;')
                lines.append(f'    rdfs:range {target_class} ;')
                if col_description:
                    lines.append(f'    rdfs:label "{col_name}" ;')
                    lines.append(f'    rdfs:comment "{_esc_comment(col_description)}" .')
                else:
                    lines.append(f'    rdfs:label "{col_name}" .')
                lines.append('')
                object_props.setdefault(prop_uri, set())
                # A forward FK (the side NOT on the reverse list of an inverse
                # pair) is single-valued → FunctionalProperty. We mark all rels
                # here and demote the multi-valued reverse side below.
                continue

            if col_type == 'closure':
                # The closure field itself is not a stored property — it is the
                # transitive closure of a base object property. Register the base
                # property as transitive; the actual edges live on the base rel.
                base = closure_base_property(col, {})
                if base:
                    object_props.setdefault(base, set()).add('owl:TransitiveProperty')
                    closure_base_props[base] = {
                        'from_table': col.get('EdgeTable') or table_name,
                        'from_col': col.get('FromColumn'),
                        'to_col': col.get('ToColumn'),
                        'subject_table': table_name,
                    }
                    lines.append(f'# {prop_uri} is the transitive closure of {base}')
                    lines.append(f'{base} a owl:ObjectProperty , owl:TransitiveProperty ;')
                    lines.append(f'    rdfs:label "{col_name.replace("Closure", "")}" .')
                    lines.append('')
                continue

            # Default: datatype property (raw scalar, calc result, etc.). Omit
            # rdfs:domain for shared field names (e.g. iri/name/displayName exist
            # on every table) — one property with N domains makes OWL-RL type
            # every subject as all N classes. Individuals are explicitly typed in
            # the ABox, so dropping domain costs no real information.
            col_datatype = col.get('datatype', 'string')
            xsd_type = datatype_to_xsd(col_datatype)
            lines.append(f'{prop_uri} a owl:DatatypeProperty ;')
            if col_name not in shared:
                lines.append(f'    rdfs:domain {class_uri} ;')
            lines.append(f'    rdfs:range {xsd_type} ;')
            if col_description:
                lines.append(f'    rdfs:label "{col_name}" ;')
                lines.append(f'    rdfs:comment "{_esc_comment(col_description)}" .')
            else:
                lines.append(f'    rdfs:label "{col_name}" .')
            if col.get('formula'):
                lines.append(f'    # calculated: {col_type}')
            lines.append('')

    # ---- Pass 2: OWL axioms (inverseOf only) ----
    #
    # NOTE on single-valuedness: a forward FK is single-valued, but we do NOT
    # declare it owl:FunctionalProperty. Under OWL's open-world / no-Unique-Name
    # semantics, FunctionalProperty is an INFERENCE rule: `x P y . x P z` entails
    # `y owl:sameAs z`. With many subjects sharing a functional target (every
    # WorkflowStep .workflow -> the one Workflow, etc.) an OWL-RL reasoner
    # collapses distinct individuals into one via sameAs, poisoning every query.
    # Single-valuedness is a *cardinality constraint*, not an identity inference,
    # so we express it as SHACL `sh:maxCount 1` in rules.shacl.ttl instead —
    # validation without merging. (See generate_shacl_rules / discover_single_
    # valued_fks.) The closure base properties stay owl:TransitiveProperty above.
    if inverse_pairs:
        lines.append('# === Inverse object-property pairs (reverse direction inferred) ===')
        for fwd_key, rev_key in sorted(inverse_pairs.items()):
            fwd_name = fwd_key.split('.', 1)[1]
            rev_name = rev_key.split('.', 1)[1]
            # Skip if either side is a collapsed multi-table back-reference: its
            # property was not emitted, so an inverseOf to it would dangle and
            # (worse) re-introduce the cross-domain type leak.
            if fwd_name in colliding or rev_name in colliding:
                continue
            fwd_prop = field_to_property_uri(fwd_name)
            rev_prop = field_to_property_uri(rev_name)
            if fwd_prop != rev_prop:
                lines.append(f'{fwd_prop} owl:inverseOf {rev_prop} .')
        lines.append('')

    # ---- Pass 3: disjointness over sibling FK-target class groups ----
    # When N relationship fields on one table each point at a different target
    # class AND share a common name prefix (FilledBy*, AttributedTo*), those
    # target classes are mutually exclusive role-fillers → owl:disjointWith.
    # This is derived structurally (no hardcoded class names): a set of
    # same-prefixed single-valued FKs from one row means "exactly one of these
    # target types", i.e. the targets are disjoint.
    disjoint_groups = discover_disjoint_class_groups(tables)
    if disjoint_groups:
        lines.append('# === Disjoint class groups (mutually-exclusive FK targets) ===')
        for group in disjoint_groups:
            classes = sorted(group)
            for i, a in enumerate(classes):
                for b in classes[i + 1:]:
                    lines.append(f'{NS}{a} owl:disjointWith {NS}{b} .')
        lines.append('')

    return '\n'.join(lines)


def _singular(s: str) -> str:
    """Crude singularizer for matching a field-name suffix against a class stem.
    'HumanAgents' -> 'HumanAgent'. Only strips a trailing 's'."""
    return s[:-1] if s.endswith('s') else s


def discover_disjoint_class_groups(tables: Dict[str, Any]) -> List[set]:
    """Find sets of classes that are mutually-exclusive (polymorphic) FK targets.

    The ONLY pattern that proves disjointness structurally is a *polymorphic
    foreign key*: one table has >=2 relationship fields of the form
    `<sharedPrefix><TargetStem>`, where each field's suffix (after the shared
    prefix) names its OWN target class. That is the relational encoding of
    "exactly one of these N types fills this slot" — e.g.
        FilledByHumanAgent       -> HumanAgents
        FilledByAIAgent          -> AIAgents
        FilledByAutomatedPipeline-> AutomatedPipelines
    The suffix (HumanAgent / AIAgent / AutomatedPipeline) singular-matches the
    target class, so the targets are pairwise owl:disjointWith.

    We deliberately REJECT same-prefix coincidences where the suffix does NOT
    name the target (e.g. Workflows.WorkflowStatus -> WorkflowStatusConcepts vs
    Workflows.WorkflowSteps -> WorkflowSteps share prefix 'Workflow' but are not
    a polymorphic slot — 'Status' is not 'WorkflowStatusConcepts'). Those are
    distinct relations, not disjoint alternatives.
    """
    import collections as _c

    groups: List[set] = []
    for tname, tdef in tables.items():
        if tname.startswith('_') or tname.startswith('$'):
            continue
        rels = [c for c in tdef.get('schema', []) if is_relationship(c)]
        if len(rels) < 2:
            continue

        # Candidate polymorphic members: those whose name suffix matches their
        # target class (singularized). Group by the prefix that remains.
        members = []  # (prefix, target)
        for col in rels:
            name, target = col['name'], col['RelatedTo']
            stem = _singular(target)
            if name.endswith(stem) and len(name) > len(stem):
                prefix = name[:-len(stem)]
                members.append((prefix, target))

        # Bucket by shared prefix; a real polymorphic FK has >=2 members all
        # sharing the SAME leading prefix.
        by_prefix = _c.defaultdict(set)
        for prefix, target in members:
            by_prefix[prefix].add(target)
        for prefix, targets in by_prefix.items():
            if prefix and len(targets) >= 2:
                groups.append(set(targets))

    # Merge overlapping groups (same triad seen in two tables → one set).
    merged: List[set] = []
    for g in groups:
        placed = False
        for m in merged:
            if m & g:
                m |= g
                placed = True
                break
        if not placed:
            merged.append(set(g))
    return merged


def _split_multi(value: Any) -> List[str]:
    """A relationship value may be a single PK, a list, or a comma-joined
    string of PKs. Normalize to a list of non-empty PK strings."""
    if value is None:
        return []
    if isinstance(value, list):
        items = value
    else:
        items = str(value).split(',')
    return [s.strip() for s in items if s is not None and str(s).strip() != '']


def generate_individuals_ttl(tables: Dict[str, Any]) -> str:
    """Generate ABox (individuals/data) from rulebook tables.

    Individuals are keyed by PRIMARY KEY value (effortless-ntwf:<pk>), not positional index.
    Relationship fields emit owl:ObjectProperty EDGES to the target individual
    (effortless-ntwf:<targetPk>), resolved through the FK value — not string literals.
    Multi-valued FK values become multiple edges (no comma-joined strings).
    The reverse direction of an inverse pair is NOT emitted; the reasoner
    infers it via owl:inverseOf, which also removes the old empty `roles ""`
    noise. Junction edge-tables additionally project a DIRECT edge on the base
    property (e.g. precedesStep) between the two endpoints so the reasoner's
    owl:TransitiveProperty can chain them into the full closure.
    """
    lines = []
    pk_index = build_pk_index(tables)
    # The edges we SUPPRESS are the back-references — relationship fields whose
    # NAME collides across tables (Roles / WorkflowSteps / Workflow). Those are
    # the reverse direction of a forward FK; the forward edge carries the
    # relation and the reverse is queried forward. Using the colliding-name set
    # (rather than find_inverse_pairs' alphabetical pick) is correct: it never
    # accidentally drops a single-valued forward FK like FilledByHumanAgent.
    suppressed_rel_names = colliding_relationship_names(tables)

    # Prefixes
    lines.append('@prefix rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#> .')
    lines.append('@prefix rdfs: <http://www.w3.org/2000/01/rdf-schema#> .')
    lines.append('@prefix xsd: <http://www.w3.org/2001/XMLSchema#> .')
    lines.append(prefix_decl())
    lines.append('')

    # Collect direct junction-projected edges to emit in a dedicated block, so
    # the base transitive property carries real edges between endpoint
    # individuals (StepPrecedence row {FromStep, ToStep} → step :precedesStep step).
    junction_edges = []  # (subject_iri, base_prop, object_iri)

    for table_name, table_def in sorted(tables.items()):
        if table_name.startswith('_') or table_name.startswith('$'):
            continue

        schema = table_def.get('schema', [])
        data = table_def.get('data', [])
        if not schema or not data:
            continue

        pk_field = pk_index.get(table_name)

        # Find closure base property + junction column pair for this table, if
        # this table IS an edge table for some closure field somewhere.
        junction_for_table = _closure_junctions_for(table_name, tables)

        col_info = {}
        for col in schema:
            col_info[col.get('name', '')] = {
                'datatype': col.get('datatype', 'string'),
                'type': col.get('type', 'raw'),
                'related_to': col.get('RelatedTo'),
                'is_rel': is_relationship(col),
            }

        lines.append(f'# === Individuals: {table_name} ===')
        lines.append('')
        class_uri = f'{NS}{table_name}'

        for i, row in enumerate(data):
            pk_value = row.get(pk_field) if pk_field else None
            if pk_value is None or str(pk_value).strip() == '':
                # No identity = a malformed rulebook row. Minting a positional
                # IRI here would be a silent fallback (see CLAUDE.md): it would
                # paper over the bad row and let every FK that points at this PK
                # dangle against a fabricated subject. The PK IS the identity in
                # this graph — fail loudly with the exact row that's broken.
                raise ValueError(
                    f"{table_name}[{i}] has no primary-key value in field "
                    f"'{pk_field}' — cannot mint an individual IRI. Fix the "
                    f"rulebook row; the OWL substrate keys every individual by PK."
                )
            ind_uri = individual_iri(table_name, pk_value)

            lines.append(f'{ind_uri} a {class_uri} ;')

            props = []
            for col_name, info in col_info.items():
                # Calc/lookup/agg are ANSWERS produced by rules — never inputs.
                if info['type'] in ('calculated', 'lookup', 'aggregation', 'closure'):
                    continue

                value = row.get(col_name)
                if value is None:
                    continue

                if info['is_rel']:
                    # Skip back-reference edges (colliding names) — they are the
                    # reverse direction, recovered by querying the forward edge.
                    if col_name in suppressed_rel_names:
                        continue
                    target_table = info['related_to']
                    prop_uri = field_to_property_uri(col_name)
                    for fk in _split_multi(value):
                        obj_iri = individual_iri(target_table, fk)
                        props.append(f'    {prop_uri} {obj_iri}')
                    continue

                # Scalar datatype property
                prop_uri = field_to_property_uri(col_name)
                turtle_val = value_to_turtle(value, info['datatype'])
                if turtle_val is not None:
                    props.append(f'    {prop_uri} {turtle_val}')

            if props:
                lines.append(' ;\n'.join(props) + ' .')
            else:
                lines[-1] = lines[-1].replace(' ;', ' .')
            lines.append('')

            # Junction projection: if this table is a closure edge table, emit a
            # direct base-property edge between the FromColumn and ToColumn
            # endpoint individuals.
            for jn in junction_for_table:
                from_pk = row.get(jn['from_col'])
                to_pk = row.get(jn['to_col'])
                if from_pk and to_pk:
                    from_iri = individual_iri(jn['endpoint_table'], from_pk)
                    to_iri = individual_iri(jn['endpoint_table'], to_pk)
                    junction_edges.append((from_iri, jn['base_prop'], to_iri))

    # Self-edge closures (e.g. Roles.DelegatesTo): the base property edges are
    # already asserted as ordinary relationship edges above (DelegatesTo is a
    # normal relationship field), so the transitive axiom in the TBox closes
    # them. Nothing extra to emit here.

    if junction_edges:
        lines.append('# === Junction-projected base edges (transitive closure seeds) ===')
        for (s, p, o) in junction_edges:
            lines.append(f'{s} {p} {o} .')
        lines.append('')

    return '\n'.join(lines)


def _closure_junctions_for(table_name: str, tables: Dict[str, Any]) -> List[Dict[str, str]]:
    """If `table_name` is the EdgeTable of any closure field, return the
    junction projection spec(s): base property + from/to columns + the endpoint
    class both columns point at."""
    out = []
    for tname, tdef in tables.items():
        if tname.startswith('_') or tname.startswith('$'):
            continue
        for col in tdef.get('schema', []):
            if col.get('type') == 'closure' and col.get('EdgeTable') == table_name:
                from_col = col.get('FromColumn')
                to_col = col.get('ToColumn')
                base = closure_base_property(col, {})
                # Endpoint class: the RelatedTo of the FromColumn relationship in
                # the edge table (both columns point at the same endpoint class
                # for a step→step / role→role precedence edge).
                endpoint_table = None
                for ec in tables.get(table_name, {}).get('schema', []):
                    if ec.get('name') == from_col and ec.get('RelatedTo'):
                        endpoint_table = ec['RelatedTo']
                        break
                if base and from_col and to_col and endpoint_table:
                    out.append({
                        'base_prop': base,
                        'from_col': from_col,
                        'to_col': to_col,
                        'endpoint_table': endpoint_table,
                    })
    return out


# =============================================================================
# LOOKUP / AGGREGATION SPARQL GENERATORS
#
# These are cross-individual joins, not scalar BINDs. Because the TBox emits
# every FK as an owl:ObjectProperty edge to the target individual (keyed by Iri),
# an INDEX(Target!{{Col}}, MATCH({{LocalFK}}, Target!{{PK}}, 0)) collapses to
# "follow the LocalFK edge, read Col on the target" — the MATCH against the PK is
# exactly what the edge already encodes. COUNTIFS becomes a sub-SELECT COUNT over
# the child individuals whose back-edge points at $this and whose criteria match.
# =============================================================================

INDENT = '                '  # matches the WHERE-body indentation in the template


def _next_join_var(n: int) -> str:
    return f'?j{n}'


def _compile_lookup_value(expr: ExprNode, state: Dict[str, Any]) -> str:
    """Recursively lower a lookup formula to a SPARQL value expression,
    appending any join triples to state['triples'] and collecting the
    OPTIONAL local-field bindings into state['locals'].

    Supports the two shapes the rulebook uses:
      INDEX(Target!{{ResultCol}}, MATCH({{LocalFK}}, Target!{{PK}}, 0))
      <the above> & "literal" & {{LocalScalar}}      (RelativePath pattern)
    """
    if isinstance(expr, Concat):
        # CONCAT genuinely needs strings, so coerce each part with STR() HERE —
        # at the string-building boundary — rather than at the leaf. This keeps
        # a *standalone* lookup (a bare INDEX/MATCH copy of one field) returning
        # its native datatype, so a copied boolean stays a boolean and downstream
        # `&&` / numeric rules work. Wrapping at the leaf was the bug: it turned
        # a copied `false` into the truthy string "false".
        parts = [f'STR({_compile_lookup_value(p, state)})' for p in expr.parts]
        return 'CONCAT(' + ', '.join(parts) + ')'

    if isinstance(expr, LiteralString):
        return f'"{escape_sparql_string(expr.value)}"'

    if isinstance(expr, LiteralInt):
        return str(expr.value)

    if isinstance(expr, FieldRef):
        # A plain local scalar (e.g. {{WorkflowStepId}} tail of a RelativePath).
        # Return the bare var; if it feeds a CONCAT, the Concat case STR()-wraps
        # it. A standalone scalar lookup keeps its datatype.
        var = field_to_sparql_var(expr.name)
        state['locals'][expr.name] = var
        return var

    if isinstance(expr, FuncCall) and expr.name == 'INDEX':
        # INDEX(Target!{{ResultCol}}, MATCH({{LocalFK}}, Target!{{TargetPK}}, 0))
        if len(expr.args) != 2:
            raise ValueError("INDEX requires 2 arguments (result-column, MATCH)")
        result_ref = expr.args[0]
        match_call = expr.args[1]
        if not isinstance(result_ref, QualifiedRef):
            raise ValueError("INDEX first argument must be Table!{{Column}}")
        if not (isinstance(match_call, FuncCall) and match_call.name == 'MATCH'):
            raise ValueError("INDEX second argument must be MATCH(...)")
        if len(match_call.args) < 2:
            raise ValueError("MATCH requires (lookup-value, lookup-array, [exact])")
        local_key = match_call.args[0]
        if not isinstance(local_key, FieldRef):
            raise ValueError("MATCH lookup-value must be a local {{field}}")
        target_pk_ref = match_call.args[1]
        if not isinstance(target_pk_ref, QualifiedRef):
            raise ValueError("MATCH lookup-array must be Target!{{PK}}")

        # Resolve the target individual by MATCHING ITS PRIMARY-KEY VALUE, not by
        # dereferencing the local key as an object-property edge. This is the only
        # form that is correct for EVERY kind of FK:
        #
        #   - A relationship FK is emitted as an owl:ObjectProperty edge to the
        #     target node, but the node ALSO carries its PK as a datatype property
        #     (effortless-ntwf:<pkCol> "<pkValue>"), so PK-matching finds the same individual.
        #   - A *derived* FK — a lookup/calculated field whose value is a target
        #     PK STRING (e.g. GateDelegateRole = INDEX(Roles!{{DelegatesTo}}, ...))
        #     is NOT an edge at all; it is a plain literal. The old code emitted
        #     `$this effortless-ntwf:gateDelegateRole ?node` and bound ?node to the literal
        #     "ntwf-vp-engineering-role", then tried to read effortless-ntwf:filledByHumanAgent
        #     off a *string*, which matches nothing (or, under OWL-RL churn, every
        #     node) — the multi-valued garbage that broke chained lookups.
        #
        # Binding the local key as a SCALAR and joining it to the target's PK
        # property — anchored to the target class so the join can't wander to a
        # same-valued property on another table — resolves both cases identically.
        local_key_prop = field_to_property_uri(local_key.name)
        target_class = f'{NS}{target_pk_ref.table}'
        target_pk_prop = field_to_property_uri(target_pk_ref.column)
        result_prop = field_to_property_uri(result_ref.column)

        kvar = _next_join_var(state['counter'][0]); state['counter'][0] += 1
        tvar = _next_join_var(state['counter'][0]); state['counter'][0] += 1
        pkvar = _next_join_var(state['counter'][0]); state['counter'][0] += 1
        rvar = _next_join_var(state['counter'][0]); state['counter'][0] += 1
        # Two flat, sibling OPTIONALs — no nesting, clean variable scoping:
        #   1. bind the local FK value (a PK string, or an edge-bound IRI)
        #   2. find the target individual whose PK datatype property equals it,
        #      anchored to the target class, AND read the result column off it.
        # The result read must live in the same group as the PK match. As a third
        # sibling OPTIONAL it ran even when (2) matched nothing — a blank or
        # dangling FK — and bound the target variable to EVERY node carrying the
        # result property, returning every value in the graph where Postgres
        # returns NULL (and, with properties shared across tables, a value that
        # depended on which rule happened to run first).
        # Match on the trailing LOCAL NAME of the target's PK property, not its
        # raw STR(). When the target's PK column is the row's own id (ConceptId,
        # StatusId, …), the ABox promotes that value to a self IRI
        # (effortless-ntwf:verdict-at-risk) rather than a bare literal, so STR(pkvar) becomes
        # "https://w3id.org/effortless-ntwf#verdict-at-risk" while the local key is the plain
        # string "verdict-at-risk" — and STR()=STR() never matches. Stripping
        # everything up to the last '#'/'/' on the PK side makes the join hold
        # whether the PK arrives as an IRI or a literal, and the local key (which
        # is always a literal here) compares cleanly. This is the OWL analogue of
        # Postgres's string = string MATCH on the id column.
        state['triples'].append(f'{INDENT}OPTIONAL {{ $this {local_key_prop} {kvar} . }}')
        state['triples'].append(
            f'{INDENT}OPTIONAL {{ {tvar} a {target_class} ; {target_pk_prop} {pkvar} ; {result_prop} {rvar} . '
            f'FILTER(REPLACE(STR({pkvar}), "^.*[#/]", "") = REPLACE(STR({kvar}), "^.*[#/]", "")) }}')
        # Return the bare looked-up value, NOT STR(rvar). A standalone lookup is a
        # straight copy of one field, so it must keep the source datatype — a
        # copied boolean stays boolean, a copied number stays numeric — or
        # downstream rules that AND/compare it break (a stringified "false" is
        # truthy in SPARQL). When this value feeds a CONCAT, the Concat case
        # re-wraps it in STR() at the string-building boundary.
        return rvar

    raise ValueError(f"unsupported lookup sub-expression: {type(expr).__name__}")


def build_lookup_where(table_name: str, expr: ExprNode) -> str:
    """Build the WHERE body for a lookup field, binding ?_result via FK-edge
    traversal to the target individual."""
    state = {'triples': [], 'locals': {}, 'counter': [0]}
    value_expr = _compile_lookup_value(expr, state)

    parts = [f'    $this a {NS}{table_name} .']
    for field_name, var in sorted(state['locals'].items()):
        prop = field_to_property_uri(field_name)
        parts.append(f'{INDENT}OPTIONAL {{ $this {prop} {var} . }}')
    parts.extend(state['triples'])
    parts.append(f'{INDENT}BIND({value_expr} AS ?_result)')
    return '\n'.join(parts)


# A closure field (type == 'closure') materializes the transitive closure of a
# base owl:TransitiveProperty over an edge table. Postgres exposes that closure
# as the view vw_<edge_table_snake>_closure(from_id, to_id, is_inferred, ...);
# a rulebook COUNTIFS that rolls up the closure references that view by name.
# This registry lets the OWL aggregation generator recognize such a range and
# count the SAME pairs the view holds — directly over the reasoned transitive
# triples — so the count matches Postgres instead of returning 0. It is built
# once per generation from the schema (see generate_shacl_rules), keyed by the
# view name as it appears in formulas.
#   { "vw_step_precedence_closure": {
#        "base_prop": "effortless-ntwf:precedesStep",     # the transitive property
#        "edge_class": "StepPrecedence",        # asserted-edge junction class
#        "from_prop": "effortless-ntwf:fromStep",           # asserted edge's from-FK
#        "to_prop":   "effortless-ntwf:toStep" } }          # asserted edge's to-FK
CLOSURE_RELATIONS: Dict[str, Dict[str, str]] = {}


def _snake(name: str) -> str:
    """PascalCase -> snake_case, matching the Postgres transpiler's view naming."""
    s = re.sub(r'([a-z0-9])([A-Z])', r'\1_\2', name)
    s = re.sub(r'([A-Z]+)([A-Z][a-z])', r'\1_\2', s)
    return s.lower()


def register_closure_relations(tables: Dict[str, Any]) -> None:
    """Populate CLOSURE_RELATIONS from every closure-typed field so a COUNTIFS
    over its view name can be counted against the reasoned transitive triples."""
    CLOSURE_RELATIONS.clear()
    for tdef in tables.values():
        if not isinstance(tdef, dict):
            continue
        for col in tdef.get('schema', []):
            if col.get('type') != 'closure':
                continue
            edge_table = col.get('EdgeTable')
            from_col = col.get('FromColumn')
            to_col = col.get('ToColumn')
            if not (edge_table and from_col and to_col):
                # Self-edge closures (only ToColumn) also exist (Roles.DelegatesTo).
                # Their asserted edges live on the OWNING row, not a junction; we
                # register what we can and let the counter handle the junction case
                # for now (the only COUNTIFS-over-closure in use is the junction one).
                continue
            view_name = f'vw_{_snake(edge_table)}_closure'
            base_prop = closure_base_property(col, {})  # effortless-ntwf:precedesStep style
            CLOSURE_RELATIONS[view_name] = {
                'base_prop': base_prop,
                'edge_class': edge_table,
                'from_prop': field_to_property_uri(from_col),
                'to_prop': field_to_property_uri(to_col),
            }


def build_closure_count_where(table_name: str, expr: ExprNode, closure: Dict[str, str]) -> str:
    """Count transitive-closure pairs, filtered by is_inferred, to match the
    Postgres closure view. A pair (?a base+ ?b) is ASSERTED iff a junction edge
    individual exists with from=?a, to=?b; otherwise INFERRED. This is exactly
    the view's is_inferred semantics.

    The closure is computed with a SPARQL TRANSITIVE PROPERTY PATH (`base+`),
    NOT by relying on OWL-RL to pre-materialize the transitive edges. take-test
    runs pyshacl with inference='none' (full RDFS/OWL-RL inference mis-binds the
    multi-domain back-reference properties — see the injector's disjointness
    workaround), so `owl:TransitiveProperty` alone never expands. `base+` walks
    the reachability at query time, so the article's headline inference (4
    asserted edges -> 10 closure pairs, 6 inferred, incl. the never-asserted
    1->5) fires regardless of the reasoner's inference mode.

    The formula shape is COUNTIFS(<closureView>!{{IsInferred}}, TRUE/FALSE).
    """
    args = expr.args
    if len(args) != 2:
        raise ValueError("closure COUNTIFS supports exactly one (range, criteria) pair")
    range_ref = args[0]
    crit = args[1]

    base = closure['base_prop']
    edge_class = closure['edge_class']
    from_prop = closure['from_prop']
    to_prop = closure['to_prop']

    # ---- PER-ROW closure count: COUNTIFS(<view>!{{ToId|FromId}}, Owner!{{PK}}) ----
    # The criteria is a column reference to the CURRENT row's key ($this), not a
    # bool literal. We count closure pairs whose matched endpoint is $this:
    #   range = to_id   -> PREDECESSORS of $this :  COUNT WHERE ?a   base+ $this
    #   range = from_id -> SUCCESSORS  of $this :  COUNT WHERE $this base+ ?b
    # This mirrors the Postgres `COUNT(*) FROM <view> WHERE <endpoint> = <pk>` the
    # `closure` field type's view exposes, so the reasoner matches it. The reach is
    # the SPARQL transitive path `base+` (same primitive as the inferred/asserted
    # counts), so it fires under pyshacl inference='none'. ($this inside the
    # sub-SELECT is correlated, exactly as the child-rollup count does it.)
    if isinstance(crit, QualifiedRef):
        if not isinstance(range_ref, QualifiedRef):
            raise ValueError("closure per-row COUNTIFS range must be <view>!{{Column}}")
        col = _snake(range_ref.column)
        if col == 'to_id':
            path = f'{INDENT}        ?a {base}+ $this .'
        elif col == 'from_id':
            path = f'{INDENT}        $this {base}+ ?b .'
        else:
            raise ValueError(
                "closure per-row COUNTIFS range must be the view's from_id/to_id "
                f"column, got {range_ref.column!r}")
        parts = [
            f'    $this a {NS}{table_name} .',
            f'{INDENT}{{',
            f'{INDENT}    SELECT (COUNT(*) AS ?_result) WHERE {{',
            path,
            f'{INDENT}    }}',
            f'{INDENT}}}',
        ]
        return '\n'.join(parts)

    # ---- AGGREGATE closure count: COUNTIFS(<view>!{{IsInferred}}, TRUE/FALSE) ----
    # TRUE() parses as a no-arg FuncCall; accept both that and a bare bool literal.
    want_inferred = None
    if isinstance(crit, LiteralBool):
        want_inferred = crit.value
    elif isinstance(crit, FuncCall) and crit.name in ('TRUE', 'FALSE'):
        want_inferred = (crit.name == 'TRUE')
    else:
        raise ValueError("closure COUNTIFS criteria must be TRUE()/FALSE() or a per-row key ref")

    # asserted := an edge individual of edge_class links ?a -> ?b directly.
    asserted = (
        f'{INDENT}        EXISTS {{ ?e a {NS}{edge_class} ; {from_prop} ?a ; {to_prop} ?b . }}'
    )
    inferred_filter = (
        f'{INDENT}        FILTER(NOT {asserted.strip()})' if want_inferred
        else f'{INDENT}        FILTER({asserted.strip()})'
    )
    # `{base}+` is a SPARQL transitive property path: ?a reaches ?b in one or
    # more hops. This materializes the closure at query time (no OWL-RL needed).
    inner = (
        f'{INDENT}        ?a {base}+ ?b .\n'
        f'{inferred_filter}'
    )
    parts = [
        f'    $this a {NS}{table_name} .',
        f'{INDENT}{{',
        f'{INDENT}    SELECT (COUNT(*) AS ?_result) WHERE {{',
        inner,
        f'{INDENT}    }}',
        f'{INDENT}}}',
    ]
    return '\n'.join(parts)


AGGREGATE_FUNCS = {'COUNTIFS': 'COUNT', 'SUMIFS': 'SUM', 'MAXIFS': 'MAX', 'MINIFS': 'MIN'}

# table -> schema, so an aggregation can tell a relationship edge from a derived
# key and find the parent's PK. Populated by generate_shacl_rules().
TABLE_SCHEMAS: Dict[str, List[Dict[str, Any]]] = {}

# Per-rule compile context: a scalar formula can contain aggregations (e.g.
# DATETIME_DIFF({{AsOfInstant}}, MAXIFS(...), "days")); each is hoisted into a
# sub-SELECT block bound to its own variable.
_COMPILE_CTX: Dict[str, Any] = {'table': None, 'blocks': []}


def _key_text(var: str) -> str:
    """A key's comparable text: an IRI's local name, a literal's lexical form."""
    return f'IF(isIRI({var}), REPLACE(STR({var}), "^.*[#/]", ""), STR({var}))'


def _column(table: str, name: str) -> Optional[Dict[str, Any]]:
    return next((c for c in TABLE_SCHEMAS.get(table, []) if c.get('name') == name), None)


def aggregation_block(table_name: str, expr: FuncCall, result_var: str) -> List[str]:
    """Sub-SELECT lines binding result_var to COUNTIFS/SUMIFS/MAXIFS/MINIFS over a child table.

    Children match on every (Child!{{Col}}, criteria) pair, as in Postgres's WHERE:
      - criteria = the parent's PK and Col is a relationship: Col's edge points at $this.
      - criteria = any other parent field ({{Field}} or Parent!{{Field}}): Col's text
        equals that field's text on $this. A blank parent value matches nothing,
        as Postgres compares against NULLIF(value, '').
      - criteria = a literal: Col equals it.
    $this is pre-bound by SHACL-AF, so the sub-SELECT stays correlated with the row.
    """
    kind = AGGREGATE_FUNCS[expr.name]
    value_ref = None
    args = expr.args
    if kind != 'COUNT':
        if len(args) < 3:
            raise ValueError(f"{expr.name} requires a value range plus at least one (range, criteria) pair")
        value_ref, args = args[0], args[1:]
        if not isinstance(value_ref, QualifiedRef):
            raise ValueError(f"{expr.name} first argument must be Child!{{{{Column}}}}")
    if len(args) < 2 or len(args) % 2 != 0:
        raise ValueError("aggregation requires an even number of (range, criteria) args")
    if not isinstance(args[0], QualifiedRef):
        raise ValueError("aggregation first range must be Child!{{Column}}")
    child_table = args[0].table
    if value_ref is not None and value_ref.table != child_table:
        raise ValueError(f"{expr.name} value range and criteria ranges must name the same child table")

    parent_pk = get_pk_field(TABLE_SCHEMAS.get(table_name, []))
    triples = [f'{INDENT}    ?child a {NS}{child_table} .']
    filters = []
    for k in range(0, len(args), 2):
        rng, crit = args[k], args[k + 1]
        if not isinstance(rng, QualifiedRef) or rng.table != child_table:
            raise ValueError("every aggregation criteria range must be Child!{{Column}} on one child table")
        col_prop = field_to_property_uri(rng.column)
        cvar = f'?c{k}'
        if isinstance(crit, (FieldRef, QualifiedRef)):
            if isinstance(crit, QualifiedRef) and crit.table != table_name:
                raise ValueError(
                    f"criteria {crit.table}!{{{{{crit.column}}}}} must name a field of {table_name}")
            parent_col = crit.name if isinstance(crit, FieldRef) else crit.column
            child_col = _column(child_table, rng.column)
            if parent_col == parent_pk and child_col is not None and is_relationship(child_col):
                triples.append(f'{INDENT}    ?child {col_prop} $this .')
                continue
            pvar = f'?p{k}'
            triples.append(f'{INDENT}    $this {field_to_property_uri(parent_col)} {pvar} .')
            triples.append(f'{INDENT}    ?child {col_prop} {cvar} .')
            # Postgres reads a RAW parent field as NULLIF(value, ''), so a blank one
            # matches nothing; a derived key is compared as-is, so "" matches every
            # child whose key is also "".
            parent_field = _column(table_name, parent_col)
            blank_guard = (f'{_key_text(pvar)} != "" && '
                           if parent_field is None or parent_field.get('type', 'raw') == 'raw' else '')
            filters.append(
                f'{INDENT}    FILTER({blank_guard}{_key_text(cvar)} = {_key_text(pvar)})')
            continue
        triples.append(f'{INDENT}    OPTIONAL {{ ?child {col_prop} {cvar} . }}')
        if isinstance(crit, FuncCall) and crit.name in ('TRUE', 'FALSE') and not crit.args:
            crit = LiteralBool(crit.name == 'TRUE')
        if isinstance(crit, LiteralBool):
            want = 'true' if crit.value else 'false'
            filters.append(f'{INDENT}    FILTER(BOUND({cvar}) && {cvar} = {want})')
        elif isinstance(crit, LiteralString):
            filters.append(f'{INDENT}    FILTER(STR({cvar}) = "{escape_sparql_string(crit.value)}")')
        elif isinstance(crit, LiteralInt):
            filters.append(f'{INDENT}    FILTER({cvar} = {crit.value})')
        else:
            raise ValueError(f"unsupported {expr.name} criteria: {type(crit).__name__}")

    if kind == 'COUNT':
        agg = 'COUNT(DISTINCT ?child)'
    elif kind == 'SUM':
        # A child missing the value contributes 0, and no match sums to 0.
        triples.append(f'{INDENT}    OPTIONAL {{ ?child {field_to_property_uri(value_ref.column)} ?sumraw . }}')
        triples.append(f'{INDENT}    BIND(COALESCE(?sumraw, 0) AS ?sumval)')
        agg = 'SUM(?sumval)'
    else:
        # MAX/MIN over no matching child is unbound, as Postgres's MAX is NULL.
        triples.append(f'{INDENT}    ?child {field_to_property_uri(value_ref.column)} ?aggval .')
        agg = f'{kind}(?aggval)'

    return [
        f'{INDENT}{{',
        f'{INDENT}    SELECT ({agg} AS {result_var}) WHERE {{',
        *triples,
        *filters,
        f'{INDENT}    }}',
        f'{INDENT}}}',
    ]


def build_aggregation_where(table_name: str, expr: ExprNode) -> str:
    """WHERE body for an aggregation field.

    A COUNTIFS whose first range names a CLOSURE relation (a closure-typed field's
    view, e.g. vw_step_precedence_closure) counts pairs of the reasoned transitive
    closure, not child individuals, so it is routed to build_closure_count_where.
    A bare aggregate becomes one sub-SELECT; an aggregate inside a scalar formula
    is hoisted by the scalar compiler."""
    if (isinstance(expr, FuncCall) and expr.name == 'COUNTIFS' and expr.args
            and isinstance(expr.args[0], QualifiedRef) and expr.args[0].table in CLOSURE_RELATIONS):
        return build_closure_count_where(table_name, expr, CLOSURE_RELATIONS[expr.args[0].table])
    if isinstance(expr, FuncCall) and expr.name in AGGREGATE_FUNCS:
        return '\n'.join([f'    $this a {NS}{table_name} .'] + aggregation_block(table_name, expr, '?_result'))
    return build_scalar_where(table_name, expr, 'aggregation')


def build_scalar_where(table_name: str, expr: ExprNode, calc_type: str, datatype: str = 'string') -> str:
    field_bindings: Dict[str, str] = {}
    _COMPILE_CTX.update(table=table_name, blocks=[])
    try:
        sparql_expr = compile_to_sparql(expr, field_bindings)
        blocks = _COMPILE_CTX['blocks']
    finally:
        _COMPILE_CTX.update(table=None, blocks=[])
    where_parts = [f'    $this a {NS}{table_name} .']
    for field_name, var_name in sorted(field_bindings.items()):
        where_parts.append(f'    OPTIONAL {{ $this {field_to_property_uri(field_name)} {var_name} . }}')
    for block in blocks:
        where_parts.extend(block)
    # A CALCULATED RELATIONSHIP — a formula-bearing field whose type is
    # `relationship` — computes a foreign-key PK string (e.g.
    # IF(flag,'verdict-at-risk','verdict-ok')). The CONSTRUCT must assert a real
    # effortless-ntwf: edge to that individual, not a string literal, so the
    # computed PK is coerced into the ontology IRI. This is the one place a scalar
    # BIND becomes an object-property edge.
    if calc_type == 'relationship':
        sparql_expr = f'IRI(CONCAT("{ONT_NS}", STR({sparql_expr})))'
    elif (datatype or '').lower() == 'integer':
        # Postgres casts an integer field's value with ::integer, which rounds.
        sparql_expr = f'IF(isNumeric({sparql_expr}), ROUND({sparql_expr}), {sparql_expr})'
    where_parts.append(f'                BIND({sparql_expr} AS ?_result)')
    return '\n'.join(where_parts)


def formula_dependency_depths(tables: Dict[str, Any]) -> Dict[Tuple[str, str], int]:
    """(table, field) -> depth in the formula DAG, emitted as sh:order.

    SHACL-AF executes rules in ascending sh:order, so a rule runs only after every
    rule that produces a value it reads. Without it an engine must re-run all
    rules until nothing changes, and a rule that fired early on a half-computed
    input leaves a stale value beside the converged one."""
    formulas = {}
    for tname, tdef in tables.items():
        if tname.startswith('_') or tname.startswith('$') or not isinstance(tdef, dict):
            continue
        for col in tdef.get('schema', []):
            if col.get('formula'):
                formulas[(tname, col['name'])] = col['formula']

    deps = {}
    for (tname, fname), formula in formulas.items():
        refs = set()
        for m in re.finditer(r'(?:(\w+)!)?\{\{(\w+)\}\}', formula):
            ref = (m.group(1) or tname, m.group(2))
            if ref in formulas and ref != (tname, fname):
                refs.add(ref)
        deps[(tname, fname)] = refs

    depths: Dict[Tuple[str, str], int] = {}
    visiting = set()

    def depth(key):
        if key in depths:
            return depths[key]
        if key in visiting:
            raise ValueError(f"formula dependency cycle through {key[0]}.{key[1]}")
        visiting.add(key)
        d = 1 + max((depth(r) for r in deps[key]), default=-1)
        visiting.discard(key)
        depths[key] = d
        return d

    for key in formulas:
        depth(key)
    return depths


def generate_shacl_rules(tables: Dict[str, Any]) -> str:
    """Generate SHACL-SPARQL rules from formulas."""
    # Register closure relations FIRST so a COUNTIFS over a closure view (e.g.
    # vw_step_precedence_closure) is recognized and counted against the reasoned
    # transitive triples rather than mis-generated as a child-rollup that finds 0.
    register_closure_relations(tables)
    rule_depths = formula_dependency_depths(tables)
    TABLE_SCHEMAS.clear()
    TABLE_SCHEMAS.update({t: d.get('schema', []) for t, d in tables.items() if isinstance(d, dict)})

    lines = []

    # Prefixes
    lines.append('@prefix rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#> .')
    lines.append('@prefix rdfs: <http://www.w3.org/2000/01/rdf-schema#> .')
    lines.append('@prefix sh: <http://www.w3.org/ns/shacl#> .')
    lines.append('@prefix xsd: <http://www.w3.org/2001/XMLSchema#> .')
    lines.append(prefix_decl())
    lines.append('')

    rule_count = 0

    for table_name, table_def in sorted(tables.items()):
        if table_name.startswith('_') or table_name.startswith('$'):
            continue

        schema = table_def.get('schema', [])
        if not schema:
            continue

        class_uri = f'{NS}{table_name}'

        # Collect calculated fields with formulas
        calc_fields = []
        for col in schema:
            formula = col.get('formula')
            if formula:
                calc_fields.append({
                    'name': col.get('name', ''),
                    'datatype': col.get('datatype', 'string'),
                    'type': col.get('type', 'calculated'),
                    'formula': formula
                })

        if not calc_fields:
            continue

        # Generate NodeShape with rules
        shape_uri = f'{NS}{table_name}Shape'
        lines.append(f'# === Shape with rules for {table_name} ===')
        lines.append(f'{shape_uri} a sh:NodeShape ;')
        lines.append(f'    sh:targetClass {class_uri}')  # No trailing semicolon yet

        # Track successful rules
        rules_added = []
        parse_errors = []

        # Generate a rule for each calculated field
        for calc in calc_fields:
            rule_name = f'rule_{table_name}_{calc["name"]}'

            try:
                expr = parse_formula(calc['formula'])

                # Lookups (INDEX/MATCH) and aggregations (COUNTIFS) are cross-
                # individual joins, not scalar BINDs on the current row. They get
                # dedicated generators that walk ObjectProperty edges / count
                # child individuals. Everything else compiles through the scalar
                # SPARQL path.
                if calc['type'] == 'lookup':
                    where_clause = build_lookup_where(table_name, expr)
                elif calc['type'] == 'aggregation':
                    where_clause = build_aggregation_where(table_name, expr)
                else:
                    where_clause = build_scalar_where(table_name, expr, calc['type'], calc['datatype'])

                # Target property
                target_prop = field_to_property_uri(calc['name'])

                rule_lines = []
                rule_lines.append(f'    sh:rule [')
                rule_lines.append(f'        a sh:SPARQLRule ;')
                rule_lines.append(f'        rdfs:label "{rule_name}" ;')
                rule_lines.append(f'        sh:order {rule_depths[(table_name, calc["name"])]} ;')
                rule_lines.append(f'        sh:prefixes {NS} ;')
                rule_lines.append(f'        sh:construct """')
                rule_lines.append(f'            PREFIX {ONT_PREFIX}: <{ONT_NS}>')
                rule_lines.append(f'            PREFIX xsd: <http://www.w3.org/2001/XMLSchema#>')
                rule_lines.append(f'            CONSTRUCT {{')
                rule_lines.append(f'                $this {target_prop} ?_result .')
                rule_lines.append(f'            }}')
                rule_lines.append(f'            WHERE {{')
                rule_lines.append(f'{where_clause}')
                rule_lines.append(f'            }}')
                rule_lines.append(f'        """ ;')
                rule_lines.append(f'    ]')

                rules_added.append(rule_lines)
                rule_count += 1

            except Exception as e:
                # Formula parsing failed — surface this loudly. Previously we
                # silently swallowed every error as a TTL comment, which let
                # the OWL substrate report 100% while 13 of 14 fields were
                # actually pre-baked into individuals.ttl as if they were raw
                # data. The audit on 2026-04-23 caught that. Now we (a) write
                # the comment as before (so the failure is visible in the
                # artifact) AND (b) emit a noisy stderr line so CI shows it.
                err_msg = f'# Rule for {calc["name"]} - parse error: {e}'
                parse_errors.append(err_msg)
                print(
                    f"OWL inject: PARSE ERROR for {table_name}.{calc['name']}: {e}",
                    file=sys.stderr,
                )

        # Now build the shape properly
        if rules_added:
            # Add semicolon to targetClass line and add rules
            lines[-1] += ' ;'
            for i, rule_lines in enumerate(rules_added):
                # Add semicolon if not last rule
                if i < len(rules_added) - 1:
                    rule_lines[-1] += ' ;'
                lines.extend(rule_lines)

            # Add parse errors as comments (but outside the shape)
            lines[-1] += ' .'
            for err in parse_errors:
                lines.append(err)
        else:
            # No rules succeeded - close shape immediately
            lines[-1] += ' .'
            for err in parse_errors:
                lines.append(err)

        lines.append('')

    # ---- Cardinality constraints for single-valued forward FKs ----
    # A forward FK holds exactly one target (sh:maxCount 1). This is the
    # constraint form of single-valuedness — it VALIDATES cardinality without the
    # individual-merging side effect that owl:FunctionalProperty would cause
    # under a reasoner (see note in generate_ontology_owl Pass 2). Polymorphic
    # FK arms (FilledByHumanAgent / ...) are each maxCount 1; the "exactly one
    # arm total" rule is a separate SPARQL constraint left to the witness fields.
    single_valued = discover_single_valued_fks(tables)
    if single_valued:
        lines.append('# === Cardinality: single-valued forward FK properties ===')
        for table_name in sorted(single_valued):
            props = single_valued[table_name]
            if not props:
                continue
            class_uri = f'{NS}{table_name}'
            lines.append(f'{NS}{table_name}CardinalityShape a sh:NodeShape ;')
            lines.append(f'    sh:targetClass {class_uri} ;')
            for i, prop_uri in enumerate(sorted(props)):
                terminator = ' ;' if i < len(props) - 1 else ' .'
                lines.append(f'    sh:property [ sh:path {prop_uri} ; sh:maxCount 1 ]{terminator}')
            lines.append('')

    lines.append(f'# Generated {rule_count} SHACL rules')

    return '\n'.join(lines)


def discover_single_valued_fks(tables: Dict[str, Any]) -> Dict[str, set]:
    """Map table -> {property URIs} for forward (single-valued) FKs.

    A forward FK is a relationship field that is NOT the reverse (multi-valued)
    side of an inverse pair, and is NOT a closure base property (those are
    transitive, inherently multi-valued). These get sh:maxCount 1.
    """
    inverse_pairs = find_inverse_pairs(tables)
    reverse_sides = set(inverse_pairs.values())
    # Closure base props (transitive) must be excluded from maxCount 1.
    closure_bases = set()
    for tname, tdef in tables.items():
        if tname.startswith('_') or tname.startswith('$'):
            continue
        for col in tdef.get('schema', []):
            if col.get('type') == 'closure':
                base = closure_base_property(col, {})
                if base:
                    closure_bases.add(base)

    out: Dict[str, set] = {}
    for tname, tdef in tables.items():
        if tname.startswith('_') or tname.startswith('$'):
            continue
        for col in tdef.get('schema', []):
            if is_relationship(col):
                key = f'{tname}.{col["name"]}'
                prop_uri = field_to_property_uri(col['name'])
                if key in reverse_sides or prop_uri in closure_bases:
                    continue
                out.setdefault(tname, set()).add(prop_uri)
    return out


# =============================================================================
# MAIN
# =============================================================================

def main():
    # Define generated files for this substrate
    GENERATED_FILES = [
        'src/ontology.owl',
        'src/individuals.ttl',
        'src/rules.shacl.ttl',
        'test-answers.json',
        'test-results.md',
    ]

    # Handle --clean argument
    if handle_clean_arg(GENERATED_FILES, "OWL substrate: Removes generated ontology, individuals, and SHACL rules"):
        return

    env_output = os.environ.get("ERB_OUTPUT_DIR")
    script_dir = Path(env_output).resolve() if env_output else Path(__file__).resolve().parent

    # The three ontology artifacts (TBox / ABox / SHACL) are the *source* of the
    # OWL project and live under src/. The conformance harness files
    # (test-answers.json / test-results.md) stay at the substrate root where the
    # grader expects them. src/ is created if missing so a fresh build works.
    src_dir = script_dir / "src"
    src_dir.mkdir(parents=True, exist_ok=True)

    print("=" * 70)
    print("OWL Execution Substrate - Formula-to-SHACL Compiler")
    print("=" * 70)
    print()

    # Load the rulebook
    print("Loading rulebook...")
    try:
        rulebook = load_rulebook()
    except FileNotFoundError as e:
        print(f"ERROR: {e}")
        sys.exit(1)

    # Filter to just table definitions (exclude metadata keys)
    tables = {k: v for k, v in rulebook.items()
              if isinstance(v, dict) and 'schema' in v}

    print(f"Found {len(tables)} tables: {', '.join(tables.keys())}")
    register_pk_collisions(tables)
    if COLLIDING_PK_SLUGS:
        print(f"Table-scoping IRIs for PK values shared across tables: {', '.join(sorted(COLLIDING_PK_SLUGS))}")

    # Count calculated fields
    total_calc = 0
    for table_name, table_def in tables.items():
        for col in table_def.get('schema', []):
            if col.get('formula'):
                total_calc += 1
    print(f"Found {total_calc} calculated fields to compile")

    print("\n" + "-" * 70)

    # Generate ontology.owl (TBox)
    print("\nGenerating src/ontology.owl (TBox - schema)...")
    ontology_content = generate_ontology_owl(tables)
    ontology_path = src_dir / "ontology.owl"
    ontology_path.write_text(ontology_content, encoding='utf-8')
    print(f"   Wrote: {ontology_path} ({len(ontology_content)} bytes)")

    # Generate individuals.ttl (ABox)
    print("\nGenerating src/individuals.ttl (ABox - data)...")
    individuals_content = generate_individuals_ttl(tables)
    individuals_path = src_dir / "individuals.ttl"
    individuals_path.write_text(individuals_content, encoding='utf-8')
    print(f"   Wrote: {individuals_path} ({len(individuals_content)} bytes)")

    # Generate rules.shacl.ttl
    print("\nGenerating src/rules.shacl.ttl (SHACL-SPARQL rules)...")
    rules_content = generate_shacl_rules(tables)
    rules_path = src_dir / "rules.shacl.ttl"
    rules_path.write_text(rules_content, encoding='utf-8')
    print(f"   Wrote: {rules_path} ({len(rules_content)} bytes)")

    print("\n" + "=" * 70)
    print("Generation complete!")
    print("=" * 70)


if __name__ == "__main__":
    main()
