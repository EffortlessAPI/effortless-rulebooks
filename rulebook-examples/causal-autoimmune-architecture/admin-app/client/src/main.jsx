// main.jsx — React client entry.
//
//   Postgres  <-  Express (API)  <-  Vite (this React app)
//
// The client is a faithful surface for the inference the Express API exposes.
// It never computes anything itself and never talks to Postgres — it renders
// whatever the API serves (which the API reads from vw_* views).
import React from 'react';
import { createRoot } from 'react-dom/client';
import App from './App.jsx';
import { LocationProvider } from './router.jsx';

createRoot(document.getElementById('root')).render(
  <React.StrictMode>
    <LocationProvider>
      <App />
    </LocationProvider>
  </React.StrictMode>
);
