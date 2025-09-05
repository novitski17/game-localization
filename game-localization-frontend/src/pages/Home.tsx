import { AuthStatus } from "../features/auth/AuthStatus";

export default function Home() {
  return (
    <div className="space-y-4">
      <h1 className="text-2xl font-bold">Home</h1>
      <AuthStatus />
    </div>
  );
}
