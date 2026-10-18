import { ONBOARDED_COOKIE } from "@shared/constants";
import { ApiError, getLogtoConfig } from "@shared/helpers";
import { cookies } from "next/headers";
import { createUser, getOnboardingStatus } from "../api/onboarding";
import { OnboardingStatus } from "../api/onboarding.schemas";

/** Gets the onboarding status, creating the user on the first sign-in. */
async function fetchOnboardingStatus(): Promise<OnboardingStatus> {
	const { status, data } = await getOnboardingStatus();

	if (status !== 200) {
		throw new ApiError(status);
	}

	if (data.data !== OnboardingStatus.NotExist) {
		return data.data;
	}

	const { status: createStatus } = await createUser();

	if (createStatus !== 201 && createStatus !== 409) {
		throw new ApiError(createStatus);
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
