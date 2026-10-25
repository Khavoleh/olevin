"use client";

import { ConfirmDialog, showErrorToast, Text } from "@shared/components";
import { getI18n } from "@shared/helpers";
import { useState, useTransition } from "react";
import { deleteAccount } from "../actions/delete-account";
import { PROFILE_I18N } from "../profile-i18n";
import SettingsButton from "./SettingsList/SettingsButton";

interface DeleteAccountButtonProps {
	language: string;
}

/** Row that asks to confirm the deletion of the account, which deletes it and all its data for good. */
const DeleteAccountButton = ({
	language,
}: Readonly<DeleteAccountButtonProps>) => {
	const t = getI18n(language, PROFILE_I18N);
	const [isOpen, setIsOpen] = useState(false);
	const [isPending, startTransition] = useTransition();

	const confirm = () =>
		startTransition(async () => {
			const error = await deleteAccount(language);

			if (error) {
				showErrorToast(error);
				setIsOpen(false);
			}
		});

	return (
		<>
			<SettingsButton
				label={t("delete_account")}
				hint={t("delete_account_hint")}
				tone="danger"
				onPress={() => setIsOpen(true)}
			/>
			<ConfirmDialog
				title={t("delete_account_title")}
				confirmLabel={t("delete_account_confirm")}
				cancelLabel={t("cancel")}
				variant="danger"
				isOpen={isOpen}
				isPending={isPending}
				onOpenChange={setIsOpen}
				onConfirm={confirm}
			>
				<Text variant="description">{t("delete_account_warning")}</Text>
			</ConfirmDialog>
		</>
	);
};

export default DeleteAccountButton;
