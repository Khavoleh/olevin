import { getI18n } from "@shared/helpers";
import { PRIVACY_I18N } from "./privacy-i18n";

interface PrivacyProps {
	language: string;
}

/** Privacy policy page. */
const Privacy = ({ language }: Readonly<PrivacyProps>) => {
	const t = getI18n(language, PRIVACY_I18N);

	return <h1>{t("title")}</h1>;
};

export default Privacy;
