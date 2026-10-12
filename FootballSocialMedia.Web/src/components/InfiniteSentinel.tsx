import { useEffect, useRef } from "react";

/** Liste sonuna gelince bir sonraki sayfayı yükler. */
export function InfiniteSentinel({ onVisible, disabled }: { onVisible: () => void; disabled?: boolean }) {
  const ref = useRef<HTMLDivElement>(null);
  useEffect(() => {
    if (disabled || !ref.current) return;
    const observer = new IntersectionObserver((entries) => entries[0].isIntersecting && onVisible(), { rootMargin: "400px" });
    observer.observe(ref.current);
    return () => observer.disconnect();
  }, [onVisible, disabled]);
  return <div ref={ref} aria-hidden="true" />;
}
