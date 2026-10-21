import { CalendarMonth } from "../../api/home.schemas";

/** Month the change is counted from, in the form that follows "since" ("August"). */
export const formatSinceMonth = (
	year: number,
	month: CalendarMonth,
	language: string,
) =>
	new Intl.DateTimeFormat(language, {
		day: "numeric",
		month: "long",
		timeZone: "UTC",
	})
		.formatToParts(
			new Date(Date.UTC(year, Object.values(CalendarMonth).indexOf(month), 1)),
		)
		.find((part) => part.type === "month")?.value ?? "";
