import History from "@features/history";

interface SnapshotsPageProps {
	params: Promise<{ lang: string }>;
}

/** Route of the snapshot history page. */
const SnapshotsPage = async ({ params }: Readonly<SnapshotsPageProps>) => {
	const { lang } = await params;

	return <History language={lang} />;
};

export default SnapshotsPage;
