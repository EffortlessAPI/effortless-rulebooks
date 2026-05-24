import { Link } from 'react-router-dom';
import type { DevUser } from '../types';

export default function Placeholder({ me }: { me: DevUser }) {
  const blurb = roleBlurb(me.role);
  return (
    <div className="page placeholder">
      <h1>{me.name}</h1>
      <p className="role-tag">role: {me.role}</p>
      <p className="lede">{blurb}</p>
      <p>
        This role is a placeholder in the demo — its UI would show the views described above.
        For the full Effortless cascade, sign out and pick the <strong>researcher</strong>{' '}
        identity instead.
      </p>
      <p>
        <Link to="/">Back to home</Link>
      </p>
    </div>
  );
}

function roleBlurb(role: string): string {
  switch (role) {
    case 'reviewer':
      return 'Reviewers approve or flag taxonomy classifications. They see read-only assessments grouped by reviewer queue, with comment threads on each Intelligence.';
    case 'public':
      return 'The public-facing taxonomy: a curated, ungated view of intelligences and their tier scores, with embedded explainer cards.';
    default:
      return 'A role-specific landing page would appear here.';
  }
}
