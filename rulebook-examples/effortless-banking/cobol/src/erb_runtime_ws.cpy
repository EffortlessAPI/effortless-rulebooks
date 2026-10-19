      *> =================================================================
      *> erb_runtime_ws.cpy - working storage of the ERB COBOL runtime.
      *>
      *> HAND-WRITTEN, NOT GENERATED. Shipped beside every generated
      *> erb_main.cbl and COPYed into its WORKING-STORAGE SECTION. The
      *> procedures that use it are in erb_runtime.cpy.
      *>
      *> A formula value is dynamically typed (as in erb_runtime.ts):
      *>   K  kind      0 null, 1 boolean, 2 number, 3 text
      *>   B  boolean   0 / 1
      *>   I  is-int    1 when the number is an integer (renders "3" not "3.0")
      *>   N  number    fixed decimal, 18 integer + 12 fraction digits
      *>   L  length    bytes of text in S
      *>   S  text
      *> Compiled formulas run on a value stack (VS-E, top = VSP). Every
      *> runtime operation pops its operands and pushes one result, even
      *> when it fails: a failure sets WS-ERR and pushes NULL, so the stack
      *> stays balanced and the field is stored NULL with the message.
      *> =================================================================
       01 VSP                  PIC 9(4) COMP-5 VALUE 0.
       01 VSTACK.
          05 VS-E OCCURS 128 TIMES.
             10 VS-K           PIC 9.
             10 VS-B           PIC 9.
             10 VS-I           PIC 9.
             10 VS-N           PIC S9(18)V9(12) COMP-3.
             10 VS-L           PIC 9(5) COMP-5.
             10 VS-S           PIC X(8192).
      *> Registers: operands are popped into RA/RB/RC, results built in RR.
       01 RA.
          05 RA-K              PIC 9.
          05 RA-B              PIC 9.
          05 RA-I              PIC 9.
          05 RA-N              PIC S9(18)V9(12) COMP-3.
          05 RA-L              PIC 9(5) COMP-5.
          05 RA-S              PIC X(8192).
       01 RB.
          05 RB-K              PIC 9.
          05 RB-B              PIC 9.
          05 RB-I              PIC 9.
          05 RB-N              PIC S9(18)V9(12) COMP-3.
          05 RB-L              PIC 9(5) COMP-5.
          05 RB-S              PIC X(8192).
       01 RC.
          05 RC-K              PIC 9.
          05 RC-B              PIC 9.
          05 RC-I              PIC 9.
          05 RC-N              PIC S9(18)V9(12) COMP-3.
          05 RC-L              PIC 9(5) COMP-5.
          05 RC-S              PIC X(8192).
       01 RR.
          05 RR-K              PIC 9.
          05 RR-B              PIC 9.
          05 RR-I              PIC 9.
          05 RR-N              PIC S9(18)V9(12) COMP-3.
          05 RR-L              PIC 9(5) COMP-5.
          05 RR-S              PIC X(8192).
      *> Aggregation scratch: MIN/MAX keep their best value here; a SUMIFS
      *> suffix expression reads the current row's value from SV.
       01 AB.
          05 AB-K              PIC 9.
          05 AB-B              PIC 9.
          05 AB-I              PIC 9.
          05 AB-N              PIC S9(18)V9(12) COMP-3.
          05 AB-L              PIC 9(5) COMP-5.
          05 AB-S              PIC X(8192).
       01 SV.
          05 SV-K              PIC 9.
          05 SV-B              PIC 9.
          05 SV-I              PIC 9.
          05 SV-N              PIC S9(18)V9(12) COMP-3.
          05 SV-L              PIC 9(5) COMP-5.
          05 SV-S              PIC X(8192).
       01 WS-TEXT-MAX          PIC 9(5) COMP-5 VALUE 8192.

      *> Failure state of the formula being evaluated.
       01 WS-ERR               PIC 9 VALUE 0.
       01 WS-ERR-MSG           PIC X(200) VALUE SPACES.
       01 WS-ERR-FIRST         PIC X(200) VALUE SPACES.
       01 WS-REPORT            PIC 9 VALUE 0.
       01 WS-ERR-COUNT         PIC 9(9) COMP-5 VALUE 0.
       01 WS-CUR-TABLE         PIC X(64) VALUE SPACES.
       01 WS-CUR-FIELD         PIC X(64) VALUE SPACES.
       01 WS-CUR-ROW           PIC 9(5) COMP-5 VALUE 0.
       01 WS-CUR-ROW-D         PIC Z(4)9.

      *> Operation arguments set by compiled code before a PERFORM.
       01 WS-NARGS             PIC 9(4) COMP-5 VALUE 0.
       01 WS-OP                PIC XX VALUE SPACES.
       01 WS-MAXMODE           PIC 9 VALUE 0.
       01 WS-T                 PIC 9 VALUE 0.
       01 WS-MATCH             PIC 9 VALUE 0.
       01 WS-OK                PIC 9 VALUE 0.
       01 WS-OK2               PIC 9 VALUE 0.
       01 WS-ANDV              PIC 9 VALUE 0.
       01 WS-CHANGED           PIC 9 VALUE 0.
       01 WS-PASS              PIC 9(5) COMP-5 VALUE 0.
       01 WS-MAX-PASSES        PIC 9(5) COMP-5 VALUE 0.
       01 WS-SETTLED           PIC 9 VALUE 0.

      *> Literal staging for compiled code.
       01 WS-LIT-L             PIC 9(5) COMP-5 VALUE 0.
       01 WS-LIT-S             PIC X(8192).
       01 WS-LIT-N             PIC S9(18)V9(12) COMP-3.
       01 WS-LIT-I             PIC 9.

      *> Aggregation accumulators.
       01 WS-CNT               PIC 9(9) COMP-5 VALUE 0.
       01 WS-ACC-N             PIC S9(18)V9(12) COMP-3.
       01 WS-ACC-CNT           PIC 9(9) COMP-5 VALUE 0.
       01 WS-HAVE-BEST         PIC 9 VALUE 0.

      *> General scratch.
       01 WS-I                 PIC 9(5) COMP-5.
       01 WS-J                 PIC 9(5) COMP-5.
       01 WS-K2                PIC 9(5) COMP-5.
       01 WS-P                 PIC 9(5) COMP-5.
       01 WS-Q                 PIC 9(5) COMP-5.
       01 WS-LEN               PIC 9(5) COMP-5.
       01 WS-CNTC              PIC S9(9) COMP-5.
       01 WS-START             PIC S9(9) COMP-5.
       01 WS-CMP               PIC S9 VALUE 0.
       01 WS-NUM-A             PIC S9(18)V9(12) COMP-3.
       01 WS-NUM-B             PIC S9(18)V9(12) COMP-3.
       01 WS-NUM-F             PIC S9(18)V9(12) COMP-3.
       01 WS-NUM-T             PIC S9(18)V9(12) COMP-3.
       01 WS-DIGITS            PIC S9(4) COMP-5.
       01 WS-NEG               PIC 9.
       01 WS-ALLINT            PIC 9.
       01 WS-CH                PIC X.
       01 WS-EXP-SEEN          PIC 9.
       01 WS-DOT-SEEN          PIC 9.
       01 WS-DIG-SEEN          PIC 9.
       01 WS-NUM-ED            PIC -(19)9.9(12).
       01 WS-INT-ED            PIC -(19)9.
       01 WS-NT                PIC X(40).
       01 WS-NT-L              PIC 9(4) COMP-5.
       01 WS-TMP               PIC X(16400).
       01 WS-TMP-L             PIC 9(5) COMP-5.
       01 WS-TMP2              PIC X(8192).
       01 WS-TMP2-L            PIC 9(5) COMP-5.
       01 WS-UNIT              PIC X(16).

      *> Interchange: one record per line, values separated by "|", each
      *> tagged:  ~ null, b0/b1, i<int>, f<decimal>, s<text>. Text escapes
      *> \\ \p (|) \t \n \r and \xHH (other control bytes), so a line holds
      *> no control character (GnuCOBOL refuses them in LINE SEQUENTIAL
      *> files). A line is <table number>|<value>|...|<value>|.
       01 WS-TAB               PIC X VALUE X"09".
       01 WS-SEP               PIC X VALUE "|".
       01 WS-HEX               PIC X(16) VALUE "0123456789ABCDEF".
       01 WS-LINE              PIC X(262144).
       01 WS-LINE-L            PIC 9(7) COMP-5 VALUE 0.
       01 WS-LINE-POS          PIC 9(7) COMP-5 VALUE 0.
       01 WS-LINE-MAX          PIC 9(7) COMP-5 VALUE 262144.
       01 WS-TOK               PIC X(16384).
       01 WS-TOK-L             PIC 9(5) COMP-5 VALUE 0.
       01 WS-EOF               PIC 9 VALUE 0.
       01 WS-TABLE-NO          PIC 9(4) COMP-5 VALUE 0.

      *> Datetimes: a reading is a day number (FUNCTION INTEGER-OF-DATE) plus
      *> seconds into that day, and the UTC offset it was written in.
       01 WS-T-DAYS            PIC S9(9) COMP-5.
       01 WS-T-SECS            PIC S9(7)V9(9) COMP-3.
       01 WS-T-OFF             PIC S9(7) COMP-5.
       01 WS-T-ZONED           PIC 9.
       01 WS-T-TOT             PIC S9(13)V9(9) COMP-3.
       01 WS-E-TOT             PIC S9(13)V9(9) COMP-3.
       01 WS-S-TOT             PIC S9(13)V9(9) COMP-3.
       01 WS-E-DAYS            PIC S9(9) COMP-5.
       01 WS-S-DAYS            PIC S9(9) COMP-5.
       01 WS-E-SECS            PIC S9(7)V9(9) COMP-3.
       01 WS-S-SECS            PIC S9(7)V9(9) COMP-3.
       01 WS-YMD               PIC 9(8).
       01 WS-YMD-X REDEFINES WS-YMD.
          05 WS-YMD-Y          PIC 9(4).
          05 WS-YMD-M          PIC 99.
          05 WS-YMD-D          PIC 99.
       01 WS-HMS-H             PIC 99.
       01 WS-HMS-M             PIC 99.
       01 WS-HMS-S             PIC 99.
       01 WS-FRAC              PIC V9(9).
       01 WS-FRAC-X REDEFINES WS-FRAC PIC 9(9).
       01 WS-E-Y               PIC 9(4).
       01 WS-E-MO              PIC 99.
       01 WS-E-CLOCK           PIC 9(8).
       01 WS-S-Y               PIC 9(4).
       01 WS-S-MO              PIC 99.
       01 WS-S-CLOCK           PIC 9(8).
       01 WS-ISEC              PIC 9(5) COMP-5.
       01 WS-ENV               PIC X(100).
       01 WS-NOW-SET           PIC 9 VALUE 0.
       01 WS-NOW-TEXT          PIC X(40).
       01 WS-NOW-L             PIC 9(4) COMP-5.
       01 WS-DD-ELAPSED        PIC 9 VALUE 0.
       01 WS-CURDT             PIC X(21).

      *> Transitive closure workspace (erb_runtime.ts computeClosureRelation):
      *> compiled code loads the edges, RT-CLOSURE writes every reachable pair
      *> with its shortest hop distance and whether an edge asserts it.
       01 CW-EDGE-MAX          PIC 9(5) COMP-5 VALUE 2000.
       01 CW-PAIR-MAX          PIC 9(5) COMP-5 VALUE 20000.
       01 CW-WIDTH             PIC 9(5) COMP-5 VALUE 256.
       01 CW-EN                PIC 9(5) COMP-5 VALUE 0.
       01 CW-EDGES.
          05 CW-E OCCURS 2000 TIMES.
             10 CW-EF-L        PIC 9(5) COMP-5.
             10 CW-EF          PIC X(256).
             10 CW-ET-L        PIC 9(5) COMP-5.
             10 CW-ET          PIC X(256).
       01 CW-PN                PIC 9(5) COMP-5 VALUE 0.
       01 CW-PAIRS.
          05 CW-P OCCURS 20000 TIMES.
             10 CW-PF-L        PIC 9(5) COMP-5.
             10 CW-PF          PIC X(256).
             10 CW-PT-L        PIC 9(5) COMP-5.
             10 CW-PT          PIC X(256).
             10 CW-PH          PIC 9(5) COMP-5.
             10 CW-PI          PIC 9.
       01 CW-VN                PIC 9(5) COMP-5 VALUE 0.
       01 CW-FN                PIC 9(5) COMP-5 VALUE 0.
       01 CW-XN                PIC 9(5) COMP-5 VALUE 0.
       01 CW-NODES.
          05 CW-V OCCURS 2000 TIMES PIC 9(5) COMP-5.
          05 CW-F OCCURS 2000 TIMES PIC 9(5) COMP-5.
          05 CW-X OCCURS 2000 TIMES PIC 9(5) COMP-5.
       01 CW-O                 PIC 9(5) COMP-5.
       01 CW-OSTART            PIC 9(5) COMP-5.
       01 CW-HOP               PIC 9(5) COMP-5.
       01 CW-I                 PIC 9(5) COMP-5.
       01 CW-J                 PIC 9(5) COMP-5.
       01 CW-K                 PIC 9(5) COMP-5.
       01 CW-U                 PIC 9(5) COMP-5.
       01 CW-W                 PIC 9(5) COMP-5.
       01 CW-A                 PIC 9(5) COMP-5.
       01 CW-FOUND             PIC 9.
