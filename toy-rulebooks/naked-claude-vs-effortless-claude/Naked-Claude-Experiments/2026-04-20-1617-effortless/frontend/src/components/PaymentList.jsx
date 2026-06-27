import { useState } from 'react'
import { PayLabel } from './CustomerDetail.jsx'

const API = '/api'
const fmt = (n) => n == null ? '-' : Number(n).toLocaleString('en-US', {minimumFractionDigits:2,maximumFractionDigits:2})
const fmtDate = (d) => d ? new Date(d).toLocaleDateString() : '-'
const fmtDateInput = (d) => d ? new Date(d).toISOString().split('T')[0] : ''

function PmtStatus({ s }) {
  const colors = { Completed: '#059669', Pending: '#d97706', Failed: '#dc2626', Refunded: '#7c3aed', Cancelled: '#9ca3af' }
  return <span style={{fontWeight:600,color:colors[s]||'#374151'}}>{s||'—'}</span>
}

export default function PaymentList({ payments, orders, onEdit, onDelete, onRefresh }) {
  const [editPmt, setEditPmt] = useState(null)
  const orderMap = Object.fromEntries(orders.map(o=>[o.order_id,o]))

  const handleEditSave = async (form) => {
    const res = await fetch(`${API}/payments/${editPmt.payment_id}`, {
      method:'PUT', headers:{'Content-Type':'application/json'},
      body:JSON.stringify({...form, amount:Number(form.amount)})
    })
    if (!res.ok) { const e = await res.json(); alert('Error: '+e.error); return }
    setEditPmt(null)
    onRefresh && onRefresh()
  }

  return (
    <div style={{background:'#fff',border:'1px solid #e5e7eb',borderRadius:12,overflow:'hidden',boxShadow:'0 1px 3px rgba(0,0,0,0.07)'}}>
      <table style={{width:'100%',borderCollapse:'collapse',fontSize:13}}>
        <thead>
          <tr style={{background:'#f9fafb'}}>
            {['Payment ID','Order','Date','Method','Amount','Completed','Status','Order Total','Order Due','Paid?','Tx ID','Actions'].map(h=>(
              <th key={h} style={{padding:'10px 12px',textAlign:'left',fontWeight:600,color:'#6b7280',fontSize:11,borderBottom:'1px solid #e5e7eb'}}>{h}</th>
            ))}
          </tr>
        </thead>
        <tbody>
          {payments.length===0 && <tr><td colSpan={12} style={{padding:24,textAlign:'center',color:'#9ca3af'}}>No payments yet</td></tr>}
          {payments.map(p=>(
            <>
              <tr key={p.payment_id} style={{borderBottom:'1px solid #f3f4f6'}}
                onMouseEnter={e=>e.currentTarget.style.background='#f9fafb'}
                onMouseLeave={e=>e.currentTarget.style.background=''}>
                <td style={{padding:'8px 12px',fontSize:11,color:'#9ca3af'}}>{p.payment_id}</td>
                <td style={{padding:'8px 12px',fontWeight:600,color:'#4f46e5'}}>#{p.order_number}</td>
                <td style={{padding:'8px 12px'}}>{fmtDate(p.payment_date)}</td>
                <td style={{padding:'8px 12px'}}>{p.payment_method||'—'}</td>
                <td style={{padding:'8px 12px',fontWeight:700}}>${fmt(p.amount)}</td>
                <td style={{padding:'8px 12px',color:'#059669',fontWeight:p.is_completed?700:400}}>${fmt(p.completed_amount)}</td>
                <td style={{padding:'8px 12px'}}><PmtStatus s={p.payment_status} /></td>
                <td style={{padding:'8px 12px'}}>${fmt(p.order_total)}</td>
                <td style={{padding:'8px 12px',color:parseFloat(p.order_amount_due)>0?'#dc2626':'#059669',fontWeight:600}}>${fmt(p.order_amount_due)}</td>
                <td style={{padding:'8px 12px'}}>{p.order_is_paid_in_full?'✅':'—'}</td>
                <td style={{padding:'8px 12px',fontSize:11,color:'#9ca3af'}}>{p.transaction_id||'—'}</td>
                <td style={{padding:'8px 12px'}}>
                  <button onClick={()=>setEditPmt(editPmt?.payment_id===p.payment_id?null:p)} style={styles.btn}>Edit</button>
                  <button onClick={()=>onDelete(p)} style={{...styles.btn,...styles.danger}}>Del</button>
                </td>
              </tr>
              {editPmt?.payment_id===p.payment_id && (
                <tr><td colSpan={12} style={{padding:'8px 12px',background:'#f0fdf4',border:'1px solid #86efac'}}>
                  <InlinePaymentForm payment={p} onSave={handleEditSave} onCancel={()=>setEditPmt(null)} />
                </td></tr>
              )}
            </>
          ))}
        </tbody>
      </table>
    </div>
  )
}

function InlinePaymentForm({ payment, onSave, onCancel }) {
  const [form, setForm] = useState({
    payment_date: payment?.payment_date ? fmtDateInput(payment.payment_date) : '',
    amount: payment?.amount || '',
    payment_method: payment?.payment_method || 'CreditCard',
    payment_status: payment?.payment_status || 'Completed',
    transaction_id: payment?.transaction_id || '',
    notes: payment?.notes || ''
  })
  const set = k => e => setForm(f=>({...f,[k]:e.target.value}))

  return (
    <div style={{display:'flex',gap:10,flexWrap:'wrap',alignItems:'flex-end'}}>
      <div><label style={lbl}>Date</label><input type="date" value={form.payment_date} onChange={set('payment_date')} style={sinp} /></div>
      <div><label style={lbl}>Amount</label><input type="number" step="0.01" value={form.amount} onChange={set('amount')} style={{...sinp,width:100}} /></div>
      <div>
        <label style={lbl}>Method</label>
        <select value={form.payment_method} onChange={set('payment_method')} style={sinp}>
          {['CreditCard','BankTransfer','Cash','Check','PayPal','Other'].map(m=><option key={m}>{m}</option>)}
        </select>
      </div>
      <div>
        <label style={lbl}>Status</label>
        <select value={form.payment_status} onChange={set('payment_status')} style={sinp}>
          {['Pending','Completed','Failed','Refunded','Cancelled'].map(s=><option key={s}>{s}</option>)}
        </select>
      </div>
      <div><label style={lbl}>Tx ID</label><input value={form.transaction_id} onChange={set('transaction_id')} style={{...sinp,width:120}} /></div>
      <div style={{flex:1}}><label style={lbl}>Notes</label><input value={form.notes} onChange={set('notes')} style={{...sinp,width:'100%'}} /></div>
      <button onClick={()=>onSave(form)} style={styles.saveBtn}>Save</button>
      <button onClick={onCancel} style={styles.cancelBtn}>Cancel</button>
    </div>
  )
}

const lbl = {display:'block',fontSize:11,fontWeight:600,color:'#6b7280',marginBottom:3,textTransform:'uppercase'}
const sinp = {padding:'6px 10px',fontSize:13,border:'1px solid #d1d5db',borderRadius:6,boxSizing:'border-box'}
const styles = {
  btn: {background:'#f3f4f6',color:'#374151',border:'1px solid #d1d5db',borderRadius:6,padding:'4px 10px',fontSize:12,fontWeight:600,cursor:'pointer',marginRight:4},
  danger: {background:'#fef2f2',color:'#b91c1c',borderColor:'#fca5a5'},
  saveBtn: {background:'#059669',color:'#fff',border:'none',borderRadius:6,padding:'7px 14px',fontSize:13,fontWeight:600,cursor:'pointer'},
  cancelBtn: {background:'#f3f4f6',color:'#374151',border:'1px solid #d1d5db',borderRadius:6,padding:'7px 14px',fontSize:13,fontWeight:600,cursor:'pointer'}
}
