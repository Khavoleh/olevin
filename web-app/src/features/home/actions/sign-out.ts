"use server";

import { signOut as logtoSignOut } from "@logto/next/server-actions";
import { ONBOARDED_COOKIE } from "@shared/constants";
import { getLogtoConfig } from "@shared/helpers";
import { cookies } from "next/headers";

/**
 * Clears the session and the onboarding mark, then redirects to the Logto sign-out page.
 */
export async function signOut(): Promise<void> {
	const config = getLogtoConfig();

	(await cookies()).delete(ONBOARDED_COOKIE.NAME);

	await logtoSignOut(config, config.baseUrl);
}
