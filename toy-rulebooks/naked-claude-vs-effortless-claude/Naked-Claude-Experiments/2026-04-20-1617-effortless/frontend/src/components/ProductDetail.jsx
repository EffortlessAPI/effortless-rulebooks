import { useState, useEffect } from 'react'

const API = '/api'
const fmt = (n) => n == null ? '-' : Number(n).toLocaleString('en-US', {minimumFractionDigits:2,maximumFractionDigits:2})
const fmtDate = (d) => d ? new Date(d).toLocaleDateString() : '-'

export default function ProductDetail({ product, onEdit, onDelete }) {
  const [data, setData] = useState(null)
  useEffect(() => {
    fetch(`${API}/products/${product.product_id}`).then(r=>r.json()).then(setData)
  }, [product.product_id])

  const p = data || product
  return (
    <div style={{background:'#fff',border:'1px solid #e5e7eb',borderRadius:12,padding:24,boxShadow:'0 1px 3px rgba(0,0,0,0.07)'}}>
      <div style={{display:'flex',justifyContent:'space-between',alignItems:'flex-start',marginBottom:20}}>
        <div>
          <div style={{fontFamily:'monospace',fontSize:13,color:'#6b7280',marginBottom:4}}>{p.sku}</div>
          <h2 style={{margin:0,fontSize:22,fontWeight:700}}>{p.display_name}</h2>
        </div>
        <div style={{display:'flex',gap:8}}>
          <button onClick={onEdit} style={styles.btn}>✏️ Edit</button>
          <button onClick={onDelete} style={{...styles.btn,...styles.danger}}>🗑️ Delete</button>
        </div>
      </div>

      {p.description && <p style={{color:'#6b7280',marginBottom:20,lineHeight:1.6}}>{p.description}</p>}

      <div style={{display:'grid',gridTemplateColumns:'repeat(auto-fit,minmax(140px,1fr))',gap:14,marginBottom:24}}>
        <Metric label="Unit Price" value={`$${fmt(p.unit_price)}`} />
        <Metric label="Cost" value={`$${fmt(p.cost)}`} />
        <Metric label="Stock Quantity" value={p.stock_quantity ?? '—'} color={p.stock_quantity===0?'#dc2626':undefined} />
        <Metric label="Status" value={p.is_active ? '✅ Active' : '❌ Inactive'} />
        <Metric label="Internal ID" value={<code style={{fontSize:12,background:'#f3f4f6',padding:'2px 6px',borderRadius:4}}>{p.product_id}</code>} />
      </div>

      <h3 style={{margin:'20px 0 12px',fontSize:16,fontWeight:700}}>Order Line Items</h3>
      {!data ? <div style={{color:'#9ca3af'}}>Loading...</div> :
        !data.line_item_list?.length ? <div style={{color:'#9ca3af',fontSize:14}}>No line items yet</div> : (
          <table style={{width:'100%',borderCollapse:'collapse',fontSize:13}}>
            <thead>
              <tr style={{background:'#f9fafb'}}>
                {['Order','Date','Qty','Unit Price','Discount','Subtotal','Notes'].map(h=>(
                  <th key={h} style={thStyle}>{h}</th>
                ))}
              </tr>
            </thead>
            <tbody>
              {data.line_item_list.map(li=>(
                <tr key={li.order_line_item_id} style={{borderBottom:'1px solid #f3f4f6'}}>
                  <td style={tdStyle}><span style={{fontWeight:600,color:'#4f46e5'}}>#{li.order_number} ({li.customer})</span></td>
                  <td style={tdStyle}>{fmtDate(li.order_date)}</td>
                  <td style={tdStyle}>{li.quantity}</td>
                  <td style={tdStyle}>${fmt(li.unit_price)}</td>
                  <td style={tdStyle}>{li.discount_percent > 0 ? `${(li.discount_percent*100).toFixed(0)}%` : '—'}</td>
                  <td style={{...tdStyle,fontWeight:600}}>${fmt(li.sub_total)}</td>
                  <td style={{...tdStyle,color:'#6b7280'}}>{li.notes||''}</td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
    </div>
  )
}

function Metric({ label, value, color }) {
  return (
    <div style={{background:'#f9fafb',border:'1px solid #e5e7eb',borderRadius:8,padding:'12px 14px'}}>
      <div style={{fontSize:12,fontWeight:600,color:'#9ca3af',textTransform:'uppercase',letterSpacing:'0.05em',marginBottom:4}}>{label}</div>
      <div style={{fontSize:16,fontWeight:700,color:color||'#1a1a2e'}}>{value}</div>
    </div>
  )
}

const thStyle = {padding:'8px 12px',textAlign:'left',fontWeight:600,color:'#6b7280',fontSize:12,borderBottom:'1px solid #e5e7eb'}
const tdStyle = {padding:'8px 12px'}
const styles = {
  btn: {background:'#f3f4f6',color:'#374151',border:'1px solid #d1d5db',borderRadius:8,padding:'8px 16px',fontSize:14,fontWeight:600,cursor:'pointer'},
  danger: {background:'#fef2f2',color:'#b91c1c',borderColor:'#fca5a5'}
}
