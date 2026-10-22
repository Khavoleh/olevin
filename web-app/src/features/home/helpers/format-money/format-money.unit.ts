import { describe, expect, it } from "vitest";
import { formatMoney } from "./format-money";

describe("formatMoney", () => {
	it.each([
		["en", 4_545.45, "UAH", "₴4,545"],
		["uk", 4_545.45, "UAH", "4\u00a0545\u00a0₴"],
		["en", 1_750, "USD", "$1,750"],
		["uk", 0.5, "EUR", "1\u00a0€"],
	])("should write %s %d %s as %s", (language, amount, currency, expected) => {
		expect(formatMoney(amount, currency, language)).toBe(expected);
	});
});
