import {
	UpdateEstimatedSnapshotBody,
	UpdateEstimatedSnapshotParams,
} from "../api/snapshots.zod";

/** Changed estimate with its identifier. */
export const UPDATE_ESTIMATE_SCHEMA = UpdateEstimatedSnapshotBody.extend(
	UpdateEstimatedSnapshotParams.shape,
);
