"use server";

import { signIn as logtoSignIn } from "@logto/next/server-actions";
import { DEFAULT_LANGUAGE } from "@shared/constants";
import { getLogtoConfig, getPathLanguage } from "@shared/helpers";

/**
 * Redirects to the Logto sign-in page in the given language and returns to the app in the same language.
 */
export async function signIn(language: string): Promise<void> {
	const config = getLogtoConfig();
	const uiLanguage = getPathLanguage(`/${language}`) ?? DEFAULT_LANGUAGE;

	await logtoSignIn(config, {
		redirectUri: `${config.baseUrl}/callback`,
		postRedirectUri: `${config.baseUrl}/${uiLanguage}`,
		extraParams: { ui_locales: uiLanguage },
	});
}
