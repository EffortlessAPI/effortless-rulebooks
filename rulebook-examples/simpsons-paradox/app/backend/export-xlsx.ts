import { spawn } from 'node:child_process';
import * as fs from 'node:fs';
import * as path from 'node:path';
import { fileURLToPath } from 'node:url';
import type { Response } from 'express';
import { query } from './db.js';

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const PROJECT_ROOT = path.resolve(__dirname, '../..');
const RULEBOOK_PATH = path.join(
  PROJECT_ROOT,
  'effortless-rulebook/simpsons-paradox-rulebook.json',
);
const PROJECT_NAME = process.env.PROJECT_NAME ?? 'simpsons-paradox';

const RESERVED_KEYS = new Set(['$schema', 'Name', 'Description', '_meta', '__meta__']);

function toSnake(name: string): string {
  return name.replace(/([A-Z])/g, '_$1').toLowerCase().replace(/^_/, '');
}

function viewFor(table: string): string {
  return `vw_${toSnake(table)}`;
}

export async function exportXlsx(res: Response): Promise<void> {
  const rulebook = JSON.parse(fs.readFileSync(RULEBOOK_PATH, 'utf8')) as Record<string, {
    schema: Array<{ name: string }>;
    data: unknown[];
  }>;
  const tableNames = Object.keys(rulebook).filter(
    (k) => !RESERVED_KEYS.has(k) && Array.isArray(rulebook[k]?.schema),
  );

  for (const table of tableNames) {
    const viewName = viewFor(table);
    const rows = await query<Record<string, unknown>>(`SELECT * FROM ${viewName}`);
    const schema = rulebook[table].schema;
    rulebook[table].data = rows.map((row) => {
      const mapped: Record<string, unknown> = {};
      for (const field of schema) {
        const snakeKey = toSnake(field.name);
        if (snakeKey in row) mapped[field.name] = row[snakeKey];
      }
      return mapped;
    });
  }

  const exportPath = path.join(PROJECT_ROOT, 'rulebook-export.json');
  const xlsxFilename = `${PROJECT_NAME}-rulebook.xlsx`;
  const xlsxPath = path.join(PROJECT_ROOT, xlsxFilename);
  fs.writeFileSync(exportPath, JSON.stringify(rulebook, null, 2));

  await new Promise<void>((resolve, reject) => {
    const proc = spawn(
      'effortless',
      ['rulebook-to-xlsx', '-i', './rulebook-export.json', '-o', xlsxFilename],
      { cwd: PROJECT_ROOT },
    );
    let stderr = '';
    proc.stderr.on('data', (d) => { stderr += d.toString(); });
    proc.on('close', (code) =>
      code === 0 ? resolve() : reject(new Error(`rulebook-to-xlsx exited ${code}: ${stderr.trim()}`)),
    );
    proc.on('error', reject);
  });

  if (!fs.existsSync(xlsxPath)) {
    throw new Error(`rulebook-to-xlsx produced no workbook at ${xlsxPath}`);
  }

  res.download(xlsxPath, xlsxFilename, () => {
    try { fs.unlinkSync(xlsxPath); } catch { /* ignore */ }
    try { fs.unlinkSync(exportPath); } catch { /* ignore */ }
  });
}
