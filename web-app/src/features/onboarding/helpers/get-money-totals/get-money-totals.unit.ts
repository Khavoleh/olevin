import { describe, expect, it } from "vitest";
import { getMoneyTotals } from "./get-money-totals";

describe("getMoneyTotals", () => {
	it("should add up the amounts of each currency", () => {
		expect(
			getMoneyTotals(
				[
					{ amount: 9000 },
					{ amount: 100, currency: "USD" },
					{ amount: 12000, currency: "UAH" },
					{ amount: 50, currency: "USD" },
				],
				"UAH",
			),
		).toEqual([
			{ amount: 21000, currency: "UAH" },
			{ amount: 150, currency: "USD" },
		]);
	});

	it("should leave out empty and zero amounts", () => {
		expect(getMoneyTotals([{}, { amount: 0, currency: "EUR" }], "UAH")).toEqual(
			[],
		);
	});
});
