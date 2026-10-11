import { getI18n } from "@shared/helpers";
import { TERMS_I18N } from "./terms-i18n";

interface TermsProps {
	language: string;
}

/** Terms of use page. */
const Terms = ({ language }: Readonly<TermsProps>) => {
	const t = getI18n(language, TERMS_I18N);

	return <h1>{t("title")}</h1>;
};

export default Terms;
