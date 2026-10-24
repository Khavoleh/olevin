/** Translation key of the plural form for the count, as `snapshots_few`. */
export const getPluralKey = (key: string, count: number, language: string) =>
	`${key}_${new Intl.PluralRules(language).select(count)}`;
