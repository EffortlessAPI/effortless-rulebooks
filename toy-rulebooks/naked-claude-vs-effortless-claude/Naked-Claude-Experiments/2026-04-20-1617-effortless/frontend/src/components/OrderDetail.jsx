import { useState, useEffect } from 'react'
import { PayLabel } from './CustomerDetail.jsx'

const API = '/api'
const fmt = (n) => n == null ? '-' : Number(n).toLocaleString('en-US', {minimumFractionDigits:2,maximumFractionDigits:2})
const fmtDate = (d) => d ? new Date(d).toLocaleDateString() : '-'
const fmtDateInput = (d) => d ? new Date(d).toISOString().split('T')[0] : ''

export default function OrderDetail({ order, products, onEdit, onDelete, onRefresh }) {
  const [data, setData] = useState(null)
  const [editLI, setEditLI] = useState(null)
  const [editPmt, setEditPmt] = useState(null)
  const [newLI, setNewLI] = useState(false)
  const [newPmt, setNewPmt] = useState(false)
  const prodMap = Object.fromEntries(products.map(p=>[p.product_id,p]))

  const loadData = () => {
    fetch(`${API}/orders/${order.order_id}`).then(r=>r.json()).then(d=>{setData(d)})
  }

  useEffect(()=>{ loadData() }, [order.order_id])

  const refresh = () => { loadData(); onRefresh && onRefresh() }

  const o = data || order

  const deleteLI = async (id) => {
    if (!confirm('Delete this line item?')) return
    await fetch(`${API}/order-line-items/${id}`, {method:'DELETE'})
    refresh()
  }
  const deletePmt = async (id) => {
    if (!confirm('Delete this payment?')) return
    await fetch(`${API}/payments/${id}`, {method:'DELETE'})
    refresh()
  }

  return (
    <div style={{background:'#fff',border:'1px solid #e5e7eb',borderRadius:12,padding:24,boxShadow:'0 1px 3px rgba(0,0,0,0.07)'}}>
      {/* Header */}
      <div style={{display:'flex',justifyContent:'space-between',alignItems:'flex-start',marginBottom:20}}>
        <div>
          <div style={{fontSize:13,color:'#9ca3af',marginBottom:4}}>{o.customer} · {fmtDate(o.order_date)}</div>
          <h2 style={{margin:0,fontSize:22,fontWeight:700}}>Order #{o.order_number}</h2>
          <div style={{marginTop:6,display:'flex',gap:8,alignItems:'center'}}>
            <span style={{background:'#f3f4f6',color:'#374151',padding:'2px 10px',borderRadius:999,fontSize:12,fontWeight:600}}>{o.order_status}</span>
            <PayLabel label={o.payment_status_label} />
          </div>
        </div>
        <div style={{display:'flex',gap:8}}>
          <button onClick={onEdit} style={styles.btn}>✏️ Edit</button>
          <button onClick={onDelete} style={{...styles.btn,...styles.danger}}>🗑️ Delete</button>
        </div>
      </div>

      {/* Money metrics */}
      <div style={{display:'grid',gridTemplateColumns:'repeat(auto-fit,minmax(120px,1fr))',gap:12,marginBottom:24}}>
        <Metric label="Subtotal" value={`$${fmt(o.sub_total)}`} />
        <Metric label="Tax Rate" value={o.tax_rate != null ? `${(o.tax_rate*100).toFixed(2)}%` : '—'} />
        <Metric label="Tax Amount" value={`$${fmt(o.tax_amount)}`} />
        <Metric label="Order Total" value={`$${fmt(o.order_total)}`} bold />
        <Metric label="Total Paid" value={`$${fmt(o.total_paid)}`} color="#059669" />
        <Metric label="Amount Due" value={`$${fmt(o.amount_due)}`} color={parseFloat(o.amount_due)>0?"#dc2626":"#059669"} bold />
        <Metric label="Items" value={o.item_count} />
        <Metric label="Total Qty" value={o.total_quantity} />
        <Metric label="Payments" value={o.payment_count} />
        {o.last_payment_date && <Metric label="Last Paid" value={fmtDate(o.last_payment_date)} />}
      </div>

      {/* Addresses + notes */}
      {(o.shipping_address || o.billing_address || o.notes) && (
        <div style={{display:'grid',gridTemplateColumns:'1fr 1fr',gap:12,marginBottom:24,fontSize:13,color:'#6b7280'}}>
          {o.shipping_address && <div><strong>Ship to:</strong> {o.shipping_address}</div>}
          {o.billing_address && <div><strong>Bill to:</strong> {o.billing_address}</div>}
          {o.notes && <div style={{gridColumn:'1/-1'}}><strong>Notes:</strong> {o.notes}</div>}
        </div>
      )}

      {/* ===== LINE ITEMS ===== */}
      <div style={{display:'flex',justifyContent:'space-between',alignItems:'center',marginBottom:10}}>
        <h3 style={{margin:0,fontSize:16,fontWeight:700}}>Line Items</h3>
        <button onClick={()=>{setNewLI(true);setEditLI(null)}} style={styles.addBtn}>+ Add Line Item</button>
      </div>

      {newLI && (
        <LineItemForm products={products} orderId={order.order_id}
          onSave={()=>{setNewLI(false);refresh()}} onCancel={()=>setNewLI(false)} />
      )}

      {!data ? <div style={{color:'#9ca3af',marginBottom:20}}>Loading...</div> : (
        <table style={{width:'100%',borderCollapse:'collapse',fontSize:13,marginBottom:24}}>
          <thead>
            <tr style={{background:'#f9fafb'}}>
              {['#','Product','Qty','Unit Price','Discount','Pre-Discount','Discount Amt','Subtotal','Notes','Actions'].map(h=>(
                <th key={h} style={thStyle}>{h}</th>
              ))}
            </tr>
          </thead>
          <tbody>
            {data.line_item_list.length===0 && <tr><td colSpan={10} style={{padding:16,textAlign:'center',color:'#9ca3af'}}>No line items</td></tr>}
            {data.line_item_list.map(li=>(
              <>
                <tr key={li.order_line_item_id} style={{borderBottom:'1px solid #f3f4f6'}}>
                  <td style={tdStyle}>{li.line_number}</td>
                  <td style={tdStyle}><span style={{fontWeight:600}}>{li.product_display_name||li.product}</span><br/><span style={{color:'#9ca3af',fontSize:11}}>{li.product_sku||li.product}</span></td>
                  <td style={tdStyle}>{li.quantity}</td>
                  <td style={tdStyle}>${fmt(li.unit_price)}</td>
                  <td style={tdStyle}>{li.discount_percent > 0 ? `${(li.discount_percent*100).toFixed(0)}%` : '—'}</td>
                  <td style={tdStyle}>${fmt(li.pre_discount)}</td>
                  <td style={tdStyle}>{li.discount_amount > 0 ? `-$${fmt(li.discount_amount)}` : '—'}</td>
                  <td style={{...tdStyle,fontWeight:700}}>${fmt(li.sub_total)}</td>
                  <td style={{...tdStyle,color:'#6b7280',fontSize:12}}>{li.notes||''}</td>
                  <td style={tdStyle}>
                    <button onClick={()=>{setEditLI(li);setNewLI(false)}} style={styles.smBtn}>Edit</button>
                    <button onClick={()=>deleteLI(li.order_line_item_id)} style={{...styles.smBtn,...styles.smDanger}}>Del</button>
                  </td>
                </tr>
                {editLI?.order_line_item_id===li.order_line_item_id && (
                  <tr><td colSpan={10} style={{padding:0,background:'#f9fafb'}}>
                    <LineItemForm products={products} orderId={order.order_id} lineItem={li}
                      onSave={()=>{setEditLI(null);refresh()}} onCancel={()=>setEditLI(null)} />
                  </td></tr>
                )}
              </>
            ))}
          </tbody>
        </table>
      )}

      {/* ===== PAYMENTS ===== */}
      <div style={{display:'flex',justifyContent:'space-between',alignItems:'center',marginBottom:10}}>
        <h3 style={{margin:0,fontSize:16,fontWeight:700}}>Payments</h3>
        <button onClick={()=>{setNewPmt(true);setEditPmt(null)}} style={styles.addBtn}>+ Add Payment</button>
      </div>

      {newPmt && (
        <PaymentForm orderId={order.order_id}
          onSave={()=>{setNewPmt(false);refresh()}} onCancel={()=>setNewPmt(false)} />
      )}

      {!data ? <div style={{color:'#9ca3af'}}>Loading...</div> : (
        <table style={{width:'100%',borderCollapse:'collapse',fontSize:13}}>
          <thead>
            <tr style={{background:'#f9fafb'}}>
              {['#','Date','Method','Amount','Completed Amt','Status','Tx ID','Notes','Actions'].map(h=>(
                <th key={h} style={thStyle}>{h}</th>
              ))}
            </tr>
          </thead>
          <tbody>
            {data.payment_list.length===0 && <tr><td colSpan={9} style={{padding:16,textAlign:'center',color:'#9ca3af'}}>No payments</td></tr>}
            {data.payment_list.map(pm=>(
              <>
                <tr key={pm.payment_id} style={{borderBottom:'1px solid #f3f4f6'}}>
                  <td style={tdStyle}>{pm.payment_number}</td>
                  <td style={tdStyle}>{fmtDate(pm.payment_date)}</td>
                  <td style={tdStyle}>{pm.payment_method||'—'}</td>
                  <td style={{...tdStyle,fontWeight:600}}>${fmt(pm.amount)}</td>
                  <td style={{...tdStyle,color:'#059669',fontWeight:pm.is_completed?700:400}}>${fmt(pm.completed_amount)}</td>
                  <td style={tdStyle}><PmtStatus s={pm.payment_status} /></td>
                  <td style={{...tdStyle,fontSize:11,color:'#9ca3af'}}>{pm.transaction_id||'—'}</td>
                  <td style={{...tdStyle,color:'#6b7280',fontSize:12}}>{pm.notes||''}</td>
                  <td style={tdStyle}>
                    <button onClick={()=>{setEditPmt(pm);setNewPmt(false)}} style={styles.smBtn}>Edit</button>
                    <button onClick={()=>deletePmt(pm.payment_id)} style={{...styles.smBtn,...styles.smDanger}}>Del</button>
                  </td>
                </tr>
                {editPmt?.payment_id===pm.payment_id && (
                  <tr><td colSpan={9} style={{padding:0,background:'#f9fafb'}}>
                    <PaymentForm orderId={order.order_id} payment={pm}
                      onSave={()=>{setEditPmt(null);refresh()}} onCancel={()=>setEditPmt(null)} />
                  </td></tr>
                )}
              </>
            ))}
          </tbody>
        </table>
      )}
    </div>
  )
}

function PmtStatus({ s }) {
  const colors = {
    Completed: '#059669', Pending: '#d97706', Failed: '#dc2626',
    Refunded: '#7c3aed', Cancelled: '#9ca3af'
  }
  return <span style={{fontWeight:600,color:colors[s]||'#374151'}}>{s||'—'}</span>
}

function LineItemForm({ products, orderId, lineItem, onSave, onCancel }) {
  const [form, setForm] = useState({
    product: lineItem?.product || '',
    quantity: lineItem?.quantity || 1,
    unit_price: lineItem?.unit_price || '',
    discount_percent: lineItem ? (lineItem.discount_percent || 0) : 0,
    notes: lineItem?.notes || ''
  })
  const set = k => e => setForm(f=>({...f,[k]:e.target.value}))

  const save = async () => {
    const body = { ...form, quantity: Number(form.quantity), unit_price: Number(form.unit_price), discount_percent: Number(form.discount_percent) }
    let res
    if (lineItem) {
      res = await fetch(`${API}/order-line-items/${lineItem.order_line_item_id}`, {method:'PUT',headers:{'Content-Type':'application/json'},body:JSON.stringify(body)})
    } else {
      res = await fetch(`${API}/order-line-items`, {method:'POST',headers:{'Content-Type':'application/json'},body:JSON.stringify({...body,order:orderId})})
    }
    if (!res.ok) { const e=await res.json(); alert('Error: '+e.error); return }
    onSave()
  }

  // Auto-fill price from product
  const onProductChange = e => {
    const pid = e.target.value
    const prod = products.find(p=>p.product_id===pid)
    setForm(f=>({...f,product:pid,unit_price:prod?.unit_price||f.unit_price}))
  }

  return (
    <div style={{padding:'12px 16px',background:'#fff7ed',border:'1px solid #fed7aa',borderRadius:8,margin:'4px 0'}}>
      <div style={{display:'flex',gap:10,flexWrap:'wrap',alignItems:'flex-end'}}>
        <div>
          <label style={lbl}>Product</label>
          <select value={form.product} onChange={onProductChange} style={{...sinp,minWidth:160}}>
            <option value="">Select...</option>
            {products.filter(p=>p.is_active||p.product_id===lineItem?.product).map(p=>(
              <option key={p.product_id} value={p.product_id}>{p.display_name} ({p.sku})</option>
            ))}
          </select>
        </div>
        <div><label style={lbl}>Qty</label><input type="number" value={form.quantity} onChange={set('quantity')} style={{...sinp,width:70}} /></div>
        <div><label style={lbl}>Unit Price</label><input type="number" step="0.01" value={form.unit_price} onChange={set('unit_price')} style={{...sinp,width:90}} /></div>
        <div><label style={lbl}>Discount %</label><input type="number" step="0.01" value={form.discount_percent} onChange={set('discount_percent')} style={{...sinp,width:90}} placeholder="0.10=10%" /></div>
        <div style={{flex:1}}><label style={lbl}>Notes</label><input value={form.notes} onChange={set('notes')} style={{...sinp,width:'100%'}} /></div>
        <button onClick={save} style={styles.saveBtn}>Save</button>
        <button onClick={onCancel} style={styles.cancelBtn}>Cancel</button>
      </div>
    </div>
  )
}

function PaymentForm({ orderId, payment, onSave, onCancel }) {
  const [form, setForm] = useState({
    payment_date: payment?.payment_date ? fmtDateInput(payment.payment_date) : new Date().toISOString().split('T')[0],
    amount: payment?.amount || '',
    payment_method: payment?.payment_method || 'CreditCard',
    payment_status: payment?.payment_status || 'Completed',
    transaction_id: payment?.transaction_id || '',
    notes: payment?.notes || ''
  })
  const set = k => e => setForm(f=>({...f,[k]:e.target.value}))

  const save = async () => {
    const body = { ...form, amount: Number(form.amount) }
    let res
    if (payment) {
      res = await fetch(`${API}/payments/${payment.payment_id}`, {method:'PUT',headers:{'Content-Type':'application/json'},body:JSON.stringify(body)})
    } else {
      res = await fetch(`${API}/payments`, {method:'POST',headers:{'Content-Type':'application/json'},body:JSON.stringify({...body,order:orderId})})
    }
    if (!res.ok) { const e=await res.json(); alert('Error: '+e.error); return }
    onSave()
  }

  return (
    <div style={{padding:'12px 16px',background:'#f0fdf4',border:'1px solid #86efac',borderRadius:8,margin:'4px 0'}}>
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
        <button onClick={save} style={styles.saveBtn}>Save</button>
        <button onClick={onCancel} style={styles.cancelBtn}>Cancel</button>
      </div>
    </div>
  )
}

const thStyle = {padding:'8px 10px',textAlign:'left',fontWeight:600,color:'#6b7280',fontSize:11,borderBottom:'1px solid #e5e7eb'}
const tdStyle = {padding:'8px 10px',fontSize:13}
const lbl = {display:'block',fontSize:11,fontWeight:600,color:'#6b7280',marginBottom:3,textTransform:'uppercase'}
const sinp = {padding:'6px 10px',fontSize:13,border:'1px solid #d1d5db',borderRadius:6,boxSizing:'border-box'}
const styles = {
  btn: {background:'#f3f4f6',color:'#374151',border:'1px solid #d1d5db',borderRadius:8,padding:'8px 16px',fontSize:14,fontWeight:600,cursor:'pointer'},
  danger: {background:'#fef2f2',color:'#b91c1c',borderColor:'#fca5a5'},
  addBtn: {background:'#4f46e5',color:'#fff',border:'none',borderRadius:8,padding:'6px 14px',fontSize:13,fontWeight:600,cursor:'pointer'},
  smBtn: {background:'#f3f4f6',color:'#374151',border:'1px solid #d1d5db',borderRadius:4,padding:'3px 8px',fontSize:11,fontWeight:600,cursor:'pointer',marginRight:3},
  smDanger: {background:'#fef2f2',color:'#b91c1c',borderColor:'#fca5a5'},
  saveBtn: {background:'#059669',color:'#fff',border:'none',borderRadius:6,padding:'7px 14px',fontSize:13,fontWeight:600,cursor:'pointer'},
  cancelBtn: {background:'#f3f4f6',color:'#374151',border:'1px solid #d1d5db',borderRadius:6,padding:'7px 14px',fontSize:13,fontWeight:600,cursor:'pointer'}
}
