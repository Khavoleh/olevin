import type { MoneyListItem } from "./money-draft";

/** Empty amount for a list, with a new id. */
export const createMoneyListItem = (): MoneyListItem => ({
	id: crypto.randomUUID(),
});
