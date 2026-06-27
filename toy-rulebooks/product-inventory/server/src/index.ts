import express, { Request, Response, NextFunction } from 'express';
import { Pool } from 'pg';
import { readFile, writeFile, mkdir } from 'fs/promises';
import { spawn } from 'child_process';
import path from 'path';
import { fileURLToPath } from 'url';

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);
const PROJECT_ROOT = path.resolve(__dirname, '../..');
const RULEBOOK_PATH = path.join(PROJECT_ROOT, 'effortless-rulebook', 'effortless-rulebook.json');
const EXPORT_DIR = path.join(PROJECT_ROOT, 'excel-export');
const EXPORT_JSON = path.join(EXPORT_DIR, 'effortless-export.json');
const EXPORT_XLSX = path.join(EXPORT_DIR, 'rulebook.xlsx');

function pascalToSnake(s: string) {
  return s.replace(/([a-z0-9])([A-Z])/g, '$1_$2').toLowerCase();
}

const app = express();
const port = process.env.PORT || 3032;

app.use(express.json());

const pool = new Pool({
  connectionString: process.env.DATABASE_URL || 'postgresql://postgres@localhost:5432/erb_product_inventory',
});

interface User {
  email: string;
  role: string;
}

interface AuthRequest extends Request {
  user?: User;
}

const devUsers: User[] = [
  { email: 'warehouse@example.com', role: 'WarehouseManager' },
  { email: 'manager@example.com', role: 'Manager' },
  { email: 'accountant@example.com', role: 'Accountant' },
];

app.use((req: AuthRequest, res: Response, next: NextFunction) => {
  const email = req.header('X-User-Email');
  if (email) {
    const user = devUsers.find((u) => u.email === email);
    if (user) {
      req.user = user;
    }
  }
  next();
});

app.get('/api/dev-users', (req: Response, res: Response) => {
  res.json(devUsers);
});

app.get('/api/me', (req: AuthRequest, res: Response) => {
  if (!req.user) {
    res.status(401).json({ error: 'Not authenticated' });
    return;
  }
  res.json(req.user);
});

// Products endpoints
app.get('/api/products', async (req: AuthRequest, res: Response) => {
  try {
    const result = await pool.query('SELECT * FROM vw_products ORDER BY name');
    res.json(result.rows);
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: 'Database error' });
  }
});

app.get('/api/products/:id', async (req: AuthRequest, res: Response) => {
  try {
    const result = await pool.query(
      'SELECT * FROM vw_products WHERE product_id = $1',
      [req.params.id],
    );
    if (result.rows.length === 0) {
      res.status(404).json({ error: 'Not found' });
      return;
    }
    res.json(result.rows[0]);
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: 'Database error' });
  }
});

app.patch('/api/products/:id', async (req: AuthRequest, res: Response) => {
  if (req.user?.role !== 'WarehouseManager') {
    res.status(403).json({ error: 'Forbidden' });
    return;
  }

  const { unit_price, reorder_level } = req.body;
  if (typeof unit_price === 'undefined' && typeof reorder_level === 'undefined') {
    res.status(400).json({ error: 'No fields to update' });
    return;
  }

  try {
    const updates: string[] = [];
    const values: unknown[] = [];
    let paramCount = 1;

    if (typeof unit_price !== 'undefined') {
      updates.push(`unit_price = $${paramCount++}`);
      values.push(unit_price);
    }
    if (typeof reorder_level !== 'undefined') {
      updates.push(`reorder_level = $${paramCount++}`);
      values.push(reorder_level);
    }

    values.push(req.params.id);

    const result = await pool.query(
      `UPDATE products SET ${updates.join(', ')} WHERE product_id = $${paramCount} RETURNING *`,
      values,
    );

    if (result.rows.length === 0) {
      res.status(404).json({ error: 'Not found' });
      return;
    }

    // Return the updated view
    const viewResult = await pool.query(
      'SELECT * FROM vw_products WHERE product_id = $1',
      [req.params.id],
    );
    res.json(viewResult.rows[0]);
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: 'Database error' });
  }
});

// Transaction Types endpoint
app.get('/api/transaction-types', async (req: AuthRequest, res: Response) => {
  try {
    const result = await pool.query('SELECT * FROM vw_transaction_types ORDER BY name');
    res.json(result.rows);
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: 'Database error' });
  }
});

// Transactions endpoints
app.get('/api/transactions', async (req: AuthRequest, res: Response) => {
  try {
    const result = await pool.query('SELECT * FROM vw_transactions ORDER BY transaction_date DESC');
    res.json(result.rows);
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: 'Database error' });
  }
});

app.get('/api/transactions/:id', async (req: AuthRequest, res: Response) => {
  try {
    const result = await pool.query(
      'SELECT * FROM vw_transactions WHERE transaction_id = $1',
      [req.params.id],
    );
    if (result.rows.length === 0) {
      res.status(404).json({ error: 'Not found' });
      return;
    }
    res.json(result.rows[0]);
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: 'Database error' });
  }
});

app.post('/api/transactions', async (req: AuthRequest, res: Response) => {
  if (req.user?.role !== 'WarehouseManager') {
    res.status(403).json({ error: 'Forbidden' });
    return;
  }

  const { transaction_id, product, transaction_type, quantity, transaction_date, notes } = req.body;

  if (!transaction_id || !product || !transaction_type || typeof quantity === 'undefined' || !transaction_date) {
    res.status(400).json({ error: 'Missing required fields' });
    return;
  }

  try {
    await pool.query(
      `INSERT INTO transactions (transaction_id, product, transaction_type, quantity, transaction_date, notes)
       VALUES ($1, $2, $3, $4, $5, $6)`,
      [transaction_id, product, transaction_type, quantity, transaction_date, notes || null],
    );

    const result = await pool.query(
      'SELECT * FROM vw_transactions WHERE transaction_id = $1',
      [transaction_id],
    );
    res.status(201).json(result.rows[0]);
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: 'Database error' });
  }
});

app.patch('/api/transactions/:id', async (req: AuthRequest, res: Response) => {
  if (req.user?.role !== 'WarehouseManager') {
    res.status(403).json({ error: 'Forbidden' });
    return;
  }

  const { quantity, notes } = req.body;
  if (typeof quantity === 'undefined' && typeof notes === 'undefined') {
    res.status(400).json({ error: 'No fields to update' });
    return;
  }

  try {
    const updates: string[] = [];
    const values: unknown[] = [];
    let paramCount = 1;

    if (typeof quantity !== 'undefined') {
      updates.push(`quantity = $${paramCount++}`);
      values.push(quantity);
    }
    if (typeof notes !== 'undefined') {
      updates.push(`notes = $${paramCount++}`);
      values.push(notes);
    }

    values.push(req.params.id);

    const result = await pool.query(
      `UPDATE transactions SET ${updates.join(', ')} WHERE transaction_id = $${paramCount} RETURNING *`,
      values,
    );

    if (result.rows.length === 0) {
      res.status(404).json({ error: 'Not found' });
      return;
    }

    const viewResult = await pool.query(
      'SELECT * FROM vw_transactions WHERE transaction_id = $1',
      [req.params.id],
    );
    res.json(viewResult.rows[0]);
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: 'Database error' });
  }
});

app.get('/api/export/xlsx', async (req: AuthRequest, res: Response) => {
  if (!req.user) {
    res.status(401).json({ error: 'Not authenticated' });
    return;
  }
  try {
    const rulebook: any = JSON.parse(await readFile(RULEBOOK_PATH, 'utf8'));

    for (const [tableName, tableDef] of Object.entries<any>(rulebook)) {
      if (!tableDef || typeof tableDef !== 'object' || !Array.isArray(tableDef.schema)) continue;
      const rawFields = tableDef.schema.filter(
        (f: any) => f.type === 'raw' || f.type === 'relationship',
      );
      if (rawFields.length === 0) continue;

      const sqlTable = pascalToSnake(tableName);
      const colMap = rawFields.map((f: any) => ({ rulebook: f.name, sql: pascalToSnake(f.name) }));
      const cols = colMap.map((c) => `"${c.sql}"`).join(', ');
      const { rows } = await pool.query(`SELECT ${cols} FROM ${sqlTable}`);
      tableDef.data = rows.map((row: any) => {
        const obj: Record<string, unknown> = {};
        for (const c of colMap) {
          let v = row[c.sql];
          if (v instanceof Date) v = v.toISOString();
          if (v !== null && v !== undefined) obj[c.rulebook] = v;
        }
        return obj;
      });
    }

    await mkdir(EXPORT_DIR, { recursive: true });
    await writeFile(EXPORT_JSON, JSON.stringify(rulebook, null, 2));

    await new Promise<void>((resolve, reject) => {
      const proc = spawn(
        'effortless',
        ['rulebook-to-xlsx', '-i', 'effortless-export.json', '-o', 'rulebook.xlsx'],
        { cwd: EXPORT_DIR, stdio: 'inherit' },
      );
      proc.on('exit', (code) =>
        code === 0 ? resolve() : reject(new Error(`rulebook-to-xlsx exit ${code}`)),
      );
      proc.on('error', reject);
    });

    res.setHeader(
      'Content-Type',
      'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
    );
    res.setHeader('Content-Disposition', 'attachment; filename="rulebook.xlsx"');
    res.sendFile(EXPORT_XLSX);
  } catch (err) {
    console.error('Excel export failed:', err);
    res.status(500).json({ error: 'Export failed', details: String(err) });
  }
});

app.listen(port, () => {
  console.log(`Server running on http://localhost:${port}`);
});
