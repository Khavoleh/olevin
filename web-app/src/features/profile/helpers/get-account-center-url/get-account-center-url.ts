const UI_LOCALES: Record<string, string> = { uk: "uk-UA", en: "en" };

/** Security page of the Logto Account Center, where the user changes their username, email, password and linked accounts. */
export const getAccountCenterUrl = (endpoint: string, language: string) => {
	const url = new URL("/account/security", endpoint);

	url.searchParams.set("ui_locales", UI_LOCALES[language] ?? language);

	return url.href;
};
