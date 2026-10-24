import type { MoneyDraft, MoneyListItem } from "@widgets/money-field";
import type { Money, SnapshotDraft } from "@widgets/snapshot-fields";
import type { GetSnapshotMonthResponseActualMonthResponse } from "../../api/snapshots.schemas";

/** Saved amount as typed: a 0 shows as an empty field. */
export const getMoneyDraft = ({ amount, currency }: Money): MoneyDraft => ({
	amount: amount || undefined,
	currency,
});

const getListDraft = (items: Money[]): MoneyListItem[] =>
	items.length
		? items.map((item) => ({ id: crypto.randomUUID(), ...getMoneyDraft(item) }))
		: [{ id: crypto.randomUUID() }];

/** Draft of the saved snapshot, with a row id for every amount of a list. */
export const getSnapshotDraft = ({
	income,
	expenses,
	saved,
	reserves,
	debtPayments,
	selfRating,
}: GetSnapshotMonthResponseActualMonthResponse): SnapshotDraft => ({
	income: getListDraft(income),
	expenses: {
		food: getMoneyDraft(expenses.food),
		housing: getMoneyDraft(expenses.housing),
		health: getMoneyDraft(expenses.health),
		clothing: getMoneyDraft(expenses.clothing),
		restaurants: getMoneyDraft(expenses.restaurants),
		transport: getMoneyDraft(expenses.transport),
		other: getMoneyDraft(expenses.other),
	},
	saved: getMoneyDraft(saved),
	reserves: getListDraft(reserves),
	debtPayments: getListDraft(debtPayments),
	selfRating,
});
