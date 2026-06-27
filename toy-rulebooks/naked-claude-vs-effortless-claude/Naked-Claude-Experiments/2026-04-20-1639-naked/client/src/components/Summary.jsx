const COLOR_DEFS = [
  { key: 'green',  label: 'Green',  bg: '#4CAF50', text: '#fff' },
  { key: 'red',    label: 'Red',    bg: '#f44336', text: '#fff' },
  { key: 'yellow', label: 'Yellow', bg: '#FFC107', text: '#333' },
  { key: 'blue',   label: 'Blue',   bg: '#2196F3', text: '#fff' },
];

export default function Summary({ summary }) {
  if (!summary) return null;

  const total   = parseInt(summary.total,   10) || 0;
  const stopped = parseInt(summary.stopped, 10) || 0;
  const other   = parseInt(summary.other,   10) || 0;

  return (
    <div style={styles.card}>
      <h2 style={styles.heading}>At a Glance</h2>

      <div style={styles.counters}>
        <div style={styles.counter}>
          <span style={styles.counterNum}>{total}</span>
          <span style={styles.counterLabel}>Total Customers</span>
        </div>
        <div style={{ ...styles.counter, ...styles.stoppedCounter }}>
          <span style={styles.counterNum}>{stopped}</span>
          <span style={styles.counterLabel}>Stopped (Green)</span>
        </div>
      </div>

      <div style={styles.colorRow}>
        {COLOR_DEFS.map(({ key, label, bg, text }) => {
          const cnt = parseInt(summary[key], 10) || 0;
          return (
            <div key={key} style={{ ...styles.colorChip, background: bg, color: text }}>
              <span style={styles.chipNum}>{cnt}</span>
              <span style={styles.chipLabel}>{label}</span>
            </div>
          );
        })}
        {other > 0 && (
          <div style={{ ...styles.colorChip, background: '#9E9E9E', color: '#fff' }}>
            <span style={styles.chipNum}>{other}</span>
            <span style={styles.chipLabel}>Other</span>
          </div>
        )}
      </div>
    </div>
  );
}

const styles = {
  card: {
    background: '#fff',
    borderRadius: 10,
    padding: '20px 24px',
    marginBottom: 24,
    boxShadow: '0 1px 4px rgba(0,0,0,0.1)',
  },
  heading: {
    fontSize: 16,
    fontWeight: 700,
    color: '#555',
    textTransform: 'uppercase',
    letterSpacing: 1,
    marginBottom: 16,
  },
  counters: {
    display: 'flex',
    gap: 32,
    marginBottom: 16,
    flexWrap: 'wrap',
  },
  counter: {
    display: 'flex',
    flexDirection: 'column',
    alignItems: 'center',
    minWidth: 80,
  },
  stoppedCounter: {
    background: '#e8f5e9',
    borderRadius: 8,
    padding: '8px 16px',
  },
  counterNum: {
    fontSize: 36,
    fontWeight: 800,
    lineHeight: 1,
    color: '#1a1a2e',
  },
  counterLabel: {
    fontSize: 12,
    color: '#777',
    marginTop: 4,
    textAlign: 'center',
  },
  colorRow: {
    display: 'flex',
    gap: 10,
    flexWrap: 'wrap',
  },
  colorChip: {
    borderRadius: 20,
    padding: '6px 14px',
    display: 'flex',
    alignItems: 'center',
    gap: 6,
    fontWeight: 600,
    fontSize: 14,
  },
  chipNum: {
    fontSize: 18,
    fontWeight: 800,
  },
  chipLabel: {
    fontSize: 13,
  },
};
