"use server";

import { requestOrRedirect } from "@shared/api";
import { PAGE_URLS } from "@shared/constants";
import { getLanguageUrl } from "@shared/helpers";
import { redirect } from "next/navigation";
import { getBaseCurrency } from "../api/onboarding";
import type { CurrencyCode } from "../api/onboarding.schemas";

/** Base currency saved in the settings. Without settings, opens the step the user continues from. */
export async function getSavedBaseCurrency(
	language: string,
): Promise<CurrencyCode> {
	const { status, data } = await requestOrRedirect(getBaseCurrency, language);

	if (status === 404) {
		redirect(getLanguageUrl(PAGE_URLS.ONBOARDING, language));
	}

	return data.data;
}
