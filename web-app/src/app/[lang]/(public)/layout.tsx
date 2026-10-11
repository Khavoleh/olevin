import PublicShell from "@widgets/public-shell";
import type { ReactNode } from "react";

interface PublicLayoutProps {
	children: ReactNode;
	params: Promise<{ lang: string }>;
}

/** Frame of the pages that open without signing in. */
const PublicLayout = async ({
	children,
	params,
}: Readonly<PublicLayoutProps>) => {
	const { lang } = await params;

	return <PublicShell language={lang}>{children}</PublicShell>;
};

export default PublicLayout;
