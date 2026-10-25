"use client";

import { ConfirmDialog, Text } from "@shared/components";
import { getI18n } from "@shared/helpers";
import { useState, useTransition } from "react";
import { signOut } from "../actions/sign-out";
import { PROFILE_I18N } from "../profile-i18n";
import SettingsButton from "./SettingsList/SettingsButton";

interface SignOutButtonProps {
	language: string;
}

/** Row that asks to confirm signing out. */
const SignOutButton = ({ language }: Readonly<SignOutButtonProps>) => {
	const t = getI18n(language, PROFILE_I18N);
	const [isOpen, setIsOpen] = useState(false);
	const [isPending, startTransition] = useTransition();

	const confirm = () => startTransition(signOut);

	return (
		<>
			<SettingsButton label={t("sign_out")} onPress={() => setIsOpen(true)} />
			<ConfirmDialog
				title={t("sign_out_title")}
				confirmLabel={t("sign_out_confirm")}
				cancelLabel={t("cancel")}
				isOpen={isOpen}
				isPending={isPending}
				onOpenChange={setIsOpen}
				onConfirm={confirm}
			>
				<Text variant="description">{t("sign_out_warning")}</Text>
			</ConfirmDialog>
		</>
	);
};

export default SignOutButton;
