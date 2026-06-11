import React from "react";
import { createRoot } from "react-dom/client";
import Root from "./Root.jsx";
import "./styles.css";

// Root gates the (unchanged) release console behind the login/sync page and the
// engine bar. App.jsx itself is untouched — see Root.jsx.
createRoot(document.getElementById("root")).render(
  <React.StrictMode>
    <Root />
  </React.StrictMode>
);
