import fs from 'node:fs/promises';
import path from 'node:path';
import { fileURLToPath } from 'node:url';
import { pool } from './db.js';

const __dirname = path.dirname(fileURLToPath(import.meta.url));

const sql = await fs.readFile(path.join(__dirname, 'schema.sql'), 'utf8');

console.log('Applying schema.sql ...');
await pool.query(sql);
console.log('Done.');
await pool.end();
