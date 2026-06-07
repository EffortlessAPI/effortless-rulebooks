import { BrowserRouter, Routes, Route, Link, useNavigate, useParams } from 'react-router-dom'
import type { ReactNode } from 'react'
import { FieldDag, TablePage, TablesIndex, RoutingContext } from './explainer-dag'
import type { ExplainerDagRouting } from './explainer-dag'
import './explainer-dag/dag.css'
import Customers from './Customers'

// One shared routing object for the whole explainer, so the field/table/index
// links and programmatic navigation all go through react-router (path routes) —
// never the package's hash fallbacks, which would collide with BrowserRouter and
// produce a doubled URL like /dag/Customers/#/dag/Customers/.
//
// navigateTable("") → the tables index (/dag); any other table → /dag/<Table>.
function useDagRouting(): ExplainerDagRouting {
  const navigate = useNavigate()
  return {
    FieldLink: ({ table, field, className, children }: { table: string; field: string; className?: string; children: ReactNode }) => (
      <Link to={`/dag/${table}/${field}`} className={className}>{children}</Link>
    ),
    TableLink: ({ table, className, children }: { table: string; className?: string; children: ReactNode }) => (
      <Link to={table ? `/dag/${table}` : '/dag'} className={className}>{children}</Link>
    ),
    onBack: () => navigate(-1),
    onHome: () => navigate('/'),
    navigate: (t: string, f: string) => navigate(`/dag/${t}/${f}`),
    navigateTable: (t: string) => navigate(t ? `/dag/${t}` : '/dag'),
  }
}

function FieldDagRoute() {
  const { table = '', field = '' } = useParams()
  const routing = useDagRouting()
  return <FieldDag table={table} field={field} routing={routing} />
}

function TablePageRoute() {
  const { table = '' } = useParams()
  const routing = useDagRouting()
  return <TablePage table={table} routing={routing} />
}

function TablesIndexRoute() {
  const routing = useDagRouting()
  return <TablesIndex routing={routing} />
}

function AppContent() {
  const dagRouting = useDagRouting()

  return (
    <RoutingContext.Provider value={dagRouting}>
      <Routes>
        <Route path="/" element={<Customers />} />
        <Route path="/dag" element={<TablesIndexRoute />} />
        <Route path="/dag/:table" element={<TablePageRoute />} />
        <Route path="/dag/:table/:field" element={<FieldDagRoute />} />
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
