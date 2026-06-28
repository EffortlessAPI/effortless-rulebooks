// L9 — ROUTING. Two surfaces, asserted under ONE describe (one server lifecycle
// per file, like the other harness files):
//  (1) per-entity RelativePath: a computed field assembling each entity's own
//      page path, chaining its parent's path (case → prediction → mechanism …).
//  (2) role-based nav trees: RoleVisibility filters /api/routing/tree so each
//      role sees its own workflow; admin sees all trees.

import { test, before, after, describe } from 'node:test';
import assert from 'node:assert/strict';
import { ROUTING } from '../oracle/dag-oracle.js';
import { CONTRACT } from './contract.js';
import { startServer, stopServer, assertAppSurfaces, getJson } from './server-fixture.js';

describe('L9 · ROUTING · RelativePath + role-based nav trees', () => {
  before(startServer);
  after(stopServer);

  // (1) per-entity RelativePath
  for (const e of ROUTING.relativePaths) {
    test(`RelativePath · ${e.endpoint} · ${e.field} = ${e.expected}`, async () => {
      await assertAppSurfaces(e.endpoint, e.field, e.expected,
        'RelativePath chains the parent entity\'s path; every entity is linkable by its own id.');
    });
  }

  // (2) role-based nav trees
  for (const [role, expectedTops] of Object.entries(ROUTING.navTrees)) {
    test(`nav tree · role ${role} sees top-level routes: ${expectedTops.join(', ')}`, async () => {
      const { status, body } = await getJson(CONTRACT.routingTree.endpoint(role));
      assert.equal(status, 200, `nav tree endpoint not surfaced (HTTP ${status})`);
      const tops = (body?.tree || []).map((n) => n.route_key).sort();
      assert.deepEqual(tops, [...expectedTops].sort(),
        `role ${role}: expected top-level routes ${JSON.stringify([...expectedTops].sort())}, got ${JSON.stringify(tops)}`);
    });
  }
});
