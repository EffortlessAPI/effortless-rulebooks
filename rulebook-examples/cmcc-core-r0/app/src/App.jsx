import { useEffect, useState } from 'react';

// Every value on screen comes straight from a vw_* view column (via
// /api/conformance-matrix, which only joins rows server-side) or from a raw
// vw_* SELECT in the "All views" tab. Nothing is recomputed here: no counts,
// no pass/fail logic, no fallbacks -- AnswerKeyOk / ReflectionOk / Conformant
// are columns of vw_substrates, computed by the rulebook's own formulas.

function fmt(v) {
  if (v === null || v === undefined) return '—';
  if (v === true) return '✓';
  if (v === false) return '✗';
  return String(v);
}

async function getJson(url) {
  const r = await fetch(url);
  const body = await r.json();
  if (!r.ok) throw new Error(body.error || `${url} -> HTTP ${r.status}`);
  return body;
}

function useFetch(url) {
  const [state, setState] = useState({ data: null, error: null });
  useEffect(() => {
    let live = true;
    getJson(url)
      .then(d => live && setState({ data: d, error: null }))
      .catch(e => live && setState({ data: null, error: e.message }));
    return () => { live = false; };
  }, [url]);
  return state;
}

function ErrorBox({ error }) {
  return <div className="error">Error: {error}</div>;
}

function Section({ id, title, view, children }) {
  return (
    <section className="card" id={id}>
      <header className="card-head">
        <h2>{title}</h2>
        {view && <code className="view-tag">{view}</code>}
      </header>
      {children}
    </section>
  );
}

function ViewTable({ columns, rows, rowKey, rowClass }) {
  return (
    <div className="grid-wrap">
      <table className="grid">
        <thead>
          <tr>{columns.map(c => <th key={c}>{c}</th>)}</tr>
        </thead>
        <tbody>
          {rows.map((row, i) => (
            <tr key={rowKey ? row[rowKey] : i} className={rowClass ? rowClass(row) : ''}>
              {columns.map(c => <td key={c}>{fmt(row[c])}</td>)}
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}

/* 1. The two-gate conformance matrix ------------------------------------- */
function ConformanceMatrix({ data }) {
  const { substrates, inputHistories, conditions, answerKeyResults, reflectionResults } = data;

  const akCell = (substrateId, inputHistoryId) =>
    answerKeyResults.find(r => r.substrate === substrateId && r.input_history === inputHistoryId);
  const rfCell = (substrateId, conditionId) =>
    reflectionResults.find(r => r.substrate === substrateId && r.reflection_condition === conditionId);

  const outcomeClass = outcome => {
    if (outcome === 'pass') return 'cell-true';
    if (outcome === 'fail') return 'cell-false';
    if (outcome === 'delegated') return 'cell-null';
    return 'cell-none';
  };
  const outcomeGlyph = outcome => {
    if (outcome === 'pass') return '✓';
    if (outcome === 'fail') return '✗';
    if (outcome === 'delegated') return '→ AK';
    return '·';
  };

  return (
    <Section id="matrix" title="The conformance matrix — two gates, three substrates" view="vw_substrates">
      <p className="lede">
        Every substrate computes the same answers (<strong>answer key</strong>, §11) over both
        input histories. Only <code>transparent</code> is also a <strong>representation</strong> in the
        paper's sense (<strong>reflection</strong>, §13) — <code>tangled</code> and <code>interpreter</code> are
        controls, deliberately built to fail it. Condition 4 (local adequacy) is always{' '}
        <em>delegated</em> to the answer key by design, never counted as a failure.
      </p>
      <div className="grid-wrap">
        <table className="matrix">
          <thead>
            <tr>
              <th className="corner">substrate</th>
              {inputHistories.map(ih => (
                <th key={ih.input_history_id} title={ih.description}>{ih.input_history_id}</th>
              ))}
              {conditions.map(c => (
                <th key={c.condition_id} title={c.description}>{c.condition_number}. {c.title}</th>
              ))}
              <th>answer key</th>
              <th>reflection</th>
              <th>conformant</th>
            </tr>
          </thead>
          <tbody>
            {substrates.map(s => (
              <tr key={s.substrate_id}>
                <th title={s.description}>
                  {s.substrate_id}
                  {s.is_control && <span className="badge badge-muted">control</span>}
                </th>
                {inputHistories.map(ih => {
                  const c = akCell(s.substrate_id, ih.input_history_id);
                  return (
                    <td key={ih.input_history_id} className={outcomeClass(c?.outcome)}
                      title={c ? `${c.traces_produced} traces produced, ${c.permitted} permitted` : 'no result recorded'}>
                      {outcomeGlyph(c?.outcome)}
                    </td>
                  );
                })}
                {conditions.map(c => {
                  const r = rfCell(s.substrate_id, c.condition_id);
                  return (
                    <td key={c.condition_id} className={outcomeClass(r?.outcome)} title={c.description}>
                      {outcomeGlyph(r?.outcome)}
                    </td>
                  );
                })}
                <td className={s.answer_key_ok ? 'cell-true' : 'cell-false'}>{fmt(s.answer_key_ok)}</td>
                <td className={s.reflection_ok ? 'cell-true' : 'cell-false'}>{fmt(s.reflection_ok)}</td>
                <td className={s.conformant ? 'cell-true' : 'cell-false'}>{fmt(s.conformant)}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
      <details className="raw">
        <summary>Substrate descriptions</summary>
        <ViewTable
          columns={['substrate_id', 'description', 'paper_section_ref', 'is_control']}
          rows={substrates}
          rowKey="substrate_id"
        />
      </details>
    </Section>
  );
}

/* 2. The R0 spec, browsed --------------------------------------------------*/
function SpecSection({ edb, commands, rules, transitions, constraints, observables }) {
  return (
    <Section id="spec" title="The R0 spec (r0/spec.json), as rulebook data">
      <p className="lede">
        This is a structural transcription, not a computation: the spec's own relations, commands,
        rules, transitions and constraints, each with the paper section it exercises.
      </p>
      <h3>EDB relations <code className="view-tag">vw_edb_relations</code></h3>
      <ViewTable columns={['relation_id', 'fields', 'field_count', 'description']} rows={edb.rows} rowKey="relation_id" />
      <h3>Commands <code className="view-tag">vw_commands</code></h3>
      <ViewTable columns={['command_id', 'params', 'param_count', 'description']} rows={commands.rows} rowKey="command_id" />
      <h3>Transitions <code className="view-tag">vw_transitions</code></h3>
      <ViewTable columns={['transition_id', 'command', 'guard', 'description']} rows={transitions.rows} rowKey="transition_id" />
      <h3>Integrity constraints <code className="view-tag">vw_integrity_constraints</code></h3>
      <ViewTable columns={['constraint_id', 'rule', 'description']} rows={constraints.rows} rowKey="constraint_id" />
      <h3>Observables <code className="view-tag">vw_observables</code></h3>
      <ViewTable columns={['observable_id', 'exposed_predicate', 'description']} rows={observables.rows} rowKey="observable_id" />
      <details className="raw">
        <summary>All {rules.rows.length} rules (vw_rules)</summary>
        <ViewTable
          columns={['rule_id', 'head_predicate', 'kind', 'paper_section_ref', 'description']}
          rows={rules.rows}
          rowKey="rule_id"
        />
      </details>
    </Section>
  );
}

/* Domain screen ------------------------------------------------------------*/
function DomainScreen() {
  const matrix = useFetch('/api/conformance-matrix');
  const edb = useFetch('/api/views/vw_edb_relations');
  const commands = useFetch('/api/views/vw_commands');
  const rules = useFetch('/api/views/vw_rules');
  const transitions = useFetch('/api/views/vw_transitions');
  const constraints = useFetch('/api/views/vw_integrity_constraints');
  const observables = useFetch('/api/views/vw_observables');

  const all = { matrix, edb, commands, rules, transitions, constraints, observables };
  const errors = Object.entries(all).filter(([, s]) => s.error);
  if (errors.length) {
    return <div>{errors.map(([k, s]) => <ErrorBox key={k} error={s.error} />)}</div>;
  }
  if (Object.values(all).some(s => !s.data)) return <div className="loading">Loading views…</div>;

  return (
    <>
      <ConformanceMatrix data={matrix.data} />
      <SpecSection
        edb={edb.data} commands={commands.data} rules={rules.data}
        transitions={transitions.data} constraints={constraints.data} observables={observables.data}
      />
    </>
  );
}

/* All-views browser ---------------------------------------------------------*/
function ViewsBrowser() {
  const [views, setViews] = useState(null);
  const [error, setError] = useState(null);
  const [selected, setSelected] = useState(null);
  const [data, setData] = useState(null);

  useEffect(() => {
    getJson('/api/views').then(setViews).catch(e => setError(e.message));
  }, []);

  useEffect(() => {
    if (!selected) return;
    setData(null);
    getJson(`/api/views/${selected}`).then(setData).catch(e => setError(e.message));
  }, [selected]);

  return (
    <div className="browser">
      <aside className="sidebar">
        <h2>Views</h2>
        {error && <ErrorBox error={error} />}
        <ul>
          {(views || []).map(v => (
            <li key={v} className={v === selected ? 'active' : ''} onClick={() => setSelected(v)}>{v}</li>
          ))}
        </ul>
      </aside>
      <div className="browser-main">
        <h2>{selected || 'Pick a view'}</h2>
        {selected && !data && !error && <div className="loading">Loading…</div>}
        {data && (
          <>
            <p className="muted">{data.rows.length} rows · {data.columns.length} columns</p>
            <ViewTable columns={data.columns} rows={data.rows} />
          </>
        )}
      </div>
    </div>
  );
}

export default function App() {
  const [tab, setTab] = useState('story');
  return (
    <div className="app">
      <header className="topbar">
        <div>
          <h1>CMCC-Core R0</h1>
          <p className="subtitle">Representation theorem, made runnable · answer key × reflection over three substrates</p>
        </div>
        <nav className="tabs">
          <button className={tab === 'story' ? 'active' : ''} onClick={() => setTab('story')}>Explorer</button>
          <button className={tab === 'views' ? 'active' : ''} onClick={() => setTab('views')}>All views</button>
        </nav>
      </header>
      <main className="main">
        {tab === 'story' ? <DomainScreen /> : <ViewsBrowser />}
      </main>
      <footer className="foot">
        Every value shown is a column of a <code>vw_*</code> view in <code>erb_cmcc_core_r0</code>. The view is the contract.
      </footer>
    </div>
  );
}
