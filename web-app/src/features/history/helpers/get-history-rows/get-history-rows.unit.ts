import { describe, expect, it } from "vitest";
import type { GetSnapshotsResponse } from "../../api/history.schemas";
import { getHistoryRows } from "./get-history-rows";

const HISTORY: GetSnapshotsResponse = {
	currency: "UAH",
	snapshots: [
		{
			id: "october",
			year: 2026,
			month: "October",
			index: 58.7,
			zone: "Unstable",
			change: 3.6,
			expenses: 36000,
			saved: 4000,
		},
		{
			id: "september",
			year: 2026,
			month: "September",
			index: 55.1,
			zone: "Unstable",
			change: null,
			expenses: 42000,
			saved: 0,
		},
	],
	estimates: [
		{
			id: "august",
			year: 2026,
			month: "August",
			expenses: 30000,
			saved: 5000,
		},
	],
	newSnapshotOpensOn: null,
};

describe("getHistoryRows", () => {
	it("should put the estimates after the snapshots, the latest first", () => {
		expect(getHistoryRows(HISTORY).map((row) => [row.kind, row.id])).toEqual([
			["snapshot", "october"],
			["snapshot", "september"],
			["estimate", "august"],
		]);
	});

	it("should count the snapshots after each one", () => {
		expect(
			getHistoryRows(HISTORY).flatMap((row) =>
				row.kind === "snapshot" ? [[row.id, row.laterSnapshots]] : [],
			),
		).toEqual([
			["october", 0],
			["september", 1],
		]);
	});

	it("should mark the only snapshot", () => {
		const [only] = getHistoryRows({
			...HISTORY,
			snapshots: HISTORY.snapshots.slice(1),
		});

		expect(only).toMatchObject({ kind: "snapshot", isOnly: true });
		expect(getHistoryRows(HISTORY)[0]).toMatchObject({ isOnly: false });
	});
});
