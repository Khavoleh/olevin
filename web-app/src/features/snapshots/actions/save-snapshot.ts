"use server";

import type { SubmissionResult } from "@conform-to/react";
import { updateSnapshot } from "../api/snapshots";
import { UPDATE_SNAPSHOT_SCHEMA } from "../constants/update-snapshot-schema";
import { submitSnapshotForm } from "../helpers/submit-snapshot-form";

/** Saves the new amounts of a snapshot, which scores the later ones again, and opens the history. */
export async function saveSnapshot(
	_lastResult: SubmissionResult | undefined,
	formData: FormData,
): Promise<SubmissionResult> {
	return submitSnapshotForm(
		formData,
		UPDATE_SNAPSHOT_SCHEMA,
		({ id, ...body }) => updateSnapshot(id, body),
	);
}
