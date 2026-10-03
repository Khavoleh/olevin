import { QueryClient } from "@tanstack/react-query";

/**
 * Data hydrated from the server counts as fresh for this long, so the browser does not refetch it on mount.
 */
const STALE_TIME_MS = 60_000;

/** Creates a query client with the app defaults. */
export function makeQueryClient(): QueryClient {
	return new QueryClient({
		defaultOptions: { queries: { staleTime: STALE_TIME_MS } },
	});
}
