import { queryClient } from "@/lib/queryClient";
import { qk } from "@/lib/queryKeys";
import { toast } from "@/utils/toast";
import { getErrorMessage } from "@/utils/error";

import {
  useQuery,
  useMutation,
  keepPreviousData,
  useQueryClient,
} from "@tanstack/react-query";

import type {
  LocalizationTableParams,
  LocalizationTableResponse,
  CreateKeyPayload,
} from "@/types";
import {
  getLocalizationTable,
  createKey,
  updateTranslationByKeyLang,
  deleteKey,
} from "./api";

export function useLocalizationTable(params: LocalizationTableParams) {
  return useQuery<LocalizationTableResponse>({
    queryKey: qk.localizationTable(params),
    queryFn: () => getLocalizationTable(params),
    staleTime: 30_000,
    placeholderData: keepPreviousData,
  });
}

export function useCreateKey(tableParams: LocalizationTableParams) {
  return useMutation<void, unknown, CreateKeyPayload>({
    mutationFn: createKey,
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: qk.localizationTable(tableParams),
      });
      toast.success("Key created");
    },
    onError: (e) => toast.error(getErrorMessage(e)),
  });
}

type UpdateVars = {
  keyId: string;
  lang: string;
  value: string;
  tableParams: LocalizationTableParams;
};
type Ctx = { key: readonly unknown[]; snapshot?: LocalizationTableResponse };

export function useUpdateTranslation() {
  return useMutation<unknown, unknown, UpdateVars, Ctx>({
    mutationFn: ({ keyId, lang, value }) =>
      updateTranslationByKeyLang(keyId, lang, value),

    onMutate: async (vars) => {
      const key = qk.localizationTable(vars.tableParams);
      await queryClient.cancelQueries({ queryKey: key });
      const snapshot = queryClient.getQueryData<LocalizationTableResponse>(key);

      if (snapshot) {
        queryClient.setQueryData<LocalizationTableResponse>(key, {
          ...snapshot,
          rows: {
            ...snapshot.rows,
            items: snapshot.rows.items.map((r) =>
              r.keyId === vars.keyId
                ? {
                    ...r,
                    valuesByLanguage: {
                      ...r.valuesByLanguage,
                      [vars.lang]: vars.value,
                    },
                  }
                : r
            ),
          },
        });
      }

      return { key, snapshot };
    },

    onError: (e, _vars, ctx) => {
      if (ctx?.snapshot) queryClient.setQueryData(ctx.key, ctx.snapshot);
      toast.error(getErrorMessage(e));
    },

    onSettled: async (_data, _err, vars) => {
      await queryClient.invalidateQueries({
        queryKey: qk.localizationTable(vars.tableParams),
      });
    },
  });
}

export function useDeleteKey(invalidateParams: LocalizationTableParams) {
  const qc = useQueryClient();
  return useMutation<void, Error, { id: string }>({
    mutationFn: ({ id }) => deleteKey(id),
    onSuccess: () => {
      toast.success("Key deleted");
      qc.invalidateQueries({
        queryKey: qk.localizationTable(invalidateParams),
      });
    },
    onError: () => toast.error("Failed to delete key"),
  });
}
