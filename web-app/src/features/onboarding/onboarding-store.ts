import { create } from "zustand";
import { createJSONStorage, persist } from "zustand/middleware";
import type { CurrencyCode } from "./api/onboarding.schemas";

interface OnboardingState {
	baseCurrency?: CurrencyCode;
	snapshotDay: number;
	timezone: string;
	researchConsent: boolean;
	setBaseCurrency: (baseCurrency: CurrencyCode) => void;
	setSnapshotDay: (snapshotDay: number) => void;
	setTimezone: (timezone: string) => void;
	setResearchConsent: (researchConsent: boolean) => void;
}

/** Draft of the settings, kept in the tab session until it is saved so a language switch or a reload doesn't lose it. */
export const useOnboardingStore = create<OnboardingState>()(
	persist(
		(set) => ({
			baseCurrency: undefined,
			snapshotDay: 31,
			timezone: "",
			researchConsent: false,
			setBaseCurrency: (baseCurrency) => set({ baseCurrency }),
			setSnapshotDay: (snapshotDay) => set({ snapshotDay: snapshotDay }),
			setTimezone: (timezone) => set({ timezone }),
			setResearchConsent: (researchConsent) => set({ researchConsent }),
		}),
		{
			name: "onboarding",
			storage: createJSONStorage(() => sessionStorage),
			skipHydration: true,
		},
	),
);
