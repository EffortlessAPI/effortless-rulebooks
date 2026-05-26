import { useState, useEffect, useCallback } from 'react';
import { api } from '../api.js';

export function useDocuments() {
  const [data, setData] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  useEffect(() => { api.documents.list().then(setData).catch(setError).finally(() => setLoading(false)); }, []);
  const refresh = useCallback(() => { setLoading(true); api.documents.list().then(setData).catch(setError).finally(() => setLoading(false)); }, []);
  return { data, loading, error, refresh };
}

export function useDocument(id) {
  const [data, setData] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  useEffect(() => { if (id) api.documents.get(id).then(setData).catch(setError).finally(() => setLoading(false)); }, [id]);
  return { data, loading, error };
}

export function useCreateDocument() {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const mutate = useCallback(async (body) => { setLoading(true); setError(null); try { return await api.documents.create(body); } catch (e) { setError(e); throw e; } finally { setLoading(false); } }, []);
  return { mutate, loading, error };
}

export function useUpdateDocument() {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const mutate = useCallback(async (id, body) => { setLoading(true); setError(null); try { return await api.documents.update(id, body); } catch (e) { setError(e); throw e; } finally { setLoading(false); } }, []);
  return { mutate, loading, error };
}

export function useDeleteDocument() {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const mutate = useCallback(async (id) => { setLoading(true); setError(null); try { return await api.documents.remove(id); } catch (e) { setError(e); throw e; } finally { setLoading(false); } }, []);
  return { mutate, loading, error };
}
