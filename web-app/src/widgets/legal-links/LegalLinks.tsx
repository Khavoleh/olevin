import { Link } from "@shared/components";
import { PAGE_URLS, REPOSITORY_URL } from "@shared/constants";
import { getI18n, getLanguageUrl } from "@shared/helpers";
import { LEGAL_LINKS_I18N } from "./legal-links-i18n";

interface LegalLinksProps {
	language: string;
	className?: string;
}

/** Links to the privacy policy, the terms and the source code. */
const LegalLinks = ({
	language,
	className = "",
}: Readonly<LegalLinksProps>) => {
	const t = getI18n(language, LEGAL_LINKS_I18N);

	return (
		<nav
			aria-label={t("legal")}
			className={`flex flex-wrap gap-x-3 gap-y-1 text-xs ${className}`}
		>
			<Link variant="subtle" href={getLanguageUrl(PAGE_URLS.PRIVACY, language)}>
				{t("privacy")}
			</Link>
			<Link variant="subtle" href={getLanguageUrl(PAGE_URLS.TERMS, language)}>
				{t("terms")}
			</Link>
			<Link
				variant="subtle"
				href={REPOSITORY_URL}
				target="_blank"
				rel="noopener noreferrer"
			>
				{t("source_code")}
			</Link>
		</nav>
	);
};

export default LegalLinks;
