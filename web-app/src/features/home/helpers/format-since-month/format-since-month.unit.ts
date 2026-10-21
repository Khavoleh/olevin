import { describe, expect, it } from "vitest";
import { CalendarMonth } from "../../api/home.schemas";
import { formatSinceMonth } from "./format-since-month";

describe("formatSinceMonth", () => {
	it("should put the month in the genitive case in Ukrainian", () => {
		expect(formatSinceMonth(2026, CalendarMonth.August, "uk")).toBe("серпня");
	});

	it("should keep the month name as it is in English", () => {
		expect(formatSinceMonth(2026, CalendarMonth.August, "en")).toBe("August");
	});
});
