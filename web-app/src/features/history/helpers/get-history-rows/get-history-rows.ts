import type { GetSnapshotsResponse } from "../../api/history.schemas";
import type { HistoryRow } from "../../interfaces/history-row";

/** Snapshots followed by the estimates, the latest first; every estimate is older than the first snapshot. */
export const getHistoryRows = ({
	snapshots,
	estimates,
}: GetSnapshotsResponse): HistoryRow[] => [
	...snapshots.map((snapshot, index) => ({
		kind: "snapshot" as const,
		laterSnapshots: index,
		isOnly: snapshots.length === 1,
		...snapshot,
	})),
	...estimates.map((estimate) => ({ kind: "estimate" as const, ...estimate })),
];
