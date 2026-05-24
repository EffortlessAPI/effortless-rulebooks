import { Link, useNavigate } from 'react-router-dom';
import { api } from '../api.js';
import CustomerForm from './CustomerForm.jsx';

export default function CustomerNew() {
  const navigate = useNavigate();
  return (
    <div>
      <Link to="/">← Back</Link>
      <h2>New customer</h2>
      <CustomerForm
        submitLabel="Create"
        onSubmit={async (data) => {
          const created = await api.create(data);
          navigate(`/customers/${created.id}`);
        }}
      />
    </div>
  );
}
