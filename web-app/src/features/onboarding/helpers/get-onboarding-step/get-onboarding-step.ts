import { ONBOARDING_STEPS } from "../../constants/onboarding-steps";

/** Step number from the URL segment, or `undefined` if there is no such step. */
export const getOnboardingStep = (segment: string): number | undefined =>
	ONBOARDING_STEPS.find((step) => String(step) === segment);
