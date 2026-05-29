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

export default function Customers() {
  const [customers, setCustomers] = useState<Customer[]>([])
  const [editState, setEditState] = useState<EditState | null>(null)
  const [editValue, setEditValue] = useState('')

  useEffect(() => {
    load()
  }, [])

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
          <DagToggle />
        </div>

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
