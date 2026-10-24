import type {
	EstimateResponse,
	SnapshotResponse,
} from "../api/history.schemas";

/** Row of the history: an actual snapshot with the number of later ones, or an estimate from onboarding. */
export type HistoryRow =
	| ({
			kind: "snapshot";
			laterSnapshots: number;
			isOnly: boolean;
	  } & SnapshotResponse)
	| ({ kind: "estimate" } & EstimateResponse);
