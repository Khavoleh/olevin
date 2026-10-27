"use server";

import { syncPushSubscription } from "../api/profile";
import { SyncPushSubscriptionBody } from "../api/profile.zod";

/** Tells the API that this browser still has its push subscription; never adds one and ignores every failure. */
export async function confirmPushSubscription(endpoint: string): Promise<void> {
	const body = SyncPushSubscriptionBody.safeParse({ endpoint });

	if (body.success)
		await syncPushSubscription(body.data).catch(() => undefined);
}
