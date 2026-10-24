import { Heading } from "@shared/components";
import { getI18n } from "@shared/helpers";
import { Suspense } from "react";
import EditMonth from "./components/EditMonth";
import NewSnapshot from "./components/NewSnapshot";
import { SNAPSHOT_FORM_I18N } from "./snapshot-form-i18n";

interface SnapshotFormProps {
	language: string;
	/** Month of the snapshot to change (`2026-09`); without it the form creates a new snapshot. */
	month?: string;
}

/** Form of a monthly snapshot: creates a new one or changes a saved snapshot or estimate. */
const SnapshotForm = ({ language, month }: Readonly<SnapshotFormProps>) => {
	const t = getI18n(language, SNAPSHOT_FORM_I18N);

	return (
		<div className="grid w-full max-w-flow-wide gap-4">
			<Heading level={1}>{t(month ? "title_edit" : "title_new")}</Heading>
			<Suspense
				fallback={
					<div aria-hidden className="grid gap-4">
						<div className="h-6 w-3/4 rounded-sm bg-skeleton" />
						<div className="h-40 rounded-lg bg-skeleton" />
						<div className="h-96 rounded-lg bg-skeleton" />
						<div className="h-40 rounded-lg bg-skeleton" />
					</div>
				}
			>
				{month ? (
					<EditMonth language={language} month={month} />
				) : (
					<NewSnapshot language={language} />
				)}
			</Suspense>
		</div>
	);
};

export default SnapshotForm;
