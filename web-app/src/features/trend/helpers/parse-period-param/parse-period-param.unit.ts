import { describe, expect, it } from "vitest";
import { TrendPeriod } from "../../api/trend.schemas";
import { parsePeriodParam } from "./parse-period-param";

describe("parsePeriodParam", () => {
	it.each([
		["6m", TrendPeriod.SixMonths],
		["12m", TrendPeriod.TwelveMonths],
		["all", TrendPeriod.All],
	])("should read %s as %s", (param, expected) => {
		expect(parsePeriodParam(param)).toBe(expected);
	});

	it.each([
		["a missing", undefined],
		["an unknown", "24m"],
		["a repeated", ["12m", "all"]],
		["an API", "TwelveMonths"],
	])("should fall back to 6 months for %s parameter", (_, param) => {
		expect(parsePeriodParam(param)).toBe(TrendPeriod.SixMonths);
	});
});
