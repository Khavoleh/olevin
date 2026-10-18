import type { OnboardingStatus } from "../api/onboarding.schemas";

/** Steps the user may open for each onboarding status. */
export const ONBOARDING_STAGES: Record<OnboardingStatus, number[]> = {
	NotExist: [1, 2, 3],
	Settings: [1, 2, 3],
	Onboarding: [4, 5],
	CfpbResponse: [6],
	Completed: [7],
};
