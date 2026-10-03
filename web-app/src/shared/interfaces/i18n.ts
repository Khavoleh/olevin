/** Translations of one language: key → text. */
type I18NKeys = Record<string, string>;

/** Translations of a feature or widget, one dictionary per supported language. */
export interface I18N {
	en: I18NKeys;
	uk: I18NKeys;
}
