"use server";

import { PAGE_URLS } from "@shared/constants";
import { ApiError, getApiToken } from "@shared/helpers";
import { redirect } from "next/navigation";
import { fetchMe } from "./fetch-me";

/**
 * Query function of the browser: refetches the user through the server, so the access token never reaches the browser.
 * The first load is prefetched on the server, see `MeInfo`.
 */
export async function getMe(): ReturnType<typeof fetchMe> {
	const token = await getApiToken();

	if (!token) {
		redirect(PAGE_URLS.SIGN_IN);
	}

	try {
		return await fetchMe(token);
	} catch (error) {
		if (error instanceof ApiError && error.status === 401) {
			redirect(PAGE_URLS.SIGN_IN);
		}

		throw error;
	}
}
