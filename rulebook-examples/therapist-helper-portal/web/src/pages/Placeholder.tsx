import type { Me } from "../App";

export default function Placeholder({ me, title }: { me: Me; title: string }) {
  return (
    <div className="placeholder">
      <h1>{title}</h1>
      <p className="muted">Signed in as <strong>{me.full_name}</strong> ({me.role}).</p>
      <div className="callout">
        <p>
          The <strong>{me.role}</strong> view is a placeholder in this demo. In a full
          build it would show:
        </p>
        <ul>
          {me.role === "supervisor" && <>
            <li>All therapists' caseloads and their at-risk clients.</li>
            <li>Supervision notes and case-conference scheduling.</li>
            <li>Rollup metrics across the whole practice.</li>
          </>}
          {me.role === "client" && <>
            <li>Your own upcoming sessions and goals.</li>
            <li>A simple journaling and mood-check-in form.</li>
            <li>Shared notes from your therapist.</li>
          </>}
        </ul>
        <p>
          To see the fully-wired experience, click <em>Switch user</em> in the
          sidebar and pick one of the <strong>therapist</strong> identities.
        </p>
      </div>
    </div>
  );
}
