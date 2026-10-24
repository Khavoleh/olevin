/** Change in points with its sign and one decimal ("+1.6", "-0.2", "0.0"). */
export const formatPoints = (value: number, language: string) =>
	new Intl.NumberFormat(language, {
		signDisplay: "exceptZero",
		minimumFractionDigits: 1,
		maximumFractionDigits: 1,
	}).format(value);
