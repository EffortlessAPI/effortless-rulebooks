// Excel exporter — driven by the declarative schema.
//
// Three kinds of sheets:
//   1. "Rules"       — one row per field (raw + computed) with description,
//                      type, formula, Excel-formula template, SQL snippet.
//   2. "Constants"   — GRADE_WEIGHTS, GRADE_BANDS, LOAD_BANDS.
//   3. <table>       — raw data for each table.
//   4. shifts_calc / events_calc — computed views with *live* Excel formulas
//                                  that reference the data sheets.
//
// The point: the workbook isn't just a data dump. The rules live in cells.
// Edit `assigned_count` for a shift, and the dependent calc cells recompute
// because they're real spreadsheet formulas.

import ExcelJS from 'exceljs';

const headerStyle = (row) => {
  row.font = { bold: true };
  row.fill = {
    type: 'pattern',
    pattern: 'solid',
    fgColor: { argb: 'FFEFEFEF' },
  };
};

// Find the 1-based column index of a header on a worksheet.
function colIndex(ws, header) {
  const headers = ws.getRow(1).values;
  // ExcelJS returns a sparse 1-indexed array
  for (let i = 1; i < headers.length; i++) {
    if (headers[i] === header) return i;
  }
  return null;
}

function colLetter(n) {
  let s = '';
  while (n > 0) {
    const r = (n - 1) % 26;
    s = String.fromCharCode(65 + r) + s;
    n = Math.floor((n - 1) / 26);
  }
  return s;
}

// Helper: "shifts!$C$2:$C$100" style range for a column.
function colRange(sheetName, col, fromRow, toRow) {
  const L = colLetter(col);
  return `${sheetName}!$${L}$${fromRow}:$${L}$${toRow}`;
}

export async function buildWorkbook(ctx, schemaTables, schemaComputed, constants) {
  const wb = new ExcelJS.Workbook();
  wb.creator = 'Volunteer Shift Scheduler';
  wb.created = new Date();

  // ---------------- Sheet: Rules ----------------
  const rules = wb.addWorksheet('Rules');
  rules.columns = [
    { header: 'kind',           key: 'kind',     width: 12 },
    { header: 'table',          key: 'table',    width: 18 },
    { header: 'field',          key: 'field',    width: 28 },
    { header: 'type',           key: 'type',     width: 18 },
    { header: 'description',    key: 'desc',     width: 60 },
    { header: 'formula',        key: 'formula',  width: 70 },
    { header: 'excel formula',  key: 'excel',    width: 70 },
    { header: 'sql formula',    key: 'sql',      width: 60 },
    { header: 'depends on',     key: 'depends',  width: 50 },
    { header: 'constants',      key: 'consts',   width: 25 },
  ];
  headerStyle(rules.getRow(1));

  // Raw column rules
  for (const [tname, t] of Object.entries(schemaTables)) {
    for (const [cname, col] of Object.entries(t.columns)) {
      rules.addRow({
        kind: 'raw',
        table: tname,
        field: cname,
        type: col.type,
        desc: [
          col.primaryKey ? 'primary key' : null,
          col.notNull ? 'not null' : null,
          col.unique ? 'unique' : null,
          col.references ? `→ ${col.references.table}.${col.references.column}` : null,
          col.check ? `check: ${col.check}` : null,
          col.default !== undefined ? `default: ${col.default}` : null,
        ].filter(Boolean).join('; ') || t.description,
        formula: '— raw —',
        excel: '',
        sql: '',
        depends: '',
        consts: '',
      });
    }
  }

  // Computed rules
  for (const [key, f] of Object.entries(schemaComputed)) {
    const [tname, fname] = key.split('.');
    rules.addRow({
      kind: f.kind,
      table: tname,
      field: fname,
      type: f.returns,
      desc: f.description,
      formula: f.formula,
      excel: f.excelFormula || '',
      sql: f.sqlFormula || '',
      depends: (f.depends || []).join(', '),
      consts: (f.constants || []).join(', '),
    });
  }

  // ---------------- Sheet: Constants ----------------
  const cons = wb.addWorksheet('Constants');
  cons.columns = [
    { header: 'group', key: 'group', width: 22 },
    { header: 'key',   key: 'key',   width: 28 },
    { header: 'value', key: 'value', width: 16 },
  ];
  headerStyle(cons.getRow(1));
  for (const [k, v] of Object.entries(constants.GRADE_WEIGHTS)) {
    cons.addRow({ group: 'GRADE_WEIGHTS', key: k, value: v });
  }
  for (const b of constants.GRADE_BANDS) {
    cons.addRow({ group: 'GRADE_BANDS', key: `min ${b.min}`, value: b.grade });
  }
  for (const [k, v] of Object.entries(constants.LOAD_BANDS)) {
    cons.addRow({ group: 'LOAD_BANDS', key: k, value: v });
  }

  // ---------------- Sheets: raw data ----------------
  const dataSheets = {};
  for (const [tname] of Object.entries(schemaTables)) {
    const rows = ctx[tname] || [];
    const ws = wb.addWorksheet(tname);
    if (rows.length) {
      ws.columns = Object.keys(rows[0])
        .filter((k) => k !== 'skills') // synthetic on volunteers; not stored
        .map((k) => ({ header: k, key: k, width: 22 }));
      rows.forEach((r) => {
        const row = { ...r };
        delete row.skills;
        ws.addRow(row);
      });
      headerStyle(ws.getRow(1));
    } else {
      ws.addRow(['(empty)']);
    }
    dataSheets[tname] = ws;
  }

  // ---------------- Sheet: shifts_calc (live formulas) ----------------
  const shiftsWs = dataSheets.shifts;
  if (shiftsWs && (ctx.shifts || []).length) {
    const calc = wb.addWorksheet('shifts_calc');
    calc.columns = [
      { header: 'shift_id',           key: 'shift_id',       width: 10 },
      { header: 'name',               key: 'name',           width: 22 },
      { header: 'required_count',     key: 'required',       width: 14 },
      { header: 'assigned_count',     key: 'assigned',       width: 14 },
      { header: 'coverage_delta',     key: 'delta',          width: 14 },
      { header: 'coverage_status',    key: 'status',         width: 16 },
      { header: 'duration_hours',     key: 'duration',       width: 14 },
      { header: 'weighted_reliab.',   key: 'wreliab',        width: 16 },
    ];
    headerStyle(calc.getRow(1));

    const shiftsName = shiftsWs.name;
    const assignsName = dataSheets.assignments?.name || 'assignments';
    const volsName    = dataSheets.volunteers?.name || 'volunteers';

    // resolve columns we'll be referencing
    const sCol_id        = colIndex(shiftsWs, 'id');
    const sCol_name      = colIndex(shiftsWs, 'name');
    const sCol_required  = colIndex(shiftsWs, 'required_count');
    const sCol_starts    = colIndex(shiftsWs, 'starts_at');
    const sCol_ends      = colIndex(shiftsWs, 'ends_at');
    const aCol_shift     = colIndex(dataSheets.assignments, 'shift_id');
    const aCol_vol       = colIndex(dataSheets.assignments, 'volunteer_id');
    const vCol_id        = colIndex(dataSheets.volunteers, 'id');
    const vCol_reliab    = colIndex(dataSheets.volunteers, 'reliability_score');

    const lastShift = ctx.shifts.length + 1;
    const lastAssn  = (ctx.assignments?.length || 0) + 1;
    const lastVol   = (ctx.volunteers?.length || 0) + 1;

    const aShiftRange  = colRange(assignsName, aCol_shift, 2, lastAssn);
    const aVolRange    = colRange(assignsName, aCol_vol, 2, lastAssn);
    const vIdRange     = colRange(volsName, vCol_id, 2, lastVol);
    const vReliabRange = colRange(volsName, vCol_reliab, 2, lastVol);

    ctx.shifts.forEach((s, i) => {
      const row = i + 2; // calc sheet's own row number
      const shiftRow = i + 2; // matches shifts sheet row (same order)
      const sIdRef     = `${shiftsName}!${colLetter(sCol_id)}${shiftRow}`;
      const sNameRef   = `${shiftsName}!${colLetter(sCol_name)}${shiftRow}`;
      const sReqRef    = `${shiftsName}!${colLetter(sCol_required)}${shiftRow}`;
      const sStartsRef = `${shiftsName}!${colLetter(sCol_starts)}${shiftRow}`;
      const sEndsRef   = `${shiftsName}!${colLetter(sCol_ends)}${shiftRow}`;

      const r = calc.addRow({});
      r.getCell(1).value = { formula: sIdRef };
      r.getCell(2).value = { formula: sNameRef };
      r.getCell(3).value = { formula: sReqRef };
      // assigned_count = COUNTIF(assignments.shift_id, shifts.id)
      r.getCell(4).value = { formula: `COUNTIF(${aShiftRange}, ${sIdRef})` };
      // coverage_delta
      r.getCell(5).value = { formula: `D${row}-C${row}` };
      // coverage_status
      r.getCell(6).value = { formula: `IF(D${row}<C${row},"under",IF(D${row}>C${row},"over","full"))` };
      // duration_hours
      r.getCell(7).value = { formula: `(${sEndsRef}-${sStartsRef})*24` };
      // weighted_reliability — IFERROR(AVG of reliability over assigned volunteers)
      r.getCell(8).value = {
        formula:
          `IFERROR(SUMPRODUCT((${aShiftRange}=${sIdRef})*1, ` +
          `SUMIF(${vIdRange}, ${aVolRange}, ${vReliabRange}))/D${row}, 0)`,
      };
    });
  }

  // ---------------- Sheet: events_calc (live formulas) ----------------
  const eventsWs = dataSheets.events;
  if (eventsWs && (ctx.events || []).length && shiftsWs) {
    const calc = wb.addWorksheet('events_calc');
    calc.columns = [
      { header: 'event_id',       key: 'event_id',  width: 10 },
      { header: 'name',           key: 'name',      width: 28 },
      { header: 'total_required', key: 'req',       width: 16 },
      { header: 'shift_count',    key: 'sc',        width: 14 },
      { header: 'coverage_pct',   key: 'cov',       width: 14 },
      { header: 'avg_reliability', key: 'rel',      width: 16 },
      { header: 'staffing_score', key: 'score',     width: 16 },
      { header: 'staffing_grade', key: 'grade',     width: 16 },
    ];
    headerStyle(calc.getRow(1));

    const evName = eventsWs.name;
    const shName = shiftsWs.name;

    const eCol_id   = colIndex(eventsWs, 'id');
    const eCol_name = colIndex(eventsWs, 'name');
    const sCol_id   = colIndex(shiftsWs, 'id');
    const sCol_event = colIndex(shiftsWs, 'event_id');
    const sCol_req   = colIndex(shiftsWs, 'required_count');

    const lastEvent = ctx.events.length + 1;
    const lastShift = ctx.shifts.length + 1;
    const sEventRange = colRange(shName, sCol_event, 2, lastShift);
    const sReqRange   = colRange(shName, sCol_req, 2, lastShift);

    ctx.events.forEach((ev, i) => {
      const row = i + 2;
      const evRow = i + 2;
      const eIdRef   = `${evName}!${colLetter(eCol_id)}${evRow}`;
      const eNameRef = `${evName}!${colLetter(eCol_name)}${evRow}`;

      const r = calc.addRow({});
      r.getCell(1).value = { formula: eIdRef };
      r.getCell(2).value = { formula: eNameRef };
      // total_required = SUMIF(shifts.event_id, this.id, shifts.required_count)
      r.getCell(3).value = { formula: `SUMIF(${sEventRange}, ${eIdRef}, ${sReqRange})` };
      // shift_count   = COUNTIF(shifts.event_id, this.id)
      r.getCell(4).value = { formula: `COUNTIF(${sEventRange}, ${eIdRef})` };
      // coverage_pct  — assigned from shifts_calc which is parallel to shifts
      // We sum shifts_calc.assigned where shifts.event_id matches:
      r.getCell(5).value = {
        formula:
          `IF(C${row}=0,1,SUMPRODUCT((${sEventRange}=${eIdRef})*1, shifts_calc!$D$2:$D$${lastShift})/C${row})`,
      };
      // avg_reliability — average of shifts_calc.weighted_reliability across this event's shifts that have anyone assigned
      r.getCell(6).value = {
        formula:
          `IFERROR(SUMPRODUCT((${sEventRange}=${eIdRef})*1, shifts_calc!$H$2:$H$${lastShift}) / ` +
          `MAX(1, SUMPRODUCT((${sEventRange}=${eIdRef})*(shifts_calc!$D$2:$D$${lastShift}>0))), 0)`,
      };
      // staffing_score = 0.6*MIN(1,coverage_pct) + 0.3*avg_reliability + 0.1*1   (skill_match_avg defaulted to 1)
      r.getCell(7).value = { formula: `0.6*MIN(1,E${row})+0.3*F${row}+0.1*1` };
      // staffing_grade
      r.getCell(8).value = {
        formula:
          `IF(G${row}>=0.93,"A",IF(G${row}>=0.85,"B",IF(G${row}>=0.75,"C",IF(G${row}>=0.65,"D","F"))))`,
      };
    });
  }

  // Reorder so Rules sheet is first.
  wb.worksheets.sort((a, b) => {
    const order = ['Rules', 'Constants'];
    const ai = order.indexOf(a.name);
    const bi = order.indexOf(b.name);
    if (ai !== -1 && bi !== -1) return ai - bi;
    if (ai !== -1) return -1;
    if (bi !== -1) return 1;
    return 0;
  });

  return wb;
}
