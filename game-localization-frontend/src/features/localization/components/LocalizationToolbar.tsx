import { useEffect, useRef } from "react";

type Props = {
  search: string;
  onSearchChange: (v: string) => void;
  page: number;
  pageSize: number;
  total: number;
  onPageChange: (p: number) => void;
  onPageSizeChange: (s: number) => void;
  includeDisabled: boolean;
  onIncludeDisabledChange: (v: boolean) => void;
  isAdmin?: boolean;
  onAddKey: () => void;
  onAddLanguage: () => void;
  isFetching?: boolean;
};

export function LocalizationToolbar(props: Props) {
  const {
    search,
    onSearchChange,
    page,
    pageSize,
    total,
    onPageChange,
    onPageSizeChange,
    includeDisabled,
    onIncludeDisabledChange,
    isAdmin = false,
    onAddKey,
    onAddLanguage,
    isFetching,
  } = props;

  const totalPages = Math.max(1, Math.ceil(total / Math.max(1, pageSize)));
  const inputRef = useRef<HTMLInputElement>(null);

  const from = (page - 1) * pageSize + 1;
  const to = Math.min(total, page * pageSize);

  useEffect(() => {
    inputRef.current?.focus();
  }, []);

  return (
    <div className="flex flex-col gap-3 md:flex-row md:items-end md:justify-between">
      <div className="flex flex-wrap items-end gap-3">
        <div className="flex flex-col">
          <label className="text-xs font-medium">Search by key</label>
          <input
            ref={inputRef}
            value={search}
            onChange={(e) => onSearchChange(e.target.value)}
            placeholder="type to search…"
            className="h-9 w-64 rounded border px-2"
          />
        </div>

        <div className="flex flex-col">
          <label className="text-xs font-medium">Page size</label>
          <select
            value={pageSize}
            onChange={(e) => onPageSizeChange(parseInt(e.target.value, 10))}
            className="h-9 w-28 rounded border px-2"
          >
            {[25, 50, 100].map((s) => (
              <option key={s} value={s}>
                {s}
              </option>
            ))}
          </select>
        </div>

        {isAdmin && (
          <label className="inline-flex items-center gap-2 text-sm select-none">
            <input
              type="checkbox"
              checked={includeDisabled}
              onChange={(e) => onIncludeDisabledChange(e.target.checked)}
            />
            Include disabled languages
          </label>
        )}
        {isFetching ? <div className="text-xs opacity-70">Loading…</div> : null}
      </div>

      <div className="flex flex-wrap items-center gap-2">
        <div className="flex gap-2">
          <button
            onClick={onAddKey}
            className="h-9 rounded bg-emerald-600 px-3 text-white"
          >
            Add key
          </button>
          <button
            onClick={onAddLanguage}
            className="h-9 rounded bg-blue-600 px-3 text-white"
          >
            Add language
          </button>
        </div>

        <div className="ml-2 text-sm opacity-80">
          Page {page} of {totalPages} · Showing {total === 0 ? 0 : from}–{to} of{" "}
          {total}
        </div>

        <div className="flex gap-1">
          <button
            onClick={() => onPageChange(Math.max(1, page - 1))}
            disabled={page <= 1}
            className="h-9 rounded border px-3 disabled:opacity-50"
          >
            Prev
          </button>
          <button
            onClick={() => onPageChange(Math.min(totalPages, page + 1))}
            disabled={page >= totalPages}
            className="h-9 rounded border px-3 disabled:opacity-50"
          >
            Next
          </button>
        </div>
      </div>
    </div>
  );
}
