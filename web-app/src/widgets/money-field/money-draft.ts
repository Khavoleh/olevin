import type { Currency } from "./constants/currencies";

/** Amount as typed; an empty amount counts as 0 and a missing currency is the base one. */
export interface MoneyDraft {
	amount?: number;
	currency?: Currency;
}

/** Amount in a list, with the id that keeps its row when another one is removed. */
export interface MoneyListItem extends MoneyDraft {
	id: string;
}
