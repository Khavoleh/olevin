"use client";

import { FormBlock } from "@shared/components";
import { getI18n } from "@shared/helpers";
import MoneyField, { type Currency } from "@widgets/money-field";
import { ESTIMATE_FIELDS_I18N } from "./i18n/estimate-fields-i18n";
import type { EstimateDraft } from "./snapshot-draft";

const FIELD_LABELS: Record<keyof EstimateDraft, string> = {
	expensesTotal: "expenses_total",
	saved: "saved",
};

interface EstimateFieldsProps {
	language: string;
	title: string;
	value: EstimateDraft;
	baseCurrency?: Currency;
	onChange: (value: EstimateDraft) => void;
}

/** Block of a rough month: its total expenses and savings. */
const EstimateFields = ({
	language,
	title,
	value,
	baseCurrency,
	onChange,
}: Readonly<EstimateFieldsProps>) => {
	const t = getI18n(language, ESTIMATE_FIELDS_I18N);

	return (
		<FormBlock title={title}>
			<div className="grid gap-2.5 lg:grid-cols-2">
				{(["expensesTotal", "saved"] as const).map((field) => (
					<MoneyField
						key={field}
						language={language}
						label={t(FIELD_LABELS[field])}
						value={value[field]}
						baseCurrency={baseCurrency}
						onChange={(money) => onChange({ ...value, [field]: money })}
					/>
				))}
			</div>
		</FormBlock>
	);
};

export default EstimateFields;
