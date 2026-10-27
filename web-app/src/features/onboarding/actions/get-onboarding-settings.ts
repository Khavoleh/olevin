"use server";

import { requestOrRedirect } from "@shared/api";
import { PAGE_URLS } from "@shared/constants";
import { getLanguageUrl } from "@shared/helpers";
import { redirect } from "next/navigation";
import { getSavedSettings } from "../api/onboarding";
import type { GetSavedSettingsResponse } from "../api/onboarding.schemas";

/** Settings saved in the first step. Without settings, opens the step the user continues from. */
export async function getOnboardingSettings(
	language: string,
): Promise<GetSavedSettingsResponse | null> {
	const { status, data } = await requestOrRedirect(getSavedSettings, language);

	if (status === 404) {
		redirect(getLanguageUrl(PAGE_URLS.ONBOARDING, language));
	}

	return data.data;
}
