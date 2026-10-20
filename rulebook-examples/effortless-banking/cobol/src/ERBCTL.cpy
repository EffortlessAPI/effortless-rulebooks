      *> =================================================================
      *> ERBCTL - the control block passed to the rules module ERBRULES.
      *>
      *> HAND-WRITTEN, NOT GENERATED. COPY it into any program that calls
      *> ERBRULES:  CALL "ERBRULES" USING ERB-CONTROL <the table groups>.
      *>
      *>   ERB-NOW      in   NOW()/TODAY() as ISO-8601 text, e.g.
      *>                     2026-06-21T22:18:08Z; spaces = the system clock
      *>   ERB-CHARSET  in   "U" the text is UTF-8; anything else = one byte
      *>                     per character (EBCDIC, ASCII, Latin-1)
      *>   ERB-RC       out  0 settled; 8 the data could not be computed
      *>                     (capacity exceeded, cyclic data) - see message
      *>   ERB-ERRORS   out  fields that computed NULL with a reason (each
      *>                     reported on SYSOUT as ERBERR|table|field|row|why)
      *>   ERB-PASSES   out  passes over the data until nothing changed
      *>   ERB-MESSAGE  out  the reason for ERB-RC 8
      *> =================================================================
       01 ERB-CONTROL.
          05 ERB-NOW              PIC X(40).
          05 ERB-CHARSET          PIC X.
          05 ERB-RC               PIC S9(4) COMP.
          05 ERB-ERRORS           PIC S9(9) COMP.
          05 ERB-PASSES           PIC S9(9) COMP.
          05 ERB-MESSAGE          PIC X(200).
