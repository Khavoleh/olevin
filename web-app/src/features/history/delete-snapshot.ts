"use server";

import { requestOrRedirect } from "@shared/api";
import { ApiError } from "@shared/helpers";
import { refresh } from "next/cache";
import { deleteSnapshot } from "./api/history";

/** Deletes the snapshot and shows the history without it; returns the error of the API on a failure. */
export async function removeSnapshot(
	id: string,
	language: string,
): Promise<string | undefined> {
	const response = await requestOrRedirect(() => deleteSnapshot(id), language);

	if (response.status !== 204) return new ApiError(response).message;

	refresh();
}
