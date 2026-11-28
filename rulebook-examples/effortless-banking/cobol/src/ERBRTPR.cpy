      *> =================================================================
      *> ERBRTPR - procedures of the ERB COBOL runtime.
      *>
      *> HAND-WRITTEN, NOT GENERATED. COPYed at the end of the generated
      *> rules module ERBRULES's PROCEDURE DIVISION. It is the COBOL
      *> counterpart of rulebook-to-typescript's erb_runtime.ts: when a rule
      *> changes there, it changes here. Its storage is ERBRTWS; it also
      *> reads the caller's control block ERB-CONTROL (ERB-NOW, ERB-CHARSET)
      *> and reports a fatal condition there (ERB-RC, ERB-MESSAGE).
      *>
      *> Conventions: operands are popped from the value stack into RA, RB,
      *> RC; the result is built in RR and pushed. A failure calls RT-FAIL
      *> (message in WS-ERR-MSG) and pushes NULL.
      *>
      *> Written in free format; rulebook-to-cobol re-flows it into fixed
      *> format (columns 8-72) for cobolDialect=ibm.
      *> =================================================================

      *> ----------------------------- stack -----------------------------
       RT-POP-A.
           PERFORM RT-CP-VSP-RA
           SUBTRACT 1 FROM VSP
           .
       RT-POP-B.
           PERFORM RT-CP-VSP-RB
           SUBTRACT 1 FROM VSP
           .
       RT-POP-C.
           PERFORM RT-CP-VSP-RC
           SUBTRACT 1 FROM VSP
           .
       RT-PUSH-R.
           IF VSP >= 128
               MOVE "value stack overflow (formula nested too deeply)"
                   TO WS-ERR-MSG
               PERFORM RT-FATAL
           END-IF
           ADD 1 TO VSP
           PERFORM RT-CP-RR-VSP
           .
       RT-PUSH-A.
           PERFORM RT-CP-RA-RR
           PERFORM RT-PUSH-R
           .
       RT-R-NULL.
           MOVE 0 TO RR-K RR-B RR-I RR-L RR-F
           MOVE 0 TO RR-N
           .
       RT-R-BOOL.
           PERFORM RT-R-NULL
           MOVE 1 TO RR-K
           MOVE WS-T TO RR-B
           .
       RT-R-EMPTY.
           PERFORM RT-R-NULL
           MOVE 3 TO RR-K
           .
       RT-R-INT.
      *>   RR := the integer WS-NUM-T
           PERFORM RT-R-NULL
           MOVE 2 TO RR-K
           MOVE 1 TO RR-I
           MOVE WS-NUM-T TO RR-N
           .
       RT-R-FLOAT.
           PERFORM RT-R-NULL
           MOVE 2 TO RR-K
           MOVE 0 TO RR-I
           MOVE WS-NUM-T TO RR-N
           .
       RT-PUSH-DBL.
      *>   Pushes the float8 WS-DBL-T: D keeps it whole, N is it rounded
      *>   to 12 places (what a field stores and what N-only routines use).
           PERFORM RT-R-NULL
           COMPUTE RR-N ROUNDED = WS-DBL-T
               ON SIZE ERROR
                   MOVE "number too large" TO WS-ERR-MSG
                   PERFORM RT-FAIL-PUSH-NULL
                   EXIT PARAGRAPH
           END-COMPUTE
           MOVE 2 TO RR-K
           MOVE 1 TO RR-F
           MOVE WS-DBL-T TO RR-D
           PERFORM RT-PUSH-R
           .
       RT-DBL-AB.
      *>   After RT-NUMZ-B / RT-NUMZ-A: WS-DBL-A, WS-DBL-B := the operands
      *>   as float8 (D when the value is one, else N); WS-FLT := 1 when
      *>   either is a float8. Postgres types PI() and TAN as float8 and
      *>   keeps + - * / on one in float8; so does this runtime.
           MOVE 0 TO WS-FLT
           IF RA-K = 2 AND RA-F = 1
               MOVE RA-D TO WS-DBL-A
               MOVE 1 TO WS-FLT
           ELSE
               MOVE WS-NUM-A TO WS-DBL-A
           END-IF
           IF RB-K = 2 AND RB-F = 1
               MOVE RB-D TO WS-DBL-B
               MOVE 1 TO WS-FLT
           ELSE
               MOVE WS-NUM-B TO WS-DBL-B
           END-IF
           .
       RT-R-TMP.
      *>   RR := the text WS-TMP(1:WS-TMP-L)
           PERFORM RT-R-EMPTY
           MOVE WS-TMP-L TO RR-L
           IF WS-TMP-L > 0
               MOVE WS-TMP(1:WS-TMP-L) TO RR-S(1:WS-TMP-L)
           END-IF
           .
       RT-PUSH-NULL.
           PERFORM RT-R-NULL
           PERFORM RT-PUSH-R
           .
       RT-PUSH-TRUE.
           MOVE 1 TO WS-T
           PERFORM RT-R-BOOL
           PERFORM RT-PUSH-R
           .
       RT-PUSH-FALSE.
           MOVE 0 TO WS-T
           PERFORM RT-R-BOOL
           PERFORM RT-PUSH-R
           .
       RT-PUSH-BOOL.
           PERFORM RT-R-BOOL
           PERFORM RT-PUSH-R
           .
       RT-PUSH-LIT-STR.
           PERFORM RT-R-EMPTY
           MOVE WS-LIT-L TO RR-L
           IF WS-LIT-L > 0
               MOVE WS-LIT-S(1:WS-LIT-L) TO RR-S(1:WS-LIT-L)
           END-IF
           PERFORM RT-PUSH-R
           .
       RT-PUSH-LIT-NUM.
           PERFORM RT-R-NULL
           MOVE 2 TO RR-K
           MOVE WS-LIT-I TO RR-I
           MOVE WS-LIT-N TO RR-N
           PERFORM RT-PUSH-R
           .
       RT-PUSH-SV.
           PERFORM RT-CP-SV-RR
           PERFORM RT-PUSH-R
           .

      *> ----------------------------- failure -----------------------------
       RT-FAIL.
      *>   The caller has put the reason in WS-ERR-MSG. The first reason wins.
           IF WS-ERR = 0
               MOVE 1 TO WS-ERR
               MOVE WS-ERR-MSG TO WS-ERR-FIRST
           END-IF
           .
       RT-FAIL-PUSH-NULL.
           PERFORM RT-FAIL
           PERFORM RT-PUSH-NULL
           .
       RT-REPORT.
      *>   A field failed: report it (on the reporting pass) and leave NULL
      *>   on the stack in place of the value. The report is one SYSOUT
      *>   line: ERBERR|<table>|<field>|<row>|<reason>.
           IF WS-REPORT = 1
               ADD 1 TO WS-ERR-COUNT
               MOVE WS-CUR-ROW TO WS-CUR-ROW-D
               DISPLAY "ERBERR|" FUNCTION TRIM(WS-CUR-TABLE)
                   "|" FUNCTION TRIM(WS-CUR-FIELD)
                   "|" FUNCTION TRIM(WS-CUR-ROW-D)
                   "|" FUNCTION TRIM(WS-ERR-FIRST)
           END-IF
           IF VSP > 0
               MOVE 0 TO VS-K(VSP) VS-B(VSP) VS-I(VSP) VS-L(VSP)
               MOVE 0 TO VS-N(VSP) VS-F(VSP)
           ELSE
               PERFORM RT-PUSH-NULL
           END-IF
           MOVE 0 TO WS-ERR
           .
       RT-FATAL.
      *>   The data cannot be computed (capacity, a cyclic dataset). The
      *>   caller gets return code 8 and the reason in ERB-MESSAGE.
           MOVE 8 TO ERB-RC
           MOVE WS-ERR-MSG TO ERB-MESSAGE
           MOVE WS-ERR-COUNT TO ERB-ERRORS
           MOVE WS-PASS TO ERB-PASSES
           GOBACK
           .

      *> ----------------------------- truthiness -----------------------------
       RT-TRUTHY-A.
           EVALUATE RA-K
               WHEN 1 MOVE RA-B TO WS-T
               WHEN 2 IF RA-N NOT = 0 MOVE 1 TO WS-T ELSE MOVE 0 TO WS-T END-IF
               WHEN 3 IF RA-L > 0 MOVE 1 TO WS-T ELSE MOVE 0 TO WS-T END-IF
               WHEN OTHER MOVE 0 TO WS-T
           END-EVALUATE
           .
       RT-TRUTHY.
      *>   Pops a value; WS-T := its truthiness (for IF).
           PERFORM RT-POP-A
           PERFORM RT-TRUTHY-A
           .
       RT-BLANK-A.
      *>   WS-T := 1 when RA is NULL or empty text.
           IF RA-K = 0 OR (RA-K = 3 AND RA-L = 0)
               MOVE 1 TO WS-T
           ELSE
               MOVE 0 TO WS-T
           END-IF
           .
       RT-NULLIF.
           IF VS-K(VSP) = 3 AND VS-L(VSP) = 0
               MOVE 0 TO VS-K(VSP)
           END-IF
           .
       RT-BOOL3.
           IF VS-K(VSP) NOT = 0 AND VS-K(VSP) NOT = 1
               MOVE 1 TO VS-K(VSP)
               MOVE 0 TO VS-B(VSP) VS-L(VSP) VS-I(VSP)
           END-IF
           .
       RT-ISTRUE.
           PERFORM RT-POP-A
           IF RA-K = 1 AND RA-B = 1 MOVE 1 TO WS-T ELSE MOVE 0 TO WS-T END-IF
           PERFORM RT-PUSH-BOOL
           .
       RT-HASVALUE.
           PERFORM RT-POP-A
           PERFORM RT-BLANK-A
           IF WS-T = 1 MOVE 0 TO WS-T ELSE MOVE 1 TO WS-T END-IF
           PERFORM RT-PUSH-BOOL
           .
       RT-ISBLANK.
           PERFORM RT-POP-A
           PERFORM RT-BLANK-A
           PERFORM RT-PUSH-BOOL
           .
       RT-ISNOTBLANK.
           PERFORM RT-HASVALUE
           .
       RT-NOT.
      *>   erbBlankLogic=coerce: NOT(blank) is TRUE.
           PERFORM RT-POP-A
           IF RA-K = 0
               MOVE 1 TO WS-T
           ELSE
               PERFORM RT-TRUTHY-A
               IF WS-T = 1 MOVE 0 TO WS-T ELSE MOVE 1 TO WS-T END-IF
           END-IF
           PERFORM RT-PUSH-BOOL
           .
       RT-AND.
      *>   WS-NARGS conditions; coerce: TRUE only when every one is TRUE.
           MOVE 1 TO WS-ANDV
           PERFORM WS-NARGS TIMES
               PERFORM RT-POP-A
               IF NOT (RA-K = 1 AND RA-B = 1)
                   MOVE 0 TO WS-ANDV
               END-IF
           END-PERFORM
           MOVE WS-ANDV TO WS-T
           PERFORM RT-PUSH-BOOL
           .
       RT-OR.
           MOVE 0 TO WS-ANDV
           PERFORM WS-NARGS TIMES
               PERFORM RT-POP-A
               IF RA-K = 1 AND RA-B = 1
                   MOVE 1 TO WS-ANDV
               END-IF
           END-PERFORM
           MOVE WS-ANDV TO WS-T
           PERFORM RT-PUSH-BOOL
           .

      *> ----------------------------- numbers -----------------------------
       RT-PARSE-NUM.
      *>   WS-TMP(1:WS-TMP-L) as a number: WS-OK, WS-NUM-T, WS-ALLINT.
           MOVE 0 TO WS-OK WS-ALLINT
           MOVE 1 TO WS-P
           PERFORM UNTIL WS-P > WS-TMP-L OR WS-TMP(WS-P:1) NOT = SPACE
               ADD 1 TO WS-P
           END-PERFORM
           MOVE WS-TMP-L TO WS-Q
           PERFORM UNTIL WS-Q < WS-P OR WS-TMP(WS-Q:1) NOT = SPACE
               SUBTRACT 1 FROM WS-Q
           END-PERFORM
           IF WS-Q < WS-P
               EXIT PARAGRAPH
           END-IF
           MOVE 0 TO WS-EXP-SEEN WS-DOT-SEEN WS-DIG-SEEN
           MOVE WS-P TO WS-I
           IF WS-TMP(WS-I:1) = "+" OR WS-TMP(WS-I:1) = "-"
               ADD 1 TO WS-I
           END-IF
           PERFORM UNTIL WS-I > WS-Q
               MOVE WS-TMP(WS-I:1) TO WS-CH
               EVALUATE TRUE
                   WHEN WS-CH >= "0" AND WS-CH <= "9"
                       IF WS-EXP-SEEN = 0 MOVE 1 TO WS-DIG-SEEN END-IF
                       IF WS-EXP-SEEN = 1 MOVE 2 TO WS-EXP-SEEN END-IF
                   WHEN WS-CH = "." AND WS-DOT-SEEN = 0 AND WS-EXP-SEEN = 0
                       MOVE 1 TO WS-DOT-SEEN
                   WHEN (WS-CH = "e" OR WS-CH = "E") AND WS-EXP-SEEN = 0
                        AND WS-DIG-SEEN = 1
                       MOVE 1 TO WS-EXP-SEEN
                       IF WS-I < WS-Q
                          AND (WS-TMP(WS-I + 1:1) = "+" OR WS-TMP(WS-I + 1:1) = "-")
                           ADD 1 TO WS-I
                       END-IF
                   WHEN OTHER
                       EXIT PARAGRAPH
               END-EVALUATE
               ADD 1 TO WS-I
           END-PERFORM
           IF WS-DIG-SEEN = 0 OR WS-EXP-SEEN = 1
               EXIT PARAGRAPH
           END-IF
           COMPUTE WS-LEN = WS-Q - WS-P + 1
           IF WS-EXP-SEEN = 2
               COMPUTE WS-NUM-T = FUNCTION NUMVAL-F(WS-TMP(WS-P:WS-LEN))
                   ON SIZE ERROR EXIT PARAGRAPH
               END-COMPUTE
           ELSE
               COMPUTE WS-NUM-T = FUNCTION NUMVAL(WS-TMP(WS-P:WS-LEN))
                   ON SIZE ERROR EXIT PARAGRAPH
               END-COMPUTE
           END-IF
           IF WS-DOT-SEEN = 0 AND WS-EXP-SEEN = 0
               MOVE 1 TO WS-ALLINT
           END-IF
           MOVE 1 TO WS-OK
           .
       RT-TONUM-A.
      *>   toNumber(RA): a number, or numeric text. WS-OK, WS-NUM-T, WS-ALLINT.
           MOVE 0 TO WS-OK
           EVALUATE RA-K
               WHEN 2
                   MOVE RA-N TO WS-NUM-T
                   MOVE RA-I TO WS-ALLINT
                   MOVE 1 TO WS-OK
               WHEN 3
                   MOVE RA-L TO WS-TMP-L
                   IF RA-L > 0
                       MOVE RA-S(1:RA-L) TO WS-TMP(1:RA-L)
                       PERFORM RT-PARSE-NUM
                   END-IF
           END-EVALUATE
           .
       RT-NUMZ-A.
      *>   numOrZero(RA) -> WS-NUM-A, WS-ALLINT
           PERFORM RT-TONUM-A
           IF WS-OK = 0
               MOVE 0 TO WS-NUM-T
               MOVE 1 TO WS-ALLINT
           END-IF
           MOVE WS-NUM-T TO WS-NUM-A
           .
       RT-NUMZ-B.
      *>   numOrZero(RB) -> WS-NUM-B, WS-OK (the int flag) in WS-NEG
           PERFORM RT-CP-RA-RC
           PERFORM RT-CP-RB-RA
           PERFORM RT-NUMZ-A
           MOVE WS-NUM-A TO WS-NUM-B
           MOVE WS-ALLINT TO WS-NEG
           PERFORM RT-CP-RC-RA
           .
       RT-ORZERO-A.
      *>   orZero(RA): blank-ish -> 0; a number stays; anything else fails.
           PERFORM RT-TRUTHY-A
           IF WS-T = 0
               MOVE 0 TO WS-NUM-A
               MOVE 1 TO WS-ALLINT
               MOVE 1 TO WS-OK
           ELSE
               IF RA-K NOT = 2
                   MOVE "expected a number" TO WS-ERR-MSG
                   PERFORM RT-FAIL
                   MOVE 0 TO WS-OK
               ELSE
                   MOVE RA-N TO WS-NUM-A
                   MOVE RA-I TO WS-ALLINT
                   MOVE 1 TO WS-OK
               END-IF
           END-IF
           .
       RT-DATE-CHECK.
      *>   WS-T := 1 when exactly one of RA, RB is text that starts YYYY-MM-DD
      *>   (for subtraction, only RA); WS-K2 := 1 (RA) or 2 (RB), the date side.
           MOVE 0 TO WS-T WS-K2 WS-J
           IF RA-K = 3 AND RA-L >= 10
               IF RA-S(1:4) IS NUMERIC AND RA-S(5:1) = "-" AND RA-S(8:1) = "-"
                   MOVE 1 TO WS-K2
               END-IF
           END-IF
           IF RB-K = 3 AND RB-L >= 10
               IF RB-S(1:4) IS NUMERIC AND RB-S(5:1) = "-" AND RB-S(8:1) = "-"
                   MOVE 1 TO WS-J
               END-IF
           END-IF
           IF WS-K2 = 1 AND WS-J = 0
               MOVE 1 TO WS-T
           END-IF
           IF WS-MAXMODE = 1 AND WS-K2 = 0 AND WS-J = 1
               MOVE 1 TO WS-T
               MOVE 2 TO WS-K2
           END-IF
           .
       RT-ARITH-PREP.
      *>   Pops B then A; fails on date arithmetic; WS-NUM-A/WS-NUM-B and
      *>   WS-ALLINT := both operands are integers.
           PERFORM RT-POP-B
           PERFORM RT-POP-A
           MOVE 0 TO WS-MATCH
           PERFORM RT-DATE-CHECK
           IF WS-T = 1
      *>       A date plus/minus a day count is a date (erb_runtime.ts erbAdd);
      *>       WS-MATCH tells the caller the date result is already pushed.
               PERFORM RT-DATE-SHIFT
               MOVE 0 TO WS-OK
               EXIT PARAGRAPH
           END-IF
           PERFORM RT-NUMZ-B
           PERFORM RT-NUMZ-A
           IF WS-NEG = 0 MOVE 0 TO WS-ALLINT END-IF
           MOVE 1 TO WS-OK
           .
       RT-ADD.
           MOVE 1 TO WS-MAXMODE
           PERFORM RT-ARITH-PREP
           IF WS-OK = 0
               IF WS-MATCH = 0 PERFORM RT-PUSH-NULL END-IF
               EXIT PARAGRAPH
           END-IF
           PERFORM RT-DBL-AB
           IF WS-FLT = 1
               COMPUTE WS-DBL-T = WS-DBL-A + WS-DBL-B
               PERFORM RT-PUSH-DBL
               EXIT PARAGRAPH
           END-IF
           COMPUTE WS-NUM-T = WS-NUM-A + WS-NUM-B
               ON SIZE ERROR
                   MOVE "number too large" TO WS-ERR-MSG
                   PERFORM RT-FAIL-PUSH-NULL
                   EXIT PARAGRAPH
           END-COMPUTE
           PERFORM RT-R-FLOAT
           MOVE WS-ALLINT TO RR-I
           PERFORM RT-PUSH-R
           .
       RT-SUB.
           MOVE 0 TO WS-MAXMODE
           PERFORM RT-ARITH-PREP
           IF WS-OK = 0
               IF WS-MATCH = 0 PERFORM RT-PUSH-NULL END-IF
               EXIT PARAGRAPH
           END-IF
           PERFORM RT-DBL-AB
           IF WS-FLT = 1
               COMPUTE WS-DBL-T = WS-DBL-A - WS-DBL-B
               PERFORM RT-PUSH-DBL
               EXIT PARAGRAPH
           END-IF
           COMPUTE WS-NUM-T = WS-NUM-A - WS-NUM-B
               ON SIZE ERROR
                   MOVE "number too large" TO WS-ERR-MSG
                   PERFORM RT-FAIL-PUSH-NULL
                   EXIT PARAGRAPH
           END-COMPUTE
           PERFORM RT-R-FLOAT
           MOVE WS-ALLINT TO RR-I
           PERFORM RT-PUSH-R
           .
       RT-MUL.
           PERFORM RT-POP-B
           PERFORM RT-POP-A
           PERFORM RT-NUMZ-B
           PERFORM RT-NUMZ-A
           IF WS-NEG = 0 MOVE 0 TO WS-ALLINT END-IF
           PERFORM RT-DBL-AB
           IF WS-FLT = 1
               COMPUTE WS-DBL-T = WS-DBL-A * WS-DBL-B
               PERFORM RT-PUSH-DBL
               EXIT PARAGRAPH
           END-IF
           COMPUTE WS-NUM-T ROUNDED = WS-NUM-A * WS-NUM-B
               ON SIZE ERROR
                   MOVE "number too large" TO WS-ERR-MSG
                   PERFORM RT-FAIL-PUSH-NULL
                   EXIT PARAGRAPH
           END-COMPUTE
           PERFORM RT-R-FLOAT
           MOVE WS-ALLINT TO RR-I
           PERFORM RT-PUSH-R
           .
       RT-DIV.
           PERFORM RT-POP-B
           PERFORM RT-POP-A
           PERFORM RT-NUMZ-B
           PERFORM RT-NUMZ-A
           PERFORM RT-DBL-AB
           IF WS-FLT = 1
               IF WS-DBL-B = 0
                   PERFORM RT-PUSH-NULL
                   EXIT PARAGRAPH
               END-IF
               COMPUTE WS-DBL-T = WS-DBL-A / WS-DBL-B
               PERFORM RT-PUSH-DBL
               EXIT PARAGRAPH
           END-IF
           IF WS-NUM-B = 0
               PERFORM RT-PUSH-NULL
               EXIT PARAGRAPH
           END-IF
           COMPUTE WS-NUM-T ROUNDED = WS-NUM-A / WS-NUM-B
               ON SIZE ERROR
                   MOVE "number too large" TO WS-ERR-MSG
                   PERFORM RT-FAIL-PUSH-NULL
                   EXIT PARAGRAPH
           END-COMPUTE
           PERFORM RT-R-FLOAT
           PERFORM RT-PUSH-R
           .
       RT-NEG.
           PERFORM RT-POP-A
           PERFORM RT-TRUTHY-A
           IF WS-T = 0
               MOVE 0 TO WS-NUM-T
               PERFORM RT-R-INT
               PERFORM RT-PUSH-R
               EXIT PARAGRAPH
           END-IF
           IF RA-K NOT = 2
               MOVE "bad operand type for unary -" TO WS-ERR-MSG
               PERFORM RT-FAIL-PUSH-NULL
               EXIT PARAGRAPH
           END-IF
           COMPUTE WS-NUM-T = 0 - RA-N
           PERFORM RT-R-FLOAT
           MOVE RA-I TO RR-I
           PERFORM RT-PUSH-R
           .
       RT-INTEGER.
      *>   The oracle's ::integer cast on a field declared integer.
           PERFORM RT-POP-A
           IF RA-K = 0 OR RA-K = 1
               PERFORM RT-PUSH-A
               EXIT PARAGRAPH
           END-IF
           PERFORM RT-TONUM-A
           IF WS-OK = 0
               PERFORM RT-PUSH-A
               EXIT PARAGRAPH
           END-IF
           MOVE 0 TO WS-DIGITS
           PERFORM RT-ROUND-HALF-AWAY
           PERFORM RT-R-INT
           PERFORM RT-PUSH-R
           .
       RT-ROUND-HALF-AWAY.
      *>   WS-NUM-T := WS-NUM-T rounded half away from zero to WS-DIGITS places.
           COMPUTE WS-NUM-F = 10 ** WS-DIGITS
           IF WS-NUM-T < 0
               COMPUTE WS-NUM-T = 0 - FUNCTION INTEGER-PART(
                   (0 - WS-NUM-T) * WS-NUM-F + 0.5) / WS-NUM-F
                   ON SIZE ERROR CONTINUE
               END-COMPUTE
           ELSE
               COMPUTE WS-NUM-T = FUNCTION INTEGER-PART(
                   WS-NUM-T * WS-NUM-F + 0.5) / WS-NUM-F
                   ON SIZE ERROR CONTINUE
               END-COMPUTE
           END-IF
           .
       RT-ROUND-DIGITS.
      *>   RB as ROUND's digits -> WS-DIGITS
           PERFORM RT-CP-RA-RC
           PERFORM RT-CP-RB-RA
           PERFORM RT-TRUTHY-A
           IF WS-T = 0 AND RA-K NOT = 2
               MOVE 0 TO WS-DIGITS
               MOVE 1 TO WS-OK
           ELSE
               PERFORM RT-TONUM-A
               IF WS-OK = 0
                   MOVE "invalid ROUND digits" TO WS-ERR-MSG
                   PERFORM RT-FAIL
               ELSE
                   COMPUTE WS-DIGITS = FUNCTION INTEGER-PART(WS-NUM-T)
               END-IF
           END-IF
           PERFORM RT-CP-RC-RA
           .
       RT-ROUND.
           PERFORM RT-POP-B
           PERFORM RT-POP-A
           PERFORM RT-BLANK-A
           IF WS-T = 1 PERFORM RT-PUSH-NULL EXIT PARAGRAPH END-IF
           PERFORM RT-ROUND-DIGITS
           IF WS-OK = 0 PERFORM RT-PUSH-NULL EXIT PARAGRAPH END-IF
           PERFORM RT-TONUM-A
           IF WS-OK = 0
               MOVE "cannot ROUND a non-number" TO WS-ERR-MSG
               PERFORM RT-FAIL-PUSH-NULL
               EXIT PARAGRAPH
           END-IF
           PERFORM RT-ROUND-HALF-AWAY
           PERFORM RT-R-FLOAT
           PERFORM RT-PUSH-R
           .
       RT-ROUNDUP.
      *>   Toward +infinity for positive numbers; a negative is truncated.
           PERFORM RT-POP-B
           PERFORM RT-POP-A
           PERFORM RT-BLANK-A
           IF WS-T = 1 PERFORM RT-PUSH-NULL EXIT PARAGRAPH END-IF
           PERFORM RT-ROUND-DIGITS
           IF WS-OK = 0 PERFORM RT-PUSH-NULL EXIT PARAGRAPH END-IF
           PERFORM RT-TONUM-A
           IF WS-OK = 0
               MOVE "cannot ROUNDUP a non-number" TO WS-ERR-MSG
               PERFORM RT-FAIL-PUSH-NULL
               EXIT PARAGRAPH
           END-IF
           COMPUTE WS-NUM-F = 10 ** WS-DIGITS
           COMPUTE WS-NUM-A = WS-NUM-T * WS-NUM-F
           COMPUTE WS-NUM-B = FUNCTION INTEGER-PART(WS-NUM-A)
           IF WS-NUM-A > 0 AND WS-NUM-A NOT = WS-NUM-B
               ADD 1 TO WS-NUM-B
           END-IF
           COMPUTE WS-NUM-T = WS-NUM-B / WS-NUM-F
           PERFORM RT-R-FLOAT
           PERFORM RT-PUSH-R
           .
       RT-ABS.
           PERFORM RT-POP-A
           PERFORM RT-ORZERO-A
           IF WS-OK = 0 PERFORM RT-PUSH-NULL EXIT PARAGRAPH END-IF
           COMPUTE WS-NUM-T = FUNCTION ABS(WS-NUM-A)
           PERFORM RT-R-FLOAT
           MOVE WS-ALLINT TO RR-I
           PERFORM RT-PUSH-R
           .
       RT-POWER.
           PERFORM RT-POP-B
           PERFORM RT-POP-A
           PERFORM RT-ORZERO-A
           IF WS-OK = 0 PERFORM RT-PUSH-NULL EXIT PARAGRAPH END-IF
           MOVE WS-NUM-A TO WS-NUM-T
           MOVE WS-ALLINT TO WS-NEG
           PERFORM RT-CP-RB-RA
           PERFORM RT-ORZERO-A
           IF WS-OK = 0 PERFORM RT-PUSH-NULL EXIT PARAGRAPH END-IF
           MOVE WS-NUM-A TO WS-NUM-B
           IF WS-NEG = 0 OR WS-NUM-B < 0 MOVE 0 TO WS-ALLINT END-IF
           COMPUTE WS-NUM-T ROUNDED = WS-NUM-T ** WS-NUM-B
               ON SIZE ERROR
                   MOVE "POWER result out of range" TO WS-ERR-MSG
                   PERFORM RT-FAIL-PUSH-NULL
                   EXIT PARAGRAPH
           END-COMPUTE
           PERFORM RT-R-FLOAT
           MOVE WS-ALLINT TO RR-I
           PERFORM RT-PUSH-R
           .
       RT-SQRT.
           PERFORM RT-POP-A
           PERFORM RT-ORZERO-A
           IF WS-OK = 0 PERFORM RT-PUSH-NULL EXIT PARAGRAPH END-IF
           IF WS-NUM-A < 0
               MOVE "SQRT of a negative number" TO WS-ERR-MSG
               PERFORM RT-FAIL-PUSH-NULL
               EXIT PARAGRAPH
           END-IF
           COMPUTE WS-NUM-T ROUNDED = FUNCTION SQRT(WS-NUM-A)
           PERFORM RT-R-FLOAT
           PERFORM RT-PUSH-R
           .
       RT-TAN.
           PERFORM RT-POP-A
           PERFORM RT-ORZERO-A
           IF WS-OK = 0 PERFORM RT-PUSH-NULL EXIT PARAGRAPH END-IF
           IF RA-K = 2 AND RA-F = 1
               MOVE RA-D TO WS-DBL-A
           ELSE
               MOVE WS-NUM-A TO WS-DBL-A
           END-IF
           COMPUTE WS-DBL-T = FUNCTION TAN(WS-DBL-A)
           PERFORM RT-PUSH-DBL
           .
       RT-LOG10.
           PERFORM RT-POP-A
           PERFORM RT-ORZERO-A
           IF WS-OK = 0 PERFORM RT-PUSH-NULL EXIT PARAGRAPH END-IF
           IF WS-NUM-A <= 0
               MOVE "LOG of a non-positive number" TO WS-ERR-MSG
               PERFORM RT-FAIL-PUSH-NULL
               EXIT PARAGRAPH
           END-IF
           COMPUTE WS-NUM-T ROUNDED = FUNCTION LOG10(WS-NUM-A)
           PERFORM RT-R-FLOAT
           PERFORM RT-PUSH-R
           .
       RT-LOG.
      *>   LOG(value, base)
           PERFORM RT-POP-B
           PERFORM RT-POP-A
           PERFORM RT-ORZERO-A
           IF WS-OK = 0 PERFORM RT-PUSH-NULL EXIT PARAGRAPH END-IF
           MOVE WS-NUM-A TO WS-NUM-T
           PERFORM RT-CP-RB-RA
           PERFORM RT-ORZERO-A
           IF WS-OK = 0 PERFORM RT-PUSH-NULL EXIT PARAGRAPH END-IF
           IF WS-NUM-T <= 0 OR WS-NUM-A <= 0 OR WS-NUM-A = 1
               MOVE "LOG of a non-positive number or base" TO WS-ERR-MSG
               PERFORM RT-FAIL-PUSH-NULL
               EXIT PARAGRAPH
           END-IF
           COMPUTE WS-NUM-T ROUNDED = FUNCTION LOG(WS-NUM-T) / FUNCTION LOG(WS-NUM-A)
           PERFORM RT-R-FLOAT
           PERFORM RT-PUSH-R
           .
       RT-PI.
           COMPUTE WS-DBL-T = FUNCTION PI
           PERFORM RT-PUSH-DBL
           .
       RT-MAXMIN.
      *>   WS-NARGS values on the stack, WS-MAXMODE 1 = MAX, 0 = MIN.
      *>   Each is orZero'd; the first of equal extremes is kept.
           COMPUTE WS-K2 = VSP - WS-NARGS + 1
           MOVE 0 TO WS-HAVE-BEST
           PERFORM VARYING WS-J FROM WS-K2 BY 1 UNTIL WS-J > VSP
               PERFORM RT-CP-VSJ-RA
               PERFORM RT-ORZERO-A
               IF WS-OK = 1
                   IF WS-HAVE-BEST = 0
                       MOVE 1 TO WS-HAVE-BEST
                       MOVE WS-NUM-A TO WS-NUM-B
                       MOVE WS-ALLINT TO WS-NEG
                   ELSE
                       IF (WS-MAXMODE = 1 AND WS-NUM-A > WS-NUM-B)
                          OR (WS-MAXMODE = 0 AND WS-NUM-A < WS-NUM-B)
                           MOVE WS-NUM-A TO WS-NUM-B
                           MOVE WS-ALLINT TO WS-NEG
                       END-IF
                   END-IF
               END-IF
           END-PERFORM
           SUBTRACT WS-NARGS FROM VSP
           IF WS-ERR = 1 PERFORM RT-PUSH-NULL EXIT PARAGRAPH END-IF
           MOVE WS-NUM-B TO WS-NUM-T
           PERFORM RT-R-FLOAT
           MOVE WS-NEG TO RR-I
           PERFORM RT-PUSH-R
           .
       RT-SUM.
           COMPUTE WS-K2 = VSP - WS-NARGS + 1
           MOVE 0 TO WS-NUM-B
           MOVE 1 TO WS-NEG
           PERFORM VARYING WS-J FROM WS-K2 BY 1 UNTIL WS-J > VSP
               PERFORM RT-CP-VSJ-RA
               PERFORM RT-NUMZ-A
               ADD WS-NUM-A TO WS-NUM-B
               IF WS-ALLINT = 0 MOVE 0 TO WS-NEG END-IF
           END-PERFORM
           SUBTRACT WS-NARGS FROM VSP
           MOVE WS-NUM-B TO WS-NUM-T
           PERFORM RT-R-FLOAT
           MOVE WS-NEG TO RR-I
           PERFORM RT-PUSH-R
           .

      *> ----------------------------- comparison -----------------------------
       RT-STRCMP.
      *>   WS-CMP := -1 / 0 / 1 comparing the texts of RA and RB bytewise.
           MOVE RA-L TO WS-LEN
           IF RB-L < WS-LEN MOVE RB-L TO WS-LEN END-IF
           MOVE 0 TO WS-CMP
           IF WS-LEN > 0
               IF RA-S(1:WS-LEN) < RB-S(1:WS-LEN)
                   MOVE -1 TO WS-CMP
               ELSE
                   IF RA-S(1:WS-LEN) > RB-S(1:WS-LEN)
                       MOVE 1 TO WS-CMP
                   END-IF
               END-IF
           END-IF
           IF WS-CMP = 0
               IF RA-L < RB-L MOVE -1 TO WS-CMP END-IF
               IF RA-L > RB-L MOVE 1 TO WS-CMP END-IF
           END-IF
           .
       RT-PYEQ.
      *>   Python ==: WS-T := RA == RB.
           MOVE 0 TO WS-T
           IF (RA-K = 1 OR RA-K = 2) AND (RB-K = 1 OR RB-K = 2)
               IF RA-K = 1 MOVE RA-B TO WS-NUM-A ELSE MOVE RA-N TO WS-NUM-A END-IF
               IF RB-K = 1 MOVE RB-B TO WS-NUM-B ELSE MOVE RB-N TO WS-NUM-B END-IF
               IF WS-NUM-A = WS-NUM-B MOVE 1 TO WS-T END-IF
               EXIT PARAGRAPH
           END-IF
           IF RA-K NOT = RB-K
               EXIT PARAGRAPH
           END-IF
           IF RA-K = 0
               MOVE 1 TO WS-T
               EXIT PARAGRAPH
           END-IF
           IF RA-K = 3 AND RA-L = RB-L
               IF RA-L = 0
                   MOVE 1 TO WS-T
               ELSE
                   IF RA-S(1:RA-L) = RB-S(1:RB-L) MOVE 1 TO WS-T END-IF
               END-IF
           END-IF
           .
       RT-TONUM-B.
      *>   toNumber(RB), leaving RA as it was (RC is scratch).
           PERFORM RT-CP-RA-RC
           PERFORM RT-CP-RB-RA
           PERFORM RT-TONUM-A
           PERFORM RT-CP-RC-RA
           .
       RT-COERCE-PAIR.
      *>   erbBlankLogic=coerce on the pair (RA, RB): a blank takes the other
      *>   side's zero (FALSE against a boolean, 0 against a number, else "").
           IF RA-K = 0 AND RB-K = 0
               MOVE 3 TO RA-K RB-K
               MOVE 0 TO RA-L RB-L
               EXIT PARAGRAPH
           END-IF
           IF RA-K = 0
               IF RB-K = 1
                   MOVE 1 TO RA-K
                   MOVE 0 TO RA-B
               ELSE
                   PERFORM RT-TONUM-B
                   IF WS-OK = 1
                       MOVE 2 TO RA-K
                       MOVE 1 TO RA-I
                       MOVE 0 TO RA-N RA-F
                   ELSE
                       MOVE 3 TO RA-K
                       MOVE 0 TO RA-L
                   END-IF
               END-IF
               EXIT PARAGRAPH
           END-IF
           IF RB-K = 0
               IF RA-K = 1
                   MOVE 1 TO RB-K
                   MOVE 0 TO RB-B
               ELSE
                   PERFORM RT-TONUM-A
                   IF WS-OK = 1
                       MOVE 2 TO RB-K
                       MOVE 1 TO RB-I
                       MOVE 0 TO RB-N RB-F
                   ELSE
                       MOVE 3 TO RB-K
                       MOVE 0 TO RB-L
                   END-IF
               END-IF
           END-IF
           .
       RT-EQ.
           PERFORM RT-POP-B
           PERFORM RT-POP-A
           PERFORM RT-COERCE-PAIR
           PERFORM RT-PYEQ
           PERFORM RT-PUSH-BOOL
           .
       RT-NE.
           PERFORM RT-POP-B
           PERFORM RT-POP-A
           PERFORM RT-COERCE-PAIR
           PERFORM RT-PYEQ
           IF WS-T = 1 MOVE 0 TO WS-T ELSE MOVE 1 TO WS-T END-IF
           PERFORM RT-PUSH-BOOL
           .
       RT-SWITCH-EQ.
      *>   One SWITCH arm, as erb_runtime.py's erb_switch: (x)::text =
      *>   (p)::text, and a NULL never matches. Stack: x p -> boolean.
           PERFORM RT-CP-VSP-RA
           IF RA-K = 0
               SUBTRACT 2 FROM VSP
               PERFORM RT-PUSH-FALSE
               EXIT PARAGRAPH
           END-IF
           SUBTRACT 1 FROM VSP
           PERFORM RT-CP-VSP-RA
           ADD 1 TO VSP
           IF RA-K = 0
               SUBTRACT 2 FROM VSP
               PERFORM RT-PUSH-FALSE
               EXIT PARAGRAPH
           END-IF
           PERFORM RT-TEXT
           PERFORM RT-POP-A
           PERFORM RT-CP-RA-RC
           PERFORM RT-TEXT
           PERFORM RT-POP-A
           PERFORM RT-CP-RC-RB
           PERFORM RT-PYEQ
           PERFORM RT-PUSH-BOOL
           .
       RT-CMP.
      *>   WS-OP is "< ", "<=", "> " or ">=".
           PERFORM RT-POP-B
           PERFORM RT-POP-A
           PERFORM RT-COERCE-PAIR
           PERFORM RT-TONUM-A
           MOVE WS-OK TO WS-OK2
           MOVE WS-NUM-T TO WS-NUM-A
           PERFORM RT-TONUM-B
           MOVE WS-NUM-T TO WS-NUM-B
           EVALUATE TRUE
               WHEN WS-OK2 = 1 AND WS-OK = 1
                   EVALUATE TRUE
                       WHEN WS-NUM-A < WS-NUM-B MOVE -1 TO WS-CMP
                       WHEN WS-NUM-A > WS-NUM-B MOVE 1 TO WS-CMP
                       WHEN OTHER MOVE 0 TO WS-CMP
                   END-EVALUATE
               WHEN RA-K = 3 AND RB-K = 3
                   PERFORM RT-STRCMP
               WHEN RA-K = 1 AND RB-K = 1
                   COMPUTE WS-CMP = RA-B - RB-B
               WHEN OTHER
                   MOVE 0 TO WS-T
                   PERFORM RT-PUSH-BOOL
                   EXIT PARAGRAPH
           END-EVALUATE
           MOVE 0 TO WS-T
           EVALUATE WS-OP
               WHEN "< " IF WS-CMP < 0 MOVE 1 TO WS-T END-IF
               WHEN "<=" IF WS-CMP <= 0 MOVE 1 TO WS-T END-IF
               WHEN "> " IF WS-CMP > 0 MOVE 1 TO WS-T END-IF
               WHEN OTHER IF WS-CMP >= 0 MOVE 1 TO WS-T END-IF
           END-EVALUATE
           PERFORM RT-PUSH-BOOL
           .
       RT-COALESCE.
           COMPUTE WS-K2 = VSP - WS-NARGS + 1
           PERFORM RT-R-NULL
           MOVE 0 TO WS-ANDV
           PERFORM VARYING WS-J FROM WS-K2 BY 1 UNTIL WS-J > VSP OR WS-ANDV = 1
               PERFORM RT-CP-VSJ-RA
               PERFORM RT-BLANK-A
               IF WS-T = 0
                   PERFORM RT-CP-RA-RR
                   MOVE 1 TO WS-ANDV
               END-IF
           END-PERFORM
           SUBTRACT WS-NARGS FROM VSP
           PERFORM RT-PUSH-R
           .

      *> ----------------------------- text -----------------------------
       RT-FMT-INT.
      *>   WS-NUM-T as an integer -> WS-NT(1:WS-NT-L)
           MOVE WS-NUM-T TO WS-INT-ED
           MOVE FUNCTION TRIM(WS-INT-ED) TO WS-NT
           COMPUTE WS-NT-L = FUNCTION LENGTH(FUNCTION TRIM(WS-INT-ED))
           .
       RT-FMT-DEC.
      *>   WS-NUM-T in its shortest decimal form, "3.0" for a whole number.
           MOVE WS-NUM-T TO WS-NUM-ED
           MOVE FUNCTION TRIM(WS-NUM-ED) TO WS-NT
           COMPUTE WS-NT-L = FUNCTION LENGTH(FUNCTION TRIM(WS-NUM-ED))
           PERFORM UNTIL WS-NT-L <= 1 OR WS-NT(WS-NT-L:1) NOT = "0"
               SUBTRACT 1 FROM WS-NT-L
           END-PERFORM
           IF WS-NT(WS-NT-L:1) = "."
               ADD 1 TO WS-NT-L
               MOVE "0" TO WS-NT(WS-NT-L:1)
           END-IF
           .
       RT-PYSTR-A.
      *>   str(RA) the reference implementation's way -> WS-TMP(1:WS-TMP-L)
           EVALUATE RA-K
               WHEN 0
                   MOVE "None" TO WS-TMP(1:4)
                   MOVE 4 TO WS-TMP-L
               WHEN 1
                   IF RA-B = 1
                       MOVE "True" TO WS-TMP(1:4)
                       MOVE 4 TO WS-TMP-L
                   ELSE
                       MOVE "False" TO WS-TMP(1:5)
                       MOVE 5 TO WS-TMP-L
                   END-IF
               WHEN 2
                   MOVE RA-N TO WS-NUM-T
                   IF RA-I = 1 PERFORM RT-FMT-INT ELSE PERFORM RT-FMT-DEC END-IF
                   MOVE WS-NT(1:WS-NT-L) TO WS-TMP(1:WS-NT-L)
                   MOVE WS-NT-L TO WS-TMP-L
               WHEN OTHER
                   MOVE RA-L TO WS-TMP-L
                   IF RA-L > 0 MOVE RA-S(1:RA-L) TO WS-TMP(1:RA-L) END-IF
           END-EVALUATE
           .
       RT-TEXT.
      *>   erbText: a CONCAT operand as text. Blank -> "", boolean -> true/false,
      *>   a whole number without ".0".
           PERFORM RT-POP-A
           EVALUATE RA-K
               WHEN 0
                   MOVE 0 TO WS-TMP-L
               WHEN 1
                   IF RA-B = 1
                       MOVE "true" TO WS-TMP(1:4)
                       MOVE 4 TO WS-TMP-L
                   ELSE
                       MOVE "false" TO WS-TMP(1:5)
                       MOVE 5 TO WS-TMP-L
                   END-IF
               WHEN 2
                   MOVE RA-N TO WS-NUM-T
                   IF RA-I = 1 OR RA-N = FUNCTION INTEGER-PART(RA-N)
                       PERFORM RT-FMT-INT
                   ELSE
                       PERFORM RT-FMT-DEC
                   END-IF
                   MOVE WS-NT(1:WS-NT-L) TO WS-TMP(1:WS-NT-L)
                   MOVE WS-NT-L TO WS-TMP-L
               WHEN OTHER
                   PERFORM RT-PUSH-A
                   EXIT PARAGRAPH
           END-EVALUATE
           PERFORM RT-R-TMP
           PERFORM RT-PUSH-R
           .
       RT-CAST.
           PERFORM RT-POP-A
           PERFORM RT-TRUTHY-A
           IF WS-T = 0
               PERFORM RT-R-EMPTY
           ELSE
               PERFORM RT-PYSTR-A
               PERFORM RT-R-TMP
           END-IF
           PERFORM RT-PUSH-R
           .
       RT-CONCAT.
      *>   WS-NARGS text parts on the stack, joined in order.
           COMPUTE WS-K2 = VSP - WS-NARGS + 1
           PERFORM RT-R-EMPTY
           PERFORM VARYING WS-J FROM WS-K2 BY 1 UNTIL WS-J > VSP
               IF VS-L(WS-J) > 0
                   IF RR-L + VS-L(WS-J) > WS-TEXT-MAX
                       MOVE "text longer than 8192 bytes" TO WS-ERR-MSG
                       PERFORM RT-FAIL
                   ELSE
                       MOVE VS-S(WS-J)(1:VS-L(WS-J))
                           TO RR-S(RR-L + 1:VS-L(WS-J))
                       ADD VS-L(WS-J) TO RR-L
                   END-IF
               END-IF
           END-PERFORM
           SUBTRACT WS-NARGS FROM VSP
           PERFORM RT-PUSH-R
           .
       RT-TEXTOP-A.
      *>   textOperand(RA): blank-ish -> ""; text stays; anything else fails.
           PERFORM RT-TRUTHY-A
           IF WS-T = 0
               MOVE 3 TO RA-K
               MOVE 0 TO RA-L
               MOVE 1 TO WS-OK
           ELSE
               IF RA-K NOT = 3
                   MOVE "text function applied to a non-text value" TO WS-ERR-MSG
                   PERFORM RT-FAIL
                   MOVE 0 TO WS-OK
               ELSE
                   MOVE 1 TO WS-OK
               END-IF
           END-IF
           .
       RT-COUNTOP-B.
      *>   countOperand(RB) -> WS-CNTC
           PERFORM RT-CP-RA-RC
           PERFORM RT-CP-RB-RA
           PERFORM RT-TRUTHY-A
           MOVE 1 TO WS-OK
           IF WS-T = 0
               MOVE 0 TO WS-CNTC
           ELSE
               IF RA-K NOT = 2 OR RA-I NOT = 1
                   MOVE "expected an integer count" TO WS-ERR-MSG
                   PERFORM RT-FAIL
                   MOVE 0 TO WS-OK
               ELSE
                   COMPUTE WS-CNTC = FUNCTION INTEGER-PART(RA-N)
               END-IF
           END-IF
           PERFORM RT-CP-RC-RA
           .
       RT-LOWER.
           PERFORM RT-POP-A
           PERFORM RT-TEXTOP-A
           IF WS-OK = 0 PERFORM RT-PUSH-NULL EXIT PARAGRAPH END-IF
           IF RA-L > 0
               MOVE FUNCTION LOWER-CASE(RA-S(1:RA-L)) TO RA-S(1:RA-L)
           END-IF
           PERFORM RT-PUSH-A
           .
       RT-UPPER.
           PERFORM RT-POP-A
           PERFORM RT-TEXTOP-A
           IF WS-OK = 0 PERFORM RT-PUSH-NULL EXIT PARAGRAPH END-IF
           IF RA-L > 0
               MOVE FUNCTION UPPER-CASE(RA-S(1:RA-L)) TO RA-S(1:RA-L)
           END-IF
           PERFORM RT-PUSH-A
           .
       RT-TRIM.
           PERFORM RT-POP-A
           PERFORM RT-TEXTOP-A
           IF WS-OK = 0 PERFORM RT-PUSH-NULL EXIT PARAGRAPH END-IF
           MOVE 1 TO WS-P
           PERFORM UNTIL WS-P > RA-L OR RA-S(WS-P:1) NOT = SPACE
               ADD 1 TO WS-P
           END-PERFORM
           MOVE RA-L TO WS-Q
           PERFORM UNTIL WS-Q < WS-P OR RA-S(WS-Q:1) NOT = SPACE
               SUBTRACT 1 FROM WS-Q
           END-PERFORM
           IF WS-Q < WS-P
               MOVE 0 TO WS-TMP-L
           ELSE
               COMPUTE WS-TMP-L = WS-Q - WS-P + 1
               MOVE RA-S(WS-P:WS-TMP-L) TO WS-TMP(1:WS-TMP-L)
           END-IF
           PERFORM RT-R-TMP
           PERFORM RT-PUSH-R
           .
       RT-CHARLEN-A.
      *>   WS-CNTC := characters in RA's text. With UTF-8 data (ERB-CHARSET
      *>   "U") continuation bytes X"80"-X"BF" are not counted; with a
      *>   single-byte code page such as EBCDIC every byte is a character.
           IF ERB-CHARSET NOT = "U"
               MOVE RA-L TO WS-CNTC
               EXIT PARAGRAPH
           END-IF
           MOVE 0 TO WS-CNTC
           PERFORM VARYING WS-I FROM 1 BY 1 UNTIL WS-I > RA-L
               IF RA-S(WS-I:1) < X"80" OR RA-S(WS-I:1) > X"BF"
                   ADD 1 TO WS-CNTC
               END-IF
           END-PERFORM
           .
       RT-LEN.
           PERFORM RT-POP-A
           PERFORM RT-TEXTOP-A
           IF WS-OK = 0 PERFORM RT-PUSH-NULL EXIT PARAGRAPH END-IF
           PERFORM RT-CHARLEN-A
           MOVE WS-CNTC TO WS-NUM-T
           PERFORM RT-R-INT
           PERFORM RT-PUSH-R
           .
       RT-SLICE-A.
      *>   RA's text := bytes [WS-START, WS-START + WS-CNTC) (0-based), clamped.
           IF WS-START < 0 MOVE 0 TO WS-START END-IF
           IF WS-CNTC < 0 MOVE 0 TO WS-CNTC END-IF
           IF WS-START >= RA-L
               MOVE 0 TO RA-L
               EXIT PARAGRAPH
           END-IF
           IF WS-START + WS-CNTC > RA-L
               COMPUTE WS-CNTC = RA-L - WS-START
           END-IF
           IF WS-CNTC = 0
               MOVE 0 TO RA-L
               EXIT PARAGRAPH
           END-IF
           MOVE RA-S(WS-START + 1:WS-CNTC) TO WS-TMP(1:WS-CNTC)
           MOVE WS-TMP(1:WS-CNTC) TO RA-S(1:WS-CNTC)
           MOVE WS-CNTC TO RA-L
           .
       RT-LEFT.
           PERFORM RT-POP-B
           PERFORM RT-POP-A
           PERFORM RT-TEXTOP-A
           IF WS-OK = 0 PERFORM RT-PUSH-NULL EXIT PARAGRAPH END-IF
           PERFORM RT-COUNTOP-B
           IF WS-OK = 0 PERFORM RT-PUSH-NULL EXIT PARAGRAPH END-IF
           IF WS-CNTC < 0
               COMPUTE WS-CNTC = RA-L + WS-CNTC
           END-IF
           MOVE 0 TO WS-START
           PERFORM RT-SLICE-A
           PERFORM RT-PUSH-A
           .
       RT-RIGHT.
           PERFORM RT-POP-B
           PERFORM RT-POP-A
           PERFORM RT-TEXTOP-A
           IF WS-OK = 0 PERFORM RT-PUSH-NULL EXIT PARAGRAPH END-IF
           PERFORM RT-COUNTOP-B
           IF WS-OK = 0 PERFORM RT-PUSH-NULL EXIT PARAGRAPH END-IF
           IF WS-CNTC <= 0
               MOVE 0 TO RA-L
           ELSE
               COMPUTE WS-START = RA-L - WS-CNTC
               PERFORM RT-SLICE-A
           END-IF
           PERFORM RT-PUSH-A
           .
       RT-MID.
      *>   MID(text, start, count)
           PERFORM RT-POP-C
           PERFORM RT-POP-B
           PERFORM RT-POP-A
           PERFORM RT-TEXTOP-A
           IF WS-OK = 0 PERFORM RT-PUSH-NULL EXIT PARAGRAPH END-IF
           PERFORM RT-CP-RA-RR
           PERFORM RT-CP-RB-RA
           PERFORM RT-TRUTHY-A
           IF WS-T = 0
               MOVE 1 TO WS-START
           ELSE
               PERFORM RT-NUMZ-A
               COMPUTE WS-START = FUNCTION INTEGER-PART(WS-NUM-A)
           END-IF
           PERFORM RT-CP-RC-RB
           PERFORM RT-CP-RR-RA
           PERFORM RT-COUNTOP-B
           IF WS-OK = 0 PERFORM RT-PUSH-NULL EXIT PARAGRAPH END-IF
           SUBTRACT 1 FROM WS-START
           IF WS-CNTC <= 0
               MOVE 0 TO RA-L
           ELSE
               PERFORM RT-SLICE-A
           END-IF
           PERFORM RT-PUSH-A
           .
       RT-SUBSTITUTE.
      *>   SUBSTITUTE(text, old, new): every occurrence, left to right.
           PERFORM RT-POP-C
           PERFORM RT-POP-B
           PERFORM RT-POP-A
           IF RB-K NOT = 3 OR RC-K NOT = 3
               MOVE "SUBSTITUTE expects text arguments" TO WS-ERR-MSG
               PERFORM RT-FAIL-PUSH-NULL
               EXIT PARAGRAPH
           END-IF
           PERFORM RT-TEXTOP-A
           IF WS-OK = 0 PERFORM RT-PUSH-NULL EXIT PARAGRAPH END-IF
           PERFORM RT-R-EMPTY
           MOVE 1 TO WS-I
           PERFORM UNTIL WS-I > RA-L
               IF RR-L + RC-L + 1 > WS-TEXT-MAX
                   MOVE "text longer than 8192 bytes" TO WS-ERR-MSG
                   PERFORM RT-FAIL-PUSH-NULL
                   EXIT PARAGRAPH
               END-IF
               MOVE 0 TO WS-T
               IF RB-L > 0
                   IF WS-I + RB-L - 1 <= RA-L
                       IF RA-S(WS-I:RB-L) = RB-S(1:RB-L)
                           MOVE 1 TO WS-T
                       END-IF
                   END-IF
               END-IF
               IF WS-T = 1
                   IF RC-L > 0
                       MOVE RC-S(1:RC-L) TO RR-S(RR-L + 1:RC-L)
                       ADD RC-L TO RR-L
                   END-IF
                   ADD RB-L TO WS-I
               ELSE
                   IF RB-L = 0 AND RC-L > 0
                       MOVE RC-S(1:RC-L) TO RR-S(RR-L + 1:RC-L)
                       ADD RC-L TO RR-L
                   END-IF
                   ADD 1 TO RR-L
                   MOVE RA-S(WS-I:1) TO RR-S(RR-L:1)
                   ADD 1 TO WS-I
               END-IF
           END-PERFORM
           IF RB-L = 0 AND RC-L > 0
               MOVE RC-S(1:RC-L) TO RR-S(RR-L + 1:RC-L)
               ADD RC-L TO RR-L
           END-IF
           PERFORM RT-PUSH-R
           .
       RT-FIND.
      *>   FIND(needle, haystack): 1-based position, 0 when absent.
           PERFORM RT-POP-B
           PERFORM RT-POP-A
           IF RA-K = 0 OR RB-K = 0
               PERFORM RT-PUSH-NULL
               EXIT PARAGRAPH
           END-IF
           PERFORM RT-PYSTR-A
           MOVE WS-TMP-L TO WS-TMP2-L
           IF WS-TMP-L > 0 MOVE WS-TMP(1:WS-TMP-L) TO WS-TMP2(201:WS-TMP-L) END-IF
           PERFORM RT-CP-RB-RA
           PERFORM RT-PYSTR-A
      *>   needle in WS-TMP2(201:), haystack in WS-TMP
           MOVE 0 TO WS-NUM-T
           IF WS-TMP2-L = 0
               MOVE 1 TO WS-NUM-T
           ELSE
               PERFORM VARYING WS-I FROM 1 BY 1
                   UNTIL WS-I + WS-TMP2-L - 1 > WS-TMP-L OR WS-NUM-T > 0
                   IF WS-TMP(WS-I:WS-TMP2-L) = WS-TMP2(201:WS-TMP2-L)
                       MOVE 0 TO WS-CNTC
                       PERFORM VARYING WS-J FROM 1 BY 1 UNTIL WS-J >= WS-I
                           IF ERB-CHARSET NOT = "U"
                              OR WS-TMP(WS-J:1) < X"80" OR WS-TMP(WS-J:1) > X"BF"
                               ADD 1 TO WS-CNTC
                           END-IF
                       END-PERFORM
                       COMPUTE WS-NUM-T = WS-CNTC + 1
                   END-IF
               END-PERFORM
           END-IF
           PERFORM RT-R-INT
           PERFORM RT-PUSH-R
           .
       RT-DTTEXT.
      *>   erbDatetimeText (erbDateTimeText=iso8601, erbTimezone=UTC): a datetime
      *>   renders as 2026-04-03T14:00:00+00:00 (converted to UTC), a date-only
      *>   value stays YYYY-MM-DD, a blank is "".
           PERFORM RT-POP-A
           PERFORM RT-BLANK-A
           IF WS-T = 1
               PERFORM RT-R-EMPTY
               PERFORM RT-PUSH-R
               EXIT PARAGRAPH
           END-IF
           IF RA-K NOT = 3
               MOVE "cannot render a non-datetime as a datetime" TO WS-ERR-MSG
               PERFORM RT-FAIL-PUSH-NULL
               EXIT PARAGRAPH
           END-IF
           MOVE RA-L TO WS-LEN
           PERFORM UNTIL WS-LEN <= 1 OR RA-S(WS-LEN:1) NOT = SPACE
               SUBTRACT 1 FROM WS-LEN
           END-PERFORM
           MOVE 1 TO WS-P
           PERFORM UNTIL WS-P >= WS-LEN OR RA-S(WS-P:1) NOT = SPACE
               ADD 1 TO WS-P
           END-PERFORM
           IF WS-LEN - WS-P + 1 = 10
               MOVE RA-S(WS-P:10) TO WS-TMP(1:10)
               IF WS-TMP(1:4) IS NUMERIC AND WS-TMP(5:1) = "-" AND WS-TMP(8:1) = "-"
                   MOVE 10 TO WS-TMP-L
                   PERFORM RT-R-TMP
                   PERFORM RT-PUSH-R
                   EXIT PARAGRAPH
               END-IF
           END-IF
           PERFORM RT-PARSE-TIME-A
           IF WS-OK = 0
               MOVE "cannot read the value as a datetime" TO WS-ERR-MSG
               PERFORM RT-FAIL-PUSH-NULL
               EXIT PARAGRAPH
           END-IF
           PERFORM RT-TIME-UTC
           PERFORM RT-FMT-TIME
           MOVE "T" TO WS-TMP(11:1)
           MOVE "+00:00" TO WS-TMP(WS-TMP-L + 1:6)
           ADD 6 TO WS-TMP-L
           PERFORM RT-R-TMP
           PERFORM RT-PUSH-R
           .

      *> ----------------------------- storing -----------------------------
      *> Each converts the top of the stack to a field's storage type, the
      *> way erb_runtime.ts's WRITERS do; a value that cannot be stored fails.
       RT-CONV-STR.
           IF VS-K(VSP) = 0 OR VS-K(VSP) = 3
               EXIT PARAGRAPH
           END-IF
           PERFORM RT-POP-A
           PERFORM RT-PYSTR-A
           PERFORM RT-R-TMP
           PERFORM RT-PUSH-R
           .
       RT-CONV-BOOL.
           EVALUATE VS-K(VSP)
               WHEN 0 CONTINUE
               WHEN 1 CONTINUE
               WHEN 2
                   PERFORM RT-POP-A
                   IF RA-N NOT = 0 MOVE 1 TO WS-T ELSE MOVE 0 TO WS-T END-IF
                   PERFORM RT-PUSH-BOOL
               WHEN 3
                   PERFORM RT-TRIM
                   PERFORM RT-POP-A
                   IF RA-L = 0
                       PERFORM RT-PUSH-NULL
                   ELSE
                       MOVE FUNCTION LOWER-CASE(RA-S(1:RA-L)) TO WS-TMP(1:RA-L)
                       IF RA-L = 4 AND WS-TMP(1:4) = "true"
                           PERFORM RT-PUSH-TRUE
                       ELSE
                           IF RA-L = 5 AND WS-TMP(1:5) = "false"
                               PERFORM RT-PUSH-FALSE
                           ELSE
                               MOVE "cannot store text in a boolean field" TO WS-ERR-MSG
                               PERFORM RT-FAIL-PUSH-NULL
                           END-IF
                       END-IF
                   END-IF
           END-EVALUATE
           .
       RT-CONV-NUM.
      *>   A number field; a whole number reads back as an integer.
           EVALUATE VS-K(VSP)
               WHEN 0 EXIT PARAGRAPH
               WHEN 1
                   PERFORM RT-POP-A
                   MOVE RA-B TO WS-NUM-T
               WHEN 2
                   PERFORM RT-POP-A
                   MOVE RA-N TO WS-NUM-T
               WHEN 3
                   PERFORM RT-POP-A
                   MOVE 1 TO WS-T
                   IF RA-L > 0
                       IF RA-S(1:RA-L) NOT = SPACES MOVE 0 TO WS-T END-IF
                   END-IF
                   IF WS-T = 1
                       PERFORM RT-PUSH-NULL
                       EXIT PARAGRAPH
                   END-IF
                   MOVE RA-L TO WS-TMP-L
                   MOVE RA-S(1:RA-L) TO WS-TMP(1:RA-L)
                   PERFORM RT-PARSE-NUM
                   IF WS-OK = 0
                       MOVE "cannot store text in a number field" TO WS-ERR-MSG
                       PERFORM RT-FAIL-PUSH-NULL
                       EXIT PARAGRAPH
                   END-IF
           END-EVALUATE
           PERFORM RT-R-FLOAT
           IF WS-NUM-T = FUNCTION INTEGER-PART(WS-NUM-T)
               MOVE 1 TO RR-I
           END-IF
           PERFORM RT-PUSH-R
           .
       RT-CONV-INT.
           PERFORM RT-CONV-NUM
           IF VS-K(VSP) = 2
               MOVE VS-N(VSP) TO WS-NUM-T
               MOVE 0 TO WS-DIGITS
               PERFORM RT-ROUND-HALF-AWAY
               MOVE WS-NUM-T TO VS-N(VSP)
               MOVE 1 TO VS-I(VSP)
               MOVE 0 TO VS-F(VSP)
           END-IF
           .
       RT-PLAIN-STR.
           IF VS-K(VSP) = 0
               MOVE 3 TO VS-K(VSP)
               MOVE 0 TO VS-L(VSP)
           END-IF
           .
       RT-PLAIN-BOOL.
           IF VS-K(VSP) = 0
               MOVE 1 TO VS-K(VSP)
               MOVE 0 TO VS-B(VSP)
           END-IF
           .
       RT-PLAIN-NUM.
           IF VS-K(VSP) = 0
               MOVE 2 TO VS-K(VSP)
               MOVE 1 TO VS-I(VSP)
               MOVE 0 TO VS-N(VSP) VS-F(VSP)
           END-IF
           .

      *> ----------------------------- lookups & aggregations -----------------------------
       RT-KEYEQ.
      *>   WS-T := RA and RB are the same lookup key.
           MOVE 0 TO WS-T
           IF RA-K = 0 OR RB-K = 0
               EXIT PARAGRAPH
           END-IF
           IF RA-K = 3 OR RB-K = 3
               IF RA-K = 3 AND RB-K = 3
                   PERFORM RT-PYEQ
               END-IF
               EXIT PARAGRAPH
           END-IF
           PERFORM RT-PYEQ
           .
       RT-CRIT-FIELD.
      *>   COUNTIFS criterion naming a field: pops owner value then the cell;
      *>   blanks compare as "".
           PERFORM RT-POP-B
           PERFORM RT-POP-A
           IF RA-K = 0 MOVE 3 TO RA-K MOVE 0 TO RA-L END-IF
           IF RB-K = 0 MOVE 3 TO RB-K MOVE 0 TO RB-L END-IF
           PERFORM RT-PYEQ
           IF WS-T = 0 MOVE 0 TO WS-MATCH END-IF
           .
       RT-CRIT-RAWFIELD.
      *>   As RT-CRIT-FIELD, for the owner's RAW field: never matches
      *>   while that field is blank (Postgres: = NULLIF(own, '')).
           PERFORM RT-POP-B
           PERFORM RT-POP-A
           IF RB-K = 0 OR (RB-K = 3 AND RB-L = 0)
               MOVE 0 TO WS-MATCH
           ELSE
               IF RA-K = 0 MOVE 3 TO RA-K MOVE 0 TO RA-L END-IF
               PERFORM RT-PYEQ
               IF WS-T = 0 MOVE 0 TO WS-MATCH END-IF
           END-IF
           .
       RT-CRIT-LIT.
           PERFORM RT-POP-B
           PERFORM RT-POP-A
           PERFORM RT-PYEQ
           IF WS-T = 0 MOVE 0 TO WS-MATCH END-IF
           .
       RT-CRIT-OP.
      *>   A quoted comparison criterion (">500"); WS-OP holds the operator.
           PERFORM RT-POP-B
           PERFORM RT-POP-A
           PERFORM RT-BLANK-A
           IF WS-T = 1 MOVE 0 TO WS-MATCH EXIT PARAGRAPH END-IF
           IF RA-K = 1
               MOVE RA-B TO WS-NUM-A
           ELSE
               PERFORM RT-TONUM-A
               IF WS-OK = 0 MOVE 0 TO WS-MATCH EXIT PARAGRAPH END-IF
               MOVE WS-NUM-T TO WS-NUM-A
           END-IF
           PERFORM RT-CP-RB-RA
           PERFORM RT-TONUM-A
           IF WS-OK = 0 MOVE 0 TO WS-MATCH EXIT PARAGRAPH END-IF
           MOVE WS-NUM-T TO WS-NUM-B
           MOVE 0 TO WS-T
           EVALUATE WS-OP
               WHEN "> " IF WS-NUM-A > WS-NUM-B MOVE 1 TO WS-T END-IF
               WHEN ">=" IF WS-NUM-A >= WS-NUM-B MOVE 1 TO WS-T END-IF
               WHEN "< " IF WS-NUM-A < WS-NUM-B MOVE 1 TO WS-T END-IF
               WHEN "<=" IF WS-NUM-A <= WS-NUM-B MOVE 1 TO WS-T END-IF
               WHEN "<>" IF WS-NUM-A NOT = WS-NUM-B MOVE 1 TO WS-T END-IF
               WHEN OTHER IF WS-NUM-A = WS-NUM-B MOVE 1 TO WS-T END-IF
           END-EVALUATE
           IF WS-T = 0 MOVE 0 TO WS-MATCH END-IF
           .
       RT-AGG-NUM.
      *>   Pops a SUM/AVERAGE target cell: WS-OK and SV := it as a number
      *>   (a boolean counts 1/0; blank and non-numeric text are skipped).
           PERFORM RT-POP-A
           MOVE 0 TO WS-OK
           PERFORM RT-BLANK-A
           IF WS-T = 1 EXIT PARAGRAPH END-IF
           IF RA-K = 1
               MOVE RA-B TO WS-NUM-T
               MOVE 1 TO WS-OK
           ELSE
               PERFORM RT-TONUM-A
           END-IF
           IF WS-OK = 1
               MOVE 2 TO SV-K
               MOVE 0 TO SV-I SV-B SV-L SV-F
               MOVE WS-NUM-T TO SV-N
           END-IF
           .
       RT-AGG-ADD.
      *>   Pops a (suffixed) target value and adds it to the running total.
           PERFORM RT-POP-A
           PERFORM RT-NUMZ-A
           ADD WS-NUM-A TO WS-ACC-N
               ON SIZE ERROR
                   MOVE "aggregate too large" TO WS-ERR-MSG
                   PERFORM RT-FAIL
           END-ADD
           ADD 1 TO WS-ACC-CNT
           .
       RT-AGG-MINMAX.
      *>   Pops a MIN/MAX target cell and keeps the extreme in AB
      *>   (WS-MAXMODE 1 = MAX); blanks are skipped.
           PERFORM RT-POP-A
           PERFORM RT-BLANK-A
           IF WS-T = 1 EXIT PARAGRAPH END-IF
           IF WS-HAVE-BEST = 0
               PERFORM RT-CP-RA-AB
               MOVE 1 TO WS-HAVE-BEST
               EXIT PARAGRAPH
           END-IF
           PERFORM RT-CP-AB-RB
           IF WS-MAXMODE = 1
               PERFORM RT-CP-RA-RC
               PERFORM RT-CP-RB-RA
               PERFORM RT-CP-RC-RB
           END-IF
      *>   now WS-T := RA < RB, where for MAX RA is the best and RB the cell
           PERFORM RT-PYLESS
           IF WS-T = 1
               IF WS-MAXMODE = 1 PERFORM RT-CP-RB-AB ELSE PERFORM RT-CP-RA-AB END-IF
           END-IF
           .
       RT-PYLESS.
           MOVE 0 TO WS-T
           IF (RA-K = 1 OR RA-K = 2) AND (RB-K = 1 OR RB-K = 2)
               IF RA-K = 1 MOVE RA-B TO WS-NUM-A ELSE MOVE RA-N TO WS-NUM-A END-IF
               IF RB-K = 1 MOVE RB-B TO WS-NUM-B ELSE MOVE RB-N TO WS-NUM-B END-IF
               IF WS-NUM-A < WS-NUM-B MOVE 1 TO WS-T END-IF
               EXIT PARAGRAPH
           END-IF
           IF RA-K = 3 AND RB-K = 3
               PERFORM RT-STRCMP
               IF WS-CMP < 0 MOVE 1 TO WS-T END-IF
               EXIT PARAGRAPH
           END-IF
           MOVE "MIN/MAX over values that cannot be ordered" TO WS-ERR-MSG
           PERFORM RT-FAIL
           .

      *> ----------------------------- datetimes -----------------------------
       RT-PARSE-TIME-A.
      *>   RA's text as a datetime reading: WS-OK, WS-T-DAYS, WS-T-SECS,
      *>   WS-T-OFF, WS-T-ZONED. Accepts YYYY-MM-DD[(T| )HH[:MM[:SS[.f]]]]
      *>   [Z | +HH[:MM] | -HH[:MM]]; text that starts with a date but does
      *>   not parse further is that date at midnight (erb_runtime.ts inZone).
           MOVE 0 TO WS-OK WS-T-ZONED WS-T-OFF WS-T-DAYS
           MOVE 0 TO WS-T-SECS
           IF RA-K NOT = 3 OR RA-L < 10 EXIT PARAGRAPH END-IF
           IF RA-S(1:4) IS NOT NUMERIC OR RA-S(5:1) NOT = "-"
              OR RA-S(6:2) IS NOT NUMERIC OR RA-S(8:1) NOT = "-"
              OR RA-S(9:2) IS NOT NUMERIC
               EXIT PARAGRAPH
           END-IF
           MOVE RA-S(1:4) TO WS-YMD-Y
           MOVE RA-S(6:2) TO WS-YMD-M
           MOVE RA-S(9:2) TO WS-YMD-D
           IF WS-YMD-Y < 1601 OR WS-YMD-M < 1 OR WS-YMD-M > 12
              OR WS-YMD-D < 1 OR WS-YMD-D > 31
               EXIT PARAGRAPH
           END-IF
           IF FUNCTION TEST-DATE-YYYYMMDD(WS-YMD) NOT = 0
               EXIT PARAGRAPH
           END-IF
           COMPUTE WS-T-DAYS = FUNCTION INTEGER-OF-DATE(WS-YMD)
           MOVE 1 TO WS-OK
           MOVE RA-L TO WS-LEN
           PERFORM UNTIL WS-LEN <= 10 OR RA-S(WS-LEN:1) NOT = SPACE
               SUBTRACT 1 FROM WS-LEN
           END-PERFORM
           IF WS-LEN = 10 EXIT PARAGRAPH END-IF
           IF NOT ((RA-S(11:1) = "T" OR RA-S(11:1) = SPACE) AND WS-LEN >= 13
                   AND RA-S(12:2) IS NUMERIC)
               EXIT PARAGRAPH
           END-IF
           MOVE RA-S(12:2) TO WS-HMS-H
           MOVE 0 TO WS-HMS-M WS-HMS-S WS-FRAC
           MOVE 14 TO WS-I
           IF WS-LEN >= 16 AND RA-S(14:1) = ":" AND RA-S(15:2) IS NUMERIC
               MOVE RA-S(15:2) TO WS-HMS-M
               MOVE 17 TO WS-I
               IF WS-LEN >= 19 AND RA-S(17:1) = ":" AND RA-S(18:2) IS NUMERIC
                   MOVE RA-S(18:2) TO WS-HMS-S
                   MOVE 20 TO WS-I
               END-IF
           END-IF
           IF WS-I <= WS-LEN AND (RA-S(WS-I:1) = "." OR RA-S(WS-I:1) = ",")
               ADD 1 TO WS-I
               MOVE 0 TO WS-CNTC
               MOVE ZEROS TO WS-FRAC-X
               PERFORM UNTIL WS-I > WS-LEN OR RA-S(WS-I:1) IS NOT NUMERIC
                   IF WS-CNTC < 9
                       ADD 1 TO WS-CNTC
                       MOVE RA-S(WS-I:1) TO WS-FRAC-X(WS-CNTC:1)
                   END-IF
                   ADD 1 TO WS-I
               END-PERFORM
           END-IF
           PERFORM UNTIL WS-I > WS-LEN OR RA-S(WS-I:1) NOT = SPACE
               ADD 1 TO WS-I
           END-PERFORM
           IF WS-I <= WS-LEN
               COMPUTE WS-K2 = WS-LEN - WS-I
               EVALUATE TRUE
                   WHEN RA-S(WS-I:1) = "Z" AND WS-K2 = 0
                       MOVE 1 TO WS-T-ZONED
                   WHEN (RA-S(WS-I:1) = "+" OR RA-S(WS-I:1) = "-")
                        AND WS-K2 >= 2
                       IF RA-S(WS-I + 1:2) IS NOT NUMERIC
                           EXIT PARAGRAPH
                       END-IF
                       COMPUTE WS-T-OFF = FUNCTION NUMVAL(RA-S(WS-I + 1:2)) * 3600
                       IF WS-K2 = 5 AND RA-S(WS-I + 3:1) = ":"
                          AND RA-S(WS-I + 4:2) IS NUMERIC
                           COMPUTE WS-T-OFF = WS-T-OFF
                               + FUNCTION NUMVAL(RA-S(WS-I + 4:2)) * 60
                       ELSE
                           IF WS-K2 = 4 AND RA-S(WS-I + 3:2) IS NUMERIC
                               COMPUTE WS-T-OFF = WS-T-OFF
                                   + FUNCTION NUMVAL(RA-S(WS-I + 3:2)) * 60
                           ELSE
                               IF WS-K2 NOT = 2
                                   MOVE 0 TO WS-T-OFF
                                   EXIT PARAGRAPH
                               END-IF
                           END-IF
                       END-IF
                       IF RA-S(WS-I:1) = "-"
                           COMPUTE WS-T-OFF = 0 - WS-T-OFF
                       END-IF
                       MOVE 1 TO WS-T-ZONED
                   WHEN OTHER
                       EXIT PARAGRAPH
               END-EVALUATE
           END-IF
           COMPUTE WS-T-SECS = WS-HMS-H * 3600 + WS-HMS-M * 60 + WS-HMS-S + WS-FRAC
           .
       RT-TIME-UTC.
      *>   The parsed reading as a UTC instant in WS-T-TOT seconds; WS-T-DAYS /
      *>   WS-T-SECS become its UTC date and time of day.
           COMPUTE WS-T-TOT = WS-T-DAYS * 86400 + WS-T-SECS - WS-T-OFF
           COMPUTE WS-T-DAYS = FUNCTION INTEGER(WS-T-TOT / 86400)
           COMPUTE WS-T-SECS = WS-T-TOT - WS-T-DAYS * 86400
           MOVE 0 TO WS-T-OFF
           .
       RT-FMT-TIME.
      *>   WS-T-DAYS + WS-T-SECS as "YYYY-MM-DD HH:MM:SS[.ffffff]" in WS-TMP.
           COMPUTE WS-YMD = FUNCTION DATE-OF-INTEGER(WS-T-DAYS)
           COMPUTE WS-ISEC = FUNCTION INTEGER-PART(WS-T-SECS)
           COMPUTE WS-HMS-H = WS-ISEC / 3600
           COMPUTE WS-HMS-M = (WS-ISEC - WS-HMS-H * 3600) / 60
           COMPUTE WS-HMS-S = WS-ISEC - WS-HMS-H * 3600 - WS-HMS-M * 60
           MOVE SPACES TO WS-TMP(1:30)
           STRING WS-YMD-Y "-" WS-YMD-M "-" WS-YMD-D " "
                  WS-HMS-H ":" WS-HMS-M ":" WS-HMS-S
                  DELIMITED BY SIZE INTO WS-TMP
           END-STRING
           MOVE 19 TO WS-TMP-L
           COMPUTE WS-FRAC = WS-T-SECS - WS-ISEC
           IF WS-FRAC NOT = 0
               MOVE "." TO WS-TMP(20:1)
               MOVE WS-FRAC-X(1:6) TO WS-TMP(21:6)
               MOVE 26 TO WS-TMP-L
               PERFORM UNTIL WS-TMP-L <= 20 OR WS-TMP(WS-TMP-L:1) NOT = "0"
                   SUBTRACT 1 FROM WS-TMP-L
               END-PERFORM
               IF WS-TMP-L = 20 MOVE 19 TO WS-TMP-L END-IF
           END-IF
           .
       RT-DATE-SHIFT.
      *>   (RA or RB is the date, per WS-K2) plus/minus the other as days
      *>   (WS-MAXMODE 1 = plus). Pushes the shifted wall-clock reading as
      *>   "YYYY-MM-DD HH:MM:SS" (the offset is dropped, as erb_date_or_none
      *>   drops it). Sets WS-MATCH 1: the result is pushed.
           MOVE 1 TO WS-MATCH
           IF WS-K2 = 2
               PERFORM RT-CP-RA-RC
               PERFORM RT-CP-RB-RA
               PERFORM RT-CP-RC-RB
           END-IF
           PERFORM RT-PARSE-TIME-A
           IF WS-OK = 0
               MOVE "cannot read the value as a date" TO WS-ERR-MSG
               PERFORM RT-FAIL-PUSH-NULL
               EXIT PARAGRAPH
           END-IF
           PERFORM RT-CP-RB-RA
           PERFORM RT-TRUTHY-A
           MOVE 0 TO WS-NUM-A
           IF WS-T = 1
               PERFORM RT-TONUM-A
               IF WS-OK = 1 MOVE WS-NUM-T TO WS-NUM-A END-IF
           END-IF
           IF WS-MAXMODE = 0
               COMPUTE WS-NUM-A = 0 - WS-NUM-A
           END-IF
           COMPUTE WS-T-TOT = WS-T-DAYS * 86400 + WS-T-SECS + WS-NUM-A * 86400
           COMPUTE WS-T-DAYS = FUNCTION INTEGER(WS-T-TOT / 86400)
           COMPUTE WS-T-SECS = WS-T-TOT - WS-T-DAYS * 86400
           PERFORM RT-FMT-TIME
           PERFORM RT-R-TMP
           PERFORM RT-PUSH-R
           .
       RT-NOW.
      *>   NOW()/TODAY(): the UTC instant, "YYYY-MM-DD HH:MM:SS+00". The
      *>   caller pins it in ERB-NOW (as FORMULA_NOW does in erb_runtime.ts);
      *>   blank means the system clock.
           IF WS-NOW-SET = 0
               IF ERB-NOW NOT = SPACES
                   PERFORM RT-R-EMPTY
                   COMPUTE RR-L = FUNCTION LENGTH(FUNCTION TRIM(ERB-NOW))
                   MOVE FUNCTION TRIM(ERB-NOW) TO RR-S(1:RR-L)
                   PERFORM RT-CP-RR-RA
                   PERFORM RT-PARSE-TIME-A
                   IF WS-OK = 0
                       MOVE "ERB-NOW is not an ISO-8601 datetime" TO WS-ERR-MSG
                       PERFORM RT-FATAL
                   END-IF
               ELSE
                   MOVE FUNCTION CURRENT-DATE TO WS-CURDT
                   MOVE WS-CURDT(1:8) TO WS-YMD
                   COMPUTE WS-T-DAYS = FUNCTION INTEGER-OF-DATE(WS-YMD)
                   COMPUTE WS-T-SECS = FUNCTION NUMVAL(WS-CURDT(9:2)) * 3600
                       + FUNCTION NUMVAL(WS-CURDT(11:2)) * 60
                       + FUNCTION NUMVAL(WS-CURDT(13:2))
                   COMPUTE WS-T-OFF = (FUNCTION NUMVAL(WS-CURDT(18:2)) * 3600
                       + FUNCTION NUMVAL(WS-CURDT(20:2)) * 60)
                   IF WS-CURDT(17:1) = "-"
                       COMPUTE WS-T-OFF = 0 - WS-T-OFF
                   END-IF
               END-IF
               PERFORM RT-TIME-UTC
               COMPUTE WS-T-SECS = FUNCTION INTEGER-PART(WS-T-SECS)
               PERFORM RT-FMT-TIME
               MOVE WS-TMP(1:WS-TMP-L) TO WS-NOW-TEXT
               MOVE "+00" TO WS-NOW-TEXT(WS-TMP-L + 1:3)
               COMPUTE WS-NOW-L = WS-TMP-L + 3
               MOVE 1 TO WS-NOW-SET
           END-IF
           PERFORM RT-R-EMPTY
           MOVE WS-NOW-L TO RR-L
           MOVE WS-NOW-TEXT(1:WS-NOW-L) TO RR-S(1:WS-NOW-L)
           PERFORM RT-PUSH-R
           .
       RT-DATETIME-DIFF.
      *>   DATETIME_DIFF(end, start, unit), as erb_runtime.ts erbDatetimeDiff.
           PERFORM RT-POP-C
           PERFORM RT-POP-B
           PERFORM RT-POP-A
           MOVE "day" TO WS-UNIT
           PERFORM RT-CP-RA-RR
           PERFORM RT-CP-RC-RA
           PERFORM RT-TRUTHY-A
           IF WS-T = 1
               PERFORM RT-PYSTR-A
               MOVE SPACES TO WS-UNIT
               IF WS-TMP-L > 16 MOVE 16 TO WS-TMP-L END-IF
               MOVE FUNCTION LOWER-CASE(WS-TMP(1:WS-TMP-L)) TO WS-UNIT
               PERFORM UNTIL WS-TMP-L <= 1 OR WS-UNIT(WS-TMP-L:1) NOT = "s"
                   MOVE SPACE TO WS-UNIT(WS-TMP-L:1)
                   SUBTRACT 1 FROM WS-TMP-L
               END-PERFORM
           END-IF
           PERFORM RT-CP-RR-RA
           PERFORM RT-BLANK-A
           IF WS-T = 1 PERFORM RT-PUSH-NULL EXIT PARAGRAPH END-IF
           PERFORM RT-CP-RB-RC
           PERFORM RT-CP-RB-RA
           PERFORM RT-BLANK-A
           IF WS-T = 1 PERFORM RT-PUSH-NULL EXIT PARAGRAPH END-IF
      *>   end
           PERFORM RT-CP-RR-RA
           PERFORM RT-PARSE-TIME-A
           IF WS-OK = 0
               MOVE "DATETIME_DIFF: cannot read the end as a datetime" TO WS-ERR-MSG
               PERFORM RT-FAIL-PUSH-NULL
               EXIT PARAGRAPH
           END-IF
           PERFORM RT-TIME-UTC
           MOVE WS-T-TOT TO WS-E-TOT
           MOVE WS-T-DAYS TO WS-E-DAYS
           MOVE WS-T-SECS TO WS-E-SECS
      *>   start
           PERFORM RT-CP-RC-RA
           PERFORM RT-PARSE-TIME-A
           IF WS-OK = 0
               MOVE "DATETIME_DIFF: cannot read the start as a datetime" TO WS-ERR-MSG
               PERFORM RT-FAIL-PUSH-NULL
               EXIT PARAGRAPH
           END-IF
           PERFORM RT-TIME-UTC
           MOVE WS-T-TOT TO WS-S-TOT
           MOVE WS-T-DAYS TO WS-S-DAYS
           MOVE WS-T-SECS TO WS-S-SECS
           EVALUATE WS-UNIT
               WHEN "hour"
                   COMPUTE WS-NUM-T ROUNDED = (WS-E-TOT - WS-S-TOT) / 3600
                   PERFORM RT-R-FLOAT
               WHEN "minute"
                   COMPUTE WS-NUM-T ROUNDED = (WS-E-TOT - WS-S-TOT) / 60
                   PERFORM RT-R-FLOAT
               WHEN "second"
                   COMPUTE WS-NUM-T = WS-E-TOT - WS-S-TOT
                   PERFORM RT-R-FLOAT
               WHEN "day"
                   IF WS-DD-ELAPSED = 1
                       COMPUTE WS-NUM-T = FUNCTION INTEGER-PART((WS-E-TOT - WS-S-TOT) / 86400)
                   ELSE
                       COMPUTE WS-NUM-T = WS-E-DAYS - WS-S-DAYS
                   END-IF
                   PERFORM RT-R-INT
               WHEN "week"
                   IF WS-DD-ELAPSED = 1
                       COMPUTE WS-NUM-T = FUNCTION INTEGER-PART((WS-E-TOT - WS-S-TOT) / 604800)
                   ELSE
                       COMPUTE WS-NUM-T = FUNCTION INTEGER-PART((WS-E-DAYS - WS-S-DAYS) / 7)
                   END-IF
                   PERFORM RT-R-INT
               WHEN "month"
               WHEN "year"
                   COMPUTE WS-YMD = FUNCTION DATE-OF-INTEGER(WS-E-DAYS)
                   MOVE WS-YMD-Y TO WS-E-Y
                   MOVE WS-YMD-M TO WS-E-MO
                   COMPUTE WS-E-CLOCK = WS-YMD-D * 1000000 + FUNCTION INTEGER-PART(WS-E-SECS)
                   COMPUTE WS-YMD = FUNCTION DATE-OF-INTEGER(WS-S-DAYS)
                   MOVE WS-YMD-Y TO WS-S-Y
                   MOVE WS-YMD-M TO WS-S-MO
                   COMPUTE WS-S-CLOCK = WS-YMD-D * 1000000 + FUNCTION INTEGER-PART(WS-S-SECS)
                   COMPUTE WS-NUM-T = (WS-E-Y - WS-S-Y) * 12 + WS-E-MO - WS-S-MO
                   IF WS-E-CLOCK < WS-S-CLOCK
                       SUBTRACT 1 FROM WS-NUM-T
                   END-IF
                   IF WS-UNIT = "year"
                       COMPUTE WS-NUM-T = FUNCTION INTEGER-PART(WS-NUM-T / 12)
                   END-IF
                   PERFORM RT-R-INT
               WHEN OTHER
                   MOVE "DATETIME_DIFF: unsupported unit" TO WS-ERR-MSG
                   PERFORM RT-FAIL-PUSH-NULL
                   EXIT PARAGRAPH
           END-EVALUATE
           PERFORM RT-PUSH-R
           .

      *> ----------------------------- transitive closure -----------------------------
      *> Nodes are named by edge index: node "e" of the from side is CW-EF(e),
      *> of the to side CW-ET(e). CW-V/CW-F/CW-X hold edge indexes whose
      *> CW-ET is the node (visited / frontier / next frontier).
       RT-CW-ADD-EDGE.
      *>   Pops the to value then the from value; a blank end is not an edge.
           PERFORM RT-POP-B
           PERFORM RT-POP-A
           PERFORM RT-BLANK-A
           IF WS-T = 1 EXIT PARAGRAPH END-IF
           PERFORM RT-CP-RA-RC
           PERFORM RT-CP-RB-RA
           PERFORM RT-BLANK-A
           IF WS-T = 1 EXIT PARAGRAPH END-IF
           IF CW-EN >= CW-EDGE-MAX
               MOVE "more than 2000 edges in a transitive closure" TO WS-ERR-MSG
               PERFORM RT-FATAL
           END-IF
           ADD 1 TO CW-EN
           PERFORM RT-PYSTR-A
           IF WS-TMP-L > CW-WIDTH
               MOVE "a closure node id is longer than 256 bytes" TO WS-ERR-MSG
               PERFORM RT-FATAL
           END-IF
           MOVE WS-TMP-L TO CW-ET-L(CW-EN)
           MOVE WS-TMP(1:WS-TMP-L) TO CW-ET(CW-EN)
           PERFORM RT-CP-RC-RA
           PERFORM RT-PYSTR-A
           IF WS-TMP-L > CW-WIDTH
               MOVE "a closure node id is longer than 256 bytes" TO WS-ERR-MSG
               PERFORM RT-FATAL
           END-IF
           MOVE WS-TMP-L TO CW-EF-L(CW-EN)
           MOVE WS-TMP(1:WS-TMP-L) TO CW-EF(CW-EN)
           .
       RT-CLOSURE.
      *>   Every reachable (origin, node) pair, breadth first so the first
      *>   arrival is the shortest derivation; a visited set makes cyclic edge
      *>   sets terminate. An origin reached again through a cycle is a pair.
           MOVE 0 TO CW-PN
           PERFORM VARYING CW-O FROM 1 BY 1 UNTIL CW-O > CW-EN
      *>       an origin is the first edge naming its from node
               MOVE 0 TO CW-FOUND
               PERFORM VARYING CW-I FROM 1 BY 1 UNTIL CW-I >= CW-O OR CW-FOUND = 1
                   IF CW-EF-L(CW-I) = CW-EF-L(CW-O)
                       IF CW-EF(CW-I)(1:CW-EF-L(CW-O)) = CW-EF(CW-O)(1:CW-EF-L(CW-O))
                           MOVE 1 TO CW-FOUND
                       END-IF
                   END-IF
               END-PERFORM
               IF CW-FOUND = 0
                   PERFORM RT-CLOSURE-FROM
               END-IF
           END-PERFORM
           .
       RT-CLOSURE-FROM.
           COMPUTE CW-OSTART = CW-PN + 1
           MOVE 0 TO CW-VN CW-FN
           MOVE 1 TO CW-HOP
      *>   the frontier starts as the origin's own out-edges
           PERFORM VARYING CW-I FROM 1 BY 1 UNTIL CW-I > CW-EN
               IF CW-EF-L(CW-I) = CW-EF-L(CW-O)
                   IF CW-EF(CW-I)(1:CW-EF-L(CW-O)) = CW-EF(CW-O)(1:CW-EF-L(CW-O))
                       MOVE CW-I TO CW-U
                       PERFORM RT-CLOSURE-ARRIVE
                   END-IF
               END-IF
           END-PERFORM
           PERFORM UNTIL CW-FN = 0
               ADD 1 TO CW-HOP
               MOVE 0 TO CW-XN
               PERFORM VARYING CW-J FROM 1 BY 1 UNTIL CW-J > CW-FN
      *>           out-edges of the frontier node CW-ET(CW-F(CW-J))
                   MOVE CW-F(CW-J) TO CW-W
                   PERFORM VARYING CW-I FROM 1 BY 1 UNTIL CW-I > CW-EN
                       IF CW-EF-L(CW-I) = CW-ET-L(CW-W)
                           IF CW-EF(CW-I)(1:CW-ET-L(CW-W)) = CW-ET(CW-W)(1:CW-ET-L(CW-W))
                               MOVE CW-I TO CW-U
                               PERFORM RT-CLOSURE-ARRIVE
                           END-IF
                       END-IF
                   END-PERFORM
               END-PERFORM
               MOVE CW-XN TO CW-FN
               PERFORM VARYING CW-J FROM 1 BY 1 UNTIL CW-J > CW-XN
                   MOVE CW-X(CW-J) TO CW-F(CW-J)
               END-PERFORM
           END-PERFORM
           .
       RT-CLOSURE-ARRIVE.
      *>   Edge CW-U arrives at node CW-ET(CW-U) at hop CW-HOP.
      *>   (A first-hop arrival writes the next frontier into CW-F directly.)
           MOVE 0 TO CW-FOUND
           PERFORM VARYING CW-K FROM CW-OSTART BY 1 UNTIL CW-K > CW-PN OR CW-FOUND = 1
               IF CW-PT-L(CW-K) = CW-ET-L(CW-U)
                   IF CW-ET-L(CW-U) = 0
                       MOVE 1 TO CW-FOUND
                   ELSE
                       IF CW-PT(CW-K)(1:CW-ET-L(CW-U)) = CW-ET(CW-U)(1:CW-ET-L(CW-U))
                           MOVE 1 TO CW-FOUND
                       END-IF
                   END-IF
               END-IF
           END-PERFORM
           IF CW-FOUND = 0
               IF CW-PN >= CW-PAIR-MAX
                   MOVE "more than 20000 pairs in a transitive closure" TO WS-ERR-MSG
                   PERFORM RT-FATAL
               END-IF
               ADD 1 TO CW-PN
               MOVE CW-EF-L(CW-O) TO CW-PF-L(CW-PN)
               MOVE CW-EF(CW-O) TO CW-PF(CW-PN)
               MOVE CW-ET-L(CW-U) TO CW-PT-L(CW-PN)
               MOVE CW-ET(CW-U) TO CW-PT(CW-PN)
               MOVE CW-HOP TO CW-PH(CW-PN)
      *>       inferred unless an edge asserts (origin, node) directly
               MOVE 1 TO CW-PI(CW-PN)
               PERFORM VARYING CW-A FROM 1 BY 1 UNTIL CW-A > CW-EN
                   IF CW-EF-L(CW-A) = CW-EF-L(CW-O) AND CW-ET-L(CW-A) = CW-ET-L(CW-U)
                       IF CW-EF(CW-A)(1:CW-EF-L(CW-O)) = CW-EF(CW-O)(1:CW-EF-L(CW-O))
                          AND CW-ET(CW-A)(1:CW-ET-L(CW-U)) = CW-ET(CW-U)(1:CW-ET-L(CW-U))
                           MOVE 0 TO CW-PI(CW-PN)
                       END-IF
                   END-IF
               END-PERFORM
           END-IF
      *>   extend unless the node is the origin or already visited
           IF CW-ET-L(CW-U) = CW-EF-L(CW-O)
               IF CW-ET(CW-U)(1:CW-ET-L(CW-U)) = CW-EF(CW-O)(1:CW-EF-L(CW-O))
                   EXIT PARAGRAPH
               END-IF
           END-IF
           MOVE 0 TO CW-FOUND
           PERFORM VARYING CW-K FROM 1 BY 1 UNTIL CW-K > CW-VN OR CW-FOUND = 1
               IF CW-ET-L(CW-V(CW-K)) = CW-ET-L(CW-U)
                   IF CW-ET(CW-V(CW-K))(1:CW-ET-L(CW-U)) = CW-ET(CW-U)(1:CW-ET-L(CW-U))
                       MOVE 1 TO CW-FOUND
                   END-IF
               END-IF
           END-PERFORM
           IF CW-FOUND = 1 EXIT PARAGRAPH END-IF
           ADD 1 TO CW-VN
           MOVE CW-U TO CW-V(CW-VN)
           IF CW-HOP = 1
               ADD 1 TO CW-FN
               MOVE CW-U TO CW-F(CW-FN)
           ELSE
               ADD 1 TO CW-XN
               MOVE CW-U TO CW-X(CW-XN)
           END-IF
           .
       RT-PUSH-CW-FROM.
           PERFORM RT-R-EMPTY
           MOVE CW-PF-L(CW-K) TO RR-L
           IF RR-L > 0 MOVE CW-PF(CW-K)(1:RR-L) TO RR-S(1:RR-L) END-IF
           PERFORM RT-PUSH-R
           .
       RT-PUSH-CW-TO.
           PERFORM RT-R-EMPTY
           MOVE CW-PT-L(CW-K) TO RR-L
           IF RR-L > 0 MOVE CW-PT(CW-K)(1:RR-L) TO RR-S(1:RR-L) END-IF
           PERFORM RT-PUSH-R
           .
       RT-PUSH-CW-HOP.
           MOVE CW-PH(CW-K) TO WS-NUM-T
           PERFORM RT-R-INT
           PERFORM RT-PUSH-R
           .
       RT-PUSH-CW-INF.
           MOVE CW-PI(CW-K) TO WS-T
           PERFORM RT-PUSH-BOOL
           .

      *> ----------------------------- index keys -----------------------------
      *> Lookups and aggregation criteria find their rows through sorted
      *> indexes. A key is the canonical form of a value under the equality
      *> the scan used (RT-KEYEQ for a lookup, RT-CRIT-FIELD for a
      *> criterion), so equal keys are exactly the rows the scan matched:
      *>   "N" + the number (a boolean is 0/1), 31 characters
      *>   "S" + the text length (4 digits) + the text
      *> padded with spaces to the index width WS-IXW.
       RT-IXKEY-A.
      *>   WS-IXK := RA's key; WS-IXK-OK 0 when RA can match nothing
      *>   (NULL in a lookup, or text longer than the index holds).
      *>   WS-IXMODE 1 = lookup, 2 = criterion (NULL compares as "").
           MOVE 1 TO WS-IXK-OK
           MOVE SPACES TO WS-IXK(1:WS-IXW)
           EVALUATE RA-K
               WHEN 0
                   IF WS-IXMODE = 1
                       MOVE 0 TO WS-IXK-OK
                   ELSE
                       MOVE "S0000" TO WS-IXK(1:5)
                   END-IF
               WHEN 1
                   MOVE RA-B TO WS-IXN
                   MOVE "N" TO WS-IXK(1:1)
                   MOVE WS-IXN-G TO WS-IXK(2:31)
               WHEN 2
                   IF RA-N = 0
                       MOVE 0 TO WS-IXN
                   ELSE
                       MOVE RA-N TO WS-IXN
                   END-IF
                   MOVE "N" TO WS-IXK(1:1)
                   MOVE WS-IXN-G TO WS-IXK(2:31)
               WHEN OTHER
                   IF RA-L + 5 > WS-IXW
                       MOVE 0 TO WS-IXK-OK
                   ELSE
                       MOVE RA-L TO WS-IXL4
                       MOVE "S" TO WS-IXK(1:1)
                       MOVE WS-IXL4 TO WS-IXK(2:4)
                       IF RA-L > 0
                           MOVE RA-S(1:RA-L) TO WS-IXK(6:RA-L)
                       END-IF
                   END-IF
           END-EVALUATE
           .

      *> ----------------------------- TEXT(value, format) -----------------------------
      *> docs/ERB-TEXT-FUNCTION.md: the Excel meaning, for a fixed subset of
      *> formats. The compiler has already split the format literal into
      *> parts; these paragraphs render one part at a time into WS-TF.
       RT-TF-DATE-BEGIN.
      *>   Pops the value. NULL stays NULL; a number or boolean comes back
      *>   as & renders it; text that is an ISO date or datetime (after
      *>   trimming spaces) is formatted - in UTC, a datetime without an
      *>   offset read as UTC - and any other text comes back unchanged.
      *>   WS-TF-SKIP is 0 when the parts are to be rendered.
           MOVE 1 TO WS-TF-SKIP
           MOVE 0 TO WS-TF-L
           PERFORM RT-POP-A
           IF RA-K = 0
               PERFORM RT-PUSH-NULL
               EXIT PARAGRAPH
           END-IF
           IF RA-K NOT = 3
               PERFORM RT-PUSH-A
               PERFORM RT-TEXT
               EXIT PARAGRAPH
           END-IF
           PERFORM RT-CP-RA-RC
           PERFORM RT-TF-ISO-A
           IF WS-OK = 1
               PERFORM RT-PARSE-TIME-A
           END-IF
           IF WS-OK = 0
               PERFORM RT-CP-RC-RA
               PERFORM RT-PUSH-A
               EXIT PARAGRAPH
           END-IF
           PERFORM RT-TIME-UTC
           COMPUTE WS-YMD = FUNCTION DATE-OF-INTEGER(WS-T-DAYS)
           COMPUTE WS-ISEC = FUNCTION INTEGER-PART(WS-T-SECS)
           COMPUTE WS-HMS-H = WS-ISEC / 3600
           COMPUTE WS-HMS-M = (WS-ISEC - WS-HMS-H * 3600) / 60
           COMPUTE WS-HMS-S = WS-ISEC - WS-HMS-H * 3600 - WS-HMS-M * 60
           MOVE 0 TO WS-TF-SKIP
           .
       RT-TF-ISO-A.
      *>   WS-OK := RA's text, trimmed, is an ISO date (YYYY-MM-DD) or
      *>   datetime (YYYY-MM-DD[T ]HH:MM[:SS[.f]] [Z | +HH[[:]MM] | -HH[[:]MM]]).
      *>   RA is left trimmed (and a final "z" upper-cased) for RT-PARSE-TIME-A.
           MOVE 0 TO WS-OK
           MOVE 1 TO WS-P
           PERFORM UNTIL WS-P > RA-L OR RA-S(WS-P:1) NOT = SPACE
               ADD 1 TO WS-P
           END-PERFORM
           MOVE RA-L TO WS-Q
           PERFORM UNTIL WS-Q < WS-P OR RA-S(WS-Q:1) NOT = SPACE
               SUBTRACT 1 FROM WS-Q
           END-PERFORM
           IF WS-Q < WS-P EXIT PARAGRAPH END-IF
           COMPUTE WS-LEN = WS-Q - WS-P + 1
           MOVE RA-S(WS-P:WS-LEN) TO WS-TMP(1:WS-LEN)
           MOVE WS-TMP(1:WS-LEN) TO RA-S(1:WS-LEN)
           MOVE WS-LEN TO RA-L
           IF RA-L < 10 EXIT PARAGRAPH END-IF
           IF RA-S(1:4) IS NOT NUMERIC OR RA-S(5:1) NOT = "-"
              OR RA-S(6:2) IS NOT NUMERIC OR RA-S(8:1) NOT = "-"
              OR RA-S(9:2) IS NOT NUMERIC
               EXIT PARAGRAPH
           END-IF
           IF RA-L = 10
               MOVE 1 TO WS-OK
               EXIT PARAGRAPH
           END-IF
           IF RA-L < 16 EXIT PARAGRAPH END-IF
           IF (RA-S(11:1) NOT = "T" AND RA-S(11:1) NOT = SPACE)
              OR RA-S(12:2) IS NOT NUMERIC OR RA-S(14:1) NOT = ":"
              OR RA-S(15:2) IS NOT NUMERIC
               EXIT PARAGRAPH
           END-IF
           MOVE 17 TO WS-I
           IF WS-I + 2 <= RA-L
               IF RA-S(WS-I:1) = ":" AND RA-S(WS-I + 1:2) IS NUMERIC
                   ADD 3 TO WS-I
                   IF WS-I < RA-L
                       IF RA-S(WS-I:1) = "." AND RA-S(WS-I + 1:1) IS NUMERIC
                           ADD 1 TO WS-I
                           PERFORM UNTIL WS-I > RA-L
                                      OR RA-S(WS-I:1) IS NOT NUMERIC
                               ADD 1 TO WS-I
                           END-PERFORM
                       END-IF
                   END-IF
               END-IF
           END-IF
           IF WS-I > RA-L
               MOVE 1 TO WS-OK
               EXIT PARAGRAPH
           END-IF
           PERFORM UNTIL WS-I > RA-L OR RA-S(WS-I:1) NOT = SPACE
               ADD 1 TO WS-I
           END-PERFORM
           IF WS-I > RA-L EXIT PARAGRAPH END-IF
           COMPUTE WS-K2 = RA-L - WS-I
           EVALUATE TRUE
               WHEN (RA-S(WS-I:1) = "Z" OR RA-S(WS-I:1) = "z") AND WS-K2 = 0
                   MOVE "Z" TO RA-S(WS-I:1)
                   MOVE 1 TO WS-OK
               WHEN RA-S(WS-I:1) NOT = "+" AND RA-S(WS-I:1) NOT = "-"
                   CONTINUE
               WHEN WS-K2 = 2
                   IF RA-S(WS-I + 1:2) IS NUMERIC MOVE 1 TO WS-OK END-IF
               WHEN WS-K2 = 4
                   IF RA-S(WS-I + 1:4) IS NUMERIC MOVE 1 TO WS-OK END-IF
               WHEN WS-K2 = 5
                   IF RA-S(WS-I + 1:2) IS NUMERIC AND RA-S(WS-I + 3:1) = ":"
                      AND RA-S(WS-I + 4:2) IS NUMERIC
                       MOVE 1 TO WS-OK
                   END-IF
           END-EVALUATE
           .
       RT-TF-PART.
      *>   Appends date part WS-TF-OP: 1 yyyy, 2 yy, 3 mm (month), 4 m,
      *>   5 dd, 6 d, 7 hh, 8 h, 9 mm (minutes), 10 m (minutes), 11 ss, 12 s.
           EVALUATE WS-TF-OP
               WHEN 1
                   MOVE WS-YMD-Y TO WS-TF(WS-TF-L + 1:4)
                   ADD 4 TO WS-TF-L
               WHEN 2
                   MOVE WS-YMD-Y(3:2) TO WS-TF(WS-TF-L + 1:2)
                   ADD 2 TO WS-TF-L
               WHEN 3
                   MOVE WS-YMD-M TO WS-TF2
                   PERFORM RT-TF-PUT2
               WHEN 4
                   MOVE WS-YMD-M TO WS-TF2
                   PERFORM RT-TF-PUT1
               WHEN 5
                   MOVE WS-YMD-D TO WS-TF2
                   PERFORM RT-TF-PUT2
               WHEN 6
                   MOVE WS-YMD-D TO WS-TF2
                   PERFORM RT-TF-PUT1
               WHEN 7
                   MOVE WS-HMS-H TO WS-TF2
                   PERFORM RT-TF-PUT2
               WHEN 8
                   MOVE WS-HMS-H TO WS-TF2
                   PERFORM RT-TF-PUT1
               WHEN 9
                   MOVE WS-HMS-M TO WS-TF2
                   PERFORM RT-TF-PUT2
               WHEN 10
                   MOVE WS-HMS-M TO WS-TF2
                   PERFORM RT-TF-PUT1
               WHEN 11
                   MOVE WS-HMS-S TO WS-TF2
                   PERFORM RT-TF-PUT2
               WHEN OTHER
                   MOVE WS-HMS-S TO WS-TF2
                   PERFORM RT-TF-PUT1
           END-EVALUATE
           .
       RT-TF-PUT2.
           MOVE WS-TF2 TO WS-TF(WS-TF-L + 1:2)
           ADD 2 TO WS-TF-L
           .
       RT-TF-PUT1.
      *>   Without a leading zero.
           IF WS-TF2 < 10
               MOVE WS-TF2(2:1) TO WS-TF(WS-TF-L + 1:1)
               ADD 1 TO WS-TF-L
           ELSE
               PERFORM RT-TF-PUT2
           END-IF
           .
       RT-TF-LITERAL.
      *>   Appends the literal part WS-LIT-S(1:WS-LIT-L).
           IF WS-LIT-L > 0
               MOVE WS-LIT-S(1:WS-LIT-L) TO WS-TF(WS-TF-L + 1:WS-LIT-L)
               ADD WS-LIT-L TO WS-TF-L
           END-IF
           .
       RT-TF-PLAIN-A.
      *>   WS-OK := RA's text is a plain decimal number.
           MOVE 0 TO WS-OK WS-DIG-SEEN WS-DOT-SEEN
           MOVE 1 TO WS-P
           PERFORM UNTIL WS-P > RA-L OR RA-S(WS-P:1) NOT = SPACE
               ADD 1 TO WS-P
           END-PERFORM
           MOVE RA-L TO WS-Q
           PERFORM UNTIL WS-Q < WS-P OR RA-S(WS-Q:1) NOT = SPACE
               SUBTRACT 1 FROM WS-Q
           END-PERFORM
           IF WS-Q < WS-P EXIT PARAGRAPH END-IF
           IF RA-S(WS-P:1) = "+" OR RA-S(WS-P:1) = "-"
               ADD 1 TO WS-P
           END-IF
           PERFORM VARYING WS-I FROM WS-P BY 1 UNTIL WS-I > WS-Q
               EVALUATE TRUE
                   WHEN RA-S(WS-I:1) IS NUMERIC
                       MOVE 1 TO WS-DIG-SEEN
                   WHEN RA-S(WS-I:1) = "." AND WS-DOT-SEEN = 0
                       MOVE 1 TO WS-DOT-SEEN
                   WHEN OTHER
                       EXIT PARAGRAPH
               END-EVALUATE
           END-PERFORM
           MOVE WS-DIG-SEEN TO WS-OK
           .
       RT-TF-END.
           PERFORM RT-R-EMPTY
           MOVE WS-TF-L TO RR-L
           IF WS-TF-L > 0
               MOVE WS-TF(1:WS-TF-L) TO RR-S(1:WS-TF-L)
           END-IF
           PERFORM RT-PUSH-R
           .
       RT-TF-NUMBER.
      *>   Pops the value and renders it with WS-TF-DEC decimals, thousands
      *>   separators when WS-TF-GRP is 1, as a percentage when WS-TF-PCT is
      *>   1; rounded half away from zero; a rounded zero has no sign. NULL
      *>   stays NULL; a boolean comes back as & renders it; text is
      *>   formatted only when it is a plain decimal number ([+-]digits[.d]
      *>   or [+-].digits, spaces around), any other text comes back as is.
           PERFORM RT-POP-A
           EVALUATE RA-K
               WHEN 0
                   PERFORM RT-PUSH-NULL
                   EXIT PARAGRAPH
               WHEN 1
                   PERFORM RT-PUSH-A
                   PERFORM RT-TEXT
                   EXIT PARAGRAPH
               WHEN 3
                   PERFORM RT-TF-PLAIN-A
                   IF WS-OK = 0
                       PERFORM RT-PUSH-A
                       EXIT PARAGRAPH
                   END-IF
                   PERFORM RT-TONUM-A
                   IF WS-OK = 0
                       PERFORM RT-PUSH-A
                       EXIT PARAGRAPH
                   END-IF
               WHEN OTHER
                   MOVE RA-N TO WS-NUM-T
           END-EVALUATE
           IF WS-TF-PCT = 1
               COMPUTE WS-NUM-T = WS-NUM-T * 100
                   ON SIZE ERROR
                       MOVE "TEXT: number too large" TO WS-ERR-MSG
                       PERFORM RT-FAIL-PUSH-NULL
                       EXIT PARAGRAPH
               END-COMPUTE
           END-IF
           MOVE WS-TF-DEC TO WS-DIGITS
           PERFORM RT-ROUND-HALF-AWAY
           MOVE 0 TO WS-TF-L
           IF WS-NUM-T < 0
               MOVE "-" TO WS-TF(1:1)
               MOVE 1 TO WS-TF-L
               COMPUTE WS-NUM-T = 0 - WS-NUM-T
           END-IF
           COMPUTE WS-NUM-A = FUNCTION INTEGER-PART(WS-NUM-T)
           MOVE WS-NUM-A TO WS-INT-ED
           MOVE FUNCTION TRIM(WS-INT-ED) TO WS-NT
           COMPUTE WS-NT-L = FUNCTION LENGTH(FUNCTION TRIM(WS-INT-ED))
           PERFORM VARYING WS-I FROM 1 BY 1 UNTIL WS-I > WS-NT-L
               IF WS-TF-GRP = 1 AND WS-I > 1
                   AND FUNCTION MOD(WS-NT-L - WS-I + 1, 3) = 0
                   ADD 1 TO WS-TF-L
                   MOVE "," TO WS-TF(WS-TF-L:1)
               END-IF
               ADD 1 TO WS-TF-L
               MOVE WS-NT(WS-I:1) TO WS-TF(WS-TF-L:1)
           END-PERFORM
           IF WS-TF-DEC > 0
               COMPUTE WS-NUM-B = (WS-NUM-T - WS-NUM-A) * 10 ** WS-TF-DEC
               MOVE WS-NUM-B TO WS-TF-FR
               ADD 1 TO WS-TF-L
               MOVE "." TO WS-TF(WS-TF-L:1)
               MOVE WS-TF-FR(13 - WS-TF-DEC:WS-TF-DEC)
                   TO WS-TF(WS-TF-L + 1:WS-TF-DEC)
               ADD WS-TF-DEC TO WS-TF-L
           END-IF
           IF WS-TF-PCT = 1
               ADD 1 TO WS-TF-L
               MOVE "%" TO WS-TF(WS-TF-L:1)
           END-IF
           PERFORM RT-TF-END
           .

      *> ----------------------------- register copies -----------------------------
      *> A value moves field by field, and only the L bytes of text it holds:
      *> a group MOVE of a register would copy all 8192 bytes of S every time.
       RT-CP-AB-RB.
           MOVE AB-K TO RB-K
           MOVE AB-B TO RB-B
           MOVE AB-I TO RB-I
           MOVE AB-N TO RB-N
           MOVE AB-F TO RB-F
           MOVE AB-D TO RB-D
           MOVE AB-L TO RB-L
           IF AB-L > 0
               MOVE AB-S(1:AB-L) TO RB-S(1:AB-L)
           END-IF
           .
       RT-CP-RA-AB.
           MOVE RA-K TO AB-K
           MOVE RA-B TO AB-B
           MOVE RA-I TO AB-I
           MOVE RA-N TO AB-N
           MOVE RA-F TO AB-F
           MOVE RA-D TO AB-D
           MOVE RA-L TO AB-L
           IF RA-L > 0
               MOVE RA-S(1:RA-L) TO AB-S(1:RA-L)
           END-IF
           .
       RT-CP-RA-RC.
           MOVE RA-K TO RC-K
           MOVE RA-B TO RC-B
           MOVE RA-I TO RC-I
           MOVE RA-N TO RC-N
           MOVE RA-F TO RC-F
           MOVE RA-D TO RC-D
           MOVE RA-L TO RC-L
           IF RA-L > 0
               MOVE RA-S(1:RA-L) TO RC-S(1:RA-L)
           END-IF
           .
       RT-CP-RA-RR.
           MOVE RA-K TO RR-K
           MOVE RA-B TO RR-B
           MOVE RA-I TO RR-I
           MOVE RA-N TO RR-N
           MOVE RA-F TO RR-F
           MOVE RA-D TO RR-D
           MOVE RA-L TO RR-L
           IF RA-L > 0
               MOVE RA-S(1:RA-L) TO RR-S(1:RA-L)
           END-IF
           .
       RT-CP-RB-AB.
           MOVE RB-K TO AB-K
           MOVE RB-B TO AB-B
           MOVE RB-I TO AB-I
           MOVE RB-N TO AB-N
           MOVE RB-F TO AB-F
           MOVE RB-D TO AB-D
           MOVE RB-L TO AB-L
           IF RB-L > 0
               MOVE RB-S(1:RB-L) TO AB-S(1:RB-L)
           END-IF
           .
       RT-CP-RB-RA.
           MOVE RB-K TO RA-K
           MOVE RB-B TO RA-B
           MOVE RB-I TO RA-I
           MOVE RB-N TO RA-N
           MOVE RB-F TO RA-F
           MOVE RB-D TO RA-D
           MOVE RB-L TO RA-L
           IF RB-L > 0
               MOVE RB-S(1:RB-L) TO RA-S(1:RB-L)
           END-IF
           .
       RT-CP-RB-RC.
           MOVE RB-K TO RC-K
           MOVE RB-B TO RC-B
           MOVE RB-I TO RC-I
           MOVE RB-N TO RC-N
           MOVE RB-F TO RC-F
           MOVE RB-D TO RC-D
           MOVE RB-L TO RC-L
           IF RB-L > 0
               MOVE RB-S(1:RB-L) TO RC-S(1:RB-L)
           END-IF
           .
       RT-CP-RC-RA.
           MOVE RC-K TO RA-K
           MOVE RC-B TO RA-B
           MOVE RC-I TO RA-I
           MOVE RC-N TO RA-N
           MOVE RC-F TO RA-F
           MOVE RC-D TO RA-D
           MOVE RC-L TO RA-L
           IF RC-L > 0
               MOVE RC-S(1:RC-L) TO RA-S(1:RC-L)
           END-IF
           .
       RT-CP-RC-RB.
           MOVE RC-K TO RB-K
           MOVE RC-B TO RB-B
           MOVE RC-I TO RB-I
           MOVE RC-N TO RB-N
           MOVE RC-F TO RB-F
           MOVE RC-D TO RB-D
           MOVE RC-L TO RB-L
           IF RC-L > 0
               MOVE RC-S(1:RC-L) TO RB-S(1:RC-L)
           END-IF
           .
       RT-CP-RR-RA.
           MOVE RR-K TO RA-K
           MOVE RR-B TO RA-B
           MOVE RR-I TO RA-I
           MOVE RR-N TO RA-N
           MOVE RR-F TO RA-F
           MOVE RR-D TO RA-D
           MOVE RR-L TO RA-L
           IF RR-L > 0
               MOVE RR-S(1:RR-L) TO RA-S(1:RR-L)
           END-IF
           .
       RT-CP-RR-VSP.
           MOVE RR-K TO VS-K(VSP)
           MOVE RR-B TO VS-B(VSP)
           MOVE RR-I TO VS-I(VSP)
           MOVE RR-N TO VS-N(VSP)
           MOVE RR-F TO VS-F(VSP)
           MOVE RR-D TO VS-D(VSP)
           MOVE RR-L TO VS-L(VSP)
           IF RR-L > 0
               MOVE RR-S(1:RR-L) TO VS-S(VSP)(1:RR-L)
           END-IF
           .
       RT-CP-SV-RR.
           MOVE SV-K TO RR-K
           MOVE SV-B TO RR-B
           MOVE SV-I TO RR-I
           MOVE SV-N TO RR-N
           MOVE SV-F TO RR-F
           MOVE SV-D TO RR-D
           MOVE SV-L TO RR-L
           IF SV-L > 0
               MOVE SV-S(1:SV-L) TO RR-S(1:SV-L)
           END-IF
           .
       RT-CP-VSJ-RA.
           MOVE VS-K(WS-J) TO RA-K
           MOVE VS-B(WS-J) TO RA-B
           MOVE VS-I(WS-J) TO RA-I
           MOVE VS-N(WS-J) TO RA-N
           MOVE VS-F(WS-J) TO RA-F
           MOVE VS-D(WS-J) TO RA-D
           MOVE VS-L(WS-J) TO RA-L
           IF VS-L(WS-J) > 0
               MOVE VS-S(WS-J)(1:VS-L(WS-J)) TO RA-S(1:VS-L(WS-J))
           END-IF
           .
       RT-CP-VSP-RA.
           MOVE VS-K(VSP) TO RA-K
           MOVE VS-B(VSP) TO RA-B
           MOVE VS-I(VSP) TO RA-I
           MOVE VS-N(VSP) TO RA-N
           MOVE VS-F(VSP) TO RA-F
           MOVE VS-D(VSP) TO RA-D
           MOVE VS-L(VSP) TO RA-L
           IF VS-L(VSP) > 0
               MOVE VS-S(VSP)(1:VS-L(VSP)) TO RA-S(1:VS-L(VSP))
           END-IF
           .
       RT-CP-VSP-RB.
           MOVE VS-K(VSP) TO RB-K
           MOVE VS-B(VSP) TO RB-B
           MOVE VS-I(VSP) TO RB-I
           MOVE VS-N(VSP) TO RB-N
           MOVE VS-F(VSP) TO RB-F
           MOVE VS-D(VSP) TO RB-D
           MOVE VS-L(VSP) TO RB-L
           IF VS-L(VSP) > 0
               MOVE VS-S(VSP)(1:VS-L(VSP)) TO RB-S(1:VS-L(VSP))
           END-IF
           .
       RT-CP-VSP-RC.
           MOVE VS-K(VSP) TO RC-K
           MOVE VS-B(VSP) TO RC-B
           MOVE VS-I(VSP) TO RC-I
           MOVE VS-N(VSP) TO RC-N
           MOVE VS-F(VSP) TO RC-F
           MOVE VS-D(VSP) TO RC-D
           MOVE VS-L(VSP) TO RC-L
           IF VS-L(VSP) > 0
               MOVE VS-S(VSP)(1:VS-L(VSP)) TO RC-S(1:VS-L(VSP))
           END-IF
           .
       RT-CP-AB-RR.
           MOVE AB-K TO RR-K
           MOVE AB-B TO RR-B
           MOVE AB-I TO RR-I
           MOVE AB-N TO RR-N
           MOVE AB-F TO RR-F
           MOVE AB-D TO RR-D
           MOVE AB-L TO RR-L
           IF AB-L > 0
               MOVE AB-S(1:AB-L) TO RR-S(1:AB-L)
           END-IF
           .
