import Snapshots from "@features/snapshots";

interface SnapshotsPageProps {
	params: Promise<{ lang: string }>;
}

/** Route of the snapshot history page. */
const SnapshotsPage = async ({ params }: Readonly<SnapshotsPageProps>) => {
	const { lang } = await params;

	return <Snapshots language={lang} />;
};

export default SnapshotsPage;
