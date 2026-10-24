import { describe, expect, it } from "vitest";
import { parseMonthParam } from "./parse-month-param";

describe("parseMonthParam", () => {
	it.each([
		["2026-09", 2026, "September"],
		["2026-12", 2026, "December"],
		["2027-01", 2027, "January"],
	] as const)("should read %s as %d %s", (param, year, month) => {
		expect(parseMonthParam(param)).toEqual({ year, month });
	});

	it.each(["2026-00", "2026-13", "2026-9", "26-09", "new", "2026-09-01"])(
		"should reject %s",
		(param) => {
			expect(parseMonthParam(param)).toBeUndefined();
		},
	);
});
