"use client";

import { PlusIcon } from "@phosphor-icons/react";
import { Button } from "@shared/components";
import { getI18n } from "@shared/helpers";
import type { Currency } from "./constants/currencies";
import { MAX_AMOUNTS } from "./constants/money-limits";
import { createMoneyListItem } from "./create-money-list-item";
import MoneyField from "./MoneyField/MoneyField";
import type { MoneyListItem } from "./money-draft";
import { MONEY_FIELD_I18N } from "./money-field-i18n";

interface MoneyListProps {
	language: string;
	items: MoneyListItem[];
	baseCurrency?: Currency;
	onChange: (items: MoneyListItem[]) => void;
}

/** Value entered as several amounts in different currencies. */
const MoneyList = ({
	language,
	items,
	baseCurrency,
	onChange,
}: Readonly<MoneyListProps>) => {
	const t = getI18n(language, MONEY_FIELD_I18N);

	return (
		<>
			<div className="grid gap-2.5 lg:grid-cols-2">
				{items.map((item, index) => (
					<MoneyField
						key={item.id}
						language={language}
						label={t("amount").replace("{number}", String(index + 1))}
						value={item}
						baseCurrency={baseCurrency}
						onChange={(value) =>
							onChange(
								items.map((current) =>
									current.id === item.id ? { ...value, id: item.id } : current,
								),
							)
						}
						onRemove={
							items.length > 1
								? () =>
										onChange(items.filter((current) => current.id !== item.id))
								: undefined
						}
					/>
				))}
			</div>
			{items.length < MAX_AMOUNTS && (
				<Button
					variant="ghost"
					onPress={() => onChange([...items, createMoneyListItem()])}
					className="justify-self-start"
				>
					<PlusIcon size={18} />
					{t("add_amount")}
				</Button>
			)}
		</>
	);
};

export default MoneyList;
