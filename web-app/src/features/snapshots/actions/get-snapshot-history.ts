"use server";

import { PAGE_URLS } from "@shared/constants";
import { ApiError, getLanguageUrl } from "@shared/helpers";
import { redirect } from "next/navigation";
import { getSnapshots } from "../api/snapshots";
import type { GetSnapshotsResponse } from "../api/snapshots.schemas";

type SnapshotHistory = { history: GetSnapshotsResponse } | { error: string };

/** Every snapshot and estimate of the user, or the error of the API. Without a session opens sign-in, without snapshots the onboarding. */
export async function getSnapshotHistory(
	language: string,
): Promise<SnapshotHistory> {
	const response = await getSnapshots().catch((error) => {
		if (error instanceof ApiError && error.status === 401) {
			redirect(getLanguageUrl(PAGE_URLS.SIGN_IN, language));
		}

		throw error;
	});

	if (response.status === 404) {
		redirect(getLanguageUrl(PAGE_URLS.ONBOARDING, language));
	}

	return response.data.data
		? { history: response.data.data }
		: { error: new ApiError(response).message };
}
