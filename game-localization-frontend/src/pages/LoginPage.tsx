import { useQuery } from "@tanstack/react-query";
import { qk } from "@/lib/queryKeys";
import { me } from "@/features/auth/api";
import { LoginForm } from "@/features/auth/components/LoginForm";

export default function LoginPage() {
  useQuery({ queryKey: qk.me, queryFn: me, retry: 0 });

  return (
    <div className="min-h-screen bg-gray-50 p-6">
      <div className="mx-auto max-w-[1280px]">
        <LoginForm />
      </div>
    </div>
  );
}
