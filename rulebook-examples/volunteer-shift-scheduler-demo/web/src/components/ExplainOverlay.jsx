import { useExplain } from './ExplainContext.jsx';

function formatValue(v) {
  if (v === null || v === undefined) return '—';
  if (typeof v === 'number') return v.toString();
  if (typeof v === 'string') return v;
  return JSON.stringify(v);
}

export default function ExplainOverlay() {
  const { stack, close, pop, push } = useExplain();
  const frame = stack[stack.length - 1];
  if (!frame) return null;

  const { title, explain } = frame;
  const inputs = explain.inputs || [];

  return (
    <div className="explain-backdrop" onClick={close}>
      <div className="explain-card" onClick={(e) => e.stopPropagation()}>
        <header>
          <div>
            <div className="bold">{title}</div>
            {stack.length > 1 && (
              <div className="nav-stack">
                <button className="ghost" onClick={pop}>← back</button>
                <span>depth {stack.length}</span>
              </div>
            )}
          </div>
          <button className="ghost" onClick={close}>✕</button>
        </header>
        <div className="body">
          <div className="result">= {formatValue(explain.result)}</div>
          <div className="formula">{explain.formula}</div>
          <div className="small muted bold" style={{ marginBottom: 4 }}>
            inputs ({inputs.length})
          </div>
          <table className="inputs">
            <tbody>
              {inputs.length === 0 && (
                <tr><td colSpan={2} className="muted">(none)</td></tr>
              )}
              {inputs.map((inp, i) => (
                <tr key={i}>
                  <td>{inp.label}</td>
                  <td>
                    {inp.ref ? (
                      <button
                        className="ghost"
                        style={{ padding: '2px 6px', color: 'var(--link)' }}
                        onClick={() => push(inp.ref)}
                        title={`inspect ${inp.ref}`}
                      >
                        {formatValue(inp.value)} ↗
                      </button>
                    ) : (
                      formatValue(inp.value)
                    )}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
}
