const express = require('express');
const fs = require('fs');
const path = require('path');

const app = express();
const PORT = 3000;

const rulebook_path = path.join(__dirname, 'effortless-rulebook/customer-fullname-rulebook.json');
let rulebook = JSON.parse(fs.readFileSync(rulebook_path, 'utf8'));

app.use(express.json());
app.use(express.static(__dirname, { extensions: ['html'] }));

app.get('/api/customers', (req, res) => {
  const customers = rulebook.Customers.data.map(c => ({
    ...c,
    FullName: `${c.FirstName} ${c.LastName}`,
    Name: c.EmailAddress.replace('@', '-')
  }));
  res.json(customers);
});

app.patch('/api/customers/:customerId', (req, res) => {
  const idx = rulebook.Customers.data.findIndex(c => c.CustomerId === req.params.customerId);
  if (idx === -1) return res.status(404).json({ error: 'Not found' });

  const { FirstName, LastName, EmailAddress } = req.body;

  if (FirstName !== undefined) rulebook.Customers.data[idx].FirstName = FirstName;
  if (LastName !== undefined) rulebook.Customers.data[idx].LastName = LastName;
  if (EmailAddress !== undefined) rulebook.Customers.data[idx].EmailAddress = EmailAddress;

  fs.writeFileSync(rulebook_path, JSON.stringify(rulebook, null, 2));

  const updated = rulebook.Customers.data[idx];
  res.json({
    ...updated,
    FullName: `${updated.FirstName} ${updated.LastName}`,
    Name: updated.EmailAddress.replace('@', '-')
  });
});

app.listen(PORT, () => {
  console.log(`Demo app running at http://localhost:${PORT}`);
  console.log(`Press Ctrl+C to stop`);
});
