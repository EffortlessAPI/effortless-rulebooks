export function num(x: any, digits = 2): string {
  const n = Number(x);
  if (!isFinite(n)) return "—";
  return n.toFixed(digits);
}
export function dateOnly(x: any): string {
  if (!x) return "—";
  const s = String(x);
  return s.length >= 10 ? s.slice(0, 10) : s;
}
