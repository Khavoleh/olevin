import { getAccessToken } from "@logto/next/server-actions";
import { getLogtoConfig } from "../auth/get-logto-config";
import { getApiResource } from "./get-api-resource";

/**
 * Access token for the API in Server Actions and Route Handlers.
 */
export async function getApiToken(): Promise<string | null> {
  try {
    return await getAccessToken(getLogtoConfig(), getApiResource());
  } catch {
    return null;
  }
}
