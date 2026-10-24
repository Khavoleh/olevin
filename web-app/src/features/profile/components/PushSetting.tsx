import { getI18n } from "@shared/helpers";
import { getProfileSettings } from "../loaders/get-profile-settings";
import { PROFILE_I18N } from "../profile-i18n";
import SettingsSwitch from "./SettingsList/SettingsSwitch";

interface PushSettingProps {
	language: string;
}

/** Push reminder on the snapshot day, as saved for the user. */
const PushSetting = async ({ language }: Readonly<PushSettingProps>) => {
	const t = getI18n(language, PROFILE_I18N);
	const result = await getProfileSettings(language);

	if ("error" in result) return null;

	return (
		<SettingsSwitch
			label={t("push")}
			hint={t("push_hint")}
			defaultSelected={result.settings.pushNotifications}
		/>
	);
};

export default PushSetting;
