import { describe, expect, it } from "vitest";
import { getPushSupport, type PushEnvironment } from "./get-push-support";

const desktop: PushEnvironment = {
	hasServiceWorker: true,
	hasPushManager: true,
	hasNotification: true,
	isIos: false,
	isStandalone: false,
	permission: "default",
};

describe("getPushSupport", () => {
	it("is available in a browser with push", () => {
		expect(getPushSupport(desktop)).toBe("available");
		expect(getPushSupport({ ...desktop, permission: "granted" })).toBe(
			"available",
		);
	});

	it("is denied once the user blocked the notifications", () => {
		expect(getPushSupport({ ...desktop, permission: "denied" })).toBe("denied");
	});

	it("asks to install the app in Safari on iPhone", () => {
		expect(
			getPushSupport({
				...desktop,
				hasPushManager: false,
				hasNotification: false,
				isIos: true,
			}),
		).toBe("install");
	});

	it("is available in the app on the iPhone Home Screen", () => {
		expect(
			getPushSupport({ ...desktop, isIos: true, isStandalone: true }),
		).toBe("available");
	});

	it("is unsupported without the push API", () => {
		expect(getPushSupport({ ...desktop, hasPushManager: false })).toBe(
			"unsupported",
		);
	});
});
