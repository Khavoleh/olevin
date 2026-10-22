/** Amount in whole units of its currency, as written in the language ("$45"). */
export const formatMoney = (
	amount: number,
	currency: string,
	language: string,
) =>
	new Intl.NumberFormat(language, {
		style: "currency",
		currency,
		currencyDisplay: "narrowSymbol",
		maximumFractionDigits: 0,
	}).format(amount);
