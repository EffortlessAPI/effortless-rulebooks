export function money(n: number | string | null | undefined): string {
  if (n === null || n === undefined || n === "") return "—";
  const x = typeof n === "number" ? n : Number(n);
  if (Number.isNaN(x)) return String(n);
  return x.toLocaleString("en-US", { style: "currency", currency: "USD" });
}

export function date(d: string | null | undefined): string {
  if (!d) return "—";
  return new Date(d).toLocaleDateString("en-US", { year: "numeric", month: "short", day: "numeric" });
}

export function dateInput(d: string | null | undefined): string {
  if (!d) return "";
  const dt = new Date(d);
  return dt.toISOString().slice(0, 10);
}

export function num(n: number | string | null | undefined, digits = 2): string {
  if (n === null || n === undefined || n === "") return "—";
  const x = typeof n === "number" ? n : Number(n);
  if (Number.isNaN(x)) return String(n);
  return x.toLocaleString("en-US", { minimumFractionDigits: 0, maximumFractionDigits: digits });
}
