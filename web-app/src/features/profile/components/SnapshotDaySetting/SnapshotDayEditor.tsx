"use client";

import {
	ConfirmDialog,
	NumberField,
	showErrorToast,
	Text,
} from "@shared/components";
import { getI18n } from "@shared/helpers";
import { useState, useTransition } from "react";
import { saveSnapshotDay } from "../../actions/save-snapshot-day";
import { PROFILE_I18N } from "../../profile-i18n";
import SettingsButton from "../SettingsList/SettingsButton";

const LAST_DAY = 31;

interface SnapshotDayEditorProps {
	language: string;
	snapshotDay: number;
}

/** Row of the snapshot day that opens the input of a new one with the note that the saved snapshots stay as they are. */
const SnapshotDayEditor = ({
	language,
	snapshotDay,
}: Readonly<SnapshotDayEditorProps>) => {
	const t = getI18n(language, PROFILE_I18N);
	const [isOpen, setIsOpen] = useState(false);
	const [selected, setSelected] = useState(snapshotDay);
	const [isPending, startTransition] = useTransition();

	const open = () => {
		setSelected(snapshotDay);
		setIsOpen(true);
	};

	const confirm = () => {
		if (selected === snapshotDay) return setIsOpen(false);

		startTransition(async () => {
			const error = await saveSnapshotDay(language, selected);

			if (error) showErrorToast(error);

			setIsOpen(false);
		});
	};

	return (
		<>
			<SettingsButton
				label={t("snapshot_day")}
				value={
					snapshotDay === LAST_DAY
						? t("snapshot_day_last")
						: t("snapshot_day_value").replace("{day}", String(snapshotDay))
				}
				onPress={open}
			/>
			<ConfirmDialog
				title={t("snapshot_day_title")}
				confirmLabel={t("snapshot_day_confirm")}
				cancelLabel={t("cancel")}
				isOpen={isOpen}
				isPending={isPending}
				isConfirmDisabled={Number.isNaN(selected)}
				onOpenChange={setIsOpen}
				onConfirm={confirm}
			>
				<NumberField
					label={t("snapshot_day_new")}
					description={t("snapshot_day_hint")}
					errorMessage={t("snapshot_day_error")}
					labelVariant="field"
					inputClassName="w-32"
					value={selected}
					onChange={setSelected}
					minValue={1}
					maxValue={LAST_DAY}
					formatOptions={{ maximumFractionDigits: 0, useGrouping: false }}
					isInvalid={Number.isNaN(selected)}
					isDisabled={isPending}
					isRequired
					validationBehavior="aria"
					autoFocus
				/>
				<Text variant="description">{t("snapshot_day_warning")}</Text>
			</ConfirmDialog>
		</>
	);
};

export default SnapshotDayEditor;
