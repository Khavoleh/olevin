/** Number of months with its unit, short with one decimal ("3.8 mths") or long ("3 months"). */
export const formatMonths = (
	value: number,
	language: string,
	unitDisplay: "short" | "long",
) =>
	new Intl.NumberFormat(language, {
		style: "unit",
		unit: "month",
		unitDisplay,
		maximumFractionDigits: 1,
	}).format(value);
