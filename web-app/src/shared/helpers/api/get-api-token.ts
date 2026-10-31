import { LogtoClientError, LogtoRequestError } from "@logto/next";
import { getAccessToken, getAccessTokenRSC } from "@logto/next/server-actions";
import { cache } from "react";
import { getLogtoConfig } from "../auth/get-logto-config";
import { getApiResource } from "./get-api-resource";

/** The same object within one render of the Server Components, a new one elsewhere. */
const getRenderScope = cache(() => ({}));

/** Whether Server Components are rendering. */
const getIsRendering = () => {
	const scope = getRenderScope();

	return scope === getRenderScope();
};

/** Whether Logto rejected the session, as opposed to being unreachable. */
const getIsSessionError = (error: unknown): boolean =>
	error instanceof LogtoClientError || error instanceof LogtoRequestError;

/** Access token for the API, or `null` without a valid session. Throws when Logto is unreachable. */
export const getApiToken = cache(async (): Promise<string | null> => {
	const getToken = getIsRendering() ? getAccessTokenRSC : getAccessToken;

	try {
		return await getToken(getLogtoConfig(), getApiResource());
	} catch (error) {
		if (!getIsSessionError(error)) {
			throw error;
		}

		console.error("Could not get the API access token", error);

		return null;
	}
});
