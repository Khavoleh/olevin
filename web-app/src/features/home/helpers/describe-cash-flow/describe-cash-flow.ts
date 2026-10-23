import type { CashFlowResponse } from "../../api/home.schemas";
import { formatPercent } from "../format-percent/format-percent";

/** What is left of the income and what the debt payments take, or why it can't be shown. */
export const describeCashFlow = (
	{ savingsRate, debtToIncome }: CashFlowResponse,
	t: (key: string) => string,
	language: string,
) => {
	const percent = (value: number) => formatPercent(value, language);

	if (savingsRate === null || debtToIncome === null) {
		return t("no_income");
	}

	const left =
		savingsRate < 0
			? `${t("deficit")} ${percent(-savingsRate)}`
			: `${t("surplus")} ${percent(savingsRate)} ${t("of_income")}`;
	const debt =
		debtToIncome > 0
			? `${t("debt_payments")} ${percent(debtToIncome)} ${t("of_income")}`
			: t("no_debt_payments");

	return `${left}; ${debt}`;
};
