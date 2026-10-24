import { PlusIcon } from "@phosphor-icons/react/dist/ssr";
import { DisabledButton, Link } from "@shared/components";
import { PAGE_URLS } from "@shared/constants";
import { formatDayMonth, getI18n, getLanguageUrl } from "@shared/helpers";
import { getSnapshotHistory } from "../../get-snapshot-history";
import { HISTORY_I18N } from "../../history-i18n";

interface NewSnapshotButtonProps {
	language: string;
}

/** Link to a new snapshot; until a month opens it is disabled with a tooltip of when the next one opens. */
const NewSnapshotButton = async ({
	language,
}: Readonly<NewSnapshotButtonProps>) => {
	const t = getI18n(language, HISTORY_I18N);
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
		<DisabledButton
			hint={`${t("not_yet")} ${formatDayMonth(newSnapshotOpensOn, language)}.`}
			className="inline-flex min-h-11 items-center justify-center gap-2 rounded-md bg-primary px-4.5 font-semibold text-on-primary opacity-45"
		>
			{label}
		</DisabledButton>
	);
};

export default NewSnapshotButton;
