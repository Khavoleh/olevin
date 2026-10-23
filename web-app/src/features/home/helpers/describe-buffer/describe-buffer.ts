import type { BufferResponse } from "../../api/home.schemas";
import { formatMonths } from "../format-months/format-months";
import { formatPercent } from "../format-percent/format-percent";

const describeReserve = (
	reserveMonths: number | null,
	t: (key: string) => string,
	language: string,
) => {
	if (reserveMonths === null) {
		return t("no_obligations");
	}

	if (reserveMonths === 0) {
		return t("no_reserve");
	}

	return `${t("reserve_covers")} ${formatMonths(reserveMonths, language, "short")} ${t("of_obligations")}`;
};

/** How many months the reserve covers and how much the expenses vary, or why it can't be shown. */
export const describeBuffer = (
	{ reserveMonths, expenseVariation }: BufferResponse,
	t: (key: string) => string,
	language: string,
) => {
	const variation =
		expenseVariation === null
			? t("variation_unknown")
			: `${t("expenses_vary")} ${formatPercent(expenseVariation, language)}`;

	return `${describeReserve(reserveMonths, t, language)}; ${variation}`;
};
