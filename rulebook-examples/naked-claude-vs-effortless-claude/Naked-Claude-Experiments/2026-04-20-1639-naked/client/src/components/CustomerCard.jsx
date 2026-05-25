import { colorStyle } from '../colorUtils.js';

export default function CustomerCard({ customer, onClick }) {
  const { name, notes, color, is_stopped } = customer;
  const cs = colorStyle(color);

  return (
    <div
      style={{
        ...styles.card,
        ...(is_stopped ? styles.stoppedCard : {}),
      }}
      onClick={onClick}
      role="button"
      tabIndex={0}
      onKeyDown={(e) => e.key === 'Enter' && onClick()}
    >
      <div style={styles.left}>
        <div
          style={{
            ...styles.colorDot,
            background: cs.bg,
            border: `2px solid ${cs.border}`,
          }}
          title={color || 'No color'}
        />
        <div style={styles.info}>
          <div style={styles.nameRow}>
            <span style={styles.name}>{name}</span>
            {is_stopped && (
              <span style={styles.stoppedBadge}>STOPPED</span>
            )}
          </div>
          {notes && (
            <span style={styles.notes}>{notes}</span>
          )}
        </div>
      </div>

      <div style={styles.right}>
        <span style={{ ...styles.colorTag, background: cs.bg, color: cs.text }}>
          {color || '—'}
        </span>
        <span style={styles.arrow}>›</span>
      </div>
    </div>
  );
}

const styles = {
  card: {
    background: '#fff',
    borderRadius: 10,
    padding: '14px 18px',
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'space-between',
    cursor: 'pointer',
    boxShadow: '0 1px 4px rgba(0,0,0,0.08)',
    transition: 'box-shadow 0.15s, transform 0.1s',
    border: '2px solid transparent',
    userSelect: 'none',
  },
  stoppedCard: {
    background: '#f1f8f1',
    border: '2px solid #a5d6a7',
    opacity: 0.85,
  },
  left: {
    display: 'flex',
    alignItems: 'center',
    gap: 14,
    flex: 1,
    minWidth: 0,
  },
  colorDot: {
    width: 18,
    height: 18,
    borderRadius: '50%',
    flexShrink: 0,
  },
  info: {
    display: 'flex',
    flexDirection: 'column',
    gap: 2,
    minWidth: 0,
  },
  nameRow: {
    display: 'flex',
    alignItems: 'center',
    gap: 8,
  },
  name: {
    fontSize: 15,
    fontWeight: 600,
    color: '#1a1a2e',
  },
  stoppedBadge: {
    background: '#4CAF50',
    color: '#fff',
    fontSize: 10,
    fontWeight: 700,
    letterSpacing: 1,
    padding: '2px 7px',
    borderRadius: 10,
    textTransform: 'uppercase',
  },
  notes: {
    fontSize: 12,
    color: '#888',
    whiteSpace: 'nowrap',
    overflow: 'hidden',
    textOverflow: 'ellipsis',
    maxWidth: 400,
  },
  right: {
    display: 'flex',
    alignItems: 'center',
    gap: 10,
    flexShrink: 0,
  },
  colorTag: {
    fontSize: 12,
    fontWeight: 600,
    padding: '3px 10px',
    borderRadius: 12,
  },
  arrow: {
    fontSize: 20,
    color: '#bbb',
    lineHeight: 1,
  },
};
