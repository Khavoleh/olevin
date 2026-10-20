import { describe, expect, it } from "vitest";
import { getFormEntries } from "./get-form-entries";

describe("getFormEntries", () => {
	it("should name nested objects and list items by their path", () => {
		expect(
			getFormEntries({
				income: [
					{ amount: 100, currency: "UAH" },
					{ amount: 2.5, currency: "USD" },
				],
				expenses: { food: { amount: 0, currency: "UAH" } },
				selfRating: 3,
			}),
		).toEqual([
			["income[0].amount", "100"],
			["income[0].currency", "UAH"],
			["income[1].amount", "2.5"],
			["income[1].currency", "USD"],
			["expenses.food.amount", "0"],
			["expenses.food.currency", "UAH"],
			["selfRating", "3"],
		]);
	});

	it("should skip missing values and empty lists", () => {
		expect(
			getFormEntries({ selfRating: undefined, reserves: [], note: null }),
		).toEqual([]);
	});
});
