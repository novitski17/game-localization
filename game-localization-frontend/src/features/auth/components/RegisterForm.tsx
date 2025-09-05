import { useState, type FormEvent } from "react";
import { useRegister } from "@/features/auth/hooks";
import { toast } from "@/utils/toast";

export function RegisterForm() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const register = useRegister();

  async function onSubmit(e: FormEvent) {
    e.preventDefault();
    try {
      await register.mutateAsync({ email, password });
      toast.success("Registration successful. You can now log in.");
      setEmail("");
      setPassword("");
    } catch {
      toast.error("Registration failed");
    }
  }

  return (
    <form
      onSubmit={onSubmit}
      className="mx-auto mt-16 w-full max-w-sm space-y-3 rounded border p-5 bg-white"
    >
      <h1 className="text-xl font-semibold">Register</h1>

      <label className="block text-sm">
        Email
        <input
          type="email"
          className="mt-1 h-9 w-full rounded border px-2"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          placeholder="you@example.com"
          required
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
        />
      </label>

      <button
        type="submit"
        disabled={register.isPending}
        className="h-10 w-full rounded bg-blue-600 text-white disabled:opacity-60"
      >
        {register.isPending ? "Registering…" : "Register"}
      </button>

      {register.isError && (
        <div className="text-sm text-red-600">Failed to register</div>
      )}
    </form>
  );
}
