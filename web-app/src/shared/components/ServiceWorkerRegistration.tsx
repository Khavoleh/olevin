"use client";

import { useEffect } from "react";

/** Registers the service worker of the app in browsers that support it. */
const ServiceWorkerRegistration = () => {
	useEffect(() => {
		if (!("serviceWorker" in navigator)) return;

		navigator.serviceWorker
			.register("/sw.js", { scope: "/", updateViaCache: "none" })
			.catch(() => undefined);
	}, []);

	return null;
};

export default ServiceWorkerRegistration;
