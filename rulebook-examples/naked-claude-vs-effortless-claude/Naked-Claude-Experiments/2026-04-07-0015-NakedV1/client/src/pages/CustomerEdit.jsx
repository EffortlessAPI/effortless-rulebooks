import { useEffect, useState } from 'react';
import { Link, useNavigate, useParams } from 'react-router-dom';
import { api } from '../api.js';
import CustomerForm from './CustomerForm.jsx';

export default function CustomerEdit() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [customer, setCustomer] = useState(null);
  const [error, setError] = useState(null);

  useEffect(() => {
    api.get(id).then(setCustomer).catch(e => setError(e.message));
  }, [id]);

  if (error) return <div className="error">Error: {error}</div>;
  if (!customer) return <div>Loading…</div>;

  return (
    <div>
      <Link to={`/customers/${id}`}>← Back</Link>
      <h2>Edit {customer.name}</h2>
      <CustomerForm
        initial={customer}
        submitLabel="Save changes"
        onSubmit={async (data) => {
          await api.update(id, data);
          navigate(`/customers/${id}`);
        }}
      />
    </div>
  );
}
