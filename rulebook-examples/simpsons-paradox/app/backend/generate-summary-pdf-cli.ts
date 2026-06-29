import fs from 'fs';
import path from 'path';
import { fileURLToPath } from 'url';
import { buildSummaryPdf } from './export-summary-pdf.js';
import { pool } from './db.js';

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const projectRoot = path.resolve(__dirname, '../..');
const outPath =
  process.env.SUMMARY_PDF_PATH ??
  path.join(projectRoot, 'simpsons-paradox-summary.pdf');

async function main() {
  const pdf = await buildSummaryPdf();
  fs.writeFileSync(outPath, pdf);
  console.log(`[summary-pdf] wrote ${outPath} (${pdf.length} bytes)`);
}

main()
  .catch((err) => {
    const message = err instanceof Error ? err.message : String(err);
    console.error('[summary-pdf] ERROR:', message);
    process.exit(1);
  })
  .finally(() => pool.end());
