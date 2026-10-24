import { formatSnapshotMonth, getI18n } from "@shared/helpers";
import { HOME_I18N } from "../../i18n/home-i18n";
import { getLatestIndex } from "../../loaders/get-latest-index";

interface SnapshotMonthProps {
	language: string;
}

/** Month of the latest snapshot. */
const SnapshotMonth = async ({ language }: Readonly<SnapshotMonthProps>) => {
	const t = getI18n(language, HOME_I18N);
	const result = await getLatestIndex(language);

	if ("error" in result) return null;

	return `${t("snapshot_for")} ${formatSnapshotMonth(result.index.year, result.index.month, language)}`;
};

export default SnapshotMonth;
