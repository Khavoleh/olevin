import { describe, expect, it } from "vitest";
import { formatPoints } from "./format-points";

describe("formatPoints", () => {
	it.each([
		["en", 1.6, "+1.6"],
		["uk", 1.6, "+1,6"],
		["en", -0.2, "-0.2"],
		["uk", 0, "0,0"],
		["en", 21, "+21.0"],
	])("should write %s %d as %s", (language, value, expected) => {
		expect(formatPoints(value, language)).toBe(expected);
	});
});
