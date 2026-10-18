import { describe, expect, it } from "vitest";
import { OnboardingStatus } from "../../api/onboarding.schemas";
import { getOnboardingPage } from "./get-onboarding-page";

describe("getOnboardingPage", () => {
	it("should return the first step of the stage the user has not completed", () => {
		const cases: [OnboardingStatus, string][] = [
			[OnboardingStatus.NotExist, "/onboarding/1"],
			[OnboardingStatus.Settings, "/onboarding/1"],
			[OnboardingStatus.Onboarding, "/onboarding/4"],
			[OnboardingStatus.CfpbResponse, "/onboarding/6"],
		];

		for (const [status, page] of cases) {
			expect(getOnboardingPage(status)).toBe(page);
		}
	});

	it("should return the home page once the onboarding is completed", () => {
		expect(getOnboardingPage(OnboardingStatus.Completed)).toBe("/");
	});
});
