/** Name and value of every leaf of the object, named the way conform parses them back (`income[0].amount`). */
export const getFormEntries = (
	value: unknown,
	name = "",
): [string, string][] => {
	if (value === undefined || value === null) return [];

	if (Array.isArray(value)) {
		return value.flatMap((item, index) =>
			getFormEntries(item, `${name}[${index}]`),
		);
	}

	if (typeof value === "object") {
		return Object.entries(value).flatMap(([key, item]) =>
			getFormEntries(item, name ? `${name}.${key}` : key),
		);
	}

	return [[name, String(value)]];
};
