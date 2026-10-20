/** Currencies an amount may be entered in, as the API names them. */
export const CURRENCIES = ["USD", "EUR", "CAD", "UAH", "PLN"] as const;

/** Currency an amount may be entered in. */
export type Currency = (typeof CURRENCIES)[number];
