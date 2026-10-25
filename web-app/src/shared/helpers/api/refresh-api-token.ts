import { clearAccessToken, getAccessToken } from "@logto/next/server-actions";
import { getLogtoConfig } from "../auth/get-logto-config";
import { getApiResource } from "./get-api-resource";

/** Replaces the saved access token with a new one, whose claims hold the latest profile. */
export async function refreshApiToken(): Promise<void> {
	const config = getLogtoConfig();

	await clearAccessToken(config);
	await getAccessToken(config, getApiResource()).catch(() => undefined);
}
