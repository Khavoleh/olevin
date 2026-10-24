import { UpdateSnapshotBody, UpdateSnapshotParams } from "../api/snapshots.zod";

/** Changed snapshot with its identifier. */
export const UPDATE_SNAPSHOT_SCHEMA = UpdateSnapshotBody.extend(
	UpdateSnapshotParams.shape,
);
