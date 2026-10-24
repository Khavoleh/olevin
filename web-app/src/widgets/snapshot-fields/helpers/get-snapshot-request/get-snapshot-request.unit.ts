import { describe, expect, it } from "vitest";
import type { SnapshotDraft } from "../../snapshot-draft";
import { getEstimateRequest, getSnapshotRequest } from "./get-snapshot-request";

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
};

describe("getSnapshotRequest", () => {
	it("should keep the amounts and currencies that are set", () => {
		const request = getSnapshotRequest(DRAFT, "UAH");

		expect(request.income).toEqual([
			{ amount: 60000, currency: "UAH" },
			{ amount: 500, currency: "USD" },
		]);
		expect(request.expenses.housing).toEqual({
			amount: 12000,
			currency: "EUR",
		});
		expect(request.selfRating).toBe(4);
	});

	it("should turn empty amounts into 0 in the base currency", () => {
		const request = getSnapshotRequest(DRAFT, "CAD");

		expect(request.expenses.health).toEqual({ amount: 0, currency: "CAD" });
		expect(request.reserves).toEqual([{ amount: 0, currency: "CAD" }]);
	});

	it("should leave the ids of the rows out of the request", () => {
		const request = getSnapshotRequest(DRAFT, "UAH");

		for (const money of [...request.income, ...request.reserves]) {
			expect(money).not.toHaveProperty("id");
		}
	});

	it("should keep an empty list and a missing self-rating", () => {
		const request = getSnapshotRequest(
			{ ...DRAFT, selfRating: undefined },
			"UAH",
		);

		expect(request.debtPayments).toEqual([]);
		expect(request.selfRating).toBeUndefined();
	});
});

describe("getEstimateRequest", () => {
	it("should fill the empty amounts with 0 in the base currency", () => {
		expect(
			getEstimateRequest(
				{ expensesTotal: { amount: 42000 }, saved: { currency: "PLN" } },
				"UAH",
			),
		).toEqual({
			expensesTotal: { amount: 42000, currency: "UAH" },
			saved: { amount: 0, currency: "PLN" },
		});
	});
});
