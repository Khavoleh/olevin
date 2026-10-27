import { describe, expect, it } from "vitest";
import { getSnapshotMonths } from "./get-snapshot-months";

describe("getSnapshotMonths", () => {
	it("should return the current month and the 2 before it from the snapshot day", () => {
		expect(
			getSnapshotMonths(new Date("2026-10-19T12:00:00Z"), "Europe/Kyiv", 19),
		).toEqual([
			{ year: 2026, month: 10 },
			{ year: 2026, month: 9 },
			{ year: 2026, month: 8 },
		]);
	});

	it("should start from the previous month before the snapshot day", () => {
		expect(
			getSnapshotMonths(new Date("2026-10-05T12:00:00Z"), "Europe/Kyiv", 6),
		).toEqual([
			{ year: 2026, month: 9 },
			{ year: 2026, month: 8 },
			{ year: 2026, month: 7 },
		]);
	});

	it("should take the last day of a shorter month as the snapshot day", () => {
		expect(
			getSnapshotMonths(new Date("2026-02-28T12:00:00Z"), "Europe/Kyiv", 31)[0],
		).toEqual({ year: 2026, month: 2 });
		expect(
			getSnapshotMonths(new Date("2026-02-27T12:00:00Z"), "Europe/Kyiv", 31)[0],
		).toEqual({ year: 2026, month: 1 });
	});

	it("should go back into the previous year", () => {
		expect(
			getSnapshotMonths(new Date("2026-02-10T12:00:00Z"), "Europe/Kyiv", 1),
		).toEqual([
			{ year: 2026, month: 2 },
			{ year: 2026, month: 1 },
			{ year: 2025, month: 12 },
		]);
		expect(
			getSnapshotMonths(new Date("2026-01-02T12:00:00Z"), "Europe/Kyiv", 5),
		).toEqual([
			{ year: 2025, month: 12 },
			{ year: 2025, month: 11 },
			{ year: 2025, month: 10 },
		]);
	});

	it("should take the date of the time zone, not of UTC", () => {
		const now = new Date("2026-11-01T02:00:00Z");

		expect(getSnapshotMonths(now, "America/Edmonton", 1)[0]).toEqual({
			year: 2026,
			month: 10,
		});
		expect(getSnapshotMonths(now, "Europe/Kyiv", 1)[0]).toEqual({
			year: 2026,
			month: 11,
		});
	});
});
