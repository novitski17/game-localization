import { api } from "@/lib/api";
import type { AuthUser, LoginPayload } from "@/types";

export async function login(payload: LoginPayload): Promise<AuthUser> {
  const { data } = await api.post<AuthUser>("/auth/login", payload);
  return data;
}

export async function me(): Promise<AuthUser> {
  const { data } = await api.get<AuthUser>("/auth/me");
  return data;
}

export async function logout(): Promise<void> {
  await api.post("/auth/logout");
}

export async function register(payload: {
  email: string;
  password: string;
}): Promise<AuthUser> {
  const { data } = await api.post<AuthUser>("/auth/register", payload);
  return data;
}
