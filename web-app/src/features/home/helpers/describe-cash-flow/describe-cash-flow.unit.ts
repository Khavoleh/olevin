import { describe, expect, it } from "vitest";
import type { CashFlowResponse } from "../../api/home.schemas";
import { COMPOSITION_CARD_I18N } from "../../i18n/composition-card-i18n";
import { describeCashFlow } from "./describe-cash-flow";

const t = {
	en: (key: string) => COMPOSITION_CARD_I18N.en[key],
	uk: (key: string) => COMPOSITION_CARD_I18N.uk[key],
};

const cashFlow = (
	savingsRate: number | null,
	debtToIncome: number | null,
): CashFlowResponse => ({
	value: 50,
	term: "Moderate",
	savingsRate,
	debtToIncome,
});

describe("describeCashFlow", () => {
	it("should write what is left and what the debts take", () => {
		expect(describeCashFlow(cashFlow(0.1, 0.3), t.uk, "uk")).toBe(
			"Після витрат і боргів лишається 10% доходу; платежі за боргами — 30% доходу",
		);
		expect(describeCashFlow(cashFlow(0.1, 0.3), t.en, "en")).toBe(
			"Left after expenses and debts: 10% of income; debt payments take 30% of income",
		);
	});

	it("should write a deficit as the amount over the income", () => {
		expect(describeCashFlow(cashFlow(-0.35, 0.3), t.uk, "uk")).toBe(
			"Витрати й борги перевищують дохід на 35%; платежі за боргами — 30% доходу",
		);
	});

	it("should say there are no debt payments instead of 0%", () => {
		expect(describeCashFlow(cashFlow(0.25, 0), t.en, "en")).toBe(
			"Left after expenses and debts: 25% of income; no debt payments",
		);
	});

	it("should explain that shares can't be worked out without income", () => {
		expect(describeCashFlow(cashFlow(null, null), t.uk, "uk")).toBe(
			"Доходу за місяць немає, тож частки від доходу не порахувати",
		);
	});
});
