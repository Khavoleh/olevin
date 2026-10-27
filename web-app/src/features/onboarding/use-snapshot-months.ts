import { useMemo, useSyncExternalStore } from "react";
import { getSnapshotMonths } from "./helpers/get-snapshot-months/get-snapshot-months";
import { useOnboardingStore } from "./onboarding-store";

const subscribeToClient = () => () => {};
const getIsClient = () => true;
const getIsServer = () => false;

/** Month of the first snapshot, then the 2 estimated months. Empty on the server. */
export const useSnapshotMonths = (language: string) => {
	const timezone = useOnboardingStore((state) => state.timezone);
	const snapshotDay = useOnboardingStore((state) => state.snapshotDay);
	const isClient = useSyncExternalStore(
		subscribeToClient,
		getIsClient,
		getIsServer,
	);

	return useMemo(() => {
		if (!isClient) return [];

		const format = new Intl.DateTimeFormat(language, {
			month: "long",
			timeZone: "UTC",
		});
		const timeZone =
			timezone || Intl.DateTimeFormat().resolvedOptions().timeZone;

		return getSnapshotMonths(new Date(), timeZone, snapshotDay).map(
			({ year, month }) => {
				const name = format.format(new Date(Date.UTC(year, month - 1, 1)));

				return {
					name,
					title: `${name.charAt(0).toUpperCase()}${name.slice(1)} ${year}`,
				};
			},
		);
	}, [isClient, language, timezone, snapshotDay]);
};
