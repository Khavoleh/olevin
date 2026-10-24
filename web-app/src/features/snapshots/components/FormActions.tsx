"use client";

import { Button, Link } from "@shared/components";
import { PAGE_URLS } from "@shared/constants";
import { getI18n, getLanguageUrl } from "@shared/helpers";
import { SNAPSHOT_FORM_I18N } from "../snapshot-form-i18n";

interface FormActionsProps {
	language: string;
	submitLabel: string;
	isPending: boolean;
}

/** Button that saves the form and the link back to the history. */
const FormActions = ({
	language,
	submitLabel,
	isPending,
}: Readonly<FormActionsProps>) => {
	const t = getI18n(language, SNAPSHOT_FORM_I18N);

	return (
		<div className="flex flex-col gap-2 md:flex-row md:items-center">
			<Button type="submit" isPending={isPending} className="md:min-w-65">
				{submitLabel}
			</Button>
			<Link
				variant="action"
				href={getLanguageUrl(PAGE_URLS.SNAPSHOTS, language)}
			>
				{t("cancel")}
			</Link>
		</div>
	);
};

export default FormActions;
