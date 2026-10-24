/** Change of the index with its sign and one decimal, as written in the language ("+3.6"). */
export const formatChange = (value: number, language: string) =>
	new Intl.NumberFormat(language, {
		minimumFractionDigits: 1,
		maximumFractionDigits: 1,
		signDisplay: "exceptZero",
	}).format(value);
