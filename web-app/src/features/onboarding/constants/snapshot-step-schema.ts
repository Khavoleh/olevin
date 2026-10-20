import { CreateFirstSnapshotBody } from "../api/onboarding.zod";

/** First snapshot form, without the estimates of the step after it. */
export const SNAPSHOT_STEP_SCHEMA = CreateFirstSnapshotBody.omit({
	previousMonth: true,
	twoMonthsAgo: true,
});
