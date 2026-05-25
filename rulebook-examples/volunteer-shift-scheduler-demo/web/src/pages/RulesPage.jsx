import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { api } from '../api.js';

// /rules — flat browser of the declarative schema. Every row is a rule.
// Click a computed field's table/field → /dag/<table>/<field>.

export default function RulesPage() {
  const [schema, setSchema] = useState(null);
  useEffect(() => { api.schema().then(setSchema); }, []);

  if (!schema) return <main>Loading rules…</main>;
  const computed = Object.entries(schema.computed);

  return (
    <main>
      <div className="crumbs">
        <Link to="/">Events</Link> <span>›</span> <span>Rules</span>
      </div>

      <h2>Schema · {Object.keys(schema.tables).length} tables · {computed.length} computed fields</h2>
      <p className="muted">
        Every derived number on the dashboard ultimately comes from one of the
        rules below. Click a row to walk it back to raw inputs.
      </p>

      <div className="card" style={{ padding: 0, overflow: 'hidden', marginTop: 12 }}>
        <table className="vols-table">
          <thead>
            <tr>
              <th>kind</th>
              <th>table</th>
              <th>field</th>
              <th>type</th>
              <th>formula</th>
              <th>depends on</th>
            </tr>
          </thead>
          <tbody>
            {computed.map(([key, f]) => {
              const [t, field] = key.split('.');
              return (
                <tr key={key}>
                  <td><span className={`dag-pill dag-pill-${f.kind}`}>{f.kind}</span></td>
                  <td><code>{t}</code></td>
                  <td>
                    <Link to={`/dag/${t}/${field}`}><code>{field}</code></Link>
                  </td>
                  <td className="small muted">{f.returns}</td>
                  <td className="small"><code>{f.formula}</code></td>
                  <td className="small muted">
                    {(f.depends || []).map((d) => {
                      const [dt, df] = d.split('.');
                      const node = schema.computed[d];
                      return (
                        <span key={d} style={{ marginRight: 6 }}>
                          {node ? <Link to={`/dag/${dt}/${df}`}><code>{d}</code></Link> : <code>{d}</code>}
                        </span>
                      );
                    })}
                  </td>
                </tr>
              );
            })}
          </tbody>
        </table>
      </div>

      <div style={{ marginTop: 20 }}>
        <h3>Constants</h3>
        <pre className="dag-formula" style={{ marginTop: 6 }}>
          {JSON.stringify(schema.constants, null, 2)}
        </pre>
      </div>
    </main>
  );
}
