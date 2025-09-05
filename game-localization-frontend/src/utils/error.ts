import { isAxiosError } from "axios";
import type { ApiErrorPayload, ApiProblemDetails } from "@/types";

export function getErrorMessage(e: unknown): string {
  if (typeof e === "string") return e;

  if (e instanceof Error && !isAxiosError(e)) {
    return e.message || "Unexpected error";
  }

  if (isAxiosError<ApiErrorPayload>(e)) {
    const data = e.response?.data;

    const msg = (data as { message?: string } | undefined)?.message;
    if (typeof msg === "string" && msg.trim() !== "") return msg;

    const pd = data as ApiProblemDetails | undefined;
    if (pd?.detail) return `${pd.status ?? ""} ${pd.detail}`.trim();
    if (pd?.title) return `${pd.status ?? ""} ${pd.title}`.trim();
    if (pd?.errors) {
      const first = Object.values(pd.errors)[0];
      if (Array.isArray(first) && first.length) return first[0];
    }

    return e.message || "Unexpected error";
  }

  return "Unexpected error";
}

export function getHttpStatus(e: unknown): number | undefined {
  return isAxiosError<ApiErrorPayload>(e) ? e.response?.status : undefined;
}

export function isUnauthorized(e: unknown): boolean {
  return getHttpStatus(e) === 401;
}
