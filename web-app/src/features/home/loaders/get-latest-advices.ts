import { requestOrRedirect } from "@shared/api";
import { ApiError } from "@shared/helpers";
import { cache } from "react";
import { getAdvices } from "../api/home";
import type { GetAdvicesResponse } from "../api/home.schemas";

type LatestAdvices = { advices: GetAdvicesResponse } | { error: string };

/** Advices for the latest snapshot, or the error of the API. */
export const getLatestAdvices = cache(
	async (language: string): Promise<LatestAdvices> => {
		const response = await requestOrRedirect(getAdvices, language);

		return response.data.data
			? { advices: response.data.data }
			: { error: new ApiError(response).message };
	},
);
