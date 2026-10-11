import Privacy from "@features/privacy";

interface PrivacyPageProps {
	params: Promise<{ lang: string }>;
}

/** Route of the privacy policy page. */
const PrivacyPage = async ({ params }: Readonly<PrivacyPageProps>) => {
	const { lang } = await params;

	return <Privacy language={lang} />;
};

export default PrivacyPage;
