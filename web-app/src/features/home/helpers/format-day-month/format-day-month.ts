/** Day and month of an API date (`2026-10-31`), as "October 31". */
export const formatDayMonth = (date: string, language: string) =>
	new Intl.DateTimeFormat(language, {
		day: "numeric",
		month: "long",
		timeZone: "UTC",
	}).format(new Date(`${date}T00:00:00Z`));
