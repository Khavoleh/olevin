import { describe, expect, it } from "vitest";
import { getAccountCenterUrl } from "./get-account-center-url";

const ENDPOINT = "https://localhost:3001";

describe("getAccountCenterUrl", () => {
	it("should open the security page of the Account Center", () => {
		const url = new URL(getAccountCenterUrl(ENDPOINT, "en"));

		expect(url.origin + url.pathname).toBe(
			"https://localhost:3001/account/security",
		);
	});

	it("should show the Account Center in the Logto locale of the app language", () => {
		const cases: [string, string][] = [
			["uk", "uk-UA"],
			["en", "en"],
		];

		for (const [language, locale] of cases) {
			const url = new URL(getAccountCenterUrl(ENDPOINT, language));

			expect(url.searchParams.get("ui_locales")).toBe(locale);
		}
	});
});
