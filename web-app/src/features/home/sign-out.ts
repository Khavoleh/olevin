"use server";

import { signOut as logtoSignOut } from "@logto/next/server-actions";
import { getLogtoConfig } from "@shared/helpers";

/**
 * Clears the session and redirects to the Logto sign-out page.
 */
export async function signOut(): Promise<void> {
	const config = getLogtoConfig();

	await logtoSignOut(config, config.baseUrl);
}
