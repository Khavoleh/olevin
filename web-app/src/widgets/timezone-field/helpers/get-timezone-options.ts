import { getTimezoneName } from "./get-timezone-name";

const RENAMED_TIMEZONES: Record<string, string> = {
	"Europe/Kiev": "Europe/Kyiv",
};

/** Option of one IANA time zone with its offset and, when it has one, its localized name. */
export const getTimezoneOption = (language: string, id: string) => {
	const offset = getTimezoneName("en", id, "longOffset");
	const name = getTimezoneName(language, id, "longGeneric");

	return {
		id,
		label: `${id.replaceAll("_", " ")} (${offset})`,
		description: name.startsWith("GMT") ? undefined : name,
	};
};

/** Options of every IANA time zone the browser knows, by their current names. */
export const getTimezoneOptions = (language: string) => {
	const zones = Intl.supportedValuesOf("timeZone").map(
		(zone) => RENAMED_TIMEZONES[zone] ?? zone,
	);

	return [...new Set(zones)]
		.sort()
		.map((zone) => getTimezoneOption(language, zone));
};
