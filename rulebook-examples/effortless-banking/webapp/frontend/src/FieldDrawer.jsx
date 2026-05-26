import React, { createContext, useContext, useEffect, useState } from 'react';
import { api } from './api.js';

const Ctx = createContext(null);

export function FieldDrawerProvider({ children }) {
  const [target, setTarget] = useState(null); // { table, field, value? }
  const [meta, setMeta] = useState(null);

  useEffect(() => {
    if (!target) return setMeta(null);
    setMeta(null);
    api.get(`/api/rulebook/field/${target.table}/${target.field}`)
      .then(setMeta)
      .catch((e) => setMeta({ error: e.message }));
  }, [target]);

  const open = (table, field, value) => setTarget({ table, field, value });
  const close = () => setTarget(null);

  return (
    <Ctx.Provider value={{ open, close, target }}>
      {children}
      {target && (
        <>
          <div className="drawer-backdrop" onClick={close} />
          <aside className="drawer" onClick={(e) => e.stopPropagation()}>
            <div className="row">
              <div className="grow">
                <div className="muted mono">{target.table}.{target.field}</div>
                <h3>{target.field}</h3>
              </div>
              <button className="ghost" onClick={close}>Close</button>
            </div>
            {!meta && <div className="empty">Loading field metadata…</div>}
            {meta?.error && <div className="empty">Error: {meta.error}</div>}
            {meta?.field && (
              <>
                <dl className="kv">
                  <dt>Type</dt><dd>{meta.field.type}</dd>
                  <dt>Datatype</dt><dd>{meta.field.datatype}</dd>
                  <dt>Nullable</dt><dd>{String(meta.field.nullable)}</dd>
                  {target.value !== undefined && (
                    <>
                      <dt>Current value</dt>
                      <dd className="mono">{formatValue(target.value)}</dd>
                    </>
                  )}
                </dl>
                {meta.field.Description && (
                  <>
                    <div className="section-title">Description</div>
                    <div>{meta.field.Description}</div>
                  </>
                )}
                {meta.field.formula && (
                  <>
                    <div className="section-title">Formula</div>
                    <div className="formula">{meta.field.formula}</div>
                  </>
                )}
                {meta.deps?.length > 0 && (
                  <>
                    <div className="section-title">Depends on</div>
                    <div>
                      {meta.deps.map((d) => (
                        <span
                          key={d.name}
                          className="dep-chip"
                          onClick={() => open(d.table, d.name)}
                          title="Drill into dependency"
                        >
                          {d.table}.{d.name}
                        </span>
                      ))}
                    </div>
                  </>
                )}
                <div className="section-title">Explainer DAG</div>
                <div className="muted" style={{ fontSize: 12 }}>
                  Reserved for a future rulebook-to-react-explainer-dag transpiler.
                  For now this drawer walks formula dependencies one hop at a time.
                </div>
              </>
            )}
          </aside>
        </>
      )}
    </Ctx.Provider>
  );
}

function formatValue(v) {
  if (v === null || v === undefined) return '∅';
  if (typeof v === 'object') return JSON.stringify(v);
  return String(v);
}

export const useFieldDrawer = () => useContext(Ctx);
