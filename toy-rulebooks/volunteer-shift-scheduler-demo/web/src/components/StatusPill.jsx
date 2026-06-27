export default function StatusPill({ status }) {
  if (!status) return null;
  return <span className={`status-pill status-${status}`}>{status}</span>;
}
