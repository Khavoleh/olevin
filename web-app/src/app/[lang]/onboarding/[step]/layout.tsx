import {
	FlowShell,
	getOnboardingStep,
	ONBOARDING_STEPS,
} from "@features/onboarding";
import { notFound } from "next/navigation";
import type { ReactNode } from "react";

/** Prerenders every step. */
export const generateStaticParams = () =>
	ONBOARDING_STEPS.map((step) => ({ step: String(step) }));

interface OnboardingLayoutProps {
	children: ReactNode;
	params: Promise<{ lang: string; step: string }>;
}

/** Layout of an onboarding step. */
const OnboardingLayout = async ({
	children,
	params,
}: Readonly<OnboardingLayoutProps>) => {
	const { lang, step: segment } = await params;
	const step = getOnboardingStep(segment);

	if (!step) notFound();

	return (
		<FlowShell language={lang} step={step}>
			{children}
		</FlowShell>
	);
};

export default OnboardingLayout;
