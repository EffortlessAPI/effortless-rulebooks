import { useEffect, useState, type ReactNode } from "react";
import { useNavigate } from "react-router-dom";
import * as api from "../api";
import { useSession } from "../session";

export const initials = (name: string) => name.split(/\s+/).map((w) => w[0]).slice(0, 2).join("").toUpperCase();
export const fmtDay = (s?: string | null) => (s ? new Date(s).toLocaleDateString("en-US", { month: "short", day: "numeric", year: "numeric" }) : "—");
export const fmtTime = (s?: string | null) => (s ? new Date(s).toLocaleString("en-US", { month: "short", day: "numeric", hour: "numeric", minute: "2-digit" }) : "—");
export const human = (id?: string | null) => (id ? id.replace(/-/g, " ").replace(/\b\w/g, (c) => c.toUpperCase()) : "—");
export const yn = (b: any) => (b === true ? "Yes" : b === false ? "No" : "—");

export function Device({ kind, children }: { kind: "phone" | "tablet"; children: ReactNode }) {
  const { toasts } = useSession();
  return (
    <div className="stage">
      <div className={`device ${kind}`}>
        <div className={`screen ${kind}`}>
          <div className="statusbar" />
          {children}
          <div className="toasts">
            {toasts.map((t) => (<div key={t.id} className={`toast ${t.tone}`}><b>{t.title}</b>{t.detail && <span className="delta">{t.detail}</span>}</div>))}
          </div>
        </div>
      </div>
    </div>
  );
}

export function AppBar({ title, eyebrow }: { title: string; eyebrow?: string }) {
  const { shell, signOut } = useSession();
  const nav = useNavigate();
  const [open, setOpen] = useState(false);
  if (!shell) return null;
  return (
    <>
      <div className="appbar" style={{ ["--accent" as any]: shell.profile?.accent_color || undefined }}>
        <button className="who" onClick={() => setOpen(true)} aria-label="Account">{initials(shell.claims.name)}</button>
        <div className="titles"><div className="eyebrow">{eyebrow || shell.claims.principal_label}</div><h1>{title}</h1></div>
        <div className="asof">judged as of<b>{fmtDay(shell.context.as_of_instant)}</b></div>
      </div>
      {open && (
        <Sheet onClose={() => setOpen(false)} title={shell.claims.name} desc={`${shell.claims.principal_label} · ${human(shell.claims.organization)}`}>
          <div className="card tight"><dl className="kv">
            <dt>Signed in as</dt><dd className="fact">{shell.claims.agent}</dd>
            <dt>Database role</dt><dd className="mono">{shell.claims.principal.replace("principal-", "pko_").replace(/-/g, "_")}</dd>
            <dt>Tables in my schema</dt><dd className="derived">{shell.tables.length}</dd>
            <dt>Things I may change</dt><dd className="derived">{shell.actions.length}</dd>
          </dl></div>
          <p className="provenance">This sign-in is a Postgres role with its own schema. A table that is not listed above does not exist for it, and the {shell.actions.length} changes it may make are rows in <code>AppActions</code>, each permitted by a row policy.</p>
          {shell.claims.is_admin && <button className="btn ghost" style={{ marginTop: 14 }} onClick={() => { setOpen(false); nav("/admin"); }}>Admin</button>}
          <button className="btn" style={{ marginTop: 10 }} onClick={() => { signOut(); nav("/"); }}>Sign out</button>
        </Sheet>
      )}
    </>
  );
}

export function Sheet({ title, desc, onClose, children }: { title: string; desc?: string; onClose: () => void; children: ReactNode }) {
  return (
    <div className="scrim" onClick={(e) => e.target === e.currentTarget && onClose()}>
      <div className="sheet" role="dialog" aria-label={title}>
        <div className="grab" /><button className="sheet-close" aria-label="Close" onClick={onClose}>×</button><h2>{title}</h2>{desc ? <p className="desc">{desc}</p> : <div style={{ height: 12 }} />}
        {children}
      </div>
    </div>
  );
}

export const Tag = ({ tone, children }: { tone: "red" | "green" | "amber" | "blue" | "purple" | "grey" | "black" | "solid-red"; children: ReactNode }) => <span className={`tag ${tone}`}>{children}</span>;
export const Err = ({ error }: { error: string | null }) => (error ? <div className="err">{error}</div> : null);
export const Loading = () => <div className="empty">Reading the views…</div>;

/** The words shown after a write: what the book worked out differently. */
export function describeWatched(w: api.Watched | null) {
  if (!w) return undefined;
  const v = (x: any) => (x === true ? "yes" : x === false ? "no" : x === null ? "blank" : String(x));
  const name = w.field.replace(/([a-z])([A-Z])/g, "$1 $2").toLowerCase();
  return w.changed ? `${name}: ${v(w.before)} → ${v(w.after)}` : `${name}: still ${v(w.after)}`;
}

type Choice = { value: string; label: string };
/**
 * A form generated from one AppActions row. Typed, picked and toggled fields are inputs; fixed, context
 * and server fields never appear, because the person does not choose them. Nothing is saved until Save,
 * and the screen behind stays exactly as it was until then.
 */
export function ActionSheet({ actionId, title, context, recordKey, watch, initial, onClose, onDone, choiceFilter, submitLabel }: {
  actionId: string; title?: string; context?: api.Row; recordKey?: string; watch?: string; initial?: api.Row;
  onClose: () => void; onDone?: (r: api.ActionResult) => void; choiceFilter?: (field: string, row: api.Row) => boolean; submitLabel?: string;
}) {
  const { action, toast, bump } = useSession();
  const a = action(actionId);
  const inputs = a.fields.filter((f) => !["fixed", "context", "server"].includes(f.input_kind));
  const [values, setValues] = useState<api.Row>(() => ({ ...Object.fromEntries(inputs.filter((f) => f.input_kind === "toggle").map((f) => [f.field_name, false])), ...(initial || {}) }));
  const [choices, setChoices] = useState<Record<string, Choice[]>>({});
  const [busy, setBusy] = useState(false); const [error, setError] = useState<string | null>(null);
  const dirty = inputs.some((f) => values[f.field_name] !== (initial?.[f.field_name] ?? (f.input_kind === "toggle" ? false : undefined)));

  useEffect(() => {
    inputs.filter((f) => f.input_kind === "choice").forEach(async (f) => {
      if (f.choices_from.includes("|")) { setChoices((c) => ({ ...c, [f.field_name]: f.choices_from.split("|").map((v) => ({ value: v, label: v.replace(/([a-z])([A-Z])/g, "$1 $2") })) })); return; }
      try {
        const table = f.choices_from.replace(/(?<!^)(?=[A-Z])/g, "_").toLowerCase();
        const { rows: rs, pk } = await api.keyedRows(table);
        setChoices((c) => ({ ...c, [f.field_name]: rs.filter((r) => !choiceFilter || choiceFilter(f.field_name, r)).map((r) => ({ value: r[pk], label: r.label || r.name || r[pk] })) }));
      } catch (e: any) { setError(`${f.field_label}: ${e.message}`); }
    });
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [actionId]);

  const save = async () => {
    setBusy(true); setError(null);
    try {
      const r = await api.act(actionId, { key: recordKey, context, values, watch });
      bump(); toast({ tone: "green", title: `${a.label}: saved`, detail: describeWatched(r.watched) });
      onDone?.(r); onClose();
    } catch (e: any) { setError(e.message); } finally { setBusy(false); }
  };
  const set = (k: string, v: any) => setValues((x) => ({ ...x, [k]: v }));
  const missing = inputs.some((f) => f.input_kind !== "toggle" && f.input_kind !== "longtext" && (values[f.field_name] == null || values[f.field_name] === ""));

  return (
    <Sheet title={title || a.label} desc={a.description} onClose={onClose}>
      {dirty && <div className="unsaved">● Not saved yet. Nothing behind this sheet has changed.</div>}
      <Err error={error} />
      {inputs.map((f) => {
        const v = values[f.field_name];
        if (f.input_kind === "toggle") return (<button key={f.field_name} className="toggle" role="switch" aria-checked={!!v} onClick={() => set(f.field_name, !v)}><span>{f.field_label}</span><span className="sw" /></button>);
        if (f.input_kind === "choice") {
          const opts = choices[f.field_name] || [];
          if (opts.length && opts.length <= 4) return (<div key={f.field_name}><div className="field" style={{ marginBottom: 6 }}><span>{f.field_label}</span></div><div className="seg">{opts.map((o) => <button key={o.value} aria-pressed={v === o.value} onClick={() => set(f.field_name, o.value)}>{o.label}</button>)}</div></div>);
          return (<label key={f.field_name} className="field"><span>{f.field_label}</span><select value={v || ""} onChange={(e) => set(f.field_name, e.target.value)}><option value="" disabled>Choose…</option>{opts.map((o) => <option key={o.value} value={o.value}>{o.label}</option>)}</select></label>);
        }
        if (f.input_kind === "longtext") return (<label key={f.field_name} className="field"><span>{f.field_label}</span><textarea value={v || ""} onChange={(e) => set(f.field_name, e.target.value)} /></label>);
        if (f.input_kind === "date") return (<label key={f.field_name} className="field"><span>{f.field_label}</span><input type="datetime-local" value={v || ""} onChange={(e) => set(f.field_name, e.target.value)} /></label>);
        return (<label key={f.field_name} className="field"><span>{f.field_label}</span><input type="text" value={v || ""} onChange={(e) => set(f.field_name, e.target.value)} /></label>);
      })}
      <button className="btn accent" disabled={busy || missing} onClick={save}>{busy ? "Saving…" : submitLabel || "Save"}</button>
      <p className="provenance">Writes <code>{a.target_table}</code> ({a.operation.toLowerCase()}) as you. The database decides whether it is allowed. Afterwards the app re-reads <code>{a.watched_field}</code>, which the rulebook works out.</p>
    </Sheet>
  );
}

/** One-tap actions (no inputs): run, toast what the book worked out, refresh. */
export function useQuickAction() {
  const { toast, bump, action } = useSession();
  return async (actionId: string, body: { key?: string; context?: api.Row; values?: api.Row; watch?: string }, opts: { silent?: boolean } = {}) => {
    try {
      const r = await api.act(actionId, body); bump();
      if (!opts.silent) toast({ tone: "green", title: action(actionId).label, detail: describeWatched(r.watched) }); return r;
    } catch (e: any) { toast({ tone: "red", title: "The database refused", detail: e.message }); return null; }
  };
}
