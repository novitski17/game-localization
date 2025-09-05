import { Routes, Route, Link } from "react-router-dom";
import ProtectedRoute from "@/app/ProtectedRoute";
import AdminRoute from "@/app/AdminRoute";
import LoginPage from "@/pages/LoginPage";
import RegisterPage from "@/pages/RegisterPage";
import Home from "@/pages/LocalizationTablePage";
import About from "@/pages/About";
import AdminLanguagesPage from "@/pages/AdminLanguagesPage";
import { useMe, useLogout } from "@/features/auth/hooks";

export default function App() {
  const { data } = useMe();
  const logout = useLogout();

  return (
    <div className="p-6 max-w-[1280px] mx-auto">
      <nav className="mb-6 flex gap-4 items-center">
        <Link className="text-blue-600 hover:underline" to="/">
          Table
        </Link>
        <Link className="text-blue-600 hover:underline" to="/about">
          About
        </Link>
        {data?.role === "Admin" && (
          <Link className="text-blue-600 hover:underline" to="/admin/languages">
            Admin
          </Link>
        )}
        <div className="ml-auto flex gap-3 items-center">
          {!data ? (
            <Link className="text-blue-600 hover:underline" to="/login">
              Login
            </Link>
          ) : (
            <button
              onClick={() => logout.mutate()}
              className="rounded border px-3 py-1 text-sm"
            >
              Sign out
            </button>
          )}
        </div>
      </nav>

      <Routes>
        <Route path="/login" element={<LoginPage />} />
        <Route path="/register" element={<RegisterPage />} />

        <Route element={<ProtectedRoute />}>
          <Route path="/" element={<Home />} />
          <Route path="/about" element={<About />} />

          <Route element={<AdminRoute />}>
            <Route path="/admin/languages" element={<AdminLanguagesPage />} />
          </Route>
        </Route>
      </Routes>
    </div>
  );
}
