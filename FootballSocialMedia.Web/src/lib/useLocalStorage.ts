import { useCallback, useState } from "react";

/** Küçük kullanıcı tercihleri için (seçili sekme vb.). localStorage erişilemezse sessizce bellekte tutar. */
export function useLocalStorage<T>(key: string, initial: T) {
  const [value, setValue] = useState<T>(() => {
    try {
      const raw = localStorage.getItem(key);
      return raw ? (JSON.parse(raw) as T) : initial;
    } catch {
      return initial;
    }
  });

  const set = useCallback(
    (next: T) => {
      setValue(next);
      try {
        localStorage.setItem(key, JSON.stringify(next));
      } catch {
        /* yok say */
      }
    },
    [key],
  );

  return [value, set] as const;
}
