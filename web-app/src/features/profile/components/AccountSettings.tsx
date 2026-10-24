import { getI18n } from "@shared/helpers";
import { PROFILE_I18N } from "../profile-i18n";
import { signOut } from "../sign-out";
import SettingsButton from "./SettingsList/SettingsButton";
import SettingsGroup from "./SettingsList/SettingsGroup";

const SIGN_OUT_FORM_ID = "sign-out-form";

interface AccountSettingsProps {
	language: string;
}

/** Signing out and deleting the account. The sign-out form stays outside the list, so its button is a row like the others. */
const AccountSettings = ({ language }: Readonly<AccountSettingsProps>) => {
	const t = getI18n(language, PROFILE_I18N);

	return (
		<>
			<form id={SIGN_OUT_FORM_ID} action={signOut} hidden />
			<SettingsGroup title={t("account")}>
				<SettingsButton
					type="submit"
					form={SIGN_OUT_FORM_ID}
					label={t("sign_out")}
				/>
				<SettingsButton
					label={t("delete_account")}
					hint={t("delete_account_hint")}
					tone="danger"
				/>
			</SettingsGroup>
		</>
	);
};

export default AccountSettings;
