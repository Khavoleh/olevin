"use client";

import { ConfirmDialog, showErrorToast, Text } from "@shared/components";
import { getI18n } from "@shared/helpers";
import TimezoneField, { formatTimezone } from "@widgets/timezone-field";
import { useState, useTransition } from "react";
import { saveTimezone } from "../../actions/save-timezone";
import { PROFILE_I18N } from "../../profile-i18n";
import SettingsButton from "../SettingsList/SettingsButton";

interface TimezoneEditorProps {
	language: string;
	timezone: string;
}

/** Row of the time zone that opens the choice of a new one with the note that the saved snapshots stay as they are. */
const TimezoneEditor = ({
	language,
	timezone,
}: Readonly<TimezoneEditorProps>) => {
	const t = getI18n(language, PROFILE_I18N);
	const [isOpen, setIsOpen] = useState(false);
	const [selected, setSelected] = useState(timezone);
	const [isPending, startTransition] = useTransition();

	const open = () => {
		setSelected(timezone);
		setIsOpen(true);
	};

	const confirm = () =>
		startTransition(async () => {
			const error = await saveTimezone(language, selected);

			if (error) showErrorToast(error);

			setIsOpen(false);
		});

	return (
		<>
			<SettingsButton
				label={t("time_zone")}
				value={formatTimezone(timezone)}
				onPress={open}
			/>
			<ConfirmDialog
				title={t("time_zone_title")}
				confirmLabel={t("time_zone_confirm")}
				cancelLabel={t("cancel")}
				isOpen={isOpen}
				isPending={isPending}
				isConfirmDisabled={selected === timezone}
				onOpenChange={setIsOpen}
				onConfirm={confirm}
			>
				<TimezoneField
					language={language}
					value={selected}
					onChange={setSelected}
					labelVariant="field"
					isDisabled={isPending}
				/>
				<Text variant="description">{t("time_zone_warning")}</Text>
			</ConfirmDialog>
		</>
	);
};

export default TimezoneEditor;
