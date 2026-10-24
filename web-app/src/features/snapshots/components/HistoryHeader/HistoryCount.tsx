import { getI18n } from "@shared/helpers";
import { getSnapshotHistory } from "../../get-snapshot-history";
import { getPluralKey } from "../../helpers/get-plural-key/get-plural-key";
import { SNAPSHOTS_I18N } from "../../snapshots-i18n";

interface HistoryCountProps {
	language: string;
}

/** Number of snapshots and estimates. */
const HistoryCount = async ({ language }: Readonly<HistoryCountProps>) => {
	const t = getI18n(language, SNAPSHOTS_I18N);
	const result = await getSnapshotHistory(language);
	const count = (key: string, value: number) =>
		`${value} ${t(getPluralKey(key, value, language))}`;

	if ("error" in result) return null;

	const { snapshots, estimates } = result.history;

	return [
		count("snapshots", snapshots.length),
		estimates.length > 0 && count("estimates", estimates.length),
	]
		.filter(Boolean)
		.join(" · ");
};

export default HistoryCount;
