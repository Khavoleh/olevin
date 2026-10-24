import { CALENDAR_MONTHS } from "@shared/constants";

/** Year and month of the page path (`2026-09`), or `undefined` when it is not a month. */
export const parseMonthParam = (param: string) => {
	const match = /^(\d{4})-(\d{2})$/.exec(param);
	const month = CALENDAR_MONTHS[Number(match?.[2]) - 1];

	if (!match || !month) return undefined;

	return { year: Number(match[1]), month };
};
