import { ErrorToast } from "@shared/components";
import { getI18n } from "@shared/helpers";
import { describeBuffer } from "../../helpers/describe-buffer/describe-buffer";
import { describeCashFlow } from "../../helpers/describe-cash-flow/describe-cash-flow";
import { COMPOSITION_CARD_I18N } from "../../i18n/composition-card-i18n";
import { getLatestComposition } from "../../loaders/get-latest-composition";
import SavingsRegularity from "./SavingsRegularity";
import SubsystemMeter from "./SubsystemMeter";

interface CompositionItemsProps {
	language: string;
}

/** Cash flow, financial buffer and savings regularity of the latest index. */
const CompositionItems = async ({
	language,
}: Readonly<CompositionItemsProps>) => {
	const t = getI18n(language, COMPOSITION_CARD_I18N);
	const result = await getLatestComposition(language);

	if ("error" in result) return <ErrorToast message={result.error} />;

	const { composition } = result;

	return (
		<>
			<SubsystemMeter
				title={t("cash_flow")}
				value={composition.cashFlow.value}
				term={composition.cashFlow.term}
				detail={describeCashFlow(composition.cashFlow, t, language)}
				language={language}
			/>
			<SubsystemMeter
				title={t("buffer")}
				value={composition.buffer.value}
				term={composition.buffer.term}
				detail={describeBuffer(composition.buffer, t, language)}
				language={language}
			/>
			<SavingsRegularity
				regularity={composition.savingsRegularity}
				language={language}
			/>
		</>
	);
};

export default CompositionItems;
