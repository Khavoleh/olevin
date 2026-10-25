"use client";

import {
	Button,
	ConfirmDialog,
	showErrorToast,
	Text,
} from "@shared/components";
import { getI18n } from "@shared/helpers";
import { useState, useTransition } from "react";
import { withdrawResearchConsent } from "../../actions/withdraw-research-consent";
import { PROFILE_I18N } from "../../profile-i18n";
import SettingsRow from "../SettingsList/SettingsRow";

interface ResearchConsentEditorProps {
	language: string;
}

/** Row of the research consent that asks to confirm its withdrawal, which deletes the CFPB answers for good. */
const ResearchConsentEditor = ({
	language,
}: Readonly<ResearchConsentEditorProps>) => {
	const t = getI18n(language, PROFILE_I18N);
	const [isOpen, setIsOpen] = useState(false);
	const [isPending, startTransition] = useTransition();

	const confirm = () =>
		startTransition(async () => {
			const error = await withdrawResearchConsent(language);

			if (error) showErrorToast(error);

			setIsOpen(false);
		});

	return (
		<>
			<SettingsRow
				label={t("research_consent")}
				hint={t("research_consent_hint")}
			>
				<Button
					variant="secondary"
					className="text-sm"
					onPress={() => setIsOpen(true)}
				>
					{t("revoke")}
				</Button>
			</SettingsRow>
			<ConfirmDialog
				title={t("research_revoke_title")}
				confirmLabel={t("research_revoke_confirm")}
				cancelLabel={t("cancel")}
				variant="danger"
				isOpen={isOpen}
				isPending={isPending}
				onOpenChange={setIsOpen}
				onConfirm={confirm}
			>
				<Text variant="description">{t("research_revoke_warning")}</Text>
			</ConfirmDialog>
		</>
	);
};

export default ResearchConsentEditor;
