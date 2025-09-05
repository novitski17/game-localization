import type {
  LocalizationTableResponse,
  LocalizationTableParams,
} from "@/types";
import { EditableCell } from "./EditableCell";
import { useState } from "react";
import { useDeleteKey } from "@/features/localization/hooks";

type Props = {
  data?: LocalizationTableResponse;
  isLoading: boolean;
  tableParams: LocalizationTableParams;
};

export function LocalizationTable({ data, isLoading, tableParams }: Props) {
  const languageCodes = data?.languageCodes ?? [];
  const rows = data?.rows.items ?? [];

  const del = useDeleteKey(tableParams);
  const [pendingId, setPendingId] = useState<string | null>(null);

  async function handleDelete(id: string, keyName: string) {
    if (
      !confirm(
        `Delete key "${keyName}"?\nAll translations for this key will be removed.`
      )
    )
      return;

    setPendingId(id);
    try {
      await del.mutateAsync({ id });
    } finally {
      setPendingId(null);
    }
  }

  return (
    <div className="overflow-auto border rounded-md">
      <table className="min-w-[900px] w-full border-collapse">
        <thead className="sticky top-0 z-20 bg-white shadow-[0_1px_0_#00000010]">
          <tr>
            <th className="sticky left-0 z-30 bg-white p-2 text-left text-sm font-semibold border-r w-[280px]">
              Key
            </th>
            {languageCodes.map((code) => (
              <th
                key={code}
                className="p-2 text-left text-sm font-semibold border-r min-w-[200px]"
              >
                {code}
              </th>
            ))}
          </tr>
        </thead>
        <tbody>
          {isLoading ? (
            Array.from({ length: 10 }).map((_, i) => (
              <tr key={i} className="animate-pulse">
                <td className="sticky left-0 z-10 bg-white p-2 border-r">
                  <div className="h-4 w-48 bg-gray-200 rounded" />
                </td>
                {languageCodes.length ? (
                  languageCodes.map((c) => (
                    <td key={c + i} className="p-2 border-r">
                      <div className="h-4 w-full bg-gray-200 rounded" />
                    </td>
                  ))
                ) : (
                  <td className="p-2">
                    <div className="h-4 w-[600px] bg-gray-200 rounded" />
                  </td>
                )}
              </tr>
            ))
          ) : rows.length === 0 ? (
            <tr>
              <td
                className="p-6 text-sm text-gray-500"
                colSpan={Math.max(2, 1 + languageCodes.length)}
              >
                Nothing found
              </td>
            </tr>
          ) : (
            rows.map((row) => (
              <tr key={row.keyId} className="hover:bg-gray-50">
                <td className="sticky left-0 z-10 bg-white p-2 text-sm border-r align-top">
                  <div className="flex items-start gap-2">
                    <div className="font-medium break-all">{row.key}</div>
                    <button
                      className="ml-auto shrink-0 rounded border px-2 h-7 text-xs hover:bg-red-50"
                      disabled={del.isPending && pendingId === row.keyId}
                      onClick={() => handleDelete(row.keyId, row.key)}
                      title="Delete key"
                      aria-label={`Delete key ${row.key}`}
                    >
                      {del.isPending && pendingId === row.keyId
                        ? "Deleting…"
                        : "Delete"}
                    </button>
                  </div>
                </td>
                {languageCodes.map((code) => (
                  <td key={code} className="p-2 text-sm border-r align-top">
                    <EditableCell
                      keyId={row.keyId}
                      lang={code}
                      value={row.valuesByLanguage?.[code]}
                      tableParams={tableParams}
                    />
                  </td>
                ))}
              </tr>
            ))
          )}
        </tbody>
      </table>
    </div>
  );
}
