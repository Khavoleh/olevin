/** Share as a percent with at most one decimal ("15%", "−20%"). */
export const formatPercent = (value: number, language: string) =>
	new Intl.NumberFormat(language, {
		style: "percent",
		maximumFractionDigits: 1,
	}).format(value);
