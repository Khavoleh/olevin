import { PAGE_URLS } from "@shared/constants";
import { OnboardingStatus } from "../../api/onboarding.schemas";
import { ONBOARDING_STAGES } from "../../constants/onboarding-stages";

/** Page the user continues from, without the language prefix. */
export const getOnboardingPage = (status: OnboardingStatus): string =>
	status === OnboardingStatus.Completed
		? PAGE_URLS.HOME
		: `${PAGE_URLS.ONBOARDING}/${ONBOARDING_STAGES[status][0]}`;
