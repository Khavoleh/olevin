import Trend from "@features/trend";

interface TrendPageProps {
	params: Promise<{ lang: string }>;
}

/** Route of the trend page. */
const TrendPage = async ({ params }: Readonly<TrendPageProps>) => {
	const { lang } = await params;

	return <Trend language={lang} />;
};

export default TrendPage;
