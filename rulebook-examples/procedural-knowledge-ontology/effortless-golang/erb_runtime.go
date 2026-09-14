package main

// Formula runtime for the Go substrate.
//
// HAND-WRITTEN, NOT GENERATED. erb_sdk.go compiles every formula to calls on
// this file, so there is one implementation of the dialect's semantics in Go.
// Those semantics are the Postgres oracle's, as rulebook-to-python implements
// them (orchestration/formula_parser.py): NULL propagates through comparisons
// and AND/OR/NOT, a raw text column reads through NULLIF(x, ''), DATETIME_DIFF
// counts calendar days, CONCAT renders a datetime as timestamptz text. When a
// rule changes there, it changes here.
//
// A Value is dynamically typed because a formula's operands are: a lookup can
// return any column, an IF can mix branches, and NULL is a value of every type.
// Typed struct fields are the SDK's public shape; Values are what formulas
// compute with in between.

import (
	"encoding/json"
	"fmt"
	"math"
	"os"
	"path/filepath"
	"regexp"
	"sort"
	"strconv"
	"strings"
	"time"
	"unicode/utf8"

	// The zone database, embedded, so erbTimezone resolves without OS tzdata.
	_ "time/tzdata"
)

// ───────────────────────────── build parameters ─────────────────────────────
//
// The ERB build parameters (docs/ERB-BUILD-PARAMETERS.md) this SDK was
// generated under. erb_sdk.go calls erbConfigure from an init(); every helper
// below reads erbParam, and nothing runs unconfigured. erb_runtime.py is the
// reference; when a rule changes there, it changes here.

var erbParams map[string]string
var erbZone *time.Location

var erbParameterValues = map[string][]string{
	"erbDateDiff":     {"calendar", "elapsed"},
	"erbTimezone":     {"UTC"}, // or any IANA zone name
	"erbDateTimeText": {"iso8601", "sql"},
	"erbBlankLogic":   {"coerce", "propagate"},
	"erbWholeNumber":  {"by-field-type", "integer", "decimal"},
}

func erbConfigure(params map[string]string) {
	for name := range params {
		if _, ok := erbParameterValues[name]; !ok {
			panic(fmt.Sprintf("erbConfigure: unknown ERB build parameter %q", name))
		}
	}
	for name, allowed := range erbParameterValues {
		value, ok := params[name]
		if !ok {
			panic(fmt.Sprintf("erbConfigure: missing ERB build parameter %q", name))
		}
		if name == "erbTimezone" {
			continue
		}
		found := false
		for _, a := range allowed {
			if a == value {
				found = true
			}
		}
		if !found {
			panic(fmt.Sprintf("erbConfigure: %s=%q is not one of %v", name, value, allowed))
		}
	}
	if params["erbWholeNumber"] != "by-field-type" {
		// A struct field is typed by its declared datatype; there is no honest
		// rendering of the other two values here.
		panic(fmt.Sprintf("erbConfigure: the Go substrate honours erbWholeNumber=by-field-type only, not %q", params["erbWholeNumber"]))
	}
	loc, err := time.LoadLocation(params["erbTimezone"])
	if err != nil {
		panic(fmt.Sprintf("erbConfigure: erbTimezone=%q cannot be resolved: %v", params["erbTimezone"], err))
	}
	erbParams = params
	erbZone = loc
}

func erbParam(name string) string {
	if erbParams == nil {
		panic("erb_runtime is not configured: erb_sdk.go must call erbConfigure() before any formula runs")
	}
	return erbParams[name]
}

func erbZoneLoc() *time.Location {
	erbParam("erbTimezone")
	return erbZone
}

func coerceBlanks() bool { return erbParam("erbBlankLogic") == "coerce" }

// ─────────────────────────────── values ───────────────────────────────

type ValueKind int

const (
	KNull ValueKind = iota
	KBool
	KNum
	KStr
	KTime
)

// Value is one formula value. Num carries IsInt so text rendering can match
// how the reference implementation renders an integer (3) against a float (3.0).
type Value struct {
	K     ValueKind
	B     bool
	N     float64
	IsInt bool
	S     string
	T     time.Time
	Zoned bool // a KTime that carries a UTC offset
}

var Null = Value{K: KNull}

func vB(b bool) Value      { return Value{K: KBool, B: b} }
func vS(s string) Value    { return Value{K: KStr, S: s} }
func vI(n int) Value       { return Value{K: KNum, N: float64(n), IsInt: true} }
func vF(n float64) Value   { return Value{K: KNum, N: n} }
func vT(t time.Time) Value { return Value{K: KTime, T: t} }
func vTZ(t time.Time) Value { return Value{K: KTime, T: t, Zoned: true} }

func (v Value) IsNull() bool { return v.K == KNull }

func vStr(p *string) Value {
	if p == nil {
		return Null
	}
	return vS(*p)
}

func vStrPlain(s string) Value { return vS(s) }

func vBool(p *bool) Value {
	if p == nil {
		return Null
	}
	return vB(*p)
}

func vBoolPlain(b bool) Value { return vB(b) }

func vInt(p *int) Value {
	if p == nil {
		return Null
	}
	return vI(*p)
}

func vIntPlain(n int) Value { return vI(n) }

func vNum(p *float64) Value {
	if p == nil {
		return Null
	}
	return numberValue(*p)
}

func vNumPlain(n float64) Value { return numberValue(n) }

func vAny(p any) Value { return fromJSON(p) }

// numberValue keeps a whole float64 that arrived from an integer column integral.
func numberValue(n float64) Value {
	if n == math.Trunc(n) && !math.IsInf(n, 0) && math.Abs(n) < 1e15 {
		return vI(int(n))
	}
	return vF(n)
}

// fromJSON converts a decoded JSON value (UseNumber) into a Value.
func fromJSON(x any) Value {
	switch t := x.(type) {
	case nil:
		return Null
	case bool:
		return vB(t)
	case string:
		return vS(t)
	case json.Number:
		if i, err := strconv.ParseInt(string(t), 10, 64); err == nil {
			return vI(int(i))
		}
		f, err := strconv.ParseFloat(string(t), 64)
		if err != nil {
			panic(fmt.Sprintf("unreadable number %q", string(t)))
		}
		return vF(f)
	case float64:
		return numberValue(t)
	case int:
		return vI(t)
	}
	panic(fmt.Sprintf("unsupported JSON value %T in a record field", x))
}

// ───────────────────────────── typed boundary ─────────────────────────────

func toStringPtr(v Value) *string {
	switch v.K {
	case KNull:
		return nil
	case KStr:
		s := v.S
		return &s
	}
	s := pyStr(v)
	return &s
}

func toBoolPtr(v Value) *bool {
	switch v.K {
	case KNull:
		return nil
	case KBool:
		b := v.B
		return &b
	case KStr:
		switch strings.ToLower(strings.TrimSpace(v.S)) {
		case "true":
			b := true
			return &b
		case "false":
			b := false
			return &b
		case "":
			return nil
		}
	case KNum:
		b := v.N != 0
		return &b
	}
	panic(fmt.Sprintf("cannot store %s in a boolean field", pyRepr(v)))
}

func toFloatPtr(v Value) *float64 {
	switch v.K {
	case KNull:
		return nil
	case KNum:
		n := v.N
		return &n
	case KBool:
		n := 0.0
		if v.B {
			n = 1
		}
		return &n
	case KStr:
		if strings.TrimSpace(v.S) == "" {
			return nil
		}
		if n, ok := toNumber(v); ok {
			f := n.N
			return &f
		}
	}
	panic(fmt.Sprintf("cannot store %s in a number field", pyRepr(v)))
}

func toIntPtr(v Value) *int {
	f := toFloatPtr(v)
	if f == nil {
		return nil
	}
	n := int(roundHalfAway(*f, 0))
	return &n
}

func strPlain(v Value) string {
	if p := toStringPtr(v); p != nil {
		return *p
	}
	return ""
}

func boolPlain(v Value) bool {
	if p := toBoolPtr(v); p != nil {
		return *p
	}
	return false
}

func intPlain(v Value) int {
	if p := toIntPtr(v); p != nil {
		return *p
	}
	return 0
}

func floatPlain(v Value) float64 {
	if p := toFloatPtr(v); p != nil {
		return *p
	}
	return 0
}

func anyPlain(v Value) any { return toJSON(v) }

func toJSON(v Value) any {
	switch v.K {
	case KNull:
		return nil
	case KBool:
		return v.B
	case KNum:
		if v.IsInt {
			return int64(v.N)
		}
		return v.N
	case KStr:
		return v.S
	case KTime:
		// A computed datetime is written per erbDateTimeText, as the reference
		// implementation's erb_json_value writes it.
		return erbDatetimeText(v).S
	}
	return nil
}

// ───────────────────────────── text rendering ─────────────────────────────

// pyStr renders a value as str() does in the reference implementation.
func pyStr(v Value) string {
	switch v.K {
	case KNull:
		return "None"
	case KBool:
		if v.B {
			return "True"
		}
		return "False"
	case KNum:
		if v.IsInt {
			return strconv.FormatInt(int64(v.N), 10)
		}
		return pyFloatRepr(v.N)
	case KStr:
		return v.S
	case KTime:
		s := v.T.Format("2006-01-02 15:04:05")
		if v.T.Nanosecond() != 0 {
			s += fmt.Sprintf(".%06d", v.T.Nanosecond()/1000)
		}
		if v.Zoned {
			s += v.T.Format("-07:00")
		}
		return s
	}
	return ""
}

func pyRepr(v Value) string {
	if v.K == KStr {
		return strconv.Quote(v.S)
	}
	return pyStr(v)
}

func pyFloatRepr(f float64) string {
	if math.IsInf(f, 1) {
		return "inf"
	}
	if math.IsInf(f, -1) {
		return "-inf"
	}
	if math.IsNaN(f) {
		return "nan"
	}
	s := strconv.FormatFloat(f, 'g', -1, 64)
	if strings.ContainsAny(s, "e") {
		// Python switches to exponent notation at 1e16 and below 1e-4.
		exp := math.Floor(math.Log10(math.Abs(f)))
		if exp >= -4 && exp < 16 {
			s = strconv.FormatFloat(f, 'f', -1, 64)
		} else {
			mant, e, _ := strings.Cut(s, "e")
			n, _ := strconv.Atoi(e)
			sign := "+"
			if n < 0 {
				sign = "-"
				n = -n
			}
			return fmt.Sprintf("%se%s%02d", mant, sign, n)
		}
	}
	if !strings.ContainsAny(s, ".e") {
		s += ".0"
	}
	return s
}

// truthy is Python truthiness, used where compiled code reads `x or ""`.
func truthy(v Value) bool {
	switch v.K {
	case KNull:
		return false
	case KBool:
		return v.B
	case KNum:
		return v.N != 0
	case KStr:
		return v.S != ""
	}
	return true
}

// erbTextOr is `str(x or "")`.
func erbTextOr(v Value) Value {
	if !truthy(v) {
		return vS("")
	}
	return vS(pyStr(v))
}

// erbTextNotNull is `str(x if x is not None else "")`.
func erbTextNotNull(v Value) Value {
	if v.K == KNull {
		return vS("")
	}
	return vS(pyStr(v))
}

func erbConcat(parts ...Value) Value {
	var b strings.Builder
	for _, p := range parts {
		b.WriteString(p.S)
	}
	return vS(b.String())
}

// ───────────────────────────── numbers ─────────────────────────────

var intPattern = regexp.MustCompile(`^[+-]?\d+$`)

// toNumber is _to_number: numbers and numeric strings are numbers; booleans
// and anything else are not.
func toNumber(v Value) (Value, bool) {
	switch v.K {
	case KNum:
		return v, true
	case KStr:
		s := strings.TrimSpace(v.S)
		if s == "" {
			return Null, false
		}
		if intPattern.MatchString(s) {
			if i, err := strconv.ParseInt(s, 10, 64); err == nil {
				return vI(int(i)), true
			}
		}
		if f, err := strconv.ParseFloat(s, 64); err == nil {
			return vF(f), true
		}
	}
	return Null, false
}

func numOrZero(v Value) Value {
	if n, ok := toNumber(v); ok {
		return n
	}
	return vI(0)
}

func arith(a, b Value, op byte) Value {
	switch op {
	case '+':
		return Value{K: KNum, N: a.N + b.N, IsInt: a.IsInt && b.IsInt}
	case '-':
		return Value{K: KNum, N: a.N - b.N, IsInt: a.IsInt && b.IsInt}
	case '*':
		return Value{K: KNum, N: a.N * b.N, IsInt: a.IsInt && b.IsInt}
	}
	panic("unknown arithmetic operator")
}

func erbNeg(v Value) Value {
	if !truthy(v) {
		return vI(0)
	}
	if v.K != KNum {
		panic(fmt.Sprintf("bad operand type for unary -: %s", pyRepr(v)))
	}
	return Value{K: KNum, N: -v.N, IsInt: v.IsInt}
}

func erbAdd(a, b Value) Value {
	at, aok := dateOrNone(a)
	bt, bok := dateOrNone(b)
	if aok && !bok {
		return vT(at.Add(daysDelta(b)))
	}
	if bok && !aok {
		return vT(bt.Add(daysDelta(a)))
	}
	return arith(numOrZero(a), numOrZero(b), '+')
}

func erbSub(a, b Value) Value {
	if at, ok := dateOrNone(a); ok {
		if _, bok := dateOrNone(b); !bok {
			return vT(at.Add(-daysDelta(b)))
		}
	}
	return arith(numOrZero(a), numOrZero(b), '-')
}

func erbMul(a, b Value) Value { return arith(numOrZero(a), numOrZero(b), '*') }

func erbDiv(a, b Value) Value {
	d := numOrZero(b)
	if d.N == 0 {
		return Null
	}
	return vF(numOrZero(a).N / d.N)
}

func daysDelta(v Value) time.Duration {
	f := 0.0
	if truthy(v) {
		n, ok := toNumber(v)
		if !ok {
			return 0
		}
		f = n.N
	}
	return time.Duration(f * float64(24*time.Hour))
}

// decimalRound rounds the shortest decimal representation of f to `digits`
// places, toward +inf when ceiling is set and half away from zero otherwise —
// Decimal(str(x)).quantize(...), so 2.675 rounds as the text "2.675" reads.
func decimalRound(f float64, digits int, ceiling bool) float64 {
	text := strconv.FormatFloat(f, 'f', -1, 64)
	neg := strings.HasPrefix(text, "-")
	text = strings.TrimPrefix(text, "-")
	intPart, frac, _ := strings.Cut(text, ".")
	digitsAll := intPart + frac
	point := len(intPart) // decimal point position within digitsAll
	keep := point + digits
	if keep < 0 {
		digitsAll = strings.Repeat("0", -keep) + digitsAll
		point += -keep
		keep = 0
	}
	for len(digitsAll) < keep {
		digitsAll += "0"
	}
	kept := digitsAll[:keep]
	rest := digitsAll[keep:]
	roundUp := false
	if ceiling {
		roundUp = !neg && strings.Trim(rest, "0") != ""
	} else {
		roundUp = len(rest) > 0 && rest[0] >= '5'
	}
	result := []byte(kept)
	if roundUp {
		i := len(result) - 1
		for ; i >= 0; i-- {
			if result[i] == '9' {
				result[i] = '0'
				continue
			}
			result[i]++
			break
		}
		if i < 0 {
			result = append([]byte{'1'}, result...)
			point++
		}
	}
	whole := string(result)
	var out string
	if point >= len(whole) {
		out = whole + strings.Repeat("0", point-len(whole))
	} else if point <= 0 {
		out = "0." + strings.Repeat("0", -point) + whole
	} else {
		out = whole[:point] + "." + whole[point:]
	}
	if out == "" {
		out = "0"
	}
	r, err := strconv.ParseFloat(out, 64)
	if err != nil {
		panic(fmt.Sprintf("rounding %v: %v", f, err))
	}
	if neg {
		r = -r
	}
	return r
}

func roundHalfAway(f float64, digits int) float64 { return decimalRound(f, digits, false) }

func roundDigits(v Value) int {
	if !truthy(v) && v.K != KNum {
		return 0
	}
	n, ok := toNumber(v)
	if !ok {
		panic(fmt.Sprintf("invalid ROUND digits %s", pyRepr(v)))
	}
	return int(n.N)
}

func erbRound(v, digits Value) Value {
	if v.K == KNull || (v.K == KStr && v.S == "") {
		return Null
	}
	n, ok := toNumber(v)
	if !ok {
		panic(fmt.Sprintf("cannot ROUND %s", pyRepr(v)))
	}
	return vF(decimalRound(n.N, roundDigits(digits), false))
}

func erbRoundup(v, digits Value) Value {
	if v.K == KNull || (v.K == KStr && v.S == "") {
		return Null
	}
	n, ok := toNumber(v)
	if !ok {
		panic(fmt.Sprintf("cannot ROUNDUP %s", pyRepr(v)))
	}
	return vF(decimalRound(n.N, roundDigits(digits), true))
}

// erbInteger is the oracle's ::integer cast on a field declared integer.
func erbInteger(v Value) Value {
	if v.K == KNull || v.K == KBool {
		return v
	}
	n, ok := toNumber(v)
	if !ok {
		return v
	}
	return vI(int(roundHalfAway(n.N, 0)))
}

func erbAbs(v Value) Value {
	x := orZero(v)
	return Value{K: KNum, N: math.Abs(x.N), IsInt: x.IsInt}
}

func orZero(v Value) Value {
	if !truthy(v) {
		return vI(0)
	}
	if v.K != KNum {
		panic(fmt.Sprintf("expected a number, got %s", pyRepr(v)))
	}
	return v
}

func erbPower(b, e Value) Value {
	x, y := orZero(b), orZero(e)
	r := math.Pow(x.N, y.N)
	return Value{K: KNum, N: r, IsInt: x.IsInt && y.IsInt && y.N >= 0}
}

func erbSqrt(v Value) Value { return vF(math.Sqrt(orZero(v).N)) }
func erbTan(v Value) Value  { return vF(math.Tan(orZero(v).N)) }
func erbLog10(v Value) Value {
	return vF(math.Log10(orZero(v).N))
}
func erbLog(v, base Value) Value {
	return vF(math.Log(orZero(v).N) / math.Log(orZero(base).N))
}

func erbMaxMin(max bool, args ...Value) Value {
	best := orZero(args[0])
	for _, a := range args[1:] {
		x := orZero(a)
		if (max && x.N > best.N) || (!max && x.N < best.N) {
			best = x
		}
	}
	return best
}

func erbSum(args ...Value) Value {
	total := vI(0)
	for _, a := range args {
		total = arith(total, numOrZero(a), '+')
	}
	return total
}

// ───────────────────────────── logic ─────────────────────────────

func erbBool3(v Value) Value {
	if v.K == KNull || v.K == KBool {
		return v
	}
	return vB(false)
}

func erbIsTrue(v Value) Value   { return vB(v.K == KBool && v.B) }
func erbHasValue(v Value) Value { return vB(!isBlank(v)) }

// erbAnd: under erbBlankLogic=coerce a blank operand is FALSE; under propagate
// FALSE if any operand is FALSE, else NULL if any is NULL, else TRUE.
func erbAnd(values ...Value) Value {
	sawNull := false
	for _, v := range values {
		if v.K == KBool && !v.B {
			return vB(false)
		}
		if v.K == KNull {
			sawNull = true
		}
	}
	if sawNull {
		if coerceBlanks() {
			return vB(false)
		}
		return Null
	}
	return vB(true)
}

func erbOr(values ...Value) Value {
	sawNull := false
	for _, v := range values {
		if v.K == KBool && v.B {
			return vB(true)
		}
		if v.K == KNull {
			sawNull = true
		}
	}
	if sawNull {
		if coerceBlanks() {
			return vB(false)
		}
		return Null
	}
	return vB(false)
}

// erbNot: coerce makes NOT(blank) TRUE; propagate keeps it NULL.
func erbNot(v Value) Value {
	if v.K == KNull {
		if coerceBlanks() {
			return vB(true)
		}
		return Null
	}
	return vB(!truthy(v))
}

func erbIf(cond Value, then func() Value, otherwise func() Value) Value {
	if truthy(cond) {
		return then()
	}
	return otherwise()
}

func isBlank(v Value) bool { return v.K == KNull || (v.K == KStr && v.S == "") }

func erbIsBlank(v Value) Value    { return vB(isBlank(v)) }
func erbIsNotBlank(v Value) Value { return vB(!isBlank(v)) }

func erbNullif(v Value) Value {
	if v.K == KStr && v.S == "" {
		return Null
	}
	return v
}

// pyEqual is Python ==.
func pyEqual(a, b Value) bool {
	an, aNum := numericLike(a)
	bn, bNum := numericLike(b)
	if aNum && bNum {
		return an == bn
	}
	if a.K != b.K {
		return false
	}
	switch a.K {
	case KNull:
		return true
	case KStr:
		return a.S == b.S
	case KTime:
		return a.T.Equal(b.T)
	}
	return false
}

// numericLike: numbers and booleans compare numerically under Python ==.
func numericLike(v Value) (float64, bool) {
	switch v.K {
	case KNum:
		return v.N, true
	case KBool:
		if v.B {
			return 1, true
		}
		return 0, true
	}
	return 0, false
}

// blankAsZeroOf: under coerce a blank compared against `other` takes the other
// side's zero — FALSE against a boolean, 0 against a number, "" otherwise.
func blankAsZeroOf(other Value) Value {
	if other.K == KBool {
		return vB(false)
	}
	if _, ok := toNumber(other); ok {
		return vI(0)
	}
	return vS("")
}

// coercedPair resolves blank operands per erbBlankLogic: (a, b, true) to
// compare, or (_, _, false) when the blank must propagate as NULL.
func coercedPair(a, b Value) (Value, Value, bool) {
	if a.K == KNull && b.K == KNull {
		if coerceBlanks() {
			return vS(""), vS(""), true
		}
		return a, b, false
	}
	if a.K == KNull || b.K == KNull {
		if !coerceBlanks() {
			return a, b, false
		}
		if a.K == KNull {
			return blankAsZeroOf(b), b, true
		}
		return a, blankAsZeroOf(a), true
	}
	return a, b, true
}

func erbEq(a, b Value) Value {
	a, b, ok := coercedPair(a, b)
	if !ok {
		return Null
	}
	return vB(pyEqual(a, b))
}

func erbNe(a, b Value) Value {
	a, b, ok := coercedPair(a, b)
	if !ok {
		return Null
	}
	return vB(!pyEqual(a, b))
}

// erbCmp is an ordered comparison: numbers and numeric strings as numbers,
// two strings as strings, two booleans as booleans; NULL makes it NULL and
// anything else cannot be ordered and is FALSE.
func erbCmp(a Value, op string, b Value) Value {
	a, b, ok := coercedPair(a, b)
	if !ok {
		return Null
	}
	var c int
	an, aok := toNumber(a)
	bn, bok := toNumber(b)
	switch {
	case aok && bok:
		c = cmpFloat(an.N, bn.N)
	case a.K == KStr && b.K == KStr:
		c = strings.Compare(a.S, b.S)
	case a.K == KBool && b.K == KBool:
		c = cmpFloat(boolNum(a.B), boolNum(b.B))
	default:
		return vB(false)
	}
	switch op {
	case "<":
		return vB(c < 0)
	case "<=":
		return vB(c <= 0)
	case ">":
		return vB(c > 0)
	}
	return vB(c >= 0)
}

func cmpFloat(a, b float64) int {
	if a < b {
		return -1
	}
	if a > b {
		return 1
	}
	return 0
}

func boolNum(b bool) float64 {
	if b {
		return 1
	}
	return 0
}

func erbCoalesce(values ...Value) Value {
	for _, v := range values {
		if !isBlank(v) {
			return v
		}
	}
	return Null
}

func erbTry(main func() Value, fallback func() Value) (result Value) {
	defer func() {
		if r := recover(); r != nil {
			result = fallback()
		}
	}()
	return main()
}

func erbIsError(main func() Value) (result Value) {
	defer func() {
		if r := recover(); r != nil {
			result = vB(true)
		}
	}()
	main()
	return vB(false)
}

// ───────────────────────────── strings ─────────────────────────────

func textOperand(v Value, fn string) string {
	if !truthy(v) {
		return ""
	}
	if v.K != KStr {
		panic(fmt.Sprintf("%s expects text, got %s", fn, pyRepr(v)))
	}
	return v.S
}

func countOperand(v Value, fn string) int {
	if !truthy(v) {
		return 0
	}
	if v.K != KNum || !v.IsInt {
		panic(fmt.Sprintf("%s expects an integer count, got %s", fn, pyRepr(v)))
	}
	return int(v.N)
}

func erbLower(v Value) Value { return vS(strings.ToLower(textOperand(v, "LOWER"))) }
func erbUpper(v Value) Value { return vS(strings.ToUpper(textOperand(v, "UPPER"))) }
func erbTrim(v Value) Value  { return vS(strings.Trim(textOperand(v, "TRIM"), " ")) }

func erbLen(v Value) Value { return vI(utf8.RuneCountInString(textOperand(v, "LEN"))) }

func erbLeft(text, count Value) Value {
	r := []rune(textOperand(text, "LEFT"))
	n := countOperand(count, "LEFT")
	return vS(string(pySlice(r, 0, n)))
}

func erbRight(text, count Value) Value {
	r := []rune(textOperand(text, "RIGHT"))
	n := countOperand(count, "RIGHT")
	if n <= 0 {
		return vS("")
	}
	return vS(string(pySlice(r, len(r)-n, len(r))))
}

func erbMid(text, start, count Value) Value {
	r := []rune(textOperand(text, "MID"))
	n := countOperand(count, "MID")
	s := 1
	if truthy(start) {
		s = int(numOrZero(start).N)
	}
	i := s - 1
	if i < 0 {
		i = 0
	}
	if n <= 0 {
		return vS("")
	}
	return vS(string(pySlice(r, i, i+n)))
}

// pySlice clamps like a Python slice with non-negative-normalised bounds.
func pySlice(r []rune, lo, hi int) []rune {
	if hi < 0 {
		hi += len(r)
		if hi < 0 {
			hi = 0
		}
	}
	if lo < 0 {
		lo = 0
	}
	if hi > len(r) {
		hi = len(r)
	}
	if lo > hi {
		return nil
	}
	return r[lo:hi]
}

func erbSubstitute(text, old, new Value) Value {
	if old.K != KStr || new.K != KStr {
		panic("SUBSTITUTE expects text arguments")
	}
	return vS(strings.ReplaceAll(textOperand(text, "SUBSTITUTE"), old.S, new.S))
}

func erbFind(needle, haystack Value) Value {
	if needle.K == KNull || haystack.K == KNull {
		return Null
	}
	h, n := pyStr(haystack), pyStr(needle)
	i := strings.Index(h, n)
	if i < 0 {
		return vI(0)
	}
	return vI(utf8.RuneCountInString(h[:i]) + 1)
}

func erbCast(v Value) Value {
	if !truthy(v) {
		return vS("")
	}
	return vS(pyStr(v))
}

// ───────────────────────────── dates ─────────────────────────────

var isoPattern = regexp.MustCompile(
	`^(\d{4})-(\d{2})-(\d{2})(?:[T ](\d{2})(?::(\d{2})(?::(\d{2})(?:[.,](\d+))?)?)?)?\s*(Z|[+-]\d{2}(?::?\d{2}(?::?\d{2})?)?)?$`)

// parseISO accepts what datetime.fromisoformat accepts for these values.
// hasZone reports whether the text carried a UTC offset.
func parseISO(s string) (t time.Time, hasZone bool, ok bool) {
	m := isoPattern.FindStringSubmatch(strings.TrimSpace(s))
	if m == nil {
		return time.Time{}, false, false
	}
	atoi := func(x string) int {
		if x == "" {
			return 0
		}
		n, _ := strconv.Atoi(x)
		return n
	}
	nanos := 0
	if m[7] != "" {
		frac := (m[7] + "000000000")[:9]
		nanos = atoi(frac)
	}
	loc := time.UTC
	if m[8] != "" {
		hasZone = true
		if m[8] != "Z" {
			sign := 1
			if m[8][0] == '-' {
				sign = -1
			}
			digits := strings.ReplaceAll(m[8][1:], ":", "")
			h := atoi(digits[:2])
			mi := 0
			if len(digits) >= 4 {
				mi = atoi(digits[2:4])
			}
			sec := 0
			if len(digits) >= 6 {
				sec = atoi(digits[4:6])
			}
			loc = time.FixedZone("", sign*(h*3600+mi*60+sec))
		}
	}
	t = time.Date(atoi(m[1]), time.Month(atoi(m[2])), atoi(m[3]), atoi(m[4]), atoi(m[5]), atoi(m[6]), nanos, loc)
	return t, hasZone, true
}

// wallClock drops the zone, keeping the local reading — the reference
// implementation strips tzinfo before any date arithmetic.
func wallClock(t time.Time) time.Time {
	return time.Date(t.Year(), t.Month(), t.Day(), t.Hour(), t.Minute(), t.Second(), t.Nanosecond(), time.UTC)
}

var datePrefix = regexp.MustCompile(`^\d{4}-\d{2}-\d{2}`)

// dateOrNone is erb_date_or_none: only text that starts YYYY-MM-DD is a date.
func dateOrNone(v Value) (time.Time, bool) {
	if v.K != KStr {
		return time.Time{}, false
	}
	s := strings.TrimSpace(v.S)
	if !datePrefix.MatchString(s) {
		return time.Time{}, false
	}
	if t, _, ok := parseISO(s); ok {
		return wallClock(t), true
	}
	if t, err := time.Parse("2006-01-02", s[:10]); err == nil {
		return t, true
	}
	return time.Time{}, false
}

// wallIn reinterprets t's wall-clock reading in loc.
func wallIn(t time.Time, loc *time.Location) time.Time {
	return time.Date(t.Year(), t.Month(), t.Day(), t.Hour(), t.Minute(), t.Second(), t.Nanosecond(), loc)
}

// inZone is the instant v names, expressed in erbTimezone. A value carrying no
// offset is taken to be in erbTimezone.
func inZone(v Value) time.Time {
	loc := erbZoneLoc()
	switch v.K {
	case KTime:
		if v.Zoned {
			return v.T.In(loc)
		}
		return wallIn(v.T, loc)
	case KStr:
		s := strings.TrimSpace(v.S)
		if t, hasZone, ok := parseISO(s); ok {
			if hasZone {
				return t.In(loc)
			}
			return wallIn(t, loc)
		}
		if len(s) >= 10 {
			if t, err := time.ParseInLocation("2006-01-02", s[:10], loc); err == nil {
				return t
			}
		}
	}
	panic(fmt.Sprintf("cannot read %s as a datetime", pyRepr(v)))
}

func truncToInt(q float64) int { return int(math.Trunc(q)) }

// erbDatetimeDiff is DATETIME_DIFF(end, start, unit) per erbDateDiff: hour,
// minute and second are exact elapsed (possibly fractional) in both modes;
// month and year are whole calendar months (AGE()) in both. calendar: day is
// calendar dates in erbTimezone subtracted, week is day/7 truncated. elapsed:
// day is seconds/86400 truncated, week seconds/604800 truncated.
func erbDatetimeDiff(end, start, unitValue Value) Value {
	unit := "day"
	if truthy(unitValue) {
		unit = strings.TrimRight(strings.ToLower(pyStr(unitValue)), "s")
	}
	if isBlank(end) || isBlank(start) {
		return Null
	}
	e := inZone(end)
	s := inZone(start)
	seconds := e.Sub(s).Seconds()
	calendar := erbParam("erbDateDiff") == "calendar"
	calendarDays := func() int {
		ed := time.Date(e.Year(), e.Month(), e.Day(), 0, 0, 0, 0, time.UTC)
		sd := time.Date(s.Year(), s.Month(), s.Day(), 0, 0, 0, 0, time.UTC)
		return int(math.Round(ed.Sub(sd).Hours() / 24))
	}
	switch unit {
	case "hour":
		return vF(seconds / 3600)
	case "minute":
		return vF(seconds / 60)
	case "second":
		return vF(seconds)
	case "day":
		if calendar {
			return vI(calendarDays())
		}
		return vI(truncToInt(seconds / 86400))
	case "week":
		if calendar {
			return vI(truncToInt(float64(calendarDays()) / 7))
		}
		return vI(truncToInt(seconds / 604800))
	case "month", "year":
		months := (e.Year()-s.Year())*12 + int(e.Month()) - int(s.Month())
		eClock := [4]int{e.Day(), e.Hour(), e.Minute(), e.Second()}
		sClock := [4]int{s.Day(), s.Hour(), s.Minute(), s.Second()}
		for i := 0; i < 4; i++ {
			if eClock[i] != sClock[i] {
				if eClock[i] < sClock[i] {
					months--
				}
				break
			}
		}
		if unit == "month" {
			return vI(months)
		}
		return vI(truncToInt(float64(months) / 12))
	}
	panic(fmt.Sprintf("DATETIME_DIFF: unsupported unit %q", unit))
}

// erbNow is NOW()/TODAY(), an instant in erbTimezone. FORMULA_NOW pins it.
func erbNow() Value {
	if override := os.Getenv("FORMULA_NOW"); override != "" {
		if _, _, ok := parseISO(override); !ok {
			panic(fmt.Sprintf("FORMULA_NOW %q is not an ISO-8601 datetime", override))
		}
		return vTZ(inZone(vS(override)))
	}
	return vTZ(time.Now().In(erbZoneLoc()))
}

var dateOnlyText = regexp.MustCompile(`^\d{4}-\d{2}-\d{2}$`)

// erbDatetimeText renders a datetime into text per erbDateTimeText, in
// erbTimezone: iso8601 = 2026-04-03T14:00:00+00:00, sql = 2026-04-03 14:00:00+00.
// A date-only value renders as YYYY-MM-DD; a blank as "".
func erbDatetimeText(v Value) Value {
	if isBlank(v) {
		return vS("")
	}
	if v.K == KStr && dateOnlyText.MatchString(strings.TrimSpace(v.S)) {
		return vS(strings.TrimSpace(v.S))
	}
	if v.K != KTime && v.K != KStr {
		panic(fmt.Sprintf("cannot render %s as a datetime", pyRepr(v)))
	}
	t := inZone(v)
	iso := erbParam("erbDateTimeText") == "iso8601"
	layout := "2006-01-02 15:04:05"
	if iso {
		layout = "2006-01-02T15:04:05"
	}
	s := t.Format(layout)
	if t.Nanosecond() != 0 {
		s += strings.TrimRight(fmt.Sprintf(".%06d", t.Nanosecond()/1000), "0")
	}
	_, offset := t.Zone()
	sign := "+"
	if offset < 0 {
		sign = "-"
		offset = -offset
	}
	if iso {
		s += fmt.Sprintf("%s%02d:%02d", sign, offset/3600, (offset%3600)/60)
	} else {
		s += fmt.Sprintf("%s%02d", sign, offset/3600)
		if m := (offset % 3600) / 60; m != 0 {
			s += fmt.Sprintf(":%02d", m)
		}
	}
	return vS(s)
}

// erbText is a non-datetime CONCAT / `&` operand as text: a blank contributes
// nothing, a number renders in its shortest exact form with no trailing .0, a
// boolean as true/false, a datetime per erbDateTimeText.
func erbText(v Value) Value {
	switch v.K {
	case KNull:
		return vS("")
	case KBool:
		if v.B {
			return vS("true")
		}
		return vS("false")
	case KNum:
		if v.IsInt || (v.N == math.Trunc(v.N) && math.Abs(v.N) < 1e15) {
			return vS(strconv.FormatInt(int64(v.N), 10))
		}
		return vS(pyFloatRepr(v.N))
	case KTime:
		return erbDatetimeText(v)
	}
	return vS(v.S)
}

// ───────────────────────────── records & tables ─────────────────────────────

// Record is one row of a generated table struct.
type Record interface {
	erbGet(field string) Value
	erbSet(field string, v Value)
	erbLoad(row map[string]any)
	erbComputeCalculations()
	erbErrors() map[string]string
	erbResetErrors()
	erbAggregate(name string) Value
	erbSetAggregate(name string, v Value)
}

// LookupSpec is =INDEX(Target!{{Return}}, MATCH({{Key}}, Target!{{Match}}, 0))
// or =LOOKUP(Target!{{Return}}, {{Key}}, Target!{{Pk}}).
type LookupSpec struct {
	Field  string
	Target string
	Return string
	Key    string
	Match  string
	Error  string
}

// Criterion is one (range, criteria) pair of a COUNTIFS-family call.
type Criterion struct {
	Range   string
	Kind    string // "field" | "literal" | "op"
	Field   string
	Literal Value
	Op      string
}

// AggregateSpec is a COUNTIFS / SUMIFS / AVERAGEIFS / MINIFS / MAXIFS call, a
// bare SUM/AVERAGE/COUNT/MIN/MAX over a table, or — with Scalar set — a
// scalar formula wrapped around such calls, computed into placeholders first.
type AggregateSpec struct {
	Field      string
	Op         string // COUNTIFS | SUM | AVERAGE | COUNT | MIN | MAX | COMPOSITE
	Table      string
	Target     string
	Criteria   []Criterion
	Suffix     func(Value) Value
	Parts      []AggregateSpec
	Scalar     func(r Record) Value
	Error      string
}

// ClosureSpec materializes vw_<entity>_closure. Filter, when set, names the
// edge table field an edge must hold TRUE in to take part (the closure field's
// EdgeFilterColumn); the view is then vw_<entity>_closure_where_<filter>.
type ClosureSpec struct {
	View   string
	Source string
	From   string
	To     string
	Filter string
}

type TableSpec struct {
	Name         string
	File         string
	RulebookRows int // rows the rulebook itself holds for this table
	New          func() Record
	Lookups      []LookupSpec
	Aggregations []AggregateSpec
}

type dataset struct {
	specs    []TableSpec
	byFile   map[string]*TableSpec
	rows     map[string][]Record
	closures map[string][]Record
	blank    map[string]Value
}

func (d *dataset) tableRows(file string) []Record {
	if rows, ok := d.closures[file]; ok {
		return rows
	}
	rows, ok := d.rows[file]
	if !ok {
		if spec, known := d.byFile[file]; known {
			panic(fmt.Sprintf("the rulebook holds %d %s rows but blank-tests has no %s.json — regenerate the test fixtures", spec.RulebookRows, spec.Name, file))
		}
		panic(fmt.Sprintf("no table %q", file))
	}
	return rows
}

// ───────────────────────────── closure rows ─────────────────────────────

type closureRecord struct{ vals map[string]Value }

func (c *closureRecord) erbGet(field string) Value {
	if v, ok := c.vals[field]; ok {
		return v
	}
	return Null
}
func (c *closureRecord) erbSet(string, Value)            {}
func (c *closureRecord) erbLoad(map[string]any)          {}
func (c *closureRecord) erbComputeCalculations()         {}
func (c *closureRecord) erbErrors() map[string]string    { return nil }
func (c *closureRecord) erbResetErrors()                 {}
func (c *closureRecord) erbAggregate(string) Value       { return Null }
func (c *closureRecord) erbSetAggregate(string, Value)   {}

// isTrueValue: an edge takes part in a filtered closure only when its filter
// field holds boolean TRUE; NULL, FALSE and anything non-boolean exclude it.
func isTrueValue(v Value) bool { return v.K == KBool && v.B }

func materializeClosure(d *dataset, spec ClosureSpec) []Record {
	source := d.tableRows(spec.Source)
	edges := make([]ClosureEdge, 0, len(source))
	for _, r := range source {
		if spec.Filter != "" && !isTrueValue(r.erbGet(spec.Filter)) {
			continue
		}
		from, to := r.erbGet(spec.From), r.erbGet(spec.To)
		if isBlank(from) || isBlank(to) {
			continue
		}
		edges = append(edges, ClosureEdge{From: pyStr(from), To: pyStr(to)})
	}
	pairs := ComputeClosureRelation(edges)
	sort.Slice(pairs, func(i, j int) bool {
		if pairs[i].FromID != pairs[j].FromID {
			return pairs[i].FromID < pairs[j].FromID
		}
		return pairs[i].ToID < pairs[j].ToID
	})
	out := make([]Record, 0, len(pairs))
	for _, p := range pairs {
		out = append(out, &closureRecord{vals: map[string]Value{
			"from_id": vS(p.FromID), "to_id": vS(p.ToID),
			"hop_distance": vI(p.HopDistance), "is_inferred": vB(p.IsInferred),
		}})
	}
	return out
}

// ───────────────────────────── lookups ─────────────────────────────

func lookupKey(v Value) string {
	switch v.K {
	case KStr:
		return "s:" + v.S
	case KNum:
		return "n:" + strconv.FormatFloat(v.N, 'g', -1, 64)
	case KBool:
		if v.B {
			return "n:1"
		}
		return "n:0"
	case KTime:
		return "t:" + v.T.String()
	}
	return ""
}

func (d *dataset) computeLookup(spec LookupSpec, owner string, rows []Record) {
	defer func() {
		if p := recover(); p != nil {
			for _, r := range rows {
				r.erbSet(spec.Field, Null)
				r.erbErrors()[spec.Field] = fmt.Sprint(p)
			}
		}
	}()
	if spec.Error != "" {
		for _, r := range rows {
			r.erbSet(spec.Field, Null)
			r.erbErrors()[spec.Field] = spec.Error
		}
		return
	}
	index := map[string]Value{}
	for _, t := range d.tableRows(spec.Target) {
		pk := t.erbGet(spec.Match)
		if pk.K == KNull {
			continue
		}
		index[lookupKey(pk)] = t.erbGet(spec.Return)
	}
	noMatch := d.blankJoinValue(spec.Target, spec.Return)
	for _, r := range rows {
		key := r.erbGet(spec.Key)
		if v, ok := index[lookupKey(key)]; ok && key.K != KNull {
			r.erbSet(spec.Field, v)
		} else {
			r.erbSet(spec.Field, noMatch)
		}
	}
}

// blankJoinValue is what a lookup renders when nothing matches: the joined
// table's field evaluated against an all-NULL row, as a LEFT JOIN evaluates a
// calculated column — a formula built on literal text still yields text.
func (d *dataset) blankJoinValue(table, field string) (result Value) {
	cacheKey := table + "." + field
	if v, ok := d.blank[cacheKey]; ok {
		return v
	}
	spec, ok := d.byFile[table]
	if !ok {
		d.blank[cacheKey] = Null
		return Null
	}
	for _, l := range spec.Lookups {
		if l.Field == field {
			if l.Error != "" {
				return Null
			}
			v := d.blankJoinValue(l.Target, l.Return)
			d.blank[cacheKey] = v
			return v
		}
	}
	defer func() {
		if r := recover(); r != nil {
			result = Null
		}
		d.blank[cacheKey] = result
	}()
	blank := spec.New()
	blank.erbResetErrors()
	blank.erbComputeCalculations()
	return blank.erbGet(field)
}

// ───────────────────────────── aggregations ─────────────────────────────

func blankAsEmpty(v Value) Value {
	if v.K == KNull {
		return vS("")
	}
	return v
}

func criterionMatches(c Criterion, row Record, record Record) bool {
	cell := row.erbGet(c.Range)
	switch c.Kind {
	case "field":
		return pyEqual(blankAsEmpty(cell), blankAsEmpty(record.erbGet(c.Field)))
	case "op":
		if isBlank(cell) {
			return false
		}
		var cellNum float64
		switch cell.K {
		case KBool:
			cellNum = boolNum(cell.B)
		default:
			n, ok := toNumber(cell)
			if !ok {
				return false
			}
			cellNum = n.N
		}
		target, ok := toNumber(c.Literal)
		if !ok {
			return false
		}
		switch c.Op {
		case ">":
			return cellNum > target.N
		case ">=":
			return cellNum >= target.N
		case "<":
			return cellNum < target.N
		case "<=":
			return cellNum <= target.N
		case "<>":
			return cellNum != target.N
		}
		return cellNum == target.N
	}
	return pyEqual(cell, c.Literal)
}

func rowMatches(criteria []Criterion, row Record, record Record) bool {
	for _, c := range criteria {
		if !criterionMatches(c, row, record) {
			return false
		}
	}
	return true
}

func (d *dataset) aggregateValue(spec AggregateSpec, record Record) Value {
	if spec.Op == "COMPOSITE" {
		for _, part := range spec.Parts {
			record.erbSetAggregate(part.Field, d.aggregateValue(part, record))
		}
		return spec.Scalar(record)
	}
	rows := d.tableRows(spec.Table)
	matching := make([]Record, 0)
	for _, row := range rows {
		if rowMatches(spec.Criteria, row, record) {
			matching = append(matching, row)
		}
	}
	switch spec.Op {
	case "COUNTIFS", "COUNT":
		return vI(len(matching))
	case "MIN", "MAX":
		values := make([]Value, 0, len(matching))
		for _, row := range matching {
			if v := row.erbGet(spec.Target); !isBlank(v) {
				values = append(values, v)
			}
		}
		if len(values) == 0 {
			return Null
		}
		best := values[0]
		for _, v := range values[1:] {
			// min()/max() keep the first of equal extremes.
			if (spec.Op == "MIN" && pyLess(v, best, spec.Op)) || (spec.Op == "MAX" && pyLess(best, v, spec.Op)) {
				best = v
			}
		}
		return best
	}
	numbers := make([]Value, 0, len(matching))
	for _, row := range matching {
		raw := row.erbGet(spec.Target)
		if isBlank(raw) {
			continue
		}
		var value Value
		switch raw.K {
		case KBool:
			value = vF(boolNum(raw.B))
		default:
			n, ok := toNumber(raw)
			if !ok {
				continue
			}
			value = vF(n.N)
		}
		if spec.Suffix != nil {
			value = spec.Suffix(value)
		}
		numbers = append(numbers, value)
	}
	if spec.Op == "SUM" {
		if len(numbers) == 0 {
			return vI(0)
		}
		total := vF(0)
		for _, n := range numbers {
			total = vF(total.N + n.N)
		}
		return total
	}
	if len(numbers) == 0 {
		return Null
	}
	total := 0.0
	for _, n := range numbers {
		total += n.N
	}
	return vF(total / float64(len(numbers)))
}

// pyLess is Python < for the values MIN/MAX rollups compare.
func pyLess(a, b Value, fn string) bool {
	an, aok := numericLike(a)
	bn, bok := numericLike(b)
	switch {
	case aok && bok:
		return an < bn
	case a.K == KStr && b.K == KStr:
		return a.S < b.S
	case a.K == KTime && b.K == KTime:
		return a.T.Before(b.T)
	}
	panic(fmt.Sprintf("%s over values that cannot be ordered: %s and %s", fn, pyRepr(a), pyRepr(b)))
}

func (d *dataset) computeAggregation(spec AggregateSpec, rows []Record) {
	for _, r := range rows {
		if spec.Error != "" {
			r.erbSet(spec.Field, Null)
			r.erbErrors()[spec.Field] = spec.Error
			continue
		}
		func() {
			defer func() {
				if p := recover(); p != nil {
					r.erbSet(spec.Field, Null)
					r.erbErrors()[spec.Field] = fmt.Sprint(p)
				}
			}()
			r.erbSet(spec.Field, d.aggregateValue(spec, r))
		}()
	}
}

// calcGuard runs one calculated field; a failure nulls that field alone and
// is recorded on the row, never swallowed.
func calcGuard(r Record, field string, compute func()) {
	defer func() {
		if p := recover(); p != nil {
			r.erbSet(field, Null)
			r.erbErrors()[field] = fmt.Sprint(p)
		}
	}()
	compute()
}

// suffixValue lets a SUMIFS target suffix (`Table!{{Field}}*0+1`) compile as a
// formula over the row's value.
type suffixValue struct{ v Value }

func (s *suffixValue) erbAggregate(string) Value { return s.v }

const erbPi = math.Pi

// ───────────────────────────── the run ─────────────────────────────

// loadRecords reads a JSON array of rows into records made by newRecord.
func loadRecords(path string, newRecord func() Record) (records []Record, err error) {
	defer func() {
		if p := recover(); p != nil {
			err = fmt.Errorf("%v", p)
		}
	}()
	return loadTable(path, &TableSpec{New: newRecord}), nil
}

func loadTable(path string, spec *TableSpec) []Record {
	data, err := os.ReadFile(path)
	if err != nil {
		panic(fmt.Sprintf("reading %s: %v", path, err))
	}
	decoder := json.NewDecoder(strings.NewReader(string(data)))
	decoder.UseNumber()
	var raw []map[string]any
	if err := decoder.Decode(&raw); err != nil {
		panic(fmt.Sprintf("parsing %s: %v", path, err))
	}
	rows := make([]Record, 0, len(raw))
	for _, m := range raw {
		r := spec.New()
		r.erbLoad(m)
		rows = append(rows, r)
	}
	return rows
}

func snapshot(d *dataset) string {
	var b strings.Builder
	for _, spec := range d.specs {
		rows, ok := d.rows[spec.File]
		if !ok {
			continue
		}
		data, err := json.Marshal(rows)
		if err != nil {
			panic(fmt.Sprintf("serializing %s: %v", spec.File, err))
		}
		b.Write(data)
	}
	return b.String()
}

// erbRun loads every table's blank test rows, computes lookups, aggregations
// and calculations across the whole dataset until nothing changes, and
// writes each table's answers.
//
// Tables depend on each other in both directions — one table's lookup reads
// another's aggregation, which counts rows of the first — so no table order
// works; a field's inputs settle one pass before the field does. The pass
// bound is the number of computed fields, which a dependency chain cannot
// exceed without a cycle.
func erbRun(specs []TableSpec, closures []ClosureSpec) {
	testing := os.Getenv("ERB_TESTING_DIR")
	if testing == "" {
		fmt.Fprintln(os.Stderr, "FATAL: ERB_TESTING_DIR is not set; point it at the domain's testing/ directory.")
		os.Exit(1)
	}
	substrate := os.Getenv("ERB_SUBSTRATE_NAME")
	if substrate == "" {
		fmt.Fprintln(os.Stderr, "FATAL: ERB_SUBSTRATE_NAME is not set; the harness must name the substrate whose answers these are.")
		os.Exit(1)
	}
	blankDir := filepath.Join(testing, "blank-tests")
	answersDir := filepath.Join(testing, substrate, "test-answers")
	if err := os.MkdirAll(answersDir, 0o755); err != nil {
		fmt.Fprintf(os.Stderr, "FATAL: creating %s: %v\n", answersDir, err)
		os.Exit(1)
	}

	d := &dataset{specs: specs, byFile: map[string]*TableSpec{}, rows: map[string][]Record{},
		closures: map[string][]Record{}, blank: map[string]Value{}}
	computed := 0
	graded := map[string]bool{}
	for i := range specs {
		spec := &specs[i]
		d.byFile[spec.File] = spec
		path := filepath.Join(blankDir, spec.File+".json")
		if _, err := os.Stat(path); err != nil {
			// No fixture: a table the rulebook declares empty has no rows to
			// test, and reads as empty. One the rulebook holds rows for stays
			// unloaded, so anything that reads it fails naming the gap.
			if spec.RulebookRows == 0 {
				d.rows[spec.File] = []Record{}
			}
			continue
		}
		d.rows[spec.File] = loadTable(path, spec)
		graded[spec.File] = true
		computed += len(spec.Lookups) + len(spec.Aggregations)
	}
	if len(graded) == 0 {
		fmt.Fprintf(os.Stderr, "FATAL: no blank tests for any table under %s\n", blankDir)
		os.Exit(1)
	}
	for _, c := range closures {
		if _, ok := d.rows[c.Source]; !ok {
			fmt.Fprintf(os.Stderr, "FATAL: closure %s needs %s.json in %s\n", c.View, c.Source, blankDir)
			os.Exit(1)
		}
		d.closures[c.View] = materializeClosure(d, c)
		fmt.Printf("  -> %s: %d pairs\n", c.View, len(d.closures[c.View]))
	}

	maxPasses := computed + calculatedFieldCount + 2
	previous := ""
	passes := 0
	for {
		passes++
		if passes > maxPasses {
			fmt.Fprintf(os.Stderr, "FATAL: the dataset did not settle within %d passes — the field dependencies are cyclic.\n", maxPasses)
			os.Exit(1)
		}
		d.blank = map[string]Value{}
		// A filtered closure's filter may be a derived field, so it is
		// re-read from the current rows on every pass and settles with them.
		for _, c := range closures {
			if c.Filter != "" {
				d.closures[c.View] = materializeClosure(d, c)
			}
		}
		for _, spec := range specs {
			rows, ok := d.rows[spec.File]
			if !ok {
				continue
			}
			for _, r := range rows {
				r.erbResetErrors()
			}
			for _, l := range spec.Lookups {
				d.computeLookup(l, spec.File, rows)
			}
			for _, a := range spec.Aggregations {
				d.computeAggregation(a, rows)
			}
			for _, r := range rows {
				r.erbComputeCalculations()
			}
		}
		current := snapshot(d)
		if current == previous {
			break
		}
		previous = current
	}
	fmt.Printf("Go substrate: dataset settled after %d passes\n", passes)

	total := 0
	names := make([]string, 0, len(graded))
	for name := range graded {
		names = append(names, name)
	}
	sort.Strings(names)
	for _, name := range names {
		rows := d.rows[name]
		data, err := json.MarshalIndent(rows, "", "  ")
		if err != nil {
			fmt.Fprintf(os.Stderr, "FATAL: serializing %s: %v\n", name, err)
			os.Exit(1)
		}
		if err := os.WriteFile(filepath.Join(answersDir, name+".json"), data, 0o644); err != nil {
			fmt.Fprintf(os.Stderr, "FATAL: writing %s: %v\n", name, err)
			os.Exit(1)
		}
		total += len(rows)
		fmt.Printf("  -> %s: %d records\n", name, len(rows))
	}
	fmt.Printf("Go substrate: wrote %d records across %d tables to %s\n", total, len(names), answersDir)
}
