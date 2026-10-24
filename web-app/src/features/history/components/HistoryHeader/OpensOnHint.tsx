import { CalendarBlankIcon } from "@phosphor-icons/react/dist/ssr";
import { Text } from "@shared/components";
import { formatDayMonth, getI18n } from "@shared/helpers";
import { getSnapshotHistory } from "../../get-snapshot-history";
import { HISTORY_I18N } from "../../history-i18n";

interface OpensOnHintProps {
	language: string;
}

/** Date the next snapshot opens on, shown on a phone. */
const OpensOnHint = async ({ language }: Readonly<OpensOnHintProps>) => {
	const t = getI18n(language, HISTORY_I18N);
	const result = await getSnapshotHistory(language);

	if ("error" in result || !result.history.newSnapshotOpensOn) return null;

	return (
		<Text variant="hint" className="flex items-center gap-1.5 md:hidden">
			<CalendarBlankIcon size={16} />
			{`${t("opens_on")} ${formatDayMonth(result.history.newSnapshotOpensOn, language)}`}
		</Text>
	);
};

export default OpensOnHint;
