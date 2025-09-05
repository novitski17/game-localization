import { type FormEvent, useState } from "react";
import { useLogin, useLogout } from "@/features/auth/hooks";
import { useQueryClient } from "@tanstack/react-query";
import { qk } from "@/lib/queryKeys";

export function LoginForm() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const login = useLogin();
  const logout = useLogout();
  const qc = useQueryClient();

  const [localError, setLocalError] = useState<string | null>(null);

  const me = !!qc.getQueryData(qk.me);

  async function onSubmit(e: FormEvent) {
    e.preventDefault();
    setLocalError(null);
    try {
      await login.mutateAsync({ email, password });
    } catch {
      setLocalError("Error while trying to sign in");
    }
  }

  return (
    <form
      onSubmit={onSubmit}
      className="mx-auto mt-16 w-full max-w-sm space-y-3 rounded border p-5 bg-white"
    >
      <h1 className="text-xl font-semibold">Sign in</h1>

      <label className="block text-sm">
        Email
        <input
          type="email"
          className="mt-1 h-9 w-full rounded border px-2"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          placeholder="you@example.com"
          required
          autoComplete="username"
        />
      </label>

      <label className="block text-sm">
        Password
        <input
          type="password"
          className="mt-1 h-9 w-full rounded border px-2"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          placeholder="••••••••"
          required
          autoComplete="current-password"
        />
      </label>

      <button
        type="submit"
        disabled={login.isPending}
        className="h-10 w-full rounded bg-blue-600 text-white disabled:opacity-60"
      >
        {login.isPending ? "Signing in…" : "Sign in"}
      </button>

      {localError ? (
        <div className="text-sm text-red-600">{localError}</div>
      ) : null}
      {login.isError ? (
        <div className="text-sm text-red-600">Invalid email or password</div>
      ) : null}

      <div className="flex flex-col gap-2 pt-2">
        <a
          href="/"
          className="inline-block w-full text-center h-10 leading-10 rounded border"
        >
          Go to table
        </a>

        <a
          href="/register"
          className="inline-block w-full text-center h-10 leading-10 rounded border"
        >
          Sign up
        </a>

        {me ? (
          <button
            type="button"
            onClick={() => logout.mutate()}
            className="h-10 w-full rounded border"
          >
            Sign out
          </button>
        ) : null}
      </div>
    </form>
  );
}
