import { requestOrRedirect } from "@shared/api";
import { ApiError } from "@shared/helpers";
import { cache } from "react";
import { getComposition } from "../api/home";
import type { GetCompositionResponse } from "../api/home.schemas";

type LatestComposition =
	| { composition: GetCompositionResponse }
	| { error: string };

/** What the latest index is made of, or the error of the API. */
export const getLatestComposition = cache(
	async (language: string): Promise<LatestComposition> => {
		const response = await requestOrRedirect(getComposition, language);

		return response.data.data
			? { composition: response.data.data }
			: { error: new ApiError(response).message };
	},
);
