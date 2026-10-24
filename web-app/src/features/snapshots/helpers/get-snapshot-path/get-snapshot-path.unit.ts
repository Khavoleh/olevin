import { describe, expect, it } from "vitest";
import { getSnapshotPath } from "./get-snapshot-path";

describe("getSnapshotPath", () => {
	it.each([
		[2026, "September", "/snapshots/2026-09"],
		[2026, "December", "/snapshots/2026-12"],
		[2027, "January", "/snapshots/2027-01"],
	] as const)("should open %d %s at %s", (year, month, expected) => {
		expect(getSnapshotPath(year, month)).toBe(expected);
	});
});
