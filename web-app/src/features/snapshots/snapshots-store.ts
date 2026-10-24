import { create } from "zustand";
import type { GetSnapshotsResponse } from "./api/snapshots.schemas";
import { PAGE_SIZES } from "./constants/page-sizes";

interface SnapshotsState {
	history: GetSnapshotsResponse | null;
	page: number;
	pageSize: number;
	setHistory: (history: GetSnapshotsResponse) => void;
	setPage: (page: number) => void;
	setPageSize: (pageSize: number) => void;
}

/** Snapshot history of the user and the page of it that is shown; the history is `null` until it loads. */
export const useSnapshotsStore = create<SnapshotsState>()((set) => ({
	history: null,
	page: 1,
	pageSize: PAGE_SIZES[0],
	setHistory: (history) => set({ history }),
	setPage: (page) => set({ page }),
	setPageSize: (pageSize) => set({ pageSize, page: 1 }),
}));
