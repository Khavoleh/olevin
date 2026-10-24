import type { SubmissionResult } from "@conform-to/react";
import { parseWithZod } from "@conform-to/zod/v4";
import { requestOrRedirect } from "@shared/api";
import { PAGE_URLS } from "@shared/constants";
import { ApiError, getLanguage, getLanguageUrl } from "@shared/helpers";
import { redirect } from "next/navigation";
import type { z } from "zod";

/** Sends the valid form to the API and opens the history; a failed response becomes the form error. */
export async function submitSnapshotForm<Schema extends z.ZodType>(
	formData: FormData,
	schema: Schema,
	send: (
		value: z.output<Schema>,
	) => Promise<{ status: number; data?: unknown }>,
): Promise<SubmissionResult> {
	const submission = parseWithZod(formData, { schema });
	const language = getLanguage(String(formData.get("language")), null);

	if (submission.status !== "success") return submission.reply();

	try {
		await requestOrRedirect(async () => {
			const response = await send(submission.value);

			if (response.status >= 300) throw new ApiError(response);
		}, language);
	} catch (error) {
		if (!(error instanceof ApiError)) throw error;

		return submission.reply({ formErrors: [error.message] });
	}

	redirect(getLanguageUrl(PAGE_URLS.SNAPSHOTS, language));
}
