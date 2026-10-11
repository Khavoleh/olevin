import { getI18n } from "@shared/helpers";
import LanguageSwitcher from "@widgets/language-switcher";
import { PROFILE_I18N } from "./profile-i18n";

interface ProfileProps {
	language: string;
}

/** Profile page. The language is switched only here, so the switcher is its first control. */
const Profile = ({ language }: Readonly<ProfileProps>) => {
	const t = getI18n(language, PROFILE_I18N);

	return (
		<>
			<h1>{t("title")}</h1>
			<div>
				<LanguageSwitcher language={language} />
			</div>
		</>
	);
};

export default Profile;
