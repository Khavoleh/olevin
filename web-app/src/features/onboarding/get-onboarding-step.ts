import { ONBOARDING_STEPS } from "./onboarding-steps";

/** Returns the step number from the `[step]` segment of the URL, or `undefined` when there is no such step. */
export const getOnboardingStep = (segment: string): number | undefined =>
	ONBOARDING_STEPS.find((step) => String(step) === segment);
