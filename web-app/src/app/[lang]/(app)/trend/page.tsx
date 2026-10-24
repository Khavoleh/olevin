import Trend from "@features/trend";

interface TrendPageProps {
	params: Promise<{ lang: string }>;
	searchParams: Promise<{ period?: string | string[] }>;
}

/** Route of the trend page. */
const TrendPage = async ({
	params,
	searchParams,
}: Readonly<TrendPageProps>) => {
	const { lang } = await params;

	return <Trend language={lang} searchParams={searchParams} />;
};

export default TrendPage;
