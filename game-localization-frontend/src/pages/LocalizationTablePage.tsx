import { useState, useEffect, useMemo } from "react";
import { useLocalizationTable } from "@/features/localization/hooks";
import { LocalizationToolbar } from "@/features/localization/components/LocalizationToolbar";
import { LocalizationTable } from "@/features/localization/components/LocalizationTable";
import { AddKeyModal } from "@/features/localization/components/AddKeyModal";
import { ManageLanguagesModal } from "@/features/localization/components/ManageLanguagesModal";
import { useDebouncedValue } from "@/hooks/useDebouncedValue";
import { useMe } from "@/features/auth/hooks";

export default function LocalizationTablePage() {
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(25);
  const [includeDisabled, setIncludeDisabled] = useState(false);
  const [search, setSearch] = useState("");

  const debouncedSearch = useDebouncedValue(search, 400);
  const params = useMemo(
    () => ({
      page,
      pageSize,
      includeDisabledLanguages: includeDisabled,
      search: debouncedSearch || undefined,
    }),
    [page, pageSize, includeDisabled, debouncedSearch]
  );

  const { data, isLoading, isFetching, error, refetch } =
    useLocalizationTable(params);
  const total = data?.rows.total ?? 0;

  useEffect(() => {
    setPage(1);
  }, [pageSize, debouncedSearch, includeDisabled]);

  const { data: me } = useMe();
  const isAdmin = me?.role === "Admin";

  const [addKeyOpen, setAddKeyOpen] = useState(false);
  const [langsOpen, setLangsOpen] = useState(false);

  return (
    <div className="space-y-4">
      <h1 className="text-2xl font-bold">Localization Table</h1>

      <LocalizationToolbar
        search={search}
        onSearchChange={setSearch}
        page={page}
        pageSize={pageSize}
        total={total}
        onPageChange={setPage}
        onPageSizeChange={setPageSize}
        includeDisabled={includeDisabled}
        onIncludeDisabledChange={setIncludeDisabled}
        isAdmin={isAdmin}
        onAddKey={() => setAddKeyOpen(true)}
        onAddLanguage={() => setLangsOpen(true)}
        isFetching={isFetching}
      />

      {error ? (
        <div className="rounded border border-red-200 bg-red-50 p-3 text-red-700">
          Failed to load the table
          <button onClick={() => refetch()} className="ml-3 underline">
            Retry
          </button>
        </div>
      ) : null}

      <LocalizationTable
        data={data}
        isLoading={isLoading}
        tableParams={params}
      />

      <AddKeyModal
        open={addKeyOpen}
        onClose={() => setAddKeyOpen(false)}
        tableParams={params}
      />

      <ManageLanguagesModal
        open={langsOpen}
        onClose={() => setLangsOpen(false)}
      />
    </div>
  );
}
