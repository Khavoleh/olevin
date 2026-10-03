import { SUPPORTED_LANGUAGES } from "@shared/constants";

/**
 * Puts the language in front of the path, replacing the language prefix the path already has.
 */
export const getLanguageUrl = (pathname: string, language: string): string => {
	const pathParts = pathname.split("/").filter(Boolean);

	if (pathParts[0] && SUPPORTED_LANGUAGES.includes(pathParts[0])) {
		pathParts.shift();
	}

	return `/${[language, ...pathParts].join("/")}`;
};
