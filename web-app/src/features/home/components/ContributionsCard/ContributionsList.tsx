import { ErrorToast, Text } from "@shared/components";
import { getI18n } from "@shared/helpers";
import { describeContributions } from "../../helpers/describe-contributions/describe-contributions";
import { CONTRIBUTIONS_CARD_I18N } from "../../i18n/contributions-card-i18n";
import { getLatestContributions } from "../../loaders/get-latest-contributions";
import ContributionRow from "./ContributionRow";

interface ContributionsListProps {
	language: string;
}

/** Factors that changed the index since the previous snapshot. */
const ContributionsList = async ({
	language,
}: Readonly<ContributionsListProps>) => {
	const t = getI18n(language, CONTRIBUTIONS_CARD_I18N);
	const result = await getLatestContributions(language);

	if ("error" in result) return <ErrorToast message={result.error} />;

	if (!result.contributions)
		return (
			<div className="rounded-lg bg-surface-sunken p-3">
				<Text variant="hint">{t("empty")}</Text>
			</div>
		);

	const lines = describeContributions(result.contributions, t, language);
	const scale = Math.max(...lines.map((line) => Math.abs(line.points)));

	return (
		<ul className="grid gap-2">
			{lines.map((line) => (
				<li key={line.key}>
					<ContributionRow
						label={line.label}
						detail={line.detail}
						points={line.points}
						scale={scale}
						language={language}
					/>
				</li>
			))}
		</ul>
	);
};

export default ContributionsList;
