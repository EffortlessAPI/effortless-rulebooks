import React from 'react';
import { createRoot } from 'react-dom/client';
import { BrowserRouter } from 'react-router-dom';
import App from './App.jsx';
import { AuthProvider } from './auth.jsx';
import { FieldDrawerProvider } from './FieldDrawer.jsx';
import './styles.css';

createRoot(document.getElementById('root')).render(
  <BrowserRouter>
    <AuthProvider>
      <FieldDrawerProvider>
        <App />
      </FieldDrawerProvider>
    </AuthProvider>
  </BrowserRouter>,
);
