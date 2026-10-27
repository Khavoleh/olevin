"use server";

import { requestOrRedirect } from "@shared/api";
import { ApiError } from "@shared/helpers";
import { refresh } from "next/cache";
import { subscribePush } from "../api/profile";
import type { SubscribePushRequest } from "../api/profile.schemas";
import { SubscribePushBody } from "../api/profile.zod";

/** Turns the push on for the push subscription of this browser; returns the error of the API on a failure. */
export async function savePushSubscription(
	language: string,
	subscription: SubscribePushRequest,
): Promise<string | undefined> {
	const body = SubscribePushBody.parse(subscription);
	const response = await requestOrRedirect(() => subscribePush(body), language);

	if (response.status !== 200 && response.status !== 201) {
		return new ApiError(response).message;
	}

	refresh();
}
