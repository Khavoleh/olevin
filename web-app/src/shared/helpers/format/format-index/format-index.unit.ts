import { describe, expect, it } from "vitest";
import { formatIndex } from "./format-index";

describe("formatIndex", () => {
	it.each([
		["uk", 58.7, "58,7"],
		["en", 58.7, "58.7"],
		["uk", 60, "60,0"],
		["en", 3.65, "3.7"],
	])("should write %s %d as %s", (language, value, expected) => {
		expect(formatIndex(value, language)).toBe(expected);
	});
});
