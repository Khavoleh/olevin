import { describe, expect, it } from "vitest";
import type { GetSnapshotMonthResponseActualMonthResponse } from "../../api/snapshots.schemas";
import { getMoneyDraft, getSnapshotDraft } from "./get-snapshot-draft";

const UAH = (amount: number) => ({ amount, currency: "UAH" as const });

const SNAPSHOT: GetSnapshotMonthResponseActualMonthResponse = {
	kind: "Actual",
	id: "october",
	laterSnapshots: 0,
	income: [UAH(60000), { amount: 500, currency: "USD" }],
	expenses: {
		food: UAH(9000),
		housing: UAH(12000),
		health: UAH(0),
		clothing: UAH(0),
		restaurants: UAH(0),
		transport: UAH(0),
		other: UAH(0),
	},
	saved: UAH(4000),
	reserves: [UAH(0)],
	debtPayments: [],
	selfRating: 4,
};

describe("getMoneyDraft", () => {
	it("should show a 0 as an empty amount and keep its currency", () => {
		expect(getMoneyDraft({ amount: 0, currency: "EUR" })).toEqual({
			amount: undefined,
			currency: "EUR",
		});
	});
});

describe("getSnapshotDraft", () => {
	it("should keep the amounts with their currencies", () => {
		const draft = getSnapshotDraft(SNAPSHOT);

		expect(
			draft.income.map(({ amount, currency }) => [amount, currency]),
		).toEqual([
			[60000, "UAH"],
			[500, "USD"],
		]);
		expect(draft.expenses.housing).toEqual({ amount: 12000, currency: "UAH" });
		expect(draft.selfRating).toBe(4);
	});

	it("should give every row of a list its own id", () => {
		const [first, second] = getSnapshotDraft(SNAPSHOT).income;

		expect(first.id).not.toBe(second.id);
	});

	it("should leave one empty row in an empty list", () => {
		const { debtPayments } = getSnapshotDraft(SNAPSHOT);

		expect(debtPayments).toHaveLength(1);
		expect(debtPayments[0].amount).toBeUndefined();
	});
});
