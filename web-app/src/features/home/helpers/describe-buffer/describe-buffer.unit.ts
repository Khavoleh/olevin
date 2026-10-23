import { describe, expect, it } from "vitest";
import type { BufferResponse } from "../../api/home.schemas";
import { COMPOSITION_CARD_I18N } from "../../i18n/composition-card-i18n";
import { describeBuffer } from "./describe-buffer";

const t = {
	en: (key: string) => COMPOSITION_CARD_I18N.en[key],
	uk: (key: string) => COMPOSITION_CARD_I18N.uk[key],
};

const buffer = (
	reserveMonths: number | null,
	expenseVariation: number | null,
): BufferResponse => ({
	value: 60.4,
	term: "Moderate",
	reserveMonths,
	expenseVariation,
});

describe("describeBuffer", () => {
	it("should write how long the reserve lasts and how much expenses vary", () => {
		expect(describeBuffer(buffer(3.5, 0.2), t.uk, "uk")).toBe(
			"Резерву вистачить на 3,5 міс. витрат і платежів; витрати коливаються на 20%",
		);
		expect(describeBuffer(buffer(3.5, 0.2), t.en, "en")).toBe(
			"The reserve covers 3.5 mths of expenses and payments; expenses vary by 20%",
		);
	});

	it("should keep a reserve above the model range", () => {
		expect(describeBuffer(buffer(24, 0.6), t.en, "en")).toBe(
			"The reserve covers 24 mths of expenses and payments; expenses vary by 60%",
		);
	});

	it("should say there is no reserve instead of 0 months", () => {
		expect(describeBuffer(buffer(0, 0.1), t.uk, "uk")).toBe(
			"Резерву немає; витрати коливаються на 10%",
		);
	});

	it("should explain that the reserve can't be counted without expenses and debt payments", () => {
		expect(describeBuffer(buffer(null, 0), t.uk, "uk")).toBe(
			"Витрат і платежів за боргами немає, тож резерв у місяцях не порахувати; витрати коливаються на 0%",
		);
	});

	it("should wait for the second month to show the variation", () => {
		expect(describeBuffer(buffer(2, null), t.en, "en")).toBe(
			"The reserve covers 2 mths of expenses and payments; expense swings show from the second month",
		);
	});
});
