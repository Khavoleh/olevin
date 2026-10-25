import { describe, expect, it } from "vitest";
import { formatTimezone } from "./format-timezone";

describe("formatTimezone", () => {
	it.each([
		["Asia/Tokyo", "Tokyo, UTC+9"],
		["Asia/Kolkata", "Kolkata, UTC+5:30"],
		["America/Argentina/Buenos_Aires", "Buenos Aires, UTC-3"],
	])("should show %s as its city with the offset", (timezone, expected) => {
		expect(formatTimezone(timezone)).toBe(expected);
	});

	it("should show UTC as its offset only", () => {
		expect(formatTimezone("UTC")).toBe("UTC+0");
	});
});
