const COLOR_MAP = {
  Green:  '#27ae60',
  Red:    '#e74c3c',
  Yellow: '#f39c12',
  Blue:   '#2980b9',
};

function colorStyle(color) {
  return COLOR_MAP[color] || '#888';
}

export function ColorBadge({ color }) {
  return (
    <span className="color-badge" style={{ background: colorStyle(color) }}>
      {color || '—'}
    </span>
  );
}

export function ColorDot({ color }) {
  return (
    <span className="color-dot" style={{ background: colorStyle(color) }} />
  );
}

export { colorStyle };
