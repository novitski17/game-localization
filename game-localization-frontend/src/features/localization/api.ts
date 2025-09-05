import { api } from "@/lib/api";
import type {
  LocalizationTableParams,
  LocalizationTableResponse,
  CreateKeyPayload,
  TranslationResponse,
} from "@/types";

type LocalizationTableQuery = {
  Page?: number;
  PageSize?: number;
  IncludeDisabledLanguages?: boolean;
  Search?: string;
};

function toQuery(params: LocalizationTableParams): LocalizationTableQuery {
  const q: LocalizationTableQuery = {};
  if (params.page != null) q.Page = params.page;
  if (params.pageSize != null) q.PageSize = params.pageSize;
  if (params.includeDisabledLanguages != null)
    q.IncludeDisabledLanguages = params.includeDisabledLanguages;
  if (params.search && params.search.trim() !== "")
    q.Search = params.search.trim();
  return q;
}

export async function getLocalizationTable(
  params: LocalizationTableParams
): Promise<LocalizationTableResponse> {
  const { data } = await api.get<LocalizationTableResponse>(
    "/localization-table",
    {
      params: toQuery(params),
    }
  );
  return data;
}

export async function createKey(payload: CreateKeyPayload): Promise<void> {
  await api.post("/localization-keys", payload);
}

export async function updateTranslationByKeyLang(
  keyId: string,
  lang: string,
  value: string
): Promise<TranslationResponse> {
  const { data } = await api.put<TranslationResponse>(
    `/translations/${keyId}/${lang}`,
    { value }
  );
  return data;
}

export async function deleteKey(id: string): Promise<void> {
  await api.delete(`/localization-keys/${id}`);
}
