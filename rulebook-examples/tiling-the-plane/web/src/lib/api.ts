export type Row = Record<string, any>;

export type ColumnMeta = { name: string; type: string; editable: boolean };
export type EntityMeta = {
  key: string;
  view: string;
  pk: string;
  editable: string[];
  required: string[];
  columns: ColumnMeta[];
};

async function j<T>(r: Response): Promise<T> {
  if (!r.ok) {
    let msg = `${r.status} ${r.statusText}`;
    try {
      const body = await r.json();
      if (body?.error) msg = body.error;
    } catch {}
    throw new Error(msg);
  }
  return r.json() as Promise<T>;
}

export const api = {
  entities: () => fetch("/api/entities").then(j<Record<string, EntityMeta>>),
  list: (entity: string) => fetch(`/api/${entity}`).then(j<Row[]>),
  get: (entity: string, id: string) => fetch(`/api/${entity}/${id}`).then(j<Row>),
  create: (entity: string, body: Row) =>
    fetch(`/api/${entity}`, {
      method: "POST",
      headers: { "content-type": "application/json" },
      body: JSON.stringify(body),
    }).then(j<Row>),
  patch: (entity: string, id: string, body: Row) =>
    fetch(`/api/${entity}/${id}`, {
      method: "PATCH",
      headers: { "content-type": "application/json" },
      body: JSON.stringify(body),
    }).then(j<Row>),
  remove: (entity: string, id: string) =>
    fetch(`/api/${entity}/${id}`, { method: "DELETE" }).then(j<{ ok: boolean }>),
};
