import { SUPPORTED_LANGUAGES } from "@shared/constants";

/**
 * Pre-renders every page of the `[lang]` segment for each supported language.
 */
export const getLanguageStaticParams = () => {
	return SUPPORTED_LANGUAGES.map((lang) => ({ lang }));
};
