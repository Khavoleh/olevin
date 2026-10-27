import { Heading, Text } from "@shared/components";
import { getI18n } from "@shared/helpers";
import type { ReactNode } from "react";
import { LEGAL_DOCUMENT_I18N } from "../legal-document-i18n";

interface LegalDocumentProps {
	language: string;
	title: string;
	updatedAt: string;
	children: ReactNode;
}

/** Page of a legal document: the title, the date of the last update (`2026-10-27`) and the sections. */
const LegalDocument = ({
	language,
	title,
	updatedAt,
	children,
}: Readonly<LegalDocumentProps>) => {
	const t = getI18n(language, LEGAL_DOCUMENT_I18N);
	const updatedOn = new Intl.DateTimeFormat(language, {
		dateStyle: "long",
		timeZone: "UTC",
	}).format(new Date(`${updatedAt}T00:00:00Z`));

	return (
		<article className="flex flex-col gap-6">
			<header className="flex flex-col gap-2">
				<Heading level={1}>{title}</Heading>
				<Text variant="hint">
					{t("updated")} <time dateTime={updatedAt}>{updatedOn}</time>
				</Text>
			</header>

			{children}
		</article>
	);
};

export default LegalDocument;
