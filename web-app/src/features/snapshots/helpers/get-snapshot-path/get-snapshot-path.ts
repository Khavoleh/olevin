import { CALENDAR_MONTHS, PAGE_URLS } from "@shared/constants";

/** Path of the page that changes the month, as `/snapshots/2026-09`. */
export const getSnapshotPath = (
	year: number,
	month: (typeof CALENDAR_MONTHS)[number],
) =>
	`${PAGE_URLS.SNAPSHOTS}/${year}-${String(CALENDAR_MONTHS.indexOf(month) + 1).padStart(2, "0")}`;
