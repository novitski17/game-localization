import { useEffect, useRef, useState } from "react";
import { useUpdateTranslation } from "@/features/localization/hooks";
import type { LocalizationTableParams } from "@/types";

type Props = {
  keyId: string;
  lang: string;
  value: string | null | undefined;
  tableParams: LocalizationTableParams;
};

export function EditableCell({ keyId, lang, value, tableParams }: Props) {
  const [editing, setEditing] = useState(false);
  const [text, setText] = useState(value ?? "");
  const [saving, setSaving] = useState(false);
  const inputRef = useRef<HTMLTextAreaElement>(null);

  const update = useUpdateTranslation();

  useEffect(() => {
    if (editing) {
      setText(value ?? "");
      setTimeout(() => inputRef.current?.focus(), 0);
    }
  }, [editing, value]);

  function startEdit() {
    setEditing(true);
  }

  function cancelEdit() {
    setEditing(false);
    setText(value ?? "");
  }

  async function saveIfChanged() {
    const original = value ?? "";
    if (text === original) {
      setEditing(false);
      return;
    }
    try {
      setSaving(true);
      await update.mutateAsync({ keyId, lang, value: text, tableParams });
      setEditing(false);
    } finally {
      setSaving(false);
    }
  }

  function onKeyDown(e: React.KeyboardEvent<HTMLTextAreaElement>) {
    if (e.key === "Enter" && !e.shiftKey) {
      e.preventDefault();
      void saveIfChanged();
    } else if (e.key === "Escape") {
      e.preventDefault();
      cancelEdit();
    }
  }

  if (!editing) {
    return (
      <div
        className="whitespace-pre-wrap text-gray-800 min-h-5 cursor-text"
        onClick={startEdit}
        title="Click to edit"
      >
        {saving ? (
          <span className="opacity-60 text-xs">Saving…</span>
        ) : (
          value ?? ""
        )}
      </div>
    );
  }

  return (
    <div className="relative">
      <textarea
        ref={inputRef}
        value={text}
        onChange={(e) => setText(e.target.value)}
        onBlur={saveIfChanged}
        onKeyDown={onKeyDown}
        rows={Math.min(6, Math.max(1, (text.match(/\n/g)?.length ?? 0) + 1))}
        className="w-full resize-y rounded border px-2 py-1 text-sm focus:outline-none focus:ring focus:ring-blue-200"
        placeholder="Enter translation…"
      />
      {saving && (
        <div className="absolute right-1 bottom-1 text-[10px] opacity-60">
          Saving…
        </div>
      )}
    </div>
  );
}
