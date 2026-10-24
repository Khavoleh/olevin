import {
	type EstimateRequest,
	getEstimateRequest,
	getSnapshotRequest,
	type SnapshotRequest,
} from "@widgets/snapshot-fields";
import type { CurrencyCode } from "../api/onboarding.schemas";
import type { FirstSnapshotDraft } from "../onboarding-store";

/** First snapshot request; the self-rating is missing until the user picks it. */
export type FirstSnapshotRequest = SnapshotRequest & {
	previousMonth: EstimateRequest;
	twoMonthsAgo: EstimateRequest;
};

/** Request of the first snapshot from the draft: empty amounts become 0, missing currencies the base one. */
export const getFirstSnapshotRequest = (
	draft: FirstSnapshotDraft,
	baseCurrency: CurrencyCode,
): FirstSnapshotRequest => ({
	...getSnapshotRequest(draft, baseCurrency),
	previousMonth: getEstimateRequest(draft.previousMonth, baseCurrency),
	twoMonthsAgo: getEstimateRequest(draft.twoMonthsAgo, baseCurrency),
});
