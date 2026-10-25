import { getProfileSettings } from "../../loaders/get-profile-settings";
import TimezoneEditor from "./TimezoneEditor";

interface TimezoneSettingProps {
	language: string;
}

/** Saved time zone of the user with its editor. */
const TimezoneSetting = async ({
	language,
}: Readonly<TimezoneSettingProps>) => {
	const result = await getProfileSettings(language);

	if ("error" in result) return null;

	return (
		<TimezoneEditor language={language} timezone={result.settings.timezone} />
	);
};

export default TimezoneSetting;
