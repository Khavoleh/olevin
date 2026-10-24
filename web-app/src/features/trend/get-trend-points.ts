import { requestOrRedirect } from "@shared/api";
import { ApiError } from "@shared/helpers";
import { cache } from "react";
import { getTrend } from "./api/trend";
import type { TrendPeriod, TrendPointResponse } from "./api/trend.schemas";

type TrendPoints = { points: TrendPointResponse[] } | { error: string };

/** Every month of the period, the oldest first, or the error of the API. */
export const getTrendPoints = cache(
	async (language: string, period: TrendPeriod): Promise<TrendPoints> => {
		const response = await requestOrRedirect(() => getTrend(period), language);

		return response.data?.data
			? { points: response.data.data }
			: { error: new ApiError(response).message };
	},
);
