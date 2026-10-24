import { describe, expect, it } from "vitest";
import type {
	GetContributionsResponse,
	IndicatorContributionResponse,
} from "../../api/home.schemas";
import { CONTRIBUTIONS_CARD_I18N } from "../../i18n/contributions-card-i18n";
import { describeContributions } from "./describe-contributions";

const t = {
	en: (key: string) => CONTRIBUTIONS_CARD_I18N.en[key],
	uk: (key: string) => CONTRIBUTIONS_CARD_I18N.uk[key],
};

const indicator = (
	points: number,
	from: number | null,
	to: number | null,
): IndicatorContributionResponse => ({ points, from, to });

const september = (): GetContributionsResponse => ({
	savingsRate: indicator(21, 0.08, 0.1),
	reserve: indicator(2.4, 2.922, 3.5),
	debtToIncome: indicator(0, 0.3, 0.3),
	expenseVariation: indicator(-2.2, 0.2, 0.23),
	savingsRegularity: {
		points: 7.5,
		fromSavingMonths: 2,
		fromMonths: 5,
		toSavingMonths: 3,
		toMonths: 6,
	},
	selfRating: { points: 2.5, from: 3, to: 4 },
});

const detailOf = (
	contributions: GetContributionsResponse,
	key: string,
	language: "en" | "uk",
) =>
	describeContributions(contributions, t[language], language).find(
		(line) => line.key === key,
	)?.detail;

describe("describeContributions", () => {
	it("should go from the largest rise to the largest fall", () => {
		expect(
			describeContributions(september(), t.en, "en").map((line) => line.key),
		).toEqual([
			"SavingsRate",
			"SavingsRegularity",
			"SelfRating",
			"Reserve",
			"DebtToIncome",
			"ExpenseVariation",
		]);
	});

	it("should write a share of income before and after", () => {
		expect(describeContributions(september(), t.uk, "uk")[0]).toEqual({
			key: "SavingsRate",
			label: "Залишок після витрат і боргів",
			detail: "8% → 10% доходу",
			points: 21,
		});
	});

	it("should write the reserve in months with one unit", () => {
		expect(detailOf(september(), "Reserve", "en")).toBe("2.9 → 3.5 mths");
	});

	it("should name the side without income", () => {
		const contributions = {
			...september(),
			debtToIncome: indicator(0, null, 0.3),
		};

		expect(detailOf(contributions, "DebtToIncome", "uk")).toBe(
			"немає доходу → 30% доходу",
		);
	});

	it("should say there is no income on both sides only once", () => {
		const contributions = {
			...september(),
			savingsRate: indicator(0, null, null),
		};

		expect(detailOf(contributions, "SavingsRate", "en")).toBe("no income");
	});

	it("should name a variation of a single month", () => {
		const contributions = {
			...september(),
			expenseVariation: indicator(0, null, 0.2),
		};

		expect(detailOf(contributions, "ExpenseVariation", "uk")).toBe(
			"один місяць → 20%",
		);
	});

	it("should write the months with savings of both windows", () => {
		expect(detailOf(september(), "SavingsRegularity", "uk")).toBe(
			"2 з 5 → 3 з 6 міс.",
		);
	});

	it("should write the self-rating out of 5", () => {
		expect(detailOf(september(), "SelfRating", "en")).toBe("3 → 4 of 5");
	});
});
