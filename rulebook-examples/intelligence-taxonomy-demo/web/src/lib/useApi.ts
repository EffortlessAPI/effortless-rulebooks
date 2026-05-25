import { useCallback, useEffect, useState } from 'react';
import { api } from './api';

export function useApi<T = unknown>(path: string | null) {
  const [data, setData] = useState<T | null>(null);
  const [error, setError] = useState<Error | null>(null);
  const [loading, setLoading] = useState<boolean>(path !== null);

  const load = useCallback(async () => {
    if (path === null) {
      setData(null);
      setLoading(false);
      return;
    }
    setLoading(true);
    try {
      const json = await api<T>(path);
      setData(json);
      setError(null);
    } catch (e) {
      setError(e as Error);
    } finally {
      setLoading(false);
    }
  }, [path]);

  useEffect(() => {
    void load();
  }, [load]);

  return { data, error, loading, reload: load };
}
