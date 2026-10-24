import { describe, expect, it } from "vitest";
import { formatDayMonth } from "./format-day-month";

describe("formatDayMonth", () => {
	it.each([
		["uk", "2026-10-31", "31 жовтня"],
		["en", "2026-10-31", "October 31"],
		["uk", "2027-01-01", "1 січня"],
	])("should write %s %s as %s", (language, date, expected) => {
		expect(formatDayMonth(date, language)).toBe(expected);
	});
});
