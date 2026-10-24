import type { Currency, MoneyDraft } from "@widgets/money-field";
import type { Money } from "../../snapshot-draft";

/** Sum of the amounts in each currency, in the order the currencies first appear; empty amounts are left out. */
export const getMoneyTotals = (
	drafts: MoneyDraft[],
	baseCurrency: Currency,
): Money[] => {
	const totals = new Map<Currency, number>();

	for (const { amount, currency = baseCurrency } of drafts) {
		if (amount) totals.set(currency, (totals.get(currency) ?? 0) + amount);
	}

	return [...totals].map(([currency, amount]) => ({ amount, currency }));
};
