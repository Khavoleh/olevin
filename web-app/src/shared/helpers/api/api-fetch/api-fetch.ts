import type { z } from "zod";
import { ApiError } from "../api-error";
import { getApiUrl } from "../get-api-url";

/**
 * Calls the API from the server with the user's access token and validates the response.
 */
export async function apiFetch<T>(
  path: string,
  schema: z.ZodType<T>,
  token: string,
): Promise<T> {
  const response = await fetch(`${getApiUrl()}${path}`, {
    headers: { Accept: "application/json", Authorization: `Bearer ${token}` },
  });

  if (!response.ok) {
    throw new ApiError(response.status);
  }

  return schema.parse(await response.json());
}
