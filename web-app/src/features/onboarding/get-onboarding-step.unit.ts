import { describe, expect, it } from "vitest";
import { getOnboardingStep } from "./get-onboarding-step";

describe("getOnboardingStep", () => {
	it("should return the number of an existing step", () => {
		expect(getOnboardingStep("1")).toBe(1);
		expect(getOnboardingStep("8")).toBe(8);
	});

	it("should return undefined for a step outside the flow", () => {
		expect(getOnboardingStep("0")).toBeUndefined();
		expect(getOnboardingStep("9")).toBeUndefined();
	});

	it("should return undefined for a segment that is not a plain number", () => {
		expect(getOnboardingStep("")).toBeUndefined();
		expect(getOnboardingStep("03")).toBeUndefined();
		expect(getOnboardingStep("2.5")).toBeUndefined();
		expect(getOnboardingStep("abc")).toBeUndefined();
	});
});
