const pool = require('./db');

async function migrate() {
  // Create the customers table if it doesn't exist
  await pool.query(`
    CREATE TABLE IF NOT EXISTS customers (
      id        SERIAL PRIMARY KEY,
      name      TEXT NOT NULL,
      slug      TEXT NOT NULL UNIQUE,
      notes     TEXT NOT NULL DEFAULT '',
      color     TEXT NOT NULL DEFAULT '',
      created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
    )
  `);

  console.log('Table "customers" is ready.');

  // Only seed if the table is empty
  const { rows: countRows } = await pool.query('SELECT COUNT(*) AS cnt FROM customers');
  const count = parseInt(countRows[0].cnt, 10);

  if (count === 0) {
    const seedData = [
      { name: 'Bob',          slug: 'bob',          notes: 'currently in launch', color: 'Green'  },
      { name: 'Alice Johnson', slug: 'alice-johnson', notes: 'an early entry',    color: 'Green'  },
      { name: 'Brian Lee',    slug: 'brian-lee',    notes: 'an early entry',     color: 'Blue'   },
      { name: 'Carla Smith',  slug: 'carla-smith',  notes: 'an early entry',     color: 'Yellow' },
      { name: 'Caroline',     slug: 'caroline',     notes: '',                   color: 'Red'    },
    ];

    for (const c of seedData) {
      await pool.query(
        'INSERT INTO customers (name, slug, notes, color) VALUES ($1, $2, $3, $4)',
        [c.name, c.slug, c.notes, c.color]
      );
    }
    console.log('Seeded 5 initial customers.');
  } else {
    console.log(`Table already has ${count} customer(s); skipping seed.`);
  }
}

migrate()
  .then(() => {
    console.log('Migration complete.');
    process.exit(0);
  })
  .catch((err) => {
    console.error('Migration failed:', err);
    process.exit(1);
  });
