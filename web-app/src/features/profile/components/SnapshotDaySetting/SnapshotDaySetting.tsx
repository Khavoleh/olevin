import { getProfileSettings } from "../../loaders/get-profile-settings";
import SnapshotDayEditor from "./SnapshotDayEditor";

interface SnapshotDaySettingProps {
	language: string;
}

/** Saved snapshot day of the user with its editor. */
const SnapshotDaySetting = async ({
	language,
}: Readonly<SnapshotDaySettingProps>) => {
	const result = await getProfileSettings(language);

	if ("error" in result) return null;

	return (
		<SnapshotDayEditor
			language={language}
			snapshotDay={result.settings.snapshotDay}
		/>
	);
};

export default SnapshotDaySetting;
