"use server";

import { requestOrRedirect } from "@shared/api";
import { ApiError } from "@shared/helpers";
import { refresh } from "next/cache";
import { updateBaseCurrency } from "../api/profile";
import type { CurrencyCode } from "../api/profile.schemas";
import { UpdateBaseCurrencyBody } from "../api/profile.zod";

/** Saves the new base currency and shows the profile with it; returns the error of the API on a failure. */
export async function saveBaseCurrency(
	language: string,
	baseCurrency: CurrencyCode,
): Promise<string | undefined> {
	const body = UpdateBaseCurrencyBody.parse({ baseCurrency });
	const response = await requestOrRedirect(
		() => updateBaseCurrency(body),
		language,
	);

	if (response.status !== 200) return new ApiError(response).message;

	refresh();
}
