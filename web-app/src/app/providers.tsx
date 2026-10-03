"use client";

import { makeQueryClient } from "@shared/helpers";
import { QueryClientProvider } from "@tanstack/react-query";
import { type ReactNode, useState } from "react";

interface ProvidersProps {
  children: ReactNode;
}

/** Browser-side context of the app. */
const Providers = ({ children }: Readonly<ProvidersProps>) => {
  const [queryClient] = useState(() => makeQueryClient());

  return (
    <QueryClientProvider client={queryClient}>{children}</QueryClientProvider>
  );
};

export default Providers;
