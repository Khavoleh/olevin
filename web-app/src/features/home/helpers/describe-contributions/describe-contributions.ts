import type { GetContributionsResponse } from "../../api/home.schemas";
import { formatPercent } from "../format-percent/format-percent";

/** One line of the change: the factor, its values before and after and the points. */
export interface ContributionLine {
	key: string;
	label: string;
	detail: string;
	points: number;
}

const describeRange = (
	from: number | null,
	to: number | null,
	format: (value: number) => string,
	unit: string,
	empty: string,
) => {
	if (from !== null && to !== null) {
		return `${format(from)} → ${format(to)}${unit}`;
	}

	const side = (value: number | null) =>
		value === null ? empty : `${format(value)}${unit}`;

	return from === null && to === null ? empty : `${side(from)} → ${side(to)}`;
};

/** Lines of every factor, from the largest rise to the largest fall ("8% → 10% of income"). */
export const describeContributions = (
	{
		savingsRate,
		reserve,
		debtToIncome,
		expenseVariation,
		savingsRegularity,
		selfRating,
	}: GetContributionsResponse,
	t: (key: string) => string,
	language: string,
): ContributionLine[] => {
	const percent = (value: number) => formatPercent(value, language);
	const number = (value: number) =>
		new Intl.NumberFormat(language, { maximumFractionDigits: 1 }).format(value);
	const ofIncome = ` ${t("of_income")}`;

	const lines = [
		{
			key: "SavingsRate",
			points: savingsRate.points,
			detail: describeRange(
				savingsRate.from,
				savingsRate.to,
				percent,
				ofIncome,
				t("no_income"),
			),
		},
		{
			key: "Reserve",
			points: reserve.points,
			detail: describeRange(
				reserve.from,
				reserve.to,
				number,
				` ${t("months")}`,
				t("no_obligations"),
			),
		},
		{
			key: "DebtToIncome",
			points: debtToIncome.points,
			detail: describeRange(
				debtToIncome.from,
				debtToIncome.to,
				percent,
				ofIncome,
				t("no_income"),
			),
		},
		{
			key: "ExpenseVariation",
			points: expenseVariation.points,
			detail: describeRange(
				expenseVariation.from,
				expenseVariation.to,
				percent,
				"",
				t("one_month"),
			),
		},
		{
			key: "SavingsRegularity",
			points: savingsRegularity.points,
			detail: `${savingsRegularity.fromSavingMonths} ${t("of")} ${savingsRegularity.fromMonths} → ${savingsRegularity.toSavingMonths} ${t("of")} ${savingsRegularity.toMonths} ${t("months")}`,
		},
		{
			key: "SelfRating",
			points: selfRating.points,
			detail: `${selfRating.from} → ${selfRating.to} ${t("of")} 5`,
		},
	];

	return lines
		.map((line) => ({ ...line, label: t(`factor_${line.key}`) }))
		.toSorted((a, b) => b.points - a.points);
};
