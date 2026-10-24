"use client";

import { showErrorToast } from "@shared/components";
import { useEffect } from "react";
import { getSnapshotHistory } from "../actions/get-snapshot-history";
import { useSnapshotsStore } from "../snapshots-store";

interface LoadSnapshotsProps {
	language: string;
}

/** Puts the snapshot history into the store. */
const LoadSnapshots = ({ language }: Readonly<LoadSnapshotsProps>) => {
	const setHistory = useSnapshotsStore((state) => state.setHistory);

	useEffect(() => {
		void getSnapshotHistory(language).then((result) => {
			if ("error" in result) {
				showErrorToast(result.error);
			} else {
				setHistory(result.history);
			}
		});
	}, [language, setHistory]);

	return null;
};

export default LoadSnapshots;
