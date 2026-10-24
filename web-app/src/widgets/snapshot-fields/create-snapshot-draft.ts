import { createMoneyListItem } from "@widgets/money-field";
import { EXPENSE_CATEGORIES } from "./expense-categories";
import type { EstimateDraft, SnapshotDraft } from "./snapshot-draft";

/** Empty snapshot with one row in every list. */
export const createSnapshotDraft = (): SnapshotDraft => ({
	income: [createMoneyListItem()],
	expenses: Object.fromEntries(
		EXPENSE_CATEGORIES.map((category) => [category, {}]),
	) as SnapshotDraft["expenses"],
	saved: {},
	reserves: [createMoneyListItem()],
	debtPayments: [createMoneyListItem()],
});

/** Empty rough month. */
export const createEstimateDraft = (): EstimateDraft => ({
	expensesTotal: {},
	saved: {},
});
