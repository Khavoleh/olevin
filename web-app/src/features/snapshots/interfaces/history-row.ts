import type {
	EstimateResponse,
	SnapshotResponse,
} from "../api/snapshots.schemas";

/** Row of the history: an actual snapshot or an estimate from onboarding. */
export type HistoryRow =
	| ({ kind: "snapshot" } & SnapshotResponse)
	| ({ kind: "estimate" } & EstimateResponse);
