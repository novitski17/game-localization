export interface LocalizationRow {
  keyId: string;
  key: string;
  valuesByLanguage: Record<string, string | null>;
}

export interface RowsPage<T> {
  items: T[];
  page: number;
  pageSize: number;
  total: number;
}

export interface LocalizationTableResponse {
  languageCodes: string[];
  rows: RowsPage<LocalizationRow>;
}

export interface LocalizationTableParams {
  page?: number;
  pageSize?: number;
  includeDisabledLanguages?: boolean;
  search?: string;
}

export interface CreateKeyPayload {
  key: string;
}

export interface TranslationResponse {
  id: string;
  localizationKeyId: string;
  languageCode: string;
  value: string;
}
