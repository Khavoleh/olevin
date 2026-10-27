/** What the browser allows for the push. */
export type PushSupport = "available" | "denied" | "install" | "unsupported";

/** Push features of the current browser. */
export interface PushEnvironment {
	hasServiceWorker: boolean;
	hasPushManager: boolean;
	hasNotification: boolean;
	isIos: boolean;
	isStandalone: boolean;
	permission: NotificationPermission;
}

/** Whether the push can be turned on here, must be allowed first, needs the app on the Home Screen or never works. */
export const getPushSupport = ({
	hasServiceWorker,
	hasPushManager,
	hasNotification,
	isIos,
	isStandalone,
	permission,
}: PushEnvironment): PushSupport => {
	if (isIos && !isStandalone) return "install";

	if (!hasServiceWorker || !hasPushManager || !hasNotification) {
		return "unsupported";
	}

	return permission === "denied" ? "denied" : "available";
};
