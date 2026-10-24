import { Button } from "@shared/components";
import { getI18n } from "@shared/helpers";
import { PROFILE_I18N } from "../profile-i18n";
import SettingsGroup from "./SettingsList/SettingsGroup";
import SettingsRow from "./SettingsList/SettingsRow";

interface DataSettingsProps {
	language: string;
}

/** Export of every snapshot as JSON or CSV. */
const DataSettings = ({ language }: Readonly<DataSettingsProps>) => {
	const t = getI18n(language, PROFILE_I18N);

	return (
		<SettingsGroup title={t("my_data")}>
			<SettingsRow label={t("download_snapshots")}>
				<Button variant="secondary" className="text-sm">
					JSON
				</Button>
				<Button variant="secondary" className="text-sm">
					CSV
				</Button>
			</SettingsRow>
		</SettingsGroup>
	);
};

export default DataSettings;
