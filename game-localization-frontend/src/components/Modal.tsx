import { type PropsWithChildren, useEffect } from "react";

type ModalProps = PropsWithChildren<{
  open: boolean;
  onClose: () => void;
  title?: string;
  widthClassName?: string;
}>;

export default function Modal({
  open,
  onClose,
  title,
  widthClassName = "max-w-md",
  children,
}: ModalProps) {
  useEffect(() => {
    function onKey(e: KeyboardEvent) {
      if (e.key === "Escape") onClose();
    }
    if (open) document.addEventListener("keydown", onKey);
    return () => document.removeEventListener("keydown", onKey);
  }, [open, onClose]);

  if (!open) return null;
  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center">
      <div className="absolute inset-0 bg-black/30" onClick={onClose} />
      <div
        className={`relative z-10 w-full ${widthClassName} rounded-lg bg-white p-4 shadow-lg border`}
      >
        {title ? <h3 className="mb-3 text-lg font-semibold">{title}</h3> : null}
        {children}
      </div>
    </div>
  );
}
