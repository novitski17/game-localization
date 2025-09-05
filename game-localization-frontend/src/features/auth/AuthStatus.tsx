import { useMe } from "./hooks";

export function AuthStatus() {
  const { data, isLoading, error, refetch } = useMe();

  if (isLoading) return <div>Loading...</div>;

  const status =
    error &&
    typeof error === "object" &&
    "response" in error &&
    error.response &&
    typeof error.response === "object" &&
    "status" in error.response
      ? (error.response as { status?: number }).status
      : undefined;

  if (status === 401) {
    return (
      <div className="p-3 rounded border border-slate-200 bg-slate-50">
        Not authorized (401)
        <button
          onClick={() => refetch()}
          className="ml-3 px-3 py-1.5 rounded border border-slate-300 hover:bg-slate-100"
        >
          Check again
        </button>
      </div>
    );
  }

  if (error) {
    return (
      <div className="p-3 rounded bg-red-50 text-red-700">
        Error: {(error as Error).message}
      </div>
    );
  }

  return (
    <div className="p-3 rounded border border-emerald-200 bg-emerald-50">
      Authorized
      <pre className="mt-2 text-sm">{JSON.stringify(data, null, 2)}</pre>
    </div>
  );
}
