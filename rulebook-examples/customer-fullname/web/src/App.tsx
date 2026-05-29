import { BrowserRouter, Routes, Route, Link, useNavigate, useParams } from 'react-router-dom'
import { FieldDag, RoutingContext } from './explainer-dag'
import './explainer-dag/dag.css'
import Customers from './Customers'

function DagRoute() {
  const navigate = useNavigate()
  const { table = '', field = '' } = useParams()
  const routing = {
    FieldLink: ({ table, field, className, children }: { table: string; field: string; className?: string; children: React.ReactNode }) => (
      <Link to={`/dag/${table}/${field}`} className={className}>{children}</Link>
    ),
    onBack: () => navigate(-1),
    navigate: (t: string, f: string) => navigate(`/dag/${t}/${f}`),
  }
  return <FieldDag table={table} field={field} routing={routing} />
}

function AppContent() {
  const navigate = useNavigate()
  const dagRouting = {
    FieldLink: ({ table, field, className, children }: { table: string; field: string; className?: string; children: React.ReactNode }) => (
      <Link to={`/dag/${table}/${field}`} className={className}>{children}</Link>
    ),
    onBack: () => navigate(-1),
    navigate: (t: string, f: string) => navigate(`/dag/${t}/${f}`),
  }

  return (
    <RoutingContext.Provider value={dagRouting}>
      <Routes>
        <Route path="/" element={<Customers />} />
        <Route path="/dag/:table/:field" element={<DagRoute />} />
      </Routes>
    </RoutingContext.Provider>
  )
}

export default function App() {
  return (
    <BrowserRouter>
      <AppContent />
    </BrowserRouter>
  )
}
