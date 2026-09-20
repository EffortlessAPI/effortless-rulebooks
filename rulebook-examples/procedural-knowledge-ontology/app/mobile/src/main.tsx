import React from "react";
import { createRoot } from "react-dom/client";
import { HashRouter } from "react-router-dom";
import { SessionProvider } from "./session";
import App from "./App";
import "./styles.css";

createRoot(document.getElementById("root")!).render(
  <React.StrictMode><SessionProvider><HashRouter><App /></HashRouter></SessionProvider></React.StrictMode>,
);
