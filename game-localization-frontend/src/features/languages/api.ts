import { api } from "@/lib/api";
import type { Language } from "@/types";

export async function getLanguages(opts?: {
  includeDisabled?: boolean;
}): Promise<Language[]> {
  const includeDisabled = opts?.includeDisabled ?? true;
  const { data } = await api.get<Language[]>("/languages", {
    params: includeDisabled ? { IncludeDisabled: true } : undefined,
  });
  return data;
}

export async function setLanguageEnabled(
  id: string,
  isEnabled: boolean
): Promise<void> {
  await api.patch(`/languages/${id}/status`, { isEnabled });
}

export async function createLanguage(payload: {
  code: string;
  name: string;
  isEnabled?: boolean;
}): Promise<Language> {
  const { data } = await api.post<Language>("/languages", {
    ...payload,
    isEnabled: payload.isEnabled ?? true,
  });
  return data;
}

export async function updateLanguage(
  id: string,
  payload: { code: string; name: string }
): Promise<Language> {
  const { data } = await api.put<Language>(`/languages/${id}`, payload);
  return data;
}

export async function deleteLanguage(id: string): Promise<void> {
  await api.delete(`/languages/${id}`);
}
