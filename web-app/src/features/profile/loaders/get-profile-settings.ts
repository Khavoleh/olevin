import { requestOrRedirect } from "@shared/api";
import { ApiError } from "@shared/helpers";
import { cache } from "react";
import { getSettings } from "../api/profile";
import type { GetSettingsResponse } from "../api/profile.schemas";

type ProfileSettings = { settings: GetSettingsResponse } | { error: string };

/** Every setting of the user, or the error of the API. */
export const getProfileSettings = cache(
	async (language: string): Promise<ProfileSettings> => {
		const response = await requestOrRedirect(getSettings, language);

		return response.data?.data
			? { settings: response.data.data }
			: { error: new ApiError(response).message };
	},
);
