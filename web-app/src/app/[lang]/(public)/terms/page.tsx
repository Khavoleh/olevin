import Terms from "@features/terms";

interface TermsPageProps {
	params: Promise<{ lang: string }>;
}

/** Route of the terms of use page. */
const TermsPage = async ({ params }: Readonly<TermsPageProps>) => {
	const { lang } = await params;

	return <Terms language={lang} />;
};

export default TermsPage;
