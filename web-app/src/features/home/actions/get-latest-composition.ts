"use server";

import { PAGE_URLS } from "@shared/constants";
import { ApiError, getLanguageUrl } from "@shared/helpers";
import { redirect } from "next/navigation";
import { getComposition } from "../api/home";
import type { GetCompositionResponse } from "../api/home.schemas";

type LatestComposition =
	| { composition: GetCompositionResponse }
	| { error: string };

/** What the latest index is made of, or the error of the API. Without a session opens sign-in, without snapshots the onboarding. */
export async function getLatestComposition(
	language: string,
): Promise<LatestComposition> {
	const response = await getComposition().catch((error) => {
		if (error instanceof ApiError && error.status === 401) {
			redirect(getLanguageUrl(PAGE_URLS.SIGN_IN, language));
		}

		throw error;
	});

	if (response.status === 404) {
		redirect(getLanguageUrl(PAGE_URLS.ONBOARDING, language));
	}

	return response.data.data
		? { composition: response.data.data }
		: { error: new ApiError(response).message };
}
