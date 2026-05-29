// AST → React. Renders a formula as an inline expression where each ref is a
// clickable chip. Operators and function names are styled but plain text.

import type { JSX, ReactNode } from "react";
import type { Node, BinOp } from "./formula.ts";

export interface JsxRenderCtx {
  // Render a single field reference as a chip. The page provides this; it's
  // responsible for knowing the field's type → color + hover behavior +
  // click navigation.
  renderChip: (table: string | null, field: string) => ReactNode;
}

const BINOP_LABEL: Record<BinOp, string> = {
  "+": "+",
  "-": "−",
  "*": "×",
  "/": "÷",
  "&": "&",
  "=": "=",
  "<>": "≠",
  "<": "<",
  ">": ">",
  "<=": "≤",
  ">=": "≥",
};

export function renderJsx(node: Node, ctx: JsxRenderCtx): ReactNode {
  return render(node, ctx, true);
}

function render(node: Node, ctx: JsxRenderCtx, top: boolean): ReactNode {
  switch (node.kind) {
    case "ref":
      return ctx.renderChip(node.table, node.field);
    case "num":
      return <span className="fx-num">{formatNumber(node.value)}</span>;
    case "str":
      return <span className="fx-str">&ldquo;{node.value}&rdquo;</span>;
    case "bool":
      return <span className="fx-bool">{node.value ? "TRUE" : "FALSE"}</span>;
    case "unary":
      return <>−{render(node.arg, ctx, false)}</>;
    case "binop": {
      const inner = (
        <>
          {render(node.left, ctx, false)} <span className="fx-op">{BINOP_LABEL[node.op]}</span> {render(node.right, ctx, false)}
        </>
      );
      return top ? inner : <>({inner})</>;
    }
    case "call":
      return renderCall(node, ctx);
  }
}

function renderCall(node: Extract<Node, { kind: "call" }>, ctx: JsxRenderCtx): JSX.Element {
  return (
    <>
      <span className="fx-fn">{node.fn.toLowerCase()}</span>
      <span className="fx-paren">(</span>
      {node.args.map((a, i) => (
        <span key={i}>
          {i > 0 ? <span className="fx-comma">,&nbsp;</span> : null}
          {render(a, ctx, false)}
        </span>
      ))}
      <span className="fx-paren">)</span>
    </>
  );
}

function formatNumber(n: number): string {
  if (Number.isInteger(n) && Math.abs(n) >= 1000) return n.toLocaleString("en-US");
  return String(n);
}
