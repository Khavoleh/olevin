"use client";

import { Button, DisabledButton } from "@shared/components";
import { getI18n } from "@shared/helpers";
import type { ReactNode } from "react";
import { SUBMIT_BUTTON_I18N } from "../i18n/submit-button-i18n";

interface SubmitButtonProps {
	language: string;
	isComplete: boolean;
	isPending?: boolean;
	children: ReactNode;
}

/** Button that finishes a step; until the form is complete it is disabled with a tooltip of why. */
const SubmitButton = ({
	language,
	isComplete,
	isPending,
	children,
}: Readonly<SubmitButtonProps>) => {
	const t = getI18n(language, SUBMIT_BUTTON_I18N);

	if (isComplete) {
		return (
			<Button
				type="submit"
				isPending={isPending}
				className="md:min-w-65 md:justify-self-start"
			>
				{children}
			</Button>
		);
	}

	return (
		<DisabledButton
			hint={t("incomplete")}
			className="inline-flex min-h-11 items-center justify-center rounded-md bg-primary px-4.5 font-semibold text-on-primary opacity-45 md:min-w-65 md:justify-self-start"
		>
			{children}
		</DisabledButton>
	);
};

export default SubmitButton;
