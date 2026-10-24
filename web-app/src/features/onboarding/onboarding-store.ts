import {
	createEstimateDraft,
	createSnapshotDraft,
	type EstimateDraft,
	type SnapshotDraft,
} from "@widgets/snapshot-fields";
import { create } from "zustand";
import { createJSONStorage, persist } from "zustand/middleware";
import type {
	CurrencyCode,
	SubmitCfpbResponseRequest,
} from "./api/onboarding.schemas";

/** First snapshot with the estimates of the 2 months before it. */
export interface FirstSnapshotDraft extends SnapshotDraft {
	previousMonth: EstimateDraft;
	twoMonthsAgo: EstimateDraft;
}

/** Chosen options of the CFPB questionnaire, as the radio values. */
export type CfpbDraft = Partial<
	Record<keyof SubmitCfpbResponseRequest, string>
>;

interface OnboardingState {
	baseCurrency?: CurrencyCode;
	snapshotDay: number;
	timezone: string;
	researchConsent: boolean;
	snapshot: FirstSnapshotDraft;
	cfpbResponse: CfpbDraft;
	setBaseCurrency: (baseCurrency: CurrencyCode) => void;
	setSnapshotDay: (snapshotDay: number) => void;
	setTimezone: (timezone: string) => void;
	setResearchConsent: (researchConsent: boolean) => void;
	setSnapshot: (snapshot: Partial<FirstSnapshotDraft>) => void;
	setCfpbResponse: (cfpbResponse: CfpbDraft) => void;
}

const EMPTY_SNAPSHOT: FirstSnapshotDraft = {
	...createSnapshotDraft(),
	previousMonth: createEstimateDraft(),
	twoMonthsAgo: createEstimateDraft(),
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
			cfpbResponse: {},
			setBaseCurrency: (baseCurrency) => set({ baseCurrency }),
			setSnapshotDay: (snapshotDay) => set({ snapshotDay: snapshotDay }),
			setTimezone: (timezone) => set({ timezone }),
			setResearchConsent: (researchConsent) => set({ researchConsent }),
			setSnapshot: (snapshot) =>
				set((state) => ({ snapshot: { ...state.snapshot, ...snapshot } })),
			setCfpbResponse: (cfpbResponse) =>
				set((state) => ({
					cfpbResponse: { ...state.cfpbResponse, ...cfpbResponse },
				})),
		}),
		{
			name: "onboarding",
			storage: createJSONStorage(() => sessionStorage),
			skipHydration: true,
		},
	),
);
