import fs from 'fs';
import path from 'path';
import { fileURLToPath } from 'url';
import { buildDetailsPdf } from './export-details-pdf.js';
import { pool } from './db.js';

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const projectRoot = path.resolve(__dirname, '../..');
const outPath =
  process.env.DETAILS_PDF_PATH ??
  path.join(projectRoot, 'simpsons-paradox-details.pdf');

async function main() {
  const pdf = await buildDetailsPdf();
  fs.writeFileSync(outPath, pdf);
  console.log(`[details-pdf] wrote ${outPath} (${pdf.length} bytes)`);
}

main()
  .catch((err) => {
    const message = err instanceof Error ? err.message : String(err);
    console.error('[details-pdf] ERROR:', message);
    process.exit(1);
  })
  .finally(() => pool.end());
