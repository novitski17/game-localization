export type UserRole = "Admin" | "Member";

export interface AuthUser {
  id: string;
  email: string;
  role: UserRole;
}

export interface LoginPayload {
  email: string;
  password: string;
}
