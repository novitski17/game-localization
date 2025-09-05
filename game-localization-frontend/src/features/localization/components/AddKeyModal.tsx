import { useState } from "react";
import Modal from "@/components/Modal";
import { useCreateKey } from "@/features/localization/hooks";
import type { LocalizationTableParams } from "@/types";
import { toast } from "@/utils/toast";

type Props = {
  open: boolean;
  onClose: () => void;
  tableParams: LocalizationTableParams;
};

export function AddKeyModal({ open, onClose, tableParams }: Props) {
  const [keyVal, setKeyVal] = useState("");
  const createKey = useCreateKey(tableParams);

  async function submit(e: React.FormEvent) {
    e.preventDefault();
    const trimmed = keyVal.trim();
    if (!trimmed) return;
    try {
      await createKey.mutateAsync({ key: trimmed });
      toast.success("Key created");
      setKeyVal("");
      onClose();
    } catch (err) {
      console.error(err);
      toast.error("Failed to create key");
    }
  }

  return (
    <Modal open={open} onClose={onClose} title="Add key">
      <form onSubmit={submit} className="space-y-3">
        <label className="block text-sm">
          Key
          <input
            className="mt-1 h-9 w-full rounded border px-2"
            value={keyVal}
            onChange={(e) => setKeyVal(e.target.value)}
            placeholder="e.g., app.title"
            autoFocus
          />
        </label>
        <div className="flex justify-end gap-2">
          <button
            type="button"
            onClick={onClose}
            className="rounded border px-3 h-9"
          >
            Cancel
          </button>
          <button
            type="submit"
            disabled={createKey.isPending}
            className="rounded bg-emerald-600 text-white px-3 h-9 disabled:opacity-60"
          >
            {createKey.isPending ? "Creating…" : "Create"}
          </button>
        </div>
      </form>
    </Modal>
  );
}
