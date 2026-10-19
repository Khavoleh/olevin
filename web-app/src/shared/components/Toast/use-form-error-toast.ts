"use client";

import type { SubmissionResult } from "@conform-to/react";
import { useEffect } from "react";
import showErrorToast from "./show-error-toast";

/** Shows the form error of a Server Action result, the message of a failed API response, as a toast. */
const useFormErrorToast = (lastResult: SubmissionResult | undefined) => {
	useEffect(() => {
		const message = lastResult?.error?.[""]?.join(", ");

		if (message) showErrorToast(message);
	}, [lastResult]);
};

export default useFormErrorToast;
