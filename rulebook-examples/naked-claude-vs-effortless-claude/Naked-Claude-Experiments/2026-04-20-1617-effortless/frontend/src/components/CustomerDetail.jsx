import { useState, useEffect } from 'react'
import { StatusBadge } from './CustomerList.jsx'

const API = '/api'
const fmt = (n) => n == null ? '-' : Number(n).toLocaleString('en-US', {minimumFractionDigits:2,maximumFractionDigits:2})
const fmtDate = (d) => d ? new Date(d).toLocaleDateString() : '-'

export default function CustomerDetail({ customer, onEdit, onDelete, onSelectOrder }) {
  const [data, setData] = useState(null)
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    fetch(`${API}/customers/${customer.customer_id}`)
      .then(r=>r.json()).then(d=>{ setData(d); setLoading(false) })
      .catch(()=>setLoading(false))
  }, [customer.customer_id])

  const c = data || customer

  return (
    <div style={{background:'#fff',border:'1px solid #e5e7eb',borderRadius:12,padding:24,boxShadow:'0 1px 3px rgba(0,0,0,0.07)'}}>
      <div style={{display:'flex',justifyContent:'space-between',alignItems:'flex-start',marginBottom:20}}>
        <div>
          <h2 style={{margin:'0 0 4px',fontSize:22,fontWeight:700}}>{c.name}</h2>
          {c.company_name && <div style={{color:'#6b7280',fontSize:15}}>{c.company_name}</div>}
        </div>
        <div style={{display:'flex',gap:8}}>
          <button onClick={onEdit} style={styles.btn}>✏️ Edit</button>
          <button onClick={onDelete} style={{...styles.btn,...styles.danger}}>🗑️ Delete</button>
        </div>
      </div>

      <div style={{display:'grid',gridTemplateColumns:'1fr 1fr',gap:16,marginBottom:20}}>
        <Field label="Email" value={c.email} />
        <Field label="Phone" value={c.phone} />
        <Field label="Billing Address" value={c.billing_address} />
        <Field label="Shipping Address" value={c.shipping_address} />
        <div>
          <Label>Status</Label>
          {c.status_display_name ? <StatusBadge label={c.status_display_name} blocking={c.is_stopped} /> : <span style={{color:'#9ca3af'}}>None</span>}
        </div>
        <Field label="Stopped" value={c.is_stopped ? '⛔ Yes (blocking status)' : '✓ No'} />
        <Field label="Customer Since" value={c.created_at ? `${fmtDate(c.created_at)} (${c.customer_since_days ?? '-'} days)` : '-'} />
        <Field label="Notes" value={c.notes} />
        <Field label="Avg Order Value" value={c.average_order_value === 'Unable to generate formula' ? '⚠️ Formula pending' : (c.average_order_value ? `$${fmt(c.average_order_value)}` : '-')} />
        <Field label="Days Since Last Order" value={c.days_since_last_order === 'Unable to generate formula' ? '⚠️ Formula pending' : c.days_since_last_order} />
        <Field label="Lifetime Margin %" value={c.lifetime_margin_percent === 'Unable to generate formula' ? '⚠️ Formula pending' : c.lifetime_margin_percent} />
      </div>

      {/* Orders list */}
      <h3 style={{margin:'24px 0 12px',fontSize:16,fontWeight:700,color:'#374151'}}>Orders</h3>
      {loading ? <div style={{color:'#9ca3af'}}>Loading orders...</div> :
        !data?.order_list?.length ? <div style={{color:'#9ca3af',fontSize:14}}>No orders</div> : (
          <table style={{width:'100%',borderCollapse:'collapse',fontSize:13}}>
            <thead>
              <tr style={{background:'#f9fafb'}}>
                {['Order #','Date','Status','Items','Total','Paid','Due','Label'].map(h=>(
                  <th key={h} style={thStyle}>{h}</th>
                ))}
              </tr>
            </thead>
            <tbody>
              {data.order_list.map(o=>(
                <tr key={o.order_id} style={{borderBottom:'1px solid #f3f4f6',cursor:'pointer'}}
                  onClick={()=>onSelectOrder(o)}
                  onMouseEnter={e=>e.currentTarget.style.background='#f9fafb'}
                  onMouseLeave={e=>e.currentTarget.style.background=''}>
                  <td style={tdStyle}><span style={{color:'#4f46e5',fontWeight:600}}>#{o.order_number}</span></td>
                  <td style={tdStyle}>{fmtDate(o.order_date)}</td>
                  <td style={tdStyle}>{o.order_status}</td>
                  <td style={tdStyle}>{o.item_count}</td>
                  <td style={tdStyle}>${fmt(o.order_total)}</td>
                  <td style={tdStyle}>${fmt(o.total_paid)}</td>
                  <td style={tdStyle}>${fmt(o.amount_due)}</td>
                  <td style={tdStyle}><PayLabel label={o.payment_status_label} /></td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
    </div>
  )
}

function Field({ label, value }) {
  return <div><Label>{label}</Label><div style={{color:'#1a1a2e',fontSize:14}}>{value||<span style={{color:'#9ca3af'}}>—</span>}</div></div>
}

function Label({ children }) {
  return <div style={{fontSize:12,fontWeight:600,color:'#9ca3af',textTransform:'uppercase',letterSpacing:'0.05em',marginBottom:3}}>{children}</div>
}

export function PayLabel({ label }) {
  const colors = { Paid: ['#f0fdf4','#166534','#86efac'], Partial: ['#fffbeb','#92400e','#fcd34d'], Unpaid: ['#fff7ed','#9a3412','#fdba74'] }
  const [bg, color, border] = colors[label] || ['#f3f4f6','#374151','#d1d5db']
  return <span style={{display:'inline-block',padding:'2px 10px',borderRadius:999,fontSize:12,fontWeight:600,background:bg,color,border:`1px solid ${border}`}}>{label||'—'}</span>
}

const thStyle = {padding:'8px 12px',textAlign:'left',fontWeight:600,color:'#6b7280',fontSize:12,borderBottom:'1px solid #e5e7eb'}
const tdStyle = {padding:'8px 12px'}
const styles = {
  btn: {background:'#f3f4f6',color:'#374151',border:'1px solid #d1d5db',borderRadius:8,padding:'8px 16px',fontSize:14,fontWeight:600,cursor:'pointer'},
  danger: {background:'#fef2f2',color:'#b91c1c',borderColor:'#fca5a5'}
}
