import { ClearOnboardingDraft } from "@features/onboarding";
import { getNewSnapshotOpensOn } from "@features/snapshots";
import AppShell from "@widgets/app-shell";
import type { ReactNode } from "react";

interface AppLayoutProps {
	children: ReactNode;
	params: Promise<{ lang: string }>;
}

/** Frame of the signed-in pages: tab bar, rail or sidebar depending on the screen width. */
const AppLayout = async ({ children, params }: Readonly<AppLayoutProps>) => {
	const { lang } = await params;

	return (
		<AppShell language={lang} newSnapshotOpensOn={getNewSnapshotOpensOn(lang)}>
			<ClearOnboardingDraft />
			{children}
		</AppShell>
	);
};

export default AppLayout;
