import { formatDayMonth, getI18n } from "@shared/helpers";
import { HOME_I18N } from "../../i18n/home-i18n";
import { getLatestIndex } from "../../loaders/get-latest-index";

interface NextSnapshotDateProps {
	language: string;
}

/** Date of the next snapshot. */
const NextSnapshotDate = async ({
	language,
}: Readonly<NextSnapshotDateProps>) => {
	const t = getI18n(language, HOME_I18N);
	const result = await getLatestIndex(language);

	if ("error" in result) return null;

	return `${t("next_snapshot")} ${formatDayMonth(result.index.nextSnapshotDate, language)}`;
};

export default NextSnapshotDate;
