import React, { useState, useEffect, useCallback } from "react";

// Map a display op glyph to the API op name.
const OP_NAME = { "+": "add", "−": "sub", "×": "mul", "÷": "div" };

export default function Calculator() {
  // The calculator's honest width comes from the API, not a hardcoded constant.
  const [cfg, setCfg] = useState({ width: 4, maxOperand: 15 });
  const [display, setDisplay] = useState("0"); // what the LCD shows
  const [acc, setAcc] = useState(null); // first operand (number) once an op is chosen
  const [op, setOp] = useState(null); // pending op glyph
  const [fresh, setFresh] = useState(true); // next digit starts a new number
  const [bits, setBits] = useState(""); // last gate output bits (LCD annunciator)
  const [over, setOver] = useState(false); // last result overflowed the register
  const [fullValue, setFullValue] = useState(null); // untruncated value on overflow
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState(false);

  useEffect(() => {
    fetch("/api/config").then((r) => r.json()).then(setCfg).catch(() => {});
  }, []);

  const MAX = cfg.maxOperand;

  const clearAll = useCallback(() => {
    setDisplay("0"); setAcc(null); setOp(null); setFresh(true);
    setBits(""); setError(false); setOver(false); setFullValue(null);
  }, []);

  const inputDigit = useCallback((d) => {
    if (error) return;
    if (d > MAX) return; // a digit bigger than the whole range can never be entered
    setBits(""); setOver(false); setFullValue(null);
    setDisplay((cur) => {
      const next = fresh || cur === "0" ? String(d) : cur + String(d);
      // clamp to the operand range; a WIDTH-bit calculator can't hold more
      if (Number(next) > MAX) return cur;
      return next;
    });
    setFresh(false);
  }, [fresh, error, MAX]);

  const chooseOp = useCallback((glyph) => {
    if (error) return;
    setAcc(Number(display));
    setOp(glyph);
    setFresh(true);
    setBits(""); setOver(false); setFullValue(null);
  }, [display, error]);

  const equals = useCallback(async () => {
    if (error || op == null || acc == null || busy) return;
    const b = Number(display);
    setBusy(true);
    try {
      const res = await fetch("/api/calc", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ op: OP_NAME[op], a: acc, b }),
      });
      const data = await res.json();
      if (!res.ok) throw new Error(data.error || "calc failed");
      // The value is the WIDTH-bit register (already wrapped by the gates). We show
      // exactly that; `overflow` lights the lamp when the true result didn't fit.
      setDisplay(String(data.value));
      setBits(data.bits);
      setOver(Boolean(data.overflow));
      setFullValue(data.overflow ? data.fullValue : null);
      setAcc(null); setOp(null); setFresh(true);
    } catch (e) {
      setError(true);
      setDisplay("E");
      setBits(""); setOver(false); setFullValue(null);
    } finally {
      setBusy(false);
    }
  }, [error, op, acc, display, busy]);

  // keyboard support
  useEffect(() => {
    const onKey = (e) => {
      if (e.key >= "0" && e.key <= "9") inputDigit(Number(e.key));
      else if (e.key === "+") chooseOp("+");
      else if (e.key === "-") chooseOp("−");
      else if (e.key === "*") chooseOp("×");
      else if (e.key === "/") { e.preventDefault(); chooseOp("÷"); }
      else if (e.key === "Enter" || e.key === "=") { e.preventDefault(); equals(); }
      else if (e.key === "Escape" || e.key.toLowerCase() === "c") clearAll();
    };
    window.addEventListener("keydown", onKey);
    return () => window.removeEventListener("keydown", onKey);
  }, [inputDigit, chooseOp, equals, clearAll]);

  const pending = op ? `${acc ?? ""} ${op}` : "";
  // Digit keys 0..9, but disable any digit that alone exceeds the range
  // (e.g. on a 3-bit calc, 8 and 9 are dead — here everything 0..9 is enterable
  //  because 9 <= 15, but multi-digit entry still clamps at MAX).
  const digitEnabled = (d) => d <= MAX;

  return (
    <div className="page">
      <div className="casio">
        <div className="brand">
          <span className="mark">bit-CALC</span>
          <span className="model">GATE-{cfg.width} · solar</span>
        </div>

        <div className="solar" aria-hidden="true">
          {Array.from({ length: 4 }).map((_, i) => <span key={i} className="cell" />)}
        </div>

        <div className={"lcd" + (error ? " lcd-err" : "")}>
          <div className="lcd-annunciators">
            <span className={busy ? "ann on" : "ann"}>SETTLING</span>
            <span className="ann on">GATES</span>
            <span className={over ? "ann over on" : "ann over"}>OVER</span>
            <span className="ann bits">{bits ? `b ${bits}` : ""}</span>
          </div>
          <div className="lcd-pending">{pending}</div>
          <div className="lcd-main">{display}</div>
          <div className="lcd-note">
            {over && fullValue != null
              ? `${fullValue} wrapped mod ${1 << cfg.width}`
              : `${cfg.width}-bit · 0–${MAX}`}
          </div>
        </div>

        <div className="keys">
          <button className="key fn wide" onClick={clearAll}>AC</button>
          <button className="key op" onClick={() => chooseOp("÷")}>÷</button>
          <button className="key op" onClick={() => chooseOp("×")}>×</button>

          {[7, 8, 9].map((d) => (
            <button key={d} className="key num" disabled={!digitEnabled(d)}
                    onClick={() => inputDigit(d)}>{d}</button>
          ))}
          <button className="key op" onClick={() => chooseOp("−")}>−</button>

          {[4, 5, 6].map((d) => (
            <button key={d} className="key num" disabled={!digitEnabled(d)}
                    onClick={() => inputDigit(d)}>{d}</button>
          ))}
          <button className="key op" onClick={() => chooseOp("+")}>+</button>

          {[1, 2, 3].map((d) => (
            <button key={d} className="key num" onClick={() => inputDigit(d)}>{d}</button>
          ))}
          <button className="key eq tall" onClick={equals} disabled={busy}>=</button>

          <button className="key num wide" onClick={() => inputDigit(0)}>0</button>
          <button className="key fn" onClick={clearAll}>C</button>
        </div>

        <div className="footnote">
          operands 0–{MAX} · results wrap mod {1 << cfg.width} · the <b>=</b> key
          settles a {cfg.width}-bit logic-gate netlist in Postgres — no arithmetic in the app
        </div>
      </div>
    </div>
  );
}
