"use server";

import type { SubmissionResult } from "@conform-to/react";
import { parseWithZod } from "@conform-to/zod/v4";
import { PAGE_URLS } from "@shared/constants";
import { ApiError, getLanguage, getLanguageUrl } from "@shared/helpers";
import { redirect } from "next/navigation";
import { submitCfpbResponse } from "../api/onboarding";
import type {
	OnboardingStatus,
	SubmitCfpbResponseRequest,
} from "../api/onboarding.schemas";
import { CFPB_STEP_SCHEMA } from "../constants/cfpb-step-schema";
import { getOnboardingPage } from "../helpers/get-onboarding-page/get-onboarding-page";
import { syncOnboardingStatus } from "../helpers/sync-onboarding-status";

/** Answers of a user who has no questionnaire to fill in: not consented, without settings, or answered before. */
const SKIPPED_STATUSES = [403, 404, 409];

async function postCfpbResponse(
	answers: SubmitCfpbResponseRequest,
): Promise<OnboardingStatus | undefined> {
	const response = await submitCfpbResponse(answers);

	if (response.status !== 201 && !SKIPPED_STATUSES.includes(response.status)) {
		throw new ApiError(response);
	}

	return syncOnboardingStatus();
}

/** Saves the CFPB answers and opens the page the user continues from; a user with no questionnaire to fill in is sent there too. */
export async function saveCfpbResponse(
	_lastResult: SubmissionResult | undefined,
	formData: FormData,
): Promise<SubmissionResult> {
	const submission = parseWithZod(formData, { schema: CFPB_STEP_SCHEMA });
	const language = getLanguage(String(formData.get("language")), null);

	if (submission.status !== "success") {
		return submission.reply();
	}

	let status: OnboardingStatus | undefined;

	try {
		status = await postCfpbResponse(submission.value);
	} catch (error) {
		if (!(error instanceof ApiError)) throw error;

		if (error.status !== 401) {
			return submission.reply({ formErrors: [error.message] });
		}
	}

	if (!status) redirect(getLanguageUrl(PAGE_URLS.SIGN_IN, language));

	redirect(getLanguageUrl(getOnboardingPage(status), language));
}
