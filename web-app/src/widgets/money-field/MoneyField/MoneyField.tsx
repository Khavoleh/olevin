"use client";

import { XIcon } from "@phosphor-icons/react";
import { Button, NumberField } from "@shared/components";
import { getI18n } from "@shared/helpers";
import { CURRENCIES, type Currency } from "../constants/currencies";
import { MAX_AMOUNT } from "../constants/money-limits";
import type { MoneyDraft } from "../money-draft";
import { MONEY_FIELD_I18N } from "../money-field-i18n";
import AddonSelect from "./AddonSelect";

interface MoneyFieldProps {
	language: string;
	label: string;
	value: MoneyDraft;
	baseCurrency?: Currency;
	onChange: (value: MoneyDraft) => void;
	onRemove?: () => void;
}

/** Amount with its currency, the base one until another is chosen, and an optional button that removes it. */
const MoneyField = ({
	language,
	label,
	value,
	baseCurrency,
	onChange,
	onRemove,
}: Readonly<MoneyFieldProps>) => {
	const t = getI18n(language, MONEY_FIELD_I18N);

	return (
		<div className="flex min-w-0 items-end gap-1">
			<NumberField
				label={label}
				placeholder="0"
				value={value.amount ?? Number.NaN}
				onChange={(amount) =>
					onChange({
						...value,
						amount: Number.isNaN(amount) ? undefined : amount,
					})
				}
				minValue={0}
				maxValue={MAX_AMOUNT}
				formatOptions={{ maximumFractionDigits: 2 }}
				addon={
					<AddonSelect
						label={t("currency").replace("{label}", label)}
						options={CURRENCIES}
						value={value.currency ?? baseCurrency}
						onChange={(currency) =>
							onChange({ ...value, currency: currency as Currency })
						}
					/>
				}
				className="min-w-0 flex-1"
			/>
			{onRemove && (
				<Button
					variant="icon"
					aria-label={t("remove").replace("{label}", label)}
					onPress={onRemove}
				>
					<XIcon size={18} />
				</Button>
			)}
		</div>
	);
};

export default MoneyField;
