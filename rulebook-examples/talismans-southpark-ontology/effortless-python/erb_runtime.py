"""
Formula runtime for the Python substrate.

HAND-WRITTEN, NOT GENERATED. erb_sdk.py compiles every formula to calls on this
module, and describes every lookup and aggregation as data this module runs, so
there is one implementation of the dialect's semantics in Python. It ships beside
the generated SDK the way erb_runtime.go ships beside the Go SDK.

This module parses no formula and reads no rulebook. Everything it computes was
compiled into erb_sdk.py by rulebook-to-python; everything it reads is the
dataset under $ERB_TESTING_DIR/blank-tests. Cross-table values come from this
run's own rows, never from another substrate's answers.

SEMANTICS are the ERB build parameters' (docs/ERB-BUILD-PARAMETERS.md in
Versioned-Stable-SSoTme-Tools): erb_sdk.py calls configure() with the set it
was generated under, and every helper below reads that set — how a blank
behaves in a predicate (erbBlankLogic), how DATETIME_DIFF counts
(erbDateDiff), the zone calendar dates live in (erbTimezone), how a datetime
renders inside text (erbDateTimeText) and whether a whole value is an integer
(erbWholeNumber). Independent of those: a raw text column reads through
NULLIF(x, ''), a blank is 0 in arithmetic and '' in CONCAT. erb_runtime.go and
erb_runtime.ts are line-for-line counterparts; when a rule changes here it
changes there.
"""

import datetime
import json
import math
import os
import re
import sys
from decimal import Decimal, ROUND_CEILING, ROUND_FLOOR, ROUND_HALF_UP
from pathlib import Path


# =============================================================================
# BUILD PARAMETERS — erb_sdk.py calls configure() before anything else runs
# =============================================================================

_PARAMETER_VALUES = {
    'erbDateDiff': ('calendar', 'elapsed'),
    'erbTimezone': ('UTC',),          # or any IANA zone name
    'erbDateTimeText': ('iso8601', 'sql'),
    'erbBlankLogic': ('coerce', 'propagate'),
    'erbWholeNumber': ('by-field-type', 'integer', 'decimal'),
}

_PARAMS = None
_ZONE = None


def configure(params):
    """Install the build parameters the SDK was generated with. Every name must
    be known and every value allowed; the zone must resolve. Called once, at
    the top of the generated erb_sdk.py."""
    global _PARAMS, _ZONE
    unknown = sorted(set(params) - set(_PARAMETER_VALUES))
    missing = sorted(set(_PARAMETER_VALUES) - set(params))
    if unknown or missing:
        raise ValueError(
            f"erb_runtime.configure: unknown parameters {unknown}, missing {missing}; "
            f"the contract defines {sorted(_PARAMETER_VALUES)}")
    for name, allowed in _PARAMETER_VALUES.items():
        if name != 'erbTimezone' and params[name] not in allowed:
            raise ValueError(
                f"erb_runtime.configure: {name}={params[name]!r} is not one of {list(allowed)}")
    zone_name = params['erbTimezone']
    if zone_name == 'UTC':
        zone = datetime.timezone.utc
    else:
        import zoneinfo
        try:
            zone = zoneinfo.ZoneInfo(zone_name)
        except Exception as exc:
            raise ValueError(
                f"erb_runtime.configure: erbTimezone={zone_name!r} cannot be resolved "
                f"on this machine ({exc}); install tzdata or use UTC") from exc
    _PARAMS = dict(params)
    _ZONE = zone


def _p(name):
    if _PARAMS is None:
        raise RuntimeError(
            "erb_runtime is not configured: erb_sdk.py must call erb_runtime.configure() "
            "with the ERB build parameters before any formula runs")
    return _PARAMS[name]


def _zone():
    _p('erbTimezone')
    return _ZONE


def parameters():
    """The configured set, for anything that records provenance."""
    return dict(_PARAMS) if _PARAMS is not None else None


# =============================================================================
# SCALAR HELPERS — every compiled formula calls these
# =============================================================================

def _to_number(value):
    """A number, a numeric string's number, or None. Booleans are not numbers."""
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
    """Arithmetic operand: blank or non-numeric is 0, as the oracle's safe cast
    plus COALESCE(..., 0) computes it."""
    number = _to_number(value)
    return 0 if number is None else number


def _compare_operands(left, right):
    """A comparable (left, right) pair, or None when the two cannot be ordered."""
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


_TRUE_SPELLINGS = ('t', 'true', 'y', 'yes', 'on', '1')
_FALSE_SPELLINGS = ('f', 'false', 'n', 'no', 'off', '0')


def erb_bool3(value):
    """A three-valued boolean: TRUE, FALSE or NULL. Text is read the way the
    oracle's non-raising boolean cast reads it — Postgres's spellings ('yes',
    'on', 't', '1', ...) are TRUE or FALSE, any other text is NULL. Anything
    else reads as FALSE."""
    if value is None or isinstance(value, bool):
        return value
    if isinstance(value, str):
        spelling = value.strip().lower()
        if spelling in _TRUE_SPELLINGS:
            return True
        if spelling in _FALSE_SPELLINGS:
            return False
        return None
    return False


def _coerce_blanks():
    return _p('erbBlankLogic') == 'coerce'


def erb_and(*values):
    """AND over three-valued operands. erbBlankLogic=coerce: a blank is FALSE.
    propagate: FALSE if any operand is FALSE, else NULL if any is NULL, else TRUE."""
    if any(v is False for v in values):
        return False
    if any(v is None for v in values):
        return False if _coerce_blanks() else None
    return True


def erb_or(*values):
    """OR over three-valued operands. erbBlankLogic=coerce: a blank is FALSE.
    propagate: TRUE if any operand is TRUE, else NULL if any is NULL, else FALSE."""
    if any(v is True for v in values):
        return True
    if any(v is None for v in values):
        return False if _coerce_blanks() else None
    return False


def erb_not(value):
    """NOT. erbBlankLogic=coerce: NOT(blank) is TRUE. propagate: NOT NULL is NULL."""
    if value is None:
        return True if _coerce_blanks() else None
    return not value


def erb_nullif(value):
    """NULLIF(x, ''): the oracle stores a blank raw text value as NULL."""
    return None if value == '' else value


def erb_blank(value) -> bool:
    """Blank means absent: NULL or the empty string."""
    return value is None or value == ''


def _blank_as_zero_of(other):
    """erbBlankLogic=coerce: a blank compared against `other` takes the other
    side's zero — 0 against a number, FALSE against a boolean, '' otherwise."""
    if isinstance(other, bool):
        return False
    if _to_number(other) is not None:
        return 0
    return ''


def _coerced_pair(a, b):
    """(a, b) with a blank side replaced by the other side's zero, or None when
    the blank must propagate. Two blanks coerce to ('', '')."""
    if a is None and b is None:
        return ('', '') if _coerce_blanks() else None
    if a is None or b is None:
        if not _coerce_blanks():
            return None
        return (_blank_as_zero_of(b), b) if a is None else (a, _blank_as_zero_of(a))
    return a, b


def erb_eq(a, b):
    """`=`. coerce: a blank equals '', 0, FALSE and another blank. propagate: NULL
    when either side is NULL."""
    pair = _coerced_pair(a, b)
    if pair is None:
        return None
    a, b = pair
    ordered = _compare_operands(a, b)
    if ordered is not None:
        return ordered[0] == ordered[1]
    return a == b


def erb_ne(a, b):
    """`<>`: the negation of erb_eq, NULL when that is NULL."""
    equal = erb_eq(a, b)
    return None if equal is None else (not equal)


def erb_cmp(a, op, b):
    """Ordered comparison: numbers and numeric strings as numbers, two strings as
    strings; operands that cannot be ordered compare FALSE. A blank operand is
    NULL under propagate and the other side's zero under coerce."""
    pair = _coerced_pair(a, b)
    if pair is None:
        return None
    a, b = pair
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


def erb_find(needle, haystack, start=None):
    """FIND as POSITION(needle IN haystack): 1-based, 0 when absent, NULL on NULL.
    With a start above 1 it is POSITION(needle IN SUBSTRING(haystack FROM start))
    + start - 1, exactly as the oracle emits it — so an absent needle reads
    start - 1, not 0."""
    if needle is None or haystack is None:
        return None
    if start is None:
        return str(haystack).find(str(needle)) + 1
    if _to_number(start) is None:
        return None
    start = int(_to_number(start))
    if start > 1:
        return str(haystack)[start - 1:].find(str(needle)) + 1 + start - 1
    return str(haystack).find(str(needle)) + 1


def erb_round(value, digits=0):
    """ROUND half away from zero, as SQL ROUND does. NULL propagates."""
    if value is None or value == '':
        return None
    d = int(digits) if digits is not None and digits != '' else 0
    quant = Decimal(1).scaleb(-d)
    return float(Decimal(str(value)).quantize(quant, rounding=ROUND_HALF_UP))


def erb_roundup(value, digits=0):
    """ROUNDUP(x, d) as the oracle's CEIL(x * 10^d) / 10^d. NULL propagates."""
    if value is None or value == '':
        return None
    d = int(digits) if digits is not None and digits != '' else 0
    quant = Decimal(1).scaleb(-d)
    return float(Decimal(str(value)).quantize(quant, rounding=ROUND_CEILING))


def erb_integer(value):
    """A value on a field declared integer: half away from zero, as ::integer casts."""
    if value is None or isinstance(value, bool):
        return value
    number = _to_number(value)
    if number is None:
        return value
    return int(Decimal(str(number)).quantize(Decimal(1), rounding=ROUND_HALF_UP))


def erb_number(value, datatype):
    """Type a computed numeric value for output per erbWholeNumber. by-field-type:
    an `integer` field holds an int, a `number`/`decimal` field a float (so a
    whole value serializes as 2.0). integer: any whole value is an int. decimal:
    every numeric value is a float. Non-numeric values pass through."""
    if value is None or isinstance(value, bool):
        return value
    number = _to_number(value) if isinstance(value, str) else value
    if not isinstance(number, (int, float)):
        return value
    mode = _p('erbWholeNumber')
    if mode == 'decimal':
        return float(number)
    if mode == 'integer':
        return int(number) if float(number).is_integer() else float(number)
    kind = (datatype or '').lower()
    if kind in ('integer', 'int', 'long', 'bigint'):
        return erb_integer(number)
    if kind in ('number', 'decimal', 'numeric', 'float', 'double', 'currency', 'percent', 'percentage'):
        return float(number)
    return value


def erb_text(value):
    """A non-datetime CONCAT / `&` operand as text: a blank contributes nothing, a
    number renders in its shortest exact form with no trailing .0 (2, 2.5), a
    boolean as true/false."""
    if value is None or value == '':
        return ''
    if isinstance(value, bool):
        return 'true' if value else 'false'
    if isinstance(value, float):
        if value.is_integer():
            return str(int(value))
        return repr(value)
    if isinstance(value, (datetime.datetime, datetime.date)):
        return erb_datetime_text(value)
    return str(value)


def _as_datetime(value):
    """A timezone-aware datetime for a datetime, a date, or ISO-8601 text; a value
    with no offset is taken to be in erbTimezone."""
    if isinstance(value, datetime.datetime):
        dt = value
    elif isinstance(value, datetime.date):
        dt = datetime.datetime.combine(value, datetime.time())
    elif isinstance(value, str):
        text = value.strip().replace('Z', '+00:00')
        try:
            dt = datetime.datetime.fromisoformat(text)
        except ValueError:
            dt = datetime.datetime.strptime(text[:10], '%Y-%m-%d')
    else:
        raise TypeError(f"cannot read {value!r} as a datetime")
    if dt.tzinfo is None:
        dt = dt.replace(tzinfo=_zone())
    return dt


def _in_zone(value):
    """The instant `value` names, expressed in erbTimezone."""
    return _as_datetime(value).astimezone(_zone())


def _is_date_only(value):
    return isinstance(value, datetime.date) and not isinstance(value, datetime.datetime) \
        or (isinstance(value, str) and re.fullmatch(r'\d{4}-\d{2}-\d{2}', value.strip() or 'x') is not None)


def erb_datetime_text(value):
    """A datetime rendered into text per erbDateTimeText, in erbTimezone:
    iso8601 = 2026-04-03T14:00:00+00:00, sql = 2026-04-03 14:00:00+00. A
    date-only value renders as YYYY-MM-DD; a blank as ''."""
    if value is None or value == '':
        return ''
    if _is_date_only(value):
        return str(value).strip()[:10] if isinstance(value, str) else value.isoformat()
    dt = _in_zone(value)
    style = _p('erbDateTimeText')
    text = dt.strftime('%Y-%m-%dT%H:%M:%S' if style == 'iso8601' else '%Y-%m-%d %H:%M:%S')
    if dt.microsecond:
        text += ('.%06d' % dt.microsecond).rstrip('0')
    offset_minutes = int(dt.utcoffset().total_seconds() // 60)
    sign = '-' if offset_minutes < 0 else '+'
    hours, minutes = divmod(abs(offset_minutes), 60)
    if style == 'iso8601':
        text += f'{sign}{hours:02d}:{minutes:02d}'
    else:
        text += f'{sign}{hours:02d}' + (f':{minutes:02d}' if minutes else '')
    return text


def erb_timestamp_text(value):
    """A value of a datetime-declared field rendered into text: always a full
    timestamp per erbDateTimeText, even when it was written as a bare date
    (the column stores that date's midnight in erbTimezone)."""
    if value is None or value == '':
        return ''
    return erb_datetime_text(_in_zone(value))


def erb_json_value(value):
    """A computed value as it is written into JSON: a datetime per erbDateTimeText;
    anything json.dump cannot write is an error, not a str() guess."""
    if isinstance(value, (datetime.datetime, datetime.date)):
        return erb_datetime_text(value)
    raise TypeError(f"a computed value of type {type(value).__name__} cannot be written as JSON: {value!r}")


def erb_date_or_none(value):
    """A naive datetime for text that starts YYYY-MM-DD, else None — the test that
    decides whether `+`/`-` is date arithmetic."""
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


def erb_add(a, b):
    """`+`: numeric addition, except date-plus-days."""
    a_dt, b_dt = erb_date_or_none(a), erb_date_or_none(b)
    if a_dt is not None and b_dt is None:
        return a_dt + _erb_days_delta(b)
    if b_dt is not None and a_dt is None:
        return b_dt + _erb_days_delta(a)
    return _num_or_zero(a) + _num_or_zero(b)


def erb_sub(a, b):
    """`-`: numeric subtraction, except date-minus-days."""
    a_dt = erb_date_or_none(a)
    if a_dt is not None and erb_date_or_none(b) is None:
        return a_dt - _erb_days_delta(b)
    return _num_or_zero(a) - _num_or_zero(b)


def erb_mul(a, b):
    """`*`: numeric multiplication (never string repetition)."""
    return _num_or_zero(a) * _num_or_zero(b)


def erb_div(a, b):
    """`/`: numeric division; a zero divisor is NULL, as NULLIF(divisor, 0) is."""
    divisor = _num_or_zero(b)
    if divisor == 0:
        return None
    return _num_or_zero(a) / divisor


def erb_right(text, count) -> str:
    """RIGHT(text, count): the last `count` characters."""
    s = text or ''
    n = count or 0
    if n <= 0:
        return ''
    return s[-n:]


def erb_mid(text, start, count) -> str:
    """MID(text, start, count): `count` characters from 1-based `start`."""
    s = text or ''
    n = count or 0
    start_idx = max(int(start or 1) - 1, 0)
    if n <= 0:
        return ''
    return s[start_idx:start_idx + n]


def erb_coalesce(*values):
    """COALESCE: the first argument that is not NULL or blank."""
    for v in values:
        if not erb_blank(v):
            return v
    return None


def erb_try(fn, fallback_fn):
    """IFERROR(expr, fallback)."""
    try:
        return fn()
    except Exception:
        return fallback_fn()


def erb_is_error(fn) -> bool:
    """ISERROR(expr)."""
    try:
        fn()
        return False
    except Exception:
        return True


_ERB_RUN_NOW = None


def erb_now():
    """NOW()/TODAY(), an aware datetime in erbTimezone. FORMULA_NOW (ISO-8601)
    pins the clock; without it, the clock is read ONCE per run and every later
    call returns that same instant.

    A run is one instant. erb_settle repeats passes until nothing changes, so
    a NOW() that ticked between passes would change every pass and a dataset
    with any NOW()/TODAY() field could never settle (it failed as "the field
    dependencies are cyclic"). Reading the clock once also keeps two fields
    that both say NOW() consistent with each other within a run."""
    global _ERB_RUN_NOW
    override = os.environ.get('FORMULA_NOW')
    if override:
        return _in_zone(override)
    if _ERB_RUN_NOW is None:
        _ERB_RUN_NOW = datetime.datetime.now(datetime.timezone.utc).astimezone(_zone())
    return _ERB_RUN_NOW


def _trunc(quotient):
    """Integer division truncated toward zero, as SQL and Go integer division are."""
    return int(quotient) if quotient >= 0 else -int(-quotient)


def erb_datetime_diff(end, start, unit='day'):
    """DATETIME_DIFF(end, start, unit), positive when end is after start, per
    erbDateDiff. hour/minute/second are exact elapsed (possibly fractional) in
    both modes; month/year are whole calendar months (Postgres AGE()) in both.
    calendar: day = calendar dates in erbTimezone subtracted, week = day/7
    truncated. elapsed: day = seconds/86400 truncated, week = seconds/604800
    truncated."""
    unit = str(unit or 'day').lower().rstrip('s')
    if end is None or start is None or end == '' or start == '':
        return None
    end_dt = _in_zone(end)
    start_dt = _in_zone(start)
    seconds = (end_dt - start_dt).total_seconds()
    calendar = _p('erbDateDiff') == 'calendar'

    if unit == 'hour':
        return seconds / 3600
    if unit == 'minute':
        return seconds / 60
    if unit == 'second':
        return seconds
    if unit == 'day':
        if calendar:
            return (end_dt.date() - start_dt.date()).days
        return _trunc(seconds / 86400)
    if unit == 'week':
        if calendar:
            return _trunc((end_dt.date() - start_dt.date()).days / 7)
        return _trunc(seconds / 604800)
    if unit in ('month', 'year'):
        if end_dt < start_dt:
            # AGE() is antisymmetric: a negative span is the forward span, negated.
            return -erb_datetime_diff(start, end, unit)
        months = (end_dt.year - start_dt.year) * 12 + (end_dt.month - start_dt.month)
        if (end_dt.day, end_dt.hour, end_dt.minute, end_dt.second) \
                < (start_dt.day, start_dt.hour, start_dt.minute, start_dt.second):
            months -= 1
        return months if unit == 'month' else _trunc(months / 12)
    raise ValueError(f"DATETIME_DIFF: unsupported unit {unit!r}")


# =============================================================================
# THE REST OF THE ORACLE'S FUNCTION SET — each helper reproduces the SQL
# rulebook-to-postgres (FormulaTranslator.cs) emits for the function, named in
# its docstring. NULL in is NULL out unless the docstring says otherwise, and
# an input the SQL cast would reject raises, as the SQL does.
# =============================================================================

def _numeric(value, fn):
    """(value)::NUMERIC: None for NULL, else a Decimal; text that is not a
    number raises, as the cast does."""
    if value is None:
        return None
    if isinstance(value, bool):
        raise ValueError(f"{fn}: cannot cast boolean {value!r} to numeric")
    if isinstance(value, (int, float, Decimal)):
        return Decimal(str(value))
    text = str(value).strip()
    try:
        return Decimal(text)
    except Exception:
        raise ValueError(f"{fn}: invalid input syntax for type numeric: {value!r}") from None


def _out(number):
    """A Decimal result as the int or float the rest of the runtime computes with."""
    if number is None:
        return None
    return int(number) if number == number.to_integral_value() else float(number)


def _sql_text(value):
    """(value)::text for an operand of SWITCH."""
    if value is None:
        return None
    return erb_text(value)


def erb_trim(text):
    """TRIM(text): leading and trailing spaces removed (spaces only, as SQL TRIM)."""
    if text is None:
        return None
    return str(text).strip(' ')


def erb_regex_match(text, pattern):
    """REGEX_MATCH / REGEXMATCH: (text)::text ~ (pattern)::text."""
    if text is None or pattern is None:
        return None
    return re.search(str(pattern), erb_text(text)) is not None


def erb_regex_replace(text, pattern, replacement):
    """REGEXP_REPLACE((text)::text, pattern, replacement, 'g'). A \\& in the
    replacement is the whole match, \\1..\\9 a group, as in Postgres."""
    if text is None or pattern is None or replacement is None:
        return None
    repl = str(replacement).replace('\\&', '\\g<0>')
    return re.sub(str(pattern), repl, erb_text(text))


def erb_regex_extract(text, pattern):
    """SUBSTRING((text)::text FROM pattern): the first group when the pattern has
    one, else the whole first match; NULL when nothing matches."""
    if text is None or pattern is None:
        return None
    match = re.search(str(pattern), erb_text(text))
    if match is None:
        return None
    return match.group(1) if match.re.groups else match.group(0)


def erb_counta(value):
    """COUNTA(x): COALESCE(array_length(string_to_array(NULLIF((x)::text, ''),
    ', '), 1), 0) — the ', '-separated elements of a joined value."""
    if value is None or value == '':
        return 0
    return len(erb_text(value).split(', '))


def erb_arrayjoin(value, _separator=None):
    """ARRAYJOIN(x[, separator]) of a single value: COALESCE((x)::text, ''). The
    separator only applies to a multi-row rollup (the 3+-argument form)."""
    return '' if value is None else erb_text(value)


def erb_switch(subject, pairs, default):
    """SWITCH(expr, pattern, result, ..., [default]): CASE WHEN (expr)::text =
    (pattern)::text THEN result ... ELSE default END. A NULL never matches.
    `pairs` holds (pattern, result) thunks so only the branch taken is computed."""
    key = _sql_text(subject)
    for pattern, result in pairs:
        candidate = _sql_text(pattern())
        if key is not None and candidate is not None and key == candidate:
            return result()
    return default() if default is not None else None


def erb_char(code):
    """CHAR(n) / CHR(n): chr((n)::int)."""
    if code is None:
        return None
    return chr(erb_integer(_out(_numeric(code, 'CHAR'))))


def erb_value(text):
    """VALUE(text): (text)::NUMERIC."""
    return _out(_numeric(text, 'VALUE'))


def erb_mod(a, b):
    """MOD((a)::NUMERIC, (b)::NUMERIC): the remainder takes the dividend's sign;
    a zero divisor raises, as it does in SQL."""
    x, y = _numeric(a, 'MOD'), _numeric(b, 'MOD')
    if x is None or y is None:
        return None
    if y == 0:
        raise ZeroDivisionError("MOD: division by zero")
    return _out(x % y)


def erb_int(value):
    """INT(x): FLOOR((x)::NUMERIC)::BIGINT."""
    x = _numeric(value, 'INT')
    return None if x is None else int(x.to_integral_value(rounding=ROUND_FLOOR))


def erb_floor(value):
    """FLOOR((x)::NUMERIC)."""
    x = _numeric(value, 'FLOOR')
    return None if x is None else int(x.to_integral_value(rounding=ROUND_FLOOR))


def erb_ceiling(value):
    """CEILING(x) / CEIL(x): CEIL((x)::NUMERIC)."""
    x = _numeric(value, 'CEILING')
    return None if x is None else int(x.to_integral_value(rounding=ROUND_CEILING))


def erb_rounddown(value, digits):
    """ROUNDDOWN(x, d): FLOOR(x * 10^d) / 10^d — toward negative infinity."""
    x = _numeric(value, 'ROUNDDOWN')
    d = _numeric(digits, 'ROUNDDOWN')
    if x is None or d is None:
        return None
    return float(x.quantize(Decimal(1).scaleb(-erb_integer(_out(d))), rounding=ROUND_FLOOR))


def erb_power(base, exponent):
    """POWER((a)::NUMERIC, (b)::NUMERIC) and the `^` operator."""
    x, y = _numeric(base, 'POWER'), _numeric(exponent, 'POWER')
    if x is None or y is None:
        return None
    if x == 0 and y < 0:
        raise ZeroDivisionError("POWER: zero raised to a negative power is undefined")
    if x < 0 and y != y.to_integral_value():
        raise ValueError("POWER: a negative number raised to a non-integer power is a complex number")
    if y == y.to_integral_value() and abs(y) <= 1000:
        return _out(x ** int(y)) if y >= 0 else float(x) ** int(y)
    return float(x) ** float(y)


def _float_fn(fn, name):
    def apply(value):
        x = _numeric(value, name)
        if x is None:
            return None
        try:
            return fn(float(x))
        except ValueError:
            raise ValueError(f"{name}: {value!r} is outside the function's domain") from None
    apply.__name__ = 'erb_' + name.lower()
    apply.__doc__ = f"{name}((x)::NUMERIC)."
    return apply


erb_exp = _float_fn(math.exp, 'EXP')
erb_sin = _float_fn(math.sin, 'SIN')
erb_cos = _float_fn(math.cos, 'COS')


def erb_ln(value):
    """LN((x)::NUMERIC): natural log; zero or a negative raises, as in SQL."""
    x = _numeric(value, 'LN')
    if x is None:
        return None
    if x <= 0:
        raise ValueError(f"LN: cannot take logarithm of {value!r}")
    return math.log(float(x))


def erb_log(value):
    """LOG(x): LOG((x)::NUMERIC), base 10. Zero or a negative raises, as in SQL."""
    x = _numeric(value, 'LOG')
    if x is None:
        return None
    if x <= 0:
        raise ValueError(f"LOG: cannot take logarithm of {value!r}")
    return math.log10(float(x))


def erb_log_base(value, base):
    """LOG(x, base): log(x) / log(base) — the Excel/Airtable argument order.
    A NULL base is NULL, not base 10."""
    x, b = _numeric(value, 'LOG'), _numeric(base, 'LOG')
    if x is None or b is None:
        return None
    if x <= 0 or b <= 0 or b == 1:
        raise ValueError(f"LOG: cannot take logarithm of {value!r} in base {base!r}")
    return math.log(float(x)) / math.log(float(b))


def erb_even(value):
    """EVEN(x): CEIL((x)::NUMERIC / 2) * 2."""
    x = _numeric(value, 'EVEN')
    return None if x is None else int((x / 2).to_integral_value(rounding=ROUND_CEILING)) * 2


def erb_odd(value):
    """ODD(x): CEIL(((x)::NUMERIC - 1) / 2) * 2 + 1."""
    x = _numeric(value, 'ODD')
    return None if x is None else int(((x - 1) / 2).to_integral_value(rounding=ROUND_CEILING)) * 2 + 1


def _when(value):
    """(value)::timestamp — the wall-clock reading in erbTimezone — or None."""
    if value is None or value == '':
        return None
    return _in_zone(value)


def _calendar_date(value):
    """(value)::date — the calendar date in erbTimezone — or None."""
    if value is None or value == '':
        return None
    if isinstance(value, datetime.date) and not isinstance(value, datetime.datetime):
        return value
    if isinstance(value, str) and _is_date_only(value):
        return datetime.date.fromisoformat(value.strip())
    return _in_zone(value).date()


def _date_part(name, part):
    def apply(value):
        dt = _when(value)
        return None if dt is None else part(dt)
    apply.__name__ = 'erb_' + name.lower()
    apply.__doc__ = f"{name}(x): EXTRACT({name} FROM (x)::timestamp)::integer."
    return apply


erb_year = _date_part('YEAR', lambda dt: dt.year)
erb_month = _date_part('MONTH', lambda dt: dt.month)
erb_day = _date_part('DAY', lambda dt: dt.day)
erb_hour = _date_part('HOUR', lambda dt: dt.hour)
erb_minute = _date_part('MINUTE', lambda dt: dt.minute)
erb_second = _date_part('SECOND', lambda dt: erb_integer(dt.second + dt.microsecond / 1e6))


def erb_weekday(value, _type=None):
    """WEEKDAY(x): EXTRACT(DOW FROM (x)::timestamp)::integer + 1 — Sunday is 1.
    The optional Excel return-type argument is accepted and ignored, as the
    oracle does."""
    dt = _when(value)
    return None if dt is None else dt.isoweekday() % 7 + 1


def erb_weeknum(value, _type=None):
    """WEEKNUM(x): EXTRACT(WEEK FROM (x)::timestamp)::integer — the ISO week.
    The optional Excel return-type argument is accepted and ignored, as the
    oracle does."""
    dt = _when(value)
    return None if dt is None else dt.isocalendar()[1]


def _add_months(dt, months):
    """`+ INTERVAL 'n month'`: the same day `months` later, clamped to the end of
    a shorter month."""
    total = dt.year * 12 + (dt.month - 1) + months
    year, month = divmod(total, 12)
    month += 1
    import calendar
    day = min(dt.day, calendar.monthrange(year, month)[1])
    return dt.replace(year=year, month=month, day=day)


def erb_eomonth(value, months):
    """EOMONTH(x, n): (DATE_TRUNC('month', (x)::timestamp) + INTERVAL '1 month' *
    (n::integer + 1) - INTERVAL '1 day')::date."""
    dt = _when(value)
    n = _numeric(months, 'EOMONTH')
    if dt is None or n is None:
        return None
    first = _add_months(dt.replace(day=1, hour=0, minute=0, second=0, microsecond=0),
                        erb_integer(_out(n)) + 1)
    return (first - datetime.timedelta(days=1)).date()


_DATEADD_UNITS = ('second', 'minute', 'hour', 'day', 'week', 'month', 'year')


def erb_dateadd(value, count, unit):
    """DATEADD(x, n, unit): (x)::timestamptz + INTERVAL 'n unit'. The unit is one
    of second, minute, hour, day, week, month, year — singular, as the oracle
    requires."""
    unit_name = str(unit or '').strip().lower()
    if unit_name not in _DATEADD_UNITS:
        raise ValueError(f"DATEADD: unit {unit!r} is not a valid interval unit "
                         f"(expected: {', '.join(_DATEADD_UNITS)})")
    dt = _when(value)
    n = _numeric(count, 'DATEADD')
    if dt is None or n is None:
        return None
    if unit_name in ('month', 'year'):
        if n != n.to_integral_value():
            raise ValueError(f"DATEADD: a fractional {unit_name} count ({count!r}) is not supported")
        return _add_months(dt, int(n) * (12 if unit_name == 'year' else 1))
    seconds = {'second': 1, 'minute': 60, 'hour': 3600, 'day': 86400, 'week': 604800}[unit_name]
    if unit_name in ('day', 'week'):
        # Days move the wall clock, as an interval of days does in the session zone.
        return (dt.replace(tzinfo=None) + datetime.timedelta(seconds=float(n) * seconds)).replace(tzinfo=dt.tzinfo)
    return (dt.astimezone(datetime.timezone.utc) + datetime.timedelta(seconds=float(n) * seconds)).astimezone(_zone())


def erb_datevalue(text):
    """DATEVALUE(text): (text)::date — the date as written; any time or offset
    after it is ignored, as the date cast ignores them."""
    if text is None or text == '':
        return None
    if isinstance(text, (datetime.date, datetime.datetime)):
        return _calendar_date(text)
    match = re.match(r'\s*(\d{4}-\d{2}-\d{2})', str(text))
    if match is None:
        raise ValueError(f"DATEVALUE: invalid input syntax for type date: {text!r}")
    return datetime.date.fromisoformat(match.group(1))


def erb_datestr(value):
    """DATESTR(x): TO_CHAR((x)::date, 'YYYY-MM-DD')."""
    d = _calendar_date(value)
    return None if d is None else d.isoformat()


def erb_datetime_parse(text, _format=None, _locale=None):
    """DATETIME_PARSE(text[, format[, locale]]): (text)::timestamptz. Text with no
    offset is read in erbTimezone. The format and locale are accepted and
    ignored, as the oracle does."""
    if text is None or text == '':
        return None
    return _in_zone(text)


# Moment-style tokens rewritten to TO_CHAR ones, in the oracle's order.
_FORMAT_REWRITES = (('T', '"T"'), ('SSS', 'MS'), ('YYYY', 'YYYY'), ('YY', 'YY'), ('MM', 'MM'),
                    ('DD', 'DD'), ('HH', 'HH24'), ('hh', 'HH12'), ('mm', 'MI'), ('ss', 'SS'),
                    ('A', 'AM'), ('a', 'am'))
_TO_CHAR_PATTERNS = (
    ('HH24', lambda dt: f'{dt.hour:02d}'),
    ('HH12', lambda dt: f'{(dt.hour % 12) or 12:02d}'),
    ('HH', lambda dt: f'{(dt.hour % 12) or 12:02d}'),
    ('YYYY', lambda dt: f'{dt.year:04d}'),
    ('YY', lambda dt: f'{dt.year % 100:02d}'),
    ('MS', lambda dt: f'{dt.microsecond // 1000:03d}'),
    ('MM', lambda dt: f'{dt.month:02d}'),
    ('MI', lambda dt: f'{dt.minute:02d}'),
    ('DD', lambda dt: f'{dt.day:02d}'),
    ('SS', lambda dt: f'{dt.second:02d}'),
    ('AM', lambda dt: 'AM' if dt.hour < 12 else 'PM'),
    ('PM', lambda dt: 'AM' if dt.hour < 12 else 'PM'),
    ('am', lambda dt: 'am' if dt.hour < 12 else 'pm'),
    ('pm', lambda dt: 'am' if dt.hour < 12 else 'pm'),
    ('D', lambda dt: str(dt.isoweekday() % 7 + 1)),
)


def erb_datetime_format(value, fmt):
    """DATETIME_FORMAT(x, fmt): TO_CHAR((x)::timestamptz AT TIME ZONE 'UTC',
    fmt), with fmt's Moment tokens rewritten to TO_CHAR ones exactly as the
    oracle rewrites them. Always rendered in UTC, as the oracle renders it."""
    if value is None or value == '' or fmt is None:
        return None
    dt = _as_datetime(value).astimezone(datetime.timezone.utc)
    pattern = str(fmt)
    for old, new in _FORMAT_REWRITES:
        pattern = pattern.replace(old, new)
    out, i = [], 0
    while i < len(pattern):
        if pattern[i] == '"':
            j = pattern.find('"', i + 1)
            j = len(pattern) if j == -1 else j
            out.append(pattern[i + 1:j])
            i = j + 1
            continue
        for token, render in _TO_CHAR_PATTERNS:
            if pattern.startswith(token, i):
                out.append(render(dt))
                i += len(token)
                break
        else:
            out.append(pattern[i])
            i += 1
    return ''.join(out)


# =============================================================================
# TEXT(value, format) — docs/ERB-TEXT-FUNCTION.md
# =============================================================================
# The generator checked the format and passes it parsed: a date format as
# (is_token, text) parts, a number format as decimals / grouped / percent.

_ISO_DATE = re.compile(r'^\d{4}-\d{2}-\d{2}$')
_ISO_DATETIME = re.compile(
    r'^\d{4}-\d{2}-\d{2}[T ]\d{2}:\d{2}(:\d{2}(\.\d+)?)?\s*(Z|z|[+-]\d{2}(:?\d{2})?)?$')
_PLAIN_NUMBER = re.compile(r'^\s*[+-]?(\d+(\.\d*)?|\.\d+)\s*$')

_TEXT_DATE_TOKENS = {
    'yyyy': lambda dt: f'{dt.year:04d}',
    'yy': lambda dt: f'{dt.year % 100:02d}',
    'MM': lambda dt: f'{dt.month:02d}',
    'M': lambda dt: str(dt.month),
    'dd': lambda dt: f'{dt.day:02d}',
    'd': lambda dt: str(dt.day),
    'HH': lambda dt: f'{dt.hour:02d}',
    'H': lambda dt: str(dt.hour),
    'mm': lambda dt: f'{dt.minute:02d}',
    'm': lambda dt: str(dt.minute),
    'ss': lambda dt: f'{dt.second:02d}',
}


def erb_text_date(value, parts):
    """TEXT(value, date format). A date or datetime (or ISO text) is formatted in
    erbTimezone; a number or boolean comes back as `&` renders it; other text
    unchanged; blank is None."""
    if value is None:
        return None
    if isinstance(value, bool) or isinstance(value, (int, float)):
        return erb_text(value)
    if isinstance(value, datetime.datetime):
        dt = _in_zone(value)
    elif isinstance(value, datetime.date):
        dt = datetime.datetime.combine(value, datetime.time())
    elif isinstance(value, str):
        text = value.strip()
        if _ISO_DATE.match(text):
            dt = datetime.datetime.strptime(text, '%Y-%m-%d')
        elif _ISO_DATETIME.match(text):
            dt = _in_zone(text.replace(' ', 'T', 1))
        else:
            return value
    else:
        return erb_text(value)
    return ''.join(_TEXT_DATE_TOKENS[t](dt) if is_token else t for is_token, t in parts)


def erb_text_number(value, decimals, grouped, percent):
    """TEXT(value, number format): round half away from zero on the exact decimal
    value (a float's shortest repr), optional ',' groups and percent. A value that
    is not a number or plain numeric text comes back as `&` renders it."""
    if value is None:
        return None
    if isinstance(value, bool):
        return erb_text(value)
    if isinstance(value, int):
        number = Decimal(value)
    elif isinstance(value, float):
        if math.isnan(value) or math.isinf(value):
            return erb_text(value)
        number = Decimal(repr(value))
    elif isinstance(value, str) and _PLAIN_NUMBER.match(value):
        text = value.strip()
        number = Decimal(text + '0' if text.endswith('.') else text)
    elif isinstance(value, str):
        return value
    else:
        return erb_text(value)
    if percent:
        number *= 100
    rounded = number.quantize(Decimal(1).scaleb(-decimals), rounding=ROUND_HALF_UP)
    negative = rounded < 0
    digits = format(abs(rounded), 'f')
    whole, _, fraction = digits.partition('.')
    if grouped:
        whole = f'{int(whole):,}'
    out = ('-' if negative else '') + whole + ('.' + fraction if decimals > 0 else '')
    return out + ('%' if percent else '')


def erb_days(end, start):
    """DAYS(end, start): (end)::date - (start)::date."""
    e, s = _calendar_date(end), _calendar_date(start)
    return None if e is None or s is None else (e - s).days


def erb_is_after(a, b):
    """IS_AFTER(a, b): (a)::timestamp > (b)::timestamp."""
    x, y = _when(a), _when(b)
    return None if x is None or y is None else x > y


def erb_is_before(a, b):
    """IS_BEFORE(a, b): (a)::timestamp < (b)::timestamp."""
    x, y = _when(a), _when(b)
    return None if x is None or y is None else x < y


def erb_is_same(a, b, unit=None):
    """IS_SAME(a, b[, unit]): equal instants, or DATE_TRUNC(unit, a) =
    DATE_TRUNC(unit, b) for unit year, month or day."""
    x, y = _when(a), _when(b)
    if unit is not None:
        unit_name = str(unit).strip().lower()
        if unit_name not in ('year', 'month', 'day'):
            raise ValueError(f"IS_SAME unit {unit!r} is not supported. Supported units: year, month, day")
    if x is None or y is None:
        return None
    if unit is None:
        return x == y
    keys = {'year': lambda d: (d.year,), 'month': lambda d: (d.year, d.month),
            'day': lambda d: (d.year, d.month, d.day)}[unit_name]
    return keys(x) == keys(y)


# =============================================================================
# TRANSITIVE CLOSURE
# =============================================================================

def compute_closure_relation(edges):
    """Every reachable (from_id, to_id) pair with its shortest hop distance, and
    is_inferred TRUE iff no asserted edge states the pair — vw_<entity>_closure.
    Cyclic edge sets terminate."""
    edges = [(src, dst) for src, dst in edges
             if not erb_blank(src) and not erb_blank(dst)]
    if not edges:
        return []
    asserted = set(edges)
    adjacency = {}
    for src, dst in edges:
        adjacency.setdefault(src, []).append(dst)

    shortest = {}
    for origin in {src for src, _ in edges}:
        frontier = [(origin, (origin,))]
        hop = 0
        while frontier:
            hop += 1
            next_frontier = []
            for node, path in frontier:
                for neighbor in adjacency.get(node, []):
                    pair = (origin, neighbor)
                    if pair not in shortest:
                        shortest[pair] = hop
                    if neighbor not in path:
                        next_frontier.append((neighbor, path + (neighbor,)))
            frontier = next_frontier

    return [
        {'from_id': from_id, 'to_id': to_id, 'hop_distance': hop_distance,
         'is_inferred': (from_id, to_id) not in asserted}
        for (from_id, to_id), hop_distance in sorted(shortest.items())
    ]


# =============================================================================
# THE DATASET — lookups, aggregations and calculations over every table
# =============================================================================
#
# erb_sdk.py describes each table as a dict:
#
#   {'name': 'Agents', 'file': 'agents', 'rulebook_rows': 12,
#    'compute': compute_agents_fields, 'calculated': {'name', ...},
#    'lookups': [...], 'aggregations': [...]}
#
# A lookup is {'field', 'target', 'return', 'key', 'match'} — INDEX/MATCH, or
# LOOKUP() resolved to the target's primary key. An aggregation is
# {'field', 'op', 'table', 'target', 'criteria', 'suffix'} for COUNTIFS and the
# SUM/AVERAGE/COUNT/MIN/MAX families, or {'field', 'op': 'COMPOSITE', 'parts',
# 'scalar'} for a scalar formula wrapped around aggregates. A target that is an
# expression (ARRAYJOIN(IF(T!{{Kind}} = "x", T!{{Name}}, BLANK()), ...)) has
# 'target' None, 'expression' (a function of one scope: the row's fields plus
# 'outer', the record's own fields renamed). Either may instead
# carry 'error': a formula the generator could not compile, reported on every row.
#
# A criterion is (range_field, kind, value): kind 'field' compares the row's
# range_field to the record's own field `value`, 'literal' to a constant,
# 'op' to (operator, number), and 'op_field' to (operator, the record's own
# field): ">" & {{BidScore}}.


class Dataset:
    def __init__(self, tables, closures):
        self.tables = tables
        self.by_file = {t['file']: t for t in tables}
        self.rows = {}
        self.closures = {}
        self.blank = {}
        self.closure_specs = closures

    def table_rows(self, file):
        if file in self.closures:
            return self.closures[file]
        if file not in self.rows:
            table = self.by_file.get(file)
            if table is not None:
                raise RuntimeError(
                    f"the rulebook holds {table['rulebook_rows']} {table['name']} rows but "
                    f"blank-tests has no {file}.json — regenerate the test fixtures")
            raise RuntimeError(f"no table {file!r}")
        return self.rows[file]

    # --- lookups ------------------------------------------------------------

    def compute_lookup(self, spec, rows):
        field = spec['field']
        if spec.get('error'):
            for record in rows:
                record[field] = None
                record.setdefault('_erb_errors', {})[field] = spec['error']
            return
        try:
            index = {}
            for target in self.table_rows(spec['target']):
                pk = target.get(spec['match'])
                if pk is not None:
                    index[pk] = target.get(spec['return'])
            no_match = self.blank_join_value(spec['target'], spec['return'])
        except Exception as exc:
            for record in rows:
                record[field] = None
                record.setdefault('_erb_errors', {})[field] = str(exc)
            return
        key_field = spec['key']
        for record in rows:
            key = record.get(key_field)
            if key is not None and key in index:
                record[field] = index[key]
            else:
                record[field] = no_match

    def blank_join_value(self, file, field):
        """What a lookup renders when nothing matches: the joined table's field
        evaluated against an all-NULL row, as a LEFT JOIN evaluates a calculated
        column — a formula built on literal text still yields text."""
        cache_key = (file, field)
        if cache_key in self.blank:
            return self.blank[cache_key]
        table = self.by_file.get(file)
        value = None
        if table is not None:
            lookup = next((l for l in table['lookups'] if l['field'] == field), None)
            if lookup is not None:
                value = None if lookup.get('error') else self.blank_join_value(lookup['target'], lookup['return'])
            elif field in table['calculated']:
                try:
                    value = table['compute']({f: None for f in table['fields']}).get(field)
                except Exception:
                    value = None
        self.blank[cache_key] = value
        return value

    # --- aggregations -------------------------------------------------------

    def compute_aggregation(self, spec, rows):
        field = spec['field']
        if spec.get('error'):
            for record in rows:
                record.setdefault('_erb_errors', {})[field] = spec['error']
            return
        try:
            if spec['op'] == 'COMPOSITE':
                for record in rows:
                    scope = dict(record)
                    for part in spec['parts']:
                        scope[part['field']] = self.aggregate_value(part, record)
                    record[field] = spec['scalar'](scope)
            else:
                for record in rows:
                    record[field] = self.aggregate_value(spec, record)
        except Exception as exc:
            for record in rows:
                record.setdefault('_erb_errors', {})[field] = str(exc)

    def aggregate_value(self, spec, record):
        matching = [row for row in self.table_rows(spec['table'])
                    if _row_matches_criteria(row, spec['criteria'], record)]
        op = spec['op']
        if op in ('COUNTIFS', 'COUNT'):
            return len(matching)
        expression = spec.get('expression')
        if expression is not None:
            # An expression as the target (agg(<expression>) FROM table WHERE ..): computed on each
            # matching row, with the record's own fields as erb_outer_<field>; BLANK() is None.
            outer = {scope_key: record.get(record_key) for scope_key, record_key in spec.get('outer', [])}
            targets = [expression({**row, **outer}) for row in matching]
        else:
            target = spec['target']
            targets = [row.get(target) for row in matching]
        if op == 'ARRAYJOIN':
            # COALESCE(string_agg(target::text, ', '), ''): NULLs skipped, rows in table order,
            # and '' when nothing is left, as rulebook-to-postgres writes it.
            values = [erb_text(v) for v in targets if v is not None]
            return ', '.join(values)
        if op in ('MIN', 'MAX'):
            values = [v for v in targets if v is not None and v != '']
            if not values:
                # 'empty': 0 where the oracle writes COALESCE(MIN(..)::numeric, 0).
                return spec.get('empty')
            return min(values) if op == 'MIN' else max(values)
        suffix = spec.get('suffix')
        numbers = []
        for raw_value in targets:
            if raw_value is None or raw_value == '':
                continue
            try:
                value = float(raw_value)
            except (TypeError, ValueError):
                continue
            if suffix is not None:
                try:
                    value = suffix(value)
                except Exception:
                    continue
            numbers.append(value)
        if op == 'SUM':
            return sum(numbers) if numbers else 0
        return (sum(numbers) / len(numbers)) if numbers else spec.get('empty')


def _criteria_compare(cell_value, op, target) -> bool:
    """An Excel comparison criteria (">500"): numeric only; blank never matches."""
    if cell_value is None or cell_value == '':
        return False
    try:
        cell_num = float(cell_value)
        target_num = float(target)
    except (TypeError, ValueError):
        return False
    if op == '>':
        return cell_num > target_num
    if op == '>=':
        return cell_num >= target_num
    if op == '<':
        return cell_num < target_num
    if op == '<=':
        return cell_num <= target_num
    if op == '<>':
        return cell_num != target_num
    return cell_num == target_num


def _row_matches_criteria(row, criteria, record) -> bool:
    for range_field, kind, value in criteria:
        cell = row.get(range_field)
        if kind in ('field', 'raw_field'):
            # A blank calculated key is '' in the oracle and None here, so both
            # spellings of blank are one value. A blank RAW key never matches: the
            # oracle compares against NULLIF(own, ''), and NULL equals nothing.
            other = record.get(value)
            if kind == 'raw_field' and (other is None or other == ''):
                return False
            if ('' if cell is None else cell) != ('' if other is None else other):
                return False
        elif kind == 'op':
            op, target = value
            if not _criteria_compare(cell, op, target):
                return False
        elif kind == 'op_field':
            op, other = value
            if not _criteria_compare(cell, op, record.get(other)):
                return False
        elif cell != value:
            return False
    return True


# =============================================================================
# THE RUN
# =============================================================================

def _load_rows(path):
    with open(path, 'r', encoding='utf-8') as f:
        rows = json.load(f)
    if not isinstance(rows, list):
        raise RuntimeError(f"{path} does not hold a JSON array of rows")
    return rows


def _type_computed(table, rows):
    """Apply erbWholeNumber to every computed field of `rows`, by the field's
    declared datatype (the SDK's 'datatypes' map)."""
    datatypes = table.get('datatypes') or {}
    computed = set(table['calculated'])
    computed.update(spec['field'] for spec in table['lookups'])
    computed.update(spec['field'] for spec in table['aggregations'])
    for record in rows:
        for field in computed:
            if field in record:
                record[field] = erb_number(record[field], datatypes.get(field))
    return rows


def _compute_in_row_order(dataset, table, self_lookups, rows):
    """Compute a table whose lookups read its OWN rows (a lattice cell reading
    its parent cells, a step reading the previous step), one row at a time in
    the order the row-to-row references require.

    Such a table is cyclic field by field (Cells.Value reads Cells.Value) but
    usually acyclic row by row. Computed table-wide, a chain of N rows needs N
    passes to settle and blows straight through the pass bound. Computed in
    dependency order within the pass, every row reads parents already computed
    in this same pass, so the whole chain settles at once.

    A lookup resolves exactly as compute_lookup does — the LAST row whose match
    field equals the key wins, and no match renders the blank-join value — but
    against this pass's rows instead of the previous pass's. If the row-to-row
    references contain a genuine cycle there is no order: fall back to the
    table-wide computation against the previous pass, so the outer pass bound
    reports the cycle exactly as before.
    """
    compute = table['compute']
    n = len(rows)
    resolved = []  # (spec, {match value: row index}, blank-join value)
    for spec in self_lookups:
        index = {}
        for i, r in enumerate(rows):
            pk = r.get(spec['match'])
            if pk is not None:
                index[pk] = i
        resolved.append((spec, index, dataset.blank_join_value(spec['target'], spec['return'])))

    parents = [[] for _ in range(n)]
    children = [[] for _ in range(n)]
    for i, r in enumerate(rows):
        for spec, index, _ in resolved:
            key = r.get(spec['key'])
            j = index.get(key) if key is not None else None
            if j is not None and j != i:
                parents[i].append(j)
                children[j].append(i)
            elif j == i:
                parents[i].append(i)  # a row reading itself is a real cycle

    indegree = [len(set(p)) for p in parents]
    for i in range(n):
        parents[i] = list(set(parents[i]))
    ready = [i for i in range(n) if indegree[i] == 0]
    order = []
    seen_child = [set() for _ in range(n)]
    while ready:
        i = ready.pop()
        order.append(i)
        for c in children[i]:
            if i in seen_child[c]:
                continue
            seen_child[c].add(i)
            indegree[c] -= 1
            if indegree[c] == 0:
                ready.append(c)

    if len(order) != n:
        for spec in self_lookups:
            dataset.compute_lookup(spec, rows)
        return _type_computed(table, [compute(r) for r in rows])

    out = [None] * n
    for i in order:
        record = rows[i]
        for spec, index, no_match in resolved:
            key = record.get(spec['key'])
            j = index.get(key) if key is not None else None
            record[spec['field']] = out[j].get(spec['return']) if j is not None else no_match
        out[i] = _type_computed(table, [compute(record)])[0]
    return out


def erb_settle(tables, closures, calculated_field_count, rows_by_file):
    """Compute every lookup, aggregation and calculated field over the dataset
    until nothing changes, and return {file: rows}.

    Tables depend on each other in both directions — one table's lookup reads
    another's aggregation, which counts rows of the first — so no table order
    works; a field's inputs settle one pass before the field does. The pass bound
    is the number of computed fields, which a dependency chain cannot exceed
    without a cycle.
    """
    dataset = Dataset(tables, closures)
    computed = 0
    for table in tables:
        if table['file'] in rows_by_file:
            dataset.rows[table['file']] = rows_by_file[table['file']]
            computed += len(table['lookups']) + len(table['aggregations'])
        elif table['rulebook_rows'] == 0:
            # A table the rulebook declares empty reads as empty. One the
            # rulebook holds rows for stays unloaded, so anything that reads it
            # fails naming the gap.
            dataset.rows[table['file']] = []

    def materialize_closure(closure):
        # A filtered closure closes over only the edges whose filter field is
        # TRUE. The filter may be a derived field, so it is re-read from the
        # current rows on every pass and settles with them.
        source = dataset.rows[closure['source']]
        edge_filter = closure.get('filter')
        dataset.closures[closure['view']] = compute_closure_relation(
            [(row.get(closure['from']), row.get(closure['to'])) for row in source
             if edge_filter is None or row.get(edge_filter) is True])

    for closure in closures:
        if closure['source'] not in dataset.rows:
            raise RuntimeError(f"closure {closure['view']} needs the rows of {closure['source']}")
        materialize_closure(closure)
        print(f"  -> {closure['view']}: {len(dataset.closures[closure['view']])} pairs")

    max_passes = computed + calculated_field_count + 2
    passes = 0
    while True:
        passes += 1
        if passes > max_passes:
            raise RuntimeError(
                f"the dataset did not settle within {max_passes} passes — the field dependencies are cyclic.")
        dataset.blank = {}
        changed = False
        for closure in closures:
            if closure.get('filter') is not None:
                materialize_closure(closure)
        for table in tables:
            file = table['file']
            if file not in dataset.rows:
                continue
            previous = dataset.rows[file]
            rows = [{k: v for k, v in r.items() if k != '_erb_errors'} for r in previous]
            self_lookups = [l for l in table['lookups'] if l['target'] == file and not l.get('error')]
            for lookup in table['lookups']:
                if lookup not in self_lookups:
                    dataset.compute_lookup(lookup, rows)
            for aggregation in table['aggregations']:
                dataset.compute_aggregation(aggregation, rows)
            compute = table['compute']
            if self_lookups:
                rows = _compute_in_row_order(dataset, table, self_lookups, rows)
            else:
                rows = _type_computed(table, [compute(r) for r in rows])
            if rows != previous:
                changed = True
            dataset.rows[file] = rows
        if not changed:
            break
    print(f"Python substrate: dataset settled after {passes} passes")
    return {file: dataset.rows[file] for file in rows_by_file}


def erb_run(tables, closures, calculated_field_count):
    """Load every table's blank tests, settle the dataset, and write each table's
    answers under $ERB_TESTING_DIR/$ERB_SUBSTRATE_NAME/test-answers."""
    testing = os.environ.get('ERB_TESTING_DIR')
    if not testing:
        sys.exit("FATAL: ERB_TESTING_DIR is not set; point it at the domain's testing/ directory.")
    substrate = os.environ.get('ERB_SUBSTRATE_NAME')
    if not substrate:
        sys.exit("FATAL: ERB_SUBSTRATE_NAME is not set; the harness must name the substrate whose answers these are.")
    blank_dir = Path(testing) / 'blank-tests'
    answers_dir = Path(testing) / substrate / 'test-answers'
    answers_dir.mkdir(parents=True, exist_ok=True)

    rows_by_file = {}
    for table in tables:
        path = blank_dir / f"{table['file']}.json"
        if path.is_file():
            rows_by_file[table['file']] = _load_rows(path)
    if not rows_by_file:
        sys.exit(f"FATAL: no blank tests for any table under {blank_dir}")

    try:
        settled = erb_settle(tables, closures, calculated_field_count, rows_by_file)
    except RuntimeError as exc:
        sys.exit(f"FATAL: {exc}")

    total = 0
    for file in sorted(settled):
        rows = settled[file]
        with open(answers_dir / f"{file}.json", 'w', encoding='utf-8') as f:
            json.dump(rows, f, indent=2, default=erb_json_value)
        total += len(rows)
        print(f"  -> {file}: {len(rows)} records")
    print(f"Python substrate: wrote {total} records across {len(settled)} tables to {answers_dir}")
