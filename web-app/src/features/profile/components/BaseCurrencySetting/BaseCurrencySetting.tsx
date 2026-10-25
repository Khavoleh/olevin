import { getProfileSettings } from "../../loaders/get-profile-settings";
import BaseCurrencyEditor from "./BaseCurrencyEditor";

interface BaseCurrencySettingProps {
	language: string;
}

/** Saved base currency of the user with its editor. */
const BaseCurrencySetting = async ({
	language,
}: Readonly<BaseCurrencySettingProps>) => {
	const result = await getProfileSettings(language);

	if ("error" in result) return null;

	return (
		<BaseCurrencyEditor
			language={language}
			baseCurrency={result.settings.baseCurrency}
		/>
	);
};

export default BaseCurrencySetting;
