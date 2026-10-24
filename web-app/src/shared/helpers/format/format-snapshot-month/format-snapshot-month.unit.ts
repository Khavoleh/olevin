import { describe, expect, it } from "vitest";
import { formatSnapshotMonth } from "./format-snapshot-month";

describe("formatSnapshotMonth", () => {
	it("should name the month in Ukrainian with its year", () => {
		expect(formatSnapshotMonth(2026, "September", "uk")).toBe("вересень 2026");
	});

	it("should name the month in English with its year", () => {
		expect(formatSnapshotMonth(2026, "January", "en")).toBe("January 2026");
	});
});
