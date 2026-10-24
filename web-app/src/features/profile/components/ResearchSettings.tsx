import { Button } from "@shared/components";
import { getI18n } from "@shared/helpers";
import { PROFILE_I18N } from "../profile-i18n";
import SettingsGroup from "./SettingsList/SettingsGroup";
import SettingsRow from "./SettingsList/SettingsRow";

interface ResearchSettingsProps {
	language: string;
}

/** Consent to take part in the research and the way to revoke it. */
const ResearchSettings = ({ language }: Readonly<ResearchSettingsProps>) => {
	const t = getI18n(language, PROFILE_I18N);

	return (
		<SettingsGroup title={t("research")}>
			<SettingsRow
				label={t("research_consent")}
				hint={t("research_consent_hint")}
			>
				<Button variant="secondary" className="text-sm">
					{t("revoke")}
				</Button>
			</SettingsRow>
		</SettingsGroup>
	);
};

export default ResearchSettings;
