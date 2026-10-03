import { LANGUAGE_COOKIE } from "@shared/constants";
import { getLanguageUrl } from "@shared/helpers";

/**
 * Saves the chosen language and opens the current page in it.
 */
export const setLanguage = async (language: string): Promise<void> => {
	await cookieStore.set({
		name: LANGUAGE_COOKIE.NAME,
		value: language,
		path: "/",
		expires: Date.now() + LANGUAGE_COOKIE.MAX_AGE * 1000,
		sameSite: "lax",
	});

	window.location.assign(
		getLanguageUrl(window.location.pathname, language) + window.location.search,
	);
};
