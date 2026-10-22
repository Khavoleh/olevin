import { ArrowRightIcon } from "@phosphor-icons/react";
import { Text } from "@shared/components";
import { getI18n } from "@shared/helpers";
import type { AdviceResponseSavingsRegularityAdviceResponse } from "../../api/home.schemas";
import { ADVICE_CARD_I18N } from "../../i18n/advice-card-i18n";
import AdviceRow from "./AdviceRow";

interface SavingsRegularityAdviceProps {
	advice: AdviceResponseSavingsRegularityAdviceResponse;
	rank: number;
	language: string;
}

/** Advice to save something in one more month. */
const SavingsRegularityAdvice = ({
	advice,
	rank,
	language,
}: Readonly<SavingsRegularityAdviceProps>) => {
	const t = getI18n(language, ADVICE_CARD_I18N);

	return (
		<AdviceRow
			rank={rank}
			title={t("regularity_title")}
			gain={advice.gain}
			language={language}
		>
			<Text
				variant="hint"
				className="flex flex-wrap items-center gap-x-1 tabular-nums"
			>
				<span>
					{t("saving_months")} {advice.savingMonths}
				</span>
				<ArrowRightIcon size={14} aria-hidden />
				<span className="sr-only">{t("to")}</span>
				<span>
					{advice.savingMonths + 1} {t("of")} {advice.months}
				</span>
			</Text>
		</AdviceRow>
	);
};

export default SavingsRegularityAdvice;
