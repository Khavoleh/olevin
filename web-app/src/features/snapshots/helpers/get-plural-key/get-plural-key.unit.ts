import { describe, expect, it } from "vitest";
import { getPluralKey } from "./get-plural-key";

describe("getPluralKey", () => {
	it.each([
		["uk", 1, "snapshots_one"],
		["uk", 3, "snapshots_few"],
		["uk", 6, "snapshots_many"],
		["uk", 21, "snapshots_one"],
		["en", 1, "snapshots_one"],
		["en", 6, "snapshots_other"],
	])("should pick the %s form for %d", (language, count, expected) => {
		expect(getPluralKey("snapshots", count, language)).toBe(expected);
	});
});
