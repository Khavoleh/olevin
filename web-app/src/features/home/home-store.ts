import { create } from "zustand";
import type { GetIndexResponse } from "./api/home.schemas";

interface HomeState {
	index: GetIndexResponse | null;
	setIndex: (index: GetIndexResponse) => void;
}

/** Latest index of the user for the overview; `null` until it loads. */
export const useHomeStore = create<HomeState>()((set) => ({
	index: null,
	setIndex: (index) => set({ index }),
}));
