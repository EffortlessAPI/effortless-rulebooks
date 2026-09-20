import { createContext, useContext, useEffect, useState, type CSSProperties, type ReactNode } from "react";
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
  const { shell, signOut, explain, setExplain } = useSession();
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
          <button className="toggle" role="switch" aria-checked={explain} style={{ marginTop: 14, marginBottom: 0 }} onClick={() => setExplain(!explain)}>
            <span>Explain every value</span><span className="sw" />
          </button>
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

export const Tag = ({ tone, f, t, children }: { tone: "red" | "green" | "amber" | "blue" | "purple" | "grey" | "black" | "solid-red"; f?: string; t?: string; children: ReactNode }) =>
  <span className={`tag ${tone}`}>{children}{f && <Why f={f} t={t} />}</span>;
export const Err = ({ error }: { error: string | null }) => (error ? <div className="err">{error}</div> : null);
export const Loading = () => <div className="empty">Reading the views…</div>;

// ---------------------------------------------------------------------------
// Provenance, everywhere a value is shown.
//
// Every key value on screen can say where it came from, and the answer is the
// rulebook's own field census — not a tooltip somebody wrote. An explainer
// names a (table, column) pair; the server resolves it and refuses loudly if
// the catalog has never heard of it, so a typo here shows as a red error rather
// than an empty popup that reads like "this value came from nowhere".
//
// Naming the pair is deliberately explicit. The label beside a value is prose
// ("In the written procedure"), and guessing the column from it would be the
// app inventing provenance, which is the one thing this screen must not do.
// ---------------------------------------------------------------------------

/** The table whose columns the explainers in this subtree name. */
const TableCtx = createContext<string | null>(null);
export const Explains = ({ t, children }: { t: string; children: ReactNode }) => <TableCtx.Provider value={t}>{children}</TableCtx.Provider>;

/** PascalCase or snake_case to something a person reads. */
export const fieldLabel = (name: string) =>
  name.replace(/_/g, " ").replace(/([a-z0-9])([A-Z])/g, "$1 $2").toLowerCase().replace(/^./, (c) => c.toUpperCase());

/**
 * The affordance: a quiet mark beside a value that opens its provenance.
 * Renders nothing when the explainers are switched off at sign-in.
 */
export function Why({ f, t, label }: { f: string; t?: string; label?: string }) {
  const { explain } = useSession();
  const inherited = useContext(TableCtx);
  const [open, setOpen] = useState(false);
  const table = t ?? inherited;
  if (!explain) return null;
  if (!table) {
    // Never a silent no-op: an explainer with no table can explain nothing.
    throw new Error(`<Why f="${f}"> names no table. Pass t="<table>", or wrap the block in <Explains t="<table>">.`);
  }
  return (
    <>
      <button className="why" title={`Where does ${label || fieldLabel(f)} come from?`} aria-label={`Where does ${label || fieldLabel(f)} come from?`}
        onClick={(e) => { e.stopPropagation(); e.preventDefault(); setOpen(true); }}>ƒ</button>
      {open && <FieldSheet t={table} f={f} onClose={() => setOpen(false)} />}
    </>
  );
}

/** A `<dl class="kv">` whose rows are `<Fact>`s, all naming columns of one table. */
export const KV = ({ t, children, style }: { t: string; children: ReactNode; style?: CSSProperties }) =>
  <Explains t={t}><dl className="kv" style={style}>{children}</dl></Explains>;

/** One labelled value inside a `<KV>`, with its own explainer. */
export const Fact = ({ f, label, tone, children }: { f: string; label: string; tone?: "fact" | "derived" | "bad" | ""; children: ReactNode }) => (
  <><dt>{label}<Why f={f} label={label} /></dt><dd className={tone || undefined}>{children}</dd></>
);

/** An inline value that is not in a `<KV>` — a cell, a stat, a sentence. */
export const Val = ({ f, t, tone, children }: { f: string; t?: string; tone?: "fact" | "derived" | "bad" | ""; children: ReactNode }) => (
  <b className={tone || undefined}>{children}<Why f={f} t={t} /></b>
);

/** A table heading that explains its column. */
export const Th = ({ f, t, children, style }: { f: string; t?: string; children: ReactNode; style?: CSSProperties }) => (
  <th style={style}>{children}<Why f={f} t={t} /></th>
);

/** A headline number with its caption, e.g. the counts a closure worked out. */
export const Big = ({ f, t, caption, tone, children }: { f: string; t?: string; caption: string; tone?: string; children: ReactNode }) => (
  <div><div className="big" style={tone ? { color: tone } : undefined}>{children}</div><div className="sub">{caption}<Why f={f} t={t} label={caption} /></div></div>
);

/** Said once per screen, so the mark is discoverable without a manual. Hidden with the explainers. */
export function ExplainerNote() {
  const { explain } = useSession();
  if (!explain) return null;
  return (
    <div className="explainer-note">
      <b className="mono" style={{ fontSize: 14 }}>ƒ</b>
      <span>Tap the mark beside any value to see the rule behind it, the question a named role asked for it, and how much of the data it actually speaks about. Switch it off in your account.</span>
    </div>
  );
}

const KIND: Record<string, string> = {
  raw: "Somebody recorded this. It is typed in by a person or stamped by the app — the rulebook does not work it out.",
  calculated: "The rulebook works this out from other values. Nobody types it, and nobody can type over it.",
  lookup: "The rulebook copies this from a related record, so the two can never disagree.",
  aggregation: "The rulebook counts or adds this up across related records every time it is read.",
  relationship: "A link to another record. The rulebook keeps it pointing at something real.",
};

/** The popup. One field, and a trail back through the fields it was worked out from. */
export function FieldSheet({ t, f, onClose }: { t: string; f: string; onClose: () => void }) {
  const [trail, setTrail] = useState<{ t: string; f: string }[]>([{ t, f }]);
  const at = trail[trail.length - 1];
  const [p, setP] = useState<api.Provenance | null>(null);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    let live = true; setP(null); setError(null);
    api.provenance(at.t, at.f).then((x) => live && setP(x)).catch((e) => live && setError(e.message));
    return () => { live = false; };
  }, [at.t, at.f]);

  const fld = p?.field;
  const title = fld ? fieldLabel(fld.field_name) : fieldLabel(at.f);
  return (
    <Sheet title={title} desc={`${at.t}.${at.f}`} onClose={onClose}>
      {trail.length > 1 && (
        <button className="btn ghost sm" style={{ marginBottom: 12 }} onClick={() => setTrail((x) => x.slice(0, -1))}>
          ‹ back to {fieldLabel(trail[trail.length - 2].f)}
        </button>
      )}
      <Err error={error} />
      {!p && !error && <Loading />}
      {p && fld && (
        <>
          <div className="card tight">
            <div className="row wrap" style={{ marginBottom: 8 }}>
              <Tag tone={fld.is_derived ? "green" : fld.field_type === "relationship" ? "blue" : "blue"}>{fld.field_type}</Tag>
              <Tag tone="grey">{fld.datatype}</Tag>
              {fld.is_witness && <Tag tone="purple">a witness</Tag>}
              {fld.is_substrate_contested && <Tag tone="red">{fld.disagreeing_substrate_count} substrates disagree</Tag>}
            </div>
            <p className="sub" style={{ color: "var(--ink-2)" }}>{KIND[fld.field_type] || fld.field_type}</p>
          </div>

          {fld.formula && (
            <div className="card tight">
              <div className="section" style={{ margin: "0 0 6px" }}>The rule, as the rulebook states it</div>
              <div className="rule" style={{ overflowWrap: "anywhere" }}>{fld.formula}</div>
              <p className="provenance">This is the whole definition. The transpiler turned it into the SQL behind <code>{p.view}.{p.column}</code>; the same text built every other substrate.</p>
            </div>
          )}

          {p.inputs.length > 0 && (
            <div className="card tight">
              <div className="section" style={{ margin: "0 0 6px" }}>Worked out from <span className="count">{p.inputs.length}</span></div>
              {p.inputs.map((i) => (
                <div key={i.key} className="row" style={{ padding: "6px 0", borderTop: "1px solid var(--line)" }}>
                  <span className="grow">
                    <b className={i.isDerived ? "derived" : "fact"}>{fieldLabel(i.field)}</b>
                    {!i.isLocal && <span className="sub"> · from {i.table}</span>}
                    {!i.exists && <span className="sub" style={{ color: "var(--red)" }}> · the catalog cannot place this reference</span>}
                  </span>
                  {i.exists && i.isLocal
                    ? <button className="btn sm ghost" onClick={() => setTrail((x) => [...x, { t: at.t, f: i.column }])}>Why?</button>
                    : <Tag tone="grey">{i.fieldType || "unknown"}</Tag>}
                </div>
              ))}
              {p.inputs.some((i) => !i.isLocal) && <p className="provenance">A value from another table is reached through a link, one hop at a time. Open that record to follow it further.</p>}
            </div>
          )}

          {p.question ? (
            <div className="card tight">
              <div className="section" style={{ margin: "0 0 6px" }}>This field exists because somebody asked</div>
              <div className="quote">“{p.question.question_text}”<span className="by">asked by {human(String(p.question.asking_role))}{p.role?.label ? ` · ${p.role.label}` : ""}</span></div>
              {p.question.why_it_matters && <p className="sub" style={{ marginTop: 8, color: "var(--ink-2)" }}>{String(p.question.why_it_matters)}</p>}
              {p.question.answerable_before === false && <div style={{ marginTop: 8 }}><Tag tone="amber">the model could not answer this before</Tag></div>}
              {p.loop && <p className="provenance" style={{ marginTop: 10 }}><b>Round {String(p.loop.loop_number)}: {String(p.loop.title).replace(/^Loop \d+: /, "")}</b>{p.loop.premise ? ` — ${p.loop.premise}` : ""}</p>}
              {p.siblings.length > 0 && (
                <div className="row wrap" style={{ marginTop: 10 }}>
                  <span className="sub" style={{ width: "100%" }}>Answered together with:</span>
                  {p.siblings.slice(0, 8).map((s) => <Tag key={s.rulebook_field_id} tone="grey">{fieldLabel(s.field_name)}</Tag>)}
                </div>
              )}
            </div>
          ) : (
            <div className="card tight">
              <div className="section" style={{ margin: "0 0 6px" }}>No question is recorded behind this field</div>
              <p className="sub">It predates the exercise that tied every new field to a named role's question. Inventing a motivation for it now would be making something up, so the record stays empty.</p>
            </div>
          )}

          <div className="card tight">
            <div className="section" style={{ margin: "0 0 6px" }}>Can it tell anything apart?</div>
            <dl className="kv">
              <dt>Rows that carry a value</dt><dd className="derived">{p.reading ? `${p.reading.populated} of ${p.reading.total}` : "—"}</dd>
              <dt>Different values across the data</dt><dd className={fld.is_discriminating ? "derived" : "bad"}>{fld.measured_distinct_value_count}</dd>
              <dt>Substrates that disagree</dt><dd className={fld.is_substrate_contested ? "bad" : "derived"}>{fld.disagreeing_substrate_count}</dd>
            </dl>
            {!fld.is_discriminating && <p className="sub" style={{ marginTop: 8, color: "var(--red)" }}>This column reads the same on every row, so it cannot separate one record from another. It looks like a working field and states nothing.</p>}
          </div>

          {p.table && (
            <div className="card tight">
              <div className="section" style={{ margin: "0 0 6px" }}>What kind of thing this is a fact about</div>
              <p className="sub"><b>{String(p.table.rulebook_table_id)}</b>{p.table.subject_area ? ` · ${String(p.table.subject_area)}` : ""} · {String(p.table.field_count)} fields</p>
              <div className="row wrap" style={{ marginTop: 8 }}>
                {p.mappings.map((m, i) => <Tag key={i} tone={m.mapping_relation === "exact" ? "green" : m.mapping_relation === "extension" ? "purple" : "blue"}>{String(m.mapping_relation)}: {String(m.target_iri).split(/[#/]/).pop()}</Tag>)}
                {p.mappings.length === 0 && <Tag tone="grey">no standard term recorded</Tag>}
              </div>
              <p className="provenance">A green term is PKO's own; blue reuses another published standard; purple is this model's extension, named as one rather than dressed up as standard.</p>
            </div>
          )}
        </>
      )}
    </Sheet>
  );
}

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
  const missing = inputs.some((f) => f.input_kind !== "toggle" && f.input_kind !== "note" && (values[f.field_name] == null || values[f.field_name] === ""));

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
        if (f.input_kind === "longtext" || f.input_kind === "note") return (<label key={f.field_name} className="field"><span>{f.field_label}</span><textarea value={v || ""} onChange={(e) => set(f.field_name, e.target.value)} /></label>);
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
