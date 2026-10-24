"use client";

import { FormBlock } from "@shared/components";
import { getI18n } from "@shared/helpers";
import MoneyField, { type Currency, MoneyList } from "@widgets/money-field";
import { SNAPSHOT_FIELDS_I18N } from "../i18n/snapshot-fields-i18n";
import type { SnapshotDraft } from "../snapshot-draft";
import ExpensesFields from "./ExpensesFields";
import SelfRatingField from "./SelfRatingField";

interface SnapshotFieldsProps {
	language: string;
	value: SnapshotDraft;
	baseCurrency?: Currency;
	selfRatingName: string;
	isSelfRatingInvalid: boolean;
	onChange: (value: Partial<SnapshotDraft>) => void;
}

/** Blocks of an actual snapshot: income, expenses, savings, reserves, debt payments and the self-rating. */
const SnapshotFields = ({
	language,
	value,
	baseCurrency,
	selfRatingName,
	isSelfRatingInvalid,
	onChange,
}: Readonly<SnapshotFieldsProps>) => {
	const t = getI18n(language, SNAPSHOT_FIELDS_I18N);

	return (
		<>
			<FormBlock title={t("income")} hint={t("income_hint")}>
				<MoneyList
					language={language}
					items={value.income}
					baseCurrency={baseCurrency}
					onChange={(income) => onChange({ income })}
				/>
			</FormBlock>

			<ExpensesFields
				language={language}
				value={value.expenses}
				baseCurrency={baseCurrency}
				onChange={(expenses) => onChange({ expenses })}
			/>

			<FormBlock title={t("saved")} hint={t("saved_hint")}>
				<div className="grid gap-2.5 lg:grid-cols-2">
					<MoneyField
						language={language}
						label={t("saved_label")}
						value={value.saved}
						baseCurrency={baseCurrency}
						onChange={(saved) => onChange({ saved })}
					/>
				</div>
			</FormBlock>

			<FormBlock title={t("reserves")} hint={t("reserves_hint")}>
				<MoneyList
					language={language}
					items={value.reserves}
					baseCurrency={baseCurrency}
					onChange={(reserves) => onChange({ reserves })}
				/>
			</FormBlock>

			<FormBlock title={t("debt_payments")} hint={t("debt_payments_hint")}>
				<MoneyList
					language={language}
					items={value.debtPayments}
					baseCurrency={baseCurrency}
					onChange={(debtPayments) => onChange({ debtPayments })}
				/>
			</FormBlock>

			<FormBlock>
				<SelfRatingField
					language={language}
					name={selfRatingName}
					value={value.selfRating}
					isInvalid={isSelfRatingInvalid}
					onChange={(selfRating) => onChange({ selfRating })}
				/>
			</FormBlock>
		</>
	);
};

export default SnapshotFields;
