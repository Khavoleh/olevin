"use server";

import type { SubmissionResult } from "@conform-to/react";
import { createSnapshot } from "../api/snapshots";
import { CreateSnapshotBody } from "../api/snapshots.zod";
import { submitSnapshotForm } from "../helpers/submit-snapshot-form";

/** Saves a snapshot of the chosen month and opens the history. */
export async function saveNewSnapshot(
	_lastResult: SubmissionResult | undefined,
	formData: FormData,
): Promise<SubmissionResult> {
	return submitSnapshotForm(formData, CreateSnapshotBody, createSnapshot);
}
