import { requestOrRedirect } from "@shared/api";
import { ApiError } from "@shared/helpers";
import { cache } from "react";
import { getContributions } from "../api/home";
import type { GetContributionsResponse } from "../api/home.schemas";

type LatestContributions =
	| { contributions: GetContributionsResponse | null }
	| { error: string };

/** What changed the index since the previous snapshot, `null` for the first one, or the error of the API. */
export const getLatestContributions = cache(
	async (language: string): Promise<LatestContributions> => {
		const response = await requestOrRedirect(getContributions, language);

		return response.data.error === null
			? { contributions: response.data.data }
			: { error: new ApiError(response).message };
	},
);
