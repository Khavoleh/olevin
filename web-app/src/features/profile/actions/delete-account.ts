"use server";

import { getLogtoContext } from "@logto/next/server-actions";
import { PAGE_URLS } from "@shared/constants";
import {
	getLanguageUrl,
	getLogtoConfig,
	getLogtoManagementApi,
} from "@shared/helpers";
import { redirect } from "next/navigation";
import { signOut } from "./sign-out";

/** Deletes the account in Logto, whose webhook then deletes the data, and signs the user out; returns the error on a failure. */
export async function deleteAccount(
	language: string,
): Promise<string | undefined> {
	const { claims } = await getLogtoContext(getLogtoConfig());

	if (!claims?.sub) redirect(getLanguageUrl(PAGE_URLS.SIGN_IN, language));

	const response = await getLogtoManagementApi()
		.apiClient.DELETE("/api/users/{userId}", {
			params: { path: { userId: claims.sub } },
		})
		.then(
			({ response }) => response,
			() => undefined,
		);

	if (!response || (!response.ok && response.status !== 404)) {
		return "The account could not be deleted. Try again later, please.";
	}

	await signOut();
}
