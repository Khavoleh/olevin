import SnapshotForm from "@features/snapshots";
import { Suspense } from "react";

interface EditSnapshotPageProps {
	params: Promise<{ lang: string; month: string }>;
}

/** Route of the page that changes the snapshot of one month. */
const EditSnapshotPage = ({ params }: Readonly<EditSnapshotPageProps>) => {
	return (
		<Suspense>
			<EditSnapshot params={params} />
		</Suspense>
	);
};

/** The month is not known at build time, so reading it waits behind `<Suspense>`. */
const EditSnapshot = async ({ params }: Readonly<EditSnapshotPageProps>) => {
	const { lang, month } = await params;

	return <SnapshotForm language={lang} month={month} />;
};

export default EditSnapshotPage;
