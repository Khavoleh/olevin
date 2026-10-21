import { CalendarMonth } from "../../api/home.schemas";

/** Month of a snapshot with its year, as "September 2026". */
export const formatSnapshotMonth = (
	year: number,
	month: CalendarMonth,
	language: string,
) => {
	const name = new Intl.DateTimeFormat(language, {
		month: "long",
		timeZone: "UTC",
	}).format(
		new Date(Date.UTC(year, Object.values(CalendarMonth).indexOf(month), 1)),
	);

	return `${name} ${year}`;
};
