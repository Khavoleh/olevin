import { CALENDAR_MONTHS } from "@shared/constants";

/** Short name of a month without the year ("Apr"). */
export const formatShortMonth = (
	year: number,
	month: (typeof CALENDAR_MONTHS)[number],
	language: string,
) =>
	new Intl.DateTimeFormat(language, {
		month: "short",
		timeZone: "UTC",
	}).format(new Date(Date.UTC(year, CALENDAR_MONTHS.indexOf(month), 1)));
