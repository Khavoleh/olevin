import { Link } from "@shared/components";
import { PAGE_URLS } from "@shared/constants";
import { getI18n, getLanguageUrl } from "@shared/helpers";
import { EXPORT_FORMATS } from "../helpers/get-export-format/get-export-format";
import { PROFILE_I18N } from "../profile-i18n";
import SettingsGroup from "./SettingsList/SettingsGroup";
import SettingsRow from "./SettingsList/SettingsRow";

interface DataSettingsProps {
	language: string;
}

/** Export of every snapshot as JSON or CSV. */
const DataSettings = ({ language }: Readonly<DataSettingsProps>) => {
	const t = getI18n(language, PROFILE_I18N);
	const exportUrl = getLanguageUrl(PAGE_URLS.EXPORT_SNAPSHOTS, language);

	return (
		<SettingsGroup title={t("my_data")}>
			<SettingsRow label={t("download_snapshots")}>
				{EXPORT_FORMATS.map((format) => (
					<Link
						key={format}
						variant="secondary"
						href={`${exportUrl}?format=${format}`}
						isDocument
						className="text-sm"
					>
						{format.toUpperCase()}
					</Link>
				))}
			</SettingsRow>
		</SettingsGroup>
	);
};

export default DataSettings;
