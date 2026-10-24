import { create } from "zustand";
import { PAGE_SIZES } from "./constants/page-sizes";

interface SnapshotsState {
	page: number;
	pageSize: number;
	setPage: (page: number) => void;
	setPageSize: (pageSize: number) => void;
}

/** Page of the snapshot history that is shown and its size. */
export const useSnapshotsStore = create<SnapshotsState>()((set) => ({
	page: 1,
	pageSize: PAGE_SIZES[0],
	setPage: (page) => set({ page }),
	setPageSize: (pageSize) => set({ pageSize, page: 1 }),
}));
