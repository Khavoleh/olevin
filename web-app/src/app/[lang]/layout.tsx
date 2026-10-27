import { ServiceWorkerRegistration, ToastRegion } from "@shared/components";
import { getI18n, getLanguageStaticParams } from "@shared/helpers";
import { APP_I18N } from "@shared/i18n";
import type { Metadata, Viewport } from "next";
import type { ReactNode } from "react";
import "../globals.css";

export const generateStaticParams = getLanguageStaticParams;

/** `cover` lets the layout use the safe areas of a phone, so the bars sit above the home indicator. */
export const viewport: Viewport = {
	width: "device-width",
	initialScale: 1,
	viewportFit: "cover",
	themeColor: "#5C4D9E",
};

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
		appleWebApp: { capable: true, title: t("title") },
	};
}

interface RootLayoutProps extends LanguageParams {
	children: ReactNode;
}

/** Document shell of every page; the frame around the content comes from the layout of the page group. */
const RootLayout = async ({ children, params }: Readonly<RootLayoutProps>) => {
	const { lang } = await params;

	return (
		<html lang={lang}>
			<body>
				{children}
				<ToastRegion language={lang} />
				<ServiceWorkerRegistration />
			</body>
		</html>
	);
};

export default RootLayout;
