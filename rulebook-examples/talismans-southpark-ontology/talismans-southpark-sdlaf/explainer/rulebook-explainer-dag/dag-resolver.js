/**
 * DAG resolver — reads window.__EFFORTLESS_EXPLAINER__.rulebook (baked at transpile time).
 * Vanilla-JS port of the React explainer-dag/lib (dagResolver.ts + formula.ts +
 * renderEnglish.ts + renderJsx.tsx + rulespeak.ts accessors). Pure data + render
 * helpers; no DOM. explainer-dag.js consumes this to build the UI.
 */
(function (global) {
  function getBundle() {
    return global.__EFFORTLESS_EXPLAINER__ || {};
  }

  function rulebook() {
    return getBundle().rulebook || {};
  }

  // A table is a top-level entry that holds a schema, whatever letter its name starts with
  // (an ontology's concept rulebook names a property's table for the property: isParentOf).
  // "$schema" and the entries that start with "_" (_meta, __meta__) are the platform's own.
  function tablesOf(rb) {
    return Object.keys(rb).filter((k) => {
      if (k.startsWith("$") || k.startsWith("_")) return false;
      const v = rb[k];
      return v && typeof v === "object" && "schema" in v;
    });
  }

  function schemaOf(rb, table) {
    const t = rb[table];
    if (!t || typeof t !== "object" || !("schema" in t)) return [];
    return t.schema || [];
  }

  function fieldOf(rb, table, field) {
    return schemaOf(rb, table).find((f) => f.name === field) || null;
  }

  function relationTarget(rb, f) {
    function candidate(v) {
      if (typeof v !== "string") return null;
      const name = v.trim();
      if (!name || /[^A-Za-z0-9_]/.test(name)) return null;
      const t = rb[name];
      return t && typeof t === "object" && "schema" in t ? name : null;
    }
    return (f.RelatedTo && f.RelatedTo.trim()) || candidate(f.formula) || candidate(f.name);
  }

  function toNode(rb, table, field) {
    const f = fieldOf(rb, table, field);
    if (!f) return null;
    return {
      table,
      field: f.name,
      datatype: f.datatype || "string",
      type: f.type || "raw",
      nullable: f.nullable ?? true,
      formula: f.formula || null,
      description: f.Description || "",
      relatedTo: f.type === "relationship" ? relationTarget(rb, f) : f.RelatedTo || null,
    };
  }

  function extractFormulaRefs(formula, sameTable) {
    if (!formula) return [];
    const refs = [];
    const re = /(?:([A-Za-z_][A-Za-z0-9_]*)\s*!\s*)?\{\{\s*([A-Za-z_][A-Za-z0-9_.]*?)\s*\}\}/g;
    let m;
    while ((m = re.exec(formula)) !== null) {
      let table = (m[1] || sameTable).trim();
      let field = m[2].trim();
      const dot = field.indexOf(".");
      if (dot > 0) { table = field.slice(0, dot).trim(); field = field.slice(dot + 1).trim(); }
      if (!field) continue;
      refs.push({ table, field });
    }
    const seen = new Set();
    return refs.filter((r) => {
      const k = `${r.table}.${r.field}`;
      if (seen.has(k)) return false;
      seen.add(k);
      return true;
    });
  }

  let _index = null;

  function getIndex(rb) {
    if (_index) return _index;
    const tables = tablesOf(rb);
    const upstream = new Map();
    const downstream = new Map();
    const all = new Set();

    for (const t of tables) {
      for (const f of schemaOf(rb, t)) {
        const key = `${t}.${f.name}`;
        all.add(key);
        upstream.set(key, f.formula ? extractFormulaRefs(f.formula, t) : []);
      }
    }
    for (const [child, parents] of upstream) {
      const [ct, cf] = child.split(".");
      for (const p of parents) {
        const pk = `${p.table}.${p.field}`;
        const list = downstream.get(pk) || [];
        list.push({ table: ct, field: cf });
        downstream.set(pk, list);
      }
    }
    _index = { upstream, downstream, all };
    return _index;
  }

  function computeDepth(rb, index, table, field, seen) {
    seen = seen || new Set();
    const key = `${table}.${field}`;
    if (seen.has(key)) return 0;
    seen.add(key);
    const f = fieldOf(rb, table, field);
    if (!f) return 0;
    if (f.type === "raw" || f.type === "relationship" || !f.formula) return 0;
    const parents = index.upstream.get(key) || [];
    if (parents.length === 0) return 0;
    let max = 0;
    for (const p of parents) {
      const d = 1 + computeDepth(rb, index, p.table, p.field, new Set(seen));
      if (d > max) max = d;
    }
    return max;
  }

  function collectLeaves(rb, index, table, field, seen, acc, isRoot) {
    seen = seen || new Set();
    acc = acc || new Map();
    isRoot = isRoot !== false;
    const key = `${table}.${field}`;
    if (seen.has(key)) return [...acc.values()];
    seen.add(key);
    const node = toNode(rb, table, field);
    if (!node) return [...acc.values()];
    if (!isRoot && (node.type === "raw" || node.type === "relationship")) {
      if (!acc.has(key)) acc.set(key, node);
      return [...acc.values()];
    }
    const parents = index.upstream.get(key) || [];
    if (!isRoot && parents.length === 0 && !node.formula) {
      if (!acc.has(key)) acc.set(key, node);
      return [...acc.values()];
    }
    for (const p of parents) collectLeaves(rb, index, p.table, p.field, seen, acc, false);
    return [...acc.values()];
  }

  function collectTransitiveDownstream(index, table, field) {
    const out = new Map();
    const queue = [{ table, field }];
    const seen = new Set([`${table}.${field}`]);
    while (queue.length) {
      const cur = queue.shift();
      const children = index.downstream.get(`${cur.table}.${cur.field}`) || [];
      for (const c of children) {
        const k = `${c.table}.${c.field}`;
        if (seen.has(k)) continue;
        seen.add(k);
        out.set(k, c);
        queue.push(c);
      }
    }
    return [...out.values()];
  }

  function resolveDag(table, field) {
    const rb = rulebook();
    const index = getIndex(rb);
    const node = toNode(rb, table, field);
    if (!node) return null;
    const key = `${table}.${field}`;
    const upRefs = index.upstream.get(key) || [];
    const downRefs = index.downstream.get(key) || [];
    const upstream = upRefs.map((r) => toNode(rb, r.table, r.field)).filter(Boolean);
    const downstream = downRefs.map((r) => toNode(rb, r.table, r.field)).filter(Boolean);
    const leaves =
      node.type === "raw" || node.type === "relationship" ? [] : collectLeaves(rb, index, table, field);
    const consumerTransitive = collectTransitiveDownstream(index, table, field);
    const depth = computeDepth(rb, index, table, field);
    return Object.assign({}, node, {
      depth,
      fanIn: upstream.length,
      fanOut: downstream.length,
      upstream,
      downstream,
      leaves,
      consumerTransitive,
    });
  }

  function listTables() {
    const rb = rulebook();
    return tablesOf(rb).map((t) => {
      const fields = schemaOf(rb, t);
      const derived = fields.filter(
        (f) => f.type === "calculated" || f.type === "lookup" || f.type === "aggregation",
      ).length;
      const rels = fields.filter((f) => f.type === "relationship").length;
      const tv = rb[t];
      return {
        table: t,
        description: (tv && tv.Description) || "",
        fieldCount: fields.length,
        derivedCount: derived,
        relationshipCount: rels,
      };
    });
  }

  function tableColumns(table) {
    const rb = rulebook();
    return schemaOf(rb, table)
      .map((f) => toNode(rb, table, f.name))
      .filter(Boolean);
  }

  function tableDescription(table) {
    const tv = rulebook()[table];
    return (tv && tv.Description) || "";
  }

  function tableExists(table) {
    return tablesOf(rulebook()).includes(table);
  }

  // ── Type metadata (mirrors React TypeBadge.tsx) ─────────────────────────────
  const TYPE_META = {
    raw:          { glyph: "●", label: "Ground truth", tone: "raw" },
    calculated:   { glyph: "ƒ", label: "Calculated",   tone: "calc" },
    aggregation:  { glyph: "Σ", label: "Aggregation",  tone: "agg" },
    relationship: { glyph: "⇢", label: "Relationship", tone: "rel" },
    lookup:       { glyph: "↗", label: "Lookup",       tone: "lookup" },
  };
  function typeGlyph(type) { return (TYPE_META[type] || TYPE_META.calculated).glyph; }
  function typeLabel(type) { return (TYPE_META[type] || TYPE_META.calculated).label; }
  function typeTone(type)  { return (TYPE_META[type] || TYPE_META.calculated).tone; }

  function isDerivedField(table, field) {
    const f = fieldOf(rulebook(), table, field);
    if (!f) return false;
    return f.type === "calculated" || f.type === "lookup" || f.type === "aggregation";
  }

  // "CumulativeBalance" → "cumulative balance" (mirrors renderEnglish.humanizeField)
  function humanizeField(field) {
    return String(field || "")
      .replace(/([a-z0-9])([A-Z])/g, "$1 $2")
      .replace(/([A-Z]+)([A-Z][a-z])/g, "$1 $2")
      .replace(/_/g, " ")
      .toLowerCase();
  }

  // ── RuleSpeak® accessors (mirrors lib/rulespeak.ts) ──────────────────────────
  // RuleSpeak® is baked once per language (rulespeakByLang). A host may offer a
  // language the build did not bake, so fall back to English rather than blank.
  function ruleSpeakSlice(lang) {
    const byLang = getBundle().rulespeakByLang || {};
    return byLang[lang] || byLang.en || {};
  }

  function ruleSpeakForField(table, field, lang) {
    return (ruleSpeakSlice(lang).fields || {})[`${table}.${field}`] || null;
  }

  // The formula read as a plain sentence in `lang`, by the engine, where the build
  // baked one (every language but English). Only that language's: no fallback here,
  // the caller reads the formula itself when there is none.
  function plainReadingForField(table, field, lang) {
    const byLang = getBundle().rulespeakByLang || {};
    const f = ((byLang[lang] || {}).fields || {})[`${table}.${field}`];
    return (f && f.plain) || null;
  }

  // The language pack's rule-tree connector words ({ allOf, anyOf, if, otherwise }),
  // and how a sentence ends in it (sentenceEnd).
  function ruleSpeakConnectors(lang) {
    return ruleSpeakSlice(lang).connectors || {};
  }

  // What ONE row of a table is called ("case" for Cases): the noun the rule sentences
  // themselves use, baked per language. Only that language's word is returned, never
  // another's: the sentence it goes into is the caller's, in a language of its own.
  function ruleSpeakNoun(table, lang) {
    const byLang = getBundle().rulespeakByLang || {};
    return ((byLang[lang] || {}).nouns || {})[table] || null;
  }

  function obligationsForEntity(table, lang) {
    const obl = ruleSpeakSlice(lang).obligations || [];
    if (!obl.length) return [];
    const norm = (s) => String(s || "").replace(/[^a-z0-9]/gi, "").toLowerCase();
    const singular = (s) => String(s || "").replace(/ies$/i, "y").replace(/s$/i, "");
    const target = norm(singular(table));
    return obl.filter((o) => norm(singular(o.entity)) === target);
  }

  function obligationsForField(table, field, lang) {
    return obligationsForEntity(table, lang).filter((o) => o.predicate === field);
  }

  // ════════════════════════════════════════════════════════════════════════
  //  Formula parser — port of lib/formula.ts. Produces an AST that renders to
  //  HTML (clickable chips) or a plain-English sentence.
  // ════════════════════════════════════════════════════════════════════════
  const OP_2CHAR = new Set(["<=", ">=", "<>"]);
  const OP_1CHAR = new Set(["+", "-", "*", "/", "&", "=", "<", ">"]);

  // ---------------------------------------------------------------------------
  // Multiple-choice forms: every common way of writing a multiple choice is
  // accepted in a rulebook formula, as equals:
  //
  //   IF(c1, v1, IF(c2, v2, d))                        nested IF
  //   IFS(c1, v1, c2, v2)                              Excel IFS
  //   SWITCH(x, p1, r1, p2, r2, d)                     Excel/Airtable SWITCH
  //   CHOOSE(i, v1, v2, v3)                            Excel CHOOSE (i counts from 1)
  //   CASE WHEN c1 THEN v1 WHEN c2 THEN v2 ELSE d END  SQL searched CASE
  //   CASE x WHEN p1 THEN r1 ELSE d END                SQL simple CASE
  //
  // Before a formula is parsed, normalizeMultiChoice rewrites its text so only
  // IF and SWITCH remain:
  //
  //   IFS(c1, v1, c2, v2)                   -> IF(c1, v1, IF(c2, v2))
  //   CASE WHEN c1 THEN v1 ... ELSE d END   -> IF(c1, v1, ... d)
  //   CASE x WHEN p1 THEN r1 ... ELSE d END -> SWITCH(x, p1, r1, ..., d)
  //   CHOOSE(i, v1, v2)                     -> SWITCH(i, 1, v1, 2, v2)
  //
  // Same semantics in every tool: the first condition (or pattern) that matches
  // wins, only the branch taken is evaluated, and a choice with no ELSE/default
  // that matches nothing is blank. Keywords are case-insensitive. A formula that
  // uses none of CASE / IFS / CHOOSE comes back unchanged, character for
  // character.
  //
  // Plain-JS form of rulebook-to-explainer-dag-react's lib/multiChoiceForms.ts, itself a
  // verbatim port of normalize_multi_choice in rulebook-to-python's
  // workload/python/orchestration/formula_parser.py (the reference); change the
  // reference and every copy together.
  // ---------------------------------------------------------------------------
  const normalizeMultiChoice = (function () {
    class MultiChoiceFormError extends Error {
    }
    const KEYWORDS = new Set(['CASE', 'IFS', 'CHOOSE']);
    const CASE_STOPS = new Set(['WHEN', 'THEN', 'ELSE', 'END']);
    function isAlpha(c) {
        return /[A-Za-z_]/.test(c) || (c.toLowerCase() !== c.toUpperCase());
    }
    function isAlnum(c) {
        return isAlpha(c) || /[0-9]/.test(c);
    }
    function tokenize(text) {
        const toks = [];
        let i = 0;
        const n = text.length;
        while (i < n) {
            const c = text[i];
            let j;
            if (c === '"' || c === "'") {
                j = i + 1;
                while (j < n && text[j] !== c)
                    j += text[j] === '\\' ? 2 : 1;
                j = Math.min(j + 1, n);
                toks.push({ kind: 'other', text: text.slice(i, j), pos: i });
                i = j;
                continue;
            }
            if (c === '{') {
                const close = text.startsWith('{{', i) ? '}}' : '}';
                j = text.indexOf(close, i);
                j = j === -1 ? n : j + close.length;
                toks.push({ kind: 'other', text: text.slice(i, j), pos: i });
                i = j;
                continue;
            }
            if (c === '[') {
                j = text.indexOf(']', i);
                j = j === -1 ? n : j + 1;
                toks.push({ kind: 'other', text: text.slice(i, j), pos: i });
                i = j;
                continue;
            }
            if (isAlpha(c)) {
                j = i;
                while (j < n && isAlnum(text[j]))
                    j++;
                toks.push({ kind: 'ident', text: text.slice(i, j), pos: i });
                i = j;
                continue;
            }
            if (c === '(' || c === ')' || c === ',') {
                toks.push({ kind: c, text: c, pos: i });
                i++;
                continue;
            }
            if (/\s/.test(c)) {
                j = i;
                while (j < n && /\s/.test(text[j]))
                    j++;
                toks.push({ kind: 'space', text: text.slice(i, j), pos: i });
                i = j;
                continue;
            }
            toks.push({ kind: 'other', text: c, pos: i });
            i++;
        }
        return toks;
    }
    function ifChain(pairs, dflt) {
        let out = dflt;
        for (let k = pairs.length - 1; k >= 0; k--) {
            const [cond, value] = pairs[k];
            out = out !== null ? `IF(${cond}, ${value}, ${out})` : `IF(${cond}, ${value})`;
        }
        return out;
    }
    class Rewriter {
        constructor(text, toks) {
            this.i = 0;
            this.text = text;
            this.toks = toks;
        }
        fail(message, pos) {
            throw new MultiChoiceFormError(`${message} at position ${pos} in formula: ${this.text}`);
        }
        peek() {
            let j = this.i;
            while (j < this.toks.length && this.toks[j].kind === 'space')
                j++;
            return j < this.toks.length ? this.toks[j] : null;
        }
        peekAfter() {
            let j = this.i + 1;
            while (j < this.toks.length && this.toks[j].kind === 'space')
                j++;
            return j < this.toks.length ? this.toks[j] : null;
        }
        skipSpace() {
            while (this.i < this.toks.length && this.toks[this.i].kind === 'space')
                this.i++;
        }
        keyword() {
            const t = this.peek();
            if (t && t.kind === 'ident' && CASE_STOPS.has(t.text.toUpperCase()))
                return t.text.toUpperCase();
            return null;
        }
        /** Rewrite tokens up to a top-level ',' or ')', the end, or (inside a CASE)
         *  one of WHEN / THEN / ELSE / END. */
        seq(inCase) {
            let out = '';
            while (this.i < this.toks.length) {
                const { kind, text: tok, pos } = this.toks[this.i];
                if (kind === ',' || kind === ')')
                    break;
                if (kind === 'ident') {
                    const upper = tok.toUpperCase();
                    if (inCase && CASE_STOPS.has(upper))
                        break;
                    const next = this.peekAfter();
                    const callFollows = !!next && next.kind === '(';
                    if (upper === 'CASE' && !callFollows) {
                        this.i++;
                        out += this.caseExpr(pos);
                        continue;
                    }
                    if (callFollows) {
                        this.i++;
                        this.skipSpace();
                        const args = this.args();
                        out += this.call(tok, upper, args, pos);
                        continue;
                    }
                }
                if (kind === '(') {
                    out += '(' + this.args().join(',') + ')';
                    continue;
                }
                out += tok;
                this.i++;
            }
            return out;
        }
        /** At '(' : rewrite each argument up to the matching ')', consuming both. */
        args() {
            const openPos = this.toks[this.i].pos;
            this.i++;
            const args = [this.seq(false)];
            while (this.i < this.toks.length && this.toks[this.i].kind === ',') {
                this.i++;
                args.push(this.seq(false));
            }
            if (this.i >= this.toks.length)
                this.fail("Unclosed '('", openPos);
            this.i++; // ')'
            if (args.length === 1 && !args[0].trim())
                return [];
            return args;
        }
        call(name, upper, args, pos) {
            if (upper === 'IFS') {
                const parts = args.map((a) => a.trim());
                if (parts.length < 2 || parts.length % 2 || parts.some((p) => !p)) {
                    this.fail(`IFS takes condition/value pairs, got ${parts.length} argument(s)`, pos);
                }
                const pairs = [];
                for (let k = 0; k < parts.length; k += 2)
                    pairs.push([parts[k], parts[k + 1]]);
                return ifChain(pairs, null);
            }
            if (upper === 'CHOOSE') {
                const parts = args.map((a) => a.trim());
                if (parts.length < 2 || parts.some((p) => !p)) {
                    this.fail(`CHOOSE takes an index and at least one value, got ${parts.length} argument(s)`, pos);
                }
                const flat = parts.slice(1).map((v, k) => `${k + 1}, ${v}`).join(', ');
                return `SWITCH(${parts[0]}, ${flat})`;
            }
            return name + '(' + args.join(',') + ')';
        }
        caseExpr(pos) {
            let subject = null;
            if (this.keyword() !== 'WHEN') {
                subject = this.seq(true).trim();
                if (!subject || this.keyword() !== 'WHEN')
                    this.fail('CASE needs WHEN', pos);
            }
            const pairs = [];
            let dflt = null;
            for (;;) {
                const kw = this.keyword();
                if (kw === 'WHEN') {
                    this.skipSpace();
                    const whenPos = this.toks[this.i].pos;
                    this.i++;
                    const cond = this.seq(true).trim();
                    if (this.keyword() !== 'THEN' || !cond)
                        this.fail('WHEN needs a condition and THEN', whenPos);
                    this.skipSpace();
                    this.i++;
                    const value = this.seq(true).trim();
                    if (!value)
                        this.fail('THEN needs a value', whenPos);
                    pairs.push([cond, value]);
                    continue;
                }
                if (kw === 'ELSE' && pairs.length && dflt === null) {
                    this.skipSpace();
                    const elsePos = this.toks[this.i].pos;
                    this.i++;
                    dflt = this.seq(true).trim();
                    if (!dflt || this.keyword() !== 'END')
                        this.fail('ELSE needs a value and then END', elsePos);
                    continue;
                }
                if (kw === 'END' && pairs.length) {
                    this.skipSpace();
                    this.i++;
                    break;
                }
                if (this.peek() === null)
                    this.fail('CASE has no END', pos);
                this.fail('CASE needs WHEN ... THEN ... and END', pos);
            }
            if (subject === null)
                return ifChain(pairs, dflt);
            const flat = pairs.map(([p, r]) => `${p}, ${r}`).join(', ');
            return `SWITCH(${subject}, ${flat}` + (dflt !== null ? `, ${dflt})` : ')');
        }
    }
    /** Rewrite CASE / IFS / CHOOSE into IF and SWITCH. Returns the text unchanged
     *  when it uses none of them. */
    function normalizeMultiChoice(formula) {
        if (!formula)
            return formula;
        const toks = tokenize(formula);
        if (!toks.some((t) => t.kind === 'ident' && KEYWORDS.has(t.text.toUpperCase())))
            return formula;
        const rw = new Rewriter(formula, toks);
        let out = rw.seq(false);
        while (rw.i < toks.length) {
            // A stray ',' or ')' at the top level: keep it, so the parser reports it.
            out += toks[rw.i].text;
            rw.i++;
            out += rw.seq(false);
        }
        return out;
    }
    return normalizeMultiChoice;
  })();

  function tokenize(src) {
    // CASE (searched and simple), IFS and CHOOSE read as the IF and SWITCH they mean.
    let s = String(normalizeMultiChoice(String(src || "")) || "").trim();
    if (s.startsWith("=")) s = s.slice(1);
    const toks = [];
    let i = 0;
    while (i < s.length) {
      const c = s[i];
      if (/\s/.test(c)) { i++; continue; }
      if (c === "{" && s[i + 1] === "{") {
        const end = s.indexOf("}}", i + 2);
        if (end < 0) throw new Error("unterminated {{...}}");
        const inner = s.slice(i + 2, end).trim();
        if (!/^[A-Za-z_][A-Za-z0-9_]*$/.test(inner)) throw new Error("bad ref name");
        toks.push({ kind: "ref", value: inner, table: null, field: inner });
        i = end + 2; continue;
      }
      if (c === '"') {
        let j = i + 1, out = "";
        while (j < s.length && s[j] !== '"') {
          if (s[j] === "\\" && j + 1 < s.length) { out += s[j + 1]; j += 2; continue; }
          out += s[j]; j++;
        }
        if (j >= s.length) throw new Error("unterminated string");
        toks.push({ kind: "str", value: out });
        i = j + 1; continue;
      }
      if (/[0-9]/.test(c) || (c === "." && /[0-9]/.test(s[i + 1] || ""))) {
        let j = i;
        while (j < s.length && /[0-9.]/.test(s[j])) j++;
        toks.push({ kind: "num", value: s.slice(i, j) });
        i = j; continue;
      }
      const two = s.slice(i, i + 2);
      if (OP_2CHAR.has(two)) { toks.push({ kind: "op", value: two }); i += 2; continue; }
      if (OP_1CHAR.has(c)) { toks.push({ kind: "op", value: c }); i++; continue; }
      if (c === "(") { toks.push({ kind: "lparen", value: "(" }); i++; continue; }
      if (c === ")") { toks.push({ kind: "rparen", value: ")" }); i++; continue; }
      if (c === ",") { toks.push({ kind: "comma", value: "," }); i++; continue; }
      if (/[A-Za-z_]/.test(c)) {
        let j = i;
        while (j < s.length && /[A-Za-z0-9_]/.test(s[j])) j++;
        const name = s.slice(i, j);
        let k = j;
        while (k < s.length && /\s/.test(s[k])) k++;
        if (s[k] === "!") {
          let m = k + 1;
          while (m < s.length && /\s/.test(s[m])) m++;
          if (s[m] === "{" && s[m + 1] === "{") {
            const end = s.indexOf("}}", m + 2);
            if (end < 0) throw new Error("unterminated {{...}} after Table!");
            const inner = s.slice(m + 2, end).trim();
            if (!/^[A-Za-z_][A-Za-z0-9_]*$/.test(inner)) throw new Error("bad cross-table ref");
            toks.push({ kind: "ref", value: `${name}!${inner}`, table: name, field: inner });
            i = end + 2; continue;
          }
        }
        toks.push({ kind: "ident", value: name });
        i = j; continue;
      }
      throw new Error(`unrecognized character "${c}"`);
    }
    return toks;
  }

  function makeCursor(toks) { return { toks, pos: 0 }; }
  function peek(c) { return c.toks[c.pos]; }
  function eat(c) { return c.toks[c.pos++]; }
  function expect(c, kind) {
    const t = peek(c);
    if (!t || t.kind !== kind) throw new Error(`expected ${kind}`);
    return eat(c);
  }

  function parsePrimary(c) {
    const t = peek(c);
    if (!t) throw new Error("unexpected EOF");
    if (t.kind === "op" && t.value === "-") { eat(c); return { kind: "unary", op: "-", arg: parsePrimary(c) }; }
    if (t.kind === "ref") { eat(c); return { kind: "ref", table: t.table || null, field: t.field }; }
    if (t.kind === "num") { eat(c); return { kind: "num", value: Number(t.value) }; }
    if (t.kind === "str") { eat(c); return { kind: "str", value: t.value }; }
    if (t.kind === "lparen") { eat(c); const e = parseExpr(c); expect(c, "rparen"); return e; }
    if (t.kind === "ident") {
      eat(c);
      const name = t.value;
      if (name === "TRUE" || name === "FALSE") {
        if (peek(c) && peek(c).kind === "lparen") { eat(c); expect(c, "rparen"); }
        return { kind: "bool", value: name === "TRUE" };
      }
      if (peek(c) && peek(c).kind === "lparen") {
        eat(c);
        const args = [];
        if (peek(c) && peek(c).kind !== "rparen") {
          args.push(parseExpr(c));
          while (peek(c) && peek(c).kind === "comma") { eat(c); args.push(parseExpr(c)); }
        }
        expect(c, "rparen");
        return { kind: "call", fn: name.toUpperCase(), args };
      }
      return { kind: "call", fn: name.toUpperCase(), args: [] };
    }
    throw new Error(`unexpected token ${t.kind}`);
  }
  function parseMulDiv(c) {
    let left = parsePrimary(c);
    while (peek(c) && peek(c).kind === "op" && (peek(c).value === "*" || peek(c).value === "/")) {
      const op = eat(c).value; left = { kind: "binop", op, left, right: parsePrimary(c) };
    }
    return left;
  }
  function parseConcat(c) {
    let left = parseMulDiv(c);
    while (peek(c) && peek(c).kind === "op" && peek(c).value === "&") {
      eat(c); left = { kind: "binop", op: "&", left, right: parseMulDiv(c) };
    }
    return left;
  }
  function parseAddSub(c) {
    let left = parseConcat(c);
    while (peek(c) && peek(c).kind === "op" && (peek(c).value === "+" || peek(c).value === "-")) {
      const op = eat(c).value; left = { kind: "binop", op, left, right: parseConcat(c) };
    }
    return left;
  }
  function parseExpr(c) {
    let left = parseAddSub(c);
    while (peek(c) && peek(c).kind === "op" && ["=", "<>", "<", ">", "<=", ">="].includes(peek(c).value)) {
      const op = eat(c).value; left = { kind: "binop", op, left, right: parseAddSub(c) };
    }
    return left;
  }
  function tryParseFormula(formula) {
    try {
      const c = makeCursor(tokenize(formula));
      const node = parseExpr(c);
      if (c.pos < c.toks.length) return null;
      return node;
    } catch { return null; }
  }

  // ── AST → English sentence (port of renderEnglish.ts) ───────────────────────
  function fmtNum(n) {
    if (Number.isInteger(n) && Math.abs(n) >= 1000) return n.toLocaleString("en-US");
    return String(n);
  }
  function singularize(term) {
    if (!term) return term;
    if (/ies$/i.test(term)) return term.replace(/ies$/i, "y");
    if (/(ses|xes|zes)$/i.test(term)) return term.slice(0, -2);
    if (/s$/i.test(term) && !/ss$/i.test(term)) return term.slice(0, -1);
    return term;
  }
  function joinList(items, conj) {
    if (items.length === 0) return "";
    if (items.length === 1) return items[0];
    if (items.length === 2) return `${items[0]} ${conj} ${items[1]}`;
    return items.slice(0, -1).join(", ") + `, ${conj} ${items[items.length - 1]}`;
  }
  function isBool(n, v) { return n && n.kind === "bool" && n.value === v; }
  function unquote(s) { return s.replace(/^"(.*)"$/, "$1"); }

  function renderEnglish(node) {
    const refLabel = (_t, f) => humanizeField(f);
    function r(n, top) {
      switch (n.kind) {
        case "ref": return refLabel(n.table, n.field);
        case "num": return fmtNum(n.value);
        case "str": return `"${n.value}"`;
        case "bool": return n.value ? "yes" : "no";
        case "unary": return `negative ${r(n.arg, false)}`;
        case "binop": return binop(n.op, n.left, n.right, top);
        case "call": return call(n, top);
      }
      return "";
    }
    function paren(s, top) { return top ? s : `(${s})`; }
    function binop(op, left, right, top) {
      const l = r(left, false), rr = r(right, false);
      switch (op) {
        case "+": return paren(`${l} plus ${rr}`, top);
        case "-": return paren(`${l} minus ${rr}`, top);
        case "*": return paren(`${l} times ${rr}`, top);
        case "/": return paren(`${l} divided by ${rr}`, top);
        case "&": return paren(`${l} joined with ${rr}`, top);
        case "=": return right.kind === "str" ? `${l} is ${rr}` : `${l} equals ${rr}`;
        case "<>": return `${l} is not ${rr}`;
        case "<": return `${l} is less than ${rr}`;
        case ">": return `${l} is greater than ${rr}`;
        case "<=": return `${l} is at most ${rr}`;
        case ">=": return `${l} is at least ${rr}`;
      }
      return `${l} ${op} ${rr}`;
    }
    function condPairs(args, dropTable) {
      if (args.length === 0) return null;
      const pairs = [];
      for (let i = 0; i + 1 < args.length; i += 2) {
        const key = args[i], val = args[i + 1];
        const keyLabel = key.kind === "ref" && key.table && key.table === dropTable
          ? refLabel(null, key.field) : r(key, false);
        const valLabel = val.kind === "ref" && val.table === null && val.field === "Name"
          ? "this one" : r(val, false);
        pairs.push(`${keyLabel} is ${valLabel}`);
      }
      return pairs.length ? joinList(pairs, "and") : null;
    }
    function rollup(verb, target, pairs) {
      const targetLabel = r(target, false);
      const rollupTable = target.kind === "ref" ? target.table : null;
      const condBody = condPairs(pairs, rollupTable);
      if (rollupTable) {
        return condBody ? `${verb} ${targetLabel} across all ${rollupTable} whose ${condBody}`
          : `${verb} ${targetLabel} across all ${rollupTable}`;
      }
      return condBody ? `${verb} ${targetLabel} where ${condBody}` : `${verb} ${targetLabel}`;
    }
    function call(n, top) {
      const fn = n.fn, a = n.args;
      if (fn === "IF" && a.length >= 2) {
        const cond = a[0], t = a[1], f = a.length >= 3 ? a[2] : null;
        if (isBool(t, true) && (f == null || isBool(f, false))) return `true when ${r(cond, false)}`;
        if (isBool(t, false) && f != null && isBool(f, true)) return `true when not ${r(cond, false)}`;
        if (f == null) return `if ${r(cond, false)} then ${r(t, false)}`;
        return `if ${r(cond, false)} then ${r(t, false)}, otherwise ${r(f, false)}`;
      }
      if (fn === "SUMIFS" && a.length >= 1) return rollup("sum of", a[0], a.slice(1));
      if (fn === "COUNTIFS") {
        if (a.length === 0) return "count of matching records";
        const table = a[0].kind === "ref" ? a[0].table : null;
        const condBody = condPairs(a, table);
        if (table) return condBody ? `count of ${table} whose ${condBody}` : `count of ${table}`;
        return condBody ? `count of matching records where ${condBody}` : "count of matching records";
      }
      if (fn === "AVERAGEIFS" && a.length >= 1) return rollup("average", a[0], a.slice(1));
      if ((fn === "MINIFS" || fn === "MAXIFS") && a.length >= 1)
        return rollup(fn === "MINIFS" ? "smallest" : "largest", a[0], a.slice(1));
      if (fn === "INDEX" && a.length === 2 && a[0].kind === "ref" && a[1].kind === "call"
          && a[1].fn === "MATCH" && a[1].args.length >= 2) {
        const target = a[0], key = a[1].args[0];
        const fieldName = refLabel(target.table, target.field);
        if (key.kind === "ref") return `the ${fieldName} of the linked ${humanizeField(key.field)}`;
        const linked = target.table ? singularize(humanizeField(target.table)) : "linked record";
        return `the ${fieldName} of the linked ${linked}`;
      }
      if (fn === "AND" && a.length) return joinList(a.map((x) => r(x, false)), "and");
      if (fn === "OR" && a.length) return joinList(a.map((x) => r(x, false)), "or");
      if (fn === "NOT" && a.length === 1) return `not ${r(a[0], false)}`;
      if (fn === "XOR" && a.length === 2) return `exactly one of ${r(a[0], false)} or ${r(a[1], false)}`;
      if (fn === "LOWER" && a.length === 1) return `${r(a[0], false)} lowercased`;
      if (fn === "UPPER" && a.length === 1) return `${r(a[0], false)} uppercased`;
      if (fn === "TRIM" && a.length === 1) return `${r(a[0], false)} with surrounding whitespace removed`;
      if (fn === "LEN" && a.length === 1) return `the length of ${r(a[0], false)}`;
      if (fn === "CONCAT" || fn === "CONCATENATE") return joinList(a.map((x) => r(x, false)), "joined with");
      if (fn === "SUBSTITUTE" && a.length >= 3)
        return `${r(a[0], false)} with “${unquote(r(a[1], false))}” replaced by “${unquote(r(a[2], false))}”`;
      if (fn === "LEFT" && a.length >= 2) return `the first ${r(a[1], false)} characters of ${r(a[0], false)}`;
      if (fn === "RIGHT" && a.length >= 2) return `the last ${r(a[1], false)} characters of ${r(a[0], false)}`;
      if (fn === "MID" && a.length >= 3) return `${r(a[1], false)} characters of ${r(a[0], false)} starting at position ${r(a[2], false)}`;
      if (fn === "ABS" && a.length === 1) return `the absolute value of ${r(a[0], false)}`;
      if (fn === "ROUND" && a.length >= 1) {
        const digits = a[1] ? `to ${r(a[1], false)} decimal places` : "to the nearest whole number";
        return `${r(a[0], false)} rounded ${digits}`;
      }
      if (fn === "FLOOR" && a.length >= 1) return `${r(a[0], false)} rounded down`;
      if (fn === "CEILING" && a.length >= 1) return `${r(a[0], false)} rounded up`;
      if (fn === "MIN") return `the smallest of ${joinList(a.map((x) => r(x, false)), "and")}`;
      if (fn === "MAX") return `the largest of ${joinList(a.map((x) => r(x, false)), "and")}`;
      if (fn === "SUM") return `the sum of ${joinList(a.map((x) => r(x, false)), "and")}`;
      if (fn === "AVERAGE") return `the average of ${joinList(a.map((x) => r(x, false)), "and")}`;
      if (fn === "ISBLANK" && a.length === 1) return `${r(a[0], false)} is blank`;
      if (fn === "ISERROR" && a.length === 1) return `${r(a[0], false)} has an error`;
      if (fn === "IFERROR" && a.length >= 2) return `${r(a[0], false)}, or ${r(a[1], false)} if that has an error`;
      if (fn === "COALESCE") return `the first of ${joinList(a.map((x) => r(x, false)), "or")} that isn't blank`;
      if (fn === "TODAY" && a.length === 0) return "today";
      if (fn === "NOW" && a.length === 0) return "now";
      if (fn === "DATEDIFF" && a.length >= 2) {
        const unit = a[2] ? unquote(r(a[2], false)) : "days";
        return `the ${unit} between ${r(a[0], false)} and ${r(a[1], false)}`;
      }
      if (fn === "DATEADD" && a.length >= 3)
        return `${r(a[1], false)} ${unquote(r(a[2], false))} after ${r(a[0], false)}`;
      return `${fn.toLowerCase()}(${a.map((x) => r(x, false)).join(", ")})`;
    }
    const out = r(node, true).trim();
    return out ? out[0].toUpperCase() + out.slice(1) : out;
  }

  // Best-effort English for a node's formula; null if none / unparseable.
  function englishForFormula(formula) {
    if (!formula) return null;
    const ast = tryParseFormula(formula);
    if (!ast) return null;
    try { return renderEnglish(ast); } catch { return null; }
  }

  global.EffortlessDagResolver = {
    resolveDag,
    listTables,
    tableColumns,
    tableDescription,
    tableExists,
    typeGlyph,
    typeLabel,
    typeTone,
    isDerivedField,
    fieldOf,
    rulebook,
    getBundle,
    humanizeField,
    ruleSpeakForField,
    ruleSpeakConnectors,
    plainReadingForField,
    ruleSpeakNoun,
    obligationsForEntity,
    obligationsForField,
    tryParseFormula,
    extractFormulaRefs,
    renderEnglish,
    englishForFormula,
  };
})(typeof window !== "undefined" ? window : globalThis);
