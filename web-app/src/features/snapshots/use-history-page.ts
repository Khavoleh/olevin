import { useMemo } from "react";
import { getHistoryRows } from "./helpers/get-history-rows/get-history-rows";
import { paginate } from "./helpers/paginate/paginate";
import { useSnapshotsStore } from "./snapshots-store";

/** Rows of the history page that is shown, or `null` until the history loads. */
export const useHistoryPage = () => {
	const history = useSnapshotsStore((state) => state.history);
	const page = useSnapshotsStore((state) => state.page);
	const pageSize = useSnapshotsStore((state) => state.pageSize);

	return useMemo(
		() => (history ? paginate(getHistoryRows(history), page, pageSize) : null),
		[history, page, pageSize],
	);
};
