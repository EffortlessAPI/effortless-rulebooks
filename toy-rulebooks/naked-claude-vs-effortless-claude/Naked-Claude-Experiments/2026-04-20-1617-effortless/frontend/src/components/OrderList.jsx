import { PayLabel } from './CustomerDetail.jsx'

const fmt = (n) => n == null ? '-' : Number(n).toLocaleString('en-US', {minimumFractionDigits:2,maximumFractionDigits:2})
const fmtDate = (d) => d ? new Date(d).toLocaleDateString() : '-'

export default function OrderList({ orders, customers, onSelect, onEdit, onDelete }) {
  const custMap = Object.fromEntries(customers.map(c=>[c.customer_id,c.name]))

  return (
    <div style={{background:'#fff',border:'1px solid #e5e7eb',borderRadius:12,overflow:'hidden',boxShadow:'0 1px 3px rgba(0,0,0,0.07)'}}>
      <table style={{width:'100%',borderCollapse:'collapse',fontSize:14}}>
        <thead>
          <tr style={{background:'#f9fafb'}}>
            {['Order','Customer','Date','Status','Items','Total','Paid','Due','Label','Actions'].map(h=>(
              <th key={h} style={{padding:'10px 12px',textAlign:'left',fontWeight:600,color:'#6b7280',fontSize:12,borderBottom:'1px solid #e5e7eb'}}>{h}</th>
            ))}
          </tr>
        </thead>
        <tbody>
          {orders.length===0 && <tr><td colSpan={10} style={{padding:24,textAlign:'center',color:'#9ca3af'}}>No orders yet</td></tr>}
          {orders.map(o=>(
            <tr key={o.order_id} style={{borderBottom:'1px solid #f3f4f6'}}
              onMouseEnter={e=>e.currentTarget.style.background='#f9fafb'}
              onMouseLeave={e=>e.currentTarget.style.background=''}>
              <td style={{padding:'10px 12px'}}>
                <span style={{fontWeight:700,color:'#4f46e5',cursor:'pointer'}} onClick={()=>onSelect(o)}>#{o.order_number}</span>
              </td>
              <td style={{padding:'10px 12px',color:'#374151'}}>{custMap[o.customer]||o.customer}</td>
              <td style={{padding:'10px 12px',color:'#6b7280',fontSize:13}}>{fmtDate(o.order_date)}</td>
              <td style={{padding:'10px 12px',fontSize:12,color:'#374151'}}>{o.order_status}</td>
              <td style={{padding:'10px 12px',textAlign:'center'}}>{o.item_count}</td>
              <td style={{padding:'10px 12px',fontWeight:600}}>${fmt(o.order_total)}</td>
              <td style={{padding:'10px 12px',color:'#059669'}}>${fmt(o.total_paid)}</td>
              <td style={{padding:'10px 12px',color:parseFloat(o.amount_due)>0?'#dc2626':'#059669',fontWeight:600}}>${fmt(o.amount_due)}</td>
              <td style={{padding:'10px 12px'}}><PayLabel label={o.payment_status_label} /></td>
              <td style={{padding:'10px 12px'}}>
                <button onClick={()=>onEdit(o)} style={styles.btn}>Edit</button>
                <button onClick={()=>onDelete(o)} style={{...styles.btn,...styles.danger}}>Delete</button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  )
}

const styles = {
  btn: {background:'#f3f4f6',color:'#374151',border:'1px solid #d1d5db',borderRadius:6,padding:'4px 10px',fontSize:12,fontWeight:600,cursor:'pointer',marginRight:4},
  danger: {background:'#fef2f2',color:'#b91c1c',borderColor:'#fca5a5'}
}
