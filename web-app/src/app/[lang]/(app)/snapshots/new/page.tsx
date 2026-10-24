import SnapshotForm from "@features/snapshots";

interface NewSnapshotPageProps {
	params: Promise<{ lang: string }>;
}

/** Route of the new snapshot page. */
const NewSnapshotPage = async ({ params }: Readonly<NewSnapshotPageProps>) => {
	const { lang } = await params;

	return <SnapshotForm language={lang} />;
};

export default NewSnapshotPage;
