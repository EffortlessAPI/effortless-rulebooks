import { useNavigate } from 'react-router-dom'

export function PlaceholderRole({ role }: { role: string }) {
  const navigate = useNavigate()

  return (
    <div style={{ padding: '2rem', maxWidth: '600px', margin: '0 auto' }}>
      <h1>🏈 Fantasy Football League Manager</h1>

      <div
        style={{
          background: '#f0f0f0',
          padding: '2rem',
          borderRadius: '8px',
          marginTop: '2rem',
          textAlign: 'center',
        }}
      >
        <h2>Role: {role}</h2>
        <p style={{ marginTop: '1rem', color: '#666' }}>
          This role would show {role}-specific views and functionality in the full implementation.
        </p>
        <p style={{ marginTop: '1rem', color: '#999', fontSize: '0.9rem' }}>
          In this demo, only the Commissioner role is fully wired. Switch to commissioner@league.local to see the
          full app.
        </p>

        <button
          onClick={() => {
            localStorage.removeItem('userEmail')
            navigate('/')
            window.location.reload()
          }}
          style={{
            marginTop: '2rem',
            padding: '0.75rem 1.5rem',
            background: '#007bff',
            color: 'white',
            border: 'none',
            borderRadius: '4px',
            cursor: 'pointer',
            fontSize: '1rem',
          }}
        >
          Switch User
        </button>
      </div>
    </div>
  )
}
