import { useEffect, useState } from 'react'
import { DagCell, DagToggle } from './explainer-dag'

interface Customer {
  CustomerId: string
  Name: string
  EmailAddress: string
  Initials: string
  FirstName: string
  LastName: string
}

interface EditState {
  customerId: string
  field: 'FirstName' | 'LastName'
}

interface ExportState {
  active: boolean
  pct: number
  label: string
  error: string | null
}

export default function Customers() {
  const [customers, setCustomers] = useState<Customer[]>([])
  const [editState, setEditState] = useState<EditState | null>(null)
  const [editValue, setEditValue] = useState('')
  const [exportState, setExportState] = useState<ExportState>({ active: false, pct: 0, label: '', error: null })

  useEffect(() => {
    load()
  }, [])

  // Export the live data to Excel. Opens an SSE stream so the progress bar
  // tracks REAL backend stages (querying views → temp rulebook → transpiler →
  // packaging); on `done` it fetches the finished workbook and triggers a
  // browser download.
  function handleExport() {
    if (exportState.active) return
    setExportState({ active: true, pct: 0, label: 'Starting…', error: null })

    const es = new EventSource('/api/export/xlsx/stream')

    es.addEventListener('progress', (e) => {
      const { label, pct } = JSON.parse((e as MessageEvent).data)
      setExportState((s) => ({ ...s, pct, label }))
    })

    es.addEventListener('error', (e) => {
      // Either a server-sent `error` event (has data) or a transport drop.
      let message = 'Export failed.'
      const data = (e as MessageEvent).data
      if (data) {
        try { message = JSON.parse(data).message || message } catch { /* keep default */ }
      }
      es.close()
      setExportState({ active: false, pct: 0, label: '', error: message })
    })

    es.addEventListener('done', async (e) => {
      const { downloadUrl, filename } = JSON.parse((e as MessageEvent).data)
      es.close()
      setExportState((s) => ({ ...s, pct: 100, label: 'Downloading…' }))
      try {
        const res = await fetch(downloadUrl)
        if (!res.ok) throw new Error(`Download failed (${res.status})`)
        const blob = await res.blob()
        const url = URL.createObjectURL(blob)
        const a = document.createElement('a')
        a.href = url
        a.download = filename || 'export.xlsx'
        a.click()
        URL.revokeObjectURL(url)
        setExportState({ active: false, pct: 0, label: '', error: null })
      } catch (err) {
        setExportState({ active: false, pct: 0, label: '', error: String((err as Error).message || err) })
      }
    })
  }

  async function load() {
    const res = await fetch('/api/customers')
    const data = await res.json()
    setCustomers(data)
  }

  async function save(customerId: string, field: string, newValue: string) {
    if (!newValue.trim()) {
      setEditState(null)
      return
    }

    const res = await fetch(`/api/customers/${customerId}`, {
      method: 'PATCH',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ [field]: newValue.trim() })
    })

    if (res.ok) {
      await load()
      setEditState(null)
    }
  }

  return (
    <div style={{ minHeight: '100vh', background: '#f5f5f5', padding: '20px' }}>
      <div style={{ maxWidth: '800px', margin: '0 auto', background: 'white', borderRadius: '8px', boxShadow: '0 2px 8px rgba(0,0,0,0.1)', padding: '30px' }}>
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '20px' }}>
          <h1 style={{ color: '#333', margin: 0, fontSize: '28px' }}>Customers</h1>
          <div style={{ display: 'flex', alignItems: 'center', gap: '12px' }}>
            <button
              onClick={handleExport}
              disabled={exportState.active}
              style={{
                font: 'inherit',
                fontSize: '13px',
                padding: '7px 14px',
                borderRadius: '5px',
                border: '1px solid #1f9d55',
                background: exportState.active ? '#9ad0b3' : '#27ae60',
                color: 'white',
                cursor: exportState.active ? 'default' : 'pointer',
                whiteSpace: 'nowrap',
              }}
            >
              {exportState.active ? 'Exporting…' : '⬇ Export to Excel'}
            </button>
            <DagToggle />
          </div>
        </div>

        {exportState.active && (
          <div style={{ marginBottom: '20px' }}>
            <div style={{ height: '10px', background: '#eee', borderRadius: '5px', overflow: 'hidden' }}>
              <div
                style={{
                  height: '100%',
                  width: `${exportState.pct}%`,
                  background: '#27ae60',
                  transition: 'width 0.25s ease',
                }}
              />
            </div>
            <div style={{ fontSize: '12px', color: '#666', marginTop: '6px' }}>
              {exportState.label} ({exportState.pct}%)
            </div>
          </div>
        )}

        {exportState.error && (
          <div style={{ marginBottom: '20px', padding: '10px 12px', background: '#fdecea', border: '1px solid #f5c6cb', borderRadius: '5px', color: '#b94a48', fontSize: '13px' }}>
            Export failed: {exportState.error}
          </div>
        )}

        <p style={{ color: '#666', marginBottom: '25px', fontSize: '14px' }}>Click any field to edit. Press Enter to save, Escape to cancel.</p>

        <table style={{ width: '100%', borderCollapse: 'collapse' }}>
          <thead>
            <tr style={{ background: '#f9f9f9' }}>
              <th style={{ padding: '12px', textAlign: 'left', fontWeight: 600, color: '#333', borderBottom: '2px solid #ddd', fontSize: '13px' }}>First Name</th>
              <th style={{ padding: '12px', textAlign: 'left', fontWeight: 600, color: '#333', borderBottom: '2px solid #ddd', fontSize: '13px' }}>Last Name</th>
              <th style={{ padding: '12px', textAlign: 'left', fontWeight: 600, color: '#333', borderBottom: '2px solid #ddd', fontSize: '13px' }}>Full Name</th>
              <th style={{ padding: '12px', textAlign: 'left', fontWeight: 600, color: '#333', borderBottom: '2px solid #ddd', fontSize: '13px' }}>Initials</th>
              <th style={{ padding: '12px', textAlign: 'left', fontWeight: 600, color: '#333', borderBottom: '2px solid #ddd', fontSize: '13px' }}>Email</th>
            </tr>
          </thead>
          <tbody>
            {customers.map(c => (
              <tr key={c.CustomerId} style={{ borderBottom: '1px solid #eee' }}>
                <td style={{ padding: '12px', fontSize: '14px' }}>
                  {editState?.customerId === c.CustomerId && editState?.field === 'FirstName' ? (
                    <input
                      type="text"
                      value={editValue}
                      onChange={(e) => setEditValue(e.target.value)}
                      onKeyDown={(e) => {
                        if (e.key === 'Enter') save(c.CustomerId, 'FirstName', editValue)
                        if (e.key === 'Escape') setEditState(null)
                      }}
                      onBlur={() => save(c.CustomerId, 'FirstName', editValue)}
                      autoFocus
                      style={{ font: 'inherit', border: '1px solid #0066cc', padding: '4px 6px', borderRadius: '3px', width: '100%' }}
                    />
                  ) : (
                    <span onClick={() => { setEditState({ customerId: c.CustomerId, field: 'FirstName' }); setEditValue(c.FirstName) }} style={{ cursor: 'pointer', padding: '2px 4px', borderRadius: '3px' }} onMouseEnter={(e) => (e.currentTarget.style.background = '#f0f0f0')} onMouseLeave={(e) => (e.currentTarget.style.background = 'transparent')}>
                      {c.FirstName}
                    </span>
                  )}
                </td>
                <td style={{ padding: '12px', fontSize: '14px' }}>
                  {editState?.customerId === c.CustomerId && editState?.field === 'LastName' ? (
                    <input
                      type="text"
                      value={editValue}
                      onChange={(e) => setEditValue(e.target.value)}
                      onKeyDown={(e) => {
                        if (e.key === 'Enter') save(c.CustomerId, 'LastName', editValue)
                        if (e.key === 'Escape') setEditState(null)
                      }}
                      onBlur={() => save(c.CustomerId, 'LastName', editValue)}
                      autoFocus
                      style={{ font: 'inherit', border: '1px solid #0066cc', padding: '4px 6px', borderRadius: '3px', width: '100%' }}
                    />
                  ) : (
                    <span onClick={() => { setEditState({ customerId: c.CustomerId, field: 'LastName' }); setEditValue(c.LastName) }} style={{ cursor: 'pointer', padding: '2px 4px', borderRadius: '3px' }} onMouseEnter={(e) => (e.currentTarget.style.background = '#f0f0f0')} onMouseLeave={(e) => (e.currentTarget.style.background = 'transparent')}>
                      {c.LastName}
                    </span>
                  )}
                </td>
                <td style={{ padding: '12px', fontSize: '14px', fontWeight: 500, color: '#333' }}>
                  <DagCell table="Customers" field="Name">{c.Name}</DagCell>
                </td>
                <td style={{ padding: '12px', fontSize: '14px' }}>
                  <DagCell table="Customers" field="Initials">{c.Initials}</DagCell>
                </td>
                <td style={{ padding: '12px', fontSize: '14px', color: '#0066cc' }}>{c.EmailAddress}</td>
              </tr>
            ))}
          </tbody>
        </table>

        <p style={{ fontSize: '12px', color: '#999', marginTop: '20px' }}>✏️ Click to edit • ↩️ Enter to save • ⎋ Escape to cancel</p>
      </div>
    </div>
  )
}
