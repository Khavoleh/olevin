import { requestOrRedirect } from "@shared/api";
import { ApiError } from "@shared/helpers";
import { cache } from "react";
import { getPushSubscriptions as requestPushSubscriptions } from "../api/profile";
import type { PushSubscriptionResponse } from "../api/profile.schemas";

type PushSubscriptions =
	| { subscriptions: PushSubscriptionResponse[] }
	| { error: string };

/** Browsers of the user that get the push, the newest first, or the error of the API. */
export const getPushSubscriptions = cache(
	async (language: string): Promise<PushSubscriptions> => {
		const response = await requestOrRedirect(
			requestPushSubscriptions,
			language,
		);

		return response.data?.data
			? { subscriptions: response.data.data }
			: { error: new ApiError(response).message };
	},
);
