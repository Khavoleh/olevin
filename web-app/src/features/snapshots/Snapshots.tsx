import { getI18n } from "@shared/helpers";
import { SNAPSHOTS_I18N } from "./snapshots-i18n";

interface SnapshotsProps {
	language: string;
}

/** History page: every monthly snapshot, with change and delete. */
const Snapshots = ({ language }: Readonly<SnapshotsProps>) => {
	const t = getI18n(language, SNAPSHOTS_I18N);

	return <h1>{t("title")}</h1>;
};

export default Snapshots;
