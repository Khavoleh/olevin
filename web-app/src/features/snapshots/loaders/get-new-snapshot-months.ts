import { requestOrRedirect } from "@shared/api";
import { ApiError } from "@shared/helpers";
import { cache } from "react";
import { getOpenMonths } from "../api/snapshots";
import type { GetOpenMonthsResponse } from "../api/snapshots.schemas";

type NewSnapshotMonths =
	| { openMonths: GetOpenMonthsResponse }
	| { error: string };

/** Months a new snapshot may be created for, or the error of the API. */
export const getNewSnapshotMonths = cache(
	async (language: string): Promise<NewSnapshotMonths> => {
		const response = await requestOrRedirect(getOpenMonths, language);

		return response.data?.data
			? { openMonths: response.data.data }
			: { error: new ApiError(response).message };
	},
);
