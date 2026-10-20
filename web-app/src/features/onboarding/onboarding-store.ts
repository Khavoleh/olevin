import {
	createMoneyListItem,
	type MoneyDraft,
	type MoneyListItem,
} from "@widgets/money-field";
import { create } from "zustand";
import { createJSONStorage, persist } from "zustand/middleware";
import type { CurrencyCode, ExpensesRequest } from "./api/onboarding.schemas";

/** Rough month before the first snapshot. */
export interface EstimateDraft {
	expensesTotal: MoneyDraft;
	saved: MoneyDraft;
}

/** First snapshot with the estimates of the 2 months before it. */
export interface SnapshotDraft {
	income: MoneyListItem[];
	expenses: Record<keyof ExpensesRequest, MoneyDraft>;
	saved: MoneyDraft;
	reserves: MoneyListItem[];
	debtPayments: MoneyListItem[];
	selfRating?: number;
	previousMonth: EstimateDraft;
	twoMonthsAgo: EstimateDraft;
}

interface OnboardingState {
	baseCurrency?: CurrencyCode;
	snapshotDay: number;
	timezone: string;
	researchConsent: boolean;
	snapshot: SnapshotDraft;
	setBaseCurrency: (baseCurrency: CurrencyCode) => void;
	setSnapshotDay: (snapshotDay: number) => void;
	setTimezone: (timezone: string) => void;
	setResearchConsent: (researchConsent: boolean) => void;
	setSnapshot: (snapshot: Partial<SnapshotDraft>) => void;
}

const EMPTY_SNAPSHOT: SnapshotDraft = {
	income: [createMoneyListItem()],
	expenses: {
		food: {},
		housing: {},
		health: {},
		clothing: {},
		restaurants: {},
		transport: {},
		other: {},
	},
	saved: {},
	reserves: [createMoneyListItem()],
	debtPayments: [createMoneyListItem()],
	previousMonth: { expensesTotal: {}, saved: {} },
	twoMonthsAgo: { expensesTotal: {}, saved: {} },
};

/** Draft of the onboarding, kept in the tab session until it is saved so a language switch or a reload doesn't lose it. */
export const useOnboardingStore = create<OnboardingState>()(
	persist(
		(set) => ({
			baseCurrency: undefined,
			snapshotDay: 31,
			timezone: "",
			researchConsent: false,
			snapshot: EMPTY_SNAPSHOT,
			setBaseCurrency: (baseCurrency) => set({ baseCurrency }),
			setSnapshotDay: (snapshotDay) => set({ snapshotDay: snapshotDay }),
			setTimezone: (timezone) => set({ timezone }),
			setResearchConsent: (researchConsent) => set({ researchConsent }),
			setSnapshot: (snapshot) =>
				set((state) => ({ snapshot: { ...state.snapshot, ...snapshot } })),
		}),
		{
			name: "onboarding",
			storage: createJSONStorage(() => sessionStorage),
			skipHydration: true,
		},
	),
);
