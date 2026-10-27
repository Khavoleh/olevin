"use server";

import { requestOrRedirect } from "@shared/api";
import { ApiError } from "@shared/helpers";
import { refresh } from "next/cache";
import { deletePushSubscription } from "../api/profile";
import { DeletePushSubscriptionParams } from "../api/profile.zod";

/** Turns the push off for a browser of the user; returns the error of the API on a failure. */
export async function removePushSubscription(
	language: string,
	id: string,
): Promise<string | undefined> {
	const params = DeletePushSubscriptionParams.parse({ id });
	const response = await requestOrRedirect(
		() => deletePushSubscription(params.id),
		language,
	);

	if (response.status !== 204) return new ApiError(response).message;

	refresh();
}
