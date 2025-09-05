export type Toast = { id: number; type: "success" | "error"; text: string };

let id = 1;
const listeners = new Set<(t: Toast) => void>();

export function onToast(cb: (t: Toast) => void) {
  listeners.add(cb);
  return () => {
    listeners.delete(cb);
  };
}

function emit(type: Toast["type"], text: string) {
  const t: Toast = { id: id++, type, text };
  listeners.forEach((l) => l(t));
}

export const toast = {
  success: (text: string) => emit("success", text),
  error: (text: string) => emit("error", text),
};
