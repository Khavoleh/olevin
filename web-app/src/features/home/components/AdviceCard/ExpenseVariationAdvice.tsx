import { Text } from "@shared/components";
import { getI18n } from "@shared/helpers";
import type { AdviceResponseExpenseVariationAdviceResponse } from "../../api/home.schemas";
import { formatMoney } from "../../helpers/format-money/format-money";
import { formatPercent } from "../../helpers/format-percent/format-percent";
import { ADVICE_CARD_I18N } from "../../i18n/advice-card-i18n";
import AdviceChange from "./AdviceChange";
import AdviceRow from "./AdviceRow";

interface ExpenseVariationAdviceProps {
	advice: AdviceResponseExpenseVariationAdviceResponse;
	rank: number;
	currency: string;
	language: string;
}

/** Advice to keep the monthly expenses closer to their average. */
const ExpenseVariationAdvice = ({
	advice,
	rank,
	currency,
	language,
}: Readonly<ExpenseVariationAdviceProps>) => {
	const t = getI18n(language, ADVICE_CARD_I18N);

	return (
		<AdviceRow
			rank={rank}
			title={t("variation_title")}
			gain={advice.gain}
			language={language}
		>
			<Text variant="hint" className="tabular-nums">
				{t("variation_detail")} {formatMoney(advice.amount, currency, language)}
			</Text>
			<AdviceChange
				indicator="ExpenseVariation"
				change={advice.expenseVariation}
				format={(value) => formatPercent(value, language)}
				language={language}
			/>
		</AdviceRow>
	);
};

export default ExpenseVariationAdvice;
