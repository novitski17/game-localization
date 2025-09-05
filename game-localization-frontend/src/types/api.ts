export interface ApiProblemDetails {
  type?: string;
  title?: string;
  status?: number;
  detail?: string;
  errors?: Record<string, string[]>;
}

export type ApiErrorPayload =
  | ApiProblemDetails
  | { message?: string }
  | unknown;
