import { ErrorToast, Text } from "@shared/components";
import { getI18n } from "@shared/helpers";
import { ADVICE_CARD_I18N } from "../../i18n/advice-card-i18n";
import { getLatestAdvices } from "../../loaders/get-latest-advices";
import Advice from "./Advice";

interface AdviceListProps {
	language: string;
}

/** Advices ranked by how much they raise the index, or why there are none. */
const AdviceList = async ({ language }: Readonly<AdviceListProps>) => {
	const t = getI18n(language, ADVICE_CARD_I18N);
	const result = await getLatestAdvices(language);

	if ("error" in result) return <ErrorToast message={result.error} />;

	const { advices } = result;

	if (advices.status !== "Shown")
		return <Text variant="description">{t(`status_${advices.status}`)}</Text>;

	return (
		<ol className="grid gap-3">
			{advices.advices.map((advice, index) => (
				<Advice
					key={advice.kind}
					advice={advice}
					rank={index + 1}
					currency={advices.currency}
					language={language}
				/>
			))}
		</ol>
	);
};

export default AdviceList;
