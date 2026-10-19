"use client";

import { useEffect } from "react";
import { getSavedBaseCurrency } from "../actions/get-saved-base-currency";
import { useOnboardingStore } from "../onboarding-store";

interface RestoreBaseCurrencyProps {
	language: string;
}

/** Puts the saved base currency into memory when it is missing there, such as after a reload. */
const RestoreBaseCurrency = ({
	language,
}: Readonly<RestoreBaseCurrencyProps>) => {
	const baseCurrency = useOnboardingStore((state) => state.baseCurrency);
	const setBaseCurrency = useOnboardingStore((state) => state.setBaseCurrency);

	useEffect(() => {
		if (!baseCurrency) {
			void getSavedBaseCurrency(language).then(setBaseCurrency);
		}
	}, [baseCurrency, language, setBaseCurrency]);

	return null;
};

export default RestoreBaseCurrency;
