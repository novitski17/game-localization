import { Navigate, Outlet } from "react-router-dom";
import { useQuery } from "@tanstack/react-query";
import { me } from "@/features/auth/api";
import { qk } from "@/lib/queryKeys";
import type { AuthUser } from "@/types";
import { isUnauthorized } from "@/utils/error";
import type { AxiosError } from "axios";

export default function AdminRoute() {
  const { data, isLoading, error } = useQuery<AuthUser, AxiosError>({
    queryKey: qk.me,
    queryFn: me,
    retry: 0,
  });

  if (isLoading) return <div className="p-6">Checking permissions…</div>;
  if (isUnauthorized(error) || !data) return <Navigate to="/login" replace />;
  if (data.role !== "Admin") return <Navigate to="/" replace />;

  return <Outlet />;
}
