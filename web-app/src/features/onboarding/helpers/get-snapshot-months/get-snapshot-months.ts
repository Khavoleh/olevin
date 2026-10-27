/** Year and month (1–12) of a snapshot. */
export interface SnapshotMonth {
	year: number;
	month: number;
}

/** Latest month whose snapshot day has come in the time zone, then the 2 months before it. */
export const getSnapshotMonths = (
	now: Date,
	timeZone: string,
	snapshotDay: number,
): SnapshotMonth[] => {
	const parts = new Intl.DateTimeFormat("en", {
		timeZone,
		year: "numeric",
		month: "numeric",
		day: "numeric",
	}).formatToParts(now);
	const year = Number(parts.find((part) => part.type === "year")?.value);
	const month = Number(parts.find((part) => part.type === "month")?.value);
	const day = Number(parts.find((part) => part.type === "day")?.value);
	const daysInMonth = new Date(Date.UTC(year, month, 0)).getUTCDate();
	const latest = day < Math.min(snapshotDay, daysInMonth) ? 1 : 0;

	return [0, 1, 2].map((back) => {
		const date = new Date(Date.UTC(year, month - 1 - latest - back, 1));

		return { year: date.getUTCFullYear(), month: date.getUTCMonth() + 1 };
	});
};
