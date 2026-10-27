"use client";

import { useEffect } from "react";
import { confirmPushSubscription } from "../actions/confirm-push-subscription";

/** Tells the API once per visit that this browser still gets the push. */
const PushSubscriptionSync = () => {
	useEffect(() => {
		if (
			!("serviceWorker" in navigator) ||
			!("Notification" in window) ||
			Notification.permission !== "granted"
		) {
			return;
		}

		navigator.serviceWorker.ready
			.then((registration) => registration.pushManager.getSubscription())
			.then((subscription) =>
				subscription
					? confirmPushSubscription(subscription.endpoint)
					: undefined,
			)
			.catch(() => undefined);
	}, []);

	return null;
};

export default PushSubscriptionSync;
