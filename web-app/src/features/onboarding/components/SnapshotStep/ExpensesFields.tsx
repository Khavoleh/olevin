"use client";

import { Text } from "@shared/components";
import { getI18n } from "@shared/helpers";
import MoneyField from "@widgets/money-field";
import type { ExpensesRequest } from "../../api/onboarding.schemas";
import { getMoneyTotals } from "../../helpers/get-money-totals/get-money-totals";
import { SNAPSHOT_STEP_I18N } from "../../i18n/snapshot-step-i18n";
import { useOnboardingStore } from "../../onboarding-store";
import FormBlock from "../FormBlock";

const CATEGORIES: (keyof ExpensesRequest)[] = [
	"food",
	"housing",
	"health",
	"clothing",
	"restaurants",
	"transport",
	"other",
];

interface ExpensesFieldsProps {
	language: string;
}

/** Expenses of the month by category with their total in each currency. */
const ExpensesFields = ({ language }: Readonly<ExpensesFieldsProps>) => {
	const t = getI18n(language, SNAPSHOT_STEP_I18N);
	const baseCurrency = useOnboardingStore((state) => state.baseCurrency);
	const expenses = useOnboardingStore((state) => state.snapshot.expenses);
	const setSnapshot = useOnboardingStore((state) => state.setSnapshot);
	const totals = baseCurrency
		? getMoneyTotals(Object.values(expenses), baseCurrency)
		: [];
	const total =
		totals
			.map(({ amount, currency }) =>
				new Intl.NumberFormat(language, {
					style: "currency",
					currency,
					currencyDisplay: "narrowSymbol",
					minimumFractionDigits: 0,
					maximumFractionDigits: 2,
				}).format(amount),
			)
			.join(" + ") || "0";

	return (
		<FormBlock title={t("expenses")} hint={t("expenses_hint")}>
			<div className="grid gap-2.5 lg:grid-cols-2">
				{CATEGORIES.map((category) => (
					<MoneyField
						key={category}
						language={language}
						label={t(category)}
						value={expenses[category]}
						baseCurrency={baseCurrency}
						onChange={(value) =>
							setSnapshot({ expenses: { ...expenses, [category]: value } })
						}
					/>
				))}
			</div>
			<div
				aria-live="polite"
				className="flex justify-between gap-2.5 border-border border-t pt-2.5"
			>
				<Text elementType="span">{t("total")}</Text>
				<Text elementType="span" className="font-semibold tabular-nums">
					{total}
				</Text>
			</div>
		</FormBlock>
	);
};

export default ExpensesFields;
