import { useEffect, useState } from "react";
import TableView from "./TableView";
import "./App.css";

const API = "/api";

export default function App() {
  const [tables, setTables] = useState([]);
  const [active, setActive] = useState(null);

  useEffect(() => {
    fetch(`${API}/meta`)
      .then((r) => r.json())
      .then((data) => {
        setTables(data);
        if (data.length) setActive(data[0].key);
      });
  }, []);

  const activeMeta = tables.find((t) => t.key === active);

  return (
    <div className="admin-shell">
      <nav className="sidebar">
        <h2>Job Search RAG</h2>
        <p className="subtitle">Admin</p>
        <ul>
          {tables.map((t) => (
            <li
              key={t.key}
              className={t.key === active ? "active" : ""}
              onClick={() => setActive(t.key)}
            >
              {t.label}
            </li>
          ))}
        </ul>
      </nav>
      <main className="content">
        {activeMeta ? (
          <TableView key={activeMeta.key} meta={activeMeta} />
        ) : (
          <p>Loading...</p>
        )}
      </main>
    </div>
  );
}
