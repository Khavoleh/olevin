"use client";

import { Radio, RadioGroup } from "@shared/components";
import type { SubmitCfpbResponseRequest } from "../../api/onboarding.schemas";
import { useOnboardingStore } from "../../onboarding-store";

interface AnswerFieldProps {
	answer: keyof SubmitCfpbResponseRequest;
	label: string;
	description?: string;
	errorMessage: string;
	labelVariant?: "field" | "group";
	options: { value: string; label: string }[];
	isInvalid: boolean;
	className?: string;
	optionsClassName: string;
}

/** Question of the CFPB questionnaire answered with one of its options, kept in the draft. */
const AnswerField = ({
	answer,
	label,
	description,
	errorMessage,
	labelVariant,
	options,
	isInvalid,
	className,
	optionsClassName,
}: Readonly<AnswerFieldProps>) => {
	const value = useOnboardingStore((state) => state.cfpbResponse[answer]);
	const setCfpbResponse = useOnboardingStore((state) => state.setCfpbResponse);

	return (
		<RadioGroup
			label={label}
			description={description}
			errorMessage={errorMessage}
			labelVariant={labelVariant}
			name={answer}
			value={value ?? null}
			onChange={(chosen) => setCfpbResponse({ [answer]: chosen })}
			isInvalid={isInvalid}
			isRequired
			validationBehavior="aria"
			className={`@container ${className ?? ""}`}
		>
			<div className={`grid gap-1.5 ${optionsClassName}`}>
				{options.map((option) => (
					<Radio key={option.value} value={option.value} variant="text-tile">
						{option.label}
					</Radio>
				))}
			</div>
		</RadioGroup>
	);
};

export default AnswerField;
