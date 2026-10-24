import { useMemo } from "react";
import type { GetSnapshotsResponse } from "./api/snapshots.schemas";
import { getHistoryRows } from "./helpers/get-history-rows/get-history-rows";
import { paginate } from "./helpers/paginate/paginate";
import { useSnapshotsStore } from "./snapshots-store";

/** Rows of the history page that is shown. */
export const useHistoryPage = (history: GetSnapshotsResponse) => {
	const page = useSnapshotsStore((state) => state.page);
	const pageSize = useSnapshotsStore((state) => state.pageSize);

	return useMemo(
		() => paginate(getHistoryRows(history), page, pageSize),
		[history, page, pageSize],
	);
};
