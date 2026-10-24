import { Heading } from "@shared/components";
import { formatIndex, getI18n } from "@shared/helpers";
import { formatSinceMonth } from "../../helpers/format-since-month/format-since-month";
import { CONTRIBUTIONS_CARD_I18N } from "../../i18n/contributions-card-i18n";
import { getLatestIndex } from "../../loaders/get-latest-index";

interface ContributionsHeadingProps {
	language: string;
}

const DELTA_CLASSES: Record<number, string> = {
	1: "text-positive",
	[-1]: "text-negative",
	0: "text-fg-muted",
};

/** Title with the month of the previous snapshot and the index change since it. */
const ContributionsHeading = async ({
	language,
}: Readonly<ContributionsHeadingProps>) => {
	const t = getI18n(language, CONTRIBUTIONS_CARD_I18N);
	const result = await getLatestIndex(language);
	const index = "error" in result ? null : result.index;
	const change = index?.change ?? null;

	if (!index || !change) return <Heading level={2}>{t("title")}</Heading>;

	return (
		<>
			<Heading level={2}>
				{`${t("title")} ${t("since")} ${formatSinceMonth(change.year, change.month, language)}`}
			</Heading>
			<span
				className={`font-semibold text-sm tabular-nums ${DELTA_CLASSES[Math.sign(change.delta)]}`}
			>
				{formatIndex(index.value - change.delta, language)}{" "}
				<span aria-hidden>→</span>
				<span className="sr-only">{t("to")}</span>{" "}
				{formatIndex(index.value, language)}
			</span>
		</>
	);
};

export default ContributionsHeading;
