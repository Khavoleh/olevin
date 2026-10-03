import { DEFAULT_LANGUAGE, SUPPORTED_LANGUAGES } from "@shared/constants";

/** Whether the value is one of the supported language codes. */
const isSupportedLanguage = (
	language: string | undefined,
): language is string =>
	Boolean(language) && SUPPORTED_LANGUAGES.includes(language as string);

/**
 * Picks the language of a request: the choice saved in the cookie first,
 * then the first supported language from `Accept-Language`, then the default language.
 */
export const getLanguage = (
	preferred: string | undefined,
	acceptLanguage: string | null,
): string => {
	if (isSupportedLanguage(preferred)) {
		return preferred;
	}

	const accepted = (acceptLanguage ?? "")
		.split(",")
		.map((part) => part.trim().split(";")[0]?.split("-")[0]?.toLowerCase())
		.find(isSupportedLanguage);

	return accepted ?? DEFAULT_LANGUAGE;
};
