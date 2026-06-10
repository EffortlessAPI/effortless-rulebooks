import React from "react";
import { createRoot } from "react-dom/client";
import { BrowserRouter } from "react-router-dom";
import Root from "./Root";
import "./styles.css";
// The explainer-dag module's own stylesheet is imported in Root.tsx alongside
// the module itself.

// Root gates the release console behind the login/sync page and the engine bar.
// react-router carries the gate + the active view (Flow/Graph/Closure) + the
// explain-DAG drill-in as real URLs — see Root.tsx and App.tsx.
const el = document.getElementById("root");
if (!el) throw new Error("missing #root element");

createRoot(el).render(
  <React.StrictMode>
    <BrowserRouter>
      <Root />
    </BrowserRouter>
  </React.StrictMode>
);
