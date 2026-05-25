import React from 'react';

const COLOR_SWATCHES = {
  Green:  '#22c55e',
  Red:    '#ef4444',
  Yellow: '#eab308',
  Blue:   '#3b82f6',
};

function ColorBubble({ color, count }) {
  const bg = COLOR_SWATCHES[color] || '#9ca3af';
  return (
    <div style={{
      display: 'flex',
      alignItems: 'center',
      gap: 8,
      background: '#fff',
      border: '1px solid #e5e7eb',
      borderRadius: 8,
      padding: '8px 14px',
    }}>
      <span style={{
        width: 14, height: 14, borderRadius: '50%',
        background: bg, display: 'inline-block', flexShrink: 0
      }} />
      <span style={{ fontSize: 14, fontWeight: 600, color: '#374151' }}>{color}</span>
      <span style={{
        marginLeft: 4, background: '#f3f4f6', borderRadius: 12,
        padding: '2px 8px', fontSize: 13, fontWeight: 700, color: '#6b7280'
      }}>{count}</span>
    </div>
  );
}

export default function Summary({ summary }) {
  if (!summary) return null;
  const { total, stopped, green, red, yellow, blue } = summary;
  return (
    <div style={{
      background: '#fff',
      border: '1px solid #e5e7eb',
      borderRadius: 12,
      padding: '20px 24px',
      marginBottom: 28,
      boxShadow: '0 1px 3px rgba(0,0,0,0.06)',
    }}>
      <div style={{ display: 'flex', gap: 24, flexWrap: 'wrap', alignItems: 'center', marginBottom: 16 }}>
        <div style={{ textAlign: 'center' }}>
          <div style={{ fontSize: 32, fontWeight: 800, color: '#1a1a2e' }}>{total}</div>
          <div style={{ fontSize: 12, color: '#6b7280', fontWeight: 500, textTransform: 'uppercase', letterSpacing: '0.05em' }}>Total Customers</div>
        </div>
        <div style={{ width: 1, background: '#e5e7eb', alignSelf: 'stretch' }} />
        <div style={{ textAlign: 'center' }}>
          <div style={{ fontSize: 32, fontWeight: 800, color: '#22c55e' }}>{stopped}</div>
          <div style={{ fontSize: 12, color: '#6b7280', fontWeight: 500, textTransform: 'uppercase', letterSpacing: '0.05em' }}>Stopped</div>
        </div>
        <div style={{ width: 1, background: '#e5e7eb', alignSelf: 'stretch' }} />
        <div style={{ flex: 1, display: 'flex', gap: 10, flexWrap: 'wrap', alignItems: 'center' }}>
          <span style={{ fontSize: 12, color: '#9ca3af', fontWeight: 500, textTransform: 'uppercase', letterSpacing: '0.05em', marginRight: 4 }}>Colors:</span>
          <ColorBubble color="Green"  count={green} />
          <ColorBubble color="Red"    count={red} />
          <ColorBubble color="Yellow" count={yellow} />
          <ColorBubble color="Blue"   count={blue} />
        </div>
      </div>
    </div>
  );
}
