import { ErrorToast } from "@shared/components";
import { getSnapshotHistory } from "../get-snapshot-history";
import HistoryList from "./HistoryList/HistoryList";
import HistoryPagination from "./HistoryPagination";
import HistoryTable from "./HistoryTable/HistoryTable";

interface HistoryContentProps {
	language: string;
}

/** Shown page of the history as cards and as a table, with the pagination. */
const HistoryContent = async ({ language }: Readonly<HistoryContentProps>) => {
	const result = await getSnapshotHistory(language);

	if ("error" in result) return <ErrorToast message={result.error} />;

	return (
		<>
			<HistoryList
				history={result.history}
				language={language}
				className="@3xl:hidden"
			/>
			<HistoryTable
				history={result.history}
				language={language}
				className="@3xl:block hidden"
			/>
			<HistoryPagination history={result.history} language={language} />
		</>
	);
};

export default HistoryContent;
