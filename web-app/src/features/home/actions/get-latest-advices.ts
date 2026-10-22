"use server";

import { PAGE_URLS } from "@shared/constants";
import { ApiError, getLanguageUrl } from "@shared/helpers";
import { redirect } from "next/navigation";
import { getAdvices } from "../api/home";
import type { GetAdvicesResponse } from "../api/home.schemas";

type LatestAdvices = { advices: GetAdvicesResponse } | { error: string };

/** Advices for the latest snapshot, or the error of the API. Without a session opens sign-in, without snapshots the onboarding. */
export async function getLatestAdvices(
	language: string,
): Promise<LatestAdvices> {
	const response = await getAdvices().catch((error) => {
		if (error instanceof ApiError && error.status === 401) {
			redirect(getLanguageUrl(PAGE_URLS.SIGN_IN, language));
		}

		throw error;
	});

	if (response.status === 404) {
		redirect(getLanguageUrl(PAGE_URLS.ONBOARDING, language));
	}

	return response.data.data
		? { advices: response.data.data }
		: { error: new ApiError(response).message };
}
