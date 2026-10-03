import { describe, expect, it } from "vitest";
import { getLanguage } from "./get-language";

describe("getLanguage", () => {
	it("should prefer the language saved in the cookie", () => {
		expect(getLanguage("uk", "en-US,en;q=0.9")).toBe("uk");
	});

	it("should ignore an unsupported cookie value", () => {
		expect(getLanguage("de", "uk-UA,uk;q=0.9")).toBe("uk");
	});

	it("should take the first supported language from Accept-Language", () => {
		expect(getLanguage(undefined, "de-DE,uk-UA;q=0.9,en;q=0.8")).toBe("uk");
	});

	it("should match language tags case-insensitively", () => {
		expect(getLanguage(undefined, "UK-UA")).toBe("uk");
	});

	it("should default to English when nothing matches", () => {
		expect(getLanguage(undefined, "de-DE,fr;q=0.8")).toBe("en");
	});

	it("should default to English without Accept-Language", () => {
		expect(getLanguage(undefined, null)).toBe("en");
	});

	it("should not treat Object.prototype keys as languages", () => {
		for (const fakeLang of ["constructor", "toString", "__proto__"]) {
			expect(getLanguage(fakeLang, fakeLang)).toBe("en");
		}
	});
});
