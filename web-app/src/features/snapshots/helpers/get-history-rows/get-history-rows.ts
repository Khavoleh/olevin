import type { GetSnapshotsResponse } from "../../api/snapshots.schemas";
import type { HistoryRow } from "../../interfaces/history-row";

/** Snapshots followed by the estimates, the latest first; every estimate is older than the first snapshot. */
export const getHistoryRows = ({
	snapshots,
	estimates,
}: GetSnapshotsResponse): HistoryRow[] => [
	...snapshots.map((snapshot) => ({ kind: "snapshot" as const, ...snapshot })),
	...estimates.map((estimate) => ({ kind: "estimate" as const, ...estimate })),
];
