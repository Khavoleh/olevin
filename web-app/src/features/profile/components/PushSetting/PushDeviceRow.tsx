"use client";

import { TrashIcon } from "@phosphor-icons/react";
import { Button } from "@shared/components";
import { getI18n } from "@shared/helpers";
import { PROFILE_I18N } from "../../profile-i18n";
import type { PushDevice } from "../../push-device";
import SettingsRow from "../SettingsList/SettingsRow";

interface PushDeviceRowProps {
	language: string;
	device: PushDevice;
	isCurrent: boolean;
	isDisabled: boolean;
	onRemove: () => void;
}

/** Browser that gets the push, with the button that turns the push off on it. */
const PushDeviceRow = ({
	language,
	device,
	isCurrent,
	isDisabled,
	onRemove,
}: Readonly<PushDeviceRowProps>) => {
	const t = getI18n(language, PROFILE_I18N);
	const added = t("push_added").replace("{date}", device.added);

	return (
		<SettingsRow
			label={device.name}
			hint={isCurrent ? `${t("push_this_device")} · ${added}` : added}
		>
			<Button
				variant="icon-danger"
				aria-label={t("push_remove").replace("{device}", device.name)}
				isDisabled={isDisabled}
				onPress={onRemove}
			>
				<TrashIcon size={18} />
			</Button>
		</SettingsRow>
	);
};

export default PushDeviceRow;
