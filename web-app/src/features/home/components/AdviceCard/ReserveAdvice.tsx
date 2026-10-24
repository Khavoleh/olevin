import { Text } from "@shared/components";
import { formatMoney, getI18n } from "@shared/helpers";
import type { AdviceResponseReserveAdviceResponse } from "../../api/home.schemas";
import { formatMonths } from "../../helpers/format-months/format-months";
import { ADVICE_CARD_I18N } from "../../i18n/advice-card-i18n";
import AdviceChange from "./AdviceChange";
import AdviceRow from "./AdviceRow";

interface ReserveAdviceProps {
	advice: AdviceResponseReserveAdviceResponse;
	rank: number;
	currency: string;
	language: string;
}

/** Advice to put the surplus into the reserve for a few months. */
const ReserveAdvice = ({
	advice,
	rank,
	currency,
	language,
}: Readonly<ReserveAdviceProps>) => {
	const t = getI18n(language, ADVICE_CARD_I18N);
	const money = (amount: number) => formatMoney(amount, currency, language);

	return (
		<AdviceRow
			rank={rank}
			title={`${t("reserve_title")} ${money(advice.monthlyAmount)} ${t("per_month")}`}
			gain={advice.gain}
			language={language}
		>
			<Text variant="hint" className="tabular-nums">
				{t("reserve_months")} {formatMonths(advice.months, language, "long")} ·{" "}
				{t("total")} {money(advice.monthlyAmount * advice.months)}
			</Text>
			<AdviceChange
				indicator="Reserve"
				change={advice.reserve}
				format={(value) => formatMonths(value, language, "short")}
				language={language}
			/>
		</AdviceRow>
	);
};

export default ReserveAdvice;
