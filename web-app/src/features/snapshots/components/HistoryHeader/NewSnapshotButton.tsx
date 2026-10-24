import { PlusIcon } from "@phosphor-icons/react/dist/ssr";
import { Link } from "@shared/components";
import { PAGE_URLS } from "@shared/constants";
import { formatDayMonth, getI18n, getLanguageUrl } from "@shared/helpers";
import { getSnapshotHistory } from "../../get-snapshot-history";
import { SNAPSHOTS_I18N } from "../../snapshots-i18n";
import ClosedSnapshotButton from "./ClosedSnapshotButton";

interface NewSnapshotButtonProps {
	language: string;
}

/** Link to a new snapshot; until a month opens it is disabled with a tooltip of when the next one opens. */
const NewSnapshotButton = async ({
	language,
}: Readonly<NewSnapshotButtonProps>) => {
	const t = getI18n(language, SNAPSHOTS_I18N);
	const result = await getSnapshotHistory(language);

	if ("error" in result) return null;

	const { newSnapshotOpensOn } = result.history;
	const label = (
		<>
			<PlusIcon size={18} />
			{t("new_snapshot")}
		</>
	);

	if (newSnapshotOpensOn === null) {
		return (
			<Link
				variant="button"
				href={getLanguageUrl(PAGE_URLS.NEW_SNAPSHOT, language)}
			>
				{label}
			</Link>
		);
	}

	return (
		<ClosedSnapshotButton
			hint={`${t("not_yet")} ${formatDayMonth(newSnapshotOpensOn, language)}.`}
		>
			{label}
		</ClosedSnapshotButton>
	);
};

export default NewSnapshotButton;
