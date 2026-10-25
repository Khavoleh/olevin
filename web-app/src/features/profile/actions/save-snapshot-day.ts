"use server";

import { requestOrRedirect } from "@shared/api";
import { ApiError } from "@shared/helpers";
import { refresh } from "next/cache";
import { updateSnapshotDay } from "../api/profile";
import { UpdateSnapshotDayBody } from "../api/profile.zod";

/** Saves the new snapshot day and shows the profile with it; returns the error of the API on a failure. */
export async function saveSnapshotDay(
	language: string,
	snapshotDay: number,
): Promise<string | undefined> {
	const body = UpdateSnapshotDayBody.parse({ snapshotDay });
	const response = await requestOrRedirect(
		() => updateSnapshotDay(body),
		language,
	);

	if (response.status !== 200) return new ApiError(response).message;

	refresh();
}
