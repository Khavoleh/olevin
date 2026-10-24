"use server";

import { requestOrRedirect } from "@shared/api";
import { ApiError } from "@shared/helpers";
import { updateLocale } from "../api/profile";
import { UpdateLocaleBody } from "../api/profile.zod";

/** Saves the chosen interface language of the user; returns the error of the API on a failure. */
export async function saveLanguage(
	language: string,
	selected: string,
): Promise<string | undefined> {
	const body = UpdateLocaleBody.parse({ locale: selected.toUpperCase() });
	const response = await requestOrRedirect(() => updateLocale(body), language);

	if (response.status !== 200) return new ApiError(response).message;
}
