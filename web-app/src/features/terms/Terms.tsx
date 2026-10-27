import { Link, Text } from "@shared/components";
import { CONTACT_EMAIL, PAGE_URLS, REPOSITORY_URL } from "@shared/constants";
import { getI18n, getLanguageUrl } from "@shared/helpers";
import LegalDocument, {
	LegalList,
	LegalSection,
} from "@widgets/legal-document";
import { TERMS_I18N } from "./terms-i18n";

interface TermsProps {
	language: string;
}

const UPDATED_AT = "2026-10-27";

/** Terms of use page: the rules of using the app, its limits and the governing law. */
const Terms = ({ language }: Readonly<TermsProps>) => {
	const t = getI18n(language, TERMS_I18N);

	return (
		<LegalDocument
			language={language}
			title={t("title")}
			updatedAt={UPDATED_AT}
		>
			<LegalSection title={t("acceptance_title")}>
				<Text>
					{t("acceptance_text")}{" "}
					<Link href={getLanguageUrl(PAGE_URLS.PRIVACY, language)}>
						{t("acceptance_privacy")}
					</Link>
					. {t("acceptance_refusal")}
				</Text>
			</LegalSection>

			<LegalSection title={t("service_title")}>
				<Text>{t("service_text")}</Text>
				<Text>{t("service_free")}</Text>
			</LegalSection>

			<LegalSection title={t("account_title")}>
				<LegalList
					items={[
						t("account_age"),
						t("account_security"),
						t("account_accuracy"),
					]}
				/>
			</LegalSection>

			<LegalSection title={t("advice_title")}>
				<Text>{t("advice_text")}</Text>
				<Text>{t("advice_rates")}</Text>
				<Text>{t("advice_responsibility")}</Text>
			</LegalSection>

			<LegalSection title={t("use_title")}>
				<Text>{t("use_intro")}</Text>
				<LegalList
					items={[
						t("use_unlawful"),
						t("use_access"),
						t("use_disrupt"),
						t("use_automation"),
					]}
				/>
			</LegalSection>

			<LegalSection title={t("data_title")}>
				<Text>{t("data_text")}</Text>
			</LegalSection>

			<LegalSection title={t("ip_title")}>
				<Text>{t("ip_brand")}</Text>
				<Text>{t("ip_code")}</Text>
				<Link
					href={REPOSITORY_URL}
					target="_blank"
					rel="noopener noreferrer"
					className="self-start"
				>
					{t("ip_code_link")}
				</Link>
			</LegalSection>

			<LegalSection title={t("availability_title")}>
				<LegalList
					items={[
						t("availability_service"),
						t("availability_suspension"),
						t("availability_leave"),
					]}
				/>
			</LegalSection>

			<LegalSection title={t("liability_title")}>
				<Text>{t("liability_as_is")}</Text>
				<Text>{t("liability_limit")}</Text>
				<Text>{t("liability_law")}</Text>
			</LegalSection>

			<LegalSection title={t("third_title")}>
				<Text>{t("third_text")}</Text>
			</LegalSection>

			<LegalSection title={t("law_title")}>
				<Text>{t("law_text")}</Text>
			</LegalSection>

			<LegalSection title={t("changes_title")}>
				<Text>{t("changes_text")}</Text>
			</LegalSection>

			<LegalSection title={t("contact_title")}>
				<Text>
					{t("contact_text")}{" "}
					<Link href={`mailto:${CONTACT_EMAIL}`} isDocument>
						{CONTACT_EMAIL}
					</Link>
					.
				</Text>
			</LegalSection>
		</LegalDocument>
	);
};

export default Terms;
