/** Year and month (1–12) of a snapshot. */
export interface SnapshotMonth {
	year: number;
	month: number;
}

/** Current month in the time zone, then the 2 months before it. */
export const getSnapshotMonths = (
	now: Date,
	timeZone: string,
): SnapshotMonth[] => {
	const parts = new Intl.DateTimeFormat("en", {
		timeZone,
		year: "numeric",
		month: "numeric",
	}).formatToParts(now);
	const year = Number(parts.find((part) => part.type === "year")?.value);
	const month = Number(parts.find((part) => part.type === "month")?.value);

	return [0, 1, 2].map((back) => {
		const date = new Date(Date.UTC(year, month - 1 - back, 1));

		return { year: date.getUTCFullYear(), month: date.getUTCMonth() + 1 };
	});
};
