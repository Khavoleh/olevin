"use server";

import { requestOrRedirect } from "@shared/api";
import { ApiError } from "@shared/helpers";
import { refresh } from "next/cache";
import { updateTimezone } from "../api/profile";
import { UpdateTimezoneBody } from "../api/profile.zod";

/** Saves the new time zone and shows the profile with it; returns the error of the API on a failure. */
export async function saveTimezone(
	language: string,
	timezone: string,
): Promise<string | undefined> {
	const body = UpdateTimezoneBody.parse({ timezone });
	const response = await requestOrRedirect(
		() => updateTimezone(body),
		language,
	);

	if (response.status !== 200) return new ApiError(response).message;

	refresh();
}
