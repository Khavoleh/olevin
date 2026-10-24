import HistoryHeader from "./components/HistoryHeader/HistoryHeader";
import HistoryList from "./components/HistoryList/HistoryList";
import HistoryPagination from "./components/HistoryPagination";
import HistoryTable from "./components/HistoryTable/HistoryTable";
import LoadSnapshots from "./components/LoadSnapshots";

interface SnapshotsProps {
	language: string;
}

/** History page: every snapshot and estimate, as cards on a phone and a table on wider screens. */
const Snapshots = ({ language }: Readonly<SnapshotsProps>) => {
	return (
		<>
			<LoadSnapshots language={language} />
			<HistoryHeader language={language} />

			<div className="@container grid gap-3">
				<HistoryList language={language} className="@3xl:hidden" />
				<HistoryTable language={language} className="@3xl:block hidden" />
				<HistoryPagination language={language} />
			</div>
		</>
	);
};

export default Snapshots;
