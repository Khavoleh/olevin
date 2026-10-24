import type { Currency, MoneyDraft, MoneyListItem } from "@widgets/money-field";
import type { EXPENSE_CATEGORIES } from "./expense-categories";

/** Category of the monthly expenses. */
export type ExpenseCategory = (typeof EXPENSE_CATEGORIES)[number];

/** Amount in its currency, as the API takes it. */
export interface Money {
	amount: number;
	currency: Currency;
}

/** Rough month with only its total expenses and savings. */
export interface EstimateDraft {
	expensesTotal: MoneyDraft;
	saved: MoneyDraft;
}

/** Amounts of an actual snapshot as typed; the self-rating is missing until the user picks it. */
export interface SnapshotDraft {
	income: MoneyListItem[];
	expenses: Record<ExpenseCategory, MoneyDraft>;
	saved: MoneyDraft;
	reserves: MoneyListItem[];
	debtPayments: MoneyListItem[];
	selfRating?: number;
}
