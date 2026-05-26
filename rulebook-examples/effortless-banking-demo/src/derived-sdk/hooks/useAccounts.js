import { useState, useEffect, useCallback } from 'react';
import { api } from '../api.js';

export function useAccounts() {
  const [data, setData] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  useEffect(() => { api.accounts.list().then(setData).catch(setError).finally(() => setLoading(false)); }, []);
  const refresh = useCallback(() => { setLoading(true); api.accounts.list().then(setData).catch(setError).finally(() => setLoading(false)); }, []);
  return { data, loading, error, refresh };
}

export function useAccount(id) {
  const [data, setData] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  useEffect(() => { if (id) api.accounts.get(id).then(setData).catch(setError).finally(() => setLoading(false)); }, [id]);
  return { data, loading, error };
}

export function useCreateAccount() {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const mutate = useCallback(async (body) => { setLoading(true); setError(null); try { return await api.accounts.create(body); } catch (e) { setError(e); throw e; } finally { setLoading(false); } }, []);
  return { mutate, loading, error };
}

export function useUpdateAccount() {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const mutate = useCallback(async (id, body) => { setLoading(true); setError(null); try { return await api.accounts.update(id, body); } catch (e) { setError(e); throw e; } finally { setLoading(false); } }, []);
  return { mutate, loading, error };
}

export function useDeleteAccount() {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const mutate = useCallback(async (id) => { setLoading(true); setError(null); try { return await api.accounts.remove(id); } catch (e) { setError(e); throw e; } finally { setLoading(false); } }, []);
  return { mutate, loading, error };
}
