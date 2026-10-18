import { getOnboardingPage, syncOnboardingStatus } from "@features/onboarding";
import { PAGE_URLS } from "@shared/constants";
import { getLanguageUrl } from "@shared/helpers";
import { redirect } from "next/navigation";
import type { NextRequest } from "next/server";

interface OnboardingRouteContext {
	params: Promise<{ lang: string }>;
}

/** Sends the user to the onboarding step they continue from, or home. */
export async function GET(
	_request: NextRequest,
	{ params }: OnboardingRouteContext,
) {
	const { lang } = await params;
	const status = await syncOnboardingStatus();

	redirect(
		getLanguageUrl(
			status ? getOnboardingPage(status) : PAGE_URLS.SIGN_IN,
			lang,
		),
	);
}
