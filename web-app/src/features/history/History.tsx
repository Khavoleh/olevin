import { Suspense } from "react";
import HistoryContent from "./components/HistoryContent";
import HistoryHeader from "./components/HistoryHeader/HistoryHeader";

interface HistoryProps {
	language: string;
}

/** History page: every snapshot and estimate, as cards on a phone and a table on wider screens. */
const History = ({ language }: Readonly<HistoryProps>) => {
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
					<HistoryContent language={language} />
				</Suspense>
			</div>
		</>
	);
};

export default History;
