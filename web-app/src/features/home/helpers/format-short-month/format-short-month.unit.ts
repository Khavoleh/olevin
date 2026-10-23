import { describe, expect, it } from "vitest";
import { CalendarMonth } from "../../api/home.schemas";
import { formatShortMonth } from "./format-short-month";

describe("formatShortMonth", () => {
	it.each([
		["en", CalendarMonth.April, "Apr"],
		["uk", CalendarMonth.April, "квіт."],
		["uk", CalendarMonth.September, "вер."],
	] as const)("should write %s %s as %s", (language, month, expected) => {
		expect(formatShortMonth(2026, month, language)).toBe(expected);
	});
});
