import { Text } from "@shared/components";
import { getI18n } from "@shared/helpers";
import type { AdviceResponseSavingsRateAdviceResponse } from "../../api/home.schemas";
import { formatMoney } from "../../helpers/format-money/format-money";
import { formatPercent } from "../../helpers/format-percent/format-percent";
import { ADVICE_CARD_I18N } from "../../i18n/advice-card-i18n";
import AdviceChange from "./AdviceChange";
import AdviceRow from "./AdviceRow";

interface SavingsRateAdviceProps {
	advice: AdviceResponseSavingsRateAdviceResponse;
	rank: number;
	currency: string;
	language: string;
}

/** Advice to cut flexible expenses, with the cut of each category. */
const SavingsRateAdvice = ({
	advice,
	rank,
	currency,
	language,
}: Readonly<SavingsRateAdviceProps>) => {
	const t = getI18n(language, ADVICE_CARD_I18N);
	const money = (amount: number) => formatMoney(amount, currency, language);
	const titleKey = advice.isPartial
		? "savings_rate_partial_title"
		: "savings_rate_title";

	return (
		<AdviceRow
			rank={rank}
			title={`${t(titleKey)} ${money(advice.amount)} ${t("per_month")}`}
			gain={advice.gain}
			language={language}
		>
			<AdviceChange
				indicator="SavingsRate"
				change={advice.savingsRate}
				format={(value) => formatPercent(value, language)}
				language={language}
			/>
			<Text variant="hint" className="tabular-nums">
				{advice.cuts
					.map(
						(cut) => `${t(`category_${cut.category}`)} −${money(cut.amount)}`,
					)
					.join(" · ")}
			</Text>
		</AdviceRow>
	);
};

export default SavingsRateAdvice;
