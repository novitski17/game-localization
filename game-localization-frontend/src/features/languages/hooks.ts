import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import {
  getLanguages,
  createLanguage,
  updateLanguage,
  setLanguageEnabled,
  deleteLanguage,
} from "./api";
import { qk } from "@/lib/queryKeys";
import { getErrorMessage } from "@/utils/error";
import { toast } from "@/utils/toast";
import type { Language } from "@/types";

export function useLanguages(opts?: { includeDisabled?: boolean }) {
  const includeDisabled = opts?.includeDisabled ?? true;
  return useQuery<Language[], Error>({
    queryKey: [...qk.languages, { includeDisabled }],
    queryFn: () => getLanguages({ includeDisabled }),
  });
}

export function useToggleLanguage() {
  const qc = useQueryClient();
  return useMutation<void, Error, { id: string; isEnabled: boolean }>({
    mutationFn: ({ id, isEnabled }) => setLanguageEnabled(id, isEnabled),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: qk.languages });
      qc.invalidateQueries({ queryKey: ["localization-table"] });
      toast.success("Language updated");
    },
    onError: (e) => toast.error(getErrorMessage(e)),
  });
}

export function useCreateLanguage() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: createLanguage,
    onSuccess: () => {
      toast.success("Language created");
      qc.invalidateQueries({ queryKey: qk.languages });
      qc.invalidateQueries({ queryKey: ["localization-table"] });
    },
    onError: () => toast.error("Failed to create language"),
  });
}

export function useUpdateLanguage() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: ({
      id,
      code,
      name,
    }: {
      id: string;
      code: string;
      name: string;
    }) => updateLanguage(id, { code, name }),
    onSuccess: () => {
      toast.success("Language updated");
      qc.invalidateQueries({ queryKey: qk.languages });
      qc.invalidateQueries({ queryKey: ["localization-table"] });
    },
    onError: () => toast.error("Failed to update language"),
  });
}

export function useDeleteLanguage() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: ({ id }: { id: string }) => deleteLanguage(id),
    onSuccess: () => {
      toast.success("Language deleted");
      qc.invalidateQueries({ queryKey: qk.languages });
      qc.invalidateQueries({ queryKey: ["localization-table"] });
    },
    onError: () => toast.error("Failed to delete language"),
  });
}
