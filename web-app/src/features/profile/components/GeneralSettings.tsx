import { getI18n } from "@shared/helpers";
import LanguageSwitcher from "@widgets/language-switcher";
import { PROFILE_I18N } from "../profile-i18n";
import SettingsButton from "./SettingsList/SettingsButton";
import SettingsGroup from "./SettingsList/SettingsGroup";
import SettingsRow from "./SettingsList/SettingsRow";

interface GeneralSettingsProps {
	language: string;
}

/** Language, base currency, snapshot day and time zone of the account. The language is switched only here. */
const GeneralSettings = ({ language }: Readonly<GeneralSettingsProps>) => {
	const t = getI18n(language, PROFILE_I18N);

	return (
		<SettingsGroup title={t("general")}>
			<SettingsRow label={t("language")}>
				<LanguageSwitcher language={language} />
			</SettingsRow>
			<SettingsButton
				label={t("base_currency")}
				hint={t("base_currency_hint")}
				value={t("base_currency_value")}
			/>
			<SettingsButton
				label={t("snapshot_day")}
				value={t("snapshot_day_value")}
			/>
			<SettingsButton label={t("time_zone")} value={t("time_zone_value")} />
		</SettingsGroup>
	);
};

export default GeneralSettings;
