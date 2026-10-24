/** Index or its change with one decimal, as written in the language ("58.7"). */
export const formatIndex = (value: number, language: string) =>
	new Intl.NumberFormat(language, {
		minimumFractionDigits: 1,
		maximumFractionDigits: 1,
	}).format(value);
