export default function StatusBadge({ name, isBlocking }) {
  return (
    <span
      className="status-badge"
      style={{
        background: isBlocking ? '#c0392b' : '#2980b9',
      }}
      title={isBlocking ? 'Blocking status' : 'Non-blocking status'}
    >
      {name}
    </span>
  );
}
