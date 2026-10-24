import { describe, expect, it } from "vitest";
import { formatMonthRange } from "./format-month-range";

describe("formatMonthRange", () => {
	it("should write the year once within one year", () => {
		expect(
			formatMonthRange(
				{ year: 2026, month: "April" },
				{ year: 2026, month: "September" },
				"uk",
			),
		).toBe("квітень — вересень 2026");
	});

	it("should write both years across a new year", () => {
		expect(
			formatMonthRange(
				{ year: 2025, month: "November" },
				{ year: 2026, month: "October" },
				"en",
			),
		).toBe("November 2025 — October 2026");
	});

	it("should write one month once", () => {
		expect(
			formatMonthRange(
				{ year: 2026, month: "September" },
				{ year: 2026, month: "September" },
				"uk",
			),
		).toBe("вересень 2026");
	});
});
