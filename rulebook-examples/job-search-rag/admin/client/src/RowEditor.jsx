import { useState } from "react";

export default function RowEditor({ meta, row, isNew, onSave, onCancel }) {
  const [form, setForm] = useState({ ...row });

  const set = (col, value) => setForm((prev) => ({ ...prev, [col]: value }));

  const handleSubmit = (e) => {
    e.preventDefault();
    // Convert empty strings to null for nullable fields
    const cleaned = {};
    for (const col of meta.columns) {
      const v = form[col];
      cleaned[col] = v === "" ? null : v;
    }
    onSave(cleaned);
  };

  return (
    <div className="editor">
      <h2>{isNew ? `New ${meta.label}` : `Edit ${meta.label}`}</h2>
      <form onSubmit={handleSubmit}>
        {meta.columns.map((col) => (
          <div className="field" key={col}>
            <label>{formatCol(col)}</label>
            {col === meta.pk && !isNew ? (
              <input value={form[col] ?? ""} disabled />
            ) : isTextArea(col) ? (
              <textarea
                rows={5}
                value={form[col] ?? ""}
                onChange={(e) => set(col, e.target.value)}
              />
            ) : isBool(col, form[col]) ? (
              <select
                value={form[col] === true || form[col] === "true" ? "true" : "false"}
                onChange={(e) => set(col, e.target.value === "true")}
              >
                <option value="true">Yes</option>
                <option value="false">No</option>
              </select>
            ) : (
              <input
                type="text"
                value={form[col] ?? ""}
                onChange={(e) => set(col, e.target.value)}
              />
            )}
          </div>
        ))}
        <div className="editor-actions">
          <button type="submit" className="btn btn-primary">
            Save
          </button>
          <button type="button" className="btn" onClick={onCancel}>
            Cancel
          </button>
        </div>
      </form>
    </div>
  );
}

function formatCol(col) {
  return col
    .replace(/_/g, " ")
    .replace(/\b\w/g, (c) => c.toUpperCase());
}

const BOOL_COLS = [
  "is_enabled",
  "is_headless",
  "disqualify_on_llm_flag",
  "is_disqualified",
];

function isBool(col, value) {
  return BOOL_COLS.includes(col) || typeof value === "boolean";
}

const TEXTAREA_COLS = [
  "description",
  "full_text",
  "content",
  "reason",
  "disqualifier_reason",
  "signals_positive",
  "signals_negative",
  "url",
  "comp_text",
];

function isTextArea(col) {
  return TEXTAREA_COLS.includes(col);
}
