import { describe, expect, it } from "vitest";
import { getSnapshotMonths } from "./get-snapshot-months";

describe("getSnapshotMonths", () => {
	it("should return the current month and the 2 before it", () => {
		expect(
			getSnapshotMonths(new Date("2026-10-19T12:00:00Z"), "Europe/Kyiv"),
		).toEqual([
			{ year: 2026, month: 10 },
			{ year: 2026, month: 9 },
			{ year: 2026, month: 8 },
		]);
	});

	it("should go back into the previous year", () => {
		expect(
			getSnapshotMonths(new Date("2026-02-10T12:00:00Z"), "Europe/Kyiv"),
		).toEqual([
			{ year: 2026, month: 2 },
			{ year: 2026, month: 1 },
			{ year: 2025, month: 12 },
		]);
	});

	it("should take the month of the time zone, not of UTC", () => {
		const now = new Date("2026-11-01T02:00:00Z");

		expect(getSnapshotMonths(now, "America/Edmonton")[0]).toEqual({
			year: 2026,
			month: 10,
		});
		expect(getSnapshotMonths(now, "Europe/Kyiv")[0]).toEqual({
			year: 2026,
			month: 11,
		});
	});
});
