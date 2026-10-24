import { Suspense } from "react";
import History from "./components/History";
import HistoryHeader from "./components/HistoryHeader/HistoryHeader";

interface SnapshotsProps {
	language: string;
}

/** History page: every snapshot and estimate, as cards on a phone and a table on wider screens. */
const Snapshots = ({ language }: Readonly<SnapshotsProps>) => {
	return (
		<>
			<HistoryHeader language={language} />

			<div className="@container grid gap-3">
				<Suspense
					fallback={
						<>
							<div aria-hidden className="grid @3xl:hidden gap-2">
								<div className="h-32 rounded-lg bg-skeleton" />
								<div className="h-32 rounded-lg bg-skeleton" />
								<div className="h-32 rounded-lg bg-skeleton" />
							</div>
							<div
								aria-hidden
								className="@3xl:block hidden h-72 rounded-xl bg-skeleton"
							/>
						</>
					}
				>
					<History language={language} />
				</Suspense>
			</div>
		</>
	);
};

export default Snapshots;
