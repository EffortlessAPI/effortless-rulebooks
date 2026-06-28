import { useEffect, useState } from 'react'
import { Routes, Route, Navigate, useNavigate, useParams } from 'react-router-dom'
import { fetchMe } from './lib/api'
import { FieldDag, RoutingContext, DagToggle } from './explainer-dag'
import './explainer-dag/dag.css'
import { Login } from './pages/Login'
import { Standings } from './pages/Standings'
import { Players } from './pages/Players'
import { Rosters } from './pages/Rosters'
import { Matchups } from './pages/Matchups'
import { PlaceholderRole } from './pages/PlaceholderRole'

interface User {
  email: string
  role: string
  name: string
}

function useDagRouting() {
  const navigate = useNavigate()
  return {
    FieldLink: ({ table, field, className, children }: {
      table: string; field: string; className?: string; children: React.ReactNode;
    }) => (
      <a href={`/dag/${table}/${field}`} className={className} onClick={(e) => {
        e.preventDefault()
        navigate(`/dag/${table}/${field}`)
      }}>{children}</a>
    ),
    onBack: () => navigate(-1),
    navigate: (t: string, f: string) => navigate(`/dag/${t}/${f}`),
  }
}

function DagRoute({ routing }: { routing: ReturnType<typeof useDagRouting> }) {
  const { table = '', field = '' } = useParams()
  return <FieldDag table={table} field={field} routing={routing} />
}

function Shell({ user, onLogout, routing }: { user: User; onLogout: () => void; routing: ReturnType<typeof useDagRouting> }) {
  return (
    <div style={{ display: 'flex', minHeight: '100vh' }}>
      <nav
        style={{
          width: '250px',
          background: '#333',
          color: 'white',
          padding: '1.5rem',
          overflowY: 'auto',
        }}
      >
        <h2 style={{ marginBottom: '1.5rem' }}>🏈 FF League</h2>
        <div style={{ fontSize: '0.85rem', marginBottom: '2rem', color: '#ccc' }}>
          <div>{user.name}</div>
          <div style={{ textTransform: 'capitalize' }}>{user.role}</div>
        </div>

        {user.role === 'commissioner' && (
          <ul style={{ listStyle: 'none' }}>
            <li style={{ marginBottom: '0.5rem' }}>
              <a href="/standings" style={{ color: '#ccc', textDecoration: 'none' }}>
                📊 Standings
              </a>
            </li>
            <li style={{ marginBottom: '0.5rem' }}>
              <a href="/matchups" style={{ color: '#ccc', textDecoration: 'none' }}>
                ⚔️ Matchups
              </a>
            </li>
            <li style={{ marginBottom: '0.5rem' }}>
              <a href="/rosters" style={{ color: '#ccc', textDecoration: 'none' }}>
                🏆 Rosters
              </a>
            </li>
            <li style={{ marginBottom: '0.5rem' }}>
              <a href="/players" style={{ color: '#ccc', textDecoration: 'none' }}>
                👥 Players
              </a>
            </li>
          </ul>
        )}

        <button
          onClick={onLogout}
          style={{
            marginTop: '2rem',
            width: '100%',
            padding: '0.75rem',
            background: '#666',
            color: 'white',
            border: 'none',
            borderRadius: '4px',
            cursor: 'pointer',
          }}
        >
          Logout
        </button>
      </nav>

      <main style={{ flex: 1, overflow: 'auto' }}>
        <div style={{ display: 'flex', justifyContent: 'flex-end', padding: '1rem' }}>
          <DagToggle />
        </div>
        <Routes>
          <Route path="/dag/:table/:field" element={<DagRoute routing={routing} />} />
          <Route path="/" element={<Standings />} />
          <Route path="/standings" element={<Standings />} />
          <Route path="/players" element={user.role === 'commissioner' ? <Players /> : <PlaceholderRole role={user.role} />} />
          <Route path="/rosters" element={user.role === 'commissioner' ? <Rosters /> : <PlaceholderRole role={user.role} />} />
          <Route path="/matchups" element={user.role === 'commissioner' ? <Matchups /> : <PlaceholderRole role={user.role} />} />
        </Routes>
      </main>
    </div>
  )
}

export default function App() {
  const [user, setUser] = useState<User | null>(null)
  const [loading, setLoading] = useState(true)
  const navigate = useNavigate()
  const routing = useDagRouting()

  useEffect(() => {
    fetchMe()
      .then((u) => {
        if (u) setUser(u)
      })
      .catch(console.error)
      .finally(() => setLoading(false))
  }, [])

  if (loading) return <div style={{ padding: '2rem' }}>Loading...</div>

  if (!user) return <Login />

  return (
    <RoutingContext.Provider value={routing}>
      <Shell
        user={user}
        routing={routing}
        onLogout={() => {
          localStorage.removeItem('userEmail')
          setUser(null)
          navigate('/')
        }}
      />
    </RoutingContext.Provider>
  )
}
