import { describe, expect, it } from "vitest";
import { formatPercent } from "./format-percent";

describe("formatPercent", () => {
	it.each([
		["en", 0.15, "15%"],
		["uk", 0.15, "15%"],
		["en", -0.2, "-20%"],
		["uk", 0.0115, "1,2%"],
	])("should write %s %d as %s", (language, value, expected) => {
		expect(formatPercent(value, language)).toBe(expected);
	});
});
