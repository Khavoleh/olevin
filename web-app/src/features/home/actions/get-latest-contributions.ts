"use server";

import { PAGE_URLS } from "@shared/constants";
import { ApiError, getLanguageUrl } from "@shared/helpers";
import { redirect } from "next/navigation";
import { getContributions } from "../api/home";
import type { GetContributionsResponse } from "../api/home.schemas";

type LatestContributions =
	| { contributions: GetContributionsResponse | null }
	| { error: string };

/** What changed the index since the previous snapshot, `null` for the first one, or the error of the API. Without a session opens sign-in, without snapshots the onboarding. */
export async function getLatestContributions(
	language: string,
): Promise<LatestContributions> {
	const response = await getContributions().catch((error) => {
		if (error instanceof ApiError && error.status === 401) {
			redirect(getLanguageUrl(PAGE_URLS.SIGN_IN, language));
		}

		throw error;
	});

	if (response.status === 404) {
		redirect(getLanguageUrl(PAGE_URLS.ONBOARDING, language));
	}

	return response.data.error === null
		? { contributions: response.data.data }
		: { error: new ApiError(response).message };
}
