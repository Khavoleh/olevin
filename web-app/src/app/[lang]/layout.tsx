import { getI18n, getLanguageStaticParams } from "@shared/helpers";
import { APP_I18N } from "@shared/i18n";
import LanguageSwitcher from "@widgets/language-switcher";
import type { Metadata } from "next";
import type { ReactNode } from "react";
import Providers from "../providers";
import "../globals.css";

export const generateStaticParams = getLanguageStaticParams;

interface LanguageParams {
	params: Promise<{ lang: string }>;
}

/** Translated title and description of every page. */
export async function generateMetadata({
	params,
}: LanguageParams): Promise<Metadata> {
	const t = getI18n((await params).lang, APP_I18N);

	return {
		title: t("title"),
		description: t("description"),
	};
}

interface RootLayoutProps extends LanguageParams {
	children: ReactNode;
}

/** Document shell of every page: header with the language switcher and the centered content. */
const RootLayout = async ({ children, params }: Readonly<RootLayoutProps>) => {
	const { lang } = await params;
	const t = getI18n(lang, APP_I18N);

	return (
		<html lang={lang}>
			<body className="flex min-h-dvh flex-col">
				<Providers>
					<header className="flex items-center justify-between px-4 py-3">
						<span className="font-semibold text-lg">{t("title")}</span>
						<LanguageSwitcher language={lang} />
					</header>
					<main className="flex flex-1 items-center justify-center px-4 py-8">
						{children}
					</main>
				</Providers>
			</body>
		</html>
	);
};

export default RootLayout;
