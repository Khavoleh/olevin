import { getApiUrl } from "./get-api-url";

/**
 * Transport of the Orval clients in `features/<tag>/api`: resolves the path against the API address.
 */
export async function apiRequest<T>(
  path: string,
  options: RequestInit,
): Promise<T> {
  const response = await fetch(`${getApiUrl()}${path}`, options);
  const body = await response.text();

  return {
    data: body ? JSON.parse(body) : undefined,
    status: response.status,
    headers: response.headers,
  } as T;
}
