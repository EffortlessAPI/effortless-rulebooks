import { useEffect, useState } from 'react'

interface DevUser {
  email: string
  role: string
  description: string
}

interface Props {
  onLogin: (email: string) => void
}

export default function Login({ onLogin }: Props) {
  const [users, setUsers] = useState<DevUser[]>([])
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    fetch('/api/dev-users')
      .then(r => r.json())
      .then(setUsers)
      .finally(() => setLoading(false))
      .catch(console.error)
  }, [])

  if (loading) {
    return <div style={{ padding: '2rem', textAlign: 'center' }}>Loading...</div>
  }

  return (
    <div style={styles.container}>
      <div style={styles.card}>
        <h1 style={styles.title}>🎉 Community Event Planner</h1>
        <p style={styles.subtitle}>Demo: select a role to continue</p>

        <div style={styles.userList}>
          {users.map(user => (
            <button
              key={user.email}
              onClick={() => onLogin(user.email)}
              style={styles.userButton}
            >
              <div style={styles.userRole}>{user.role}</div>
              <div style={styles.userEmail}>{user.email}</div>
              <div style={styles.userDesc}>{user.description}</div>
            </button>
          ))}
        </div>
      </div>
    </div>
  )
}

const styles = {
  container: {
    display: 'flex',
    justifyContent: 'center' as const,
    alignItems: 'center',
    minHeight: '100vh',
    background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
    padding: '2rem',
  },
  card: {
    background: 'white',
    borderRadius: '8px',
    padding: '3rem',
    maxWidth: '500px',
    width: '100%',
    boxShadow: '0 4px 12px rgba(0, 0, 0, 0.15)',
  },
  title: {
    textAlign: 'center' as const,
    marginBottom: '0.5rem',
    color: '#333',
    fontSize: '2rem',
  },
  subtitle: {
    textAlign: 'center' as const,
    color: '#666',
    marginBottom: '2rem',
    fontSize: '1rem',
  },
  userList: {
    display: 'flex',
    flexDirection: 'column' as const,
    gap: '1rem',
  },
  userButton: {
    padding: '1rem',
    border: '2px solid #e0e0e0',
    background: '#f9f9f9',
    borderRadius: '8px',
    cursor: 'pointer',
    transition: 'all 0.2s',
    textAlign: 'left' as const,
    fontSize: '1rem',
  },
  userRole: {
    fontWeight: 'bold',
    color: '#667eea',
    marginBottom: '0.25rem',
  },
  userEmail: {
    color: '#333',
    marginBottom: '0.25rem',
    fontFamily: 'monospace',
    fontSize: '0.9rem',
  },
  userDesc: {
    color: '#999',
    fontSize: '0.85rem',
  },
}
