import { getTimezoneName } from "../get-timezone-name";

/** Short name of an IANA time zone: its city with the current UTC offset, such as `Kyiv, UTC+2`. */
export const formatTimezone = (timezone: string) => {
	const city = timezone.split("/").at(-1)?.replaceAll("_", " ") ?? timezone;
	const name = getTimezoneName("en", timezone, "shortOffset");
	const offset = name === "GMT" ? "UTC+0" : name.replace("GMT", "UTC");

	return city === "UTC" ? offset : `${city}, ${offset}`;
};
