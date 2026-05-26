import { useContext } from 'react';
import { DagToggleContext, DagToggleSetterContext } from './context';

export function DagToggle() {
  const dagEnabled = useContext(DagToggleContext);
  const setDagEnabled = useContext(DagToggleSetterContext);

  if (!setDagEnabled) return null;

  return (
    <button
      onClick={() => setDagEnabled(!dagEnabled)}
      style={{
        padding: '0.4rem 0.8rem',
        fontSize: '0.85rem',
        background: dagEnabled ? '#667eea' : '#ddd',
        color: dagEnabled ? 'white' : '#333',
        border: 'none',
        borderRadius: '4px',
        cursor: 'pointer',
        transition: 'background 0.2s',
      }}
      title="Toggle DAG explainer mode"
    >
      {dagEnabled ? '📊 DAG On' : '📊 DAG Off'}
    </button>
  );
}
