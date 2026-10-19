"use client";

import { UNSTABLE_ToastQueue as ToastQueue } from "react-aria-components";
import { flushSync } from "react-dom";

/** Queue of the toasts of the app; updates run in a view transition so toasts fade in and out. */
const toastQueue = new ToastQueue<string>({
	maxVisibleToasts: 3,
	wrapUpdate: (update) => {
		if (!("startViewTransition" in document)) {
			update();
			return;
		}

		document.startViewTransition(() => flushSync(update));
	},
});

export default toastQueue;
