import { describe, expect, it } from "vitest";
import { getPathLanguage } from "./get-path-language";

describe("getPathLanguage", () => {
	it("should return the language prefix", () => {
		expect(getPathLanguage("/uk/sign-in")).toBe("uk");
		expect(getPathLanguage("/en")).toBe("en");
	});

	it("should return undefined without a supported language prefix", () => {
		expect(getPathLanguage("/")).toBeUndefined();
		expect(getPathLanguage("/sign-in")).toBeUndefined();
		expect(getPathLanguage("/fr/sign-in")).toBeUndefined();
	});

	it("should not match a segment that only starts with a language", () => {
		expect(getPathLanguage("/english")).toBeUndefined();
	});
});
