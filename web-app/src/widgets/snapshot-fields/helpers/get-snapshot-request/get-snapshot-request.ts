import type { Currency, MoneyDraft } from "@widgets/money-field";
import { EXPENSE_CATEGORIES } from "../../expense-categories";
import type {
	EstimateDraft,
	ExpenseCategory,
	Money,
	SnapshotDraft,
} from "../../snapshot-draft";

/** Amounts of an actual snapshot as the API takes them; the self-rating is missing until the user picks it. */
export interface SnapshotRequest {
	income: Money[];
	expenses: Record<ExpenseCategory, Money>;
	saved: Money;
	reserves: Money[];
	debtPayments: Money[];
	selfRating?: number;
}

/** Amounts of a rough month as the API takes them. */
export interface EstimateRequest {
	expensesTotal: Money;
	saved: Money;
}

const toMoney = (
	{ amount, currency }: MoneyDraft,
	baseCurrency: Currency,
): Money => ({
	amount: amount ?? 0,
	currency: currency ?? baseCurrency,
});

/** Request of an actual snapshot from the draft: empty amounts become 0, missing currencies the base one. */
export const getSnapshotRequest = (
	draft: SnapshotDraft,
	baseCurrency: Currency,
): SnapshotRequest => {
	const list = (items: MoneyDraft[]) =>
		items.map((item) => toMoney(item, baseCurrency));

	return {
		income: list(draft.income),
		expenses: Object.fromEntries(
			EXPENSE_CATEGORIES.map((category) => [
				category,
				toMoney(draft.expenses[category], baseCurrency),
			]),
		) as SnapshotRequest["expenses"],
		saved: toMoney(draft.saved, baseCurrency),
		reserves: list(draft.reserves),
		debtPayments: list(draft.debtPayments),
		selfRating: draft.selfRating,
	};
};

/** Request of a rough month from the draft: empty amounts become 0, missing currencies the base one. */
export const getEstimateRequest = (
	draft: EstimateDraft,
	baseCurrency: Currency,
): EstimateRequest => ({
	expensesTotal: toMoney(draft.expensesTotal, baseCurrency),
	saved: toMoney(draft.saved, baseCurrency),
});
