import { useMemo, useState } from "react";
import {
  useLanguages,
  useCreateLanguage,
  useUpdateLanguage,
  useToggleLanguage,
  useDeleteLanguage,
} from "@/features/languages/hooks";

type Draft = { code: string; name: string };

function validateCode(code: string) {
  return /^[a-z]{2,3}(-[A-Z]{2})?$/.test(code);
}

export default function AdminLanguagesPage() {
  const { data, isLoading, error, refetch } = useLanguages({
    includeDisabled: true,
  });
  const createMut = useCreateLanguage();
  const updateMut = useUpdateLanguage();
  const toggleMut = useToggleLanguage();
  const deleteMut = useDeleteLanguage();

  const [newLang, setNewLang] = useState<Draft>({ code: "", name: "" });

  async function onCreate(e: React.FormEvent) {
    e.preventDefault();
    const code = newLang.code.trim();
    const name = newLang.name.trim();
    if (!code || !name) return;
    if (!validateCode(code)) {
      alert("Language code must match: en, fr, pt-BR");
      return;
    }
    await createMut.mutateAsync({ code, name, isEnabled: true });
    setNewLang({ code: "", name: "" });
  }

  const [editing, setEditing] = useState<Record<string, Draft>>({});
  const startEdit = (id: string, code: string, name: string) =>
    setEditing((s) => ({ ...s, [id]: { code, name } }));
  const cancelEdit = (id: string) =>
    setEditing((s) => {
      const { [id]: _, ...rest } = s;
      return rest;
    });
  const saveEdit = async (id: string) => {
    const draft = editing[id];
    if (!draft) return;
    const code = draft.code.trim();
    const name = draft.name.trim();
    if (!code || !name) return;
    if (!validateCode(code)) {
      alert("Language code must match: en, fr, pt-BR");
      return;
    }
    await updateMut.mutateAsync({ id, code, name });
    cancelEdit(id);
  };

  const busy =
    createMut.isPending ||
    updateMut.isPending ||
    toggleMut.isPending ||
    deleteMut.isPending;
  const langs = useMemo(() => data ?? [], [data]);

  return (
    <div className="space-y-4">
      <h1 className="text-2xl font-bold">Admin — Languages</h1>
      <p className="text-sm opacity-75">
        Create, edit code/name, enable/disable, and delete a language. Enabled
        languages immediately appear as columns in the localization table.
      </p>

      {/* Create a new language */}
      <form
        onSubmit={onCreate}
        className="flex flex-wrap gap-2 items-end rounded border p-3 bg-white"
      >
        <label className="text-sm">
          Code
          <input
            className="mt-1 h-9 rounded border px-2 w-28"
            value={newLang.code}
            onChange={(e) =>
              setNewLang((s) => ({ ...s, code: e.target.value }))
            }
            placeholder="it"
          />
        </label>
        <label className="text-sm">
          Name
          <input
            className="mt-1 h-9 rounded border px-2 w-64"
            value={newLang.name}
            onChange={(e) =>
              setNewLang((s) => ({ ...s, name: e.target.value }))
            }
            placeholder="Italiano"
          />
        </label>
        <button
          type="submit"
          disabled={
            createMut.isPending || !newLang.code.trim() || !newLang.name.trim()
          }
          className="h-9 rounded bg-blue-600 text-white px-3 disabled:opacity-60"
        >
          {createMut.isPending ? "Creating…" : "Add language"}
        </button>
      </form>

      {/* Languages table */}
      <div className="overflow-auto border rounded-md">
        <table className="min-w-[720px] w-full border-collapse">
          <thead className="sticky top-0 z-10 bg-white shadow-[0_1px_0_#00000010]">
            <tr className="text-left">
              <th className="p-2 text-sm font-semibold border-r w-[160px]">
                Code
              </th>
              <th className="p-2 text-sm font-semibold border-r">Name</th>
              <th className="p-2 text-sm font-semibold border-r w-[120px]">
                Status
              </th>
              <th className="p-2 text-sm font-semibold w-[260px]">Actions</th>
            </tr>
          </thead>
          <tbody>
            {isLoading ? (
              Array.from({ length: 6 }).map((_, i) => (
                <tr key={i} className="animate-pulse">
                  <td className="p-2 border-t">
                    <div className="h-4 w-24 bg-gray-200 rounded" />
                  </td>
                  <td className="p-2 border-t">
                    <div className="h-4 w-64 bg-gray-200 rounded" />
                  </td>
                  <td className="p-2 border-t">
                    <div className="h-4 w-20 bg-gray-200 rounded" />
                  </td>
                  <td className="p-2 border-t">
                    <div className="h-8 w-48 bg-gray-200 rounded" />
                  </td>
                </tr>
              ))
            ) : error ? (
              <tr>
                <td colSpan={4} className="p-3 text-sm text-red-600">
                  Failed to load.{" "}
                  <button onClick={() => refetch()} className="underline">
                    Retry
                  </button>
                </td>
              </tr>
            ) : langs.length === 0 ? (
              <tr>
                <td colSpan={4} className="p-4 text-sm text-gray-500">
                  No languages
                </td>
              </tr>
            ) : (
              langs.map((lng) => {
                const isEdit = !!editing[lng.id];
                const d = editing[lng.id] ?? { code: lng.code, name: lng.name };
                return (
                  <tr key={lng.id} className="hover:bg-gray-50 border-t">
                    <td className="p-2 align-top">
                      {isEdit ? (
                        <input
                          className="h-9 w-full rounded border px-2 font-mono"
                          value={d.code}
                          onChange={(e) =>
                            setEditing((s) => ({
                              ...s,
                              [lng.id]: { ...d, code: e.target.value },
                            }))
                          }
                        />
                      ) : (
                        <span className="font-mono">{lng.code}</span>
                      )}
                    </td>
                    <td className="p-2 align-top">
                      {isEdit ? (
                        <input
                          className="h-9 w-full rounded border px-2"
                          value={d.name}
                          onChange={(e) =>
                            setEditing((s) => ({
                              ...s,
                              [lng.id]: { ...d, name: e.target.value },
                            }))
                          }
                        />
                      ) : (
                        <span>{lng.name}</span>
                      )}
                    </td>
                    <td className="p-2 align-top">
                      <span
                        className={
                          lng.isEnabled ? "text-emerald-700" : "text-gray-500"
                        }
                      >
                        {lng.isEnabled ? "Enabled" : "Disabled"}
                      </span>
                    </td>
                    <td className="p-2 align-top">
                      <div className="flex flex-wrap gap-2">
                        {!isEdit ? (
                          <>
                            <button
                              className="rounded border px-3 h-8"
                              disabled={busy}
                              onClick={() =>
                                startEdit(lng.id, lng.code, lng.name)
                              }
                            >
                              Edit
                            </button>
                            <button
                              className={`rounded px-3 h-8 ${
                                lng.isEnabled
                                  ? "border"
                                  : "bg-blue-600 text-white"
                              }`}
                              disabled={toggleMut.isPending}
                              onClick={() =>
                                toggleMut.mutate({
                                  id: lng.id,
                                  isEnabled: !lng.isEnabled,
                                })
                              }
                            >
                              {lng.isEnabled ? "Disable" : "Enable"}
                            </button>
                            <button
                              className="rounded border px-3 h-8 hover:bg-red-50"
                              disabled={deleteMut.isPending}
                              onClick={() => {
                                if (!confirm(`Delete language ${lng.code}?`))
                                  return;
                                deleteMut.mutate({ id: lng.id });
                              }}
                            >
                              Delete
                            </button>
                          </>
                        ) : (
                          <>
                            <button
                              className="rounded bg-emerald-600 text-white px-3 h-8 disabled:opacity-60"
                              disabled={
                                updateMut.isPending ||
                                !d.code.trim() ||
                                !d.name.trim()
                              }
                              onClick={() => saveEdit(lng.id)}
                            >
                              Save
                            </button>
                            <button
                              className="rounded border px-3 h-8"
                              disabled={busy}
                              onClick={() => cancelEdit(lng.id)}
                            >
                              Cancel
                            </button>
                          </>
                        )}
                      </div>
                    </td>
                  </tr>
                );
              })
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
}
