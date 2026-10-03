import { getAccessTokenRSC } from "@logto/next/server-actions";
import { getLogtoConfig } from "../auth/get-logto-config";
import { getApiResource } from "./get-api-resource";

/**
 * Access token for the API in Server Components.
 */
export async function getApiTokenRSC(): Promise<string | null> {
  try {
    return await getAccessTokenRSC(getLogtoConfig(), getApiResource());
  } catch {
    return null;
  }
}
