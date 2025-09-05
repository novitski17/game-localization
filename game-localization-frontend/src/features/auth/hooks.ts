import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { login, me, logout, register as apiRegister } from "./api";
import { qk } from "@/lib/queryKeys";
import { getErrorMessage } from "@/utils/error";
import { toast } from "@/utils/toast";
import type { LoginPayload, AuthUser } from "@/types";

export function useMe() {
  return useQuery<AuthUser, Error>({
    queryKey: qk.me,
    queryFn: me,
  });
}

export function useLogin() {
  const qc = useQueryClient();
  return useMutation<AuthUser, Error, LoginPayload>({
    mutationFn: login,
    onSuccess: (user) => {
      qc.setQueryData(qk.me, user);
      toast.success("Login successful");
    },
    onError: (e) => toast.error(getErrorMessage(e)),
  });
}

export function useLogout() {
  const qc = useQueryClient();
  return useMutation<void, Error, void>({
    mutationFn: logout,
    onSuccess: () => {
      qc.removeQueries({ queryKey: qk.me, exact: false });
      toast.success("Signed out");
    },
    onError: (e) => toast.error(getErrorMessage(e)),
  });
}

export function useRegister() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: apiRegister,
    onSuccess: (user) => {
      qc.setQueryData(qk.me, user);
    },
  });
}
