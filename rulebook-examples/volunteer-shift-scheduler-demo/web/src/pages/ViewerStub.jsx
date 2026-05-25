export default function ViewerStub() {
  return (
    <div className="stub">
      <h2 style={{ marginTop: 0 }}>Viewer view — stub</h2>
      <p>
        Board members and co-organizers land here. They get a read-only operational picture of
        the schedule.
      </p>
      <p>The dashboard above is what they see — same numbers, no edit controls.</p>
      <ul>
        <li>Read-only event grid (currently visible above)</li>
        <li>Click into an event to see staffing detail (same explain-DAG, no edit buttons)</li>
        <li>Filter by upcoming-only vs all</li>
        <li>Subscribe to digest emails (out of scope for demo)</li>
      </ul>
    </div>
  );
}
