import { useMemo } from "react";
import type { GetSnapshotsResponse } from "./api/history.schemas";
import { getHistoryRows } from "./helpers/get-history-rows/get-history-rows";
import { paginate } from "./helpers/paginate/paginate";
import { useHistoryStore } from "./history-store";

/** Rows of the history page that is shown. */
export const useHistoryPage = (history: GetSnapshotsResponse) => {
	const page = useHistoryStore((state) => state.page);
	const pageSize = useHistoryStore((state) => state.pageSize);

	return useMemo(
		() => paginate(getHistoryRows(history), page, pageSize),
		[history, page, pageSize],
	);
};
