import { requestOrRedirect } from "@shared/api";
import { ApiError } from "@shared/helpers";
import { cache } from "react";
import { getIndex } from "../api/home";
import type { GetIndexResponse } from "../api/home.schemas";

type LatestIndex = { index: GetIndexResponse } | { error: string };

/** Latest index of the user, or the error of the API. */
export const getLatestIndex = cache(
	async (language: string): Promise<LatestIndex> => {
		const response = await requestOrRedirect(getIndex, language);

		return response.data.data
			? { index: response.data.data }
			: { error: new ApiError(response).message };
	},
);
