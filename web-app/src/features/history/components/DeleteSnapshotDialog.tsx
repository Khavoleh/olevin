"use client";

import { ConfirmDialog, showErrorToast, Text } from "@shared/components";
import { getI18n, getPluralKey } from "@shared/helpers";
import { useTransition } from "react";
import { removeSnapshot } from "../delete-snapshot";
import { HISTORY_I18N } from "../history-i18n";
import type { DeletableSnapshot } from "../interfaces/deletable-snapshot";

interface DeleteSnapshotDialogProps {
	snapshot: DeletableSnapshot;
	monthName: string;
	language: string;
	isOpen: boolean;
	onOpenChange: (isOpen: boolean) => void;
}

/** Confirmation of the deletion that says how many later snapshots are recalculated. */
const DeleteSnapshotDialog = ({
	snapshot: { id, laterSnapshots },
	monthName,
	language,
	isOpen,
	onOpenChange,
}: Readonly<DeleteSnapshotDialogProps>) => {
	const t = getI18n(language, HISTORY_I18N);
	const [isPending, startTransition] = useTransition();

	const confirm = () =>
		startTransition(async () => {
			const error = await removeSnapshot(id, language);

			if (error) showErrorToast(error);

			onOpenChange(false);
		});

	return (
		<ConfirmDialog
			title={t("delete_title").replace("{month}", monthName)}
			confirmLabel={t("delete")}
			cancelLabel={t("cancel")}
			variant="danger"
			isOpen={isOpen}
			isPending={isPending}
			onOpenChange={onOpenChange}
			onConfirm={confirm}
		>
			<Text variant="description">{t("delete_description")}</Text>
			{laterSnapshots > 0 && (
				<Text variant="description">
					{t(getPluralKey("delete_later", laterSnapshots, language)).replace(
						"{count}",
						String(laterSnapshots),
					)}
				</Text>
			)}
		</ConfirmDialog>
	);
};

export default DeleteSnapshotDialog;
