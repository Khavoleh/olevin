import { getNewSnapshotMonths } from "./get-new-snapshot-months";

/** Day the next month opens for a snapshot, or `null` while a month is open; an API error counts as open. */
export const getNewSnapshotOpensOn = async (
	language: string,
): Promise<string | null> => {
	const result = await getNewSnapshotMonths(language);

	return "error" in result ? null : result.openMonths.opensOn;
};
