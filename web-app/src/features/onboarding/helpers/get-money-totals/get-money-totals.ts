import type { MoneyDraft } from "@widgets/money-field";
import type { CurrencyCode, Money } from "../../api/onboarding.schemas";

/** Sum of the amounts in each currency, in the order the currencies first appear; empty amounts are left out. */
export const getMoneyTotals = (
	drafts: MoneyDraft[],
	baseCurrency: CurrencyCode,
): Money[] => {
	const totals = new Map<CurrencyCode, number>();

	for (const { amount, currency = baseCurrency } of drafts) {
		if (amount) totals.set(currency, (totals.get(currency) ?? 0) + amount);
	}

	return [...totals].map(([currency, amount]) => ({ amount, currency }));
};
