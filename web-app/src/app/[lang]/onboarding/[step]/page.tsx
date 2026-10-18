import Onboarding, { getOnboardingStep } from "@features/onboarding";
import { notFound } from "next/navigation";

interface OnboardingPageProps {
	params: Promise<{ lang: string; step: string }>;
}

/** Page of an onboarding step. */
const OnboardingPage = async ({ params }: Readonly<OnboardingPageProps>) => {
	const { lang, step: segment } = await params;
	const step = getOnboardingStep(segment);

	if (!step) notFound();

	return <Onboarding language={lang} step={step} />;
};

export default OnboardingPage;
