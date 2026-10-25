import { getI18n } from "@shared/helpers";
import { Suspense } from "react";
import { PROFILE_I18N } from "../profile-i18n";
import BaseCurrencySetting from "./BaseCurrencySetting/BaseCurrencySetting";
import LanguageSetting from "./LanguageSetting";
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
			<Suspense
				fallback={
					<SettingsRow label={t("language")}>
						<span aria-hidden className="h-10.5 w-23 rounded-md bg-skeleton" />
					</SettingsRow>
				}
			>
				<LanguageSetting language={language} />
			</Suspense>
			<Suspense
				fallback={
					<SettingsRow
						label={t("base_currency")}
						hint={t("base_currency_hint")}
					>
						<span aria-hidden className="h-5 w-14 rounded-md bg-skeleton" />
					</SettingsRow>
				}
			>
				<BaseCurrencySetting language={language} />
			</Suspense>
			<SettingsButton
				label={t("snapshot_day")}
				value={t("snapshot_day_value")}
			/>
			<SettingsButton label={t("time_zone")} value={t("time_zone_value")} />
		</SettingsGroup>
	);
};

export default GeneralSettings;
