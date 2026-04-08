import { useEffect, useState } from 'react';

export default function App() {
  const [tables, setTables] = useState([]);
  const [selected, setSelected] = useState(null);
  const [data, setData] = useState(null);
  const [error, setError] = useState(null);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    fetch('/api/tables')
      .then(r => r.json())
      .then(setTables)
      .catch(e => setError(e.message));
  }, []);

  useEffect(() => {
    if (!selected) return;
    setLoading(true);
    setError(null);
    fetch(`/api/tables/${selected}`)
      .then(r => r.json())
      .then(d => {
        if (d.error) setError(d.error);
        else setData(d);
      })
      .catch(e => setError(e.message))
      .finally(() => setLoading(false));
  }, [selected]);

  return (
    <div className="app">
      <aside className="sidebar">
        <h2>Tables</h2>
        <ul>
          {tables.map(t => (
            <li
              key={t}
              className={t === selected ? 'active' : ''}
              onClick={() => setSelected(t)}
            >
              {t}
            </li>
          ))}
        </ul>
      </aside>
      <main className="main">
        <h1>{selected || 'Effortless Rulebook Browser'}</h1>
        {error && <div className="error">Error: {error}</div>}
        {loading && <div>Loading…</div>}
        {!loading && data && (
          <div className="grid-wrap">
            <table className="grid">
              <thead>
                <tr>
                  {data.columns.map(c => <th key={c}>{c}</th>)}
                </tr>
              </thead>
              <tbody>
                {data.rows.map((row, i) => (
                  <tr key={i}>
                    {data.columns.map(c => (
                      <td key={c}>{formatCell(row[c])}</td>
                    ))}
                  </tr>
                ))}
              </tbody>
            </table>
            <div className="meta">{data.rows.length} row(s)</div>
          </div>
        )}
        {!selected && !error && (
          <p>Select a table or view from the left to browse its rows.</p>
        )}
      </main>
    </div>
  );
}

function formatCell(v) {
  if (v === null || v === undefined) return '';
  if (typeof v === 'object') return JSON.stringify(v);
  return String(v);
}
