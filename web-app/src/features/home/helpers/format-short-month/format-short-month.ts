import { CalendarMonth } from "../../api/home.schemas";

/** Short name of a month without the year ("Apr"). */
export const formatShortMonth = (
	year: number,
	month: CalendarMonth,
	language: string,
) =>
	new Intl.DateTimeFormat(language, {
		month: "short",
		timeZone: "UTC",
	}).format(
		new Date(Date.UTC(year, Object.values(CalendarMonth).indexOf(month), 1)),
	);
