import Home from "@features/home";

interface HomePageProps {
	params: Promise<{ lang: string }>;
}

/** Route of the home page. */
const HomePage = async ({ params }: Readonly<HomePageProps>) => {
	const { lang } = await params;

	return <Home language={lang} />;
};

export default HomePage;
