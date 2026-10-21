import { describe, expect, it } from "vitest";
import { CalendarMonth } from "../../api/home.schemas";
import { formatSnapshotMonth } from "./format-snapshot-month";

describe("formatSnapshotMonth", () => {
	it("should name the month in Ukrainian with its year", () => {
		expect(formatSnapshotMonth(2026, CalendarMonth.September, "uk")).toBe(
			"вересень 2026",
		);
	});

	it("should name the month in English with its year", () => {
		expect(formatSnapshotMonth(2026, CalendarMonth.January, "en")).toBe(
			"January 2026",
		);
	});
});
