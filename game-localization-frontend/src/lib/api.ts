import axios, { AxiosError } from "axios";

const baseURL = import.meta.env.VITE_API_BASE_URL;

if (!baseURL) {
  console.warn("VITE API_BASE_URL not set. Check .env");
}

export const api = axios.create({
  baseURL,
  withCredentials: true,
  headers: { "Content-Type": "application/json" },
});

api.interceptors.response.use(
  (r) => r,
  (error: AxiosError) => Promise.reject(error)
);
