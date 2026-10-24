import { ErrorToast } from "@shared/components";
import { getI18n } from "@shared/helpers";
import LanguageSwitcher from "@widgets/language-switcher";
import { saveLanguage } from "../actions/save-language";
import { getProfileSettings } from "../loaders/get-profile-settings";
import { PROFILE_I18N } from "../profile-i18n";
import SettingsRow from "./SettingsList/SettingsRow";

interface LanguageSettingProps {
	language: string;
}

/** Saved interface language with a warning when the page is shown in another one. */
const LanguageSetting = async ({
	language,
}: Readonly<LanguageSettingProps>) => {
	const t = getI18n(language, PROFILE_I18N);
	const result = await getProfileSettings(language);

	if ("error" in result) return <ErrorToast message={result.error} />;

	const savedLanguage = result.settings.locale.toLowerCase();
	const isMismatch = savedLanguage !== language;
	const savedLanguageName = new Intl.DisplayNames(language, {
		type: "language",
	}).of(savedLanguage);

	return (
		<SettingsRow
			label={t("language")}
			hint={
				isMismatch
					? t("language_mismatch").replace(
							"{language}",
							savedLanguageName ?? savedLanguage,
						)
					: undefined
			}
			hintTone="warning"
		>
			<LanguageSwitcher
				language={language}
				selectedLanguage={savedLanguage}
				saveLanguage={saveLanguage.bind(null, language)}
			/>
		</SettingsRow>
	);
};

export default LanguageSetting;
