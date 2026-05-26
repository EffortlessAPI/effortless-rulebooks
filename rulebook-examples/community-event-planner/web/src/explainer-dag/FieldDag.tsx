import { useContext } from 'react';
import { RoutingContext } from './context';
import { embeddedRulebook } from './embedded-rulebook';
import './dag.css';

interface Props {
  table: string;
  field: string;
  routing: { onBack: () => void; navigate: (t: string, f: string) => void };
}

export function FieldDag({ table, field, routing }: Props) {
  const rulebook = embeddedRulebook;
  const tableObj = (rulebook.tables as any)[table];

  if (!tableObj || !tableObj.fields[field]) {
    return (
      <div className="dag-container">
        <button onClick={routing.onBack}>← Back</button>
        <p>Field not found: {table}.{field}</p>
      </div>
    );
  }

  const fieldDef = tableObj.fields[field];

  const renderDependencies = () => {
    if (fieldDef.type === 'raw') {
      return <p style={{ color: '#666' }}>This is a raw input field (no dependencies).</p>;
    }

    if (!fieldDef.formula) {
      return <p style={{ color: '#666' }}>No formula defined.</p>;
    }

    // Extract field references from formula
    const matches = fieldDef.formula.match(/\{\{([^}]+)\}\}/g) || [];
    const refFields = matches.map((m: string) => m.replace(/[{}]/g, ''));

    return (
      <div>
        <h3>Dependencies</h3>
        <ul style={{ marginLeft: '1rem' }}>
          {refFields.map((ref: string) => (
            <li key={ref}>
              <button
                onClick={() => routing.navigate(table, ref)}
                style={{
                  background: 'none',
                  border: '1px solid #667eea',
                  color: '#667eea',
                  padding: '0.25rem 0.5rem',
                  borderRadius: '3px',
                  cursor: 'pointer',
                }}
              >
                {table}.{ref} →
              </button>
            </li>
          ))}
        </ul>
      </div>
    );
  };

  return (
    <div className="dag-container">
      <button onClick={routing.onBack} style={{ marginBottom: '1rem' }}>← Back</button>

      <div className="dag-card">
        <h2>
          {table}.<strong>{field}</strong>
        </h2>

        <div style={{ marginBottom: '1rem' }}>
          <span className="field-type">{fieldDef.type}</span>
          {fieldDef.datatype && <span className="field-datatype">{fieldDef.datatype}</span>}
        </div>

        {fieldDef.type !== 'raw' && (
          <div style={{ marginBottom: '1rem', background: '#f5f5f5', padding: '1rem', borderRadius: '4px', fontFamily: 'monospace', fontSize: '0.9rem', overflowX: 'auto' }}>
            <strong>Formula:</strong>
            <pre style={{ margin: '0.5rem 0 0 0' }}>{fieldDef.formula}</pre>
          </div>
        )}

        {renderDependencies()}
      </div>
    </div>
  );
}
