import { Link, Text } from "@shared/components";
import { CONTACT_EMAIL } from "@shared/constants";
import { getI18n } from "@shared/helpers";
import LegalDocument, {
	LegalList,
	LegalSection,
} from "@widgets/legal-document";
import { PRIVACY_I18N } from "./privacy-i18n";

interface PrivacyProps {
	language: string;
}

const UPDATED_AT = "2026-10-27";

/** Privacy policy page: what data the app collects, why, where it is kept and how the user controls it. */
const Privacy = ({ language }: Readonly<PrivacyProps>) => {
	const t = getI18n(language, PRIVACY_I18N);

	return (
		<LegalDocument
			language={language}
			title={t("title")}
			updatedAt={UPDATED_AT}
		>
			<Text>{t("intro")}</Text>

			<LegalSection title={t("collect_title")}>
				<Text>{t("collect_intro")}</Text>
				<LegalList
					items={[
						t("collect_account"),
						t("collect_google"),
						t("collect_settings"),
						t("collect_snapshots"),
						t("collect_research"),
						t("collect_push"),
						t("collect_technical"),
					]}
				/>
				<Text>{t("collect_no_bank")}</Text>
			</LegalSection>

			<LegalSection title={t("use_title")}>
				<Text>{t("use_intro")}</Text>
				<LegalList
					items={[
						t("use_service"),
						t("use_reminders"),
						t("use_emails"),
						t("use_security"),
						t("use_statistics"),
					]}
				/>
				<Text>{t("use_no_sale")}</Text>
			</LegalSection>

			<LegalSection title={t("bases_title")}>
				<Text>{t("bases_intro")}</Text>
				<LegalList
					items={[
						t("bases_contract"),
						t("bases_consent"),
						t("bases_interests"),
					]}
				/>
			</LegalSection>

			<LegalSection title={t("research_title")}>
				<Text>{t("research_consent")}</Text>
				<Text>{t("research_withdraw")}</Text>
				<Text>{t("research_aggregate")}</Text>
			</LegalSection>

			<LegalSection title={t("processors_title")}>
				<Text>{t("processors_intro")}</Text>
				<LegalList
					items={[
						t("processors_ovh"),
						t("processors_google"),
						t("processors_resend"),
						t("processors_push"),
						t("processors_rates"),
					]}
				/>
				<Text>{t("processors_transfers")}</Text>
			</LegalSection>

			<LegalSection title={t("storage_title")}>
				<Text>{t("storage_intro")}</Text>
				<LegalList
					items={[
						t("storage_session"),
						t("storage_language"),
						t("storage_onboarded"),
						t("storage_draft"),
						t("storage_service_worker"),
					]}
				/>
			</LegalSection>

			<LegalSection title={t("retention_title")}>
				<LegalList
					items={[
						t("retention_account"),
						t("retention_deletion"),
						t("retention_logs"),
						t("retention_aggregate"),
					]}
				/>
			</LegalSection>

			<LegalSection title={t("rights_title")}>
				<Text>{t("rights_intro")}</Text>
				<LegalList
					items={[
						t("rights_edit"),
						t("rights_export"),
						t("rights_consent"),
						t("rights_push"),
						t("rights_delete"),
					]}
				/>
				<Text>{t("rights_email")}</Text>
			</LegalSection>

			<LegalSection title={t("children_title")}>
				<Text>{t("children_text")}</Text>
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

export default Privacy;
