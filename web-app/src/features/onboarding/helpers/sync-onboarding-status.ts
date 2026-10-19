import { ONBOARDED_COOKIE } from "@shared/constants";
import { ApiError, getLogtoConfig } from "@shared/helpers";
import { cookies } from "next/headers";
import { createUser, getOnboardingStatus } from "../api/onboarding";
import { OnboardingStatus } from "../api/onboarding.schemas";

/** Gets the onboarding status, creating the user on the first sign-in. */
async function fetchOnboardingStatus(): Promise<OnboardingStatus> {
	const response = await getOnboardingStatus();

	if (response.status !== 200) {
		throw new ApiError(response);
	}

	if (response.data.data !== OnboardingStatus.NotExist) {
		return response.data.data;
	}

	const createResponse = await createUser();

	if (createResponse.status !== 201 && createResponse.status !== 409) {
		throw new ApiError(createResponse);
	}

	return OnboardingStatus.Settings;
}

/** Gets the onboarding status and remembers a completed onboarding in a cookie. Returns `undefined` without a session. */
export async function syncOnboardingStatus(): Promise<
	OnboardingStatus | undefined
> {
	let status: OnboardingStatus;

	try {
		status = await fetchOnboardingStatus();
	} catch (error) {
		if (error instanceof ApiError && error.status === 401) {
			return undefined;
		}

		throw error;
	}

	if (status === OnboardingStatus.Completed) {
		(await cookies()).set(ONBOARDED_COOKIE.NAME, "1", {
			httpOnly: true,
			secure: getLogtoConfig().cookieSecure,
			sameSite: "lax",
			path: "/",
			maxAge: ONBOARDED_COOKIE.MAX_AGE,
		});
	}

	return status;
}
