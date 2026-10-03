import { DEFAULT_LANGUAGE, SUPPORTED_LANGUAGES } from "@shared/constants";
import type { I18N } from "@shared/interfaces";

/** Own translation of the key, ignoring `Object.prototype` members like `constructor`. */
const lookup = (keys: Record<string, string>, key: string) =>
	Object.hasOwn(keys, key) ? keys[key] : undefined;

/**
 * Returns `t(key)` for the given language. Falls back to the default language, then to the key itself.
 */
export const getI18n = (language: string, translations: I18N) => {
	const current = (
		SUPPORTED_LANGUAGES.includes(language) ? language : DEFAULT_LANGUAGE
	) as keyof I18N;

	return (key: string): string => {
		return (
			lookup(translations[current], key) ??
			lookup(translations[DEFAULT_LANGUAGE as keyof I18N], key) ??
			key
		);
	};
};
