import { describe, expect, it } from "vitest";
import { formatMonths } from "./format-months";

describe("formatMonths", () => {
	it.each([
		["en", 3.8333, "short", "3.8 mths"],
		["uk", 3.8333, "short", "3,8 міс."],
		["en", 3, "long", "3 months"],
		["uk", 3, "long", "3 місяці"],
		["uk", 5, "long", "5 місяців"],
	] as const)(
		"should write %s %d %s as %s",
		(language, value, display, expected) => {
			expect(formatMonths(value, language, display)).toBe(expected);
		},
	);
});
