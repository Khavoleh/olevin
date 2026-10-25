"use client";

import { useEffect } from "react";
import { clearOnboardingDraft } from "../onboarding-store";

/** Clears the onboarding draft once the user is past the onboarding. */
const ClearOnboardingDraft = () => {
	useEffect(() => {
		clearOnboardingDraft();
	}, []);

	return null;
};

export default ClearOnboardingDraft;
