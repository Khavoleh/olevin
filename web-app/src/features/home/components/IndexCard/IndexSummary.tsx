import { ErrorToast, ZoneBadge } from "@shared/components";
import { formatIndex, getI18n } from "@shared/helpers";
import { INDEX_CARD_I18N } from "../../i18n/index-card-i18n";
import { getLatestIndex } from "../../loaders/get-latest-index";
import IndexChange from "./IndexChange";
import IndexGauge from "./IndexGauge/IndexGauge";

interface IndexSummaryProps {
	language: string;
}

/** Gauge of the latest index with its zone and change. */
const IndexSummary = async ({ language }: Readonly<IndexSummaryProps>) => {
	const t = getI18n(language, INDEX_CARD_I18N);
	const result = await getLatestIndex(language);

	if ("error" in result) return <ErrorToast message={result.error} />;

	const { index } = result;

	return (
		<>
			<IndexGauge
				value={index.value}
				zone={index.zone}
				valueText={formatIndex(index.value, language)}
				outOfText={t("out_of")}
				label={`${t("title")} ${formatIndex(index.value, language)} ${t("out_of")}`}
			/>

			<div className="flex flex-wrap items-center justify-center gap-2.5">
				<ZoneBadge zone={index.zone} language={language} />
				{index.change ? (
					<IndexChange change={index.change} language={language} />
				) : null}
			</div>
		</>
	);
};

export default IndexSummary;
