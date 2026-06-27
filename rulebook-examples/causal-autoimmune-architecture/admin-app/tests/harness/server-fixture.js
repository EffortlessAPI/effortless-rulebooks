// server-fixture.js — boots the admin app on an ephemeral port for the suite,
// and provides a getJson helper + an assertion that the app SURFACES a value.
//
// The harness deliberately exercises the REAL Express app (imported from
// ../../server/index.js), so a green test proves the *app* serves the inference,
// not just that Postgres computed it.

import { app } from '../../server/index.js';
import { closePool } from '../../server/db.js';
import assert from 'node:assert/strict';
import { TOL } from '../oracle/dag-oracle.js';

let server;
let baseUrl;

export async function startServer() {
  if (server) return baseUrl;
  await new Promise((resolve) => {
    server = app.listen(0, () => {
      const { port } = server.address();
      baseUrl = `http://localhost:${port}`;
      resolve();
    });
  });
  return baseUrl;
}

export async function stopServer() {
  if (server) await new Promise((r) => server.close(r));
  server = null;
  baseUrl = null;
  await closePool();
}

// Fetch JSON from the app. Returns { status, body }.
export async function getJson(path, options) {
  const res = await fetch(`${baseUrl}${path}`, options);
  let body = null;
  try {
    body = await res.json();
  } catch {
    body = null;
  }
  return { status: res.status, body };
}

// The core load-bearing assertion. Asks the app for `path`, and requires the
// app to surface `field` === expected. A 501 (or any non-200, or a missing
// field) FAILS with the witness in the message — that is the RED state today.
export async function assertAppSurfaces(path, field, expected, witness) {
  const { status, body } = await getJson(path);
  const ctx = `\n   GET ${path}\n   field: ${field}\n   witness: ${witness}`;

  assert.equal(
    status,
    200,
    `App does not yet surface this node (HTTP ${status}). Wire the endpoint to read vw_*.${ctx}`,
  );
  assert.ok(
    body && Object.prototype.hasOwnProperty.call(body, field),
    `App responded 200 but is missing field "${field}".${ctx}`,
  );

  const actual = body[field];
  if (typeof expected === 'number' && !Number.isInteger(expected)) {
    assert.ok(
      Math.abs(Number(actual) - expected) <= TOL,
      `Expected ${field} ≈ ${expected} (±${TOL}) but app served ${actual}.${ctx}`,
    );
  } else {
    assert.deepEqual(
      actual,
      expected,
      `Expected ${field} = ${JSON.stringify(expected)} but app served ${JSON.stringify(actual)}.${ctx}`,
    );
  }
}
