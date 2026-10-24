import { CodeIcon } from "@phosphor-icons/react/dist/ssr";
import { PAGE_URLS, REPOSITORY_URL } from "@shared/constants";
import { getI18n, getLanguageUrl } from "@shared/helpers";
import { PROFILE_I18N } from "../profile-i18n";
import SettingsGroup from "./SettingsList/SettingsGroup";
import SettingsLink from "./SettingsList/SettingsLink";

const LICENSE = "PolyForm Noncommercial 1.0.0";

interface DocumentSettingsProps {
	language: string;
}

/** Links to the privacy policy, the terms and the source code with its license. */
const DocumentSettings = ({ language }: Readonly<DocumentSettingsProps>) => {
	const t = getI18n(language, PROFILE_I18N);

	return (
		<SettingsGroup title={t("documents")}>
			<SettingsLink
				label={t("privacy")}
				href={getLanguageUrl(PAGE_URLS.PRIVACY, language)}
			/>
			<SettingsLink
				label={t("terms")}
				href={getLanguageUrl(PAGE_URLS.TERMS, language)}
			/>
			<SettingsLink
				label={t("source_code")}
				hint={`${REPOSITORY_URL.replace("https://", "")} · ${LICENSE}`}
				href={REPOSITORY_URL}
				isExternal
				icon={<CodeIcon size={18} />}
			/>
		</SettingsGroup>
	);
};

export default DocumentSettings;
