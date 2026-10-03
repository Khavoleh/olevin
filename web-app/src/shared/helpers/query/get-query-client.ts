import type { QueryClient } from "@tanstack/react-query";
import { cache } from "react";
import { makeQueryClient } from "./make-query-client";

/**
 * Returns the query client of the current server request.
 */
export const getQueryClient: () => QueryClient = cache(makeQueryClient);
