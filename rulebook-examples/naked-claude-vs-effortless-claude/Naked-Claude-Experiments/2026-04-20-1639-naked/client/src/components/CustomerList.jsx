import CustomerCard from './CustomerCard.jsx';

export default function CustomerList({ customers, onSelect }) {
  if (!customers.length) {
    return (
      <div style={styles.empty}>
        No customers yet. Click <strong>+ Add Customer</strong> to get started.
      </div>
    );
  }

  return (
    <div>
      <h2 style={styles.heading}>Customers <span style={styles.count}>({customers.length})</span></h2>
      <div style={styles.list}>
        {customers.map((c) => (
          <CustomerCard key={c.id} customer={c} onClick={() => onSelect(c.id)} />
        ))}
      </div>
    </div>
  );
}

const styles = {
  heading: {
    fontSize: 18,
    fontWeight: 700,
    color: '#333',
    marginBottom: 12,
  },
  count: {
    fontWeight: 400,
    color: '#888',
    fontSize: 15,
  },
  list: {
    display: 'flex',
    flexDirection: 'column',
    gap: 10,
  },
  empty: {
    background: '#fff',
    borderRadius: 10,
    padding: 40,
    textAlign: 'center',
    color: '#888',
    fontSize: 15,
    boxShadow: '0 1px 4px rgba(0,0,0,0.1)',
  },
};
