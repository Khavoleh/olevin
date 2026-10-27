import { describe, expect, it } from "vitest";
import { decodeBase64Url } from "./decode-base64-url";

describe("decodeBase64Url", () => {
	it("decodes the URL-safe alphabet", () => {
		expect([...decodeBase64Url("-_8")]).toEqual([251, 255]);
	});

	it("decodes a value without padding", () => {
		expect([...decodeBase64Url("AQID")]).toEqual([1, 2, 3]);
		expect([...decodeBase64Url("AQI")]).toEqual([1, 2]);
	});

	it("gives the 65 bytes of a VAPID public key", () => {
		const key =
			"BH0Ulm1Lh1kYXqGV3yMAOcho5OPliSjyxHuY42XyEnBELkQ-o_JSvLed2KfTD0T59Py6w6gQuLSPGfzKDi_ISXM";

		expect(decodeBase64Url(key)).toHaveLength(65);
		expect(decodeBase64Url(key)[0]).toBe(4);
	});
});
