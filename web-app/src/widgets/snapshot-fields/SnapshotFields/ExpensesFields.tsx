"use client";

import { FormBlock, Text } from "@shared/components";
import { getI18n } from "@shared/helpers";
import MoneyField, { type Currency } from "@widgets/money-field";
import { EXPENSE_CATEGORIES } from "../expense-categories";
import { getMoneyTotals } from "../helpers/get-money-totals/get-money-totals";
import { SNAPSHOT_FIELDS_I18N } from "../i18n/snapshot-fields-i18n";
import type { SnapshotDraft } from "../snapshot-draft";

interface ExpensesFieldsProps {
	language: string;
	value: SnapshotDraft["expenses"];
	baseCurrency?: Currency;
	onChange: (value: SnapshotDraft["expenses"]) => void;
}

/** Expenses of the month by category with their total in each currency. */
const ExpensesFields = ({
	language,
	value,
	baseCurrency,
	onChange,
}: Readonly<ExpensesFieldsProps>) => {
	const t = getI18n(language, SNAPSHOT_FIELDS_I18N);
	const totals = baseCurrency
		? getMoneyTotals(Object.values(value), baseCurrency)
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
				{EXPENSE_CATEGORIES.map((category) => (
					<MoneyField
						key={category}
						language={language}
						label={t(category)}
						value={value[category]}
						baseCurrency={baseCurrency}
						onChange={(money) => onChange({ ...value, [category]: money })}
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
