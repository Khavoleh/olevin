import { describe, expect, it } from "vitest";
import { formatShortMonth } from "./format-short-month";

describe("formatShortMonth", () => {
	it.each([
		["en", "April", "Apr"],
		["uk", "April", "квіт."],
		["uk", "September", "вер."],
	] as const)("should write %s %s as %s", (language, month, expected) => {
		expect(formatShortMonth(2026, month, language)).toBe(expected);
	});
});
