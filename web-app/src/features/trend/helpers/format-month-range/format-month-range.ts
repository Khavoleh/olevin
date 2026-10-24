import { CALENDAR_MONTHS } from "@shared/constants";
import type { TrendPointResponse } from "../../api/trend.schemas";

type Month = Pick<TrendPointResponse, "year" | "month">;

const formatMonthName = ({ year, month }: Month, language: string) =>
	new Intl.DateTimeFormat(language, { month: "long", timeZone: "UTC" }).format(
		new Date(Date.UTC(year, CALENDAR_MONTHS.indexOf(month), 1)),
	);

/** Months of the trend as "April — September 2026"; the year is written once within one year. */
export const formatMonthRange = (
	first: Month,
	last: Month,
	language: string,
) => {
	const end = `${formatMonthName(last, language)} ${last.year}`;

	if (first.year !== last.year)
		return `${formatMonthName(first, language)} ${first.year} — ${end}`;

	return first.month === last.month
		? end
		: `${formatMonthName(first, language)} — ${end}`;
};
