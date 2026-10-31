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

/** Access token for the API, or `null` without a session. */
export const getApiToken = cache(async (): Promise<string | null> => {
	const getToken = getIsRendering() ? getAccessTokenRSC : getAccessToken;

	try {
		return await getToken(getLogtoConfig(), getApiResource());
	} catch (error) {
		console.error("Could not get the API access token", error);

		return null;
	}
});
