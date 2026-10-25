"use server";

import { requestOrRedirect } from "@shared/api";
import { ApiError } from "@shared/helpers";
import { refresh } from "next/cache";
import { withdrawResearchConsent as deleteResearchConsent } from "../api/profile";

/** Withdraws the research consent and shows the profile without the research; returns the error of the API on a failure. */
export async function withdrawResearchConsent(
	language: string,
): Promise<string | undefined> {
	const response = await requestOrRedirect(deleteResearchConsent, language);

	if (response.status !== 204) return new ApiError(response).message;

	refresh();
}
