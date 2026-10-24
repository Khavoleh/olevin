import { requestOrRedirect } from "@shared/api";
import { ApiError } from "@shared/helpers";
import { notFound } from "next/navigation";
import { cache } from "react";
import { getSnapshotMonth } from "../api/snapshots";
import type {
	CalendarMonth,
	GetSnapshotMonthResponse,
} from "../api/snapshots.schemas";

type MonthAmounts = { amounts: GetSnapshotMonthResponse } | { error: string };

/** Saved amounts of the snapshot or the estimate of the month, or the error of the API; a month without them is not found. */
export const getMonthAmounts = cache(
	async (
		language: string,
		year: number,
		month: CalendarMonth,
	): Promise<MonthAmounts> => {
		const response = await requestOrRedirect(
			() => getSnapshotMonth(year, month),
			language,
		);

		if (response.status === 404) notFound();

		return response.data?.data
			? { amounts: response.data.data }
			: { error: new ApiError(response).message };
	},
);
