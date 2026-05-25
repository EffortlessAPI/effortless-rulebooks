export function Placeholder({
  role,
  title,
  body,
}: {
  role: string;
  title: string;
  body: string;
}) {
  return (
    <div className="page placeholder">
      <h1>{title}</h1>
      <p className="muted">
        You're signed in as <code>{role}</code>.
      </p>
      <p>{body}</p>
    </div>
  );
}
