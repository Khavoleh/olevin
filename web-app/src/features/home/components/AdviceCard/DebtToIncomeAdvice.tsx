import { getI18n } from "@shared/helpers";
import type { AdviceResponseDebtToIncomeAdviceResponse } from "../../api/home.schemas";
import { formatMoney } from "../../helpers/format-money/format-money";
import { formatPercent } from "../../helpers/format-percent/format-percent";
import { ADVICE_CARD_I18N } from "../../i18n/advice-card-i18n";
import AdviceChange from "./AdviceChange";
import AdviceRow from "./AdviceRow";

interface DebtToIncomeAdviceProps {
	advice: AdviceResponseDebtToIncomeAdviceResponse;
	rank: number;
	currency: string;
	language: string;
}

/** Advice to lower the debt payments, which also raises the savings rate. */
const DebtToIncomeAdvice = ({
	advice,
	rank,
	currency,
	language,
}: Readonly<DebtToIncomeAdviceProps>) => {
	const t = getI18n(language, ADVICE_CARD_I18N);
	const percent = (value: number) => formatPercent(value, language);

	return (
		<AdviceRow
			rank={rank}
			title={`${t("debt_title")} ${formatMoney(advice.amount, currency, language)} ${t("per_month")}`}
			gain={advice.gain}
			language={language}
		>
			<AdviceChange
				indicator="DebtToIncome"
				change={advice.debtToIncome}
				format={percent}
				language={language}
			/>
			<AdviceChange
				indicator="SavingsRate"
				change={advice.savingsRate}
				format={percent}
				language={language}
			/>
		</AdviceRow>
	);
};

export default DebtToIncomeAdvice;
