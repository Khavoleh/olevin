import { SUPPORTED_LANGUAGES } from "@shared/constants";

/**
 * Returns the language from the first segment of the path, or `undefined` when the path has no language prefix.
 */
export const getPathLanguage = (pathname: string): string | undefined => {
	const [, language] = pathname.split("/");

	return language && SUPPORTED_LANGUAGES.includes(language)
		? language
		: undefined;
};
