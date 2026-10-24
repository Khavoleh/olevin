"use server";

import type { SubmissionResult } from "@conform-to/react";
import { updateEstimatedSnapshot } from "../api/snapshots";
import { UPDATE_ESTIMATE_SCHEMA } from "../constants/update-estimate-schema";
import { submitSnapshotForm } from "../helpers/submit-snapshot-form";

/** Saves the new amounts of an estimate, which scores every snapshot again, and opens the history. */
export async function saveEstimate(
	_lastResult: SubmissionResult | undefined,
	formData: FormData,
): Promise<SubmissionResult> {
	return submitSnapshotForm(
		formData,
		UPDATE_ESTIMATE_SCHEMA,
		({ id, ...body }) => updateEstimatedSnapshot(id, body),
	);
}
