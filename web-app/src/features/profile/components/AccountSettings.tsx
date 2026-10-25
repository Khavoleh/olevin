import { PAGE_URLS } from "@shared/constants";
import { getI18n, getLanguageUrl } from "@shared/helpers";
import { PROFILE_I18N } from "../profile-i18n";
import AccountCenterLink from "./AccountCenterLink";
import DeleteAccountButton from "./DeleteAccountButton";
import SettingsGroup from "./SettingsList/SettingsGroup";
import SignOutButton from "./SignOutButton";

interface AccountSettingsProps {
	language: string;
}

/** The account in the Logto Account Center, signing out and deleting the account. */
const AccountSettings = ({ language }: Readonly<AccountSettingsProps>) => {
	const t = getI18n(language, PROFILE_I18N);

	return (
		<SettingsGroup title={t("account")}>
			<AccountCenterLink
				label={t("account_center")}
				hint={t("account_center_hint")}
				href={getLanguageUrl(PAGE_URLS.ACCOUNT_CENTER, language)}
			/>
			<SignOutButton language={language} />
			<DeleteAccountButton language={language} />
		</SettingsGroup>
	);
};

export default AccountSettings;
