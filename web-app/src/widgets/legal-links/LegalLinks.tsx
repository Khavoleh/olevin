import { PAGE_URLS, REPOSITORY_URL } from "@shared/constants";
import { getI18n, getLanguageUrl } from "@shared/helpers";
import Link from "next/link";
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
  const linkClasses = "hover:text-fg hover:underline";

  return (
    <nav
      aria-label={t("legal")}
      className={`flex flex-wrap gap-x-3 gap-y-1 text-fg-muted text-sm ${className}`}
    >
      <Link
        href={getLanguageUrl(PAGE_URLS.PRIVACY, language)}
        className={linkClasses}
      >
        {t("privacy")}
      </Link>
      <Link
        href={getLanguageUrl(PAGE_URLS.TERMS, language)}
        className={linkClasses}
      >
        {t("terms")}
      </Link>
      <a
        href={REPOSITORY_URL}
        target="_blank"
        rel="noopener noreferrer"
        className={linkClasses}
      >
        {t("source_code")}
      </a>
    </nav>
  );
};

export default LegalLinks;
