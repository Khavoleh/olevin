import type { I18N } from "@shared/interfaces";
import { describe, expect, it } from "vitest";
import { getI18n } from "./get-i18n";

const MOCK_I18N: I18N = {
	en: {
		greeting: "Hello",
		button: "Click me",
	},
	uk: {
		greeting: "Привіт",
		button: "Натисни мене",
	},
};

const PARTIAL_I18N: I18N = {
	en: {
		title: "Title",
		description: "Description",
	},
	uk: {
		title: "Заголовок",
		// description is missing - should fallback to English
	},
};

describe("getI18n", () => {
	it("should return English translation for English", () => {
		const t = getI18n("en", MOCK_I18N);

		expect(t("greeting")).toBe("Hello");
		expect(t("button")).toBe("Click me");
	});

	it("should return Ukrainian translation for Ukrainian", () => {
		const t = getI18n("uk", MOCK_I18N);

		expect(t("greeting")).toBe("Привіт");
		expect(t("button")).toBe("Натисни мене");
	});

	it("should return key if translation is missing in both languages", () => {
		const t = getI18n("en", MOCK_I18N);

		expect(t("nonexistent_key")).toBe("nonexistent_key");
	});

	it("should fallback to English if Ukrainian translation is missing", () => {
		const t = getI18n("uk", PARTIAL_I18N);

		expect(t("description")).toBe("Description");
	});

	it("should return Ukrainian translation when it exists", () => {
		const t = getI18n("uk", PARTIAL_I18N);

		expect(t("title")).toBe("Заголовок");
	});

	it("should default to English for an unsupported language", () => {
		const t = getI18n("fr", MOCK_I18N);

		expect(t("greeting")).toBe("Hello");
	});

	it("should not return Object.prototype members as translations", () => {
		const t = getI18n("en", MOCK_I18N);

		expect(t("constructor")).toBe("constructor");
		expect(t("toString")).toBe("toString");
	});
});
