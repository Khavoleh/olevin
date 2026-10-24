import { requestOrRedirect } from "@shared/api";
import { ApiError } from "@shared/helpers";
import { cache } from "react";
import { getSnapshots } from "./api/history";
import type { GetSnapshotsResponse } from "./api/history.schemas";

type SnapshotHistory = { history: GetSnapshotsResponse } | { error: string };

/** Every snapshot and estimate of the user, or the error of the API. */
export const getSnapshotHistory = cache(
	async (language: string): Promise<SnapshotHistory> => {
		const response = await requestOrRedirect(getSnapshots, language);

		return response.data?.data
			? { history: response.data.data }
			: { error: new ApiError(response).message };
	},
);
