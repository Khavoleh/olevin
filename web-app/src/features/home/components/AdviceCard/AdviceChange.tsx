import { ArrowRightIcon } from "@phosphor-icons/react/dist/ssr";
import { Text } from "@shared/components";
import { getI18n } from "@shared/helpers";
import type { IndicatorChangeResponse } from "../../api/home.schemas";
import { ADVICE_CARD_I18N } from "../../i18n/advice-card-i18n";

interface AdviceChangeProps {
	indicator: string;
	change: IndicatorChangeResponse;
	format: (value: number) => string;
	language: string;
}

/** Indicator before and after the advice, as "Savings rate: 10% → 15%". */
const AdviceChange = ({
	indicator,
	change,
	format,
	language,
}: Readonly<AdviceChangeProps>) => {
	const t = getI18n(language, ADVICE_CARD_I18N);

	return (
		<Text
			variant="hint"
			className="flex flex-wrap items-center gap-x-1 tabular-nums"
		>
			<span>
				{t(`indicator_${indicator}`)}: {format(change.from)}
			</span>
			<ArrowRightIcon size={14} aria-hidden />
			<span className="sr-only">{t("to")}</span>
			<span>{format(change.to)}</span>
		</Text>
	);
};

export default AdviceChange;
