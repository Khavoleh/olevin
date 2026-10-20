import { describe, expect, it } from "vitest";
import type { SnapshotDraft } from "../../onboarding-store";
import { getFirstSnapshotRequest } from "./get-first-snapshot-request";

const DRAFT: SnapshotDraft = {
	income: [
		{ id: "salary", amount: 60000 },
		{ id: "freelance", amount: 500, currency: "USD" },
	],
	expenses: {
		food: { amount: 9000 },
		housing: { amount: 12000, currency: "EUR" },
		health: {},
		clothing: {},
		restaurants: {},
		transport: {},
		other: {},
	},
	saved: { amount: 4000 },
	reserves: [{ id: "cash" }],
	debtPayments: [],
	selfRating: 4,
	previousMonth: { expensesTotal: { amount: 42000 }, saved: {} },
	twoMonthsAgo: { expensesTotal: {}, saved: { amount: 5000, currency: "PLN" } },
};

describe("getFirstSnapshotRequest", () => {
	it("should keep the amounts and currencies that are set", () => {
		const request = getFirstSnapshotRequest(DRAFT, "UAH");

		expect(request.income).toEqual([
			{ amount: 60000, currency: "UAH" },
			{ amount: 500, currency: "USD" },
		]);
		expect(request.expenses.housing).toEqual({
			amount: 12000,
			currency: "EUR",
		});
		expect(request.twoMonthsAgo.saved).toEqual({
			amount: 5000,
			currency: "PLN",
		});
		expect(request.selfRating).toBe(4);
	});

	it("should turn empty amounts into 0 in the base currency", () => {
		const request = getFirstSnapshotRequest(DRAFT, "CAD");

		expect(request.expenses.health).toEqual({ amount: 0, currency: "CAD" });
		expect(request.reserves).toEqual([{ amount: 0, currency: "CAD" }]);
		expect(request.previousMonth.saved).toEqual({
			amount: 0,
			currency: "CAD",
		});
	});

	it("should leave the ids of the rows out of the request", () => {
		const request = getFirstSnapshotRequest(DRAFT, "UAH");

		for (const money of [...request.income, ...request.reserves]) {
			expect(money).not.toHaveProperty("id");
		}
	});

	it("should keep an empty list and a missing self-rating", () => {
		const request = getFirstSnapshotRequest(
			{ ...DRAFT, selfRating: undefined },
			"UAH",
		);

		expect(request.debtPayments).toEqual([]);
		expect(request.selfRating).toBeUndefined();
	});
});
