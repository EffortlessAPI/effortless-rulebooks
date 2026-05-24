const { Pool } = require('pg');
const crypto = require('crypto');

const pool = new Pool({
  connectionString: process.env.DATABASE_URL ||
    'postgresql://postgres@localhost:5432/erb_v4_nakedclaude_demo'
});

function newId(prefix) {
  return `${prefix}_${crypto.randomBytes(6).toString('hex')}`;
}

module.exports = { pool, newId };
