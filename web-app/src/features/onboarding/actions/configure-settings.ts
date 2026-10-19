"use server";

import type { SubmissionResult } from "@conform-to/react";
import { parseWithZod } from "@conform-to/zod/v4";
import { PAGE_URLS } from "@shared/constants";
import { ApiError, getLanguageUrl } from "@shared/helpers";
import { redirect } from "next/navigation";
import { configureUserSettings } from "../api/onboarding";
import {
	type ConfigureUserSettingsRequest,
	OnboardingStatus,
} from "../api/onboarding.schemas";
import { SETTINGS_SCHEMA } from "../constants/settings-schema";
import { getOnboardingPage } from "../helpers/get-onboarding-page/get-onboarding-page";
import { syncOnboardingStatus } from "../helpers/sync-onboarding-status";

async function saveSettings(
	settings: ConfigureUserSettingsRequest,
): Promise<OnboardingStatus | undefined> {
	if (!(await syncOnboardingStatus())) return undefined;

	const response = await configureUserSettings(settings);

	if (response.status !== 201) {
		throw new ApiError(response);
	}

	return OnboardingStatus.Onboarding;
}

/** Saves the settings, creating the user first if they don't exist yet, and opens the next step; settings saved before are a conflict shown as an error. */
export async function configureSettings(
	_lastResult: SubmissionResult | undefined,
	formData: FormData,
): Promise<SubmissionResult> {
	const submission = parseWithZod(formData, { schema: SETTINGS_SCHEMA });

	if (submission.status !== "success") {
		return submission.reply();
	}

	const language = submission.value.locale.toLowerCase();
	let status: OnboardingStatus | undefined;

	try {
		status = await saveSettings(submission.value);
	} catch (error) {
		if (error instanceof ApiError) {
			return submission.reply({ formErrors: [error.message] });
		}

		throw error;
	}

	if (!status) redirect(getLanguageUrl(PAGE_URLS.SIGN_IN, language));

	redirect(getLanguageUrl(getOnboardingPage(status), language));
}
