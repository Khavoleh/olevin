import { CALENDAR_MONTHS } from "@shared/constants";

/** Month of a snapshot with its year, as "September 2026". */
export const formatSnapshotMonth = (
	year: number,
	month: (typeof CALENDAR_MONTHS)[number],
	language: string,
) => {
	const name = new Intl.DateTimeFormat(language, {
		month: "long",
		timeZone: "UTC",
	}).format(new Date(Date.UTC(year, CALENDAR_MONTHS.indexOf(month), 1)));

	return `${name} ${year}`;
};
