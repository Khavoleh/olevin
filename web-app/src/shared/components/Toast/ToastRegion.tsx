"use client";

import { WarningCircleIcon, XIcon } from "@phosphor-icons/react";
import { getI18n } from "@shared/helpers";
import {
	UNSTABLE_ToastRegion as AriaToastRegion,
	Button,
	Text,
	UNSTABLE_Toast as Toast,
	UNSTABLE_ToastContent as ToastContent,
} from "react-aria-components";
import { TOAST_I18N } from "./toast-i18n";
import toastQueue from "./toast-queue";

interface ToastRegionProps {
	language: string;
}

/** Corner of the screen where the toasts of the app appear; mounted once in the root layout. */
const ToastRegion = ({ language }: Readonly<ToastRegionProps>) => {
	const t = getI18n(language, TOAST_I18N);

	return (
		<AriaToastRegion
			queue={toastQueue}
			className="fixed inset-x-4 bottom-[max(1rem,env(safe-area-inset-bottom))] z-50 flex flex-col-reverse gap-2 rounded-lg md:left-auto md:w-96"
		>
			{({ toast }) => (
				<Toast
					toast={toast}
					className="flex items-start gap-3 rounded-lg border border-danger-border bg-danger-soft p-3 text-danger shadow-overlay"
				>
					<WarningCircleIcon size={20} className="mt-px flex-none" />
					<ToastContent className="min-w-0 flex-1">
						<Text slot="title" className="text-sm">
							{toast.content}
						</Text>
					</ToastContent>
					<Button
						slot="close"
						aria-label={t("close")}
						className="flex size-6 flex-none cursor-pointer items-center justify-center rounded-sm transition-colors hover:bg-danger-border"
					>
						<XIcon size={16} />
					</Button>
				</Toast>
			)}
		</AriaToastRegion>
	);
};

export default ToastRegion;
