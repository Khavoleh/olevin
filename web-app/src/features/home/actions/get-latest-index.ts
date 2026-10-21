"use server";

import { PAGE_URLS } from "@shared/constants";
import { ApiError, getLanguageUrl } from "@shared/helpers";
import { redirect } from "next/navigation";
import { getIndex } from "../api/home";
import type { GetIndexResponse } from "../api/home.schemas";

type LatestIndex = { index: GetIndexResponse } | { error: string };

/** Latest index of the user, or the error of the API. Without a session opens sign-in, without snapshots the onboarding. */
export async function getLatestIndex(language: string): Promise<LatestIndex> {
	const response = await getIndex().catch((error) => {
		if (error instanceof ApiError && error.status === 401) {
			redirect(getLanguageUrl(PAGE_URLS.SIGN_IN, language));
		}

		throw error;
	});

	if (response.status === 404) {
		redirect(getLanguageUrl(PAGE_URLS.ONBOARDING, language));
	}

	return response.data.data
		? { index: response.data.data }
		: { error: new ApiError(response).message };
}
