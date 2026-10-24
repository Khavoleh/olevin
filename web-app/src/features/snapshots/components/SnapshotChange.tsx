import { getI18n } from "@shared/helpers";
import { formatChange } from "../helpers/format-change/format-change";
import { SNAPSHOTS_I18N } from "../snapshots-i18n";

interface SnapshotChangeProps {
	change: number | null;
	language: string;
}

const SIGN_CLASSES: Record<number, string> = {
	1: "text-positive",
	[-1]: "text-negative",
	0: "text-fg-muted",
};

/** Change of the index since the previous snapshot with its sign, or "first" for the first snapshot. */
const SnapshotChange = ({
	change,
	language,
}: Readonly<SnapshotChangeProps>) => {
	const t = getI18n(language, SNAPSHOTS_I18N);

	if (change === null)
		return <span className="text-fg-muted text-sm">{t("first")}</span>;

	return (
		<span
			className={`whitespace-nowrap font-semibold text-sm tabular-nums ${SIGN_CLASSES[Math.sign(change)]}`}
		>
			{formatChange(change, language)}
		</span>
	);
};

export default SnapshotChange;
