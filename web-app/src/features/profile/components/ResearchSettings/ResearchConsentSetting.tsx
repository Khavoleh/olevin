import { getI18n } from "@shared/helpers";
import { getProfileSettings } from "../../loaders/get-profile-settings";
import { PROFILE_I18N } from "../../profile-i18n";
import SettingsGroup from "../SettingsList/SettingsGroup";
import ResearchConsentEditor from "./ResearchConsentEditor";

interface ResearchConsentSettingProps {
	language: string;
}

/** Research group with the consent and its withdrawal; nothing once the user has not consented. */
const ResearchConsentSetting = async ({
	language,
}: Readonly<ResearchConsentSettingProps>) => {
	const t = getI18n(language, PROFILE_I18N);
	const result = await getProfileSettings(language);

	if ("error" in result || !result.settings.researchConsent) return null;

	return (
		<SettingsGroup title={t("research")}>
			<ResearchConsentEditor language={language} />
		</SettingsGroup>
	);
};

export default ResearchConsentSetting;
