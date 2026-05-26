import { Link } from 'react-router-dom';

interface PlaceholderProps {
  title: string;
  description: string;
}

export default function Placeholder({ title, description }: PlaceholderProps) {
  return (
    <div className="container" style={{ textAlign: 'center', paddingTop: '4rem' }}>
      <h1>{title}</h1>
      <p style={{ fontSize: '1.1rem', color: '#666', marginBottom: '2rem' }}>{description}</p>
      <div style={{ background: '#f0f0f0', padding: '2rem', borderRadius: '8px', maxWidth: '500px', margin: '0 auto' }}>
        <p>This feature is reserved for Warehouse Manager role.</p>
        <p style={{ color: '#999', fontSize: '0.9rem', marginTop: '1rem' }}>
          Your current role does not have access to this view. Please sign in as a Warehouse Manager to see full inventory management features.
        </p>
      </div>
      <Link to="/" style={{ display: 'inline-block', marginTop: '2rem', padding: '0.75rem 1.5rem', background: '#0066cc', color: 'white', textDecoration: 'none', borderRadius: '4px' }}>
        Back to Dashboard
      </Link>
    </div>
  );
}
