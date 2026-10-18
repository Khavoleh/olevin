import { Logo } from "@shared/components";
import { PAGE_URLS } from "@shared/constants";
import { getLanguageUrl } from "@shared/helpers";
import LanguageSwitcher from "@widgets/language-switcher";
import LegalLinks from "@widgets/legal-links";
import Link from "next/link";
import type { ReactNode } from "react";

interface PublicShellProps {
	language: string;
	children: ReactNode;
}

/** Frame of the pages that open without signing in: header with the language switcher, the content and the legal links. */
const PublicShell = ({ language, children }: Readonly<PublicShellProps>) => {
	return (
		<div className="flex min-h-dvh flex-col">
			<header className="mx-auto flex w-full max-w-content items-center justify-between gap-3 px-4 py-3 md:px-8">
				<Link
					href={getLanguageUrl(PAGE_URLS.HOME, language)}
					className="flex min-h-11 items-center gap-2 text-primary"
				>
					<Logo size={26} />
					<span className="font-heading font-semibold text-fg text-xl">
						Olevin
					</span>
				</Link>
				<LanguageSwitcher language={language} />
			</header>

			<main className="mx-auto flex w-full max-w-content flex-1 flex-col px-4 py-6 md:px-8 md:py-8">
				{children}
			</main>

			<footer className="mx-auto w-full max-w-content px-4 pt-4 pb-6 md:px-8">
				<LegalLinks language={language} />
			</footer>
		</div>
	);
};

export default PublicShell;
