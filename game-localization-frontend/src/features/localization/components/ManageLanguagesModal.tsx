import Modal from "@/components/Modal";
import { useLanguages, useToggleLanguage } from "@/features/languages/hooks";

type Props = { open: boolean; onClose: () => void };

export function ManageLanguagesModal({ open, onClose }: Props) {
  const { data, isLoading, error, refetch } = useLanguages({
    includeDisabled: true,
  });
  const toggle = useToggleLanguage();

  return (
    <Modal open={open} onClose={onClose} title="Languages">
      {isLoading ? (
        <div className="p-2 text-sm opacity-70">Loading…</div>
      ) : error ? (
        <div className="p-2 text-sm text-red-600">
          Failed to load.{" "}
          <button onClick={() => refetch()} className="underline">
            Retry
          </button>
        </div>
      ) : (
        <div className="max-h-[60vh] overflow-auto">
          <table className="w-full text-sm">
            <thead>
              <tr className="text-left">
                <th className="p-2">Code</th>
                <th className="p-2">Name</th>
                <th className="p-2">Status</th>
                <th className="p-2"></th>
              </tr>
            </thead>
            <tbody>
              {(data ?? []).map((lng) => (
                <tr key={lng.id} className="border-t">
                  <td className="p-2 font-mono">{lng.code}</td>
                  <td className="p-2">{lng.name}</td>
                  <td className="p-2">
                    {lng.isEnabled ? "Enabled" : "Disabled"}
                  </td>
                  <td className="p-2 text-right">
                    <button
                      className={`rounded px-3 h-8 ${
                        lng.isEnabled ? "border" : "bg-blue-600 text-white"
                      }`}
                      disabled={toggle.isPending}
                      onClick={() =>
                        toggle.mutate({ id: lng.id, isEnabled: !lng.isEnabled })
                      }
                    >
                      {lng.isEnabled ? "Disable" : "Enable"}
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
      <div className="mt-3 text-right">
        <button onClick={onClose} className="rounded border px-3 h-9">
          Close
        </button>
      </div>
    </Modal>
  );
}
