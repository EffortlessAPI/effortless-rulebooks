import React, { useEffect, useState } from 'react'
import { Routes, Route, Link, useNavigate, useParams, useLocation } from 'react-router-dom'
import Login from './pages/Login'
import Dashboard from './pages/Dashboard'
import EventDetail from './pages/EventDetail'
import VenueDetail from './pages/VenueDetail'
import SpeakerSchedule from './pages/SpeakerSchedule'
import { FieldDag, RoutingContext, DagToggle } from './explainer-dag'
import './explainer-dag/dag.css'
import './App.css'

interface User {
  email: string
  role: string
}

function DagRoute() {
  const { table = '', field = '' } = useParams()
  const navigate = useNavigate()
  return (
    <FieldDag
      table={table}
      field={field}
      routing={{
        navigate: (t: string, f: string) => navigate(`/dag/${t}/${f}`),
        onBack: () => navigate(-1)
      }}
    />
  )
}

function App() {
  const [user, setUser] = useState<User | null>(null)
  const [loading, setLoading] = useState(true)
  const navigate = useNavigate()
  const location = useLocation()

  useEffect(() => {
    const stored = localStorage.getItem('user')
    if (stored) {
      setUser(JSON.parse(stored))
    }
    setLoading(false)
  }, [])

  const handleLogin = (email: string) => {
    const user = { email, role: 'Admin' }
    setUser(user)
    localStorage.setItem('user', JSON.stringify(user))
  }

  const handleLogout = () => {
    setUser(null)
    localStorage.removeItem('user')
    navigate('/login')
  }

  const dagRouting = {
    FieldLink: ({ table, field, className, children }: { table: string; field: string; className?: string; children: React.ReactNode }) => (
      <Link to={`/dag/${table}/${field}`} className={className}>{children}</Link>
    ),
    navigate: (table: string, field: string) => navigate(`/dag/${table}/${field}`),
    onBack: () => navigate(-1)
  }

  if (loading) {
    return <div className="loading">Loading...</div>
  }

  if (!user) {
    return <Login onLogin={handleLogin} />
  }

  const isDagRoute = location.pathname.startsWith('/dag')

  return (
    <RoutingContext.Provider value={dagRouting}>
      <div className="app">
            <header className="app-header">
              <div className="header-left">
                <h1 onClick={() => navigate('/')} style={{ cursor: 'pointer' }}>🎉 Community Event Planner</h1>
                {!isDagRoute && (
                  <nav className="app-nav">
                    <button
                      className={`nav-button ${location.pathname === '/' ? 'active' : ''}`}
                      onClick={() => navigate('/')}
                    >
                      Dashboard
                    </button>
                    <button
                      className={`nav-button ${location.pathname === '/speakers' ? 'active' : ''}`}
                      onClick={() => navigate('/speakers')}
                    >
                      Speakers
                    </button>
                  </nav>
                )}
              </div>
              <div className="header-right">
                <span className="user-info">{user.email}</span>
                {!isDagRoute && <DagToggle />}
                <button onClick={handleLogout} className="logout-btn">Logout</button>
              </div>
            </header>
            <main className="app-main">
              <Routes>
                <Route path="/dag/:table/:field" element={<DagRoute />} />
                <Route path="/" element={<Dashboard />} />
                <Route path="/events/:id" element={<EventDetail />} />
                <Route path="/venues/:id" element={<VenueDetail />} />
                <Route path="/speakers" element={<SpeakerSchedule />} />
              </Routes>
            </main>
      </div>
    </RoutingContext.Provider>
  )
}

export default App
