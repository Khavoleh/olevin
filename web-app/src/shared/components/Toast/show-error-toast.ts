"use client";

import toastQueue from "./toast-queue";

/** Shows a toast with the error message, as the API sent it. */
const showErrorToast = (message: string) =>
	toastQueue.add(message, { timeout: 8000 });

export default showErrorToast;
