import { useEffect, useState } from "react";
import { onToast } from "@/utils/toast";
import type { Toast } from "@/utils/toast";

export function Toasts() {
  const [items, setItems] = useState<Toast[]>([]);

  useEffect(() => {
    const off = onToast((t: Toast) => {
      setItems((prev) => [...prev, t]);
      setTimeout(
        () => setItems((prev) => prev.filter((x) => x.id !== t.id)),
        3000
      );
    });
    return off;
  }, []);

  return (
    <div className="fixed bottom-4 right-4 flex flex-col gap-2 z-50">
      {items.map((t) => (
        <div
          key={t.id}
          className={`px-3 py-2 rounded shadow text-sm border bg-white ${
            t.type === "error" ? "border-red-300" : "border-emerald-300"
          }`}
        >
          {t.text}
        </div>
      ))}
    </div>
  );
}
