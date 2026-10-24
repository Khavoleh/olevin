"use client";

import { ConfirmDialog, Text } from "@shared/components";
import { getI18n, getPluralKey } from "@shared/helpers";
import { SNAPSHOT_FORM_I18N } from "../snapshot-form-i18n";

interface RecalculationDialogProps {
	language: string;
	laterSnapshots: number;
	isOpen: boolean;
	isPending: boolean;
	onOpenChange: (isOpen: boolean) => void;
	onConfirm: () => void;
}

/** Confirmation of a change that says how many later snapshots are recalculated. */
const RecalculationDialog = ({
	language,
	laterSnapshots,
	...props
}: Readonly<RecalculationDialogProps>) => {
	const t = getI18n(language, SNAPSHOT_FORM_I18N);

	return (
		<ConfirmDialog
			{...props}
			title={t("recalculate_title")}
			confirmLabel={t("recalculate_confirm")}
			cancelLabel={t("cancel")}
		>
			<Text variant="description">
				{t(getPluralKey("recalculate", laterSnapshots, language)).replace(
					"{count}",
					String(laterSnapshots),
				)}
			</Text>
		</ConfirmDialog>
	);
};

export default RecalculationDialog;
