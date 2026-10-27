import { getI18n } from "@shared/helpers";
import { Suspense } from "react";
import { PROFILE_I18N } from "../profile-i18n";
import PushSetting from "./PushSetting/PushSetting";
import SettingsGroup from "./SettingsList/SettingsGroup";
import SettingsRow from "./SettingsList/SettingsRow";

interface ReminderSettingsProps {
	language: string;
}

/** Push reminder on the snapshot day with the browsers that get it. */
const ReminderSettings = ({ language }: Readonly<ReminderSettingsProps>) => {
	const t = getI18n(language, PROFILE_I18N);

	return (
		<SettingsGroup title={t("reminders")}>
			<Suspense
				fallback={
					<SettingsRow label={t("push")} hint={t("push_hint")}>
						<span aria-hidden className="h-7 w-11.5 rounded-full bg-skeleton" />
					</SettingsRow>
				}
			>
				<PushSetting language={language} />
			</Suspense>
		</SettingsGroup>
	);
};

export default ReminderSettings;
