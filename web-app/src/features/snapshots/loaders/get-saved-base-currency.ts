import { requestOrRedirect } from "@shared/api";
import { ApiError } from "@shared/helpers";
import { cache } from "react";
import { getSnapshotBaseCurrency } from "../api/snapshots";
import type { CurrencyCode } from "../api/snapshots.schemas";

type SavedBaseCurrency = { baseCurrency: CurrencyCode } | { error: string };

/** Base currency from the settings, or the error of the API. */
export const getSavedBaseCurrency = cache(
	async (language: string): Promise<SavedBaseCurrency> => {
		const response = await requestOrRedirect(getSnapshotBaseCurrency, language);

		return response.data?.data
			? { baseCurrency: response.data.data }
			: { error: new ApiError(response).message };
	},
);
