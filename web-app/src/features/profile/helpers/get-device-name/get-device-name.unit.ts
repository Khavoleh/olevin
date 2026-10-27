import { describe, expect, it } from "vitest";
import { getDeviceName } from "./get-device-name";

describe("getDeviceName", () => {
	it.each([
		[
			"Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/141.0.0.0 Safari/537.36",
			"Chrome · macOS",
		],
		[
			"Mozilla/5.0 (iPhone; CPU iPhone OS 18_6 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/18.6 Mobile/15E148 Safari/604.1",
			"Safari · iPhone",
		],
		[
			"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/141.0.0.0 Safari/537.36 Edg/141.0.0.0",
			"Edge · Windows",
		],
		[
			"Mozilla/5.0 (X11; Ubuntu; Linux x86_64; rv:144.0) Gecko/20100101 Firefox/144.0",
			"Firefox · Linux",
		],
		[
			"Mozilla/5.0 (Linux; Android 14; SM-S918B) AppleWebKit/537.36 (KHTML, like Gecko) SamsungBrowser/28.0 Chrome/130.0.0.0 Mobile Safari/537.36",
			"Samsung Internet · Android",
		],
	])("names %s", (userAgent, name) => {
		expect(getDeviceName(userAgent, "Browser")).toBe(name);
	});

	it("gives the fallback for an unknown user agent", () => {
		expect(getDeviceName("curl/8.7.1", "Browser")).toBe("Browser");
	});
});
