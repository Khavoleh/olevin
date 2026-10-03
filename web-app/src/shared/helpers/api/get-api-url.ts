import { z } from "zod";

/** Server environment variables that the API client needs. */
const apiEnvSchema = z.object({
  API_URL: z.url(),
});

/**
 * Reads the API address from the server environment.
 */
export function getApiUrl(): string {
  return apiEnvSchema.parse(process.env).API_URL.replace(/\/$/, "");
}
