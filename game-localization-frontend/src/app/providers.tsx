import type { PropsWithChildren } from "react";
import { QueryClientProvider } from "@tanstack/react-query";
import { queryClient } from "@/lib/queryClient";
import { Toasts } from "@/components/Toasts";
import { BrowserRouter } from "react-router-dom";

export function AppProviders({ children }: PropsWithChildren) {
  return (
    <QueryClientProvider client={queryClient}>
      <BrowserRouter>
        {children}
        <Toasts />
      </BrowserRouter>
    </QueryClientProvider>
  );
}
