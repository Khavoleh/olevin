import { getI18n } from "@shared/helpers";
import { PROFILE_I18N } from "../profile-i18n";
import SettingsGroup from "./SettingsList/SettingsGroup";
import SettingsSwitch from "./SettingsList/SettingsSwitch";

interface ReminderSettingsProps {
	language: string;
}

/** Push reminder on the snapshot day. */
const ReminderSettings = ({ language }: Readonly<ReminderSettingsProps>) => {
	const t = getI18n(language, PROFILE_I18N);

	return (
		<SettingsGroup title={t("reminders")}>
			<SettingsSwitch label={t("push")} hint={t("push_hint")} defaultSelected />
		</SettingsGroup>
	);
};

export default ReminderSettings;
