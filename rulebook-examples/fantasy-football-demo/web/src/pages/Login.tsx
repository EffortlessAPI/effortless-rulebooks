import { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { fetchDevUsers } from '../lib/api'

interface DevUser {
  email: string
  role: string
  name: string
}

export function Login() {
  const [users, setUsers] = useState<DevUser[]>([])
  const [loading, setLoading] = useState(true)
  const navigate = useNavigate()

  useEffect(() => {
    fetchDevUsers()
      .then(setUsers)
      .catch(console.error)
      .finally(() => setLoading(false))
  }, [])

  const handleLogin = (email: string) => {
    localStorage.setItem('userEmail', email)
    navigate('/')
    window.location.reload()
  }

  if (loading) return <div style={{ padding: '2rem' }}>Loading...</div>

  const grouped = users.reduce(
    (acc, user) => {
      if (!acc[user.role]) acc[user.role] = []
      acc[user.role].push(user)
      return acc
    },
    {} as Record<string, DevUser[]>
  )

  return (
    <div style={{ padding: '2rem', maxWidth: '600px', margin: '0 auto' }}>
      <h1>🏈 Fantasy Football League Manager</h1>
      <p style={{ marginBottom: '2rem', color: '#666' }}>Select a user to log in</p>

      {Object.entries(grouped).map(([role, roleUsers]) => (
        <div key={role} style={{ marginBottom: '2rem' }}>
          <h3 style={{ textTransform: 'capitalize', marginBottom: '0.5rem' }}>{role}</h3>
          <div style={{ display: 'flex', flexDirection: 'column', gap: '0.5rem' }}>
            {roleUsers.map((user) => (
              <button
                key={user.email}
                onClick={() => handleLogin(user.email)}
                style={{
                  padding: '0.75rem 1rem',
                  background: '#007bff',
                  color: 'white',
                  border: 'none',
                  borderRadius: '4px',
                  cursor: 'pointer',
                  fontSize: '1rem',
                  textAlign: 'left',
                }}
              >
                {user.email}
              </button>
            ))}
          </div>
        </div>
      ))}
    </div>
  )
}
