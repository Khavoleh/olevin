import { Heading } from "@shared/components";
import { getI18n } from "@shared/helpers";
import { SNAPSHOT_FORM_I18N } from "./snapshot-form-i18n";

interface SnapshotFormProps {
	language: string;
	/** Month of the snapshot to change (`2026-09`); without it the form creates a new snapshot. */
	month?: string;
}

/** Form of a monthly snapshot: creates a new one or changes an existing one. */
const SnapshotForm = ({ language, month }: Readonly<SnapshotFormProps>) => {
	const t = getI18n(language, SNAPSHOT_FORM_I18N);

	return <Heading level={1}>{t(month ? "title_edit" : "title_new")}</Heading>;
};

export default SnapshotForm;
