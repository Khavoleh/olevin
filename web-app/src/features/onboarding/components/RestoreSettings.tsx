"use client";

import { useEffect } from "react";
import { getOnboardingSettings } from "../actions/get-onboarding-settings";
import { useOnboardingStore } from "../onboarding-store";

interface RestoreSettingsProps {
	language: string;
}

/** Puts the saved base currency, snapshot day and time zone into memory when they are missing there, such as after a reload. */
const RestoreSettings = ({ language }: Readonly<RestoreSettingsProps>) => {
	const baseCurrency = useOnboardingStore((state) => state.baseCurrency);

	useEffect(() => {
		if (!baseCurrency) {
			void getOnboardingSettings(language).then((settings) => {
				if (settings) useOnboardingStore.setState(settings);
			});
		}
	}, [baseCurrency, language]);

	return null;
};

export default RestoreSettings;
