import { describe, expect, it } from "vitest";
import { getLanguageUrl } from "./get-language-url";

describe("getLanguageUrl", () => {
	it("should replace an existing language prefix", () => {
		expect(getLanguageUrl("/en/sign-in", "uk")).toBe("/uk/sign-in");
	});

	it("should add a language prefix to a path without one", () => {
		expect(getLanguageUrl("/sign-in", "en")).toBe("/en/sign-in");
	});

	it("should handle the root path", () => {
		expect(getLanguageUrl("/", "uk")).toBe("/uk");
		expect(getLanguageUrl("", "uk")).toBe("/uk");
	});

	it("should handle a path with only a language prefix", () => {
		expect(getLanguageUrl("/en", "uk")).toBe("/uk");
	});

	it("should drop trailing slashes", () => {
		expect(getLanguageUrl("/uk/sign-in/", "en")).toBe("/en/sign-in");
	});

	it("should keep nested segments", () => {
		expect(getLanguageUrl("/uk/snapshots/2026-09", "en")).toBe(
			"/en/snapshots/2026-09",
		);
	});
});
