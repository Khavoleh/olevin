import { describe, expect, it } from "vitest";
import { formatChange } from "./format-change";

describe("formatChange", () => {
	it.each([
		["en", 3.6, "+3.6"],
		["uk", 3.6, "+3,6"],
		["en", -2.6, "-2.6"],
		["en", 0, "0.0"],
	])("should write %s %d as %s", (language, value, expected) => {
		expect(formatChange(value, language)).toBe(expected);
	});
});
