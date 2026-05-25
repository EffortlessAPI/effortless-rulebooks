// Minimal paginated Airtable fetch helper.
// Returns all records (across pages) for the given table name.

const API_KEY = process.env.AIRTABLE_API_KEY;
const BASE_ID = process.env.AIRTABLE_BASE_ID || 'appeUOAaOIdoqPSx3';

export async function fetchAllRecords(tableName) {
  if (!API_KEY) throw new Error('AIRTABLE_API_KEY not set');
  const records = [];
  let offset;
  do {
    const url = new URL(`https://api.airtable.com/v0/${BASE_ID}/${encodeURIComponent(tableName)}`);
    url.searchParams.set('pageSize', '100');
    if (offset) url.searchParams.set('offset', offset);
    const res = await fetch(url, { headers: { Authorization: `Bearer ${API_KEY}` } });
    if (!res.ok) {
      const body = await res.text();
      throw new Error(`Airtable ${tableName} ${res.status}: ${body}`);
    }
    const data = await res.json();
    records.push(...data.records);
    offset = data.offset;
  } while (offset);
  return records;
}

export const baseId = BASE_ID;
