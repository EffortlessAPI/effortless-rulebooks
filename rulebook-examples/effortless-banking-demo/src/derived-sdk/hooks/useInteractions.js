import { useState, useEffect, useCallback } from 'react';
import { api } from '../api.js';

export function useInteractions() {
  const [data, setData] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  useEffect(() => { api.interactions.list().then(setData).catch(setError).finally(() => setLoading(false)); }, []);
  const refresh = useCallback(() => { setLoading(true); api.interactions.list().then(setData).catch(setError).finally(() => setLoading(false)); }, []);
  return { data, loading, error, refresh };
}

export function useInteraction(id) {
  const [data, setData] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  useEffect(() => { if (id) api.interactions.get(id).then(setData).catch(setError).finally(() => setLoading(false)); }, [id]);
  return { data, loading, error };
}

export function useCreateInteraction() {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const mutate = useCallback(async (body) => { setLoading(true); setError(null); try { return await api.interactions.create(body); } catch (e) { setError(e); throw e; } finally { setLoading(false); } }, []);
  return { mutate, loading, error };
}

export function useUpdateInteraction() {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const mutate = useCallback(async (id, body) => { setLoading(true); setError(null); try { return await api.interactions.update(id, body); } catch (e) { setError(e); throw e; } finally { setLoading(false); } }, []);
  return { mutate, loading, error };
}

export function useDeleteInteraction() {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const mutate = useCallback(async (id) => { setLoading(true); setError(null); try { return await api.interactions.remove(id); } catch (e) { setError(e); throw e; } finally { setLoading(false); } }, []);
  return { mutate, loading, error };
}
