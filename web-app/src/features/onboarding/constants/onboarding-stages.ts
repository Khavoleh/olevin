import type { OnboardingStatus } from "../api/onboarding.schemas";

/** Steps the user may open for each onboarding status. */
export const ONBOARDING_STAGES: Record<OnboardingStatus, number[]> = {
	NotExist: [1],
	Settings: [1],
	Onboarding: [2, 3],
	CfpbResponse: [4],
	Completed: [],
};
