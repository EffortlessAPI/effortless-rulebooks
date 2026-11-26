// The live rows: the rulebook's stored rows as they are right now, with every
// edit the page or the admin has made since the last save. Every engine computes
// from these rows; Save writes them back to the rulebook file.
import { readHub, storedRows, writeHub } from './hub.mjs';

function coerce(field, value, table) {
  if (value === undefined) throw new Error(`${table}.${field.name}: no value given`);
  if (value === null || value === '') return null;
  switch (field.datatype) {
    case 'boolean':
      if (typeof value === 'boolean') return value;
      if (value === 'true' || value === 'false') return value === 'true';
      throw new Error(`${table}.${field.name} is true or false, not ${JSON.stringify(value)}`);
    case 'integer': {
      const n = Number(value);
      if (!Number.isInteger(n)) throw new Error(`${table}.${field.name} is a whole number, not ${JSON.stringify(value)}`);
      return n;
    }
    case 'number': case 'decimal': case 'float': {
      const n = Number(value);
      if (!Number.isFinite(n)) throw new Error(`${table}.${field.name} is a number, not ${JSON.stringify(value)}`);
      return n;
    }
    default:
      return String(value);
  }
}

export class Workspace {
  constructor(hubPath) {
    this.hubPath = hubPath;
    this.version = 0;
    this.tableVersion = {};
    this.reload();
  }

  reload() {
    this.hub = readHub(this.hubPath);
    this.tables = this.hub.tables;
    this.data = storedRows(this.hub);
    this.saved = storedRows(this.hub);
    this.index = {};
    for (const name of Object.keys(this.tables)) { this.reindex(name); this.touch(name); }
  }

  reindex(name) {
    const key = this.table(name).key;
    this.index[name] = new Map(this.data[name].map((row) => [String(row[key]), row]));
  }

  touch(name) { this.version++; this.tableVersion[name] = this.version; }

  table(name) {
    const t = this.tables[name];
    if (!t) throw new Error(`The rulebook has no table ${name}`);
    return t;
  }

  rows(name) { this.table(name); return this.data[name]; }
  get(name, key) { this.table(name); return this.index[name].get(String(key)) || null; }
  need(name, key) {
    const row = this.get(name, key);
    if (!row) throw new Error(`${name} has no row ${JSON.stringify(key)}`);
    return row;
  }

  checkLinks(name, values) {
    const t = this.table(name);
    for (const [fieldName, value] of Object.entries(values)) {
      const field = t.byName[fieldName];
      if (field.type === 'relationship' && value !== null && !this.get(field.relatedTo, value)) {
        throw new Error(`${name}.${fieldName}: ${field.relatedTo} has no row ${JSON.stringify(value)}`);
      }
    }
  }

  clean(name, values, { partial }) {
    const t = this.table(name);
    const out = {};
    for (const [fieldName, value] of Object.entries(values)) {
      const field = t.byName[fieldName];
      if (!field) throw new Error(`${name} has no field ${fieldName}`);
      if (!field.stored) throw new Error(`${name}.${fieldName} is computed (${field.type}); it cannot be written`);
      out[fieldName] = coerce(field, value, name);
    }
    if (!partial) for (const f of t.stored) if (!(f in out)) out[f] = null;
    return out;
  }

  patch(name, key, values) {
    const t = this.table(name);
    const row = this.need(name, key);
    const clean = this.clean(name, values, { partial: true });
    if (t.key in clean && String(clean[t.key]) !== String(key)) throw new Error(`${name}: a row's key (${t.key}) cannot be changed; delete the row and add another`);
    this.checkLinks(name, clean);
    Object.assign(row, clean);
    this.touch(name);
    return row;
  }

  insert(name, values) {
    const t = this.table(name);
    const clean = this.clean(name, values, { partial: false });
    const key = clean[t.key];
    if (key === null) throw new Error(`${name}: a new row needs a value in its key field ${t.key}`);
    if (this.get(name, key)) throw new Error(`${name} already has a row ${JSON.stringify(key)}`);
    this.checkLinks(name, clean);
    this.data[name].push(clean);
    this.index[name].set(String(key), clean);
    this.touch(name);
    return clean;
  }

  // Many rows at once, links checked after all of them are in (rows may point at each other).
  insertMany(name, rowsToAdd) {
    const t = this.table(name);
    const cleaned = rowsToAdd.map((values) => this.clean(name, values, { partial: false }));
    for (const row of cleaned) {
      const key = row[t.key];
      if (key === null) throw new Error(`${name}: a new row needs a value in its key field ${t.key}`);
      if (this.get(name, key)) throw new Error(`${name} already has a row ${JSON.stringify(key)}`);
      this.data[name].push(row);
      this.index[name].set(String(key), row);
    }
    if (cleaned.length) this.touch(name);
    return cleaned.length;
  }

  remove(name, key) {
    this.need(name, key);
    this.data[name] = this.data[name].filter((row) => String(row[this.table(name).key]) !== String(key));
    this.index[name].delete(String(key));
    this.touch(name);
  }

  removeMany(name, keys) {
    if (!keys.length) return 0;
    const gone = new Set(keys.map(String));
    const key = this.table(name).key;
    this.data[name] = this.data[name].filter((row) => !gone.has(String(row[key])));
    for (const k of gone) this.index[name].delete(k);
    this.touch(name);
    return gone.size;
  }

  replaceAll(rows) {
    for (const name of Object.keys(this.tables)) {
      if (!Array.isArray(rows[name])) throw new Error(`No rows given for table ${name}`);
      this.data[name] = rows[name].map((row) => this.clean(name, Object.fromEntries(this.table(name).stored.map((f) => [f, row[f] ?? null])), { partial: false }));
      this.reindex(name);
      this.touch(name);
    }
  }

  // A write that fails half way is undone: take a snapshot before, restore it on error.
  snapshot() { return structuredClone(this.data); }
  restore(snapshot) {
    this.data = snapshot;
    for (const name of Object.keys(this.tables)) { this.reindex(name); this.touch(name); }
  }

  // What differs from the rulebook file: rows added, removed, and fields changed.
  changes() {
    const out = { total: 0, tables: {} };
    for (const [name, t] of Object.entries(this.tables)) {
      const before = new Map(this.saved[name].map((row) => [String(row[t.key]), row]));
      const added = [], changed = [];
      for (const row of this.data[name]) {
        const key = String(row[t.key]);
        const old = before.get(key);
        if (!old) { added.push(key); continue; }
        before.delete(key);
        const fields = {};
        for (const f of t.stored) if ((old[f] ?? null) !== (row[f] ?? null)) fields[f] = [old[f] ?? null, row[f] ?? null];
        if (Object.keys(fields).length) changed.push({ key, fields });
      }
      const removed = [...before.keys()];
      const count = added.length + changed.length + removed.length;
      if (count) { out.tables[name] = { added, changed, removed }; out.total += count; }
    }
    return out;
  }

  hubChangedOnDisk() { return readHub(this.hubPath).hash !== this.hub.hash; }

  save() {
    const changed = this.changes().total;
    this.hub = writeHub(this.hub, this.data);
    this.tables = this.hub.tables;
    this.saved = storedRows(this.hub);
    return changed;
  }
}
