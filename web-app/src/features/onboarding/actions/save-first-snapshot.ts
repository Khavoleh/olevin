"use server";

import type { SubmissionResult } from "@conform-to/react";
import { parseWithZod } from "@conform-to/zod/v4";
import { PAGE_URLS } from "@shared/constants";
import { ApiError, getLanguage, getLanguageUrl } from "@shared/helpers";
import { redirect } from "next/navigation";
import { createFirstSnapshot } from "../api/onboarding";
import type {
	FirstSnapshot,
	OnboardingStatus,
} from "../api/onboarding.schemas";
import { CreateFirstSnapshotBody } from "../api/onboarding.zod";
import { getOnboardingPage } from "../helpers/get-onboarding-page/get-onboarding-page";
import { syncOnboardingStatus } from "../helpers/sync-onboarding-status";

async function postFirstSnapshot(
	snapshot: FirstSnapshot,
): Promise<OnboardingStatus | undefined> {
	const response = await createFirstSnapshot(snapshot);

	if (response.status !== 201) {
		throw new ApiError(response);
	}

	return syncOnboardingStatus();
}

/** Saves the first snapshot with the estimates of the 2 months before it and opens the next step; a saved first snapshot is a conflict shown as an error. */
export async function saveFirstSnapshot(
	_lastResult: SubmissionResult | undefined,
	formData: FormData,
): Promise<SubmissionResult> {
	const submission = parseWithZod(formData, {
		schema: CreateFirstSnapshotBody,
	});
	const language = getLanguage(String(formData.get("language")), null);

	if (submission.status !== "success") {
		return submission.reply();
	}

	let status: OnboardingStatus | undefined;

	try {
		status = await postFirstSnapshot(submission.value);
	} catch (error) {
		if (!(error instanceof ApiError)) throw error;

		if (error.status !== 401) {
			return submission.reply({ formErrors: [error.message] });
		}
	}

	if (!status) redirect(getLanguageUrl(PAGE_URLS.SIGN_IN, language));

	redirect(getLanguageUrl(getOnboardingPage(status), language));
}
