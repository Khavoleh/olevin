import { getOnboardingPage, syncOnboardingStatus } from "@features/onboarding";
import { handleSignIn } from "@logto/next/server-actions";
import { PAGE_URLS } from "@shared/constants";
import { getLogtoConfig } from "@shared/helpers";
import { redirect } from "next/navigation";
import type { NextRequest } from "next/server";

/** Logto redirect URI: signs the user in and sends them home or to their onboarding step. */
export async function GET(request: NextRequest) {
	await handleSignIn(getLogtoConfig(), request.nextUrl.searchParams);

	const status = await syncOnboardingStatus();

	redirect(status ? getOnboardingPage(status) : PAGE_URLS.SIGN_IN);
}
