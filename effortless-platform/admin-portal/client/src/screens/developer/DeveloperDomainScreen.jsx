import { useNavigate, useParams } from "react-router-dom";
import { useState } from "react";
import ScreenHeader from "../../components/ScreenHeader.jsx";
import RichText, { RichInline } from "../../components/RichText.jsx";

// The reception desk for one domain.
//
// Reads only from props that already flow through usePortal():
//   - `projects` for tile-level _meta (tagline, motif, signature rows)
//   - `rulebook` for the live entity data, calculated-field schema, and
//     per-entity / per-rule importance flags
// Nothing fetched here — the heavy reads already happened in usePortal().

export default function DeveloperDomainScreen({ screen, rulebook, projects }) {
  const navigate = useNavigate();
  const { domain } = useParams();
  const dom = (projects?.projects || []).find((p) => p.id === domain) || {};
  const meta = rulebook?._meta || {};

  const entities = entityList(rulebook);
  const important = entities.filter((e) => e.entity.important);
  const otherCount = entities.length - important.length;
  const rules = importantRules(rulebook);
  const otherRuleCount = countCalculatedFields(rulebook) - rules.length;

  const motif = dom.motif || meta.motif || "default";
  const palette = dom.motifPalette || meta.motif_palette || {};
  const heroStyle = {};
  if (palette.primary) heroStyle["--motif-primary"] = palette.primary;
  if (palette.accent)  heroStyle["--motif-accent"]  = palette.accent;
  if (palette.ink)     heroStyle["--motif-ink"]     = palette.ink;

  return (
    <>
      <ScreenHeader screen={screen} />

      <section className={`reception-hero motif-${motif}`} style={heroStyle}>
        <div className="reception-hero-band">
          {dom.logoUrl && (
            <img src={dom.logoUrl} alt="" className="reception-hero-logo" />
          )}
          <div>
            <div className="reception-hero-slug">{domain}</div>
            <h1 className="reception-hero-name">{dom.displayName || dom.name || rulebook?.Name || domain}</h1>
            {dom.tagline && <div className="reception-hero-tagline">{dom.tagline}</div>}
          </div>
        </div>

        <div className="reception-hero-body">
          <div className="reception-hero-left">
            {meta.description_rich
              ? <RichText text={meta.description_rich} className="reception-description" />
              : <p className="reception-description muted">— no <code>description_rich</code> authored in <code>_meta</code> yet —</p>
            }
            <TelemetryStrip entities={entities} />
            {meta.journal_seed && (
              <div className="reception-journal">
                <span className="reception-journal-marker">📖</span>
                <span>{meta.journal_seed}</span>
              </div>
            )}
          </div>

          <div className="reception-hero-right">
            <h3 className="muted small">USE CASES</h3>
            {Array.isArray(meta.use_cases) && meta.use_cases.length > 0 ? (
              <ol className="use-case-list">
                {meta.use_cases.map((uc, i) => (
                  <li key={i}><RichInline text={uc} /></li>
                ))}
              </ol>
            ) : (
              <p className="muted small">— no use cases authored in <code>_meta.use_cases</code> yet —</p>
            )}
          </div>
        </div>
      </section>

      {important.length === 0 ? (
        <NoImportanceFlags entities={entities} domain={domain} navigate={navigate} />
      ) : (
        important.map(({ name, entity }) => (
          <EntityScroller key={name} name={name} entity={entity} domain={domain} navigate={navigate} />
        ))
      )}

      {otherCount > 0 && (
        <div className="reception-more">
          <button className="link-like" onClick={() => navigate(`/developer/${domain}/entities`)}>
            +{otherCount} more {otherCount === 1 ? "table" : "tables"} →
          </button>
        </div>
      )}

      <RulesPanel rules={rules} otherCount={otherRuleCount} domain={domain} navigate={navigate} />

      <footer className="reception-footer muted small">
        <span>{entities.length} {entities.length === 1 ? "entity" : "entities"}</span>
        <span>·</span>
        <span>{countCalculatedFields(rulebook)} calculated/lookup/aggregation fields</span>
        <span>·</span>
        <button className="link-like" onClick={() => navigate(`/developer/${domain}/rulebook-json`)}>view JSON</button>
        <span>·</span>
        <button className="link-like" onClick={() => navigate(`/developer/${domain}/builds`)}>build receipts</button>
        <span>·</span>
        <button className="link-like" onClick={() => navigate(`/developer/${domain}/explorer`)}>open explorer</button>
      </footer>
    </>
  );
}

// ─────────────────────────────────────────────────────────────────────────────
// Telemetry strip

function TelemetryStrip({ entities }) {
  if (entities.length === 0) return null;
  const parts = [];
  for (const { name, entity } of entities) {
    const n = Array.isArray(entity.data) ? entity.data.length : 0;
    parts.push(<span key={name}><strong>{n}</strong> {name.toLowerCase()}</span>);
  }
  const pending = pendingApprovalCount(entities);
  if (pending != null) {
    parts.push(<span key="pending" className="telemetry-warn"><strong>{pending}</strong> awaiting approval</span>);
  }
  // Join with middots.
  const out = [];
  parts.forEach((p, i) => {
    if (i > 0) out.push(<span key={`sep-${i}`} className="telemetry-sep">·</span>);
    out.push(p);
  });
  return <div className="telemetry-strip">{out}</div>;
}

// Detect the ACME-style "awaiting approval" pattern generically: rows in any
// entity that have BOTH an IsApproved field set to false AND a ProjectType-
// RequiresManagerApproval (or similarly-named) field set to true. Returns
// null when no such pattern can be derived — never a fake zero.
function pendingApprovalCount(entities) {
  let total = 0;
  let found = false;
  for (const { entity } of entities) {
    if (!Array.isArray(entity.data) || !Array.isArray(entity.schema)) continue;
    const hasApproved = entity.schema.some((f) => f.name === "IsApproved");
    const gateField = entity.schema.find((f) => /RequiresManagerApproval$/i.test(f.name))?.name;
    if (!hasApproved && !gateField) continue;
    found = true;
    for (const row of entity.data) {
      const approved = !!row["IsApproved"];
      const gated = gateField ? truthy(row[gateField]) : true;
      if (!approved && gated) total += 1;
    }
  }
  return found ? total : null;
}

function truthy(v) {
  if (v === true) return true;
  if (typeof v === "string") return /^(true|yes|1)$/i.test(v.trim());
  return false;
}

// ─────────────────────────────────────────────────────────────────────────────
// Entity scroller — one horizontal strip per important entity.

function EntityScroller({ name, entity, domain, navigate }) {
  const fields = entity.important_fields && entity.important_fields.length
    ? entity.important_fields
    : (entity.schema || []).slice(0, 3).map((f) => f.name);
  const labelField = pickLabelField(fields);
  const rows = Array.isArray(entity.data) ? entity.data : [];
  const pkField = (entity.schema || []).find((f) => /Id$/i.test(f.name))?.name;

  return (
    <section className="entity-scroller">
      <header className="entity-scroller-header">
        <h2>{name}</h2>
        {entity.summary_rich && (
          <div className="entity-scroller-summary">
            <RichInline text={entity.summary_rich} />
          </div>
        )}
      </header>
      {rows.length === 0 ? (
        <div className="muted small" style={{ padding: "12px 0" }}>— no rows in <code>data[]</code> —</div>
      ) : (
        <div className="entity-scroller-strip">
          {rows.map((row, i) => (
            <article key={i} className="entity-card" onClick={() => navigate(`/developer/${domain}/data`)}>
              <header className="entity-card-name">
                {labelField ? (row[labelField] ?? "—") : (pkField ? row[pkField] : "—")}
              </header>
              <dl className="entity-card-fields">
                {fields.filter((f) => f !== labelField).map((f) => (
                  <div key={f} className="entity-card-field">
                    <dt>{f}</dt>
                    <dd>{formatValue(row[f])}</dd>
                  </div>
                ))}
              </dl>
            </article>
          ))}
        </div>
      )}
    </section>
  );
}

function pickLabelField(fields) {
  return (
    fields.find((f) => f === "Name") ||
    fields.find((f) => f === "FullName") ||
    fields.find((f) => /Name$/.test(f)) ||
    fields[0] ||
    null
  );
}

function formatValue(v) {
  if (v == null || v === "") return <span className="muted">—</span>;
  if (v === true || v === "True" || v === "true") return "✓";
  if (v === false || v === "False" || v === "false") return "✗";
  if (typeof v === "string" && v.length > 64) return v.slice(0, 60) + "…";
  return String(v);
}

// ─────────────────────────────────────────────────────────────────────────────
// Rules panel — one row per important calculated/lookup/aggregation field.

function RulesPanel({ rules, otherCount, domain, navigate }) {
  if (rules.length === 0 && otherCount === 0) return null;
  return (
    <section className="rules-panel">
      <header className="rules-panel-header">
        <h2>Rules that matter</h2>
        <span className="muted small">
          {rules.length} featured · {otherCount} more in the DAG
        </span>
      </header>
      {rules.length === 0 ? (
        <div className="muted small" style={{ padding: "8px 0" }}>
          — no rules flagged <code>important: true</code> yet —
        </div>
      ) : (
        <ul className="rules-list">
          {rules.map((r, i) => <RuleRow key={i} rule={r} />)}
        </ul>
      )}
      {otherCount > 0 && (
        <div className="reception-more">
          <button className="link-like" onClick={() => navigate(`/developer/${domain}/formulas`)}>
            +{otherCount} more calculated fields →
          </button>
        </div>
      )}
    </section>
  );
}

function RuleRow({ rule }) {
  const [open, setOpen] = useState(false);
  return (
    <li className={`rule-row ${open ? "open" : ""}`}>
      <button className="rule-row-summary" onClick={() => setOpen((o) => !o)}>
        <span className="rule-row-type">{rule.field.type}</span>
        <span className="rule-row-path"><code>{rule.entity}.{rule.field.name}</code></span>
        <span className="muted small rule-row-chev">{open ? "▾" : "▸"}</span>
      </button>
      {open && (
        <div className="rule-row-body">
          {rule.field.explanation_rich && (
            <RichText text={rule.field.explanation_rich} />
          )}
          <pre className="rule-row-formula">{rule.field.formula}</pre>
          {rule.example && (
            <div className="rule-row-example">
              <div className="muted small">Worked example on <code>{rule.example.pk}</code>:</div>
              <div className="rule-row-example-value">
                <strong>{rule.entity}.{rule.field.name}</strong> = {formatValue(rule.example.value)}
              </div>
            </div>
          )}
        </div>
      )}
    </li>
  );
}

// ─────────────────────────────────────────────────────────────────────────────
// No-importance fallback (the page still has to render for un-curated domains).

function NoImportanceFlags({ entities, domain, navigate }) {
  return (
    <section className="reception-empty">
      <p className="muted">
        No entity in this rulebook has <code>important: true</code> yet. Author one in
        <code> _meta</code> to feature it here.
      </p>
      <div className="domain-gallery" style={{ marginTop: 12 }}>
        {entities.map(({ name, entity }) => (
          <div key={name} className="card clickable" onClick={() => navigate(`/developer/${domain}/data`)}>
            <h3>{name}</h3>
            <div className="sub">{Array.isArray(entity.data) ? entity.data.length : 0} rows</div>
          </div>
        ))}
      </div>
    </section>
  );
}

// ─────────────────────────────────────────────────────────────────────────────
// Pure helpers — pull from rulebook structure only.

function entityList(rb) {
  if (!rb) return [];
  const out = [];
  for (const [name, value] of Object.entries(rb)) {
    if (name.startsWith("$") || name.startsWith("_")) continue;
    if (name === "Name" || name === "Description") continue;
    if (!value || typeof value !== "object" || !Array.isArray(value.schema)) continue;
    out.push({ name, entity: value });
  }
  return out;
}

function countCalculatedFields(rb) {
  let n = 0;
  for (const { entity } of entityList(rb)) {
    for (const f of entity.schema) {
      if (["calculated", "lookup", "aggregation"].includes(f.type)) n += 1;
    }
  }
  return n;
}

function importantRules(rb) {
  const out = [];
  for (const { name, entity } of entityList(rb)) {
    for (const f of entity.schema) {
      if (!f.important) continue;
      if (!["calculated", "lookup", "aggregation"].includes(f.type)) continue;
      let example = null;
      if (Array.isArray(entity.data) && entity.data.length > 0) {
        const pkField = entity.schema.find((s) => /Id$/i.test(s.name))?.name;
        const row = entity.data.find((r) => r[f.name] != null && r[f.name] !== "") || entity.data[0];
        if (row) {
          example = {
            pk: pkField ? row[pkField] : "row-0",
            value: row[f.name],
          };
        }
      }
      out.push({ entity: name, field: f, example });
    }
  }
  return out;
}
