/** Name of the time zone in the given style, such as `GMT+02:00` for `longOffset`. */
export const getTimezoneName = (
	language: string,
	timeZone: string,
	timeZoneName: "shortOffset" | "longOffset" | "longGeneric",
) =>
	new Intl.DateTimeFormat(language, { timeZone, timeZoneName })
		.formatToParts()
		.find((part) => part.type === "timeZoneName")?.value ?? "";
