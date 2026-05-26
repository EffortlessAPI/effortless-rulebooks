import { useState, useEffect, useCallback } from 'react';
import { api } from '../api.js';

export function useBusinesses() {
  const [data, setData] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  useEffect(() => { api.businesses.list().then(setData).catch(setError).finally(() => setLoading(false)); }, []);
  const refresh = useCallback(() => { setLoading(true); api.businesses.list().then(setData).catch(setError).finally(() => setLoading(false)); }, []);
  return { data, loading, error, refresh };
}

export function useBusiness(id) {
  const [data, setData] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  useEffect(() => { if (id) api.businesses.get(id).then(setData).catch(setError).finally(() => setLoading(false)); }, [id]);
  return { data, loading, error };
}

export function useCreateBusiness() {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const mutate = useCallback(async (body) => { setLoading(true); setError(null); try { return await api.businesses.create(body); } catch (e) { setError(e); throw e; } finally { setLoading(false); } }, []);
  return { mutate, loading, error };
}

export function useUpdateBusiness() {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const mutate = useCallback(async (id, body) => { setLoading(true); setError(null); try { return await api.businesses.update(id, body); } catch (e) { setError(e); throw e; } finally { setLoading(false); } }, []);
  return { mutate, loading, error };
}

export function useDeleteBusiness() {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const mutate = useCallback(async (id) => { setLoading(true); setError(null); try { return await api.businesses.remove(id); } catch (e) { setError(e); throw e; } finally { setLoading(false); } }, []);
  return { mutate, loading, error };
}
