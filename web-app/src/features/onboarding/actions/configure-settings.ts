"use server";

import type { SubmissionResult } from "@conform-to/react";
import { parseWithZod } from "@conform-to/zod/v4";
import { PAGE_URLS } from "@shared/constants";
import { ApiError, getLanguageUrl } from "@shared/helpers";
import { redirect } from "next/navigation";
import { configureUserSettings } from "../api/onboarding";
import { OnboardingStatus } from "../api/onboarding.schemas";
import { SETTINGS_SCHEMA } from "../constants/settings-schema";
import { getOnboardingPage } from "../helpers/get-onboarding-page/get-onboarding-page";
import { syncOnboardingStatus } from "../helpers/sync-onboarding-status";

/** Saves the settings, creating the user first if they don't exist yet, and opens the next step. */
export async function configureSettings(
	_lastResult: SubmissionResult | undefined,
	formData: FormData,
): Promise<SubmissionResult> {
	const submission = parseWithZod(formData, { schema: SETTINGS_SCHEMA });

	if (submission.status !== "success") {
		return submission.reply();
	}

	const language = submission.value.locale.toLowerCase();
	let status = await syncOnboardingStatus();

	if (!status) redirect(getLanguageUrl(PAGE_URLS.SIGN_IN, language));

	if (status === OnboardingStatus.Settings) {
		const { status: code, data } = await configureUserSettings(
			submission.value,
		);

		if (code === 400) {
			return submission.reply({ formErrors: data.validationErrors ?? [] });
		}

		if (code !== 201 && code !== 409) {
			throw new ApiError(code);
		}

		status = OnboardingStatus.Onboarding;
	}

	redirect(getLanguageUrl(getOnboardingPage(status), language));
}
