const fmt = (n) => n == null ? '-' : Number(n).toLocaleString('en-US', {minimumFractionDigits:2,maximumFractionDigits:2})

export default function Dashboard({ summary, onNavigate }) {
  if (!summary) return <div style={{color:'#666'}}>Loading dashboard...</div>
  const { customers, orders, products, payments, total_revenue, outstanding } = summary

  return (
    <div>
      <h2 style={{margin:'0 0 20px',fontSize:22,fontWeight:700,color:'#1a1a2e'}}>Dashboard</h2>

      {/* Big metrics row */}
      <div style={{display:'grid',gridTemplateColumns:'repeat(auto-fit,minmax(160px,1fr))',gap:16,marginBottom:28}}>
        <MetricCard title="Total Revenue" value={`$${fmt(total_revenue)}`} color="#059669" />
        <MetricCard title="Outstanding" value={`$${fmt(outstanding)}`} color={parseFloat(outstanding)>0?"#dc2626":"#059669"} />
        <MetricCard title="Customers" value={customers.total} sub={`${customers.stopped} stopped`} color="#4f46e5" onClick={()=>onNavigate('customers')} />
        <MetricCard title="Orders" value={orders.total} color="#7c3aed" onClick={()=>onNavigate('orders')} />
        <MetricCard title="Products" value={products.total} sub={`${products.active} active`} color="#0891b2" onClick={()=>onNavigate('products')} />
      </div>

      <div style={{display:'grid',gridTemplateColumns:'1fr 1fr',gap:24}}>
        {/* Orders by status */}
        <Section title="Orders by Status">
          {orders.by_status.length === 0 ? <Empty /> : orders.by_status.map(s=>(
            <Row key={s.order_status} label={s.order_status||'(none)'} value={s.cnt} />
          ))}
        </Section>

        {/* Payments by status */}
        <Section title="Payments by Status">
          {payments.by_status.length === 0 ? <Empty /> : payments.by_status.map(s=>(
            <Row key={s.payment_status} label={s.payment_status||'(none)'}
              value={s.cnt}
              extra={s.payment_status==='Completed' ? ` · $${fmt(s.completed_amount)}` : ''} />
          ))}
        </Section>
      </div>
    </div>
  )
}

function MetricCard({ title, value, sub, color, onClick }) {
  return (
    <div onClick={onClick} style={{
      background:'#fff',border:'1px solid #e5e7eb',borderRadius:12,padding:'18px 20px',
      cursor:onClick?'pointer':'default',boxShadow:'0 1px 3px rgba(0,0,0,0.07)',
      transition:'box-shadow 0.15s',
    }}
    onMouseEnter={e=>onClick&&(e.currentTarget.style.boxShadow='0 4px 12px rgba(0,0,0,0.12)')}
    onMouseLeave={e=>onClick&&(e.currentTarget.style.boxShadow='0 1px 3px rgba(0,0,0,0.07)')}
    >
      <div style={{fontSize:13,fontWeight:600,color:'#6b7280',marginBottom:6,textTransform:'uppercase',letterSpacing:'0.05em'}}>{title}</div>
      <div style={{fontSize:28,fontWeight:700,color}}>{value}</div>
      {sub && <div style={{fontSize:13,color:'#9ca3af',marginTop:4}}>{sub}</div>}
    </div>
  )
}

function Section({ title, children }) {
  return (
    <div style={{background:'#fff',border:'1px solid #e5e7eb',borderRadius:12,padding:'18px 20px',boxShadow:'0 1px 3px rgba(0,0,0,0.07)'}}>
      <div style={{fontSize:14,fontWeight:700,color:'#374151',marginBottom:12,borderBottom:'1px solid #f3f4f6',paddingBottom:8}}>{title}</div>
      {children}
    </div>
  )
}

function Row({ label, value, extra='' }) {
  return (
    <div style={{display:'flex',justifyContent:'space-between',alignItems:'center',padding:'5px 0',fontSize:14}}>
      <span style={{color:'#374151'}}>{label}</span>
      <span style={{fontWeight:600,color:'#1a1a2e'}}>{value}{extra}</span>
    </div>
  )
}

function Empty() { return <div style={{color:'#9ca3af',fontSize:14}}>No data</div> }
